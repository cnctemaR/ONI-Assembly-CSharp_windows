using System;
using System.Collections;
using System.Xml;

namespace Mono.Xml
{
	internal class XmlDsigC14NTransformNamespacesComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			XmlNode xmlNode = x as XmlNode;
			XmlNode xmlNode2 = y as XmlNode;
			if (xmlNode == xmlNode2)
			{
				return 0;
			}
			if (xmlNode == null)
			{
				return -1;
			}
			if (xmlNode2 == null)
			{
				return 1;
			}
			if (xmlNode.Prefix == string.Empty)
			{
				return -1;
			}
			if (xmlNode2.Prefix == string.Empty)
			{
				return 1;
			}
			return string.Compare(xmlNode.LocalName, xmlNode2.LocalName);
		}
	}
}
