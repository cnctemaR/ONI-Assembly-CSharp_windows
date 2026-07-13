using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpY : Op
	{
		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			int yregSize = OpX.GetYRegSize(instruction.GetOpKind(operand));
			if (yregSize == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 4);
				defaultInterpolatedStringHandler.AppendLiteral("Operand ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler.AppendLiteral(": expected OpKind = ");
				defaultInterpolatedStringHandler.AppendFormatted("MemoryESDI");
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted("MemoryESEDI");
				defaultInterpolatedStringHandler.AppendLiteral(" or ");
				defaultInterpolatedStringHandler.AppendFormatted("MemoryESRDI");
				encoder.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				return;
			}
			Code code = instruction.Code;
			if (code - Code.Cmpsb_m8_m8 <= 3)
			{
				int xregSize = OpX.GetXRegSize(instruction.Op0Kind);
				if (xregSize != yregSize)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(64, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("Same sized register must be used: reg #1 size = ");
					defaultInterpolatedStringHandler2.AppendFormatted<int>(xregSize * 8);
					defaultInterpolatedStringHandler2.AppendLiteral(", reg #2 size = ");
					defaultInterpolatedStringHandler2.AppendFormatted<int>(yregSize * 8);
					encoder.ErrorMessage = defaultInterpolatedStringHandler2.ToStringAndClear();
					return;
				}
			}
			encoder.SetAddrSize(yregSize);
		}
	}
}
