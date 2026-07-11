using System;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class RC2CryptoServiceProvider : RC2
	{
		public override int EffectiveKeySize
		{
			get
			{
				return base.EffectiveKeySize;
			}
			set
			{
				if (value != this.KeySizeValue)
				{
					throw new CryptographicUnexpectedOperationException(Locale.GetText("Effective key size must match key size for compatibility"));
				}
				base.EffectiveKeySize = value;
			}
		}

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new RC2Transform(this, false, rgbKey, rgbIV);
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new RC2Transform(this, true, rgbKey, rgbIV);
		}

		public override void GenerateIV()
		{
			this.IVValue = KeyBuilder.IV(this.BlockSizeValue >> 3);
		}

		public override void GenerateKey()
		{
			this.KeyValue = KeyBuilder.Key(this.KeySizeValue >> 3);
		}

		[MonoTODO("Use salt in algorithm")]
		[ComVisible(false)]
		public bool UseSalt
		{
			get
			{
				return this._useSalt;
			}
			set
			{
				this._useSalt = value;
			}
		}

		private bool _useSalt;
	}
}
