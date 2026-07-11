using System;
using System.Threading;

namespace System.Linq.Parallel
{
	internal class PipelineSpoolingTask<TInputOutput, TIgnoreKey> : SpoolingTaskBase
	{
		internal PipelineSpoolingTask(int taskIndex, QueryTaskGroupState groupState, QueryOperatorEnumerator<TInputOutput, TIgnoreKey> source, AsynchronousChannel<TInputOutput> destination)
			: base(taskIndex, groupState)
		{
			this._source = source;
			this._destination = destination;
		}

		protected override void SpoolingWork()
		{
			TInputOutput tinputOutput = default(TInputOutput);
			TIgnoreKey tignoreKey = default(TIgnoreKey);
			QueryOperatorEnumerator<TInputOutput, TIgnoreKey> source = this._source;
			AsynchronousChannel<TInputOutput> destination = this._destination;
			CancellationToken mergedCancellationToken = this._groupState.CancellationState.MergedCancellationToken;
			while (source.MoveNext(ref tinputOutput, ref tignoreKey) && !mergedCancellationToken.IsCancellationRequested)
			{
				destination.Enqueue(tinputOutput);
			}
			destination.FlushBuffers();
		}

		protected override void SpoolingFinally()
		{
			base.SpoolingFinally();
			if (this._destination != null)
			{
				this._destination.SetDone();
			}
			this._source.Dispose();
		}

		private QueryOperatorEnumerator<TInputOutput, TIgnoreKey> _source;

		private AsynchronousChannel<TInputOutput> _destination;
	}
}
