using System;

namespace Microsoft.Cci.Pdb
{
	internal struct CV_PRIMITIVE_TYPE
	{
		private const uint CV_MMASK = 1792U;

		private const uint CV_TMASK = 240U;

		private const uint CV_SMASK = 15U;

		private const int CV_MSHIFT = 8;

		private const int CV_TSHIFT = 4;

		private const int CV_SSHIFT = 0;

		private const uint CV_FIRST_NONPRIM = 4096U;
	}
}
