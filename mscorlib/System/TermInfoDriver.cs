using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
	internal class TermInfoDriver : IConsoleDriver
	{
		private static string TryTermInfoDir(string dir, string term)
		{
			string text = string.Format("{0}/{1:x}/{2}", dir, (int)term[0], term);
			if (File.Exists(text))
			{
				return text;
			}
			text = Path.Combine(dir, term.Substring(0, 1), term);
			if (File.Exists(text))
			{
				return text;
			}
			return null;
		}

		private static string SearchTerminfo(string term)
		{
			if (term == null || term == string.Empty)
			{
				return null;
			}
			string environmentVariable = Environment.GetEnvironmentVariable("TERMINFO");
			if (environmentVariable != null && Directory.Exists(environmentVariable))
			{
				string text = TermInfoDriver.TryTermInfoDir(environmentVariable, term);
				if (text != null)
				{
					return text;
				}
			}
			foreach (string text2 in TermInfoDriver.locations)
			{
				if (Directory.Exists(text2))
				{
					string text = TermInfoDriver.TryTermInfoDir(text2, term);
					if (text != null)
					{
						return text;
					}
				}
			}
			return null;
		}

		private void WriteConsole(string str)
		{
			if (str == null)
			{
				return;
			}
			this.stdout.InternalWriteString(str);
		}

		public TermInfoDriver()
			: this(Environment.GetEnvironmentVariable("TERM"))
		{
		}

		public TermInfoDriver(string term)
		{
			this.term = term;
			string text = TermInfoDriver.SearchTerminfo(term);
			if (text != null)
			{
				this.reader = new TermInfoReader(term, text);
			}
			else if (term == "xterm")
			{
				this.reader = new TermInfoReader(term, KnownTerminals.xterm);
			}
			else if (term == "linux")
			{
				this.reader = new TermInfoReader(term, KnownTerminals.linux);
			}
			if (this.reader == null)
			{
				this.reader = new TermInfoReader(term, KnownTerminals.ansi);
			}
			if (!(Console.stdout is CStreamWriter))
			{
				this.stdout = new CStreamWriter(Console.OpenStandardOutput(0), Console.OutputEncoding, false);
				this.stdout.AutoFlush = true;
				return;
			}
			this.stdout = (CStreamWriter)Console.stdout;
		}

		public bool Initialized
		{
			get
			{
				return this.inited;
			}
		}

		public void Init()
		{
			if (this.inited)
			{
				return;
			}
			object obj = this.initLock;
			lock (obj)
			{
				if (!this.inited)
				{
					this.inited = true;
					if (!ConsoleDriver.IsConsole)
					{
						throw new IOException("Not a tty.");
					}
					ConsoleDriver.SetEcho(false);
					string text = null;
					this.keypadXmit = this.reader.Get(TermInfoStrings.KeypadXmit);
					this.keypadLocal = this.reader.Get(TermInfoStrings.KeypadLocal);
					if (this.keypadXmit != null)
					{
						this.WriteConsole(this.keypadXmit);
						if (this.keypadLocal != null)
						{
							text += this.keypadLocal;
						}
					}
					this.origPair = this.reader.Get(TermInfoStrings.OrigPair);
					this.origColors = this.reader.Get(TermInfoStrings.OrigColors);
					this.setfgcolor = this.reader.Get(TermInfoStrings.SetAForeground);
					this.setbgcolor = this.reader.Get(TermInfoStrings.SetABackground);
					this.maxColors = this.reader.Get(TermInfoNumbers.MaxColors);
					this.maxColors = Math.Max(Math.Min(this.maxColors, 16), 1);
					string text2 = ((this.origColors == null) ? this.origPair : this.origColors);
					if (text2 != null)
					{
						text += text2;
					}
					if (!ConsoleDriver.TtySetup(this.keypadXmit, text, out this.control_characters, out TermInfoDriver.native_terminal_size))
					{
						this.control_characters = new byte[17];
						TermInfoDriver.native_terminal_size = null;
					}
					this.stdin = new StreamReader(Console.OpenStandardInput(0), Console.InputEncoding);
					this.clear = this.reader.Get(TermInfoStrings.ClearScreen);
					this.bell = this.reader.Get(TermInfoStrings.Bell);
					if (this.clear == null)
					{
						this.clear = this.reader.Get(TermInfoStrings.CursorHome);
						this.clear += this.reader.Get(TermInfoStrings.ClrEos);
					}
					this.csrVisible = this.reader.Get(TermInfoStrings.CursorNormal);
					if (this.csrVisible == null)
					{
						this.csrVisible = this.reader.Get(TermInfoStrings.CursorVisible);
					}
					this.csrInvisible = this.reader.Get(TermInfoStrings.CursorInvisible);
					if (this.term == "cygwin" || this.term == "linux" || (this.term != null && this.term.StartsWith("xterm")) || this.term == "rxvt" || this.term == "dtterm")
					{
						this.titleFormat = "\u001b]0;{0}\a";
					}
					else if (this.term == "iris-ansi")
					{
						this.titleFormat = "\u001bP1.y{0}\u001b\\";
					}
					else if (this.term == "sun-cmd")
					{
						this.titleFormat = "\u001b]l{0}\u001b\\";
					}
					this.cursorAddress = this.reader.Get(TermInfoStrings.CursorAddress);
					this.GetCursorPosition();
					if (this.noGetPosition)
					{
						this.WriteConsole(this.clear);
						this.cursorLeft = 0;
						this.cursorTop = 0;
					}
				}
			}
		}

		private void IncrementX()
		{
			this.cursorLeft++;
			if (this.cursorLeft >= this.WindowWidth)
			{
				this.cursorTop++;
				this.cursorLeft = 0;
				if (this.cursorTop >= this.WindowHeight)
				{
					if (this.rl_starty != -1)
					{
						this.rl_starty--;
					}
					this.cursorTop--;
				}
			}
		}

		public void WriteSpecialKey(ConsoleKeyInfo key)
		{
			switch (key.Key)
			{
			case ConsoleKey.Backspace:
				if (this.cursorLeft > 0 && (this.cursorLeft > this.rl_startx || this.cursorTop != this.rl_starty))
				{
					this.cursorLeft--;
					this.SetCursorPosition(this.cursorLeft, this.cursorTop);
					this.WriteConsole(" ");
					this.SetCursorPosition(this.cursorLeft, this.cursorTop);
					return;
				}
				break;
			case ConsoleKey.Tab:
			{
				int num = 8 - this.cursorLeft % 8;
				for (int i = 0; i < num; i++)
				{
					this.IncrementX();
				}
				this.WriteConsole("\t");
				return;
			}
			case (ConsoleKey)10:
			case (ConsoleKey)11:
			case ConsoleKey.Enter:
				break;
			case ConsoleKey.Clear:
				this.WriteConsole(this.clear);
				this.cursorLeft = 0;
				this.cursorTop = 0;
				break;
			default:
				return;
			}
		}

		public void WriteSpecialKey(char c)
		{
			this.WriteSpecialKey(this.CreateKeyInfoFromInt((int)c, false));
		}

		public bool IsSpecialKey(ConsoleKeyInfo key)
		{
			if (!this.inited)
			{
				return false;
			}
			switch (key.Key)
			{
			case ConsoleKey.Backspace:
				return true;
			case ConsoleKey.Tab:
				return true;
			case ConsoleKey.Clear:
				return true;
			case ConsoleKey.Enter:
				this.cursorLeft = 0;
				this.cursorTop++;
				if (this.cursorTop >= this.WindowHeight)
				{
					this.cursorTop--;
				}
				return false;
			}
			this.IncrementX();
			return false;
		}

		public bool IsSpecialKey(char c)
		{
			return this.IsSpecialKey(this.CreateKeyInfoFromInt((int)c, false));
		}

		private void ChangeColor(string format, ConsoleColor color)
		{
			if ((color & (ConsoleColor)(-16)) != ConsoleColor.Black)
			{
				throw new ArgumentException("Invalid Console Color");
			}
			int num = TermInfoDriver._consoleColorToAnsiCode[(int)color] % this.maxColors;
			this.WriteConsole(ParameterizedStrings.Evaluate(format, new ParameterizedStrings.FormatParam[] { num }));
		}

		public ConsoleColor BackgroundColor
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return this.bgcolor;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				this.ChangeColor(this.setbgcolor, value);
				this.bgcolor = value;
			}
		}

		public ConsoleColor ForegroundColor
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return this.fgcolor;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				this.ChangeColor(this.setfgcolor, value);
				this.fgcolor = value;
			}
		}

		private void GetCursorPosition()
		{
			int num = 0;
			int num2 = 0;
			int num3 = ConsoleDriver.InternalKeyAvailable(0);
			int num4;
			while (num3-- > 0)
			{
				num4 = this.stdin.Read();
				this.AddToBuffer(num4);
			}
			this.WriteConsole("\u001b[6n");
			if (ConsoleDriver.InternalKeyAvailable(1000) <= 0)
			{
				this.noGetPosition = true;
				return;
			}
			for (num4 = this.stdin.Read(); num4 != 27; num4 = this.stdin.Read())
			{
				this.AddToBuffer(num4);
				if (ConsoleDriver.InternalKeyAvailable(100) <= 0)
				{
					return;
				}
			}
			num4 = this.stdin.Read();
			if (num4 != 91)
			{
				this.AddToBuffer(27);
				this.AddToBuffer(num4);
				return;
			}
			num4 = this.stdin.Read();
			if (num4 != 59)
			{
				num = num4 - 48;
				num4 = this.stdin.Read();
				while (num4 >= 48 && num4 <= 57)
				{
					num = num * 10 + num4 - 48;
					num4 = this.stdin.Read();
				}
				num--;
			}
			num4 = this.stdin.Read();
			if (num4 != 82)
			{
				num2 = num4 - 48;
				num4 = this.stdin.Read();
				while (num4 >= 48 && num4 <= 57)
				{
					num2 = num2 * 10 + num4 - 48;
					num4 = this.stdin.Read();
				}
				num2--;
			}
			this.cursorLeft = num2;
			this.cursorTop = num;
		}

		public int BufferHeight
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				this.CheckWindowDimensions();
				return this.bufferHeight;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				throw new NotSupportedException();
			}
		}

		public int BufferWidth
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				this.CheckWindowDimensions();
				return this.bufferWidth;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				throw new NotSupportedException();
			}
		}

		public bool CapsLock
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return false;
			}
		}

		public int CursorLeft
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return this.cursorLeft;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				this.SetCursorPosition(value, this.CursorTop);
			}
		}

		public int CursorTop
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return this.cursorTop;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				this.SetCursorPosition(this.CursorLeft, value);
			}
		}

		public bool CursorVisible
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return this.cursorVisible;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				this.cursorVisible = value;
				this.WriteConsole(value ? this.csrVisible : this.csrInvisible);
			}
		}

		[MonoTODO]
		public int CursorSize
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return 1;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
			}
		}

		public bool KeyAvailable
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return this.writepos > this.readpos || ConsoleDriver.InternalKeyAvailable(0) > 0;
			}
		}

		public int LargestWindowHeight
		{
			get
			{
				return this.WindowHeight;
			}
		}

		public int LargestWindowWidth
		{
			get
			{
				return this.WindowWidth;
			}
		}

		public bool NumberLock
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return false;
			}
		}

		public string Title
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return this.title;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				this.title = value;
				this.WriteConsole(string.Format(this.titleFormat, value));
			}
		}

		public bool TreatControlCAsInput
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return this.controlCAsInput;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				if (this.controlCAsInput == value)
				{
					return;
				}
				ConsoleDriver.SetBreak(value);
				this.controlCAsInput = value;
			}
		}

		private unsafe void CheckWindowDimensions()
		{
			if (TermInfoDriver.native_terminal_size == null || TermInfoDriver.terminal_size == *TermInfoDriver.native_terminal_size)
			{
				return;
			}
			if (*TermInfoDriver.native_terminal_size == -1)
			{
				int num = this.reader.Get(TermInfoNumbers.Columns);
				if (num != 0)
				{
					this.windowWidth = num;
				}
				num = this.reader.Get(TermInfoNumbers.Lines);
				if (num != 0)
				{
					this.windowHeight = num;
				}
			}
			else
			{
				TermInfoDriver.terminal_size = *TermInfoDriver.native_terminal_size;
				this.windowWidth = TermInfoDriver.terminal_size >> 16;
				this.windowHeight = TermInfoDriver.terminal_size & 65535;
			}
			this.bufferHeight = this.windowHeight;
			this.bufferWidth = this.windowWidth;
		}

		public int WindowHeight
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				this.CheckWindowDimensions();
				return this.windowHeight;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				throw new NotSupportedException();
			}
		}

		public int WindowLeft
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return 0;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				throw new NotSupportedException();
			}
		}

		public int WindowTop
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				return 0;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				throw new NotSupportedException();
			}
		}

		public int WindowWidth
		{
			get
			{
				if (!this.inited)
				{
					this.Init();
				}
				this.CheckWindowDimensions();
				return this.windowWidth;
			}
			set
			{
				if (!this.inited)
				{
					this.Init();
				}
				throw new NotSupportedException();
			}
		}

		public void Clear()
		{
			if (!this.inited)
			{
				this.Init();
			}
			this.WriteConsole(this.clear);
			this.cursorLeft = 0;
			this.cursorTop = 0;
		}

		public void Beep(int frequency, int duration)
		{
			if (!this.inited)
			{
				this.Init();
			}
			this.WriteConsole(this.bell);
		}

		public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor)
		{
			if (!this.inited)
			{
				this.Init();
			}
			throw new NotImplementedException();
		}

		private void AddToBuffer(int b)
		{
			if (this.buffer == null)
			{
				this.buffer = new char[1024];
			}
			else if (this.writepos >= this.buffer.Length)
			{
				char[] array = new char[this.buffer.Length * 2];
				Buffer.BlockCopy(this.buffer, 0, array, 0, this.buffer.Length);
				this.buffer = array;
			}
			char[] array2 = this.buffer;
			int num = this.writepos;
			this.writepos = num + 1;
			array2[num] = (ushort)b;
		}

		private void AdjustBuffer()
		{
			if (this.readpos >= this.writepos)
			{
				this.readpos = (this.writepos = 0);
			}
		}

		private ConsoleKeyInfo CreateKeyInfoFromInt(int n, bool alt)
		{
			char c = (char)n;
			ConsoleKey consoleKey = (ConsoleKey)n;
			bool flag = false;
			bool flag2 = false;
			if (n <= 19)
			{
				switch (n)
				{
				case 8:
				case 9:
				case 12:
				case 13:
					goto IL_00C7;
				case 10:
					consoleKey = ConsoleKey.Enter;
					goto IL_00C7;
				case 11:
					break;
				default:
					if (n == 19)
					{
						goto IL_00C7;
					}
					break;
				}
			}
			else
			{
				if (n == 27)
				{
					consoleKey = ConsoleKey.Escape;
					goto IL_00C7;
				}
				if (n == 32)
				{
					consoleKey = ConsoleKey.Spacebar;
					goto IL_00C7;
				}
				switch (n)
				{
				case 42:
					consoleKey = ConsoleKey.Multiply;
					goto IL_00C7;
				case 43:
					consoleKey = ConsoleKey.Add;
					goto IL_00C7;
				case 45:
					consoleKey = ConsoleKey.Subtract;
					goto IL_00C7;
				case 47:
					consoleKey = ConsoleKey.Divide;
					goto IL_00C7;
				}
			}
			if (n >= 1 && n <= 26)
			{
				flag2 = true;
				consoleKey = ConsoleKey.A + n - 1;
			}
			else if (n >= 97 && n <= 122)
			{
				consoleKey = (ConsoleKey)(-32) + n;
			}
			else if (n >= 65 && n <= 90)
			{
				flag = true;
			}
			else if (n < 48 || n > 57)
			{
				consoleKey = (ConsoleKey)0;
			}
			IL_00C7:
			return new ConsoleKeyInfo(c, consoleKey, flag, alt, flag2);
		}

		private object GetKeyFromBuffer(bool cooked)
		{
			if (this.readpos >= this.writepos)
			{
				return null;
			}
			int num = (int)this.buffer[this.readpos];
			if (!cooked || !this.rootmap.StartsWith(num))
			{
				this.readpos++;
				this.AdjustBuffer();
				return this.CreateKeyInfoFromInt(num, false);
			}
			int num2;
			TermInfoStrings termInfoStrings = this.rootmap.Match(this.buffer, this.readpos, this.writepos - this.readpos, out num2);
			if (termInfoStrings == (TermInfoStrings)(-1))
			{
				if (this.buffer[this.readpos] != '\u001b' || this.writepos - this.readpos < 2)
				{
					return null;
				}
				this.readpos += 2;
				this.AdjustBuffer();
				if (this.buffer[this.readpos + 1] == '\u007f')
				{
					return new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, true, false);
				}
				return this.CreateKeyInfoFromInt((int)this.buffer[this.readpos + 1], true);
			}
			else
			{
				if (this.keymap[termInfoStrings] != null)
				{
					ConsoleKeyInfo consoleKeyInfo = (ConsoleKeyInfo)this.keymap[termInfoStrings];
					this.readpos += num2;
					this.AdjustBuffer();
					return consoleKeyInfo;
				}
				this.readpos++;
				this.AdjustBuffer();
				return this.CreateKeyInfoFromInt(num, false);
			}
		}

		private ConsoleKeyInfo ReadKeyInternal(out bool fresh)
		{
			if (!this.inited)
			{
				this.Init();
			}
			this.InitKeys();
			object obj;
			if ((obj = this.GetKeyFromBuffer(true)) == null)
			{
				do
				{
					if (ConsoleDriver.InternalKeyAvailable(150) > 0)
					{
						do
						{
							this.AddToBuffer(this.stdin.Read());
						}
						while (ConsoleDriver.InternalKeyAvailable(0) > 0);
					}
					else if (this.stdin.DataAvailable())
					{
						do
						{
							this.AddToBuffer(this.stdin.Read());
						}
						while (this.stdin.DataAvailable());
					}
					else
					{
						if ((obj = this.GetKeyFromBuffer(false)) != null)
						{
							break;
						}
						this.AddToBuffer(this.stdin.Read());
					}
					obj = this.GetKeyFromBuffer(true);
				}
				while (obj == null);
				fresh = true;
			}
			else
			{
				fresh = false;
			}
			return (ConsoleKeyInfo)obj;
		}

		private bool InputPending()
		{
			return this.readpos < this.writepos || this.stdin.DataAvailable();
		}

		private void QueueEcho(char c)
		{
			if (this.echobuf == null)
			{
				this.echobuf = new char[1024];
			}
			char[] array = this.echobuf;
			int num = this.echon;
			this.echon = num + 1;
			array[num] = c;
			if (this.echon == this.echobuf.Length || !this.InputPending())
			{
				this.stdout.InternalWriteChars(this.echobuf, this.echon);
				this.echon = 0;
			}
		}

		private void Echo(ConsoleKeyInfo key)
		{
			if (!this.IsSpecialKey(key))
			{
				this.QueueEcho(key.KeyChar);
				return;
			}
			this.EchoFlush();
			this.WriteSpecialKey(key);
		}

		private void EchoFlush()
		{
			if (this.echon == 0)
			{
				return;
			}
			this.stdout.InternalWriteChars(this.echobuf, this.echon);
			this.echon = 0;
		}

		public int Read([In] [Out] char[] dest, int index, int count)
		{
			bool flag = false;
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			object keyFromBuffer;
			while ((keyFromBuffer = this.GetKeyFromBuffer(true)) != null)
			{
				ConsoleKeyInfo consoleKeyInfo = (ConsoleKeyInfo)keyFromBuffer;
				char c = consoleKeyInfo.KeyChar;
				if (consoleKeyInfo.Key != ConsoleKey.Backspace)
				{
					if (consoleKeyInfo.Key == ConsoleKey.Enter)
					{
						num = stringBuilder.Length;
					}
					stringBuilder.Append(c);
				}
				else if (stringBuilder.Length > num)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					int num2 = stringBuilder2.Length;
					stringBuilder2.Length = num2 - 1;
				}
			}
			this.rl_startx = this.cursorLeft;
			this.rl_starty = this.cursorTop;
			for (;;)
			{
				bool flag2;
				ConsoleKeyInfo consoleKeyInfo = this.ReadKeyInternal(out flag2);
				flag = flag || flag2;
				char c = consoleKeyInfo.KeyChar;
				if (consoleKeyInfo.Key != ConsoleKey.Backspace)
				{
					if (consoleKeyInfo.Key == ConsoleKey.Enter)
					{
						num = stringBuilder.Length;
					}
					stringBuilder.Append(c);
					goto IL_00E0;
				}
				if (stringBuilder.Length > num)
				{
					StringBuilder stringBuilder3 = stringBuilder;
					int num2 = stringBuilder3.Length;
					stringBuilder3.Length = num2 - 1;
					goto IL_00E0;
				}
				IL_00EA:
				if (consoleKeyInfo.Key == ConsoleKey.Enter)
				{
					break;
				}
				continue;
				IL_00E0:
				if (flag)
				{
					this.Echo(consoleKeyInfo);
					goto IL_00EA;
				}
				goto IL_00EA;
			}
			this.EchoFlush();
			this.rl_startx = -1;
			this.rl_starty = -1;
			int num3 = 0;
			while (count > 0 && num3 < stringBuilder.Length)
			{
				dest[index + num3] = stringBuilder[num3];
				num3++;
				count--;
			}
			for (int i = num3; i < stringBuilder.Length; i++)
			{
				this.AddToBuffer((int)stringBuilder[i]);
			}
			return num3;
		}

		public ConsoleKeyInfo ReadKey(bool intercept)
		{
			bool flag;
			ConsoleKeyInfo consoleKeyInfo = this.ReadKeyInternal(out flag);
			if (!intercept && flag)
			{
				this.Echo(consoleKeyInfo);
				this.EchoFlush();
			}
			return consoleKeyInfo;
		}

		public string ReadLine()
		{
			return this.ReadUntilConditionInternal(true);
		}

		public string ReadToEnd()
		{
			return this.ReadUntilConditionInternal(false);
		}

		private string ReadUntilConditionInternal(bool haltOnNewLine)
		{
			if (!this.inited)
			{
				this.Init();
			}
			this.GetCursorPosition();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			this.rl_startx = this.cursorLeft;
			this.rl_starty = this.cursorTop;
			char c = (char)this.control_characters[4];
			for (;;)
			{
				bool flag2;
				ConsoleKeyInfo consoleKeyInfo = this.ReadKeyInternal(out flag2);
				flag = flag || flag2;
				char keyChar = consoleKeyInfo.KeyChar;
				if (keyChar == c && keyChar != '\0' && stringBuilder.Length == 0)
				{
					break;
				}
				bool flag3 = haltOnNewLine && consoleKeyInfo.Key == ConsoleKey.Enter;
				if (flag3)
				{
					goto IL_00AC;
				}
				if (consoleKeyInfo.Key != ConsoleKey.Backspace)
				{
					stringBuilder.Append(keyChar);
					goto IL_00AC;
				}
				if (stringBuilder.Length > 0)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					int length = stringBuilder2.Length;
					stringBuilder2.Length = length - 1;
					goto IL_00AC;
				}
				IL_00B6:
				if (flag3)
				{
					goto Block_10;
				}
				continue;
				IL_00AC:
				if (flag)
				{
					this.Echo(consoleKeyInfo);
					goto IL_00B6;
				}
				goto IL_00B6;
			}
			return null;
			Block_10:
			this.EchoFlush();
			this.rl_startx = -1;
			this.rl_starty = -1;
			return stringBuilder.ToString();
		}

		public void ResetColor()
		{
			if (!this.inited)
			{
				this.Init();
			}
			string text = ((this.origPair != null) ? this.origPair : this.origColors);
			this.WriteConsole(text);
		}

		public void SetBufferSize(int width, int height)
		{
			if (!this.inited)
			{
				this.Init();
			}
			throw new NotImplementedException(string.Empty);
		}

		public void SetCursorPosition(int left, int top)
		{
			if (!this.inited)
			{
				this.Init();
			}
			this.CheckWindowDimensions();
			if (left < 0 || left >= this.bufferWidth)
			{
				throw new ArgumentOutOfRangeException("left", "Value must be positive and below the buffer width.");
			}
			if (top < 0 || top >= this.bufferHeight)
			{
				throw new ArgumentOutOfRangeException("top", "Value must be positive and below the buffer height.");
			}
			if (this.cursorAddress == null)
			{
				throw new NotSupportedException("This terminal does not suport setting the cursor position.");
			}
			this.WriteConsole(ParameterizedStrings.Evaluate(this.cursorAddress, new ParameterizedStrings.FormatParam[] { top, left }));
			this.cursorLeft = left;
			this.cursorTop = top;
		}

		public void SetWindowPosition(int left, int top)
		{
			if (!this.inited)
			{
				this.Init();
			}
		}

		public void SetWindowSize(int width, int height)
		{
			if (!this.inited)
			{
				this.Init();
			}
		}

		private void CreateKeyMap()
		{
			this.keymap = new Hashtable();
			this.keymap[TermInfoStrings.KeyBackspace] = new ConsoleKeyInfo('\0', ConsoleKey.Backspace, false, false, false);
			this.keymap[TermInfoStrings.KeyClear] = new ConsoleKeyInfo('\0', ConsoleKey.Clear, false, false, false);
			this.keymap[TermInfoStrings.KeyDown] = new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false);
			this.keymap[TermInfoStrings.KeyF1] = new ConsoleKeyInfo('\0', ConsoleKey.F1, false, false, false);
			this.keymap[TermInfoStrings.KeyF10] = new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false);
			this.keymap[TermInfoStrings.KeyF2] = new ConsoleKeyInfo('\0', ConsoleKey.F2, false, false, false);
			this.keymap[TermInfoStrings.KeyF3] = new ConsoleKeyInfo('\0', ConsoleKey.F3, false, false, false);
			this.keymap[TermInfoStrings.KeyF4] = new ConsoleKeyInfo('\0', ConsoleKey.F4, false, false, false);
			this.keymap[TermInfoStrings.KeyF5] = new ConsoleKeyInfo('\0', ConsoleKey.F5, false, false, false);
			this.keymap[TermInfoStrings.KeyF6] = new ConsoleKeyInfo('\0', ConsoleKey.F6, false, false, false);
			this.keymap[TermInfoStrings.KeyF7] = new ConsoleKeyInfo('\0', ConsoleKey.F7, false, false, false);
			this.keymap[TermInfoStrings.KeyF8] = new ConsoleKeyInfo('\0', ConsoleKey.F8, false, false, false);
			this.keymap[TermInfoStrings.KeyF9] = new ConsoleKeyInfo('\0', ConsoleKey.F9, false, false, false);
			this.keymap[TermInfoStrings.KeyHome] = new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false);
			this.keymap[TermInfoStrings.KeyLeft] = new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false);
			this.keymap[TermInfoStrings.KeyLl] = new ConsoleKeyInfo('\0', ConsoleKey.NumPad1, false, false, false);
			this.keymap[TermInfoStrings.KeyNpage] = new ConsoleKeyInfo('\0', ConsoleKey.PageDown, false, false, false);
			this.keymap[TermInfoStrings.KeyPpage] = new ConsoleKeyInfo('\0', ConsoleKey.PageUp, false, false, false);
			this.keymap[TermInfoStrings.KeyRight] = new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false);
			this.keymap[TermInfoStrings.KeySf] = new ConsoleKeyInfo('\0', ConsoleKey.PageDown, false, false, false);
			this.keymap[TermInfoStrings.KeySr] = new ConsoleKeyInfo('\0', ConsoleKey.PageUp, false, false, false);
			this.keymap[TermInfoStrings.KeyUp] = new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false);
			this.keymap[TermInfoStrings.KeyA1] = new ConsoleKeyInfo('\0', ConsoleKey.NumPad7, false, false, false);
			this.keymap[TermInfoStrings.KeyA3] = new ConsoleKeyInfo('\0', ConsoleKey.NumPad9, false, false, false);
			this.keymap[TermInfoStrings.KeyB2] = new ConsoleKeyInfo('\0', ConsoleKey.NumPad5, false, false, false);
			this.keymap[TermInfoStrings.KeyC1] = new ConsoleKeyInfo('\0', ConsoleKey.NumPad1, false, false, false);
			this.keymap[TermInfoStrings.KeyC3] = new ConsoleKeyInfo('\0', ConsoleKey.NumPad3, false, false, false);
			this.keymap[TermInfoStrings.KeyBtab] = new ConsoleKeyInfo('\0', ConsoleKey.Tab, true, false, false);
			this.keymap[TermInfoStrings.KeyBeg] = new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false);
			this.keymap[TermInfoStrings.KeyCopy] = new ConsoleKeyInfo('C', ConsoleKey.C, false, true, false);
			this.keymap[TermInfoStrings.KeyEnd] = new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false);
			this.keymap[TermInfoStrings.KeyEnter] = new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false);
			this.keymap[TermInfoStrings.KeyHelp] = new ConsoleKeyInfo('\0', ConsoleKey.Help, false, false, false);
			this.keymap[TermInfoStrings.KeyPrint] = new ConsoleKeyInfo('\0', ConsoleKey.Print, false, false, false);
			this.keymap[TermInfoStrings.KeyUndo] = new ConsoleKeyInfo('Z', ConsoleKey.Z, false, true, false);
			this.keymap[TermInfoStrings.KeySbeg] = new ConsoleKeyInfo('\0', ConsoleKey.Home, true, false, false);
			this.keymap[TermInfoStrings.KeyScopy] = new ConsoleKeyInfo('C', ConsoleKey.C, true, true, false);
			this.keymap[TermInfoStrings.KeySdc] = new ConsoleKeyInfo('\t', ConsoleKey.Delete, true, false, false);
			this.keymap[TermInfoStrings.KeyShelp] = new ConsoleKeyInfo('\0', ConsoleKey.Help, true, false, false);
			this.keymap[TermInfoStrings.KeyShome] = new ConsoleKeyInfo('\0', ConsoleKey.Home, true, false, false);
			this.keymap[TermInfoStrings.KeySleft] = new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, true, false, false);
			this.keymap[TermInfoStrings.KeySprint] = new ConsoleKeyInfo('\0', ConsoleKey.Print, true, false, false);
			this.keymap[TermInfoStrings.KeySright] = new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, true, false, false);
			this.keymap[TermInfoStrings.KeySundo] = new ConsoleKeyInfo('Z', ConsoleKey.Z, true, false, false);
			this.keymap[TermInfoStrings.KeyF11] = new ConsoleKeyInfo('\0', ConsoleKey.F11, false, false, false);
			this.keymap[TermInfoStrings.KeyF12] = new ConsoleKeyInfo('\0', ConsoleKey.F12, false, false, false);
			this.keymap[TermInfoStrings.KeyF13] = new ConsoleKeyInfo('\0', ConsoleKey.F13, false, false, false);
			this.keymap[TermInfoStrings.KeyF14] = new ConsoleKeyInfo('\0', ConsoleKey.F14, false, false, false);
			this.keymap[TermInfoStrings.KeyF15] = new ConsoleKeyInfo('\0', ConsoleKey.F15, false, false, false);
			this.keymap[TermInfoStrings.KeyF16] = new ConsoleKeyInfo('\0', ConsoleKey.F16, false, false, false);
			this.keymap[TermInfoStrings.KeyF17] = new ConsoleKeyInfo('\0', ConsoleKey.F17, false, false, false);
			this.keymap[TermInfoStrings.KeyF18] = new ConsoleKeyInfo('\0', ConsoleKey.F18, false, false, false);
			this.keymap[TermInfoStrings.KeyF19] = new ConsoleKeyInfo('\0', ConsoleKey.F19, false, false, false);
			this.keymap[TermInfoStrings.KeyF20] = new ConsoleKeyInfo('\0', ConsoleKey.F20, false, false, false);
			this.keymap[TermInfoStrings.KeyF21] = new ConsoleKeyInfo('\0', ConsoleKey.F21, false, false, false);
			this.keymap[TermInfoStrings.KeyF22] = new ConsoleKeyInfo('\0', ConsoleKey.F22, false, false, false);
			this.keymap[TermInfoStrings.KeyF23] = new ConsoleKeyInfo('\0', ConsoleKey.F23, false, false, false);
			this.keymap[TermInfoStrings.KeyF24] = new ConsoleKeyInfo('\0', ConsoleKey.F24, false, false, false);
			this.keymap[TermInfoStrings.KeyDc] = new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false);
			this.keymap[TermInfoStrings.KeyIc] = new ConsoleKeyInfo('\0', ConsoleKey.Insert, false, false, false);
		}

		private void InitKeys()
		{
			if (this.initKeys)
			{
				return;
			}
			this.CreateKeyMap();
			this.rootmap = new ByteMatcher();
			foreach (TermInfoStrings termInfoStrings in new TermInfoStrings[]
			{
				TermInfoStrings.KeyBackspace,
				TermInfoStrings.KeyClear,
				TermInfoStrings.KeyDown,
				TermInfoStrings.KeyF1,
				TermInfoStrings.KeyF10,
				TermInfoStrings.KeyF2,
				TermInfoStrings.KeyF3,
				TermInfoStrings.KeyF4,
				TermInfoStrings.KeyF5,
				TermInfoStrings.KeyF6,
				TermInfoStrings.KeyF7,
				TermInfoStrings.KeyF8,
				TermInfoStrings.KeyF9,
				TermInfoStrings.KeyHome,
				TermInfoStrings.KeyLeft,
				TermInfoStrings.KeyLl,
				TermInfoStrings.KeyNpage,
				TermInfoStrings.KeyPpage,
				TermInfoStrings.KeyRight,
				TermInfoStrings.KeySf,
				TermInfoStrings.KeySr,
				TermInfoStrings.KeyUp,
				TermInfoStrings.KeyA1,
				TermInfoStrings.KeyA3,
				TermInfoStrings.KeyB2,
				TermInfoStrings.KeyC1,
				TermInfoStrings.KeyC3,
				TermInfoStrings.KeyBtab,
				TermInfoStrings.KeyBeg,
				TermInfoStrings.KeyCopy,
				TermInfoStrings.KeyEnd,
				TermInfoStrings.KeyEnter,
				TermInfoStrings.KeyHelp,
				TermInfoStrings.KeyPrint,
				TermInfoStrings.KeyUndo,
				TermInfoStrings.KeySbeg,
				TermInfoStrings.KeyScopy,
				TermInfoStrings.KeySdc,
				TermInfoStrings.KeyShelp,
				TermInfoStrings.KeyShome,
				TermInfoStrings.KeySleft,
				TermInfoStrings.KeySprint,
				TermInfoStrings.KeySright,
				TermInfoStrings.KeySundo,
				TermInfoStrings.KeyF11,
				TermInfoStrings.KeyF12,
				TermInfoStrings.KeyF13,
				TermInfoStrings.KeyF14,
				TermInfoStrings.KeyF15,
				TermInfoStrings.KeyF16,
				TermInfoStrings.KeyF17,
				TermInfoStrings.KeyF18,
				TermInfoStrings.KeyF19,
				TermInfoStrings.KeyF20,
				TermInfoStrings.KeyF21,
				TermInfoStrings.KeyF22,
				TermInfoStrings.KeyF23,
				TermInfoStrings.KeyF24,
				TermInfoStrings.KeyDc,
				TermInfoStrings.KeyIc
			})
			{
				this.AddStringMapping(termInfoStrings);
			}
			this.rootmap.AddMapping(TermInfoStrings.KeyBackspace, new byte[] { this.control_characters[2] });
			this.rootmap.Sort();
			this.initKeys = true;
		}

		private void AddStringMapping(TermInfoStrings s)
		{
			byte[] stringBytes = this.reader.GetStringBytes(s);
			if (stringBytes == null)
			{
				return;
			}
			this.rootmap.AddMapping(s, stringBytes);
		}

		private unsafe static int* native_terminal_size;

		private static int terminal_size;

		private static readonly string[] locations = new string[] { "/usr/share/terminfo", "/etc/terminfo", "/usr/lib/terminfo", "/lib/terminfo" };

		private TermInfoReader reader;

		private int cursorLeft;

		private int cursorTop;

		private string title = string.Empty;

		private string titleFormat = string.Empty;

		private bool cursorVisible = true;

		private string csrVisible;

		private string csrInvisible;

		private string clear;

		private string bell;

		private string term;

		private StreamReader stdin;

		private CStreamWriter stdout;

		private int windowWidth;

		private int windowHeight;

		private int bufferHeight;

		private int bufferWidth;

		private char[] buffer;

		private int readpos;

		private int writepos;

		private string keypadXmit;

		private string keypadLocal;

		private bool controlCAsInput;

		private bool inited;

		private object initLock = new object();

		private bool initKeys;

		private string origPair;

		private string origColors;

		private string cursorAddress;

		private ConsoleColor fgcolor = ConsoleColor.White;

		private ConsoleColor bgcolor;

		private string setfgcolor;

		private string setbgcolor;

		private int maxColors;

		private bool noGetPosition;

		private Hashtable keymap;

		private ByteMatcher rootmap;

		private int rl_startx = -1;

		private int rl_starty = -1;

		private byte[] control_characters;

		private static readonly int[] _consoleColorToAnsiCode = new int[]
		{
			0, 4, 2, 6, 1, 5, 3, 7, 8, 12,
			10, 14, 9, 13, 11, 15
		};

		private char[] echobuf;

		private int echon;
	}
}
