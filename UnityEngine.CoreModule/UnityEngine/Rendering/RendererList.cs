using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering
{
	[MovedFrom("UnityEngine.Rendering.RendererUtils")]
	[NativeHeader("Runtime/Graphics/ScriptableRenderLoop/RendererList.h")]
	public struct RendererList
	{
		public extern bool isValid
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal RendererList(UIntPtr ctx, uint indx)
		{
			this.context = ctx;
			this.index = indx;
			this.frame = 0U;
			this.type = 0U;
			this.contextID = 0U;
		}

		internal UIntPtr context;

		internal uint index;

		internal uint frame;

		internal uint type;

		internal uint contextID;

		public static readonly RendererList nullRendererList = new RendererList(UIntPtr.Zero, uint.MaxValue);
	}
}
