using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Sprites
{
	public sealed class DataUtility
	{
		public static Vector4 GetInnerUV(Sprite sprite)
		{
			Vector4 vector;
			DataUtility.INTERNAL_CALL_GetInnerUV(sprite, out vector);
			return vector;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetInnerUV(Sprite sprite, out Vector4 value);

		public static Vector4 GetOuterUV(Sprite sprite)
		{
			Vector4 vector;
			DataUtility.INTERNAL_CALL_GetOuterUV(sprite, out vector);
			return vector;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetOuterUV(Sprite sprite, out Vector4 value);

		public static Vector4 GetPadding(Sprite sprite)
		{
			Vector4 vector;
			DataUtility.INTERNAL_CALL_GetPadding(sprite, out vector);
			return vector;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPadding(Sprite sprite, out Vector4 value);

		public static Vector2 GetMinSize(Sprite sprite)
		{
			Vector2 vector;
			DataUtility.Internal_GetMinSize(sprite, out vector);
			return vector;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetMinSize(Sprite sprite, out Vector2 output);
	}
}
