using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeClass("Unity::Cloth")]
	[NativeHeader("Modules/Cloth/Cloth.h")]
	[RequireComponent(typeof(Transform), typeof(SkinnedMeshRenderer))]
	public sealed class Cloth : Component
	{
		public extern Vector3[] vertices
		{
			[NativeName("GetPositions")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Vector3[] normals
		{
			[NativeName("GetNormals")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ClothSkinningCoefficient[] coefficients
		{
			[NativeName("GetCoefficients")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetCoefficients")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CapsuleCollider[] capsuleColliders
		{
			[NativeName("GetCapsuleColliders")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetCapsuleColliders")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ClothSphereColliderPair[] sphereColliders
		{
			[NativeName("GetSphereColliders")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetSphereColliders")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float sleepThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float bendingStiffness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float stretchingStiffness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float damping
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 externalAcceleration
		{
			get
			{
				Vector3 vector;
				this.get_externalAcceleration_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_externalAcceleration_Injected(ref value);
			}
		}

		public Vector3 randomAcceleration
		{
			get
			{
				Vector3 vector;
				this.get_randomAcceleration_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_randomAcceleration_Injected(ref value);
			}
		}

		public extern bool useGravity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float friction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float collisionMassScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enableContinuousCollision
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float useVirtualParticles
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float worldVelocityScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float worldAccelerationScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float clothSolverFrequency
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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

		public extern bool useTethers
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float stiffnessFrequency
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float selfCollisionDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float selfCollisionStiffness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearTransformMotion();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern uint[] GetSelfAndInterCollisionIndices();

		internal void Internal_GetSelfAndInterCollisionIndices(List<uint> indicesOutList)
		{
			uint[] selfAndInterCollisionIndices = this.GetSelfAndInterCollisionIndices();
			indicesOutList.Clear();
			indicesOutList.AddRange(selfAndInterCollisionIndices);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSelfAndInterCollisionIndices(uint[] indicesIn);

		internal void Internal_SetSelfAndInterCollisionIndices(List<uint> indicesInList)
		{
			this.SetSelfAndInterCollisionIndices(indicesInList.ToArray());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern uint[] GetVirtualParticleIndices();

		internal void Internal_GetVirtualParticleIndices(List<uint> indicesOutList)
		{
			uint[] virtualParticleIndices = this.GetVirtualParticleIndices();
			indicesOutList.Clear();
			indicesOutList.AddRange(virtualParticleIndices);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVirtualParticleIndices(uint[] indicesIn);

		internal void Internal_SetVirtualParticleIndices(List<uint> indicesInList)
		{
			this.SetVirtualParticleIndices(indicesInList.ToArray());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Vector3[] GetVirtualParticleWeights();

		internal void Internal_GetVirtualParticleWeights(List<Vector3> weightsOutList)
		{
			Vector3[] virtualParticleWeights = this.GetVirtualParticleWeights();
			weightsOutList.Clear();
			weightsOutList.AddRange(virtualParticleWeights);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVirtualParticleWeights(Vector3[] weightsIn);

		internal void Internal_SetVirtualParticleWeights(List<Vector3> weightsInList)
		{
			this.SetVirtualParticleWeights(weightsInList.ToArray());
		}

		[Obsolete("useContinuousCollision is no longer supported, use enableContinuousCollision instead")]
		public float useContinuousCollision { get; set; }

		[Obsolete("Deprecated.Cloth.selfCollisions is no longer supported since Unity 5.0.", true)]
		public bool selfCollision { get; }

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetEnabledFading(bool enabled, float interpolationTime);

		[ExcludeFromDocs]
		public void SetEnabledFading(bool enabled)
		{
			this.SetEnabledFading(enabled, 0.5f);
		}

		private RaycastHit Raycast(Ray ray, float maxDistance, ref bool hasHit)
		{
			RaycastHit raycastHit;
			this.Raycast_Injected(ref ray, maxDistance, ref hasHit, out raycastHit);
			return raycastHit;
		}

		internal bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
		{
			bool flag = false;
			hitInfo = this.Raycast(ray, maxDistance, ref flag);
			return flag;
		}

		public void GetVirtualParticleIndices(List<uint> indices)
		{
			bool flag = indices == null;
			if (flag)
			{
				throw new ArgumentNullException("indices");
			}
			this.Internal_GetVirtualParticleIndices(indices);
		}

		public void SetVirtualParticleIndices(List<uint> indices)
		{
			bool flag = indices == null;
			if (flag)
			{
				throw new ArgumentNullException("indices");
			}
			this.Internal_SetVirtualParticleIndices(indices);
		}

		public void GetVirtualParticleWeights(List<Vector3> weights)
		{
			bool flag = weights == null;
			if (flag)
			{
				throw new ArgumentNullException("weights");
			}
			this.Internal_GetVirtualParticleWeights(weights);
		}

		public void SetVirtualParticleWeights(List<Vector3> weights)
		{
			bool flag = weights == null;
			if (flag)
			{
				throw new ArgumentNullException("weights");
			}
			this.Internal_SetVirtualParticleWeights(weights);
		}

		public void GetSelfAndInterCollisionIndices(List<uint> indices)
		{
			bool flag = indices == null;
			if (flag)
			{
				throw new ArgumentNullException("indices");
			}
			this.Internal_GetSelfAndInterCollisionIndices(indices);
		}

		public void SetSelfAndInterCollisionIndices(List<uint> indices)
		{
			bool flag = indices == null;
			if (flag)
			{
				throw new ArgumentNullException("indices");
			}
			this.Internal_SetSelfAndInterCollisionIndices(indices);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_externalAcceleration_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_externalAcceleration_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_randomAcceleration_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_randomAcceleration_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Raycast_Injected(ref Ray ray, float maxDistance, ref bool hasHit, out RaycastHit ret);
	}
}
