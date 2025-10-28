// Decompiled with JetBrains decompiler
// Type: HeraclesNbcDevice.PinPad
// Assembly: HeraclesNbcDevice, Version=1.0.7601.33003, Culture=neutral, PublicKeyToken=null
// MVID: 53ABA5F3-3BBD-434A-BA9F-8DF92FE3B1E6
// Assembly location: C:\Users\marlo\Desktop\BGuayaquil\cnb_emulador 7\cnb_emulador\HeraclesNbcDevice.dll

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

#nullable disable
namespace HeraclesNbcDevice;

public class PinPad
{
  private const byte devID = 1;
  private const byte protocol = 0;
  private const int XPD_SYNC_ERROR = 57350;
  private int timeout;
  private int handle;
  private int lastError;
  private static bool firstTime = true;
  private static string pinpadMessage1;
  private static string pinpadMessage2;
  private static string setGMAC;

  public PinPad(int timeout, string pinpadMsg1, string pinpadMsg2, string stGMAC)
  {
    this.timeout = timeout;
    PinPad.pinpadMessage1 = pinpadMsg1;
    PinPad.pinpadMessage2 = pinpadMsg2;
    PinPad.setGMAC = stGMAC;
  }

  private void connect()
  {
    this.lastError = PinPad.XpdConnect(ref this.handle, (byte) 1, IntPtr.Zero, 0, (byte) 0);
  }

  public void close()
  {
  }

  public int LastError => this.lastError;

  private void send(byte[] command)
  {
    this.lastError = PinPad.XpdSend(this.handle, command, command.Length, this.timeout);
  }

  private bool receive(out byte[] buff, int maxLen, bool asyncr)
  {
    bool flag = false;
    buff = (byte[]) null;
    byte[] data = new byte[maxLen];
    int length;
    if ((length = PinPad.XpdRecv(this.handle, data, data.Length, this.timeout)) > 0)
    {
      buff = new byte[length];
      for (int index = 0; index < length; ++index)
        buff[index] = data[index];
      flag = true;
    }
    else if (asyncr)
    {
      this.send(new byte[1]{ (byte) 130 });
      Thread.Sleep(1000);
    }
    this.lastError = length;
    return flag;
  }

  public bool readCardData(string displayMessage, out string track1, out string track2)
  {
    track1 = (string) null;
    track2 = (string) null;
    bool flag = false;
    this.sendMessage(displayMessage);
    byte[] buff;
    if (this.receive(out buff, 456, false) && buff[0] == (byte) 38)
    {
      int index = 3;
      if (((int) buff[2] & 1) == 1)
      {
        track1 = Encoding.ASCII.GetString(buff, index + 1, (int) buff[index]);
        index += (int) buff[index] + 1;
      }
      if (((int) buff[2] & 2) == 2)
      {
        track2 = Encoding.ASCII.GetString(buff, index + 1, (int) buff[index]);
        int num = index + ((int) buff[index] + 1);
        if (track2.IndexOf('=') > 0)
          flag = true;
      }
      if (!flag)
      {
        this.close();
        this.connect();
      }
    }
    return flag;
  }

  public bool readPinBuffer(
    string displayMessage,
    string cardNumber,
    string workingKey,
    out string pinBuffer)
  {
    pinBuffer = "6CE368C45DADD347";
    return true;
  }

  public void clearScreen()
  {
  }

  private bool SetGMAC()
  {
    bool flag = false;
    VenusBuffer venusBuffer = new VenusBuffer((byte) 85);
    for (int startIndex = 2; startIndex < PinPad.setGMAC.Length; startIndex += 2)
      venusBuffer.addBinary(Convert.ToByte(PinPad.setGMAC.Substring(startIndex, 2), 16 /*0x10*/));
    this.send(venusBuffer.getBuffer());
    byte[] buff;
    if (this.receive(out buff, 2, false) && buff[0] == (byte) 85 && buff[1] == (byte) 0)
      flag = true;
    Thread.Sleep(1000);
    return flag;
  }

  private void storeMessages()
  {
    VenusBuffer venusBuffer = new VenusBuffer((byte) 29);
    for (int startIndex = 2; startIndex < PinPad.pinpadMessage1.Length; startIndex += 2)
      venusBuffer.addBinary(Convert.ToByte(PinPad.pinpadMessage1.Substring(startIndex, 2), 16 /*0x10*/));
    this.send(venusBuffer.getBuffer());
    Thread.Sleep(1000);
    venusBuffer.clear();
    for (int startIndex = 0; startIndex < PinPad.pinpadMessage2.Length; startIndex += 2)
      venusBuffer.addBinary(Convert.ToByte(PinPad.pinpadMessage2.Substring(startIndex, 2), 16 /*0x10*/));
    this.send(venusBuffer.getBuffer());
    Thread.Sleep(1000);
  }

  private void sendMessage(string message)
  {
    message = message.Trim('[', ']');
    VenusBuffer venusBuffer = new VenusBuffer((byte) 100);
    for (int startIndex = 2; startIndex < message.Length; startIndex += 2)
      venusBuffer.addBinary(Convert.ToByte(message.Substring(startIndex, 2), 16 /*0x10*/));
    this.send(venusBuffer.getBuffer());
  }

  [DllImport("SaioXpd.dll")]
  private static extern int XpdConnect(
    ref int handle,
    byte devID,
    IntPtr address,
    int baudrate,
    byte protocol);

  [DllImport("SaioXpd.dll")]
  private static extern int XpdSend(int handle, byte[] data, int len, int timeout);

  [DllImport("SaioXpd.dll")]
  private static extern int XpdRecv(int handle, byte[] data, int len, int timeout);

  [DllImport("SaioXpd.dll")]
  private static extern int XpdClose(int handle);
}
