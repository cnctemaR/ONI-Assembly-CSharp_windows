using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D
{
	public sealed class AdjustableArrowCap : CustomLineCap
	{
		internal AdjustableArrowCap(IntPtr nativeCap)
			: base(nativeCap)
		{
		}

		public AdjustableArrowCap(float width, float height)
			: this(width, height, true)
		{
		}

		public AdjustableArrowCap(float width, float height, bool isFilled)
		{
			IntPtr intPtr;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateAdjustableArrowCap(height, width, isFilled, out intPtr));
			base.SetNativeLineCap(intPtr);
		}

		public float Height
		{
			get
			{
				float num;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetAdjustableArrowCapHeight(new HandleRef(this, this.nativeCap), out num));
				return num;
			}
			set
			{
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetAdjustableArrowCapHeight(new HandleRef(this, this.nativeCap), value));
			}
		}

		public float Width
		{
			get
			{
				float num;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetAdjustableArrowCapWidth(new HandleRef(this, this.nativeCap), out num));
				return num;
			}
			set
			{
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetAdjustableArrowCapWidth(new HandleRef(this, this.nativeCap), value));
			}
		}

		public float MiddleInset
		{
			get
			{
				float num;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetAdjustableArrowCapMiddleInset(new HandleRef(this, this.nativeCap), out num));
				return num;
			}
			set
			{
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetAdjustableArrowCapMiddleInset(new HandleRef(this, this.nativeCap), value));
			}
		}

		public bool Filled
		{
			get
			{
				bool flag;
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetAdjustableArrowCapFillState(new HandleRef(this, this.nativeCap), out flag));
				return flag;
			}
			set
			{
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetAdjustableArrowCapFillState(new HandleRef(this, this.nativeCap), value));
			}
		}
	}
}
