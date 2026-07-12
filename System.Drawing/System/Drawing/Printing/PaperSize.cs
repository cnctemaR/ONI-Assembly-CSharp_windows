using System;
using System.Globalization;

namespace System.Drawing.Printing
{
	[Serializable]
	public class PaperSize
	{
		public PaperSize()
		{
			this._kind = PaperKind.Custom;
			this._name = string.Empty;
			this._createdByDefaultConstructor = true;
		}

		internal PaperSize(PaperKind kind, string name, int width, int height)
		{
			this._kind = kind;
			this._name = name;
			this._width = width;
			this._height = height;
		}

		public PaperSize(string name, int width, int height)
		{
			this._kind = PaperKind.Custom;
			this._name = name;
			this._width = width;
			this._height = height;
		}

		public int Height
		{
			get
			{
				return this._height;
			}
			set
			{
				if (this._kind != PaperKind.Custom && !this._createdByDefaultConstructor)
				{
					throw new ArgumentException(SR.Format("PaperSize cannot be changed unless the Kind property is set to Custom.", Array.Empty<object>()));
				}
				this._height = value;
			}
		}

		public PaperKind Kind
		{
			get
			{
				if (this._kind <= PaperKind.PrcEnvelopeNumber10Rotated && this._kind != (PaperKind)48 && this._kind != (PaperKind)49)
				{
					return this._kind;
				}
				return PaperKind.Custom;
			}
		}

		public string PaperName
		{
			get
			{
				return this._name;
			}
			set
			{
				if (this._kind != PaperKind.Custom && !this._createdByDefaultConstructor)
				{
					throw new ArgumentException(SR.Format("PaperSize cannot be changed unless the Kind property is set to Custom.", Array.Empty<object>()));
				}
				this._name = value;
			}
		}

		public int RawKind
		{
			get
			{
				return (int)this._kind;
			}
			set
			{
				this._kind = (PaperKind)value;
			}
		}

		public int Width
		{
			get
			{
				return this._width;
			}
			set
			{
				if (this._kind != PaperKind.Custom && !this._createdByDefaultConstructor)
				{
					throw new ArgumentException(SR.Format("PaperSize cannot be changed unless the Kind property is set to Custom.", Array.Empty<object>()));
				}
				this._width = value;
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[PaperSize ",
				this.PaperName,
				" Kind=",
				this.Kind.ToString(),
				" Height=",
				this.Height.ToString(CultureInfo.InvariantCulture),
				" Width=",
				this.Width.ToString(CultureInfo.InvariantCulture),
				"]"
			});
		}

		private PaperKind _kind;

		private string _name;

		private int _width;

		private int _height;

		private bool _createdByDefaultConstructor;
	}
}
