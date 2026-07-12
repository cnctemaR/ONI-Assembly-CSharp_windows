using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum EXPORTSYM_FLAGS : ushort
	{
		fConstant = 1,
		fData = 2,
		fPrivate = 4,
		fNoName = 8,
		fOrdinal = 16,
		fForwarder = 32
	}
}
