using System;
using System.Collections;
using System.IO;
using System.Text;

namespace System.Xml.Schema
{
	internal sealed class XdrValidator : BaseValidator
	{
		internal XdrValidator(BaseValidator validator)
			: base(validator)
		{
			this.Init();
		}

		internal XdrValidator(XmlValidatingReaderImpl reader, XmlSchemaCollection schemaCollection, IValidationEventHandling eventHandling)
			: base(reader, schemaCollection, eventHandling)
		{
			this.Init();
		}

		private void Init()
		{
			this.nsManager = this.reader.NamespaceManager;
			if (this.nsManager == null)
			{
				this.nsManager = new XmlNamespaceManager(base.NameTable);
				this.isProcessContents = true;
			}
			this.validationStack = new HWStack(10);
			this.textValue = new StringBuilder();
			this.name = XmlQualifiedName.Empty;
			this.attPresence = new Hashtable();
			this.Push(XmlQualifiedName.Empty);
			this.schemaInfo = new SchemaInfo();
			this.checkDatatype = false;
		}

		public override void Validate()
		{
			if (this.IsInlineSchemaStarted)
			{
				this.ProcessInlineSchema();
				return;
			}
			XmlNodeType nodeType = this.reader.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType - XmlNodeType.Text > 1)
				{
					switch (nodeType)
					{
					case XmlNodeType.Whitespace:
						base.ValidateWhitespace();
						return;
					case XmlNodeType.SignificantWhitespace:
						break;
					case XmlNodeType.EndElement:
						goto IL_005E;
					default:
						return;
					}
				}
				base.ValidateText();
				return;
			}
			this.ValidateElement();
			if (!this.reader.IsEmptyElement)
			{
				return;
			}
			IL_005E:
			this.ValidateEndElement();
		}

		private void ValidateElement()
		{
			this.elementName.Init(this.reader.LocalName, XmlSchemaDatatype.XdrCanonizeUri(this.reader.NamespaceURI, base.NameTable, base.SchemaNames));
			this.ValidateChildElement();
			if (base.SchemaNames.IsXDRRoot(this.elementName.Name, this.elementName.Namespace) && this.reader.Depth > 0)
			{
				this.inlineSchemaParser = new Parser(SchemaType.XDR, base.NameTable, base.SchemaNames, base.EventHandler);
				this.inlineSchemaParser.StartParsing(this.reader, null);
				this.inlineSchemaParser.ParseReaderNode();
				return;
			}
			this.ProcessElement();
		}

		private void ValidateChildElement()
		{
			if (this.context.NeedValidateChildren)
			{
				int num = 0;
				this.context.ElementDecl.ContentValidator.ValidateElement(this.elementName, this.context, out num);
				if (num < 0)
				{
					XmlSchemaValidator.ElementValidationError(this.elementName, this.context, base.EventHandler, this.reader, this.reader.BaseURI, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition, null);
				}
			}
		}

		private bool IsInlineSchemaStarted
		{
			get
			{
				return this.inlineSchemaParser != null;
			}
		}

		private void ProcessInlineSchema()
		{
			if (!this.inlineSchemaParser.ParseReaderNode())
			{
				this.inlineSchemaParser.FinishParsing();
				SchemaInfo xdrSchema = this.inlineSchemaParser.XdrSchema;
				if (xdrSchema != null && xdrSchema.ErrorCount == 0)
				{
					foreach (string text in xdrSchema.TargetNamespaces.Keys)
					{
						if (!this.schemaInfo.HasSchema(text))
						{
							this.schemaInfo.Add(xdrSchema, base.EventHandler);
							base.SchemaCollection.Add(text, xdrSchema, null, false);
							break;
						}
					}
				}
				this.inlineSchemaParser = null;
			}
		}

		private void ProcessElement()
		{
			this.Push(this.elementName);
			if (this.isProcessContents)
			{
				this.nsManager.PopScope();
			}
			this.context.ElementDecl = this.ThoroughGetElementDecl();
			if (this.context.ElementDecl != null)
			{
				this.ValidateStartElement();
				this.ValidateEndStartElement();
				this.context.NeedValidateChildren = true;
				this.context.ElementDecl.ContentValidator.InitValidation(this.context);
			}
		}

		private void ValidateEndElement()
		{
			if (this.isProcessContents)
			{
				this.nsManager.PopScope();
			}
			if (this.context.ElementDecl != null)
			{
				if (this.context.NeedValidateChildren && !this.context.ElementDecl.ContentValidator.CompleteValidation(this.context))
				{
					XmlSchemaValidator.CompleteValidationError(this.context, base.EventHandler, this.reader, this.reader.BaseURI, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition, null);
				}
				if (this.checkDatatype)
				{
					string text = ((!this.hasSibling) ? this.textString : this.textValue.ToString());
					this.CheckValue(text, null);
					this.checkDatatype = false;
					this.textValue.Length = 0;
					this.textString = string.Empty;
				}
			}
			this.Pop();
		}

		private SchemaElementDecl ThoroughGetElementDecl()
		{
			if (this.reader.Depth == 0)
			{
				this.LoadSchema(string.Empty);
			}
			if (this.reader.MoveToFirstAttribute())
			{
				do
				{
					string namespaceURI = this.reader.NamespaceURI;
					string localName = this.reader.LocalName;
					if (Ref.Equal(namespaceURI, base.SchemaNames.NsXmlNs))
					{
						this.LoadSchema(this.reader.Value);
						if (this.isProcessContents)
						{
							this.nsManager.AddNamespace((this.reader.Prefix.Length == 0) ? string.Empty : this.reader.LocalName, this.reader.Value);
						}
					}
					if (Ref.Equal(namespaceURI, base.SchemaNames.QnDtDt.Namespace) && Ref.Equal(localName, base.SchemaNames.QnDtDt.Name))
					{
						this.reader.SchemaTypeObject = XmlSchemaDatatype.FromXdrName(this.reader.Value);
					}
				}
				while (this.reader.MoveToNextAttribute());
				this.reader.MoveToElement();
			}
			SchemaElementDecl elementDecl = this.schemaInfo.GetElementDecl(this.elementName);
			if (elementDecl == null && this.schemaInfo.TargetNamespaces.ContainsKey(this.context.Namespace))
			{
				base.SendValidationEvent("The '{0}' element is not declared.", XmlSchemaValidator.QNameString(this.context.LocalName, this.context.Namespace));
			}
			return elementDecl;
		}

		private void ValidateStartElement()
		{
			if (this.context.ElementDecl != null)
			{
				if (this.context.ElementDecl.SchemaType != null)
				{
					this.reader.SchemaTypeObject = this.context.ElementDecl.SchemaType;
				}
				else
				{
					this.reader.SchemaTypeObject = this.context.ElementDecl.Datatype;
				}
				if (this.reader.IsEmptyElement && !this.context.IsNill && this.context.ElementDecl.DefaultValueTyped != null)
				{
					this.reader.TypedValueObject = this.context.ElementDecl.DefaultValueTyped;
					this.context.IsNill = true;
				}
				if (this.context.ElementDecl.HasRequiredAttribute)
				{
					this.attPresence.Clear();
				}
			}
			if (this.reader.MoveToFirstAttribute())
			{
				do
				{
					if (this.reader.NamespaceURI != base.SchemaNames.NsXmlNs)
					{
						try
						{
							this.reader.SchemaTypeObject = null;
							SchemaAttDef attributeXdr = this.schemaInfo.GetAttributeXdr(this.context.ElementDecl, this.QualifiedName(this.reader.LocalName, this.reader.NamespaceURI));
							if (attributeXdr != null)
							{
								if (this.context.ElementDecl != null && this.context.ElementDecl.HasRequiredAttribute)
								{
									this.attPresence.Add(attributeXdr.Name, attributeXdr);
								}
								this.reader.SchemaTypeObject = ((attributeXdr.SchemaType != null) ? attributeXdr.SchemaType : attributeXdr.Datatype);
								if (attributeXdr.Datatype != null)
								{
									string value = this.reader.Value;
									this.CheckValue(value, attributeXdr);
								}
							}
						}
						catch (XmlSchemaException ex)
						{
							ex.SetSource(this.reader.BaseURI, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition);
							base.SendValidationEvent(ex);
						}
					}
				}
				while (this.reader.MoveToNextAttribute());
				this.reader.MoveToElement();
			}
		}

		private void ValidateEndStartElement()
		{
			if (this.context.ElementDecl.HasDefaultAttribute)
			{
				for (int i = 0; i < this.context.ElementDecl.DefaultAttDefs.Count; i++)
				{
					this.reader.AddDefaultAttribute((SchemaAttDef)this.context.ElementDecl.DefaultAttDefs[i]);
				}
			}
			if (this.context.ElementDecl.HasRequiredAttribute)
			{
				try
				{
					this.context.ElementDecl.CheckAttributes(this.attPresence, this.reader.StandAlone);
				}
				catch (XmlSchemaException ex)
				{
					ex.SetSource(this.reader.BaseURI, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition);
					base.SendValidationEvent(ex);
				}
			}
			if (this.context.ElementDecl.Datatype != null)
			{
				this.checkDatatype = true;
				this.hasSibling = false;
				this.textString = string.Empty;
				this.textValue.Length = 0;
			}
		}

		private void LoadSchemaFromLocation(string uri)
		{
			if (!XdrBuilder.IsXdrSchema(uri))
			{
				return;
			}
			string text = uri.Substring("x-schema:".Length);
			XmlReader xmlReader = null;
			SchemaInfo schemaInfo = null;
			try
			{
				Uri uri2 = base.XmlResolver.ResolveUri(base.BaseUri, text);
				Stream stream = (Stream)base.XmlResolver.GetEntity(uri2, null, null);
				xmlReader = new XmlTextReader(uri2.ToString(), stream, base.NameTable);
				((XmlTextReader)xmlReader).XmlResolver = base.XmlResolver;
				Parser parser = new Parser(SchemaType.XDR, base.NameTable, base.SchemaNames, base.EventHandler);
				parser.XmlResolver = base.XmlResolver;
				parser.Parse(xmlReader, uri);
				while (xmlReader.Read())
				{
				}
				schemaInfo = parser.XdrSchema;
			}
			catch (XmlSchemaException ex)
			{
				base.SendValidationEvent("Cannot load the schema for the namespace '{0}' - {1}", new string[] { uri, ex.Message }, XmlSeverityType.Error);
			}
			catch (Exception ex2)
			{
				base.SendValidationEvent("Cannot load the schema for the namespace '{0}' - {1}", new string[] { uri, ex2.Message }, XmlSeverityType.Warning);
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
			}
			if (schemaInfo != null && schemaInfo.ErrorCount == 0)
			{
				this.schemaInfo.Add(schemaInfo, base.EventHandler);
				base.SchemaCollection.Add(uri, schemaInfo, null, false);
			}
		}

		private void LoadSchema(string uri)
		{
			if (this.schemaInfo.TargetNamespaces.ContainsKey(uri))
			{
				return;
			}
			if (base.XmlResolver == null)
			{
				return;
			}
			SchemaInfo schemaInfo = null;
			if (base.SchemaCollection != null)
			{
				schemaInfo = base.SchemaCollection.GetSchemaInfo(uri);
			}
			if (schemaInfo == null)
			{
				this.LoadSchemaFromLocation(uri);
				return;
			}
			if (schemaInfo.SchemaType != SchemaType.XDR)
			{
				throw new XmlException("Unsupported combination of validation types.", string.Empty, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition);
			}
			this.schemaInfo.Add(schemaInfo, base.EventHandler);
		}

		private bool HasSchema
		{
			get
			{
				return this.schemaInfo.SchemaType > SchemaType.None;
			}
		}

		public override bool PreserveWhitespace
		{
			get
			{
				return this.context.ElementDecl != null && this.context.ElementDecl.ContentValidator.PreserveWhitespace;
			}
		}

		private void ProcessTokenizedType(XmlTokenizedType ttype, string name)
		{
			switch (ttype)
			{
			case XmlTokenizedType.ID:
				if (this.FindId(name) != null)
				{
					base.SendValidationEvent("'{0}' is already used as an ID.", name);
					return;
				}
				this.AddID(name, this.context.LocalName);
				return;
			case XmlTokenizedType.IDREF:
				if (this.FindId(name) == null)
				{
					this.idRefListHead = new IdRefNode(this.idRefListHead, name, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition);
					return;
				}
				break;
			case XmlTokenizedType.IDREFS:
				break;
			case XmlTokenizedType.ENTITY:
				BaseValidator.ProcessEntity(this.schemaInfo, name, this, base.EventHandler, this.reader.BaseURI, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition);
				break;
			default:
				return;
			}
		}

		public override void CompleteValidation()
		{
			if (this.HasSchema)
			{
				this.CheckForwardRefs();
				return;
			}
			base.SendValidationEvent(new XmlSchemaException("No validation occurred.", string.Empty), XmlSeverityType.Warning);
		}

		private void CheckValue(string value, SchemaAttDef attdef)
		{
			try
			{
				this.reader.TypedValueObject = null;
				bool flag = attdef != null;
				XmlSchemaDatatype xmlSchemaDatatype = (flag ? attdef.Datatype : this.context.ElementDecl.Datatype);
				if (xmlSchemaDatatype != null)
				{
					if (xmlSchemaDatatype.TokenizedType != XmlTokenizedType.CDATA)
					{
						value = value.Trim();
					}
					if (value.Length != 0)
					{
						object obj = xmlSchemaDatatype.ParseValue(value, base.NameTable, this.nsManager);
						this.reader.TypedValueObject = obj;
						XmlTokenizedType tokenizedType = xmlSchemaDatatype.TokenizedType;
						if (tokenizedType == XmlTokenizedType.ENTITY || tokenizedType == XmlTokenizedType.ID || tokenizedType == XmlTokenizedType.IDREF)
						{
							if (xmlSchemaDatatype.Variety == XmlSchemaDatatypeVariety.List)
							{
								string[] array = (string[])obj;
								for (int i = 0; i < array.Length; i++)
								{
									this.ProcessTokenizedType(xmlSchemaDatatype.TokenizedType, array[i]);
								}
							}
							else
							{
								this.ProcessTokenizedType(xmlSchemaDatatype.TokenizedType, (string)obj);
							}
						}
						SchemaDeclBase schemaDeclBase = (flag ? attdef : this.context.ElementDecl);
						if (schemaDeclBase.MaxLength != (long)((ulong)(-1)) && (long)value.Length > schemaDeclBase.MaxLength)
						{
							base.SendValidationEvent("The actual length is greater than the MaxLength value.", value);
						}
						if (schemaDeclBase.MinLength != (long)((ulong)(-1)) && (long)value.Length < schemaDeclBase.MinLength)
						{
							base.SendValidationEvent("The actual length is less than the MinLength value.", value);
						}
						if (schemaDeclBase.Values != null && !schemaDeclBase.CheckEnumeration(obj))
						{
							if (xmlSchemaDatatype.TokenizedType == XmlTokenizedType.NOTATION)
							{
								base.SendValidationEvent("'{0}' is not in the notation list.", obj.ToString());
							}
							else
							{
								base.SendValidationEvent("'{0}' is not in the enumeration list.", obj.ToString());
							}
						}
						if (!schemaDeclBase.CheckValue(obj))
						{
							if (flag)
							{
								base.SendValidationEvent("The value of the '{0}' attribute does not equal its fixed value.", attdef.Name.ToString());
							}
							else
							{
								base.SendValidationEvent("The value of the '{0}' element does not equal its fixed value.", XmlSchemaValidator.QNameString(this.context.LocalName, this.context.Namespace));
							}
						}
					}
				}
			}
			catch (XmlSchemaException)
			{
				if (attdef != null)
				{
					base.SendValidationEvent("The '{0}' attribute has an invalid value according to its data type.", attdef.Name.ToString());
				}
				else
				{
					base.SendValidationEvent("The '{0}' element has an invalid value according to its data type.", XmlSchemaValidator.QNameString(this.context.LocalName, this.context.Namespace));
				}
			}
		}

		public static void CheckDefaultValue(string value, SchemaAttDef attdef, SchemaInfo sinfo, XmlNamespaceManager nsManager, XmlNameTable NameTable, object sender, ValidationEventHandler eventhandler, string baseUri, int lineNo, int linePos)
		{
			try
			{
				XmlSchemaDatatype datatype = attdef.Datatype;
				if (datatype != null)
				{
					if (datatype.TokenizedType != XmlTokenizedType.CDATA)
					{
						value = value.Trim();
					}
					if (value.Length != 0)
					{
						object obj = datatype.ParseValue(value, NameTable, nsManager);
						XmlTokenizedType tokenizedType = datatype.TokenizedType;
						if (tokenizedType == XmlTokenizedType.ENTITY)
						{
							if (datatype.Variety == XmlSchemaDatatypeVariety.List)
							{
								string[] array = (string[])obj;
								for (int i = 0; i < array.Length; i++)
								{
									BaseValidator.ProcessEntity(sinfo, array[i], sender, eventhandler, baseUri, lineNo, linePos);
								}
							}
							else
							{
								BaseValidator.ProcessEntity(sinfo, (string)obj, sender, eventhandler, baseUri, lineNo, linePos);
							}
						}
						else if (tokenizedType == XmlTokenizedType.ENUMERATION && !attdef.CheckEnumeration(obj))
						{
							XmlSchemaException ex = new XmlSchemaException("'{0}' is not in the enumeration list.", obj.ToString(), baseUri, lineNo, linePos);
							if (eventhandler == null)
							{
								throw ex;
							}
							eventhandler(sender, new ValidationEventArgs(ex));
						}
						attdef.DefaultValueTyped = obj;
					}
				}
			}
			catch
			{
				XmlSchemaException ex2 = new XmlSchemaException("The default value of '{0}' attribute is invalid according to its datatype.", attdef.Name.ToString(), baseUri, lineNo, linePos);
				if (eventhandler == null)
				{
					throw ex2;
				}
				eventhandler(sender, new ValidationEventArgs(ex2));
			}
		}

		internal void AddID(string name, object node)
		{
			if (this.IDs == null)
			{
				this.IDs = new Hashtable();
			}
			this.IDs.Add(name, node);
		}

		public override object FindId(string name)
		{
			if (this.IDs != null)
			{
				return this.IDs[name];
			}
			return null;
		}

		private void Push(XmlQualifiedName elementName)
		{
			this.context = (ValidationState)this.validationStack.Push();
			if (this.context == null)
			{
				this.context = new ValidationState();
				this.validationStack.AddToTop(this.context);
			}
			this.context.LocalName = elementName.Name;
			this.context.Namespace = elementName.Namespace;
			this.context.HasMatched = false;
			this.context.IsNill = false;
			this.context.NeedValidateChildren = false;
		}

		private void Pop()
		{
			if (this.validationStack.Length > 1)
			{
				this.validationStack.Pop();
				this.context = (ValidationState)this.validationStack.Peek();
			}
		}

		private void CheckForwardRefs()
		{
			IdRefNode next;
			for (IdRefNode idRefNode = this.idRefListHead; idRefNode != null; idRefNode = next)
			{
				if (this.FindId(idRefNode.Id) == null)
				{
					base.SendValidationEvent(new XmlSchemaException("Reference to undeclared ID is '{0}'.", idRefNode.Id, this.reader.BaseURI, idRefNode.LineNo, idRefNode.LinePos));
				}
				next = idRefNode.Next;
				idRefNode.Next = null;
			}
			this.idRefListHead = null;
		}

		private XmlQualifiedName QualifiedName(string name, string ns)
		{
			return new XmlQualifiedName(name, XmlSchemaDatatype.XdrCanonizeUri(ns, base.NameTable, base.SchemaNames));
		}

		private const int STACK_INCREMENT = 10;

		private HWStack validationStack;

		private Hashtable attPresence;

		private XmlQualifiedName name = XmlQualifiedName.Empty;

		private XmlNamespaceManager nsManager;

		private bool isProcessContents;

		private Hashtable IDs;

		private IdRefNode idRefListHead;

		private Parser inlineSchemaParser;

		private const string x_schema = "x-schema:";
	}
}
