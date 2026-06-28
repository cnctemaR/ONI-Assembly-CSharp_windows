using System;
using System.Collections;
using System.IO;
using System.Text;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	public sealed class XmlSchemaValidator
	{
		public XmlSchemaValidator(XmlNameTable nameTable, XmlSchemaSet schemas, IXmlNamespaceResolver nsResolver, XmlSchemaValidationFlags options)
		{
			this.nameTable = nameTable;
			this.schemas = schemas;
			this.nsResolver = nsResolver;
			this.options = options;
		}

		public event ValidationEventHandler ValidationEventHandler;

		public object ValidationEventSender
		{
			get
			{
				return this.nominalEventSender;
			}
			set
			{
				this.nominalEventSender = value;
			}
		}

		public IXmlLineInfo LineInfoProvider
		{
			get
			{
				return this.lineInfo;
			}
			set
			{
				this.lineInfo = value;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				this.xmlResolver = value;
			}
		}

		public Uri SourceUri
		{
			get
			{
				return this.sourceUri;
			}
			set
			{
				this.sourceUri = value;
			}
		}

		private string BaseUri
		{
			get
			{
				return (!(this.sourceUri != null)) ? string.Empty : this.sourceUri.AbsoluteUri;
			}
		}

		private XsdValidationContext Context
		{
			get
			{
				return this.state.Context;
			}
		}

		private bool IgnoreWarnings
		{
			get
			{
				return (this.options & XmlSchemaValidationFlags.ReportValidationWarnings) == XmlSchemaValidationFlags.None;
			}
		}

		private bool IgnoreIdentity
		{
			get
			{
				return (this.options & XmlSchemaValidationFlags.ProcessIdentityConstraints) == XmlSchemaValidationFlags.None;
			}
		}

		public XmlSchemaAttribute[] GetExpectedAttributes()
		{
			XmlSchemaComplexType xmlSchemaComplexType = this.Context.ActualType as XmlSchemaComplexType;
			if (xmlSchemaComplexType == null)
			{
				return XmlSchemaValidator.emptyAttributeArray;
			}
			ArrayList arrayList = new ArrayList();
			foreach (object obj in xmlSchemaComplexType.AttributeUses)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				if (!this.occuredAtts.Contains((XmlQualifiedName)dictionaryEntry.Key))
				{
					arrayList.Add(dictionaryEntry.Value);
				}
			}
			return (XmlSchemaAttribute[])arrayList.ToArray(typeof(XmlSchemaAttribute));
		}

		private void CollectAtomicParticles(XmlSchemaParticle p, ArrayList al)
		{
			if (p is XmlSchemaGroupBase)
			{
				foreach (XmlSchemaObject xmlSchemaObject in ((XmlSchemaGroupBase)p).Items)
				{
					XmlSchemaParticle xmlSchemaParticle = (XmlSchemaParticle)xmlSchemaObject;
					this.CollectAtomicParticles(xmlSchemaParticle, al);
				}
			}
			else
			{
				al.Add(p);
			}
		}

		[MonoTODO]
		public XmlSchemaParticle[] GetExpectedParticles()
		{
			ArrayList arrayList = new ArrayList();
			this.Context.State.GetExpectedParticles(arrayList);
			ArrayList arrayList2 = new ArrayList();
			foreach (object obj in arrayList)
			{
				XmlSchemaParticle xmlSchemaParticle = (XmlSchemaParticle)obj;
				this.CollectAtomicParticles(xmlSchemaParticle, arrayList2);
			}
			return (XmlSchemaParticle[])arrayList2.ToArray(typeof(XmlSchemaParticle));
		}

		public void GetUnspecifiedDefaultAttributes(ArrayList defaultAttributeList)
		{
			if (defaultAttributeList == null)
			{
				throw new ArgumentNullException("defaultAttributeList");
			}
			if (this.transition != XmlSchemaValidator.Transition.StartTag)
			{
				throw new InvalidOperationException("Method 'GetUnsoecifiedDefaultAttributes' works only when the validator state is inside a start tag.");
			}
			foreach (XmlSchemaAttribute xmlSchemaAttribute in this.GetExpectedAttributes())
			{
				if (xmlSchemaAttribute.ValidatedDefaultValue != null || xmlSchemaAttribute.ValidatedFixedValue != null)
				{
					defaultAttributeList.Add(xmlSchemaAttribute);
				}
			}
			defaultAttributeList.AddRange(this.defaultAttributes);
		}

		public void AddSchema(XmlSchema schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			this.schemas.Add(schema);
			this.schemas.Compile();
		}

		public void Initialize()
		{
			this.transition = XmlSchemaValidator.Transition.Content;
			this.state = new XsdParticleStateManager();
			if (!this.schemas.IsCompiled)
			{
				this.schemas.Compile();
			}
		}

		public void Initialize(XmlSchemaObject partialValidationType)
		{
			if (partialValidationType == null)
			{
				throw new ArgumentNullException("partialValidationType");
			}
			this.startType = partialValidationType;
			this.Initialize();
		}

		public void EndValidation()
		{
			this.CheckState(XmlSchemaValidator.Transition.Content);
			this.transition = XmlSchemaValidator.Transition.Finished;
			if (this.schemas.Count == 0)
			{
				return;
			}
			if (this.depth > 0)
			{
				throw new InvalidOperationException(string.Format("There are {0} open element(s). ValidateEndElement() must be called for each open element.", this.depth));
			}
			if (!this.IgnoreIdentity && this.idManager.HasMissingIDReferences())
			{
				this.HandleError("There are missing ID references: " + this.idManager.GetMissingIDString());
			}
		}

		[MonoTODO]
		public void SkipToEndElement(XmlSchemaInfo info)
		{
			this.CheckState(XmlSchemaValidator.Transition.Content);
			if (this.schemas.Count == 0)
			{
				return;
			}
			this.state.PopContext();
		}

		public object ValidateAttribute(string localName, string ns, string attributeValue, XmlSchemaInfo info)
		{
			if (attributeValue == null)
			{
				throw new ArgumentNullException("attributeValue");
			}
			return this.ValidateAttribute(localName, ns, () => attributeValue, info);
		}

		public object ValidateAttribute(string localName, string ns, XmlValueGetter attributeValue, XmlSchemaInfo info)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (ns == null)
			{
				throw new ArgumentNullException("ns");
			}
			if (attributeValue == null)
			{
				throw new ArgumentNullException("attributeValue");
			}
			this.CheckState(XmlSchemaValidator.Transition.StartTag);
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(localName, ns);
			if (this.occuredAtts.Contains(xmlQualifiedName))
			{
				throw new InvalidOperationException(string.Format("Attribute '{0}' has already been validated in the same element.", xmlQualifiedName));
			}
			this.occuredAtts.Add(xmlQualifiedName);
			if (ns == "http://www.w3.org/2000/xmlns/")
			{
				return null;
			}
			if (this.schemas.Count == 0)
			{
				return null;
			}
			if (this.Context.Element != null && this.Context.XsiType == null)
			{
				if (this.Context.ActualType is XmlSchemaComplexType)
				{
					return this.AssessAttributeElementLocallyValidType(localName, ns, attributeValue, info);
				}
				this.HandleError("Current simple type cannot accept attributes other than schema instance namespace.");
			}
			return null;
		}

		public void ValidateElement(string localName, string ns, XmlSchemaInfo info)
		{
			this.ValidateElement(localName, ns, info, null, null, null, null);
		}

		public void ValidateElement(string localName, string ns, XmlSchemaInfo info, string xsiType, string xsiNil, string schemaLocation, string noNsSchemaLocation)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (ns == null)
			{
				throw new ArgumentNullException("ns");
			}
			this.CheckState(XmlSchemaValidator.Transition.Content);
			this.transition = XmlSchemaValidator.Transition.StartTag;
			if (schemaLocation != null)
			{
				this.HandleSchemaLocation(schemaLocation);
			}
			if (noNsSchemaLocation != null)
			{
				this.HandleNoNSSchemaLocation(noNsSchemaLocation);
			}
			this.elementQNameStack.Add(new XmlQualifiedName(localName, ns));
			if (this.schemas.Count == 0)
			{
				return;
			}
			if (!this.IgnoreIdentity)
			{
				this.idManager.OnStartElement();
			}
			this.defaultAttributes = XmlSchemaValidator.emptyAttributeArray;
			if (this.skipValidationDepth < 0 || this.depth <= this.skipValidationDepth)
			{
				if (this.shouldValidateCharacters)
				{
					this.ValidateEndSimpleContent(null);
				}
				this.AssessOpenStartElementSchemaValidity(localName, ns);
			}
			if (xsiNil != null)
			{
				this.HandleXsiNil(xsiNil, info);
			}
			if (xsiType != null)
			{
				this.HandleXsiType(xsiType);
			}
			if (this.xsiNilDepth < this.depth)
			{
				this.shouldValidateCharacters = true;
			}
			if (info != null)
			{
				info.IsNil = this.xsiNilDepth >= 0;
				info.SchemaElement = this.Context.Element;
				info.SchemaType = this.Context.ActualSchemaType;
				info.SchemaAttribute = null;
				info.IsDefault = false;
				info.MemberType = null;
			}
		}

		public object ValidateEndElement(XmlSchemaInfo info)
		{
			return this.ValidateEndElement(info, null);
		}

		[MonoTODO]
		public object ValidateEndElement(XmlSchemaInfo info, object var)
		{
			if (this.transition == XmlSchemaValidator.Transition.StartTag)
			{
				this.ValidateEndOfAttributes(info);
			}
			this.CheckState(XmlSchemaValidator.Transition.Content);
			this.elementQNameStack.RemoveAt(this.elementQNameStack.Count - 1);
			if (this.schemas.Count == 0)
			{
				return null;
			}
			if (this.depth == 0)
			{
				throw new InvalidOperationException("There was no corresponding call to 'ValidateElement' method.");
			}
			this.depth--;
			object obj = null;
			if (this.depth == this.skipValidationDepth)
			{
				this.skipValidationDepth = -1;
			}
			else if (this.skipValidationDepth < 0 || this.depth <= this.skipValidationDepth)
			{
				obj = this.AssessEndElementSchemaValidity(info);
			}
			return obj;
		}

		public void ValidateEndOfAttributes(XmlSchemaInfo info)
		{
			try
			{
				this.CheckState(XmlSchemaValidator.Transition.StartTag);
				this.transition = XmlSchemaValidator.Transition.Content;
				if (this.schemas.Count != 0)
				{
					if (this.skipValidationDepth < 0 || this.depth <= this.skipValidationDepth)
					{
						this.AssessCloseStartElementSchemaValidity(info);
					}
					this.depth++;
				}
			}
			finally
			{
				this.occuredAtts.Clear();
			}
		}

		public void ValidateText(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.ValidateText(() => value);
		}

		public void ValidateText(XmlValueGetter getter)
		{
			if (getter == null)
			{
				throw new ArgumentNullException("getter");
			}
			this.CheckState(XmlSchemaValidator.Transition.Content);
			if (this.schemas.Count == 0)
			{
				return;
			}
			if (this.skipValidationDepth >= 0 && this.depth > this.skipValidationDepth)
			{
				return;
			}
			XmlSchemaComplexType xmlSchemaComplexType = this.Context.ActualType as XmlSchemaComplexType;
			if (xmlSchemaComplexType != null)
			{
				XmlSchemaContentType contentType = xmlSchemaComplexType.ContentType;
				if (contentType != XmlSchemaContentType.Empty)
				{
					if (contentType == XmlSchemaContentType.ElementOnly)
					{
						string text = this.storedCharacters.ToString();
						if (text.Length > 0 && !XmlChar.IsWhitespace(text))
						{
							this.HandleError("Not allowed character content was found.");
						}
					}
				}
				else
				{
					this.HandleError("Not allowed character content was found.");
				}
			}
			this.ValidateCharacters(getter);
		}

		public void ValidateWhitespace(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.ValidateWhitespace(() => value);
		}

		public void ValidateWhitespace(XmlValueGetter getter)
		{
			this.ValidateText(getter);
		}

		private void HandleError(string message)
		{
			this.HandleError(message, null, false);
		}

		private void HandleError(string message, Exception innerException)
		{
			this.HandleError(message, innerException, false);
		}

		private void HandleError(string message, Exception innerException, bool isWarning)
		{
			if (isWarning && this.IgnoreWarnings)
			{
				return;
			}
			XmlSchemaValidationException ex = new XmlSchemaValidationException(message, this.nominalEventSender, this.BaseUri, null, innerException);
			this.HandleError(ex, isWarning);
		}

		private void HandleError(XmlSchemaValidationException exception)
		{
			this.HandleError(exception, false);
		}

		private void HandleError(XmlSchemaValidationException exception, bool isWarning)
		{
			if (isWarning && this.IgnoreWarnings)
			{
				return;
			}
			if (this.ValidationEventHandler == null)
			{
				throw exception;
			}
			ValidationEventArgs e = new ValidationEventArgs(exception, exception.Message, (!isWarning) ? XmlSeverityType.Error : XmlSeverityType.Warning);
			this.ValidationEventHandler(this.nominalEventSender, e);
		}

		private void CheckState(XmlSchemaValidator.Transition expected)
		{
			if (this.transition == expected)
			{
				return;
			}
			if (this.transition == XmlSchemaValidator.Transition.None)
			{
				throw new InvalidOperationException("Initialize() must be called before processing validation.");
			}
			throw new InvalidOperationException(string.Format("Unexpected attempt to validate state transition from {0} to {1}.", this.transition, expected));
		}

		private XmlSchemaElement FindElement(string name, string ns)
		{
			return (XmlSchemaElement)this.schemas.GlobalElements[new XmlQualifiedName(name, ns)];
		}

		private XmlSchemaType FindType(XmlQualifiedName qname)
		{
			return (XmlSchemaType)this.schemas.GlobalTypes[qname];
		}

		private void ValidateStartElementParticle(string localName, string ns)
		{
			if (this.Context.State == null)
			{
				return;
			}
			this.Context.XsiType = null;
			this.state.CurrentElement = null;
			this.Context.EvaluateStartElement(localName, ns);
			if (this.Context.IsInvalid)
			{
				this.HandleError("Invalid start element: " + ns + ":" + localName);
			}
			this.Context.PushCurrentElement(this.state.CurrentElement);
		}

		private void AssessOpenStartElementSchemaValidity(string localName, string ns)
		{
			if (this.xsiNilDepth >= 0 && this.xsiNilDepth < this.depth)
			{
				this.HandleError("Element item appeared, while current element context is nil.");
			}
			this.ValidateStartElementParticle(localName, ns);
			if (this.Context.Element == null)
			{
				this.state.CurrentElement = this.FindElement(localName, ns);
				this.Context.PushCurrentElement(this.state.CurrentElement);
			}
			if (!this.IgnoreIdentity)
			{
				this.ValidateKeySelectors();
				this.ValidateKeyFields(false, this.xsiNilDepth == this.depth, this.Context.ActualType, null, null, null);
			}
		}

		private void AssessCloseStartElementSchemaValidity(XmlSchemaInfo info)
		{
			if (this.Context.XsiType != null)
			{
				this.AssessCloseStartElementLocallyValidType(info);
			}
			else if (this.Context.Element != null)
			{
				this.AssessElementLocallyValidElement();
				if (this.Context.Element.ElementType != null)
				{
					this.AssessCloseStartElementLocallyValidType(info);
				}
			}
			if (this.Context.Element == null)
			{
				XmlSchemaContentProcessing processContents = this.state.ProcessContents;
				if (processContents != XmlSchemaContentProcessing.Skip)
				{
					if (processContents != XmlSchemaContentProcessing.Lax)
					{
						XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)this.elementQNameStack[this.elementQNameStack.Count - 1];
						if (this.Context.XsiType == null && (this.schemas.Contains(xmlQualifiedName.Namespace) || !this.schemas.MissedSubComponents(xmlQualifiedName.Namespace)))
						{
							this.HandleError("Element declaration for " + xmlQualifiedName + " is missing.");
						}
					}
				}
			}
			this.state.PushContext();
			XsdValidationState xsdValidationState = null;
			if (this.state.ProcessContents == XmlSchemaContentProcessing.Skip)
			{
				this.skipValidationDepth = this.depth;
			}
			else
			{
				XmlSchemaComplexType xmlSchemaComplexType = this.Context.ActualType as XmlSchemaComplexType;
				if (xmlSchemaComplexType != null)
				{
					xsdValidationState = this.state.Create(xmlSchemaComplexType.ValidatableParticle);
				}
				else if (this.state.ProcessContents == XmlSchemaContentProcessing.Lax)
				{
					xsdValidationState = this.state.Create(XmlSchemaAny.AnyTypeContent);
				}
				else
				{
					xsdValidationState = this.state.Create(XmlSchemaParticle.Empty);
				}
			}
			this.Context.State = xsdValidationState;
		}

		private void AssessElementLocallyValidElement()
		{
			XmlSchemaElement element = this.Context.Element;
			XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)this.elementQNameStack[this.elementQNameStack.Count - 1];
			if (element == null)
			{
				this.HandleError("Element declaration is required for " + xmlQualifiedName);
			}
			if (element.ActualIsAbstract)
			{
				this.HandleError("Abstract element declaration was specified for " + xmlQualifiedName);
			}
		}

		private void AssessCloseStartElementLocallyValidType(XmlSchemaInfo info)
		{
			object actualType = this.Context.ActualType;
			if (actualType == null)
			{
				this.HandleError("Schema type does not exist.");
				return;
			}
			XmlSchemaComplexType xmlSchemaComplexType = actualType as XmlSchemaComplexType;
			XmlSchemaSimpleType xmlSchemaSimpleType = actualType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType == null)
			{
				if (xmlSchemaComplexType != null)
				{
					this.AssessCloseStartElementLocallyValidComplexType(xmlSchemaComplexType, info);
				}
			}
		}

		private void AssessCloseStartElementLocallyValidComplexType(XmlSchemaComplexType cType, XmlSchemaInfo info)
		{
			if (cType.IsAbstract)
			{
				this.HandleError("Target complex type is abstract.");
				return;
			}
			foreach (XmlSchemaAttribute xmlSchemaAttribute in this.GetExpectedAttributes())
			{
				if (xmlSchemaAttribute.ValidatedUse == XmlSchemaUse.Required && xmlSchemaAttribute.ValidatedFixedValue == null)
				{
					this.HandleError("Required attribute " + xmlSchemaAttribute.QualifiedName + " was not found.");
				}
				else if (xmlSchemaAttribute.ValidatedDefaultValue != null || xmlSchemaAttribute.ValidatedFixedValue != null)
				{
					this.defaultAttributesCache.Add(xmlSchemaAttribute);
				}
			}
			if (this.defaultAttributesCache.Count == 0)
			{
				this.defaultAttributes = XmlSchemaValidator.emptyAttributeArray;
			}
			else
			{
				this.defaultAttributes = (XmlSchemaAttribute[])this.defaultAttributesCache.ToArray(typeof(XmlSchemaAttribute));
			}
			this.defaultAttributesCache.Clear();
			if (!this.IgnoreIdentity)
			{
				foreach (XmlSchemaAttribute xmlSchemaAttribute2 in this.defaultAttributes)
				{
					XmlSchemaDatatype xmlSchemaDatatype = (xmlSchemaAttribute2.AttributeType as XmlSchemaDatatype) ?? xmlSchemaAttribute2.AttributeSchemaType.Datatype;
					object obj = xmlSchemaAttribute2.ValidatedFixedValue ?? xmlSchemaAttribute2.ValidatedDefaultValue;
					string text = this.idManager.AssessEachAttributeIdentityConstraint(xmlSchemaDatatype, obj, ((XmlQualifiedName)this.elementQNameStack[this.elementQNameStack.Count - 1]).Name);
					if (text != null)
					{
						this.HandleError(text);
					}
				}
			}
			if (!this.IgnoreIdentity)
			{
				foreach (XmlSchemaAttribute xmlSchemaAttribute3 in this.defaultAttributes)
				{
					this.ValidateKeyFieldsAttribute(xmlSchemaAttribute3, xmlSchemaAttribute3.ValidatedFixedValue ?? xmlSchemaAttribute3.ValidatedDefaultValue);
				}
			}
		}

		private object AssessAttributeElementLocallyValidType(string localName, string ns, XmlValueGetter getter, XmlSchemaInfo info)
		{
			XmlSchemaComplexType xmlSchemaComplexType = this.Context.ActualType as XmlSchemaComplexType;
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(localName, ns);
			XmlSchemaObject xmlSchemaObject = XmlSchemaUtil.FindAttributeDeclaration(ns, this.schemas, xmlSchemaComplexType, xmlQualifiedName);
			if (xmlSchemaObject == null)
			{
				this.HandleError("Attribute declaration was not found for " + xmlQualifiedName);
			}
			XmlSchemaAttribute xmlSchemaAttribute = xmlSchemaObject as XmlSchemaAttribute;
			if (xmlSchemaAttribute != null)
			{
				this.AssessAttributeLocallyValidUse(xmlSchemaAttribute);
				return this.AssessAttributeLocallyValid(xmlSchemaAttribute, info, getter);
			}
			return null;
		}

		private object AssessAttributeLocallyValid(XmlSchemaAttribute attr, XmlSchemaInfo info, XmlValueGetter getter)
		{
			if (attr.AttributeType == null)
			{
				this.HandleError("Attribute type is missing for " + attr.QualifiedName);
			}
			XmlSchemaDatatype xmlSchemaDatatype = attr.AttributeType as XmlSchemaDatatype;
			if (xmlSchemaDatatype == null)
			{
				xmlSchemaDatatype = ((XmlSchemaSimpleType)attr.AttributeType).Datatype;
			}
			object obj = null;
			if (xmlSchemaDatatype == XmlSchemaSimpleType.AnySimpleType)
			{
				if (attr.ValidatedFixedValue == null)
				{
					goto IL_0115;
				}
			}
			try
			{
				this.CurrentAttributeType = xmlSchemaDatatype;
				obj = getter();
			}
			catch (Exception ex)
			{
				this.HandleError(string.Format("Attribute value is invalid against its data type {0}", (xmlSchemaDatatype == null) ? XmlTokenizedType.CDATA : xmlSchemaDatatype.TokenizedType), ex);
			}
			XmlSchemaSimpleType xmlSchemaSimpleType = attr.AttributeType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType != null)
			{
				this.ValidateRestrictedSimpleTypeValue(xmlSchemaSimpleType, ref xmlSchemaDatatype, new XmlAtomicValue(obj, attr.AttributeSchemaType).Value);
			}
			if (attr.ValidatedFixedValue != null)
			{
				if (!XmlSchemaUtil.AreSchemaDatatypeEqual(attr.AttributeSchemaType, attr.ValidatedFixedTypedValue, attr.AttributeSchemaType, obj))
				{
					this.HandleError(string.Format("The value of the attribute {0} does not match with its fixed value '{1}' in the space of type {2}", attr.QualifiedName, attr.ValidatedFixedValue, xmlSchemaDatatype));
				}
				obj = attr.ValidatedFixedTypedValue;
			}
			IL_0115:
			if (!this.IgnoreIdentity)
			{
				string text = this.idManager.AssessEachAttributeIdentityConstraint(xmlSchemaDatatype, obj, ((XmlQualifiedName)this.elementQNameStack[this.elementQNameStack.Count - 1]).Name);
				if (text != null)
				{
					this.HandleError(text);
				}
			}
			if (!this.IgnoreIdentity)
			{
				this.ValidateKeyFieldsAttribute(attr, obj);
			}
			return obj;
		}

		private void AssessAttributeLocallyValidUse(XmlSchemaAttribute attr)
		{
			if (attr.ValidatedUse == XmlSchemaUse.Prohibited)
			{
				this.HandleError("Attribute " + attr.QualifiedName + " is prohibited in this context.");
			}
		}

		private object AssessEndElementSchemaValidity(XmlSchemaInfo info)
		{
			object obj = this.ValidateEndSimpleContent(info);
			this.ValidateEndElementParticle();
			if (!this.IgnoreIdentity)
			{
				this.ValidateEndElementKeyConstraints();
			}
			if (this.xsiNilDepth == this.depth)
			{
				this.xsiNilDepth = -1;
			}
			return obj;
		}

		private void ValidateEndElementParticle()
		{
			if (this.Context.State != null && !this.Context.EvaluateEndElement())
			{
				this.HandleError("Invalid end element. There are still required content items.");
			}
			this.Context.PopCurrentElement();
			this.state.PopContext();
			this.Context.XsiType = null;
		}

		private void ValidateCharacters(XmlValueGetter getter)
		{
			if (this.xsiNilDepth >= 0 && this.xsiNilDepth < this.depth)
			{
				this.HandleError("Element item appeared, while current element context is nil.");
			}
			if (this.shouldValidateCharacters)
			{
				this.CurrentAttributeType = null;
				this.storedCharacters.Append(getter());
			}
		}

		private object ValidateEndSimpleContent(XmlSchemaInfo info)
		{
			object obj = null;
			if (this.shouldValidateCharacters)
			{
				obj = this.ValidateEndSimpleContentCore(info);
			}
			this.shouldValidateCharacters = false;
			this.storedCharacters.Length = 0;
			return obj;
		}

		private object ValidateEndSimpleContentCore(XmlSchemaInfo info)
		{
			if (this.Context.ActualType == null)
			{
				return null;
			}
			string text = this.storedCharacters.ToString();
			object obj = null;
			if (text.Length == 0 && this.Context.Element != null && this.Context.Element.ValidatedDefaultValue != null)
			{
				text = this.Context.Element.ValidatedDefaultValue;
			}
			XmlSchemaDatatype xmlSchemaDatatype = this.Context.ActualType as XmlSchemaDatatype;
			XmlSchemaSimpleType xmlSchemaSimpleType = this.Context.ActualType as XmlSchemaSimpleType;
			if (xmlSchemaDatatype == null)
			{
				if (xmlSchemaSimpleType != null)
				{
					xmlSchemaDatatype = xmlSchemaSimpleType.Datatype;
				}
				else
				{
					XmlSchemaComplexType xmlSchemaComplexType = this.Context.ActualType as XmlSchemaComplexType;
					xmlSchemaDatatype = xmlSchemaComplexType.Datatype;
					XmlSchemaContentType contentType = xmlSchemaComplexType.ContentType;
					if (contentType != XmlSchemaContentType.Empty)
					{
						if (contentType == XmlSchemaContentType.ElementOnly)
						{
							if (text.Length > 0 && !XmlChar.IsWhitespace(text))
							{
								this.HandleError("Character content not allowed in an elementOnly model.");
							}
						}
					}
					else if (text.Length > 0)
					{
						this.HandleError("Character content not allowed in an empty model.");
					}
				}
			}
			if (xmlSchemaDatatype != null)
			{
				if (this.Context.Element != null && this.Context.Element.ValidatedFixedValue != null && text != this.Context.Element.ValidatedFixedValue)
				{
					this.HandleError("Fixed value constraint was not satisfied.");
				}
				obj = this.AssessStringValid(xmlSchemaSimpleType, xmlSchemaDatatype, text);
			}
			if (!this.IgnoreIdentity)
			{
				this.ValidateSimpleContentIdentity(xmlSchemaDatatype, text);
			}
			this.shouldValidateCharacters = false;
			if (info != null)
			{
				info.IsNil = this.xsiNilDepth >= 0;
				info.SchemaElement = null;
				info.SchemaType = this.Context.ActualType as XmlSchemaType;
				if (info.SchemaType == null)
				{
					info.SchemaType = XmlSchemaType.GetBuiltInSimpleType(xmlSchemaDatatype.TypeCode);
				}
				info.SchemaAttribute = null;
				info.IsDefault = false;
				info.MemberType = null;
			}
			return obj;
		}

		private object AssessStringValid(XmlSchemaSimpleType st, XmlSchemaDatatype dt, string value)
		{
			XmlSchemaDatatype xmlSchemaDatatype = dt;
			object obj = null;
			if (st != null)
			{
				string text = xmlSchemaDatatype.Normalize(value);
				XmlSchemaDerivationMethod derivedBy = st.DerivedBy;
				if (derivedBy != XmlSchemaDerivationMethod.Restriction)
				{
					if (derivedBy != XmlSchemaDerivationMethod.List)
					{
						if (derivedBy == XmlSchemaDerivationMethod.Union)
						{
							XmlSchemaSimpleTypeUnion xmlSchemaSimpleTypeUnion = st.Content as XmlSchemaSimpleTypeUnion;
							string text2 = text;
							bool flag = false;
							object[] validatedTypes = xmlSchemaSimpleTypeUnion.ValidatedTypes;
							int i = 0;
							while (i < validatedTypes.Length)
							{
								object obj2 = validatedTypes[i];
								XmlSchemaDatatype xmlSchemaDatatype2 = obj2 as XmlSchemaDatatype;
								XmlSchemaSimpleType xmlSchemaSimpleType = obj2 as XmlSchemaSimpleType;
								if (xmlSchemaDatatype2 != null)
								{
									try
									{
										obj = xmlSchemaDatatype2.ParseValue(text2, this.nameTable, this.nsResolver);
									}
									catch (Exception)
									{
										goto IL_01A2;
									}
									goto IL_019A;
								}
								try
								{
									obj = this.AssessStringValid(xmlSchemaSimpleType, xmlSchemaSimpleType.Datatype, text2);
								}
								catch (XmlSchemaValidationException)
								{
									goto IL_01A2;
								}
								goto IL_019A;
								IL_01A2:
								i++;
								continue;
								IL_019A:
								flag = true;
								break;
							}
							if (!flag)
							{
								this.HandleError("Union type value contains one or more invalid values.");
							}
						}
					}
					else
					{
						XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = st.Content as XmlSchemaSimpleTypeList;
						string[] array = text.Split(XmlChar.WhitespaceChars);
						object[] array2 = new object[array.Length];
						XmlSchemaDatatype xmlSchemaDatatype2 = xmlSchemaSimpleTypeList.ValidatedListItemType as XmlSchemaDatatype;
						XmlSchemaSimpleType xmlSchemaSimpleType = xmlSchemaSimpleTypeList.ValidatedListItemType as XmlSchemaSimpleType;
						for (int j = 0; j < array.Length; j++)
						{
							string text3 = array[j];
							if (!(text3 == string.Empty))
							{
								if (xmlSchemaDatatype2 != null)
								{
									try
									{
										array2[j] = xmlSchemaDatatype2.ParseValue(text3, this.nameTable, this.nsResolver);
									}
									catch (Exception ex)
									{
										this.HandleError("List type value contains one or more invalid values.", ex);
										break;
									}
								}
								else
								{
									this.AssessStringValid(xmlSchemaSimpleType, xmlSchemaSimpleType.Datatype, text3);
								}
							}
						}
						obj = array2;
					}
				}
				else
				{
					XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = st.Content as XmlSchemaSimpleTypeRestriction;
					if (xmlSchemaSimpleTypeRestriction != null)
					{
						XmlSchemaSimpleType xmlSchemaSimpleType2 = st.BaseXmlSchemaType as XmlSchemaSimpleType;
						if (xmlSchemaSimpleType2 != null)
						{
							obj = this.AssessStringValid(xmlSchemaSimpleType2, dt, value);
						}
						if (!xmlSchemaSimpleTypeRestriction.ValidateValueWithFacets(value, this.nameTable, this.nsResolver))
						{
							this.HandleError("Specified value was invalid against the facets.");
							goto IL_0237;
						}
					}
					xmlSchemaDatatype = st.Datatype;
				}
			}
			IL_0237:
			if (xmlSchemaDatatype != null)
			{
				try
				{
					obj = xmlSchemaDatatype.ParseValue(value, this.nameTable, this.nsResolver);
				}
				catch (Exception ex2)
				{
					this.HandleError(string.Format("Invalidly typed data was specified.", new object[0]), ex2);
				}
			}
			return obj;
		}

		private void ValidateRestrictedSimpleTypeValue(XmlSchemaSimpleType st, ref XmlSchemaDatatype dt, string normalized)
		{
			XmlSchemaDerivationMethod derivedBy = st.DerivedBy;
			if (derivedBy != XmlSchemaDerivationMethod.Restriction)
			{
				if (derivedBy != XmlSchemaDerivationMethod.List)
				{
					if (derivedBy == XmlSchemaDerivationMethod.Union)
					{
						XmlSchemaSimpleTypeUnion xmlSchemaSimpleTypeUnion = st.Content as XmlSchemaSimpleTypeUnion;
						bool flag = false;
						object[] validatedTypes = xmlSchemaSimpleTypeUnion.ValidatedTypes;
						int i = 0;
						while (i < validatedTypes.Length)
						{
							object obj = validatedTypes[i];
							XmlSchemaDatatype xmlSchemaDatatype = obj as XmlSchemaDatatype;
							XmlSchemaSimpleType xmlSchemaSimpleType = obj as XmlSchemaSimpleType;
							if (xmlSchemaDatatype != null)
							{
								try
								{
									xmlSchemaDatatype.ParseValue(normalized, this.nameTable, this.nsResolver);
								}
								catch (Exception)
								{
									goto IL_0170;
								}
								goto IL_0168;
							}
							try
							{
								this.AssessStringValid(xmlSchemaSimpleType, xmlSchemaSimpleType.Datatype, normalized);
							}
							catch (XmlSchemaValidationException)
							{
								goto IL_0170;
							}
							goto IL_0168;
							IL_0170:
							i++;
							continue;
							IL_0168:
							flag = true;
							break;
						}
						if (!flag)
						{
							this.HandleError("Union type value contains one or more invalid values.");
						}
					}
				}
				else
				{
					XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = st.Content as XmlSchemaSimpleTypeList;
					string[] array = normalized.Split(XmlChar.WhitespaceChars);
					XmlSchemaDatatype xmlSchemaDatatype = xmlSchemaSimpleTypeList.ValidatedListItemType as XmlSchemaDatatype;
					XmlSchemaSimpleType xmlSchemaSimpleType = xmlSchemaSimpleTypeList.ValidatedListItemType as XmlSchemaSimpleType;
					foreach (string text in array)
					{
						if (!(text == string.Empty))
						{
							if (xmlSchemaDatatype != null)
							{
								try
								{
									xmlSchemaDatatype.ParseValue(text, this.nameTable, this.nsResolver);
								}
								catch (Exception ex)
								{
									this.HandleError("List type value contains one or more invalid values.", ex);
									break;
								}
							}
							else
							{
								this.AssessStringValid(xmlSchemaSimpleType, xmlSchemaSimpleType.Datatype, text);
							}
						}
					}
				}
			}
			else
			{
				XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = st.Content as XmlSchemaSimpleTypeRestriction;
				if (xmlSchemaSimpleTypeRestriction != null)
				{
					XmlSchemaSimpleType xmlSchemaSimpleType2 = st.BaseXmlSchemaType as XmlSchemaSimpleType;
					if (xmlSchemaSimpleType2 != null)
					{
						this.AssessStringValid(xmlSchemaSimpleType2, dt, normalized);
					}
					if (!xmlSchemaSimpleTypeRestriction.ValidateValueWithFacets(normalized, this.nameTable, this.nsResolver))
					{
						this.HandleError("Specified value was invalid against the facets.");
						return;
					}
				}
				dt = st.Datatype;
			}
		}

		private XsdKeyTable CreateNewKeyTable(XmlSchemaIdentityConstraint ident)
		{
			XsdKeyTable xsdKeyTable = new XsdKeyTable(ident);
			xsdKeyTable.StartDepth = this.depth;
			this.keyTables.Add(xsdKeyTable);
			return xsdKeyTable;
		}

		private void ValidateKeySelectors()
		{
			if (this.tmpKeyrefPool != null)
			{
				this.tmpKeyrefPool.Clear();
			}
			if (this.Context.Element != null && this.Context.Element.Constraints.Count > 0)
			{
				for (int i = 0; i < this.Context.Element.Constraints.Count; i++)
				{
					XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint = (XmlSchemaIdentityConstraint)this.Context.Element.Constraints[i];
					XsdKeyTable xsdKeyTable = this.CreateNewKeyTable(xmlSchemaIdentityConstraint);
					if (xmlSchemaIdentityConstraint is XmlSchemaKeyref)
					{
						if (this.tmpKeyrefPool == null)
						{
							this.tmpKeyrefPool = new ArrayList();
						}
						this.tmpKeyrefPool.Add(xsdKeyTable);
					}
				}
			}
			for (int j = 0; j < this.keyTables.Count; j++)
			{
				XsdKeyTable xsdKeyTable2 = (XsdKeyTable)this.keyTables[j];
				if (xsdKeyTable2.SelectorMatches(this.elementQNameStack, this.depth) != null)
				{
					XsdKeyEntry xsdKeyEntry = new XsdKeyEntry(xsdKeyTable2, this.depth, this.lineInfo);
					xsdKeyTable2.Entries.Add(xsdKeyEntry);
				}
			}
		}

		private void ValidateKeyFieldsAttribute(XmlSchemaAttribute attr, object value)
		{
			this.ValidateKeyFields(true, false, attr.AttributeType, attr.QualifiedName.Name, attr.QualifiedName.Namespace, value);
		}

		private void ValidateKeyFields(bool isAttr, bool isNil, object schemaType, string attrName, string attrNs, object value)
		{
			for (int i = 0; i < this.keyTables.Count; i++)
			{
				XsdKeyTable xsdKeyTable = (XsdKeyTable)this.keyTables[i];
				for (int j = 0; j < xsdKeyTable.Entries.Count; j++)
				{
					this.CurrentAttributeType = null;
					try
					{
						xsdKeyTable.Entries[j].ProcessMatch(isAttr, this.elementQNameStack, this.nominalEventSender, this.nameTable, this.BaseUri, schemaType, this.nsResolver, this.lineInfo, (!isAttr) ? this.depth : (this.depth + 1), attrName, attrNs, value, isNil, this.currentKeyFieldConsumers);
					}
					catch (XmlSchemaValidationException ex)
					{
						this.HandleError(ex);
					}
				}
			}
		}

		private void ValidateEndElementKeyConstraints()
		{
			for (int i = 0; i < this.keyTables.Count; i++)
			{
				XsdKeyTable xsdKeyTable = this.keyTables[i] as XsdKeyTable;
				if (xsdKeyTable.StartDepth == this.depth)
				{
					this.ValidateEndKeyConstraint(xsdKeyTable);
				}
				else
				{
					for (int j = 0; j < xsdKeyTable.Entries.Count; j++)
					{
						XsdKeyEntry xsdKeyEntry = xsdKeyTable.Entries[j];
						if (xsdKeyEntry.StartDepth == this.depth)
						{
							if (xsdKeyEntry.KeyFound)
							{
								xsdKeyTable.FinishedEntries.Add(xsdKeyEntry);
							}
							else if (xsdKeyTable.SourceSchemaIdentity is XmlSchemaKey)
							{
								this.HandleError("Key sequence is missing.");
							}
							xsdKeyTable.Entries.RemoveAt(j);
							j--;
						}
						else
						{
							for (int k = 0; k < xsdKeyEntry.KeyFields.Count; k++)
							{
								XsdKeyEntryField xsdKeyEntryField = xsdKeyEntry.KeyFields[k];
								if (!xsdKeyEntryField.FieldFound && xsdKeyEntryField.FieldFoundDepth == this.depth)
								{
									xsdKeyEntryField.FieldFoundDepth = 0;
									xsdKeyEntryField.FieldFoundPath = null;
								}
							}
						}
					}
				}
			}
			for (int l = 0; l < this.keyTables.Count; l++)
			{
				XsdKeyTable xsdKeyTable2 = this.keyTables[l] as XsdKeyTable;
				if (xsdKeyTable2.StartDepth == this.depth)
				{
					this.keyTables.RemoveAt(l);
					l--;
				}
			}
		}

		private void ValidateEndKeyConstraint(XsdKeyTable seq)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < seq.Entries.Count; i++)
			{
				XsdKeyEntry xsdKeyEntry = seq.Entries[i];
				if (!xsdKeyEntry.KeyFound)
				{
					if (seq.SourceSchemaIdentity is XmlSchemaKey)
					{
						arrayList.Add(string.Concat(new object[] { "line ", xsdKeyEntry.SelectorLineNumber, "position ", xsdKeyEntry.SelectorLinePosition }));
					}
				}
			}
			if (arrayList.Count > 0)
			{
				this.HandleError("Invalid identity constraints were found. Key was not found. " + string.Join(", ", arrayList.ToArray(typeof(string)) as string[]));
			}
			arrayList.Clear();
			XmlSchemaKeyref xmlSchemaKeyref = seq.SourceSchemaIdentity as XmlSchemaKeyref;
			if (xmlSchemaKeyref != null)
			{
				for (int j = this.keyTables.Count - 1; j >= 0; j--)
				{
					XsdKeyTable xsdKeyTable = this.keyTables[j] as XsdKeyTable;
					if (xsdKeyTable.SourceSchemaIdentity == xmlSchemaKeyref.Target)
					{
						seq.ReferencedKey = xsdKeyTable;
						for (int k = 0; k < seq.FinishedEntries.Count; k++)
						{
							XsdKeyEntry xsdKeyEntry2 = seq.FinishedEntries[k];
							for (int l = 0; l < xsdKeyTable.FinishedEntries.Count; l++)
							{
								XsdKeyEntry xsdKeyEntry3 = xsdKeyTable.FinishedEntries[l];
								if (xsdKeyEntry2.CompareIdentity(xsdKeyEntry3))
								{
									xsdKeyEntry2.KeyRefFound = true;
									break;
								}
							}
						}
					}
				}
				if (seq.ReferencedKey == null)
				{
					this.HandleError("Target key was not found.");
				}
				for (int m = 0; m < seq.FinishedEntries.Count; m++)
				{
					XsdKeyEntry xsdKeyEntry4 = seq.FinishedEntries[m];
					if (!xsdKeyEntry4.KeyRefFound)
					{
						arrayList.Add(string.Concat(new object[] { " line ", xsdKeyEntry4.SelectorLineNumber, ", position ", xsdKeyEntry4.SelectorLinePosition }));
					}
				}
				if (arrayList.Count > 0)
				{
					this.HandleError("Invalid identity constraints were found. Referenced key was not found: " + string.Join(" / ", arrayList.ToArray(typeof(string)) as string[]));
				}
			}
		}

		private void ValidateSimpleContentIdentity(XmlSchemaDatatype dt, string value)
		{
			if (this.currentKeyFieldConsumers != null)
			{
				while (this.currentKeyFieldConsumers.Count > 0)
				{
					XsdKeyEntryField xsdKeyEntryField = this.currentKeyFieldConsumers[0] as XsdKeyEntryField;
					if (xsdKeyEntryField.Identity != null)
					{
						this.HandleError("Two or more identical field was found. Former value is '" + xsdKeyEntryField.Identity + "' .");
					}
					object obj = null;
					if (dt != null)
					{
						try
						{
							obj = dt.ParseValue(value, this.nameTable, this.nsResolver);
						}
						catch (Exception ex)
						{
							this.HandleError("Identity value is invalid against its data type " + dt.TokenizedType, ex);
						}
					}
					if (obj == null)
					{
						obj = value;
					}
					if (!xsdKeyEntryField.SetIdentityField(obj, this.depth == this.xsiNilDepth, dt as XsdAnySimpleType, this.depth, this.lineInfo))
					{
						this.HandleError("Two or more identical key value was found: '" + value + "' .");
					}
					this.currentKeyFieldConsumers.RemoveAt(0);
				}
			}
		}

		private object GetXsiType(string name)
		{
			XmlQualifiedName xmlQualifiedName = XmlQualifiedName.Parse(name, this.nsResolver, true);
			object obj;
			if (xmlQualifiedName == XmlSchemaComplexType.AnyTypeName)
			{
				obj = XmlSchemaComplexType.AnyType;
			}
			else if (XmlSchemaUtil.IsBuiltInDatatypeName(xmlQualifiedName))
			{
				obj = XmlSchemaDatatype.FromName(xmlQualifiedName);
			}
			else
			{
				obj = this.FindType(xmlQualifiedName);
			}
			return obj;
		}

		private void HandleXsiType(string typename)
		{
			XmlSchemaElement element = this.Context.Element;
			object xsiType = this.GetXsiType(typename);
			if (xsiType == null)
			{
				this.HandleError("The instance type was not found: " + typename);
				return;
			}
			XmlSchemaType xmlSchemaType = xsiType as XmlSchemaType;
			if (xmlSchemaType != null && this.Context.Element != null)
			{
				XmlSchemaType xmlSchemaType2 = element.ElementType as XmlSchemaType;
				if (xmlSchemaType2 != null && (xmlSchemaType.DerivedBy & xmlSchemaType2.FinalResolved) != XmlSchemaDerivationMethod.Empty)
				{
					this.HandleError("The instance type is prohibited by the type of the context element.");
				}
				if (xmlSchemaType2 != xsiType && (xmlSchemaType.DerivedBy & element.BlockResolved) != XmlSchemaDerivationMethod.Empty)
				{
					this.HandleError("The instance type is prohibited by the context element.");
				}
			}
			XmlSchemaComplexType xmlSchemaComplexType = xsiType as XmlSchemaComplexType;
			if (xmlSchemaComplexType != null && xmlSchemaComplexType.IsAbstract)
			{
				this.HandleError("The instance type is abstract: " + typename);
			}
			else
			{
				if (element != null)
				{
					this.AssessLocalTypeDerivationOK(xsiType, element.ElementType, element.BlockResolved);
				}
				this.Context.XsiType = xsiType;
			}
		}

		private void AssessLocalTypeDerivationOK(object xsiType, object baseType, XmlSchemaDerivationMethod flag)
		{
			XmlSchemaType xmlSchemaType = xsiType as XmlSchemaType;
			XmlSchemaComplexType xmlSchemaComplexType = baseType as XmlSchemaComplexType;
			XmlSchemaComplexType xmlSchemaComplexType2 = xmlSchemaType as XmlSchemaComplexType;
			if (xsiType != baseType)
			{
				if (xmlSchemaComplexType != null)
				{
					flag |= xmlSchemaComplexType.BlockResolved;
				}
				if (flag == XmlSchemaDerivationMethod.All)
				{
					this.HandleError("Prohibited element type substitution.");
					return;
				}
				if (xmlSchemaType != null && (flag & xmlSchemaType.DerivedBy) != XmlSchemaDerivationMethod.Empty)
				{
					this.HandleError("Prohibited element type substitution.");
					return;
				}
			}
			if (xmlSchemaComplexType2 != null)
			{
				try
				{
					xmlSchemaComplexType2.ValidateTypeDerivationOK(baseType, null, null);
				}
				catch (XmlSchemaValidationException ex)
				{
					this.HandleError(ex);
				}
			}
			else
			{
				XmlSchemaSimpleType xmlSchemaSimpleType = xsiType as XmlSchemaSimpleType;
				if (xmlSchemaSimpleType != null)
				{
					try
					{
						xmlSchemaSimpleType.ValidateTypeDerivationOK(baseType, null, null, true);
					}
					catch (XmlSchemaValidationException ex2)
					{
						this.HandleError(ex2);
					}
				}
				else if (!(xsiType is XmlSchemaDatatype))
				{
					this.HandleError("Primitive data type cannot be derived type using xsi:type specification.");
				}
			}
		}

		private void HandleXsiNil(string value, XmlSchemaInfo info)
		{
			XmlSchemaElement element = this.Context.Element;
			if (!element.ActualIsNillable)
			{
				this.HandleError(string.Format("Current element '{0}' is not nillable and thus does not allow occurence of 'nil' attribute.", this.Context.Element.QualifiedName));
				return;
			}
			value = value.Trim(XmlChar.WhitespaceChars);
			if (value == "true")
			{
				if (element.ValidatedFixedValue != null)
				{
					this.HandleError("Schema instance nil was specified, where the element declaration for " + element.QualifiedName + "has fixed value constraints.");
				}
				this.xsiNilDepth = this.depth;
				if (info != null)
				{
					info.IsNil = true;
				}
			}
		}

		private XmlSchema ReadExternalSchema(string uri)
		{
			Uri uri2 = new Uri(this.SourceUri, uri.Trim(XmlChar.WhitespaceChars));
			XmlTextReader xmlTextReader = null;
			XmlSchema xmlSchema;
			try
			{
				xmlTextReader = new XmlTextReader(uri2.ToString(), (Stream)this.xmlResolver.GetEntity(uri2, null, typeof(Stream)), this.nameTable);
				xmlSchema = XmlSchema.Read(xmlTextReader, this.ValidationEventHandler);
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
			return xmlSchema;
		}

		private void HandleSchemaLocation(string schemaLocation)
		{
			if (this.xmlResolver == null)
			{
				return;
			}
			XmlSchema xmlSchema = null;
			bool flag = false;
			string[] array = null;
			try
			{
				schemaLocation = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype.ParseValue(schemaLocation, null, null) as string;
				array = schemaLocation.Split(XmlChar.WhitespaceChars);
			}
			catch (Exception ex)
			{
				this.HandleError("Invalid schemaLocation attribute format.", ex, true);
				array = new string[0];
			}
			if (array.Length % 2 != 0)
			{
				this.HandleError("Invalid schemaLocation attribute format.");
			}
			int i = 0;
			while (i < array.Length)
			{
				try
				{
					xmlSchema = this.ReadExternalSchema(array[i + 1]);
				}
				catch (Exception ex2)
				{
					this.HandleError("Could not resolve schema location URI: " + schemaLocation, ex2, true);
					goto IL_010B;
				}
				goto IL_00A7;
				IL_010B:
				i += 2;
				continue;
				IL_00A7:
				if (xmlSchema.TargetNamespace == null)
				{
					xmlSchema.TargetNamespace = array[i];
				}
				else if (xmlSchema.TargetNamespace != array[i])
				{
					this.HandleError("Specified schema has different target namespace.");
				}
				if (xmlSchema != null && !this.schemas.Contains(xmlSchema.TargetNamespace))
				{
					flag = true;
					this.schemas.Add(xmlSchema);
					goto IL_010B;
				}
				goto IL_010B;
			}
			if (flag)
			{
				this.schemas.Compile();
			}
		}

		private void HandleNoNSSchemaLocation(string noNsSchemaLocation)
		{
			if (this.xmlResolver == null)
			{
				return;
			}
			XmlSchema xmlSchema = null;
			bool flag = false;
			try
			{
				xmlSchema = this.ReadExternalSchema(noNsSchemaLocation);
			}
			catch (Exception ex)
			{
				this.HandleError("Could not resolve schema location URI: " + noNsSchemaLocation, ex, true);
			}
			if (xmlSchema != null && xmlSchema.TargetNamespace != null)
			{
				this.HandleError("Specified schema has different target namespace.");
			}
			if (xmlSchema != null && !this.schemas.Contains(xmlSchema.TargetNamespace))
			{
				flag = true;
				this.schemas.Add(xmlSchema);
			}
			if (flag)
			{
				this.schemas.Compile();
			}
		}

		private static readonly XmlSchemaAttribute[] emptyAttributeArray = new XmlSchemaAttribute[0];

		private object nominalEventSender;

		private IXmlLineInfo lineInfo;

		private IXmlNamespaceResolver nsResolver;

		private Uri sourceUri;

		private XmlNameTable nameTable;

		private XmlSchemaSet schemas;

		private XmlResolver xmlResolver = new XmlUrlResolver();

		private XmlSchemaObject startType;

		private XmlSchemaValidationFlags options;

		private XmlSchemaValidator.Transition transition;

		private XsdParticleStateManager state;

		private ArrayList occuredAtts = new ArrayList();

		private XmlSchemaAttribute[] defaultAttributes = XmlSchemaValidator.emptyAttributeArray;

		private ArrayList defaultAttributesCache = new ArrayList();

		private XsdIDManager idManager = new XsdIDManager();

		private ArrayList keyTables = new ArrayList();

		private ArrayList currentKeyFieldConsumers = new ArrayList();

		private ArrayList tmpKeyrefPool;

		private ArrayList elementQNameStack = new ArrayList();

		private StringBuilder storedCharacters = new StringBuilder();

		private bool shouldValidateCharacters;

		private int depth;

		private int xsiNilDepth = -1;

		private int skipValidationDepth = -1;

		internal XmlSchemaDatatype CurrentAttributeType;

		private enum Transition
		{
			None,
			Content,
			StartTag,
			Finished
		}
	}
}
