using System;
using System.Collections.Generic;

namespace System.Collections.Concurrent
{
	public abstract class Partitioner<TSource>
	{
		public abstract IList<IEnumerator<TSource>> GetPartitions(int partitionCount);

		public virtual bool SupportsDynamicPartitions
		{
			get
			{
				return false;
			}
		}

		public virtual IEnumerable<TSource> GetDynamicPartitions()
		{
			throw new NotSupportedException("Dynamic partitions are not supported by this partitioner.");
		}
	}
}
