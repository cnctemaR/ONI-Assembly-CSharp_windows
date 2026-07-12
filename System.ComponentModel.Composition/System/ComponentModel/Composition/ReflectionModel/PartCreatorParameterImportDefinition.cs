using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class PartCreatorParameterImportDefinition : ReflectionParameterImportDefinition, IPartCreatorImportDefinition
	{
		public PartCreatorParameterImportDefinition(Lazy<ParameterInfo> importingLazyParameter, ICompositionElement origin, ContractBasedImportDefinition productImportDefinition)
			: base(importingLazyParameter, "System.ComponentModel.Composition.Contracts.ExportFactory", CompositionConstants.PartCreatorTypeIdentity, productImportDefinition.RequiredMetadata, productImportDefinition.Cardinality, CreationPolicy.Any, MetadataServices.EmptyMetadata, origin)
		{
			Assumes.NotNull<ContractBasedImportDefinition>(productImportDefinition);
			this._productImportDefinition = productImportDefinition;
		}

		public ContractBasedImportDefinition ProductImportDefinition
		{
			get
			{
				return this._productImportDefinition;
			}
		}

		public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
		{
			return base.IsConstraintSatisfiedBy(exportDefinition) && PartCreatorExportDefinition.IsProductConstraintSatisfiedBy(this._productImportDefinition, exportDefinition);
		}

		public override Expression<Func<ExportDefinition, bool>> Constraint
		{
			get
			{
				return ConstraintServices.CreatePartCreatorConstraint(base.Constraint, this._productImportDefinition);
			}
		}

		private readonly ContractBasedImportDefinition _productImportDefinition;
	}
}
