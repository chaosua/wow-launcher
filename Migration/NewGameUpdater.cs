using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace wow_launcher_cs.Migration;

public class NewGameUpdater
{
    private const string UpdatesApiUrl = "https://api.freedom-wow.in.ua/launcher/updates";
    private const string DownloadUrl = "https://freedom-wow.in.ua/uploads/";
    
    private readonly HttpClient _httpClient = new();

    ~NewGameUpdater() => _httpClient.Dispose();
    
    public event EventHandler<ProgressEventArgs> UpdateProgress;

    public async Task RunAsync()
    {
        var actions = await FetchUpdatesAsync();
        var total = actions.Length;

        for (var i = 0; i < total; i++)
        {
            var action = actions[i];
            var fileWeight = 1f / total;
            var baseProgress = i * fileWeight * 100f;
            
            await ProcessActionAsync(action, async currentFileProgress =>
            {
                var combined = baseProgress + currentFileProgress * fileWeight;
                OnUpdateProgress(new ProgressEventArgs
                {
                    Progress = combined,
                    Current = i + 1,
                    Total = total
                });
                await Task.CompletedTask;
            });
        }
        
        OnUpdateProgress(new ProgressEventArgs { Progress = 100f, Current = 0, Total = 0 });
    }
    
    private async Task<UpdateAction[]> FetchUpdatesAsync()
    {
        var resp = await _httpClient.GetAsync(UpdatesApiUrl);
        resp.EnsureSuccessStatusCode();

        var raw = await resp.Content.ReadAsStringAsync();
        var serializer = new JavaScriptSerializer();
        serializer.RegisterConverters([new UpdateActionConverter()]);
        return serializer.Deserialize<UpdateAction[]>(raw) ?? [];
    }
    
    private async Task ProcessActionAsync(UpdateAction action, Func<float, Task> progressCallback)
    {
        var actionNeeded = await CheckActionAsync(action);
        switch (actionNeeded)
        {
            case FileActionNeeded.Download:
            case FileActionNeeded.Update:
                await DownloadFileAsync(action, progressCallback);
                break;
            case FileActionNeeded.Delete:
                DeleteFile(action);
                break;
            case FileActionNeeded.NoAction:
            default:
                break;
        }
    }
    
    private async Task<FileActionNeeded> CheckActionAsync(UpdateAction action)
    {
        var path = action.LocalFilePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        switch (action.ActionType)
        {
            case 0:
                if (!File.Exists(path))
                    return FileActionNeeded.Download;

                var currentHash = await ComputeHashAsync(path);
                if (string.IsNullOrEmpty(action.Hash))
                    throw new Exception("Missing hash for update action");

                return currentHash == action.Hash
                    ? FileActionNeeded.NoAction
                    : FileActionNeeded.Update;
            case 1:
                return File.Exists(path)
                    ? FileActionNeeded.Delete
                    : FileActionNeeded.NoAction;
            default:
                throw new Exception("Invalid action type");
        }
    }
    
    private async Task DownloadFileAsync(UpdateAction action, Func<float, Task> progressCallback)
    {
        var path = action.LocalFilePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var temp = path + ".tmp";

        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        using var resp = await _httpClient.GetAsync(DownloadUrl + action.DownloadFilePath, HttpCompletionOption.ResponseHeadersRead);
        resp.EnsureSuccessStatusCode();

        var total = resp.Content.Headers.ContentLength ?? -1L;
        var downloaded = 0L;

        using var stream = await resp.Content.ReadAsStreamAsync();
        var file = File.Create(temp);
        var buffer = new byte[81920];
        int read;
        var emitTimer = Stopwatch.StartNew();

        while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            file.Write(buffer, 0, read);
            downloaded += read;

            if (emitTimer.ElapsedMilliseconds >= 10 || downloaded == total)
            {
                var prog = total > 0 ? (downloaded / (float)total) * 100f : 99f;
                await progressCallback(prog);
                emitTimer.Restart();
            }
        }

        file.Dispose();
        emitTimer.Stop();

        if (total > 0)
            await progressCallback(100f);

        if (!string.IsNullOrEmpty(action.Hash))
        {
            var actual = await ComputeHashAsync(temp);
            if (actual != action.Hash)
                throw new Exception($"Hash mismatch for {path}");
        }

        try
        {
            if (File.Exists(path))
                File.Delete(path);
            
            File.Move(temp, path);
        }
        catch (IOException ex)
        {
            throw new Exception(ex.ToString());
        }
    }
    
    private void DeleteFile(UpdateAction action)
    {
        var path = action.LocalFilePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (File.Exists(path))
            File.Delete(path);
    }
    
    private async Task<string> ComputeHashAsync(string path)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(path);
        var buffer = new byte[81920];
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            sha.TransformBlock(buffer, 0, bytesRead, null, 0);
        
        sha.TransformFinalBlock([], 0, 0);
        return BitConverter.ToString(sha.Hash).Replace("-", "").ToLowerInvariant();
    }
    
    protected virtual void OnUpdateProgress(ProgressEventArgs e) => UpdateProgress?.Invoke(this, e);
}