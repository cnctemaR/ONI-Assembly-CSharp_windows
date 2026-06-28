using System;
using System.IO;
using System.Xml;
using Mono.Xml;

namespace System.Security.Cryptography.Xml
{
	public class XmlDsigExcC14NTransform : Transform
	{
		public XmlDsigExcC14NTransform()
			: this(false, null)
		{
		}

		public XmlDsigExcC14NTransform(bool includeComments)
			: this(includeComments, null)
		{
		}

		public XmlDsigExcC14NTransform(string inclusiveNamespacesPrefixList)
			: this(false, inclusiveNamespacesPrefixList)
		{
		}

		public XmlDsigExcC14NTransform(bool includeComments, string inclusiveNamespacesPrefixList)
		{
			if (includeComments)
			{
				base.Algorithm = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";
			}
			else
			{
				base.Algorithm = "http://www.w3.org/2001/10/xml-exc-c14n#";
			}
			this.inclusiveNamespacesPrefixList = inclusiveNamespacesPrefixList;
			this.canonicalizer = new XmlCanonicalizer(includeComments, true, base.PropagatedNamespaces);
		}

		public string InclusiveNamespacesPrefixList
		{
			get
			{
				return this.inclusiveNamespacesPrefixList;
			}
			set
			{
				this.inclusiveNamespacesPrefixList = value;
			}
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
			this.canonicalizer.InclusiveNamespacesPrefixList = this.InclusiveNamespacesPrefixList;
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

		private string inclusiveNamespacesPrefixList;
	}
}
