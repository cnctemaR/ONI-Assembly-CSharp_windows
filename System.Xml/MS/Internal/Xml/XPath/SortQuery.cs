using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class SortQuery : Query
	{
		public SortQuery(Query qyInput)
		{
			this._results = new List<SortKey>();
			this._comparer = new XPathSortComparer();
			this._qyInput = qyInput;
			this.count = 0;
		}

		private SortQuery(SortQuery other)
			: base(other)
		{
			this._results = new List<SortKey>(other._results);
			this._comparer = other._comparer.Clone();
			this._qyInput = Query.Clone(other._qyInput);
			this.count = 0;
		}

		public override void Reset()
		{
			this.count = 0;
		}

		public override void SetXsltContext(XsltContext xsltContext)
		{
			this._qyInput.SetXsltContext(xsltContext);
			if (this._qyInput.StaticType != XPathResultType.NodeSet && this._qyInput.StaticType != XPathResultType.Any)
			{
				throw XPathException.Create("Expression must evaluate to a node-set.");
			}
		}

		private void BuildResultsList()
		{
			int numSorts = this._comparer.NumSorts;
			XPathNavigator xpathNavigator;
			while ((xpathNavigator = this._qyInput.Advance()) != null)
			{
				SortKey sortKey = new SortKey(numSorts, this._results.Count, xpathNavigator.Clone());
				for (int i = 0; i < numSorts; i++)
				{
					sortKey[i] = this._comparer.Expression(i).Evaluate(this._qyInput);
				}
				this._results.Add(sortKey);
			}
			this._results.Sort(this._comparer);
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			this._qyInput.Evaluate(context);
			this._results.Clear();
			this.BuildResultsList();
			this.count = 0;
			return this;
		}

		public override XPathNavigator Advance()
		{
			if (this.count < this._results.Count)
			{
				List<SortKey> results = this._results;
				int count = this.count;
				this.count = count + 1;
				return results[count].Node;
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
				return this._results[this.count - 1].Node;
			}
		}

		internal void AddSort(Query evalQuery, IComparer comparer)
		{
			this._comparer.AddSort(evalQuery, comparer);
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
				return this._results.Count;
			}
		}

		public override QueryProps Properties
		{
			get
			{
				return (QueryProps)7;
			}
		}

		private List<SortKey> _results;

		private XPathSortComparer _comparer;

		private Query _qyInput;
	}
}
