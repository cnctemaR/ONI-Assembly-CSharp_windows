using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal abstract class CacheOutputQuery : Query
	{
		public CacheOutputQuery(Query input)
		{
			this.input = input;
			this.outputBuffer = new List<XPathNavigator>();
			this.count = 0;
		}

		protected CacheOutputQuery(CacheOutputQuery other)
			: base(other)
		{
			this.input = Query.Clone(other.input);
			this.outputBuffer = new List<XPathNavigator>(other.outputBuffer);
			this.count = other.count;
		}

		public override void Reset()
		{
			this.count = 0;
		}

		public override void SetXsltContext(XsltContext context)
		{
			this.input.SetXsltContext(context);
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			this.outputBuffer.Clear();
			this.count = 0;
			return this.input.Evaluate(context);
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

		public override XPathResultType StaticType
		{
			get
			{
				return XPathResultType.NodeSet;
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

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			this.input.PrintQuery(w);
			w.WriteEndElement();
		}

		internal Query input;

		protected List<XPathNavigator> outputBuffer;
	}
}
