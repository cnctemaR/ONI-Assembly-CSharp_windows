using System;

namespace System.Threading
{
	public readonly struct AsyncLocalValueChangedArgs<T>
	{
		public T PreviousValue { get; }

		public T CurrentValue { get; }

		public bool ThreadContextChanged { get; }

		internal AsyncLocalValueChangedArgs(T previousValue, T currentValue, bool contextChanged)
		{
			this = default(AsyncLocalValueChangedArgs<T>);
			this.PreviousValue = previousValue;
			this.CurrentValue = currentValue;
			this.ThreadContextChanged = contextChanged;
		}
	}
}
