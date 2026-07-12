using System;
using System.Collections.ObjectModel;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Primitives
{
	internal class ComposablePartCatalogDebuggerProxy
	{
		public ComposablePartCatalogDebuggerProxy(ComposablePartCatalog catalog)
		{
			Requires.NotNull<ComposablePartCatalog>(catalog, "catalog");
			this._catalog = catalog;
		}

		public ReadOnlyCollection<ComposablePartDefinition> Parts
		{
			get
			{
				return this._catalog.Parts.ToReadOnlyCollection<ComposablePartDefinition>();
			}
		}

		private readonly ComposablePartCatalog _catalog;
	}
}
