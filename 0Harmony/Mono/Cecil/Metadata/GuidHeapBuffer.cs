using System;
using System.Collections.Generic;

namespace Mono.Cecil.Metadata
{
	internal sealed class GuidHeapBuffer : HeapBuffer
	{
		public override bool IsEmpty
		{
			get
			{
				return this.length == 0;
			}
		}

		public GuidHeapBuffer()
			: base(16)
		{
		}

		public uint GetGuidIndex(Guid guid)
		{
			uint num;
			if (this.guids.TryGetValue(guid, out num))
			{
				return num;
			}
			num = (uint)(this.guids.Count + 1);
			this.WriteGuid(guid);
			this.guids.Add(guid, num);
			return num;
		}

		private void WriteGuid(Guid guid)
		{
			base.WriteBytes(guid.ToByteArray());
		}

		private readonly Dictionary<Guid, uint> guids = new Dictionary<Guid, uint>();
	}
}
