using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public interface ISetPropertyVisitor
	{
		void Visit<TContainer, TSet, TValue>(Property<TContainer, TSet> property, ref TContainer container, ref TSet set) where TSet : ISet<TValue>;
	}
}
