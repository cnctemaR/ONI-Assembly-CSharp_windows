using System;
using System.Collections;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class NamespaceSortOrder : IComparer
	{
		internal NamespaceSortOrder()
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
			bool flag = Utils.IsDefaultNamespaceNode(xmlNode);
			bool flag2 = Utils.IsDefaultNamespaceNode(xmlNode2);
			if (flag && flag2)
			{
				return 0;
			}
			if (flag)
			{
				return -1;
			}
			if (flag2)
			{
				return 1;
			}
			return string.CompareOrdinal(xmlNode.LocalName, xmlNode2.LocalName);
		}
	}
}
