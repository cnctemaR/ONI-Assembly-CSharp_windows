using System;
using UnityEngine.Scripting;

namespace Unity.Properties
{
	[RequireAttributeUsages]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class CreatePropertyAttribute : RequiredMemberAttribute
	{
		public bool ReadOnly { get; set; } = false;
	}
}
