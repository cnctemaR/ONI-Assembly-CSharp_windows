using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D
{
	public sealed class HatchBrush : Brush
	{
		public HatchBrush(HatchStyle hatchstyle, Color foreColor)
			: this(hatchstyle, foreColor, Color.FromArgb(-16777216))
		{
		}

		public HatchBrush(HatchStyle hatchstyle, Color foreColor, Color backColor)
		{
			if (hatchstyle < HatchStyle.Horizontal || hatchstyle > HatchStyle.SolidDiamond)
			{
				throw new ArgumentException(SR.Format("The value of argument '{0}' ({1}) is invalid for Enum type '{2}'.", new object[] { "hatchstyle", hatchstyle, "HatchStyle" }), "hatchstyle");
			}
			IntPtr intPtr;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateHatchBrush((int)hatchstyle, foreColor.ToArgb(), backColor.ToArgb(), out intPtr));
			base.SetNativeBrushInternal(intPtr);
		}

		internal HatchBrush(IntPtr nativeBrush)
		{
			base.SetNativeBrushInternal(nativeBrush);
		}

		public override object Clone()
		{
			IntPtr zero = IntPtr.Zero;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCloneBrush(new HandleRef(this, base.NativeBrush), out zero));
			return new HatchBrush(zero);
		}

		public HatchStyle HatchStyle
		{
			get
			{
				int num;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetHatchStyle(new HandleRef(this, base.NativeBrush), out num));
				return (HatchStyle)num;
			}
		}

		public Color ForegroundColor
		{
			get
			{
				int num;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetHatchForegroundColor(new HandleRef(this, base.NativeBrush), out num));
				return Color.FromArgb(num);
			}
		}

		public Color BackgroundColor
		{
			get
			{
				int num;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetHatchBackgroundColor(new HandleRef(this, base.NativeBrush), out num));
				return Color.FromArgb(num);
			}
		}
	}
}
