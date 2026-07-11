using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>The Audio Distortion Filter distorts the sound from an AudioSource or sounds reaching the AudioListener.</para>
	/// </summary>
	[RequireComponent(typeof(AudioBehaviour))]
	public sealed class AudioDistortionFilter : Behaviour
	{
		/// <summary>
		///   <para>Distortion value. 0.0 to 1.0. Default = 0.5.</para>
		/// </summary>
		public extern float distortionLevel
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
