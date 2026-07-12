using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal abstract class ReflectionImportDefinition : ContractBasedImportDefinition, ICompositionElement
	{
		public ReflectionImportDefinition(string contractName, string requiredTypeIdentity, IEnumerable<KeyValuePair<string, Type>> requiredMetadata, ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite, CreationPolicy requiredCreationPolicy, IDictionary<string, object> metadata, ICompositionElement origin)
			: base(contractName, requiredTypeIdentity, requiredMetadata, cardinality, isRecomposable, isPrerequisite, requiredCreationPolicy, metadata)
		{
			this._origin = origin;
		}

		string ICompositionElement.DisplayName
		{
			get
			{
				return this.GetDisplayName();
			}
		}

		ICompositionElement ICompositionElement.Origin
		{
			get
			{
				return this._origin;
			}
		}

		public abstract ImportingItem ToImportingItem();

		protected abstract string GetDisplayName();

		private readonly ICompositionElement _origin;
	}
}
