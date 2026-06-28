using System;

namespace System.Drawing.Printing
{
	[Serializable]
	public class PaperSize
	{
		public PaperSize()
		{
		}

		public PaperSize(string name, int width, int height)
		{
			this.width = width;
			this.height = height;
			this.name = name;
		}

		internal PaperSize(string name, int width, int height, PaperKind kind, bool isDefault)
		{
			this.width = width;
			this.height = height;
			this.name = name;
			this.is_default = isDefault;
		}

		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (this.kind != PaperKind.Custom)
				{
					throw new ArgumentException();
				}
				this.width = value;
			}
		}

		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				if (this.kind != PaperKind.Custom)
				{
					throw new ArgumentException();
				}
				this.height = value;
			}
		}

		public string PaperName
		{
			get
			{
				return this.name;
			}
			set
			{
				if (this.kind != PaperKind.Custom)
				{
					throw new ArgumentException();
				}
				this.name = value;
			}
		}

		public PaperKind Kind
		{
			get
			{
				if (this.kind > PaperKind.PrcEnvelopeNumber10Rotated)
				{
					return PaperKind.Custom;
				}
				return this.kind;
			}
		}

		public int RawKind
		{
			get
			{
				return (int)this.kind;
			}
			set
			{
				this.kind = (PaperKind)value;
			}
		}

		internal bool IsDefault
		{
			get
			{
				return this.is_default;
			}
			set
			{
				this.is_default = value;
			}
		}

		internal void SetKind(PaperKind k)
		{
			this.kind = k;
		}

		public override string ToString()
		{
			string text = "[PaperSize {0} Kind={1} Height={2} Width={3}]";
			return string.Format(text, new object[] { this.PaperName, this.Kind, this.Height, this.Width });
		}

		private string name;

		private int width;

		private int height;

		private PaperKind kind;

		internal bool is_default;
	}
}
