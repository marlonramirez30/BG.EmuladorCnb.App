// Decompiled with JetBrains decompiler
// Type: HeraclesNbcDevice.ZlibConstants
// Assembly: HeraclesNbcDevice, Version=1.0.7601.33003, Culture=neutral, PublicKeyToken=null
// MVID: 53ABA5F3-3BBD-434A-BA9F-8DF92FE3B1E6
// Assembly location: C:\Users\marlo\Desktop\BGuayaquil\cnb_emulador 7\cnb_emulador\HeraclesNbcDevice.dll

#nullable disable
namespace HeraclesNbcDevice;

public static class ZlibConstants
{
  public const int WindowBitsMax = 15;
  public const int WindowBitsDefault = 15;
  public const int Z_OK = 0;
  public const int Z_STREAM_END = 1;
  public const int Z_NEED_DICT = 2;
  public const int Z_STREAM_ERROR = -2;
  public const int Z_DATA_ERROR = -3;
  public const int Z_BUF_ERROR = -5;
  public const int WorkingBufferSizeDefault = 16384 /*0x4000*/;
  public const int WorkingBufferSizeMin = 1024 /*0x0400*/;
}
