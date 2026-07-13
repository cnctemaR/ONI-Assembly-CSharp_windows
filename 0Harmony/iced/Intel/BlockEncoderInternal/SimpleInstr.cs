using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.BlockEncoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class SimpleInstr : Instr
	{
		public SimpleInstr(BlockEncoder blockEncoder, Block block, in Instruction instruction)
			: base(block, instruction.IP)
		{
			this.Done = true;
			this.instruction = instruction;
			this.Size = blockEncoder.GetInstructionSize(in instruction, instruction.IP);
		}

		public override void Initialize(BlockEncoder blockEncoder)
		{
		}

		public override bool Optimize(ulong gained)
		{
			return false;
		}

		[return: Nullable(2)]
		public override string TryEncode(Encoder encoder, out ConstantOffsets constantOffsets, out bool isOriginalInstruction)
		{
			isOriginalInstruction = true;
			uint num;
			string text;
			if (!encoder.TryEncode(in this.instruction, this.IP, out num, out text))
			{
				constantOffsets = default(ConstantOffsets);
				return Instr.CreateErrorMessage(text, in this.instruction);
			}
			constantOffsets = encoder.GetConstantOffsets();
			return null;
		}

		private Instruction instruction;
	}
}
