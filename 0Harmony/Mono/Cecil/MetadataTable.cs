using System;
using Mono.Cecil.Metadata;

namespace Mono.Cecil
{
	internal abstract class MetadataTable
	{
		public abstract int Length { get; }

		public bool IsLarge
		{
			get
			{
				return this.Length > 65535;
			}
		}

		public abstract void Write(TableHeapBuffer buffer);

		public abstract void Sort();
	}
}
