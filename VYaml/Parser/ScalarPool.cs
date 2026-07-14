using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace VYaml.Parser
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class ScalarPool
	{
		public Scalar Rent()
		{
			Scalar scalar = this.fastItem;
			if (scalar != null && Interlocked.CompareExchange<Scalar>(ref this.fastItem, null, scalar) == scalar)
			{
				return scalar;
			}
			if (this.items.TryDequeue(out scalar))
			{
				return scalar;
			}
			return new Scalar(256);
		}

		public void Return(Scalar value)
		{
			value.Clear();
			if (this.fastItem != null || Interlocked.CompareExchange<Scalar>(ref this.fastItem, value, null) != null)
			{
				this.items.Enqueue(value);
			}
		}

		public static readonly ScalarPool Shared = new ScalarPool();

		private readonly ConcurrentQueue<Scalar> items = new ConcurrentQueue<Scalar>();

		[Nullable(2)]
		private Scalar fastItem;
	}
}
