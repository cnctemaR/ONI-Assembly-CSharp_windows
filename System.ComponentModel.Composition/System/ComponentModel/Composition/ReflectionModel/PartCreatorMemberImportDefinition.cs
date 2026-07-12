using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq.Expressions;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class PartCreatorMemberImportDefinition : ReflectionMemberImportDefinition, IPartCreatorImportDefinition
	{
		public PartCreatorMemberImportDefinition(LazyMemberInfo importingLazyMember, ICompositionElement origin, ContractBasedImportDefinition productImportDefinition)
			: base(importingLazyMember, "System.ComponentModel.Composition.Contracts.ExportFactory", CompositionConstants.PartCreatorTypeIdentity, productImportDefinition.RequiredMetadata, productImportDefinition.Cardinality, productImportDefinition.IsRecomposable, false, productImportDefinition.RequiredCreationPolicy, MetadataServices.EmptyMetadata, origin)
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
