using System;
using System.Security.Cryptography;

namespace Mono.Security.Protocol.Tls
{
	internal class TlsCipherSuite : CipherSuite
	{
		public TlsCipherSuite(short code, string name, CipherAlgorithmType cipherAlgorithmType, HashAlgorithmType hashAlgorithmType, ExchangeAlgorithmType exchangeAlgorithmType, bool exportable, bool blockMode, byte keyMaterialSize, byte expandedKeyMaterialSize, short effectiveKeyBytes, byte ivSize, byte blockSize)
			: base(code, name, cipherAlgorithmType, hashAlgorithmType, exchangeAlgorithmType, exportable, blockMode, keyMaterialSize, expandedKeyMaterialSize, effectiveKeyBytes, ivSize, blockSize)
		{
		}

		public override byte[] ComputeServerRecordMAC(ContentType contentType, byte[] fragment)
		{
			object obj = this.headerLock;
			byte[] hash;
			lock (obj)
			{
				if (this.header == null)
				{
					this.header = new byte[13];
				}
				ulong num = ((base.Context is ClientContext) ? base.Context.ReadSequenceNumber : base.Context.WriteSequenceNumber);
				base.Write(this.header, 0, num);
				this.header[8] = (byte)contentType;
				base.Write(this.header, 9, base.Context.Protocol);
				base.Write(this.header, 11, (short)fragment.Length);
				KeyedHashAlgorithm serverHMAC = base.ServerHMAC;
				serverHMAC.TransformBlock(this.header, 0, this.header.Length, this.header, 0);
				serverHMAC.TransformBlock(fragment, 0, fragment.Length, fragment, 0);
				serverHMAC.TransformFinalBlock(CipherSuite.EmptyArray, 0, 0);
				hash = serverHMAC.Hash;
			}
			return hash;
		}

		public override byte[] ComputeClientRecordMAC(ContentType contentType, byte[] fragment)
		{
			object obj = this.headerLock;
			byte[] hash;
			lock (obj)
			{
				if (this.header == null)
				{
					this.header = new byte[13];
				}
				ulong num = ((base.Context is ClientContext) ? base.Context.WriteSequenceNumber : base.Context.ReadSequenceNumber);
				base.Write(this.header, 0, num);
				this.header[8] = (byte)contentType;
				base.Write(this.header, 9, base.Context.Protocol);
				base.Write(this.header, 11, (short)fragment.Length);
				KeyedHashAlgorithm clientHMAC = base.ClientHMAC;
				clientHMAC.TransformBlock(this.header, 0, this.header.Length, this.header, 0);
				clientHMAC.TransformBlock(fragment, 0, fragment.Length, fragment, 0);
				clientHMAC.TransformFinalBlock(CipherSuite.EmptyArray, 0, 0);
				hash = clientHMAC.Hash;
			}
			return hash;
		}

		public override void ComputeMasterSecret(byte[] preMasterSecret)
		{
			base.Context.MasterSecret = new byte[preMasterSecret.Length];
			base.Context.MasterSecret = base.PRF(preMasterSecret, "master secret", base.Context.RandomCS, 48);
		}

		public override void ComputeKeys()
		{
			TlsStream tlsStream = new TlsStream(base.PRF(base.Context.MasterSecret, "key expansion", base.Context.RandomSC, base.KeyBlockSize));
			base.Context.Negotiating.ClientWriteMAC = tlsStream.ReadBytes(base.HashSize);
			base.Context.Negotiating.ServerWriteMAC = tlsStream.ReadBytes(base.HashSize);
			base.Context.ClientWriteKey = tlsStream.ReadBytes((int)base.KeyMaterialSize);
			base.Context.ServerWriteKey = tlsStream.ReadBytes((int)base.KeyMaterialSize);
			if (base.IvSize != 0)
			{
				base.Context.ClientWriteIV = tlsStream.ReadBytes((int)base.IvSize);
				base.Context.ServerWriteIV = tlsStream.ReadBytes((int)base.IvSize);
			}
			else
			{
				base.Context.ClientWriteIV = CipherSuite.EmptyArray;
				base.Context.ServerWriteIV = CipherSuite.EmptyArray;
			}
			ClientSessionCache.SetContextInCache(base.Context);
			tlsStream.Reset();
		}

		private const int MacHeaderLength = 13;

		private byte[] header;

		private object headerLock = new object();
	}
}
