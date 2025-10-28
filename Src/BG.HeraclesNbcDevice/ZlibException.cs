// Decompiled with JetBrains decompiler
// Type: HeraclesNbcDevice.ZlibException
// Assembly: HeraclesNbcDevice, Version=1.0.7601.33003, Culture=neutral, PublicKeyToken=null
// MVID: 53ABA5F3-3BBD-434A-BA9F-8DF92FE3B1E6
// Assembly location: C:\Users\marlo\Desktop\BGuayaquil\cnb_emulador 7\cnb_emulador\HeraclesNbcDevice.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace HeraclesNbcDevice;

[Guid("ebc25cf6-9120-4283-b972-0e5520d0000E")]
public class ZlibException : Exception
{
  public ZlibException()
  {
  }

  public ZlibException(string s)
    : base(s)
  {
  }
}
