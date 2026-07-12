using System;

namespace System.ComponentModel.Composition
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class PartCreationPolicyAttribute : Attribute
	{
		public PartCreationPolicyAttribute(CreationPolicy creationPolicy)
		{
			this.CreationPolicy = creationPolicy;
		}

		public CreationPolicy CreationPolicy { get; private set; }

		internal static PartCreationPolicyAttribute Default = new PartCreationPolicyAttribute(CreationPolicy.Any);

		internal static PartCreationPolicyAttribute Shared = new PartCreationPolicyAttribute(CreationPolicy.Shared);
	}
}
