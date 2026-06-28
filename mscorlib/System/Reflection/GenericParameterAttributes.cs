using System;

namespace System.Reflection
{
	[Flags]
	public enum GenericParameterAttributes
	{
		Covariant = 1,
		Contravariant = 2,
		VarianceMask = 3,
		None = 0,
		ReferenceTypeConstraint = 4,
		NotNullableValueTypeConstraint = 8,
		DefaultConstructorConstraint = 16,
		SpecialConstraintMask = 28
	}
}
