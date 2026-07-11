using System;

namespace System.Linq.Expressions
{
	public interface IArgumentProvider
	{
		Expression GetArgument(int index);

		int ArgumentCount { get; }
	}
}
