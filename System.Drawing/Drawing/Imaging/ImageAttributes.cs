using System;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ImageAttributes : IDisposable, ICloneable
	{
		internal ImageAttributes(IntPtr native)
		{
			this.nativeImageAttr = native;
		}

		public ImageAttributes()
		{
			Status status = GDIPlus.GdipCreateImageAttributes(out this.nativeImageAttr);
			GDIPlus.CheckStatus(status);
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativeImageAttr;
			}
		}

		public void ClearBrushRemapTable()
		{
			this.ClearRemapTable(ColorAdjustType.Brush);
		}

		public void ClearColorKey()
		{
			this.ClearColorKey(ColorAdjustType.Default);
		}

		public void ClearColorKey(ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesColorKeys(this.nativeImageAttr, type, false, 0, 0);
			GDIPlus.CheckStatus(status);
		}

		public void ClearColorMatrix()
		{
			this.ClearColorMatrix(ColorAdjustType.Default);
		}

		public void ClearColorMatrix(ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesColorMatrix(this.nativeImageAttr, type, false, IntPtr.Zero, IntPtr.Zero, ColorMatrixFlag.Default);
			GDIPlus.CheckStatus(status);
		}

		public void ClearGamma()
		{
			this.ClearGamma(ColorAdjustType.Default);
		}

		public void ClearGamma(ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesGamma(this.nativeImageAttr, type, false, 0f);
			GDIPlus.CheckStatus(status);
		}

		public void ClearNoOp()
		{
			this.ClearNoOp(ColorAdjustType.Default);
		}

		public void ClearNoOp(ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesNoOp(this.nativeImageAttr, type, false);
			GDIPlus.CheckStatus(status);
		}

		public void ClearOutputChannel()
		{
			this.ClearOutputChannel(ColorAdjustType.Default);
		}

		public void ClearOutputChannel(ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesOutputChannel(this.nativeImageAttr, type, false, ColorChannelFlag.ColorChannelLast);
			GDIPlus.CheckStatus(status);
		}

		public void ClearOutputChannelColorProfile()
		{
			this.ClearOutputChannelColorProfile(ColorAdjustType.Default);
		}

		public void ClearOutputChannelColorProfile(ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesOutputChannelColorProfile(this.nativeImageAttr, type, false, null);
			GDIPlus.CheckStatus(status);
		}

		public void ClearRemapTable()
		{
			this.ClearRemapTable(ColorAdjustType.Default);
		}

		public void ClearRemapTable(ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesRemapTable(this.nativeImageAttr, type, false, 0U, IntPtr.Zero);
			GDIPlus.CheckStatus(status);
		}

		public void ClearThreshold()
		{
			this.ClearThreshold(ColorAdjustType.Default);
		}

		public void ClearThreshold(ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesThreshold(this.nativeImageAttr, type, false, 0f);
			GDIPlus.CheckStatus(status);
		}

		public void SetColorKey(Color colorLow, Color colorHigh)
		{
			this.SetColorKey(colorLow, colorHigh, ColorAdjustType.Default);
		}

		public void SetColorMatrix(ColorMatrix colorMatrix)
		{
			this.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
		}

		public void SetColorMatrix(ColorMatrix colorMatrix, ColorMatrixFlag colorMatrixFlag)
		{
			this.SetColorMatrix(colorMatrix, colorMatrixFlag, ColorAdjustType.Default);
		}

		public void SetColorMatrix(ColorMatrix colorMatrix, ColorMatrixFlag colorMatrixFlag, ColorAdjustType colorAdjustType)
		{
			IntPtr intPtr = ColorMatrix.Alloc(colorMatrix);
			try
			{
				Status status = GDIPlus.GdipSetImageAttributesColorMatrix(this.nativeImageAttr, colorAdjustType, true, intPtr, IntPtr.Zero, colorMatrixFlag);
				GDIPlus.CheckStatus(status);
			}
			finally
			{
				ColorMatrix.Free(intPtr);
			}
		}

		public void Dispose()
		{
			if (this.nativeImageAttr != IntPtr.Zero)
			{
				Status status = GDIPlus.GdipDisposeImageAttributes(this.nativeImageAttr);
				this.nativeImageAttr = IntPtr.Zero;
				GDIPlus.CheckStatus(status);
			}
			GC.SuppressFinalize(this);
		}

		~ImageAttributes()
		{
			this.Dispose();
		}

		public object Clone()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCloneImageAttributes(this.nativeImageAttr, out intPtr);
			GDIPlus.CheckStatus(status);
			return new ImageAttributes(intPtr);
		}

		[MonoTODO("Not supported by libgdiplus")]
		public void GetAdjustedPalette(ColorPalette palette, ColorAdjustType type)
		{
			IntPtr gdipalette = palette.getGDIPalette();
			try
			{
				Status status = GDIPlus.GdipGetImageAttributesAdjustedPalette(this.nativeImageAttr, gdipalette, type);
				GDIPlus.CheckStatus(status);
				palette.setFromGDIPalette(gdipalette);
			}
			finally
			{
				if (gdipalette != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(gdipalette);
				}
			}
		}

		public void SetBrushRemapTable(ColorMap[] map)
		{
			GdiColorMap gdiColorMap = default(GdiColorMap);
			int num = Marshal.SizeOf(gdiColorMap);
			int num2 = num * map.Length;
			IntPtr intPtr2;
			IntPtr intPtr = (intPtr2 = Marshal.AllocHGlobal(num2));
			try
			{
				for (int i = 0; i < map.Length; i++)
				{
					gdiColorMap.from = map[i].OldColor.ToArgb();
					gdiColorMap.to = map[i].NewColor.ToArgb();
					Marshal.StructureToPtr(gdiColorMap, intPtr, false);
					intPtr = (IntPtr)(intPtr.ToInt64() + (long)num);
				}
				Status status = GDIPlus.GdipSetImageAttributesRemapTable(this.nativeImageAttr, ColorAdjustType.Brush, true, (uint)map.Length, intPtr2);
				GDIPlus.CheckStatus(status);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr2);
			}
		}

		public void SetColorKey(Color colorLow, Color colorHigh, ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesColorKeys(this.nativeImageAttr, type, true, colorLow.ToArgb(), colorHigh.ToArgb());
			GDIPlus.CheckStatus(status);
		}

		public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix)
		{
			this.SetColorMatrices(newColorMatrix, grayMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
		}

		public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag flags)
		{
			this.SetColorMatrices(newColorMatrix, grayMatrix, flags, ColorAdjustType.Default);
		}

		public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag mode, ColorAdjustType type)
		{
			IntPtr intPtr = ColorMatrix.Alloc(newColorMatrix);
			Status status;
			try
			{
				if (grayMatrix == null)
				{
					status = GDIPlus.GdipSetImageAttributesColorMatrix(this.nativeImageAttr, type, true, intPtr, IntPtr.Zero, mode);
				}
				else
				{
					IntPtr intPtr2 = ColorMatrix.Alloc(grayMatrix);
					try
					{
						status = GDIPlus.GdipSetImageAttributesColorMatrix(this.nativeImageAttr, type, true, intPtr, intPtr2, mode);
					}
					finally
					{
						ColorMatrix.Free(intPtr2);
					}
				}
			}
			finally
			{
				ColorMatrix.Free(intPtr);
			}
			GDIPlus.CheckStatus(status);
		}

		public void SetGamma(float gamma)
		{
			this.SetGamma(gamma, ColorAdjustType.Default);
		}

		public void SetGamma(float gamma, ColorAdjustType coloradjust)
		{
			Status status = GDIPlus.GdipSetImageAttributesGamma(this.nativeImageAttr, coloradjust, true, gamma);
			GDIPlus.CheckStatus(status);
		}

		public void SetNoOp()
		{
			this.SetNoOp(ColorAdjustType.Default);
		}

		public void SetNoOp(ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesNoOp(this.nativeImageAttr, type, true);
			GDIPlus.CheckStatus(status);
		}

		[MonoTODO("Not supported by libgdiplus")]
		public void SetOutputChannel(ColorChannelFlag flags)
		{
			this.SetOutputChannel(flags, ColorAdjustType.Default);
		}

		[MonoTODO("Not supported by libgdiplus")]
		public void SetOutputChannel(ColorChannelFlag flags, ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesOutputChannel(this.nativeImageAttr, type, true, flags);
			GDIPlus.CheckStatus(status);
		}

		[MonoTODO("Not supported by libgdiplus")]
		public void SetOutputChannelColorProfile(string colorProfileFilename)
		{
			this.SetOutputChannelColorProfile(colorProfileFilename, ColorAdjustType.Default);
		}

		[MonoTODO("Not supported by libgdiplus")]
		public void SetOutputChannelColorProfile(string colorProfileFilename, ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesOutputChannelColorProfile(this.nativeImageAttr, type, true, colorProfileFilename);
			GDIPlus.CheckStatus(status);
		}

		public void SetRemapTable(ColorMap[] map)
		{
			this.SetRemapTable(map, ColorAdjustType.Default);
		}

		public void SetRemapTable(ColorMap[] map, ColorAdjustType type)
		{
			GdiColorMap gdiColorMap = default(GdiColorMap);
			int num = Marshal.SizeOf(gdiColorMap);
			int num2 = num * map.Length;
			IntPtr intPtr2;
			IntPtr intPtr = (intPtr2 = Marshal.AllocHGlobal(num2));
			try
			{
				for (int i = 0; i < map.Length; i++)
				{
					gdiColorMap.from = map[i].OldColor.ToArgb();
					gdiColorMap.to = map[i].NewColor.ToArgb();
					Marshal.StructureToPtr(gdiColorMap, intPtr, false);
					intPtr = (IntPtr)(intPtr.ToInt64() + (long)num);
				}
				Status status = GDIPlus.GdipSetImageAttributesRemapTable(this.nativeImageAttr, type, true, (uint)map.Length, intPtr2);
				GDIPlus.CheckStatus(status);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr2);
			}
		}

		[MonoTODO("Not supported by libgdiplus")]
		public void SetThreshold(float threshold)
		{
			this.SetThreshold(threshold, ColorAdjustType.Default);
		}

		[MonoTODO("Not supported by libgdiplus")]
		public void SetThreshold(float threshold, ColorAdjustType type)
		{
			Status status = GDIPlus.GdipSetImageAttributesThreshold(this.nativeImageAttr, type, true, 0f);
			GDIPlus.CheckStatus(status);
		}

		public void SetWrapMode(WrapMode mode)
		{
			this.SetWrapMode(mode, Color.Black);
		}

		public void SetWrapMode(WrapMode mode, Color color)
		{
			this.SetWrapMode(mode, color, false);
		}

		public void SetWrapMode(WrapMode mode, Color color, bool clamp)
		{
			Status status = GDIPlus.GdipSetImageAttributesWrapMode(this.nativeImageAttr, mode, color.ToArgb(), clamp);
			GDIPlus.CheckStatus(status);
		}

		private IntPtr nativeImageAttr;
	}
}
