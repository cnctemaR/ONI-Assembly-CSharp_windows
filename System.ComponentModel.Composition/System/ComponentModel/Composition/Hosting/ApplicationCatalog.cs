using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	public class ApplicationCatalog : ComposablePartCatalog, ICompositionElement
	{
		public ApplicationCatalog()
		{
		}

		public ApplicationCatalog(ICompositionElement definitionOrigin)
		{
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this._definitionOrigin = definitionOrigin;
		}

		public ApplicationCatalog(ReflectionContext reflectionContext)
		{
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			this._reflectionContext = reflectionContext;
		}

		public ApplicationCatalog(ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
		{
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this._reflectionContext = reflectionContext;
			this._definitionOrigin = definitionOrigin;
		}

		internal ComposablePartCatalog CreateCatalog(string location, string pattern)
		{
			if (this._reflectionContext != null)
			{
				if (this._definitionOrigin == null)
				{
					return new DirectoryCatalog(location, pattern, this._reflectionContext);
				}
				return new DirectoryCatalog(location, pattern, this._reflectionContext, this._definitionOrigin);
			}
			else
			{
				if (this._definitionOrigin == null)
				{
					return new DirectoryCatalog(location, pattern);
				}
				return new DirectoryCatalog(location, pattern, this._definitionOrigin);
			}
		}

		private AggregateCatalog InnerCatalog
		{
			get
			{
				if (this._innerCatalog == null)
				{
					object thisLock = this._thisLock;
					lock (thisLock)
					{
						if (this._innerCatalog == null)
						{
							string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
							Assumes.NotNull<string>(baseDirectory);
							List<ComposablePartCatalog> list = new List<ComposablePartCatalog>();
							list.Add(this.CreateCatalog(baseDirectory, "*.exe"));
							list.Add(this.CreateCatalog(baseDirectory, "*.dll"));
							string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
							if (!string.IsNullOrEmpty(relativeSearchPath))
							{
								foreach (string text in relativeSearchPath.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
								{
									string text2 = Path.Combine(baseDirectory, text);
									if (Directory.Exists(text2))
									{
										list.Add(this.CreateCatalog(text2, "*.dll"));
									}
								}
							}
							AggregateCatalog aggregateCatalog = new AggregateCatalog(list);
							this._innerCatalog = aggregateCatalog;
						}
					}
				}
				return this._innerCatalog;
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this._isDisposed)
				{
					IDisposable disposable = null;
					object thisLock = this._thisLock;
					lock (thisLock)
					{
						disposable = this._innerCatalog;
						this._innerCatalog = null;
						this._isDisposed = true;
					}
					if (disposable != null)
					{
						disposable.Dispose();
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
			this.ThrowIfDisposed();
			return this.InnerCatalog.GetEnumerator();
		}

		public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ImportDefinition>(definition, "definition");
			return this.InnerCatalog.GetExports(definition);
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private string GetDisplayName()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} (Path=\"{1}\") (PrivateProbingPath=\"{2}\")", base.GetType().Name, AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath);
		}

		public override string ToString()
		{
			return this.GetDisplayName();
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

		private bool _isDisposed;

		private volatile AggregateCatalog _innerCatalog;

		private readonly object _thisLock = new object();

		private ICompositionElement _definitionOrigin;

		private ReflectionContext _reflectionContext;
	}
}
