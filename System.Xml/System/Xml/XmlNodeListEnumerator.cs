using System;
using System.Collections;

namespace System.Xml
{
	internal class XmlNodeListEnumerator : IEnumerator
	{
		public XmlNodeListEnumerator(XPathNodeList list)
		{
			this.list = list;
			this.index = -1;
			this.valid = false;
		}

		public void Reset()
		{
			this.index = -1;
		}

		public bool MoveNext()
		{
			this.index++;
			if (this.list.ReadUntil(this.index + 1) - 1 < this.index)
			{
				return false;
			}
			this.valid = this.list[this.index] != null;
			return this.valid;
		}

		public object Current
		{
			get
			{
				if (this.valid)
				{
					return this.list[this.index];
				}
				return null;
			}
		}

		private XPathNodeList list;

		private int index;

		private bool valid;
	}
}
