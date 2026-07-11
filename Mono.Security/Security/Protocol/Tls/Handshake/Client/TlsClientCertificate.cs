using System;
using System.Security.Cryptography.X509Certificates;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsClientCertificate : HandshakeMessage
	{
		public TlsClientCertificate(Context context)
			: base(context, HandshakeType.Certificate)
		{
		}

		public X509Certificate ClientCertificate
		{
			get
			{
				if (!this.clientCertSelected)
				{
					this.GetClientCertificate();
					this.clientCertSelected = true;
				}
				return this.clientCert;
			}
		}

		public override void Update()
		{
			base.Update();
			base.Reset();
		}

		private void GetClientCertificate()
		{
			ClientContext clientContext = (ClientContext)base.Context;
			if (clientContext.ClientSettings.Certificates != null && clientContext.ClientSettings.Certificates.Count > 0)
			{
				this.clientCert = clientContext.SslStream.RaiseClientCertificateSelection(base.Context.ClientSettings.Certificates, new X509Certificate(base.Context.ServerSettings.Certificates[0].RawData), base.Context.ClientSettings.TargetHost, null);
			}
			clientContext.ClientSettings.ClientCertificate = this.clientCert;
		}

		private void SendCertificates()
		{
			TlsStream tlsStream = new TlsStream();
			for (X509Certificate x509Certificate = this.ClientCertificate; x509Certificate != null; x509Certificate = this.FindParentCertificate(x509Certificate))
			{
				byte[] rawCertData = x509Certificate.GetRawCertData();
				tlsStream.WriteInt24(rawCertData.Length);
				tlsStream.Write(rawCertData);
			}
			base.WriteInt24((int)tlsStream.Length);
			base.Write(tlsStream.ToArray());
		}

		protected override void ProcessAsSsl3()
		{
			if (this.ClientCertificate != null)
			{
				this.SendCertificates();
			}
		}

		protected override void ProcessAsTls1()
		{
			if (this.ClientCertificate != null)
			{
				this.SendCertificates();
				return;
			}
			base.WriteInt24(0);
		}

		private X509Certificate FindParentCertificate(X509Certificate cert)
		{
			if (cert.GetName() == cert.GetIssuerName())
			{
				return null;
			}
			foreach (X509Certificate x509Certificate in base.Context.ClientSettings.Certificates)
			{
				if (x509Certificate.GetName() == cert.GetIssuerName())
				{
					return x509Certificate;
				}
			}
			return null;
		}

		private bool clientCertSelected;

		private X509Certificate clientCert;
	}
}
