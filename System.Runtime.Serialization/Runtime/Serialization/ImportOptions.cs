using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
	public class ImportOptions
	{
		public CodeDomProvider CodeProvider
		{
			get
			{
				return this.code_provider;
			}
			set
			{
				this.code_provider = value;
			}
		}

		public IDataContractSurrogate DataContractSurrogate
		{
			get
			{
				return this.surrogate;
			}
			set
			{
				this.surrogate = value;
			}
		}

		public bool EnableDataBinding
		{
			get
			{
				return this.enable_data_binding;
			}
			set
			{
				this.enable_data_binding = value;
			}
		}

		public bool GenerateInternal
		{
			get
			{
				return this.generate_internal;
			}
			set
			{
				this.generate_internal = value;
			}
		}

		public bool GenerateSerializable
		{
			get
			{
				return this.generate_serializable;
			}
			set
			{
				this.generate_serializable = value;
			}
		}

		public bool ImportXmlType
		{
			get
			{
				return this.import_xml_type;
			}
			set
			{
				this.import_xml_type = value;
			}
		}

		public IDictionary<string, string> Namespaces
		{
			get
			{
				return this.namespaces;
			}
		}

		public ICollection<Type> ReferencedCollectionTypes
		{
			get
			{
				return this.referenced_collection_types;
			}
		}

		public ICollection<Type> ReferencedTypes
		{
			get
			{
				return this.referenced_types;
			}
		}

		private IDataContractSurrogate surrogate;

		private ICollection<Type> referenced_collection_types = new List<Type>();

		private ICollection<Type> referenced_types = new List<Type>();

		private bool enable_data_binding;

		private bool generate_internal;

		private bool generate_serializable;

		private bool import_xml_type;

		private IDictionary<string, string> namespaces = new Dictionary<string, string>();

		private CodeDomProvider code_provider;
	}
}
