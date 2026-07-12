using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class LazyExportDefinition : ExportDefinition
	{
		public LazyExportDefinition(string contractName, Lazy<IDictionary<string, object>> metadata)
			: base(contractName, null)
		{
			this._metadata = metadata;
		}

		public override IDictionary<string, object> Metadata
		{
			get
			{
				return this._metadata.Value ?? MetadataServices.EmptyMetadata;
			}
		}

		private readonly Lazy<IDictionary<string, object>> _metadata;
	}
}
