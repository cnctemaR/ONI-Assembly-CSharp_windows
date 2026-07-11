using System;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Text;

namespace System.Net.Mime
{
	internal class MimeBasePart
	{
		internal MimeBasePart()
		{
		}

		internal static bool ShouldUseBase64Encoding(Encoding encoding)
		{
			return encoding == Encoding.Unicode || encoding == Encoding.UTF8 || encoding == Encoding.UTF32 || encoding == Encoding.BigEndianUnicode;
		}

		internal static string EncodeHeaderValue(string value, Encoding encoding, bool base64Encoding)
		{
			return MimeBasePart.EncodeHeaderValue(value, encoding, base64Encoding, 0);
		}

		internal static string EncodeHeaderValue(string value, Encoding encoding, bool base64Encoding, int headerLength)
		{
			if (MimeBasePart.IsAscii(value, false))
			{
				return value;
			}
			if (encoding == null)
			{
				encoding = Encoding.GetEncoding("utf-8");
			}
			IEncodableStream encoderForHeader = new EncodedStreamFactory().GetEncoderForHeader(encoding, base64Encoding, headerLength);
			byte[] bytes = encoding.GetBytes(value);
			encoderForHeader.EncodeBytes(bytes, 0, bytes.Length);
			return encoderForHeader.GetEncodedString();
		}

		internal static string DecodeHeaderValue(string value)
		{
			if (value == null || value.Length == 0)
			{
				return string.Empty;
			}
			string text = string.Empty;
			string[] array = value.Split(new char[] { '\r', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[] { '?' });
				if (array2.Length != 5 || array2[0] != "=" || array2[4] != "=")
				{
					return value;
				}
				string text2 = array2[1];
				bool flag = array2[2] == "B";
				byte[] bytes = Encoding.ASCII.GetBytes(array2[3]);
				int num = new EncodedStreamFactory().GetEncoderForHeader(Encoding.GetEncoding(text2), flag, 0).DecodeBytes(bytes, 0, bytes.Length);
				Encoding encoding = Encoding.GetEncoding(text2);
				text += encoding.GetString(bytes, 0, num);
			}
			return text;
		}

		internal static Encoding DecodeEncoding(string value)
		{
			if (value == null || value.Length == 0)
			{
				return null;
			}
			string[] array = value.Split(new char[] { '?', '\r', '\n' });
			if (array.Length < 5 || array[0] != "=" || array[4] != "=")
			{
				return null;
			}
			return Encoding.GetEncoding(array[1]);
		}

		internal static bool IsAscii(string value, bool permitCROrLF)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			foreach (char c in value)
			{
				if (c > '\u007f')
				{
					return false;
				}
				if (!permitCROrLF && (c == '\r' || c == '\n'))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsAnsi(string value, bool permitCROrLF)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			foreach (char c in value)
			{
				if (c > 'ÿ')
				{
					return false;
				}
				if (!permitCROrLF && (c == '\r' || c == '\n'))
				{
					return false;
				}
			}
			return true;
		}

		internal string ContentID
		{
			get
			{
				return this.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentID)];
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.ContentID));
					return;
				}
				this.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentID)] = value;
			}
		}

		internal string ContentLocation
		{
			get
			{
				return this.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentLocation)];
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.ContentLocation));
					return;
				}
				this.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentLocation)] = value;
			}
		}

		internal NameValueCollection Headers
		{
			get
			{
				if (this.headers == null)
				{
					this.headers = new HeaderCollection();
				}
				if (this.contentType == null)
				{
					this.contentType = new ContentType();
				}
				this.contentType.PersistIfNeeded(this.headers, false);
				if (this.contentDisposition != null)
				{
					this.contentDisposition.PersistIfNeeded(this.headers, false);
				}
				return this.headers;
			}
		}

		internal ContentType ContentType
		{
			get
			{
				if (this.contentType == null)
				{
					this.contentType = new ContentType();
				}
				return this.contentType;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.contentType = value;
				this.contentType.PersistIfNeeded((HeaderCollection)this.Headers, true);
			}
		}

		internal void PrepareHeaders(bool allowUnicode)
		{
			this.contentType.PersistIfNeeded((HeaderCollection)this.Headers, false);
			this.headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentType), this.contentType.Encode(allowUnicode));
			if (this.contentDisposition != null)
			{
				this.contentDisposition.PersistIfNeeded((HeaderCollection)this.Headers, false);
				this.headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition), this.contentDisposition.Encode(allowUnicode));
			}
		}

		internal virtual void Send(BaseWriter writer, bool allowUnicode)
		{
			throw new NotImplementedException();
		}

		internal virtual IAsyncResult BeginSend(BaseWriter writer, AsyncCallback callback, bool allowUnicode, object state)
		{
			throw new NotImplementedException();
		}

		internal void EndSend(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			LazyAsyncResult lazyAsyncResult = asyncResult as MimeBasePart.MimePartAsyncResult;
			if (lazyAsyncResult == null || lazyAsyncResult.AsyncObject != this)
			{
				throw new ArgumentException(global::SR.GetString("The IAsyncResult object was not returned from the corresponding asynchronous method on this class."), "asyncResult");
			}
			if (lazyAsyncResult.EndCalled)
			{
				throw new InvalidOperationException(global::SR.GetString("{0} can only be called once for each asynchronous operation.", new object[] { "EndSend" }));
			}
			lazyAsyncResult.InternalWaitForCompletion();
			lazyAsyncResult.EndCalled = true;
			if (lazyAsyncResult.Result is Exception)
			{
				throw (Exception)lazyAsyncResult.Result;
			}
		}

		protected ContentType contentType;

		protected ContentDisposition contentDisposition;

		private HeaderCollection headers;

		internal const string defaultCharSet = "utf-8";

		internal class MimePartAsyncResult : LazyAsyncResult
		{
			internal MimePartAsyncResult(MimeBasePart part, object state, AsyncCallback callback)
				: base(part, state, callback)
			{
			}
		}
	}
}
