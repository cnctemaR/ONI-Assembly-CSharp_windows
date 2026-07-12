using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Diagnostics;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	[DebuggerTypeProxy(typeof(DirectoryCatalog.DirectoryCatalogDebuggerProxy))]
	public class DirectoryCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged, ICompositionElement
	{
		public DirectoryCatalog(string path)
			: this(path, "*.dll")
		{
		}

		public DirectoryCatalog(string path, ReflectionContext reflectionContext)
			: this(path, "*.dll", reflectionContext)
		{
		}

		public DirectoryCatalog(string path, ICompositionElement definitionOrigin)
			: this(path, "*.dll", definitionOrigin)
		{
		}

		public DirectoryCatalog(string path, ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
			: this(path, "*.dll", reflectionContext, definitionOrigin)
		{
		}

		public DirectoryCatalog(string path, string searchPattern)
		{
			this._thisLock = new Lock();
			base..ctor();
			Requires.NotNullOrEmpty(path, "path");
			Requires.NotNullOrEmpty(searchPattern, "searchPattern");
			this._definitionOrigin = this;
			this.Initialize(path, searchPattern);
		}

		public DirectoryCatalog(string path, string searchPattern, ICompositionElement definitionOrigin)
		{
			this._thisLock = new Lock();
			base..ctor();
			Requires.NotNullOrEmpty(path, "path");
			Requires.NotNullOrEmpty(searchPattern, "searchPattern");
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this._definitionOrigin = definitionOrigin;
			this.Initialize(path, searchPattern);
		}

		public DirectoryCatalog(string path, string searchPattern, ReflectionContext reflectionContext)
		{
			this._thisLock = new Lock();
			base..ctor();
			Requires.NotNullOrEmpty(path, "path");
			Requires.NotNullOrEmpty(searchPattern, "searchPattern");
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			this._reflectionContext = reflectionContext;
			this._definitionOrigin = this;
			this.Initialize(path, searchPattern);
		}

		public DirectoryCatalog(string path, string searchPattern, ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
		{
			this._thisLock = new Lock();
			base..ctor();
			Requires.NotNullOrEmpty(path, "path");
			Requires.NotNullOrEmpty(searchPattern, "searchPattern");
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this._reflectionContext = reflectionContext;
			this._definitionOrigin = definitionOrigin;
			this.Initialize(path, searchPattern);
		}

		public string FullPath
		{
			get
			{
				return this._fullPath;
			}
		}

		public ReadOnlyCollection<string> LoadedFiles
		{
			get
			{
				ReadOnlyCollection<string> loadedFiles;
				using (new ReadLock(this._thisLock))
				{
					loadedFiles = this._loadedFiles;
				}
				return loadedFiles;
			}
		}

		public string Path
		{
			get
			{
				return this._path;
			}
		}

		public string SearchPattern
		{
			get
			{
				return this._searchPattern;
			}
		}

		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && !this._isDisposed)
				{
					bool flag = false;
					ComposablePartCatalogCollection composablePartCatalogCollection = null;
					try
					{
						using (new WriteLock(this._thisLock))
						{
							if (!this._isDisposed)
							{
								flag = true;
								composablePartCatalogCollection = this._catalogCollection;
								this._catalogCollection = null;
								this._assemblyCatalogs = null;
								this._isDisposed = true;
							}
						}
					}
					finally
					{
						if (composablePartCatalogCollection != null)
						{
							composablePartCatalogCollection.Dispose();
						}
						if (flag)
						{
							this._thisLock.Dispose();
						}
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override IEnumerator<ComposablePartDefinition> GetEnumerator()
		{
			return this._catalogCollection.SelectMany<ComposablePartCatalog, ComposablePartDefinition>((ComposablePartCatalog catalog) => catalog).GetEnumerator();
		}

		public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ImportDefinition>(definition, "definition");
			return this._catalogCollection.SelectMany<ComposablePartCatalog, Tuple<ComposablePartDefinition, ExportDefinition>>((ComposablePartCatalog catalog) => catalog.GetExports(definition));
		}

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

		public void Refresh()
		{
			this.ThrowIfDisposed();
			Assumes.NotNull<ReadOnlyCollection<string>>(this._loadedFiles);
			ComposablePartDefinition[] array2;
			ComposablePartDefinition[] array3;
			for (;;)
			{
				string[] files = this.GetFiles();
				object loadedFiles;
				string[] array;
				using (new ReadLock(this._thisLock))
				{
					loadedFiles = this._loadedFiles;
					array = this._loadedFiles.ToArray<string>();
				}
				List<Tuple<string, AssemblyCatalog>> list;
				List<Tuple<string, AssemblyCatalog>> list2;
				this.DiffChanges(array, files, out list, out list2);
				if (list.Count == 0 && list2.Count == 0)
				{
					break;
				}
				array2 = list.SelectMany<Tuple<string, AssemblyCatalog>, ComposablePartDefinition>((Tuple<string, AssemblyCatalog> cat) => cat.Item2).ToArray<ComposablePartDefinition>();
				array3 = list2.SelectMany<Tuple<string, AssemblyCatalog>, ComposablePartDefinition>((Tuple<string, AssemblyCatalog> cat) => cat.Item2).ToArray<ComposablePartDefinition>();
				using (AtomicComposition atomicComposition = new AtomicComposition())
				{
					ComposablePartCatalogChangeEventArgs e = new ComposablePartCatalogChangeEventArgs(array2, array3, atomicComposition);
					this.OnChanging(e);
					using (new WriteLock(this._thisLock))
					{
						if (loadedFiles != this._loadedFiles)
						{
							continue;
						}
						foreach (Tuple<string, AssemblyCatalog> tuple in list)
						{
							this._assemblyCatalogs.Add(tuple.Item1, tuple.Item2);
							this._catalogCollection.Add(tuple.Item2);
						}
						foreach (Tuple<string, AssemblyCatalog> tuple2 in list2)
						{
							this._assemblyCatalogs.Remove(tuple2.Item1);
							this._catalogCollection.Remove(tuple2.Item2);
						}
						this._loadedFiles = files.ToReadOnlyCollection<string>();
						atomicComposition.Complete();
					}
				}
				goto IL_01CF;
			}
			return;
			IL_01CF:
			ComposablePartCatalogChangeEventArgs e2 = new ComposablePartCatalogChangeEventArgs(array2, array3, null);
			this.OnChanged(e2);
		}

		public override string ToString()
		{
			return this.GetDisplayName();
		}

		private AssemblyCatalog CreateAssemblyCatalogGuarded(string assemblyFilePath)
		{
			Exception ex = null;
			try
			{
				return (this._reflectionContext != null) ? new AssemblyCatalog(assemblyFilePath, this._reflectionContext, this) : new AssemblyCatalog(assemblyFilePath, this);
			}
			catch (FileNotFoundException ex)
			{
			}
			catch (FileLoadException ex)
			{
			}
			catch (BadImageFormatException ex)
			{
			}
			catch (ReflectionTypeLoadException ex)
			{
			}
			CompositionTrace.AssemblyLoadFailed(this, assemblyFilePath, ex);
			return null;
		}

		private void DiffChanges(string[] beforeFiles, string[] afterFiles, out List<Tuple<string, AssemblyCatalog>> catalogsToAdd, out List<Tuple<string, AssemblyCatalog>> catalogsToRemove)
		{
			catalogsToAdd = new List<Tuple<string, AssemblyCatalog>>();
			catalogsToRemove = new List<Tuple<string, AssemblyCatalog>>();
			foreach (string text in afterFiles.Except<string>(beforeFiles))
			{
				AssemblyCatalog assemblyCatalog = this.CreateAssemblyCatalogGuarded(text);
				if (assemblyCatalog != null)
				{
					catalogsToAdd.Add(new Tuple<string, AssemblyCatalog>(text, assemblyCatalog));
				}
			}
			IEnumerable<string> enumerable = beforeFiles.Except<string>(afterFiles);
			using (new ReadLock(this._thisLock))
			{
				foreach (string text2 in enumerable)
				{
					AssemblyCatalog assemblyCatalog2;
					if (this._assemblyCatalogs.TryGetValue(text2, out assemblyCatalog2))
					{
						catalogsToRemove.Add(new Tuple<string, AssemblyCatalog>(text2, assemblyCatalog2));
					}
				}
			}
		}

		private string GetDisplayName()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} (Path=\"{1}\")", base.GetType().Name, this._path);
		}

		private string[] GetFiles()
		{
			return Directory.GetFiles(this._fullPath, this._searchPattern);
		}

		private static string GetFullPath(string path)
		{
			if (!global::System.IO.Path.IsPathRooted(path) && AppDomain.CurrentDomain.BaseDirectory != null)
			{
				path = global::System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
			}
			return global::System.IO.Path.GetFullPath(path);
		}

		private void Initialize(string path, string searchPattern)
		{
			this._path = path;
			this._fullPath = DirectoryCatalog.GetFullPath(path);
			this._searchPattern = searchPattern;
			this._assemblyCatalogs = new Dictionary<string, AssemblyCatalog>();
			this._catalogCollection = new ComposablePartCatalogCollection(null, null, null);
			this._loadedFiles = this.GetFiles().ToReadOnlyCollection<string>();
			foreach (string text in this._loadedFiles)
			{
				AssemblyCatalog assemblyCatalog = this.CreateAssemblyCatalogGuarded(text);
				if (assemblyCatalog != null)
				{
					this._assemblyCatalogs.Add(text, assemblyCatalog);
					this._catalogCollection.Add(assemblyCatalog);
				}
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

		string ICompositionElement.DisplayName
		{
			get
			{
				return this.GetDisplayName();
			}
		}

		ICompositionElement ICompositionElement.Origin
		{
			get
			{
				return null;
			}
		}

		private readonly Lock _thisLock;

		private readonly ICompositionElement _definitionOrigin;

		private ComposablePartCatalogCollection _catalogCollection;

		private Dictionary<string, AssemblyCatalog> _assemblyCatalogs;

		private volatile bool _isDisposed;

		private string _path;

		private string _fullPath;

		private string _searchPattern;

		private ReadOnlyCollection<string> _loadedFiles;

		private readonly ReflectionContext _reflectionContext;

		internal class DirectoryCatalogDebuggerProxy
		{
			public DirectoryCatalogDebuggerProxy(DirectoryCatalog catalog)
			{
				Requires.NotNull<DirectoryCatalog>(catalog, "catalog");
				this._catalog = catalog;
			}

			public ReadOnlyCollection<Assembly> Assemblies
			{
				get
				{
					return this._catalog._assemblyCatalogs.Values.Select<AssemblyCatalog, Assembly>((AssemblyCatalog catalog) => catalog.Assembly).ToReadOnlyCollection<Assembly>();
				}
			}

			public ReflectionContext ReflectionContext
			{
				get
				{
					return this._catalog._reflectionContext;
				}
			}

			public string SearchPattern
			{
				get
				{
					return this._catalog.SearchPattern;
				}
			}

			public string Path
			{
				get
				{
					return this._catalog._path;
				}
			}

			public string FullPath
			{
				get
				{
					return this._catalog._fullPath;
				}
			}

			public ReadOnlyCollection<string> LoadedFiles
			{
				get
				{
					return this._catalog._loadedFiles;
				}
			}

			public ReadOnlyCollection<ComposablePartDefinition> Parts
			{
				get
				{
					return this._catalog.Parts.ToReadOnlyCollection<ComposablePartDefinition>();
				}
			}

			private readonly DirectoryCatalog _catalog;
		}
	}
}
