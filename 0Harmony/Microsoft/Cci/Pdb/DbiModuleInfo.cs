using System;

namespace Microsoft.Cci.Pdb
{
	internal class DbiModuleInfo
	{
		internal DbiModuleInfo(BitAccess bits, bool readStrings)
		{
			bits.ReadInt32(out this.opened);
			new DbiSecCon(bits);
			bits.ReadUInt16(out this.flags);
			bits.ReadInt16(out this.stream);
			bits.ReadInt32(out this.cbSyms);
			bits.ReadInt32(out this.cbOldLines);
			bits.ReadInt32(out this.cbLines);
			bits.ReadInt16(out this.files);
			bits.ReadInt16(out this.pad1);
			bits.ReadUInt32(out this.offsets);
			bits.ReadInt32(out this.niSource);
			bits.ReadInt32(out this.niCompiler);
			if (readStrings)
			{
				bits.ReadCString(out this.moduleName);
				bits.ReadCString(out this.objectName);
			}
			else
			{
				bits.SkipCString(out this.moduleName);
				bits.SkipCString(out this.objectName);
			}
			bits.Align(4);
		}

		internal int opened;

		internal ushort flags;

		internal short stream;

		internal int cbSyms;

		internal int cbOldLines;

		internal int cbLines;

		internal short files;

		internal short pad1;

		internal uint offsets;

		internal int niSource;

		internal int niCompiler;

		internal string moduleName;

		internal string objectName;
	}
}
