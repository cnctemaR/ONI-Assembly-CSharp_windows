using System;
using System.Collections;

namespace System.Xml.Schema
{
	[Obsolete("Use XmlSchemaSet.")]
	public sealed class XmlSchemaCollection : IEnumerable, ICollection
	{
		public XmlSchemaCollection()
			: this(new NameTable())
		{
		}

		public XmlSchemaCollection(XmlNameTable nameTable)
			: this(new XmlSchemaSet(nameTable))
		{
			this.schemaSet.ValidationEventHandler += this.OnValidationError;
		}

		internal XmlSchemaCollection(XmlSchemaSet schemaSet)
		{
			this.schemaSet = schemaSet;
		}

		public event ValidationEventHandler ValidationEventHandler;

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			XmlSchemaSet xmlSchemaSet = this.schemaSet;
			lock (xmlSchemaSet)
			{
				this.schemaSet.CopyTo(array, index);
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		internal XmlSchemaSet SchemaSet
		{
			get
			{
				return this.schemaSet;
			}
		}

		public int Count
		{
			get
			{
				return this.schemaSet.Count;
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return this.schemaSet.NameTable;
			}
		}

		public XmlSchema this[string ns]
		{
			get
			{
				ICollection collection = this.schemaSet.Schemas(ns);
				if (collection == null)
				{
					return null;
				}
				IEnumerator enumerator = collection.GetEnumerator();
				if (enumerator.MoveNext())
				{
					return (XmlSchema)enumerator.Current;
				}
				return null;
			}
		}

		public XmlSchema Add(string ns, XmlReader reader)
		{
			return this.Add(ns, reader, new XmlUrlResolver());
		}

		public XmlSchema Add(string ns, XmlReader reader, XmlResolver resolver)
		{
			XmlSchema xmlSchema = XmlSchema.Read(reader, this.ValidationEventHandler);
			if (xmlSchema.TargetNamespace == null)
			{
				xmlSchema.TargetNamespace = ns;
			}
			else if (ns != null && xmlSchema.TargetNamespace != ns)
			{
				throw new XmlSchemaException("The actual targetNamespace in the schema does not match the parameter.");
			}
			return this.Add(xmlSchema);
		}

		public XmlSchema Add(string ns, string uri)
		{
			XmlReader xmlReader = new XmlTextReader(uri);
			XmlSchema xmlSchema;
			try
			{
				xmlSchema = this.Add(ns, xmlReader);
			}
			finally
			{
				xmlReader.Close();
			}
			return xmlSchema;
		}

		public XmlSchema Add(XmlSchema schema)
		{
			return this.Add(schema, new XmlUrlResolver());
		}

		public XmlSchema Add(XmlSchema schema, XmlResolver resolver)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			XmlSchemaSet xmlSchemaSet = new XmlSchemaSet(this.schemaSet.NameTable);
			xmlSchemaSet.Add(this.schemaSet);
			xmlSchemaSet.Add(schema);
			xmlSchemaSet.ValidationEventHandler += this.ValidationEventHandler;
			xmlSchemaSet.XmlResolver = resolver;
			xmlSchemaSet.Compile();
			if (!xmlSchemaSet.IsCompiled)
			{
				return null;
			}
			this.schemaSet = xmlSchemaSet;
			return schema;
		}

		public void Add(XmlSchemaCollection schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			XmlSchemaSet xmlSchemaSet = new XmlSchemaSet(this.schemaSet.NameTable);
			xmlSchemaSet.Add(this.schemaSet);
			xmlSchemaSet.Add(schema.schemaSet);
			xmlSchemaSet.ValidationEventHandler += this.ValidationEventHandler;
			xmlSchemaSet.XmlResolver = this.schemaSet.XmlResolver;
			xmlSchemaSet.Compile();
			if (!xmlSchemaSet.IsCompiled)
			{
				return;
			}
			this.schemaSet = xmlSchemaSet;
		}

		public bool Contains(string ns)
		{
			XmlSchemaSet xmlSchemaSet = this.schemaSet;
			bool flag;
			lock (xmlSchemaSet)
			{
				flag = this.schemaSet.Contains(ns);
			}
			return flag;
		}

		public bool Contains(XmlSchema schema)
		{
			XmlSchemaSet xmlSchemaSet = this.schemaSet;
			bool flag;
			lock (xmlSchemaSet)
			{
				flag = this.schemaSet.Contains(schema);
			}
			return flag;
		}

		public void CopyTo(XmlSchema[] array, int index)
		{
			XmlSchemaSet xmlSchemaSet = this.schemaSet;
			lock (xmlSchemaSet)
			{
				this.schemaSet.CopyTo(array, index);
			}
		}

		public XmlSchemaCollectionEnumerator GetEnumerator()
		{
			return new XmlSchemaCollectionEnumerator(this.schemaSet.Schemas());
		}

		private void OnValidationError(object o, ValidationEventArgs e)
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

		private XmlSchemaSet schemaSet;
	}
}
