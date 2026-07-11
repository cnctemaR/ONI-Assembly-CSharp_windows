using System;
using System.Collections;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class XmlDsigNodeList : XmlNodeList
	{
		public XmlDsigNodeList(ArrayList rgNodes)
		{
			this._rgNodes = rgNodes;
		}

		public override int Count
		{
			get
			{
				return this._rgNodes.Count;
			}
		}

		public override IEnumerator GetEnumerator()
		{
			return this._rgNodes.GetEnumerator();
		}

		public override XmlNode Item(int index)
		{
			if (index < 0 || this._rgNodes.Count <= index)
			{
				return null;
			}
			return (XmlNode)this._rgNodes[index];
		}

		private ArrayList _rgNodes;
	}
}
