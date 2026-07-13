using System;
using System.Runtime.CompilerServices;
using Mono.Cecil;

namespace MonoMod
{
	internal static class <24b3ba8a-00b7-40fc-a603-2711fa115297>MultiTargetShims
	{
		[NullableContext(1)]
		public static TypeReference GetConstraintType(this GenericParameterConstraint constraint)
		{
			return constraint.ConstraintType;
		}
	}
}
