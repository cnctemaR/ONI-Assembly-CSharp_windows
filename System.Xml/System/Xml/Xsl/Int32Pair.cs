using System;

namespace System.Xml.Xsl
{
	internal struct Int32Pair
	{
		public Int32Pair(int left, int right)
		{
			this.left = left;
			this.right = right;
		}

		public int Left
		{
			get
			{
				return this.left;
			}
		}

		public int Right
		{
			get
			{
				return this.right;
			}
		}

		public override bool Equals(object other)
		{
			if (other is Int32Pair)
			{
				Int32Pair int32Pair = (Int32Pair)other;
				return this.left == int32Pair.left && this.right == int32Pair.right;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.left.GetHashCode() ^ this.right.GetHashCode();
		}

		private int left;

		private int right;
	}
}
