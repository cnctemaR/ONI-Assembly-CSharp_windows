using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Video
{
	[NativeHeader("Modules/Video/Public/VideoClip.h")]
	[RequiredByNativeCode]
	public sealed class VideoClip : Object
	{
		private VideoClip()
		{
		}

		public string originalPath
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					VideoClip.get_originalPath_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public ulong frameCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoClip.get_frameCount_Injected(intPtr);
			}
		}

		public double frameRate
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoClip.get_frameRate_Injected(intPtr);
			}
		}

		[NativeName("Duration")]
		public double length
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoClip.get_length_Injected(intPtr);
			}
		}

		public uint width
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoClip.get_width_Injected(intPtr);
			}
		}

		public uint height
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoClip.get_height_Injected(intPtr);
			}
		}

		public uint pixelAspectRatioNumerator
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoClip.get_pixelAspectRatioNumerator_Injected(intPtr);
			}
		}

		public uint pixelAspectRatioDenominator
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoClip.get_pixelAspectRatioDenominator_Injected(intPtr);
			}
		}

		public bool sRGB
		{
			[NativeName("IssRGB")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoClip.get_sRGB_Injected(intPtr);
			}
		}

		public ushort audioTrackCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoClip.get_audioTrackCount_Injected(intPtr);
			}
		}

		public ushort GetAudioChannelCount(ushort audioTrackIdx)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoClip.GetAudioChannelCount_Injected(intPtr, audioTrackIdx);
		}

		public uint GetAudioSampleRate(ushort audioTrackIdx)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoClip.GetAudioSampleRate_Injected(intPtr, audioTrackIdx);
		}

		public string GetAudioLanguage(ushort audioTrackIdx)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				VideoClip.GetAudioLanguage_Injected(intPtr, audioTrackIdx, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_originalPath_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong get_frameCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double get_frameRate_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double get_length_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_width_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_height_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_pixelAspectRatioNumerator_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_pixelAspectRatioDenominator_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_sRGB_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ushort get_audioTrackCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ushort GetAudioChannelCount_Injected(IntPtr _unity_self, ushort audioTrackIdx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetAudioSampleRate_Injected(IntPtr _unity_self, ushort audioTrackIdx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAudioLanguage_Injected(IntPtr _unity_self, ushort audioTrackIdx, out ManagedSpanWrapper ret);
	}
}
