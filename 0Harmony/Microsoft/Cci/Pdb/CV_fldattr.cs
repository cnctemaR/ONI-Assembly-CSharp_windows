using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum CV_fldattr
	{
		access = 3,
		mprop = 28,
		pseudo = 32,
		noinherit = 64,
		noconstruct = 128,
		compgenx = 256
	}
}
