using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Unix.Native
{
	public class Stdlib
	{
		internal Stdlib()
		{
		}

		static Stdlib()
		{
			Array values = Enum.GetValues(typeof(Signum));
			Stdlib.registered_signals = new SignalHandler[(int)values.GetValue(values.Length - 1)];
		}

		public static Errno GetLastError()
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			return NativeConvert.ToErrno(lastWin32Error);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_SetLastError")]
		private static extern void SetLastError(int error);

		protected static void SetLastError(Errno error)
		{
			int num = NativeConvert.FromErrno(error);
			Stdlib.SetLastError(num);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_InvokeSignalHandler")]
		internal static extern void InvokeSignalHandler(int signum, IntPtr handler);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_SIG_DFL")]
		private static extern IntPtr GetDefaultSignal();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_SIG_ERR")]
		private static extern IntPtr GetErrorSignal();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_SIG_IGN")]
		private static extern IntPtr GetIgnoreSignal();

		private static void _ErrorHandler(int signum)
		{
			Console.Error.WriteLine("Error handler invoked for signum " + signum + ".  Don't do that.");
		}

		private static void _DefaultHandler(int signum)
		{
			Console.Error.WriteLine("Default handler invoked for signum " + signum + ".  Don't do that.");
		}

		private static void _IgnoreHandler(int signum)
		{
			Console.Error.WriteLine("Ignore handler invoked for signum " + signum + ".  Don't do that.");
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "signal", SetLastError = true)]
		private static extern IntPtr sys_signal(int signum, SignalHandler handler);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "signal", SetLastError = true)]
		private static extern IntPtr sys_signal(int signum, IntPtr handler);

		[CLSCompliant(false)]
		[Obsolete("This is not safe; use Mono.Unix.UnixSignal for signal delivery or SetSignalAction()")]
		public static SignalHandler signal(Signum signum, SignalHandler handler)
		{
			int num = NativeConvert.FromSignum(signum);
			Delegate[] invocationList = handler.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Marshal.Prelink(invocationList[i].Method);
			}
			SignalHandler[] array = Stdlib.registered_signals;
			lock (array)
			{
				Stdlib.registered_signals[(int)signum] = handler;
			}
			IntPtr intPtr;
			if (handler == Stdlib.SIG_DFL)
			{
				intPtr = Stdlib.sys_signal(num, Stdlib._SIG_DFL);
			}
			else if (handler == Stdlib.SIG_ERR)
			{
				intPtr = Stdlib.sys_signal(num, Stdlib._SIG_ERR);
			}
			else if (handler == Stdlib.SIG_IGN)
			{
				intPtr = Stdlib.sys_signal(num, Stdlib._SIG_IGN);
			}
			else
			{
				intPtr = Stdlib.sys_signal(num, handler);
			}
			return Stdlib.TranslateHandler(intPtr);
		}

		private static SignalHandler TranslateHandler(IntPtr handler)
		{
			if (handler == Stdlib._SIG_DFL)
			{
				return Stdlib.SIG_DFL;
			}
			if (handler == Stdlib._SIG_ERR)
			{
				return Stdlib.SIG_ERR;
			}
			if (handler == Stdlib._SIG_IGN)
			{
				return Stdlib.SIG_IGN;
			}
			return (SignalHandler)Marshal.GetDelegateForFunctionPointer(handler, typeof(SignalHandler));
		}

		public static int SetSignalAction(Signum signal, SignalAction action)
		{
			return Stdlib.SetSignalAction(NativeConvert.FromSignum(signal), action);
		}

		public static int SetSignalAction(RealTimeSignum rts, SignalAction action)
		{
			return Stdlib.SetSignalAction(NativeConvert.FromRealTimeSignum(rts), action);
		}

		private static int SetSignalAction(int signum, SignalAction action)
		{
			IntPtr intPtr = IntPtr.Zero;
			switch (action)
			{
			case SignalAction.Default:
				intPtr = Stdlib._SIG_DFL;
				break;
			case SignalAction.Ignore:
				intPtr = Stdlib._SIG_IGN;
				break;
			case SignalAction.Error:
				intPtr = Stdlib._SIG_ERR;
				break;
			default:
				throw new ArgumentException("Invalid action value.", "action");
			}
			IntPtr intPtr2 = Stdlib.sys_signal(signum, intPtr);
			if (intPtr2 == Stdlib._SIG_ERR)
			{
				return -1;
			}
			return 0;
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "raise")]
		private static extern int sys_raise(int sig);

		[CLSCompliant(false)]
		public static int raise(Signum sig)
		{
			return Stdlib.sys_raise(NativeConvert.FromSignum(sig));
		}

		public static int raise(RealTimeSignum rts)
		{
			return Stdlib.sys_raise(NativeConvert.FromRealTimeSignum(rts));
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib__IOFBF")]
		private static extern int GetFullyBuffered();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib__IOLBF")]
		private static extern int GetLineBuffered();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib__IONBF")]
		private static extern int GetNonBuffered();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_BUFSIZ")]
		private static extern int GetBufferSize();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_CreateFilePosition")]
		internal static extern IntPtr CreateFilePosition();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_DumpFilePosition")]
		internal static extern int DumpFilePosition(StringBuilder buf, HandleRef handle, int len);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_EOF")]
		private static extern int GetEOF();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_FILENAME_MAX")]
		private static extern int GetFilenameMax();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_FOPEN_MAX")]
		private static extern int GetFopenMax();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_L_tmpnam")]
		private static extern int GetTmpnamLength();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_stdin")]
		private static extern IntPtr GetStandardInput();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_stdout")]
		private static extern IntPtr GetStandardOutput();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_stderr")]
		private static extern IntPtr GetStandardError();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_TMP_MAX")]
		private static extern int GetTmpMax();

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int remove([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string filename);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int rename([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string oldpath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string newpath);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern IntPtr tmpfile();

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "tmpnam", SetLastError = true)]
		private static extern IntPtr sys_tmpnam(StringBuilder s);

		[Obsolete("Syscall.mkstemp() should be preferred.")]
		public static string tmpnam(StringBuilder s)
		{
			if (s != null && s.Capacity < Stdlib.L_tmpnam)
			{
				throw new ArgumentOutOfRangeException("s", "s.Capacity < L_tmpnam");
			}
			object obj = Stdlib.tmpnam_lock;
			string text;
			lock (obj)
			{
				IntPtr intPtr = Stdlib.sys_tmpnam(s);
				text = UnixMarshal.PtrToString(intPtr);
			}
			return text;
		}

		[Obsolete("Syscall.mkstemp() should be preferred.")]
		public static string tmpnam()
		{
			object obj = Stdlib.tmpnam_lock;
			string text;
			lock (obj)
			{
				IntPtr intPtr = Stdlib.sys_tmpnam(null);
				text = UnixMarshal.PtrToString(intPtr);
			}
			return text;
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int fclose(IntPtr stream);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int fflush(IntPtr stream);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern IntPtr fopen([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, string mode);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern IntPtr freopen([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, string mode, IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_setbuf", SetLastError = true)]
		public static extern int setbuf(IntPtr stream, IntPtr buf);

		[CLSCompliant(false)]
		public unsafe static int setbuf(IntPtr stream, byte* buf)
		{
			return Stdlib.setbuf(stream, (IntPtr)((void*)buf));
		}

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_setvbuf", SetLastError = true)]
		public static extern int setvbuf(IntPtr stream, IntPtr buf, int mode, ulong size);

		[CLSCompliant(false)]
		public unsafe static int setvbuf(IntPtr stream, byte* buf, int mode, ulong size)
		{
			return Stdlib.setvbuf(stream, (IntPtr)((void*)buf), mode, size);
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fprintf")]
		private static extern int sys_fprintf(IntPtr stream, string format, string message);

		public static int fprintf(IntPtr stream, string message)
		{
			return Stdlib.sys_fprintf(stream, "%s", message);
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse fprintf (IntPtr, string) instead.")]
		public static int fprintf(IntPtr stream, string format, params object[] parameters)
		{
			object[] array = new object[checked(parameters.Length + 2)];
			array[0] = stream;
			array[1] = format;
			Array.Copy(parameters, 0, array, 2, parameters.Length);
			return (int)XPrintfFunctions.fprintf(array);
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "printf")]
		private static extern int sys_printf(string format, string message);

		public static int printf(string message)
		{
			return Stdlib.sys_printf("%s", message);
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse printf (string) instead.")]
		public static int printf(string format, params object[] parameters)
		{
			object[] array = new object[checked(parameters.Length + 1)];
			array[0] = format;
			Array.Copy(parameters, 0, array, 1, parameters.Length);
			return (int)XPrintfFunctions.printf(array);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_snprintf")]
		private static extern int sys_snprintf(StringBuilder s, ulong n, string format, string message);

		[CLSCompliant(false)]
		public static int snprintf(StringBuilder s, ulong n, string message)
		{
			if (n > (ulong)((long)s.Capacity))
			{
				throw new ArgumentOutOfRangeException("n", "n must be <= s.Capacity");
			}
			return Stdlib.sys_snprintf(s, n, "%s", message);
		}

		public static int snprintf(StringBuilder s, string message)
		{
			return Stdlib.sys_snprintf(s, (ulong)((long)s.Capacity), "%s", message);
		}

		[CLSCompliant(false)]
		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse snprintf (StringBuilder, string) instead.")]
		public static int snprintf(StringBuilder s, ulong n, string format, params object[] parameters)
		{
			if (n > (ulong)((long)s.Capacity))
			{
				throw new ArgumentOutOfRangeException("n", "n must be <= s.Capacity");
			}
			object[] array = new object[checked(parameters.Length + 3)];
			array[0] = s;
			array[1] = n;
			array[2] = format;
			Array.Copy(parameters, 0, array, 3, parameters.Length);
			return (int)XPrintfFunctions.snprintf(array);
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse snprintf (StringBuilder, string) instead.")]
		[CLSCompliant(false)]
		public static int snprintf(StringBuilder s, string format, params object[] parameters)
		{
			object[] array = new object[checked(parameters.Length + 3)];
			array[0] = s;
			array[1] = (ulong)((long)s.Capacity);
			array[2] = format;
			Array.Copy(parameters, 0, array, 3, parameters.Length);
			return (int)XPrintfFunctions.snprintf(array);
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int fgetc(IntPtr stream);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fgets", SetLastError = true)]
		private static extern IntPtr sys_fgets(StringBuilder sb, int size, IntPtr stream);

		public static StringBuilder fgets(StringBuilder sb, int size, IntPtr stream)
		{
			IntPtr intPtr = Stdlib.sys_fgets(sb, size, stream);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			return sb;
		}

		public static StringBuilder fgets(StringBuilder sb, IntPtr stream)
		{
			return Stdlib.fgets(sb, sb.Capacity, stream);
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int fputc(int c, IntPtr stream);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int fputs(string s, IntPtr stream);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int getc(IntPtr stream);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int getchar();

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int putc(int c, IntPtr stream);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int putchar(int c);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int puts(string s);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int ungetc(int c, IntPtr stream);

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fread", SetLastError = true)]
		public static extern ulong fread(IntPtr ptr, ulong size, ulong nmemb, IntPtr stream);

		[CLSCompliant(false)]
		public unsafe static ulong fread(void* ptr, ulong size, ulong nmemb, IntPtr stream)
		{
			return Stdlib.fread((IntPtr)ptr, size, nmemb, stream);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fread", SetLastError = true)]
		private static extern ulong sys_fread([Out] byte[] ptr, ulong size, ulong nmemb, IntPtr stream);

		[CLSCompliant(false)]
		public static ulong fread(byte[] ptr, ulong size, ulong nmemb, IntPtr stream)
		{
			if (size * nmemb > (ulong)((long)ptr.Length))
			{
				throw new ArgumentOutOfRangeException("nmemb");
			}
			return Stdlib.sys_fread(ptr, size, nmemb, stream);
		}

		[CLSCompliant(false)]
		public static ulong fread(byte[] ptr, IntPtr stream)
		{
			return Stdlib.fread(ptr, 1UL, (ulong)((long)ptr.Length), stream);
		}

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fwrite", SetLastError = true)]
		public static extern ulong fwrite(IntPtr ptr, ulong size, ulong nmemb, IntPtr stream);

		[CLSCompliant(false)]
		public unsafe static ulong fwrite(void* ptr, ulong size, ulong nmemb, IntPtr stream)
		{
			return Stdlib.fwrite((IntPtr)ptr, size, nmemb, stream);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fwrite", SetLastError = true)]
		private static extern ulong sys_fwrite(byte[] ptr, ulong size, ulong nmemb, IntPtr stream);

		[CLSCompliant(false)]
		public static ulong fwrite(byte[] ptr, ulong size, ulong nmemb, IntPtr stream)
		{
			if (size * nmemb > (ulong)((long)ptr.Length))
			{
				throw new ArgumentOutOfRangeException("nmemb");
			}
			return Stdlib.sys_fwrite(ptr, size, nmemb, stream);
		}

		[CLSCompliant(false)]
		public static ulong fwrite(byte[] ptr, IntPtr stream)
		{
			return Stdlib.fwrite(ptr, 1UL, (ulong)((long)ptr.Length), stream);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fgetpos", SetLastError = true)]
		private static extern int sys_fgetpos(IntPtr stream, HandleRef pos);

		public static int fgetpos(IntPtr stream, FilePosition pos)
		{
			return Stdlib.sys_fgetpos(stream, pos.Handle);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fseek", SetLastError = true)]
		private static extern int sys_fseek(IntPtr stream, long offset, int origin);

		[CLSCompliant(false)]
		public static int fseek(IntPtr stream, long offset, SeekFlags origin)
		{
			int num = (int)NativeConvert.FromSeekFlags(origin);
			return Stdlib.sys_fseek(stream, offset, num);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fsetpos", SetLastError = true)]
		private static extern int sys_fsetpos(IntPtr stream, HandleRef pos);

		public static int fsetpos(IntPtr stream, FilePosition pos)
		{
			return Stdlib.sys_fsetpos(stream, pos.Handle);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_ftell", SetLastError = true)]
		public static extern long ftell(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_rewind", SetLastError = true)]
		public static extern int rewind(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_clearerr", SetLastError = true)]
		public static extern int clearerr(IntPtr stream);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		public static extern int feof(IntPtr stream);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ferror(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_perror", SetLastError = true)]
		private static extern int perror(string s, int err);

		public static int perror(string s)
		{
			return Stdlib.perror(s, Marshal.GetLastWin32Error());
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_EXIT_FAILURE")]
		private static extern int GetExitFailure();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_EXIT_SUCCESS")]
		private static extern int GetExitSuccess();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_MB_CUR_MAX")]
		private static extern int GetMbCurMax();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_RAND_MAX")]
		private static extern int GetRandMax();

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		public static extern int rand();

		[CLSCompliant(false)]
		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		public static extern void srand(uint seed);

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_calloc", SetLastError = true)]
		public static extern IntPtr calloc(ulong nmemb, ulong size);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		public static extern void free(IntPtr ptr);

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_malloc", SetLastError = true)]
		public static extern IntPtr malloc(ulong size);

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_realloc", SetLastError = true)]
		public static extern IntPtr realloc(IntPtr ptr, ulong size);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		public static extern void abort();

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		public static extern void exit(int status);

		[CLSCompliant(false)]
		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		public static extern void _Exit(int status);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "getenv")]
		private static extern IntPtr sys_getenv(string name);

		public static string getenv(string name)
		{
			IntPtr intPtr = Stdlib.sys_getenv(name);
			return UnixMarshal.PtrToString(intPtr);
		}

		[CLSCompliant(false)]
		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int system(string @string);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "strerror", SetLastError = true)]
		private static extern IntPtr sys_strerror(int errnum);

		[CLSCompliant(false)]
		public static string strerror(Errno errnum)
		{
			int num = NativeConvert.FromErrno(errnum);
			object obj = Stdlib.strerror_lock;
			string text;
			lock (obj)
			{
				IntPtr intPtr = Stdlib.sys_strerror(num);
				text = UnixMarshal.PtrToString(intPtr);
			}
			return text;
		}

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_strlen", SetLastError = true)]
		public static extern ulong strlen(IntPtr s);

		internal const string LIBC = "msvcrt";

		internal const string MPH = "MonoPosixHelper";

		private static readonly IntPtr _SIG_DFL = Stdlib.GetDefaultSignal();

		private static readonly IntPtr _SIG_ERR = Stdlib.GetErrorSignal();

		private static readonly IntPtr _SIG_IGN = Stdlib.GetIgnoreSignal();

		[CLSCompliant(false)]
		public static readonly SignalHandler SIG_DFL = new SignalHandler(Stdlib._DefaultHandler);

		[CLSCompliant(false)]
		public static readonly SignalHandler SIG_ERR = new SignalHandler(Stdlib._ErrorHandler);

		[CLSCompliant(false)]
		public static readonly SignalHandler SIG_IGN = new SignalHandler(Stdlib._IgnoreHandler);

		private static readonly SignalHandler[] registered_signals;

		[CLSCompliant(false)]
		public static readonly int _IOFBF = Stdlib.GetFullyBuffered();

		[CLSCompliant(false)]
		public static readonly int _IOLBF = Stdlib.GetLineBuffered();

		[CLSCompliant(false)]
		public static readonly int _IONBF = Stdlib.GetNonBuffered();

		[CLSCompliant(false)]
		public static readonly int BUFSIZ = Stdlib.GetBufferSize();

		[CLSCompliant(false)]
		public static readonly int EOF = Stdlib.GetEOF();

		[CLSCompliant(false)]
		public static readonly int FOPEN_MAX = Stdlib.GetFopenMax();

		[CLSCompliant(false)]
		public static readonly int FILENAME_MAX = Stdlib.GetFilenameMax();

		[CLSCompliant(false)]
		public static readonly int L_tmpnam = Stdlib.GetTmpnamLength();

		public static readonly IntPtr stderr = Stdlib.GetStandardError();

		public static readonly IntPtr stdin = Stdlib.GetStandardInput();

		public static readonly IntPtr stdout = Stdlib.GetStandardOutput();

		[CLSCompliant(false)]
		public static readonly int TMP_MAX = Stdlib.GetTmpMax();

		private static object tmpnam_lock = new object();

		[CLSCompliant(false)]
		public static readonly int EXIT_FAILURE = Stdlib.GetExitFailure();

		[CLSCompliant(false)]
		public static readonly int EXIT_SUCCESS = Stdlib.GetExitSuccess();

		[CLSCompliant(false)]
		public static readonly int MB_CUR_MAX = Stdlib.GetMbCurMax();

		[CLSCompliant(false)]
		public static readonly int RAND_MAX = Stdlib.GetRandMax();

		private static object strerror_lock = new object();
	}
}
