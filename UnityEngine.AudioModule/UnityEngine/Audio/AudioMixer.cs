using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Audio
{
	[ExcludeFromObjectFactory]
	[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioMixer.bindings.h")]
	[ExcludeFromPreset]
	[NativeHeader("Modules/Audio/Public/AudioMixer.h")]
	public class AudioMixer : Object
	{
		internal AudioMixer()
		{
		}

		[NativeProperty]
		public AudioMixerGroup outputAudioMixerGroup
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<AudioMixerGroup>(AudioMixer.get_outputAudioMixerGroup_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioMixer.set_outputAudioMixerGroup_Injected(intPtr, Object.MarshalledUnityObject.Marshal<AudioMixerGroup>(value));
			}
		}

		[NativeMethod("FindSnapshotFromName")]
		public unsafe AudioMixerSnapshot FindSnapshot(string name)
		{
			AudioMixerSnapshot audioMixerSnapshot;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixer>(this);
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
				IntPtr intPtr2 = AudioMixer.FindSnapshot_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				IntPtr intPtr2;
				audioMixerSnapshot = Unmarshal.UnmarshalUnityObject<AudioMixerSnapshot>(intPtr2);
				char* ptr = null;
			}
			return audioMixerSnapshot;
		}

		[NativeMethod("AudioMixerBindings::FindMatchingGroups", IsFreeFunction = true, HasExplicitThis = true)]
		public unsafe AudioMixerGroup[] FindMatchingGroups(string subPath)
		{
			AudioMixerGroup[] array;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(subPath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = subPath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				array = AudioMixer.FindMatchingGroups_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return array;
		}

		internal void TransitionToSnapshot(AudioMixerSnapshot snapshot, float timeToReach)
		{
			bool flag = snapshot == null;
			if (flag)
			{
				throw new ArgumentException("null Snapshot passed to AudioMixer.TransitionToSnapshot of AudioMixer '" + base.name + "'");
			}
			bool flag2 = snapshot.audioMixer != this;
			if (flag2)
			{
				throw new ArgumentException(string.Concat(new string[] { "Snapshot '", snapshot.name, "' passed to AudioMixer.TransitionToSnapshot is not a snapshot from AudioMixer '", base.name, "'" }));
			}
			this.TransitionToSnapshotInternal(snapshot, timeToReach);
		}

		[NativeMethod("TransitionToSnapshot")]
		private void TransitionToSnapshotInternal(AudioMixerSnapshot snapshot, float timeToReach)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AudioMixer.TransitionToSnapshotInternal_Injected(intPtr, Object.MarshalledUnityObject.Marshal<AudioMixerSnapshot>(snapshot), timeToReach);
		}

		[NativeMethod("AudioMixerBindings::TransitionToSnapshots", IsFreeFunction = true, HasExplicitThis = true, ThrowsException = true)]
		public unsafe void TransitionToSnapshots(AudioMixerSnapshot[] snapshots, float[] weights, float timeToReach)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(weights);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				AudioMixer.TransitionToSnapshots_Injected(intPtr, snapshots, ref managedSpanWrapper, timeToReach);
			}
		}

		[NativeProperty]
		public AudioMixerUpdateMode updateMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AudioMixer.get_updateMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AudioMixer.set_updateMode_Injected(intPtr, value);
			}
		}

		[NativeMethod]
		public unsafe bool SetFloat(string name, float value)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixer>(this);
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
				flag = AudioMixer.SetFloat_Injected(intPtr, ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeMethod]
		public unsafe bool ClearFloat(string name)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixer>(this);
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
				flag = AudioMixer.ClearFloat_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeMethod]
		public unsafe bool GetFloat(string name, out float value)
		{
			bool float_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixer>(this);
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
				float_Injected = AudioMixer.GetFloat_Injected(intPtr, ref managedSpanWrapper, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return float_Injected;
		}

		[NativeMethod("AudioMixerBindings::GetAbsoluteAudibilityFromGroup", HasExplicitThis = true, IsFreeFunction = true)]
		internal float GetAbsoluteAudibilityFromGroup(AudioMixerGroup group)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AudioMixer.GetAbsoluteAudibilityFromGroup_Injected(intPtr, Object.MarshalledUnityObject.Marshal<AudioMixerGroup>(group));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_outputAudioMixerGroup_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_outputAudioMixerGroup_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr FindSnapshot_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioMixerGroup[] FindMatchingGroups_Injected(IntPtr _unity_self, ref ManagedSpanWrapper subPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TransitionToSnapshotInternal_Injected(IntPtr _unity_self, IntPtr snapshot, float timeToReach);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TransitionToSnapshots_Injected(IntPtr _unity_self, AudioMixerSnapshot[] snapshots, ref ManagedSpanWrapper weights, float timeToReach);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioMixerUpdateMode get_updateMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_updateMode_Injected(IntPtr _unity_self, AudioMixerUpdateMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetFloat_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ClearFloat_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetFloat_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, out float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetAbsoluteAudibilityFromGroup_Injected(IntPtr _unity_self, IntPtr group);
	}
}
