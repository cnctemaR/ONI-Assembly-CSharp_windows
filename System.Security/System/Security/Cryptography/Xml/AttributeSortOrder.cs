using System;
using System.Collections;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class AttributeSortOrder : IComparer
	{
		internal AttributeSortOrder()
		{
		}

		public int Compare(object a, object b)
		{
			XmlNode xmlNode = a as XmlNode;
			XmlNode xmlNode2 = b as XmlNode;
			if (xmlNode == null || xmlNode2 == null)
			{
				throw new ArgumentException();
			}
			int num = string.CompareOrdinal(xmlNode.NamespaceURI, xmlNode2.NamespaceURI);
			if (num != 0)
			{
				return num;
			}
			return string.CompareOrdinal(xmlNode.LocalName, xmlNode2.LocalName);
		}
	}
}
