using System;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements.Experimental
{
	internal static class Lerp
	{
		public static float Interpolate(float start, float end, float ratio)
		{
			return Mathf.LerpUnclamped(start, end, ratio);
		}

		public static int Interpolate(int start, int end, float ratio)
		{
			return Mathf.RoundToInt(Mathf.LerpUnclamped((float)start, (float)end, ratio));
		}

		public static Rect Interpolate(Rect r1, Rect r2, float ratio)
		{
			return new Rect(Mathf.LerpUnclamped(r1.x, r2.x, ratio), Mathf.LerpUnclamped(r1.y, r2.y, ratio), Mathf.LerpUnclamped(r1.width, r2.width, ratio), Mathf.LerpUnclamped(r1.height, r2.height, ratio));
		}

		public static Color Interpolate(Color start, Color end, float ratio)
		{
			return Color.LerpUnclamped(start, end, ratio);
		}

		public static Vector2 Interpolate(Vector2 start, Vector2 end, float ratio)
		{
			return Vector2.LerpUnclamped(start, end, ratio);
		}

		public static Vector3 Interpolate(Vector3 start, Vector3 end, float ratio)
		{
			return Vector3.LerpUnclamped(start, end, ratio);
		}

		public static Quaternion Interpolate(Quaternion start, Quaternion end, float ratio)
		{
			return Quaternion.SlerpUnclamped(start, end, ratio);
		}

		internal static StyleValues Interpolate(StyleValues start, StyleValues end, float ratio)
		{
			StyleValues styleValues = default(StyleValues);
			foreach (StyleValue styleValue in end.m_StyleValues.m_Values)
			{
				StyleValue styleValue2 = default(StyleValue);
				bool flag = !start.m_StyleValues.TryGetStyleValue(styleValue.id, ref styleValue2);
				if (flag)
				{
					throw new ArgumentException("Start StyleValues must contain the same values as end values. Missing property:" + styleValue.id);
				}
				switch (styleValue.id)
				{
				case StylePropertyID.Unknown:
				case StylePropertyID.Position:
				case StylePropertyID.BorderLeftColor:
				case StylePropertyID.BorderTopColor:
				case StylePropertyID.BorderRightColor:
				case StylePropertyID.BorderBottomColor:
				case StylePropertyID.FlexDirection:
				case StylePropertyID.FlexWrap:
				case StylePropertyID.JustifyContent:
				case StylePropertyID.AlignContent:
				case StylePropertyID.AlignSelf:
				case StylePropertyID.AlignItems:
				case StylePropertyID.UnityTextAlign:
				case StylePropertyID.WhiteSpace:
				case StylePropertyID.Font:
				case StylePropertyID.FontStyleAndWeight:
				case StylePropertyID.BackgroundScaleMode:
				case StylePropertyID.Visibility:
				case StylePropertyID.Overflow:
				case StylePropertyID.OverflowClipBox:
				case StylePropertyID.Display:
				case StylePropertyID.BackgroundImage:
				case StylePropertyID.SliceLeft:
				case StylePropertyID.SliceTop:
				case StylePropertyID.SliceRight:
				case StylePropertyID.SliceBottom:
				case StylePropertyID.BorderRadius:
				case StylePropertyID.BorderWidth:
				case StylePropertyID.Flex:
				case StylePropertyID.Margin:
				case StylePropertyID.Padding:
				case StylePropertyID.Cursor:
				case StylePropertyID.Custom:
					goto IL_01D4;
				case StylePropertyID.MarginLeft:
				case StylePropertyID.MarginTop:
				case StylePropertyID.MarginRight:
				case StylePropertyID.MarginBottom:
				case StylePropertyID.PaddingLeft:
				case StylePropertyID.PaddingTop:
				case StylePropertyID.PaddingRight:
				case StylePropertyID.PaddingBottom:
				case StylePropertyID.PositionLeft:
				case StylePropertyID.PositionTop:
				case StylePropertyID.PositionRight:
				case StylePropertyID.PositionBottom:
				case StylePropertyID.Width:
				case StylePropertyID.Height:
				case StylePropertyID.MinWidth:
				case StylePropertyID.MinHeight:
				case StylePropertyID.MaxWidth:
				case StylePropertyID.MaxHeight:
				case StylePropertyID.FlexBasis:
				case StylePropertyID.FlexGrow:
				case StylePropertyID.FlexShrink:
				case StylePropertyID.BorderLeftWidth:
				case StylePropertyID.BorderTopWidth:
				case StylePropertyID.BorderRightWidth:
				case StylePropertyID.BorderBottomWidth:
				case StylePropertyID.BorderTopLeftRadius:
				case StylePropertyID.BorderTopRightRadius:
				case StylePropertyID.BorderBottomRightRadius:
				case StylePropertyID.BorderBottomLeftRadius:
				case StylePropertyID.FontSize:
				case StylePropertyID.Opacity:
					styleValues.SetValue(styleValue.id, Lerp.Interpolate(styleValue2.number, styleValue.number, ratio));
					break;
				case StylePropertyID.Color:
				case StylePropertyID.BackgroundColor:
				case StylePropertyID.BackgroundImageTintColor:
				case StylePropertyID.BorderColor:
					styleValues.SetValue(styleValue.id, Lerp.Interpolate(styleValue2.color, styleValue.color, ratio));
					break;
				default:
					goto IL_01D4;
				}
				continue;
				IL_01D4:
				throw new ArgumentException("Style Value can't be animated");
			}
			return styleValues;
		}
	}
}
