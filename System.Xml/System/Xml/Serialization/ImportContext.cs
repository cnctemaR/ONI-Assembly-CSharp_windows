using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.Xml.Serialization
{
	public class ImportContext
	{
		public ImportContext(CodeIdentifiers identifiers, bool shareTypes)
		{
			this.typeIdentifiers = identifiers;
			this.shareTypes = shareTypes;
		}

		internal ImportContext()
			: this(null, false)
		{
		}

		internal SchemaObjectCache Cache
		{
			get
			{
				if (this.cache == null)
				{
					this.cache = new SchemaObjectCache();
				}
				return this.cache;
			}
		}

		internal Hashtable Elements
		{
			get
			{
				if (this.elements == null)
				{
					this.elements = new Hashtable();
				}
				return this.elements;
			}
		}

		internal Hashtable Mappings
		{
			get
			{
				if (this.mappings == null)
				{
					this.mappings = new Hashtable();
				}
				return this.mappings;
			}
		}

		public CodeIdentifiers TypeIdentifiers
		{
			get
			{
				if (this.typeIdentifiers == null)
				{
					this.typeIdentifiers = new CodeIdentifiers();
				}
				return this.typeIdentifiers;
			}
		}

		public bool ShareTypes
		{
			get
			{
				return this.shareTypes;
			}
		}

		public StringCollection Warnings
		{
			get
			{
				return this.Cache.Warnings;
			}
		}

		private bool shareTypes;

		private SchemaObjectCache cache;

		private Hashtable mappings;

		private Hashtable elements;

		private CodeIdentifiers typeIdentifiers;
	}
}
