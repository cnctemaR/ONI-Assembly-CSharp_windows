using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum CV_Line_Flags : uint
	{
		linenumStart = 16777215U,
		deltaLineEnd = 2130706432U,
		fStatement = 2147483648U
	}
}
