using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Script interface for particle systems (Shuriken).</para>
	/// </summary>
	[RequireComponent(typeof(Transform))]
	[UsedByNativeCode]
	public sealed class ParticleSystem : Component
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetParticleCurrentSize(ref ParticleSystem.Particle particle);

		internal Vector3 GetParticleCurrentSize3D(ref ParticleSystem.Particle particle)
		{
			Vector3 vector;
			ParticleSystem.INTERNAL_CALL_GetParticleCurrentSize3D(this, ref particle, out vector);
			return vector;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetParticleCurrentSize3D(ParticleSystem self, ref ParticleSystem.Particle particle, out Vector3 value);

		internal Color32 GetParticleCurrentColor(ref ParticleSystem.Particle particle)
		{
			Color32 color;
			ParticleSystem.INTERNAL_CALL_GetParticleCurrentColor(this, ref particle, out color);
			return color;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetParticleCurrentColor(ParticleSystem self, ref ParticleSystem.Particle particle, out Color32 value);

		/// <summary>
		///   <para>Is the Particle System playing right now?</para>
		/// </summary>
		public extern bool isPlaying
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Is the Particle System currently emitting particles? A Particle System may stop emitting when its emission module has finished, it has been paused or if the system has been stopped using ParticleSystem.Stop|Stop with the ParticleSystemStopBehavior.StopEmitting|StopEmitting flag. Resume emitting by calling ParticleSystem.Play|Play.</para>
		/// </summary>
		public extern bool isEmitting
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Is the Particle System stopped right now?</para>
		/// </summary>
		public extern bool isStopped
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Is the Particle System paused right now?</para>
		/// </summary>
		public extern bool isPaused
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Playback position in seconds.</para>
		/// </summary>
		public extern float time
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The current number of particles (Read Only).</para>
		/// </summary>
		public extern int particleCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Override the random seed used for the particle system emission.</para>
		/// </summary>
		public extern uint randomSeed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Controls whether the Particle System uses an automatically-generated random number to seed the random number generator.</para>
		/// </summary>
		public extern bool useAutoRandomSeed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Does this system support Automatic Culling?</para>
		/// </summary>
		public extern bool automaticCullingEnabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Access the main particle system settings.</para>
		/// </summary>
		public ParticleSystem.MainModule main
		{
			get
			{
				return new ParticleSystem.MainModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system emission module.</para>
		/// </summary>
		public ParticleSystem.EmissionModule emission
		{
			get
			{
				return new ParticleSystem.EmissionModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system shape module.</para>
		/// </summary>
		public ParticleSystem.ShapeModule shape
		{
			get
			{
				return new ParticleSystem.ShapeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system velocity over lifetime module.</para>
		/// </summary>
		public ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime
		{
			get
			{
				return new ParticleSystem.VelocityOverLifetimeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system limit velocity over lifetime module.</para>
		/// </summary>
		public ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime
		{
			get
			{
				return new ParticleSystem.LimitVelocityOverLifetimeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system velocity inheritance module.</para>
		/// </summary>
		public ParticleSystem.InheritVelocityModule inheritVelocity
		{
			get
			{
				return new ParticleSystem.InheritVelocityModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system force over lifetime module.</para>
		/// </summary>
		public ParticleSystem.ForceOverLifetimeModule forceOverLifetime
		{
			get
			{
				return new ParticleSystem.ForceOverLifetimeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system color over lifetime module.</para>
		/// </summary>
		public ParticleSystem.ColorOverLifetimeModule colorOverLifetime
		{
			get
			{
				return new ParticleSystem.ColorOverLifetimeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system color by lifetime module.</para>
		/// </summary>
		public ParticleSystem.ColorBySpeedModule colorBySpeed
		{
			get
			{
				return new ParticleSystem.ColorBySpeedModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system size over lifetime module.</para>
		/// </summary>
		public ParticleSystem.SizeOverLifetimeModule sizeOverLifetime
		{
			get
			{
				return new ParticleSystem.SizeOverLifetimeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system size by speed module.</para>
		/// </summary>
		public ParticleSystem.SizeBySpeedModule sizeBySpeed
		{
			get
			{
				return new ParticleSystem.SizeBySpeedModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system rotation over lifetime module.</para>
		/// </summary>
		public ParticleSystem.RotationOverLifetimeModule rotationOverLifetime
		{
			get
			{
				return new ParticleSystem.RotationOverLifetimeModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system rotation by speed  module.</para>
		/// </summary>
		public ParticleSystem.RotationBySpeedModule rotationBySpeed
		{
			get
			{
				return new ParticleSystem.RotationBySpeedModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system external forces module.</para>
		/// </summary>
		public ParticleSystem.ExternalForcesModule externalForces
		{
			get
			{
				return new ParticleSystem.ExternalForcesModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system noise module.</para>
		/// </summary>
		public ParticleSystem.NoiseModule noise
		{
			get
			{
				return new ParticleSystem.NoiseModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system collision module.</para>
		/// </summary>
		public ParticleSystem.CollisionModule collision
		{
			get
			{
				return new ParticleSystem.CollisionModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system trigger module.</para>
		/// </summary>
		public ParticleSystem.TriggerModule trigger
		{
			get
			{
				return new ParticleSystem.TriggerModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system sub emitters module.</para>
		/// </summary>
		public ParticleSystem.SubEmittersModule subEmitters
		{
			get
			{
				return new ParticleSystem.SubEmittersModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system texture sheet animation module.</para>
		/// </summary>
		public ParticleSystem.TextureSheetAnimationModule textureSheetAnimation
		{
			get
			{
				return new ParticleSystem.TextureSheetAnimationModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system lights module.</para>
		/// </summary>
		public ParticleSystem.LightsModule lights
		{
			get
			{
				return new ParticleSystem.LightsModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system trails module.</para>
		/// </summary>
		public ParticleSystem.TrailModule trails
		{
			get
			{
				return new ParticleSystem.TrailModule(this);
			}
		}

		/// <summary>
		///   <para>Access the particle system Custom Data module.</para>
		/// </summary>
		public ParticleSystem.CustomDataModule customData
		{
			get
			{
				return new ParticleSystem.CustomDataModule(this);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetParticles(ParticleSystem.Particle[] particles, int size);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetParticles(ParticleSystem.Particle[] particles);

		public void SetCustomParticleData(List<Vector4> customData, ParticleSystemCustomData streamIndex)
		{
			this.SetCustomParticleDataInternal(customData, (int)streamIndex);
		}

		public int GetCustomParticleData(List<Vector4> customData, ParticleSystemCustomData streamIndex)
		{
			return this.GetCustomParticleDataInternal(customData, (int)streamIndex);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetCustomParticleDataInternal(object customData, int streamIndex);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetCustomParticleDataInternal(object customData, int streamIndex);

		/// <summary>
		///   <para>Fastforwards the particle system by simulating particles over given period of time, then pauses it.</para>
		/// </summary>
		/// <param name="t">Time period in seconds to advance the ParticleSystem simulation by. If restart is true, the ParticleSystem will be reset to 0 time, and then advanced by this value. If restart is false, the ParticleSystem simulation will be advanced in time from its current state by this value.</param>
		/// <param name="withChildren">Fastforward all child particle systems as well.</param>
		/// <param name="restart">Restart and start from the beginning.</param>
		/// <param name="fixedTimeStep">Only update the system at fixed intervals, based on the value in "Fixed Time" in the Time options.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart, [DefaultValue("true")] bool fixedTimeStep);

		[ExcludeFromDocs]
		public void Simulate(float t, bool withChildren, bool restart)
		{
			bool flag = true;
			this.Simulate(t, withChildren, restart, flag);
		}

		[ExcludeFromDocs]
		public void Simulate(float t, bool withChildren)
		{
			bool flag = true;
			bool flag2 = true;
			this.Simulate(t, withChildren, flag2, flag);
		}

		[ExcludeFromDocs]
		public void Simulate(float t)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			this.Simulate(t, flag3, flag2, flag);
		}

		/// <summary>
		///   <para>Starts the particle system.</para>
		/// </summary>
		/// <param name="withChildren">Play all child particle systems as well.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Play([DefaultValue("true")] bool withChildren);

		[ExcludeFromDocs]
		public void Play()
		{
			bool flag = true;
			this.Play(flag);
		}

		/// <summary>
		///   <para>Pauses the system so no new particles are emitted and the existing particles are not updated.</para>
		/// </summary>
		/// <param name="withChildren">Pause all child Particle Systems as well.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Pause([DefaultValue("true")] bool withChildren);

		[ExcludeFromDocs]
		public void Pause()
		{
			bool flag = true;
			this.Pause(flag);
		}

		/// <summary>
		///   <para>Stops playing the particle system using the supplied stop behaviour.</para>
		/// </summary>
		/// <param name="withChildren">Stop all child particle systems as well.</param>
		/// <param name="stopBehavior">Stop emitting or stop emitting and clear the system.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Stop([DefaultValue("true")] bool withChildren, [DefaultValue("ParticleSystemStopBehavior.StopEmitting")] ParticleSystemStopBehavior stopBehavior);

		/// <summary>
		///   <para>Stops playing the particle system using the supplied stop behaviour.</para>
		/// </summary>
		/// <param name="withChildren">Stop all child particle systems as well.</param>
		/// <param name="stopBehavior">Stop emitting or stop emitting and clear the system.</param>
		[ExcludeFromDocs]
		public void Stop(bool withChildren)
		{
			ParticleSystemStopBehavior particleSystemStopBehavior = ParticleSystemStopBehavior.StopEmitting;
			this.Stop(withChildren, particleSystemStopBehavior);
		}

		[ExcludeFromDocs]
		public void Stop()
		{
			ParticleSystemStopBehavior particleSystemStopBehavior = ParticleSystemStopBehavior.StopEmitting;
			bool flag = true;
			this.Stop(flag, particleSystemStopBehavior);
		}

		/// <summary>
		///   <para>Remove all particles in the particle system.</para>
		/// </summary>
		/// <param name="withChildren">Clear all child particle systems as well.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear([DefaultValue("true")] bool withChildren);

		[ExcludeFromDocs]
		public void Clear()
		{
			bool flag = true;
			this.Clear(flag);
		}

		/// <summary>
		///   <para>Does the system contain any live particles, or will it produce more?</para>
		/// </summary>
		/// <param name="withChildren">Check all child particle systems as well.</param>
		/// <returns>
		///   <para>True if the Particle System is still "alive". False if the Particle System has stopped emitting particles and all particles are dead.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsAlive([DefaultValue("true")] bool withChildren);

		[ExcludeFromDocs]
		public bool IsAlive()
		{
			bool flag = true;
			return this.IsAlive(flag);
		}

		/// <summary>
		///   <para>Emit count particles immediately.</para>
		/// </summary>
		/// <param name="count">Number of particles to emit.</param>
		[RequiredByNativeCode]
		public void Emit(int count)
		{
			ParticleSystem.INTERNAL_CALL_Emit(this, count);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Emit(ParticleSystem self, int count);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_EmitOld(ref ParticleSystem.Particle particle);

		public void Emit(ParticleSystem.EmitParams emitParams, int count)
		{
			this.Internal_Emit(ref emitParams, count);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Emit(ref ParticleSystem.EmitParams emitParams, int count);

		/// <summary>
		///   <para>Triggers the specified sub emitter on all particles of the Particle System.</para>
		/// </summary>
		/// <param name="subEmitterIndex">Index of the sub emitter to trigger.</param>
		public void TriggerSubEmitter(int subEmitterIndex)
		{
			this.Internal_TriggerSubEmitter(subEmitterIndex, null);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void TriggerSubEmitter(int subEmitterIndex, ref ParticleSystem.Particle particle);

		public void TriggerSubEmitter(int subEmitterIndex, List<ParticleSystem.Particle> particles)
		{
			this.Internal_TriggerSubEmitter(subEmitterIndex, particles);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Internal_TriggerSubEmitter(int subEmitterIndex, object particles);

		/// <summary>
		///   <para></para>
		/// </summary>
		/// <param name="position"></param>
		/// <param name="velocity"></param>
		/// <param name="size"></param>
		/// <param name="lifetime"></param>
		/// <param name="color"></param>
		[Obsolete("Emit with specific parameters is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties", false)]
		public void Emit(Vector3 position, Vector3 velocity, float size, float lifetime, Color32 color)
		{
			ParticleSystem.Particle particle = default(ParticleSystem.Particle);
			particle.position = position;
			particle.velocity = velocity;
			particle.lifetime = lifetime;
			particle.startLifetime = lifetime;
			particle.startSize = size;
			particle.rotation3D = Vector3.zero;
			particle.angularVelocity3D = Vector3.zero;
			particle.startColor = color;
			particle.randomSeed = 5U;
			this.Internal_EmitOld(ref particle);
		}

		[Obsolete("Emit with a single particle structure is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties", false)]
		public void Emit(ParticleSystem.Particle particle)
		{
			this.Internal_EmitOld(ref particle);
		}

		/// <summary>
		///   <para>Start delay in seconds.</para>
		/// </summary>
		[Obsolete("startDelay property is deprecated.Use main.startDelay or main.startDelayMultiplier instead.", false)]
		public float startDelay
		{
			get
			{
				return this.main.startDelayMultiplier;
			}
			set
			{
				this.main.startDelayMultiplier = value;
			}
		}

		/// <summary>
		///   <para>Is the particle system looping?</para>
		/// </summary>
		[Obsolete("loop property is deprecated.Use main.loop instead.", false)]
		public bool loop
		{
			get
			{
				return this.main.loop;
			}
			set
			{
				this.main.loop = value;
			}
		}

		/// <summary>
		///   <para>If set to true, the particle system will automatically start playing on startup.</para>
		/// </summary>
		[Obsolete("playOnAwake property is deprecated.Use main.playOnAwake instead.", false)]
		public bool playOnAwake
		{
			get
			{
				return this.main.playOnAwake;
			}
			set
			{
				this.main.playOnAwake = value;
			}
		}

		/// <summary>
		///   <para>The duration of the particle system in seconds (Read Only).</para>
		/// </summary>
		[Obsolete("duration property is deprecated.Use main.duration instead.", false)]
		public float duration
		{
			get
			{
				return this.main.duration;
			}
		}

		/// <summary>
		///   <para>The playback speed of the particle system. 1 is normal playback speed.</para>
		/// </summary>
		[Obsolete("playbackSpeed property is deprecated.Use main.simulationSpeed instead.", false)]
		public float playbackSpeed
		{
			get
			{
				return this.main.simulationSpeed;
			}
			set
			{
				this.main.simulationSpeed = value;
			}
		}

		/// <summary>
		///   <para>When set to false, the particle system will not emit particles.</para>
		/// </summary>
		[Obsolete("enableEmission property is deprecated.Use emission.enabled instead.", false)]
		public bool enableEmission
		{
			get
			{
				return this.emission.enabled;
			}
			set
			{
				this.emission.enabled = value;
			}
		}

		/// <summary>
		///   <para>The rate of emission.</para>
		/// </summary>
		[Obsolete("emissionRate property is deprecated.Use emission.rateOverTime, emission.rateOverDistance, emission.rateOverTimeMultiplier or emission.rateOverDistanceMultiplier instead.", false)]
		public float emissionRate
		{
			get
			{
				return this.emission.rateOverTimeMultiplier;
			}
			set
			{
				this.emission.rateOverTime = value;
			}
		}

		/// <summary>
		///   <para>The initial speed of particles when emitted. When using curves, this values acts as a scale on the curve.</para>
		/// </summary>
		[Obsolete("startSpeed property is deprecated.Use main.startSpeed or main.startSpeedMultiplier instead.", false)]
		public float startSpeed
		{
			get
			{
				return this.main.startSpeedMultiplier;
			}
			set
			{
				this.main.startSpeedMultiplier = value;
			}
		}

		/// <summary>
		///   <para>The initial size of particles when emitted. When using curves, this values acts as a scale on the curve.</para>
		/// </summary>
		[Obsolete("startSize property is deprecated.Use main.startSize or main.startSizeMultiplier instead.", false)]
		public float startSize
		{
			get
			{
				return this.main.startSizeMultiplier;
			}
			set
			{
				this.main.startSizeMultiplier = value;
			}
		}

		/// <summary>
		///   <para>The initial color of particles when emitted.</para>
		/// </summary>
		[Obsolete("startColor property is deprecated.Use main.startColor instead.", false)]
		public Color startColor
		{
			get
			{
				return this.main.startColor.color;
			}
			set
			{
				this.main.startColor = value;
			}
		}

		/// <summary>
		///   <para>The initial rotation of particles when emitted. When using curves, this values acts as a scale on the curve.</para>
		/// </summary>
		[Obsolete("startRotation property is deprecated.Use main.startRotation or main.startRotationMultiplier instead.", false)]
		public float startRotation
		{
			get
			{
				return this.main.startRotationMultiplier;
			}
			set
			{
				this.main.startRotationMultiplier = value;
			}
		}

		/// <summary>
		///   <para>The initial 3D rotation of particles when emitted. When using curves, this values acts as a scale on the curves.</para>
		/// </summary>
		[Obsolete("startRotation3D property is deprecated.Use main.startRotationX, main.startRotationY and main.startRotationZ instead. (Or main.startRotationXMultiplier, main.startRotationYMultiplier and main.startRotationZMultiplier).", false)]
		public Vector3 startRotation3D
		{
			get
			{
				return new Vector3(this.main.startRotationXMultiplier, this.main.startRotationYMultiplier, this.main.startRotationZMultiplier);
			}
			set
			{
				ParticleSystem.MainModule main = this.main;
				main.startRotationXMultiplier = value.x;
				main.startRotationYMultiplier = value.y;
				main.startRotationZMultiplier = value.z;
			}
		}

		/// <summary>
		///   <para>The total lifetime in seconds that particles will have when emitted. When using curves, this values acts as a scale on the curve. This value is set in the particle when it is created by the particle system.</para>
		/// </summary>
		[Obsolete("startLifetime property is deprecated.Use main.startLifetime or main.startLifetimeMultiplier instead.", false)]
		public float startLifetime
		{
			get
			{
				return this.main.startLifetimeMultiplier;
			}
			set
			{
				this.main.startLifetimeMultiplier = value;
			}
		}

		/// <summary>
		///   <para>Scale being applied to the gravity defined by Physics.gravity.</para>
		/// </summary>
		[Obsolete("gravityModifier property is deprecated.Use main.gravityModifier or main.gravityModifierMultiplier instead.", false)]
		public float gravityModifier
		{
			get
			{
				return this.main.gravityModifierMultiplier;
			}
			set
			{
				this.main.gravityModifierMultiplier = value;
			}
		}

		/// <summary>
		///   <para>The maximum number of particles to emit.</para>
		/// </summary>
		[Obsolete("maxParticles property is deprecated.Use main.maxParticles instead.", false)]
		public int maxParticles
		{
			get
			{
				return this.main.maxParticles;
			}
			set
			{
				this.main.maxParticles = value;
			}
		}

		/// <summary>
		///   <para>This selects the space in which to simulate particles. It can be either world or local space.</para>
		/// </summary>
		[Obsolete("simulationSpace property is deprecated.Use main.simulationSpace instead.", false)]
		public ParticleSystemSimulationSpace simulationSpace
		{
			get
			{
				return this.main.simulationSpace;
			}
			set
			{
				this.main.simulationSpace = value;
			}
		}

		/// <summary>
		///   <para>The scaling mode applied to particle sizes and positions.</para>
		/// </summary>
		[Obsolete("scalingMode property is deprecated.Use main.scalingMode instead.", false)]
		public ParticleSystemScalingMode scalingMode
		{
			get
			{
				return this.main.scalingMode;
			}
			set
			{
				this.main.scalingMode = value;
			}
		}

		/// <summary>
		///   <para>Script interface for the main module.</para>
		/// </summary>
		public struct MainModule
		{
			internal MainModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>The duration of the particle system in seconds.</para>
			/// </summary>
			public float duration
			{
				get
				{
					return ParticleSystem.MainModule.GetDuration(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetDuration(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Is the particle system looping?</para>
			/// </summary>
			public bool loop
			{
				get
				{
					return ParticleSystem.MainModule.GetLoop(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetLoop(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>When looping is enabled, this controls whether this particle system will look like it has already simulated for one loop when first becoming visible.</para>
			/// </summary>
			public bool prewarm
			{
				get
				{
					return ParticleSystem.MainModule.GetPrewarm(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetPrewarm(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Start delay in seconds.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve startDelay
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartDelay(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.MainModule.SetStartDelay(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Start delay multiplier in seconds.</para>
			/// </summary>
			public float startDelayMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartDelayMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartDelayMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The total lifetime in seconds that each new particle will have.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve startLifetime
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartLifetime(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.MainModule.SetStartLifetime(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Start lifetime multiplier.</para>
			/// </summary>
			public float startLifetimeMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartLifetimeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartLifetimeMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The initial speed of particles when emitted.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve startSpeed
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartSpeed(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.MainModule.SetStartSpeed(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>A multiplier of the initial speed of particles when emitted.</para>
			/// </summary>
			public float startSpeedMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartSpeedMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartSpeedMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>A flag to enable specifying particle size individually for each axis.</para>
			/// </summary>
			public bool startSize3D
			{
				get
				{
					return ParticleSystem.MainModule.GetStartSize3D(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartSize3D(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The initial size of particles when emitted.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve startSize
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartSizeX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Start size multiplier.</para>
			/// </summary>
			public float startSizeMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartSizeXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The initial size of particles along the X axis when emitted.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve startSizeX
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartSizeX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Start rotation multiplier along the X axis.</para>
			/// </summary>
			public float startSizeXMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartSizeXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The initial size of particles along the Y axis when emitted.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve startSizeY
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartSizeY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Start rotation multiplier along the Y axis.</para>
			/// </summary>
			public float startSizeYMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartSizeYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The initial size of particles along the Z axis when emitted.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve startSizeZ
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartSizeZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Start rotation multiplier along the Z axis.</para>
			/// </summary>
			public float startSizeZMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartSizeZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>A flag to enable 3D particle rotation.</para>
			/// </summary>
			public bool startRotation3D
			{
				get
				{
					return ParticleSystem.MainModule.GetStartRotation3D(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotation3D(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The initial rotation of particles when emitted.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve startRotation
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartRotationZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Start rotation multiplier.</para>
			/// </summary>
			public float startRotationMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartRotationZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The initial rotation of particles around the X axis when emitted.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve startRotationX
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartRotationX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Start rotation multiplier around the X axis.</para>
			/// </summary>
			public float startRotationXMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartRotationXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The initial rotation of particles around the Y axis when emitted.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve startRotationY
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartRotationY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Start rotation multiplier around the Y axis.</para>
			/// </summary>
			public float startRotationYMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartRotationYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The initial rotation of particles around the Z axis when emitted.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve startRotationZ
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartRotationZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Start rotation multiplier around the Z axis.</para>
			/// </summary>
			public float startRotationZMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartRotationZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Makes some particles spin in the opposite direction.</para>
			/// </summary>
			public float flipRotation
			{
				get
				{
					return ParticleSystem.MainModule.GetFlipRotation(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetFlipRotation(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The initial color of particles when emitted.</para>
			/// </summary>
			public ParticleSystem.MinMaxGradient startColor
			{
				get
				{
					ParticleSystem.MinMaxGradient minMaxGradient = default(ParticleSystem.MinMaxGradient);
					ParticleSystem.MainModule.GetStartColor(this.m_ParticleSystem, ref minMaxGradient);
					return minMaxGradient;
				}
				set
				{
					ParticleSystem.MainModule.SetStartColor(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Scale applied to the gravity, defined by Physics.gravity.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve gravityModifier
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetGravityModifier(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.MainModule.SetGravityModifier(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the gravity mulutiplier.</para>
			/// </summary>
			public float gravityModifierMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetGravityModifierMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetGravityModifierMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>This selects the space in which to simulate particles. It can be either world or local space.</para>
			/// </summary>
			public ParticleSystemSimulationSpace simulationSpace
			{
				get
				{
					return ParticleSystem.MainModule.GetSimulationSpace(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetSimulationSpace(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Simulate particles relative to a custom transform component.</para>
			/// </summary>
			public Transform customSimulationSpace
			{
				get
				{
					return ParticleSystem.MainModule.GetCustomSimulationSpace(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetCustomSimulationSpace(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Override the default playback speed of the Particle System.</para>
			/// </summary>
			public float simulationSpeed
			{
				get
				{
					return ParticleSystem.MainModule.GetSimulationSpeed(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetSimulationSpeed(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>When true, use the unscaled delta time to simulate the Particle System. Otherwise, use the scaled delta time.</para>
			/// </summary>
			public bool useUnscaledTime
			{
				get
				{
					return ParticleSystem.MainModule.GetUseUnscaledTime(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetUseUnscaledTime(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Control how the particle system's Transform Component is applied to the particle system.</para>
			/// </summary>
			public ParticleSystemScalingMode scalingMode
			{
				get
				{
					return ParticleSystem.MainModule.GetScalingMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetScalingMode(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>If set to true, the particle system will automatically start playing on startup.</para>
			/// </summary>
			public bool playOnAwake
			{
				get
				{
					return ParticleSystem.MainModule.GetPlayOnAwake(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetPlayOnAwake(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The maximum number of particles to emit.</para>
			/// </summary>
			public int maxParticles
			{
				get
				{
					return ParticleSystem.MainModule.GetMaxParticles(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetMaxParticles(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Control how the Particle System calculates its velocity, when moving in the world.</para>
			/// </summary>
			public ParticleSystemEmitterVelocityMode emitterVelocityMode
			{
				get
				{
					return (!ParticleSystem.MainModule.GetUseRigidbodyForVelocity(this.m_ParticleSystem)) ? ParticleSystemEmitterVelocityMode.Transform : ParticleSystemEmitterVelocityMode.Rigidbody;
				}
				set
				{
					ParticleSystem.MainModule.SetUseRigidbodyForVelocity(this.m_ParticleSystem, value == ParticleSystemEmitterVelocityMode.Rigidbody);
				}
			}

			/// <summary>
			///   <para>Configure whether the GameObject will automatically disable or destroy itself, when the Particle System is stopped and all particles have died.</para>
			/// </summary>
			public ParticleSystemStopAction stopAction
			{
				get
				{
					return ParticleSystem.MainModule.GetStopAction(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStopAction(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDuration(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetDuration(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLoop(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetLoop(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetPrewarm(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetPrewarm(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartDelay(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartDelay(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartDelayMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartDelayMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartLifetime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartLifetime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartLifetimeMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartLifetimeMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSpeedMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartSpeedMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSize3D(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetStartSize3D(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSizeX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartSizeX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSizeXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartSizeXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSizeY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartSizeY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSizeYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartSizeYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSizeZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartSizeZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSizeZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartSizeZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotation3D(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetStartRotation3D(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotationX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartRotationX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotationXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartRotationXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotationY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartRotationY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotationYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartRotationYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotationZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartRotationZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotationZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartRotationZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFlipRotation(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetFlipRotation(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetGravityModifier(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetGravityModifier(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetGravityModifierMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetGravityModifierMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSimulationSpace(ParticleSystem system, ParticleSystemSimulationSpace value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemSimulationSpace GetSimulationSpace(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCustomSimulationSpace(ParticleSystem system, Transform value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Transform GetCustomSimulationSpace(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSimulationSpeed(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetSimulationSpeed(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseUnscaledTime(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseUnscaledTime(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetScalingMode(ParticleSystem system, ParticleSystemScalingMode value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemScalingMode GetScalingMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetPlayOnAwake(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetPlayOnAwake(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMaxParticles(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxParticles(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseRigidbodyForVelocity(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseRigidbodyForVelocity(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStopAction(ParticleSystem system, ParticleSystemStopAction value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemStopAction GetStopAction(ParticleSystem system);

			/// <summary>
			///   <para>Cause some particles to spin in  the opposite direction.</para>
			/// </summary>
			[Obsolete("Please use flipRotation instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/MainModule.flipRotation", false)]
			public float randomizeRotationDirection
			{
				get
				{
					return this.flipRotation;
				}
				set
				{
					this.flipRotation = value;
				}
			}

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Emission module.</para>
		/// </summary>
		public struct EmissionModule
		{
			internal EmissionModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Emission module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.EmissionModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.EmissionModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The rate at which new particles are spawned, over time.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve rateOverTime
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.EmissionModule.GetRateOverTime(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.EmissionModule.SetRateOverTime(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the rate over time multiplier.</para>
			/// </summary>
			public float rateOverTimeMultiplier
			{
				get
				{
					return ParticleSystem.EmissionModule.GetRateOverTimeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.EmissionModule.SetRateOverTimeMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The rate at which new particles are spawned, over distance.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve rateOverDistance
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.EmissionModule.GetRateOverDistance(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.EmissionModule.SetRateOverDistance(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the rate over distance multiplier.</para>
			/// </summary>
			public float rateOverDistanceMultiplier
			{
				get
				{
					return ParticleSystem.EmissionModule.GetRateOverDistanceMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.EmissionModule.SetRateOverDistanceMultiplier(this.m_ParticleSystem, value);
				}
			}

			public void SetBursts(ParticleSystem.Burst[] bursts)
			{
				this.SetBursts(bursts, bursts.Length);
			}

			public void SetBursts(ParticleSystem.Burst[] bursts, int size)
			{
				this.burstCount = size;
				for (int i = 0; i < size; i++)
				{
					ParticleSystem.EmissionModule.SetBurst(this.m_ParticleSystem, i, bursts[i]);
				}
			}

			public int GetBursts(ParticleSystem.Burst[] bursts)
			{
				int burstCount = this.burstCount;
				for (int i = 0; i < burstCount; i++)
				{
					bursts[i] = ParticleSystem.EmissionModule.GetBurst(this.m_ParticleSystem, i);
				}
				return burstCount;
			}

			public void SetBurst(int index, ParticleSystem.Burst burst)
			{
				ParticleSystem.EmissionModule.SetBurst(this.m_ParticleSystem, index, burst);
			}

			/// <summary>
			///   <para>Get a single burst from the array of bursts.</para>
			/// </summary>
			/// <param name="index">The index of the burst to retrieve.</param>
			/// <returns>
			///   <para>The burst data at the given index.</para>
			/// </returns>
			public ParticleSystem.Burst GetBurst(int index)
			{
				return ParticleSystem.EmissionModule.GetBurst(this.m_ParticleSystem, index);
			}

			/// <summary>
			///   <para>The current number of bursts.</para>
			/// </summary>
			public int burstCount
			{
				get
				{
					return ParticleSystem.EmissionModule.GetBurstCount(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.EmissionModule.SetBurstCount(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetBurstCount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRateOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRateOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRateOverTimeMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRateOverTimeMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRateOverDistance(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRateOverDistance(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRateOverDistanceMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRateOverDistanceMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBurstCount(ParticleSystem system, int value);

			private static void SetBurst(ParticleSystem system, int index, ParticleSystem.Burst burst)
			{
				ParticleSystem.EmissionModule.INTERNAL_CALL_SetBurst(system, index, ref burst);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetBurst(ParticleSystem system, int index, ref ParticleSystem.Burst burst);

			private static ParticleSystem.Burst GetBurst(ParticleSystem system, int index)
			{
				ParticleSystem.Burst burst;
				ParticleSystem.EmissionModule.INTERNAL_CALL_GetBurst(system, index, out burst);
				return burst;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetBurst(ParticleSystem system, int index, out ParticleSystem.Burst value);

			/// <summary>
			///   <para>The emission type.</para>
			/// </summary>
			[Obsolete("ParticleSystemEmissionType no longer does anything. Time and Distance based emission are now both always active.", false)]
			public ParticleSystemEmissionType type
			{
				get
				{
					return ParticleSystemEmissionType.Time;
				}
				set
				{
				}
			}

			/// <summary>
			///   <para>The rate at which new particles are spawned.</para>
			/// </summary>
			[Obsolete("rate property is deprecated. Use rateOverTime or rateOverDistance instead.", false)]
			public ParticleSystem.MinMaxCurve rate
			{
				get
				{
					return this.rateOverTime;
				}
				set
				{
					this.rateOverTime = value;
				}
			}

			/// <summary>
			///   <para>Change the rate multiplier.</para>
			/// </summary>
			[Obsolete("rateMultiplier property is deprecated. Use rateOverTimeMultiplier or rateOverDistanceMultiplier instead.", false)]
			public float rateMultiplier
			{
				get
				{
					return this.rateOverTimeMultiplier;
				}
				set
				{
					this.rateOverTimeMultiplier = value;
				}
			}

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Shape module.</para>
		/// </summary>
		public struct ShapeModule
		{
			internal ShapeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Shape module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.ShapeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Type of shape to emit particles from.</para>
			/// </summary>
			public ParticleSystemShapeType shapeType
			{
				get
				{
					return ParticleSystem.ShapeModule.GetShapeType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetShapeType(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Randomizes the starting direction of particles.</para>
			/// </summary>
			public float randomDirectionAmount
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRandomDirectionAmount(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRandomDirectionAmount(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Spherizes the starting direction of particles.</para>
			/// </summary>
			public float sphericalDirectionAmount
			{
				get
				{
					return ParticleSystem.ShapeModule.GetSphericalDirectionAmount(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetSphericalDirectionAmount(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Randomizes the starting position of particles.</para>
			/// </summary>
			public float randomPositionAmount
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRandomPositionAmount(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRandomPositionAmount(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Align particles based on their initial direction of travel.</para>
			/// </summary>
			public bool alignToDirection
			{
				get
				{
					return ParticleSystem.ShapeModule.GetAlignToDirection(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetAlignToDirection(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Radius of the shape.</para>
			/// </summary>
			public float radius
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRadius(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRadius(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The mode used for generating particles along the radius.</para>
			/// </summary>
			public ParticleSystemShapeMultiModeValue radiusMode
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRadiusMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRadiusMode(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Control the gap between emission points along the radius.</para>
			/// </summary>
			public float radiusSpread
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRadiusSpread(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRadiusSpread(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>When using one of the animated modes, how quickly to move the emission position along the radius.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve radiusSpeed
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ShapeModule.GetRadiusSpeed(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.ShapeModule.SetRadiusSpeed(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>A multiplier of the radius speed of the emission shape.</para>
			/// </summary>
			public float radiusSpeedMultiplier
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRadiusSpeedMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRadiusSpeedMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Thickness of the radius.</para>
			/// </summary>
			public float radiusThickness
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRadiusThickness(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRadiusThickness(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Angle of the cone.</para>
			/// </summary>
			public float angle
			{
				get
				{
					return ParticleSystem.ShapeModule.GetAngle(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetAngle(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Length of the cone.</para>
			/// </summary>
			public float length
			{
				get
				{
					return ParticleSystem.ShapeModule.GetLength(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetLength(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Thickness of the box.</para>
			/// </summary>
			public Vector3 boxThickness
			{
				get
				{
					return ParticleSystem.ShapeModule.GetBoxThickness(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetBoxThickness(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Where on the mesh to emit particles from.</para>
			/// </summary>
			public ParticleSystemMeshShapeType meshShapeType
			{
				get
				{
					return ParticleSystem.ShapeModule.GetMeshShapeType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMeshShapeType(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Mesh to emit particles from.</para>
			/// </summary>
			public Mesh mesh
			{
				get
				{
					return ParticleSystem.ShapeModule.GetMesh(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMesh(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>MeshRenderer to emit particles from.</para>
			/// </summary>
			public MeshRenderer meshRenderer
			{
				get
				{
					return ParticleSystem.ShapeModule.GetMeshRenderer(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMeshRenderer(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>SkinnedMeshRenderer to emit particles from.</para>
			/// </summary>
			public SkinnedMeshRenderer skinnedMeshRenderer
			{
				get
				{
					return ParticleSystem.ShapeModule.GetSkinnedMeshRenderer(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetSkinnedMeshRenderer(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Sprite to emit particles from.</para>
			/// </summary>
			public Sprite sprite
			{
				get
				{
					return ParticleSystem.ShapeModule.GetSprite(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetSprite(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>SpriteRenderer to emit particles from.</para>
			/// </summary>
			public SpriteRenderer spriteRenderer
			{
				get
				{
					return ParticleSystem.ShapeModule.GetSpriteRenderer(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetSpriteRenderer(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Emit from a single material, or the whole mesh.</para>
			/// </summary>
			public bool useMeshMaterialIndex
			{
				get
				{
					return ParticleSystem.ShapeModule.GetUseMeshMaterialIndex(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetUseMeshMaterialIndex(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Emit particles from a single material of a mesh.</para>
			/// </summary>
			public int meshMaterialIndex
			{
				get
				{
					return ParticleSystem.ShapeModule.GetMeshMaterialIndex(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMeshMaterialIndex(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Modulate the particle colors with the vertex colors, or the material color if no vertex colors exist.</para>
			/// </summary>
			public bool useMeshColors
			{
				get
				{
					return ParticleSystem.ShapeModule.GetUseMeshColors(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetUseMeshColors(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Move particles away from the surface of the source mesh.</para>
			/// </summary>
			public float normalOffset
			{
				get
				{
					return ParticleSystem.ShapeModule.GetNormalOffset(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetNormalOffset(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Circle arc angle.</para>
			/// </summary>
			public float arc
			{
				get
				{
					return ParticleSystem.ShapeModule.GetArc(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetArc(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The mode used for generating particles around the arc.</para>
			/// </summary>
			public ParticleSystemShapeMultiModeValue arcMode
			{
				get
				{
					return ParticleSystem.ShapeModule.GetArcMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetArcMode(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Control the gap between emission points around the arc.</para>
			/// </summary>
			public float arcSpread
			{
				get
				{
					return ParticleSystem.ShapeModule.GetArcSpread(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetArcSpread(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>When using one of the animated modes, how quickly to move the emission position around the arc.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve arcSpeed
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ShapeModule.GetArcSpeed(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.ShapeModule.SetArcSpeed(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>A multiplier of the arc speed of the emission shape.</para>
			/// </summary>
			public float arcSpeedMultiplier
			{
				get
				{
					return ParticleSystem.ShapeModule.GetArcSpeedMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetArcSpeedMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The radius of the Donut shape.</para>
			/// </summary>
			public float donutRadius
			{
				get
				{
					return ParticleSystem.ShapeModule.GetDonutRadius(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetDonutRadius(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Apply an offset to the position from which particles are emitted.</para>
			/// </summary>
			public Vector3 position
			{
				get
				{
					return ParticleSystem.ShapeModule.GetPosition(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetPosition(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Apply a rotation to the shape from which particles are emitted.</para>
			/// </summary>
			public Vector3 rotation
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRotation(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRotation(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Apply scale to the shape from which particles are emitted.</para>
			/// </summary>
			public Vector3 scale
			{
				get
				{
					return ParticleSystem.ShapeModule.GetScale(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetScale(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Selects a texture to be used for tinting particle start colors.</para>
			/// </summary>
			public Texture2D texture
			{
				get
				{
					return ParticleSystem.ShapeModule.GetTexture(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetTexture(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Selects which channel of the texture is used for discarding particles.</para>
			/// </summary>
			public ParticleSystemShapeTextureChannel textureClipChannel
			{
				get
				{
					return (ParticleSystemShapeTextureChannel)ParticleSystem.ShapeModule.GetTextureClipChannel(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetTextureClipChannel(this.m_ParticleSystem, (int)value);
				}
			}

			/// <summary>
			///   <para>Discards particles when they are spawned on an area of the texture with a value lower than this threshold.</para>
			/// </summary>
			public float textureClipThreshold
			{
				get
				{
					return ParticleSystem.ShapeModule.GetTextureClipThreshold(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetTextureClipThreshold(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>When enabled, the RGB channels of the texture are applied to the particle color when spawned.</para>
			/// </summary>
			public bool textureColorAffectsParticles
			{
				get
				{
					return ParticleSystem.ShapeModule.GetTextureColorAffectsParticles(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetTextureColorAffectsParticles(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>When enabled, the alpha channel of the texture is applied to the particle alpha when spawned.</para>
			/// </summary>
			public bool textureAlphaAffectsParticles
			{
				get
				{
					return ParticleSystem.ShapeModule.GetTextureAlphaAffectsParticles(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetTextureAlphaAffectsParticles(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>When enabled, 4 neighboring samples are taken from the texture, and combined to give the final particle value.</para>
			/// </summary>
			public bool textureBilinearFiltering
			{
				get
				{
					return ParticleSystem.ShapeModule.GetTextureBilinearFiltering(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetTextureBilinearFiltering(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>When using a Mesh as a source shape type, this option controls which UV channel on the Mesh is used for reading the source texture.</para>
			/// </summary>
			public int textureUVChannel
			{
				get
				{
					return ParticleSystem.ShapeModule.GetTextureUVChannel(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetTextureUVChannel(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetShapeType(ParticleSystem system, ParticleSystemShapeType value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemShapeType GetShapeType(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRandomDirectionAmount(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRandomDirectionAmount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSphericalDirectionAmount(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetSphericalDirectionAmount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRandomPositionAmount(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRandomPositionAmount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetAlignToDirection(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetAlignToDirection(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadius(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadius(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusMode(ParticleSystem system, ParticleSystemShapeMultiModeValue value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemShapeMultiModeValue GetRadiusMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusSpread(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadiusSpread(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRadiusSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusSpeedMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadiusSpeedMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusThickness(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadiusThickness(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetAngle(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetAngle(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLength(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetLength(ParticleSystem system);

			private static void SetBoxThickness(ParticleSystem system, Vector3 value)
			{
				ParticleSystem.ShapeModule.INTERNAL_CALL_SetBoxThickness(system, ref value);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetBoxThickness(ParticleSystem system, ref Vector3 value);

			private static Vector3 GetBoxThickness(ParticleSystem system)
			{
				Vector3 vector;
				ParticleSystem.ShapeModule.INTERNAL_CALL_GetBoxThickness(system, out vector);
				return vector;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetBoxThickness(ParticleSystem system, out Vector3 value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshShapeType(ParticleSystem system, ParticleSystemMeshShapeType value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemMeshShapeType GetMeshShapeType(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMesh(ParticleSystem system, Mesh value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Mesh GetMesh(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshRenderer(ParticleSystem system, MeshRenderer value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern MeshRenderer GetMeshRenderer(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSkinnedMeshRenderer(ParticleSystem system, SkinnedMeshRenderer value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern SkinnedMeshRenderer GetSkinnedMeshRenderer(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSprite(ParticleSystem system, Sprite value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Sprite GetSprite(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSpriteRenderer(ParticleSystem system, SpriteRenderer value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern SpriteRenderer GetSpriteRenderer(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseMeshMaterialIndex(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseMeshMaterialIndex(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshMaterialIndex(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMeshMaterialIndex(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseMeshColors(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseMeshColors(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetNormalOffset(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetNormalOffset(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetArc(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetArc(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetArcMode(ParticleSystem system, ParticleSystemShapeMultiModeValue value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemShapeMultiModeValue GetArcMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetArcSpread(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetArcSpread(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetArcSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetArcSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetArcSpeedMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetArcSpeedMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDonutRadius(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetDonutRadius(ParticleSystem system);

			private static void SetPosition(ParticleSystem system, Vector3 value)
			{
				ParticleSystem.ShapeModule.INTERNAL_CALL_SetPosition(system, ref value);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetPosition(ParticleSystem system, ref Vector3 value);

			private static Vector3 GetPosition(ParticleSystem system)
			{
				Vector3 vector;
				ParticleSystem.ShapeModule.INTERNAL_CALL_GetPosition(system, out vector);
				return vector;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetPosition(ParticleSystem system, out Vector3 value);

			private static void SetRotation(ParticleSystem system, Vector3 value)
			{
				ParticleSystem.ShapeModule.INTERNAL_CALL_SetRotation(system, ref value);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRotation(ParticleSystem system, ref Vector3 value);

			private static Vector3 GetRotation(ParticleSystem system)
			{
				Vector3 vector;
				ParticleSystem.ShapeModule.INTERNAL_CALL_GetRotation(system, out vector);
				return vector;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetRotation(ParticleSystem system, out Vector3 value);

			private static void SetScale(ParticleSystem system, Vector3 value)
			{
				ParticleSystem.ShapeModule.INTERNAL_CALL_SetScale(system, ref value);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetScale(ParticleSystem system, ref Vector3 value);

			private static Vector3 GetScale(ParticleSystem system)
			{
				Vector3 vector;
				ParticleSystem.ShapeModule.INTERNAL_CALL_GetScale(system, out vector);
				return vector;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetScale(ParticleSystem system, out Vector3 value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetTexture(ParticleSystem system, Texture2D value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Texture2D GetTexture(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetTextureClipChannel(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetTextureClipChannel(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetTextureClipThreshold(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetTextureClipThreshold(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetTextureColorAffectsParticles(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetTextureColorAffectsParticles(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetTextureAlphaAffectsParticles(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetTextureAlphaAffectsParticles(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetTextureBilinearFiltering(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetTextureBilinearFiltering(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetTextureUVChannel(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetTextureUVChannel(ParticleSystem system);

			/// <summary>
			///   <para>Scale of the box.</para>
			/// </summary>
			[Obsolete("Please use scale instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/ShapeModule.scale", false)]
			public Vector3 box
			{
				get
				{
					return ParticleSystem.ShapeModule.GetScale(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetScale(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Apply a scaling factor to the mesh used for generating source positions.</para>
			/// </summary>
			[Obsolete("meshScale property is deprecated.Please use scale instead.", false)]
			public float meshScale
			{
				get
				{
					return this.scale.x;
				}
				set
				{
					this.scale = new Vector3(value, value, value);
				}
			}

			/// <summary>
			///   <para>Randomizes the starting direction of particles.</para>
			/// </summary>
			[Obsolete("randomDirection property is deprecated.Use randomDirectionAmount instead.", false)]
			public bool randomDirection
			{
				get
				{
					return this.randomDirectionAmount >= 0.5f;
				}
				set
				{
					this.randomDirectionAmount = ((!value) ? 0f : 1f);
				}
			}

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Velocity Over Lifetime module.</para>
		/// </summary>
		public struct VelocityOverLifetimeModule
		{
			internal VelocityOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Velocity Over Lifetime module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle speed based on lifetime, on the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle speed based on lifetime, on the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle speed based on lifetime, on the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>X axis speed multiplier.</para>
			/// </summary>
			public float xMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Y axis speed multiplier.</para>
			/// </summary>
			public float yMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Z axis speed multiplier.</para>
			/// </summary>
			public float zMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle speed based on lifetime, around the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve orbitalX
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetOrbitalX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetOrbitalX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle speed based on lifetime, around the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve orbitalY
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetOrbitalY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetOrbitalY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle speed based on lifetime, around the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve orbitalZ
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetOrbitalZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetOrbitalZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>X axis speed multiplier.</para>
			/// </summary>
			public float orbitalXMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetOrbitalXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetOrbitalXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Y axis speed multiplier.</para>
			/// </summary>
			public float orbitalYMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetOrbitalYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetOrbitalYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Z axis speed multiplier.</para>
			/// </summary>
			public float orbitalZMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetOrbitalZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetOrbitalZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Specify a custom center of rotation for the orbital and radial velocities.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve orbitalOffsetX
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetOrbitalOffsetX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetOrbitalOffsetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Specify a custom center of rotation for the orbital and radial velocities.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve orbitalOffsetY
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetOrbitalOffsetY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetOrbitalOffsetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Specify a custom center of rotation for the orbital and radial velocities.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve orbitalOffsetZ
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetOrbitalOffsetZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetOrbitalOffsetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>This method is more efficient than accessing the whole curve, if you only want to change the overall offset multiplier.</para>
			/// </summary>
			public float orbitalOffsetXMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetOrbitalOffsetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetOrbitalOffsetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>This method is more efficient than accessing the whole curve, if you only want to change the overall offset multiplier.</para>
			/// </summary>
			public float orbitalOffsetYMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetOrbitalOffsetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetOrbitalOffsetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>This method is more efficient than accessing the whole curve, if you only want to change the overall offset multiplier.</para>
			/// </summary>
			public float orbitalOffsetZMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetOrbitalOffsetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetOrbitalOffsetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle speed based on lifetime, away from a center position.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve radial
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetRadial(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetRadial(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Radial speed multiplier.</para>
			/// </summary>
			public float radialMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetRadialMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetRadialMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle speed based on lifetime, without affecting the direction of the particles.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve speedModifier
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetSpeedModifier(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetSpeedModifier(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Speed multiplier.</para>
			/// </summary>
			public float speedModifierMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetSpeedModifierMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetSpeedModifierMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Specifies if the velocities are in local space (rotated with the transform) or world space.</para>
			/// </summary>
			public ParticleSystemSimulationSpace space
			{
				get
				{
					return (!ParticleSystem.VelocityOverLifetimeModule.GetWorldSpace(this.m_ParticleSystem)) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOrbitalX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetOrbitalX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOrbitalY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetOrbitalY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOrbitalZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetOrbitalZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOrbitalXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetOrbitalXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOrbitalYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetOrbitalYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOrbitalZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetOrbitalZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOrbitalOffsetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetOrbitalOffsetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOrbitalOffsetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetOrbitalOffsetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOrbitalOffsetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetOrbitalOffsetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOrbitalOffsetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetOrbitalOffsetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOrbitalOffsetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetOrbitalOffsetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOrbitalOffsetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetOrbitalOffsetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadial(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRadial(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadialMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadialMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSpeedModifier(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetSpeedModifier(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSpeedModifierMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetSpeedModifierMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetWorldSpace(ParticleSystem system);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Limit Velocity Over Lifetime module.</para>
		/// </summary>
		public struct LimitVelocityOverLifetimeModule
		{
			internal LimitVelocityOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Limit Force Over Lifetime module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Maximum velocity curve for the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve limitX
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the limit multiplier on the X axis.</para>
			/// </summary>
			public float limitXMultiplier
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Maximum velocity curve for the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve limitY
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the limit multiplier on the Y axis.</para>
			/// </summary>
			public float limitYMultiplier
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Maximum velocity curve for the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve limitZ
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the limit multiplier on the Z axis.</para>
			/// </summary>
			public float limitZMultiplier
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Maximum velocity curve, when not using one curve per axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve limit
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetMagnitude(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetMagnitude(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the limit multiplier.</para>
			/// </summary>
			public float limitMultiplier
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetMagnitudeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetMagnitudeMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Controls how much the velocity that exceeds the velocity limit should be dampened.</para>
			/// </summary>
			public float dampen
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetDampen(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetDampen(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Set the velocity limit on each axis separately.</para>
			/// </summary>
			public bool separateAxes
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Specifies if the velocity limits are in local space (rotated with the transform) or world space.</para>
			/// </summary>
			public ParticleSystemSimulationSpace space
			{
				get
				{
					return (!ParticleSystem.LimitVelocityOverLifetimeModule.GetWorldSpace(this.m_ParticleSystem)) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
				}
			}

			/// <summary>
			///   <para>Controls the amount of drag applied to the particle velocities.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve drag
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetDrag(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetDrag(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the drag multiplier.</para>
			/// </summary>
			public float dragMultiplier
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetDragMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetDragMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Adjust the amount of drag applied to particles, based on their sizes.</para>
			/// </summary>
			public bool multiplyDragByParticleSize
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetMultiplyDragByParticleSize(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetMultiplyDragByParticleSize(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Adjust the amount of drag applied to particles, based on their speeds.</para>
			/// </summary>
			public bool multiplyDragByParticleVelocity
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetMultiplyDragByParticleVelocity(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetMultiplyDragByParticleVelocity(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMagnitude(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetMagnitude(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMagnitudeMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMagnitudeMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDampen(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetDampen(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetWorldSpace(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDrag(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetDrag(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDragMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetDragMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMultiplyDragByParticleSize(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetMultiplyDragByParticleSize(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMultiplyDragByParticleVelocity(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetMultiplyDragByParticleVelocity(ParticleSystem system);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>The Inherit Velocity Module controls how the velocity of the emitter is transferred to the particles as they are emitted.</para>
		/// </summary>
		public struct InheritVelocityModule
		{
			internal InheritVelocityModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the InheritVelocity module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.InheritVelocityModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.InheritVelocityModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>How to apply emitter velocity to particles.</para>
			/// </summary>
			public ParticleSystemInheritVelocityMode mode
			{
				get
				{
					return ParticleSystem.InheritVelocityModule.GetMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.InheritVelocityModule.SetMode(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Curve to define how much emitter velocity is applied during the lifetime of a particle.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve curve
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.InheritVelocityModule.GetCurve(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.InheritVelocityModule.SetCurve(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the curve multiplier.</para>
			/// </summary>
			public float curveMultiplier
			{
				get
				{
					return ParticleSystem.InheritVelocityModule.GetCurveMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.InheritVelocityModule.SetCurveMultiplier(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMode(ParticleSystem system, ParticleSystemInheritVelocityMode value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemInheritVelocityMode GetMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCurve(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetCurve(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCurveMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetCurveMultiplier(ParticleSystem system);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Force Over Lifetime module.</para>
		/// </summary>
		public struct ForceOverLifetimeModule
		{
			internal ForceOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Force Over Lifetime module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.ForceOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The curve defining particle forces in the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ForceOverLifetimeModule.GetX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>The curve defining particle forces in the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ForceOverLifetimeModule.GetY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>The curve defining particle forces in the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ForceOverLifetimeModule.GetZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the X axis mulutiplier.</para>
			/// </summary>
			public float xMultiplier
			{
				get
				{
					return ParticleSystem.ForceOverLifetimeModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Change the Y axis multiplier.</para>
			/// </summary>
			public float yMultiplier
			{
				get
				{
					return ParticleSystem.ForceOverLifetimeModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Change the Z axis multiplier.</para>
			/// </summary>
			public float zMultiplier
			{
				get
				{
					return ParticleSystem.ForceOverLifetimeModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Are the forces being applied in local or world space?</para>
			/// </summary>
			public ParticleSystemSimulationSpace space
			{
				get
				{
					return (!ParticleSystem.ForceOverLifetimeModule.GetWorldSpace(this.m_ParticleSystem)) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World;
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
				}
			}

			/// <summary>
			///   <para>When randomly selecting values between two curves or constants, this flag will cause a new random force to be chosen on each frame.</para>
			/// </summary>
			public bool randomized
			{
				get
				{
					return ParticleSystem.ForceOverLifetimeModule.GetRandomized(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetRandomized(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetWorldSpace(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRandomized(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetRandomized(ParticleSystem system);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Color Over Lifetime module.</para>
		/// </summary>
		public struct ColorOverLifetimeModule
		{
			internal ColorOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Color Over Lifetime module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.ColorOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ColorOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The gradient controlling the particle colors.</para>
			/// </summary>
			public ParticleSystem.MinMaxGradient color
			{
				get
				{
					ParticleSystem.MinMaxGradient minMaxGradient = default(ParticleSystem.MinMaxGradient);
					ParticleSystem.ColorOverLifetimeModule.GetColor(this.m_ParticleSystem, ref minMaxGradient);
					return minMaxGradient;
				}
				set
				{
					ParticleSystem.ColorOverLifetimeModule.SetColor(this.m_ParticleSystem, ref value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Color By Speed module.</para>
		/// </summary>
		public struct ColorBySpeedModule
		{
			internal ColorBySpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Color By Speed module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.ColorBySpeedModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ColorBySpeedModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The gradient controlling the particle colors.</para>
			/// </summary>
			public ParticleSystem.MinMaxGradient color
			{
				get
				{
					ParticleSystem.MinMaxGradient minMaxGradient = default(ParticleSystem.MinMaxGradient);
					ParticleSystem.ColorBySpeedModule.GetColor(this.m_ParticleSystem, ref minMaxGradient);
					return minMaxGradient;
				}
				set
				{
					ParticleSystem.ColorBySpeedModule.SetColor(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Apply the color gradient between these minimum and maximum speeds.</para>
			/// </summary>
			public Vector2 range
			{
				get
				{
					return ParticleSystem.ColorBySpeedModule.GetRange(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ColorBySpeedModule.SetRange(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			private static void SetRange(ParticleSystem system, Vector2 value)
			{
				ParticleSystem.ColorBySpeedModule.INTERNAL_CALL_SetRange(system, ref value);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

			private static Vector2 GetRange(ParticleSystem system)
			{
				Vector2 vector;
				ParticleSystem.ColorBySpeedModule.INTERNAL_CALL_GetRange(system, out vector);
				return vector;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Size Over Lifetime module.</para>
		/// </summary>
		public struct SizeOverLifetimeModule
		{
			internal SizeOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Size Over Lifetime module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle size based on lifetime.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve size
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeOverLifetimeModule.GetX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Size multiplier.</para>
			/// </summary>
			public float sizeMultiplier
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Size over lifetime curve for the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeOverLifetimeModule.GetX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>X axis size multiplier.</para>
			/// </summary>
			public float xMultiplier
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Size over lifetime curve for the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeOverLifetimeModule.GetY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Y axis size multiplier.</para>
			/// </summary>
			public float yMultiplier
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Size over lifetime curve for the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeOverLifetimeModule.GetZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Z axis size multiplier.</para>
			/// </summary>
			public float zMultiplier
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Set the size over lifetime on each axis separately.</para>
			/// </summary>
			public bool separateAxes
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Size By Speed module.</para>
		/// </summary>
		public struct SizeBySpeedModule
		{
			internal SizeBySpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Size By Speed module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Curve to control particle size based on speed.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve size
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeBySpeedModule.GetX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Size multiplier.</para>
			/// </summary>
			public float sizeMultiplier
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Size by speed curve for the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeBySpeedModule.GetX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>X axis size multiplier.</para>
			/// </summary>
			public float xMultiplier
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Size by speed curve for the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeBySpeedModule.GetY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Y axis size multiplier.</para>
			/// </summary>
			public float yMultiplier
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Size by speed curve for the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeBySpeedModule.GetZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Z axis size multiplier.</para>
			/// </summary>
			public float zMultiplier
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Set the size by speed on each axis separately.</para>
			/// </summary>
			public bool separateAxes
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Apply the size curve between these minimum and maximum speeds.</para>
			/// </summary>
			public Vector2 range
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetRange(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetRange(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			private static void SetRange(ParticleSystem system, Vector2 value)
			{
				ParticleSystem.SizeBySpeedModule.INTERNAL_CALL_SetRange(system, ref value);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

			private static Vector2 GetRange(ParticleSystem system)
			{
				Vector2 vector;
				ParticleSystem.SizeBySpeedModule.INTERNAL_CALL_GetRange(system, out vector);
				return vector;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Rotation Over Lifetime module.</para>
		/// </summary>
		public struct RotationOverLifetimeModule
		{
			internal RotationOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Rotation Over Lifetime module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.RotationOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Rotation over lifetime curve for the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationOverLifetimeModule.GetX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Rotation multiplier around the X axis.</para>
			/// </summary>
			public float xMultiplier
			{
				get
				{
					return ParticleSystem.RotationOverLifetimeModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Rotation over lifetime curve for the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationOverLifetimeModule.GetY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Rotation multiplier around the Y axis.</para>
			/// </summary>
			public float yMultiplier
			{
				get
				{
					return ParticleSystem.RotationOverLifetimeModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Rotation over lifetime curve for the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationOverLifetimeModule.GetZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Rotation multiplier around the Z axis.</para>
			/// </summary>
			public float zMultiplier
			{
				get
				{
					return ParticleSystem.RotationOverLifetimeModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Set the rotation over lifetime on each axis separately.</para>
			/// </summary>
			public bool separateAxes
			{
				get
				{
					return ParticleSystem.RotationOverLifetimeModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Rotation By Speed module.</para>
		/// </summary>
		public struct RotationBySpeedModule
		{
			internal RotationBySpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Rotation By Speed module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Rotation by speed curve for the X axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationBySpeedModule.GetX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Speed multiplier along the X axis.</para>
			/// </summary>
			public float xMultiplier
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Rotation by speed curve for the Y axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationBySpeedModule.GetY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Speed multiplier along the Y axis.</para>
			/// </summary>
			public float yMultiplier
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Rotation by speed curve for the Z axis.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationBySpeedModule.GetZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Speed multiplier along the Z axis.</para>
			/// </summary>
			public float zMultiplier
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Set the rotation by speed on each axis separately.</para>
			/// </summary>
			public bool separateAxes
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Apply the rotation curve between these minimum and maximum speeds.</para>
			/// </summary>
			public Vector2 range
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetRange(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetRange(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			private static void SetRange(ParticleSystem system, Vector2 value)
			{
				ParticleSystem.RotationBySpeedModule.INTERNAL_CALL_SetRange(system, ref value);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

			private static Vector2 GetRange(ParticleSystem system)
			{
				Vector2 vector;
				ParticleSystem.RotationBySpeedModule.INTERNAL_CALL_GetRange(system, out vector);
				return vector;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the External Forces module.</para>
		/// </summary>
		public struct ExternalForcesModule
		{
			internal ExternalForcesModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the External Forces module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.ExternalForcesModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ExternalForcesModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Multiplies the magnitude of applied external forces.</para>
			/// </summary>
			public float multiplier
			{
				get
				{
					return ParticleSystem.ExternalForcesModule.GetMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ExternalForcesModule.SetMultiplier(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMultiplier(ParticleSystem system);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Noise Module.
		///
		/// The Noise Module allows you to apply turbulence to the movement of your particles. Use the low quality settings to create computationally efficient Noise, or simulate smoother, richer Noise with the higher quality settings. You can also choose to define the behavior of the Noise individually for each axis.</para>
		/// </summary>
		public struct NoiseModule
		{
			internal NoiseModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Noise module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.NoiseModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Control the noise separately for each axis.</para>
			/// </summary>
			public bool separateAxes
			{
				get
				{
					return ParticleSystem.NoiseModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>How strong the overall noise effect is.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve strength
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetStrengthX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Strength multiplier.</para>
			/// </summary>
			public float strengthMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetStrengthXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Define the strength of the effect on the X axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve strengthX
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetStrengthX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>X axis strength multiplier.</para>
			/// </summary>
			public float strengthXMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetStrengthXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Define the strength of the effect on the Y axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve strengthY
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetStrengthY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Y axis strength multiplier.</para>
			/// </summary>
			public float strengthYMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetStrengthYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Define the strength of the effect on the Z axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve strengthZ
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetStrengthZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Z axis strength multiplier.</para>
			/// </summary>
			public float strengthZMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetStrengthZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Low values create soft, smooth noise, and high values create rapidly changing noise.</para>
			/// </summary>
			public float frequency
			{
				get
				{
					return ParticleSystem.NoiseModule.GetFrequency(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetFrequency(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Higher frequency noise will reduce the strength by a proportional amount, if enabled.</para>
			/// </summary>
			public bool damping
			{
				get
				{
					return ParticleSystem.NoiseModule.GetDamping(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetDamping(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Layers of noise that combine to produce final noise.</para>
			/// </summary>
			public int octaveCount
			{
				get
				{
					return ParticleSystem.NoiseModule.GetOctaveCount(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetOctaveCount(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>When combining each octave, scale the intensity by this amount.</para>
			/// </summary>
			public float octaveMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetOctaveMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetOctaveMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>When combining each octave, zoom in by this amount.</para>
			/// </summary>
			public float octaveScale
			{
				get
				{
					return ParticleSystem.NoiseModule.GetOctaveScale(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetOctaveScale(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Generate 1D, 2D or 3D noise.</para>
			/// </summary>
			public ParticleSystemNoiseQuality quality
			{
				get
				{
					return (ParticleSystemNoiseQuality)ParticleSystem.NoiseModule.GetQuality(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetQuality(this.m_ParticleSystem, (int)value);
				}
			}

			/// <summary>
			///   <para>Scroll the noise map over the particle system.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve scrollSpeed
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetScrollSpeed(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.NoiseModule.SetScrollSpeed(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Scroll speed multiplier.</para>
			/// </summary>
			public float scrollSpeedMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetScrollSpeedMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetScrollSpeedMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Enable remapping of the final noise values, allowing for noise values to be translated into different values.</para>
			/// </summary>
			public bool remapEnabled
			{
				get
				{
					return ParticleSystem.NoiseModule.GetRemapEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Define how the noise values are remapped.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve remap
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetRemapX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Remap multiplier.</para>
			/// </summary>
			public float remapMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetRemapXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Define how the noise values are remapped on the X axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve remapX
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetRemapX(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapX(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>X axis remap multiplier.</para>
			/// </summary>
			public float remapXMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetRemapXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapXMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Define how the noise values are remapped on the Y axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve remapY
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetRemapY(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapY(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Y axis remap multiplier.</para>
			/// </summary>
			public float remapYMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetRemapYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapYMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Define how the noise values are remapped on the Z axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve remapZ
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetRemapZ(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapZ(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Z axis remap multiplier.</para>
			/// </summary>
			public float remapZMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetRemapZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapZMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>How much the noise affects the particle positions.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve positionAmount
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetPositionAmount(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.NoiseModule.SetPositionAmount(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>How much the noise affects the particle rotation, in degrees per second.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve rotationAmount
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetRotationAmount(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.NoiseModule.SetRotationAmount(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>How much the noise affects the particle sizes, applied as a multiplier on the size of each particle.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve sizeAmount
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetSizeAmount(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.NoiseModule.SetSizeAmount(this.m_ParticleSystem, ref value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStrengthX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStrengthX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStrengthY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStrengthY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStrengthZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStrengthZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStrengthXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStrengthXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStrengthYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStrengthYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStrengthZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStrengthZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFrequency(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetFrequency(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDamping(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetDamping(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOctaveCount(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetOctaveCount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOctaveMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetOctaveMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOctaveScale(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetOctaveScale(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetQuality(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetQuality(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetScrollSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetScrollSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetScrollSpeedMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetScrollSpeedMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetRemapEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRemapX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRemapY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRemapZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRemapXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRemapYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRemapZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetPositionAmount(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetPositionAmount(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRotationAmount(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRotationAmount(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSizeAmount(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetSizeAmount(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Collision module.</para>
		/// </summary>
		public struct CollisionModule
		{
			internal CollisionModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Collision module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.CollisionModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The type of particle collision to perform.</para>
			/// </summary>
			public ParticleSystemCollisionType type
			{
				get
				{
					return ParticleSystem.CollisionModule.GetType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetType(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Choose between 2D and 3D world collisions.</para>
			/// </summary>
			public ParticleSystemCollisionMode mode
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMode(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>How much speed is lost from each particle after a collision.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve dampen
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.CollisionModule.GetDampen(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.CollisionModule.SetDampen(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the dampen multiplier.</para>
			/// </summary>
			public float dampenMultiplier
			{
				get
				{
					return ParticleSystem.CollisionModule.GetDampenMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetDampenMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>How much force is applied to each particle after a collision.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve bounce
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.CollisionModule.GetBounce(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.CollisionModule.SetBounce(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the bounce multiplier.</para>
			/// </summary>
			public float bounceMultiplier
			{
				get
				{
					return ParticleSystem.CollisionModule.GetBounceMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetBounceMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>How much a particle's lifetime is reduced after a collision.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve lifetimeLoss
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.CollisionModule.GetLifetimeLoss(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.CollisionModule.SetLifetimeLoss(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the lifetime loss multiplier.</para>
			/// </summary>
			public float lifetimeLossMultiplier
			{
				get
				{
					return ParticleSystem.CollisionModule.GetLifetimeLossMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetLifetimeLossMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Kill particles whose speed falls below this threshold, after a collision.</para>
			/// </summary>
			public float minKillSpeed
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMinKillSpeed(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMinKillSpeed(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Kill particles whose speed goes above this threshold, after a collision.</para>
			/// </summary>
			public float maxKillSpeed
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMaxKillSpeed(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMaxKillSpeed(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Control which layers this particle system collides with.</para>
			/// </summary>
			public LayerMask collidesWith
			{
				get
				{
					return ParticleSystem.CollisionModule.GetCollidesWith(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetCollidesWith(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Allow particles to collide with dynamic colliders when using world collision mode.</para>
			/// </summary>
			public bool enableDynamicColliders
			{
				get
				{
					return ParticleSystem.CollisionModule.GetEnableDynamicColliders(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetEnableDynamicColliders(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The maximum number of collision shapes that will be considered for particle collisions. Excess shapes will be ignored. Terrains take priority.</para>
			/// </summary>
			public int maxCollisionShapes
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMaxCollisionShapes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMaxCollisionShapes(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Specifies the accuracy of particle collisions against colliders in the scene.</para>
			/// </summary>
			public ParticleSystemCollisionQuality quality
			{
				get
				{
					return (ParticleSystemCollisionQuality)ParticleSystem.CollisionModule.GetQuality(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetQuality(this.m_ParticleSystem, (int)value);
				}
			}

			/// <summary>
			///   <para>Size of voxels in the collision cache.</para>
			/// </summary>
			public float voxelSize
			{
				get
				{
					return ParticleSystem.CollisionModule.GetVoxelSize(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetVoxelSize(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>A multiplier applied to the size of each particle before collisions are processed.</para>
			/// </summary>
			public float radiusScale
			{
				get
				{
					return ParticleSystem.CollisionModule.GetRadiusScale(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetRadiusScale(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Send collision callback messages.</para>
			/// </summary>
			public bool sendCollisionMessages
			{
				get
				{
					return ParticleSystem.CollisionModule.GetUsesCollisionMessages(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetUsesCollisionMessages(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>How much force is applied to a Collider when hit by particles from this Particle System.</para>
			/// </summary>
			public float colliderForce
			{
				get
				{
					return ParticleSystem.CollisionModule.GetColliderForce(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetColliderForce(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>If true, the collision angle is considered when applying forces from particles to Colliders.</para>
			/// </summary>
			public bool multiplyColliderForceByCollisionAngle
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMultiplyColliderForceByCollisionAngle(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMultiplyColliderForceByCollisionAngle(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>If true, particle speeds are considered when applying forces to Colliders.</para>
			/// </summary>
			public bool multiplyColliderForceByParticleSpeed
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMultiplyColliderForceByParticleSpeed(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMultiplyColliderForceByParticleSpeed(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>If true, particle sizes are considered when applying forces to Colliders.</para>
			/// </summary>
			public bool multiplyColliderForceByParticleSize
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMultiplyColliderForceByParticleSize(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMultiplyColliderForceByParticleSize(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Set a collision plane to be used with this particle system.</para>
			/// </summary>
			/// <param name="index">Specifies which plane to set.</param>
			/// <param name="transform">The plane to set.</param>
			public void SetPlane(int index, Transform transform)
			{
				ParticleSystem.CollisionModule.SetPlane(this.m_ParticleSystem, index, transform);
			}

			/// <summary>
			///   <para>Get a collision plane associated with this particle system.</para>
			/// </summary>
			/// <param name="index">Specifies which plane to access.</param>
			/// <returns>
			///   <para>The plane.</para>
			/// </returns>
			public Transform GetPlane(int index)
			{
				return ParticleSystem.CollisionModule.GetPlane(this.m_ParticleSystem, index);
			}

			/// <summary>
			///   <para>The maximum number of planes it is possible to set as colliders.</para>
			/// </summary>
			public int maxPlaneCount
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMaxPlaneCount(this.m_ParticleSystem);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetType(ParticleSystem system, ParticleSystemCollisionType value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemCollisionType GetType(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMode(ParticleSystem system, ParticleSystemCollisionMode value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemCollisionMode GetMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDampen(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetDampen(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDampenMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetDampenMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBounce(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetBounce(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBounceMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetBounceMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLifetimeLoss(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetLifetimeLoss(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLifetimeLossMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetLifetimeLossMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMinKillSpeed(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMinKillSpeed(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMaxKillSpeed(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMaxKillSpeed(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCollidesWith(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetCollidesWith(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnableDynamicColliders(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnableDynamicColliders(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnableInteriorCollisions(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnableInteriorCollisions(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMaxCollisionShapes(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxCollisionShapes(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetQuality(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetQuality(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetVoxelSize(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetVoxelSize(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusScale(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadiusScale(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUsesCollisionMessages(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUsesCollisionMessages(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColliderForce(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetColliderForce(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMultiplyColliderForceByCollisionAngle(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetMultiplyColliderForceByCollisionAngle(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMultiplyColliderForceByParticleSpeed(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetMultiplyColliderForceByParticleSpeed(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMultiplyColliderForceByParticleSize(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetMultiplyColliderForceByParticleSize(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetPlane(ParticleSystem system, int index, Transform transform);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Transform GetPlane(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxPlaneCount(ParticleSystem system);

			/// <summary>
			///   <para>Allow particles to collide when inside colliders.</para>
			/// </summary>
			[Obsolete("enableInteriorCollisions property is deprecated and is no longer required and has no effect on the particle system.", false)]
			public bool enableInteriorCollisions
			{
				get
				{
					return ParticleSystem.CollisionModule.GetEnableInteriorCollisions(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetEnableInteriorCollisions(this.m_ParticleSystem, value);
				}
			}

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Trigger module.</para>
		/// </summary>
		public struct TriggerModule
		{
			internal TriggerModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Trigger module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.TriggerModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TriggerModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Choose what action to perform when particles are inside the trigger volume.</para>
			/// </summary>
			public ParticleSystemOverlapAction inside
			{
				get
				{
					return ParticleSystem.TriggerModule.GetInside(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TriggerModule.SetInside(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Choose what action to perform when particles are outside the trigger volume.</para>
			/// </summary>
			public ParticleSystemOverlapAction outside
			{
				get
				{
					return ParticleSystem.TriggerModule.GetOutside(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TriggerModule.SetOutside(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Choose what action to perform when particles enter the trigger volume.</para>
			/// </summary>
			public ParticleSystemOverlapAction enter
			{
				get
				{
					return ParticleSystem.TriggerModule.GetEnter(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TriggerModule.SetEnter(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Choose what action to perform when particles leave the trigger volume.</para>
			/// </summary>
			public ParticleSystemOverlapAction exit
			{
				get
				{
					return ParticleSystem.TriggerModule.GetExit(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TriggerModule.SetExit(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>A multiplier applied to the size of each particle before overlaps are processed.</para>
			/// </summary>
			public float radiusScale
			{
				get
				{
					return ParticleSystem.TriggerModule.GetRadiusScale(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TriggerModule.SetRadiusScale(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Set a collision shape associated with this particle system trigger.</para>
			/// </summary>
			/// <param name="index">Which collider to set.</param>
			/// <param name="collider">The collider to associate with this trigger.</param>
			public void SetCollider(int index, Component collider)
			{
				ParticleSystem.TriggerModule.SetCollider(this.m_ParticleSystem, index, collider);
			}

			/// <summary>
			///   <para>Get a collision shape associated with this particle system trigger.</para>
			/// </summary>
			/// <param name="index">Which collider to return.</param>
			/// <returns>
			///   <para>The collider at the given index.</para>
			/// </returns>
			public Component GetCollider(int index)
			{
				return ParticleSystem.TriggerModule.GetCollider(this.m_ParticleSystem, index);
			}

			/// <summary>
			///   <para>The maximum number of collision shapes that can be attached to this particle system trigger.</para>
			/// </summary>
			public int maxColliderCount
			{
				get
				{
					return ParticleSystem.TriggerModule.GetMaxColliderCount(this.m_ParticleSystem);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetInside(ParticleSystem system, ParticleSystemOverlapAction value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemOverlapAction GetInside(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOutside(ParticleSystem system, ParticleSystemOverlapAction value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemOverlapAction GetOutside(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnter(ParticleSystem system, ParticleSystemOverlapAction value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemOverlapAction GetEnter(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetExit(ParticleSystem system, ParticleSystemOverlapAction value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemOverlapAction GetExit(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusScale(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadiusScale(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCollider(ParticleSystem system, int index, Component collider);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Component GetCollider(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxColliderCount(ParticleSystem system);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Sub Emitters module.</para>
		/// </summary>
		public struct SubEmittersModule
		{
			internal SubEmittersModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Sub Emitters module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The total number of sub-emitters.</para>
			/// </summary>
			public int subEmittersCount
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetSubEmittersCount(this.m_ParticleSystem);
				}
			}

			/// <summary>
			///   <para>Add a new sub-emitter.</para>
			/// </summary>
			/// <param name="subEmitter">The sub-emitter to be added.</param>
			/// <param name="type">The event that creates new particles.</param>
			/// <param name="properties">The properties of the new particles.</param>
			public void AddSubEmitter(ParticleSystem subEmitter, ParticleSystemSubEmitterType type, ParticleSystemSubEmitterProperties properties)
			{
				ParticleSystem.SubEmittersModule.AddSubEmitter(this.m_ParticleSystem, subEmitter, (int)type, (int)properties);
			}

			/// <summary>
			///   <para>Remove a sub-emitter from the given index in the array.</para>
			/// </summary>
			/// <param name="index">The index from which to remove a sub-emitter.</param>
			public void RemoveSubEmitter(int index)
			{
				ParticleSystem.SubEmittersModule.RemoveSubEmitter(this.m_ParticleSystem, index);
			}

			/// <summary>
			///   <para>Set the Particle System to use as the sub-emitter at the given index.</para>
			/// </summary>
			/// <param name="index">The index of the sub-emitter being modified.</param>
			/// <param name="subEmitter">The Particle System being used as this sub-emitter.</param>
			public void SetSubEmitterSystem(int index, ParticleSystem subEmitter)
			{
				ParticleSystem.SubEmittersModule.SetSubEmitterSystem(this.m_ParticleSystem, index, subEmitter);
			}

			/// <summary>
			///   <para>Set the type of the sub-emitter at the given index.</para>
			/// </summary>
			/// <param name="index">The index of the sub-emitter being modified.</param>
			/// <param name="type">The new spawning type to assign to this sub-emitter.</param>
			public void SetSubEmitterType(int index, ParticleSystemSubEmitterType type)
			{
				ParticleSystem.SubEmittersModule.SetSubEmitterType(this.m_ParticleSystem, index, (int)type);
			}

			/// <summary>
			///   <para>Set the properties of the sub-emitter at the given index.</para>
			/// </summary>
			/// <param name="index">The index of the sub-emitter being modified.</param>
			/// <param name="properties">The new properties to assign to this sub-emitter.</param>
			public void SetSubEmitterProperties(int index, ParticleSystemSubEmitterProperties properties)
			{
				ParticleSystem.SubEmittersModule.SetSubEmitterProperties(this.m_ParticleSystem, index, (int)properties);
			}

			/// <summary>
			///   <para>Get the sub-emitter Particle System at the given index.</para>
			/// </summary>
			/// <param name="index">The index of the desired sub-emitter.</param>
			/// <returns>
			///   <para>The sub-emitter being requested.</para>
			/// </returns>
			public ParticleSystem GetSubEmitterSystem(int index)
			{
				return ParticleSystem.SubEmittersModule.GetSubEmitterSystem(this.m_ParticleSystem, index);
			}

			/// <summary>
			///   <para>Get the type of the sub-emitter at the given index.</para>
			/// </summary>
			/// <param name="index">The index of the desired sub-emitter.</param>
			/// <returns>
			///   <para>The type of the requested sub-emitter.</para>
			/// </returns>
			public ParticleSystemSubEmitterType GetSubEmitterType(int index)
			{
				return (ParticleSystemSubEmitterType)ParticleSystem.SubEmittersModule.GetSubEmitterType(this.m_ParticleSystem, index);
			}

			/// <summary>
			///   <para>Get the properties of the sub-emitter at the given index.</para>
			/// </summary>
			/// <param name="index">The index of the desired sub-emitter.</param>
			/// <returns>
			///   <para>The properties of the requested sub-emitter.</para>
			/// </returns>
			public ParticleSystemSubEmitterProperties GetSubEmitterProperties(int index)
			{
				return (ParticleSystemSubEmitterProperties)ParticleSystem.SubEmittersModule.GetSubEmitterProperties(this.m_ParticleSystem, index);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetSubEmittersCount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBirth(ParticleSystem system, int index, ParticleSystem value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetBirth(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCollision(ParticleSystem system, int index, ParticleSystem value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetCollision(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDeath(ParticleSystem system, int index, ParticleSystem value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetDeath(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void AddSubEmitter(ParticleSystem system, ParticleSystem subEmitter, int type, int properties);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void RemoveSubEmitter(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSubEmitterSystem(ParticleSystem system, int index, ParticleSystem subEmitter);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSubEmitterType(ParticleSystem system, int index, int type);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSubEmitterProperties(ParticleSystem system, int index, int properties);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetSubEmitterSystem(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetSubEmitterType(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetSubEmitterProperties(ParticleSystem system, int index);

			/// <summary>
			///   <para>Sub particle system which spawns at the locations of the birth of the particles from the parent system.</para>
			/// </summary>
			[Obsolete("birth0 property is deprecated.Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
			public ParticleSystem birth0
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetBirth(this.m_ParticleSystem, 0);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetBirth(this.m_ParticleSystem, 0, value);
				}
			}

			/// <summary>
			///   <para>Sub particle system which spawns at the locations of the birth of the particles from the parent system.</para>
			/// </summary>
			[Obsolete("birth1 property is deprecated.Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
			public ParticleSystem birth1
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetBirth(this.m_ParticleSystem, 1);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetBirth(this.m_ParticleSystem, 1, value);
				}
			}

			/// <summary>
			///   <para>Sub particle system which spawns at the locations of the collision of the particles from the parent system.</para>
			/// </summary>
			[Obsolete("collision0 property is deprecated.Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
			public ParticleSystem collision0
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetCollision(this.m_ParticleSystem, 0);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetCollision(this.m_ParticleSystem, 0, value);
				}
			}

			/// <summary>
			///   <para>Sub particle system which spawns at the locations of the collision of the particles from the parent system.</para>
			/// </summary>
			[Obsolete("collision1 property is deprecated.Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
			public ParticleSystem collision1
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetCollision(this.m_ParticleSystem, 1);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetCollision(this.m_ParticleSystem, 1, value);
				}
			}

			/// <summary>
			///   <para>Sub particle system which spawns at the locations of the death of the particles from the parent system.</para>
			/// </summary>
			[Obsolete("death0 property is deprecated.Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
			public ParticleSystem death0
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetDeath(this.m_ParticleSystem, 0);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetDeath(this.m_ParticleSystem, 0, value);
				}
			}

			/// <summary>
			///   <para>Sub particle system to spawn on death of the parent system's particles.</para>
			/// </summary>
			[Obsolete("death1 property is deprecated.Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
			public ParticleSystem death1
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetDeath(this.m_ParticleSystem, 1);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetDeath(this.m_ParticleSystem, 1, value);
				}
			}

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Texture Sheet Animation module.</para>
		/// </summary>
		public struct TextureSheetAnimationModule
		{
			internal TextureSheetAnimationModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Texture Sheet Animation module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Select whether the animated texture information comes from a grid of frames on a single texture, or from a list of Sprite objects.</para>
			/// </summary>
			public ParticleSystemAnimationMode mode
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetMode(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Defines the tiling of the texture in the X axis.</para>
			/// </summary>
			public int numTilesX
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetNumTilesX(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetNumTilesX(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Defines the tiling of the texture in the Y axis.</para>
			/// </summary>
			public int numTilesY
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetNumTilesY(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetNumTilesY(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Specifies the animation type.</para>
			/// </summary>
			public ParticleSystemAnimationType animation
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetAnimationType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetAnimationType(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Use a random row of the texture sheet for each particle emitted.</para>
			/// </summary>
			public bool useRandomRow
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetUseRandomRow(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetUseRandomRow(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Curve to control which frame of the texture sheet animation to play.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve frameOverTime
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.TextureSheetAnimationModule.GetFrameOverTime(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetFrameOverTime(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Frame over time mutiplier.</para>
			/// </summary>
			public float frameOverTimeMultiplier
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetFrameOverTimeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetFrameOverTimeMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Define a random starting frame for the texture sheet animation.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve startFrame
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.TextureSheetAnimationModule.GetStartFrame(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetStartFrame(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Starting frame multiplier.</para>
			/// </summary>
			public float startFrameMultiplier
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetStartFrameMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetStartFrameMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Specifies how many times the animation will loop during the lifetime of the particle.</para>
			/// </summary>
			public int cycleCount
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetCycleCount(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetCycleCount(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Explicitly select which row of the texture sheet is used, when ParticleSystem.TextureSheetAnimationModule.useRandomRow is set to false.</para>
			/// </summary>
			public int rowIndex
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetRowIndex(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetRowIndex(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Choose which UV channels will receive texture animation.</para>
			/// </summary>
			public UVChannelFlags uvChannelMask
			{
				get
				{
					return (UVChannelFlags)ParticleSystem.TextureSheetAnimationModule.GetUVChannelMask(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetUVChannelMask(this.m_ParticleSystem, (int)value);
				}
			}

			/// <summary>
			///   <para>Flip the U coordinate on particles, causing them to appear mirrored horizontally.</para>
			/// </summary>
			public float flipU
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetFlipU(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetFlipU(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Flip the V coordinate on particles, causing them to appear mirrored vertically.</para>
			/// </summary>
			public float flipV
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetFlipV(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetFlipV(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The total number of sprites.</para>
			/// </summary>
			public int spriteCount
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetSpriteCount(this.m_ParticleSystem);
				}
			}

			/// <summary>
			///   <para>Add a new Sprite.</para>
			/// </summary>
			/// <param name="sprite">The Sprite to be added.</param>
			public void AddSprite(Sprite sprite)
			{
				ParticleSystem.TextureSheetAnimationModule.AddSprite(this.m_ParticleSystem, sprite);
			}

			/// <summary>
			///   <para>Remove a Sprite from the given index in the array.</para>
			/// </summary>
			/// <param name="index">The index from which to remove a Sprite.</param>
			public void RemoveSprite(int index)
			{
				ParticleSystem.TextureSheetAnimationModule.RemoveSprite(this.m_ParticleSystem, index);
			}

			/// <summary>
			///   <para>Set the Sprite at the given index.</para>
			/// </summary>
			/// <param name="index">The index of the Sprite being modified.</param>
			/// <param name="sprite">The Sprite being assigned.</param>
			public void SetSprite(int index, Sprite sprite)
			{
				ParticleSystem.TextureSheetAnimationModule.SetSprite(this.m_ParticleSystem, index, sprite);
			}

			/// <summary>
			///   <para>Get the Sprite at the given index.</para>
			/// </summary>
			/// <param name="index">The index of the desired Sprite.</param>
			/// <returns>
			///   <para>The Sprite being requested.</para>
			/// </returns>
			public Sprite GetSprite(int index)
			{
				return ParticleSystem.TextureSheetAnimationModule.GetSprite(this.m_ParticleSystem, index);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMode(ParticleSystem system, ParticleSystemAnimationMode value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemAnimationMode GetMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetNumTilesX(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetNumTilesX(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetNumTilesY(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetNumTilesY(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetAnimationType(ParticleSystem system, ParticleSystemAnimationType value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemAnimationType GetAnimationType(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseRandomRow(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseRandomRow(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFrameOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetFrameOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFrameOverTimeMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetFrameOverTimeMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartFrame(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartFrame(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartFrameMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartFrameMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCycleCount(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetCycleCount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRowIndex(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetRowIndex(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUVChannelMask(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetUVChannelMask(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFlipU(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetFlipU(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFlipV(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetFlipV(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetSpriteCount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void AddSprite(ParticleSystem system, Object sprite);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void RemoveSprite(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSprite(ParticleSystem system, int index, Object sprite);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Sprite GetSprite(ParticleSystem system, int index);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Access the ParticleSystem Lights Module.</para>
		/// </summary>
		public struct LightsModule
		{
			internal LightsModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Lights module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.LightsModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Choose what proportion of particles will receive a dynamic light.</para>
			/// </summary>
			public float ratio
			{
				get
				{
					return ParticleSystem.LightsModule.GetRatio(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetRatio(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Randomly assign lights to new particles based on ParticleSystem.LightsModule.ratio.</para>
			/// </summary>
			public bool useRandomDistribution
			{
				get
				{
					return ParticleSystem.LightsModule.GetUseRandomDistribution(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetUseRandomDistribution(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Select what Light prefab you want to base your particle lights on.</para>
			/// </summary>
			public Light light
			{
				get
				{
					return ParticleSystem.LightsModule.GetLightPrefab(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetLightPrefab(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Toggle whether the particle lights will have their color multiplied by the particle color.</para>
			/// </summary>
			public bool useParticleColor
			{
				get
				{
					return ParticleSystem.LightsModule.GetUseParticleColor(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetUseParticleColor(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Toggle where the particle size will be multiplied by the light range, to determine the final light range.</para>
			/// </summary>
			public bool sizeAffectsRange
			{
				get
				{
					return ParticleSystem.LightsModule.GetSizeAffectsRange(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetSizeAffectsRange(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Toggle whether the particle alpha gets multiplied by the light intensity, when computing the final light intensity.</para>
			/// </summary>
			public bool alphaAffectsIntensity
			{
				get
				{
					return ParticleSystem.LightsModule.GetAlphaAffectsIntensity(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetAlphaAffectsIntensity(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Define a curve to apply custom range scaling to particle lights.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve range
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LightsModule.GetRange(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.LightsModule.SetRange(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Range multiplier.</para>
			/// </summary>
			public float rangeMultiplier
			{
				get
				{
					return ParticleSystem.LightsModule.GetRangeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetRangeMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Define a curve to apply custom intensity scaling to particle lights.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve intensity
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LightsModule.GetIntensity(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.LightsModule.SetIntensity(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Intensity multiplier.</para>
			/// </summary>
			public float intensityMultiplier
			{
				get
				{
					return ParticleSystem.LightsModule.GetIntensityMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetIntensityMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Set a limit on how many lights this Module can create.</para>
			/// </summary>
			public int maxLights
			{
				get
				{
					return ParticleSystem.LightsModule.GetMaxLights(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetMaxLights(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRatio(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRatio(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseRandomDistribution(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseRandomDistribution(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLightPrefab(ParticleSystem system, Light value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Light GetLightPrefab(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseParticleColor(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseParticleColor(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSizeAffectsRange(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSizeAffectsRange(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetAlphaAffectsIntensity(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetAlphaAffectsIntensity(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRange(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRange(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRangeMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRangeMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetIntensity(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetIntensity(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetIntensityMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetIntensityMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMaxLights(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxLights(ParticleSystem system);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Access the particle system trails module.</para>
		/// </summary>
		public struct TrailModule
		{
			internal TrailModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Trail module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.TrailModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Choose how particle trails are generated.</para>
			/// </summary>
			public ParticleSystemTrailMode mode
			{
				get
				{
					return ParticleSystem.TrailModule.GetMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetMode(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Choose what proportion of particles will receive a trail.</para>
			/// </summary>
			public float ratio
			{
				get
				{
					return ParticleSystem.TrailModule.GetRatio(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetRatio(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The curve describing the trail lifetime, throughout the lifetime of the particle.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve lifetime
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.TrailModule.GetLifetime(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.TrailModule.SetLifetime(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the lifetime multiplier.</para>
			/// </summary>
			public float lifetimeMultiplier
			{
				get
				{
					return ParticleSystem.TrailModule.GetLifetimeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetLifetimeMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Set the minimum distance each trail can travel before a new vertex is added to it.</para>
			/// </summary>
			public float minVertexDistance
			{
				get
				{
					return ParticleSystem.TrailModule.GetMinVertexDistance(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetMinVertexDistance(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Choose whether the U coordinate of the trail texture is tiled or stretched.</para>
			/// </summary>
			public ParticleSystemTrailTextureMode textureMode
			{
				get
				{
					return ParticleSystem.TrailModule.GetTextureMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetTextureMode(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Drop new trail points in world space, regardless of Particle System Simulation Space.</para>
			/// </summary>
			public bool worldSpace
			{
				get
				{
					return ParticleSystem.TrailModule.GetWorldSpace(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetWorldSpace(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>If enabled, Trails will disappear immediately when their owning particle dies. Otherwise, the trail will persist until all its points have naturally expired, based on its lifetime.</para>
			/// </summary>
			public bool dieWithParticles
			{
				get
				{
					return ParticleSystem.TrailModule.GetDieWithParticles(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetDieWithParticles(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Set whether the particle size will act as a multiplier on top of the trail width.</para>
			/// </summary>
			public bool sizeAffectsWidth
			{
				get
				{
					return ParticleSystem.TrailModule.GetSizeAffectsWidth(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetSizeAffectsWidth(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Set whether the particle size will act as a multiplier on top of the trail lifetime.</para>
			/// </summary>
			public bool sizeAffectsLifetime
			{
				get
				{
					return ParticleSystem.TrailModule.GetSizeAffectsLifetime(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetSizeAffectsLifetime(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Toggle whether the trail will inherit the particle color as its starting color.</para>
			/// </summary>
			public bool inheritParticleColor
			{
				get
				{
					return ParticleSystem.TrailModule.GetInheritParticleColor(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetInheritParticleColor(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The gradient controlling the trail colors during the lifetime of the attached particle.</para>
			/// </summary>
			public ParticleSystem.MinMaxGradient colorOverLifetime
			{
				get
				{
					ParticleSystem.MinMaxGradient minMaxGradient = default(ParticleSystem.MinMaxGradient);
					ParticleSystem.TrailModule.GetColorOverLifetime(this.m_ParticleSystem, ref minMaxGradient);
					return minMaxGradient;
				}
				set
				{
					ParticleSystem.TrailModule.SetColorOverLifetime(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>The curve describing the width, of each trail point.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve widthOverTrail
			{
				get
				{
					ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.TrailModule.GetWidthOverTrail(this.m_ParticleSystem, ref minMaxCurve);
					return minMaxCurve;
				}
				set
				{
					ParticleSystem.TrailModule.SetWidthOverTrail(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Change the width multiplier.</para>
			/// </summary>
			public float widthOverTrailMultiplier
			{
				get
				{
					return ParticleSystem.TrailModule.GetWidthOverTrailMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetWidthOverTrailMultiplier(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>The gradient controlling the trail colors over the length of the trail.</para>
			/// </summary>
			public ParticleSystem.MinMaxGradient colorOverTrail
			{
				get
				{
					ParticleSystem.MinMaxGradient minMaxGradient = default(ParticleSystem.MinMaxGradient);
					ParticleSystem.TrailModule.GetColorOverTrail(this.m_ParticleSystem, ref minMaxGradient);
					return minMaxGradient;
				}
				set
				{
					ParticleSystem.TrailModule.SetColorOverTrail(this.m_ParticleSystem, ref value);
				}
			}

			/// <summary>
			///   <para>Configures the trails to generate Normals and Tangents. With this data, Scene lighting can affect the trails via Normal Maps and the Unity Standard Shader, or your own custom-built Shaders.</para>
			/// </summary>
			public bool generateLightingData
			{
				get
				{
					return ParticleSystem.TrailModule.GetGenerateLightingData(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetGenerateLightingData(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Select how many lines to create through the Particle System.</para>
			/// </summary>
			public int ribbonCount
			{
				get
				{
					return ParticleSystem.TrailModule.GetRibbonCount(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetRibbonCount(this.m_ParticleSystem, value);
				}
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMode(ParticleSystem system, ParticleSystemTrailMode value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemTrailMode GetMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRatio(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRatio(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLifetime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetLifetime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLifetimeMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetLifetimeMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMinVertexDistance(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMinVertexDistance(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetTextureMode(ParticleSystem system, ParticleSystemTrailTextureMode value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemTrailTextureMode GetTextureMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetWorldSpace(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDieWithParticles(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetDieWithParticles(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSizeAffectsWidth(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSizeAffectsWidth(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSizeAffectsLifetime(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSizeAffectsLifetime(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetInheritParticleColor(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetInheritParticleColor(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColorOverLifetime(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColorOverLifetime(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWidthOverTrail(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetWidthOverTrail(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWidthOverTrailMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetWidthOverTrailMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColorOverTrail(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColorOverTrail(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetGenerateLightingData(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetGenerateLightingData(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRibbonCount(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetRibbonCount(ParticleSystem system);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for the Custom Data module.</para>
		/// </summary>
		public struct CustomDataModule
		{
			internal CustomDataModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			/// <summary>
			///   <para>Enable/disable the Custom Data module.</para>
			/// </summary>
			public bool enabled
			{
				get
				{
					return ParticleSystem.CustomDataModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CustomDataModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			/// <summary>
			///   <para>Choose the type of custom data to generate for the chosen data stream.</para>
			/// </summary>
			/// <param name="stream">The name of the custom data stream to enable data generation on.</param>
			/// <param name="mode">The type of data to generate.</param>
			public void SetMode(ParticleSystemCustomData stream, ParticleSystemCustomDataMode mode)
			{
				ParticleSystem.CustomDataModule.SetMode(this.m_ParticleSystem, (int)stream, mode);
			}

			/// <summary>
			///   <para>Find out the type of custom data that is being generated for the chosen data stream.</para>
			/// </summary>
			/// <param name="stream">The name of the custom data stream to query.</param>
			/// <returns>
			///   <para>The type of data being generated for the requested stream.</para>
			/// </returns>
			public ParticleSystemCustomDataMode GetMode(ParticleSystemCustomData stream)
			{
				return ParticleSystem.CustomDataModule.GetMode(this.m_ParticleSystem, (int)stream);
			}

			/// <summary>
			///   <para>Specify how many curves are used to generate custom data for this stream.</para>
			/// </summary>
			/// <param name="stream">The name of the custom data stream to apply the curve to.</param>
			/// <param name="curveCount">The number of curves to generate data for.</param>
			/// <param name="count"></param>
			public void SetVectorComponentCount(ParticleSystemCustomData stream, int count)
			{
				ParticleSystem.CustomDataModule.SetVectorComponentCount(this.m_ParticleSystem, (int)stream, count);
			}

			/// <summary>
			///   <para>Query how many ParticleSystem.MinMaxCurve elements are being used to generate this stream of custom data.</para>
			/// </summary>
			/// <param name="stream">The name of the custom data stream to retrieve the curve from.</param>
			/// <returns>
			///   <para>The number of curves.</para>
			/// </returns>
			public int GetVectorComponentCount(ParticleSystemCustomData stream)
			{
				return ParticleSystem.CustomDataModule.GetVectorComponentCount(this.m_ParticleSystem, (int)stream);
			}

			public void SetVector(ParticleSystemCustomData stream, int component, ParticleSystem.MinMaxCurve curve)
			{
				ParticleSystem.CustomDataModule.SetVector(this.m_ParticleSystem, (int)stream, component, ref curve);
			}

			/// <summary>
			///   <para>Get a ParticleSystem.MinMaxCurve, that is being used to generate custom data.</para>
			/// </summary>
			/// <param name="stream">The name of the custom data stream to retrieve the curve from.</param>
			/// <param name="component">The component index to retrieve the curve for (0-3, mapping to the xyzw components of a Vector4 or float4).</param>
			/// <returns>
			///   <para>The curve being used to generate custom data.</para>
			/// </returns>
			public ParticleSystem.MinMaxCurve GetVector(ParticleSystemCustomData stream, int component)
			{
				ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
				ParticleSystem.CustomDataModule.GetVector(this.m_ParticleSystem, (int)stream, component, ref minMaxCurve);
				return minMaxCurve;
			}

			public void SetColor(ParticleSystemCustomData stream, ParticleSystem.MinMaxGradient gradient)
			{
				ParticleSystem.CustomDataModule.SetColor(this.m_ParticleSystem, (int)stream, ref gradient);
			}

			/// <summary>
			///   <para>Get a ParticleSystem.MinMaxGradient, that is being used to generate custom HDR color data.</para>
			/// </summary>
			/// <param name="stream">The name of the custom data stream to retrieve the gradient from.</param>
			/// <returns>
			///   <para>The color gradient being used to generate custom color data.</para>
			/// </returns>
			public ParticleSystem.MinMaxGradient GetColor(ParticleSystemCustomData stream)
			{
				ParticleSystem.MinMaxGradient minMaxGradient = default(ParticleSystem.MinMaxGradient);
				ParticleSystem.CustomDataModule.GetColor(this.m_ParticleSystem, (int)stream, ref minMaxGradient);
				return minMaxGradient;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMode(ParticleSystem system, int stream, ParticleSystemCustomDataMode mode);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetVectorComponentCount(ParticleSystem system, int stream, int count);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetVector(ParticleSystem system, int stream, int component, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColor(ParticleSystem system, int stream, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemCustomDataMode GetMode(ParticleSystem system, int stream);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetVectorComponentCount(ParticleSystem system, int stream);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetVector(ParticleSystem system, int stream, int component, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColor(ParticleSystem system, int stream, ref ParticleSystem.MinMaxGradient gradient);

			private ParticleSystem m_ParticleSystem;
		}

		/// <summary>
		///   <para>Script interface for a Particle.</para>
		/// </summary>
		[RequiredByNativeCode("particleSystemParticle", Optional = true)]
		public struct Particle
		{
			/// <summary>
			///   <para>The lifetime of the particle.</para>
			/// </summary>
			[Obsolete("Please use Particle.remainingLifetime instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/Particle.remainingLifetime", false)]
			public float lifetime
			{
				get
				{
					return this.remainingLifetime;
				}
				set
				{
					this.remainingLifetime = value;
				}
			}

			/// <summary>
			///   <para>The random value of the particle.</para>
			/// </summary>
			[Obsolete("randomValue property is deprecated.Use randomSeed instead to control random behavior of particles.", false)]
			public float randomValue
			{
				get
				{
					return BitConverter.ToSingle(BitConverter.GetBytes(this.m_RandomSeed), 0);
				}
				set
				{
					this.m_RandomSeed = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
				}
			}

			[Obsolete("size property is deprecated.Use startSize or GetCurrentSize() instead.", false)]
			public float size
			{
				get
				{
					return this.startSize;
				}
				set
				{
					this.startSize = value;
				}
			}

			[Obsolete("color property is deprecated.Use startColor or GetCurrentColor() instead.", false)]
			public Color32 color
			{
				get
				{
					return this.startColor;
				}
				set
				{
					this.startColor = value;
				}
			}

			/// <summary>
			///   <para>The position of the particle.</para>
			/// </summary>
			public Vector3 position
			{
				get
				{
					return this.m_Position;
				}
				set
				{
					this.m_Position = value;
				}
			}

			/// <summary>
			///   <para>The velocity of the particle.</para>
			/// </summary>
			public Vector3 velocity
			{
				get
				{
					return this.m_Velocity;
				}
				set
				{
					this.m_Velocity = value;
				}
			}

			/// <summary>
			///   <para>The animated velocity of the particle.</para>
			/// </summary>
			public Vector3 animatedVelocity
			{
				get
				{
					return this.m_AnimatedVelocity;
				}
			}

			/// <summary>
			///   <para>The total velocity of the particle.</para>
			/// </summary>
			public Vector3 totalVelocity
			{
				get
				{
					return this.m_Velocity + this.m_AnimatedVelocity;
				}
			}

			/// <summary>
			///   <para>The remaining lifetime of the particle.</para>
			/// </summary>
			public float remainingLifetime
			{
				get
				{
					return this.m_Lifetime;
				}
				set
				{
					this.m_Lifetime = value;
				}
			}

			/// <summary>
			///   <para>The starting lifetime of the particle.</para>
			/// </summary>
			public float startLifetime
			{
				get
				{
					return this.m_StartLifetime;
				}
				set
				{
					this.m_StartLifetime = value;
				}
			}

			/// <summary>
			///   <para>The initial color of the particle. The current color of the particle is calculated procedurally based on this value and the active color modules.</para>
			/// </summary>
			public Color32 startColor
			{
				get
				{
					return this.m_StartColor;
				}
				set
				{
					this.m_StartColor = value;
				}
			}

			/// <summary>
			///   <para>The random seed of the particle.</para>
			/// </summary>
			public uint randomSeed
			{
				get
				{
					return this.m_RandomSeed;
				}
				set
				{
					this.m_RandomSeed = value;
				}
			}

			public Vector3 axisOfRotation
			{
				get
				{
					return this.m_AxisOfRotation;
				}
				set
				{
					this.m_AxisOfRotation = value;
				}
			}

			/// <summary>
			///   <para>The initial size of the particle. The current size of the particle is calculated procedurally based on this value and the active size modules.</para>
			/// </summary>
			public float startSize
			{
				get
				{
					return this.m_StartSize.x;
				}
				set
				{
					this.m_StartSize = new Vector3(value, value, value);
				}
			}

			/// <summary>
			///   <para>The initial 3D size of the particle. The current size of the particle is calculated procedurally based on this value and the active size modules.</para>
			/// </summary>
			public Vector3 startSize3D
			{
				get
				{
					return this.m_StartSize;
				}
				set
				{
					this.m_StartSize = value;
				}
			}

			/// <summary>
			///   <para>The rotation of the particle.</para>
			/// </summary>
			public float rotation
			{
				get
				{
					return this.m_Rotation.z * 57.29578f;
				}
				set
				{
					this.m_Rotation = new Vector3(0f, 0f, value * 0.017453292f);
				}
			}

			/// <summary>
			///   <para>The 3D rotation of the particle.</para>
			/// </summary>
			public Vector3 rotation3D
			{
				get
				{
					return this.m_Rotation * 57.29578f;
				}
				set
				{
					this.m_Rotation = value * 0.017453292f;
				}
			}

			/// <summary>
			///   <para>The angular velocity of the particle.</para>
			/// </summary>
			public float angularVelocity
			{
				get
				{
					return this.m_AngularVelocity.z * 57.29578f;
				}
				set
				{
					this.m_AngularVelocity = new Vector3(0f, 0f, value * 0.017453292f);
				}
			}

			/// <summary>
			///   <para>The 3D angular velocity of the particle.</para>
			/// </summary>
			public Vector3 angularVelocity3D
			{
				get
				{
					return this.m_AngularVelocity * 57.29578f;
				}
				set
				{
					this.m_AngularVelocity = value * 0.017453292f;
				}
			}

			/// <summary>
			///   <para>Calculate the current size of the particle by applying the relevant curves to its startSize property.</para>
			/// </summary>
			/// <param name="system">The particle system from which this particle was emitted.</param>
			/// <returns>
			///   <para>Current size.</para>
			/// </returns>
			public float GetCurrentSize(ParticleSystem system)
			{
				return system.GetParticleCurrentSize(ref this);
			}

			/// <summary>
			///   <para>Calculate the current 3D size of the particle by applying the relevant curves to its startSize3D property.</para>
			/// </summary>
			/// <param name="system">The particle system from which this particle was emitted.</param>
			/// <returns>
			///   <para>Current size.</para>
			/// </returns>
			public Vector3 GetCurrentSize3D(ParticleSystem system)
			{
				return system.GetParticleCurrentSize3D(ref this);
			}

			/// <summary>
			///   <para>Calculate the current color of the particle by applying the relevant curves to its startColor property.</para>
			/// </summary>
			/// <param name="system">The particle system from which this particle was emitted.</param>
			/// <returns>
			///   <para>Current color.</para>
			/// </returns>
			public Color32 GetCurrentColor(ParticleSystem system)
			{
				return system.GetParticleCurrentColor(ref this);
			}

			private Vector3 m_Position;

			private Vector3 m_Velocity;

			private Vector3 m_AnimatedVelocity;

			private Vector3 m_InitialVelocity;

			private Vector3 m_AxisOfRotation;

			private Vector3 m_Rotation;

			private Vector3 m_AngularVelocity;

			private Vector3 m_StartSize;

			private Color32 m_StartColor;

			private uint m_RandomSeed;

			private float m_Lifetime;

			private float m_StartLifetime;

			private float m_EmitAccumulator0;

			private float m_EmitAccumulator1;
		}

		/// <summary>
		///   <para>Script interface for a Burst.</para>
		/// </summary>
		public struct Burst
		{
			/// <summary>
			///   <para>Construct a new Burst with a time and count.</para>
			/// </summary>
			/// <param name="_time">Time to emit the burst.</param>
			/// <param name="_minCount">Minimum number of particles to emit.</param>
			/// <param name="_maxCount">Maximum number of particles to emit.</param>
			/// <param name="_count">Number of particles to emit.</param>
			/// <param name="_cycleCount">Number of times to play the burst. (0 means indefinitely).</param>
			/// <param name="_repeatInterval">How often to repeat the burst, in seconds.</param>
			public Burst(float _time, short _count)
			{
				this.m_Time = _time;
				this.m_Count = (float)_count;
				this.m_RepeatCount = 0;
				this.m_RepeatInterval = 0f;
			}

			/// <summary>
			///   <para>Construct a new Burst with a time and count.</para>
			/// </summary>
			/// <param name="_time">Time to emit the burst.</param>
			/// <param name="_minCount">Minimum number of particles to emit.</param>
			/// <param name="_maxCount">Maximum number of particles to emit.</param>
			/// <param name="_count">Number of particles to emit.</param>
			/// <param name="_cycleCount">Number of times to play the burst. (0 means indefinitely).</param>
			/// <param name="_repeatInterval">How often to repeat the burst, in seconds.</param>
			public Burst(float _time, short _minCount, short _maxCount)
			{
				this.m_Time = _time;
				this.m_Count = new ParticleSystem.MinMaxCurve((float)_minCount, (float)_maxCount);
				this.m_RepeatCount = 0;
				this.m_RepeatInterval = 0f;
			}

			/// <summary>
			///   <para>Construct a new Burst with a time and count.</para>
			/// </summary>
			/// <param name="_time">Time to emit the burst.</param>
			/// <param name="_minCount">Minimum number of particles to emit.</param>
			/// <param name="_maxCount">Maximum number of particles to emit.</param>
			/// <param name="_count">Number of particles to emit.</param>
			/// <param name="_cycleCount">Number of times to play the burst. (0 means indefinitely).</param>
			/// <param name="_repeatInterval">How often to repeat the burst, in seconds.</param>
			public Burst(float _time, short _minCount, short _maxCount, int _cycleCount, float _repeatInterval)
			{
				this.m_Time = _time;
				this.m_Count = new ParticleSystem.MinMaxCurve((float)_minCount, (float)_maxCount);
				this.m_RepeatCount = _cycleCount - 1;
				this.m_RepeatInterval = _repeatInterval;
			}

			public Burst(float _time, ParticleSystem.MinMaxCurve _count)
			{
				this.m_Time = _time;
				this.m_Count = _count;
				this.m_RepeatCount = 0;
				this.m_RepeatInterval = 0f;
			}

			public Burst(float _time, ParticleSystem.MinMaxCurve _count, int _cycleCount, float _repeatInterval)
			{
				this.m_Time = _time;
				this.m_Count = _count;
				this.m_RepeatCount = _cycleCount - 1;
				this.m_RepeatInterval = _repeatInterval;
			}

			/// <summary>
			///   <para>The time that each burst occurs.</para>
			/// </summary>
			public float time
			{
				get
				{
					return this.m_Time;
				}
				set
				{
					this.m_Time = value;
				}
			}

			/// <summary>
			///   <para>Number of particles to be emitted.</para>
			/// </summary>
			public ParticleSystem.MinMaxCurve count
			{
				get
				{
					return this.m_Count;
				}
				set
				{
					this.m_Count = value;
				}
			}

			/// <summary>
			///   <para>Minimum number of particles to be emitted.</para>
			/// </summary>
			public short minCount
			{
				get
				{
					return (short)this.m_Count.constantMin;
				}
				set
				{
					this.m_Count.constantMin = (float)value;
				}
			}

			/// <summary>
			///   <para>Maximum number of particles to be emitted.</para>
			/// </summary>
			public short maxCount
			{
				get
				{
					return (short)this.m_Count.constantMax;
				}
				set
				{
					this.m_Count.constantMax = (float)value;
				}
			}

			/// <summary>
			///   <para>How many times to play the burst. (0 means infinitely).</para>
			/// </summary>
			public int cycleCount
			{
				get
				{
					return this.m_RepeatCount + 1;
				}
				set
				{
					this.m_RepeatCount = value - 1;
				}
			}

			/// <summary>
			///   <para>How often to repeat the burst, in seconds.</para>
			/// </summary>
			public float repeatInterval
			{
				get
				{
					return this.m_RepeatInterval;
				}
				set
				{
					this.m_RepeatInterval = value;
				}
			}

			private float m_Time;

			private ParticleSystem.MinMaxCurve m_Count;

			private int m_RepeatCount;

			private float m_RepeatInterval;
		}

		/// <summary>
		///   <para>Script interface for a Min-Max Curve.</para>
		/// </summary>
		[Serializable]
		public struct MinMaxCurve
		{
			/// <summary>
			///   <para>A single constant value for the entire curve.</para>
			/// </summary>
			/// <param name="constant">Constant value.</param>
			public MinMaxCurve(float constant)
			{
				this.m_Mode = ParticleSystemCurveMode.Constant;
				this.m_CurveMultiplier = 0f;
				this.m_CurveMin = null;
				this.m_CurveMax = null;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = constant;
			}

			/// <summary>
			///   <para>Use one curve when evaluating numbers along this Min-Max curve.</para>
			/// </summary>
			/// <param name="scalar">A multiplier to be applied to the curve.</param>
			/// <param name="curve">A single curve for evaluating against.</param>
			/// <param name="multiplier"></param>
			public MinMaxCurve(float multiplier, AnimationCurve curve)
			{
				this.m_Mode = ParticleSystemCurveMode.Curve;
				this.m_CurveMultiplier = multiplier;
				this.m_CurveMin = null;
				this.m_CurveMax = curve;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = 0f;
			}

			/// <summary>
			///   <para>Randomly select values based on the interval between the minimum and maximum curves.</para>
			/// </summary>
			/// <param name="scalar">A multiplier to be applied to the curves.</param>
			/// <param name="min">The curve describing the minimum values to be evaluated.</param>
			/// <param name="max">The curve describing the maximum values to be evaluated.</param>
			/// <param name="multiplier"></param>
			public MinMaxCurve(float multiplier, AnimationCurve min, AnimationCurve max)
			{
				this.m_Mode = ParticleSystemCurveMode.TwoCurves;
				this.m_CurveMultiplier = multiplier;
				this.m_CurveMin = min;
				this.m_CurveMax = max;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = 0f;
			}

			/// <summary>
			///   <para>Randomly select values based on the interval between the minimum and maximum constants.</para>
			/// </summary>
			/// <param name="min">The constant describing the minimum values to be evaluated.</param>
			/// <param name="max">The constant describing the maximum values to be evaluated.</param>
			public MinMaxCurve(float min, float max)
			{
				this.m_Mode = ParticleSystemCurveMode.TwoConstants;
				this.m_CurveMultiplier = 0f;
				this.m_CurveMin = null;
				this.m_CurveMax = null;
				this.m_ConstantMin = min;
				this.m_ConstantMax = max;
			}

			/// <summary>
			///   <para>Set the mode that the min-max curve will use to evaluate values.</para>
			/// </summary>
			public ParticleSystemCurveMode mode
			{
				get
				{
					return this.m_Mode;
				}
				set
				{
					this.m_Mode = value;
				}
			}

			/// <summary>
			///   <para>Set a multiplier to be applied to the curves.</para>
			/// </summary>
			public float curveMultiplier
			{
				get
				{
					return this.m_CurveMultiplier;
				}
				set
				{
					this.m_CurveMultiplier = value;
				}
			}

			/// <summary>
			///   <para>Set a curve for the upper bound.</para>
			/// </summary>
			public AnimationCurve curveMax
			{
				get
				{
					return this.m_CurveMax;
				}
				set
				{
					this.m_CurveMax = value;
				}
			}

			/// <summary>
			///   <para>Set a curve for the lower bound.</para>
			/// </summary>
			public AnimationCurve curveMin
			{
				get
				{
					return this.m_CurveMin;
				}
				set
				{
					this.m_CurveMin = value;
				}
			}

			/// <summary>
			///   <para>Set a constant for the upper bound.</para>
			/// </summary>
			public float constantMax
			{
				get
				{
					return this.m_ConstantMax;
				}
				set
				{
					this.m_ConstantMax = value;
				}
			}

			/// <summary>
			///   <para>Set a constant for the lower bound.</para>
			/// </summary>
			public float constantMin
			{
				get
				{
					return this.m_ConstantMin;
				}
				set
				{
					this.m_ConstantMin = value;
				}
			}

			/// <summary>
			///   <para>Set the constant value.</para>
			/// </summary>
			public float constant
			{
				get
				{
					return this.m_ConstantMax;
				}
				set
				{
					this.m_ConstantMax = value;
				}
			}

			/// <summary>
			///   <para>Set the curve.</para>
			/// </summary>
			public AnimationCurve curve
			{
				get
				{
					return this.m_CurveMax;
				}
				set
				{
					this.m_CurveMax = value;
				}
			}

			/// <summary>
			///   <para>Manually query the curve to calculate values based on what mode it is in.</para>
			/// </summary>
			/// <param name="time">Normalized time (in the range 0 - 1, where 1 represents 100%) at which to evaluate the curve. This is valid when ParticleSystem.MinMaxCurve.mode is set to ParticleSystemCurveMode.Curve or ParticleSystemCurveMode.TwoCurves.</param>
			/// <param name="lerpFactor">Blend between the 2 curves/constants (Valid when ParticleSystem.MinMaxCurve.mode is set to ParticleSystemCurveMode.TwoConstants or ParticleSystemCurveMode.TwoCurves).</param>
			/// <returns>
			///   <para>Calculated curve/constant value.</para>
			/// </returns>
			public float Evaluate(float time)
			{
				return this.Evaluate(time, 1f);
			}

			/// <summary>
			///   <para>Manually query the curve to calculate values based on what mode it is in.</para>
			/// </summary>
			/// <param name="time">Normalized time (in the range 0 - 1, where 1 represents 100%) at which to evaluate the curve. This is valid when ParticleSystem.MinMaxCurve.mode is set to ParticleSystemCurveMode.Curve or ParticleSystemCurveMode.TwoCurves.</param>
			/// <param name="lerpFactor">Blend between the 2 curves/constants (Valid when ParticleSystem.MinMaxCurve.mode is set to ParticleSystemCurveMode.TwoConstants or ParticleSystemCurveMode.TwoCurves).</param>
			/// <returns>
			///   <para>Calculated curve/constant value.</para>
			/// </returns>
			public float Evaluate(float time, float lerpFactor)
			{
				switch (this.mode)
				{
				case ParticleSystemCurveMode.Constant:
					return this.m_ConstantMax;
				case ParticleSystemCurveMode.TwoCurves:
					return Mathf.Lerp(this.m_CurveMin.Evaluate(time), this.m_CurveMax.Evaluate(time), lerpFactor) * this.m_CurveMultiplier;
				case ParticleSystemCurveMode.TwoConstants:
					return Mathf.Lerp(this.m_ConstantMin, this.m_ConstantMax, lerpFactor);
				}
				return this.m_CurveMax.Evaluate(time) * this.m_CurveMultiplier;
			}

			public static implicit operator ParticleSystem.MinMaxCurve(float constant)
			{
				return new ParticleSystem.MinMaxCurve(constant);
			}

			[SerializeField]
			private ParticleSystemCurveMode m_Mode;

			[SerializeField]
			private float m_CurveMultiplier;

			[SerializeField]
			private AnimationCurve m_CurveMin;

			[SerializeField]
			private AnimationCurve m_CurveMax;

			[SerializeField]
			private float m_ConstantMin;

			[SerializeField]
			private float m_ConstantMax;
		}

		/// <summary>
		///   <para>MinMaxGradient contains two Gradients, and returns a Color based on ParticleSystem.MinMaxGradient.mode. Depending on the mode, the Color returned may be randomized.
		/// Gradients are edited via the ParticleSystem Inspector once a ParticleSystemGradientMode requiring them has been selected. Some modes do not require gradients, only colors.</para>
		/// </summary>
		[Serializable]
		public struct MinMaxGradient
		{
			/// <summary>
			///   <para>A single constant color for the entire gradient.</para>
			/// </summary>
			/// <param name="color">Constant color.</param>
			public MinMaxGradient(Color color)
			{
				this.m_Mode = ParticleSystemGradientMode.Color;
				this.m_GradientMin = null;
				this.m_GradientMax = null;
				this.m_ColorMin = Color.black;
				this.m_ColorMax = color;
			}

			/// <summary>
			///   <para>Use one gradient when evaluating numbers along this Min-Max gradient.</para>
			/// </summary>
			/// <param name="gradient">A single gradient for evaluating against.</param>
			public MinMaxGradient(Gradient gradient)
			{
				this.m_Mode = ParticleSystemGradientMode.Gradient;
				this.m_GradientMin = null;
				this.m_GradientMax = gradient;
				this.m_ColorMin = Color.black;
				this.m_ColorMax = Color.black;
			}

			/// <summary>
			///   <para>Randomly select colors based on the interval between the minimum and maximum constants.</para>
			/// </summary>
			/// <param name="min">The constant color describing the minimum colors to be evaluated.</param>
			/// <param name="max">The constant color describing the maximum colors to be evaluated.</param>
			public MinMaxGradient(Color min, Color max)
			{
				this.m_Mode = ParticleSystemGradientMode.TwoColors;
				this.m_GradientMin = null;
				this.m_GradientMax = null;
				this.m_ColorMin = min;
				this.m_ColorMax = max;
			}

			/// <summary>
			///   <para>Randomly select colors based on the interval between the minimum and maximum gradients.</para>
			/// </summary>
			/// <param name="min">The gradient describing the minimum colors to be evaluated.</param>
			/// <param name="max">The gradient describing the maximum colors to be evaluated.</param>
			public MinMaxGradient(Gradient min, Gradient max)
			{
				this.m_Mode = ParticleSystemGradientMode.TwoGradients;
				this.m_GradientMin = min;
				this.m_GradientMax = max;
				this.m_ColorMin = Color.black;
				this.m_ColorMax = Color.black;
			}

			/// <summary>
			///   <para>Set the mode that the min-max gradient will use to evaluate colors.</para>
			/// </summary>
			public ParticleSystemGradientMode mode
			{
				get
				{
					return this.m_Mode;
				}
				set
				{
					this.m_Mode = value;
				}
			}

			/// <summary>
			///   <para>Set a gradient for the upper bound.</para>
			/// </summary>
			public Gradient gradientMax
			{
				get
				{
					return this.m_GradientMax;
				}
				set
				{
					this.m_GradientMax = value;
				}
			}

			/// <summary>
			///   <para>Set a gradient for the lower bound.</para>
			/// </summary>
			public Gradient gradientMin
			{
				get
				{
					return this.m_GradientMin;
				}
				set
				{
					this.m_GradientMin = value;
				}
			}

			/// <summary>
			///   <para>Set a constant color for the upper bound.</para>
			/// </summary>
			public Color colorMax
			{
				get
				{
					return this.m_ColorMax;
				}
				set
				{
					this.m_ColorMax = value;
				}
			}

			/// <summary>
			///   <para>Set a constant color for the lower bound.</para>
			/// </summary>
			public Color colorMin
			{
				get
				{
					return this.m_ColorMin;
				}
				set
				{
					this.m_ColorMin = value;
				}
			}

			/// <summary>
			///   <para>Set a constant color.</para>
			/// </summary>
			public Color color
			{
				get
				{
					return this.m_ColorMax;
				}
				set
				{
					this.m_ColorMax = value;
				}
			}

			/// <summary>
			///   <para>Set the gradient.</para>
			/// </summary>
			public Gradient gradient
			{
				get
				{
					return this.m_GradientMax;
				}
				set
				{
					this.m_GradientMax = value;
				}
			}

			/// <summary>
			///   <para>Manually query the gradient to calculate colors based on what mode it is in.</para>
			/// </summary>
			/// <param name="time">Normalized time (in the range 0 - 1, where 1 represents 100%) at which to evaluate the gradient. This is valid when ParticleSystem.MinMaxGradient.mode is set to ParticleSystemGradientMode.Gradient or ParticleSystemGradientMode.TwoGradients.</param>
			/// <param name="lerpFactor">Blend between the 2 gradients/colors (Valid when ParticleSystem.MinMaxGradient.mode is set to ParticleSystemGradientMode.TwoColors or ParticleSystemGradientMode.TwoGradients).</param>
			/// <returns>
			///   <para>Calculated gradient/color value.</para>
			/// </returns>
			public Color Evaluate(float time)
			{
				return this.Evaluate(time, 1f);
			}

			/// <summary>
			///   <para>Manually query the gradient to calculate colors based on what mode it is in.</para>
			/// </summary>
			/// <param name="time">Normalized time (in the range 0 - 1, where 1 represents 100%) at which to evaluate the gradient. This is valid when ParticleSystem.MinMaxGradient.mode is set to ParticleSystemGradientMode.Gradient or ParticleSystemGradientMode.TwoGradients.</param>
			/// <param name="lerpFactor">Blend between the 2 gradients/colors (Valid when ParticleSystem.MinMaxGradient.mode is set to ParticleSystemGradientMode.TwoColors or ParticleSystemGradientMode.TwoGradients).</param>
			/// <returns>
			///   <para>Calculated gradient/color value.</para>
			/// </returns>
			public Color Evaluate(float time, float lerpFactor)
			{
				switch (this.m_Mode)
				{
				case ParticleSystemGradientMode.Color:
					return this.m_ColorMax;
				case ParticleSystemGradientMode.TwoColors:
					return Color.Lerp(this.m_ColorMin, this.m_ColorMax, lerpFactor);
				case ParticleSystemGradientMode.TwoGradients:
					return Color.Lerp(this.m_GradientMin.Evaluate(time), this.m_GradientMax.Evaluate(time), lerpFactor);
				case ParticleSystemGradientMode.RandomColor:
					return this.m_GradientMax.Evaluate(lerpFactor);
				}
				return this.m_GradientMax.Evaluate(time);
			}

			public static implicit operator ParticleSystem.MinMaxGradient(Color color)
			{
				return new ParticleSystem.MinMaxGradient(color);
			}

			public static implicit operator ParticleSystem.MinMaxGradient(Gradient gradient)
			{
				return new ParticleSystem.MinMaxGradient(gradient);
			}

			[SerializeField]
			private ParticleSystemGradientMode m_Mode;

			[SerializeField]
			private Gradient m_GradientMin;

			[SerializeField]
			private Gradient m_GradientMax;

			[SerializeField]
			private Color m_ColorMin;

			[SerializeField]
			private Color m_ColorMax;
		}

		/// <summary>
		///   <para>Script interface for particle emission parameters.</para>
		/// </summary>
		public struct EmitParams
		{
			/// <summary>
			///   <para>Override the position of emitted particles.</para>
			/// </summary>
			public Vector3 position
			{
				get
				{
					return this.m_Particle.position;
				}
				set
				{
					this.m_Particle.position = value;
					this.m_PositionSet = true;
				}
			}

			/// <summary>
			///   <para>When overriding the position of particles, setting this flag to true allows you to retain the influence of the shape module.</para>
			/// </summary>
			public bool applyShapeToPosition
			{
				get
				{
					return this.m_ApplyShapeToPosition;
				}
				set
				{
					this.m_ApplyShapeToPosition = value;
				}
			}

			/// <summary>
			///   <para>Override the velocity of emitted particles.</para>
			/// </summary>
			public Vector3 velocity
			{
				get
				{
					return this.m_Particle.velocity;
				}
				set
				{
					this.m_Particle.velocity = value;
					this.m_VelocitySet = true;
				}
			}

			/// <summary>
			///   <para>Override the lifetime of emitted particles.</para>
			/// </summary>
			public float startLifetime
			{
				get
				{
					return this.m_Particle.startLifetime;
				}
				set
				{
					this.m_Particle.startLifetime = value;
					this.m_StartLifetimeSet = true;
				}
			}

			/// <summary>
			///   <para>Override the initial size of emitted particles.</para>
			/// </summary>
			public float startSize
			{
				get
				{
					return this.m_Particle.startSize;
				}
				set
				{
					this.m_Particle.startSize = value;
					this.m_StartSizeSet = true;
				}
			}

			/// <summary>
			///   <para>Override the initial 3D size of emitted particles.</para>
			/// </summary>
			public Vector3 startSize3D
			{
				get
				{
					return this.m_Particle.startSize3D;
				}
				set
				{
					this.m_Particle.startSize3D = value;
					this.m_StartSizeSet = true;
				}
			}

			/// <summary>
			///   <para>Override the axis of rotation of emitted particles.</para>
			/// </summary>
			public Vector3 axisOfRotation
			{
				get
				{
					return this.m_Particle.axisOfRotation;
				}
				set
				{
					this.m_Particle.axisOfRotation = value;
					this.m_AxisOfRotationSet = true;
				}
			}

			/// <summary>
			///   <para>Override the rotation of emitted particles.</para>
			/// </summary>
			public float rotation
			{
				get
				{
					return this.m_Particle.rotation;
				}
				set
				{
					this.m_Particle.rotation = value;
					this.m_RotationSet = true;
				}
			}

			/// <summary>
			///   <para>Override the 3D rotation of emitted particles.</para>
			/// </summary>
			public Vector3 rotation3D
			{
				get
				{
					return this.m_Particle.rotation3D;
				}
				set
				{
					this.m_Particle.rotation3D = value;
					this.m_RotationSet = true;
				}
			}

			/// <summary>
			///   <para>Override the angular velocity of emitted particles.</para>
			/// </summary>
			public float angularVelocity
			{
				get
				{
					return this.m_Particle.angularVelocity;
				}
				set
				{
					this.m_Particle.angularVelocity = value;
					this.m_AngularVelocitySet = true;
				}
			}

			/// <summary>
			///   <para>Override the 3D angular velocity of emitted particles.</para>
			/// </summary>
			public Vector3 angularVelocity3D
			{
				get
				{
					return this.m_Particle.angularVelocity3D;
				}
				set
				{
					this.m_Particle.angularVelocity3D = value;
					this.m_AngularVelocitySet = true;
				}
			}

			/// <summary>
			///   <para>Override the initial color of emitted particles.</para>
			/// </summary>
			public Color32 startColor
			{
				get
				{
					return this.m_Particle.startColor;
				}
				set
				{
					this.m_Particle.startColor = value;
					this.m_StartColorSet = true;
				}
			}

			/// <summary>
			///   <para>Override the random seed of emitted particles.</para>
			/// </summary>
			public uint randomSeed
			{
				get
				{
					return this.m_Particle.randomSeed;
				}
				set
				{
					this.m_Particle.randomSeed = value;
					this.m_RandomSeedSet = true;
				}
			}

			/// <summary>
			///   <para>Revert the position back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetPosition()
			{
				this.m_PositionSet = false;
			}

			/// <summary>
			///   <para>Revert the velocity back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetVelocity()
			{
				this.m_VelocitySet = false;
			}

			/// <summary>
			///   <para>Revert the axis of rotation back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetAxisOfRotation()
			{
				this.m_AxisOfRotationSet = false;
			}

			/// <summary>
			///   <para>Reverts rotation and rotation3D back to the values specified in the inspector.</para>
			/// </summary>
			public void ResetRotation()
			{
				this.m_RotationSet = false;
			}

			/// <summary>
			///   <para>Reverts angularVelocity and angularVelocity3D back to the values specified in the inspector.</para>
			/// </summary>
			public void ResetAngularVelocity()
			{
				this.m_AngularVelocitySet = false;
			}

			/// <summary>
			///   <para>Revert the initial size back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetStartSize()
			{
				this.m_StartSizeSet = false;
			}

			/// <summary>
			///   <para>Revert the initial color back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetStartColor()
			{
				this.m_StartColorSet = false;
			}

			/// <summary>
			///   <para>Revert the random seed back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetRandomSeed()
			{
				this.m_RandomSeedSet = false;
			}

			/// <summary>
			///   <para>Revert the lifetime back to the value specified in the inspector.</para>
			/// </summary>
			public void ResetStartLifetime()
			{
				this.m_StartLifetimeSet = false;
			}

			private ParticleSystem.Particle m_Particle;

			private bool m_PositionSet;

			private bool m_VelocitySet;

			private bool m_AxisOfRotationSet;

			private bool m_RotationSet;

			private bool m_AngularVelocitySet;

			private bool m_StartSizeSet;

			private bool m_StartColorSet;

			private bool m_RandomSeedSet;

			private bool m_StartLifetimeSet;

			private bool m_ApplyShapeToPosition;
		}
	}
}
