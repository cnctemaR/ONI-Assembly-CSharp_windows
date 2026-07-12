using System;
using System.Threading;

namespace Internal.Runtime.Augments
{
	internal sealed class RuntimeThread
	{
		private RuntimeThread(Thread t)
		{
			this.thread = t;
		}

		public void ResetThreadPoolThread()
		{
		}

		public static RuntimeThread InitializeThreadPoolThread()
		{
			return new RuntimeThread(null);
		}

		public static RuntimeThread Create(ParameterizedThreadStart start, int maxStackSize)
		{
			return new RuntimeThread(new Thread(start, maxStackSize));
		}

		public bool IsBackground
		{
			get
			{
				return this.thread.IsBackground;
			}
			set
			{
				this.thread.IsBackground = value;
			}
		}

		public void Start()
		{
			this.thread.Start();
		}

		public void Start(object state)
		{
			this.thread.Start(state);
		}

		public static void Sleep(int millisecondsTimeout)
		{
			Thread.Sleep(millisecondsTimeout);
		}

		public static bool Yield()
		{
			return Thread.Yield();
		}

		public static bool SpinWait(int iterations)
		{
			Thread.SpinWait(iterations);
			return true;
		}

		public static int GetCurrentProcessorId()
		{
			return 1;
		}

		internal static readonly int OptimalMaxSpinWaitsPerSpinIteration = 64;

		private readonly Thread thread;
	}
}
