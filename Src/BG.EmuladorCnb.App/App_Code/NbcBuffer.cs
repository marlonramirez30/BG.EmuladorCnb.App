using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG.EmuladorCnb.App.App_Code
{
    public class NbcBuffer
    {
        private string bufferName;
        private string bufferValue;

        public NbcBuffer(string bufferName) => this.bufferName = bufferName;

        public NbcBuffer(string bufferName, string bufferValue)
        {
            this.bufferName = bufferName;
            this.bufferValue = bufferValue;
        }

        public string BufferName => this.bufferName;

        public string BufferValue
        {
            get => this.bufferValue;
            set => this.bufferValue = value;
        }
    }
}
