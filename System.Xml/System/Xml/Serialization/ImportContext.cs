using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.Xml.Serialization
{
	public class ImportContext
	{
		public ImportContext(CodeIdentifiers identifiers, bool shareTypes)
		{
			this._typeIdentifiers = identifiers;
			this._shareTypes = shareTypes;
			if (shareTypes)
			{
				this.MappedTypes = new Hashtable();
				this.DataMappedTypes = new Hashtable();
				this.SharedAnonymousTypes = new Hashtable();
			}
		}

		public bool ShareTypes
		{
			get
			{
				return this._shareTypes;
			}
		}

		public CodeIdentifiers TypeIdentifiers
		{
			get
			{
				return this._typeIdentifiers;
			}
		}

		public StringCollection Warnings
		{
			get
			{
				return this._warnings;
			}
		}

		private bool _shareTypes;

		private CodeIdentifiers _typeIdentifiers;

		private StringCollection _warnings = new StringCollection();

		internal Hashtable MappedTypes;

		internal Hashtable DataMappedTypes;

		internal Hashtable SharedAnonymousTypes;
	}
}
