using System;

namespace System.Linq.Parallel
{
	internal class SortQueryOperatorResults<TInputOutput, TSortKey> : QueryResults<TInputOutput>
	{
		internal SortQueryOperatorResults(QueryResults<TInputOutput> childQueryResults, SortQueryOperator<TInputOutput, TSortKey> op, QuerySettings settings)
		{
			this._childQueryResults = childQueryResults;
			this._op = op;
			this._settings = settings;
		}

		internal override bool IsIndexible
		{
			get
			{
				return false;
			}
		}

		internal override void GivePartitionedStream(IPartitionedStreamRecipient<TInputOutput> recipient)
		{
			this._childQueryResults.GivePartitionedStream(new SortQueryOperatorResults<TInputOutput, TSortKey>.ChildResultsRecipient(recipient, this._op, this._settings));
		}

		protected QueryResults<TInputOutput> _childQueryResults;

		private SortQueryOperator<TInputOutput, TSortKey> _op;

		private QuerySettings _settings;

		private class ChildResultsRecipient : IPartitionedStreamRecipient<TInputOutput>
		{
			internal ChildResultsRecipient(IPartitionedStreamRecipient<TInputOutput> outputRecipient, SortQueryOperator<TInputOutput, TSortKey> op, QuerySettings settings)
			{
				this._outputRecipient = outputRecipient;
				this._op = op;
				this._settings = settings;
			}

			public void Receive<TKey>(PartitionedStream<TInputOutput, TKey> childPartitionedStream)
			{
				this._op.WrapPartitionedStream<TKey>(childPartitionedStream, this._outputRecipient, false, this._settings);
			}

			private IPartitionedStreamRecipient<TInputOutput> _outputRecipient;

			private SortQueryOperator<TInputOutput, TSortKey> _op;

			private QuerySettings _settings;
		}
	}
}
