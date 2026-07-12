using System;
using System.Threading;

namespace System.Text
{
	[Serializable]
	public abstract class DecoderFallback
	{
		public static DecoderFallback ReplacementFallback
		{
			get
			{
				DecoderFallback decoderFallback;
				if ((decoderFallback = DecoderFallback.s_replacementFallback) == null)
				{
					decoderFallback = Interlocked.CompareExchange<DecoderFallback>(ref DecoderFallback.s_replacementFallback, new DecoderReplacementFallback(), null) ?? DecoderFallback.s_replacementFallback;
				}
				return decoderFallback;
			}
		}

		public static DecoderFallback ExceptionFallback
		{
			get
			{
				DecoderFallback decoderFallback;
				if ((decoderFallback = DecoderFallback.s_exceptionFallback) == null)
				{
					decoderFallback = Interlocked.CompareExchange<DecoderFallback>(ref DecoderFallback.s_exceptionFallback, new DecoderExceptionFallback(), null) ?? DecoderFallback.s_exceptionFallback;
				}
				return decoderFallback;
			}
		}

		public abstract DecoderFallbackBuffer CreateFallbackBuffer();

		public abstract int MaxCharCount { get; }

		private static DecoderFallback s_replacementFallback;

		private static DecoderFallback s_exceptionFallback;
	}
}
