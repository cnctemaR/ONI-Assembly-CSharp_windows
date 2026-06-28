using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace System.Security.Cryptography.Xml
{
	public class XmlDsigXsltTransform : Transform
	{
		public XmlDsigXsltTransform()
			: this(false)
		{
		}

		public XmlDsigXsltTransform(bool includeComments)
		{
			this.comments = includeComments;
			base.Algorithm = "http://www.w3.org/TR/1999/REC-xslt-19991116";
		}

		public override Type[] InputTypes
		{
			get
			{
				if (this.input == null)
				{
					this.input = new Type[3];
					this.input[0] = typeof(Stream);
					this.input[1] = typeof(XmlDocument);
					this.input[2] = typeof(XmlNodeList);
				}
				return this.input;
			}
		}

		public override Type[] OutputTypes
		{
			get
			{
				if (this.output == null)
				{
					this.output = new Type[1];
					this.output[0] = typeof(Stream);
				}
				return this.output;
			}
		}

		protected override XmlNodeList GetInnerXml()
		{
			return this.xnl;
		}

		public override object GetOutput()
		{
			if (this.xnl == null)
			{
				throw new ArgumentNullException("LoadInnerXml before transformation.");
			}
			XmlResolver resolver = base.GetResolver();
			XslTransform xslTransform = new XslTransform();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.XmlResolver = resolver;
			foreach (object obj in this.xnl)
			{
				XmlNode xmlNode = (XmlNode)obj;
				xmlDocument.AppendChild(xmlDocument.ImportNode(xmlNode, true));
			}
			xslTransform.Load(xmlDocument, resolver);
			if (this.inputDoc == null)
			{
				throw new ArgumentNullException("LoadInput before transformation.");
			}
			MemoryStream memoryStream = new MemoryStream();
			xslTransform.XmlResolver = resolver;
			xslTransform.Transform(this.inputDoc, null, memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream;
		}

		public override object GetOutput(Type type)
		{
			if (type != typeof(Stream))
			{
				throw new ArgumentException("type");
			}
			return this.GetOutput();
		}

		public override void LoadInnerXml(XmlNodeList nodeList)
		{
			if (nodeList == null)
			{
				throw new CryptographicException("nodeList");
			}
			this.xnl = nodeList;
		}

		public override void LoadInput(object obj)
		{
			Stream stream = obj as Stream;
			if (stream != null)
			{
				this.inputDoc = new XmlDocument();
				this.inputDoc.XmlResolver = base.GetResolver();
				this.inputDoc.Load(new XmlSignatureStreamReader(new StreamReader(stream)));
				return;
			}
			XmlDocument xmlDocument = obj as XmlDocument;
			if (xmlDocument != null)
			{
				this.inputDoc = xmlDocument;
				return;
			}
			XmlNodeList xmlNodeList = obj as XmlNodeList;
			if (xmlNodeList != null)
			{
				this.inputDoc = new XmlDocument();
				this.inputDoc.XmlResolver = base.GetResolver();
				for (int i = 0; i < xmlNodeList.Count; i++)
				{
					this.inputDoc.AppendChild(this.inputDoc.ImportNode(xmlNodeList[i], true));
				}
			}
		}

		private Type[] input;

		private Type[] output;

		private bool comments;

		private XmlNodeList xnl;

		private XmlDocument inputDoc;
	}
}
