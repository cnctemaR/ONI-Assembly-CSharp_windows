using System;

namespace System.Text
{
	[Serializable]
	internal sealed class InternalDecoderBestFitFallback : DecoderFallback
	{
		internal InternalDecoderBestFitFallback(Encoding encoding)
		{
			this._encoding = encoding;
		}

		public override DecoderFallbackBuffer CreateFallbackBuffer()
		{
			return new InternalDecoderBestFitFallbackBuffer(this);
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
			InternalDecoderBestFitFallback internalDecoderBestFitFallback = value as InternalDecoderBestFitFallback;
			return internalDecoderBestFitFallback != null && this._encoding.CodePage == internalDecoderBestFitFallback._encoding.CodePage;
		}

		public override int GetHashCode()
		{
			return this._encoding.CodePage;
		}

		internal Encoding _encoding;

		internal char[] _arrayBestFit;

		internal char _cReplacement = '?';
	}
}
