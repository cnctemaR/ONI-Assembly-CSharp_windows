using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Shaders
{
	[NativeHeader("Modules/ShaderRuntime/Public/ShaderTypes.h")]
	public sealed class Utility
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsShaderStageEnabled(ShaderStageFlags flags, ShaderStage stage);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsShaderTypeEnabled(ShaderTypeFlags flags, ShaderType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ShaderStageFlags ShaderStageToFlags(ShaderStage stage);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ShaderTypeFlags ShaderTypeToFlags(ShaderType type);
	}
}
