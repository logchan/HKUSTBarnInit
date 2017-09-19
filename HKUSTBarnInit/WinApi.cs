using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HKUSTBarnInit
{
    public static class WinApi
    {
        #region WinApi Definitions from pinvoke.net
        // ReSharper disable InconsistentNaming

        [Flags]
        enum SPIF
        {
            None = 0x00,
            /// <summary>Writes the new system-wide parameter setting to the user profile.</summary>
            SPIF_UPDATEINIFILE = 0x01,
            /// <summary>Broadcasts the WM_SETTINGCHANGE message after updating the user profile.</summary>
            SPIF_SENDCHANGE = 0x02,
            /// <summary>Same as SPIF_SENDCHANGE.</summary>
            SPIF_SENDWININICHANGE = 0x02
        }

        enum SPI : uint
        {
            SPI_GETMOUSE = 0x03,
            SPI_SETMOUSE = 0x04,
            SPI_GETMOUSESPEED = 0x70,
            SPI_SETMOUSESPEED = 0x71
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, int[] pvParam, SPIF fWinIni);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, int pvParam, SPIF fWinIni);

        // ReSharper restore InconsistentNaming
        #endregion

        public static void SetMousePrecision(bool enabled)
        {
            var param = new int[3];
            if (!SystemParametersInfo(SPI.SPI_GETMOUSE, 0, param, 0))
            {
                throw new Exception("SPI_GETMOUSE");
            }

            param[2] = enabled ? 1 : 0;
            if (!SystemParametersInfo(SPI.SPI_SETMOUSE, 0, param, SPIF.SPIF_SENDCHANGE))
            {
                throw new Exception("SPI_SETMOUSE");
            }
        }

        public static int GetMouseSpeed()
        {
            var param = new int[1];
            if (!SystemParametersInfo(SPI.SPI_GETMOUSESPEED, 0, param, 0))
            {
                throw new Exception("SPI_GETMOUSESPEED");
            }

            return param[0];
        }

        public static void SetMouseSpeed(int value)
        {
            if (!SystemParametersInfo(SPI.SPI_SETMOUSESPEED, 0, value, SPIF.SPIF_SENDCHANGE))
            {
                throw new Exception("SPI_SETMOUSESPEED");
            }
        }

        public static void RunCmd(string command)
        {
            var cmd = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };
            cmd.ErrorDataReceived += (sendingProcess, data) => Console.WriteLine(data.Data);
            cmd.OutputDataReceived += (sendingProcess, data) => Console.WriteLine(data.Data);
            cmd.Start();

            cmd.BeginOutputReadLine();
            cmd.BeginErrorReadLine();
            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
        }

        public static void RestartExplorer()
        {
            RunCmd("taskkill /f /im explorer.exe");
            Thread.Sleep(1000);
            Process.Start(@"C:\Windows\explorer.exe");
        }
    }
}
