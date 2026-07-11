using System;

namespace UnityEngine.Scripting
{
	/// <summary>
	///   <para>PreserveAttribute prevents byte code stripping from removing a class, method, field, or property.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
	public class PreserveAttribute : Attribute
	{
	}
}
