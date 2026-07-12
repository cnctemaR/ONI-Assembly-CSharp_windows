using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	public abstract class MessageProcessingHandler : DelegatingHandler
	{
		protected MessageProcessingHandler()
		{
		}

		protected MessageProcessingHandler(HttpMessageHandler innerHandler)
			: base(innerHandler)
		{
		}

		protected abstract HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken);

		protected abstract HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken);

		protected internal sealed override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			request = this.ProcessRequest(request, cancellationToken);
			HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
			return this.ProcessResponse(httpResponseMessage, cancellationToken);
		}
	}
}
