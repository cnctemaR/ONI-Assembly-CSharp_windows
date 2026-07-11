using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngineInternal.Video
{
	[NativeHeader("Modules/Video/Public/Base/MediaComponent.h")]
	[UsedByNativeCode]
	internal class VideoPlayback
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StartPlayback();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void PausePlayback();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StopPlayback();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern VideoError GetStatus();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsReady();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsPlaying();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Step();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool CanStep();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern uint GetWidth();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern uint GetHeight();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetFrameRate();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetDuration();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ulong GetFrameCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern uint GetPixelAspectRatioNumerator();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern uint GetPixelAspectRatioDenominator();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern VideoPixelFormat GetPixelFormat();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool CanNotSkipOnDrop();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetSkipOnDrop(bool skipOnDrop);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetTexture(Texture texture, out long outputFrameNum);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SeekToFrame(long frameIndex, VideoPlayback.Callback seekCompletedCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SeekToTime(double secs, VideoPlayback.Callback seekCompletedCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetPlaybackSpeed();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPlaybackSpeed(float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetLoop();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetLoop(bool value);

		internal IntPtr m_Ptr;

		public delegate void Callback();
	}
}
