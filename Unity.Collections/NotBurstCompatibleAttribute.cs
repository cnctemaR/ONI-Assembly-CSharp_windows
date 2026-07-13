using System;

namespace Unity.Collections
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
	public class NotBurstCompatibleAttribute : Attribute
	{
	}
}
