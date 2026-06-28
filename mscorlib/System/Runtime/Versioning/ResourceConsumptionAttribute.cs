using System;
using System.Diagnostics;

namespace System.Runtime.Versioning
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
	[Conditional("RESOURCE_ANNOTATION_WORK")]
	public sealed class ResourceConsumptionAttribute : Attribute
	{
		public ResourceConsumptionAttribute(ResourceScope resourceScope)
		{
			this.resource = resourceScope;
			this.consumption = resourceScope;
		}

		public ResourceConsumptionAttribute(ResourceScope resourceScope, ResourceScope consumptionScope)
		{
			this.resource = resourceScope;
			this.consumption = consumptionScope;
		}

		public ResourceScope ConsumptionScope
		{
			get
			{
				return this.consumption;
			}
		}

		public ResourceScope ResourceScope
		{
			get
			{
				return this.resource;
			}
		}

		private ResourceScope resource;

		private ResourceScope consumption;
	}
}
