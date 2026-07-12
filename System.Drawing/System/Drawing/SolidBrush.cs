using System;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	public sealed class SolidBrush : Brush
	{
		public SolidBrush(Color color)
		{
			this._color = color;
			IntPtr zero = IntPtr.Zero;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateSolidFill(this._color.ToArgb(), out zero));
			base.SetNativeBrushInternal(zero);
		}

		internal SolidBrush(Color color, bool immutable)
			: this(color)
		{
			this._immutable = immutable;
		}

		internal SolidBrush(IntPtr nativeBrush)
		{
			base.SetNativeBrushInternal(nativeBrush);
		}

		public override object Clone()
		{
			IntPtr zero = IntPtr.Zero;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCloneBrush(new HandleRef(this, base.NativeBrush), out zero));
			return new SolidBrush(zero);
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing)
			{
				this._immutable = false;
			}
			else if (this._immutable)
			{
				throw new ArgumentException(SR.Format("Changes cannot be made to {0} because permissions are not valid.", new object[] { "Brush" }));
			}
			base.Dispose(disposing);
		}

		public Color Color
		{
			get
			{
				if (this._color == Color.Empty)
				{
					int num;
					SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetSolidFillColor(new HandleRef(this, base.NativeBrush), out num));
					this._color = Color.FromArgb(num);
				}
				return this._color;
			}
			set
			{
				if (this._immutable)
				{
					throw new ArgumentException(SR.Format("Changes cannot be made to {0} because permissions are not valid.", new object[] { "Brush" }));
				}
				if (this._color != value)
				{
					Color color = this._color;
					this.InternalSetColor(value);
				}
			}
		}

		private void InternalSetColor(Color value)
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetSolidFillColor(new HandleRef(this, base.NativeBrush), value.ToArgb()));
			this._color = value;
		}

		private Color _color = Color.Empty;

		private bool _immutable;
	}
}
