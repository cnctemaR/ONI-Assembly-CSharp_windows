using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.TextCore.Text;

namespace UnityEngine
{
	[NativeHeader("Modules/IMGUI/GUIStyle.bindings.h")]
	[RequiredByNativeCode]
	[NativeHeader("IMGUIScriptingClasses.h")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class GUIStyle
	{
		[NativeProperty("Name", false, TargetType.Function)]
		internal unsafe string rawName
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					GUIStyle.get_rawName_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			set
			{
				try
				{
					IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					GUIStyle.set_rawName_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		[NativeProperty("Font", false, TargetType.Function)]
		public Font font
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Font>(GUIStyle.get_font_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_font_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Font>(value));
			}
		}

		[NativeProperty("m_ImagePosition", false, TargetType.Field)]
		public ImagePosition imagePosition
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GUIStyle.get_imagePosition_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_imagePosition_Injected(intPtr, value);
			}
		}

		[NativeProperty("m_Alignment", false, TargetType.Field)]
		public TextAnchor alignment
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GUIStyle.get_alignment_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_alignment_Injected(intPtr, value);
			}
		}

		[NativeProperty("m_WordWrap", false, TargetType.Field)]
		public bool wordWrap
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GUIStyle.get_wordWrap_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_wordWrap_Injected(intPtr, value);
			}
		}

		[NativeProperty("m_Clipping", false, TargetType.Field)]
		public TextClipping clipping
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GUIStyle.get_clipping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_clipping_Injected(intPtr, value);
			}
		}

		[NativeProperty("m_ContentOffset", false, TargetType.Field)]
		public Vector2 contentOffset
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				GUIStyle.get_contentOffset_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_contentOffset_Injected(intPtr, ref value);
			}
		}

		[NativeProperty("m_FixedWidth", false, TargetType.Field)]
		public float fixedWidth
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GUIStyle.get_fixedWidth_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_fixedWidth_Injected(intPtr, value);
			}
		}

		[NativeProperty("m_FixedHeight", false, TargetType.Field)]
		public float fixedHeight
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GUIStyle.get_fixedHeight_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_fixedHeight_Injected(intPtr, value);
			}
		}

		[NativeProperty("m_StretchWidth", false, TargetType.Field)]
		public bool stretchWidth
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GUIStyle.get_stretchWidth_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_stretchWidth_Injected(intPtr, value);
			}
		}

		[NativeProperty("m_StretchHeight", false, TargetType.Field)]
		public bool stretchHeight
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GUIStyle.get_stretchHeight_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_stretchHeight_Injected(intPtr, value);
			}
		}

		[NativeProperty("m_FontSize", false, TargetType.Field)]
		public int fontSize
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GUIStyle.get_fontSize_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_fontSize_Injected(intPtr, value);
			}
		}

		[NativeProperty("m_FontStyle", false, TargetType.Field)]
		public FontStyle fontStyle
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GUIStyle.get_fontStyle_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_fontStyle_Injected(intPtr, value);
			}
		}

		[NativeProperty("m_RichText", false, TargetType.Field)]
		public bool richText
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GUIStyle.get_richText_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_richText_Injected(intPtr, value);
			}
		}

		[NativeProperty("m_IsGizmo", false, TargetType.Field)]
		internal bool isGizmo
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GUIStyle.get_isGizmo_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_isGizmo_Injected(intPtr, value);
			}
		}

		[Obsolete("Don't use clipOffset - put things inside BeginGroup instead. This functionality will be removed in a later version.", false)]
		[NativeProperty("m_ClipOffset", false, TargetType.Field)]
		public Vector2 clipOffset
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				GUIStyle.get_clipOffset_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_clipOffset_Injected(intPtr, ref value);
			}
		}

		[NativeProperty("m_ClipOffset", false, TargetType.Field)]
		internal Vector2 Internal_clipOffset
		{
			get
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				GUIStyle.get_Internal_clipOffset_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GUIStyle.set_Internal_clipOffset_Injected(intPtr, ref value);
			}
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_Create", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create([UnityMarshalAs(NativeType.ScriptingObjectPtr)] GUIStyle self);

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_Copy", IsThreadSafe = true)]
		private static IntPtr Internal_Copy([UnityMarshalAs(NativeType.ScriptingObjectPtr)] GUIStyle self, GUIStyle other)
		{
			return GUIStyle.Internal_Copy_Injected(self, (other == null) ? ((IntPtr)0) : GUIStyle.BindingsMarshaller.ConvertToNative(other));
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_Destroy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr self);

		[FreeFunction(Name = "GUIStyle_Bindings::GetStyleStatePtr", IsThreadSafe = true, HasExplicitThis = true)]
		private IntPtr GetStyleStatePtr(int idx)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GUIStyle.GetStyleStatePtr_Injected(intPtr, idx);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::AssignStyleState", HasExplicitThis = true)]
		private void AssignStyleState(int idx, IntPtr srcStyleState)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GUIStyle.AssignStyleState_Injected(intPtr, idx, srcStyleState);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::GetRectOffsetPtr", HasExplicitThis = true)]
		private IntPtr GetRectOffsetPtr(int idx)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GUIStyle.GetRectOffsetPtr_Injected(intPtr, idx);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::AssignRectOffset", HasExplicitThis = true)]
		private void AssignRectOffset(int idx, IntPtr srcRectOffset)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GUIStyle.AssignRectOffset_Injected(intPtr, idx, srcRectOffset);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_Draw", HasExplicitThis = true)]
		private void Internal_Draw(Rect screenRect, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GUIStyle.Internal_Draw_Injected(intPtr, ref screenRect, content, isHover, isActive, on, hasKeyboardFocus);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_Draw2", HasExplicitThis = true)]
		private void Internal_Draw2(Rect position, GUIContent content, int controlID, bool on)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GUIStyle.Internal_Draw2_Injected(intPtr, ref position, content, controlID, on);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_DrawCursor", HasExplicitThis = true)]
		private void Internal_DrawCursor(Rect position, GUIContent content, Vector2 pos, Color cursorColor)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GUIStyle.Internal_DrawCursor_Injected(intPtr, ref position, content, ref pos, ref cursorColor);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_DrawWithTextSelection", HasExplicitThis = true)]
		private void Internal_DrawWithTextSelection(Rect screenRect, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus, bool drawSelectionAsComposition, Vector2 cursorFirstPosition, Vector2 cursorLastPosition, Color cursorColor, Color selectionColor)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GUIStyle.Internal_DrawWithTextSelection_Injected(intPtr, ref screenRect, content, isHover, isActive, on, hasKeyboardFocus, drawSelectionAsComposition, ref cursorFirstPosition, ref cursorLastPosition, ref cursorColor, ref selectionColor);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_CalcSize", HasExplicitThis = true)]
		internal Vector2 Internal_CalcSize(GUIContent content)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			GUIStyle.Internal_CalcSize_Injected(intPtr, content, out vector);
			return vector;
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_CalcSizeWithConstraints", HasExplicitThis = true)]
		internal Vector2 Internal_CalcSizeWithConstraints(GUIContent content, Vector2 maxSize)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			GUIStyle.Internal_CalcSizeWithConstraints_Injected(intPtr, content, ref maxSize, out vector);
			return vector;
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_CalcHeight", HasExplicitThis = true)]
		private float Internal_CalcHeight(GUIContent content, float width)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GUIStyle.Internal_CalcHeight_Injected(intPtr, content, width);
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_CalcMinMaxWidth", HasExplicitThis = true)]
		private Vector2 Internal_CalcMinMaxWidth(GUIContent content)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			GUIStyle.Internal_CalcMinMaxWidth_Injected(intPtr, content, out vector);
			return vector;
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_GetTextRectOffset", HasExplicitThis = true)]
		internal Vector2 Internal_GetTextRectOffset(Rect screenRect, GUIContent content, Vector2 textSize)
		{
			IntPtr intPtr = GUIStyle.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			GUIStyle.Internal_GetTextRectOffset_Injected(intPtr, ref screenRect, content, ref textSize, out vector);
			return vector;
		}

		[FreeFunction(Name = "GUIStyle_Bindings::SetMouseTooltip")]
		internal unsafe static void SetMouseTooltip(string tooltip, Rect screenRect)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tooltip, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tooltip.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				GUIStyle.SetMouseTooltip_Injected(ref managedSpanWrapper, ref screenRect);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction(Name = "GUIStyle_Bindings::IsTooltipActive")]
		internal unsafe static bool IsTooltipActive(string tooltip)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tooltip, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tooltip.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = GUIStyle.IsTooltipActive_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_GetCursorFlashOffset")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float Internal_GetCursorFlashOffset();

		[FreeFunction(Name = "GUIStyle::SetDefaultFont")]
		internal static void SetDefaultFont(Font font)
		{
			GUIStyle.SetDefaultFont_Injected(Object.MarshalledUnityObject.Marshal<Font>(font));
		}

		[FreeFunction(Name = "GUIStyle::GetDefaultFont")]
		internal static Font GetDefaultFont()
		{
			return Unmarshal.UnmarshalUnityObject<Font>(GUIStyle.GetDefaultFont_Injected());
		}

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_DestroyTextGenerator")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_DestroyTextGenerator(int meshInfoId);

		[FreeFunction(Name = "GUIStyle_Bindings::Internal_CleanupAllTextGenerator")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_CleanupAllTextGenerator();

		public GUIStyle()
		{
			this.m_Ptr = GUIStyle.Internal_Create(this);
		}

		public GUIStyle(GUIStyle other)
		{
			bool flag = other == null;
			if (flag)
			{
				Debug.LogError("Copied style is null. Using StyleNotFound instead.");
				other = GUISkin.error;
			}
			this.m_Ptr = GUIStyle.Internal_Copy(this, other);
		}

		protected override void Finalize()
		{
			try
			{
				bool flag = this.m_Ptr != IntPtr.Zero;
				if (flag)
				{
					GUIStyle.Internal_Destroy(this.m_Ptr);
					this.m_Ptr = IntPtr.Zero;
				}
			}
			finally
			{
				base.Finalize();
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

		public string name
		{
			get
			{
				string text;
				if ((text = this.m_Name) == null)
				{
					text = (this.m_Name = this.rawName);
				}
				return text;
			}
			set
			{
				this.m_Name = value;
				this.rawName = value;
			}
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
			get
			{
				return Mathf.Round(IMGUITextHandle.GetLineHeight(this));
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

		public void Draw(Rect position, GUIContent content, int controlID, bool on, bool hover)
		{
			this.Draw(position, content, controlID, hover, GUIUtility.hotControl == controlID, on, GUIUtility.HasKeyFocus(controlID));
		}

		private void Draw(Rect position, GUIContent content, int controlId, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
		{
			bool flag = controlId == -1;
			if (flag)
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
			bool flag = current.type == EventType.Repaint;
			if (flag)
			{
				Color cursorColor = new Color(0f, 0f, 0f, 0f);
				float cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
				float num = (Time.realtimeSinceStartup - GUIStyle.Internal_GetCursorFlashOffset()) % cursorFlashSpeed / cursorFlashSpeed;
				bool flag2 = cursorFlashSpeed == 0f || num < 0.5f;
				if (flag2)
				{
					cursorColor = GUI.skin.settings.cursorColor;
				}
				this.Internal_DrawCursor(position, content, this.GetCursorPixelPosition(position, content, character), cursorColor);
			}
		}

		internal void DrawWithTextSelection(Rect position, GUIContent content, bool isActive, bool hasKeyboardFocus, int firstSelectedCharacter, int lastSelectedCharacter, bool drawSelectionAsComposition, Color selectionColor)
		{
			bool flag = firstSelectedCharacter > lastSelectedCharacter;
			if (flag)
			{
				int num = lastSelectedCharacter;
				lastSelectedCharacter = firstSelectedCharacter;
				firstSelectedCharacter = num;
			}
			Vector2 vector = this.GetCursorPixelPosition(position, content, firstSelectedCharacter);
			Vector2 vector2 = this.GetCursorPixelPosition(position, content, lastSelectedCharacter);
			Vector2 vector3 = new Vector2(string.IsNullOrEmpty(content.text) ? 0f : 1f, 0f);
			vector -= vector3;
			vector2 -= vector3;
			Color cursorColor = new Color(0f, 0f, 0f, 0f);
			float cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
			float num2 = (Time.realtimeSinceStartup - GUIStyle.Internal_GetCursorFlashOffset()) % cursorFlashSpeed / cursorFlashSpeed;
			bool flag2 = cursorFlashSpeed == 0f || num2 < 0.5f;
			if (flag2)
			{
				cursorColor = GUI.skin.settings.cursorColor;
			}
			bool flag3 = position.Contains(Event.current.mousePosition);
			this.Internal_DrawWithTextSelection(position, content, flag3, isActive, false, hasKeyboardFocus, drawSelectionAsComposition, vector, vector2, cursorColor, selectionColor);
		}

		internal void DrawWithTextSelection(Rect position, GUIContent content, int controlID, int firstSelectedCharacter, int lastSelectedCharacter, bool drawSelectionAsComposition)
		{
			this.DrawWithTextSelection(position, content, controlID == GUIUtility.hotControl, controlID == GUIUtility.keyboardControl && GUIStyle.showKeyboardFocus, firstSelectedCharacter, lastSelectedCharacter, drawSelectionAsComposition, GUI.skin.settings.selectionColor);
		}

		public void DrawWithTextSelection(Rect position, GUIContent content, int controlID, int firstSelectedCharacter, int lastSelectedCharacter)
		{
			this.DrawWithTextSelection(position, content, controlID, firstSelectedCharacter, lastSelectedCharacter, false);
		}

		public static implicit operator GUIStyle(string str)
		{
			bool flag = GUISkin.current == null;
			GUIStyle guistyle;
			if (flag)
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
			Rect rect = position;
			rect.width = ((this.fixedWidth == 0f) ? rect.width : this.fixedWidth);
			rect.height = ((this.fixedHeight == 0f) ? rect.height : this.fixedHeight);
			IMGUITextHandle textHandle = IMGUITextHandle.GetTextHandle(this, this.padding.Remove(rect), content.textWithWhitespace, Color.white);
			Vector2 cursorPositionFromStringIndexUsingLineHeight = textHandle.GetCursorPositionFromStringIndexUsingLineHeight(cursorStringIndex, false, true);
			cursorPositionFromStringIndexUsingLineHeight = new Vector2(Mathf.Max(0f, cursorPositionFromStringIndexUsingLineHeight.x), cursorPositionFromStringIndexUsingLineHeight.y);
			Vector2 vector = this.Internal_GetTextRectOffset(rect, content, new Vector2(textHandle.preferredSize.x, (textHandle.preferredSize.y > 0f) ? textHandle.preferredSize.y : this.lineHeight));
			return cursorPositionFromStringIndexUsingLineHeight + vector + this.Internal_clipOffset - new Vector2(0f, this.lineHeight);
		}

		internal Rect[] GetHyperlinkRects(IMGUITextHandle handle, Rect content)
		{
			content = this.padding.Remove(content);
			return handle.GetHyperlinkRects(content);
		}

		public int GetCursorStringIndex(Rect position, GUIContent content, Vector2 cursorPixelPosition)
		{
			return IMGUITextHandle.GetTextHandle(this, position, content.textWithWhitespace, Color.white).GetCursorIndexFromPosition(cursorPixelPosition, true);
		}

		internal int GetNumCharactersThatFitWithinWidth(string text, float width)
		{
			return IMGUITextHandle.GetTextHandle(this, new Rect(0f, 0f, width, 1f), text, Color.white).GetNumCharactersThatFitWithinWidth(width);
		}

		public Vector2 CalcSize(GUIContent content)
		{
			return this.Internal_CalcSize(content);
		}

		internal Vector2 CalcSizeWithConstraints(GUIContent content, Vector2 constraints)
		{
			Vector2 vector = this.Internal_CalcSizeWithConstraints(content, constraints);
			bool flag = constraints.x > 0f;
			if (flag)
			{
				vector.x = Mathf.Min(vector.x, constraints.x);
			}
			bool flag2 = constraints.y > 0f;
			if (flag2)
			{
				vector.y = Mathf.Min(vector.y, constraints.y);
			}
			return vector;
		}

		public Vector2 CalcScreenSize(Vector2 contentSize)
		{
			return new Vector2((this.fixedWidth != 0f) ? this.fixedWidth : Mathf.Ceil(contentSize.x + (float)this.padding.left + (float)this.padding.right), (this.fixedHeight != 0f) ? this.fixedHeight : Mathf.Ceil(contentSize.y + (float)this.padding.top + (float)this.padding.bottom));
		}

		public float CalcHeight(GUIContent content, float width)
		{
			return this.Internal_CalcHeight(content, width);
		}

		internal Vector2 GetPreferredSize(string content, Rect rect)
		{
			return IMGUITextHandle.GetTextHandle(this, this.padding.Remove(rect), content, Color.white).preferredSize;
		}

		public bool isHeightDependantOnWidth
		{
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
			return string.Format("GUIStyle '{0}'", this.name);
		}

		[RequiredByNativeCode]
		internal static void GetMeshInfo(GUIStyle style, Color color, string content, Rect rect, ref MeshInfoBindings[] meshInfos, ref Vector2 dimensions, ref int generationId)
		{
			bool flag = false;
			IMGUITextHandle textHandle = IMGUITextHandle.GetTextHandle(style, rect, content, color, ref flag);
			generationId = TextHandle.settings.GetHashCode();
			float num = 1f / GUIUtility.pixelsPerPoint;
			bool flag2 = !flag;
			if (flag2)
			{
				TextInfo textInfo = textHandle.textInfo;
				meshInfos = new MeshInfoBindings[textInfo.materialCount];
				for (int i = 0; i < textInfo.materialCount; i++)
				{
					meshInfos[i].vertexData = new TextCoreVertex[textInfo.meshInfo[i].vertexCount];
					meshInfos[i].vertexCount = textInfo.meshInfo[i].vertexCount;
					meshInfos[i].material = textInfo.meshInfo[i].material;
					Array.Copy(textInfo.meshInfo[i].vertexData, meshInfos[i].vertexData, textInfo.meshInfo[i].vertexCount);
					for (int j = 0; j < meshInfos[i].vertexData.Length; j++)
					{
						TextCoreVertex[] vertexData = meshInfos[i].vertexData;
						int num2 = j;
						vertexData[num2].position = vertexData[num2].position * num;
					}
				}
			}
			dimensions = textHandle.preferredSize;
		}

		[RequiredByNativeCode]
		internal static void GetDimensions(GUIStyle style, Color color, string content, Rect rect, ref Vector2 dimensions)
		{
			dimensions = style.GetPreferredSize(content, rect);
		}

		[RequiredByNativeCode]
		internal static void GetLineHeight(GUIStyle style, ref float lineHeight)
		{
			lineHeight = style.lineHeight;
		}

		[RequiredByNativeCode]
		internal static void EmptyManagedCache()
		{
			IMGUITextHandle.EmptyManagedCache();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rawName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rawName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_font_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_font_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ImagePosition get_imagePosition_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_imagePosition_Injected(IntPtr _unity_self, ImagePosition value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextAnchor get_alignment_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_alignment_Injected(IntPtr _unity_self, TextAnchor value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_wordWrap_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wordWrap_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextClipping get_clipping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_clipping_Injected(IntPtr _unity_self, TextClipping value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_contentOffset_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_contentOffset_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_fixedWidth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_fixedWidth_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_fixedHeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_fixedHeight_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_stretchWidth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_stretchWidth_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_stretchHeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_stretchHeight_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_fontSize_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_fontSize_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern FontStyle get_fontStyle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_fontStyle_Injected(IntPtr _unity_self, FontStyle value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_richText_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_richText_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isGizmo_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_isGizmo_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_clipOffset_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_clipOffset_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_Internal_clipOffset_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_Internal_clipOffset_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Copy_Injected(GUIStyle self, IntPtr other);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetStyleStatePtr_Injected(IntPtr _unity_self, int idx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AssignStyleState_Injected(IntPtr _unity_self, int idx, IntPtr srcStyleState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetRectOffsetPtr_Injected(IntPtr _unity_self, int idx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AssignRectOffset_Injected(IntPtr _unity_self, int idx, IntPtr srcRectOffset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Draw_Injected(IntPtr _unity_self, [In] ref Rect screenRect, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Draw2_Injected(IntPtr _unity_self, [In] ref Rect position, GUIContent content, int controlID, bool on);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawCursor_Injected(IntPtr _unity_self, [In] ref Rect position, GUIContent content, [In] ref Vector2 pos, [In] ref Color cursorColor);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawWithTextSelection_Injected(IntPtr _unity_self, [In] ref Rect screenRect, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus, bool drawSelectionAsComposition, [In] ref Vector2 cursorFirstPosition, [In] ref Vector2 cursorLastPosition, [In] ref Color cursorColor, [In] ref Color selectionColor);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CalcSize_Injected(IntPtr _unity_self, GUIContent content, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CalcSizeWithConstraints_Injected(IntPtr _unity_self, GUIContent content, [In] ref Vector2 maxSize, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float Internal_CalcHeight_Injected(IntPtr _unity_self, GUIContent content, float width);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CalcMinMaxWidth_Injected(IntPtr _unity_self, GUIContent content, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetTextRectOffset_Injected(IntPtr _unity_self, [In] ref Rect screenRect, GUIContent content, [In] ref Vector2 textSize, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMouseTooltip_Injected(ref ManagedSpanWrapper tooltip, [In] ref Rect screenRect);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTooltipActive_Injected(ref ManagedSpanWrapper tooltip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDefaultFont_Injected(IntPtr font);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDefaultFont_Injected();

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

		[NonSerialized]
		private string m_Name;

		internal static bool showKeyboardFocus = true;

		private static GUIStyle s_None;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(GUIStyle guiStyle)
			{
				return guiStyle.m_Ptr;
			}
		}
	}
}
