using System;
using System.Collections;

namespace System.Xml.XPath
{
	public abstract class XPathNodeIterator : IEnumerable, ICloneable
	{
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public virtual int Count
		{
			get
			{
				if (this._count == -1)
				{
					XPathNodeIterator xpathNodeIterator = this.Clone();
					while (xpathNodeIterator.MoveNext())
					{
					}
					this._count = xpathNodeIterator.CurrentPosition;
				}
				return this._count;
			}
		}

		public abstract XPathNavigator Current { get; }

		public abstract int CurrentPosition { get; }

		public abstract XPathNodeIterator Clone();

		public virtual IEnumerator GetEnumerator()
		{
			while (this.MoveNext())
			{
				XPathNavigator xpathNavigator = this.Current;
				yield return xpathNavigator;
			}
			yield break;
		}

		public abstract bool MoveNext();

		private int _count = -1;
	}
}
