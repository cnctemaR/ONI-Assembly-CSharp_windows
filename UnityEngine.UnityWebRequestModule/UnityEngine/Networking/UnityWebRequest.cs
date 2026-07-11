using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Bindings;
using UnityEngineInternal;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>The UnityWebRequest object is used to communicate with web servers.</para>
	/// </summary>
	[NativeHeader("Modules/UnityWebRequest/Public/UnityWebRequest.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class UnityWebRequest : IDisposable
	{
		/// <summary>
		///   <para>Creates a UnityWebRequest with the default options and no attached DownloadHandler or UploadHandler. Default method is GET.</para>
		/// </summary>
		/// <param name="url">The target URL with which this UnityWebRequest will communicate. Also accessible via the url property.</param>
		public UnityWebRequest()
		{
			this.m_Ptr = UnityWebRequest.Create();
			this.InternalSetDefaults();
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest with the default options and no attached DownloadHandler or UploadHandler. Default method is GET.</para>
		/// </summary>
		/// <param name="url">The target URL with which this UnityWebRequest will communicate. Also accessible via the url property.</param>
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

		[NativeConditional("ENABLE_UNITYWEBREQUEST")]
		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetWebErrorString(UnityWebRequest.UnityWebRequestError err);

		/// <summary>
		///   <para>If true, any CertificateHandler attached to this UnityWebRequest will have CertificateHandler.Dispose called automatically when UnityWebRequest.Dispose is called.</para>
		/// </summary>
		public bool disposeCertificateHandlerOnDispose { get; set; }

		/// <summary>
		///   <para>If true, any DownloadHandler attached to this UnityWebRequest will have DownloadHandler.Dispose called automatically when UnityWebRequest.Dispose is called.</para>
		/// </summary>
		public bool disposeDownloadHandlerOnDispose { get; set; }

		/// <summary>
		///   <para>If true, any UploadHandler attached to this UnityWebRequest will have UploadHandler.Dispose called automatically when UnityWebRequest.Dispose is called.</para>
		/// </summary>
		public bool disposeUploadHandlerOnDispose { get; set; }

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

		/// <summary>
		///   <para>Signals that this UnityWebRequest is no longer being used, and should clean up any resources it is using.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Begin communicating with the remote server.</para>
		/// </summary>
		/// <returns>
		///   <para>An AsyncOperation indicating the progress/completion state of the UnityWebRequest. Yield this object to wait until the UnityWebRequest is done.</para>
		/// </returns>
		[Obsolete("Use SendWebRequest.  It returns a UnityWebRequestAsyncOperation which contains a reference to the WebRequest object.", false)]
		public AsyncOperation Send()
		{
			return this.SendWebRequest();
		}

		/// <summary>
		///   <para>Begin communicating with the remote server.</para>
		/// </summary>
		public UnityWebRequestAsyncOperation SendWebRequest()
		{
			UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = this.BeginWebRequest();
			if (unityWebRequestAsyncOperation != null)
			{
				unityWebRequestAsyncOperation.webRequest = this;
			}
			return unityWebRequestAsyncOperation;
		}

		/// <summary>
		///   <para>If in progress, halts the UnityWebRequest as soon as possible.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Defines the HTTP verb used by this UnityWebRequest, such as GET or POST.</para>
		/// </summary>
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

		/// <summary>
		///   <para>A human-readable string describing any system errors encountered by this UnityWebRequest object while handling HTTP requests or responses. (Read Only)</para>
		/// </summary>
		public string error
		{
			get
			{
				string text;
				if (!this.isNetworkError && !this.isHttpError)
				{
					text = null;
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

		/// <summary>
		///   <para>Determines whether this UnityWebRequest will include Expect: 100-Continue in its outgoing request headers. (Default: true).</para>
		/// </summary>
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

		/// <summary>
		///   <para>Defines the target URL for the UnityWebRequest to communicate with.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Defines the target URI for the UnityWebRequest to communicate with.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The numeric HTTP response code returned by the server, such as 200, 404 or 500. (Read Only)</para>
		/// </summary>
		public extern long responseCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetUploadProgress();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsExecuting();

		/// <summary>
		///   <para>Returns a floating-point value between 0.0 and 1.0, indicating the progress of uploading body data to the server.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Returns true while a UnityWebRequest’s configuration properties can be altered. (Read Only)</para>
		/// </summary>
		public extern bool isModifiable
		{
			[NativeMethod("IsModifiable")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns true after the UnityWebRequest has finished communicating with the remote server. (Read Only)</para>
		/// </summary>
		public extern bool isDone
		{
			[NativeMethod("IsDone")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns true after this UnityWebRequest encounters a system error. (Read Only)</para>
		/// </summary>
		public extern bool isNetworkError
		{
			[NativeMethod("IsNetworkError")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns true after this UnityWebRequest receives an HTTP response code indicating an error. (Read Only)</para>
		/// </summary>
		public extern bool isHttpError
		{
			[NativeMethod("IsHttpError")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetDownloadProgress();

		/// <summary>
		///   <para>Returns a floating-point value between 0.0 and 1.0, indicating the progress of downloading body data from the server. (Read Only)</para>
		/// </summary>
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

		/// <summary>
		///   <para>Returns the number of bytes of body data the system has uploaded to the remote server. (Read Only)</para>
		/// </summary>
		public extern ulong uploadedBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the number of bytes of body data the system has downloaded from the remote server. (Read Only)</para>
		/// </summary>
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

		/// <summary>
		///   <para>Indicates the number of redirects which this UnityWebRequest will follow before halting with a “Redirect Limit Exceeded” system error.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Indicates whether the UnityWebRequest system should employ the HTTP/1.1 chunked-transfer encoding method.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Retrieves the value of a custom request header.</para>
		/// </summary>
		/// <param name="name">Name of the custom request header. Case-insensitive.</param>
		/// <returns>
		///   <para>The value of the custom request header. If no custom header with a matching name has been set, returns an empty string.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetRequestHeader(string name);

		[NativeMethod("SetRequestHeader")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern UnityWebRequest.UnityWebRequestError InternalSetRequestHeader(string name, string value);

		/// <summary>
		///   <para>Set a HTTP request header to a custom value.</para>
		/// </summary>
		/// <param name="name">The key of the header to be set. Case-sensitive.</param>
		/// <param name="value">The header's intended value.</param>
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

		/// <summary>
		///   <para>Retrieves the value of a response header from the latest HTTP response received.</para>
		/// </summary>
		/// <param name="name">The name of the HTTP header to retrieve. Case-insensitive.</param>
		/// <returns>
		///   <para>The value of the HTTP header from the latest HTTP response. If no header with a matching name has been received, or no responses have been received, returns null.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetResponseHeader(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string[] GetResponseHeaderKeys();

		/// <summary>
		///   <para>Retrieves a dictionary containing all the response headers received by this UnityWebRequest in the latest HTTP response.</para>
		/// </summary>
		/// <returns>
		///   <para>A dictionary containing all the response headers received in the latest HTTP response. If no responses have been received, returns null.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Holds a reference to the UploadHandler object which manages body data to be uploaded to the remote server.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Holds a reference to a DownloadHandler object, which manages body data received from the remote server by this UnityWebRequest.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Holds a reference to a CertificateHandler object, which manages certificate validation for this UnityWebRequest.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Sets UnityWebRequest to attempt to abort after the number of seconds in timeout have passed.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Creates a UnityWebRequest configured for HTTP GET.</para>
		/// </summary>
		/// <param name="uri">The URI of the resource to retrieve via HTTP GET.</param>
		/// <returns>
		///   <para>A UnityWebRequest object configured to retrieve data from uri.</para>
		/// </returns>
		public static UnityWebRequest Get(string uri)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerBuffer(), null);
		}

		public static UnityWebRequest Get(Uri uri)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerBuffer(), null);
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest configured for HTTP DELETE.</para>
		/// </summary>
		/// <param name="uri">The URI to which a DELETE request should be sent.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to send an HTTP DELETE request.</para>
		/// </returns>
		public static UnityWebRequest Delete(string uri)
		{
			return new UnityWebRequest(uri, "DELETE");
		}

		public static UnityWebRequest Delete(Uri uri)
		{
			return new UnityWebRequest(uri, "DELETE");
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest configured to send a HTTP HEAD request.</para>
		/// </summary>
		/// <param name="uri">The URI to which to send a HTTP HEAD request.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to transmit a HTTP HEAD request.</para>
		/// </returns>
		public static UnityWebRequest Head(string uri)
		{
			return new UnityWebRequest(uri, "HEAD");
		}

		public static UnityWebRequest Head(Uri uri)
		{
			return new UnityWebRequest(uri, "HEAD");
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest intended to download an image via HTTP GET and create a Texture based on the retrieved data.</para>
		/// </summary>
		/// <param name="uri">The URI of the image to download.</param>
		/// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
		/// <returns>
		///   <para>A UnityWebRequest properly configured to download an image and convert it to a Texture.</para>
		/// </returns>
		[Obsolete("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestTexture.GetTexture(*)", true)]
		public static UnityWebRequest GetTexture(string uri)
		{
			throw new NotSupportedException("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead.");
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest intended to download an image via HTTP GET and create a Texture based on the retrieved data.</para>
		/// </summary>
		/// <param name="uri">The URI of the image to download.</param>
		/// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
		/// <returns>
		///   <para>A UnityWebRequest properly configured to download an image and convert it to a Texture.</para>
		/// </returns>
		[Obsolete("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestTexture.GetTexture(*)", true)]
		public static UnityWebRequest GetTexture(string uri, bool nonReadable)
		{
			throw new NotSupportedException("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead.");
		}

		/// <summary>
		///   <para>OBSOLETE. Use UnityWebRequestMultimedia.GetAudioClip().</para>
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="audioType"></param>
		[Obsolete("UnityWebRequest.GetAudioClip is obsolete. Use UnityWebRequestMultimedia.GetAudioClip instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestMultimedia.GetAudioClip(*)", true)]
		public static UnityWebRequest GetAudioClip(string uri, AudioType audioType)
		{
			return null;
		}

		[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
		public static UnityWebRequest GetAssetBundle(string uri)
		{
			return null;
		}

		/// <summary>
		///   <para>Deprecated. Replaced by UnityWebRequestAssetBundle.GetAssetBundle.</para>
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="crc"></param>
		/// <param name="version"></param>
		/// <param name="hash"></param>
		/// <param name="cachedAssetBundle"></param>
		[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
		public static UnityWebRequest GetAssetBundle(string uri, uint crc)
		{
			return null;
		}

		/// <summary>
		///   <para>Deprecated. Replaced by UnityWebRequestAssetBundle.GetAssetBundle.</para>
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="crc"></param>
		/// <param name="version"></param>
		/// <param name="hash"></param>
		/// <param name="cachedAssetBundle"></param>
		[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
		public static UnityWebRequest GetAssetBundle(string uri, uint version, uint crc)
		{
			return null;
		}

		/// <summary>
		///   <para>Deprecated. Replaced by UnityWebRequestAssetBundle.GetAssetBundle.</para>
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="crc"></param>
		/// <param name="version"></param>
		/// <param name="hash"></param>
		/// <param name="cachedAssetBundle"></param>
		[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
		public static UnityWebRequest GetAssetBundle(string uri, Hash128 hash, uint crc)
		{
			return null;
		}

		/// <summary>
		///   <para>Deprecated. Replaced by UnityWebRequestAssetBundle.GetAssetBundle.</para>
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="crc"></param>
		/// <param name="version"></param>
		/// <param name="hash"></param>
		/// <param name="cachedAssetBundle"></param>
		[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
		public static UnityWebRequest GetAssetBundle(string uri, CachedAssetBundle cachedAssetBundle, uint crc)
		{
			return null;
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest configured to upload raw data to a remote server via HTTP PUT.</para>
		/// </summary>
		/// <param name="uri">The URI to which the data will be sent.</param>
		/// <param name="bodyData">The data to transmit to the remote server.
		///
		/// If a string, the string will be converted to raw bytes via &lt;a href="http:msdn.microsoft.comen-uslibrarysystem.text.encoding.utf8"&gt;System.Text.Encoding.UTF8&lt;a&gt;.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to transmit bodyData to uri via HTTP PUT.</para>
		/// </returns>
		public static UnityWebRequest Put(string uri, byte[] bodyData)
		{
			return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(bodyData));
		}

		public static UnityWebRequest Put(Uri uri, byte[] bodyData)
		{
			return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(bodyData));
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest configured to upload raw data to a remote server via HTTP PUT.</para>
		/// </summary>
		/// <param name="uri">The URI to which the data will be sent.</param>
		/// <param name="bodyData">The data to transmit to the remote server.
		///
		/// If a string, the string will be converted to raw bytes via &lt;a href="http:msdn.microsoft.comen-uslibrarysystem.text.encoding.utf8"&gt;System.Text.Encoding.UTF8&lt;a&gt;.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to transmit bodyData to uri via HTTP PUT.</para>
		/// </returns>
		public static UnityWebRequest Put(string uri, string bodyData)
		{
			return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData)));
		}

		public static UnityWebRequest Put(Uri uri, string bodyData)
		{
			return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData)));
		}

		/// <summary>
		///   <para>Creates a UnityWebRequest configured to send form data to a server via HTTP POST.</para>
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="postData">Form body data. Will be URLEncoded prior to transmission.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to send form data to uri via POST.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Create a UnityWebRequest configured to send form data to a server via HTTP POST.</para>
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="formData">Form fields or files encapsulated in a WWWForm object, for formatting and transmission to the remote server.</param>
		/// <returns>
		///   <para>A UnityWebRequest configured to send form data to uri via POST.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Escapes characters in a string to ensure they are URL-friendly.</para>
		/// </summary>
		/// <param name="s">A string with characters to be escaped.</param>
		/// <param name="e">The text encoding to use.</param>
		public static string EscapeURL(string s)
		{
			return UnityWebRequest.EscapeURL(s, Encoding.UTF8);
		}

		/// <summary>
		///   <para>Escapes characters in a string to ensure they are URL-friendly.</para>
		/// </summary>
		/// <param name="s">A string with characters to be escaped.</param>
		/// <param name="e">The text encoding to use.</param>
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
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				byte[] array = WWWTranscoder.URLEncode(bytes);
				text = e.GetString(array);
			}
			return text;
		}

		/// <summary>
		///   <para>Converts URL-friendly escape sequences back to normal text.</para>
		/// </summary>
		/// <param name="s">A string containing escaped characters.</param>
		/// <param name="e">The text encoding to use.</param>
		public static string UnEscapeURL(string s)
		{
			return UnityWebRequest.UnEscapeURL(s, Encoding.UTF8);
		}

		/// <summary>
		///   <para>Converts URL-friendly escape sequences back to normal text.</para>
		/// </summary>
		/// <param name="s">A string containing escaped characters.</param>
		/// <param name="e">The text encoding to use.</param>
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
				byte[] bytes = Encoding.UTF8.GetBytes(s);
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

		/// <summary>
		///   <para>Generate a random 40-byte array for use as a multipart form boundary.</para>
		/// </summary>
		/// <returns>
		///   <para>40 random bytes, guaranteed to contain only printable ASCII values.</para>
		/// </returns>
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

		/// <summary>
		///   <para>The string "GET", commonly used as the verb for an HTTP GET request.</para>
		/// </summary>
		public const string kHttpVerbGET = "GET";

		/// <summary>
		///   <para>The string "HEAD", commonly used as the verb for an HTTP HEAD request.</para>
		/// </summary>
		public const string kHttpVerbHEAD = "HEAD";

		/// <summary>
		///   <para>The string "POST", commonly used as the verb for an HTTP POST request.</para>
		/// </summary>
		public const string kHttpVerbPOST = "POST";

		/// <summary>
		///   <para>The string "PUT", commonly used as the verb for an HTTP PUT request.</para>
		/// </summary>
		public const string kHttpVerbPUT = "PUT";

		/// <summary>
		///   <para>The string "CREATE", commonly used as the verb for an HTTP CREATE request.</para>
		/// </summary>
		public const string kHttpVerbCREATE = "CREATE";

		/// <summary>
		///   <para>The string "DELETE", commonly used as the verb for an HTTP DELETE request.</para>
		/// </summary>
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
