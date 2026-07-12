using System;
using System.Xml.Xsl.Runtime;

namespace System.Xml.Xsl.Qil
{
	internal class WhitespaceRule
	{
		protected WhitespaceRule()
		{
		}

		public WhitespaceRule(string localName, string namespaceName, bool preserveSpace)
		{
			this.Init(localName, namespaceName, preserveSpace);
		}

		protected void Init(string localName, string namespaceName, bool preserveSpace)
		{
			this._localName = localName;
			this._namespaceName = namespaceName;
			this._preserveSpace = preserveSpace;
		}

		public string LocalName
		{
			get
			{
				return this._localName;
			}
			set
			{
				this._localName = value;
			}
		}

		public string NamespaceName
		{
			get
			{
				return this._namespaceName;
			}
			set
			{
				this._namespaceName = value;
			}
		}

		public bool PreserveSpace
		{
			get
			{
				return this._preserveSpace;
			}
		}

		public void GetObjectData(XmlQueryDataWriter writer)
		{
			writer.WriteStringQ(this._localName);
			writer.WriteStringQ(this._namespaceName);
			writer.Write(this._preserveSpace);
		}

		public WhitespaceRule(XmlQueryDataReader reader)
		{
			this._localName = reader.ReadStringQ();
			this._namespaceName = reader.ReadStringQ();
			this._preserveSpace = reader.ReadBoolean();
		}

		private string _localName;

		private string _namespaceName;

		private bool _preserveSpace;
	}
}
