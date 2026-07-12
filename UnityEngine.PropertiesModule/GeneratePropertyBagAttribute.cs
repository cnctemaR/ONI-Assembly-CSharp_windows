using System;

namespace Unity.Properties
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class GeneratePropertyBagAttribute : Attribute
	{
	}
}
