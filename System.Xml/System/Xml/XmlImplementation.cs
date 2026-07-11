using System;

namespace System.Xml
{
	public class XmlImplementation
	{
		public XmlImplementation()
			: this(new NameTable())
		{
		}

		public XmlImplementation(XmlNameTable nt)
		{
			this.nameTable = nt;
		}

		public bool HasFeature(string strFeature, string strVersion)
		{
			return string.Compare("XML", strFeature, StringComparison.OrdinalIgnoreCase) == 0 && (strVersion == null || strVersion == "1.0" || strVersion == "2.0");
		}

		public virtual XmlDocument CreateDocument()
		{
			return new XmlDocument(this);
		}

		internal XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		private XmlNameTable nameTable;
	}
}
