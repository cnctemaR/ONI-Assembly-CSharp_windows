using System;
using System.Collections;

namespace System.Xml
{
	internal class XmlEmptyElementListEnumerator : IEnumerator
	{
		public XmlEmptyElementListEnumerator(XmlElementList list)
		{
		}

		public bool MoveNext()
		{
			return false;
		}

		public void Reset()
		{
		}

		public object Current
		{
			get
			{
				return null;
			}
		}
	}
}
