using System;
using System.Collections.Generic;

namespace System.Security.Cryptography.Asn1
{
	internal class SetOfValueComparer : IComparer<ReadOnlyMemory<byte>>
	{
		internal static SetOfValueComparer Instance { get; } = new SetOfValueComparer();

		public unsafe int Compare(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y)
		{
			ReadOnlySpan<byte> span = x.Span;
			ReadOnlySpan<byte> span2 = y.Span;
			int num = Math.Min(x.Length, y.Length);
			int num3;
			for (int i = 0; i < num; i++)
			{
				int num2 = (int)(*span[i]);
				byte b = *span2[i];
				num3 = num2 - (int)b;
				if (num3 != 0)
				{
					return num3;
				}
			}
			num3 = x.Length - y.Length;
			if (num3 != 0)
			{
				return num3;
			}
			return 0;
		}
	}
}
