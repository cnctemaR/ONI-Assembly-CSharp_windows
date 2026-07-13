using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequestTexture/Public/DownloadHandlerTexture.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerTexture : DownloadHandler
	{
		private static IntPtr Create([UnityMarshalAs(NativeType.ScriptingObjectPtr)] DownloadHandlerTexture obj, DownloadedTextureParams parameters)
		{
			return DownloadHandlerTexture.Create_Injected(obj, ref parameters);
		}

		private void InternalCreateTexture(DownloadedTextureParams parameters)
		{
			this.m_Ptr = DownloadHandlerTexture.Create(this, parameters);
		}

		public DownloadHandlerTexture()
			: this(true)
		{
		}

		public DownloadHandlerTexture(bool readable)
		{
			DownloadedTextureParams @default = DownloadedTextureParams.Default;
			@default.readable = readable;
			this.InternalCreateTexture(@default);
		}

		public DownloadHandlerTexture(DownloadedTextureParams parameters)
		{
			this.InternalCreateTexture(parameters);
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

		public Texture2D texture
		{
			get
			{
				return this.InternalGetTextureNative();
			}
		}

		[NativeThrows]
		private Texture2D InternalGetTextureNative()
		{
			IntPtr intPtr = DownloadHandlerTexture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture2D>(DownloadHandlerTexture.InternalGetTextureNative_Injected(intPtr));
		}

		public static Texture2D GetContent(UnityWebRequest www)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerTexture>(www).texture;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected(DownloadHandlerTexture obj, [In] ref DownloadedTextureParams parameters);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalGetTextureNative_Injected(IntPtr _unity_self);

		private NativeArray<byte> m_NativeData;

		internal new static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(DownloadHandlerTexture handler)
			{
				return handler.m_Ptr;
			}
		}
	}
}
