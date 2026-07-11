using System;
using System.Threading;

namespace System.Net
{
	internal sealed class Semaphore : WaitHandle
	{
		internal Semaphore(int initialCount, int maxCount)
		{
			lock (this)
			{
				int num;
				this.Handle = Semaphore.CreateSemaphore_internal(initialCount, maxCount, null, out num);
			}
		}

		internal bool ReleaseSemaphore()
		{
			int num;
			return Semaphore.ReleaseSemaphore_internal(this.Handle, 1, out num);
		}
	}
}
