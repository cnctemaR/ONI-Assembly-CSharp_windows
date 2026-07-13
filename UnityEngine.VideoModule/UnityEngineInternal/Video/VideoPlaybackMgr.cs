using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngineInternal.Video
{
	[NativeHeader("Modules/Video/Public/Base/VideoMediaPlayback.h")]
	[UsedByNativeCode]
	internal class VideoPlaybackMgr : IDisposable
	{
		public VideoPlaybackMgr()
		{
			this.m_Ptr = VideoPlaybackMgr.Internal_Create();
		}

		public void Dispose()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
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

		public unsafe VideoPlayback CreateVideoPlayback(string fileName, VideoPlaybackMgr.MessageCallback errorCallback, VideoPlaybackMgr.Callback readyCallback, VideoPlaybackMgr.Callback reachedEndCallback, bool splitAlpha = false)
		{
			VideoPlayback videoPlayback;
			try
			{
				IntPtr intPtr = VideoPlaybackMgr.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(fileName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = fileName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr2 = VideoPlaybackMgr.CreateVideoPlayback_Injected(intPtr, ref managedSpanWrapper, errorCallback, readyCallback, reachedEndCallback, splitAlpha);
			}
			finally
			{
				IntPtr intPtr2;
				IntPtr intPtr3 = intPtr2;
				videoPlayback = ((intPtr3 == 0) ? null : VideoPlayback.BindingsMarshaller.ConvertToManaged(intPtr3));
				char* ptr = null;
			}
			return videoPlayback;
		}

		public void ReleaseVideoPlayback(VideoPlayback playback)
		{
			IntPtr intPtr = VideoPlaybackMgr.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlaybackMgr.ReleaseVideoPlayback_Injected(intPtr, (playback == null) ? ((IntPtr)0) : VideoPlayback.BindingsMarshaller.ConvertToNative(playback));
		}

		public ulong videoPlaybackCount
		{
			get
			{
				IntPtr intPtr = VideoPlaybackMgr.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlaybackMgr.get_videoPlaybackCount_Injected(intPtr);
			}
		}

		public void Update()
		{
			IntPtr intPtr = VideoPlaybackMgr.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlaybackMgr.Update_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateVideoPlayback_Injected(IntPtr _unity_self, ref ManagedSpanWrapper fileName, VideoPlaybackMgr.MessageCallback errorCallback, VideoPlaybackMgr.Callback readyCallback, VideoPlaybackMgr.Callback reachedEndCallback, bool splitAlpha);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseVideoPlayback_Injected(IntPtr _unity_self, IntPtr playback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong get_videoPlaybackCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Update_Injected(IntPtr _unity_self);

		internal IntPtr m_Ptr;

		public delegate void Callback();

		public delegate void MessageCallback(string message);

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(VideoPlaybackMgr videoPlaybackMgr)
			{
				return videoPlaybackMgr.m_Ptr;
			}
		}
	}
}
