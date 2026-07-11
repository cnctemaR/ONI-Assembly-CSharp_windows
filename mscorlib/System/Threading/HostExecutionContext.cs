using System;

namespace System.Threading
{
	[MonoTODO("Useless until the runtime supports it")]
	public class HostExecutionContext
	{
		public HostExecutionContext()
		{
			this._state = null;
		}

		public HostExecutionContext(object state)
		{
			this._state = state;
		}

		public virtual HostExecutionContext CreateCopy()
		{
			return new HostExecutionContext(this._state);
		}

		protected internal object State
		{
			get
			{
				return this._state;
			}
			set
			{
				this._state = value;
			}
		}

		private object _state;
	}
}
