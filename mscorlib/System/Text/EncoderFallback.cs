using System;
using System.Threading;

namespace System.Text
{
	[Serializable]
	public abstract class EncoderFallback
	{
		private static object InternalSyncObject
		{
			get
			{
				if (EncoderFallback.s_InternalSyncObject == null)
				{
					object obj = new object();
					Interlocked.CompareExchange<object>(ref EncoderFallback.s_InternalSyncObject, obj, null);
				}
				return EncoderFallback.s_InternalSyncObject;
			}
		}

		public static EncoderFallback ReplacementFallback
		{
			get
			{
				if (EncoderFallback.replacementFallback == null)
				{
					object internalSyncObject = EncoderFallback.InternalSyncObject;
					lock (internalSyncObject)
					{
						if (EncoderFallback.replacementFallback == null)
						{
							EncoderFallback.replacementFallback = new EncoderReplacementFallback();
						}
					}
				}
				return EncoderFallback.replacementFallback;
			}
		}

		public static EncoderFallback ExceptionFallback
		{
			get
			{
				if (EncoderFallback.exceptionFallback == null)
				{
					object internalSyncObject = EncoderFallback.InternalSyncObject;
					lock (internalSyncObject)
					{
						if (EncoderFallback.exceptionFallback == null)
						{
							EncoderFallback.exceptionFallback = new EncoderExceptionFallback();
						}
					}
				}
				return EncoderFallback.exceptionFallback;
			}
		}

		public abstract EncoderFallbackBuffer CreateFallbackBuffer();

		public abstract int MaxCharCount { get; }

		internal bool bIsMicrosoftBestFitFallback;

		private static volatile EncoderFallback replacementFallback;

		private static volatile EncoderFallback exceptionFallback;

		private static object s_InternalSyncObject;
	}
}
