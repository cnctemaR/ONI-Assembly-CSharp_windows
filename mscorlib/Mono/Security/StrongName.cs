using System;
using System.Configuration.Assemblies;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Mono.Security
{
	internal sealed class StrongName
	{
		public StrongName()
		{
		}

		public StrongName(int keySize)
		{
			this.rsa = new RSAManaged(keySize);
		}

		public StrongName(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length == 16)
			{
				int i = 0;
				int num = 0;
				while (i < data.Length)
				{
					num += (int)data[i++];
				}
				if (num == 4)
				{
					this.publicKey = (byte[])data.Clone();
				}
			}
			else
			{
				this.RSA = CryptoConvert.FromCapiKeyBlob(data);
				if (this.rsa == null)
				{
					throw new ArgumentException("data isn't a correctly encoded RSA public key");
				}
			}
		}

		public StrongName(RSA rsa)
		{
			if (rsa == null)
			{
				throw new ArgumentNullException("rsa");
			}
			this.RSA = rsa;
		}

		private void InvalidateCache()
		{
			this.publicKey = null;
			this.keyToken = null;
		}

		public bool CanSign
		{
			get
			{
				if (this.rsa == null)
				{
					return false;
				}
				if (this.RSA is RSACryptoServiceProvider)
				{
					return !(this.rsa as RSACryptoServiceProvider).PublicOnly;
				}
				if (this.RSA is RSAManaged)
				{
					return !(this.rsa as RSAManaged).PublicOnly;
				}
				bool flag;
				try
				{
					RSAParameters rsaparameters = this.rsa.ExportParameters(true);
					flag = rsaparameters.D != null && rsaparameters.P != null && rsaparameters.Q != null;
				}
				catch (CryptographicException)
				{
					flag = false;
				}
				return flag;
			}
		}

		public RSA RSA
		{
			get
			{
				if (this.rsa == null)
				{
					this.rsa = RSA.Create();
				}
				return this.rsa;
			}
			set
			{
				this.rsa = value;
				this.InvalidateCache();
			}
		}

		public byte[] PublicKey
		{
			get
			{
				if (this.publicKey == null)
				{
					byte[] array = CryptoConvert.ToCapiKeyBlob(this.rsa, false);
					this.publicKey = new byte[32 + (this.rsa.KeySize >> 3)];
					this.publicKey[0] = array[4];
					this.publicKey[1] = array[5];
					this.publicKey[2] = array[6];
					this.publicKey[3] = array[7];
					this.publicKey[4] = 4;
					this.publicKey[5] = 128;
					this.publicKey[6] = 0;
					this.publicKey[7] = 0;
					byte[] bytes = BitConverterLE.GetBytes(this.publicKey.Length - 12);
					this.publicKey[8] = bytes[0];
					this.publicKey[9] = bytes[1];
					this.publicKey[10] = bytes[2];
					this.publicKey[11] = bytes[3];
					this.publicKey[12] = 6;
					Buffer.BlockCopy(array, 1, this.publicKey, 13, this.publicKey.Length - 13);
					this.publicKey[23] = 49;
				}
				return (byte[])this.publicKey.Clone();
			}
		}

		public byte[] PublicKeyToken
		{
			get
			{
				if (this.keyToken == null)
				{
					byte[] array = this.PublicKey;
					if (array == null)
					{
						return null;
					}
					HashAlgorithm hashAlgorithm = HashAlgorithm.Create(this.TokenAlgorithm);
					byte[] array2 = hashAlgorithm.ComputeHash(array);
					this.keyToken = new byte[8];
					Buffer.BlockCopy(array2, array2.Length - 8, this.keyToken, 0, 8);
					Array.Reverse(this.keyToken, 0, 8);
				}
				return (byte[])this.keyToken.Clone();
			}
		}

		public string TokenAlgorithm
		{
			get
			{
				if (this.tokenAlgorithm == null)
				{
					this.tokenAlgorithm = "SHA1";
				}
				return this.tokenAlgorithm;
			}
			set
			{
				string text = value.ToUpper(CultureInfo.InvariantCulture);
				if (text == "SHA1" || text == "MD5")
				{
					this.tokenAlgorithm = value;
					this.InvalidateCache();
					return;
				}
				throw new ArgumentException("Unsupported hash algorithm for token");
			}
		}

		public byte[] GetBytes()
		{
			return CryptoConvert.ToCapiPrivateKeyBlob(this.RSA);
		}

		private uint RVAtoPosition(uint r, int sections, byte[] headers)
		{
			for (int i = 0; i < sections; i++)
			{
				uint num = BitConverterLE.ToUInt32(headers, i * 40 + 20);
				uint num2 = BitConverterLE.ToUInt32(headers, i * 40 + 12);
				int num3 = (int)BitConverterLE.ToUInt32(headers, i * 40 + 8);
				if (num2 <= r && (ulong)r < (ulong)num2 + (ulong)((long)num3))
				{
					return num + r - num2;
				}
			}
			return 0U;
		}

		internal StrongName.StrongNameSignature StrongHash(Stream stream, StrongName.StrongNameOptions options)
		{
			StrongName.StrongNameSignature strongNameSignature = new StrongName.StrongNameSignature();
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create(this.TokenAlgorithm);
			CryptoStream cryptoStream = new CryptoStream(Stream.Null, hashAlgorithm, CryptoStreamMode.Write);
			byte[] array = new byte[128];
			stream.Read(array, 0, 128);
			if (BitConverterLE.ToUInt16(array, 0) != 23117)
			{
				return null;
			}
			uint num = BitConverterLE.ToUInt32(array, 60);
			cryptoStream.Write(array, 0, 128);
			if (num != 128U)
			{
				byte[] array2 = new byte[num - 128U];
				stream.Read(array2, 0, array2.Length);
				cryptoStream.Write(array2, 0, array2.Length);
			}
			byte[] array3 = new byte[248];
			stream.Read(array3, 0, 248);
			if (BitConverterLE.ToUInt32(array3, 0) != 17744U)
			{
				return null;
			}
			if (BitConverterLE.ToUInt16(array3, 4) != 332)
			{
				return null;
			}
			byte[] array4 = new byte[8];
			Buffer.BlockCopy(array4, 0, array3, 88, 4);
			Buffer.BlockCopy(array4, 0, array3, 152, 8);
			cryptoStream.Write(array3, 0, 248);
			ushort num2 = BitConverterLE.ToUInt16(array3, 6);
			int num3 = (int)(num2 * 40);
			byte[] array5 = new byte[num3];
			stream.Read(array5, 0, num3);
			cryptoStream.Write(array5, 0, num3);
			uint num4 = BitConverterLE.ToUInt32(array3, 232);
			uint num5 = this.RVAtoPosition(num4, (int)num2, array5);
			int num6 = (int)BitConverterLE.ToUInt32(array3, 236);
			byte[] array6 = new byte[num6];
			stream.Position = (long)((ulong)num5);
			stream.Read(array6, 0, num6);
			uint num7 = BitConverterLE.ToUInt32(array6, 32);
			strongNameSignature.SignaturePosition = this.RVAtoPosition(num7, (int)num2, array5);
			strongNameSignature.SignatureLength = BitConverterLE.ToUInt32(array6, 36);
			uint num8 = BitConverterLE.ToUInt32(array6, 8);
			strongNameSignature.MetadataPosition = this.RVAtoPosition(num8, (int)num2, array5);
			strongNameSignature.MetadataLength = BitConverterLE.ToUInt32(array6, 12);
			if (options == StrongName.StrongNameOptions.Metadata)
			{
				cryptoStream.Close();
				hashAlgorithm.Initialize();
				byte[] array7 = new byte[strongNameSignature.MetadataLength];
				stream.Position = (long)((ulong)strongNameSignature.MetadataPosition);
				stream.Read(array7, 0, array7.Length);
				strongNameSignature.Hash = hashAlgorithm.ComputeHash(array7);
				return strongNameSignature;
			}
			for (int i = 0; i < (int)num2; i++)
			{
				uint num9 = BitConverterLE.ToUInt32(array5, i * 40 + 20);
				int num10 = (int)BitConverterLE.ToUInt32(array5, i * 40 + 16);
				byte[] array8 = new byte[num10];
				stream.Position = (long)((ulong)num9);
				stream.Read(array8, 0, num10);
				if (num9 <= strongNameSignature.SignaturePosition && (ulong)strongNameSignature.SignaturePosition < (ulong)num9 + (ulong)((long)num10))
				{
					int num11 = (int)(strongNameSignature.SignaturePosition - num9);
					if (num11 > 0)
					{
						cryptoStream.Write(array8, 0, num11);
					}
					strongNameSignature.Signature = new byte[strongNameSignature.SignatureLength];
					Buffer.BlockCopy(array8, num11, strongNameSignature.Signature, 0, (int)strongNameSignature.SignatureLength);
					Array.Reverse(strongNameSignature.Signature);
					int num12 = (int)((long)num11 + (long)((ulong)strongNameSignature.SignatureLength));
					int num13 = num10 - num12;
					if (num13 > 0)
					{
						cryptoStream.Write(array8, num12, num13);
					}
				}
				else
				{
					cryptoStream.Write(array8, 0, num10);
				}
			}
			cryptoStream.Close();
			strongNameSignature.Hash = hashAlgorithm.Hash;
			return strongNameSignature;
		}

		public byte[] Hash(string fileName)
		{
			FileStream fileStream = File.OpenRead(fileName);
			StrongName.StrongNameSignature strongNameSignature = this.StrongHash(fileStream, StrongName.StrongNameOptions.Metadata);
			fileStream.Close();
			return strongNameSignature.Hash;
		}

		public bool Sign(string fileName)
		{
			bool flag = false;
			StrongName.StrongNameSignature strongNameSignature;
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				strongNameSignature = this.StrongHash(fileStream, StrongName.StrongNameOptions.Signature);
				fileStream.Close();
			}
			if (strongNameSignature.Hash == null)
			{
				return false;
			}
			byte[] array = null;
			try
			{
				RSAPKCS1SignatureFormatter rsapkcs1SignatureFormatter = new RSAPKCS1SignatureFormatter(this.rsa);
				rsapkcs1SignatureFormatter.SetHashAlgorithm(this.TokenAlgorithm);
				array = rsapkcs1SignatureFormatter.CreateSignature(strongNameSignature.Hash);
				Array.Reverse(array);
			}
			catch (CryptographicException)
			{
				return false;
			}
			using (FileStream fileStream2 = File.OpenWrite(fileName))
			{
				fileStream2.Position = (long)((ulong)strongNameSignature.SignaturePosition);
				fileStream2.Write(array, 0, array.Length);
				fileStream2.Close();
				flag = true;
			}
			return flag;
		}

		public bool Verify(string fileName)
		{
			bool flag = false;
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				flag = this.Verify(fileStream);
				fileStream.Close();
			}
			return flag;
		}

		public bool Verify(Stream stream)
		{
			StrongName.StrongNameSignature strongNameSignature = this.StrongHash(stream, StrongName.StrongNameOptions.Signature);
			if (strongNameSignature.Hash == null)
			{
				return false;
			}
			bool flag;
			try
			{
				AssemblyHashAlgorithm assemblyHashAlgorithm = AssemblyHashAlgorithm.SHA1;
				if (this.tokenAlgorithm == "MD5")
				{
					assemblyHashAlgorithm = AssemblyHashAlgorithm.MD5;
				}
				flag = StrongName.Verify(this.rsa, assemblyHashAlgorithm, strongNameSignature.Hash, strongNameSignature.Signature);
			}
			catch (CryptographicException)
			{
				flag = false;
			}
			return flag;
		}

		public static bool IsAssemblyStrongnamed(string assemblyName)
		{
			if (!StrongName.initialized)
			{
				object obj = StrongName.lockObject;
				lock (obj)
				{
					if (!StrongName.initialized)
					{
						string machineConfigPath = Environment.GetMachineConfigPath();
						StrongNameManager.LoadConfig(machineConfigPath);
						StrongName.initialized = true;
					}
				}
			}
			bool flag;
			try
			{
				AssemblyName assemblyName2 = AssemblyName.GetAssemblyName(assemblyName);
				if (assemblyName2 == null)
				{
					flag = false;
				}
				else
				{
					byte[] mappedPublicKey = StrongNameManager.GetMappedPublicKey(assemblyName2.GetPublicKeyToken());
					if (mappedPublicKey == null || mappedPublicKey.Length < 12)
					{
						mappedPublicKey = assemblyName2.GetPublicKey();
						if (mappedPublicKey == null || mappedPublicKey.Length < 12)
						{
							return false;
						}
					}
					if (!StrongNameManager.MustVerify(assemblyName2))
					{
						flag = true;
					}
					else
					{
						RSA rsa = CryptoConvert.FromCapiPublicKeyBlob(mappedPublicKey, 12);
						StrongName strongName = new StrongName(rsa);
						bool flag2 = strongName.Verify(assemblyName);
						flag = flag2;
					}
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public static bool VerifySignature(byte[] publicKey, int algorithm, byte[] hash, byte[] signature)
		{
			bool flag;
			try
			{
				RSA rsa = CryptoConvert.FromCapiPublicKeyBlob(publicKey);
				flag = StrongName.Verify(rsa, (AssemblyHashAlgorithm)algorithm, hash, signature);
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		private static bool Verify(RSA rsa, AssemblyHashAlgorithm algorithm, byte[] hash, byte[] signature)
		{
			RSAPKCS1SignatureDeformatter rsapkcs1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
			if (algorithm != AssemblyHashAlgorithm.MD5)
			{
				if (algorithm != AssemblyHashAlgorithm.SHA1 && algorithm != AssemblyHashAlgorithm.None)
				{
				}
				rsapkcs1SignatureDeformatter.SetHashAlgorithm("SHA1");
			}
			else
			{
				rsapkcs1SignatureDeformatter.SetHashAlgorithm("MD5");
			}
			return rsapkcs1SignatureDeformatter.VerifySignature(hash, signature);
		}

		private RSA rsa;

		private byte[] publicKey;

		private byte[] keyToken;

		private string tokenAlgorithm;

		private static object lockObject = new object();

		private static bool initialized = false;

		internal class StrongNameSignature
		{
			public byte[] Hash
			{
				get
				{
					return this.hash;
				}
				set
				{
					this.hash = value;
				}
			}

			public byte[] Signature
			{
				get
				{
					return this.signature;
				}
				set
				{
					this.signature = value;
				}
			}

			public uint MetadataPosition
			{
				get
				{
					return this.metadataPosition;
				}
				set
				{
					this.metadataPosition = value;
				}
			}

			public uint MetadataLength
			{
				get
				{
					return this.metadataLength;
				}
				set
				{
					this.metadataLength = value;
				}
			}

			public uint SignaturePosition
			{
				get
				{
					return this.signaturePosition;
				}
				set
				{
					this.signaturePosition = value;
				}
			}

			public uint SignatureLength
			{
				get
				{
					return this.signatureLength;
				}
				set
				{
					this.signatureLength = value;
				}
			}

			public byte CliFlag
			{
				get
				{
					return this.cliFlag;
				}
				set
				{
					this.cliFlag = value;
				}
			}

			public uint CliFlagPosition
			{
				get
				{
					return this.cliFlagPosition;
				}
				set
				{
					this.cliFlagPosition = value;
				}
			}

			private byte[] hash;

			private byte[] signature;

			private uint signaturePosition;

			private uint signatureLength;

			private uint metadataPosition;

			private uint metadataLength;

			private byte cliFlag;

			private uint cliFlagPosition;
		}

		internal enum StrongNameOptions
		{
			Metadata,
			Signature
		}
	}
}
