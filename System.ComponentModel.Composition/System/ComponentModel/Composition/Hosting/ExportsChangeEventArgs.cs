using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	public class ExportsChangeEventArgs : EventArgs
	{
		public ExportsChangeEventArgs(IEnumerable<ExportDefinition> addedExports, IEnumerable<ExportDefinition> removedExports, AtomicComposition atomicComposition)
		{
			Requires.NotNull<IEnumerable<ExportDefinition>>(addedExports, "addedExports");
			Requires.NotNull<IEnumerable<ExportDefinition>>(removedExports, "removedExports");
			this._addedExports = addedExports.AsArray<ExportDefinition>();
			this._removedExports = removedExports.AsArray<ExportDefinition>();
			this.AtomicComposition = atomicComposition;
		}

		public IEnumerable<ExportDefinition> AddedExports
		{
			get
			{
				return this._addedExports;
			}
		}

		public IEnumerable<ExportDefinition> RemovedExports
		{
			get
			{
				return this._removedExports;
			}
		}

		public IEnumerable<string> ChangedContractNames
		{
			get
			{
				if (this._changedContractNames == null)
				{
					this._changedContractNames = (from export in this.AddedExports.Concat<ExportDefinition>(this.RemovedExports)
						select export.ContractName).Distinct<string>().ToArray<string>();
				}
				return this._changedContractNames;
			}
		}

		public AtomicComposition AtomicComposition { get; private set; }

		private readonly IEnumerable<ExportDefinition> _addedExports;

		private readonly IEnumerable<ExportDefinition> _removedExports;

		private IEnumerable<string> _changedContractNames;
	}
}
