using System;
using System.Runtime.InteropServices;

namespace System.IO.Ports
{
	internal class SerialPortStream : Stream, ISerialStream, IDisposable
	{
		[DllImport("MonoPosixHelper", SetLastError = true)]
		private static extern int open_serial(string portName);

		public SerialPortStream(string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits, bool dtrEnable, bool rtsEnable, Handshake handshake, int readTimeout, int writeTimeout, int readBufferSize, int writeBufferSize)
		{
			this.fd = SerialPortStream.open_serial(portName);
			if (this.fd == -1)
			{
				SerialPortStream.ThrowIOException();
			}
			this.TryBaudRate(baudRate);
			if (!SerialPortStream.set_attributes(this.fd, baudRate, parity, dataBits, stopBits, handshake))
			{
				SerialPortStream.ThrowIOException();
			}
			this.read_timeout = readTimeout;
			this.write_timeout = writeTimeout;
			this.SetSignal(SerialSignal.Dtr, dtrEnable);
			if (handshake != Handshake.RequestToSend && handshake != Handshake.RequestToSendXOnXOff)
			{
				this.SetSignal(SerialSignal.Rts, rtsEnable);
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return true;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this.read_timeout;
			}
			set
			{
				if (value < 0 && value != -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.read_timeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return this.write_timeout;
			}
			set
			{
				if (value < 0 && value != -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.write_timeout = value;
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

		public override void Flush()
		{
		}

		[DllImport("MonoPosixHelper", SetLastError = true)]
		private static extern int read_serial(int fd, byte[] buffer, int offset, int count);

		[DllImport("MonoPosixHelper", SetLastError = true)]
		private static extern bool poll_serial(int fd, out int error, int timeout);

		public override int Read([In] [Out] byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException("offset or count less than zero.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			int num;
			bool flag = SerialPortStream.poll_serial(this.fd, out num, this.read_timeout);
			if (num == -1)
			{
				SerialPortStream.ThrowIOException();
			}
			if (!flag)
			{
				throw new TimeoutException();
			}
			int num2 = SerialPortStream.read_serial(this.fd, buffer, offset, count);
			if (num2 == -1)
			{
				SerialPortStream.ThrowIOException();
			}
			return num2;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		[DllImport("MonoPosixHelper", SetLastError = true)]
		private static extern int write_serial(int fd, byte[] buffer, int offset, int count, int timeout);

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			if (SerialPortStream.write_serial(this.fd, buffer, offset, count, this.write_timeout) < 0)
			{
				throw new TimeoutException("The operation has timed-out");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			if (SerialPortStream.close_serial(this.fd) != 0)
			{
				SerialPortStream.ThrowIOException();
			}
		}

		[DllImport("MonoPosixHelper", SetLastError = true)]
		private static extern int close_serial(int fd);

		public override void Close()
		{
			((IDisposable)this).Dispose();
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SerialPortStream()
		{
			try
			{
				this.Dispose(false);
			}
			catch (IOException)
			{
			}
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		[DllImport("MonoPosixHelper", SetLastError = true)]
		private static extern bool set_attributes(int fd, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake);

		public void SetAttributes(int baud_rate, Parity parity, int data_bits, StopBits sb, Handshake hs)
		{
			if (!SerialPortStream.set_attributes(this.fd, baud_rate, parity, data_bits, sb, hs))
			{
				SerialPortStream.ThrowIOException();
			}
		}

		[DllImport("MonoPosixHelper", SetLastError = true)]
		private static extern int get_bytes_in_buffer(int fd, int input);

		public int BytesToRead
		{
			get
			{
				int num = SerialPortStream.get_bytes_in_buffer(this.fd, 1);
				if (num == -1)
				{
					SerialPortStream.ThrowIOException();
				}
				return num;
			}
		}

		public int BytesToWrite
		{
			get
			{
				int num = SerialPortStream.get_bytes_in_buffer(this.fd, 0);
				if (num == -1)
				{
					SerialPortStream.ThrowIOException();
				}
				return num;
			}
		}

		[DllImport("MonoPosixHelper", SetLastError = true)]
		private static extern int discard_buffer(int fd, bool inputBuffer);

		public void DiscardInBuffer()
		{
			if (SerialPortStream.discard_buffer(this.fd, true) != 0)
			{
				SerialPortStream.ThrowIOException();
			}
		}

		public void DiscardOutBuffer()
		{
			if (SerialPortStream.discard_buffer(this.fd, false) != 0)
			{
				SerialPortStream.ThrowIOException();
			}
		}

		[DllImport("MonoPosixHelper", SetLastError = true)]
		private static extern SerialSignal get_signals(int fd, out int error);

		public SerialSignal GetSignals()
		{
			int num;
			SerialSignal serialSignal = SerialPortStream.get_signals(this.fd, out num);
			if (num == -1)
			{
				SerialPortStream.ThrowIOException();
			}
			return serialSignal;
		}

		[DllImport("MonoPosixHelper", SetLastError = true)]
		private static extern int set_signal(int fd, SerialSignal signal, bool value);

		public void SetSignal(SerialSignal signal, bool value)
		{
			if (signal < SerialSignal.Cd || signal > SerialSignal.Rts || signal == SerialSignal.Cd || signal == SerialSignal.Cts || signal == SerialSignal.Dsr)
			{
				throw new Exception("Invalid internal value");
			}
			if (SerialPortStream.set_signal(this.fd, signal, value) == -1)
			{
				SerialPortStream.ThrowIOException();
			}
		}

		[DllImport("MonoPosixHelper", SetLastError = true)]
		private static extern int breakprop(int fd);

		public void SetBreakState(bool value)
		{
			if (value && SerialPortStream.breakprop(this.fd) == -1)
			{
				SerialPortStream.ThrowIOException();
			}
		}

		[DllImport("libc")]
		private static extern IntPtr strerror(int errnum);

		private static void ThrowIOException()
		{
			throw new IOException(Marshal.PtrToStringAnsi(SerialPortStream.strerror(Marshal.GetLastWin32Error())));
		}

		[DllImport("MonoPosixHelper")]
		private static extern bool is_baud_rate_legal(int baud_rate);

		private void TryBaudRate(int baudRate)
		{
			if (!SerialPortStream.is_baud_rate_legal(baudRate))
			{
				throw new ArgumentOutOfRangeException("baudRate", "Given baud rate is not supported on this platform.");
			}
		}

		private int fd;

		private int read_timeout;

		private int write_timeout;

		private bool disposed;
	}
}
