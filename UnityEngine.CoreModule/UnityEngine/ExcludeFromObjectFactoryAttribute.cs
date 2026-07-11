using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Add this attribute to a class to prevent the class and its inherited classes from being created with ObjectFactory methods.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	[UsedByNativeCode]
	public class ExcludeFromObjectFactoryAttribute : Attribute
	{
	}
}
