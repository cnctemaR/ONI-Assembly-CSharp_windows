using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Rendering
{
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	public static class ShaderWarmup
	{
		[FreeFunction(Name = "ShaderWarmupScripting::WarmupShader")]
		public static void WarmupShader(Shader shader, ShaderWarmupSetup setup)
		{
			ShaderWarmup.WarmupShader_Injected(Object.MarshalledUnityObject.Marshal<Shader>(shader), ref setup);
		}

		[FreeFunction(Name = "ShaderWarmupScripting::WarmupShaderFromCollection")]
		public static void WarmupShaderFromCollection(ShaderVariantCollection collection, Shader shader, ShaderWarmupSetup setup)
		{
			ShaderWarmup.WarmupShaderFromCollection_Injected(Object.MarshalledUnityObject.Marshal<ShaderVariantCollection>(collection), Object.MarshalledUnityObject.Marshal<Shader>(shader), ref setup);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WarmupShader_Injected(IntPtr shader, [In] ref ShaderWarmupSetup setup);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WarmupShaderFromCollection_Injected(IntPtr collection, IntPtr shader, [In] ref ShaderWarmupSetup setup);
	}
}
