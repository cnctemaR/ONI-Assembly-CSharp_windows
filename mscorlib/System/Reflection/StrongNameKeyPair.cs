using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Permissions;
using Mono.Security;
using Mono.Security.Cryptography;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public class StrongNameKeyPair : ISerializable, IDeserializationCallback
	{
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public StrongNameKeyPair(byte[] keyPairArray)
		{
			if (keyPairArray == null)
			{
				throw new ArgumentNullException("keyPairArray");
			}
			this.LoadKey(keyPairArray);
			this.GetRSA();
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public StrongNameKeyPair(FileStream keyPairFile)
		{
			if (keyPairFile == null)
			{
				throw new ArgumentNullException("keyPairFile");
			}
			byte[] array = new byte[keyPairFile.Length];
			keyPairFile.Read(array, 0, array.Length);
			this.LoadKey(array);
			this.GetRSA();
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public StrongNameKeyPair(string keyPairContainer)
		{
			if (keyPairContainer == null)
			{
				throw new ArgumentNullException("keyPairContainer");
			}
			this._keyPairContainer = keyPairContainer;
			this.GetRSA();
		}

		protected StrongNameKeyPair(SerializationInfo info, StreamingContext context)
		{
			this._publicKey = (byte[])info.GetValue("_publicKey", typeof(byte[]));
			this._keyPairContainer = info.GetString("_keyPairContainer");
			this._keyPairExported = info.GetBoolean("_keyPairExported");
			this._keyPairArray = (byte[])info.GetValue("_keyPairArray", typeof(byte[]));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("_publicKey", this._publicKey, typeof(byte[]));
			info.AddValue("_keyPairContainer", this._keyPairContainer);
			info.AddValue("_keyPairExported", this._keyPairExported);
			info.AddValue("_keyPairArray", this._keyPairArray, typeof(byte[]));
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		private RSA GetRSA()
		{
			if (this._rsa != null)
			{
				return this._rsa;
			}
			if (this._keyPairArray != null)
			{
				try
				{
					this._rsa = CryptoConvert.FromCapiKeyBlob(this._keyPairArray);
				}
				catch
				{
					this._keyPairArray = null;
				}
			}
			else if (this._keyPairContainer != null)
			{
				this._rsa = new RSACryptoServiceProvider(new CspParameters
				{
					KeyContainerName = this._keyPairContainer
				});
			}
			return this._rsa;
		}

		private void LoadKey(byte[] key)
		{
			try
			{
				if (key.Length == 16)
				{
					int i = 0;
					int num = 0;
					while (i < key.Length)
					{
						num += (int)key[i++];
					}
					if (num == 4)
					{
						this._publicKey = (byte[])key.Clone();
					}
				}
				else
				{
					this._keyPairArray = key;
				}
			}
			catch
			{
			}
		}

		public byte[] PublicKey
		{
			get
			{
				if (this._publicKey == null)
				{
					RSA rsa = this.GetRSA();
					if (rsa == null)
					{
						throw new ArgumentException("invalid keypair");
					}
					byte[] array = CryptoConvert.ToCapiKeyBlob(rsa, false);
					this._publicKey = new byte[array.Length + 12];
					this._publicKey[0] = 0;
					this._publicKey[1] = 36;
					this._publicKey[2] = 0;
					this._publicKey[3] = 0;
					this._publicKey[4] = 4;
					this._publicKey[5] = 128;
					this._publicKey[6] = 0;
					this._publicKey[7] = 0;
					int num = array.Length;
					this._publicKey[8] = (byte)(num % 256);
					this._publicKey[9] = (byte)(num / 256);
					this._publicKey[10] = 0;
					this._publicKey[11] = 0;
					Buffer.BlockCopy(array, 0, this._publicKey, 12, array.Length);
				}
				return this._publicKey;
			}
		}

		internal StrongName StrongName()
		{
			RSA rsa = this.GetRSA();
			if (rsa != null)
			{
				return new StrongName(rsa);
			}
			if (this._publicKey != null)
			{
				return new StrongName(this._publicKey);
			}
			return null;
		}

		private byte[] _publicKey;

		private string _keyPairContainer;

		private bool _keyPairExported;

		private byte[] _keyPairArray;

		[NonSerialized]
		private RSA _rsa;
	}
}
