using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class SignedXml
	{
		public SignedXml()
		{
			this.m_signature = new Signature();
			this.m_signature.SignedInfo = new SignedInfo();
			this.hashes = new Hashtable(2);
		}

		public SignedXml(XmlDocument document)
			: this()
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			this.envdoc = document;
		}

		public SignedXml(XmlElement elem)
			: this()
		{
			if (elem == null)
			{
				throw new ArgumentNullException("elem");
			}
			this.envdoc = new XmlDocument();
			this.envdoc.LoadXml(elem.OuterXml);
		}

		[ComVisible(false)]
		public EncryptedXml EncryptedXml
		{
			get
			{
				return this.encryptedXml;
			}
			set
			{
				this.encryptedXml = value;
			}
		}

		public KeyInfo KeyInfo
		{
			get
			{
				if (this.m_signature.KeyInfo == null)
				{
					this.m_signature.KeyInfo = new KeyInfo();
				}
				return this.m_signature.KeyInfo;
			}
			set
			{
				this.m_signature.KeyInfo = value;
			}
		}

		public Signature Signature
		{
			get
			{
				return this.m_signature;
			}
		}

		public string SignatureLength
		{
			get
			{
				return this.m_signature.SignedInfo.SignatureLength;
			}
		}

		public string SignatureMethod
		{
			get
			{
				return this.m_signature.SignedInfo.SignatureMethod;
			}
		}

		public byte[] SignatureValue
		{
			get
			{
				return this.m_signature.SignatureValue;
			}
		}

		public SignedInfo SignedInfo
		{
			get
			{
				return this.m_signature.SignedInfo;
			}
		}

		public AsymmetricAlgorithm SigningKey
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
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

		public void AddObject(DataObject dataObject)
		{
			this.m_signature.AddObject(dataObject);
		}

		public void AddReference(Reference reference)
		{
			if (reference == null)
			{
				throw new ArgumentNullException("reference");
			}
			this.m_signature.SignedInfo.AddReference(reference);
		}

		private Stream ApplyTransform(Transform t, XmlDocument input)
		{
			if (t is XmlDsigXPathTransform || t is XmlDsigEnvelopedSignatureTransform || t is XmlDecryptionTransform)
			{
				input = (XmlDocument)input.Clone();
			}
			t.LoadInput(input);
			if (t is XmlDsigEnvelopedSignatureTransform)
			{
				return this.CanonicalizeOutput(t.GetOutput());
			}
			object output = t.GetOutput();
			if (output is Stream)
			{
				return (Stream)output;
			}
			if (output is XmlDocument)
			{
				MemoryStream memoryStream = new MemoryStream();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
				((XmlDocument)output).WriteTo(xmlTextWriter);
				xmlTextWriter.Flush();
				memoryStream.Position = 0L;
				return memoryStream;
			}
			if (output == null)
			{
				throw new NotImplementedException("This should not occur. Transform is " + t + ".");
			}
			return this.CanonicalizeOutput(output);
		}

		private Stream CanonicalizeOutput(object obj)
		{
			Transform c14NMethod = this.GetC14NMethod();
			c14NMethod.LoadInput(obj);
			return (Stream)c14NMethod.GetOutput();
		}

		private XmlDocument GetManifest(Reference r)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;
			if (r.Uri[0] == '#')
			{
				if (this.signatureElement != null)
				{
					XmlElement idElement = this.GetIdElement(this.signatureElement.OwnerDocument, r.Uri.Substring(1));
					if (idElement == null)
					{
						throw new CryptographicException("Manifest targeted by Reference was not found: " + r.Uri.Substring(1));
					}
					xmlDocument.LoadXml(idElement.OuterXml);
					this.FixupNamespaceNodes(idElement, xmlDocument.DocumentElement, false);
				}
			}
			else if (this.xmlResolver != null)
			{
				Stream stream = (Stream)this.xmlResolver.GetEntity(new Uri(r.Uri), null, typeof(Stream));
				xmlDocument.Load(stream);
			}
			if (xmlDocument.FirstChild != null)
			{
				if (this.manifests == null)
				{
					this.manifests = new ArrayList();
				}
				this.manifests.Add(xmlDocument);
				return xmlDocument;
			}
			return null;
		}

		private void FixupNamespaceNodes(XmlElement src, XmlElement dst, bool ignoreDefault)
		{
			foreach (object obj in src.SelectNodes("namespace::*"))
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (!(xmlAttribute.LocalName == "xml"))
				{
					if (!ignoreDefault || !(xmlAttribute.LocalName == "xmlns"))
					{
						dst.SetAttributeNode(dst.OwnerDocument.ImportNode(xmlAttribute, true) as XmlAttribute);
					}
				}
			}
		}

		private byte[] GetReferenceHash(Reference r, bool check_hmac)
		{
			Stream stream = null;
			XmlDocument xmlDocument = null;
			if (r.Uri == string.Empty)
			{
				xmlDocument = this.envdoc;
			}
			else if (r.Type == "http://www.w3.org/2000/09/xmldsig#Manifest")
			{
				xmlDocument = this.GetManifest(r);
			}
			else
			{
				xmlDocument = new XmlDocument();
				xmlDocument.PreserveWhitespace = true;
				string text = null;
				if (r.Uri.StartsWith("#xpointer"))
				{
					string text2 = string.Join(string.Empty, r.Uri.Substring(9).Split(SignedXml.whitespaceChars));
					if (text2.Length < 2 || text2[0] != '(' || text2[text2.Length - 1] != ')')
					{
						text2 = string.Empty;
					}
					else
					{
						text2 = text2.Substring(1, text2.Length - 2);
					}
					if (text2 == "/")
					{
						xmlDocument = this.envdoc;
					}
					else if (text2.Length > 6 && text2.StartsWith("id(") && text2[text2.Length - 1] == ')')
					{
						text = text2.Substring(4, text2.Length - 6);
					}
				}
				else if (r.Uri[0] == '#')
				{
					text = r.Uri.Substring(1);
				}
				else if (this.xmlResolver != null)
				{
					try
					{
						Uri uri = new Uri(r.Uri);
						stream = (Stream)this.xmlResolver.GetEntity(uri, null, typeof(Stream));
					}
					catch
					{
						stream = File.OpenRead(r.Uri);
					}
				}
				if (text != null)
				{
					XmlElement xmlElement = null;
					foreach (object obj in this.m_signature.ObjectList)
					{
						DataObject dataObject = (DataObject)obj;
						if (dataObject.Id == text)
						{
							xmlElement = dataObject.GetXml();
							xmlElement.SetAttribute("xmlns", "http://www.w3.org/2000/09/xmldsig#");
							xmlDocument.LoadXml(xmlElement.OuterXml);
							foreach (object obj2 in xmlElement.ChildNodes)
							{
								XmlNode xmlNode = (XmlNode)obj2;
								if (xmlNode.NodeType == XmlNodeType.Element)
								{
									this.FixupNamespaceNodes(xmlNode as XmlElement, xmlDocument.DocumentElement, true);
								}
							}
							break;
						}
					}
					if (xmlElement == null && this.envdoc != null)
					{
						xmlElement = this.GetIdElement(this.envdoc, text);
						if (xmlElement != null)
						{
							xmlDocument.LoadXml(xmlElement.OuterXml);
						}
					}
					if (xmlElement == null)
					{
						throw new CryptographicException(string.Format("Malformed reference object: {0}", text));
					}
				}
			}
			if (r.TransformChain.Count > 0)
			{
				foreach (object obj3 in r.TransformChain)
				{
					Transform transform = (Transform)obj3;
					if (stream == null)
					{
						stream = this.ApplyTransform(transform, xmlDocument);
					}
					else
					{
						transform.LoadInput(stream);
						object output = transform.GetOutput();
						if (output is Stream)
						{
							stream = (Stream)output;
						}
						else
						{
							stream = this.CanonicalizeOutput(output);
						}
					}
				}
			}
			else if (stream == null)
			{
				if (r.Uri[0] != '#')
				{
					stream = new MemoryStream();
					xmlDocument.Save(stream);
				}
				else
				{
					stream = this.ApplyTransform(new XmlDsigC14NTransform(), xmlDocument);
				}
			}
			HashAlgorithm hash = this.GetHash(r.DigestMethod, check_hmac);
			return (hash != null) ? hash.ComputeHash(stream) : null;
		}

		private void DigestReferences()
		{
			foreach (object obj in this.m_signature.SignedInfo.References)
			{
				Reference reference = (Reference)obj;
				if (reference.DigestMethod == null)
				{
					reference.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
				}
				reference.DigestValue = this.GetReferenceHash(reference, false);
			}
		}

		private Transform GetC14NMethod()
		{
			Transform transform = (Transform)CryptoConfig.CreateFromName(this.m_signature.SignedInfo.CanonicalizationMethod);
			if (transform == null)
			{
				throw new CryptographicException("Unknown Canonicalization Method {0}", this.m_signature.SignedInfo.CanonicalizationMethod);
			}
			return transform;
		}

		private Stream SignedInfoTransformed()
		{
			Transform c14NMethod = this.GetC14NMethod();
			if (this.signatureElement == null)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.PreserveWhitespace = true;
				xmlDocument.LoadXml(this.m_signature.SignedInfo.GetXml().OuterXml);
				if (this.envdoc != null)
				{
					foreach (object obj in this.envdoc.DocumentElement.SelectNodes("namespace::*"))
					{
						XmlAttribute xmlAttribute = (XmlAttribute)obj;
						if (!(xmlAttribute.LocalName == "xml"))
						{
							if (!(xmlAttribute.Prefix == xmlDocument.DocumentElement.Prefix))
							{
								xmlDocument.DocumentElement.SetAttributeNode(xmlDocument.ImportNode(xmlAttribute, true) as XmlAttribute);
							}
						}
					}
				}
				c14NMethod.LoadInput(xmlDocument);
			}
			else
			{
				XmlElement xmlElement = this.signatureElement.GetElementsByTagName("SignedInfo", "http://www.w3.org/2000/09/xmldsig#")[0] as XmlElement;
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.WriteStartElement(xmlElement.Prefix, xmlElement.LocalName, xmlElement.NamespaceURI);
				XmlNodeList xmlNodeList = xmlElement.SelectNodes("namespace::*");
				foreach (object obj2 in xmlNodeList)
				{
					XmlAttribute xmlAttribute2 = (XmlAttribute)obj2;
					if (xmlAttribute2.ParentNode != xmlElement)
					{
						if (!(xmlAttribute2.LocalName == "xml"))
						{
							if (!(xmlAttribute2.Prefix == xmlElement.Prefix))
							{
								xmlAttribute2.WriteTo(xmlTextWriter);
							}
						}
					}
				}
				foreach (object obj3 in xmlElement.Attributes)
				{
					XmlNode xmlNode = (XmlNode)obj3;
					xmlNode.WriteTo(xmlTextWriter);
				}
				foreach (object obj4 in xmlElement.ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj4;
					xmlNode2.WriteTo(xmlTextWriter);
				}
				xmlTextWriter.WriteEndElement();
				byte[] bytes = Encoding.UTF8.GetBytes(stringWriter.ToString());
				MemoryStream memoryStream = new MemoryStream();
				memoryStream.Write(bytes, 0, bytes.Length);
				memoryStream.Position = 0L;
				c14NMethod.LoadInput(memoryStream);
			}
			return (Stream)c14NMethod.GetOutput();
		}

		private HashAlgorithm GetHash(string algorithm, bool check_hmac)
		{
			HashAlgorithm hashAlgorithm = (HashAlgorithm)this.hashes[algorithm];
			if (hashAlgorithm == null)
			{
				hashAlgorithm = HashAlgorithm.Create(algorithm);
				if (hashAlgorithm == null)
				{
					throw new CryptographicException("Unknown hash algorithm: {0}", algorithm);
				}
				this.hashes.Add(algorithm, hashAlgorithm);
			}
			else
			{
				hashAlgorithm.Initialize();
			}
			if (check_hmac && hashAlgorithm is KeyedHashAlgorithm)
			{
				return null;
			}
			return hashAlgorithm;
		}

		public bool CheckSignature()
		{
			return this.CheckSignatureInternal(null) != null;
		}

		private bool CheckReferenceIntegrity(ArrayList referenceList)
		{
			if (referenceList == null)
			{
				return false;
			}
			foreach (object obj in referenceList)
			{
				Reference reference = (Reference)obj;
				byte[] referenceHash = this.GetReferenceHash(reference, true);
				if (!this.Compare(reference.DigestValue, referenceHash))
				{
					return false;
				}
			}
			return true;
		}

		public bool CheckSignature(AsymmetricAlgorithm key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return this.CheckSignatureInternal(key) != null;
		}

		private AsymmetricAlgorithm CheckSignatureInternal(AsymmetricAlgorithm key)
		{
			this.pkEnumerator = null;
			if (key != null)
			{
				if (!this.CheckSignatureWithKey(key))
				{
					return null;
				}
			}
			else
			{
				if (this.Signature.KeyInfo == null)
				{
					return null;
				}
				while ((key = this.GetPublicKey()) != null)
				{
					if (this.CheckSignatureWithKey(key))
					{
						break;
					}
				}
				this.pkEnumerator = null;
				if (key == null)
				{
					return null;
				}
			}
			if (!this.CheckReferenceIntegrity(this.m_signature.SignedInfo.References))
			{
				return null;
			}
			if (this.manifests != null)
			{
				for (int i = 0; i < this.manifests.Count; i++)
				{
					Manifest manifest = new Manifest((this.manifests[i] as XmlDocument).DocumentElement);
					if (!this.CheckReferenceIntegrity(manifest.References))
					{
						return null;
					}
				}
			}
			return key;
		}

		private bool CheckSignatureWithKey(AsymmetricAlgorithm key)
		{
			if (key == null)
			{
				return false;
			}
			SignatureDescription signatureDescription = (SignatureDescription)CryptoConfig.CreateFromName(this.m_signature.SignedInfo.SignatureMethod);
			if (signatureDescription == null)
			{
				return false;
			}
			AsymmetricSignatureDeformatter asymmetricSignatureDeformatter = (AsymmetricSignatureDeformatter)CryptoConfig.CreateFromName(signatureDescription.DeformatterAlgorithm);
			if (asymmetricSignatureDeformatter == null)
			{
				return false;
			}
			bool flag;
			try
			{
				asymmetricSignatureDeformatter.SetKey(key);
				asymmetricSignatureDeformatter.SetHashAlgorithm(signatureDescription.DigestAlgorithm);
				HashAlgorithm hash = this.GetHash(signatureDescription.DigestAlgorithm, true);
				MemoryStream memoryStream = (MemoryStream)this.SignedInfoTransformed();
				byte[] array = hash.ComputeHash(memoryStream);
				flag = asymmetricSignatureDeformatter.VerifySignature(array, this.m_signature.SignatureValue);
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		private bool Compare(byte[] expected, byte[] actual)
		{
			bool flag = expected != null && actual != null;
			if (flag)
			{
				int num = expected.Length;
				flag = num == actual.Length;
				if (flag)
				{
					for (int i = 0; i < num; i++)
					{
						if (expected[i] != actual[i])
						{
							return false;
						}
					}
				}
			}
			return flag;
		}

		public bool CheckSignature(KeyedHashAlgorithm macAlg)
		{
			if (macAlg == null)
			{
				throw new ArgumentNullException("macAlg");
			}
			this.pkEnumerator = null;
			Stream stream = this.SignedInfoTransformed();
			if (stream == null)
			{
				return false;
			}
			byte[] array = macAlg.ComputeHash(stream);
			if (this.m_signature.SignedInfo.SignatureLength != null)
			{
				int num = int.Parse(this.m_signature.SignedInfo.SignatureLength);
				if ((num & 7) != 0)
				{
					throw new CryptographicException("Signature length must be a multiple of 8 bits.");
				}
				num >>= 3;
				if (num != this.m_signature.SignatureValue.Length)
				{
					throw new CryptographicException("Invalid signature length.");
				}
				int num2 = Math.Max(10, array.Length / 2);
				if (num < num2)
				{
					throw new CryptographicException("HMAC signature is too small");
				}
				if (num < array.Length)
				{
					byte[] array2 = new byte[num];
					Buffer.BlockCopy(array, 0, array2, 0, num);
					array = array2;
				}
			}
			return this.Compare(this.m_signature.SignatureValue, array) && this.CheckReferenceIntegrity(this.m_signature.SignedInfo.References);
		}

		[MonoTODO]
		[ComVisible(false)]
		public bool CheckSignature(X509Certificate2 certificate, bool verifySignatureOnly)
		{
			throw new NotImplementedException();
		}

		public bool CheckSignatureReturningKey(out AsymmetricAlgorithm signingKey)
		{
			signingKey = this.CheckSignatureInternal(null);
			return signingKey != null;
		}

		public void ComputeSignature()
		{
			if (this.key != null)
			{
				if (this.m_signature.SignedInfo.SignatureMethod == null)
				{
					this.m_signature.SignedInfo.SignatureMethod = this.key.SignatureAlgorithm;
				}
				else if (this.m_signature.SignedInfo.SignatureMethod != this.key.SignatureAlgorithm)
				{
					throw new CryptographicException("Specified SignatureAlgorithm is not supported by the signing key.");
				}
				this.DigestReferences();
				AsymmetricSignatureFormatter asymmetricSignatureFormatter = null;
				if (this.key is DSA)
				{
					asymmetricSignatureFormatter = new DSASignatureFormatter(this.key);
				}
				else if (this.key is RSA)
				{
					asymmetricSignatureFormatter = new RSAPKCS1SignatureFormatter(this.key);
				}
				if (asymmetricSignatureFormatter != null)
				{
					SignatureDescription signatureDescription = (SignatureDescription)CryptoConfig.CreateFromName(this.m_signature.SignedInfo.SignatureMethod);
					HashAlgorithm hash = this.GetHash(signatureDescription.DigestAlgorithm, false);
					byte[] array = hash.ComputeHash(this.SignedInfoTransformed());
					asymmetricSignatureFormatter.SetHashAlgorithm("SHA1");
					this.m_signature.SignatureValue = asymmetricSignatureFormatter.CreateSignature(array);
				}
				return;
			}
			throw new CryptographicException("signing key is not specified");
		}

		public void ComputeSignature(KeyedHashAlgorithm macAlg)
		{
			if (macAlg == null)
			{
				throw new ArgumentNullException("macAlg");
			}
			string text = null;
			if (macAlg is HMACSHA1)
			{
				text = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
			}
			else if (macAlg is HMACSHA256)
			{
				text = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
			}
			else if (macAlg is HMACSHA384)
			{
				text = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";
			}
			else if (macAlg is HMACSHA512)
			{
				text = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
			}
			else if (macAlg is HMACRIPEMD160)
			{
				text = "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160";
			}
			if (text == null)
			{
				throw new CryptographicException("unsupported algorithm");
			}
			this.DigestReferences();
			this.m_signature.SignedInfo.SignatureMethod = text;
			this.m_signature.SignatureValue = macAlg.ComputeHash(this.SignedInfoTransformed());
		}

		public virtual XmlElement GetIdElement(XmlDocument document, string idValue)
		{
			if (document == null || idValue == null)
			{
				return null;
			}
			XmlElement xmlElement = document.GetElementById(idValue);
			if (xmlElement == null)
			{
				xmlElement = (XmlElement)document.SelectSingleNode("//*[@Id='" + idValue + "']");
			}
			return xmlElement;
		}

		protected virtual AsymmetricAlgorithm GetPublicKey()
		{
			if (this.m_signature.KeyInfo == null)
			{
				return null;
			}
			if (this.pkEnumerator == null)
			{
				this.pkEnumerator = this.m_signature.KeyInfo.GetEnumerator();
			}
			if (this._x509Enumerator != null)
			{
				if (this._x509Enumerator.MoveNext())
				{
					X509Certificate x509Certificate = (X509Certificate)this._x509Enumerator.Current;
					return new X509Certificate2(x509Certificate.GetRawCertData()).PublicKey.Key;
				}
				this._x509Enumerator = null;
			}
			while (this.pkEnumerator.MoveNext())
			{
				AsymmetricAlgorithm asymmetricAlgorithm = null;
				KeyInfoClause keyInfoClause = (KeyInfoClause)this.pkEnumerator.Current;
				if (keyInfoClause is DSAKeyValue)
				{
					asymmetricAlgorithm = DSA.Create();
				}
				else if (keyInfoClause is RSAKeyValue)
				{
					asymmetricAlgorithm = RSA.Create();
				}
				if (asymmetricAlgorithm != null)
				{
					asymmetricAlgorithm.FromXmlString(keyInfoClause.GetXml().InnerXml);
					return asymmetricAlgorithm;
				}
				if (keyInfoClause is KeyInfoX509Data)
				{
					this._x509Enumerator = ((KeyInfoX509Data)keyInfoClause).Certificates.GetEnumerator();
					if (this._x509Enumerator.MoveNext())
					{
						X509Certificate x509Certificate2 = (X509Certificate)this._x509Enumerator.Current;
						return new X509Certificate2(x509Certificate2.GetRawCertData()).PublicKey.Key;
					}
				}
			}
			return null;
		}

		public XmlElement GetXml()
		{
			return this.m_signature.GetXml(this.envdoc);
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.signatureElement = value;
			this.m_signature.LoadXml(value);
			foreach (object obj in this.m_signature.SignedInfo.References)
			{
				Reference reference = (Reference)obj;
				foreach (object obj2 in reference.TransformChain)
				{
					Transform transform = (Transform)obj2;
					if (transform is XmlDecryptionTransform)
					{
						((XmlDecryptionTransform)transform).EncryptedXml = this.EncryptedXml;
					}
				}
			}
		}

		[ComVisible(false)]
		public XmlResolver Resolver
		{
			set
			{
				this.xmlResolver = value;
			}
		}

		public const string XmlDsigCanonicalizationUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

		public const string XmlDsigCanonicalizationWithCommentsUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";

		public const string XmlDsigDSAUrl = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";

		public const string XmlDsigHMACSHA1Url = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";

		public const string XmlDsigMinimalCanonicalizationUrl = "http://www.w3.org/2000/09/xmldsig#minimal";

		public const string XmlDsigNamespaceUrl = "http://www.w3.org/2000/09/xmldsig#";

		public const string XmlDsigRSASHA1Url = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

		public const string XmlDsigSHA1Url = "http://www.w3.org/2000/09/xmldsig#sha1";

		public const string XmlDecryptionTransformUrl = "http://www.w3.org/2002/07/decrypt#XML";

		public const string XmlDsigBase64TransformUrl = "http://www.w3.org/2000/09/xmldsig#base64";

		public const string XmlDsigC14NTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

		public const string XmlDsigC14NWithCommentsTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";

		public const string XmlDsigEnvelopedSignatureTransformUrl = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";

		public const string XmlDsigExcC14NTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#";

		public const string XmlDsigExcC14NWithCommentsTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";

		public const string XmlDsigXPathTransformUrl = "http://www.w3.org/TR/1999/REC-xpath-19991116";

		public const string XmlDsigXsltTransformUrl = "http://www.w3.org/TR/1999/REC-xslt-19991116";

		public const string XmlLicenseTransformUrl = "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform";

		private EncryptedXml encryptedXml;

		protected Signature m_signature;

		private AsymmetricAlgorithm key;

		protected string m_strSigningKeyName;

		private XmlDocument envdoc;

		private IEnumerator pkEnumerator;

		private XmlElement signatureElement;

		private Hashtable hashes;

		private XmlResolver xmlResolver = new XmlUrlResolver();

		private ArrayList manifests;

		private IEnumerator _x509Enumerator;

		private static readonly char[] whitespaceChars = new char[] { ' ', '\r', '\n', '\t' };
	}
}
