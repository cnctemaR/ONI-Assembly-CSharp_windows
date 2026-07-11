using System;

namespace System.Text
{
	[Serializable]
	public sealed class EncoderFallbackException : ArgumentException
	{
		public EncoderFallbackException()
			: this(null)
		{
		}

		public EncoderFallbackException(string message)
		{
			this.index = -1;
			base..ctor(message);
		}

		public EncoderFallbackException(string message, Exception innerException)
		{
			this.index = -1;
			base..ctor(message, innerException);
		}

		internal EncoderFallbackException(char charUnknown, int index)
		{
			this.index = -1;
			base..ctor(null);
			this.char_unknown = charUnknown;
			this.index = index;
		}

		internal EncoderFallbackException(char charUnknownHigh, char charUnknownLow, int index)
		{
			this.index = -1;
			base..ctor(null);
			this.char_unknown_high = charUnknownHigh;
			this.char_unknown_low = charUnknownLow;
			this.index = index;
		}

		public char CharUnknown
		{
			get
			{
				return this.char_unknown;
			}
		}

		public char CharUnknownHigh
		{
			get
			{
				return this.char_unknown_high;
			}
		}

		public char CharUnknownLow
		{
			get
			{
				return this.char_unknown_low;
			}
		}

		[MonoTODO]
		public int Index
		{
			get
			{
				return this.index;
			}
		}

		[MonoTODO]
		public bool IsUnknownSurrogate()
		{
			throw new NotImplementedException();
		}

		private const string defaultMessage = "Failed to decode the input byte sequence to Unicode characters.";

		private char char_unknown;

		private char char_unknown_high;

		private char char_unknown_low;

		private int index;
	}
}
