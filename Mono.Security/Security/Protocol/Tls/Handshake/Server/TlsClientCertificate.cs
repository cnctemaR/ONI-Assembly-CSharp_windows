using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.X509;
using Mono.Security.X509.Extensions;

namespace Mono.Security.Protocol.Tls.Handshake.Server
{
	internal class TlsClientCertificate : HandshakeMessage
	{
		public TlsClientCertificate(Context context, byte[] buffer)
			: base(context, HandshakeType.Certificate, buffer)
		{
		}

		public override void Update()
		{
			foreach (Mono.Security.X509.X509Certificate x509Certificate in this.clientCertificates)
			{
				base.Context.ClientSettings.Certificates.Add(new global::System.Security.Cryptography.X509Certificates.X509Certificate(x509Certificate.RawData));
			}
		}

		public bool HasCertificate
		{
			get
			{
				return this.clientCertificates.Count > 0;
			}
		}

		protected override void ProcessAsSsl3()
		{
			this.ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			int num = 0;
			int i = base.ReadInt24();
			this.clientCertificates = new Mono.Security.X509.X509CertificateCollection();
			while (i > num)
			{
				int num2 = base.ReadInt24();
				num += num2 + 3;
				byte[] array = base.ReadBytes(num2);
				this.clientCertificates.Add(new Mono.Security.X509.X509Certificate(array));
			}
			if (this.clientCertificates.Count > 0)
			{
				this.validateCertificates(this.clientCertificates);
				return;
			}
			if ((base.Context as ServerContext).ClientCertificateRequired)
			{
				throw new TlsException(AlertDescription.NoCertificate);
			}
		}

		private bool checkCertificateUsage(Mono.Security.X509.X509Certificate cert)
		{
			ServerContext serverContext = (ServerContext)base.Context;
			if (cert.Version < 3)
			{
				return true;
			}
			KeyUsages keyUsages = KeyUsages.none;
			switch (serverContext.Negotiating.Cipher.ExchangeAlgorithmType)
			{
			case ExchangeAlgorithmType.DiffieHellman:
				keyUsages = KeyUsages.keyAgreement;
				break;
			case ExchangeAlgorithmType.Fortezza:
				return false;
			case ExchangeAlgorithmType.RsaKeyX:
			case ExchangeAlgorithmType.RsaSign:
				keyUsages = KeyUsages.digitalSignature;
				break;
			}
			KeyUsageExtension keyUsageExtension = null;
			ExtendedKeyUsageExtension extendedKeyUsageExtension = null;
			Mono.Security.X509.X509Extension x509Extension = cert.Extensions["2.5.29.15"];
			if (x509Extension != null)
			{
				keyUsageExtension = new KeyUsageExtension(x509Extension);
			}
			x509Extension = cert.Extensions["2.5.29.37"];
			if (x509Extension != null)
			{
				extendedKeyUsageExtension = new ExtendedKeyUsageExtension(x509Extension);
			}
			if (keyUsageExtension != null && extendedKeyUsageExtension != null)
			{
				return keyUsageExtension.Support(keyUsages) && extendedKeyUsageExtension.KeyPurpose.Contains("1.3.6.1.5.5.7.3.2");
			}
			if (keyUsageExtension != null)
			{
				return keyUsageExtension.Support(keyUsages);
			}
			if (extendedKeyUsageExtension != null)
			{
				return extendedKeyUsageExtension.KeyPurpose.Contains("1.3.6.1.5.5.7.3.2");
			}
			x509Extension = cert.Extensions["2.16.840.1.113730.1.1"];
			return x509Extension != null && new NetscapeCertTypeExtension(x509Extension).Support(NetscapeCertTypeExtension.CertTypes.SslClient);
		}

		private void validateCertificates(Mono.Security.X509.X509CertificateCollection certificates)
		{
			ServerContext serverContext = (ServerContext)base.Context;
			AlertDescription alertDescription = AlertDescription.BadCertificate;
			global::System.Security.Cryptography.X509Certificates.X509Certificate x509Certificate = null;
			int[] array = null;
			if (certificates.Count > 0)
			{
				Mono.Security.X509.X509Certificate x509Certificate2 = certificates[0];
				ArrayList arrayList = new ArrayList();
				if (!this.checkCertificateUsage(x509Certificate2))
				{
					arrayList.Add(-2146762490);
				}
				Mono.Security.X509.X509Chain x509Chain;
				if (certificates.Count > 1)
				{
					Mono.Security.X509.X509CertificateCollection x509CertificateCollection = new Mono.Security.X509.X509CertificateCollection(certificates);
					x509CertificateCollection.Remove(x509Certificate2);
					x509Chain = new Mono.Security.X509.X509Chain(x509CertificateCollection);
				}
				else
				{
					x509Chain = new Mono.Security.X509.X509Chain();
				}
				bool flag = false;
				try
				{
					flag = x509Chain.Build(x509Certificate2);
				}
				catch (Exception)
				{
					flag = false;
				}
				if (!flag)
				{
					Mono.Security.X509.X509ChainStatusFlags status = x509Chain.Status;
					if (status <= Mono.Security.X509.X509ChainStatusFlags.NotSignatureValid)
					{
						if (status == Mono.Security.X509.X509ChainStatusFlags.NotTimeValid)
						{
							alertDescription = AlertDescription.CertificateExpired;
							arrayList.Add(-2146762495);
							goto IL_016C;
						}
						if (status == Mono.Security.X509.X509ChainStatusFlags.NotTimeNested)
						{
							arrayList.Add(-2146762494);
							goto IL_016C;
						}
						if (status == Mono.Security.X509.X509ChainStatusFlags.NotSignatureValid)
						{
							arrayList.Add(-2146869232);
							goto IL_016C;
						}
					}
					else
					{
						if (status == Mono.Security.X509.X509ChainStatusFlags.UntrustedRoot)
						{
							alertDescription = AlertDescription.UnknownCA;
							arrayList.Add(-2146762487);
							goto IL_016C;
						}
						if (status == Mono.Security.X509.X509ChainStatusFlags.InvalidBasicConstraints)
						{
							arrayList.Add(-2146869223);
							goto IL_016C;
						}
						if (status == Mono.Security.X509.X509ChainStatusFlags.PartialChain)
						{
							alertDescription = AlertDescription.UnknownCA;
							arrayList.Add(-2146762486);
							goto IL_016C;
						}
					}
					alertDescription = AlertDescription.CertificateUnknown;
					arrayList.Add((int)x509Chain.Status);
				}
				IL_016C:
				x509Certificate = new global::System.Security.Cryptography.X509Certificates.X509Certificate(x509Certificate2.RawData);
				array = (int[])arrayList.ToArray(typeof(int));
			}
			else
			{
				array = new int[0];
			}
			global::System.Security.Cryptography.X509Certificates.X509CertificateCollection x509CertificateCollection2 = new global::System.Security.Cryptography.X509Certificates.X509CertificateCollection();
			foreach (Mono.Security.X509.X509Certificate x509Certificate3 in certificates)
			{
				x509CertificateCollection2.Add(new global::System.Security.Cryptography.X509Certificates.X509Certificate(x509Certificate3.RawData));
			}
			if (!serverContext.SslStream.RaiseClientCertificateValidation(x509Certificate, array))
			{
				throw new TlsException(alertDescription, "Invalid certificate received from client.");
			}
			base.Context.ClientSettings.ClientCertificate = x509Certificate;
		}

		private Mono.Security.X509.X509CertificateCollection clientCertificates;
	}
}
