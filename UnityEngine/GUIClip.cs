using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal sealed class GUIClip
	{
		internal static void Push(Rect screenRect, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
		{
			GUIClip.Internal_Push(screenRect, scrollOffset, renderOffset, resetOffset);
		}

		internal static void Pop()
		{
			GUIClip.Internal_Pop();
		}

		public static Vector2 Unclip(Vector2 pos)
		{
			GUIClip.Unclip_Vector2(ref pos);
			return pos;
		}

		public static Rect Unclip(Rect rect)
		{
			GUIClip.Unclip_Rect(ref rect);
			return rect;
		}

		public static Vector2 Clip(Vector2 absolutePos)
		{
			GUIClip.Clip_Vector2(ref absolutePos);
			return absolutePos;
		}

		public static Rect Clip(Rect absoluteRect)
		{
			GUIClip.Internal_Clip_Rect(ref absoluteRect);
			return absoluteRect;
		}

		public static Vector2 GetAbsoluteMousePosition()
		{
			Vector2 vector;
			GUIClip.Internal_GetAbsoluteMousePosition(out vector);
			return vector;
		}

		internal static void Internal_Push(Rect screenRect, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
		{
			GUIClip.INTERNAL_CALL_Internal_Push(ref screenRect, ref scrollOffset, ref renderOffset, resetOffset);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_Push(ref Rect screenRect, ref Vector2 scrollOffset, ref Vector2 renderOffset, bool resetOffset);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_Pop();

		internal static Rect GetTopRect()
		{
			Rect rect;
			GUIClip.INTERNAL_CALL_GetTopRect(out rect);
			return rect;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTopRect(out Rect value);

		public static extern bool enabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private static void Unclip_Vector2(ref Vector2 pos)
		{
			GUIClip.INTERNAL_CALL_Unclip_Vector2(ref pos);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Unclip_Vector2(ref Vector2 pos);

		public static Rect topmostRect
		{
			get
			{
				Rect rect;
				GUIClip.INTERNAL_get_topmostRect(out rect);
				return rect;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_topmostRect(out Rect value);

		private static void Unclip_Rect(ref Rect rect)
		{
			GUIClip.INTERNAL_CALL_Unclip_Rect(ref rect);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Unclip_Rect(ref Rect rect);

		private static void Clip_Vector2(ref Vector2 absolutePos)
		{
			GUIClip.INTERNAL_CALL_Clip_Vector2(ref absolutePos);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Clip_Vector2(ref Vector2 absolutePos);

		private static void Internal_Clip_Rect(ref Rect absoluteRect)
		{
			GUIClip.INTERNAL_CALL_Internal_Clip_Rect(ref absoluteRect);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_Clip_Rect(ref Rect absoluteRect);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Reapply();

		internal static Matrix4x4 GetMatrix()
		{
			Matrix4x4 matrix4x;
			GUIClip.INTERNAL_CALL_GetMatrix(out matrix4x);
			return matrix4x;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetMatrix(out Matrix4x4 value);

		internal static void SetMatrix(Matrix4x4 m)
		{
			GUIClip.INTERNAL_CALL_SetMatrix(ref m);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetMatrix(ref Matrix4x4 m);

		internal static void SetTransform(Matrix4x4 clipTransform, Matrix4x4 objectTransform, Rect clipRect)
		{
			GUIClip.INTERNAL_CALL_SetTransform(ref clipTransform, ref objectTransform, ref clipRect);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTransform(ref Matrix4x4 clipTransform, ref Matrix4x4 objectTransform, ref Rect clipRect);

		public static Rect visibleRect
		{
			get
			{
				Rect rect;
				GUIClip.INTERNAL_get_visibleRect(out rect);
				return rect;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_visibleRect(out Rect value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetAbsoluteMousePosition(out Vector2 output);
	}
}
