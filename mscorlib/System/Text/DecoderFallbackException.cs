using System;
using System.Runtime.Serialization;

namespace System.Text
{
	[Serializable]
	public sealed class DecoderFallbackException : ArgumentException
	{
		public DecoderFallbackException()
			: base("Value does not fall within the expected range.")
		{
			base.HResult = -2147024809;
		}

		public DecoderFallbackException(string message)
			: base(message)
		{
			base.HResult = -2147024809;
		}

		public DecoderFallbackException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147024809;
		}

		public DecoderFallbackException(string message, byte[] bytesUnknown, int index)
			: base(message)
		{
			this._bytesUnknown = bytesUnknown;
			this._index = index;
		}

		private DecoderFallbackException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}

		public byte[] BytesUnknown
		{
			get
			{
				return this._bytesUnknown;
			}
		}

		public int Index
		{
			get
			{
				return this._index;
			}
		}

		private byte[] _bytesUnknown;

		private int _index;
	}
}
