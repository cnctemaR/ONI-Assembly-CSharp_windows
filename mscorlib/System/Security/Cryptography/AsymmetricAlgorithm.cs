using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class AsymmetricAlgorithm : IDisposable
	{
		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public abstract string KeyExchangeAlgorithm { get; }

		public virtual int KeySize
		{
			get
			{
				return this.KeySizeValue;
			}
			set
			{
				if (!KeySizes.IsLegalKeySize(this.LegalKeySizesValue, value))
				{
					throw new CryptographicException(Locale.GetText("Key size not supported by algorithm."));
				}
				this.KeySizeValue = value;
			}
		}

		public virtual KeySizes[] LegalKeySizes
		{
			get
			{
				return this.LegalKeySizesValue;
			}
		}

		public abstract string SignatureAlgorithm { get; }

		public void Clear()
		{
			this.Dispose(false);
		}

		protected abstract void Dispose(bool disposing);

		public abstract void FromXmlString(string xmlString);

		public abstract string ToXmlString(bool includePrivateParameters);

		public static AsymmetricAlgorithm Create()
		{
			return AsymmetricAlgorithm.Create("System.Security.Cryptography.AsymmetricAlgorithm");
		}

		public static AsymmetricAlgorithm Create(string algName)
		{
			return (AsymmetricAlgorithm)CryptoConfig.CreateFromName(algName);
		}

		internal static byte[] GetNamedParam(string xml, string param)
		{
			string text = "<" + param + ">";
			int num = xml.IndexOf(text);
			if (num == -1)
			{
				return null;
			}
			string text2 = "</" + param + ">";
			int num2 = xml.IndexOf(text2);
			if (num2 == -1 || num2 <= num)
			{
				return null;
			}
			num += text.Length;
			string text3 = xml.Substring(num, num2 - num);
			return Convert.FromBase64String(text3);
		}

		protected int KeySizeValue;

		protected KeySizes[] LegalKeySizesValue;
	}
}
