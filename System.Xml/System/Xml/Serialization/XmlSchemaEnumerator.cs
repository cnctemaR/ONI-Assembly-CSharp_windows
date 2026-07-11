using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	public class XmlSchemaEnumerator : IEnumerator<XmlSchema>, IDisposable, IEnumerator
	{
		public XmlSchemaEnumerator(XmlSchemas list)
		{
			this.list = list;
			this.idx = -1;
			this.end = list.Count - 1;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (this.idx >= this.end)
			{
				return false;
			}
			this.idx++;
			return true;
		}

		public XmlSchema Current
		{
			get
			{
				return this.list[this.idx];
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.list[this.idx];
			}
		}

		void IEnumerator.Reset()
		{
			this.idx = -1;
		}

		private XmlSchemas list;

		private int idx;

		private int end;
	}
}
