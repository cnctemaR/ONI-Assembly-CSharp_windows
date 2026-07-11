using System;
using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsServerFinished : HandshakeMessage
	{
		public TlsServerFinished(Context context, byte[] buffer)
			: base(context, HandshakeType.Finished, buffer)
		{
		}

		public override void Update()
		{
			base.Update();
			base.Context.HandshakeState = HandshakeState.Finished;
		}

		protected override void ProcessAsSsl3()
		{
			SslHandshakeHash sslHandshakeHash = new SslHandshakeHash(base.Context.MasterSecret);
			byte[] array = base.Context.HandshakeMessages.ToArray();
			sslHandshakeHash.TransformBlock(array, 0, array.Length, array, 0);
			sslHandshakeHash.TransformBlock(TlsServerFinished.Ssl3Marker, 0, TlsServerFinished.Ssl3Marker.Length, TlsServerFinished.Ssl3Marker, 0);
			sslHandshakeHash.TransformFinalBlock(CipherSuite.EmptyArray, 0, 0);
			byte[] array2 = base.ReadBytes((int)this.Length);
			if (!HandshakeMessage.Compare(sslHandshakeHash.Hash, array2))
			{
				throw new TlsException(AlertDescription.InsuficientSecurity, "Invalid ServerFinished message received.");
			}
		}

		protected override void ProcessAsTls1()
		{
			byte[] array = base.ReadBytes((int)this.Length);
			HashAlgorithm hashAlgorithm = new MD5SHA1();
			byte[] array2 = base.Context.HandshakeMessages.ToArray();
			byte[] array3 = hashAlgorithm.ComputeHash(array2, 0, array2.Length);
			if (!HandshakeMessage.Compare(base.Context.Current.Cipher.PRF(base.Context.MasterSecret, "server finished", array3, 12), array))
			{
				throw new TlsException("Invalid ServerFinished message received.");
			}
		}

		private static byte[] Ssl3Marker = new byte[] { 83, 82, 86, 82 };
	}
}
