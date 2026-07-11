using System;
using System.Collections.Generic;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal class StyleValueCollection
	{
		public StyleLength GetStyleLength(StylePropertyID id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleLength styleLength;
			if (flag)
			{
				styleLength = new StyleLength(styleValue.length, styleValue.keyword);
			}
			else
			{
				styleLength = StyleKeyword.Null;
			}
			return styleLength;
		}

		public StyleFloat GetStyleFloat(StylePropertyID id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleFloat styleFloat;
			if (flag)
			{
				styleFloat = new StyleFloat(styleValue.number, styleValue.keyword);
			}
			else
			{
				styleFloat = StyleKeyword.Null;
			}
			return styleFloat;
		}

		public StyleInt GetStyleInt(StylePropertyID id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleInt styleInt;
			if (flag)
			{
				styleInt = new StyleInt((int)styleValue.number, styleValue.keyword);
			}
			else
			{
				styleInt = StyleKeyword.Null;
			}
			return styleInt;
		}

		public StyleColor GetStyleColor(StylePropertyID id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleColor styleColor;
			if (flag)
			{
				styleColor = new StyleColor(styleValue.color, styleValue.keyword);
			}
			else
			{
				styleColor = StyleKeyword.Null;
			}
			return styleColor;
		}

		public StyleBackground GetStyleBackground(StylePropertyID id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleBackground styleBackground;
			if (flag)
			{
				Texture2D texture2D = (styleValue.resource.IsAllocated ? (styleValue.resource.Target as Texture2D) : null);
				styleBackground = new StyleBackground(texture2D, styleValue.keyword);
			}
			else
			{
				styleBackground = StyleKeyword.Null;
			}
			return styleBackground;
		}

		public StyleFont GetStyleFont(StylePropertyID id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleFont styleFont;
			if (flag)
			{
				Font font = (styleValue.resource.IsAllocated ? (styleValue.resource.Target as Font) : null);
				styleFont = new StyleFont(font, styleValue.keyword);
			}
			else
			{
				styleFont = StyleKeyword.Null;
			}
			return styleFont;
		}

		public bool TryGetStyleValue(StylePropertyID id, ref StyleValue value)
		{
			value.id = StylePropertyID.Unknown;
			foreach (StyleValue styleValue in this.m_Values)
			{
				bool flag = styleValue.id == id;
				if (flag)
				{
					value = styleValue;
					return true;
				}
			}
			return false;
		}

		public void SetStyleValue(StyleValue value)
		{
			for (int i = 0; i < this.m_Values.Count; i++)
			{
				bool flag = this.m_Values[i].id == value.id;
				if (flag)
				{
					this.m_Values[i] = value;
					return;
				}
			}
			this.m_Values.Add(value);
		}

		internal List<StyleValue> m_Values = new List<StyleValue>();
	}
}
