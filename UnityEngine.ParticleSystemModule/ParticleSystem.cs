using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/ParticleSystem/ScriptBindings/ParticleSystemModulesScriptBindings.h")]
	[NativeHeader("Modules/ParticleSystem/ScriptBindings/ParticleSystemScriptBindings.h")]
	[NativeHeader("Modules/ParticleSystem/ParticleSystem.h")]
	[NativeHeader("ParticleSystemScriptingClasses.h")]
	[UsedByNativeCode]
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Modules/ParticleSystem/ParticleSystemGeometryJob.h")]
	[NativeHeader("ParticleSystemScriptingClasses.h")]
	[NativeHeader("Modules/ParticleSystem/ParticleSystem.h")]
	[NativeHeader("Modules/ParticleSystem/ScriptBindings/ParticleSystemScriptBindings.h")]
	public sealed class ParticleSystem : Component
	{
		[Obsolete("SetTrails is deprecated. Use SetParticlesAndTrails() instead. Avoid SetTrails when ParticleSystem.trails.dieWithParticles is false.", false)]
		[FreeFunction(Name = "ParticleSystemScriptBindings::SetTrailData", HasExplicitThis = true)]
		public void SetTrails(ParticleSystem.Trails trailData)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.SetTrails_Injected(intPtr, ref trailData);
		}

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
			this.EmitOld_Internal(ref particle);
		}

		[Obsolete("Emit with a single particle structure is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties", false)]
		public void Emit(ParticleSystem.Particle particle)
		{
			this.EmitOld_Internal(ref particle);
		}

		[Obsolete("startDelay property is deprecated. Use main.startDelay or main.startDelayMultiplier instead.", false)]
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

		[Obsolete("loop property is deprecated. Use main.loop instead.", false)]
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

		[Obsolete("playOnAwake property is deprecated. Use main.playOnAwake instead.", false)]
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

		[Obsolete("duration property is deprecated. Use main.duration instead.", false)]
		public float duration
		{
			get
			{
				return this.main.duration;
			}
		}

		[Obsolete("playbackSpeed property is deprecated. Use main.simulationSpeed instead.", false)]
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

		[Obsolete("enableEmission property is deprecated. Use emission.enabled instead.", false)]
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

		[Obsolete("emissionRate property is deprecated. Use emission.rateOverTime, emission.rateOverDistance, emission.rateOverTimeMultiplier or emission.rateOverDistanceMultiplier instead.", false)]
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

		[Obsolete("startSpeed property is deprecated. Use main.startSpeed or main.startSpeedMultiplier instead.", false)]
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

		[Obsolete("startSize property is deprecated. Use main.startSize or main.startSizeMultiplier instead.", false)]
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

		[Obsolete("startColor property is deprecated. Use main.startColor instead.", false)]
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

		[Obsolete("startRotation property is deprecated. Use main.startRotation or main.startRotationMultiplier instead.", false)]
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

		[Obsolete("startRotation3D property is deprecated. Use main.startRotationX, main.startRotationY and main.startRotationZ instead. (Or main.startRotationXMultiplier, main.startRotationYMultiplier and main.startRotationZMultiplier).", false)]
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

		[Obsolete("startLifetime property is deprecated. Use main.startLifetime or main.startLifetimeMultiplier instead.", false)]
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

		[Obsolete("gravityModifier property is deprecated. Use main.gravityModifier or main.gravityModifierMultiplier instead.", false)]
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

		[Obsolete("maxParticles property is deprecated. Use main.maxParticles instead.", false)]
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

		[Obsolete("simulationSpace property is deprecated. Use main.simulationSpace instead.", false)]
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

		[Obsolete("scalingMode property is deprecated. Use main.scalingMode instead.", false)]
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

		[Obsolete("automaticCullingEnabled property is deprecated. Use proceduralSimulationSupported instead (UnityUpgradable) -> proceduralSimulationSupported", true)]
		public bool automaticCullingEnabled
		{
			get
			{
				return this.proceduralSimulationSupported;
			}
		}

		public bool isPlaying
		{
			[NativeName("SyncJobs(false)->IsPlaying")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystem.get_isPlaying_Injected(intPtr);
			}
		}

		public bool isEmitting
		{
			[NativeName("SyncJobs(false)->IsEmitting")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystem.get_isEmitting_Injected(intPtr);
			}
		}

		public bool isStopped
		{
			[NativeName("SyncJobs(false)->IsStopped")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystem.get_isStopped_Injected(intPtr);
			}
		}

		public bool isPaused
		{
			[NativeName("SyncJobs(false)->IsPaused")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystem.get_isPaused_Injected(intPtr);
			}
		}

		public int particleCount
		{
			[NativeName("SyncJobs(false)->GetParticleCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystem.get_particleCount_Injected(intPtr);
			}
		}

		public float time
		{
			[NativeName("SyncJobs(false)->GetSecPosition")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystem.get_time_Injected(intPtr);
			}
			[NativeName("SyncJobs(false)->SetSecPosition")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystem.set_time_Injected(intPtr, value);
			}
		}

		public float totalTime
		{
			[NativeName("SyncJobs(false)->GetTotalSecPosition")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystem.get_totalTime_Injected(intPtr);
			}
		}

		public uint randomSeed
		{
			[NativeName("GetRandomSeed")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystem.get_randomSeed_Injected(intPtr);
			}
			[NativeName("SyncJobs(false)->SetRandomSeed")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystem.set_randomSeed_Injected(intPtr, value);
			}
		}

		public bool useAutoRandomSeed
		{
			[NativeName("GetAutoRandomSeed")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystem.get_useAutoRandomSeed_Injected(intPtr);
			}
			[NativeName("SyncJobs(false)->SetAutoRandomSeed")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystem.set_useAutoRandomSeed_Injected(intPtr, value);
			}
		}

		public bool proceduralSimulationSupported
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystem.get_proceduralSimulationSupported_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::GetParticleCurrentSize", HasExplicitThis = true)]
		internal float GetParticleCurrentSize(ref ParticleSystem.Particle particle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ParticleSystem.GetParticleCurrentSize_Injected(intPtr, ref particle);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::GetParticleCurrentSize3D", HasExplicitThis = true)]
		internal Vector3 GetParticleCurrentSize3D(ref ParticleSystem.Particle particle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			ParticleSystem.GetParticleCurrentSize3D_Injected(intPtr, ref particle, out vector);
			return vector;
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::GetParticleCurrentColor", HasExplicitThis = true)]
		internal Color32 GetParticleCurrentColor(ref ParticleSystem.Particle particle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Color32 color;
			ParticleSystem.GetParticleCurrentColor_Injected(intPtr, ref particle, out color);
			return color;
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::GetParticleMeshIndex", HasExplicitThis = true)]
		internal int GetParticleMeshIndex(ref ParticleSystem.Particle particle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ParticleSystem.GetParticleMeshIndex_Injected(intPtr, ref particle);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::SetParticles", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void SetParticles([Out] ParticleSystem.Particle[] particles, int size, int offset)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (particles != null)
				{
					fixed (ParticleSystem.Particle[] array = particles)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				ParticleSystem.SetParticles_Injected(intPtr, out blittableArrayWrapper, size, offset);
			}
			finally
			{
				ParticleSystem.Particle[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<ParticleSystem.Particle>(ref array);
			}
		}

		public void SetParticles([Out] ParticleSystem.Particle[] particles, int size)
		{
			this.SetParticles(particles, size, 0);
		}

		public void SetParticles([Out] ParticleSystem.Particle[] particles)
		{
			this.SetParticles(particles, -1);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::SetParticlesWithNativeArray", HasExplicitThis = true, ThrowsException = true)]
		private void SetParticlesWithNativeArray(IntPtr particles, int particlesLength, int size, int offset)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.SetParticlesWithNativeArray_Injected(intPtr, particles, particlesLength, size, offset);
		}

		public void SetParticles([Out] NativeArray<ParticleSystem.Particle> particles, int size, int offset)
		{
			this.SetParticlesWithNativeArray((IntPtr)particles.GetUnsafeReadOnlyPtr<ParticleSystem.Particle>(), particles.Length, size, offset);
		}

		public void SetParticles([Out] NativeArray<ParticleSystem.Particle> particles, int size)
		{
			this.SetParticles(particles, size, 0);
		}

		public void SetParticles([Out] NativeArray<ParticleSystem.Particle> particles)
		{
			this.SetParticles(particles, -1);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::GetParticles", HasExplicitThis = true, ThrowsException = true)]
		public unsafe int GetParticles([NotNull] [Out] ParticleSystem.Particle[] particles, int size, int offset)
		{
			if (particles == null)
			{
				ThrowHelper.ThrowArgumentNullException(particles, "particles");
			}
			int particles_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (ParticleSystem.Particle[] array = particles)
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					particles_Injected = ParticleSystem.GetParticles_Injected(intPtr, out blittableArrayWrapper, size, offset);
				}
			}
			finally
			{
				ParticleSystem.Particle[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<ParticleSystem.Particle>(ref array);
			}
			return particles_Injected;
		}

		public int GetParticles([Out] ParticleSystem.Particle[] particles, int size)
		{
			return this.GetParticles(particles, size, 0);
		}

		public int GetParticles([Out] ParticleSystem.Particle[] particles)
		{
			return this.GetParticles(particles, -1);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::GetParticlesWithNativeArray", HasExplicitThis = true, ThrowsException = true)]
		private int GetParticlesWithNativeArray(IntPtr particles, int particlesLength, int size, int offset)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ParticleSystem.GetParticlesWithNativeArray_Injected(intPtr, particles, particlesLength, size, offset);
		}

		public int GetParticles([Out] NativeArray<ParticleSystem.Particle> particles, int size, int offset)
		{
			return this.GetParticlesWithNativeArray((IntPtr)particles.GetUnsafePtr<ParticleSystem.Particle>(), particles.Length, size, offset);
		}

		public int GetParticles([Out] NativeArray<ParticleSystem.Particle> particles, int size)
		{
			return this.GetParticles(particles, size, 0);
		}

		public int GetParticles([Out] NativeArray<ParticleSystem.Particle> particles)
		{
			return this.GetParticles(particles, -1);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::SetCustomParticleData", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void SetCustomParticleData([NotNull] List<Vector4> customData, ParticleSystemCustomData streamIndex)
		{
			if (customData == null)
			{
				ThrowHelper.ThrowArgumentNullException(customData, "customData");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (Vector4[] array = NoAllocHelpers.ExtractArrayFromList<Vector4>(customData))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, customData.Count);
					ParticleSystem.SetCustomParticleData_Injected(intPtr, ref blittableListWrapper, streamIndex);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Vector4>(customData);
			}
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::GetCustomParticleData", HasExplicitThis = true, ThrowsException = true)]
		public unsafe int GetCustomParticleData([NotNull] List<Vector4> customData, ParticleSystemCustomData streamIndex)
		{
			if (customData == null)
			{
				ThrowHelper.ThrowArgumentNullException(customData, "customData");
			}
			int customParticleData_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (Vector4[] array = NoAllocHelpers.ExtractArrayFromList<Vector4>(customData))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, customData.Count);
					customParticleData_Injected = ParticleSystem.GetCustomParticleData_Injected(intPtr, ref blittableListWrapper, streamIndex);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Vector4>(customData);
			}
			return customParticleData_Injected;
		}

		public ParticleSystem.PlaybackState GetPlaybackState()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.PlaybackState playbackState;
			ParticleSystem.GetPlaybackState_Injected(intPtr, out playbackState);
			return playbackState;
		}

		public void SetPlaybackState(ParticleSystem.PlaybackState playbackState)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.SetPlaybackState_Injected(intPtr, ref playbackState);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::GetTrailData", HasExplicitThis = true)]
		private void GetTrailDataInternal(ref ParticleSystem.Trails trailData)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.GetTrailDataInternal_Injected(intPtr, ref trailData);
		}

		public ParticleSystem.Trails GetTrails()
		{
			ParticleSystem.Trails trails = default(ParticleSystem.Trails);
			trails.Allocate();
			this.GetTrailDataInternal(ref trails);
			return trails;
		}

		public int GetTrails(ref ParticleSystem.Trails trailData)
		{
			trailData.Allocate();
			this.GetTrailDataInternal(ref trailData);
			return trailData.positions.Count;
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::SetParticlesAndTrailData", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void SetParticlesAndTrails([Out] ParticleSystem.Particle[] particles, ParticleSystem.Trails trailData, int size, int offset)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (particles != null)
				{
					fixed (ParticleSystem.Particle[] array = particles)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				ParticleSystem.SetParticlesAndTrails_Injected(intPtr, out blittableArrayWrapper, ref trailData, size, offset);
			}
			finally
			{
				ParticleSystem.Particle[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<ParticleSystem.Particle>(ref array);
			}
		}

		public void SetParticlesAndTrails([Out] ParticleSystem.Particle[] particles, ParticleSystem.Trails trailData, int size)
		{
			this.SetParticlesAndTrails(particles, trailData, size, 0);
		}

		public void SetParticlesAndTrails([Out] ParticleSystem.Particle[] particles, ParticleSystem.Trails trailData)
		{
			this.SetParticlesAndTrails(particles, trailData, -1);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::SetParticlesAndTrailDataWithNativeArray", HasExplicitThis = true, ThrowsException = true)]
		private void SetParticlesAndTrailsWithNativeArray(IntPtr particles, ParticleSystem.Trails trailData, int particlesLength, int size, int offset)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.SetParticlesAndTrailsWithNativeArray_Injected(intPtr, particles, ref trailData, particlesLength, size, offset);
		}

		public void SetParticlesAndTrails([Out] NativeArray<ParticleSystem.Particle> particles, ParticleSystem.Trails trailData, int size, int offset)
		{
			this.SetParticlesAndTrailsWithNativeArray((IntPtr)particles.GetUnsafeReadOnlyPtr<ParticleSystem.Particle>(), trailData, particles.Length, size, offset);
		}

		public void SetParticlesAndTrails([Out] NativeArray<ParticleSystem.Particle> particles, ParticleSystem.Trails trailData, int size)
		{
			this.SetParticlesAndTrails(particles, trailData, size, 0);
		}

		public void SetParticlesAndTrails([Out] NativeArray<ParticleSystem.Particle> particles, ParticleSystem.Trails trailData)
		{
			this.SetParticlesAndTrails(particles, trailData, -1);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::Simulate", HasExplicitThis = true)]
		public void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart, [DefaultValue("true")] bool fixedTimeStep)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.Simulate_Injected(intPtr, t, withChildren, restart, fixedTimeStep);
		}

		public void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart)
		{
			this.Simulate(t, withChildren, restart, true);
		}

		public void Simulate(float t, [DefaultValue("true")] bool withChildren)
		{
			this.Simulate(t, withChildren, true);
		}

		public void Simulate(float t)
		{
			this.Simulate(t, true);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::Play", HasExplicitThis = true)]
		public void Play([DefaultValue("true")] bool withChildren)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.Play_Injected(intPtr, withChildren);
		}

		public void Play()
		{
			this.Play(true);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::Pause", HasExplicitThis = true)]
		public void Pause([DefaultValue("true")] bool withChildren)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.Pause_Injected(intPtr, withChildren);
		}

		public void Pause()
		{
			this.Pause(true);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::Stop", HasExplicitThis = true)]
		public void Stop([DefaultValue("true")] bool withChildren, [DefaultValue("ParticleSystemStopBehavior.StopEmitting")] ParticleSystemStopBehavior stopBehavior)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.Stop_Injected(intPtr, withChildren, stopBehavior);
		}

		public void Stop([DefaultValue("true")] bool withChildren)
		{
			this.Stop(withChildren, ParticleSystemStopBehavior.StopEmitting);
		}

		public void Stop()
		{
			this.Stop(true);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::Clear", HasExplicitThis = true)]
		public void Clear([DefaultValue("true")] bool withChildren)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.Clear_Injected(intPtr, withChildren);
		}

		public void Clear()
		{
			this.Clear(true);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::IsAlive", HasExplicitThis = true)]
		public bool IsAlive([DefaultValue("true")] bool withChildren)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ParticleSystem.IsAlive_Injected(intPtr, withChildren);
		}

		public bool IsAlive()
		{
			return this.IsAlive(true);
		}

		[RequiredByNativeCode]
		public void Emit(int count)
		{
			this.Emit_Internal(count);
		}

		[NativeName("SyncJobs()->Emit")]
		private void Emit_Internal(int count)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.Emit_Internal_Injected(intPtr, count);
		}

		[NativeName("SyncJobs()->EmitParticlesExternal")]
		public void Emit(ParticleSystem.EmitParams emitParams, int count)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.Emit_Injected(intPtr, ref emitParams, count);
		}

		[NativeName("SyncJobs()->EmitParticleExternal")]
		private void EmitOld_Internal(ref ParticleSystem.Particle particle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.EmitOld_Internal_Injected(intPtr, ref particle);
		}

		public void TriggerSubEmitter(int subEmitterIndex)
		{
			this.TriggerSubEmitterForAllParticles(subEmitterIndex);
		}

		public void TriggerSubEmitter(int subEmitterIndex, ref ParticleSystem.Particle particle)
		{
			this.TriggerSubEmitterForParticle(subEmitterIndex, particle);
		}

		public void TriggerSubEmitter(int subEmitterIndex, List<ParticleSystem.Particle> particles)
		{
			bool flag = particles == null;
			if (flag)
			{
				this.TriggerSubEmitterForAllParticles(subEmitterIndex);
			}
			else
			{
				this.TriggerSubEmitterForParticles(subEmitterIndex, particles);
			}
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::TriggerSubEmitterForParticle", HasExplicitThis = true)]
		internal void TriggerSubEmitterForParticle(int subEmitterIndex, ParticleSystem.Particle particle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.TriggerSubEmitterForParticle_Injected(intPtr, subEmitterIndex, ref particle);
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::TriggerSubEmitterForParticles", HasExplicitThis = true)]
		private unsafe void TriggerSubEmitterForParticles(int subEmitterIndex, List<ParticleSystem.Particle> particles)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (particles != null)
				{
					fixed (ParticleSystem.Particle[] array = NoAllocHelpers.ExtractArrayFromList<ParticleSystem.Particle>(particles))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, particles.Count);
					}
				}
				ParticleSystem.TriggerSubEmitterForParticles_Injected(intPtr, subEmitterIndex, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<ParticleSystem.Particle>(particles);
			}
		}

		[FreeFunction(Name = "ParticleSystemScriptBindings::TriggerSubEmitterForAllParticles", HasExplicitThis = true)]
		private void TriggerSubEmitterForAllParticles(int subEmitterIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.TriggerSubEmitterForAllParticles_Injected(intPtr, subEmitterIndex);
		}

		[FreeFunction(Name = "ParticleSystemGeometryJob::ResetPreMappedBufferMemory")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResetPreMappedBufferMemory();

		[FreeFunction(Name = "ParticleSystemGeometryJob::SetMaximumPreMappedBufferCounts")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetMaximumPreMappedBufferCounts(int vertexBuffersCount, int indexBuffersCount);

		[NativeName("SetUsesAxisOfRotation")]
		public void AllocateAxisOfRotationAttribute()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.AllocateAxisOfRotationAttribute_Injected(intPtr);
		}

		[NativeName("SetUsesMeshIndex")]
		public void AllocateMeshIndexAttribute()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.AllocateMeshIndexAttribute_Injected(intPtr);
		}

		[NativeName("SetUsesCustomData")]
		public void AllocateCustomDataAttribute(ParticleSystemCustomData stream)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.AllocateCustomDataAttribute_Injected(intPtr, stream);
		}

		public bool has3DParticleRotations
		{
			[NativeName("Has3DParticleRotations")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystem.get_has3DParticleRotations_Injected(intPtr);
			}
		}

		public bool hasNonUniformParticleSizes
		{
			[NativeName("HasNonUniformParticleSizes")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystem.get_hasNonUniformParticleSizes_Injected(intPtr);
			}
		}

		internal unsafe void* GetManagedJobData()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ParticleSystem.GetManagedJobData_Injected(intPtr);
		}

		internal JobHandle GetManagedJobHandle()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			JobHandle jobHandle;
			ParticleSystem.GetManagedJobHandle_Injected(intPtr, out jobHandle);
			return jobHandle;
		}

		internal void SetManagedJobHandle(JobHandle handle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystem>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystem.SetManagedJobHandle_Injected(intPtr, ref handle);
		}

		[FreeFunction("ScheduleManagedJob", ThrowsException = true)]
		internal unsafe static JobHandle ScheduleManagedJob(ref JobsUtility.JobScheduleParameters parameters, void* additionalData)
		{
			JobHandle jobHandle;
			ParticleSystem.ScheduleManagedJob_Injected(ref parameters, additionalData, out jobHandle);
			return jobHandle;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void CopyManagedJobData(void* systemPtr, out NativeParticleData particleData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool UserJobCanBeScheduled();

		public ParticleSystem.MainModule main
		{
			get
			{
				return new ParticleSystem.MainModule(this);
			}
		}

		public ParticleSystem.EmissionModule emission
		{
			get
			{
				return new ParticleSystem.EmissionModule(this);
			}
		}

		public ParticleSystem.ShapeModule shape
		{
			get
			{
				return new ParticleSystem.ShapeModule(this);
			}
		}

		public ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime
		{
			get
			{
				return new ParticleSystem.VelocityOverLifetimeModule(this);
			}
		}

		public ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime
		{
			get
			{
				return new ParticleSystem.LimitVelocityOverLifetimeModule(this);
			}
		}

		public ParticleSystem.InheritVelocityModule inheritVelocity
		{
			get
			{
				return new ParticleSystem.InheritVelocityModule(this);
			}
		}

		public ParticleSystem.LifetimeByEmitterSpeedModule lifetimeByEmitterSpeed
		{
			get
			{
				return new ParticleSystem.LifetimeByEmitterSpeedModule(this);
			}
		}

		public ParticleSystem.ForceOverLifetimeModule forceOverLifetime
		{
			get
			{
				return new ParticleSystem.ForceOverLifetimeModule(this);
			}
		}

		public ParticleSystem.ColorOverLifetimeModule colorOverLifetime
		{
			get
			{
				return new ParticleSystem.ColorOverLifetimeModule(this);
			}
		}

		public ParticleSystem.ColorBySpeedModule colorBySpeed
		{
			get
			{
				return new ParticleSystem.ColorBySpeedModule(this);
			}
		}

		public ParticleSystem.SizeOverLifetimeModule sizeOverLifetime
		{
			get
			{
				return new ParticleSystem.SizeOverLifetimeModule(this);
			}
		}

		public ParticleSystem.SizeBySpeedModule sizeBySpeed
		{
			get
			{
				return new ParticleSystem.SizeBySpeedModule(this);
			}
		}

		public ParticleSystem.RotationOverLifetimeModule rotationOverLifetime
		{
			get
			{
				return new ParticleSystem.RotationOverLifetimeModule(this);
			}
		}

		public ParticleSystem.RotationBySpeedModule rotationBySpeed
		{
			get
			{
				return new ParticleSystem.RotationBySpeedModule(this);
			}
		}

		public ParticleSystem.ExternalForcesModule externalForces
		{
			get
			{
				return new ParticleSystem.ExternalForcesModule(this);
			}
		}

		public ParticleSystem.NoiseModule noise
		{
			get
			{
				return new ParticleSystem.NoiseModule(this);
			}
		}

		public ParticleSystem.CollisionModule collision
		{
			get
			{
				return new ParticleSystem.CollisionModule(this);
			}
		}

		public ParticleSystem.TriggerModule trigger
		{
			get
			{
				return new ParticleSystem.TriggerModule(this);
			}
		}

		public ParticleSystem.SubEmittersModule subEmitters
		{
			get
			{
				return new ParticleSystem.SubEmittersModule(this);
			}
		}

		public ParticleSystem.TextureSheetAnimationModule textureSheetAnimation
		{
			get
			{
				return new ParticleSystem.TextureSheetAnimationModule(this);
			}
		}

		public ParticleSystem.LightsModule lights
		{
			get
			{
				return new ParticleSystem.LightsModule(this);
			}
		}

		public ParticleSystem.TrailModule trails
		{
			get
			{
				return new ParticleSystem.TrailModule(this);
			}
		}

		public ParticleSystem.CustomDataModule customData
		{
			get
			{
				return new ParticleSystem.CustomDataModule(this);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTrails_Injected(IntPtr _unity_self, [In] ref ParticleSystem.Trails trailData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isPlaying_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isEmitting_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isStopped_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isPaused_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_particleCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_time_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_time_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_totalTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_randomSeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_randomSeed_Injected(IntPtr _unity_self, uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useAutoRandomSeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useAutoRandomSeed_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_proceduralSimulationSupported_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetParticleCurrentSize_Injected(IntPtr _unity_self, ref ParticleSystem.Particle particle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetParticleCurrentSize3D_Injected(IntPtr _unity_self, ref ParticleSystem.Particle particle, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetParticleCurrentColor_Injected(IntPtr _unity_self, ref ParticleSystem.Particle particle, out Color32 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetParticleMeshIndex_Injected(IntPtr _unity_self, ref ParticleSystem.Particle particle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetParticles_Injected(IntPtr _unity_self, out BlittableArrayWrapper particles, int size, int offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetParticlesWithNativeArray_Injected(IntPtr _unity_self, IntPtr particles, int particlesLength, int size, int offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetParticles_Injected(IntPtr _unity_self, out BlittableArrayWrapper particles, int size, int offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetParticlesWithNativeArray_Injected(IntPtr _unity_self, IntPtr particles, int particlesLength, int size, int offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetCustomParticleData_Injected(IntPtr _unity_self, ref BlittableListWrapper customData, ParticleSystemCustomData streamIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetCustomParticleData_Injected(IntPtr _unity_self, ref BlittableListWrapper customData, ParticleSystemCustomData streamIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPlaybackState_Injected(IntPtr _unity_self, out ParticleSystem.PlaybackState ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPlaybackState_Injected(IntPtr _unity_self, [In] ref ParticleSystem.PlaybackState playbackState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTrailDataInternal_Injected(IntPtr _unity_self, ref ParticleSystem.Trails trailData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetParticlesAndTrails_Injected(IntPtr _unity_self, out BlittableArrayWrapper particles, [In] ref ParticleSystem.Trails trailData, int size, int offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetParticlesAndTrailsWithNativeArray_Injected(IntPtr _unity_self, IntPtr particles, [In] ref ParticleSystem.Trails trailData, int particlesLength, int size, int offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Simulate_Injected(IntPtr _unity_self, float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart, [DefaultValue("true")] bool fixedTimeStep);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Play_Injected(IntPtr _unity_self, [DefaultValue("true")] bool withChildren);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Pause_Injected(IntPtr _unity_self, [DefaultValue("true")] bool withChildren);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop_Injected(IntPtr _unity_self, [DefaultValue("true")] bool withChildren, [DefaultValue("ParticleSystemStopBehavior.StopEmitting")] ParticleSystemStopBehavior stopBehavior);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Clear_Injected(IntPtr _unity_self, [DefaultValue("true")] bool withChildren);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsAlive_Injected(IntPtr _unity_self, [DefaultValue("true")] bool withChildren);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Emit_Internal_Injected(IntPtr _unity_self, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Emit_Injected(IntPtr _unity_self, [In] ref ParticleSystem.EmitParams emitParams, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EmitOld_Internal_Injected(IntPtr _unity_self, ref ParticleSystem.Particle particle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TriggerSubEmitterForParticle_Injected(IntPtr _unity_self, int subEmitterIndex, [In] ref ParticleSystem.Particle particle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TriggerSubEmitterForParticles_Injected(IntPtr _unity_self, int subEmitterIndex, ref BlittableListWrapper particles);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TriggerSubEmitterForAllParticles_Injected(IntPtr _unity_self, int subEmitterIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AllocateAxisOfRotationAttribute_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AllocateMeshIndexAttribute_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AllocateCustomDataAttribute_Injected(IntPtr _unity_self, ParticleSystemCustomData stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_has3DParticleRotations_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasNonUniformParticleSizes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* GetManagedJobData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetManagedJobHandle_Injected(IntPtr _unity_self, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetManagedJobHandle_Injected(IntPtr _unity_self, [In] ref JobHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ScheduleManagedJob_Injected(ref JobsUtility.JobScheduleParameters parameters, void* additionalData, out JobHandle ret);

		public struct MainModule
		{
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

			internal MainModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public Vector3 emitterVelocity
			{
				get
				{
					Vector3 vector;
					ParticleSystem.MainModule.get_emitterVelocity_Injected(ref this, out vector);
					return vector;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_emitterVelocity_Injected(ref this, ref value);
				}
			}

			public extern float duration
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool loop
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool prewarm
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve startDelay
			{
				get
				{
					return this.startDelayBlittable;
				}
				set
				{
					this.startDelayBlittable = value;
				}
			}

			[NativeName("StartDelay")]
			private ParticleSystem.MinMaxCurveBlittable startDelayBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.MainModule.get_startDelayBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_startDelayBlittable_Injected(ref this, ref value);
				}
			}

			public extern float startDelayMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve startLifetime
			{
				get
				{
					return this.startLifetimeBlittable;
				}
				set
				{
					this.startLifetimeBlittable = value;
				}
			}

			[NativeName("StartLifetime")]
			private ParticleSystem.MinMaxCurveBlittable startLifetimeBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.MainModule.get_startLifetimeBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_startLifetimeBlittable_Injected(ref this, ref value);
				}
			}

			public extern float startLifetimeMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve startSpeed
			{
				get
				{
					return this.startSpeedBlittable;
				}
				set
				{
					this.startSpeedBlittable = value;
				}
			}

			[NativeName("StartSpeed")]
			private ParticleSystem.MinMaxCurveBlittable startSpeedBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.MainModule.get_startSpeedBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_startSpeedBlittable_Injected(ref this, ref value);
				}
			}

			public extern float startSpeedMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool startSize3D
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve startSize
			{
				get
				{
					return this.startSizeBlittable;
				}
				set
				{
					this.startSizeBlittable = value;
				}
			}

			[NativeName("StartSizeX")]
			private ParticleSystem.MinMaxCurveBlittable startSizeBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.MainModule.get_startSizeBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_startSizeBlittable_Injected(ref this, ref value);
				}
			}

			[NativeName("StartSizeXMultiplier")]
			public extern float startSizeMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve startSizeX
			{
				get
				{
					return this.startSizeXBlittable;
				}
				set
				{
					this.startSizeXBlittable = value;
				}
			}

			[NativeName("StartSizeX")]
			private ParticleSystem.MinMaxCurveBlittable startSizeXBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.MainModule.get_startSizeXBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_startSizeXBlittable_Injected(ref this, ref value);
				}
			}

			public extern float startSizeXMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve startSizeY
			{
				get
				{
					return this.startSizeYBlittable;
				}
				set
				{
					this.startSizeYBlittable = value;
				}
			}

			[NativeName("StartSizeY")]
			private ParticleSystem.MinMaxCurveBlittable startSizeYBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.MainModule.get_startSizeYBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_startSizeYBlittable_Injected(ref this, ref value);
				}
			}

			public extern float startSizeYMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve startSizeZ
			{
				get
				{
					return this.startSizeZBlittable;
				}
				set
				{
					this.startSizeZBlittable = value;
				}
			}

			[NativeName("StartSizeZ")]
			private ParticleSystem.MinMaxCurveBlittable startSizeZBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.MainModule.get_startSizeZBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_startSizeZBlittable_Injected(ref this, ref value);
				}
			}

			public extern float startSizeZMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool startRotation3D
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve startRotation
			{
				get
				{
					return this.startRotationBlittable;
				}
				set
				{
					this.startRotationBlittable = value;
				}
			}

			[NativeName("StartRotationZ")]
			private ParticleSystem.MinMaxCurveBlittable startRotationBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.MainModule.get_startRotationBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_startRotationBlittable_Injected(ref this, ref value);
				}
			}

			[NativeName("StartRotationZMultiplier")]
			public extern float startRotationMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve startRotationX
			{
				get
				{
					return this.startRotationXBlittable;
				}
				set
				{
					this.startRotationXBlittable = value;
				}
			}

			[NativeName("StartRotationX")]
			private ParticleSystem.MinMaxCurveBlittable startRotationXBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.MainModule.get_startRotationXBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_startRotationXBlittable_Injected(ref this, ref value);
				}
			}

			public extern float startRotationXMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve startRotationY
			{
				get
				{
					return this.startRotationYBlittable;
				}
				set
				{
					this.startRotationYBlittable = value;
				}
			}

			[NativeName("StartRotationY")]
			private ParticleSystem.MinMaxCurveBlittable startRotationYBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.MainModule.get_startRotationYBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_startRotationYBlittable_Injected(ref this, ref value);
				}
			}

			public extern float startRotationYMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve startRotationZ
			{
				get
				{
					return this.startRotationZBlittable;
				}
				set
				{
					this.startRotationZBlittable = value;
				}
			}

			[NativeName("StartRotationZ")]
			private ParticleSystem.MinMaxCurveBlittable startRotationZBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.MainModule.get_startRotationZBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_startRotationZBlittable_Injected(ref this, ref value);
				}
			}

			public extern float startRotationZMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float flipRotation
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxGradient startColor
			{
				get
				{
					return this.startColorBlittable;
				}
				set
				{
					this.startColorBlittable = value;
				}
			}

			[NativeName("StartColor")]
			private ParticleSystem.MinMaxGradientBlittable startColorBlittable
			{
				get
				{
					ParticleSystem.MinMaxGradientBlittable minMaxGradientBlittable;
					ParticleSystem.MainModule.get_startColorBlittable_Injected(ref this, out minMaxGradientBlittable);
					return minMaxGradientBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_startColorBlittable_Injected(ref this, ref value);
				}
			}

			public extern ParticleSystemGravitySource gravitySource
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve gravityModifier
			{
				get
				{
					return this.gravityModifierBlittable;
				}
				set
				{
					this.gravityModifierBlittable = value;
				}
			}

			[NativeName("GravityModifier")]
			private ParticleSystem.MinMaxCurveBlittable gravityModifierBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.MainModule.get_gravityModifierBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_gravityModifierBlittable_Injected(ref this, ref value);
				}
			}

			public extern float gravityModifierMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemSimulationSpace simulationSpace
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public Transform customSimulationSpace
			{
				get
				{
					return Unmarshal.UnmarshalUnityObject<Transform>(ParticleSystem.MainModule.get_customSimulationSpace_Injected(ref this));
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_customSimulationSpace_Injected(ref this, Object.MarshalledUnityObject.Marshal<Transform>(value));
				}
			}

			public extern float simulationSpeed
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool useUnscaledTime
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemScalingMode scalingMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool playOnAwake
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int maxParticles
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemEmitterVelocityMode emitterVelocityMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemStopAction stopAction
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemRingBufferMode ringBufferMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public Vector2 ringBufferLoopRange
			{
				get
				{
					Vector2 vector;
					ParticleSystem.MainModule.get_ringBufferLoopRange_Injected(ref this, out vector);
					return vector;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.MainModule.set_ringBufferLoopRange_Injected(ref this, ref value);
				}
			}

			public extern ParticleSystemCullingMode cullingMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_emitterVelocity_Injected(ref ParticleSystem.MainModule _unity_self, out Vector3 ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_emitterVelocity_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref Vector3 value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startDelayBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startDelayBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startLifetimeBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startLifetimeBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startSpeedBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startSpeedBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startSizeBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startSizeBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startSizeXBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startSizeXBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startSizeYBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startSizeYBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startSizeZBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startSizeZBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startRotationBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startRotationBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startRotationXBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startRotationXBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startRotationYBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startRotationYBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startRotationZBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startRotationZBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startColorBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxGradientBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startColorBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxGradientBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_gravityModifierBlittable_Injected(ref ParticleSystem.MainModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_gravityModifierBlittable_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr get_customSimulationSpace_Injected(ref ParticleSystem.MainModule _unity_self);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_customSimulationSpace_Injected(ref ParticleSystem.MainModule _unity_self, IntPtr value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_ringBufferLoopRange_Injected(ref ParticleSystem.MainModule _unity_self, out Vector2 ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_ringBufferLoopRange_Injected(ref ParticleSystem.MainModule _unity_self, [In] ref Vector2 value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct EmissionModule
		{
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

			internal EmissionModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve rateOverTime
			{
				get
				{
					return this.rateOverTimeBlittable;
				}
				set
				{
					this.rateOverTimeBlittable = value;
				}
			}

			[NativeName("RateOverTime")]
			private ParticleSystem.MinMaxCurveBlittable rateOverTimeBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.EmissionModule.get_rateOverTimeBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.EmissionModule.set_rateOverTimeBlittable_Injected(ref this, ref value);
				}
			}

			public extern float rateOverTimeMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve rateOverDistance
			{
				get
				{
					return this.rateOverDistanceBlittable;
				}
				set
				{
					this.rateOverDistanceBlittable = value;
				}
			}

			[NativeName("RateOverDistance")]
			private ParticleSystem.MinMaxCurveBlittable rateOverDistanceBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.EmissionModule.get_rateOverDistanceBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.EmissionModule.set_rateOverDistanceBlittable_Injected(ref this, ref value);
				}
			}

			public extern float rateOverDistanceMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
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
					this.SetBurst(i, bursts[i]);
				}
			}

			public int GetBursts(ParticleSystem.Burst[] bursts)
			{
				int burstCount = this.burstCount;
				for (int i = 0; i < burstCount; i++)
				{
					bursts[i] = this.GetBurst(i);
				}
				return burstCount;
			}

			[NativeThrows]
			public void SetBurst(int index, ParticleSystem.Burst burst)
			{
				ParticleSystem.EmissionModule.SetBurst_Injected(ref this, index, ref burst);
			}

			[NativeThrows]
			public ParticleSystem.Burst GetBurst(int index)
			{
				ParticleSystem.Burst burst;
				ParticleSystem.EmissionModule.GetBurst_Injected(ref this, index, out burst);
				return burst;
			}

			public extern int burstCount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_rateOverTimeBlittable_Injected(ref ParticleSystem.EmissionModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_rateOverTimeBlittable_Injected(ref ParticleSystem.EmissionModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_rateOverDistanceBlittable_Injected(ref ParticleSystem.EmissionModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_rateOverDistanceBlittable_Injected(ref ParticleSystem.EmissionModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBurst_Injected(ref ParticleSystem.EmissionModule _unity_self, int index, [In] ref ParticleSystem.Burst burst);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetBurst_Injected(ref ParticleSystem.EmissionModule _unity_self, int index, out ParticleSystem.Burst ret);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct ShapeModule
		{
			[Obsolete("Please use scale instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/ShapeModule.scale", false)]
			public Vector3 box
			{
				get
				{
					return this.scale;
				}
				set
				{
					this.scale = value;
				}
			}

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

			[Obsolete("randomDirection property is deprecated. Use randomDirectionAmount instead.", false)]
			public bool randomDirection
			{
				get
				{
					return this.randomDirectionAmount >= 0.5f;
				}
				set
				{
					this.randomDirectionAmount = (value ? 1f : 0f);
				}
			}

			internal ShapeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemShapeType shapeType
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float randomDirectionAmount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float sphericalDirectionAmount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float randomPositionAmount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool alignToDirection
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float radius
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemShapeMultiModeValue radiusMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float radiusSpread
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve radiusSpeed
			{
				get
				{
					return this.radiusSpeedBlittable;
				}
				set
				{
					this.radiusSpeedBlittable = value;
				}
			}

			[NativeName("RadiusSpeed")]
			private ParticleSystem.MinMaxCurveBlittable radiusSpeedBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.ShapeModule.get_radiusSpeedBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_radiusSpeedBlittable_Injected(ref this, ref value);
				}
			}

			public extern float radiusSpeedMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float radiusThickness
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float angle
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float length
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public Vector3 boxThickness
			{
				get
				{
					Vector3 vector;
					ParticleSystem.ShapeModule.get_boxThickness_Injected(ref this, out vector);
					return vector;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_boxThickness_Injected(ref this, ref value);
				}
			}

			public extern ParticleSystemMeshShapeType meshShapeType
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public Mesh mesh
			{
				get
				{
					return Unmarshal.UnmarshalUnityObject<Mesh>(ParticleSystem.ShapeModule.get_mesh_Injected(ref this));
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_mesh_Injected(ref this, Object.MarshalledUnityObject.Marshal<Mesh>(value));
				}
			}

			public MeshRenderer meshRenderer
			{
				get
				{
					return Unmarshal.UnmarshalUnityObject<MeshRenderer>(ParticleSystem.ShapeModule.get_meshRenderer_Injected(ref this));
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_meshRenderer_Injected(ref this, Object.MarshalledUnityObject.Marshal<MeshRenderer>(value));
				}
			}

			public SkinnedMeshRenderer skinnedMeshRenderer
			{
				get
				{
					return Unmarshal.UnmarshalUnityObject<SkinnedMeshRenderer>(ParticleSystem.ShapeModule.get_skinnedMeshRenderer_Injected(ref this));
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_skinnedMeshRenderer_Injected(ref this, Object.MarshalledUnityObject.Marshal<SkinnedMeshRenderer>(value));
				}
			}

			public Sprite sprite
			{
				get
				{
					return Unmarshal.UnmarshalUnityObject<Sprite>(ParticleSystem.ShapeModule.get_sprite_Injected(ref this));
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_sprite_Injected(ref this, Object.MarshalledUnityObject.Marshal<Sprite>(value));
				}
			}

			public SpriteRenderer spriteRenderer
			{
				get
				{
					return Unmarshal.UnmarshalUnityObject<SpriteRenderer>(ParticleSystem.ShapeModule.get_spriteRenderer_Injected(ref this));
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_spriteRenderer_Injected(ref this, Object.MarshalledUnityObject.Marshal<SpriteRenderer>(value));
				}
			}

			public extern bool useMeshMaterialIndex
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int meshMaterialIndex
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool useMeshColors
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float normalOffset
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemShapeMultiModeValue meshSpawnMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float meshSpawnSpread
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve meshSpawnSpeed
			{
				get
				{
					return this.meshSpawnSpeedBlittable;
				}
				set
				{
					this.meshSpawnSpeedBlittable = value;
				}
			}

			[NativeName("MeshSpawnSpeed")]
			private ParticleSystem.MinMaxCurveBlittable meshSpawnSpeedBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.ShapeModule.get_meshSpawnSpeedBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_meshSpawnSpeedBlittable_Injected(ref this, ref value);
				}
			}

			public extern float meshSpawnSpeedMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float arc
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemShapeMultiModeValue arcMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float arcSpread
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve arcSpeed
			{
				get
				{
					return this.arcSpeedBlittable;
				}
				set
				{
					this.arcSpeedBlittable = value;
				}
			}

			[NativeName("ArcSpeed")]
			private ParticleSystem.MinMaxCurveBlittable arcSpeedBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.ShapeModule.get_arcSpeedBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_arcSpeedBlittable_Injected(ref this, ref value);
				}
			}

			public extern float arcSpeedMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float donutRadius
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public Vector3 position
			{
				get
				{
					Vector3 vector;
					ParticleSystem.ShapeModule.get_position_Injected(ref this, out vector);
					return vector;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_position_Injected(ref this, ref value);
				}
			}

			public Vector3 rotation
			{
				get
				{
					Vector3 vector;
					ParticleSystem.ShapeModule.get_rotation_Injected(ref this, out vector);
					return vector;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_rotation_Injected(ref this, ref value);
				}
			}

			public Vector3 scale
			{
				get
				{
					Vector3 vector;
					ParticleSystem.ShapeModule.get_scale_Injected(ref this, out vector);
					return vector;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_scale_Injected(ref this, ref value);
				}
			}

			public Texture2D texture
			{
				get
				{
					return Unmarshal.UnmarshalUnityObject<Texture2D>(ParticleSystem.ShapeModule.get_texture_Injected(ref this));
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ShapeModule.set_texture_Injected(ref this, Object.MarshalledUnityObject.Marshal<Texture2D>(value));
				}
			}

			public extern ParticleSystemShapeTextureChannel textureClipChannel
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float textureClipThreshold
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool textureColorAffectsParticles
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool textureAlphaAffectsParticles
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool textureBilinearFiltering
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int textureUVChannel
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_radiusSpeedBlittable_Injected(ref ParticleSystem.ShapeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_radiusSpeedBlittable_Injected(ref ParticleSystem.ShapeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_boxThickness_Injected(ref ParticleSystem.ShapeModule _unity_self, out Vector3 ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_boxThickness_Injected(ref ParticleSystem.ShapeModule _unity_self, [In] ref Vector3 value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr get_mesh_Injected(ref ParticleSystem.ShapeModule _unity_self);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_mesh_Injected(ref ParticleSystem.ShapeModule _unity_self, IntPtr value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr get_meshRenderer_Injected(ref ParticleSystem.ShapeModule _unity_self);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_meshRenderer_Injected(ref ParticleSystem.ShapeModule _unity_self, IntPtr value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr get_skinnedMeshRenderer_Injected(ref ParticleSystem.ShapeModule _unity_self);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_skinnedMeshRenderer_Injected(ref ParticleSystem.ShapeModule _unity_self, IntPtr value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr get_sprite_Injected(ref ParticleSystem.ShapeModule _unity_self);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_sprite_Injected(ref ParticleSystem.ShapeModule _unity_self, IntPtr value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr get_spriteRenderer_Injected(ref ParticleSystem.ShapeModule _unity_self);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_spriteRenderer_Injected(ref ParticleSystem.ShapeModule _unity_self, IntPtr value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_meshSpawnSpeedBlittable_Injected(ref ParticleSystem.ShapeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_meshSpawnSpeedBlittable_Injected(ref ParticleSystem.ShapeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_arcSpeedBlittable_Injected(ref ParticleSystem.ShapeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_arcSpeedBlittable_Injected(ref ParticleSystem.ShapeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_position_Injected(ref ParticleSystem.ShapeModule _unity_self, out Vector3 ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_position_Injected(ref ParticleSystem.ShapeModule _unity_self, [In] ref Vector3 value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_rotation_Injected(ref ParticleSystem.ShapeModule _unity_self, out Vector3 ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_rotation_Injected(ref ParticleSystem.ShapeModule _unity_self, [In] ref Vector3 value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_scale_Injected(ref ParticleSystem.ShapeModule _unity_self, out Vector3 ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_scale_Injected(ref ParticleSystem.ShapeModule _unity_self, [In] ref Vector3 value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr get_texture_Injected(ref ParticleSystem.ShapeModule _unity_self);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_texture_Injected(ref ParticleSystem.ShapeModule _unity_self, IntPtr value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct CollisionModule
		{
			[Obsolete("The maxPlaneCount restriction has been removed. Please use planeCount instead to find out how many planes there are. (UnityUpgradable) -> UnityEngine.ParticleSystem/CollisionModule.planeCount", false)]
			public int maxPlaneCount
			{
				get
				{
					return this.planeCount;
				}
			}

			internal CollisionModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemCollisionType type
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemCollisionMode mode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve dampen
			{
				get
				{
					return this.dampenBlittable;
				}
				set
				{
					this.dampenBlittable = value;
				}
			}

			[NativeName("Dampen")]
			private ParticleSystem.MinMaxCurveBlittable dampenBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.CollisionModule.get_dampenBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.CollisionModule.set_dampenBlittable_Injected(ref this, ref value);
				}
			}

			public extern float dampenMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve bounce
			{
				get
				{
					return this.bounceBlittable;
				}
				set
				{
					this.bounceBlittable = value;
				}
			}

			[NativeName("Bounce")]
			private ParticleSystem.MinMaxCurveBlittable bounceBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.CollisionModule.get_bounceBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.CollisionModule.set_bounceBlittable_Injected(ref this, ref value);
				}
			}

			public extern float bounceMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve lifetimeLoss
			{
				get
				{
					return this.lifetimeLossBlittable;
				}
				set
				{
					this.lifetimeLossBlittable = value;
				}
			}

			[NativeName("LifetimeLoss")]
			private ParticleSystem.MinMaxCurveBlittable lifetimeLossBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.CollisionModule.get_lifetimeLossBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.CollisionModule.set_lifetimeLossBlittable_Injected(ref this, ref value);
				}
			}

			public extern float lifetimeLossMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float minKillSpeed
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float maxKillSpeed
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public LayerMask collidesWith
			{
				get
				{
					LayerMask layerMask;
					ParticleSystem.CollisionModule.get_collidesWith_Injected(ref this, out layerMask);
					return layerMask;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.CollisionModule.set_collidesWith_Injected(ref this, ref value);
				}
			}

			public extern bool enableDynamicColliders
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int maxCollisionShapes
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemCollisionQuality quality
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float voxelSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float radiusScale
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool sendCollisionMessages
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float colliderForce
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool multiplyColliderForceByCollisionAngle
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool multiplyColliderForceByParticleSpeed
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool multiplyColliderForceByParticleSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[NativeThrows]
			public void AddPlane(Transform transform)
			{
				ParticleSystem.CollisionModule.AddPlane_Injected(ref this, Object.MarshalledUnityObject.Marshal<Transform>(transform));
			}

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern void RemovePlane(int index);

			public void RemovePlane(Transform transform)
			{
				this.RemovePlaneObject(transform);
			}

			[NativeThrows]
			private void RemovePlaneObject(Transform transform)
			{
				ParticleSystem.CollisionModule.RemovePlaneObject_Injected(ref this, Object.MarshalledUnityObject.Marshal<Transform>(transform));
			}

			[NativeThrows]
			public void SetPlane(int index, Transform transform)
			{
				ParticleSystem.CollisionModule.SetPlane_Injected(ref this, index, Object.MarshalledUnityObject.Marshal<Transform>(transform));
			}

			[NativeThrows]
			public Transform GetPlane(int index)
			{
				return Unmarshal.UnmarshalUnityObject<Transform>(ParticleSystem.CollisionModule.GetPlane_Injected(ref this, index));
			}

			[NativeThrows]
			public extern int planeCount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			[Obsolete("enableInteriorCollisions property is deprecated and is no longer required and has no effect on the particle system.", false)]
			public extern bool enableInteriorCollisions
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_dampenBlittable_Injected(ref ParticleSystem.CollisionModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_dampenBlittable_Injected(ref ParticleSystem.CollisionModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_bounceBlittable_Injected(ref ParticleSystem.CollisionModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_bounceBlittable_Injected(ref ParticleSystem.CollisionModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_lifetimeLossBlittable_Injected(ref ParticleSystem.CollisionModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_lifetimeLossBlittable_Injected(ref ParticleSystem.CollisionModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_collidesWith_Injected(ref ParticleSystem.CollisionModule _unity_self, out LayerMask ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_collidesWith_Injected(ref ParticleSystem.CollisionModule _unity_self, [In] ref LayerMask value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void AddPlane_Injected(ref ParticleSystem.CollisionModule _unity_self, IntPtr transform);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void RemovePlaneObject_Injected(ref ParticleSystem.CollisionModule _unity_self, IntPtr transform);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetPlane_Injected(ref ParticleSystem.CollisionModule _unity_self, int index, IntPtr transform);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr GetPlane_Injected(ref ParticleSystem.CollisionModule _unity_self, int index);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct TriggerModule
		{
			[Obsolete("The maxColliderCount restriction has been removed. Please use colliderCount instead to find out how many colliders there are. (UnityUpgradable) -> UnityEngine.ParticleSystem/TriggerModule.colliderCount", false)]
			public int maxColliderCount
			{
				get
				{
					return this.colliderCount;
				}
			}

			internal TriggerModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemOverlapAction inside
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemOverlapAction outside
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemOverlapAction enter
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemOverlapAction exit
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemColliderQueryMode colliderQueryMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float radiusScale
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[NativeThrows]
			public void AddCollider(Component collider)
			{
				ParticleSystem.TriggerModule.AddCollider_Injected(ref this, Object.MarshalledUnityObject.Marshal<Component>(collider));
			}

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern void RemoveCollider(int index);

			public void RemoveCollider(Component collider)
			{
				this.RemoveColliderObject(collider);
			}

			[NativeThrows]
			private void RemoveColliderObject(Component collider)
			{
				ParticleSystem.TriggerModule.RemoveColliderObject_Injected(ref this, Object.MarshalledUnityObject.Marshal<Component>(collider));
			}

			[NativeThrows]
			public void SetCollider(int index, Component collider)
			{
				ParticleSystem.TriggerModule.SetCollider_Injected(ref this, index, Object.MarshalledUnityObject.Marshal<Component>(collider));
			}

			[NativeThrows]
			public Component GetCollider(int index)
			{
				return Unmarshal.UnmarshalUnityObject<Component>(ParticleSystem.TriggerModule.GetCollider_Injected(ref this, index));
			}

			[NativeThrows]
			public extern int colliderCount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void AddCollider_Injected(ref ParticleSystem.TriggerModule _unity_self, IntPtr collider);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void RemoveColliderObject_Injected(ref ParticleSystem.TriggerModule _unity_self, IntPtr collider);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCollider_Injected(ref ParticleSystem.TriggerModule _unity_self, int index, IntPtr collider);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr GetCollider_Injected(ref ParticleSystem.TriggerModule _unity_self, int index);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct SubEmittersModule
		{
			[Obsolete("birth0 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
			public ParticleSystem birth0
			{
				get
				{
					ParticleSystem.SubEmittersModule.ThrowNotImplemented();
					return null;
				}
				set
				{
					ParticleSystem.SubEmittersModule.ThrowNotImplemented();
				}
			}

			[Obsolete("birth1 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
			public ParticleSystem birth1
			{
				get
				{
					ParticleSystem.SubEmittersModule.ThrowNotImplemented();
					return null;
				}
				set
				{
					ParticleSystem.SubEmittersModule.ThrowNotImplemented();
				}
			}

			[Obsolete("collision0 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
			public ParticleSystem collision0
			{
				get
				{
					ParticleSystem.SubEmittersModule.ThrowNotImplemented();
					return null;
				}
				set
				{
					ParticleSystem.SubEmittersModule.ThrowNotImplemented();
				}
			}

			[Obsolete("collision1 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
			public ParticleSystem collision1
			{
				get
				{
					ParticleSystem.SubEmittersModule.ThrowNotImplemented();
					return null;
				}
				set
				{
					ParticleSystem.SubEmittersModule.ThrowNotImplemented();
				}
			}

			[Obsolete("death0 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
			public ParticleSystem death0
			{
				get
				{
					ParticleSystem.SubEmittersModule.ThrowNotImplemented();
					return null;
				}
				set
				{
					ParticleSystem.SubEmittersModule.ThrowNotImplemented();
				}
			}

			[Obsolete("death1 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
			public ParticleSystem death1
			{
				get
				{
					ParticleSystem.SubEmittersModule.ThrowNotImplemented();
					return null;
				}
				set
				{
					ParticleSystem.SubEmittersModule.ThrowNotImplemented();
				}
			}

			private static void ThrowNotImplemented()
			{
				throw new NotImplementedException();
			}

			internal SubEmittersModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int subEmittersCount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			[NativeThrows]
			public void AddSubEmitter(ParticleSystem subEmitter, ParticleSystemSubEmitterType type, ParticleSystemSubEmitterProperties properties, float emitProbability)
			{
				ParticleSystem.SubEmittersModule.AddSubEmitter_Injected(ref this, Object.MarshalledUnityObject.Marshal<ParticleSystem>(subEmitter), type, properties, emitProbability);
			}

			public void AddSubEmitter(ParticleSystem subEmitter, ParticleSystemSubEmitterType type, ParticleSystemSubEmitterProperties properties)
			{
				this.AddSubEmitter(subEmitter, type, properties, 1f);
			}

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern void RemoveSubEmitter(int index);

			public void RemoveSubEmitter(ParticleSystem subEmitter)
			{
				this.RemoveSubEmitterObject(subEmitter);
			}

			[NativeThrows]
			private void RemoveSubEmitterObject(ParticleSystem subEmitter)
			{
				ParticleSystem.SubEmittersModule.RemoveSubEmitterObject_Injected(ref this, Object.MarshalledUnityObject.Marshal<ParticleSystem>(subEmitter));
			}

			[NativeThrows]
			public void SetSubEmitterSystem(int index, ParticleSystem subEmitter)
			{
				ParticleSystem.SubEmittersModule.SetSubEmitterSystem_Injected(ref this, index, Object.MarshalledUnityObject.Marshal<ParticleSystem>(subEmitter));
			}

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern void SetSubEmitterType(int index, ParticleSystemSubEmitterType type);

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern void SetSubEmitterProperties(int index, ParticleSystemSubEmitterProperties properties);

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern void SetSubEmitterEmitProbability(int index, float emitProbability);

			[NativeThrows]
			public ParticleSystem GetSubEmitterSystem(int index)
			{
				return Unmarshal.UnmarshalUnityObject<ParticleSystem>(ParticleSystem.SubEmittersModule.GetSubEmitterSystem_Injected(ref this, index));
			}

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern ParticleSystemSubEmitterType GetSubEmitterType(int index);

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern ParticleSystemSubEmitterProperties GetSubEmitterProperties(int index);

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern float GetSubEmitterEmitProbability(int index);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void AddSubEmitter_Injected(ref ParticleSystem.SubEmittersModule _unity_self, IntPtr subEmitter, ParticleSystemSubEmitterType type, ParticleSystemSubEmitterProperties properties, float emitProbability);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void RemoveSubEmitterObject_Injected(ref ParticleSystem.SubEmittersModule _unity_self, IntPtr subEmitter);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSubEmitterSystem_Injected(ref ParticleSystem.SubEmittersModule _unity_self, int index, IntPtr subEmitter);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr GetSubEmitterSystem_Injected(ref ParticleSystem.SubEmittersModule _unity_self, int index);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct TextureSheetAnimationModule
		{
			[Obsolete("flipU property is deprecated. Use ParticleSystemRenderer.flip.x instead.", false)]
			public float flipU
			{
				get
				{
					return this.m_ParticleSystem.GetComponent<ParticleSystemRenderer>().flip.x;
				}
				set
				{
					ParticleSystemRenderer component = this.m_ParticleSystem.GetComponent<ParticleSystemRenderer>();
					Vector3 flip = component.flip;
					flip.x = value;
					component.flip = flip;
				}
			}

			[Obsolete("flipV property is deprecated. Use ParticleSystemRenderer.flip.y instead.", false)]
			public float flipV
			{
				get
				{
					return this.m_ParticleSystem.GetComponent<ParticleSystemRenderer>().flip.y;
				}
				set
				{
					ParticleSystemRenderer component = this.m_ParticleSystem.GetComponent<ParticleSystemRenderer>();
					Vector3 flip = component.flip;
					flip.y = value;
					component.flip = flip;
				}
			}

			[Obsolete("useRandomRow property is deprecated. Use rowMode instead.", false)]
			public bool useRandomRow
			{
				get
				{
					return this.rowMode == ParticleSystemAnimationRowMode.Random;
				}
				set
				{
					this.rowMode = (value ? ParticleSystemAnimationRowMode.Random : ParticleSystemAnimationRowMode.Custom);
				}
			}

			internal TextureSheetAnimationModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemAnimationMode mode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemAnimationTimeMode timeMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float fps
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int numTilesX
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int numTilesY
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemAnimationType animation
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemAnimationRowMode rowMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve frameOverTime
			{
				get
				{
					return this.frameOverTimeBlittable;
				}
				set
				{
					this.frameOverTimeBlittable = value;
				}
			}

			[NativeName("FrameOverTime")]
			private ParticleSystem.MinMaxCurveBlittable frameOverTimeBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.TextureSheetAnimationModule.get_frameOverTimeBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.TextureSheetAnimationModule.set_frameOverTimeBlittable_Injected(ref this, ref value);
				}
			}

			public extern float frameOverTimeMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve startFrame
			{
				get
				{
					return this.startFrameBlittable;
				}
				set
				{
					this.startFrameBlittable = value;
				}
			}

			[NativeName("StartFrame")]
			private ParticleSystem.MinMaxCurveBlittable startFrameBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.TextureSheetAnimationModule.get_startFrameBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.TextureSheetAnimationModule.set_startFrameBlittable_Injected(ref this, ref value);
				}
			}

			public extern float startFrameMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int cycleCount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int rowIndex
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern UVChannelFlags uvChannelMask
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int spriteCount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public Vector2 speedRange
			{
				get
				{
					Vector2 vector;
					ParticleSystem.TextureSheetAnimationModule.get_speedRange_Injected(ref this, out vector);
					return vector;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.TextureSheetAnimationModule.set_speedRange_Injected(ref this, ref value);
				}
			}

			[NativeThrows]
			public void AddSprite(Sprite sprite)
			{
				ParticleSystem.TextureSheetAnimationModule.AddSprite_Injected(ref this, Object.MarshalledUnityObject.Marshal<Sprite>(sprite));
			}

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern void RemoveSprite(int index);

			[NativeThrows]
			public void SetSprite(int index, Sprite sprite)
			{
				ParticleSystem.TextureSheetAnimationModule.SetSprite_Injected(ref this, index, Object.MarshalledUnityObject.Marshal<Sprite>(sprite));
			}

			[NativeThrows]
			public Sprite GetSprite(int index)
			{
				return Unmarshal.UnmarshalUnityObject<Sprite>(ParticleSystem.TextureSheetAnimationModule.GetSprite_Injected(ref this, index));
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_frameOverTimeBlittable_Injected(ref ParticleSystem.TextureSheetAnimationModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_frameOverTimeBlittable_Injected(ref ParticleSystem.TextureSheetAnimationModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_startFrameBlittable_Injected(ref ParticleSystem.TextureSheetAnimationModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_startFrameBlittable_Injected(ref ParticleSystem.TextureSheetAnimationModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_speedRange_Injected(ref ParticleSystem.TextureSheetAnimationModule _unity_self, out Vector2 ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_speedRange_Injected(ref ParticleSystem.TextureSheetAnimationModule _unity_self, [In] ref Vector2 value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void AddSprite_Injected(ref ParticleSystem.TextureSheetAnimationModule _unity_self, IntPtr sprite);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSprite_Injected(ref ParticleSystem.TextureSheetAnimationModule _unity_self, int index, IntPtr sprite);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr GetSprite_Injected(ref ParticleSystem.TextureSheetAnimationModule _unity_self, int index);

			internal ParticleSystem m_ParticleSystem;
		}

		[RequiredByNativeCode("particleSystemParticle", Optional = true)]
		public struct Particle
		{
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

			[Obsolete("randomValue property is deprecated. Use randomSeed instead to control random behavior of particles.", false)]
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

			[Obsolete("size property is deprecated. Use startSize or GetCurrentSize() instead.", false)]
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

			[Obsolete("color property is deprecated. Use startColor or GetCurrentColor() instead.", false)]
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

			public Vector3 animatedVelocity
			{
				get
				{
					return this.m_AnimatedVelocity;
				}
			}

			public Vector3 totalVelocity
			{
				get
				{
					return this.m_Velocity + this.m_AnimatedVelocity;
				}
			}

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

			public Vector3 startSize3D
			{
				get
				{
					return this.m_StartSize;
				}
				set
				{
					this.m_StartSize = value;
					this.m_Flags |= 1U;
				}
			}

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

			public Vector3 rotation3D
			{
				get
				{
					return this.m_Rotation * 57.29578f;
				}
				set
				{
					this.m_Rotation = value * 0.017453292f;
					this.m_Flags |= 2U;
				}
			}

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

			public Vector3 angularVelocity3D
			{
				get
				{
					return this.m_AngularVelocity * 57.29578f;
				}
				set
				{
					this.m_AngularVelocity = value * 0.017453292f;
					this.m_Flags |= 2U;
				}
			}

			public float GetCurrentSize(ParticleSystem system)
			{
				return system.GetParticleCurrentSize(ref this);
			}

			public Vector3 GetCurrentSize3D(ParticleSystem system)
			{
				return system.GetParticleCurrentSize3D(ref this);
			}

			public Color32 GetCurrentColor(ParticleSystem system)
			{
				return system.GetParticleCurrentColor(ref this);
			}

			public void SetMeshIndex(int index)
			{
				this.m_MeshIndex = index;
				this.m_Flags |= 4U;
			}

			public int GetMeshIndex(ParticleSystem system)
			{
				return system.GetParticleMeshIndex(ref this);
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

			private uint m_ParentRandomSeed;

			private float m_Lifetime;

			private float m_StartLifetime;

			private int m_MeshIndex;

			private float m_EmitAccumulator0;

			private float m_EmitAccumulator1;

			private uint m_Flags;

			[Flags]
			private enum Flags
			{
				Size3D = 1,
				Rotation3D = 2,
				MeshIndex = 4
			}
		}

		[NativeType(CodegenOptions.Custom, "MonoBurst", Header = "Runtime/Scripting/ScriptingCommonStructDefinitions.h")]
		public struct Burst
		{
			public Burst(float _time, short _count)
			{
				this.m_Time = _time;
				ParticleSystem.MinMaxCurve minMaxCurve = (float)_count;
				this.m_Count = ParticleSystem.MinMaxCurveBlittable.FromMixMaxCurve(in minMaxCurve);
				this.m_RepeatCount = 0;
				this.m_RepeatInterval = 0f;
				this.m_InvProbability = 0f;
			}

			public Burst(float _time, short _minCount, short _maxCount)
			{
				this.m_Time = _time;
				ParticleSystem.MinMaxCurve minMaxCurve = new ParticleSystem.MinMaxCurve((float)_minCount, (float)_maxCount);
				this.m_Count = ParticleSystem.MinMaxCurveBlittable.FromMixMaxCurve(in minMaxCurve);
				this.m_RepeatCount = 0;
				this.m_RepeatInterval = 0f;
				this.m_InvProbability = 0f;
			}

			public Burst(float _time, short _minCount, short _maxCount, int _cycleCount, float _repeatInterval)
			{
				this.m_Time = _time;
				ParticleSystem.MinMaxCurve minMaxCurve = new ParticleSystem.MinMaxCurve((float)_minCount, (float)_maxCount);
				this.m_Count = ParticleSystem.MinMaxCurveBlittable.FromMixMaxCurve(in minMaxCurve);
				this.m_RepeatCount = _cycleCount - 1;
				this.m_RepeatInterval = _repeatInterval;
				this.m_InvProbability = 0f;
			}

			public Burst(float _time, ParticleSystem.MinMaxCurve _count)
			{
				this.m_Time = _time;
				this.m_Count = ParticleSystem.MinMaxCurveBlittable.FromMixMaxCurve(in _count);
				this.m_RepeatCount = 0;
				this.m_RepeatInterval = 0f;
				this.m_InvProbability = 0f;
			}

			public Burst(float _time, ParticleSystem.MinMaxCurve _count, int _cycleCount, float _repeatInterval)
			{
				this.m_Time = _time;
				this.m_Count = ParticleSystem.MinMaxCurveBlittable.FromMixMaxCurve(in _count);
				this.m_RepeatCount = _cycleCount - 1;
				this.m_RepeatInterval = _repeatInterval;
				this.m_InvProbability = 0f;
			}

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

			public ParticleSystem.MinMaxCurve count
			{
				get
				{
					return ParticleSystem.MinMaxCurveBlittable.ToMinMaxCurve(in this.m_Count);
				}
				set
				{
					this.m_Count = ParticleSystem.MinMaxCurveBlittable.FromMixMaxCurve(in value);
				}
			}

			public short minCount
			{
				get
				{
					return (short)this.m_Count.m_ConstantMin;
				}
				set
				{
					this.m_Count.m_ConstantMin = (float)value;
				}
			}

			public short maxCount
			{
				get
				{
					return (short)this.m_Count.m_ConstantMax;
				}
				set
				{
					this.m_Count.m_ConstantMax = (float)value;
				}
			}

			public int cycleCount
			{
				get
				{
					return this.m_RepeatCount + 1;
				}
				set
				{
					bool flag = value < 0;
					if (flag)
					{
						throw new ArgumentOutOfRangeException("cycleCount", "cycleCount must be at least 0: " + value.ToString());
					}
					this.m_RepeatCount = value - 1;
				}
			}

			public float repeatInterval
			{
				get
				{
					return this.m_RepeatInterval;
				}
				set
				{
					bool flag = value <= 0f;
					if (flag)
					{
						throw new ArgumentOutOfRangeException("repeatInterval", "repeatInterval must be greater than 0.0f: " + value.ToString());
					}
					this.m_RepeatInterval = value;
				}
			}

			public float probability
			{
				get
				{
					return 1f - this.m_InvProbability;
				}
				set
				{
					bool flag = value < 0f || value > 1f;
					if (flag)
					{
						throw new ArgumentOutOfRangeException("probability", "probability must be between 0.0f and 1.0f: " + value.ToString());
					}
					this.m_InvProbability = 1f - value;
				}
			}

			private float m_Time;

			private ParticleSystem.MinMaxCurveBlittable m_Count;

			private int m_RepeatCount;

			private float m_RepeatInterval;

			private float m_InvProbability;
		}

		[Serializable]
		public struct MinMaxCurve
		{
			public MinMaxCurve(float constant)
			{
				this.m_Mode = ParticleSystemCurveMode.Constant;
				this.m_CurveMultiplier = 0f;
				this.m_CurveMin = null;
				this.m_CurveMax = null;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = constant;
			}

			public MinMaxCurve(float multiplier, AnimationCurve curve)
			{
				this.m_Mode = ParticleSystemCurveMode.Curve;
				this.m_CurveMultiplier = multiplier;
				this.m_CurveMin = null;
				this.m_CurveMax = curve;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = 0f;
			}

			public MinMaxCurve(float multiplier, AnimationCurve min, AnimationCurve max)
			{
				this.m_Mode = ParticleSystemCurveMode.TwoCurves;
				this.m_CurveMultiplier = multiplier;
				this.m_CurveMin = min;
				this.m_CurveMax = max;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = 0f;
			}

			public MinMaxCurve(float min, float max)
			{
				this.m_Mode = ParticleSystemCurveMode.TwoConstants;
				this.m_CurveMultiplier = 0f;
				this.m_CurveMin = null;
				this.m_CurveMax = null;
				this.m_ConstantMin = min;
				this.m_ConstantMax = max;
			}

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

			public float Evaluate(float time)
			{
				return this.Evaluate(time, 1f);
			}

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
			internal ParticleSystemCurveMode m_Mode;

			[SerializeField]
			internal float m_CurveMultiplier;

			[SerializeField]
			internal AnimationCurve m_CurveMin;

			[SerializeField]
			internal AnimationCurve m_CurveMax;

			[SerializeField]
			internal float m_ConstantMin;

			[SerializeField]
			internal float m_ConstantMax;
		}

		[NativeType(CodegenOptions.Custom, "MonoMinMaxCurve", Header = "Runtime/Scripting/ScriptingCommonStructDefinitions.h")]
		[RequiredByNativeCode]
		[Serializable]
		internal struct MinMaxCurveBlittable
		{
			public static implicit operator ParticleSystem.MinMaxCurve(ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable)
			{
				return ParticleSystem.MinMaxCurveBlittable.ToMinMaxCurve(in minMaxCurveBlittable);
			}

			public static implicit operator ParticleSystem.MinMaxCurveBlittable(ParticleSystem.MinMaxCurve minMaxCurve)
			{
				return ParticleSystem.MinMaxCurveBlittable.FromMixMaxCurve(in minMaxCurve);
			}

			internal static ParticleSystem.MinMaxCurveBlittable FromMixMaxCurve(in ParticleSystem.MinMaxCurve minMaxCurve)
			{
				ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable = new ParticleSystem.MinMaxCurveBlittable
				{
					m_Mode = minMaxCurve.m_Mode,
					m_CurveMultiplier = minMaxCurve.m_CurveMultiplier,
					m_ConstantMin = minMaxCurve.m_ConstantMin,
					m_ConstantMax = minMaxCurve.m_ConstantMax
				};
				bool flag = minMaxCurve.m_CurveMin != null;
				if (flag)
				{
					minMaxCurveBlittable.m_CurveMin = minMaxCurve.m_CurveMin.m_Ptr;
				}
				bool flag2 = minMaxCurve.m_CurveMax != null;
				if (flag2)
				{
					minMaxCurveBlittable.m_CurveMax = minMaxCurve.m_CurveMax.m_Ptr;
				}
				return minMaxCurveBlittable;
			}

			internal static ParticleSystem.MinMaxCurve ToMinMaxCurve(in ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable)
			{
				ParticleSystem.MinMaxCurve minMaxCurve = default(ParticleSystem.MinMaxCurve);
				minMaxCurve.m_Mode = minMaxCurveBlittable.m_Mode;
				minMaxCurve.m_CurveMultiplier = minMaxCurveBlittable.m_CurveMultiplier;
				bool flag = minMaxCurveBlittable.m_CurveMin != IntPtr.Zero;
				if (flag)
				{
					minMaxCurve.m_CurveMin = new AnimationCurve(minMaxCurveBlittable.m_CurveMin, false);
				}
				bool flag2 = minMaxCurveBlittable.m_CurveMax != IntPtr.Zero;
				if (flag2)
				{
					minMaxCurve.m_CurveMax = new AnimationCurve(minMaxCurveBlittable.m_CurveMax, false);
				}
				minMaxCurve.m_ConstantMin = minMaxCurveBlittable.m_ConstantMin;
				minMaxCurve.m_ConstantMax = minMaxCurveBlittable.m_ConstantMax;
				return minMaxCurve;
			}

			private ParticleSystemCurveMode m_Mode;

			private float m_CurveMultiplier;

			private IntPtr m_CurveMin;

			private IntPtr m_CurveMax;

			internal float m_ConstantMin;

			internal float m_ConstantMax;
		}

		[Serializable]
		public struct MinMaxGradient
		{
			public MinMaxGradient(Color color)
			{
				this.m_Mode = ParticleSystemGradientMode.Color;
				this.m_GradientMin = null;
				this.m_GradientMax = null;
				this.m_ColorMin = Color.black;
				this.m_ColorMax = color;
			}

			public MinMaxGradient(Gradient gradient)
			{
				this.m_Mode = ParticleSystemGradientMode.Gradient;
				this.m_GradientMin = null;
				this.m_GradientMax = gradient;
				this.m_ColorMin = Color.black;
				this.m_ColorMax = Color.black;
			}

			public MinMaxGradient(Color min, Color max)
			{
				this.m_Mode = ParticleSystemGradientMode.TwoColors;
				this.m_GradientMin = null;
				this.m_GradientMax = null;
				this.m_ColorMin = min;
				this.m_ColorMax = max;
			}

			public MinMaxGradient(Gradient min, Gradient max)
			{
				this.m_Mode = ParticleSystemGradientMode.TwoGradients;
				this.m_GradientMin = min;
				this.m_GradientMax = max;
				this.m_ColorMin = Color.black;
				this.m_ColorMax = Color.black;
			}

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

			public Color Evaluate(float time)
			{
				return this.Evaluate(time, 1f);
			}

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
			internal ParticleSystemGradientMode m_Mode;

			[SerializeField]
			internal Gradient m_GradientMin;

			[SerializeField]
			internal Gradient m_GradientMax;

			[SerializeField]
			internal Color m_ColorMin;

			[SerializeField]
			internal Color m_ColorMax;
		}

		[RequiredByNativeCode]
		[NativeType(CodegenOptions.Custom, "MonoMinMaxGradient", Header = "Runtime/Scripting/ScriptingCommonStructDefinitions.h")]
		[Serializable]
		internal struct MinMaxGradientBlittable
		{
			public static implicit operator ParticleSystem.MinMaxGradient(ParticleSystem.MinMaxGradientBlittable minMaxGradientBlittable)
			{
				return ParticleSystem.MinMaxGradientBlittable.ToMinMaxGradient(in minMaxGradientBlittable);
			}

			public static implicit operator ParticleSystem.MinMaxGradientBlittable(ParticleSystem.MinMaxGradient minMaxGradient)
			{
				return ParticleSystem.MinMaxGradientBlittable.FromMixMaxGradient(in minMaxGradient);
			}

			internal static ParticleSystem.MinMaxGradientBlittable FromMixMaxGradient(in ParticleSystem.MinMaxGradient minMaxGradient)
			{
				ParticleSystem.MinMaxGradientBlittable minMaxGradientBlittable = new ParticleSystem.MinMaxGradientBlittable
				{
					m_Mode = minMaxGradient.m_Mode,
					m_ColorMin = minMaxGradient.m_ColorMin,
					m_ColorMax = minMaxGradient.m_ColorMax
				};
				bool flag = minMaxGradient.m_GradientMin != null;
				if (flag)
				{
					minMaxGradientBlittable.m_GradientMin = minMaxGradient.m_GradientMin.m_Ptr;
				}
				bool flag2 = minMaxGradient.m_GradientMax != null;
				if (flag2)
				{
					minMaxGradientBlittable.m_GradientMax = minMaxGradient.m_GradientMax.m_Ptr;
				}
				return minMaxGradientBlittable;
			}

			internal static ParticleSystem.MinMaxGradient ToMinMaxGradient(in ParticleSystem.MinMaxGradientBlittable minMaxGradientBlittable)
			{
				ParticleSystem.MinMaxGradient minMaxGradient = default(ParticleSystem.MinMaxGradient);
				minMaxGradient.m_Mode = minMaxGradientBlittable.m_Mode;
				bool flag = minMaxGradientBlittable.m_GradientMin != IntPtr.Zero;
				if (flag)
				{
					minMaxGradient.m_GradientMin = new Gradient(minMaxGradientBlittable.m_GradientMin);
				}
				bool flag2 = minMaxGradientBlittable.m_GradientMax != IntPtr.Zero;
				if (flag2)
				{
					minMaxGradient.m_GradientMax = new Gradient(minMaxGradientBlittable.m_GradientMax);
				}
				minMaxGradient.m_ColorMin = minMaxGradientBlittable.m_ColorMin;
				minMaxGradient.m_ColorMax = minMaxGradientBlittable.m_ColorMax;
				return minMaxGradient;
			}

			private ParticleSystemGradientMode m_Mode;

			private IntPtr m_GradientMin;

			private IntPtr m_GradientMax;

			private Color m_ColorMin;

			private Color m_ColorMax;
		}

		public struct EmitParams
		{
			public ParticleSystem.Particle particle
			{
				get
				{
					return this.m_Particle;
				}
				set
				{
					this.m_Particle = value;
					this.m_PositionSet = true;
					this.m_VelocitySet = true;
					this.m_AxisOfRotationSet = true;
					this.m_RotationSet = true;
					this.m_AngularVelocitySet = true;
					this.m_StartSizeSet = true;
					this.m_StartColorSet = true;
					this.m_RandomSeedSet = true;
					this.m_StartLifetimeSet = true;
					this.m_MeshIndexSet = true;
				}
			}

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

			public int meshIndex
			{
				set
				{
					this.m_Particle.SetMeshIndex(value);
					this.m_MeshIndexSet = true;
				}
			}

			public void ResetPosition()
			{
				this.m_PositionSet = false;
			}

			public void ResetVelocity()
			{
				this.m_VelocitySet = false;
			}

			public void ResetAxisOfRotation()
			{
				this.m_AxisOfRotationSet = false;
			}

			public void ResetRotation()
			{
				this.m_RotationSet = false;
			}

			public void ResetAngularVelocity()
			{
				this.m_AngularVelocitySet = false;
			}

			public void ResetStartSize()
			{
				this.m_StartSizeSet = false;
			}

			public void ResetStartColor()
			{
				this.m_StartColorSet = false;
			}

			public void ResetRandomSeed()
			{
				this.m_RandomSeedSet = false;
			}

			public void ResetStartLifetime()
			{
				this.m_StartLifetimeSet = false;
			}

			public void ResetMeshIndex()
			{
				this.m_MeshIndexSet = false;
			}

			[NativeName("particle")]
			private ParticleSystem.Particle m_Particle;

			[NativeName("positionSet")]
			private bool m_PositionSet;

			[NativeName("velocitySet")]
			private bool m_VelocitySet;

			[NativeName("axisOfRotationSet")]
			private bool m_AxisOfRotationSet;

			[NativeName("rotationSet")]
			private bool m_RotationSet;

			[NativeName("rotationalSpeedSet")]
			private bool m_AngularVelocitySet;

			[NativeName("startSizeSet")]
			private bool m_StartSizeSet;

			[NativeName("startColorSet")]
			private bool m_StartColorSet;

			[NativeName("randomSeedSet")]
			private bool m_RandomSeedSet;

			[NativeName("startLifetimeSet")]
			private bool m_StartLifetimeSet;

			[NativeName("meshIndexSet")]
			private bool m_MeshIndexSet;

			[NativeName("applyShapeToPosition")]
			private bool m_ApplyShapeToPosition;
		}

		public struct PlaybackState
		{
			internal float m_AccumulatedDt;

			internal float m_StartDelay;

			internal float m_PlaybackTime;

			internal int m_RingBufferIndex;

			internal ParticleSystem.PlaybackState.Emission m_Emission;

			internal ParticleSystem.PlaybackState.Initial m_Initial;

			internal ParticleSystem.PlaybackState.Shape m_Shape;

			internal ParticleSystem.PlaybackState.Force m_Force;

			internal ParticleSystem.PlaybackState.Collision m_Collision;

			internal ParticleSystem.PlaybackState.Noise m_Noise;

			internal ParticleSystem.PlaybackState.Lights m_Lights;

			internal ParticleSystem.PlaybackState.Trail m_Trail;

			internal struct Seed
			{
				public uint x;

				public uint y;

				public uint z;

				public uint w;
			}

			internal struct Seed4
			{
				public ParticleSystem.PlaybackState.Seed x;

				public ParticleSystem.PlaybackState.Seed y;

				public ParticleSystem.PlaybackState.Seed z;

				public ParticleSystem.PlaybackState.Seed w;
			}

			internal struct Emission
			{
				public float m_ParticleSpacing;

				public float m_ToEmitAccumulator;

				public ParticleSystem.PlaybackState.Seed m_Random;
			}

			internal struct Initial
			{
				public ParticleSystem.PlaybackState.Seed4 m_Random;
			}

			internal struct Shape
			{
				public ParticleSystem.PlaybackState.Seed4 m_Random;

				public float m_RadiusTimer;

				public float m_RadiusTimerPrev;

				public float m_ArcTimer;

				public float m_ArcTimerPrev;

				public float m_MeshSpawnTimer;

				public float m_MeshSpawnTimerPrev;

				public int m_OrderedMeshVertexIndex;
			}

			internal struct Force
			{
				public ParticleSystem.PlaybackState.Seed4 m_Random;
			}

			internal struct Collision
			{
				public ParticleSystem.PlaybackState.Seed4 m_Random;
			}

			internal struct Noise
			{
				public float m_ScrollOffset;
			}

			internal struct Lights
			{
				public ParticleSystem.PlaybackState.Seed m_Random;

				public float m_ParticleEmissionCounter;
			}

			internal struct Trail
			{
				public float m_Timer;
			}
		}

		[NativeType(CodegenOptions.Custom, "MonoParticleTrails")]
		public struct Trails
		{
			internal void Allocate()
			{
				bool flag = this.positions == null;
				if (flag)
				{
					this.positions = new List<Vector4>();
				}
				bool flag2 = this.frontPositions == null;
				if (flag2)
				{
					this.frontPositions = new List<int>();
				}
				bool flag3 = this.backPositions == null;
				if (flag3)
				{
					this.backPositions = new List<int>();
				}
				bool flag4 = this.positionCounts == null;
				if (flag4)
				{
					this.positionCounts = new List<int>();
				}
				bool flag5 = this.textureOffsets == null;
				if (flag5)
				{
					this.textureOffsets = new List<float>();
				}
			}

			public int capacity
			{
				get
				{
					bool flag = this.positions == null;
					int num;
					if (flag)
					{
						num = 0;
					}
					else
					{
						num = this.positions.Capacity;
					}
					return num;
				}
				set
				{
					this.Allocate();
					this.positions.Capacity = value;
					this.frontPositions.Capacity = value;
					this.backPositions.Capacity = value;
					this.positionCounts.Capacity = value;
					this.textureOffsets.Capacity = value;
				}
			}

			internal List<Vector4> positions;

			internal List<int> frontPositions;

			internal List<int> backPositions;

			internal List<int> positionCounts;

			internal List<float> textureOffsets;

			internal int maxTrailCount;

			internal int maxPositionsPerTrailCount;
		}

		public struct ColliderData
		{
			public int GetColliderCount(int particleIndex)
			{
				bool flag = particleIndex < this.particleStartIndices.Length - 1;
				int num;
				if (flag)
				{
					num = this.particleStartIndices[particleIndex + 1] - this.particleStartIndices[particleIndex];
				}
				else
				{
					num = this.colliderIndices.Length - this.particleStartIndices[particleIndex];
				}
				return num;
			}

			public Component GetCollider(int particleIndex, int colliderIndex)
			{
				bool flag = colliderIndex >= this.GetColliderCount(particleIndex);
				if (flag)
				{
					throw new IndexOutOfRangeException("colliderIndex exceeded the total number of colliders for the requested particle");
				}
				int num = this.particleStartIndices[particleIndex] + colliderIndex;
				return this.colliders[this.colliderIndices[num]];
			}

			internal Component[] colliders;

			internal int[] colliderIndices;

			internal int[] particleStartIndices;
		}

		public struct VelocityOverLifetimeModule
		{
			internal VelocityOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					return this.xBlittable;
				}
				set
				{
					this.xBlittable = value;
				}
			}

			[NativeName("X")]
			private ParticleSystem.MinMaxCurveBlittable xBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.VelocityOverLifetimeModule.get_xBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.set_xBlittable_Injected(ref this, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					return this.yBlittable;
				}
				set
				{
					this.yBlittable = value;
				}
			}

			[NativeName("Y")]
			private ParticleSystem.MinMaxCurveBlittable yBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.VelocityOverLifetimeModule.get_yBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.set_yBlittable_Injected(ref this, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					return this.zBlittable;
				}
				set
				{
					this.zBlittable = value;
				}
			}

			[NativeName("Z")]
			private ParticleSystem.MinMaxCurveBlittable zBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.VelocityOverLifetimeModule.get_zBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.set_zBlittable_Injected(ref this, ref value);
				}
			}

			public extern float xMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float yMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float zMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve orbitalX
			{
				get
				{
					return this.orbitalXBlittable;
				}
				set
				{
					this.orbitalXBlittable = value;
				}
			}

			[NativeName("OrbitalX")]
			private ParticleSystem.MinMaxCurveBlittable orbitalXBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.VelocityOverLifetimeModule.get_orbitalXBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.set_orbitalXBlittable_Injected(ref this, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve orbitalY
			{
				get
				{
					return this.orbitalYBlittable;
				}
				set
				{
					this.orbitalYBlittable = value;
				}
			}

			[NativeName("OrbitalY")]
			private ParticleSystem.MinMaxCurveBlittable orbitalYBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.VelocityOverLifetimeModule.get_orbitalYBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.set_orbitalYBlittable_Injected(ref this, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve orbitalZ
			{
				get
				{
					return this.orbitalZBlittable;
				}
				set
				{
					this.orbitalZBlittable = value;
				}
			}

			[NativeName("OrbitalZ")]
			private ParticleSystem.MinMaxCurveBlittable orbitalZBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.VelocityOverLifetimeModule.get_orbitalZBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.set_orbitalZBlittable_Injected(ref this, ref value);
				}
			}

			public extern float orbitalXMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float orbitalYMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float orbitalZMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve orbitalOffsetX
			{
				get
				{
					return this.orbitalOffsetXBlittable;
				}
				set
				{
					this.orbitalOffsetXBlittable = value;
				}
			}

			[NativeName("OrbitalOffsetX")]
			private ParticleSystem.MinMaxCurveBlittable orbitalOffsetXBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.VelocityOverLifetimeModule.get_orbitalOffsetXBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.set_orbitalOffsetXBlittable_Injected(ref this, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve orbitalOffsetY
			{
				get
				{
					return this.orbitalOffsetYBlittable;
				}
				set
				{
					this.orbitalOffsetYBlittable = value;
				}
			}

			[NativeName("OrbitalOffsetY")]
			private ParticleSystem.MinMaxCurveBlittable orbitalOffsetYBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.VelocityOverLifetimeModule.get_orbitalOffsetYBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.set_orbitalOffsetYBlittable_Injected(ref this, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve orbitalOffsetZ
			{
				get
				{
					return this.orbitalOffsetZBlittable;
				}
				set
				{
					this.orbitalOffsetZBlittable = value;
				}
			}

			[NativeName("OrbitalOffsetZ")]
			private ParticleSystem.MinMaxCurveBlittable orbitalOffsetZBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.VelocityOverLifetimeModule.get_orbitalOffsetZBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.set_orbitalOffsetZBlittable_Injected(ref this, ref value);
				}
			}

			public extern float orbitalOffsetXMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float orbitalOffsetYMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float orbitalOffsetZMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve radial
			{
				get
				{
					return this.radialBlittable;
				}
				set
				{
					this.radialBlittable = value;
				}
			}

			[NativeName("Radial")]
			private ParticleSystem.MinMaxCurveBlittable radialBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.VelocityOverLifetimeModule.get_radialBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.set_radialBlittable_Injected(ref this, ref value);
				}
			}

			public extern float radialMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve speedModifier
			{
				get
				{
					return this.speedModifierBlittable;
				}
				set
				{
					this.speedModifierBlittable = value;
				}
			}

			[NativeName("SpeedModifier")]
			private ParticleSystem.MinMaxCurveBlittable speedModifierBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.VelocityOverLifetimeModule.get_speedModifierBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.set_speedModifierBlittable_Injected(ref this, ref value);
				}
			}

			public extern float speedModifierMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemSimulationSpace space
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_xBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_xBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_yBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_yBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_zBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_zBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_orbitalXBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_orbitalXBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_orbitalYBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_orbitalYBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_orbitalZBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_orbitalZBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_orbitalOffsetXBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_orbitalOffsetXBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_orbitalOffsetYBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_orbitalOffsetYBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_orbitalOffsetZBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_orbitalOffsetZBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_radialBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_radialBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_speedModifierBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_speedModifierBlittable_Injected(ref ParticleSystem.VelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct LimitVelocityOverLifetimeModule
		{
			internal LimitVelocityOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve limitX
			{
				get
				{
					return this.limitXBlittable;
				}
				set
				{
					this.limitXBlittable = value;
				}
			}

			[NativeName("LimitX")]
			private ParticleSystem.MinMaxCurveBlittable limitXBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.LimitVelocityOverLifetimeModule.get_limitXBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.set_limitXBlittable_Injected(ref this, ref value);
				}
			}

			public extern float limitXMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve limitY
			{
				get
				{
					return this.limitYBlittable;
				}
				set
				{
					this.limitYBlittable = value;
				}
			}

			[NativeName("LimitY")]
			private ParticleSystem.MinMaxCurveBlittable limitYBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.LimitVelocityOverLifetimeModule.get_limitYBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.set_limitYBlittable_Injected(ref this, ref value);
				}
			}

			public extern float limitYMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve limitZ
			{
				get
				{
					return this.limitZBlittable;
				}
				set
				{
					this.limitZBlittable = value;
				}
			}

			[NativeName("LimitZ")]
			private ParticleSystem.MinMaxCurveBlittable limitZBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.LimitVelocityOverLifetimeModule.get_limitZBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.set_limitZBlittable_Injected(ref this, ref value);
				}
			}

			public extern float limitZMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve limit
			{
				get
				{
					return this.limitBlittable;
				}
				set
				{
					this.limitBlittable = value;
				}
			}

			[NativeName("Magnitude")]
			private ParticleSystem.MinMaxCurveBlittable limitBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.LimitVelocityOverLifetimeModule.get_limitBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.set_limitBlittable_Injected(ref this, ref value);
				}
			}

			[NativeName("MagnitudeMultiplier")]
			public extern float limitMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float dampen
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool separateAxes
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemSimulationSpace space
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve drag
			{
				get
				{
					return this.dragBlittable;
				}
				set
				{
					this.dragBlittable = value;
				}
			}

			[NativeName("Drag")]
			private ParticleSystem.MinMaxCurveBlittable dragBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.LimitVelocityOverLifetimeModule.get_dragBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.set_dragBlittable_Injected(ref this, ref value);
				}
			}

			public extern float dragMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool multiplyDragByParticleSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool multiplyDragByParticleVelocity
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_limitXBlittable_Injected(ref ParticleSystem.LimitVelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_limitXBlittable_Injected(ref ParticleSystem.LimitVelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_limitYBlittable_Injected(ref ParticleSystem.LimitVelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_limitYBlittable_Injected(ref ParticleSystem.LimitVelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_limitZBlittable_Injected(ref ParticleSystem.LimitVelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_limitZBlittable_Injected(ref ParticleSystem.LimitVelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_limitBlittable_Injected(ref ParticleSystem.LimitVelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_limitBlittable_Injected(ref ParticleSystem.LimitVelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_dragBlittable_Injected(ref ParticleSystem.LimitVelocityOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_dragBlittable_Injected(ref ParticleSystem.LimitVelocityOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct InheritVelocityModule
		{
			internal InheritVelocityModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemInheritVelocityMode mode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve curve
			{
				get
				{
					return this.curveBlittable;
				}
				set
				{
					this.curveBlittable = value;
				}
			}

			[NativeName("Curve")]
			private ParticleSystem.MinMaxCurveBlittable curveBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.InheritVelocityModule.get_curveBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.InheritVelocityModule.set_curveBlittable_Injected(ref this, ref value);
				}
			}

			public extern float curveMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_curveBlittable_Injected(ref ParticleSystem.InheritVelocityModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_curveBlittable_Injected(ref ParticleSystem.InheritVelocityModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct LifetimeByEmitterSpeedModule
		{
			internal LifetimeByEmitterSpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve curve
			{
				get
				{
					return this.curveBlittable;
				}
				set
				{
					this.curveBlittable = value;
				}
			}

			[NativeName("Curve")]
			private ParticleSystem.MinMaxCurveBlittable curveBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.LifetimeByEmitterSpeedModule.get_curveBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.LifetimeByEmitterSpeedModule.set_curveBlittable_Injected(ref this, ref value);
				}
			}

			public extern float curveMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public Vector2 range
			{
				get
				{
					Vector2 vector;
					ParticleSystem.LifetimeByEmitterSpeedModule.get_range_Injected(ref this, out vector);
					return vector;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.LifetimeByEmitterSpeedModule.set_range_Injected(ref this, ref value);
				}
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_curveBlittable_Injected(ref ParticleSystem.LifetimeByEmitterSpeedModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_curveBlittable_Injected(ref ParticleSystem.LifetimeByEmitterSpeedModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_range_Injected(ref ParticleSystem.LifetimeByEmitterSpeedModule _unity_self, out Vector2 ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_range_Injected(ref ParticleSystem.LifetimeByEmitterSpeedModule _unity_self, [In] ref Vector2 value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct ForceOverLifetimeModule
		{
			internal ForceOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					return this.xBlittable;
				}
				set
				{
					this.xBlittable = value;
				}
			}

			[NativeName("X")]
			private ParticleSystem.MinMaxCurveBlittable xBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.ForceOverLifetimeModule.get_xBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ForceOverLifetimeModule.set_xBlittable_Injected(ref this, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					return this.yBlittable;
				}
				set
				{
					this.yBlittable = value;
				}
			}

			[NativeName("Y")]
			private ParticleSystem.MinMaxCurveBlittable yBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.ForceOverLifetimeModule.get_yBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ForceOverLifetimeModule.set_yBlittable_Injected(ref this, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					return this.zBlittable;
				}
				set
				{
					this.zBlittable = value;
				}
			}

			[NativeName("Z")]
			private ParticleSystem.MinMaxCurveBlittable zBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.ForceOverLifetimeModule.get_zBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ForceOverLifetimeModule.set_zBlittable_Injected(ref this, ref value);
				}
			}

			public extern float xMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float yMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float zMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemSimulationSpace space
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool randomized
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_xBlittable_Injected(ref ParticleSystem.ForceOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_xBlittable_Injected(ref ParticleSystem.ForceOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_yBlittable_Injected(ref ParticleSystem.ForceOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_yBlittable_Injected(ref ParticleSystem.ForceOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_zBlittable_Injected(ref ParticleSystem.ForceOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_zBlittable_Injected(ref ParticleSystem.ForceOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct ColorOverLifetimeModule
		{
			internal ColorOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxGradient color
			{
				get
				{
					return this.colorBlittable;
				}
				set
				{
					this.colorBlittable = value;
				}
			}

			[NativeName("Color")]
			private ParticleSystem.MinMaxGradientBlittable colorBlittable
			{
				get
				{
					ParticleSystem.MinMaxGradientBlittable minMaxGradientBlittable;
					ParticleSystem.ColorOverLifetimeModule.get_colorBlittable_Injected(ref this, out minMaxGradientBlittable);
					return minMaxGradientBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ColorOverLifetimeModule.set_colorBlittable_Injected(ref this, ref value);
				}
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_colorBlittable_Injected(ref ParticleSystem.ColorOverLifetimeModule _unity_self, out ParticleSystem.MinMaxGradientBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_colorBlittable_Injected(ref ParticleSystem.ColorOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxGradientBlittable value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct ColorBySpeedModule
		{
			internal ColorBySpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxGradient color
			{
				get
				{
					return this.colorBlittable;
				}
				set
				{
					this.colorBlittable = value;
				}
			}

			[NativeName("Color")]
			private ParticleSystem.MinMaxGradientBlittable colorBlittable
			{
				get
				{
					ParticleSystem.MinMaxGradientBlittable minMaxGradientBlittable;
					ParticleSystem.ColorBySpeedModule.get_colorBlittable_Injected(ref this, out minMaxGradientBlittable);
					return minMaxGradientBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ColorBySpeedModule.set_colorBlittable_Injected(ref this, ref value);
				}
			}

			public Vector2 range
			{
				get
				{
					Vector2 vector;
					ParticleSystem.ColorBySpeedModule.get_range_Injected(ref this, out vector);
					return vector;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ColorBySpeedModule.set_range_Injected(ref this, ref value);
				}
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_colorBlittable_Injected(ref ParticleSystem.ColorBySpeedModule _unity_self, out ParticleSystem.MinMaxGradientBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_colorBlittable_Injected(ref ParticleSystem.ColorBySpeedModule _unity_self, [In] ref ParticleSystem.MinMaxGradientBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_range_Injected(ref ParticleSystem.ColorBySpeedModule _unity_self, out Vector2 ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_range_Injected(ref ParticleSystem.ColorBySpeedModule _unity_self, [In] ref Vector2 value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct SizeOverLifetimeModule
		{
			internal SizeOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve size
			{
				get
				{
					return this.sizeBlittable;
				}
				set
				{
					this.sizeBlittable = value;
				}
			}

			[NativeName("X")]
			private ParticleSystem.MinMaxCurveBlittable sizeBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.SizeOverLifetimeModule.get_sizeBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.SizeOverLifetimeModule.set_sizeBlittable_Injected(ref this, ref value);
				}
			}

			[NativeName("XMultiplier")]
			public extern float sizeMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					return this.xBlittable;
				}
				set
				{
					this.xBlittable = value;
				}
			}

			[NativeName("X")]
			private ParticleSystem.MinMaxCurveBlittable xBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.SizeOverLifetimeModule.get_xBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.SizeOverLifetimeModule.set_xBlittable_Injected(ref this, ref value);
				}
			}

			public extern float xMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					return this.yBlittable;
				}
				set
				{
					this.yBlittable = value;
				}
			}

			[NativeName("Y")]
			private ParticleSystem.MinMaxCurveBlittable yBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.SizeOverLifetimeModule.get_yBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.SizeOverLifetimeModule.set_yBlittable_Injected(ref this, ref value);
				}
			}

			public extern float yMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					return this.zBlittable;
				}
				set
				{
					this.zBlittable = value;
				}
			}

			[NativeName("Z")]
			private ParticleSystem.MinMaxCurveBlittable zBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.SizeOverLifetimeModule.get_zBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.SizeOverLifetimeModule.set_zBlittable_Injected(ref this, ref value);
				}
			}

			public extern float zMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool separateAxes
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_sizeBlittable_Injected(ref ParticleSystem.SizeOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_sizeBlittable_Injected(ref ParticleSystem.SizeOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_xBlittable_Injected(ref ParticleSystem.SizeOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_xBlittable_Injected(ref ParticleSystem.SizeOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_yBlittable_Injected(ref ParticleSystem.SizeOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_yBlittable_Injected(ref ParticleSystem.SizeOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_zBlittable_Injected(ref ParticleSystem.SizeOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_zBlittable_Injected(ref ParticleSystem.SizeOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct SizeBySpeedModule
		{
			internal SizeBySpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve size
			{
				get
				{
					return this.sizeBlittable;
				}
				set
				{
					this.sizeBlittable = value;
				}
			}

			[NativeName("X")]
			private ParticleSystem.MinMaxCurveBlittable sizeBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.SizeBySpeedModule.get_sizeBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.SizeBySpeedModule.set_sizeBlittable_Injected(ref this, ref value);
				}
			}

			[NativeName("XMultiplier")]
			public extern float sizeMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					return this.xBlittable;
				}
				set
				{
					this.xBlittable = value;
				}
			}

			[NativeName("X")]
			private ParticleSystem.MinMaxCurveBlittable xBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.SizeBySpeedModule.get_xBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.SizeBySpeedModule.set_xBlittable_Injected(ref this, ref value);
				}
			}

			public extern float xMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					return this.yBlittable;
				}
				set
				{
					this.yBlittable = value;
				}
			}

			[NativeName("Y")]
			private ParticleSystem.MinMaxCurveBlittable yBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.SizeBySpeedModule.get_yBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.SizeBySpeedModule.set_yBlittable_Injected(ref this, ref value);
				}
			}

			public extern float yMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					return this.zBlittable;
				}
				set
				{
					this.zBlittable = value;
				}
			}

			[NativeName("Z")]
			private ParticleSystem.MinMaxCurveBlittable zBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.SizeBySpeedModule.get_zBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.SizeBySpeedModule.set_zBlittable_Injected(ref this, ref value);
				}
			}

			public extern float zMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool separateAxes
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public Vector2 range
			{
				get
				{
					Vector2 vector;
					ParticleSystem.SizeBySpeedModule.get_range_Injected(ref this, out vector);
					return vector;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.SizeBySpeedModule.set_range_Injected(ref this, ref value);
				}
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_sizeBlittable_Injected(ref ParticleSystem.SizeBySpeedModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_sizeBlittable_Injected(ref ParticleSystem.SizeBySpeedModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_xBlittable_Injected(ref ParticleSystem.SizeBySpeedModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_xBlittable_Injected(ref ParticleSystem.SizeBySpeedModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_yBlittable_Injected(ref ParticleSystem.SizeBySpeedModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_yBlittable_Injected(ref ParticleSystem.SizeBySpeedModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_zBlittable_Injected(ref ParticleSystem.SizeBySpeedModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_zBlittable_Injected(ref ParticleSystem.SizeBySpeedModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_range_Injected(ref ParticleSystem.SizeBySpeedModule _unity_self, out Vector2 ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_range_Injected(ref ParticleSystem.SizeBySpeedModule _unity_self, [In] ref Vector2 value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct RotationOverLifetimeModule
		{
			internal RotationOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					return this.xBlittable;
				}
				set
				{
					this.xBlittable = value;
				}
			}

			[NativeName("X")]
			private ParticleSystem.MinMaxCurveBlittable xBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.RotationOverLifetimeModule.get_xBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.RotationOverLifetimeModule.set_xBlittable_Injected(ref this, ref value);
				}
			}

			public extern float xMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					return this.yBlittable;
				}
				set
				{
					this.yBlittable = value;
				}
			}

			[NativeName("Y")]
			private ParticleSystem.MinMaxCurveBlittable yBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.RotationOverLifetimeModule.get_yBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.RotationOverLifetimeModule.set_yBlittable_Injected(ref this, ref value);
				}
			}

			public extern float yMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					return this.zBlittable;
				}
				set
				{
					this.zBlittable = value;
				}
			}

			[NativeName("Z")]
			private ParticleSystem.MinMaxCurveBlittable zBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.RotationOverLifetimeModule.get_zBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.RotationOverLifetimeModule.set_zBlittable_Injected(ref this, ref value);
				}
			}

			public extern float zMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool separateAxes
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_xBlittable_Injected(ref ParticleSystem.RotationOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_xBlittable_Injected(ref ParticleSystem.RotationOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_yBlittable_Injected(ref ParticleSystem.RotationOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_yBlittable_Injected(ref ParticleSystem.RotationOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_zBlittable_Injected(ref ParticleSystem.RotationOverLifetimeModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_zBlittable_Injected(ref ParticleSystem.RotationOverLifetimeModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct RotationBySpeedModule
		{
			internal RotationBySpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					return this.xBlittable;
				}
				set
				{
					this.xBlittable = value;
				}
			}

			[NativeName("X")]
			private ParticleSystem.MinMaxCurveBlittable xBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.RotationBySpeedModule.get_xBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.RotationBySpeedModule.set_xBlittable_Injected(ref this, ref value);
				}
			}

			public extern float xMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					return this.yBlittable;
				}
				set
				{
					this.yBlittable = value;
				}
			}

			[NativeName("Y")]
			private ParticleSystem.MinMaxCurveBlittable yBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.RotationBySpeedModule.get_yBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.RotationBySpeedModule.set_yBlittable_Injected(ref this, ref value);
				}
			}

			public extern float yMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					return this.zBlittable;
				}
				set
				{
					this.zBlittable = value;
				}
			}

			[NativeName("Z")]
			private ParticleSystem.MinMaxCurveBlittable zBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.RotationBySpeedModule.get_zBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.RotationBySpeedModule.set_zBlittable_Injected(ref this, ref value);
				}
			}

			public extern float zMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool separateAxes
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public Vector2 range
			{
				get
				{
					Vector2 vector;
					ParticleSystem.RotationBySpeedModule.get_range_Injected(ref this, out vector);
					return vector;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.RotationBySpeedModule.set_range_Injected(ref this, ref value);
				}
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_xBlittable_Injected(ref ParticleSystem.RotationBySpeedModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_xBlittable_Injected(ref ParticleSystem.RotationBySpeedModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_yBlittable_Injected(ref ParticleSystem.RotationBySpeedModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_yBlittable_Injected(ref ParticleSystem.RotationBySpeedModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_zBlittable_Injected(ref ParticleSystem.RotationBySpeedModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_zBlittable_Injected(ref ParticleSystem.RotationBySpeedModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_range_Injected(ref ParticleSystem.RotationBySpeedModule _unity_self, out Vector2 ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_range_Injected(ref ParticleSystem.RotationBySpeedModule _unity_self, [In] ref Vector2 value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct ExternalForcesModule
		{
			internal ExternalForcesModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float multiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve multiplierCurve
			{
				get
				{
					return this.multiplierCurveBlittable;
				}
				set
				{
					this.multiplierCurveBlittable = value;
				}
			}

			[NativeName("MultiplierCurve")]
			private ParticleSystem.MinMaxCurveBlittable multiplierCurveBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.ExternalForcesModule.get_multiplierCurveBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ExternalForcesModule.set_multiplierCurveBlittable_Injected(ref this, ref value);
				}
			}

			public extern ParticleSystemGameObjectFilter influenceFilter
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public LayerMask influenceMask
			{
				get
				{
					LayerMask layerMask;
					ParticleSystem.ExternalForcesModule.get_influenceMask_Injected(ref this, out layerMask);
					return layerMask;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.ExternalForcesModule.set_influenceMask_Injected(ref this, ref value);
				}
			}

			[NativeThrows]
			public extern int influenceCount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public bool IsAffectedBy(ParticleSystemForceField field)
			{
				return ParticleSystem.ExternalForcesModule.IsAffectedBy_Injected(ref this, Object.MarshalledUnityObject.Marshal<ParticleSystemForceField>(field));
			}

			[NativeThrows]
			public void AddInfluence([NotNull] ParticleSystemForceField field)
			{
				if (field == null)
				{
					ThrowHelper.ThrowArgumentNullException(field, "field");
				}
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(field);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(field, "field");
				}
				ParticleSystem.ExternalForcesModule.AddInfluence_Injected(ref this, intPtr);
			}

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private extern void RemoveInfluenceAtIndex(int index);

			public void RemoveInfluence(int index)
			{
				this.RemoveInfluenceAtIndex(index);
			}

			[NativeThrows]
			public void RemoveInfluence([NotNull] ParticleSystemForceField field)
			{
				if (field == null)
				{
					ThrowHelper.ThrowArgumentNullException(field, "field");
				}
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(field);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(field, "field");
				}
				ParticleSystem.ExternalForcesModule.RemoveInfluence_Injected(ref this, intPtr);
			}

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern void RemoveAllInfluences();

			[NativeThrows]
			public void SetInfluence(int index, [NotNull] ParticleSystemForceField field)
			{
				if (field == null)
				{
					ThrowHelper.ThrowArgumentNullException(field, "field");
				}
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(field);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(field, "field");
				}
				ParticleSystem.ExternalForcesModule.SetInfluence_Injected(ref this, index, intPtr);
			}

			[NativeThrows]
			public ParticleSystemForceField GetInfluence(int index)
			{
				return Unmarshal.UnmarshalUnityObject<ParticleSystemForceField>(ParticleSystem.ExternalForcesModule.GetInfluence_Injected(ref this, index));
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_multiplierCurveBlittable_Injected(ref ParticleSystem.ExternalForcesModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_multiplierCurveBlittable_Injected(ref ParticleSystem.ExternalForcesModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_influenceMask_Injected(ref ParticleSystem.ExternalForcesModule _unity_self, out LayerMask ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_influenceMask_Injected(ref ParticleSystem.ExternalForcesModule _unity_self, [In] ref LayerMask value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool IsAffectedBy_Injected(ref ParticleSystem.ExternalForcesModule _unity_self, IntPtr field);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void AddInfluence_Injected(ref ParticleSystem.ExternalForcesModule _unity_self, IntPtr field);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void RemoveInfluence_Injected(ref ParticleSystem.ExternalForcesModule _unity_self, IntPtr field);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetInfluence_Injected(ref ParticleSystem.ExternalForcesModule _unity_self, int index, IntPtr field);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr GetInfluence_Injected(ref ParticleSystem.ExternalForcesModule _unity_self, int index);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct NoiseModule
		{
			internal NoiseModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool separateAxes
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve strength
			{
				get
				{
					return this.strengthBlittable;
				}
				set
				{
					this.strengthBlittable = value;
				}
			}

			[NativeName("StrengthX")]
			private ParticleSystem.MinMaxCurveBlittable strengthBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.NoiseModule.get_strengthBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.NoiseModule.set_strengthBlittable_Injected(ref this, ref value);
				}
			}

			[NativeName("StrengthXMultiplier")]
			public extern float strengthMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve strengthX
			{
				get
				{
					return this.strengthXBlittable;
				}
				set
				{
					this.strengthXBlittable = value;
				}
			}

			[NativeName("StrengthX")]
			private ParticleSystem.MinMaxCurveBlittable strengthXBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.NoiseModule.get_strengthXBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.NoiseModule.set_strengthXBlittable_Injected(ref this, ref value);
				}
			}

			public extern float strengthXMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve strengthY
			{
				get
				{
					return this.strengthYBlittable;
				}
				set
				{
					this.strengthYBlittable = value;
				}
			}

			[NativeName("StrengthY")]
			private ParticleSystem.MinMaxCurveBlittable strengthYBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.NoiseModule.get_strengthYBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.NoiseModule.set_strengthYBlittable_Injected(ref this, ref value);
				}
			}

			public extern float strengthYMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve strengthZ
			{
				get
				{
					return this.strengthZBlittable;
				}
				set
				{
					this.strengthZBlittable = value;
				}
			}

			[NativeName("StrengthZ")]
			private ParticleSystem.MinMaxCurveBlittable strengthZBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.NoiseModule.get_strengthZBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.NoiseModule.set_strengthZBlittable_Injected(ref this, ref value);
				}
			}

			public extern float strengthZMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float frequency
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool damping
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int octaveCount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float octaveMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float octaveScale
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemNoiseQuality quality
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve scrollSpeed
			{
				get
				{
					return this.scrollSpeedBlittable;
				}
				set
				{
					this.scrollSpeedBlittable = value;
				}
			}

			[NativeName("ScrollSpeed")]
			private ParticleSystem.MinMaxCurveBlittable scrollSpeedBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.NoiseModule.get_scrollSpeedBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.NoiseModule.set_scrollSpeedBlittable_Injected(ref this, ref value);
				}
			}

			public extern float scrollSpeedMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool remapEnabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve remap
			{
				get
				{
					return this.remapBlittable;
				}
				set
				{
					this.remapBlittable = value;
				}
			}

			[NativeName("RemapX")]
			private ParticleSystem.MinMaxCurveBlittable remapBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.NoiseModule.get_remapBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.NoiseModule.set_remapBlittable_Injected(ref this, ref value);
				}
			}

			[NativeName("RemapXMultiplier")]
			public extern float remapMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve remapX
			{
				get
				{
					return this.remapXBlittable;
				}
				set
				{
					this.remapXBlittable = value;
				}
			}

			[NativeName("RemapX")]
			private ParticleSystem.MinMaxCurveBlittable remapXBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.NoiseModule.get_remapXBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.NoiseModule.set_remapXBlittable_Injected(ref this, ref value);
				}
			}

			public extern float remapXMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve remapY
			{
				get
				{
					return this.remapYBlittable;
				}
				set
				{
					this.remapYBlittable = value;
				}
			}

			[NativeName("RemapY")]
			private ParticleSystem.MinMaxCurveBlittable remapYBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.NoiseModule.get_remapYBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.NoiseModule.set_remapYBlittable_Injected(ref this, ref value);
				}
			}

			public extern float remapYMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve remapZ
			{
				get
				{
					return this.remapZBlittable;
				}
				set
				{
					this.remapZBlittable = value;
				}
			}

			[NativeName("RemapZ")]
			private ParticleSystem.MinMaxCurveBlittable remapZBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.NoiseModule.get_remapZBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.NoiseModule.set_remapZBlittable_Injected(ref this, ref value);
				}
			}

			public extern float remapZMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve positionAmount
			{
				get
				{
					return this.positionAmountBlittable;
				}
				set
				{
					this.positionAmountBlittable = value;
				}
			}

			[NativeName("PositionAmount")]
			private ParticleSystem.MinMaxCurveBlittable positionAmountBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.NoiseModule.get_positionAmountBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.NoiseModule.set_positionAmountBlittable_Injected(ref this, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve rotationAmount
			{
				get
				{
					return this.rotationAmountBlittable;
				}
				set
				{
					this.rotationAmountBlittable = value;
				}
			}

			[NativeName("RotationAmount")]
			private ParticleSystem.MinMaxCurveBlittable rotationAmountBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.NoiseModule.get_rotationAmountBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.NoiseModule.set_rotationAmountBlittable_Injected(ref this, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve sizeAmount
			{
				get
				{
					return this.sizeAmountBlittable;
				}
				set
				{
					this.sizeAmountBlittable = value;
				}
			}

			[NativeName("SizeAmount")]
			private ParticleSystem.MinMaxCurveBlittable sizeAmountBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.NoiseModule.get_sizeAmountBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.NoiseModule.set_sizeAmountBlittable_Injected(ref this, ref value);
				}
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_strengthBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_strengthBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_strengthXBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_strengthXBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_strengthYBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_strengthYBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_strengthZBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_strengthZBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_scrollSpeedBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_scrollSpeedBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_remapBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_remapBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_remapXBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_remapXBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_remapYBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_remapYBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_remapZBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_remapZBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_positionAmountBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_positionAmountBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_rotationAmountBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_rotationAmountBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_sizeAmountBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_sizeAmountBlittable_Injected(ref ParticleSystem.NoiseModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct LightsModule
		{
			internal LightsModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float ratio
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool useRandomDistribution
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public Light light
			{
				get
				{
					return Unmarshal.UnmarshalUnityObject<Light>(ParticleSystem.LightsModule.get_light_Injected(ref this));
				}
				[NativeThrows]
				set
				{
					ParticleSystem.LightsModule.set_light_Injected(ref this, Object.MarshalledUnityObject.Marshal<Light>(value));
				}
			}

			public extern bool useParticleColor
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool sizeAffectsRange
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool alphaAffectsIntensity
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve range
			{
				get
				{
					return this.rangeBlittable;
				}
				set
				{
					this.rangeBlittable = value;
				}
			}

			[NativeName("Range")]
			private ParticleSystem.MinMaxCurveBlittable rangeBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.LightsModule.get_rangeBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.LightsModule.set_rangeBlittable_Injected(ref this, ref value);
				}
			}

			public extern float rangeMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve intensity
			{
				get
				{
					return this.intensityBlittable;
				}
				set
				{
					this.intensityBlittable = value;
				}
			}

			[NativeName("Intensity")]
			private ParticleSystem.MinMaxCurveBlittable intensityBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.LightsModule.get_intensityBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.LightsModule.set_intensityBlittable_Injected(ref this, ref value);
				}
			}

			public extern float intensityMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int maxLights
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr get_light_Injected(ref ParticleSystem.LightsModule _unity_self);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_light_Injected(ref ParticleSystem.LightsModule _unity_self, IntPtr value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_rangeBlittable_Injected(ref ParticleSystem.LightsModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_rangeBlittable_Injected(ref ParticleSystem.LightsModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_intensityBlittable_Injected(ref ParticleSystem.LightsModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_intensityBlittable_Injected(ref ParticleSystem.LightsModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct TrailModule
		{
			internal TrailModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemTrailMode mode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float ratio
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxCurve lifetime
			{
				get
				{
					return this.lifetimeBlittable;
				}
				set
				{
					this.lifetimeBlittable = value;
				}
			}

			[NativeName("Lifetime")]
			private ParticleSystem.MinMaxCurveBlittable lifetimeBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.TrailModule.get_lifetimeBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.TrailModule.set_lifetimeBlittable_Injected(ref this, ref value);
				}
			}

			public extern float lifetimeMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float minVertexDistance
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern ParticleSystemTrailTextureMode textureMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public Vector2 textureScale
			{
				get
				{
					Vector2 vector;
					ParticleSystem.TrailModule.get_textureScale_Injected(ref this, out vector);
					return vector;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.TrailModule.set_textureScale_Injected(ref this, ref value);
				}
			}

			public extern bool worldSpace
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool dieWithParticles
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool sizeAffectsWidth
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool sizeAffectsLifetime
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool inheritParticleColor
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxGradient colorOverLifetime
			{
				get
				{
					return this.colorOverLifetimeBlittable;
				}
				set
				{
					this.colorOverLifetimeBlittable = value;
				}
			}

			[NativeName("ColorOverLifetime")]
			private ParticleSystem.MinMaxGradientBlittable colorOverLifetimeBlittable
			{
				get
				{
					ParticleSystem.MinMaxGradientBlittable minMaxGradientBlittable;
					ParticleSystem.TrailModule.get_colorOverLifetimeBlittable_Injected(ref this, out minMaxGradientBlittable);
					return minMaxGradientBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.TrailModule.set_colorOverLifetimeBlittable_Injected(ref this, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve widthOverTrail
			{
				get
				{
					return this.widthOverTrailBlittable;
				}
				set
				{
					this.widthOverTrailBlittable = value;
				}
			}

			[NativeName("WidthOverTrail")]
			private ParticleSystem.MinMaxCurveBlittable widthOverTrailBlittable
			{
				get
				{
					ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
					ParticleSystem.TrailModule.get_widthOverTrailBlittable_Injected(ref this, out minMaxCurveBlittable);
					return minMaxCurveBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.TrailModule.set_widthOverTrailBlittable_Injected(ref this, ref value);
				}
			}

			public extern float widthOverTrailMultiplier
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public ParticleSystem.MinMaxGradient colorOverTrail
			{
				get
				{
					return this.colorOverTrailBlittable;
				}
				set
				{
					this.colorOverTrailBlittable = value;
				}
			}

			[NativeName("ColorOverTrail")]
			private ParticleSystem.MinMaxGradientBlittable colorOverTrailBlittable
			{
				get
				{
					ParticleSystem.MinMaxGradientBlittable minMaxGradientBlittable;
					ParticleSystem.TrailModule.get_colorOverTrailBlittable_Injected(ref this, out minMaxGradientBlittable);
					return minMaxGradientBlittable;
				}
				[NativeThrows]
				set
				{
					ParticleSystem.TrailModule.set_colorOverTrailBlittable_Injected(ref this, ref value);
				}
			}

			public extern bool generateLightingData
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern int ribbonCount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern float shadowBias
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool splitSubEmitterRibbons
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public extern bool attachRibbonsToTransform
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_lifetimeBlittable_Injected(ref ParticleSystem.TrailModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_lifetimeBlittable_Injected(ref ParticleSystem.TrailModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_textureScale_Injected(ref ParticleSystem.TrailModule _unity_self, out Vector2 ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_textureScale_Injected(ref ParticleSystem.TrailModule _unity_self, [In] ref Vector2 value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_colorOverLifetimeBlittable_Injected(ref ParticleSystem.TrailModule _unity_self, out ParticleSystem.MinMaxGradientBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_colorOverLifetimeBlittable_Injected(ref ParticleSystem.TrailModule _unity_self, [In] ref ParticleSystem.MinMaxGradientBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_widthOverTrailBlittable_Injected(ref ParticleSystem.TrailModule _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_widthOverTrailBlittable_Injected(ref ParticleSystem.TrailModule _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void get_colorOverTrailBlittable_Injected(ref ParticleSystem.TrailModule _unity_self, out ParticleSystem.MinMaxGradientBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void set_colorOverTrailBlittable_Injected(ref ParticleSystem.TrailModule _unity_self, [In] ref ParticleSystem.MinMaxGradientBlittable value);

			internal ParticleSystem m_ParticleSystem;
		}

		public struct CustomDataModule
		{
			internal CustomDataModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public extern bool enabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[NativeThrows]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern void SetMode(ParticleSystemCustomData stream, ParticleSystemCustomDataMode mode);

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern ParticleSystemCustomDataMode GetMode(ParticleSystemCustomData stream);

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern void SetVectorComponentCount(ParticleSystemCustomData stream, int count);

			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern int GetVectorComponentCount(ParticleSystemCustomData stream);

			public void SetVector(ParticleSystemCustomData stream, int component, ParticleSystem.MinMaxCurve curve)
			{
				this.SetVectorInternal(stream, component, ParticleSystem.MinMaxCurveBlittable.FromMixMaxCurve(in curve));
			}

			[NativeThrows]
			private void SetVectorInternal(ParticleSystemCustomData stream, int component, ParticleSystem.MinMaxCurveBlittable curve)
			{
				ParticleSystem.CustomDataModule.SetVectorInternal_Injected(ref this, stream, component, ref curve);
			}

			public ParticleSystem.MinMaxCurve GetVector(ParticleSystemCustomData stream, int component)
			{
				ParticleSystem.MinMaxCurveBlittable vectorInternal = this.GetVectorInternal(stream, component);
				return ParticleSystem.MinMaxCurveBlittable.ToMinMaxCurve(in vectorInternal);
			}

			[NativeThrows]
			private ParticleSystem.MinMaxCurveBlittable GetVectorInternal(ParticleSystemCustomData stream, int component)
			{
				ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
				ParticleSystem.CustomDataModule.GetVectorInternal_Injected(ref this, stream, component, out minMaxCurveBlittable);
				return minMaxCurveBlittable;
			}

			public void SetColor(ParticleSystemCustomData stream, ParticleSystem.MinMaxGradient gradient)
			{
				this.SetColorInternal(stream, ParticleSystem.MinMaxGradientBlittable.FromMixMaxGradient(in gradient));
			}

			[NativeThrows]
			private void SetColorInternal(ParticleSystemCustomData stream, ParticleSystem.MinMaxGradientBlittable gradient)
			{
				ParticleSystem.CustomDataModule.SetColorInternal_Injected(ref this, stream, ref gradient);
			}

			public ParticleSystem.MinMaxGradient GetColor(ParticleSystemCustomData stream)
			{
				ParticleSystem.MinMaxGradientBlittable colorInternal = this.GetColorInternal(stream);
				return ParticleSystem.MinMaxGradientBlittable.ToMinMaxGradient(in colorInternal);
			}

			[NativeThrows]
			private ParticleSystem.MinMaxGradientBlittable GetColorInternal(ParticleSystemCustomData stream)
			{
				ParticleSystem.MinMaxGradientBlittable minMaxGradientBlittable;
				ParticleSystem.CustomDataModule.GetColorInternal_Injected(ref this, stream, out minMaxGradientBlittable);
				return minMaxGradientBlittable;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetVectorInternal_Injected(ref ParticleSystem.CustomDataModule _unity_self, ParticleSystemCustomData stream, int component, [In] ref ParticleSystem.MinMaxCurveBlittable curve);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetVectorInternal_Injected(ref ParticleSystem.CustomDataModule _unity_self, ParticleSystemCustomData stream, int component, out ParticleSystem.MinMaxCurveBlittable ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColorInternal_Injected(ref ParticleSystem.CustomDataModule _unity_self, ParticleSystemCustomData stream, [In] ref ParticleSystem.MinMaxGradientBlittable gradient);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColorInternal_Injected(ref ParticleSystem.CustomDataModule _unity_self, ParticleSystemCustomData stream, out ParticleSystem.MinMaxGradientBlittable ret);

			internal ParticleSystem m_ParticleSystem;
		}
	}
}
