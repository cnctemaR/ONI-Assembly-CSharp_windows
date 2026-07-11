using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	internal class XChildrenIterator : IEnumerable, IEnumerable<object>
	{
		public XChildrenIterator(XContainer source)
		{
			this.source = source;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IEnumerator<object> GetEnumerator()
		{
			if (this.n == null)
			{
				this.n = this.source.FirstNode;
				if (this.n == null)
				{
					yield break;
				}
			}
			do
			{
				yield return this.n;
				this.n = this.n.NextNode;
			}
			while (this.n != this.source.LastNode);
			yield break;
		}

		private XContainer source;

		private XNode n;
	}
}
