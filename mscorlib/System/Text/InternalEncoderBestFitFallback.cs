using System;

namespace System.Text
{
	[Serializable]
	internal class InternalEncoderBestFitFallback : EncoderFallback
	{
		internal InternalEncoderBestFitFallback(Encoding encoding)
		{
			this._encoding = encoding;
		}

		public override EncoderFallbackBuffer CreateFallbackBuffer()
		{
			return new InternalEncoderBestFitFallbackBuffer(this);
		}

		public override int MaxCharCount
		{
			get
			{
				return 1;
			}
		}

		public override bool Equals(object value)
		{
			InternalEncoderBestFitFallback internalEncoderBestFitFallback = value as InternalEncoderBestFitFallback;
			return internalEncoderBestFitFallback != null && this._encoding.CodePage == internalEncoderBestFitFallback._encoding.CodePage;
		}

		public override int GetHashCode()
		{
			return this._encoding.CodePage;
		}

		internal Encoding _encoding;

		internal char[] _arrayBestFit;
	}
}
