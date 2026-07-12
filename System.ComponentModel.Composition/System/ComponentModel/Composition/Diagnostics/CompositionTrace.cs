using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Diagnostics
{
	internal static class CompositionTrace
	{
		internal static void PartDefinitionResurrected(ComposablePartDefinition definition)
		{
			Assumes.NotNull<ComposablePartDefinition>(definition);
			if (CompositionTraceSource.CanWriteInformation)
			{
				CompositionTraceSource.WriteInformation(CompositionTraceId.Rejection_DefinitionResurrected, Strings.CompositionTrace_Rejection_DefinitionResurrected, new object[] { definition.GetDisplayName() });
			}
		}

		internal static void PartDefinitionRejected(ComposablePartDefinition definition, ChangeRejectedException exception)
		{
			Assumes.NotNull<ComposablePartDefinition, ChangeRejectedException>(definition, exception);
			if (CompositionTraceSource.CanWriteWarning)
			{
				CompositionTraceSource.WriteWarning(CompositionTraceId.Rejection_DefinitionRejected, Strings.CompositionTrace_Rejection_DefinitionRejected, new object[]
				{
					definition.GetDisplayName(),
					exception.Message
				});
			}
		}

		internal static void AssemblyLoadFailed(DirectoryCatalog catalog, string fileName, Exception exception)
		{
			Assumes.NotNull<DirectoryCatalog, Exception>(catalog, exception);
			Assumes.NotNullOrEmpty(fileName);
			if (CompositionTraceSource.CanWriteWarning)
			{
				CompositionTraceSource.WriteWarning(CompositionTraceId.Discovery_AssemblyLoadFailed, Strings.CompositionTrace_Discovery_AssemblyLoadFailed, new object[]
				{
					catalog.GetDisplayName(),
					fileName,
					exception.Message
				});
			}
		}

		internal static void DefinitionMarkedWithPartNotDiscoverableAttribute(Type type)
		{
			Assumes.NotNull<Type>(type);
			if (CompositionTraceSource.CanWriteInformation)
			{
				CompositionTraceSource.WriteInformation(CompositionTraceId.Discovery_DefinitionMarkedWithPartNotDiscoverableAttribute, Strings.CompositionTrace_Discovery_DefinitionMarkedWithPartNotDiscoverableAttribute, new object[] { type.GetDisplayName() });
			}
		}

		internal static void DefinitionMismatchedExportArity(Type type, MemberInfo member)
		{
			Assumes.NotNull<Type>(type);
			Assumes.NotNull<MemberInfo>(member);
			if (CompositionTraceSource.CanWriteInformation)
			{
				CompositionTraceSource.WriteInformation(CompositionTraceId.Discovery_DefinitionMismatchedExportArity, Strings.CompositionTrace_Discovery_DefinitionMismatchedExportArity, new object[]
				{
					type.GetDisplayName(),
					member.GetDisplayName()
				});
			}
		}

		internal static void DefinitionContainsNoExports(Type type)
		{
			Assumes.NotNull<Type>(type);
			if (CompositionTraceSource.CanWriteInformation)
			{
				CompositionTraceSource.WriteInformation(CompositionTraceId.Discovery_DefinitionContainsNoExports, Strings.CompositionTrace_Discovery_DefinitionContainsNoExports, new object[] { type.GetDisplayName() });
			}
		}

		internal static void MemberMarkedWithMultipleImportAndImportMany(ReflectionItem item)
		{
			Assumes.NotNull<ReflectionItem>(item);
			if (CompositionTraceSource.CanWriteError)
			{
				CompositionTraceSource.WriteError(CompositionTraceId.Discovery_MemberMarkedWithMultipleImportAndImportMany, Strings.CompositionTrace_Discovery_MemberMarkedWithMultipleImportAndImportMany, new object[] { item.GetDisplayName() });
			}
		}
	}
}
