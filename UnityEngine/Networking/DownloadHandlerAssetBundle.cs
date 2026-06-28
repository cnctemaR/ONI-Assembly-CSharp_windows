using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerAssetBundle : DownloadHandler
	{
		public DownloadHandlerAssetBundle(string url, uint crc)
		{
			base.InternalCreateAssetBundle(url, crc);
		}

		public DownloadHandlerAssetBundle(string url, uint version, uint crc)
		{
			Hash128 hash = new Hash128(0U, 0U, 0U, version);
			base.InternalCreateAssetBundle(url, hash, crc);
		}

		public DownloadHandlerAssetBundle(string url, Hash128 hash, uint crc)
		{
			base.InternalCreateAssetBundle(url, hash, crc);
		}

		protected override byte[] GetData()
		{
			throw new NotSupportedException("Raw data access is not supported for asset bundles");
		}

		protected override string GetText()
		{
			throw new NotSupportedException("String access is not supported for asset bundles");
		}

		public extern AssetBundle assetBundle
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static AssetBundle GetContent(UnityWebRequest www)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerAssetBundle>(www).assetBundle;
		}
	}
}
