using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public struct ArrayWithOffset
	{
		public ArrayWithOffset(object array, int offset)
		{
			this.array = array;
			this.offset = offset;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (!(obj is ArrayWithOffset))
			{
				return false;
			}
			ArrayWithOffset arrayWithOffset = (ArrayWithOffset)obj;
			return arrayWithOffset.array == this.array && arrayWithOffset.offset == this.offset;
		}

		public bool Equals(ArrayWithOffset obj)
		{
			return obj.array == this.array && obj.offset == this.offset;
		}

		public override int GetHashCode()
		{
			return this.offset;
		}

		public object GetArray()
		{
			return this.array;
		}

		public int GetOffset()
		{
			return this.offset;
		}

		public static bool operator ==(ArrayWithOffset a, ArrayWithOffset b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ArrayWithOffset a, ArrayWithOffset b)
		{
			return !a.Equals(b);
		}

		private object array;

		private int offset;
	}
}
