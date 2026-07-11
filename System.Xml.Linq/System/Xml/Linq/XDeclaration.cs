using System;
using System.Text;

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

		internal XDeclaration(XmlReader r)
		{
			this.version = r.GetAttribute("version");
			this.encoding = r.GetAttribute("encoding");
			this.standalone = r.GetAttribute("standalone");
			r.Read();
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
			StringBuilder stringBuilder = new StringBuilder("<?xml");
			if (this.version != null)
			{
				stringBuilder.Append(" version=\"");
				stringBuilder.Append(this.version);
				stringBuilder.Append("\"");
			}
			if (this.encoding != null)
			{
				stringBuilder.Append(" encoding=\"");
				stringBuilder.Append(this.encoding);
				stringBuilder.Append("\"");
			}
			if (this.standalone != null)
			{
				stringBuilder.Append(" standalone=\"");
				stringBuilder.Append(this.standalone);
				stringBuilder.Append("\"");
			}
			stringBuilder.Append("?>");
			return stringBuilder.ToString();
		}

		private string version;

		private string encoding;

		private string standalone;
	}
}
