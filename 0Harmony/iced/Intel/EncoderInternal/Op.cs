using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal abstract class Op
	{
		[NullableContext(1)]
		public abstract void Encode(Encoder encoder, in Instruction instruction, int operand);

		public virtual OpKind GetImmediateOpKind()
		{
			return (OpKind)(-1);
		}

		public virtual OpKind GetNearBranchOpKind()
		{
			return (OpKind)(-1);
		}

		public virtual OpKind GetFarBranchOpKind()
		{
			return (OpKind)(-1);
		}
	}
}
