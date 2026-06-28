using System;
using System.IO;
using System.Runtime.Serialization;

namespace System.Net
{
	[Serializable]
	public class FileWebResponse : WebResponse, IDisposable, ISerializable
	{
		internal FileWebResponse(global::System.Uri responseUri, FileStream fileStream)
		{
			try
			{
				this.responseUri = responseUri;
				this.fileStream = fileStream;
				this.contentLength = fileStream.Length;
				this.webHeaders = new WebHeaderCollection();
				this.webHeaders.Add("Content-Length", Convert.ToString(this.contentLength));
				this.webHeaders.Add("Content-Type", "application/octet-stream");
			}
			catch (Exception ex)
			{
				throw new WebException(ex.Message, ex);
			}
		}

		[Obsolete("Serialization is obsoleted for this type", false)]
		protected FileWebResponse(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.responseUri = (global::System.Uri)serializationInfo.GetValue("responseUri", typeof(global::System.Uri));
			this.contentLength = serializationInfo.GetInt64("contentLength");
			this.webHeaders = (WebHeaderCollection)serializationInfo.GetValue("webHeaders", typeof(WebHeaderCollection));
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.GetObjectData(serializationInfo, streamingContext);
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override long ContentLength
		{
			get
			{
				this.CheckDisposed();
				return this.contentLength;
			}
		}

		public override string ContentType
		{
			get
			{
				this.CheckDisposed();
				return "application/octet-stream";
			}
		}

		public override WebHeaderCollection Headers
		{
			get
			{
				this.CheckDisposed();
				return this.webHeaders;
			}
		}

		public override global::System.Uri ResponseUri
		{
			get
			{
				this.CheckDisposed();
				return this.responseUri;
			}
		}

		protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			serializationInfo.AddValue("responseUri", this.responseUri, typeof(global::System.Uri));
			serializationInfo.AddValue("contentLength", this.contentLength);
			serializationInfo.AddValue("webHeaders", this.webHeaders, typeof(WebHeaderCollection));
		}

		public override Stream GetResponseStream()
		{
			this.CheckDisposed();
			return this.fileStream;
		}

		~FileWebResponse()
		{
			this.Dispose(false);
		}

		public override void Close()
		{
			((IDisposable)this).Dispose();
		}

		private void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			if (disposing)
			{
				this.responseUri = null;
				this.webHeaders = null;
			}
			FileStream fileStream = this.fileStream;
			this.fileStream = null;
			if (fileStream != null)
			{
				fileStream.Close();
			}
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		private global::System.Uri responseUri;

		private FileStream fileStream;

		private long contentLength;

		private WebHeaderCollection webHeaders;

		private bool disposed;
	}
}
