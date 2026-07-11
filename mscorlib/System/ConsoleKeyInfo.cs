using System;

namespace System
{
	[Serializable]
	public struct ConsoleKeyInfo
	{
		public ConsoleKeyInfo(char keyChar, ConsoleKey key, bool shift, bool alt, bool control)
		{
			this.key = key;
			this.keychar = keyChar;
			this.modifiers = (ConsoleModifiers)0;
			this.SetModifiers(shift, alt, control);
		}

		internal ConsoleKeyInfo(ConsoleKeyInfo other)
		{
			this.key = other.key;
			this.keychar = other.keychar;
			this.modifiers = other.modifiers;
		}

		internal void SetKey(ConsoleKey key)
		{
			this.key = key;
		}

		internal void SetKeyChar(char keyChar)
		{
			this.keychar = keyChar;
		}

		internal void SetModifiers(bool shift, bool alt, bool control)
		{
			this.modifiers = ((!shift) ? ((ConsoleModifiers)0) : ConsoleModifiers.Shift);
			this.modifiers |= ((!alt) ? ((ConsoleModifiers)0) : ConsoleModifiers.Alt);
			this.modifiers |= ((!control) ? ((ConsoleModifiers)0) : ConsoleModifiers.Control);
		}

		public ConsoleKey Key
		{
			get
			{
				return this.key;
			}
		}

		public char KeyChar
		{
			get
			{
				return this.keychar;
			}
		}

		public ConsoleModifiers Modifiers
		{
			get
			{
				return this.modifiers;
			}
		}

		public override bool Equals(object value)
		{
			return value is ConsoleKeyInfo && this.Equals((ConsoleKeyInfo)value);
		}

		public bool Equals(ConsoleKeyInfo obj)
		{
			return this.key == obj.key && obj.keychar == this.keychar && obj.modifiers == this.modifiers;
		}

		public override int GetHashCode()
		{
			return this.key.GetHashCode() ^ this.keychar.GetHashCode() ^ this.modifiers.GetHashCode();
		}

		public static bool operator ==(ConsoleKeyInfo a, ConsoleKeyInfo b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ConsoleKeyInfo a, ConsoleKeyInfo b)
		{
			return !a.Equals(b);
		}

		internal static ConsoleKeyInfo Empty = new ConsoleKeyInfo('\0', (ConsoleKey)0, false, false, false);

		private ConsoleKey key;

		private char keychar;

		private ConsoleModifiers modifiers;
	}
}
