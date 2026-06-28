using System;
using System.ComponentModel;

namespace System.Data
{
	[Editor]
	[Serializable]
	internal delegate void DelegateValidateRemoveConstraint(ConstraintCollection sender, Constraint constraintToRemove, ref bool fail, ref string failReason);
}
