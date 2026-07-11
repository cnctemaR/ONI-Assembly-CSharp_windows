using System;

namespace Unity.Burst
{
	/// <summary>
	///   <para>The BurstDiscard attribute lets you remove a method or property from being compiled to native code by the burst compiler.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
	public class BurstDiscardAttribute : Attribute
	{
	}
}
