using System;

namespace UnityEngine.UIElements
{
	internal struct RareData : IStyleDataGroup<RareData>, IEquatable<RareData>
	{
		public RareData Copy()
		{
			return this;
		}

		public void CopyFrom(ref RareData other)
		{
			this = other;
		}

		public static bool operator ==(RareData lhs, RareData rhs)
		{
			return lhs.cursor == rhs.cursor && lhs.textOverflow == rhs.textOverflow && lhs.unityBackgroundImageTintColor == rhs.unityBackgroundImageTintColor && lhs.unityOverflowClipBox == rhs.unityOverflowClipBox && lhs.unitySliceBottom == rhs.unitySliceBottom && lhs.unitySliceLeft == rhs.unitySliceLeft && lhs.unitySliceRight == rhs.unitySliceRight && lhs.unitySliceScale == rhs.unitySliceScale && lhs.unitySliceTop == rhs.unitySliceTop && lhs.unityTextOverflowPosition == rhs.unityTextOverflowPosition;
		}

		public static bool operator !=(RareData lhs, RareData rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(RareData other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return !flag && obj is RareData && this.Equals((RareData)obj);
		}

		public override int GetHashCode()
		{
			int num = this.cursor.GetHashCode();
			num = (num * 397) ^ (int)this.textOverflow;
			num = (num * 397) ^ this.unityBackgroundImageTintColor.GetHashCode();
			num = (num * 397) ^ (int)this.unityOverflowClipBox;
			num = (num * 397) ^ this.unitySliceBottom;
			num = (num * 397) ^ this.unitySliceLeft;
			num = (num * 397) ^ this.unitySliceRight;
			num = (num * 397) ^ this.unitySliceScale.GetHashCode();
			num = (num * 397) ^ this.unitySliceTop;
			return (num * 397) ^ (int)this.unityTextOverflowPosition;
		}

		public Cursor cursor;

		public TextOverflow textOverflow;

		public Color unityBackgroundImageTintColor;

		public OverflowClipBox unityOverflowClipBox;

		public int unitySliceBottom;

		public int unitySliceLeft;

		public int unitySliceRight;

		public float unitySliceScale;

		public int unitySliceTop;

		public TextOverflowPosition unityTextOverflowPosition;
	}
}
