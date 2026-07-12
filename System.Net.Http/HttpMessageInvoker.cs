using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	public class HttpMessageInvoker : IDisposable
	{
		public HttpMessageInvoker(HttpMessageHandler handler)
			: this(handler, true)
		{
		}

		public HttpMessageInvoker(HttpMessageHandler handler, bool disposeHandler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			this.handler = handler;
			this.disposeHandler = disposeHandler;
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.disposeHandler && this.handler != null)
			{
				this.handler.Dispose();
				this.handler = null;
			}
		}

		public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return this.handler.SendAsync(request, cancellationToken);
		}

		private protected HttpMessageHandler handler;

		private readonly bool disposeHandler;
	}
}
