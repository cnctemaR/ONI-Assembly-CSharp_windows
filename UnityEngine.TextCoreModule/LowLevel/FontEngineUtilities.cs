using System;

namespace UnityEngine.TextCore.LowLevel
{
	internal struct FontEngineUtilities
	{
		internal static bool Approximately(float a, float b)
		{
			return Mathf.Abs(a - b) < 0.001f;
		}

		internal static int MaxValue(int a, int b, int c)
		{
			return (a >= b) ? ((a >= c) ? a : c) : ((b >= c) ? b : c);
		}
	}
}
