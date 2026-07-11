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
			bool flag2;
			try
			{
				lock (this)
				{
					this.lockCount++;
					Monitor.Exit(this.rwlock);
					flag = true;
					flag2 = Monitor.Wait(this, timeout);
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
			return flag2;
		}

		public bool IsEmpty
		{
			get
			{
				bool flag;
				lock (this)
				{
					flag = this.lockCount == 0;
				}
				return flag;
			}
		}

		public void Pulse()
		{
			lock (this)
			{
				Monitor.Pulse(this);
			}
		}

		public void PulseAll()
		{
			lock (this)
			{
				Monitor.PulseAll(this);
			}
		}

		private ReaderWriterLock rwlock;

		private int lockCount;
	}
}
