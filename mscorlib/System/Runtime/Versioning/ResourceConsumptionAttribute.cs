using System;
using System.Diagnostics;

namespace System.Runtime.Versioning
{
	[Conditional("RESOURCE_ANNOTATION_WORK")]
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
	public sealed class ResourceConsumptionAttribute : Attribute
	{
		public ResourceConsumptionAttribute(ResourceScope resourceScope)
		{
			this._resourceScope = resourceScope;
			this._consumptionScope = this._resourceScope;
		}

		public ResourceConsumptionAttribute(ResourceScope resourceScope, ResourceScope consumptionScope)
		{
			this._resourceScope = resourceScope;
			this._consumptionScope = consumptionScope;
		}

		public ResourceScope ResourceScope
		{
			get
			{
				return this._resourceScope;
			}
		}

		public ResourceScope ConsumptionScope
		{
			get
			{
				return this._consumptionScope;
			}
		}

		private ResourceScope _consumptionScope;

		private ResourceScope _resourceScope;
	}
}
