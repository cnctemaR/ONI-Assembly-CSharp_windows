using System;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	[TypeDependency("System.SZArrayHelper")]
	public interface IReadOnlyCollection<out T> : IEnumerable<T>, IEnumerable
	{
		int Count { get; }
	}
}
