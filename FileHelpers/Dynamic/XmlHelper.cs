using System;
using System.IO;
using System.Xml;

namespace FileHelpers.Dynamic
{
	internal sealed class XmlHelper : IDisposable
	{
		public void BeginWriteFile(string filename)
		{
			this.BeginWriteStream(new StreamWriter(new FileStream(filename, FileMode.Create)));
		}

		public void BeginWriteStream(TextWriter writer)
		{
			this.Writer = new XmlTextWriter(writer)
			{
				Formatting = Formatting.Indented,
				Indentation = 4
			};
		}

		public void BeginReadFile(string filename)
		{
			this.Reader = new XmlTextReader(new StreamReader(filename));
		}

		public void WriteElement(string element, string valueStr)
		{
			this.Writer.WriteStartElement(element);
			this.Writer.WriteString(valueStr);
			this.Writer.WriteEndElement();
		}

		public void WriteElement(string element, string valueStr, string defaultVal)
		{
			if (valueStr != defaultVal)
			{
				this.WriteElement(element, valueStr);
			}
		}

		public void WriteElement(string element, bool mustWrite)
		{
			if (mustWrite)
			{
				this.Writer.WriteStartElement(element);
				this.Writer.WriteEndElement();
			}
		}

		public void WriteAttribute(string attb, string valueStr, string defaultVal)
		{
			if (valueStr != defaultVal)
			{
				this.WriteAttribute(attb, valueStr);
			}
		}

		public void WriteAttribute(string attb, string valueStr)
		{
			this.Writer.WriteStartAttribute(attb, string.Empty);
			this.Writer.WriteString(valueStr);
			this.Writer.WriteEndAttribute();
		}

		public void EndWrite()
		{
			if (this.Writer != null)
			{
				this.Writer.Close();
			}
			this.Writer = null;
		}

		public void EndRead()
		{
			if (this.Reader != null)
			{
				this.Reader.Close();
			}
			this.Reader = null;
		}

		public void ReadToNextElement()
		{
			while (this.Reader.Read())
			{
				if (this.Reader.NodeType == XmlNodeType.Element)
				{
					return;
				}
			}
		}

		public void Dispose()
		{
			this.EndRead();
			this.EndWrite();
		}

		internal XmlTextWriter Writer;

		internal XmlTextReader Reader;
	}
}
