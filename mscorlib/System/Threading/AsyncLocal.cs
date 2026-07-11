using System;
using System.Security;

namespace System.Threading
{
	public sealed class AsyncLocal<T> : IAsyncLocal
	{
		public AsyncLocal()
		{
		}

		[SecurityCritical]
		public AsyncLocal(Action<AsyncLocalValueChangedArgs<T>> valueChangedHandler)
		{
			this.m_valueChangedHandler = valueChangedHandler;
		}

		public T Value
		{
			[SecuritySafeCritical]
			get
			{
				object localValue = ExecutionContext.GetLocalValue(this);
				if (localValue != null)
				{
					return (T)((object)localValue);
				}
				return default(T);
			}
			[SecuritySafeCritical]
			set
			{
				ExecutionContext.SetLocalValue(this, value, this.m_valueChangedHandler != null);
			}
		}

		[SecurityCritical]
		void IAsyncLocal.OnValueChanged(object previousValueObj, object currentValueObj, bool contextChanged)
		{
			T t = ((previousValueObj == null) ? default(T) : ((T)((object)previousValueObj)));
			T t2 = ((currentValueObj == null) ? default(T) : ((T)((object)currentValueObj)));
			this.m_valueChangedHandler(new AsyncLocalValueChangedArgs<T>(t, t2, contextChanged));
		}

		[SecurityCritical]
		private readonly Action<AsyncLocalValueChangedArgs<T>> m_valueChangedHandler;
	}
}
