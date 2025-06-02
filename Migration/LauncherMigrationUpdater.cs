using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wow_launcher_cs.Migration;

public class LauncherMigrationUpdater
{
    private readonly HttpClient _httpClient = new();
    private const string LauncherDownloadUrl = "https://freedom-wow.in.ua/freedom-launcher.exe";
    
    ~LauncherMigrationUpdater() => _httpClient.Dispose();

    public async Task<string> DownloadUpdateAsync()
    {
        var response = await _httpClient.GetAsync(LauncherDownloadUrl);
        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var tempDir = Path.GetTempPath();
        var exePath = Path.Combine(tempDir, "freedom-launcher-update.exe");

        File.WriteAllBytes(exePath, bytes);
        return exePath;
    }

    public void PrepareUpdaterScript(string newExePath)
    {
        var currentExe = Assembly.GetEntryAssembly()?.Location
                         ?? Process.GetCurrentProcess().MainModule!.FileName;

        var nameNoExt = Path.GetFileNameWithoutExtension(currentExe);

        var oldEscaped = currentExe.Replace("'", "''");
        var newEscaped = newExePath.Replace("'", "''");

        var oldPath = $"{oldEscaped}";
        var newPath = $"{newEscaped}";

        var lines = new[]
        {
            "@echo off",
            "setlocal",
            $"set \"PROC={nameNoExt}.exe\"",
            $"set \"OLD={oldPath}\"",
            $"set \"NEW={newPath}\"",
            "taskkill /IM \"%PROC%\" /F 2>nul",
            ":wait_process",
            "tasklist /FI \"IMAGENAME eq %PROC%\" 2>nul | find /I \"%PROC%\" >nul",
            "if not errorlevel 1 ( ping -n 2 127.0.0.1 >nul & goto wait_process )",
            "del /F \"%OLD%\"",
            "move \"%NEW%\" \"%OLD%\"",
            "start \"\" \"%OLD%\"",
            "endlocal",
            "exit /B"
        };

        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            UseShellExecute = false,
            RedirectStandardInput = true,
            CreateNoWindow = true
        };

        var proc = Process.Start(psi)!;
        using var sw = proc.StandardInput;
        foreach (var line in lines)
            sw.WriteLine(line);
    }
}