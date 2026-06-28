using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Net
{
	[ComVisible(true)]
	public class WebClient : global::System.ComponentModel.Component
	{
		static WebClient()
		{
			int num = 0;
			int i = 48;
			while (i <= 57)
			{
				WebClient.hexBytes[num] = (byte)i;
				i++;
				num++;
			}
			int j = 97;
			while (j <= 102)
			{
				WebClient.hexBytes[num] = (byte)j;
				j++;
				num++;
			}
		}

		public event DownloadDataCompletedEventHandler DownloadDataCompleted;

		public event global::System.ComponentModel.AsyncCompletedEventHandler DownloadFileCompleted;

		public event DownloadProgressChangedEventHandler DownloadProgressChanged;

		public event DownloadStringCompletedEventHandler DownloadStringCompleted;

		public event OpenReadCompletedEventHandler OpenReadCompleted;

		public event OpenWriteCompletedEventHandler OpenWriteCompleted;

		public event UploadDataCompletedEventHandler UploadDataCompleted;

		public event UploadFileCompletedEventHandler UploadFileCompleted;

		public event UploadProgressChangedEventHandler UploadProgressChanged;

		public event UploadStringCompletedEventHandler UploadStringCompleted;

		public event UploadValuesCompletedEventHandler UploadValuesCompleted;

		public string BaseAddress
		{
			get
			{
				if (this.baseString == null && this.baseAddress == null)
				{
					return string.Empty;
				}
				this.baseString = this.baseAddress.ToString();
				return this.baseString;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.baseAddress = null;
				}
				else
				{
					this.baseAddress = new global::System.Uri(value);
				}
			}
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		[global::System.MonoTODO]
		public global::System.Net.Cache.RequestCachePolicy CachePolicy
		{
			get
			{
				throw WebClient.GetMustImplement();
			}
			set
			{
				throw WebClient.GetMustImplement();
			}
		}

		[global::System.MonoTODO]
		public bool UseDefaultCredentials
		{
			get
			{
				throw WebClient.GetMustImplement();
			}
			set
			{
				throw WebClient.GetMustImplement();
			}
		}

		public ICredentials Credentials
		{
			get
			{
				return this.credentials;
			}
			set
			{
				this.credentials = value;
			}
		}

		public WebHeaderCollection Headers
		{
			get
			{
				if (this.headers == null)
				{
					this.headers = new WebHeaderCollection();
				}
				return this.headers;
			}
			set
			{
				this.headers = value;
			}
		}

		public global::System.Collections.Specialized.NameValueCollection QueryString
		{
			get
			{
				if (this.queryString == null)
				{
					this.queryString = new global::System.Collections.Specialized.NameValueCollection();
				}
				return this.queryString;
			}
			set
			{
				this.queryString = value;
			}
		}

		public WebHeaderCollection ResponseHeaders
		{
			get
			{
				return this.responseHeaders;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Encoding");
				}
				this.encoding = value;
			}
		}

		public IWebProxy Proxy
		{
			get
			{
				return this.proxy;
			}
			set
			{
				this.proxy = value;
			}
		}

		public bool IsBusy
		{
			get
			{
				return this.is_busy;
			}
		}

		private void CheckBusy()
		{
			if (this.IsBusy)
			{
				throw new NotSupportedException("WebClient does not support conccurent I/O operations.");
			}
		}

		private void SetBusy()
		{
			lock (this)
			{
				this.CheckBusy();
				this.is_busy = true;
			}
		}

		public byte[] DownloadData(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return this.DownloadData(this.CreateUri(address));
		}

		public byte[] DownloadData(global::System.Uri address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			byte[] array;
			try
			{
				this.SetBusy();
				this.async = false;
				array = this.DownloadDataCore(address, null);
			}
			finally
			{
				this.is_busy = false;
			}
			return array;
		}

		private byte[] DownloadDataCore(global::System.Uri address, object userToken)
		{
			WebRequest webRequest = null;
			byte[] array;
			try
			{
				webRequest = this.SetupRequest(address);
				WebResponse webResponse = this.GetWebResponse(webRequest);
				Stream responseStream = webResponse.GetResponseStream();
				array = this.ReadAll(responseStream, (int)webResponse.ContentLength, userToken);
			}
			catch (ThreadInterruptedException)
			{
				if (webRequest != null)
				{
					webRequest.Abort();
				}
				throw;
			}
			catch (WebException ex)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new WebException("An error occurred performing a WebClient request.", ex2);
			}
			return array;
		}

		public void DownloadFile(string address, string fileName)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			this.DownloadFile(this.CreateUri(address), fileName);
		}

		public void DownloadFile(global::System.Uri address, string fileName)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			try
			{
				this.SetBusy();
				this.async = false;
				this.DownloadFileCore(address, fileName, null);
			}
			catch (WebException ex)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new WebException("An error occurred performing a WebClient request.", ex2);
			}
			finally
			{
				this.is_busy = false;
			}
		}

		private void DownloadFileCore(global::System.Uri address, string fileName, object userToken)
		{
			WebRequest webRequest = null;
			using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
			{
				try
				{
					webRequest = this.SetupRequest(address);
					WebResponse webResponse = this.GetWebResponse(webRequest);
					Stream responseStream = webResponse.GetResponseStream();
					int num = (int)webResponse.ContentLength;
					int num2 = ((num > -1 && num <= 32768) ? num : 32768);
					byte[] array = new byte[num2];
					long num3 = 0L;
					int num4;
					while ((num4 = responseStream.Read(array, 0, num2)) != 0)
					{
						if (this.async)
						{
							num3 += (long)num4;
							this.OnDownloadProgressChanged(new DownloadProgressChangedEventArgs(num3, webResponse.ContentLength, userToken));
						}
						fileStream.Write(array, 0, num4);
					}
				}
				catch (ThreadInterruptedException)
				{
					if (webRequest != null)
					{
						webRequest.Abort();
					}
					throw;
				}
			}
		}

		public Stream OpenRead(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return this.OpenRead(this.CreateUri(address));
		}

		public Stream OpenRead(global::System.Uri address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			Stream responseStream;
			try
			{
				this.SetBusy();
				this.async = false;
				WebRequest webRequest = this.SetupRequest(address);
				WebResponse webResponse = this.GetWebResponse(webRequest);
				responseStream = webResponse.GetResponseStream();
			}
			catch (WebException ex)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new WebException("An error occurred performing a WebClient request.", ex2);
			}
			finally
			{
				this.is_busy = false;
			}
			return responseStream;
		}

		public Stream OpenWrite(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return this.OpenWrite(this.CreateUri(address));
		}

		public Stream OpenWrite(string address, string method)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return this.OpenWrite(this.CreateUri(address), method);
		}

		public Stream OpenWrite(global::System.Uri address)
		{
			return this.OpenWrite(address, null);
		}

		public Stream OpenWrite(global::System.Uri address, string method)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			Stream requestStream;
			try
			{
				this.SetBusy();
				this.async = false;
				WebRequest webRequest = this.SetupRequest(address, method, true);
				requestStream = webRequest.GetRequestStream();
			}
			catch (WebException ex)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new WebException("An error occurred performing a WebClient request.", ex2);
			}
			finally
			{
				this.is_busy = false;
			}
			return requestStream;
		}

		private string DetermineMethod(global::System.Uri address, string method, bool is_upload)
		{
			if (method != null)
			{
				return method;
			}
			if (address.Scheme == global::System.Uri.UriSchemeFtp)
			{
				return (!is_upload) ? "RETR" : "STOR";
			}
			return (!is_upload) ? "GET" : "POST";
		}

		public byte[] UploadData(string address, byte[] data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return this.UploadData(this.CreateUri(address), data);
		}

		public byte[] UploadData(string address, string method, byte[] data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return this.UploadData(this.CreateUri(address), method, data);
		}

		public byte[] UploadData(global::System.Uri address, byte[] data)
		{
			return this.UploadData(address, null, data);
		}

		public byte[] UploadData(global::System.Uri address, string method, byte[] data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] array;
			try
			{
				this.SetBusy();
				this.async = false;
				array = this.UploadDataCore(address, method, data, null);
			}
			catch (WebException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new WebException("An error occurred performing a WebClient request.", ex);
			}
			finally
			{
				this.is_busy = false;
			}
			return array;
		}

		private byte[] UploadDataCore(global::System.Uri address, string method, byte[] data, object userToken)
		{
			WebRequest webRequest = this.SetupRequest(address, method, true);
			byte[] array;
			try
			{
				int num = data.Length;
				webRequest.ContentLength = (long)num;
				using (Stream requestStream = webRequest.GetRequestStream())
				{
					requestStream.Write(data, 0, num);
				}
				WebResponse webResponse = this.GetWebResponse(webRequest);
				Stream responseStream = webResponse.GetResponseStream();
				array = this.ReadAll(responseStream, (int)webResponse.ContentLength, userToken);
			}
			catch (ThreadInterruptedException)
			{
				if (webRequest != null)
				{
					webRequest.Abort();
				}
				throw;
			}
			return array;
		}

		public byte[] UploadFile(string address, string fileName)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return this.UploadFile(this.CreateUri(address), fileName);
		}

		public byte[] UploadFile(global::System.Uri address, string fileName)
		{
			return this.UploadFile(address, null, fileName);
		}

		public byte[] UploadFile(string address, string method, string fileName)
		{
			return this.UploadFile(this.CreateUri(address), method, fileName);
		}

		public byte[] UploadFile(global::System.Uri address, string method, string fileName)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			byte[] array;
			try
			{
				this.SetBusy();
				this.async = false;
				array = this.UploadFileCore(address, method, fileName, null);
			}
			catch (WebException ex)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new WebException("An error occurred performing a WebClient request.", ex2);
			}
			finally
			{
				this.is_busy = false;
			}
			return array;
		}

		private byte[] UploadFileCore(global::System.Uri address, string method, string fileName, object userToken)
		{
			string text = this.Headers["Content-Type"];
			if (text != null)
			{
				string text2 = text.ToLower();
				if (text2.StartsWith("multipart/"))
				{
					throw new WebException("Content-Type cannot be set to a multipart type for this request.");
				}
			}
			else
			{
				text = "application/octet-stream";
			}
			string text3 = "------------" + DateTime.Now.Ticks.ToString("x");
			this.Headers["Content-Type"] = string.Format("multipart/form-data; boundary={0}", text3);
			Stream stream = null;
			Stream stream2 = null;
			byte[] array = null;
			fileName = Path.GetFullPath(fileName);
			WebRequest webRequest = null;
			try
			{
				stream2 = File.OpenRead(fileName);
				webRequest = this.SetupRequest(address, method, true);
				stream = webRequest.GetRequestStream();
				byte[] bytes = Encoding.ASCII.GetBytes("--" + text3 + "\r\n");
				stream.Write(bytes, 0, bytes.Length);
				string text4 = string.Format("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\nContent-Type: {1}\r\n\r\n", Path.GetFileName(fileName), text);
				byte[] bytes2 = Encoding.UTF8.GetBytes(text4);
				stream.Write(bytes2, 0, bytes2.Length);
				byte[] array2 = new byte[4096];
				int num;
				while ((num = stream2.Read(array2, 0, 4096)) != 0)
				{
					stream.Write(array2, 0, num);
				}
				stream.WriteByte(13);
				stream.WriteByte(10);
				stream.Write(bytes, 0, bytes.Length);
				stream.Close();
				stream = null;
				WebResponse webResponse = this.GetWebResponse(webRequest);
				Stream responseStream = webResponse.GetResponseStream();
				array = this.ReadAll(responseStream, (int)webResponse.ContentLength, userToken);
			}
			catch (ThreadInterruptedException)
			{
				if (webRequest != null)
				{
					webRequest.Abort();
				}
				throw;
			}
			finally
			{
				if (stream2 != null)
				{
					stream2.Close();
				}
				if (stream != null)
				{
					stream.Close();
				}
			}
			return array;
		}

		public byte[] UploadValues(string address, global::System.Collections.Specialized.NameValueCollection data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return this.UploadValues(this.CreateUri(address), data);
		}

		public byte[] UploadValues(string address, string method, global::System.Collections.Specialized.NameValueCollection data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return this.UploadValues(this.CreateUri(address), method, data);
		}

		public byte[] UploadValues(global::System.Uri address, global::System.Collections.Specialized.NameValueCollection data)
		{
			return this.UploadValues(address, null, data);
		}

		public byte[] UploadValues(global::System.Uri address, string method, global::System.Collections.Specialized.NameValueCollection data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] array;
			try
			{
				this.SetBusy();
				this.async = false;
				array = this.UploadValuesCore(address, method, data, null);
			}
			catch (WebException ex)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new WebException("An error occurred performing a WebClient request.", ex2);
			}
			finally
			{
				this.is_busy = false;
			}
			return array;
		}

		private byte[] UploadValuesCore(global::System.Uri uri, string method, global::System.Collections.Specialized.NameValueCollection data, object userToken)
		{
			string text = this.Headers["Content-Type"];
			if (text != null && string.Compare(text, WebClient.urlEncodedCType, true) != 0)
			{
				throw new WebException("Content-Type header cannot be changed from its default value for this request.");
			}
			this.Headers["Content-Type"] = WebClient.urlEncodedCType;
			WebRequest webRequest = this.SetupRequest(uri, method, true);
			byte[] array2;
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				foreach (object obj in data)
				{
					string text2 = (string)obj;
					byte[] array = Encoding.UTF8.GetBytes(text2);
					WebClient.UrlEncodeAndWrite(memoryStream, array);
					memoryStream.WriteByte(61);
					array = Encoding.UTF8.GetBytes(data[text2]);
					WebClient.UrlEncodeAndWrite(memoryStream, array);
					memoryStream.WriteByte(38);
				}
				int num = (int)memoryStream.Length;
				if (num > 0)
				{
					memoryStream.SetLength((long)(--num));
				}
				byte[] buffer = memoryStream.GetBuffer();
				webRequest.ContentLength = (long)num;
				using (Stream requestStream = webRequest.GetRequestStream())
				{
					requestStream.Write(buffer, 0, num);
				}
				memoryStream.Close();
				WebResponse webResponse = this.GetWebResponse(webRequest);
				Stream responseStream = webResponse.GetResponseStream();
				array2 = this.ReadAll(responseStream, (int)webResponse.ContentLength, userToken);
			}
			catch (ThreadInterruptedException)
			{
				webRequest.Abort();
				throw;
			}
			return array2;
		}

		public string DownloadString(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return this.encoding.GetString(this.DownloadData(this.CreateUri(address)));
		}

		public string DownloadString(global::System.Uri address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return this.encoding.GetString(this.DownloadData(this.CreateUri(address)));
		}

		public string UploadString(string address, string data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] array = this.UploadData(address, this.encoding.GetBytes(data));
			return this.encoding.GetString(array);
		}

		public string UploadString(string address, string method, string data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] array = this.UploadData(address, method, this.encoding.GetBytes(data));
			return this.encoding.GetString(array);
		}

		public string UploadString(global::System.Uri address, string data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] array = this.UploadData(address, this.encoding.GetBytes(data));
			return this.encoding.GetString(array);
		}

		public string UploadString(global::System.Uri address, string method, string data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] array = this.UploadData(address, method, this.encoding.GetBytes(data));
			return this.encoding.GetString(array);
		}

		private global::System.Uri CreateUri(string address)
		{
			return this.MakeUri(address);
		}

		private global::System.Uri CreateUri(global::System.Uri address)
		{
			string query = address.Query;
			if (string.IsNullOrEmpty(query))
			{
				query = this.GetQueryString(true);
			}
			if (this.baseAddress == null && query == null)
			{
				return address;
			}
			if (this.baseAddress == null)
			{
				return new global::System.Uri(address.ToString() + query, query != null);
			}
			if (query == null)
			{
				return new global::System.Uri(this.baseAddress, address.ToString());
			}
			return new global::System.Uri(this.baseAddress, address.ToString() + query, query != null);
		}

		private string GetQueryString(bool add_qmark)
		{
			if (this.queryString == null || this.queryString.Count == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (add_qmark)
			{
				stringBuilder.Append('?');
			}
			foreach (object obj in this.queryString)
			{
				string text = (string)obj;
				stringBuilder.AppendFormat("{0}={1}&", text, this.UrlEncode(this.queryString[text]));
			}
			if (stringBuilder.Length != 0)
			{
				stringBuilder.Length--;
			}
			if (stringBuilder.Length == 0)
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		private global::System.Uri MakeUri(string path)
		{
			string text = this.GetQueryString(true);
			if (this.baseAddress == null && text == null)
			{
				try
				{
					return new global::System.Uri(path);
				}
				catch (ArgumentNullException)
				{
					if (Environment.UnityWebSecurityEnabled)
					{
						throw;
					}
					path = Path.GetFullPath(path);
					return new global::System.Uri("file://" + path);
				}
				catch (global::System.UriFormatException)
				{
					if (Environment.UnityWebSecurityEnabled)
					{
						throw;
					}
					path = Path.GetFullPath(path);
					return new global::System.Uri("file://" + path);
				}
			}
			if (this.baseAddress == null)
			{
				return new global::System.Uri(path + text, text != null);
			}
			if (text == null)
			{
				return new global::System.Uri(this.baseAddress, path);
			}
			return new global::System.Uri(this.baseAddress, path + text, text != null);
		}

		private WebRequest SetupRequest(global::System.Uri uri)
		{
			WebRequest webRequest = this.GetWebRequest(uri);
			if (this.Proxy != null)
			{
				webRequest.Proxy = this.Proxy;
			}
			webRequest.Credentials = this.credentials;
			if (this.headers != null && this.headers.Count != 0 && webRequest is HttpWebRequest)
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
				string text = this.headers["Expect"];
				string text2 = this.headers["Content-Type"];
				string text3 = this.headers["Accept"];
				string text4 = this.headers["Connection"];
				string text5 = this.headers["User-Agent"];
				string text6 = this.headers["Referer"];
				this.headers.RemoveInternal("Expect");
				this.headers.RemoveInternal("Content-Type");
				this.headers.RemoveInternal("Accept");
				this.headers.RemoveInternal("Connection");
				this.headers.RemoveInternal("Referer");
				this.headers.RemoveInternal("User-Agent");
				webRequest.Headers = this.headers;
				if (text != null && text.Length > 0)
				{
					httpWebRequest.Expect = text;
				}
				if (text3 != null && text3.Length > 0)
				{
					httpWebRequest.Accept = text3;
				}
				if (text2 != null && text2.Length > 0)
				{
					httpWebRequest.ContentType = text2;
				}
				if (text4 != null && text4.Length > 0)
				{
					httpWebRequest.Connection = text4;
				}
				if (text5 != null && text5.Length > 0)
				{
					httpWebRequest.UserAgent = text5;
				}
				if (text6 != null && text6.Length > 0)
				{
					httpWebRequest.Referer = text6;
				}
			}
			this.responseHeaders = null;
			return webRequest;
		}

		private WebRequest SetupRequest(global::System.Uri uri, string method, bool is_upload)
		{
			WebRequest webRequest = this.SetupRequest(uri);
			webRequest.Method = this.DetermineMethod(uri, method, is_upload);
			return webRequest;
		}

		private byte[] ReadAll(Stream stream, int length, object userToken)
		{
			MemoryStream memoryStream = null;
			bool flag = length == -1;
			int num = ((!flag) ? length : 8192);
			if (flag)
			{
				memoryStream = new MemoryStream();
			}
			int num2 = 0;
			byte[] array = new byte[num];
			int num3;
			while ((num3 = stream.Read(array, num2, num)) != 0)
			{
				if (flag)
				{
					memoryStream.Write(array, 0, num3);
				}
				else
				{
					num2 += num3;
					num -= num3;
				}
				if (this.async)
				{
					this.OnDownloadProgressChanged(new DownloadProgressChangedEventArgs((long)num3, (long)length, userToken));
				}
			}
			if (flag)
			{
				return memoryStream.ToArray();
			}
			return array;
		}

		private string UrlEncode(string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int length = str.Length;
			for (int i = 0; i < length; i++)
			{
				char c = str[i];
				if (c == ' ')
				{
					stringBuilder.Append('+');
				}
				else if ((c < '0' && c != '-' && c != '.') || (c < 'A' && c > '9') || (c > 'Z' && c < 'a' && c != '_') || c > 'z')
				{
					stringBuilder.Append('%');
					int num = (int)(c >> 4);
					stringBuilder.Append((char)WebClient.hexBytes[num]);
					num = (int)(c & '\u000f');
					stringBuilder.Append((char)WebClient.hexBytes[num]);
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private static void UrlEncodeAndWrite(Stream stream, byte[] bytes)
		{
			if (bytes == null)
			{
				return;
			}
			int num = bytes.Length;
			if (num == 0)
			{
				return;
			}
			for (int i = 0; i < num; i++)
			{
				char c = (char)bytes[i];
				if (c == ' ')
				{
					stream.WriteByte(43);
				}
				else if ((c < '0' && c != '-' && c != '.') || (c < 'A' && c > '9') || (c > 'Z' && c < 'a' && c != '_') || c > 'z')
				{
					stream.WriteByte(37);
					int num2 = (int)(c >> 4);
					stream.WriteByte(WebClient.hexBytes[num2]);
					num2 = (int)(c & '\u000f');
					stream.WriteByte(WebClient.hexBytes[num2]);
				}
				else
				{
					stream.WriteByte((byte)c);
				}
			}
		}

		public void CancelAsync()
		{
			lock (this)
			{
				if (this.async_thread != null)
				{
					Thread thread = this.async_thread;
					this.CompleteAsync();
					thread.Interrupt();
				}
			}
		}

		private void CompleteAsync()
		{
			lock (this)
			{
				this.is_busy = false;
				this.async_thread = null;
			}
		}

		public void DownloadDataAsync(global::System.Uri address)
		{
			this.DownloadDataAsync(address, null);
		}

		public void DownloadDataAsync(global::System.Uri address, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			lock (this)
			{
				this.SetBusy();
				this.async = true;
				this.async_thread = new Thread(delegate(object state)
				{
					object[] array2 = (object[])state;
					try
					{
						byte[] array3 = this.DownloadDataCore((global::System.Uri)array2[0], array2[1]);
						this.OnDownloadDataCompleted(new DownloadDataCompletedEventArgs(array3, null, false, array2[1]));
					}
					catch (ThreadInterruptedException)
					{
						this.OnDownloadDataCompleted(new DownloadDataCompletedEventArgs(null, null, true, array2[1]));
						throw;
					}
					catch (Exception ex)
					{
						this.OnDownloadDataCompleted(new DownloadDataCompletedEventArgs(null, ex, false, array2[1]));
					}
				});
				object[] array = new object[] { address, userToken };
				this.async_thread.Start(array);
			}
		}

		public void DownloadFileAsync(global::System.Uri address, string fileName)
		{
			this.DownloadFileAsync(address, fileName, null);
		}

		public void DownloadFileAsync(global::System.Uri address, string fileName, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			lock (this)
			{
				this.SetBusy();
				this.async = true;
				this.async_thread = new Thread(delegate(object state)
				{
					object[] array2 = (object[])state;
					try
					{
						this.DownloadFileCore((global::System.Uri)array2[0], (string)array2[1], array2[2]);
						this.OnDownloadFileCompleted(new global::System.ComponentModel.AsyncCompletedEventArgs(null, false, array2[2]));
					}
					catch (ThreadInterruptedException)
					{
						this.OnDownloadFileCompleted(new global::System.ComponentModel.AsyncCompletedEventArgs(null, true, array2[2]));
					}
					catch (Exception ex)
					{
						this.OnDownloadFileCompleted(new global::System.ComponentModel.AsyncCompletedEventArgs(ex, false, array2[2]));
					}
				});
				object[] array = new object[] { address, fileName, userToken };
				this.async_thread.Start(array);
			}
		}

		public void DownloadStringAsync(global::System.Uri address)
		{
			this.DownloadStringAsync(address, null);
		}

		public void DownloadStringAsync(global::System.Uri address, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			lock (this)
			{
				this.SetBusy();
				this.async = true;
				this.async_thread = new Thread(delegate(object state)
				{
					object[] array2 = (object[])state;
					try
					{
						string @string = this.encoding.GetString(this.DownloadDataCore((global::System.Uri)array2[0], array2[1]));
						this.OnDownloadStringCompleted(new DownloadStringCompletedEventArgs(@string, null, false, array2[1]));
					}
					catch (ThreadInterruptedException)
					{
						this.OnDownloadStringCompleted(new DownloadStringCompletedEventArgs(null, null, true, array2[1]));
					}
					catch (Exception ex)
					{
						this.OnDownloadStringCompleted(new DownloadStringCompletedEventArgs(null, ex, false, array2[1]));
					}
				});
				object[] array = new object[] { address, userToken };
				this.async_thread.Start(array);
			}
		}

		public void OpenReadAsync(global::System.Uri address)
		{
			this.OpenReadAsync(address, null);
		}

		public void OpenReadAsync(global::System.Uri address, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			lock (this)
			{
				this.SetBusy();
				this.async = true;
				this.async_thread = new Thread(delegate(object state)
				{
					object[] array2 = (object[])state;
					WebRequest webRequest = null;
					try
					{
						webRequest = this.SetupRequest((global::System.Uri)array2[0]);
						WebResponse webResponse = this.GetWebResponse(webRequest);
						Stream responseStream = webResponse.GetResponseStream();
						this.OnOpenReadCompleted(new OpenReadCompletedEventArgs(responseStream, null, false, array2[1]));
					}
					catch (ThreadInterruptedException)
					{
						if (webRequest != null)
						{
							webRequest.Abort();
						}
						this.OnOpenReadCompleted(new OpenReadCompletedEventArgs(null, null, true, array2[1]));
					}
					catch (Exception ex)
					{
						this.OnOpenReadCompleted(new OpenReadCompletedEventArgs(null, ex, false, array2[1]));
					}
				});
				object[] array = new object[] { address, userToken };
				this.async_thread.Start(array);
			}
		}

		public void OpenWriteAsync(global::System.Uri address)
		{
			this.OpenWriteAsync(address, null);
		}

		public void OpenWriteAsync(global::System.Uri address, string method)
		{
			this.OpenWriteAsync(address, method, null);
		}

		public void OpenWriteAsync(global::System.Uri address, string method, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			lock (this)
			{
				this.SetBusy();
				this.async = true;
				this.async_thread = new Thread(delegate(object state)
				{
					object[] array2 = (object[])state;
					WebRequest webRequest = null;
					try
					{
						webRequest = this.SetupRequest((global::System.Uri)array2[0], (string)array2[1], true);
						Stream requestStream = webRequest.GetRequestStream();
						this.OnOpenWriteCompleted(new OpenWriteCompletedEventArgs(requestStream, null, false, array2[2]));
					}
					catch (ThreadInterruptedException)
					{
						if (webRequest != null)
						{
							webRequest.Abort();
						}
						this.OnOpenWriteCompleted(new OpenWriteCompletedEventArgs(null, null, true, array2[2]));
					}
					catch (Exception ex)
					{
						this.OnOpenWriteCompleted(new OpenWriteCompletedEventArgs(null, ex, false, array2[2]));
					}
				});
				object[] array = new object[] { address, method, userToken };
				this.async_thread.Start(array);
			}
		}

		public void UploadDataAsync(global::System.Uri address, byte[] data)
		{
			this.UploadDataAsync(address, null, data);
		}

		public void UploadDataAsync(global::System.Uri address, string method, byte[] data)
		{
			this.UploadDataAsync(address, method, data, null);
		}

		public void UploadDataAsync(global::System.Uri address, string method, byte[] data, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			lock (this)
			{
				this.SetBusy();
				this.async = true;
				this.async_thread = new Thread(delegate(object state)
				{
					object[] array2 = (object[])state;
					try
					{
						byte[] array3 = this.UploadDataCore((global::System.Uri)array2[0], (string)array2[1], (byte[])array2[2], array2[3]);
						this.OnUploadDataCompleted(new UploadDataCompletedEventArgs(array3, null, false, array2[3]));
					}
					catch (ThreadInterruptedException)
					{
						this.OnUploadDataCompleted(new UploadDataCompletedEventArgs(null, null, true, array2[3]));
					}
					catch (Exception ex)
					{
						this.OnUploadDataCompleted(new UploadDataCompletedEventArgs(null, ex, false, array2[3]));
					}
				});
				object[] array = new object[] { address, method, data, userToken };
				this.async_thread.Start(array);
			}
		}

		public void UploadFileAsync(global::System.Uri address, string fileName)
		{
			this.UploadFileAsync(address, null, fileName);
		}

		public void UploadFileAsync(global::System.Uri address, string method, string fileName)
		{
			this.UploadFileAsync(address, method, fileName, null);
		}

		public void UploadFileAsync(global::System.Uri address, string method, string fileName, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			lock (this)
			{
				this.SetBusy();
				this.async = true;
				this.async_thread = new Thread(delegate(object state)
				{
					object[] array2 = (object[])state;
					try
					{
						byte[] array3 = this.UploadFileCore((global::System.Uri)array2[0], (string)array2[1], (string)array2[2], array2[3]);
						this.OnUploadFileCompleted(new UploadFileCompletedEventArgs(array3, null, false, array2[3]));
					}
					catch (ThreadInterruptedException)
					{
						this.OnUploadFileCompleted(new UploadFileCompletedEventArgs(null, null, true, array2[3]));
					}
					catch (Exception ex)
					{
						this.OnUploadFileCompleted(new UploadFileCompletedEventArgs(null, ex, false, array2[3]));
					}
				});
				object[] array = new object[] { address, method, fileName, userToken };
				this.async_thread.Start(array);
			}
		}

		public void UploadStringAsync(global::System.Uri address, string data)
		{
			this.UploadStringAsync(address, null, data);
		}

		public void UploadStringAsync(global::System.Uri address, string method, string data)
		{
			this.UploadStringAsync(address, method, data, null);
		}

		public void UploadStringAsync(global::System.Uri address, string method, string data, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			lock (this)
			{
				this.CheckBusy();
				this.async = true;
				this.async_thread = new Thread(delegate(object state)
				{
					object[] array2 = (object[])state;
					try
					{
						string text = this.UploadString((global::System.Uri)array2[0], (string)array2[1], (string)array2[2]);
						this.OnUploadStringCompleted(new UploadStringCompletedEventArgs(text, null, false, array2[3]));
					}
					catch (ThreadInterruptedException)
					{
						this.OnUploadStringCompleted(new UploadStringCompletedEventArgs(null, null, true, array2[3]));
					}
					catch (Exception ex)
					{
						this.OnUploadStringCompleted(new UploadStringCompletedEventArgs(null, ex, false, array2[3]));
					}
				});
				object[] array = new object[] { address, method, data, userToken };
				this.async_thread.Start(array);
			}
		}

		public void UploadValuesAsync(global::System.Uri address, global::System.Collections.Specialized.NameValueCollection values)
		{
			this.UploadValuesAsync(address, null, values);
		}

		public void UploadValuesAsync(global::System.Uri address, string method, global::System.Collections.Specialized.NameValueCollection values)
		{
			this.UploadValuesAsync(address, method, values, null);
		}

		public void UploadValuesAsync(global::System.Uri address, string method, global::System.Collections.Specialized.NameValueCollection values, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			lock (this)
			{
				this.CheckBusy();
				this.async = true;
				this.async_thread = new Thread(delegate(object state)
				{
					object[] array2 = (object[])state;
					try
					{
						byte[] array3 = this.UploadValuesCore((global::System.Uri)array2[0], (string)array2[1], (global::System.Collections.Specialized.NameValueCollection)array2[2], array2[3]);
						this.OnUploadValuesCompleted(new UploadValuesCompletedEventArgs(array3, null, false, array2[3]));
					}
					catch (ThreadInterruptedException)
					{
						this.OnUploadValuesCompleted(new UploadValuesCompletedEventArgs(null, null, true, array2[3]));
					}
					catch (Exception ex)
					{
						this.OnUploadValuesCompleted(new UploadValuesCompletedEventArgs(null, ex, false, array2[3]));
					}
				});
				object[] array = new object[] { address, method, values, userToken };
				this.async_thread.Start(array);
			}
		}

		protected virtual void OnDownloadDataCompleted(DownloadDataCompletedEventArgs args)
		{
			this.CompleteAsync();
			if (this.DownloadDataCompleted != null)
			{
				this.DownloadDataCompleted(this, args);
			}
		}

		protected virtual void OnDownloadFileCompleted(global::System.ComponentModel.AsyncCompletedEventArgs args)
		{
			this.CompleteAsync();
			if (this.DownloadFileCompleted != null)
			{
				this.DownloadFileCompleted(this, args);
			}
		}

		protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
		{
			if (this.DownloadProgressChanged != null)
			{
				this.DownloadProgressChanged(this, e);
			}
		}

		protected virtual void OnDownloadStringCompleted(DownloadStringCompletedEventArgs args)
		{
			this.CompleteAsync();
			if (this.DownloadStringCompleted != null)
			{
				this.DownloadStringCompleted(this, args);
			}
		}

		protected virtual void OnOpenReadCompleted(OpenReadCompletedEventArgs args)
		{
			this.CompleteAsync();
			if (this.OpenReadCompleted != null)
			{
				this.OpenReadCompleted(this, args);
			}
		}

		protected virtual void OnOpenWriteCompleted(OpenWriteCompletedEventArgs args)
		{
			this.CompleteAsync();
			if (this.OpenWriteCompleted != null)
			{
				this.OpenWriteCompleted(this, args);
			}
		}

		protected virtual void OnUploadDataCompleted(UploadDataCompletedEventArgs args)
		{
			this.CompleteAsync();
			if (this.UploadDataCompleted != null)
			{
				this.UploadDataCompleted(this, args);
			}
		}

		protected virtual void OnUploadFileCompleted(UploadFileCompletedEventArgs args)
		{
			this.CompleteAsync();
			if (this.UploadFileCompleted != null)
			{
				this.UploadFileCompleted(this, args);
			}
		}

		protected virtual void OnUploadProgressChanged(UploadProgressChangedEventArgs e)
		{
			if (this.UploadProgressChanged != null)
			{
				this.UploadProgressChanged(this, e);
			}
		}

		protected virtual void OnUploadStringCompleted(UploadStringCompletedEventArgs args)
		{
			this.CompleteAsync();
			if (this.UploadStringCompleted != null)
			{
				this.UploadStringCompleted(this, args);
			}
		}

		protected virtual void OnUploadValuesCompleted(UploadValuesCompletedEventArgs args)
		{
			this.CompleteAsync();
			if (this.UploadValuesCompleted != null)
			{
				this.UploadValuesCompleted(this, args);
			}
		}

		protected virtual WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
		{
			WebResponse webResponse = request.EndGetResponse(result);
			this.responseHeaders = webResponse.Headers;
			return webResponse;
		}

		protected virtual WebRequest GetWebRequest(global::System.Uri address)
		{
			return WebRequest.Create(address);
		}

		protected virtual WebResponse GetWebResponse(WebRequest request)
		{
			WebResponse response = request.GetResponse();
			this.responseHeaders = response.Headers;
			return response;
		}

		private static readonly string urlEncodedCType = "application/x-www-form-urlencoded";

		private static byte[] hexBytes = new byte[16];

		private ICredentials credentials;

		private WebHeaderCollection headers;

		private WebHeaderCollection responseHeaders;

		private global::System.Uri baseAddress;

		private string baseString;

		private global::System.Collections.Specialized.NameValueCollection queryString;

		private bool is_busy;

		private bool async;

		private Thread async_thread;

		private Encoding encoding = Encoding.Default;

		private IWebProxy proxy;
	}
}
