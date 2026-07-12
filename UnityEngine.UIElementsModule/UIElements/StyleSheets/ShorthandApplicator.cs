using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.StyleSheets
{
	internal static class ShorthandApplicator
	{
		public static void ApplyBackgroundPosition(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			BackgroundPosition backgroundPosition;
			BackgroundPosition backgroundPosition2;
			ShorthandApplicator.CompileBackgroundPosition(reader, out backgroundPosition, out backgroundPosition2);
			computedStyle.visualData.Write().backgroundPositionX = backgroundPosition;
			computedStyle.visualData.Write().backgroundPositionY = backgroundPosition2;
		}

		public static void ApplyBorderColor(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			Color color;
			Color color2;
			Color color3;
			Color color4;
			ShorthandApplicator.CompileBoxArea(reader, out color, out color2, out color3, out color4);
			computedStyle.visualData.Write().borderTopColor = color;
			computedStyle.visualData.Write().borderRightColor = color2;
			computedStyle.visualData.Write().borderBottomColor = color3;
			computedStyle.visualData.Write().borderLeftColor = color4;
		}

		public static void ApplyBorderRadius(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			Length length;
			Length length2;
			Length length3;
			Length length4;
			ShorthandApplicator.CompileBorderRadius(reader, out length, out length2, out length3, out length4);
			computedStyle.visualData.Write().borderTopLeftRadius = length;
			computedStyle.visualData.Write().borderTopRightRadius = length2;
			computedStyle.visualData.Write().borderBottomRightRadius = length3;
			computedStyle.visualData.Write().borderBottomLeftRadius = length4;
		}

		public static void ApplyBorderWidth(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			float num;
			float num2;
			float num3;
			float num4;
			ShorthandApplicator.CompileBoxArea(reader, out num, out num2, out num3, out num4);
			computedStyle.layoutData.Write().borderTopWidth = num;
			computedStyle.layoutData.Write().borderRightWidth = num2;
			computedStyle.layoutData.Write().borderBottomWidth = num3;
			computedStyle.layoutData.Write().borderLeftWidth = num4;
		}

		public static void ApplyFlex(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			float num;
			float num2;
			Length length;
			ShorthandApplicator.CompileFlexShorthand(reader, out num, out num2, out length);
			computedStyle.layoutData.Write().flexGrow = num;
			computedStyle.layoutData.Write().flexShrink = num2;
			computedStyle.layoutData.Write().flexBasis = length;
		}

		public static void ApplyMargin(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			Length length;
			Length length2;
			Length length3;
			Length length4;
			ShorthandApplicator.CompileBoxArea(reader, out length, out length2, out length3, out length4);
			computedStyle.layoutData.Write().marginTop = length;
			computedStyle.layoutData.Write().marginRight = length2;
			computedStyle.layoutData.Write().marginBottom = length3;
			computedStyle.layoutData.Write().marginLeft = length4;
		}

		public static void ApplyPadding(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			Length length;
			Length length2;
			Length length3;
			Length length4;
			ShorthandApplicator.CompileBoxArea(reader, out length, out length2, out length3, out length4);
			computedStyle.layoutData.Write().paddingTop = length;
			computedStyle.layoutData.Write().paddingRight = length2;
			computedStyle.layoutData.Write().paddingBottom = length3;
			computedStyle.layoutData.Write().paddingLeft = length4;
		}

		public static void ApplyTransition(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			List<TimeValue> list;
			List<TimeValue> list2;
			List<StylePropertyName> list3;
			List<EasingFunction> list4;
			ShorthandApplicator.CompileTransition(reader, out list, out list2, out list3, out list4);
			computedStyle.transitionData.Write().transitionDelay.CopyFrom<TimeValue>(list);
			computedStyle.transitionData.Write().transitionDuration.CopyFrom<TimeValue>(list2);
			computedStyle.transitionData.Write().transitionProperty.CopyFrom<StylePropertyName>(list3);
			computedStyle.transitionData.Write().transitionTimingFunction.CopyFrom<EasingFunction>(list4);
		}

		public static void ApplyUnityBackgroundScaleMode(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			BackgroundPosition backgroundPosition;
			BackgroundPosition backgroundPosition2;
			BackgroundRepeat backgroundRepeat;
			BackgroundSize backgroundSize;
			ShorthandApplicator.CompileUnityBackgroundScaleMode(reader, out backgroundPosition, out backgroundPosition2, out backgroundRepeat, out backgroundSize);
			computedStyle.visualData.Write().backgroundPositionX = backgroundPosition;
			computedStyle.visualData.Write().backgroundPositionY = backgroundPosition2;
			computedStyle.visualData.Write().backgroundRepeat = backgroundRepeat;
			computedStyle.visualData.Write().backgroundSize = backgroundSize;
		}

		public static void ApplyUnityTextOutline(StylePropertyReader reader, ref ComputedStyle computedStyle)
		{
			Color color;
			float num;
			ShorthandApplicator.CompileTextOutline(reader, out color, out num);
			computedStyle.inheritedData.Write().unityTextOutlineColor = color;
			computedStyle.inheritedData.Write().unityTextOutlineWidth = num;
		}

		private static bool CompileFlexShorthand(StylePropertyReader reader, out float grow, out float shrink, out Length basis)
		{
			grow = 0f;
			shrink = 1f;
			basis = Length.Auto();
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
					basis = Length.Auto();
				}
				else
				{
					bool flag4 = reader.IsKeyword(0, StyleValueKeyword.Auto);
					if (flag4)
					{
						flag = true;
						grow = 1f;
						shrink = 1f;
						basis = Length.Auto();
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
									basis = Length.Auto();
								}
							}
							else
							{
								bool flag12 = valueType == StyleValueType.Dimension;
								if (flag12)
								{
									basis = reader.ReadLength(num);
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
								float num2 = reader.ReadFloat(num);
								bool flag15 = !flag6;
								if (flag15)
								{
									flag6 = true;
									grow = num2;
								}
								else
								{
									shrink = num2;
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

		private static void CompileBorderRadius(StylePropertyReader reader, out Length top, out Length right, out Length bottom, out Length left)
		{
			ShorthandApplicator.CompileBoxArea(reader, out top, out right, out bottom, out left);
			bool flag = top.IsAuto() || top.IsNone();
			if (flag)
			{
				top = 0f;
			}
			bool flag2 = right.IsAuto() || right.IsNone();
			if (flag2)
			{
				right = 0f;
			}
			bool flag3 = bottom.IsAuto() || bottom.IsNone();
			if (flag3)
			{
				bottom = 0f;
			}
			bool flag4 = left.IsAuto() || left.IsNone();
			if (flag4)
			{
				left = 0f;
			}
		}

		private static void CompileBackgroundPosition(StylePropertyReader reader, out BackgroundPosition backgroundPositionX, out BackgroundPosition backgroundPositionY)
		{
			int valueCount = reader.valueCount;
			StylePropertyValue value = reader.GetValue(0);
			StylePropertyValue stylePropertyValue = ((valueCount > 1) ? reader.GetValue(1) : default(StylePropertyValue));
			StylePropertyValue stylePropertyValue2 = ((valueCount > 2) ? reader.GetValue(2) : default(StylePropertyValue));
			StylePropertyValue stylePropertyValue3 = ((valueCount > 3) ? reader.GetValue(3) : default(StylePropertyValue));
			backgroundPositionX = default(BackgroundPosition);
			backgroundPositionY = default(BackgroundPosition);
			bool flag = valueCount == 1;
			if (flag)
			{
				BackgroundPositionKeyword backgroundPositionKeyword = (BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 0);
				bool flag2 = backgroundPositionKeyword == BackgroundPositionKeyword.Left;
				if (flag2)
				{
					backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Left);
					backgroundPositionY = BackgroundPosition.Initial();
				}
				else
				{
					bool flag3 = backgroundPositionKeyword == BackgroundPositionKeyword.Right;
					if (flag3)
					{
						backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Right);
						backgroundPositionY = BackgroundPosition.Initial();
					}
					else
					{
						bool flag4 = backgroundPositionKeyword == BackgroundPositionKeyword.Top;
						if (flag4)
						{
							backgroundPositionX = BackgroundPosition.Initial();
							backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Top);
						}
						else
						{
							bool flag5 = backgroundPositionKeyword == BackgroundPositionKeyword.Bottom;
							if (flag5)
							{
								backgroundPositionX = BackgroundPosition.Initial();
								backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Bottom);
							}
							else
							{
								bool flag6 = backgroundPositionKeyword == BackgroundPositionKeyword.Center;
								if (flag6)
								{
									backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
									backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag7 = valueCount == 2;
				if (flag7)
				{
					bool flag8 = (value.handle.valueType == StyleValueType.Dimension || value.handle.valueType == StyleValueType.Float) && (value.handle.valueType == StyleValueType.Dimension || value.handle.valueType == StyleValueType.Float);
					if (flag8)
					{
						backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Left, value.sheet.ReadDimension(value.handle).ToLength());
						backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Top, stylePropertyValue.sheet.ReadDimension(stylePropertyValue.handle).ToLength());
					}
					else
					{
						bool flag9 = value.handle.valueType == StyleValueType.Enum && stylePropertyValue.handle.valueType == StyleValueType.Enum;
						if (flag9)
						{
							BackgroundPositionKeyword backgroundPositionKeyword2 = (BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 0);
							BackgroundPositionKeyword backgroundPositionKeyword3 = (BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 1);
							bool flag10 = backgroundPositionKeyword3 == BackgroundPositionKeyword.Left;
							if (flag10)
							{
								ShorthandApplicator.<CompileBackgroundPosition>g__SwapKeyword|16_0(ref backgroundPositionKeyword2, ref backgroundPositionKeyword3);
							}
							bool flag11 = backgroundPositionKeyword3 == BackgroundPositionKeyword.Right;
							if (flag11)
							{
								ShorthandApplicator.<CompileBackgroundPosition>g__SwapKeyword|16_0(ref backgroundPositionKeyword2, ref backgroundPositionKeyword3);
							}
							bool flag12 = backgroundPositionKeyword2 == BackgroundPositionKeyword.Top;
							if (flag12)
							{
								ShorthandApplicator.<CompileBackgroundPosition>g__SwapKeyword|16_0(ref backgroundPositionKeyword2, ref backgroundPositionKeyword3);
							}
							bool flag13 = backgroundPositionKeyword2 == BackgroundPositionKeyword.Bottom;
							if (flag13)
							{
								ShorthandApplicator.<CompileBackgroundPosition>g__SwapKeyword|16_0(ref backgroundPositionKeyword2, ref backgroundPositionKeyword3);
							}
							backgroundPositionX = new BackgroundPosition(backgroundPositionKeyword2);
							backgroundPositionY = new BackgroundPosition(backgroundPositionKeyword3);
						}
					}
				}
				else
				{
					bool flag14 = valueCount == 3;
					if (flag14)
					{
						bool flag15 = value.handle.valueType == StyleValueType.Enum && stylePropertyValue.handle.valueType == StyleValueType.Enum && stylePropertyValue2.handle.valueType == StyleValueType.Dimension;
						if (flag15)
						{
							backgroundPositionX = new BackgroundPosition((BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 0));
							backgroundPositionY = new BackgroundPosition((BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 1), reader.ReadLength(2));
						}
						else
						{
							bool flag16 = value.handle.valueType == StyleValueType.Enum && stylePropertyValue.handle.valueType == StyleValueType.Dimension && stylePropertyValue2.handle.valueType == StyleValueType.Enum;
							if (flag16)
							{
								backgroundPositionX = new BackgroundPosition((BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 0), reader.ReadLength(1));
								backgroundPositionY = new BackgroundPosition((BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 2));
							}
						}
					}
					else
					{
						bool flag17 = valueCount == 4;
						if (flag17)
						{
							bool flag18 = value.handle.valueType == StyleValueType.Enum && stylePropertyValue.handle.valueType == StyleValueType.Dimension && stylePropertyValue2.handle.valueType == StyleValueType.Enum && stylePropertyValue3.handle.valueType == StyleValueType.Dimension;
							if (flag18)
							{
								backgroundPositionX = new BackgroundPosition((BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 0), reader.ReadLength(1));
								backgroundPositionY = new BackgroundPosition((BackgroundPositionKeyword)reader.ReadEnum(StyleEnumType.BackgroundPositionKeyword, 2), reader.ReadLength(3));
							}
						}
					}
				}
			}
		}

		public static void CompileUnityBackgroundScaleMode(StylePropertyReader reader, out BackgroundPosition backgroundPositionX, out BackgroundPosition backgroundPositionY, out BackgroundRepeat backgroundRepeat, out BackgroundSize backgroundSize)
		{
			ScaleMode scaleMode = (ScaleMode)reader.ReadEnum(StyleEnumType.ScaleMode, 0);
			backgroundPositionX = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(scaleMode);
			backgroundPositionY = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(scaleMode);
			backgroundRepeat = BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat(scaleMode);
			backgroundSize = BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize(scaleMode);
		}

		private static void CompileBoxArea(StylePropertyReader reader, out Length top, out Length right, out Length bottom, out Length left)
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
				top = (right = (bottom = (left = reader.ReadLength(0))));
				break;
			case 2:
				top = (bottom = reader.ReadLength(0));
				left = (right = reader.ReadLength(1));
				break;
			case 3:
				top = reader.ReadLength(0);
				left = (right = reader.ReadLength(1));
				bottom = reader.ReadLength(2);
				break;
			default:
				top = reader.ReadLength(0);
				right = reader.ReadLength(1);
				bottom = reader.ReadLength(2);
				left = reader.ReadLength(3);
				break;
			}
		}

		private static void CompileBoxArea(StylePropertyReader reader, out float top, out float right, out float bottom, out float left)
		{
			Length length;
			Length length2;
			Length length3;
			Length length4;
			ShorthandApplicator.CompileBoxArea(reader, out length, out length2, out length3, out length4);
			top = length.value;
			right = length2.value;
			bottom = length3.value;
			left = length4.value;
		}

		private static void CompileBoxArea(StylePropertyReader reader, out Color top, out Color right, out Color bottom, out Color left)
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
				top = (right = (bottom = (left = reader.ReadColor(0))));
				break;
			case 2:
				top = (bottom = reader.ReadColor(0));
				left = (right = reader.ReadColor(1));
				break;
			case 3:
				top = reader.ReadColor(0);
				left = (right = reader.ReadColor(1));
				bottom = reader.ReadColor(2);
				break;
			default:
				top = reader.ReadColor(0);
				right = reader.ReadColor(1);
				bottom = reader.ReadColor(2);
				left = reader.ReadColor(3);
				break;
			}
		}

		private static void CompileTextOutline(StylePropertyReader reader, out Color outlineColor, out float outlineWidth)
		{
			outlineColor = Color.clear;
			outlineWidth = 0f;
			int valueCount = reader.valueCount;
			for (int i = 0; i < valueCount; i++)
			{
				StyleValueType valueType = reader.GetValueType(i);
				bool flag = valueType == StyleValueType.Dimension;
				if (flag)
				{
					outlineWidth = reader.ReadFloat(i);
				}
				else
				{
					bool flag2 = valueType == StyleValueType.Enum || valueType == StyleValueType.Color;
					if (flag2)
					{
						outlineColor = reader.ReadColor(i);
					}
				}
			}
		}

		private static void CompileTransition(StylePropertyReader reader, out List<TimeValue> outDelay, out List<TimeValue> outDuration, out List<StylePropertyName> outProperty, out List<EasingFunction> outTimingFunction)
		{
			ShorthandApplicator.s_TransitionDelayList.Clear();
			ShorthandApplicator.s_TransitionDurationList.Clear();
			ShorthandApplicator.s_TransitionPropertyList.Clear();
			ShorthandApplicator.s_TransitionTimingFunctionList.Clear();
			bool flag = true;
			bool flag2 = false;
			int valueCount = reader.valueCount;
			int num = 0;
			int num2 = 0;
			for (;;)
			{
				bool flag3 = flag2;
				if (flag3)
				{
					break;
				}
				StylePropertyName stylePropertyName = InitialStyle.transitionProperty[0];
				TimeValue timeValue = InitialStyle.transitionDuration[0];
				TimeValue timeValue2 = InitialStyle.transitionDelay[0];
				EasingFunction easingFunction = InitialStyle.transitionTimingFunction[0];
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				bool flag7 = false;
				bool flag8 = false;
				while (num2 < valueCount && !flag8)
				{
					StyleValueType valueType = reader.GetValueType(num2);
					StyleValueType styleValueType = valueType;
					StyleValueType styleValueType2 = styleValueType;
					if (styleValueType2 <= StyleValueType.Dimension)
					{
						if (styleValueType2 != StyleValueType.Keyword)
						{
							if (styleValueType2 != StyleValueType.Dimension)
							{
								goto IL_01A3;
							}
							TimeValue timeValue3 = reader.ReadTimeValue(num2);
							bool flag9 = !flag4;
							if (flag9)
							{
								flag4 = true;
								timeValue = timeValue3;
							}
							else
							{
								bool flag10 = !flag5;
								if (flag10)
								{
									flag5 = true;
									timeValue2 = timeValue3;
								}
								else
								{
									flag = false;
								}
							}
						}
						else
						{
							bool flag11 = reader.IsKeyword(num2, StyleValueKeyword.None) && num == 0;
							if (flag11)
							{
								flag2 = true;
								flag6 = true;
								stylePropertyName = new StylePropertyName("none");
							}
							else
							{
								flag = false;
							}
						}
					}
					else if (styleValueType2 != StyleValueType.Enum)
					{
						if (styleValueType2 != StyleValueType.CommaSeparator)
						{
							goto IL_01A3;
						}
						flag8 = true;
						num++;
					}
					else
					{
						string text = reader.ReadAsString(num2);
						int num3;
						bool flag12 = !flag7 && StylePropertyUtil.TryGetEnumIntValue(StyleEnumType.EasingMode, text, out num3);
						if (flag12)
						{
							flag7 = true;
							easingFunction = (EasingMode)num3;
						}
						else
						{
							bool flag13 = !flag6;
							if (flag13)
							{
								flag6 = true;
								stylePropertyName = new StylePropertyName(text);
							}
							else
							{
								flag = false;
							}
						}
					}
					IL_01A7:
					num2++;
					continue;
					IL_01A3:
					flag = false;
					goto IL_01A7;
				}
				ShorthandApplicator.s_TransitionDelayList.Add(timeValue2);
				ShorthandApplicator.s_TransitionDurationList.Add(timeValue);
				ShorthandApplicator.s_TransitionPropertyList.Add(stylePropertyName);
				ShorthandApplicator.s_TransitionTimingFunctionList.Add(easingFunction);
				if (num2 >= valueCount || !flag)
				{
					goto IL_0209;
				}
			}
			flag = false;
			IL_0209:
			bool flag14 = flag;
			if (flag14)
			{
				outProperty = ShorthandApplicator.s_TransitionPropertyList;
				outDelay = ShorthandApplicator.s_TransitionDelayList;
				outDuration = ShorthandApplicator.s_TransitionDurationList;
				outTimingFunction = ShorthandApplicator.s_TransitionTimingFunctionList;
			}
			else
			{
				outProperty = InitialStyle.transitionProperty;
				outDelay = InitialStyle.transitionDelay;
				outDuration = InitialStyle.transitionDuration;
				outTimingFunction = InitialStyle.transitionTimingFunction;
			}
		}

		[CompilerGenerated]
		internal static void <CompileBackgroundPosition>g__SwapKeyword|16_0(ref BackgroundPositionKeyword a, ref BackgroundPositionKeyword b)
		{
			BackgroundPositionKeyword backgroundPositionKeyword = a;
			a = b;
			b = backgroundPositionKeyword;
		}

		private static List<TimeValue> s_TransitionDelayList = new List<TimeValue>();

		private static List<TimeValue> s_TransitionDurationList = new List<TimeValue>();

		private static List<StylePropertyName> s_TransitionPropertyList = new List<StylePropertyName>();

		private static List<EasingFunction> s_TransitionTimingFunctionList = new List<EasingFunction>();
	}
}
