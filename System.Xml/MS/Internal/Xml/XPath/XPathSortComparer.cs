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
			this._expressions = new Query[size];
			this._comparers = new IComparer[size];
		}

		public XPathSortComparer()
			: this(3)
		{
		}

		public void AddSort(Query evalQuery, IComparer comparer)
		{
			if (this._numSorts == this._expressions.Length)
			{
				Query[] array = new Query[this._numSorts * 2];
				IComparer[] array2 = new IComparer[this._numSorts * 2];
				for (int i = 0; i < this._numSorts; i++)
				{
					array[i] = this._expressions[i];
					array2[i] = this._comparers[i];
				}
				this._expressions = array;
				this._comparers = array2;
			}
			if (evalQuery.StaticType == XPathResultType.NodeSet || evalQuery.StaticType == XPathResultType.Any)
			{
				evalQuery = new StringFunctions(Function.FunctionType.FuncString, new Query[] { evalQuery });
			}
			this._expressions[this._numSorts] = evalQuery;
			this._comparers[this._numSorts] = comparer;
			this._numSorts++;
		}

		public int NumSorts
		{
			get
			{
				return this._numSorts;
			}
		}

		public Query Expression(int i)
		{
			return this._expressions[i];
		}

		int IComparer<SortKey>.Compare(SortKey x, SortKey y)
		{
			for (int i = 0; i < x.NumKeys; i++)
			{
				int num = this._comparers[i].Compare(x[i], y[i]);
				if (num != 0)
				{
					return num;
				}
			}
			return x.OriginalPosition - y.OriginalPosition;
		}

		internal XPathSortComparer Clone()
		{
			XPathSortComparer xpathSortComparer = new XPathSortComparer(this._numSorts);
			for (int i = 0; i < this._numSorts; i++)
			{
				xpathSortComparer._comparers[i] = this._comparers[i];
				xpathSortComparer._expressions[i] = (Query)this._expressions[i].Clone();
			}
			xpathSortComparer._numSorts = this._numSorts;
			return xpathSortComparer;
		}

		private const int minSize = 3;

		private Query[] _expressions;

		private IComparer[] _comparers;

		private int _numSorts;
	}
}
