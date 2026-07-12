using System;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal sealed class DisposableReflectionComposablePart : ReflectionComposablePart, IDisposable
	{
		public DisposableReflectionComposablePart(ReflectionComposablePartDefinition definition)
			: base(definition)
		{
		}

		protected override void ReleaseInstanceIfNecessary(object instance)
		{
			IDisposable disposable = instance as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		protected override void EnsureRunning()
		{
			base.EnsureRunning();
			if (this._isDisposed == 1)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		void IDisposable.Dispose()
		{
			if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
			{
				this.ReleaseInstanceIfNecessary(base.CachedInstance);
			}
		}

		private volatile int _isDisposed;
	}
}
