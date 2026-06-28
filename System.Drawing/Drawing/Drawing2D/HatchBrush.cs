using System;

namespace System.Drawing.Drawing2D
{
	public sealed class HatchBrush : Brush
	{
		internal HatchBrush(IntPtr ptr)
			: base(ptr)
		{
		}

		public HatchBrush(HatchStyle hatchStyle, Color foreColor)
			: this(hatchStyle, foreColor, Color.Black)
		{
		}

		public HatchBrush(HatchStyle hatchStyle, Color foreColor, Color backColor)
		{
			Status status = GDIPlus.GdipCreateHatchBrush(hatchStyle, foreColor.ToArgb(), backColor.ToArgb(), out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public Color BackgroundColor
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetHatchBackgroundColor(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return Color.FromArgb(num);
			}
		}

		public Color ForegroundColor
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetHatchForegroundColor(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return Color.FromArgb(num);
			}
		}

		public HatchStyle HatchStyle
		{
			get
			{
				HatchStyle hatchStyle;
				Status status = GDIPlus.GdipGetHatchStyle(this.nativeObject, out hatchStyle);
				GDIPlus.CheckStatus(status);
				return hatchStyle;
			}
		}

		public override object Clone()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCloneBrush(this.nativeObject, out intPtr);
			GDIPlus.CheckStatus(status);
			return new HatchBrush(intPtr);
		}
	}
}
