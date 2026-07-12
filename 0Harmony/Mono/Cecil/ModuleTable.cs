using System;
using Mono.Cecil.Metadata;

namespace Mono.Cecil
{
	internal sealed class ModuleTable : OneRowTable<Row<uint, uint>>
	{
		public override void Write(TableHeapBuffer buffer)
		{
			buffer.WriteUInt16(0);
			buffer.WriteString(this.row.Col1);
			buffer.WriteGuid(this.row.Col2);
			buffer.WriteUInt16(0);
			buffer.WriteUInt16(0);
		}
	}
}
