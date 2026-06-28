using System;
using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Tls.Handshake.Server
{
	internal class TlsClientFinished : HandshakeMessage
	{
		public TlsClientFinished(Context context, byte[] buffer)
			: base(context, HandshakeType.Finished, buffer)
		{
		}

		protected override void ProcessAsSsl3()
		{
			HashAlgorithm hashAlgorithm = new SslHandshakeHash(base.Context.MasterSecret);
			TlsStream tlsStream = new TlsStream();
			tlsStream.Write(base.Context.HandshakeMessages.ToArray());
			tlsStream.Write(1129074260);
			hashAlgorithm.TransformFinalBlock(tlsStream.ToArray(), 0, (int)tlsStream.Length);
			tlsStream.Reset();
			byte[] array = base.ReadBytes((int)this.Length);
			byte[] hash = hashAlgorithm.Hash;
			if (!HandshakeMessage.Compare(array, hash))
			{
				throw new TlsException(AlertDescription.DecryptError, "Decrypt error.");
			}
		}

		protected override void ProcessAsTls1()
		{
			byte[] array = base.ReadBytes((int)this.Length);
			HashAlgorithm hashAlgorithm = new MD5SHA1();
			byte[] array2 = base.Context.HandshakeMessages.ToArray();
			byte[] array3 = hashAlgorithm.ComputeHash(array2, 0, array2.Length);
			byte[] array4 = base.Context.Current.Cipher.PRF(base.Context.MasterSecret, "client finished", array3, 12);
			if (!HandshakeMessage.Compare(array, array4))
			{
				throw new TlsException(AlertDescription.DecryptError, "Decrypt error.");
			}
		}
	}
}
