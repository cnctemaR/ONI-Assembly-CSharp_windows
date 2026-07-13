using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class OpMRBX : Op
	{
		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction, int operand)
		{
			if (!encoder.Verify(operand, OpKind.Memory, instruction.GetOpKind(operand)))
			{
				return;
			}
			Register memoryBase = instruction.MemoryBase;
			if (instruction.MemoryDisplSize != 0 || instruction.MemoryDisplacement64 != 0UL || instruction.MemoryIndexScale != 1 || instruction.MemoryIndex != Register.AL || (memoryBase != Register.BX && memoryBase != Register.EBX && memoryBase != Register.RBX))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Operand ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler.AppendLiteral(": Operand must be [bx+al], [ebx+al], or [rbx+al]");
				encoder.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				return;
			}
			int num;
			if (memoryBase == Register.RBX)
			{
				num = 8;
			}
			else if (memoryBase == Register.EBX)
			{
				num = 4;
			}
			else
			{
				num = 2;
			}
			encoder.SetAddrSize(num);
		}
	}
}
