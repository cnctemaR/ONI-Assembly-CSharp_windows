using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	public class CompositionBatch
	{
		public CompositionBatch()
			: this(null, null)
		{
		}

		public CompositionBatch(IEnumerable<ComposablePart> partsToAdd, IEnumerable<ComposablePart> partsToRemove)
		{
			this._partsToAdd = new List<ComposablePart>();
			if (partsToAdd != null)
			{
				foreach (ComposablePart composablePart in partsToAdd)
				{
					if (composablePart == null)
					{
						throw ExceptionBuilder.CreateContainsNullElement("partsToAdd");
					}
					this._partsToAdd.Add(composablePart);
				}
			}
			this._readOnlyPartsToAdd = this._partsToAdd.AsReadOnly();
			this._partsToRemove = new List<ComposablePart>();
			if (partsToRemove != null)
			{
				foreach (ComposablePart composablePart2 in partsToRemove)
				{
					if (composablePart2 == null)
					{
						throw ExceptionBuilder.CreateContainsNullElement("partsToRemove");
					}
					this._partsToRemove.Add(composablePart2);
				}
			}
			this._readOnlyPartsToRemove = this._partsToRemove.AsReadOnly();
		}

		public ReadOnlyCollection<ComposablePart> PartsToAdd
		{
			get
			{
				object @lock = this._lock;
				ReadOnlyCollection<ComposablePart> readOnlyPartsToAdd;
				lock (@lock)
				{
					this._copyNeededForAdd = true;
					readOnlyPartsToAdd = this._readOnlyPartsToAdd;
				}
				return readOnlyPartsToAdd;
			}
		}

		public ReadOnlyCollection<ComposablePart> PartsToRemove
		{
			get
			{
				object @lock = this._lock;
				ReadOnlyCollection<ComposablePart> readOnlyPartsToRemove;
				lock (@lock)
				{
					this._copyNeededForRemove = true;
					readOnlyPartsToRemove = this._readOnlyPartsToRemove;
				}
				return readOnlyPartsToRemove;
			}
		}

		public void AddPart(ComposablePart part)
		{
			Requires.NotNull<ComposablePart>(part, "part");
			object @lock = this._lock;
			lock (@lock)
			{
				if (this._copyNeededForAdd)
				{
					this._partsToAdd = new List<ComposablePart>(this._partsToAdd);
					this._readOnlyPartsToAdd = this._partsToAdd.AsReadOnly();
					this._copyNeededForAdd = false;
				}
				this._partsToAdd.Add(part);
			}
		}

		public void RemovePart(ComposablePart part)
		{
			Requires.NotNull<ComposablePart>(part, "part");
			object @lock = this._lock;
			lock (@lock)
			{
				if (this._copyNeededForRemove)
				{
					this._partsToRemove = new List<ComposablePart>(this._partsToRemove);
					this._readOnlyPartsToRemove = this._partsToRemove.AsReadOnly();
					this._copyNeededForRemove = false;
				}
				this._partsToRemove.Add(part);
			}
		}

		public ComposablePart AddExport(Export export)
		{
			Requires.NotNull<Export>(export, "export");
			ComposablePart composablePart = new CompositionBatch.SingleExportComposablePart(export);
			this.AddPart(composablePart);
			return composablePart;
		}

		private object _lock = new object();

		private bool _copyNeededForAdd;

		private bool _copyNeededForRemove;

		private List<ComposablePart> _partsToAdd;

		private ReadOnlyCollection<ComposablePart> _readOnlyPartsToAdd;

		private List<ComposablePart> _partsToRemove;

		private ReadOnlyCollection<ComposablePart> _readOnlyPartsToRemove;

		private class SingleExportComposablePart : ComposablePart
		{
			public SingleExportComposablePart(Export export)
			{
				Assumes.NotNull<Export>(export);
				this._export = export;
			}

			public override IDictionary<string, object> Metadata
			{
				get
				{
					return MetadataServices.EmptyMetadata;
				}
			}

			public override IEnumerable<ExportDefinition> ExportDefinitions
			{
				get
				{
					return new ExportDefinition[] { this._export.Definition };
				}
			}

			public override IEnumerable<ImportDefinition> ImportDefinitions
			{
				get
				{
					return Enumerable.Empty<ImportDefinition>();
				}
			}

			public override object GetExportedValue(ExportDefinition definition)
			{
				Requires.NotNull<ExportDefinition>(definition, "definition");
				if (definition != this._export.Definition)
				{
					throw ExceptionBuilder.CreateExportDefinitionNotOnThisComposablePart("definition");
				}
				return this._export.Value;
			}

			public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
			{
				Requires.NotNull<ImportDefinition>(definition, "definition");
				Requires.NotNullOrNullElements<Export>(exports, "exports");
				throw ExceptionBuilder.CreateImportDefinitionNotOnThisComposablePart("definition");
			}

			private readonly Export _export;
		}
	}
}
