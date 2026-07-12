using System;
using Unity.Profiling.Memory;
using UnityEngine;

namespace Unity.MemoryProfiler
{
	internal class DefaultMetadataCollect : MetadataCollect
	{
		public DefaultMetadataCollect()
		{
			MetadataInjector.DefaultCollectorInjected = 1;
		}

		public override void CollectMetadata(MemorySnapshotMetadata data)
		{
			data.Description = "Project name: " + Application.productName;
		}
	}
}
