using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class KeyedHashAlgorithm : HashAlgorithm
	{
		~KeyedHashAlgorithm()
		{
			this.Dispose(false);
		}

		public virtual byte[] Key
		{
			get
			{
				return (byte[])this.KeyValue.Clone();
			}
			set
			{
				if (this.State != 0)
				{
					throw new CryptographicException(Locale.GetText("Key can't be changed at this state."));
				}
				this.ZeroizeKey();
				this.KeyValue = (byte[])value.Clone();
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.ZeroizeKey();
			base.Dispose(disposing);
		}

		private void ZeroizeKey()
		{
			if (this.KeyValue != null)
			{
				Array.Clear(this.KeyValue, 0, this.KeyValue.Length);
			}
		}

		public new static KeyedHashAlgorithm Create()
		{
			return KeyedHashAlgorithm.Create("System.Security.Cryptography.KeyedHashAlgorithm");
		}

		public new static KeyedHashAlgorithm Create(string algName)
		{
			return (KeyedHashAlgorithm)CryptoConfig.CreateFromName(algName);
		}

		protected byte[] KeyValue;
	}
}
