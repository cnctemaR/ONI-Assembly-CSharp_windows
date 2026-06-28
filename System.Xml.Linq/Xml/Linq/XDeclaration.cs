using System;

namespace System.Xml.Linq
{
	public class XDeclaration
	{
		public XDeclaration(string version, string encoding, string standalone)
		{
			this.version = version;
			this.encoding = encoding;
			this.standalone = standalone;
		}

		public XDeclaration(XDeclaration other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.version = other.version;
			this.encoding = other.encoding;
			this.standalone = other.standalone;
		}

		public string Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				this.encoding = value;
			}
		}

		public string Standalone
		{
			get
			{
				return this.standalone;
			}
			set
			{
				this.standalone = value;
			}
		}

		public string Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"<?xml",
				(this.version == null) ? null : " version=\"",
				(this.version == null) ? null : this.version,
				(this.version == null) ? null : "\"",
				(this.encoding == null) ? null : " encoding=\"",
				(this.encoding == null) ? null : this.encoding,
				(this.encoding == null) ? null : "\"",
				(this.standalone == null) ? null : " standalone=\"",
				(this.standalone == null) ? null : this.standalone,
				(this.standalone == null) ? null : "\"",
				"?>"
			});
		}

		private string encoding;

		private string standalone;

		private string version;
	}
}
