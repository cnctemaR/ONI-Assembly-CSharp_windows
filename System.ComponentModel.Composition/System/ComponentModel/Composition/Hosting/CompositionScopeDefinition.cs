using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	[DebuggerTypeProxy(typeof(CompositionScopeDefinitionDebuggerProxy))]
	public class CompositionScopeDefinition : ComposablePartCatalog, INotifyComposablePartCatalogChanged
	{
		protected CompositionScopeDefinition()
		{
		}

		public CompositionScopeDefinition(ComposablePartCatalog catalog, IEnumerable<CompositionScopeDefinition> children)
		{
			Requires.NotNull<ComposablePartCatalog>(catalog, "catalog");
			Requires.NullOrNotNullElements<CompositionScopeDefinition>(children, "children");
			this.InitializeCompositionScopeDefinition(catalog, children, null);
		}

		public CompositionScopeDefinition(ComposablePartCatalog catalog, IEnumerable<CompositionScopeDefinition> children, IEnumerable<ExportDefinition> publicSurface)
		{
			Requires.NotNull<ComposablePartCatalog>(catalog, "catalog");
			Requires.NullOrNotNullElements<CompositionScopeDefinition>(children, "children");
			Requires.NullOrNotNullElements<ExportDefinition>(publicSurface, "publicSurface");
			this.InitializeCompositionScopeDefinition(catalog, children, publicSurface);
		}

		private void InitializeCompositionScopeDefinition(ComposablePartCatalog catalog, IEnumerable<CompositionScopeDefinition> children, IEnumerable<ExportDefinition> publicSurface)
		{
			this._catalog = catalog;
			if (children != null)
			{
				this._children = children.ToArray<CompositionScopeDefinition>();
			}
			if (publicSurface != null)
			{
				this._publicSurface = publicSurface;
			}
			INotifyComposablePartCatalogChanged notifyComposablePartCatalogChanged = this._catalog as INotifyComposablePartCatalogChanged;
			if (notifyComposablePartCatalogChanged != null)
			{
				notifyComposablePartCatalogChanged.Changed += this.OnChangedInternal;
				notifyComposablePartCatalogChanged.Changing += this.OnChangingInternal;
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
				{
					INotifyComposablePartCatalogChanged notifyComposablePartCatalogChanged = this._catalog as INotifyComposablePartCatalogChanged;
					if (notifyComposablePartCatalogChanged != null)
					{
						notifyComposablePartCatalogChanged.Changed -= this.OnChangedInternal;
						notifyComposablePartCatalogChanged.Changing -= this.OnChangingInternal;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public virtual IEnumerable<CompositionScopeDefinition> Children
		{
			get
			{
				this.ThrowIfDisposed();
				return this._children;
			}
		}

		public virtual IEnumerable<ExportDefinition> PublicSurface
		{
			get
			{
				this.ThrowIfDisposed();
				if (this._publicSurface == null)
				{
					return this.SelectMany<ComposablePartDefinition, ExportDefinition>((ComposablePartDefinition p) => p.ExportDefinitions);
				}
				return this._publicSurface;
			}
		}

		public override IEnumerator<ComposablePartDefinition> GetEnumerator()
		{
			return this._catalog.GetEnumerator();
		}

		public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			this.ThrowIfDisposed();
			return this._catalog.GetExports(definition);
		}

		internal IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExportsFromPublicSurface(ImportDefinition definition)
		{
			Assumes.NotNull<ImportDefinition, string>(definition, "definition");
			List<Tuple<ComposablePartDefinition, ExportDefinition>> list = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
			foreach (ExportDefinition exportDefinition in this.PublicSurface)
			{
				if (definition.IsConstraintSatisfiedBy(exportDefinition))
				{
					foreach (Tuple<ComposablePartDefinition, ExportDefinition> tuple in this.GetExports(definition))
					{
						if (tuple.Item2 == exportDefinition)
						{
							list.Add(tuple);
							break;
						}
					}
				}
			}
			return list;
		}

		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

		protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
		{
			EventHandler<ComposablePartCatalogChangeEventArgs> changed = this.Changed;
			if (changed != null)
			{
				changed(this, e);
			}
		}

		protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
		{
			EventHandler<ComposablePartCatalogChangeEventArgs> changing = this.Changing;
			if (changing != null)
			{
				changing(this, e);
			}
		}

		private void OnChangedInternal(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			this.OnChanged(e);
		}

		private void OnChangingInternal(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			this.OnChanging(e);
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed == 1)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private ComposablePartCatalog _catalog;

		private IEnumerable<ExportDefinition> _publicSurface;

		private IEnumerable<CompositionScopeDefinition> _children = Enumerable.Empty<CompositionScopeDefinition>();

		private volatile int _isDisposed;
	}
}
