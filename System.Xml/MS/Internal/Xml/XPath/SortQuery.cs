using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class SortQuery : Query
	{
		public SortQuery(Query qyInput)
		{
			this.results = new List<SortKey>();
			this.comparer = new XPathSortComparer();
			this.qyInput = qyInput;
			this.count = 0;
		}

		private SortQuery(SortQuery other)
			: base(other)
		{
			this.results = new List<SortKey>(other.results);
			this.comparer = other.comparer.Clone();
			this.qyInput = Query.Clone(other.qyInput);
			this.count = 0;
		}

		public override void Reset()
		{
			this.count = 0;
		}

		public override void SetXsltContext(XsltContext xsltContext)
		{
			this.qyInput.SetXsltContext(xsltContext);
			if (this.qyInput.StaticType != XPathResultType.NodeSet && this.qyInput.StaticType != XPathResultType.Any)
			{
				throw XPathException.Create("Expression must evaluate to a node-set.");
			}
		}

		private void BuildResultsList()
		{
			int numSorts = this.comparer.NumSorts;
			XPathNavigator xpathNavigator;
			while ((xpathNavigator = this.qyInput.Advance()) != null)
			{
				SortKey sortKey = new SortKey(numSorts, this.results.Count, xpathNavigator.Clone());
				for (int i = 0; i < numSorts; i++)
				{
					sortKey[i] = this.comparer.Expression(i).Evaluate(this.qyInput);
				}
				this.results.Add(sortKey);
			}
			this.results.Sort(this.comparer);
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			this.qyInput.Evaluate(context);
			this.results.Clear();
			this.BuildResultsList();
			this.count = 0;
			return this;
		}

		public override XPathNavigator Advance()
		{
			if (this.count < this.results.Count)
			{
				List<SortKey> list = this.results;
				int count = this.count;
				this.count = count + 1;
				return list[count].Node;
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
				return this.results[this.count - 1].Node;
			}
		}

		internal void AddSort(Query evalQuery, IComparer comparer)
		{
			this.comparer.AddSort(evalQuery, comparer);
		}

		public override XPathNodeIterator Clone()
		{
			return new SortQuery(this);
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
				return this.results.Count;
			}
		}

		public override QueryProps Properties
		{
			get
			{
				return (QueryProps)7;
			}
		}

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			this.qyInput.PrintQuery(w);
			w.WriteElementString("XPathSortComparer", "... PrintTree() not implemented ...");
			w.WriteEndElement();
		}

		private List<SortKey> results;

		private XPathSortComparer comparer;

		private Query qyInput;
	}
}
