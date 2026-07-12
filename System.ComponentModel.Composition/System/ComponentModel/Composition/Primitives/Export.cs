using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
	public class Export
	{
		protected Export()
		{
		}

		public Export(string contractName, Func<object> exportedValueGetter)
			: this(new ExportDefinition(contractName, null), exportedValueGetter)
		{
		}

		public Export(string contractName, IDictionary<string, object> metadata, Func<object> exportedValueGetter)
			: this(new ExportDefinition(contractName, metadata), exportedValueGetter)
		{
		}

		public Export(ExportDefinition definition, Func<object> exportedValueGetter)
		{
			Requires.NotNull<ExportDefinition>(definition, "definition");
			Requires.NotNull<Func<object>>(exportedValueGetter, "exportedValueGetter");
			this._definition = definition;
			this._exportedValueGetter = exportedValueGetter;
		}

		public virtual ExportDefinition Definition
		{
			get
			{
				if (this._definition != null)
				{
					return this._definition;
				}
				throw ExceptionBuilder.CreateNotOverriddenByDerived("Definition");
			}
		}

		public IDictionary<string, object> Metadata
		{
			get
			{
				return this.Definition.Metadata;
			}
		}

		public object Value
		{
			get
			{
				if (this._exportedValue == Export._EmptyValue)
				{
					object exportedValueCore = this.GetExportedValueCore();
					Interlocked.CompareExchange(ref this._exportedValue, exportedValueCore, Export._EmptyValue);
				}
				return this._exportedValue;
			}
		}

		protected virtual object GetExportedValueCore()
		{
			if (this._exportedValueGetter != null)
			{
				return this._exportedValueGetter();
			}
			throw ExceptionBuilder.CreateNotOverriddenByDerived("GetExportedValueCore");
		}

		private readonly ExportDefinition _definition;

		private readonly Func<object> _exportedValueGetter;

		private static readonly object _EmptyValue = new object();

		private volatile object _exportedValue = Export._EmptyValue;
	}
}
