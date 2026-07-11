using System;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements.Experimental
{
	public struct StyleValues
	{
		public float top
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.PositionTop).value;
			}
			set
			{
				this.SetValue(StylePropertyID.PositionTop, value);
			}
		}

		public float left
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.PositionLeft).value;
			}
			set
			{
				this.SetValue(StylePropertyID.PositionLeft, value);
			}
		}

		public float width
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.Width).value;
			}
			set
			{
				this.SetValue(StylePropertyID.Width, value);
			}
		}

		public float height
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.Height).value;
			}
			set
			{
				this.SetValue(StylePropertyID.Height, value);
			}
		}

		public float right
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.PositionRight).value;
			}
			set
			{
				this.SetValue(StylePropertyID.PositionRight, value);
			}
		}

		public float bottom
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.PositionBottom).value;
			}
			set
			{
				this.SetValue(StylePropertyID.PositionBottom, value);
			}
		}

		public Color color
		{
			get
			{
				return this.Values().GetStyleColor(StylePropertyID.Color).value;
			}
			set
			{
				this.SetValue(StylePropertyID.Color, value);
			}
		}

		public Color backgroundColor
		{
			get
			{
				return this.Values().GetStyleColor(StylePropertyID.BackgroundColor).value;
			}
			set
			{
				this.SetValue(StylePropertyID.BackgroundColor, value);
			}
		}

		public Color unityBackgroundImageTintColor
		{
			get
			{
				return this.Values().GetStyleColor(StylePropertyID.BackgroundImageTintColor).value;
			}
			set
			{
				this.SetValue(StylePropertyID.BackgroundImageTintColor, value);
			}
		}

		public Color borderColor
		{
			get
			{
				return this.Values().GetStyleColor(StylePropertyID.BorderColor).value;
			}
			set
			{
				this.SetValue(StylePropertyID.BorderColor, value);
			}
		}

		public float marginLeft
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.MarginLeft).value;
			}
			set
			{
				this.SetValue(StylePropertyID.MarginLeft, value);
			}
		}

		public float marginTop
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.MarginTop).value;
			}
			set
			{
				this.SetValue(StylePropertyID.MarginTop, value);
			}
		}

		public float marginRight
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.MarginRight).value;
			}
			set
			{
				this.SetValue(StylePropertyID.MarginRight, value);
			}
		}

		public float marginBottom
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.MarginBottom).value;
			}
			set
			{
				this.SetValue(StylePropertyID.MarginBottom, value);
			}
		}

		public float paddingLeft
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.PaddingLeft).value;
			}
			set
			{
				this.SetValue(StylePropertyID.PaddingLeft, value);
			}
		}

		public float paddingTop
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.PaddingTop).value;
			}
			set
			{
				this.SetValue(StylePropertyID.PaddingTop, value);
			}
		}

		public float paddingRight
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.PaddingRight).value;
			}
			set
			{
				this.SetValue(StylePropertyID.PaddingRight, value);
			}
		}

		public float paddingBottom
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.PaddingBottom).value;
			}
			set
			{
				this.SetValue(StylePropertyID.PaddingBottom, value);
			}
		}

		public float borderLeftWidth
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.BorderLeftWidth).value;
			}
			set
			{
				this.SetValue(StylePropertyID.BorderLeftWidth, value);
			}
		}

		public float borderRightWidth
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.BorderRightWidth).value;
			}
			set
			{
				this.SetValue(StylePropertyID.BorderRightWidth, value);
			}
		}

		public float borderTopWidth
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.BorderTopWidth).value;
			}
			set
			{
				this.SetValue(StylePropertyID.BorderTopWidth, value);
			}
		}

		public float borderBottomWidth
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.BorderBottomWidth).value;
			}
			set
			{
				this.SetValue(StylePropertyID.BorderBottomWidth, value);
			}
		}

		public float borderTopLeftRadius
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.BorderTopLeftRadius).value;
			}
			set
			{
				this.SetValue(StylePropertyID.BorderTopLeftRadius, value);
			}
		}

		public float borderTopRightRadius
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.BorderTopRightRadius).value;
			}
			set
			{
				this.SetValue(StylePropertyID.BorderTopRightRadius, value);
			}
		}

		public float borderBottomLeftRadius
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.BorderBottomLeftRadius).value;
			}
			set
			{
				this.SetValue(StylePropertyID.BorderBottomLeftRadius, value);
			}
		}

		public float borderBottomRightRadius
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.BorderBottomRightRadius).value;
			}
			set
			{
				this.SetValue(StylePropertyID.BorderBottomRightRadius, value);
			}
		}

		public float opacity
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.Opacity).value;
			}
			set
			{
				this.SetValue(StylePropertyID.Opacity, value);
			}
		}

		public float flexGrow
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.FlexGrow).value;
			}
			set
			{
				this.SetValue(StylePropertyID.FlexGrow, value);
			}
		}

		public float flexShrink
		{
			get
			{
				return this.Values().GetStyleFloat(StylePropertyID.FlexShrink).value;
			}
			set
			{
				this.SetValue(StylePropertyID.FlexGrow, value);
			}
		}

		internal void SetValue(StylePropertyID id, float value)
		{
			StyleValue styleValue = default(StyleValue);
			styleValue.id = id;
			styleValue.number = value;
			this.Values().SetStyleValue(styleValue);
		}

		internal void SetValue(StylePropertyID id, Color value)
		{
			StyleValue styleValue = default(StyleValue);
			styleValue.id = id;
			styleValue.color = value;
			this.Values().SetStyleValue(styleValue);
		}

		internal StyleValueCollection Values()
		{
			bool flag = this.m_StyleValues == null;
			if (flag)
			{
				this.m_StyleValues = new StyleValueCollection();
			}
			return this.m_StyleValues;
		}

		internal StyleValueCollection m_StyleValues;
	}
}
