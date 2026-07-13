using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[RequireComponent(typeof(Transform), typeof(SkinnedMeshRenderer))]
	[NativeClass("Unity::Cloth")]
	[NativeHeader("Modules/Cloth/Cloth.h")]
	public sealed class Cloth : Component
	{
		public Vector3[] vertices
		{
			[NativeName("GetPositions")]
			get
			{
				Vector3[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					Cloth.get_vertices_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Vector3[] array;
					blittableArrayWrapper.Unmarshal<Vector3>(ref array);
					array2 = array;
				}
				return array2;
			}
		}

		public Vector3[] normals
		{
			[NativeName("GetNormals")]
			get
			{
				Vector3[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					Cloth.get_normals_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Vector3[] array;
					blittableArrayWrapper.Unmarshal<Vector3>(ref array);
					array2 = array;
				}
				return array2;
			}
		}

		public unsafe ClothSkinningCoefficient[] coefficients
		{
			[NativeName("GetCoefficients")]
			get
			{
				ClothSkinningCoefficient[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					Cloth.get_coefficients_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					ClothSkinningCoefficient[] array;
					blittableArrayWrapper.Unmarshal<ClothSkinningCoefficient>(ref array);
					array2 = array;
				}
				return array2;
			}
			[NativeName("SetCoefficients")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<ClothSkinningCoefficient> span = new Span<ClothSkinningCoefficient>(value);
				fixed (ClothSkinningCoefficient* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Cloth.set_coefficients_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		public CapsuleCollider[] capsuleColliders
		{
			[NativeName("GetCapsuleColliders")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_capsuleColliders_Injected(intPtr);
			}
			[NativeName("SetCapsuleColliders")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_capsuleColliders_Injected(intPtr, value);
			}
		}

		public ClothSphereColliderPair[] sphereColliders
		{
			[NativeName("GetSphereColliders")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_sphereColliders_Injected(intPtr);
			}
			[NativeName("SetSphereColliders")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_sphereColliders_Injected(intPtr, value);
			}
		}

		public float sleepThreshold
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_sleepThreshold_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_sleepThreshold_Injected(intPtr, value);
			}
		}

		public float bendingStiffness
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_bendingStiffness_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_bendingStiffness_Injected(intPtr, value);
			}
		}

		public float stretchingStiffness
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_stretchingStiffness_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_stretchingStiffness_Injected(intPtr, value);
			}
		}

		public float damping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_damping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_damping_Injected(intPtr, value);
			}
		}

		public Vector3 externalAcceleration
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Cloth.get_externalAcceleration_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_externalAcceleration_Injected(intPtr, ref value);
			}
		}

		public Vector3 randomAcceleration
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Cloth.get_randomAcceleration_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_randomAcceleration_Injected(intPtr, ref value);
			}
		}

		public bool useGravity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_useGravity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_useGravity_Injected(intPtr, value);
			}
		}

		public bool enabled
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_enabled_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_enabled_Injected(intPtr, value);
			}
		}

		public float friction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_friction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_friction_Injected(intPtr, value);
			}
		}

		public float collisionMassScale
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_collisionMassScale_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_collisionMassScale_Injected(intPtr, value);
			}
		}

		public bool enableContinuousCollision
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_enableContinuousCollision_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_enableContinuousCollision_Injected(intPtr, value);
			}
		}

		public float useVirtualParticles
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_useVirtualParticles_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_useVirtualParticles_Injected(intPtr, value);
			}
		}

		public float worldVelocityScale
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_worldVelocityScale_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_worldVelocityScale_Injected(intPtr, value);
			}
		}

		public float worldAccelerationScale
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_worldAccelerationScale_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_worldAccelerationScale_Injected(intPtr, value);
			}
		}

		public float clothSolverFrequency
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_clothSolverFrequency_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_clothSolverFrequency_Injected(intPtr, value);
			}
		}

		[Obsolete("Parameter solverFrequency is obsolete and no longer supported. Please use clothSolverFrequency instead.")]
		public bool solverFrequency
		{
			get
			{
				return this.clothSolverFrequency > 0f;
			}
			set
			{
				this.clothSolverFrequency = (value ? 120f : 0f);
			}
		}

		public bool useTethers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_useTethers_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_useTethers_Injected(intPtr, value);
			}
		}

		public float stiffnessFrequency
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_stiffnessFrequency_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_stiffnessFrequency_Injected(intPtr, value);
			}
		}

		public float selfCollisionDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_selfCollisionDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_selfCollisionDistance_Injected(intPtr, value);
			}
		}

		public float selfCollisionStiffness
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cloth.get_selfCollisionStiffness_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cloth.set_selfCollisionStiffness_Injected(intPtr, value);
			}
		}

		public void ClearTransformMotion()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Cloth.ClearTransformMotion_Injected(intPtr);
		}

		public unsafe void GetSelfAndInterCollisionIndices([NotNull] List<uint> indices)
		{
			if (indices == null)
			{
				ThrowHelper.ThrowArgumentNullException(indices, "indices");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (uint[] array = NoAllocHelpers.ExtractArrayFromList<uint>(indices))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, indices.Count);
					Cloth.GetSelfAndInterCollisionIndices_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<uint>(indices);
			}
		}

		public unsafe void SetSelfAndInterCollisionIndices([NotNull] List<uint> indices)
		{
			if (indices == null)
			{
				ThrowHelper.ThrowArgumentNullException(indices, "indices");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (uint[] array = NoAllocHelpers.ExtractArrayFromList<uint>(indices))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, indices.Count);
					Cloth.SetSelfAndInterCollisionIndices_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<uint>(indices);
			}
		}

		public unsafe void GetVirtualParticleIndices([NotNull] List<uint> indicesOutList)
		{
			if (indicesOutList == null)
			{
				ThrowHelper.ThrowArgumentNullException(indicesOutList, "indicesOutList");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (uint[] array = NoAllocHelpers.ExtractArrayFromList<uint>(indicesOutList))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, indicesOutList.Count);
					Cloth.GetVirtualParticleIndices_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<uint>(indicesOutList);
			}
		}

		public unsafe void SetVirtualParticleIndices([NotNull] List<uint> indicesIn)
		{
			if (indicesIn == null)
			{
				ThrowHelper.ThrowArgumentNullException(indicesIn, "indicesIn");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (uint[] array = NoAllocHelpers.ExtractArrayFromList<uint>(indicesIn))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, indicesIn.Count);
					Cloth.SetVirtualParticleIndices_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<uint>(indicesIn);
			}
		}

		public unsafe void GetVirtualParticleWeights([NotNull] List<Vector3> weightsOutList)
		{
			if (weightsOutList == null)
			{
				ThrowHelper.ThrowArgumentNullException(weightsOutList, "weightsOutList");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (Vector3[] array = NoAllocHelpers.ExtractArrayFromList<Vector3>(weightsOutList))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, weightsOutList.Count);
					Cloth.GetVirtualParticleWeights_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Vector3>(weightsOutList);
			}
		}

		public unsafe void SetVirtualParticleWeights([NotNull] List<Vector3> weights)
		{
			if (weights == null)
			{
				ThrowHelper.ThrowArgumentNullException(weights, "weights");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (Vector3[] array = NoAllocHelpers.ExtractArrayFromList<Vector3>(weights))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, weights.Count);
					Cloth.SetVirtualParticleWeights_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Vector3>(weights);
			}
		}

		[Obsolete("useContinuousCollision is no longer supported, use enableContinuousCollision instead")]
		public float useContinuousCollision { get; set; }

		[Obsolete("Deprecated.Cloth.selfCollisions is no longer supported since Unity 5.0.", true)]
		public bool selfCollision { get; }

		public void SetEnabledFading(bool enabled, float interpolationTime)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cloth>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Cloth.SetEnabledFading_Injected(intPtr, enabled, interpolationTime);
		}

		[ExcludeFromDocs]
		public void SetEnabledFading(bool enabled)
		{
			this.SetEnabledFading(enabled, 0.5f);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_vertices_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_normals_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_coefficients_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_coefficients_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CapsuleCollider[] get_capsuleColliders_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_capsuleColliders_Injected(IntPtr _unity_self, CapsuleCollider[] value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ClothSphereColliderPair[] get_sphereColliders_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sphereColliders_Injected(IntPtr _unity_self, ClothSphereColliderPair[] value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_sleepThreshold_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sleepThreshold_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_bendingStiffness_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bendingStiffness_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_stretchingStiffness_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_stretchingStiffness_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_damping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_damping_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_externalAcceleration_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_externalAcceleration_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_randomAcceleration_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_randomAcceleration_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useGravity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useGravity_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enabled_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enabled_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_friction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_friction_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_collisionMassScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_collisionMassScale_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableContinuousCollision_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enableContinuousCollision_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_useVirtualParticles_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useVirtualParticles_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_worldVelocityScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_worldVelocityScale_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_worldAccelerationScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_worldAccelerationScale_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_clothSolverFrequency_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_clothSolverFrequency_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useTethers_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useTethers_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_stiffnessFrequency_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_stiffnessFrequency_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_selfCollisionDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_selfCollisionDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_selfCollisionStiffness_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_selfCollisionStiffness_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearTransformMotion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSelfAndInterCollisionIndices_Injected(IntPtr _unity_self, ref BlittableListWrapper indices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSelfAndInterCollisionIndices_Injected(IntPtr _unity_self, ref BlittableListWrapper indices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVirtualParticleIndices_Injected(IntPtr _unity_self, ref BlittableListWrapper indicesOutList);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVirtualParticleIndices_Injected(IntPtr _unity_self, ref BlittableListWrapper indicesIn);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVirtualParticleWeights_Injected(IntPtr _unity_self, ref BlittableListWrapper weightsOutList);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVirtualParticleWeights_Injected(IntPtr _unity_self, ref BlittableListWrapper weights);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetEnabledFading_Injected(IntPtr _unity_self, bool enabled, float interpolationTime);
	}
}
