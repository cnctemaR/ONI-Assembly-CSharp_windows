using System;

namespace Mono.Cecil
{
	internal sealed class FieldDefinitionProjection
	{
		public FieldDefinitionProjection(FieldDefinition field, FieldDefinitionTreatment treatment)
		{
			this.Attributes = field.Attributes;
			this.Treatment = treatment;
		}

		public readonly FieldAttributes Attributes;

		public readonly FieldDefinitionTreatment Treatment;
	}
}
