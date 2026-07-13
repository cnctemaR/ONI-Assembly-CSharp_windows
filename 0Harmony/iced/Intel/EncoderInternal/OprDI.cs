using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OprDI : Op
	{
		private static int GetRegSize(OpKind opKind)
		{
			if (opKind == OpKind.MemorySegRDI)
			{
				return 8;
			}
			if (opKind == OpKind.MemorySegEDI)
			{
				return 4;
			}
			if (opKind == OpKind.MemorySegDI)
			{
				return 2;
			}
			return 0;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			int regSize = OprDI.GetRegSize(instruction.GetOpKind(operand));
			if (regSize == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 4);
				defaultInterpolatedStringHandler.AppendLiteral("Operand ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler.AppendLiteral(": expected OpKind = ");
				defaultInterpolatedStringHandler.AppendFormatted("MemorySegDI");
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted("MemorySegEDI");
				defaultInterpolatedStringHandler.AppendLiteral(" or ");
				defaultInterpolatedStringHandler.AppendFormatted("MemorySegRDI");
				encoder.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				return;
			}
			encoder.SetAddrSize(regSize);
		}
	}
}
