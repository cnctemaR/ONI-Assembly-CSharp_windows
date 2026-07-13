using System;
using System.Runtime.CompilerServices;
using Mono.Cecil;

namespace MonoMod
{
	internal static class MultiTargetShims
	{
		[NullableContext(1)]
		public static TypeReference GetConstraintType(this GenericParameterConstraint constraint)
		{
			return constraint.ConstraintType;
		}
	}
}
