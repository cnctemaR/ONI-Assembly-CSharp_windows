using System;

namespace UnityEngine.UIElements
{
	internal struct VisualData : IStyleDataGroup<VisualData>, IEquatable<VisualData>
	{
		public VisualData Copy()
		{
			return this;
		}

		public void CopyFrom(ref VisualData other)
		{
			this = other;
		}

		public static bool operator ==(VisualData lhs, VisualData rhs)
		{
			return lhs.backgroundColor == rhs.backgroundColor && lhs.backgroundImage == rhs.backgroundImage && lhs.backgroundPositionX == rhs.backgroundPositionX && lhs.backgroundPositionY == rhs.backgroundPositionY && lhs.backgroundRepeat == rhs.backgroundRepeat && lhs.backgroundSize == rhs.backgroundSize && lhs.borderBottomColor == rhs.borderBottomColor && lhs.borderBottomLeftRadius == rhs.borderBottomLeftRadius && lhs.borderBottomRightRadius == rhs.borderBottomRightRadius && lhs.borderLeftColor == rhs.borderLeftColor && lhs.borderRightColor == rhs.borderRightColor && lhs.borderTopColor == rhs.borderTopColor && lhs.borderTopLeftRadius == rhs.borderTopLeftRadius && lhs.borderTopRightRadius == rhs.borderTopRightRadius && lhs.opacity == rhs.opacity && lhs.overflow == rhs.overflow;
		}

		public static bool operator !=(VisualData lhs, VisualData rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(VisualData other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return !flag && obj is VisualData && this.Equals((VisualData)obj);
		}

		public override int GetHashCode()
		{
			int num = this.backgroundColor.GetHashCode();
			num = (num * 397) ^ this.backgroundImage.GetHashCode();
			num = (num * 397) ^ this.backgroundPositionX.GetHashCode();
			num = (num * 397) ^ this.backgroundPositionY.GetHashCode();
			num = (num * 397) ^ this.backgroundRepeat.GetHashCode();
			num = (num * 397) ^ this.backgroundSize.GetHashCode();
			num = (num * 397) ^ this.borderBottomColor.GetHashCode();
			num = (num * 397) ^ this.borderBottomLeftRadius.GetHashCode();
			num = (num * 397) ^ this.borderBottomRightRadius.GetHashCode();
			num = (num * 397) ^ this.borderLeftColor.GetHashCode();
			num = (num * 397) ^ this.borderRightColor.GetHashCode();
			num = (num * 397) ^ this.borderTopColor.GetHashCode();
			num = (num * 397) ^ this.borderTopLeftRadius.GetHashCode();
			num = (num * 397) ^ this.borderTopRightRadius.GetHashCode();
			num = (num * 397) ^ this.opacity.GetHashCode();
			return (num * 397) ^ (int)this.overflow;
		}

		public Color backgroundColor;

		public Background backgroundImage;

		public BackgroundPosition backgroundPositionX;

		public BackgroundPosition backgroundPositionY;

		public BackgroundRepeat backgroundRepeat;

		public BackgroundSize backgroundSize;

		public Color borderBottomColor;

		public Length borderBottomLeftRadius;

		public Length borderBottomRightRadius;

		public Color borderLeftColor;

		public Color borderRightColor;

		public Color borderTopColor;

		public Length borderTopLeftRadius;

		public Length borderTopRightRadius;

		public float opacity;

		public OverflowInternal overflow;
	}
}
