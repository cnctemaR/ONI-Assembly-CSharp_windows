using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	public class AggregateExportProvider : ExportProvider, IDisposable
	{
		public AggregateExportProvider(params ExportProvider[] providers)
		{
			ExportProvider[] array;
			if (providers != null)
			{
				array = new ExportProvider[providers.Length];
				for (int i = 0; i < providers.Length; i++)
				{
					ExportProvider exportProvider = providers[i];
					if (exportProvider == null)
					{
						throw ExceptionBuilder.CreateContainsNullElement("providers");
					}
					array[i] = exportProvider;
					exportProvider.ExportsChanged += this.OnExportChangedInternal;
					exportProvider.ExportsChanging += this.OnExportChangingInternal;
				}
			}
			else
			{
				array = new ExportProvider[0];
			}
			this._providers = array;
			this._readOnlyProviders = new ReadOnlyCollection<ExportProvider>(this._providers);
		}

		public AggregateExportProvider(IEnumerable<ExportProvider> providers)
			: this((providers != null) ? providers.AsArray<ExportProvider>() : null)
		{
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
			{
				foreach (ExportProvider exportProvider in this._providers)
				{
					exportProvider.ExportsChanged -= this.OnExportChangedInternal;
					exportProvider.ExportsChanging -= this.OnExportChangingInternal;
				}
			}
		}

		public ReadOnlyCollection<ExportProvider> Providers
		{
			get
			{
				this.ThrowIfDisposed();
				return this._readOnlyProviders;
			}
		}

		protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			this.ThrowIfDisposed();
			ExportProvider[] array;
			if (definition.Cardinality == ImportCardinality.ZeroOrMore)
			{
				List<Export> list = new List<Export>();
				array = this._providers;
				for (int i = 0; i < array.Length; i++)
				{
					foreach (Export export in array[i].GetExports(definition, atomicComposition))
					{
						list.Add(export);
					}
				}
				return list;
			}
			IEnumerable<Export> enumerable = null;
			array = this._providers;
			for (int i = 0; i < array.Length; i++)
			{
				IEnumerable<Export> enumerable2;
				bool flag = array[i].TryGetExports(definition, atomicComposition, out enumerable2);
				bool flag2 = enumerable2.FastAny<Export>();
				if (flag && flag2)
				{
					return enumerable2;
				}
				if (flag2)
				{
					enumerable = ((enumerable != null) ? enumerable.Concat<Export>(enumerable2) : enumerable2);
				}
			}
			return enumerable;
		}

		private void OnExportChangedInternal(object sender, ExportsChangeEventArgs e)
		{
			this.OnExportsChanged(e);
		}

		private void OnExportChangingInternal(object sender, ExportsChangeEventArgs e)
		{
			this.OnExportsChanging(e);
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed == 1)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private readonly ReadOnlyCollection<ExportProvider> _readOnlyProviders;

		private readonly ExportProvider[] _providers;

		private volatile int _isDisposed;
	}
}
