using System;
using System.Threading;

namespace System.Text
{
	[Serializable]
	public abstract class DecoderFallback
	{
		private static object InternalSyncObject
		{
			get
			{
				if (DecoderFallback.s_InternalSyncObject == null)
				{
					object obj = new object();
					Interlocked.CompareExchange<object>(ref DecoderFallback.s_InternalSyncObject, obj, null);
				}
				return DecoderFallback.s_InternalSyncObject;
			}
		}

		public static DecoderFallback ReplacementFallback
		{
			get
			{
				if (DecoderFallback.replacementFallback == null)
				{
					object internalSyncObject = DecoderFallback.InternalSyncObject;
					lock (internalSyncObject)
					{
						if (DecoderFallback.replacementFallback == null)
						{
							DecoderFallback.replacementFallback = new DecoderReplacementFallback();
						}
					}
				}
				return DecoderFallback.replacementFallback;
			}
		}

		public static DecoderFallback ExceptionFallback
		{
			get
			{
				if (DecoderFallback.exceptionFallback == null)
				{
					object internalSyncObject = DecoderFallback.InternalSyncObject;
					lock (internalSyncObject)
					{
						if (DecoderFallback.exceptionFallback == null)
						{
							DecoderFallback.exceptionFallback = new DecoderExceptionFallback();
						}
					}
				}
				return DecoderFallback.exceptionFallback;
			}
		}

		public abstract DecoderFallbackBuffer CreateFallbackBuffer();

		public abstract int MaxCharCount { get; }

		internal bool IsMicrosoftBestFitFallback
		{
			get
			{
				return this.bIsMicrosoftBestFitFallback;
			}
		}

		internal bool bIsMicrosoftBestFitFallback;

		private static volatile DecoderFallback replacementFallback;

		private static volatile DecoderFallback exceptionFallback;

		private static object s_InternalSyncObject;
	}
}
