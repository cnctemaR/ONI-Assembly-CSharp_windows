using System;

namespace System.Linq.Parallel
{
	internal abstract class SpoolingTaskBase : QueryTask
	{
		protected SpoolingTaskBase(int taskIndex, QueryTaskGroupState groupState)
			: base(taskIndex, groupState)
		{
		}

		protected override void Work()
		{
			try
			{
				this.SpoolingWork();
			}
			catch (Exception ex)
			{
				OperationCanceledException ex2 = ex as OperationCanceledException;
				if (ex2 == null || !(ex2.CancellationToken == this._groupState.CancellationState.MergedCancellationToken) || !this._groupState.CancellationState.MergedCancellationToken.IsCancellationRequested)
				{
					this._groupState.CancellationState.InternalCancellationTokenSource.Cancel();
					throw;
				}
			}
			finally
			{
				this.SpoolingFinally();
			}
		}

		protected abstract void SpoolingWork();

		protected virtual void SpoolingFinally()
		{
		}
	}
}
