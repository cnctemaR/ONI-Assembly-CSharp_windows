using System;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public sealed class ReaderWriterLock : CriticalFinalizerObject
	{
		public ReaderWriterLock()
		{
			this.writer_queue = new LockQueue(this);
			this.reader_locks = new Hashtable();
			GC.SuppressFinalize(this);
		}

		~ReaderWriterLock()
		{
		}

		public bool IsReaderLockHeld
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				bool flag2;
				lock (this)
				{
					flag2 = this.reader_locks.ContainsKey(Thread.CurrentThreadId);
				}
				return flag2;
			}
		}

		public bool IsWriterLockHeld
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				bool flag2;
				lock (this)
				{
					flag2 = this.state < 0 && Thread.CurrentThreadId == this.writer_lock_owner;
				}
				return flag2;
			}
		}

		public int WriterSeqNum
		{
			get
			{
				int num;
				lock (this)
				{
					num = this.seq_num;
				}
				return num;
			}
		}

		public void AcquireReaderLock(int millisecondsTimeout)
		{
			this.AcquireReaderLock(millisecondsTimeout, 1);
		}

		private void AcquireReaderLock(int millisecondsTimeout, int initialLockCount)
		{
			lock (this)
			{
				if (this.HasWriterLock())
				{
					this.AcquireWriterLock(millisecondsTimeout, initialLockCount);
				}
				else
				{
					object obj = this.reader_locks[Thread.CurrentThreadId];
					if (obj == null)
					{
						this.readers++;
						try
						{
							if (this.state < 0 || !this.writer_queue.IsEmpty)
							{
								while (Monitor.Wait(this, millisecondsTimeout))
								{
									if (this.state >= 0)
									{
										goto IL_007B;
									}
								}
								throw new ApplicationException("Timeout expired");
							}
							IL_007B:;
						}
						finally
						{
							this.readers--;
						}
						this.reader_locks[Thread.CurrentThreadId] = initialLockCount;
						this.state += initialLockCount;
					}
					else
					{
						this.reader_locks[Thread.CurrentThreadId] = (int)obj + 1;
						this.state++;
					}
				}
			}
		}

		public void AcquireReaderLock(TimeSpan timeout)
		{
			int num = this.CheckTimeout(timeout);
			this.AcquireReaderLock(num, 1);
		}

		public void AcquireWriterLock(int millisecondsTimeout)
		{
			this.AcquireWriterLock(millisecondsTimeout, 1);
		}

		private void AcquireWriterLock(int millisecondsTimeout, int initialLockCount)
		{
			lock (this)
			{
				if (this.HasWriterLock())
				{
					this.state--;
				}
				else
				{
					if (this.state != 0 || !this.writer_queue.IsEmpty)
					{
						while (this.writer_queue.Wait(millisecondsTimeout))
						{
							if (this.state == 0)
							{
								goto IL_005A;
							}
						}
						throw new ApplicationException("Timeout expired");
					}
					IL_005A:
					this.state = -initialLockCount;
					this.writer_lock_owner = Thread.CurrentThreadId;
					this.seq_num++;
				}
			}
		}

		public void AcquireWriterLock(TimeSpan timeout)
		{
			int num = this.CheckTimeout(timeout);
			this.AcquireWriterLock(num, 1);
		}

		public bool AnyWritersSince(int seqNum)
		{
			bool flag2;
			lock (this)
			{
				flag2 = this.seq_num > seqNum;
			}
			return flag2;
		}

		public void DowngradeFromWriterLock(ref LockCookie lockCookie)
		{
			lock (this)
			{
				if (!this.HasWriterLock())
				{
					throw new ApplicationException("The thread does not have the writer lock.");
				}
				if (lockCookie.WriterLocks != 0)
				{
					this.state++;
				}
				else
				{
					this.state = lockCookie.ReaderLocks;
					this.reader_locks[Thread.CurrentThreadId] = this.state;
					if (this.readers > 0)
					{
						Monitor.PulseAll(this);
					}
				}
			}
		}

		public LockCookie ReleaseLock()
		{
			LockCookie lockCookie;
			lock (this)
			{
				lockCookie = this.GetLockCookie();
				if (lockCookie.WriterLocks != 0)
				{
					this.ReleaseWriterLock(lockCookie.WriterLocks);
				}
				else if (lockCookie.ReaderLocks != 0)
				{
					this.ReleaseReaderLock(lockCookie.ReaderLocks, lockCookie.ReaderLocks);
				}
			}
			return lockCookie;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void ReleaseReaderLock()
		{
			lock (this)
			{
				if (!this.HasWriterLock())
				{
					if (this.state > 0)
					{
						object obj = this.reader_locks[Thread.CurrentThreadId];
						if (obj != null)
						{
							this.ReleaseReaderLock((int)obj, 1);
							return;
						}
					}
					throw new ApplicationException("The thread does not have any reader or writer locks.");
				}
				this.ReleaseWriterLock();
			}
		}

		private void ReleaseReaderLock(int currentCount, int releaseCount)
		{
			int num = currentCount - releaseCount;
			if (num == 0)
			{
				this.reader_locks.Remove(Thread.CurrentThreadId);
			}
			else
			{
				this.reader_locks[Thread.CurrentThreadId] = num;
			}
			this.state -= releaseCount;
			if (this.state == 0 && !this.writer_queue.IsEmpty)
			{
				this.writer_queue.Pulse();
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void ReleaseWriterLock()
		{
			lock (this)
			{
				if (!this.HasWriterLock())
				{
					throw new ApplicationException("The thread does not have the writer lock.");
				}
				this.ReleaseWriterLock(1);
			}
		}

		private void ReleaseWriterLock(int releaseCount)
		{
			this.state += releaseCount;
			if (this.state == 0)
			{
				if (this.readers > 0)
				{
					Monitor.PulseAll(this);
					return;
				}
				if (!this.writer_queue.IsEmpty)
				{
					this.writer_queue.Pulse();
				}
			}
		}

		public void RestoreLock(ref LockCookie lockCookie)
		{
			lock (this)
			{
				if (lockCookie.WriterLocks != 0)
				{
					this.AcquireWriterLock(-1, lockCookie.WriterLocks);
				}
				else if (lockCookie.ReaderLocks != 0)
				{
					this.AcquireReaderLock(-1, lockCookie.ReaderLocks);
				}
			}
		}

		public LockCookie UpgradeToWriterLock(int millisecondsTimeout)
		{
			LockCookie lockCookie;
			lock (this)
			{
				lockCookie = this.GetLockCookie();
				if (lockCookie.WriterLocks != 0)
				{
					this.state--;
					return lockCookie;
				}
				if (lockCookie.ReaderLocks != 0)
				{
					this.ReleaseReaderLock(lockCookie.ReaderLocks, lockCookie.ReaderLocks);
				}
			}
			this.AcquireWriterLock(millisecondsTimeout);
			return lockCookie;
		}

		public LockCookie UpgradeToWriterLock(TimeSpan timeout)
		{
			int num = this.CheckTimeout(timeout);
			return this.UpgradeToWriterLock(num);
		}

		private LockCookie GetLockCookie()
		{
			LockCookie lockCookie = new LockCookie(Thread.CurrentThreadId);
			if (this.HasWriterLock())
			{
				lockCookie.WriterLocks = -this.state;
			}
			else
			{
				object obj = this.reader_locks[Thread.CurrentThreadId];
				if (obj != null)
				{
					lockCookie.ReaderLocks = (int)obj;
				}
			}
			return lockCookie;
		}

		private bool HasWriterLock()
		{
			return this.state < 0 && Thread.CurrentThreadId == this.writer_lock_owner;
		}

		private int CheckTimeout(TimeSpan timeout)
		{
			int num = (int)timeout.TotalMilliseconds;
			if (num < -1)
			{
				throw new ArgumentOutOfRangeException("timeout", "Number must be either non-negative or -1");
			}
			return num;
		}

		private int seq_num = 1;

		private int state;

		private int readers;

		private int writer_lock_owner;

		private LockQueue writer_queue;

		private Hashtable reader_locks;
	}
}
