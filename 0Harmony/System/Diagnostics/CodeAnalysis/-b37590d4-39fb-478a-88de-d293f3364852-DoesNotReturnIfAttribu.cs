using System;

namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	internal sealed class <b37590d4-39fb-478a-88de-d293f3364852>DoesNotReturnIfAttribute : Attribute
	{
		public <b37590d4-39fb-478a-88de-d293f3364852>DoesNotReturnIfAttribute(bool parameterValue)
		{
			this.ParameterValue = parameterValue;
		}

		public bool ParameterValue { get; }
	}
}
