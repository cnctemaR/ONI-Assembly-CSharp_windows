using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Net.Sockets
{
	public class SocketAsyncEventArgs : EventArgs, IDisposable
	{
		public SocketAsyncEventArgs()
		{
			this.AcceptSocket = null;
			this.Buffer = null;
			this.BufferList = null;
			this.BytesTransferred = 0;
			this.Count = 0;
			this.DisconnectReuseSocket = false;
			this.LastOperation = SocketAsyncOperation.None;
			this.Offset = 0;
			this.RemoteEndPoint = null;
			this.SendPacketsElements = null;
			this.SendPacketsFlags = TransmitFileOptions.UseDefaultWorkerThread;
			this.SendPacketsSendSize = -1;
			this.SocketError = SocketError.Success;
			this.SocketFlags = SocketFlags.None;
			this.UserToken = null;
		}

		public event EventHandler<SocketAsyncEventArgs> Completed;

		public Socket AcceptSocket { get; set; }

		public byte[] Buffer { get; private set; }

		[global::System.MonoTODO("not supported in all cases")]
		public IList<ArraySegment<byte>> BufferList
		{
			get
			{
				return this._bufferList;
			}
			set
			{
				if (this.Buffer != null && value != null)
				{
					throw new ArgumentException("Buffer and BufferList properties cannot both be non-null.");
				}
				this._bufferList = value;
			}
		}

		public int BytesTransferred { get; private set; }

		public int Count { get; private set; }

		public bool DisconnectReuseSocket { get; set; }

		public SocketAsyncOperation LastOperation { get; private set; }

		public int Offset { get; private set; }

		public EndPoint RemoteEndPoint { get; set; }

		public IPPacketInformation ReceiveMessageFromPacketInfo { get; private set; }

		public SendPacketsElement[] SendPacketsElements { get; set; }

		public TransmitFileOptions SendPacketsFlags { get; set; }

		[global::System.MonoTODO("unused property")]
		public int SendPacketsSendSize { get; set; }

		public SocketError SocketError { get; set; }

		public SocketFlags SocketFlags { get; set; }

		public object UserToken { get; set; }

		~SocketAsyncEventArgs()
		{
			this.Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			Socket acceptSocket = this.AcceptSocket;
			if (acceptSocket != null)
			{
				acceptSocket.Close();
			}
			if (disposing)
			{
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
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
				completed(e.curSocket, e);
			}
		}

		public void SetBuffer(int offset, int count)
		{
			this.SetBufferInternal(this.Buffer, offset, count);
		}

		public void SetBuffer(byte[] buffer, int offset, int count)
		{
			this.SetBufferInternal(buffer, offset, count);
		}

		private void SetBufferInternal(byte[] buffer, int offset, int count)
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

		private void ReceiveCallback()
		{
			this.SocketError = SocketError.Success;
			this.LastOperation = SocketAsyncOperation.Receive;
			SocketError socketError = SocketError.Success;
			if (!this.curSocket.Connected)
			{
				this.SocketError = SocketError.NotConnected;
				return;
			}
			try
			{
				this.BytesTransferred = this.curSocket.Receive_nochecks(this.Buffer, this.Offset, this.Count, this.SocketFlags, out socketError);
			}
			finally
			{
				this.SocketError = socketError;
				this.OnCompleted(this);
			}
		}

		private void ConnectCallback()
		{
			this.LastOperation = SocketAsyncOperation.Connect;
			SocketError socketError = SocketError.AccessDenied;
			try
			{
				socketError = this.TryConnect(this.RemoteEndPoint);
			}
			finally
			{
				this.SocketError = socketError;
				this.OnCompleted(this);
			}
		}

		private SocketError TryConnect(EndPoint endpoint)
		{
			this.curSocket.Connected = false;
			SocketError socketError = SocketError.Success;
			try
			{
				if (!this.curSocket.Blocking)
				{
					int num;
					this.curSocket.Poll(-1, SelectMode.SelectWrite, out num);
					socketError = (SocketError)num;
					if (num != 0)
					{
						return socketError;
					}
					this.curSocket.Connected = true;
				}
				else
				{
					this.curSocket.seed_endpoint = endpoint;
					this.curSocket.Connect(endpoint);
					this.curSocket.Connected = true;
				}
			}
			catch (SocketException ex)
			{
				socketError = ex.SocketErrorCode;
			}
			return socketError;
		}

		private void SendCallback()
		{
			this.SocketError = SocketError.Success;
			this.LastOperation = SocketAsyncOperation.Send;
			SocketError socketError = SocketError.Success;
			if (!this.curSocket.Connected)
			{
				this.SocketError = SocketError.NotConnected;
				return;
			}
			try
			{
				if (this.Buffer != null)
				{
					this.BytesTransferred = this.curSocket.Send_nochecks(this.Buffer, this.Offset, this.Count, SocketFlags.None, out socketError);
				}
				else if (this.BufferList != null)
				{
					this.BytesTransferred = 0;
					foreach (ArraySegment<byte> arraySegment in this.BufferList)
					{
						this.BytesTransferred += this.curSocket.Send_nochecks(arraySegment.Array, arraySegment.Offset, arraySegment.Count, SocketFlags.None, out socketError);
						if (socketError != SocketError.Success)
						{
							break;
						}
					}
				}
			}
			finally
			{
				this.SocketError = socketError;
				this.OnCompleted(this);
			}
		}

		private void AcceptCallback()
		{
			this.SocketError = SocketError.Success;
			this.LastOperation = SocketAsyncOperation.Accept;
			try
			{
				this.curSocket.Accept(this.AcceptSocket);
			}
			catch (SocketException ex)
			{
				this.SocketError = ex.SocketErrorCode;
				throw;
			}
			finally
			{
				this.OnCompleted(this);
			}
		}

		private void DisconnectCallback()
		{
			this.SocketError = SocketError.Success;
			this.LastOperation = SocketAsyncOperation.Disconnect;
			try
			{
				this.curSocket.Disconnect(this.DisconnectReuseSocket);
			}
			catch (SocketException ex)
			{
				this.SocketError = ex.SocketErrorCode;
				throw;
			}
			finally
			{
				this.OnCompleted(this);
			}
		}

		private void ReceiveFromCallback()
		{
			this.SocketError = SocketError.Success;
			this.LastOperation = SocketAsyncOperation.ReceiveFrom;
			try
			{
				EndPoint remoteEndPoint = this.RemoteEndPoint;
				if (this.Buffer != null)
				{
					this.BytesTransferred = this.curSocket.ReceiveFrom_nochecks(this.Buffer, this.Offset, this.Count, this.SocketFlags, ref remoteEndPoint);
				}
				else if (this.BufferList != null)
				{
					throw new NotImplementedException();
				}
			}
			catch (SocketException ex)
			{
				this.SocketError = ex.SocketErrorCode;
				throw;
			}
			finally
			{
				this.OnCompleted(this);
			}
		}

		private void SendToCallback()
		{
			this.SocketError = SocketError.Success;
			this.LastOperation = SocketAsyncOperation.SendTo;
			int i = 0;
			try
			{
				int count = this.Count;
				while (i < count)
				{
					i += this.curSocket.SendTo_nochecks(this.Buffer, this.Offset, count, this.SocketFlags, this.RemoteEndPoint);
				}
				this.BytesTransferred = i;
			}
			catch (SocketException ex)
			{
				this.SocketError = ex.SocketErrorCode;
				throw;
			}
			finally
			{
				this.OnCompleted(this);
			}
		}

		internal void DoOperation(SocketAsyncOperation operation, Socket socket)
		{
			this.curSocket = socket;
			ThreadStart threadStart;
			switch (operation)
			{
			case SocketAsyncOperation.Accept:
				threadStart = new ThreadStart(this.AcceptCallback);
				goto IL_00BE;
			case SocketAsyncOperation.Connect:
				threadStart = new ThreadStart(this.ConnectCallback);
				goto IL_00BE;
			case SocketAsyncOperation.Disconnect:
				threadStart = new ThreadStart(this.DisconnectCallback);
				goto IL_00BE;
			case SocketAsyncOperation.Receive:
				threadStart = new ThreadStart(this.ReceiveCallback);
				goto IL_00BE;
			case SocketAsyncOperation.ReceiveFrom:
				threadStart = new ThreadStart(this.ReceiveFromCallback);
				goto IL_00BE;
			case SocketAsyncOperation.Send:
				threadStart = new ThreadStart(this.SendCallback);
				goto IL_00BE;
			case SocketAsyncOperation.SendTo:
				threadStart = new ThreadStart(this.SendToCallback);
				goto IL_00BE;
			}
			throw new NotSupportedException();
			IL_00BE:
			new Thread(threadStart)
			{
				IsBackground = true
			}.Start();
		}

		private IList<ArraySegment<byte>> _bufferList;

		private Socket curSocket;
	}
}
