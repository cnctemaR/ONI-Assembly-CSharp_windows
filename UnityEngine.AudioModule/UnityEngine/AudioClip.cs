using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IntegerTime;
using UnityEngine.Audio;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[StaticAccessor("AudioClipBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/Audio/Public/ScriptBindings/Audio.bindings.h")]
	public sealed class AudioClip : AudioResource, IAudioGenerator, GeneratorInstance.ICapabilities
	{
		private AudioClip()
		{
		}

		private unsafe static bool GetData([NotNull] AudioClip clip, Span<float> data, int samplesOffset)
		{
			if (clip == null)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(clip);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			Span<float> span = data;
			bool data_Injected;
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				data_Injected = AudioClip.GetData_Injected(intPtr, ref managedSpanWrapper, samplesOffset);
			}
			return data_Injected;
		}

		private unsafe static bool SetData([NotNull] AudioClip clip, ReadOnlySpan<float> data, int samplesOffset)
		{
			if (clip == null)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(clip);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			ReadOnlySpan<float> readOnlySpan = data;
			bool flag;
			fixed (float* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				flag = AudioClip.SetData_Injected(intPtr, ref managedSpanWrapper, samplesOffset);
			}
			return flag;
		}

		private static AudioClip Construct_Internal()
		{
			return Unmarshal.UnmarshalUnityObject<AudioClip>(AudioClip.Construct_Internal_Injected());
		}

		private string GetName()
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				AudioClip.GetName_Injected(intPtr, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		private unsafe void CreateUserSound(string name, int lengthSamples, int channels, int frequency, bool stream)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AudioClip.CreateUserSound_Injected(intPtr, ref managedSpanWrapper, lengthSamples, channels, frequency, stream);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeProperty("LengthSec")]
		public float length
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioClip.get_length_Injected(intPtr);
			}
		}

		[NativeProperty("SampleCount")]
		public int samples
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioClip.get_samples_Injected(intPtr);
			}
		}

		[NativeProperty("ChannelCount")]
		public int channels
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioClip.get_channels_Injected(intPtr);
			}
		}

		public int frequency
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioClip.get_frequency_Injected(intPtr);
			}
		}

		[Obsolete("Use AudioClip.loadState instead to get more detailed information about the loading process.")]
		public bool isReadyToPlay
		{
			[NativeName("ReadyToPlay")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioClip.get_isReadyToPlay_Injected(intPtr);
			}
		}

		public AudioClipLoadType loadType
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioClip.get_loadType_Injected(intPtr);
			}
		}

		public bool LoadAudioData()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AudioClip.LoadAudioData_Injected(intPtr);
		}

		public bool UnloadAudioData()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AudioClip.UnloadAudioData_Injected(intPtr);
		}

		public bool preloadAudioData
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioClip.get_preloadAudioData_Injected(intPtr);
			}
		}

		public bool ambisonic
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioClip.get_ambisonic_Injected(intPtr);
			}
		}

		public bool loadInBackground
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioClip.get_loadInBackground_Injected(intPtr);
			}
		}

		public AudioDataLoadState loadState
		{
			[NativeMethod(Name = "AudioClipBindings::GetLoadState", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioClip.get_loadState_Injected(intPtr);
			}
		}

		public bool GetData(Span<float> data, int offsetSamples)
		{
			bool flag = this.channels <= 0;
			bool flag2;
			if (flag)
			{
				Debug.Log("AudioClip.GetData failed; AudioClip " + this.GetName() + " contains no data");
				flag2 = false;
			}
			else
			{
				flag2 = AudioClip.GetData(this, data, offsetSamples);
			}
			return flag2;
		}

		public bool GetData(float[] data, int offsetSamples)
		{
			bool flag = this.channels <= 0;
			bool flag2;
			if (flag)
			{
				Debug.Log("AudioClip.GetData failed; AudioClip " + this.GetName() + " contains no data");
				flag2 = false;
			}
			else
			{
				flag2 = AudioClip.GetData(this, data.AsSpan<float>(), offsetSamples);
			}
			return flag2;
		}

		public bool SetData(float[] data, int offsetSamples)
		{
			bool flag = this.channels <= 0;
			bool flag2;
			if (flag)
			{
				Debug.Log("AudioClip.SetData failed; AudioClip " + this.GetName() + " contains no data");
				flag2 = false;
			}
			else
			{
				bool flag3 = offsetSamples < 0 || offsetSamples >= this.samples;
				if (flag3)
				{
					throw new ArgumentException("AudioClip.SetData failed; invalid offsetSamples");
				}
				bool flag4 = data == null || data.Length == 0;
				if (flag4)
				{
					throw new ArgumentException("AudioClip.SetData failed; invalid data");
				}
				flag2 = AudioClip.SetData(this, data.AsSpan<float>(), offsetSamples);
			}
			return flag2;
		}

		public bool SetData(ReadOnlySpan<float> data, int offsetSamples)
		{
			bool flag = this.channels <= 0;
			bool flag2;
			if (flag)
			{
				Debug.Log("AudioClip.SetData failed; AudioClip " + this.GetName() + " contains no data");
				flag2 = false;
			}
			else
			{
				bool flag3 = offsetSamples < 0 || offsetSamples >= this.samples;
				if (flag3)
				{
					throw new ArgumentException("AudioClip.SetData failed; invalid offsetSamples");
				}
				bool flag4 = data.Length == 0;
				if (flag4)
				{
					throw new ArgumentException("AudioClip.SetData failed; invalid data");
				}
				flag2 = AudioClip.SetData(this, data, offsetSamples);
			}
			return flag2;
		}

		[Obsolete("The _3D argument of AudioClip is deprecated. Use the spatialBlend property of AudioSource instead to morph between 2D and 3D playback.")]
		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream)
		{
			return AudioClip.Create(name, lengthSamples, channels, frequency, stream);
		}

		[Obsolete("The _3D argument of AudioClip is deprecated. Use the spatialBlend property of AudioSource instead to morph between 2D and 3D playback.")]
		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream, AudioClip.PCMReaderCallback pcmreadercallback)
		{
			return AudioClip.Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, null);
		}

		[Obsolete("The _3D argument of AudioClip is deprecated. Use the spatialBlend property of AudioSource instead to morph between 2D and 3D playback.")]
		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream, AudioClip.PCMReaderCallback pcmreadercallback, AudioClip.PCMSetPositionCallback pcmsetpositioncallback)
		{
			return AudioClip.Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, pcmsetpositioncallback);
		}

		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream)
		{
			return AudioClip.Create(name, lengthSamples, channels, frequency, stream, null, null);
		}

		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream, AudioClip.PCMReaderCallback pcmreadercallback)
		{
			return AudioClip.Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, null);
		}

		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream, AudioClip.PCMReaderCallback pcmreadercallback, AudioClip.PCMSetPositionCallback pcmsetpositioncallback)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new NullReferenceException();
			}
			bool flag2 = lengthSamples <= 0;
			if (flag2)
			{
				throw new ArgumentException("Length of created clip must be larger than 0");
			}
			bool flag3 = channels <= 0;
			if (flag3)
			{
				throw new ArgumentException("Number of channels in created clip must be greater than 0");
			}
			bool flag4 = frequency <= 0;
			if (flag4)
			{
				throw new ArgumentException("Frequency in created clip must be greater than 0");
			}
			AudioClip audioClip = AudioClip.Construct_Internal();
			bool flag5 = pcmreadercallback != null;
			if (flag5)
			{
				audioClip.m_PCMReaderCallback += pcmreadercallback;
			}
			bool flag6 = pcmsetpositioncallback != null;
			if (flag6)
			{
				audioClip.m_PCMSetPositionCallback += pcmsetpositioncallback;
			}
			audioClip.CreateUserSound(name, lengthSamples, channels, frequency, stream);
			return audioClip;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event AudioClip.PCMReaderCallback m_PCMReaderCallback = null;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event AudioClip.PCMSetPositionCallback m_PCMSetPositionCallback = null;

		[RequiredByNativeCode]
		private void InvokePCMReaderCallback_Internal(float[] data)
		{
			bool flag = this.m_PCMReaderCallback != null;
			if (flag)
			{
				this.m_PCMReaderCallback(data);
			}
		}

		[RequiredByNativeCode]
		private void InvokePCMSetPositionCallback_Internal(int position)
		{
			bool flag = this.m_PCMSetPositionCallback != null;
			if (flag)
			{
				this.m_PCMSetPositionCallback(position);
			}
		}

		bool GeneratorInstance.ICapabilities.isRealtime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool GeneratorInstance.ICapabilities.isFinite
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		DiscreteTime? GeneratorInstance.ICapabilities.length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		GeneratorInstance IAudioGenerator.CreateInstance(ControlContext context, AudioFormat? nestedFormat, ProcessorInstance.CreationParameters parameters)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetData_Injected(IntPtr clip, ref ManagedSpanWrapper data, int samplesOffset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetData_Injected(IntPtr clip, ref ManagedSpanWrapper data, int samplesOffset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Construct_Internal_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateUserSound_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, int lengthSamples, int channels, int frequency, bool stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_length_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_samples_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_channels_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_frequency_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isReadyToPlay_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioClipLoadType get_loadType_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool LoadAudioData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UnloadAudioData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_preloadAudioData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_ambisonic_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_loadInBackground_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioDataLoadState get_loadState_Injected(IntPtr _unity_self);

		public delegate void PCMReaderCallback(float[] data);

		public delegate void PCMSetPositionCallback(int position);
	}
}
