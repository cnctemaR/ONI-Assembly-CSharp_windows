using System;

namespace System.Text
{
	[Serializable]
	public abstract class DecoderFallback
	{
		public static DecoderFallback ExceptionFallback
		{
			get
			{
				return DecoderFallback.exception_fallback;
			}
		}

		public abstract int MaxCharCount { get; }

		public static DecoderFallback ReplacementFallback
		{
			get
			{
				return DecoderFallback.replacement_fallback;
			}
		}

		internal static DecoderFallback StandardSafeFallback
		{
			get
			{
				return DecoderFallback.standard_safe_fallback;
			}
		}

		public abstract DecoderFallbackBuffer CreateFallbackBuffer();

		private static DecoderFallback exception_fallback = new DecoderExceptionFallback();

		private static DecoderFallback replacement_fallback = new DecoderReplacementFallback();

		private static DecoderFallback standard_safe_fallback = new DecoderReplacementFallback("\ufffd");
	}
}
