using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	public class ComposablePartExportProvider : ExportProvider, IDisposable
	{
		public ComposablePartExportProvider()
			: this(false)
		{
		}

		public ComposablePartExportProvider(bool isThreadSafe)
			: this(isThreadSafe ? CompositionOptions.IsThreadSafe : CompositionOptions.Default)
		{
		}

		public ComposablePartExportProvider(CompositionOptions compositionOptions)
		{
			if (compositionOptions > (CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe | CompositionOptions.ExportCompositionService))
			{
				throw new ArgumentOutOfRangeException("compositionOptions");
			}
			this._compositionOptions = compositionOptions;
			this._lock = new CompositionLock(compositionOptions.HasFlag(CompositionOptions.IsThreadSafe));
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
				bool flag = false;
				ImportEngine importEngine = null;
				try
				{
					using (this._lock.LockStateForWrite())
					{
						if (!this._isDisposed)
						{
							importEngine = this._importEngine;
							this._importEngine = null;
							this._sourceProvider = null;
							this._isDisposed = true;
							flag = true;
						}
					}
				}
				finally
				{
					if (importEngine != null)
					{
						importEngine.Dispose();
					}
					if (flag)
					{
						this._lock.Dispose();
					}
				}
			}
		}

		public ExportProvider SourceProvider
		{
			get
			{
				this.ThrowIfDisposed();
				return this._sourceProvider;
			}
			set
			{
				this.ThrowIfDisposed();
				Requires.NotNull<ExportProvider>(value, "value");
				using (this._lock.LockStateForWrite())
				{
					this.EnsureCanSet<ExportProvider>(this._sourceProvider);
					this._sourceProvider = value;
				}
			}
		}

		private ImportEngine ImportEngine
		{
			get
			{
				if (this._importEngine == null)
				{
					Assumes.NotNull<ExportProvider>(this._sourceProvider);
					ImportEngine importEngine = new ImportEngine(this._sourceProvider, this._compositionOptions);
					using (this._lock.LockStateForWrite())
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
				return this._importEngine;
			}
		}

		protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			this.ThrowIfDisposed();
			this.EnsureRunning();
			List<ComposablePart> list = null;
			using (this._lock.LockStateForRead())
			{
				list = atomicComposition.GetValueAllowNull(this, this._parts);
			}
			if (list.Count == 0)
			{
				return null;
			}
			List<Export> list2 = new List<Export>();
			foreach (ComposablePart composablePart in list)
			{
				foreach (ExportDefinition exportDefinition in composablePart.ExportDefinitions)
				{
					if (definition.IsConstraintSatisfiedBy(exportDefinition))
					{
						list2.Add(this.CreateExport(composablePart, exportDefinition));
					}
				}
			}
			return list2;
		}

		public void Compose(CompositionBatch batch)
		{
			this.ThrowIfDisposed();
			this.EnsureRunning();
			Requires.NotNull<CompositionBatch>(batch, "batch");
			if (batch.PartsToAdd.Count == 0 && batch.PartsToRemove.Count == 0)
			{
				return;
			}
			CompositionResult compositionResult = CompositionResult.SucceededResult;
			List<ComposablePart> updatedPartsList = this.GetUpdatedPartsList(ref batch);
			using (AtomicComposition atomicComposition = new AtomicComposition())
			{
				if (this._currentlyComposing)
				{
					throw new InvalidOperationException(Strings.ReentrantCompose);
				}
				this._currentlyComposing = true;
				try
				{
					atomicComposition.SetValue(this, updatedPartsList);
					this.Recompose(batch, atomicComposition);
					foreach (ComposablePart composablePart in batch.PartsToAdd)
					{
						try
						{
							this.ImportEngine.PreviewImports(composablePart, atomicComposition);
						}
						catch (ChangeRejectedException ex)
						{
							compositionResult = compositionResult.MergeResult(new CompositionResult(ex.Errors));
						}
					}
					compositionResult.ThrowOnErrors(atomicComposition);
					using (this._lock.LockStateForWrite())
					{
						this._parts = updatedPartsList;
					}
					atomicComposition.Complete();
				}
				finally
				{
					this._currentlyComposing = false;
				}
			}
			using (IEnumerator<ComposablePart> enumerator = batch.PartsToAdd.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ComposablePart part = enumerator.Current;
					compositionResult = compositionResult.MergeResult(CompositionServices.TryInvoke(delegate
					{
						this.ImportEngine.SatisfyImports(part);
					}));
				}
			}
			compositionResult.ThrowOnErrors();
		}

		private List<ComposablePart> GetUpdatedPartsList(ref CompositionBatch batch)
		{
			Assumes.NotNull<CompositionBatch>(batch);
			List<ComposablePart> list = null;
			using (this._lock.LockStateForRead())
			{
				list = this._parts.ToList<ComposablePart>();
			}
			foreach (ComposablePart composablePart in batch.PartsToAdd)
			{
				list.Add(composablePart);
			}
			List<ComposablePart> list2 = null;
			foreach (ComposablePart composablePart2 in batch.PartsToRemove)
			{
				if (list.Remove(composablePart2))
				{
					if (list2 == null)
					{
						list2 = new List<ComposablePart>();
					}
					list2.Add(composablePart2);
				}
			}
			batch = new CompositionBatch(batch.PartsToAdd, list2);
			return list;
		}

		private void Recompose(CompositionBatch batch, AtomicComposition atomicComposition)
		{
			ComposablePartExportProvider.<>c__DisplayClass21_0 CS$<>8__locals1 = new ComposablePartExportProvider.<>c__DisplayClass21_0();
			CS$<>8__locals1.<>4__this = this;
			Assumes.NotNull<CompositionBatch>(batch);
			foreach (ComposablePart composablePart in batch.PartsToRemove)
			{
				this.ImportEngine.ReleaseImports(composablePart, atomicComposition);
			}
			ComposablePartExportProvider.<>c__DisplayClass21_0 CS$<>8__locals2 = CS$<>8__locals1;
			IEnumerable<ExportDefinition> enumerable;
			if (batch.PartsToAdd.Count == 0)
			{
				enumerable = new ExportDefinition[0];
			}
			else
			{
				enumerable = batch.PartsToAdd.SelectMany<ComposablePart, ExportDefinition>((ComposablePart part) => part.ExportDefinitions).ToArray<ExportDefinition>();
			}
			CS$<>8__locals2.addedExports = enumerable;
			ComposablePartExportProvider.<>c__DisplayClass21_0 CS$<>8__locals3 = CS$<>8__locals1;
			IEnumerable<ExportDefinition> enumerable2;
			if (batch.PartsToRemove.Count == 0)
			{
				enumerable2 = new ExportDefinition[0];
			}
			else
			{
				enumerable2 = batch.PartsToRemove.SelectMany<ComposablePart, ExportDefinition>((ComposablePart part) => part.ExportDefinitions).ToArray<ExportDefinition>();
			}
			CS$<>8__locals3.removedExports = enumerable2;
			this.OnExportsChanging(new ExportsChangeEventArgs(CS$<>8__locals1.addedExports, CS$<>8__locals1.removedExports, atomicComposition));
			atomicComposition.AddCompleteAction(delegate
			{
				CS$<>8__locals1.<>4__this.OnExportsChanged(new ExportsChangeEventArgs(CS$<>8__locals1.addedExports, CS$<>8__locals1.removedExports, null));
			});
		}

		private Export CreateExport(ComposablePart part, ExportDefinition export)
		{
			return new Export(export, () => this.GetExportedValue(part, export));
		}

		private object GetExportedValue(ComposablePart part, ExportDefinition export)
		{
			this.ThrowIfDisposed();
			this.EnsureRunning();
			return CompositionServices.GetExportedValueFromComposedPart(this.ImportEngine, part, export);
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
		}

		[DebuggerStepThrough]
		private void EnsureCanRun()
		{
			if (this._sourceProvider == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ObjectMustBeInitialized, "SourceProvider"));
			}
		}

		[DebuggerStepThrough]
		private void EnsureRunning()
		{
			if (!this._isRunning)
			{
				using (this._lock.LockStateForWrite())
				{
					if (!this._isRunning)
					{
						this.EnsureCanRun();
						this._isRunning = true;
					}
				}
			}
		}

		[DebuggerStepThrough]
		private void EnsureCanSet<T>(T currentValue) where T : class
		{
			if (this._isRunning || currentValue != null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ObjectAlreadyInitialized, Array.Empty<object>()));
			}
		}

		private List<ComposablePart> _parts = new List<ComposablePart>();

		private volatile bool _isDisposed;

		private volatile bool _isRunning;

		private CompositionLock _lock;

		private ExportProvider _sourceProvider;

		private ImportEngine _importEngine;

		private volatile bool _currentlyComposing;

		private CompositionOptions _compositionOptions;
	}
}
