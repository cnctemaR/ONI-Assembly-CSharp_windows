using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class TransformChain
	{
		public TransformChain()
		{
			this._transforms = new ArrayList();
		}

		public void Add(Transform transform)
		{
			if (transform != null)
			{
				this._transforms.Add(transform);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this._transforms.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this._transforms.Count;
			}
		}

		public Transform this[int index]
		{
			get
			{
				if (index >= this._transforms.Count)
				{
					throw new ArgumentException("Index was out of range. Must be non-negative and less than the size of the collection.", "index");
				}
				return (Transform)this._transforms[index];
			}
		}

		internal Stream TransformToOctetStream(object inputObject, Type inputType, XmlResolver resolver, string baseUri)
		{
			object obj = inputObject;
			foreach (object obj2 in this._transforms)
			{
				Transform transform = (Transform)obj2;
				if (obj == null || transform.AcceptsType(obj.GetType()))
				{
					transform.Resolver = resolver;
					transform.BaseURI = baseUri;
					transform.LoadInput(obj);
					obj = transform.GetOutput();
				}
				else if (obj is Stream)
				{
					if (!transform.AcceptsType(typeof(XmlDocument)))
					{
						throw new CryptographicException("The input type was invalid for this transform.");
					}
					Stream stream = obj as Stream;
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.PreserveWhitespace = true;
					XmlReader xmlReader = Utils.PreProcessStreamInput(stream, resolver, baseUri);
					xmlDocument.Load(xmlReader);
					transform.LoadInput(xmlDocument);
					stream.Close();
					obj = transform.GetOutput();
				}
				else if (obj is XmlNodeList)
				{
					if (!transform.AcceptsType(typeof(Stream)))
					{
						throw new CryptographicException("The input type was invalid for this transform.");
					}
					MemoryStream memoryStream = new MemoryStream(new CanonicalXml((XmlNodeList)obj, resolver, false).GetBytes());
					transform.LoadInput(memoryStream);
					obj = transform.GetOutput();
					memoryStream.Close();
				}
				else
				{
					if (!(obj is XmlDocument))
					{
						throw new CryptographicException("The input type was invalid for this transform.");
					}
					if (!transform.AcceptsType(typeof(Stream)))
					{
						throw new CryptographicException("The input type was invalid for this transform.");
					}
					MemoryStream memoryStream2 = new MemoryStream(new CanonicalXml((XmlDocument)obj, resolver).GetBytes());
					transform.LoadInput(memoryStream2);
					obj = transform.GetOutput();
					memoryStream2.Close();
				}
			}
			if (obj is Stream)
			{
				return obj as Stream;
			}
			if (obj is XmlNodeList)
			{
				return new MemoryStream(new CanonicalXml((XmlNodeList)obj, resolver, false).GetBytes());
			}
			if (obj is XmlDocument)
			{
				return new MemoryStream(new CanonicalXml((XmlDocument)obj, resolver).GetBytes());
			}
			throw new CryptographicException("The input type was invalid for this transform.");
		}

		internal Stream TransformToOctetStream(Stream input, XmlResolver resolver, string baseUri)
		{
			return this.TransformToOctetStream(input, typeof(Stream), resolver, baseUri);
		}

		internal Stream TransformToOctetStream(XmlDocument document, XmlResolver resolver, string baseUri)
		{
			return this.TransformToOctetStream(document, typeof(XmlDocument), resolver, baseUri);
		}

		internal XmlElement GetXml(XmlDocument document, string ns)
		{
			XmlElement xmlElement = document.CreateElement("Transforms", ns);
			foreach (object obj in this._transforms)
			{
				Transform transform = (Transform)obj;
				if (transform != null)
				{
					XmlElement xml = transform.GetXml(document);
					if (xml != null)
					{
						xmlElement.AppendChild(xml);
					}
				}
			}
			return xmlElement;
		}

		internal void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(value.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
			XmlNodeList xmlNodeList = value.SelectNodes("ds:Transform", xmlNamespaceManager);
			if (xmlNodeList.Count == 0)
			{
				throw new CryptographicException("Malformed element {0}.", "Transforms");
			}
			this._transforms.Clear();
			for (int i = 0; i < xmlNodeList.Count; i++)
			{
				XmlElement xmlElement = (XmlElement)xmlNodeList.Item(i);
				Transform transform = CryptoHelpers.CreateFromName<Transform>(Utils.GetAttribute(xmlElement, "Algorithm", "http://www.w3.org/2000/09/xmldsig#"));
				if (transform == null)
				{
					throw new CryptographicException("Unknown transform has been encountered.");
				}
				transform.LoadInnerXml(xmlElement.ChildNodes);
				this._transforms.Add(transform);
			}
		}

		private ArrayList _transforms;
	}
}
