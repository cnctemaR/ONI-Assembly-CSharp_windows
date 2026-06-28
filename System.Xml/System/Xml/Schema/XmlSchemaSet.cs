using System;
using System.Collections;

namespace System.Xml.Schema
{
	public class XmlSchemaSet
	{
		public XmlSchemaSet()
			: this(new NameTable())
		{
		}

		public XmlSchemaSet(XmlNameTable nameTable)
		{
			if (nameTable == null)
			{
				throw new ArgumentNullException("nameTable");
			}
			this.nameTable = nameTable;
			this.schemas = new ArrayList();
			this.CompilationId = Guid.NewGuid();
		}

		public event ValidationEventHandler ValidationEventHandler;

		public int Count
		{
			get
			{
				return this.schemas.Count;
			}
		}

		public XmlSchemaObjectTable GlobalAttributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new XmlSchemaObjectTable();
				}
				return this.attributes;
			}
		}

		public XmlSchemaObjectTable GlobalElements
		{
			get
			{
				if (this.elements == null)
				{
					this.elements = new XmlSchemaObjectTable();
				}
				return this.elements;
			}
		}

		public XmlSchemaObjectTable GlobalTypes
		{
			get
			{
				if (this.types == null)
				{
					this.types = new XmlSchemaObjectTable();
				}
				return this.types;
			}
		}

		public bool IsCompiled
		{
			get
			{
				return this.isCompiled;
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		public XmlSchemaCompilationSettings CompilationSettings
		{
			get
			{
				return this.settings;
			}
			set
			{
				this.settings = value;
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

		internal Hashtable IDCollection
		{
			get
			{
				if (this.idCollection == null)
				{
					this.idCollection = new Hashtable();
				}
				return this.idCollection;
			}
		}

		internal XmlSchemaObjectTable NamedIdentities
		{
			get
			{
				if (this.namedIdentities == null)
				{
					this.namedIdentities = new XmlSchemaObjectTable();
				}
				return this.namedIdentities;
			}
		}

		public XmlSchema Add(string targetNamespace, string url)
		{
			XmlTextReader xmlTextReader = null;
			XmlSchema xmlSchema;
			try
			{
				xmlTextReader = new XmlTextReader(url, this.nameTable);
				xmlSchema = this.Add(targetNamespace, xmlTextReader);
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

		public XmlSchema Add(string targetNamespace, XmlReader reader)
		{
			XmlSchema xmlSchema = XmlSchema.Read(reader, this.ValidationEventHandler);
			if (xmlSchema.TargetNamespace == null)
			{
				xmlSchema.TargetNamespace = targetNamespace;
			}
			else if (targetNamespace != null && xmlSchema.TargetNamespace != targetNamespace)
			{
				throw new XmlSchemaException("The actual targetNamespace in the schema does not match the parameter.");
			}
			this.Add(xmlSchema);
			return xmlSchema;
		}

		[MonoTODO]
		public void Add(XmlSchemaSet schemaSet)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object obj in schemaSet.schemas)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (!this.schemas.Contains(xmlSchema))
				{
					arrayList.Add(xmlSchema);
				}
			}
			foreach (object obj2 in arrayList)
			{
				XmlSchema xmlSchema2 = (XmlSchema)obj2;
				this.Add(xmlSchema2);
			}
		}

		public XmlSchema Add(XmlSchema schema)
		{
			this.schemas.Add(schema);
			this.ResetCompile();
			return schema;
		}

		public void Compile()
		{
			this.ClearGlobalComponents();
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(this.schemas);
			this.IDCollection.Clear();
			this.NamedIdentities.Clear();
			Hashtable hashtable = new Hashtable();
			foreach (object obj in arrayList)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (!xmlSchema.IsCompiled)
				{
					xmlSchema.CompileSubset(this.ValidationEventHandler, this, this.xmlResolver, hashtable);
				}
			}
			foreach (object obj2 in arrayList)
			{
				XmlSchema xmlSchema2 = (XmlSchema)obj2;
				foreach (object obj3 in xmlSchema2.Elements.Values)
				{
					XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)obj3;
					xmlSchemaElement.FillSubstitutionElementInfo();
				}
			}
			foreach (object obj4 in arrayList)
			{
				XmlSchema xmlSchema3 = (XmlSchema)obj4;
				xmlSchema3.Validate(this.ValidationEventHandler);
			}
			foreach (object obj5 in arrayList)
			{
				XmlSchema xmlSchema4 = (XmlSchema)obj5;
				this.AddGlobalComponents(xmlSchema4);
			}
			this.isCompiled = true;
		}

		private void ClearGlobalComponents()
		{
			this.GlobalElements.Clear();
			this.GlobalAttributes.Clear();
			this.GlobalTypes.Clear();
		}

		private void AddGlobalComponents(XmlSchema schema)
		{
			foreach (object obj in schema.Elements.Values)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)obj;
				this.GlobalElements.Add(xmlSchemaElement.QualifiedName, xmlSchemaElement);
			}
			foreach (object obj2 in schema.Attributes.Values)
			{
				XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)obj2;
				this.GlobalAttributes.Add(xmlSchemaAttribute.QualifiedName, xmlSchemaAttribute);
			}
			foreach (object obj3 in schema.SchemaTypes.Values)
			{
				XmlSchemaType xmlSchemaType = (XmlSchemaType)obj3;
				this.GlobalTypes.Add(xmlSchemaType.QualifiedName, xmlSchemaType);
			}
		}

		public bool Contains(string targetNamespace)
		{
			targetNamespace = this.GetSafeNs(targetNamespace);
			foreach (object obj in this.schemas)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (this.GetSafeNs(xmlSchema.TargetNamespace) == targetNamespace)
				{
					return true;
				}
			}
			return false;
		}

		public bool Contains(XmlSchema targetNamespace)
		{
			foreach (object obj in this.schemas)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (xmlSchema == targetNamespace)
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(XmlSchema[] array, int index)
		{
			this.schemas.CopyTo(array, index);
		}

		internal void CopyTo(Array array, int index)
		{
			this.schemas.CopyTo(array, index);
		}

		private string GetSafeNs(string ns)
		{
			return (ns != null) ? ns : string.Empty;
		}

		[MonoTODO]
		public XmlSchema Remove(XmlSchema schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(this.schemas);
			if (!arrayList.Contains(schema))
			{
				return null;
			}
			if (!schema.IsCompiled)
			{
				schema.CompileSubset(this.ValidationEventHandler, this, this.xmlResolver);
			}
			this.schemas.Remove(schema);
			this.ResetCompile();
			return schema;
		}

		private void ResetCompile()
		{
			this.isCompiled = false;
			this.ClearGlobalComponents();
		}

		public bool RemoveRecursive(XmlSchema schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(this.schemas);
			if (!arrayList.Contains(schema))
			{
				return false;
			}
			arrayList.Remove(schema);
			this.schemas.Remove(schema);
			if (!this.IsCompiled)
			{
				return true;
			}
			this.ClearGlobalComponents();
			foreach (object obj in arrayList)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (xmlSchema.IsCompiled)
				{
					this.AddGlobalComponents(schema);
				}
			}
			return true;
		}

		public XmlSchema Reprocess(XmlSchema schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(this.schemas);
			if (!arrayList.Contains(schema))
			{
				throw new ArgumentException("Target schema is not contained in the schema set.");
			}
			this.ClearGlobalComponents();
			foreach (object obj in arrayList)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (schema == xmlSchema)
				{
					schema.CompileSubset(this.ValidationEventHandler, this, this.xmlResolver);
				}
				if (xmlSchema.IsCompiled)
				{
					this.AddGlobalComponents(schema);
				}
			}
			return (!schema.IsCompiled) ? null : schema;
		}

		public ICollection Schemas()
		{
			return this.schemas;
		}

		public ICollection Schemas(string targetNamespace)
		{
			targetNamespace = this.GetSafeNs(targetNamespace);
			ArrayList arrayList = new ArrayList();
			foreach (object obj in this.schemas)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (this.GetSafeNs(xmlSchema.TargetNamespace) == targetNamespace)
				{
					arrayList.Add(xmlSchema);
				}
			}
			return arrayList;
		}

		internal bool MissedSubComponents(string targetNamespace)
		{
			foreach (object obj in this.Schemas(targetNamespace))
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (xmlSchema.missedSubComponents)
				{
					return true;
				}
			}
			return false;
		}

		private XmlNameTable nameTable;

		private XmlResolver xmlResolver = new XmlUrlResolver();

		private ArrayList schemas;

		private XmlSchemaObjectTable attributes;

		private XmlSchemaObjectTable elements;

		private XmlSchemaObjectTable types;

		private Hashtable idCollection;

		private XmlSchemaObjectTable namedIdentities;

		private XmlSchemaCompilationSettings settings = new XmlSchemaCompilationSettings();

		private bool isCompiled;

		internal Guid CompilationId;
	}
}
