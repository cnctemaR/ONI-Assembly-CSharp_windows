using System;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Tls.Handshake.Server
{
	internal class TlsClientCertificateVerify : HandshakeMessage
	{
		public TlsClientCertificateVerify(Context context, byte[] buffer)
			: base(context, HandshakeType.CertificateVerify, buffer)
		{
		}

		protected override void ProcessAsSsl3()
		{
			ServerContext serverContext = (ServerContext)base.Context;
			int num = (int)base.ReadInt16();
			byte[] array = base.ReadBytes(num);
			SslHandshakeHash sslHandshakeHash = new SslHandshakeHash(serverContext.MasterSecret);
			sslHandshakeHash.TransformFinalBlock(serverContext.HandshakeMessages.ToArray(), 0, (int)serverContext.HandshakeMessages.Length);
			if (!sslHandshakeHash.VerifySignature(serverContext.ClientSettings.CertificateRSA, array))
			{
				throw new TlsException(AlertDescription.HandshakeFailiure, "Handshake Failure.");
			}
		}

		protected override void ProcessAsTls1()
		{
			ServerContext serverContext = (ServerContext)base.Context;
			int num = (int)base.ReadInt16();
			byte[] array = base.ReadBytes(num);
			MD5SHA1 md5SHA = new MD5SHA1();
			md5SHA.ComputeHash(serverContext.HandshakeMessages.ToArray(), 0, (int)serverContext.HandshakeMessages.Length);
			if (!md5SHA.VerifySignature(serverContext.ClientSettings.CertificateRSA, array))
			{
				throw new TlsException(AlertDescription.HandshakeFailiure, "Handshake Failure.");
			}
		}
	}
}
