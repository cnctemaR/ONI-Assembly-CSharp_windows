using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Makes a variable not show up in the inspector but be serialized.</para>
	/// </summary>
	[UsedByNativeCode]
	public sealed class HideInInspector : Attribute
	{
	}
}
