using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/RenderAs2D/Public/RenderAs2DUtil.h")]
	internal struct RenderAs2DUtil
	{
		[FreeFunction("RenderAs2DUtil::InitializeCanRenderAs2D")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InitializeCanRenderAs2D();

		[FreeFunction("RenderAs2DUtil::DisposeCanRenderAs2D")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DisposeCanRenderAs2D();
	}
}
