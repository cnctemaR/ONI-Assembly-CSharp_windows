using System;
using System.IO;
using System.Security.Principal;

namespace System.Net.Security
{
	public class NegotiateStream : AuthenticatedStream
	{
		[global::System.MonoTODO]
		public NegotiateStream(Stream innerStream)
			: base(innerStream, false)
		{
		}

		[global::System.MonoTODO]
		public NegotiateStream(Stream innerStream, bool leaveStreamOpen)
			: base(innerStream, leaveStreamOpen)
		{
		}

		public override bool CanRead
		{
			get
			{
				return base.InnerStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return base.InnerStream.CanSeek;
			}
		}

		[global::System.MonoTODO]
		public override bool CanTimeout
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool CanWrite
		{
			get
			{
				return base.InnerStream.CanWrite;
			}
		}

		[global::System.MonoTODO]
		public virtual TokenImpersonationLevel ImpersonationLevel
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[global::System.MonoTODO]
		public override bool IsAuthenticated
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[global::System.MonoTODO]
		public override bool IsEncrypted
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[global::System.MonoTODO]
		public override bool IsMutuallyAuthenticated
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[global::System.MonoTODO]
		public override bool IsServer
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[global::System.MonoTODO]
		public override bool IsSigned
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override long Length
		{
			get
			{
				return base.InnerStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return base.InnerStream.Position;
			}
			set
			{
				base.InnerStream.Position = value;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this.readTimeout;
			}
			set
			{
				this.readTimeout = value;
			}
		}

		[global::System.MonoTODO]
		public virtual IIdentity RemoteIdentity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return this.writeTimeout;
			}
			set
			{
				this.writeTimeout = value;
			}
		}

		[global::System.MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsClient(AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsClient(NetworkCredential credential, string targetName, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsClient(NetworkCredential credential, string targetName, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel allowedImpersonationLevel, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsServer(AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsServer(NetworkCredential credential, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel requiredImpersonationLevel, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public virtual void AuthenticateAsClient()
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public virtual void AuthenticateAsClient(NetworkCredential credential, string targetName)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public virtual void AuthenticateAsClient(NetworkCredential credential, string targetName, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel requiredImpersonationLevel)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public virtual void AuthenticateAsServer()
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public virtual void AuthenticateAsServer(NetworkCredential credential, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel requiredImpersonationLevel)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		[global::System.MonoTODO]
		public virtual void EndAuthenticateAsClient(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override int EndRead(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public virtual void EndAuthenticateAsServer(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override void Flush()
		{
			base.InnerStream.Flush();
		}

		[global::System.MonoTODO]
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		private int readTimeout;

		private int writeTimeout;
	}
}
