using System;

namespace Unity.Hierarchy
{
	internal static class SpanExtensions
	{
		public unsafe static bool Contains<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>
		{
			for (int i = 0; i < span.Length; i++)
			{
				T t = *span[i];
				bool flag = t.Equals(value);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}
	}
}
