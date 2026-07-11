using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace System.IO
{
	[ComVisible(true)]
	public class FileStream : Stream
	{
		[Obsolete("Use FileStream(SafeFileHandle handle, FileAccess access) instead")]
		public FileStream(IntPtr handle, FileAccess access)
			: this(handle, access, true, 8192, false)
		{
		}

		[Obsolete("Use FileStream(SafeFileHandle handle, FileAccess access) instead")]
		public FileStream(IntPtr handle, FileAccess access, bool ownsHandle)
			: this(handle, access, ownsHandle, 8192, false)
		{
		}

		[Obsolete("Use FileStream(SafeFileHandle handle, FileAccess access, int bufferSize) instead")]
		public FileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize)
			: this(handle, access, ownsHandle, bufferSize, false)
		{
		}

		[Obsolete("Use FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) instead")]
		public FileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync)
			: this(handle, access, ownsHandle, bufferSize, isAsync, false)
		{
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		internal FileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync, bool noBuffering)
		{
			this.name = "[Unknown]";
			base..ctor();
			this.handle = MonoIO.InvalidHandle;
			if (handle == this.handle)
			{
				throw new ArgumentException("handle", Locale.GetText("Invalid."));
			}
			if (access < FileAccess.Read || access > FileAccess.ReadWrite)
			{
				throw new ArgumentOutOfRangeException("access");
			}
			MonoIOError monoIOError;
			MonoFileType fileType = MonoIO.GetFileType(handle, out monoIOError);
			if (monoIOError != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(this.name, monoIOError);
			}
			if (fileType == MonoFileType.Unknown)
			{
				throw new IOException("Invalid handle.");
			}
			if (fileType == MonoFileType.Disk)
			{
				this.canseek = true;
			}
			else
			{
				this.canseek = false;
			}
			this.handle = handle;
			this.access = access;
			this.owner = ownsHandle;
			this.async = isAsync;
			this.anonymous = false;
			this.InitBuffer(bufferSize, noBuffering);
			if (this.canseek)
			{
				this.buf_start = MonoIO.Seek(handle, 0L, SeekOrigin.Current, out monoIOError);
				if (monoIOError != MonoIOError.ERROR_SUCCESS)
				{
					throw MonoIO.GetException(this.name, monoIOError);
				}
			}
			this.append_startpos = 0L;
		}

		public FileStream(string path, FileMode mode)
			: this(path, mode, (mode != FileMode.Append) ? FileAccess.ReadWrite : FileAccess.Write, FileShare.Read, 8192, false, FileOptions.None)
		{
		}

		public FileStream(string path, FileMode mode, FileAccess access)
			: this(path, mode, access, (access != FileAccess.Write) ? FileShare.Read : FileShare.None, 8192, false, false)
		{
		}

		public FileStream(string path, FileMode mode, FileAccess access, FileShare share)
			: this(path, mode, access, share, 8192, false, FileOptions.None)
		{
		}

		public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
			: this(path, mode, access, share, bufferSize, false, FileOptions.None)
		{
		}

		public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync)
			: this(path, mode, access, share, bufferSize, useAsync, FileOptions.None)
		{
		}

		public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
			: this(path, mode, access, share, bufferSize, false, options)
		{
		}

		public FileStream(SafeFileHandle handle, FileAccess access)
			: this(handle, access, 8192, false)
		{
		}

		public FileStream(SafeFileHandle handle, FileAccess access, int bufferSize)
			: this(handle, access, bufferSize, false)
		{
		}

		[MonoLimitation("Need to use SafeFileHandle instead of underlying handle")]
		public FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync)
			: this(handle.DangerousGetHandle(), access, false, bufferSize, isAsync)
		{
			this.safeHandle = handle;
		}

		public FileStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options)
		{
			this.name = "[Unknown]";
			base..ctor();
			throw new NotImplementedException();
		}

		public FileStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity)
		{
			this.name = "[Unknown]";
			base..ctor();
			throw new NotImplementedException();
		}

		internal FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool isAsync, bool anonymous)
			: this(path, mode, access, share, bufferSize, anonymous, (!isAsync) ? FileOptions.None : FileOptions.Asynchronous)
		{
		}

		internal FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool anonymous, FileOptions options)
		{
			this.name = "[Unknown]";
			base..ctor();
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("Path is empty");
			}
			share &= ~FileShare.Inheritable;
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", "Positive number required.");
			}
			if (mode < FileMode.CreateNew || mode > FileMode.Append)
			{
				throw new ArgumentOutOfRangeException("mode", "Enum value was out of legal range.");
			}
			if (access < FileAccess.Read || access > FileAccess.ReadWrite)
			{
				throw new ArgumentOutOfRangeException("access", "Enum value was out of legal range.");
			}
			if ((share < FileShare.None) || share > (FileShare.Read | FileShare.Write | FileShare.Delete))
			{
				throw new ArgumentOutOfRangeException("share", "Enum value was out of legal range.");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Name has invalid chars");
			}
			if (Directory.Exists(path))
			{
				string text = Locale.GetText("Access to the path '{0}' is denied.");
				throw new UnauthorizedAccessException(string.Format(text, this.GetSecureFileName(path, false)));
			}
			if (mode == FileMode.Append && (access & FileAccess.Read) == FileAccess.Read)
			{
				throw new ArgumentException("Append access can be requested only in write-only mode.");
			}
			if ((access & FileAccess.Write) == (FileAccess)0 && mode != FileMode.Open && mode != FileMode.OpenOrCreate)
			{
				string text2 = Locale.GetText("Combining FileMode: {0} with FileAccess: {1} is invalid.");
				throw new ArgumentException(string.Format(text2, access, mode));
			}
			string text3;
			if (Path.DirectorySeparatorChar != '/' && path.IndexOf('/') >= 0)
			{
				text3 = Path.GetDirectoryName(Path.GetFullPath(path));
			}
			else
			{
				text3 = Path.GetDirectoryName(path);
			}
			if (text3.Length > 0)
			{
				string fullPath = Path.GetFullPath(text3);
				if (!Directory.Exists(fullPath))
				{
					string text4 = Locale.GetText("Could not find a part of the path \"{0}\".");
					string text5 = ((!anonymous) ? Path.GetFullPath(path) : text3);
					throw new DirectoryNotFoundException(string.Format(text4, text5));
				}
			}
			if (access == FileAccess.Read && mode != FileMode.Create && mode != FileMode.OpenOrCreate && mode != FileMode.CreateNew && !File.Exists(path))
			{
				string text6 = Locale.GetText("Could not find file \"{0}\".");
				string secureFileName = this.GetSecureFileName(path);
				throw new FileNotFoundException(string.Format(text6, secureFileName), secureFileName);
			}
			if (!anonymous)
			{
				this.name = path;
			}
			MonoIOError monoIOError;
			this.handle = MonoIO.Open(path, mode, access, share, options, out monoIOError);
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw MonoIO.GetException(this.GetSecureFileName(path), monoIOError);
			}
			this.access = access;
			this.owner = true;
			this.anonymous = anonymous;
			if (MonoIO.GetFileType(this.handle, out monoIOError) == MonoFileType.Disk)
			{
				this.canseek = true;
				this.async = (options & FileOptions.Asynchronous) != FileOptions.None;
			}
			else
			{
				this.canseek = false;
				this.async = false;
			}
			if (access == FileAccess.Read && this.canseek && bufferSize == 8192)
			{
				long length = this.Length;
				if ((long)bufferSize > length)
				{
					bufferSize = (int)((length >= 1000L) ? length : 1000L);
				}
			}
			this.InitBuffer(bufferSize, false);
			if (mode == FileMode.Append)
			{
				this.Seek(0L, SeekOrigin.End);
				this.append_startpos = this.Position;
			}
			else
			{
				this.append_startpos = 0L;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.access == FileAccess.Read || this.access == FileAccess.ReadWrite;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.access == FileAccess.Write || this.access == FileAccess.ReadWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.canseek;
			}
		}

		public virtual bool IsAsync
		{
			get
			{
				return this.async;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public override long Length
		{
			get
			{
				if (this.handle == MonoIO.InvalidHandle)
				{
					throw new ObjectDisposedException("Stream has been closed");
				}
				if (!this.CanSeek)
				{
					throw new NotSupportedException("The stream does not support seeking");
				}
				this.FlushBufferIfDirty();
				MonoIOError monoIOError;
				long length = MonoIO.GetLength(this.handle, out monoIOError);
				if (monoIOError != MonoIOError.ERROR_SUCCESS)
				{
					throw MonoIO.GetException(this.GetSecureFileName(this.name), monoIOError);
				}
				return length;
			}
		}

		public override long Position
		{
			get
			{
				if (this.handle == MonoIO.InvalidHandle)
				{
					throw new ObjectDisposedException("Stream has been closed");
				}
				if (!this.CanSeek)
				{
					throw new NotSupportedException("The stream does not support seeking");
				}
				return this.buf_start + (long)this.buf_offset;
			}
			set
			{
				if (this.handle == MonoIO.InvalidHandle)
				{
					throw new ObjectDisposedException("Stream has been closed");
				}
				if (!this.CanSeek)
				{
					throw new NotSupportedException("The stream does not support seeking");
				}
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("Attempt to set the position to a negative value");
				}
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		[Obsolete("Use SafeFileHandle instead")]
		public virtual IntPtr Handle
		{
			[PermissionSet(SecurityAction.InheritanceDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
			[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
			get
			{
				return this.handle;
			}
		}

		public virtual SafeFileHandle SafeFileHandle
		{
			[PermissionSet(SecurityAction.InheritanceDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
			[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
			get
			{
				SafeFileHandle safeFileHandle;
				if (this.safeHandle != null)
				{
					safeFileHandle = this.safeHandle;
				}
				else
				{
					safeFileHandle = new SafeFileHandle(this.handle, this.owner);
				}
				this.FlushBuffer();
				return safeFileHandle;
			}
		}

		public override int ReadByte()
		{
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (!this.CanRead)
			{
				throw new NotSupportedException("Stream does not support reading");
			}
			if (this.buf_size != 0)
			{
				if (this.buf_offset >= this.buf_length)
				{
					this.RefillBuffer();
					if (this.buf_length == 0)
					{
						return -1;
					}
				}
				return (int)this.buf[this.buf_offset++];
			}
			if (this.ReadData(this.handle, this.buf, 0, 1) == 0)
			{
				return -1;
			}
			return (int)this.buf[0];
		}

		public override void WriteByte(byte value)
		{
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Stream does not support writing");
			}
			if (this.buf_offset == this.buf_size)
			{
				this.FlushBuffer();
			}
			if (this.buf_size == 0)
			{
				this.buf[0] = value;
				this.buf_dirty = true;
				this.buf_length = 1;
				this.FlushBuffer();
				return;
			}
			this.buf[this.buf_offset++] = value;
			if (this.buf_offset > this.buf_length)
			{
				this.buf_length = this.buf_offset;
			}
			this.buf_dirty = true;
		}

		public override int Read([In] [Out] byte[] array, int offset, int count)
		{
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (!this.CanRead)
			{
				throw new NotSupportedException("Stream does not support reading");
			}
			int num = array.Length;
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (offset > num)
			{
				throw new ArgumentException("destination offset is beyond array size");
			}
			if (offset > num - count)
			{
				throw new ArgumentException("Reading would overrun buffer");
			}
			if (this.async)
			{
				IAsyncResult asyncResult = this.BeginRead(array, offset, count, null, null);
				return this.EndRead(asyncResult);
			}
			return this.ReadInternal(array, offset, count);
		}

		private int ReadInternal(byte[] dest, int offset, int count)
		{
			int num = 0;
			int num2 = this.ReadSegment(dest, offset, count);
			num += num2;
			count -= num2;
			if (count == 0)
			{
				return num;
			}
			if (count > this.buf_size)
			{
				this.FlushBuffer();
				num2 = this.ReadData(this.handle, dest, offset + num, count);
				this.buf_start += (long)num2;
			}
			else
			{
				this.RefillBuffer();
				num2 = this.ReadSegment(dest, offset + num, count);
			}
			return num + num2;
		}

		public override IAsyncResult BeginRead(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
		{
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (!this.CanRead)
			{
				throw new NotSupportedException("This stream does not support reading");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (numBytes < 0)
			{
				throw new ArgumentOutOfRangeException("numBytes", "Must be >= 0");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
			}
			if (numBytes > array.Length - offset)
			{
				throw new ArgumentException("Buffer too small. numBytes/offset wrong.");
			}
			if (!this.async)
			{
				return base.BeginRead(array, offset, numBytes, userCallback, stateObject);
			}
			FileStream.ReadDelegate readDelegate = new FileStream.ReadDelegate(this.ReadInternal);
			return readDelegate.BeginInvoke(array, offset, numBytes, userCallback, stateObject);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!this.async)
			{
				return base.EndRead(asyncResult);
			}
			AsyncResult asyncResult2 = asyncResult as AsyncResult;
			if (asyncResult2 == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			FileStream.ReadDelegate readDelegate = asyncResult2.AsyncDelegate as FileStream.ReadDelegate;
			if (readDelegate == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			return readDelegate.EndInvoke(asyncResult);
		}

		public override void Write(byte[] array, int offset, int count)
		{
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (offset > array.Length - count)
			{
				throw new ArgumentException("Reading would overrun buffer");
			}
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Stream does not support writing");
			}
			if (this.async)
			{
				IAsyncResult asyncResult = this.BeginWrite(array, offset, count, null, null);
				this.EndWrite(asyncResult);
				return;
			}
			this.WriteInternal(array, offset, count);
		}

		private void WriteInternal(byte[] src, int offset, int count)
		{
			if (count > this.buf_size)
			{
				this.FlushBuffer();
				int i = count;
				while (i > 0)
				{
					MonoIOError monoIOError;
					int num = MonoIO.Write(this.handle, src, offset, i, out monoIOError);
					if (monoIOError != MonoIOError.ERROR_SUCCESS)
					{
						throw MonoIO.GetException(this.GetSecureFileName(this.name), monoIOError);
					}
					i -= num;
					offset += num;
				}
				this.buf_start += (long)count;
			}
			else
			{
				int num2 = 0;
				while (count > 0)
				{
					int num3 = this.WriteSegment(src, offset + num2, count);
					num2 += num3;
					count -= num3;
					if (count == 0)
					{
						break;
					}
					this.FlushBuffer();
				}
			}
		}

		public override IAsyncResult BeginWrite(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
		{
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (!this.CanWrite)
			{
				throw new NotSupportedException("This stream does not support writing");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (numBytes < 0)
			{
				throw new ArgumentOutOfRangeException("numBytes", "Must be >= 0");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
			}
			if (numBytes > array.Length - offset)
			{
				throw new ArgumentException("array too small. numBytes/offset wrong.");
			}
			if (!this.async)
			{
				return base.BeginWrite(array, offset, numBytes, userCallback, stateObject);
			}
			FileStreamAsyncResult fileStreamAsyncResult = new FileStreamAsyncResult(userCallback, stateObject);
			fileStreamAsyncResult.BytesRead = -1;
			fileStreamAsyncResult.Count = numBytes;
			fileStreamAsyncResult.OriginalCount = numBytes;
			if (this.buf_dirty)
			{
				MemoryStream memoryStream = new MemoryStream();
				this.FlushBuffer(memoryStream);
				memoryStream.Write(array, offset, numBytes);
				offset = 0;
				numBytes = (int)memoryStream.Length;
			}
			FileStream.WriteDelegate writeDelegate = new FileStream.WriteDelegate(this.WriteInternal);
			return writeDelegate.BeginInvoke(array, offset, numBytes, userCallback, stateObject);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!this.async)
			{
				base.EndWrite(asyncResult);
				return;
			}
			AsyncResult asyncResult2 = asyncResult as AsyncResult;
			if (asyncResult2 == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			FileStream.WriteDelegate writeDelegate = asyncResult2.AsyncDelegate as FileStream.WriteDelegate;
			if (writeDelegate == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			writeDelegate.EndInvoke(asyncResult);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (!this.CanSeek)
			{
				throw new NotSupportedException("The stream does not support seeking");
			}
			long num;
			switch (origin)
			{
			case SeekOrigin.Begin:
				num = offset;
				break;
			case SeekOrigin.Current:
				num = this.Position + offset;
				break;
			case SeekOrigin.End:
				num = this.Length + offset;
				break;
			default:
				throw new ArgumentException("origin", "Invalid SeekOrigin");
			}
			if (num < 0L)
			{
				throw new IOException("Attempted to Seek before the beginning of the stream");
			}
			if (num < this.append_startpos)
			{
				throw new IOException("Can't seek back over pre-existing data in append mode");
			}
			this.FlushBuffer();
			MonoIOError monoIOError;
			this.buf_start = MonoIO.Seek(this.handle, num, SeekOrigin.Begin, out monoIOError);
			if (monoIOError != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(this.GetSecureFileName(this.name), monoIOError);
			}
			return this.buf_start;
		}

		public override void SetLength(long value)
		{
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (!this.CanSeek)
			{
				throw new NotSupportedException("The stream does not support seeking");
			}
			if (!this.CanWrite)
			{
				throw new NotSupportedException("The stream does not support writing");
			}
			if (value < 0L)
			{
				throw new ArgumentOutOfRangeException("value is less than 0");
			}
			this.Flush();
			MonoIOError monoIOError;
			MonoIO.SetLength(this.handle, value, out monoIOError);
			if (monoIOError != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(this.GetSecureFileName(this.name), monoIOError);
			}
			if (this.Position > value)
			{
				this.Position = value;
			}
		}

		public override void Flush()
		{
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			this.FlushBuffer();
		}

		public virtual void Lock(long position, long length)
		{
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (position < 0L)
			{
				throw new ArgumentOutOfRangeException("position must not be negative");
			}
			if (length < 0L)
			{
				throw new ArgumentOutOfRangeException("length must not be negative");
			}
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			MonoIOError monoIOError;
			MonoIO.Lock(this.handle, position, length, out monoIOError);
			if (monoIOError != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(this.GetSecureFileName(this.name), monoIOError);
			}
		}

		public virtual void Unlock(long position, long length)
		{
			if (this.handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (position < 0L)
			{
				throw new ArgumentOutOfRangeException("position must not be negative");
			}
			if (length < 0L)
			{
				throw new ArgumentOutOfRangeException("length must not be negative");
			}
			MonoIOError monoIOError;
			MonoIO.Unlock(this.handle, position, length, out monoIOError);
			if (monoIOError != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(this.GetSecureFileName(this.name), monoIOError);
			}
		}

		~FileStream()
		{
			this.Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			Exception ex = null;
			if (this.handle != MonoIO.InvalidHandle)
			{
				try
				{
					this.FlushBuffer();
				}
				catch (Exception ex2)
				{
					ex = ex2;
				}
				if (this.owner)
				{
					MonoIOError monoIOError;
					MonoIO.Close(this.handle, out monoIOError);
					if (monoIOError != MonoIOError.ERROR_SUCCESS)
					{
						throw MonoIO.GetException(this.GetSecureFileName(this.name), monoIOError);
					}
					this.handle = MonoIO.InvalidHandle;
				}
			}
			this.canseek = false;
			this.access = (FileAccess)0;
			if (disposing)
			{
				this.buf = null;
			}
			if (disposing)
			{
				GC.SuppressFinalize(this);
			}
			if (ex != null)
			{
				throw ex;
			}
		}

		public FileSecurity GetAccessControl()
		{
			throw new NotImplementedException();
		}

		public void SetAccessControl(FileSecurity fileSecurity)
		{
			throw new NotImplementedException();
		}

		private int ReadSegment(byte[] dest, int dest_offset, int count)
		{
			if (count > this.buf_length - this.buf_offset)
			{
				count = this.buf_length - this.buf_offset;
			}
			if (count > 0)
			{
				Buffer.BlockCopy(this.buf, this.buf_offset, dest, dest_offset, count);
				this.buf_offset += count;
			}
			return count;
		}

		private int WriteSegment(byte[] src, int src_offset, int count)
		{
			if (count > this.buf_size - this.buf_offset)
			{
				count = this.buf_size - this.buf_offset;
			}
			if (count > 0)
			{
				Buffer.BlockCopy(src, src_offset, this.buf, this.buf_offset, count);
				this.buf_offset += count;
				if (this.buf_offset > this.buf_length)
				{
					this.buf_length = this.buf_offset;
				}
				this.buf_dirty = true;
			}
			return count;
		}

		private void FlushBuffer(Stream st)
		{
			if (this.buf_dirty)
			{
				if (this.CanSeek)
				{
					MonoIOError monoIOError;
					MonoIO.Seek(this.handle, this.buf_start, SeekOrigin.Begin, out monoIOError);
					if (monoIOError != MonoIOError.ERROR_SUCCESS)
					{
						throw MonoIO.GetException(this.GetSecureFileName(this.name), monoIOError);
					}
				}
				if (st == null)
				{
					MonoIOError monoIOError;
					MonoIO.Write(this.handle, this.buf, 0, this.buf_length, out monoIOError);
					if (monoIOError != MonoIOError.ERROR_SUCCESS)
					{
						throw MonoIO.GetException(this.GetSecureFileName(this.name), monoIOError);
					}
				}
				else
				{
					st.Write(this.buf, 0, this.buf_length);
				}
			}
			this.buf_start += (long)this.buf_offset;
			this.buf_offset = (this.buf_length = 0);
			this.buf_dirty = false;
		}

		private void FlushBuffer()
		{
			this.FlushBuffer(null);
		}

		private void FlushBufferIfDirty()
		{
			if (this.buf_dirty)
			{
				this.FlushBuffer(null);
			}
		}

		private void RefillBuffer()
		{
			this.FlushBuffer(null);
			this.buf_length = this.ReadData(this.handle, this.buf, 0, this.buf_size);
		}

		private int ReadData(IntPtr handle, byte[] buf, int offset, int count)
		{
			MonoIOError monoIOError;
			int num = MonoIO.Read(handle, buf, offset, count, out monoIOError);
			if (monoIOError == MonoIOError.ERROR_BROKEN_PIPE)
			{
				num = 0;
			}
			else if (monoIOError != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(this.GetSecureFileName(this.name), monoIOError);
			}
			if (num == -1)
			{
				throw new IOException();
			}
			return num;
		}

		private void InitBuffer(int size, bool noBuffering)
		{
			if (noBuffering)
			{
				size = 0;
				this.buf = new byte[1];
			}
			else
			{
				if (size <= 0)
				{
					throw new ArgumentOutOfRangeException("bufferSize", "Positive number required.");
				}
				if (size < 8)
				{
					size = 8;
				}
				this.buf = new byte[size];
			}
			this.buf_size = size;
			this.buf_start = 0L;
			this.buf_offset = (this.buf_length = 0);
			this.buf_dirty = false;
		}

		private string GetSecureFileName(string filename)
		{
			return (!this.anonymous) ? Path.GetFullPath(filename) : Path.GetFileName(filename);
		}

		private string GetSecureFileName(string filename, bool full)
		{
			return (!this.anonymous) ? ((!full) ? filename : Path.GetFullPath(filename)) : Path.GetFileName(filename);
		}

		internal const int DefaultBufferSize = 8192;

		private FileAccess access;

		private bool owner;

		private bool async;

		private bool canseek;

		private long append_startpos;

		private bool anonymous;

		private byte[] buf;

		private int buf_size;

		private int buf_length;

		private int buf_offset;

		private bool buf_dirty;

		private long buf_start;

		private string name;

		private IntPtr handle;

		private SafeFileHandle safeHandle;

		private delegate int ReadDelegate(byte[] buffer, int offset, int count);

		private delegate void WriteDelegate(byte[] buffer, int offset, int count);
	}
}
