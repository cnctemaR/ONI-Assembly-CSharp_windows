using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	public class AudioMixerGroup : Object, ISubAssetNotDuplicatable
	{
		internal AudioMixerGroup()
		{
		}

		public extern AudioMixer audioMixer
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
