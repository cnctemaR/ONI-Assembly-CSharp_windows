using System;

namespace System.Threading.Tasks
{
	public struct ParallelLoopResult
	{
		public bool IsCompleted
		{
			get
			{
				return this.m_completed;
			}
		}

		public long? LowestBreakIteration
		{
			get
			{
				return this.m_lowestBreakIteration;
			}
		}

		internal bool m_completed;

		internal long? m_lowestBreakIteration;
	}
}
