using System;
using System.Threading;

namespace System
{
	internal sealed class LazyDebugView<T>
	{
		public LazyDebugView(Lazy<T> lazy)
		{
			this._lazy = lazy;
		}

		public bool IsValueCreated
		{
			get
			{
				return this._lazy.IsValueCreated;
			}
		}

		public T Value
		{
			get
			{
				return this._lazy.ValueForDebugDisplay;
			}
		}

		public LazyThreadSafetyMode? Mode
		{
			get
			{
				return this._lazy.Mode;
			}
		}

		public bool IsValueFaulted
		{
			get
			{
				return this._lazy.IsValueFaulted;
			}
		}

		private readonly Lazy<T> _lazy;
	}
}
