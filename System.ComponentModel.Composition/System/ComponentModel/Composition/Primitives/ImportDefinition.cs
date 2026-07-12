using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
	public class ImportDefinition
	{
		protected ImportDefinition()
		{
		}

		public ImportDefinition(Expression<Func<ExportDefinition, bool>> constraint, string contractName, ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite)
			: this(contractName, cardinality, isRecomposable, isPrerequisite, MetadataServices.EmptyMetadata)
		{
			Requires.NotNull<Expression<Func<ExportDefinition, bool>>>(constraint, "constraint");
			this._constraint = constraint;
		}

		public ImportDefinition(Expression<Func<ExportDefinition, bool>> constraint, string contractName, ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite, IDictionary<string, object> metadata)
			: this(contractName, cardinality, isRecomposable, isPrerequisite, metadata)
		{
			Requires.NotNull<Expression<Func<ExportDefinition, bool>>>(constraint, "constraint");
			this._constraint = constraint;
		}

		internal ImportDefinition(string contractName, ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite, IDictionary<string, object> metadata)
		{
			if (cardinality != ImportCardinality.ExactlyOne && cardinality != ImportCardinality.ZeroOrMore && cardinality != ImportCardinality.ZeroOrOne)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ArgumentOutOfRange_InvalidEnum, "cardinality", cardinality, typeof(ImportCardinality).Name), "cardinality");
			}
			this._contractName = contractName ?? ImportDefinition.EmptyContractName;
			this._cardinality = cardinality;
			this._isRecomposable = isRecomposable;
			this._isPrerequisite = isPrerequisite;
			if (metadata != null)
			{
				this._metadata = metadata;
			}
		}

		public virtual string ContractName
		{
			get
			{
				return this._contractName;
			}
		}

		public virtual IDictionary<string, object> Metadata
		{
			get
			{
				return this._metadata;
			}
		}

		public virtual ImportCardinality Cardinality
		{
			get
			{
				return this._cardinality;
			}
		}

		public virtual Expression<Func<ExportDefinition, bool>> Constraint
		{
			get
			{
				if (this._constraint != null)
				{
					return this._constraint;
				}
				throw ExceptionBuilder.CreateNotOverriddenByDerived("Constraint");
			}
		}

		public virtual bool IsPrerequisite
		{
			get
			{
				return this._isPrerequisite;
			}
		}

		public virtual bool IsRecomposable
		{
			get
			{
				return this._isRecomposable;
			}
		}

		public virtual bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
		{
			Requires.NotNull<ExportDefinition>(exportDefinition, "exportDefinition");
			if (this._compiledConstraint == null)
			{
				this._compiledConstraint = this.Constraint.Compile();
			}
			return this._compiledConstraint(exportDefinition);
		}

		public override string ToString()
		{
			return this.Constraint.Body.ToString();
		}

		internal static readonly string EmptyContractName = string.Empty;

		private readonly Expression<Func<ExportDefinition, bool>> _constraint;

		private readonly ImportCardinality _cardinality = ImportCardinality.ExactlyOne;

		private readonly string _contractName = ImportDefinition.EmptyContractName;

		private readonly bool _isRecomposable;

		private readonly bool _isPrerequisite = true;

		private Func<ExportDefinition, bool> _compiledConstraint;

		private readonly IDictionary<string, object> _metadata = MetadataServices.EmptyMetadata;
	}
}
