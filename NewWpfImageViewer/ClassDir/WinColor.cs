using Microsoft.Win32;

namespace NewWpfImageViewer.ClassDir
{
    public static class WinColor
    {
        public static string GetColor()
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM\"))
            {
                int a = (int)registryKey.GetValue("ColorizationColor");
                return a.ToString("X");
            }
        }
    }
}
