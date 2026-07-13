using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements
{
	internal static class MathUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Matrix4x4 PreApply2DOffset(ref Matrix4x4 m, Vector2 p)
		{
			m.m03 += p.x;
			m.m13 += p.y;
			return m;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Matrix4x4 PostApply2DOffset(ref Matrix4x4 m, Vector2 p)
		{
			m.m03 += m.m00 * p.x + m.m01 * p.y;
			m.m13 += m.m10 * p.x + m.m11 * p.y;
			m.m23 += m.m20 * p.x + m.m21 * p.y;
			return m;
		}
	}
}
