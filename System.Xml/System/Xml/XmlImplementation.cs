using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Xml
{
	public class XmlImplementation
	{
		public XmlImplementation()
			: this(new NameTable())
		{
		}

		public XmlImplementation(XmlNameTable nameTable)
		{
			this.InternalNameTable = nameTable;
		}

		public virtual XmlDocument CreateDocument()
		{
			return new XmlDocument(this);
		}

		public bool HasFeature(string strFeature, string strVersion)
		{
			if (string.Compare(strFeature, "xml", true, CultureInfo.InvariantCulture) == 0)
			{
				if (strVersion != null)
				{
					if (XmlImplementation.<>f__switch$map32 == null)
					{
						XmlImplementation.<>f__switch$map32 = new Dictionary<string, int>(2)
						{
							{ "1.0", 0 },
							{ "2.0", 0 }
						};
					}
					int num;
					if (!XmlImplementation.<>f__switch$map32.TryGetValue(strVersion, out num))
					{
						return false;
					}
					if (num != 0)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal XmlNameTable InternalNameTable;
	}
}
