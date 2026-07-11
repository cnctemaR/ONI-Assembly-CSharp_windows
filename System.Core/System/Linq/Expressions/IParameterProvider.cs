using System;

namespace System.Linq.Expressions
{
	internal interface IParameterProvider
	{
		ParameterExpression GetParameter(int index);

		int ParameterCount { get; }
	}
}
