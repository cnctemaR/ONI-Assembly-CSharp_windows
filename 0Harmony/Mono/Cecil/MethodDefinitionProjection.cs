using System;

namespace Mono.Cecil
{
	internal sealed class MethodDefinitionProjection
	{
		public MethodDefinitionProjection(MethodDefinition method, MethodDefinitionTreatment treatment)
		{
			this.Attributes = method.Attributes;
			this.ImplAttributes = method.ImplAttributes;
			this.Name = method.Name;
			this.Treatment = treatment;
		}

		public readonly MethodAttributes Attributes;

		public readonly MethodImplAttributes ImplAttributes;

		public readonly string Name;

		public readonly MethodDefinitionTreatment Treatment;
	}
}
