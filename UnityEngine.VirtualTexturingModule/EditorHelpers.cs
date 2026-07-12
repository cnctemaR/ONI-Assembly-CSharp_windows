using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.VirtualTexturing
{
	[NativeConditional("UNITY_EDITOR")]
	[StaticAccessor("VirtualTexturing::Editor", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
	public static class EditorHelpers
	{
		[NativeThrows]
		internal static extern int tileSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ValidateTextureStack([NotNull("ArgumentNullException")] Texture[] textures, out string errorMessage);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern EditorHelpers.StackValidationResult[] ValidateMaterialTextureStacks([NotNull("ArgumentNullException")] Material mat);

		[NativeConditional("UNITY_EDITOR", "{}")]
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GraphicsFormat[] QuerySupportedFormats();

		[NativeHeader("Runtime/Shaders/SharedMaterialData.h")]
		internal struct StackValidationResult
		{
			public string stackName;

			public string errorMessage;
		}
	}
}
