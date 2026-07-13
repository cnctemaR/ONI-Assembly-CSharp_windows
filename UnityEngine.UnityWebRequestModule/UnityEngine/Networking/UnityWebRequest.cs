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
		[NativeMethod(IsThreadSafe = true)]
		[NativeConditional("ENABLE_UNITYWEBREQUEST")]
		private static string GetWebErrorString(UnityWebRequest.UnityWebRequestError err)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				UnityWebRequest.GetWebErrorString_Injected(err, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[VisibleToOtherModules]
		internal static string GetHTTPStatusString(long responseCode)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				UnityWebRequest.GetHTTPStatusString_Injected(responseCode, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public bool disposeCertificateHandlerOnDispose { get; set; }

		public bool disposeDownloadHandlerOnDispose { get; set; }

		public bool disposeUploadHandlerOnDispose { get; set; }

		public static void ClearCookieCache()
		{
			UnityWebRequest.ClearCookieCache(null, null);
		}

		public static void ClearCookieCache(Uri uri)
		{
			bool flag = uri == null;
			if (flag)
			{
				UnityWebRequest.ClearCookieCache(null, null);
			}
			else
			{
				string host = uri.Host;
				string text = uri.AbsolutePath;
				bool flag2 = text == "/";
				if (flag2)
				{
					text = null;
				}
				UnityWebRequest.ClearCookieCache(host, text);
			}
		}

		private unsafe static void ClearCookieCache(string domain, string path)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(domain, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = domain.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = path.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				UnityWebRequest.ClearCookieCache_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr Create();

		[NativeMethod(IsThreadSafe = true)]
		private void Release()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			UnityWebRequest.Release_Injected(intPtr);
		}

		internal void InternalDestroy()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
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
			bool disposeDownloadHandlerOnDispose = this.disposeDownloadHandlerOnDispose;
			if (disposeDownloadHandlerOnDispose)
			{
				DownloadHandler downloadHandler = this.downloadHandler;
				bool flag = downloadHandler != null;
				if (flag)
				{
					downloadHandler.Dispose();
				}
			}
			bool disposeUploadHandlerOnDispose = this.disposeUploadHandlerOnDispose;
			if (disposeUploadHandlerOnDispose)
			{
				UploadHandler uploadHandler = this.uploadHandler;
				bool flag2 = uploadHandler != null;
				if (flag2)
				{
					uploadHandler.Dispose();
				}
			}
			bool disposeCertificateHandlerOnDispose = this.disposeCertificateHandlerOnDispose;
			if (disposeCertificateHandlerOnDispose)
			{
				CertificateHandler certificateHandler = this.certificateHandler;
				bool flag3 = certificateHandler != null;
				if (flag3)
				{
					certificateHandler.Dispose();
				}
			}
		}

		[NativeThrows]
		internal UnityWebRequestAsyncOperation BeginWebRequest()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = UnityWebRequest.BeginWebRequest_Injected(intPtr);
			return (intPtr2 == 0) ? null : UnityWebRequestAsyncOperation.BindingsMarshaller.ConvertToManaged(intPtr2);
		}

		[Obsolete("Use SendWebRequest.  It returns a UnityWebRequestAsyncOperation which contains a reference to the WebRequest object.", false)]
		public AsyncOperation Send()
		{
			return this.SendWebRequest();
		}

		public UnityWebRequestAsyncOperation SendWebRequest()
		{
			UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = this.BeginWebRequest();
			bool flag = unityWebRequestAsyncOperation != null;
			if (flag)
			{
				unityWebRequestAsyncOperation.webRequest = this;
			}
			return unityWebRequestAsyncOperation;
		}

		[NativeMethod(IsThreadSafe = true)]
		public void Abort()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			UnityWebRequest.Abort_Injected(intPtr);
		}

		private UnityWebRequest.UnityWebRequestError SetMethod(UnityWebRequest.UnityWebRequestMethod methodType)
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.SetMethod_Injected(intPtr, methodType);
		}

		internal void InternalSetMethod(UnityWebRequest.UnityWebRequestMethod methodType)
		{
			bool flag = !this.isModifiable;
			if (flag)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent and its request method can no longer be altered");
			}
			UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetMethod(methodType);
			bool flag2 = unityWebRequestError > UnityWebRequest.UnityWebRequestError.OK;
			if (flag2)
			{
				throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
			}
		}

		private unsafe UnityWebRequest.UnityWebRequestError SetCustomMethod(string customMethodName)
		{
			UnityWebRequest.UnityWebRequestError unityWebRequestError;
			try
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(customMethodName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = customMethodName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				unityWebRequestError = UnityWebRequest.SetCustomMethod_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return unityWebRequestError;
		}

		internal void InternalSetCustomMethod(string customMethodName)
		{
			bool flag = !this.isModifiable;
			if (flag)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent and its request method can no longer be altered");
			}
			UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetCustomMethod(customMethodName);
			bool flag2 = unityWebRequestError > UnityWebRequest.UnityWebRequestError.OK;
			if (flag2)
			{
				throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
			}
		}

		internal UnityWebRequest.UnityWebRequestMethod GetMethod()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.GetMethod_Injected(intPtr);
		}

		internal string GetCustomMethod()
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				UnityWebRequest.GetCustomMethod_Injected(intPtr, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

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
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Cannot set a UnityWebRequest's method to an empty or null string");
				}
				string text = value.ToUpper();
				string text2 = text;
				if (!(text2 == "GET"))
				{
					if (!(text2 == "POST"))
					{
						if (!(text2 == "PUT"))
						{
							if (!(text2 == "HEAD"))
							{
								this.InternalSetCustomMethod(value.ToUpper());
							}
							else
							{
								this.InternalSetMethod(UnityWebRequest.UnityWebRequestMethod.Head);
							}
						}
						else
						{
							this.InternalSetMethod(UnityWebRequest.UnityWebRequestMethod.Put);
						}
					}
					else
					{
						this.InternalSetMethod(UnityWebRequest.UnityWebRequestMethod.Post);
					}
				}
				else
				{
					this.InternalSetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
				}
			}
		}

		private UnityWebRequest.UnityWebRequestError GetError()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.GetError_Injected(intPtr);
		}

		public string error
		{
			get
			{
				UnityWebRequest.Result result = this.result;
				UnityWebRequest.Result result2 = result;
				string text;
				if (result2 > UnityWebRequest.Result.Success)
				{
					if (result2 != UnityWebRequest.Result.ProtocolError)
					{
						text = UnityWebRequest.GetWebErrorString(this.GetError());
					}
					else
					{
						text = string.Format("HTTP/1.1 {0} {1}", this.responseCode, UnityWebRequest.GetHTTPStatusString(this.responseCode));
					}
				}
				else
				{
					text = null;
				}
				return text;
			}
		}

		private bool use100Continue
		{
			get
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return UnityWebRequest.get_use100Continue_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				UnityWebRequest.set_use100Continue_Injected(intPtr, value);
			}
		}

		public bool useHttpContinue
		{
			get
			{
				return this.use100Continue;
			}
			set
			{
				bool flag = !this.isModifiable;
				if (flag)
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
				string text = "https://localhost/";
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
				bool flag = !value.IsAbsoluteUri;
				if (flag)
				{
					throw new ArgumentException("URI must be absolute");
				}
				this.InternalSetUrl(WebRequestUtils.MakeUriString(value, value.OriginalString, false));
				this.m_Uri = value;
			}
		}

		private string GetUrl()
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				UnityWebRequest.GetUrl_Injected(intPtr, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		private unsafe UnityWebRequest.UnityWebRequestError SetUrl(string url)
		{
			UnityWebRequest.UnityWebRequestError unityWebRequestError;
			try
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(url, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = url.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				unityWebRequestError = UnityWebRequest.SetUrl_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return unityWebRequestError;
		}

		private void InternalSetUrl(string url)
		{
			bool flag = !this.isModifiable;
			if (flag)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent and its URL cannot be altered");
			}
			UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetUrl(url);
			bool flag2 = unityWebRequestError > UnityWebRequest.UnityWebRequestError.OK;
			if (flag2)
			{
				throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
			}
		}

		public long responseCode
		{
			get
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return UnityWebRequest.get_responseCode_Injected(intPtr);
			}
		}

		private float GetUploadProgress()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.GetUploadProgress_Injected(intPtr);
		}

		private bool IsExecuting()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.IsExecuting_Injected(intPtr);
		}

		public float uploadProgress
		{
			get
			{
				bool flag = !this.IsExecuting() && !this.isDone;
				float num;
				if (flag)
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

		public bool isModifiable
		{
			[NativeMethod("IsModifiable")]
			get
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return UnityWebRequest.get_isModifiable_Injected(intPtr);
			}
		}

		public bool isDone
		{
			get
			{
				return this.result > UnityWebRequest.Result.InProgress;
			}
		}

		[Obsolete("UnityWebRequest.isNetworkError is deprecated. Use (UnityWebRequest.result == UnityWebRequest.Result.ConnectionError) instead.", false)]
		public bool isNetworkError
		{
			get
			{
				return this.result == UnityWebRequest.Result.ConnectionError;
			}
		}

		[Obsolete("UnityWebRequest.isHttpError is deprecated. Use (UnityWebRequest.result == UnityWebRequest.Result.ProtocolError) instead.", false)]
		public bool isHttpError
		{
			get
			{
				return this.result == UnityWebRequest.Result.ProtocolError;
			}
		}

		public UnityWebRequest.Result result
		{
			[NativeMethod("GetResult")]
			get
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return UnityWebRequest.get_result_Injected(intPtr);
			}
		}

		private float GetDownloadProgress()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.GetDownloadProgress_Injected(intPtr);
		}

		public float downloadProgress
		{
			get
			{
				bool flag = !this.IsExecuting() && !this.isDone;
				float num;
				if (flag)
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

		public ulong uploadedBytes
		{
			get
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return UnityWebRequest.get_uploadedBytes_Injected(intPtr);
			}
		}

		public ulong downloadedBytes
		{
			get
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return UnityWebRequest.get_downloadedBytes_Injected(intPtr);
			}
		}

		private int GetRedirectLimit()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.GetRedirectLimit_Injected(intPtr);
		}

		[NativeThrows]
		private void SetRedirectLimitFromScripting(int limit)
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			UnityWebRequest.SetRedirectLimitFromScripting_Injected(intPtr, limit);
		}

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

		private bool GetChunked()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.GetChunked_Injected(intPtr);
		}

		private UnityWebRequest.UnityWebRequestError SetChunked(bool chunked)
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.SetChunked_Injected(intPtr, chunked);
		}

		[Obsolete("HTTP/2 and many HTTP/1.1 servers don't support this; we recommend leaving it set to false (default).", false)]
		public bool chunkedTransfer
		{
			get
			{
				return this.GetChunked();
			}
			set
			{
				bool flag = !this.isModifiable;
				if (flag)
				{
					throw new InvalidOperationException("UnityWebRequest has already been sent and its chunked transfer encoding setting cannot be altered");
				}
				UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetChunked(value);
				bool flag2 = unityWebRequestError > UnityWebRequest.UnityWebRequestError.OK;
				if (flag2)
				{
					throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
				}
			}
		}

		public unsafe string GetRequestHeader(string name)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				UnityWebRequest.GetRequestHeader_Injected(intPtr, ref managedSpanWrapper, out managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				ManagedSpanWrapper managedSpanWrapper2;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper2);
			}
			return stringAndDispose;
		}

		[NativeMethod("SetRequestHeader")]
		internal unsafe UnityWebRequest.UnityWebRequestError InternalSetRequestHeader(string name, string value)
		{
			UnityWebRequest.UnityWebRequestError unityWebRequestError;
			try
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = value.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				unityWebRequestError = UnityWebRequest.InternalSetRequestHeader_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return unityWebRequestError;
		}

		public void SetRequestHeader(string name, string value)
		{
			bool flag = string.IsNullOrEmpty(name);
			if (flag)
			{
				throw new ArgumentException("Cannot set a Request Header with a null or empty name");
			}
			bool flag2 = value == null;
			if (flag2)
			{
				throw new ArgumentException("Cannot set a Request header with a null");
			}
			bool flag3 = !this.isModifiable;
			if (flag3)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent and its request headers cannot be altered");
			}
			UnityWebRequest.UnityWebRequestError unityWebRequestError = this.InternalSetRequestHeader(name, value);
			bool flag4 = unityWebRequestError > UnityWebRequest.UnityWebRequestError.OK;
			if (flag4)
			{
				throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
			}
		}

		public unsafe string GetResponseHeader(string name)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				UnityWebRequest.GetResponseHeader_Injected(intPtr, ref managedSpanWrapper, out managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				ManagedSpanWrapper managedSpanWrapper2;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper2);
			}
			return stringAndDispose;
		}

		internal string[] GetResponseHeaderKeys()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.GetResponseHeaderKeys_Injected(intPtr);
		}

		public Dictionary<string, string> GetResponseHeaders()
		{
			string[] responseHeaderKeys = this.GetResponseHeaderKeys();
			bool flag = responseHeaderKeys == null || responseHeaderKeys.Length == 0;
			Dictionary<string, string> dictionary;
			if (flag)
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

		private UnityWebRequest.UnityWebRequestError SetUploadHandler(UploadHandler uh)
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.SetUploadHandler_Injected(intPtr, (uh == null) ? ((IntPtr)0) : UploadHandler.BindingsMarshaller.ConvertToNative(uh));
		}

		public UploadHandler uploadHandler
		{
			get
			{
				return this.m_UploadHandler;
			}
			set
			{
				bool flag = !this.isModifiable;
				if (flag)
				{
					throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the upload handler");
				}
				UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetUploadHandler(value);
				bool flag2 = unityWebRequestError > UnityWebRequest.UnityWebRequestError.OK;
				if (flag2)
				{
					throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
				}
				this.m_UploadHandler = value;
			}
		}

		private UnityWebRequest.UnityWebRequestError SetDownloadHandler(DownloadHandler dh)
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.SetDownloadHandler_Injected(intPtr, (dh == null) ? ((IntPtr)0) : DownloadHandler.BindingsMarshaller.ConvertToNative(dh));
		}

		public DownloadHandler downloadHandler
		{
			get
			{
				return this.m_DownloadHandler;
			}
			set
			{
				bool flag = !this.isModifiable;
				if (flag)
				{
					throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the download handler");
				}
				UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetDownloadHandler(value);
				bool flag2 = unityWebRequestError > UnityWebRequest.UnityWebRequestError.OK;
				if (flag2)
				{
					throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
				}
				this.m_DownloadHandler = value;
			}
		}

		private UnityWebRequest.UnityWebRequestError SetCertificateHandler(CertificateHandler ch)
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.SetCertificateHandler_Injected(intPtr, (ch == null) ? ((IntPtr)0) : CertificateHandler.BindingsMarshaller.ConvertToNative(ch));
		}

		public CertificateHandler certificateHandler
		{
			get
			{
				return this.m_CertificateHandler;
			}
			set
			{
				bool flag = !this.isModifiable;
				if (flag)
				{
					throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the certificate handler");
				}
				UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetCertificateHandler(value);
				bool flag2 = unityWebRequestError > UnityWebRequest.UnityWebRequestError.OK;
				if (flag2)
				{
					throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
				}
				this.m_CertificateHandler = value;
			}
		}

		private int GetTimeoutMsec()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.GetTimeoutMsec_Injected(intPtr);
		}

		private UnityWebRequest.UnityWebRequestError SetTimeoutMsec(int timeout)
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.SetTimeoutMsec_Injected(intPtr, timeout);
		}

		public int timeout
		{
			get
			{
				return this.GetTimeoutMsec() / 1000;
			}
			set
			{
				bool flag = !this.isModifiable;
				if (flag)
				{
					throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the timeout");
				}
				value = Math.Max(value, 0);
				UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetTimeoutMsec(value * 1000);
				bool flag2 = unityWebRequestError > UnityWebRequest.UnityWebRequestError.OK;
				if (flag2)
				{
					throw new InvalidOperationException(UnityWebRequest.GetWebErrorString(unityWebRequestError));
				}
			}
		}

		private bool GetSuppressErrorsToConsole()
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.GetSuppressErrorsToConsole_Injected(intPtr);
		}

		private UnityWebRequest.UnityWebRequestError SetSuppressErrorsToConsole(bool suppress)
		{
			IntPtr intPtr = UnityWebRequest.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UnityWebRequest.SetSuppressErrorsToConsole_Injected(intPtr, suppress);
		}

		internal bool suppressErrorsToConsole
		{
			get
			{
				return this.GetSuppressErrorsToConsole();
			}
			set
			{
				bool flag = !this.isModifiable;
				if (flag)
				{
					throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the timeout");
				}
				UnityWebRequest.UnityWebRequestError unityWebRequestError = this.SetSuppressErrorsToConsole(value);
				bool flag2 = unityWebRequestError > UnityWebRequest.UnityWebRequestError.OK;
				if (flag2)
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestTexture.GetTexture(*)", true)]
		public static UnityWebRequest GetTexture(string uri)
		{
			throw new NotSupportedException("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead.");
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestTexture.GetTexture(*)", true)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("UnityWebRequest.Post with only a string data is obsolete. Use UnityWebRequest.Post with content type argument or UnityWebRequest.PostWwwForm instead (UnityUpgradable) -> [UnityEngine] UnityWebRequest.PostWwwForm(*)", false)]
		public static UnityWebRequest Post(string uri, string postData)
		{
			return UnityWebRequest.PostWwwForm(uri, postData);
		}

		[Obsolete("UnityWebRequest.Post with only a string data is obsolete. Use UnityWebRequest.Post with content type argument or UnityWebRequest.PostWwwForm instead (UnityUpgradable) -> [UnityEngine] UnityWebRequest.PostWwwForm(*)", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static UnityWebRequest Post(Uri uri, string postData)
		{
			return UnityWebRequest.PostWwwForm(uri, postData);
		}

		public static UnityWebRequest PostWwwForm(string uri, string form)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			UnityWebRequest.SetupPostWwwForm(unityWebRequest, form);
			return unityWebRequest;
		}

		public static UnityWebRequest PostWwwForm(Uri uri, string form)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			UnityWebRequest.SetupPostWwwForm(unityWebRequest, form);
			return unityWebRequest;
		}

		private static void SetupPostWwwForm(UnityWebRequest request, string postData)
		{
			request.downloadHandler = new DownloadHandlerBuffer();
			bool flag = string.IsNullOrEmpty(postData);
			if (!flag)
			{
				string text = WWWTranscoder.DataEncode(postData, Encoding.UTF8);
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				request.uploadHandler = new UploadHandlerRaw(bytes);
				request.uploadHandler.contentType = "application/x-www-form-urlencoded";
			}
		}

		public static UnityWebRequest Post(string uri, string postData, string contentType)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			UnityWebRequest.SetupPost(unityWebRequest, postData, contentType);
			return unityWebRequest;
		}

		public static UnityWebRequest Post(Uri uri, string postData, string contentType)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			UnityWebRequest.SetupPost(unityWebRequest, postData, contentType);
			return unityWebRequest;
		}

		private static void SetupPost(UnityWebRequest request, string postData, string contentType)
		{
			request.downloadHandler = new DownloadHandlerBuffer();
			bool flag = string.IsNullOrEmpty(postData);
			if (flag)
			{
				request.SetRequestHeader("Content-Type", contentType);
			}
			else
			{
				byte[] bytes = Encoding.UTF8.GetBytes(postData);
				request.uploadHandler = new UploadHandlerRaw(bytes);
				request.uploadHandler.contentType = contentType;
			}
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
			request.downloadHandler = new DownloadHandlerBuffer();
			bool flag = formData == null;
			if (!flag)
			{
				byte[] array = formData.data;
				bool flag2 = array.Length == 0;
				if (flag2)
				{
					array = null;
				}
				bool flag3 = array != null;
				if (flag3)
				{
					request.uploadHandler = new UploadHandlerRaw(array);
				}
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
			request.downloadHandler = new DownloadHandlerBuffer();
			byte[] array = null;
			bool flag = multipartFormSections != null && multipartFormSections.Count != 0;
			if (flag)
			{
				array = UnityWebRequest.SerializeFormSections(multipartFormSections, boundary);
			}
			bool flag2 = array == null;
			if (!flag2)
			{
				request.uploadHandler = new UploadHandlerRaw(array)
				{
					contentType = "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary, 0, boundary.Length)
				};
			}
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
			request.downloadHandler = new DownloadHandlerBuffer();
			byte[] array = null;
			bool flag = formFields != null && formFields.Count != 0;
			if (flag)
			{
				array = UnityWebRequest.SerializeSimpleForm(formFields);
			}
			bool flag2 = array == null;
			if (!flag2)
			{
				request.uploadHandler = new UploadHandlerRaw(array)
				{
					contentType = "application/x-www-form-urlencoded"
				};
			}
		}

		public static string EscapeURL(string s)
		{
			return UnityWebRequest.EscapeURL(s, Encoding.UTF8);
		}

		public static string EscapeURL(string s, Encoding e)
		{
			bool flag = s == null;
			string text;
			if (flag)
			{
				text = null;
			}
			else
			{
				bool flag2 = s == "";
				if (flag2)
				{
					text = "";
				}
				else
				{
					bool flag3 = e == null;
					if (flag3)
					{
						text = null;
					}
					else
					{
						byte[] bytes = e.GetBytes(s);
						byte[] array = WWWTranscoder.URLEncode(bytes);
						text = e.GetString(array);
					}
				}
			}
			return text;
		}

		public static string UnEscapeURL(string s)
		{
			return UnityWebRequest.UnEscapeURL(s, Encoding.UTF8);
		}

		public static string UnEscapeURL(string s, Encoding e)
		{
			bool flag = s == null;
			string text;
			if (flag)
			{
				text = null;
			}
			else
			{
				bool flag2 = s.IndexOf('%') == -1 && s.IndexOf('+') == -1;
				if (flag2)
				{
					text = s;
				}
				else
				{
					byte[] bytes = e.GetBytes(s);
					byte[] array = WWWTranscoder.URLDecode(bytes);
					text = e.GetString(array);
				}
			}
			return text;
		}

		public static byte[] SerializeFormSections(List<IMultipartFormSection> multipartFormSections, byte[] boundary)
		{
			bool flag = multipartFormSections == null || multipartFormSections.Count == 0;
			byte[] array;
			if (flag)
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
					bool flag2 = !string.IsNullOrEmpty(sectionName);
					if (flag2)
					{
						text2 = text2 + "; name=\"" + sectionName + "\"";
					}
					bool flag3 = !string.IsNullOrEmpty(fileName);
					if (flag3)
					{
						text2 = text2 + "; filename=\"" + fileName + "\"";
					}
					text2 += "\r\n";
					string contentType = multipartFormSection2.contentType;
					bool flag4 = !string.IsNullOrEmpty(contentType);
					if (flag4)
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
				bool flag = num > 57;
				if (flag)
				{
					num += 7;
				}
				bool flag2 = num > 90;
				if (flag2)
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
				bool flag = text.Length > 0;
				if (flag)
				{
					text += "&";
				}
				text = text + WWWTranscoder.DataEncode(keyValuePair.Key) + "=" + WWWTranscoder.DataEncode(keyValuePair.Value);
			}
			return Encoding.UTF8.GetBytes(text);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetWebErrorString_Injected(UnityWebRequest.UnityWebRequestError err, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetHTTPStatusString_Injected(long responseCode, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearCookieCache_Injected(ref ManagedSpanWrapper domain, ref ManagedSpanWrapper path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Release_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr BeginWebRequest_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Abort_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.UnityWebRequestError SetMethod_Injected(IntPtr _unity_self, UnityWebRequest.UnityWebRequestMethod methodType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.UnityWebRequestError SetCustomMethod_Injected(IntPtr _unity_self, ref ManagedSpanWrapper customMethodName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.UnityWebRequestMethod GetMethod_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCustomMethod_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.UnityWebRequestError GetError_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_use100Continue_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_use100Continue_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetUrl_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.UnityWebRequestError SetUrl_Injected(IntPtr _unity_self, ref ManagedSpanWrapper url);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long get_responseCode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetUploadProgress_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsExecuting_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isModifiable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.Result get_result_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetDownloadProgress_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong get_uploadedBytes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong get_downloadedBytes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRedirectLimit_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRedirectLimitFromScripting_Injected(IntPtr _unity_self, int limit);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetChunked_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.UnityWebRequestError SetChunked_Injected(IntPtr _unity_self, bool chunked);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRequestHeader_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.UnityWebRequestError InternalSetRequestHeader_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetResponseHeader_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetResponseHeaderKeys_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.UnityWebRequestError SetUploadHandler_Injected(IntPtr _unity_self, IntPtr uh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.UnityWebRequestError SetDownloadHandler_Injected(IntPtr _unity_self, IntPtr dh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.UnityWebRequestError SetCertificateHandler_Injected(IntPtr _unity_self, IntPtr ch);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetTimeoutMsec_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.UnityWebRequestError SetTimeoutMsec_Injected(IntPtr _unity_self, int timeout);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetSuppressErrorsToConsole_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityWebRequest.UnityWebRequestError SetSuppressErrorsToConsole_Injected(IntPtr _unity_self, bool suppress);

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
			OKCached,
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
			RedirectLimitInvalid,
			InvalidRedirect,
			CannotModifyRequest,
			HeaderNameContainsInvalidCharacters,
			HeaderValueContainsInvalidCharacters,
			CannotOverrideSystemHeaders,
			AlreadySent,
			InvalidMethod,
			NotImplemented,
			NoInternetConnection,
			DataProcessingError,
			InsecureConnectionNotAllowed
		}

		public enum Result
		{
			InProgress,
			Success,
			ConnectionError,
			ProtocolError,
			DataProcessingError
		}

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(UnityWebRequest unityWebRequest)
			{
				return unityWebRequest.m_Ptr;
			}
		}
	}
}
