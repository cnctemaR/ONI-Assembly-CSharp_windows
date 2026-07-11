using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Parallel;

namespace System.Linq
{
	public class ParallelQuery : IEnumerable
	{
		internal ParallelQuery(QuerySettings specifiedSettings)
		{
			this._specifiedSettings = specifiedSettings;
		}

		internal QuerySettings SpecifiedQuerySettings
		{
			get
			{
				return this._specifiedSettings;
			}
		}

		[ExcludeFromCodeCoverage]
		internal virtual ParallelQuery<TCastTo> Cast<TCastTo>()
		{
			throw new NotSupportedException();
		}

		[ExcludeFromCodeCoverage]
		internal virtual ParallelQuery<TCastTo> OfType<TCastTo>()
		{
			throw new NotSupportedException();
		}

		[ExcludeFromCodeCoverage]
		internal virtual IEnumerator GetEnumeratorUntyped()
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumeratorUntyped();
		}

		private QuerySettings _specifiedSettings;
	}
}
