using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing.Internal;
using System.Internal;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Drawing
{
	internal class SafeNativeMethods
	{
		[DllImport("gdi32", CharSet = CharSet.Auto, EntryPoint = "CreateCompatibleBitmap", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr IntCreateCompatibleBitmap(HandleRef hDC, int width, int height);

		public static IntPtr CreateCompatibleBitmap(HandleRef hDC, int width, int height)
		{
			return global::System.Internal.HandleCollector.Add(SafeNativeMethods.IntCreateCompatibleBitmap(hDC, width, height), SafeNativeMethods.CommonHandles.GDI);
		}

		[DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int BitBlt(HandleRef hDC, int x, int y, int nWidth, int nHeight, HandleRef hSrcDC, int xSrc, int ySrc, int dwRop);

		[DllImport("gdi32")]
		public static extern int GetDIBits(HandleRef hdc, HandleRef hbm, int arg1, int arg2, IntPtr arg3, ref NativeMethods.BITMAPINFO_FLAT bmi, int arg5);

		[DllImport("gdi32")]
		public static extern uint GetPaletteEntries(HandleRef hpal, int iStartIndex, int nEntries, byte[] lppe);

		[DllImport("gdi32", CharSet = CharSet.Auto, EntryPoint = "CreateDIBSection", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr IntCreateDIBSection(HandleRef hdc, ref NativeMethods.BITMAPINFO_FLAT bmi, int iUsage, ref IntPtr ppvBits, IntPtr hSection, int dwOffset);

		public static IntPtr CreateDIBSection(HandleRef hdc, ref NativeMethods.BITMAPINFO_FLAT bmi, int iUsage, ref IntPtr ppvBits, IntPtr hSection, int dwOffset)
		{
			return global::System.Internal.HandleCollector.Add(SafeNativeMethods.IntCreateDIBSection(hdc, ref bmi, iUsage, ref ppvBits, hSection, dwOffset), SafeNativeMethods.CommonHandles.GDI);
		}

		[DllImport("kernel32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GlobalFree(HandleRef handle);

		[DllImport("gdi32", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int StartDoc(HandleRef hDC, SafeNativeMethods.DOCINFO lpDocInfo);

		[DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int StartPage(HandleRef hDC);

		[DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int EndPage(HandleRef hDC);

		[DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int AbortDoc(HandleRef hDC);

		[DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int EndDoc(HandleRef hDC);

		[DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool PrintDlg([In] [Out] SafeNativeMethods.PRINTDLG lppd);

		[DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool PrintDlg([In] [Out] SafeNativeMethods.PRINTDLGX86 lppd);

		[DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int DeviceCapabilities(string pDevice, string pPort, short fwCapabilities, IntPtr pOutput, IntPtr pDevMode);

		[DllImport("winspool.drv", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int DocumentProperties(HandleRef hwnd, HandleRef hPrinter, string pDeviceName, IntPtr pDevModeOutput, HandleRef pDevModeInput, int fMode);

		[DllImport("winspool.drv", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int DocumentProperties(HandleRef hwnd, HandleRef hPrinter, string pDeviceName, IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode);

		[DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int EnumPrinters(int flags, string name, int level, IntPtr pPrinterEnum, int cbBuf, out int pcbNeeded, out int pcReturned);

		[DllImport("kernel32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GlobalLock(HandleRef handle);

		[DllImport("gdi32", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr ResetDC(HandleRef hDC, HandleRef lpDevMode);

		[DllImport("kernel32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern bool GlobalUnlock(HandleRef handle);

		[DllImport("gdi32", CharSet = CharSet.Auto, EntryPoint = "CreateRectRgn", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr IntCreateRectRgn(int x1, int y1, int x2, int y2);

		public static IntPtr CreateRectRgn(int x1, int y1, int x2, int y2)
		{
			return global::System.Internal.HandleCollector.Add(SafeNativeMethods.IntCreateRectRgn(x1, y1, x2, y2), SafeNativeMethods.CommonHandles.GDI);
		}

		[DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int GetClipRgn(HandleRef hDC, HandleRef hRgn);

		[DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int SelectClipRgn(HandleRef hDC, HandleRef hRgn);

		[DllImport("gdi32", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int AddFontResourceEx(string lpszFilename, int fl, IntPtr pdv);

		public static int AddFontFile(string fileName)
		{
			return SafeNativeMethods.AddFontResourceEx(fileName, 16, IntPtr.Zero);
		}

		internal static IntPtr SaveClipRgn(IntPtr hDC)
		{
			IntPtr intPtr = SafeNativeMethods.CreateRectRgn(0, 0, 0, 0);
			IntPtr intPtr2 = IntPtr.Zero;
			try
			{
				if (SafeNativeMethods.GetClipRgn(new HandleRef(null, hDC), new HandleRef(null, intPtr)) > 0)
				{
					intPtr2 = intPtr;
					intPtr = IntPtr.Zero;
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					SafeNativeMethods.DeleteObject(new HandleRef(null, intPtr));
				}
			}
			return intPtr2;
		}

		internal static void RestoreClipRgn(IntPtr hDC, IntPtr hRgn)
		{
			try
			{
				SafeNativeMethods.SelectClipRgn(new HandleRef(null, hDC), new HandleRef(null, hRgn));
			}
			finally
			{
				if (hRgn != IntPtr.Zero)
				{
					SafeNativeMethods.DeleteObject(new HandleRef(null, hRgn));
				}
			}
		}

		[DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int ExtEscape(HandleRef hDC, int nEscape, int cbInput, ref int inData, int cbOutput, out int outData);

		[DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int ExtEscape(HandleRef hDC, int nEscape, int cbInput, byte[] inData, int cbOutput, out int outData);

		[DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int IntersectClipRect(HandleRef hDC, int x1, int y1, int x2, int y2);

		[DllImport("kernel32", CharSet = CharSet.Auto, EntryPoint = "GlobalAlloc", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr IntGlobalAlloc(int uFlags, UIntPtr dwBytes);

		public static IntPtr GlobalAlloc(int uFlags, uint dwBytes)
		{
			return SafeNativeMethods.IntGlobalAlloc(uFlags, new UIntPtr(dwBytes));
		}

		internal unsafe static void ZeroMemory(byte* ptr, ulong length)
		{
			byte* ptr2 = ptr + length;
			while (ptr != ptr2)
			{
				*(ptr++) = 0;
			}
		}

		[DllImport("gdi32", CharSet = CharSet.Auto, EntryPoint = "DeleteObject", ExactSpelling = true, SetLastError = true)]
		internal static extern int IntDeleteObject(HandleRef hObject);

		public static int DeleteObject(HandleRef hObject)
		{
			global::System.Internal.HandleCollector.Remove((IntPtr)hObject, SafeNativeMethods.CommonHandles.GDI);
			return SafeNativeMethods.IntDeleteObject(hObject);
		}

		[DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr SelectObject(HandleRef hdc, HandleRef obj);

		[DllImport("user32", EntryPoint = "CreateIconFromResourceEx", SetLastError = true)]
		private unsafe static extern IntPtr IntCreateIconFromResourceEx(byte* pbIconBits, int cbIconBits, bool fIcon, int dwVersion, int csDesired, int cyDesired, int flags);

		public unsafe static IntPtr CreateIconFromResourceEx(byte* pbIconBits, int cbIconBits, bool fIcon, int dwVersion, int csDesired, int cyDesired, int flags)
		{
			return global::System.Internal.HandleCollector.Add(SafeNativeMethods.IntCreateIconFromResourceEx(pbIconBits, cbIconBits, fIcon, dwVersion, csDesired, cyDesired, flags), SafeNativeMethods.CommonHandles.Icon);
		}

		[DllImport("shell32.dll", BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = "ExtractAssociatedIcon")]
		public static extern IntPtr IntExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);

		public static IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index)
		{
			return global::System.Internal.HandleCollector.Add(SafeNativeMethods.IntExtractAssociatedIcon(hInst, iconPath, ref index), SafeNativeMethods.CommonHandles.Icon);
		}

		[DllImport("user32", CharSet = CharSet.Auto, EntryPoint = "LoadIcon", SetLastError = true)]
		private static extern IntPtr IntLoadIcon(HandleRef hInst, IntPtr iconId);

		public static IntPtr LoadIcon(HandleRef hInst, int iconId)
		{
			return SafeNativeMethods.IntLoadIcon(hInst, new IntPtr(iconId));
		}

		[DllImport("user32", CharSet = CharSet.Auto, EntryPoint = "DestroyIcon", ExactSpelling = true, SetLastError = true)]
		private static extern bool IntDestroyIcon(HandleRef hIcon);

		public static bool DestroyIcon(HandleRef hIcon)
		{
			global::System.Internal.HandleCollector.Remove((IntPtr)hIcon, SafeNativeMethods.CommonHandles.Icon);
			return SafeNativeMethods.IntDestroyIcon(hIcon);
		}

		[DllImport("user32", CharSet = CharSet.Auto, EntryPoint = "CopyImage", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr IntCopyImage(HandleRef hImage, int uType, int cxDesired, int cyDesired, int fuFlags);

		public static IntPtr CopyImage(HandleRef hImage, int uType, int cxDesired, int cyDesired, int fuFlags)
		{
			int num;
			if (uType == 1)
			{
				num = SafeNativeMethods.CommonHandles.Icon;
			}
			else
			{
				num = SafeNativeMethods.CommonHandles.GDI;
			}
			return global::System.Internal.HandleCollector.Add(SafeNativeMethods.IntCopyImage(hImage, uType, cxDesired, cyDesired, fuFlags), num);
		}

		[DllImport("gdi32", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetObject(HandleRef hObject, int nSize, [In] [Out] SafeNativeMethods.BITMAP bm);

		[DllImport("gdi32", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetObject(HandleRef hObject, int nSize, [In] [Out] SafeNativeMethods.LOGFONT lf);

		public static int GetObject(HandleRef hObject, SafeNativeMethods.LOGFONT lp)
		{
			return SafeNativeMethods.GetObject(hObject, Marshal.SizeOf(typeof(SafeNativeMethods.LOGFONT)), lp);
		}

		[DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern bool GetIconInfo(HandleRef hIcon, [In] [Out] SafeNativeMethods.ICONINFO info);

		[DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern bool DrawIconEx(HandleRef hDC, int x, int y, HandleRef hIcon, int width, int height, int iStepIfAniCursor, HandleRef hBrushFlickerFree, int diFlags);

		[DllImport("oleaut32.dll", PreserveSig = false)]
		public static extern SafeNativeMethods.IPicture OleCreatePictureIndirect(SafeNativeMethods.PICTDESC pictdesc, [In] ref Guid refiid, bool fOwn);

		public const int ERROR_CANCELLED = 1223;

		public const int E_UNEXPECTED = -2147418113;

		public const int E_NOTIMPL = -2147467263;

		public const int E_ABORT = -2147467260;

		public const int E_FAIL = -2147467259;

		public const int E_ACCESSDENIED = -2147024891;

		public const int GMEM_MOVEABLE = 2;

		public const int GMEM_ZEROINIT = 64;

		public const int DM_IN_BUFFER = 8;

		public const int DM_OUT_BUFFER = 2;

		public const int DT_PLOTTER = 0;

		public const int DT_RASPRINTER = 2;

		public const int TECHNOLOGY = 2;

		public const int DC_PAPERS = 2;

		public const int DC_PAPERSIZE = 3;

		public const int DC_BINS = 6;

		public const int DC_DUPLEX = 7;

		public const int DC_BINNAMES = 12;

		public const int DC_ENUMRESOLUTIONS = 13;

		public const int DC_PAPERNAMES = 16;

		public const int DC_ORIENTATION = 17;

		public const int DC_COPIES = 18;

		public const int PD_ALLPAGES = 0;

		public const int PD_SELECTION = 1;

		public const int PD_PAGENUMS = 2;

		public const int PD_CURRENTPAGE = 4194304;

		public const int PD_RETURNDEFAULT = 1024;

		public const int DI_NORMAL = 3;

		public const int IMAGE_ICON = 1;

		public const int IDI_APPLICATION = 32512;

		public const int IDI_HAND = 32513;

		public const int IDI_QUESTION = 32514;

		public const int IDI_EXCLAMATION = 32515;

		public const int IDI_ASTERISK = 32516;

		public const int IDI_WINLOGO = 32517;

		public const int IDI_WARNING = 32515;

		public const int IDI_ERROR = 32513;

		public const int IDI_INFORMATION = 32516;

		public const int SRCCOPY = 13369376;

		public const int PLANES = 14;

		public const int BITSPIXEL = 12;

		public const int LOGPIXELSX = 88;

		public const int LOGPIXELSY = 90;

		public const int PHYSICALWIDTH = 110;

		public const int PHYSICALHEIGHT = 111;

		public const int PHYSICALOFFSETX = 112;

		public const int PHYSICALOFFSETY = 113;

		public const int VERTRES = 10;

		public const int HORZRES = 8;

		public const int DM_ORIENTATION = 1;

		public const int DM_PAPERSIZE = 2;

		public const int DM_PAPERLENGTH = 4;

		public const int DM_PAPERWIDTH = 8;

		public const int DM_COPIES = 256;

		public const int DM_DEFAULTSOURCE = 512;

		public const int DM_PRINTQUALITY = 1024;

		public const int DM_COLOR = 2048;

		public const int DM_DUPLEX = 4096;

		public const int DM_YRESOLUTION = 8192;

		public const int DM_COLLATE = 32768;

		public const int DMORIENT_PORTRAIT = 1;

		public const int DMORIENT_LANDSCAPE = 2;

		public const int DMPAPER_LETTER = 1;

		public const int DMPAPER_LETTERSMALL = 2;

		public const int DMPAPER_TABLOID = 3;

		public const int DMPAPER_LEDGER = 4;

		public const int DMPAPER_LEGAL = 5;

		public const int DMPAPER_STATEMENT = 6;

		public const int DMPAPER_EXECUTIVE = 7;

		public const int DMPAPER_A3 = 8;

		public const int DMPAPER_A4 = 9;

		public const int DMPAPER_A4SMALL = 10;

		public const int DMPAPER_A5 = 11;

		public const int DMPAPER_B4 = 12;

		public const int DMPAPER_B5 = 13;

		public const int DMPAPER_FOLIO = 14;

		public const int DMPAPER_QUARTO = 15;

		public const int DMPAPER_10X14 = 16;

		public const int DMPAPER_11X17 = 17;

		public const int DMPAPER_NOTE = 18;

		public const int DMPAPER_ENV_9 = 19;

		public const int DMPAPER_ENV_10 = 20;

		public const int DMPAPER_ENV_11 = 21;

		public const int DMPAPER_ENV_12 = 22;

		public const int DMPAPER_ENV_14 = 23;

		public const int DMPAPER_CSHEET = 24;

		public const int DMPAPER_DSHEET = 25;

		public const int DMPAPER_ESHEET = 26;

		public const int DMPAPER_ENV_DL = 27;

		public const int DMPAPER_ENV_C5 = 28;

		public const int DMPAPER_ENV_C3 = 29;

		public const int DMPAPER_ENV_C4 = 30;

		public const int DMPAPER_ENV_C6 = 31;

		public const int DMPAPER_ENV_C65 = 32;

		public const int DMPAPER_ENV_B4 = 33;

		public const int DMPAPER_ENV_B5 = 34;

		public const int DMPAPER_ENV_B6 = 35;

		public const int DMPAPER_ENV_ITALY = 36;

		public const int DMPAPER_ENV_MONARCH = 37;

		public const int DMPAPER_ENV_PERSONAL = 38;

		public const int DMPAPER_FANFOLD_US = 39;

		public const int DMPAPER_FANFOLD_STD_GERMAN = 40;

		public const int DMPAPER_FANFOLD_LGL_GERMAN = 41;

		public const int DMPAPER_ISO_B4 = 42;

		public const int DMPAPER_JAPANESE_POSTCARD = 43;

		public const int DMPAPER_9X11 = 44;

		public const int DMPAPER_10X11 = 45;

		public const int DMPAPER_15X11 = 46;

		public const int DMPAPER_ENV_INVITE = 47;

		public const int DMPAPER_RESERVED_48 = 48;

		public const int DMPAPER_RESERVED_49 = 49;

		public const int DMPAPER_LETTER_EXTRA = 50;

		public const int DMPAPER_LEGAL_EXTRA = 51;

		public const int DMPAPER_TABLOID_EXTRA = 52;

		public const int DMPAPER_A4_EXTRA = 53;

		public const int DMPAPER_LETTER_TRANSVERSE = 54;

		public const int DMPAPER_A4_TRANSVERSE = 55;

		public const int DMPAPER_LETTER_EXTRA_TRANSVERSE = 56;

		public const int DMPAPER_A_PLUS = 57;

		public const int DMPAPER_B_PLUS = 58;

		public const int DMPAPER_LETTER_PLUS = 59;

		public const int DMPAPER_A4_PLUS = 60;

		public const int DMPAPER_A5_TRANSVERSE = 61;

		public const int DMPAPER_B5_TRANSVERSE = 62;

		public const int DMPAPER_A3_EXTRA = 63;

		public const int DMPAPER_A5_EXTRA = 64;

		public const int DMPAPER_B5_EXTRA = 65;

		public const int DMPAPER_A2 = 66;

		public const int DMPAPER_A3_TRANSVERSE = 67;

		public const int DMPAPER_A3_EXTRA_TRANSVERSE = 68;

		public const int DMPAPER_DBL_JAPANESE_POSTCARD = 69;

		public const int DMPAPER_A6 = 70;

		public const int DMPAPER_JENV_KAKU2 = 71;

		public const int DMPAPER_JENV_KAKU3 = 72;

		public const int DMPAPER_JENV_CHOU3 = 73;

		public const int DMPAPER_JENV_CHOU4 = 74;

		public const int DMPAPER_LETTER_ROTATED = 75;

		public const int DMPAPER_A3_ROTATED = 76;

		public const int DMPAPER_A4_ROTATED = 77;

		public const int DMPAPER_A5_ROTATED = 78;

		public const int DMPAPER_B4_JIS_ROTATED = 79;

		public const int DMPAPER_B5_JIS_ROTATED = 80;

		public const int DMPAPER_JAPANESE_POSTCARD_ROTATED = 81;

		public const int DMPAPER_DBL_JAPANESE_POSTCARD_ROTATED = 82;

		public const int DMPAPER_A6_ROTATED = 83;

		public const int DMPAPER_JENV_KAKU2_ROTATED = 84;

		public const int DMPAPER_JENV_KAKU3_ROTATED = 85;

		public const int DMPAPER_JENV_CHOU3_ROTATED = 86;

		public const int DMPAPER_JENV_CHOU4_ROTATED = 87;

		public const int DMPAPER_B6_JIS = 88;

		public const int DMPAPER_B6_JIS_ROTATED = 89;

		public const int DMPAPER_12X11 = 90;

		public const int DMPAPER_JENV_YOU4 = 91;

		public const int DMPAPER_JENV_YOU4_ROTATED = 92;

		public const int DMPAPER_P16K = 93;

		public const int DMPAPER_P32K = 94;

		public const int DMPAPER_P32KBIG = 95;

		public const int DMPAPER_PENV_1 = 96;

		public const int DMPAPER_PENV_2 = 97;

		public const int DMPAPER_PENV_3 = 98;

		public const int DMPAPER_PENV_4 = 99;

		public const int DMPAPER_PENV_5 = 100;

		public const int DMPAPER_PENV_6 = 101;

		public const int DMPAPER_PENV_7 = 102;

		public const int DMPAPER_PENV_8 = 103;

		public const int DMPAPER_PENV_9 = 104;

		public const int DMPAPER_PENV_10 = 105;

		public const int DMPAPER_P16K_ROTATED = 106;

		public const int DMPAPER_P32K_ROTATED = 107;

		public const int DMPAPER_P32KBIG_ROTATED = 108;

		public const int DMPAPER_PENV_1_ROTATED = 109;

		public const int DMPAPER_PENV_2_ROTATED = 110;

		public const int DMPAPER_PENV_3_ROTATED = 111;

		public const int DMPAPER_PENV_4_ROTATED = 112;

		public const int DMPAPER_PENV_5_ROTATED = 113;

		public const int DMPAPER_PENV_6_ROTATED = 114;

		public const int DMPAPER_PENV_7_ROTATED = 115;

		public const int DMPAPER_PENV_8_ROTATED = 116;

		public const int DMPAPER_PENV_9_ROTATED = 117;

		public const int DMPAPER_PENV_10_ROTATED = 118;

		public const int DMPAPER_LAST = 118;

		public const int DMBIN_UPPER = 1;

		public const int DMBIN_LOWER = 2;

		public const int DMBIN_MIDDLE = 3;

		public const int DMBIN_MANUAL = 4;

		public const int DMBIN_ENVELOPE = 5;

		public const int DMBIN_ENVMANUAL = 6;

		public const int DMBIN_AUTO = 7;

		public const int DMBIN_TRACTOR = 8;

		public const int DMBIN_SMALLFMT = 9;

		public const int DMBIN_LARGEFMT = 10;

		public const int DMBIN_LARGECAPACITY = 11;

		public const int DMBIN_CASSETTE = 14;

		public const int DMBIN_FORMSOURCE = 15;

		public const int DMBIN_LAST = 15;

		public const int DMBIN_USER = 256;

		public const int DMRES_DRAFT = -1;

		public const int DMRES_LOW = -2;

		public const int DMRES_MEDIUM = -3;

		public const int DMRES_HIGH = -4;

		public const int DMCOLOR_MONOCHROME = 1;

		public const int DMCOLOR_COLOR = 2;

		public const int DMDUP_SIMPLEX = 1;

		public const int DMDUP_VERTICAL = 2;

		public const int DMDUP_HORIZONTAL = 3;

		public const int DMCOLLATE_FALSE = 0;

		public const int DMCOLLATE_TRUE = 1;

		public const int PRINTER_ENUM_LOCAL = 2;

		public const int PRINTER_ENUM_CONNECTIONS = 4;

		public const int SRCPAINT = 15597702;

		public const int SRCAND = 8913094;

		public const int SRCINVERT = 6684742;

		public const int SRCERASE = 4457256;

		public const int NOTSRCCOPY = 3342344;

		public const int NOTSRCERASE = 1114278;

		public const int MERGECOPY = 12583114;

		public const int MERGEPAINT = 12255782;

		public const int PATCOPY = 15728673;

		public const int PATPAINT = 16452105;

		public const int PATINVERT = 5898313;

		public const int DSTINVERT = 5570569;

		public const int BLACKNESS = 66;

		public const int WHITENESS = 16711778;

		public const int CAPTUREBLT = 1073741824;

		public const int SM_CXICON = 11;

		public const int SM_CYICON = 12;

		public const int DEFAULT_CHARSET = 1;

		public const int NOMIRRORBITMAP = -2147483648;

		public const int QUERYESCSUPPORT = 8;

		public const int CHECKJPEGFORMAT = 4119;

		public const int CHECKPNGFORMAT = 4120;

		public const int ERROR_ACCESS_DENIED = 5;

		public const int ERROR_INVALID_PARAMETER = 87;

		public const int ERROR_PROC_NOT_FOUND = 127;

		internal class Gdip : GDIPlus
		{
			static Gdip()
			{
				AppDomain currentDomain = AppDomain.CurrentDomain;
				currentDomain.ProcessExit += SafeNativeMethods.Gdip.OnProcessExit;
				if (!currentDomain.IsDefaultAppDomain())
				{
					currentDomain.DomainUnload += SafeNativeMethods.Gdip.OnProcessExit;
				}
			}

			private static bool Initialized
			{
				get
				{
					return SafeNativeMethods.Gdip.s_initToken != IntPtr.Zero;
				}
			}

			internal static IDictionary ThreadData
			{
				get
				{
					LocalDataStoreSlot namedDataSlot = Thread.GetNamedDataSlot("system.drawing.threaddata");
					IDictionary dictionary = (IDictionary)Thread.GetData(namedDataSlot);
					if (dictionary == null)
					{
						dictionary = new Hashtable();
						Thread.SetData(namedDataSlot, dictionary);
					}
					return dictionary;
				}
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			private static void ClearThreadData()
			{
				Thread.SetData(Thread.GetNamedDataSlot("system.drawing.threaddata"), null);
			}

			private static void Shutdown()
			{
				if (SafeNativeMethods.Gdip.Initialized)
				{
					SafeNativeMethods.Gdip.ClearThreadData();
					AppDomain currentDomain = AppDomain.CurrentDomain;
					currentDomain.ProcessExit -= SafeNativeMethods.Gdip.OnProcessExit;
					if (!currentDomain.IsDefaultAppDomain())
					{
						currentDomain.DomainUnload -= SafeNativeMethods.Gdip.OnProcessExit;
					}
				}
			}

			[PrePrepareMethod]
			private static void OnProcessExit(object sender, EventArgs e)
			{
				SafeNativeMethods.Gdip.Shutdown();
			}

			internal static void DummyFunction()
			{
			}

			internal static void CheckStatus(int status)
			{
				if (status != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(status);
				}
			}

			internal static Exception StatusException(int status)
			{
				switch (status)
				{
				case 1:
					return new ExternalException(SR.Format("A generic error occurred in GDI+.", Array.Empty<object>()), -2147467259);
				case 2:
					return new ArgumentException(SR.Format("Parameter is not valid.", Array.Empty<object>()));
				case 3:
					return new OutOfMemoryException(SR.Format("Out of memory.", Array.Empty<object>()));
				case 4:
					return new InvalidOperationException(SR.Format("Object is currently in use elsewhere.", Array.Empty<object>()));
				case 5:
					return new OutOfMemoryException(SR.Format("Buffer is too small (internal GDI+ error).", Array.Empty<object>()));
				case 6:
					return new NotImplementedException(SR.Format("Not implemented.", Array.Empty<object>()));
				case 7:
					return new ExternalException(SR.Format("A generic error occurred in GDI+.", Array.Empty<object>()), -2147467259);
				case 8:
					return new InvalidOperationException(SR.Format("Bitmap region is already locked.", Array.Empty<object>()));
				case 9:
					return new ExternalException(SR.Format("Function was ended.", Array.Empty<object>()), -2147467260);
				case 10:
					return new FileNotFoundException(SR.Format("File not found.", Array.Empty<object>()));
				case 11:
					return new OverflowException(SR.Format("Overflow error.", Array.Empty<object>()));
				case 12:
					return new ExternalException(SR.Format("File access is denied.", Array.Empty<object>()), -2147024891);
				case 13:
					return new ArgumentException(SR.Format("Image format is unknown.", Array.Empty<object>()));
				case 14:
					return new ArgumentException(SR.Format("Font '{0}' cannot be found.", new object[] { "?" }));
				case 15:
					return new ArgumentException(SR.Format("Font '{0}' does not support style '{1}'.", new object[] { "?", "?" }));
				case 16:
					return new ArgumentException(SR.Format("Only TrueType fonts are supported. This is not a TrueType font.", Array.Empty<object>()));
				case 17:
					return new ExternalException(SR.Format("Current version of GDI+ does not support this feature.", Array.Empty<object>()), -2147467259);
				case 18:
					return new ExternalException(SR.Format("GDI+ is not properly initialized (internal GDI+ error).", Array.Empty<object>()), -2147467259);
				case 19:
					return new ArgumentException(SR.Format("Property cannot be found.", Array.Empty<object>()));
				case 20:
					return new ArgumentException(SR.Format("Property is not supported.", Array.Empty<object>()));
				default:
					return new ExternalException(SR.Format("Unknown GDI+ error occurred.", Array.Empty<object>()), -2147418113);
				}
			}

			internal static PointF[] ConvertGPPOINTFArrayF(IntPtr memory, int count)
			{
				if (memory == IntPtr.Zero)
				{
					throw new ArgumentNullException("memory");
				}
				PointF[] array = new PointF[count];
				Type typeFromHandle = typeof(GPPOINTF);
				int num = Marshal.SizeOf(typeFromHandle);
				for (int i = 0; i < count; i++)
				{
					GPPOINTF gppointf = (GPPOINTF)Marshal.PtrToStructure((IntPtr)((long)memory + (long)(i * num)), typeFromHandle);
					array[i] = new PointF(gppointf.X, gppointf.Y);
				}
				return array;
			}

			internal static Point[] ConvertGPPOINTArray(IntPtr memory, int count)
			{
				if (memory == IntPtr.Zero)
				{
					throw new ArgumentNullException("memory");
				}
				Point[] array = new Point[count];
				Type typeFromHandle = typeof(GPPOINT);
				int num = Marshal.SizeOf(typeFromHandle);
				for (int i = 0; i < count; i++)
				{
					GPPOINT gppoint = (GPPOINT)Marshal.PtrToStructure((IntPtr)((long)memory + (long)(i * num)), typeFromHandle);
					array[i] = new Point(gppoint.X, gppoint.Y);
				}
				return array;
			}

			internal static IntPtr ConvertPointToMemory(PointF[] points)
			{
				if (points == null)
				{
					throw new ArgumentNullException("points");
				}
				int num = Marshal.SizeOf(typeof(GPPOINTF));
				int num2 = points.Length;
				IntPtr intPtr = Marshal.AllocHGlobal(checked(num2 * num));
				for (int i = 0; i < num2; i++)
				{
					checked
					{
						Marshal.StructureToPtr<GPPOINTF>(new GPPOINTF(points[i]), (IntPtr)((long)intPtr + unchecked((long)(checked(i * num)))), false);
					}
				}
				return intPtr;
			}

			internal static IntPtr ConvertPointToMemory(Point[] points)
			{
				if (points == null)
				{
					throw new ArgumentNullException("points");
				}
				int num = Marshal.SizeOf(typeof(GPPOINT));
				int num2 = points.Length;
				IntPtr intPtr = Marshal.AllocHGlobal(checked(num2 * num));
				for (int i = 0; i < num2; i++)
				{
					checked
					{
						Marshal.StructureToPtr<GPPOINT>(new GPPOINT(points[i]), (IntPtr)((long)intPtr + unchecked((long)(checked(i * num)))), false);
					}
				}
				return intPtr;
			}

			internal static IntPtr ConvertRectangleToMemory(RectangleF[] rect)
			{
				if (rect == null)
				{
					throw new ArgumentNullException("rect");
				}
				int num = Marshal.SizeOf(typeof(GPRECTF));
				int num2 = rect.Length;
				IntPtr intPtr = Marshal.AllocHGlobal(checked(num2 * num));
				for (int i = 0; i < num2; i++)
				{
					checked
					{
						Marshal.StructureToPtr<GPRECTF>(new GPRECTF(rect[i]), (IntPtr)((long)intPtr + unchecked((long)(checked(i * num)))), false);
					}
				}
				return intPtr;
			}

			internal static IntPtr ConvertRectangleToMemory(Rectangle[] rect)
			{
				if (rect == null)
				{
					throw new ArgumentNullException("rect");
				}
				int num = Marshal.SizeOf(typeof(GPRECT));
				int num2 = rect.Length;
				IntPtr intPtr = Marshal.AllocHGlobal(checked(num2 * num));
				for (int i = 0; i < num2; i++)
				{
					checked
					{
						Marshal.StructureToPtr<GPRECT>(new GPRECT(rect[i]), (IntPtr)((long)intPtr + unchecked((long)(checked(i * num)))), false);
					}
				}
				return intPtr;
			}

			private static readonly TraceSwitch s_gdiPlusInitialization = new TraceSwitch("GdiPlusInitialization", "Tracks GDI+ initialization and teardown");

			private static IntPtr s_initToken = (IntPtr)1;

			private const string ThreadDataSlotName = "system.drawing.threaddata";

			internal const int Ok = 0;

			internal const int GenericError = 1;

			internal const int InvalidParameter = 2;

			internal const int OutOfMemory = 3;

			internal const int ObjectBusy = 4;

			internal const int InsufficientBuffer = 5;

			internal const int NotImplemented = 6;

			internal const int Win32Error = 7;

			internal const int WrongState = 8;

			internal const int Aborted = 9;

			internal const int FileNotFound = 10;

			internal const int ValueOverflow = 11;

			internal const int AccessDenied = 12;

			internal const int UnknownImageFormat = 13;

			internal const int FontFamilyNotFound = 14;

			internal const int FontStyleNotFound = 15;

			internal const int NotTrueTypeFont = 16;

			internal const int UnsupportedGdiplusVersion = 17;

			internal const int GdiplusNotInitialized = 18;

			internal const int PropertyNotFound = 19;

			internal const int PropertyNotSupported = 20;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class ENHMETAHEADER
		{
			public int iType;

			public int nSize = 40;

			public int rclBounds_left;

			public int rclBounds_top;

			public int rclBounds_right;

			public int rclBounds_bottom;

			public int rclFrame_left;

			public int rclFrame_top;

			public int rclFrame_right;

			public int rclFrame_bottom;

			public int dSignature;

			public int nVersion;

			public int nBytes;

			public int nRecords;

			public short nHandles;

			public short sReserved;

			public int nDescription;

			public int offDescription;

			public int nPalEntries;

			public int szlDevice_cx;

			public int szlDevice_cy;

			public int szlMillimeters_cx;

			public int szlMillimeters_cy;

			public int cbPixelFormat;

			public int offPixelFormat;

			public int bOpenGL;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class DOCINFO
		{
			public int cbSize = 20;

			public string lpszDocName;

			public string lpszOutput;

			public string lpszDatatype;

			public int fwType;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class PRINTDLG
		{
			public int lStructSize;

			public IntPtr hwndOwner;

			public IntPtr hDevMode;

			public IntPtr hDevNames;

			public IntPtr hDC;

			public int Flags;

			public short nFromPage;

			public short nToPage;

			public short nMinPage;

			public short nMaxPage;

			public short nCopies;

			public IntPtr hInstance;

			public IntPtr lCustData;

			public IntPtr lpfnPrintHook;

			public IntPtr lpfnSetupHook;

			public string lpPrintTemplateName;

			public string lpSetupTemplateName;

			public IntPtr hPrintTemplate;

			public IntPtr hSetupTemplate;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
		public class PRINTDLGX86
		{
			public int lStructSize;

			public IntPtr hwndOwner;

			public IntPtr hDevMode;

			public IntPtr hDevNames;

			public IntPtr hDC;

			public int Flags;

			public short nFromPage;

			public short nToPage;

			public short nMinPage;

			public short nMaxPage;

			public short nCopies;

			public IntPtr hInstance;

			public IntPtr lCustData;

			public IntPtr lpfnPrintHook;

			public IntPtr lpfnSetupHook;

			public string lpPrintTemplateName;

			public string lpSetupTemplateName;

			public IntPtr hPrintTemplate;

			public IntPtr hSetupTemplate;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class ICONINFO
		{
			public int fIcon;

			public int xHotspot;

			public int yHotspot;

			public IntPtr hbmMask = IntPtr.Zero;

			public IntPtr hbmColor = IntPtr.Zero;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class BITMAP
		{
			public int bmType;

			public int bmWidth;

			public int bmHeight;

			public int bmWidthBytes;

			public short bmPlanes;

			public short bmBitsPixel;

			public IntPtr bmBits = IntPtr.Zero;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class BITMAPINFOHEADER
		{
			public int biSize = 40;

			public int biWidth;

			public int biHeight;

			public short biPlanes;

			public short biBitCount;

			public int biCompression;

			public int biSizeImage;

			public int biXPelsPerMeter;

			public int biYPelsPerMeter;

			public int biClrUsed;

			public int biClrImportant;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class LOGFONT
		{
			public override string ToString()
			{
				return string.Concat(new string[]
				{
					"lfHeight=",
					this.lfHeight.ToString(),
					", lfWidth=",
					this.lfWidth.ToString(),
					", lfEscapement=",
					this.lfEscapement.ToString(),
					", lfOrientation=",
					this.lfOrientation.ToString(),
					", lfWeight=",
					this.lfWeight.ToString(),
					", lfItalic=",
					this.lfItalic.ToString(),
					", lfUnderline=",
					this.lfUnderline.ToString(),
					", lfStrikeOut=",
					this.lfStrikeOut.ToString(),
					", lfCharSet=",
					this.lfCharSet.ToString(),
					", lfOutPrecision=",
					this.lfOutPrecision.ToString(),
					", lfClipPrecision=",
					this.lfClipPrecision.ToString(),
					", lfQuality=",
					this.lfQuality.ToString(),
					", lfPitchAndFamily=",
					this.lfPitchAndFamily.ToString(),
					", lfFaceName=",
					this.lfFaceName
				});
			}

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

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string lfFaceName;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		public struct ICONDIR
		{
			public short idReserved;

			public short idType;

			public short idCount;

			public SafeNativeMethods.ICONDIRENTRY idEntries;
		}

		public struct ICONDIRENTRY
		{
			public byte bWidth;

			public byte bHeight;

			public byte bColorCount;

			public byte bReserved;

			public short wPlanes;

			public short wBitCount;

			public int dwBytesInRes;

			public int dwImageOffset;
		}

		public class Ole
		{
			public const int PICTYPE_ICON = 3;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class PICTDESC
		{
			public static SafeNativeMethods.PICTDESC CreateIconPICTDESC(IntPtr hicon)
			{
				return new SafeNativeMethods.PICTDESC
				{
					cbSizeOfStruct = 12,
					picType = 3,
					union1 = hicon
				};
			}

			internal int cbSizeOfStruct;

			public int picType;

			internal IntPtr union1;

			internal int union2;

			internal int union3;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class DEVMODE
		{
			public override string ToString()
			{
				return string.Concat(new string[]
				{
					"[DEVMODE: dmDeviceName=",
					this.dmDeviceName,
					", dmSpecVersion=",
					this.dmSpecVersion.ToString(),
					", dmDriverVersion=",
					this.dmDriverVersion.ToString(),
					", dmSize=",
					this.dmSize.ToString(),
					", dmDriverExtra=",
					this.dmDriverExtra.ToString(),
					", dmFields=",
					this.dmFields.ToString(),
					", dmOrientation=",
					this.dmOrientation.ToString(),
					", dmPaperSize=",
					this.dmPaperSize.ToString(),
					", dmPaperLength=",
					this.dmPaperLength.ToString(),
					", dmPaperWidth=",
					this.dmPaperWidth.ToString(),
					", dmScale=",
					this.dmScale.ToString(),
					", dmCopies=",
					this.dmCopies.ToString(),
					", dmDefaultSource=",
					this.dmDefaultSource.ToString(),
					", dmPrintQuality=",
					this.dmPrintQuality.ToString(),
					", dmColor=",
					this.dmColor.ToString(),
					", dmDuplex=",
					this.dmDuplex.ToString(),
					", dmYResolution=",
					this.dmYResolution.ToString(),
					", dmTTOption=",
					this.dmTTOption.ToString(),
					", dmCollate=",
					this.dmCollate.ToString(),
					", dmFormName=",
					this.dmFormName,
					", dmLogPixels=",
					this.dmLogPixels.ToString(),
					", dmBitsPerPel=",
					this.dmBitsPerPel.ToString(),
					", dmPelsWidth=",
					this.dmPelsWidth.ToString(),
					", dmPelsHeight=",
					this.dmPelsHeight.ToString(),
					", dmDisplayFlags=",
					this.dmDisplayFlags.ToString(),
					", dmDisplayFrequency=",
					this.dmDisplayFrequency.ToString(),
					", dmICMMethod=",
					this.dmICMMethod.ToString(),
					", dmICMIntent=",
					this.dmICMIntent.ToString(),
					", dmMediaType=",
					this.dmMediaType.ToString(),
					", dmDitherType=",
					this.dmDitherType.ToString(),
					", dmICCManufacturer=",
					this.dmICCManufacturer.ToString(),
					", dmICCModel=",
					this.dmICCModel.ToString(),
					", dmPanningWidth=",
					this.dmPanningWidth.ToString(),
					", dmPanningHeight=",
					this.dmPanningHeight.ToString(),
					"]"
				});
			}

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
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

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string dmFormName;

			public short dmLogPixels;

			public int dmBitsPerPel;

			public int dmPelsWidth;

			public int dmPelsHeight;

			public int dmDisplayFlags;

			public int dmDisplayFrequency;

			public int dmICMMethod;

			public int dmICMIntent;

			public int dmMediaType;

			public int dmDitherType;

			public int dmICCManufacturer;

			public int dmICCModel;

			public int dmPanningWidth;

			public int dmPanningHeight;
		}

		public sealed class CommonHandles
		{
			public static readonly int GDI = global::System.Internal.HandleCollector.RegisterType("GDI", 50, 500);

			public static readonly int HDC = global::System.Internal.HandleCollector.RegisterType("HDC", 100, 2);

			public static readonly int Icon = global::System.Internal.HandleCollector.RegisterType("Icon", 20, 500);

			public static readonly int Kernel = global::System.Internal.HandleCollector.RegisterType("Kernel", 0, 1000);
		}

		public class StreamConsts
		{
			public const int STREAM_SEEK_SET = 0;

			public const int STREAM_SEEK_CUR = 1;

			public const int STREAM_SEEK_END = 2;
		}

		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[Guid("7BF80980-BF32-101A-8BBB-00AA00300CAB")]
		[ComImport]
		public interface IPicture
		{
			IntPtr GetHandle();

			IntPtr GetHPal();

			[return: MarshalAs(UnmanagedType.I2)]
			short GetPictureType();

			int GetWidth();

			int GetHeight();

			void Render();

			void SetHPal([In] IntPtr phpal);

			IntPtr GetCurDC();

			void SelectPicture([In] IntPtr hdcIn, [MarshalAs(UnmanagedType.LPArray)] [Out] int[] phdcOut, [MarshalAs(UnmanagedType.LPArray)] [Out] int[] phbmpOut);

			[return: MarshalAs(UnmanagedType.Bool)]
			bool GetKeepOriginalFormat();

			void SetKeepOriginalFormat([MarshalAs(UnmanagedType.Bool)] [In] bool pfkeep);

			void PictureChanged();

			[PreserveSig]
			int SaveAsFile([MarshalAs(UnmanagedType.Interface)] [In] UnsafeNativeMethods.IStream pstm, [In] int fSaveMemCopy, out int pcbSize);

			int GetAttributes();

			void SetHdc([In] IntPtr hdc);
		}
	}
}
