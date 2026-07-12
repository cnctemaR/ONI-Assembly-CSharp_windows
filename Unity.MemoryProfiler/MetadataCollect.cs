using System;
using UnityEngine.Profiling.Memory.Experimental;

namespace Unity.MemoryProfiler
{
	public abstract class MetadataCollect : IDisposable
	{
		public MetadataCollect()
		{
			if (MetadataInjector.DefaultCollector != null && MetadataInjector.DefaultCollector != this && MetadataInjector.DefaultCollectorInjected != 0)
			{
				MemoryProfiler.createMetaData -= MetadataInjector.DefaultCollector.CollectMetadata;
				MetadataInjector.CollectorCount -= 1L;
				MetadataInjector.DefaultCollectorInjected = 0;
			}
			MemoryProfiler.createMetaData += this.CollectMetadata;
			MetadataInjector.CollectorCount += 1L;
		}

		public abstract void CollectMetadata(MetaData data);

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				MemoryProfiler.createMetaData -= this.CollectMetadata;
				MetadataInjector.CollectorCount -= 1L;
				if (MetadataInjector.DefaultCollector != null && MetadataInjector.CollectorCount < 1L && MetadataInjector.DefaultCollector != this)
				{
					MetadataInjector.DefaultCollectorInjected = 1;
					MemoryProfiler.createMetaData += MetadataInjector.DefaultCollector.CollectMetadata;
					MetadataInjector.CollectorCount += 1L;
				}
			}
		}

		private bool disposed;
	}
}
