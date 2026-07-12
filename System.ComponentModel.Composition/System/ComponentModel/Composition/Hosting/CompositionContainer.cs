using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	public class CompositionContainer : ExportProvider, ICompositionService, IDisposable
	{
		public CompositionContainer()
			: this(null, Array.Empty<ExportProvider>())
		{
		}

		public CompositionContainer(params ExportProvider[] providers)
			: this(null, providers)
		{
		}

		public CompositionContainer(CompositionOptions compositionOptions, params ExportProvider[] providers)
			: this(null, compositionOptions, providers)
		{
		}

		public CompositionContainer(ComposablePartCatalog catalog, params ExportProvider[] providers)
			: this(catalog, false, providers)
		{
		}

		public CompositionContainer(ComposablePartCatalog catalog, bool isThreadSafe, params ExportProvider[] providers)
			: this(catalog, isThreadSafe ? CompositionOptions.IsThreadSafe : CompositionOptions.Default, providers)
		{
		}

		public CompositionContainer(ComposablePartCatalog catalog, CompositionOptions compositionOptions, params ExportProvider[] providers)
		{
			if (compositionOptions > (CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe | CompositionOptions.ExportCompositionService))
			{
				throw new ArgumentOutOfRangeException("compositionOptions");
			}
			this._compositionOptions = compositionOptions;
			this._partExportProvider = new ComposablePartExportProvider(compositionOptions);
			this._partExportProvider.SourceProvider = this;
			if (catalog != null || providers.Length != 0)
			{
				if (catalog != null)
				{
					this._catalogExportProvider = new CatalogExportProvider(catalog, compositionOptions);
					this._catalogExportProvider.SourceProvider = this;
					this._localExportProvider = new AggregateExportProvider(new ExportProvider[] { this._partExportProvider, this._catalogExportProvider });
				}
				else
				{
					this._localExportProvider = new AggregateExportProvider(new ExportProvider[] { this._partExportProvider });
				}
				if (providers != null && providers.Length != 0)
				{
					this._ancestorExportProvider = new AggregateExportProvider(providers);
					this._rootProvider = new AggregateExportProvider(new ExportProvider[] { this._localExportProvider, this._ancestorExportProvider });
				}
				else
				{
					this._rootProvider = this._localExportProvider;
				}
			}
			else
			{
				this._rootProvider = this._partExportProvider;
			}
			if (compositionOptions.HasFlag(CompositionOptions.ExportCompositionService))
			{
				this.ComposeExportedValue(new CompositionContainer.CompositionServiceShim(this));
			}
			this._rootProvider.ExportsChanged += this.OnExportsChangedInternal;
			this._rootProvider.ExportsChanging += this.OnExportsChangingInternal;
			this._providers = ((providers != null) ? new ReadOnlyCollection<ExportProvider>((ExportProvider[])providers.Clone()) : CompositionContainer.EmptyProviders);
		}

		internal CompositionOptions CompositionOptions
		{
			get
			{
				this.ThrowIfDisposed();
				return this._compositionOptions;
			}
		}

		public ComposablePartCatalog Catalog
		{
			get
			{
				this.ThrowIfDisposed();
				if (this._catalogExportProvider == null)
				{
					return null;
				}
				return this._catalogExportProvider.Catalog;
			}
		}

		internal CatalogExportProvider CatalogExportProvider
		{
			get
			{
				this.ThrowIfDisposed();
				return this._catalogExportProvider;
			}
		}

		public ReadOnlyCollection<ExportProvider> Providers
		{
			get
			{
				this.ThrowIfDisposed();
				return this._providers;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this._isDisposed)
			{
				ExportProvider exportProvider = null;
				AggregateExportProvider aggregateExportProvider = null;
				AggregateExportProvider aggregateExportProvider2 = null;
				ComposablePartExportProvider composablePartExportProvider = null;
				CatalogExportProvider catalogExportProvider = null;
				ImportEngine importEngine = null;
				object @lock = this._lock;
				lock (@lock)
				{
					if (!this._isDisposed)
					{
						exportProvider = this._rootProvider;
						this._rootProvider = null;
						aggregateExportProvider2 = this._localExportProvider;
						this._localExportProvider = null;
						aggregateExportProvider = this._ancestorExportProvider;
						this._ancestorExportProvider = null;
						composablePartExportProvider = this._partExportProvider;
						this._partExportProvider = null;
						catalogExportProvider = this._catalogExportProvider;
						this._catalogExportProvider = null;
						importEngine = this._importEngine;
						this._importEngine = null;
						this._isDisposed = true;
					}
				}
				if (exportProvider != null)
				{
					exportProvider.ExportsChanged -= this.OnExportsChangedInternal;
					exportProvider.ExportsChanging -= this.OnExportsChangingInternal;
				}
				if (aggregateExportProvider != null)
				{
					aggregateExportProvider.Dispose();
				}
				if (aggregateExportProvider2 != null)
				{
					aggregateExportProvider2.Dispose();
				}
				if (catalogExportProvider != null)
				{
					catalogExportProvider.Dispose();
				}
				if (composablePartExportProvider != null)
				{
					composablePartExportProvider.Dispose();
				}
				if (importEngine != null)
				{
					importEngine.Dispose();
				}
			}
		}

		public void Compose(CompositionBatch batch)
		{
			Requires.NotNull<CompositionBatch>(batch, "batch");
			this.ThrowIfDisposed();
			this._partExportProvider.Compose(batch);
		}

		public void ReleaseExport(Export export)
		{
			Requires.NotNull<Export>(export, "export");
			IDisposable disposable = export as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		public void ReleaseExport<T>(Lazy<T> export)
		{
			Requires.NotNull<Lazy<T>>(export, "export");
			IDisposable disposable = export as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		public void ReleaseExports(IEnumerable<Export> exports)
		{
			Requires.NotNullOrNullElements<Export>(exports, "exports");
			foreach (Export export in exports)
			{
				this.ReleaseExport(export);
			}
		}

		public void ReleaseExports<T>(IEnumerable<Lazy<T>> exports)
		{
			Requires.NotNullOrNullElements<Lazy<T>>(exports, "exports");
			foreach (Lazy<T> lazy in exports)
			{
				this.ReleaseExport<T>(lazy);
			}
		}

		public void ReleaseExports<T, TMetadataView>(IEnumerable<Lazy<T, TMetadataView>> exports)
		{
			Requires.NotNullOrNullElements<Lazy<T, TMetadataView>>(exports, "exports");
			foreach (Lazy<T, TMetadataView> lazy in exports)
			{
				this.ReleaseExport<T>(lazy);
			}
		}

		public void SatisfyImportsOnce(ComposablePart part)
		{
			this.ThrowIfDisposed();
			if (this._importEngine == null)
			{
				ImportEngine importEngine = new ImportEngine(this, this._compositionOptions);
				object @lock = this._lock;
				lock (@lock)
				{
					if (this._importEngine == null)
					{
						Thread.MemoryBarrier();
						this._importEngine = importEngine;
						importEngine = null;
					}
				}
				if (importEngine != null)
				{
					importEngine.Dispose();
				}
			}
			this._importEngine.SatisfyImportsOnce(part);
		}

		internal void OnExportsChangedInternal(object sender, ExportsChangeEventArgs e)
		{
			this.OnExportsChanged(e);
		}

		internal void OnExportsChangingInternal(object sender, ExportsChangeEventArgs e)
		{
			this.OnExportsChanging(e);
		}

		protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			this.ThrowIfDisposed();
			IEnumerable<Export> enumerable = null;
			object obj;
			if (!definition.Metadata.TryGetValue("System.ComponentModel.Composition.ImportSource", out obj))
			{
				obj = ImportSource.Any;
			}
			switch ((ImportSource)obj)
			{
			case ImportSource.Any:
				Assumes.NotNull<ExportProvider>(this._rootProvider);
				this._rootProvider.TryGetExports(definition, atomicComposition, out enumerable);
				break;
			case ImportSource.Local:
				Assumes.NotNull<AggregateExportProvider>(this._localExportProvider);
				this._localExportProvider.TryGetExports(definition.RemoveImportSource(), atomicComposition, out enumerable);
				break;
			case ImportSource.NonLocal:
				if (this._ancestorExportProvider != null)
				{
					this._ancestorExportProvider.TryGetExports(definition.RemoveImportSource(), atomicComposition, out enumerable);
				}
				break;
			}
			return enumerable;
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private CompositionOptions _compositionOptions;

		private ImportEngine _importEngine;

		private ComposablePartExportProvider _partExportProvider;

		private ExportProvider _rootProvider;

		private CatalogExportProvider _catalogExportProvider;

		private AggregateExportProvider _localExportProvider;

		private AggregateExportProvider _ancestorExportProvider;

		private readonly ReadOnlyCollection<ExportProvider> _providers;

		private volatile bool _isDisposed;

		private object _lock = new object();

		private static ReadOnlyCollection<ExportProvider> EmptyProviders = new ReadOnlyCollection<ExportProvider>(new ExportProvider[0]);

		private class CompositionServiceShim : ICompositionService
		{
			public CompositionServiceShim(CompositionContainer innerContainer)
			{
				Assumes.NotNull<CompositionContainer>(innerContainer);
				this._innerContainer = innerContainer;
			}

			void ICompositionService.SatisfyImportsOnce(ComposablePart part)
			{
				this._innerContainer.SatisfyImportsOnce(part);
			}

			private CompositionContainer _innerContainer;
		}
	}
}
