using System;
using UnityEngine.Scripting;

namespace UnityEngine.TestTools
{
	[UsedByNativeCode]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method)]
	public class ExcludeFromCoverageAttribute : Attribute
	{
	}
}
