using System;

namespace Mono.Cecil
{
	[Flags]
	internal enum GenericParameterAttributes : ushort
	{
		VarianceMask = 3,
		NonVariant = 0,
		Covariant = 1,
		Contravariant = 2,
		SpecialConstraintMask = 28,
		ReferenceTypeConstraint = 4,
		NotNullableValueTypeConstraint = 8,
		DefaultConstructorConstraint = 16,
		AllowByRefLikeConstraint = 32
	}
}
