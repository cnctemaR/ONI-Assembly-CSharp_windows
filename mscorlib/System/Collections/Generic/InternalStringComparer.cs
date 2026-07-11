using System;

namespace System.Collections.Generic
{
	[Serializable]
	internal sealed class InternalStringComparer : EqualityComparer<string>
	{
		public override int GetHashCode(string obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return obj.GetHashCode();
		}

		public override bool Equals(string x, string y)
		{
			if (x == null)
			{
				return y == null;
			}
			return x == y || x.Equals(y);
		}

		internal override int IndexOf(string[] array, string value, int startIndex, int count)
		{
			int num = startIndex + count;
			for (int i = startIndex; i < num; i++)
			{
				if (Array.UnsafeLoad<string>(array, i) == value)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
