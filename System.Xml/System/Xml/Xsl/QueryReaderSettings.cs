using System;
using System.IO;

namespace System.Xml.Xsl
{
	internal class QueryReaderSettings
	{
		public QueryReaderSettings(XmlNameTable xmlNameTable)
		{
			this.xmlReaderSettings = new XmlReaderSettings();
			this.xmlReaderSettings.NameTable = xmlNameTable;
			this.xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
			this.xmlReaderSettings.XmlResolver = null;
			this.xmlReaderSettings.DtdProcessing = DtdProcessing.Prohibit;
			this.xmlReaderSettings.CloseInput = true;
		}

		public QueryReaderSettings(XmlReader reader)
		{
			XmlValidatingReader xmlValidatingReader = reader as XmlValidatingReader;
			if (xmlValidatingReader != null)
			{
				this.validatingReader = true;
				reader = xmlValidatingReader.Impl.Reader;
			}
			this.xmlReaderSettings = reader.Settings;
			if (this.xmlReaderSettings != null)
			{
				this.xmlReaderSettings = this.xmlReaderSettings.Clone();
				this.xmlReaderSettings.NameTable = reader.NameTable;
				this.xmlReaderSettings.CloseInput = true;
				this.xmlReaderSettings.LineNumberOffset = 0;
				this.xmlReaderSettings.LinePositionOffset = 0;
				XmlTextReaderImpl xmlTextReaderImpl = reader as XmlTextReaderImpl;
				if (xmlTextReaderImpl != null)
				{
					this.xmlReaderSettings.XmlResolver = xmlTextReaderImpl.GetResolver();
					return;
				}
			}
			else
			{
				this.xmlNameTable = reader.NameTable;
				XmlTextReader xmlTextReader = reader as XmlTextReader;
				if (xmlTextReader != null)
				{
					XmlTextReaderImpl impl = xmlTextReader.Impl;
					this.entityHandling = impl.EntityHandling;
					this.namespaces = impl.Namespaces;
					this.normalization = impl.Normalization;
					this.prohibitDtd = impl.DtdProcessing == DtdProcessing.Prohibit;
					this.whitespaceHandling = impl.WhitespaceHandling;
					this.xmlResolver = impl.GetResolver();
					return;
				}
				this.entityHandling = EntityHandling.ExpandEntities;
				this.namespaces = true;
				this.normalization = true;
				this.prohibitDtd = true;
				this.whitespaceHandling = WhitespaceHandling.All;
				this.xmlResolver = null;
			}
		}

		public XmlReader CreateReader(Stream stream, string baseUri)
		{
			XmlReader xmlReader;
			if (this.xmlReaderSettings != null)
			{
				xmlReader = XmlReader.Create(stream, this.xmlReaderSettings, baseUri);
			}
			else
			{
				xmlReader = new XmlTextReaderImpl(baseUri, stream, this.xmlNameTable)
				{
					EntityHandling = this.entityHandling,
					Namespaces = this.namespaces,
					Normalization = this.normalization,
					DtdProcessing = (this.prohibitDtd ? DtdProcessing.Prohibit : DtdProcessing.Parse),
					WhitespaceHandling = this.whitespaceHandling,
					XmlResolver = this.xmlResolver
				};
			}
			if (this.validatingReader)
			{
				xmlReader = new XmlValidatingReader(xmlReader);
			}
			return xmlReader;
		}

		public XmlNameTable NameTable
		{
			get
			{
				if (this.xmlReaderSettings == null)
				{
					return this.xmlNameTable;
				}
				return this.xmlReaderSettings.NameTable;
			}
		}

		private bool validatingReader;

		private XmlReaderSettings xmlReaderSettings;

		private XmlNameTable xmlNameTable;

		private EntityHandling entityHandling;

		private bool namespaces;

		private bool normalization;

		private bool prohibitDtd;

		private WhitespaceHandling whitespaceHandling;

		private XmlResolver xmlResolver;
	}
}
