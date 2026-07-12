using System;
using System.Reflection;

namespace Microsoft.SqlServer.Server
{
	internal sealed class FieldInfoEx : IComparable
	{
		internal FieldInfoEx(FieldInfo fi, int offset, Normalizer normalizer)
		{
			this.FieldInfo = fi;
			this.Offset = offset;
			this.Normalizer = normalizer;
		}

		public int CompareTo(object other)
		{
			FieldInfoEx fieldInfoEx = other as FieldInfoEx;
			if (fieldInfoEx == null)
			{
				return -1;
			}
			return this.Offset.CompareTo(fieldInfoEx.Offset);
		}

		internal readonly int Offset;

		internal readonly FieldInfo FieldInfo;

		internal readonly Normalizer Normalizer;
	}
}
