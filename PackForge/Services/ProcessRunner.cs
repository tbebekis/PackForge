// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Runs external processes and captures output.
/// </summary>
public class ProcessRunner
{
    // ● public
    /// <summary>
    /// Runs a process asynchronously.
    /// </summary>
    public async Task<ProcessRunResult> RunAsync(string FileName, string Arguments, string WorkingDirectory)
    {
        ProcessRunResult Result = new();
        StringBuilder Output = new();
        StringBuilder Error = new();

        ProcessStartInfo StartInfo = new()
        {
            FileName = FileName,
            Arguments = Arguments ?? string.Empty,
            WorkingDirectory = string.IsNullOrWhiteSpace(WorkingDirectory) ? Environment.CurrentDirectory : WorkingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using Process Process = new() { StartInfo = StartInfo, EnableRaisingEvents = true };
        Process.OutputDataReceived += (Sender, Args) =>
        {
            if (Args.Data != null)
            {
                Output.AppendLine(Args.Data);
                AppHost.Log(Args.Data);
            }
        };
        Process.ErrorDataReceived += (Sender, Args) =>
        {
            if (Args.Data != null)
            {
                Error.AppendLine(Args.Data);
                AppHost.Log(Args.Data);
            }
        };

        AppHost.Log($"{FileName} {Arguments}");
        Process.Start();
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();
        await Process.WaitForExitAsync();

        Result.ExitCode = Process.ExitCode;
        Result.OutputText = Output.ToString();
        Result.ErrorText = Error.ToString();
        AppHost.Log($"Exit code: {Result.ExitCode}");
        return Result;
    }
}
