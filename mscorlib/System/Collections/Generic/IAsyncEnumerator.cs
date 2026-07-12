using System;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
	public interface IAsyncEnumerator<out T> : IAsyncDisposable
	{
		ValueTask<bool> MoveNextAsync();

		T Current { get; }
	}
}
