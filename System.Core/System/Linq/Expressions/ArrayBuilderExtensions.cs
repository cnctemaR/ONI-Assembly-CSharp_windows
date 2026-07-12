using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
	internal static class ArrayBuilderExtensions
	{
		public static ReadOnlyCollection<T> ToReadOnly<T>(this global::System.Collections.Generic.ArrayBuilder<T> builder)
		{
			return new TrueReadOnlyCollection<T>(builder.ToArray());
		}
	}
}
