using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>The Cloth class provides an interface to cloth simulation physics.</para>
	/// </summary>
	[NativeClass("Unity::Cloth")]
	[RequireComponent(typeof(Transform), typeof(SkinnedMeshRenderer))]
	[NativeHeader("Runtime/Cloth/Cloth.h")]
	public sealed class Cloth : Component
	{
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

		/// <summary>
		///   <para>The current vertex positions of the cloth object.</para>
		/// </summary>
		public extern Vector3[] vertices
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The current normals of the cloth object.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Clear the pending transform changes from affecting the cloth simulation.</para>
		/// </summary>
		public void ClearTransformMotion()
		{
			Cloth.INTERNAL_CALL_ClearTransformMotion(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ClearTransformMotion(Cloth self);

		/// <summary>
		///   <para>The cloth skinning coefficients used to set up how the cloth interacts with the skinned mesh.</para>
		/// </summary>
		public extern ClothSkinningCoefficient[] coefficients
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Fade the cloth simulation in or out.</para>
		/// </summary>
		/// <param name="enabled">Fading enabled or not.</param>
		/// <param name="interpolationTime"></param>
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

		/// <summary>
		///   <para>An array of CapsuleColliders which this Cloth instance should collide with.</para>
		/// </summary>
		public extern CapsuleCollider[] capsuleColliders
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>An array of ClothSphereColliderPairs which this Cloth instance should collide with.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Cloth's sleep threshold.</para>
		/// </summary>
		public extern float sleepThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Bending stiffness of the cloth.</para>
		/// </summary>
		public extern float bendingStiffness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Stretching stiffness of the cloth.</para>
		/// </summary>
		public extern float stretchingStiffness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Damp cloth motion.</para>
		/// </summary>
		public extern float damping
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>A constant, external acceleration applied to the cloth.</para>
		/// </summary>
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

		/// <summary>
		///   <para>A random, external acceleration applied to the cloth.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Should gravity affect the cloth simulation?</para>
		/// </summary>
		public extern bool useGravity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Is this cloth enabled?</para>
		/// </summary>
		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The friction of the cloth when colliding with the character.</para>
		/// </summary>
		public extern float friction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much to increase mass of colliding particles.</para>
		/// </summary>
		public extern float collisionMassScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enable continuous collision to improve collision stability.</para>
		/// </summary>
		public extern bool enableContinuousCollision
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Add one virtual particle per triangle to improve collision stability.</para>
		/// </summary>
		public extern float useVirtualParticles
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much world-space movement of the character will affect cloth vertices.</para>
		/// </summary>
		public extern float worldVelocityScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much world-space acceleration of the character will affect cloth vertices.</para>
		/// </summary>
		public extern float worldAccelerationScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Number of cloth solver iterations per second.</para>
		/// </summary>
		public extern float clothSolverFrequency
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Use Tether Anchors.</para>
		/// </summary>
		public extern bool useTethers
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Sets the stiffness frequency parameter.</para>
		/// </summary>
		public extern float stiffnessFrequency
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Minimum distance at which two cloth particles repel each other (default: 0.0).</para>
		/// </summary>
		public extern float selfCollisionDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Self-collision stiffness defines how strong the separating impulse should be for colliding particles.</para>
		/// </summary>
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
