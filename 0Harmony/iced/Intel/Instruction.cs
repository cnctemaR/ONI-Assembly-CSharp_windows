using System;
using System.Runtime.CompilerServices;
using Iced.Intel.EncoderInternal;

namespace Iced.Intel
{
	[NullableContext(1)]
	[Nullable(0)]
	internal struct Instruction : IEquatable<Instruction>
	{
		private static void InitializeSignedImmediate(ref Instruction instruction, int operand, long immediate)
		{
			OpKind immediateOpKind = Instruction.GetImmediateOpKind(instruction.Code, operand);
			instruction.SetOpKind(operand, immediateOpKind);
			switch (immediateOpKind)
			{
			case OpKind.Immediate8:
				if (-128L > immediate || immediate > 255L)
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.InternalImmediate8 = (uint)((byte)immediate);
				return;
			case OpKind.Immediate8_2nd:
				if (-128L > immediate || immediate > 255L)
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.InternalImmediate8_2nd = (uint)((byte)immediate);
				return;
			case OpKind.Immediate16:
				if (-32768L > immediate || immediate > 65535L)
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.InternalImmediate16 = (uint)((ushort)immediate);
				return;
			case OpKind.Immediate32:
				if (-2147483648L > immediate || immediate > (long)((ulong)(-1)))
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.Immediate32 = (uint)immediate;
				return;
			case OpKind.Immediate64:
				instruction.Immediate64 = (ulong)immediate;
				return;
			case OpKind.Immediate8to16:
				if (-128L > immediate || immediate > 127L)
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.InternalImmediate8 = (uint)((byte)immediate);
				return;
			case OpKind.Immediate8to32:
				if (-128L > immediate || immediate > 127L)
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.InternalImmediate8 = (uint)((byte)immediate);
				return;
			case OpKind.Immediate8to64:
				if (-128L > immediate || immediate > 127L)
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.InternalImmediate8 = (uint)((byte)immediate);
				return;
			case OpKind.Immediate32to64:
				if (-2147483648L > immediate || immediate > 2147483647L)
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.Immediate32 = (uint)immediate;
				return;
			default:
				throw new ArgumentOutOfRangeException("instruction");
			}
		}

		private static void InitializeUnsignedImmediate(ref Instruction instruction, int operand, ulong immediate)
		{
			OpKind immediateOpKind = Instruction.GetImmediateOpKind(instruction.Code, operand);
			instruction.SetOpKind(operand, immediateOpKind);
			switch (immediateOpKind)
			{
			case OpKind.Immediate8:
				if (immediate > 255UL)
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.InternalImmediate8 = (uint)((byte)immediate);
				return;
			case OpKind.Immediate8_2nd:
				if (immediate > 255UL)
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.InternalImmediate8_2nd = (uint)((byte)immediate);
				return;
			case OpKind.Immediate16:
				if (immediate > 65535UL)
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.InternalImmediate16 = (uint)((ushort)immediate);
				return;
			case OpKind.Immediate32:
				if (immediate > (ulong)(-1))
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.Immediate32 = (uint)immediate;
				return;
			case OpKind.Immediate64:
				instruction.Immediate64 = immediate;
				return;
			case OpKind.Immediate8to16:
				if (immediate > 127UL && (65408UL > immediate || immediate > 65535UL))
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.InternalImmediate8 = (uint)((byte)immediate);
				return;
			case OpKind.Immediate8to32:
				if (immediate > 127UL && ((ulong)(-128) > immediate || immediate > (ulong)(-1)))
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.InternalImmediate8 = (uint)((byte)immediate);
				return;
			case OpKind.Immediate8to64:
				if (immediate + 128UL > 255UL)
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.InternalImmediate8 = (uint)((byte)immediate);
				return;
			case OpKind.Immediate32to64:
				if (immediate + (ulong)(-2147483648) > (ulong)(-1))
				{
					throw new ArgumentOutOfRangeException("immediate");
				}
				instruction.Immediate32 = (uint)immediate;
				return;
			default:
				throw new ArgumentOutOfRangeException("instruction");
			}
		}

		private static OpKind GetImmediateOpKind(Code code, int operand)
		{
			OpCodeHandler[] handlers = OpCodeHandlers.Handlers;
			if (code >= (Code)handlers.Length)
			{
				throw new ArgumentOutOfRangeException("code");
			}
			Op[] operands = handlers[(int)code].Operands;
			if (operand >= operands.Length)
			{
				string text = "operand";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler.AppendFormatted<Code>(code);
				defaultInterpolatedStringHandler.AppendLiteral(" doesn't have at least ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand + 1);
				defaultInterpolatedStringHandler.AppendLiteral(" operands");
				throw new ArgumentOutOfRangeException(text, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			OpKind opKind = operands[operand].GetImmediateOpKind();
			if (opKind == OpKind.Immediate8 && operand > 0 && operand + 1 == operands.Length)
			{
				OpKind immediateOpKind = operands[operand - 1].GetImmediateOpKind();
				if (immediateOpKind == OpKind.Immediate8 || immediateOpKind == OpKind.Immediate16)
				{
					opKind = OpKind.Immediate8_2nd;
				}
			}
			if (opKind == (OpKind)(-1))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler2.AppendFormatted<Code>(code);
				defaultInterpolatedStringHandler2.AppendLiteral("'s op");
				defaultInterpolatedStringHandler2.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler2.AppendLiteral(" isn't an immediate operand");
				throw new ArgumentException(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			return opKind;
		}

		private static OpKind GetNearBranchOpKind(Code code, int operand)
		{
			OpCodeHandler[] handlers = OpCodeHandlers.Handlers;
			if (code >= (Code)handlers.Length)
			{
				throw new ArgumentOutOfRangeException("code");
			}
			Op[] operands = handlers[(int)code].Operands;
			if (operand >= operands.Length)
			{
				string text = "operand";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler.AppendFormatted<Code>(code);
				defaultInterpolatedStringHandler.AppendLiteral(" doesn't have at least ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand + 1);
				defaultInterpolatedStringHandler.AppendLiteral(" operands");
				throw new ArgumentOutOfRangeException(text, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			OpKind nearBranchOpKind = operands[operand].GetNearBranchOpKind();
			if (nearBranchOpKind == (OpKind)(-1))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(33, 2);
				defaultInterpolatedStringHandler2.AppendFormatted<Code>(code);
				defaultInterpolatedStringHandler2.AppendLiteral("'s op");
				defaultInterpolatedStringHandler2.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler2.AppendLiteral(" isn't a near branch operand");
				throw new ArgumentException(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			return nearBranchOpKind;
		}

		private static OpKind GetFarBranchOpKind(Code code, int operand)
		{
			OpCodeHandler[] handlers = OpCodeHandlers.Handlers;
			if (code >= (Code)handlers.Length)
			{
				throw new ArgumentOutOfRangeException("code");
			}
			Op[] operands = handlers[(int)code].Operands;
			if (operand >= operands.Length)
			{
				string text = "operand";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler.AppendFormatted<Code>(code);
				defaultInterpolatedStringHandler.AppendLiteral(" doesn't have at least ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand + 1);
				defaultInterpolatedStringHandler.AppendLiteral(" operands");
				throw new ArgumentOutOfRangeException(text, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			OpKind farBranchOpKind = operands[operand].GetFarBranchOpKind();
			if (farBranchOpKind == (OpKind)(-1))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler2.AppendFormatted<Code>(code);
				defaultInterpolatedStringHandler2.AppendLiteral("'s op");
				defaultInterpolatedStringHandler2.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler2.AppendLiteral(" isn't a far branch operand");
				throw new ArgumentException(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			return farBranchOpKind;
		}

		private static Instruction CreateString_Reg_SegRSI(Code code, int addressSize, Register register, Register segmentPrefix, RepPrefixKind repPrefix)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			if (repPrefix == RepPrefixKind.Repe)
			{
				instruction.InternalSetHasRepePrefix();
			}
			else if (repPrefix == RepPrefixKind.Repne)
			{
				instruction.InternalSetHasRepnePrefix();
			}
			instruction.Op0Register = register;
			if (addressSize == 64)
			{
				instruction.Op1Kind = OpKind.MemorySegRSI;
			}
			else if (addressSize == 32)
			{
				instruction.Op1Kind = OpKind.MemorySegESI;
			}
			else
			{
				if (addressSize != 16)
				{
					throw new ArgumentOutOfRangeException("addressSize");
				}
				instruction.Op1Kind = OpKind.MemorySegSI;
			}
			instruction.SegmentPrefix = segmentPrefix;
			return instruction;
		}

		private static Instruction CreateString_Reg_ESRDI(Code code, int addressSize, Register register, RepPrefixKind repPrefix)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			if (repPrefix == RepPrefixKind.Repe)
			{
				instruction.InternalSetHasRepePrefix();
			}
			else if (repPrefix == RepPrefixKind.Repne)
			{
				instruction.InternalSetHasRepnePrefix();
			}
			instruction.Op0Register = register;
			if (addressSize == 64)
			{
				instruction.Op1Kind = OpKind.MemoryESRDI;
			}
			else if (addressSize == 32)
			{
				instruction.Op1Kind = OpKind.MemoryESEDI;
			}
			else
			{
				if (addressSize != 16)
				{
					throw new ArgumentOutOfRangeException("addressSize");
				}
				instruction.Op1Kind = OpKind.MemoryESDI;
			}
			return instruction;
		}

		private static Instruction CreateString_ESRDI_Reg(Code code, int addressSize, Register register, RepPrefixKind repPrefix)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			if (repPrefix == RepPrefixKind.Repe)
			{
				instruction.InternalSetHasRepePrefix();
			}
			else if (repPrefix == RepPrefixKind.Repne)
			{
				instruction.InternalSetHasRepnePrefix();
			}
			if (addressSize == 64)
			{
				instruction.Op0Kind = OpKind.MemoryESRDI;
			}
			else if (addressSize == 32)
			{
				instruction.Op0Kind = OpKind.MemoryESEDI;
			}
			else
			{
				if (addressSize != 16)
				{
					throw new ArgumentOutOfRangeException("addressSize");
				}
				instruction.Op0Kind = OpKind.MemoryESDI;
			}
			instruction.Op1Register = register;
			return instruction;
		}

		private static Instruction CreateString_SegRSI_ESRDI(Code code, int addressSize, Register segmentPrefix, RepPrefixKind repPrefix)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			if (repPrefix == RepPrefixKind.Repe)
			{
				instruction.InternalSetHasRepePrefix();
			}
			else if (repPrefix == RepPrefixKind.Repne)
			{
				instruction.InternalSetHasRepnePrefix();
			}
			if (addressSize == 64)
			{
				instruction.Op0Kind = OpKind.MemorySegRSI;
				instruction.Op1Kind = OpKind.MemoryESRDI;
			}
			else if (addressSize == 32)
			{
				instruction.Op0Kind = OpKind.MemorySegESI;
				instruction.Op1Kind = OpKind.MemoryESEDI;
			}
			else
			{
				if (addressSize != 16)
				{
					throw new ArgumentOutOfRangeException("addressSize");
				}
				instruction.Op0Kind = OpKind.MemorySegSI;
				instruction.Op1Kind = OpKind.MemoryESDI;
			}
			instruction.SegmentPrefix = segmentPrefix;
			return instruction;
		}

		private static Instruction CreateString_ESRDI_SegRSI(Code code, int addressSize, Register segmentPrefix, RepPrefixKind repPrefix)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			if (repPrefix == RepPrefixKind.Repe)
			{
				instruction.InternalSetHasRepePrefix();
			}
			else if (repPrefix == RepPrefixKind.Repne)
			{
				instruction.InternalSetHasRepnePrefix();
			}
			if (addressSize == 64)
			{
				instruction.Op0Kind = OpKind.MemoryESRDI;
				instruction.Op1Kind = OpKind.MemorySegRSI;
			}
			else if (addressSize == 32)
			{
				instruction.Op0Kind = OpKind.MemoryESEDI;
				instruction.Op1Kind = OpKind.MemorySegESI;
			}
			else
			{
				if (addressSize != 16)
				{
					throw new ArgumentOutOfRangeException("addressSize");
				}
				instruction.Op0Kind = OpKind.MemoryESDI;
				instruction.Op1Kind = OpKind.MemorySegSI;
			}
			instruction.SegmentPrefix = segmentPrefix;
			return instruction;
		}

		private static Instruction CreateMaskmov(Code code, int addressSize, Register register1, Register register2, Register segmentPrefix)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			if (addressSize == 64)
			{
				instruction.Op0Kind = OpKind.MemorySegRDI;
			}
			else if (addressSize == 32)
			{
				instruction.Op0Kind = OpKind.MemorySegEDI;
			}
			else
			{
				if (addressSize != 16)
				{
					throw new ArgumentOutOfRangeException("addressSize");
				}
				instruction.Op0Kind = OpKind.MemorySegDI;
			}
			instruction.Op1Register = register1;
			instruction.Op2Register = register2;
			instruction.SegmentPrefix = segmentPrefix;
			return instruction;
		}

		private static void InitMemoryOperand(ref Instruction instruction, in MemoryOperand memory)
		{
			instruction.InternalMemoryBase = memory.Base;
			instruction.InternalMemoryIndex = memory.Index;
			instruction.MemoryIndexScale = memory.Scale;
			instruction.MemoryDisplSize = memory.DisplSize;
			instruction.MemoryDisplacement64 = (ulong)memory.Displacement;
			instruction.IsBroadcast = memory.IsBroadcast;
			instruction.SegmentPrefix = memory.SegmentPrefix;
		}

		public static Instruction Create(Code code)
		{
			return new Instruction
			{
				Code = code
			};
		}

		public static Instruction Create(Code code, Register register)
		{
			return new Instruction
			{
				Code = code,
				Op0Register = register
			};
		}

		public static Instruction Create(Code code, int immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			Instruction.InitializeSignedImmediate(ref instruction, 0, (long)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, uint immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			Instruction.InitializeUnsignedImmediate(ref instruction, 0, (ulong)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, in MemoryOperand memory)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2)
		{
			return new Instruction
			{
				Code = code,
				Op0Register = register1,
				Op1Register = register2
			};
		}

		public static Instruction Create(Code code, Register register, int immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register;
			Instruction.InitializeSignedImmediate(ref instruction, 1, (long)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register, uint immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register;
			Instruction.InitializeUnsignedImmediate(ref instruction, 1, (ulong)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register, long immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register;
			Instruction.InitializeSignedImmediate(ref instruction, 1, immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register, ulong immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register;
			Instruction.InitializeUnsignedImmediate(ref instruction, 1, immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register, in MemoryOperand memory)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register;
			instruction.Op1Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			return instruction;
		}

		public static Instruction Create(Code code, int immediate, Register register)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			Instruction.InitializeSignedImmediate(ref instruction, 0, (long)immediate);
			instruction.Op1Register = register;
			return instruction;
		}

		public static Instruction Create(Code code, uint immediate, Register register)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			Instruction.InitializeUnsignedImmediate(ref instruction, 0, (ulong)immediate);
			instruction.Op1Register = register;
			return instruction;
		}

		public static Instruction Create(Code code, int immediate1, int immediate2)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			Instruction.InitializeSignedImmediate(ref instruction, 0, (long)immediate1);
			Instruction.InitializeSignedImmediate(ref instruction, 1, (long)immediate2);
			return instruction;
		}

		public static Instruction Create(Code code, uint immediate1, uint immediate2)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			Instruction.InitializeUnsignedImmediate(ref instruction, 0, (ulong)immediate1);
			Instruction.InitializeUnsignedImmediate(ref instruction, 1, (ulong)immediate2);
			return instruction;
		}

		public static Instruction Create(Code code, in MemoryOperand memory, Register register)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			instruction.Op1Register = register;
			return instruction;
		}

		public static Instruction Create(Code code, in MemoryOperand memory, int immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			Instruction.InitializeSignedImmediate(ref instruction, 1, (long)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, in MemoryOperand memory, uint immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			Instruction.InitializeUnsignedImmediate(ref instruction, 1, (ulong)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, Register register3)
		{
			return new Instruction
			{
				Code = code,
				Op0Register = register1,
				Op1Register = register2,
				Op2Register = register3
			};
		}

		public static Instruction Create(Code code, Register register1, Register register2, int immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			Instruction.InitializeSignedImmediate(ref instruction, 2, (long)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, uint immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			Instruction.InitializeUnsignedImmediate(ref instruction, 2, (ulong)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, in MemoryOperand memory)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			return instruction;
		}

		public static Instruction Create(Code code, Register register, int immediate1, int immediate2)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register;
			Instruction.InitializeSignedImmediate(ref instruction, 1, (long)immediate1);
			Instruction.InitializeSignedImmediate(ref instruction, 2, (long)immediate2);
			return instruction;
		}

		public static Instruction Create(Code code, Register register, uint immediate1, uint immediate2)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register;
			Instruction.InitializeUnsignedImmediate(ref instruction, 1, (ulong)immediate1);
			Instruction.InitializeUnsignedImmediate(ref instruction, 2, (ulong)immediate2);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, in MemoryOperand memory, Register register2)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			instruction.Op2Register = register2;
			return instruction;
		}

		public static Instruction Create(Code code, Register register, in MemoryOperand memory, int immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register;
			instruction.Op1Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			Instruction.InitializeSignedImmediate(ref instruction, 2, (long)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register, in MemoryOperand memory, uint immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register;
			instruction.Op1Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			Instruction.InitializeUnsignedImmediate(ref instruction, 2, (ulong)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, in MemoryOperand memory, Register register1, Register register2)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			instruction.Op1Register = register1;
			instruction.Op2Register = register2;
			return instruction;
		}

		public static Instruction Create(Code code, in MemoryOperand memory, Register register, int immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			instruction.Op1Register = register;
			Instruction.InitializeSignedImmediate(ref instruction, 2, (long)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, in MemoryOperand memory, Register register, uint immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			instruction.Op1Register = register;
			Instruction.InitializeUnsignedImmediate(ref instruction, 2, (ulong)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, Register register3, Register register4)
		{
			return new Instruction
			{
				Code = code,
				Op0Register = register1,
				Op1Register = register2,
				Op2Register = register3,
				Op3Register = register4
			};
		}

		public static Instruction Create(Code code, Register register1, Register register2, Register register3, int immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Register = register3;
			Instruction.InitializeSignedImmediate(ref instruction, 3, (long)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, Register register3, uint immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Register = register3;
			Instruction.InitializeUnsignedImmediate(ref instruction, 3, (ulong)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, Register register3, in MemoryOperand memory)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Register = register3;
			instruction.Op3Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, int immediate1, int immediate2)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			Instruction.InitializeSignedImmediate(ref instruction, 2, (long)immediate1);
			Instruction.InitializeSignedImmediate(ref instruction, 3, (long)immediate2);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, uint immediate1, uint immediate2)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			Instruction.InitializeUnsignedImmediate(ref instruction, 2, (ulong)immediate1);
			Instruction.InitializeUnsignedImmediate(ref instruction, 3, (ulong)immediate2);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, in MemoryOperand memory, Register register3)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			instruction.Op3Register = register3;
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, in MemoryOperand memory, int immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			Instruction.InitializeSignedImmediate(ref instruction, 3, (long)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, in MemoryOperand memory, uint immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			Instruction.InitializeUnsignedImmediate(ref instruction, 3, (ulong)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, Register register3, Register register4, int immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Register = register3;
			instruction.Op3Register = register4;
			Instruction.InitializeSignedImmediate(ref instruction, 4, (long)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, Register register3, Register register4, uint immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Register = register3;
			instruction.Op3Register = register4;
			Instruction.InitializeUnsignedImmediate(ref instruction, 4, (ulong)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, Register register3, in MemoryOperand memory, int immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Register = register3;
			instruction.Op3Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			Instruction.InitializeSignedImmediate(ref instruction, 4, (long)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, Register register3, in MemoryOperand memory, uint immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Register = register3;
			instruction.Op3Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			Instruction.InitializeUnsignedImmediate(ref instruction, 4, (ulong)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, in MemoryOperand memory, Register register3, int immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			instruction.Op3Register = register3;
			Instruction.InitializeSignedImmediate(ref instruction, 4, (long)immediate);
			return instruction;
		}

		public static Instruction Create(Code code, Register register1, Register register2, in MemoryOperand memory, Register register3, uint immediate)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = code;
			instruction.Op0Register = register1;
			instruction.Op1Register = register2;
			instruction.Op2Kind = OpKind.Memory;
			Instruction.InitMemoryOperand(ref instruction, in memory);
			instruction.Op3Register = register3;
			Instruction.InitializeUnsignedImmediate(ref instruction, 4, (ulong)immediate);
			return instruction;
		}

		public static Instruction CreateBranch(Code code, ulong target)
		{
			return new Instruction
			{
				Code = code,
				Op0Kind = Instruction.GetNearBranchOpKind(code, 0),
				NearBranch64 = target
			};
		}

		public static Instruction CreateBranch(Code code, ushort selector, uint offset)
		{
			return new Instruction
			{
				Code = code,
				Op0Kind = Instruction.GetFarBranchOpKind(code, 0),
				FarBranchSelector = selector,
				FarBranch32 = offset
			};
		}

		public static Instruction CreateXbegin(int bitness, ulong target)
		{
			Instruction instruction = default(Instruction);
			if (bitness != 16)
			{
				if (bitness != 32)
				{
					if (bitness != 64)
					{
						throw new ArgumentOutOfRangeException("bitness");
					}
					instruction.Code = Code.Xbegin_rel32;
					instruction.Op0Kind = OpKind.NearBranch64;
					instruction.NearBranch64 = target;
				}
				else
				{
					instruction.Code = Code.Xbegin_rel32;
					instruction.Op0Kind = OpKind.NearBranch32;
					instruction.NearBranch32 = (uint)target;
				}
			}
			else
			{
				instruction.Code = Code.Xbegin_rel16;
				instruction.Op0Kind = OpKind.NearBranch32;
				instruction.NearBranch32 = (uint)target;
			}
			return instruction;
		}

		public static Instruction CreateOutsb(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Outsb_DX_m8, addressSize, Register.DX, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepOutsb(int addressSize)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Outsb_DX_m8, addressSize, Register.DX, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateOutsw(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Outsw_DX_m16, addressSize, Register.DX, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepOutsw(int addressSize)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Outsw_DX_m16, addressSize, Register.DX, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateOutsd(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Outsd_DX_m32, addressSize, Register.DX, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepOutsd(int addressSize)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Outsd_DX_m32, addressSize, Register.DX, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateLodsb(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Lodsb_AL_m8, addressSize, Register.AL, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepLodsb(int addressSize)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Lodsb_AL_m8, addressSize, Register.AL, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateLodsw(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Lodsw_AX_m16, addressSize, Register.AX, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepLodsw(int addressSize)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Lodsw_AX_m16, addressSize, Register.AX, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateLodsd(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Lodsd_EAX_m32, addressSize, Register.EAX, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepLodsd(int addressSize)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Lodsd_EAX_m32, addressSize, Register.EAX, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateLodsq(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Lodsq_RAX_m64, addressSize, Register.RAX, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepLodsq(int addressSize)
		{
			return Instruction.CreateString_Reg_SegRSI(Code.Lodsq_RAX_m64, addressSize, Register.RAX, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateScasb(int addressSize, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_Reg_ESRDI(Code.Scasb_AL_m8, addressSize, Register.AL, repPrefix);
		}

		public static Instruction CreateRepeScasb(int addressSize)
		{
			return Instruction.CreateString_Reg_ESRDI(Code.Scasb_AL_m8, addressSize, Register.AL, RepPrefixKind.Repe);
		}

		public static Instruction CreateRepneScasb(int addressSize)
		{
			return Instruction.CreateString_Reg_ESRDI(Code.Scasb_AL_m8, addressSize, Register.AL, RepPrefixKind.Repne);
		}

		public static Instruction CreateScasw(int addressSize, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_Reg_ESRDI(Code.Scasw_AX_m16, addressSize, Register.AX, repPrefix);
		}

		public static Instruction CreateRepeScasw(int addressSize)
		{
			return Instruction.CreateString_Reg_ESRDI(Code.Scasw_AX_m16, addressSize, Register.AX, RepPrefixKind.Repe);
		}

		public static Instruction CreateRepneScasw(int addressSize)
		{
			return Instruction.CreateString_Reg_ESRDI(Code.Scasw_AX_m16, addressSize, Register.AX, RepPrefixKind.Repne);
		}

		public static Instruction CreateScasd(int addressSize, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_Reg_ESRDI(Code.Scasd_EAX_m32, addressSize, Register.EAX, repPrefix);
		}

		public static Instruction CreateRepeScasd(int addressSize)
		{
			return Instruction.CreateString_Reg_ESRDI(Code.Scasd_EAX_m32, addressSize, Register.EAX, RepPrefixKind.Repe);
		}

		public static Instruction CreateRepneScasd(int addressSize)
		{
			return Instruction.CreateString_Reg_ESRDI(Code.Scasd_EAX_m32, addressSize, Register.EAX, RepPrefixKind.Repne);
		}

		public static Instruction CreateScasq(int addressSize, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_Reg_ESRDI(Code.Scasq_RAX_m64, addressSize, Register.RAX, repPrefix);
		}

		public static Instruction CreateRepeScasq(int addressSize)
		{
			return Instruction.CreateString_Reg_ESRDI(Code.Scasq_RAX_m64, addressSize, Register.RAX, RepPrefixKind.Repe);
		}

		public static Instruction CreateRepneScasq(int addressSize)
		{
			return Instruction.CreateString_Reg_ESRDI(Code.Scasq_RAX_m64, addressSize, Register.RAX, RepPrefixKind.Repne);
		}

		public static Instruction CreateInsb(int addressSize, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Insb_m8_DX, addressSize, Register.DX, repPrefix);
		}

		public static Instruction CreateRepInsb(int addressSize)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Insb_m8_DX, addressSize, Register.DX, RepPrefixKind.Repe);
		}

		public static Instruction CreateInsw(int addressSize, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Insw_m16_DX, addressSize, Register.DX, repPrefix);
		}

		public static Instruction CreateRepInsw(int addressSize)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Insw_m16_DX, addressSize, Register.DX, RepPrefixKind.Repe);
		}

		public static Instruction CreateInsd(int addressSize, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Insd_m32_DX, addressSize, Register.DX, repPrefix);
		}

		public static Instruction CreateRepInsd(int addressSize)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Insd_m32_DX, addressSize, Register.DX, RepPrefixKind.Repe);
		}

		public static Instruction CreateStosb(int addressSize, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Stosb_m8_AL, addressSize, Register.AL, repPrefix);
		}

		public static Instruction CreateRepStosb(int addressSize)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Stosb_m8_AL, addressSize, Register.AL, RepPrefixKind.Repe);
		}

		public static Instruction CreateStosw(int addressSize, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Stosw_m16_AX, addressSize, Register.AX, repPrefix);
		}

		public static Instruction CreateRepStosw(int addressSize)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Stosw_m16_AX, addressSize, Register.AX, RepPrefixKind.Repe);
		}

		public static Instruction CreateStosd(int addressSize, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Stosd_m32_EAX, addressSize, Register.EAX, repPrefix);
		}

		public static Instruction CreateRepStosd(int addressSize)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Stosd_m32_EAX, addressSize, Register.EAX, RepPrefixKind.Repe);
		}

		public static Instruction CreateStosq(int addressSize, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Stosq_m64_RAX, addressSize, Register.RAX, repPrefix);
		}

		public static Instruction CreateRepStosq(int addressSize)
		{
			return Instruction.CreateString_ESRDI_Reg(Code.Stosq_m64_RAX, addressSize, Register.RAX, RepPrefixKind.Repe);
		}

		public static Instruction CreateCmpsb(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_SegRSI_ESRDI(Code.Cmpsb_m8_m8, addressSize, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepeCmpsb(int addressSize)
		{
			return Instruction.CreateString_SegRSI_ESRDI(Code.Cmpsb_m8_m8, addressSize, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateRepneCmpsb(int addressSize)
		{
			return Instruction.CreateString_SegRSI_ESRDI(Code.Cmpsb_m8_m8, addressSize, Register.None, RepPrefixKind.Repne);
		}

		public static Instruction CreateCmpsw(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_SegRSI_ESRDI(Code.Cmpsw_m16_m16, addressSize, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepeCmpsw(int addressSize)
		{
			return Instruction.CreateString_SegRSI_ESRDI(Code.Cmpsw_m16_m16, addressSize, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateRepneCmpsw(int addressSize)
		{
			return Instruction.CreateString_SegRSI_ESRDI(Code.Cmpsw_m16_m16, addressSize, Register.None, RepPrefixKind.Repne);
		}

		public static Instruction CreateCmpsd(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_SegRSI_ESRDI(Code.Cmpsd_m32_m32, addressSize, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepeCmpsd(int addressSize)
		{
			return Instruction.CreateString_SegRSI_ESRDI(Code.Cmpsd_m32_m32, addressSize, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateRepneCmpsd(int addressSize)
		{
			return Instruction.CreateString_SegRSI_ESRDI(Code.Cmpsd_m32_m32, addressSize, Register.None, RepPrefixKind.Repne);
		}

		public static Instruction CreateCmpsq(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_SegRSI_ESRDI(Code.Cmpsq_m64_m64, addressSize, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepeCmpsq(int addressSize)
		{
			return Instruction.CreateString_SegRSI_ESRDI(Code.Cmpsq_m64_m64, addressSize, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateRepneCmpsq(int addressSize)
		{
			return Instruction.CreateString_SegRSI_ESRDI(Code.Cmpsq_m64_m64, addressSize, Register.None, RepPrefixKind.Repne);
		}

		public static Instruction CreateMovsb(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_ESRDI_SegRSI(Code.Movsb_m8_m8, addressSize, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepMovsb(int addressSize)
		{
			return Instruction.CreateString_ESRDI_SegRSI(Code.Movsb_m8_m8, addressSize, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateMovsw(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_ESRDI_SegRSI(Code.Movsw_m16_m16, addressSize, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepMovsw(int addressSize)
		{
			return Instruction.CreateString_ESRDI_SegRSI(Code.Movsw_m16_m16, addressSize, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateMovsd(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_ESRDI_SegRSI(Code.Movsd_m32_m32, addressSize, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepMovsd(int addressSize)
		{
			return Instruction.CreateString_ESRDI_SegRSI(Code.Movsd_m32_m32, addressSize, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateMovsq(int addressSize, Register segmentPrefix = Register.None, RepPrefixKind repPrefix = RepPrefixKind.None)
		{
			return Instruction.CreateString_ESRDI_SegRSI(Code.Movsq_m64_m64, addressSize, segmentPrefix, repPrefix);
		}

		public static Instruction CreateRepMovsq(int addressSize)
		{
			return Instruction.CreateString_ESRDI_SegRSI(Code.Movsq_m64_m64, addressSize, Register.None, RepPrefixKind.Repe);
		}

		public static Instruction CreateMaskmovq(int addressSize, Register register1, Register register2, Register segmentPrefix = Register.None)
		{
			return Instruction.CreateMaskmov(Code.Maskmovq_rDI_mm_mm, addressSize, register1, register2, segmentPrefix);
		}

		public static Instruction CreateMaskmovdqu(int addressSize, Register register1, Register register2, Register segmentPrefix = Register.None)
		{
			return Instruction.CreateMaskmov(Code.Maskmovdqu_rDI_xmm_xmm, addressSize, register1, register2, segmentPrefix);
		}

		public static Instruction CreateVmaskmovdqu(int addressSize, Register register1, Register register2, Register segmentPrefix = Register.None)
		{
			return Instruction.CreateMaskmov(Code.VEX_Vmaskmovdqu_rDI_xmm_xmm, addressSize, register1, register2, segmentPrefix);
		}

		public static Instruction CreateDeclareByte(byte b0)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 1U;
			instruction.SetDeclareByteValue(0, b0);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 2U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 3U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 4U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3, byte b4)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 5U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			instruction.SetDeclareByteValue(4, b4);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 6U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			instruction.SetDeclareByteValue(4, b4);
			instruction.SetDeclareByteValue(5, b5);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 7U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			instruction.SetDeclareByteValue(4, b4);
			instruction.SetDeclareByteValue(5, b5);
			instruction.SetDeclareByteValue(6, b6);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 8U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			instruction.SetDeclareByteValue(4, b4);
			instruction.SetDeclareByteValue(5, b5);
			instruction.SetDeclareByteValue(6, b6);
			instruction.SetDeclareByteValue(7, b7);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 9U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			instruction.SetDeclareByteValue(4, b4);
			instruction.SetDeclareByteValue(5, b5);
			instruction.SetDeclareByteValue(6, b6);
			instruction.SetDeclareByteValue(7, b7);
			instruction.SetDeclareByteValue(8, b8);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, byte b9)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 10U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			instruction.SetDeclareByteValue(4, b4);
			instruction.SetDeclareByteValue(5, b5);
			instruction.SetDeclareByteValue(6, b6);
			instruction.SetDeclareByteValue(7, b7);
			instruction.SetDeclareByteValue(8, b8);
			instruction.SetDeclareByteValue(9, b9);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, byte b9, byte b10)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 11U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			instruction.SetDeclareByteValue(4, b4);
			instruction.SetDeclareByteValue(5, b5);
			instruction.SetDeclareByteValue(6, b6);
			instruction.SetDeclareByteValue(7, b7);
			instruction.SetDeclareByteValue(8, b8);
			instruction.SetDeclareByteValue(9, b9);
			instruction.SetDeclareByteValue(10, b10);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, byte b9, byte b10, byte b11)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 12U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			instruction.SetDeclareByteValue(4, b4);
			instruction.SetDeclareByteValue(5, b5);
			instruction.SetDeclareByteValue(6, b6);
			instruction.SetDeclareByteValue(7, b7);
			instruction.SetDeclareByteValue(8, b8);
			instruction.SetDeclareByteValue(9, b9);
			instruction.SetDeclareByteValue(10, b10);
			instruction.SetDeclareByteValue(11, b11);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, byte b9, byte b10, byte b11, byte b12)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 13U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			instruction.SetDeclareByteValue(4, b4);
			instruction.SetDeclareByteValue(5, b5);
			instruction.SetDeclareByteValue(6, b6);
			instruction.SetDeclareByteValue(7, b7);
			instruction.SetDeclareByteValue(8, b8);
			instruction.SetDeclareByteValue(9, b9);
			instruction.SetDeclareByteValue(10, b10);
			instruction.SetDeclareByteValue(11, b11);
			instruction.SetDeclareByteValue(12, b12);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, byte b9, byte b10, byte b11, byte b12, byte b13)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 14U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			instruction.SetDeclareByteValue(4, b4);
			instruction.SetDeclareByteValue(5, b5);
			instruction.SetDeclareByteValue(6, b6);
			instruction.SetDeclareByteValue(7, b7);
			instruction.SetDeclareByteValue(8, b8);
			instruction.SetDeclareByteValue(9, b9);
			instruction.SetDeclareByteValue(10, b10);
			instruction.SetDeclareByteValue(11, b11);
			instruction.SetDeclareByteValue(12, b12);
			instruction.SetDeclareByteValue(13, b13);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, byte b9, byte b10, byte b11, byte b12, byte b13, byte b14)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 15U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			instruction.SetDeclareByteValue(4, b4);
			instruction.SetDeclareByteValue(5, b5);
			instruction.SetDeclareByteValue(6, b6);
			instruction.SetDeclareByteValue(7, b7);
			instruction.SetDeclareByteValue(8, b8);
			instruction.SetDeclareByteValue(9, b9);
			instruction.SetDeclareByteValue(10, b10);
			instruction.SetDeclareByteValue(11, b11);
			instruction.SetDeclareByteValue(12, b12);
			instruction.SetDeclareByteValue(13, b13);
			instruction.SetDeclareByteValue(14, b14);
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, byte b9, byte b10, byte b11, byte b12, byte b13, byte b14, byte b15)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = 16U;
			instruction.SetDeclareByteValue(0, b0);
			instruction.SetDeclareByteValue(1, b1);
			instruction.SetDeclareByteValue(2, b2);
			instruction.SetDeclareByteValue(3, b3);
			instruction.SetDeclareByteValue(4, b4);
			instruction.SetDeclareByteValue(5, b5);
			instruction.SetDeclareByteValue(6, b6);
			instruction.SetDeclareByteValue(7, b7);
			instruction.SetDeclareByteValue(8, b8);
			instruction.SetDeclareByteValue(9, b9);
			instruction.SetDeclareByteValue(10, b10);
			instruction.SetDeclareByteValue(11, b11);
			instruction.SetDeclareByteValue(12, b12);
			instruction.SetDeclareByteValue(13, b13);
			instruction.SetDeclareByteValue(14, b14);
			instruction.SetDeclareByteValue(15, b15);
			return instruction;
		}

		[NullableContext(0)]
		public unsafe static Instruction CreateDeclareByte(global::System.ReadOnlySpan<byte> data)
		{
			if (data.Length - 1 > 15)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_data();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = (uint)data.Length;
			for (int i = 0; i < data.Length; i++)
			{
				instruction.SetDeclareByteValue(i, *data[i]);
			}
			return instruction;
		}

		public static Instruction CreateDeclareByte(byte[] data)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			return Instruction.CreateDeclareByte(data, 0, data.Length);
		}

		public static Instruction CreateDeclareByte(byte[] data, int index, int length)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			if (length - 1 > 15)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_length();
			}
			if ((ulong)index + (ulong)length > (ulong)data.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareByte;
			instruction.InternalDeclareDataCount = (uint)length;
			for (int i = 0; i < length; i++)
			{
				instruction.SetDeclareByteValue(i, data[index + i]);
			}
			return instruction;
		}

		public static Instruction CreateDeclareWord(ushort w0)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareWord;
			instruction.InternalDeclareDataCount = 1U;
			instruction.SetDeclareWordValue(0, w0);
			return instruction;
		}

		public static Instruction CreateDeclareWord(ushort w0, ushort w1)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareWord;
			instruction.InternalDeclareDataCount = 2U;
			instruction.SetDeclareWordValue(0, w0);
			instruction.SetDeclareWordValue(1, w1);
			return instruction;
		}

		public static Instruction CreateDeclareWord(ushort w0, ushort w1, ushort w2)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareWord;
			instruction.InternalDeclareDataCount = 3U;
			instruction.SetDeclareWordValue(0, w0);
			instruction.SetDeclareWordValue(1, w1);
			instruction.SetDeclareWordValue(2, w2);
			return instruction;
		}

		public static Instruction CreateDeclareWord(ushort w0, ushort w1, ushort w2, ushort w3)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareWord;
			instruction.InternalDeclareDataCount = 4U;
			instruction.SetDeclareWordValue(0, w0);
			instruction.SetDeclareWordValue(1, w1);
			instruction.SetDeclareWordValue(2, w2);
			instruction.SetDeclareWordValue(3, w3);
			return instruction;
		}

		public static Instruction CreateDeclareWord(ushort w0, ushort w1, ushort w2, ushort w3, ushort w4)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareWord;
			instruction.InternalDeclareDataCount = 5U;
			instruction.SetDeclareWordValue(0, w0);
			instruction.SetDeclareWordValue(1, w1);
			instruction.SetDeclareWordValue(2, w2);
			instruction.SetDeclareWordValue(3, w3);
			instruction.SetDeclareWordValue(4, w4);
			return instruction;
		}

		public static Instruction CreateDeclareWord(ushort w0, ushort w1, ushort w2, ushort w3, ushort w4, ushort w5)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareWord;
			instruction.InternalDeclareDataCount = 6U;
			instruction.SetDeclareWordValue(0, w0);
			instruction.SetDeclareWordValue(1, w1);
			instruction.SetDeclareWordValue(2, w2);
			instruction.SetDeclareWordValue(3, w3);
			instruction.SetDeclareWordValue(4, w4);
			instruction.SetDeclareWordValue(5, w5);
			return instruction;
		}

		public static Instruction CreateDeclareWord(ushort w0, ushort w1, ushort w2, ushort w3, ushort w4, ushort w5, ushort w6)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareWord;
			instruction.InternalDeclareDataCount = 7U;
			instruction.SetDeclareWordValue(0, w0);
			instruction.SetDeclareWordValue(1, w1);
			instruction.SetDeclareWordValue(2, w2);
			instruction.SetDeclareWordValue(3, w3);
			instruction.SetDeclareWordValue(4, w4);
			instruction.SetDeclareWordValue(5, w5);
			instruction.SetDeclareWordValue(6, w6);
			return instruction;
		}

		public static Instruction CreateDeclareWord(ushort w0, ushort w1, ushort w2, ushort w3, ushort w4, ushort w5, ushort w6, ushort w7)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareWord;
			instruction.InternalDeclareDataCount = 8U;
			instruction.SetDeclareWordValue(0, w0);
			instruction.SetDeclareWordValue(1, w1);
			instruction.SetDeclareWordValue(2, w2);
			instruction.SetDeclareWordValue(3, w3);
			instruction.SetDeclareWordValue(4, w4);
			instruction.SetDeclareWordValue(5, w5);
			instruction.SetDeclareWordValue(6, w6);
			instruction.SetDeclareWordValue(7, w7);
			return instruction;
		}

		[NullableContext(0)]
		public unsafe static Instruction CreateDeclareWord(global::System.ReadOnlySpan<byte> data)
		{
			if (data.Length - 1 > 15 || (data.Length & 1) != 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_data();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareWord;
			instruction.InternalDeclareDataCount = (uint)(data.Length / 2);
			for (int i = 0; i < data.Length; i += 2)
			{
				uint num = (uint)((int)(*data[i]) | ((int)(*data[i + 1]) << 8));
				instruction.SetDeclareWordValue(i / 2, (ushort)num);
			}
			return instruction;
		}

		public static Instruction CreateDeclareWord(byte[] data)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			return Instruction.CreateDeclareWord(data, 0, data.Length);
		}

		public static Instruction CreateDeclareWord(byte[] data, int index, int length)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			if (length - 1 > 15 || (length & 1) != 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_length();
			}
			if ((ulong)index + (ulong)length > (ulong)data.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareWord;
			instruction.InternalDeclareDataCount = (uint)(length / 2);
			for (int i = 0; i < length; i += 2)
			{
				uint num = (uint)((int)data[index + i] | ((int)data[index + i + 1] << 8));
				instruction.SetDeclareWordValue(i / 2, (ushort)num);
			}
			return instruction;
		}

		[NullableContext(0)]
		public unsafe static Instruction CreateDeclareWord(global::System.ReadOnlySpan<ushort> data)
		{
			if (data.Length - 1 > 7)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_data();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareWord;
			instruction.InternalDeclareDataCount = (uint)data.Length;
			for (int i = 0; i < data.Length; i++)
			{
				instruction.SetDeclareWordValue(i, *data[i]);
			}
			return instruction;
		}

		public static Instruction CreateDeclareWord(ushort[] data)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			return Instruction.CreateDeclareWord(data, 0, data.Length);
		}

		public static Instruction CreateDeclareWord(ushort[] data, int index, int length)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			if (length - 1 > 7)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_length();
			}
			if ((ulong)index + (ulong)length > (ulong)data.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareWord;
			instruction.InternalDeclareDataCount = (uint)length;
			for (int i = 0; i < length; i++)
			{
				instruction.SetDeclareWordValue(i, data[index + i]);
			}
			return instruction;
		}

		public static Instruction CreateDeclareDword(uint d0)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareDword;
			instruction.InternalDeclareDataCount = 1U;
			instruction.SetDeclareDwordValue(0, d0);
			return instruction;
		}

		public static Instruction CreateDeclareDword(uint d0, uint d1)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareDword;
			instruction.InternalDeclareDataCount = 2U;
			instruction.SetDeclareDwordValue(0, d0);
			instruction.SetDeclareDwordValue(1, d1);
			return instruction;
		}

		public static Instruction CreateDeclareDword(uint d0, uint d1, uint d2)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareDword;
			instruction.InternalDeclareDataCount = 3U;
			instruction.SetDeclareDwordValue(0, d0);
			instruction.SetDeclareDwordValue(1, d1);
			instruction.SetDeclareDwordValue(2, d2);
			return instruction;
		}

		public static Instruction CreateDeclareDword(uint d0, uint d1, uint d2, uint d3)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareDword;
			instruction.InternalDeclareDataCount = 4U;
			instruction.SetDeclareDwordValue(0, d0);
			instruction.SetDeclareDwordValue(1, d1);
			instruction.SetDeclareDwordValue(2, d2);
			instruction.SetDeclareDwordValue(3, d3);
			return instruction;
		}

		[NullableContext(0)]
		public unsafe static Instruction CreateDeclareDword(global::System.ReadOnlySpan<byte> data)
		{
			if (data.Length - 1 > 15 || (data.Length & 3) != 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_data();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareDword;
			instruction.InternalDeclareDataCount = (uint)(data.Length / 4);
			for (int i = 0; i < data.Length; i += 4)
			{
				uint num = (uint)((int)(*data[i]) | ((int)(*data[i + 1]) << 8) | ((int)(*data[i + 2]) << 16) | ((int)(*data[i + 3]) << 24));
				instruction.SetDeclareDwordValue(i / 4, num);
			}
			return instruction;
		}

		public static Instruction CreateDeclareDword(byte[] data)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			return Instruction.CreateDeclareDword(data, 0, data.Length);
		}

		public static Instruction CreateDeclareDword(byte[] data, int index, int length)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			if (length - 1 > 15 || (length & 3) != 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_length();
			}
			if ((ulong)index + (ulong)length > (ulong)data.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareDword;
			instruction.InternalDeclareDataCount = (uint)(length / 4);
			for (int i = 0; i < length; i += 4)
			{
				uint num = (uint)((int)data[index + i] | ((int)data[index + i + 1] << 8) | ((int)data[index + i + 2] << 16) | ((int)data[index + i + 3] << 24));
				instruction.SetDeclareDwordValue(i / 4, num);
			}
			return instruction;
		}

		[NullableContext(0)]
		public unsafe static Instruction CreateDeclareDword(global::System.ReadOnlySpan<uint> data)
		{
			if (data.Length - 1 > 3)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_data();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareDword;
			instruction.InternalDeclareDataCount = (uint)data.Length;
			for (int i = 0; i < data.Length; i++)
			{
				instruction.SetDeclareDwordValue(i, *data[i]);
			}
			return instruction;
		}

		public static Instruction CreateDeclareDword(uint[] data)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			return Instruction.CreateDeclareDword(data, 0, data.Length);
		}

		public static Instruction CreateDeclareDword(uint[] data, int index, int length)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			if (length - 1 > 3)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_length();
			}
			if ((ulong)index + (ulong)length > (ulong)data.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareDword;
			instruction.InternalDeclareDataCount = (uint)length;
			for (int i = 0; i < length; i++)
			{
				instruction.SetDeclareDwordValue(i, data[index + i]);
			}
			return instruction;
		}

		public static Instruction CreateDeclareQword(ulong q0)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareQword;
			instruction.InternalDeclareDataCount = 1U;
			instruction.SetDeclareQwordValue(0, q0);
			return instruction;
		}

		public static Instruction CreateDeclareQword(ulong q0, ulong q1)
		{
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareQword;
			instruction.InternalDeclareDataCount = 2U;
			instruction.SetDeclareQwordValue(0, q0);
			instruction.SetDeclareQwordValue(1, q1);
			return instruction;
		}

		[NullableContext(0)]
		public unsafe static Instruction CreateDeclareQword(global::System.ReadOnlySpan<byte> data)
		{
			if (data.Length - 1 > 15 || (data.Length & 7) != 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_data();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareQword;
			instruction.InternalDeclareDataCount = (uint)(data.Length / 8);
			for (int i = 0; i < data.Length; i += 8)
			{
				uint num = (uint)((int)(*data[i]) | ((int)(*data[i + 1]) << 8) | ((int)(*data[i + 2]) << 16) | ((int)(*data[i + 3]) << 24));
				uint num2 = (uint)((int)(*data[i + 4]) | ((int)(*data[i + 5]) << 8) | ((int)(*data[i + 6]) << 16) | ((int)(*data[i + 7]) << 24));
				instruction.SetDeclareQwordValue(i / 8, (ulong)num | ((ulong)num2 << 32));
			}
			return instruction;
		}

		public static Instruction CreateDeclareQword(byte[] data)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			return Instruction.CreateDeclareQword(data, 0, data.Length);
		}

		public static Instruction CreateDeclareQword(byte[] data, int index, int length)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			if (length - 1 > 15 || (length & 7) != 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_length();
			}
			if ((ulong)index + (ulong)length > (ulong)data.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareQword;
			instruction.InternalDeclareDataCount = (uint)(length / 8);
			for (int i = 0; i < length; i += 8)
			{
				uint num = (uint)((int)data[index + i] | ((int)data[index + i + 1] << 8) | ((int)data[index + i + 2] << 16) | ((int)data[index + i + 3] << 24));
				uint num2 = (uint)((int)data[index + i + 4] | ((int)data[index + i + 5] << 8) | ((int)data[index + i + 6] << 16) | ((int)data[index + i + 7] << 24));
				instruction.SetDeclareQwordValue(i / 8, (ulong)num | ((ulong)num2 << 32));
			}
			return instruction;
		}

		[NullableContext(0)]
		public unsafe static Instruction CreateDeclareQword(global::System.ReadOnlySpan<ulong> data)
		{
			if (data.Length - 1 > 1)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_data();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareQword;
			instruction.InternalDeclareDataCount = (uint)data.Length;
			for (int i = 0; i < data.Length; i++)
			{
				instruction.SetDeclareQwordValue(i, (ulong)(*data[i]));
			}
			return instruction;
		}

		public static Instruction CreateDeclareQword(ulong[] data)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			return Instruction.CreateDeclareQword(data, 0, data.Length);
		}

		public static Instruction CreateDeclareQword(ulong[] data, int index, int length)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			if (length - 1 > 1)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_length();
			}
			if ((ulong)index + (ulong)length > (ulong)data.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			Instruction instruction = default(Instruction);
			instruction.Code = Code.DeclareQword;
			instruction.InternalDeclareDataCount = (uint)length;
			for (int i = 0; i < length; i++)
			{
				instruction.SetDeclareQwordValue(i, data[index + i]);
			}
			return instruction;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in Instruction left, in Instruction right)
		{
			return Instruction.EqualsInternal(in left, in right);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in Instruction left, in Instruction right)
		{
			return !Instruction.EqualsInternal(in left, in right);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Instruction other)
		{
			return Instruction.EqualsInternal(in this, in other);
		}

		readonly bool IEquatable<Instruction>.Equals(Instruction other)
		{
			return Instruction.EqualsInternal(in this, in other);
		}

		private static bool EqualsInternal(in Instruction a, in Instruction b)
		{
			return a.memDispl == b.memDispl && ((a.flags1 ^ b.flags1) & 4294180863U) == 0U && a.immediate == b.immediate && a.code == b.code && a.memBaseReg == b.memBaseReg && a.memIndexReg == b.memIndexReg && a.reg0 == b.reg0 && a.reg1 == b.reg1 && a.reg2 == b.reg2 && a.reg3 == b.reg3 && a.opKind0 == b.opKind0 && a.opKind1 == b.opKind1 && a.opKind2 == b.opKind2 && a.opKind3 == b.opKind3 && a.scale == b.scale && a.displSize == b.displSize && a.pad == b.pad;
		}

		public override readonly int GetHashCode()
		{
			return (int)((uint)this.memDispl ^ (uint)(this.memDispl >> 32) ^ (this.flags1 & 4294180863U) ^ this.immediate ^ (uint)((uint)this.code << 8) ^ (uint)((uint)this.memBaseReg << 16) ^ (uint)((uint)this.memIndexReg << 24) ^ (uint)this.reg3 ^ (uint)((uint)this.reg2 << 8) ^ (uint)((uint)this.reg1 << 16) ^ (uint)((uint)this.reg0 << 24) ^ (uint)this.opKind3 ^ (uint)((uint)this.opKind2 << 8) ^ (uint)((uint)this.opKind1 << 16) ^ (uint)((uint)this.opKind0 << 24) ^ (uint)this.scale ^ (uint)((uint)this.displSize << 8) ^ (uint)((uint)this.pad << 16));
		}

		[NullableContext(2)]
		public override readonly bool Equals(object obj)
		{
			if (obj is Instruction)
			{
				Instruction instruction = (Instruction)obj;
				return Instruction.EqualsInternal(in this, in instruction);
			}
			return false;
		}

		public static bool EqualsAllBits(in Instruction a, in Instruction b)
		{
			return a.nextRip == b.nextRip && a.memDispl == b.memDispl && a.flags1 == b.flags1 && a.immediate == b.immediate && a.code == b.code && a.memBaseReg == b.memBaseReg && a.memIndexReg == b.memIndexReg && a.reg0 == b.reg0 && a.reg1 == b.reg1 && a.reg2 == b.reg2 && a.reg3 == b.reg3 && a.opKind0 == b.opKind0 && a.opKind1 == b.opKind1 && a.opKind2 == b.opKind2 && a.opKind3 == b.opKind3 && a.scale == b.scale && a.displSize == b.displSize && a.len == b.len && a.pad == b.pad;
		}

		public ushort IP16
		{
			readonly get
			{
				return (ushort)((uint)this.nextRip - (uint)this.Length);
			}
			set
			{
				this.nextRip = (ulong)((int)value + this.Length);
			}
		}

		public uint IP32
		{
			readonly get
			{
				return (uint)this.nextRip - (uint)this.Length;
			}
			set
			{
				this.nextRip = (ulong)(value + (uint)this.Length);
			}
		}

		public ulong IP
		{
			readonly get
			{
				return this.nextRip - (ulong)this.Length;
			}
			set
			{
				this.nextRip = value + (ulong)this.Length;
			}
		}

		public ushort NextIP16
		{
			readonly get
			{
				return (ushort)this.nextRip;
			}
			set
			{
				this.nextRip = (ulong)value;
			}
		}

		public uint NextIP32
		{
			readonly get
			{
				return (uint)this.nextRip;
			}
			set
			{
				this.nextRip = (ulong)value;
			}
		}

		public ulong NextIP
		{
			readonly get
			{
				return this.nextRip;
			}
			set
			{
				this.nextRip = value;
			}
		}

		public CodeSize CodeSize
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (CodeSize)((this.flags1 >> 18) & 3U);
			}
			set
			{
				this.flags1 = (this.flags1 & 4294180863U) | (uint)((uint)(value & CodeSize.Code64) << 18);
			}
		}

		internal CodeSize InternalCodeSize
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.flags1 |= (uint)((uint)value << 18);
			}
		}

		public readonly bool IsInvalid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.code == 0;
			}
		}

		public Code Code
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (Code)this.code;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if (value >= (Code)4936)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_value();
				}
				this.code = (ushort)value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalSetCodeNoCheck(Code code)
		{
			this.code = (ushort)code;
		}

		public readonly Mnemonic Mnemonic
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Code.Mnemonic();
			}
		}

		public unsafe readonly int OpCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (int)(*InstructionOpCounts.OpCount[(int)this.code]);
			}
		}

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (int)this.len;
			}
			set
			{
				this.len = (byte)value;
			}
		}

		internal readonly bool Internal_HasRepeOrRepnePrefix
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.flags1 & 1610612736U) > 0U;
			}
		}

		internal readonly uint HasAnyOf_Lock_Rep_Repne_Prefix
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.flags1 & 3758096384U;
			}
		}

		private readonly bool IsXacquireInstr()
		{
			if (this.Op0Kind != OpKind.Memory)
			{
				return false;
			}
			if (this.HasLockPrefix)
			{
				return this.Code != Code.Cmpxchg16b_m128;
			}
			return this.Mnemonic == Mnemonic.Xchg;
		}

		private readonly bool IsXreleaseInstr()
		{
			if (this.Op0Kind != OpKind.Memory)
			{
				return false;
			}
			if (this.HasLockPrefix)
			{
				return this.Code != Code.Cmpxchg16b_m128;
			}
			Code code = this.Code;
			return code - Code.Xchg_rm8_r8 <= 7 || code == Code.Mov_rm8_imm8 || code - Code.Mov_rm16_imm16 <= 2;
		}

		public bool HasXacquirePrefix
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (this.flags1 & 1073741824U) != 0U && this.IsXacquireInstr();
			}
			set
			{
				if (value)
				{
					this.flags1 |= 1073741824U;
					return;
				}
				this.flags1 &= 3221225471U;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalSetHasXacquirePrefix()
		{
			this.flags1 |= 1073741824U;
		}

		public bool HasXreleasePrefix
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (this.flags1 & 536870912U) != 0U && this.IsXreleaseInstr();
			}
			set
			{
				if (value)
				{
					this.flags1 |= 536870912U;
					return;
				}
				this.flags1 &= 3758096383U;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalSetHasXreleasePrefix()
		{
			this.flags1 |= 536870912U;
		}

		public bool HasRepPrefix
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (this.flags1 & 536870912U) > 0U;
			}
			set
			{
				if (value)
				{
					this.flags1 |= 536870912U;
					return;
				}
				this.flags1 &= 3758096383U;
			}
		}

		public bool HasRepePrefix
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (this.flags1 & 536870912U) > 0U;
			}
			set
			{
				if (value)
				{
					this.flags1 |= 536870912U;
					return;
				}
				this.flags1 &= 3758096383U;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalSetHasRepePrefix()
		{
			this.flags1 = (this.flags1 & 3221225471U) | 536870912U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalClearHasRepePrefix()
		{
			this.flags1 &= 3758096383U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalClearHasRepeRepnePrefix()
		{
			this.flags1 &= 2684354559U;
		}

		public bool HasRepnePrefix
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (this.flags1 & 1073741824U) > 0U;
			}
			set
			{
				if (value)
				{
					this.flags1 |= 1073741824U;
					return;
				}
				this.flags1 &= 3221225471U;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalSetHasRepnePrefix()
		{
			this.flags1 = (this.flags1 & 3758096383U) | 1073741824U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalClearHasRepnePrefix()
		{
			this.flags1 &= 3221225471U;
		}

		public bool HasLockPrefix
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (this.flags1 & 2147483648U) > 0U;
			}
			set
			{
				if (value)
				{
					this.flags1 |= 2147483648U;
					return;
				}
				this.flags1 &= 2147483647U;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalSetHasLockPrefix()
		{
			this.flags1 |= 2147483648U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalClearHasLockPrefix()
		{
			this.flags1 &= 2147483647U;
		}

		public OpKind Op0Kind
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (OpKind)this.opKind0;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.opKind0 = (byte)value;
			}
		}

		internal readonly bool Internal_Op0IsNotReg_or_Op1IsNotReg
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.opKind0 | this.opKind1) > 0;
			}
		}

		public OpKind Op1Kind
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (OpKind)this.opKind1;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.opKind1 = (byte)value;
			}
		}

		public OpKind Op2Kind
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (OpKind)this.opKind2;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.opKind2 = (byte)value;
			}
		}

		public OpKind Op3Kind
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (OpKind)this.opKind3;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.opKind3 = (byte)value;
			}
		}

		public OpKind Op4Kind
		{
			readonly get
			{
				return OpKind.Immediate8;
			}
			set
			{
				if (value != OpKind.Immediate8)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_value();
				}
			}
		}

		public readonly OpKind GetOpKind(int operand)
		{
			switch (operand)
			{
			case 0:
				return this.Op0Kind;
			case 1:
				return this.Op1Kind;
			case 2:
				return this.Op2Kind;
			case 3:
				return this.Op3Kind;
			case 4:
				return this.Op4Kind;
			default:
				ThrowHelper.ThrowArgumentOutOfRangeException_operand();
				return OpKind.Register;
			}
		}

		public readonly bool HasOpKind(OpKind opKind)
		{
			for (int i = 0; i < this.OpCount; i++)
			{
				if (this.GetOpKind(i) == opKind)
				{
					return true;
				}
			}
			return false;
		}

		public void SetOpKind(int operand, OpKind opKind)
		{
			switch (operand)
			{
			case 0:
				this.Op0Kind = opKind;
				return;
			case 1:
				this.Op1Kind = opKind;
				return;
			case 2:
				this.Op2Kind = opKind;
				return;
			case 3:
				this.Op3Kind = opKind;
				return;
			case 4:
				this.Op4Kind = opKind;
				return;
			default:
				ThrowHelper.ThrowArgumentOutOfRangeException_operand();
				return;
			}
		}

		public readonly bool HasSegmentPrefix
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return ((this.flags1 >> 5) & 7U) - 1U < 6U;
			}
		}

		public Register SegmentPrefix
		{
			readonly get
			{
				uint num = ((this.flags1 >> 5) & 7U) - 1U;
				if (num >= 6U)
				{
					return Register.None;
				}
				return Register.ES + (int)num;
			}
			set
			{
				uint num;
				if (value == Register.None)
				{
					num = 0U;
				}
				else
				{
					num = (uint)((value - Register.ES + 1) & 7);
				}
				this.flags1 = (this.flags1 & 4294967071U) | (num << 5);
			}
		}

		public readonly Register MemorySegment
		{
			get
			{
				Register segmentPrefix = this.SegmentPrefix;
				if (segmentPrefix != Register.None)
				{
					return segmentPrefix;
				}
				Register memoryBase = this.MemoryBase;
				if (memoryBase == Register.BP || memoryBase == Register.EBP || memoryBase == Register.ESP || memoryBase == Register.RBP || memoryBase == Register.RSP)
				{
					return Register.SS;
				}
				return Register.DS;
			}
		}

		public int MemoryDisplSize
		{
			readonly get
			{
				int num;
				switch (this.displSize)
				{
				case 0:
					num = 0;
					break;
				case 1:
					num = 1;
					break;
				case 2:
					num = 2;
					break;
				case 3:
					num = 4;
					break;
				default:
					num = 8;
					break;
				}
				return num;
			}
			set
			{
				byte b;
				switch (value)
				{
				case 0:
					b = 0;
					goto IL_002E;
				case 1:
					b = 1;
					goto IL_002E;
				case 2:
					b = 2;
					goto IL_002E;
				case 4:
					b = 3;
					goto IL_002E;
				}
				b = 4;
				IL_002E:
				this.displSize = b;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalSetMemoryDisplSize(uint scale)
		{
			this.displSize = (byte)scale;
		}

		public bool IsBroadcast
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (this.flags1 & 67108864U) > 0U;
			}
			set
			{
				if (value)
				{
					this.flags1 |= 67108864U;
					return;
				}
				this.flags1 &= 4227858431U;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalSetIsBroadcast()
		{
			this.flags1 |= 67108864U;
		}

		public unsafe readonly MemorySize MemorySize
		{
			get
			{
				int num = (int)this.Code;
				if (this.IsBroadcast)
				{
					return (MemorySize)(*InstructionMemorySizes.SizesBcst[num]);
				}
				return (MemorySize)(*InstructionMemorySizes.SizesNormal[num]);
			}
		}

		public int MemoryIndexScale
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return 1 << (int)this.scale;
			}
			set
			{
				if (value == 1)
				{
					this.scale = 0;
					return;
				}
				if (value == 2)
				{
					this.scale = 1;
					return;
				}
				if (value == 4)
				{
					this.scale = 2;
					return;
				}
				this.scale = 3;
			}
		}

		internal int InternalMemoryIndexScale
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (int)this.scale;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.scale = (byte)value;
			}
		}

		public uint MemoryDisplacement32
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (uint)this.memDispl;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.memDispl = (ulong)value;
			}
		}

		public ulong MemoryDisplacement64
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.memDispl;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.memDispl = value;
			}
		}

		public readonly ulong GetImmediate(int operand)
		{
			ulong num;
			switch (this.GetOpKind(operand))
			{
			case OpKind.Immediate8:
				num = (ulong)this.Immediate8;
				break;
			case OpKind.Immediate8_2nd:
				num = (ulong)this.Immediate8_2nd;
				break;
			case OpKind.Immediate16:
				num = (ulong)this.Immediate16;
				break;
			case OpKind.Immediate32:
				num = (ulong)this.Immediate32;
				break;
			case OpKind.Immediate64:
				num = this.Immediate64;
				break;
			case OpKind.Immediate8to16:
				num = (ulong)((long)this.Immediate8to16);
				break;
			case OpKind.Immediate8to32:
				num = (ulong)((long)this.Immediate8to32);
				break;
			case OpKind.Immediate8to64:
				num = (ulong)this.Immediate8to64;
				break;
			case OpKind.Immediate32to64:
				num = (ulong)this.Immediate32to64;
				break;
			default:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Op");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler.AppendLiteral(" isn't an immediate operand");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "operand");
			}
			}
			return num;
		}

		public void SetImmediate(int operand, int immediate)
		{
			this.SetImmediate(operand, (ulong)((long)immediate));
		}

		public void SetImmediate(int operand, uint immediate)
		{
			this.SetImmediate(operand, (ulong)immediate);
		}

		public void SetImmediate(int operand, long immediate)
		{
			this.SetImmediate(operand, (ulong)immediate);
		}

		public void SetImmediate(int operand, ulong immediate)
		{
			switch (this.GetOpKind(operand))
			{
			case OpKind.Immediate8:
				this.Immediate8 = (byte)immediate;
				return;
			case OpKind.Immediate8_2nd:
				this.Immediate8_2nd = (byte)immediate;
				return;
			case OpKind.Immediate16:
				this.Immediate16 = (ushort)immediate;
				return;
			case OpKind.Immediate32:
				this.Immediate32 = (uint)immediate;
				return;
			case OpKind.Immediate64:
				this.Immediate64 = immediate;
				return;
			case OpKind.Immediate8to16:
				this.Immediate8to16 = (short)immediate;
				return;
			case OpKind.Immediate8to32:
				this.Immediate8to32 = (int)immediate;
				return;
			case OpKind.Immediate8to64:
				this.Immediate8to64 = (long)immediate;
				return;
			case OpKind.Immediate32to64:
				this.Immediate32to64 = (long)immediate;
				return;
			default:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Op");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler.AppendLiteral(" isn't an immediate operand");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "operand");
			}
			}
		}

		public byte Immediate8
		{
			readonly get
			{
				return (byte)this.immediate;
			}
			set
			{
				this.immediate = (uint)value;
			}
		}

		internal uint InternalImmediate8
		{
			set
			{
				this.immediate = value;
			}
		}

		public byte Immediate8_2nd
		{
			readonly get
			{
				return (byte)this.memDispl;
			}
			set
			{
				this.memDispl = (ulong)value;
			}
		}

		internal uint InternalImmediate8_2nd
		{
			set
			{
				this.memDispl = (ulong)value;
			}
		}

		public ushort Immediate16
		{
			readonly get
			{
				return (ushort)this.immediate;
			}
			set
			{
				this.immediate = (uint)value;
			}
		}

		internal uint InternalImmediate16
		{
			set
			{
				this.immediate = value;
			}
		}

		public uint Immediate32
		{
			readonly get
			{
				return this.immediate;
			}
			set
			{
				this.immediate = value;
			}
		}

		public ulong Immediate64
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (this.memDispl << 32) | (ulong)this.immediate;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.immediate = (uint)value;
				this.memDispl = (ulong)((uint)(value >> 32));
			}
		}

		internal uint InternalImmediate64_lo
		{
			set
			{
				this.immediate = value;
			}
		}

		internal uint InternalImmediate64_hi
		{
			set
			{
				this.memDispl = (ulong)value;
			}
		}

		public short Immediate8to16
		{
			readonly get
			{
				return (short)((sbyte)this.immediate);
			}
			set
			{
				this.immediate = (uint)((sbyte)value);
			}
		}

		public int Immediate8to32
		{
			readonly get
			{
				return (int)((sbyte)this.immediate);
			}
			set
			{
				this.immediate = (uint)((sbyte)value);
			}
		}

		public long Immediate8to64
		{
			readonly get
			{
				return (long)((sbyte)this.immediate);
			}
			set
			{
				this.immediate = (uint)((sbyte)value);
			}
		}

		public long Immediate32to64
		{
			readonly get
			{
				return (long)this.immediate;
			}
			set
			{
				this.immediate = (uint)value;
			}
		}

		public ushort NearBranch16
		{
			readonly get
			{
				return (ushort)this.memDispl;
			}
			set
			{
				this.memDispl = (ulong)value;
			}
		}

		internal uint InternalNearBranch16
		{
			set
			{
				this.memDispl = (ulong)value;
			}
		}

		public uint NearBranch32
		{
			readonly get
			{
				return (uint)this.memDispl;
			}
			set
			{
				this.memDispl = (ulong)value;
			}
		}

		public ulong NearBranch64
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.memDispl;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.memDispl = value;
			}
		}

		public readonly ulong NearBranchTarget
		{
			get
			{
				ulong num;
				switch (this.Op0Kind)
				{
				case OpKind.NearBranch16:
					num = (ulong)this.NearBranch16;
					break;
				case OpKind.NearBranch32:
					num = (ulong)this.NearBranch32;
					break;
				case OpKind.NearBranch64:
					num = this.NearBranch64;
					break;
				default:
					num = 0UL;
					break;
				}
				return num;
			}
		}

		public ushort FarBranch16
		{
			readonly get
			{
				return (ushort)this.immediate;
			}
			set
			{
				this.immediate = (uint)value;
			}
		}

		internal uint InternalFarBranch16
		{
			set
			{
				this.immediate = value;
			}
		}

		public uint FarBranch32
		{
			readonly get
			{
				return this.immediate;
			}
			set
			{
				this.immediate = value;
			}
		}

		public ushort FarBranchSelector
		{
			readonly get
			{
				return (ushort)this.memDispl;
			}
			set
			{
				this.memDispl = (ulong)value;
			}
		}

		internal uint InternalFarBranchSelector
		{
			set
			{
				this.memDispl = (ulong)value;
			}
		}

		public Register MemoryBase
		{
			readonly get
			{
				return (Register)this.memBaseReg;
			}
			set
			{
				this.memBaseReg = (byte)value;
			}
		}

		internal Register InternalMemoryBase
		{
			set
			{
				this.memBaseReg = (byte)value;
			}
		}

		public Register MemoryIndex
		{
			readonly get
			{
				return (Register)this.memIndexReg;
			}
			set
			{
				this.memIndexReg = (byte)value;
			}
		}

		internal Register InternalMemoryIndex
		{
			set
			{
				this.memIndexReg = (byte)value;
			}
		}

		public Register Op0Register
		{
			readonly get
			{
				return (Register)this.reg0;
			}
			set
			{
				this.reg0 = (byte)value;
			}
		}

		internal Register InternalOp0Register
		{
			set
			{
				this.reg0 = (byte)value;
			}
		}

		public Register Op1Register
		{
			readonly get
			{
				return (Register)this.reg1;
			}
			set
			{
				this.reg1 = (byte)value;
			}
		}

		internal Register InternalOp1Register
		{
			set
			{
				this.reg1 = (byte)value;
			}
		}

		public Register Op2Register
		{
			readonly get
			{
				return (Register)this.reg2;
			}
			set
			{
				this.reg2 = (byte)value;
			}
		}

		internal Register InternalOp2Register
		{
			set
			{
				this.reg2 = (byte)value;
			}
		}

		public Register Op3Register
		{
			readonly get
			{
				return (Register)this.reg3;
			}
			set
			{
				this.reg3 = (byte)value;
			}
		}

		internal Register InternalOp3Register
		{
			set
			{
				this.reg3 = (byte)value;
			}
		}

		public Register Op4Register
		{
			readonly get
			{
				return Register.None;
			}
			set
			{
				if (value != Register.None)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_value();
				}
			}
		}

		public readonly Register GetOpRegister(int operand)
		{
			switch (operand)
			{
			case 0:
				return this.Op0Register;
			case 1:
				return this.Op1Register;
			case 2:
				return this.Op2Register;
			case 3:
				return this.Op3Register;
			case 4:
				return this.Op4Register;
			default:
				ThrowHelper.ThrowArgumentOutOfRangeException_operand();
				return Register.None;
			}
		}

		public void SetOpRegister(int operand, Register register)
		{
			switch (operand)
			{
			case 0:
				this.Op0Register = register;
				return;
			case 1:
				this.Op1Register = register;
				return;
			case 2:
				this.Op2Register = register;
				return;
			case 3:
				this.Op3Register = register;
				return;
			case 4:
				this.Op4Register = register;
				return;
			default:
				ThrowHelper.ThrowArgumentOutOfRangeException_operand();
				return;
			}
		}

		public Register OpMask
		{
			readonly get
			{
				int num = (int)((this.flags1 >> 15) & 7U);
				if (num != 0)
				{
					return num + Register.K0;
				}
				return Register.None;
			}
			set
			{
				uint num;
				if (value == Register.None)
				{
					num = 0U;
				}
				else
				{
					num = (uint)((value - Register.K0) & 7);
				}
				this.flags1 = (this.flags1 & 4294737919U) | (num << 15);
			}
		}

		internal uint InternalOpMask
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (this.flags1 >> 15) & 7U;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.flags1 |= value << 15;
			}
		}

		public readonly bool HasOpMask
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.flags1 & 229376U) > 0U;
			}
		}

		internal readonly bool HasOpMask_or_ZeroingMasking
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.flags1 & 268664832U) > 0U;
			}
		}

		public bool ZeroingMasking
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (this.flags1 & 268435456U) > 0U;
			}
			set
			{
				if (value)
				{
					this.flags1 |= 268435456U;
					return;
				}
				this.flags1 &= 4026531839U;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalSetZeroingMasking()
		{
			this.flags1 |= 268435456U;
		}

		public bool MergingMasking
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (this.flags1 & 268435456U) == 0U;
			}
			set
			{
				if (value)
				{
					this.flags1 &= 4026531839U;
					return;
				}
				this.flags1 |= 268435456U;
			}
		}

		public RoundingControl RoundingControl
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (RoundingControl)((this.flags1 >> 12) & 7U);
			}
			set
			{
				this.flags1 = (this.flags1 & 4294938623U) | (uint)((uint)value << 12);
			}
		}

		internal uint InternalRoundingControl
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.flags1 |= value << 12;
			}
		}

		internal readonly bool HasRoundingControlOrSae
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.flags1 & 134246400U) > 0U;
			}
		}

		public int DeclareDataCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (int)(((this.flags1 >> 8) & 15U) + 1U);
			}
			set
			{
				this.flags1 = (this.flags1 & 4294963455U) | (uint)((uint)((value - 1) & 15) << 8);
			}
		}

		internal uint InternalDeclareDataCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.flags1 |= value - 1U << 8;
			}
		}

		public void SetDeclareByteValue(int index, sbyte value)
		{
			this.SetDeclareByteValue(index, (byte)value);
		}

		public void SetDeclareByteValue(int index, byte value)
		{
			switch (index)
			{
			case 0:
				this.reg0 = value;
				return;
			case 1:
				this.reg1 = value;
				return;
			case 2:
				this.reg2 = value;
				return;
			case 3:
				this.reg3 = value;
				return;
			case 4:
				this.immediate = (this.immediate & 4294967040U) | (uint)value;
				return;
			case 5:
				this.immediate = (this.immediate & 4294902015U) | (uint)((uint)value << 8);
				return;
			case 6:
				this.immediate = (this.immediate & 4278255615U) | (uint)((uint)value << 16);
				return;
			case 7:
				this.immediate = (this.immediate & 16777215U) | (uint)((uint)value << 24);
				return;
			case 8:
				this.memDispl = (this.memDispl & 18446744073709551360UL) | (ulong)value;
				return;
			case 9:
				this.memDispl = (this.memDispl & 18446744073709486335UL) | ((ulong)value << 8);
				return;
			case 10:
				this.memDispl = (this.memDispl & 18446744073692839935UL) | ((ulong)value << 16);
				return;
			case 11:
				this.memDispl = (this.memDispl & 18446744069431361535UL) | ((ulong)value << 24);
				return;
			case 12:
				this.memDispl = (this.memDispl & 18446742978492891135UL) | ((ulong)value << 32);
				return;
			case 13:
				this.memDispl = (this.memDispl & 18446463698244468735UL) | ((ulong)value << 40);
				return;
			case 14:
				this.memDispl = (this.memDispl & 18374967954648334335UL) | ((ulong)value << 48);
				return;
			case 15:
				this.memDispl = (this.memDispl & 72057594037927935UL) | ((ulong)value << 56);
				return;
			default:
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
				return;
			}
		}

		public readonly byte GetDeclareByteValue(int index)
		{
			switch (index)
			{
			case 0:
				return this.reg0;
			case 1:
				return this.reg1;
			case 2:
				return this.reg2;
			case 3:
				return this.reg3;
			case 4:
				return (byte)this.immediate;
			case 5:
				return (byte)(this.immediate >> 8);
			case 6:
				return (byte)(this.immediate >> 16);
			case 7:
				return (byte)(this.immediate >> 24);
			case 8:
				return (byte)this.memDispl;
			case 9:
				return (byte)((uint)this.memDispl >> 8);
			case 10:
				return (byte)((uint)this.memDispl >> 16);
			case 11:
				return (byte)((uint)this.memDispl >> 24);
			case 12:
				return (byte)(this.memDispl >> 32);
			case 13:
				return (byte)(this.memDispl >> 40);
			case 14:
				return (byte)(this.memDispl >> 48);
			case 15:
				return (byte)(this.memDispl >> 56);
			default:
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
				return 0;
			}
		}

		public void SetDeclareWordValue(int index, short value)
		{
			this.SetDeclareWordValue(index, (ushort)value);
		}

		public void SetDeclareWordValue(int index, ushort value)
		{
			switch (index)
			{
			case 0:
				this.reg0 = (byte)value;
				this.reg1 = (byte)(value >> 8);
				return;
			case 1:
				this.reg2 = (byte)value;
				this.reg3 = (byte)(value >> 8);
				return;
			case 2:
				this.immediate = (this.immediate & 4294901760U) | (uint)value;
				return;
			case 3:
				this.immediate = (uint)((int)((ushort)this.immediate) | ((int)value << 16));
				return;
			case 4:
				this.memDispl = (this.memDispl & 18446744073709486080UL) | (ulong)value;
				return;
			case 5:
				this.memDispl = (this.memDispl & 18446744069414649855UL) | ((ulong)value << 16);
				return;
			case 6:
				this.memDispl = (this.memDispl & 18446462603027808255UL) | ((ulong)value << 32);
				return;
			case 7:
				this.memDispl = (this.memDispl & 281474976710655UL) | ((ulong)value << 48);
				return;
			default:
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
				return;
			}
		}

		public readonly ushort GetDeclareWordValue(int index)
		{
			switch (index)
			{
			case 0:
				return (ushort)((int)this.reg0 | ((int)this.reg1 << 8));
			case 1:
				return (ushort)((int)this.reg2 | ((int)this.reg3 << 8));
			case 2:
				return (ushort)this.immediate;
			case 3:
				return (ushort)(this.immediate >> 16);
			case 4:
				return (ushort)this.memDispl;
			case 5:
				return (ushort)((uint)this.memDispl >> 16);
			case 6:
				return (ushort)(this.memDispl >> 32);
			case 7:
				return (ushort)(this.memDispl >> 48);
			default:
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
				return 0;
			}
		}

		public void SetDeclareDwordValue(int index, int value)
		{
			this.SetDeclareDwordValue(index, (uint)value);
		}

		public void SetDeclareDwordValue(int index, uint value)
		{
			switch (index)
			{
			case 0:
				this.reg0 = (byte)value;
				this.reg1 = (byte)(value >> 8);
				this.reg2 = (byte)(value >> 16);
				this.reg3 = (byte)(value >> 24);
				return;
			case 1:
				this.immediate = value;
				return;
			case 2:
				this.memDispl = (this.memDispl & 18446744069414584320UL) | (ulong)value;
				return;
			case 3:
				this.memDispl = (this.memDispl & (ulong)(-1)) | ((ulong)value << 32);
				return;
			default:
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
				return;
			}
		}

		public readonly uint GetDeclareDwordValue(int index)
		{
			switch (index)
			{
			case 0:
				return (uint)((int)this.reg0 | ((int)this.reg1 << 8) | ((int)this.reg2 << 16) | ((int)this.reg3 << 24));
			case 1:
				return this.immediate;
			case 2:
				return (uint)this.memDispl;
			case 3:
				return (uint)(this.memDispl >> 32);
			default:
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
				return 0U;
			}
		}

		public void SetDeclareQwordValue(int index, long value)
		{
			this.SetDeclareQwordValue(index, (ulong)value);
		}

		public void SetDeclareQwordValue(int index, ulong value)
		{
			if (index == 0)
			{
				uint num = (uint)value;
				this.reg0 = (byte)num;
				this.reg1 = (byte)(num >> 8);
				this.reg2 = (byte)(num >> 16);
				this.reg3 = (byte)(num >> 24);
				this.immediate = (uint)(value >> 32);
				return;
			}
			if (index != 1)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
				return;
			}
			this.memDispl = value;
		}

		public readonly ulong GetDeclareQwordValue(int index)
		{
			if (index == 0)
			{
				return (ulong)this.reg0 | (ulong)((ulong)this.reg1 << 8) | (ulong)((ulong)this.reg2 << 16) | (ulong)((ulong)this.reg3 << 24) | ((ulong)this.immediate << 32);
			}
			if (index != 1)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
				return 0UL;
			}
			return this.memDispl;
		}

		public readonly bool IsVsib
		{
			get
			{
				bool flag;
				return this.TryGetVsib64(out flag);
			}
		}

		public readonly bool IsVsib32
		{
			get
			{
				bool flag;
				return this.TryGetVsib64(out flag) && !flag;
			}
		}

		public readonly bool IsVsib64
		{
			get
			{
				bool flag;
				return this.TryGetVsib64(out flag) && flag;
			}
		}

		public readonly bool TryGetVsib64(out bool vsib64)
		{
			Code code = this.Code;
			if (code <= Code.EVEX_Vscatterpf1qpd_vm64z_k1)
			{
				if (code <= Code.EVEX_Vscatterqpd_vm64z_k1_zmm)
				{
					switch (code)
					{
					case Code.VEX_Vpgatherdd_xmm_vm32x_xmm:
					case Code.VEX_Vpgatherdd_ymm_vm32y_ymm:
					case Code.VEX_Vpgatherdq_xmm_vm32x_xmm:
					case Code.VEX_Vpgatherdq_ymm_vm32x_ymm:
					case Code.EVEX_Vpgatherdd_xmm_k1_vm32x:
					case Code.EVEX_Vpgatherdd_ymm_k1_vm32y:
					case Code.EVEX_Vpgatherdd_zmm_k1_vm32z:
					case Code.EVEX_Vpgatherdq_xmm_k1_vm32x:
					case Code.EVEX_Vpgatherdq_ymm_k1_vm32x:
					case Code.EVEX_Vpgatherdq_zmm_k1_vm32y:
					case Code.VEX_Vgatherdps_xmm_vm32x_xmm:
					case Code.VEX_Vgatherdps_ymm_vm32y_ymm:
					case Code.VEX_Vgatherdpd_xmm_vm32x_xmm:
					case Code.VEX_Vgatherdpd_ymm_vm32x_ymm:
					case Code.EVEX_Vgatherdps_xmm_k1_vm32x:
					case Code.EVEX_Vgatherdps_ymm_k1_vm32y:
					case Code.EVEX_Vgatherdps_zmm_k1_vm32z:
					case Code.EVEX_Vgatherdpd_xmm_k1_vm32x:
					case Code.EVEX_Vgatherdpd_ymm_k1_vm32x:
					case Code.EVEX_Vgatherdpd_zmm_k1_vm32y:
						goto IL_017F;
					case Code.VEX_Vpgatherqd_xmm_vm64x_xmm:
					case Code.VEX_Vpgatherqd_xmm_vm64y_xmm:
					case Code.VEX_Vpgatherqq_xmm_vm64x_xmm:
					case Code.VEX_Vpgatherqq_ymm_vm64y_ymm:
					case Code.EVEX_Vpgatherqd_xmm_k1_vm64x:
					case Code.EVEX_Vpgatherqd_xmm_k1_vm64y:
					case Code.EVEX_Vpgatherqd_ymm_k1_vm64z:
					case Code.EVEX_Vpgatherqq_xmm_k1_vm64x:
					case Code.EVEX_Vpgatherqq_ymm_k1_vm64y:
					case Code.EVEX_Vpgatherqq_zmm_k1_vm64z:
					case Code.VEX_Vgatherqps_xmm_vm64x_xmm:
					case Code.VEX_Vgatherqps_xmm_vm64y_xmm:
					case Code.VEX_Vgatherqpd_xmm_vm64x_xmm:
					case Code.VEX_Vgatherqpd_ymm_vm64y_ymm:
					case Code.EVEX_Vgatherqps_xmm_k1_vm64x:
					case Code.EVEX_Vgatherqps_xmm_k1_vm64y:
					case Code.EVEX_Vgatherqps_ymm_k1_vm64z:
					case Code.EVEX_Vgatherqpd_xmm_k1_vm64x:
					case Code.EVEX_Vgatherqpd_ymm_k1_vm64y:
					case Code.EVEX_Vgatherqpd_zmm_k1_vm64z:
						break;
					default:
						switch (code)
						{
						case Code.EVEX_Vpscatterdd_vm32x_k1_xmm:
						case Code.EVEX_Vpscatterdd_vm32y_k1_ymm:
						case Code.EVEX_Vpscatterdd_vm32z_k1_zmm:
						case Code.EVEX_Vpscatterdq_vm32x_k1_xmm:
						case Code.EVEX_Vpscatterdq_vm32x_k1_ymm:
						case Code.EVEX_Vpscatterdq_vm32y_k1_zmm:
						case Code.EVEX_Vscatterdps_vm32x_k1_xmm:
						case Code.EVEX_Vscatterdps_vm32y_k1_ymm:
						case Code.EVEX_Vscatterdps_vm32z_k1_zmm:
						case Code.EVEX_Vscatterdpd_vm32x_k1_xmm:
						case Code.EVEX_Vscatterdpd_vm32x_k1_ymm:
						case Code.EVEX_Vscatterdpd_vm32y_k1_zmm:
							goto IL_017F;
						case Code.EVEX_Vpscatterqd_vm64x_k1_xmm:
						case Code.EVEX_Vpscatterqd_vm64y_k1_xmm:
						case Code.EVEX_Vpscatterqd_vm64z_k1_ymm:
						case Code.EVEX_Vpscatterqq_vm64x_k1_xmm:
						case Code.EVEX_Vpscatterqq_vm64y_k1_ymm:
						case Code.EVEX_Vpscatterqq_vm64z_k1_zmm:
						case Code.EVEX_Vscatterqps_vm64x_k1_xmm:
						case Code.EVEX_Vscatterqps_vm64y_k1_xmm:
						case Code.EVEX_Vscatterqps_vm64z_k1_ymm:
						case Code.EVEX_Vscatterqpd_vm64x_k1_xmm:
						case Code.EVEX_Vscatterqpd_vm64y_k1_ymm:
						case Code.EVEX_Vscatterqpd_vm64z_k1_zmm:
							break;
						default:
							goto IL_0189;
						}
						break;
					}
				}
				else
				{
					if (code - Code.EVEX_Vgatherpf0dps_vm32z_k1 <= 7)
					{
						goto IL_017F;
					}
					if (code - Code.EVEX_Vgatherpf0qps_vm64z_k1 > 7)
					{
						goto IL_0189;
					}
				}
				vsib64 = true;
				return true;
			}
			if (code <= Code.MVEX_Vscatterdpd_mvt_k1_zmm)
			{
				if (code - Code.MVEX_Vpgatherdd_zmm_k1_mvt > 3 && code - Code.MVEX_Vpscatterdd_mvt_k1_zmm > 3)
				{
					goto IL_0189;
				}
			}
			else if (code - Code.MVEX_Undoc_zmm_k1_mvt_512_66_0F38_W0_B0 > 1 && code - Code.MVEX_Undoc_zmm_k1_mvt_512_66_0F38_W0_C0 > 8)
			{
				goto IL_0189;
			}
			IL_017F:
			vsib64 = false;
			return true;
			IL_0189:
			vsib64 = false;
			return false;
		}

		public bool SuppressAllExceptions
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (this.flags1 & 134217728U) > 0U;
			}
			set
			{
				if (value)
				{
					this.flags1 |= 134217728U;
					return;
				}
				this.flags1 &= 4160749567U;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void InternalSetSuppressAllExceptions()
		{
			this.flags1 |= 134217728U;
		}

		public readonly bool IsIPRelativeMemoryOperand
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.MemoryBase == Register.RIP || this.MemoryBase == Register.EIP;
			}
		}

		public readonly ulong IPRelativeMemoryAddress
		{
			get
			{
				if (this.MemoryBase != Register.EIP)
				{
					return this.MemoryDisplacement64;
				}
				return (ulong)this.MemoryDisplacement32;
			}
		}

		public override readonly string ToString()
		{
			return base.ToString() ?? string.Empty;
		}

		public readonly ulong GetVirtualAddress(int operand, int elementIndex, VAGetRegisterValue getRegisterValue)
		{
			if (getRegisterValue == null)
			{
				throw new ArgumentNullException("getRegisterValue");
			}
			VARegisterValueProviderDelegateImpl varegisterValueProviderDelegateImpl = new VARegisterValueProviderDelegateImpl(getRegisterValue);
			ulong num;
			if (this.TryGetVirtualAddress(operand, elementIndex, varegisterValueProviderDelegateImpl, out num))
			{
				return num;
			}
			return 0UL;
		}

		public readonly ulong GetVirtualAddress(int operand, int elementIndex, IVARegisterValueProvider registerValueProvider)
		{
			if (registerValueProvider == null)
			{
				throw new ArgumentNullException("registerValueProvider");
			}
			VARegisterValueProviderAdapter varegisterValueProviderAdapter = new VARegisterValueProviderAdapter(registerValueProvider);
			ulong num;
			if (this.TryGetVirtualAddress(operand, elementIndex, varegisterValueProviderAdapter, out num))
			{
				return num;
			}
			return 0UL;
		}

		public readonly bool TryGetVirtualAddress(int operand, int elementIndex, out ulong result, VATryGetRegisterValue getRegisterValue)
		{
			if (getRegisterValue == null)
			{
				throw new ArgumentNullException("getRegisterValue");
			}
			VATryGetRegisterValueDelegateImpl vatryGetRegisterValueDelegateImpl = new VATryGetRegisterValueDelegateImpl(getRegisterValue);
			return this.TryGetVirtualAddress(operand, elementIndex, vatryGetRegisterValueDelegateImpl, out result);
		}

		public readonly bool TryGetVirtualAddress(int operand, int elementIndex, IVATryGetRegisterValueProvider registerValueProvider, out ulong result)
		{
			if (registerValueProvider == null)
			{
				throw new ArgumentNullException("registerValueProvider");
			}
			switch (this.GetOpKind(operand))
			{
			case OpKind.Register:
			case OpKind.NearBranch16:
			case OpKind.NearBranch32:
			case OpKind.NearBranch64:
			case OpKind.FarBranch16:
			case OpKind.FarBranch32:
			case OpKind.Immediate8:
			case OpKind.Immediate8_2nd:
			case OpKind.Immediate16:
			case OpKind.Immediate32:
			case OpKind.Immediate64:
			case OpKind.Immediate8to16:
			case OpKind.Immediate8to32:
			case OpKind.Immediate8to64:
			case OpKind.Immediate32to64:
				result = 0UL;
				return true;
			case OpKind.MemorySegSI:
			{
				ulong num;
				ulong num2;
				if (registerValueProvider.TryGetRegisterValue(this.MemorySegment, 0, 0, out num) && registerValueProvider.TryGetRegisterValue(Register.SI, 0, 0, out num2))
				{
					result = num + (ulong)((ushort)num2);
					return true;
				}
				break;
			}
			case OpKind.MemorySegESI:
			{
				ulong num;
				ulong num2;
				if (registerValueProvider.TryGetRegisterValue(this.MemorySegment, 0, 0, out num) && registerValueProvider.TryGetRegisterValue(Register.ESI, 0, 0, out num2))
				{
					result = num + (ulong)((uint)num2);
					return true;
				}
				break;
			}
			case OpKind.MemorySegRSI:
			{
				ulong num;
				ulong num2;
				if (registerValueProvider.TryGetRegisterValue(this.MemorySegment, 0, 0, out num) && registerValueProvider.TryGetRegisterValue(Register.RSI, 0, 0, out num2))
				{
					result = num + num2;
					return true;
				}
				break;
			}
			case OpKind.MemorySegDI:
			{
				ulong num;
				ulong num2;
				if (registerValueProvider.TryGetRegisterValue(this.MemorySegment, 0, 0, out num) && registerValueProvider.TryGetRegisterValue(Register.DI, 0, 0, out num2))
				{
					result = num + (ulong)((ushort)num2);
					return true;
				}
				break;
			}
			case OpKind.MemorySegEDI:
			{
				ulong num;
				ulong num2;
				if (registerValueProvider.TryGetRegisterValue(this.MemorySegment, 0, 0, out num) && registerValueProvider.TryGetRegisterValue(Register.EDI, 0, 0, out num2))
				{
					result = num + (ulong)((uint)num2);
					return true;
				}
				break;
			}
			case OpKind.MemorySegRDI:
			{
				ulong num;
				ulong num2;
				if (registerValueProvider.TryGetRegisterValue(this.MemorySegment, 0, 0, out num) && registerValueProvider.TryGetRegisterValue(Register.RDI, 0, 0, out num2))
				{
					result = num + num2;
					return true;
				}
				break;
			}
			case OpKind.MemoryESDI:
			{
				ulong num;
				ulong num2;
				if (registerValueProvider.TryGetRegisterValue(Register.ES, 0, 0, out num) && registerValueProvider.TryGetRegisterValue(Register.DI, 0, 0, out num2))
				{
					result = num + (ulong)((ushort)num2);
					return true;
				}
				break;
			}
			case OpKind.MemoryESEDI:
			{
				ulong num;
				ulong num2;
				if (registerValueProvider.TryGetRegisterValue(Register.ES, 0, 0, out num) && registerValueProvider.TryGetRegisterValue(Register.EDI, 0, 0, out num2))
				{
					result = num + (ulong)((uint)num2);
					return true;
				}
				break;
			}
			case OpKind.MemoryESRDI:
			{
				ulong num;
				ulong num2;
				if (registerValueProvider.TryGetRegisterValue(Register.ES, 0, 0, out num) && registerValueProvider.TryGetRegisterValue(Register.RDI, 0, 0, out num2))
				{
					result = num + num2;
					return true;
				}
				break;
			}
			case OpKind.Memory:
			{
				Register memoryBase = this.MemoryBase;
				Register memoryIndex = this.MemoryIndex;
				int addressSizeInBytes = InstructionUtils.GetAddressSizeInBytes(memoryBase, memoryIndex, this.MemoryDisplSize, this.CodeSize);
				ulong num3 = this.MemoryDisplacement64;
				ulong num4;
				if (addressSizeInBytes == 8)
				{
					num4 = ulong.MaxValue;
				}
				else if (addressSizeInBytes == 4)
				{
					num4 = (ulong)(-1);
				}
				else
				{
					num4 = 65535UL;
				}
				if (memoryBase != Register.None && memoryBase != Register.RIP && memoryBase != Register.EIP)
				{
					ulong num2;
					if (!registerValueProvider.TryGetRegisterValue(memoryBase, 0, 0, out num2))
					{
						break;
					}
					num3 += num2;
				}
				Code code = this.Code;
				if (memoryIndex != Register.None && !code.IgnoresIndex() && !code.IsTileStrideIndex())
				{
					bool flag;
					if (this.TryGetVsib64(out flag))
					{
						ulong num2;
						bool flag2;
						if (flag)
						{
							flag2 = registerValueProvider.TryGetRegisterValue(memoryIndex, elementIndex, 8, out num2);
						}
						else
						{
							flag2 = registerValueProvider.TryGetRegisterValue(memoryIndex, elementIndex, 4, out num2);
							num2 = (ulong)((long)((int)num2));
						}
						if (!flag2)
						{
							break;
						}
						num3 += num2 << this.InternalMemoryIndexScale;
					}
					else
					{
						ulong num2;
						if (!registerValueProvider.TryGetRegisterValue(memoryIndex, 0, 0, out num2))
						{
							break;
						}
						num3 += num2 << this.InternalMemoryIndexScale;
					}
				}
				num3 &= num4;
				if (!code.IgnoresSegment())
				{
					ulong num;
					if (!registerValueProvider.TryGetRegisterValue(this.MemorySegment, 0, 0, out num))
					{
						break;
					}
					num3 += num;
				}
				result = num3;
				return true;
			}
			default:
				throw new InvalidOperationException();
			}
			result = 0UL;
			return false;
		}

		internal const int TOTAL_SIZE = 40;

		private ulong nextRip;

		private ulong memDispl;

		private uint flags1;

		private uint immediate;

		private ushort code;

		private byte memBaseReg;

		private byte memIndexReg;

		private byte reg0;

		private byte reg1;

		private byte reg2;

		private byte reg3;

		private byte opKind0;

		private byte opKind1;

		private byte opKind2;

		private byte opKind3;

		private byte scale;

		private byte displSize;

		private byte len;

		private byte pad;

		[Flags]
		private enum InstrFlags1 : uint
		{
			SegmentPrefixMask = 7U,
			SegmentPrefixShift = 5U,
			DataLengthMask = 15U,
			DataLengthShift = 8U,
			RoundingControlMask = 7U,
			RoundingControlShift = 12U,
			OpMaskMask = 7U,
			OpMaskShift = 15U,
			CodeSizeMask = 3U,
			CodeSizeShift = 18U,
			Broadcast = 67108864U,
			SuppressAllExceptions = 134217728U,
			ZeroingMasking = 268435456U,
			RepePrefix = 536870912U,
			RepnePrefix = 1073741824U,
			LockPrefix = 2147483648U,
			EqualsIgnoreMask = 786432U
		}

		[Flags]
		private enum MvexInstrFlags : uint
		{
			MvexRegMemConvShift = 16U,
			MvexRegMemConvMask = 31U,
			EvictionHint = 2147483648U
		}
	}
}
