using System;
using Mono.Cecil;

internal static class MultiTargetShims
{
	public static TypeReference GetConstraintType(this GenericParameterConstraint constraint)
	{
		return constraint.ConstraintType;
	}

	private static readonly object[] _NoArgs = new object[0];
}
