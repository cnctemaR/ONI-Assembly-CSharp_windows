using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[NativeHeader("IMGUIScriptingClasses.h")]
	[NativeHeader("Modules/IMGUI/GUIStyle.bindings.h")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class GUIStyle
	{
		public GUIStyle()
		{
			this.m_Ptr = GUIStyle.Internal_Create(this);
		}

		public GUIStyle(GUIStyle other)
		{
			if (other == null)
			{
				Debug.LogError("Copied style is null. Using StyleNotFound instead.");
				other = GUISkin.error;
			}
			this.m_Ptr = GUIStyle.Internal_Copy(this, other);
		}

		[NativeProperty("Name", false, TargetType.Function)]
		public extern string name
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("Font", false, TargetType.Function)]
		public extern Font font
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ImagePosition", false, TargetType.Field)]
		public extern ImagePosition imagePosition
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_Alignment", false, TargetType.Field)]
		public extern TextAnchor alignment
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_WordWrap", false, TargetType.Field)]
		public extern bool wordWrap
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_Clipping", false, TargetType.Field)]
		public extern TextClipping clipping
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ContentOffset", false, TargetType.Field)]
		public Vector2 contentOffset
		{
			get
			{
				Vector2 vector;
				this.get_contentOffset_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_contentOffset_Injected(ref value);
			}
		}

		[NativeProperty("m_FixedWidth", false, TargetType.Field)]
		public extern float fixedWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_FixedHeight", false, TargetType.Field)]
		public extern float fixedHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_StretchWidth", false, TargetType.Field)]
		public extern bool stretchWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_StretchHeight", false, TargetType.Field)]
		public extern bool stretchHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_FontSize", false, TargetType.Field)]
		public extern int fontSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_FontStyle", false, TargetType.Field)]
		public extern FontStyle fontStyle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_RichText", false, TargetType.Field)]
		public extern bool richText
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("m_ClipOffset", false, TargetType.Field)]
		[Obsolete("Don't use clipOffset - put things inside BeginGroup instead. This functionality will be removed in a later version.", false)]
		public Vector2 clipOffset
		{
			get
			{
				Vector2 vector;
				this.get_clipOffset_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_clipOffset_Injected(ref value);
			}
		}

		[NativeProperty("m_ClipOffset", false, TargetType.Field)]
		internal Vector2 Internal_clipOffset
		{
			get
			{
				Vector2 vector;
				this.get_Internal_clipOffset_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_Internal_clipOffset_Injected(ref value);
			}
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_Create", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create(GUIStyle self);

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_Copy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Copy(GUIStyle self, GUIStyle other);

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_Destroy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr self);

		[FreeFunction(Name = "GUIStyle_Bindings::GetStyleStatePtr", IsThreadSafe = true, HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetStyleStatePtr(int idx);

		[FreeFunction(Name = "GUIStyle_Bindings::AssignStyleState", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AssignStyleState(int idx, IntPtr srcStyleState);

		[FreeFunction(Name = "GUIStyle_Bindings::GetRectOffsetPtr", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetRectOffsetPtr(int idx);

		[FreeFunction(Name = "GUIStyle_Bindings::AssignRectOffset", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AssignRectOffset(int idx, IntPtr srcRectOffset);

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_GetLineHeight")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float Internal_GetLineHeight(IntPtr target);

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_Draw", HasExplicitThis = true)]
		private void Internal_Draw(Rect screenRect, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
		{
			this.Internal_Draw_Injected(ref screenRect, content, isHover, isActive, on, hasKeyboardFocus);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_Draw2", HasExplicitThis = true)]
		private void Internal_Draw2(Rect position, GUIContent content, int controlID, bool on)
		{
			this.Internal_Draw2_Injected(ref position, content, controlID, on);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_DrawCursor", HasExplicitThis = true)]
		private void Internal_DrawCursor(Rect position, GUIContent content, int pos, Color cursorColor)
		{
			this.Internal_DrawCursor_Injected(ref position, content, pos, ref cursorColor);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_DrawWithTextSelection", HasExplicitThis = true)]
		private void Internal_DrawWithTextSelection(Rect screenRect, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus, bool drawSelectionAsComposition, int cursorFirst, int cursorLast, Color cursorColor, Color selectionColor)
		{
			this.Internal_DrawWithTextSelection_Injected(ref screenRect, content, isHover, isActive, on, hasKeyboardFocus, drawSelectionAsComposition, cursorFirst, cursorLast, ref cursorColor, ref selectionColor);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_GetCursorPixelPosition", HasExplicitThis = true)]
		internal Vector2 Internal_GetCursorPixelPosition(Rect position, GUIContent content, int cursorStringIndex)
		{
			Vector2 vector;
			this.Internal_GetCursorPixelPosition_Injected(ref position, content, cursorStringIndex, out vector);
			return vector;
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_GetCursorStringIndex", HasExplicitThis = true)]
		internal int Internal_GetCursorStringIndex(Rect position, GUIContent content, Vector2 cursorPixelPosition)
		{
			return this.Internal_GetCursorStringIndex_Injected(ref position, content, ref cursorPixelPosition);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_GetNumCharactersThatFitWithinWidth", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int Internal_GetNumCharactersThatFitWithinWidth(string text, float width);

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_CalcSize", HasExplicitThis = true)]
		internal Vector2 Internal_CalcSize(GUIContent content)
		{
			Vector2 vector;
			this.Internal_CalcSize_Injected(content, out vector);
			return vector;
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_CalcSizeWithConstraints", HasExplicitThis = true)]
		internal Vector2 Internal_CalcSizeWithConstraints(GUIContent content, Vector2 maxSize)
		{
			Vector2 vector;
			this.Internal_CalcSizeWithConstraints_Injected(content, ref maxSize, out vector);
			return vector;
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_CalcHeight", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float Internal_CalcHeight(GUIContent content, float width);

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_CalcMinMaxWidth", HasExplicitThis = true)]
		private Vector2 Internal_CalcMinMaxWidth(GUIContent content)
		{
			Vector2 vector;
			this.Internal_CalcMinMaxWidth_Injected(content, out vector);
			return vector;
		}

		[FreeFunction(Name = "GUIStyle_Bindings::SetMouseTooltip")]
		internal static void SetMouseTooltip(string tooltip, Rect screenRect)
		{
			GUIStyle.SetMouseTooltip_Injected(tooltip, ref screenRect);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_GetCursorFlashOffset")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float Internal_GetCursorFlashOffset();

		[FreeFunction(Name = "GUIStyle::SetDefaultFont")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetDefaultFont(Font font);

		~GUIStyle()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				GUIStyle.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		internal static void CleanupRoots()
		{
			GUIStyle.s_None = null;
		}

		internal void InternalOnAfterDeserialize()
		{
			this.m_Normal = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(0));
			this.m_Hover = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(1));
			this.m_Active = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(2));
			this.m_Focused = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(3));
			this.m_OnNormal = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(4));
			this.m_OnHover = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(5));
			this.m_OnActive = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(6));
			this.m_OnFocused = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(7));
		}

		public GUIStyleState normal
		{
			get
			{
				GUIStyleState guistyleState;
				if ((guistyleState = this.m_Normal) == null)
				{
					guistyleState = (this.m_Normal = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(0)));
				}
				return guistyleState;
			}
			set
			{
				this.AssignStyleState(0, value.m_Ptr);
			}
		}

		public GUIStyleState hover
		{
			get
			{
				GUIStyleState guistyleState;
				if ((guistyleState = this.m_Hover) == null)
				{
					guistyleState = (this.m_Hover = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(1)));
				}
				return guistyleState;
			}
			set
			{
				this.AssignStyleState(1, value.m_Ptr);
			}
		}

		public GUIStyleState active
		{
			get
			{
				GUIStyleState guistyleState;
				if ((guistyleState = this.m_Active) == null)
				{
					guistyleState = (this.m_Active = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(2)));
				}
				return guistyleState;
			}
			set
			{
				this.AssignStyleState(2, value.m_Ptr);
			}
		}

		public GUIStyleState onNormal
		{
			get
			{
				GUIStyleState guistyleState;
				if ((guistyleState = this.m_OnNormal) == null)
				{
					guistyleState = (this.m_OnNormal = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(4)));
				}
				return guistyleState;
			}
			set
			{
				this.AssignStyleState(4, value.m_Ptr);
			}
		}

		public GUIStyleState onHover
		{
			get
			{
				GUIStyleState guistyleState;
				if ((guistyleState = this.m_OnHover) == null)
				{
					guistyleState = (this.m_OnHover = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(5)));
				}
				return guistyleState;
			}
			set
			{
				this.AssignStyleState(5, value.m_Ptr);
			}
		}

		public GUIStyleState onActive
		{
			get
			{
				GUIStyleState guistyleState;
				if ((guistyleState = this.m_OnActive) == null)
				{
					guistyleState = (this.m_OnActive = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(6)));
				}
				return guistyleState;
			}
			set
			{
				this.AssignStyleState(6, value.m_Ptr);
			}
		}

		public GUIStyleState focused
		{
			get
			{
				GUIStyleState guistyleState;
				if ((guistyleState = this.m_Focused) == null)
				{
					guistyleState = (this.m_Focused = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(3)));
				}
				return guistyleState;
			}
			set
			{
				this.AssignStyleState(3, value.m_Ptr);
			}
		}

		public GUIStyleState onFocused
		{
			get
			{
				GUIStyleState guistyleState;
				if ((guistyleState = this.m_OnFocused) == null)
				{
					guistyleState = (this.m_OnFocused = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(7)));
				}
				return guistyleState;
			}
			set
			{
				this.AssignStyleState(7, value.m_Ptr);
			}
		}

		public RectOffset border
		{
			get
			{
				RectOffset rectOffset;
				if ((rectOffset = this.m_Border) == null)
				{
					rectOffset = (this.m_Border = new RectOffset(this, this.GetRectOffsetPtr(0)));
				}
				return rectOffset;
			}
			set
			{
				this.AssignRectOffset(0, value.m_Ptr);
			}
		}

		public RectOffset margin
		{
			get
			{
				RectOffset rectOffset;
				if ((rectOffset = this.m_Margin) == null)
				{
					rectOffset = (this.m_Margin = new RectOffset(this, this.GetRectOffsetPtr(1)));
				}
				return rectOffset;
			}
			set
			{
				this.AssignRectOffset(1, value.m_Ptr);
			}
		}

		public RectOffset padding
		{
			get
			{
				RectOffset rectOffset;
				if ((rectOffset = this.m_Padding) == null)
				{
					rectOffset = (this.m_Padding = new RectOffset(this, this.GetRectOffsetPtr(2)));
				}
				return rectOffset;
			}
			set
			{
				this.AssignRectOffset(2, value.m_Ptr);
			}
		}

		public RectOffset overflow
		{
			get
			{
				RectOffset rectOffset;
				if ((rectOffset = this.m_Overflow) == null)
				{
					rectOffset = (this.m_Overflow = new RectOffset(this, this.GetRectOffsetPtr(3)));
				}
				return rectOffset;
			}
			set
			{
				this.AssignRectOffset(3, value.m_Ptr);
			}
		}

		public float lineHeight
		{
			[CompilerGenerated]
			get
			{
				return Mathf.Round(GUIStyle.Internal_GetLineHeight(this.m_Ptr));
			}
		}

		public void Draw(Rect position, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
		{
			this.Draw(position, GUIContent.none, -1, isHover, isActive, on, hasKeyboardFocus);
		}

		public void Draw(Rect position, string text, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
		{
			this.Draw(position, GUIContent.Temp(text), -1, isHover, isActive, on, hasKeyboardFocus);
		}

		public void Draw(Rect position, Texture image, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
		{
			this.Draw(position, GUIContent.Temp(image), -1, isHover, isActive, on, hasKeyboardFocus);
		}

		public void Draw(Rect position, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
		{
			this.Draw(position, content, -1, isHover, isActive, on, hasKeyboardFocus);
		}

		public void Draw(Rect position, GUIContent content, int controlID)
		{
			this.Draw(position, content, controlID, false, false, false, false);
		}

		public void Draw(Rect position, GUIContent content, int controlID, bool on)
		{
			this.Draw(position, content, controlID, false, false, on, false);
		}

		private void Draw(Rect position, GUIContent content, int controlId, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
		{
			if (controlId == -1)
			{
				this.Internal_Draw(position, content, isHover, isActive, on, hasKeyboardFocus);
			}
			else
			{
				this.Internal_Draw2(position, content, controlId, on);
			}
		}

		public void DrawCursor(Rect position, GUIContent content, int controlID, int character)
		{
			Event current = Event.current;
			if (current.type == EventType.Repaint)
			{
				Color cursorColor = new Color(0f, 0f, 0f, 0f);
				float cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
				float num = (Time.realtimeSinceStartup - GUIStyle.Internal_GetCursorFlashOffset()) % cursorFlashSpeed / cursorFlashSpeed;
				if (cursorFlashSpeed == 0f || num < 0.5f)
				{
					cursorColor = GUI.skin.settings.cursorColor;
				}
				this.Internal_DrawCursor(position, content, character, cursorColor);
			}
		}

		internal void DrawWithTextSelection(Rect position, GUIContent content, bool isActive, bool hasKeyboardFocus, int firstSelectedCharacter, int lastSelectedCharacter, bool drawSelectionAsComposition, Color selectionColor)
		{
			Color cursorColor = new Color(0f, 0f, 0f, 0f);
			float cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
			float num = (Time.realtimeSinceStartup - GUIStyle.Internal_GetCursorFlashOffset()) % cursorFlashSpeed / cursorFlashSpeed;
			if (cursorFlashSpeed == 0f || num < 0.5f)
			{
				cursorColor = GUI.skin.settings.cursorColor;
			}
			this.Internal_DrawWithTextSelection(position, content, position.Contains(Event.current.mousePosition), isActive, false, hasKeyboardFocus, drawSelectionAsComposition, firstSelectedCharacter, lastSelectedCharacter, cursorColor, selectionColor);
		}

		internal void DrawWithTextSelection(Rect position, GUIContent content, bool isActive, bool hasKeyboardFocus, int firstSelectedCharacter, int lastSelectedCharacter, bool drawSelectionAsComposition)
		{
			this.DrawWithTextSelection(position, content, isActive, hasKeyboardFocus, firstSelectedCharacter, lastSelectedCharacter, drawSelectionAsComposition, GUI.skin.settings.selectionColor);
		}

		internal void DrawWithTextSelection(Rect position, GUIContent content, int controlID, int firstSelectedCharacter, int lastSelectedCharacter, bool drawSelectionAsComposition)
		{
			this.DrawWithTextSelection(position, content, controlID == GUIUtility.hotControl, controlID == GUIUtility.keyboardControl && GUIStyle.showKeyboardFocus, firstSelectedCharacter, lastSelectedCharacter, drawSelectionAsComposition);
		}

		public void DrawWithTextSelection(Rect position, GUIContent content, int controlID, int firstSelectedCharacter, int lastSelectedCharacter)
		{
			this.DrawWithTextSelection(position, content, controlID, firstSelectedCharacter, lastSelectedCharacter, false);
		}

		public static implicit operator GUIStyle(string str)
		{
			GUIStyle guistyle;
			if (GUISkin.current == null)
			{
				Debug.LogError("Unable to use a named GUIStyle without a current skin. Most likely you need to move your GUIStyle initialization code to OnGUI");
				guistyle = GUISkin.error;
			}
			else
			{
				guistyle = GUISkin.current.GetStyle(str);
			}
			return guistyle;
		}

		public static GUIStyle none
		{
			[CompilerGenerated]
			get
			{
				GUIStyle guistyle;
				if ((guistyle = GUIStyle.s_None) == null)
				{
					guistyle = (GUIStyle.s_None = new GUIStyle());
				}
				return guistyle;
			}
		}

		public Vector2 GetCursorPixelPosition(Rect position, GUIContent content, int cursorStringIndex)
		{
			return this.Internal_GetCursorPixelPosition(position, content, cursorStringIndex);
		}

		public int GetCursorStringIndex(Rect position, GUIContent content, Vector2 cursorPixelPosition)
		{
			return this.Internal_GetCursorStringIndex(position, content, cursorPixelPosition);
		}

		internal int GetNumCharactersThatFitWithinWidth(string text, float width)
		{
			return this.Internal_GetNumCharactersThatFitWithinWidth(text, width);
		}

		public Vector2 CalcSize(GUIContent content)
		{
			return this.Internal_CalcSize(content);
		}

		internal Vector2 CalcSizeWithConstraints(GUIContent content, Vector2 constraints)
		{
			return this.Internal_CalcSizeWithConstraints(content, constraints);
		}

		public Vector2 CalcScreenSize(Vector2 contentSize)
		{
			return new Vector2((this.fixedWidth == 0f) ? Mathf.Ceil(contentSize.x + (float)this.padding.left + (float)this.padding.right) : this.fixedWidth, (this.fixedHeight == 0f) ? Mathf.Ceil(contentSize.y + (float)this.padding.top + (float)this.padding.bottom) : this.fixedHeight);
		}

		public float CalcHeight(GUIContent content, float width)
		{
			return this.Internal_CalcHeight(content, width);
		}

		public bool isHeightDependantOnWidth
		{
			[CompilerGenerated]
			get
			{
				return this.fixedHeight == 0f && this.wordWrap && this.imagePosition != ImagePosition.ImageOnly;
			}
		}

		public void CalcMinMaxWidth(GUIContent content, out float minWidth, out float maxWidth)
		{
			Vector2 vector = this.Internal_CalcMinMaxWidth(content);
			minWidth = vector.x;
			maxWidth = vector.y;
		}

		public override string ToString()
		{
			return UnityString.Format("GUIStyle '{0}'", new object[] { this.name });
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_contentOffset_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_contentOffset_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_clipOffset_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_clipOffset_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_Internal_clipOffset_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_Internal_clipOffset_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Draw_Injected(ref Rect screenRect, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Draw2_Injected(ref Rect position, GUIContent content, int controlID, bool on);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DrawCursor_Injected(ref Rect position, GUIContent content, int pos, ref Color cursorColor);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_DrawWithTextSelection_Injected(ref Rect screenRect, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus, bool drawSelectionAsComposition, int cursorFirst, int cursorLast, ref Color cursorColor, ref Color selectionColor);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetCursorPixelPosition_Injected(ref Rect position, GUIContent content, int cursorStringIndex, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetCursorStringIndex_Injected(ref Rect position, GUIContent content, ref Vector2 cursorPixelPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_CalcSize_Injected(GUIContent content, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_CalcSizeWithConstraints_Injected(GUIContent content, ref Vector2 maxSize, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_CalcMinMaxWidth_Injected(GUIContent content, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMouseTooltip_Injected(string tooltip, ref Rect screenRect);

		[NonSerialized]
		internal IntPtr m_Ptr;

		[NonSerialized]
		private GUIStyleState m_Normal;

		[NonSerialized]
		private GUIStyleState m_Hover;

		[NonSerialized]
		private GUIStyleState m_Active;

		[NonSerialized]
		private GUIStyleState m_Focused;

		[NonSerialized]
		private GUIStyleState m_OnNormal;

		[NonSerialized]
		private GUIStyleState m_OnHover;

		[NonSerialized]
		private GUIStyleState m_OnActive;

		[NonSerialized]
		private GUIStyleState m_OnFocused;

		[NonSerialized]
		private RectOffset m_Border;

		[NonSerialized]
		private RectOffset m_Padding;

		[NonSerialized]
		private RectOffset m_Margin;

		[NonSerialized]
		private RectOffset m_Overflow;

		internal static bool showKeyboardFocus = true;

		private static GUIStyle s_None;
	}
}
