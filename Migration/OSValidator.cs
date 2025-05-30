using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace wow_launcher_cs.Migration;

public static class OSValidator
{
    [DllImport("ntdll.dll", CharSet = CharSet.Unicode)]
    private static extern int RtlGetVersion(ref OSVERSIONINFOW versionInfo);
    
    private const string WebView2ClientId = "{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}";
    
    private static readonly string[] Paths =
    [
        $@"SOFTWARE\Microsoft\EdgeUpdate\Clients\{WebView2ClientId}",
        $@"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{WebView2ClientId}",
        $@"Software\Microsoft\EdgeUpdate\Clients\{WebView2ClientId}"
    ];
    
    public static bool SystemIsValidForUpdate()
    {
        if (!IsWindows10OrNewer())
            return false;

        // maybe check version
        return GetInstalledVersion() != null;
    }

    private static bool IsWindows10OrNewer()
    {
        var osvi = new OSVERSIONINFOW { dwOSVersionInfoSize = (uint)Marshal.SizeOf<OSVERSIONINFOW>() };
        if (RtlGetVersion(ref osvi) != 0)
            return false;

        return !(osvi.dwMajorVersion <= 6);
    }
    
    private static string GetInstalledVersion()
    {
        foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
        {
            foreach (var path in Paths)
            {
                using var key = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenSubKey(path);
                if (key != null)
                {
                    var pv = key.GetValue("pv") as string;
                    if (!string.IsNullOrEmpty(pv) && pv != "0.0.0.0")
                        return pv;
                }

                if (hive == RegistryHive.LocalMachine)
                {
                    using var key32 = RegistryKey.OpenBaseKey(hive, RegistryView.Registry32).OpenSubKey(path);
                    
                    if (key32 != null)
                    {
                        var pv32 = key32.GetValue("pv") as string;
                        if (!string.IsNullOrEmpty(pv32) && pv32 != "0.0.0.0")
                            return pv32;
                    }
                }
            }
        }
        return null;
    }
}