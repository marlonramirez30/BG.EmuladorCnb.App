// Decompiled with JetBrains decompiler
// Type: HeraclesNbcDevice.Printer
// Assembly: HeraclesNbcDevice, Version=1.0.7601.33003, Culture=neutral, PublicKeyToken=null
// MVID: 53ABA5F3-3BBD-434A-BA9F-8DF92FE3B1E6
// Assembly location: C:\Users\marlo\Desktop\BGuayaquil\cnb_emulador 7\cnb_emulador\HeraclesNbcDevice.dll

using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace HeraclesNbcDevice;

public static class Printer
{
  private const string KernelDll = "Coredll.dll";
  private const int ETO_OPAQUE = 2;
  private const int ETO_CLIPPED = 4;
  private const int DMRES_DRAFT = -1;
  private const int DMRES_HIGH = -4;
  private const int SRCCOPY = 13369376;
  public static uint PTR_OK = 0;
  public static uint PTR_HEAD_TEMPERATURE = 1;
  public static uint PTR_PAPER_OUT = 4;
  public static uint PTR_POWER_SUPPLY = 8;
  public static uint PTR_PRINTED_USED = 16 /*0x10*/;
  public static uint PTR_NO_STATUS = (uint) byte.MaxValue;

  public static bool print(ArrayList data) => true;

  private static string getCurrentDirectory()
  {
    try
    {
      StringBuilder lpFilename = new StringBuilder((int) byte.MaxValue);
      int moduleFileName = (int) Printer.GetModuleFileName(IntPtr.Zero, lpFilename, lpFilename.Capacity);
      return lpFilename.ToString().Substring(0, lpFilename.ToString().LastIndexOf('\\'));
    }
    catch (Exception ex)
    {
      try
      {
        return Directory.GetCurrentDirectory();
      }
      catch
      {
        return "\\";
      }
    }
  }

  [DllImport("coredll.dll", SetLastError = true)]
  private static extern uint GetModuleFileName([In] IntPtr hModule, [Out] StringBuilder lpFilename, [In] int nSize);

  [DllImport("Coredll.dll")]
  private static extern IntPtr CreateDC(
    [In] string lpszDriver,
    [In] string lpszDevice,
    [In] string lpszOutput,
    [In] ref Printer.DEVMODE devMode);

  [DllImport("Coredll.dll")]
  private static extern IntPtr CreateCompatibleDC([In] IntPtr HDC);

  [DllImport("Coredll.dll")]
  private static extern bool DeleteDC([In] IntPtr HDC);

  [DllImport("Coredll.dll", EntryPoint = "StartDocW")]
  private static extern int StartDoc([In] IntPtr HDC, Printer.DOCINFO docInfo);

  [DllImport("Coredll.dll")]
  private static extern int StartPage([In] IntPtr HDC);

  [DllImport("Coredll.dll")]
  private static extern int EndPage([In] IntPtr HDC);

  [DllImport("Coredll.dll")]
  private static extern int EndDoc([In] IntPtr HDC);

  [DllImport("Coredll.dll", EntryPoint = "ExtTextOutW")]
  private static extern int ExtTextOut(
    [In] IntPtr HDC,
    [In] int X,
    [In] int Y,
    [In] int fuOptions,
    [In] ref Printer.RECT lprc,
    [In] string lpString,
    [In] int cbCount,
    [In] int[] lpDx);

  [DllImport("Coredll.dll")]
  private static extern IntPtr SelectObject([In] IntPtr HDC, [In] IntPtr hgdiobj);

  [DllImport("Coredll.dll")]
  private static extern IntPtr DeleteObject([In] IntPtr HDC);

  [DllImport("Coredll.dll", EntryPoint = "CreateFontIndirectW")]
  private static extern IntPtr CreateFontIndirect([In] ref Printer.LOGFONT lplf);

  [DllImport("Coredll.dll")]
  private static extern bool BitBlt(
    [In] IntPtr hdcDest,
    [In] int nXDest,
    [In] int nYDest,
    [In] int nWidth,
    [In] int nHeight,
    [In] IntPtr hdcSrc,
    [In] int nXSrc,
    [In] int nYSrc,
    [In] int dwRop);

  [DllImport("SaioBase.dll")]
  private static extern bool PrinterStatus(out uint status);

  public static uint getPrinterStatus() => 0;

  private struct LOGFONT
  {
    private const short LF_FACESIZE = 32 /*0x20*/;
    public int lfHeight;
    public int lfWidth;
    public int lfEscapement;
    public int lfOrientation;
    public int lfWeight;
    public byte lfItalic;
    public byte lfUnderline;
    public byte lfStrikeOut;
    public byte lfCharSet;
    public byte lfOutPrecision;
    public byte lfClipPrecision;
    public byte lfQuality;
    public byte lfPitchAndFamily;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32 /*0x20*/)]
    public string lfFaceName;
  }

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  private struct DOCINFO
  {
    [MarshalAs(UnmanagedType.LPWStr)]
    public string pDocName;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string pOutFile;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string pDataType;
  }

  public struct RECT
  {
    public int left;
    public int top;
    public int right;
    public int bottom;
  }

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  public struct DEVMODE
  {
    private const int CCHDEVICENAME = 32 /*0x20*/;
    private const int CCHFORMNAME = 32 /*0x20*/;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32 /*0x20*/)]
    public string dmDeviceName;
    public short dmSpecVersion;
    public short dmDriverVersion;
    public short dmSize;
    public short dmDriverExtra;
    public int dmFields;
    public short dmOrientation;
    public short dmPaperSize;
    public short dmPaperLength;
    public short dmPaperWidth;
    public short dmScale;
    public short dmCopies;
    public short dmDefaultSource;
    public short dmPrintQuality;
    public short dmColor;
    public short dmDuplex;
    public short dmYResolution;
    public short dmTTOption;
    public short dmCollate;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32 /*0x20*/)]
    public string dmFormName;
    public short dmLogPixels;
    public int dmBitsPerPel;
    public int dmPelsWidth;
    public int dmPelsHeight;
    public int dmDisplayFlags;
    public int dmDisplayFrequency;
    public int dmDisplayOrientation;
  }
}
