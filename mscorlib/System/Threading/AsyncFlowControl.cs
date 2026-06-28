using System;
using System.Security;

namespace System.Threading
{
	public struct AsyncFlowControl : IDisposable
	{
		internal AsyncFlowControl(Thread t, AsyncFlowControlType type)
		{
			this._t = t;
			this._type = type;
		}

		void IDisposable.Dispose()
		{
			if (this._t != null)
			{
				this.Undo();
				this._t = null;
				this._type = AsyncFlowControlType.None;
			}
		}

		public void Undo()
		{
			if (this._t == null)
			{
				throw new InvalidOperationException(Locale.GetText("Can only be called once."));
			}
			AsyncFlowControlType type = this._type;
			if (type != AsyncFlowControlType.Execution)
			{
				if (type == AsyncFlowControlType.Security)
				{
					SecurityContext.RestoreFlow();
				}
			}
			else
			{
				ExecutionContext.RestoreFlow();
			}
			this._t = null;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is AsyncFlowControl && obj.Equals(this);
		}

		public bool Equals(AsyncFlowControl obj)
		{
			return this._t == obj._t && this._type == obj._type;
		}

		public static bool operator ==(AsyncFlowControl a, AsyncFlowControl b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(AsyncFlowControl a, AsyncFlowControl b)
		{
			return !a.Equals(b);
		}

		private Thread _t;

		private AsyncFlowControlType _type;
	}
}
