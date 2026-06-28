using System;

namespace System.Collections.Generic
{
	public interface IEnumerator<T> : IEnumerator, IDisposable
	{
		T Current { get; }
	}
}
