using System;

namespace System.Drawing.Printing
{
	[Serializable]
	public class PaperSource
	{
		public PaperSource()
		{
		}

		internal PaperSource(string sourceName, PaperSourceKind kind)
		{
			this.source_name = sourceName;
			this.kind = kind;
		}

		internal PaperSource(string sourceName, PaperSourceKind kind, bool isDefault)
		{
			this.source_name = sourceName;
			this.kind = kind;
			this.is_default = this.IsDefault;
		}

		public PaperSourceKind Kind
		{
			get
			{
				if (this.kind >= (PaperSourceKind)256)
				{
					return PaperSourceKind.Custom;
				}
				return this.kind;
			}
		}

		public string SourceName
		{
			get
			{
				return this.source_name;
			}
			set
			{
				this.source_name = value;
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
				this.kind = (PaperSourceKind)value;
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

		public override string ToString()
		{
			string text = "[PaperSource {0} Kind={1}]";
			return string.Format(text, this.SourceName, this.Kind);
		}

		private PaperSourceKind kind;

		private string source_name;

		internal bool is_default;
	}
}
