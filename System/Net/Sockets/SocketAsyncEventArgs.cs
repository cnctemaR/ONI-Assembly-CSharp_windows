using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Net.Sockets
{
	public class SocketAsyncEventArgs : EventArgs, IDisposable
	{
		public Exception ConnectByNameError { get; internal set; }

		public Socket AcceptSocket { get; set; }

		public byte[] Buffer { get; private set; }

		public IList<ArraySegment<byte>> BufferList
		{
			get
			{
				return this.m_BufferList;
			}
			set
			{
				if (this.Buffer != null && value != null)
				{
					throw new ArgumentException("Buffer and BufferList properties cannot both be non-null.");
				}
				this.m_BufferList = value;
			}
		}

		public int BytesTransferred { get; internal set; }

		public int Count { get; internal set; }

		public bool DisconnectReuseSocket { get; set; }

		public SocketAsyncOperation LastOperation { get; private set; }

		public int Offset { get; private set; }

		public EndPoint RemoteEndPoint
		{
			get
			{
				return this.remote_ep;
			}
			set
			{
				this.remote_ep = value;
			}
		}

		public IPPacketInformation ReceiveMessageFromPacketInfo { get; private set; }

		public SendPacketsElement[] SendPacketsElements { get; set; }

		public TransmitFileOptions SendPacketsFlags { get; set; }

		[MonoTODO("unused property")]
		public int SendPacketsSendSize { get; set; }

		public SocketError SocketError { get; set; }

		public SocketFlags SocketFlags { get; set; }

		public object UserToken { get; set; }

		public Socket ConnectSocket
		{
			get
			{
				SocketError socketError = this.SocketError;
				if (socketError == SocketError.AccessDenied)
				{
					return null;
				}
				return this.current_socket;
			}
		}

		internal bool PolicyRestricted { get; private set; }

		public event EventHandler<SocketAsyncEventArgs> Completed;

		internal SocketAsyncEventArgs(bool policy)
			: this()
		{
			this.PolicyRestricted = policy;
		}

		public SocketAsyncEventArgs()
		{
			this.SendPacketsSendSize = -1;
		}

		~SocketAsyncEventArgs()
		{
			this.Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			this.disposed = true;
			if (disposing)
			{
				int num = this.in_progress;
				return;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void SetLastOperation(SocketAsyncOperation op)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("System.Net.Sockets.SocketAsyncEventArgs");
			}
			if (Interlocked.Exchange(ref this.in_progress, 1) != 0)
			{
				throw new InvalidOperationException("Operation already in progress");
			}
			this.LastOperation = op;
		}

		internal void Complete()
		{
			this.OnCompleted(this);
		}

		protected virtual void OnCompleted(SocketAsyncEventArgs e)
		{
			if (e == null)
			{
				return;
			}
			EventHandler<SocketAsyncEventArgs> completed = e.Completed;
			if (completed != null)
			{
				completed(e.current_socket, e);
			}
		}

		public void SetBuffer(int offset, int count)
		{
			this.SetBuffer(this.Buffer, offset, count);
		}

		public void SetBuffer(byte[] buffer, int offset, int count)
		{
			if (buffer != null)
			{
				if (this.BufferList != null)
				{
					throw new ArgumentException("Buffer and BufferList properties cannot both be non-null.");
				}
				int num = buffer.Length;
				if (offset < 0 || (offset != 0 && offset >= num))
				{
					throw new ArgumentOutOfRangeException("offset");
				}
				if (count < 0 || count > num - offset)
				{
					throw new ArgumentOutOfRangeException("count");
				}
				this.Count = count;
				this.Offset = offset;
			}
			this.Buffer = buffer;
		}

		internal void StartOperationCommon(Socket socket)
		{
			this.current_socket = socket;
		}

		internal void StartOperationWrapperConnect(MultipleConnectAsync args)
		{
			this.SetLastOperation(SocketAsyncOperation.Connect);
		}

		internal void FinishConnectByNameSyncFailure(Exception exception, int bytesTransferred, SocketFlags flags)
		{
			throw new NotImplementedException();
		}

		internal void FinishOperationAsyncFailure(Exception exception, int bytesTransferred, SocketFlags flags)
		{
			throw new NotImplementedException();
		}

		internal void FinishWrapperConnectSuccess(Socket connectSocket, int bytesTransferred, SocketFlags flags)
		{
			this.SetResults(SocketError.Success, bytesTransferred, flags);
			this.current_socket = connectSocket;
			this.OnCompleted(this);
		}

		internal void SetResults(SocketError socketError, int bytesTransferred, SocketFlags flags)
		{
			this.SocketError = socketError;
			this.BytesTransferred = bytesTransferred;
			this.SocketFlags = flags;
		}

		private bool disposed;

		internal volatile int in_progress;

		internal EndPoint remote_ep;

		internal Socket current_socket;

		internal SocketAsyncResult socket_async_result = new SocketAsyncResult();

		internal IList<ArraySegment<byte>> m_BufferList;
	}
}
