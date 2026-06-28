using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class EncryptedXml
	{
		[MonoTODO]
		public EncryptedXml()
		{
		}

		[MonoTODO]
		public EncryptedXml(XmlDocument document)
		{
			this.document = document;
		}

		[MonoTODO]
		public EncryptedXml(XmlDocument document, Evidence evidence)
		{
			this.document = document;
			this.DocumentEvidence = evidence;
		}

		public Evidence DocumentEvidence
		{
			get
			{
				return this.documentEvidence;
			}
			set
			{
				this.documentEvidence = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				this.encoding = value;
			}
		}

		public CipherMode Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				this.mode = value;
			}
		}

		public PaddingMode Padding
		{
			get
			{
				return this.padding;
			}
			set
			{
				this.padding = value;
			}
		}

		public string Recipient
		{
			get
			{
				return this.recipient;
			}
			set
			{
				this.recipient = value;
			}
		}

		public XmlResolver Resolver
		{
			get
			{
				return this.resolver;
			}
			set
			{
				this.resolver = value;
			}
		}

		public void AddKeyNameMapping(string keyName, object keyObject)
		{
			this.keyNameMapping[keyName] = keyObject;
		}

		public void ClearKeyNameMappings()
		{
			this.keyNameMapping.Clear();
		}

		public byte[] DecryptData(EncryptedData encryptedData, SymmetricAlgorithm symAlg)
		{
			if (encryptedData == null)
			{
				throw new ArgumentNullException("encryptedData");
			}
			if (symAlg == null)
			{
				throw new ArgumentNullException("symAlg");
			}
			PaddingMode paddingMode = symAlg.Padding;
			byte[] array;
			try
			{
				symAlg.Padding = this.Padding;
				array = this.Transform(encryptedData.CipherData.CipherValue, symAlg.CreateDecryptor(), symAlg.BlockSize / 8, true);
			}
			finally
			{
				symAlg.Padding = paddingMode;
			}
			return array;
		}

		public void DecryptDocument()
		{
			XmlNodeList elementsByTagName = this.document.GetElementsByTagName("EncryptedData", "http://www.w3.org/2001/04/xmlenc#");
			foreach (object obj in elementsByTagName)
			{
				XmlNode xmlNode = (XmlNode)obj;
				EncryptedData encryptedData = new EncryptedData();
				encryptedData.LoadXml((XmlElement)xmlNode);
				SymmetricAlgorithm decryptionKey = this.GetDecryptionKey(encryptedData, encryptedData.EncryptionMethod.KeyAlgorithm);
				this.ReplaceData((XmlElement)xmlNode, this.DecryptData(encryptedData, decryptionKey));
			}
		}

		public virtual byte[] DecryptEncryptedKey(EncryptedKey encryptedKey)
		{
			if (encryptedKey == null)
			{
				throw new ArgumentNullException("encryptedKey");
			}
			object obj = null;
			foreach (object obj2 in encryptedKey.KeyInfo)
			{
				KeyInfoClause keyInfoClause = (KeyInfoClause)obj2;
				if (keyInfoClause is KeyInfoName)
				{
					obj = this.keyNameMapping[((KeyInfoName)keyInfoClause).Value];
					break;
				}
			}
			string keyAlgorithm = encryptedKey.EncryptionMethod.KeyAlgorithm;
			if (keyAlgorithm != null)
			{
				if (EncryptedXml.<>f__switch$map8 == null)
				{
					EncryptedXml.<>f__switch$map8 = new Dictionary<string, int>(2)
					{
						{ "http://www.w3.org/2001/04/xmlenc#rsa-1_5", 0 },
						{ "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p", 1 }
					};
				}
				int num;
				if (EncryptedXml.<>f__switch$map8.TryGetValue(keyAlgorithm, out num))
				{
					if (num == 0)
					{
						return EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, (RSA)obj, false);
					}
					if (num == 1)
					{
						return EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, (RSA)obj, true);
					}
				}
			}
			return EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, (SymmetricAlgorithm)obj);
		}

		public static byte[] DecryptKey(byte[] keyData, SymmetricAlgorithm symAlg)
		{
			if (keyData == null)
			{
				throw new ArgumentNullException("keyData");
			}
			if (symAlg == null)
			{
				throw new ArgumentNullException("symAlg");
			}
			if (symAlg is TripleDES)
			{
				return SymmetricKeyWrap.TripleDESKeyWrapDecrypt(symAlg.Key, keyData);
			}
			if (symAlg is Rijndael)
			{
				return SymmetricKeyWrap.AESKeyWrapDecrypt(symAlg.Key, keyData);
			}
			throw new CryptographicException("The specified cryptographic transform is not supported.");
		}

		[MonoTODO("Test this.")]
		public static byte[] DecryptKey(byte[] keyData, RSA rsa, bool fOAEP)
		{
			AsymmetricKeyExchangeDeformatter asymmetricKeyExchangeDeformatter;
			if (fOAEP)
			{
				asymmetricKeyExchangeDeformatter = new RSAOAEPKeyExchangeDeformatter(rsa);
			}
			else
			{
				asymmetricKeyExchangeDeformatter = new RSAPKCS1KeyExchangeDeformatter(rsa);
			}
			return asymmetricKeyExchangeDeformatter.DecryptKeyExchange(keyData);
		}

		public EncryptedData Encrypt(XmlElement inputElement, string keyName)
		{
			SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
			symmetricAlgorithm.KeySize = 256;
			symmetricAlgorithm.GenerateKey();
			symmetricAlgorithm.GenerateIV();
			EncryptedData encryptedData = new EncryptedData();
			EncryptedKey encryptedKey = new EncryptedKey();
			object obj = this.keyNameMapping[keyName];
			encryptedKey.EncryptionMethod = new EncryptionMethod(EncryptedXml.GetKeyWrapAlgorithmUri(obj));
			if (obj is RSA)
			{
				encryptedKey.CipherData = new CipherData(EncryptedXml.EncryptKey(symmetricAlgorithm.Key, (RSA)obj, false));
			}
			else
			{
				encryptedKey.CipherData = new CipherData(EncryptedXml.EncryptKey(symmetricAlgorithm.Key, (SymmetricAlgorithm)obj));
			}
			encryptedKey.KeyInfo = new KeyInfo();
			encryptedKey.KeyInfo.AddClause(new KeyInfoName(keyName));
			encryptedData.Type = "http://www.w3.org/2001/04/xmlenc#Element";
			encryptedData.EncryptionMethod = new EncryptionMethod(EncryptedXml.GetAlgorithmUri(symmetricAlgorithm));
			encryptedData.KeyInfo = new KeyInfo();
			encryptedData.KeyInfo.AddClause(new KeyInfoEncryptedKey(encryptedKey));
			encryptedData.CipherData = new CipherData(this.EncryptData(inputElement, symmetricAlgorithm, false));
			return encryptedData;
		}

		[MonoTODO]
		public EncryptedData Encrypt(XmlElement inputElement, X509Certificate2 certificate)
		{
			throw new NotImplementedException();
		}

		public byte[] EncryptData(byte[] plainText, SymmetricAlgorithm symAlg)
		{
			if (plainText == null)
			{
				throw new ArgumentNullException("plainText");
			}
			if (symAlg == null)
			{
				throw new ArgumentNullException("symAlg");
			}
			PaddingMode paddingMode = symAlg.Padding;
			byte[] array;
			try
			{
				symAlg.Padding = this.Padding;
				array = this.EncryptDataCore(plainText, symAlg);
			}
			finally
			{
				symAlg.Padding = paddingMode;
			}
			return array;
		}

		private byte[] EncryptDataCore(byte[] plainText, SymmetricAlgorithm symAlg)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(symAlg.IV);
			binaryWriter.Write(this.Transform(plainText, symAlg.CreateEncryptor()));
			binaryWriter.Flush();
			byte[] array = memoryStream.ToArray();
			binaryWriter.Close();
			memoryStream.Close();
			return array;
		}

		public byte[] EncryptData(XmlElement inputElement, SymmetricAlgorithm symAlg, bool content)
		{
			if (inputElement == null)
			{
				throw new ArgumentNullException("inputElement");
			}
			if (content)
			{
				return this.EncryptData(this.Encoding.GetBytes(inputElement.InnerXml), symAlg);
			}
			return this.EncryptData(this.Encoding.GetBytes(inputElement.OuterXml), symAlg);
		}

		public static byte[] EncryptKey(byte[] keyData, SymmetricAlgorithm symAlg)
		{
			if (keyData == null)
			{
				throw new ArgumentNullException("keyData");
			}
			if (symAlg == null)
			{
				throw new ArgumentNullException("symAlg");
			}
			if (symAlg is TripleDES)
			{
				return SymmetricKeyWrap.TripleDESKeyWrapEncrypt(symAlg.Key, keyData);
			}
			if (symAlg is Rijndael)
			{
				return SymmetricKeyWrap.AESKeyWrapEncrypt(symAlg.Key, keyData);
			}
			throw new CryptographicException("The specified cryptographic transform is not supported.");
		}

		[MonoTODO("Test this.")]
		public static byte[] EncryptKey(byte[] keyData, RSA rsa, bool fOAEP)
		{
			AsymmetricKeyExchangeFormatter asymmetricKeyExchangeFormatter;
			if (fOAEP)
			{
				asymmetricKeyExchangeFormatter = new RSAOAEPKeyExchangeFormatter(rsa);
			}
			else
			{
				asymmetricKeyExchangeFormatter = new RSAPKCS1KeyExchangeFormatter(rsa);
			}
			return asymmetricKeyExchangeFormatter.CreateKeyExchange(keyData);
		}

		private static SymmetricAlgorithm GetAlgorithm(string symAlgUri)
		{
			if (symAlgUri != null)
			{
				if (EncryptedXml.<>f__switch$map9 == null)
				{
					EncryptedXml.<>f__switch$map9 = new Dictionary<string, int>(9)
					{
						{ "http://www.w3.org/2001/04/xmlenc#aes128-cbc", 0 },
						{ "http://www.w3.org/2001/04/xmlenc#kw-aes128", 0 },
						{ "http://www.w3.org/2001/04/xmlenc#aes192-cbc", 1 },
						{ "http://www.w3.org/2001/04/xmlenc#kw-aes192", 1 },
						{ "http://www.w3.org/2001/04/xmlenc#aes256-cbc", 2 },
						{ "http://www.w3.org/2001/04/xmlenc#kw-aes256", 2 },
						{ "http://www.w3.org/2001/04/xmlenc#des-cbc", 3 },
						{ "http://www.w3.org/2001/04/xmlenc#tripledes-cbc", 4 },
						{ "http://www.w3.org/2001/04/xmlenc#kw-tripledes", 4 }
					};
				}
				int num;
				if (EncryptedXml.<>f__switch$map9.TryGetValue(symAlgUri, out num))
				{
					SymmetricAlgorithm symmetricAlgorithm;
					switch (num)
					{
					case 0:
						symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
						symmetricAlgorithm.KeySize = 128;
						break;
					case 1:
						symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
						symmetricAlgorithm.KeySize = 192;
						break;
					case 2:
						symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
						symmetricAlgorithm.KeySize = 256;
						break;
					case 3:
						symmetricAlgorithm = SymmetricAlgorithm.Create("DES");
						break;
					case 4:
						symmetricAlgorithm = SymmetricAlgorithm.Create("TripleDES");
						break;
					default:
						goto IL_0130;
					}
					return symmetricAlgorithm;
				}
			}
			IL_0130:
			throw new CryptographicException("symAlgUri");
		}

		private static string GetAlgorithmUri(SymmetricAlgorithm symAlg)
		{
			if (symAlg is Rijndael)
			{
				int keySize = symAlg.KeySize;
				if (keySize == 128)
				{
					return "http://www.w3.org/2001/04/xmlenc#aes128-cbc";
				}
				if (keySize == 192)
				{
					return "http://www.w3.org/2001/04/xmlenc#aes192-cbc";
				}
				if (keySize == 256)
				{
					return "http://www.w3.org/2001/04/xmlenc#aes256-cbc";
				}
			}
			else
			{
				if (symAlg is DES)
				{
					return "http://www.w3.org/2001/04/xmlenc#des-cbc";
				}
				if (symAlg is TripleDES)
				{
					return "http://www.w3.org/2001/04/xmlenc#tripledes-cbc";
				}
			}
			throw new ArgumentException("symAlg");
		}

		private static string GetKeyWrapAlgorithmUri(object keyAlg)
		{
			if (keyAlg is Rijndael)
			{
				int keySize = ((Rijndael)keyAlg).KeySize;
				if (keySize == 128)
				{
					return "http://www.w3.org/2001/04/xmlenc#kw-aes128";
				}
				if (keySize == 192)
				{
					return "http://www.w3.org/2001/04/xmlenc#kw-aes192";
				}
				if (keySize == 256)
				{
					return "http://www.w3.org/2001/04/xmlenc#kw-aes256";
				}
			}
			else
			{
				if (keyAlg is RSA)
				{
					return "http://www.w3.org/2001/04/xmlenc#rsa-1_5";
				}
				if (keyAlg is TripleDES)
				{
					return "http://www.w3.org/2001/04/xmlenc#kw-tripledes";
				}
			}
			throw new ArgumentException("keyAlg");
		}

		public virtual byte[] GetDecryptionIV(EncryptedData encryptedData, string symAlgUri)
		{
			if (encryptedData == null)
			{
				throw new ArgumentNullException("encryptedData");
			}
			SymmetricAlgorithm algorithm = EncryptedXml.GetAlgorithm(symAlgUri);
			byte[] array = new byte[algorithm.BlockSize / 8];
			Buffer.BlockCopy(encryptedData.CipherData.CipherValue, 0, array, 0, array.Length);
			return array;
		}

		public virtual SymmetricAlgorithm GetDecryptionKey(EncryptedData encryptedData, string symAlgUri)
		{
			if (encryptedData == null)
			{
				throw new ArgumentNullException("encryptedData");
			}
			if (symAlgUri == null)
			{
				return null;
			}
			SymmetricAlgorithm algorithm = EncryptedXml.GetAlgorithm(symAlgUri);
			algorithm.IV = this.GetDecryptionIV(encryptedData, encryptedData.EncryptionMethod.KeyAlgorithm);
			KeyInfo keyInfo = encryptedData.KeyInfo;
			foreach (object obj in keyInfo)
			{
				KeyInfoClause keyInfoClause = (KeyInfoClause)obj;
				if (keyInfoClause is KeyInfoEncryptedKey)
				{
					algorithm.Key = this.DecryptEncryptedKey(((KeyInfoEncryptedKey)keyInfoClause).EncryptedKey);
					break;
				}
			}
			return algorithm;
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
			XmlDocument ownerDocument = inputElement.OwnerDocument;
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(this.Encoding.GetString(decryptedData, 0, decryptedData.Length)));
			xmlTextReader.MoveToContent();
			XmlNode xmlNode = ownerDocument.ReadNode(xmlTextReader);
			inputElement.ParentNode.ReplaceChild(xmlNode, inputElement);
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
			XmlDocument ownerDocument = inputElement.OwnerDocument;
			inputElement.ParentNode.ReplaceChild(encryptedData.GetXml(ownerDocument), inputElement);
		}

		private byte[] Transform(byte[] data, ICryptoTransform transform)
		{
			return this.Transform(data, transform, 0, false);
		}

		private byte[] Transform(byte[] data, ICryptoTransform transform, int blockOctetCount, bool trimPadding)
		{
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.FlushFinalBlock();
			int num = 0;
			checked
			{
				if (trimPadding)
				{
					num = (int)memoryStream.GetBuffer()[(int)((IntPtr)(unchecked(memoryStream.Length - 1L)))];
				}
				if (num > blockOctetCount)
				{
					num = 0;
				}
			}
			byte[] array = new byte[memoryStream.Length - (long)blockOctetCount - (long)num];
			Array.Copy(memoryStream.GetBuffer(), blockOctetCount, array, 0, array.Length);
			cryptoStream.Close();
			memoryStream.Close();
			return array;
		}

		public const string XmlEncAES128KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes128";

		public const string XmlEncAES128Url = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";

		public const string XmlEncAES192KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes192";

		public const string XmlEncAES192Url = "http://www.w3.org/2001/04/xmlenc#aes192-cbc";

		public const string XmlEncAES256KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes256";

		public const string XmlEncAES256Url = "http://www.w3.org/2001/04/xmlenc#aes256-cbc";

		public const string XmlEncDESUrl = "http://www.w3.org/2001/04/xmlenc#des-cbc";

		public const string XmlEncElementContentUrl = "http://www.w3.org/2001/04/xmlenc#Content";

		public const string XmlEncElementUrl = "http://www.w3.org/2001/04/xmlenc#Element";

		public const string XmlEncEncryptedKeyUrl = "http://www.w3.org/2001/04/xmlenc#EncryptedKey";

		public const string XmlEncNamespaceUrl = "http://www.w3.org/2001/04/xmlenc#";

		public const string XmlEncRSA15Url = "http://www.w3.org/2001/04/xmlenc#rsa-1_5";

		public const string XmlEncRSAOAEPUrl = "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p";

		public const string XmlEncSHA256Url = "http://www.w3.org/2001/04/xmlenc#sha256";

		public const string XmlEncSHA512Url = "http://www.w3.org/2001/04/xmlenc#sha512";

		public const string XmlEncTripleDESKeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-tripledes";

		public const string XmlEncTripleDESUrl = "http://www.w3.org/2001/04/xmlenc#tripledes-cbc";

		private Evidence documentEvidence;

		private Encoding encoding = Encoding.UTF8;

		internal Hashtable keyNameMapping = new Hashtable();

		private CipherMode mode = CipherMode.CBC;

		private PaddingMode padding = PaddingMode.ISO10126;

		private string recipient;

		private XmlResolver resolver;

		private XmlDocument document;
	}
}
