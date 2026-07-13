using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	public struct RenderBuffer
	{
		[FreeFunction(Name = "RenderBufferScripting::SetLoadAction", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetLoadAction(RenderBufferLoadAction action);

		[FreeFunction(Name = "RenderBufferScripting::SetStoreAction", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetStoreAction(RenderBufferStoreAction action);

		[FreeFunction(Name = "RenderBufferScripting::GetLoadAction", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern RenderBufferLoadAction GetLoadAction();

		[FreeFunction(Name = "RenderBufferScripting::GetStoreAction", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern RenderBufferStoreAction GetStoreAction();

		[FreeFunction(Name = "RenderBufferScripting::GetNativeRenderBufferPtr", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern IntPtr GetNativeRenderBufferPtr();

		internal RenderBufferLoadAction loadAction
		{
			get
			{
				return this.GetLoadAction();
			}
			set
			{
				this.SetLoadAction(value);
			}
		}

		internal RenderBufferStoreAction storeAction
		{
			get
			{
				return this.GetStoreAction();
			}
			set
			{
				this.SetStoreAction(value);
			}
		}

		internal int m_RenderTextureInstanceID;

		internal IntPtr m_BufferPtr;
	}
}
