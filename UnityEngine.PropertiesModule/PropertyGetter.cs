using System;

namespace Unity.Properties
{
	public delegate TValue PropertyGetter<TContainer, out TValue>(ref TContainer container);
}
