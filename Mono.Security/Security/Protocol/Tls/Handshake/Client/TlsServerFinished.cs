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
			HashAlgorithm hashAlgorithm = new SslHandshakeHash(base.Context.MasterSecret);
			byte[] array = base.Context.HandshakeMessages.ToArray();
			hashAlgorithm.TransformBlock(array, 0, array.Length, array, 0);
			hashAlgorithm.TransformBlock(TlsServerFinished.Ssl3Marker, 0, TlsServerFinished.Ssl3Marker.Length, TlsServerFinished.Ssl3Marker, 0);
			hashAlgorithm.TransformFinalBlock(CipherSuite.EmptyArray, 0, 0);
			byte[] array2 = base.ReadBytes((int)this.Length);
			byte[] hash = hashAlgorithm.Hash;
			if (!HandshakeMessage.Compare(hash, array2))
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
			byte[] array4 = base.Context.Current.Cipher.PRF(base.Context.MasterSecret, "server finished", array3, 12);
			if (!HandshakeMessage.Compare(array4, array))
			{
				throw new TlsException("Invalid ServerFinished message received.");
			}
		}

		private static byte[] Ssl3Marker = new byte[] { 83, 82, 86, 82 };
	}
}
