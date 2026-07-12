using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.Rendering.HybridV2
{
	public class HybridV2ShaderReflection
	{
		[FreeFunction("ShaderScripting::GetDOTSInstancingCbuffersPointer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDOTSInstancingCbuffersPointer([NotNull("ArgumentNullException")] Shader shader, ref int cbufferCount);

		[FreeFunction("ShaderScripting::GetDOTSInstancingPropertiesPointer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDOTSInstancingPropertiesPointer([NotNull("ArgumentNullException")] Shader shader, ref int propertyCount);

		[FreeFunction("Shader::GetDOTSReflectionVersionNumber")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetDOTSReflectionVersionNumber();

		public unsafe static NativeArray<DOTSInstancingCbuffer> GetDOTSInstancingCbuffers(Shader shader)
		{
			bool flag = shader == null;
			NativeArray<DOTSInstancingCbuffer> nativeArray;
			if (flag)
			{
				nativeArray = default(NativeArray<DOTSInstancingCbuffer>);
			}
			else
			{
				int num = 0;
				IntPtr dotsinstancingCbuffersPointer = HybridV2ShaderReflection.GetDOTSInstancingCbuffersPointer(shader, ref num);
				bool flag2 = dotsinstancingCbuffersPointer == IntPtr.Zero;
				if (flag2)
				{
					nativeArray = default(NativeArray<DOTSInstancingCbuffer>);
				}
				else
				{
					NativeArray<DOTSInstancingCbuffer> nativeArray2 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<DOTSInstancingCbuffer>((void*)dotsinstancingCbuffersPointer, num, Allocator.Temp);
					nativeArray = nativeArray2;
				}
			}
			return nativeArray;
		}

		public unsafe static NativeArray<DOTSInstancingProperty> GetDOTSInstancingProperties(Shader shader)
		{
			bool flag = shader == null;
			NativeArray<DOTSInstancingProperty> nativeArray;
			if (flag)
			{
				nativeArray = default(NativeArray<DOTSInstancingProperty>);
			}
			else
			{
				int num = 0;
				IntPtr dotsinstancingPropertiesPointer = HybridV2ShaderReflection.GetDOTSInstancingPropertiesPointer(shader, ref num);
				bool flag2 = dotsinstancingPropertiesPointer == IntPtr.Zero;
				if (flag2)
				{
					nativeArray = default(NativeArray<DOTSInstancingProperty>);
				}
				else
				{
					NativeArray<DOTSInstancingProperty> nativeArray2 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<DOTSInstancingProperty>((void*)dotsinstancingPropertiesPointer, num, Allocator.Temp);
					nativeArray = nativeArray2;
				}
			}
			return nativeArray;
		}
	}
}
