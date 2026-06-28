using System;

namespace System.Text
{
	[Serializable]
	public abstract class EncoderFallback
	{
		public static EncoderFallback ExceptionFallback
		{
			get
			{
				return EncoderFallback.exception_fallback;
			}
		}

		public abstract int MaxCharCount { get; }

		public static EncoderFallback ReplacementFallback
		{
			get
			{
				return EncoderFallback.replacement_fallback;
			}
		}

		internal static EncoderFallback StandardSafeFallback
		{
			get
			{
				return EncoderFallback.standard_safe_fallback;
			}
		}

		public abstract EncoderFallbackBuffer CreateFallbackBuffer();

		private static EncoderFallback exception_fallback = new EncoderExceptionFallback();

		private static EncoderFallback replacement_fallback = new EncoderReplacementFallback();

		private static EncoderFallback standard_safe_fallback = new EncoderReplacementFallback("\ufffd");
	}
}
