using System;
using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public struct LockCookie
	{
		internal LockCookie(int thread_id)
		{
			this.ThreadId = thread_id;
			this.ReaderLocks = 0;
			this.WriterLocks = 0;
		}

		internal LockCookie(int thread_id, int reader_locks, int writer_locks)
		{
			this.ThreadId = thread_id;
			this.ReaderLocks = reader_locks;
			this.WriterLocks = writer_locks;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Equals(LockCookie obj)
		{
			return this.ThreadId == obj.ThreadId && this.ReaderLocks == obj.ReaderLocks && this.WriterLocks == obj.WriterLocks;
		}

		public override bool Equals(object obj)
		{
			return obj is LockCookie && obj.Equals(this);
		}

		public static bool operator ==(LockCookie a, LockCookie b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(LockCookie a, LockCookie b)
		{
			return !a.Equals(b);
		}

		internal int ThreadId;

		internal int ReaderLocks;

		internal int WriterLocks;
	}
}
