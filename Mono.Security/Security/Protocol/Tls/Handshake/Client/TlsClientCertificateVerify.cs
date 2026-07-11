using System;
using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsClientCertificateVerify : HandshakeMessage
	{
		public TlsClientCertificateVerify(Context context)
			: base(context, HandshakeType.CertificateVerify)
		{
		}

		public override void Update()
		{
			base.Update();
			base.Reset();
		}

		protected override void ProcessAsSsl3()
		{
			AsymmetricAlgorithm asymmetricAlgorithm = null;
			ClientContext clientContext = (ClientContext)base.Context;
			asymmetricAlgorithm = clientContext.SslStream.RaisePrivateKeySelection(clientContext.ClientSettings.ClientCertificate, clientContext.ClientSettings.TargetHost);
			if (asymmetricAlgorithm == null)
			{
				throw new TlsException(AlertDescription.UserCancelled, "Client certificate Private Key unavailable.");
			}
			SslHandshakeHash sslHandshakeHash = new SslHandshakeHash(clientContext.MasterSecret);
			sslHandshakeHash.TransformFinalBlock(clientContext.HandshakeMessages.ToArray(), 0, (int)clientContext.HandshakeMessages.Length);
			byte[] array = null;
			if (!(asymmetricAlgorithm is RSACryptoServiceProvider))
			{
				try
				{
					array = sslHandshakeHash.CreateSignature((RSA)asymmetricAlgorithm);
				}
				catch (NotImplementedException)
				{
				}
			}
			if (array == null)
			{
				RSA clientCertRSA = this.getClientCertRSA((RSA)asymmetricAlgorithm);
				array = sslHandshakeHash.CreateSignature(clientCertRSA);
			}
			base.Write((short)array.Length);
			this.Write(array, 0, array.Length);
		}

		protected override void ProcessAsTls1()
		{
			AsymmetricAlgorithm asymmetricAlgorithm = null;
			ClientContext clientContext = (ClientContext)base.Context;
			asymmetricAlgorithm = clientContext.SslStream.RaisePrivateKeySelection(clientContext.ClientSettings.ClientCertificate, clientContext.ClientSettings.TargetHost);
			if (asymmetricAlgorithm == null)
			{
				throw new TlsException(AlertDescription.UserCancelled, "Client certificate Private Key unavailable.");
			}
			MD5SHA1 md5SHA = new MD5SHA1();
			md5SHA.ComputeHash(clientContext.HandshakeMessages.ToArray(), 0, (int)clientContext.HandshakeMessages.Length);
			byte[] array = null;
			if (!(asymmetricAlgorithm is RSACryptoServiceProvider))
			{
				try
				{
					array = md5SHA.CreateSignature((RSA)asymmetricAlgorithm);
				}
				catch (NotImplementedException)
				{
				}
			}
			if (array == null)
			{
				RSA clientCertRSA = this.getClientCertRSA((RSA)asymmetricAlgorithm);
				array = md5SHA.CreateSignature(clientCertRSA);
			}
			base.Write((short)array.Length);
			this.Write(array, 0, array.Length);
		}

		private RSA getClientCertRSA(RSA privKey)
		{
			RSAParameters rsaparameters = default(RSAParameters);
			RSAParameters rsaparameters2 = privKey.ExportParameters(true);
			ASN1 asn = new ASN1(base.Context.ClientSettings.Certificates[0].GetPublicKey());
			ASN1 asn2 = asn[0];
			if (asn2 == null || asn2.Tag != 2)
			{
				return null;
			}
			ASN1 asn3 = asn[1];
			if (asn3.Tag != 2)
			{
				return null;
			}
			rsaparameters.Modulus = this.getUnsignedBigInteger(asn2.Value);
			rsaparameters.Exponent = asn3.Value;
			rsaparameters.D = rsaparameters2.D;
			rsaparameters.DP = rsaparameters2.DP;
			rsaparameters.DQ = rsaparameters2.DQ;
			rsaparameters.InverseQ = rsaparameters2.InverseQ;
			rsaparameters.P = rsaparameters2.P;
			rsaparameters.Q = rsaparameters2.Q;
			RSAManaged rsamanaged = new RSAManaged(rsaparameters.Modulus.Length << 3);
			rsamanaged.ImportParameters(rsaparameters);
			return rsamanaged;
		}

		private byte[] getUnsignedBigInteger(byte[] integer)
		{
			if (integer[0] == 0)
			{
				int num = integer.Length - 1;
				byte[] array = new byte[num];
				Buffer.BlockCopy(integer, 1, array, 0, num);
				return array;
			}
			return integer;
		}
	}
}
