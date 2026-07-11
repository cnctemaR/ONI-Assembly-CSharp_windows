using System;
using System.Threading;

namespace System.Net.Sockets
{
	internal abstract class MultipleConnectAsync
	{
		public bool StartConnectAsync(SocketAsyncEventArgs args, DnsEndPoint endPoint)
		{
			object obj = this.lockObject;
			bool flag2;
			lock (obj)
			{
				this.userArgs = args;
				this.endPoint = endPoint;
				if (this.state == MultipleConnectAsync.State.Canceled)
				{
					this.SyncFail(new SocketException(SocketError.OperationAborted));
					flag2 = false;
				}
				else
				{
					this.state = MultipleConnectAsync.State.DnsQuery;
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
			object obj = this.lockObject;
			lock (obj)
			{
				if (this.state == MultipleConnectAsync.State.Canceled)
				{
					return true;
				}
				try
				{
					this.addressList = Dns.EndGetHostAddresses(result);
				}
				catch (Exception ex2)
				{
					this.state = MultipleConnectAsync.State.Completed;
					ex = ex2;
				}
				if (ex == null)
				{
					this.state = MultipleConnectAsync.State.ConnectAttempt;
					this.internalArgs = new SocketAsyncEventArgs();
					this.internalArgs.Completed += this.InternalConnectCallback;
					this.internalArgs.SetBuffer(this.userArgs.Buffer, this.userArgs.Offset, this.userArgs.Count);
					ex = this.AttemptConnection();
					if (ex != null)
					{
						this.state = MultipleConnectAsync.State.Completed;
					}
				}
			}
			return ex == null || this.Fail(sync, ex);
		}

		private void InternalConnectCallback(object sender, SocketAsyncEventArgs args)
		{
			Exception ex = null;
			object obj = this.lockObject;
			lock (obj)
			{
				if (this.state == MultipleConnectAsync.State.Canceled)
				{
					ex = new SocketException(SocketError.OperationAborted);
				}
				else if (args.SocketError == SocketError.Success)
				{
					this.state = MultipleConnectAsync.State.Completed;
				}
				else if (args.SocketError == SocketError.OperationAborted)
				{
					ex = new SocketException(SocketError.OperationAborted);
					this.state = MultipleConnectAsync.State.Canceled;
				}
				else
				{
					SocketError socketError = args.SocketError;
					Exception ex2 = this.AttemptConnection();
					if (ex2 == null)
					{
						return;
					}
					SocketException ex3 = ex2 as SocketException;
					if (ex3 != null && ex3.SocketErrorCode == SocketError.NoData)
					{
						ex = new SocketException(socketError);
					}
					else
					{
						ex = ex2;
					}
					this.state = MultipleConnectAsync.State.Completed;
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
			try
			{
				Socket socket = null;
				IPAddress ipaddress = this.GetNextAddress(out socket);
				if (ipaddress == null)
				{
					return new SocketException(SocketError.NoData);
				}
				this.internalArgs.RemoteEndPoint = new IPEndPoint(ipaddress, this.endPoint.Port);
				if (!socket.ConnectAsync(this.internalArgs))
				{
					return new SocketException(this.internalArgs.SocketError);
				}
			}
			catch (ObjectDisposedException)
			{
				return new SocketException(SocketError.OperationAborted);
			}
			catch (Exception ex)
			{
				return ex;
			}
			return null;
		}

		protected abstract void OnSucceed();

		protected void Succeed()
		{
			this.OnSucceed();
			this.userArgs.FinishWrapperConnectSuccess(this.internalArgs.ConnectSocket, this.internalArgs.BytesTransferred, this.internalArgs.SocketFlags);
			this.internalArgs.Dispose();
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
			if (this.internalArgs != null)
			{
				this.internalArgs.Dispose();
			}
			SocketException ex = e as SocketException;
			if (ex != null)
			{
				this.userArgs.FinishConnectByNameSyncFailure(ex, 0, SocketFlags.None);
				return;
			}
			throw e;
		}

		private void AsyncFail(Exception e)
		{
			this.OnFail(false);
			if (this.internalArgs != null)
			{
				this.internalArgs.Dispose();
			}
			this.userArgs.FinishOperationAsyncFailure(e, 0, SocketFlags.None);
		}

		public void Cancel()
		{
			bool flag = false;
			object obj = this.lockObject;
			lock (obj)
			{
				switch (this.state)
				{
				case MultipleConnectAsync.State.NotStarted:
					flag = true;
					break;
				case MultipleConnectAsync.State.DnsQuery:
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.CallAsyncFail));
					flag = true;
					break;
				case MultipleConnectAsync.State.ConnectAttempt:
					flag = true;
					break;
				}
				this.state = MultipleConnectAsync.State.Canceled;
			}
			if (flag)
			{
				this.OnFail(true);
			}
		}

		private void CallAsyncFail(object ignored)
		{
			this.AsyncFail(new SocketException(SocketError.OperationAborted));
		}

		protected abstract IPAddress GetNextAddress(out Socket attemptSocket);

		protected SocketAsyncEventArgs userArgs;

		protected SocketAsyncEventArgs internalArgs;

		protected DnsEndPoint endPoint;

		protected IPAddress[] addressList;

		protected int nextAddress;

		private MultipleConnectAsync.State state;

		private object lockObject = new object();

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
