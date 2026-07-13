using System;

namespace UnityEngine.TextCore.Text
{
	internal struct HighlightState
	{
		public HighlightState(Color32 color, Offset padding)
		{
			this.color = color;
			this.padding = padding;
		}

		public static bool operator ==(HighlightState lhs, HighlightState rhs)
		{
			return lhs.color.r == rhs.color.r && lhs.color.g == rhs.color.g && lhs.color.b == rhs.color.b && lhs.color.a == rhs.color.a && lhs.padding == rhs.padding;
		}

		public static bool operator !=(HighlightState lhs, HighlightState rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(HighlightState other)
		{
			return base.Equals(other);
		}

		public Color32 color;

		public Offset padding;
	}
}
