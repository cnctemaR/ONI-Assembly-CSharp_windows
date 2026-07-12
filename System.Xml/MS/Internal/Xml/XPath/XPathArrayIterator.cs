using System;
using System.Collections;
using System.Diagnostics;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	[DebuggerDisplay("Position={CurrentPosition}, Current={debuggerDisplayProxy, nq}")]
	internal class XPathArrayIterator : ResetableIterator
	{
		public XPathArrayIterator(IList list)
		{
			this.list = list;
		}

		public XPathArrayIterator(XPathArrayIterator it)
		{
			this.list = it.list;
			this.index = it.index;
		}

		public XPathArrayIterator(XPathNodeIterator nodeIterator)
		{
			this.list = new ArrayList();
			while (nodeIterator.MoveNext())
			{
				XPathNavigator xpathNavigator = nodeIterator.Current;
				this.list.Add(xpathNavigator.Clone());
			}
		}

		public IList AsList
		{
			get
			{
				return this.list;
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathArrayIterator(this);
		}

		public override XPathNavigator Current
		{
			get
			{
				if (this.index < 1)
				{
					throw new InvalidOperationException(SR.Format("Enumeration has not started. Call MoveNext.", string.Empty));
				}
				return (XPathNavigator)this.list[this.index - 1];
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return this.index;
			}
		}

		public override int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public override bool MoveNext()
		{
			if (this.index == this.list.Count)
			{
				return false;
			}
			this.index++;
			return true;
		}

		public override void Reset()
		{
			this.index = 0;
		}

		public override IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		private object debuggerDisplayProxy
		{
			get
			{
				if (this.index >= 1)
				{
					return new XPathNavigator.DebuggerDisplayProxy(this.Current);
				}
				return null;
			}
		}

		protected IList list;

		protected int index;
	}
}
