using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static class StyleSheetUtility
	{
		public static StyleSheet CreateInstanceWithHideFlags()
		{
			StyleSheet styleSheet = ScriptableObject.CreateInstance<StyleSheet>();
			styleSheet.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontUnloadUnusedAsset;
			return styleSheet;
		}

		public static Dimension ToDimension(this Length length)
		{
			bool flag = length.IsAuto() || length.IsNone();
			if (flag)
			{
				throw new InvalidCastException(string.Format("Can't convert a Length to a Dimension because it contains the '{0}' keyword.", length));
			}
			return new Dimension(length.value, length.unit.ToDimensionUnit());
		}

		public static Dimension.Unit ToDimensionUnit(this LengthUnit unit)
		{
			if (!true)
			{
			}
			Dimension.Unit unit2;
			if (unit != LengthUnit.Pixel)
			{
				if (unit != LengthUnit.Percent)
				{
					throw new InvalidCastException(string.Format("Can't convert a LengthUnit to a Dimension.Unit because it does not contain a valid keyword. Expected 'px' or '%', but was {0}", unit));
				}
				unit2 = Dimension.Unit.Percent;
			}
			else
			{
				unit2 = Dimension.Unit.Pixel;
			}
			if (!true)
			{
			}
			return unit2;
		}

		public static Dimension ToDimension(this Angle angle)
		{
			bool flag = angle.IsNone();
			if (flag)
			{
				throw new InvalidCastException(string.Format("Can't convert a Rotate to a Dimension because it contains the '{0}' keyword.", angle));
			}
			return new Dimension(angle.value, angle.unit.ToDimensionUnit());
		}

		public static Dimension.Unit ToDimensionUnit(this AngleUnit unit)
		{
			if (!true)
			{
			}
			Dimension.Unit unit2;
			switch (unit)
			{
			case AngleUnit.Degree:
				unit2 = Dimension.Unit.Degree;
				break;
			case AngleUnit.Gradian:
				unit2 = Dimension.Unit.Gradian;
				break;
			case AngleUnit.Radian:
				unit2 = Dimension.Unit.Radian;
				break;
			case AngleUnit.Turn:
				unit2 = Dimension.Unit.Turn;
				break;
			default:
				throw new InvalidCastException(string.Format("Can't convert a AngleUnit to a Dimension.Unit because it does not contain a valid keyword. Expected 'deg', 'grad', 'rad' or 'turn', but was {0}", unit));
			}
			if (!true)
			{
			}
			return unit2;
		}

		public static Dimension ToDimension(this TimeValue timeValue)
		{
			return new Dimension(timeValue.value, timeValue.unit.ToDimensionUnit());
		}

		public static Dimension.Unit ToDimensionUnit(this TimeUnit unit)
		{
			if (!true)
			{
			}
			Dimension.Unit unit2;
			if (unit != TimeUnit.Second)
			{
				if (unit != TimeUnit.Millisecond)
				{
					throw new InvalidCastException(string.Format("Can't convert a TimeUnit to a Dimension.Unit because it does not contain a valid keyword. Expected 's' or 'ms', but was {0}", unit));
				}
				unit2 = Dimension.Unit.Millisecond;
			}
			else
			{
				unit2 = Dimension.Unit.Second;
			}
			if (!true)
			{
			}
			return unit2;
		}

		public static StyleValueKeyword ToStyleValueKeyword(this StyleKeyword keyword)
		{
			if (!true)
			{
			}
			StyleValueKeyword styleValueKeyword;
			switch (keyword)
			{
			case StyleKeyword.Auto:
				styleValueKeyword = StyleValueKeyword.Auto;
				break;
			case StyleKeyword.None:
				styleValueKeyword = StyleValueKeyword.None;
				break;
			case StyleKeyword.Initial:
				styleValueKeyword = StyleValueKeyword.Initial;
				break;
			default:
				throw new InvalidCastException(string.Format("Can't convert a StyleKeyword to a StyleValueKeyword because it does not contain a valid keyword. Expected 'auto', 'none' or 'initial', but was {0}.", keyword));
			}
			if (!true)
			{
			}
			return styleValueKeyword;
		}

		public static void TransferStylePropertyHandles(StyleSheet fromStyleSheet, StyleProperty fromStyleProperty, StyleSheet toStyleSheet, StyleProperty toStyleProperty)
		{
			Assert.IsNotNull<StyleSheet>(fromStyleSheet);
			Assert.IsNotNull<StyleSheet>(toStyleSheet);
			Assert.IsNotNull<StyleProperty>(fromStyleProperty);
			Assert.IsNotNull<StyleProperty>(toStyleProperty);
			Assert.IsFalse(fromStyleProperty == toStyleProperty, "Cannot transfer a StyleProperty unto itself.");
			List<StyleValueHandle> list;
			using (CollectionPool<List<StyleValueHandle>, StyleValueHandle>.Get(out list))
			{
				list.AddRange(toStyleProperty.values);
				foreach (StyleValueHandle styleValueHandle in fromStyleProperty.values)
				{
					StyleValueType valueType = styleValueHandle.valueType;
					if (!true)
					{
					}
					int num;
					switch (valueType)
					{
					case StyleValueType.Invalid:
						num = styleValueHandle.valueIndex;
						break;
					case StyleValueType.Keyword:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadKeyword(styleValueHandle));
						break;
					case StyleValueType.Float:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadFloat(styleValueHandle));
						break;
					case StyleValueType.Dimension:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadDimension(styleValueHandle));
						break;
					case StyleValueType.Color:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadColor(styleValueHandle));
						break;
					case StyleValueType.ResourcePath:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadResourcePath(styleValueHandle));
						break;
					case StyleValueType.AssetReference:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadAssetReference(styleValueHandle));
						break;
					case StyleValueType.Enum:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadEnum(styleValueHandle));
						break;
					case StyleValueType.Variable:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadVariable(styleValueHandle));
						break;
					case StyleValueType.String:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadString(styleValueHandle));
						break;
					case StyleValueType.Function:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadFunction(styleValueHandle));
						break;
					case StyleValueType.CommaSeparator:
						num = styleValueHandle.valueIndex;
						break;
					case StyleValueType.ScalableImage:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadScalableImage(styleValueHandle));
						break;
					case StyleValueType.MissingAssetReference:
						num = toStyleSheet.AddValue(fromStyleSheet.ReadMissingAssetReferenceUrl(styleValueHandle));
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
					if (!true)
					{
					}
					int num2 = num;
					list.Add(new StyleValueHandle(num2, valueType));
				}
				toStyleProperty.requireVariableResolve |= fromStyleProperty.requireVariableResolve;
				toStyleProperty.values = list.ToArray();
			}
		}

		public static string GetEnumExportString(Enum value)
		{
			return StyleSheetUtility.ConvertCamelToDash(value.ToString());
		}

		public static string ConvertCamelToDash(string camel)
		{
			string text = Regex.Replace(Regex.Replace(camel, "(\\P{Ll})(\\P{Ll}\\p{Ll})", "$1-$2"), "(\\p{Ll})(\\P{Ll})", "$1-$2");
			string text2 = text.ToLowerInvariant();
			return StyleSheetUtility.SpecialEnumToStringCases.GetValueOrDefault(text2, text2);
		}

		public static string ConvertDashToHungarian(string dash)
		{
			return StyleSheetUtility.ConvertDashToUpperNoSpace(dash, true, false);
		}

		public static string ConvertDashToCamel(string dash)
		{
			return StyleSheetUtility.ConvertDashToUpperNoSpace(dash, false, false);
		}

		public static string ConvertDashToHuman(string dash)
		{
			return StyleSheetUtility.ConvertDashToUpperNoSpace(dash, true, true);
		}

		public static string ConvertDashToUpperNoSpace(string dash, bool firstCase, bool addSpace)
		{
			string text;
			bool flag = StyleSheetUtility.SpecialStringToEnumCases.TryGetValue(dash, out text);
			string text2;
			if (flag)
			{
				text2 = text;
			}
			else
			{
				StringBuilder stringBuilder = GenericPool<StringBuilder>.Get();
				try
				{
					bool flag2 = firstCase;
					foreach (char c in dash)
					{
						bool flag3 = c == '-';
						if (flag3)
						{
							if (addSpace)
							{
								stringBuilder.Append(' ');
							}
							flag2 = true;
						}
						else
						{
							bool flag4 = flag2;
							if (flag4)
							{
								stringBuilder.Append(char.ToUpper(c, CultureInfo.InvariantCulture));
								flag2 = false;
							}
							else
							{
								stringBuilder.Append(char.ToLowerInvariant(c));
							}
						}
					}
					text2 = stringBuilder.ToString();
				}
				finally
				{
					GenericPool<StringBuilder>.Release(stringBuilder.Clear());
				}
			}
			return text2;
		}

		public static string GetDimensionUnitExportString(Dimension.Unit unit)
		{
			if (!true)
			{
			}
			string text;
			switch (unit)
			{
			case Dimension.Unit.Unitless:
				text = string.Empty;
				break;
			case Dimension.Unit.Pixel:
				text = "px";
				break;
			case Dimension.Unit.Percent:
				text = "%";
				break;
			case Dimension.Unit.Second:
				text = "s";
				break;
			case Dimension.Unit.Millisecond:
				text = "ms";
				break;
			case Dimension.Unit.Degree:
				text = "deg";
				break;
			case Dimension.Unit.Gradian:
				text = "grad";
				break;
			case Dimension.Unit.Radian:
				text = "rad";
				break;
			case Dimension.Unit.Turn:
				text = "turn";
				break;
			default:
				throw new ArgumentOutOfRangeException("unit", unit, null);
			}
			if (!true)
			{
			}
			return text;
		}

		public static void GetValueOffsets(StyleSheet styleSheet, Span<StyleValueHandle> handles, List<int> offsets)
		{
			offsets.Clear();
			bool flag = handles.Length == 0;
			if (!flag)
			{
				offsets.Add(0);
				int num = 0;
				for (;;)
				{
					num = StyleSheetUtility.GetNextValueOffset(styleSheet, handles, num);
					bool flag2 = num >= 0 && num < handles.Length;
					if (!flag2)
					{
						break;
					}
					offsets.Add(num);
				}
			}
		}

		internal unsafe static int GetNextValueOffset(StyleSheet styleSheet, Span<StyleValueHandle> handles, int index)
		{
			bool flag = index < 0 || index >= handles.Length;
			if (!flag)
			{
				int num = index;
				StyleValueHandle styleValueHandle = *handles[index];
				switch (styleValueHandle.valueType)
				{
				case StyleValueType.Keyword:
				case StyleValueType.Float:
				case StyleValueType.Dimension:
				case StyleValueType.Color:
				case StyleValueType.ResourcePath:
				case StyleValueType.AssetReference:
				case StyleValueType.Enum:
				case StyleValueType.Variable:
				case StyleValueType.String:
				case StyleValueType.ScalableImage:
				case StyleValueType.MissingAssetReference:
					return num + 1 + StyleSheetUtility.<GetNextValueOffset>g__OffsetByComma|19_0(handles, num + 1);
				case StyleValueType.Function:
				{
					StyleValueHandle styleValueHandle2 = *handles[++index];
					float num2 = styleSheet.ReadFloat(styleValueHandle2);
					int num3;
					index = (num3 = index + 1);
					int num4 = num3;
					while ((float)num4 < (float)num3 + num2)
					{
						bool flag2 = handles[num4].valueType == StyleValueType.Function;
						if (flag2)
						{
							int nextValueOffset = StyleSheetUtility.GetNextValueOffset(styleSheet, handles, num4);
							bool flag3 = nextValueOffset > 0;
							if (!flag3)
							{
								return -1;
							}
							index = nextValueOffset;
						}
						else
						{
							index++;
						}
						num4++;
					}
					return index + StyleSheetUtility.<GetNextValueOffset>g__OffsetByComma|19_0(handles, index);
				}
				}
				throw new ArgumentOutOfRangeException();
			}
			return -1;
		}

		[CompilerGenerated]
		internal static int <GetNextValueOffset>g__OffsetByComma|19_0(Span<StyleValueHandle> handles, int next)
		{
			return (next < handles.Length && handles[next].valueType == StyleValueType.CommaSeparator) ? 1 : 0;
		}

		private static readonly Dictionary<string, string> SpecialEnumToStringCases = new Dictionary<string, string> { { "no-wrap", "nowrap" } };

		private static readonly Dictionary<string, string> SpecialStringToEnumCases = new Dictionary<string, string>
		{
			{ "nowrap", "NoWrap" },
			{ "sdf", "SDF" },
			{ "uv", "UV" }
		};
	}
}
