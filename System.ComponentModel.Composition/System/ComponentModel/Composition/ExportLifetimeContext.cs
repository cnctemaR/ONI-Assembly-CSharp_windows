using System;

namespace System.ComponentModel.Composition
{
	public sealed class ExportLifetimeContext<T> : IDisposable
	{
		public ExportLifetimeContext(T value, Action disposeAction)
		{
			this._value = value;
			this._disposeAction = disposeAction;
		}

		public T Value
		{
			get
			{
				return this._value;
			}
		}

		public void Dispose()
		{
			if (this._disposeAction != null)
			{
				this._disposeAction();
			}
		}

		private readonly T _value;

		private readonly Action _disposeAction;
	}
}
