using System;

namespace System
{
	public delegate TResult Func<in T, out TResult>(T arg);
}
