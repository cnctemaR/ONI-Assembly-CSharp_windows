using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class ReflectionParameterImportDefinition : ReflectionImportDefinition
	{
		public ReflectionParameterImportDefinition(Lazy<ParameterInfo> importingLazyParameter, string contractName, string requiredTypeIdentity, IEnumerable<KeyValuePair<string, Type>> requiredMetadata, ImportCardinality cardinality, CreationPolicy requiredCreationPolicy, IDictionary<string, object> metadata, ICompositionElement origin)
			: base(contractName, requiredTypeIdentity, requiredMetadata, cardinality, false, true, requiredCreationPolicy, metadata, origin)
		{
			Assumes.NotNull<Lazy<ParameterInfo>>(importingLazyParameter);
			this._importingLazyParameter = importingLazyParameter;
		}

		public override ImportingItem ToImportingItem()
		{
			return new ImportingParameter(this, new ImportType(this.ImportingLazyParameter.GetNotNullValue<ParameterInfo>("parameter").ParameterType, this.Cardinality));
		}

		public Lazy<ParameterInfo> ImportingLazyParameter
		{
			get
			{
				return this._importingLazyParameter;
			}
		}

		protected override string GetDisplayName()
		{
			ParameterInfo notNullValue = this.ImportingLazyParameter.GetNotNullValue<ParameterInfo>("parameter");
			return string.Format(CultureInfo.CurrentCulture, "{0} (Parameter=\"{1}\", ContractName=\"{2}\")", notNullValue.Member.GetDisplayName(), notNullValue.Name, this.ContractName);
		}

		private Lazy<ParameterInfo> _importingLazyParameter;
	}
}
