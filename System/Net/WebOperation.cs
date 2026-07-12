using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class WebOperation
	{
		public HttpWebRequest Request { get; }

		public WebConnection Connection { get; private set; }

		public ServicePoint ServicePoint { get; private set; }

		public BufferOffsetSize WriteBuffer { get; }

		public bool IsNtlmChallenge { get; }

		public WebOperation(HttpWebRequest request, BufferOffsetSize writeBuffer, bool isNtlmChallenge, CancellationToken cancellationToken)
		{
			this.Request = request;
			this.WriteBuffer = writeBuffer;
			this.IsNtlmChallenge = isNtlmChallenge;
			this.cts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken[] { cancellationToken });
			this.requestTask = new TaskCompletionSource<WebRequestStream>();
			this.requestWrittenTask = new TaskCompletionSource<WebRequestStream>();
			this.completeResponseReadTask = new TaskCompletionSource<bool>();
			this.responseTask = new TaskCompletionSource<WebResponseStream>();
			this.finishedTask = new TaskCompletionSource<ValueTuple<bool, WebOperation>>();
		}

		public bool Aborted
		{
			get
			{
				return this.disposedInfo != null || this.Request.Aborted || (this.cts != null && this.cts.IsCancellationRequested);
			}
		}

		public bool Closed
		{
			get
			{
				return this.Aborted || this.closedInfo != null;
			}
		}

		public void Abort()
		{
			if (!this.SetDisposed(ref this.disposedInfo).Item2)
			{
				return;
			}
			CancellationTokenSource cancellationTokenSource = this.cts;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			this.SetCanceled();
			this.Close();
		}

		public void Close()
		{
			if (!this.SetDisposed(ref this.closedInfo).Item2)
			{
				return;
			}
			WebRequestStream webRequestStream = Interlocked.Exchange<WebRequestStream>(ref this.writeStream, null);
			if (webRequestStream != null)
			{
				try
				{
					webRequestStream.Close();
				}
				catch
				{
				}
			}
		}

		private void SetCanceled()
		{
			this.requestTask.TrySetCanceled();
			this.requestWrittenTask.TrySetCanceled();
			this.responseTask.TrySetCanceled();
			this.completeResponseReadTask.TrySetCanceled();
		}

		private void SetError(Exception error)
		{
			this.requestTask.TrySetException(error);
			this.requestWrittenTask.TrySetException(error);
			this.responseTask.TrySetException(error);
			this.completeResponseReadTask.TrySetException(error);
		}

		private ValueTuple<ExceptionDispatchInfo, bool> SetDisposed(ref ExceptionDispatchInfo field)
		{
			ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(new WebException(global::SR.GetString("The request was canceled"), WebExceptionStatus.RequestCanceled));
			ExceptionDispatchInfo exceptionDispatchInfo2 = Interlocked.CompareExchange<ExceptionDispatchInfo>(ref field, exceptionDispatchInfo, null);
			return new ValueTuple<ExceptionDispatchInfo, bool>(exceptionDispatchInfo2 ?? exceptionDispatchInfo, exceptionDispatchInfo2 == null);
		}

		internal void ThrowIfDisposed()
		{
			this.ThrowIfDisposed(CancellationToken.None);
		}

		internal void ThrowIfDisposed(CancellationToken cancellationToken)
		{
			if (this.Aborted || cancellationToken.IsCancellationRequested)
			{
				this.ThrowDisposed(ref this.disposedInfo);
			}
		}

		internal void ThrowIfClosedOrDisposed()
		{
			this.ThrowIfClosedOrDisposed(CancellationToken.None);
		}

		internal void ThrowIfClosedOrDisposed(CancellationToken cancellationToken)
		{
			if (this.Closed || cancellationToken.IsCancellationRequested)
			{
				this.ThrowDisposed(ref this.closedInfo);
			}
		}

		private void ThrowDisposed(ref ExceptionDispatchInfo field)
		{
			ValueTuple<ExceptionDispatchInfo, bool> valueTuple = this.SetDisposed(ref field);
			ExceptionDispatchInfo item = valueTuple.Item1;
			if (valueTuple.Item2)
			{
				CancellationTokenSource cancellationTokenSource = this.cts;
				if (cancellationTokenSource != null)
				{
					cancellationTokenSource.Cancel();
				}
			}
			item.Throw();
		}

		internal void RegisterRequest(ServicePoint servicePoint, WebConnection connection)
		{
			if (servicePoint == null)
			{
				throw new ArgumentNullException("servicePoint");
			}
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			lock (this)
			{
				if (Interlocked.CompareExchange(ref this.requestSent, 1, 0) != 0)
				{
					throw new InvalidOperationException("Invalid nested call.");
				}
				this.ServicePoint = servicePoint;
				this.Connection = connection;
			}
			this.cts.Token.Register(delegate
			{
				this.Request.FinishedReading = true;
				this.SetDisposed(ref this.disposedInfo);
			});
		}

		public void SetPriorityRequest(WebOperation operation)
		{
			lock (this)
			{
				if (this.requestSent != 1 || this.ServicePoint == null || this.finishedReading)
				{
					throw new InvalidOperationException("Should never happen.");
				}
				if (Interlocked.CompareExchange<WebOperation>(ref this.priorityRequest, operation, null) != null)
				{
					throw new InvalidOperationException("Invalid nested request.");
				}
			}
		}

		public Task<WebRequestStream> GetRequestStream()
		{
			return this.requestTask.Task;
		}

		public Task WaitUntilRequestWritten()
		{
			return this.requestWrittenTask.Task;
		}

		public WebRequestStream WriteStream
		{
			get
			{
				this.ThrowIfDisposed();
				return this.writeStream;
			}
		}

		public Task<WebResponseStream> GetResponseStream()
		{
			return this.responseTask.Task;
		}

		internal async Task<ValueTuple<bool, WebOperation>> WaitForCompletion(bool ignoreErrors)
		{
			ValueTuple<bool, WebOperation> valueTuple;
			try
			{
				valueTuple = await this.finishedTask.Task.ConfigureAwait(false);
			}
			catch
			{
				if (!ignoreErrors)
				{
					throw;
				}
				valueTuple = new ValueTuple<bool, WebOperation>(false, null);
			}
			return valueTuple;
		}

		internal async void Run()
		{
			try
			{
				this.FinishReading();
				this.ThrowIfClosedOrDisposed();
				WebRequestStream webRequestStream = await this.Connection.InitConnection(this, this.cts.Token).ConfigureAwait(false);
				WebRequestStream requestStream = webRequestStream;
				this.ThrowIfClosedOrDisposed();
				this.writeStream = requestStream;
				await requestStream.Initialize(this.cts.Token).ConfigureAwait(false);
				this.ThrowIfClosedOrDisposed();
				this.requestTask.TrySetResult(requestStream);
				WebResponseStream stream = new WebResponseStream(requestStream);
				this.responseStream = stream;
				await stream.InitReadAsync(this.cts.Token).ConfigureAwait(false);
				this.responseTask.TrySetResult(stream);
				requestStream = null;
				stream = null;
			}
			catch (OperationCanceledException)
			{
				this.SetCanceled();
			}
			catch (Exception ex)
			{
				this.SetError(ex);
			}
		}

		private async void FinishReading()
		{
			bool ok = false;
			Exception error = null;
			try
			{
				bool flag = await this.completeResponseReadTask.Task.ConfigureAwait(false);
				ok = flag;
			}
			catch (Exception error)
			{
			}
			WebResponseStream webResponseStream;
			WebOperation webOperation;
			lock (this)
			{
				this.finishedReading = true;
				webResponseStream = Interlocked.Exchange<WebResponseStream>(ref this.responseStream, null);
				webOperation = Interlocked.Exchange<WebOperation>(ref this.priorityRequest, null);
				this.Request.FinishedReading = true;
			}
			if (error != null)
			{
				if (webOperation != null)
				{
					webOperation.SetError(error);
				}
				this.finishedTask.TrySetException(error);
			}
			else
			{
				bool flag2 = !this.Aborted && ok && webResponseStream != null && webResponseStream.KeepAlive;
				if (webOperation != null && webOperation.Aborted)
				{
					webOperation = null;
					flag2 = false;
				}
				this.finishedTask.TrySetResult(new ValueTuple<bool, WebOperation>(flag2, webOperation));
			}
		}

		internal void CompleteRequestWritten(WebRequestStream stream, Exception error = null)
		{
			if (error != null)
			{
				this.SetError(error);
				return;
			}
			this.requestWrittenTask.TrySetResult(stream);
		}

		internal void CompleteResponseRead(bool ok, Exception error = null)
		{
			if (error != null)
			{
				this.completeResponseReadTask.TrySetException(error);
				return;
			}
			this.completeResponseReadTask.TrySetResult(ok);
		}

		internal readonly int ID;

		private CancellationTokenSource cts;

		private TaskCompletionSource<WebRequestStream> requestTask;

		private TaskCompletionSource<WebRequestStream> requestWrittenTask;

		private TaskCompletionSource<WebResponseStream> responseTask;

		private TaskCompletionSource<bool> completeResponseReadTask;

		private TaskCompletionSource<ValueTuple<bool, WebOperation>> finishedTask;

		private WebRequestStream writeStream;

		private WebResponseStream responseStream;

		private ExceptionDispatchInfo disposedInfo;

		private ExceptionDispatchInfo closedInfo;

		private WebOperation priorityRequest;

		private volatile bool finishedReading;

		private int requestSent;
	}
}
