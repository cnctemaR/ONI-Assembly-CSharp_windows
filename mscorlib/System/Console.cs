using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace System
{
	public static class Console
	{
		static Console()
		{
			if (Environment.IsRunningOnWindows)
			{
				try
				{
					Console.inputEncoding = Encoding.GetEncoding(Console.WindowsConsole.GetInputCodePage());
					Console.outputEncoding = Encoding.GetEncoding(Console.WindowsConsole.GetOutputCodePage());
				}
				catch
				{
					Console.inputEncoding = (Console.outputEncoding = Encoding.Default);
				}
			}
			else
			{
				int num = 0;
				Encoding.InternalCodePage(ref num);
				if (num != -1 && ((num & 268435455) == 3 || (num & 268435456) != 0))
				{
					Console.inputEncoding = (Console.outputEncoding = Encoding.UTF8Unmarked);
				}
				else
				{
					Console.inputEncoding = (Console.outputEncoding = Encoding.Default);
				}
			}
			Console.SetEncodings(Console.inputEncoding, Console.outputEncoding);
		}

		public static event ConsoleCancelEventHandler CancelKeyPress
		{
			add
			{
				if (!ConsoleDriver.Initialized)
				{
					ConsoleDriver.Init();
				}
				Console.cancel_event = (ConsoleCancelEventHandler)Delegate.Combine(Console.cancel_event, value);
			}
			remove
			{
				if (!ConsoleDriver.Initialized)
				{
					ConsoleDriver.Init();
				}
				Console.cancel_event = (ConsoleCancelEventHandler)Delegate.Remove(Console.cancel_event, value);
			}
		}

		private static void SetEncodings(Encoding inputEncoding, Encoding outputEncoding)
		{
			Console.stderr = new UnexceptionalStreamWriter(Console.OpenStandardError(0), outputEncoding);
			((StreamWriter)Console.stderr).AutoFlush = true;
			Console.stderr = TextWriter.Synchronized(Console.stderr, true);
			if (!Environment.IsRunningOnWindows && ConsoleDriver.IsConsole)
			{
				Console.stdout = TextWriter.Synchronized(new CStreamWriter(Console.OpenStandardOutput(0), outputEncoding)
				{
					AutoFlush = true
				}, true);
				Console.stdin = new CStreamReader(Console.OpenStandardInput(0), inputEncoding);
			}
			else
			{
				Console.stdout = new UnexceptionalStreamWriter(Console.OpenStandardOutput(0), outputEncoding);
				((StreamWriter)Console.stdout).AutoFlush = true;
				Console.stdout = TextWriter.Synchronized(Console.stdout, true);
				Console.stdin = new UnexceptionalStreamReader(Console.OpenStandardInput(0), inputEncoding);
				Console.stdin = TextReader.Synchronized(Console.stdin);
			}
			GC.SuppressFinalize(Console.stdout);
			GC.SuppressFinalize(Console.stderr);
			GC.SuppressFinalize(Console.stdin);
		}

		public static TextWriter Error
		{
			get
			{
				return Console.stderr;
			}
		}

		public static TextWriter Out
		{
			get
			{
				return Console.stdout;
			}
		}

		public static TextReader In
		{
			get
			{
				return Console.stdin;
			}
		}

		public static Stream OpenStandardError()
		{
			return Console.OpenStandardError(0);
		}

		private static Stream Open(IntPtr handle, FileAccess access, int bufferSize)
		{
			Stream stream;
			try
			{
				stream = new FileStream(handle, access, false, bufferSize, false, bufferSize == 0);
			}
			catch (IOException)
			{
				stream = new NullStream();
			}
			return stream;
		}

		[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public static Stream OpenStandardError(int bufferSize)
		{
			return Console.Open(MonoIO.ConsoleError, FileAccess.Write, bufferSize);
		}

		public static Stream OpenStandardInput()
		{
			return Console.OpenStandardInput(0);
		}

		[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public static Stream OpenStandardInput(int bufferSize)
		{
			return Console.Open(MonoIO.ConsoleInput, FileAccess.Read, bufferSize);
		}

		public static Stream OpenStandardOutput()
		{
			return Console.OpenStandardOutput(0);
		}

		[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public static Stream OpenStandardOutput(int bufferSize)
		{
			return Console.Open(MonoIO.ConsoleOutput, FileAccess.Write, bufferSize);
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public static void SetError(TextWriter newError)
		{
			if (newError == null)
			{
				throw new ArgumentNullException("newError");
			}
			Console.stderr = newError;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public static void SetIn(TextReader newIn)
		{
			if (newIn == null)
			{
				throw new ArgumentNullException("newIn");
			}
			Console.stdin = newIn;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public static void SetOut(TextWriter newOut)
		{
			if (newOut == null)
			{
				throw new ArgumentNullException("newOut");
			}
			Console.stdout = newOut;
		}

		public static void Write(bool value)
		{
			Console.stdout.Write(value);
		}

		public static void Write(char value)
		{
			Console.stdout.Write(value);
		}

		public static void Write(char[] buffer)
		{
			Console.stdout.Write(buffer);
		}

		public static void Write(decimal value)
		{
			Console.stdout.Write(value);
		}

		public static void Write(double value)
		{
			Console.stdout.Write(value);
		}

		public static void Write(int value)
		{
			Console.stdout.Write(value);
		}

		public static void Write(long value)
		{
			Console.stdout.Write(value);
		}

		public static void Write(object value)
		{
			Console.stdout.Write(value);
		}

		public static void Write(float value)
		{
			Console.stdout.Write(value);
		}

		public static void Write(string value)
		{
			Console.stdout.Write(value);
		}

		[CLSCompliant(false)]
		public static void Write(uint value)
		{
			Console.stdout.Write(value);
		}

		[CLSCompliant(false)]
		public static void Write(ulong value)
		{
			Console.stdout.Write(value);
		}

		public static void Write(string format, object arg0)
		{
			Console.stdout.Write(format, arg0);
		}

		public static void Write(string format, params object[] arg)
		{
			Console.stdout.Write(format, arg);
		}

		public static void Write(char[] buffer, int index, int count)
		{
			Console.stdout.Write(buffer, index, count);
		}

		public static void Write(string format, object arg0, object arg1)
		{
			Console.stdout.Write(format, arg0, arg1);
		}

		public static void Write(string format, object arg0, object arg1, object arg2)
		{
			Console.stdout.Write(format, arg0, arg1, arg2);
		}

		[CLSCompliant(false)]
		public static void Write(string format, object arg0, object arg1, object arg2, object arg3, __arglist)
		{
			ArgIterator argIterator = new ArgIterator(__arglist);
			int remainingCount = argIterator.GetRemainingCount();
			object[] array = new object[remainingCount + 4];
			array[0] = arg0;
			array[1] = arg1;
			array[2] = arg2;
			array[3] = arg3;
			for (int i = 0; i < remainingCount; i++)
			{
				TypedReference nextArg = argIterator.GetNextArg();
				array[i + 4] = TypedReference.ToObject(nextArg);
			}
			Console.stdout.Write(string.Format(format, array));
		}

		public static void WriteLine()
		{
			Console.stdout.WriteLine();
		}

		public static void WriteLine(bool value)
		{
			Console.stdout.WriteLine(value);
		}

		public static void WriteLine(char value)
		{
			Console.stdout.WriteLine(value);
		}

		public static void WriteLine(char[] buffer)
		{
			Console.stdout.WriteLine(buffer);
		}

		public static void WriteLine(decimal value)
		{
			Console.stdout.WriteLine(value);
		}

		public static void WriteLine(double value)
		{
			Console.stdout.WriteLine(value);
		}

		public static void WriteLine(int value)
		{
			Console.stdout.WriteLine(value);
		}

		public static void WriteLine(long value)
		{
			Console.stdout.WriteLine(value);
		}

		public static void WriteLine(object value)
		{
			Console.stdout.WriteLine(value);
		}

		public static void WriteLine(float value)
		{
			Console.stdout.WriteLine(value);
		}

		public static void WriteLine(string value)
		{
			Console.stdout.WriteLine(value);
		}

		[CLSCompliant(false)]
		public static void WriteLine(uint value)
		{
			Console.stdout.WriteLine(value);
		}

		[CLSCompliant(false)]
		public static void WriteLine(ulong value)
		{
			Console.stdout.WriteLine(value);
		}

		public static void WriteLine(string format, object arg0)
		{
			Console.stdout.WriteLine(format, arg0);
		}

		public static void WriteLine(string format, params object[] arg)
		{
			Console.stdout.WriteLine(format, arg);
		}

		public static void WriteLine(char[] buffer, int index, int count)
		{
			Console.stdout.WriteLine(buffer, index, count);
		}

		public static void WriteLine(string format, object arg0, object arg1)
		{
			Console.stdout.WriteLine(format, arg0, arg1);
		}

		public static void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			Console.stdout.WriteLine(format, arg0, arg1, arg2);
		}

		[CLSCompliant(false)]
		public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, __arglist)
		{
			ArgIterator argIterator = new ArgIterator(__arglist);
			int remainingCount = argIterator.GetRemainingCount();
			object[] array = new object[remainingCount + 4];
			array[0] = arg0;
			array[1] = arg1;
			array[2] = arg2;
			array[3] = arg3;
			for (int i = 0; i < remainingCount; i++)
			{
				TypedReference nextArg = argIterator.GetNextArg();
				array[i + 4] = TypedReference.ToObject(nextArg);
			}
			Console.stdout.WriteLine(string.Format(format, array));
		}

		public static int Read()
		{
			if (Console.stdin is CStreamReader && ConsoleDriver.IsConsole)
			{
				return ConsoleDriver.Read();
			}
			return Console.stdin.Read();
		}

		public static string ReadLine()
		{
			if (Console.stdin is CStreamReader && ConsoleDriver.IsConsole)
			{
				return ConsoleDriver.ReadLine();
			}
			return Console.stdin.ReadLine();
		}

		public static Encoding InputEncoding
		{
			get
			{
				return Console.inputEncoding;
			}
			set
			{
				Console.inputEncoding = value;
				Console.SetEncodings(Console.inputEncoding, Console.outputEncoding);
			}
		}

		public static Encoding OutputEncoding
		{
			get
			{
				return Console.outputEncoding;
			}
			set
			{
				Console.outputEncoding = value;
				Console.SetEncodings(Console.inputEncoding, Console.outputEncoding);
			}
		}

		public static ConsoleColor BackgroundColor
		{
			get
			{
				return ConsoleDriver.BackgroundColor;
			}
			set
			{
				ConsoleDriver.BackgroundColor = value;
			}
		}

		public static int BufferHeight
		{
			get
			{
				return ConsoleDriver.BufferHeight;
			}
			[MonoLimitation("Implemented only on Windows")]
			set
			{
				ConsoleDriver.BufferHeight = value;
			}
		}

		public static int BufferWidth
		{
			get
			{
				return ConsoleDriver.BufferWidth;
			}
			[MonoLimitation("Implemented only on Windows")]
			set
			{
				ConsoleDriver.BufferWidth = value;
			}
		}

		[MonoLimitation("Implemented only on Windows")]
		public static bool CapsLock
		{
			get
			{
				return ConsoleDriver.CapsLock;
			}
		}

		public static int CursorLeft
		{
			get
			{
				return ConsoleDriver.CursorLeft;
			}
			set
			{
				ConsoleDriver.CursorLeft = value;
			}
		}

		public static int CursorTop
		{
			get
			{
				return ConsoleDriver.CursorTop;
			}
			set
			{
				ConsoleDriver.CursorTop = value;
			}
		}

		public static int CursorSize
		{
			get
			{
				return ConsoleDriver.CursorSize;
			}
			set
			{
				ConsoleDriver.CursorSize = value;
			}
		}

		public static bool CursorVisible
		{
			get
			{
				return ConsoleDriver.CursorVisible;
			}
			set
			{
				ConsoleDriver.CursorVisible = value;
			}
		}

		public static ConsoleColor ForegroundColor
		{
			get
			{
				return ConsoleDriver.ForegroundColor;
			}
			set
			{
				ConsoleDriver.ForegroundColor = value;
			}
		}

		public static bool KeyAvailable
		{
			get
			{
				return ConsoleDriver.KeyAvailable;
			}
		}

		public static int LargestWindowHeight
		{
			get
			{
				return ConsoleDriver.LargestWindowHeight;
			}
		}

		public static int LargestWindowWidth
		{
			get
			{
				return ConsoleDriver.LargestWindowWidth;
			}
		}

		[MonoLimitation("Only works on windows")]
		public static bool NumberLock
		{
			get
			{
				return ConsoleDriver.NumberLock;
			}
		}

		public static string Title
		{
			get
			{
				return ConsoleDriver.Title;
			}
			set
			{
				ConsoleDriver.Title = value;
			}
		}

		public static bool TreatControlCAsInput
		{
			get
			{
				return ConsoleDriver.TreatControlCAsInput;
			}
			set
			{
				ConsoleDriver.TreatControlCAsInput = value;
			}
		}

		[MonoLimitation("Only works on windows")]
		public static int WindowHeight
		{
			get
			{
				return ConsoleDriver.WindowHeight;
			}
			set
			{
				ConsoleDriver.WindowHeight = value;
			}
		}

		[MonoLimitation("Only works on windows")]
		public static int WindowLeft
		{
			get
			{
				return ConsoleDriver.WindowLeft;
			}
			set
			{
				ConsoleDriver.WindowLeft = value;
			}
		}

		[MonoLimitation("Only works on windows")]
		public static int WindowTop
		{
			get
			{
				return ConsoleDriver.WindowTop;
			}
			set
			{
				ConsoleDriver.WindowTop = value;
			}
		}

		[MonoLimitation("Only works on windows")]
		public static int WindowWidth
		{
			get
			{
				return ConsoleDriver.WindowWidth;
			}
			set
			{
				ConsoleDriver.WindowWidth = value;
			}
		}

		public static void Beep()
		{
			Console.Beep(1000, 500);
		}

		public static void Beep(int frequency, int duration)
		{
			if (frequency < 37 || frequency > 32767)
			{
				throw new ArgumentOutOfRangeException("frequency");
			}
			if (duration <= 0)
			{
				throw new ArgumentOutOfRangeException("duration");
			}
			ConsoleDriver.Beep(frequency, duration);
		}

		public static void Clear()
		{
			ConsoleDriver.Clear();
		}

		[MonoLimitation("Implemented only on Windows")]
		public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
		{
			ConsoleDriver.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);
		}

		[MonoLimitation("Implemented only on Windows")]
		public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor)
		{
			ConsoleDriver.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, sourceChar, sourceForeColor, sourceBackColor);
		}

		public static ConsoleKeyInfo ReadKey()
		{
			return Console.ReadKey(false);
		}

		public static ConsoleKeyInfo ReadKey(bool intercept)
		{
			return ConsoleDriver.ReadKey(intercept);
		}

		public static void ResetColor()
		{
			ConsoleDriver.ResetColor();
		}

		[MonoLimitation("Only works on windows")]
		public static void SetBufferSize(int width, int height)
		{
			ConsoleDriver.SetBufferSize(width, height);
		}

		public static void SetCursorPosition(int left, int top)
		{
			ConsoleDriver.SetCursorPosition(left, top);
		}

		public static void SetWindowPosition(int left, int top)
		{
			ConsoleDriver.SetWindowPosition(left, top);
		}

		public static void SetWindowSize(int width, int height)
		{
			ConsoleDriver.SetWindowSize(width, height);
		}

		internal static void DoConsoleCancelEvent()
		{
			bool flag = true;
			if (Console.cancel_event != null)
			{
				ConsoleCancelEventArgs e = new ConsoleCancelEventArgs(ConsoleSpecialKey.ControlC);
				Delegate[] invocationList = Console.cancel_event.GetInvocationList();
				foreach (ConsoleCancelEventHandler consoleCancelEventHandler in invocationList)
				{
					try
					{
						consoleCancelEventHandler(null, e);
					}
					catch
					{
					}
				}
				flag = !e.Cancel;
			}
			if (flag)
			{
				Environment.Exit(58);
			}
		}

		internal static TextWriter stdout;

		private static TextWriter stderr;

		private static TextReader stdin;

		private static Encoding inputEncoding;

		private static Encoding outputEncoding;

		private static ConsoleCancelEventHandler cancel_event;

		private static readonly Console.InternalCancelHandler cancel_handler = new Console.InternalCancelHandler(Console.DoConsoleCancelEvent);

		private class WindowsConsole
		{
			[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			private static extern int GetConsoleCP();

			[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			private static extern int GetConsoleOutputCP();

			[MethodImpl(MethodImplOptions.NoInlining)]
			public static int GetInputCodePage()
			{
				return Console.WindowsConsole.GetConsoleCP();
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public static int GetOutputCodePage()
			{
				return Console.WindowsConsole.GetConsoleOutputCP();
			}
		}

		private delegate void InternalCancelHandler();
	}
}
