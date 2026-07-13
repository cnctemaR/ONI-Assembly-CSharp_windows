using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/Mesh/SkinnedMeshRenderer.h")]
	[RequiredByNativeCode]
	public class SkinnedMeshRenderer : Renderer
	{
		public SkinQuality quality
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SkinnedMeshRenderer.get_quality_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SkinnedMeshRenderer.set_quality_Injected(intPtr, value);
			}
		}

		public bool updateWhenOffscreen
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SkinnedMeshRenderer.get_updateWhenOffscreen_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SkinnedMeshRenderer.set_updateWhenOffscreen_Injected(intPtr, value);
			}
		}

		public bool forceMatrixRecalculationPerRender
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SkinnedMeshRenderer.get_forceMatrixRecalculationPerRender_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SkinnedMeshRenderer.set_forceMatrixRecalculationPerRender_Injected(intPtr, value);
			}
		}

		public Transform rootBone
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Transform>(SkinnedMeshRenderer.get_rootBone_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SkinnedMeshRenderer.set_rootBone_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Transform>(value));
			}
		}

		public Transform[] bones
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SkinnedMeshRenderer.get_bones_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SkinnedMeshRenderer.set_bones_Injected(intPtr, value);
			}
		}

		[NativeProperty("Mesh")]
		public Mesh sharedMesh
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Mesh>(SkinnedMeshRenderer.get_sharedMesh_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SkinnedMeshRenderer.set_sharedMesh_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Mesh>(value));
			}
		}

		[NativeProperty("SkinnedMeshMotionVectors")]
		public bool skinnedMotionVectors
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SkinnedMeshRenderer.get_skinnedMotionVectors_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SkinnedMeshRenderer.set_skinnedMotionVectors_Injected(intPtr, value);
			}
		}

		public float GetBlendShapeWeight(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return SkinnedMeshRenderer.GetBlendShapeWeight_Injected(intPtr, index);
		}

		public void SetBlendShapeWeight(int index, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			SkinnedMeshRenderer.SetBlendShapeWeight_Injected(intPtr, index, value);
		}

		public void BakeMesh(Mesh mesh)
		{
			this.BakeMesh(mesh, false);
		}

		public void BakeMesh([NotNull] Mesh mesh, bool useScale)
		{
			if (mesh == null)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(mesh);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			SkinnedMeshRenderer.BakeMesh_Injected(intPtr, intPtr2, useScale);
		}

		public GraphicsBuffer GetVertexBuffer()
		{
			bool flag = this == null;
			if (flag)
			{
				throw new NullReferenceException();
			}
			return this.GetVertexBufferImpl();
		}

		public GraphicsBuffer GetPreviousVertexBuffer()
		{
			bool flag = this == null;
			if (flag)
			{
				throw new NullReferenceException();
			}
			return this.GetPreviousVertexBufferImpl();
		}

		[FreeFunction(Name = "SkinnedMeshRendererScripting::GetVertexBufferPtr", HasExplicitThis = true)]
		private GraphicsBuffer GetVertexBufferImpl()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr vertexBufferImpl_Injected = SkinnedMeshRenderer.GetVertexBufferImpl_Injected(intPtr);
			return (vertexBufferImpl_Injected == 0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(vertexBufferImpl_Injected);
		}

		[FreeFunction(Name = "SkinnedMeshRendererScripting::GetPreviousVertexBufferPtr", HasExplicitThis = true)]
		private GraphicsBuffer GetPreviousVertexBufferImpl()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr previousVertexBufferImpl_Injected = SkinnedMeshRenderer.GetPreviousVertexBufferImpl_Injected(intPtr);
			return (previousVertexBufferImpl_Injected == 0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(previousVertexBufferImpl_Injected);
		}

		public GraphicsBuffer.Target vertexBufferTarget
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SkinnedMeshRenderer.get_vertexBufferTarget_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SkinnedMeshRenderer.set_vertexBufferTarget_Injected(intPtr, value);
			}
		}

		[FreeFunction(Name = "SkinnedMeshRendererScripting::SetShaderUserValue", HasExplicitThis = true)]
		internal void Internal_SetShaderUserValueUInt(uint v)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			SkinnedMeshRenderer.Internal_SetShaderUserValueUInt_Injected(intPtr, v);
		}

		public void SetShaderUserValue(uint v)
		{
			this.Internal_SetShaderUserValueUInt(v);
		}

		[FreeFunction(Name = "SkinnedMeshRendererScripting::GetShaderUserValue", HasExplicitThis = true)]
		internal uint Internal_GetShaderUserValueUInt()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SkinnedMeshRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return SkinnedMeshRenderer.Internal_GetShaderUserValueUInt_Injected(intPtr);
		}

		public uint GetShaderUserValue()
		{
			return this.Internal_GetShaderUserValueUInt();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SkinQuality get_quality_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_quality_Injected(IntPtr _unity_self, SkinQuality value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_updateWhenOffscreen_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_updateWhenOffscreen_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_forceMatrixRecalculationPerRender_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_forceMatrixRecalculationPerRender_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_rootBone_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rootBone_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Transform[] get_bones_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bones_Injected(IntPtr _unity_self, Transform[] value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_sharedMesh_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sharedMesh_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_skinnedMotionVectors_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_skinnedMotionVectors_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetBlendShapeWeight_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBlendShapeWeight_Injected(IntPtr _unity_self, int index, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BakeMesh_Injected(IntPtr _unity_self, IntPtr mesh, bool useScale);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetVertexBufferImpl_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetPreviousVertexBufferImpl_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsBuffer.Target get_vertexBufferTarget_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_vertexBufferTarget_Injected(IntPtr _unity_self, GraphicsBuffer.Target value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetShaderUserValueUInt_Injected(IntPtr _unity_self, uint v);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint Internal_GetShaderUserValueUInt_Injected(IntPtr _unity_self);
	}
}
