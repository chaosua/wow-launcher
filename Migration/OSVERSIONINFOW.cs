using System.Runtime.InteropServices;

namespace wow_launcher_cs.Migration;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
struct OSVERSIONINFOW
{
    public uint dwOSVersionInfoSize;
    public uint dwMajorVersion;
    public uint dwMinorVersion;
    public uint dwBuildNumber;
    public uint dwPlatformId;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string szCSDVersion;
}