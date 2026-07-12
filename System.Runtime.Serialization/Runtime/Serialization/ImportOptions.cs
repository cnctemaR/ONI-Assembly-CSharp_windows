using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
	public class ImportOptions
	{
		public bool GenerateSerializable
		{
			get
			{
				return this.generateSerializable;
			}
			set
			{
				this.generateSerializable = value;
			}
		}

		public bool GenerateInternal
		{
			get
			{
				return this.generateInternal;
			}
			set
			{
				this.generateInternal = value;
			}
		}

		public bool EnableDataBinding
		{
			get
			{
				return this.enableDataBinding;
			}
			set
			{
				this.enableDataBinding = value;
			}
		}

		public CodeDomProvider CodeProvider
		{
			get
			{
				return this.codeProvider;
			}
			set
			{
				this.codeProvider = value;
			}
		}

		public ICollection<Type> ReferencedTypes
		{
			get
			{
				if (this.referencedTypes == null)
				{
					this.referencedTypes = new List<Type>();
				}
				return this.referencedTypes;
			}
		}

		public ICollection<Type> ReferencedCollectionTypes
		{
			get
			{
				if (this.referencedCollectionTypes == null)
				{
					this.referencedCollectionTypes = new List<Type>();
				}
				return this.referencedCollectionTypes;
			}
		}

		public IDictionary<string, string> Namespaces
		{
			get
			{
				if (this.namespaces == null)
				{
					this.namespaces = new Dictionary<string, string>();
				}
				return this.namespaces;
			}
		}

		public bool ImportXmlType
		{
			get
			{
				return this.importXmlType;
			}
			set
			{
				this.importXmlType = value;
			}
		}

		public IDataContractSurrogate DataContractSurrogate
		{
			get
			{
				return this.dataContractSurrogate;
			}
			set
			{
				this.dataContractSurrogate = value;
			}
		}

		private bool generateSerializable;

		private bool generateInternal;

		private bool enableDataBinding;

		private CodeDomProvider codeProvider;

		private ICollection<Type> referencedTypes;

		private ICollection<Type> referencedCollectionTypes;

		private IDictionary<string, string> namespaces;

		private bool importXmlType;

		private IDataContractSurrogate dataContractSurrogate;
	}
}
