using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal struct VisualData : IStyleDataGroup<VisualData>, IEquatable<VisualData>
	{
		public VisualData Copy()
		{
			return new VisualData
			{
				backgroundColor = this.backgroundColor,
				backgroundImage = this.backgroundImage,
				backgroundPositionX = this.backgroundPositionX,
				backgroundPositionY = this.backgroundPositionY,
				backgroundRepeat = this.backgroundRepeat,
				backgroundSize = this.backgroundSize,
				borderBottomColor = this.borderBottomColor,
				borderBottomLeftRadius = this.borderBottomLeftRadius,
				borderBottomRightRadius = this.borderBottomRightRadius,
				borderLeftColor = this.borderLeftColor,
				borderRightColor = this.borderRightColor,
				borderTopColor = this.borderTopColor,
				borderTopLeftRadius = this.borderTopLeftRadius,
				borderTopRightRadius = this.borderTopRightRadius,
				filter = new List<FilterFunction>(this.filter),
				opacity = this.opacity,
				overflow = this.overflow
			};
		}

		public void CopyFrom(ref VisualData other)
		{
			this.backgroundColor = other.backgroundColor;
			this.backgroundImage = other.backgroundImage;
			this.backgroundPositionX = other.backgroundPositionX;
			this.backgroundPositionY = other.backgroundPositionY;
			this.backgroundRepeat = other.backgroundRepeat;
			this.backgroundSize = other.backgroundSize;
			this.borderBottomColor = other.borderBottomColor;
			this.borderBottomLeftRadius = other.borderBottomLeftRadius;
			this.borderBottomRightRadius = other.borderBottomRightRadius;
			this.borderLeftColor = other.borderLeftColor;
			this.borderRightColor = other.borderRightColor;
			this.borderTopColor = other.borderTopColor;
			this.borderTopLeftRadius = other.borderTopLeftRadius;
			this.borderTopRightRadius = other.borderTopRightRadius;
			bool flag = this.filter != other.filter;
			if (flag)
			{
				this.filter.Clear();
				this.filter.AddRange(other.filter);
			}
			this.opacity = other.opacity;
			this.overflow = other.overflow;
		}

		public static bool operator ==(VisualData lhs, VisualData rhs)
		{
			return lhs.backgroundColor == rhs.backgroundColor && lhs.backgroundImage == rhs.backgroundImage && lhs.backgroundPositionX == rhs.backgroundPositionX && lhs.backgroundPositionY == rhs.backgroundPositionY && lhs.backgroundRepeat == rhs.backgroundRepeat && lhs.backgroundSize == rhs.backgroundSize && lhs.borderBottomColor == rhs.borderBottomColor && lhs.borderBottomLeftRadius == rhs.borderBottomLeftRadius && lhs.borderBottomRightRadius == rhs.borderBottomRightRadius && lhs.borderLeftColor == rhs.borderLeftColor && lhs.borderRightColor == rhs.borderRightColor && lhs.borderTopColor == rhs.borderTopColor && lhs.borderTopLeftRadius == rhs.borderTopLeftRadius && lhs.borderTopRightRadius == rhs.borderTopRightRadius && lhs.filter == rhs.filter && lhs.opacity == rhs.opacity && lhs.overflow == rhs.overflow;
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
			num = (num * 397) ^ this.filter.GetHashCode();
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

		public List<FilterFunction> filter;

		public float opacity;

		public OverflowInternal overflow;
	}
}
