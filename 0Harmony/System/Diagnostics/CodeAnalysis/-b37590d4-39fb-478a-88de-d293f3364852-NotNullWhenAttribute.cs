using System;

namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	internal sealed class <b37590d4-39fb-478a-88de-d293f3364852>NotNullWhenAttribute : Attribute
	{
		public <b37590d4-39fb-478a-88de-d293f3364852>NotNullWhenAttribute(bool returnValue)
		{
			this.ReturnValue = returnValue;
		}

		public bool ReturnValue { get; }
	}
}
