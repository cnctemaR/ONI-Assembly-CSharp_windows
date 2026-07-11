using System;
using System.IO;
using Unity;

namespace System.Net
{
	public class FtpWebResponse : WebResponse
	{
		internal FtpWebResponse(FtpWebRequest request, Uri uri, string method, bool keepAlive)
		{
			this.lastModified = DateTime.MinValue;
			this.bannerMessage = string.Empty;
			this.welcomeMessage = string.Empty;
			this.exitMessage = string.Empty;
			this.contentLength = -1L;
			base..ctor();
			this.request = request;
			this.uri = uri;
			this.method = method;
		}

		internal FtpWebResponse(FtpWebRequest request, Uri uri, string method, FtpStatusCode statusCode, string statusDescription)
		{
			this.lastModified = DateTime.MinValue;
			this.bannerMessage = string.Empty;
			this.welcomeMessage = string.Empty;
			this.exitMessage = string.Empty;
			this.contentLength = -1L;
			base..ctor();
			this.request = request;
			this.uri = uri;
			this.method = method;
			this.statusCode = statusCode;
			this.statusDescription = statusDescription;
		}

		internal FtpWebResponse(FtpWebRequest request, Uri uri, string method, FtpStatus status)
			: this(request, uri, method, status.StatusCode, status.StatusDescription)
		{
		}

		public override long ContentLength
		{
			get
			{
				return this.contentLength;
			}
		}

		public override WebHeaderCollection Headers
		{
			get
			{
				return new WebHeaderCollection();
			}
		}

		public override Uri ResponseUri
		{
			get
			{
				return this.uri;
			}
		}

		public DateTime LastModified
		{
			get
			{
				return this.lastModified;
			}
			internal set
			{
				this.lastModified = value;
			}
		}

		public string BannerMessage
		{
			get
			{
				return this.bannerMessage;
			}
			internal set
			{
				this.bannerMessage = value;
			}
		}

		public string WelcomeMessage
		{
			get
			{
				return this.welcomeMessage;
			}
			internal set
			{
				this.welcomeMessage = value;
			}
		}

		public string ExitMessage
		{
			get
			{
				return this.exitMessage;
			}
			internal set
			{
				this.exitMessage = value;
			}
		}

		public FtpStatusCode StatusCode
		{
			get
			{
				return this.statusCode;
			}
			internal set
			{
				this.statusCode = value;
			}
		}

		public override bool SupportsHeaders
		{
			get
			{
				return true;
			}
		}

		public string StatusDescription
		{
			get
			{
				return this.statusDescription;
			}
			internal set
			{
				this.statusDescription = value;
			}
		}

		public override void Close()
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			if (this.stream != null)
			{
				this.stream.Close();
				if (this.stream == Stream.Null)
				{
					this.request.OperationCompleted();
				}
			}
			this.stream = null;
		}

		public override Stream GetResponseStream()
		{
			if (this.stream == null)
			{
				return Stream.Null;
			}
			if (this.method != "RETR" && this.method != "NLST")
			{
				this.CheckDisposed();
			}
			return this.stream;
		}

		internal Stream Stream
		{
			get
			{
				return this.stream;
			}
			set
			{
				this.stream = value;
			}
		}

		internal void UpdateStatus(FtpStatus status)
		{
			this.statusCode = status.StatusCode;
			this.statusDescription = status.StatusDescription;
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		internal bool IsFinal()
		{
			return this.statusCode >= FtpStatusCode.CommandOK;
		}

		internal FtpWebResponse()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private Stream stream;

		private Uri uri;

		private FtpStatusCode statusCode;

		private DateTime lastModified;

		private string bannerMessage;

		private string welcomeMessage;

		private string exitMessage;

		private string statusDescription;

		private string method;

		private bool disposed;

		private FtpWebRequest request;

		internal long contentLength;
	}
}
