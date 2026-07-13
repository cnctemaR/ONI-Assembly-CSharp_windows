using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[RequireComponent(typeof(AudioBehaviour))]
	public sealed class AudioLowPassFilter : Behaviour
	{
		private AnimationCurve GetCustomLowpassLevelCurveCopy()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioLowPassFilter>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr customLowpassLevelCurveCopy_Injected = AudioLowPassFilter.GetCustomLowpassLevelCurveCopy_Injected(intPtr);
			return (customLowpassLevelCurveCopy_Injected == 0) ? null : AnimationCurve.BindingsMarshaller.ConvertToManaged(customLowpassLevelCurveCopy_Injected);
		}

		[NativeThrows]
		[NativeMethod(Name = "AudioLowPassFilterBindings::SetCustomLowpassLevelCurveHelper", IsFreeFunction = true)]
		private static void SetCustomLowpassLevelCurveHelper([NotNull] AudioLowPassFilter source, AnimationCurve curve)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioLowPassFilter>(source);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			AudioLowPassFilter.SetCustomLowpassLevelCurveHelper_Injected(intPtr, (curve == null) ? ((IntPtr)0) : AnimationCurve.BindingsMarshaller.ConvertToNative(curve));
		}

		public AnimationCurve customCutoffCurve
		{
			get
			{
				return this.GetCustomLowpassLevelCurveCopy();
			}
			set
			{
				AudioLowPassFilter.SetCustomLowpassLevelCurveHelper(this, value);
			}
		}

		public float cutoffFrequency
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioLowPassFilter>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioLowPassFilter.get_cutoffFrequency_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioLowPassFilter>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioLowPassFilter.set_cutoffFrequency_Injected(intPtr, value);
			}
		}

		public float lowpassResonanceQ
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioLowPassFilter>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioLowPassFilter.get_lowpassResonanceQ_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioLowPassFilter>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioLowPassFilter.set_lowpassResonanceQ_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetCustomLowpassLevelCurveCopy_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetCustomLowpassLevelCurveHelper_Injected(IntPtr source, IntPtr curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_cutoffFrequency_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cutoffFrequency_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_lowpassResonanceQ_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lowpassResonanceQ_Injected(IntPtr _unity_self, float value);
	}
}
