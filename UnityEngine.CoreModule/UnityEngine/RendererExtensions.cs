using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	public static class RendererExtensions
	{
		public static void UpdateGIMaterials(this Renderer renderer)
		{
			RendererExtensions.UpdateGIMaterialsForRenderer(renderer);
		}

		[FreeFunction("RendererScripting::UpdateGIMaterialsForRenderer")]
		internal static void UpdateGIMaterialsForRenderer(Renderer renderer)
		{
			RendererExtensions.UpdateGIMaterialsForRenderer_Injected(Object.MarshalledUnityObject.Marshal<Renderer>(renderer));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateGIMaterialsForRenderer_Injected(IntPtr renderer);
	}
}
