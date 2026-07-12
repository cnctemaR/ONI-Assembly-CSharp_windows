using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	internal class AssemblyCatalogDebuggerProxy
	{
		public AssemblyCatalogDebuggerProxy(AssemblyCatalog catalog)
		{
			Requires.NotNull<AssemblyCatalog>(catalog, "catalog");
			this._catalog = catalog;
		}

		public Assembly Assembly
		{
			get
			{
				return this._catalog.Assembly;
			}
		}

		public ReadOnlyCollection<ComposablePartDefinition> Parts
		{
			get
			{
				return this._catalog.Parts.ToReadOnlyCollection<ComposablePartDefinition>();
			}
		}

		private readonly AssemblyCatalog _catalog;
	}
}
