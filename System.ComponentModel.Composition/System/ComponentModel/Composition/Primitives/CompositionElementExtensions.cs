using System;

namespace System.ComponentModel.Composition.Primitives
{
	internal static class CompositionElementExtensions
	{
		public static ICompositionElement ToSerializableElement(this ICompositionElement element)
		{
			return SerializableCompositionElement.FromICompositionElement(element);
		}

		public static ICompositionElement ToElement(this Export export)
		{
			ICompositionElement compositionElement = export as ICompositionElement;
			if (compositionElement != null)
			{
				return compositionElement;
			}
			return export.Definition.ToElement();
		}

		public static ICompositionElement ToElement(this ExportDefinition definition)
		{
			return CompositionElementExtensions.ToElementCore(definition);
		}

		public static ICompositionElement ToElement(this ImportDefinition definition)
		{
			return CompositionElementExtensions.ToElementCore(definition);
		}

		public static ICompositionElement ToElement(this ComposablePart part)
		{
			return CompositionElementExtensions.ToElementCore(part);
		}

		public static ICompositionElement ToElement(this ComposablePartDefinition definition)
		{
			return CompositionElementExtensions.ToElementCore(definition);
		}

		public static string GetDisplayName(this ComposablePartDefinition definition)
		{
			return CompositionElementExtensions.GetDisplayNameCore(definition);
		}

		public static string GetDisplayName(this ComposablePartCatalog catalog)
		{
			return CompositionElementExtensions.GetDisplayNameCore(catalog);
		}

		private static string GetDisplayNameCore(object value)
		{
			ICompositionElement compositionElement = value as ICompositionElement;
			if (compositionElement != null)
			{
				return compositionElement.DisplayName;
			}
			return value.ToString();
		}

		private static ICompositionElement ToElementCore(object value)
		{
			ICompositionElement compositionElement = value as ICompositionElement;
			if (compositionElement != null)
			{
				return compositionElement;
			}
			return new CompositionElement(value);
		}
	}
}
