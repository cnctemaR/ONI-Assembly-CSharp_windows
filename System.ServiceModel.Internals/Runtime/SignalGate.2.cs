using System;

namespace System.Runtime
{
	internal class SignalGate<T> : SignalGate
	{
		public bool Signal(T result)
		{
			this.result = result;
			return base.Signal();
		}

		public bool Unlock(out T result)
		{
			if (base.Unlock())
			{
				result = this.result;
				return true;
			}
			result = default(T);
			return false;
		}

		private T result;
	}
}
