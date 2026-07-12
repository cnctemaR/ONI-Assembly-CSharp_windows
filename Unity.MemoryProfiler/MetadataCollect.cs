using System;
using Unity.Profiling.Memory;

namespace Unity.MemoryProfiler
{
	public abstract class MetadataCollect : IDisposable
	{
		protected MetadataCollect()
		{
			if (MetadataInjector.DefaultCollector != null && MetadataInjector.DefaultCollector != this && MetadataInjector.DefaultCollectorInjected != 0)
			{
				MemoryProfiler.CreatingMetadata -= MetadataInjector.DefaultCollector.CollectMetadata;
				MetadataInjector.CollectorCount -= 1L;
				MetadataInjector.DefaultCollectorInjected = 0;
			}
			MemoryProfiler.CreatingMetadata += this.CollectMetadata;
			MetadataInjector.CollectorCount += 1L;
		}

		public abstract void CollectMetadata(MemorySnapshotMetadata data);

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				MemoryProfiler.CreatingMetadata -= this.CollectMetadata;
				MetadataInjector.CollectorCount -= 1L;
				if (MetadataInjector.DefaultCollector != null && MetadataInjector.CollectorCount < 1L && MetadataInjector.DefaultCollector != this)
				{
					MetadataInjector.DefaultCollectorInjected = 1;
					MemoryProfiler.CreatingMetadata += MetadataInjector.DefaultCollector.CollectMetadata;
					MetadataInjector.CollectorCount += 1L;
				}
			}
		}

		private bool disposed;
	}
}
