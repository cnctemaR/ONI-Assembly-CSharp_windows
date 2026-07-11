using System;

namespace System.Text
{
	[Serializable]
	public sealed class EncoderExceptionFallback : EncoderFallback
	{
		public override EncoderFallbackBuffer CreateFallbackBuffer()
		{
			return new EncoderExceptionFallbackBuffer();
		}

		public override int MaxCharCount
		{
			get
			{
				return 0;
			}
		}

		public override bool Equals(object value)
		{
			return value is EncoderExceptionFallback;
		}

		public override int GetHashCode()
		{
			return 654;
		}
	}
}
