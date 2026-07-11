using System;

namespace System.Threading
{
	internal class LockQueue
	{
		public LockQueue(ReaderWriterLock rwlock)
		{
			this.rwlock = rwlock;
		}

		public bool Wait(int timeout)
		{
			bool flag = false;
			bool flag3;
			try
			{
				lock (this)
				{
					this.lockCount++;
					Monitor.Exit(this.rwlock);
					flag = true;
					flag3 = Monitor.Wait(this, timeout);
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Enter(this.rwlock);
					this.lockCount--;
				}
			}
			return flag3;
		}

		public bool IsEmpty
		{
			get
			{
				bool flag2;
				lock (this)
				{
					flag2 = this.lockCount == 0;
				}
				return flag2;
			}
		}

		public void Pulse()
		{
			lock (this)
			{
				Monitor.Pulse(this);
			}
		}

		private ReaderWriterLock rwlock;

		private int lockCount;
	}
}
