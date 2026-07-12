using System;

namespace Microsoft.Cci.Pdb
{
	internal struct DbiHeader
	{
		internal DbiHeader(BitAccess bits)
		{
			bits.ReadInt32(out this.sig);
			bits.ReadInt32(out this.ver);
			bits.ReadInt32(out this.age);
			bits.ReadInt16(out this.gssymStream);
			bits.ReadUInt16(out this.vers);
			bits.ReadInt16(out this.pssymStream);
			bits.ReadUInt16(out this.pdbver);
			bits.ReadInt16(out this.symrecStream);
			bits.ReadUInt16(out this.pdbver2);
			bits.ReadInt32(out this.gpmodiSize);
			bits.ReadInt32(out this.secconSize);
			bits.ReadInt32(out this.secmapSize);
			bits.ReadInt32(out this.filinfSize);
			bits.ReadInt32(out this.tsmapSize);
			bits.ReadInt32(out this.mfcIndex);
			bits.ReadInt32(out this.dbghdrSize);
			bits.ReadInt32(out this.ecinfoSize);
			bits.ReadUInt16(out this.flags);
			bits.ReadUInt16(out this.machine);
			bits.ReadInt32(out this.reserved);
		}

		internal int sig;

		internal int ver;

		internal int age;

		internal short gssymStream;

		internal ushort vers;

		internal short pssymStream;

		internal ushort pdbver;

		internal short symrecStream;

		internal ushort pdbver2;

		internal int gpmodiSize;

		internal int secconSize;

		internal int secmapSize;

		internal int filinfSize;

		internal int tsmapSize;

		internal int mfcIndex;

		internal int dbghdrSize;

		internal int ecinfoSize;

		internal ushort flags;

		internal ushort machine;

		internal int reserved;
	}
}
