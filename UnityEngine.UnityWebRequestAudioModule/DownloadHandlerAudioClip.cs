using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequestAudio/Public/DownloadHandlerAudioClip.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerAudioClip : DownloadHandler
	{
		private unsafe static IntPtr Create([UnityMarshalAs(NativeType.ScriptingObjectPtr)] DownloadHandlerAudioClip obj, string url, AudioType audioType)
		{
			IntPtr intPtr;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(url, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = url.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				intPtr = DownloadHandlerAudioClip.Create_Injected(obj, ref managedSpanWrapper, audioType);
			}
			finally
			{
				char* ptr = null;
			}
			return intPtr;
		}

		private void InternalCreateAudioClip(string url, AudioType audioType)
		{
			this.m_Ptr = DownloadHandlerAudioClip.Create(this, url, audioType);
		}

		public DownloadHandlerAudioClip(string url, AudioType audioType)
		{
			this.InternalCreateAudioClip(url, audioType);
		}

		public DownloadHandlerAudioClip(Uri uri, AudioType audioType)
		{
			this.InternalCreateAudioClip(uri.AbsoluteUri, audioType);
		}

		protected override NativeArray<byte> GetNativeData()
		{
			return DownloadHandler.InternalGetNativeArray(this, ref this.m_NativeData);
		}

		public override void Dispose()
		{
			DownloadHandler.DisposeNativeArray(ref this.m_NativeData);
			base.Dispose();
		}

		protected override string GetText()
		{
			throw new NotSupportedException("String access is not supported for audio clips");
		}

		[NativeThrows]
		public AudioClip audioClip
		{
			get
			{
				IntPtr intPtr = DownloadHandlerAudioClip.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<AudioClip>(DownloadHandlerAudioClip.get_audioClip_Injected(intPtr));
			}
		}

		public bool streamAudio
		{
			get
			{
				IntPtr intPtr = DownloadHandlerAudioClip.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return DownloadHandlerAudioClip.get_streamAudio_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = DownloadHandlerAudioClip.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				DownloadHandlerAudioClip.set_streamAudio_Injected(intPtr, value);
			}
		}

		public bool compressed
		{
			get
			{
				IntPtr intPtr = DownloadHandlerAudioClip.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return DownloadHandlerAudioClip.get_compressed_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = DownloadHandlerAudioClip.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				DownloadHandlerAudioClip.set_compressed_Injected(intPtr, value);
			}
		}

		public static AudioClip GetContent(UnityWebRequest www)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerAudioClip>(www).audioClip;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected(DownloadHandlerAudioClip obj, ref ManagedSpanWrapper url, AudioType audioType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_audioClip_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_streamAudio_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_streamAudio_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_compressed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_compressed_Injected(IntPtr _unity_self, bool value);

		private NativeArray<byte> m_NativeData;

		internal new static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(DownloadHandlerAudioClip handler)
			{
				return handler.m_Ptr;
			}
		}
	}
}
