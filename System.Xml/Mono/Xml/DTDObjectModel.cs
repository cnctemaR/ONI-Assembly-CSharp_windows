using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Mono.Xml2;

namespace Mono.Xml
{
	internal class DTDObjectModel
	{
		public DTDObjectModel(XmlNameTable nameTable)
		{
			this.nameTable = nameTable;
			this.elementDecls = new DTDElementDeclarationCollection(this);
			this.attListDecls = new DTDAttListDeclarationCollection(this);
			this.entityDecls = new DTDEntityDeclarationCollection(this);
			this.peDecls = new DTDParameterEntityDeclarationCollection(this);
			this.notationDecls = new DTDNotationDeclarationCollection(this);
			this.factory = new DTDAutomataFactory(this);
			this.validationErrors = new ArrayList();
			this.externalResources = new Hashtable();
		}

		public string BaseURI
		{
			get
			{
				return this.baseURI;
			}
			set
			{
				this.baseURI = value;
			}
		}

		public bool IsStandalone
		{
			get
			{
				return this.isStandalone;
			}
			set
			{
				this.isStandalone = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		public string PublicId
		{
			get
			{
				return this.publicId;
			}
			set
			{
				this.publicId = value;
			}
		}

		public string SystemId
		{
			get
			{
				return this.systemId;
			}
			set
			{
				this.systemId = value;
			}
		}

		public string InternalSubset
		{
			get
			{
				return this.intSubset;
			}
			set
			{
				this.intSubset = value;
			}
		}

		public bool InternalSubsetHasPEReference
		{
			get
			{
				return this.intSubsetHasPERef;
			}
			set
			{
				this.intSubsetHasPERef = value;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
			set
			{
				this.lineNumber = value;
			}
		}

		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
			set
			{
				this.linePosition = value;
			}
		}

		internal XmlSchema CreateXsdSchema()
		{
			XmlSchema xmlSchema = new XmlSchema();
			xmlSchema.SourceUri = this.BaseURI;
			xmlSchema.LineNumber = this.LineNumber;
			xmlSchema.LinePosition = this.LinePosition;
			foreach (DTDNode dtdnode in this.ElementDecls.Values)
			{
				DTDElementDeclaration dtdelementDeclaration = (DTDElementDeclaration)dtdnode;
				xmlSchema.Items.Add(dtdelementDeclaration.CreateXsdElement());
			}
			return xmlSchema;
		}

		public string ResolveEntity(string name)
		{
			DTDEntityDeclaration dtdentityDeclaration = this.EntityDecls[name];
			if (dtdentityDeclaration == null)
			{
				this.AddError(new XmlSchemaException("Required entity was not found.", this.LineNumber, this.LinePosition, null, this.BaseURI, null));
				return " ";
			}
			return dtdentityDeclaration.EntityValue;
		}

		internal XmlResolver Resolver
		{
			get
			{
				return this.resolver;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				this.resolver = value;
			}
		}

		internal Hashtable ExternalResources
		{
			get
			{
				return this.externalResources;
			}
		}

		public DTDAutomataFactory Factory
		{
			get
			{
				return this.factory;
			}
		}

		public DTDElementDeclaration RootElement
		{
			get
			{
				return this.ElementDecls[this.Name];
			}
		}

		public DTDElementDeclarationCollection ElementDecls
		{
			get
			{
				return this.elementDecls;
			}
		}

		public DTDAttListDeclarationCollection AttListDecls
		{
			get
			{
				return this.attListDecls;
			}
		}

		public DTDEntityDeclarationCollection EntityDecls
		{
			get
			{
				return this.entityDecls;
			}
		}

		public DTDParameterEntityDeclarationCollection PEDecls
		{
			get
			{
				return this.peDecls;
			}
		}

		public DTDNotationDeclarationCollection NotationDecls
		{
			get
			{
				return this.notationDecls;
			}
		}

		public DTDAutomata RootAutomata
		{
			get
			{
				if (this.rootAutomata == null)
				{
					this.rootAutomata = new DTDElementAutomata(this, this.Name);
				}
				return this.rootAutomata;
			}
		}

		public DTDEmptyAutomata Empty
		{
			get
			{
				if (this.emptyAutomata == null)
				{
					this.emptyAutomata = new DTDEmptyAutomata(this);
				}
				return this.emptyAutomata;
			}
		}

		public DTDAnyAutomata Any
		{
			get
			{
				if (this.anyAutomata == null)
				{
					this.anyAutomata = new DTDAnyAutomata(this);
				}
				return this.anyAutomata;
			}
		}

		public DTDInvalidAutomata Invalid
		{
			get
			{
				if (this.invalidAutomata == null)
				{
					this.invalidAutomata = new DTDInvalidAutomata(this);
				}
				return this.invalidAutomata;
			}
		}

		public XmlSchemaException[] Errors
		{
			get
			{
				return this.validationErrors.ToArray(typeof(XmlSchemaException)) as XmlSchemaException[];
			}
		}

		public void AddError(XmlSchemaException ex)
		{
			this.validationErrors.Add(ex);
		}

		internal string GenerateEntityAttributeText(string entityName)
		{
			DTDEntityDeclaration dtdentityDeclaration = this.EntityDecls[entityName];
			if (dtdentityDeclaration == null)
			{
				return null;
			}
			return dtdentityDeclaration.EntityValue;
		}

		internal Mono.Xml2.XmlTextReader GenerateEntityContentReader(string entityName, XmlParserContext context)
		{
			DTDEntityDeclaration dtdentityDeclaration = this.EntityDecls[entityName];
			if (dtdentityDeclaration == null)
			{
				return null;
			}
			if (dtdentityDeclaration.SystemId != null)
			{
				Uri uri = ((!(dtdentityDeclaration.BaseURI == string.Empty)) ? new Uri(dtdentityDeclaration.BaseURI) : null);
				Stream stream = this.resolver.GetEntity(this.resolver.ResolveUri(uri, dtdentityDeclaration.SystemId), null, typeof(Stream)) as Stream;
				return new Mono.Xml2.XmlTextReader(stream, XmlNodeType.Element, context);
			}
			return new Mono.Xml2.XmlTextReader(dtdentityDeclaration.EntityValue, XmlNodeType.Element, context);
		}

		public const int AllowedExternalEntitiesMax = 256;

		private DTDAutomataFactory factory;

		private DTDElementAutomata rootAutomata;

		private DTDEmptyAutomata emptyAutomata;

		private DTDAnyAutomata anyAutomata;

		private DTDInvalidAutomata invalidAutomata;

		private DTDElementDeclarationCollection elementDecls;

		private DTDAttListDeclarationCollection attListDecls;

		private DTDParameterEntityDeclarationCollection peDecls;

		private DTDEntityDeclarationCollection entityDecls;

		private DTDNotationDeclarationCollection notationDecls;

		private ArrayList validationErrors;

		private XmlResolver resolver;

		private XmlNameTable nameTable;

		private Hashtable externalResources;

		private string baseURI;

		private string name;

		private string publicId;

		private string systemId;

		private string intSubset;

		private bool intSubsetHasPERef;

		private bool isStandalone;

		private int lineNumber;

		private int linePosition;
	}
}
