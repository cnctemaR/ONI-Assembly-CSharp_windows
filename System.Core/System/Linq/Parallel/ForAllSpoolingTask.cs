using System;

namespace System.Linq.Parallel
{
	internal class ForAllSpoolingTask<TInputOutput, TIgnoreKey> : SpoolingTaskBase
	{
		internal ForAllSpoolingTask(int taskIndex, QueryTaskGroupState groupState, QueryOperatorEnumerator<TInputOutput, TIgnoreKey> source)
			: base(taskIndex, groupState)
		{
			this._source = source;
		}

		protected override void SpoolingWork()
		{
			TInputOutput tinputOutput = default(TInputOutput);
			TIgnoreKey tignoreKey = default(TIgnoreKey);
			while (this._source.MoveNext(ref tinputOutput, ref tignoreKey))
			{
			}
		}

		protected override void SpoolingFinally()
		{
			base.SpoolingFinally();
			this._source.Dispose();
		}

		private QueryOperatorEnumerator<TInputOutput, TIgnoreKey> _source;
	}
}
