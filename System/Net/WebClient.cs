using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Cache;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	public class WebClient : Component
	{
		public WebClient()
		{
			if (base.GetType() == typeof(WebClient))
			{
				GC.SuppressFinalize(this);
			}
		}

		public event DownloadStringCompletedEventHandler DownloadStringCompleted;

		public event DownloadDataCompletedEventHandler DownloadDataCompleted;

		public event AsyncCompletedEventHandler DownloadFileCompleted;

		public event UploadStringCompletedEventHandler UploadStringCompleted;

		public event UploadDataCompletedEventHandler UploadDataCompleted;

		public event UploadFileCompletedEventHandler UploadFileCompleted;

		public event UploadValuesCompletedEventHandler UploadValuesCompleted;

		public event OpenReadCompletedEventHandler OpenReadCompleted;

		public event OpenWriteCompletedEventHandler OpenWriteCompleted;

		public event DownloadProgressChangedEventHandler DownloadProgressChanged;

		public event UploadProgressChangedEventHandler UploadProgressChanged;

		protected virtual void OnDownloadStringCompleted(DownloadStringCompletedEventArgs e)
		{
			DownloadStringCompletedEventHandler downloadStringCompleted = this.DownloadStringCompleted;
			if (downloadStringCompleted == null)
			{
				return;
			}
			downloadStringCompleted(this, e);
		}

		protected virtual void OnDownloadDataCompleted(DownloadDataCompletedEventArgs e)
		{
			DownloadDataCompletedEventHandler downloadDataCompleted = this.DownloadDataCompleted;
			if (downloadDataCompleted == null)
			{
				return;
			}
			downloadDataCompleted(this, e);
		}

		protected virtual void OnDownloadFileCompleted(AsyncCompletedEventArgs e)
		{
			AsyncCompletedEventHandler downloadFileCompleted = this.DownloadFileCompleted;
			if (downloadFileCompleted == null)
			{
				return;
			}
			downloadFileCompleted(this, e);
		}

		protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
		{
			DownloadProgressChangedEventHandler downloadProgressChanged = this.DownloadProgressChanged;
			if (downloadProgressChanged == null)
			{
				return;
			}
			downloadProgressChanged(this, e);
		}

		protected virtual void OnUploadStringCompleted(UploadStringCompletedEventArgs e)
		{
			UploadStringCompletedEventHandler uploadStringCompleted = this.UploadStringCompleted;
			if (uploadStringCompleted == null)
			{
				return;
			}
			uploadStringCompleted(this, e);
		}

		protected virtual void OnUploadDataCompleted(UploadDataCompletedEventArgs e)
		{
			UploadDataCompletedEventHandler uploadDataCompleted = this.UploadDataCompleted;
			if (uploadDataCompleted == null)
			{
				return;
			}
			uploadDataCompleted(this, e);
		}

		protected virtual void OnUploadFileCompleted(UploadFileCompletedEventArgs e)
		{
			UploadFileCompletedEventHandler uploadFileCompleted = this.UploadFileCompleted;
			if (uploadFileCompleted == null)
			{
				return;
			}
			uploadFileCompleted(this, e);
		}

		protected virtual void OnUploadValuesCompleted(UploadValuesCompletedEventArgs e)
		{
			UploadValuesCompletedEventHandler uploadValuesCompleted = this.UploadValuesCompleted;
			if (uploadValuesCompleted == null)
			{
				return;
			}
			uploadValuesCompleted(this, e);
		}

		protected virtual void OnUploadProgressChanged(UploadProgressChangedEventArgs e)
		{
			UploadProgressChangedEventHandler uploadProgressChanged = this.UploadProgressChanged;
			if (uploadProgressChanged == null)
			{
				return;
			}
			uploadProgressChanged(this, e);
		}

		protected virtual void OnOpenReadCompleted(OpenReadCompletedEventArgs e)
		{
			OpenReadCompletedEventHandler openReadCompleted = this.OpenReadCompleted;
			if (openReadCompleted == null)
			{
				return;
			}
			openReadCompleted(this, e);
		}

		protected virtual void OnOpenWriteCompleted(OpenWriteCompletedEventArgs e)
		{
			OpenWriteCompletedEventHandler openWriteCompleted = this.OpenWriteCompleted;
			if (openWriteCompleted == null)
			{
				return;
			}
			openWriteCompleted(this, e);
		}

		private void StartOperation()
		{
			if (Interlocked.Increment(ref this._callNesting) > 1)
			{
				this.EndOperation();
				throw new NotSupportedException("WebClient does not support concurrent I/O operations.");
			}
			this._contentLength = -1L;
			this._webResponse = null;
			this._webRequest = null;
			this._method = null;
			this._canceled = false;
			WebClient.ProgressData progress = this._progress;
			if (progress == null)
			{
				return;
			}
			progress.Reset();
		}

		private AsyncOperation StartAsyncOperation(object userToken)
		{
			if (!this._initWebClientAsync)
			{
				this._openReadOperationCompleted = delegate(object arg)
				{
					this.OnOpenReadCompleted((OpenReadCompletedEventArgs)arg);
				};
				this._openWriteOperationCompleted = delegate(object arg)
				{
					this.OnOpenWriteCompleted((OpenWriteCompletedEventArgs)arg);
				};
				this._downloadStringOperationCompleted = delegate(object arg)
				{
					this.OnDownloadStringCompleted((DownloadStringCompletedEventArgs)arg);
				};
				this._downloadDataOperationCompleted = delegate(object arg)
				{
					this.OnDownloadDataCompleted((DownloadDataCompletedEventArgs)arg);
				};
				this._downloadFileOperationCompleted = delegate(object arg)
				{
					this.OnDownloadFileCompleted((AsyncCompletedEventArgs)arg);
				};
				this._uploadStringOperationCompleted = delegate(object arg)
				{
					this.OnUploadStringCompleted((UploadStringCompletedEventArgs)arg);
				};
				this._uploadDataOperationCompleted = delegate(object arg)
				{
					this.OnUploadDataCompleted((UploadDataCompletedEventArgs)arg);
				};
				this._uploadFileOperationCompleted = delegate(object arg)
				{
					this.OnUploadFileCompleted((UploadFileCompletedEventArgs)arg);
				};
				this._uploadValuesOperationCompleted = delegate(object arg)
				{
					this.OnUploadValuesCompleted((UploadValuesCompletedEventArgs)arg);
				};
				this._reportDownloadProgressChanged = delegate(object arg)
				{
					this.OnDownloadProgressChanged((DownloadProgressChangedEventArgs)arg);
				};
				this._reportUploadProgressChanged = delegate(object arg)
				{
					this.OnUploadProgressChanged((UploadProgressChangedEventArgs)arg);
				};
				this._progress = new WebClient.ProgressData();
				this._initWebClientAsync = true;
			}
			AsyncOperation asyncOperation = AsyncOperationManager.CreateOperation(userToken);
			this.StartOperation();
			this._asyncOp = asyncOperation;
			return asyncOperation;
		}

		private void EndOperation()
		{
			Interlocked.Decrement(ref this._callNesting);
		}

		public Encoding Encoding
		{
			get
			{
				return this._encoding;
			}
			set
			{
				WebClient.ThrowIfNull(value, "Encoding");
				this._encoding = value;
			}
		}

		public string BaseAddress
		{
			get
			{
				if (!(this._baseAddress != null))
				{
					return string.Empty;
				}
				return this._baseAddress.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this._baseAddress = null;
					return;
				}
				try
				{
					this._baseAddress = new Uri(value);
				}
				catch (UriFormatException ex)
				{
					throw new ArgumentException("The specified value is not a valid base address.", "value", ex);
				}
			}
		}

		public ICredentials Credentials
		{
			get
			{
				return this._credentials;
			}
			set
			{
				this._credentials = value;
			}
		}

		public bool UseDefaultCredentials
		{
			get
			{
				return this._credentials == CredentialCache.DefaultCredentials;
			}
			set
			{
				this._credentials = (value ? CredentialCache.DefaultCredentials : null);
			}
		}

		public WebHeaderCollection Headers
		{
			get
			{
				WebHeaderCollection webHeaderCollection;
				if ((webHeaderCollection = this._headers) == null)
				{
					webHeaderCollection = (this._headers = new WebHeaderCollection());
				}
				return webHeaderCollection;
			}
			set
			{
				this._headers = value;
			}
		}

		public NameValueCollection QueryString
		{
			get
			{
				NameValueCollection nameValueCollection;
				if ((nameValueCollection = this._requestParameters) == null)
				{
					nameValueCollection = (this._requestParameters = new NameValueCollection());
				}
				return nameValueCollection;
			}
			set
			{
				this._requestParameters = value;
			}
		}

		public WebHeaderCollection ResponseHeaders
		{
			get
			{
				WebResponse webResponse = this._webResponse;
				if (webResponse == null)
				{
					return null;
				}
				return webResponse.Headers;
			}
		}

		public IWebProxy Proxy
		{
			get
			{
				if (!this._proxySet)
				{
					return WebRequest.DefaultWebProxy;
				}
				return this._proxy;
			}
			set
			{
				this._proxy = value;
				this._proxySet = true;
			}
		}

		public RequestCachePolicy CachePolicy { get; set; }

		public bool IsBusy
		{
			get
			{
				return this._asyncOp != null;
			}
		}

		protected virtual WebRequest GetWebRequest(Uri address)
		{
			WebRequest webRequest = WebRequest.Create(address);
			this.CopyHeadersTo(webRequest);
			if (this.Credentials != null)
			{
				webRequest.Credentials = this.Credentials;
			}
			if (this._method != null)
			{
				webRequest.Method = this._method;
			}
			if (this._contentLength != -1L)
			{
				webRequest.ContentLength = this._contentLength;
			}
			if (this._proxySet)
			{
				webRequest.Proxy = this._proxy;
			}
			if (this.CachePolicy != null)
			{
				webRequest.CachePolicy = this.CachePolicy;
			}
			return webRequest;
		}

		protected virtual WebResponse GetWebResponse(WebRequest request)
		{
			WebResponse response = request.GetResponse();
			this._webResponse = response;
			return response;
		}

		protected virtual WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
		{
			WebResponse webResponse = request.EndGetResponse(result);
			this._webResponse = webResponse;
			return webResponse;
		}

		private async Task<WebResponse> GetWebResponseTaskAsync(WebRequest request)
		{
			BeginEndAwaitableAdapter beginEndAwaitableAdapter = new BeginEndAwaitableAdapter();
			request.BeginGetResponse(BeginEndAwaitableAdapter.Callback, beginEndAwaitableAdapter);
			IAsyncResult asyncResult = await beginEndAwaitableAdapter;
			return this.GetWebResponse(request, asyncResult);
		}

		public byte[] DownloadData(string address)
		{
			return this.DownloadData(this.GetUri(address));
		}

		public byte[] DownloadData(Uri address)
		{
			WebClient.ThrowIfNull(address, "address");
			this.StartOperation();
			byte[] array;
			try
			{
				WebRequest webRequest;
				array = this.DownloadDataInternal(address, out webRequest);
			}
			finally
			{
				this.EndOperation();
			}
			return array;
		}

		private byte[] DownloadDataInternal(Uri address, out WebRequest request)
		{
			request = null;
			byte[] array;
			try
			{
				request = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				array = this.DownloadBits(request, new ChunkedMemoryStream());
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(request);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			return array;
		}

		public void DownloadFile(string address, string fileName)
		{
			this.DownloadFile(this.GetUri(address), fileName);
		}

		public void DownloadFile(Uri address, string fileName)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(fileName, "fileName");
			WebRequest webRequest = null;
			FileStream fileStream = null;
			bool flag = false;
			this.StartOperation();
			try
			{
				fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
				webRequest = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				this.DownloadBits(webRequest, fileStream);
				flag = true;
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(webRequest);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
					if (!flag)
					{
						File.Delete(fileName);
					}
				}
				this.EndOperation();
			}
		}

		public Stream OpenRead(string address)
		{
			return this.OpenRead(this.GetUri(address));
		}

		public Stream OpenRead(Uri address)
		{
			WebClient.ThrowIfNull(address, "address");
			WebRequest webRequest = null;
			this.StartOperation();
			Stream responseStream;
			try
			{
				webRequest = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				responseStream = (this._webResponse = this.GetWebResponse(webRequest)).GetResponseStream();
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(webRequest);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			finally
			{
				this.EndOperation();
			}
			return responseStream;
		}

		public Stream OpenWrite(string address)
		{
			return this.OpenWrite(this.GetUri(address), null);
		}

		public Stream OpenWrite(Uri address)
		{
			return this.OpenWrite(address, null);
		}

		public Stream OpenWrite(string address, string method)
		{
			return this.OpenWrite(this.GetUri(address), method);
		}

		public Stream OpenWrite(Uri address, string method)
		{
			WebClient.ThrowIfNull(address, "address");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			WebRequest webRequest = null;
			this.StartOperation();
			Stream stream;
			try
			{
				this._method = method;
				webRequest = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				stream = new WebClient.WebClientWriteStream(webRequest.GetRequestStream(), webRequest, this);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(webRequest);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			finally
			{
				this.EndOperation();
			}
			return stream;
		}

		public byte[] UploadData(string address, byte[] data)
		{
			return this.UploadData(this.GetUri(address), null, data);
		}

		public byte[] UploadData(Uri address, byte[] data)
		{
			return this.UploadData(address, null, data);
		}

		public byte[] UploadData(string address, string method, byte[] data)
		{
			return this.UploadData(this.GetUri(address), method, data);
		}

		public byte[] UploadData(Uri address, string method, byte[] data)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(data, "data");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			this.StartOperation();
			byte[] array;
			try
			{
				WebRequest webRequest;
				array = this.UploadDataInternal(address, method, data, out webRequest);
			}
			finally
			{
				this.EndOperation();
			}
			return array;
		}

		private byte[] UploadDataInternal(Uri address, string method, byte[] data, out WebRequest request)
		{
			request = null;
			byte[] array;
			try
			{
				this._method = method;
				this._contentLength = (long)data.Length;
				request = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				array = this.UploadBits(request, null, data, 0, null, null);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(request);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			return array;
		}

		private void OpenFileInternal(bool needsHeaderAndBoundary, string fileName, ref FileStream fs, ref byte[] buffer, ref byte[] formHeaderBytes, ref byte[] boundaryBytes)
		{
			fileName = Path.GetFullPath(fileName);
			WebHeaderCollection headers = this.Headers;
			string text = headers["Content-Type"];
			if (text == null)
			{
				text = "application/octet-stream";
			}
			else if (text.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase))
			{
				throw new WebException("The Content-Type header cannot be set to a multipart type for this request.");
			}
			fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			int num = 8192;
			this._contentLength = -1L;
			if (string.Equals(this._method, "POST", StringComparison.Ordinal))
			{
				if (needsHeaderAndBoundary)
				{
					string text2 = "---------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
					headers["Content-Type"] = "multipart/form-data; boundary=" + text2;
					string text3 = string.Concat(new string[]
					{
						"--",
						text2,
						"\r\nContent-Disposition: form-data; name=\"file\"; filename=\"",
						Path.GetFileName(fileName),
						"\"\r\nContent-Type: ",
						text,
						"\r\n\r\n"
					});
					formHeaderBytes = Encoding.UTF8.GetBytes(text3);
					boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + text2 + "--\r\n");
				}
				else
				{
					formHeaderBytes = Array.Empty<byte>();
					boundaryBytes = Array.Empty<byte>();
				}
				if (fs.CanSeek)
				{
					this._contentLength = fs.Length + (long)formHeaderBytes.Length + (long)boundaryBytes.Length;
					num = (int)Math.Min(8192L, fs.Length);
				}
			}
			else
			{
				headers["Content-Type"] = text;
				formHeaderBytes = null;
				boundaryBytes = null;
				if (fs.CanSeek)
				{
					this._contentLength = fs.Length;
					num = (int)Math.Min(8192L, fs.Length);
				}
			}
			buffer = new byte[num];
		}

		public byte[] UploadFile(string address, string fileName)
		{
			return this.UploadFile(this.GetUri(address), fileName);
		}

		public byte[] UploadFile(Uri address, string fileName)
		{
			return this.UploadFile(address, null, fileName);
		}

		public byte[] UploadFile(string address, string method, string fileName)
		{
			return this.UploadFile(this.GetUri(address), method, fileName);
		}

		public byte[] UploadFile(Uri address, string method, string fileName)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(fileName, "fileName");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			FileStream fileStream = null;
			WebRequest webRequest = null;
			this.StartOperation();
			byte[] array4;
			try
			{
				this._method = method;
				byte[] array = null;
				byte[] array2 = null;
				byte[] array3 = null;
				Uri uri = this.GetUri(address);
				bool flag = uri.Scheme != Uri.UriSchemeFile;
				this.OpenFileInternal(flag, fileName, ref fileStream, ref array3, ref array, ref array2);
				webRequest = (this._webRequest = this.GetWebRequest(uri));
				array4 = this.UploadBits(webRequest, fileStream, array3, 0, array, array2);
			}
			catch (Exception ex)
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				if (ex is OutOfMemoryException)
				{
					throw;
				}
				WebClient.AbortRequest(webRequest);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			finally
			{
				this.EndOperation();
			}
			return array4;
		}

		private byte[] GetValuesToUpload(NameValueCollection data)
		{
			WebHeaderCollection headers = this.Headers;
			string text = headers["Content-Type"];
			if (text != null && !string.Equals(text, "application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
			{
				throw new WebException("The Content-Type header cannot be changed from its default value for this request.");
			}
			headers["Content-Type"] = "application/x-www-form-urlencoded";
			string text2 = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text3 in data.AllKeys)
			{
				stringBuilder.Append(text2);
				stringBuilder.Append(WebClient.UrlEncode(text3));
				stringBuilder.Append('=');
				stringBuilder.Append(WebClient.UrlEncode(data[text3]));
				text2 = "&";
			}
			byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
			this._contentLength = (long)bytes.Length;
			return bytes;
		}

		public byte[] UploadValues(string address, NameValueCollection data)
		{
			return this.UploadValues(this.GetUri(address), null, data);
		}

		public byte[] UploadValues(Uri address, NameValueCollection data)
		{
			return this.UploadValues(address, null, data);
		}

		public byte[] UploadValues(string address, string method, NameValueCollection data)
		{
			return this.UploadValues(this.GetUri(address), method, data);
		}

		public byte[] UploadValues(Uri address, string method, NameValueCollection data)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(data, "data");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			WebRequest webRequest = null;
			this.StartOperation();
			byte[] array;
			try
			{
				byte[] valuesToUpload = this.GetValuesToUpload(data);
				this._method = method;
				webRequest = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				array = this.UploadBits(webRequest, null, valuesToUpload, 0, null, null);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(webRequest);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			finally
			{
				this.EndOperation();
			}
			return array;
		}

		public string UploadString(string address, string data)
		{
			return this.UploadString(this.GetUri(address), null, data);
		}

		public string UploadString(Uri address, string data)
		{
			return this.UploadString(address, null, data);
		}

		public string UploadString(string address, string method, string data)
		{
			return this.UploadString(this.GetUri(address), method, data);
		}

		public string UploadString(Uri address, string method, string data)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(data, "data");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			this.StartOperation();
			string stringUsingEncoding;
			try
			{
				byte[] bytes = this.Encoding.GetBytes(data);
				WebRequest webRequest;
				byte[] array = this.UploadDataInternal(address, method, bytes, out webRequest);
				stringUsingEncoding = this.GetStringUsingEncoding(webRequest, array);
			}
			finally
			{
				this.EndOperation();
			}
			return stringUsingEncoding;
		}

		public string DownloadString(string address)
		{
			return this.DownloadString(this.GetUri(address));
		}

		public string DownloadString(Uri address)
		{
			WebClient.ThrowIfNull(address, "address");
			this.StartOperation();
			string stringUsingEncoding;
			try
			{
				WebRequest webRequest;
				byte[] array = this.DownloadDataInternal(address, out webRequest);
				stringUsingEncoding = this.GetStringUsingEncoding(webRequest, array);
			}
			finally
			{
				this.EndOperation();
			}
			return stringUsingEncoding;
		}

		private static void AbortRequest(WebRequest request)
		{
			try
			{
				if (request != null)
				{
					request.Abort();
				}
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
			}
		}

		private void CopyHeadersTo(WebRequest request)
		{
			if (this._headers == null)
			{
				return;
			}
			HttpWebRequest httpWebRequest = request as HttpWebRequest;
			if (httpWebRequest == null)
			{
				return;
			}
			string text = this._headers["Accept"];
			string text2 = this._headers["Connection"];
			string text3 = this._headers["Content-Type"];
			string text4 = this._headers["Expect"];
			string text5 = this._headers["Referer"];
			string text6 = this._headers["User-Agent"];
			string text7 = this._headers["Host"];
			this._headers.Remove("Accept");
			this._headers.Remove("Connection");
			this._headers.Remove("Content-Type");
			this._headers.Remove("Expect");
			this._headers.Remove("Referer");
			this._headers.Remove("User-Agent");
			this._headers.Remove("Host");
			request.Headers = this._headers;
			if (!string.IsNullOrEmpty(text))
			{
				httpWebRequest.Accept = text;
			}
			if (!string.IsNullOrEmpty(text2))
			{
				httpWebRequest.Connection = text2;
			}
			if (!string.IsNullOrEmpty(text3))
			{
				httpWebRequest.ContentType = text3;
			}
			if (!string.IsNullOrEmpty(text4))
			{
				httpWebRequest.Expect = text4;
			}
			if (!string.IsNullOrEmpty(text5))
			{
				httpWebRequest.Referer = text5;
			}
			if (!string.IsNullOrEmpty(text6))
			{
				httpWebRequest.UserAgent = text6;
			}
			if (!string.IsNullOrEmpty(text7))
			{
				httpWebRequest.Host = text7;
			}
		}

		private Uri GetUri(string address)
		{
			WebClient.ThrowIfNull(address, "address");
			Uri uri;
			if (this._baseAddress != null)
			{
				if (!Uri.TryCreate(this._baseAddress, address, out uri))
				{
					return new Uri(Path.GetFullPath(address));
				}
			}
			else if (!Uri.TryCreate(address, UriKind.Absolute, out uri))
			{
				return new Uri(Path.GetFullPath(address));
			}
			return this.GetUri(uri);
		}

		private Uri GetUri(Uri address)
		{
			WebClient.ThrowIfNull(address, "address");
			Uri uri = address;
			if (!address.IsAbsoluteUri && this._baseAddress != null && !Uri.TryCreate(this._baseAddress, address, out uri))
			{
				return address;
			}
			if (string.IsNullOrEmpty(uri.Query) && this._requestParameters != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string text = string.Empty;
				for (int i = 0; i < this._requestParameters.Count; i++)
				{
					stringBuilder.Append(text).Append(this._requestParameters.AllKeys[i]).Append('=')
						.Append(this._requestParameters[i]);
					text = "&";
				}
				uri = new UriBuilder(uri)
				{
					Query = stringBuilder.ToString()
				}.Uri;
			}
			return uri;
		}

		private byte[] DownloadBits(WebRequest request, Stream writeStream)
		{
			byte[] array2;
			try
			{
				WebResponse webResponse = (this._webResponse = this.GetWebResponse(request));
				long contentLength = webResponse.ContentLength;
				byte[] array = new byte[(contentLength == -1L || contentLength > 65536L) ? 65536L : contentLength];
				if (writeStream is ChunkedMemoryStream)
				{
					if (contentLength > 2147483647L)
					{
						throw new WebException("The message length limit was exceeded", WebExceptionStatus.MessageLengthLimitExceeded);
					}
					writeStream.SetLength((long)array.Length);
				}
				using (Stream responseStream = webResponse.GetResponseStream())
				{
					if (responseStream != null)
					{
						int num;
						while ((num = responseStream.Read(array, 0, array.Length)) != 0)
						{
							writeStream.Write(array, 0, num);
						}
					}
				}
				ChunkedMemoryStream chunkedMemoryStream = writeStream as ChunkedMemoryStream;
				array2 = ((chunkedMemoryStream != null) ? chunkedMemoryStream.ToArray() : null);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				if (writeStream != null)
				{
					writeStream.Close();
				}
				WebClient.AbortRequest(request);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			return array2;
		}

		private async void DownloadBitsAsync(WebRequest request, Stream writeStream, AsyncOperation asyncOp, Action<byte[], Exception, AsyncOperation> completionDelegate)
		{
			Exception exception = null;
			try
			{
				WebResponse webResponse = await this.GetWebResponseTaskAsync(request).ConfigureAwait(false);
				WebResponse webResponse2 = webResponse;
				this._webResponse = webResponse2;
				WebResponse webResponse3 = webResponse2;
				long contentLength = webResponse3.ContentLength;
				byte[] copyBuffer = new byte[(contentLength == -1L || contentLength > 65536L) ? 65536L : contentLength];
				if (writeStream is ChunkedMemoryStream)
				{
					if (contentLength > 2147483647L)
					{
						throw new WebException("The message length limit was exceeded", WebExceptionStatus.MessageLengthLimitExceeded);
					}
					writeStream.SetLength((long)copyBuffer.Length);
				}
				if (contentLength >= 0L)
				{
					this._progress.TotalBytesToReceive = contentLength;
				}
				using (writeStream)
				{
					using (Stream readStream = webResponse3.GetResponseStream())
					{
						if (readStream != null)
						{
							for (;;)
							{
								int num = await readStream.ReadAsync(new Memory<byte>(copyBuffer), default(CancellationToken)).ConfigureAwait(false);
								if (num == 0)
								{
									break;
								}
								this._progress.BytesReceived += (long)num;
								if (this._progress.BytesReceived != this._progress.TotalBytesToReceive)
								{
									this.PostProgressChanged(asyncOp, this._progress);
								}
								await writeStream.WriteAsync(new ReadOnlyMemory<byte>(copyBuffer, 0, num), default(CancellationToken)).ConfigureAwait(false);
							}
						}
						if (this._progress.TotalBytesToReceive < 0L)
						{
							this._progress.TotalBytesToReceive = this._progress.BytesReceived;
						}
						this.PostProgressChanged(asyncOp, this._progress);
					}
					Stream readStream = null;
				}
				Stream stream = null;
				ChunkedMemoryStream chunkedMemoryStream = writeStream as ChunkedMemoryStream;
				completionDelegate((chunkedMemoryStream != null) ? chunkedMemoryStream.ToArray() : null, null, asyncOp);
				copyBuffer = null;
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				exception = WebClient.GetExceptionToPropagate(ex);
				WebClient.AbortRequest(request);
				if (writeStream != null)
				{
					writeStream.Close();
				}
			}
			finally
			{
				if (exception != null)
				{
					completionDelegate(null, exception, asyncOp);
				}
			}
		}

		private byte[] UploadBits(WebRequest request, Stream readStream, byte[] buffer, int chunkSize, byte[] header, byte[] footer)
		{
			byte[] array;
			try
			{
				if (request.RequestUri.Scheme == Uri.UriSchemeFile)
				{
					footer = (header = null);
				}
				using (Stream requestStream = request.GetRequestStream())
				{
					if (header != null)
					{
						requestStream.Write(header, 0, header.Length);
					}
					if (readStream != null)
					{
						try
						{
							for (;;)
							{
								int num = readStream.Read(buffer, 0, buffer.Length);
								if (num <= 0)
								{
									break;
								}
								requestStream.Write(buffer, 0, num);
							}
							goto IL_008F;
						}
						finally
						{
							if (readStream != null)
							{
								((IDisposable)readStream).Dispose();
							}
						}
					}
					int num2;
					for (int i = 0; i < buffer.Length; i += num2)
					{
						num2 = buffer.Length - i;
						if (chunkSize != 0 && num2 > chunkSize)
						{
							num2 = chunkSize;
						}
						requestStream.Write(buffer, i, num2);
					}
					IL_008F:
					if (footer != null)
					{
						requestStream.Write(footer, 0, footer.Length);
					}
				}
				array = this.DownloadBits(request, new ChunkedMemoryStream());
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(request);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			return array;
		}

		private async void UploadBitsAsync(WebRequest request, Stream readStream, byte[] buffer, int chunkSize, byte[] header, byte[] footer, AsyncOperation asyncOp, Action<byte[], Exception, AsyncOperation> completionDelegate)
		{
			this._progress.HasUploadPhase = true;
			Exception exception = null;
			try
			{
				if (request.RequestUri.Scheme == Uri.UriSchemeFile)
				{
					header = (footer = null);
				}
				Stream stream = await request.GetRequestStreamAsync().ConfigureAwait(false);
				using (Stream writeStream = stream)
				{
					if (header != null)
					{
						await writeStream.WriteAsync(new ReadOnlyMemory<byte>(header), default(CancellationToken)).ConfigureAwait(false);
						this._progress.BytesSent += (long)header.Length;
						this.PostProgressChanged(asyncOp, this._progress);
					}
					if (readStream != null)
					{
						using (readStream)
						{
							for (;;)
							{
								int bytesRead = await readStream.ReadAsync(new Memory<byte>(buffer), default(CancellationToken)).ConfigureAwait(false);
								if (bytesRead <= 0)
								{
									break;
								}
								await writeStream.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, bytesRead), default(CancellationToken)).ConfigureAwait(false);
								this._progress.BytesSent += (long)bytesRead;
								this.PostProgressChanged(asyncOp, this._progress);
							}
						}
						Stream stream2 = null;
					}
					else
					{
						int bytesRead = 0;
						while (bytesRead < buffer.Length)
						{
							int toWrite = buffer.Length - bytesRead;
							if (chunkSize != 0 && toWrite > chunkSize)
							{
								toWrite = chunkSize;
							}
							await writeStream.WriteAsync(new ReadOnlyMemory<byte>(buffer, bytesRead, toWrite), default(CancellationToken)).ConfigureAwait(false);
							bytesRead += toWrite;
							this._progress.BytesSent += (long)toWrite;
							this.PostProgressChanged(asyncOp, this._progress);
						}
					}
					if (footer != null)
					{
						await writeStream.WriteAsync(new ReadOnlyMemory<byte>(footer), default(CancellationToken)).ConfigureAwait(false);
						this._progress.BytesSent += (long)footer.Length;
						this.PostProgressChanged(asyncOp, this._progress);
					}
				}
				Stream writeStream = null;
				this.DownloadBitsAsync(request, new ChunkedMemoryStream(), asyncOp, completionDelegate);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				exception = WebClient.GetExceptionToPropagate(ex);
				WebClient.AbortRequest(request);
			}
			finally
			{
				if (exception != null)
				{
					completionDelegate(null, exception, asyncOp);
				}
			}
		}

		private static bool ByteArrayHasPrefix(byte[] prefix, byte[] byteArray)
		{
			if (prefix == null || byteArray == null || prefix.Length > byteArray.Length)
			{
				return false;
			}
			for (int i = 0; i < prefix.Length; i++)
			{
				if (prefix[i] != byteArray[i])
				{
					return false;
				}
			}
			return true;
		}

		private string GetStringUsingEncoding(WebRequest request, byte[] data)
		{
			Encoding encoding = null;
			int num = -1;
			string text;
			try
			{
				text = request.ContentType;
			}
			catch (Exception ex) when (ex is NotImplementedException || ex is NotSupportedException)
			{
				text = null;
			}
			if (text != null)
			{
				text = text.ToLower(CultureInfo.InvariantCulture);
				string[] array = text.Split(WebClient.s_parseContentTypeSeparators);
				bool flag = false;
				foreach (string text2 in array)
				{
					if (text2 == "charset")
					{
						flag = true;
					}
					else if (flag)
					{
						try
						{
							encoding = Encoding.GetEncoding(text2);
						}
						catch (ArgumentException)
						{
							break;
						}
					}
				}
			}
			if (encoding == null)
			{
				Encoding[] array3 = WebClient.s_knownEncodings;
				for (int j = 0; j < array3.Length; j++)
				{
					byte[] preamble = array3[j].GetPreamble();
					if (WebClient.ByteArrayHasPrefix(preamble, data))
					{
						encoding = array3[j];
						num = preamble.Length;
						break;
					}
				}
			}
			if (encoding == null)
			{
				encoding = this.Encoding;
			}
			if (num == -1)
			{
				byte[] preamble2 = encoding.GetPreamble();
				num = (WebClient.ByteArrayHasPrefix(preamble2, data) ? preamble2.Length : 0);
			}
			return encoding.GetString(data, num, data.Length - num);
		}

		private string MapToDefaultMethod(Uri address)
		{
			if (!string.Equals(((!address.IsAbsoluteUri && this._baseAddress != null) ? new Uri(this._baseAddress, address) : address).Scheme, Uri.UriSchemeFtp, StringComparison.Ordinal))
			{
				return "POST";
			}
			return "STOR";
		}

		private static string UrlEncode(string str)
		{
			if (str == null)
			{
				return null;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			return Encoding.ASCII.GetString(WebClient.UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false));
		}

		private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				char c = (char)bytes[offset + i];
				if (c == ' ')
				{
					num++;
				}
				else if (!WebClient.IsSafe(c))
				{
					num2++;
				}
			}
			if (!alwaysCreateReturnValue && num == 0 && num2 == 0)
			{
				return bytes;
			}
			byte[] array = new byte[count + num2 * 2];
			int num3 = 0;
			for (int j = 0; j < count; j++)
			{
				byte b = bytes[offset + j];
				char c2 = (char)b;
				if (WebClient.IsSafe(c2))
				{
					array[num3++] = b;
				}
				else if (c2 == ' ')
				{
					array[num3++] = 43;
				}
				else
				{
					array[num3++] = 37;
					array[num3++] = (byte)WebClient.IntToHex((b >> 4) & 15);
					array[num3++] = (byte)WebClient.IntToHex((int)(b & 15));
				}
			}
			return array;
		}

		private static char IntToHex(int n)
		{
			if (n <= 9)
			{
				return (char)(n + 48);
			}
			return (char)(n - 10 + 97);
		}

		private static bool IsSafe(char ch)
		{
			if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
			{
				return true;
			}
			if (ch != '!')
			{
				switch (ch)
				{
				case '\'':
				case '(':
				case ')':
				case '*':
				case '-':
				case '.':
					return true;
				case '+':
				case ',':
					break;
				default:
					if (ch == '_')
					{
						return true;
					}
					break;
				}
				return false;
			}
			return true;
		}

		private void InvokeOperationCompleted(AsyncOperation asyncOp, SendOrPostCallback callback, AsyncCompletedEventArgs eventArgs)
		{
			if (Interlocked.CompareExchange<AsyncOperation>(ref this._asyncOp, null, asyncOp) == asyncOp)
			{
				this.EndOperation();
				asyncOp.PostOperationCompleted(callback, eventArgs);
			}
		}

		public void OpenReadAsync(Uri address)
		{
			this.OpenReadAsync(address, null);
		}

		public void OpenReadAsync(Uri address, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			AsyncOperation asyncOp = this.StartAsyncOperation(userToken);
			try
			{
				WebRequest request = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				request.BeginGetResponse(delegate(IAsyncResult iar)
				{
					Stream stream = null;
					Exception ex2 = null;
					try
					{
						stream = (this._webResponse = this.GetWebResponse(request, iar)).GetResponseStream();
					}
					catch (Exception ex3) when (!(ex3 is OutOfMemoryException))
					{
						ex2 = WebClient.GetExceptionToPropagate(ex3);
					}
					this.InvokeOperationCompleted(asyncOp, this._openReadOperationCompleted, new OpenReadCompletedEventArgs(stream, ex2, this._canceled, asyncOp.UserSuppliedState));
				}, null);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				this.InvokeOperationCompleted(asyncOp, this._openReadOperationCompleted, new OpenReadCompletedEventArgs(null, WebClient.GetExceptionToPropagate(ex), this._canceled, asyncOp.UserSuppliedState));
			}
		}

		public void OpenWriteAsync(Uri address)
		{
			this.OpenWriteAsync(address, null, null);
		}

		public void OpenWriteAsync(Uri address, string method)
		{
			this.OpenWriteAsync(address, method, null);
		}

		public void OpenWriteAsync(Uri address, string method, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			AsyncOperation asyncOp = this.StartAsyncOperation(userToken);
			try
			{
				this._method = method;
				WebRequest request = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				request.BeginGetRequestStream(delegate(IAsyncResult iar)
				{
					WebClient.WebClientWriteStream webClientWriteStream = null;
					Exception ex2 = null;
					try
					{
						webClientWriteStream = new WebClient.WebClientWriteStream(request.EndGetRequestStream(iar), request, this);
					}
					catch (Exception ex3) when (!(ex3 is OutOfMemoryException))
					{
						ex2 = WebClient.GetExceptionToPropagate(ex3);
					}
					this.InvokeOperationCompleted(asyncOp, this._openWriteOperationCompleted, new OpenWriteCompletedEventArgs(webClientWriteStream, ex2, this._canceled, asyncOp.UserSuppliedState));
				}, null);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				OpenWriteCompletedEventArgs e = new OpenWriteCompletedEventArgs(null, WebClient.GetExceptionToPropagate(ex), this._canceled, asyncOp.UserSuppliedState);
				this.InvokeOperationCompleted(asyncOp, this._openWriteOperationCompleted, e);
			}
		}

		private void DownloadStringAsyncCallback(byte[] returnBytes, Exception exception, object state)
		{
			AsyncOperation asyncOperation = (AsyncOperation)state;
			string text = null;
			try
			{
				if (returnBytes != null)
				{
					text = this.GetStringUsingEncoding(this._webRequest, returnBytes);
				}
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				exception = WebClient.GetExceptionToPropagate(ex);
			}
			DownloadStringCompletedEventArgs e = new DownloadStringCompletedEventArgs(text, exception, this._canceled, asyncOperation.UserSuppliedState);
			this.InvokeOperationCompleted(asyncOperation, this._downloadStringOperationCompleted, e);
		}

		public void DownloadStringAsync(Uri address)
		{
			this.DownloadStringAsync(address, null);
		}

		public void DownloadStringAsync(Uri address, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			AsyncOperation asyncOperation = this.StartAsyncOperation(userToken);
			try
			{
				WebRequest webRequest = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				this.DownloadBitsAsync(webRequest, new ChunkedMemoryStream(), asyncOperation, new Action<byte[], Exception, AsyncOperation>(this.DownloadStringAsyncCallback));
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				this.DownloadStringAsyncCallback(null, WebClient.GetExceptionToPropagate(ex), asyncOperation);
			}
		}

		private void DownloadDataAsyncCallback(byte[] returnBytes, Exception exception, object state)
		{
			AsyncOperation asyncOperation = (AsyncOperation)state;
			DownloadDataCompletedEventArgs e = new DownloadDataCompletedEventArgs(returnBytes, exception, this._canceled, asyncOperation.UserSuppliedState);
			this.InvokeOperationCompleted(asyncOperation, this._downloadDataOperationCompleted, e);
		}

		public void DownloadDataAsync(Uri address)
		{
			this.DownloadDataAsync(address, null);
		}

		public void DownloadDataAsync(Uri address, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			AsyncOperation asyncOperation = this.StartAsyncOperation(userToken);
			try
			{
				WebRequest webRequest = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				this.DownloadBitsAsync(webRequest, new ChunkedMemoryStream(), asyncOperation, new Action<byte[], Exception, AsyncOperation>(this.DownloadDataAsyncCallback));
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				this.DownloadDataAsyncCallback(null, WebClient.GetExceptionToPropagate(ex), asyncOperation);
			}
		}

		private void DownloadFileAsyncCallback(byte[] returnBytes, Exception exception, object state)
		{
			AsyncOperation asyncOperation = (AsyncOperation)state;
			AsyncCompletedEventArgs e = new AsyncCompletedEventArgs(exception, this._canceled, asyncOperation.UserSuppliedState);
			this.InvokeOperationCompleted(asyncOperation, this._downloadFileOperationCompleted, e);
		}

		public void DownloadFileAsync(Uri address, string fileName)
		{
			this.DownloadFileAsync(address, fileName, null);
		}

		public void DownloadFileAsync(Uri address, string fileName, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(fileName, "fileName");
			FileStream fileStream = null;
			AsyncOperation asyncOperation = this.StartAsyncOperation(userToken);
			try
			{
				fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
				WebRequest webRequest = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				this.DownloadBitsAsync(webRequest, fileStream, asyncOperation, new Action<byte[], Exception, AsyncOperation>(this.DownloadFileAsyncCallback));
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				this.DownloadFileAsyncCallback(null, WebClient.GetExceptionToPropagate(ex), asyncOperation);
			}
		}

		public void UploadStringAsync(Uri address, string data)
		{
			this.UploadStringAsync(address, null, data, null);
		}

		public void UploadStringAsync(Uri address, string method, string data)
		{
			this.UploadStringAsync(address, method, data, null);
		}

		public void UploadStringAsync(Uri address, string method, string data, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(data, "data");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			AsyncOperation asyncOperation = this.StartAsyncOperation(userToken);
			try
			{
				byte[] bytes = this.Encoding.GetBytes(data);
				this._method = method;
				this._contentLength = (long)bytes.Length;
				WebRequest webRequest = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				this.UploadBitsAsync(webRequest, null, bytes, 0, null, null, asyncOperation, delegate(byte[] bytesResult, Exception error, AsyncOperation uploadAsyncOp)
				{
					string text = null;
					if (error == null && bytesResult != null)
					{
						try
						{
							text = this.GetStringUsingEncoding(this._webRequest, bytesResult);
						}
						catch (Exception ex2) when (!(ex2 is OutOfMemoryException))
						{
							error = WebClient.GetExceptionToPropagate(ex2);
						}
					}
					this.InvokeOperationCompleted(uploadAsyncOp, this._uploadStringOperationCompleted, new UploadStringCompletedEventArgs(text, error, this._canceled, uploadAsyncOp.UserSuppliedState));
				});
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				UploadStringCompletedEventArgs e = new UploadStringCompletedEventArgs(null, WebClient.GetExceptionToPropagate(ex), this._canceled, asyncOperation.UserSuppliedState);
				this.InvokeOperationCompleted(asyncOperation, this._uploadStringOperationCompleted, e);
			}
		}

		public void UploadDataAsync(Uri address, byte[] data)
		{
			this.UploadDataAsync(address, null, data, null);
		}

		public void UploadDataAsync(Uri address, string method, byte[] data)
		{
			this.UploadDataAsync(address, method, data, null);
		}

		public void UploadDataAsync(Uri address, string method, byte[] data, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(data, "data");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			AsyncOperation asyncOp = this.StartAsyncOperation(userToken);
			try
			{
				this._method = method;
				this._contentLength = (long)data.Length;
				WebRequest webRequest = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				int num = 0;
				if (this.UploadProgressChanged != null)
				{
					num = (int)Math.Min(8192L, (long)data.Length);
				}
				this.UploadBitsAsync(webRequest, null, data, num, null, null, asyncOp, delegate(byte[] result, Exception error, AsyncOperation uploadAsyncOp)
				{
					this.InvokeOperationCompleted(asyncOp, this._uploadDataOperationCompleted, new UploadDataCompletedEventArgs(result, error, this._canceled, uploadAsyncOp.UserSuppliedState));
				});
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				UploadDataCompletedEventArgs e = new UploadDataCompletedEventArgs(null, WebClient.GetExceptionToPropagate(ex), this._canceled, asyncOp.UserSuppliedState);
				this.InvokeOperationCompleted(asyncOp, this._uploadDataOperationCompleted, e);
			}
		}

		public void UploadFileAsync(Uri address, string fileName)
		{
			this.UploadFileAsync(address, null, fileName, null);
		}

		public void UploadFileAsync(Uri address, string method, string fileName)
		{
			this.UploadFileAsync(address, method, fileName, null);
		}

		public void UploadFileAsync(Uri address, string method, string fileName, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(fileName, "fileName");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			FileStream fileStream = null;
			AsyncOperation asyncOp = this.StartAsyncOperation(userToken);
			try
			{
				this._method = method;
				byte[] array = null;
				byte[] array2 = null;
				byte[] array3 = null;
				Uri uri = this.GetUri(address);
				bool flag = uri.Scheme != Uri.UriSchemeFile;
				this.OpenFileInternal(flag, fileName, ref fileStream, ref array3, ref array, ref array2);
				WebRequest webRequest = (this._webRequest = this.GetWebRequest(uri));
				this.UploadBitsAsync(webRequest, fileStream, array3, 0, array, array2, asyncOp, delegate(byte[] result, Exception error, AsyncOperation uploadAsyncOp)
				{
					this.InvokeOperationCompleted(asyncOp, this._uploadFileOperationCompleted, new UploadFileCompletedEventArgs(result, error, this._canceled, uploadAsyncOp.UserSuppliedState));
				});
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				UploadFileCompletedEventArgs e = new UploadFileCompletedEventArgs(null, WebClient.GetExceptionToPropagate(ex), this._canceled, asyncOp.UserSuppliedState);
				this.InvokeOperationCompleted(asyncOp, this._uploadFileOperationCompleted, e);
			}
		}

		public void UploadValuesAsync(Uri address, NameValueCollection data)
		{
			this.UploadValuesAsync(address, null, data, null);
		}

		public void UploadValuesAsync(Uri address, string method, NameValueCollection data)
		{
			this.UploadValuesAsync(address, method, data, null);
		}

		public void UploadValuesAsync(Uri address, string method, NameValueCollection data, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(data, "data");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			AsyncOperation asyncOp = this.StartAsyncOperation(userToken);
			try
			{
				byte[] valuesToUpload = this.GetValuesToUpload(data);
				this._method = method;
				WebRequest webRequest = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				int num = 0;
				if (this.UploadProgressChanged != null)
				{
					num = (int)Math.Min(8192L, (long)valuesToUpload.Length);
				}
				this.UploadBitsAsync(webRequest, null, valuesToUpload, num, null, null, asyncOp, delegate(byte[] result, Exception error, AsyncOperation uploadAsyncOp)
				{
					this.InvokeOperationCompleted(asyncOp, this._uploadValuesOperationCompleted, new UploadValuesCompletedEventArgs(result, error, this._canceled, uploadAsyncOp.UserSuppliedState));
				});
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				UploadValuesCompletedEventArgs e = new UploadValuesCompletedEventArgs(null, WebClient.GetExceptionToPropagate(ex), this._canceled, asyncOp.UserSuppliedState);
				this.InvokeOperationCompleted(asyncOp, this._uploadValuesOperationCompleted, e);
			}
		}

		private static Exception GetExceptionToPropagate(Exception e)
		{
			if (!(e is WebException) && !(e is SecurityException))
			{
				return new WebException("An exception occurred during a WebClient request.", e);
			}
			return e;
		}

		public void CancelAsync()
		{
			WebRequest webRequest = this._webRequest;
			this._canceled = true;
			WebClient.AbortRequest(webRequest);
		}

		public Task<string> DownloadStringTaskAsync(string address)
		{
			return this.DownloadStringTaskAsync(this.GetUri(address));
		}

		public Task<string> DownloadStringTaskAsync(Uri address)
		{
			TaskCompletionSource<string> tcs = new TaskCompletionSource<string>(address);
			DownloadStringCompletedEventHandler handler = null;
			handler = delegate(object sender, DownloadStringCompletedEventArgs e)
			{
				this.HandleCompletion<DownloadStringCompletedEventArgs, DownloadStringCompletedEventHandler, string>(tcs, e, (DownloadStringCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, DownloadStringCompletedEventHandler completion)
				{
					webClient.DownloadStringCompleted -= completion;
				});
			};
			this.DownloadStringCompleted += handler;
			try
			{
				this.DownloadStringAsync(address, tcs);
			}
			catch
			{
				this.DownloadStringCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		public Task<Stream> OpenReadTaskAsync(string address)
		{
			return this.OpenReadTaskAsync(this.GetUri(address));
		}

		public Task<Stream> OpenReadTaskAsync(Uri address)
		{
			TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>(address);
			OpenReadCompletedEventHandler handler = null;
			handler = delegate(object sender, OpenReadCompletedEventArgs e)
			{
				this.HandleCompletion<OpenReadCompletedEventArgs, OpenReadCompletedEventHandler, Stream>(tcs, e, (OpenReadCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, OpenReadCompletedEventHandler completion)
				{
					webClient.OpenReadCompleted -= completion;
				});
			};
			this.OpenReadCompleted += handler;
			try
			{
				this.OpenReadAsync(address, tcs);
			}
			catch
			{
				this.OpenReadCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		public Task<Stream> OpenWriteTaskAsync(string address)
		{
			return this.OpenWriteTaskAsync(this.GetUri(address), null);
		}

		public Task<Stream> OpenWriteTaskAsync(Uri address)
		{
			return this.OpenWriteTaskAsync(address, null);
		}

		public Task<Stream> OpenWriteTaskAsync(string address, string method)
		{
			return this.OpenWriteTaskAsync(this.GetUri(address), method);
		}

		public Task<Stream> OpenWriteTaskAsync(Uri address, string method)
		{
			TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>(address);
			OpenWriteCompletedEventHandler handler = null;
			handler = delegate(object sender, OpenWriteCompletedEventArgs e)
			{
				this.HandleCompletion<OpenWriteCompletedEventArgs, OpenWriteCompletedEventHandler, Stream>(tcs, e, (OpenWriteCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, OpenWriteCompletedEventHandler completion)
				{
					webClient.OpenWriteCompleted -= completion;
				});
			};
			this.OpenWriteCompleted += handler;
			try
			{
				this.OpenWriteAsync(address, method, tcs);
			}
			catch
			{
				this.OpenWriteCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		public Task<string> UploadStringTaskAsync(string address, string data)
		{
			return this.UploadStringTaskAsync(address, null, data);
		}

		public Task<string> UploadStringTaskAsync(Uri address, string data)
		{
			return this.UploadStringTaskAsync(address, null, data);
		}

		public Task<string> UploadStringTaskAsync(string address, string method, string data)
		{
			return this.UploadStringTaskAsync(this.GetUri(address), method, data);
		}

		public Task<string> UploadStringTaskAsync(Uri address, string method, string data)
		{
			TaskCompletionSource<string> tcs = new TaskCompletionSource<string>(address);
			UploadStringCompletedEventHandler handler = null;
			handler = delegate(object sender, UploadStringCompletedEventArgs e)
			{
				this.HandleCompletion<UploadStringCompletedEventArgs, UploadStringCompletedEventHandler, string>(tcs, e, (UploadStringCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, UploadStringCompletedEventHandler completion)
				{
					webClient.UploadStringCompleted -= completion;
				});
			};
			this.UploadStringCompleted += handler;
			try
			{
				this.UploadStringAsync(address, method, data, tcs);
			}
			catch
			{
				this.UploadStringCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		public Task<byte[]> DownloadDataTaskAsync(string address)
		{
			return this.DownloadDataTaskAsync(this.GetUri(address));
		}

		public Task<byte[]> DownloadDataTaskAsync(Uri address)
		{
			TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(address);
			DownloadDataCompletedEventHandler handler = null;
			handler = delegate(object sender, DownloadDataCompletedEventArgs e)
			{
				this.HandleCompletion<DownloadDataCompletedEventArgs, DownloadDataCompletedEventHandler, byte[]>(tcs, e, (DownloadDataCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, DownloadDataCompletedEventHandler completion)
				{
					webClient.DownloadDataCompleted -= completion;
				});
			};
			this.DownloadDataCompleted += handler;
			try
			{
				this.DownloadDataAsync(address, tcs);
			}
			catch
			{
				this.DownloadDataCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		public Task DownloadFileTaskAsync(string address, string fileName)
		{
			return this.DownloadFileTaskAsync(this.GetUri(address), fileName);
		}

		public Task DownloadFileTaskAsync(Uri address, string fileName)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(address);
			AsyncCompletedEventHandler handler = null;
			handler = delegate(object sender, AsyncCompletedEventArgs e)
			{
				this.HandleCompletion<AsyncCompletedEventArgs, AsyncCompletedEventHandler, object>(tcs, e, (AsyncCompletedEventArgs args) => null, handler, delegate(WebClient webClient, AsyncCompletedEventHandler completion)
				{
					webClient.DownloadFileCompleted -= completion;
				});
			};
			this.DownloadFileCompleted += handler;
			try
			{
				this.DownloadFileAsync(address, fileName, tcs);
			}
			catch
			{
				this.DownloadFileCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		public Task<byte[]> UploadDataTaskAsync(string address, byte[] data)
		{
			return this.UploadDataTaskAsync(this.GetUri(address), null, data);
		}

		public Task<byte[]> UploadDataTaskAsync(Uri address, byte[] data)
		{
			return this.UploadDataTaskAsync(address, null, data);
		}

		public Task<byte[]> UploadDataTaskAsync(string address, string method, byte[] data)
		{
			return this.UploadDataTaskAsync(this.GetUri(address), method, data);
		}

		public Task<byte[]> UploadDataTaskAsync(Uri address, string method, byte[] data)
		{
			TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(address);
			UploadDataCompletedEventHandler handler = null;
			handler = delegate(object sender, UploadDataCompletedEventArgs e)
			{
				this.HandleCompletion<UploadDataCompletedEventArgs, UploadDataCompletedEventHandler, byte[]>(tcs, e, (UploadDataCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, UploadDataCompletedEventHandler completion)
				{
					webClient.UploadDataCompleted -= completion;
				});
			};
			this.UploadDataCompleted += handler;
			try
			{
				this.UploadDataAsync(address, method, data, tcs);
			}
			catch
			{
				this.UploadDataCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		public Task<byte[]> UploadFileTaskAsync(string address, string fileName)
		{
			return this.UploadFileTaskAsync(this.GetUri(address), null, fileName);
		}

		public Task<byte[]> UploadFileTaskAsync(Uri address, string fileName)
		{
			return this.UploadFileTaskAsync(address, null, fileName);
		}

		public Task<byte[]> UploadFileTaskAsync(string address, string method, string fileName)
		{
			return this.UploadFileTaskAsync(this.GetUri(address), method, fileName);
		}

		public Task<byte[]> UploadFileTaskAsync(Uri address, string method, string fileName)
		{
			TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(address);
			UploadFileCompletedEventHandler handler = null;
			handler = delegate(object sender, UploadFileCompletedEventArgs e)
			{
				this.HandleCompletion<UploadFileCompletedEventArgs, UploadFileCompletedEventHandler, byte[]>(tcs, e, (UploadFileCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, UploadFileCompletedEventHandler completion)
				{
					webClient.UploadFileCompleted -= completion;
				});
			};
			this.UploadFileCompleted += handler;
			try
			{
				this.UploadFileAsync(address, method, fileName, tcs);
			}
			catch
			{
				this.UploadFileCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		public Task<byte[]> UploadValuesTaskAsync(string address, NameValueCollection data)
		{
			return this.UploadValuesTaskAsync(this.GetUri(address), null, data);
		}

		public Task<byte[]> UploadValuesTaskAsync(string address, string method, NameValueCollection data)
		{
			return this.UploadValuesTaskAsync(this.GetUri(address), method, data);
		}

		public Task<byte[]> UploadValuesTaskAsync(Uri address, NameValueCollection data)
		{
			return this.UploadValuesTaskAsync(address, null, data);
		}

		public Task<byte[]> UploadValuesTaskAsync(Uri address, string method, NameValueCollection data)
		{
			TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(address);
			UploadValuesCompletedEventHandler handler = null;
			handler = delegate(object sender, UploadValuesCompletedEventArgs e)
			{
				this.HandleCompletion<UploadValuesCompletedEventArgs, UploadValuesCompletedEventHandler, byte[]>(tcs, e, (UploadValuesCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, UploadValuesCompletedEventHandler completion)
				{
					webClient.UploadValuesCompleted -= completion;
				});
			};
			this.UploadValuesCompleted += handler;
			try
			{
				this.UploadValuesAsync(address, method, data, tcs);
			}
			catch
			{
				this.UploadValuesCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		private void HandleCompletion<TAsyncCompletedEventArgs, TCompletionDelegate, T>(TaskCompletionSource<T> tcs, TAsyncCompletedEventArgs e, Func<TAsyncCompletedEventArgs, T> getResult, TCompletionDelegate handler, Action<WebClient, TCompletionDelegate> unregisterHandler) where TAsyncCompletedEventArgs : AsyncCompletedEventArgs
		{
			if (e.UserState == tcs)
			{
				try
				{
					unregisterHandler(this, handler);
				}
				finally
				{
					if (e.Error != null)
					{
						tcs.TrySetException(e.Error);
					}
					else if (e.Cancelled)
					{
						tcs.TrySetCanceled();
					}
					else
					{
						tcs.TrySetResult(getResult(e));
					}
				}
			}
		}

		private void PostProgressChanged(AsyncOperation asyncOp, WebClient.ProgressData progress)
		{
			if (asyncOp != null && (progress.BytesSent > 0L || progress.BytesReceived > 0L))
			{
				if (progress.HasUploadPhase)
				{
					if (this.UploadProgressChanged != null)
					{
						int num = ((progress.TotalBytesToReceive < 0L && progress.BytesReceived == 0L) ? ((progress.TotalBytesToSend < 0L) ? 0 : ((progress.TotalBytesToSend == 0L) ? 50 : ((int)(50L * progress.BytesSent / progress.TotalBytesToSend)))) : ((progress.TotalBytesToSend < 0L) ? 50 : ((progress.TotalBytesToReceive == 0L) ? 100 : ((int)(50L * progress.BytesReceived / progress.TotalBytesToReceive + 50L)))));
						asyncOp.Post(this._reportUploadProgressChanged, new UploadProgressChangedEventArgs(num, asyncOp.UserSuppliedState, progress.BytesSent, progress.TotalBytesToSend, progress.BytesReceived, progress.TotalBytesToReceive));
						return;
					}
				}
				else if (this.DownloadProgressChanged != null)
				{
					int num = ((progress.TotalBytesToReceive < 0L) ? 0 : ((progress.TotalBytesToReceive == 0L) ? 100 : ((int)(100L * progress.BytesReceived / progress.TotalBytesToReceive))));
					asyncOp.Post(this._reportDownloadProgressChanged, new DownloadProgressChangedEventArgs(num, asyncOp.UserSuppliedState, progress.BytesReceived, progress.TotalBytesToReceive));
				}
			}
		}

		private static void ThrowIfNull(object argument, string parameterName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool AllowReadStreamBuffering { get; set; }

		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool AllowWriteStreamBuffering { get; set; }

		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event WriteStreamClosedEventHandler WriteStreamClosed
		{
			add
			{
			}
			remove
			{
			}
		}

		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void OnWriteStreamClosed(WriteStreamClosedEventArgs e)
		{
		}

		private const int DefaultCopyBufferLength = 8192;

		private const int DefaultDownloadBufferLength = 65536;

		private const string DefaultUploadFileContentType = "application/octet-stream";

		private const string UploadFileContentType = "multipart/form-data";

		private const string UploadValuesContentType = "application/x-www-form-urlencoded";

		private Uri _baseAddress;

		private ICredentials _credentials;

		private WebHeaderCollection _headers;

		private NameValueCollection _requestParameters;

		private WebResponse _webResponse;

		private WebRequest _webRequest;

		private Encoding _encoding = Encoding.Default;

		private string _method;

		private long _contentLength = -1L;

		private bool _initWebClientAsync;

		private bool _canceled;

		private WebClient.ProgressData _progress;

		private IWebProxy _proxy;

		private bool _proxySet;

		private int _callNesting;

		private AsyncOperation _asyncOp;

		private SendOrPostCallback _downloadDataOperationCompleted;

		private SendOrPostCallback _openReadOperationCompleted;

		private SendOrPostCallback _openWriteOperationCompleted;

		private SendOrPostCallback _downloadStringOperationCompleted;

		private SendOrPostCallback _downloadFileOperationCompleted;

		private SendOrPostCallback _uploadStringOperationCompleted;

		private SendOrPostCallback _uploadDataOperationCompleted;

		private SendOrPostCallback _uploadFileOperationCompleted;

		private SendOrPostCallback _uploadValuesOperationCompleted;

		private SendOrPostCallback _reportDownloadProgressChanged;

		private SendOrPostCallback _reportUploadProgressChanged;

		private static readonly char[] s_parseContentTypeSeparators = new char[] { ';', '=', ' ' };

		private static readonly Encoding[] s_knownEncodings = new Encoding[]
		{
			Encoding.UTF8,
			Encoding.UTF32,
			Encoding.Unicode,
			Encoding.BigEndianUnicode
		};

		private sealed class ProgressData
		{
			internal void Reset()
			{
				this.BytesSent = 0L;
				this.TotalBytesToSend = -1L;
				this.BytesReceived = 0L;
				this.TotalBytesToReceive = -1L;
				this.HasUploadPhase = false;
			}

			internal long BytesSent;

			internal long TotalBytesToSend = -1L;

			internal long BytesReceived;

			internal long TotalBytesToReceive = -1L;

			internal bool HasUploadPhase;
		}

		private sealed class WebClientWriteStream : DelegatingStream
		{
			public WebClientWriteStream(Stream stream, WebRequest request, WebClient webClient)
				: base(stream)
			{
				this._request = request;
				this._webClient = webClient;
			}

			protected override void Dispose(bool disposing)
			{
				try
				{
					if (disposing)
					{
						this._webClient.GetWebResponse(this._request).Dispose();
					}
				}
				finally
				{
					base.Dispose(disposing);
				}
			}

			private readonly WebRequest _request;

			private readonly WebClient _webClient;
		}
	}
}
