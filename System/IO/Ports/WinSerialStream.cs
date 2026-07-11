using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.IO.Ports
{
	internal class WinSerialStream : Stream, ISerialStream, IDisposable
	{
		[DllImport("kernel32", SetLastError = true)]
		private static extern int CreateFile(string port_name, uint desired_access, uint share_mode, uint security_attrs, uint creation, uint flags, uint template);

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool SetupComm(int handle, int read_buffer_size, int write_buffer_size);

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool PurgeComm(int handle, uint flags);

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool SetCommTimeouts(int handle, Timeouts timeouts);

		public WinSerialStream(string port_name, int baud_rate, int data_bits, Parity parity, StopBits sb, bool dtr_enable, bool rts_enable, Handshake hs, int read_timeout, int write_timeout, int read_buffer_size, int write_buffer_size)
		{
			this.handle = WinSerialStream.CreateFile((port_name != null && !port_name.StartsWith("\\\\.\\")) ? ("\\\\.\\" + port_name) : port_name, 3221225472U, 0U, 0U, 3U, 1073741824U, 0U);
			if (this.handle == -1)
			{
				this.ReportIOError(port_name);
			}
			this.SetAttributes(baud_rate, parity, data_bits, sb, hs);
			if (!WinSerialStream.PurgeComm(this.handle, 12U) || !WinSerialStream.SetupComm(this.handle, read_buffer_size, write_buffer_size))
			{
				this.ReportIOError(null);
			}
			this.read_timeout = read_timeout;
			this.write_timeout = write_timeout;
			this.timeouts = new Timeouts(read_timeout, write_timeout);
			if (!WinSerialStream.SetCommTimeouts(this.handle, this.timeouts))
			{
				this.ReportIOError(null);
			}
			this.SetSignal(SerialSignal.Dtr, dtr_enable);
			if (hs != Handshake.RequestToSend && hs != Handshake.RequestToSendXOnXOff)
			{
				this.SetSignal(SerialSignal.Rts, rts_enable);
			}
			NativeOverlapped nativeOverlapped = default(NativeOverlapped);
			this.write_event = new ManualResetEvent(false);
			nativeOverlapped.EventHandle = this.write_event.Handle;
			this.write_overlapped = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeOverlapped)));
			Marshal.StructureToPtr<NativeOverlapped>(nativeOverlapped, this.write_overlapped, true);
			NativeOverlapped nativeOverlapped2 = default(NativeOverlapped);
			this.read_event = new ManualResetEvent(false);
			nativeOverlapped2.EventHandle = this.read_event.Handle;
			this.read_overlapped = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeOverlapped)));
			Marshal.StructureToPtr<NativeOverlapped>(nativeOverlapped2, this.read_overlapped, true);
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
				this.timeouts.SetValues(value, this.write_timeout);
				if (!WinSerialStream.SetCommTimeouts(this.handle, this.timeouts))
				{
					this.ReportIOError(null);
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
				this.timeouts.SetValues(this.read_timeout, value);
				if (!WinSerialStream.SetCommTimeouts(this.handle, this.timeouts))
				{
					this.ReportIOError(null);
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

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool CloseHandle(int handle);

		protected override void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			WinSerialStream.CloseHandle(this.handle);
			Marshal.FreeHGlobal(this.write_overlapped);
			Marshal.FreeHGlobal(this.read_overlapped);
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override void Close()
		{
			((IDisposable)this).Dispose();
		}

		~WinSerialStream()
		{
			this.Dispose(false);
		}

		public override void Flush()
		{
			this.CheckDisposed();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		[DllImport("kernel32", SetLastError = true)]
		private unsafe static extern bool ReadFile(int handle, byte* buffer, int bytes_to_read, out int bytes_read, IntPtr overlapped);

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool GetOverlappedResult(int handle, IntPtr overlapped, ref int bytes_transfered, bool wait);

		public unsafe override int Read([In] [Out] byte[] buffer, int offset, int count)
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
			fixed (byte[] array = buffer)
			{
				byte* ptr;
				if (buffer == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				if (WinSerialStream.ReadFile(this.handle, ptr + offset, count, out num, this.read_overlapped))
				{
					return num;
				}
				if ((long)Marshal.GetLastWin32Error() != 997L)
				{
					this.ReportIOError(null);
				}
				if (!WinSerialStream.GetOverlappedResult(this.handle, this.read_overlapped, ref num, true))
				{
					this.ReportIOError(null);
				}
			}
			if (num == 0)
			{
				throw new TimeoutException();
			}
			return num;
		}

		[DllImport("kernel32", SetLastError = true)]
		private unsafe static extern bool WriteFile(int handle, byte* buffer, int bytes_to_write, out int bytes_written, IntPtr overlapped);

		public unsafe override void Write(byte[] buffer, int offset, int count)
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
			int num = 0;
			fixed (byte[] array = buffer)
			{
				byte* ptr;
				if (buffer == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				if (WinSerialStream.WriteFile(this.handle, ptr + offset, count, out num, this.write_overlapped))
				{
					return;
				}
				if ((long)Marshal.GetLastWin32Error() != 997L)
				{
					this.ReportIOError(null);
				}
				if (!WinSerialStream.GetOverlappedResult(this.handle, this.write_overlapped, ref num, true))
				{
					this.ReportIOError(null);
				}
			}
			if (num < count)
			{
				throw new TimeoutException();
			}
		}

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool GetCommState(int handle, [Out] DCB dcb);

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool SetCommState(int handle, DCB dcb);

		public void SetAttributes(int baud_rate, Parity parity, int data_bits, StopBits bits, Handshake hs)
		{
			DCB dcb = new DCB();
			if (!WinSerialStream.GetCommState(this.handle, dcb))
			{
				this.ReportIOError(null);
			}
			dcb.SetValues(baud_rate, parity, data_bits, bits, hs);
			if (!WinSerialStream.SetCommState(this.handle, dcb))
			{
				this.ReportIOError(null);
			}
		}

		private void ReportIOError(string optional_arg)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			string text;
			if (lastWin32Error - 2 > 1)
			{
				if (lastWin32Error != 87)
				{
					text = new Win32Exception().Message;
				}
				else
				{
					text = "Parameter is incorrect.";
				}
			}
			else
			{
				text = "The port `" + optional_arg + "' does not exist.";
			}
			throw new IOException(text);
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		public void DiscardInBuffer()
		{
			if (!WinSerialStream.PurgeComm(this.handle, 8U))
			{
				this.ReportIOError(null);
			}
		}

		public void DiscardOutBuffer()
		{
			if (!WinSerialStream.PurgeComm(this.handle, 4U))
			{
				this.ReportIOError(null);
			}
		}

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool ClearCommError(int handle, out uint errors, out CommStat stat);

		public int BytesToRead
		{
			get
			{
				uint num;
				CommStat commStat;
				if (!WinSerialStream.ClearCommError(this.handle, out num, out commStat))
				{
					this.ReportIOError(null);
				}
				return (int)commStat.BytesIn;
			}
		}

		public int BytesToWrite
		{
			get
			{
				uint num;
				CommStat commStat;
				if (!WinSerialStream.ClearCommError(this.handle, out num, out commStat))
				{
					this.ReportIOError(null);
				}
				return (int)commStat.BytesOut;
			}
		}

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool GetCommModemStatus(int handle, out uint flags);

		public SerialSignal GetSignals()
		{
			uint num;
			if (!WinSerialStream.GetCommModemStatus(this.handle, out num))
			{
				this.ReportIOError(null);
			}
			SerialSignal serialSignal = SerialSignal.None;
			if ((num & 128U) != 0U)
			{
				serialSignal |= SerialSignal.Cd;
			}
			if ((num & 16U) != 0U)
			{
				serialSignal |= SerialSignal.Cts;
			}
			if ((num & 32U) != 0U)
			{
				serialSignal |= SerialSignal.Dsr;
			}
			return serialSignal;
		}

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool EscapeCommFunction(int handle, uint flags);

		public void SetSignal(SerialSignal signal, bool value)
		{
			if (signal != SerialSignal.Rts && signal != SerialSignal.Dtr)
			{
				throw new Exception("Wrong internal value");
			}
			uint num;
			if (signal == SerialSignal.Rts)
			{
				if (value)
				{
					num = 3U;
				}
				else
				{
					num = 4U;
				}
			}
			else if (value)
			{
				num = 5U;
			}
			else
			{
				num = 6U;
			}
			if (!WinSerialStream.EscapeCommFunction(this.handle, num))
			{
				this.ReportIOError(null);
			}
		}

		public void SetBreakState(bool value)
		{
			if (!WinSerialStream.EscapeCommFunction(this.handle, value ? 8U : 9U))
			{
				this.ReportIOError(null);
			}
		}

		private const uint GenericRead = 2147483648U;

		private const uint GenericWrite = 1073741824U;

		private const uint OpenExisting = 3U;

		private const uint FileFlagOverlapped = 1073741824U;

		private const uint PurgeRxClear = 8U;

		private const uint PurgeTxClear = 4U;

		private const uint WinInfiniteTimeout = 4294967295U;

		private const uint FileIOPending = 997U;

		private const uint SetRts = 3U;

		private const uint ClearRts = 4U;

		private const uint SetDtr = 5U;

		private const uint ClearDtr = 6U;

		private const uint SetBreak = 8U;

		private const uint ClearBreak = 9U;

		private const uint CtsOn = 16U;

		private const uint DsrOn = 32U;

		private const uint RsldOn = 128U;

		private const uint EvRxChar = 1U;

		private const uint EvCts = 8U;

		private const uint EvDsr = 16U;

		private const uint EvRlsd = 32U;

		private const uint EvBreak = 64U;

		private const uint EvErr = 128U;

		private const uint EvRing = 256U;

		private int handle;

		private int read_timeout;

		private int write_timeout;

		private bool disposed;

		private IntPtr write_overlapped;

		private IntPtr read_overlapped;

		private ManualResetEvent read_event;

		private ManualResetEvent write_event;

		private Timeouts timeouts;
	}
}
