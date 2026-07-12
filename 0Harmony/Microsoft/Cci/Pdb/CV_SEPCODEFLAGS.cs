using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum CV_SEPCODEFLAGS : uint
	{
		fIsLexicalScope = 1U,
		fReturnsToParent = 2U
	}
}
