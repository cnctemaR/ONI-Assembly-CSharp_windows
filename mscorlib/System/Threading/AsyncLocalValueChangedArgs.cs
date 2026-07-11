using System;

namespace System.Threading
{
	public struct AsyncLocalValueChangedArgs<T>
	{
		public T PreviousValue { get; private set; }

		public T CurrentValue { get; private set; }

		public bool ThreadContextChanged { get; private set; }

		internal AsyncLocalValueChangedArgs(T previousValue, T currentValue, bool contextChanged)
		{
			this = default(AsyncLocalValueChangedArgs<T>);
			this.PreviousValue = previousValue;
			this.CurrentValue = currentValue;
			this.ThreadContextChanged = contextChanged;
		}
	}
}
