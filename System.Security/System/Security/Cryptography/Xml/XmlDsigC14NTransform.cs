using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using Mono.Xml;

namespace System.Security.Cryptography.Xml
{
	public class XmlDsigC14NTransform : Transform
	{
		public XmlDsigC14NTransform()
			: this(false)
		{
		}

		public XmlDsigC14NTransform(bool includeComments)
		{
			if (includeComments)
			{
				base.Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";
			}
			else
			{
				base.Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
			}
			this.canonicalizer = new XmlCanonicalizer(includeComments, false, base.PropagatedNamespaces);
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

		[ComVisible(false)]
		public override byte[] GetDigestedOutput(HashAlgorithm hash)
		{
			return hash.ComputeHash((Stream)this.GetOutput());
		}

		public override object GetOutput()
		{
			return this.s;
		}

		public override object GetOutput(Type type)
		{
			if (type == typeof(Stream))
			{
				return this.GetOutput();
			}
			throw new ArgumentException("type");
		}

		public override void LoadInnerXml(XmlNodeList nodeList)
		{
		}

		public override void LoadInput(object obj)
		{
			Stream stream = obj as Stream;
			if (stream != null)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.PreserveWhitespace = true;
				xmlDocument.XmlResolver = base.GetResolver();
				xmlDocument.Load(new XmlSignatureStreamReader(new StreamReader(stream)));
				this.s = this.canonicalizer.Canonicalize(xmlDocument);
				return;
			}
			XmlDocument xmlDocument2 = obj as XmlDocument;
			if (xmlDocument2 != null)
			{
				this.s = this.canonicalizer.Canonicalize(xmlDocument2);
				return;
			}
			XmlNodeList xmlNodeList = obj as XmlNodeList;
			if (xmlNodeList != null)
			{
				this.s = this.canonicalizer.Canonicalize(xmlNodeList);
				return;
			}
			throw new ArgumentException("obj");
		}

		private Type[] input;

		private Type[] output;

		private XmlCanonicalizer canonicalizer;

		private Stream s;
	}
}
