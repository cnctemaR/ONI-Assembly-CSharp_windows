using System;
using System.Threading;

namespace System
{
	[Serializable]
	public class Lazy<T, TMetadata> : Lazy<T>
	{
		public Lazy(Func<T> valueFactory, TMetadata metadata)
			: base(valueFactory)
		{
			this._metadata = metadata;
		}

		public Lazy(TMetadata metadata)
		{
			this._metadata = metadata;
		}

		public Lazy(TMetadata metadata, bool isThreadSafe)
			: base(isThreadSafe)
		{
			this._metadata = metadata;
		}

		public Lazy(Func<T> valueFactory, TMetadata metadata, bool isThreadSafe)
			: base(valueFactory, isThreadSafe)
		{
			this._metadata = metadata;
		}

		public Lazy(TMetadata metadata, LazyThreadSafetyMode mode)
			: base(mode)
		{
			this._metadata = metadata;
		}

		public Lazy(Func<T> valueFactory, TMetadata metadata, LazyThreadSafetyMode mode)
			: base(valueFactory, mode)
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

		private TMetadata _metadata;
	}
}
