using System;
using System.Collections;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class Key
	{
		public Key(XmlQualifiedName name, int matchkey, int usekey)
		{
			this.name = name;
			this.matchKey = matchkey;
			this.useKey = usekey;
			this.keyNodes = null;
		}

		public XmlQualifiedName Name
		{
			get
			{
				return this.name;
			}
		}

		public int MatchKey
		{
			get
			{
				return this.matchKey;
			}
		}

		public int UseKey
		{
			get
			{
				return this.useKey;
			}
		}

		public void AddKey(XPathNavigator root, Hashtable table)
		{
			if (this.keyNodes == null)
			{
				this.keyNodes = new ArrayList();
			}
			this.keyNodes.Add(new DocumentKeyList(root, table));
		}

		public Hashtable GetKeys(XPathNavigator root)
		{
			if (this.keyNodes != null)
			{
				for (int i = 0; i < this.keyNodes.Count; i++)
				{
					if (((DocumentKeyList)this.keyNodes[i]).RootNav.IsSamePosition(root))
					{
						return ((DocumentKeyList)this.keyNodes[i]).KeyTable;
					}
				}
			}
			return null;
		}

		public Key Clone()
		{
			return new Key(this.name, this.matchKey, this.useKey);
		}

		private XmlQualifiedName name;

		private int matchKey;

		private int useKey;

		private ArrayList keyNodes;
	}
}
