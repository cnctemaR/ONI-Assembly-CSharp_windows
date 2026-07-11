using System;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class XmlDsigBase64Transform : Transform
	{
		public XmlDsigBase64Transform()
		{
			base.Algorithm = "http://www.w3.org/2000/09/xmldsig#base64";
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
			return null;
		}

		public override object GetOutput()
		{
			return this.cs;
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
		}

		public override void LoadInput(object obj)
		{
			XmlNodeList xmlNodeList = null;
			Stream stream = null;
			if (obj is Stream)
			{
				stream = obj as Stream;
			}
			else if (obj is XmlDocument)
			{
				xmlNodeList = (obj as XmlDocument).SelectNodes("//.");
			}
			else if (obj is XmlNodeList)
			{
				xmlNodeList = (XmlNodeList)obj;
			}
			if (xmlNodeList != null)
			{
				stream = new MemoryStream();
				StreamWriter streamWriter = new StreamWriter(stream);
				foreach (object obj2 in xmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj2;
					XmlNodeType nodeType = xmlNode.NodeType;
					switch (nodeType)
					{
					case XmlNodeType.Attribute:
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
						break;
					default:
						if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.SignificantWhitespace)
						{
							continue;
						}
						break;
					}
					streamWriter.Write(xmlNode.Value);
				}
				streamWriter.Flush();
				stream.Position = 0L;
			}
			if (stream != null)
			{
				this.cs = new CryptoStream(stream, new FromBase64Transform(), CryptoStreamMode.Read);
			}
		}

		private CryptoStream cs;

		private Type[] input;

		private Type[] output;
	}
}
