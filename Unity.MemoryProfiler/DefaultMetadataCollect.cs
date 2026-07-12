using System;
using UnityEngine;
using UnityEngine.Profiling.Memory.Experimental;

namespace Unity.MemoryProfiler
{
	internal class DefaultMetadataCollect : MetadataCollect
	{
		public DefaultMetadataCollect()
		{
			MetadataInjector.DefaultCollectorInjected = 1;
		}

		public override void CollectMetadata(MetaData data)
		{
			data.content = "Project name: " + Application.productName;
			data.platform = string.Empty;
		}
	}
}
