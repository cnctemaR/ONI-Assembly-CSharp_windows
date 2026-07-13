using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IntegerTime;
using UnityEngine.Bindings;

namespace UnityEngine.Audio
{
	[NativeHeader("Modules/Audio/Public/AudioRandomContainer.h")]
	[HelpURL("AudioRandomContainer-UI")]
	[ExcludeFromPreset]
	internal sealed class AudioRandomContainer : AudioResource, IAudioGenerator, GeneratorInstance.ICapabilities
	{
		internal AudioRandomContainer()
		{
			AudioRandomContainer.Internal_Create(this);
		}

		internal float volume
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_volume_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_volume_Injected(intPtr, value);
			}
		}

		internal Vector2 volumeRandomizationRange
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				AudioRandomContainer.get_volumeRandomizationRange_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_volumeRandomizationRange_Injected(intPtr, ref value);
			}
		}

		internal bool volumeRandomizationEnabled
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_volumeRandomizationEnabled_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_volumeRandomizationEnabled_Injected(intPtr, value);
			}
		}

		internal float pitch
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_pitch_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_pitch_Injected(intPtr, value);
			}
		}

		internal Vector2 pitchRandomizationRange
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				AudioRandomContainer.get_pitchRandomizationRange_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_pitchRandomizationRange_Injected(intPtr, ref value);
			}
		}

		internal bool pitchRandomizationEnabled
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_pitchRandomizationEnabled_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_pitchRandomizationEnabled_Injected(intPtr, value);
			}
		}

		internal AudioContainerElement[] elements
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_elements_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_elements_Injected(intPtr, value);
			}
		}

		internal AudioRandomContainerTriggerMode triggerMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_triggerMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_triggerMode_Injected(intPtr, value);
			}
		}

		internal AudioRandomContainerPlaybackMode playbackMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_playbackMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_playbackMode_Injected(intPtr, value);
			}
		}

		internal int avoidRepeatingLast
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_avoidRepeatingLast_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_avoidRepeatingLast_Injected(intPtr, value);
			}
		}

		internal AudioRandomContainerAutomaticTriggerMode automaticTriggerMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_automaticTriggerMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_automaticTriggerMode_Injected(intPtr, value);
			}
		}

		internal float automaticTriggerTime
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_automaticTriggerTime_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_automaticTriggerTime_Injected(intPtr, value);
			}
		}

		internal Vector2 automaticTriggerTimeRandomizationRange
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				AudioRandomContainer.get_automaticTriggerTimeRandomizationRange_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_automaticTriggerTimeRandomizationRange_Injected(intPtr, ref value);
			}
		}

		internal bool automaticTriggerTimeRandomizationEnabled
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_automaticTriggerTimeRandomizationEnabled_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_automaticTriggerTimeRandomizationEnabled_Injected(intPtr, value);
			}
		}

		internal AudioRandomContainerLoopMode loopMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_loopMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_loopMode_Injected(intPtr, value);
			}
		}

		internal int loopCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_loopCount_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_loopCount_Injected(intPtr, value);
			}
		}

		internal Vector2 loopCountRandomizationRange
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				AudioRandomContainer.get_loopCountRandomizationRange_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_loopCountRandomizationRange_Injected(intPtr, ref value);
			}
		}

		internal bool loopCountRandomizationEnabled
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioRandomContainer.get_loopCountRandomizationEnabled_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioRandomContainer.set_loopCountRandomizationEnabled_Injected(intPtr, value);
			}
		}

		internal void NotifyObservers(AudioRandomContainer.ChangeEventType eventType)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioRandomContainer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AudioRandomContainer.NotifyObservers_Injected(intPtr, eventType);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] AudioRandomContainer self);

		bool GeneratorInstance.ICapabilities.isFinite
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool GeneratorInstance.ICapabilities.isRealtime
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

		GeneratorInstance IAudioGenerator.CreateInstance(ControlContext context, AudioFormat? nestedFormat, ProcessorInstance.CreationParameters creationParameters)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_volume_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_volume_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_volumeRandomizationRange_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_volumeRandomizationRange_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_volumeRandomizationEnabled_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_volumeRandomizationEnabled_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_pitch_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_pitch_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_pitchRandomizationRange_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_pitchRandomizationRange_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_pitchRandomizationEnabled_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_pitchRandomizationEnabled_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioContainerElement[] get_elements_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_elements_Injected(IntPtr _unity_self, AudioContainerElement[] value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioRandomContainerTriggerMode get_triggerMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_triggerMode_Injected(IntPtr _unity_self, AudioRandomContainerTriggerMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioRandomContainerPlaybackMode get_playbackMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_playbackMode_Injected(IntPtr _unity_self, AudioRandomContainerPlaybackMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_avoidRepeatingLast_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_avoidRepeatingLast_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioRandomContainerAutomaticTriggerMode get_automaticTriggerMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_automaticTriggerMode_Injected(IntPtr _unity_self, AudioRandomContainerAutomaticTriggerMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_automaticTriggerTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_automaticTriggerTime_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_automaticTriggerTimeRandomizationRange_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_automaticTriggerTimeRandomizationRange_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_automaticTriggerTimeRandomizationEnabled_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_automaticTriggerTimeRandomizationEnabled_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioRandomContainerLoopMode get_loopMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_loopMode_Injected(IntPtr _unity_self, AudioRandomContainerLoopMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_loopCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_loopCount_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_loopCountRandomizationRange_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_loopCountRandomizationRange_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_loopCountRandomizationEnabled_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_loopCountRandomizationEnabled_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void NotifyObservers_Injected(IntPtr _unity_self, AudioRandomContainer.ChangeEventType eventType);

		internal enum ChangeEventType
		{
			Volume,
			Pitch,
			List
		}
	}
}
