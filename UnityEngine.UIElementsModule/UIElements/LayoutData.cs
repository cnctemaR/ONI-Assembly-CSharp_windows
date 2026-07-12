using System;

namespace UnityEngine.UIElements
{
	internal struct LayoutData : IStyleDataGroup<LayoutData>, IEquatable<LayoutData>
	{
		public LayoutData Copy()
		{
			return this;
		}

		public void CopyFrom(ref LayoutData other)
		{
			this = other;
		}

		public static bool operator ==(LayoutData lhs, LayoutData rhs)
		{
			return lhs.alignContent == rhs.alignContent && lhs.alignItems == rhs.alignItems && lhs.alignSelf == rhs.alignSelf && lhs.borderBottomWidth == rhs.borderBottomWidth && lhs.borderLeftWidth == rhs.borderLeftWidth && lhs.borderRightWidth == rhs.borderRightWidth && lhs.borderTopWidth == rhs.borderTopWidth && lhs.bottom == rhs.bottom && lhs.display == rhs.display && lhs.flexBasis == rhs.flexBasis && lhs.flexDirection == rhs.flexDirection && lhs.flexGrow == rhs.flexGrow && lhs.flexShrink == rhs.flexShrink && lhs.flexWrap == rhs.flexWrap && lhs.height == rhs.height && lhs.justifyContent == rhs.justifyContent && lhs.left == rhs.left && lhs.marginBottom == rhs.marginBottom && lhs.marginLeft == rhs.marginLeft && lhs.marginRight == rhs.marginRight && lhs.marginTop == rhs.marginTop && lhs.maxHeight == rhs.maxHeight && lhs.maxWidth == rhs.maxWidth && lhs.minHeight == rhs.minHeight && lhs.minWidth == rhs.minWidth && lhs.paddingBottom == rhs.paddingBottom && lhs.paddingLeft == rhs.paddingLeft && lhs.paddingRight == rhs.paddingRight && lhs.paddingTop == rhs.paddingTop && lhs.position == rhs.position && lhs.right == rhs.right && lhs.top == rhs.top && lhs.width == rhs.width;
		}

		public static bool operator !=(LayoutData lhs, LayoutData rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(LayoutData other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return !flag && obj is LayoutData && this.Equals((LayoutData)obj);
		}

		public override int GetHashCode()
		{
			int num = (int)this.alignContent;
			num = (num * 397) ^ (int)this.alignItems;
			num = (num * 397) ^ (int)this.alignSelf;
			num = (num * 397) ^ this.borderBottomWidth.GetHashCode();
			num = (num * 397) ^ this.borderLeftWidth.GetHashCode();
			num = (num * 397) ^ this.borderRightWidth.GetHashCode();
			num = (num * 397) ^ this.borderTopWidth.GetHashCode();
			num = (num * 397) ^ this.bottom.GetHashCode();
			num = (num * 397) ^ (int)this.display;
			num = (num * 397) ^ this.flexBasis.GetHashCode();
			num = (num * 397) ^ (int)this.flexDirection;
			num = (num * 397) ^ this.flexGrow.GetHashCode();
			num = (num * 397) ^ this.flexShrink.GetHashCode();
			num = (num * 397) ^ (int)this.flexWrap;
			num = (num * 397) ^ this.height.GetHashCode();
			num = (num * 397) ^ (int)this.justifyContent;
			num = (num * 397) ^ this.left.GetHashCode();
			num = (num * 397) ^ this.marginBottom.GetHashCode();
			num = (num * 397) ^ this.marginLeft.GetHashCode();
			num = (num * 397) ^ this.marginRight.GetHashCode();
			num = (num * 397) ^ this.marginTop.GetHashCode();
			num = (num * 397) ^ this.maxHeight.GetHashCode();
			num = (num * 397) ^ this.maxWidth.GetHashCode();
			num = (num * 397) ^ this.minHeight.GetHashCode();
			num = (num * 397) ^ this.minWidth.GetHashCode();
			num = (num * 397) ^ this.paddingBottom.GetHashCode();
			num = (num * 397) ^ this.paddingLeft.GetHashCode();
			num = (num * 397) ^ this.paddingRight.GetHashCode();
			num = (num * 397) ^ this.paddingTop.GetHashCode();
			num = (num * 397) ^ (int)this.position;
			num = (num * 397) ^ this.right.GetHashCode();
			num = (num * 397) ^ this.top.GetHashCode();
			return (num * 397) ^ this.width.GetHashCode();
		}

		public Align alignContent;

		public Align alignItems;

		public Align alignSelf;

		public float borderBottomWidth;

		public float borderLeftWidth;

		public float borderRightWidth;

		public float borderTopWidth;

		public Length bottom;

		public DisplayStyle display;

		public Length flexBasis;

		public FlexDirection flexDirection;

		public float flexGrow;

		public float flexShrink;

		public Wrap flexWrap;

		public Length height;

		public Justify justifyContent;

		public Length left;

		public Length marginBottom;

		public Length marginLeft;

		public Length marginRight;

		public Length marginTop;

		public Length maxHeight;

		public Length maxWidth;

		public Length minHeight;

		public Length minWidth;

		public Length paddingBottom;

		public Length paddingLeft;

		public Length paddingRight;

		public Length paddingTop;

		public Position position;

		public Length right;

		public Length top;

		public Length width;
	}
}
