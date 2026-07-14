using System;

namespace VYaml.Internal
{
	public static class ByteSequenceHash
	{
		public unsafe static int GetHashCode(ReadOnlySpan<byte> span)
		{
			uint num = 2166136261U;
			ReadOnlySpan<byte> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				num = ((uint)(*readOnlySpan[i]) ^ num) * 16777619U;
			}
			return (int)num;
		}
	}
}
