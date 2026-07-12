using System;
using System.Collections.Generic;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
	public class ExportDefinition
	{
		protected ExportDefinition()
		{
		}

		public ExportDefinition(string contractName, IDictionary<string, object> metadata)
		{
			Requires.NotNullOrEmpty(contractName, "contractName");
			this._contractName = contractName;
			if (metadata != null)
			{
				this._metadata = metadata.AsReadOnly();
			}
		}

		public virtual string ContractName
		{
			get
			{
				if (this._contractName != null)
				{
					return this._contractName;
				}
				throw ExceptionBuilder.CreateNotOverriddenByDerived("ContractName");
			}
		}

		public virtual IDictionary<string, object> Metadata
		{
			get
			{
				return this._metadata;
			}
		}

		public override string ToString()
		{
			return this.ContractName;
		}

		private readonly IDictionary<string, object> _metadata = MetadataServices.EmptyMetadata;

		private readonly string _contractName;
	}
}
