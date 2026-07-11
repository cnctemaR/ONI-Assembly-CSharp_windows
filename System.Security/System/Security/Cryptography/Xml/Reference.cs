using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class Reference
	{
		public Reference()
		{
			this._transformChain = new TransformChain();
			this._refTarget = null;
			this._refTargetType = ReferenceTargetType.UriReference;
			this._cachedXml = null;
			this._digestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
		}

		public Reference(Stream stream)
		{
			this._transformChain = new TransformChain();
			this._refTarget = stream;
			this._refTargetType = ReferenceTargetType.Stream;
			this._cachedXml = null;
			this._digestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
		}

		public Reference(string uri)
		{
			this._transformChain = new TransformChain();
			this._refTarget = uri;
			this._uri = uri;
			this._refTargetType = ReferenceTargetType.UriReference;
			this._cachedXml = null;
			this._digestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
		}

		internal Reference(XmlElement element)
		{
			this._transformChain = new TransformChain();
			this._refTarget = element;
			this._refTargetType = ReferenceTargetType.XmlElement;
			this._cachedXml = null;
			this._digestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
		}

		public string Id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		public string Uri
		{
			get
			{
				return this._uri;
			}
			set
			{
				this._uri = value;
				this._cachedXml = null;
			}
		}

		public string Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
				this._cachedXml = null;
			}
		}

		public string DigestMethod
		{
			get
			{
				return this._digestMethod;
			}
			set
			{
				this._digestMethod = value;
				this._cachedXml = null;
			}
		}

		public byte[] DigestValue
		{
			get
			{
				return this._digestValue;
			}
			set
			{
				this._digestValue = value;
				this._cachedXml = null;
			}
		}

		public TransformChain TransformChain
		{
			get
			{
				if (this._transformChain == null)
				{
					this._transformChain = new TransformChain();
				}
				return this._transformChain;
			}
			set
			{
				this._transformChain = value;
				this._cachedXml = null;
			}
		}

		internal bool CacheValid
		{
			get
			{
				return this._cachedXml != null;
			}
		}

		internal SignedXml SignedXml
		{
			get
			{
				return this._signedXml;
			}
			set
			{
				this._signedXml = value;
			}
		}

		internal ReferenceTargetType ReferenceTargetType
		{
			get
			{
				return this._refTargetType;
			}
		}

		public XmlElement GetXml()
		{
			if (this.CacheValid)
			{
				return this._cachedXml;
			}
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			XmlElement xmlElement = document.CreateElement("Reference", "http://www.w3.org/2000/09/xmldsig#");
			if (!string.IsNullOrEmpty(this._id))
			{
				xmlElement.SetAttribute("Id", this._id);
			}
			if (this._uri != null)
			{
				xmlElement.SetAttribute("URI", this._uri);
			}
			if (!string.IsNullOrEmpty(this._type))
			{
				xmlElement.SetAttribute("Type", this._type);
			}
			if (this.TransformChain.Count != 0)
			{
				xmlElement.AppendChild(this.TransformChain.GetXml(document, "http://www.w3.org/2000/09/xmldsig#"));
			}
			if (string.IsNullOrEmpty(this._digestMethod))
			{
				throw new CryptographicException("A DigestMethod must be specified on a Reference prior to generating XML.");
			}
			XmlElement xmlElement2 = document.CreateElement("DigestMethod", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement2.SetAttribute("Algorithm", this._digestMethod);
			xmlElement.AppendChild(xmlElement2);
			if (this.DigestValue == null)
			{
				if (this._hashAlgorithm.Hash == null)
				{
					throw new CryptographicException("A Reference must contain a DigestValue.");
				}
				this.DigestValue = this._hashAlgorithm.Hash;
			}
			XmlElement xmlElement3 = document.CreateElement("DigestValue", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement3.AppendChild(document.CreateTextNode(Convert.ToBase64String(this._digestValue)));
			xmlElement.AppendChild(xmlElement3);
			return xmlElement;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this._id = Utils.GetAttribute(value, "Id", "http://www.w3.org/2000/09/xmldsig#");
			this._uri = Utils.GetAttribute(value, "URI", "http://www.w3.org/2000/09/xmldsig#");
			this._type = Utils.GetAttribute(value, "Type", "http://www.w3.org/2000/09/xmldsig#");
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(value.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
			this.TransformChain = new TransformChain();
			XmlElement xmlElement = value.SelectSingleNode("ds:Transforms", xmlNamespaceManager) as XmlElement;
			if (xmlElement != null)
			{
				XmlNodeList xmlNodeList = xmlElement.SelectNodes("ds:Transform", xmlNamespaceManager);
				if (xmlNodeList != null)
				{
					foreach (object obj in xmlNodeList)
					{
						XmlElement xmlElement2 = ((XmlNode)obj) as XmlElement;
						Transform transform = CryptoHelpers.CreateFromName(Utils.GetAttribute(xmlElement2, "Algorithm", "http://www.w3.org/2000/09/xmldsig#")) as Transform;
						if (transform == null)
						{
							throw new CryptographicException("Unknown transform has been encountered.");
						}
						this.AddTransform(transform);
						transform.LoadInnerXml(xmlElement2.ChildNodes);
						if (transform is XmlDsigEnvelopedSignatureTransform)
						{
							XmlNode xmlNode = xmlElement2.SelectSingleNode("ancestor::ds:Signature[1]", xmlNamespaceManager);
							XmlNodeList xmlNodeList2 = xmlElement2.SelectNodes("//ds:Signature", xmlNamespaceManager);
							if (xmlNodeList2 != null)
							{
								int num = 0;
								foreach (object obj2 in xmlNodeList2)
								{
									XmlNode xmlNode2 = (XmlNode)obj2;
									num++;
									if (xmlNode2 == xmlNode)
									{
										((XmlDsigEnvelopedSignatureTransform)transform).SignaturePosition = num;
										break;
									}
								}
							}
						}
					}
				}
			}
			XmlElement xmlElement3 = value.SelectSingleNode("ds:DigestMethod", xmlNamespaceManager) as XmlElement;
			if (xmlElement3 == null)
			{
				throw new CryptographicException("Malformed element {0}.", "Reference/DigestMethod");
			}
			this._digestMethod = Utils.GetAttribute(xmlElement3, "Algorithm", "http://www.w3.org/2000/09/xmldsig#");
			XmlElement xmlElement4 = value.SelectSingleNode("ds:DigestValue", xmlNamespaceManager) as XmlElement;
			if (xmlElement4 == null)
			{
				throw new CryptographicException("Malformed element {0}.", "Reference/DigestValue");
			}
			this._digestValue = Convert.FromBase64String(Utils.DiscardWhiteSpaces(xmlElement4.InnerText));
			this._cachedXml = value;
		}

		public void AddTransform(Transform transform)
		{
			if (transform == null)
			{
				throw new ArgumentNullException("transform");
			}
			transform.Reference = this;
			this.TransformChain.Add(transform);
		}

		internal void UpdateHashValue(XmlDocument document, CanonicalXmlNodeList refList)
		{
			this.DigestValue = this.CalculateHashValue(document, refList);
		}

		internal byte[] CalculateHashValue(XmlDocument document, CanonicalXmlNodeList refList)
		{
			this._hashAlgorithm = CryptoHelpers.CreateFromName(this._digestMethod) as HashAlgorithm;
			if (this._hashAlgorithm == null)
			{
				throw new CryptographicException("Could not create hash algorithm object.");
			}
			string text = ((document == null) ? (Environment.CurrentDirectory + "\\") : document.BaseURI);
			Stream stream = null;
			WebResponse webResponse = null;
			Stream stream2 = null;
			XmlResolver xmlResolver = null;
			byte[] array = null;
			try
			{
				switch (this._refTargetType)
				{
				case ReferenceTargetType.Stream:
					xmlResolver = (this.SignedXml.ResolverSet ? this.SignedXml._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
					stream = this.TransformChain.TransformToOctetStream((Stream)this._refTarget, xmlResolver, text);
					break;
				case ReferenceTargetType.XmlElement:
					xmlResolver = (this.SignedXml.ResolverSet ? this.SignedXml._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
					stream = this.TransformChain.TransformToOctetStream(Utils.PreProcessElementInput((XmlElement)this._refTarget, xmlResolver, text), xmlResolver, text);
					break;
				case ReferenceTargetType.UriReference:
					if (this._uri == null)
					{
						xmlResolver = (this.SignedXml.ResolverSet ? this.SignedXml._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
						stream = this.TransformChain.TransformToOctetStream(null, xmlResolver, text);
					}
					else if (this._uri.Length == 0)
					{
						if (document == null)
						{
							throw new CryptographicException(string.Format(CultureInfo.CurrentCulture, "An XmlDocument context is required to resolve the Reference Uri {0}.", this._uri));
						}
						xmlResolver = (this.SignedXml.ResolverSet ? this.SignedXml._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
						XmlDocument xmlDocument = Utils.DiscardComments(Utils.PreProcessDocumentInput(document, xmlResolver, text));
						stream = this.TransformChain.TransformToOctetStream(xmlDocument, xmlResolver, text);
					}
					else
					{
						if (this._uri[0] != '#')
						{
							throw new CryptographicException("Unable to resolve Uri {0}.", this._uri);
						}
						bool flag = true;
						string idFromLocalUri = Utils.GetIdFromLocalUri(this._uri, out flag);
						if (idFromLocalUri == "xpointer(/)")
						{
							if (document == null)
							{
								throw new CryptographicException(string.Format(CultureInfo.CurrentCulture, "An XmlDocument context is required to resolve the Reference Uri {0}.", this._uri));
							}
							xmlResolver = (this.SignedXml.ResolverSet ? this.SignedXml._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
							stream = this.TransformChain.TransformToOctetStream(Utils.PreProcessDocumentInput(document, xmlResolver, text), xmlResolver, text);
						}
						else
						{
							XmlElement xmlElement = this.SignedXml.GetIdElement(document, idFromLocalUri);
							if (xmlElement != null)
							{
								this._namespaces = Utils.GetPropagatedAttributes(xmlElement.ParentNode as XmlElement);
							}
							if (xmlElement == null && refList != null)
							{
								foreach (object obj in refList)
								{
									XmlElement xmlElement2 = ((XmlNode)obj) as XmlElement;
									if (xmlElement2 != null && Utils.HasAttribute(xmlElement2, "Id", "http://www.w3.org/2000/09/xmldsig#") && Utils.GetAttribute(xmlElement2, "Id", "http://www.w3.org/2000/09/xmldsig#").Equals(idFromLocalUri))
									{
										xmlElement = xmlElement2;
										if (this._signedXml._context != null)
										{
											this._namespaces = Utils.GetPropagatedAttributes(this._signedXml._context);
											break;
										}
										break;
									}
								}
							}
							if (xmlElement == null)
							{
								throw new CryptographicException("Malformed reference element.");
							}
							XmlDocument xmlDocument2 = Utils.PreProcessElementInput(xmlElement, xmlResolver, text);
							Utils.AddNamespaces(xmlDocument2.DocumentElement, this._namespaces);
							xmlResolver = (this.SignedXml.ResolverSet ? this.SignedXml._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text));
							if (flag)
							{
								XmlDocument xmlDocument3 = Utils.DiscardComments(xmlDocument2);
								stream = this.TransformChain.TransformToOctetStream(xmlDocument3, xmlResolver, text);
							}
							else
							{
								stream = this.TransformChain.TransformToOctetStream(xmlDocument2, xmlResolver, text);
							}
						}
					}
					break;
				default:
					throw new CryptographicException("Unable to resolve Uri {0}.", this._uri);
				}
				stream = SignedXmlDebugLog.LogReferenceData(this, stream);
				array = this._hashAlgorithm.ComputeHash(stream);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
				if (webResponse != null)
				{
					webResponse.Close();
				}
				if (stream2 != null)
				{
					stream2.Close();
				}
			}
			return array;
		}

		internal const string DefaultDigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";

		private string _id;

		private string _uri;

		private string _type;

		private TransformChain _transformChain;

		private string _digestMethod;

		private byte[] _digestValue;

		private HashAlgorithm _hashAlgorithm;

		private object _refTarget;

		private ReferenceTargetType _refTargetType;

		private XmlElement _cachedXml;

		private SignedXml _signedXml;

		internal CanonicalXmlNodeList _namespaces;
	}
}
