using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Attribute used to make a float, int, or string variable in a script be delayed.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class DelayedAttribute : PropertyAttribute
	{
	}
}
