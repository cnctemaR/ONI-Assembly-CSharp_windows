using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	public abstract class DelegatingHandler : HttpMessageHandler
	{
		protected DelegatingHandler()
		{
		}

		protected DelegatingHandler(HttpMessageHandler innerHandler)
		{
			if (innerHandler == null)
			{
				throw new ArgumentNullException("innerHandler");
			}
			this.InnerHandler = innerHandler;
		}

		public HttpMessageHandler InnerHandler
		{
			get
			{
				return this.handler;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("InnerHandler");
				}
				this.handler = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				if (this.InnerHandler != null)
				{
					this.InnerHandler.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (this.InnerHandler == null)
			{
				throw new InvalidOperationException("The inner handler has not been assigned.");
			}
			return this.InnerHandler.SendAsync(request, cancellationToken);
		}

		private bool disposed;

		private HttpMessageHandler handler;
	}
}
