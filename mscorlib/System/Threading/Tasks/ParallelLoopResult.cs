using System;

namespace System.Threading.Tasks
{
	public struct ParallelLoopResult
	{
		public bool IsCompleted
		{
			get
			{
				return this._completed;
			}
		}

		public long? LowestBreakIteration
		{
			get
			{
				return this._lowestBreakIteration;
			}
		}

		internal bool _completed;

		internal long? _lowestBreakIteration;
	}
}
