using System;
using System.Security.Permissions;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	public sealed class AesManaged : Aes
	{
		public override void GenerateIV()
		{
			this.IVValue = KeyBuilder.IV(this.BlockSizeValue >> 3);
		}

		public override void GenerateKey()
		{
			this.KeyValue = KeyBuilder.Key(this.KeySizeValue >> 3);
		}

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new AesTransform(this, false, rgbKey, rgbIV);
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new AesTransform(this, true, rgbKey, rgbIV);
		}

		public override byte[] IV
		{
			get
			{
				return base.IV;
			}
			set
			{
				base.IV = value;
			}
		}

		public override byte[] Key
		{
			get
			{
				return base.Key;
			}
			set
			{
				base.Key = value;
			}
		}

		public override int KeySize
		{
			get
			{
				return base.KeySize;
			}
			set
			{
				base.KeySize = value;
			}
		}

		public override ICryptoTransform CreateDecryptor()
		{
			return this.CreateDecryptor(this.Key, this.IV);
		}

		public override ICryptoTransform CreateEncryptor()
		{
			return this.CreateEncryptor(this.Key, this.IV);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
