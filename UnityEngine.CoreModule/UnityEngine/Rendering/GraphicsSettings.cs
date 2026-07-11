using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Script interface for.</para>
	/// </summary>
	[NativeHeader("Runtime/Camera/GraphicsSettings.h")]
	[StaticAccessor("GetGraphicsSettings()", StaticAccessorType.Dot)]
	public sealed class GraphicsSettings : Object
	{
		private GraphicsSettings()
		{
		}

		/// <summary>
		///   <para>Transparent object sorting mode.</para>
		/// </summary>
		public static extern TransparencySortMode transparencySortMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>An axis that describes the direction along which the distances of objects are measured for the purpose of sorting.</para>
		/// </summary>
		public static Vector3 transparencySortAxis
		{
			get
			{
				Vector3 vector;
				GraphicsSettings.get_transparencySortAxis_Injected(out vector);
				return vector;
			}
			set
			{
				GraphicsSettings.set_transparencySortAxis_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>If this is true, Light intensity is multiplied against linear color values. If it is false, gamma color values are used.</para>
		/// </summary>
		public static extern bool lightsUseLinearIntensity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Whether to use a Light's color temperature when calculating the final color of that Light."</para>
		/// </summary>
		public static extern bool lightsUseColorTemperature
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enable/Disable SRP batcher (experimental) at runtime.</para>
		/// </summary>
		public static extern bool useScriptableRenderPipelineBatching
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns true if shader define was set when compiling shaders for current GraphicsTier.</para>
		/// </summary>
		/// <param name="tier"></param>
		/// <param name="defineHash"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasShaderDefine(GraphicsTier tier, BuiltinShaderDefine defineHash);

		/// <summary>
		///   <para>Returns true if shader define was set when compiling shaders for given tier.</para>
		/// </summary>
		/// <param name="defineHash"></param>
		public static bool HasShaderDefine(BuiltinShaderDefine defineHash)
		{
			return GraphicsSettings.HasShaderDefine(Graphics.activeTier, defineHash);
		}

		[NativeName("RenderPipeline")]
		private static extern ScriptableObject INTERNAL_renderPipelineAsset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The RenderPipelineAsset that describes how the Scene should be rendered.</para>
		/// </summary>
		public static RenderPipelineAsset renderPipelineAsset
		{
			get
			{
				return GraphicsSettings.INTERNAL_renderPipelineAsset as RenderPipelineAsset;
			}
			set
			{
				GraphicsSettings.INTERNAL_renderPipelineAsset = value;
			}
		}

		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Object GetGraphicsSettings();

		/// <summary>
		///   <para>Set built-in shader mode.</para>
		/// </summary>
		/// <param name="type">Built-in shader type to change.</param>
		/// <param name="mode">Mode to use for built-in shader.</param>
		[NativeName("SetShaderModeScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShaderMode(BuiltinShaderType type, BuiltinShaderMode mode);

		/// <summary>
		///   <para>Get built-in shader mode.</para>
		/// </summary>
		/// <param name="type">Built-in shader type to query.</param>
		/// <returns>
		///   <para>Mode used for built-in shader.</para>
		/// </returns>
		[NativeName("GetShaderModeScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern BuiltinShaderMode GetShaderMode(BuiltinShaderType type);

		/// <summary>
		///   <para>Set custom shader to use instead of a built-in shader.</para>
		/// </summary>
		/// <param name="type">Built-in shader type to set custom shader to.</param>
		/// <param name="shader">The shader to use.</param>
		[NativeName("SetCustomShaderScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCustomShader(BuiltinShaderType type, Shader shader);

		/// <summary>
		///   <para>Get custom shader used instead of a built-in shader.</para>
		/// </summary>
		/// <param name="type">Built-in shader type to query custom shader for.</param>
		/// <returns>
		///   <para>The shader used.</para>
		/// </returns>
		[NativeName("GetCustomShaderScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Shader GetCustomShader(BuiltinShaderType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_transparencySortAxis_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_transparencySortAxis_Injected(ref Vector3 value);
	}
}
