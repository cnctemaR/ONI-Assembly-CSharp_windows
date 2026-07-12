using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Linq;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	public abstract class ExportProvider
	{
		public Lazy<T> GetExport<T>()
		{
			return this.GetExport<T>(null);
		}

		public Lazy<T> GetExport<T>(string contractName)
		{
			return this.GetExportCore<T>(contractName);
		}

		public Lazy<T, TMetadataView> GetExport<T, TMetadataView>()
		{
			return this.GetExport<T, TMetadataView>(null);
		}

		public Lazy<T, TMetadataView> GetExport<T, TMetadataView>(string contractName)
		{
			return this.GetExportCore<T, TMetadataView>(contractName);
		}

		public IEnumerable<Lazy<object, object>> GetExports(Type type, Type metadataViewType, string contractName)
		{
			IEnumerable<Export> exportsCore = this.GetExportsCore(type, metadataViewType, contractName, ImportCardinality.ZeroOrMore);
			Collection<Lazy<object, object>> collection = new Collection<Lazy<object, object>>();
			Func<Export, Lazy<object, object>> func = ExportServices.CreateSemiStronglyTypedLazyFactory(type, metadataViewType);
			foreach (Export export in exportsCore)
			{
				collection.Add(func(export));
			}
			return collection;
		}

		public IEnumerable<Lazy<T>> GetExports<T>()
		{
			return this.GetExports<T>(null);
		}

		public IEnumerable<Lazy<T>> GetExports<T>(string contractName)
		{
			return this.GetExportsCore<T>(contractName);
		}

		public IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>()
		{
			return this.GetExports<T, TMetadataView>(null);
		}

		public IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>(string contractName)
		{
			return this.GetExportsCore<T, TMetadataView>(contractName);
		}

		public T GetExportedValue<T>()
		{
			return this.GetExportedValue<T>(null);
		}

		public T GetExportedValue<T>(string contractName)
		{
			return this.GetExportedValueCore<T>(contractName, ImportCardinality.ExactlyOne);
		}

		public T GetExportedValueOrDefault<T>()
		{
			return this.GetExportedValueOrDefault<T>(null);
		}

		public T GetExportedValueOrDefault<T>(string contractName)
		{
			return this.GetExportedValueCore<T>(contractName, ImportCardinality.ZeroOrOne);
		}

		public IEnumerable<T> GetExportedValues<T>()
		{
			return this.GetExportedValues<T>(null);
		}

		public IEnumerable<T> GetExportedValues<T>(string contractName)
		{
			return this.GetExportedValuesCore<T>(contractName);
		}

		private IEnumerable<T> GetExportedValuesCore<T>(string contractName)
		{
			IEnumerable<Export> exportsCore = this.GetExportsCore(typeof(T), null, contractName, ImportCardinality.ZeroOrMore);
			Collection<T> collection = new Collection<T>();
			foreach (Export export in exportsCore)
			{
				collection.Add(ExportServices.GetCastedExportedValue<T>(export));
			}
			return collection;
		}

		private T GetExportedValueCore<T>(string contractName, ImportCardinality cardinality)
		{
			Assumes.IsTrue(cardinality.IsAtMostOne());
			Export export = this.GetExportsCore(typeof(T), null, contractName, cardinality).SingleOrDefault<Export>();
			if (export == null)
			{
				return default(T);
			}
			return ExportServices.GetCastedExportedValue<T>(export);
		}

		private IEnumerable<Lazy<T>> GetExportsCore<T>(string contractName)
		{
			IEnumerable<Export> exportsCore = this.GetExportsCore(typeof(T), null, contractName, ImportCardinality.ZeroOrMore);
			Collection<Lazy<T>> collection = new Collection<Lazy<T>>();
			foreach (Export export in exportsCore)
			{
				collection.Add(ExportServices.CreateStronglyTypedLazyOfT<T>(export));
			}
			return collection;
		}

		private IEnumerable<Lazy<T, TMetadataView>> GetExportsCore<T, TMetadataView>(string contractName)
		{
			IEnumerable<Export> exportsCore = this.GetExportsCore(typeof(T), typeof(TMetadataView), contractName, ImportCardinality.ZeroOrMore);
			Collection<Lazy<T, TMetadataView>> collection = new Collection<Lazy<T, TMetadataView>>();
			foreach (Export export in exportsCore)
			{
				collection.Add(ExportServices.CreateStronglyTypedLazyOfTM<T, TMetadataView>(export));
			}
			return collection;
		}

		private Lazy<T, TMetadataView> GetExportCore<T, TMetadataView>(string contractName)
		{
			Export export = this.GetExportsCore(typeof(T), typeof(TMetadataView), contractName, ImportCardinality.ExactlyOne).SingleOrDefault<Export>();
			if (export == null)
			{
				return null;
			}
			return ExportServices.CreateStronglyTypedLazyOfTM<T, TMetadataView>(export);
		}

		private Lazy<T> GetExportCore<T>(string contractName)
		{
			Export export = this.GetExportsCore(typeof(T), null, contractName, ImportCardinality.ExactlyOne).SingleOrDefault<Export>();
			if (export == null)
			{
				return null;
			}
			return ExportServices.CreateStronglyTypedLazyOfT<T>(export);
		}

		private IEnumerable<Export> GetExportsCore(Type type, Type metadataViewType, string contractName, ImportCardinality cardinality)
		{
			Requires.NotNull<Type>(type, "type");
			if (string.IsNullOrEmpty(contractName))
			{
				contractName = AttributedModelServices.GetContractName(type);
			}
			if (metadataViewType == null)
			{
				metadataViewType = ExportServices.DefaultMetadataViewType;
			}
			if (!MetadataViewProvider.IsViewTypeValid(metadataViewType))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.InvalidMetadataView, metadataViewType.Name));
			}
			ImportDefinition importDefinition = ExportProvider.BuildImportDefinition(type, metadataViewType, contractName, cardinality);
			return this.GetExports(importDefinition, null);
		}

		private static ImportDefinition BuildImportDefinition(Type type, Type metadataViewType, string contractName, ImportCardinality cardinality)
		{
			Assumes.NotNull<Type, Type, string>(type, metadataViewType, contractName);
			IEnumerable<KeyValuePair<string, Type>> requiredMetadata = CompositionServices.GetRequiredMetadata(metadataViewType);
			IDictionary<string, object> importMetadata = CompositionServices.GetImportMetadata(type, null);
			string text = null;
			if (type != typeof(object))
			{
				text = AttributedModelServices.GetTypeIdentity(type);
			}
			return new ContractBasedImportDefinition(contractName, text, requiredMetadata, cardinality, false, true, CreationPolicy.Any, importMetadata);
		}

		public event EventHandler<ExportsChangeEventArgs> ExportsChanged;

		public event EventHandler<ExportsChangeEventArgs> ExportsChanging;

		public IEnumerable<Export> GetExports(ImportDefinition definition)
		{
			return this.GetExports(definition, null);
		}

		public IEnumerable<Export> GetExports(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			Requires.NotNull<ImportDefinition>(definition, "definition");
			IEnumerable<Export> enumerable;
			ExportCardinalityCheckResult exportCardinalityCheckResult = this.TryGetExportsCore(definition, atomicComposition, out enumerable);
			if (exportCardinalityCheckResult == ExportCardinalityCheckResult.Match)
			{
				return enumerable;
			}
			if (exportCardinalityCheckResult != ExportCardinalityCheckResult.NoExports)
			{
				Assumes.IsTrue(exportCardinalityCheckResult == ExportCardinalityCheckResult.TooManyExports);
				throw new ImportCardinalityMismatchException(string.Format(CultureInfo.CurrentCulture, Strings.CardinalityMismatch_TooManyExports, definition.ToString()));
			}
			throw new ImportCardinalityMismatchException(string.Format(CultureInfo.CurrentCulture, Strings.CardinalityMismatch_NoExports, definition.ToString()));
		}

		public bool TryGetExports(ImportDefinition definition, AtomicComposition atomicComposition, out IEnumerable<Export> exports)
		{
			Requires.NotNull<ImportDefinition>(definition, "definition");
			exports = null;
			return this.TryGetExportsCore(definition, atomicComposition, out exports) == ExportCardinalityCheckResult.Match;
		}

		protected abstract IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition);

		protected virtual void OnExportsChanged(ExportsChangeEventArgs e)
		{
			EventHandler<ExportsChangeEventArgs> exportsChanged = this.ExportsChanged;
			if (exportsChanged != null)
			{
				CompositionServices.TryFire<ExportsChangeEventArgs>(exportsChanged, this, e).ThrowOnErrors(e.AtomicComposition);
			}
		}

		protected virtual void OnExportsChanging(ExportsChangeEventArgs e)
		{
			EventHandler<ExportsChangeEventArgs> exportsChanging = this.ExportsChanging;
			if (exportsChanging != null)
			{
				CompositionServices.TryFire<ExportsChangeEventArgs>(exportsChanging, this, e).ThrowOnErrors(e.AtomicComposition);
			}
		}

		private ExportCardinalityCheckResult TryGetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition, out IEnumerable<Export> exports)
		{
			Assumes.NotNull<ImportDefinition>(definition);
			exports = this.GetExportsCore(definition, atomicComposition);
			ExportCardinalityCheckResult exportCardinalityCheckResult = ExportServices.CheckCardinality<Export>(definition, exports);
			if (exportCardinalityCheckResult == ExportCardinalityCheckResult.TooManyExports && definition.Cardinality == ImportCardinality.ZeroOrOne)
			{
				exportCardinalityCheckResult = ExportCardinalityCheckResult.Match;
				exports = null;
			}
			if (exports == null)
			{
				exports = ExportProvider.EmptyExports;
			}
			return exportCardinalityCheckResult;
		}

		private static readonly Export[] EmptyExports = new Export[0];
	}
}
