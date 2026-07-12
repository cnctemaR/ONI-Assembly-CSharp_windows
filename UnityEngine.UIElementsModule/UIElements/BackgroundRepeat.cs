using System;

namespace UnityEngine.UIElements
{
	public struct BackgroundRepeat : IEquatable<BackgroundRepeat>
	{
		public BackgroundRepeat(Repeat repeatX, Repeat repeatY)
		{
			this.x = repeatX;
			this.y = repeatY;
		}

		internal static BackgroundRepeat Initial()
		{
			return BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat(ScaleMode.StretchToFill);
		}

		public override bool Equals(object obj)
		{
			return obj is BackgroundRepeat && this.Equals((BackgroundRepeat)obj);
		}

		public bool Equals(BackgroundRepeat other)
		{
			return other.x == this.x && other.y == this.y;
		}

		public override int GetHashCode()
		{
			int num = 1500536833;
			num = num * -1521134295 + this.x.GetHashCode();
			return num * -1521134295 + this.y.GetHashCode();
		}

		public static bool operator ==(BackgroundRepeat style1, BackgroundRepeat style2)
		{
			return style1.Equals(style2);
		}

		public static bool operator !=(BackgroundRepeat style1, BackgroundRepeat style2)
		{
			return !(style1 == style2);
		}

		public override string ToString()
		{
			return string.Format("(x:{0}, y:{1})", this.x, this.y);
		}

		public Repeat x;

		public Repeat y;
	}
}
