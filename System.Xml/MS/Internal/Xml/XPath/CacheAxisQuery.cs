using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal abstract class CacheAxisQuery : BaseAxisQuery
	{
		public CacheAxisQuery(Query qyInput, string name, string prefix, XPathNodeType typeTest)
			: base(qyInput, name, prefix, typeTest)
		{
			this.outputBuffer = new List<XPathNavigator>();
			this.count = 0;
		}

		protected CacheAxisQuery(CacheAxisQuery other)
			: base(other)
		{
			this.outputBuffer = new List<XPathNavigator>(other.outputBuffer);
			this.count = other.count;
		}

		public override void Reset()
		{
			this.count = 0;
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			base.Evaluate(context);
			this.outputBuffer.Clear();
			return this;
		}

		public override XPathNavigator Advance()
		{
			if (this.count < this.outputBuffer.Count)
			{
				List<XPathNavigator> list = this.outputBuffer;
				int count = this.count;
				this.count = count + 1;
				return list[count];
			}
			return null;
		}

		public override XPathNavigator Current
		{
			get
			{
				if (this.count == 0)
				{
					return null;
				}
				return this.outputBuffer[this.count - 1];
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return this.count;
			}
		}

		public override int Count
		{
			get
			{
				return this.outputBuffer.Count;
			}
		}

		public override QueryProps Properties
		{
			get
			{
				return (QueryProps)23;
			}
		}

		protected List<XPathNavigator> outputBuffer;
	}
}
