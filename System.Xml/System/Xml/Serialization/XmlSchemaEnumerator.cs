using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	[MonoTODO]
	public class XmlSchemaEnumerator : IEnumerator<XmlSchema>, IDisposable, IEnumerator
	{
		public XmlSchemaEnumerator(XmlSchemas list)
		{
			this.e = list.GetEnumerator();
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		void IEnumerator.Reset()
		{
			this.e.Reset();
		}

		public XmlSchema Current
		{
			get
			{
				return (XmlSchema)this.e.Current;
			}
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			return this.e.MoveNext();
		}

		private IEnumerator e;
	}
}
