// Decompiled with JetBrains decompiler
// Type: HeraclesNbcDevice.SharedUtils
// Assembly: HeraclesNbcDevice, Version=1.0.7601.33003, Culture=neutral, PublicKeyToken=null
// MVID: 53ABA5F3-3BBD-434A-BA9F-8DF92FE3B1E6
// Assembly location: C:\Users\marlo\Desktop\BGuayaquil\cnb_emulador 7\cnb_emulador\HeraclesNbcDevice.dll

using System.IO;
using System.Text;

#nullable disable
namespace HeraclesNbcDevice;

internal class SharedUtils
{
  public static int URShift(int number, int bits) => number >>> bits;

  public static int ReadInput(TextReader sourceTextReader, byte[] target, int start, int count)
  {
    if (target.Length == 0)
      return 0;
    char[] buffer = new char[target.Length];
    int num = sourceTextReader.Read(buffer, start, count);
    if (num == 0)
      return -1;
    for (int index = start; index < start + num; ++index)
      target[index] = (byte) buffer[index];
    return num;
  }

  internal static byte[] ToByteArray(string sourceString) => Encoding.UTF8.GetBytes(sourceString);

  internal static char[] ToCharArray(byte[] byteArray) => Encoding.UTF8.GetChars(byteArray);
}
