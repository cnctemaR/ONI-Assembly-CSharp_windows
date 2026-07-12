using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class EncryptedXml
	{
		public EncryptedXml()
			: this(new XmlDocument())
		{
		}

		public EncryptedXml(XmlDocument document)
			: this(document, null)
		{
		}

		public EncryptedXml(XmlDocument document, Evidence evidence)
		{
			this._document = document;
			this._evidence = evidence;
			this._xmlResolver = null;
			this._padding = PaddingMode.ISO10126;
			this._mode = CipherMode.CBC;
			this._encoding = Encoding.UTF8;
			this._keyNameMapping = new Hashtable(4);
			this._xmlDsigSearchDepth = 20;
		}

		private bool IsOverXmlDsigRecursionLimit()
		{
			return this._xmlDsigSearchDepthCounter > this.XmlDSigSearchDepth;
		}

		public int XmlDSigSearchDepth
		{
			get
			{
				return this._xmlDsigSearchDepth;
			}
			set
			{
				this._xmlDsigSearchDepth = value;
			}
		}

		public Evidence DocumentEvidence
		{
			get
			{
				return this._evidence;
			}
			set
			{
				this._evidence = value;
			}
		}

		public XmlResolver Resolver
		{
			get
			{
				return this._xmlResolver;
			}
			set
			{
				this._xmlResolver = value;
			}
		}

		public PaddingMode Padding
		{
			get
			{
				return this._padding;
			}
			set
			{
				this._padding = value;
			}
		}

		public CipherMode Mode
		{
			get
			{
				return this._mode;
			}
			set
			{
				this._mode = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return this._encoding;
			}
			set
			{
				this._encoding = value;
			}
		}

		public string Recipient
		{
			get
			{
				if (this._recipient == null)
				{
					this._recipient = string.Empty;
				}
				return this._recipient;
			}
			set
			{
				this._recipient = value;
			}
		}

		private byte[] GetCipherValue(CipherData cipherData)
		{
			if (cipherData == null)
			{
				throw new ArgumentNullException("cipherData");
			}
			WebResponse webResponse = null;
			Stream stream = null;
			if (cipherData.CipherValue != null)
			{
				return cipherData.CipherValue;
			}
			if (cipherData.CipherReference == null)
			{
				throw new CryptographicException("Cipher data is not specified.");
			}
			if (cipherData.CipherReference.CipherValue != null)
			{
				return cipherData.CipherReference.CipherValue;
			}
			if (cipherData.CipherReference.Uri == null)
			{
				throw new CryptographicException(" The specified Uri is not supported.");
			}
			Stream stream2;
			if (cipherData.CipherReference.Uri.Length == 0)
			{
				string text = ((this._document == null) ? null : this._document.BaseURI);
				TransformChain transformChain = cipherData.CipherReference.TransformChain;
				if (transformChain == null)
				{
					throw new CryptographicException(" The specified Uri is not supported.");
				}
				stream2 = transformChain.TransformToOctetStream(this._document, this._xmlResolver, text);
			}
			else
			{
				if (cipherData.CipherReference.Uri[0] != '#')
				{
					throw new CryptographicException("Unable to resolve Uri {0}.", cipherData.CipherReference.Uri);
				}
				string text2 = Utils.ExtractIdFromLocalUri(cipherData.CipherReference.Uri);
				XmlElement idElement = this.GetIdElement(this._document, text2);
				if (idElement == null || idElement.OuterXml == null)
				{
					throw new CryptographicException(" The specified Uri is not supported.");
				}
				stream = new MemoryStream(this._encoding.GetBytes(idElement.OuterXml));
				string text3 = ((this._document == null) ? null : this._document.BaseURI);
				TransformChain transformChain2 = cipherData.CipherReference.TransformChain;
				if (transformChain2 == null)
				{
					throw new CryptographicException(" The specified Uri is not supported.");
				}
				stream2 = transformChain2.TransformToOctetStream(stream, this._xmlResolver, text3);
			}
			byte[] array = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Utils.Pump(stream2, memoryStream);
				array = memoryStream.ToArray();
				if (webResponse != null)
				{
					webResponse.Close();
				}
				if (stream != null)
				{
					stream.Close();
				}
				stream2.Close();
			}
			cipherData.CipherReference.CipherValue = array;
			return array;
		}

		public virtual XmlElement GetIdElement(XmlDocument document, string idValue)
		{
			return SignedXml.DefaultGetIdElement(document, idValue);
		}

		public virtual byte[] GetDecryptionIV(EncryptedData encryptedData, string symmetricAlgorithmUri)
		{
			if (encryptedData == null)
			{
				throw new ArgumentNullException("encryptedData");
			}
			if (symmetricAlgorithmUri == null)
			{
				if (encryptedData.EncryptionMethod == null)
				{
					throw new CryptographicException("Symmetric algorithm is not specified.");
				}
				symmetricAlgorithmUri = encryptedData.EncryptionMethod.KeyAlgorithm;
			}
			int num;
			if (!(symmetricAlgorithmUri == "http://www.w3.org/2001/04/xmlenc#des-cbc") && !(symmetricAlgorithmUri == "http://www.w3.org/2001/04/xmlenc#tripledes-cbc"))
			{
				if (!(symmetricAlgorithmUri == "http://www.w3.org/2001/04/xmlenc#aes128-cbc") && !(symmetricAlgorithmUri == "http://www.w3.org/2001/04/xmlenc#aes192-cbc") && !(symmetricAlgorithmUri == "http://www.w3.org/2001/04/xmlenc#aes256-cbc"))
				{
					throw new CryptographicException(" The specified Uri is not supported.");
				}
				num = 16;
			}
			else
			{
				num = 8;
			}
			byte[] array = new byte[num];
			Buffer.BlockCopy(this.GetCipherValue(encryptedData.CipherData), 0, array, 0, array.Length);
			return array;
		}

		public virtual SymmetricAlgorithm GetDecryptionKey(EncryptedData encryptedData, string symmetricAlgorithmUri)
		{
			if (encryptedData == null)
			{
				throw new ArgumentNullException("encryptedData");
			}
			if (encryptedData.KeyInfo == null)
			{
				return null;
			}
			IEnumerator enumerator = encryptedData.KeyInfo.GetEnumerator();
			EncryptedKey encryptedKey = null;
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				KeyInfoName keyInfoName = obj as KeyInfoName;
				if (keyInfoName != null)
				{
					string value = keyInfoName.Value;
					if ((SymmetricAlgorithm)this._keyNameMapping[value] != null)
					{
						return (SymmetricAlgorithm)this._keyNameMapping[value];
					}
					XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(this._document.NameTable);
					xmlNamespaceManager.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
					XmlNodeList xmlNodeList = this._document.SelectNodes("//enc:EncryptedKey", xmlNamespaceManager);
					if (xmlNodeList == null)
					{
						break;
					}
					using (IEnumerator enumerator2 = xmlNodeList.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object obj2 = enumerator2.Current;
							XmlElement xmlElement = ((XmlNode)obj2) as XmlElement;
							EncryptedKey encryptedKey2 = new EncryptedKey();
							encryptedKey2.LoadXml(xmlElement);
							if (encryptedKey2.CarriedKeyName == value && encryptedKey2.Recipient == this.Recipient)
							{
								encryptedKey = encryptedKey2;
								break;
							}
						}
						break;
					}
				}
				KeyInfoRetrievalMethod keyInfoRetrievalMethod = enumerator.Current as KeyInfoRetrievalMethod;
				if (keyInfoRetrievalMethod != null)
				{
					string text = Utils.ExtractIdFromLocalUri(keyInfoRetrievalMethod.Uri);
					encryptedKey = new EncryptedKey();
					encryptedKey.LoadXml(this.GetIdElement(this._document, text));
					break;
				}
				KeyInfoEncryptedKey keyInfoEncryptedKey = enumerator.Current as KeyInfoEncryptedKey;
				if (keyInfoEncryptedKey != null)
				{
					encryptedKey = keyInfoEncryptedKey.EncryptedKey;
					break;
				}
			}
			if (encryptedKey == null)
			{
				return null;
			}
			if (symmetricAlgorithmUri == null)
			{
				if (encryptedData.EncryptionMethod == null)
				{
					throw new CryptographicException("Symmetric algorithm is not specified.");
				}
				symmetricAlgorithmUri = encryptedData.EncryptionMethod.KeyAlgorithm;
			}
			byte[] array = this.DecryptEncryptedKey(encryptedKey);
			if (array == null)
			{
				throw new CryptographicException("Unable to retrieve the decryption key.");
			}
			SymmetricAlgorithm symmetricAlgorithm = CryptoHelpers.CreateFromName<SymmetricAlgorithm>(symmetricAlgorithmUri);
			if (symmetricAlgorithm == null)
			{
				throw new CryptographicException("Symmetric algorithm is not specified.");
			}
			symmetricAlgorithm.Key = array;
			return symmetricAlgorithm;
		}

		public virtual byte[] DecryptEncryptedKey(EncryptedKey encryptedKey)
		{
			if (encryptedKey == null)
			{
				throw new ArgumentNullException("encryptedKey");
			}
			if (encryptedKey.KeyInfo == null)
			{
				return null;
			}
			foreach (object obj in encryptedKey.KeyInfo)
			{
				KeyInfoName keyInfoName = obj as KeyInfoName;
				bool flag;
				if (keyInfoName == null)
				{
					IEnumerator enumerator;
					KeyInfoX509Data keyInfoX509Data = enumerator.Current as KeyInfoX509Data;
					if (keyInfoX509Data != null)
					{
						foreach (X509Certificate2 x509Certificate in Utils.BuildBagOfCerts(keyInfoX509Data, CertUsageType.Decryption))
						{
							using (RSA rsaprivateKey = x509Certificate.GetRSAPrivateKey())
							{
								if (rsaprivateKey != null)
								{
									if (encryptedKey.CipherData == null || encryptedKey.CipherData.CipherValue == null)
									{
										throw new CryptographicException("Symmetric algorithm is not specified.");
									}
									flag = encryptedKey.EncryptionMethod != null && encryptedKey.EncryptionMethod.KeyAlgorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p";
									return EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, rsaprivateKey, flag);
								}
							}
						}
						break;
					}
					KeyInfoRetrievalMethod keyInfoRetrievalMethod = enumerator.Current as KeyInfoRetrievalMethod;
					EncryptedKey encryptedKey2;
					if (keyInfoRetrievalMethod != null)
					{
						string text = Utils.ExtractIdFromLocalUri(keyInfoRetrievalMethod.Uri);
						encryptedKey2 = new EncryptedKey();
						encryptedKey2.LoadXml(this.GetIdElement(this._document, text));
						try
						{
							this._xmlDsigSearchDepthCounter++;
							if (this.IsOverXmlDsigRecursionLimit())
							{
								throw new CryptoSignedXmlRecursionException();
							}
							return this.DecryptEncryptedKey(encryptedKey2);
						}
						finally
						{
							this._xmlDsigSearchDepthCounter--;
						}
					}
					KeyInfoEncryptedKey keyInfoEncryptedKey = enumerator.Current as KeyInfoEncryptedKey;
					if (keyInfoEncryptedKey == null)
					{
						continue;
					}
					encryptedKey2 = keyInfoEncryptedKey.EncryptedKey;
					byte[] array = this.DecryptEncryptedKey(encryptedKey2);
					if (array == null)
					{
						continue;
					}
					SymmetricAlgorithm symmetricAlgorithm = CryptoHelpers.CreateFromName<SymmetricAlgorithm>(encryptedKey.EncryptionMethod.KeyAlgorithm);
					if (symmetricAlgorithm == null)
					{
						throw new CryptographicException("Symmetric algorithm is not specified.");
					}
					symmetricAlgorithm.Key = array;
					if (encryptedKey.CipherData == null || encryptedKey.CipherData.CipherValue == null)
					{
						throw new CryptographicException("Symmetric algorithm is not specified.");
					}
					symmetricAlgorithm.Key = array;
					return EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, symmetricAlgorithm);
				}
				string value = keyInfoName.Value;
				object obj2 = this._keyNameMapping[value];
				if (obj2 == null)
				{
					break;
				}
				if (encryptedKey.CipherData == null || encryptedKey.CipherData.CipherValue == null)
				{
					throw new CryptographicException("Symmetric algorithm is not specified.");
				}
				if (obj2 is SymmetricAlgorithm)
				{
					return EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, (SymmetricAlgorithm)obj2);
				}
				flag = encryptedKey.EncryptionMethod != null && encryptedKey.EncryptionMethod.KeyAlgorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p";
				return EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, (RSA)obj2, flag);
			}
			return null;
		}

		public void AddKeyNameMapping(string keyName, object keyObject)
		{
			if (keyName == null)
			{
				throw new ArgumentNullException("keyName");
			}
			if (keyObject == null)
			{
				throw new ArgumentNullException("keyObject");
			}
			if (!(keyObject is SymmetricAlgorithm) && !(keyObject is RSA))
			{
				throw new CryptographicException("The specified cryptographic transform is not supported.");
			}
			this._keyNameMapping.Add(keyName, keyObject);
		}

		public void ClearKeyNameMappings()
		{
			this._keyNameMapping.Clear();
		}

		public EncryptedData Encrypt(XmlElement inputElement, X509Certificate2 certificate)
		{
			if (inputElement == null)
			{
				throw new ArgumentNullException("inputElement");
			}
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			EncryptedData encryptedData2;
			using (RSA rsapublicKey = certificate.GetRSAPublicKey())
			{
				if (rsapublicKey == null)
				{
					throw new NotSupportedException("The certificate key algorithm is not supported.");
				}
				EncryptedData encryptedData = new EncryptedData();
				encryptedData.Type = "http://www.w3.org/2001/04/xmlenc#Element";
				encryptedData.EncryptionMethod = new EncryptionMethod("http://www.w3.org/2001/04/xmlenc#aes256-cbc");
				EncryptedKey encryptedKey = new EncryptedKey();
				encryptedKey.EncryptionMethod = new EncryptionMethod("http://www.w3.org/2001/04/xmlenc#rsa-1_5");
				encryptedKey.KeyInfo.AddClause(new KeyInfoX509Data(certificate));
				RijndaelManaged rijndaelManaged = new RijndaelManaged();
				encryptedKey.CipherData.CipherValue = EncryptedXml.EncryptKey(rijndaelManaged.Key, rsapublicKey, false);
				KeyInfoEncryptedKey keyInfoEncryptedKey = new KeyInfoEncryptedKey(encryptedKey);
				encryptedData.KeyInfo.AddClause(keyInfoEncryptedKey);
				encryptedData.CipherData.CipherValue = this.EncryptData(inputElement, rijndaelManaged, false);
				encryptedData2 = encryptedData;
			}
			return encryptedData2;
		}

		public EncryptedData Encrypt(XmlElement inputElement, string keyName)
		{
			if (inputElement == null)
			{
				throw new ArgumentNullException("inputElement");
			}
			if (keyName == null)
			{
				throw new ArgumentNullException("keyName");
			}
			object obj = null;
			if (this._keyNameMapping != null)
			{
				obj = this._keyNameMapping[keyName];
			}
			if (obj == null)
			{
				throw new CryptographicException("Unable to retrieve the encryption key.");
			}
			SymmetricAlgorithm symmetricAlgorithm = obj as SymmetricAlgorithm;
			RSA rsa = obj as RSA;
			EncryptedData encryptedData = new EncryptedData();
			encryptedData.Type = "http://www.w3.org/2001/04/xmlenc#Element";
			encryptedData.EncryptionMethod = new EncryptionMethod("http://www.w3.org/2001/04/xmlenc#aes256-cbc");
			string text = null;
			if (symmetricAlgorithm == null)
			{
				text = "http://www.w3.org/2001/04/xmlenc#rsa-1_5";
			}
			else if (symmetricAlgorithm is TripleDES)
			{
				text = "http://www.w3.org/2001/04/xmlenc#kw-tripledes";
			}
			else
			{
				if (!(symmetricAlgorithm is Rijndael) && !(symmetricAlgorithm is Aes))
				{
					throw new CryptographicException("The specified cryptographic transform is not supported.");
				}
				int keySize = symmetricAlgorithm.KeySize;
				if (keySize != 128)
				{
					if (keySize != 192)
					{
						if (keySize == 256)
						{
							text = "http://www.w3.org/2001/04/xmlenc#kw-aes256";
						}
					}
					else
					{
						text = "http://www.w3.org/2001/04/xmlenc#kw-aes192";
					}
				}
				else
				{
					text = "http://www.w3.org/2001/04/xmlenc#kw-aes128";
				}
			}
			EncryptedKey encryptedKey = new EncryptedKey();
			encryptedKey.EncryptionMethod = new EncryptionMethod(text);
			encryptedKey.KeyInfo.AddClause(new KeyInfoName(keyName));
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			encryptedKey.CipherData.CipherValue = ((symmetricAlgorithm == null) ? EncryptedXml.EncryptKey(rijndaelManaged.Key, rsa, false) : EncryptedXml.EncryptKey(rijndaelManaged.Key, symmetricAlgorithm));
			KeyInfoEncryptedKey keyInfoEncryptedKey = new KeyInfoEncryptedKey(encryptedKey);
			encryptedData.KeyInfo.AddClause(keyInfoEncryptedKey);
			encryptedData.CipherData.CipherValue = this.EncryptData(inputElement, rijndaelManaged, false);
			return encryptedData;
		}

		public void DecryptDocument()
		{
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(this._document.NameTable);
			xmlNamespaceManager.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
			XmlNodeList xmlNodeList = this._document.SelectNodes("//enc:EncryptedData", xmlNamespaceManager);
			if (xmlNodeList != null)
			{
				foreach (object obj in xmlNodeList)
				{
					XmlElement xmlElement = ((XmlNode)obj) as XmlElement;
					EncryptedData encryptedData = new EncryptedData();
					encryptedData.LoadXml(xmlElement);
					SymmetricAlgorithm decryptionKey = this.GetDecryptionKey(encryptedData, null);
					if (decryptionKey == null)
					{
						throw new CryptographicException("Unable to retrieve the decryption key.");
					}
					byte[] array = this.DecryptData(encryptedData, decryptionKey);
					this.ReplaceData(xmlElement, array);
				}
			}
		}

		public byte[] EncryptData(byte[] plaintext, SymmetricAlgorithm symmetricAlgorithm)
		{
			if (plaintext == null)
			{
				throw new ArgumentNullException("plaintext");
			}
			if (symmetricAlgorithm == null)
			{
				throw new ArgumentNullException("symmetricAlgorithm");
			}
			CipherMode mode = symmetricAlgorithm.Mode;
			PaddingMode padding = symmetricAlgorithm.Padding;
			byte[] array = null;
			try
			{
				symmetricAlgorithm.Mode = this._mode;
				symmetricAlgorithm.Padding = this._padding;
				array = symmetricAlgorithm.CreateEncryptor().TransformFinalBlock(plaintext, 0, plaintext.Length);
			}
			finally
			{
				symmetricAlgorithm.Mode = mode;
				symmetricAlgorithm.Padding = padding;
			}
			byte[] array2;
			if (this._mode == CipherMode.ECB)
			{
				array2 = array;
			}
			else
			{
				byte[] iv = symmetricAlgorithm.IV;
				array2 = new byte[array.Length + iv.Length];
				Buffer.BlockCopy(iv, 0, array2, 0, iv.Length);
				Buffer.BlockCopy(array, 0, array2, iv.Length, array.Length);
			}
			return array2;
		}

		public byte[] EncryptData(XmlElement inputElement, SymmetricAlgorithm symmetricAlgorithm, bool content)
		{
			if (inputElement == null)
			{
				throw new ArgumentNullException("inputElement");
			}
			if (symmetricAlgorithm == null)
			{
				throw new ArgumentNullException("symmetricAlgorithm");
			}
			byte[] array = (content ? this._encoding.GetBytes(inputElement.InnerXml) : this._encoding.GetBytes(inputElement.OuterXml));
			return this.EncryptData(array, symmetricAlgorithm);
		}

		public byte[] DecryptData(EncryptedData encryptedData, SymmetricAlgorithm symmetricAlgorithm)
		{
			if (encryptedData == null)
			{
				throw new ArgumentNullException("encryptedData");
			}
			if (symmetricAlgorithm == null)
			{
				throw new ArgumentNullException("symmetricAlgorithm");
			}
			byte[] cipherValue = this.GetCipherValue(encryptedData.CipherData);
			CipherMode mode = symmetricAlgorithm.Mode;
			PaddingMode padding = symmetricAlgorithm.Padding;
			byte[] iv = symmetricAlgorithm.IV;
			byte[] array = null;
			if (this._mode != CipherMode.ECB)
			{
				array = this.GetDecryptionIV(encryptedData, null);
			}
			byte[] array2 = null;
			try
			{
				int num = 0;
				if (array != null)
				{
					symmetricAlgorithm.IV = array;
					num = array.Length;
				}
				symmetricAlgorithm.Mode = this._mode;
				symmetricAlgorithm.Padding = this._padding;
				array2 = symmetricAlgorithm.CreateDecryptor().TransformFinalBlock(cipherValue, num, cipherValue.Length - num);
			}
			finally
			{
				symmetricAlgorithm.Mode = mode;
				symmetricAlgorithm.Padding = padding;
				symmetricAlgorithm.IV = iv;
			}
			return array2;
		}

		public void ReplaceData(XmlElement inputElement, byte[] decryptedData)
		{
			if (inputElement == null)
			{
				throw new ArgumentNullException("inputElement");
			}
			if (decryptedData == null)
			{
				throw new ArgumentNullException("decryptedData");
			}
			XmlNode parentNode = inputElement.ParentNode;
			if (parentNode.NodeType == XmlNodeType.Document)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.PreserveWhitespace = true;
				using (StringReader stringReader = new StringReader(this._encoding.GetString(decryptedData)))
				{
					using (XmlReader xmlReader = XmlReader.Create(stringReader, Utils.GetSecureXmlReaderSettings(this._xmlResolver)))
					{
						xmlDocument.Load(xmlReader);
					}
				}
				XmlNode xmlNode = inputElement.OwnerDocument.ImportNode(xmlDocument.DocumentElement, true);
				parentNode.RemoveChild(inputElement);
				parentNode.AppendChild(xmlNode);
				return;
			}
			XmlNode xmlNode2 = parentNode.OwnerDocument.CreateElement(parentNode.Prefix, parentNode.LocalName, parentNode.NamespaceURI);
			try
			{
				parentNode.AppendChild(xmlNode2);
				xmlNode2.InnerXml = this._encoding.GetString(decryptedData);
				XmlNode xmlNode3 = xmlNode2.FirstChild;
				XmlNode nextSibling = inputElement.NextSibling;
				while (xmlNode3 != null)
				{
					XmlNode nextSibling2 = xmlNode3.NextSibling;
					parentNode.InsertBefore(xmlNode3, nextSibling);
					xmlNode3 = nextSibling2;
				}
			}
			finally
			{
				parentNode.RemoveChild(xmlNode2);
			}
			parentNode.RemoveChild(inputElement);
		}

		public static void ReplaceElement(XmlElement inputElement, EncryptedData encryptedData, bool content)
		{
			if (inputElement == null)
			{
				throw new ArgumentNullException("inputElement");
			}
			if (encryptedData == null)
			{
				throw new ArgumentNullException("encryptedData");
			}
			XmlElement xml = encryptedData.GetXml(inputElement.OwnerDocument);
			if (content)
			{
				Utils.RemoveAllChildren(inputElement);
				inputElement.AppendChild(xml);
				return;
			}
			inputElement.ParentNode.ReplaceChild(xml, inputElement);
		}

		public static byte[] EncryptKey(byte[] keyData, SymmetricAlgorithm symmetricAlgorithm)
		{
			if (keyData == null)
			{
				throw new ArgumentNullException("keyData");
			}
			if (symmetricAlgorithm == null)
			{
				throw new ArgumentNullException("symmetricAlgorithm");
			}
			if (symmetricAlgorithm is TripleDES)
			{
				return SymmetricKeyWrap.TripleDESKeyWrapEncrypt(symmetricAlgorithm.Key, keyData);
			}
			if (symmetricAlgorithm is Rijndael || symmetricAlgorithm is Aes)
			{
				return SymmetricKeyWrap.AESKeyWrapEncrypt(symmetricAlgorithm.Key, keyData);
			}
			throw new CryptographicException("The specified cryptographic transform is not supported.");
		}

		public static byte[] EncryptKey(byte[] keyData, RSA rsa, bool useOAEP)
		{
			if (keyData == null)
			{
				throw new ArgumentNullException("keyData");
			}
			if (rsa == null)
			{
				throw new ArgumentNullException("rsa");
			}
			if (useOAEP)
			{
				return new RSAOAEPKeyExchangeFormatter(rsa).CreateKeyExchange(keyData);
			}
			return new RSAPKCS1KeyExchangeFormatter(rsa).CreateKeyExchange(keyData);
		}

		public static byte[] DecryptKey(byte[] keyData, SymmetricAlgorithm symmetricAlgorithm)
		{
			if (keyData == null)
			{
				throw new ArgumentNullException("keyData");
			}
			if (symmetricAlgorithm == null)
			{
				throw new ArgumentNullException("symmetricAlgorithm");
			}
			if (symmetricAlgorithm is TripleDES)
			{
				return SymmetricKeyWrap.TripleDESKeyWrapDecrypt(symmetricAlgorithm.Key, keyData);
			}
			if (symmetricAlgorithm is Rijndael || symmetricAlgorithm is Aes)
			{
				return SymmetricKeyWrap.AESKeyWrapDecrypt(symmetricAlgorithm.Key, keyData);
			}
			throw new CryptographicException("The specified cryptographic transform is not supported.");
		}

		public static byte[] DecryptKey(byte[] keyData, RSA rsa, bool useOAEP)
		{
			if (keyData == null)
			{
				throw new ArgumentNullException("keyData");
			}
			if (rsa == null)
			{
				throw new ArgumentNullException("rsa");
			}
			if (useOAEP)
			{
				return new RSAOAEPKeyExchangeDeformatter(rsa).DecryptKeyExchange(keyData);
			}
			return new RSAPKCS1KeyExchangeDeformatter(rsa).DecryptKeyExchange(keyData);
		}

		public const string XmlEncNamespaceUrl = "http://www.w3.org/2001/04/xmlenc#";

		public const string XmlEncElementUrl = "http://www.w3.org/2001/04/xmlenc#Element";

		public const string XmlEncElementContentUrl = "http://www.w3.org/2001/04/xmlenc#Content";

		public const string XmlEncEncryptedKeyUrl = "http://www.w3.org/2001/04/xmlenc#EncryptedKey";

		public const string XmlEncDESUrl = "http://www.w3.org/2001/04/xmlenc#des-cbc";

		public const string XmlEncTripleDESUrl = "http://www.w3.org/2001/04/xmlenc#tripledes-cbc";

		public const string XmlEncAES128Url = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";

		public const string XmlEncAES256Url = "http://www.w3.org/2001/04/xmlenc#aes256-cbc";

		public const string XmlEncAES192Url = "http://www.w3.org/2001/04/xmlenc#aes192-cbc";

		public const string XmlEncRSA15Url = "http://www.w3.org/2001/04/xmlenc#rsa-1_5";

		public const string XmlEncRSAOAEPUrl = "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p";

		public const string XmlEncTripleDESKeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-tripledes";

		public const string XmlEncAES128KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes128";

		public const string XmlEncAES256KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes256";

		public const string XmlEncAES192KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes192";

		public const string XmlEncSHA256Url = "http://www.w3.org/2001/04/xmlenc#sha256";

		public const string XmlEncSHA512Url = "http://www.w3.org/2001/04/xmlenc#sha512";

		private XmlDocument _document;

		private Evidence _evidence;

		private XmlResolver _xmlResolver;

		private const int _capacity = 4;

		private Hashtable _keyNameMapping;

		private PaddingMode _padding;

		private CipherMode _mode;

		private Encoding _encoding;

		private string _recipient;

		private int _xmlDsigSearchDepthCounter;

		private int _xmlDsigSearchDepth;
	}
}
