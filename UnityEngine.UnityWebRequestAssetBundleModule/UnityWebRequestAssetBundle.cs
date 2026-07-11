using System;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>Helpers for downloading asset bundles using UnityWebRequest.</para>
	/// </summary>
	public static class UnityWebRequestAssetBundle
	{
		/// <summary>
		///   <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
		/// </summary>
		/// <param name="uri">The URI of the asset bundle to download.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
		///
		/// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="cachedAssetBundle">A structure used to download a given version of AssetBundle to a customized cache path.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
		/// </returns>
		public static UnityWebRequest GetAssetBundle(string uri)
		{
			return UnityWebRequestAssetBundle.GetAssetBundle(uri, 0U);
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
		/// </summary>
		/// <param name="uri">The URI of the asset bundle to download.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
		///
		/// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="cachedAssetBundle">A structure used to download a given version of AssetBundle to a customized cache path.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
		/// </returns>
		public static UnityWebRequest GetAssetBundle(Uri uri)
		{
			return UnityWebRequestAssetBundle.GetAssetBundle(uri, 0U);
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
		/// </summary>
		/// <param name="uri">The URI of the asset bundle to download.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
		///
		/// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="cachedAssetBundle">A structure used to download a given version of AssetBundle to a customized cache path.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
		/// </returns>
		public static UnityWebRequest GetAssetBundle(string uri, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, crc), null);
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
		/// </summary>
		/// <param name="uri">The URI of the asset bundle to download.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
		///
		/// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="cachedAssetBundle">A structure used to download a given version of AssetBundle to a customized cache path.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
		/// </returns>
		public static UnityWebRequest GetAssetBundle(Uri uri, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri.AbsoluteUri, crc), null);
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
		/// </summary>
		/// <param name="uri">The URI of the asset bundle to download.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
		///
		/// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="cachedAssetBundle">A structure used to download a given version of AssetBundle to a customized cache path.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
		/// </returns>
		public static UnityWebRequest GetAssetBundle(string uri, uint version, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, version, crc), null);
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
		/// </summary>
		/// <param name="uri">The URI of the asset bundle to download.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
		///
		/// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="cachedAssetBundle">A structure used to download a given version of AssetBundle to a customized cache path.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
		/// </returns>
		public static UnityWebRequest GetAssetBundle(Uri uri, uint version, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri.AbsoluteUri, version, crc), null);
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
		/// </summary>
		/// <param name="uri">The URI of the asset bundle to download.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
		///
		/// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="cachedAssetBundle">A structure used to download a given version of AssetBundle to a customized cache path.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
		/// </returns>
		public static UnityWebRequest GetAssetBundle(string uri, Hash128 hash, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, hash, crc), null);
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
		/// </summary>
		/// <param name="uri">The URI of the asset bundle to download.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
		///
		/// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="cachedAssetBundle">A structure used to download a given version of AssetBundle to a customized cache path.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
		/// </returns>
		public static UnityWebRequest GetAssetBundle(Uri uri, Hash128 hash, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri.AbsoluteUri, hash, crc), null);
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
		/// </summary>
		/// <param name="uri">The URI of the asset bundle to download.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
		///
		/// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="cachedAssetBundle">A structure used to download a given version of AssetBundle to a customized cache path.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
		/// </returns>
		public static UnityWebRequest GetAssetBundle(string uri, CachedAssetBundle cachedAssetBundle, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, cachedAssetBundle.name, cachedAssetBundle.hash, crc), null);
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest optimized for downloading a Unity Asset Bundle via HTTP GET.</para>
		/// </summary>
		/// <param name="uri">The URI of the asset bundle to download.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="version">An integer version number, which will be compared to the cached version of the asset bundle to download. Increment this number to force Unity to redownload a cached asset bundle.
		///
		/// Analogous to the version parameter for WWW.LoadFromCacheOrDownload.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="cachedAssetBundle">A structure used to download a given version of AssetBundle to a customized cache path.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to downloading a Unity Asset Bundle.</para>
		/// </returns>
		public static UnityWebRequest GetAssetBundle(Uri uri, CachedAssetBundle cachedAssetBundle, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri.AbsoluteUri, cachedAssetBundle.name, cachedAssetBundle.hash, crc), null);
		}
	}
}
