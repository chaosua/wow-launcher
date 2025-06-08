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

        var tempExe = newExePath;
        var originalExe = currentExe;

        string cmd = $@"/C ping -n 2 127.0.0.1 >nul & del /F /Q ""{originalExe}"" & move /Y ""{tempExe}"" ""{originalExe}"" & start """" ""{originalExe}""";

        var psi = new ProcessStartInfo("cmd.exe", cmd)
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true
        };

        Process.Start(psi);
    }
}