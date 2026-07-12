using System;

namespace System.Drawing.Printing
{
	[Serializable]
	public class PaperSource
	{
		public PaperSource()
		{
			this._kind = PaperSourceKind.Custom;
			this._name = string.Empty;
		}

		internal PaperSource(PaperSourceKind kind, string name)
		{
			this._kind = kind;
			this._name = name;
		}

		public PaperSourceKind Kind
		{
			get
			{
				if (this._kind >= (PaperSourceKind)256)
				{
					return PaperSourceKind.Custom;
				}
				return this._kind;
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
				this._kind = (PaperSourceKind)value;
			}
		}

		public string SourceName
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[PaperSource ",
				this.SourceName,
				" Kind=",
				this.Kind.ToString(),
				"]"
			});
		}

		private string _name;

		private PaperSourceKind _kind;
	}
}
