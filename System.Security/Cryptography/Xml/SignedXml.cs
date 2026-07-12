using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class SignedXml
	{
		public SignedXml()
		{
			this.Initialize(null);
		}

		public SignedXml(XmlDocument document)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			this.Initialize(document.DocumentElement);
		}

		public SignedXml(XmlElement elem)
		{
			if (elem == null)
			{
				throw new ArgumentNullException("elem");
			}
			this.Initialize(elem);
		}

		private void Initialize(XmlElement element)
		{
			this._containingDocument = ((element == null) ? null : element.OwnerDocument);
			this._context = element;
			this.m_signature = new Signature();
			this.m_signature.SignedXml = this;
			this.m_signature.SignedInfo = new SignedInfo();
			this._signingKey = null;
			this._safeCanonicalizationMethods = new Collection<string>(SignedXml.KnownCanonicalizationMethods);
		}

		public string SigningKeyName
		{
			get
			{
				return this.m_strSigningKeyName;
			}
			set
			{
				this.m_strSigningKeyName = value;
			}
		}

		public XmlResolver Resolver
		{
			set
			{
				this._xmlResolver = value;
				this._bResolverSet = true;
			}
		}

		internal bool ResolverSet
		{
			get
			{
				return this._bResolverSet;
			}
		}

		public Func<SignedXml, bool> SignatureFormatValidator
		{
			get
			{
				return this._signatureFormatValidator;
			}
			set
			{
				this._signatureFormatValidator = value;
			}
		}

		public Collection<string> SafeCanonicalizationMethods
		{
			get
			{
				return this._safeCanonicalizationMethods;
			}
		}

		public AsymmetricAlgorithm SigningKey
		{
			get
			{
				return this._signingKey;
			}
			set
			{
				this._signingKey = value;
			}
		}

		public EncryptedXml EncryptedXml
		{
			get
			{
				if (this._exml == null)
				{
					this._exml = new EncryptedXml(this._containingDocument);
				}
				return this._exml;
			}
			set
			{
				this._exml = value;
			}
		}

		public Signature Signature
		{
			get
			{
				return this.m_signature;
			}
		}

		public SignedInfo SignedInfo
		{
			get
			{
				return this.m_signature.SignedInfo;
			}
		}

		public string SignatureMethod
		{
			get
			{
				return this.m_signature.SignedInfo.SignatureMethod;
			}
		}

		public string SignatureLength
		{
			get
			{
				return this.m_signature.SignedInfo.SignatureLength;
			}
		}

		public byte[] SignatureValue
		{
			get
			{
				return this.m_signature.SignatureValue;
			}
		}

		public KeyInfo KeyInfo
		{
			get
			{
				return this.m_signature.KeyInfo;
			}
			set
			{
				this.m_signature.KeyInfo = value;
			}
		}

		public XmlElement GetXml()
		{
			if (this._containingDocument != null)
			{
				return this.m_signature.GetXml(this._containingDocument);
			}
			return this.m_signature.GetXml();
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.m_signature.LoadXml(value);
			if (this._context == null)
			{
				this._context = value;
			}
			this._bCacheValid = false;
		}

		public void AddReference(Reference reference)
		{
			this.m_signature.SignedInfo.AddReference(reference);
		}

		public void AddObject(DataObject dataObject)
		{
			this.m_signature.AddObject(dataObject);
		}

		public bool CheckSignature()
		{
			AsymmetricAlgorithm asymmetricAlgorithm;
			return this.CheckSignatureReturningKey(out asymmetricAlgorithm);
		}

		public bool CheckSignatureReturningKey(out AsymmetricAlgorithm signingKey)
		{
			SignedXmlDebugLog.LogBeginSignatureVerification(this, this._context);
			signingKey = null;
			bool flag = false;
			if (!this.CheckSignatureFormat())
			{
				return false;
			}
			AsymmetricAlgorithm publicKey;
			do
			{
				publicKey = this.GetPublicKey();
				if (publicKey != null)
				{
					flag = this.CheckSignature(publicKey);
					SignedXmlDebugLog.LogVerificationResult(this, publicKey, flag);
				}
			}
			while (publicKey != null && !flag);
			signingKey = publicKey;
			return flag;
		}

		public bool CheckSignature(AsymmetricAlgorithm key)
		{
			if (!this.CheckSignatureFormat())
			{
				return false;
			}
			if (!this.CheckSignedInfo(key))
			{
				SignedXmlDebugLog.LogVerificationFailure(this, "SignedInfo");
				return false;
			}
			if (!this.CheckDigestedReferences())
			{
				SignedXmlDebugLog.LogVerificationFailure(this, "references");
				return false;
			}
			SignedXmlDebugLog.LogVerificationResult(this, key, true);
			return true;
		}

		public bool CheckSignature(KeyedHashAlgorithm macAlg)
		{
			if (!this.CheckSignatureFormat())
			{
				return false;
			}
			if (!this.CheckSignedInfo(macAlg))
			{
				SignedXmlDebugLog.LogVerificationFailure(this, "SignedInfo");
				return false;
			}
			if (!this.CheckDigestedReferences())
			{
				SignedXmlDebugLog.LogVerificationFailure(this, "references");
				return false;
			}
			SignedXmlDebugLog.LogVerificationResult(this, macAlg, true);
			return true;
		}

		public bool CheckSignature(X509Certificate2 certificate, bool verifySignatureOnly)
		{
			if (!verifySignatureOnly)
			{
				foreach (X509Extension x509Extension in certificate.Extensions)
				{
					if (string.Compare(x509Extension.Oid.Value, "2.5.29.15", StringComparison.OrdinalIgnoreCase) == 0)
					{
						X509KeyUsageExtension x509KeyUsageExtension = new X509KeyUsageExtension();
						x509KeyUsageExtension.CopyFrom(x509Extension);
						SignedXmlDebugLog.LogVerifyKeyUsage(this, certificate, x509KeyUsageExtension);
						if ((x509KeyUsageExtension.KeyUsages & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.None && (x509KeyUsageExtension.KeyUsages & X509KeyUsageFlags.NonRepudiation) <= X509KeyUsageFlags.None)
						{
							SignedXmlDebugLog.LogVerificationFailure(this, "X509 key usage verification");
							return false;
						}
						break;
					}
				}
				X509Chain x509Chain = new X509Chain();
				x509Chain.ChainPolicy.ExtraStore.AddRange(this.BuildBagOfCerts());
				bool flag = x509Chain.Build(certificate);
				SignedXmlDebugLog.LogVerifyX509Chain(this, x509Chain, certificate);
				if (!flag)
				{
					SignedXmlDebugLog.LogVerificationFailure(this, "X509 chain verification");
					return false;
				}
			}
			using (AsymmetricAlgorithm anyPublicKey = Utils.GetAnyPublicKey(certificate))
			{
				if (!this.CheckSignature(anyPublicKey))
				{
					return false;
				}
			}
			SignedXmlDebugLog.LogVerificationResult(this, certificate, true);
			return true;
		}

		public void ComputeSignature()
		{
			SignedXmlDebugLog.LogBeginSignatureComputation(this, this._context);
			this.BuildDigestedReferences();
			AsymmetricAlgorithm signingKey = this.SigningKey;
			if (signingKey == null)
			{
				throw new CryptographicException("Signing key is not loaded.");
			}
			if (this.SignedInfo.SignatureMethod == null)
			{
				if (signingKey is DSA)
				{
					this.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
				}
				else
				{
					if (!(signingKey is RSA))
					{
						throw new CryptographicException("Failed to create signing key.");
					}
					if (this.SignedInfo.SignatureMethod == null)
					{
						this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
					}
				}
			}
			SignatureDescription signatureDescription = CryptoHelpers.CreateFromName<SignatureDescription>(this.SignedInfo.SignatureMethod);
			if (signatureDescription == null)
			{
				throw new CryptographicException("SignatureDescription could not be created for the signature algorithm supplied.");
			}
			HashAlgorithm hashAlgorithm = signatureDescription.CreateDigest();
			if (hashAlgorithm == null)
			{
				throw new CryptographicException("Could not create hash algorithm object.");
			}
			this.GetC14NDigest(hashAlgorithm);
			AsymmetricSignatureFormatter asymmetricSignatureFormatter = signatureDescription.CreateFormatter(signingKey);
			SignedXmlDebugLog.LogSigning(this, signingKey, signatureDescription, hashAlgorithm, asymmetricSignatureFormatter);
			this.m_signature.SignatureValue = asymmetricSignatureFormatter.CreateSignature(hashAlgorithm);
		}

		public void ComputeSignature(KeyedHashAlgorithm macAlg)
		{
			if (macAlg == null)
			{
				throw new ArgumentNullException("macAlg");
			}
			HMAC hmac = macAlg as HMAC;
			if (hmac == null)
			{
				throw new CryptographicException("The key does not fit the SignatureMethod.");
			}
			int num;
			if (this.m_signature.SignedInfo.SignatureLength == null)
			{
				num = hmac.HashSize;
			}
			else
			{
				num = Convert.ToInt32(this.m_signature.SignedInfo.SignatureLength, null);
			}
			if (num < 0 || num > hmac.HashSize)
			{
				throw new CryptographicException("The length of the signature with a MAC should be less than the hash output length.");
			}
			if (num % 8 != 0)
			{
				throw new CryptographicException("The length in bits of the signature with a MAC should be a multiple of 8.");
			}
			this.BuildDigestedReferences();
			string hashName = hmac.HashName;
			if (!(hashName == "SHA1"))
			{
				if (!(hashName == "SHA256"))
				{
					if (!(hashName == "SHA384"))
					{
						if (!(hashName == "SHA512"))
						{
							if (!(hashName == "MD5"))
							{
								if (!(hashName == "RIPEMD160"))
								{
									throw new CryptographicException("The key does not fit the SignatureMethod.");
								}
								this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160";
							}
							else
							{
								this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-md5";
							}
						}
						else
						{
							this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
						}
					}
					else
					{
						this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";
					}
				}
				else
				{
					this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
				}
			}
			else
			{
				this.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
			}
			Array c14NDigest = this.GetC14NDigest(hmac);
			SignedXmlDebugLog.LogSigning(this, hmac);
			this.m_signature.SignatureValue = new byte[num / 8];
			Buffer.BlockCopy(c14NDigest, 0, this.m_signature.SignatureValue, 0, num / 8);
		}

		protected virtual AsymmetricAlgorithm GetPublicKey()
		{
			if (this.KeyInfo == null)
			{
				throw new CryptographicException("A KeyInfo element is required to check the signature.");
			}
			if (this._x509Enum != null)
			{
				AsymmetricAlgorithm nextCertificatePublicKey = this.GetNextCertificatePublicKey();
				if (nextCertificatePublicKey != null)
				{
					return nextCertificatePublicKey;
				}
			}
			if (this._keyInfoEnum == null)
			{
				this._keyInfoEnum = this.KeyInfo.GetEnumerator();
			}
			while (this._keyInfoEnum.MoveNext())
			{
				RSAKeyValue rsakeyValue = this._keyInfoEnum.Current as RSAKeyValue;
				if (rsakeyValue != null)
				{
					return rsakeyValue.Key;
				}
				DSAKeyValue dsakeyValue = this._keyInfoEnum.Current as DSAKeyValue;
				if (dsakeyValue != null)
				{
					return dsakeyValue.Key;
				}
				KeyInfoX509Data keyInfoX509Data = this._keyInfoEnum.Current as KeyInfoX509Data;
				if (keyInfoX509Data != null)
				{
					this._x509Collection = Utils.BuildBagOfCerts(keyInfoX509Data, CertUsageType.Verification);
					if (this._x509Collection.Count > 0)
					{
						this._x509Enum = this._x509Collection.GetEnumerator();
						AsymmetricAlgorithm nextCertificatePublicKey2 = this.GetNextCertificatePublicKey();
						if (nextCertificatePublicKey2 != null)
						{
							return nextCertificatePublicKey2;
						}
					}
				}
			}
			return null;
		}

		private X509Certificate2Collection BuildBagOfCerts()
		{
			X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
			if (this.KeyInfo != null)
			{
				foreach (object obj in this.KeyInfo)
				{
					KeyInfoX509Data keyInfoX509Data = ((KeyInfoClause)obj) as KeyInfoX509Data;
					if (keyInfoX509Data != null)
					{
						x509Certificate2Collection.AddRange(Utils.BuildBagOfCerts(keyInfoX509Data, CertUsageType.Verification));
					}
				}
			}
			return x509Certificate2Collection;
		}

		private AsymmetricAlgorithm GetNextCertificatePublicKey()
		{
			while (this._x509Enum.MoveNext())
			{
				X509Certificate2 x509Certificate = (X509Certificate2)this._x509Enum.Current;
				if (x509Certificate != null)
				{
					return Utils.GetAnyPublicKey(x509Certificate);
				}
			}
			return null;
		}

		public virtual XmlElement GetIdElement(XmlDocument document, string idValue)
		{
			return SignedXml.DefaultGetIdElement(document, idValue);
		}

		internal static XmlElement DefaultGetIdElement(XmlDocument document, string idValue)
		{
			if (document == null)
			{
				return null;
			}
			try
			{
				XmlConvert.VerifyNCName(idValue);
			}
			catch (XmlException)
			{
				return null;
			}
			XmlElement xmlElement = document.GetElementById(idValue);
			if (xmlElement != null)
			{
				XmlDocument xmlDocument = (XmlDocument)document.CloneNode(true);
				XmlElement elementById = xmlDocument.GetElementById(idValue);
				if (elementById != null)
				{
					elementById.Attributes.RemoveAll();
					if (xmlDocument.GetElementById(idValue) != null)
					{
						throw new CryptographicException("Malformed reference element.");
					}
				}
				return xmlElement;
			}
			xmlElement = SignedXml.GetSingleReferenceTarget(document, "Id", idValue);
			if (xmlElement != null)
			{
				return xmlElement;
			}
			xmlElement = SignedXml.GetSingleReferenceTarget(document, "id", idValue);
			if (xmlElement != null)
			{
				return xmlElement;
			}
			return SignedXml.GetSingleReferenceTarget(document, "ID", idValue);
		}

		private static bool DefaultSignatureFormatValidator(SignedXml signedXml)
		{
			return !signedXml.DoesSignatureUseTruncatedHmac() && signedXml.DoesSignatureUseSafeCanonicalizationMethod();
		}

		private bool DoesSignatureUseTruncatedHmac()
		{
			if (this.SignedInfo.SignatureLength == null)
			{
				return false;
			}
			HMAC hmac = CryptoHelpers.CreateFromName<HMAC>(this.SignatureMethod);
			if (hmac == null)
			{
				return false;
			}
			int num = 0;
			return !int.TryParse(this.SignedInfo.SignatureLength, out num) || num != hmac.HashSize;
		}

		private bool DoesSignatureUseSafeCanonicalizationMethod()
		{
			using (IEnumerator<string> enumerator = this.SafeCanonicalizationMethods.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (string.Equals(enumerator.Current, this.SignedInfo.CanonicalizationMethod, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			SignedXmlDebugLog.LogUnsafeCanonicalizationMethod(this, this.SignedInfo.CanonicalizationMethod, this.SafeCanonicalizationMethods);
			return false;
		}

		private bool ReferenceUsesSafeTransformMethods(Reference reference)
		{
			TransformChain transformChain = reference.TransformChain;
			int count = transformChain.Count;
			for (int i = 0; i < count; i++)
			{
				Transform transform = transformChain[i];
				if (!this.IsSafeTransform(transform.Algorithm))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsSafeTransform(string transformAlgorithm)
		{
			using (IEnumerator<string> enumerator = this.SafeCanonicalizationMethods.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (string.Equals(enumerator.Current, transformAlgorithm, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			using (IEnumerator<string> enumerator = SignedXml.DefaultSafeTransformMethods.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (string.Equals(enumerator.Current, transformAlgorithm, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			SignedXmlDebugLog.LogUnsafeTransformMethod(this, transformAlgorithm, this.SafeCanonicalizationMethods, SignedXml.DefaultSafeTransformMethods);
			return false;
		}

		private static IList<string> KnownCanonicalizationMethods
		{
			get
			{
				if (SignedXml.s_knownCanonicalizationMethods == null)
				{
					SignedXml.s_knownCanonicalizationMethods = new List<string> { "http://www.w3.org/TR/2001/REC-xml-c14n-20010315", "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments", "http://www.w3.org/2001/10/xml-exc-c14n#", "http://www.w3.org/2001/10/xml-exc-c14n#WithComments" };
				}
				return SignedXml.s_knownCanonicalizationMethods;
			}
		}

		private static IList<string> DefaultSafeTransformMethods
		{
			get
			{
				if (SignedXml.s_defaultSafeTransformMethods == null)
				{
					SignedXml.s_defaultSafeTransformMethods = new List<string> { "http://www.w3.org/2000/09/xmldsig#enveloped-signature", "http://www.w3.org/2000/09/xmldsig#base64", "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform", "http://www.w3.org/2002/07/decrypt#XML" };
				}
				return SignedXml.s_defaultSafeTransformMethods;
			}
		}

		private byte[] GetC14NDigest(HashAlgorithm hash)
		{
			bool flag = hash is KeyedHashAlgorithm;
			if (flag || !this._bCacheValid || !this.SignedInfo.CacheValid)
			{
				string text = ((this._containingDocument == null) ? null : this._containingDocument.BaseURI);
				XmlResolver xmlResolver = (this._bResolverSet ? this._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
				XmlDocument xmlDocument = Utils.PreProcessElementInput(this.SignedInfo.GetXml(), xmlResolver, text);
				CanonicalXmlNodeList canonicalXmlNodeList = ((this._context == null) ? null : Utils.GetPropagatedAttributes(this._context));
				SignedXmlDebugLog.LogNamespacePropagation(this, canonicalXmlNodeList);
				Utils.AddNamespaces(xmlDocument.DocumentElement, canonicalXmlNodeList);
				Transform canonicalizationMethodObject = this.SignedInfo.CanonicalizationMethodObject;
				canonicalizationMethodObject.Resolver = xmlResolver;
				canonicalizationMethodObject.BaseURI = text;
				SignedXmlDebugLog.LogBeginCanonicalization(this, canonicalizationMethodObject);
				canonicalizationMethodObject.LoadInput(xmlDocument);
				SignedXmlDebugLog.LogCanonicalizedOutput(this, canonicalizationMethodObject);
				this._digestedSignedInfo = canonicalizationMethodObject.GetDigestedOutput(hash);
				this._bCacheValid = !flag;
			}
			return this._digestedSignedInfo;
		}

		private int GetReferenceLevel(int index, ArrayList references)
		{
			if (this._refProcessed[index])
			{
				return this._refLevelCache[index];
			}
			this._refProcessed[index] = true;
			Reference reference = (Reference)references[index];
			if (reference.Uri == null || reference.Uri.Length == 0 || (reference.Uri.Length > 0 && reference.Uri[0] != '#'))
			{
				this._refLevelCache[index] = 0;
				return 0;
			}
			if (reference.Uri.Length <= 0 || reference.Uri[0] != '#')
			{
				throw new CryptographicException("Malformed reference element.");
			}
			string text = Utils.ExtractIdFromLocalUri(reference.Uri);
			if (text == "xpointer(/)")
			{
				this._refLevelCache[index] = 0;
				return 0;
			}
			for (int i = 0; i < references.Count; i++)
			{
				if (((Reference)references[i]).Id == text)
				{
					this._refLevelCache[index] = this.GetReferenceLevel(i, references) + 1;
					return this._refLevelCache[index];
				}
			}
			this._refLevelCache[index] = 0;
			return 0;
		}

		private void BuildDigestedReferences()
		{
			ArrayList references = this.SignedInfo.References;
			this._refProcessed = new bool[references.Count];
			this._refLevelCache = new int[references.Count];
			SignedXml.ReferenceLevelSortOrder referenceLevelSortOrder = new SignedXml.ReferenceLevelSortOrder();
			referenceLevelSortOrder.References = references;
			ArrayList arrayList = new ArrayList();
			foreach (object obj in references)
			{
				Reference reference = (Reference)obj;
				arrayList.Add(reference);
			}
			arrayList.Sort(referenceLevelSortOrder);
			CanonicalXmlNodeList canonicalXmlNodeList = new CanonicalXmlNodeList();
			foreach (object obj2 in this.m_signature.ObjectList)
			{
				DataObject dataObject = (DataObject)obj2;
				canonicalXmlNodeList.Add(dataObject.GetXml());
			}
			foreach (object obj3 in arrayList)
			{
				Reference reference2 = (Reference)obj3;
				if (reference2.DigestMethod == null)
				{
					reference2.DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
				}
				SignedXmlDebugLog.LogSigningReference(this, reference2);
				reference2.UpdateHashValue(this._containingDocument, canonicalXmlNodeList);
				if (reference2.Id != null)
				{
					canonicalXmlNodeList.Add(reference2.GetXml());
				}
			}
		}

		private bool CheckDigestedReferences()
		{
			ArrayList references = this.m_signature.SignedInfo.References;
			int i = 0;
			while (i < references.Count)
			{
				Reference reference = (Reference)references[i];
				if (!this.ReferenceUsesSafeTransformMethods(reference))
				{
					return false;
				}
				SignedXmlDebugLog.LogVerifyReference(this, reference);
				byte[] array = null;
				try
				{
					array = reference.CalculateHashValue(this._containingDocument, this.m_signature.ReferencedItems);
				}
				catch (CryptoSignedXmlRecursionException)
				{
					SignedXmlDebugLog.LogSignedXmlRecursionLimit(this, reference);
					return false;
				}
				SignedXmlDebugLog.LogVerifyReferenceHash(this, reference, array, reference.DigestValue);
				if (!SignedXml.CryptographicEquals(array, reference.DigestValue))
				{
					return false;
				}
				i++;
				continue;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static bool CryptographicEquals(byte[] a, byte[] b)
		{
			int num = 0;
			if (a.Length != b.Length)
			{
				return false;
			}
			int num2 = a.Length;
			for (int i = 0; i < num2; i++)
			{
				num |= (int)(a[i] - b[i]);
			}
			return num == 0;
		}

		private bool CheckSignatureFormat()
		{
			if (this._signatureFormatValidator == null)
			{
				return true;
			}
			SignedXmlDebugLog.LogBeginCheckSignatureFormat(this, this._signatureFormatValidator);
			bool flag = this._signatureFormatValidator(this);
			SignedXmlDebugLog.LogFormatValidationResult(this, flag);
			return flag;
		}

		private bool CheckSignedInfo(AsymmetricAlgorithm key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			SignedXmlDebugLog.LogBeginCheckSignedInfo(this, this.m_signature.SignedInfo);
			SignatureDescription signatureDescription = CryptoHelpers.CreateFromName<SignatureDescription>(this.SignatureMethod);
			if (signatureDescription == null)
			{
				throw new CryptographicException("SignatureDescription could not be created for the signature algorithm supplied.");
			}
			Type type = Type.GetType(signatureDescription.KeyAlgorithm);
			if (!SignedXml.IsKeyTheCorrectAlgorithm(key, type))
			{
				return false;
			}
			HashAlgorithm hashAlgorithm = signatureDescription.CreateDigest();
			if (hashAlgorithm == null)
			{
				throw new CryptographicException("Could not create hash algorithm object.");
			}
			byte[] c14NDigest = this.GetC14NDigest(hashAlgorithm);
			AsymmetricSignatureDeformatter asymmetricSignatureDeformatter = signatureDescription.CreateDeformatter(key);
			SignedXmlDebugLog.LogVerifySignedInfo(this, key, signatureDescription, hashAlgorithm, asymmetricSignatureDeformatter, c14NDigest, this.m_signature.SignatureValue);
			return asymmetricSignatureDeformatter.VerifySignature(c14NDigest, this.m_signature.SignatureValue);
		}

		private bool CheckSignedInfo(KeyedHashAlgorithm macAlg)
		{
			if (macAlg == null)
			{
				throw new ArgumentNullException("macAlg");
			}
			SignedXmlDebugLog.LogBeginCheckSignedInfo(this, this.m_signature.SignedInfo);
			int num;
			if (this.m_signature.SignedInfo.SignatureLength == null)
			{
				num = macAlg.HashSize;
			}
			else
			{
				num = Convert.ToInt32(this.m_signature.SignedInfo.SignatureLength, null);
			}
			if (num < 0 || num > macAlg.HashSize)
			{
				throw new CryptographicException("The length of the signature with a MAC should be less than the hash output length.");
			}
			if (num % 8 != 0)
			{
				throw new CryptographicException("The length in bits of the signature with a MAC should be a multiple of 8.");
			}
			if (this.m_signature.SignatureValue == null)
			{
				throw new CryptographicException("Signature requires a SignatureValue.");
			}
			if (this.m_signature.SignatureValue.Length != num / 8)
			{
				throw new CryptographicException("The length of the signature with a MAC should be less than the hash output length.");
			}
			byte[] c14NDigest = this.GetC14NDigest(macAlg);
			SignedXmlDebugLog.LogVerifySignedInfo(this, macAlg, c14NDigest, this.m_signature.SignatureValue);
			for (int i = 0; i < this.m_signature.SignatureValue.Length; i++)
			{
				if (this.m_signature.SignatureValue[i] != c14NDigest[i])
				{
					return false;
				}
			}
			return true;
		}

		private static XmlElement GetSingleReferenceTarget(XmlDocument document, string idAttributeName, string idValue)
		{
			string text = string.Concat(new string[] { "//*[@", idAttributeName, "=\"", idValue, "\"]" });
			XmlNodeList xmlNodeList = document.SelectNodes(text);
			if (xmlNodeList == null || xmlNodeList.Count == 0)
			{
				return null;
			}
			if (xmlNodeList.Count == 1)
			{
				return xmlNodeList[0] as XmlElement;
			}
			throw new CryptographicException("Malformed reference element.");
		}

		private static bool IsKeyTheCorrectAlgorithm(AsymmetricAlgorithm key, Type expectedType)
		{
			Type type = key.GetType();
			if (type == expectedType)
			{
				return true;
			}
			if (expectedType.IsSubclassOf(type))
			{
				return true;
			}
			while (expectedType != null && expectedType.BaseType != typeof(AsymmetricAlgorithm))
			{
				expectedType = expectedType.BaseType;
			}
			return !(expectedType == null) && type.IsSubclassOf(expectedType);
		}

		protected Signature m_signature;

		protected string m_strSigningKeyName;

		private AsymmetricAlgorithm _signingKey;

		private XmlDocument _containingDocument;

		private IEnumerator _keyInfoEnum;

		private X509Certificate2Collection _x509Collection;

		private IEnumerator _x509Enum;

		private bool[] _refProcessed;

		private int[] _refLevelCache;

		internal XmlResolver _xmlResolver;

		internal XmlElement _context;

		private bool _bResolverSet;

		private Func<SignedXml, bool> _signatureFormatValidator = new Func<SignedXml, bool>(SignedXml.DefaultSignatureFormatValidator);

		private Collection<string> _safeCanonicalizationMethods;

		private static IList<string> s_knownCanonicalizationMethods;

		private static IList<string> s_defaultSafeTransformMethods;

		private const string XmlDsigMoreHMACMD5Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-md5";

		private const string XmlDsigMoreHMACSHA256Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";

		private const string XmlDsigMoreHMACSHA384Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";

		private const string XmlDsigMoreHMACSHA512Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";

		private const string XmlDsigMoreHMACRIPEMD160Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160";

		private EncryptedXml _exml;

		public const string XmlDsigNamespaceUrl = "http://www.w3.org/2000/09/xmldsig#";

		public const string XmlDsigMinimalCanonicalizationUrl = "http://www.w3.org/2000/09/xmldsig#minimal";

		public const string XmlDsigCanonicalizationUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

		public const string XmlDsigCanonicalizationWithCommentsUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";

		public const string XmlDsigSHA1Url = "http://www.w3.org/2000/09/xmldsig#sha1";

		public const string XmlDsigDSAUrl = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";

		public const string XmlDsigRSASHA1Url = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

		public const string XmlDsigHMACSHA1Url = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";

		public const string XmlDsigSHA256Url = "http://www.w3.org/2001/04/xmlenc#sha256";

		public const string XmlDsigRSASHA256Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

		public const string XmlDsigSHA384Url = "http://www.w3.org/2001/04/xmldsig-more#sha384";

		public const string XmlDsigRSASHA384Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";

		public const string XmlDsigSHA512Url = "http://www.w3.org/2001/04/xmlenc#sha512";

		public const string XmlDsigRSASHA512Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";

		public const string XmlDsigC14NTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

		public const string XmlDsigC14NWithCommentsTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";

		public const string XmlDsigExcC14NTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#";

		public const string XmlDsigExcC14NWithCommentsTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";

		public const string XmlDsigBase64TransformUrl = "http://www.w3.org/2000/09/xmldsig#base64";

		public const string XmlDsigXPathTransformUrl = "http://www.w3.org/TR/1999/REC-xpath-19991116";

		public const string XmlDsigXsltTransformUrl = "http://www.w3.org/TR/1999/REC-xslt-19991116";

		public const string XmlDsigEnvelopedSignatureTransformUrl = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";

		public const string XmlDecryptionTransformUrl = "http://www.w3.org/2002/07/decrypt#XML";

		public const string XmlLicenseTransformUrl = "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform";

		private bool _bCacheValid;

		private byte[] _digestedSignedInfo;

		private class ReferenceLevelSortOrder : IComparer
		{
			public ArrayList References
			{
				get
				{
					return this._references;
				}
				set
				{
					this._references = value;
				}
			}

			public int Compare(object a, object b)
			{
				Reference reference = a as Reference;
				Reference reference2 = b as Reference;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				foreach (object obj in this.References)
				{
					Reference reference3 = (Reference)obj;
					if (reference3 == reference)
					{
						num = num3;
					}
					if (reference3 == reference2)
					{
						num2 = num3;
					}
					num3++;
				}
				int referenceLevel = reference.SignedXml.GetReferenceLevel(num, this.References);
				int referenceLevel2 = reference2.SignedXml.GetReferenceLevel(num2, this.References);
				return referenceLevel.CompareTo(referenceLevel2);
			}

			private ArrayList _references;
		}
	}
}
