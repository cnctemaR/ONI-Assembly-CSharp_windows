using System;

namespace System.Threading.Tasks
{
	internal class ParallelLoopStateFlags64 : ParallelLoopStateFlags
	{
		internal long LowestBreakIteration
		{
			get
			{
				if (IntPtr.Size >= 8)
				{
					return this._lowestBreakIteration;
				}
				return Interlocked.Read(ref this._lowestBreakIteration);
			}
		}

		internal long? NullableLowestBreakIteration
		{
			get
			{
				if (this._lowestBreakIteration == 9223372036854775807L)
				{
					return null;
				}
				if (IntPtr.Size >= 8)
				{
					return new long?(this._lowestBreakIteration);
				}
				return new long?(Interlocked.Read(ref this._lowestBreakIteration));
			}
		}

		internal bool ShouldExitLoop(long CallerIteration)
		{
			int loopStateFlags = base.LoopStateFlags;
			return loopStateFlags != 0 && ((loopStateFlags & 13) != 0 || ((loopStateFlags & 2) != 0 && CallerIteration > this.LowestBreakIteration));
		}

		internal bool ShouldExitLoop()
		{
			int loopStateFlags = base.LoopStateFlags;
			return loopStateFlags != 0 && (loopStateFlags & 9) != 0;
		}

		internal long _lowestBreakIteration = long.MaxValue;
	}
}
