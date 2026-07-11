using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Add this attribute to a class to prevent creating a Preset from the instances of the class.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	[UsedByNativeCode]
	public class ExcludeFromPresetAttribute : Attribute
	{
	}
}
