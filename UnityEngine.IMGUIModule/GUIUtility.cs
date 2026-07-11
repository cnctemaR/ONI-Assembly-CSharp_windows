using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Utilities/CopyPaste.h")]
	[NativeHeader("Runtime/Input/InputManager.h")]
	[NativeHeader("Runtime/Camera/RenderLayers/GUITexture.h")]
	[NativeHeader("Modules/IMGUI/GUIManager.h")]
	[NativeHeader("Modules/IMGUI/GUIUtility.h")]
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
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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
			[NativeMethod("GetGUIManager().GetGUIPixelOffset", true)]
			get
			{
				Vector2 vector;
				GUIUtility.get_s_EditorScreenPointOffset_Injected(out vector);
				return vector;
			}
			[NativeMethod("GetGUIManager().SetGUIPixelOffset", true)]
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
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			[FreeFunction("GUITexture::SetManualTex2SRGBEnabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string systemCopyBuffer
		{
			[FreeFunction("GetCopyBuffer")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("SetCopyBuffer")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetGUIState()", StaticAccessorType.Dot)]
		public static int GetControlID(int hint, FocusType focusType, Rect rect)
		{
			return GUIUtility.GetControlID_Injected(hint, focusType, ref rect);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void BeginContainerFromOwner(ScriptableObject owner);

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void BeginContainer(ObjectGUIState objectGUIState);

		[NativeMethod("EndContainer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_EndContainer();

		[FreeFunction("GetSpecificGUIState(0).m_EternalGUIState->GetNextUniqueID")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetPermanentControlID();

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int CheckForTabEvent(Event evt);

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetKeyboardControlToFirstControlId();

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetKeyboardControlToLastControlId();

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasFocusableControls();

		public static Rect AlignRectToDevice(Rect rect, out int widthInPixels, out int heightInPixels)
		{
			Rect rect2;
			GUIUtility.AlignRectToDevice_Injected(ref rect, out widthInPixels, out heightInPixels, out rect2);
			return rect2;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_GetBuiltinSkin(int skin);

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
			if (GUIUtility.enabledStateChanged != null)
			{
				GUIUtility.enabledStateChanged();
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
				GUIUtility.Internal_SetHotControl(value);
			}
		}

		[RequiredByNativeCode]
		internal static void TakeCapture()
		{
			if (GUIUtility.takeCapture != null)
			{
				GUIUtility.takeCapture();
			}
		}

		[RequiredByNativeCode]
		internal static void RemoveCapture()
		{
			if (GUIUtility.releaseCapture != null)
			{
				GUIUtility.releaseCapture();
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

		public static void ExitGUI()
		{
			GUIUtility.guiIsExiting = true;
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
		internal static bool ProcessEvent(int instanceID, IntPtr nativeEventPtr)
		{
			return GUIUtility.processEvent != null && GUIUtility.processEvent(instanceID, nativeEventPtr);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void EndContainer()
		{
			GUIUtility.Internal_EndContainer();
			GUIUtility.Internal_ExitGUI();
		}

		internal static void CleanupRoots()
		{
			if (GUIUtility.cleanupRoots != null)
			{
				GUIUtility.cleanupRoots();
			}
		}

		[RequiredByNativeCode]
		internal static void BeginGUI(int skinMode, int instanceID, int useGUILayout)
		{
			GUIUtility.s_SkinMode = skinMode;
			GUIUtility.s_OriginalID = instanceID;
			GUIUtility.ResetGlobalState();
			if (useGUILayout != 0)
			{
				GUILayoutUtility.Begin(instanceID);
			}
		}

		[RequiredByNativeCode]
		internal static void EndGUI(int layoutType)
		{
			try
			{
				if (Event.current.type == EventType.Layout)
				{
					if (layoutType != 0)
					{
						if (layoutType != 1)
						{
							if (layoutType == 2)
							{
								GUILayoutUtility.LayoutFromEditorWindow();
							}
						}
						else
						{
							GUILayoutUtility.Layout();
						}
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
			return GUIUtility.endContainerGUIFromException != null && GUIUtility.endContainerGUIFromException(exception);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void ResetGlobalState()
		{
			GUI.skin = null;
			GUIUtility.guiIsExiting = false;
			GUI.changed = false;
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

		internal static void CheckOnGUI()
		{
			if (GUIUtility.guiDepth <= 0)
			{
				throw new ArgumentException("You can only call GUI functions from inside OnGUI.");
			}
		}

		public static Vector2 GUIToScreenPoint(Vector2 guiPoint)
		{
			return GUIUtility.InternalWindowToScreenPoint(GUIClip.UnclipToWindow(guiPoint));
		}

		internal static Rect GUIToScreenRect(Rect guiRect)
		{
			Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
			guiRect.x = vector.x;
			guiRect.y = vector.y;
			return guiRect;
		}

		public static Vector2 ScreenToGUIPoint(Vector2 screenPoint)
		{
			return GUIClip.ClipToWindow(GUIUtility.InternalScreenToWindowPoint(screenPoint));
		}

		public static Rect ScreenToGUIRect(Rect screenRect)
		{
			Vector2 vector = GUIUtility.ScreenToGUIPoint(new Vector2(screenRect.x, screenRect.y));
			screenRect.x = vector.x;
			screenRect.y = vector.y;
			return screenRect;
		}

		public static void RotateAroundPivot(float angle, Vector2 pivotPoint)
		{
			Matrix4x4 matrix = GUI.matrix;
			GUI.matrix = Matrix4x4.identity;
			Vector2 vector = GUIClip.Unclip(pivotPoint);
			Matrix4x4 matrix4x = Matrix4x4.TRS(vector, Quaternion.Euler(0f, 0f, angle), Vector3.one) * Matrix4x4.TRS(-vector, Quaternion.identity, Vector3.one);
			GUI.matrix = matrix4x * matrix;
		}

		public static void ScaleAroundPivot(Vector2 scale, Vector2 pivotPoint)
		{
			Matrix4x4 matrix = GUI.matrix;
			Vector2 vector = GUIClip.Unclip(pivotPoint);
			Matrix4x4 matrix4x = Matrix4x4.TRS(vector, Quaternion.identity, new Vector3(scale.x, scale.y, 1f)) * Matrix4x4.TRS(-vector, Quaternion.identity, Vector3.one);
			GUI.matrix = matrix4x * matrix;
		}

		public static Rect AlignRectToDevice(Rect rect)
		{
			int num;
			int num2;
			return GUIUtility.AlignRectToDevice(rect, out num, out num2);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_s_EditorScreenPointOffset_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_s_EditorScreenPointOffset_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetControlID_Injected(int hint, FocusType focusType, ref Rect rect);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AlignRectToDevice_Injected(ref Rect rect, out int widthInPixels, out int heightInPixels, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_MultiplyPoint_Injected(ref Vector3 point, ref Matrix4x4 transform, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalWindowToScreenPoint_Injected(ref Vector2 windowPoint, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalScreenToWindowPoint_Injected(ref Vector2 screenPoint, out Vector2 ret);

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
		internal static Action enabledStateChanged;
	}
}
