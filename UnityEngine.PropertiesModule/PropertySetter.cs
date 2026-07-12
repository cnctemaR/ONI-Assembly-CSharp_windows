using System;

namespace Unity.Properties
{
	public delegate void PropertySetter<TContainer, in TValue>(ref TContainer container, TValue value);
}
