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
			this.localName = localName;
			this.namespaceName = namespaceName;
			this.preserveSpace = preserveSpace;
		}

		public string LocalName
		{
			get
			{
				return this.localName;
			}
			set
			{
				this.localName = value;
			}
		}

		public string NamespaceName
		{
			get
			{
				return this.namespaceName;
			}
			set
			{
				this.namespaceName = value;
			}
		}

		public bool PreserveSpace
		{
			get
			{
				return this.preserveSpace;
			}
		}

		public void GetObjectData(XmlQueryDataWriter writer)
		{
			writer.WriteStringQ(this.localName);
			writer.WriteStringQ(this.namespaceName);
			writer.Write(this.preserveSpace);
		}

		public WhitespaceRule(XmlQueryDataReader reader)
		{
			this.localName = reader.ReadStringQ();
			this.namespaceName = reader.ReadStringQ();
			this.preserveSpace = reader.ReadBoolean();
		}

		private string localName;

		private string namespaceName;

		private bool preserveSpace;
	}
}
