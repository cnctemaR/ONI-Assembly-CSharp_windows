using System;
using UnityEngine.Scripting;

namespace Unity.Burst
{
	[AttributeUsage(AttributeTargets.Method)]
	[RequireAttributeUsages]
	public class BurstAuthorizedExternalMethodAttribute : Attribute
	{
	}
}
