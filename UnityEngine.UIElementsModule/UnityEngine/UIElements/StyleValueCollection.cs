using System;
using System.Collections.Generic;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal class StyleValueCollection
	{
		public StyleLength GetStyleLength(StylePropertyId id)
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

		public StyleFloat GetStyleFloat(StylePropertyId id)
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

		public StyleInt GetStyleInt(StylePropertyId id)
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

		public StyleColor GetStyleColor(StylePropertyId id)
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

		public StyleBackground GetStyleBackground(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			if (flag)
			{
				Texture2D texture2D = (styleValue.resource.IsAllocated ? (styleValue.resource.Target as Texture2D) : null);
				bool flag2 = texture2D != null;
				if (flag2)
				{
					return new StyleBackground(texture2D, styleValue.keyword);
				}
				Sprite sprite = (styleValue.resource.IsAllocated ? (styleValue.resource.Target as Sprite) : null);
				bool flag3 = sprite != null;
				if (flag3)
				{
					return new StyleBackground(sprite, styleValue.keyword);
				}
				VectorImage vectorImage = (styleValue.resource.IsAllocated ? (styleValue.resource.Target as VectorImage) : null);
				bool flag4 = vectorImage != null;
				if (flag4)
				{
					return new StyleBackground(vectorImage, styleValue.keyword);
				}
			}
			return StyleKeyword.Null;
		}

		public StyleBackgroundPosition GetStyleBackgroundPosition(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleBackgroundPosition styleBackgroundPosition;
			if (flag)
			{
				styleBackgroundPosition = new StyleBackgroundPosition(styleValue.position);
			}
			else
			{
				styleBackgroundPosition = StyleKeyword.Null;
			}
			return styleBackgroundPosition;
		}

		public StyleBackgroundRepeat GetStyleBackgroundRepeat(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleBackgroundRepeat styleBackgroundRepeat;
			if (flag)
			{
				styleBackgroundRepeat = new StyleBackgroundRepeat(styleValue.repeat);
			}
			else
			{
				styleBackgroundRepeat = StyleKeyword.Null;
			}
			return styleBackgroundRepeat;
		}

		public StyleFont GetStyleFont(StylePropertyId id)
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

		public StyleFontDefinition GetStyleFontDefinition(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleFontDefinition styleFontDefinition;
			if (flag)
			{
				object obj = (styleValue.resource.IsAllocated ? styleValue.resource.Target : null);
				styleFontDefinition = new StyleFontDefinition(obj, styleValue.keyword);
			}
			else
			{
				styleFontDefinition = StyleKeyword.Null;
			}
			return styleFontDefinition;
		}

		public StyleMaterialDefinition GetStyleMaterialDefinition(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			if (flag)
			{
				object obj = (styleValue.resource.IsAllocated ? styleValue.resource.Target : null);
				bool flag2 = obj != null;
				if (flag2)
				{
					return new StyleMaterialDefinition(obj, styleValue.keyword);
				}
			}
			return StyleKeyword.Null;
		}

		public StyleRatio GetStyleRatio(StylePropertyId id)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = this.TryGetStyleValue(id, ref styleValue);
			StyleRatio styleRatio;
			if (flag)
			{
				styleRatio = new StyleRatio(styleValue.number);
			}
			else
			{
				styleRatio = StyleKeyword.Null;
			}
			return styleRatio;
		}

		public bool TryGetStyleValue(StylePropertyId id, ref StyleValue value)
		{
			value.id = StylePropertyId.Unknown;
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
					bool flag2 = value.keyword == StyleKeyword.Null;
					if (flag2)
					{
						this.m_Values.RemoveAt(i);
					}
					else
					{
						this.m_Values[i] = value;
					}
					return;
				}
			}
			this.m_Values.Add(value);
		}

		internal List<StyleValue> m_Values = new List<StyleValue>();
	}
}
