using System;

namespace System.Threading.Tasks
{
	internal class ParallelLoopStateFlags
	{
		internal int LoopStateFlags
		{
			get
			{
				return this._loopStateFlags;
			}
		}

		internal bool AtomicLoopStateUpdate(int newState, int illegalStates)
		{
			int num = 0;
			return this.AtomicLoopStateUpdate(newState, illegalStates, ref num);
		}

		internal bool AtomicLoopStateUpdate(int newState, int illegalStates, ref int oldState)
		{
			SpinWait spinWait = default(SpinWait);
			for (;;)
			{
				oldState = this._loopStateFlags;
				if ((oldState & illegalStates) != 0)
				{
					break;
				}
				if (Interlocked.CompareExchange(ref this._loopStateFlags, oldState | newState, oldState) == oldState)
				{
					return true;
				}
				spinWait.SpinOnce();
			}
			return false;
		}

		internal void SetExceptional()
		{
			this.AtomicLoopStateUpdate(1, 0);
		}

		internal void Stop()
		{
			if (!this.AtomicLoopStateUpdate(4, 2))
			{
				throw new InvalidOperationException("Stop was called after Break was called.");
			}
		}

		internal bool Cancel()
		{
			return this.AtomicLoopStateUpdate(8, 0);
		}

		internal const int ParallelLoopStateNone = 0;

		internal const int ParallelLoopStateExceptional = 1;

		internal const int ParallelLoopStateBroken = 2;

		internal const int ParallelLoopStateStopped = 4;

		internal const int ParallelLoopStateCanceled = 8;

		private volatile int _loopStateFlags;
	}
}
