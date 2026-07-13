using System;

namespace Unity.Scripting.LifecycleManagement
{
	internal sealed class ScopedLazy<TValue, TScope> where TValue : class
	{
		public ScopedLazy(Func<TValue> factory, bool checkScopeActive = true)
		{
			this._data = new Lazy<TValue>(factory);
		}

		public ScopedLazy(bool checkScopeActive = true)
			: this(new Func<TValue>(Activator.CreateInstance<TValue>), checkScopeActive)
		{
		}

		public void Cleanup()
		{
			this._data = null;
		}

		public TValue Value
		{
			get
			{
				return this._data.Value;
			}
		}

		private Lazy<TValue> _data;
	}
}
