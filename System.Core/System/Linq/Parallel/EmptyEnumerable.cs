using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class EmptyEnumerable<T> : ParallelQuery<T>
	{
		private EmptyEnumerable()
			: base(QuerySettings.Empty)
		{
		}

		internal static EmptyEnumerable<T> Instance
		{
			get
			{
				if (EmptyEnumerable<T>.s_instance == null)
				{
					EmptyEnumerable<T>.s_instance = new EmptyEnumerable<T>();
				}
				return EmptyEnumerable<T>.s_instance;
			}
		}

		public override IEnumerator<T> GetEnumerator()
		{
			if (EmptyEnumerable<T>.s_enumeratorInstance == null)
			{
				EmptyEnumerable<T>.s_enumeratorInstance = new EmptyEnumerator<T>();
			}
			return EmptyEnumerable<T>.s_enumeratorInstance;
		}

		private static volatile EmptyEnumerable<T> s_instance;

		private static volatile EmptyEnumerator<T> s_enumeratorInstance;
	}
}
