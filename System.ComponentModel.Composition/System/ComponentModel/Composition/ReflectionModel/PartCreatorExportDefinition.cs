using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class PartCreatorExportDefinition : ExportDefinition
	{
		public PartCreatorExportDefinition(ExportDefinition productDefinition)
		{
			this._productDefinition = productDefinition;
		}

		public override string ContractName
		{
			get
			{
				return "System.ComponentModel.Composition.Contracts.ExportFactory";
			}
		}

		public override IDictionary<string, object> Metadata
		{
			get
			{
				if (this._metadata == null)
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>(this._productDefinition.Metadata);
					dictionary["ExportTypeIdentity"] = CompositionConstants.PartCreatorTypeIdentity;
					dictionary["ProductDefinition"] = this._productDefinition;
					this._metadata = dictionary.AsReadOnly();
				}
				return this._metadata;
			}
		}

		internal static bool IsProductConstraintSatisfiedBy(ImportDefinition productImportDefinition, ExportDefinition exportDefinition)
		{
			object obj = null;
			if (exportDefinition.Metadata.TryGetValue("ProductDefinition", out obj))
			{
				ExportDefinition exportDefinition2 = obj as ExportDefinition;
				if (exportDefinition2 != null)
				{
					return productImportDefinition.IsConstraintSatisfiedBy(exportDefinition2);
				}
			}
			return false;
		}

		private readonly ExportDefinition _productDefinition;

		private IDictionary<string, object> _metadata;
	}
}
