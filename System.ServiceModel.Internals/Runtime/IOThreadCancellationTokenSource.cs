using System;
using System.Threading;

namespace System.Runtime
{
	internal class IOThreadCancellationTokenSource : IDisposable
	{
		public IOThreadCancellationTokenSource(TimeSpan timeout)
		{
			TimeoutHelper.ThrowIfNegativeArgument(timeout);
			this.timeout = timeout;
		}

		public IOThreadCancellationTokenSource(int timeout)
			: this(TimeSpan.FromMilliseconds((double)timeout))
		{
		}

		public CancellationToken Token
		{
			get
			{
				if (this.token == null)
				{
					if (this.timeout >= TimeoutHelper.MaxWait)
					{
						this.token = new CancellationToken?(CancellationToken.None);
					}
					else
					{
						this.timer = new IOThreadTimer(IOThreadCancellationTokenSource.onCancel, this, true);
						this.source = new CancellationTokenSource();
						this.timer.Set(this.timeout);
						this.token = new CancellationToken?(this.source.Token);
					}
				}
				return this.token.Value;
			}
		}

		public void Dispose()
		{
			if (this.source != null && this.timer.Cancel())
			{
				this.source.Dispose();
				this.source = null;
			}
		}

		private static void OnCancel(object obj)
		{
			((IOThreadCancellationTokenSource)obj).Cancel();
		}

		private void Cancel()
		{
			this.source.Cancel();
			this.source.Dispose();
			this.source = null;
		}

		private static readonly Action<object> onCancel = Fx.ThunkCallback<object>(new Action<object>(IOThreadCancellationTokenSource.OnCancel));

		private readonly TimeSpan timeout;

		private CancellationTokenSource source;

		private CancellationToken? token;

		private IOThreadTimer timer;
	}
}
