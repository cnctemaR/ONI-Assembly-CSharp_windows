using System;
using System.Collections;
using System.Threading;
using System.Xml.XmlConfiguration;

namespace System.Xml.Schema
{
	[Obsolete("Use System.Xml.Schema.XmlSchemaSet for schema compilation and validation. http://go.microsoft.com/fwlink/?linkid=14202")]
	public sealed class XmlSchemaCollection : ICollection, IEnumerable
	{
		public XmlSchemaCollection()
			: this(new NameTable())
		{
		}

		public XmlSchemaCollection(XmlNameTable nametable)
		{
			if (nametable == null)
			{
				throw new ArgumentNullException("nametable");
			}
			this.nameTable = nametable;
			this.collection = Hashtable.Synchronized(new Hashtable());
			this.xmlResolver = XmlReaderSection.CreateDefaultResolver();
			this.isThreadSafe = true;
			if (this.isThreadSafe)
			{
				this.wLock = new ReaderWriterLock();
			}
		}

		public int Count
		{
			get
			{
				return this.collection.Count;
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		public event ValidationEventHandler ValidationEventHandler
		{
			add
			{
				this.validationEventHandler = (ValidationEventHandler)Delegate.Combine(this.validationEventHandler, value);
			}
			remove
			{
				this.validationEventHandler = (ValidationEventHandler)Delegate.Remove(this.validationEventHandler, value);
			}
		}

		internal XmlResolver XmlResolver
		{
			set
			{
				this.xmlResolver = value;
			}
		}

		public XmlSchema Add(string ns, string uri)
		{
			if (uri == null || uri.Length == 0)
			{
				throw new ArgumentNullException("uri");
			}
			XmlTextReader xmlTextReader = new XmlTextReader(uri, this.nameTable);
			xmlTextReader.XmlResolver = this.xmlResolver;
			XmlSchema xmlSchema = null;
			try
			{
				xmlSchema = this.Add(ns, xmlTextReader, this.xmlResolver);
				while (xmlTextReader.Read())
				{
				}
			}
			finally
			{
				xmlTextReader.Close();
			}
			return xmlSchema;
		}

		public XmlSchema Add(string ns, XmlReader reader)
		{
			return this.Add(ns, reader, this.xmlResolver);
		}

		public XmlSchema Add(string ns, XmlReader reader, XmlResolver resolver)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			XmlNameTable xmlNameTable = reader.NameTable;
			SchemaInfo schemaInfo = new SchemaInfo();
			Parser parser = new Parser(SchemaType.None, xmlNameTable, this.GetSchemaNames(xmlNameTable), this.validationEventHandler);
			parser.XmlResolver = resolver;
			SchemaType schemaType;
			try
			{
				schemaType = parser.Parse(reader, ns);
			}
			catch (XmlSchemaException ex)
			{
				this.SendValidationEvent(ex);
				return null;
			}
			if (schemaType == SchemaType.XSD)
			{
				schemaInfo.SchemaType = SchemaType.XSD;
				return this.Add(ns, schemaInfo, parser.XmlSchema, true, resolver);
			}
			SchemaInfo xdrSchema = parser.XdrSchema;
			return this.Add(ns, parser.XdrSchema, null, true, resolver);
		}

		public XmlSchema Add(XmlSchema schema)
		{
			return this.Add(schema, this.xmlResolver);
		}

		public XmlSchema Add(XmlSchema schema, XmlResolver resolver)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			SchemaInfo schemaInfo = new SchemaInfo();
			schemaInfo.SchemaType = SchemaType.XSD;
			return this.Add(schema.TargetNamespace, schemaInfo, schema, true, resolver);
		}

		public void Add(XmlSchemaCollection schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			if (this == schema)
			{
				return;
			}
			IDictionaryEnumerator enumerator = schema.collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				XmlSchemaCollectionNode xmlSchemaCollectionNode = (XmlSchemaCollectionNode)enumerator.Value;
				this.Add(xmlSchemaCollectionNode.NamespaceURI, xmlSchemaCollectionNode);
			}
		}

		public XmlSchema this[string ns]
		{
			get
			{
				XmlSchemaCollectionNode xmlSchemaCollectionNode = (XmlSchemaCollectionNode)this.collection[(ns != null) ? ns : string.Empty];
				if (xmlSchemaCollectionNode == null)
				{
					return null;
				}
				return xmlSchemaCollectionNode.Schema;
			}
		}

		public bool Contains(XmlSchema schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			return this[schema.TargetNamespace] != null;
		}

		public bool Contains(string ns)
		{
			return this.collection[(ns != null) ? ns : string.Empty] != null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new XmlSchemaCollectionEnumerator(this.collection);
		}

		public XmlSchemaCollectionEnumerator GetEnumerator()
		{
			return new XmlSchemaCollectionEnumerator(this.collection);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			XmlSchemaCollectionEnumerator enumerator = this.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (index == array.Length && array.IsFixedSize)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				array.SetValue(enumerator.Current, index++);
			}
		}

		public void CopyTo(XmlSchema[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			XmlSchemaCollectionEnumerator enumerator = this.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current != null)
				{
					if (index == array.Length)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					array[index++] = enumerator.Current;
				}
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		int ICollection.Count
		{
			get
			{
				return this.collection.Count;
			}
		}

		internal SchemaInfo GetSchemaInfo(string ns)
		{
			XmlSchemaCollectionNode xmlSchemaCollectionNode = (XmlSchemaCollectionNode)this.collection[(ns != null) ? ns : string.Empty];
			if (xmlSchemaCollectionNode == null)
			{
				return null;
			}
			return xmlSchemaCollectionNode.SchemaInfo;
		}

		internal SchemaNames GetSchemaNames(XmlNameTable nt)
		{
			if (this.nameTable != nt)
			{
				return new SchemaNames(nt);
			}
			if (this.schemaNames == null)
			{
				this.schemaNames = new SchemaNames(this.nameTable);
			}
			return this.schemaNames;
		}

		internal XmlSchema Add(string ns, SchemaInfo schemaInfo, XmlSchema schema, bool compile)
		{
			return this.Add(ns, schemaInfo, schema, compile, this.xmlResolver);
		}

		private XmlSchema Add(string ns, SchemaInfo schemaInfo, XmlSchema schema, bool compile, XmlResolver resolver)
		{
			int num = 0;
			if (schema != null)
			{
				if (schema.ErrorCount == 0 && compile)
				{
					if (!schema.CompileSchema(this, resolver, schemaInfo, ns, this.validationEventHandler, this.nameTable, true))
					{
						num = 1;
					}
					ns = ((schema.TargetNamespace == null) ? string.Empty : schema.TargetNamespace);
				}
				num += schema.ErrorCount;
			}
			else
			{
				num += schemaInfo.ErrorCount;
				ns = this.NameTable.Add(ns);
			}
			if (num == 0)
			{
				this.Add(ns, new XmlSchemaCollectionNode
				{
					NamespaceURI = ns,
					SchemaInfo = schemaInfo,
					Schema = schema
				});
				return schema;
			}
			return null;
		}

		private void Add(string ns, XmlSchemaCollectionNode node)
		{
			if (this.isThreadSafe)
			{
				this.wLock.AcquireWriterLock(this.timeout);
			}
			try
			{
				if (this.collection[ns] != null)
				{
					this.collection.Remove(ns);
				}
				this.collection.Add(ns, node);
			}
			finally
			{
				if (this.isThreadSafe)
				{
					this.wLock.ReleaseWriterLock();
				}
			}
		}

		private void SendValidationEvent(XmlSchemaException e)
		{
			if (this.validationEventHandler != null)
			{
				this.validationEventHandler(this, new ValidationEventArgs(e));
				return;
			}
			throw e;
		}

		internal ValidationEventHandler EventHandler
		{
			get
			{
				return this.validationEventHandler;
			}
			set
			{
				this.validationEventHandler = value;
			}
		}

		private Hashtable collection;

		private XmlNameTable nameTable;

		private SchemaNames schemaNames;

		private ReaderWriterLock wLock;

		private int timeout = -1;

		private bool isThreadSafe = true;

		private ValidationEventHandler validationEventHandler;

		private XmlResolver xmlResolver;
	}
}
