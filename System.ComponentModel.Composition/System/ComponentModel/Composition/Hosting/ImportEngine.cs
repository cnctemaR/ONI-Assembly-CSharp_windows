using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	public class ImportEngine : ICompositionService, IDisposable
	{
		public ImportEngine(ExportProvider sourceProvider)
			: this(sourceProvider, CompositionOptions.Default)
		{
		}

		public ImportEngine(ExportProvider sourceProvider, bool isThreadSafe)
			: this(sourceProvider, isThreadSafe ? CompositionOptions.IsThreadSafe : CompositionOptions.Default)
		{
		}

		public ImportEngine(ExportProvider sourceProvider, CompositionOptions compositionOptions)
		{
			Requires.NotNull<ExportProvider>(sourceProvider, "sourceProvider");
			this._compositionOptions = compositionOptions;
			this._sourceProvider = sourceProvider;
			this._sourceProvider.ExportsChanging += this.OnExportsChanging;
			this._lock = new CompositionLock(compositionOptions.HasFlag(CompositionOptions.IsThreadSafe));
		}

		public void PreviewImports(ComposablePart part, AtomicComposition atomicComposition)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ComposablePart>(part, "part");
			if (this._compositionOptions.HasFlag(CompositionOptions.DisableSilentRejection))
			{
				return;
			}
			IDisposable compositionLockHolder = (this._lock.IsThreadSafe ? this._lock.LockComposition() : null);
			bool flag = compositionLockHolder != null;
			try
			{
				if (flag && atomicComposition != null)
				{
					atomicComposition.AddRevertAction(delegate
					{
						compositionLockHolder.Dispose();
					});
				}
				ImportEngine.PartManager partManager = this.GetPartManager(part, true);
				this.TryPreviewImportsStateMachine(partManager, part, atomicComposition).ThrowOnErrors(atomicComposition);
				this.StartSatisfyingImports(partManager, atomicComposition);
				if (flag && atomicComposition != null)
				{
					atomicComposition.AddCompleteAction(delegate
					{
						compositionLockHolder.Dispose();
					});
				}
			}
			finally
			{
				if (flag && atomicComposition == null)
				{
					compositionLockHolder.Dispose();
				}
			}
		}

		public void SatisfyImports(ComposablePart part)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ComposablePart>(part, "part");
			ImportEngine.PartManager partManager = this.GetPartManager(part, true);
			if (partManager.State == ImportEngine.ImportState.Composed)
			{
				return;
			}
			using (this._lock.LockComposition())
			{
				this.TrySatisfyImports(partManager, part, true).ThrowOnErrors();
			}
		}

		public void SatisfyImportsOnce(ComposablePart part)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ComposablePart>(part, "part");
			ImportEngine.PartManager partManager = this.GetPartManager(part, true);
			if (partManager.State == ImportEngine.ImportState.Composed)
			{
				return;
			}
			using (this._lock.LockComposition())
			{
				this.TrySatisfyImports(partManager, part, false).ThrowOnErrors();
			}
		}

		public void ReleaseImports(ComposablePart part, AtomicComposition atomicComposition)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ComposablePart>(part, "part");
			using (this._lock.LockComposition())
			{
				ImportEngine.PartManager partManager = this.GetPartManager(part, false);
				if (partManager != null)
				{
					this.StopSatisfyingImports(partManager, atomicComposition);
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
				ExportProvider exportProvider = null;
				using (this._lock.LockStateForWrite())
				{
					if (!this._isDisposed)
					{
						exportProvider = this._sourceProvider;
						this._sourceProvider = null;
						this._recompositionManager = null;
						this._partManagers = null;
						this._isDisposed = true;
						flag = true;
					}
				}
				if (exportProvider != null)
				{
					exportProvider.ExportsChanging -= this.OnExportsChanging;
				}
				if (flag)
				{
					this._lock.Dispose();
				}
			}
		}

		private CompositionResult TryPreviewImportsStateMachine(ImportEngine.PartManager partManager, ComposablePart part, AtomicComposition atomicComposition)
		{
			CompositionResult compositionResult = CompositionResult.SucceededResult;
			if (partManager.State == ImportEngine.ImportState.ImportsPreviewing)
			{
				return new CompositionResult(new CompositionError[] { ErrorBuilder.CreatePartCycle(part) });
			}
			if (partManager.State == ImportEngine.ImportState.NoImportsSatisfied)
			{
				partManager.State = ImportEngine.ImportState.ImportsPreviewing;
				IEnumerable<ImportDefinition> enumerable = part.ImportDefinitions.Where<ImportDefinition>(new Func<ImportDefinition, bool>(ImportEngine.IsRequiredImportForPreview));
				atomicComposition.AddRevertActionAllowNull(delegate
				{
					partManager.State = ImportEngine.ImportState.NoImportsSatisfied;
				});
				compositionResult = compositionResult.MergeResult(this.TrySatisfyImportSubset(partManager, enumerable, atomicComposition));
				if (!compositionResult.Succeeded)
				{
					partManager.State = ImportEngine.ImportState.NoImportsSatisfied;
					return compositionResult;
				}
				partManager.State = ImportEngine.ImportState.ImportsPreviewed;
			}
			return compositionResult;
		}

		private CompositionResult TrySatisfyImportsStateMachine(ImportEngine.PartManager partManager, ComposablePart part)
		{
			CompositionResult compositionResult = CompositionResult.SucceededResult;
			while (partManager.State < ImportEngine.ImportState.Composed)
			{
				ImportEngine.ImportState state = partManager.State;
				switch (partManager.State)
				{
				case ImportEngine.ImportState.NoImportsSatisfied:
				case ImportEngine.ImportState.ImportsPreviewed:
				{
					partManager.State = ImportEngine.ImportState.PreExportImportsSatisfying;
					IEnumerable<ImportDefinition> enumerable = part.ImportDefinitions.Where<ImportDefinition>((ImportDefinition import) => import.IsPrerequisite);
					compositionResult = compositionResult.MergeResult(this.TrySatisfyImportSubset(partManager, enumerable, null));
					partManager.State = ImportEngine.ImportState.PreExportImportsSatisfied;
					break;
				}
				case ImportEngine.ImportState.ImportsPreviewing:
					return new CompositionResult(new CompositionError[] { ErrorBuilder.CreatePartCycle(part) });
				case ImportEngine.ImportState.PreExportImportsSatisfying:
				case ImportEngine.ImportState.PostExportImportsSatisfying:
					if (this.InPrerequisiteLoop())
					{
						return compositionResult.MergeError(ErrorBuilder.CreatePartCycle(part));
					}
					return compositionResult;
				case ImportEngine.ImportState.PreExportImportsSatisfied:
				{
					partManager.State = ImportEngine.ImportState.PostExportImportsSatisfying;
					IEnumerable<ImportDefinition> enumerable2 = part.ImportDefinitions.Where<ImportDefinition>((ImportDefinition import) => !import.IsPrerequisite);
					compositionResult = compositionResult.MergeResult(this.TrySatisfyImportSubset(partManager, enumerable2, null));
					partManager.State = ImportEngine.ImportState.PostExportImportsSatisfied;
					break;
				}
				case ImportEngine.ImportState.PostExportImportsSatisfied:
					partManager.State = ImportEngine.ImportState.ComposedNotifying;
					partManager.ClearSavedImports();
					compositionResult = compositionResult.MergeResult(partManager.TryOnComposed());
					partManager.State = ImportEngine.ImportState.Composed;
					break;
				case ImportEngine.ImportState.ComposedNotifying:
					return compositionResult;
				}
				if (!compositionResult.Succeeded)
				{
					partManager.State = state;
					return compositionResult;
				}
			}
			return compositionResult;
		}

		private CompositionResult TrySatisfyImports(ImportEngine.PartManager partManager, ComposablePart part, bool shouldTrackImports)
		{
			Assumes.NotNull<ComposablePart>(part);
			CompositionResult compositionResult = CompositionResult.SucceededResult;
			if (partManager.State == ImportEngine.ImportState.Composed)
			{
				return compositionResult;
			}
			if (this._recursionStateStack.Count >= 100)
			{
				return compositionResult.MergeError(ErrorBuilder.ComposeTookTooManyIterations(100));
			}
			this._recursionStateStack.Push(partManager);
			try
			{
				compositionResult = compositionResult.MergeResult(this.TrySatisfyImportsStateMachine(partManager, part));
			}
			finally
			{
				this._recursionStateStack.Pop();
			}
			if (shouldTrackImports)
			{
				this.StartSatisfyingImports(partManager, null);
			}
			return compositionResult;
		}

		private CompositionResult TrySatisfyImportSubset(ImportEngine.PartManager partManager, IEnumerable<ImportDefinition> imports, AtomicComposition atomicComposition)
		{
			CompositionResult compositionResult = CompositionResult.SucceededResult;
			ComposablePart part = partManager.Part;
			foreach (ImportDefinition importDefinition in imports)
			{
				Export[] array = partManager.GetSavedImport(importDefinition);
				if (array == null)
				{
					CompositionResult<IEnumerable<Export>> compositionResult2 = ImportEngine.TryGetExports(this._sourceProvider, part, importDefinition, atomicComposition);
					if (!compositionResult2.Succeeded)
					{
						compositionResult = compositionResult.MergeResult(compositionResult2.ToResult());
						continue;
					}
					array = compositionResult2.Value.AsArray<Export>();
				}
				if (atomicComposition == null)
				{
					compositionResult = compositionResult.MergeResult(partManager.TrySetImport(importDefinition, array));
				}
				else
				{
					partManager.SetSavedImport(importDefinition, array, atomicComposition);
				}
			}
			return compositionResult;
		}

		private void OnExportsChanging(object sender, ExportsChangeEventArgs e)
		{
			CompositionResult compositionResult = CompositionResult.SucceededResult;
			AtomicComposition atomicComposition = e.AtomicComposition;
			IEnumerable<ImportEngine.PartManager> enumerable = this._recompositionManager.GetAffectedParts(e.ChangedContractNames);
			ImportEngine.EngineContext engineContext;
			if (atomicComposition != null && atomicComposition.TryGetValue<ImportEngine.EngineContext>(this, out engineContext))
			{
				enumerable = enumerable.ConcatAllowingNull<ImportEngine.PartManager>(engineContext.GetAddedPartManagers()).Except<ImportEngine.PartManager>(engineContext.GetRemovedPartManagers());
			}
			IEnumerable<ExportDefinition> enumerable2 = e.AddedExports.ConcatAllowingNull<ExportDefinition>(e.RemovedExports);
			foreach (ImportEngine.PartManager partManager in enumerable)
			{
				compositionResult = compositionResult.MergeResult(this.TryRecomposeImports(partManager, enumerable2, atomicComposition));
			}
			compositionResult.ThrowOnErrors(atomicComposition);
		}

		private CompositionResult TryRecomposeImports(ImportEngine.PartManager partManager, IEnumerable<ExportDefinition> changedExports, AtomicComposition atomicComposition)
		{
			CompositionResult compositionResult = CompositionResult.SucceededResult;
			ImportEngine.ImportState state = partManager.State;
			if (state != ImportEngine.ImportState.ImportsPreviewed && state != ImportEngine.ImportState.Composed)
			{
				return new CompositionResult(new CompositionError[] { ErrorBuilder.InvalidStateForRecompposition(partManager.Part) });
			}
			IEnumerable<ImportDefinition> affectedImports = ImportEngine.RecompositionManager.GetAffectedImports(partManager.Part, changedExports);
			bool flag = partManager.State == ImportEngine.ImportState.Composed;
			bool flag2 = false;
			foreach (ImportDefinition importDefinition in affectedImports)
			{
				compositionResult = compositionResult.MergeResult(this.TryRecomposeImport(partManager, flag, importDefinition, atomicComposition));
				flag2 = true;
			}
			if (compositionResult.Succeeded && flag2 && flag)
			{
				if (atomicComposition == null)
				{
					compositionResult = compositionResult.MergeResult(partManager.TryOnComposed());
				}
				else
				{
					atomicComposition.AddCompleteAction(delegate
					{
						partManager.TryOnComposed().ThrowOnErrors();
					});
				}
			}
			return compositionResult;
		}

		private CompositionResult TryRecomposeImport(ImportEngine.PartManager partManager, bool partComposed, ImportDefinition import, AtomicComposition atomicComposition)
		{
			if (partComposed && !import.IsRecomposable)
			{
				return new CompositionResult(new CompositionError[] { ErrorBuilder.PreventedByExistingImport(partManager.Part, import) });
			}
			CompositionResult<IEnumerable<Export>> compositionResult = ImportEngine.TryGetExports(this._sourceProvider, partManager.Part, import, atomicComposition);
			if (!compositionResult.Succeeded)
			{
				return compositionResult.ToResult();
			}
			Export[] exports = compositionResult.Value.AsArray<Export>();
			if (partComposed)
			{
				if (atomicComposition == null)
				{
					return partManager.TrySetImport(import, exports);
				}
				atomicComposition.AddCompleteAction(delegate
				{
					partManager.TrySetImport(import, exports).ThrowOnErrors();
				});
			}
			else
			{
				partManager.SetSavedImport(import, exports, atomicComposition);
			}
			return CompositionResult.SucceededResult;
		}

		private void StartSatisfyingImports(ImportEngine.PartManager partManager, AtomicComposition atomicComposition)
		{
			if (atomicComposition == null)
			{
				if (!partManager.TrackingImports)
				{
					partManager.TrackingImports = true;
					this._recompositionManager.AddPartToIndex(partManager);
					return;
				}
			}
			else
			{
				this.GetEngineContext(atomicComposition).AddPartManager(partManager);
			}
		}

		private void StopSatisfyingImports(ImportEngine.PartManager partManager, AtomicComposition atomicComposition)
		{
			if (atomicComposition == null)
			{
				this._partManagers.Remove(partManager.Part);
				partManager.DisposeAllDependencies();
				if (partManager.TrackingImports)
				{
					partManager.TrackingImports = false;
					this._recompositionManager.AddPartToUnindex(partManager);
					return;
				}
			}
			else
			{
				this.GetEngineContext(atomicComposition).RemovePartManager(partManager);
			}
		}

		private ImportEngine.PartManager GetPartManager(ComposablePart part, bool createIfNotpresent)
		{
			ImportEngine.PartManager partManager = null;
			using (this._lock.LockStateForRead())
			{
				if (this._partManagers.TryGetValue(part, out partManager))
				{
					return partManager;
				}
			}
			if (createIfNotpresent)
			{
				using (this._lock.LockStateForWrite())
				{
					if (!this._partManagers.TryGetValue(part, out partManager))
					{
						partManager = new ImportEngine.PartManager(this, part);
						this._partManagers.Add(part, partManager);
					}
				}
			}
			return partManager;
		}

		private ImportEngine.EngineContext GetEngineContext(AtomicComposition atomicComposition)
		{
			Assumes.NotNull<AtomicComposition>(atomicComposition);
			ImportEngine.EngineContext engineContext;
			if (!atomicComposition.TryGetValue<ImportEngine.EngineContext>(this, true, out engineContext))
			{
				ImportEngine.EngineContext engineContext2;
				atomicComposition.TryGetValue<ImportEngine.EngineContext>(this, false, out engineContext2);
				engineContext = new ImportEngine.EngineContext(this, engineContext2);
				atomicComposition.SetValue(this, engineContext);
				atomicComposition.AddCompleteAction(new Action(engineContext.Complete));
			}
			return engineContext;
		}

		private bool InPrerequisiteLoop()
		{
			ImportEngine.PartManager partManager = this._recursionStateStack.First<ImportEngine.PartManager>();
			ImportEngine.PartManager partManager2 = null;
			foreach (ImportEngine.PartManager partManager3 in this._recursionStateStack.Skip<ImportEngine.PartManager>(1))
			{
				if (partManager3.State == ImportEngine.ImportState.PreExportImportsSatisfying)
				{
					return true;
				}
				if (partManager3 == partManager)
				{
					partManager2 = partManager3;
					break;
				}
			}
			Assumes.IsTrue(partManager2 == partManager);
			return false;
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private static CompositionResult<IEnumerable<Export>> TryGetExports(ExportProvider provider, ComposablePart part, ImportDefinition definition, AtomicComposition atomicComposition)
		{
			CompositionResult<IEnumerable<Export>> compositionResult;
			try
			{
				compositionResult = new CompositionResult<IEnumerable<Export>>(provider.GetExports(definition, atomicComposition).AsArray<Export>());
			}
			catch (ImportCardinalityMismatchException ex)
			{
				CompositionException ex2 = new CompositionException(ErrorBuilder.CreateImportCardinalityMismatch(ex, definition));
				compositionResult = new CompositionResult<IEnumerable<Export>>(new CompositionError[] { ErrorBuilder.CreatePartCannotSetImport(part, definition, ex2) });
			}
			return compositionResult;
		}

		internal static bool IsRequiredImportForPreview(ImportDefinition import)
		{
			return import.Cardinality == ImportCardinality.ExactlyOne;
		}

		private const int MaximumNumberOfCompositionIterations = 100;

		private volatile bool _isDisposed;

		private ExportProvider _sourceProvider;

		private Stack<ImportEngine.PartManager> _recursionStateStack = new Stack<ImportEngine.PartManager>();

		private ConditionalWeakTable<ComposablePart, ImportEngine.PartManager> _partManagers = new ConditionalWeakTable<ComposablePart, ImportEngine.PartManager>();

		private ImportEngine.RecompositionManager _recompositionManager = new ImportEngine.RecompositionManager();

		private readonly CompositionLock _lock;

		private readonly CompositionOptions _compositionOptions;

		private class EngineContext
		{
			public EngineContext(ImportEngine importEngine, ImportEngine.EngineContext parentEngineContext)
			{
				this._importEngine = importEngine;
				this._parentEngineContext = parentEngineContext;
			}

			public void AddPartManager(ImportEngine.PartManager part)
			{
				Assumes.NotNull<ImportEngine.PartManager>(part);
				if (!this._removedPartManagers.Remove(part))
				{
					this._addedPartManagers.Add(part);
				}
			}

			public void RemovePartManager(ImportEngine.PartManager part)
			{
				Assumes.NotNull<ImportEngine.PartManager>(part);
				if (!this._addedPartManagers.Remove(part))
				{
					this._removedPartManagers.Add(part);
				}
			}

			public IEnumerable<ImportEngine.PartManager> GetAddedPartManagers()
			{
				if (this._parentEngineContext != null)
				{
					return this._addedPartManagers.ConcatAllowingNull<ImportEngine.PartManager>(this._parentEngineContext.GetAddedPartManagers());
				}
				return this._addedPartManagers;
			}

			public IEnumerable<ImportEngine.PartManager> GetRemovedPartManagers()
			{
				if (this._parentEngineContext != null)
				{
					return this._removedPartManagers.ConcatAllowingNull<ImportEngine.PartManager>(this._parentEngineContext.GetRemovedPartManagers());
				}
				return this._removedPartManagers;
			}

			public void Complete()
			{
				foreach (ImportEngine.PartManager partManager in this._addedPartManagers)
				{
					this._importEngine.StartSatisfyingImports(partManager, null);
				}
				foreach (ImportEngine.PartManager partManager2 in this._removedPartManagers)
				{
					this._importEngine.StopSatisfyingImports(partManager2, null);
				}
			}

			private ImportEngine _importEngine;

			private List<ImportEngine.PartManager> _addedPartManagers = new List<ImportEngine.PartManager>();

			private List<ImportEngine.PartManager> _removedPartManagers = new List<ImportEngine.PartManager>();

			private ImportEngine.EngineContext _parentEngineContext;
		}

		private class PartManager
		{
			public PartManager(ImportEngine importEngine, ComposablePart part)
			{
				this._importEngine = importEngine;
				this._part = part;
			}

			public ComposablePart Part
			{
				get
				{
					return this._part;
				}
			}

			public ImportEngine.ImportState State
			{
				get
				{
					ImportEngine.ImportState state;
					using (this._importEngine._lock.LockStateForRead())
					{
						state = this._state;
					}
					return state;
				}
				set
				{
					using (this._importEngine._lock.LockStateForWrite())
					{
						this._state = value;
					}
				}
			}

			public bool TrackingImports { get; set; }

			public IEnumerable<string> GetImportedContractNames()
			{
				if (this.Part == null)
				{
					return Enumerable.Empty<string>();
				}
				if (this._importedContractNames == null)
				{
					this._importedContractNames = this.Part.ImportDefinitions.Select<ImportDefinition, string>((ImportDefinition import) => import.ContractName ?? ImportDefinition.EmptyContractName).Distinct<string>().ToArray<string>();
				}
				return this._importedContractNames;
			}

			public CompositionResult TrySetImport(ImportDefinition import, IEnumerable<Export> exports)
			{
				CompositionResult compositionResult;
				try
				{
					this.Part.SetImport(import, exports);
					this.UpdateDisposableDependencies(import, exports);
					compositionResult = CompositionResult.SucceededResult;
				}
				catch (CompositionException ex)
				{
					compositionResult = new CompositionResult(new CompositionError[] { ErrorBuilder.CreatePartCannotSetImport(this.Part, import, ex) });
				}
				catch (ComposablePartException ex2)
				{
					compositionResult = new CompositionResult(new CompositionError[] { ErrorBuilder.CreatePartCannotSetImport(this.Part, import, ex2) });
				}
				return compositionResult;
			}

			public void SetSavedImport(ImportDefinition import, Export[] exports, AtomicComposition atomicComposition)
			{
				if (atomicComposition != null)
				{
					Export[] savedExports = this.GetSavedImport(import);
					atomicComposition.AddRevertAction(delegate
					{
						this.SetSavedImport(import, savedExports, null);
					});
				}
				if (this._importCache == null)
				{
					this._importCache = new Dictionary<ImportDefinition, Export[]>();
				}
				this._importCache[import] = exports;
			}

			public Export[] GetSavedImport(ImportDefinition import)
			{
				Export[] array = null;
				if (this._importCache != null)
				{
					this._importCache.TryGetValue(import, out array);
				}
				return array;
			}

			public void ClearSavedImports()
			{
				this._importCache = null;
			}

			public CompositionResult TryOnComposed()
			{
				CompositionResult compositionResult;
				try
				{
					this.Part.Activate();
					compositionResult = CompositionResult.SucceededResult;
				}
				catch (ComposablePartException ex)
				{
					compositionResult = new CompositionResult(new CompositionError[] { ErrorBuilder.CreatePartCannotActivate(this.Part, ex) });
				}
				return compositionResult;
			}

			public void UpdateDisposableDependencies(ImportDefinition import, IEnumerable<Export> exports)
			{
				List<IDisposable> list = null;
				foreach (IDisposable disposable2 in exports.OfType<IDisposable>())
				{
					if (list == null)
					{
						list = new List<IDisposable>();
					}
					list.Add(disposable2);
				}
				List<IDisposable> list2 = null;
				if (this._importedDisposableExports != null && this._importedDisposableExports.TryGetValue(import, out list2))
				{
					list2.ForEach(delegate(IDisposable disposable)
					{
						disposable.Dispose();
					});
					if (list == null)
					{
						this._importedDisposableExports.Remove(import);
						if (!this._importedDisposableExports.FastAny<KeyValuePair<ImportDefinition, List<IDisposable>>>())
						{
							this._importedDisposableExports = null;
						}
						return;
					}
				}
				if (list != null)
				{
					if (this._importedDisposableExports == null)
					{
						this._importedDisposableExports = new Dictionary<ImportDefinition, List<IDisposable>>();
					}
					this._importedDisposableExports[import] = list;
				}
			}

			public void DisposeAllDependencies()
			{
				if (this._importedDisposableExports != null)
				{
					IEnumerable<IDisposable> enumerable = this._importedDisposableExports.Values.SelectMany<List<IDisposable>, IDisposable>((List<IDisposable> exports) => exports);
					this._importedDisposableExports = null;
					enumerable.ForEach<IDisposable>(delegate(IDisposable disposableExport)
					{
						disposableExport.Dispose();
					});
				}
			}

			private Dictionary<ImportDefinition, List<IDisposable>> _importedDisposableExports;

			private Dictionary<ImportDefinition, Export[]> _importCache;

			private string[] _importedContractNames;

			private ComposablePart _part;

			private ImportEngine.ImportState _state;

			private readonly ImportEngine _importEngine;
		}

		private class RecompositionManager
		{
			public void AddPartToIndex(ImportEngine.PartManager partManager)
			{
				this._partsToIndex.Add(partManager);
			}

			public void AddPartToUnindex(ImportEngine.PartManager partManager)
			{
				this._partsToUnindex.Add(partManager);
			}

			public IEnumerable<ImportEngine.PartManager> GetAffectedParts(IEnumerable<string> changedContractNames)
			{
				this.UpdateImportIndex();
				List<ImportEngine.PartManager> list = new List<ImportEngine.PartManager>();
				list.AddRange(this.GetPartsImporting(ImportDefinition.EmptyContractName));
				foreach (string text in changedContractNames)
				{
					list.AddRange(this.GetPartsImporting(text));
				}
				return list;
			}

			public static IEnumerable<ImportDefinition> GetAffectedImports(ComposablePart part, IEnumerable<ExportDefinition> changedExports)
			{
				return part.ImportDefinitions.Where<ImportDefinition>((ImportDefinition import) => ImportEngine.RecompositionManager.IsAffectedImport(import, changedExports));
			}

			private static bool IsAffectedImport(ImportDefinition import, IEnumerable<ExportDefinition> changedExports)
			{
				foreach (ExportDefinition exportDefinition in changedExports)
				{
					if (import.IsConstraintSatisfiedBy(exportDefinition))
					{
						return true;
					}
				}
				return false;
			}

			public IEnumerable<ImportEngine.PartManager> GetPartsImporting(string contractName)
			{
				WeakReferenceCollection<ImportEngine.PartManager> weakReferenceCollection;
				if (!this._partManagerIndex.TryGetValue(contractName, out weakReferenceCollection))
				{
					return Enumerable.Empty<ImportEngine.PartManager>();
				}
				return weakReferenceCollection.AliveItemsToList();
			}

			private void AddIndexEntries(ImportEngine.PartManager partManager)
			{
				foreach (string text in partManager.GetImportedContractNames())
				{
					WeakReferenceCollection<ImportEngine.PartManager> weakReferenceCollection;
					if (!this._partManagerIndex.TryGetValue(text, out weakReferenceCollection))
					{
						weakReferenceCollection = new WeakReferenceCollection<ImportEngine.PartManager>();
						this._partManagerIndex.Add(text, weakReferenceCollection);
					}
					if (!weakReferenceCollection.Contains(partManager))
					{
						weakReferenceCollection.Add(partManager);
					}
				}
			}

			private void RemoveIndexEntries(ImportEngine.PartManager partManager)
			{
				foreach (string text in partManager.GetImportedContractNames())
				{
					WeakReferenceCollection<ImportEngine.PartManager> weakReferenceCollection;
					if (this._partManagerIndex.TryGetValue(text, out weakReferenceCollection))
					{
						weakReferenceCollection.Remove(partManager);
						if (weakReferenceCollection.AliveItemsToList().Count == 0)
						{
							this._partManagerIndex.Remove(text);
						}
					}
				}
			}

			private void UpdateImportIndex()
			{
				List<ImportEngine.PartManager> list = this._partsToIndex.AliveItemsToList();
				this._partsToIndex.Clear();
				List<ImportEngine.PartManager> list2 = this._partsToUnindex.AliveItemsToList();
				this._partsToUnindex.Clear();
				if (list.Count == 0 && list2.Count == 0)
				{
					return;
				}
				foreach (ImportEngine.PartManager partManager in list)
				{
					int num = list2.IndexOf(partManager);
					if (num >= 0)
					{
						list2[num] = null;
					}
					else
					{
						this.AddIndexEntries(partManager);
					}
				}
				foreach (ImportEngine.PartManager partManager2 in list2)
				{
					if (partManager2 != null)
					{
						this.RemoveIndexEntries(partManager2);
					}
				}
			}

			private WeakReferenceCollection<ImportEngine.PartManager> _partsToIndex = new WeakReferenceCollection<ImportEngine.PartManager>();

			private WeakReferenceCollection<ImportEngine.PartManager> _partsToUnindex = new WeakReferenceCollection<ImportEngine.PartManager>();

			private Dictionary<string, WeakReferenceCollection<ImportEngine.PartManager>> _partManagerIndex = new Dictionary<string, WeakReferenceCollection<ImportEngine.PartManager>>();
		}

		private enum ImportState
		{
			NoImportsSatisfied,
			ImportsPreviewing,
			ImportsPreviewed,
			PreExportImportsSatisfying,
			PreExportImportsSatisfied,
			PostExportImportsSatisfying,
			PostExportImportsSatisfied,
			ComposedNotifying,
			Composed
		}
	}
}
