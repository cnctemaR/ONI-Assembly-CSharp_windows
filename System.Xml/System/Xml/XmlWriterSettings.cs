using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Xml.Xsl.Runtime;

namespace System.Xml
{
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public sealed class XmlWriterSettings
	{
		public XmlWriterSettings()
		{
			this.Initialize();
		}

		public bool Async
		{
			get
			{
				return this.useAsync;
			}
			set
			{
				this.CheckReadOnly("Async");
				this.useAsync = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				this.CheckReadOnly("Encoding");
				this.encoding = value;
			}
		}

		public bool OmitXmlDeclaration
		{
			get
			{
				return this.omitXmlDecl;
			}
			set
			{
				this.CheckReadOnly("OmitXmlDeclaration");
				this.omitXmlDecl = value;
			}
		}

		public NewLineHandling NewLineHandling
		{
			get
			{
				return this.newLineHandling;
			}
			set
			{
				this.CheckReadOnly("NewLineHandling");
				if (value > NewLineHandling.None)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.newLineHandling = value;
			}
		}

		public string NewLineChars
		{
			get
			{
				return this.newLineChars;
			}
			set
			{
				this.CheckReadOnly("NewLineChars");
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.newLineChars = value;
			}
		}

		public bool Indent
		{
			get
			{
				return this.indent == TriState.True;
			}
			set
			{
				this.CheckReadOnly("Indent");
				this.indent = (value ? TriState.True : TriState.False);
			}
		}

		public string IndentChars
		{
			get
			{
				return this.indentChars;
			}
			set
			{
				this.CheckReadOnly("IndentChars");
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.indentChars = value;
			}
		}

		public bool NewLineOnAttributes
		{
			get
			{
				return this.newLineOnAttributes;
			}
			set
			{
				this.CheckReadOnly("NewLineOnAttributes");
				this.newLineOnAttributes = value;
			}
		}

		public bool CloseOutput
		{
			get
			{
				return this.closeOutput;
			}
			set
			{
				this.CheckReadOnly("CloseOutput");
				this.closeOutput = value;
			}
		}

		public ConformanceLevel ConformanceLevel
		{
			get
			{
				return this.conformanceLevel;
			}
			set
			{
				this.CheckReadOnly("ConformanceLevel");
				if (value > ConformanceLevel.Document)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.conformanceLevel = value;
			}
		}

		public bool CheckCharacters
		{
			get
			{
				return this.checkCharacters;
			}
			set
			{
				this.CheckReadOnly("CheckCharacters");
				this.checkCharacters = value;
			}
		}

		public NamespaceHandling NamespaceHandling
		{
			get
			{
				return this.namespaceHandling;
			}
			set
			{
				this.CheckReadOnly("NamespaceHandling");
				if (value > NamespaceHandling.OmitDuplicates)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.namespaceHandling = value;
			}
		}

		public bool WriteEndDocumentOnClose
		{
			get
			{
				return this.writeEndDocumentOnClose;
			}
			set
			{
				this.CheckReadOnly("WriteEndDocumentOnClose");
				this.writeEndDocumentOnClose = value;
			}
		}

		public XmlOutputMethod OutputMethod
		{
			get
			{
				return this.outputMethod;
			}
			internal set
			{
				this.outputMethod = value;
			}
		}

		public void Reset()
		{
			this.CheckReadOnly("Reset");
			this.Initialize();
		}

		public XmlWriterSettings Clone()
		{
			XmlWriterSettings xmlWriterSettings = base.MemberwiseClone() as XmlWriterSettings;
			xmlWriterSettings.cdataSections = new List<XmlQualifiedName>(this.cdataSections);
			xmlWriterSettings.isReadOnly = false;
			return xmlWriterSettings;
		}

		internal List<XmlQualifiedName> CDataSectionElements
		{
			get
			{
				return this.cdataSections;
			}
		}

		public bool DoNotEscapeUriAttributes
		{
			get
			{
				return this.doNotEscapeUriAttributes;
			}
			set
			{
				this.CheckReadOnly("DoNotEscapeUriAttributes");
				this.doNotEscapeUriAttributes = value;
			}
		}

		internal bool MergeCDataSections
		{
			get
			{
				return this.mergeCDataSections;
			}
			set
			{
				this.CheckReadOnly("MergeCDataSections");
				this.mergeCDataSections = value;
			}
		}

		internal string MediaType
		{
			get
			{
				return this.mediaType;
			}
			set
			{
				this.CheckReadOnly("MediaType");
				this.mediaType = value;
			}
		}

		internal string DocTypeSystem
		{
			get
			{
				return this.docTypeSystem;
			}
			set
			{
				this.CheckReadOnly("DocTypeSystem");
				this.docTypeSystem = value;
			}
		}

		internal string DocTypePublic
		{
			get
			{
				return this.docTypePublic;
			}
			set
			{
				this.CheckReadOnly("DocTypePublic");
				this.docTypePublic = value;
			}
		}

		internal XmlStandalone Standalone
		{
			get
			{
				return this.standalone;
			}
			set
			{
				this.CheckReadOnly("Standalone");
				this.standalone = value;
			}
		}

		internal bool AutoXmlDeclaration
		{
			get
			{
				return this.autoXmlDecl;
			}
			set
			{
				this.CheckReadOnly("AutoXmlDeclaration");
				this.autoXmlDecl = value;
			}
		}

		internal TriState IndentInternal
		{
			get
			{
				return this.indent;
			}
			set
			{
				this.indent = value;
			}
		}

		internal bool IsQuerySpecific
		{
			get
			{
				return this.cdataSections.Count != 0 || this.docTypePublic != null || this.docTypeSystem != null || this.standalone == XmlStandalone.Yes;
			}
		}

		internal XmlWriter CreateWriter(string outputFileName)
		{
			if (outputFileName == null)
			{
				throw new ArgumentNullException("outputFileName");
			}
			XmlWriterSettings xmlWriterSettings = this;
			if (!xmlWriterSettings.CloseOutput)
			{
				xmlWriterSettings = xmlWriterSettings.Clone();
				xmlWriterSettings.CloseOutput = true;
			}
			FileStream fileStream = null;
			XmlWriter xmlWriter;
			try
			{
				fileStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, this.useAsync);
				xmlWriter = xmlWriterSettings.CreateWriter(fileStream);
			}
			catch
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				throw;
			}
			return xmlWriter;
		}

		internal XmlWriter CreateWriter(Stream output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			XmlWriter xmlWriter;
			if (this.Encoding.WebName == "utf-8")
			{
				switch (this.OutputMethod)
				{
				case XmlOutputMethod.Xml:
					if (this.Indent)
					{
						xmlWriter = new XmlUtf8RawTextWriterIndent(output, this);
					}
					else
					{
						xmlWriter = new XmlUtf8RawTextWriter(output, this);
					}
					break;
				case XmlOutputMethod.Html:
					if (this.Indent)
					{
						xmlWriter = new HtmlUtf8RawTextWriterIndent(output, this);
					}
					else
					{
						xmlWriter = new HtmlUtf8RawTextWriter(output, this);
					}
					break;
				case XmlOutputMethod.Text:
					xmlWriter = new TextUtf8RawTextWriter(output, this);
					break;
				case XmlOutputMethod.AutoDetect:
					xmlWriter = new XmlAutoDetectWriter(output, this);
					break;
				default:
					return null;
				}
			}
			else
			{
				switch (this.OutputMethod)
				{
				case XmlOutputMethod.Xml:
					if (this.Indent)
					{
						xmlWriter = new XmlEncodedRawTextWriterIndent(output, this);
					}
					else
					{
						xmlWriter = new XmlEncodedRawTextWriter(output, this);
					}
					break;
				case XmlOutputMethod.Html:
					if (this.Indent)
					{
						xmlWriter = new HtmlEncodedRawTextWriterIndent(output, this);
					}
					else
					{
						xmlWriter = new HtmlEncodedRawTextWriter(output, this);
					}
					break;
				case XmlOutputMethod.Text:
					xmlWriter = new TextEncodedRawTextWriter(output, this);
					break;
				case XmlOutputMethod.AutoDetect:
					xmlWriter = new XmlAutoDetectWriter(output, this);
					break;
				default:
					return null;
				}
			}
			if (this.OutputMethod != XmlOutputMethod.AutoDetect && this.IsQuerySpecific)
			{
				xmlWriter = new QueryOutputWriter((XmlRawWriter)xmlWriter, this);
			}
			xmlWriter = new XmlWellFormedWriter(xmlWriter, this);
			if (this.useAsync)
			{
				xmlWriter = new XmlAsyncCheckWriter(xmlWriter);
			}
			return xmlWriter;
		}

		internal XmlWriter CreateWriter(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			XmlWriter xmlWriter;
			switch (this.OutputMethod)
			{
			case XmlOutputMethod.Xml:
				if (this.Indent)
				{
					xmlWriter = new XmlEncodedRawTextWriterIndent(output, this);
				}
				else
				{
					xmlWriter = new XmlEncodedRawTextWriter(output, this);
				}
				break;
			case XmlOutputMethod.Html:
				if (this.Indent)
				{
					xmlWriter = new HtmlEncodedRawTextWriterIndent(output, this);
				}
				else
				{
					xmlWriter = new HtmlEncodedRawTextWriter(output, this);
				}
				break;
			case XmlOutputMethod.Text:
				xmlWriter = new TextEncodedRawTextWriter(output, this);
				break;
			case XmlOutputMethod.AutoDetect:
				xmlWriter = new XmlAutoDetectWriter(output, this);
				break;
			default:
				return null;
			}
			if (this.OutputMethod != XmlOutputMethod.AutoDetect && this.IsQuerySpecific)
			{
				xmlWriter = new QueryOutputWriter((XmlRawWriter)xmlWriter, this);
			}
			xmlWriter = new XmlWellFormedWriter(xmlWriter, this);
			if (this.useAsync)
			{
				xmlWriter = new XmlAsyncCheckWriter(xmlWriter);
			}
			return xmlWriter;
		}

		internal XmlWriter CreateWriter(XmlWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			return this.AddConformanceWrapper(output);
		}

		internal bool ReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				this.isReadOnly = value;
			}
		}

		private void CheckReadOnly(string propertyName)
		{
			if (this.isReadOnly)
			{
				throw new XmlException("The '{0}' property is read only and cannot be set.", base.GetType().Name + "." + propertyName);
			}
		}

		private void Initialize()
		{
			this.encoding = Encoding.UTF8;
			this.omitXmlDecl = false;
			this.newLineHandling = NewLineHandling.Replace;
			this.newLineChars = Environment.NewLine;
			this.indent = TriState.Unknown;
			this.indentChars = "  ";
			this.newLineOnAttributes = false;
			this.closeOutput = false;
			this.namespaceHandling = NamespaceHandling.Default;
			this.conformanceLevel = ConformanceLevel.Document;
			this.checkCharacters = true;
			this.writeEndDocumentOnClose = true;
			this.outputMethod = XmlOutputMethod.Xml;
			this.cdataSections.Clear();
			this.mergeCDataSections = false;
			this.mediaType = null;
			this.docTypeSystem = null;
			this.docTypePublic = null;
			this.standalone = XmlStandalone.Omit;
			this.doNotEscapeUriAttributes = false;
			this.useAsync = false;
			this.isReadOnly = false;
		}

		private XmlWriter AddConformanceWrapper(XmlWriter baseWriter)
		{
			ConformanceLevel conformanceLevel = ConformanceLevel.Auto;
			XmlWriterSettings settings = baseWriter.Settings;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (settings == null)
			{
				if (this.newLineHandling == NewLineHandling.Replace)
				{
					flag3 = true;
					flag4 = true;
				}
				if (this.checkCharacters)
				{
					flag = true;
					flag4 = true;
				}
			}
			else
			{
				if (this.conformanceLevel != settings.ConformanceLevel)
				{
					conformanceLevel = this.ConformanceLevel;
					flag4 = true;
				}
				if (this.checkCharacters && !settings.CheckCharacters)
				{
					flag = true;
					flag2 = conformanceLevel == ConformanceLevel.Auto;
					flag4 = true;
				}
				if (this.newLineHandling == NewLineHandling.Replace && settings.NewLineHandling == NewLineHandling.None)
				{
					flag3 = true;
					flag4 = true;
				}
			}
			XmlWriter xmlWriter = baseWriter;
			if (flag4)
			{
				if (conformanceLevel != ConformanceLevel.Auto)
				{
					xmlWriter = new XmlWellFormedWriter(xmlWriter, this);
				}
				if (flag || flag3)
				{
					xmlWriter = new XmlCharCheckingWriter(xmlWriter, flag, flag2, flag3, this.NewLineChars);
				}
			}
			if (this.IsQuerySpecific && (settings == null || !settings.IsQuerySpecific))
			{
				xmlWriter = new QueryOutputWriterV1(xmlWriter, this);
			}
			return xmlWriter;
		}

		internal void GetObjectData(XmlQueryDataWriter writer)
		{
			writer.Write(this.Encoding.CodePage);
			writer.Write(this.OmitXmlDeclaration);
			writer.Write((sbyte)this.NewLineHandling);
			writer.WriteStringQ(this.NewLineChars);
			writer.Write((sbyte)this.IndentInternal);
			writer.WriteStringQ(this.IndentChars);
			writer.Write(this.NewLineOnAttributes);
			writer.Write(this.CloseOutput);
			writer.Write((sbyte)this.ConformanceLevel);
			writer.Write(this.CheckCharacters);
			writer.Write((sbyte)this.outputMethod);
			writer.Write(this.cdataSections.Count);
			foreach (XmlQualifiedName xmlQualifiedName in this.cdataSections)
			{
				writer.Write(xmlQualifiedName.Name);
				writer.Write(xmlQualifiedName.Namespace);
			}
			writer.Write(this.mergeCDataSections);
			writer.WriteStringQ(this.mediaType);
			writer.WriteStringQ(this.docTypeSystem);
			writer.WriteStringQ(this.docTypePublic);
			writer.Write((sbyte)this.standalone);
			writer.Write(this.autoXmlDecl);
			writer.Write(this.ReadOnly);
		}

		internal XmlWriterSettings(XmlQueryDataReader reader)
		{
			this.Encoding = Encoding.GetEncoding(reader.ReadInt32());
			this.OmitXmlDeclaration = reader.ReadBoolean();
			this.NewLineHandling = (NewLineHandling)reader.ReadSByte(0, 2);
			this.NewLineChars = reader.ReadStringQ();
			this.IndentInternal = (TriState)reader.ReadSByte(-1, 1);
			this.IndentChars = reader.ReadStringQ();
			this.NewLineOnAttributes = reader.ReadBoolean();
			this.CloseOutput = reader.ReadBoolean();
			this.ConformanceLevel = (ConformanceLevel)reader.ReadSByte(0, 2);
			this.CheckCharacters = reader.ReadBoolean();
			this.outputMethod = (XmlOutputMethod)reader.ReadSByte(0, 3);
			int num = reader.ReadInt32();
			this.cdataSections = new List<XmlQualifiedName>(num);
			for (int i = 0; i < num; i++)
			{
				this.cdataSections.Add(new XmlQualifiedName(reader.ReadString(), reader.ReadString()));
			}
			this.mergeCDataSections = reader.ReadBoolean();
			this.mediaType = reader.ReadStringQ();
			this.docTypeSystem = reader.ReadStringQ();
			this.docTypePublic = reader.ReadStringQ();
			this.Standalone = (XmlStandalone)reader.ReadSByte(0, 2);
			this.autoXmlDecl = reader.ReadBoolean();
			this.ReadOnly = reader.ReadBoolean();
		}

		private bool useAsync;

		private Encoding encoding;

		private bool omitXmlDecl;

		private NewLineHandling newLineHandling;

		private string newLineChars;

		private TriState indent;

		private string indentChars;

		private bool newLineOnAttributes;

		private bool closeOutput;

		private NamespaceHandling namespaceHandling;

		private ConformanceLevel conformanceLevel;

		private bool checkCharacters;

		private bool writeEndDocumentOnClose;

		private XmlOutputMethod outputMethod;

		private List<XmlQualifiedName> cdataSections = new List<XmlQualifiedName>();

		private bool doNotEscapeUriAttributes;

		private bool mergeCDataSections;

		private string mediaType;

		private string docTypeSystem;

		private string docTypePublic;

		private XmlStandalone standalone;

		private bool autoXmlDecl;

		private bool isReadOnly;
	}
}
