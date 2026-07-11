using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Extension methods to the Renderer class, used only for the UpdateGIMaterials method used by the Global Illumination System.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	public static class RendererExtensions
	{
		/// <summary>
		///   <para>Schedules an update of the albedo and emissive Textures of a system that contains the Renderer.</para>
		/// </summary>
		/// <param name="renderer"></param>
		public static void UpdateGIMaterials(this Renderer renderer)
		{
			RendererExtensions.UpdateGIMaterialsForRenderer(renderer);
		}

		[FreeFunction("RendererScripting::UpdateGIMaterialsForRenderer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateGIMaterialsForRenderer(Renderer renderer);
	}
}
