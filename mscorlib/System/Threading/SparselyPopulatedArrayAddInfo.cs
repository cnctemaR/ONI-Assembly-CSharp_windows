using System;

namespace System.Threading
{
	internal struct SparselyPopulatedArrayAddInfo<T> where T : class
	{
		internal SparselyPopulatedArrayAddInfo(SparselyPopulatedArrayFragment<T> source, int index)
		{
			this._source = source;
			this._index = index;
		}

		internal SparselyPopulatedArrayFragment<T> Source
		{
			get
			{
				return this._source;
			}
		}

		internal int Index
		{
			get
			{
				return this._index;
			}
		}

		private SparselyPopulatedArrayFragment<T> _source;

		private int _index;
	}
}
