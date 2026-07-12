using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq.Expressions;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	internal static class ImportSourceImportDefinitionHelpers
	{
		public static ImportDefinition RemoveImportSource(this ImportDefinition definition)
		{
			ContractBasedImportDefinition contractBasedImportDefinition = definition as ContractBasedImportDefinition;
			if (contractBasedImportDefinition == null)
			{
				return definition;
			}
			return new ImportSourceImportDefinitionHelpers.NonImportSourceImportDefinition(contractBasedImportDefinition);
		}

		internal class NonImportSourceImportDefinition : ContractBasedImportDefinition
		{
			public NonImportSourceImportDefinition(ContractBasedImportDefinition sourceDefinition)
			{
				Assumes.NotNull<ContractBasedImportDefinition>(sourceDefinition);
				this._sourceDefinition = sourceDefinition;
				this._metadata = null;
			}

			public override string ContractName
			{
				get
				{
					return this._sourceDefinition.ContractName;
				}
			}

			public override IDictionary<string, object> Metadata
			{
				get
				{
					IDictionary<string, object> dictionary = this._metadata;
					if (dictionary == null)
					{
						dictionary = new Dictionary<string, object>(this._sourceDefinition.Metadata);
						dictionary.Remove("System.ComponentModel.Composition.ImportSource");
						this._metadata = dictionary;
					}
					return dictionary;
				}
			}

			public override ImportCardinality Cardinality
			{
				get
				{
					return this._sourceDefinition.Cardinality;
				}
			}

			public override Expression<Func<ExportDefinition, bool>> Constraint
			{
				get
				{
					return this._sourceDefinition.Constraint;
				}
			}

			public override bool IsPrerequisite
			{
				get
				{
					return this._sourceDefinition.IsPrerequisite;
				}
			}

			public override bool IsRecomposable
			{
				get
				{
					return this._sourceDefinition.IsRecomposable;
				}
			}

			public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
			{
				Requires.NotNull<ExportDefinition>(exportDefinition, "exportDefinition");
				return this._sourceDefinition.IsConstraintSatisfiedBy(exportDefinition);
			}

			public override string ToString()
			{
				return this._sourceDefinition.ToString();
			}

			public override string RequiredTypeIdentity
			{
				get
				{
					return this._sourceDefinition.RequiredTypeIdentity;
				}
			}

			public override IEnumerable<KeyValuePair<string, Type>> RequiredMetadata
			{
				get
				{
					return this._sourceDefinition.RequiredMetadata;
				}
			}

			public override CreationPolicy RequiredCreationPolicy
			{
				get
				{
					return this._sourceDefinition.RequiredCreationPolicy;
				}
			}

			private ContractBasedImportDefinition _sourceDefinition;

			private IDictionary<string, object> _metadata;
		}
	}
}
