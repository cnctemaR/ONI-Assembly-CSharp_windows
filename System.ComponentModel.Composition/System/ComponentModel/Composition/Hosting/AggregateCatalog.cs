using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	public class AggregateCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
	{
		public AggregateCatalog()
			: this(null)
		{
		}

		public AggregateCatalog(params ComposablePartCatalog[] catalogs)
			: this(catalogs)
		{
		}

		public AggregateCatalog(IEnumerable<ComposablePartCatalog> catalogs)
		{
			Requires.NullOrNotNullElements<ComposablePartCatalog>(catalogs, "catalogs");
			this._catalogs = new ComposablePartCatalogCollection(catalogs, new Action<ComposablePartCatalogChangeEventArgs>(this.OnChanged), new Action<ComposablePartCatalogChangeEventArgs>(this.OnChanging));
		}

		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
		{
			add
			{
				this._catalogs.Changed += value;
			}
			remove
			{
				this._catalogs.Changed -= value;
			}
		}

		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
		{
			add
			{
				this._catalogs.Changing += value;
			}
			remove
			{
				this._catalogs.Changing -= value;
			}
		}

		public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ImportDefinition>(definition, "definition");
			List<Tuple<ComposablePartDefinition, ExportDefinition>> list = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
			foreach (ComposablePartCatalog composablePartCatalog in this._catalogs)
			{
				foreach (Tuple<ComposablePartDefinition, ExportDefinition> tuple in composablePartCatalog.GetExports(definition))
				{
					list.Add(tuple);
				}
			}
			return list;
		}

		public ICollection<ComposablePartCatalog> Catalogs
		{
			get
			{
				this.ThrowIfDisposed();
				return this._catalogs;
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
				{
					this._catalogs.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override IEnumerator<ComposablePartDefinition> GetEnumerator()
		{
			return this._catalogs.SelectMany<ComposablePartCatalog, ComposablePartDefinition>((ComposablePartCatalog catalog) => catalog).GetEnumerator();
		}

		protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
		{
			this._catalogs.OnChanged(this, e);
		}

		protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
		{
			this._catalogs.OnChanging(this, e);
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed == 1)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private ComposablePartCatalogCollection _catalogs;

		private volatile int _isDisposed;
	}
}
