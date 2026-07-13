using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UIElements.StyleSheets.Syntax;

namespace UnityEngine.UIElements.StyleSheets
{
	internal class StylePropertyValueMatcher : BaseStyleMatcher
	{
		private StylePropertyValue current
		{
			get
			{
				return base.hasCurrent ? this.m_Values[base.currentIndex] : default(StylePropertyValue);
			}
		}

		public override int valueCount
		{
			get
			{
				return this.m_Values.Count;
			}
		}

		public override bool isCurrentVariable
		{
			get
			{
				return false;
			}
		}

		public override bool isCurrentComma
		{
			get
			{
				return base.hasCurrent && this.m_Values[base.currentIndex].handle.valueType == StyleValueType.CommaSeparator;
			}
		}

		public MatchResult Match(Expression exp, List<StylePropertyValue> values)
		{
			MatchResult matchResult = new MatchResult
			{
				errorCode = MatchResultErrorCode.None
			};
			bool flag = values == null || values.Count == 0;
			MatchResult matchResult2;
			if (flag)
			{
				matchResult.errorCode = MatchResultErrorCode.EmptyValue;
				matchResult2 = matchResult;
			}
			else
			{
				base.Initialize();
				this.m_Values = values;
				StyleValueHandle handle = this.m_Values[0].handle;
				bool flag2 = handle.valueType == StyleValueType.Keyword && handle.valueIndex == 1;
				bool flag3;
				if (flag2)
				{
					base.MoveNext();
					flag3 = true;
				}
				else
				{
					flag3 = base.Match(exp);
				}
				bool flag4 = !flag3;
				if (flag4)
				{
					StyleSheet sheet = this.current.sheet;
					matchResult.errorCode = MatchResultErrorCode.Syntax;
					matchResult.errorValue = sheet.ReadAsString(this.current.handle);
				}
				else
				{
					bool hasCurrent = base.hasCurrent;
					if (hasCurrent)
					{
						StyleSheet sheet2 = this.current.sheet;
						matchResult.errorCode = MatchResultErrorCode.ExpectedEndOfValue;
						matchResult.errorValue = sheet2.ReadAsString(this.current.handle);
					}
				}
				matchResult2 = matchResult;
			}
			return matchResult2;
		}

		protected override bool MatchKeyword(string keyword)
		{
			StylePropertyValue current = this.current;
			bool flag = current.handle.valueType == StyleValueType.Keyword;
			bool flag2;
			if (flag)
			{
				StyleValueKeyword valueIndex = (StyleValueKeyword)current.handle.valueIndex;
				flag2 = valueIndex.ToUssString().Equals(keyword, StringComparison.OrdinalIgnoreCase);
			}
			else
			{
				bool flag3 = current.handle.valueType == StyleValueType.Enum;
				if (flag3)
				{
					string text = current.sheet.ReadEnum(current.handle);
					flag2 = text.Equals(keyword, StringComparison.OrdinalIgnoreCase);
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		protected override bool MatchNumber(Expression exp)
		{
			StylePropertyValue current = this.current;
			bool flag = current.handle.valueType == StyleValueType.Float;
			bool flag2;
			if (flag)
			{
				float num = current.sheet.ReadFloat(current.handle);
				flag2 = exp.min <= num && num <= exp.max;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		protected override bool MatchInteger()
		{
			return this.current.handle.valueType == StyleValueType.Float;
		}

		protected override bool MatchLength()
		{
			StylePropertyValue current = this.current;
			bool flag = current.handle.valueType == StyleValueType.Dimension;
			bool flag2;
			if (flag)
			{
				Dimension dimension = current.sheet.ReadDimension(current.handle);
				flag2 = dimension.unit == Dimension.Unit.Pixel;
			}
			else
			{
				bool flag3 = current.handle.valueType == StyleValueType.Float;
				if (flag3)
				{
					float num = current.sheet.ReadFloat(current.handle);
					flag2 = Mathf.Approximately(0f, num);
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		protected override bool MatchPercentage()
		{
			StylePropertyValue current = this.current;
			bool flag = current.handle.valueType == StyleValueType.Dimension;
			bool flag2;
			if (flag)
			{
				Dimension dimension = current.sheet.ReadDimension(current.handle);
				flag2 = dimension.unit == Dimension.Unit.Percent;
			}
			else
			{
				bool flag3 = current.handle.valueType == StyleValueType.Float;
				if (flag3)
				{
					float num = current.sheet.ReadFloat(current.handle);
					flag2 = Mathf.Approximately(0f, num);
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		protected override bool MatchColor()
		{
			StylePropertyValue current = this.current;
			bool flag = current.handle.valueType == StyleValueType.Color;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = current.handle.valueType == StyleValueType.Enum;
				if (flag3)
				{
					Color clear = Color.clear;
					string text = current.sheet.ReadAsString(current.handle);
					bool flag4 = StyleSheetColor.TryGetColor(text, out clear);
					if (flag4)
					{
						return true;
					}
				}
				flag2 = false;
			}
			return flag2;
		}

		protected override bool MatchResource()
		{
			return this.current.handle.valueType == StyleValueType.ResourcePath;
		}

		protected override bool MatchUrl()
		{
			StyleValueType valueType = this.current.handle.valueType;
			return valueType == StyleValueType.AssetReference || valueType == StyleValueType.ScalableImage;
		}

		protected override bool MatchTime()
		{
			StylePropertyValue current = this.current;
			bool flag = current.handle.valueType == StyleValueType.Dimension;
			bool flag2;
			if (flag)
			{
				Dimension dimension = current.sheet.ReadDimension(current.handle);
				flag2 = dimension.unit == Dimension.Unit.Second || dimension.unit == Dimension.Unit.Millisecond;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		protected override bool MatchFilterFunction()
		{
			int valueIndex = this.current.handle.valueIndex;
			base.MoveNext();
			StylePropertyValue current = this.current;
			int num = (int)current.sheet.ReadFloat(current.handle);
			for (int i = 0; i < num; i++)
			{
				base.MoveNext();
			}
			return true;
		}

		protected override bool MatchMaterialPropertyValue()
		{
			int valueIndex = this.current.handle.valueIndex;
			base.MoveNext();
			StylePropertyValue current = this.current;
			int num = (int)current.sheet.ReadFloat(current.handle);
			for (int i = 0; i < num; i++)
			{
				base.MoveNext();
			}
			return true;
		}

		protected override bool MatchCustomIdent()
		{
			StylePropertyValue current = this.current;
			bool flag = current.handle.valueType == StyleValueType.Enum;
			bool flag2;
			if (flag)
			{
				string text = current.sheet.ReadAsString(current.handle);
				Match match = BaseStyleMatcher.s_CustomIdentRegex.Match(text);
				flag2 = match.Success && match.Length == text.Length;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		protected override bool MatchAngle()
		{
			StylePropertyValue current = this.current;
			bool flag = current.handle.valueType == StyleValueType.Dimension;
			if (flag)
			{
				Dimension dimension = current.sheet.ReadDimension(current.handle);
				Dimension.Unit unit = dimension.unit;
				Dimension.Unit unit2 = unit;
				if (unit2 - Dimension.Unit.Degree <= 3)
				{
					return true;
				}
			}
			bool flag2 = current.handle.valueType == StyleValueType.Float;
			bool flag3;
			if (flag2)
			{
				float num = current.sheet.ReadFloat(current.handle);
				flag3 = Mathf.Approximately(0f, num);
			}
			else
			{
				flag3 = false;
			}
			return flag3;
		}

		private List<StylePropertyValue> m_Values;
	}
}
