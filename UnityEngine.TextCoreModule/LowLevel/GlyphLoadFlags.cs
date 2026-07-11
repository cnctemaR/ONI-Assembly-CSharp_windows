using System;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	public enum GlyphLoadFlags
	{
		LOAD_DEFAULT,
		LOAD_NO_SCALE,
		LOAD_NO_HINTING,
		LOAD_RENDER = 4,
		LOAD_NO_BITMAP = 8,
		LOAD_FORCE_AUTOHINT = 32,
		LOAD_MONOCHROME = 4096,
		LOAD_NO_AUTOHINT = 32768,
		LOAD_COMPUTE_METRICS = 2097152,
		LOAD_BITMAP_METRICS_ONLY = 4194304
	}
}
