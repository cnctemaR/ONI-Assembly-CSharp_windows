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
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			string text = string.Empty;
			string[] array = value.Split(MimeBasePart.s_headerValueSplitChars, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(MimeBasePart.s_questionMarkSplitChars);
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
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			string[] array = value.Split(MimeBasePart.s_decodeEncodingSplitChars);
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
				if (this._headers == null)
				{
					this._headers = new HeaderCollection();
				}
				if (this._contentType == null)
				{
					this._contentType = new ContentType();
				}
				this._contentType.PersistIfNeeded(this._headers, false);
				if (this._contentDisposition != null)
				{
					this._contentDisposition.PersistIfNeeded(this._headers, false);
				}
				return this._headers;
			}
		}

		internal ContentType ContentType
		{
			get
			{
				ContentType contentType;
				if ((contentType = this._contentType) == null)
				{
					contentType = (this._contentType = new ContentType());
				}
				return contentType;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._contentType = value;
				this._contentType.PersistIfNeeded((HeaderCollection)this.Headers, true);
			}
		}

		internal void PrepareHeaders(bool allowUnicode)
		{
			this._contentType.PersistIfNeeded((HeaderCollection)this.Headers, false);
			this._headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentType), this._contentType.Encode(allowUnicode));
			if (this._contentDisposition != null)
			{
				this._contentDisposition.PersistIfNeeded((HeaderCollection)this.Headers, false);
				this._headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition), this._contentDisposition.Encode(allowUnicode));
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
				throw new ArgumentException("The IAsyncResult object was not returned from the corresponding asynchronous method on this class.", "asyncResult");
			}
			if (lazyAsyncResult.EndCalled)
			{
				throw new InvalidOperationException(SR.Format("{0} can only be called once for each asynchronous operation.", "EndSend"));
			}
			lazyAsyncResult.InternalWaitForCompletion();
			lazyAsyncResult.EndCalled = true;
			if (lazyAsyncResult.Result is Exception)
			{
				throw (Exception)lazyAsyncResult.Result;
			}
		}

		internal const string DefaultCharSet = "utf-8";

		private static readonly char[] s_decodeEncodingSplitChars = new char[] { '?', '\r', '\n' };

		protected ContentType _contentType;

		protected ContentDisposition _contentDisposition;

		private HeaderCollection _headers;

		private static readonly char[] s_headerValueSplitChars = new char[] { '\r', '\n', ' ' };

		private static readonly char[] s_questionMarkSplitChars = new char[] { '?' };

		internal class MimePartAsyncResult : LazyAsyncResult
		{
			internal MimePartAsyncResult(MimeBasePart part, object state, AsyncCallback callback)
				: base(part, state, callback)
			{
			}
		}
	}
}
