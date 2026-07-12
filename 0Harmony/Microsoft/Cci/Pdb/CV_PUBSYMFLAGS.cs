using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum CV_PUBSYMFLAGS : uint
	{
		fNone = 0U,
		fCode = 1U,
		fFunction = 2U,
		fManaged = 4U,
		fMSIL = 8U
	}
}
