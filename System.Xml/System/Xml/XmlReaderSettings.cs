using System;
using System.ComponentModel;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Xml.Schema;
using System.Xml.XmlConfiguration;
using Microsoft.Win32;

namespace System.Xml
{
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public sealed class XmlReaderSettings
	{
		public XmlReaderSettings()
		{
			this.Initialize();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		public XmlReaderSettings(XmlResolver resolver)
		{
			this.Initialize(resolver);
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

		public XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
			set
			{
				this.CheckReadOnly("NameTable");
				this.nameTable = value;
			}
		}

		internal bool IsXmlResolverSet { get; set; }

		public XmlResolver XmlResolver
		{
			set
			{
				this.CheckReadOnly("XmlResolver");
				this.xmlResolver = value;
				this.IsXmlResolverSet = true;
			}
		}

		internal XmlResolver GetXmlResolver()
		{
			return this.xmlResolver;
		}

		internal XmlResolver GetXmlResolver_CheckConfig()
		{
			if (XmlReaderSection.ProhibitDefaultUrlResolver && !this.IsXmlResolverSet)
			{
				return null;
			}
			return this.xmlResolver;
		}

		public int LineNumberOffset
		{
			get
			{
				return this.lineNumberOffset;
			}
			set
			{
				this.CheckReadOnly("LineNumberOffset");
				this.lineNumberOffset = value;
			}
		}

		public int LinePositionOffset
		{
			get
			{
				return this.linePositionOffset;
			}
			set
			{
				this.CheckReadOnly("LinePositionOffset");
				this.linePositionOffset = value;
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

		public long MaxCharactersInDocument
		{
			get
			{
				return this.maxCharactersInDocument;
			}
			set
			{
				this.CheckReadOnly("MaxCharactersInDocument");
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.maxCharactersInDocument = value;
			}
		}

		public long MaxCharactersFromEntities
		{
			get
			{
				return this.maxCharactersFromEntities;
			}
			set
			{
				this.CheckReadOnly("MaxCharactersFromEntities");
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.maxCharactersFromEntities = value;
			}
		}

		public bool IgnoreWhitespace
		{
			get
			{
				return this.ignoreWhitespace;
			}
			set
			{
				this.CheckReadOnly("IgnoreWhitespace");
				this.ignoreWhitespace = value;
			}
		}

		public bool IgnoreProcessingInstructions
		{
			get
			{
				return this.ignorePIs;
			}
			set
			{
				this.CheckReadOnly("IgnoreProcessingInstructions");
				this.ignorePIs = value;
			}
		}

		public bool IgnoreComments
		{
			get
			{
				return this.ignoreComments;
			}
			set
			{
				this.CheckReadOnly("IgnoreComments");
				this.ignoreComments = value;
			}
		}

		[Obsolete("Use XmlReaderSettings.DtdProcessing property instead.")]
		public bool ProhibitDtd
		{
			get
			{
				return this.dtdProcessing == DtdProcessing.Prohibit;
			}
			set
			{
				this.CheckReadOnly("ProhibitDtd");
				this.dtdProcessing = (value ? DtdProcessing.Prohibit : DtdProcessing.Parse);
			}
		}

		public DtdProcessing DtdProcessing
		{
			get
			{
				return this.dtdProcessing;
			}
			set
			{
				this.CheckReadOnly("DtdProcessing");
				if (value > DtdProcessing.Parse)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.dtdProcessing = value;
			}
		}

		public bool CloseInput
		{
			get
			{
				return this.closeInput;
			}
			set
			{
				this.CheckReadOnly("CloseInput");
				this.closeInput = value;
			}
		}

		public ValidationType ValidationType
		{
			get
			{
				return this.validationType;
			}
			set
			{
				this.CheckReadOnly("ValidationType");
				if (value > ValidationType.Schema)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.validationType = value;
			}
		}

		public XmlSchemaValidationFlags ValidationFlags
		{
			get
			{
				return this.validationFlags;
			}
			set
			{
				this.CheckReadOnly("ValidationFlags");
				if (value > (XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.AllowXmlAttributes))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.validationFlags = value;
			}
		}

		public XmlSchemaSet Schemas
		{
			get
			{
				if (this.schemas == null)
				{
					this.schemas = new XmlSchemaSet();
				}
				return this.schemas;
			}
			set
			{
				this.CheckReadOnly("Schemas");
				this.schemas = value;
			}
		}

		public event ValidationEventHandler ValidationEventHandler
		{
			add
			{
				this.CheckReadOnly("ValidationEventHandler");
				this.valEventHandler = (ValidationEventHandler)Delegate.Combine(this.valEventHandler, value);
			}
			remove
			{
				this.CheckReadOnly("ValidationEventHandler");
				this.valEventHandler = (ValidationEventHandler)Delegate.Remove(this.valEventHandler, value);
			}
		}

		public void Reset()
		{
			this.CheckReadOnly("Reset");
			this.Initialize();
		}

		public XmlReaderSettings Clone()
		{
			XmlReaderSettings xmlReaderSettings = base.MemberwiseClone() as XmlReaderSettings;
			xmlReaderSettings.ReadOnly = false;
			return xmlReaderSettings;
		}

		internal ValidationEventHandler GetEventHandler()
		{
			return this.valEventHandler;
		}

		internal XmlReader CreateReader(string inputUri, XmlParserContext inputContext)
		{
			if (inputUri == null)
			{
				throw new ArgumentNullException("inputUri");
			}
			if (inputUri.Length == 0)
			{
				throw new ArgumentException(Res.GetString("The string was not recognized as a valid Uri."), "inputUri");
			}
			XmlResolver xmlResolver = this.GetXmlResolver();
			if (xmlResolver == null)
			{
				xmlResolver = XmlReaderSettings.CreateDefaultResolver();
			}
			XmlReader xmlReader = new XmlTextReaderImpl(inputUri, this, inputContext, xmlResolver);
			if (this.ValidationType != ValidationType.None)
			{
				xmlReader = this.AddValidation(xmlReader);
			}
			if (this.useAsync)
			{
				xmlReader = XmlAsyncCheckReader.CreateAsyncCheckWrapper(xmlReader);
			}
			return xmlReader;
		}

		internal XmlReader CreateReader(Stream input, Uri baseUri, string baseUriString, XmlParserContext inputContext)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (baseUriString == null)
			{
				if (baseUri == null)
				{
					baseUriString = string.Empty;
				}
				else
				{
					baseUriString = baseUri.ToString();
				}
			}
			XmlReader xmlReader = new XmlTextReaderImpl(input, null, 0, this, baseUri, baseUriString, inputContext, this.closeInput);
			if (this.ValidationType != ValidationType.None)
			{
				xmlReader = this.AddValidation(xmlReader);
			}
			if (this.useAsync)
			{
				xmlReader = XmlAsyncCheckReader.CreateAsyncCheckWrapper(xmlReader);
			}
			return xmlReader;
		}

		internal XmlReader CreateReader(TextReader input, string baseUriString, XmlParserContext inputContext)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (baseUriString == null)
			{
				baseUriString = string.Empty;
			}
			XmlReader xmlReader = new XmlTextReaderImpl(input, this, baseUriString, inputContext);
			if (this.ValidationType != ValidationType.None)
			{
				xmlReader = this.AddValidation(xmlReader);
			}
			if (this.useAsync)
			{
				xmlReader = XmlAsyncCheckReader.CreateAsyncCheckWrapper(xmlReader);
			}
			return xmlReader;
		}

		internal XmlReader CreateReader(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			return this.AddValidationAndConformanceWrapper(reader);
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
			this.Initialize(null);
		}

		private void Initialize(XmlResolver resolver)
		{
			this.nameTable = null;
			if (!XmlReaderSettings.EnableLegacyXmlSettings())
			{
				this.xmlResolver = resolver;
				this.maxCharactersFromEntities = 10000000L;
			}
			else
			{
				this.xmlResolver = ((resolver == null) ? XmlReaderSettings.CreateDefaultResolver() : resolver);
				this.maxCharactersFromEntities = 0L;
			}
			this.lineNumberOffset = 0;
			this.linePositionOffset = 0;
			this.checkCharacters = true;
			this.conformanceLevel = ConformanceLevel.Document;
			this.ignoreWhitespace = false;
			this.ignorePIs = false;
			this.ignoreComments = false;
			this.dtdProcessing = DtdProcessing.Prohibit;
			this.closeInput = false;
			this.maxCharactersInDocument = 0L;
			this.schemas = null;
			this.validationType = ValidationType.None;
			this.validationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints;
			this.validationFlags |= XmlSchemaValidationFlags.AllowXmlAttributes;
			this.useAsync = false;
			this.isReadOnly = false;
			this.IsXmlResolverSet = false;
		}

		private static XmlResolver CreateDefaultResolver()
		{
			return new XmlUrlResolver();
		}

		internal XmlReader AddValidation(XmlReader reader)
		{
			if (this.validationType == ValidationType.Schema)
			{
				XmlResolver xmlResolver = this.GetXmlResolver_CheckConfig();
				if (xmlResolver == null && !this.IsXmlResolverSet && !XmlReaderSettings.EnableLegacyXmlSettings())
				{
					xmlResolver = new XmlUrlResolver();
				}
				reader = new XsdValidatingReader(reader, xmlResolver, this);
			}
			else if (this.validationType == ValidationType.DTD)
			{
				reader = this.CreateDtdValidatingReader(reader);
			}
			return reader;
		}

		private XmlReader AddValidationAndConformanceWrapper(XmlReader reader)
		{
			if (this.validationType == ValidationType.DTD)
			{
				reader = this.CreateDtdValidatingReader(reader);
			}
			reader = this.AddConformanceWrapper(reader);
			if (this.validationType == ValidationType.Schema)
			{
				reader = new XsdValidatingReader(reader, this.GetXmlResolver_CheckConfig(), this);
			}
			return reader;
		}

		private XmlValidatingReaderImpl CreateDtdValidatingReader(XmlReader baseReader)
		{
			return new XmlValidatingReaderImpl(baseReader, this.GetEventHandler(), (this.ValidationFlags & XmlSchemaValidationFlags.ProcessIdentityConstraints) > XmlSchemaValidationFlags.None);
		}

		internal XmlReader AddConformanceWrapper(XmlReader baseReader)
		{
			XmlReaderSettings settings = baseReader.Settings;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			DtdProcessing dtdProcessing = (DtdProcessing)(-1);
			bool flag5 = false;
			if (settings == null)
			{
				if (this.conformanceLevel != ConformanceLevel.Auto && this.conformanceLevel != XmlReader.GetV1ConformanceLevel(baseReader))
				{
					throw new InvalidOperationException(Res.GetString("Cannot change conformance checking to {0}. Make sure the ConformanceLevel in XmlReaderSettings is set to Auto for wrapping scenarios.", new object[] { this.conformanceLevel.ToString() }));
				}
				XmlTextReader xmlTextReader = baseReader as XmlTextReader;
				if (xmlTextReader == null)
				{
					XmlValidatingReader xmlValidatingReader = baseReader as XmlValidatingReader;
					if (xmlValidatingReader != null)
					{
						xmlTextReader = (XmlTextReader)xmlValidatingReader.Reader;
					}
				}
				if (this.ignoreWhitespace)
				{
					WhitespaceHandling whitespaceHandling = WhitespaceHandling.All;
					if (xmlTextReader != null)
					{
						whitespaceHandling = xmlTextReader.WhitespaceHandling;
					}
					if (whitespaceHandling == WhitespaceHandling.All)
					{
						flag2 = true;
						flag5 = true;
					}
				}
				if (this.ignoreComments)
				{
					flag3 = true;
					flag5 = true;
				}
				if (this.ignorePIs)
				{
					flag4 = true;
					flag5 = true;
				}
				DtdProcessing dtdProcessing2 = DtdProcessing.Parse;
				if (xmlTextReader != null)
				{
					dtdProcessing2 = xmlTextReader.DtdProcessing;
				}
				if ((this.dtdProcessing == DtdProcessing.Prohibit && dtdProcessing2 != DtdProcessing.Prohibit) || (this.dtdProcessing == DtdProcessing.Ignore && dtdProcessing2 == DtdProcessing.Parse))
				{
					dtdProcessing = this.dtdProcessing;
					flag5 = true;
				}
			}
			else
			{
				if (this.conformanceLevel != settings.ConformanceLevel && this.conformanceLevel != ConformanceLevel.Auto)
				{
					throw new InvalidOperationException(Res.GetString("Cannot change conformance checking to {0}. Make sure the ConformanceLevel in XmlReaderSettings is set to Auto for wrapping scenarios.", new object[] { this.conformanceLevel.ToString() }));
				}
				if (this.checkCharacters && !settings.CheckCharacters)
				{
					flag = true;
					flag5 = true;
				}
				if (this.ignoreWhitespace && !settings.IgnoreWhitespace)
				{
					flag2 = true;
					flag5 = true;
				}
				if (this.ignoreComments && !settings.IgnoreComments)
				{
					flag3 = true;
					flag5 = true;
				}
				if (this.ignorePIs && !settings.IgnoreProcessingInstructions)
				{
					flag4 = true;
					flag5 = true;
				}
				if ((this.dtdProcessing == DtdProcessing.Prohibit && settings.DtdProcessing != DtdProcessing.Prohibit) || (this.dtdProcessing == DtdProcessing.Ignore && settings.DtdProcessing == DtdProcessing.Parse))
				{
					dtdProcessing = this.dtdProcessing;
					flag5 = true;
				}
			}
			if (!flag5)
			{
				return baseReader;
			}
			IXmlNamespaceResolver xmlNamespaceResolver = baseReader as IXmlNamespaceResolver;
			if (xmlNamespaceResolver != null)
			{
				return new XmlCharCheckingReaderWithNS(baseReader, xmlNamespaceResolver, flag, flag2, flag3, flag4, dtdProcessing);
			}
			return new XmlCharCheckingReader(baseReader, flag, flag2, flag3, flag4, dtdProcessing);
		}

		internal static bool EnableLegacyXmlSettings()
		{
			if (XmlReaderSettings.s_enableLegacyXmlSettings != null)
			{
				return XmlReaderSettings.s_enableLegacyXmlSettings.Value;
			}
			if (!BinaryCompatibility.TargetsAtLeast_Desktop_V4_5_2)
			{
				XmlReaderSettings.s_enableLegacyXmlSettings = new bool?(true);
				return XmlReaderSettings.s_enableLegacyXmlSettings.Value;
			}
			bool flag = false;
			if (!XmlReaderSettings.ReadSettingsFromRegistry(Registry.LocalMachine, ref flag))
			{
				XmlReaderSettings.ReadSettingsFromRegistry(Registry.CurrentUser, ref flag);
			}
			XmlReaderSettings.s_enableLegacyXmlSettings = new bool?(flag);
			return XmlReaderSettings.s_enableLegacyXmlSettings.Value;
		}

		[SecuritySafeCritical]
		[RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
		private static bool ReadSettingsFromRegistry(RegistryKey hive, ref bool value)
		{
			try
			{
				using (RegistryKey registryKey = hive.OpenSubKey("SOFTWARE\\Microsoft\\.NETFramework\\XML", false))
				{
					if (registryKey != null && registryKey.GetValueKind("EnableLegacyXmlSettings") == RegistryValueKind.DWord)
					{
						value = (int)registryKey.GetValue("EnableLegacyXmlSettings") == 1;
						return true;
					}
				}
			}
			catch
			{
			}
			return false;
		}

		private bool useAsync;

		private XmlNameTable nameTable;

		private XmlResolver xmlResolver;

		private int lineNumberOffset;

		private int linePositionOffset;

		private ConformanceLevel conformanceLevel;

		private bool checkCharacters;

		private long maxCharactersInDocument;

		private long maxCharactersFromEntities;

		private bool ignoreWhitespace;

		private bool ignorePIs;

		private bool ignoreComments;

		private DtdProcessing dtdProcessing;

		private ValidationType validationType;

		private XmlSchemaValidationFlags validationFlags;

		private XmlSchemaSet schemas;

		private ValidationEventHandler valEventHandler;

		private bool closeInput;

		private bool isReadOnly;

		private static bool? s_enableLegacyXmlSettings;
	}
}
