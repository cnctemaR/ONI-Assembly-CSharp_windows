using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace VYaml.Internal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class ReusableByteSequenceBuilderPool
	{
		public static ReusableByteSequenceBuilder Rent()
		{
			ReusableByteSequenceBuilder reusableByteSequenceBuilder;
			if (ReusableByteSequenceBuilderPool.queue.TryDequeue(out reusableByteSequenceBuilder))
			{
				return reusableByteSequenceBuilder;
			}
			return new ReusableByteSequenceBuilder();
		}

		public static void Return(ReusableByteSequenceBuilder builder)
		{
			builder.Reset();
			ReusableByteSequenceBuilderPool.queue.Enqueue(builder);
		}

		private static readonly ConcurrentQueue<ReusableByteSequenceBuilder> queue = new ConcurrentQueue<ReusableByteSequenceBuilder>();
	}
}
