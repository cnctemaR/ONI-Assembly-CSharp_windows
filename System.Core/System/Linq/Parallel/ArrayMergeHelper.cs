using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq.Parallel
{
	internal class ArrayMergeHelper<TInputOutput> : IMergeHelper<TInputOutput>
	{
		public ArrayMergeHelper(QuerySettings settings, QueryResults<TInputOutput> queryResults)
		{
			this._settings = settings;
			this._queryResults = queryResults;
			int count = this._queryResults.Count;
			this._outputArray = new TInputOutput[count];
		}

		private void ToArrayElement(int index)
		{
			this._outputArray[index] = this._queryResults[index];
		}

		public void Execute()
		{
			new QueryExecutionOption<int>(QueryOperator<int>.AsQueryOperator(ParallelEnumerable.Range(0, this._queryResults.Count)), this._settings).ForAll<int>(new Action<int>(this.ToArrayElement));
		}

		[ExcludeFromCodeCoverage]
		public IEnumerator<TInputOutput> GetEnumerator()
		{
			return this.GetResultsAsArray().GetEnumerator();
		}

		public TInputOutput[] GetResultsAsArray()
		{
			return this._outputArray;
		}

		private QueryResults<TInputOutput> _queryResults;

		private TInputOutput[] _outputArray;

		private QuerySettings _settings;
	}
}
