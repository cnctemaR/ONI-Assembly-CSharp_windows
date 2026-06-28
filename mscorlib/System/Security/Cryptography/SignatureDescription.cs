using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class SignatureDescription
	{
		public SignatureDescription()
		{
		}

		public SignatureDescription(SecurityElement el)
		{
			if (el == null)
			{
				throw new ArgumentNullException("el");
			}
			SecurityElement securityElement = el.SearchForChildByTag("Deformatter");
			this._DeformatterAlgorithm = ((securityElement != null) ? securityElement.Text : null);
			securityElement = el.SearchForChildByTag("Digest");
			this._DigestAlgorithm = ((securityElement != null) ? securityElement.Text : null);
			securityElement = el.SearchForChildByTag("Formatter");
			this._FormatterAlgorithm = ((securityElement != null) ? securityElement.Text : null);
			securityElement = el.SearchForChildByTag("Key");
			this._KeyAlgorithm = ((securityElement != null) ? securityElement.Text : null);
		}

		public string DeformatterAlgorithm
		{
			get
			{
				return this._DeformatterAlgorithm;
			}
			set
			{
				this._DeformatterAlgorithm = value;
			}
		}

		public string DigestAlgorithm
		{
			get
			{
				return this._DigestAlgorithm;
			}
			set
			{
				this._DigestAlgorithm = value;
			}
		}

		public string FormatterAlgorithm
		{
			get
			{
				return this._FormatterAlgorithm;
			}
			set
			{
				this._FormatterAlgorithm = value;
			}
		}

		public string KeyAlgorithm
		{
			get
			{
				return this._KeyAlgorithm;
			}
			set
			{
				this._KeyAlgorithm = value;
			}
		}

		public virtual AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
		{
			if (this._DeformatterAlgorithm == null)
			{
				throw new ArgumentNullException("DeformatterAlgorithm");
			}
			AsymmetricSignatureDeformatter asymmetricSignatureDeformatter = (AsymmetricSignatureDeformatter)CryptoConfig.CreateFromName(this._DeformatterAlgorithm);
			if (this._KeyAlgorithm == null)
			{
				throw new NullReferenceException("KeyAlgorithm");
			}
			asymmetricSignatureDeformatter.SetKey(key);
			return asymmetricSignatureDeformatter;
		}

		public virtual HashAlgorithm CreateDigest()
		{
			if (this._DigestAlgorithm == null)
			{
				throw new ArgumentNullException("DigestAlgorithm");
			}
			return (HashAlgorithm)CryptoConfig.CreateFromName(this._DigestAlgorithm);
		}

		public virtual AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
		{
			if (this._FormatterAlgorithm == null)
			{
				throw new ArgumentNullException("FormatterAlgorithm");
			}
			AsymmetricSignatureFormatter asymmetricSignatureFormatter = (AsymmetricSignatureFormatter)CryptoConfig.CreateFromName(this._FormatterAlgorithm);
			if (this._KeyAlgorithm == null)
			{
				throw new NullReferenceException("KeyAlgorithm");
			}
			asymmetricSignatureFormatter.SetKey(key);
			return asymmetricSignatureFormatter;
		}

		private string _DeformatterAlgorithm;

		private string _DigestAlgorithm;

		private string _FormatterAlgorithm;

		private string _KeyAlgorithm;
	}
}
