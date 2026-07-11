using System;
using System.Collections;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal struct DocumentKeyList
	{
		public DocumentKeyList(XPathNavigator rootNav, Hashtable keyTable)
		{
			this.rootNav = rootNav;
			this.keyTable = keyTable;
		}

		public XPathNavigator RootNav
		{
			get
			{
				return this.rootNav;
			}
		}

		public Hashtable KeyTable
		{
			get
			{
				return this.keyTable;
			}
		}

		private XPathNavigator rootNav;

		private Hashtable keyTable;
	}
}
