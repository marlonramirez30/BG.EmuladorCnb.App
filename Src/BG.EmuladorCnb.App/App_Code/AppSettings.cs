using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG.EmuladorCnb.App.App_Code
{
    public class AppSettings
    {
        public string WindowState { get; set; }
        public WindowSize WindowSize { get; set; }
    }

    public class WindowSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
