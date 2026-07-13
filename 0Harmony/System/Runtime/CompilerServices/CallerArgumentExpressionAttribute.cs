using System;

namespace System.Runtime.CompilerServices
{
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	internal sealed class CallerArgumentExpressionAttribute : Attribute
	{
		public CallerArgumentExpressionAttribute(string parameterName)
		{
			this.ParameterName = parameterName;
		}

		public string ParameterName { get; }
	}
}
