using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
	public static class AttributedModelServices
	{
		public static TMetadataView GetMetadataView<TMetadataView>(IDictionary<string, object> metadata)
		{
			Requires.NotNull<IDictionary<string, object>>(metadata, "metadata");
			return MetadataViewProvider.GetMetadataView<TMetadataView>(metadata);
		}

		public static ComposablePart CreatePart(object attributedPart)
		{
			Requires.NotNull<object>(attributedPart, "attributedPart");
			return AttributedModelDiscovery.CreatePart(attributedPart);
		}

		public static ComposablePart CreatePart(object attributedPart, ReflectionContext reflectionContext)
		{
			Requires.NotNull<object>(attributedPart, "attributedPart");
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			return AttributedModelDiscovery.CreatePart(attributedPart, reflectionContext);
		}

		public static ComposablePart CreatePart(ComposablePartDefinition partDefinition, object attributedPart)
		{
			Requires.NotNull<ComposablePartDefinition>(partDefinition, "partDefinition");
			Requires.NotNull<object>(attributedPart, "attributedPart");
			ReflectionComposablePartDefinition reflectionComposablePartDefinition = partDefinition as ReflectionComposablePartDefinition;
			if (reflectionComposablePartDefinition == null)
			{
				throw ExceptionBuilder.CreateReflectionModelInvalidPartDefinition("partDefinition", partDefinition.GetType());
			}
			return AttributedModelDiscovery.CreatePart(reflectionComposablePartDefinition, attributedPart);
		}

		public static ComposablePartDefinition CreatePartDefinition(Type type, ICompositionElement origin)
		{
			Requires.NotNull<Type>(type, "type");
			return AttributedModelServices.CreatePartDefinition(type, origin, false);
		}

		public static ComposablePartDefinition CreatePartDefinition(Type type, ICompositionElement origin, bool ensureIsDiscoverable)
		{
			Requires.NotNull<Type>(type, "type");
			if (ensureIsDiscoverable)
			{
				return AttributedModelDiscovery.CreatePartDefinitionIfDiscoverable(type, origin);
			}
			return AttributedModelDiscovery.CreatePartDefinition(type, null, false, origin);
		}

		public static string GetTypeIdentity(Type type)
		{
			Requires.NotNull<Type>(type, "type");
			return ContractNameServices.GetTypeIdentity(type);
		}

		public static string GetTypeIdentity(MethodInfo method)
		{
			Requires.NotNull<MethodInfo>(method, "method");
			return ContractNameServices.GetTypeIdentityFromMethod(method);
		}

		public static string GetContractName(Type type)
		{
			Requires.NotNull<Type>(type, "type");
			return AttributedModelServices.GetTypeIdentity(type);
		}

		public static ComposablePart AddExportedValue<T>(this CompositionBatch batch, T exportedValue)
		{
			Requires.NotNull<CompositionBatch>(batch, "batch");
			string contractName = AttributedModelServices.GetContractName(typeof(T));
			return batch.AddExportedValue(contractName, exportedValue);
		}

		public static void ComposeExportedValue<T>(this CompositionContainer container, T exportedValue)
		{
			Requires.NotNull<CompositionContainer>(container, "container");
			CompositionBatch compositionBatch = new CompositionBatch();
			compositionBatch.AddExportedValue(exportedValue);
			container.Compose(compositionBatch);
		}

		public static ComposablePart AddExportedValue<T>(this CompositionBatch batch, string contractName, T exportedValue)
		{
			Requires.NotNull<CompositionBatch>(batch, "batch");
			string typeIdentity = AttributedModelServices.GetTypeIdentity(typeof(T));
			return batch.AddExport(new Export(contractName, new Dictionary<string, object> { { "ExportTypeIdentity", typeIdentity } }, () => exportedValue));
		}

		public static void ComposeExportedValue<T>(this CompositionContainer container, string contractName, T exportedValue)
		{
			Requires.NotNull<CompositionContainer>(container, "container");
			CompositionBatch compositionBatch = new CompositionBatch();
			compositionBatch.AddExportedValue(contractName, exportedValue);
			container.Compose(compositionBatch);
		}

		public static ComposablePart AddPart(this CompositionBatch batch, object attributedPart)
		{
			Requires.NotNull<CompositionBatch>(batch, "batch");
			Requires.NotNull<object>(attributedPart, "attributedPart");
			ComposablePart composablePart = AttributedModelServices.CreatePart(attributedPart);
			batch.AddPart(composablePart);
			return composablePart;
		}

		public static void ComposeParts(this CompositionContainer container, params object[] attributedParts)
		{
			Requires.NotNull<CompositionContainer>(container, "container");
			Requires.NotNullOrNullElements<object>(attributedParts, "attributedParts");
			CompositionBatch compositionBatch = new CompositionBatch(attributedParts.Select<object, ComposablePart>((object attributedPart) => AttributedModelServices.CreatePart(attributedPart)).ToArray<ComposablePart>(), Enumerable.Empty<ComposablePart>());
			container.Compose(compositionBatch);
		}

		public static ComposablePart SatisfyImportsOnce(this ICompositionService compositionService, object attributedPart)
		{
			Requires.NotNull<ICompositionService>(compositionService, "compositionService");
			Requires.NotNull<object>(attributedPart, "attributedPart");
			ComposablePart composablePart = AttributedModelServices.CreatePart(attributedPart);
			compositionService.SatisfyImportsOnce(composablePart);
			return composablePart;
		}

		public static ComposablePart SatisfyImportsOnce(this ICompositionService compositionService, object attributedPart, ReflectionContext reflectionContext)
		{
			Requires.NotNull<ICompositionService>(compositionService, "compositionService");
			Requires.NotNull<object>(attributedPart, "attributedPart");
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			ComposablePart composablePart = AttributedModelServices.CreatePart(attributedPart, reflectionContext);
			compositionService.SatisfyImportsOnce(composablePart);
			return composablePart;
		}

		public static bool Exports(this ComposablePartDefinition part, Type contractType)
		{
			Requires.NotNull<ComposablePartDefinition>(part, "part");
			Requires.NotNull<Type>(contractType, "contractType");
			return part.Exports(AttributedModelServices.GetContractName(contractType));
		}

		public static bool Exports<T>(this ComposablePartDefinition part)
		{
			Requires.NotNull<ComposablePartDefinition>(part, "part");
			return part.Exports(typeof(T));
		}

		public static bool Imports(this ComposablePartDefinition part, Type contractType)
		{
			Requires.NotNull<ComposablePartDefinition>(part, "part");
			Requires.NotNull<Type>(contractType, "contractType");
			return part.Imports(AttributedModelServices.GetContractName(contractType));
		}

		public static bool Imports<T>(this ComposablePartDefinition part)
		{
			Requires.NotNull<ComposablePartDefinition>(part, "part");
			return part.Imports(typeof(T));
		}

		public static bool Imports(this ComposablePartDefinition part, Type contractType, ImportCardinality importCardinality)
		{
			Requires.NotNull<ComposablePartDefinition>(part, "part");
			Requires.NotNull<Type>(contractType, "contractType");
			return part.Imports(AttributedModelServices.GetContractName(contractType), importCardinality);
		}

		public static bool Imports<T>(this ComposablePartDefinition part, ImportCardinality importCardinality)
		{
			Requires.NotNull<ComposablePartDefinition>(part, "part");
			return part.Imports(typeof(T), importCardinality);
		}
	}
}
