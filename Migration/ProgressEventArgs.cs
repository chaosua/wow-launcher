using System;

namespace wow_launcher_cs.Migration;

public class ProgressEventArgs : EventArgs
{
    public float Progress { get; set; }
    public int Current { get; set; }
    public int Total { get; set; }
}