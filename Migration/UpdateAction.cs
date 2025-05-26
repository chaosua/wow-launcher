namespace wow_launcher_cs.Migration;

public class UpdateAction
{
    public uint ActionType { get; set; }
    public string LocalFilePath { get; set; }
    public string DownloadFilePath { get; set; }
    public string Hash { get; set; }
}