using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>A DownloadHandler subclass specialized for downloading AssetBundles.</para>
	/// </summary>
	[NativeHeader("Modules/UnityWebRequestAssetBundle/Public/DownloadHandlerAssetBundle.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DownloadHandlerAssetBundle : DownloadHandler
	{
		/// <summary>
		///   <para>Standard constructor for non-cached asset bundles.</para>
		/// </summary>
		/// <param name="url">The nominal (pre-redirect) URL at which the asset bundle is located.</param>
		/// <param name="crc">A checksum to compare to the downloaded data for integrity checking, or zero to skip integrity checking.</param>
		public DownloadHandlerAssetBundle(string url, uint crc)
		{
			this.InternalCreateAssetBundle(url, crc);
		}

		/// <summary>
		///   <para>Simple versioned constructor. Caches downloaded asset bundles.</para>
		/// </summary>
		/// <param name="url">The nominal (pre-redirect) URL at which the asset bundle is located.</param>
		/// <param name="crc">A checksum to compare to the downloaded data for integrity checking, or zero to skip integrity checking.</param>
		/// <param name="version">Current version number of the asset bundle at url. Increment to redownload.</param>
		public DownloadHandlerAssetBundle(string url, uint version, uint crc)
		{
			this.InternalCreateAssetBundleCached(url, "", new Hash128(0U, 0U, 0U, version), crc);
		}

		/// <summary>
		///   <para>Versioned constructor. Caches downloaded asset bundles.</para>
		/// </summary>
		/// <param name="url">The nominal (pre-redirect) URL at which the asset bundle is located.</param>
		/// <param name="crc">A checksum to compare to the downloaded data for integrity checking, or zero to skip integrity checking.</param>
		/// <param name="hash">A hash object defining the version of the asset bundle.</param>
		public DownloadHandlerAssetBundle(string url, Hash128 hash, uint crc)
		{
			this.InternalCreateAssetBundleCached(url, "", hash, crc);
		}

		public DownloadHandlerAssetBundle(string url, string name, Hash128 hash, uint crc)
		{
			this.InternalCreateAssetBundleCached(url, name, hash, crc);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(DownloadHandlerAssetBundle obj, string url, uint crc);

		private static IntPtr CreateCached(DownloadHandlerAssetBundle obj, string url, string name, Hash128 hash, uint crc)
		{
			return DownloadHandlerAssetBundle.CreateCached_Injected(obj, url, name, ref hash, crc);
		}

		private void InternalCreateAssetBundle(string url, uint crc)
		{
			this.m_Ptr = DownloadHandlerAssetBundle.Create(this, url, crc);
		}

		private void InternalCreateAssetBundleCached(string url, string name, Hash128 hash, uint crc)
		{
			this.m_Ptr = DownloadHandlerAssetBundle.CreateCached(this, url, name, hash, crc);
		}

		/// <summary>
		///   <para>Not implemented. Throws &lt;a href="http:msdn.microsoft.comen-uslibrarysystem.notsupportedexception"&gt;NotSupportedException&lt;a&gt;.</para>
		/// </summary>
		/// <returns>
		///   <para>Not implemented.</para>
		/// </returns>
		protected override byte[] GetData()
		{
			throw new NotSupportedException("Raw data access is not supported for asset bundles");
		}

		/// <summary>
		///   <para>Not implemented. Throws &lt;a href="http:msdn.microsoft.comen-uslibrarysystem.notsupportedexception"&gt;NotSupportedException&lt;a&gt;.</para>
		/// </summary>
		/// <returns>
		///   <para>Not implemented.</para>
		/// </returns>
		protected override string GetText()
		{
			throw new NotSupportedException("String access is not supported for asset bundles");
		}

		/// <summary>
		///   <para>Returns the downloaded AssetBundle, or null. (Read Only)</para>
		/// </summary>
		public extern AssetBundle assetBundle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the downloaded AssetBundle, or null.</para>
		/// </summary>
		/// <param name="www">A finished UnityWebRequest object with DownloadHandlerAssetBundle attached.</param>
		/// <returns>
		///   <para>The same as DownloadHandlerAssetBundle.assetBundle</para>
		/// </returns>
		public static AssetBundle GetContent(UnityWebRequest www)
		{
			return DownloadHandler.GetCheckedDownloader<DownloadHandlerAssetBundle>(www).assetBundle;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateCached_Injected(DownloadHandlerAssetBundle obj, string url, string name, ref Hash128 hash, uint crc);
	}
}
