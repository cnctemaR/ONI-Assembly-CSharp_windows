using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.BlockEncoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class Instr
	{
		protected Instr(Block block, ulong origIp)
		{
			this.OrigIP = origIp;
			this.Block = block;
		}

		public abstract void Initialize(BlockEncoder blockEncoder);

		public abstract bool Optimize(ulong gained);

		[return: Nullable(2)]
		public abstract string TryEncode(Encoder encoder, out ConstantOffsets constantOffsets, out bool isOriginalInstruction);

		protected static string CreateErrorMessage(string errorMessage, in Instruction instruction)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 3);
			defaultInterpolatedStringHandler.AppendFormatted(errorMessage);
			defaultInterpolatedStringHandler.AppendLiteral(" : 0x");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(instruction.IP, "X");
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted(instruction.ToString());
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		public static Instr Create(BlockEncoder blockEncoder, Block block, in Instruction instruction)
		{
			Code code = instruction.Code;
			if (code <= Code.Jmp_rel8_64)
			{
				if (code - Code.Jo_rel8_16 > 47)
				{
					if (code - Code.Xbegin_rel16 <= 1)
					{
						return new XbeginInstr(blockEncoder, block, in instruction);
					}
					switch (code)
					{
					case Code.Loopne_rel8_16_CX:
					case Code.Loopne_rel8_32_CX:
					case Code.Loopne_rel8_16_ECX:
					case Code.Loopne_rel8_32_ECX:
					case Code.Loopne_rel8_64_ECX:
					case Code.Loopne_rel8_16_RCX:
					case Code.Loopne_rel8_64_RCX:
					case Code.Loope_rel8_16_CX:
					case Code.Loope_rel8_32_CX:
					case Code.Loope_rel8_16_ECX:
					case Code.Loope_rel8_32_ECX:
					case Code.Loope_rel8_64_ECX:
					case Code.Loope_rel8_16_RCX:
					case Code.Loope_rel8_64_RCX:
					case Code.Loop_rel8_16_CX:
					case Code.Loop_rel8_32_CX:
					case Code.Loop_rel8_16_ECX:
					case Code.Loop_rel8_32_ECX:
					case Code.Loop_rel8_64_ECX:
					case Code.Loop_rel8_16_RCX:
					case Code.Loop_rel8_64_RCX:
					case Code.Jcxz_rel8_16:
					case Code.Jcxz_rel8_32:
					case Code.Jecxz_rel8_16:
					case Code.Jecxz_rel8_32:
					case Code.Jecxz_rel8_64:
					case Code.Jrcxz_rel8_16:
					case Code.Jrcxz_rel8_64:
						return new SimpleBranchInstr(blockEncoder, block, in instruction);
					case Code.In_AL_imm8:
					case Code.In_AX_imm8:
					case Code.In_EAX_imm8:
					case Code.Out_imm8_AL:
					case Code.Out_imm8_AX:
					case Code.Out_imm8_EAX:
					case Code.Jmp_ptr1616:
					case Code.Jmp_ptr1632:
						goto IL_013B;
					case Code.Call_rel16:
					case Code.Call_rel32_32:
					case Code.Call_rel32_64:
						return new CallInstr(blockEncoder, block, in instruction);
					case Code.Jmp_rel16:
					case Code.Jmp_rel32_32:
					case Code.Jmp_rel32_64:
					case Code.Jmp_rel8_16:
					case Code.Jmp_rel8_32:
					case Code.Jmp_rel8_64:
						return new JmpInstr(blockEncoder, block, in instruction);
					default:
						goto IL_013B;
					}
				}
			}
			else if (code - Code.Jo_rel16 > 47 && code - Code.VEX_KNC_Jkzd_kr_rel8_64 > 1 && code - Code.VEX_KNC_Jkzd_kr_rel32_64 > 1)
			{
				goto IL_013B;
			}
			return new JccInstr(blockEncoder, block, in instruction);
			IL_013B:
			if (blockEncoder.Bitness == 64)
			{
				int opCount = instruction.OpCount;
				int i = 0;
				while (i < opCount)
				{
					if (instruction.GetOpKind(i) == OpKind.Memory)
					{
						if (instruction.IsIPRelativeMemoryOperand)
						{
							return new IpRelMemOpInstr(blockEncoder, block, in instruction);
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			return new SimpleInstr(blockEncoder, block, in instruction);
		}

		[return: Nullable(2)]
		protected string EncodeBranchToPointerData(Encoder encoder, bool isCall, ulong ip, BlockData pointerData, out uint size, uint minSize)
		{
			if (minSize > 2147483647U)
			{
				throw new ArgumentOutOfRangeException("minSize");
			}
			Instruction instruction = default(Instruction);
			instruction.Op0Kind = OpKind.Memory;
			instruction.MemoryDisplSize = encoder.Bitness / 8;
			if (encoder.Bitness != 64)
			{
				throw new InvalidOperationException();
			}
			instruction.InternalSetCodeNoCheck(isCall ? Code.Call_rm64 : Code.Jmp_rm64);
			instruction.MemoryBase = Register.RIP;
			ulong num = ip + 6UL;
			long num2 = (long)(pointerData.Address - num);
			if (-2147483648L > num2 || num2 > 2147483647L)
			{
				size = 0U;
				return "Block is too big";
			}
			instruction.MemoryDisplacement64 = pointerData.Address;
			RelocKind relocKind = RelocKind.Offset64;
			string text;
			if (!encoder.TryEncode(in instruction, ip, out size, out text))
			{
				return text;
			}
			if (this.Block.CanAddRelocInfos && relocKind != RelocKind.Offset64)
			{
				ConstantOffsets constantOffsets = encoder.GetConstantOffsets();
				if (!constantOffsets.HasDisplacement)
				{
					return "Internal error: no displ";
				}
				this.Block.AddRelocInfo(new RelocInfo(relocKind, this.IP + (ulong)constantOffsets.DisplacementOffset));
			}
			while (size < minSize)
			{
				size += 1U;
				this.Block.CodeWriter.WriteByte(144);
			}
			return null;
		}

		protected static long CorrectDiff(bool inBlock, long diff, ulong gained)
		{
			if (inBlock && diff >= 0L)
			{
				return diff - (long)gained;
			}
			return diff;
		}

		protected static long ConvertDiffToBitnessDiff(int bitness, long diff)
		{
			long num;
			if (bitness != 16)
			{
				if (bitness != 32)
				{
					num = diff;
				}
				else
				{
					num = (long)((int)diff);
				}
			}
			else
			{
				num = (long)((short)diff);
			}
			return num;
		}

		public readonly Block Block;

		public uint Size;

		public ulong IP;

		public readonly ulong OrigIP;

		public bool Done;

		protected const uint CallOrJmpPointerDataInstructionSize64 = 6U;
	}
}
