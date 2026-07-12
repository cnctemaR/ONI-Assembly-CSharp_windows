using System;

namespace Mono.Cecil
{
	internal sealed class CustomAttributeValueProjection
	{
		public CustomAttributeValueProjection(AttributeTargets targets, CustomAttributeValueTreatment treatment)
		{
			this.Targets = targets;
			this.Treatment = treatment;
		}

		public readonly AttributeTargets Targets;

		public readonly CustomAttributeValueTreatment Treatment;
	}
}
