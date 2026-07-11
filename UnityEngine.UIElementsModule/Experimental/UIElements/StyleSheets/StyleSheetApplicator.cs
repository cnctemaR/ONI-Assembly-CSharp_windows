using System;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal static class StyleSheetApplicator
	{
		private static void Apply<T>(T val, int specificity, ref StyleValue<T> property)
		{
			property.Apply(new StyleValue<T>(val, specificity), StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity);
		}

		public static void ApplyValue<T>(int specificity, ref StyleValue<T> property, T value = default(T))
		{
			StyleSheetApplicator.Apply<T>(value, specificity, ref property);
		}

		public static void ApplyBool(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<bool> property)
		{
			bool flag = sheet.ReadKeyword(handles[0]) == StyleValueKeyword.True;
			StyleSheetApplicator.Apply<bool>(flag, specificity, ref property);
		}

		public static void ApplyFloat(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<float> property)
		{
			float num = sheet.ReadFloat(handles[0]);
			StyleSheetApplicator.Apply<float>(num, specificity, ref property);
		}

		public static void ApplyFloatOrKeyword(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<FloatOrKeyword> property)
		{
			StyleValueHandle styleValueHandle = handles[0];
			FloatOrKeyword floatOrKeyword;
			if (styleValueHandle.valueType == StyleValueType.Keyword)
			{
				floatOrKeyword = new FloatOrKeyword((StyleValueKeyword)styleValueHandle.valueIndex);
			}
			else
			{
				floatOrKeyword = new FloatOrKeyword(sheet.ReadFloat(styleValueHandle));
			}
			StyleSheetApplicator.Apply<FloatOrKeyword>(floatOrKeyword, specificity, ref property);
		}

		public static void ApplyInt(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<int> property)
		{
			int num = (int)sheet.ReadFloat(handles[0]);
			StyleSheetApplicator.Apply<int>(num, specificity, ref property);
		}

		public static void ApplyEnum<T>(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<int> property)
		{
			int enumValue = StyleSheetCache.GetEnumValue<T>(sheet, handles[0]);
			StyleSheetApplicator.Apply<int>(enumValue, specificity, ref property);
		}

		public static void ApplyColor(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<Color> property)
		{
			Color color = sheet.ReadColor(handles[0]);
			StyleSheetApplicator.Apply<Color>(color, specificity, ref property);
		}

		public static void CompileCursor(StyleSheet sheet, StyleValueHandle[] handles, out float hotspotX, out float hotspotY, out int cursorId, out Texture2D texture)
		{
			StyleValueHandle styleValueHandle = handles[0];
			int num = 0;
			bool flag = styleValueHandle.valueType == StyleValueType.ResourcePath || styleValueHandle.valueType == StyleValueType.AssetReference;
			cursorId = 0;
			texture = null;
			hotspotX = 0f;
			hotspotY = 0f;
			if (flag)
			{
				if (StyleSheetApplicator.TryGetSourceFromHandle(sheet, handles[num++], out texture))
				{
					if (num < handles.Length && handles[num].valueType == StyleValueType.Float && sheet.TryReadFloat(handles, num++, out hotspotX))
					{
						if (!sheet.TryReadFloat(handles, num++, out hotspotY))
						{
						}
					}
				}
				if (num < handles.Length)
				{
					if (StyleSheetApplicator.getCursorIdFunc != null)
					{
						cursorId = StyleSheetApplicator.getCursorIdFunc(sheet, handles[num]);
					}
				}
			}
			else if (StyleSheetApplicator.getCursorIdFunc != null)
			{
				cursorId = StyleSheetApplicator.getCursorIdFunc(sheet, styleValueHandle);
			}
		}

		public static void ApplyCursor(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<CursorStyle> property)
		{
			float num;
			float num2;
			int num3;
			Texture2D texture2D;
			StyleSheetApplicator.CompileCursor(sheet, handles, out num, out num2, out num3, out texture2D);
			CursorStyle cursorStyle = new CursorStyle
			{
				texture = texture2D,
				hotspot = new Vector2(num, num2),
				defaultCursorId = num3
			};
			StyleSheetApplicator.Apply<CursorStyle>(cursorStyle, specificity, ref property);
		}

		public static void ApplyFont(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<Font> property)
		{
			StyleValueHandle styleValueHandle = handles[0];
			Font font = null;
			StyleValueType valueType = styleValueHandle.valueType;
			if (valueType != StyleValueType.ResourcePath)
			{
				if (valueType != StyleValueType.AssetReference)
				{
					Debug.LogWarning("Invalid value for font " + styleValueHandle.valueType);
				}
				else
				{
					font = sheet.ReadAssetReference(styleValueHandle) as Font;
					if (font == null)
					{
						Debug.LogWarning("Invalid font reference");
					}
				}
			}
			else
			{
				string text = sheet.ReadResourcePath(styleValueHandle);
				if (!string.IsNullOrEmpty(text))
				{
					font = Panel.loadResourceFunc(text, typeof(Font)) as Font;
				}
				if (font == null)
				{
					Debug.LogWarning(string.Format("Font not found for path: {0}", text));
				}
			}
			if (font != null)
			{
				StyleSheetApplicator.Apply<Font>(font, specificity, ref property);
			}
		}

		public static void ApplyImage(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<Texture2D> property)
		{
			Texture2D texture2D = null;
			StyleValueHandle styleValueHandle = handles[0];
			if (styleValueHandle.valueType == StyleValueType.Keyword)
			{
				if (styleValueHandle.valueIndex != 5)
				{
					Debug.LogWarning("Invalid keyword for image source " + (StyleValueKeyword)styleValueHandle.valueIndex);
				}
			}
			else if (!StyleSheetApplicator.TryGetSourceFromHandle(sheet, styleValueHandle, out texture2D))
			{
				return;
			}
			StyleSheetApplicator.Apply<Texture2D>(texture2D, specificity, ref property);
		}

		private static bool TryGetSourceFromHandle(StyleSheet sheet, StyleValueHandle handle, out Texture2D source)
		{
			source = null;
			StyleValueType valueType = handle.valueType;
			if (valueType != StyleValueType.ResourcePath)
			{
				if (valueType != StyleValueType.AssetReference)
				{
					Debug.LogWarning("Invalid value for image source " + handle.valueType);
					return false;
				}
				source = sheet.ReadAssetReference(handle) as Texture2D;
				if (source == null)
				{
					Debug.LogWarning("Invalid texture specified");
					return false;
				}
			}
			else
			{
				string text = sheet.ReadResourcePath(handle);
				if (!string.IsNullOrEmpty(text))
				{
					source = Panel.loadResourceFunc(text, typeof(Texture2D)) as Texture2D;
				}
				if (source == null)
				{
					Debug.LogWarning(string.Format("Texture not found for path: {0}", text));
					return false;
				}
			}
			return true;
		}

		public static bool CompileFlexShorthand(StyleSheet sheet, StyleValueHandle[] handles, out float grow, out float shrink, out FloatOrKeyword basis)
		{
			grow = 0f;
			shrink = 0f;
			basis = new FloatOrKeyword(StyleValueKeyword.Auto);
			bool flag = false;
			if (handles.Length == 1 && handles[0].valueType == StyleValueType.Keyword && handles[0].valueIndex == 2)
			{
				flag = true;
				grow = 0f;
				shrink = 1f;
				basis = new FloatOrKeyword(StyleValueKeyword.Auto);
			}
			else if (handles.Length == 1 && handles[0].valueType == StyleValueType.Keyword && handles[0].valueIndex == 5)
			{
				flag = true;
				grow = 0f;
				shrink = 0f;
				basis = new FloatOrKeyword(StyleValueKeyword.Auto);
			}
			else if (handles.Length <= 3 && handles[0].valueType == StyleValueType.Keyword && handles[0].valueIndex == 1)
			{
				flag = true;
				basis = new FloatOrKeyword(StyleValueKeyword.Auto);
				grow = 1f;
				shrink = 1f;
				if (handles.Length > 1)
				{
					grow = sheet.ReadFloat(handles[1]);
					if (handles.Length > 2)
					{
						shrink = sheet.ReadFloat(handles[2]);
					}
				}
			}
			else if (handles.Length <= 3 && handles[0].valueType == StyleValueType.Float)
			{
				flag = true;
				grow = sheet.ReadFloat(handles[0]);
				shrink = 1f;
				basis = new FloatOrKeyword(0f);
				if (handles.Length > 1)
				{
					if (handles[1].valueType == StyleValueType.Float)
					{
						shrink = sheet.ReadFloat(handles[1]);
						if (handles.Length > 2)
						{
							if (handles[2].valueType == StyleValueType.Keyword && handles[2].valueIndex == 1)
							{
								basis = new FloatOrKeyword(StyleValueKeyword.Auto);
							}
							else if (handles[2].valueType == StyleValueType.Float)
							{
								basis = new FloatOrKeyword(sheet.ReadFloat(handles[2]));
							}
						}
					}
					else if (handles[1].valueType == StyleValueType.Keyword && handles[1].valueIndex == 1)
					{
						basis = new FloatOrKeyword(StyleValueKeyword.Auto);
					}
				}
			}
			return flag;
		}

		public static void ApplyFlexShorthand(StyleSheet sheet, StyleValueHandle[] handles, int specificity, VisualElementStylesData styleData)
		{
			float num;
			float num2;
			FloatOrKeyword floatOrKeyword;
			bool flag = StyleSheetApplicator.CompileFlexShorthand(sheet, handles, out num, out num2, out floatOrKeyword);
			if (flag)
			{
				StyleSheetApplicator.ApplyValue<float>(specificity, ref styleData.flexGrow, num);
				StyleSheetApplicator.ApplyValue<float>(specificity, ref styleData.flexShrink, num2);
				StyleSheetApplicator.ApplyValue<FloatOrKeyword>(specificity, ref styleData.flexBasis, floatOrKeyword);
			}
		}

		internal static StyleSheetApplicator.GetCursorIdFunction getCursorIdFunc = null;

		internal delegate int GetCursorIdFunction(StyleSheet sheet, StyleValueHandle handle);

		public static class Shorthand
		{
			private static void ReadFourSidesArea(StyleSheet sheet, StyleValueHandle[] handles, out float top, out float right, out float bottom, out float left)
			{
				top = 0f;
				right = 0f;
				bottom = 0f;
				left = 0f;
				switch (handles.Length)
				{
				case 0:
					break;
				case 1:
					top = (right = (bottom = (left = sheet.ReadFloat(handles[0]))));
					break;
				case 2:
					top = (bottom = sheet.ReadFloat(handles[0]));
					left = (right = sheet.ReadFloat(handles[1]));
					break;
				case 3:
					top = sheet.ReadFloat(handles[0]);
					left = (right = sheet.ReadFloat(handles[1]));
					bottom = sheet.ReadFloat(handles[2]);
					break;
				default:
					top = sheet.ReadFloat(handles[0]);
					right = sheet.ReadFloat(handles[1]);
					bottom = sheet.ReadFloat(handles[2]);
					left = sheet.ReadFloat(handles[3]);
					break;
				}
			}

			public static void ApplyBorderRadius(StyleSheet sheet, StyleValueHandle[] handles, int specificity, VisualElementStylesData styleData)
			{
				float num;
				float num2;
				float num3;
				float num4;
				StyleSheetApplicator.Shorthand.ReadFourSidesArea(sheet, handles, out num, out num2, out num3, out num4);
				StyleSheetApplicator.Apply<float>(num, specificity, ref styleData.borderTopLeftRadius);
				StyleSheetApplicator.Apply<float>(num2, specificity, ref styleData.borderTopRightRadius);
				StyleSheetApplicator.Apply<float>(num4, specificity, ref styleData.borderBottomLeftRadius);
				StyleSheetApplicator.Apply<float>(num3, specificity, ref styleData.borderBottomRightRadius);
			}

			public static void ApplyMargin(StyleSheet sheet, StyleValueHandle[] handles, int specificity, VisualElementStylesData styleData)
			{
				float num;
				float num2;
				float num3;
				float num4;
				StyleSheetApplicator.Shorthand.ReadFourSidesArea(sheet, handles, out num, out num2, out num3, out num4);
				StyleSheetApplicator.Apply<float>(num, specificity, ref styleData.marginTop);
				StyleSheetApplicator.Apply<float>(num2, specificity, ref styleData.marginRight);
				StyleSheetApplicator.Apply<float>(num3, specificity, ref styleData.marginBottom);
				StyleSheetApplicator.Apply<float>(num4, specificity, ref styleData.marginLeft);
			}

			public static void ApplyPadding(StyleSheet sheet, StyleValueHandle[] handles, int specificity, VisualElementStylesData styleData)
			{
				float num;
				float num2;
				float num3;
				float num4;
				StyleSheetApplicator.Shorthand.ReadFourSidesArea(sheet, handles, out num, out num2, out num3, out num4);
				StyleSheetApplicator.Apply<float>(num, specificity, ref styleData.paddingTop);
				StyleSheetApplicator.Apply<float>(num2, specificity, ref styleData.paddingRight);
				StyleSheetApplicator.Apply<float>(num3, specificity, ref styleData.paddingBottom);
				StyleSheetApplicator.Apply<float>(num4, specificity, ref styleData.paddingLeft);
			}
		}
	}
}
