using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Iced.Intel.EncoderInternal;

namespace Iced.Intel
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class Encoder
	{
		public bool PreventVEX2
		{
			get
			{
				return this.Internal_PreventVEX2 > 0U;
			}
			set
			{
				this.Internal_PreventVEX2 = (value ? uint.MaxValue : 0U);
			}
		}

		public uint VEX_WIG
		{
			get
			{
				return (this.Internal_VEX_WIG_LIG >> 7) & 1U;
			}
			set
			{
				this.Internal_VEX_WIG_LIG = (this.Internal_VEX_WIG_LIG & 4294967167U) | ((value & 1U) << 7);
			}
		}

		public uint VEX_LIG
		{
			get
			{
				return (this.Internal_VEX_WIG_LIG >> 2) & 1U;
			}
			set
			{
				this.Internal_VEX_WIG_LIG = (this.Internal_VEX_WIG_LIG & 4294967291U) | ((value & 1U) << 2);
				this.Internal_VEX_LIG = (value & 1U) << 2;
			}
		}

		public uint EVEX_WIG
		{
			get
			{
				return this.Internal_EVEX_WIG >> 7;
			}
			set
			{
				this.Internal_EVEX_WIG = (value & 1U) << 7;
			}
		}

		public uint EVEX_LIG
		{
			get
			{
				return this.Internal_EVEX_LIG >> 5;
			}
			set
			{
				this.Internal_EVEX_LIG = (value & 3U) << 5;
			}
		}

		public int Bitness
		{
			get
			{
				return this.bitness;
			}
		}

		private Encoder(CodeWriter writer, int bitness)
		{
			if (writer == null)
			{
				ThrowHelper.ThrowArgumentNullException_writer();
			}
			this.immSizes = Encoder.s_immSizes;
			this.writer = writer;
			this.bitness = bitness;
			this.handlers = OpCodeHandlers.Handlers;
			this.handler = null;
			this.opSize16Flags = ((bitness != 16) ? EncoderFlags.P66 : EncoderFlags.None);
			this.opSize32Flags = ((bitness == 16) ? EncoderFlags.P66 : EncoderFlags.None);
			this.adrSize16Flags = ((bitness != 16) ? EncoderFlags.P67 : EncoderFlags.None);
			this.adrSize32Flags = ((bitness != 32) ? EncoderFlags.P67 : EncoderFlags.None);
		}

		public static Encoder Create(int bitness, CodeWriter writer)
		{
			if (bitness == 16 || bitness == 32 || bitness == 64)
			{
				return new Encoder(writer, bitness);
			}
			throw new ArgumentOutOfRangeException("bitness");
		}

		public uint Encode(in Instruction instruction, ulong rip)
		{
			uint num;
			string text;
			if (!this.TryEncode(in instruction, rip, out num, out text))
			{
				Encoder.ThrowEncoderException(in instruction, text);
			}
			return num;
		}

		private static void ThrowEncoderException(in Instruction instruction, string errorMessage)
		{
			throw new EncoderException(errorMessage, in instruction);
		}

		[NullableContext(2)]
		public bool TryEncode(in Instruction instruction, ulong rip, out uint encodedLength, [<b37590d4-39fb-478a-88de-d293f3364852>NotNullWhen(false)] out string errorMessage)
		{
			this.currentRip = rip;
			this.eip = (uint)rip;
			this.errorMessage = null;
			this.EncoderFlags = EncoderFlags.None;
			this.DisplSize = DisplSize.None;
			this.ImmSize = ImmSize.None;
			this.ModRM = 0;
			OpCodeHandler opCodeHandler = this.handlers[(int)instruction.Code];
			this.handler = opCodeHandler;
			this.OpCode = opCodeHandler.OpCode;
			if (opCodeHandler.GroupIndex >= 0)
			{
				this.EncoderFlags = EncoderFlags.ModRM;
				this.ModRM = (byte)(opCodeHandler.GroupIndex << 3);
			}
			if (opCodeHandler.RmGroupIndex >= 0)
			{
				this.EncoderFlags = EncoderFlags.ModRM;
				this.ModRM |= (byte)(opCodeHandler.RmGroupIndex | 192);
			}
			EncFlags3 encFlags = opCodeHandler.EncFlags3 & (EncFlags3.Bit16or32 | EncFlags3.Bit64);
			if (encFlags != EncFlags3.Bit16or32)
			{
				if (encFlags != EncFlags3.Bit64)
				{
					if (encFlags != (EncFlags3.Bit16or32 | EncFlags3.Bit64))
					{
						throw new InvalidOperationException();
					}
				}
				else if (this.bitness != 64)
				{
					this.ErrorMessage = "The instruction can only be used in 64-bit mode";
				}
			}
			else if (this.bitness == 64)
			{
				this.ErrorMessage = "The instruction can only be used in 16/32-bit mode";
			}
			switch (opCodeHandler.OpSize)
			{
			case CodeSize.Unknown:
				break;
			case CodeSize.Code16:
				this.EncoderFlags |= this.opSize16Flags;
				break;
			case CodeSize.Code32:
				this.EncoderFlags |= this.opSize32Flags;
				break;
			case CodeSize.Code64:
				if ((opCodeHandler.EncFlags3 & EncFlags3.DefaultOpSize64) == EncFlags3.None)
				{
					this.EncoderFlags |= EncoderFlags.W;
				}
				break;
			default:
				throw new InvalidOperationException();
			}
			switch (opCodeHandler.AddrSize)
			{
			case CodeSize.Unknown:
			case CodeSize.Code64:
				break;
			case CodeSize.Code16:
				this.EncoderFlags |= this.adrSize16Flags;
				break;
			case CodeSize.Code32:
				this.EncoderFlags |= this.adrSize32Flags;
				break;
			default:
				throw new InvalidOperationException();
			}
			if (!opCodeHandler.IsSpecialInstr)
			{
				Op[] operands = opCodeHandler.Operands;
				for (int i = 0; i < operands.Length; i++)
				{
					operands[i].Encode(this, in instruction, i);
				}
				if ((opCodeHandler.EncFlags3 & EncFlags3.Fwait) != EncFlags3.None)
				{
					this.WriteByteInternal(155U);
				}
				opCodeHandler.Encode(this, in instruction);
				uint opCode = this.OpCode;
				if (!opCodeHandler.Is2ByteOpCode)
				{
					this.WriteByteInternal(opCode);
				}
				else
				{
					this.WriteByteInternal(opCode >> 8);
					this.WriteByteInternal(opCode);
				}
				if ((this.EncoderFlags & (EncoderFlags.ModRM | EncoderFlags.Displ)) != EncoderFlags.None)
				{
					this.WriteModRM();
				}
				if (this.ImmSize != ImmSize.None)
				{
					this.WriteImmediate();
				}
			}
			else
			{
				opCodeHandler.Encode(this, in instruction);
			}
			uint num = (uint)this.currentRip - (uint)rip;
			if (num > 15U && !opCodeHandler.IsSpecialInstr)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Instruction length > ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(15);
				defaultInterpolatedStringHandler.AppendLiteral(" bytes");
				this.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			errorMessage = this.errorMessage;
			if (errorMessage != null)
			{
				encodedLength = 0U;
				return false;
			}
			encodedLength = num;
			return true;
		}

		[Nullable(2)]
		internal string ErrorMessage
		{
			[NullableContext(2)]
			set
			{
				if (this.errorMessage == null)
				{
					this.errorMessage = value;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool Verify(int operand, OpKind expected, OpKind actual)
		{
			if (expected == actual)
			{
				return true;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Operand ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
			defaultInterpolatedStringHandler.AppendLiteral(": Expected: ");
			defaultInterpolatedStringHandler.AppendFormatted<OpKind>(expected);
			defaultInterpolatedStringHandler.AppendLiteral(", actual: ");
			defaultInterpolatedStringHandler.AppendFormatted<OpKind>(actual);
			this.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool Verify(int operand, Register expected, Register actual)
		{
			if (expected == actual)
			{
				return true;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Operand ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
			defaultInterpolatedStringHandler.AppendLiteral(": Expected: ");
			defaultInterpolatedStringHandler.AppendFormatted<Register>(expected);
			defaultInterpolatedStringHandler.AppendLiteral(", actual: ");
			defaultInterpolatedStringHandler.AppendFormatted<Register>(actual);
			this.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool Verify(int operand, Register register, Register regLo, Register regHi)
		{
			if (this.bitness != 64 && regHi > regLo + 7)
			{
				regHi = regLo + 7;
			}
			if (regLo <= register && register <= regHi)
			{
				return true;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 4);
			defaultInterpolatedStringHandler.AppendLiteral("Operand ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
			defaultInterpolatedStringHandler.AppendLiteral(": Register ");
			defaultInterpolatedStringHandler.AppendFormatted<Register>(register);
			defaultInterpolatedStringHandler.AppendLiteral(" is not between ");
			defaultInterpolatedStringHandler.AppendFormatted<Register>(regLo);
			defaultInterpolatedStringHandler.AppendLiteral(" and ");
			defaultInterpolatedStringHandler.AppendFormatted<Register>(regHi);
			defaultInterpolatedStringHandler.AppendLiteral(" (inclusive)");
			this.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
			return false;
		}

		internal void AddBranch(OpKind opKind, int immSize, in Instruction instruction, int operand)
		{
			if (!this.Verify(operand, opKind, instruction.GetOpKind(operand)))
			{
				return;
			}
			switch (immSize)
			{
			case 1:
				switch (opKind)
				{
				case OpKind.NearBranch16:
					this.EncoderFlags |= this.opSize16Flags;
					this.ImmSize = ImmSize.RipRelSize1_Target16;
					this.Immediate = (uint)instruction.NearBranch16;
					return;
				case OpKind.NearBranch32:
					this.EncoderFlags |= this.opSize32Flags;
					this.ImmSize = ImmSize.RipRelSize1_Target32;
					this.Immediate = instruction.NearBranch32;
					return;
				case OpKind.NearBranch64:
				{
					this.ImmSize = ImmSize.RipRelSize1_Target64;
					ulong num = instruction.NearBranch64;
					this.Immediate = (uint)num;
					this.ImmediateHi = (uint)(num >> 32);
					return;
				}
				default:
					throw new InvalidOperationException();
				}
				break;
			case 2:
				if (opKind == OpKind.NearBranch16)
				{
					this.EncoderFlags |= this.opSize16Flags;
					this.ImmSize = ImmSize.RipRelSize2_Target16;
					this.Immediate = (uint)instruction.NearBranch16;
					return;
				}
				throw new InvalidOperationException();
			case 4:
			{
				if (opKind == OpKind.NearBranch32)
				{
					this.EncoderFlags |= this.opSize32Flags;
					this.ImmSize = ImmSize.RipRelSize4_Target32;
					this.Immediate = instruction.NearBranch32;
					return;
				}
				if (opKind != OpKind.NearBranch64)
				{
					throw new InvalidOperationException();
				}
				this.ImmSize = ImmSize.RipRelSize4_Target64;
				ulong num = instruction.NearBranch64;
				this.Immediate = (uint)num;
				this.ImmediateHi = (uint)(num >> 32);
				return;
			}
			}
			throw new InvalidOperationException();
		}

		internal void AddBranchX(int immSize, in Instruction instruction, int operand)
		{
			if (this.bitness == 64)
			{
				if (!this.Verify(operand, OpKind.NearBranch64, instruction.GetOpKind(operand)))
				{
					return;
				}
				ulong nearBranch = instruction.NearBranch64;
				if (immSize == 2)
				{
					this.EncoderFlags |= EncoderFlags.P66;
					this.ImmSize = ImmSize.RipRelSize2_Target64;
					this.Immediate = (uint)nearBranch;
					this.ImmediateHi = (uint)(nearBranch >> 32);
					return;
				}
				if (immSize != 4)
				{
					throw new InvalidOperationException();
				}
				this.ImmSize = ImmSize.RipRelSize4_Target64;
				this.Immediate = (uint)nearBranch;
				this.ImmediateHi = (uint)(nearBranch >> 32);
				return;
			}
			else
			{
				if (!this.Verify(operand, OpKind.NearBranch32, instruction.GetOpKind(operand)))
				{
					return;
				}
				if (immSize == 2)
				{
					this.EncoderFlags |= (EncoderFlags)((this.bitness & 32) << 2);
					this.ImmSize = ImmSize.RipRelSize2_Target32;
					this.Immediate = instruction.NearBranch32;
					return;
				}
				if (immSize != 4)
				{
					if (immSize != 8)
					{
					}
					throw new InvalidOperationException();
				}
				this.EncoderFlags |= (EncoderFlags)((this.bitness & 16) << 3);
				this.ImmSize = ImmSize.RipRelSize4_Target32;
				this.Immediate = instruction.NearBranch32;
				return;
			}
		}

		internal void AddBranchDisp(int displSize, in Instruction instruction, int operand)
		{
			OpKind opKind;
			if (displSize != 2)
			{
				if (displSize != 4)
				{
					throw new InvalidOperationException();
				}
				opKind = OpKind.NearBranch32;
				this.ImmSize = ImmSize.Size4;
				this.Immediate = instruction.NearBranch32;
			}
			else
			{
				opKind = OpKind.NearBranch16;
				this.ImmSize = ImmSize.Size2;
				this.Immediate = (uint)instruction.NearBranch16;
			}
			this.Verify(operand, opKind, instruction.GetOpKind(operand));
		}

		internal void AddFarBranch(in Instruction instruction, int operand, int size)
		{
			if (size == 2)
			{
				if (!this.Verify(operand, OpKind.FarBranch16, instruction.GetOpKind(operand)))
				{
					return;
				}
				this.ImmSize = ImmSize.Size2_2;
				this.Immediate = (uint)instruction.FarBranch16;
				this.ImmediateHi = (uint)instruction.FarBranchSelector;
			}
			else
			{
				if (!this.Verify(operand, OpKind.FarBranch32, instruction.GetOpKind(operand)))
				{
					return;
				}
				this.ImmSize = ImmSize.Size4_2;
				this.Immediate = instruction.FarBranch32;
				this.ImmediateHi = (uint)instruction.FarBranchSelector;
			}
			if (this.bitness != size * 8)
			{
				this.EncoderFlags |= EncoderFlags.P66;
			}
		}

		internal void SetAddrSize(int regSize)
		{
			if (this.bitness == 64)
			{
				if (regSize == 2)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Invalid register size: ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(regSize * 8);
					defaultInterpolatedStringHandler.AppendLiteral(", must be 32-bit or 64-bit");
					this.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
					return;
				}
				if (regSize == 4)
				{
					this.EncoderFlags |= EncoderFlags.P67;
					return;
				}
			}
			else
			{
				if (regSize == 8)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(49, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("Invalid register size: ");
					defaultInterpolatedStringHandler2.AppendFormatted<int>(regSize * 8);
					defaultInterpolatedStringHandler2.AppendLiteral(", must be 16-bit or 32-bit");
					this.ErrorMessage = defaultInterpolatedStringHandler2.ToStringAndClear();
					return;
				}
				if (this.bitness == 16)
				{
					if (regSize == 4)
					{
						this.EncoderFlags |= EncoderFlags.P67;
						return;
					}
				}
				else if (regSize == 2)
				{
					this.EncoderFlags |= EncoderFlags.P67;
				}
			}
		}

		internal void AddAbsMem(in Instruction instruction, int operand)
		{
			this.EncoderFlags |= EncoderFlags.Displ;
			OpKind opKind = instruction.GetOpKind(operand);
			if (opKind != OpKind.Memory)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Operand ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler.AppendLiteral(": Expected OpKind ");
				defaultInterpolatedStringHandler.AppendFormatted("Memory");
				defaultInterpolatedStringHandler.AppendLiteral(", actual: ");
				defaultInterpolatedStringHandler.AppendFormatted<OpKind>(opKind);
				this.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				return;
			}
			if (instruction.MemoryBase != Register.None || instruction.MemoryIndex != Register.None)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(62, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("Operand ");
				defaultInterpolatedStringHandler2.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler2.AppendLiteral(": Absolute addresses can't have base and/or index regs");
				this.ErrorMessage = defaultInterpolatedStringHandler2.ToStringAndClear();
				return;
			}
			if (instruction.MemoryIndexScale != 1)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(50, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("Operand ");
				defaultInterpolatedStringHandler3.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler3.AppendLiteral(": Absolute addresses must have scale == *1");
				this.ErrorMessage = defaultInterpolatedStringHandler3.ToStringAndClear();
				return;
			}
			int memoryDisplSize = instruction.MemoryDisplSize;
			if (memoryDisplSize != 2)
			{
				if (memoryDisplSize != 4)
				{
					if (memoryDisplSize != 8)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(71, 3);
						defaultInterpolatedStringHandler4.AppendLiteral("Operand ");
						defaultInterpolatedStringHandler4.AppendFormatted<int>(operand);
						defaultInterpolatedStringHandler4.AppendLiteral(": ");
						defaultInterpolatedStringHandler4.AppendFormatted("Instruction");
						defaultInterpolatedStringHandler4.AppendLiteral(".");
						defaultInterpolatedStringHandler4.AppendFormatted("MemoryDisplSize");
						defaultInterpolatedStringHandler4.AppendLiteral(" must be initialized to 2 (16-bit), 4 (32-bit) or 8 (64-bit)");
						this.ErrorMessage = defaultInterpolatedStringHandler4.ToStringAndClear();
						return;
					}
					if (this.bitness != 64)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(61, 1);
						defaultInterpolatedStringHandler5.AppendLiteral("Operand ");
						defaultInterpolatedStringHandler5.AppendFormatted<int>(operand);
						defaultInterpolatedStringHandler5.AppendLiteral(": 64-bit abs address is only available in 64-bit mode");
						this.ErrorMessage = defaultInterpolatedStringHandler5.ToStringAndClear();
						return;
					}
					this.DisplSize = DisplSize.Size8;
					ulong memoryDisplacement = instruction.MemoryDisplacement64;
					this.Displ = (uint)memoryDisplacement;
					this.DisplHi = (uint)(memoryDisplacement >> 32);
					return;
				}
				else
				{
					this.EncoderFlags |= this.adrSize32Flags;
					this.DisplSize = DisplSize.Size4;
					if (instruction.MemoryDisplacement64 > (ulong)(-1))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(41, 1);
						defaultInterpolatedStringHandler6.AppendLiteral("Operand ");
						defaultInterpolatedStringHandler6.AppendFormatted<int>(operand);
						defaultInterpolatedStringHandler6.AppendLiteral(": Displacement must fit in a uint");
						this.ErrorMessage = defaultInterpolatedStringHandler6.ToStringAndClear();
						return;
					}
					this.Displ = instruction.MemoryDisplacement32;
					return;
				}
			}
			else
			{
				if (this.bitness == 64)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(59, 1);
					defaultInterpolatedStringHandler7.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler7.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler7.AppendLiteral(": 16-bit abs addresses can't be used in 64-bit mode");
					this.ErrorMessage = defaultInterpolatedStringHandler7.ToStringAndClear();
					return;
				}
				if (this.bitness == 32)
				{
					this.EncoderFlags |= EncoderFlags.P67;
				}
				this.DisplSize = DisplSize.Size2;
				if (instruction.MemoryDisplacement64 > 65535UL)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(43, 1);
					defaultInterpolatedStringHandler8.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler8.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler8.AppendLiteral(": Displacement must fit in a ushort");
					this.ErrorMessage = defaultInterpolatedStringHandler8.ToStringAndClear();
					return;
				}
				this.Displ = instruction.MemoryDisplacement32;
				return;
			}
		}

		internal void AddModRMRegister(in Instruction instruction, int operand, Register regLo, Register regHi)
		{
			if (!this.Verify(operand, OpKind.Register, instruction.GetOpKind(operand)))
			{
				return;
			}
			Register opRegister = instruction.GetOpRegister(operand);
			if (!this.Verify(operand, opRegister, regLo, regHi))
			{
				return;
			}
			uint num = (uint)(opRegister - regLo);
			if (regLo == Register.AL)
			{
				if (opRegister >= Register.SPL)
				{
					num -= 4U;
					this.EncoderFlags |= EncoderFlags.REX;
				}
				else if (opRegister >= Register.AH)
				{
					this.EncoderFlags |= EncoderFlags.HighLegacy8BitRegs;
				}
			}
			this.ModRM |= (byte)((num & 7U) << 3);
			this.EncoderFlags |= EncoderFlags.ModRM;
			this.EncoderFlags |= (EncoderFlags)((num & 8U) >> 1);
			this.EncoderFlags |= (EncoderFlags)((num & 16U) << 5);
		}

		internal void AddReg(in Instruction instruction, int operand, Register regLo, Register regHi)
		{
			if (!this.Verify(operand, OpKind.Register, instruction.GetOpKind(operand)))
			{
				return;
			}
			Register opRegister = instruction.GetOpRegister(operand);
			if (!this.Verify(operand, opRegister, regLo, regHi))
			{
				return;
			}
			uint num = (uint)(opRegister - regLo);
			if (regLo == Register.AL)
			{
				if (opRegister >= Register.SPL)
				{
					num -= 4U;
					this.EncoderFlags |= EncoderFlags.REX;
				}
				else if (opRegister >= Register.AH)
				{
					this.EncoderFlags |= EncoderFlags.HighLegacy8BitRegs;
				}
			}
			this.OpCode |= num & 7U;
			this.EncoderFlags |= (EncoderFlags)(num >> 3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void AddRegOrMem(in Instruction instruction, int operand, Register regLo, Register regHi, bool allowMemOp, bool allowRegOp)
		{
			this.AddRegOrMem(in instruction, operand, regLo, regHi, Register.None, Register.None, allowMemOp, allowRegOp);
		}

		internal void AddRegOrMem(in Instruction instruction, int operand, Register regLo, Register regHi, Register vsibIndexRegLo, Register vsibIndexRegHi, bool allowMemOp, bool allowRegOp)
		{
			OpKind opKind = instruction.GetOpKind(operand);
			this.EncoderFlags |= EncoderFlags.ModRM;
			if (opKind == OpKind.Register)
			{
				if (!allowRegOp)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler.AppendLiteral(": register operand is not allowed");
					this.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
					return;
				}
				Register opRegister = instruction.GetOpRegister(operand);
				if (!this.Verify(operand, opRegister, regLo, regHi))
				{
					return;
				}
				uint num = (uint)(opRegister - regLo);
				if (regLo == Register.AL)
				{
					if (opRegister >= Register.R8L)
					{
						num -= 4U;
					}
					else if (opRegister >= Register.SPL)
					{
						num -= 4U;
						this.EncoderFlags |= EncoderFlags.REX;
					}
					else if (opRegister >= Register.AH)
					{
						this.EncoderFlags |= EncoderFlags.HighLegacy8BitRegs;
					}
				}
				this.ModRM |= (byte)(num & 7U);
				this.ModRM |= 192;
				this.EncoderFlags |= (EncoderFlags)((num >> 3) & 3U);
				return;
			}
			else
			{
				if (opKind != OpKind.Memory)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(63, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler2.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler2.AppendLiteral(": Expected a register or memory operand, but opKind is ");
					defaultInterpolatedStringHandler2.AppendFormatted<OpKind>(opKind);
					this.ErrorMessage = defaultInterpolatedStringHandler2.ToStringAndClear();
					return;
				}
				if (!allowMemOp)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(39, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler3.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler3.AppendLiteral(": memory operand is not allowed");
					this.ErrorMessage = defaultInterpolatedStringHandler3.ToStringAndClear();
					return;
				}
				if (instruction.MemorySize.IsBroadcast())
				{
					this.EncoderFlags |= EncoderFlags.Broadcast;
				}
				CodeSize codeSize = instruction.CodeSize;
				if (codeSize == CodeSize.Unknown)
				{
					if (this.bitness == 64)
					{
						codeSize = CodeSize.Code64;
					}
					else if (this.bitness == 32)
					{
						codeSize = CodeSize.Code32;
					}
					else
					{
						codeSize = CodeSize.Code16;
					}
				}
				int num2 = InstructionUtils.GetAddressSizeInBytes(instruction.MemoryBase, instruction.MemoryIndex, instruction.MemoryDisplSize, codeSize) * 8;
				if (num2 != this.bitness)
				{
					this.EncoderFlags |= EncoderFlags.P67;
				}
				if ((this.EncoderFlags & EncoderFlags.RegIsMemory) != EncoderFlags.None && Encoder.GetRegisterOpSize(in instruction) != num2)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(76, 1);
					defaultInterpolatedStringHandler4.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler4.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler4.AppendLiteral(": Register operand size must equal memory addressing mode (16/32/64)");
					this.ErrorMessage = defaultInterpolatedStringHandler4.ToStringAndClear();
					return;
				}
				if (num2 != 16)
				{
					this.AddMemOp(in instruction, operand, num2, vsibIndexRegLo, vsibIndexRegHi);
					return;
				}
				if (vsibIndexRegLo != Register.None)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(91, 1);
					defaultInterpolatedStringHandler5.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler5.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler5.AppendLiteral(": VSIB operands can't use 16-bit addressing. It must be 32-bit or 64-bit addressing");
					this.ErrorMessage = defaultInterpolatedStringHandler5.ToStringAndClear();
					return;
				}
				this.AddMemOp16(in instruction, operand);
				return;
			}
		}

		private static int GetRegisterOpSize(in Instruction instruction)
		{
			if (instruction.Op0Kind == OpKind.Register)
			{
				Register op0Register = instruction.Op0Register;
				if (op0Register.IsGPR64())
				{
					return 64;
				}
				if (op0Register.IsGPR32())
				{
					return 32;
				}
				if (op0Register.IsGPR16())
				{
					return 16;
				}
			}
			return 0;
		}

		private bool TryConvertToDisp8N(in Instruction instruction, int displ, out sbyte compressedValue)
		{
			TryConvertToDisp8N tryConvertToDisp8N = this.handler.TryConvertToDisp8N;
			if (tryConvertToDisp8N != null)
			{
				return tryConvertToDisp8N(this, this.handler, in instruction, displ, out compressedValue);
			}
			if (-128 <= displ && displ <= 127)
			{
				compressedValue = (sbyte)displ;
				return true;
			}
			compressedValue = 0;
			return false;
		}

		private void AddMemOp16(in Instruction instruction, int operand)
		{
			if (this.bitness == 64)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Operand ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler.AppendLiteral(": 16-bit addressing can't be used by 64-bit code");
				this.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				return;
			}
			Register memoryBase = instruction.MemoryBase;
			Register memoryIndex = instruction.MemoryIndex;
			int num = instruction.MemoryDisplSize;
			if (memoryBase != Register.BX || memoryIndex != Register.SI)
			{
				if (memoryBase == Register.BX && memoryIndex == Register.DI)
				{
					this.ModRM |= 1;
				}
				else if (memoryBase == Register.BP && memoryIndex == Register.SI)
				{
					this.ModRM |= 2;
				}
				else if (memoryBase == Register.BP && memoryIndex == Register.DI)
				{
					this.ModRM |= 3;
				}
				else if (memoryBase == Register.SI && memoryIndex == Register.None)
				{
					this.ModRM |= 4;
				}
				else if (memoryBase == Register.DI && memoryIndex == Register.None)
				{
					this.ModRM |= 5;
				}
				else if (memoryBase == Register.BP && memoryIndex == Register.None)
				{
					this.ModRM |= 6;
				}
				else if (memoryBase == Register.BX && memoryIndex == Register.None)
				{
					this.ModRM |= 7;
				}
				else
				{
					if (memoryBase != Register.None || memoryIndex != Register.None)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(62, 3);
						defaultInterpolatedStringHandler2.AppendLiteral("Operand ");
						defaultInterpolatedStringHandler2.AppendFormatted<int>(operand);
						defaultInterpolatedStringHandler2.AppendLiteral(": Invalid 16-bit base + index registers: base=");
						defaultInterpolatedStringHandler2.AppendFormatted<Register>(memoryBase);
						defaultInterpolatedStringHandler2.AppendLiteral(", index=");
						defaultInterpolatedStringHandler2.AppendFormatted<Register>(memoryIndex);
						this.ErrorMessage = defaultInterpolatedStringHandler2.ToStringAndClear();
						return;
					}
					this.ModRM |= 6;
					this.DisplSize = DisplSize.Size2;
					if (instruction.MemoryDisplacement64 > 65535UL)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(43, 1);
						defaultInterpolatedStringHandler3.AppendLiteral("Operand ");
						defaultInterpolatedStringHandler3.AppendFormatted<int>(operand);
						defaultInterpolatedStringHandler3.AppendLiteral(": Displacement must fit in a ushort");
						this.ErrorMessage = defaultInterpolatedStringHandler3.ToStringAndClear();
						return;
					}
					this.Displ = instruction.MemoryDisplacement32;
				}
			}
			if (memoryBase != Register.None || memoryIndex != Register.None)
			{
				if (instruction.MemoryDisplacement64 < 18446744073709518848UL || instruction.MemoryDisplacement64 > 65535UL)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(54, 1);
					defaultInterpolatedStringHandler4.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler4.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler4.AppendLiteral(": Displacement must fit in a short or a ushort");
					this.ErrorMessage = defaultInterpolatedStringHandler4.ToStringAndClear();
					return;
				}
				this.Displ = instruction.MemoryDisplacement32;
				if (num == 0 && memoryBase == Register.BP && memoryIndex == Register.None)
				{
					num = 1;
					if (this.Displ != 0U)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(50, 1);
						defaultInterpolatedStringHandler5.AppendLiteral("Operand ");
						defaultInterpolatedStringHandler5.AppendFormatted<int>(operand);
						defaultInterpolatedStringHandler5.AppendLiteral(": Displacement must be 0 if displSize == 0");
						this.ErrorMessage = defaultInterpolatedStringHandler5.ToStringAndClear();
						return;
					}
				}
				if (num == 1)
				{
					sbyte b;
					if (this.TryConvertToDisp8N(in instruction, (int)((short)this.Displ), out b))
					{
						this.Displ = (uint)b;
					}
					else
					{
						num = 2;
					}
				}
				if (num == 0)
				{
					if (this.Displ != 0U)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(50, 1);
						defaultInterpolatedStringHandler6.AppendLiteral("Operand ");
						defaultInterpolatedStringHandler6.AppendFormatted<int>(operand);
						defaultInterpolatedStringHandler6.AppendLiteral(": Displacement must be 0 if displSize == 0");
						this.ErrorMessage = defaultInterpolatedStringHandler6.ToStringAndClear();
						return;
					}
				}
				else if (num == 1)
				{
					if (this.Displ < 4294967168U || this.Displ > 127U)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(43, 1);
						defaultInterpolatedStringHandler7.AppendLiteral("Operand ");
						defaultInterpolatedStringHandler7.AppendFormatted<int>(operand);
						defaultInterpolatedStringHandler7.AppendLiteral(": Displacement must fit in an sbyte");
						this.ErrorMessage = defaultInterpolatedStringHandler7.ToStringAndClear();
						return;
					}
					this.ModRM |= 64;
					this.DisplSize = DisplSize.Size1;
					return;
				}
				else
				{
					if (num == 2)
					{
						this.ModRM |= 128;
						this.DisplSize = DisplSize.Size2;
						return;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(57, 2);
					defaultInterpolatedStringHandler8.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler8.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler8.AppendLiteral(": Invalid displacement size: ");
					defaultInterpolatedStringHandler8.AppendFormatted<int>(num);
					defaultInterpolatedStringHandler8.AppendLiteral(", must be 0, 1, or 2");
					this.ErrorMessage = defaultInterpolatedStringHandler8.ToStringAndClear();
					return;
				}
			}
		}

		private void AddMemOp(in Instruction instruction, int operand, int addrSize, Register vsibIndexRegLo, Register vsibIndexRegHi)
		{
			if (this.bitness != 64 && addrSize == 64)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Operand ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler.AppendLiteral(": 64-bit addressing can only be used in 64-bit mode");
				this.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				return;
			}
			Register memoryBase = instruction.MemoryBase;
			Register memoryIndex = instruction.MemoryIndex;
			int num = instruction.MemoryDisplSize;
			Register register;
			Register register2;
			if (addrSize == 64)
			{
				register = Register.RAX;
				register2 = Register.R15;
			}
			else
			{
				register = Register.EAX;
				register2 = Register.R15D;
			}
			Register register3;
			Register register4;
			if (vsibIndexRegLo != Register.None)
			{
				register3 = vsibIndexRegLo;
				register4 = vsibIndexRegHi;
			}
			else
			{
				register3 = register;
				register4 = register2;
			}
			if (memoryBase != Register.None && memoryBase != Register.RIP && memoryBase != Register.EIP && !this.Verify(operand, memoryBase, register, register2))
			{
				return;
			}
			if (memoryIndex != Register.None && !this.Verify(operand, memoryIndex, register3, register4))
			{
				return;
			}
			if (num != 0 && num != 1 && num != 4 && num != 8)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(50, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("Operand ");
				defaultInterpolatedStringHandler2.AppendFormatted<int>(operand);
				defaultInterpolatedStringHandler2.AppendLiteral(": Invalid displ size: ");
				defaultInterpolatedStringHandler2.AppendFormatted<int>(num);
				defaultInterpolatedStringHandler2.AppendLiteral(", must be 0, 1, 4, 8");
				this.ErrorMessage = defaultInterpolatedStringHandler2.ToStringAndClear();
				return;
			}
			if (memoryBase == Register.RIP || memoryBase == Register.EIP)
			{
				if (memoryIndex != Register.None)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(61, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler3.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler3.AppendLiteral(": RIP relative addressing can't use an index register");
					this.ErrorMessage = defaultInterpolatedStringHandler3.ToStringAndClear();
					return;
				}
				if (instruction.InternalMemoryIndexScale != 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(51, 1);
					defaultInterpolatedStringHandler4.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler4.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler4.AppendLiteral(": RIP relative addressing must use scale *1");
					this.ErrorMessage = defaultInterpolatedStringHandler4.ToStringAndClear();
					return;
				}
				if (this.bitness != 64)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(70, 1);
					defaultInterpolatedStringHandler5.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler5.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler5.AppendLiteral(": RIP/EIP relative addressing is only available in 64-bit mode");
					this.ErrorMessage = defaultInterpolatedStringHandler5.ToStringAndClear();
					return;
				}
				if ((this.EncoderFlags & EncoderFlags.MustUseSib) != EncoderFlags.None)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(53, 1);
					defaultInterpolatedStringHandler6.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler6.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler6.AppendLiteral(": RIP/EIP relative addressing isn't supported");
					this.ErrorMessage = defaultInterpolatedStringHandler6.ToStringAndClear();
					return;
				}
				this.ModRM |= 5;
				ulong memoryDisplacement = instruction.MemoryDisplacement64;
				if (memoryBase == Register.RIP)
				{
					this.DisplSize = DisplSize.RipRelSize4_Target64;
					this.Displ = (uint)memoryDisplacement;
					this.DisplHi = (uint)(memoryDisplacement >> 32);
					return;
				}
				this.DisplSize = DisplSize.RipRelSize4_Target32;
				if (memoryDisplacement > (ulong)(-1))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(51, 2);
					defaultInterpolatedStringHandler7.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler7.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler7.AppendLiteral(": Target address doesn't fit in 32 bits: 0x");
					defaultInterpolatedStringHandler7.AppendFormatted<ulong>(memoryDisplacement, "X");
					this.ErrorMessage = defaultInterpolatedStringHandler7.ToStringAndClear();
					return;
				}
				this.Displ = (uint)memoryDisplacement;
				return;
			}
			else
			{
				int internalMemoryIndexScale = instruction.InternalMemoryIndexScale;
				this.Displ = instruction.MemoryDisplacement32;
				if (addrSize == 64)
				{
					if (instruction.MemoryDisplacement64 < 18446744071562067968UL || instruction.MemoryDisplacement64 > 2147483647UL)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(41, 1);
						defaultInterpolatedStringHandler8.AppendLiteral("Operand ");
						defaultInterpolatedStringHandler8.AppendFormatted<int>(operand);
						defaultInterpolatedStringHandler8.AppendLiteral(": Displacement must fit in an int");
						this.ErrorMessage = defaultInterpolatedStringHandler8.ToStringAndClear();
						return;
					}
				}
				else if (instruction.MemoryDisplacement64 < 18446744071562067968UL || instruction.MemoryDisplacement64 > (ulong)(-1))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(51, 1);
					defaultInterpolatedStringHandler9.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler9.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler9.AppendLiteral(": Displacement must fit in an int or a uint");
					this.ErrorMessage = defaultInterpolatedStringHandler9.ToStringAndClear();
					return;
				}
				if (memoryBase != Register.None || memoryIndex != Register.None)
				{
					int num2 = ((memoryBase == Register.None) ? (-1) : (memoryBase - register));
					int num3 = ((memoryIndex == Register.None) ? (-1) : (memoryIndex - register3));
					if (num == 0 && (num2 & 7) == 5)
					{
						num = 1;
						if (this.Displ != 0U)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(50, 1);
							defaultInterpolatedStringHandler10.AppendLiteral("Operand ");
							defaultInterpolatedStringHandler10.AppendFormatted<int>(operand);
							defaultInterpolatedStringHandler10.AppendLiteral(": Displacement must be 0 if displSize == 0");
							this.ErrorMessage = defaultInterpolatedStringHandler10.ToStringAndClear();
							return;
						}
					}
					if (num == 1)
					{
						sbyte b;
						if (this.TryConvertToDisp8N(in instruction, (int)this.Displ, out b))
						{
							this.Displ = (uint)b;
						}
						else
						{
							num = addrSize / 8;
						}
					}
					if (memoryBase == Register.None)
					{
						this.DisplSize = DisplSize.Size4;
					}
					else if (num == 1)
					{
						if (this.Displ < 4294967168U || this.Displ > 127U)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler11 = new DefaultInterpolatedStringHandler(43, 1);
							defaultInterpolatedStringHandler11.AppendLiteral("Operand ");
							defaultInterpolatedStringHandler11.AppendFormatted<int>(operand);
							defaultInterpolatedStringHandler11.AppendLiteral(": Displacement must fit in an sbyte");
							this.ErrorMessage = defaultInterpolatedStringHandler11.ToStringAndClear();
							return;
						}
						this.ModRM |= 64;
						this.DisplSize = DisplSize.Size1;
					}
					else if (num == addrSize / 8)
					{
						this.ModRM |= 128;
						this.DisplSize = DisplSize.Size4;
					}
					else
					{
						if (num != 0)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler12 = new DefaultInterpolatedStringHandler(24, 2);
							defaultInterpolatedStringHandler12.AppendLiteral("Operand ");
							defaultInterpolatedStringHandler12.AppendFormatted<int>(operand);
							defaultInterpolatedStringHandler12.AppendLiteral(": Invalid ");
							defaultInterpolatedStringHandler12.AppendFormatted("MemoryDisplSize");
							defaultInterpolatedStringHandler12.AppendLiteral(" value");
							this.ErrorMessage = defaultInterpolatedStringHandler12.ToStringAndClear();
							return;
						}
						if (this.Displ != 0U)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler13 = new DefaultInterpolatedStringHandler(50, 1);
							defaultInterpolatedStringHandler13.AppendLiteral("Operand ");
							defaultInterpolatedStringHandler13.AppendFormatted<int>(operand);
							defaultInterpolatedStringHandler13.AppendLiteral(": Displacement must be 0 if displSize == 0");
							this.ErrorMessage = defaultInterpolatedStringHandler13.ToStringAndClear();
							return;
						}
					}
					if (memoryIndex == Register.None && (num2 & 7) != 4 && internalMemoryIndexScale == 0 && (this.EncoderFlags & EncoderFlags.MustUseSib) == EncoderFlags.None)
					{
						this.ModRM |= (byte)(num2 & 7);
					}
					else
					{
						this.EncoderFlags |= EncoderFlags.Sib;
						this.Sib = (byte)(internalMemoryIndexScale << 6);
						this.ModRM |= 4;
						if (memoryIndex == Register.RSP || memoryIndex == Register.ESP)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler14 = new DefaultInterpolatedStringHandler(52, 1);
							defaultInterpolatedStringHandler14.AppendLiteral("Operand ");
							defaultInterpolatedStringHandler14.AppendFormatted<int>(operand);
							defaultInterpolatedStringHandler14.AppendLiteral(": ESP/RSP can't be used as an index register");
							this.ErrorMessage = defaultInterpolatedStringHandler14.ToStringAndClear();
							return;
						}
						if (num2 < 0)
						{
							this.Sib |= 5;
						}
						else
						{
							this.Sib |= (byte)(num2 & 7);
						}
						if (num3 < 0)
						{
							this.Sib |= 32;
						}
						else
						{
							this.Sib |= (byte)((num3 & 7) << 3);
						}
					}
					if (num2 >= 0)
					{
						this.EncoderFlags |= (EncoderFlags)(num2 >> 3);
					}
					if (num3 >= 0)
					{
						this.EncoderFlags |= (EncoderFlags)((num3 >> 2) & 2);
						this.EncoderFlags |= (EncoderFlags)((num3 & 16) << 27);
					}
					return;
				}
				if (vsibIndexRegLo != Register.None)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler15 = new DefaultInterpolatedStringHandler(58, 1);
					defaultInterpolatedStringHandler15.AppendLiteral("Operand ");
					defaultInterpolatedStringHandler15.AppendFormatted<int>(operand);
					defaultInterpolatedStringHandler15.AppendLiteral(": VSIB addressing can't use an offset-only address");
					this.ErrorMessage = defaultInterpolatedStringHandler15.ToStringAndClear();
					return;
				}
				if (this.bitness == 64 || internalMemoryIndexScale != 0 || (this.EncoderFlags & EncoderFlags.MustUseSib) != EncoderFlags.None)
				{
					this.ModRM |= 4;
					this.DisplSize = DisplSize.Size4;
					this.EncoderFlags |= EncoderFlags.Sib;
					this.Sib = (byte)(37 | (internalMemoryIndexScale << 6));
					return;
				}
				this.ModRM |= 5;
				this.DisplSize = DisplSize.Size4;
				return;
			}
		}

		[Nullable(0)]
		private unsafe static global::System.ReadOnlySpan<byte> SegmentOverrides
		{
			get
			{
				return new global::System.ReadOnlySpan<byte>((void*)(&<b37590d4-39fb-478a-88de-d293f3364852><PrivateImplementationDetails>.776E6876E834221736FF27BADB982C51F52B6A7D95C340DB983274E3E09E9C3D), 6);
			}
		}

		internal unsafe void WritePrefixes(in Instruction instruction, bool canWriteF3 = true)
		{
			Register segmentPrefix = instruction.SegmentPrefix;
			if (segmentPrefix != Register.None)
			{
				this.WriteByteInternal((uint)(*Encoder.SegmentOverrides[segmentPrefix - Register.ES]));
			}
			if ((this.EncoderFlags & EncoderFlags.PF0) != EncoderFlags.None || instruction.HasLockPrefix)
			{
				this.WriteByteInternal(240U);
			}
			if ((this.EncoderFlags & EncoderFlags.P66) != EncoderFlags.None)
			{
				this.WriteByteInternal(102U);
			}
			if ((this.EncoderFlags & EncoderFlags.P67) != EncoderFlags.None)
			{
				this.WriteByteInternal(103U);
			}
			if (canWriteF3 && instruction.HasRepePrefix)
			{
				this.WriteByteInternal(243U);
			}
			if (instruction.HasRepnePrefix)
			{
				this.WriteByteInternal(242U);
			}
		}

		private void WriteModRM()
		{
			if ((this.EncoderFlags & EncoderFlags.ModRM) != EncoderFlags.None)
			{
				this.WriteByteInternal((uint)this.ModRM);
				if ((this.EncoderFlags & EncoderFlags.Sib) != EncoderFlags.None)
				{
					this.WriteByteInternal((uint)this.Sib);
				}
			}
			this.displAddr = (uint)this.currentRip;
			switch (this.DisplSize)
			{
			case DisplSize.None:
				return;
			case DisplSize.Size1:
				this.WriteByteInternal(this.Displ);
				return;
			case DisplSize.Size2:
			{
				uint num = this.Displ;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				return;
			}
			case DisplSize.Size4:
			{
				uint num = this.Displ;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				this.WriteByteInternal(num >> 16);
				this.WriteByteInternal(num >> 24);
				return;
			}
			case DisplSize.Size8:
			{
				uint num = this.Displ;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				this.WriteByteInternal(num >> 16);
				this.WriteByteInternal(num >> 24);
				num = this.DisplHi;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				this.WriteByteInternal(num >> 16);
				this.WriteByteInternal(num >> 24);
				return;
			}
			case DisplSize.RipRelSize4_Target32:
			{
				uint num2 = (uint)this.currentRip + 4U + this.immSizes[(int)this.ImmSize];
				uint num = this.Displ - num2;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				this.WriteByteInternal(num >> 16);
				this.WriteByteInternal(num >> 24);
				return;
			}
			case DisplSize.RipRelSize4_Target64:
			{
				ulong num3 = this.currentRip + 4UL + (ulong)this.immSizes[(int)this.ImmSize];
				long num4 = (long)((((ulong)this.DisplHi << 32) | (ulong)this.Displ) - num3);
				if (num4 < -2147483648L || num4 > 2147483647L)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(96, 4);
					defaultInterpolatedStringHandler.AppendLiteral("RIP relative distance is too far away: NextIP: 0x");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(num3, "X16");
					defaultInterpolatedStringHandler.AppendLiteral(" target: 0x");
					defaultInterpolatedStringHandler.AppendFormatted<uint>(this.DisplHi, "X8");
					defaultInterpolatedStringHandler.AppendFormatted<uint>(this.Displ, "X8");
					defaultInterpolatedStringHandler.AppendLiteral(", diff = ");
					defaultInterpolatedStringHandler.AppendFormatted<long>(num4);
					defaultInterpolatedStringHandler.AppendLiteral(", diff must fit in an Int32");
					this.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				uint num = (uint)num4;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				this.WriteByteInternal(num >> 16);
				this.WriteByteInternal(num >> 24);
				return;
			}
			default:
				throw new InvalidOperationException();
			}
		}

		private void WriteImmediate()
		{
			this.immAddr = (uint)this.currentRip;
			switch (this.ImmSize)
			{
			case ImmSize.None:
				return;
			case ImmSize.Size1:
			case ImmSize.SizeIbReg:
			case ImmSize.Size1OpCode:
				this.WriteByteInternal(this.Immediate);
				return;
			case ImmSize.Size2:
			{
				uint num = this.Immediate;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				return;
			}
			case ImmSize.Size4:
			{
				uint num = this.Immediate;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				this.WriteByteInternal(num >> 16);
				this.WriteByteInternal(num >> 24);
				return;
			}
			case ImmSize.Size8:
			{
				uint num = this.Immediate;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				this.WriteByteInternal(num >> 16);
				this.WriteByteInternal(num >> 24);
				num = this.ImmediateHi;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				this.WriteByteInternal(num >> 16);
				this.WriteByteInternal(num >> 24);
				return;
			}
			case ImmSize.Size2_1:
			{
				uint num = this.Immediate;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				this.WriteByteInternal(this.ImmediateHi);
				return;
			}
			case ImmSize.Size1_1:
				this.WriteByteInternal(this.Immediate);
				this.WriteByteInternal(this.ImmediateHi);
				return;
			case ImmSize.Size2_2:
			{
				uint num = this.Immediate;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				num = this.ImmediateHi;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				return;
			}
			case ImmSize.Size4_2:
			{
				uint num = this.Immediate;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				this.WriteByteInternal(num >> 16);
				this.WriteByteInternal(num >> 24);
				num = this.ImmediateHi;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				return;
			}
			case ImmSize.RipRelSize1_Target16:
			{
				ushort num2 = (ushort)((uint)this.currentRip + 1U);
				short num3 = (short)this.Immediate - (short)num2;
				if (num3 < -128 || num3 > 127)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(89, 3);
					defaultInterpolatedStringHandler.AppendLiteral("Branch distance is too far away: NextIP: 0x");
					defaultInterpolatedStringHandler.AppendFormatted<ushort>(num2, "X4");
					defaultInterpolatedStringHandler.AppendLiteral(" target: 0x");
					defaultInterpolatedStringHandler.AppendFormatted<ushort>((ushort)this.Immediate, "X4");
					defaultInterpolatedStringHandler.AppendLiteral(", diff = ");
					defaultInterpolatedStringHandler.AppendFormatted<short>(num3);
					defaultInterpolatedStringHandler.AppendLiteral(", diff must fit in an Int8");
					this.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				this.WriteByteInternal((uint)num3);
				return;
			}
			case ImmSize.RipRelSize1_Target32:
			{
				uint num4 = (uint)this.currentRip + 1U;
				int num5 = (int)(this.Immediate - num4);
				if (num5 < -128 || num5 > 127)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(89, 3);
					defaultInterpolatedStringHandler2.AppendLiteral("Branch distance is too far away: NextIP: 0x");
					defaultInterpolatedStringHandler2.AppendFormatted<uint>(num4, "X8");
					defaultInterpolatedStringHandler2.AppendLiteral(" target: 0x");
					defaultInterpolatedStringHandler2.AppendFormatted<uint>(this.Immediate, "X8");
					defaultInterpolatedStringHandler2.AppendLiteral(", diff = ");
					defaultInterpolatedStringHandler2.AppendFormatted<int>(num5);
					defaultInterpolatedStringHandler2.AppendLiteral(", diff must fit in an Int8");
					this.ErrorMessage = defaultInterpolatedStringHandler2.ToStringAndClear();
				}
				this.WriteByteInternal((uint)num5);
				return;
			}
			case ImmSize.RipRelSize1_Target64:
			{
				ulong num6 = this.currentRip + 1UL;
				long num7 = (long)((((ulong)this.ImmediateHi << 32) | (ulong)this.Immediate) - num6);
				if (num7 < -128L || num7 > 127L)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(89, 4);
					defaultInterpolatedStringHandler3.AppendLiteral("Branch distance is too far away: NextIP: 0x");
					defaultInterpolatedStringHandler3.AppendFormatted<ulong>(num6, "X16");
					defaultInterpolatedStringHandler3.AppendLiteral(" target: 0x");
					defaultInterpolatedStringHandler3.AppendFormatted<uint>(this.ImmediateHi, "X8");
					defaultInterpolatedStringHandler3.AppendFormatted<uint>(this.Immediate, "X8");
					defaultInterpolatedStringHandler3.AppendLiteral(", diff = ");
					defaultInterpolatedStringHandler3.AppendFormatted<long>(num7);
					defaultInterpolatedStringHandler3.AppendLiteral(", diff must fit in an Int8");
					this.ErrorMessage = defaultInterpolatedStringHandler3.ToStringAndClear();
				}
				this.WriteByteInternal((uint)num7);
				return;
			}
			case ImmSize.RipRelSize2_Target16:
			{
				uint num4 = (uint)this.currentRip + 2U;
				uint num = this.Immediate - num4;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				return;
			}
			case ImmSize.RipRelSize2_Target32:
			{
				uint num4 = (uint)this.currentRip + 2U;
				int num5 = (int)(this.Immediate - num4);
				if (num5 < -32768 || num5 > 32767)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(90, 3);
					defaultInterpolatedStringHandler4.AppendLiteral("Branch distance is too far away: NextIP: 0x");
					defaultInterpolatedStringHandler4.AppendFormatted<uint>(num4, "X8");
					defaultInterpolatedStringHandler4.AppendLiteral(" target: 0x");
					defaultInterpolatedStringHandler4.AppendFormatted<uint>(this.Immediate, "X8");
					defaultInterpolatedStringHandler4.AppendLiteral(", diff = ");
					defaultInterpolatedStringHandler4.AppendFormatted<int>(num5);
					defaultInterpolatedStringHandler4.AppendLiteral(", diff must fit in an Int16");
					this.ErrorMessage = defaultInterpolatedStringHandler4.ToStringAndClear();
				}
				uint num = (uint)num5;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				return;
			}
			case ImmSize.RipRelSize2_Target64:
			{
				ulong num6 = this.currentRip + 2UL;
				long num7 = (long)((((ulong)this.ImmediateHi << 32) | (ulong)this.Immediate) - num6);
				if (num7 < -32768L || num7 > 32767L)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(90, 4);
					defaultInterpolatedStringHandler5.AppendLiteral("Branch distance is too far away: NextIP: 0x");
					defaultInterpolatedStringHandler5.AppendFormatted<ulong>(num6, "X16");
					defaultInterpolatedStringHandler5.AppendLiteral(" target: 0x");
					defaultInterpolatedStringHandler5.AppendFormatted<uint>(this.ImmediateHi, "X8");
					defaultInterpolatedStringHandler5.AppendFormatted<uint>(this.Immediate, "X8");
					defaultInterpolatedStringHandler5.AppendLiteral(", diff = ");
					defaultInterpolatedStringHandler5.AppendFormatted<long>(num7);
					defaultInterpolatedStringHandler5.AppendLiteral(", diff must fit in an Int16");
					this.ErrorMessage = defaultInterpolatedStringHandler5.ToStringAndClear();
				}
				uint num = (uint)num7;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				return;
			}
			case ImmSize.RipRelSize4_Target32:
			{
				uint num4 = (uint)this.currentRip + 4U;
				uint num = this.Immediate - num4;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				this.WriteByteInternal(num >> 16);
				this.WriteByteInternal(num >> 24);
				return;
			}
			case ImmSize.RipRelSize4_Target64:
			{
				ulong num6 = this.currentRip + 4UL;
				long num7 = (long)((((ulong)this.ImmediateHi << 32) | (ulong)this.Immediate) - num6);
				if (num7 < -2147483648L || num7 > 2147483647L)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(90, 4);
					defaultInterpolatedStringHandler6.AppendLiteral("Branch distance is too far away: NextIP: 0x");
					defaultInterpolatedStringHandler6.AppendFormatted<ulong>(num6, "X16");
					defaultInterpolatedStringHandler6.AppendLiteral(" target: 0x");
					defaultInterpolatedStringHandler6.AppendFormatted<uint>(this.ImmediateHi, "X8");
					defaultInterpolatedStringHandler6.AppendFormatted<uint>(this.Immediate, "X8");
					defaultInterpolatedStringHandler6.AppendLiteral(", diff = ");
					defaultInterpolatedStringHandler6.AppendFormatted<long>(num7);
					defaultInterpolatedStringHandler6.AppendLiteral(", diff must fit in an Int32");
					this.ErrorMessage = defaultInterpolatedStringHandler6.ToStringAndClear();
				}
				uint num = (uint)num7;
				this.WriteByteInternal(num);
				this.WriteByteInternal(num >> 8);
				this.WriteByteInternal(num >> 16);
				this.WriteByteInternal(num >> 24);
				return;
			}
			default:
				throw new InvalidOperationException();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteByte(byte value)
		{
			this.WriteByteInternal((uint)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void WriteByteInternal(uint value)
		{
			this.writer.WriteByte((byte)value);
			this.currentRip += 1UL;
		}

		public ConstantOffsets GetConstantOffsets()
		{
			ConstantOffsets constantOffsets = default(ConstantOffsets);
			switch (this.DisplSize)
			{
			case DisplSize.None:
				break;
			case DisplSize.Size1:
				constantOffsets.DisplacementSize = 1;
				constantOffsets.DisplacementOffset = (byte)(this.displAddr - this.eip);
				break;
			case DisplSize.Size2:
				constantOffsets.DisplacementSize = 2;
				constantOffsets.DisplacementOffset = (byte)(this.displAddr - this.eip);
				break;
			case DisplSize.Size4:
			case DisplSize.RipRelSize4_Target32:
			case DisplSize.RipRelSize4_Target64:
				constantOffsets.DisplacementSize = 4;
				constantOffsets.DisplacementOffset = (byte)(this.displAddr - this.eip);
				break;
			case DisplSize.Size8:
				constantOffsets.DisplacementSize = 8;
				constantOffsets.DisplacementOffset = (byte)(this.displAddr - this.eip);
				break;
			default:
				throw new InvalidOperationException();
			}
			switch (this.ImmSize)
			{
			case ImmSize.None:
			case ImmSize.SizeIbReg:
			case ImmSize.Size1OpCode:
				break;
			case ImmSize.Size1:
			case ImmSize.RipRelSize1_Target16:
			case ImmSize.RipRelSize1_Target32:
			case ImmSize.RipRelSize1_Target64:
				constantOffsets.ImmediateSize = 1;
				constantOffsets.ImmediateOffset = (byte)(this.immAddr - this.eip);
				break;
			case ImmSize.Size2:
			case ImmSize.RipRelSize2_Target16:
			case ImmSize.RipRelSize2_Target32:
			case ImmSize.RipRelSize2_Target64:
				constantOffsets.ImmediateSize = 2;
				constantOffsets.ImmediateOffset = (byte)(this.immAddr - this.eip);
				break;
			case ImmSize.Size4:
			case ImmSize.RipRelSize4_Target32:
			case ImmSize.RipRelSize4_Target64:
				constantOffsets.ImmediateSize = 4;
				constantOffsets.ImmediateOffset = (byte)(this.immAddr - this.eip);
				break;
			case ImmSize.Size8:
				constantOffsets.ImmediateSize = 8;
				constantOffsets.ImmediateOffset = (byte)(this.immAddr - this.eip);
				break;
			case ImmSize.Size2_1:
				constantOffsets.ImmediateSize = 2;
				constantOffsets.ImmediateOffset = (byte)(this.immAddr - this.eip);
				constantOffsets.ImmediateSize2 = 1;
				constantOffsets.ImmediateOffset2 = (byte)(this.immAddr - this.eip + 2U);
				break;
			case ImmSize.Size1_1:
				constantOffsets.ImmediateSize = 1;
				constantOffsets.ImmediateOffset = (byte)(this.immAddr - this.eip);
				constantOffsets.ImmediateSize2 = 1;
				constantOffsets.ImmediateOffset2 = (byte)(this.immAddr - this.eip + 1U);
				break;
			case ImmSize.Size2_2:
				constantOffsets.ImmediateSize = 2;
				constantOffsets.ImmediateOffset = (byte)(this.immAddr - this.eip);
				constantOffsets.ImmediateSize2 = 2;
				constantOffsets.ImmediateOffset2 = (byte)(this.immAddr - this.eip + 2U);
				break;
			case ImmSize.Size4_2:
				constantOffsets.ImmediateSize = 4;
				constantOffsets.ImmediateOffset = (byte)(this.immAddr - this.eip);
				constantOffsets.ImmediateSize2 = 2;
				constantOffsets.ImmediateOffset2 = (byte)(this.immAddr - this.eip + 4U);
				break;
			default:
				throw new InvalidOperationException();
			}
			return constantOffsets;
		}

		private static readonly uint[] s_immSizes = new uint[]
		{
			0U, 1U, 2U, 4U, 8U, 3U, 2U, 4U, 6U, 1U,
			1U, 1U, 2U, 2U, 2U, 4U, 4U, 1U, 1U
		};

		internal uint Internal_PreventVEX2;

		internal uint Internal_VEX_WIG_LIG;

		internal uint Internal_VEX_LIG;

		internal uint Internal_EVEX_WIG;

		internal uint Internal_EVEX_LIG;

		internal const string ERROR_ONLY_1632_BIT_MODE = "The instruction can only be used in 16/32-bit mode";

		internal const string ERROR_ONLY_64_BIT_MODE = "The instruction can only be used in 64-bit mode";

		private readonly CodeWriter writer;

		private readonly int bitness;

		private readonly OpCodeHandler[] handlers;

		private readonly uint[] immSizes;

		private ulong currentRip;

		private string errorMessage;

		private OpCodeHandler handler;

		private uint eip;

		private uint displAddr;

		private uint immAddr;

		internal uint Immediate;

		internal uint ImmediateHi;

		private uint Displ;

		private uint DisplHi;

		private readonly EncoderFlags opSize16Flags;

		private readonly EncoderFlags opSize32Flags;

		private readonly EncoderFlags adrSize16Flags;

		private readonly EncoderFlags adrSize32Flags;

		internal uint OpCode;

		internal EncoderFlags EncoderFlags;

		private DisplSize DisplSize;

		internal ImmSize ImmSize;

		private byte ModRM;

		private byte Sib;
	}
}
