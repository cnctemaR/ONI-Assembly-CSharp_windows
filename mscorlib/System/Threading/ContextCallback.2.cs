using System;

namespace System.Threading
{
	internal delegate void ContextCallback<TState>(ref TState state);
}
