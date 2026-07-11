using System;
using System.Collections;
using System.Text;

namespace System.Xml.Schema
{
	internal class BaseValidator
	{
		public BaseValidator(BaseValidator other)
		{
			this.reader = other.reader;
			this.schemaCollection = other.schemaCollection;
			this.eventHandling = other.eventHandling;
			this.nameTable = other.nameTable;
			this.schemaNames = other.schemaNames;
			this.positionInfo = other.positionInfo;
			this.xmlResolver = other.xmlResolver;
			this.baseUri = other.baseUri;
			this.elementName = other.elementName;
		}

		public BaseValidator(XmlValidatingReaderImpl reader, XmlSchemaCollection schemaCollection, IValidationEventHandling eventHandling)
		{
			this.reader = reader;
			this.schemaCollection = schemaCollection;
			this.eventHandling = eventHandling;
			this.nameTable = reader.NameTable;
			this.positionInfo = PositionInfo.GetPositionInfo(reader);
			this.elementName = new XmlQualifiedName();
		}

		public XmlValidatingReaderImpl Reader
		{
			get
			{
				return this.reader;
			}
		}

		public XmlSchemaCollection SchemaCollection
		{
			get
			{
				return this.schemaCollection;
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		public SchemaNames SchemaNames
		{
			get
			{
				if (this.schemaNames != null)
				{
					return this.schemaNames;
				}
				if (this.schemaCollection != null)
				{
					this.schemaNames = this.schemaCollection.GetSchemaNames(this.nameTable);
				}
				else
				{
					this.schemaNames = new SchemaNames(this.nameTable);
				}
				return this.schemaNames;
			}
		}

		public PositionInfo PositionInfo
		{
			get
			{
				return this.positionInfo;
			}
		}

		public XmlResolver XmlResolver
		{
			get
			{
				return this.xmlResolver;
			}
			set
			{
				this.xmlResolver = value;
			}
		}

		public Uri BaseUri
		{
			get
			{
				return this.baseUri;
			}
			set
			{
				this.baseUri = value;
			}
		}

		public ValidationEventHandler EventHandler
		{
			get
			{
				return (ValidationEventHandler)this.eventHandling.EventHandler;
			}
		}

		public SchemaInfo SchemaInfo
		{
			get
			{
				return this.schemaInfo;
			}
			set
			{
				this.schemaInfo = value;
			}
		}

		public IDtdInfo DtdInfo
		{
			get
			{
				return this.schemaInfo;
			}
			set
			{
				SchemaInfo schemaInfo = value as SchemaInfo;
				if (schemaInfo == null)
				{
					throw new XmlException("An internal error has occurred.", string.Empty);
				}
				this.schemaInfo = schemaInfo;
			}
		}

		public virtual bool PreserveWhitespace
		{
			get
			{
				return false;
			}
		}

		public virtual void Validate()
		{
		}

		public virtual void CompleteValidation()
		{
		}

		public virtual object FindId(string name)
		{
			return null;
		}

		public void ValidateText()
		{
			if (this.context.NeedValidateChildren)
			{
				if (this.context.IsNill)
				{
					this.SendValidationEvent("Element '{0}' must have no character or element children.", XmlSchemaValidator.QNameString(this.context.LocalName, this.context.Namespace));
					return;
				}
				ContentValidator contentValidator = this.context.ElementDecl.ContentValidator;
				XmlSchemaContentType contentType = contentValidator.ContentType;
				if (contentType == XmlSchemaContentType.ElementOnly)
				{
					ArrayList arrayList = contentValidator.ExpectedElements(this.context, false);
					if (arrayList == null)
					{
						this.SendValidationEvent("The element {0} cannot contain text.", XmlSchemaValidator.BuildElementName(this.context.LocalName, this.context.Namespace));
					}
					else
					{
						this.SendValidationEvent("The element {0} cannot contain text. List of possible elements expected: {1}.", new string[]
						{
							XmlSchemaValidator.BuildElementName(this.context.LocalName, this.context.Namespace),
							XmlSchemaValidator.PrintExpectedElements(arrayList, false)
						});
					}
				}
				else if (contentType == XmlSchemaContentType.Empty)
				{
					this.SendValidationEvent("The element cannot contain text. Content model is empty.", string.Empty);
				}
				if (this.checkDatatype)
				{
					this.SaveTextValue(this.reader.Value);
				}
			}
		}

		public void ValidateWhitespace()
		{
			if (this.context.NeedValidateChildren)
			{
				int contentType = (int)this.context.ElementDecl.ContentValidator.ContentType;
				if (this.context.IsNill)
				{
					this.SendValidationEvent("Element '{0}' must have no character or element children.", XmlSchemaValidator.QNameString(this.context.LocalName, this.context.Namespace));
				}
				if (contentType == 1)
				{
					this.SendValidationEvent("The element cannot contain white space. Content model is empty.", string.Empty);
				}
				if (this.checkDatatype)
				{
					this.SaveTextValue(this.reader.Value);
				}
			}
		}

		private void SaveTextValue(string value)
		{
			if (this.textString.Length == 0)
			{
				this.textString = value;
				return;
			}
			if (!this.hasSibling)
			{
				this.textValue.Append(this.textString);
				this.hasSibling = true;
			}
			this.textValue.Append(value);
		}

		protected void SendValidationEvent(string code)
		{
			this.SendValidationEvent(code, string.Empty);
		}

		protected void SendValidationEvent(string code, string[] args)
		{
			this.SendValidationEvent(new XmlSchemaException(code, args, this.reader.BaseURI, this.positionInfo.LineNumber, this.positionInfo.LinePosition));
		}

		protected void SendValidationEvent(string code, string arg)
		{
			this.SendValidationEvent(new XmlSchemaException(code, arg, this.reader.BaseURI, this.positionInfo.LineNumber, this.positionInfo.LinePosition));
		}

		protected void SendValidationEvent(string code, string arg1, string arg2)
		{
			this.SendValidationEvent(new XmlSchemaException(code, new string[] { arg1, arg2 }, this.reader.BaseURI, this.positionInfo.LineNumber, this.positionInfo.LinePosition));
		}

		protected void SendValidationEvent(XmlSchemaException e)
		{
			this.SendValidationEvent(e, XmlSeverityType.Error);
		}

		protected void SendValidationEvent(string code, string msg, XmlSeverityType severity)
		{
			this.SendValidationEvent(new XmlSchemaException(code, msg, this.reader.BaseURI, this.positionInfo.LineNumber, this.positionInfo.LinePosition), severity);
		}

		protected void SendValidationEvent(string code, string[] args, XmlSeverityType severity)
		{
			this.SendValidationEvent(new XmlSchemaException(code, args, this.reader.BaseURI, this.positionInfo.LineNumber, this.positionInfo.LinePosition), severity);
		}

		protected void SendValidationEvent(XmlSchemaException e, XmlSeverityType severity)
		{
			if (this.eventHandling != null)
			{
				this.eventHandling.SendEvent(e, severity);
				return;
			}
			if (severity == XmlSeverityType.Error)
			{
				throw e;
			}
		}

		protected static void ProcessEntity(SchemaInfo sinfo, string name, object sender, ValidationEventHandler eventhandler, string baseUri, int lineNumber, int linePosition)
		{
			XmlSchemaException ex = null;
			SchemaEntity schemaEntity;
			if (!sinfo.GeneralEntities.TryGetValue(new XmlQualifiedName(name), out schemaEntity))
			{
				ex = new XmlSchemaException("Reference to an undeclared entity, '{0}'.", name, baseUri, lineNumber, linePosition);
			}
			else if (schemaEntity.NData.IsEmpty)
			{
				ex = new XmlSchemaException("Reference to an unparsed entity, '{0}'.", name, baseUri, lineNumber, linePosition);
			}
			if (ex == null)
			{
				return;
			}
			if (eventhandler != null)
			{
				eventhandler(sender, new ValidationEventArgs(ex));
				return;
			}
			throw ex;
		}

		protected static void ProcessEntity(SchemaInfo sinfo, string name, IValidationEventHandling eventHandling, string baseUriStr, int lineNumber, int linePosition)
		{
			string text = null;
			SchemaEntity schemaEntity;
			if (!sinfo.GeneralEntities.TryGetValue(new XmlQualifiedName(name), out schemaEntity))
			{
				text = "Reference to an undeclared entity, '{0}'.";
			}
			else if (schemaEntity.NData.IsEmpty)
			{
				text = "Reference to an unparsed entity, '{0}'.";
			}
			if (text == null)
			{
				return;
			}
			XmlSchemaException ex = new XmlSchemaException(text, name, baseUriStr, lineNumber, linePosition);
			if (eventHandling != null)
			{
				eventHandling.SendEvent(ex, XmlSeverityType.Error);
				return;
			}
			throw ex;
		}

		public static BaseValidator CreateInstance(ValidationType valType, XmlValidatingReaderImpl reader, XmlSchemaCollection schemaCollection, IValidationEventHandling eventHandling, bool processIdentityConstraints)
		{
			switch (valType)
			{
			case ValidationType.None:
				return new BaseValidator(reader, schemaCollection, eventHandling);
			case ValidationType.Auto:
				return new AutoValidator(reader, schemaCollection, eventHandling);
			case ValidationType.DTD:
				return new DtdValidator(reader, eventHandling, processIdentityConstraints);
			case ValidationType.XDR:
				return new XdrValidator(reader, schemaCollection, eventHandling);
			case ValidationType.Schema:
				return new XsdValidator(reader, schemaCollection, eventHandling);
			default:
				return null;
			}
		}

		private XmlSchemaCollection schemaCollection;

		private IValidationEventHandling eventHandling;

		private XmlNameTable nameTable;

		private SchemaNames schemaNames;

		private PositionInfo positionInfo;

		private XmlResolver xmlResolver;

		private Uri baseUri;

		protected SchemaInfo schemaInfo;

		protected XmlValidatingReaderImpl reader;

		protected XmlQualifiedName elementName;

		protected ValidationState context;

		protected StringBuilder textValue;

		protected string textString;

		protected bool hasSibling;

		protected bool checkDatatype;
	}
}
