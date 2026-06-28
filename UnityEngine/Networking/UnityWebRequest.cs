using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine.Networking
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class UnityWebRequest : IDisposable
	{
		public UnityWebRequest()
		{
			this.InternalCreate();
			this.InternalSetDefaults();
		}

		public UnityWebRequest(string url)
		{
			this.InternalCreate();
			this.InternalSetDefaults();
			this.url = url;
		}

		public UnityWebRequest(string url, string method)
		{
			this.InternalCreate();
			this.InternalSetDefaults();
			this.url = url;
			this.method = method;
		}

		public UnityWebRequest(string url, string method, DownloadHandler downloadHandler, UploadHandler uploadHandler)
		{
			this.InternalCreate();
			this.InternalSetDefaults();
			this.url = url;
			this.method = method;
			this.downloadHandler = downloadHandler;
			this.uploadHandler = uploadHandler;
		}

		public static UnityWebRequest Get(string uri)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerBuffer(), null);
		}

		public static UnityWebRequest Delete(string uri)
		{
			return new UnityWebRequest(uri, "DELETE");
		}

		public static UnityWebRequest Head(string uri)
		{
			return new UnityWebRequest(uri, "HEAD");
		}

		public static UnityWebRequest GetTexture(string uri)
		{
			return UnityWebRequest.GetTexture(uri, false);
		}

		public static UnityWebRequest GetTexture(string uri, bool nonReadable)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerTexture(!nonReadable), null);
		}

		public static UnityWebRequest GetAudioClip(string uri, AudioType audioType)
		{
			Type type = Type.GetType("UnityEngine.Networking.DownloadHandlerAudioClip");
			UnityWebRequest unityWebRequest;
			if (type == null)
			{
				unityWebRequest = UnityWebRequest.Get(uri);
			}
			else
			{
				ConstructorInfo constructor = type.GetConstructor(new Type[]
				{
					typeof(string),
					typeof(AudioType)
				});
				UnityWebRequest unityWebRequest2 = new UnityWebRequest(uri, "GET", constructor.Invoke(new object[] { uri, audioType }) as DownloadHandler, null);
				unityWebRequest = unityWebRequest2;
			}
			return unityWebRequest;
		}

		public static UnityWebRequest GetAssetBundle(string uri)
		{
			return UnityWebRequest.GetAssetBundle(uri, 0U);
		}

		public static UnityWebRequest GetAssetBundle(string uri, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, crc), null);
		}

		public static UnityWebRequest GetAssetBundle(string uri, uint version, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, version, crc), null);
		}

		public static UnityWebRequest GetAssetBundle(string uri, Hash128 hash, uint crc)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, hash, crc), null);
		}

		public static UnityWebRequest Put(string uri, byte[] bodyData)
		{
			return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(bodyData));
		}

		public static UnityWebRequest Put(string uri, string bodyData)
		{
			return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData)));
		}

		public static UnityWebRequest Post(string uri, string postData)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			byte[] array = null;
			if (!string.IsNullOrEmpty(postData))
			{
				string text = WWWTranscoder.URLEncode(postData, Encoding.UTF8);
				array = Encoding.UTF8.GetBytes(text);
			}
			unityWebRequest.uploadHandler = new UploadHandlerRaw(array);
			unityWebRequest.uploadHandler.contentType = "application/x-www-form-urlencoded";
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			return unityWebRequest;
		}

		public static UnityWebRequest Post(string uri, WWWForm formData)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			byte[] array = null;
			if (formData != null)
			{
				array = formData.data;
				if (array.Length == 0)
				{
					array = null;
				}
			}
			unityWebRequest.uploadHandler = new UploadHandlerRaw(array);
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			if (formData != null)
			{
				Dictionary<string, string> headers = formData.headers;
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					unityWebRequest.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return unityWebRequest;
		}

		public static UnityWebRequest Post(string uri, List<IMultipartFormSection> multipartFormSections)
		{
			byte[] array = UnityWebRequest.GenerateBoundary();
			return UnityWebRequest.Post(uri, multipartFormSections, array);
		}

		public static UnityWebRequest Post(string uri, List<IMultipartFormSection> multipartFormSections, byte[] boundary)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			byte[] array = null;
			if (multipartFormSections != null && multipartFormSections.Count != 0)
			{
				array = UnityWebRequest.SerializeFormSections(multipartFormSections, boundary);
			}
			unityWebRequest.uploadHandler = new UploadHandlerRaw(array)
			{
				contentType = "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary, 0, boundary.Length)
			};
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			return unityWebRequest;
		}

		public static UnityWebRequest Post(string uri, Dictionary<string, string> formFields)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
			byte[] array = null;
			if (formFields != null && formFields.Count != 0)
			{
				array = UnityWebRequest.SerializeSimpleForm(formFields);
			}
			unityWebRequest.uploadHandler = new UploadHandlerRaw(array)
			{
				contentType = "application/x-www-form-urlencoded"
			};
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			return unityWebRequest;
		}

		public static byte[] SerializeFormSections(List<IMultipartFormSection> multipartFormSections, byte[] boundary)
		{
			byte[] bytes = Encoding.UTF8.GetBytes("\r\n");
			byte[] bytes2 = WWW.DefaultEncoding.GetBytes("--");
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
				if (!string.IsNullOrEmpty(fileName))
				{
					text = "file";
				}
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
			list.TrimExcess();
			return list.ToArray();
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
				text = text + Uri.EscapeDataString(keyValuePair.Key) + "=" + Uri.EscapeDataString(keyValuePair.Value);
			}
			return Encoding.UTF8.GetBytes(text);
		}

		public bool disposeDownloadHandlerOnDispose { get; set; }

		public bool disposeUploadHandlerOnDispose { get; set; }

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalCreate();

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalDestroy();

		private void InternalSetDefaults()
		{
			this.disposeDownloadHandlerOnDispose = true;
			this.disposeUploadHandlerOnDispose = true;
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
				DownloadHandler downloadHandler = this.GetDownloadHandler();
				if (downloadHandler != null)
				{
					downloadHandler.Dispose();
				}
			}
			if (this.disposeUploadHandlerOnDispose)
			{
				UploadHandler uploadHandler = this.GetUploadHandler();
				if (uploadHandler != null)
				{
					uploadHandler.Dispose();
				}
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern AsyncOperation InternalBegin();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalAbort();

		public AsyncOperation Send()
		{
			return this.InternalBegin();
		}

		public void Abort()
		{
			this.InternalAbort();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalSetMethod(UnityWebRequest.UnityWebRequestMethod methodType);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalSetCustomMethod(string customMethodName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int InternalGetMethod();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string InternalGetCustomMethod();

		public string method
		{
			get
			{
				string text;
				switch (this.InternalGetMethod())
				{
				case 0:
					text = "GET";
					break;
				case 1:
					text = "POST";
					break;
				case 2:
					text = "PUT";
					break;
				case 3:
					text = "HEAD";
					break;
				default:
					text = this.InternalGetCustomMethod();
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int InternalGetError();

		public extern string error
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool useHttpContinue
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public string url
		{
			get
			{
				return this.InternalGetUrl();
			}
			set
			{
				string text = "http://localhost/";
				this.InternalSetUrl(WebRequestUtils.MakeInitialUrl(value, text));
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string InternalGetUrl();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetUrl(string url);

		public extern long responseCode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float uploadProgress
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isModifiable
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isDone
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isError
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float downloadProgress
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ulong uploadedBytes
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ulong downloadedBytes
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int redirectLimit
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool chunkedTransfer
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetRequestHeader(string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalSetRequestHeader(string name, string value);

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
			this.InternalSetRequestHeader(name, value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetResponseHeader(string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string[] InternalGetResponseHeaderKeys();

		public Dictionary<string, string> GetResponseHeaders()
		{
			string[] array = this.InternalGetResponseHeaderKeys();
			Dictionary<string, string> dictionary;
			if (array == null)
			{
				dictionary = null;
			}
			else
			{
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>(array.Length, StringComparer.OrdinalIgnoreCase);
				for (int i = 0; i < array.Length; i++)
				{
					string responseHeader = this.GetResponseHeader(array[i]);
					dictionary2.Add(array[i], responseHeader);
				}
				dictionary = dictionary2;
			}
			return dictionary;
		}

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern UploadHandler GetUploadHandler();

		public extern UploadHandler uploadHandler
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern DownloadHandler GetDownloadHandler();

		public extern DownloadHandler downloadHandler
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int timeout
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		private static string GetErrorDescription(UnityWebRequest.UnityWebRequestError errorCode)
		{
			switch (errorCode)
			{
			case UnityWebRequest.UnityWebRequestError.OK:
				return "No Error";
			case UnityWebRequest.UnityWebRequestError.SDKError:
				return "Internal Error With Transport Layer";
			case UnityWebRequest.UnityWebRequestError.UnsupportedProtocol:
				return "Specified Transport Protocol is Unsupported";
			case UnityWebRequest.UnityWebRequestError.MalformattedUrl:
				return "URL is Malformatted";
			case UnityWebRequest.UnityWebRequestError.CannotResolveProxy:
				return "Unable to resolve specified proxy server";
			case UnityWebRequest.UnityWebRequestError.CannotResolveHost:
				return "Unable to resolve host specified in URL";
			case UnityWebRequest.UnityWebRequestError.CannotConnectToHost:
				return "Unable to connect to host specified in URL";
			case UnityWebRequest.UnityWebRequestError.AccessDenied:
				return "Remote server denied access to the specified URL";
			case UnityWebRequest.UnityWebRequestError.GenericHTTPError:
				return "Unknown/Generic HTTP Error - Check HTTP Error code";
			case UnityWebRequest.UnityWebRequestError.WriteError:
				return "Error when transmitting request to remote server - transmission terminated prematurely";
			case UnityWebRequest.UnityWebRequestError.ReadError:
				return "Error when reading response from remote server - transmission terminated prematurely";
			case UnityWebRequest.UnityWebRequestError.OutOfMemory:
				return "Out of Memory";
			case UnityWebRequest.UnityWebRequestError.Timeout:
				return "Timeout occurred while waiting for response from remote server";
			case UnityWebRequest.UnityWebRequestError.HTTPPostError:
				return "Error while transmitting HTTP POST body data";
			case UnityWebRequest.UnityWebRequestError.SSLCannotConnect:
				return "Unable to connect to SSL server at remote host";
			case UnityWebRequest.UnityWebRequestError.Aborted:
				return "Request was manually aborted by local code";
			case UnityWebRequest.UnityWebRequestError.TooManyRedirects:
				return "Redirect limit exceeded";
			case UnityWebRequest.UnityWebRequestError.ReceivedNoData:
				return "Received an empty response from remote host";
			case UnityWebRequest.UnityWebRequestError.SSLNotSupported:
				return "SSL connections are not supported on the local machine";
			case UnityWebRequest.UnityWebRequestError.FailedToSendData:
				return "Failed to transmit body data";
			case UnityWebRequest.UnityWebRequestError.FailedToReceiveData:
				return "Failed to receive response body data";
			case UnityWebRequest.UnityWebRequestError.SSLCertificateError:
				return "Failure to authenticate SSL certificate of remote host";
			case UnityWebRequest.UnityWebRequestError.SSLCipherNotAvailable:
				return "SSL cipher received from remote host is not supported on the local machine";
			case UnityWebRequest.UnityWebRequestError.SSLCACertError:
				return "Failure to authenticate Certificate Authority of the SSL certificate received from the remote host";
			case UnityWebRequest.UnityWebRequestError.UnrecognizedContentEncoding:
				return "Remote host returned data with an unrecognized/unparseable content encoding";
			case UnityWebRequest.UnityWebRequestError.LoginFailed:
				return "HTTP authentication failed";
			case UnityWebRequest.UnityWebRequestError.SSLShutdownFailed:
				return "Failure while shutting down SSL connection";
			}
			return "Unknown error";
		}

		[NonSerialized]
		internal IntPtr m_Ptr;

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
			GenericHTTPError,
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
