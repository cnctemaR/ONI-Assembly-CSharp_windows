using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequestAssetBundle/Public/DownloadHandlerAssetBundle.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerAssetBundle : DownloadHandler
	{
		private unsafe static IntPtr Create([UnityMarshalAs(NativeType.ScriptingObjectPtr)] DownloadHandlerAssetBundle obj, string url, uint crc)
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
				intPtr = DownloadHandlerAssetBundle.Create_Injected(obj, ref managedSpanWrapper, crc);
			}
			finally
			{
				char* ptr = null;
			}
			return intPtr;
		}

		private unsafe static IntPtr CreateCached([UnityMarshalAs(NativeType.ScriptingObjectPtr)] DownloadHandlerAssetBundle obj, string url, string name, Hash128 hash, uint crc)
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
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = name.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				intPtr = DownloadHandlerAssetBundle.CreateCached_Injected(obj, ref managedSpanWrapper, ref managedSpanWrapper2, ref hash, crc);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return intPtr;
		}

		private void InternalCreateAssetBundle(string url, uint crc)
		{
			this.m_Ptr = DownloadHandlerAssetBundle.Create(this, url, crc);
		}

		private void InternalCreateAssetBundleCached(string url, string name, Hash128 hash, uint crc)
		{
			this.m_Ptr = DownloadHandlerAssetBundle.CreateCached(this, url, name, hash, crc);
		}

		public DownloadHandlerAssetBundle(string url, uint crc)
		{
			this.InternalCreateAssetBundle(url, crc);
		}

		public DownloadHandlerAssetBundle(string url, uint version, uint crc)
		{
			this.InternalCreateAssetBundleCached(url, "", new Hash128(0U, 0U, 0U, version), crc);
		}

		public DownloadHandlerAssetBundle(string url, Hash128 hash, uint crc)
		{
			this.InternalCreateAssetBundleCached(url, "", hash, crc);
		}

		public DownloadHandlerAssetBundle(string url, string name, Hash128 hash, uint crc)
		{
			this.InternalCreateAssetBundleCached(url, name, hash, crc);
		}

		public DownloadHandlerAssetBundle(string url, CachedAssetBundle cachedBundle, uint crc)
		{
			this.InternalCreateAssetBundleCached(url, cachedBundle.name, cachedBundle.hash, crc);
		}

		protected override byte[] GetData()
		{
			throw new NotSupportedException("Raw data access is not supported for asset bundles");
		}

		protected override string GetText()
		{
			throw new NotSupportedException("String access is not supported for asset bundles");
		}

		public AssetBundle assetBundle
		{
			get
			{
				IntPtr intPtr = DownloadHandlerAssetBundle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<AssetBundle>(DownloadHandlerAssetBundle.get_assetBundle_Injected(intPtr));
			}
		}

		public bool autoLoadAssetBundle
		{
			get
			{
				IntPtr intPtr = DownloadHandlerAssetBundle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return DownloadHandlerAssetBundle.get_autoLoadAssetBundle_Injected(intPtr);
			}
			[NativeThrows]
			set
			{
				IntPtr intPtr = DownloadHandlerAssetBundle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				DownloadHandlerAssetBundle.set_autoLoadAssetBundle_Injected(intPtr, value);
			}
		}

		public bool isDownloadComplete
		{
			get
			{
				IntPtr intPtr = DownloadHandlerAssetBundle.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return DownloadHandlerAssetBundle.get_isDownloadComplete_Injected(intPtr);
			}
		}

		public static AssetBundle GetContent(UnityWebRequest www)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerAssetBundle>(www).assetBundle;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected(DownloadHandlerAssetBundle obj, ref ManagedSpanWrapper url, uint crc);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateCached_Injected(DownloadHandlerAssetBundle obj, ref ManagedSpanWrapper url, ref ManagedSpanWrapper name, [In] ref Hash128 hash, uint crc);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_assetBundle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_autoLoadAssetBundle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_autoLoadAssetBundle_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isDownloadComplete_Injected(IntPtr _unity_self);

		internal new static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(DownloadHandlerAssetBundle handler)
			{
				return handler.m_Ptr;
			}
		}
	}
}
