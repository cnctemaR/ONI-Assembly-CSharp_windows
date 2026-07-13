using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Audio;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[StaticAccessor("AudioSourceBindings", StaticAccessorType.DoubleColon)]
	[RequireComponent(typeof(Transform))]
	public sealed class AudioSource : AudioBehaviour
	{
		private static float GetPitch([NotNull] AudioSource source)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(source);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			return AudioSource.GetPitch_Injected(intPtr);
		}

		private static void SetPitch([NotNull] AudioSource source, float pitch)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(source);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			AudioSource.SetPitch_Injected(intPtr, pitch);
		}

		private static void PlayHelper([NotNull] AudioSource source, ulong delay)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(source);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			AudioSource.PlayHelper_Injected(intPtr, delay);
		}

		private void Play(double delay)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AudioSource.Play_Injected(intPtr, delay);
		}

		private static void PlayOneShotHelper([NotNull] AudioSource source, [NotNull] AudioClip clip, float volumeScale)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			if (clip == null)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(source);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(clip);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			AudioSource.PlayOneShotHelper_Injected(intPtr, intPtr2, volumeScale);
		}

		private void Stop(bool stopOneShots)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AudioSource.Stop_Injected(intPtr, stopOneShots);
		}

		[NativeThrows]
		private static void SetCustomCurveHelper([NotNull] AudioSource source, AudioSourceCurveType type, AnimationCurve curve)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(source);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			AudioSource.SetCustomCurveHelper_Injected(intPtr, type, (curve == null) ? ((IntPtr)0) : AnimationCurve.BindingsMarshaller.ConvertToNative(curve));
		}

		private static AnimationCurve GetCustomCurveHelper([NotNull] AudioSource source, AudioSourceCurveType type)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(source);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			IntPtr customCurveHelper_Injected = AudioSource.GetCustomCurveHelper_Injected(intPtr, type);
			return (customCurveHelper_Injected == 0) ? null : AnimationCurve.BindingsMarshaller.ConvertToManaged(customCurveHelper_Injected);
		}

		private unsafe static void GetOutputDataHelper([NotNull] AudioSource source, [Out] float[] samples, int channel)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(source);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(source, "source");
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (samples != null)
				{
					fixed (float[] array = samples)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				AudioSource.GetOutputDataHelper_Injected(intPtr, out blittableArrayWrapper, channel);
			}
			finally
			{
				float[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<float>(ref array);
			}
		}

		[NativeThrows]
		private unsafe static void GetSpectrumDataHelper([NotNull] AudioSource source, [Out] float[] samples, int channel, FFTWindow window)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(source);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(source, "source");
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (samples != null)
				{
					fixed (float[] array = samples)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				AudioSource.GetSpectrumDataHelper_Injected(intPtr, out blittableArrayWrapper, channel, window);
			}
			finally
			{
				float[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<float>(ref array);
			}
		}

		public float volume
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_volume_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_volume_Injected(intPtr, value);
			}
		}

		public float pitch
		{
			get
			{
				return AudioSource.GetPitch(this);
			}
			set
			{
				AudioSource.SetPitch(this, value);
			}
		}

		[NativeProperty("SecPosition")]
		public float time
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_time_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_time_Injected(intPtr, value);
			}
		}

		[NativeProperty("SamplePosition")]
		public int timeSamples
		{
			[NativeMethod(IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_timeSamples_Injected(intPtr);
			}
			[NativeMethod(IsThreadSafe = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_timeSamples_Injected(intPtr, value);
			}
		}

		public AudioClip clip
		{
			get
			{
				return this.generatorObject as AudioClip;
			}
			set
			{
				this.generatorObject = value;
			}
		}

		public AudioResource resource
		{
			get
			{
				return this.generatorObject as AudioResource;
			}
			set
			{
				this.generatorObject = value;
			}
		}

		public IAudioGenerator generator
		{
			get
			{
				return (IAudioGenerator)this.generatorObject;
			}
			set
			{
				this.generatorObject = (Object)value;
			}
		}

		public unsafe ProcessorInstance generatorInstance
		{
			get
			{
				GeneratorInstance.GeneratorHeader* generatorHeader = (GeneratorInstance.GeneratorHeader*)this.generatorHeader;
				bool flag = generatorHeader != null;
				ProcessorInstance processorInstance;
				if (flag)
				{
					GeneratorInstance generatorInstance = new GeneratorInstance(generatorHeader);
					processorInstance = in generatorInstance;
				}
				else
				{
					processorInstance = default(ProcessorInstance);
				}
				return processorInstance;
			}
		}

		internal unsafe void* generatorHeader
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_generatorHeader_Injected(intPtr);
			}
		}

		internal Object generatorObject
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Object>(AudioSource.get_generatorObject_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_generatorObject_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Object>(value));
			}
		}

		public AudioMixerGroup outputAudioMixerGroup
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<AudioMixerGroup>(AudioSource.get_outputAudioMixerGroup_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_outputAudioMixerGroup_Injected(intPtr, Object.MarshalledUnityObject.Marshal<AudioMixerGroup>(value));
			}
		}

		[ExcludeFromDocs]
		public void Play()
		{
			AudioSource.PlayHelper(this, 0UL);
		}

		public void Play([DefaultValue("0")] ulong delay)
		{
			AudioSource.PlayHelper(this, delay);
		}

		public void PlayDelayed(float delay)
		{
			this.Play((delay < 0f) ? 0.0 : (-(double)delay));
		}

		public void PlayScheduled(double time)
		{
			this.Play((time < 0.0) ? 0.0 : time);
		}

		[ExcludeFromDocs]
		public void PlayOneShot(AudioClip clip)
		{
			this.PlayOneShot(clip, 1f);
		}

		public void PlayOneShot(AudioClip clip, [DefaultValue("1.0F")] float volumeScale)
		{
			bool flag = clip == null;
			if (flag)
			{
				Debug.LogWarning("PlayOneShot was called with a null AudioClip.");
			}
			else
			{
				AudioSource.PlayOneShotHelper(this, clip, volumeScale);
			}
		}

		public void SetScheduledStartTime(double time)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AudioSource.SetScheduledStartTime_Injected(intPtr, time);
		}

		public void SetScheduledEndTime(double time)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AudioSource.SetScheduledEndTime_Injected(intPtr, time);
		}

		public void Stop()
		{
			this.Stop(true);
		}

		public void Pause()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AudioSource.Pause_Injected(intPtr);
		}

		public void UnPause()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AudioSource.UnPause_Injected(intPtr);
		}

		internal void SkipToNextElementIfHasContainer()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AudioSource.SkipToNextElementIfHasContainer_Injected(intPtr);
		}

		public bool isPlaying
		{
			[NativeName("IsPlayingScripting")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_isPlaying_Injected(intPtr);
			}
		}

		internal bool isContainerPlaying
		{
			[NativeName("IsContainerPlaying")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_isContainerPlaying_Injected(intPtr);
			}
		}

		internal ActivePlayable[] containerActivePlayables
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_containerActivePlayables_Injected(intPtr);
			}
		}

		public bool isVirtual
		{
			[NativeName("GetLastVirtualState")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_isVirtual_Injected(intPtr);
			}
		}

		[ExcludeFromDocs]
		public static void PlayClipAtPoint(AudioClip clip, Vector3 position)
		{
			AudioSource.PlayClipAtPoint(clip, position, 1f);
		}

		public static void PlayClipAtPoint(AudioClip clip, Vector3 position, [DefaultValue("1.0F")] float volume)
		{
			GameObject gameObject = new GameObject("One shot audio");
			gameObject.transform.position = position;
			AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
			audioSource.clip = clip;
			audioSource.spatialBlend = 1f;
			audioSource.volume = volume;
			audioSource.Play();
			Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
		}

		public bool loop
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_loop_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_loop_Injected(intPtr, value);
			}
		}

		public bool ignoreListenerVolume
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_ignoreListenerVolume_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_ignoreListenerVolume_Injected(intPtr, value);
			}
		}

		public bool playOnAwake
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_playOnAwake_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_playOnAwake_Injected(intPtr, value);
			}
		}

		public bool ignoreListenerPause
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_ignoreListenerPause_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_ignoreListenerPause_Injected(intPtr, value);
			}
		}

		public AudioVelocityUpdateMode velocityUpdateMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_velocityUpdateMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_velocityUpdateMode_Injected(intPtr, value);
			}
		}

		[NativeProperty("StereoPan")]
		public float panStereo
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_panStereo_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_panStereo_Injected(intPtr, value);
			}
		}

		[NativeProperty("SpatialBlendMix")]
		public float spatialBlend
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_spatialBlend_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_spatialBlend_Injected(intPtr, value);
			}
		}

		public bool spatialize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_spatialize_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_spatialize_Injected(intPtr, value);
			}
		}

		public bool spatializePostEffects
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_spatializePostEffects_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_spatializePostEffects_Injected(intPtr, value);
			}
		}

		public void SetCustomCurve(AudioSourceCurveType type, AnimationCurve curve)
		{
			AudioSource.SetCustomCurveHelper(this, type, curve);
		}

		public AnimationCurve GetCustomCurve(AudioSourceCurveType type)
		{
			return AudioSource.GetCustomCurveHelper(this, type);
		}

		public float reverbZoneMix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_reverbZoneMix_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_reverbZoneMix_Injected(intPtr, value);
			}
		}

		public bool bypassEffects
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_bypassEffects_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_bypassEffects_Injected(intPtr, value);
			}
		}

		public bool bypassListenerEffects
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_bypassListenerEffects_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_bypassListenerEffects_Injected(intPtr, value);
			}
		}

		public bool bypassReverbZones
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_bypassReverbZones_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_bypassReverbZones_Injected(intPtr, value);
			}
		}

		public float dopplerLevel
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_dopplerLevel_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_dopplerLevel_Injected(intPtr, value);
			}
		}

		public float spread
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_spread_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_spread_Injected(intPtr, value);
			}
		}

		public int priority
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_priority_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_priority_Injected(intPtr, value);
			}
		}

		public bool mute
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_mute_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_mute_Injected(intPtr, value);
			}
		}

		public float minDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_minDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_minDistance_Injected(intPtr, value);
			}
		}

		public float maxDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_maxDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_maxDistance_Injected(intPtr, value);
			}
		}

		public AudioRolloffMode rolloffMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioSource.get_rolloffMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioSource.set_rolloffMode_Injected(intPtr, value);
			}
		}

		[Obsolete("GetOutputData returning a float[] is deprecated, use GetOutputData and pass a pre allocated array instead.")]
		public float[] GetOutputData(int numSamples, int channel)
		{
			float[] array = new float[numSamples];
			AudioSource.GetOutputDataHelper(this, array, channel);
			return array;
		}

		public void GetOutputData(float[] samples, int channel)
		{
			AudioSource.GetOutputDataHelper(this, samples, channel);
		}

		[Obsolete("GetSpectrumData returning a float[] is deprecated, use GetSpectrumData and pass a pre allocated array instead.")]
		public float[] GetSpectrumData(int numSamples, int channel, FFTWindow window)
		{
			float[] array = new float[numSamples];
			AudioSource.GetSpectrumDataHelper(this, array, channel, window);
			return array;
		}

		public void GetSpectrumData(float[] samples, int channel, FFTWindow window)
		{
			AudioSource.GetSpectrumDataHelper(this, samples, channel, window);
		}

		[Obsolete("minVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
		public float minVolume
		{
			get
			{
				Debug.LogError("minVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.");
				return 0f;
			}
			set
			{
				Debug.LogError("minVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.");
			}
		}

		[Obsolete("maxVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
		public float maxVolume
		{
			get
			{
				Debug.LogError("maxVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.");
				return 0f;
			}
			set
			{
				Debug.LogError("maxVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.");
			}
		}

		[Obsolete("rolloffFactor is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
		public float rolloffFactor
		{
			get
			{
				Debug.LogError("rolloffFactor is not supported anymore. Use min-, maxDistance and rolloffMode instead.");
				return 0f;
			}
			set
			{
				Debug.LogError("rolloffFactor is not supported anymore. Use min-, maxDistance and rolloffMode instead.");
			}
		}

		public bool SetSpatializerFloat(int index, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AudioSource.SetSpatializerFloat_Injected(intPtr, index, value);
		}

		public bool GetSpatializerFloat(int index, out float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AudioSource.GetSpatializerFloat_Injected(intPtr, index, out value);
		}

		public bool GetAmbisonicDecoderFloat(int index, out float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AudioSource.GetAmbisonicDecoderFloat_Injected(intPtr, index, out value);
		}

		public bool SetAmbisonicDecoderFloat(int index, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AudioSource.SetAmbisonicDecoderFloat_Injected(intPtr, index, value);
		}

		internal float GetAudioRandomContainerRuntimeMeterValue()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioSource>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AudioSource.GetAudioRandomContainerRuntimeMeterValue_Injected(intPtr);
		}

		[Obsolete("AudioSource.generatorDefinition has been deprecated. Use AudioSource.generator instead. (UnityUpgradable) -> generator", true)]
		public IAudioGenerator generatorDefinition
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[Obsolete("AudioSource.generatorHandle has been deprecated. Use AudioSource.generatorInstance instead. (UnityUpgradable) -> generatorInstance", true)]
		public ProcessorInstance generatorHandle
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetPitch_Injected(IntPtr source);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPitch_Injected(IntPtr source, float pitch);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PlayHelper_Injected(IntPtr source, ulong delay);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Play_Injected(IntPtr _unity_self, double delay);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PlayOneShotHelper_Injected(IntPtr source, IntPtr clip, float volumeScale);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop_Injected(IntPtr _unity_self, bool stopOneShots);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetCustomCurveHelper_Injected(IntPtr source, AudioSourceCurveType type, IntPtr curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetCustomCurveHelper_Injected(IntPtr source, AudioSourceCurveType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetOutputDataHelper_Injected(IntPtr source, out BlittableArrayWrapper samples, int channel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSpectrumDataHelper_Injected(IntPtr source, out BlittableArrayWrapper samples, int channel, FFTWindow window);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_volume_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_volume_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_time_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_time_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_timeSamples_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_timeSamples_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* get_generatorHeader_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_generatorObject_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_generatorObject_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_outputAudioMixerGroup_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_outputAudioMixerGroup_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetScheduledStartTime_Injected(IntPtr _unity_self, double time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetScheduledEndTime_Injected(IntPtr _unity_self, double time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Pause_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnPause_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SkipToNextElementIfHasContainer_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isPlaying_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isContainerPlaying_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ActivePlayable[] get_containerActivePlayables_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isVirtual_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_loop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_loop_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_ignoreListenerVolume_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_ignoreListenerVolume_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_playOnAwake_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_playOnAwake_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_ignoreListenerPause_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_ignoreListenerPause_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioVelocityUpdateMode get_velocityUpdateMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_velocityUpdateMode_Injected(IntPtr _unity_self, AudioVelocityUpdateMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_panStereo_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_panStereo_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_spatialBlend_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_spatialBlend_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_spatialize_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_spatialize_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_spatializePostEffects_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_spatializePostEffects_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_reverbZoneMix_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_reverbZoneMix_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_bypassEffects_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bypassEffects_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_bypassListenerEffects_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bypassListenerEffects_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_bypassReverbZones_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bypassReverbZones_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_dopplerLevel_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_dopplerLevel_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_spread_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_spread_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_priority_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_priority_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_mute_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_mute_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_minDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_minDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_maxDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioRolloffMode get_rolloffMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rolloffMode_Injected(IntPtr _unity_self, AudioRolloffMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetSpatializerFloat_Injected(IntPtr _unity_self, int index, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetSpatializerFloat_Injected(IntPtr _unity_self, int index, out float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetAmbisonicDecoderFloat_Injected(IntPtr _unity_self, int index, out float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetAmbisonicDecoderFloat_Injected(IntPtr _unity_self, int index, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetAudioRandomContainerRuntimeMeterValue_Injected(IntPtr _unity_self);
	}
}
