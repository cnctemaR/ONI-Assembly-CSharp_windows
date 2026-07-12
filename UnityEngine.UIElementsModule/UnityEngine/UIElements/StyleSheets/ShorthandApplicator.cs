using System;

namespace UnityEngine.UIElements.StyleSheets
{
	internal static class ShorthandApplicator
	{
		public static void ApplyBorderColor(StylePropertyReader reader, ComputedStyle computedStyle)
		{
			StyleColor styleColor;
			StyleColor styleColor2;
			StyleColor styleColor3;
			StyleColor styleColor4;
			ShorthandApplicator.CompileBoxAreaNoKeyword(reader, out styleColor, out styleColor2, out styleColor3, out styleColor4);
			computedStyle.nonInheritedData.borderTopColor = styleColor;
			computedStyle.nonInheritedData.borderRightColor = styleColor2;
			computedStyle.nonInheritedData.borderBottomColor = styleColor3;
			computedStyle.nonInheritedData.borderLeftColor = styleColor4;
		}

		public static void ApplyBorderRadius(StylePropertyReader reader, ComputedStyle computedStyle)
		{
			StyleLength styleLength;
			StyleLength styleLength2;
			StyleLength styleLength3;
			StyleLength styleLength4;
			ShorthandApplicator.CompileBoxAreaNoKeyword(reader, out styleLength, out styleLength2, out styleLength3, out styleLength4);
			computedStyle.nonInheritedData.borderTopLeftRadius = styleLength;
			computedStyle.nonInheritedData.borderTopRightRadius = styleLength2;
			computedStyle.nonInheritedData.borderBottomRightRadius = styleLength3;
			computedStyle.nonInheritedData.borderBottomLeftRadius = styleLength4;
		}

		public static void ApplyBorderWidth(StylePropertyReader reader, ComputedStyle computedStyle)
		{
			StyleFloat styleFloat;
			StyleFloat styleFloat2;
			StyleFloat styleFloat3;
			StyleFloat styleFloat4;
			ShorthandApplicator.CompileBoxAreaNoKeyword(reader, out styleFloat, out styleFloat2, out styleFloat3, out styleFloat4);
			computedStyle.nonInheritedData.borderTopWidth = styleFloat;
			computedStyle.nonInheritedData.borderRightWidth = styleFloat2;
			computedStyle.nonInheritedData.borderBottomWidth = styleFloat3;
			computedStyle.nonInheritedData.borderLeftWidth = styleFloat4;
		}

		public static void ApplyFlex(StylePropertyReader reader, ComputedStyle computedStyle)
		{
			StyleFloat styleFloat;
			StyleFloat styleFloat2;
			StyleLength styleLength;
			ShorthandApplicator.CompileFlexShorthand(reader, out styleFloat, out styleFloat2, out styleLength);
			computedStyle.nonInheritedData.flexGrow = styleFloat;
			computedStyle.nonInheritedData.flexShrink = styleFloat2;
			computedStyle.nonInheritedData.flexBasis = styleLength;
		}

		public static void ApplyMargin(StylePropertyReader reader, ComputedStyle computedStyle)
		{
			StyleLength styleLength;
			StyleLength styleLength2;
			StyleLength styleLength3;
			StyleLength styleLength4;
			ShorthandApplicator.CompileBoxArea(reader, out styleLength, out styleLength2, out styleLength3, out styleLength4);
			computedStyle.nonInheritedData.marginTop = styleLength;
			computedStyle.nonInheritedData.marginRight = styleLength2;
			computedStyle.nonInheritedData.marginBottom = styleLength3;
			computedStyle.nonInheritedData.marginLeft = styleLength4;
		}

		public static void ApplyPadding(StylePropertyReader reader, ComputedStyle computedStyle)
		{
			StyleLength styleLength;
			StyleLength styleLength2;
			StyleLength styleLength3;
			StyleLength styleLength4;
			ShorthandApplicator.CompileBoxAreaNoKeyword(reader, out styleLength, out styleLength2, out styleLength3, out styleLength4);
			computedStyle.nonInheritedData.paddingTop = styleLength;
			computedStyle.nonInheritedData.paddingRight = styleLength2;
			computedStyle.nonInheritedData.paddingBottom = styleLength3;
			computedStyle.nonInheritedData.paddingLeft = styleLength4;
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

		private static void CompileBoxAreaNoKeyword(StylePropertyReader reader, out StyleLength top, out StyleLength right, out StyleLength bottom, out StyleLength left)
		{
			ShorthandApplicator.CompileBoxArea(reader, out top, out right, out bottom, out left);
			bool flag = top.keyword > StyleKeyword.Undefined;
			if (flag)
			{
				top.value = 0f;
			}
			bool flag2 = right.keyword > StyleKeyword.Undefined;
			if (flag2)
			{
				right.value = 0f;
			}
			bool flag3 = bottom.keyword > StyleKeyword.Undefined;
			if (flag3)
			{
				bottom.value = 0f;
			}
			bool flag4 = left.keyword > StyleKeyword.Undefined;
			if (flag4)
			{
				left.value = 0f;
			}
		}

		private static void CompileBoxAreaNoKeyword(StylePropertyReader reader, out StyleFloat top, out StyleFloat right, out StyleFloat bottom, out StyleFloat left)
		{
			StyleLength styleLength;
			StyleLength styleLength2;
			StyleLength styleLength3;
			StyleLength styleLength4;
			ShorthandApplicator.CompileBoxAreaNoKeyword(reader, out styleLength, out styleLength2, out styleLength3, out styleLength4);
			top = styleLength.ToStyleFloat();
			right = styleLength2.ToStyleFloat();
			bottom = styleLength3.ToStyleFloat();
			left = styleLength4.ToStyleFloat();
		}

		private static void CompileBoxAreaNoKeyword(StylePropertyReader reader, out StyleColor top, out StyleColor right, out StyleColor bottom, out StyleColor left)
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
			bool flag = top.keyword > StyleKeyword.Undefined;
			if (flag)
			{
				top.value = Color.clear;
			}
			bool flag2 = right.keyword > StyleKeyword.Undefined;
			if (flag2)
			{
				right.value = Color.clear;
			}
			bool flag3 = bottom.keyword > StyleKeyword.Undefined;
			if (flag3)
			{
				bottom.value = Color.clear;
			}
			bool flag4 = left.keyword > StyleKeyword.Undefined;
			if (flag4)
			{
				left.value = Color.clear;
			}
		}
	}
}
