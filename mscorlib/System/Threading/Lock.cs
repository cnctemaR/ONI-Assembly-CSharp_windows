using System;

namespace System.Threading
{
	public class Lock
	{
		public void Acquire()
		{
			Monitor.Enter(this._lock);
		}

		public void Release()
		{
			Monitor.Exit(this._lock);
		}

		private object _lock = new object();
	}
}
