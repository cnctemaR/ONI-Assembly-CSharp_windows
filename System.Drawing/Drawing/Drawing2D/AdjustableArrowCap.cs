using System;

namespace System.Drawing.Drawing2D
{
	public sealed class AdjustableArrowCap : CustomLineCap
	{
		internal AdjustableArrowCap(IntPtr ptr)
			: base(ptr)
		{
		}

		public AdjustableArrowCap(float width, float height)
			: this(width, height, true)
		{
		}

		public AdjustableArrowCap(float width, float height, bool isFilled)
		{
			Status status = GDIPlus.GdipCreateAdjustableArrowCap(height, width, isFilled, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public bool Filled
		{
			get
			{
				bool flag;
				Status status = GDIPlus.GdipGetAdjustableArrowCapFillState(this.nativeObject, out flag);
				GDIPlus.CheckStatus(status);
				return flag;
			}
			set
			{
				Status status = GDIPlus.GdipSetAdjustableArrowCapFillState(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public float Width
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetAdjustableArrowCapWidth(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
			set
			{
				Status status = GDIPlus.GdipSetAdjustableArrowCapWidth(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public float Height
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetAdjustableArrowCapHeight(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
			set
			{
				Status status = GDIPlus.GdipSetAdjustableArrowCapHeight(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public float MiddleInset
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetAdjustableArrowCapMiddleInset(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
			set
			{
				Status status = GDIPlus.GdipSetAdjustableArrowCapMiddleInset(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}
	}
}
