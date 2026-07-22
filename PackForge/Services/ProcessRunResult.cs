// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Contains the result of a process run.
/// </summary>
public class ProcessRunResult
{
    // ● properties
    /// <summary>
    /// Gets or sets the process exit code.
    /// </summary>
    public int ExitCode { get; set; }
    /// <summary>
    /// Gets or sets the captured standard output text.
    /// </summary>
    public string OutputText { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the captured standard error text.
    /// </summary>
    public string ErrorText { get; set; } = string.Empty;
}
