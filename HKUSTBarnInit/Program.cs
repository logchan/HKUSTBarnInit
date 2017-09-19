using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace HKUSTBarnInit
{
    class Program
    {
        static void Log(string message)
        {
            message = $"{DateTime.Now:HH:mm:ss.fff} {message}";
            Console.WriteLine(message);
        }

        static void Log(Exception ex, string source)
        {
            Log($"Exception from {source}: {ex.Message}");
        }

        static void Do(string source, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Log(ex, source);
            }
        }

        static void Greetings()
        {
            var ver = Assembly.GetExecutingAssembly().GetName().Version;

            Console.WriteLine($"HKUST Barn Init {ver.Major}.{ver.Minor}");
            Console.WriteLine("Author: logchan (https://github.com/logchan)");
            Console.WriteLine("Icons made by Freepik from www.flaticon.com is licensed by CC 3.0 BY");
            Console.WriteLine("----------------------------");
        }

        static void Main(string[] args)
        {
            Greetings();

            var settings = ConfigurationManager.AppSettings;
            
            Do("SetMousePrecision", () =>
            {
                var mousePrecision = settings.IsTrue("MousePrecision");
                Log($"Set mouse precision to {mousePrecision}");
                WinApi.SetMousePrecision(mousePrecision);
            });
            
            Do("SetMouseSpeed", () =>
            {
                var speed = WinApi.GetMouseSpeed();
                Log($"Current mouse speed is {speed}");

                if (!Int32.TryParse(settings["MouseSpeed"], out speed) || speed < 1 || speed > 20)
                {
                    Log("Skip set mouse speed");
                    return;
                }

                Log($"Set mouse speed to {speed}");
                WinApi.SetMouseSpeed(speed);
            });
            
            Do("SetTaskbarCombine", () =>
            {
                var combine = settings.IsTrue("TaskbarCombine");
                Log($"Set taskbar combine to {combine}");

                var regKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                Registry.SetValue(regKey, "TaskbarGlomLevel", combine ? 0x1 : 0x2, RegistryValueKind.DWord);
            });

            Do("ClearTaskbarPin", () =>
            {
                var clearPin = settings.IsTrue("ClearTaskbarPin");
                if (!clearPin)
                {
                    Log("Skip clear taskbar pin");
                    return;
                }

                Log("Clear taskbar pin");
                WinApi.RunCmd(
                    @"DEL /F /S /Q /A ""%AppData%\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar\*"""
                );
                WinApi.RunCmd(
                    @"REG DELETE HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband /F"
                );
            });

            Do("ExplorerSettings", () =>
            {
                Log("Update explorer settings");
                var regKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";

                if (settings.IsTrue("ShowFilenameExtension"))
                {
                    Log("Show filename extension");
                    Registry.SetValue(regKey, "HideFileExt", 0x0, RegistryValueKind.DWord);
                }

                if (settings.IsTrue("StartExplorerToThisPC"))
                {
                    Log("Start explorer to This PC");
                    Registry.SetValue(regKey, "LaunchTo", 0x1, RegistryValueKind.DWord);
                }
            });

            Do("RestartExplorer", () =>
            {
                Log("Restart explorer");
                WinApi.RestartExplorer();
            });
            
            Do("SetRdp", () =>
            {
                var rdpServer = settings["RdpServer"];
                var rdpUser = settings["RdpUser"];

                if (String.IsNullOrWhiteSpace(rdpServer) || String.IsNullOrWhiteSpace(rdpUser))
                {
                    Log("Skip RDP settings");
                    return;
                }

                Log("Set RDP settings");
                var regKey = @"HKEY_CURRENT_USER\Software\Microsoft\Terminal Server Client\Servers\" + rdpServer;
                Registry.SetValue(regKey, "UsernameHint", rdpUser, RegistryValueKind.String);
            });
            
            Console.WriteLine("Done. Press Enter to exit.");
            Console.ReadLine();
        }
    }
}
