// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Loads and saves PackForge project settings as JSON files.
/// </summary>
public class PublisherProjectStore
{
    // ● private fields
    ObservableCollection<PublisherProjectSettings> fItems;

    // ● private
    string CreateFileName(string Name)
    {
        string Result = Sys.StrToValidFileName(Name).Trim();
        if (string.IsNullOrWhiteSpace(Result))
            throw new Exception("Project name cannot be converted to a file name.");
        return Result + ".json";
    }
    void Normalize(PublisherProjectSettings Item)
    {
        if (Item.LinuxPublish == null)
            Item.LinuxPublish = new() { RuntimeIdentifier = "linux-x64" };
        if (Item.WindowsPublish == null)
            Item.WindowsPublish = new() { RuntimeIdentifier = "win-x64" };
        if (Item.Deb == null)
            Item.Deb = new();
        if (Item.Inno == null)
            Item.Inno = new();
        if (string.IsNullOrWhiteSpace(Item.LinuxIconFilePath) && !string.IsNullOrWhiteSpace(Item.IconFilePath))
            Item.LinuxIconFilePath = Item.IconFilePath;
        if (string.IsNullOrWhiteSpace(Item.WindowsIconFilePath) && !string.IsNullOrWhiteSpace(Item.IconFilePath))
            Item.WindowsIconFilePath = Item.IconFilePath;
        if (string.IsNullOrWhiteSpace(Item.Inno.InstallScope))
            Item.Inno.InstallScope = Item.Inno.DefaultDirName.StartsWith("{autopf}", StringComparison.OrdinalIgnoreCase) ? "All Users" : "User only";
        if (string.IsNullOrWhiteSpace(Item.Inno.DefaultDirName))
            Item.Inno.DefaultDirName = Item.Inno.InstallScope.IsSameText("All Users") ? "{autopf}\\{ProjectName}" : "{localappdata}\\Programs\\{ProjectName}";
        if (string.IsNullOrWhiteSpace(Item.AppId))
            Item.AppId = Guid.NewGuid().ToString("B").ToUpperInvariant();
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherProjectStore"/> class.
    /// </summary>
    public PublisherProjectStore()
    {
    }

    // ● public
    /// <summary>
    /// Loads all project settings from the project folder.
    /// </summary>
    public void Load()
    {
        Items.Clear();
        Directory.CreateDirectory(FolderPath);

        foreach (string FilePath in Directory.GetFiles(FolderPath, "*.json").OrderBy(x => x))
        {
            PublisherProjectSettings Item = Json.LoadFromFile(typeof(PublisherProjectSettings), FilePath) as PublisherProjectSettings;
            if (Item != null)
            {
                if (string.IsNullOrWhiteSpace(Item.Name))
                    Item.Name = Path.GetFileNameWithoutExtension(FilePath);
                Normalize(Item);
                Item.StoredName = Item.Name;
                Items.Add(Item);
            }
        }
    }
    /// <summary>
    /// Saves project settings to disk and updates the in-memory collection.
    /// </summary>
    public void Save(PublisherProjectSettings Item)
    {
        if (Item == null)
            throw new Exception("Project settings are null.");

        ValidateName(Item.Name);
        Normalize(Item);

        string OldName = Item.StoredName ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(OldName) && !Item.Name.IsSameText(OldName))
        {
            string OldFilePath = GetFilePath(OldName);
            if (File.Exists(OldFilePath))
                File.Delete(OldFilePath);
        }

        Json.SaveToFile(Item, GetFilePath(Item.Name));
        Item.StoredName = Item.Name;

        PublisherProjectSettings Existing = Find(Item.Name);
        if (Existing == null)
            Items.Add(Item);
    }
    /// <summary>
    /// Deletes a project by name from disk and memory.
    /// </summary>
    public bool Delete(string Name)
    {
        bool Result = false;
        string FilePath = GetFilePath(Name);

        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
            Result = true;
        }

        PublisherProjectSettings Item = Find(Name);
        if (Item != null)
        {
            Items.Remove(Item);
            Result = true;
        }

        return Result;
    }
    /// <summary>
    /// Finds a project by name.
    /// </summary>
    public PublisherProjectSettings Find(string Name)
    {
        foreach (PublisherProjectSettings Item in Items)
            if (Item.Name.IsSameText(Name))
                return Item;
        return null;
    }
    /// <summary>
    /// Validates a project name for file persistence.
    /// </summary>
    public void ValidateName(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new Exception("Project name is required.");
        if (!Name.IsSameText(Name.Trim()))
            throw new Exception("Project name cannot start or end with spaces.");
        if (!Sys.IsValidFileName(Name))
            throw new Exception("Project name contains invalid file name characters.");
        CreateFileName(Name);
    }
    /// <summary>
    /// Returns the full JSON file path for a project name.
    /// </summary>
    public string GetFilePath(string Name) => Path.Combine(FolderPath, CreateFileName(Name));

    // ● properties
    /// <summary>
    /// Gets the folder where project JSON files are stored.
    /// </summary>
    public string FolderPath => Path.Combine(SysConfig.AppDataFolderPath, "Projects");
    /// <summary>
    /// Gets or sets the loaded project settings.
    /// </summary>
    public ObservableCollection<PublisherProjectSettings> Items
    {
        get
        {
            if (fItems == null)
                fItems = new();
            return fItems;
        }
        set
        {
            fItems = value;
        }
    }
}
