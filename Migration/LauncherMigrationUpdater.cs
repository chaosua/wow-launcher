using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

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

        var psScript = $$"""
                         while (Get-Process -Name '{{nameNoExt}}' -ErrorAction SilentlyContinue) { Start-Sleep -Seconds 1 }
                         Remove-Item -LiteralPath '{{oldPath}}' -Force
                         Move-Item -LiteralPath '{{newPath}}' -Destination '{{oldPath}}'
                         Start-Process -FilePath '{{oldPath}}'
                         """;
        
        var arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"& {{ {psScript} }}\"";

        var psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process.Start(psi);
    }
}