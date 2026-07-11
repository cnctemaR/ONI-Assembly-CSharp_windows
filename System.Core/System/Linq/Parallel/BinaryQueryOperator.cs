using System;

namespace System.Linq.Parallel
{
	internal abstract class BinaryQueryOperator<TLeftInput, TRightInput, TOutput> : QueryOperator<TOutput>
	{
		internal BinaryQueryOperator(ParallelQuery<TLeftInput> leftChild, ParallelQuery<TRightInput> rightChild)
			: this(QueryOperator<TLeftInput>.AsQueryOperator(leftChild), QueryOperator<TRightInput>.AsQueryOperator(rightChild))
		{
		}

		internal BinaryQueryOperator(QueryOperator<TLeftInput> leftChild, QueryOperator<TRightInput> rightChild)
			: base(false, leftChild.SpecifiedQuerySettings.Merge(rightChild.SpecifiedQuerySettings))
		{
			this._leftChild = leftChild;
			this._rightChild = rightChild;
		}

		internal QueryOperator<TLeftInput> LeftChild
		{
			get
			{
				return this._leftChild;
			}
		}

		internal QueryOperator<TRightInput> RightChild
		{
			get
			{
				return this._rightChild;
			}
		}

		internal sealed override OrdinalIndexState OrdinalIndexState
		{
			get
			{
				return this._indexState;
			}
		}

		protected void SetOrdinalIndex(OrdinalIndexState indexState)
		{
			this._indexState = indexState;
		}

		public abstract void WrapPartitionedStream<TLeftKey, TRightKey>(PartitionedStream<TLeftInput, TLeftKey> leftPartitionedStream, PartitionedStream<TRightInput, TRightKey> rightPartitionedStream, IPartitionedStreamRecipient<TOutput> outputRecipient, bool preferStriping, QuerySettings settings);

		private readonly QueryOperator<TLeftInput> _leftChild;

		private readonly QueryOperator<TRightInput> _rightChild;

		private OrdinalIndexState _indexState = OrdinalIndexState.Shuffled;

		internal class BinaryQueryOperatorResults : QueryResults<TOutput>
		{
			internal BinaryQueryOperatorResults(QueryResults<TLeftInput> leftChildQueryResults, QueryResults<TRightInput> rightChildQueryResults, BinaryQueryOperator<TLeftInput, TRightInput, TOutput> op, QuerySettings settings, bool preferStriping)
			{
				this._leftChildQueryResults = leftChildQueryResults;
				this._rightChildQueryResults = rightChildQueryResults;
				this._op = op;
				this._settings = settings;
				this._preferStriping = preferStriping;
			}

			internal override void GivePartitionedStream(IPartitionedStreamRecipient<TOutput> recipient)
			{
				if (this._settings.ExecutionMode.Value == ParallelExecutionMode.Default && this._op.LimitsParallelism)
				{
					PartitionedStream<TOutput, int> partitionedStream = ExchangeUtilities.PartitionDataSource<TOutput>(this._op.AsSequentialQuery(this._settings.CancellationState.ExternalCancellationToken), this._settings.DegreeOfParallelism.Value, this._preferStriping);
					recipient.Receive<int>(partitionedStream);
					return;
				}
				if (this.IsIndexible)
				{
					PartitionedStream<TOutput, int> partitionedStream2 = ExchangeUtilities.PartitionDataSource<TOutput>(this, this._settings.DegreeOfParallelism.Value, this._preferStriping);
					recipient.Receive<int>(partitionedStream2);
					return;
				}
				this._leftChildQueryResults.GivePartitionedStream(new BinaryQueryOperator<TLeftInput, TRightInput, TOutput>.BinaryQueryOperatorResults.LeftChildResultsRecipient(recipient, this, this._preferStriping, this._settings));
			}

			protected QueryResults<TLeftInput> _leftChildQueryResults;

			protected QueryResults<TRightInput> _rightChildQueryResults;

			private BinaryQueryOperator<TLeftInput, TRightInput, TOutput> _op;

			private QuerySettings _settings;

			private bool _preferStriping;

			private class LeftChildResultsRecipient : IPartitionedStreamRecipient<TLeftInput>
			{
				internal LeftChildResultsRecipient(IPartitionedStreamRecipient<TOutput> outputRecipient, BinaryQueryOperator<TLeftInput, TRightInput, TOutput>.BinaryQueryOperatorResults results, bool preferStriping, QuerySettings settings)
				{
					this._outputRecipient = outputRecipient;
					this._results = results;
					this._preferStriping = preferStriping;
					this._settings = settings;
				}

				public void Receive<TLeftKey>(PartitionedStream<TLeftInput, TLeftKey> source)
				{
					BinaryQueryOperator<TLeftInput, TRightInput, TOutput>.BinaryQueryOperatorResults.RightChildResultsRecipient<TLeftKey> rightChildResultsRecipient = new BinaryQueryOperator<TLeftInput, TRightInput, TOutput>.BinaryQueryOperatorResults.RightChildResultsRecipient<TLeftKey>(this._outputRecipient, this._results._op, source, this._preferStriping, this._settings);
					this._results._rightChildQueryResults.GivePartitionedStream(rightChildResultsRecipient);
				}

				private IPartitionedStreamRecipient<TOutput> _outputRecipient;

				private BinaryQueryOperator<TLeftInput, TRightInput, TOutput>.BinaryQueryOperatorResults _results;

				private bool _preferStriping;

				private QuerySettings _settings;
			}

			private class RightChildResultsRecipient<TLeftKey> : IPartitionedStreamRecipient<TRightInput>
			{
				internal RightChildResultsRecipient(IPartitionedStreamRecipient<TOutput> outputRecipient, BinaryQueryOperator<TLeftInput, TRightInput, TOutput> op, PartitionedStream<TLeftInput, TLeftKey> leftPartitionedStream, bool preferStriping, QuerySettings settings)
				{
					this._outputRecipient = outputRecipient;
					this._op = op;
					this._preferStriping = preferStriping;
					this._leftPartitionedStream = leftPartitionedStream;
					this._settings = settings;
				}

				public void Receive<TRightKey>(PartitionedStream<TRightInput, TRightKey> rightPartitionedStream)
				{
					this._op.WrapPartitionedStream<TLeftKey, TRightKey>(this._leftPartitionedStream, rightPartitionedStream, this._outputRecipient, this._preferStriping, this._settings);
				}

				private IPartitionedStreamRecipient<TOutput> _outputRecipient;

				private PartitionedStream<TLeftInput, TLeftKey> _leftPartitionedStream;

				private BinaryQueryOperator<TLeftInput, TRightInput, TOutput> _op;

				private bool _preferStriping;

				private QuerySettings _settings;
			}
		}
	}
}
