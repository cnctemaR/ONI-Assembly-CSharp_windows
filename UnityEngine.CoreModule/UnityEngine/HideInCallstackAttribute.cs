using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class HideInCallstackAttribute : Attribute
	{
	}
}
