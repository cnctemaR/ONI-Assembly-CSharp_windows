using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/IMGUI/GUIDebugger.bindings.h")]
	internal class GUIDebugger
	{
		[NativeConditional("UNITY_EDITOR")]
		public static void LogLayoutEntry(Rect rect, int left, int right, int top, int bottom, GUIStyle style)
		{
			GUIDebugger.LogLayoutEntry_Injected(ref rect, left, right, top, bottom, (style == null) ? ((IntPtr)0) : GUIStyle.BindingsMarshaller.ConvertToNative(style));
		}

		[NativeConditional("UNITY_EDITOR")]
		public static void LogLayoutGroupEntry(Rect rect, int left, int right, int top, int bottom, GUIStyle style, bool isVertical)
		{
			GUIDebugger.LogLayoutGroupEntry_Injected(ref rect, left, right, top, bottom, (style == null) ? ((IntPtr)0) : GUIStyle.BindingsMarshaller.ConvertToNative(style), isVertical);
		}

		[NativeMethod("LogEndGroup")]
		[NativeConditional("UNITY_EDITOR")]
		[StaticAccessor("GetGUIDebuggerManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LogLayoutEndGroup();

		[StaticAccessor("GetGUIDebuggerManager()", StaticAccessorType.Dot)]
		[NativeConditional("UNITY_EDITOR")]
		public unsafe static void LogBeginProperty(string targetTypeAssemblyQualifiedName, string path, Rect position)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(targetTypeAssemblyQualifiedName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = targetTypeAssemblyQualifiedName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = path.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				GUIDebugger.LogBeginProperty_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, ref position);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[StaticAccessor("GetGUIDebuggerManager()", StaticAccessorType.Dot)]
		[NativeConditional("UNITY_EDITOR")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LogEndProperty();

		[NativeConditional("UNITY_EDITOR")]
		public static extern bool active
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LogLayoutEntry_Injected([In] ref Rect rect, int left, int right, int top, int bottom, IntPtr style);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LogLayoutGroupEntry_Injected([In] ref Rect rect, int left, int right, int top, int bottom, IntPtr style, bool isVertical);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LogBeginProperty_Injected(ref ManagedSpanWrapper targetTypeAssemblyQualifiedName, ref ManagedSpanWrapper path, [In] ref Rect position);
	}
}
