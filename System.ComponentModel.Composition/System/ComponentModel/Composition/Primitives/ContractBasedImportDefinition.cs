using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
	public class ContractBasedImportDefinition : ImportDefinition
	{
		protected ContractBasedImportDefinition()
		{
		}

		public ContractBasedImportDefinition(string contractName, string requiredTypeIdentity, IEnumerable<KeyValuePair<string, Type>> requiredMetadata, ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite, CreationPolicy requiredCreationPolicy)
			: this(contractName, requiredTypeIdentity, requiredMetadata, cardinality, isRecomposable, isPrerequisite, requiredCreationPolicy, MetadataServices.EmptyMetadata)
		{
		}

		public ContractBasedImportDefinition(string contractName, string requiredTypeIdentity, IEnumerable<KeyValuePair<string, Type>> requiredMetadata, ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite, CreationPolicy requiredCreationPolicy, IDictionary<string, object> metadata)
			: base(contractName, cardinality, isRecomposable, isPrerequisite, metadata)
		{
			Requires.NotNullOrEmpty(contractName, "contractName");
			this._requiredTypeIdentity = requiredTypeIdentity;
			if (requiredMetadata != null)
			{
				this._requiredMetadata = requiredMetadata;
			}
			this._requiredCreationPolicy = requiredCreationPolicy;
		}

		public virtual string RequiredTypeIdentity
		{
			get
			{
				return this._requiredTypeIdentity;
			}
		}

		public virtual IEnumerable<KeyValuePair<string, Type>> RequiredMetadata
		{
			get
			{
				this.ValidateRequiredMetadata();
				return this._requiredMetadata;
			}
		}

		private void ValidateRequiredMetadata()
		{
			if (!this._isRequiredMetadataValidated)
			{
				foreach (KeyValuePair<string, Type> keyValuePair in this._requiredMetadata)
				{
					if (keyValuePair.Key == null || keyValuePair.Value == null)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.Argument_NullElement, "requiredMetadata"));
					}
				}
				this._isRequiredMetadataValidated = true;
			}
		}

		public virtual CreationPolicy RequiredCreationPolicy
		{
			get
			{
				return this._requiredCreationPolicy;
			}
		}

		public override Expression<Func<ExportDefinition, bool>> Constraint
		{
			get
			{
				if (this._constraint == null)
				{
					this._constraint = ConstraintServices.CreateConstraint(this.ContractName, this.RequiredTypeIdentity, this.RequiredMetadata, this.RequiredCreationPolicy);
				}
				return this._constraint;
			}
		}

		public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
		{
			Requires.NotNull<ExportDefinition>(exportDefinition, "exportDefinition");
			return StringComparers.ContractName.Equals(this.ContractName, exportDefinition.ContractName) && this.MatchRequiredMatadata(exportDefinition);
		}

		private bool MatchRequiredMatadata(ExportDefinition definition)
		{
			if (!string.IsNullOrEmpty(this.RequiredTypeIdentity))
			{
				string value = definition.Metadata.GetValue<string>("ExportTypeIdentity");
				if (!StringComparers.ContractName.Equals(this.RequiredTypeIdentity, value))
				{
					return false;
				}
			}
			foreach (KeyValuePair<string, Type> keyValuePair in this.RequiredMetadata)
			{
				string key = keyValuePair.Key;
				Type value2 = keyValuePair.Value;
				object obj = null;
				if (!definition.Metadata.TryGetValue(key, out obj))
				{
					return false;
				}
				if (obj != null)
				{
					if (!value2.IsInstanceOfType(obj))
					{
						return false;
					}
				}
				else if (value2.IsValueType)
				{
					return false;
				}
			}
			if (this.RequiredCreationPolicy == CreationPolicy.Any)
			{
				return true;
			}
			CreationPolicy value3 = definition.Metadata.GetValue<CreationPolicy>("System.ComponentModel.Composition.CreationPolicy");
			return value3 == CreationPolicy.Any || value3 == this.RequiredCreationPolicy;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("\n\tContractName\t{0}", this.ContractName));
			stringBuilder.Append(string.Format("\n\tRequiredTypeIdentity\t{0}", this.RequiredTypeIdentity));
			if (this._requiredCreationPolicy != CreationPolicy.Any)
			{
				stringBuilder.Append(string.Format("\n\tRequiredCreationPolicy\t{0}", this.RequiredCreationPolicy));
			}
			if (this._requiredMetadata.Count<KeyValuePair<string, Type>>() > 0)
			{
				stringBuilder.Append(string.Format("\n\tRequiredMetadata", Array.Empty<object>()));
				foreach (KeyValuePair<string, Type> keyValuePair in this._requiredMetadata)
				{
					stringBuilder.Append(string.Format("\n\t\t{0}\t({1})", keyValuePair.Key, keyValuePair.Value));
				}
			}
			return stringBuilder.ToString();
		}

		private readonly IEnumerable<KeyValuePair<string, Type>> _requiredMetadata = Enumerable.Empty<KeyValuePair<string, Type>>();

		private Expression<Func<ExportDefinition, bool>> _constraint;

		private readonly CreationPolicy _requiredCreationPolicy;

		private readonly string _requiredTypeIdentity;

		private bool _isRequiredMetadataValidated;
	}
}
