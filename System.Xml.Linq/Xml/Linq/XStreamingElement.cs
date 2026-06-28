using System;
using System.Collections.Generic;
using System.IO;

namespace System.Xml.Linq
{
	public class XStreamingElement
	{
		public XStreamingElement(XName name)
		{
			this.Name = name;
		}

		public XStreamingElement(XName name, object content)
			: this(name)
		{
			this.Add(content);
		}

		public XStreamingElement(XName name, params object[] content)
			: this(name)
		{
			this.Add(content);
		}

		public XName Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal IEnumerable<object> Contents
		{
			get
			{
				return this.contents;
			}
		}

		public void Add(object content)
		{
			if (this.contents == null)
			{
				this.contents = new List<object>();
			}
			this.contents.Add(content);
		}

		public void Add(params object[] content)
		{
			if (this.contents == null)
			{
				this.contents = new List<object>();
			}
			this.contents.Add(content);
		}

		public void Save(string fileName)
		{
			using (TextWriter textWriter = File.CreateText(fileName))
			{
				this.Save(textWriter);
			}
		}

		public void Save(TextWriter textWriter)
		{
			this.Save(textWriter, SaveOptions.None);
		}

		public void Save(XmlWriter writer)
		{
			this.WriteTo(writer);
		}

		public void Save(string fileName, SaveOptions options)
		{
			using (TextWriter textWriter = File.CreateText(fileName))
			{
				this.Save(textWriter, options);
			}
		}

		public void Save(TextWriter textWriter, SaveOptions options)
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = (options != SaveOptions.DisableFormatting)
			}))
			{
				this.Save(xmlWriter);
			}
		}

		public override string ToString()
		{
			return this.ToString(SaveOptions.None);
		}

		public string ToString(SaveOptions options)
		{
			StringWriter stringWriter = new StringWriter();
			this.Save(stringWriter, options);
			return stringWriter.ToString();
		}

		public void WriteTo(XmlWriter writer)
		{
			writer.WriteStartElement(this.name.LocalName, this.name.Namespace.NamespaceName);
			this.WriteContents(this.contents, writer);
			writer.WriteEndElement();
		}

		private void WriteContents(IEnumerable<object> items, XmlWriter w)
		{
			foreach (object obj in XUtil.ExpandArray(items))
			{
				if (obj != null)
				{
					if (obj is XNode)
					{
						((XNode)obj).WriteTo(w);
					}
					else if (obj is object[])
					{
						this.WriteContents((object[])obj, w);
					}
					else if (obj is XAttribute)
					{
						this.WriteAttribute((XAttribute)obj, w);
					}
					else
					{
						new XText(obj.ToString()).WriteTo(w);
					}
				}
			}
		}

		private void WriteAttribute(XAttribute a, XmlWriter w)
		{
			if (a.IsNamespaceDeclaration)
			{
				if (a.Name.Namespace == XNamespace.Xmlns)
				{
					w.WriteAttributeString("xmlns", a.Name.LocalName, XNamespace.Xmlns.NamespaceName, a.Value);
				}
				else
				{
					w.WriteAttributeString("xmlns", a.Value);
				}
			}
			else
			{
				w.WriteAttributeString(a.Name.LocalName, a.Name.Namespace.NamespaceName, a.Value);
			}
		}

		private XName name;

		private List<object> contents;
	}
}
