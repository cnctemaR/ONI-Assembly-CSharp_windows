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

		public static void ApplyDefault<T>(int specificity, ref StyleValue<T> property)
		{
			StyleSheetApplicator.Apply<T>(default(T), specificity, ref property);
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

		public static void ApplyCursor(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<CursorStyle> property)
		{
			StyleValueHandle styleValueHandle = handles[0];
			bool flag = styleValueHandle.valueType == StyleValueType.ResourcePath;
			if (flag)
			{
				string text = sheet.ReadResourcePath(handles[0]);
				Texture2D texture2D = Panel.loadResourceFunc(text, typeof(Texture2D)) as Texture2D;
				if (texture2D != null)
				{
					Vector2 zero = Vector2.zero;
					sheet.TryReadFloat(handles, 1, out zero.x);
					sheet.TryReadFloat(handles, 2, out zero.y);
					CursorStyle cursorStyle = new CursorStyle
					{
						texture = texture2D,
						hotspot = zero
					};
					StyleSheetApplicator.Apply<CursorStyle>(cursorStyle, specificity, ref property);
				}
			}
			else if (StyleSheetApplicator.createDefaultCursorStyleFunc != null)
			{
				CursorStyle cursorStyle2 = StyleSheetApplicator.createDefaultCursorStyleFunc(sheet, styleValueHandle);
				StyleSheetApplicator.Apply<CursorStyle>(cursorStyle2, specificity, ref property);
			}
		}

		public static void ApplyResource<T>(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<T> property) where T : Object
		{
			StyleValueHandle styleValueHandle = handles[0];
			if (styleValueHandle.valueType == StyleValueType.Keyword && styleValueHandle.valueIndex == 5)
			{
				StyleSheetApplicator.Apply<T>((T)((object)null), specificity, ref property);
			}
			else
			{
				T t = (T)((object)null);
				string text = sheet.ReadResourcePath(styleValueHandle);
				if (!string.IsNullOrEmpty(text))
				{
					t = Panel.loadResourceFunc(text, typeof(T)) as T;
					if (t != null)
					{
						StyleSheetApplicator.Apply<T>(t, specificity, ref property);
					}
					else
					{
						Debug.LogWarning(string.Format("{0} resource/file not found for path: {1}", typeof(T).Name, text));
					}
				}
			}
		}

		internal static StyleSheetApplicator.CreateDefaultCursorStyleFunction createDefaultCursorStyleFunc = null;

		internal delegate CursorStyle CreateDefaultCursorStyleFunction(StyleSheet sheet, StyleValueHandle handle);

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
