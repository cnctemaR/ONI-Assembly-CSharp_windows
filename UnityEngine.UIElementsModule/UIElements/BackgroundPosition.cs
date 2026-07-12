using System;

namespace UnityEngine.UIElements
{
	public struct BackgroundPosition : IEquatable<BackgroundPosition>
	{
		public BackgroundPosition(BackgroundPositionKeyword keyword)
		{
			this.keyword = keyword;
			this.offset = new Length(0f);
		}

		public BackgroundPosition(BackgroundPositionKeyword keyword, Length offset)
		{
			this.keyword = keyword;
			this.offset = offset;
		}

		internal static BackgroundPosition Initial()
		{
			return BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.StretchToFill);
		}

		public override bool Equals(object obj)
		{
			return obj is BackgroundPosition && this.Equals((BackgroundPosition)obj);
		}

		public bool Equals(BackgroundPosition other)
		{
			return other.offset == this.offset && other.keyword == this.keyword;
		}

		public override int GetHashCode()
		{
			int num = 1500536833;
			num = num * -1521134295 + this.keyword.GetHashCode();
			return num * -1521134295 + this.offset.GetHashCode();
		}

		public static bool operator ==(BackgroundPosition style1, BackgroundPosition style2)
		{
			return style1.Equals(style2);
		}

		public static bool operator !=(BackgroundPosition style1, BackgroundPosition style2)
		{
			return !(style1 == style2);
		}

		public override string ToString()
		{
			return string.Format("(type:{0} x:{1})", this.keyword, this.offset);
		}

		public BackgroundPositionKeyword keyword;

		public Length offset;
	}
}
