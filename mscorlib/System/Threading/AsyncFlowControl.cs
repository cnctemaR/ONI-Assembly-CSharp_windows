using System;
using System.Security;

namespace System.Threading
{
	public struct AsyncFlowControl : IDisposable
	{
		[SecurityCritical]
		internal void Setup()
		{
			this.useEC = true;
			Thread currentThread = Thread.CurrentThread;
			this._ec = currentThread.GetMutableExecutionContext();
			this._ec.isFlowSuppressed = true;
			this._thread = currentThread;
		}

		public void Dispose()
		{
			this.Undo();
		}

		[SecuritySafeCritical]
		public void Undo()
		{
			if (this._thread == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("AsyncFlowControl object can be used only once to call Undo()."));
			}
			if (this._thread != Thread.CurrentThread)
			{
				throw new InvalidOperationException(Environment.GetResourceString("AsyncFlowControl object must be used on the thread where it was created."));
			}
			if (this.useEC)
			{
				if (Thread.CurrentThread.GetMutableExecutionContext() != this._ec)
				{
					throw new InvalidOperationException(Environment.GetResourceString("AsyncFlowControl objects can be used to restore flow only on the Context that had its flow suppressed."));
				}
				ExecutionContext.RestoreFlow();
			}
			this._thread = null;
		}

		public override int GetHashCode()
		{
			if (this._thread != null)
			{
				return this._thread.GetHashCode();
			}
			return this.ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is AsyncFlowControl && this.Equals((AsyncFlowControl)obj);
		}

		public bool Equals(AsyncFlowControl obj)
		{
			return obj.useEC == this.useEC && obj._ec == this._ec && obj._thread == this._thread;
		}

		public static bool operator ==(AsyncFlowControl a, AsyncFlowControl b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(AsyncFlowControl a, AsyncFlowControl b)
		{
			return !(a == b);
		}

		private bool useEC;

		private ExecutionContext _ec;

		private Thread _thread;
	}
}
