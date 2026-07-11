using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngineInternal.Video
{
	[UsedByNativeCode]
	[NativeHeader("Modules/Video/Public/Base/VideoMediaPlayback.h")]
	internal class VideoPlaybackMgr : IDisposable
	{
		public VideoPlaybackMgr()
		{
			this.m_Ptr = VideoPlaybackMgr.Internal_Create();
		}

		public void Dispose()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				VideoPlaybackMgr.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern VideoPlayback CreateVideoPlayback(string fileName, VideoPlaybackMgr.MessageCallback errorCallback, VideoPlaybackMgr.Callback readyCallback, VideoPlaybackMgr.Callback reachedEndCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ReleaseVideoPlayback(VideoPlayback playback);

		public extern ulong videoPlaybackCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ProcessOSMainLoopMessagesForTesting();

		internal IntPtr m_Ptr;

		public delegate void Callback();

		public delegate void MessageCallback(string message);
	}
}
