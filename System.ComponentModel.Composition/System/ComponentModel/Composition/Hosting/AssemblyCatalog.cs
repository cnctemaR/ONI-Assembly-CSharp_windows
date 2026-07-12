using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	[DebuggerTypeProxy(typeof(AssemblyCatalogDebuggerProxy))]
	public class AssemblyCatalog : ComposablePartCatalog, ICompositionElement
	{
		public AssemblyCatalog(string codeBase)
		{
			Requires.NotNullOrEmpty(codeBase, "codeBase");
			this.InitializeAssemblyCatalog(AssemblyCatalog.LoadAssembly(codeBase));
			this._definitionOrigin = this;
		}

		public AssemblyCatalog(string codeBase, ReflectionContext reflectionContext)
		{
			Requires.NotNullOrEmpty(codeBase, "codeBase");
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			this.InitializeAssemblyCatalog(AssemblyCatalog.LoadAssembly(codeBase));
			this._reflectionContext = reflectionContext;
			this._definitionOrigin = this;
		}

		public AssemblyCatalog(string codeBase, ICompositionElement definitionOrigin)
		{
			Requires.NotNullOrEmpty(codeBase, "codeBase");
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this.InitializeAssemblyCatalog(AssemblyCatalog.LoadAssembly(codeBase));
			this._definitionOrigin = definitionOrigin;
		}

		public AssemblyCatalog(string codeBase, ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
		{
			Requires.NotNullOrEmpty(codeBase, "codeBase");
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this.InitializeAssemblyCatalog(AssemblyCatalog.LoadAssembly(codeBase));
			this._reflectionContext = reflectionContext;
			this._definitionOrigin = definitionOrigin;
		}

		public AssemblyCatalog(Assembly assembly, ReflectionContext reflectionContext)
		{
			Requires.NotNull<Assembly>(assembly, "assembly");
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			this.InitializeAssemblyCatalog(assembly);
			this._reflectionContext = reflectionContext;
			this._definitionOrigin = this;
		}

		public AssemblyCatalog(Assembly assembly, ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
		{
			Requires.NotNull<Assembly>(assembly, "assembly");
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this.InitializeAssemblyCatalog(assembly);
			this._reflectionContext = reflectionContext;
			this._definitionOrigin = definitionOrigin;
		}

		public AssemblyCatalog(Assembly assembly)
		{
			Requires.NotNull<Assembly>(assembly, "assembly");
			this.InitializeAssemblyCatalog(assembly);
			this._definitionOrigin = this;
		}

		public AssemblyCatalog(Assembly assembly, ICompositionElement definitionOrigin)
		{
			Requires.NotNull<Assembly>(assembly, "assembly");
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this.InitializeAssemblyCatalog(assembly);
			this._definitionOrigin = definitionOrigin;
		}

		private void InitializeAssemblyCatalog(Assembly assembly)
		{
			this._assembly = assembly;
		}

		public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			return this.InnerCatalog.GetExports(definition);
		}

		private ComposablePartCatalog InnerCatalog
		{
			get
			{
				this.ThrowIfDisposed();
				if (this._innerCatalog == null)
				{
					CatalogReflectionContextAttribute firstAttribute = this._assembly.GetFirstAttribute<CatalogReflectionContextAttribute>();
					Assembly assembly = ((firstAttribute != null) ? firstAttribute.CreateReflectionContext().MapAssembly(this._assembly) : this._assembly);
					object thisLock = this._thisLock;
					lock (thisLock)
					{
						if (this._innerCatalog == null)
						{
							TypeCatalog typeCatalog = ((this._reflectionContext != null) ? new TypeCatalog(assembly.GetTypes(), this._reflectionContext, this._definitionOrigin) : new TypeCatalog(assembly.GetTypes(), this._definitionOrigin));
							Thread.MemoryBarrier();
							this._innerCatalog = typeCatalog;
						}
					}
				}
				return this._innerCatalog;
			}
		}

		public Assembly Assembly
		{
			get
			{
				return this._assembly;
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

		public override string ToString()
		{
			return this.GetDisplayName();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0 && disposing && this._innerCatalog != null)
				{
					this._innerCatalog.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override IEnumerator<ComposablePartDefinition> GetEnumerator()
		{
			return this.InnerCatalog.GetEnumerator();
		}

		private void ThrowIfDisposed()
		{
			if (this._isDisposed == 1)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private string GetDisplayName()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} (Assembly=\"{1}\")", base.GetType().Name, this.Assembly.FullName);
		}

		private static Assembly LoadAssembly(string codeBase)
		{
			Requires.NotNullOrEmpty(codeBase, "codeBase");
			AssemblyName assemblyName;
			try
			{
				assemblyName = AssemblyName.GetAssemblyName(codeBase);
			}
			catch (ArgumentException)
			{
				assemblyName = new AssemblyName();
				assemblyName.CodeBase = codeBase;
			}
			return Assembly.Load(assemblyName);
		}

		private readonly object _thisLock = new object();

		private readonly ICompositionElement _definitionOrigin;

		private volatile Assembly _assembly;

		private volatile ComposablePartCatalog _innerCatalog;

		private int _isDisposed;

		private ReflectionContext _reflectionContext;
	}
}
