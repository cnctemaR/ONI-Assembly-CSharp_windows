using System;

namespace Iced.Intel
{
	internal static class CodeExtensions
	{
		internal static bool IgnoresSegment(this Code code)
		{
			return code - Code.Lea_r16_m <= 2 || code - Code.Bndcl_bnd_rm32 <= 3 || code - Code.Bndmk_bnd_m32 <= 3;
		}

		internal static bool IgnoresIndex(this Code code)
		{
			return code == Code.Bndldx_bnd_mib || code == Code.Bndstx_mib_bnd;
		}

		internal static bool IsTileStrideIndex(this Code code)
		{
			return code - Code.VEX_Tileloaddt1_tmm_sibmem <= 2;
		}
	}
}
