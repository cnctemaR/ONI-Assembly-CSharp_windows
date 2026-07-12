using System;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
	public class ExportFactory<T>
	{
		public ExportFactory(Func<Tuple<T, Action>> exportLifetimeContextCreator)
		{
			if (exportLifetimeContextCreator == null)
			{
				throw new ArgumentNullException("exportLifetimeContextCreator");
			}
			this._exportLifetimeContextCreator = exportLifetimeContextCreator;
		}

		public ExportLifetimeContext<T> CreateExport()
		{
			Tuple<T, Action> tuple = this._exportLifetimeContextCreator();
			return new ExportLifetimeContext<T>(tuple.Item1, tuple.Item2);
		}

		internal bool IncludeInScopedCatalog(ComposablePartDefinition composablePartDefinition)
		{
			return this.OnFilterScopedCatalog(composablePartDefinition);
		}

		protected virtual bool OnFilterScopedCatalog(ComposablePartDefinition composablePartDefinition)
		{
			return true;
		}

		private Func<Tuple<T, Action>> _exportLifetimeContextCreator;
	}
}
