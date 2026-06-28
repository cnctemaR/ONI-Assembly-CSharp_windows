using System;

namespace System.Drawing
{
	public sealed class SolidBrush : Brush
	{
		internal SolidBrush(IntPtr ptr)
			: base(ptr)
		{
			int num;
			Status status = GDIPlus.GdipGetSolidFillColor(ptr, out num);
			GDIPlus.CheckStatus(status);
			this.color = Color.FromArgb(num);
		}

		public SolidBrush(Color color)
		{
			this.color = color;
			Status status = GDIPlus.GdipCreateSolidFill(color.ToArgb(), out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				if (this.isModifiable)
				{
					this.color = value;
					Status status = GDIPlus.GdipSetSolidFillColor(this.nativeObject, value.ToArgb());
					GDIPlus.CheckStatus(status);
					return;
				}
				throw new ArgumentException(Locale.GetText("This SolidBrush object can't be modified."));
			}
		}

		public override object Clone()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCloneBrush(this.nativeObject, out intPtr);
			GDIPlus.CheckStatus(status);
			return new SolidBrush(intPtr);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.isModifiable)
			{
				throw new ArgumentException(Locale.GetText("This SolidBrush object can't be modified."));
			}
			base.Dispose(disposing);
		}

		internal bool isModifiable = true;

		private Color color;
	}
}
