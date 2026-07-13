using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.Audio
{
	[NativeHeader("Modules/Audio/Public/AudioMixerGroup.h")]
	public class AudioMixerGroup : Object, ISubAssetNotDuplicatable
	{
		internal AudioMixerGroup()
		{
		}

		[NativeProperty]
		public AudioMixer audioMixer
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AudioMixerGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<AudioMixer>(AudioMixerGroup.get_audioMixer_Injected(intPtr));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_audioMixer_Injected(IntPtr _unity_self);
	}
}
