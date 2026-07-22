// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Represents one project validation issue.
/// </summary>
public class ValidationIssue
{
    // ● public
    /// <summary>
    /// Returns a readable validation issue line.
    /// </summary>
    public override string ToString() => $"{Severity} {Code}: {Message}";

    // ● properties
    /// <summary>
    /// Gets or sets the issue severity.
    /// </summary>
    public string Severity { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the issue code.
    /// </summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the issue message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the related file or folder path.
    /// </summary>
    public string Path { get; set; } = string.Empty;
}
