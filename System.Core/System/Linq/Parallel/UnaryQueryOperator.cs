using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal abstract class UnaryQueryOperator<TInput, TOutput> : QueryOperator<TOutput>
	{
		internal UnaryQueryOperator(IEnumerable<TInput> child)
			: this(QueryOperator<TInput>.AsQueryOperator(child))
		{
		}

		internal UnaryQueryOperator(IEnumerable<TInput> child, bool outputOrdered)
			: this(QueryOperator<TInput>.AsQueryOperator(child), outputOrdered)
		{
		}

		private UnaryQueryOperator(QueryOperator<TInput> child)
			: this(child, child.OutputOrdered, child.SpecifiedQuerySettings)
		{
		}

		internal UnaryQueryOperator(QueryOperator<TInput> child, bool outputOrdered)
			: this(child, outputOrdered, child.SpecifiedQuerySettings)
		{
		}

		private UnaryQueryOperator(QueryOperator<TInput> child, bool outputOrdered, QuerySettings settings)
			: base(outputOrdered, settings)
		{
			this._child = child;
		}

		internal QueryOperator<TInput> Child
		{
			get
			{
				return this._child;
			}
		}

		internal sealed override OrdinalIndexState OrdinalIndexState
		{
			get
			{
				return this._indexState;
			}
		}

		protected void SetOrdinalIndexState(OrdinalIndexState indexState)
		{
			this._indexState = indexState;
		}

		internal abstract void WrapPartitionedStream<TKey>(PartitionedStream<TInput, TKey> inputStream, IPartitionedStreamRecipient<TOutput> recipient, bool preferStriping, QuerySettings settings);

		private readonly QueryOperator<TInput> _child;

		private OrdinalIndexState _indexState = OrdinalIndexState.Shuffled;

		internal class UnaryQueryOperatorResults : QueryResults<TOutput>
		{
			internal UnaryQueryOperatorResults(QueryResults<TInput> childQueryResults, UnaryQueryOperator<TInput, TOutput> op, QuerySettings settings, bool preferStriping)
			{
				this._childQueryResults = childQueryResults;
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
				this._childQueryResults.GivePartitionedStream(new UnaryQueryOperator<TInput, TOutput>.UnaryQueryOperatorResults.ChildResultsRecipient(recipient, this._op, this._preferStriping, this._settings));
			}

			protected QueryResults<TInput> _childQueryResults;

			private UnaryQueryOperator<TInput, TOutput> _op;

			private QuerySettings _settings;

			private bool _preferStriping;

			private class ChildResultsRecipient : IPartitionedStreamRecipient<TInput>
			{
				internal ChildResultsRecipient(IPartitionedStreamRecipient<TOutput> outputRecipient, UnaryQueryOperator<TInput, TOutput> op, bool preferStriping, QuerySettings settings)
				{
					this._outputRecipient = outputRecipient;
					this._op = op;
					this._preferStriping = preferStriping;
					this._settings = settings;
				}

				public void Receive<TKey>(PartitionedStream<TInput, TKey> inputStream)
				{
					this._op.WrapPartitionedStream<TKey>(inputStream, this._outputRecipient, this._preferStriping, this._settings);
				}

				private IPartitionedStreamRecipient<TOutput> _outputRecipient;

				private UnaryQueryOperator<TInput, TOutput> _op;

				private bool _preferStriping;

				private QuerySettings _settings;
			}
		}
	}
}
