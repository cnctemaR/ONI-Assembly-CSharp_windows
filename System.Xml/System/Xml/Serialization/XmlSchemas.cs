using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	public class XmlSchemas : CollectionBase, IEnumerable<XmlSchema>, IEnumerable
	{
		IEnumerator<XmlSchema> IEnumerable<XmlSchema>.GetEnumerator()
		{
			return new XmlSchemaEnumerator(this);
		}

		public XmlSchema this[int index]
		{
			get
			{
				if (index < 0 || index > this.Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				return (XmlSchema)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public XmlSchema this[string ns]
		{
			get
			{
				return (XmlSchema)this.table[(ns == null) ? string.Empty : ns];
			}
		}

		[MonoTODO]
		public bool IsCompiled
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO]
		public void Compile(ValidationEventHandler handler, bool fullCompile)
		{
			foreach (object obj in this)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (fullCompile || !xmlSchema.IsCompiled)
				{
					xmlSchema.Compile(handler);
				}
			}
		}

		public int Add(XmlSchema schema)
		{
			this.Insert(this.Count, schema);
			return this.Count - 1;
		}

		public void Add(XmlSchemas schemas)
		{
			foreach (object obj in schemas)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				this.Add(xmlSchema);
			}
		}

		[MonoNotSupported("")]
		public int Add(XmlSchema schema, Uri baseUri)
		{
			throw new NotImplementedException();
		}

		[MonoNotSupported("")]
		public void AddReference(XmlSchema schema)
		{
			throw new NotImplementedException();
		}

		public bool Contains(XmlSchema schema)
		{
			return base.List.Contains(schema);
		}

		[MonoNotSupported("")]
		public bool Contains(string targetNamespace)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(XmlSchema[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		public object Find(XmlQualifiedName name, Type type)
		{
			XmlSchema xmlSchema = this.table[name.Namespace] as XmlSchema;
			if (xmlSchema == null)
			{
				foreach (object obj in this)
				{
					XmlSchema xmlSchema2 = (XmlSchema)obj;
					object obj2 = this.Find(xmlSchema2, name, type);
					if (obj2 != null)
					{
						return obj2;
					}
				}
				return null;
			}
			object obj3 = this.Find(xmlSchema, name, type);
			if (obj3 == null)
			{
				foreach (object obj4 in this)
				{
					XmlSchema xmlSchema3 = (XmlSchema)obj4;
					object obj5 = this.Find(xmlSchema3, name, type);
					if (obj5 != null)
					{
						return obj5;
					}
				}
			}
			return obj3;
		}

		private object Find(XmlSchema schema, XmlQualifiedName name, Type type)
		{
			if (!schema.IsCompiled)
			{
				schema.Compile(null);
			}
			XmlSchemaObjectTable xmlSchemaObjectTable = null;
			if (type == typeof(XmlSchemaSimpleType) || type == typeof(XmlSchemaComplexType))
			{
				xmlSchemaObjectTable = schema.SchemaTypes;
			}
			else if (type == typeof(XmlSchemaAttribute))
			{
				xmlSchemaObjectTable = schema.Attributes;
			}
			else if (type == typeof(XmlSchemaAttributeGroup))
			{
				xmlSchemaObjectTable = schema.AttributeGroups;
			}
			else if (type == typeof(XmlSchemaElement))
			{
				xmlSchemaObjectTable = schema.Elements;
			}
			else if (type == typeof(XmlSchemaGroup))
			{
				xmlSchemaObjectTable = schema.Groups;
			}
			else if (type == typeof(XmlSchemaNotation))
			{
				xmlSchemaObjectTable = schema.Notations;
			}
			object obj = ((xmlSchemaObjectTable == null) ? null : xmlSchemaObjectTable[name]);
			if (obj != null && obj.GetType() != type)
			{
				return null;
			}
			return obj;
		}

		[MonoNotSupported("")]
		public IList GetSchemas(string ns)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(XmlSchema schema)
		{
			return base.List.IndexOf(schema);
		}

		public void Insert(int index, XmlSchema schema)
		{
			base.List.Insert(index, schema);
		}

		public static bool IsDataSet(XmlSchema schema)
		{
			XmlSchemaElement xmlSchemaElement = ((schema.Items.Count != 1) ? null : (schema.Items[0] as XmlSchemaElement));
			if (xmlSchemaElement != null && xmlSchemaElement.UnhandledAttributes != null && xmlSchemaElement.UnhandledAttributes.Length > 0)
			{
				for (int i = 0; i < xmlSchemaElement.UnhandledAttributes.Length; i++)
				{
					XmlAttribute xmlAttribute = xmlSchemaElement.UnhandledAttributes[i];
					if (xmlAttribute.NamespaceURI == XmlSchemas.msdataNS && xmlAttribute.LocalName == "IsDataSet")
					{
						return xmlAttribute.Value.ToLower(CultureInfo.InvariantCulture) == "true";
					}
				}
			}
			return false;
		}

		protected override void OnClear()
		{
			this.table.Clear();
		}

		protected override void OnInsert(int index, object value)
		{
			string text = ((XmlSchema)value).TargetNamespace;
			if (text == null)
			{
				text = string.Empty;
			}
			this.table[text] = value;
		}

		protected override void OnRemove(int index, object value)
		{
			this.table.Remove(value);
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			string text = ((XmlSchema)oldValue).TargetNamespace;
			if (text == null)
			{
				text = string.Empty;
			}
			this.table[text] = newValue;
		}

		public void Remove(XmlSchema schema)
		{
			base.List.Remove(schema);
		}

		private static string msdataNS = "urn:schemas-microsoft-com:xml-msdata";

		private Hashtable table = new Hashtable();
	}
}
