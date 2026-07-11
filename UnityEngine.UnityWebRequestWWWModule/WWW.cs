using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace UnityEngine
{
	/// <summary>
	///   <para>Simple access to web pages.</para>
	/// </summary>
	public class WWW : CustomYieldInstruction, IDisposable
	{
		/// <summary>
		///   <para>Creates a WWW request with the given URL.</para>
		/// </summary>
		/// <param name="url">The url to download. Must be '%' escaped.</param>
		/// <returns>
		///   <para>A new WWW object. When it has been downloaded, the results can be fetched from the returned object.</para>
		/// </returns>
		public WWW(string url)
		{
			this._uwr = UnityWebRequest.Get(url);
			this._uwr.SendWebRequest();
		}

		/// <summary>
		///   <para>Creates a WWW request with the given URL.</para>
		/// </summary>
		/// <param name="url">The url to download. Must be '%' escaped.</param>
		/// <param name="form">A WWWForm instance containing the form data to post.</param>
		/// <returns>
		///   <para>A new WWW object. When it has been downloaded, the results can be fetched from the returned object.</para>
		/// </returns>
		public WWW(string url, WWWForm form)
		{
			this._uwr = UnityWebRequest.Post(url, form);
			this._uwr.chunkedTransfer = false;
			this._uwr.SendWebRequest();
		}

		/// <summary>
		///   <para>Creates a WWW request with the given URL.</para>
		/// </summary>
		/// <param name="url">The url to download. Must be '%' escaped.</param>
		/// <param name="postData">A byte array of data to be posted to the url.</param>
		/// <returns>
		///   <para>A new WWW object. When it has been downloaded, the results can be fetched from the returned object.</para>
		/// </returns>
		public WWW(string url, byte[] postData)
		{
			this._uwr = new UnityWebRequest(url, "POST");
			this._uwr.chunkedTransfer = false;
			UploadHandler uploadHandler = new UploadHandlerRaw(postData);
			uploadHandler.contentType = "application/x-www-form-urlencoded";
			this._uwr.uploadHandler = uploadHandler;
			this._uwr.downloadHandler = new DownloadHandlerBuffer();
			this._uwr.SendWebRequest();
		}

		/// <summary>
		///   <para>Creates a WWW request with the given URL.</para>
		/// </summary>
		/// <param name="url">The url to download. Must be '%' escaped.</param>
		/// <param name="postData">A byte array of data to be posted to the url.</param>
		/// <param name="headers">A hash table of custom headers to send with the request.</param>
		/// <returns>
		///   <para>A new WWW object. When it has been downloaded, the results can be fetched from the returned object.</para>
		/// </returns>
		[Obsolete("This overload is deprecated. Use UnityEngine.WWW.WWW(string, byte[], System.Collections.Generic.Dictionary<string, string>) instead.")]
		public WWW(string url, byte[] postData, Hashtable headers)
		{
			string text = ((postData != null) ? "POST" : "GET");
			this._uwr = new UnityWebRequest(url, text);
			this._uwr.chunkedTransfer = false;
			UploadHandler uploadHandler = new UploadHandlerRaw(postData);
			uploadHandler.contentType = "application/x-www-form-urlencoded";
			this._uwr.uploadHandler = uploadHandler;
			this._uwr.downloadHandler = new DownloadHandlerBuffer();
			IEnumerator enumerator = headers.Keys.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					this._uwr.SetRequestHeader((string)obj, (string)headers[obj]);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			this._uwr.SendWebRequest();
		}

		public WWW(string url, byte[] postData, Dictionary<string, string> headers)
		{
			string text = ((postData != null) ? "POST" : "GET");
			this._uwr = new UnityWebRequest(url, text);
			this._uwr.chunkedTransfer = false;
			UploadHandler uploadHandler = new UploadHandlerRaw(postData);
			uploadHandler.contentType = "application/x-www-form-urlencoded";
			this._uwr.uploadHandler = uploadHandler;
			this._uwr.downloadHandler = new DownloadHandlerBuffer();
			foreach (KeyValuePair<string, string> keyValuePair in headers)
			{
				this._uwr.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
			}
			this._uwr.SendWebRequest();
		}

		internal WWW(string url, string name, Hash128 hash, uint crc)
		{
			this._uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, new CachedAssetBundle(name, hash), crc);
			this._uwr.SendWebRequest();
		}

		/// <summary>
		///   <para>Escapes characters in a string to ensure they are URL-friendly.</para>
		/// </summary>
		/// <param name="s">A string with characters to be escaped.</param>
		/// <param name="e">The text encoding to use.</param>
		public static string EscapeURL(string s)
		{
			return WWW.EscapeURL(s, Encoding.UTF8);
		}

		/// <summary>
		///   <para>Escapes characters in a string to ensure they are URL-friendly.</para>
		/// </summary>
		/// <param name="s">A string with characters to be escaped.</param>
		/// <param name="e">The text encoding to use.</param>
		public static string EscapeURL(string s, Encoding e)
		{
			return UnityWebRequest.EscapeURL(s, e);
		}

		/// <summary>
		///   <para>Converts URL-friendly escape sequences back to normal text.</para>
		/// </summary>
		/// <param name="s">A string containing escaped characters.</param>
		/// <param name="e">The text encoding to use.</param>
		public static string UnEscapeURL(string s)
		{
			return WWW.UnEscapeURL(s, Encoding.UTF8);
		}

		/// <summary>
		///   <para>Converts URL-friendly escape sequences back to normal text.</para>
		/// </summary>
		/// <param name="s">A string containing escaped characters.</param>
		/// <param name="e">The text encoding to use.</param>
		public static string UnEscapeURL(string s, Encoding e)
		{
			return UnityWebRequest.UnEscapeURL(s, e);
		}

		/// <summary>
		///   <para>Loads an AssetBundle with the specified version number from the cache. If the AssetBundle is not currently cached, it will automatically be downloaded and stored in the cache for future retrieval from local storage.</para>
		/// </summary>
		/// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
		/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBundle from url.</param>
		/// <param name="hash">Hash128 which is used as the version of the AssetBundle.</param>
		/// <param name="cachedBundle">A structure used to download a given version of AssetBundle to a customized cache path.
		///
		/// Analogous to the cachedAssetBundle parameter for UnityWebRequestAssetBundle.GetAssetBundle.&lt;/param&gt;</param>
		/// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
		/// <returns>
		///   <para>A WWW instance, which can be used to access the data once the load/download operation is completed.</para>
		/// </returns>
		public static WWW LoadFromCacheOrDownload(string url, int version)
		{
			return WWW.LoadFromCacheOrDownload(url, version, 0U);
		}

		/// <summary>
		///   <para>Loads an AssetBundle with the specified version number from the cache. If the AssetBundle is not currently cached, it will automatically be downloaded and stored in the cache for future retrieval from local storage.</para>
		/// </summary>
		/// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
		/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBundle from url.</param>
		/// <param name="hash">Hash128 which is used as the version of the AssetBundle.</param>
		/// <param name="cachedBundle">A structure used to download a given version of AssetBundle to a customized cache path.
		///
		/// Analogous to the cachedAssetBundle parameter for UnityWebRequestAssetBundle.GetAssetBundle.&lt;/param&gt;</param>
		/// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
		/// <returns>
		///   <para>A WWW instance, which can be used to access the data once the load/download operation is completed.</para>
		/// </returns>
		public static WWW LoadFromCacheOrDownload(string url, int version, uint crc)
		{
			Hash128 hash = new Hash128(0U, 0U, 0U, (uint)version);
			return WWW.LoadFromCacheOrDownload(url, hash, crc);
		}

		public static WWW LoadFromCacheOrDownload(string url, Hash128 hash)
		{
			return WWW.LoadFromCacheOrDownload(url, hash, 0U);
		}

		/// <summary>
		///   <para>Loads an AssetBundle with the specified version number from the cache. If the AssetBundle is not currently cached, it will automatically be downloaded and stored in the cache for future retrieval from local storage.</para>
		/// </summary>
		/// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
		/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBundle from url.</param>
		/// <param name="hash">Hash128 which is used as the version of the AssetBundle.</param>
		/// <param name="cachedBundle">A structure used to download a given version of AssetBundle to a customized cache path.
		///
		/// Analogous to the cachedAssetBundle parameter for UnityWebRequestAssetBundle.GetAssetBundle.&lt;/param&gt;</param>
		/// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
		/// <returns>
		///   <para>A WWW instance, which can be used to access the data once the load/download operation is completed.</para>
		/// </returns>
		public static WWW LoadFromCacheOrDownload(string url, Hash128 hash, uint crc)
		{
			return new WWW(url, "", hash, crc);
		}

		/// <summary>
		///   <para>Loads an AssetBundle with the specified version number from the cache. If the AssetBundle is not currently cached, it will automatically be downloaded and stored in the cache for future retrieval from local storage.</para>
		/// </summary>
		/// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
		/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBundle from url.</param>
		/// <param name="hash">Hash128 which is used as the version of the AssetBundle.</param>
		/// <param name="cachedBundle">A structure used to download a given version of AssetBundle to a customized cache path.
		///
		/// Analogous to the cachedAssetBundle parameter for UnityWebRequestAssetBundle.GetAssetBundle.&lt;/param&gt;</param>
		/// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
		/// <returns>
		///   <para>A WWW instance, which can be used to access the data once the load/download operation is completed.</para>
		/// </returns>
		public static WWW LoadFromCacheOrDownload(string url, CachedAssetBundle cachedBundle, uint crc = 0U)
		{
			return new WWW(url, cachedBundle.name, cachedBundle.hash, crc);
		}

		/// <summary>
		///   <para>Streams an AssetBundle that can contain any kind of asset from the project folder.</para>
		/// </summary>
		public AssetBundle assetBundle
		{
			get
			{
				if (this._assetBundle == null)
				{
					if (!this.WaitUntilDoneIfPossible())
					{
						return null;
					}
					if (this._uwr.isNetworkError)
					{
						return null;
					}
					DownloadHandlerAssetBundle downloadHandlerAssetBundle = this._uwr.downloadHandler as DownloadHandlerAssetBundle;
					if (downloadHandlerAssetBundle != null)
					{
						this._assetBundle = downloadHandlerAssetBundle.assetBundle;
					}
					else
					{
						byte[] bytes = this.bytes;
						if (bytes == null)
						{
							return null;
						}
						this._assetBundle = AssetBundle.LoadFromMemory(bytes);
					}
				}
				return this._assetBundle;
			}
		}

		/// <summary>
		///   <para>Returns a AudioClip generated from the downloaded data (Read Only).</para>
		/// </summary>
		[Obsolete("Obsolete msg (UnityUpgradable) -> * UnityEngine.WWW.GetAudioClip()", true)]
		public Object audioClip
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		///   <para>Returns the contents of the fetched web page as a byte array (Read Only).</para>
		/// </summary>
		public byte[] bytes
		{
			get
			{
				byte[] array;
				if (!this.WaitUntilDoneIfPossible())
				{
					array = new byte[0];
				}
				else if (this._uwr.isNetworkError)
				{
					array = new byte[0];
				}
				else
				{
					DownloadHandler downloadHandler = this._uwr.downloadHandler;
					if (downloadHandler == null)
					{
						array = new byte[0];
					}
					else
					{
						array = downloadHandler.data;
					}
				}
				return array;
			}
		}

		/// <summary>
		///   <para>Returns a MovieTexture generated from the downloaded data (Read Only).</para>
		/// </summary>
		[Obsolete("Obsolete msg (UnityUpgradable) -> * UnityEngine.WWW.GetMovieTexture()", true)]
		public Object movie
		{
			get
			{
				return null;
			}
		}

		[Obsolete("WWW.size is obsolete. Please use WWW.bytesDownloaded instead")]
		public int size
		{
			get
			{
				return this.bytesDownloaded;
			}
		}

		/// <summary>
		///   <para>The number of bytes downloaded by this WWW query (read only).</para>
		/// </summary>
		public int bytesDownloaded
		{
			get
			{
				return (int)this._uwr.downloadedBytes;
			}
		}

		/// <summary>
		///   <para>Returns an error message if there was an error during the download (Read Only).</para>
		/// </summary>
		public string error
		{
			get
			{
				string text;
				if (!this._uwr.isDone)
				{
					text = null;
				}
				else if (this._uwr.isNetworkError)
				{
					text = this._uwr.error;
				}
				else if (this._uwr.responseCode >= 400L)
				{
					text = string.Format("{0} {1}", this._uwr.responseCode, this.GetStatusCodeName(this._uwr.responseCode));
				}
				else
				{
					text = null;
				}
				return text;
			}
		}

		/// <summary>
		///   <para>Is the download already finished? (Read Only)</para>
		/// </summary>
		public bool isDone
		{
			get
			{
				return this._uwr.isDone;
			}
		}

		/// <summary>
		///   <para>How far has the download progressed (Read Only).</para>
		/// </summary>
		public float progress
		{
			get
			{
				float num = this._uwr.downloadProgress;
				if (num < 0f)
				{
					num = 0f;
				}
				return num;
			}
		}

		/// <summary>
		///   <para>Dictionary of headers returned by the request.</para>
		/// </summary>
		public Dictionary<string, string> responseHeaders
		{
			get
			{
				Dictionary<string, string> dictionary;
				if (!this.isDone)
				{
					dictionary = new Dictionary<string, string>();
				}
				else
				{
					if (this._responseHeaders == null)
					{
						this._responseHeaders = this._uwr.GetResponseHeaders();
						if (this._responseHeaders != null)
						{
							this._responseHeaders["STATUS"] = string.Format("HTTP/1.1 {0} {1}", this._uwr.responseCode, this.GetStatusCodeName(this._uwr.responseCode));
						}
						else
						{
							this._responseHeaders = new Dictionary<string, string>();
						}
					}
					dictionary = this._responseHeaders;
				}
				return dictionary;
			}
		}

		[Obsolete("Please use WWW.text instead. (UnityUpgradable) -> text", true)]
		public string data
		{
			get
			{
				return this.text;
			}
		}

		/// <summary>
		///   <para>Returns the contents of the fetched web page as a string (Read Only).</para>
		/// </summary>
		public string text
		{
			get
			{
				string text;
				if (!this.WaitUntilDoneIfPossible())
				{
					text = "";
				}
				else if (this._uwr.isNetworkError)
				{
					text = "";
				}
				else
				{
					DownloadHandler downloadHandler = this._uwr.downloadHandler;
					if (downloadHandler == null)
					{
						text = "";
					}
					else
					{
						text = downloadHandler.text;
					}
				}
				return text;
			}
		}

		private Texture2D CreateTextureFromDownloadedData(bool markNonReadable)
		{
			Texture2D texture2D;
			if (!this.WaitUntilDoneIfPossible())
			{
				texture2D = new Texture2D(2, 2);
			}
			else if (this._uwr.isNetworkError)
			{
				texture2D = null;
			}
			else
			{
				DownloadHandler downloadHandler = this._uwr.downloadHandler;
				if (downloadHandler == null)
				{
					texture2D = null;
				}
				else
				{
					Texture2D texture2D2 = new Texture2D(2, 2);
					texture2D2.LoadImage(downloadHandler.data, markNonReadable);
					texture2D = texture2D2;
				}
			}
			return texture2D;
		}

		/// <summary>
		///   <para>Returns a Texture2D generated from the downloaded data (Read Only).</para>
		/// </summary>
		public Texture2D texture
		{
			get
			{
				return this.CreateTextureFromDownloadedData(false);
			}
		}

		/// <summary>
		///   <para>Returns a non-readable Texture2D generated from the downloaded data (Read Only).</para>
		/// </summary>
		public Texture2D textureNonReadable
		{
			get
			{
				return this.CreateTextureFromDownloadedData(true);
			}
		}

		/// <summary>
		///   <para>Replaces the contents of an existing Texture2D with an image from the downloaded data.</para>
		/// </summary>
		/// <param name="tex">An existing texture object to be overwritten with the image data.</param>
		/// <param name="texture"></param>
		public void LoadImageIntoTexture(Texture2D texture)
		{
			if (this.WaitUntilDoneIfPossible())
			{
				if (this._uwr.isNetworkError)
				{
					Debug.LogError("Cannot load image: download failed");
				}
				else
				{
					DownloadHandler downloadHandler = this._uwr.downloadHandler;
					if (downloadHandler == null)
					{
						Debug.LogError("Cannot load image: internal error");
					}
					else
					{
						texture.LoadImage(downloadHandler.data, false);
					}
				}
			}
		}

		/// <summary>
		///   <para>Obsolete, has no effect.</para>
		/// </summary>
		public ThreadPriority threadPriority { get; set; }

		/// <summary>
		///   <para>How far has the upload progressed (Read Only).</para>
		/// </summary>
		public float uploadProgress
		{
			get
			{
				float num = this._uwr.uploadProgress;
				if (num < 0f)
				{
					num = 0f;
				}
				return num;
			}
		}

		/// <summary>
		///   <para>The URL of this WWW request (Read Only).</para>
		/// </summary>
		public string url
		{
			get
			{
				return this._uwr.url;
			}
		}

		public override bool keepWaiting
		{
			get
			{
				return !this._uwr.isDone;
			}
		}

		/// <summary>
		///   <para>Disposes of an existing WWW object.</para>
		/// </summary>
		public void Dispose()
		{
			this._uwr.Dispose();
		}

		internal Object GetAudioClipInternal(bool threeD, bool stream, bool compressed, AudioType audioType)
		{
			return WebRequestWWW.InternalCreateAudioClipUsingDH(this._uwr.downloadHandler, this._uwr.url, stream, compressed, audioType);
		}

		[Obsolete("MovieTexture is deprecated. Use VideoPlayer instead.", false)]
		internal object GetMovieTextureInternal()
		{
			return WebRequestWWW.InternalCreateMovieTextureUsingDH(this._uwr.downloadHandler);
		}

		public AudioClip GetAudioClip()
		{
			return this.GetAudioClip(true, false, AudioType.UNKNOWN);
		}

		/// <summary>
		///   <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
		/// </summary>
		/// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip
		/// the .audioClip property defaults to 3D.</param>
		/// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false) or the stream can be played even if only part of the clip is downloaded (true).
		/// The latter will disable seeking on the clip (with .time and/or .timeSamples).</param>
		/// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
		/// <returns>
		///   <para>The returned AudioClip.</para>
		/// </returns>
		public AudioClip GetAudioClip(bool threeD)
		{
			return this.GetAudioClip(threeD, false, AudioType.UNKNOWN);
		}

		/// <summary>
		///   <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
		/// </summary>
		/// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip
		/// the .audioClip property defaults to 3D.</param>
		/// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false) or the stream can be played even if only part of the clip is downloaded (true).
		/// The latter will disable seeking on the clip (with .time and/or .timeSamples).</param>
		/// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
		/// <returns>
		///   <para>The returned AudioClip.</para>
		/// </returns>
		public AudioClip GetAudioClip(bool threeD, bool stream)
		{
			return this.GetAudioClip(threeD, stream, AudioType.UNKNOWN);
		}

		/// <summary>
		///   <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
		/// </summary>
		/// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip
		/// the .audioClip property defaults to 3D.</param>
		/// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false) or the stream can be played even if only part of the clip is downloaded (true).
		/// The latter will disable seeking on the clip (with .time and/or .timeSamples).</param>
		/// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
		/// <returns>
		///   <para>The returned AudioClip.</para>
		/// </returns>
		public AudioClip GetAudioClip(bool threeD, bool stream, AudioType audioType)
		{
			return (AudioClip)this.GetAudioClipInternal(threeD, stream, false, audioType);
		}

		/// <summary>
		///   <para>Returns an AudioClip generated from the downloaded data that is compressed in memory (Read Only).</para>
		/// </summary>
		/// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.</param>
		/// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
		/// <returns>
		///   <para>The returned AudioClip.</para>
		/// </returns>
		public AudioClip GetAudioClipCompressed()
		{
			return this.GetAudioClipCompressed(false, AudioType.UNKNOWN);
		}

		/// <summary>
		///   <para>Returns an AudioClip generated from the downloaded data that is compressed in memory (Read Only).</para>
		/// </summary>
		/// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.</param>
		/// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
		/// <returns>
		///   <para>The returned AudioClip.</para>
		/// </returns>
		public AudioClip GetAudioClipCompressed(bool threeD)
		{
			return this.GetAudioClipCompressed(threeD, AudioType.UNKNOWN);
		}

		/// <summary>
		///   <para>Returns an AudioClip generated from the downloaded data that is compressed in memory (Read Only).</para>
		/// </summary>
		/// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.</param>
		/// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
		/// <returns>
		///   <para>The returned AudioClip.</para>
		/// </returns>
		public AudioClip GetAudioClipCompressed(bool threeD, AudioType audioType)
		{
			return (AudioClip)this.GetAudioClipInternal(threeD, false, true, audioType);
		}

		/// <summary>
		///   <para>MovieTexture has been deprecated. Refer to the new movie playback solution VideoPlayer.</para>
		/// </summary>
		[Obsolete("MovieTexture is deprecated. Use VideoPlayer instead.", false)]
		public MovieTexture GetMovieTexture()
		{
			return (MovieTexture)this.GetMovieTextureInternal();
		}

		private bool WaitUntilDoneIfPossible()
		{
			bool flag;
			if (this._uwr.isDone)
			{
				flag = true;
			}
			else if (this.url.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
			{
				while (!this._uwr.isDone)
				{
				}
				flag = true;
			}
			else
			{
				Debug.LogError("You are trying to load data from a www stream which has not completed the download yet.\nYou need to yield the download or wait until isDone returns true.");
				flag = false;
			}
			return flag;
		}

		private string GetStatusCodeName(long statusCode)
		{
			if (statusCode >= 400L && statusCode <= 416L)
			{
				switch ((int)(statusCode - 400L))
				{
				case 0:
					return "Bad Request";
				case 1:
					return "Unauthorized";
				case 2:
					return "Payment Required";
				case 3:
					return "Forbidden";
				case 4:
					return "Not Found";
				case 5:
					return "Method Not Allowed";
				case 6:
					return "Not Acceptable";
				case 7:
					return "Proxy Authentication Required";
				case 8:
					return "Request Timeout";
				case 9:
					return "Conflict";
				case 10:
					return "Gone";
				case 11:
					return "Length Required";
				case 12:
					return "Precondition Failed";
				case 13:
					return "Request Entity Too Large";
				case 14:
					return "Request-URI Too Long";
				case 15:
					return "Unsupported Media Type";
				case 16:
					return "Requested Range Not Satisfiable";
				}
			}
			if (statusCode >= 200L && statusCode <= 206L)
			{
				switch ((int)(statusCode - 200L))
				{
				case 0:
					return "OK";
				case 1:
					return "Created";
				case 2:
					return "Accepted";
				case 3:
					return "Non-Authoritative Information";
				case 4:
					return "No Content";
				case 5:
					return "Reset Content";
				case 6:
					return "Partial Content";
				}
			}
			if (statusCode >= 300L && statusCode <= 307L)
			{
				switch ((int)(statusCode - 300L))
				{
				case 0:
					return "Multiple Choices";
				case 1:
					return "Moved Permanently";
				case 2:
					return "Found";
				case 3:
					return "See Other";
				case 4:
					return "Not Modified";
				case 5:
					return "Use Proxy";
				case 7:
					return "Temporary Redirect";
				}
			}
			if (statusCode >= 500L && statusCode <= 505L)
			{
				switch ((int)(statusCode - 500L))
				{
				case 0:
					return "Internal Server Error";
				case 1:
					return "Not Implemented";
				case 2:
					return "Bad Gateway";
				case 3:
					return "Service Unavailable";
				case 4:
					return "Gateway Timeout";
				case 5:
					return "HTTP Version Not Supported";
				}
			}
			string text;
			if (statusCode != 41L)
			{
				text = "";
			}
			else
			{
				text = "Expectation Failed";
			}
			return text;
		}

		private UnityWebRequest _uwr;

		private AssetBundle _assetBundle;

		private Dictionary<string, string> _responseHeaders;
	}
}
