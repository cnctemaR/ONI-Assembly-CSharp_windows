using System;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal abstract class ImportingItem
	{
		protected ImportingItem(ContractBasedImportDefinition definition, ImportType importType)
		{
			Assumes.NotNull<ContractBasedImportDefinition>(definition);
			this._definition = definition;
			this._importType = importType;
		}

		public ContractBasedImportDefinition Definition
		{
			get
			{
				return this._definition;
			}
		}

		public ImportType ImportType
		{
			get
			{
				return this._importType;
			}
		}

		public object CastExportsToImportType(Export[] exports)
		{
			if (this.Definition.Cardinality == ImportCardinality.ZeroOrMore)
			{
				return this.CastExportsToCollectionImportType(exports);
			}
			return this.CastExportsToSingleImportType(exports);
		}

		private object CastExportsToCollectionImportType(Export[] exports)
		{
			Assumes.NotNull<Export[]>(exports);
			Type type = this.ImportType.ElementType ?? typeof(object);
			Array array = Array.CreateInstance(type, exports.Length);
			for (int i = 0; i < array.Length; i++)
			{
				object obj = this.CastSingleExportToImportType(type, exports[i]);
				array.SetValue(obj, i);
			}
			return array;
		}

		private object CastExportsToSingleImportType(Export[] exports)
		{
			Assumes.NotNull<Export[]>(exports);
			Assumes.IsTrue(exports.Length < 2);
			if (exports.Length == 0)
			{
				return null;
			}
			return this.CastSingleExportToImportType(this.ImportType.ActualType, exports[0]);
		}

		private object CastSingleExportToImportType(Type type, Export export)
		{
			if (this.ImportType.CastExport != null)
			{
				return this.ImportType.CastExport(export);
			}
			return this.Cast(type, export);
		}

		private object Cast(Type type, Export export)
		{
			object value = export.Value;
			object obj;
			if (!ContractServices.TryCast(type, value, out obj))
			{
				throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ImportNotAssignableFromExport, export.ToElement().DisplayName, type.FullName), this.Definition.ToElement());
			}
			return obj;
		}

		private readonly ContractBasedImportDefinition _definition;

		private readonly ImportType _importType;
	}
}
