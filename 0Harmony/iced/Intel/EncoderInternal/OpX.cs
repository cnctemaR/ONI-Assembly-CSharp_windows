using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpX : Op
	{
		internal static int GetXRegSize(OpKind opKind)
		{
			if (opKind == OpKind.MemorySegRSI)
			{
				return 8;
			}
			if (opKind == OpKind.MemorySegESI)
			{
				return 4;
			}
			if (opKind == OpKind.MemorySegSI)
			{
				return 2;
			}
			return 0;
		}

		internal static int GetYRegSize(OpKind opKind)
		{
			if (opKind == OpKind.MemoryESRDI)
			{
				return 8;
			}
			if (opKind == OpKind.MemoryESEDI)
			{
				return 4;
			}
			if (opKind == OpKind.MemoryESDI)
			{
				return 2;
			}
			return 0;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			int xregSize = OpX.GetXRegSize(instruction.GetOpKind(operand));
			if (xregSize == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 4);
				defaultInterpolatedStringHandler.AppendLiteral("Operand ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler.AppendLiteral(": expected OpKind = ");
				defaultInterpolatedStringHandler.AppendFormatted("MemorySegSI");
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted("MemorySegESI");
				defaultInterpolatedStringHandler.AppendLiteral(" or ");
				defaultInterpolatedStringHandler.AppendFormatted("MemorySegRSI");
				encoder.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				return;
			}
			Code code = instruction.Code;
			if (code - Code.Movsb_m8_m8 <= 3)
			{
				int yregSize = OpX.GetYRegSize(instruction.Op0Kind);
				if (xregSize != yregSize)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(64, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("Same sized register must be used: reg #1 size = ");
					defaultInterpolatedStringHandler2.AppendFormatted<int>(yregSize * 8);
					defaultInterpolatedStringHandler2.AppendLiteral(", reg #2 size = ");
					defaultInterpolatedStringHandler2.AppendFormatted<int>(xregSize * 8);
					encoder.ErrorMessage = defaultInterpolatedStringHandler2.ToStringAndClear();
					return;
				}
			}
			encoder.SetAddrSize(xregSize);
		}
	}
}
