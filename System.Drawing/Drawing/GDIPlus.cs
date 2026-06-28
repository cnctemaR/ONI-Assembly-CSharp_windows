using System;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;

namespace System.Drawing
{
	[SuppressUnmanagedCodeSecurity]
	internal static class GDIPlus
	{
		static GDIPlus()
		{
			int platform = (int)Environment.OSVersion.Platform;
			if (platform == 4 || platform == 6 || platform == 128)
			{
				if (Environment.GetEnvironmentVariable("not_supported_MONO_MWF_USE_NEW_X11_BACKEND") != null || Environment.GetEnvironmentVariable("MONO_MWF_MAC_FORCE_X11") != null)
				{
					GDIPlus.UseX11Drawable = true;
				}
				else
				{
					IntPtr intPtr = Marshal.AllocHGlobal(8192);
					if (GDIPlus.uname(intPtr) != 0)
					{
						GDIPlus.UseX11Drawable = true;
					}
					else
					{
						string text = Marshal.PtrToStringAnsi(intPtr);
						if (text == "Darwin")
						{
							GDIPlus.UseCarbonDrawable = true;
						}
						else
						{
							GDIPlus.UseX11Drawable = true;
						}
					}
					Marshal.FreeHGlobal(intPtr);
				}
			}
			GdiplusStartupInput gdiplusStartupInput = GdiplusStartupInput.MakeGdiplusStartupInput();
			GdiplusStartupOutput gdiplusStartupOutput = GdiplusStartupOutput.MakeGdiplusStartupOutput();
			try
			{
				GDIPlus.GdiplusStartup(ref GDIPlus.GdiPlusToken, ref gdiplusStartupInput, ref gdiplusStartupOutput);
			}
			catch (TypeInitializationException)
			{
				Console.Error.WriteLine("* ERROR: Can not initialize GDI+ library{0}{0}Please check http://www.mono-project.com/Problem:GDIPlusInit for details", Environment.NewLine);
			}
			AppDomain.CurrentDomain.ProcessExit += GDIPlus.ProcessExit;
		}

		[DllImport("gdiplus.dll")]
		internal static extern Status GdiplusStartup(ref ulong token, ref GdiplusStartupInput input, ref GdiplusStartupOutput output);

		[DllImport("gdiplus.dll")]
		internal static extern void GdiplusShutdown(ref ulong token);

		private static void ProcessExit(object sender, EventArgs e)
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public static bool RunningOnWindows()
		{
			return !GDIPlus.UseX11Drawable && !GDIPlus.UseCarbonDrawable;
		}

		public static bool RunningOnUnix()
		{
			return GDIPlus.UseX11Drawable || GDIPlus.UseCarbonDrawable;
		}

		public static void FromUnManagedMemoryToPointI(IntPtr prt, Point[] pts)
		{
			int num = Marshal.SizeOf(pts[0]);
			IntPtr intPtr = prt;
			int i = 0;
			while (i < pts.Length)
			{
				pts[i] = (Point)Marshal.PtrToStructure(intPtr, typeof(Point));
				i++;
				intPtr = new IntPtr(intPtr.ToInt64() + (long)num);
			}
			Marshal.FreeHGlobal(prt);
		}

		public static void FromUnManagedMemoryToPoint(IntPtr prt, PointF[] pts)
		{
			int num = Marshal.SizeOf(pts[0]);
			IntPtr intPtr = prt;
			int i = 0;
			while (i < pts.Length)
			{
				pts[i] = (PointF)Marshal.PtrToStructure(intPtr, typeof(PointF));
				i++;
				intPtr = new IntPtr(intPtr.ToInt64() + (long)num);
			}
			Marshal.FreeHGlobal(prt);
		}

		public static IntPtr FromPointToUnManagedMemoryI(Point[] pts)
		{
			int num = Marshal.SizeOf(pts[0]);
			IntPtr intPtr = Marshal.AllocHGlobal(num * pts.Length);
			IntPtr intPtr2 = intPtr;
			int i = 0;
			while (i < pts.Length)
			{
				Marshal.StructureToPtr(pts[i], intPtr2, false);
				i++;
				intPtr2 = new IntPtr(intPtr2.ToInt64() + (long)num);
			}
			return intPtr;
		}

		public static void FromUnManagedMemoryToRectangles(IntPtr prt, RectangleF[] pts)
		{
			int num = Marshal.SizeOf(pts[0]);
			IntPtr intPtr = prt;
			int i = 0;
			while (i < pts.Length)
			{
				pts[i] = (RectangleF)Marshal.PtrToStructure(intPtr, typeof(RectangleF));
				i++;
				intPtr = new IntPtr(intPtr.ToInt64() + (long)num);
			}
			Marshal.FreeHGlobal(prt);
		}

		public static IntPtr FromPointToUnManagedMemory(PointF[] pts)
		{
			int num = Marshal.SizeOf(pts[0]);
			IntPtr intPtr = Marshal.AllocHGlobal(num * pts.Length);
			IntPtr intPtr2 = intPtr;
			int i = 0;
			while (i < pts.Length)
			{
				Marshal.StructureToPtr(pts[i], intPtr2, false);
				i++;
				intPtr2 = new IntPtr(intPtr2.ToInt64() + (long)num);
			}
			return intPtr;
		}

		internal static void CheckStatus(Status status)
		{
			string text;
			switch (status)
			{
			case Status.Ok:
				return;
			case Status.GenericError:
				text = Locale.GetText("Generic Error [GDI+ status: {0}]", new object[] { status });
				throw new Exception(text);
			case Status.InvalidParameter:
				text = Locale.GetText("A null reference or invalid value was found [GDI+ status: {0}]", new object[] { status });
				throw new ArgumentException(text);
			case Status.OutOfMemory:
				text = Locale.GetText("Not enough memory to complete operation [GDI+ status: {0}]", new object[] { status });
				throw new OutOfMemoryException(text);
			case Status.ObjectBusy:
				text = Locale.GetText("Object is busy and cannot state allow this operation [GDI+ status: {0}]", new object[] { status });
				throw new MemberAccessException(text);
			case Status.InsufficientBuffer:
				text = Locale.GetText("Insufficient buffer provided to complete operation [GDI+ status: {0}]", new object[] { status });
				throw new InternalBufferOverflowException(text);
			case Status.NotImplemented:
				text = Locale.GetText("The requested feature is not implemented [GDI+ status: {0}]", new object[] { status });
				throw new NotImplementedException(text);
			case Status.Win32Error:
				text = Locale.GetText("The operation is invalid [GDI+ status: {0}]", new object[] { status });
				throw new InvalidOperationException(text);
			case Status.WrongState:
				text = Locale.GetText("Object is not in a state that can allow this operation [GDI+ status: {0}]", new object[] { status });
				throw new ArgumentException(text);
			case Status.FileNotFound:
				text = Locale.GetText("Requested file was not found [GDI+ status: {0}]", new object[] { status });
				throw new FileNotFoundException(text);
			case Status.ValueOverflow:
				text = Locale.GetText("Argument is out of range [GDI+ status: {0}]", new object[] { status });
				throw new OverflowException(text);
			case Status.AccessDenied:
				text = Locale.GetText("Access to resource was denied [GDI+ status: {0}]", new object[] { status });
				throw new UnauthorizedAccessException(text);
			case Status.UnknownImageFormat:
				text = Locale.GetText("Either the image format is unknown or you don't have the required libraries to decode this format [GDI+ status: {0}]", new object[] { status });
				throw new NotSupportedException(text);
			case Status.FontFamilyNotFound:
				text = Locale.GetText("The requested FontFamily could not be found [GDI+ status: {0}]", new object[] { status });
				throw new ArgumentException(text);
			case Status.PropertyNotSupported:
				text = Locale.GetText("Property not supported [GDI+ status: {0}]", new object[] { status });
				throw new NotSupportedException(text);
			}
			text = Locale.GetText("Unknown Error [GDI+ status: {0}]", new object[] { status });
			throw new Exception(text);
		}

		[DllImport("gdiplus.dll")]
		internal static extern IntPtr GdipAlloc(int size);

		[DllImport("gdiplus.dll")]
		internal static extern void GdipFree(IntPtr ptr);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCloneBrush(IntPtr brush, out IntPtr clonedBrush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDeleteBrush(IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetBrushType(IntPtr brush, out BrushType type);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateRegion(out IntPtr region);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateRegionRgnData(byte[] data, int size, out IntPtr region);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDeleteRegion(IntPtr region);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCloneRegion(IntPtr region, out IntPtr cloned);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateRegionRect(ref RectangleF rect, out IntPtr region);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateRegionRectI(ref Rectangle rect, out IntPtr region);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateRegionPath(IntPtr path, out IntPtr region);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTranslateRegion(IntPtr region, float dx, float dy);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTranslateRegionI(IntPtr region, int dx, int dy);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsVisibleRegionPoint(IntPtr region, float x, float y, IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsVisibleRegionPointI(IntPtr region, int x, int y, IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsVisibleRegionRect(IntPtr region, float x, float y, float width, float height, IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsVisibleRegionRectI(IntPtr region, int x, int y, int width, int height, IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCombineRegionRect(IntPtr region, ref RectangleF rect, CombineMode combineMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCombineRegionRectI(IntPtr region, ref Rectangle rect, CombineMode combineMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCombineRegionPath(IntPtr region, IntPtr path, CombineMode combineMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetRegionBounds(IntPtr region, IntPtr graphics, ref RectangleF rect);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetInfinite(IntPtr region);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetEmpty(IntPtr region);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsEmptyRegion(IntPtr region, IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsInfiniteRegion(IntPtr region, IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCombineRegionRegion(IntPtr region, IntPtr region2, CombineMode combineMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsEqualRegion(IntPtr region, IntPtr region2, IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetRegionDataSize(IntPtr region, out int bufferSize);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetRegionData(IntPtr region, byte[] buffer, int bufferSize, out int sizeFilled);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetRegionScansCount(IntPtr region, out int count, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetRegionScans(IntPtr region, IntPtr rects, out int count, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTransformRegion(IntPtr region, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillRegion(IntPtr graphics, IntPtr brush, IntPtr region);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetRegionHRgn(IntPtr region, IntPtr graphics, ref IntPtr hRgn);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateRegionHrgn(IntPtr hRgn, out IntPtr region);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateSolidFill(int color, out IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetSolidFillColor(IntPtr brush, out int color);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetSolidFillColor(IntPtr brush, int color);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateHatchBrush(HatchStyle hatchstyle, int foreColor, int backColor, out IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetHatchStyle(IntPtr brush, out HatchStyle hatchstyle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetHatchForegroundColor(IntPtr brush, out int foreColor);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetHatchBackgroundColor(IntPtr brush, out int backColor);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetTextureImage(IntPtr texture, out IntPtr image);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateTexture(IntPtr image, WrapMode wrapMode, out IntPtr texture);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateTextureIAI(IntPtr image, IntPtr imageAttributes, int x, int y, int width, int height, out IntPtr texture);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateTextureIA(IntPtr image, IntPtr imageAttributes, float x, float y, float width, float height, out IntPtr texture);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateTexture2I(IntPtr image, WrapMode wrapMode, int x, int y, int width, int height, out IntPtr texture);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateTexture2(IntPtr image, WrapMode wrapMode, float x, float y, float width, float height, out IntPtr texture);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetTextureTransform(IntPtr texture, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetTextureTransform(IntPtr texture, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetTextureWrapMode(IntPtr texture, out WrapMode wrapMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetTextureWrapMode(IntPtr texture, WrapMode wrapMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipMultiplyTextureTransform(IntPtr texture, IntPtr matrix, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipResetTextureTransform(IntPtr texture);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRotateTextureTransform(IntPtr texture, float angle, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipScaleTextureTransform(IntPtr texture, float sx, float sy, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTranslateTextureTransform(IntPtr texture, float dx, float dy, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreatePathGradientFromPath(IntPtr path, out IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreatePathGradientI(Point[] points, int count, WrapMode wrapMode, out IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreatePathGradient(PointF[] points, int count, WrapMode wrapMode, out IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathGradientBlendCount(IntPtr brush, out int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathGradientBlend(IntPtr brush, float[] blend, float[] positions, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPathGradientBlend(IntPtr brush, float[] blend, float[] positions, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathGradientCenterColor(IntPtr brush, out int color);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPathGradientCenterColor(IntPtr brush, int color);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathGradientCenterPoint(IntPtr brush, out PointF point);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPathGradientCenterPoint(IntPtr brush, ref PointF point);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathGradientFocusScales(IntPtr brush, out float xScale, out float yScale);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPathGradientFocusScales(IntPtr brush, float xScale, float yScale);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathGradientPresetBlendCount(IntPtr brush, out int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathGradientPresetBlend(IntPtr brush, int[] blend, float[] positions, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPathGradientPresetBlend(IntPtr brush, int[] blend, float[] positions, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathGradientRect(IntPtr brush, out RectangleF rect);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathGradientSurroundColorCount(IntPtr brush, out int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathGradientSurroundColorsWithCount(IntPtr brush, int[] color, ref int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPathGradientSurroundColorsWithCount(IntPtr brush, int[] color, ref int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathGradientTransform(IntPtr brush, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPathGradientTransform(IntPtr brush, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathGradientWrapMode(IntPtr brush, out WrapMode wrapMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPathGradientWrapMode(IntPtr brush, WrapMode wrapMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPathGradientLinearBlend(IntPtr brush, float focus, float scale);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPathGradientSigmaBlend(IntPtr brush, float focus, float scale);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipMultiplyPathGradientTransform(IntPtr texture, IntPtr matrix, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipResetPathGradientTransform(IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRotatePathGradientTransform(IntPtr brush, float angle, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipScalePathGradientTransform(IntPtr brush, float sx, float sy, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTranslatePathGradientTransform(IntPtr brush, float dx, float dy, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateLineBrushI(ref Point point1, ref Point point2, int color1, int color2, WrapMode wrapMode, out IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateLineBrush(ref PointF point1, ref PointF point2, int color1, int color2, WrapMode wrapMode, out IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateLineBrushFromRectI(ref Rectangle rect, int color1, int color2, LinearGradientMode linearGradientMode, WrapMode wrapMode, out IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateLineBrushFromRect(ref RectangleF rect, int color1, int color2, LinearGradientMode linearGradientMode, WrapMode wrapMode, out IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateLineBrushFromRectWithAngleI(ref Rectangle rect, int color1, int color2, float angle, bool isAngleScaleable, WrapMode wrapMode, out IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateLineBrushFromRectWithAngle(ref RectangleF rect, int color1, int color2, float angle, bool isAngleScaleable, WrapMode wrapMode, out IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetLineBlendCount(IntPtr brush, out int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetLineBlend(IntPtr brush, float[] blend, float[] positions, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetLineBlend(IntPtr brush, float[] blend, float[] positions, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetLineGammaCorrection(IntPtr brush, bool useGammaCorrection);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetLineGammaCorrection(IntPtr brush, out bool useGammaCorrection);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetLinePresetBlendCount(IntPtr brush, out int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetLinePresetBlend(IntPtr brush, int[] blend, float[] positions, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetLinePresetBlend(IntPtr brush, int[] blend, float[] positions, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetLineColors(IntPtr brush, int color1, int color2);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetLineColors(IntPtr brush, int[] colors);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetLineRectI(IntPtr brush, out Rectangle rect);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetLineRect(IntPtr brush, out RectangleF rect);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetLineTransform(IntPtr brush, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetLineTransform(IntPtr brush, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetLineWrapMode(IntPtr brush, WrapMode wrapMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetLineWrapMode(IntPtr brush, out WrapMode wrapMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetLineLinearBlend(IntPtr brush, float focus, float scale);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetLineSigmaBlend(IntPtr brush, float focus, float scale);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipMultiplyLineTransform(IntPtr brush, IntPtr matrix, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipResetLineTransform(IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRotateLineTransform(IntPtr brush, float angle, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipScaleLineTransform(IntPtr brush, float sx, float sy, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTranslateLineTransform(IntPtr brush, float dx, float dy, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateFromHDC(IntPtr hDC, out IntPtr graphics);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDeleteGraphics(IntPtr graphics);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRestoreGraphics(IntPtr graphics, uint graphicsState);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSaveGraphics(IntPtr graphics, out uint state);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipMultiplyWorldTransform(IntPtr graphics, IntPtr matrix, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRotateWorldTransform(IntPtr graphics, float angle, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTranslateWorldTransform(IntPtr graphics, float dx, float dy, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawArc(IntPtr graphics, IntPtr pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawArcI(IntPtr graphics, IntPtr pen, int x, int y, int width, int height, float startAngle, float sweepAngle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawBezier(IntPtr graphics, IntPtr pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawBezierI(IntPtr graphics, IntPtr pen, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawEllipseI(IntPtr graphics, IntPtr pen, int x, int y, int width, int height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawEllipse(IntPtr graphics, IntPtr pen, float x, float y, float width, float height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawLine(IntPtr graphics, IntPtr pen, float x1, float y1, float x2, float y2);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawLineI(IntPtr graphics, IntPtr pen, int x1, int y1, int x2, int y2);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawLines(IntPtr graphics, IntPtr pen, PointF[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawLinesI(IntPtr graphics, IntPtr pen, Point[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawPath(IntPtr graphics, IntPtr pen, IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawPie(IntPtr graphics, IntPtr pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawPieI(IntPtr graphics, IntPtr pen, int x, int y, int width, int height, float startAngle, float sweepAngle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawPolygon(IntPtr graphics, IntPtr pen, PointF[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawPolygonI(IntPtr graphics, IntPtr pen, Point[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawRectangle(IntPtr graphics, IntPtr pen, float x, float y, float width, float height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawRectangleI(IntPtr graphics, IntPtr pen, int x, int y, int width, int height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawRectangles(IntPtr graphics, IntPtr pen, RectangleF[] rects, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawRectanglesI(IntPtr graphics, IntPtr pen, Rectangle[] rects, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillEllipseI(IntPtr graphics, IntPtr pen, int x, int y, int width, int height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillEllipse(IntPtr graphics, IntPtr pen, float x, float y, float width, float height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillPolygon(IntPtr graphics, IntPtr brush, PointF[] points, int count, FillMode fillMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillPolygonI(IntPtr graphics, IntPtr brush, Point[] points, int count, FillMode fillMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillPolygon2(IntPtr graphics, IntPtr brush, PointF[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillPolygon2I(IntPtr graphics, IntPtr brush, Point[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillRectangle(IntPtr graphics, IntPtr brush, float x1, float y1, float x2, float y2);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillRectangleI(IntPtr graphics, IntPtr brush, int x1, int y1, int x2, int y2);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillRectangles(IntPtr graphics, IntPtr brush, RectangleF[] rects, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillRectanglesI(IntPtr graphics, IntPtr brush, Rectangle[] rects, int count);

		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode)]
		internal static extern Status GdipDrawString(IntPtr graphics, string text, int len, IntPtr font, ref RectangleF rc, IntPtr format, IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetDC(IntPtr graphics, out IntPtr hdc);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipReleaseDC(IntPtr graphics, IntPtr hdc);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawImageRectI(IntPtr graphics, IntPtr image, int x, int y, int width, int height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetRenderingOrigin(IntPtr graphics, out int x, out int y);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetRenderingOrigin(IntPtr graphics, int x, int y);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCloneBitmapArea(float x, float y, float width, float height, PixelFormat format, IntPtr original, out IntPtr bitmap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCloneBitmapAreaI(int x, int y, int width, int height, PixelFormat format, IntPtr original, out IntPtr bitmap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipResetWorldTransform(IntPtr graphics);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetWorldTransform(IntPtr graphics, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetWorldTransform(IntPtr graphics, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipScaleWorldTransform(IntPtr graphics, float sx, float sy, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGraphicsClear(IntPtr graphics, int argb);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawClosedCurve(IntPtr graphics, IntPtr pen, PointF[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawClosedCurveI(IntPtr graphics, IntPtr pen, Point[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawClosedCurve2(IntPtr graphics, IntPtr pen, PointF[] points, int count, float tension);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawClosedCurve2I(IntPtr graphics, IntPtr pen, Point[] points, int count, float tension);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawCurve(IntPtr graphics, IntPtr pen, PointF[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawCurveI(IntPtr graphics, IntPtr pen, Point[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawCurve2(IntPtr graphics, IntPtr pen, PointF[] points, int count, float tension);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawCurve2I(IntPtr graphics, IntPtr pen, Point[] points, int count, float tension);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawCurve3(IntPtr graphics, IntPtr pen, PointF[] points, int count, int offset, int numberOfSegments, float tension);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawCurve3I(IntPtr graphics, IntPtr pen, Point[] points, int count, int offset, int numberOfSegments, float tension);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetClipRect(IntPtr graphics, float x, float y, float width, float height, CombineMode combineMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetClipRectI(IntPtr graphics, int x, int y, int width, int height, CombineMode combineMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetClipPath(IntPtr graphics, IntPtr path, CombineMode combineMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetClipRegion(IntPtr graphics, IntPtr region, CombineMode combineMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetClipGraphics(IntPtr graphics, IntPtr srcgraphics, CombineMode combineMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipResetClip(IntPtr graphics);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipEndContainer(IntPtr graphics, uint state);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetClip(IntPtr graphics, IntPtr region);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillClosedCurve(IntPtr graphics, IntPtr brush, PointF[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillClosedCurveI(IntPtr graphics, IntPtr brush, Point[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillClosedCurve2(IntPtr graphics, IntPtr brush, PointF[] points, int count, float tension, FillMode fillMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillClosedCurve2I(IntPtr graphics, IntPtr brush, Point[] points, int count, float tension, FillMode fillMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillPie(IntPtr graphics, IntPtr brush, float x, float y, float width, float height, float startAngle, float sweepAngle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillPieI(IntPtr graphics, IntPtr brush, int x, int y, int width, int height, float startAngle, float sweepAngle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFillPath(IntPtr graphics, IntPtr brush, IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetNearestColor(IntPtr graphics, out int argb);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsVisiblePoint(IntPtr graphics, float x, float y, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsVisiblePointI(IntPtr graphics, int x, int y, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsVisibleRect(IntPtr graphics, float x, float y, float width, float height, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsVisibleRectI(IntPtr graphics, int x, int y, int width, int height, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTransformPoints(IntPtr graphics, CoordinateSpace destSpace, CoordinateSpace srcSpace, IntPtr points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTransformPointsI(IntPtr graphics, CoordinateSpace destSpace, CoordinateSpace srcSpace, IntPtr points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTranslateClip(IntPtr graphics, float dx, float dy);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTranslateClipI(IntPtr graphics, int dx, int dy);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetClipBounds(IntPtr graphics, out RectangleF rect);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetCompositingMode(IntPtr graphics, CompositingMode compositingMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetCompositingMode(IntPtr graphics, out CompositingMode compositingMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetCompositingQuality(IntPtr graphics, CompositingQuality compositingQuality);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetCompositingQuality(IntPtr graphics, out CompositingQuality compositingQuality);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetInterpolationMode(IntPtr graphics, InterpolationMode interpolationMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetInterpolationMode(IntPtr graphics, out InterpolationMode interpolationMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetDpiX(IntPtr graphics, out float dpi);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetDpiY(IntPtr graphics, out float dpi);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsClipEmpty(IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsVisibleClipEmpty(IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPageUnit(IntPtr graphics, out GraphicsUnit unit);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPageScale(IntPtr graphics, out float scale);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPageUnit(IntPtr graphics, GraphicsUnit unit);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPageScale(IntPtr graphics, float scale);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPixelOffsetMode(IntPtr graphics, PixelOffsetMode pixelOffsetMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPixelOffsetMode(IntPtr graphics, out PixelOffsetMode pixelOffsetMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetSmoothingMode(IntPtr graphics, SmoothingMode smoothingMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetSmoothingMode(IntPtr graphics, out SmoothingMode smoothingMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetTextContrast(IntPtr graphics, int contrast);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetTextContrast(IntPtr graphics, out int contrast);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetTextRenderingHint(IntPtr graphics, TextRenderingHint mode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetTextRenderingHint(IntPtr graphics, out TextRenderingHint mode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetVisibleClipBounds(IntPtr graphics, out RectangleF rect);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFlush(IntPtr graphics, FlushIntention intention);

		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode)]
		internal static extern Status GdipAddPathString(IntPtr path, string s, int lenght, IntPtr family, int style, float emSize, ref RectangleF layoutRect, IntPtr format);

		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode)]
		internal static extern Status GdipAddPathStringI(IntPtr path, string s, int lenght, IntPtr family, int style, float emSize, ref Rectangle layoutRect, IntPtr format);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreatePen1(int argb, float width, GraphicsUnit unit, out IntPtr pen);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreatePen2(IntPtr brush, float width, GraphicsUnit unit, out IntPtr pen);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipClonePen(IntPtr pen, out IntPtr clonepen);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDeletePen(IntPtr pen);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenBrushFill(IntPtr pen, IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenBrushFill(IntPtr pen, out IntPtr brush);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenFillType(IntPtr pen, out PenType type);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenColor(IntPtr pen, int color);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenColor(IntPtr pen, out int color);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenCompoundArray(IntPtr pen, float[] dash, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenCompoundArray(IntPtr pen, float[] dash, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenCompoundCount(IntPtr pen, out int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenDashCap197819(IntPtr pen, DashCap dashCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenDashCap197819(IntPtr pen, out DashCap dashCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenDashStyle(IntPtr pen, DashStyle dashStyle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenDashStyle(IntPtr pen, out DashStyle dashStyle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenDashOffset(IntPtr pen, float offset);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenDashOffset(IntPtr pen, out float offset);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenDashCount(IntPtr pen, out int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenDashArray(IntPtr pen, float[] dash, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenDashArray(IntPtr pen, float[] dash, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenMiterLimit(IntPtr pen, float miterLimit);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenMiterLimit(IntPtr pen, out float miterLimit);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenLineJoin(IntPtr pen, LineJoin lineJoin);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenLineJoin(IntPtr pen, out LineJoin lineJoin);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenLineCap197819(IntPtr pen, LineCap startCap, LineCap endCap, DashCap dashCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenMode(IntPtr pen, PenAlignment alignment);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenMode(IntPtr pen, out PenAlignment alignment);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenStartCap(IntPtr pen, LineCap startCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenStartCap(IntPtr pen, out LineCap startCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenEndCap(IntPtr pen, LineCap endCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenEndCap(IntPtr pen, out LineCap endCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenCustomStartCap(IntPtr pen, IntPtr customCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenCustomStartCap(IntPtr pen, out IntPtr customCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenCustomEndCap(IntPtr pen, IntPtr customCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenCustomEndCap(IntPtr pen, out IntPtr customCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenTransform(IntPtr pen, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenTransform(IntPtr pen, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPenWidth(IntPtr pen, float width);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPenWidth(IntPtr pen, out float width);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipResetPenTransform(IntPtr pen);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipMultiplyPenTransform(IntPtr pen, IntPtr matrix, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRotatePenTransform(IntPtr pen, float angle, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipScalePenTransform(IntPtr pen, float sx, float sy, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTranslatePenTransform(IntPtr pen, float dx, float dy, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateCustomLineCap(IntPtr fillPath, IntPtr strokePath, LineCap baseCap, float baseInset, out IntPtr customCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDeleteCustomLineCap(IntPtr customCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCloneCustomLineCap(IntPtr customCap, out IntPtr clonedCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetCustomLineCapStrokeCaps(IntPtr customCap, LineCap startCap, LineCap endCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetCustomLineCapStrokeCaps(IntPtr customCap, out LineCap startCap, out LineCap endCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetCustomLineCapStrokeJoin(IntPtr customCap, LineJoin lineJoin);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetCustomLineCapStrokeJoin(IntPtr customCap, out LineJoin lineJoin);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetCustomLineCapBaseCap(IntPtr customCap, LineCap baseCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetCustomLineCapBaseCap(IntPtr customCap, out LineCap baseCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetCustomLineCapBaseInset(IntPtr customCap, float inset);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetCustomLineCapBaseInset(IntPtr customCap, out float inset);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetCustomLineCapWidthScale(IntPtr customCap, float widthScale);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetCustomLineCapWidthScale(IntPtr customCap, out float widthScale);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateAdjustableArrowCap(float height, float width, bool isFilled, out IntPtr arrowCap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetAdjustableArrowCapHeight(IntPtr arrowCap, float height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetAdjustableArrowCapHeight(IntPtr arrowCap, out float height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetAdjustableArrowCapWidth(IntPtr arrowCap, float width);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetAdjustableArrowCapWidth(IntPtr arrowCap, out float width);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetAdjustableArrowCapMiddleInset(IntPtr arrowCap, float middleInset);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetAdjustableArrowCapMiddleInset(IntPtr arrowCap, out float middleInset);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetAdjustableArrowCapFillState(IntPtr arrowCap, bool isFilled);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetAdjustableArrowCapFillState(IntPtr arrowCap, out bool isFilled);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateFromHWND(IntPtr hwnd, out IntPtr graphics);

		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode)]
		internal unsafe static extern Status GdipMeasureString(IntPtr graphics, string str, int length, IntPtr font, ref RectangleF layoutRect, IntPtr stringFormat, out RectangleF boundingBox, int* codepointsFitted, int* linesFilled);

		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode)]
		internal static extern Status GdipMeasureCharacterRanges(IntPtr graphics, string str, int length, IntPtr font, ref RectangleF layoutRect, IntPtr stringFormat, int regcount, out IntPtr regions);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetStringFormatMeasurableCharacterRanges(IntPtr native, int cnt, CharacterRange[] range);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetStringFormatMeasurableCharacterRangeCount(IntPtr native, out int cnt);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateBitmapFromScan0(int width, int height, int stride, PixelFormat format, IntPtr scan0, out IntPtr bmp);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateBitmapFromGraphics(int width, int height, IntPtr target, out IntPtr bitmap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipBitmapLockBits(IntPtr bmp, ref Rectangle rc, ImageLockMode flags, PixelFormat format, [In] [Out] BitmapData bmpData);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipBitmapSetResolution(IntPtr bmp, float xdpi, float ydpi);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipBitmapUnlockBits(IntPtr bmp, [In] [Out] BitmapData bmpData);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipBitmapGetPixel(IntPtr bmp, int x, int y, out int argb);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipBitmapSetPixel(IntPtr bmp, int x, int y, int argb);

		[DllImport("gdiplus.dll", CharSet = CharSet.Auto)]
		internal static extern Status GdipLoadImageFromFile([MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr image);

		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern Status GdipLoadImageFromStream([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = System.Drawing.ComIStreamMarshaler)] IStream stream, out IntPtr image);

		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern Status GdipSaveImageToStream(HandleRef image, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = System.Drawing.ComIStreamMarshaler)] IStream stream, [In] ref Guid clsidEncoder, HandleRef encoderParams);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCloneImage(IntPtr image, out IntPtr imageclone);

		[DllImport("gdiplus.dll", CharSet = CharSet.Auto)]
		internal static extern Status GdipLoadImageFromFileICM([MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr image);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateBitmapFromHBITMAP(IntPtr hBitMap, IntPtr gdiPalette, out IntPtr image);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDisposeImage(IntPtr image);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageFlags(IntPtr image, out int flag);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageType(IntPtr image, out ImageType type);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipImageGetFrameDimensionsCount(IntPtr image, out uint count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipImageGetFrameDimensionsList(IntPtr image, [Out] Guid[] dimensionIDs, uint count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageHeight(IntPtr image, out uint height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageHorizontalResolution(IntPtr image, out float resolution);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImagePaletteSize(IntPtr image, out int size);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImagePalette(IntPtr image, IntPtr palette, int size);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetImagePalette(IntPtr image, IntPtr palette);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageDimension(IntPtr image, out float width, out float height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImagePixelFormat(IntPtr image, out PixelFormat format);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPropertyCount(IntPtr image, out uint propNumbers);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPropertyIdList(IntPtr image, uint propNumbers, [Out] int[] list);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPropertySize(IntPtr image, out int bufferSize, out int propNumbers);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetAllPropertyItems(IntPtr image, int bufferSize, int propNumbers, IntPtr items);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageRawFormat(IntPtr image, out Guid format);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageVerticalResolution(IntPtr image, out float resolution);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageWidth(IntPtr image, out uint width);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageBounds(IntPtr image, out RectangleF source, ref GraphicsUnit unit);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetEncoderParameterListSize(IntPtr image, ref Guid encoder, out uint size);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetEncoderParameterList(IntPtr image, ref Guid encoder, uint size, IntPtr buffer);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipImageGetFrameCount(IntPtr image, ref Guid guidDimension, out uint count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipImageSelectActiveFrame(IntPtr image, ref Guid guidDimension, int frameIndex);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPropertyItemSize(IntPtr image, int propertyID, out int propertySize);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPropertyItem(IntPtr image, int propertyID, int propertySize, IntPtr buffer);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRemovePropertyItem(IntPtr image, int propertyId);

		[DllImport("gdiplus.dll")]
		internal unsafe static extern Status GdipSetPropertyItem(IntPtr image, GdipPropertyItem* propertyItem);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageThumbnail(IntPtr image, uint width, uint height, out IntPtr thumbImage, IntPtr callback, IntPtr callBackData);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipImageRotateFlip(IntPtr image, RotateFlipType rotateFlipType);

		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode)]
		internal static extern Status GdipSaveImageToFile(IntPtr image, string filename, ref Guid encoderClsID, IntPtr encoderParameters);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSaveAdd(IntPtr image, IntPtr encoderParameters);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSaveAddImage(IntPtr image, IntPtr imagenew, IntPtr encoderParameters);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawImageI(IntPtr graphics, IntPtr image, int x, int y);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageGraphicsContext(IntPtr image, out IntPtr graphics);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawImage(IntPtr graphics, IntPtr image, float x, float y);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipBeginContainer(IntPtr graphics, ref RectangleF dstrect, ref RectangleF srcrect, GraphicsUnit unit, out uint state);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipBeginContainerI(IntPtr graphics, ref Rectangle dstrect, ref Rectangle srcrect, GraphicsUnit unit, out uint state);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipBeginContainer2(IntPtr graphics, out uint state);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawImagePoints(IntPtr graphics, IntPtr image, PointF[] destPoints, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawImagePointsI(IntPtr graphics, IntPtr image, Point[] destPoints, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawImageRectRectI(IntPtr graphics, IntPtr image, int dstx, int dsty, int dstwidth, int dstheight, int srcx, int srcy, int srcwidth, int srcheight, GraphicsUnit srcUnit, IntPtr imageattr, Graphics.DrawImageAbort callback, IntPtr callbackData);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawImageRectRect(IntPtr graphics, IntPtr image, float dstx, float dsty, float dstwidth, float dstheight, float srcx, float srcy, float srcwidth, float srcheight, GraphicsUnit srcUnit, IntPtr imageattr, Graphics.DrawImageAbort callback, IntPtr callbackData);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawImagePointsRectI(IntPtr graphics, IntPtr image, Point[] destPoints, int count, int srcx, int srcy, int srcwidth, int srcheight, GraphicsUnit srcUnit, IntPtr imageattr, Graphics.DrawImageAbort callback, IntPtr callbackData);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawImagePointsRect(IntPtr graphics, IntPtr image, PointF[] destPoints, int count, float srcx, float srcy, float srcwidth, float srcheight, GraphicsUnit srcUnit, IntPtr imageattr, Graphics.DrawImageAbort callback, IntPtr callbackData);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawImageRect(IntPtr graphics, IntPtr image, float x, float y, float width, float height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawImagePointRect(IntPtr graphics, IntPtr image, float x, float y, float srcx, float srcy, float srcwidth, float srcheight, GraphicsUnit srcUnit);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDrawImagePointRectI(IntPtr graphics, IntPtr image, int x, int y, int srcx, int srcy, int srcwidth, int srcheight, GraphicsUnit srcUnit);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateStringFormat(StringFormatFlags formatAttributes, int language, out IntPtr native);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateHBITMAPFromBitmap(IntPtr bmp, out IntPtr HandleBmp, int clrbackground);

		[DllImport("gdiplus.dll", CharSet = CharSet.Auto)]
		internal static extern Status GdipCreateBitmapFromFile([MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr bitmap);

		[DllImport("gdiplus.dll", CharSet = CharSet.Auto)]
		internal static extern Status GdipCreateBitmapFromFileICM([MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr bitmap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateHICONFromBitmap(IntPtr bmp, out IntPtr HandleIcon);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateBitmapFromHICON(IntPtr hicon, out IntPtr bitmap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateBitmapFromResource(IntPtr hInstance, string lpBitmapName, out IntPtr bitmap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateMatrix(out IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateMatrix2(float m11, float m12, float m21, float m22, float dx, float dy, out IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateMatrix3(ref RectangleF rect, PointF[] dstplg, out IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateMatrix3I(ref Rectangle rect, Point[] dstplg, out IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDeleteMatrix(IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCloneMatrix(IntPtr matrix, out IntPtr cloneMatrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetMatrixElements(IntPtr matrix, float m11, float m12, float m21, float m22, float dx, float dy);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetMatrixElements(IntPtr matrix, IntPtr matrixOut);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipMultiplyMatrix(IntPtr matrix, IntPtr matrix2, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTranslateMatrix(IntPtr matrix, float offsetX, float offsetY, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipScaleMatrix(IntPtr matrix, float scaleX, float scaleY, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRotateMatrix(IntPtr matrix, float angle, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipShearMatrix(IntPtr matrix, float shearX, float shearY, MatrixOrder order);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipInvertMatrix(IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTransformMatrixPoints(IntPtr matrix, PointF[] pts, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTransformMatrixPointsI(IntPtr matrix, Point[] pts, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipVectorTransformMatrixPoints(IntPtr matrix, PointF[] pts, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipVectorTransformMatrixPointsI(IntPtr matrix, Point[] pts, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsMatrixInvertible(IntPtr matrix, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsMatrixIdentity(IntPtr matrix, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsMatrixEqual(IntPtr matrix, IntPtr matrix2, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreatePath(FillMode brushMode, out IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreatePath2(PointF[] points, byte[] types, int count, FillMode brushMode, out IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreatePath2I(Point[] points, byte[] types, int count, FillMode brushMode, out IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipClonePath(IntPtr path, out IntPtr clonePath);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDeletePath(IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipResetPath(IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPointCount(IntPtr path, out int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathTypes(IntPtr path, [Out] byte[] types, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathPoints(IntPtr path, [Out] PointF[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathPointsI(IntPtr path, [Out] Point[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathFillMode(IntPtr path, out FillMode fillMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPathFillMode(IntPtr path, FillMode fillMode);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipStartPathFigure(IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipClosePathFigure(IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipClosePathFigures(IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetPathMarker(IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipClearPathMarkers(IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipReversePath(IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathLastPoint(IntPtr path, out PointF lastPoint);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathLine(IntPtr path, float x1, float y1, float x2, float y2);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathLine2(IntPtr path, PointF[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathLine2I(IntPtr path, Point[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathArc(IntPtr path, float x, float y, float width, float height, float startAngle, float sweepAngle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathBezier(IntPtr path, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathBeziers(IntPtr path, PointF[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathCurve(IntPtr path, PointF[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathCurveI(IntPtr path, Point[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathCurve2(IntPtr path, PointF[] points, int count, float tension);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathCurve2I(IntPtr path, Point[] points, int count, float tension);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathCurve3(IntPtr path, PointF[] points, int count, int offset, int numberOfSegments, float tension);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathCurve3I(IntPtr path, Point[] points, int count, int offset, int numberOfSegments, float tension);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathClosedCurve(IntPtr path, PointF[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathClosedCurveI(IntPtr path, Point[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathClosedCurve2(IntPtr path, PointF[] points, int count, float tension);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathClosedCurve2I(IntPtr path, Point[] points, int count, float tension);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathRectangle(IntPtr path, float x, float y, float width, float height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathRectangles(IntPtr path, RectangleF[] rects, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathEllipse(IntPtr path, float x, float y, float width, float height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathEllipseI(IntPtr path, int x, int y, int width, int height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathPie(IntPtr path, float x, float y, float width, float height, float startAngle, float sweepAngle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathPieI(IntPtr path, int x, int y, int width, int height, float startAngle, float sweepAngle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathPolygon(IntPtr path, PointF[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathPath(IntPtr path, IntPtr addingPath, bool connect);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathLineI(IntPtr path, int x1, int y1, int x2, int y2);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathArcI(IntPtr path, int x, int y, int width, int height, float startAngle, float sweepAngle);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathBezierI(IntPtr path, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathBeziersI(IntPtr path, Point[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathPolygonI(IntPtr path, Point[] points, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathRectangleI(IntPtr path, int x, int y, int width, int height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipAddPathRectanglesI(IntPtr path, Rectangle[] rects, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipFlattenPath(IntPtr path, IntPtr matrix, float floatness);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipTransformPath(IntPtr path, IntPtr matrix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipWarpPath(IntPtr path, IntPtr matrix, PointF[] points, int count, float srcx, float srcy, float srcwidth, float srcheight, WarpMode mode, float flatness);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipWidenPath(IntPtr path, IntPtr pen, IntPtr matrix, float flatness);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathWorldBounds(IntPtr path, out RectangleF bounds, IntPtr matrix, IntPtr pen);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetPathWorldBoundsI(IntPtr path, out Rectangle bounds, IntPtr matrix, IntPtr pen);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsVisiblePathPoint(IntPtr path, float x, float y, IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsVisiblePathPointI(IntPtr path, int x, int y, IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsOutlineVisiblePathPoint(IntPtr path, float x, float y, IntPtr pen, IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsOutlineVisiblePathPointI(IntPtr path, int x, int y, IntPtr pen, IntPtr graphics, out bool result);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreatePathIter(out IntPtr iterator, IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPathIterGetCount(IntPtr iterator, out int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPathIterGetSubpathCount(IntPtr iterator, out int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDeletePathIter(IntPtr iterator);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPathIterCopyData(IntPtr iterator, out int resultCount, PointF[] points, byte[] types, int startIndex, int endIndex);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPathIterEnumerate(IntPtr iterator, out int resultCount, PointF[] points, byte[] types, int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPathIterHasCurve(IntPtr iterator, out bool curve);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPathIterNextMarkerPath(IntPtr iterator, out int resultCount, IntPtr path);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPathIterNextMarker(IntPtr iterator, out int resultCount, out int startIndex, out int endIndex);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPathIterNextPathType(IntPtr iterator, out int resultCount, out byte pathType, out int startIndex, out int endIndex);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPathIterNextSubpathPath(IntPtr iterator, out int resultCount, IntPtr path, out bool isClosed);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPathIterNextSubpath(IntPtr iterator, out int resultCount, out int startIndex, out int endIndex, out bool isClosed);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPathIterRewind(IntPtr iterator);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateImageAttributes(out IntPtr imageattr);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetImageAttributesColorKeys(IntPtr imageattr, ColorAdjustType type, bool enableFlag, int colorLow, int colorHigh);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDisposeImageAttributes(IntPtr imageattr);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetImageAttributesColorMatrix(IntPtr imageattr, ColorAdjustType type, bool enableFlag, IntPtr colorMatrix, IntPtr grayMatrix, ColorMatrixFlag flags);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetImageAttributesGamma(IntPtr imageattr, ColorAdjustType type, bool enableFlag, float gamma);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetImageAttributesNoOp(IntPtr imageattr, ColorAdjustType type, bool enableFlag);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetImageAttributesOutputChannel(IntPtr imageattr, ColorAdjustType type, bool enableFlag, ColorChannelFlag channelFlags);

		[DllImport("gdiplus.dll", CharSet = CharSet.Auto)]
		internal static extern Status GdipSetImageAttributesOutputChannelColorProfile(IntPtr imageattr, ColorAdjustType type, bool enableFlag, [MarshalAs(UnmanagedType.LPWStr)] string profileName);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetImageAttributesRemapTable(IntPtr imageattr, ColorAdjustType type, bool enableFlag, uint mapSize, IntPtr colorMap);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetImageAttributesThreshold(IntPtr imageattr, ColorAdjustType type, bool enableFlag, float thresHold);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCloneImageAttributes(IntPtr imageattr, out IntPtr cloneImageattr);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageAttributesAdjustedPalette(IntPtr imageattr, IntPtr colorPalette, ColorAdjustType colorAdjustType);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetImageAttributesWrapMode(IntPtr imageattr, WrapMode wrap, int argb, bool clamp);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateFont(IntPtr fontFamily, float emSize, FontStyle style, GraphicsUnit unit, out IntPtr font);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDeleteFont(IntPtr font);

		[DllImport("gdiplus.dll", CharSet = CharSet.Auto)]
		internal static extern Status GdipGetLogFont(IntPtr font, IntPtr graphics, [MarshalAs(UnmanagedType.AsAny)] [Out] object logfontA);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateFontFromDC(IntPtr hdc, out IntPtr font);

		[DllImport("gdiplus.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern Status GdipCreateFontFromLogfont(IntPtr hdc, ref LOGFONT lf, out IntPtr ptr);

		[DllImport("gdiplus.dll", CharSet = CharSet.Ansi)]
		internal static extern Status GdipCreateFontFromHfont(IntPtr hdc, out IntPtr font, ref LOGFONT lf);

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
		internal static extern IntPtr CreateFontIndirect(ref LOGFONT logfont);

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		internal static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetIconInfo(IntPtr hIcon, out IconInfo iconinfo);

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		internal static extern IntPtr CreateIconIndirect([In] ref IconInfo piconinfo);

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		internal static extern bool DestroyIcon(IntPtr hIcon);

		[DllImport("gdi32.dll")]
		internal static extern bool DeleteObject(IntPtr hObject);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetDesktopWindow();

		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetSysColor")]
		public static extern uint Win32GetSysColor(GetSysColorIndex index);

		[DllImport("libX11")]
		internal static extern IntPtr XOpenDisplay(IntPtr display);

		[DllImport("libX11")]
		internal static extern int XCloseDisplay(IntPtr display);

		[DllImport("libX11")]
		internal static extern IntPtr XRootWindow(IntPtr display, int screen);

		[DllImport("libX11")]
		internal static extern int XDefaultScreen(IntPtr display);

		[DllImport("libX11")]
		internal static extern uint XDefaultDepth(IntPtr display, int screen);

		[DllImport("libX11")]
		internal static extern IntPtr XGetImage(IntPtr display, IntPtr drawable, int src_x, int src_y, int width, int height, int pane, int format);

		[DllImport("libX11")]
		internal static extern int XGetPixel(IntPtr image, int x, int y);

		[DllImport("libX11")]
		internal static extern int XDestroyImage(IntPtr image);

		[DllImport("libX11")]
		internal static extern IntPtr XDefaultVisual(IntPtr display, int screen);

		[DllImport("libX11")]
		internal static extern IntPtr XGetVisualInfo(IntPtr display, int vinfo_mask, ref XVisualInfo vinfo_template, ref int nitems);

		[DllImport("libX11")]
		internal static extern IntPtr XVisualIDFromVisual(IntPtr visual);

		[DllImport("libX11")]
		internal static extern void XFree(IntPtr data);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetFontCollectionFamilyCount(IntPtr collection, out int found);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetFontCollectionFamilyList(IntPtr collection, int getCount, IntPtr[] dest, out int retCount);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipNewInstalledFontCollection(out IntPtr collection);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipNewPrivateFontCollection(out IntPtr collection);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDeletePrivateFontCollection(ref IntPtr collection);

		[DllImport("gdiplus.dll", CharSet = CharSet.Auto)]
		internal static extern Status GdipPrivateAddFontFile(IntPtr collection, [MarshalAs(UnmanagedType.LPWStr)] string fileName);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPrivateAddMemoryFont(IntPtr collection, IntPtr mem, int length);

		[DllImport("gdiplus.dll", CharSet = CharSet.Auto)]
		internal static extern Status GdipCreateFontFamilyFromName([MarshalAs(UnmanagedType.LPWStr)] string fName, IntPtr collection, out IntPtr fontFamily);

		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode)]
		internal static extern Status GdipGetFamilyName(IntPtr family, StringBuilder name, int language);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetGenericFontFamilySansSerif(out IntPtr fontFamily);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetGenericFontFamilySerif(out IntPtr fontFamily);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetGenericFontFamilyMonospace(out IntPtr fontFamily);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetCellAscent(IntPtr fontFamily, int style, out short ascent);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetCellDescent(IntPtr fontFamily, int style, out short descent);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetLineSpacing(IntPtr fontFamily, int style, out short spacing);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetEmHeight(IntPtr fontFamily, int style, out short emHeight);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipIsStyleAvailable(IntPtr fontFamily, int style, out bool styleAvailable);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDeleteFontFamily(IntPtr fontFamily);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetFontSize(IntPtr font, out float size);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetFontHeight(IntPtr font, IntPtr graphics, out float height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetFontHeightGivenDPI(IntPtr font, float dpi, out float height);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCloneFontFamily(IntPtr fontFamily, out IntPtr clone);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateStringFormat(int formatAttributes, int language, out IntPtr format);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipStringFormatGetGenericDefault(out IntPtr format);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipStringFormatGetGenericTypographic(out IntPtr format);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipDeleteStringFormat(IntPtr format);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCloneStringFormat(IntPtr srcformat, out IntPtr format);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetStringFormatFlags(IntPtr format, StringFormatFlags flags);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetStringFormatFlags(IntPtr format, out StringFormatFlags flags);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetStringFormatAlign(IntPtr format, StringAlignment align);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetStringFormatAlign(IntPtr format, out StringAlignment align);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetStringFormatLineAlign(IntPtr format, StringAlignment align);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetStringFormatLineAlign(IntPtr format, out StringAlignment align);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetStringFormatTrimming(IntPtr format, StringTrimming trimming);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetStringFormatTrimming(IntPtr format, out StringTrimming trimming);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetStringFormatHotkeyPrefix(IntPtr format, HotkeyPrefix hotkeyPrefix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetStringFormatHotkeyPrefix(IntPtr format, out HotkeyPrefix hotkeyPrefix);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetStringFormatTabStops(IntPtr format, float firstTabOffset, int count, float[] tabStops);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetStringFormatDigitSubstitution(IntPtr format, int language, out StringDigitSubstitute substitute);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetStringFormatDigitSubstitution(IntPtr format, int language, StringDigitSubstitute substitute);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetStringFormatTabStopCount(IntPtr format, out int count);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetStringFormatTabStops(IntPtr format, int count, out float firstTabOffset, [In] [Out] float[] tabStops);

		[DllImport("gdiplus.dll", CharSet = CharSet.Auto)]
		internal static extern Status GdipCreateMetafileFromFile([MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr metafile);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateMetafileFromEmf(IntPtr hEmf, bool deleteEmf, out IntPtr metafile);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateMetafileFromWmf(IntPtr hWmf, bool deleteWmf, WmfPlaceableFileHeader wmfPlaceableFileHeader, out IntPtr metafile);

		[DllImport("gdiplus.dll", CharSet = CharSet.Auto)]
		internal static extern Status GdipGetMetafileHeaderFromFile([MarshalAs(UnmanagedType.LPWStr)] string filename, IntPtr header);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetMetafileHeaderFromMetafile(IntPtr metafile, IntPtr header);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetMetafileHeaderFromEmf(IntPtr hEmf, IntPtr header);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetMetafileHeaderFromWmf(IntPtr hWmf, IntPtr wmfPlaceableFileHeader, IntPtr header);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetHemfFromMetafile(IntPtr metafile, out IntPtr hEmf);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetMetafileDownLevelRasterizationLimit(IntPtr metafile, ref uint metafileRasterizationLimitDpi);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetMetafileDownLevelRasterizationLimit(IntPtr metafile, uint metafileRasterizationLimitDpi);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipPlayMetafileRecord(IntPtr metafile, EmfPlusRecordType recordType, int flags, int dataSize, byte[] data);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRecordMetafile(IntPtr hdc, EmfType type, ref RectangleF frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)] string description, out IntPtr metafile);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRecordMetafileI(IntPtr hdc, EmfType type, ref Rectangle frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)] string description, out IntPtr metafile);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRecordMetafileFileName([MarshalAs(UnmanagedType.LPWStr)] string filename, IntPtr hdc, EmfType type, ref RectangleF frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)] string description, out IntPtr metafile);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRecordMetafileFileNameI([MarshalAs(UnmanagedType.LPWStr)] string filename, IntPtr hdc, EmfType type, ref Rectangle frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)] string description, out IntPtr metafile);

		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern Status GdipCreateMetafileFromStream([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = System.Drawing.ComIStreamMarshaler)] IStream stream, out IntPtr metafile);

		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern Status GdipGetMetafileHeaderFromStream([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = System.Drawing.ComIStreamMarshaler)] IStream stream, IntPtr header);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRecordMetafileStream([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = System.Drawing.ComIStreamMarshaler)] IStream stream, IntPtr hdc, EmfType type, ref RectangleF frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)] string description, out IntPtr metafile);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRecordMetafileStreamI([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = System.Drawing.ComIStreamMarshaler)] IStream stream, IntPtr hdc, EmfType type, ref Rectangle frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)] string description, out IntPtr metafile);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageDecodersSize(out int decoderNums, out int arraySize);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageDecoders(int decoderNums, int arraySize, IntPtr decoders);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageEncodersSize(out int encoderNums, out int arraySize);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetImageEncoders(int encoderNums, int arraySize, IntPtr encoders);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateFromContext_macosx(IntPtr cgref, int width, int height, out IntPtr graphics);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSetVisibleClip_linux(IntPtr graphics, ref Rectangle rect);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateFromXDrawable_linux(IntPtr drawable, IntPtr display, out IntPtr graphics);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipLoadImageFromDelegate_linux(GDIPlus.StreamGetHeaderDelegate getHeader, GDIPlus.StreamGetBytesDelegate getBytes, GDIPlus.StreamPutBytesDelegate putBytes, GDIPlus.StreamSeekDelegate doSeek, GDIPlus.StreamCloseDelegate close, GDIPlus.StreamSizeDelegate size, out IntPtr image);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipSaveImageToDelegate_linux(IntPtr image, GDIPlus.StreamGetBytesDelegate getBytes, GDIPlus.StreamPutBytesDelegate putBytes, GDIPlus.StreamSeekDelegate doSeek, GDIPlus.StreamCloseDelegate close, GDIPlus.StreamSizeDelegate size, ref Guid encoderClsID, IntPtr encoderParameters);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipCreateMetafileFromDelegate_linux(GDIPlus.StreamGetHeaderDelegate getHeader, GDIPlus.StreamGetBytesDelegate getBytes, GDIPlus.StreamPutBytesDelegate putBytes, GDIPlus.StreamSeekDelegate doSeek, GDIPlus.StreamCloseDelegate close, GDIPlus.StreamSizeDelegate size, out IntPtr metafile);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipGetMetafileHeaderFromDelegate_linux(GDIPlus.StreamGetHeaderDelegate getHeader, GDIPlus.StreamGetBytesDelegate getBytes, GDIPlus.StreamPutBytesDelegate putBytes, GDIPlus.StreamSeekDelegate doSeek, GDIPlus.StreamCloseDelegate close, GDIPlus.StreamSizeDelegate size, IntPtr header);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRecordMetafileFromDelegate_linux(GDIPlus.StreamGetHeaderDelegate getHeader, GDIPlus.StreamGetBytesDelegate getBytes, GDIPlus.StreamPutBytesDelegate putBytes, GDIPlus.StreamSeekDelegate doSeek, GDIPlus.StreamCloseDelegate close, GDIPlus.StreamSizeDelegate size, IntPtr hdc, EmfType type, ref RectangleF frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)] string description, out IntPtr metafile);

		[DllImport("gdiplus.dll")]
		internal static extern Status GdipRecordMetafileFromDelegateI_linux(GDIPlus.StreamGetHeaderDelegate getHeader, GDIPlus.StreamGetBytesDelegate getBytes, GDIPlus.StreamPutBytesDelegate putBytes, GDIPlus.StreamSeekDelegate doSeek, GDIPlus.StreamCloseDelegate close, GDIPlus.StreamSizeDelegate size, IntPtr hdc, EmfType type, ref Rectangle frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)] string description, out IntPtr metafile);

		[DllImport("libc")]
		private static extern int uname(IntPtr buf);

		public const int FACESIZE = 32;

		public const int LANG_NEUTRAL = 0;

		public static IntPtr Display = IntPtr.Zero;

		public static bool UseX11Drawable = false;

		public static bool UseCarbonDrawable = false;

		internal static ulong GdiPlusToken = 0UL;

		internal sealed class GdiPlusStreamHelper
		{
			public GdiPlusStreamHelper(Stream s, bool seekToOrigin)
			{
				this.managedBuf = new byte[4096];
				this.stream = s;
				if (this.stream != null && this.stream.CanSeek && seekToOrigin)
				{
					this.stream.Seek(0L, SeekOrigin.Begin);
				}
			}

			public int StreamGetHeaderImpl(IntPtr buf, int bufsz)
			{
				this.start_buf = new byte[bufsz];
				int num;
				try
				{
					num = this.stream.Read(this.start_buf, 0, bufsz);
				}
				catch (IOException)
				{
					return -1;
				}
				if (num > 0 && buf != IntPtr.Zero)
				{
					Marshal.Copy(this.start_buf, 0, (IntPtr)buf.ToInt64(), num);
				}
				this.start_buf_pos = 0;
				this.start_buf_len = num;
				return num;
			}

			public GDIPlus.StreamGetHeaderDelegate GetHeaderDelegate
			{
				get
				{
					if (this.stream != null && this.stream.CanRead)
					{
						if (this.sghd == null)
						{
							this.sghd = new GDIPlus.StreamGetHeaderDelegate(this.StreamGetHeaderImpl);
						}
						return this.sghd;
					}
					return null;
				}
			}

			public int StreamGetBytesImpl(IntPtr buf, int bufsz, bool peek)
			{
				if (buf == IntPtr.Zero && peek)
				{
					return -1;
				}
				if (bufsz > this.managedBuf.Length)
				{
					this.managedBuf = new byte[bufsz];
				}
				int num = 0;
				long num2 = 0L;
				if (bufsz > 0)
				{
					if (this.stream.CanSeek)
					{
						num2 = this.stream.Position;
					}
					if (this.start_buf_len > 0)
					{
						if (this.start_buf_len > bufsz)
						{
							Array.Copy(this.start_buf, this.start_buf_pos, this.managedBuf, 0, bufsz);
							this.start_buf_pos += bufsz;
							this.start_buf_len -= bufsz;
							num = bufsz;
							bufsz = 0;
						}
						else
						{
							Array.Copy(this.start_buf, this.start_buf_pos, this.managedBuf, 0, this.start_buf_len);
							bufsz -= this.start_buf_len;
							num = this.start_buf_len;
							this.start_buf_len = 0;
						}
					}
					if (bufsz > 0)
					{
						try
						{
							num += this.stream.Read(this.managedBuf, num, bufsz);
						}
						catch (IOException)
						{
							return -1;
						}
					}
					if (num > 0 && buf != IntPtr.Zero)
					{
						Marshal.Copy(this.managedBuf, 0, (IntPtr)buf.ToInt64(), num);
					}
					if (this.stream.CanSeek || bufsz != 10 || peek)
					{
					}
					if (peek)
					{
						if (!this.stream.CanSeek)
						{
							throw new NotSupportedException();
						}
						this.stream.Seek(num2, SeekOrigin.Begin);
					}
				}
				return num;
			}

			public GDIPlus.StreamGetBytesDelegate GetBytesDelegate
			{
				get
				{
					if (this.stream != null && this.stream.CanRead)
					{
						if (this.sgbd == null)
						{
							this.sgbd = new GDIPlus.StreamGetBytesDelegate(this.StreamGetBytesImpl);
						}
						return this.sgbd;
					}
					return null;
				}
			}

			public long StreamSeekImpl(int offset, int whence)
			{
				if (whence < 0 || whence > 2)
				{
					return -1L;
				}
				this.start_buf_pos += this.start_buf_len;
				this.start_buf_len = 0;
				SeekOrigin seekOrigin;
				switch (whence)
				{
				case 0:
					seekOrigin = SeekOrigin.Begin;
					break;
				case 1:
					seekOrigin = SeekOrigin.Current;
					break;
				case 2:
					seekOrigin = SeekOrigin.End;
					break;
				default:
					return -1L;
				}
				return this.stream.Seek((long)offset, seekOrigin);
			}

			public GDIPlus.StreamSeekDelegate SeekDelegate
			{
				get
				{
					if (this.stream != null && this.stream.CanSeek)
					{
						if (this.skd == null)
						{
							this.skd = new GDIPlus.StreamSeekDelegate(this.StreamSeekImpl);
						}
						return this.skd;
					}
					return null;
				}
			}

			public int StreamPutBytesImpl(IntPtr buf, int bufsz)
			{
				if (bufsz > this.managedBuf.Length)
				{
					this.managedBuf = new byte[bufsz];
				}
				Marshal.Copy(buf, this.managedBuf, 0, bufsz);
				this.stream.Write(this.managedBuf, 0, bufsz);
				return bufsz;
			}

			public GDIPlus.StreamPutBytesDelegate PutBytesDelegate
			{
				get
				{
					if (this.stream != null && this.stream.CanWrite)
					{
						if (this.spbd == null)
						{
							this.spbd = new GDIPlus.StreamPutBytesDelegate(this.StreamPutBytesImpl);
						}
						return this.spbd;
					}
					return null;
				}
			}

			public void StreamCloseImpl()
			{
				this.stream.Close();
			}

			public GDIPlus.StreamCloseDelegate CloseDelegate
			{
				get
				{
					if (this.stream != null)
					{
						if (this.scd == null)
						{
							this.scd = new GDIPlus.StreamCloseDelegate(this.StreamCloseImpl);
						}
						return this.scd;
					}
					return null;
				}
			}

			public long StreamSizeImpl()
			{
				long num;
				try
				{
					num = this.stream.Length;
				}
				catch
				{
					num = -1L;
				}
				return num;
			}

			public GDIPlus.StreamSizeDelegate SizeDelegate
			{
				get
				{
					if (this.stream != null)
					{
						if (this.ssd == null)
						{
							this.ssd = new GDIPlus.StreamSizeDelegate(this.StreamSizeImpl);
						}
						return this.ssd;
					}
					return null;
				}
			}

			private const int default_bufsize = 4096;

			public Stream stream;

			private GDIPlus.StreamGetHeaderDelegate sghd;

			private GDIPlus.StreamGetBytesDelegate sgbd;

			private GDIPlus.StreamSeekDelegate skd;

			private GDIPlus.StreamPutBytesDelegate spbd;

			private GDIPlus.StreamCloseDelegate scd;

			private GDIPlus.StreamSizeDelegate ssd;

			private byte[] start_buf;

			private int start_buf_pos;

			private int start_buf_len;

			private byte[] managedBuf;
		}

		public delegate int StreamGetHeaderDelegate(IntPtr buf, int bufsz);

		public delegate int StreamGetBytesDelegate(IntPtr buf, int bufsz, bool peek);

		public delegate long StreamSeekDelegate(int offset, int whence);

		public delegate int StreamPutBytesDelegate(IntPtr buf, int bufsz);

		public delegate void StreamCloseDelegate();

		public delegate long StreamSizeDelegate();
	}
}
