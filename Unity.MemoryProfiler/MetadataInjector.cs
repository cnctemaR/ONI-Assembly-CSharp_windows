using System;
using UnityEngine;

namespace Unity.MemoryProfiler
{
	internal static class MetadataInjector
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void PlayerInitMetadata()
		{
			if (!Application.isEditor)
			{
				DefaultMetadataCollect defaultCollector = MetadataInjector.DefaultCollector;
				if (defaultCollector != null)
				{
					defaultCollector.Dispose();
				}
				MetadataInjector.DefaultCollector = null;
				MetadataInjector.DefaultCollectorInjected = 0;
				MetadataInjector.CollectorCount = 0L;
			}
			MetadataInjector.InitializeMetadataCollection();
		}

		private static void InitializeMetadataCollection()
		{
			MetadataInjector.DefaultCollector = new DefaultMetadataCollect();
		}

		public static DefaultMetadataCollect DefaultCollector;

		public static long CollectorCount;

		public static byte DefaultCollectorInjected;
	}
}
