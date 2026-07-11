using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class ConsoleDriver
	{
		static ConsoleDriver()
		{
			if (!ConsoleDriver.IsConsole)
			{
				ConsoleDriver.driver = ConsoleDriver.CreateNullConsoleDriver();
				return;
			}
			if (Environment.IsRunningOnWindows)
			{
				ConsoleDriver.driver = ConsoleDriver.CreateWindowsConsoleDriver();
				return;
			}
			string environmentVariable = Environment.GetEnvironmentVariable("TERM");
			if (environmentVariable == "dumb")
			{
				ConsoleDriver.is_console = false;
				ConsoleDriver.driver = ConsoleDriver.CreateNullConsoleDriver();
				return;
			}
			ConsoleDriver.driver = ConsoleDriver.CreateTermInfoDriver(environmentVariable);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static IConsoleDriver CreateNullConsoleDriver()
		{
			return new NullConsoleDriver();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static IConsoleDriver CreateWindowsConsoleDriver()
		{
			return new WindowsConsoleDriver();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static IConsoleDriver CreateTermInfoDriver(string term)
		{
			return new TermInfoDriver(term);
		}

		public static bool Initialized
		{
			get
			{
				return ConsoleDriver.driver.Initialized;
			}
		}

		public static ConsoleColor BackgroundColor
		{
			get
			{
				return ConsoleDriver.driver.BackgroundColor;
			}
			set
			{
				if (value < ConsoleColor.Black || value > ConsoleColor.White)
				{
					throw new ArgumentOutOfRangeException("value", "Not a ConsoleColor value.");
				}
				ConsoleDriver.driver.BackgroundColor = value;
			}
		}

		public static int BufferHeight
		{
			get
			{
				return ConsoleDriver.driver.BufferHeight;
			}
			set
			{
				ConsoleDriver.driver.BufferHeight = value;
			}
		}

		public static int BufferWidth
		{
			get
			{
				return ConsoleDriver.driver.BufferWidth;
			}
			set
			{
				ConsoleDriver.driver.BufferWidth = value;
			}
		}

		public static bool CapsLock
		{
			get
			{
				return ConsoleDriver.driver.CapsLock;
			}
		}

		public static int CursorLeft
		{
			get
			{
				return ConsoleDriver.driver.CursorLeft;
			}
			set
			{
				ConsoleDriver.driver.CursorLeft = value;
			}
		}

		public static int CursorSize
		{
			get
			{
				return ConsoleDriver.driver.CursorSize;
			}
			set
			{
				ConsoleDriver.driver.CursorSize = value;
			}
		}

		public static int CursorTop
		{
			get
			{
				return ConsoleDriver.driver.CursorTop;
			}
			set
			{
				ConsoleDriver.driver.CursorTop = value;
			}
		}

		public static bool CursorVisible
		{
			get
			{
				return ConsoleDriver.driver.CursorVisible;
			}
			set
			{
				ConsoleDriver.driver.CursorVisible = value;
			}
		}

		public static bool KeyAvailable
		{
			get
			{
				return ConsoleDriver.driver.KeyAvailable;
			}
		}

		public static ConsoleColor ForegroundColor
		{
			get
			{
				return ConsoleDriver.driver.ForegroundColor;
			}
			set
			{
				if (value < ConsoleColor.Black || value > ConsoleColor.White)
				{
					throw new ArgumentOutOfRangeException("value", "Not a ConsoleColor value.");
				}
				ConsoleDriver.driver.ForegroundColor = value;
			}
		}

		public static int LargestWindowHeight
		{
			get
			{
				return ConsoleDriver.driver.LargestWindowHeight;
			}
		}

		public static int LargestWindowWidth
		{
			get
			{
				return ConsoleDriver.driver.LargestWindowWidth;
			}
		}

		public static bool NumberLock
		{
			get
			{
				return ConsoleDriver.driver.NumberLock;
			}
		}

		public static string Title
		{
			get
			{
				return ConsoleDriver.driver.Title;
			}
			set
			{
				ConsoleDriver.driver.Title = value;
			}
		}

		public static bool TreatControlCAsInput
		{
			get
			{
				return ConsoleDriver.driver.TreatControlCAsInput;
			}
			set
			{
				ConsoleDriver.driver.TreatControlCAsInput = value;
			}
		}

		public static int WindowHeight
		{
			get
			{
				return ConsoleDriver.driver.WindowHeight;
			}
			set
			{
				ConsoleDriver.driver.WindowHeight = value;
			}
		}

		public static int WindowLeft
		{
			get
			{
				return ConsoleDriver.driver.WindowLeft;
			}
			set
			{
				ConsoleDriver.driver.WindowLeft = value;
			}
		}

		public static int WindowTop
		{
			get
			{
				return ConsoleDriver.driver.WindowTop;
			}
			set
			{
				ConsoleDriver.driver.WindowTop = value;
			}
		}

		public static int WindowWidth
		{
			get
			{
				return ConsoleDriver.driver.WindowWidth;
			}
			set
			{
				ConsoleDriver.driver.WindowWidth = value;
			}
		}

		public static bool IsErrorRedirected
		{
			get
			{
				return !ConsoleDriver.Isatty(MonoIO.ConsoleError);
			}
		}

		public static bool IsOutputRedirected
		{
			get
			{
				return !ConsoleDriver.Isatty(MonoIO.ConsoleOutput);
			}
		}

		public static bool IsInputRedirected
		{
			get
			{
				return !ConsoleDriver.Isatty(MonoIO.ConsoleInput);
			}
		}

		public static void Beep(int frequency, int duration)
		{
			ConsoleDriver.driver.Beep(frequency, duration);
		}

		public static void Clear()
		{
			ConsoleDriver.driver.Clear();
		}

		public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
		{
			ConsoleDriver.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, ' ', ConsoleColor.Black, ConsoleColor.Black);
		}

		public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor)
		{
			ConsoleDriver.driver.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, sourceChar, sourceForeColor, sourceBackColor);
		}

		public static void Init()
		{
			ConsoleDriver.driver.Init();
		}

		public static int Read()
		{
			return (int)ConsoleDriver.ReadKey(false).KeyChar;
		}

		public static string ReadLine()
		{
			return ConsoleDriver.driver.ReadLine();
		}

		public static ConsoleKeyInfo ReadKey(bool intercept)
		{
			return ConsoleDriver.driver.ReadKey(intercept);
		}

		public static void ResetColor()
		{
			ConsoleDriver.driver.ResetColor();
		}

		public static void SetBufferSize(int width, int height)
		{
			ConsoleDriver.driver.SetBufferSize(width, height);
		}

		public static void SetCursorPosition(int left, int top)
		{
			ConsoleDriver.driver.SetCursorPosition(left, top);
		}

		public static void SetWindowPosition(int left, int top)
		{
			ConsoleDriver.driver.SetWindowPosition(left, top);
		}

		public static void SetWindowSize(int width, int height)
		{
			ConsoleDriver.driver.SetWindowSize(width, height);
		}

		public static bool IsConsole
		{
			get
			{
				if (ConsoleDriver.called_isatty)
				{
					return ConsoleDriver.is_console;
				}
				ConsoleDriver.is_console = ConsoleDriver.Isatty(MonoIO.ConsoleOutput) && ConsoleDriver.Isatty(MonoIO.ConsoleInput);
				ConsoleDriver.called_isatty = true;
				return ConsoleDriver.is_console;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Isatty(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int InternalKeyAvailable(int ms_timeout);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern bool TtySetup(string keypadXmit, string teardown, out byte[] control_characters, out int* address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool SetEcho(bool wantEcho);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool SetBreak(bool wantBreak);

		internal static IConsoleDriver driver;

		private static bool is_console;

		private static bool called_isatty;
	}
}
