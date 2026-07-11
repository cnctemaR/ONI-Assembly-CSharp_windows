using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class XPathSortComparer : IComparer<SortKey>
	{
		public XPathSortComparer(int size)
		{
			if (size <= 0)
			{
				size = 3;
			}
			this.expressions = new Query[size];
			this.comparers = new IComparer[size];
		}

		public XPathSortComparer()
			: this(3)
		{
		}

		public void AddSort(Query evalQuery, IComparer comparer)
		{
			if (this.numSorts == this.expressions.Length)
			{
				Query[] array = new Query[this.numSorts * 2];
				IComparer[] array2 = new IComparer[this.numSorts * 2];
				for (int i = 0; i < this.numSorts; i++)
				{
					array[i] = this.expressions[i];
					array2[i] = this.comparers[i];
				}
				this.expressions = array;
				this.comparers = array2;
			}
			if (evalQuery.StaticType == XPathResultType.NodeSet || evalQuery.StaticType == XPathResultType.Any)
			{
				evalQuery = new StringFunctions(Function.FunctionType.FuncString, new Query[] { evalQuery });
			}
			this.expressions[this.numSorts] = evalQuery;
			this.comparers[this.numSorts] = comparer;
			this.numSorts++;
		}

		public int NumSorts
		{
			get
			{
				return this.numSorts;
			}
		}

		public Query Expression(int i)
		{
			return this.expressions[i];
		}

		int IComparer<SortKey>.Compare(SortKey x, SortKey y)
		{
			for (int i = 0; i < x.NumKeys; i++)
			{
				int num = this.comparers[i].Compare(x[i], y[i]);
				if (num != 0)
				{
					return num;
				}
			}
			return x.OriginalPosition - y.OriginalPosition;
		}

		internal XPathSortComparer Clone()
		{
			XPathSortComparer xpathSortComparer = new XPathSortComparer(this.numSorts);
			for (int i = 0; i < this.numSorts; i++)
			{
				xpathSortComparer.comparers[i] = this.comparers[i];
				xpathSortComparer.expressions[i] = (Query)this.expressions[i].Clone();
			}
			xpathSortComparer.numSorts = this.numSorts;
			return xpathSortComparer;
		}

		private const int minSize = 3;

		private Query[] expressions;

		private IComparer[] comparers;

		private int numSorts;
	}
}
