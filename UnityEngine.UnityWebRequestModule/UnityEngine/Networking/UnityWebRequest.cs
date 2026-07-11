using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Bindings;
using UnityEngineInternal;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequest/Public/UnityWebRequest.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class UnityWebRequest : IDisposable
	{
		public UnityWebRequest()
		{
			this.m_Ptr = UnityWebRequest.Create();
			this.InternalSetDefaults();
		}

		public UnityWebRequest(string url)
		{
			this.m_Ptr = UnityWebRequest.Create();
			this.InternalSetDefaults();
			this.url = url;
		}

		public UnityWebRequest(Uri uri)
		{
			this.m_Ptr = UnityWebRequest.Create();
			this.InternalSetDefaults();
			this.uri = uri;
		}

		public UnityWebRequest(string url, string method)
		{
			this.m_Ptr = UnityWebRequest.Create();
			this.InternalSetDefaults();
			this.url = url;
			this.method = method;
		}

		public UnityWebRequest(Uri uri, string method)
		{
			this.m_Ptr = UnityWebRequest.Create();
			this.InternalSetDefaults();
			this.uri = uri;
			this.method = method;
		}

		public UnityWebRequest(string url, string method, DownloadHandler downloadHandler, UploadHandler uploadHandler)
		{
			this.m_Ptr = UnityWebRequest.Create();
			this.InternalSetDefaults();
			this.url = url;
			this.method = method;
			this.downloadHandler = downloadHandler;
			this.uploadHandler = uploadHandler;
		}

		public UnityWebRequest(Uri uri, string method, DownloadHandler downloadHandler, UploadHandler uploadHandler)
		{
			this.m_Ptr = UnityWebRequest.Create();
			this.InternalSetDefaults();
			this.uri = uri;
			this.method = method;
			this.downloadHandler = downloadHandler;
			this.uploadHandler = uploadHandler;
		}

		[NativeMethod(IsThreadSafe = true)]
		[NativeConditional("ENABLE_UNITYWEBREQUEST")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetWebErrorString(UnityWebRequest.UnityWebRequestError err);

		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetHTTPStatusString(long responseCode);

		public bool disposeCertificateHandlerOnDispose { get; set; }

		public bool disposeDownloadHandlerOnDispose { get; set; }

		public bool disposeUploadHandlerOnDispose { get; set; }

		public static void ClearCookieCache()
		{
			UnityWebRequest.ClearCookieCache(null, null);
		}

		public static void ClearCookieCache(Uri uri)
		{
			if (uri == null)
			{
				UnityWebRequest.ClearCookieCache(null, null);
			}
			else
			{
				string host = uri.Host;
				string text = uri.AbsolutePath;
				if (text == "/")
				{
					text = null;
				}
				UnityWebRequest.ClearCookieCache(host, text);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearCookieCache(string domain, string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr Create();

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Release();

		internal void InternalDestroy()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				this.Abort();
				this.Release();
				this.m_Ptr = IntPtr.Zero;
			}
		}

		private void InternalSetDefaults()
		{
			this.disposeDownloadHandlerOnDispose = true;
			this.disposeUploadHandlerOnDispose = true;
			this.disposeCertificateHandlerOnDispose = true;
		}

		~UnityWebRequest()
		{
			this.DisposeHandlers();
			this.InternalDestroy();
		}

		public void Dispose()
		{
			this.DisposeHandlers();
			this.InternalDestroy();
			GC.SuppressFinalize(this);
		}

		private void DisposeHandlers()
		{
			if (this.disposeDownloadHandlerOnDispose)
			{
				DownloadHandler downloadHandler = this.downloadHandler;
				if (downloadHandler != null)
				{
					downloadHandler.Dispose();
				}
			}
			if (this.disposeUploadHandlerOnDispose)
			{
				UploadHandler uploadHandler = this.uploadHandler;
				if (uploadHandler != null)
				{
					uploadHandler.Dispose();
				}
			}
			if (this.disposeCertificateHandlerOnDispose)
			{
				CertificateHandler certificateHandler = this.certificateHandler;
				if (certificateHandler != null)
				{
					certificateHandler.Dispose();
				}
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern UnityWebRequestAsyncOperation BeginWebRequest();

		[Obsolete("Use SendWebRequest.  It returns a UnityWebRequestAsyncOperation which contains a reference to the WebRequest object.", false)]
		public AsyncOperation Send()
		{
			return this.SendWebRequest();
		}

		public UnityWebRequestAsyncOperation SendWebRequest()
		{
			UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = this.BeginWebRequest();
			if (unityWebRequestAsyncOperation != null)
			{
				unityWebRequestAsyncOperation.webRequest = this;
			}
			return unityWebRequestAsyncOperation;
		}

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Abort();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern UnityWebRequest.UnityWebRequestError SetMethod(UnityWebRequest.UnityWebRequestMethod methodType);

		internal void InternalSetMethod(UnityWebRequest.UnityWebRequestMethod methodType)
		{
			if (!this.isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent and its request method can no longer be altered");
			}
			UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetMethod(methodType);
			if (unityWebRequestError != UnityWebRequest.UnityWebRequestError.OK)
			{
				throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern UnityWebRequest.UnityWebRequestError SetCustomMethod(string customMethodName);

		internal void InternalSetCustomMethod(string customMethodName)
		{
			if (!this.isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent and its request method can no longer be altered");
			}
			UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetCustomMethod(customMethodName);
			if (unityWebRequestError != UnityWebRequest.UnityWebRequestError.OK)
			{
				throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern UnityWebRequest.UnityWebRequestMethod GetMethod();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetCustomMethod();

		public string method
		{
			get
			{
				string text;
				switch (this.GetMethod())
				{
				case UnityWebRequest.UnityWebRequestMethod.Get:
					text = "GET";
					break;
				case UnityWebRequest.UnityWebRequestMethod.Post:
					text = "POST";
					break;
				case UnityWebRequest.UnityWebRequestMethod.Put:
					text = "PUT";
					break;
				case UnityWebRequest.UnityWebRequestMethod.Head:
					text = "HEAD";
					break;
				default:
					text = this.GetCustomMethod();
					break;
				}
				return text;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Cannot set a UnityWebRequest's method to an empty or null string");
				}
				string text = value.ToUpper();
				if (text != null)
				{
					if (text == "GET")
					{
						this.InternalSetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
						return;
					}
					if (text == "POST")
					{
						this.InternalSetMethod(UnityWebRequest.UnityWebRequestMethod.Post);
						return;
					}
					if (text == "PUT")
					{
						this.InternalSetMethod(UnityWebRequest.UnityWebRequestMethod.Put);
						return;
					}
					if (text == "HEAD")
					{
						this.InternalSetMethod(UnityWebRequest.UnityWebRequestMethod.Head);
						return;
					}
				}
				this.InternalSetCustomMethod(value.ToUpper());
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern UnityWebRequest.UnityWebRequestError GetError();

		public string error
		{
			get
			{
				string text;
				if (!this.isNetworkError && !this.isHttpError)
				{
					text = null;
				}
				else if (this.isHttpError)
				{
					string httpstatusString = UnityWebRequest.GetHTTPStatusString(this.responseCode);
					text = string.Format("HTTP/1.1 {0} {1}", this.responseCode, httpstatusString);
				}
				else
				{
					text = UnityWebRequest.GetWebErrorString(this.GetError());
				}
				return text;
			}
		}

		private extern bool use100Continue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public bool useHttpContinue
		{
			get
			{
				return this.use100Continue;
			}
			set
			{
				if (!this.isModifiable)
				{
					throw new InvalidOperationException("UnityWebRequest has already been sent and its 100-Continue setting cannot be altered");
				}
				this.use100Continue = value;
			}
		}

		public string url
		{
			get
			{
				return this.GetUrl();
			}
			set
			{
				string text = "http://localhost/";
				this.InternalSetUrl(WebRequestUtils.MakeInitialUrl(value, text));
			}
		}

		public Uri uri
		{
			get
			{
				return new Uri(this.GetUrl());
			}
			set
			{
				if (!value.IsAbsoluteUri)
				{
					throw new ArgumentException("URI must be absolute");
				}
				this.InternalSetUrl(WebRequestUtils.MakeUriString(value, value.OriginalString, false));
				this.m_Uri = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string GetUrl();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern UnityWebRequest.UnityWebRequestError SetUrl(string url);

		private void InternalSetUrl(string url)
		{
			if (!this.isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent and its URL cannot be altered");
			}
			UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetUrl(url);
			if (unityWebRequestError != UnityWebRequest.UnityWebRequestError.OK)
			{
				throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
			}
		}

		public extern long responseCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetUploadProgress();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsExecuting();

		public float uploadProgress
		{
			get
			{
				float num;
				if (!this.IsExecuting() && !this.isDone)
				{
					num = -1f;
				}
				else
				{
					num = this.GetUploadProgress();
				}
				return num;
			}
		}

		public extern bool isModifiable
		{
			[NativeMethod("IsModifiable")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isDone
		{
			[NativeMethod("IsDone")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isNetworkError
		{
			[NativeMethod("IsNetworkError")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isHttpError
		{
			[NativeMethod("IsHttpError")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetDownloadProgress();

		public float downloadProgress
		{
			get
			{
				float num;
				if (!this.IsExecuting() && !this.isDone)
				{
					num = -1f;
				}
				else
				{
					num = this.GetDownloadProgress();
				}
				return num;
			}
		}

		public extern ulong uploadedBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ulong downloadedBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetRedirectLimit();

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRedirectLimitFromScripting(int limit);

		public int redirectLimit
		{
			get
			{
				return this.GetRedirectLimit();
			}
			set
			{
				this.SetRedirectLimitFromScripting(value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetChunked();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern UnityWebRequest.UnityWebRequestError SetChunked(bool chunked);

		public bool chunkedTransfer
		{
			get
			{
				return this.GetChunked();
			}
			set
			{
				if (!this.isModifiable)
				{
					throw new InvalidOperationException("UnityWebRequest has already been sent and its chunked transfer encoding setting cannot be altered");
				}
				UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetChunked(value);
				if (unityWebRequestError != UnityWebRequest.UnityWebRequestError.OK)
				{
					throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetRequestHeader(string name);

		[NativeMethod("SetRequestHeader")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern UnityWebRequest.UnityWebRequestError InternalSetRequestHeader(string name, string value);

		public void SetRequestHeader(string name, string value)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Cannot set a Request Header with a null or empty name");
			}
			if (value == null)
			{
				throw new ArgumentException("Cannot set a Request header with a null");
			}
			if (!this.isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent and its request headers cannot be altered");
			}
			UnityWebRequest.UnityWebRequestError unityWebRequestError = this.InternalSetRequestHeader(name, value);
			if (unityWebRequestError != UnityWebRequest.UnityWebRequestError.OK)
			{
				throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetResponseHeader(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string[] GetResponseHeaderKeys();

		public Dictionary<string, string> GetResponseHeaders()
		{
			string[] responseHeaderKeys = this.GetResponseHeaderKeys();
			Dictionary<string, string> dictionary;
			if (responseHeaderKeys == null || responseHeaderKeys.Length == 0)
			{
				dictionary = null;
			}
			else
			{
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>(responseHeaderKeys.Length, StringComparer.OrdinalIgnoreCase);
				for (int i = 0; i < responseHeaderKeys.Length; i++)
				{
					string responseHeader = this.GetResponseHeader(responseHeaderKeys[i]);
					dictionary2.Add(responseHeaderKeys[i], responseHeader);
				}
				dictionary = dictionary2;
			}
			return dictionary;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern UnityWebRequest.UnityWebRequestError SetUploadHandler(UploadHandler uh);

		public UploadHandler uploadHandler
		{
			get
			{
				return this.m_UploadHandler;
			}
			set
			{
				if (!this.isModifiable)
				{
					throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the upload handler");
				}
				UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetUploadHandler(value);
				if (unityWebRequestError != UnityWebRequest.UnityWebRequestError.OK)
				{
					throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
				}
				this.m_UploadHandler = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern UnityWebRequest.UnityWebRequestError SetDownloadHandler(DownloadHandler dh);

		public DownloadHandler downloadHandler
		{
			get
			{
				return this.m_DownloadHandler;
			}
			set
			{
				if (!this.isModifiable)
				{
					throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the download handler");
				}
				UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetDownloadHandler(value);
				if (unityWebRequestError != UnityWebRequest.UnityWebRequestError.OK)
				{
					throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
				}
				this.m_DownloadHandler = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern UnityWebRequest.UnityWebRequestError SetCertificateHandler(CertificateHandler ch);

		public CertificateHandler certificateHandler
		{
			get
			{
				return this.m_CertificateHandler;
			}
			set
			{
				if (!this.isModifiable)
				{
					throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the certificate handler");
				}
				UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetCertificateHandler(value);
				if (unityWebRequestError != UnityWebRequest.UnityWebRequestError.OK)
				{
					throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
				}
				this.m_CertificateHandler = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetTimeoutMsec();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern UnityWebRequest.UnityWebRequestError SetTimeoutMsec(int timeout);

		public int timeout
		{
			get
			{
				return this.GetTimeoutMsec() / 1000;
			}
			set
			{
				if (!this.isModifiable)
				{
					throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the timeout");
				}
				value = Math.Max(value, 0);
				UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetTimeoutMsec(value * 1000);
				if (unityWebRequestError != UnityWebRequest.UnityWebRequestError.OK)
				{
					throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
				}
			}
		}

		public static UnityWebRequest Get(string uri)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerBuffer(), null);
		}

		public static UnityWebRequest Get(Uri uri)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerBuffer(), null);
		}

		public static UnityWebRequest Delete(string uri)
		{
			return new UnityWebRequest(uri, "DELETE");
		}

		public static UnityWebRequest Delete(Uri uri)
		{
			return new UnityWebRequest(uri, "DELETE");
		}

		public static UnityWebRequest Head(string uri)
		{
			return new UnityWebRequest(uri, "HEAD");
		}

		public static UnityWebRequest Head(Uri uri)
		{
			return new UnityWebRequest(uri, "HEAD");
		}

		[Obsolete("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestTexture.GetTexture(*)", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static UnityWebRequest GetTexture(string uri)
		{
			throw new NotSupportedException("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead.");
		}

		[Obsolete("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestTexture.GetTexture(*)", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static UnityWebRequest GetTexture(string uri, bool nonReadable)
		{
			throw new NotSupportedException("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead.");
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("UnityWebRequest.GetAudioClip is obsolete. Use UnityWebRequestMultimedia.GetAudioClip instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestMultimedia.GetAudioClip(*)", true)]
		public static UnityWebRequest GetAudioClip(string uri, AudioType audioType)
		{
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
		public static UnityWebRequest GetAssetBundle(string uri)
		{
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
		public static UnityWebRequest GetAssetBundle(string uri, uint crc)
		{
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
		public static UnityWebRequest GetAssetBundle(string uri, uint version, uint crc)
		{
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
		public static UnityWebRequest GetAssetBundle(string uri, Hash128 hash, uint crc)
		{
			return null;
		}

		[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static UnityWebRequest GetAssetBundle(string uri, CachedAssetBundle cachedAssetBundle, uint crc)
		{
			return null;
		}

		public static UnityWebRequest Put(string uri, byte[] bodyData)
		{
			return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(bodyData));
		}

		public static UnityWebRequest Put(Uri uri, byte[] bodyData)
		{
			return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(bodyData));
		}

		public static UnityWebRequest Put(string uri, string bodyData)
		{
			return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData)));
		}

		public static UnityWebRequest Put(Uri uri, string bodyData)
		{
			return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData)));
		}

		public static UnityWebRequest Post(string uri, string postData)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			UnityWebRequest.SetupPost(unityWebRequest, postData);
			return unityWebRequest;
		}

		public static UnityWebRequest Post(Uri uri, string postData)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			UnityWebRequest.SetupPost(unityWebRequest, postData);
			return unityWebRequest;
		}

		private static void SetupPost(UnityWebRequest request, string postData)
		{
			byte[] array = null;
			if (!string.IsNullOrEmpty(postData))
			{
				string text = WWWTranscoder.DataEncode(postData, Encoding.UTF8);
				array = Encoding.UTF8.GetBytes(text);
			}
			request.uploadHandler = new UploadHandlerRaw(array);
			request.uploadHandler.contentType = "application/x-www-form-urlencoded";
			request.downloadHandler = new DownloadHandlerBuffer();
		}

		public static UnityWebRequest Post(string uri, WWWForm formData)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			UnityWebRequest.SetupPost(unityWebRequest, formData);
			return unityWebRequest;
		}

		public static UnityWebRequest Post(Uri uri, WWWForm formData)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			UnityWebRequest.SetupPost(unityWebRequest, formData);
			return unityWebRequest;
		}

		private static void SetupPost(UnityWebRequest request, WWWForm formData)
		{
			byte[] array = null;
			if (formData != null)
			{
				array = formData.data;
				if (array.Length == 0)
				{
					array = null;
				}
			}
			request.uploadHandler = new UploadHandlerRaw(array);
			request.downloadHandler = new DownloadHandlerBuffer();
			if (formData != null)
			{
				Dictionary<string, string> headers = formData.headers;
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					request.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		public static UnityWebRequest Post(string uri, List<IMultipartFormSection> multipartFormSections)
		{
			byte[] array = UnityWebRequest.GenerateBoundary();
			return UnityWebRequest.Post(uri, multipartFormSections, array);
		}

		public static UnityWebRequest Post(Uri uri, List<IMultipartFormSection> multipartFormSections)
		{
			byte[] array = UnityWebRequest.GenerateBoundary();
			return UnityWebRequest.Post(uri, multipartFormSections, array);
		}

		public static UnityWebRequest Post(string uri, List<IMultipartFormSection> multipartFormSections, byte[] boundary)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			UnityWebRequest.SetupPost(unityWebRequest, multipartFormSections, boundary);
			return unityWebRequest;
		}

		public static UnityWebRequest Post(Uri uri, List<IMultipartFormSection> multipartFormSections, byte[] boundary)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			UnityWebRequest.SetupPost(unityWebRequest, multipartFormSections, boundary);
			return unityWebRequest;
		}

		private static void SetupPost(UnityWebRequest request, List<IMultipartFormSection> multipartFormSections, byte[] boundary)
		{
			byte[] array = null;
			if (multipartFormSections != null && multipartFormSections.Count != 0)
			{
				array = UnityWebRequest.SerializeFormSections(multipartFormSections, boundary);
			}
			request.uploadHandler = new UploadHandlerRaw(array)
			{
				contentType = "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary, 0, boundary.Length)
			};
			request.downloadHandler = new DownloadHandlerBuffer();
		}

		public static UnityWebRequest Post(string uri, Dictionary<string, string> formFields)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			UnityWebRequest.SetupPost(unityWebRequest, formFields);
			return unityWebRequest;
		}

		public static UnityWebRequest Post(Uri uri, Dictionary<string, string> formFields)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			UnityWebRequest.SetupPost(unityWebRequest, formFields);
			return unityWebRequest;
		}

		private static void SetupPost(UnityWebRequest request, Dictionary<string, string> formFields)
		{
			byte[] array = null;
			if (formFields != null && formFields.Count != 0)
			{
				array = UnityWebRequest.SerializeSimpleForm(formFields);
			}
			request.uploadHandler = new UploadHandlerRaw(array)
			{
				contentType = "application/x-www-form-urlencoded"
			};
			request.downloadHandler = new DownloadHandlerBuffer();
		}

		public static string EscapeURL(string s)
		{
			return UnityWebRequest.EscapeURL(s, Encoding.UTF8);
		}

		public static string EscapeURL(string s, Encoding e)
		{
			string text;
			if (s == null)
			{
				text = null;
			}
			else if (s == "")
			{
				text = "";
			}
			else if (e == null)
			{
				text = null;
			}
			else
			{
				byte[] bytes = e.GetBytes(s);
				byte[] array = WWWTranscoder.URLEncode(bytes);
				text = e.GetString(array);
			}
			return text;
		}

		public static string UnEscapeURL(string s)
		{
			return UnityWebRequest.UnEscapeURL(s, Encoding.UTF8);
		}

		public static string UnEscapeURL(string s, Encoding e)
		{
			string text;
			if (s == null)
			{
				text = null;
			}
			else if (s.IndexOf('%') == -1 && s.IndexOf('+') == -1)
			{
				text = s;
			}
			else
			{
				byte[] bytes = e.GetBytes(s);
				byte[] array = WWWTranscoder.URLDecode(bytes);
				text = e.GetString(array);
			}
			return text;
		}

		public static byte[] SerializeFormSections(List<IMultipartFormSection> multipartFormSections, byte[] boundary)
		{
			byte[] array;
			if (multipartFormSections == null || multipartFormSections.Count == 0)
			{
				array = null;
			}
			else
			{
				byte[] bytes = Encoding.UTF8.GetBytes("\r\n");
				byte[] bytes2 = WWWForm.DefaultEncoding.GetBytes("--");
				int num = 0;
				foreach (IMultipartFormSection multipartFormSection in multipartFormSections)
				{
					num += 64 + multipartFormSection.sectionData.Length;
				}
				List<byte> list = new List<byte>(num);
				foreach (IMultipartFormSection multipartFormSection2 in multipartFormSections)
				{
					string text = "form-data";
					string sectionName = multipartFormSection2.sectionName;
					string fileName = multipartFormSection2.fileName;
					string text2 = "Content-Disposition: " + text;
					if (!string.IsNullOrEmpty(sectionName))
					{
						text2 = text2 + "; name=\"" + sectionName + "\"";
					}
					if (!string.IsNullOrEmpty(fileName))
					{
						text2 = text2 + "; filename=\"" + fileName + "\"";
					}
					text2 += "\r\n";
					string contentType = multipartFormSection2.contentType;
					if (!string.IsNullOrEmpty(contentType))
					{
						text2 = text2 + "Content-Type: " + contentType + "\r\n";
					}
					list.AddRange(bytes);
					list.AddRange(bytes2);
					list.AddRange(boundary);
					list.AddRange(bytes);
					list.AddRange(Encoding.UTF8.GetBytes(text2));
					list.AddRange(bytes);
					list.AddRange(multipartFormSection2.sectionData);
				}
				list.AddRange(bytes);
				list.AddRange(bytes2);
				list.AddRange(boundary);
				list.AddRange(bytes2);
				list.AddRange(bytes);
				array = list.ToArray();
			}
			return array;
		}

		public static byte[] GenerateBoundary()
		{
			byte[] array = new byte[40];
			for (int i = 0; i < 40; i++)
			{
				int num = Random.Range(48, 110);
				if (num > 57)
				{
					num += 7;
				}
				if (num > 90)
				{
					num += 6;
				}
				array[i] = (byte)num;
			}
			return array;
		}

		public static byte[] SerializeSimpleForm(Dictionary<string, string> formFields)
		{
			string text = "";
			foreach (KeyValuePair<string, string> keyValuePair in formFields)
			{
				if (text.Length > 0)
				{
					text += "&";
				}
				text = text + WWWTranscoder.DataEncode(keyValuePair.Key) + "=" + WWWTranscoder.DataEncode(keyValuePair.Value);
			}
			return Encoding.UTF8.GetBytes(text);
		}

		[NonSerialized]
		internal IntPtr m_Ptr;

		[NonSerialized]
		internal DownloadHandler m_DownloadHandler;

		[NonSerialized]
		internal UploadHandler m_UploadHandler;

		[NonSerialized]
		internal CertificateHandler m_CertificateHandler;

		[NonSerialized]
		internal Uri m_Uri;

		public const string kHttpVerbGET = "GET";

		public const string kHttpVerbHEAD = "HEAD";

		public const string kHttpVerbPOST = "POST";

		public const string kHttpVerbPUT = "PUT";

		public const string kHttpVerbCREATE = "CREATE";

		public const string kHttpVerbDELETE = "DELETE";

		internal enum UnityWebRequestMethod
		{
			Get,
			Post,
			Put,
			Head,
			Custom
		}

		internal enum UnityWebRequestError
		{
			OK,
			Unknown,
			SDKError,
			UnsupportedProtocol,
			MalformattedUrl,
			CannotResolveProxy,
			CannotResolveHost,
			CannotConnectToHost,
			AccessDenied,
			GenericHttpError,
			WriteError,
			ReadError,
			OutOfMemory,
			Timeout,
			HTTPPostError,
			SSLCannotConnect,
			Aborted,
			TooManyRedirects,
			ReceivedNoData,
			SSLNotSupported,
			FailedToSendData,
			FailedToReceiveData,
			SSLCertificateError,
			SSLCipherNotAvailable,
			SSLCACertError,
			UnrecognizedContentEncoding,
			LoginFailed,
			SSLShutdownFailed,
			NoInternetConnection
		}
	}
}
