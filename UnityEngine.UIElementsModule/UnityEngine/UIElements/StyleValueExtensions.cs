using System;
using System.Collections.Generic;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements
{
	internal static class StyleValueExtensions
	{
		internal static string DebugString<T>(this IStyleValue<T> styleValue)
		{
			return (styleValue.keyword != StyleKeyword.Undefined) ? string.Format("{0}", styleValue.keyword) : string.Format("{0}", styleValue.value);
		}

		internal static LayoutValue ToLayoutValue(this Length length)
		{
			bool flag = length.IsAuto();
			LayoutValue layoutValue;
			if (flag)
			{
				layoutValue = LayoutValue.Auto();
			}
			else
			{
				bool flag2 = length.IsNone();
				if (flag2)
				{
					layoutValue = float.NaN;
				}
				else
				{
					LengthUnit unit = length.unit;
					LengthUnit lengthUnit = unit;
					if (lengthUnit != LengthUnit.Pixel)
					{
						if (lengthUnit != LengthUnit.Percent)
						{
							Debug.LogAssertion(string.Format("Unexpected unit '{0}'", length.unit));
							layoutValue = float.NaN;
						}
						else
						{
							layoutValue = LayoutValue.Percent(length.value);
						}
					}
					else
					{
						layoutValue = LayoutValue.Point(length.value);
					}
				}
			}
			return layoutValue;
		}

		internal static Length ToLength(this StyleKeyword keyword)
		{
			StyleKeyword styleKeyword = keyword;
			StyleKeyword styleKeyword2 = styleKeyword;
			Length length;
			if (styleKeyword2 != StyleKeyword.Auto)
			{
				if (styleKeyword2 != StyleKeyword.None)
				{
					Debug.LogAssertion("Unexpected StyleKeyword '" + keyword.ToString() + "'");
					length = default(Length);
				}
				else
				{
					length = Length.None();
				}
			}
			else
			{
				length = Length.Auto();
			}
			return length;
		}

		internal static Rotate ToRotate(this StyleKeyword keyword)
		{
			StyleKeyword styleKeyword = keyword;
			StyleKeyword styleKeyword2 = styleKeyword;
			Rotate rotate;
			if (styleKeyword2 != StyleKeyword.None)
			{
				Debug.LogAssertion("Unexpected StyleKeyword '" + keyword.ToString() + "'");
				rotate = default(Rotate);
			}
			else
			{
				rotate = Rotate.None();
			}
			return rotate;
		}

		internal static Scale ToScale(this StyleKeyword keyword)
		{
			StyleKeyword styleKeyword = keyword;
			StyleKeyword styleKeyword2 = styleKeyword;
			Scale scale;
			if (styleKeyword2 != StyleKeyword.None)
			{
				Debug.LogAssertion("Unexpected StyleKeyword '" + keyword.ToString() + "'");
				scale = default(Scale);
			}
			else
			{
				scale = Scale.None();
			}
			return scale;
		}

		internal static Translate ToTranslate(this StyleKeyword keyword)
		{
			StyleKeyword styleKeyword = keyword;
			StyleKeyword styleKeyword2 = styleKeyword;
			Translate translate;
			if (styleKeyword2 != StyleKeyword.None)
			{
				Debug.LogAssertion("Unexpected StyleKeyword '" + keyword.ToString() + "'");
				translate = default(Translate);
			}
			else
			{
				translate = Translate.None();
			}
			return translate;
		}

		internal static TextAutoSize ToTextAutoSize(this StyleKeyword keyword)
		{
			StyleKeyword styleKeyword = keyword;
			StyleKeyword styleKeyword2 = styleKeyword;
			TextAutoSize textAutoSize;
			if (styleKeyword2 != StyleKeyword.None)
			{
				Debug.LogAssertion("Unexpected StyleKeyword '" + keyword.ToString() + "'");
				textAutoSize = default(TextAutoSize);
			}
			else
			{
				textAutoSize = TextAutoSize.None();
			}
			return textAutoSize;
		}

		internal static Length ToLength(this StyleLength styleLength)
		{
			StyleKeyword keyword = styleLength.keyword;
			StyleKeyword styleKeyword = keyword;
			Length length;
			if (styleKeyword - StyleKeyword.Auto > 1)
			{
				length = styleLength.value;
			}
			else
			{
				length = styleLength.keyword.ToLength();
			}
			return length;
		}

		internal static StyleRatio ToStyleRatio(this StyleKeyword keyword)
		{
			StyleKeyword styleKeyword = keyword;
			StyleKeyword styleKeyword2 = styleKeyword;
			StyleRatio styleRatio;
			if (styleKeyword2 != StyleKeyword.Auto)
			{
				Debug.LogAssertion("Unexpected StyleKeyword '" + keyword.ToString() + "'");
				styleRatio = default(StyleRatio);
			}
			else
			{
				styleRatio = StyleRatio.Auto();
			}
			return styleRatio;
		}

		internal static void CopyFrom<T>(this List<T> list, List<T> other)
		{
			list.Clear();
			list.AddRange(other);
		}
	}
}
