using System;

namespace UnityEngine.UIElements.StyleSheets
{
	internal class StyleValuePropertyReader : IStylePropertyReader
	{
		public StylePropertyID propertyID { get; private set; }

		public int specificity { get; private set; }

		public int valueCount
		{
			get
			{
				return 1;
			}
		}

		public void Set(StylePropertyID id, StyleValue value, int spec)
		{
			this.propertyID = id;
			this.m_CurrentStyleValue = value;
			this.specificity = spec;
		}

		public void Set(StyleCursor cursor, int spec)
		{
			this.propertyID = StylePropertyID.Cursor;
			this.m_CurrentCursor = cursor;
			this.specificity = spec;
		}

		public bool IsValueType(int index, StyleValueType type)
		{
			bool flag = type == StyleValueType.Keyword;
			bool flag2;
			if (flag)
			{
				flag2 = this.m_CurrentStyleValue.keyword > StyleKeyword.Undefined;
			}
			else
			{
				flag2 = this.m_CurrentStyleValue.keyword == StyleKeyword.Undefined;
			}
			return flag2;
		}

		public bool IsKeyword(int index, StyleValueKeyword keyword)
		{
			bool flag = this.m_CurrentStyleValue.keyword == StyleKeyword.Undefined;
			return !flag && this.m_CurrentStyleValue.keyword == keyword.ToStyleKeyword();
		}

		public string ReadAsString(int index)
		{
			bool flag = this.m_CurrentStyleValue.keyword > StyleKeyword.Undefined;
			string text;
			if (flag)
			{
				text = this.m_CurrentStyleValue.keyword.ToString();
			}
			else
			{
				text = this.m_CurrentStyleValue.number.ToString();
			}
			return text;
		}

		public StyleLength ReadStyleLength(int index)
		{
			return new StyleLength(this.m_CurrentStyleValue.length, this.m_CurrentStyleValue.keyword)
			{
				specificity = this.specificity
			};
		}

		public StyleFloat ReadStyleFloat(int index)
		{
			return new StyleFloat(this.m_CurrentStyleValue.number, this.m_CurrentStyleValue.keyword)
			{
				specificity = this.specificity
			};
		}

		public StyleInt ReadStyleInt(int index)
		{
			return new StyleInt((int)this.m_CurrentStyleValue.number, this.m_CurrentStyleValue.keyword)
			{
				specificity = this.specificity
			};
		}

		public StyleColor ReadStyleColor(int index)
		{
			return new StyleColor(this.m_CurrentStyleValue.color, this.m_CurrentStyleValue.keyword)
			{
				specificity = this.specificity
			};
		}

		public StyleInt ReadStyleEnum<T>(int index)
		{
			return new StyleInt((int)this.m_CurrentStyleValue.number, this.m_CurrentStyleValue.keyword)
			{
				specificity = this.specificity
			};
		}

		public StyleFont ReadStyleFont(int index)
		{
			Font font = null;
			bool isAllocated = this.m_CurrentStyleValue.resource.IsAllocated;
			if (isAllocated)
			{
				font = this.m_CurrentStyleValue.resource.Target as Font;
			}
			return new StyleFont(font, this.m_CurrentStyleValue.keyword)
			{
				specificity = this.specificity
			};
		}

		public StyleBackground ReadStyleBackground(int index)
		{
			StyleBackground styleBackground = new StyleBackground(this.m_CurrentStyleValue.keyword);
			bool isAllocated = this.m_CurrentStyleValue.resource.IsAllocated;
			if (isAllocated)
			{
				Texture2D texture2D = this.m_CurrentStyleValue.resource.Target as Texture2D;
				bool flag = texture2D != null;
				if (flag)
				{
					styleBackground = new StyleBackground(texture2D, this.m_CurrentStyleValue.keyword);
				}
				else
				{
					VectorImage vectorImage = this.m_CurrentStyleValue.resource.Target as VectorImage;
					bool flag2 = vectorImage != null;
					if (flag2)
					{
						styleBackground = new StyleBackground(vectorImage, this.m_CurrentStyleValue.keyword);
					}
				}
			}
			styleBackground.specificity = this.specificity;
			return styleBackground;
		}

		public StyleCursor ReadStyleCursor(int index)
		{
			return new StyleCursor(this.m_CurrentCursor.value, this.m_CurrentCursor.keyword)
			{
				specificity = this.specificity
			};
		}

		private StyleValue m_CurrentStyleValue;

		private StyleCursor m_CurrentCursor;
	}
}
