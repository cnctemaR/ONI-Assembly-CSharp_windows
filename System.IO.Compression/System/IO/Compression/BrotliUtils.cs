using System;

namespace System.IO.Compression
{
	internal static class BrotliUtils
	{
		internal static int GetQualityFromCompressionLevel(CompressionLevel level)
		{
			switch (level)
			{
			case CompressionLevel.Optimal:
				return 11;
			case CompressionLevel.Fastest:
				return 1;
			case CompressionLevel.NoCompression:
				return 0;
			default:
				return (int)level;
			}
		}

		public const int WindowBits_Min = 10;

		public const int WindowBits_Default = 22;

		public const int WindowBits_Max = 24;

		public const int Quality_Min = 0;

		public const int Quality_Default = 11;

		public const int Quality_Max = 11;

		public const int MaxInputSize = 2147483132;
	}
}
