using System;

namespace Microsoft.Cci.Pdb
{
	internal struct DbiDbgHdr
	{
		internal DbiDbgHdr(BitAccess bits)
		{
			bits.ReadUInt16(out this.snFPO);
			bits.ReadUInt16(out this.snException);
			bits.ReadUInt16(out this.snFixup);
			bits.ReadUInt16(out this.snOmapToSrc);
			bits.ReadUInt16(out this.snOmapFromSrc);
			bits.ReadUInt16(out this.snSectionHdr);
			bits.ReadUInt16(out this.snTokenRidMap);
			bits.ReadUInt16(out this.snXdata);
			bits.ReadUInt16(out this.snPdata);
			bits.ReadUInt16(out this.snNewFPO);
			bits.ReadUInt16(out this.snSectionHdrOrig);
		}

		internal ushort snFPO;

		internal ushort snException;

		internal ushort snFixup;

		internal ushort snOmapToSrc;

		internal ushort snOmapFromSrc;

		internal ushort snSectionHdr;

		internal ushort snTokenRidMap;

		internal ushort snXdata;

		internal ushort snPdata;

		internal ushort snNewFPO;

		internal ushort snSectionHdrOrig;
	}
}
