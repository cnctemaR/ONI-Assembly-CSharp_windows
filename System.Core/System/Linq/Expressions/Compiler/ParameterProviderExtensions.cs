using System;

namespace System.Linq.Expressions.Compiler
{
	internal static class ParameterProviderExtensions
	{
		public static int IndexOf(this IParameterProvider provider, ParameterExpression parameter)
		{
			int i = 0;
			int parameterCount = provider.ParameterCount;
			while (i < parameterCount)
			{
				if (provider.GetParameter(i) == parameter)
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		public static bool Contains(this IParameterProvider provider, ParameterExpression parameter)
		{
			return provider.IndexOf(parameter) >= 0;
		}
	}
}
