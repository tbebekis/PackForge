// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Generates Debian package build scripts.
/// </summary>
static public class DebScriptGenerator
{
    // ● private
    static string R(PublisherProjectSettings Project, string Text) => PublisherProjectPatterns.ResolveBuild(Text, Project);
    static string Q(string Text) => "'" + (Text ?? string.Empty).Replace("'", "'\"'\"'") + "'";
    static string DebDescription(string Text)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return " .";

        StringBuilder Builder = new();
        string[] Lines = Text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        foreach (string Line in Lines)
        {
            if (string.IsNullOrWhiteSpace(Line))
                Builder.AppendLine(" .");
            else
                Builder.AppendLine(" " + Line);
        }
        return Builder.ToString().TrimEnd();
    }
    static void MakeExecutable(string FilePath)
    {
        if (OperatingSystem.IsWindows())
            return;

        File.SetUnixFileMode(
            FilePath,
            UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
            UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
            UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
    }
    static void EnsureFolderCanBeCreated(string FolderPath, string FieldTitle)
    {
        if (string.IsNullOrWhiteSpace(FolderPath))
            throw new Exception(FieldTitle + " is required.");
        if (File.Exists(FolderPath))
            throw new Exception(FieldTitle + " points to an existing file, not a folder." + Environment.NewLine + FolderPath);
        Directory.CreateDirectory(FolderPath);
    }

    // ● public
    /// <summary>
    /// Generates the Debian build script and returns its file path.
    /// </summary>
    static public string Generate(PublisherProjectSettings Project)
    {
        if (Project == null)
            throw new Exception("Project settings are null.");
        if (!Project.Deb.IsEnabled)
            throw new Exception("Debian generation is disabled.");

        string PublishOutputFolder = PublisherProjectPatterns.ResolvePublishOutputFolder(Project, Project.LinuxPublish);
        if (string.IsNullOrWhiteSpace(PublishOutputFolder))
            throw new Exception("Linux publish output folder is required.");

        EnsureFolderCanBeCreated(PublishOutputFolder, "Linux publish output folder");

        string AppName = R(Project, Project.AppName);
        string PackageName = R(Project, Project.PackageName);
        string Version = R(Project, Project.Version);
        string Architecture = R(Project, Project.Deb.Architecture);
        string ExecutableName = R(Project, Project.Deb.ExecutableName);
        string CommandName = R(Project, Project.Deb.CommandName);
        string InstallDir = R(Project, Project.Deb.InstallDir);
        string ScriptFileName = R(Project, Project.Deb.ScriptFileName);
        string IconFilePath = R(Project, Project.LinuxIconFilePath);
        string OutputFileName = R(Project, Project.Deb.OutputFileNamePattern);
        string BuildOutputFolder = R(Project, Project.Deb.BuildOutputFolder);
        if (string.IsNullOrWhiteSpace(BuildOutputFolder))
            BuildOutputFolder = "Output";
        string ScriptFilePath = Path.Combine(PublishOutputFolder, ScriptFileName);

        if (string.IsNullOrWhiteSpace(ScriptFileName))
            throw new Exception("Debian script file name is required.");

        StringBuilder Builder = new();
        Builder.AppendLine("#!/usr/bin/env bash");
        Builder.AppendLine("set -euo pipefail");
        Builder.AppendLine();
        Builder.AppendLine("APP_NAME=" + Q(AppName));
        Builder.AppendLine("PACKAGE_NAME=" + Q(PackageName));
        Builder.AppendLine("VERSION=" + Q(Version));
        Builder.AppendLine("ARCHITECTURE=" + Q(Architecture));
        Builder.AppendLine("EXECUTABLE_NAME=" + Q(ExecutableName));
        Builder.AppendLine("COMMAND_NAME=" + Q(CommandName));
        Builder.AppendLine("SCRIPT_FILE_NAME=" + Q(ScriptFileName));
        Builder.AppendLine("MAINTAINER=" + Q(R(Project, Project.Maintainer)));
        Builder.AppendLine("HOMEPAGE=" + Q(R(Project, Project.Homepage)));
        Builder.AppendLine("DESCRIPTION_SHORT=" + Q(R(Project, Project.DescriptionShort)));
        Builder.AppendLine("DEPENDENCIES=" + Q(R(Project, Project.Deb.Dependencies)));
        Builder.AppendLine("INSTALL_DIR=" + Q(InstallDir));
        Builder.AppendLine("SECTION=" + Q(R(Project, Project.Deb.Section)));
        Builder.AppendLine("PRIORITY=" + Q(R(Project, Project.Deb.Priority)));
        Builder.AppendLine("DESKTOP_CATEGORIES=" + Q(R(Project, Project.Deb.DesktopCategories)));
        Builder.AppendLine("DESKTOP_KEYWORDS=" + Q(R(Project, Project.Deb.DesktopKeywords)));
        Builder.AppendLine("SOURCE_DIR=\"$(cd \"$(dirname \"${BASH_SOURCE[0]}\")\" && pwd)\"");
        Builder.AppendLine("ICON_FILE=" + Q(IconFilePath));
        Builder.AppendLine("OUTPUT_DIR=" + Q(BuildOutputFolder));
        Builder.AppendLine("OUTPUT_FILE_NAME=" + Q(OutputFileName));
        Builder.AppendLine();
        Builder.AppendLine("command -v dpkg-deb >/dev/null 2>&1 || { echo \"dpkg-deb is required.\" >&2; exit 1; }");
        Builder.AppendLine("[ -d \"$SOURCE_DIR\" ] || { echo \"Source folder not found: $SOURCE_DIR\" >&2; exit 1; }");
        Builder.AppendLine("[ -f \"$SOURCE_DIR/$EXECUTABLE_NAME\" ] || { echo \"Executable not found: $SOURCE_DIR/$EXECUTABLE_NAME\" >&2; exit 1; }");
        Builder.AppendLine("if [ -n \"$ICON_FILE\" ] && [[ \"$ICON_FILE\" != /* ]]; then ICON_FILE=\"$SOURCE_DIR/$ICON_FILE\"; fi");
        Builder.AppendLine("if [[ \"$OUTPUT_DIR\" != /* ]]; then OUTPUT_DIR=\"$SOURCE_DIR/$OUTPUT_DIR\"; fi");
        Builder.AppendLine("if [ -n \"$ICON_FILE\" ] && [ ! -f \"$ICON_FILE\" ]; then echo \"Icon file not found: $ICON_FILE\" >&2; exit 1; fi");
        Builder.AppendLine();
        Builder.AppendLine("BUILD_ROOT=$(mktemp -d)");
        Builder.AppendLine("trap 'rm -rf \"$BUILD_ROOT\"' EXIT");
        Builder.AppendLine("mkdir -p \"$BUILD_ROOT/DEBIAN\"");
        Builder.AppendLine("mkdir -p \"$BUILD_ROOT$INSTALL_DIR\"");
        Builder.AppendLine("mkdir -p \"$BUILD_ROOT/usr/bin\"");
        Builder.AppendLine("mkdir -p \"$BUILD_ROOT/usr/share/applications\"");
        Builder.AppendLine("mkdir -p \"$BUILD_ROOT/usr/share/icons/hicolor/512x512/apps\"");
        Builder.AppendLine("mkdir -p \"$OUTPUT_DIR\"");
        Builder.AppendLine();
        Builder.AppendLine("shopt -s dotglob nullglob");
        Builder.AppendLine("for ITEM in \"$SOURCE_DIR\"/*; do");
        Builder.AppendLine("  BASE_NAME=$(basename \"$ITEM\")");
        Builder.AppendLine("  case \"$BASE_NAME\" in");
        Builder.AppendLine("    \"$SCRIPT_FILE_NAME\"|\"$OUTPUT_FILE_NAME\"|*.deb|*.tar.gz|*.zip) continue ;;");
        Builder.AppendLine("  esac");
        Builder.AppendLine("  cp -a \"$ITEM\" \"$BUILD_ROOT$INSTALL_DIR/\"");
        Builder.AppendLine("done");
        Builder.AppendLine("chmod +x \"$BUILD_ROOT$INSTALL_DIR/$EXECUTABLE_NAME\"");
        Builder.AppendLine();
        Builder.AppendLine("cat > \"$BUILD_ROOT/usr/bin/$COMMAND_NAME\" <<EOF");
        Builder.AppendLine("#!/usr/bin/env bash");
        Builder.AppendLine("exec \"$INSTALL_DIR/$EXECUTABLE_NAME\" \"\\$@\"");
        Builder.AppendLine("EOF");
        Builder.AppendLine("chmod +x \"$BUILD_ROOT/usr/bin/$COMMAND_NAME\"");
        Builder.AppendLine();
        Builder.AppendLine("cat > \"$BUILD_ROOT/DEBIAN/control\" <<EOF");
        Builder.AppendLine("Package: $PACKAGE_NAME");
        Builder.AppendLine("Version: $VERSION");
        Builder.AppendLine("Section: $SECTION");
        Builder.AppendLine("Priority: $PRIORITY");
        Builder.AppendLine("Architecture: $ARCHITECTURE");
        Builder.AppendLine("Maintainer: $MAINTAINER");
        Builder.AppendLine("Homepage: $HOMEPAGE");
        Builder.AppendLine("Depends: $DEPENDENCIES");
        Builder.AppendLine("Description: $DESCRIPTION_SHORT");
        Builder.AppendLine(DebDescription(R(Project, Project.DescriptionLong)));
        Builder.AppendLine("EOF");
        Builder.AppendLine();
        Builder.AppendLine("cat > \"$BUILD_ROOT/usr/share/applications/$PACKAGE_NAME.desktop\" <<EOF");
        Builder.AppendLine("[Desktop Entry]");
        Builder.AppendLine("Type=Application");
        Builder.AppendLine("Name=$APP_NAME");
        Builder.AppendLine("Comment=$DESCRIPTION_SHORT");
        Builder.AppendLine("Exec=$COMMAND_NAME");
        Builder.AppendLine("Terminal=false");
        Builder.AppendLine("Categories=$DESKTOP_CATEGORIES");
        Builder.AppendLine("Keywords=$DESKTOP_KEYWORDS");
        Builder.AppendLine("EOF");
        Builder.AppendLine();
        Builder.AppendLine("if [ -n \"$ICON_FILE\" ]; then");
        Builder.AppendLine("  cp \"$ICON_FILE\" \"$BUILD_ROOT/usr/share/icons/hicolor/512x512/apps/$PACKAGE_NAME.png\"");
        Builder.AppendLine("  printf 'Icon=%s\\n' \"$PACKAGE_NAME\" >> \"$BUILD_ROOT/usr/share/applications/$PACKAGE_NAME.desktop\"");
        Builder.AppendLine("fi");
        Builder.AppendLine();
        Builder.AppendLine("dpkg-deb --build \"$BUILD_ROOT\" \"$OUTPUT_DIR/$OUTPUT_FILE_NAME\"");
        Builder.AppendLine("echo \"Created: $OUTPUT_DIR/$OUTPUT_FILE_NAME\"");

        File.WriteAllText(ScriptFilePath, Builder.ToString(), new UTF8Encoding(false));
        MakeExecutable(ScriptFilePath);
        return ScriptFilePath;
    }
}
