using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.Audio
{
	[NativeHeader("Modules/Audio/Public/AudioMixerSnapshot.h")]
	public class AudioMixerSnapshot : Object, ISubAssetNotDuplicatable
	{
		internal AudioMixerSnapshot()
		{
		}

		[NativeProperty]
		public AudioMixer audioMixer
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixerSnapshot>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<AudioMixer>(AudioMixerSnapshot.get_audioMixer_Injected(intPtr));
			}
		}

		public void TransitionTo(float timeToReach)
		{
			this.audioMixer.TransitionToSnapshot(this, timeToReach);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_audioMixer_Injected(IntPtr _unity_self);
	}
}
