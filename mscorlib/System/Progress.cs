using System;
using System.Threading;

namespace System
{
	public class Progress<T> : IProgress<T>
	{
		public Progress()
		{
			this._synchronizationContext = SynchronizationContext.Current ?? ProgressStatics.DefaultContext;
			this._invokeHandlers = new SendOrPostCallback(this.InvokeHandlers);
		}

		public Progress(Action<T> handler)
			: this()
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			this._handler = handler;
		}

		public event EventHandler<T> ProgressChanged;

		protected virtual void OnReport(T value)
		{
			bool handler = this._handler != null;
			EventHandler<T> progressChanged = this.ProgressChanged;
			if (handler || progressChanged != null)
			{
				this._synchronizationContext.Post(this._invokeHandlers, value);
			}
		}

		void IProgress<T>.Report(T value)
		{
			this.OnReport(value);
		}

		private void InvokeHandlers(object state)
		{
			T t = (T)((object)state);
			Action<T> handler = this._handler;
			EventHandler<T> progressChanged = this.ProgressChanged;
			if (handler != null)
			{
				handler(t);
			}
			if (progressChanged != null)
			{
				progressChanged(this, t);
			}
		}

		private readonly SynchronizationContext _synchronizationContext;

		private readonly Action<T> _handler;

		private readonly SendOrPostCallback _invokeHandlers;
	}
}
