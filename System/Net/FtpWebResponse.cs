using System;
using System.IO;
using Unity;

namespace System.Net
{
	public class FtpWebResponse : WebResponse, IDisposable
	{
		internal FtpWebResponse(Stream responseStream, long contentLength, Uri responseUri, FtpStatusCode statusCode, string statusLine, DateTime lastModified, string bannerMessage, string welcomeMessage, string exitMessage)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, contentLength, statusLine);
			}
			this._responseStream = responseStream;
			if (responseStream == null && contentLength < 0L)
			{
				contentLength = 0L;
			}
			this._contentLength = contentLength;
			this._responseUri = responseUri;
			this._statusCode = statusCode;
			this._statusLine = statusLine;
			this._lastModified = lastModified;
			this._bannerMessage = bannerMessage;
			this._welcomeMessage = welcomeMessage;
			this._exitMessage = exitMessage;
		}

		internal void UpdateStatus(FtpStatusCode statusCode, string statusLine, string exitMessage)
		{
			this._statusCode = statusCode;
			this._statusLine = statusLine;
			this._exitMessage = exitMessage;
		}

		public override Stream GetResponseStream()
		{
			Stream stream;
			if (this._responseStream != null)
			{
				stream = this._responseStream;
			}
			else
			{
				stream = (this._responseStream = new FtpWebResponse.EmptyStream());
			}
			return stream;
		}

		internal void SetResponseStream(Stream stream)
		{
			if (stream == null || stream == Stream.Null || stream is FtpWebResponse.EmptyStream)
			{
				return;
			}
			this._responseStream = stream;
		}

		public override void Close()
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, null, "Close");
			}
			Stream responseStream = this._responseStream;
			if (responseStream != null)
			{
				responseStream.Close();
			}
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Exit(this, null, "Close");
			}
		}

		public override long ContentLength
		{
			get
			{
				return this._contentLength;
			}
		}

		public override WebHeaderCollection Headers
		{
			get
			{
				if (this._ftpRequestHeaders == null)
				{
					lock (this)
					{
						if (this._ftpRequestHeaders == null)
						{
							this._ftpRequestHeaders = new WebHeaderCollection();
						}
					}
				}
				return this._ftpRequestHeaders;
			}
		}

		public override bool SupportsHeaders
		{
			get
			{
				return true;
			}
		}

		public override Uri ResponseUri
		{
			get
			{
				return this._responseUri;
			}
		}

		public FtpStatusCode StatusCode
		{
			get
			{
				return this._statusCode;
			}
		}

		public string StatusDescription
		{
			get
			{
				return this._statusLine;
			}
		}

		public DateTime LastModified
		{
			get
			{
				return this._lastModified;
			}
		}

		public string BannerMessage
		{
			get
			{
				return this._bannerMessage;
			}
		}

		public string WelcomeMessage
		{
			get
			{
				return this._welcomeMessage;
			}
		}

		public string ExitMessage
		{
			get
			{
				return this._exitMessage;
			}
		}

		internal FtpWebResponse()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		internal Stream _responseStream;

		private long _contentLength;

		private Uri _responseUri;

		private FtpStatusCode _statusCode;

		private string _statusLine;

		private WebHeaderCollection _ftpRequestHeaders;

		private DateTime _lastModified;

		private string _bannerMessage;

		private string _welcomeMessage;

		private string _exitMessage;

		internal sealed class EmptyStream : MemoryStream
		{
			internal EmptyStream()
				: base(Array.Empty<byte>(), false)
			{
			}
		}
	}
}
