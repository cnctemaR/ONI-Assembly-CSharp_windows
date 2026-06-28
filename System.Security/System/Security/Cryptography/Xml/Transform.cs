using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public abstract class Transform
	{
		protected Transform()
		{
			if (SecurityManager.SecurityEnabled)
			{
				this.xmlResolver = new XmlSecureResolver(new XmlUrlResolver(), new Evidence());
			}
			else
			{
				this.xmlResolver = new XmlUrlResolver();
			}
		}

		public string Algorithm
		{
			get
			{
				return this.algo;
			}
			set
			{
				this.algo = value;
			}
		}

		public abstract Type[] InputTypes { get; }

		public abstract Type[] OutputTypes { get; }

		[ComVisible(false)]
		public XmlResolver Resolver
		{
			set
			{
				this.xmlResolver = value;
			}
		}

		[ComVisible(false)]
		[MonoTODO]
		public XmlElement Context
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[ComVisible(false)]
		public Hashtable PropagatedNamespaces
		{
			get
			{
				return this.propagated_namespaces;
			}
		}

		[ComVisible(false)]
		public virtual byte[] GetDigestedOutput(HashAlgorithm hash)
		{
			return hash.ComputeHash((Stream)this.GetOutput(typeof(Stream)));
		}

		protected abstract XmlNodeList GetInnerXml();

		public abstract object GetOutput();

		public abstract object GetOutput(Type type);

		public XmlElement GetXml()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.XmlResolver = this.GetResolver();
			XmlElement xmlElement = xmlDocument.CreateElement("Transform", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement.SetAttribute("Algorithm", this.algo);
			XmlNodeList innerXml = this.GetInnerXml();
			if (innerXml != null)
			{
				foreach (object obj in innerXml)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNode xmlNode2 = xmlDocument.ImportNode(xmlNode, true);
					xmlElement.AppendChild(xmlNode2);
				}
			}
			return xmlElement;
		}

		public abstract void LoadInnerXml(XmlNodeList nodeList);

		public abstract void LoadInput(object obj);

		internal XmlResolver GetResolver()
		{
			return this.xmlResolver;
		}

		private string algo;

		private XmlResolver xmlResolver;

		private Hashtable propagated_namespaces = new Hashtable();
	}
}
