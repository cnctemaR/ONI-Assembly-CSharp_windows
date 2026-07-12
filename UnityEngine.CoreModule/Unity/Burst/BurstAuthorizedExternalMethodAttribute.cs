using System;
using UnityEngine.Scripting;

namespace Unity.Burst
{
	[RequireAttributeUsages]
	[AttributeUsage(AttributeTargets.Method)]
	public class BurstAuthorizedExternalMethodAttribute : Attribute
	{
	}
}
