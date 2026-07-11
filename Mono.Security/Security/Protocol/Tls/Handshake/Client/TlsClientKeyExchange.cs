using System;
using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsClientKeyExchange : HandshakeMessage
	{
		public TlsClientKeyExchange(Context context)
			: base(context, HandshakeType.ClientKeyExchange)
		{
		}

		protected override void ProcessAsSsl3()
		{
			this.ProcessCommon(false);
		}

		protected override void ProcessAsTls1()
		{
			this.ProcessCommon(true);
		}

		public void ProcessCommon(bool sendLength)
		{
			byte[] array = base.Context.Negotiating.Cipher.CreatePremasterSecret();
			RSA rsa;
			if (base.Context.ServerSettings.ServerKeyExchange)
			{
				rsa = new RSAManaged();
				rsa.ImportParameters(base.Context.ServerSettings.RsaParameters);
			}
			else
			{
				rsa = base.Context.ServerSettings.CertificateRSA;
			}
			byte[] array2 = new RSAPKCS1KeyExchangeFormatter(rsa).CreateKeyExchange(array);
			if (sendLength)
			{
				base.Write((short)array2.Length);
			}
			base.Write(array2);
			base.Context.Negotiating.Cipher.ComputeMasterSecret(array);
			base.Context.Negotiating.Cipher.ComputeKeys();
			rsa.Clear();
		}
	}
}
