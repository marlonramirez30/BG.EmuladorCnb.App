// Decompiled with JetBrains decompiler
// Type: HeraclesNbcDevice.VenusBuffer
// Assembly: HeraclesNbcDevice, Version=1.0.7601.33003, Culture=neutral, PublicKeyToken=null
// MVID: 53ABA5F3-3BBD-434A-BA9F-8DF92FE3B1E6
// Assembly location: C:\Users\marlo\Desktop\BGuayaquil\cnb_emulador 7\cnb_emulador\HeraclesNbcDevice.dll

using System.Text;

#nullable disable
namespace HeraclesNbcDevice;

internal class VenusBuffer
{
  private StringBuilder sb;

  public VenusBuffer(byte command)
  {
    this.sb = new StringBuilder(1);
    this.addBinary(command);
  }

  public void addBinary(params byte[] element)
  {
    char[] chArray = new char[element.Length];
    for (int index = 0; index < chArray.Length; ++index)
      chArray[index] = (char) element[index];
    this.sb.Append(new string(chArray));
  }

  public void addString(params string[] element)
  {
    for (int index = 0; index < element.Length; ++index)
      this.sb.Append(element[index]);
  }

  public byte[] getBuffer()
  {
    byte[] buffer = new byte[this.sb.Length];
    for (int index = 0; index < buffer.Length; ++index)
      buffer[index] = (byte) this.sb[index];
    return buffer;
  }

  public void clear() => this.sb = new StringBuilder(1);
}
