using System;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.Yoga;

namespace UnityEngine.UIElements
{
	internal static class StyleValueExtensions
	{
		internal static StyleFloat ToStyleFloat(this StyleLength styleLength)
		{
			return new StyleFloat(styleLength.value.value, styleLength.keyword);
		}

		internal static StyleEnum<T> ToStyleEnum<T>(this StyleInt styleInt, T value) where T : struct, IConvertible
		{
			return new StyleEnum<T>(value, styleInt.keyword);
		}

		internal static StyleLength ToStyleLength(this StyleValue styleValue)
		{
			return new StyleLength(new Length(styleValue.number), styleValue.keyword);
		}

		internal static StyleFloat ToStyleFloat(this StyleValue styleValue)
		{
			return new StyleFloat(styleValue.number, styleValue.keyword);
		}

		internal static string DebugString<T>(this IStyleValue<T> styleValue)
		{
			return (styleValue.keyword != StyleKeyword.Undefined) ? string.Format("{0}", styleValue.keyword) : string.Format("{0}", styleValue.value);
		}

		internal static YogaValue ToYogaValue(this StyleLength styleValue)
		{
			bool flag = styleValue.keyword == StyleKeyword.Auto;
			YogaValue yogaValue;
			if (flag)
			{
				yogaValue = YogaValue.Auto();
			}
			else
			{
				bool flag2 = styleValue.keyword == StyleKeyword.None;
				if (flag2)
				{
					yogaValue = float.NaN;
				}
				else
				{
					Length value = styleValue.value;
					LengthUnit unit = value.unit;
					LengthUnit lengthUnit = unit;
					if (lengthUnit != LengthUnit.Pixel)
					{
						if (lengthUnit != LengthUnit.Percent)
						{
							Debug.LogAssertion(string.Format("Unexpected unit '{0}'", value.unit));
							yogaValue = float.NaN;
						}
						else
						{
							yogaValue = YogaValue.Percent(value.value);
						}
					}
					else
					{
						yogaValue = YogaValue.Point(value.value);
					}
				}
			}
			return yogaValue;
		}

		internal static StyleKeyword ToStyleKeyword(this StyleValueKeyword styleValueKeyword)
		{
			StyleKeyword styleKeyword;
			if (styleValueKeyword != StyleValueKeyword.Initial)
			{
				if (styleValueKeyword != StyleValueKeyword.Auto)
				{
					if (styleValueKeyword != StyleValueKeyword.None)
					{
						styleKeyword = StyleKeyword.Undefined;
					}
					else
					{
						styleKeyword = StyleKeyword.None;
					}
				}
				else
				{
					styleKeyword = StyleKeyword.Auto;
				}
			}
			else
			{
				styleKeyword = StyleKeyword.Initial;
			}
			return styleKeyword;
		}
	}
}
