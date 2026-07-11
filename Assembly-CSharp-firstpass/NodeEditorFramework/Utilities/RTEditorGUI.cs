using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace NodeEditorFramework.Utilities
{
	public static class RTEditorGUI
	{
		private static float textFieldHeight
		{
			get
			{
				return GUI.skin.textField.CalcHeight(new GUIContent("i"), 10f);
			}
		}

		public static Rect PrefixLabel(Rect totalPos, GUIContent label, GUIStyle style)
		{
			if (label == GUIContent.none)
			{
				return totalPos;
			}
			GUI.Label(new Rect(totalPos.x + RTEditorGUI.indent, totalPos.y, Mathf.Min(RTEditorGUI.getLabelWidth() - RTEditorGUI.indent, totalPos.width / 2f), totalPos.height), label, style);
			return new Rect(totalPos.x + RTEditorGUI.getLabelWidth(), totalPos.y, totalPos.width - RTEditorGUI.getLabelWidth(), totalPos.height);
		}

		public static Rect PrefixLabel(Rect totalPos, float percentage, GUIContent label, GUIStyle style)
		{
			if (label == GUIContent.none)
			{
				return totalPos;
			}
			GUI.Label(new Rect(totalPos.x + RTEditorGUI.indent, totalPos.y, totalPos.width * percentage, totalPos.height), label, style);
			return new Rect(totalPos.x + totalPos.width * percentage, totalPos.y, totalPos.width * (1f - percentage), totalPos.height);
		}

		private static Rect IndentedRect(Rect source)
		{
			return new Rect(source.x + RTEditorGUI.indent, source.y, source.width - RTEditorGUI.indent, source.height);
		}

		private static float getLabelWidth()
		{
			if (RTEditorGUI.labelWidth == 0f)
			{
				return 150f;
			}
			return RTEditorGUI.labelWidth;
		}

		private static float getFieldWidth()
		{
			if (RTEditorGUI.fieldWidth == 0f)
			{
				return 50f;
			}
			return RTEditorGUI.fieldWidth;
		}

		private static Rect GetFieldRect(GUIContent label, GUIStyle style, params GUILayoutOption[] options)
		{
			float num = 0f;
			float num2 = 0f;
			if (label != GUIContent.none)
			{
				style.CalcMinMaxWidth(label, out num, out num2);
			}
			return GUILayoutUtility.GetRect(RTEditorGUI.getFieldWidth() + num + 5f, RTEditorGUI.getFieldWidth() + num2 + 5f, RTEditorGUI.textFieldHeight, RTEditorGUI.textFieldHeight, options);
		}

		private static Rect GetSliderRect(GUIContent label, GUIStyle style, params GUILayoutOption[] options)
		{
			float num = 0f;
			float num2 = 0f;
			if (label != GUIContent.none)
			{
				style.CalcMinMaxWidth(label, out num, out num2);
			}
			return GUILayoutUtility.GetRect(RTEditorGUI.getFieldWidth() + num + 5f, RTEditorGUI.getFieldWidth() + num2 + 5f + 100f, RTEditorGUI.textFieldHeight, RTEditorGUI.textFieldHeight, options);
		}

		private static Rect GetSliderRect(Rect sliderRect)
		{
			return new Rect(sliderRect.x, sliderRect.y, sliderRect.width - RTEditorGUI.getFieldWidth() - 5f, sliderRect.height);
		}

		private static Rect GetSliderFieldRect(Rect sliderRect)
		{
			return new Rect(sliderRect.x + sliderRect.width - RTEditorGUI.getFieldWidth(), sliderRect.y, RTEditorGUI.getFieldWidth(), sliderRect.height);
		}

		public static void Space()
		{
			RTEditorGUI.Space(6f);
		}

		public static void Space(float pixels)
		{
			GUILayoutUtility.GetRect(pixels, pixels);
		}

		public static void Seperator()
		{
			RTEditorGUI.setupSeperator();
			GUILayout.Box(GUIContent.none, RTEditorGUI.seperator, new GUILayoutOption[] { GUILayout.Height(1f) });
		}

		public static void Seperator(Rect rect)
		{
			RTEditorGUI.setupSeperator();
			GUI.Box(new Rect(rect.x, rect.y, rect.width, 1f), GUIContent.none, RTEditorGUI.seperator);
		}

		private static void setupSeperator()
		{
			if (RTEditorGUI.seperator == null)
			{
				RTEditorGUI.seperator = new GUIStyle();
				RTEditorGUI.seperator.normal.background = RTEditorGUI.ColorToTex(1, new Color(0.6f, 0.6f, 0.6f));
				RTEditorGUI.seperator.stretchWidth = true;
				RTEditorGUI.seperator.margin = new RectOffset(0, 0, 7, 7);
			}
		}

		public static void BeginChangeCheck()
		{
			RTEditorGUI.changeStack.Push(GUI.changed);
			GUI.changed = false;
		}

		public static bool EndChangeCheck()
		{
			bool changed = GUI.changed;
			if (RTEditorGUI.changeStack.Count > 0)
			{
				GUI.changed = RTEditorGUI.changeStack.Pop();
				if (changed && RTEditorGUI.changeStack.Count > 0 && !RTEditorGUI.changeStack.Peek())
				{
					RTEditorGUI.changeStack.Pop();
					RTEditorGUI.changeStack.Push(changed);
				}
			}
			else
			{
				global::Debug.LogWarning("Requesting more EndChangeChecks than issuing BeginChangeChecks!");
			}
			return changed;
		}

		public static bool Foldout(bool foldout, string content, params GUILayoutOption[] options)
		{
			return RTEditorGUI.Foldout(foldout, new GUIContent(content), options);
		}

		public static bool Foldout(bool foldout, string content, GUIStyle style, params GUILayoutOption[] options)
		{
			return RTEditorGUI.Foldout(foldout, new GUIContent(content), style, options);
		}

		public static bool Foldout(bool foldout, GUIContent content, params GUILayoutOption[] options)
		{
			return RTEditorGUI.Foldout(foldout, content, GUI.skin.toggle, options);
		}

		public static bool Foldout(bool foldout, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			return GUILayout.Toggle(foldout, content, style, options);
		}

		public static bool Toggle(bool toggle, string content, params GUILayoutOption[] options)
		{
			return RTEditorGUI.Toggle(toggle, new GUIContent(content), options);
		}

		public static bool Toggle(bool toggle, string content, GUIStyle style, params GUILayoutOption[] options)
		{
			return RTEditorGUI.Toggle(toggle, new GUIContent(content), style, options);
		}

		public static bool Toggle(bool toggle, GUIContent content, params GUILayoutOption[] options)
		{
			return RTEditorGUI.Toggle(toggle, content, GUI.skin.toggle, options);
		}

		public static bool Toggle(bool toggle, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			return GUILayout.Toggle(toggle, content, style, options);
		}

		public static string TextField(GUIContent label, string text, GUIStyle style, params GUILayoutOption[] options)
		{
			if (style == null)
			{
				style = GUI.skin.textField;
			}
			text = GUI.TextField(RTEditorGUI.PrefixLabel(RTEditorGUI.GetFieldRect(label, style, options), 0.5f, label, style), text);
			return text;
		}

		public static int OptionSlider(GUIContent label, int selected, string[] selectableOptions, params GUILayoutOption[] options)
		{
			return RTEditorGUI.OptionSlider(label, selected, selectableOptions, GUI.skin.label, options);
		}

		public static int OptionSlider(GUIContent label, int selected, string[] selectableOptions, GUIStyle style, params GUILayoutOption[] options)
		{
			if (style == null)
			{
				style = GUI.skin.textField;
			}
			Rect rect = RTEditorGUI.PrefixLabel(RTEditorGUI.GetSliderRect(label, style, options), 0.5f, label, style);
			selected = Mathf.RoundToInt(GUI.HorizontalSlider(RTEditorGUI.GetSliderRect(rect), (float)selected, 0f, (float)(selectableOptions.Length - 1)));
			GUI.Label(RTEditorGUI.GetSliderFieldRect(rect), selectableOptions[selected]);
			return selected;
		}

		public static int MathPowerSlider(GUIContent label, int baseValue, int value, int minPow, int maxPow, params GUILayoutOption[] options)
		{
			int num = (int)Math.Floor(Math.Log((double)value) / Math.Log((double)baseValue));
			num = RTEditorGUI.MathPowerSliderRaw(label, baseValue, num, minPow, maxPow, options);
			return (int)Math.Pow((double)baseValue, (double)num);
		}

		public static int MathPowerSliderRaw(GUIContent label, int baseValue, int power, int minPow, int maxPow, params GUILayoutOption[] options)
		{
			Rect rect = RTEditorGUI.PrefixLabel(RTEditorGUI.GetSliderRect(label, GUI.skin.label, options), 0.5f, label, GUI.skin.label);
			power = Mathf.RoundToInt(GUI.HorizontalSlider(RTEditorGUI.GetSliderRect(rect), (float)power, (float)minPow, (float)maxPow));
			GUI.Label(RTEditorGUI.GetSliderFieldRect(rect), Mathf.Pow((float)baseValue, (float)power).ToString());
			return power;
		}

		public static int IntSlider(string label, int value, int minValue, int maxValue, params GUILayoutOption[] options)
		{
			return (int)RTEditorGUI.Slider(new GUIContent(label), (float)value, (float)minValue, (float)maxValue, options);
		}

		public static int IntSlider(GUIContent label, int value, int minValue, int maxValue, params GUILayoutOption[] options)
		{
			return (int)RTEditorGUI.Slider(label, (float)value, (float)minValue, (float)maxValue, options);
		}

		public static int IntSlider(int value, int minValue, int maxValue, params GUILayoutOption[] options)
		{
			return (int)RTEditorGUI.Slider(GUIContent.none, (float)value, (float)minValue, (float)maxValue, options);
		}

		public static int IntField(string label, int value, params GUILayoutOption[] options)
		{
			return (int)RTEditorGUI.FloatField(new GUIContent(label), (float)value, options);
		}

		public static int IntField(GUIContent label, int value, params GUILayoutOption[] options)
		{
			return (int)RTEditorGUI.FloatField(label, (float)value, options);
		}

		public static int IntField(int value, params GUILayoutOption[] options)
		{
			return (int)RTEditorGUI.FloatField((float)value, options);
		}

		public static float Slider(float value, float minValue, float maxValue, params GUILayoutOption[] options)
		{
			return RTEditorGUI.Slider(GUIContent.none, value, minValue, maxValue, options);
		}

		public static float Slider(string label, float value, float minValue, float maxValue, params GUILayoutOption[] options)
		{
			return RTEditorGUI.Slider(new GUIContent(label), value, minValue, maxValue, options);
		}

		public static float Slider(GUIContent label, float value, float minValue, float maxValue, params GUILayoutOption[] options)
		{
			Rect rect = RTEditorGUI.PrefixLabel(RTEditorGUI.GetSliderRect(label, GUI.skin.label, options), 0.5f, label, GUI.skin.label);
			value = GUI.HorizontalSlider(RTEditorGUI.GetSliderRect(rect), value, minValue, maxValue);
			value = Mathf.Min(maxValue, Mathf.Max(minValue, RTEditorGUI.FloatField(RTEditorGUI.GetSliderFieldRect(rect), value, new GUILayoutOption[] { GUILayout.Width(60f) })));
			return value;
		}

		public static float FloatField(string label, float value, params GUILayoutOption[] fieldOptions)
		{
			return RTEditorGUI.FloatField(new GUIContent(label), value, fieldOptions);
		}

		public static float FloatField(GUIContent label, float value, params GUILayoutOption[] options)
		{
			return RTEditorGUI.FloatField(RTEditorGUI.PrefixLabel(RTEditorGUI.GetFieldRect(label, GUI.skin.label, options), 0.5f, label, GUI.skin.label), value, options);
		}

		public static float FloatField(float value, params GUILayoutOption[] options)
		{
			return RTEditorGUI.FloatField(RTEditorGUI.GetFieldRect(GUIContent.none, null, options), value, options);
		}

		public static float FloatField(Rect pos, float value, params GUILayoutOption[] options)
		{
			int num = GUIUtility.GetControlID("FloatField".GetHashCode(), FocusType.Keyboard, pos) + 1;
			if (num == 0)
			{
				return value;
			}
			bool flag = RTEditorGUI.activeFloatField == num;
			bool flag2 = num == GUIUtility.keyboardControl;
			if (flag2 && flag && RTEditorGUI.activeFloatFieldLastValue != value)
			{
				RTEditorGUI.activeFloatFieldLastValue = value;
				RTEditorGUI.activeFloatFieldString = value.ToString();
			}
			string text = (flag ? RTEditorGUI.activeFloatFieldString : value.ToString());
			string text2 = GUI.TextField(pos, text);
			if (flag)
			{
				RTEditorGUI.activeFloatFieldString = text2;
			}
			bool flag3 = true;
			if (text2 == "")
			{
				value = (RTEditorGUI.activeFloatFieldLastValue = 0f);
			}
			else if (text2 != value.ToString())
			{
				float num2;
				flag3 = float.TryParse(text2, out num2);
				if (flag3)
				{
					value = (RTEditorGUI.activeFloatFieldLastValue = num2);
				}
			}
			if (flag2 && !flag)
			{
				RTEditorGUI.activeFloatField = num;
				RTEditorGUI.activeFloatFieldString = text2;
				RTEditorGUI.activeFloatFieldLastValue = value;
			}
			else if (!flag2 && flag)
			{
				RTEditorGUI.activeFloatField = -1;
				if (!flag3)
				{
					value = text2.ForceParse();
				}
			}
			return value;
		}

		public static float ForceParse(this string str)
		{
			float num;
			if (float.TryParse(str, out num))
			{
				return num;
			}
			bool flag = false;
			List<char> list = new List<char>(str);
			for (int i = 0; i < list.Count; i++)
			{
				if (CharUnicodeInfo.GetUnicodeCategory(str[i]) != UnicodeCategory.DecimalDigitNumber)
				{
					list.RemoveRange(i, list.Count - i);
					break;
				}
				if (str[i] == '.')
				{
					if (flag)
					{
						list.RemoveRange(i, list.Count - i);
						break;
					}
					flag = true;
				}
			}
			if (list.Count == 0)
			{
				return 0f;
			}
			str = new string(list.ToArray());
			if (!float.TryParse(str, out num))
			{
				global::Debug.LogError("Could not parse " + str);
			}
			return num;
		}

		public static T ObjectField<T>(T obj, bool allowSceneObjects) where T : global::UnityEngine.Object
		{
			return RTEditorGUI.ObjectField<T>(GUIContent.none, obj, allowSceneObjects, Array.Empty<GUILayoutOption>());
		}

		public static T ObjectField<T>(string label, T obj, bool allowSceneObjects) where T : global::UnityEngine.Object
		{
			return RTEditorGUI.ObjectField<T>(new GUIContent(label), obj, allowSceneObjects, Array.Empty<GUILayoutOption>());
		}

		public static T ObjectField<T>(GUIContent label, T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : global::UnityEngine.Object
		{
			if (obj.GetType() == typeof(Texture2D))
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Label(label, Array.Empty<GUILayoutOption>());
				bool flag = GUILayout.Button(obj as Texture2D, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(64f),
					GUILayout.MaxHeight(64f)
				});
				GUILayout.EndHorizontal();
			}
			else
			{
				GUIStyle guistyle = new GUIStyle(GUI.skin.box);
				bool flag = GUILayout.Button(label, guistyle, Array.Empty<GUILayoutOption>());
			}
			return obj;
		}

		public static Enum EnumPopup(Enum selected)
		{
			return RTEditorGUI.EnumPopup(GUIContent.none, selected);
		}

		public static Enum EnumPopup(string label, Enum selected)
		{
			return RTEditorGUI.EnumPopup(new GUIContent(label), selected);
		}

		public static Enum EnumPopup(GUIContent label, Enum selected)
		{
			label.text = label.text + ": " + selected.ToString();
			GUILayout.Label(label, Array.Empty<GUILayoutOption>());
			return selected;
		}

		public static int Popup(GUIContent label, int selected, string[] displayedOptions)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			label.text = label.text + ": " + selected.ToString();
			GUILayout.Label(label, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return selected;
		}

		public static int Popup(string label, int selected, string[] displayedOptions)
		{
			GUILayout.Label(label + ": " + selected.ToString(), Array.Empty<GUILayoutOption>());
			return selected;
		}

		public static int Popup(int selected, string[] displayedOptions)
		{
			return RTEditorGUI.Popup("", selected, displayedOptions);
		}

		public static void DrawTexture(Texture texture, int texSize, GUIStyle style, params GUILayoutOption[] options)
		{
			RTEditorGUI.DrawTexture(texture, texSize, style, 1, 2, 3, 4, options);
		}

		public static void DrawTexture(Texture texture, int texSize, GUIStyle style, int shuffleRed, int shuffleGreen, int shuffleBlue, int shuffleAlpha, params GUILayoutOption[] options)
		{
			if (RTEditorGUI.texVizMat == null)
			{
				RTEditorGUI.texVizMat = new Material(Shader.Find("Hidden/GUITextureClip_ChannelControl"));
			}
			RTEditorGUI.texVizMat.SetInt("shuffleRed", shuffleRed);
			RTEditorGUI.texVizMat.SetInt("shuffleGreen", shuffleGreen);
			RTEditorGUI.texVizMat.SetInt("shuffleBlue", shuffleBlue);
			RTEditorGUI.texVizMat.SetInt("shuffleAlpha", shuffleAlpha);
			if (options == null || options.Length == 0)
			{
				options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
			}
			Rect rect = ((style == null) ? GUILayoutUtility.GetRect((float)texSize, (float)texSize, options) : GUILayoutUtility.GetRect((float)texSize, (float)texSize, style, options));
			if (Event.current.type == EventType.Repaint)
			{
				Graphics.DrawTexture(rect, texture, RTEditorGUI.texVizMat);
			}
		}

		private static void SetupLineMat(Texture tex, Color col)
		{
			if (RTEditorGUI.lineMaterial == null)
			{
				RTEditorGUI.lineMaterial = new Material(Shader.Find("Hidden/LineShader"));
			}
			if (tex == null)
			{
				tex = ((RTEditorGUI.lineTexture != null) ? RTEditorGUI.lineTexture : (RTEditorGUI.lineTexture = ResourceManager.LoadTexture("Textures/AALine.png")));
			}
			RTEditorGUI.lineMaterial.SetTexture("_LineTexture", tex);
			RTEditorGUI.lineMaterial.SetColor("_LineColor", col);
			RTEditorGUI.lineMaterial.SetPass(0);
		}

		public static void DrawBezier(Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan, Color col, Texture2D tex, float width = 1f)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			int num = RTEditorGUI.CalculateBezierSegmentCount(startPos, endPos, startTan, endTan);
			RTEditorGUI.DrawBezier(startPos, endPos, startTan, endTan, col, tex, num, width);
		}

		public static void DrawBezier(Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan, Color col, Texture2D tex, int segmentCount, float width)
		{
			if (Event.current.type != EventType.Repaint && Event.current.type != EventType.KeyDown)
			{
				return;
			}
			Vector2[] array = new Vector2[segmentCount + 1];
			for (int i = 0; i <= segmentCount; i++)
			{
				array[i] = RTEditorGUI.GetBezierPoint((float)i / (float)segmentCount, startPos, endPos, startTan, endTan);
			}
			RTEditorGUI.DrawPolygonLine(array, col, tex, width);
		}

		public static void DrawBezier(Rect clippingRect, Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan, Color col, Texture2D tex, int segmentCount, float width)
		{
			if (Event.current.type != EventType.Repaint && Event.current.type != EventType.KeyDown)
			{
				return;
			}
			Vector2[] array = new Vector2[segmentCount + 1];
			for (int i = 0; i <= segmentCount; i++)
			{
				array[i] = RTEditorGUI.GetBezierPoint((float)i / (float)segmentCount, startPos, endPos, startTan, endTan);
			}
			RTEditorGUI.DrawPolygonLine(clippingRect, array, col, tex, width);
		}

		public static void DrawPolygonLine(Vector2[] points, Color col, Texture2D tex, float width = 1f)
		{
			RTEditorGUI.DrawPolygonLine(GUIScaleUtility.getTopRect, points, col, tex, width);
		}

		public static void DrawPolygonLine(Rect clippingRect, Vector2[] points, Color col, Texture2D tex, float width = 1f)
		{
			if (Event.current.type != EventType.Repaint && Event.current.type != EventType.KeyDown)
			{
				return;
			}
			if (points.Length == 1)
			{
				return;
			}
			if (points.Length == 2)
			{
				RTEditorGUI.DrawLine(points[0], points[1], col, tex, width);
			}
			RTEditorGUI.SetupLineMat(tex, col);
			GL.Begin(5);
			GL.Color(Color.white);
			clippingRect.x = (clippingRect.y = 0f);
			Vector2 vector = points[0];
			for (int i = 1; i < points.Length; i++)
			{
				Vector2 vector2 = points[i];
				Vector2 vector3 = vector;
				Vector2 vector4 = vector2;
				bool flag;
				bool flag2;
				if (RTEditorGUI.SegmentRectIntersection(clippingRect, ref vector, ref vector2, out flag, out flag2))
				{
					Vector2 vector5;
					if (i < points.Length - 1)
					{
						vector5 = RTEditorGUI.CalculatePointPerpendicular(vector3, vector4, points[i + 1]);
					}
					else
					{
						vector5 = RTEditorGUI.CalculateLinePerpendicular(vector3, vector4);
					}
					if (flag)
					{
						GL.End();
						GL.Begin(5);
						RTEditorGUI.DrawLineSegment(vector, vector5 * width / 2f);
					}
					if (i == 1)
					{
						RTEditorGUI.DrawLineSegment(vector, RTEditorGUI.CalculateLinePerpendicular(vector, vector2) * width / 2f);
					}
					RTEditorGUI.DrawLineSegment(vector2, vector5 * width / 2f);
				}
				else if (flag2)
				{
					GL.End();
					GL.Begin(5);
				}
				vector = vector4;
			}
			GL.End();
		}

		private static int CalculateBezierSegmentCount(Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan)
		{
			float num = Vector2.Angle(startTan - startPos, endPos - startPos) * Vector2.Angle(endTan - endPos, startPos - endPos) * (endTan.magnitude + startTan.magnitude);
			num = 2f + Mathf.Pow(num / 400f, 0.125f);
			float num2 = 1f + (startPos - endPos).magnitude;
			num2 = Mathf.Pow(num2, 0.25f);
			return 4 + (int)(num * num2);
		}

		private static Vector2 CalculateLinePerpendicular(Vector2 startPos, Vector2 endPos)
		{
			return new Vector2(endPos.y - startPos.y, startPos.x - endPos.x).normalized;
		}

		private static Vector2 CalculatePointPerpendicular(Vector2 prevPos, Vector2 pointPos, Vector2 nextPos)
		{
			return RTEditorGUI.CalculateLinePerpendicular(pointPos, pointPos + (nextPos - prevPos));
		}

		private static Vector2 GetBezierPoint(float t, Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan)
		{
			float num = 1f - t;
			float num2 = num * t;
			return startPos * num * num * num + startTan * 3f * num * num2 + endTan * 3f * num2 * t + endPos * t * t * t;
		}

		private static void DrawLineSegment(Vector2 point, Vector2 perpendicular)
		{
			GL.TexCoord2(0f, 0f);
			GL.Vertex(point - perpendicular);
			GL.TexCoord2(0f, 1f);
			GL.Vertex(point + perpendicular);
		}

		public static void DrawLine(Vector2 startPos, Vector2 endPos, Color col, Texture2D tex, float width = 1f)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			RTEditorGUI.DrawLine(GUIScaleUtility.getTopRect, startPos, endPos, col, tex, width);
		}

		public static void DrawLine(Rect clippingRect, Vector2 startPos, Vector2 endPos, Color col, Texture2D tex, float width = 1f)
		{
			RTEditorGUI.SetupLineMat(tex, col);
			GL.Begin(5);
			GL.Color(Color.white);
			clippingRect.x = (clippingRect.y = 0f);
			if (RTEditorGUI.SegmentRectIntersection(clippingRect, ref startPos, ref endPos))
			{
				Vector2 vector = RTEditorGUI.CalculateLinePerpendicular(startPos, endPos) * width / 2f;
				RTEditorGUI.DrawLineSegment(startPos, vector);
				RTEditorGUI.DrawLineSegment(endPos, vector);
			}
			GL.End();
		}

		public static List<Vector2> GetLine(Rect clippingRect, Vector2 startPos, Vector2 endPos, float width = 1f, bool noClip = false)
		{
			List<Vector2> list = new List<Vector2>();
			if (noClip || RTEditorGUI.SegmentRectIntersection(clippingRect, ref startPos, ref endPos))
			{
				Vector2 vector = RTEditorGUI.CalculateLinePerpendicular(startPos, endPos) * width / 2f;
				list.Add(startPos - vector);
				list.Add(endPos + vector);
			}
			return list;
		}

		private static bool SegmentRectIntersection(Rect bounds, ref Vector2 p0, ref Vector2 p1)
		{
			bool flag;
			bool flag2;
			return RTEditorGUI.SegmentRectIntersection(bounds, ref p0, ref p1, out flag, out flag2);
		}

		private static bool SegmentRectIntersection(Rect bounds, ref Vector2 p0, ref Vector2 p1, out bool clippedP0, out bool clippedP1)
		{
			float num = 0f;
			float num2 = 1f;
			float num3 = p1.x - p0.x;
			float num4 = p1.y - p0.y;
			if (RTEditorGUI.ClipTest(-num3, p0.x - bounds.xMin, ref num, ref num2) && RTEditorGUI.ClipTest(num3, bounds.xMax - p0.x, ref num, ref num2) && RTEditorGUI.ClipTest(-num4, p0.y - bounds.yMin, ref num, ref num2) && RTEditorGUI.ClipTest(num4, bounds.yMax - p0.y, ref num, ref num2))
			{
				clippedP0 = num > 0f;
				clippedP1 = num2 < 1f;
				if (clippedP1)
				{
					p1.x = p0.x + num2 * num3;
					p1.y = p0.y + num2 * num4;
				}
				if (clippedP0)
				{
					p0.x += num * num3;
					p0.y += num * num4;
				}
				return true;
			}
			clippedP1 = (clippedP0 = true);
			return false;
		}

		private static bool ClipTest(float p, float q, ref float t0, ref float t1)
		{
			float num = q / p;
			if (p < 0f)
			{
				if (num > t1)
				{
					return false;
				}
				if (num > t0)
				{
					t0 = num;
				}
			}
			else if (p > 0f)
			{
				if (num < t0)
				{
					return false;
				}
				if (num < t1)
				{
					t1 = num;
				}
			}
			else if (q < 0f)
			{
				return false;
			}
			return true;
		}

		public static Texture2D ColorToTex(int pxSize, Color col)
		{
			Texture2D texture2D = new Texture2D(pxSize, pxSize);
			texture2D.name = "RTEditorGUI";
			for (int i = 0; i < pxSize; i++)
			{
				for (int j = 0; j < pxSize; j++)
				{
					texture2D.SetPixel(i, j, col);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D Tint(Texture2D tex, Color color)
		{
			Texture2D texture2D = global::UnityEngine.Object.Instantiate<Texture2D>(tex);
			for (int i = 0; i < tex.width; i++)
			{
				for (int j = 0; j < tex.height; j++)
				{
					texture2D.SetPixel(i, j, tex.GetPixel(i, j) * color);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D RotateTextureCCW(Texture2D tex, int quarterSteps)
		{
			if (tex == null)
			{
				return null;
			}
			tex = global::UnityEngine.Object.Instantiate<Texture2D>(tex);
			int width = tex.width;
			int height = tex.height;
			Color[] pixels = tex.GetPixels();
			Color[] array = new Color[width * height];
			for (int i = 0; i < quarterSteps; i++)
			{
				for (int j = 0; j < width; j++)
				{
					for (int k = 0; k < height; k++)
					{
						array[j * width + k] = pixels[(width - k - 1) * width + j];
					}
				}
				array.CopyTo(pixels, 0);
			}
			tex.SetPixels(pixels);
			tex.Apply();
			return tex;
		}

		public static float labelWidth = 150f;

		public static float fieldWidth = 50f;

		public static float indent = 0f;

		private static GUIStyle seperator;

		private static Stack<bool> changeStack = new Stack<bool>();

		private static int activeFloatField = -1;

		private static float activeFloatFieldLastValue = 0f;

		private static string activeFloatFieldString = "";

		private static Material texVizMat;

		private static Material lineMaterial;

		private static Texture2D lineTexture;
	}
}
