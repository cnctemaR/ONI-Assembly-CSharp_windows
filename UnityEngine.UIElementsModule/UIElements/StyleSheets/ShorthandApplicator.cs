using System;

namespace UnityEngine.UIElements.StyleSheets
{
	internal static class ShorthandApplicator
	{
		public static void ApplyBorderColor(StylePropertyReader reader, VisualElementStylesData styleData)
		{
			StyleColor styleColor;
			StyleColor styleColor2;
			StyleColor styleColor3;
			StyleColor styleColor4;
			ShorthandApplicator.CompileBoxArea(reader, out styleColor, out styleColor2, out styleColor3, out styleColor4);
			bool flag = styleColor.keyword > StyleKeyword.Undefined;
			if (flag)
			{
				styleColor.value = Color.clear;
			}
			bool flag2 = styleColor2.keyword > StyleKeyword.Undefined;
			if (flag2)
			{
				styleColor2.value = Color.clear;
			}
			bool flag3 = styleColor3.keyword > StyleKeyword.Undefined;
			if (flag3)
			{
				styleColor3.value = Color.clear;
			}
			bool flag4 = styleColor4.keyword > StyleKeyword.Undefined;
			if (flag4)
			{
				styleColor4.value = Color.clear;
			}
			styleData.borderTopColor.Apply<StyleColor>(styleColor, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity);
			styleData.borderRightColor.Apply<StyleColor>(styleColor2, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity);
			styleData.borderBottomColor.Apply<StyleColor>(styleColor3, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity);
			styleData.borderLeftColor.Apply<StyleColor>(styleColor4, StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity);
		}

		public static void ApplyBorderRadius(StylePropertyReader reader, VisualElementStylesData styleData)
		{
			StyleLength styleLength;
			StyleLength styleLength2;
			StyleLength styleLength3;
			StyleLength styleLength4;
			ShorthandApplicator.CompileBoxArea(reader, out styleLength, out styleLength2, out styleLength3, out styleLength4);
			bool flag = styleLength.keyword > StyleKeyword.Undefined;
			if (flag)
			{
				styleLength.value = 0f;
			}
			bool flag2 = styleLength2.keyword > StyleKeyword.Undefined;
			if (flag2)
			{
				styleLength2.value = 0f;
			}
			bool flag3 = styleLength4.keyword > StyleKeyword.Undefined;
			if (flag3)
			{
				styleLength4.value = 0f;
			}
			bool flag4 = styleLength3.keyword > StyleKeyword.Undefined;
			if (flag4)
			{
				styleLength3.value = 0f;
			}
			styleData.borderTopLeftRadius = styleLength;
			styleData.borderTopRightRadius = styleLength2;
			styleData.borderBottomLeftRadius = styleLength4;
			styleData.borderBottomRightRadius = styleLength3;
		}

		public static void ApplyBorderWidth(StylePropertyReader reader, VisualElementStylesData styleData)
		{
			StyleLength styleLength;
			StyleLength styleLength2;
			StyleLength styleLength3;
			StyleLength styleLength4;
			ShorthandApplicator.CompileBoxArea(reader, out styleLength, out styleLength2, out styleLength3, out styleLength4);
			bool flag = styleLength.keyword > StyleKeyword.Undefined;
			if (flag)
			{
				styleLength.value = 0f;
			}
			bool flag2 = styleLength2.keyword > StyleKeyword.Undefined;
			if (flag2)
			{
				styleLength2.value = 0f;
			}
			bool flag3 = styleLength3.keyword > StyleKeyword.Undefined;
			if (flag3)
			{
				styleLength3.value = 0f;
			}
			bool flag4 = styleLength4.keyword > StyleKeyword.Undefined;
			if (flag4)
			{
				styleLength4.value = 0f;
			}
			styleData.borderTopWidth = styleLength.ToStyleFloat();
			styleData.borderRightWidth = styleLength2.ToStyleFloat();
			styleData.borderBottomWidth = styleLength3.ToStyleFloat();
			styleData.borderLeftWidth = styleLength4.ToStyleFloat();
		}

		public static void ApplyFlex(StylePropertyReader reader, VisualElementStylesData styleData)
		{
			StyleFloat styleFloat;
			StyleFloat styleFloat2;
			StyleLength styleLength;
			bool flag = ShorthandApplicator.CompileFlexShorthand(reader, out styleFloat, out styleFloat2, out styleLength);
			bool flag2 = flag;
			if (flag2)
			{
				styleData.flexGrow = styleFloat;
				styleData.flexShrink = styleFloat2;
				styleData.flexBasis = styleLength;
			}
		}

		public static void ApplyMargin(StylePropertyReader reader, VisualElementStylesData styleData)
		{
			StyleLength styleLength;
			StyleLength styleLength2;
			StyleLength styleLength3;
			StyleLength styleLength4;
			ShorthandApplicator.CompileBoxArea(reader, out styleLength, out styleLength2, out styleLength3, out styleLength4);
			styleData.marginTop = styleLength;
			styleData.marginRight = styleLength2;
			styleData.marginBottom = styleLength3;
			styleData.marginLeft = styleLength4;
		}

		public static void ApplyPadding(StylePropertyReader reader, VisualElementStylesData styleData)
		{
			StyleLength styleLength;
			StyleLength styleLength2;
			StyleLength styleLength3;
			StyleLength styleLength4;
			ShorthandApplicator.CompileBoxArea(reader, out styleLength, out styleLength2, out styleLength3, out styleLength4);
			styleData.paddingTop = styleLength;
			styleData.paddingRight = styleLength2;
			styleData.paddingBottom = styleLength3;
			styleData.paddingLeft = styleLength4;
		}

		private static bool CompileFlexShorthand(StylePropertyReader reader, out StyleFloat grow, out StyleFloat shrink, out StyleLength basis)
		{
			grow = 0f;
			shrink = 1f;
			basis = StyleKeyword.Auto;
			bool flag = false;
			int valueCount = reader.valueCount;
			bool flag2 = valueCount == 1 && reader.IsValueType(0, StyleValueType.Keyword);
			if (flag2)
			{
				bool flag3 = reader.IsKeyword(0, StyleValueKeyword.None);
				if (flag3)
				{
					flag = true;
					grow = 0f;
					shrink = 0f;
					basis = StyleKeyword.Auto;
				}
				else
				{
					bool flag4 = reader.IsKeyword(0, StyleValueKeyword.Auto);
					if (flag4)
					{
						flag = true;
						grow = 1f;
						shrink = 1f;
						basis = StyleKeyword.Auto;
					}
				}
			}
			else
			{
				bool flag5 = valueCount <= 3;
				if (flag5)
				{
					flag = true;
					grow = 0f;
					shrink = 1f;
					basis = Length.Percent(0f);
					bool flag6 = false;
					bool flag7 = false;
					int num = 0;
					while (num < valueCount && flag)
					{
						StyleValueType valueType = reader.GetValueType(num);
						bool flag8 = valueType == StyleValueType.Dimension || valueType == StyleValueType.Keyword;
						if (flag8)
						{
							bool flag9 = flag7;
							if (flag9)
							{
								flag = false;
								break;
							}
							flag7 = true;
							bool flag10 = valueType == StyleValueType.Keyword;
							if (flag10)
							{
								bool flag11 = reader.IsKeyword(num, StyleValueKeyword.Auto);
								if (flag11)
								{
									basis = StyleKeyword.Auto;
								}
							}
							else
							{
								bool flag12 = valueType == StyleValueType.Dimension;
								if (flag12)
								{
									basis = reader.ReadStyleLength(num);
								}
							}
							bool flag13 = flag6 && num != valueCount - 1;
							if (flag13)
							{
								flag = false;
							}
						}
						else
						{
							bool flag14 = valueType == StyleValueType.Float;
							if (flag14)
							{
								StyleFloat styleFloat = reader.ReadStyleFloat(num);
								bool flag15 = !flag6;
								if (flag15)
								{
									flag6 = true;
									grow = styleFloat;
								}
								else
								{
									shrink = styleFloat;
								}
							}
							else
							{
								flag = false;
							}
						}
						num++;
					}
				}
			}
			grow.specificity = reader.specificity;
			shrink.specificity = reader.specificity;
			basis.specificity = reader.specificity;
			return flag;
		}

		private static void CompileBoxArea(StylePropertyReader reader, out StyleLength top, out StyleLength right, out StyleLength bottom, out StyleLength left)
		{
			top = 0f;
			right = 0f;
			bottom = 0f;
			left = 0f;
			switch (reader.valueCount)
			{
			case 0:
				break;
			case 1:
				top = (right = (bottom = (left = reader.ReadStyleLength(0))));
				break;
			case 2:
				top = (bottom = reader.ReadStyleLength(0));
				left = (right = reader.ReadStyleLength(1));
				break;
			case 3:
				top = reader.ReadStyleLength(0);
				left = (right = reader.ReadStyleLength(1));
				bottom = reader.ReadStyleLength(2);
				break;
			default:
				top = reader.ReadStyleLength(0);
				right = reader.ReadStyleLength(1);
				bottom = reader.ReadStyleLength(2);
				left = reader.ReadStyleLength(3);
				break;
			}
		}

		private static void CompileBoxArea(StylePropertyReader reader, out StyleColor top, out StyleColor right, out StyleColor bottom, out StyleColor left)
		{
			top = Color.clear;
			right = Color.clear;
			bottom = Color.clear;
			left = Color.clear;
			switch (reader.valueCount)
			{
			case 0:
				break;
			case 1:
				top = (right = (bottom = (left = reader.ReadStyleColor(0))));
				break;
			case 2:
				top = (bottom = reader.ReadStyleColor(0));
				left = (right = reader.ReadStyleColor(1));
				break;
			case 3:
				top = reader.ReadStyleColor(0);
				left = (right = reader.ReadStyleColor(1));
				bottom = reader.ReadStyleColor(2);
				break;
			default:
				top = reader.ReadStyleColor(0);
				right = reader.ReadStyleColor(1);
				bottom = reader.ReadStyleColor(2);
				left = reader.ReadStyleColor(3);
				break;
			}
		}
	}
}
