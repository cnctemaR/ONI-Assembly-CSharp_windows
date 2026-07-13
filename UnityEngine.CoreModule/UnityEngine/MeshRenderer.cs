using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/Mesh/MeshRenderer.h")]
	public class MeshRenderer : Renderer
	{
		[RequiredByNativeCode]
		private void DontStripMeshRenderer()
		{
		}

		public Mesh additionalVertexStreams
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Mesh>(MeshRenderer.get_additionalVertexStreams_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				MeshRenderer.set_additionalVertexStreams_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Mesh>(value));
			}
		}

		public Mesh enlightenVertexStream
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Mesh>(MeshRenderer.get_enlightenVertexStream_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				MeshRenderer.set_enlightenVertexStream_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Mesh>(value));
			}
		}

		public int subMeshStartIndex
		{
			[NativeName("GetSubMeshStartIndex")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return MeshRenderer.get_subMeshStartIndex_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "MeshRendererScripting::SetShaderUserValue", HasExplicitThis = true)]
		internal void Internal_SetShaderUserValueUInt(uint v)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MeshRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MeshRenderer.Internal_SetShaderUserValueUInt_Injected(intPtr, v);
		}

		public void SetShaderUserValue(uint v)
		{
			this.Internal_SetShaderUserValueUInt(v);
		}

		[FreeFunction(Name = "MeshRendererScripting::GetShaderUserValue", HasExplicitThis = true)]
		internal uint Internal_GetShaderUserValueUInt()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<MeshRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return MeshRenderer.Internal_GetShaderUserValueUInt_Injected(intPtr);
		}

		public uint GetShaderUserValue()
		{
			return this.Internal_GetShaderUserValueUInt();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_additionalVertexStreams_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_additionalVertexStreams_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_enlightenVertexStream_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enlightenVertexStream_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_subMeshStartIndex_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetShaderUserValueUInt_Injected(IntPtr _unity_self, uint v);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint Internal_GetShaderUserValueUInt_Injected(IntPtr _unity_self);
	}
}
