using System;

namespace System.Collections.Generic
{
	internal sealed class ObjectEqualityComparer : IEqualityComparer
	{
		private ObjectEqualityComparer()
		{
		}

		int IEqualityComparer.GetHashCode(object obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return obj.GetHashCode();
		}

		bool IEqualityComparer.Equals(object x, object y)
		{
			if (x == null)
			{
				return y == null;
			}
			return y != null && x.Equals(y);
		}

		internal static readonly ObjectEqualityComparer Default = new ObjectEqualityComparer();
	}
}
