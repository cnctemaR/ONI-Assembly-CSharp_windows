using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	public class XmlSchemaAny : XmlSchemaParticle
	{
		public XmlSchemaAny()
		{
			this.wildcard = new XsdWildcard(this);
		}

		internal static XmlSchemaAny AnyTypeContent
		{
			get
			{
				if (XmlSchemaAny.anyTypeContent == null)
				{
					XmlSchemaAny.anyTypeContent = new XmlSchemaAny();
					XmlSchemaAny.anyTypeContent.MaxOccursString = "unbounded";
					XmlSchemaAny.anyTypeContent.MinOccurs = 0m;
					XmlSchemaAny.anyTypeContent.CompileOccurence(null, null);
					XmlSchemaAny.anyTypeContent.Namespace = "##any";
					XmlSchemaAny.anyTypeContent.wildcard.HasValueAny = true;
					XmlSchemaAny.anyTypeContent.wildcard.ResolvedNamespaces = new StringCollection();
					XsdWildcard xsdWildcard = XmlSchemaAny.anyTypeContent.wildcard;
					XmlSchemaContentProcessing xmlSchemaContentProcessing = XmlSchemaContentProcessing.Lax;
					XmlSchemaAny.anyTypeContent.ProcessContents = xmlSchemaContentProcessing;
					xsdWildcard.ResolvedProcessing = xmlSchemaContentProcessing;
					XmlSchemaAny.anyTypeContent.wildcard.SkipCompile = true;
				}
				return XmlSchemaAny.anyTypeContent;
			}
		}

		[XmlAttribute("namespace")]
		public string Namespace
		{
			get
			{
				return this.nameSpace;
			}
			set
			{
				this.nameSpace = value;
			}
		}

		[XmlAttribute("processContents")]
		[DefaultValue(XmlSchemaContentProcessing.None)]
		public XmlSchemaContentProcessing ProcessContents
		{
			get
			{
				return this.processing;
			}
			set
			{
				this.processing = value;
			}
		}

		internal bool HasValueAny
		{
			get
			{
				return this.wildcard.HasValueAny;
			}
		}

		internal bool HasValueLocal
		{
			get
			{
				return this.wildcard.HasValueLocal;
			}
		}

		internal bool HasValueOther
		{
			get
			{
				return this.wildcard.HasValueOther;
			}
		}

		internal bool HasValueTargetNamespace
		{
			get
			{
				return this.wildcard.HasValueTargetNamespace;
			}
		}

		internal StringCollection ResolvedNamespaces
		{
			get
			{
				return this.wildcard.ResolvedNamespaces;
			}
		}

		internal XmlSchemaContentProcessing ResolvedProcessContents
		{
			get
			{
				return this.wildcard.ResolvedProcessing;
			}
		}

		internal string TargetNamespace
		{
			get
			{
				return this.wildcard.TargetNamespace;
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (this.CompilationId == schema.CompilationId)
			{
				return 0;
			}
			this.errorCount = 0;
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			this.wildcard.TargetNamespace = base.AncestorSchema.TargetNamespace;
			if (this.wildcard.TargetNamespace == null)
			{
				this.wildcard.TargetNamespace = string.Empty;
			}
			base.CompileOccurence(h, schema);
			this.wildcard.Compile(this.Namespace, h, schema);
			if (this.processing == XmlSchemaContentProcessing.None)
			{
				this.wildcard.ResolvedProcessing = XmlSchemaContentProcessing.Strict;
			}
			else
			{
				this.wildcard.ResolvedProcessing = this.processing;
			}
			this.CompilationId = schema.CompilationId;
			return this.errorCount;
		}

		internal override XmlSchemaParticle GetOptimizedParticle(bool isTop)
		{
			if (this.OptimizedParticle != null)
			{
				return this.OptimizedParticle;
			}
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			this.CopyInfo(xmlSchemaAny);
			xmlSchemaAny.CompileOccurence(null, null);
			xmlSchemaAny.wildcard = this.wildcard;
			this.OptimizedParticle = xmlSchemaAny;
			xmlSchemaAny.Namespace = this.Namespace;
			xmlSchemaAny.ProcessContents = this.ProcessContents;
			xmlSchemaAny.Annotation = base.Annotation;
			xmlSchemaAny.UnhandledAttributes = base.UnhandledAttributes;
			return this.OptimizedParticle;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			return this.errorCount;
		}

		internal override bool ParticleEquals(XmlSchemaParticle other)
		{
			XmlSchemaAny xmlSchemaAny = other as XmlSchemaAny;
			if (xmlSchemaAny == null)
			{
				return false;
			}
			if (this.HasValueAny != xmlSchemaAny.HasValueAny || this.HasValueLocal != xmlSchemaAny.HasValueLocal || this.HasValueOther != xmlSchemaAny.HasValueOther || this.HasValueTargetNamespace != xmlSchemaAny.HasValueTargetNamespace || this.ResolvedProcessContents != xmlSchemaAny.ResolvedProcessContents || base.ValidatedMaxOccurs != xmlSchemaAny.ValidatedMaxOccurs || base.ValidatedMinOccurs != xmlSchemaAny.ValidatedMinOccurs || this.ResolvedNamespaces.Count != xmlSchemaAny.ResolvedNamespaces.Count)
			{
				return false;
			}
			for (int i = 0; i < this.ResolvedNamespaces.Count; i++)
			{
				if (this.ResolvedNamespaces[i] != xmlSchemaAny.ResolvedNamespaces[i])
				{
					return false;
				}
			}
			return true;
		}

		internal bool ExamineAttributeWildcardIntersection(XmlSchemaAny other, ValidationEventHandler h, XmlSchema schema)
		{
			return this.wildcard.ExamineAttributeWildcardIntersection(other, h, schema);
		}

		internal override bool ValidateDerivationByRestriction(XmlSchemaParticle baseParticle, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			XmlSchemaAny xmlSchemaAny = baseParticle as XmlSchemaAny;
			if (xmlSchemaAny == null)
			{
				if (raiseError)
				{
					base.error(h, "Invalid particle derivation by restriction was found.");
				}
				return false;
			}
			return this.ValidateOccurenceRangeOK(baseParticle, h, schema, raiseError) && this.wildcard.ValidateWildcardSubset(xmlSchemaAny.wildcard, h, schema, raiseError);
		}

		internal override void CheckRecursion(int depth, ValidationEventHandler h, XmlSchema schema)
		{
		}

		internal override void ValidateUniqueParticleAttribution(XmlSchemaObjectTable qnames, ArrayList nsNames, ValidationEventHandler h, XmlSchema schema)
		{
			foreach (object obj in nsNames)
			{
				XmlSchemaAny xmlSchemaAny = (XmlSchemaAny)obj;
				if (!this.ExamineAttributeWildcardIntersection(xmlSchemaAny, h, schema))
				{
					base.error(h, "Ambiguous -any- particle was found.");
				}
			}
			nsNames.Add(this);
		}

		internal override void ValidateUniqueTypeAttribution(XmlSchemaObjectTable labels, ValidationEventHandler h, XmlSchema schema)
		{
		}

		internal bool ValidateWildcardAllowsNamespaceName(string ns, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			return this.wildcard.ValidateWildcardAllowsNamespaceName(ns, h, schema, raiseError);
		}

		internal static XmlSchemaAny Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "any")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaAny.Read, name=" + reader.Name, null);
				reader.SkipToEnd();
				return null;
			}
			xmlSchemaAny.LineNumber = reader.LineNumber;
			xmlSchemaAny.LinePosition = reader.LinePosition;
			xmlSchemaAny.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaAny.Id = reader.Value;
				}
				else if (reader.Name == "maxOccurs")
				{
					try
					{
						xmlSchemaAny.MaxOccursString = reader.Value;
					}
					catch (Exception ex)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for maxOccurs", ex);
					}
				}
				else if (reader.Name == "minOccurs")
				{
					try
					{
						xmlSchemaAny.MinOccursString = reader.Value;
					}
					catch (Exception ex2)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for minOccurs", ex2);
					}
				}
				else if (reader.Name == "namespace")
				{
					xmlSchemaAny.nameSpace = reader.Value;
				}
				else if (reader.Name == "processContents")
				{
					Exception ex3;
					xmlSchemaAny.processing = XmlSchemaUtil.ReadProcessingAttribute(reader, out ex3);
					if (ex3 != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for processContents", ex3);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for any", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaAny);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaAny;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "any")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaAny.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaAny.Annotation = xmlSchemaAnnotation;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaAny;
		}

		private const string xmlname = "any";

		private static XmlSchemaAny anyTypeContent;

		private string nameSpace;

		private XmlSchemaContentProcessing processing;

		private XsdWildcard wildcard;
	}
}
