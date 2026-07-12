using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public interface ISetPropertyBagVisitor
	{
		void Visit<TSet, TValue>(ISetPropertyBag<TSet, TValue> properties, ref TSet container) where TSet : ISet<TValue>;
	}
}
