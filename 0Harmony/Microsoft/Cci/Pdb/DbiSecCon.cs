using System;

namespace Microsoft.Cci.Pdb
{
	internal struct DbiSecCon
	{
		internal DbiSecCon(BitAccess bits)
		{
			bits.ReadInt16(out this.section);
			bits.ReadInt16(out this.pad1);
			bits.ReadInt32(out this.offset);
			bits.ReadInt32(out this.size);
			bits.ReadUInt32(out this.flags);
			bits.ReadInt16(out this.module);
			bits.ReadInt16(out this.pad2);
			bits.ReadUInt32(out this.dataCrc);
			bits.ReadUInt32(out this.relocCrc);
		}

		internal short section;

		internal short pad1;

		internal int offset;

		internal int size;

		internal uint flags;

		internal short module;

		internal short pad2;

		internal uint dataCrc;

		internal uint relocCrc;
	}
}
