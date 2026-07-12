using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	public class HttpClient : HttpMessageInvoker
	{
		public HttpClient()
			: this(new HttpClientHandler(), true)
		{
		}

		public HttpClient(HttpMessageHandler handler)
			: this(handler, true)
		{
		}

		public HttpClient(HttpMessageHandler handler, bool disposeHandler)
			: base(handler, disposeHandler)
		{
			this.buffer_size = 2147483647L;
			this.timeout = HttpClient.TimeoutDefault;
			this.cts = new CancellationTokenSource();
		}

		public Uri BaseAddress
		{
			get
			{
				return this.base_address;
			}
			set
			{
				this.base_address = value;
			}
		}

		public HttpRequestHeaders DefaultRequestHeaders
		{
			get
			{
				HttpRequestHeaders httpRequestHeaders;
				if ((httpRequestHeaders = this.headers) == null)
				{
					httpRequestHeaders = (this.headers = new HttpRequestHeaders());
				}
				return httpRequestHeaders;
			}
		}

		public long MaxResponseContentBufferSize
		{
			get
			{
				return this.buffer_size;
			}
			set
			{
				if (value <= 0L)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.buffer_size = value;
			}
		}

		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				if (value != global::System.Threading.Timeout.InfiniteTimeSpan && (value <= TimeSpan.Zero || value.TotalMilliseconds > 2147483647.0))
				{
					throw new ArgumentOutOfRangeException();
				}
				this.timeout = value;
			}
		}

		public void CancelPendingRequests()
		{
			using (CancellationTokenSource cancellationTokenSource = Interlocked.Exchange<CancellationTokenSource>(ref this.cts, new CancellationTokenSource()))
			{
				cancellationTokenSource.Cancel();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				this.cts.Cancel();
				this.cts.Dispose();
			}
			base.Dispose(disposing);
		}

		public Task<HttpResponseMessage> DeleteAsync(string requestUri)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri));
		}

		public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
		}

		public Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri));
		}

		public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
		}

		public Task<HttpResponseMessage> GetAsync(string requestUri)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri));
		}

		public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
		}

		public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption);
		}

		public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
		}

		public Task<HttpResponseMessage> GetAsync(Uri requestUri)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri));
		}

		public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
		}

		public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption);
		}

		public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
		}

		public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
			{
				Content = content
			});
		}

		public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
			{
				Content = content
			}, cancellationToken);
		}

		public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
			{
				Content = content
			});
		}

		public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
			{
				Content = content
			}, cancellationToken);
		}

		public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
			{
				Content = content
			});
		}

		public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
			{
				Content = content
			}, cancellationToken);
		}

		public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
			{
				Content = content
			});
		}

		public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
			{
				Content = content
			}, cancellationToken);
		}

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
		{
			return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
		}

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
		{
			return this.SendAsync(request, completionOption, CancellationToken.None);
		}

		public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
		}

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (request.SetIsUsed())
			{
				throw new InvalidOperationException("Cannot send the same request message multiple times");
			}
			Uri requestUri = request.RequestUri;
			if (requestUri == null)
			{
				if (this.base_address == null)
				{
					throw new InvalidOperationException("The request URI must either be an absolute URI or BaseAddress must be set");
				}
				request.RequestUri = this.base_address;
			}
			else if (!requestUri.IsAbsoluteUri || (requestUri.Scheme == Uri.UriSchemeFile && requestUri.OriginalString.StartsWith("/", StringComparison.Ordinal)))
			{
				if (this.base_address == null)
				{
					throw new InvalidOperationException("The request URI must either be an absolute URI or BaseAddress must be set");
				}
				request.RequestUri = new Uri(this.base_address, requestUri);
			}
			if (this.headers != null)
			{
				request.Headers.AddHeaders(this.headers);
			}
			return this.SendAsyncWorker(request, completionOption, cancellationToken);
		}

		private async Task<HttpResponseMessage> SendAsyncWorker(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
		{
			HttpResponseMessage httpResponseMessage2;
			using (CancellationTokenSource lcts = CancellationTokenSource.CreateLinkedTokenSource(this.cts.Token, cancellationToken))
			{
				HttpClientHandler httpClientHandler = this.handler as HttpClientHandler;
				if (httpClientHandler != null)
				{
					httpClientHandler.SetWebRequestTimeout(this.timeout);
				}
				lcts.CancelAfter(this.timeout);
				Task<HttpResponseMessage> task = base.SendAsync(request, lcts.Token);
				if (task == null)
				{
					throw new InvalidOperationException("Handler failed to return a value");
				}
				HttpResponseMessage httpResponseMessage = await task.ConfigureAwait(false);
				HttpResponseMessage response = httpResponseMessage;
				if (response == null)
				{
					throw new InvalidOperationException("Handler failed to return a response");
				}
				if (response.Content != null && (completionOption & HttpCompletionOption.ResponseHeadersRead) == HttpCompletionOption.ResponseContentRead)
				{
					await response.Content.LoadIntoBufferAsync(this.MaxResponseContentBufferSize).ConfigureAwait(false);
				}
				httpResponseMessage2 = response;
			}
			return httpResponseMessage2;
		}

		public async Task<byte[]> GetByteArrayAsync(string requestUri)
		{
			HttpResponseMessage httpResponseMessage = await this.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
			byte[] array;
			using (HttpResponseMessage resp = httpResponseMessage)
			{
				resp.EnsureSuccessStatusCode();
				array = await resp.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
			}
			return array;
		}

		public async Task<byte[]> GetByteArrayAsync(Uri requestUri)
		{
			HttpResponseMessage httpResponseMessage = await this.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
			byte[] array;
			using (HttpResponseMessage resp = httpResponseMessage)
			{
				resp.EnsureSuccessStatusCode();
				array = await resp.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
			}
			return array;
		}

		public async Task<Stream> GetStreamAsync(string requestUri)
		{
			object obj = await this.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
			obj.EnsureSuccessStatusCode();
			return await obj.Content.ReadAsStreamAsync().ConfigureAwait(false);
		}

		public async Task<Stream> GetStreamAsync(Uri requestUri)
		{
			object obj = await this.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
			obj.EnsureSuccessStatusCode();
			return await obj.Content.ReadAsStreamAsync().ConfigureAwait(false);
		}

		public async Task<string> GetStringAsync(string requestUri)
		{
			HttpResponseMessage httpResponseMessage = await this.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
			string text;
			using (HttpResponseMessage resp = httpResponseMessage)
			{
				resp.EnsureSuccessStatusCode();
				text = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			return text;
		}

		public async Task<string> GetStringAsync(Uri requestUri)
		{
			HttpResponseMessage httpResponseMessage = await this.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
			string text;
			using (HttpResponseMessage resp = httpResponseMessage)
			{
				resp.EnsureSuccessStatusCode();
				text = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			return text;
		}

		public Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content)
		{
			throw new PlatformNotSupportedException();
		}

		public Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			throw new PlatformNotSupportedException();
		}

		public Task<HttpResponseMessage> PatchAsync(Uri requestUri, HttpContent content)
		{
			throw new PlatformNotSupportedException();
		}

		public Task<HttpResponseMessage> PatchAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			throw new PlatformNotSupportedException();
		}

		private static readonly TimeSpan TimeoutDefault = TimeSpan.FromSeconds(100.0);

		private Uri base_address;

		private CancellationTokenSource cts;

		private bool disposed;

		private HttpRequestHeaders headers;

		private long buffer_size;

		private TimeSpan timeout;
	}
}
