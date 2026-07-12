using System;

namespace System.Security.Cryptography.Xml
{
	internal class DSASignatureDescription : SignatureDescription
	{
		public DSASignatureDescription()
		{
			base.KeyAlgorithm = typeof(DSA).AssemblyQualifiedName;
			base.FormatterAlgorithm = typeof(DSASignatureFormatter).AssemblyQualifiedName;
			base.DeformatterAlgorithm = typeof(DSASignatureDeformatter).AssemblyQualifiedName;
			base.DigestAlgorithm = "SHA1";
		}

		public sealed override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
		{
			AsymmetricSignatureDeformatter asymmetricSignatureDeformatter = (AsymmetricSignatureDeformatter)CryptoConfig.CreateFromName(base.DeformatterAlgorithm);
			asymmetricSignatureDeformatter.SetKey(key);
			asymmetricSignatureDeformatter.SetHashAlgorithm("SHA1");
			return asymmetricSignatureDeformatter;
		}

		public sealed override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
		{
			AsymmetricSignatureFormatter asymmetricSignatureFormatter = (AsymmetricSignatureFormatter)CryptoConfig.CreateFromName(base.FormatterAlgorithm);
			asymmetricSignatureFormatter.SetKey(key);
			asymmetricSignatureFormatter.SetHashAlgorithm("SHA1");
			return asymmetricSignatureFormatter;
		}

		public sealed override HashAlgorithm CreateDigest()
		{
			return SHA1.Create();
		}

		private const string HashAlgorithm = "SHA1";
	}
}
