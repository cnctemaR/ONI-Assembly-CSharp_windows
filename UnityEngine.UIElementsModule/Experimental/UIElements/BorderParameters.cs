using System;

namespace UnityEngine.Experimental.UIElements
{
	internal struct BorderParameters
	{
		public void SetWidth(float top, float right, float bottom, float left)
		{
			this.topWidth = top;
			this.rightWidth = right;
			this.bottomWidth = bottom;
			this.leftWidth = left;
		}

		public void SetWidth(float allBorders)
		{
			this.SetWidth(allBorders, allBorders, allBorders, allBorders);
		}

		public void SetRadius(float topLeft, float topRight, float bottomRight, float bottomLeft)
		{
			this.topLeftRadius = topLeft;
			this.topRightRadius = topRight;
			this.bottomRightRadius = bottomRight;
			this.bottomLeftRadius = bottomLeft;
		}

		public void SetRadius(float radius)
		{
			this.SetRadius(radius, radius, radius, radius);
		}

		public Vector4 GetWidths()
		{
			return new Vector4(this.leftWidth, this.topWidth, this.rightWidth, this.bottomWidth);
		}

		public Vector4 GetRadiuses()
		{
			return new Vector4(this.topLeftRadius, this.topRightRadius, this.bottomRightRadius, this.bottomLeftRadius);
		}

		public static void SetFromStyle(ref BorderParameters border, IStyle style)
		{
			border.SetWidth(style.borderTopWidth, style.borderRightWidth, style.borderBottomWidth, style.borderLeftWidth);
			border.SetRadius(style.borderTopLeftRadius, style.borderTopRightRadius, style.borderBottomRightRadius, style.borderBottomLeftRadius);
		}

		public float leftWidth;

		public float topWidth;

		public float rightWidth;

		public float bottomWidth;

		public float topLeftRadius;

		public float topRightRadius;

		public float bottomRightRadius;

		public float bottomLeftRadius;
	}
}
