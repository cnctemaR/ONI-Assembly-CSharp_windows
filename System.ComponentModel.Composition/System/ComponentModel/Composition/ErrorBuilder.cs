using System;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
	internal static class ErrorBuilder
	{
		public static CompositionError PreventedByExistingImport(ComposablePart part, ImportDefinition import)
		{
			return CompositionError.Create(CompositionErrorId.ImportEngine_PreventedByExistingImport, Strings.ImportEngine_PreventedByExistingImport, new object[]
			{
				import.ToElement().DisplayName,
				part.ToElement().DisplayName
			});
		}

		public static CompositionError InvalidStateForRecompposition(ComposablePart part)
		{
			return CompositionError.Create(CompositionErrorId.ImportEngine_InvalidStateForRecomposition, Strings.ImportEngine_InvalidStateForRecomposition, new object[] { part.ToElement().DisplayName });
		}

		public static CompositionError ComposeTookTooManyIterations(int maximumNumberOfCompositionIterations)
		{
			return CompositionError.Create(CompositionErrorId.ImportEngine_ComposeTookTooManyIterations, Strings.ImportEngine_ComposeTookTooManyIterations, new object[] { maximumNumberOfCompositionIterations });
		}

		public static CompositionError CreateImportCardinalityMismatch(ImportCardinalityMismatchException exception, ImportDefinition definition)
		{
			Assumes.NotNull<ImportCardinalityMismatchException, ImportDefinition>(exception, definition);
			CompositionErrorId compositionErrorId = CompositionErrorId.ImportEngine_ImportCardinalityMismatch;
			string message = exception.Message;
			object[] array = new object[2];
			array[0] = definition.ToElement();
			return CompositionError.Create(compositionErrorId, message, array);
		}

		public static CompositionError CreatePartCannotActivate(ComposablePart part, Exception innerException)
		{
			Assumes.NotNull<ComposablePart, Exception>(part, innerException);
			ICompositionElement compositionElement = part.ToElement();
			return CompositionError.Create(CompositionErrorId.ImportEngine_PartCannotActivate, compositionElement, innerException, Strings.ImportEngine_PartCannotActivate, new object[] { compositionElement.DisplayName });
		}

		public static CompositionError CreatePartCannotSetImport(ComposablePart part, ImportDefinition definition, Exception innerException)
		{
			Assumes.NotNull<ComposablePart, ImportDefinition, Exception>(part, definition, innerException);
			ICompositionElement compositionElement = definition.ToElement();
			return CompositionError.Create(CompositionErrorId.ImportEngine_PartCannotSetImport, compositionElement, innerException, Strings.ImportEngine_PartCannotSetImport, new object[]
			{
				compositionElement.DisplayName,
				part.ToElement().DisplayName
			});
		}

		public static CompositionError CreateCannotGetExportedValue(ComposablePart part, ExportDefinition definition, Exception innerException)
		{
			Assumes.NotNull<ComposablePart, ExportDefinition, Exception>(part, definition, innerException);
			ICompositionElement compositionElement = definition.ToElement();
			return CompositionError.Create(CompositionErrorId.ImportEngine_PartCannotGetExportedValue, compositionElement, innerException, Strings.ImportEngine_PartCannotGetExportedValue, new object[]
			{
				compositionElement.DisplayName,
				part.ToElement().DisplayName
			});
		}

		public static CompositionError CreatePartCycle(ComposablePart part)
		{
			Assumes.NotNull<ComposablePart>(part);
			ICompositionElement compositionElement = part.ToElement();
			return CompositionError.Create(CompositionErrorId.ImportEngine_PartCycle, compositionElement, Strings.ImportEngine_PartCycle, new object[] { compositionElement.DisplayName });
		}
	}
}
