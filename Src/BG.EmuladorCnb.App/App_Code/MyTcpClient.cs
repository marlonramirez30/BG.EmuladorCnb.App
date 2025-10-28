using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BG.EmuladorCnb.App.App_Code
{
    public class MyTcpClient
    {
        private IPEndPoint remoteEndPoint;
        private TcpClient tcpClient;
        private NetworkStream stream;
        private int receiveTimeout;
        private int writeTimeout;
        private ManualResetEvent mre = new ManualResetEvent(false);
        private bool endWrite;
        private bool endRead;
        private Socket socket;

        public MyTcpClient(string ip, int port)
        {
            this.remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            this.writeTimeout = 10000;
            this.receiveTimeout = 10000;
        }

        public short connect()
        {
            short num;
            try
            {
                this.tcpClient = new TcpClient();
                this.tcpClient.Connect(this.remoteEndPoint);
                this.socket = this.tcpClient.Client;
                this.stream = this.tcpClient.GetStream();
                num = (short)0;
            }
            catch (IOException ex)
            {
                num = (short)1;
            }
            catch (Exception ex)
            {
                num = (short)1;
            }
            return num;
        }

        public short send(string data, bool withRetry)
        {
            short num1;
            try
            {
                this.endWrite = false;
                byte[] bytes = MyTcpClient.getBytes(this.getBinaryLength(data.Length + 2) + data);
                this.stream.Write(bytes, 0, bytes.Length);
                num1 = (short)0;
            }
            catch (IOException ex)
            {
                num1 = (short)2;
            }
            catch (Exception ex)
            {
                num1 = (short)2;
            }
            finally
            {
                this.endWrite = true;
            }
            if (num1 != (short)0 & withRetry)
            {
                this.close();
                int num2 = (int)this.connect();
                num1 = this.send(data, false);
            }
            return num1;
        }

        public string read()
        {
            StateObject state = new StateObject();
            state.stream = this.stream;
            state.buffer = new byte[2];
            string str;
            try
            {
                this.mre.Reset();
                this.endRead = false;
                this.stream.BeginRead(state.buffer, 0, state.buffer.Length, new AsyncCallback(this.readCallBack), (object)state);
                this.mre.WaitOne(this.receiveTimeout, false);
                if (this.endRead)
                {
                    str = state.data.ToString().Substring(2);
                }
                else
                {
                    this.endRead = true;
                    str = (string)null;
                }
            }
            catch (Exception ex)
            {
                str = (string)null;
            }
            return str;
        }

        private void readCallBack(IAsyncResult ar)
        {
            try
            {
                StateObject asyncState = (StateObject)ar.AsyncState;
                NetworkStream stream = asyncState.stream;
                int length = stream.EndRead(ar);
                if (length > 0)
                {
                    asyncState.data.Append(MyTcpClient.getString(asyncState.buffer, 0, length));
                    if (asyncState.data.Length < this.getLength((byte)asyncState.data[0], (byte)asyncState.data[1]))
                    {
                        asyncState.buffer = new byte[this.getLength((byte)asyncState.data[0], (byte)asyncState.data[1]) - asyncState.data.Length];
                        stream.BeginRead(asyncState.buffer, 0, asyncState.buffer.Length, new AsyncCallback(this.readCallBack), (object)asyncState);
                    }
                    else
                    {
                        this.endRead = true;
                        this.mre.Set();
                    }
                }
                else
                {
                    this.endRead = false;
                    this.mre.Set();
                }
            }
            catch (Exception ex)
            {
                this.endRead = false;
                this.mre.Set();
            }
        }

        public int ReceiveTimeout
        {
            get => this.receiveTimeout;
            set => this.receiveTimeout = value;
        }

        public int WriteTimeout
        {
            get => this.writeTimeout;
            set => this.writeTimeout = value;
        }

        public void close()
        {
            try
            {
                if (this.stream != null)
                {
                    this.stream.Close();
                }
                if (this.tcpClient != null)
                {
                    this.tcpClient.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static byte[] getBytes(string data)
        {
            byte[] bytes = new byte[data.Length];
            for (int index = 0; index < data.Length; ++index)
                bytes[index] = (byte)data[index];
            return bytes;
        }

        public static string getString(byte[] bytes)
        {
            char[] chArray = new char[bytes.Length];
            for (int index = 0; index < bytes.Length; ++index)
                chArray[index] = (char)bytes[index];
            return new string(chArray);
        }

        public static string getString(byte[] bytes, int idx, int length)
        {
            char[] chArray = new char[length];
            for (int index = idx; index < Math.Min(bytes.Length - idx, idx + length); ++index)
                chArray[index] = (char)bytes[index];
            return new string(chArray);
        }

        public string getBinaryLength(int length)
        {
            byte[] numArray1 = new byte[2];
            char[] chArray = new char[2];
            byte[] numArray2 = numArray1;
            int num = length <= 65536 /*0x010000*/ ? (int)(byte)length : throw new Exception();
            numArray2[1] = (byte)num;
            numArray1[0] = (byte)(length >> 8);
            chArray[1] = (char)numArray1[1];
            chArray[0] = (char)numArray1[0];
            return new string(chArray);
        }

        public int getLength(byte h1, byte h2) => (int)h1 << 8 | (int)h2;
    }
}
