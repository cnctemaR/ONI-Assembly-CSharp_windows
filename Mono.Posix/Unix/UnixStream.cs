using System;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixStream : Stream, IDisposable
	{
		public UnixStream(int fileDescriptor)
			: this(fileDescriptor, true)
		{
		}

		public UnixStream(int fileDescriptor, bool ownsHandle)
		{
			if (fileDescriptor == -1)
			{
				throw new ArgumentException(Locale.GetText("Invalid file descriptor"), "fileDescriptor");
			}
			this.fileDescriptor = fileDescriptor;
			this.owner = ownsHandle;
			long num = Syscall.lseek(fileDescriptor, 0L, SeekFlags.SEEK_CUR);
			if (num != -1L)
			{
				this.canSeek = true;
			}
			long num2 = Syscall.read(fileDescriptor, IntPtr.Zero, 0UL);
			if (num2 != -1L)
			{
				this.canRead = true;
			}
			long num3 = Syscall.write(fileDescriptor, IntPtr.Zero, 0UL);
			if (num3 != -1L)
			{
				this.canWrite = true;
			}
		}

		void IDisposable.Dispose()
		{
			this.AssertNotDisposed();
			if (this.owner)
			{
				this.Close();
			}
			GC.SuppressFinalize(this);
		}

		private void AssertNotDisposed()
		{
			if (this.fileDescriptor == -1)
			{
				throw new ObjectDisposedException("Invalid File Descriptor");
			}
		}

		public int Handle
		{
			get
			{
				return this.fileDescriptor;
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
					throw new NotSupportedException("File descriptor doesn't support seeking");
				}
				this.RefreshStat();
				return this.stat.st_size;
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
				long num = Syscall.lseek(this.fileDescriptor, 0L, SeekFlags.SEEK_CUR);
				if (num == -1L)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				return num;
			}
			set
			{
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		[CLSCompliant(false)]
		public FilePermissions Protection
		{
			get
			{
				this.RefreshStat();
				return this.stat.st_mode;
			}
			set
			{
				value &= ~FilePermissions.S_IFMT;
				int num = Syscall.fchmod(this.fileDescriptor, value);
				UnixMarshal.ThrowExceptionForLastErrorIf(num);
			}
		}

		public FileTypes FileType
		{
			get
			{
				int protection = (int)this.Protection;
				return (FileTypes)(protection & 61440);
			}
		}

		public FileAccessPermissions FileAccessPermissions
		{
			get
			{
				int protection = (int)this.Protection;
				return (FileAccessPermissions)(protection & 511);
			}
			set
			{
				int num = (int)this.Protection;
				num &= -512;
				num |= (int)value;
				this.Protection = (FilePermissions)num;
			}
		}

		public FileSpecialAttributes FileSpecialAttributes
		{
			get
			{
				int protection = (int)this.Protection;
				return (FileSpecialAttributes)(protection & 3584);
			}
			set
			{
				int num = (int)this.Protection;
				num &= -3585;
				num |= (int)value;
				this.Protection = (FilePermissions)num;
			}
		}

		public UnixUserInfo OwnerUser
		{
			get
			{
				this.RefreshStat();
				return new UnixUserInfo(this.stat.st_uid);
			}
		}

		public long OwnerUserId
		{
			get
			{
				this.RefreshStat();
				return (long)((ulong)this.stat.st_uid);
			}
		}

		public UnixGroupInfo OwnerGroup
		{
			get
			{
				this.RefreshStat();
				return new UnixGroupInfo((long)((ulong)this.stat.st_gid));
			}
		}

		public long OwnerGroupId
		{
			get
			{
				this.RefreshStat();
				return (long)((ulong)this.stat.st_gid);
			}
		}

		private void RefreshStat()
		{
			this.AssertNotDisposed();
			int num = Syscall.fstat(this.fileDescriptor, out this.stat);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public void AdviseFileAccessPattern(FileAccessPattern pattern, long offset, long len)
		{
			FileHandleOperations.AdviseFileAccessPattern(this.fileDescriptor, pattern, offset, len);
		}

		public void AdviseFileAccessPattern(FileAccessPattern pattern)
		{
			this.AdviseFileAccessPattern(pattern, 0L, 0L);
		}

		public override void Flush()
		{
		}

		public unsafe override int Read([In] [Out] byte[] buffer, int offset, int count)
		{
			this.AssertNotDisposed();
			this.AssertValidBuffer(buffer, offset, count);
			if (!this.CanRead)
			{
				throw new NotSupportedException("Stream does not support reading");
			}
			if (buffer.Length == 0)
			{
				return 0;
			}
			long num;
			fixed (byte* ptr = &buffer[offset])
			{
				do
				{
					num = Syscall.read(this.fileDescriptor, (void*)ptr, (ulong)((long)count));
				}
				while (UnixMarshal.ShouldRetrySyscall((int)num));
			}
			if (num == -1L)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
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

		public unsafe int ReadAtOffset([In] [Out] byte[] buffer, int offset, int count, long fileOffset)
		{
			this.AssertNotDisposed();
			this.AssertValidBuffer(buffer, offset, count);
			if (!this.CanRead)
			{
				throw new NotSupportedException("Stream does not support reading");
			}
			if (buffer.Length == 0)
			{
				return 0;
			}
			long num;
			fixed (byte* ptr = &buffer[offset])
			{
				do
				{
					num = Syscall.pread(this.fileDescriptor, (void*)ptr, (ulong)((long)count), fileOffset);
				}
				while (UnixMarshal.ShouldRetrySyscall((int)num));
			}
			if (num == -1L)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return (int)num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.AssertNotDisposed();
			if (!this.CanSeek)
			{
				throw new NotSupportedException("The File Descriptor does not support seeking");
			}
			SeekFlags seekFlags = SeekFlags.SEEK_CUR;
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
			}
			long num = Syscall.lseek(this.fileDescriptor, offset, seekFlags);
			if (num == -1L)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return num;
		}

		public override void SetLength(long value)
		{
			this.AssertNotDisposed();
			if (value < 0L)
			{
				throw new ArgumentOutOfRangeException("value", "< 0");
			}
			if (!this.CanSeek && !this.CanWrite)
			{
				throw new NotSupportedException("You can't truncating the current file descriptor");
			}
			int num;
			do
			{
				num = Syscall.ftruncate(this.fileDescriptor, value);
			}
			while (UnixMarshal.ShouldRetrySyscall(num));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public unsafe override void Write(byte[] buffer, int offset, int count)
		{
			this.AssertNotDisposed();
			this.AssertValidBuffer(buffer, offset, count);
			if (!this.CanWrite)
			{
				throw new NotSupportedException("File Descriptor does not support writing");
			}
			if (buffer.Length == 0)
			{
				return;
			}
			long num;
			fixed (byte* ptr = &buffer[offset])
			{
				do
				{
					num = Syscall.write(this.fileDescriptor, (void*)ptr, (ulong)((long)count));
				}
				while (UnixMarshal.ShouldRetrySyscall((int)num));
			}
			if (num == -1L)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
		}

		public unsafe void WriteAtOffset(byte[] buffer, int offset, int count, long fileOffset)
		{
			this.AssertNotDisposed();
			this.AssertValidBuffer(buffer, offset, count);
			if (!this.CanWrite)
			{
				throw new NotSupportedException("File Descriptor does not support writing");
			}
			if (buffer.Length == 0)
			{
				return;
			}
			long num;
			fixed (byte* ptr = &buffer[offset])
			{
				do
				{
					num = Syscall.pwrite(this.fileDescriptor, (void*)ptr, (ulong)((long)count), fileOffset);
				}
				while (UnixMarshal.ShouldRetrySyscall((int)num));
			}
			if (num == -1L)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
		}

		public void SendTo(UnixStream output)
		{
			this.SendTo(output, (ulong)output.Length);
		}

		[CLSCompliant(false)]
		public void SendTo(UnixStream output, ulong count)
		{
			this.SendTo(output.Handle, count);
		}

		[CLSCompliant(false)]
		public void SendTo(int out_fd, ulong count)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Unable to write to the current file descriptor");
			}
			long position = this.Position;
			long num = Syscall.sendfile(out_fd, this.fileDescriptor, ref position, count);
			if (num == -1L)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
		}

		public void SetOwner(long user, long group)
		{
			this.AssertNotDisposed();
			int num = Syscall.fchown(this.fileDescriptor, Convert.ToUInt32(user), Convert.ToUInt32(group));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public void SetOwner(string user, string group)
		{
			this.AssertNotDisposed();
			long userId = new UnixUserInfo(user).UserId;
			long groupId = new UnixGroupInfo(group).GroupId;
			this.SetOwner(userId, groupId);
		}

		public void SetOwner(string user)
		{
			this.AssertNotDisposed();
			Passwd passwd = Syscall.getpwnam(user);
			if (passwd == null)
			{
				throw new ArgumentException(Locale.GetText("invalid username"), "user");
			}
			long num = (long)((ulong)passwd.pw_uid);
			long num2 = (long)((ulong)passwd.pw_gid);
			this.SetOwner(num, num2);
		}

		[CLSCompliant(false)]
		public long GetConfigurationValue(PathconfName name)
		{
			this.AssertNotDisposed();
			long num = Syscall.fpathconf(this.fileDescriptor, name);
			if (num == -1L && Stdlib.GetLastError() != (Errno)0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return num;
		}

		~UnixStream()
		{
			this.Close();
		}

		public override void Close()
		{
			if (this.fileDescriptor == -1)
			{
				return;
			}
			this.Flush();
			if (!this.owner)
			{
				return;
			}
			int num;
			do
			{
				num = Syscall.close(this.fileDescriptor);
			}
			while (UnixMarshal.ShouldRetrySyscall(num));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			this.fileDescriptor = -1;
			GC.SuppressFinalize(this);
		}

		public const int InvalidFileDescriptor = -1;

		public const int StandardInputFileDescriptor = 0;

		public const int StandardOutputFileDescriptor = 1;

		public const int StandardErrorFileDescriptor = 2;

		private bool canSeek;

		private bool canRead;

		private bool canWrite;

		private bool owner = true;

		private int fileDescriptor = -1;

		private Stat stat;
	}
}
