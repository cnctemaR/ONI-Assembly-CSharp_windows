using System;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public class StdioFileStream : Stream
	{
		public StdioFileStream(IntPtr fileStream)
			: this(fileStream, true)
		{
		}

		public StdioFileStream(IntPtr fileStream, bool ownsHandle)
		{
			this.owner = true;
			this.file = StdioFileStream.InvalidFileStream;
			base..ctor();
			this.InitStream(fileStream, ownsHandle);
		}

		public StdioFileStream(IntPtr fileStream, FileAccess access)
			: this(fileStream, access, true)
		{
		}

		public StdioFileStream(IntPtr fileStream, FileAccess access, bool ownsHandle)
		{
			this.owner = true;
			this.file = StdioFileStream.InvalidFileStream;
			base..ctor();
			this.InitStream(fileStream, ownsHandle);
			this.InitCanReadWrite(access);
		}

		public StdioFileStream(string path)
		{
			this.owner = true;
			this.file = StdioFileStream.InvalidFileStream;
			base..ctor();
			this.InitStream(StdioFileStream.Fopen(path, "rb"), true);
		}

		public StdioFileStream(string path, string mode)
		{
			this.owner = true;
			this.file = StdioFileStream.InvalidFileStream;
			base..ctor();
			this.InitStream(StdioFileStream.Fopen(path, mode), true);
		}

		public StdioFileStream(string path, FileMode mode)
		{
			this.owner = true;
			this.file = StdioFileStream.InvalidFileStream;
			base..ctor();
			this.InitStream(StdioFileStream.Fopen(path, StdioFileStream.ToFopenMode(path, mode)), true);
		}

		public StdioFileStream(string path, FileAccess access)
		{
			this.owner = true;
			this.file = StdioFileStream.InvalidFileStream;
			base..ctor();
			this.InitStream(StdioFileStream.Fopen(path, StdioFileStream.ToFopenMode(path, access)), true);
			this.InitCanReadWrite(access);
		}

		public StdioFileStream(string path, FileMode mode, FileAccess access)
		{
			this.owner = true;
			this.file = StdioFileStream.InvalidFileStream;
			base..ctor();
			this.InitStream(StdioFileStream.Fopen(path, StdioFileStream.ToFopenMode(path, mode, access)), true);
			this.InitCanReadWrite(access);
		}

		private static IntPtr Fopen(string path, string mode)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("path");
			}
			if (mode == null)
			{
				throw new ArgumentNullException("mode");
			}
			IntPtr intPtr = Stdlib.fopen(path, mode);
			if (intPtr == IntPtr.Zero)
			{
				throw new DirectoryNotFoundException("path", UnixMarshal.CreateExceptionForLastError());
			}
			return intPtr;
		}

		private void InitStream(IntPtr fileStream, bool ownsHandle)
		{
			if (StdioFileStream.InvalidFileStream == fileStream)
			{
				throw new ArgumentException(Locale.GetText("Invalid file stream"), "fileStream");
			}
			this.file = fileStream;
			this.owner = ownsHandle;
			try
			{
				long num = (long)Stdlib.fseek(this.file, 0L, SeekFlags.SEEK_CUR);
				if (num != -1L)
				{
					this.canSeek = true;
				}
				Stdlib.fread(IntPtr.Zero, 0UL, 0UL, this.file);
				if (Stdlib.ferror(this.file) == 0)
				{
					this.canRead = true;
				}
				Stdlib.fwrite(IntPtr.Zero, 0UL, 0UL, this.file);
				if (Stdlib.ferror(this.file) == 0)
				{
					this.canWrite = true;
				}
				Stdlib.clearerr(this.file);
			}
			catch (Exception)
			{
				throw new ArgumentException(Locale.GetText("Invalid file stream"), "fileStream");
			}
			GC.KeepAlive(this);
		}

		private void InitCanReadWrite(FileAccess access)
		{
			this.canRead = this.canRead && (access == FileAccess.Read || access == FileAccess.ReadWrite);
			this.canWrite = this.canWrite && (access == FileAccess.Write || access == FileAccess.ReadWrite);
		}

		private static string ToFopenMode(string file, FileMode mode)
		{
			string text = NativeConvert.ToFopenMode(mode);
			StdioFileStream.AssertFileMode(file, mode);
			return text;
		}

		private static string ToFopenMode(string file, FileAccess access)
		{
			return NativeConvert.ToFopenMode(access);
		}

		private static string ToFopenMode(string file, FileMode mode, FileAccess access)
		{
			string text = NativeConvert.ToFopenMode(mode, access);
			bool flag = StdioFileStream.AssertFileMode(file, mode);
			if (mode == FileMode.OpenOrCreate && access == FileAccess.Read && !flag)
			{
				text = "w+b";
			}
			return text;
		}

		private static bool AssertFileMode(string file, FileMode mode)
		{
			bool flag = StdioFileStream.FileExists(file);
			if (mode == FileMode.CreateNew && flag)
			{
				throw new IOException("File exists and FileMode.CreateNew specified");
			}
			if ((mode == FileMode.Open || mode == FileMode.Truncate) && !flag)
			{
				throw new FileNotFoundException("File doesn't exist and FileMode.Open specified", file);
			}
			return flag;
		}

		private static bool FileExists(string file)
		{
			IntPtr intPtr = Stdlib.fopen(file, "r");
			bool flag = intPtr != IntPtr.Zero;
			if (intPtr != IntPtr.Zero)
			{
				Stdlib.fclose(intPtr);
			}
			return flag;
		}

		private void AssertNotDisposed()
		{
			if (this.file == StdioFileStream.InvalidFileStream)
			{
				throw new ObjectDisposedException("Invalid File Stream");
			}
			GC.KeepAlive(this);
		}

		public IntPtr Handle
		{
			get
			{
				this.AssertNotDisposed();
				GC.KeepAlive(this);
				return this.file;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.canRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.canSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.canWrite;
			}
		}

		public override long Length
		{
			get
			{
				this.AssertNotDisposed();
				if (!this.CanSeek)
				{
					throw new NotSupportedException("File Stream doesn't support seeking");
				}
				long num = Stdlib.ftell(this.file);
				if (num == -1L)
				{
					throw new NotSupportedException("Unable to obtain current file position");
				}
				int num2 = Stdlib.fseek(this.file, 0L, SeekFlags.SEEK_END);
				UnixMarshal.ThrowExceptionForLastErrorIf(num2);
				long num3 = Stdlib.ftell(this.file);
				if (num3 == -1L)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				num2 = Stdlib.fseek(this.file, num, SeekFlags.SEEK_SET);
				UnixMarshal.ThrowExceptionForLastErrorIf(num2);
				GC.KeepAlive(this);
				return num3;
			}
		}

		public override long Position
		{
			get
			{
				this.AssertNotDisposed();
				if (!this.CanSeek)
				{
					throw new NotSupportedException("The stream does not support seeking");
				}
				long num = Stdlib.ftell(this.file);
				if (num == -1L)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				GC.KeepAlive(this);
				return num;
			}
			set
			{
				this.AssertNotDisposed();
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		public void SaveFilePosition(FilePosition pos)
		{
			this.AssertNotDisposed();
			int num = Stdlib.fgetpos(this.file, pos);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			GC.KeepAlive(this);
		}

		public void RestoreFilePosition(FilePosition pos)
		{
			this.AssertNotDisposed();
			if (pos == null)
			{
				throw new ArgumentNullException("value");
			}
			int num = Stdlib.fsetpos(this.file, pos);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			GC.KeepAlive(this);
		}

		public override void Flush()
		{
			this.AssertNotDisposed();
			int num = Stdlib.fflush(this.file);
			if (num != 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			GC.KeepAlive(this);
		}

		public unsafe override int Read([In] [Out] byte[] buffer, int offset, int count)
		{
			this.AssertNotDisposed();
			this.AssertValidBuffer(buffer, offset, count);
			if (!this.CanRead)
			{
				throw new NotSupportedException("Stream does not support reading");
			}
			ulong num;
			fixed (byte* ptr = &buffer[offset])
			{
				num = Stdlib.fread((void*)ptr, 1UL, (ulong)((long)count), this.file);
			}
			if (num != (ulong)((long)count) && Stdlib.ferror(this.file) != 0)
			{
				throw new IOException();
			}
			GC.KeepAlive(this);
			return (int)num;
		}

		private void AssertValidBuffer(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (offset > buffer.Length)
			{
				throw new ArgumentException("destination offset is beyond array size");
			}
			if (offset > buffer.Length - count)
			{
				throw new ArgumentException("would overrun buffer");
			}
		}

		public void Rewind()
		{
			this.AssertNotDisposed();
			Stdlib.rewind(this.file);
			GC.KeepAlive(this);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.AssertNotDisposed();
			if (!this.CanSeek)
			{
				throw new NotSupportedException("The File Stream does not support seeking");
			}
			SeekFlags seekFlags;
			switch (origin)
			{
			case SeekOrigin.Begin:
				seekFlags = SeekFlags.SEEK_SET;
				break;
			case SeekOrigin.Current:
				seekFlags = SeekFlags.SEEK_CUR;
				break;
			case SeekOrigin.End:
				seekFlags = SeekFlags.SEEK_END;
				break;
			default:
				throw new ArgumentException("origin");
			}
			int num = Stdlib.fseek(this.file, offset, seekFlags);
			if (num != 0)
			{
				throw new IOException("Unable to seek", UnixMarshal.CreateExceptionForLastError());
			}
			long num2 = Stdlib.ftell(this.file);
			if (num2 == -1L)
			{
				throw new IOException("Unable to get current file position", UnixMarshal.CreateExceptionForLastError());
			}
			GC.KeepAlive(this);
			return num2;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("ANSI C doesn't provide a way to truncate a file");
		}

		public unsafe override void Write(byte[] buffer, int offset, int count)
		{
			this.AssertNotDisposed();
			this.AssertValidBuffer(buffer, offset, count);
			if (!this.CanWrite)
			{
				throw new NotSupportedException("File Stream does not support writing");
			}
			ulong num;
			fixed (byte* ptr = &buffer[offset])
			{
				num = Stdlib.fwrite((void*)ptr, 1UL, (ulong)((long)count), this.file);
			}
			if (num != (ulong)((long)count))
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			GC.KeepAlive(this);
		}

		~StdioFileStream()
		{
			this.Close();
		}

		public override void Close()
		{
			if (this.file == StdioFileStream.InvalidFileStream)
			{
				return;
			}
			if (this.owner)
			{
				int num = Stdlib.fclose(this.file);
				if (num != 0)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
			}
			else
			{
				this.Flush();
			}
			this.file = StdioFileStream.InvalidFileStream;
			this.canRead = false;
			this.canSeek = false;
			this.canWrite = false;
			GC.SuppressFinalize(this);
			GC.KeepAlive(this);
		}

		public static readonly IntPtr InvalidFileStream = IntPtr.Zero;

		public static readonly IntPtr StandardInput = Stdlib.stdin;

		public static readonly IntPtr StandardOutput = Stdlib.stdout;

		public static readonly IntPtr StandardError = Stdlib.stderr;

		private bool canSeek;

		private bool canRead;

		private bool canWrite;

		private bool owner;

		private IntPtr file;
	}
}
