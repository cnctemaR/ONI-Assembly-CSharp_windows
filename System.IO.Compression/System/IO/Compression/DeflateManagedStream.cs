using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression
{
	internal sealed class DeflateManagedStream : Stream
	{
		internal DeflateManagedStream(Stream stream, ZipArchiveEntry.CompressionMethodValues method)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("Stream does not support reading.", "stream");
			}
			this.InitializeInflater(stream, false, null, method);
		}

		internal void InitializeInflater(Stream stream, bool leaveOpen, IFileFormatReader reader = null, ZipArchiveEntry.CompressionMethodValues method = ZipArchiveEntry.CompressionMethodValues.Deflate)
		{
			if (!stream.CanRead)
			{
				throw new ArgumentException("Stream does not support reading.", "stream");
			}
			this._inflater = new InflaterManaged(reader, method == ZipArchiveEntry.CompressionMethodValues.Deflate64);
			this._stream = stream;
			this._mode = CompressionMode.Decompress;
			this._leaveOpen = leaveOpen;
			this._buffer = new byte[8192];
		}

		internal void SetFileFormatWriter(IFileFormatWriter writer)
		{
			if (writer != null)
			{
				this._formatWriter = writer;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this._stream != null && this._mode == CompressionMode.Decompress && this._stream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._stream != null && this._mode == CompressionMode.Compress && this._stream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException("This operation is not supported.");
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException("This operation is not supported.");
			}
			set
			{
				throw new NotSupportedException("This operation is not supported.");
			}
		}

		public override void Flush()
		{
			this.EnsureNotDisposed();
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			this.EnsureNotDisposed();
			if (!cancellationToken.IsCancellationRequested)
			{
				return Task.CompletedTask;
			}
			return Task.FromCanceled(cancellationToken);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("This operation is not supported.");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("This operation is not supported.");
		}

		public override int Read(byte[] array, int offset, int count)
		{
			this.EnsureDecompressionMode();
			this.ValidateParameters(array, offset, count);
			this.EnsureNotDisposed();
			int num = offset;
			int num2 = count;
			for (;;)
			{
				int num3 = this._inflater.Inflate(array, num, num2);
				num += num3;
				num2 -= num3;
				if (num2 == 0 || this._inflater.Finished())
				{
					goto IL_008A;
				}
				int num4 = this._stream.Read(this._buffer, 0, this._buffer.Length);
				if (num4 <= 0)
				{
					goto IL_008A;
				}
				if (num4 > this._buffer.Length)
				{
					break;
				}
				this._inflater.SetInput(this._buffer, 0, num4);
			}
			throw new InvalidDataException("Found invalid data while decoding.");
			IL_008A:
			return count - num2;
		}

		private void ValidateParameters(byte[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException("Offset plus count is larger than the length of target array.");
			}
		}

		private void EnsureNotDisposed()
		{
			if (this._stream == null)
			{
				DeflateManagedStream.ThrowStreamClosedException();
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowStreamClosedException()
		{
			throw new ObjectDisposedException(null, "Can not access a closed Stream.");
		}

		private void EnsureDecompressionMode()
		{
			if (this._mode != CompressionMode.Decompress)
			{
				DeflateManagedStream.ThrowCannotReadFromDeflateManagedStreamException();
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowCannotReadFromDeflateManagedStreamException()
		{
			throw new InvalidOperationException("Reading from the compression stream is not supported.");
		}

		private void EnsureCompressionMode()
		{
			if (this._mode != CompressionMode.Compress)
			{
				DeflateManagedStream.ThrowCannotWriteToDeflateManagedStreamException();
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowCannotWriteToDeflateManagedStreamException()
		{
			throw new InvalidOperationException("Writing to the compression stream is not supported.");
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return TaskToApm.Begin(this.ReadAsync(buffer, offset, count, CancellationToken.None), asyncCallback, asyncState);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return TaskToApm.End<int>(asyncResult);
		}

		public override Task<int> ReadAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
		{
			this.EnsureDecompressionMode();
			if (this._asyncOperations != 0)
			{
				throw new InvalidOperationException("Only one asynchronous reader or writer is allowed time at one time.");
			}
			this.ValidateParameters(array, offset, count);
			this.EnsureNotDisposed();
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			Interlocked.Increment(ref this._asyncOperations);
			Task<int> task = null;
			Task<int> task2;
			try
			{
				int num = this._inflater.Inflate(array, offset, count);
				if (num != 0)
				{
					task2 = Task.FromResult<int>(num);
				}
				else if (this._inflater.Finished())
				{
					task2 = Task.FromResult<int>(0);
				}
				else
				{
					task = this._stream.ReadAsync(this._buffer, 0, this._buffer.Length, cancellationToken);
					if (task == null)
					{
						throw new InvalidOperationException("Stream does not support reading.");
					}
					task2 = this.ReadAsyncCore(task, array, offset, count, cancellationToken);
				}
			}
			finally
			{
				if (task == null)
				{
					Interlocked.Decrement(ref this._asyncOperations);
				}
			}
			return task2;
		}

		private async Task<int> ReadAsyncCore(Task<int> readTask, byte[] array, int offset, int count, CancellationToken cancellationToken)
		{
			int num2;
			try
			{
				int num;
				for (;;)
				{
					num = await readTask.ConfigureAwait(false);
					this.EnsureNotDisposed();
					if (num <= 0)
					{
						break;
					}
					if (num > this._buffer.Length)
					{
						goto Block_2;
					}
					cancellationToken.ThrowIfCancellationRequested();
					this._inflater.SetInput(this._buffer, 0, num);
					num = this._inflater.Inflate(array, offset, count);
					if (num != 0 || this._inflater.Finished())
					{
						goto IL_012C;
					}
					readTask = this._stream.ReadAsync(this._buffer, 0, this._buffer.Length, cancellationToken);
					if (readTask == null)
					{
						goto Block_5;
					}
				}
				return 0;
				Block_2:
				throw new InvalidDataException("Found invalid data while decoding.");
				Block_5:
				throw new InvalidOperationException("Stream does not support reading.");
				IL_012C:
				num2 = num;
			}
			finally
			{
				Interlocked.Decrement(ref this._asyncOperations);
			}
			return num2;
		}

		public override void Write(byte[] array, int offset, int count)
		{
			this.EnsureCompressionMode();
			this.ValidateParameters(array, offset, count);
			this.EnsureNotDisposed();
			this.DoMaintenance(array, offset, count);
			this.WriteDeflaterOutput();
			this._deflater.SetInput(array, offset, count);
			this.WriteDeflaterOutput();
		}

		private void WriteDeflaterOutput()
		{
			while (!this._deflater.NeedsInput())
			{
				int deflateOutput = this._deflater.GetDeflateOutput(this._buffer);
				if (deflateOutput > 0)
				{
					this._stream.Write(this._buffer, 0, deflateOutput);
				}
			}
		}

		private void DoMaintenance(byte[] array, int offset, int count)
		{
			if (count <= 0)
			{
				return;
			}
			this._wroteBytes = true;
			if (this._formatWriter == null)
			{
				return;
			}
			if (!this._wroteHeader)
			{
				byte[] header = this._formatWriter.GetHeader();
				this._stream.Write(header, 0, header.Length);
				this._wroteHeader = true;
			}
			this._formatWriter.UpdateWithBytesRead(array, offset, count);
		}

		private void PurgeBuffers(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (this._stream == null)
			{
				return;
			}
			this.Flush();
			if (this._mode != CompressionMode.Compress)
			{
				return;
			}
			if (this._wroteBytes)
			{
				this.WriteDeflaterOutput();
				bool flag;
				do
				{
					int num;
					flag = this._deflater.Finish(this._buffer, out num);
					if (num > 0)
					{
						this._stream.Write(this._buffer, 0, num);
					}
				}
				while (!flag);
			}
			else
			{
				bool flag2;
				do
				{
					int num2;
					flag2 = this._deflater.Finish(this._buffer, out num2);
				}
				while (!flag2);
			}
			if (this._formatWriter != null && this._wroteHeader)
			{
				byte[] footer = this._formatWriter.GetFooter();
				this._stream.Write(footer, 0, footer.Length);
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				this.PurgeBuffers(disposing);
			}
			finally
			{
				try
				{
					if (disposing && !this._leaveOpen && this._stream != null)
					{
						this._stream.Dispose();
					}
				}
				finally
				{
					this._stream = null;
					try
					{
						DeflaterManaged deflater = this._deflater;
						if (deflater != null)
						{
							deflater.Dispose();
						}
						InflaterManaged inflater = this._inflater;
						if (inflater != null)
						{
							inflater.Dispose();
						}
					}
					finally
					{
						this._deflater = null;
						this._inflater = null;
						base.Dispose(disposing);
					}
				}
			}
		}

		public override Task WriteAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
		{
			this.EnsureCompressionMode();
			if (this._asyncOperations != 0)
			{
				throw new InvalidOperationException("Only one asynchronous reader or writer is allowed time at one time.");
			}
			this.ValidateParameters(array, offset, count);
			this.EnsureNotDisposed();
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			return this.WriteAsyncCore(array, offset, count, cancellationToken);
		}

		private async Task WriteAsyncCore(byte[] array, int offset, int count, CancellationToken cancellationToken)
		{
			Interlocked.Increment(ref this._asyncOperations);
			try
			{
				await base.WriteAsync(array, offset, count, cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				Interlocked.Decrement(ref this._asyncOperations);
			}
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return TaskToApm.Begin(this.WriteAsync(buffer, offset, count, CancellationToken.None), asyncCallback, asyncState);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			TaskToApm.End(asyncResult);
		}

		internal const int DefaultBufferSize = 8192;

		private Stream _stream;

		private CompressionMode _mode;

		private bool _leaveOpen;

		private InflaterManaged _inflater;

		private DeflaterManaged _deflater;

		private byte[] _buffer;

		private int _asyncOperations;

		private IFileFormatWriter _formatWriter;

		private bool _wroteHeader;

		private bool _wroteBytes;
	}
}
