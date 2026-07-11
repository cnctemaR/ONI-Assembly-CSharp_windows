using System;
using System.Collections;

namespace System.Xml.Serialization
{
	public class XmlTypeMapping : XmlMapping
	{
		internal XmlTypeMapping(string elementName, string ns, TypeData typeData, string xmlType, string xmlTypeNamespace)
			: base(elementName, ns)
		{
			this.type = typeData;
			this.xmlType = xmlType;
			this.xmlTypeNamespace = xmlTypeNamespace;
		}

		public string TypeFullName
		{
			get
			{
				return this.type.FullTypeName;
			}
		}

		public string TypeName
		{
			get
			{
				return this.type.TypeName;
			}
		}

		public string XsdTypeName
		{
			get
			{
				return this.XmlType;
			}
		}

		public string XsdTypeNamespace
		{
			get
			{
				return this.XmlTypeNamespace;
			}
		}

		internal TypeData TypeData
		{
			get
			{
				return this.type;
			}
		}

		internal string XmlType
		{
			get
			{
				return this.xmlType;
			}
			set
			{
				this.xmlType = value;
			}
		}

		internal string XmlTypeNamespace
		{
			get
			{
				return this.xmlTypeNamespace;
			}
			set
			{
				this.xmlTypeNamespace = value;
			}
		}

		internal ArrayList DerivedTypes
		{
			get
			{
				return this._derivedTypes;
			}
			set
			{
				this._derivedTypes = value;
			}
		}

		internal bool MultiReferenceType
		{
			get
			{
				return this.multiReferenceType;
			}
			set
			{
				this.multiReferenceType = value;
			}
		}

		internal XmlTypeMapping BaseMap
		{
			get
			{
				return this.baseMap;
			}
			set
			{
				this.baseMap = value;
			}
		}

		internal bool IsSimpleType
		{
			get
			{
				return this.isSimpleType;
			}
			set
			{
				this.isSimpleType = value;
			}
		}

		internal string Documentation
		{
			get
			{
				return this.documentation;
			}
			set
			{
				this.documentation = value;
			}
		}

		internal bool IncludeInSchema
		{
			get
			{
				return this.includeInSchema;
			}
			set
			{
				this.includeInSchema = value;
			}
		}

		internal bool IsNullable
		{
			get
			{
				return this.isNullable;
			}
			set
			{
				this.isNullable = value;
			}
		}

		internal XmlTypeMapping GetRealTypeMap(Type objectType)
		{
			if (this.TypeData.SchemaType == SchemaTypes.Enum)
			{
				return this;
			}
			if (this.TypeData.Type == objectType)
			{
				return this;
			}
			for (int i = 0; i < this._derivedTypes.Count; i++)
			{
				XmlTypeMapping xmlTypeMapping = (XmlTypeMapping)this._derivedTypes[i];
				if (xmlTypeMapping.TypeData.Type == objectType)
				{
					return xmlTypeMapping;
				}
			}
			return null;
		}

		internal XmlTypeMapping GetRealElementMap(string name, string ens)
		{
			if (this.xmlType == name && this.xmlTypeNamespace == ens)
			{
				return this;
			}
			foreach (object obj in this._derivedTypes)
			{
				XmlTypeMapping xmlTypeMapping = (XmlTypeMapping)obj;
				if (xmlTypeMapping.xmlType == name && xmlTypeMapping.xmlTypeNamespace == ens)
				{
					return xmlTypeMapping;
				}
			}
			return null;
		}

		internal void UpdateRoot(XmlQualifiedName qname)
		{
			if (qname != null)
			{
				this._elementName = qname.Name;
				this._namespace = qname.Namespace;
			}
		}

		private string xmlType;

		private string xmlTypeNamespace;

		private TypeData type;

		private XmlTypeMapping baseMap;

		private bool multiReferenceType;

		private bool isSimpleType;

		private string documentation;

		private bool includeInSchema;

		private bool isNullable = true;

		private ArrayList _derivedTypes = new ArrayList();
	}
}
