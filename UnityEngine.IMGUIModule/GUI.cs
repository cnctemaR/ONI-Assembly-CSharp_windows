using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine
{
	[NativeHeader("Modules/IMGUI/GUISkin.bindings.h")]
	[NativeHeader("Modules/IMGUI/GUI.bindings.h")]
	public class GUI
	{
		public static Color color
		{
			get
			{
				Color color;
				GUI.get_color_Injected(out color);
				return color;
			}
			set
			{
				GUI.set_color_Injected(ref value);
			}
		}

		public static Color backgroundColor
		{
			get
			{
				Color color;
				GUI.get_backgroundColor_Injected(out color);
				return color;
			}
			set
			{
				GUI.set_backgroundColor_Injected(ref value);
			}
		}

		public static Color contentColor
		{
			get
			{
				Color color;
				GUI.get_contentColor_Injected(out color);
				return color;
			}
			set
			{
				GUI.set_contentColor_Injected(ref value);
			}
		}

		public static extern bool changed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int depth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool usePageScrollbars
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern Material blendMaterial
		{
			[FreeFunction("GetGUIBlendMaterial")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern Material blitMaterial
		{
			[FreeFunction("GetGUIBlitMaterial")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern Material roundedRectMaterial
		{
			[FreeFunction("GetGUIRoundedRectMaterial")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GrabMouseControl(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasMouseControl(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseMouseControl();

		[FreeFunction("GetGUIState().SetNameOfNextControl")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetNextControlName(string name);

		[FreeFunction("GetGUIState().GetNameOfFocusedControl")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetNameOfFocusedControl();

		[FreeFunction("GetGUIState().FocusKeyboardControl")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FocusControl(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalRepaintEditorWindow();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetTooltip();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetTooltip(string value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetMouseTooltip();

		private static Rect Internal_DoModalWindow(int id, int instanceID, Rect clientRect, GUI.WindowFunction func, GUIContent content, GUIStyle style, object skin)
		{
			Rect rect;
			GUI.Internal_DoModalWindow_Injected(id, instanceID, ref clientRect, func, content, style, skin, out rect);
			return rect;
		}

		private static Rect Internal_DoWindow(int id, int instanceID, Rect clientRect, GUI.WindowFunction func, GUIContent title, GUIStyle style, object skin, bool forceRectOnLayout)
		{
			Rect rect;
			GUI.Internal_DoWindow_Injected(id, instanceID, ref clientRect, func, title, style, skin, forceRectOnLayout, out rect);
			return rect;
		}

		public static void DragWindow(Rect position)
		{
			GUI.DragWindow_Injected(ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BringWindowToFront(int windowID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BringWindowToBack(int windowID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FocusWindow(int windowID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnfocusWindow();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BeginWindows();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_EndWindows();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string Internal_Concatenate(GUIContent first, GUIContent second);

		internal static int scrollTroughSide { get; set; }

		internal static DateTime nextScrollStepTime { get; set; } = DateTime.Now;

		public static GUISkin skin
		{
			get
			{
				GUIUtility.CheckOnGUI();
				return GUI.s_Skin;
			}
			set
			{
				GUIUtility.CheckOnGUI();
				GUI.DoSetSkin(value);
			}
		}

		internal static void DoSetSkin(GUISkin newSkin)
		{
			if (!newSkin)
			{
				newSkin = GUIUtility.GetDefaultSkin();
			}
			GUI.s_Skin = newSkin;
			newSkin.MakeCurrent();
		}

		internal static void CleanupRoots()
		{
			GUI.s_Skin = null;
			GUIUtility.CleanupRoots();
			GUILayoutUtility.CleanupRoots();
			GUISkin.CleanupRoots();
			GUIStyle.CleanupRoots();
		}

		public static Matrix4x4 matrix
		{
			get
			{
				return GUIClip.GetMatrix();
			}
			set
			{
				GUIClip.SetMatrix(value);
			}
		}

		public static string tooltip
		{
			get
			{
				string text = GUI.Internal_GetTooltip();
				string text2;
				if (text != null)
				{
					text2 = text;
				}
				else
				{
					text2 = "";
				}
				return text2;
			}
			set
			{
				GUI.Internal_SetTooltip(value);
			}
		}

		protected static string mouseTooltip
		{
			[CompilerGenerated]
			get
			{
				return GUI.Internal_GetMouseTooltip();
			}
		}

		protected static Rect tooltipRect
		{
			get
			{
				return GUI.s_ToolTipRect;
			}
			set
			{
				GUI.s_ToolTipRect = value;
			}
		}

		public static void Label(Rect position, string text)
		{
			GUI.Label(position, GUIContent.Temp(text), GUI.s_Skin.label);
		}

		public static void Label(Rect position, Texture image)
		{
			GUI.Label(position, GUIContent.Temp(image), GUI.s_Skin.label);
		}

		public static void Label(Rect position, GUIContent content)
		{
			GUI.Label(position, content, GUI.s_Skin.label);
		}

		public static void Label(Rect position, string text, GUIStyle style)
		{
			GUI.Label(position, GUIContent.Temp(text), style);
		}

		public static void Label(Rect position, Texture image, GUIStyle style)
		{
			GUI.Label(position, GUIContent.Temp(image), style);
		}

		public static void Label(Rect position, GUIContent content, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			GUI.DoLabel(position, content, style);
		}

		public static void DrawTexture(Rect position, Texture image)
		{
			GUI.DrawTexture(position, image, ScaleMode.StretchToFill);
		}

		public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode)
		{
			GUI.DrawTexture(position, image, scaleMode, true);
		}

		public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend)
		{
			GUI.DrawTexture(position, image, scaleMode, alphaBlend, 0f);
		}

		public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect)
		{
			GUI.DrawTexture(position, image, scaleMode, alphaBlend, imageAspect, GUI.color, 0f, 0f);
		}

		public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect, Color color, float borderWidth, float borderRadius)
		{
			Vector4 vector = Vector4.one * borderWidth;
			GUI.DrawTexture(position, image, scaleMode, alphaBlend, imageAspect, color, vector, borderRadius);
		}

		public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect, Color color, Vector4 borderWidths, float borderRadius)
		{
			Vector4 vector = Vector4.one * borderRadius;
			GUI.DrawTexture(position, image, scaleMode, alphaBlend, imageAspect, color, borderWidths, vector);
		}

		public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect, Color color, Vector4 borderWidths, Vector4 borderRadiuses)
		{
			GUIUtility.CheckOnGUI();
			if (Event.current.type == EventType.Repaint)
			{
				if (image == null)
				{
					Debug.LogWarning("null texture passed to GUI.DrawTexture");
				}
				else
				{
					if (imageAspect == 0f)
					{
						imageAspect = (float)image.width / (float)image.height;
					}
					Material material;
					if (borderWidths != Vector4.zero || borderRadiuses != Vector4.zero)
					{
						material = GUI.roundedRectMaterial;
					}
					else
					{
						material = ((!alphaBlend) ? GUI.blitMaterial : GUI.blendMaterial);
					}
					Internal_DrawTextureArguments internal_DrawTextureArguments = new Internal_DrawTextureArguments
					{
						leftBorder = 0,
						rightBorder = 0,
						topBorder = 0,
						bottomBorder = 0,
						color = color,
						borderWidths = borderWidths,
						cornerRadiuses = borderRadiuses,
						texture = image,
						mat = material
					};
					GUI.CalculateScaledTextureRects(position, scaleMode, imageAspect, ref internal_DrawTextureArguments.screenRect, ref internal_DrawTextureArguments.sourceRect);
					Graphics.Internal_DrawTexture(ref internal_DrawTextureArguments);
				}
			}
		}

		internal static bool CalculateScaledTextureRects(Rect position, ScaleMode scaleMode, float imageAspect, ref Rect outScreenRect, ref Rect outSourceRect)
		{
			float num = position.width / position.height;
			bool flag = false;
			if (scaleMode != ScaleMode.StretchToFill)
			{
				if (scaleMode != ScaleMode.ScaleAndCrop)
				{
					if (scaleMode == ScaleMode.ScaleToFit)
					{
						if (num > imageAspect)
						{
							float num2 = imageAspect / num;
							outScreenRect = new Rect(position.xMin + position.width * (1f - num2) * 0.5f, position.yMin, num2 * position.width, position.height);
							outSourceRect = new Rect(0f, 0f, 1f, 1f);
							flag = true;
						}
						else
						{
							float num3 = num / imageAspect;
							outScreenRect = new Rect(position.xMin, position.yMin + position.height * (1f - num3) * 0.5f, position.width, num3 * position.height);
							outSourceRect = new Rect(0f, 0f, 1f, 1f);
							flag = true;
						}
					}
				}
				else if (num > imageAspect)
				{
					float num4 = imageAspect / num;
					outScreenRect = position;
					outSourceRect = new Rect(0f, (1f - num4) * 0.5f, 1f, num4);
					flag = true;
				}
				else
				{
					float num5 = num / imageAspect;
					outScreenRect = position;
					outSourceRect = new Rect(0.5f - num5 * 0.5f, 0f, num5, 1f);
					flag = true;
				}
			}
			else
			{
				outScreenRect = position;
				outSourceRect = new Rect(0f, 0f, 1f, 1f);
				flag = true;
			}
			return flag;
		}

		public static void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords)
		{
			GUI.DrawTextureWithTexCoords(position, image, texCoords, true);
		}

		public static void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords, bool alphaBlend)
		{
			GUIUtility.CheckOnGUI();
			if (Event.current.type == EventType.Repaint)
			{
				Material material = ((!alphaBlend) ? GUI.blitMaterial : GUI.blendMaterial);
				Internal_DrawTextureArguments internal_DrawTextureArguments = default(Internal_DrawTextureArguments);
				internal_DrawTextureArguments.texture = image;
				internal_DrawTextureArguments.mat = material;
				internal_DrawTextureArguments.leftBorder = 0;
				internal_DrawTextureArguments.rightBorder = 0;
				internal_DrawTextureArguments.topBorder = 0;
				internal_DrawTextureArguments.bottomBorder = 0;
				internal_DrawTextureArguments.color = GUI.color;
				internal_DrawTextureArguments.screenRect = position;
				internal_DrawTextureArguments.sourceRect = texCoords;
				Graphics.Internal_DrawTexture(ref internal_DrawTextureArguments);
			}
		}

		public static void Box(Rect position, string text)
		{
			GUI.Box(position, GUIContent.Temp(text), GUI.s_Skin.box);
		}

		public static void Box(Rect position, Texture image)
		{
			GUI.Box(position, GUIContent.Temp(image), GUI.s_Skin.box);
		}

		public static void Box(Rect position, GUIContent content)
		{
			GUI.Box(position, content, GUI.s_Skin.box);
		}

		public static void Box(Rect position, string text, GUIStyle style)
		{
			GUI.Box(position, GUIContent.Temp(text), style);
		}

		public static void Box(Rect position, Texture image, GUIStyle style)
		{
			GUI.Box(position, GUIContent.Temp(image), style);
		}

		public static void Box(Rect position, GUIContent content, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			int controlID = GUIUtility.GetControlID(GUI.s_BoxHash, FocusType.Passive);
			if (Event.current.type == EventType.Repaint)
			{
				style.Draw(position, content, controlID);
			}
		}

		public static bool Button(Rect position, string text)
		{
			return GUI.Button(position, GUIContent.Temp(text), GUI.s_Skin.button);
		}

		public static bool Button(Rect position, Texture image)
		{
			return GUI.Button(position, GUIContent.Temp(image), GUI.s_Skin.button);
		}

		public static bool Button(Rect position, GUIContent content)
		{
			return GUI.Button(position, content, GUI.s_Skin.button);
		}

		public static bool Button(Rect position, string text, GUIStyle style)
		{
			return GUI.Button(position, GUIContent.Temp(text), style);
		}

		public static bool Button(Rect position, Texture image, GUIStyle style)
		{
			return GUI.Button(position, GUIContent.Temp(image), style);
		}

		public static bool Button(Rect position, GUIContent content, GUIStyle style)
		{
			int controlID = GUIUtility.GetControlID(GUI.s_ButonHash, FocusType.Passive, position);
			return GUI.Button(position, controlID, content, style);
		}

		internal static bool Button(Rect position, int id, GUIContent content, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoButton(position, id, content, style);
		}

		public static bool RepeatButton(Rect position, string text)
		{
			return GUI.DoRepeatButton(position, GUIContent.Temp(text), GUI.s_Skin.button, FocusType.Passive);
		}

		public static bool RepeatButton(Rect position, Texture image)
		{
			return GUI.DoRepeatButton(position, GUIContent.Temp(image), GUI.s_Skin.button, FocusType.Passive);
		}

		public static bool RepeatButton(Rect position, GUIContent content)
		{
			return GUI.DoRepeatButton(position, content, GUI.s_Skin.button, FocusType.Passive);
		}

		public static bool RepeatButton(Rect position, string text, GUIStyle style)
		{
			return GUI.DoRepeatButton(position, GUIContent.Temp(text), style, FocusType.Passive);
		}

		public static bool RepeatButton(Rect position, Texture image, GUIStyle style)
		{
			return GUI.DoRepeatButton(position, GUIContent.Temp(image), style, FocusType.Passive);
		}

		public static bool RepeatButton(Rect position, GUIContent content, GUIStyle style)
		{
			return GUI.DoRepeatButton(position, content, style, FocusType.Passive);
		}

		private static bool DoRepeatButton(Rect position, GUIContent content, GUIStyle style, FocusType focusType)
		{
			GUIUtility.CheckOnGUI();
			int controlID = GUIUtility.GetControlID(GUI.s_RepeatButtonHash, focusType, position);
			EventType typeForControl = Event.current.GetTypeForControl(controlID);
			bool flag;
			if (typeForControl != EventType.MouseDown)
			{
				if (typeForControl != EventType.MouseUp)
				{
					if (typeForControl != EventType.Repaint)
					{
						flag = false;
					}
					else
					{
						style.Draw(position, content, controlID);
						flag = controlID == GUIUtility.hotControl && position.Contains(Event.current.mousePosition);
					}
				}
				else if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					Event.current.Use();
					flag = position.Contains(Event.current.mousePosition);
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				if (position.Contains(Event.current.mousePosition))
				{
					GUIUtility.hotControl = controlID;
					Event.current.Use();
				}
				flag = false;
			}
			return flag;
		}

		public static string TextField(Rect position, string text)
		{
			GUIContent guicontent = GUIContent.Temp(text);
			GUI.DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), guicontent, false, -1, GUI.skin.textField);
			return guicontent.text;
		}

		public static string TextField(Rect position, string text, int maxLength)
		{
			GUIContent guicontent = GUIContent.Temp(text);
			GUI.DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), guicontent, false, maxLength, GUI.skin.textField);
			return guicontent.text;
		}

		public static string TextField(Rect position, string text, GUIStyle style)
		{
			GUIContent guicontent = GUIContent.Temp(text);
			GUI.DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), guicontent, false, -1, style);
			return guicontent.text;
		}

		public static string TextField(Rect position, string text, int maxLength, GUIStyle style)
		{
			GUIContent guicontent = GUIContent.Temp(text);
			GUI.DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), guicontent, false, maxLength, style);
			return guicontent.text;
		}

		public static string PasswordField(Rect position, string password, char maskChar)
		{
			return GUI.PasswordField(position, password, maskChar, -1, GUI.skin.textField);
		}

		public static string PasswordField(Rect position, string password, char maskChar, int maxLength)
		{
			return GUI.PasswordField(position, password, maskChar, maxLength, GUI.skin.textField);
		}

		public static string PasswordField(Rect position, string password, char maskChar, GUIStyle style)
		{
			return GUI.PasswordField(position, password, maskChar, -1, style);
		}

		public static string PasswordField(Rect position, string password, char maskChar, int maxLength, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			string text = GUI.PasswordFieldGetStrToShow(password, maskChar);
			GUIContent guicontent = GUIContent.Temp(text);
			bool changed = GUI.changed;
			GUI.changed = false;
			if (TouchScreenKeyboard.isSupported)
			{
				GUI.DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard), guicontent, false, maxLength, style, password, maskChar);
			}
			else
			{
				GUI.DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), guicontent, false, maxLength, style);
			}
			text = ((!GUI.changed) ? password : guicontent.text);
			GUI.changed = GUI.changed || changed;
			return text;
		}

		internal static string PasswordFieldGetStrToShow(string password, char maskChar)
		{
			return (Event.current.type != EventType.Repaint && Event.current.type != EventType.MouseDown) ? password : "".PadRight(password.Length, maskChar);
		}

		public static string TextArea(Rect position, string text)
		{
			GUIContent guicontent = GUIContent.Temp(text);
			GUI.DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), guicontent, true, -1, GUI.skin.textArea);
			return guicontent.text;
		}

		public static string TextArea(Rect position, string text, int maxLength)
		{
			GUIContent guicontent = GUIContent.Temp(text);
			GUI.DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), guicontent, true, maxLength, GUI.skin.textArea);
			return guicontent.text;
		}

		public static string TextArea(Rect position, string text, GUIStyle style)
		{
			GUIContent guicontent = GUIContent.Temp(text);
			GUI.DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), guicontent, true, -1, style);
			return guicontent.text;
		}

		public static string TextArea(Rect position, string text, int maxLength, GUIStyle style)
		{
			GUIContent guicontent = GUIContent.Temp(text);
			GUI.DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), guicontent, true, maxLength, style);
			return guicontent.text;
		}

		internal static void DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style)
		{
			GUI.DoTextField(position, id, content, multiline, maxLength, style, null);
		}

		internal static void DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, string secureText)
		{
			GUI.DoTextField(position, id, content, multiline, maxLength, style, secureText, '\0');
		}

		internal static void DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, string secureText, char maskChar)
		{
			GUIUtility.CheckOnGUI();
			if (maxLength >= 0 && content.text.Length > maxLength)
			{
				content.text = content.text.Substring(0, maxLength);
			}
			TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), id);
			textEditor.text = content.text;
			textEditor.SaveBackup();
			textEditor.position = position;
			textEditor.style = style;
			textEditor.multiline = multiline;
			textEditor.controlID = id;
			textEditor.DetectFocusChange();
			if (TouchScreenKeyboard.isSupported)
			{
				GUI.HandleTextFieldEventForTouchscreen(position, id, content, multiline, maxLength, style, secureText, maskChar, textEditor);
			}
			else
			{
				GUI.HandleTextFieldEventForDesktop(position, id, content, multiline, maxLength, style, textEditor);
			}
			textEditor.UpdateScrollOffsetIfNeeded(Event.current);
		}

		private static void HandleTextFieldEventForTouchscreen(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, string secureText, char maskChar, TextEditor editor)
		{
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.MouseDown)
			{
				if (type == EventType.Repaint)
				{
					if (editor.keyboardOnScreen != null)
					{
						content.text = editor.keyboardOnScreen.text;
						if (maxLength >= 0 && content.text.Length > maxLength)
						{
							content.text = content.text.Substring(0, maxLength);
						}
						if (editor.keyboardOnScreen.status != TouchScreenKeyboard.Status.Visible)
						{
							editor.keyboardOnScreen = null;
							GUI.changed = true;
						}
					}
					string text = content.text;
					if (secureText != null)
					{
						content.text = GUI.PasswordFieldGetStrToShow(text, maskChar);
					}
					style.Draw(position, content, id, false);
					content.text = text;
				}
			}
			else if (position.Contains(current.mousePosition))
			{
				GUIUtility.hotControl = id;
				if (GUI.s_HotTextField != -1 && GUI.s_HotTextField != id)
				{
					TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUI.s_HotTextField);
					textEditor.keyboardOnScreen = null;
				}
				GUI.s_HotTextField = id;
				if (GUIUtility.keyboardControl != id)
				{
					GUIUtility.keyboardControl = id;
				}
				editor.keyboardOnScreen = TouchScreenKeyboard.Open(secureText ?? content.text, TouchScreenKeyboardType.Default, true, multiline, secureText != null);
				current.Use();
			}
		}

		private static void HandleTextFieldEventForDesktop(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, TextEditor editor)
		{
			Event current = Event.current;
			bool flag = false;
			switch (current.type)
			{
			case EventType.MouseDown:
				if (position.Contains(current.mousePosition))
				{
					GUIUtility.hotControl = id;
					GUIUtility.keyboardControl = id;
					editor.m_HasFocus = true;
					editor.MoveCursorToPosition(Event.current.mousePosition);
					if (Event.current.clickCount == 2 && GUI.skin.settings.doubleClickSelectsWord)
					{
						editor.SelectCurrentWord();
						editor.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
						editor.MouseDragSelectsWholeWords(true);
					}
					if (Event.current.clickCount == 3 && GUI.skin.settings.tripleClickSelectsLine)
					{
						editor.SelectCurrentParagraph();
						editor.MouseDragSelectsWholeWords(true);
						editor.DblClickSnap(TextEditor.DblClickSnapping.PARAGRAPHS);
					}
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == id)
				{
					editor.MouseDragSelectsWholeWords(false);
					GUIUtility.hotControl = 0;
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == id)
				{
					if (current.shift)
					{
						editor.MoveCursorToPosition(Event.current.mousePosition);
					}
					else
					{
						editor.SelectToPosition(Event.current.mousePosition);
					}
					current.Use();
				}
				break;
			case EventType.KeyDown:
				if (GUIUtility.keyboardControl != id)
				{
					return;
				}
				if (editor.HandleKeyEvent(current))
				{
					current.Use();
					flag = true;
					content.text = editor.text;
				}
				else
				{
					if (current.keyCode == KeyCode.Tab || current.character == '\t')
					{
						return;
					}
					char character = current.character;
					if (character == '\n' && !multiline && !current.alt)
					{
						return;
					}
					Font font = style.font;
					if (!font)
					{
						font = GUI.skin.font;
					}
					if (font.HasCharacter(character) || character == '\n')
					{
						editor.Insert(character);
						flag = true;
					}
					else if (character == '\0')
					{
						if (Input.compositionString.Length > 0)
						{
							editor.ReplaceSelection("");
							flag = true;
						}
						current.Use();
					}
				}
				break;
			case EventType.Repaint:
				if (GUIUtility.keyboardControl != id)
				{
					style.Draw(position, content, id, false);
				}
				else
				{
					editor.DrawCursor(content.text);
				}
				break;
			}
			if (GUIUtility.keyboardControl == id)
			{
				GUIUtility.textFieldInput = true;
			}
			if (flag)
			{
				GUI.changed = true;
				content.text = editor.text;
				if (maxLength >= 0 && content.text.Length > maxLength)
				{
					content.text = content.text.Substring(0, maxLength);
				}
				current.Use();
			}
		}

		public static bool Toggle(Rect position, bool value, string text)
		{
			return GUI.Toggle(position, value, GUIContent.Temp(text), GUI.s_Skin.toggle);
		}

		public static bool Toggle(Rect position, bool value, Texture image)
		{
			return GUI.Toggle(position, value, GUIContent.Temp(image), GUI.s_Skin.toggle);
		}

		public static bool Toggle(Rect position, bool value, GUIContent content)
		{
			return GUI.Toggle(position, value, content, GUI.s_Skin.toggle);
		}

		public static bool Toggle(Rect position, bool value, string text, GUIStyle style)
		{
			return GUI.Toggle(position, value, GUIContent.Temp(text), style);
		}

		public static bool Toggle(Rect position, bool value, Texture image, GUIStyle style)
		{
			return GUI.Toggle(position, value, GUIContent.Temp(image), style);
		}

		public static bool Toggle(Rect position, bool value, GUIContent content, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoToggle(position, GUIUtility.GetControlID(GUI.s_ToggleHash, FocusType.Passive, position), value, content, style);
		}

		public static bool Toggle(Rect position, int id, bool value, GUIContent content, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoToggle(position, id, value, content, style);
		}

		public static int Toolbar(Rect position, int selected, string[] texts)
		{
			return GUI.Toolbar(position, selected, GUIContent.Temp(texts), GUI.s_Skin.button);
		}

		public static int Toolbar(Rect position, int selected, Texture[] images)
		{
			return GUI.Toolbar(position, selected, GUIContent.Temp(images), GUI.s_Skin.button);
		}

		public static int Toolbar(Rect position, int selected, GUIContent[] contents)
		{
			return GUI.Toolbar(position, selected, contents, GUI.s_Skin.button);
		}

		public static int Toolbar(Rect position, int selected, string[] texts, GUIStyle style)
		{
			return GUI.Toolbar(position, selected, GUIContent.Temp(texts), style);
		}

		public static int Toolbar(Rect position, int selected, Texture[] images, GUIStyle style)
		{
			return GUI.Toolbar(position, selected, GUIContent.Temp(images), style);
		}

		public static int Toolbar(Rect position, int selected, GUIContent[] contents, GUIStyle style)
		{
			return GUI.Toolbar(position, selected, contents, null, style, GUI.ToolbarButtonSize.Fixed);
		}

		public static int Toolbar(Rect position, int selected, GUIContent[] contents, GUIStyle style, GUI.ToolbarButtonSize buttonSize)
		{
			return GUI.Toolbar(position, selected, contents, null, style, buttonSize);
		}

		internal static int Toolbar(Rect position, int selected, GUIContent[] contents, string[] controlNames, GUIStyle style, GUI.ToolbarButtonSize buttonSize)
		{
			GUIUtility.CheckOnGUI();
			GUIStyle guistyle;
			GUIStyle guistyle2;
			GUIStyle guistyle3;
			GUI.FindStyles(ref style, out guistyle, out guistyle2, out guistyle3, "left", "mid", "right");
			return GUI.DoButtonGrid(position, selected, contents, controlNames, contents.Length, style, guistyle, guistyle2, guistyle3, buttonSize);
		}

		public static int SelectionGrid(Rect position, int selected, string[] texts, int xCount)
		{
			return GUI.SelectionGrid(position, selected, GUIContent.Temp(texts), xCount, null);
		}

		public static int SelectionGrid(Rect position, int selected, Texture[] images, int xCount)
		{
			return GUI.SelectionGrid(position, selected, GUIContent.Temp(images), xCount, null);
		}

		public static int SelectionGrid(Rect position, int selected, GUIContent[] content, int xCount)
		{
			return GUI.SelectionGrid(position, selected, content, xCount, null);
		}

		public static int SelectionGrid(Rect position, int selected, string[] texts, int xCount, GUIStyle style)
		{
			return GUI.SelectionGrid(position, selected, GUIContent.Temp(texts), xCount, style);
		}

		public static int SelectionGrid(Rect position, int selected, Texture[] images, int xCount, GUIStyle style)
		{
			return GUI.SelectionGrid(position, selected, GUIContent.Temp(images), xCount, style);
		}

		public static int SelectionGrid(Rect position, int selected, GUIContent[] contents, int xCount, GUIStyle style)
		{
			if (style == null)
			{
				style = GUI.s_Skin.button;
			}
			return GUI.DoButtonGrid(position, selected, contents, null, xCount, style, style, style, style, GUI.ToolbarButtonSize.Fixed);
		}

		internal static void FindStyles(ref GUIStyle style, out GUIStyle firstStyle, out GUIStyle midStyle, out GUIStyle lastStyle, string first, string mid, string last)
		{
			if (style == null)
			{
				style = GUI.skin.button;
			}
			string name = style.name;
			midStyle = GUI.skin.FindStyle(name + mid) ?? style;
			firstStyle = GUI.skin.FindStyle(name + first) ?? midStyle;
			lastStyle = GUI.skin.FindStyle(name + last) ?? midStyle;
		}

		internal static int CalcTotalHorizSpacing(int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle)
		{
			int num;
			if (xCount < 2)
			{
				num = 0;
			}
			else if (xCount == 2)
			{
				num = Mathf.Max(firstStyle.margin.right, lastStyle.margin.left);
			}
			else
			{
				int num2 = Mathf.Max(midStyle.margin.left, midStyle.margin.right);
				num = Mathf.Max(firstStyle.margin.right, midStyle.margin.left) + Mathf.Max(midStyle.margin.right, lastStyle.margin.left) + num2 * (xCount - 3);
			}
			return num;
		}

		private static bool DoControl(Rect position, int id, bool on, GUIContent content, GUIStyle style)
		{
			Event current = Event.current;
			switch (current.type)
			{
			case EventType.MouseDown:
				if (position.Contains(current.mousePosition))
				{
					GUI.GrabMouseControl(id);
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUI.HasMouseControl(id))
				{
					GUI.ReleaseMouseControl();
					current.Use();
					if (position.Contains(current.mousePosition))
					{
						GUI.changed = true;
						return !on;
					}
				}
				break;
			case EventType.MouseDrag:
				if (GUI.HasMouseControl(id))
				{
					current.Use();
				}
				break;
			case EventType.KeyDown:
				if (current.character == ' ' && GUIUtility.keyboardControl == id)
				{
					current.Use();
					GUI.changed = true;
					return !on;
				}
				break;
			case EventType.Repaint:
				style.Draw(position, content, id, on);
				break;
			}
			return on;
		}

		private static void DoLabel(Rect position, GUIContent content, GUIStyle style)
		{
			Event current = Event.current;
			if (current.type == EventType.Repaint)
			{
				style.Draw(position, content, false, false, false, false);
				if (!string.IsNullOrEmpty(content.tooltip) && position.Contains(current.mousePosition) && GUIClip.visibleRect.Contains(current.mousePosition))
				{
					GUIStyle.SetMouseTooltip(content.tooltip, position);
				}
			}
		}

		internal static bool DoToggle(Rect position, int id, bool value, GUIContent content, GUIStyle style)
		{
			return GUI.DoControl(position, id, value, content, style);
		}

		internal static bool DoButton(Rect position, int id, GUIContent content, GUIStyle style)
		{
			return GUI.DoControl(position, id, false, content, style);
		}

		private static int DoButtonGrid(Rect position, int selected, GUIContent[] contents, string[] controlNames, int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle, GUI.ToolbarButtonSize buttonSize)
		{
			GUIUtility.CheckOnGUI();
			int num = contents.Length;
			int num2;
			if (num == 0)
			{
				num2 = selected;
			}
			else if (xCount <= 0)
			{
				Debug.LogWarning("You are trying to create a SelectionGrid with zero or less elements to be displayed in the horizontal direction. Set xCount to a positive value.");
				num2 = selected;
			}
			else
			{
				int num3 = num / xCount;
				if (num % xCount != 0)
				{
					num3++;
				}
				float num4 = (float)GUI.CalcTotalHorizSpacing(xCount, style, firstStyle, midStyle, lastStyle);
				float num5 = (float)(Mathf.Max(style.margin.top, style.margin.bottom) * (num3 - 1));
				float num6 = (position.width - num4) / (float)xCount;
				float num7 = (position.height - num5) / (float)num3;
				if (style.fixedWidth != 0f)
				{
					num6 = style.fixedWidth;
				}
				if (style.fixedHeight != 0f)
				{
					num7 = style.fixedHeight;
				}
				Rect[] array = GUI.CalcMouseRects(position, contents, xCount, num6, num7, style, firstStyle, midStyle, lastStyle, false, buttonSize);
				GUIStyle guistyle = null;
				int num8 = 0;
				for (int i = 0; i < num; i++)
				{
					Rect rect = array[i];
					GUIContent guicontent = contents[i];
					if (controlNames != null)
					{
						GUI.SetNextControlName(controlNames[i]);
					}
					int controlID = GUIUtility.GetControlID(GUI.s_ButtonGridHash, FocusType.Passive, rect);
					if (i == selected)
					{
						num8 = controlID;
					}
					switch (Event.current.GetTypeForControl(controlID))
					{
					case EventType.MouseDown:
						if (rect.Contains(Event.current.mousePosition))
						{
							GUIUtility.hotControl = controlID;
							Event.current.Use();
						}
						break;
					case EventType.MouseUp:
						if (GUIUtility.hotControl == controlID)
						{
							GUIUtility.hotControl = 0;
							Event.current.Use();
							GUI.changed = true;
							return i;
						}
						break;
					case EventType.MouseDrag:
						if (GUIUtility.hotControl == controlID)
						{
							Event.current.Use();
						}
						break;
					case EventType.Repaint:
					{
						GUIStyle guistyle2 = ((num != 1) ? ((i != 0) ? ((i != num - 1) ? midStyle : lastStyle) : firstStyle) : style);
						bool flag = rect.Contains(Event.current.mousePosition);
						bool flag2 = GUIUtility.hotControl == controlID;
						if (selected != i)
						{
							guistyle2.Draw(rect, guicontent, flag && (GUI.enabled || flag2) && (flag2 || GUIUtility.hotControl == 0), GUI.enabled && flag2, false, false);
						}
						else
						{
							guistyle = guistyle2;
						}
						if (flag)
						{
							GUIUtility.mouseUsed = true;
							if (!string.IsNullOrEmpty(guicontent.tooltip))
							{
								GUIStyle.SetMouseTooltip(guicontent.tooltip, rect);
							}
						}
						break;
					}
					}
				}
				if (guistyle != null)
				{
					Rect rect2 = array[selected];
					GUIContent guicontent2 = contents[selected];
					bool flag3 = rect2.Contains(Event.current.mousePosition);
					bool flag4 = GUIUtility.hotControl == num8;
					guistyle.Draw(rect2, guicontent2, flag3 && (GUI.enabled || flag4) && (flag4 || GUIUtility.hotControl == 0), GUI.enabled && flag4, true, false);
				}
				num2 = selected;
			}
			return num2;
		}

		private static Rect[] CalcMouseRects(Rect position, GUIContent[] contents, int xCount, float elemWidth, float elemHeight, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle, bool addBorders, GUI.ToolbarButtonSize buttonSize)
		{
			int num = contents.Length;
			int num2 = 0;
			float num3 = position.xMin;
			float num4 = position.yMin;
			GUIStyle guistyle = style;
			Rect[] array = new Rect[num];
			if (num > 1)
			{
				guistyle = firstStyle;
			}
			for (int i = 0; i < num; i++)
			{
				float num5 = 0f;
				if (buttonSize != GUI.ToolbarButtonSize.Fixed)
				{
					if (buttonSize == GUI.ToolbarButtonSize.FitToContents)
					{
						num5 = guistyle.CalcSize(contents[i]).x;
					}
				}
				else
				{
					num5 = elemWidth;
				}
				if (!addBorders)
				{
					array[i] = new Rect(num3, num4, num5, elemHeight);
				}
				else
				{
					array[i] = guistyle.margin.Add(new Rect(num3, num4, num5, elemHeight));
				}
				array[i] = GUIUtility.AlignRectToDevice(array[i]);
				GUIStyle guistyle2 = midStyle;
				if (i == num - 2 || i == xCount - 2)
				{
					guistyle2 = lastStyle;
				}
				num3 = array[i].xMax + (float)Mathf.Max(guistyle.margin.right, guistyle2.margin.left);
				num2++;
				if (num2 >= xCount)
				{
					num2 = 0;
					num4 += elemHeight + (float)Mathf.Max(style.margin.top, style.margin.bottom);
					num3 = position.xMin;
					guistyle2 = firstStyle;
				}
				guistyle = guistyle2;
			}
			return array;
		}

		public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue)
		{
			return GUI.Slider(position, value, 0f, leftValue, rightValue, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb, true, 0);
		}

		public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb)
		{
			return GUI.Slider(position, value, 0f, leftValue, rightValue, slider, thumb, true, 0);
		}

		public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue)
		{
			return GUI.Slider(position, value, 0f, topValue, bottomValue, GUI.skin.verticalSlider, GUI.skin.verticalSliderThumb, false, 0);
		}

		public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue, GUIStyle slider, GUIStyle thumb)
		{
			return GUI.Slider(position, value, 0f, topValue, bottomValue, slider, thumb, false, 0);
		}

		public static float Slider(Rect position, float value, float size, float start, float end, GUIStyle slider, GUIStyle thumb, bool horiz, int id)
		{
			GUIUtility.CheckOnGUI();
			if (id == 0)
			{
				id = GUIUtility.GetControlID(GUI.s_SliderHash, FocusType.Passive, position);
			}
			SliderHandler sliderHandler = new SliderHandler(position, value, size, start, end, slider, thumb, horiz, id);
			return sliderHandler.Handle();
		}

		public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue)
		{
			return GUI.Scroller(position, value, size, leftValue, rightValue, GUI.skin.horizontalScrollbar, GUI.skin.horizontalScrollbarThumb, GUI.skin.horizontalScrollbarLeftButton, GUI.skin.horizontalScrollbarRightButton, true);
		}

		public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle style)
		{
			return GUI.Scroller(position, value, size, leftValue, rightValue, style, GUI.skin.GetStyle(style.name + "thumb"), GUI.skin.GetStyle(style.name + "leftbutton"), GUI.skin.GetStyle(style.name + "rightbutton"), true);
		}

		internal static bool ScrollerRepeatButton(int scrollerID, Rect rect, GUIStyle style)
		{
			bool flag = false;
			if (GUI.DoRepeatButton(rect, GUIContent.none, style, FocusType.Passive))
			{
				bool flag2 = GUI.s_ScrollControlId != scrollerID;
				GUI.s_ScrollControlId = scrollerID;
				if (flag2)
				{
					flag = true;
					GUI.nextScrollStepTime = DateTime.Now.AddMilliseconds(250.0);
				}
				else if (DateTime.Now >= GUI.nextScrollStepTime)
				{
					flag = true;
					GUI.nextScrollStepTime = DateTime.Now.AddMilliseconds(30.0);
				}
				if (Event.current.type == EventType.Repaint)
				{
					GUI.InternalRepaintEditorWindow();
				}
			}
			return flag;
		}

		public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue)
		{
			return GUI.Scroller(position, value, size, topValue, bottomValue, GUI.skin.verticalScrollbar, GUI.skin.verticalScrollbarThumb, GUI.skin.verticalScrollbarUpButton, GUI.skin.verticalScrollbarDownButton, false);
		}

		public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue, GUIStyle style)
		{
			return GUI.Scroller(position, value, size, topValue, bottomValue, style, GUI.skin.GetStyle(style.name + "thumb"), GUI.skin.GetStyle(style.name + "upbutton"), GUI.skin.GetStyle(style.name + "downbutton"), false);
		}

		internal static float Scroller(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUIStyle leftButton, GUIStyle rightButton, bool horiz)
		{
			GUIUtility.CheckOnGUI();
			int controlID = GUIUtility.GetControlID(GUI.s_SliderHash, FocusType.Passive, position);
			Rect rect;
			Rect rect2;
			Rect rect3;
			if (horiz)
			{
				rect = new Rect(position.x + leftButton.fixedWidth, position.y, position.width - leftButton.fixedWidth - rightButton.fixedWidth, position.height);
				rect2 = new Rect(position.x, position.y, leftButton.fixedWidth, position.height);
				rect3 = new Rect(position.xMax - rightButton.fixedWidth, position.y, rightButton.fixedWidth, position.height);
			}
			else
			{
				rect = new Rect(position.x, position.y + leftButton.fixedHeight, position.width, position.height - leftButton.fixedHeight - rightButton.fixedHeight);
				rect2 = new Rect(position.x, position.y, position.width, leftButton.fixedHeight);
				rect3 = new Rect(position.x, position.yMax - rightButton.fixedHeight, position.width, rightButton.fixedHeight);
			}
			value = GUI.Slider(rect, value, size, leftValue, rightValue, slider, thumb, horiz, controlID);
			bool flag = Event.current.type == EventType.MouseUp;
			if (GUI.ScrollerRepeatButton(controlID, rect2, leftButton))
			{
				value -= 10f * ((leftValue >= rightValue) ? (-1f) : 1f);
			}
			if (GUI.ScrollerRepeatButton(controlID, rect3, rightButton))
			{
				value += 10f * ((leftValue >= rightValue) ? (-1f) : 1f);
			}
			if (flag && Event.current.type == EventType.Used)
			{
				GUI.s_ScrollControlId = 0;
			}
			if (leftValue < rightValue)
			{
				value = Mathf.Clamp(value, leftValue, rightValue - size);
			}
			else
			{
				value = Mathf.Clamp(value, rightValue, leftValue - size);
			}
			return value;
		}

		public static void BeginClip(Rect position, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
		{
			GUIUtility.CheckOnGUI();
			GUIClip.Push(position, scrollOffset, renderOffset, resetOffset);
		}

		public static void BeginGroup(Rect position)
		{
			GUI.BeginGroup(position, GUIContent.none, GUIStyle.none);
		}

		public static void BeginGroup(Rect position, string text)
		{
			GUI.BeginGroup(position, GUIContent.Temp(text), GUIStyle.none);
		}

		public static void BeginGroup(Rect position, Texture image)
		{
			GUI.BeginGroup(position, GUIContent.Temp(image), GUIStyle.none);
		}

		public static void BeginGroup(Rect position, GUIContent content)
		{
			GUI.BeginGroup(position, content, GUIStyle.none);
		}

		public static void BeginGroup(Rect position, GUIStyle style)
		{
			GUI.BeginGroup(position, GUIContent.none, style);
		}

		public static void BeginGroup(Rect position, string text, GUIStyle style)
		{
			GUI.BeginGroup(position, GUIContent.Temp(text), style);
		}

		public static void BeginGroup(Rect position, Texture image, GUIStyle style)
		{
			GUI.BeginGroup(position, GUIContent.Temp(image), style);
		}

		public static void BeginGroup(Rect position, GUIContent content, GUIStyle style)
		{
			GUI.BeginGroup(position, content, style, Vector2.zero);
		}

		internal static void BeginGroup(Rect position, GUIContent content, GUIStyle style, Vector2 scrollOffset)
		{
			GUIUtility.CheckOnGUI();
			int controlID = GUIUtility.GetControlID(GUI.s_BeginGroupHash, FocusType.Passive);
			if (content != GUIContent.none || style != GUIStyle.none)
			{
				EventType type = Event.current.type;
				if (type != EventType.Repaint)
				{
					if (position.Contains(Event.current.mousePosition))
					{
						GUIUtility.mouseUsed = true;
					}
				}
				else
				{
					style.Draw(position, content, controlID);
				}
			}
			GUIClip.Push(position, scrollOffset, Vector2.zero, false);
		}

		public static void EndGroup()
		{
			GUIUtility.CheckOnGUI();
			GUIClip.Internal_Pop();
		}

		public static void BeginClip(Rect position)
		{
			GUIUtility.CheckOnGUI();
			GUIClip.Push(position, Vector2.zero, Vector2.zero, false);
		}

		public static void EndClip()
		{
			GUIUtility.CheckOnGUI();
			GUIClip.Pop();
		}

		public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect)
		{
			return GUI.BeginScrollView(position, scrollPosition, viewRect, false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView);
		}

		public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical)
		{
			return GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView);
		}

		public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
		{
			return GUI.BeginScrollView(position, scrollPosition, viewRect, false, false, horizontalScrollbar, verticalScrollbar, GUI.skin.scrollView);
		}

		public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
		{
			return GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, GUI.skin.scrollView);
		}

		protected static Vector2 DoBeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
		{
			return GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);
		}

		internal static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
		{
			GUIUtility.CheckOnGUI();
			int controlID = GUIUtility.GetControlID(GUI.s_ScrollviewHash, FocusType.Passive);
			ScrollViewState scrollViewState = (ScrollViewState)GUIUtility.GetStateObject(typeof(ScrollViewState), controlID);
			if (scrollViewState.apply)
			{
				scrollPosition = scrollViewState.scrollPosition;
				scrollViewState.apply = false;
			}
			scrollViewState.position = position;
			scrollViewState.scrollPosition = scrollPosition;
			scrollViewState.visibleRect = (scrollViewState.viewRect = viewRect);
			scrollViewState.visibleRect.width = position.width;
			scrollViewState.visibleRect.height = position.height;
			GUI.s_ScrollViewStates.Push(scrollViewState);
			Rect rect = new Rect(position);
			EventType type = Event.current.type;
			if (type != EventType.Layout)
			{
				if (type != EventType.Used)
				{
					bool flag = alwaysShowVertical;
					bool flag2 = alwaysShowHorizontal;
					if (flag2 || viewRect.width > rect.width)
					{
						scrollViewState.visibleRect.height = position.height - horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
						rect.height -= horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
						flag2 = true;
					}
					if (flag || viewRect.height > rect.height)
					{
						scrollViewState.visibleRect.width = position.width - verticalScrollbar.fixedWidth + (float)verticalScrollbar.margin.left;
						rect.width -= verticalScrollbar.fixedWidth + (float)verticalScrollbar.margin.left;
						flag = true;
						if (!flag2 && viewRect.width > rect.width)
						{
							scrollViewState.visibleRect.height = position.height - horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
							rect.height -= horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
							flag2 = true;
						}
					}
					if (Event.current.type == EventType.Repaint && background != GUIStyle.none)
					{
						background.Draw(position, position.Contains(Event.current.mousePosition), false, flag2 && flag, false);
					}
					if (flag2 && horizontalScrollbar != GUIStyle.none)
					{
						scrollPosition.x = GUI.HorizontalScrollbar(new Rect(position.x, position.yMax - horizontalScrollbar.fixedHeight, rect.width, horizontalScrollbar.fixedHeight), scrollPosition.x, Mathf.Min(rect.width, viewRect.width), 0f, viewRect.width, horizontalScrollbar);
					}
					else
					{
						GUIUtility.GetControlID(GUI.s_SliderHash, FocusType.Passive);
						GUIUtility.GetControlID(GUI.s_RepeatButtonHash, FocusType.Passive);
						GUIUtility.GetControlID(GUI.s_RepeatButtonHash, FocusType.Passive);
						scrollPosition.x = ((horizontalScrollbar == GUIStyle.none) ? Mathf.Clamp(scrollPosition.x, 0f, Mathf.Max(viewRect.width - position.width, 0f)) : 0f);
					}
					if (flag && verticalScrollbar != GUIStyle.none)
					{
						scrollPosition.y = GUI.VerticalScrollbar(new Rect(rect.xMax + (float)verticalScrollbar.margin.left, rect.y, verticalScrollbar.fixedWidth, rect.height), scrollPosition.y, Mathf.Min(rect.height, viewRect.height), 0f, viewRect.height, verticalScrollbar);
					}
					else
					{
						GUIUtility.GetControlID(GUI.s_SliderHash, FocusType.Passive);
						GUIUtility.GetControlID(GUI.s_RepeatButtonHash, FocusType.Passive);
						GUIUtility.GetControlID(GUI.s_RepeatButtonHash, FocusType.Passive);
						scrollPosition.y = ((verticalScrollbar == GUIStyle.none) ? Mathf.Clamp(scrollPosition.y, 0f, Mathf.Max(viewRect.height - position.height, 0f)) : 0f);
					}
				}
			}
			else
			{
				GUIUtility.GetControlID(GUI.s_SliderHash, FocusType.Passive);
				GUIUtility.GetControlID(GUI.s_RepeatButtonHash, FocusType.Passive);
				GUIUtility.GetControlID(GUI.s_RepeatButtonHash, FocusType.Passive);
				GUIUtility.GetControlID(GUI.s_SliderHash, FocusType.Passive);
				GUIUtility.GetControlID(GUI.s_RepeatButtonHash, FocusType.Passive);
				GUIUtility.GetControlID(GUI.s_RepeatButtonHash, FocusType.Passive);
			}
			GUIClip.Push(rect, new Vector2(Mathf.Round(-scrollPosition.x - viewRect.x), Mathf.Round(-scrollPosition.y - viewRect.y)), Vector2.zero, false);
			return scrollPosition;
		}

		public static void EndScrollView()
		{
			GUI.EndScrollView(true);
		}

		public static void EndScrollView(bool handleScrollWheel)
		{
			GUIUtility.CheckOnGUI();
			ScrollViewState scrollViewState = (ScrollViewState)GUI.s_ScrollViewStates.Peek();
			GUIClip.Pop();
			GUI.s_ScrollViewStates.Pop();
			if (handleScrollWheel && Event.current.type == EventType.ScrollWheel && scrollViewState.position.Contains(Event.current.mousePosition))
			{
				scrollViewState.scrollPosition.x = Mathf.Clamp(scrollViewState.scrollPosition.x + Event.current.delta.x * 20f, 0f, scrollViewState.viewRect.width - scrollViewState.visibleRect.width);
				scrollViewState.scrollPosition.y = Mathf.Clamp(scrollViewState.scrollPosition.y + Event.current.delta.y * 20f, 0f, scrollViewState.viewRect.height - scrollViewState.visibleRect.height);
				if (scrollViewState.scrollPosition.x < 0f)
				{
					scrollViewState.scrollPosition.x = 0f;
				}
				if (scrollViewState.scrollPosition.y < 0f)
				{
					scrollViewState.scrollPosition.y = 0f;
				}
				scrollViewState.apply = true;
				Event.current.Use();
			}
		}

		internal static ScrollViewState GetTopScrollView()
		{
			ScrollViewState scrollViewState;
			if (GUI.s_ScrollViewStates.Count != 0)
			{
				scrollViewState = (ScrollViewState)GUI.s_ScrollViewStates.Peek();
			}
			else
			{
				scrollViewState = null;
			}
			return scrollViewState;
		}

		public static void ScrollTo(Rect position)
		{
			ScrollViewState topScrollView = GUI.GetTopScrollView();
			if (topScrollView != null)
			{
				topScrollView.ScrollTo(position);
			}
		}

		public static bool ScrollTowards(Rect position, float maxDelta)
		{
			ScrollViewState topScrollView = GUI.GetTopScrollView();
			return topScrollView != null && topScrollView.ScrollTowards(position, maxDelta);
		}

		public static Rect Window(int id, Rect clientRect, GUI.WindowFunction func, string text)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoWindow(id, clientRect, func, GUIContent.Temp(text), GUI.skin.window, GUI.skin, true);
		}

		public static Rect Window(int id, Rect clientRect, GUI.WindowFunction func, Texture image)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoWindow(id, clientRect, func, GUIContent.Temp(image), GUI.skin.window, GUI.skin, true);
		}

		public static Rect Window(int id, Rect clientRect, GUI.WindowFunction func, GUIContent content)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoWindow(id, clientRect, func, content, GUI.skin.window, GUI.skin, true);
		}

		public static Rect Window(int id, Rect clientRect, GUI.WindowFunction func, string text, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoWindow(id, clientRect, func, GUIContent.Temp(text), style, GUI.skin, true);
		}

		public static Rect Window(int id, Rect clientRect, GUI.WindowFunction func, Texture image, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoWindow(id, clientRect, func, GUIContent.Temp(image), style, GUI.skin, true);
		}

		public static Rect Window(int id, Rect clientRect, GUI.WindowFunction func, GUIContent title, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoWindow(id, clientRect, func, title, style, GUI.skin, true);
		}

		public static Rect ModalWindow(int id, Rect clientRect, GUI.WindowFunction func, string text)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoModalWindow(id, clientRect, func, GUIContent.Temp(text), GUI.skin.window, GUI.skin);
		}

		public static Rect ModalWindow(int id, Rect clientRect, GUI.WindowFunction func, Texture image)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoModalWindow(id, clientRect, func, GUIContent.Temp(image), GUI.skin.window, GUI.skin);
		}

		public static Rect ModalWindow(int id, Rect clientRect, GUI.WindowFunction func, GUIContent content)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoModalWindow(id, clientRect, func, content, GUI.skin.window, GUI.skin);
		}

		public static Rect ModalWindow(int id, Rect clientRect, GUI.WindowFunction func, string text, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoModalWindow(id, clientRect, func, GUIContent.Temp(text), style, GUI.skin);
		}

		public static Rect ModalWindow(int id, Rect clientRect, GUI.WindowFunction func, Texture image, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoModalWindow(id, clientRect, func, GUIContent.Temp(image), style, GUI.skin);
		}

		public static Rect ModalWindow(int id, Rect clientRect, GUI.WindowFunction func, GUIContent content, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			return GUI.DoModalWindow(id, clientRect, func, content, style, GUI.skin);
		}

		private static Rect DoWindow(int id, Rect clientRect, GUI.WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout)
		{
			return GUI.Internal_DoWindow(id, GUIUtility.s_OriginalID, clientRect, func, title, style, skin, forceRectOnLayout);
		}

		private static Rect DoModalWindow(int id, Rect clientRect, GUI.WindowFunction func, GUIContent content, GUIStyle style, GUISkin skin)
		{
			return GUI.Internal_DoModalWindow(id, GUIUtility.s_OriginalID, clientRect, func, content, style, skin);
		}

		[RequiredByNativeCode]
		internal static void CallWindowDelegate(GUI.WindowFunction func, int id, int instanceID, GUISkin _skin, int forceRect, float width, float height, GUIStyle style)
		{
			GUILayoutUtility.SelectIDList(id, true);
			GUISkin skin = GUI.skin;
			if (Event.current.type == EventType.Layout)
			{
				if (forceRect != 0)
				{
					GUILayoutOption[] array = new GUILayoutOption[]
					{
						GUILayout.Width(width),
						GUILayout.Height(height)
					};
					GUILayoutUtility.BeginWindow(id, style, array);
				}
				else
				{
					GUILayoutUtility.BeginWindow(id, style, null);
				}
			}
			else
			{
				GUILayoutUtility.BeginWindow(id, GUIStyle.none, null);
			}
			GUI.skin = _skin;
			func(id);
			if (Event.current.type == EventType.Layout)
			{
				GUILayoutUtility.Layout();
			}
			GUI.skin = skin;
		}

		public static void DragWindow()
		{
			GUI.DragWindow(new Rect(0f, 0f, 10000f, 10000f));
		}

		internal static void BeginWindows(int skinMode, int editorWindowInstanceID)
		{
			GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
			GenericStack layoutGroups = GUILayoutUtility.current.layoutGroups;
			GUILayoutGroup windows = GUILayoutUtility.current.windows;
			Matrix4x4 matrix = GUI.matrix;
			GUI.Internal_BeginWindows();
			GUI.matrix = matrix;
			GUILayoutUtility.current.topLevel = topLevel;
			GUILayoutUtility.current.layoutGroups = layoutGroups;
			GUILayoutUtility.current.windows = windows;
		}

		internal static void EndWindows()
		{
			GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
			GenericStack layoutGroups = GUILayoutUtility.current.layoutGroups;
			GUILayoutGroup windows = GUILayoutUtility.current.windows;
			GUI.Internal_EndWindows();
			GUILayoutUtility.current.topLevel = topLevel;
			GUILayoutUtility.current.layoutGroups = layoutGroups;
			GUILayoutUtility.current.windows = windows;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_color_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_color_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_backgroundColor_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_backgroundColor_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_contentColor_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_contentColor_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DoModalWindow_Injected(int id, int instanceID, ref Rect clientRect, GUI.WindowFunction func, GUIContent content, GUIStyle style, object skin, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DoWindow_Injected(int id, int instanceID, ref Rect clientRect, GUI.WindowFunction func, GUIContent title, GUIStyle style, object skin, bool forceRectOnLayout, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DragWindow_Injected(ref Rect position);

		private const float s_ScrollStepSize = 10f;

		private static int s_ScrollControlId;

		private static int s_HotTextField = -1;

		private static readonly int s_BoxHash = "Box".GetHashCode();

		private static readonly int s_ButonHash = "Button".GetHashCode();

		private static readonly int s_RepeatButtonHash = "repeatButton".GetHashCode();

		private static readonly int s_ToggleHash = "Toggle".GetHashCode();

		private static readonly int s_ButtonGridHash = "ButtonGrid".GetHashCode();

		private static readonly int s_SliderHash = "Slider".GetHashCode();

		private static readonly int s_BeginGroupHash = "BeginGroup".GetHashCode();

		private static readonly int s_ScrollviewHash = "scrollView".GetHashCode();

		private static GUISkin s_Skin;

		internal static Rect s_ToolTipRect;

		private static readonly GenericStack s_ScrollViewStates = new GenericStack();

		public enum ToolbarButtonSize
		{
			Fixed,
			FitToContents
		}

		public delegate void WindowFunction(int id);

		public abstract class Scope : IDisposable
		{
			protected abstract void CloseScope();

			~Scope()
			{
				if (!this.m_Disposed)
				{
					Debug.LogError("Scope was not disposed! You should use the 'using' keyword or manually call Dispose.");
				}
			}

			public void Dispose()
			{
				if (!this.m_Disposed)
				{
					this.m_Disposed = true;
					if (!GUIUtility.guiIsExiting)
					{
						this.CloseScope();
					}
				}
			}

			private bool m_Disposed;
		}

		public class GroupScope : GUI.Scope
		{
			public GroupScope(Rect position)
			{
				GUI.BeginGroup(position);
			}

			public GroupScope(Rect position, string text)
			{
				GUI.BeginGroup(position, text);
			}

			public GroupScope(Rect position, Texture image)
			{
				GUI.BeginGroup(position, image);
			}

			public GroupScope(Rect position, GUIContent content)
			{
				GUI.BeginGroup(position, content);
			}

			public GroupScope(Rect position, GUIStyle style)
			{
				GUI.BeginGroup(position, style);
			}

			public GroupScope(Rect position, string text, GUIStyle style)
			{
				GUI.BeginGroup(position, text, style);
			}

			public GroupScope(Rect position, Texture image, GUIStyle style)
			{
				GUI.BeginGroup(position, image, style);
			}

			protected override void CloseScope()
			{
				GUI.EndGroup();
			}
		}

		public class ScrollViewScope : GUI.Scope
		{
			public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect);
			}

			public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical);
			}

			public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, horizontalScrollbar, verticalScrollbar);
			}

			public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar);
			}

			internal ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);
			}

			public Vector2 scrollPosition { get; private set; }

			public bool handleScrollWheel { get; set; }

			protected override void CloseScope()
			{
				GUI.EndScrollView(this.handleScrollWheel);
			}
		}

		public class ClipScope : GUI.Scope
		{
			public ClipScope(Rect position)
			{
				GUI.BeginClip(position);
			}

			internal ClipScope(Rect position, Vector2 scrollOffset)
			{
				GUI.BeginClip(position, scrollOffset, default(Vector2), false);
			}

			protected override void CloseScope()
			{
				GUI.EndClip();
			}
		}

		internal struct ColorScope : IDisposable
		{
			public ColorScope(Color newColor)
			{
				this.m_Disposed = false;
				this.m_PreviousColor = GUI.color;
				GUI.color = newColor;
			}

			public ColorScope(float r, float g, float b, float a = 1f)
			{
				this = new GUI.ColorScope(new Color(r, g, b, a));
			}

			public void Dispose()
			{
				if (!this.m_Disposed)
				{
					this.m_Disposed = true;
					GUI.color = this.m_PreviousColor;
				}
			}

			private bool m_Disposed;

			private Color m_PreviousColor;
		}
	}
}
