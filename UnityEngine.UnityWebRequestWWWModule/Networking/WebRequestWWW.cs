using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequestAudio/Public/DownloadHandlerAudioClip.h")]
	internal static class WebRequestWWW
	{
		[FreeFunction("UnityWebRequestCreateAudioClip")]
		internal unsafe static AudioClip InternalCreateAudioClipUsingDH(DownloadHandler dh, string url, bool stream, bool compressed, AudioType audioType)
		{
			AudioClip audioClip;
			try
			{
				IntPtr intPtr = ((dh == null) ? ((IntPtr)0) : DownloadHandler.BindingsMarshaller.ConvertToNative(dh));
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(url, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = url.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr2 = WebRequestWWW.InternalCreateAudioClipUsingDH_Injected(intPtr, ref managedSpanWrapper, stream, compressed, audioType);
			}
			finally
			{
				IntPtr intPtr2;
				audioClip = Unmarshal.UnmarshalUnityObject<AudioClip>(intPtr2);
				char* ptr = null;
			}
			return audioClip;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalCreateAudioClipUsingDH_Injected(IntPtr dh, ref ManagedSpanWrapper url, bool stream, bool compressed, AudioType audioType);
	}
}
