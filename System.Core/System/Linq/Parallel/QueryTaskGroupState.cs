using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	internal class QueryTaskGroupState
	{
		internal QueryTaskGroupState(CancellationState cancellationState, int queryId)
		{
			this._cancellationState = cancellationState;
			this._queryId = queryId;
		}

		internal bool IsAlreadyEnded
		{
			get
			{
				return this._alreadyEnded == 1;
			}
		}

		internal CancellationState CancellationState
		{
			get
			{
				return this._cancellationState;
			}
		}

		internal int QueryId
		{
			get
			{
				return this._queryId;
			}
		}

		internal void QueryBegin(Task rootTask)
		{
			this._rootTask = rootTask;
		}

		internal void QueryEnd(bool userInitiatedDispose)
		{
			if (Interlocked.Exchange(ref this._alreadyEnded, 1) == 0)
			{
				try
				{
					this._rootTask.Wait();
				}
				catch (AggregateException ex)
				{
					AggregateException ex2 = ex.Flatten();
					bool flag = true;
					for (int i = 0; i < ex2.InnerExceptions.Count; i++)
					{
						OperationCanceledException ex3 = ex2.InnerExceptions[i] as OperationCanceledException;
						if (ex3 == null || !ex3.CancellationToken.IsCancellationRequested || ex3.CancellationToken != this._cancellationState.ExternalCancellationToken)
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						throw ex2;
					}
				}
				finally
				{
					IDisposable rootTask = this._rootTask;
					if (rootTask != null)
					{
						rootTask.Dispose();
					}
				}
				if (this._cancellationState.MergedCancellationToken.IsCancellationRequested)
				{
					if (!this._cancellationState.TopLevelDisposedFlag.Value)
					{
						CancellationState.ThrowWithStandardMessageIfCanceled(this._cancellationState.ExternalCancellationToken);
					}
					if (!userInitiatedDispose)
					{
						throw new ObjectDisposedException("enumerator", "The query enumerator has been disposed.");
					}
				}
			}
		}

		private Task _rootTask;

		private int _alreadyEnded;

		private CancellationState _cancellationState;

		private int _queryId;
	}
}
