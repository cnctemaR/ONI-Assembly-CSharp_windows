using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[StaticAccessor("ScalableBufferManager::GetInstance()", StaticAccessorType.Dot)]
	[NativeHeader("Runtime/GfxDevice/ScalableBufferManager.h")]
	public static class ScalableBufferManager
	{
		public static extern float widthScaleFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float heightScaleFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResizeBuffers(float widthScale, float heightScale);
	}
}
