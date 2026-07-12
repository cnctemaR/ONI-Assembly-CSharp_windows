using System;
using System.Threading;

namespace System.Text
{
	[Serializable]
	public abstract class EncoderFallback
	{
		public static EncoderFallback ReplacementFallback
		{
			get
			{
				if (EncoderFallback.s_replacementFallback == null)
				{
					Interlocked.CompareExchange<EncoderFallback>(ref EncoderFallback.s_replacementFallback, new EncoderReplacementFallback(), null);
				}
				return EncoderFallback.s_replacementFallback;
			}
		}

		public static EncoderFallback ExceptionFallback
		{
			get
			{
				if (EncoderFallback.s_exceptionFallback == null)
				{
					Interlocked.CompareExchange<EncoderFallback>(ref EncoderFallback.s_exceptionFallback, new EncoderExceptionFallback(), null);
				}
				return EncoderFallback.s_exceptionFallback;
			}
		}

		public abstract EncoderFallbackBuffer CreateFallbackBuffer();

		public abstract int MaxCharCount { get; }

		private static EncoderFallback s_replacementFallback;

		private static EncoderFallback s_exceptionFallback;
	}
}
