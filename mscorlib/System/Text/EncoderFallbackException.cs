using System;
using System.Runtime.Serialization;

namespace System.Text
{
	[Serializable]
	public sealed class EncoderFallbackException : ArgumentException
	{
		public EncoderFallbackException()
			: base("Value does not fall within the expected range.")
		{
			base.HResult = -2147024809;
		}

		public EncoderFallbackException(string message)
			: base(message)
		{
			base.HResult = -2147024809;
		}

		public EncoderFallbackException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147024809;
		}

		internal EncoderFallbackException(string message, char charUnknown, int index)
			: base(message)
		{
			this._charUnknown = charUnknown;
			this._index = index;
		}

		internal EncoderFallbackException(string message, char charUnknownHigh, char charUnknownLow, int index)
			: base(message)
		{
			if (!char.IsHighSurrogate(charUnknownHigh))
			{
				throw new ArgumentOutOfRangeException("charUnknownHigh", SR.Format("Valid values are between {0} and {1}, inclusive.", 55296, 56319));
			}
			if (!char.IsLowSurrogate(charUnknownLow))
			{
				throw new ArgumentOutOfRangeException("CharUnknownLow", SR.Format("Valid values are between {0} and {1}, inclusive.", 56320, 57343));
			}
			this._charUnknownHigh = charUnknownHigh;
			this._charUnknownLow = charUnknownLow;
			this._index = index;
		}

		private EncoderFallbackException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}

		public char CharUnknown
		{
			get
			{
				return this._charUnknown;
			}
		}

		public char CharUnknownHigh
		{
			get
			{
				return this._charUnknownHigh;
			}
		}

		public char CharUnknownLow
		{
			get
			{
				return this._charUnknownLow;
			}
		}

		public int Index
		{
			get
			{
				return this._index;
			}
		}

		public bool IsUnknownSurrogate()
		{
			return this._charUnknownHigh > '\0';
		}

		private char _charUnknown;

		private char _charUnknownHigh;

		private char _charUnknownLow;

		private int _index;
	}
}
