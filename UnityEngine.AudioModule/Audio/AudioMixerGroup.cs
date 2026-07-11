using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	/// <summary>
	///   <para>Object representing a group in the mixer.</para>
	/// </summary>
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
