using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Scales render textures to support dynamic resolution if the target platform/graphics API supports it.</para>
	/// </summary>
	[NativeHeader("Runtime/GfxDevice/ScalableBufferManager.h")]
	[StaticAccessor("ScalableBufferManager::GetInstance()", StaticAccessorType.Dot)]
	public static class ScalableBufferManager
	{
		/// <summary>
		///   <para>Width scale factor to control dynamic resolution.</para>
		/// </summary>
		public static extern float widthScaleFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Height scale factor to control dynamic resolution.</para>
		/// </summary>
		public static extern float heightScaleFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Function to resize all buffers marked as DynamicallyScalable.</para>
		/// </summary>
		/// <param name="widthScale">New scale factor for the width the ScalableBufferManager will use to resize all render textures the user marked as DynamicallyScalable, has to be some value greater than 0.0 and less than or equal to 1.0.</param>
		/// <param name="heightScale">New scale factor for the height the ScalableBufferManager will use to resize all render textures the user marked as DynamicallyScalable, has to be some value greater than 0.0 and less than or equal to 1.0.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResizeBuffers(float widthScale, float heightScale);
	}
}
