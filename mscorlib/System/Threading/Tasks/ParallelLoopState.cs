using System;
using System.Diagnostics;
using System.Security.Permissions;
using Unity;

namespace System.Threading.Tasks
{
	[DebuggerDisplay("ShouldExitCurrentIteration = {ShouldExitCurrentIteration}")]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public class ParallelLoopState
	{
		internal ParallelLoopState(ParallelLoopStateFlags fbase)
		{
			this.m_flagsBase = fbase;
		}

		internal virtual bool InternalShouldExitCurrentIteration
		{
			get
			{
				throw new NotSupportedException(Environment.GetResourceString("This method is not supported."));
			}
		}

		public bool ShouldExitCurrentIteration
		{
			get
			{
				return this.InternalShouldExitCurrentIteration;
			}
		}

		public bool IsStopped
		{
			get
			{
				return (this.m_flagsBase.LoopStateFlags & ParallelLoopStateFlags.PLS_STOPPED) != 0;
			}
		}

		public bool IsExceptional
		{
			get
			{
				return (this.m_flagsBase.LoopStateFlags & ParallelLoopStateFlags.PLS_EXCEPTIONAL) != 0;
			}
		}

		internal virtual long? InternalLowestBreakIteration
		{
			get
			{
				throw new NotSupportedException(Environment.GetResourceString("This method is not supported."));
			}
		}

		public long? LowestBreakIteration
		{
			get
			{
				return this.InternalLowestBreakIteration;
			}
		}

		public void Stop()
		{
			this.m_flagsBase.Stop();
		}

		internal virtual void InternalBreak()
		{
			throw new NotSupportedException(Environment.GetResourceString("This method is not supported."));
		}

		public void Break()
		{
			this.InternalBreak();
		}

		internal static void Break(int iteration, ParallelLoopStateFlags32 pflags)
		{
			int pls_NONE = ParallelLoopStateFlags.PLS_NONE;
			if (pflags.AtomicLoopStateUpdate(ParallelLoopStateFlags.PLS_BROKEN, ParallelLoopStateFlags.PLS_STOPPED | ParallelLoopStateFlags.PLS_EXCEPTIONAL | ParallelLoopStateFlags.PLS_CANCELED, ref pls_NONE))
			{
				int num = pflags.m_lowestBreakIteration;
				if (iteration < num)
				{
					SpinWait spinWait = default(SpinWait);
					while (Interlocked.CompareExchange(ref pflags.m_lowestBreakIteration, iteration, num) != num)
					{
						spinWait.SpinOnce();
						num = pflags.m_lowestBreakIteration;
						if (iteration > num)
						{
							break;
						}
					}
				}
				return;
			}
			if ((pls_NONE & ParallelLoopStateFlags.PLS_STOPPED) != 0)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Break was called after Stop was called."));
			}
		}

		internal static void Break(long iteration, ParallelLoopStateFlags64 pflags)
		{
			int pls_NONE = ParallelLoopStateFlags.PLS_NONE;
			if (pflags.AtomicLoopStateUpdate(ParallelLoopStateFlags.PLS_BROKEN, ParallelLoopStateFlags.PLS_STOPPED | ParallelLoopStateFlags.PLS_EXCEPTIONAL | ParallelLoopStateFlags.PLS_CANCELED, ref pls_NONE))
			{
				long num = pflags.LowestBreakIteration;
				if (iteration < num)
				{
					SpinWait spinWait = default(SpinWait);
					while (Interlocked.CompareExchange(ref pflags.m_lowestBreakIteration, iteration, num) != num)
					{
						spinWait.SpinOnce();
						num = pflags.LowestBreakIteration;
						if (iteration > num)
						{
							break;
						}
					}
				}
				return;
			}
			if ((pls_NONE & ParallelLoopStateFlags.PLS_STOPPED) != 0)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Break was called after Stop was called."));
			}
		}

		internal ParallelLoopState()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private ParallelLoopStateFlags m_flagsBase;
	}
}
