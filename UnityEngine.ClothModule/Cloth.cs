using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequireComponent(typeof(Transform), typeof(SkinnedMeshRenderer))]
	[NativeHeader("Runtime/Cloth/Cloth.h")]
	[NativeClass("Unity::Cloth")]
	public sealed class Cloth : Component
	{
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

		public void GetVirtualParticleIndices(List<uint> indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			this.GetVirtualParticleIndicesMono(indices);
		}

		public void SetVirtualParticleIndices(List<uint> indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			this.SetVirtualParticleIndicesMono(indices);
		}

		public void GetVirtualParticleWeights(List<Vector3> weights)
		{
			if (weights == null)
			{
				throw new ArgumentNullException("weights");
			}
			this.GetVirtualParticleWeightsMono(weights);
		}

		public void SetVirtualParticleWeights(List<Vector3> weights)
		{
			if (weights == null)
			{
				throw new ArgumentNullException("weights");
			}
			this.SetVirtualParticleWeightsMono(weights);
		}

		public void GetSelfAndInterCollisionIndices(List<uint> indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			this.GetSelfAndInterCollisionIndicesMono(indices);
		}

		public void SetSelfAndInterCollisionIndices(List<uint> indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			this.SetSelfAndInterCollisionIndicesMono(indices);
		}

		[Obsolete("Deprecated. Cloth.selfCollisions is no longer supported since Unity 5.0.", true)]
		public extern bool selfCollision
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Vector3[] vertices
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Vector3[] normals
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("useContinuousCollision is no longer supported, use enableContinuousCollision instead")]
		public extern float useContinuousCollision
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public void ClearTransformMotion()
		{
			Cloth.INTERNAL_CALL_ClearTransformMotion(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ClearTransformMotion(Cloth self);

		public extern ClothSkinningCoefficient[] coefficients
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetEnabledFading(bool enabled, [DefaultValue("0.5f")] float interpolationTime);

		[ExcludeFromDocs]
		public void SetEnabledFading(bool enabled)
		{
			float num = 0.5f;
			this.SetEnabledFading(enabled, num);
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
				this.clothSolverFrequency = ((!value) ? 0f : 120f);
			}
		}

		public extern CapsuleCollider[] capsuleColliders
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ClothSphereColliderPair[] sphereColliders
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetVirtualParticleIndicesMono(object indicesOutList);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetVirtualParticleIndicesMono(object indicesInList);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetVirtualParticleWeightsMono(object weightsOutList);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetVirtualParticleWeightsMono(object weightsInList);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetSelfAndInterCollisionIndicesMono(object indicesOutList);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetSelfAndInterCollisionIndicesMono(object indicesInList);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_externalAcceleration_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_externalAcceleration_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_randomAcceleration_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_randomAcceleration_Injected(ref Vector3 value);
	}
}
