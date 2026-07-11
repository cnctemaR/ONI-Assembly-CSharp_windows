using System;

namespace System
{
	internal class NullConsoleDriver : IConsoleDriver
	{
		public ConsoleColor BackgroundColor
		{
			get
			{
				return ConsoleColor.Black;
			}
			set
			{
			}
		}

		public int BufferHeight
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public int BufferWidth
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public bool CapsLock
		{
			get
			{
				return false;
			}
		}

		public int CursorLeft
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public int CursorSize
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public int CursorTop
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public bool CursorVisible
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public ConsoleColor ForegroundColor
		{
			get
			{
				return ConsoleColor.Black;
			}
			set
			{
			}
		}

		public bool KeyAvailable
		{
			get
			{
				return false;
			}
		}

		public bool Initialized
		{
			get
			{
				return true;
			}
		}

		public int LargestWindowHeight
		{
			get
			{
				return 0;
			}
		}

		public int LargestWindowWidth
		{
			get
			{
				return 0;
			}
		}

		public bool NumberLock
		{
			get
			{
				return false;
			}
		}

		public string Title
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public bool TreatControlCAsInput
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public int WindowHeight
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public int WindowLeft
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public int WindowTop
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public int WindowWidth
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public void Beep(int frequency, int duration)
		{
		}

		public void Clear()
		{
		}

		public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor)
		{
		}

		public void Init()
		{
		}

		public string ReadLine()
		{
			return null;
		}

		public ConsoleKeyInfo ReadKey(bool intercept)
		{
			return NullConsoleDriver.EmptyConsoleKeyInfo;
		}

		public void ResetColor()
		{
		}

		public void SetBufferSize(int width, int height)
		{
		}

		public void SetCursorPosition(int left, int top)
		{
		}

		public void SetWindowPosition(int left, int top)
		{
		}

		public void SetWindowSize(int width, int height)
		{
		}

		private static readonly ConsoleKeyInfo EmptyConsoleKeyInfo = new ConsoleKeyInfo('\0', (ConsoleKey)0, false, false, false);
	}
}
