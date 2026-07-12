using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal abstract class ResetableIterator : XPathNodeIterator
	{
		public ResetableIterator()
		{
			this.count = -1;
		}

		protected ResetableIterator(ResetableIterator other)
		{
			this.count = other.count;
		}

		protected void ResetCount()
		{
			this.count = -1;
		}

		public abstract void Reset();

		public abstract override int CurrentPosition { get; }
	}
}
