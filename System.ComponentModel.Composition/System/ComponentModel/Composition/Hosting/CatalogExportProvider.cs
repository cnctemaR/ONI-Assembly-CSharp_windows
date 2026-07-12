using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Diagnostics;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	public class CatalogExportProvider : ExportProvider, IDisposable
	{
		public CatalogExportProvider(ComposablePartCatalog catalog)
			: this(catalog, CompositionOptions.Default)
		{
		}

		public CatalogExportProvider(ComposablePartCatalog catalog, bool isThreadSafe)
			: this(catalog, isThreadSafe ? CompositionOptions.IsThreadSafe : CompositionOptions.Default)
		{
		}

		public CatalogExportProvider(ComposablePartCatalog catalog, CompositionOptions compositionOptions)
		{
			Requires.NotNull<ComposablePartCatalog>(catalog, "catalog");
			if (compositionOptions > (CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe | CompositionOptions.ExportCompositionService))
			{
				throw new ArgumentOutOfRangeException("compositionOptions");
			}
			this._catalog = catalog;
			this._compositionOptions = compositionOptions;
			INotifyComposablePartCatalogChanged notifyComposablePartCatalogChanged = this._catalog as INotifyComposablePartCatalogChanged;
			if (notifyComposablePartCatalogChanged != null)
			{
				notifyComposablePartCatalogChanged.Changing += this.OnCatalogChanging;
			}
			CompositionScopeDefinition compositionScopeDefinition = this._catalog as CompositionScopeDefinition;
			if (compositionScopeDefinition != null)
			{
				this._innerExportProvider = new AggregateExportProvider(new ExportProvider[]
				{
					new CatalogExportProvider.ScopeManager(this, compositionScopeDefinition),
					new CatalogExportProvider.InnerCatalogExportProvider(new Func<ImportDefinition, AtomicComposition, IEnumerable<Export>>(this.InternalGetExportsCore))
				});
			}
			else
			{
				this._innerExportProvider = new CatalogExportProvider.InnerCatalogExportProvider(new Func<ImportDefinition, AtomicComposition, IEnumerable<Export>>(this.InternalGetExportsCore));
			}
			this._lock = new CompositionLock(compositionOptions.HasFlag(CompositionOptions.IsThreadSafe));
		}

		public ComposablePartCatalog Catalog
		{
			get
			{
				this.ThrowIfDisposed();
				return this._catalog;
			}
		}

		public ExportProvider SourceProvider
		{
			get
			{
				this.ThrowIfDisposed();
				ExportProvider sourceProvider;
				using (this._lock.LockStateForRead())
				{
					sourceProvider = this._sourceProvider;
				}
				return sourceProvider;
			}
			set
			{
				this.ThrowIfDisposed();
				Requires.NotNull<ExportProvider>(value, "value");
				ImportEngine importEngine = null;
				AggregateExportProvider aggregateExportProvider = null;
				bool flag = true;
				try
				{
					importEngine = new ImportEngine(value, this._compositionOptions);
					value.ExportsChanging += this.OnExportsChangingInternal;
					using (this._lock.LockStateForWrite())
					{
						this.EnsureCanSet<ExportProvider>(this._sourceProvider);
						this._sourceProvider = value;
						this._importEngine = importEngine;
						flag = false;
					}
				}
				finally
				{
					if (flag)
					{
						value.ExportsChanging -= this.OnExportsChangingInternal;
						importEngine.Dispose();
						if (aggregateExportProvider != null)
						{
							aggregateExportProvider.Dispose();
						}
					}
				}
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
				bool flag = false;
				INotifyComposablePartCatalogChanged notifyComposablePartCatalogChanged = null;
				HashSet<IDisposable> hashSet = null;
				ImportEngine importEngine = null;
				ExportProvider exportProvider = null;
				AggregateExportProvider aggregateExportProvider = null;
				try
				{
					using (this._lock.LockStateForWrite())
					{
						if (!this._isDisposed)
						{
							notifyComposablePartCatalogChanged = this._catalog as INotifyComposablePartCatalogChanged;
							this._catalog = null;
							aggregateExportProvider = this._innerExportProvider as AggregateExportProvider;
							this._innerExportProvider = null;
							exportProvider = this._sourceProvider;
							this._sourceProvider = null;
							importEngine = this._importEngine;
							this._importEngine = null;
							hashSet = this._partsToDispose;
							this._gcRoots = null;
							flag = true;
							this._isDisposed = true;
						}
					}
				}
				finally
				{
					if (notifyComposablePartCatalogChanged != null)
					{
						notifyComposablePartCatalogChanged.Changing -= this.OnCatalogChanging;
					}
					if (aggregateExportProvider != null)
					{
						aggregateExportProvider.Dispose();
					}
					if (exportProvider != null)
					{
						exportProvider.ExportsChanging -= this.OnExportsChangingInternal;
					}
					if (importEngine != null)
					{
						importEngine.Dispose();
					}
					if (hashSet != null)
					{
						foreach (IDisposable disposable2 in hashSet)
						{
							disposable2.Dispose();
						}
					}
					if (flag)
					{
						this._lock.Dispose();
					}
				}
			}
		}

		protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			this.ThrowIfDisposed();
			this.EnsureRunning();
			Assumes.NotNull<ExportProvider>(this._innerExportProvider);
			IEnumerable<Export> enumerable;
			this._innerExportProvider.TryGetExports(definition, atomicComposition, out enumerable);
			return enumerable;
		}

		private IEnumerable<Export> InternalGetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			this.ThrowIfDisposed();
			this.EnsureRunning();
			ComposablePartCatalog valueAllowNull = atomicComposition.GetValueAllowNull(this._catalog);
			IPartCreatorImportDefinition partCreatorImportDefinition = definition as IPartCreatorImportDefinition;
			bool flag = false;
			if (partCreatorImportDefinition != null)
			{
				definition = partCreatorImportDefinition.ProductImportDefinition;
				flag = true;
			}
			CreationPolicy requiredCreationPolicy = definition.GetRequiredCreationPolicy();
			List<Export> list = new List<Export>();
			foreach (Tuple<ComposablePartDefinition, ExportDefinition> tuple in valueAllowNull.GetExports(definition))
			{
				if (!this.IsRejected(tuple.Item1, atomicComposition))
				{
					list.Add(this.CreateExport(tuple.Item1, tuple.Item2, flag, requiredCreationPolicy));
				}
			}
			return list;
		}

		private Export CreateExport(ComposablePartDefinition partDefinition, ExportDefinition exportDefinition, bool isExportFactory, CreationPolicy importPolicy)
		{
			if (isExportFactory)
			{
				return new CatalogExportProvider.PartCreatorExport(this, partDefinition, exportDefinition);
			}
			return CatalogExportProvider.CatalogExport.CreateExport(this, partDefinition, exportDefinition, importPolicy);
		}

		private void OnExportsChangingInternal(object sender, ExportsChangeEventArgs e)
		{
			this.UpdateRejections(e.AddedExports.Concat<ExportDefinition>(e.RemovedExports), e.AtomicComposition);
		}

		private static ExportDefinition[] GetExportsFromPartDefinitions(IEnumerable<ComposablePartDefinition> partDefinitions)
		{
			List<ExportDefinition> list = new List<ExportDefinition>();
			foreach (ComposablePartDefinition composablePartDefinition in partDefinitions)
			{
				foreach (ExportDefinition exportDefinition in composablePartDefinition.ExportDefinitions)
				{
					list.Add(exportDefinition);
					list.Add(new PartCreatorExportDefinition(exportDefinition));
				}
			}
			return list.ToArray();
		}

		private void OnCatalogChanging(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			using (AtomicComposition atomicComposition = new AtomicComposition(e.AtomicComposition))
			{
				atomicComposition.SetValue(this._catalog, new CatalogExportProvider.CatalogChangeProxy(this._catalog, e.AddedDefinitions, e.RemovedDefinitions));
				IEnumerable<ExportDefinition> addedExports = CatalogExportProvider.GetExportsFromPartDefinitions(e.AddedDefinitions);
				IEnumerable<ExportDefinition> removedExports = CatalogExportProvider.GetExportsFromPartDefinitions(e.RemovedDefinitions);
				foreach (ComposablePartDefinition composablePartDefinition in e.RemovedDefinitions)
				{
					CatalogExportProvider.CatalogPart catalogPart = null;
					bool flag = false;
					using (this._lock.LockStateForRead())
					{
						flag = this._activatedParts.TryGetValue(composablePartDefinition, out catalogPart);
					}
					if (flag)
					{
						ComposablePartDefinition capturedDefinition = composablePartDefinition;
						this.ReleasePart(null, catalogPart, atomicComposition);
						atomicComposition.AddCompleteActionAllowNull(delegate
						{
							using (this._lock.LockStateForWrite())
							{
								this._activatedParts.Remove(capturedDefinition);
							}
						});
					}
				}
				this.UpdateRejections(addedExports.ConcatAllowingNull<ExportDefinition>(removedExports), atomicComposition);
				this.OnExportsChanging(new ExportsChangeEventArgs(addedExports, removedExports, atomicComposition));
				atomicComposition.AddCompleteAction(delegate
				{
					this.OnExportsChanged(new ExportsChangeEventArgs(addedExports, removedExports, null));
				});
				atomicComposition.Complete();
			}
		}

		private CatalogExportProvider.CatalogPart GetComposablePart(ComposablePartDefinition partDefinition, bool isSharedPart)
		{
			this.ThrowIfDisposed();
			this.EnsureRunning();
			CatalogExportProvider.CatalogPart catalogPart = null;
			if (isSharedPart)
			{
				catalogPart = this.GetSharedPart(partDefinition);
			}
			else
			{
				ComposablePart composablePart = partDefinition.CreatePart();
				catalogPart = new CatalogExportProvider.CatalogPart(composablePart);
				IDisposable disposable = composablePart as IDisposable;
				if (disposable != null)
				{
					using (this._lock.LockStateForWrite())
					{
						this._partsToDispose.Add(disposable);
					}
				}
			}
			return catalogPart;
		}

		private CatalogExportProvider.CatalogPart GetSharedPart(ComposablePartDefinition partDefinition)
		{
			CatalogExportProvider.CatalogPart catalogPart = null;
			using (this._lock.LockStateForRead())
			{
				if (this._activatedParts.TryGetValue(partDefinition, out catalogPart))
				{
					return catalogPart;
				}
			}
			ComposablePart composablePart = partDefinition.CreatePart();
			IDisposable disposable2 = composablePart as IDisposable;
			using (this._lock.LockStateForWrite())
			{
				if (!this._activatedParts.TryGetValue(partDefinition, out catalogPart))
				{
					catalogPart = new CatalogExportProvider.CatalogPart(composablePart);
					this._activatedParts.Add(partDefinition, catalogPart);
					if (disposable2 != null)
					{
						this._partsToDispose.Add(disposable2);
					}
					disposable2 = null;
				}
			}
			if (disposable2 != null)
			{
				disposable2.Dispose();
			}
			return catalogPart;
		}

		private object GetExportedValue(CatalogExportProvider.CatalogPart part, ExportDefinition export, bool isSharedPart)
		{
			this.ThrowIfDisposed();
			this.EnsureRunning();
			Assumes.NotNull<CatalogExportProvider.CatalogPart, ExportDefinition>(part, export);
			bool importsSatisfied = part.ImportsSatisfied;
			object exportedValueFromComposedPart = CompositionServices.GetExportedValueFromComposedPart(importsSatisfied ? null : this._importEngine, part.Part, export);
			if (!importsSatisfied)
			{
				part.ImportsSatisfied = true;
			}
			if (exportedValueFromComposedPart != null && !isSharedPart && part.Part.IsRecomposable())
			{
				this.PreventPartCollection(exportedValueFromComposedPart, part.Part);
			}
			return exportedValueFromComposedPart;
		}

		private void ReleasePart(object exportedValue, CatalogExportProvider.CatalogPart catalogPart, AtomicComposition atomicComposition)
		{
			this.ThrowIfDisposed();
			this.EnsureRunning();
			Assumes.NotNull<CatalogExportProvider.CatalogPart>(catalogPart);
			this._importEngine.ReleaseImports(catalogPart.Part, atomicComposition);
			if (exportedValue != null)
			{
				atomicComposition.AddCompleteActionAllowNull(delegate
				{
					this.AllowPartCollection(exportedValue);
				});
			}
			IDisposable diposablePart = catalogPart.Part as IDisposable;
			if (diposablePart != null)
			{
				atomicComposition.AddCompleteActionAllowNull(delegate
				{
					bool flag = false;
					using (this._lock.LockStateForWrite())
					{
						flag = this._partsToDispose.Remove(diposablePart);
					}
					if (flag)
					{
						diposablePart.Dispose();
					}
				});
			}
		}

		private void PreventPartCollection(object exportedValue, ComposablePart part)
		{
			Assumes.NotNull<object, ComposablePart>(exportedValue, part);
			using (this._lock.LockStateForWrite())
			{
				ConditionalWeakTable<object, List<ComposablePart>> conditionalWeakTable = this._gcRoots;
				if (conditionalWeakTable == null)
				{
					conditionalWeakTable = new ConditionalWeakTable<object, List<ComposablePart>>();
				}
				List<ComposablePart> list;
				if (!conditionalWeakTable.TryGetValue(exportedValue, out list))
				{
					list = new List<ComposablePart>();
					conditionalWeakTable.Add(exportedValue, list);
				}
				list.Add(part);
				if (this._gcRoots == null)
				{
					Thread.MemoryBarrier();
					this._gcRoots = conditionalWeakTable;
				}
			}
		}

		private void AllowPartCollection(object gcRoot)
		{
			if (this._gcRoots != null)
			{
				using (this._lock.LockStateForWrite())
				{
					this._gcRoots.Remove(gcRoot);
				}
			}
		}

		private bool IsRejected(ComposablePartDefinition definition, AtomicComposition atomicComposition)
		{
			bool flag = false;
			if (atomicComposition != null)
			{
				CatalogExportProvider.AtomicCompositionQueryState atomicCompositionQueryState = this.GetAtomicCompositionQuery(atomicComposition)(definition);
				switch (atomicCompositionQueryState)
				{
				case CatalogExportProvider.AtomicCompositionQueryState.TreatAsRejected:
					return true;
				case CatalogExportProvider.AtomicCompositionQueryState.TreatAsValidated:
					return false;
				case CatalogExportProvider.AtomicCompositionQueryState.NeedsTesting:
					flag = true;
					break;
				default:
					Assumes.IsTrue(atomicCompositionQueryState == CatalogExportProvider.AtomicCompositionQueryState.Unknown);
					break;
				}
			}
			if (!flag)
			{
				using (this._lock.LockStateForRead())
				{
					if (this._activatedParts.ContainsKey(definition))
					{
						return false;
					}
					if (this._rejectedParts.Contains(definition))
					{
						return true;
					}
				}
			}
			return this.DetermineRejection(definition, atomicComposition);
		}

		private bool DetermineRejection(ComposablePartDefinition definition, AtomicComposition parentAtomicComposition)
		{
			ChangeRejectedException exception = null;
			using (AtomicComposition atomicComposition = new AtomicComposition(parentAtomicComposition))
			{
				this.UpdateAtomicCompositionQuery(atomicComposition, (ComposablePartDefinition def) => definition.Equals(def), CatalogExportProvider.AtomicCompositionQueryState.TreatAsValidated);
				ComposablePart newPart = definition.CreatePart();
				try
				{
					this._importEngine.PreviewImports(newPart, atomicComposition);
					atomicComposition.AddCompleteActionAllowNull(delegate
					{
						using (this._lock.LockStateForWrite())
						{
							if (!this._activatedParts.ContainsKey(definition))
							{
								this._activatedParts.Add(definition, new CatalogExportProvider.CatalogPart(newPart));
								IDisposable disposable2 = newPart as IDisposable;
								if (disposable2 != null)
								{
									this._partsToDispose.Add(disposable2);
								}
							}
						}
					});
					atomicComposition.Complete();
					return false;
				}
				catch (ChangeRejectedException ex)
				{
					exception = ex;
				}
			}
			parentAtomicComposition.AddCompleteActionAllowNull(delegate
			{
				using (this._lock.LockStateForWrite())
				{
					this._rejectedParts.Add(definition);
				}
				CompositionTrace.PartDefinitionRejected(definition, exception);
			});
			if (parentAtomicComposition != null)
			{
				this.UpdateAtomicCompositionQuery(parentAtomicComposition, (ComposablePartDefinition def) => definition.Equals(def), CatalogExportProvider.AtomicCompositionQueryState.TreatAsRejected);
			}
			return true;
		}

		private void UpdateRejections(IEnumerable<ExportDefinition> changedExports, AtomicComposition atomicComposition)
		{
			using (AtomicComposition atomicComposition2 = new AtomicComposition(atomicComposition))
			{
				HashSet<ComposablePartDefinition> affectedRejections = new HashSet<ComposablePartDefinition>();
				Func<ComposablePartDefinition, CatalogExportProvider.AtomicCompositionQueryState> atomicCompositionQuery = this.GetAtomicCompositionQuery(atomicComposition2);
				ComposablePartDefinition[] array;
				using (this._lock.LockStateForRead())
				{
					array = this._rejectedParts.ToArray<ComposablePartDefinition>();
				}
				foreach (ComposablePartDefinition composablePartDefinition in array)
				{
					if (atomicCompositionQuery(composablePartDefinition) != CatalogExportProvider.AtomicCompositionQueryState.TreatAsValidated)
					{
						using (IEnumerator<ImportDefinition> enumerator = composablePartDefinition.ImportDefinitions.Where<ImportDefinition>(new Func<ImportDefinition, bool>(ImportEngine.IsRequiredImportForPreview)).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ImportDefinition import = enumerator.Current;
								if (changedExports.Any<ExportDefinition>((ExportDefinition export) => import.IsConstraintSatisfiedBy(export)))
								{
									affectedRejections.Add(composablePartDefinition);
									break;
								}
							}
						}
					}
				}
				this.UpdateAtomicCompositionQuery(atomicComposition2, (ComposablePartDefinition def) => affectedRejections.Contains(def), CatalogExportProvider.AtomicCompositionQueryState.NeedsTesting);
				List<ExportDefinition> resurrectedExports = new List<ExportDefinition>();
				foreach (ComposablePartDefinition composablePartDefinition2 in affectedRejections)
				{
					if (!this.IsRejected(composablePartDefinition2, atomicComposition2))
					{
						resurrectedExports.AddRange(composablePartDefinition2.ExportDefinitions);
						ComposablePartDefinition capturedPartDefinition = composablePartDefinition2;
						atomicComposition2.AddCompleteAction(delegate
						{
							using (this._lock.LockStateForWrite())
							{
								this._rejectedParts.Remove(capturedPartDefinition);
							}
							CompositionTrace.PartDefinitionResurrected(capturedPartDefinition);
						});
					}
				}
				if (resurrectedExports.Any<ExportDefinition>())
				{
					this.OnExportsChanging(new ExportsChangeEventArgs(resurrectedExports, new ExportDefinition[0], atomicComposition2));
					atomicComposition2.AddCompleteAction(delegate
					{
						this.OnExportsChanged(new ExportsChangeEventArgs(resurrectedExports, new ExportDefinition[0], null));
					});
				}
				atomicComposition2.Complete();
			}
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		[DebuggerStepThrough]
		private void EnsureCanRun()
		{
			if (this._sourceProvider == null || this._importEngine == null)
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

		private Func<ComposablePartDefinition, CatalogExportProvider.AtomicCompositionQueryState> GetAtomicCompositionQuery(AtomicComposition atomicComposition)
		{
			Func<ComposablePartDefinition, CatalogExportProvider.AtomicCompositionQueryState> func;
			atomicComposition.TryGetValue<Func<ComposablePartDefinition, CatalogExportProvider.AtomicCompositionQueryState>>(this, out func);
			if (func == null)
			{
				return (ComposablePartDefinition definition) => CatalogExportProvider.AtomicCompositionQueryState.Unknown;
			}
			return func;
		}

		private void UpdateAtomicCompositionQuery(AtomicComposition atomicComposition, Func<ComposablePartDefinition, bool> query, CatalogExportProvider.AtomicCompositionQueryState state)
		{
			Func<ComposablePartDefinition, CatalogExportProvider.AtomicCompositionQueryState> parentQuery = this.GetAtomicCompositionQuery(atomicComposition);
			Func<ComposablePartDefinition, CatalogExportProvider.AtomicCompositionQueryState> func = delegate(ComposablePartDefinition definition)
			{
				if (query(definition))
				{
					return state;
				}
				return parentQuery(definition);
			};
			atomicComposition.SetValue(this, func);
		}

		private readonly CompositionLock _lock;

		private Dictionary<ComposablePartDefinition, CatalogExportProvider.CatalogPart> _activatedParts = new Dictionary<ComposablePartDefinition, CatalogExportProvider.CatalogPart>();

		private HashSet<ComposablePartDefinition> _rejectedParts = new HashSet<ComposablePartDefinition>();

		private ConditionalWeakTable<object, List<ComposablePart>> _gcRoots;

		private HashSet<IDisposable> _partsToDispose = new HashSet<IDisposable>();

		private ComposablePartCatalog _catalog;

		private volatile bool _isDisposed;

		private volatile bool _isRunning;

		private ExportProvider _sourceProvider;

		private ImportEngine _importEngine;

		private CompositionOptions _compositionOptions;

		private ExportProvider _innerExportProvider;

		private class CatalogChangeProxy : ComposablePartCatalog
		{
			public CatalogChangeProxy(ComposablePartCatalog originalCatalog, IEnumerable<ComposablePartDefinition> addedParts, IEnumerable<ComposablePartDefinition> removedParts)
			{
				this._originalCatalog = originalCatalog;
				this._addedParts = new List<ComposablePartDefinition>(addedParts);
				this._removedParts = new HashSet<ComposablePartDefinition>(removedParts);
			}

			public override IEnumerator<ComposablePartDefinition> GetEnumerator()
			{
				return this._originalCatalog.Concat<ComposablePartDefinition>(this._addedParts).Except<ComposablePartDefinition>(this._removedParts).GetEnumerator();
			}

			public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
			{
				Requires.NotNull<ImportDefinition>(definition, "definition");
				IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> enumerable = from partAndExport in this._originalCatalog.GetExports(definition)
					where !this._removedParts.Contains(partAndExport.Item1)
					select partAndExport;
				List<Tuple<ComposablePartDefinition, ExportDefinition>> list = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
				foreach (ComposablePartDefinition composablePartDefinition in this._addedParts)
				{
					foreach (ExportDefinition exportDefinition in composablePartDefinition.ExportDefinitions)
					{
						if (definition.IsConstraintSatisfiedBy(exportDefinition))
						{
							list.Add(new Tuple<ComposablePartDefinition, ExportDefinition>(composablePartDefinition, exportDefinition));
						}
					}
				}
				return enumerable.Concat<Tuple<ComposablePartDefinition, ExportDefinition>>(list);
			}

			private ComposablePartCatalog _originalCatalog;

			private List<ComposablePartDefinition> _addedParts;

			private HashSet<ComposablePartDefinition> _removedParts;
		}

		private class CatalogExport : Export
		{
			public CatalogExport(CatalogExportProvider catalogExportProvider, ComposablePartDefinition partDefinition, ExportDefinition definition)
			{
				this._catalogExportProvider = catalogExportProvider;
				this._partDefinition = partDefinition;
				this._definition = definition;
			}

			public override ExportDefinition Definition
			{
				get
				{
					return this._definition;
				}
			}

			protected virtual bool IsSharedPart
			{
				get
				{
					return true;
				}
			}

			protected CatalogExportProvider.CatalogPart GetPartCore()
			{
				return this._catalogExportProvider.GetComposablePart(this._partDefinition, this.IsSharedPart);
			}

			protected void ReleasePartCore(CatalogExportProvider.CatalogPart part, object value)
			{
				this._catalogExportProvider.ReleasePart(value, part, null);
			}

			protected virtual CatalogExportProvider.CatalogPart GetPart()
			{
				return this.GetPartCore();
			}

			protected override object GetExportedValueCore()
			{
				return this._catalogExportProvider.GetExportedValue(this.GetPart(), this._definition, this.IsSharedPart);
			}

			public static CatalogExportProvider.CatalogExport CreateExport(CatalogExportProvider catalogExportProvider, ComposablePartDefinition partDefinition, ExportDefinition definition, CreationPolicy importCreationPolicy)
			{
				if (CatalogExportProvider.CatalogExport.ShouldUseSharedPart(partDefinition.Metadata.GetValue<CreationPolicy>("System.ComponentModel.Composition.CreationPolicy"), importCreationPolicy))
				{
					return new CatalogExportProvider.CatalogExport(catalogExportProvider, partDefinition, definition);
				}
				return new CatalogExportProvider.NonSharedCatalogExport(catalogExportProvider, partDefinition, definition);
			}

			private static bool ShouldUseSharedPart(CreationPolicy partPolicy, CreationPolicy importPolicy)
			{
				if (partPolicy == CreationPolicy.Any)
				{
					return importPolicy == CreationPolicy.Any || importPolicy == CreationPolicy.NewScope || importPolicy == CreationPolicy.Shared;
				}
				if (partPolicy != CreationPolicy.NonShared)
				{
					Assumes.IsTrue(partPolicy == CreationPolicy.Shared);
					Assumes.IsTrue(importPolicy != CreationPolicy.NonShared && importPolicy != CreationPolicy.NewScope);
					return true;
				}
				Assumes.IsTrue(importPolicy != CreationPolicy.Shared);
				return false;
			}

			protected readonly CatalogExportProvider _catalogExportProvider;

			protected readonly ComposablePartDefinition _partDefinition;

			protected readonly ExportDefinition _definition;
		}

		private sealed class NonSharedCatalogExport : CatalogExportProvider.CatalogExport, IDisposable
		{
			public NonSharedCatalogExport(CatalogExportProvider catalogExportProvider, ComposablePartDefinition partDefinition, ExportDefinition definition)
				: base(catalogExportProvider, partDefinition, definition)
			{
			}

			protected override CatalogExportProvider.CatalogPart GetPart()
			{
				if (this._part == null)
				{
					CatalogExportProvider.CatalogPart catalogPart = base.GetPartCore();
					object @lock = this._lock;
					lock (@lock)
					{
						if (this._part == null)
						{
							Thread.MemoryBarrier();
							this._part = catalogPart;
							catalogPart = null;
						}
					}
					if (catalogPart != null)
					{
						base.ReleasePartCore(catalogPart, null);
					}
				}
				return this._part;
			}

			protected override bool IsSharedPart
			{
				get
				{
					return false;
				}
			}

			void IDisposable.Dispose()
			{
				if (this._part != null)
				{
					base.ReleasePartCore(this._part, base.Value);
					this._part = null;
				}
			}

			private CatalogExportProvider.CatalogPart _part;

			private readonly object _lock = new object();
		}

		internal abstract class FactoryExport : Export
		{
			public FactoryExport(ComposablePartDefinition partDefinition, ExportDefinition exportDefinition)
			{
				this._partDefinition = partDefinition;
				this._exportDefinition = exportDefinition;
				this._factoryExportDefinition = new PartCreatorExportDefinition(this._exportDefinition);
			}

			public override ExportDefinition Definition
			{
				get
				{
					return this._factoryExportDefinition;
				}
			}

			protected override object GetExportedValueCore()
			{
				if (this._factoryExportPartDefinition == null)
				{
					this._factoryExportPartDefinition = new CatalogExportProvider.FactoryExport.FactoryExportPartDefinition(this);
				}
				return this._factoryExportPartDefinition;
			}

			protected ComposablePartDefinition UnderlyingPartDefinition
			{
				get
				{
					return this._partDefinition;
				}
			}

			protected ExportDefinition UnderlyingExportDefinition
			{
				get
				{
					return this._exportDefinition;
				}
			}

			public abstract Export CreateExportProduct();

			private readonly ComposablePartDefinition _partDefinition;

			private readonly ExportDefinition _exportDefinition;

			private ExportDefinition _factoryExportDefinition;

			private CatalogExportProvider.FactoryExport.FactoryExportPartDefinition _factoryExportPartDefinition;

			private class FactoryExportPartDefinition : ComposablePartDefinition
			{
				public FactoryExportPartDefinition(CatalogExportProvider.FactoryExport FactoryExport)
				{
					this._FactoryExport = FactoryExport;
				}

				public override IEnumerable<ExportDefinition> ExportDefinitions
				{
					get
					{
						return new ExportDefinition[] { this._FactoryExport.Definition };
					}
				}

				public override IEnumerable<ImportDefinition> ImportDefinitions
				{
					get
					{
						return Enumerable.Empty<ImportDefinition>();
					}
				}

				public ExportDefinition FactoryExportDefinition
				{
					get
					{
						return this._FactoryExport.Definition;
					}
				}

				public Export CreateProductExport()
				{
					return this._FactoryExport.CreateExportProduct();
				}

				public override ComposablePart CreatePart()
				{
					return new CatalogExportProvider.FactoryExport.FactoryExportPart(this);
				}

				private readonly CatalogExportProvider.FactoryExport _FactoryExport;
			}

			private sealed class FactoryExportPart : ComposablePart, IDisposable
			{
				public FactoryExportPart(CatalogExportProvider.FactoryExport.FactoryExportPartDefinition definition)
				{
					this._definition = definition;
					this._export = definition.CreateProductExport();
				}

				public override IEnumerable<ExportDefinition> ExportDefinitions
				{
					get
					{
						return this._definition.ExportDefinitions;
					}
				}

				public override IEnumerable<ImportDefinition> ImportDefinitions
				{
					get
					{
						return this._definition.ImportDefinitions;
					}
				}

				public override object GetExportedValue(ExportDefinition definition)
				{
					if (definition != this._definition.FactoryExportDefinition)
					{
						throw ExceptionBuilder.CreateExportDefinitionNotOnThisComposablePart("definition");
					}
					return this._export.Value;
				}

				public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
				{
					throw ExceptionBuilder.CreateImportDefinitionNotOnThisComposablePart("definition");
				}

				public void Dispose()
				{
					IDisposable disposable = this._export as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}

				private readonly CatalogExportProvider.FactoryExport.FactoryExportPartDefinition _definition;

				private readonly Export _export;
			}
		}

		internal class PartCreatorExport : CatalogExportProvider.FactoryExport
		{
			public PartCreatorExport(CatalogExportProvider catalogExportProvider, ComposablePartDefinition partDefinition, ExportDefinition exportDefinition)
				: base(partDefinition, exportDefinition)
			{
				this._catalogExportProvider = catalogExportProvider;
			}

			public override Export CreateExportProduct()
			{
				return new CatalogExportProvider.NonSharedCatalogExport(this._catalogExportProvider, base.UnderlyingPartDefinition, base.UnderlyingExportDefinition);
			}

			private readonly CatalogExportProvider _catalogExportProvider;
		}

		internal class ScopeFactoryExport : CatalogExportProvider.FactoryExport
		{
			internal ScopeFactoryExport(CatalogExportProvider.ScopeManager scopeManager, CompositionScopeDefinition catalog, ComposablePartDefinition partDefinition, ExportDefinition exportDefinition)
				: base(partDefinition, exportDefinition)
			{
				this._scopeManager = scopeManager;
				this._catalog = catalog;
			}

			public virtual Export CreateExportProduct(Func<ComposablePartDefinition, bool> filter)
			{
				return new CatalogExportProvider.ScopeFactoryExport.ScopeCatalogExport(this, filter);
			}

			public override Export CreateExportProduct()
			{
				return new CatalogExportProvider.ScopeFactoryExport.ScopeCatalogExport(this, null);
			}

			private readonly CatalogExportProvider.ScopeManager _scopeManager;

			private readonly CompositionScopeDefinition _catalog;

			private sealed class ScopeCatalogExport : Export, IDisposable
			{
				public ScopeCatalogExport(CatalogExportProvider.ScopeFactoryExport scopeFactoryExport, Func<ComposablePartDefinition, bool> catalogFilter)
				{
					this._scopeFactoryExport = scopeFactoryExport;
					this._catalogFilter = catalogFilter;
				}

				public override ExportDefinition Definition
				{
					get
					{
						return this._scopeFactoryExport.UnderlyingExportDefinition;
					}
				}

				protected override object GetExportedValueCore()
				{
					if (this._export == null)
					{
						CompositionScopeDefinition compositionScopeDefinition = new CompositionScopeDefinition(new FilteredCatalog(this._scopeFactoryExport._catalog, this._catalogFilter), this._scopeFactoryExport._catalog.Children);
						CompositionContainer compositionContainer = this._scopeFactoryExport._scopeManager.CreateChildContainer(compositionScopeDefinition);
						Export export = compositionContainer.CatalogExportProvider.CreateExport(this._scopeFactoryExport.UnderlyingPartDefinition, this._scopeFactoryExport.UnderlyingExportDefinition, false, CreationPolicy.Any);
						object @lock = this._lock;
						lock (@lock)
						{
							if (this._export == null)
							{
								this._childContainer = compositionContainer;
								Thread.MemoryBarrier();
								this._export = export;
								compositionContainer = null;
							}
						}
						if (compositionContainer != null)
						{
							compositionContainer.Dispose();
						}
					}
					return this._export.Value;
				}

				public void Dispose()
				{
					CompositionContainer compositionContainer = null;
					if (this._export != null)
					{
						object @lock = this._lock;
						lock (@lock)
						{
							Export export = this._export;
							compositionContainer = this._childContainer;
							this._childContainer = null;
							Thread.MemoryBarrier();
							this._export = null;
						}
					}
					if (compositionContainer != null)
					{
						compositionContainer.Dispose();
					}
				}

				private readonly CatalogExportProvider.ScopeFactoryExport _scopeFactoryExport;

				private Func<ComposablePartDefinition, bool> _catalogFilter;

				private CompositionContainer _childContainer;

				private Export _export;

				private readonly object _lock = new object();
			}
		}

		internal class ScopeManager : ExportProvider
		{
			public ScopeManager(CatalogExportProvider catalogExportProvider, CompositionScopeDefinition scopeDefinition)
			{
				Assumes.NotNull<CatalogExportProvider>(catalogExportProvider);
				Assumes.NotNull<CompositionScopeDefinition>(scopeDefinition);
				this._scopeDefinition = scopeDefinition;
				this._catalogExportProvider = catalogExportProvider;
			}

			protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
			{
				List<Export> list = new List<Export>();
				ImportDefinition importDefinition = CatalogExportProvider.ScopeManager.TranslateImport(definition);
				if (importDefinition == null)
				{
					return list;
				}
				foreach (CompositionScopeDefinition compositionScopeDefinition in this._scopeDefinition.Children)
				{
					foreach (Tuple<ComposablePartDefinition, ExportDefinition> tuple in compositionScopeDefinition.GetExportsFromPublicSurface(importDefinition))
					{
						using (CompositionContainer compositionContainer = this.CreateChildContainer(compositionScopeDefinition))
						{
							using (AtomicComposition atomicComposition2 = new AtomicComposition(atomicComposition))
							{
								if (!compositionContainer.CatalogExportProvider.DetermineRejection(tuple.Item1, atomicComposition2))
								{
									list.Add(this.CreateScopeExport(compositionScopeDefinition, tuple.Item1, tuple.Item2));
								}
							}
						}
					}
				}
				return list;
			}

			private Export CreateScopeExport(CompositionScopeDefinition childCatalog, ComposablePartDefinition partDefinition, ExportDefinition exportDefinition)
			{
				return new CatalogExportProvider.ScopeFactoryExport(this, childCatalog, partDefinition, exportDefinition);
			}

			internal CompositionContainer CreateChildContainer(ComposablePartCatalog childCatalog)
			{
				return new CompositionContainer(childCatalog, this._catalogExportProvider._compositionOptions, new ExportProvider[] { this._catalogExportProvider._sourceProvider });
			}

			private static ImportDefinition TranslateImport(ImportDefinition definition)
			{
				IPartCreatorImportDefinition partCreatorImportDefinition = definition as IPartCreatorImportDefinition;
				if (partCreatorImportDefinition == null)
				{
					return null;
				}
				ContractBasedImportDefinition productImportDefinition = partCreatorImportDefinition.ProductImportDefinition;
				ImportDefinition importDefinition = null;
				CreationPolicy requiredCreationPolicy = productImportDefinition.RequiredCreationPolicy;
				if (requiredCreationPolicy != CreationPolicy.Any)
				{
					if (requiredCreationPolicy - CreationPolicy.NonShared <= 1)
					{
						importDefinition = new ContractBasedImportDefinition(productImportDefinition.ContractName, productImportDefinition.RequiredTypeIdentity, productImportDefinition.RequiredMetadata, productImportDefinition.Cardinality, productImportDefinition.IsRecomposable, productImportDefinition.IsPrerequisite, CreationPolicy.Any, productImportDefinition.Metadata);
					}
				}
				else
				{
					importDefinition = productImportDefinition;
				}
				return importDefinition;
			}

			private CompositionScopeDefinition _scopeDefinition;

			private CatalogExportProvider _catalogExportProvider;
		}

		private class InnerCatalogExportProvider : ExportProvider
		{
			public InnerCatalogExportProvider(Func<ImportDefinition, AtomicComposition, IEnumerable<Export>> getExportsCore)
			{
				this._getExportsCore = getExportsCore;
			}

			protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
			{
				Assumes.NotNull<Func<ImportDefinition, AtomicComposition, IEnumerable<Export>>>(this._getExportsCore);
				return this._getExportsCore(definition, atomicComposition);
			}

			private Func<ImportDefinition, AtomicComposition, IEnumerable<Export>> _getExportsCore;
		}

		private enum AtomicCompositionQueryState
		{
			Unknown,
			TreatAsRejected,
			TreatAsValidated,
			NeedsTesting
		}

		private class CatalogPart
		{
			public CatalogPart(ComposablePart part)
			{
				this.Part = part;
			}

			public ComposablePart Part { get; private set; }

			public bool ImportsSatisfied
			{
				get
				{
					return this._importsSatisfied;
				}
				set
				{
					this._importsSatisfied = value;
				}
			}

			private volatile bool _importsSatisfied;
		}
	}
}
