using System;

namespace System.Text
{
	[Serializable]
	public sealed class DecoderFallbackException : ArgumentException
	{
		public DecoderFallbackException()
			: this(null)
		{
		}

		public DecoderFallbackException(string message)
		{
			this.index = -1;
			base..ctor(message);
		}

		public DecoderFallbackException(string message, Exception innerException)
		{
			this.index = -1;
			base..ctor(message, innerException);
		}

		public DecoderFallbackException(string message, byte[] bytesUnknown, int index)
		{
			this.index = -1;
			base..ctor(message);
			this.bytes_unknown = bytesUnknown;
			this.index = index;
		}

		[MonoTODO]
		public byte[] BytesUnknown
		{
			get
			{
				return this.bytes_unknown;
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

		private const string defaultMessage = "Failed to decode the input byte sequence to Unicode characters.";

		private byte[] bytes_unknown;

		private int index;
	}
}
