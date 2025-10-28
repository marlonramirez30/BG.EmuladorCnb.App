using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BG.EmuladorCnb.App.App_Code
{
    public class MyLog
    {
        private string fullFileName;
        private string logName;

        public MyLog(string logFile)
        {
            string str = Path.Combine(MyLog.getCurrentDirectory(), "Logs");
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);
            this.fullFileName = Path.Combine(str, logFile);
        }

        public string FullFileName => this.fullFileName;

        public void writeLine(params string[] data)
        {
            StringBuilder stringBuilder = new StringBuilder($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")}]-");
            for (int index = 0; index < data.Length; ++index)
                stringBuilder.Append(data[index]);
            StreamWriter streamWriter = (StreamWriter)null;
            try
            {
                streamWriter = new StreamWriter(this.fullFileName, true);
                streamWriter.WriteLine(stringBuilder.ToString());
            }
            catch
            {
            }
            finally
            {
                streamWriter?.Flush();
                streamWriter.Close();
            }
        }

        private static string getCurrentDirectory()
        {
            try
            {
                StringBuilder lpFilename = new StringBuilder((int)byte.MaxValue);
                int moduleFileName = (int)MyLog.GetModuleFileName(IntPtr.Zero, lpFilename, lpFilename.Capacity);
                return lpFilename.ToString().Substring(0, lpFilename.ToString().LastIndexOf('\\'));
            }
            catch (Exception ex)
            {
                try
                {
                    return Directory.GetCurrentDirectory();
                }
                catch
                {
                    return "\\";
                }
            }
        }

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern uint GetModuleFileName([In] IntPtr hModule, [Out] StringBuilder lpFilename, [In] int nSize);
    }
}
