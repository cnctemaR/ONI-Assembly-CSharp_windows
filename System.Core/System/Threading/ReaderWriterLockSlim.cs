using System;
using System.Security.Permissions;

namespace System.Threading
{
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	public class ReaderWriterLockSlim : IDisposable
	{
		public ReaderWriterLockSlim()
		{
		}

		public ReaderWriterLockSlim(LockRecursionPolicy recursionPolicy)
		{
			this.recursionPolicy = recursionPolicy;
			if (recursionPolicy != LockRecursionPolicy.NoRecursion)
			{
				throw new NotImplementedException("recursionPolicy != NoRecursion not currently implemented");
			}
		}

		public void EnterReadLock()
		{
			this.TryEnterReadLock(-1);
		}

		public bool TryEnterReadLock(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			if (this.read_locks == null)
			{
				throw new ObjectDisposedException(null);
			}
			if (Thread.CurrentThread == this.write_thread)
			{
				throw new LockRecursionException("Read lock cannot be acquired while write lock is held");
			}
			this.EnterMyLock();
			ReaderWriterLockSlim.LockDetails readLockDetails = this.GetReadLockDetails(Thread.CurrentThread.ManagedThreadId, true);
			if (readLockDetails.ReadLocks != 0)
			{
				this.ExitMyLock();
				throw new LockRecursionException("Recursive read lock can only be aquired in SupportsRecursion mode");
			}
			readLockDetails.ReadLocks++;
			while (this.owners < 0 || this.numWriteWaiters != 0U)
			{
				if (millisecondsTimeout == 0)
				{
					this.ExitMyLock();
					return false;
				}
				if (this.readEvent == null)
				{
					this.LazyCreateEvent(ref this.readEvent, false);
				}
				else if (!this.WaitOnEvent(this.readEvent, ref this.numReadWaiters, millisecondsTimeout))
				{
					return false;
				}
			}
			this.owners++;
			this.ExitMyLock();
			return true;
		}

		public bool TryEnterReadLock(TimeSpan timeout)
		{
			return this.TryEnterReadLock(ReaderWriterLockSlim.CheckTimeout(timeout));
		}

		public void ExitReadLock()
		{
			this.EnterMyLock();
			if (this.owners < 1)
			{
				this.ExitMyLock();
				throw new SynchronizationLockException("Releasing lock and no read lock taken");
			}
			this.owners--;
			this.GetReadLockDetails(Thread.CurrentThread.ManagedThreadId, false).ReadLocks--;
			this.ExitAndWakeUpAppropriateWaiters();
		}

		public void EnterWriteLock()
		{
			this.TryEnterWriteLock(-1);
		}

		public bool TryEnterWriteLock(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			if (this.read_locks == null)
			{
				throw new ObjectDisposedException(null);
			}
			if (this.IsWriteLockHeld)
			{
				throw new LockRecursionException();
			}
			this.EnterMyLock();
			ReaderWriterLockSlim.LockDetails readLockDetails = this.GetReadLockDetails(Thread.CurrentThread.ManagedThreadId, false);
			if (readLockDetails != null && readLockDetails.ReadLocks > 0)
			{
				this.ExitMyLock();
				throw new LockRecursionException("Write lock cannot be acquired while read lock is held");
			}
			while (this.owners != 0)
			{
				if (this.owners == 1 && this.upgradable_thread == Thread.CurrentThread)
				{
					this.owners = -1;
					this.write_thread = Thread.CurrentThread;
					IL_0178:
					this.ExitMyLock();
					return true;
				}
				if (millisecondsTimeout == 0)
				{
					this.ExitMyLock();
					return false;
				}
				if (this.upgradable_thread == Thread.CurrentThread)
				{
					if (this.upgradeEvent == null)
					{
						this.LazyCreateEvent(ref this.upgradeEvent, false);
					}
					else
					{
						if (this.numUpgradeWaiters > 0U)
						{
							this.ExitMyLock();
							throw new ApplicationException("Upgrading lock to writer lock already in process, deadlock");
						}
						if (!this.WaitOnEvent(this.upgradeEvent, ref this.numUpgradeWaiters, millisecondsTimeout))
						{
							return false;
						}
					}
				}
				else if (this.writeEvent == null)
				{
					this.LazyCreateEvent(ref this.writeEvent, true);
				}
				else if (!this.WaitOnEvent(this.writeEvent, ref this.numWriteWaiters, millisecondsTimeout))
				{
					return false;
				}
			}
			this.owners = -1;
			this.write_thread = Thread.CurrentThread;
			goto IL_0178;
		}

		public bool TryEnterWriteLock(TimeSpan timeout)
		{
			return this.TryEnterWriteLock(ReaderWriterLockSlim.CheckTimeout(timeout));
		}

		public void ExitWriteLock()
		{
			this.EnterMyLock();
			if (this.owners != -1)
			{
				this.ExitMyLock();
				throw new SynchronizationLockException("Calling ExitWriterLock when no write lock is held");
			}
			if (this.upgradable_thread == Thread.CurrentThread)
			{
				this.owners = 1;
			}
			else
			{
				this.owners = 0;
			}
			this.write_thread = null;
			this.ExitAndWakeUpAppropriateWaiters();
		}

		public void EnterUpgradeableReadLock()
		{
			this.TryEnterUpgradeableReadLock(-1);
		}

		public bool TryEnterUpgradeableReadLock(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			if (this.read_locks == null)
			{
				throw new ObjectDisposedException(null);
			}
			if (this.IsUpgradeableReadLockHeld)
			{
				throw new LockRecursionException();
			}
			if (this.IsWriteLockHeld)
			{
				throw new LockRecursionException();
			}
			this.EnterMyLock();
			while (this.owners != 0 || this.numWriteWaiters != 0U || this.upgradable_thread != null)
			{
				if (millisecondsTimeout == 0)
				{
					this.ExitMyLock();
					return false;
				}
				if (this.readEvent == null)
				{
					this.LazyCreateEvent(ref this.readEvent, false);
				}
				else if (!this.WaitOnEvent(this.readEvent, ref this.numReadWaiters, millisecondsTimeout))
				{
					return false;
				}
			}
			this.owners++;
			this.upgradable_thread = Thread.CurrentThread;
			this.ExitMyLock();
			return true;
		}

		public bool TryEnterUpgradeableReadLock(TimeSpan timeout)
		{
			return this.TryEnterUpgradeableReadLock(ReaderWriterLockSlim.CheckTimeout(timeout));
		}

		public void ExitUpgradeableReadLock()
		{
			this.EnterMyLock();
			this.owners--;
			this.upgradable_thread = null;
			this.ExitAndWakeUpAppropriateWaiters();
		}

		public void Dispose()
		{
			this.read_locks = null;
		}

		public bool IsReadLockHeld
		{
			get
			{
				return this.RecursiveReadCount != 0;
			}
		}

		public bool IsWriteLockHeld
		{
			get
			{
				return this.RecursiveWriteCount != 0;
			}
		}

		public bool IsUpgradeableReadLockHeld
		{
			get
			{
				return this.RecursiveUpgradeCount != 0;
			}
		}

		public int CurrentReadCount
		{
			get
			{
				return this.owners & 268435455;
			}
		}

		public int RecursiveReadCount
		{
			get
			{
				this.EnterMyLock();
				ReaderWriterLockSlim.LockDetails readLockDetails = this.GetReadLockDetails(Thread.CurrentThread.ManagedThreadId, false);
				int num = ((readLockDetails != null) ? readLockDetails.ReadLocks : 0);
				this.ExitMyLock();
				return num;
			}
		}

		public int RecursiveUpgradeCount
		{
			get
			{
				return (this.upgradable_thread != Thread.CurrentThread) ? 0 : 1;
			}
		}

		public int RecursiveWriteCount
		{
			get
			{
				return (this.write_thread != Thread.CurrentThread) ? 0 : 1;
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

		public LockRecursionPolicy RecursionPolicy
		{
			get
			{
				return this.recursionPolicy;
			}
		}

		private void EnterMyLock()
		{
			if (Interlocked.CompareExchange(ref this.myLock, 1, 0) != 0)
			{
				this.EnterMyLockSpin();
			}
		}

		private void EnterMyLockSpin()
		{
			int num = 0;
			for (;;)
			{
				if (num < 3 && ReaderWriterLockSlim.smp)
				{
					Thread.SpinWait(20);
				}
				else
				{
					Thread.Sleep(0);
				}
				if (Interlocked.CompareExchange(ref this.myLock, 1, 0) == 0)
				{
					break;
				}
				num++;
			}
		}

		private void ExitMyLock()
		{
			this.myLock = 0;
		}

		private bool MyLockHeld
		{
			get
			{
				return this.myLock != 0;
			}
		}

		private void ExitAndWakeUpAppropriateWaiters()
		{
			if (this.owners == 1 && this.numUpgradeWaiters != 0U)
			{
				this.ExitMyLock();
				this.upgradeEvent.Set();
			}
			else if (this.owners == 0 && this.numWriteWaiters > 0U)
			{
				this.ExitMyLock();
				this.writeEvent.Set();
			}
			else if (this.owners >= 0 && this.numReadWaiters != 0U)
			{
				this.ExitMyLock();
				this.readEvent.Set();
			}
			else
			{
				this.ExitMyLock();
			}
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
			}
		}

		private bool WaitOnEvent(EventWaitHandle waitEvent, ref uint numWaiters, int millisecondsTimeout)
		{
			waitEvent.Reset();
			numWaiters += 1U;
			bool flag = false;
			this.ExitMyLock();
			try
			{
				flag = waitEvent.WaitOne(millisecondsTimeout, false);
			}
			finally
			{
				this.EnterMyLock();
				numWaiters -= 1U;
				if (!flag)
				{
					this.ExitMyLock();
				}
			}
			return flag;
		}

		private static int CheckTimeout(TimeSpan timeout)
		{
			int num;
			try
			{
				num = checked((int)timeout.TotalMilliseconds);
			}
			catch (OverflowException)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return num;
		}

		private ReaderWriterLockSlim.LockDetails GetReadLockDetails(int threadId, bool create)
		{
			int i;
			ReaderWriterLockSlim.LockDetails lockDetails;
			for (i = 0; i < this.read_locks.Length; i++)
			{
				lockDetails = this.read_locks[i];
				if (lockDetails == null)
				{
					break;
				}
				if (lockDetails.ThreadId == threadId)
				{
					return lockDetails;
				}
			}
			if (!create)
			{
				return null;
			}
			if (i == this.read_locks.Length)
			{
				Array.Resize<ReaderWriterLockSlim.LockDetails>(ref this.read_locks, this.read_locks.Length * 2);
			}
			lockDetails = (this.read_locks[i] = new ReaderWriterLockSlim.LockDetails());
			lockDetails.ThreadId = threadId;
			return lockDetails;
		}

		private static readonly bool smp = Environment.ProcessorCount > 1;

		private int myLock;

		private int owners;

		private Thread upgradable_thread;

		private Thread write_thread;

		private uint numWriteWaiters;

		private uint numReadWaiters;

		private uint numUpgradeWaiters;

		private EventWaitHandle writeEvent;

		private EventWaitHandle readEvent;

		private EventWaitHandle upgradeEvent;

		private readonly LockRecursionPolicy recursionPolicy;

		private ReaderWriterLockSlim.LockDetails[] read_locks = new ReaderWriterLockSlim.LockDetails[8];

		private sealed class LockDetails
		{
			public int ThreadId;

			public int ReadLocks;
		}
	}
}
