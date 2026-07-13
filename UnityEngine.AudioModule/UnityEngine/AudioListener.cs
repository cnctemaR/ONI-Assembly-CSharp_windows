using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[RequireComponent(typeof(Transform))]
	[StaticAccessor("AudioListenerBindings", StaticAccessorType.DoubleColon)]
	public sealed class AudioListener : AudioBehaviour
	{
		[NativeThrows]
		private unsafe static void GetOutputDataHelper([Out] float[] samples, int channel)
		{
			try
			{
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
				AudioListener.GetOutputDataHelper_Injected(out blittableArrayWrapper, channel);
			}
			finally
			{
				float[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<float>(ref array);
			}
		}

		[NativeThrows]
		private unsafe static void GetSpectrumDataHelper([Out] float[] samples, int channel, FFTWindow window)
		{
			try
			{
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
				AudioListener.GetSpectrumDataHelper_Injected(out blittableArrayWrapper, channel, window);
			}
			finally
			{
				float[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<float>(ref array);
			}
		}

		public static extern float volume
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("ListenerPause")]
		public static extern bool pause
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public AudioVelocityUpdateMode velocityUpdateMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioListener>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioListener.get_velocityUpdateMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioListener>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioListener.set_velocityUpdateMode_Injected(intPtr, value);
			}
		}

		[Obsolete("GetOutputData returning a float[] is deprecated, use GetOutputData and pass a pre allocated array instead.")]
		public static float[] GetOutputData(int numSamples, int channel)
		{
			float[] array = new float[numSamples];
			AudioListener.GetOutputDataHelper(array, channel);
			return array;
		}

		public static void GetOutputData(float[] samples, int channel)
		{
			AudioListener.GetOutputDataHelper(samples, channel);
		}

		[Obsolete("GetSpectrumData returning a float[] is deprecated, use GetSpectrumData and pass a pre allocated array instead.")]
		public static float[] GetSpectrumData(int numSamples, int channel, FFTWindow window)
		{
			float[] array = new float[numSamples];
			AudioListener.GetSpectrumDataHelper(array, channel, window);
			return array;
		}

		public static void GetSpectrumData(float[] samples, int channel, FFTWindow window)
		{
			AudioListener.GetSpectrumDataHelper(samples, channel, window);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetOutputDataHelper_Injected(out BlittableArrayWrapper samples, int channel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSpectrumDataHelper_Injected(out BlittableArrayWrapper samples, int channel, FFTWindow window);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioVelocityUpdateMode get_velocityUpdateMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_velocityUpdateMode_Injected(IntPtr _unity_self, AudioVelocityUpdateMode value);
	}
}
