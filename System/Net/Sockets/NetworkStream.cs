using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;

namespace System.Net.Sockets
{
	public class NetworkStream : Stream, IDisposable
	{
		public NetworkStream(Socket socket)
			: this(socket, FileAccess.ReadWrite, false)
		{
		}

		public NetworkStream(Socket socket, bool owns_socket)
			: this(socket, FileAccess.ReadWrite, owns_socket)
		{
		}

		public NetworkStream(Socket socket, FileAccess access)
			: this(socket, access, false)
		{
		}

		public NetworkStream(Socket socket, FileAccess access, bool owns_socket)
		{
			if (socket == null)
			{
				throw new ArgumentNullException("socket is null");
			}
			if (socket.SocketType != SocketType.Stream)
			{
				throw new ArgumentException("Socket is not of type Stream", "socket");
			}
			if (!socket.Connected)
			{
				throw new IOException("Not connected");
			}
			if (!socket.Blocking)
			{
				throw new IOException("Operation not allowed on a non-blocking socket.");
			}
			this.socket = socket;
			this.owns_socket = owns_socket;
			this.access = access;
			this.readable = this.CanRead;
			this.writeable = this.CanWrite;
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override bool CanRead
		{
			get
			{
				return this.access == FileAccess.ReadWrite || this.access == FileAccess.Read;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.access == FileAccess.ReadWrite || this.access == FileAccess.Write;
			}
		}

		public virtual bool DataAvailable
		{
			get
			{
				this.CheckDisposed();
				return this.socket.Available > 0;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		protected bool Readable
		{
			get
			{
				return this.readable;
			}
			set
			{
				this.readable = value;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this.socket.ReceiveTimeout;
			}
			set
			{
				if (value <= 0 && value != -1)
				{
					throw new ArgumentOutOfRangeException("value", "The value specified is less than or equal to zero and is not Infinite.");
				}
				this.socket.ReceiveTimeout = value;
			}
		}

		protected Socket Socket
		{
			get
			{
				return this.socket;
			}
		}

		protected bool Writeable
		{
			get
			{
				return this.writeable;
			}
			set
			{
				this.writeable = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return this.socket.SendTimeout;
			}
			set
			{
				if (value <= 0 && value != -1)
				{
					throw new ArgumentOutOfRangeException("value", "The value specified is less than or equal to zero and is not Infinite");
				}
				this.socket.SendTimeout = value;
			}
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			this.CheckDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer is null");
			}
			int num = buffer.Length;
			if (offset < 0 || offset > num)
			{
				throw new ArgumentOutOfRangeException("offset exceeds the size of buffer");
			}
			if (size < 0 || offset + size > num)
			{
				throw new ArgumentOutOfRangeException("offset+size exceeds the size of buffer");
			}
			IAsyncResult asyncResult;
			try
			{
				asyncResult = this.socket.BeginReceive(buffer, offset, size, SocketFlags.None, callback, state);
			}
			catch (Exception ex)
			{
				throw new IOException("BeginReceive failure", ex);
			}
			return asyncResult;
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			this.CheckDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer is null");
			}
			int num = buffer.Length;
			if (offset < 0 || offset > num)
			{
				throw new ArgumentOutOfRangeException("offset exceeds the size of buffer");
			}
			if (size < 0 || offset + size > num)
			{
				throw new ArgumentOutOfRangeException("offset+size exceeds the size of buffer");
			}
			IAsyncResult asyncResult;
			try
			{
				asyncResult = this.socket.BeginSend(buffer, offset, size, SocketFlags.None, callback, state);
			}
			catch
			{
				throw new IOException("BeginWrite failure");
			}
			return asyncResult;
		}

		~NetworkStream()
		{
			this.Dispose(false);
		}

		public void Close(int timeout)
		{
			if (timeout < -1)
			{
				throw new ArgumentOutOfRangeException("timeout", "timeout is less than -1");
			}
			global::System.Timers.Timer timer = new global::System.Timers.Timer();
			timer.Elapsed += this.OnTimeoutClose;
			timer.Interval = (double)timeout;
			timer.AutoReset = false;
			timer.Enabled = true;
		}

		private void OnTimeoutClose(object source, global::System.Timers.ElapsedEventArgs e)
		{
			this.Close();
		}

		protected override void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			if (this.owns_socket)
			{
				Socket socket = this.socket;
				if (socket != null)
				{
					socket.Close();
				}
			}
			this.socket = null;
			this.access = (FileAccess)0;
		}

		public override int EndRead(IAsyncResult ar)
		{
			this.CheckDisposed();
			if (ar == null)
			{
				throw new ArgumentNullException("async result is null");
			}
			int num;
			try
			{
				num = this.socket.EndReceive(ar);
			}
			catch (Exception ex)
			{
				throw new IOException("EndRead failure", ex);
			}
			return num;
		}

		public override void EndWrite(IAsyncResult ar)
		{
			this.CheckDisposed();
			if (ar == null)
			{
				throw new ArgumentNullException("async result is null");
			}
			try
			{
				this.socket.EndSend(ar);
			}
			catch (Exception ex)
			{
				throw new IOException("EndWrite failure", ex);
			}
		}

		public override void Flush()
		{
		}

		public override int Read([In] [Out] byte[] buffer, int offset, int size)
		{
			this.CheckDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer is null");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset exceeds the size of buffer");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset+size exceeds the size of buffer");
			}
			int num;
			try
			{
				num = this.socket.Receive(buffer, offset, size, SocketFlags.None);
			}
			catch (Exception ex)
			{
				throw new IOException("Read failure", ex);
			}
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			this.CheckDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset exceeds the size of buffer");
			}
			if (size < 0 || size > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("offset+size exceeds the size of buffer");
			}
			try
			{
				int num = 0;
				while (size - num > 0)
				{
					num += this.socket.Send(buffer, offset + num, size - num, SocketFlags.None);
				}
			}
			catch (Exception ex)
			{
				throw new IOException("Write failure", ex);
			}
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		private FileAccess access;

		private Socket socket;

		private bool owns_socket;

		private bool readable;

		private bool writeable;

		private bool disposed;
	}
}
