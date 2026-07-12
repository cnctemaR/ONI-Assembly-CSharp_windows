using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace System.Security.Cryptography.Xml
{
	public class XmlDsigXsltTransform : Transform
	{
		public XmlDsigXsltTransform()
		{
			base.Algorithm = "http://www.w3.org/TR/1999/REC-xslt-19991116";
		}

		public XmlDsigXsltTransform(bool includeComments)
		{
			this._includeComments = includeComments;
			base.Algorithm = "http://www.w3.org/TR/1999/REC-xslt-19991116";
		}

		public override Type[] InputTypes
		{
			get
			{
				return this._inputTypes;
			}
		}

		public override Type[] OutputTypes
		{
			get
			{
				return this._outputTypes;
			}
		}

		public override void LoadInnerXml(XmlNodeList nodeList)
		{
			if (nodeList == null)
			{
				throw new CryptographicException("Unknown transform has been encountered.");
			}
			XmlElement xmlElement = null;
			int num = 0;
			foreach (object obj in nodeList)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlWhitespace))
				{
					if (xmlNode is XmlElement)
					{
						if (num != 0)
						{
							throw new CryptographicException("Unknown transform has been encountered.");
						}
						xmlElement = xmlNode as XmlElement;
						num++;
					}
					else
					{
						num++;
					}
				}
			}
			if (num != 1 || xmlElement == null)
			{
				throw new CryptographicException("Unknown transform has been encountered.");
			}
			this._xslNodes = nodeList;
			this._xslFragment = xmlElement.OuterXml.Trim(null);
		}

		protected override XmlNodeList GetInnerXml()
		{
			return this._xslNodes;
		}

		public override void LoadInput(object obj)
		{
			if (this._inputStream != null)
			{
				this._inputStream.Close();
			}
			this._inputStream = new MemoryStream();
			if (obj is Stream)
			{
				this._inputStream = (Stream)obj;
				return;
			}
			if (!(obj is XmlNodeList))
			{
				if (obj is XmlDocument)
				{
					byte[] bytes = new CanonicalXml((XmlDocument)obj, null, this._includeComments).GetBytes();
					if (bytes == null)
					{
						return;
					}
					this._inputStream.Write(bytes, 0, bytes.Length);
					this._inputStream.Flush();
					this._inputStream.Position = 0L;
				}
				return;
			}
			byte[] bytes2 = new CanonicalXml((XmlNodeList)obj, null, this._includeComments).GetBytes();
			if (bytes2 == null)
			{
				return;
			}
			this._inputStream.Write(bytes2, 0, bytes2.Length);
			this._inputStream.Flush();
			this._inputStream.Position = 0L;
		}

		public override object GetOutput()
		{
			XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.XmlResolver = null;
			xmlReaderSettings.MaxCharactersFromEntities = 10000000L;
			xmlReaderSettings.MaxCharactersInDocument = 0L;
			object obj;
			using (StringReader stringReader = new StringReader(this._xslFragment))
			{
				XmlReader xmlReader = XmlReader.Create(stringReader, xmlReaderSettings, null);
				xslCompiledTransform.Load(xmlReader, XsltSettings.Default, null);
				XPathDocument xpathDocument = new XPathDocument(XmlReader.Create(this._inputStream, xmlReaderSettings, base.BaseURI), XmlSpace.Preserve);
				MemoryStream memoryStream = new MemoryStream();
				XmlWriter xmlWriter = new XmlTextWriter(memoryStream, null);
				xslCompiledTransform.Transform(xpathDocument, null, xmlWriter);
				memoryStream.Position = 0L;
				obj = memoryStream;
			}
			return obj;
		}

		public override object GetOutput(Type type)
		{
			if (type != typeof(Stream) && !type.IsSubclassOf(typeof(Stream)))
			{
				throw new ArgumentException("The input type was invalid for this transform.", "type");
			}
			return (Stream)this.GetOutput();
		}

		private Type[] _inputTypes = new Type[]
		{
			typeof(Stream),
			typeof(XmlDocument),
			typeof(XmlNodeList)
		};

		private Type[] _outputTypes = new Type[] { typeof(Stream) };

		private XmlNodeList _xslNodes;

		private string _xslFragment;

		private Stream _inputStream;

		private bool _includeComments;
	}
}
