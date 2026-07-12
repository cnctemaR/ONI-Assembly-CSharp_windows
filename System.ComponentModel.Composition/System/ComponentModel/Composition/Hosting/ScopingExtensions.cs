using System;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	public static class ScopingExtensions
	{
		public static bool Exports(this ComposablePartDefinition part, string contractName)
		{
			Requires.NotNull<ComposablePartDefinition>(part, "part");
			Requires.NotNull<string>(contractName, "contractName");
			foreach (ExportDefinition exportDefinition in part.ExportDefinitions)
			{
				if (StringComparers.ContractName.Equals(contractName, exportDefinition.ContractName))
				{
					return true;
				}
			}
			return false;
		}

		public static bool Imports(this ComposablePartDefinition part, string contractName)
		{
			Requires.NotNull<ComposablePartDefinition>(part, "part");
			Requires.NotNull<string>(contractName, "contractName");
			foreach (ImportDefinition importDefinition in part.ImportDefinitions)
			{
				if (StringComparers.ContractName.Equals(contractName, importDefinition.ContractName))
				{
					return true;
				}
			}
			return false;
		}

		public static bool Imports(this ComposablePartDefinition part, string contractName, ImportCardinality importCardinality)
		{
			Requires.NotNull<ComposablePartDefinition>(part, "part");
			Requires.NotNull<string>(contractName, "contractName");
			foreach (ImportDefinition importDefinition in part.ImportDefinitions)
			{
				if (StringComparers.ContractName.Equals(contractName, importDefinition.ContractName) && importDefinition.Cardinality == importCardinality)
				{
					return true;
				}
			}
			return false;
		}

		public static bool ContainsPartMetadataWithKey(this ComposablePartDefinition part, string key)
		{
			Requires.NotNull<ComposablePartDefinition>(part, "part");
			Requires.NotNull<string>(key, "key");
			return part.Metadata.ContainsKey(key);
		}

		public static bool ContainsPartMetadata<T>(this ComposablePartDefinition part, string key, T value)
		{
			Requires.NotNull<ComposablePartDefinition>(part, "part");
			Requires.NotNull<string>(key, "key");
			object obj = null;
			if (!part.Metadata.TryGetValue(key, out obj))
			{
				return false;
			}
			if (value == null)
			{
				return obj == null;
			}
			return value.Equals(obj);
		}

		public static FilteredCatalog Filter(this ComposablePartCatalog catalog, Func<ComposablePartDefinition, bool> filter)
		{
			Requires.NotNull<ComposablePartCatalog>(catalog, "catalog");
			Requires.NotNull<Func<ComposablePartDefinition, bool>>(filter, "filter");
			return new FilteredCatalog(catalog, filter);
		}
	}
}
