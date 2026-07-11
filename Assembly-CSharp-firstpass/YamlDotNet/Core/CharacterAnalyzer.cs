using System;

namespace YamlDotNet.Core
{
	[Serializable]
	internal class CharacterAnalyzer<TBuffer> where TBuffer : ILookAheadBuffer
	{
		public CharacterAnalyzer(TBuffer buffer)
		{
			this.buffer = buffer;
		}

		public TBuffer Buffer
		{
			get
			{
				return this.buffer;
			}
		}

		public bool EndOfInput
		{
			get
			{
				TBuffer tbuffer = this.buffer;
				return tbuffer.EndOfInput;
			}
		}

		public char Peek(int offset)
		{
			TBuffer tbuffer = this.buffer;
			return tbuffer.Peek(offset);
		}

		public void Skip(int length)
		{
			TBuffer tbuffer = this.buffer;
			tbuffer.Skip(length);
		}

		public bool IsAlphaNumericDashOrUnderscore(int offset = 0)
		{
			TBuffer tbuffer = this.buffer;
			char c = tbuffer.Peek(offset);
			return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || c == '-';
		}

		public bool IsAscii(int offset = 0)
		{
			TBuffer tbuffer = this.buffer;
			return tbuffer.Peek(offset) <= '\u007f';
		}

		public bool IsPrintable(int offset = 0)
		{
			TBuffer tbuffer = this.buffer;
			char c = tbuffer.Peek(offset);
			return c == '\t' || c == '\n' || c == '\r' || (c >= ' ' && c <= '~') || c == '\u0085' || (c >= '\u00a0' && c <= '\ud7ff') || (c >= '\ue000' && c <= '\ufffd');
		}

		public bool IsDigit(int offset = 0)
		{
			TBuffer tbuffer = this.buffer;
			char c = tbuffer.Peek(offset);
			return c >= '0' && c <= '9';
		}

		public int AsDigit(int offset = 0)
		{
			TBuffer tbuffer = this.buffer;
			return (int)(tbuffer.Peek(offset) - '0');
		}

		public bool IsHex(int offset)
		{
			TBuffer tbuffer = this.buffer;
			char c = tbuffer.Peek(offset);
			return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
		}

		public int AsHex(int offset)
		{
			TBuffer tbuffer = this.buffer;
			char c = tbuffer.Peek(offset);
			if (c <= '9')
			{
				return (int)(c - '0');
			}
			if (c <= 'F')
			{
				return (int)(c - 'A' + '\n');
			}
			return (int)(c - 'a' + '\n');
		}

		public bool IsSpace(int offset = 0)
		{
			return this.Check(' ', offset);
		}

		public bool IsZero(int offset = 0)
		{
			return this.Check('\0', offset);
		}

		public bool IsTab(int offset = 0)
		{
			return this.Check('\t', offset);
		}

		public bool IsWhite(int offset = 0)
		{
			return this.IsSpace(offset) || this.IsTab(offset);
		}

		public bool IsBreak(int offset = 0)
		{
			return this.Check("\r\n\u0085\u2028\u2029", offset);
		}

		public bool IsCrLf(int offset = 0)
		{
			return this.Check('\r', offset) && this.Check('\n', offset + 1);
		}

		public bool IsBreakOrZero(int offset = 0)
		{
			return this.IsBreak(offset) || this.IsZero(offset);
		}

		public bool IsWhiteBreakOrZero(int offset = 0)
		{
			return this.IsWhite(offset) || this.IsBreakOrZero(offset);
		}

		public bool Check(char expected, int offset = 0)
		{
			TBuffer tbuffer = this.buffer;
			return tbuffer.Peek(offset) == expected;
		}

		public bool Check(string expectedCharacters, int offset = 0)
		{
			Debug.Assert(expectedCharacters.Length > 1, "Use Check(char, int) instead.");
			TBuffer tbuffer = this.buffer;
			char c = tbuffer.Peek(offset);
			return expectedCharacters.IndexOf(c) != -1;
		}

		private readonly TBuffer buffer;
	}
}
