// Decompiled with JetBrains decompiler
// Type: HeraclesNbcDevice.InternalConstants
// Assembly: HeraclesNbcDevice, Version=1.0.7601.33003, Culture=neutral, PublicKeyToken=null
// MVID: 53ABA5F3-3BBD-434A-BA9F-8DF92FE3B1E6
// Assembly location: C:\Users\marlo\Desktop\BGuayaquil\cnb_emulador 7\cnb_emulador\HeraclesNbcDevice.dll

#nullable disable
namespace HeraclesNbcDevice;

internal static class InternalConstants
{
  internal static readonly int MAX_BITS = 15;
  internal static readonly int BL_CODES = 19;
  internal static readonly int D_CODES = 30;
  internal static readonly int LITERALS = 256 /*0x0100*/;
  internal static readonly int LENGTH_CODES = 29;
  internal static readonly int L_CODES = InternalConstants.LITERALS + 1 + InternalConstants.LENGTH_CODES;
  internal static readonly int MAX_BL_BITS = 7;
  internal static readonly int REP_3_6 = 16 /*0x10*/;
  internal static readonly int REPZ_3_10 = 17;
  internal static readonly int REPZ_11_138 = 18;
}
