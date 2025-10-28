using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG.EmuladorCnb.App.App_Code
{
    public class NbcBufferList
    {
        private Queue bufferQueue;
        private string ss = "<<";
        private string es = ">>";

        public NbcBufferList() => this.bufferQueue = new Queue(2);

        public void add(string bufferName, string bufferValue)
        {
            this.add(new NbcBuffer(bufferName, bufferValue));
        }

        public void add(NbcBuffer nbcBuffer)
        {
            NbcBuffer nbcBuffer1;
            if ((nbcBuffer1 = this.findNbcBuffer(nbcBuffer.BufferName)) == null)
                this.bufferQueue.Enqueue((object)nbcBuffer);
            else
                nbcBuffer1.BufferValue = nbcBuffer.BufferValue;
        }

        public NbcBuffer getBufferAt(int idx)
        {
            NbcBuffer bufferAt = (NbcBuffer)null;
            int num = 0;
            foreach (NbcBuffer buffer in this.bufferQueue)
            {
                bufferAt = buffer;
                if (num != idx)
                    ++num;
                else
                    break;
            }
            return bufferAt;
        }

        public string getBufferValue(string bufferName)
        {
            NbcBuffer nbcBuffer = this.findNbcBuffer(bufferName);
            return nbcBuffer == null ? string.Empty : nbcBuffer.BufferValue;
        }

        public void clear() => this.bufferQueue.Clear();

        private NbcBuffer findNbcBuffer(string bufferName)
        {
            NbcBuffer nbcBuffer = (NbcBuffer)null;
            foreach (object buffer in this.bufferQueue)
            {
                if (((NbcBuffer)buffer).BufferName.Equals(bufferName))
                {
                    nbcBuffer = (NbcBuffer)buffer;
                    break;
                }
            }
            return nbcBuffer;
        }

        public string getBufferString()
        {
            string str = "";
            foreach (NbcBuffer buffer in this.bufferQueue)
                str = str + this.ss + buffer.BufferName + this.es + buffer.BufferValue;
            return str + this.ss;
        }

        public int Count => this.bufferQueue.Count;
    }
}
