using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Video
{
	/// <summary>
	///   <para>A container for video data.</para>
	/// </summary>
	[RequiredByNativeCode]
	[NativeHeader("Modules/Video/Public/VideoClip.h")]
	public sealed class VideoClip : Object
	{
		private VideoClip()
		{
		}

		/// <summary>
		///   <para>The video clip path in the project's assets. (Read Only).</para>
		/// </summary>
		public extern string originalPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The length of the VideoClip in frames. (Read Only).</para>
		/// </summary>
		public extern ulong frameCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The frame rate of the clip in frames/second. (Read Only).</para>
		/// </summary>
		public extern double frameRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The length of the video clip in seconds. (Read Only).</para>
		/// </summary>
		[NativeName("Duration")]
		public extern double length
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The width of the images in the video clip in pixels. (Read Only).</para>
		/// </summary>
		public extern uint width
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The height of the images in the video clip in pixels. (Read Only).</para>
		/// </summary>
		public extern uint height
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Numerator of the pixel aspect ratio (num:den). (Read Only).</para>
		/// </summary>
		public extern uint pixelAspectRatioNumerator
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Denominator of the pixel aspect ratio (num:den). (Read Only).</para>
		/// </summary>
		public extern uint pixelAspectRatioDenominator
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Number of audio tracks in the clip.</para>
		/// </summary>
		public extern ushort audioTrackCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The number of channels in the audio track.  E.g. 2 for a stereo track.</para>
		/// </summary>
		/// <param name="audioTrackIdx">Index of the audio queried audio track.</param>
		/// <returns>
		///   <para>The number of channels.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ushort GetAudioChannelCount(ushort audioTrackIdx);

		/// <summary>
		///   <para>Get the audio track sampling rate in Hertz.</para>
		/// </summary>
		/// <param name="audioTrackIdx">Index of the audio queried audio track.</param>
		/// <returns>
		///   <para>The sampling rate in Hertz.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern uint GetAudioSampleRate(ushort audioTrackIdx);

		/// <summary>
		///   <para>Get the audio track language.  Can be unknown.</para>
		/// </summary>
		/// <param name="audioTrackIdx">Index of the audio queried audio track.</param>
		/// <returns>
		///   <para>The abbreviated name of the language.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetAudioLanguage(ushort audioTrackIdx);
	}
}
