using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Sockets
{
	internal abstract class MultipleConnectAsync
	{
		public bool StartConnectAsync(SocketAsyncEventArgs args, DnsEndPoint endPoint)
		{
			object lockObject = this._lockObject;
			bool flag2;
			lock (lockObject)
			{
				if (endPoint.AddressFamily != AddressFamily.Unspecified && endPoint.AddressFamily != AddressFamily.InterNetwork && endPoint.AddressFamily != AddressFamily.InterNetworkV6)
				{
					NetEventSource.Fail(this, FormattableStringFactory.Create("Unexpected endpoint address family: {0}", new object[] { endPoint.AddressFamily }), "StartConnectAsync");
				}
				this._userArgs = args;
				this._endPoint = endPoint;
				if (this._state == MultipleConnectAsync.State.Canceled)
				{
					this.SyncFail(new SocketException(995));
					flag2 = false;
				}
				else
				{
					if (this._state != MultipleConnectAsync.State.NotStarted)
					{
						NetEventSource.Fail(this, "MultipleConnectAsync.StartConnectAsync(): Unexpected object state", "StartConnectAsync");
					}
					this._state = MultipleConnectAsync.State.DnsQuery;
					IAsyncResult asyncResult = Dns.BeginGetHostAddresses(endPoint.Host, new AsyncCallback(this.DnsCallback), null);
					if (asyncResult.CompletedSynchronously)
					{
						flag2 = this.DoDnsCallback(asyncResult, true);
					}
					else
					{
						flag2 = true;
					}
				}
			}
			return flag2;
		}

		private void DnsCallback(IAsyncResult result)
		{
			if (!result.CompletedSynchronously)
			{
				this.DoDnsCallback(result, false);
			}
		}

		private bool DoDnsCallback(IAsyncResult result, bool sync)
		{
			Exception ex = null;
			object lockObject = this._lockObject;
			lock (lockObject)
			{
				if (this._state == MultipleConnectAsync.State.Canceled)
				{
					return true;
				}
				if (this._state != MultipleConnectAsync.State.DnsQuery)
				{
					NetEventSource.Fail(this, "MultipleConnectAsync.DoDnsCallback(): Unexpected object state", "DoDnsCallback");
				}
				try
				{
					this._addressList = Dns.EndGetHostAddresses(result);
					if (this._addressList == null)
					{
						NetEventSource.Fail(this, "MultipleConnectAsync.DoDnsCallback(): EndGetHostAddresses returned null!", "DoDnsCallback");
					}
				}
				catch (Exception ex2)
				{
					this._state = MultipleConnectAsync.State.Completed;
					ex = ex2;
				}
				if (ex == null)
				{
					this._state = MultipleConnectAsync.State.ConnectAttempt;
					this._internalArgs = new SocketAsyncEventArgs();
					this._internalArgs.Completed += this.InternalConnectCallback;
					this._internalArgs.CopyBufferFrom(this._userArgs);
					ex = this.AttemptConnection();
					if (ex != null)
					{
						this._state = MultipleConnectAsync.State.Completed;
					}
				}
			}
			return ex == null || this.Fail(sync, ex);
		}

		private void InternalConnectCallback(object sender, SocketAsyncEventArgs args)
		{
			Exception ex = null;
			object lockObject = this._lockObject;
			lock (lockObject)
			{
				if (this._state == MultipleConnectAsync.State.Canceled)
				{
					ex = new SocketException(995);
				}
				else if (args.SocketError == SocketError.Success)
				{
					this._state = MultipleConnectAsync.State.Completed;
				}
				else if (args.SocketError == SocketError.OperationAborted)
				{
					ex = new SocketException(995);
					this._state = MultipleConnectAsync.State.Canceled;
				}
				else
				{
					SocketError socketError = args.SocketError;
					args.in_progress = 0;
					Exception ex2 = this.AttemptConnection();
					if (ex2 == null)
					{
						return;
					}
					SocketException ex3 = ex2 as SocketException;
					if (ex3 != null && ex3.SocketErrorCode == SocketError.NoData)
					{
						ex = new SocketException((int)socketError);
					}
					else
					{
						ex = ex2;
					}
					this._state = MultipleConnectAsync.State.Completed;
				}
			}
			if (ex == null)
			{
				this.Succeed();
				return;
			}
			this.AsyncFail(ex);
		}

		private Exception AttemptConnection()
		{
			Exception ex;
			try
			{
				Socket socket;
				IPAddress nextAddress = this.GetNextAddress(out socket);
				if (nextAddress == null)
				{
					ex = new SocketException(11004);
				}
				else
				{
					this._internalArgs.RemoteEndPoint = new IPEndPoint(nextAddress, this._endPoint.Port);
					ex = this.AttemptConnection(socket, this._internalArgs);
				}
			}
			catch (Exception ex2)
			{
				if (ex2 is ObjectDisposedException)
				{
					NetEventSource.Fail(this, "unexpected ObjectDisposedException", "AttemptConnection");
				}
				ex = ex2;
			}
			return ex;
		}

		private Exception AttemptConnection(Socket attemptSocket, SocketAsyncEventArgs args)
		{
			try
			{
				if (attemptSocket == null)
				{
					NetEventSource.Fail(null, "attemptSocket is null!", "AttemptConnection");
				}
				if (!attemptSocket.ConnectAsync(args))
				{
					this.InternalConnectCallback(null, args);
				}
			}
			catch (ObjectDisposedException)
			{
				return new SocketException(995);
			}
			catch (Exception ex)
			{
				return ex;
			}
			return null;
		}

		protected abstract void OnSucceed();

		private void Succeed()
		{
			this.OnSucceed();
			this._userArgs.FinishWrapperConnectSuccess(this._internalArgs.ConnectSocket, this._internalArgs.BytesTransferred, this._internalArgs.SocketFlags);
			this._internalArgs.Dispose();
		}

		protected abstract void OnFail(bool abortive);

		private bool Fail(bool sync, Exception e)
		{
			if (sync)
			{
				this.SyncFail(e);
				return false;
			}
			this.AsyncFail(e);
			return true;
		}

		private void SyncFail(Exception e)
		{
			this.OnFail(false);
			if (this._internalArgs != null)
			{
				this._internalArgs.Dispose();
			}
			SocketException ex = e as SocketException;
			if (ex != null)
			{
				this._userArgs.FinishConnectByNameSyncFailure(ex, 0, SocketFlags.None);
				return;
			}
			ExceptionDispatchInfo.Throw(e);
		}

		private void AsyncFail(Exception e)
		{
			this.OnFail(false);
			if (this._internalArgs != null)
			{
				this._internalArgs.Dispose();
			}
			this._userArgs.FinishOperationAsyncFailure(e, 0, SocketFlags.None);
		}

		public void Cancel()
		{
			bool flag = false;
			object lockObject = this._lockObject;
			lock (lockObject)
			{
				switch (this._state)
				{
				case MultipleConnectAsync.State.NotStarted:
					flag = true;
					break;
				case MultipleConnectAsync.State.DnsQuery:
					Task.Factory.StartNew(delegate(object s)
					{
						this.CallAsyncFail(s);
					}, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
					flag = true;
					break;
				case MultipleConnectAsync.State.ConnectAttempt:
					flag = true;
					break;
				case MultipleConnectAsync.State.Completed:
					break;
				default:
					NetEventSource.Fail(this, "Unexpected object state", "Cancel");
					break;
				}
				this._state = MultipleConnectAsync.State.Canceled;
			}
			if (flag)
			{
				this.OnFail(true);
			}
		}

		private void CallAsyncFail(object ignored)
		{
			this.AsyncFail(new SocketException(995));
		}

		protected abstract IPAddress GetNextAddress(out Socket attemptSocket);

		protected SocketAsyncEventArgs _userArgs;

		protected SocketAsyncEventArgs _internalArgs;

		protected DnsEndPoint _endPoint;

		protected IPAddress[] _addressList;

		protected int _nextAddress;

		private MultipleConnectAsync.State _state;

		private object _lockObject = new object();

		private enum State
		{
			NotStarted,
			DnsQuery,
			ConnectAttempt,
			Completed,
			Canceled
		}
	}
}
