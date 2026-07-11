using System;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class XmlDsigC14NTransform : Transform
	{
		public XmlDsigC14NTransform()
		{
			base.Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
		}

		public XmlDsigC14NTransform(bool includeComments)
		{
			this._includeComments = includeComments;
			base.Algorithm = (includeComments ? "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments" : "http://www.w3.org/TR/2001/REC-xml-c14n-20010315");
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
		}

		protected override XmlNodeList GetInnerXml()
		{
			return null;
		}

		public override void LoadInput(object obj)
		{
			XmlResolver xmlResolver = (base.ResolverSet ? this._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), base.BaseURI));
			if (obj is Stream)
			{
				this._cXml = new CanonicalXml((Stream)obj, this._includeComments, xmlResolver, base.BaseURI);
				return;
			}
			if (obj is XmlDocument)
			{
				this._cXml = new CanonicalXml((XmlDocument)obj, xmlResolver, this._includeComments);
				return;
			}
			if (obj is XmlNodeList)
			{
				this._cXml = new CanonicalXml((XmlNodeList)obj, xmlResolver, this._includeComments);
				return;
			}
			throw new ArgumentException("Type of input object is invalid.", "obj");
		}

		public override object GetOutput()
		{
			return new MemoryStream(this._cXml.GetBytes());
		}

		public override object GetOutput(Type type)
		{
			if (type != typeof(Stream) && !type.IsSubclassOf(typeof(Stream)))
			{
				throw new ArgumentException("The input type was invalid for this transform.", "type");
			}
			return new MemoryStream(this._cXml.GetBytes());
		}

		public override byte[] GetDigestedOutput(HashAlgorithm hash)
		{
			return this._cXml.GetDigestedBytes(hash);
		}

		private Type[] _inputTypes = new Type[]
		{
			typeof(Stream),
			typeof(XmlDocument),
			typeof(XmlNodeList)
		};

		private Type[] _outputTypes = new Type[] { typeof(Stream) };

		private CanonicalXml _cXml;

		private bool _includeComments;
	}
}
