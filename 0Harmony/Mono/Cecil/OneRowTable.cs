using System;

namespace Mono.Cecil
{
	internal abstract class OneRowTable<TRow> : MetadataTable where TRow : struct
	{
		public sealed override int Length
		{
			get
			{
				return 1;
			}
		}

		public sealed override void Sort()
		{
		}

		internal TRow row;
	}
}
