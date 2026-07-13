using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Input/InputManager.h")]
	[NativeHeader("Runtime/Camera/RenderLayers/GUITexture.h")]
	[NativeHeader("Runtime/Utilities/CopyPaste.h")]
	[NativeHeader("Modules/IMGUI/GUIManager.h")]
	[NativeHeader("Modules/IMGUI/GUIUtility.h")]
	[NativeHeader("Runtime/Input/InputBindings.h")]
	public class GUIUtility
	{
		public static extern bool hasModalWindow
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeProperty("GetGUIState().m_PixelsPerPoint", true, TargetType.Field)]
		internal static extern float pixelsPerPoint
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEditor.UIToolkitAuthoringModule" })]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("GetGUIState().m_OnGUIDepth", true, TargetType.Field)]
		internal static extern int guiDepth
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static Vector2 s_EditorScreenPointOffset
		{
			[NativeMethod("GetGUIState().GetGUIPixelOffset", true)]
			get
			{
				Vector2 vector;
				GUIUtility.get_s_EditorScreenPointOffset_Injected(out vector);
				return vector;
			}
			[NativeMethod("GetGUIState().SetGUIPixelOffset", true)]
			set
			{
				GUIUtility.set_s_EditorScreenPointOffset_Injected(ref value);
			}
		}

		[NativeProperty("GetGUIState().m_CanvasGUIState.m_IsMouseUsed", true, TargetType.Field)]
		internal static extern bool mouseUsed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetInputManager()", StaticAccessorType.Dot)]
		internal static extern bool textFieldInput
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool manualTex2SRGBEnabled
		{
			[FreeFunction("GUITexture::IsManualTex2SRGBEnabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("GUITexture::SetManualTex2SRGBEnabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public unsafe static string systemCopyBuffer
		{
			[FreeFunction("GetCopyBuffer")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					GUIUtility.get_systemCopyBuffer_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			[FreeFunction("SetCopyBuffer")]
			set
			{
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					GUIUtility.set_systemCopyBuffer_Injected(ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		[FreeFunction("GetGUIState().GetControlID")]
		private static int Internal_GetControlID(int hint, FocusType focusType, Rect rect)
		{
			return GUIUtility.Internal_GetControlID_Injected(hint, focusType, ref rect);
		}

		public static int GetControlID(int hint, FocusType focusType, Rect rect)
		{
			GUIUtility.s_ControlCount++;
			return GUIUtility.Internal_GetControlID(hint, focusType, rect);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void BeginContainerFromOwner(ScriptableObject owner)
		{
			GUIUtility.BeginContainerFromOwner_Injected(Object.MarshalledUnityObject.Marshal<ScriptableObject>(owner));
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void BeginContainer(ObjectGUIState objectGUIState)
		{
			GUIUtility.BeginContainer_Injected((objectGUIState == null) ? ((IntPtr)0) : ObjectGUIState.BindingsMarshaller.ConvertToNative(objectGUIState));
		}

		[NativeMethod("EndContainer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_EndContainer();

		[FreeFunction("GetSpecificGUIState(0).m_EternalGUIState->GetNextUniqueID")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetPermanentControlID();

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static int CheckForTabEvent(Event evt)
		{
			return GUIUtility.CheckForTabEvent_Injected((evt == null) ? ((IntPtr)0) : Event.BindingsMarshaller.ConvertToNative(evt));
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetKeyboardControlToFirstControlId();

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetKeyboardControlToLastControlId();

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasFocusableControls();

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool OwnsId(int id);

		public static Rect AlignRectToDevice(Rect rect, out int widthInPixels, out int heightInPixels)
		{
			Rect rect2;
			GUIUtility.AlignRectToDevice_Injected(ref rect, out widthInPixels, out heightInPixels, out rect2);
			return rect2;
		}

		[StaticAccessor("InputBindings", StaticAccessorType.DoubleColon)]
		internal static string compositionString
		{
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					GUIUtility.get_compositionString_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		[StaticAccessor("InputBindings", StaticAccessorType.DoubleColon)]
		internal static extern IMECompositionMode imeCompositionMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("InputBindings", StaticAccessorType.DoubleColon)]
		internal static Vector2 compositionCursorPos
		{
			get
			{
				Vector2 vector;
				GUIUtility.get_compositionCursorPos_Injected(out vector);
				return vector;
			}
			set
			{
				GUIUtility.set_compositionCursorPos_Injected(ref value);
			}
		}

		internal static Vector3 Internal_MultiplyPoint(Vector3 point, Matrix4x4 transform)
		{
			Vector3 vector;
			GUIUtility.Internal_MultiplyPoint_Injected(ref point, ref transform, out vector);
			return vector;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetChanged();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetChanged(bool changed);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetDidGUIWindowsEatLastEvent(bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetHotControl();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetKeyboardControl();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetHotControl(int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetKeyboardControl(int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object Internal_GetDefaultSkin(int skinMode);

		private static Object Internal_GetBuiltinSkin(int skin)
		{
			return Unmarshal.UnmarshalUnityObject<Object>(GUIUtility.Internal_GetBuiltinSkin_Injected(skin));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ExitGUI();

		private static Vector2 InternalWindowToScreenPoint(Vector2 windowPoint)
		{
			Vector2 vector;
			GUIUtility.InternalWindowToScreenPoint_Injected(ref windowPoint, out vector);
			return vector;
		}

		private static Vector2 InternalScreenToWindowPoint(Vector2 screenPoint)
		{
			Vector2 vector;
			GUIUtility.InternalScreenToWindowPoint_Injected(ref screenPoint, out vector);
			return vector;
		}

		[RequiredByNativeCode]
		private static void MarkGUIChanged()
		{
			Action action = GUIUtility.guiChanged;
			if (action != null)
			{
				action();
			}
		}

		public static int GetControlID(FocusType focus)
		{
			return GUIUtility.GetControlID(0, focus);
		}

		public static int GetControlID(GUIContent contents, FocusType focus)
		{
			return GUIUtility.GetControlID(contents.hash, focus);
		}

		public static int GetControlID(FocusType focus, Rect position)
		{
			return GUIUtility.GetControlID(0, focus, position);
		}

		public static int GetControlID(GUIContent contents, FocusType focus, Rect position)
		{
			return GUIUtility.GetControlID(contents.hash, focus, position);
		}

		public static int GetControlID(int hint, FocusType focus)
		{
			GUIUtility.CheckOnGUI();
			return GUIUtility.GetControlID(hint, focus, Rect.zero);
		}

		public static object GetStateObject(Type t, int controlID)
		{
			return GUIStateObjects.GetStateObject(t, controlID);
		}

		public static object QueryStateObject(Type t, int controlID)
		{
			return GUIStateObjects.QueryStateObject(t, controlID);
		}

		internal static bool guiIsExiting { get; set; }

		public static int hotControl
		{
			get
			{
				return GUIUtility.Internal_GetHotControl();
			}
			set
			{
				GUIUtility.WarnOnGUI();
				GUIUtility.Internal_SetHotControl(value);
			}
		}

		[RequiredByNativeCode]
		internal static void TakeCapture()
		{
			GUIUtility.WarnOnGUI();
			Action action = GUIUtility.takeCapture;
			if (action != null)
			{
				action();
			}
		}

		[RequiredByNativeCode]
		internal static void RemoveCapture()
		{
			Action action = GUIUtility.releaseCapture;
			if (action != null)
			{
				action();
			}
		}

		public static int keyboardControl
		{
			get
			{
				return GUIUtility.Internal_GetKeyboardControl();
			}
			set
			{
				GUIUtility.Internal_SetKeyboardControl(value);
			}
		}

		internal static bool HasKeyFocus(int controlID)
		{
			GUIUtility.WarnOnGUI();
			return controlID == GUIUtility.keyboardControl && (GUIUtility.s_HasCurrentWindowKeyFocusFunc == null || GUIUtility.s_HasCurrentWindowKeyFocusFunc());
		}

		public static void ExitGUI()
		{
			GUIUtility.WarnOnGUI();
			throw new ExitGUIException();
		}

		internal static GUISkin GetDefaultSkin(int skinMode)
		{
			return GUIUtility.Internal_GetDefaultSkin(skinMode) as GUISkin;
		}

		internal static GUISkin GetDefaultSkin()
		{
			return GUIUtility.Internal_GetDefaultSkin(GUIUtility.s_SkinMode) as GUISkin;
		}

		internal static GUISkin GetBuiltinSkin(int skin)
		{
			return GUIUtility.Internal_GetBuiltinSkin(skin) as GUISkin;
		}

		[RequiredByNativeCode]
		internal static void ProcessEvent(int instanceID, IntPtr nativeEventPtr, out bool result)
		{
			bool flag = GUIUtility.beforeEventProcessed != null;
			if (flag)
			{
				GUIUtility.m_Event.CopyFromPtr(nativeEventPtr);
				GUIUtility.beforeEventProcessed(GUIUtility.m_Event.type, GUIUtility.m_Event.keyCode, GUIUtility.m_Event.modifiers);
			}
			result = false;
			bool flag2 = GUIUtility.processEvent != null;
			if (flag2)
			{
				foreach (Delegate @delegate in GUIUtility.processEvent.GetInvocationList())
				{
					Func<int, IntPtr, bool> func = @delegate as Func<int, IntPtr, bool>;
					bool flag3 = func == null;
					if (!flag3)
					{
						result |= func(instanceID, nativeEventPtr);
					}
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void EndContainer()
		{
			GUIUtility.Internal_EndContainer();
			GUIUtility.Internal_ExitGUI();
		}

		internal static void CleanupRoots()
		{
			Action action = GUIUtility.cleanupRoots;
			if (action != null)
			{
				action();
			}
		}

		[RequiredByNativeCode]
		internal static void BeginGUI(int skinMode, int instanceID, int useGUILayout)
		{
			GUIUtility.s_SkinMode = skinMode;
			GUIUtility.s_OriginalID = instanceID;
			GUIUtility.ResetGlobalState();
			bool flag = useGUILayout != 0;
			if (flag)
			{
				GUILayoutUtility.Begin(instanceID);
			}
		}

		[RequiredByNativeCode]
		internal static void DestroyGUI(int instanceID)
		{
			GUILayoutUtility.RemoveSelectedIdList(instanceID, false);
		}

		[RequiredByNativeCode]
		internal static void EndGUI(int layoutType)
		{
			try
			{
				bool flag = Event.current.type == EventType.Layout;
				if (flag)
				{
					switch (layoutType)
					{
					case 1:
						GUILayoutUtility.Layout();
						break;
					case 2:
						GUILayoutUtility.LayoutFromEditorWindow();
						break;
					}
				}
				GUILayoutUtility.SelectIDList(GUIUtility.s_OriginalID, false);
				GUIContent.ClearStaticCache();
			}
			finally
			{
				GUIUtility.Internal_ExitGUI();
			}
		}

		[RequiredByNativeCode]
		internal static bool EndGUIFromException(Exception exception)
		{
			GUIUtility.Internal_ExitGUI();
			return GUIUtility.ShouldRethrowException(exception);
		}

		[RequiredByNativeCode]
		internal static bool EndContainerGUIFromException(Exception exception)
		{
			bool flag = GUIUtility.endContainerGUIFromException != null;
			return flag && GUIUtility.endContainerGUIFromException(exception);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void ResetGlobalState()
		{
			GUI.skin = null;
			GUIUtility.guiIsExiting = false;
			GUI.changed = false;
			GUI.scrollViewStates.Clear();
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static bool IsExitGUIException(Exception exception)
		{
			while (exception is TargetInvocationException && exception.InnerException != null)
			{
				exception = exception.InnerException;
			}
			return exception is ExitGUIException;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static bool ShouldRethrowException(Exception exception)
		{
			return GUIUtility.IsExitGUIException(exception);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static bool isUITK { get; set; } = false;

		internal static void CheckOnGUI()
		{
			bool flag = GUIUtility.guiDepth <= 0;
			if (flag)
			{
				throw new ArgumentException("You can only call GUI functions from inside OnGUI.");
			}
		}

		internal static void WarnOnGUI()
		{
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static float RoundToPixelGrid(float v)
		{
			GUIUtility.WarnOnGUI();
			return Mathf.Floor(v * GUIUtility.pixelsPerPoint + 0.48f) / GUIUtility.pixelsPerPoint;
		}

		internal static float RoundToPixelGrid(float v, float scale)
		{
			return Mathf.Floor(v * scale + 0.48f) / scale;
		}

		public static Vector2 GUIToScreenPoint(Vector2 guiPoint)
		{
			GUIUtility.WarnOnGUI();
			return GUIUtility.InternalWindowToScreenPoint(GUIClip.UnclipToWindow(guiPoint));
		}

		public static Rect GUIToScreenRect(Rect guiRect)
		{
			GUIUtility.WarnOnGUI();
			Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
			guiRect.x = vector.x;
			guiRect.y = vector.y;
			return guiRect;
		}

		public static Vector2 ScreenToGUIPoint(Vector2 screenPoint)
		{
			GUIUtility.WarnOnGUI();
			return GUIClip.ClipToWindow(GUIUtility.InternalScreenToWindowPoint(screenPoint));
		}

		public static Rect ScreenToGUIRect(Rect screenRect)
		{
			GUIUtility.WarnOnGUI();
			Vector2 vector = GUIUtility.ScreenToGUIPoint(new Vector2(screenRect.x, screenRect.y));
			screenRect.x = vector.x;
			screenRect.y = vector.y;
			return screenRect;
		}

		public static void RotateAroundPivot(float angle, Vector2 pivotPoint)
		{
			GUIUtility.WarnOnGUI();
			Matrix4x4 matrix = GUI.matrix;
			GUI.matrix = Matrix4x4.identity;
			Vector2 vector = GUIClip.Unclip(pivotPoint);
			Matrix4x4 matrix4x = Matrix4x4.TRS(vector, Quaternion.Euler(0f, 0f, angle), Vector3.one) * Matrix4x4.TRS(-vector, Quaternion.identity, Vector3.one);
			GUI.matrix = matrix4x * matrix;
		}

		public static void ScaleAroundPivot(Vector2 scale, Vector2 pivotPoint)
		{
			GUIUtility.WarnOnGUI();
			Matrix4x4 matrix = GUI.matrix;
			Vector2 vector = GUIClip.Unclip(pivotPoint);
			Matrix4x4 matrix4x = Matrix4x4.TRS(vector, Quaternion.identity, new Vector3(scale.x, scale.y, 1f)) * Matrix4x4.TRS(-vector, Quaternion.identity, Vector3.one);
			GUI.matrix = matrix4x * matrix;
		}

		public static Rect AlignRectToDevice(Rect rect)
		{
			GUIUtility.WarnOnGUI();
			int num;
			int num2;
			return GUIUtility.AlignRectToDevice(rect, out num, out num2);
		}

		internal static bool HitTest(Rect rect, Vector2 point, int offset)
		{
			return point.x >= rect.xMin - (float)offset && point.x < rect.xMax + (float)offset && point.y >= rect.yMin - (float)offset && point.y < rect.yMax + (float)offset;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static bool HitTest(Rect rect, Vector2 point, bool isDirectManipulationDevice)
		{
			int num = 0;
			return GUIUtility.HitTest(rect, point, num);
		}

		internal static bool HitTest(Rect rect, Event evt)
		{
			return GUIUtility.HitTest(rect, evt.mousePosition, evt.isDirectManipulationDevice);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_s_EditorScreenPointOffset_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_s_EditorScreenPointOffset_Injected([In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_systemCopyBuffer_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_systemCopyBuffer_Injected(ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetControlID_Injected(int hint, FocusType focusType, [In] ref Rect rect);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginContainerFromOwner_Injected(IntPtr owner);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginContainer_Injected(IntPtr objectGUIState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CheckForTabEvent_Injected(IntPtr evt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AlignRectToDevice_Injected([In] ref Rect rect, out int widthInPixels, out int heightInPixels, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_compositionString_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_compositionCursorPos_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_compositionCursorPos_Injected([In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_MultiplyPoint_Injected([In] ref Vector3 point, [In] ref Matrix4x4 transform, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_GetBuiltinSkin_Injected(int skin);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalWindowToScreenPoint_Injected([In] ref Vector2 windowPoint, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalScreenToWindowPoint_Injected([In] ref Vector2 screenPoint, out Vector2 ret);

		internal static int s_ControlCount = 0;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static int s_SkinMode;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static int s_OriginalID;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static Action takeCapture;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static Action releaseCapture;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static Func<int, IntPtr, bool> processEvent;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static Action cleanupRoots;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static Func<Exception, bool> endContainerGUIFromException;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static Action guiChanged;

		internal static Action<EventType, KeyCode, EventModifiers> beforeEventProcessed;

		private static Event m_Event = new Event();

		internal static Func<bool> s_HasCurrentWindowKeyFocusFunc;
	}
}
