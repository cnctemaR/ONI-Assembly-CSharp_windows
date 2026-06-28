using System;
using System.Diagnostics;

namespace System.Runtime.Versioning
{
	[Conditional("RESOURCE_ANNOTATION_WORK")]
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
	public sealed class ResourceExposureAttribute : Attribute
	{
		public ResourceExposureAttribute(ResourceScope exposureLevel)
		{
			this.exposure = exposureLevel;
		}

		public ResourceScope ResourceExposureLevel
		{
			get
			{
				return this.exposure;
			}
		}

		private ResourceScope exposure;
	}
}
