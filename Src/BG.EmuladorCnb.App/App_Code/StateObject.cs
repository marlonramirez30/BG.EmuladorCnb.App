using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BG.EmuladorCnb.App.App_Code
{
    public class StateObject
    {
        public StringBuilder data = new StringBuilder(2);
        public NetworkStream stream = (NetworkStream)null;
        public byte[] buffer;
    }
}
