using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixMarshal
	{
		private UnixMarshal()
		{
		}

		[CLSCompliant(false)]
		public static string GetErrorDescription(Errno errno)
		{
			return ErrorMarshal.Translate(errno);
		}

		public static IntPtr AllocHeap(long size)
		{
			if (size < 0L)
			{
				throw new ArgumentOutOfRangeException("size", "< 0");
			}
			return Stdlib.malloc((ulong)size);
		}

		public static IntPtr ReAllocHeap(IntPtr ptr, long size)
		{
			if (size < 0L)
			{
				throw new ArgumentOutOfRangeException("size", "< 0");
			}
			return Stdlib.realloc(ptr, (ulong)size);
		}

		public static void FreeHeap(IntPtr ptr)
		{
			Stdlib.free(ptr);
		}

		public unsafe static string PtrToStringUnix(IntPtr p)
		{
			if (p == IntPtr.Zero)
			{
				return null;
			}
			int num = checked((int)Stdlib.strlen(p));
			return new string((sbyte*)(void*)p, 0, num, UnixEncoding.Instance);
		}

		public static string PtrToString(IntPtr p)
		{
			if (p == IntPtr.Zero)
			{
				return null;
			}
			return UnixMarshal.PtrToString(p, UnixEncoding.Instance);
		}

		public unsafe static string PtrToString(IntPtr p, Encoding encoding)
		{
			if (p == IntPtr.Zero)
			{
				return null;
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			int num = UnixMarshal.GetStringByteLength(p, encoding);
			string text = new string((sbyte*)(void*)p, 0, num, encoding);
			num = text.Length;
			while (num > 0 && text[num - 1] == '\0')
			{
				num--;
			}
			if (num == text.Length)
			{
				return text;
			}
			return text.Substring(0, num);
		}

		private static int GetStringByteLength(IntPtr p, Encoding encoding)
		{
			Type type = encoding.GetType();
			int num;
			if (typeof(UTF8Encoding).IsAssignableFrom(type) || typeof(UTF7Encoding).IsAssignableFrom(type) || typeof(UnixEncoding).IsAssignableFrom(type) || typeof(ASCIIEncoding).IsAssignableFrom(type))
			{
				num = checked((int)Stdlib.strlen(p));
			}
			else if (typeof(UnicodeEncoding).IsAssignableFrom(type))
			{
				num = UnixMarshal.GetInt16BufferLength(p);
			}
			else
			{
				num = UnixMarshal.GetRandomBufferLength(p, encoding.GetMaxByteCount(1));
			}
			if (num == -1)
			{
				throw new NotSupportedException("Unable to determine native string buffer length");
			}
			return num;
		}

		private static int GetInt16BufferLength(IntPtr p)
		{
			int num = 0;
			checked
			{
				while (Marshal.ReadInt16(p, unchecked(num * 2)) != 0)
				{
					num++;
				}
				return num * 2;
			}
		}

		private static int GetInt32BufferLength(IntPtr p)
		{
			int num = 0;
			checked
			{
				while (Marshal.ReadInt32(p, unchecked(num * 4)) != 0)
				{
					num++;
				}
				return num * 4;
			}
		}

		private static int GetRandomBufferLength(IntPtr p, int nullLength)
		{
			switch (nullLength)
			{
			case 1:
				return checked((int)Stdlib.strlen(p));
			case 2:
				return UnixMarshal.GetInt16BufferLength(p);
			case 4:
				return UnixMarshal.GetInt32BufferLength(p);
			}
			int num = 0;
			int num2 = 0;
			do
			{
				if (Marshal.ReadByte(p, num++) == 0)
				{
					num2++;
				}
				else
				{
					num2 = 0;
				}
			}
			while (num2 != nullLength);
			return num;
		}

		public static string[] PtrToStringArray(IntPtr stringArray)
		{
			return UnixMarshal.PtrToStringArray(stringArray, UnixEncoding.Instance);
		}

		public static string[] PtrToStringArray(IntPtr stringArray, Encoding encoding)
		{
			if (stringArray == IntPtr.Zero)
			{
				return new string[0];
			}
			int num = UnixMarshal.CountStrings(stringArray);
			return UnixMarshal.PtrToStringArray(num, stringArray, encoding);
		}

		private static int CountStrings(IntPtr stringArray)
		{
			int num = 0;
			while (Marshal.ReadIntPtr(stringArray, num * IntPtr.Size) != IntPtr.Zero)
			{
				num++;
			}
			return num;
		}

		public static string[] PtrToStringArray(int count, IntPtr stringArray)
		{
			return UnixMarshal.PtrToStringArray(count, stringArray, UnixEncoding.Instance);
		}

		public static string[] PtrToStringArray(int count, IntPtr stringArray, Encoding encoding)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (stringArray == IntPtr.Zero)
			{
				return new string[count];
			}
			string[] array = new string[count];
			for (int i = 0; i < count; i++)
			{
				IntPtr intPtr = Marshal.ReadIntPtr(stringArray, i * IntPtr.Size);
				array[i] = UnixMarshal.PtrToString(intPtr, encoding);
			}
			return array;
		}

		public static IntPtr StringToHeap(string s)
		{
			return UnixMarshal.StringToHeap(s, UnixEncoding.Instance);
		}

		public static IntPtr StringToHeap(string s, Encoding encoding)
		{
			return UnixMarshal.StringToHeap(s, 0, s.Length, encoding);
		}

		public static IntPtr StringToHeap(string s, int index, int count)
		{
			return UnixMarshal.StringToHeap(s, index, count, UnixEncoding.Instance);
		}

		public static IntPtr StringToHeap(string s, int index, int count, Encoding encoding)
		{
			if (s == null)
			{
				return IntPtr.Zero;
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			int maxByteCount = encoding.GetMaxByteCount(1);
			char[] array = s.ToCharArray(index, count);
			byte[] array2 = new byte[encoding.GetByteCount(array) + maxByteCount];
			int bytes = encoding.GetBytes(array, 0, array.Length, array2, 0);
			if (bytes != array2.Length - maxByteCount)
			{
				throw new NotSupportedException("encoding.GetBytes() doesn't equal encoding.GetByteCount()!");
			}
			IntPtr intPtr = UnixMarshal.AllocHeap((long)array2.Length);
			if (intPtr == IntPtr.Zero)
			{
				throw new UnixIOException(Errno.ENOMEM);
			}
			bool flag = false;
			try
			{
				Marshal.Copy(array2, 0, intPtr, array2.Length);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					UnixMarshal.FreeHeap(intPtr);
				}
			}
			return intPtr;
		}

		public static bool ShouldRetrySyscall(int r)
		{
			return r == -1 && Stdlib.GetLastError() == Errno.EINTR;
		}

		[CLSCompliant(false)]
		public static bool ShouldRetrySyscall(int r, out Errno errno)
		{
			errno = (Errno)0;
			return r == -1 && (errno = Stdlib.GetLastError()) == Errno.EINTR;
		}

		internal static string EscapeFormatString(string message, char[] permitted)
		{
			if (message == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(message.Length);
			for (int i = 0; i < message.Length; i++)
			{
				char c = message[i];
				stringBuilder.Append(c);
				if (c == '%' && i + 1 < message.Length)
				{
					char c2 = message[i + 1];
					if (c2 == '%' || UnixMarshal.IsCharPresent(permitted, c2))
					{
						stringBuilder.Append(c2);
					}
					else
					{
						stringBuilder.Append('%').Append(c2);
					}
					i++;
				}
				else if (c == '%')
				{
					stringBuilder.Append('%');
				}
			}
			return stringBuilder.ToString();
		}

		private static bool IsCharPresent(char[] array, char c)
		{
			if (array == null)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == c)
				{
					return true;
				}
			}
			return false;
		}

		internal static Exception CreateExceptionForError(Errno errno)
		{
			string errorDescription = UnixMarshal.GetErrorDescription(errno);
			UnixIOException ex = new UnixIOException(errno);
			switch (errno)
			{
			case Errno.EPERM:
				break;
			case Errno.ENOENT:
				return new FileNotFoundException(errorDescription, ex);
			default:
				switch (errno)
				{
				case Errno.ENOSPC:
				case Errno.ESPIPE:
				case Errno.EROFS:
				case Errno.ENOTEMPTY:
					goto IL_00ED;
				default:
					if (errno == Errno.EOVERFLOW)
					{
						return new OverflowException(errorDescription, ex);
					}
					if (errno != Errno.EOPNOTSUPP)
					{
						return ex;
					}
					break;
				case Errno.ERANGE:
					return new ArgumentOutOfRangeException(errorDescription);
				case Errno.ENAMETOOLONG:
					return new PathTooLongException(errorDescription, ex);
				}
				break;
			case Errno.EIO:
			case Errno.ENXIO:
				goto IL_00ED;
			case Errno.ENOEXEC:
				return new InvalidProgramException(errorDescription, ex);
			case Errno.EBADF:
			case Errno.EINVAL:
				return new ArgumentException(errorDescription, ex);
			case Errno.EACCES:
			case Errno.EISDIR:
				return new UnauthorizedAccessException(errorDescription, ex);
			case Errno.EFAULT:
				return new NullReferenceException(errorDescription, ex);
			case Errno.ENOTDIR:
				return new DirectoryNotFoundException(errorDescription, ex);
			}
			return new InvalidOperationException(errorDescription, ex);
			IL_00ED:
			return new IOException(errorDescription, ex);
		}

		internal static Exception CreateExceptionForLastError()
		{
			return UnixMarshal.CreateExceptionForError(Stdlib.GetLastError());
		}

		[CLSCompliant(false)]
		public static void ThrowExceptionForError(Errno errno)
		{
			throw UnixMarshal.CreateExceptionForError(errno);
		}

		public static void ThrowExceptionForLastError()
		{
			throw UnixMarshal.CreateExceptionForLastError();
		}

		[CLSCompliant(false)]
		public static void ThrowExceptionForErrorIf(int retval, Errno errno)
		{
			if (retval == -1)
			{
				UnixMarshal.ThrowExceptionForError(errno);
			}
		}

		public static void ThrowExceptionForLastErrorIf(int retval)
		{
			if (retval == -1)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
		}
	}
}
