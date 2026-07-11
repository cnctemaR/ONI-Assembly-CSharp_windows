using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	/// <summary>
	///   <para>Used in conjunction with the ReadOnlyAttribute, WriteAccessRequiredAttribute lets you specify which struct method and property require write access to be invoked.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
	public class WriteAccessRequiredAttribute : Attribute
	{
	}
}
