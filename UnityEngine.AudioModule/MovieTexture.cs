using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
	/// </summary>
	[ExcludeFromPreset]
	[ExcludeFromObjectFactory]
	[Obsolete("MovieTexture is deprecated. Use VideoPlayer instead.", false)]
	public sealed class MovieTexture : Texture
	{
		private MovieTexture()
		{
		}

		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		public void Play()
		{
			MovieTexture.INTERNAL_CALL_Play(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Play(MovieTexture self);

		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		public void Stop()
		{
			MovieTexture.INTERNAL_CALL_Stop(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Stop(MovieTexture self);

		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		public void Pause()
		{
			MovieTexture.INTERNAL_CALL_Pause(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Pause(MovieTexture self);

		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		public extern AudioClip audioClip
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		public extern bool loop
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		public extern bool isPlaying
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		public extern bool isReadyToPlay
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		public extern float duration
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
