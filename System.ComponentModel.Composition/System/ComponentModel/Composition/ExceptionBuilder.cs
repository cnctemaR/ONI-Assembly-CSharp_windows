using System;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
	internal static class ExceptionBuilder
	{
		public static Exception CreateDiscoveryException(string messageFormat, params string[] arguments)
		{
			return new InvalidOperationException(ExceptionBuilder.Format(messageFormat, arguments));
		}

		public static ArgumentException CreateContainsNullElement(string parameterName)
		{
			Assumes.NotNull<string>(parameterName);
			return new ArgumentException(ExceptionBuilder.Format(Strings.Argument_NullElement, new string[] { parameterName }), parameterName);
		}

		public static ObjectDisposedException CreateObjectDisposed(object instance)
		{
			Assumes.NotNull<object>(instance);
			return new ObjectDisposedException(instance.GetType().ToString());
		}

		public static NotImplementedException CreateNotOverriddenByDerived(string memberName)
		{
			Assumes.NotNullOrEmpty(memberName);
			return new NotImplementedException(ExceptionBuilder.Format(Strings.NotImplemented_NotOverriddenByDerived, new string[] { memberName }));
		}

		public static ArgumentException CreateExportDefinitionNotOnThisComposablePart(string parameterName)
		{
			Assumes.NotNullOrEmpty(parameterName);
			return new ArgumentException(ExceptionBuilder.Format(Strings.ExportDefinitionNotOnThisComposablePart, new string[] { parameterName }), parameterName);
		}

		public static ArgumentException CreateImportDefinitionNotOnThisComposablePart(string parameterName)
		{
			Assumes.NotNullOrEmpty(parameterName);
			return new ArgumentException(ExceptionBuilder.Format(Strings.ImportDefinitionNotOnThisComposablePart, new string[] { parameterName }), parameterName);
		}

		public static CompositionException CreateCannotGetExportedValue(ComposablePart part, ExportDefinition definition, Exception innerException)
		{
			Assumes.NotNull<ComposablePart, ExportDefinition, Exception>(part, definition, innerException);
			return new CompositionException(ErrorBuilder.CreateCannotGetExportedValue(part, definition, innerException));
		}

		public static ArgumentException CreateReflectionModelInvalidPartDefinition(string parameterName, Type partDefinitionType)
		{
			Assumes.NotNullOrEmpty(parameterName);
			Assumes.NotNull<Type>(partDefinitionType);
			return new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_InvalidPartDefinition, partDefinitionType), parameterName);
		}

		public static ArgumentException ExportFactory_TooManyGenericParameters(string typeName)
		{
			Assumes.NotNullOrEmpty(typeName);
			return new ArgumentException(ExceptionBuilder.Format(Strings.ExportFactory_TooManyGenericParameters, new string[] { typeName }), typeName);
		}

		private static string Format(string format, params string[] arguments)
		{
			return string.Format(CultureInfo.CurrentCulture, format, arguments);
		}
	}
}
