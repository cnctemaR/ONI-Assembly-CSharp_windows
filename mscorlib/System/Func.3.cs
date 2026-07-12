using System;

namespace System
{
	public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
}
