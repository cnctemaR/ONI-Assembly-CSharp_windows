using System;

namespace Unity.Properties
{
	public interface ITypeVisitor
	{
		void Visit<TContainer>();
	}
}
