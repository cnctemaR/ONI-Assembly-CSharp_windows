using System;
using System.Xml.Schema;

namespace System.Xml
{
	public sealed class XmlReaderSettings
	{
		public XmlReaderSettings()
		{
			this.Reset();
		}

		public event ValidationEventHandler ValidationEventHandler;

		public XmlReaderSettings Clone()
		{
			return (XmlReaderSettings)base.MemberwiseClone();
		}

		public void Reset()
		{
			this.checkCharacters = true;
			this.closeInput = false;
			this.conformance = ConformanceLevel.Document;
			this.ignoreComments = false;
			this.ignoreProcessingInstructions = false;
			this.ignoreWhitespace = false;
			this.lineNumberOffset = 0;
			this.linePositionOffset = 0;
			this.prohibitDtd = true;
			this.schemas = null;
			this.schemasNeedsInitialization = true;
			this.validationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.AllowXmlAttributes;
			this.validationType = ValidationType.None;
			this.xmlResolver = new XmlUrlResolver();
		}

		public bool CheckCharacters
		{
			get
			{
				return this.checkCharacters;
			}
			set
			{
				this.checkCharacters = value;
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
				this.closeInput = value;
			}
		}

		public ConformanceLevel ConformanceLevel
		{
			get
			{
				return this.conformance;
			}
			set
			{
				this.conformance = value;
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
				this.ignoreComments = value;
			}
		}

		public bool IgnoreProcessingInstructions
		{
			get
			{
				return this.ignoreProcessingInstructions;
			}
			set
			{
				this.ignoreProcessingInstructions = value;
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
				this.ignoreWhitespace = value;
			}
		}

		public int LineNumberOffset
		{
			get
			{
				return this.lineNumberOffset;
			}
			set
			{
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
				this.linePositionOffset = value;
			}
		}

		public bool ProhibitDtd
		{
			get
			{
				return this.prohibitDtd;
			}
			set
			{
				this.prohibitDtd = value;
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
				this.nameTable = value;
			}
		}

		public XmlSchemaSet Schemas
		{
			get
			{
				if (this.schemasNeedsInitialization)
				{
					this.schemas = new XmlSchemaSet();
					this.schemasNeedsInitialization = false;
				}
				return this.schemas;
			}
			set
			{
				this.schemas = value;
				this.schemasNeedsInitialization = false;
			}
		}

		internal void OnValidationError(object o, ValidationEventArgs e)
		{
			if (this.ValidationEventHandler != null)
			{
				this.ValidationEventHandler(o, e);
			}
			else if (e.Severity == XmlSeverityType.Error)
			{
				throw e.Exception;
			}
		}

		internal void SetSchemas(XmlSchemaSet schemas)
		{
			this.schemas = schemas;
		}

		public XmlSchemaValidationFlags ValidationFlags
		{
			get
			{
				return this.validationFlags;
			}
			set
			{
				this.validationFlags = value;
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
				this.validationType = value;
			}
		}

		public XmlResolver XmlResolver
		{
			internal get
			{
				return this.xmlResolver;
			}
			set
			{
				this.xmlResolver = value;
			}
		}

		private bool checkCharacters;

		private bool closeInput;

		private ConformanceLevel conformance;

		private bool ignoreComments;

		private bool ignoreProcessingInstructions;

		private bool ignoreWhitespace;

		private int lineNumberOffset;

		private int linePositionOffset;

		private bool prohibitDtd;

		private XmlNameTable nameTable;

		private XmlSchemaSet schemas;

		private bool schemasNeedsInitialization;

		private XmlSchemaValidationFlags validationFlags;

		private ValidationType validationType;

		private XmlResolver xmlResolver;
	}
}
