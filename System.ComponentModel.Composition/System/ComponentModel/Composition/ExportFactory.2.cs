using System;

namespace System.ComponentModel.Composition
{
	public class ExportFactory<T, TMetadata> : ExportFactory<T>
	{
		public ExportFactory(Func<Tuple<T, Action>> exportLifetimeContextCreator, TMetadata metadata)
			: base(exportLifetimeContextCreator)
		{
			this._metadata = metadata;
		}

		public TMetadata Metadata
		{
			get
			{
				return this._metadata;
			}
		}

		private readonly TMetadata _metadata;
	}
}
