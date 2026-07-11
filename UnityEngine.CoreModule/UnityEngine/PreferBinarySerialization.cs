using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Prefer ScriptableObject derived type to use binary serialization regardless of project's asset serialization mode.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class PreferBinarySerialization : Attribute
	{
	}
}
