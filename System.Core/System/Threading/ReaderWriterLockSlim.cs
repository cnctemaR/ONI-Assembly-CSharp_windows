using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace System.Threading
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class ReaderWriterLockSlim : IDisposable
	{
		private void InitializeThreadCounts()
		{
			this.upgradeLockOwnerId = -1;
			this.writeLockOwnerId = -1;
		}

		public ReaderWriterLockSlim()
			: this(LockRecursionPolicy.NoRecursion)
		{
		}

		public ReaderWriterLockSlim(LockRecursionPolicy recursionPolicy)
		{
			if (recursionPolicy == LockRecursionPolicy.SupportsRecursion)
			{
				this.fIsReentrant = true;
			}
			this.InitializeThreadCounts();
			this.fNoWaiters = true;
			this.lockID = Interlocked.Increment(ref ReaderWriterLockSlim.s_nextLockID);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsRWEntryEmpty(ReaderWriterCount rwc)
		{
			return rwc.lockID == 0L || (rwc.readercount == 0 && rwc.writercount == 0 && rwc.upgradecount == 0);
		}

		private bool IsRwHashEntryChanged(ReaderWriterCount lrwc)
		{
			return lrwc.lockID != this.lockID;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ReaderWriterCount GetThreadRWCount(bool dontAllocate)
		{
			ReaderWriterCount next = ReaderWriterLockSlim.t_rwc;
			ReaderWriterCount readerWriterCount = null;
			while (next != null)
			{
				if (next.lockID == this.lockID)
				{
					return next;
				}
				if (!dontAllocate && readerWriterCount == null && ReaderWriterLockSlim.IsRWEntryEmpty(next))
				{
					readerWriterCount = next;
				}
				next = next.next;
			}
			if (dontAllocate)
			{
				return null;
			}
			if (readerWriterCount == null)
			{
				readerWriterCount = new ReaderWriterCount();
				readerWriterCount.next = ReaderWriterLockSlim.t_rwc;
				ReaderWriterLockSlim.t_rwc = readerWriterCount;
			}
			readerWriterCount.lockID = this.lockID;
			return readerWriterCount;
		}

		public void EnterReadLock()
		{
			this.TryEnterReadLock(-1);
		}

		public bool TryEnterReadLock(TimeSpan timeout)
		{
			return this.TryEnterReadLock(new ReaderWriterLockSlim.TimeoutTracker(timeout));
		}

		public bool TryEnterReadLock(int millisecondsTimeout)
		{
			return this.TryEnterReadLock(new ReaderWriterLockSlim.TimeoutTracker(millisecondsTimeout));
		}

		private bool TryEnterReadLock(ReaderWriterLockSlim.TimeoutTracker timeout)
		{
			return this.TryEnterReadLockCore(timeout);
		}

		private bool TryEnterReadLockCore(ReaderWriterLockSlim.TimeoutTracker timeout)
		{
			if (this.fDisposed)
			{
				throw new ObjectDisposedException(null);
			}
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			ReaderWriterCount readerWriterCount;
			if (!this.fIsReentrant)
			{
				if (managedThreadId == this.writeLockOwnerId)
				{
					throw new LockRecursionException(global::SR.GetString("A read lock may not be acquired with the write lock held in this mode."));
				}
				this.EnterMyLock();
				readerWriterCount = this.GetThreadRWCount(false);
				if (readerWriterCount.readercount > 0)
				{
					this.ExitMyLock();
					throw new LockRecursionException(global::SR.GetString("Recursive read lock acquisitions not allowed in this mode."));
				}
				if (managedThreadId == this.upgradeLockOwnerId)
				{
					readerWriterCount.readercount++;
					this.owners += 1U;
					this.ExitMyLock();
					return true;
				}
			}
			else
			{
				this.EnterMyLock();
				readerWriterCount = this.GetThreadRWCount(false);
				if (readerWriterCount.readercount > 0)
				{
					readerWriterCount.readercount++;
					this.ExitMyLock();
					return true;
				}
				if (managedThreadId == this.upgradeLockOwnerId)
				{
					readerWriterCount.readercount++;
					this.owners += 1U;
					this.ExitMyLock();
					this.fUpgradeThreadHoldingRead = true;
					return true;
				}
				if (managedThreadId == this.writeLockOwnerId)
				{
					readerWriterCount.readercount++;
					this.owners += 1U;
					this.ExitMyLock();
					return true;
				}
			}
			bool flag = true;
			int num = 0;
			while (this.owners >= 268435454U)
			{
				if (num < 20)
				{
					this.ExitMyLock();
					if (timeout.IsExpired)
					{
						return false;
					}
					num++;
					ReaderWriterLockSlim.SpinWait(num);
					this.EnterMyLock();
					if (this.IsRwHashEntryChanged(readerWriterCount))
					{
						readerWriterCount = this.GetThreadRWCount(false);
					}
				}
				else if (this.readEvent == null)
				{
					this.LazyCreateEvent(ref this.readEvent, false);
					if (this.IsRwHashEntryChanged(readerWriterCount))
					{
						readerWriterCount = this.GetThreadRWCount(false);
					}
				}
				else
				{
					flag = this.WaitOnEvent(this.readEvent, ref this.numReadWaiters, timeout, false);
					if (!flag)
					{
						return false;
					}
					if (this.IsRwHashEntryChanged(readerWriterCount))
					{
						readerWriterCount = this.GetThreadRWCount(false);
					}
				}
			}
			this.owners += 1U;
			readerWriterCount.readercount++;
			this.ExitMyLock();
			return flag;
		}

		public void EnterWriteLock()
		{
			this.TryEnterWriteLock(-1);
		}

		public bool TryEnterWriteLock(TimeSpan timeout)
		{
			return this.TryEnterWriteLock(new ReaderWriterLockSlim.TimeoutTracker(timeout));
		}

		public bool TryEnterWriteLock(int millisecondsTimeout)
		{
			return this.TryEnterWriteLock(new ReaderWriterLockSlim.TimeoutTracker(millisecondsTimeout));
		}

		private bool TryEnterWriteLock(ReaderWriterLockSlim.TimeoutTracker timeout)
		{
			return this.TryEnterWriteLockCore(timeout);
		}

		private bool TryEnterWriteLockCore(ReaderWriterLockSlim.TimeoutTracker timeout)
		{
			if (this.fDisposed)
			{
				throw new ObjectDisposedException(null);
			}
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			bool flag = false;
			ReaderWriterCount readerWriterCount;
			if (!this.fIsReentrant)
			{
				if (managedThreadId == this.writeLockOwnerId)
				{
					throw new LockRecursionException(global::SR.GetString("Recursive write lock acquisitions not allowed in this mode."));
				}
				if (managedThreadId == this.upgradeLockOwnerId)
				{
					flag = true;
				}
				this.EnterMyLock();
				readerWriterCount = this.GetThreadRWCount(true);
				if (readerWriterCount != null && readerWriterCount.readercount > 0)
				{
					this.ExitMyLock();
					throw new LockRecursionException(global::SR.GetString("Write lock may not be acquired with read lock held. This pattern is prone to deadlocks. Please ensure that read locks are released before taking a write lock. If an upgrade is necessary, use an upgrade lock in place of the read lock."));
				}
			}
			else
			{
				this.EnterMyLock();
				readerWriterCount = this.GetThreadRWCount(false);
				if (managedThreadId == this.writeLockOwnerId)
				{
					readerWriterCount.writercount++;
					this.ExitMyLock();
					return true;
				}
				if (managedThreadId == this.upgradeLockOwnerId)
				{
					flag = true;
				}
				else if (readerWriterCount.readercount > 0)
				{
					this.ExitMyLock();
					throw new LockRecursionException(global::SR.GetString("Write lock may not be acquired with read lock held. This pattern is prone to deadlocks. Please ensure that read locks are released before taking a write lock. If an upgrade is necessary, use an upgrade lock in place of the read lock."));
				}
			}
			int num = 0;
			while (!this.IsWriterAcquired())
			{
				if (flag)
				{
					uint numReaders = this.GetNumReaders();
					if (numReaders == 1U)
					{
						this.SetWriterAcquired();
					}
					else
					{
						if (numReaders != 2U || readerWriterCount == null)
						{
							goto IL_012E;
						}
						if (this.IsRwHashEntryChanged(readerWriterCount))
						{
							readerWriterCount = this.GetThreadRWCount(false);
						}
						if (readerWriterCount.readercount <= 0)
						{
							goto IL_012E;
						}
						this.SetWriterAcquired();
					}
					IL_01C6:
					if (this.fIsReentrant)
					{
						if (this.IsRwHashEntryChanged(readerWriterCount))
						{
							readerWriterCount = this.GetThreadRWCount(false);
						}
						readerWriterCount.writercount++;
					}
					this.ExitMyLock();
					this.writeLockOwnerId = managedThreadId;
					return true;
				}
				IL_012E:
				if (num < 20)
				{
					this.ExitMyLock();
					if (timeout.IsExpired)
					{
						return false;
					}
					num++;
					ReaderWriterLockSlim.SpinWait(num);
					this.EnterMyLock();
				}
				else if (flag)
				{
					if (this.waitUpgradeEvent == null)
					{
						this.LazyCreateEvent(ref this.waitUpgradeEvent, true);
					}
					else if (!this.WaitOnEvent(this.waitUpgradeEvent, ref this.numWriteUpgradeWaiters, timeout, true))
					{
						return false;
					}
				}
				else if (this.writeEvent == null)
				{
					this.LazyCreateEvent(ref this.writeEvent, true);
				}
				else if (!this.WaitOnEvent(this.writeEvent, ref this.numWriteWaiters, timeout, true))
				{
					return false;
				}
			}
			this.SetWriterAcquired();
			goto IL_01C6;
		}

		public void EnterUpgradeableReadLock()
		{
			this.TryEnterUpgradeableReadLock(-1);
		}

		public bool TryEnterUpgradeableReadLock(TimeSpan timeout)
		{
			return this.TryEnterUpgradeableReadLock(new ReaderWriterLockSlim.TimeoutTracker(timeout));
		}

		public bool TryEnterUpgradeableReadLock(int millisecondsTimeout)
		{
			return this.TryEnterUpgradeableReadLock(new ReaderWriterLockSlim.TimeoutTracker(millisecondsTimeout));
		}

		private bool TryEnterUpgradeableReadLock(ReaderWriterLockSlim.TimeoutTracker timeout)
		{
			return this.TryEnterUpgradeableReadLockCore(timeout);
		}

		private bool TryEnterUpgradeableReadLockCore(ReaderWriterLockSlim.TimeoutTracker timeout)
		{
			if (this.fDisposed)
			{
				throw new ObjectDisposedException(null);
			}
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			ReaderWriterCount readerWriterCount;
			if (!this.fIsReentrant)
			{
				if (managedThreadId == this.upgradeLockOwnerId)
				{
					throw new LockRecursionException(global::SR.GetString("Recursive upgradeable lock acquisitions not allowed in this mode."));
				}
				if (managedThreadId == this.writeLockOwnerId)
				{
					throw new LockRecursionException(global::SR.GetString("Upgradeable lock may not be acquired with write lock held in this mode. Acquiring Upgradeable lock gives the ability to read along with an option to upgrade to a writer."));
				}
				this.EnterMyLock();
				readerWriterCount = this.GetThreadRWCount(true);
				if (readerWriterCount != null && readerWriterCount.readercount > 0)
				{
					this.ExitMyLock();
					throw new LockRecursionException(global::SR.GetString("Upgradeable lock may not be acquired with read lock held."));
				}
			}
			else
			{
				this.EnterMyLock();
				readerWriterCount = this.GetThreadRWCount(false);
				if (managedThreadId == this.upgradeLockOwnerId)
				{
					readerWriterCount.upgradecount++;
					this.ExitMyLock();
					return true;
				}
				if (managedThreadId == this.writeLockOwnerId)
				{
					this.owners += 1U;
					this.upgradeLockOwnerId = managedThreadId;
					readerWriterCount.upgradecount++;
					if (readerWriterCount.readercount > 0)
					{
						this.fUpgradeThreadHoldingRead = true;
					}
					this.ExitMyLock();
					return true;
				}
				if (readerWriterCount.readercount > 0)
				{
					this.ExitMyLock();
					throw new LockRecursionException(global::SR.GetString("Upgradeable lock may not be acquired with read lock held."));
				}
			}
			int num = 0;
			while (this.upgradeLockOwnerId != -1 || this.owners >= 268435454U)
			{
				if (num < 20)
				{
					this.ExitMyLock();
					if (timeout.IsExpired)
					{
						return false;
					}
					num++;
					ReaderWriterLockSlim.SpinWait(num);
					this.EnterMyLock();
				}
				else if (this.upgradeEvent == null)
				{
					this.LazyCreateEvent(ref this.upgradeEvent, true);
				}
				else if (!this.WaitOnEvent(this.upgradeEvent, ref this.numUpgradeWaiters, timeout, false))
				{
					return false;
				}
			}
			this.owners += 1U;
			this.upgradeLockOwnerId = managedThreadId;
			if (this.fIsReentrant)
			{
				if (this.IsRwHashEntryChanged(readerWriterCount))
				{
					readerWriterCount = this.GetThreadRWCount(false);
				}
				readerWriterCount.upgradecount++;
			}
			this.ExitMyLock();
			return true;
		}

		public void ExitReadLock()
		{
			this.EnterMyLock();
			ReaderWriterCount threadRWCount = this.GetThreadRWCount(true);
			if (threadRWCount == null || threadRWCount.readercount < 1)
			{
				this.ExitMyLock();
				throw new SynchronizationLockException(global::SR.GetString("The read lock is being released without being held."));
			}
			if (this.fIsReentrant)
			{
				if (threadRWCount.readercount > 1)
				{
					threadRWCount.readercount--;
					this.ExitMyLock();
					return;
				}
				if (Thread.CurrentThread.ManagedThreadId == this.upgradeLockOwnerId)
				{
					this.fUpgradeThreadHoldingRead = false;
				}
			}
			this.owners -= 1U;
			threadRWCount.readercount--;
			this.ExitAndWakeUpAppropriateWaiters();
		}

		public void ExitWriteLock()
		{
			if (!this.fIsReentrant)
			{
				if (Thread.CurrentThread.ManagedThreadId != this.writeLockOwnerId)
				{
					throw new SynchronizationLockException(global::SR.GetString("The write lock is being released without being held."));
				}
				this.EnterMyLock();
			}
			else
			{
				this.EnterMyLock();
				ReaderWriterCount threadRWCount = this.GetThreadRWCount(false);
				if (threadRWCount == null)
				{
					this.ExitMyLock();
					throw new SynchronizationLockException(global::SR.GetString("The write lock is being released without being held."));
				}
				if (threadRWCount.writercount < 1)
				{
					this.ExitMyLock();
					throw new SynchronizationLockException(global::SR.GetString("The write lock is being released without being held."));
				}
				threadRWCount.writercount--;
				if (threadRWCount.writercount > 0)
				{
					this.ExitMyLock();
					return;
				}
			}
			this.ClearWriterAcquired();
			this.writeLockOwnerId = -1;
			this.ExitAndWakeUpAppropriateWaiters();
		}

		public void ExitUpgradeableReadLock()
		{
			if (!this.fIsReentrant)
			{
				if (Thread.CurrentThread.ManagedThreadId != this.upgradeLockOwnerId)
				{
					throw new SynchronizationLockException(global::SR.GetString("The upgradeable lock is being released without being held."));
				}
				this.EnterMyLock();
			}
			else
			{
				this.EnterMyLock();
				ReaderWriterCount threadRWCount = this.GetThreadRWCount(true);
				if (threadRWCount == null)
				{
					this.ExitMyLock();
					throw new SynchronizationLockException(global::SR.GetString("The upgradeable lock is being released without being held."));
				}
				if (threadRWCount.upgradecount < 1)
				{
					this.ExitMyLock();
					throw new SynchronizationLockException(global::SR.GetString("The upgradeable lock is being released without being held."));
				}
				threadRWCount.upgradecount--;
				if (threadRWCount.upgradecount > 0)
				{
					this.ExitMyLock();
					return;
				}
				this.fUpgradeThreadHoldingRead = false;
			}
			this.owners -= 1U;
			this.upgradeLockOwnerId = -1;
			this.ExitAndWakeUpAppropriateWaiters();
		}

		private void LazyCreateEvent(ref EventWaitHandle waitEvent, bool makeAutoResetEvent)
		{
			this.ExitMyLock();
			EventWaitHandle eventWaitHandle;
			if (makeAutoResetEvent)
			{
				eventWaitHandle = new AutoResetEvent(false);
			}
			else
			{
				eventWaitHandle = new ManualResetEvent(false);
			}
			this.EnterMyLock();
			if (waitEvent == null)
			{
				waitEvent = eventWaitHandle;
				return;
			}
			eventWaitHandle.Close();
		}

		private bool WaitOnEvent(EventWaitHandle waitEvent, ref uint numWaiters, ReaderWriterLockSlim.TimeoutTracker timeout, bool isWriteWaiter)
		{
			waitEvent.Reset();
			numWaiters += 1U;
			this.fNoWaiters = false;
			if (this.numWriteWaiters == 1U)
			{
				this.SetWritersWaiting();
			}
			if (this.numWriteUpgradeWaiters == 1U)
			{
				this.SetUpgraderWaiting();
			}
			bool flag = false;
			this.ExitMyLock();
			try
			{
				flag = waitEvent.WaitOne(timeout.RemainingMilliseconds);
			}
			finally
			{
				this.EnterMyLock();
				numWaiters -= 1U;
				if (this.numWriteWaiters == 0U && this.numWriteUpgradeWaiters == 0U && this.numUpgradeWaiters == 0U && this.numReadWaiters == 0U)
				{
					this.fNoWaiters = true;
				}
				if (this.numWriteWaiters == 0U)
				{
					this.ClearWritersWaiting();
				}
				if (this.numWriteUpgradeWaiters == 0U)
				{
					this.ClearUpgraderWaiting();
				}
				if (!flag)
				{
					if (isWriteWaiter)
					{
						this.ExitAndWakeUpAppropriateReadWaiters();
					}
					else
					{
						this.ExitMyLock();
					}
				}
			}
			return flag;
		}

		private void ExitAndWakeUpAppropriateWaiters()
		{
			if (this.fNoWaiters)
			{
				this.ExitMyLock();
				return;
			}
			this.ExitAndWakeUpAppropriateWaitersPreferringWriters();
		}

		private void ExitAndWakeUpAppropriateWaitersPreferringWriters()
		{
			uint numReaders = this.GetNumReaders();
			if (this.fIsReentrant && this.numWriteUpgradeWaiters > 0U && this.fUpgradeThreadHoldingRead && numReaders == 2U)
			{
				this.ExitMyLock();
				this.waitUpgradeEvent.Set();
				return;
			}
			if (numReaders == 1U && this.numWriteUpgradeWaiters > 0U)
			{
				this.ExitMyLock();
				this.waitUpgradeEvent.Set();
				return;
			}
			if (numReaders == 0U && this.numWriteWaiters > 0U)
			{
				this.ExitMyLock();
				this.writeEvent.Set();
				return;
			}
			this.ExitAndWakeUpAppropriateReadWaiters();
		}

		private void ExitAndWakeUpAppropriateReadWaiters()
		{
			if (this.numWriteWaiters != 0U || this.numWriteUpgradeWaiters != 0U || this.fNoWaiters)
			{
				this.ExitMyLock();
				return;
			}
			bool flag = this.numReadWaiters > 0U;
			bool flag2 = this.numUpgradeWaiters != 0U && this.upgradeLockOwnerId == -1;
			this.ExitMyLock();
			if (flag)
			{
				this.readEvent.Set();
			}
			if (flag2)
			{
				this.upgradeEvent.Set();
			}
		}

		private bool IsWriterAcquired()
		{
			return (this.owners & 3221225471U) == 0U;
		}

		private void SetWriterAcquired()
		{
			this.owners |= 2147483648U;
		}

		private void ClearWriterAcquired()
		{
			this.owners &= 2147483647U;
		}

		private void SetWritersWaiting()
		{
			this.owners |= 1073741824U;
		}

		private void ClearWritersWaiting()
		{
			this.owners &= 3221225471U;
		}

		private void SetUpgraderWaiting()
		{
			this.owners |= 536870912U;
		}

		private void ClearUpgraderWaiting()
		{
			this.owners &= 3758096383U;
		}

		private uint GetNumReaders()
		{
			return this.owners & 268435455U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnterMyLock()
		{
			if (Interlocked.CompareExchange(ref this.myLock, 1, 0) != 0)
			{
				this.EnterMyLockSpin();
			}
		}

		private void EnterMyLockSpin()
		{
			int processorCount = Environment.ProcessorCount;
			int num = 0;
			for (;;)
			{
				if (num < 10 && processorCount > 1)
				{
					Thread.SpinWait(20 * (num + 1));
				}
				else if (num < 15)
				{
					Thread.Sleep(0);
				}
				else
				{
					Thread.Sleep(1);
				}
				if (this.myLock == 0 && Interlocked.CompareExchange(ref this.myLock, 1, 0) == 0)
				{
					break;
				}
				num++;
			}
		}

		private void ExitMyLock()
		{
			Volatile.Write(ref this.myLock, 0);
		}

		private static void SpinWait(int SpinCount)
		{
			if (SpinCount < 5 && Environment.ProcessorCount > 1)
			{
				Thread.SpinWait(20 * SpinCount);
				return;
			}
			if (SpinCount < 17)
			{
				Thread.Sleep(0);
				return;
			}
			Thread.Sleep(1);
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !this.fDisposed)
			{
				if (this.WaitingReadCount > 0 || this.WaitingUpgradeCount > 0 || this.WaitingWriteCount > 0)
				{
					throw new SynchronizationLockException(global::SR.GetString("The lock is being disposed while still being used. It either is being held by a thread and/or has active waiters waiting to acquire the lock."));
				}
				if (this.IsReadLockHeld || this.IsUpgradeableReadLockHeld || this.IsWriteLockHeld)
				{
					throw new SynchronizationLockException(global::SR.GetString("The lock is being disposed while still being used. It either is being held by a thread and/or has active waiters waiting to acquire the lock."));
				}
				if (this.writeEvent != null)
				{
					this.writeEvent.Close();
					this.writeEvent = null;
				}
				if (this.readEvent != null)
				{
					this.readEvent.Close();
					this.readEvent = null;
				}
				if (this.upgradeEvent != null)
				{
					this.upgradeEvent.Close();
					this.upgradeEvent = null;
				}
				if (this.waitUpgradeEvent != null)
				{
					this.waitUpgradeEvent.Close();
					this.waitUpgradeEvent = null;
				}
				this.fDisposed = true;
			}
		}

		public bool IsReadLockHeld
		{
			get
			{
				return this.RecursiveReadCount > 0;
			}
		}

		public bool IsUpgradeableReadLockHeld
		{
			get
			{
				return this.RecursiveUpgradeCount > 0;
			}
		}

		public bool IsWriteLockHeld
		{
			get
			{
				return this.RecursiveWriteCount > 0;
			}
		}

		public LockRecursionPolicy RecursionPolicy
		{
			get
			{
				if (this.fIsReentrant)
				{
					return LockRecursionPolicy.SupportsRecursion;
				}
				return LockRecursionPolicy.NoRecursion;
			}
		}

		public int CurrentReadCount
		{
			get
			{
				int numReaders = (int)this.GetNumReaders();
				if (this.upgradeLockOwnerId != -1)
				{
					return numReaders - 1;
				}
				return numReaders;
			}
		}

		public int RecursiveReadCount
		{
			get
			{
				int num = 0;
				ReaderWriterCount threadRWCount = this.GetThreadRWCount(true);
				if (threadRWCount != null)
				{
					num = threadRWCount.readercount;
				}
				return num;
			}
		}

		public int RecursiveUpgradeCount
		{
			get
			{
				if (this.fIsReentrant)
				{
					int num = 0;
					ReaderWriterCount threadRWCount = this.GetThreadRWCount(true);
					if (threadRWCount != null)
					{
						num = threadRWCount.upgradecount;
					}
					return num;
				}
				if (Thread.CurrentThread.ManagedThreadId == this.upgradeLockOwnerId)
				{
					return 1;
				}
				return 0;
			}
		}

		public int RecursiveWriteCount
		{
			get
			{
				if (this.fIsReentrant)
				{
					int num = 0;
					ReaderWriterCount threadRWCount = this.GetThreadRWCount(true);
					if (threadRWCount != null)
					{
						num = threadRWCount.writercount;
					}
					return num;
				}
				if (Thread.CurrentThread.ManagedThreadId == this.writeLockOwnerId)
				{
					return 1;
				}
				return 0;
			}
		}

		public int WaitingReadCount
		{
			get
			{
				return (int)this.numReadWaiters;
			}
		}

		public int WaitingUpgradeCount
		{
			get
			{
				return (int)this.numUpgradeWaiters;
			}
		}

		public int WaitingWriteCount
		{
			get
			{
				return (int)this.numWriteWaiters;
			}
		}

		private bool fIsReentrant;

		private int myLock;

		private const int LockSpinCycles = 20;

		private const int LockSpinCount = 10;

		private const int LockSleep0Count = 5;

		private uint numWriteWaiters;

		private uint numReadWaiters;

		private uint numWriteUpgradeWaiters;

		private uint numUpgradeWaiters;

		private bool fNoWaiters;

		private int upgradeLockOwnerId;

		private int writeLockOwnerId;

		private EventWaitHandle writeEvent;

		private EventWaitHandle readEvent;

		private EventWaitHandle upgradeEvent;

		private EventWaitHandle waitUpgradeEvent;

		private static long s_nextLockID;

		private long lockID;

		[ThreadStatic]
		private static ReaderWriterCount t_rwc;

		private bool fUpgradeThreadHoldingRead;

		private const int MaxSpinCount = 20;

		private uint owners;

		private const uint WRITER_HELD = 2147483648U;

		private const uint WAITING_WRITERS = 1073741824U;

		private const uint WAITING_UPGRADER = 536870912U;

		private const uint MAX_READER = 268435454U;

		private const uint READER_MASK = 268435455U;

		private bool fDisposed;

		private struct TimeoutTracker
		{
			public TimeoutTracker(TimeSpan timeout)
			{
				long num = (long)timeout.TotalMilliseconds;
				if (num < -1L || num > 2147483647L)
				{
					throw new ArgumentOutOfRangeException("timeout");
				}
				this.m_total = (int)num;
				if (this.m_total != -1 && this.m_total != 0)
				{
					this.m_start = Environment.TickCount;
					return;
				}
				this.m_start = 0;
			}

			public TimeoutTracker(int millisecondsTimeout)
			{
				if (millisecondsTimeout < -1)
				{
					throw new ArgumentOutOfRangeException("millisecondsTimeout");
				}
				this.m_total = millisecondsTimeout;
				if (this.m_total != -1 && this.m_total != 0)
				{
					this.m_start = Environment.TickCount;
					return;
				}
				this.m_start = 0;
			}

			public int RemainingMilliseconds
			{
				get
				{
					if (this.m_total == -1 || this.m_total == 0)
					{
						return this.m_total;
					}
					int num = Environment.TickCount - this.m_start;
					if (num < 0 || num >= this.m_total)
					{
						return 0;
					}
					return this.m_total - num;
				}
			}

			public bool IsExpired
			{
				get
				{
					return this.RemainingMilliseconds == 0;
				}
			}

			private int m_total;

			private int m_start;
		}
	}
}
