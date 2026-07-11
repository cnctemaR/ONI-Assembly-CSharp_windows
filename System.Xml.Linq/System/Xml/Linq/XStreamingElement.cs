using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace System.Xml.Linq
{
	public class XStreamingElement
	{
		public XStreamingElement(XName name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
		}

		public XStreamingElement(XName name, object content)
			: this(name)
		{
			object obj;
			if (!(content is List<object>))
			{
				obj = content;
			}
			else
			{
				(obj = new object[1])[0] = content;
			}
			this.content = obj;
		}

		public XStreamingElement(XName name, params object[] content)
			: this(name)
		{
			this.content = content;
		}

		public XName Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.name = value;
			}
		}

		public void Add(object content)
		{
			if (content != null)
			{
				List<object> list = this.content as List<object>;
				if (list == null)
				{
					list = new List<object>();
					if (this.content != null)
					{
						list.Add(this.content);
					}
					this.content = list;
				}
				list.Add(content);
			}
		}

		public void Add(params object[] content)
		{
			this.Add(content);
		}

		public void Save(string fileName)
		{
			this.Save(fileName, SaveOptions.None);
		}

		public void Save(string fileName, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			using (XmlWriter xmlWriter = XmlWriter.Create(fileName, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(Stream stream)
		{
			this.Save(stream, SaveOptions.None);
		}

		public void Save(Stream stream, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(TextWriter textWriter)
		{
			this.Save(textWriter, SaveOptions.None);
		}

		public void Save(TextWriter textWriter, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteStartDocument();
			this.WriteTo(writer);
			writer.WriteEndDocument();
		}

		public override string ToString()
		{
			return this.GetXmlString(SaveOptions.None);
		}

		public string ToString(SaveOptions options)
		{
			return this.GetXmlString(options);
		}

		public void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			new StreamingElementWriter(writer).WriteStreamingElement(this);
		}

		private string GetXmlString(SaveOptions o)
		{
			string text;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.OmitXmlDeclaration = true;
				if ((o & SaveOptions.DisableFormatting) == SaveOptions.None)
				{
					xmlWriterSettings.Indent = true;
				}
				if ((o & SaveOptions.OmitDuplicateNamespaces) != SaveOptions.None)
				{
					xmlWriterSettings.NamespaceHandling |= NamespaceHandling.OmitDuplicates;
				}
				using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
				{
					this.WriteTo(xmlWriter);
				}
				text = stringWriter.ToString();
			}
			return text;
		}

		internal XName name;

		internal object content;
	}
}
