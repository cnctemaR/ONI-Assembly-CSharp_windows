using System;
using UnityEngine;

namespace Unity.MemoryProfiler
{
	internal static class MetadataInjector
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void PlayerInitMetadata()
		{
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
