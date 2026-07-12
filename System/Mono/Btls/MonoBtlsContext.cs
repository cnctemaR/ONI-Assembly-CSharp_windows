using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;
using Mono.Net.Security;
using Mono.Security.Interface;

namespace Mono.Btls
{
	internal class MonoBtlsContext : MobileTlsContext, IMonoBtlsBioMono
	{
		public MonoBtlsContext(MobileAuthenticatedStream parent, MonoSslAuthenticationOptions options)
			: base(parent, options)
		{
			if (base.IsServer && base.LocalServerCertificate != null)
			{
				this.nativeServerCertificate = MonoBtlsContext.GetPrivateCertificate(base.LocalServerCertificate);
			}
		}

		private static X509CertificateImplBtls GetPrivateCertificate(X509Certificate certificate)
		{
			X509CertificateImplBtls x509CertificateImplBtls = certificate.Impl as X509CertificateImplBtls;
			if (x509CertificateImplBtls != null)
			{
				return (X509CertificateImplBtls)x509CertificateImplBtls.Clone();
			}
			string text = Guid.NewGuid().ToString();
			X509CertificateImplBtls x509CertificateImplBtls2;
			using (SafePasswordHandle safePasswordHandle = new SafePasswordHandle(text))
			{
				x509CertificateImplBtls2 = new X509CertificateImplBtls(certificate.Export(X509ContentType.Pfx, text), safePasswordHandle, X509KeyStorageFlags.DefaultKeySet);
			}
			return x509CertificateImplBtls2;
		}

		public new MonoBtlsProvider Provider
		{
			get
			{
				return (MonoBtlsProvider)base.Provider;
			}
		}

		private int VerifyCallback(MonoBtlsX509StoreCtx storeCtx)
		{
			int num;
			using (X509ChainImplBtls x509ChainImplBtls = new X509ChainImplBtls(storeCtx))
			{
				using (X509Chain x509Chain = new X509Chain(x509ChainImplBtls))
				{
					X509Certificate2 certificate = x509Chain.ChainElements[0].Certificate;
					bool flag = base.ValidateCertificate(certificate, x509Chain);
					this.certificateValidated = true;
					num = (flag ? 1 : 0);
				}
			}
			return num;
		}

		private int SelectCallback(string[] acceptableIssuers)
		{
			if (this.nativeClientCertificate != null)
			{
				return 1;
			}
			this.GetPeerCertificate();
			X509Certificate x509Certificate = base.SelectClientCertificate(acceptableIssuers);
			if (x509Certificate == null)
			{
				return 1;
			}
			this.nativeClientCertificate = MonoBtlsContext.GetPrivateCertificate(x509Certificate);
			this.clientCertificate = new X509Certificate(this.nativeClientCertificate);
			this.SetPrivateCertificate(this.nativeClientCertificate);
			return 1;
		}

		private int ServerNameCallback()
		{
			string serverName = this.ssl.GetServerName();
			X509Certificate x509Certificate = base.SelectServerCertificate(serverName);
			if (x509Certificate == null)
			{
				return 1;
			}
			this.nativeServerCertificate = MonoBtlsContext.GetPrivateCertificate(x509Certificate);
			this.SetPrivateCertificate(this.nativeServerCertificate);
			return 1;
		}

		public override void StartHandshake()
		{
			this.InitializeConnection();
			this.ssl = new MonoBtlsSsl(this.ctx);
			this.bio = new MonoBtlsBioMono(this);
			this.ssl.SetBio(this.bio);
			if (base.IsServer)
			{
				if (this.nativeServerCertificate != null)
				{
					this.SetPrivateCertificate(this.nativeServerCertificate);
				}
			}
			else
			{
				this.ssl.SetServerName(base.ServerName);
			}
			if (base.Options.AllowRenegotiation)
			{
				this.ssl.SetRenegotiateMode(MonoBtlsSslRenegotiateMode.FREELY);
			}
		}

		private void SetPrivateCertificate(X509CertificateImplBtls privateCert)
		{
			this.ssl.SetCertificate(privateCert.X509);
			this.ssl.SetPrivateKey(privateCert.NativePrivateKey);
			X509CertificateImplCollection intermediateCertificates = privateCert.IntermediateCertificates;
			if (intermediateCertificates == null)
			{
				X509Chain x509Chain = new X509Chain(false);
				x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
				x509Chain.Build(new X509Certificate2(privateCert.X509.GetRawData(MonoBtlsX509Format.DER), ""));
				X509ChainElementCollection chainElements = x509Chain.ChainElements;
				for (int i = 1; i < chainElements.Count; i++)
				{
					X509Certificate2 certificate = chainElements[i].Certificate;
					if (certificate.SubjectName.RawData.SequenceEqual<byte>(certificate.IssuerName.RawData))
					{
						return;
					}
					this.ssl.AddIntermediateCertificate(MonoBtlsX509.LoadFromData(certificate.RawData, MonoBtlsX509Format.DER));
				}
				return;
			}
			for (int j = 0; j < intermediateCertificates.Count; j++)
			{
				X509CertificateImplBtls x509CertificateImplBtls = (X509CertificateImplBtls)intermediateCertificates[j];
				this.ssl.AddIntermediateCertificate(x509CertificateImplBtls.X509);
			}
		}

		private static Exception GetException(MonoBtlsSslError status)
		{
			string text;
			int num;
			int error = MonoBtlsError.GetError(out text, out num);
			if (error == 0)
			{
				return new MonoBtlsException(status);
			}
			int errorReason = MonoBtlsError.GetErrorReason(error);
			if (errorReason > 0)
			{
				return new TlsException((AlertDescription)errorReason);
			}
			string errorString = MonoBtlsError.GetErrorString(error);
			string text2;
			if (text != null)
			{
				text2 = string.Format("{0} {1}\n  at {2}:{3}", new object[] { status, errorString, text, num });
			}
			else
			{
				text2 = string.Format("{0} {1}", status, errorString);
			}
			return new MonoBtlsException(text2);
		}

		public override bool ProcessHandshake()
		{
			bool flag = false;
			while (!flag)
			{
				MonoBtlsError.ClearError();
				MonoBtlsSslError monoBtlsSslError = this.DoProcessHandshake();
				if (monoBtlsSslError != MonoBtlsSslError.None)
				{
					if (monoBtlsSslError - MonoBtlsSslError.WantRead > 1)
					{
						this.ctx.CheckLastError("ProcessHandshake");
						throw MonoBtlsContext.GetException(monoBtlsSslError);
					}
					return false;
				}
				else if (this.connected)
				{
					flag = true;
				}
				else
				{
					this.connected = true;
				}
			}
			this.ssl.PrintErrors();
			return true;
		}

		private MonoBtlsSslError DoProcessHandshake()
		{
			if (this.connected)
			{
				return this.ssl.Handshake();
			}
			if (base.IsServer)
			{
				return this.ssl.Accept();
			}
			return this.ssl.Connect();
		}

		public override void FinishHandshake()
		{
			this.InitializeSession();
			this.isAuthenticated = true;
		}

		private void InitializeConnection()
		{
			this.ctx = new MonoBtlsSslCtx();
			MonoBtlsProvider.SetupCertificateStore(this.ctx.CertificateStore, base.Settings, base.IsServer);
			if (!base.IsServer || base.AskForClientCertificate)
			{
				this.ctx.SetVerifyCallback(new MonoBtlsVerifyCallback(this.VerifyCallback), false);
			}
			if (!base.IsServer)
			{
				this.ctx.SetSelectCallback(new MonoBtlsSelectCallback(this.SelectCallback));
			}
			if (base.IsServer && (base.Options.ServerCertSelectionDelegate != null || base.Settings.ClientCertificateSelectionCallback != null))
			{
				this.ctx.SetServerNameCallback(new MonoBtlsServerNameCallback(this.ServerNameCallback));
			}
			this.ctx.SetVerifyParam(MonoBtlsProvider.GetVerifyParam(base.Settings, base.ServerName, base.IsServer));
			TlsProtocolCode? tlsProtocolCode;
			TlsProtocolCode? tlsProtocolCode2;
			base.GetProtocolVersions(out tlsProtocolCode, out tlsProtocolCode2);
			if (tlsProtocolCode != null)
			{
				this.ctx.SetMinVersion((int)tlsProtocolCode.Value);
			}
			if (tlsProtocolCode2 != null)
			{
				this.ctx.SetMaxVersion((int)tlsProtocolCode2.Value);
			}
			if (base.Settings != null && base.Settings.EnabledCiphers != null)
			{
				short[] array = new short[base.Settings.EnabledCiphers.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (short)base.Settings.EnabledCiphers[i];
				}
				this.ctx.SetCiphers(array, true);
			}
			if (base.IsServer)
			{
				MonoTlsSettings settings = base.Settings;
				if (((settings != null) ? settings.ClientCertificateIssuers : null) != null)
				{
					this.ctx.SetClientCertificateIssuers(base.Settings.ClientCertificateIssuers);
				}
			}
		}

		private void GetPeerCertificate()
		{
			if (this.remoteCertificate != null)
			{
				return;
			}
			using (MonoBtlsX509 peerCertificate = this.ssl.GetPeerCertificate())
			{
				if (peerCertificate != null)
				{
					this.remoteCertificate = MonoBtlsProvider.CreateCertificate(peerCertificate);
				}
			}
		}

		private void InitializeSession()
		{
			this.GetPeerCertificate();
			if (base.IsServer && base.AskForClientCertificate && !this.certificateValidated && !base.ValidateCertificate(null, null))
			{
				throw new TlsException(AlertDescription.CertificateUnknown);
			}
			CipherSuiteCode cipherSuiteCode = (CipherSuiteCode)this.ssl.GetCipher();
			TlsProtocolCode tlsProtocolCode = (TlsProtocolCode)this.ssl.GetVersion();
			string serverName = this.ssl.GetServerName();
			this.connectionInfo = new MonoTlsConnectionInfo
			{
				CipherSuiteCode = cipherSuiteCode,
				ProtocolVersion = MonoBtlsContext.GetProtocol(tlsProtocolCode),
				PeerDomainName = serverName
			};
		}

		private static TlsProtocols GetProtocol(TlsProtocolCode protocol)
		{
			switch (protocol)
			{
			case TlsProtocolCode.Tls10:
				return TlsProtocols.Tls10;
			case TlsProtocolCode.Tls11:
				return TlsProtocols.Tls11;
			case TlsProtocolCode.Tls12:
				return TlsProtocols.Tls12;
			default:
				throw new NotSupportedException();
			}
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		[return: TupleElementNames(new string[] { "ret", "wantMore" })]
		public override ValueTuple<int, bool> Read(byte[] buffer, int offset, int size)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(size);
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			ValueTuple<int, bool> valueTuple;
			try
			{
				MonoBtlsError.ClearError();
				MonoBtlsSslError monoBtlsSslError = this.ssl.Read(intPtr, ref size);
				if (monoBtlsSslError == MonoBtlsSslError.WantRead)
				{
					valueTuple = new ValueTuple<int, bool>(0, true);
				}
				else if (monoBtlsSslError == MonoBtlsSslError.ZeroReturn)
				{
					valueTuple = new ValueTuple<int, bool>(size, false);
				}
				else
				{
					if (monoBtlsSslError != MonoBtlsSslError.None)
					{
						throw MonoBtlsContext.GetException(monoBtlsSslError);
					}
					if (size > 0)
					{
						Marshal.Copy(intPtr, buffer, offset, size);
					}
					valueTuple = new ValueTuple<int, bool>(size, false);
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return valueTuple;
		}

		[return: TupleElementNames(new string[] { "ret", "wantMore" })]
		public override ValueTuple<int, bool> Write(byte[] buffer, int offset, int size)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(size);
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			ValueTuple<int, bool> valueTuple;
			try
			{
				MonoBtlsError.ClearError();
				Marshal.Copy(buffer, offset, intPtr, size);
				MonoBtlsSslError monoBtlsSslError = this.ssl.Write(intPtr, ref size);
				if (monoBtlsSslError == MonoBtlsSslError.WantWrite)
				{
					valueTuple = new ValueTuple<int, bool>(0, true);
				}
				else
				{
					if (monoBtlsSslError != MonoBtlsSslError.None)
					{
						throw MonoBtlsContext.GetException(monoBtlsSslError);
					}
					valueTuple = new ValueTuple<int, bool>(size, false);
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return valueTuple;
		}

		public override bool CanRenegotiate
		{
			get
			{
				return false;
			}
		}

		public override void Renegotiate()
		{
			throw new NotSupportedException();
		}

		public override void Shutdown()
		{
			if (base.Settings == null || !base.Settings.SendCloseNotify)
			{
				this.ssl.SetQuietShutdown();
			}
			this.ssl.Shutdown();
		}

		public override bool PendingRenegotiation()
		{
			return this.ssl.RenegotiatePending();
		}

		private void Dispose<T>(ref T disposable) where T : class, IDisposable
		{
			try
			{
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			catch
			{
			}
			finally
			{
				disposable = default(T);
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.Dispose<MonoBtlsSsl>(ref this.ssl);
					this.Dispose<MonoBtlsSslCtx>(ref this.ctx);
					this.Dispose<X509Certificate2>(ref this.remoteCertificate);
					this.Dispose<X509CertificateImplBtls>(ref this.nativeServerCertificate);
					this.Dispose<X509CertificateImplBtls>(ref this.nativeClientCertificate);
					this.Dispose<X509Certificate>(ref this.clientCertificate);
					this.Dispose<MonoBtlsBio>(ref this.bio);
					this.Dispose<MonoBtlsBio>(ref this.errbio);
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		int IMonoBtlsBioMono.Read(byte[] buffer, int offset, int size, out bool wantMore)
		{
			return base.Parent.InternalRead(buffer, offset, size, out wantMore);
		}

		bool IMonoBtlsBioMono.Write(byte[] buffer, int offset, int size)
		{
			return base.Parent.InternalWrite(buffer, offset, size);
		}

		void IMonoBtlsBioMono.Flush()
		{
		}

		void IMonoBtlsBioMono.Close()
		{
		}

		public override bool HasContext
		{
			get
			{
				return this.ssl != null && this.ssl.IsValid;
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return this.isAuthenticated;
			}
		}

		public override MonoTlsConnectionInfo ConnectionInfo
		{
			get
			{
				return this.connectionInfo;
			}
		}

		internal override bool IsRemoteCertificateAvailable
		{
			get
			{
				return this.remoteCertificate != null;
			}
		}

		internal override X509Certificate LocalClientCertificate
		{
			get
			{
				return this.clientCertificate;
			}
		}

		public override X509Certificate2 RemoteCertificate
		{
			get
			{
				return this.remoteCertificate;
			}
		}

		public override TlsProtocols NegotiatedProtocol
		{
			get
			{
				return this.connectionInfo.ProtocolVersion;
			}
		}

		private X509Certificate2 remoteCertificate;

		private X509Certificate clientCertificate;

		private X509CertificateImplBtls nativeServerCertificate;

		private X509CertificateImplBtls nativeClientCertificate;

		private MonoBtlsSslCtx ctx;

		private MonoBtlsSsl ssl;

		private MonoBtlsBio bio;

		private MonoBtlsBio errbio;

		private MonoTlsConnectionInfo connectionInfo;

		private bool certificateValidated;

		private bool isAuthenticated;

		private bool connected;
	}
}
