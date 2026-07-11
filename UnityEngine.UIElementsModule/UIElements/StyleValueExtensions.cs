using System;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.Yoga;

namespace UnityEngine.UIElements
{
	internal static class StyleValueExtensions
	{
		internal static StyleFloat ToStyleFloat(this StyleLength styleLength)
		{
			return new StyleFloat(styleLength.value.value, styleLength.keyword)
			{
				specificity = styleLength.specificity
			};
		}

		internal static StyleEnum<T> ToStyleEnum<T>(this StyleInt styleInt, T value) where T : struct, IConvertible
		{
			return new StyleEnum<T>(value, styleInt.keyword)
			{
				specificity = styleInt.specificity
			};
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

		internal static U GetSpecifiedValueOrDefault<T, U>(this T styleValue, U defaultValue) where T : IStyleValue<U>
		{
			bool flag = styleValue.specificity != 0;
			U u;
			if (flag)
			{
				u = styleValue.value;
			}
			else
			{
				u = defaultValue;
			}
			return u;
		}

		internal static float GetSpecifiedValueOrDefault(this StyleLength styleValue, float defaultValue)
		{
			bool flag = styleValue.specificity != 0;
			float num;
			if (flag)
			{
				num = styleValue.value.value;
			}
			else
			{
				num = defaultValue;
			}
			return num;
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
					bool flag3 = styleValue.specificity != 0;
					if (flag3)
					{
						Length value = styleValue.value;
						LengthUnit unit = value.unit;
						if (unit != LengthUnit.Pixel)
						{
							if (unit != LengthUnit.Percent)
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
					else
					{
						yogaValue = float.NaN;
					}
				}
			}
			return yogaValue;
		}

		internal static bool CanApply(int specificity, int otherSpecificity, StylePropertyApplyMode mode)
		{
			bool flag;
			switch (mode)
			{
			case StylePropertyApplyMode.Copy:
				flag = true;
				break;
			case StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity:
			{
				bool flag2 = specificity == 0 && otherSpecificity == -1;
				flag = flag2 || otherSpecificity >= specificity;
				break;
			}
			case StylePropertyApplyMode.CopyIfNotInline:
				flag = specificity < int.MaxValue;
				break;
			default:
				Debug.Assert(false, "Invalid mode " + mode);
				flag = false;
				break;
			}
			return flag;
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

		internal const int UndefinedSpecificity = 0;

		internal const int UnitySpecificity = -1;

		internal const int InlineSpecificity = 2147483647;
	}
}
