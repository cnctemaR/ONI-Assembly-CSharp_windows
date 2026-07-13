using System;

namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class <1c2fb156-e9ba-45cc-af54-d5335bdb59af>MaybeNullWhenAttribute : Attribute
	{
		public bool ReturnValue { get; }

		public <1c2fb156-e9ba-45cc-af54-d5335bdb59af>MaybeNullWhenAttribute(bool returnValue)
		{
			this.ReturnValue = returnValue;
		}
	}
}
