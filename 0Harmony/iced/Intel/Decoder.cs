using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Iced.Intel.DecoderInternal;

namespace Iced.Intel
{
	internal sealed class Decoder : IEnumerable<Instruction>, IEnumerable
	{
		public ulong IP
		{
			get
			{
				return this.instructionPointer;
			}
			set
			{
				this.instructionPointer = value;
			}
		}

		public int Bitness { get; }

		static Decoder()
		{
			OpCodeHandler_Invalid instance = OpCodeHandler_Invalid.Instance;
			global::System.ReadOnlySpan<byte> sizesNormal = InstructionMemorySizes.SizesNormal;
			Code[] codeValues = OpCodeHandler_D3NOW.CodeValues;
			global::System.ReadOnlySpan<byte> opCount = InstructionOpCounts.OpCount;
			ushort[] toMnemonic = MnemonicUtilsData.toMnemonic;
		}

		private Decoder(CodeReader reader, ulong ip, DecoderOptions options, int bitness)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.reader = reader;
			this.instructionPointer = ip;
			this.options = options;
			this.invalidCheckMask = (((options & DecoderOptions.NoInvalidCheck) == DecoderOptions.None) ? uint.MaxValue : 0U);
			this.memRegs16 = Decoder.s_memRegs16;
			this.Bitness = bitness;
			if (bitness == 64)
			{
				this.is64bMode = true;
				this.defaultCodeSize = CodeSize.Code64;
				this.defaultOperandSize = OpSize.Size32;
				this.defaultInvertedOperandSize = OpSize.Size16;
				this.defaultAddressSize = OpSize.Size64;
				this.defaultInvertedAddressSize = OpSize.Size32;
				this.maskE0 = 224U;
				this.rexMask = 240U;
			}
			else if (bitness == 32)
			{
				this.is64bMode = false;
				this.defaultCodeSize = CodeSize.Code32;
				this.defaultOperandSize = OpSize.Size32;
				this.defaultInvertedOperandSize = OpSize.Size16;
				this.defaultAddressSize = OpSize.Size32;
				this.defaultInvertedAddressSize = OpSize.Size16;
				this.maskE0 = 0U;
				this.rexMask = 0U;
			}
			else
			{
				this.is64bMode = false;
				this.defaultCodeSize = CodeSize.Code16;
				this.defaultOperandSize = OpSize.Size16;
				this.defaultInvertedOperandSize = OpSize.Size32;
				this.defaultAddressSize = OpSize.Size16;
				this.defaultInvertedAddressSize = OpSize.Size32;
				this.maskE0 = 0U;
				this.rexMask = 0U;
			}
			this.is64bMode_and_W = (this.is64bMode ? 128U : 0U);
			this.reg15Mask = (this.is64bMode ? 15U : 7U);
			this.handlers_MAP0 = OpCodeHandlersTables_Legacy.Handlers_MAP0;
			this.handlers_VEX_0F = OpCodeHandlersTables_VEX.Handlers_0F;
			this.handlers_VEX_0F38 = OpCodeHandlersTables_VEX.Handlers_0F38;
			this.handlers_VEX_0F3A = OpCodeHandlersTables_VEX.Handlers_0F3A;
			this.handlers_EVEX_0F = OpCodeHandlersTables_EVEX.Handlers_0F;
			this.handlers_EVEX_0F38 = OpCodeHandlersTables_EVEX.Handlers_0F38;
			this.handlers_EVEX_0F3A = OpCodeHandlersTables_EVEX.Handlers_0F3A;
			this.handlers_EVEX_MAP5 = OpCodeHandlersTables_EVEX.Handlers_MAP5;
			this.handlers_EVEX_MAP6 = OpCodeHandlersTables_EVEX.Handlers_MAP6;
			this.handlers_XOP_MAP8 = OpCodeHandlersTables_XOP.Handlers_MAP8;
			this.handlers_XOP_MAP9 = OpCodeHandlersTables_XOP.Handlers_MAP9;
			this.handlers_XOP_MAP10 = OpCodeHandlersTables_XOP.Handlers_MAP10;
		}

		[NullableContext(1)]
		public static Decoder Create(int bitness, CodeReader reader, ulong ip, DecoderOptions options = DecoderOptions.None)
		{
			if (bitness == 16 || bitness == 32 || bitness == 64)
			{
				return new Decoder(reader, ip, options, bitness);
			}
			throw new ArgumentOutOfRangeException("bitness");
		}

		[NullableContext(1)]
		public static Decoder Create(int bitness, byte[] data, ulong ip, DecoderOptions options = DecoderOptions.None)
		{
			return Decoder.Create(bitness, new ByteArrayCodeReader(data), ip, options);
		}

		[NullableContext(1)]
		public static Decoder Create(int bitness, CodeReader reader, DecoderOptions options = DecoderOptions.None)
		{
			return Decoder.Create(bitness, reader, 0UL, options);
		}

		[NullableContext(1)]
		public static Decoder Create(int bitness, byte[] data, DecoderOptions options = DecoderOptions.None)
		{
			return Decoder.Create(bitness, new ByteArrayCodeReader(data), 0UL, options);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal uint ReadByte()
		{
			uint instructionLength = this.state.zs.instructionLength;
			if (instructionLength < 15U)
			{
				uint num = (uint)this.reader.ReadByte();
				if (num <= 255U)
				{
					this.state.zs.instructionLength = instructionLength + 1U;
					return num;
				}
				this.state.zs.flags = this.state.zs.flags | StateFlags.NoMoreBytes;
			}
			this.state.zs.flags = this.state.zs.flags | StateFlags.IsInvalid;
			return 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal uint ReadUInt16()
		{
			return this.ReadByte() | (this.ReadByte() << 8);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal uint ReadUInt32()
		{
			return this.ReadByte() | (this.ReadByte() << 8) | (this.ReadByte() << 16) | (this.ReadByte() << 24);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ulong ReadUInt64()
		{
			return (ulong)this.ReadUInt32() | ((ulong)this.ReadUInt32() << 32);
		}

		public DecoderError LastError
		{
			get
			{
				if ((this.state.zs.flags & StateFlags.NoMoreBytes) != (StateFlags)0U)
				{
					return DecoderError.NoMoreBytes;
				}
				if ((this.state.zs.flags & StateFlags.IsInvalid) != (StateFlags)0U)
				{
					return DecoderError.InvalidInstruction;
				}
				return DecoderError.None;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Instruction Decode()
		{
			Instruction instruction;
			this.Decode(out instruction);
			return instruction;
		}

		public void Decode(out Instruction instruction)
		{
			instruction = default(Instruction);
			this.state.zs = default(Decoder.ZState);
			this.state.operandSize = this.defaultOperandSize;
			this.state.addressSize = this.defaultAddressSize;
			uint num = this.ReadByte();
			if ((num & this.rexMask) == 64U)
			{
				StateFlags stateFlags = this.state.zs.flags | StateFlags.HasRex;
				if ((num & 8U) != 0U)
				{
					stateFlags |= StateFlags.W;
					this.state.operandSize = OpSize.Size64;
				}
				this.state.zs.flags = stateFlags;
				this.state.zs.extraRegisterBase = (num << 1) & 8U;
				this.state.zs.extraIndexRegisterBase = (num << 2) & 8U;
				this.state.zs.extraBaseRegisterBase = (num << 3) & 8U;
				num = this.ReadByte();
			}
			this.DecodeTable(this.handlers_MAP0[(int)num], ref instruction);
			instruction.InternalCodeSize = this.defaultCodeSize;
			uint instructionLength = this.state.zs.instructionLength;
			instruction.Length = (int)instructionLength;
			ulong num2 = this.instructionPointer;
			num2 += (ulong)instructionLength;
			this.instructionPointer = num2;
			instruction.NextIP = num2;
			StateFlags flags = this.state.zs.flags;
			if ((flags & (StateFlags.IpRel64 | StateFlags.IpRel32 | StateFlags.IsInvalid | StateFlags.Lock)) != (StateFlags)0U)
			{
				ulong num3 = instruction.MemoryDisplacement64 + num2;
				instruction.MemoryDisplacement64 = num3;
				if ((flags & (StateFlags.IpRel64 | StateFlags.IsInvalid | StateFlags.Lock)) == StateFlags.IpRel64)
				{
					return;
				}
				if ((flags & StateFlags.IpRel64) == (StateFlags)0U)
				{
					instruction.MemoryDisplacement64 = num3 - num2;
				}
				if ((flags & StateFlags.IpRel32) != (StateFlags)0U)
				{
					instruction.MemoryDisplacement64 = (ulong)((uint)instruction.MemoryDisplacement64 + (uint)num2);
				}
				if ((flags & StateFlags.IsInvalid) != (StateFlags)0U || (flags & (StateFlags.Lock | StateFlags.AllowLock) & (StateFlags)this.invalidCheckMask) == StateFlags.Lock)
				{
					instruction = default(Instruction);
					this.state.zs.flags = flags | StateFlags.IsInvalid;
					instruction.InternalCodeSize = this.defaultCodeSize;
					instruction.Length = (int)instructionLength;
					instruction.NextIP = num2;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ResetRexPrefixState()
		{
			this.state.zs.flags = this.state.zs.flags & ~(StateFlags.HasRex | StateFlags.W);
			if ((this.state.zs.flags & StateFlags.Has66) == (StateFlags)0U)
			{
				this.state.operandSize = this.defaultOperandSize;
			}
			else
			{
				this.state.operandSize = this.defaultInvertedOperandSize;
			}
			this.state.zs.extraRegisterBase = 0U;
			this.state.zs.extraIndexRegisterBase = 0U;
			this.state.zs.extraBaseRegisterBase = 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void CallOpCodeHandlerXXTable(ref Instruction instruction)
		{
			uint num = this.ReadByte();
			this.DecodeTable(this.handlers_MAP0[(int)num], ref instruction);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal uint GetCurrentInstructionPointer32()
		{
			return (uint)this.instructionPointer + this.state.zs.instructionLength;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ulong GetCurrentInstructionPointer64()
		{
			return this.instructionPointer + (ulong)this.state.zs.instructionLength;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ClearMandatoryPrefix(ref Instruction instruction)
		{
			instruction.InternalClearHasRepeRepnePrefix();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void SetXacquireXrelease(ref Instruction instruction)
		{
			if (instruction.HasLockPrefix)
			{
				if (this.state.zs.mandatoryPrefix == MandatoryPrefixByte.PF2)
				{
					this.ClearMandatoryPrefixF2(ref instruction);
					instruction.InternalSetHasXacquirePrefix();
					return;
				}
				if (this.state.zs.mandatoryPrefix == MandatoryPrefixByte.PF3)
				{
					this.ClearMandatoryPrefixF3(ref instruction);
					instruction.InternalSetHasXreleasePrefix();
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ClearMandatoryPrefixF3(ref Instruction instruction)
		{
			instruction.InternalClearHasRepePrefix();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ClearMandatoryPrefixF2(ref Instruction instruction)
		{
			instruction.InternalClearHasRepnePrefix();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void SetInvalidInstruction()
		{
			this.state.zs.flags = this.state.zs.flags | StateFlags.IsInvalid;
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void DecodeTable(OpCodeHandler[] table, ref Instruction instruction)
		{
			this.DecodeTable(table[(int)this.ReadByte()], ref instruction);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DecodeTable(OpCodeHandler handler, ref Instruction instruction)
		{
			if (handler.HasModRM)
			{
				uint num = this.ReadByte();
				this.state.modrm = num;
				this.state.mod = num >> 6;
				this.state.reg = (num >> 3) & 7U;
				this.state.rm = num & 7U;
			}
			handler.Decode(this, ref instruction);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ReadModRM()
		{
			uint num = this.ReadByte();
			this.state.modrm = num;
			this.state.mod = num >> 6;
			this.state.reg = (num >> 3) & 7U;
			this.state.rm = num & 7U;
		}

		internal void VEX2(ref Instruction instruction)
		{
			if ((((this.state.zs.flags & StateFlags.HasRex) | (StateFlags)this.state.zs.mandatoryPrefix) & (StateFlags)this.invalidCheckMask) != (StateFlags)0U)
			{
				this.SetInvalidInstruction();
			}
			this.state.zs.flags = this.state.zs.flags & ~StateFlags.W;
			this.state.zs.extraIndexRegisterBase = 0U;
			this.state.zs.extraBaseRegisterBase = 0U;
			uint num = this.state.modrm;
			this.state.vectorLength = (num >> 2) & 1U;
			this.state.zs.mandatoryPrefix = (MandatoryPrefixByte)(num & 3U);
			num = ~num;
			this.state.zs.extraRegisterBase = (num >> 4) & 8U;
			num = (num >> 3) & 15U;
			this.state.vvvv = num;
			this.state.vvvv_invalidCheck = num;
			this.DecodeTable(this.handlers_VEX_0F, ref instruction);
		}

		internal void VEX3(ref Instruction instruction)
		{
			if ((((this.state.zs.flags & StateFlags.HasRex) | (StateFlags)this.state.zs.mandatoryPrefix) & (StateFlags)this.invalidCheckMask) != (StateFlags)0U)
			{
				this.SetInvalidInstruction();
			}
			this.state.zs.flags = this.state.zs.flags & ~StateFlags.W;
			uint num = this.ReadByte();
			this.state.zs.flags = this.state.zs.flags | (StateFlags)(num & 128U);
			this.state.vectorLength = (num >> 2) & 1U;
			this.state.zs.mandatoryPrefix = (MandatoryPrefixByte)(num & 3U);
			num = (~num >> 3) & 15U;
			this.state.vvvv_invalidCheck = num;
			this.state.vvvv = num & this.reg15Mask;
			uint modrm = this.state.modrm;
			uint num2 = ~modrm & this.maskE0;
			this.state.zs.extraRegisterBase = (num2 >> 4) & 8U;
			this.state.zs.extraIndexRegisterBase = (num2 >> 3) & 8U;
			this.state.zs.extraBaseRegisterBase = (num2 >> 2) & 8U;
			uint num3 = this.ReadByte();
			int num4 = (int)(modrm & 31U);
			OpCodeHandler[] array;
			if (num4 == 1)
			{
				array = this.handlers_VEX_0F;
			}
			else if (num4 == 2)
			{
				array = this.handlers_VEX_0F38;
			}
			else
			{
				if (num4 != 3)
				{
					this.SetInvalidInstruction();
					return;
				}
				array = this.handlers_VEX_0F3A;
			}
			this.DecodeTable(array[(int)num3], ref instruction);
		}

		internal void XOP(ref Instruction instruction)
		{
			if ((((this.state.zs.flags & StateFlags.HasRex) | (StateFlags)this.state.zs.mandatoryPrefix) & (StateFlags)this.invalidCheckMask) != (StateFlags)0U)
			{
				this.SetInvalidInstruction();
			}
			this.state.zs.flags = this.state.zs.flags & ~StateFlags.W;
			uint num = this.ReadByte();
			this.state.zs.flags = this.state.zs.flags | (StateFlags)(num & 128U);
			this.state.vectorLength = (num >> 2) & 1U;
			this.state.zs.mandatoryPrefix = (MandatoryPrefixByte)(num & 3U);
			num = (~num >> 3) & 15U;
			this.state.vvvv_invalidCheck = num;
			this.state.vvvv = num & this.reg15Mask;
			uint modrm = this.state.modrm;
			uint num2 = ~modrm & this.maskE0;
			this.state.zs.extraRegisterBase = (num2 >> 4) & 8U;
			this.state.zs.extraIndexRegisterBase = (num2 >> 3) & 8U;
			this.state.zs.extraBaseRegisterBase = (num2 >> 2) & 8U;
			uint num3 = this.ReadByte();
			int num4 = (int)(modrm & 31U);
			OpCodeHandler[] array;
			if (num4 == 8)
			{
				array = this.handlers_XOP_MAP8;
			}
			else if (num4 == 9)
			{
				array = this.handlers_XOP_MAP9;
			}
			else
			{
				if (num4 != 10)
				{
					this.SetInvalidInstruction();
					return;
				}
				array = this.handlers_XOP_MAP10;
			}
			this.DecodeTable(array[(int)num3], ref instruction);
		}

		internal void EVEX_MVEX(ref Instruction instruction)
		{
			if ((((this.state.zs.flags & StateFlags.HasRex) | (StateFlags)this.state.zs.mandatoryPrefix) & (StateFlags)this.invalidCheckMask) != (StateFlags)0U)
			{
				this.SetInvalidInstruction();
			}
			this.state.zs.flags = this.state.zs.flags & ~StateFlags.W;
			uint modrm = this.state.modrm;
			uint num = this.ReadByte();
			uint num2 = this.ReadByte();
			uint num3 = this.ReadByte();
			uint num4 = this.ReadByte();
			if ((num & 4U) == 0U)
			{
				this.SetInvalidInstruction();
				return;
			}
			if ((modrm & 8U) == 0U)
			{
				this.state.zs.mandatoryPrefix = (MandatoryPrefixByte)(num & 3U);
				this.state.zs.flags = this.state.zs.flags | (StateFlags)(num & 128U);
				uint num5 = num2 & 7U;
				this.state.aaa = num5;
				instruction.InternalOpMask = num5;
				if ((num2 & 128U) != 0U)
				{
					if ((num5 ^ this.invalidCheckMask) == 4294967295U)
					{
						this.SetInvalidInstruction();
					}
					this.state.zs.flags = this.state.zs.flags | StateFlags.z;
					instruction.InternalSetZeroingMasking();
				}
				this.state.zs.flags = this.state.zs.flags | (StateFlags)(num2 & 16U);
				this.state.vectorLength = (num2 >> 5) & 3U;
				num = (~num >> 3) & 15U;
				if (this.is64bMode)
				{
					uint num6 = (~num2 & 8U) << 1;
					this.state.zs.extraIndexRegisterBaseVSIB = num6;
					num6 += num;
					this.state.vvvv = num6;
					this.state.vvvv_invalidCheck = num6;
					uint num7 = ~modrm;
					this.state.zs.extraRegisterBase = (num7 >> 4) & 8U;
					this.state.zs.extraIndexRegisterBase = (num7 >> 3) & 8U;
					this.state.extraRegisterBaseEVEX = num7 & 16U;
					num7 >>= 2;
					this.state.extraBaseRegisterBaseEVEX = num7 & 24U;
					this.state.zs.extraBaseRegisterBase = num7 & 8U;
				}
				else
				{
					this.state.vvvv_invalidCheck = num;
					this.state.vvvv = num & 7U;
					this.state.zs.flags = this.state.zs.flags | (StateFlags)((~num2 & 8U) << 3);
				}
				OpCodeHandler[] array;
				switch (modrm & 7U)
				{
				case 1U:
					array = this.handlers_EVEX_0F;
					goto IL_0279;
				case 2U:
					array = this.handlers_EVEX_0F38;
					goto IL_0279;
				case 3U:
					array = this.handlers_EVEX_0F3A;
					goto IL_0279;
				case 5U:
					array = this.handlers_EVEX_MAP5;
					goto IL_0279;
				case 6U:
					array = this.handlers_EVEX_MAP6;
					goto IL_0279;
				}
				this.SetInvalidInstruction();
				return;
				IL_0279:
				OpCodeHandler opCodeHandler = array[(int)num3];
				this.state.modrm = num4;
				this.state.mod = num4 >> 6;
				this.state.reg = (num4 >> 3) & 7U;
				this.state.rm = num4 & 7U;
				if ((((this.state.zs.flags & StateFlags.b) | (StateFlags)this.state.vectorLength) & (StateFlags)this.invalidCheckMask) == (StateFlags.IpRel64 | StateFlags.IpRel32))
				{
					this.SetInvalidInstruction();
				}
				opCodeHandler.Decode(this, ref instruction);
				return;
			}
			this.SetInvalidInstruction();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Register ReadOpSegReg()
		{
			uint reg = this.state.reg;
			if (reg < 6U)
			{
				return Register.ES + (int)reg;
			}
			this.SetInvalidInstruction();
			return Register.None;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool ReadOpMem(ref Instruction instruction)
		{
			if (this.state.addressSize == OpSize.Size64)
			{
				return this.ReadOpMem32Or64(ref instruction, Register.RAX, Register.RAX, TupleType.N1, false);
			}
			if (this.state.addressSize == OpSize.Size32)
			{
				return this.ReadOpMem32Or64(ref instruction, Register.EAX, Register.EAX, TupleType.N1, false);
			}
			this.ReadOpMem16(ref instruction, TupleType.N1);
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ReadOpMemSib(ref Instruction instruction)
		{
			bool flag;
			if (this.state.addressSize == OpSize.Size64)
			{
				flag = this.ReadOpMem32Or64(ref instruction, Register.RAX, Register.RAX, TupleType.N1, false);
			}
			else if (this.state.addressSize == OpSize.Size32)
			{
				flag = this.ReadOpMem32Or64(ref instruction, Register.EAX, Register.EAX, TupleType.N1, false);
			}
			else
			{
				this.ReadOpMem16(ref instruction, TupleType.N1);
				flag = false;
			}
			if (this.invalidCheckMask != 0U && !flag)
			{
				this.SetInvalidInstruction();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ReadOpMem_MPX(ref Instruction instruction)
		{
			if (this.is64bMode)
			{
				this.state.addressSize = OpSize.Size64;
				this.ReadOpMem32Or64(ref instruction, Register.RAX, Register.RAX, TupleType.N1, false);
				return;
			}
			if (this.state.addressSize == OpSize.Size32)
			{
				this.ReadOpMem32Or64(ref instruction, Register.EAX, Register.EAX, TupleType.N1, false);
				return;
			}
			this.ReadOpMem16(ref instruction, TupleType.N1);
			if (this.invalidCheckMask != 0U)
			{
				this.SetInvalidInstruction();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ReadOpMem(ref Instruction instruction, TupleType tupleType)
		{
			if (this.state.addressSize == OpSize.Size64)
			{
				this.ReadOpMem32Or64(ref instruction, Register.RAX, Register.RAX, tupleType, false);
				return;
			}
			if (this.state.addressSize == OpSize.Size32)
			{
				this.ReadOpMem32Or64(ref instruction, Register.EAX, Register.EAX, tupleType, false);
				return;
			}
			this.ReadOpMem16(ref instruction, tupleType);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ReadOpMem_VSIB(ref Instruction instruction, Register vsibIndex, TupleType tupleType)
		{
			bool flag;
			if (this.state.addressSize == OpSize.Size64)
			{
				flag = this.ReadOpMem32Or64(ref instruction, Register.RAX, vsibIndex, tupleType, true);
			}
			else if (this.state.addressSize == OpSize.Size32)
			{
				flag = this.ReadOpMem32Or64(ref instruction, Register.EAX, vsibIndex, tupleType, true);
			}
			else
			{
				this.ReadOpMem16(ref instruction, tupleType);
				flag = false;
			}
			if (this.invalidCheckMask != 0U && !flag)
			{
				this.SetInvalidInstruction();
			}
		}

		private void ReadOpMem16(ref Instruction instruction, TupleType tupleType)
		{
			Decoder.RegInfo2 regInfo = this.memRegs16[(int)this.state.rm];
			Register register;
			Register register2;
			regInfo.Deconstruct(out register, out register2);
			Register register3 = register;
			Register register4 = register2;
			int mod = (int)this.state.mod;
			if (mod != 0)
			{
				if (mod != 1)
				{
					instruction.InternalSetMemoryDisplSize(2U);
					this.displIndex = this.state.zs.instructionLength;
					instruction.MemoryDisplacement64 = (ulong)this.ReadUInt16();
				}
				else
				{
					instruction.InternalSetMemoryDisplSize(1U);
					this.displIndex = this.state.zs.instructionLength;
					if (tupleType == TupleType.N1)
					{
						instruction.MemoryDisplacement64 = (ulong)((ushort)((sbyte)this.ReadByte()));
					}
					else
					{
						instruction.MemoryDisplacement64 = (ulong)((ushort)(this.GetDisp8N(tupleType) * (uint)((sbyte)this.ReadByte())));
					}
				}
			}
			else if (this.state.rm == 6U)
			{
				instruction.InternalSetMemoryDisplSize(2U);
				this.displIndex = this.state.zs.instructionLength;
				instruction.MemoryDisplacement64 = (ulong)this.ReadUInt16();
				register3 = Register.None;
			}
			instruction.InternalMemoryBase = register3;
			instruction.InternalMemoryIndex = register4;
		}

		private bool ReadOpMem32Or64(ref Instruction instruction, Register baseReg, Register indexReg, TupleType tupleType, bool isVsib)
		{
			int mod = (int)this.state.mod;
			uint num;
			uint num2;
			uint num3;
			if (mod != 0)
			{
				if (mod != 1)
				{
					if (this.state.rm != 4U)
					{
						this.displIndex = this.state.zs.instructionLength;
						if (this.state.addressSize == OpSize.Size64)
						{
							instruction.MemoryDisplacement64 = (ulong)((long)this.ReadUInt32());
							instruction.InternalSetMemoryDisplSize(4U);
						}
						else
						{
							instruction.MemoryDisplacement64 = (ulong)this.ReadUInt32();
							instruction.InternalSetMemoryDisplSize(3U);
						}
						instruction.InternalMemoryBase = (int)(this.state.zs.extraBaseRegisterBase + this.state.rm) + baseReg;
						return false;
					}
					num = this.ReadByte();
					num2 = ((this.state.addressSize == OpSize.Size64) ? 4U : 3U);
					this.displIndex = this.state.zs.instructionLength;
					num3 = this.ReadUInt32();
				}
				else
				{
					if (this.state.rm != 4U)
					{
						instruction.InternalSetMemoryDisplSize(1U);
						this.displIndex = this.state.zs.instructionLength;
						if (this.state.addressSize == OpSize.Size64)
						{
							if (tupleType == TupleType.N1)
							{
								instruction.MemoryDisplacement64 = (ulong)((long)((sbyte)this.ReadByte()));
							}
							else
							{
								instruction.MemoryDisplacement64 = (ulong)this.GetDisp8N(tupleType) * (ulong)((long)((sbyte)this.ReadByte()));
							}
						}
						else if (tupleType == TupleType.N1)
						{
							instruction.MemoryDisplacement64 = (ulong)((sbyte)this.ReadByte());
						}
						else
						{
							instruction.MemoryDisplacement64 = (ulong)(this.GetDisp8N(tupleType) * (uint)((sbyte)this.ReadByte()));
						}
						instruction.InternalMemoryBase = (int)(this.state.zs.extraBaseRegisterBase + this.state.rm) + baseReg;
						return false;
					}
					num = this.ReadByte();
					num2 = 1U;
					this.displIndex = this.state.zs.instructionLength;
					if (tupleType == TupleType.N1)
					{
						num3 = (uint)((sbyte)this.ReadByte());
					}
					else
					{
						num3 = this.GetDisp8N(tupleType) * (uint)((sbyte)this.ReadByte());
					}
				}
			}
			else if (this.state.rm == 4U)
			{
				num = this.ReadByte();
				num2 = 0U;
				num3 = 0U;
			}
			else
			{
				if (this.state.rm == 5U)
				{
					this.displIndex = this.state.zs.instructionLength;
					if (this.state.addressSize == OpSize.Size64)
					{
						instruction.MemoryDisplacement64 = (ulong)((long)this.ReadUInt32());
						instruction.InternalSetMemoryDisplSize(4U);
					}
					else
					{
						instruction.MemoryDisplacement64 = (ulong)this.ReadUInt32();
						instruction.InternalSetMemoryDisplSize(3U);
					}
					if (this.is64bMode)
					{
						if (this.state.addressSize == OpSize.Size64)
						{
							this.state.zs.flags = this.state.zs.flags | StateFlags.IpRel64;
							instruction.InternalMemoryBase = Register.RIP;
						}
						else
						{
							this.state.zs.flags = this.state.zs.flags | StateFlags.IpRel32;
							instruction.InternalMemoryBase = Register.EIP;
						}
					}
					return false;
				}
				instruction.InternalMemoryBase = (int)(this.state.zs.extraBaseRegisterBase + this.state.rm) + baseReg;
				return false;
			}
			uint num4 = ((num >> 3) & 7U) + this.state.zs.extraIndexRegisterBase;
			uint num5 = num & 7U;
			instruction.InternalMemoryIndexScale = (int)(num >> 6);
			if (!isVsib)
			{
				if (num4 != 4U)
				{
					instruction.InternalMemoryIndex = (int)num4 + indexReg;
				}
			}
			else
			{
				instruction.InternalMemoryIndex = (int)(num4 + this.state.zs.extraIndexRegisterBaseVSIB) + indexReg;
			}
			if (num5 == 5U && this.state.mod == 0U)
			{
				this.displIndex = this.state.zs.instructionLength;
				if (this.state.addressSize == OpSize.Size64)
				{
					instruction.MemoryDisplacement64 = (ulong)((long)this.ReadUInt32());
					instruction.InternalSetMemoryDisplSize(4U);
				}
				else
				{
					instruction.MemoryDisplacement64 = (ulong)this.ReadUInt32();
					instruction.InternalSetMemoryDisplSize(3U);
				}
			}
			else
			{
				instruction.InternalMemoryBase = (int)(num5 + this.state.zs.extraBaseRegisterBase) + baseReg;
				instruction.InternalSetMemoryDisplSize(num2);
				if (this.state.addressSize == OpSize.Size64)
				{
					instruction.MemoryDisplacement64 = (ulong)((long)num3);
				}
				else
				{
					instruction.MemoryDisplacement64 = (ulong)num3;
				}
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private uint GetDisp8N(TupleType tupleType)
		{
			return TupleTypeTable.GetDisp8N(tupleType, (this.state.zs.flags & StateFlags.b) > (StateFlags)0U);
		}

		public ConstantOffsets GetConstantOffsets(in Instruction instruction)
		{
			ConstantOffsets constantOffsets = default(ConstantOffsets);
			int memoryDisplSize = instruction.MemoryDisplSize;
			if (memoryDisplSize != 0)
			{
				constantOffsets.DisplacementOffset = (byte)this.displIndex;
				if (memoryDisplSize == 8 && (this.state.zs.flags & StateFlags.Addr64) == (StateFlags)0U)
				{
					constantOffsets.DisplacementSize = 4;
				}
				else
				{
					constantOffsets.DisplacementSize = (byte)memoryDisplSize;
				}
			}
			if ((this.state.zs.flags & StateFlags.NoImm) == (StateFlags)0U)
			{
				int num = 0;
				for (int i = instruction.OpCount - 1; i >= 0; i--)
				{
					switch (instruction.GetOpKind(i))
					{
					case OpKind.NearBranch16:
						if ((this.state.zs.flags & StateFlags.BranchImm8) != (StateFlags)0U)
						{
							constantOffsets.ImmediateOffset = (byte)(instruction.Length - 1);
							constantOffsets.ImmediateSize = 1;
						}
						else if ((this.state.zs.flags & StateFlags.Xbegin) == (StateFlags)0U)
						{
							constantOffsets.ImmediateOffset = (byte)(instruction.Length - 2);
							constantOffsets.ImmediateSize = 2;
						}
						else if (this.state.operandSize != OpSize.Size16)
						{
							constantOffsets.ImmediateOffset = (byte)(instruction.Length - 4);
							constantOffsets.ImmediateSize = 4;
						}
						else
						{
							constantOffsets.ImmediateOffset = (byte)(instruction.Length - 2);
							constantOffsets.ImmediateSize = 2;
						}
						break;
					case OpKind.NearBranch32:
					case OpKind.NearBranch64:
						if ((this.state.zs.flags & StateFlags.BranchImm8) != (StateFlags)0U)
						{
							constantOffsets.ImmediateOffset = (byte)(instruction.Length - 1);
							constantOffsets.ImmediateSize = 1;
						}
						else if ((this.state.zs.flags & StateFlags.Xbegin) == (StateFlags)0U)
						{
							constantOffsets.ImmediateOffset = (byte)(instruction.Length - 4);
							constantOffsets.ImmediateSize = 4;
						}
						else if (this.state.operandSize != OpSize.Size16)
						{
							constantOffsets.ImmediateOffset = (byte)(instruction.Length - 4);
							constantOffsets.ImmediateSize = 4;
						}
						else
						{
							constantOffsets.ImmediateOffset = (byte)(instruction.Length - 2);
							constantOffsets.ImmediateSize = 2;
						}
						break;
					case OpKind.FarBranch16:
						constantOffsets.ImmediateOffset = (byte)(instruction.Length - 4);
						constantOffsets.ImmediateSize = 2;
						constantOffsets.ImmediateOffset2 = (byte)(instruction.Length - 2);
						constantOffsets.ImmediateSize2 = 2;
						break;
					case OpKind.FarBranch32:
						constantOffsets.ImmediateOffset = (byte)(instruction.Length - 6);
						constantOffsets.ImmediateSize = 4;
						constantOffsets.ImmediateOffset2 = (byte)(instruction.Length - 2);
						constantOffsets.ImmediateSize2 = 2;
						break;
					case OpKind.Immediate8:
					case OpKind.Immediate8to16:
					case OpKind.Immediate8to32:
					case OpKind.Immediate8to64:
						constantOffsets.ImmediateOffset = (byte)(instruction.Length - num - 1);
						constantOffsets.ImmediateSize = 1;
						return constantOffsets;
					case OpKind.Immediate8_2nd:
						constantOffsets.ImmediateOffset2 = (byte)(instruction.Length - 1);
						constantOffsets.ImmediateSize2 = 1;
						num = 1;
						break;
					case OpKind.Immediate16:
						constantOffsets.ImmediateOffset = (byte)(instruction.Length - num - 2);
						constantOffsets.ImmediateSize = 2;
						return constantOffsets;
					case OpKind.Immediate32:
					case OpKind.Immediate32to64:
						constantOffsets.ImmediateOffset = (byte)(instruction.Length - num - 4);
						constantOffsets.ImmediateSize = 4;
						return constantOffsets;
					case OpKind.Immediate64:
						constantOffsets.ImmediateOffset = (byte)(instruction.Length - num - 8);
						constantOffsets.ImmediateSize = 8;
						return constantOffsets;
					}
				}
			}
			return constantOffsets;
		}

		public Decoder.Enumerator GetEnumerator()
		{
			return new Decoder.Enumerator(this);
		}

		IEnumerator<Instruction> IEnumerable<Instruction>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private ulong instructionPointer;

		private readonly CodeReader reader;

		private readonly Decoder.RegInfo2[] memRegs16;

		private readonly OpCodeHandler[] handlers_MAP0;

		private readonly OpCodeHandler[] handlers_VEX_0F;

		private readonly OpCodeHandler[] handlers_VEX_0F38;

		private readonly OpCodeHandler[] handlers_VEX_0F3A;

		private readonly OpCodeHandler[] handlers_EVEX_0F;

		private readonly OpCodeHandler[] handlers_EVEX_0F38;

		private readonly OpCodeHandler[] handlers_EVEX_0F3A;

		private readonly OpCodeHandler[] handlers_EVEX_MAP5;

		private readonly OpCodeHandler[] handlers_EVEX_MAP6;

		private readonly OpCodeHandler[] handlers_XOP_MAP8;

		private readonly OpCodeHandler[] handlers_XOP_MAP9;

		private readonly OpCodeHandler[] handlers_XOP_MAP10;

		internal Decoder.State state;

		internal uint displIndex;

		internal readonly DecoderOptions options;

		internal readonly uint invalidCheckMask;

		internal readonly uint is64bMode_and_W;

		internal readonly uint reg15Mask;

		private readonly uint maskE0;

		private readonly uint rexMask;

		internal readonly CodeSize defaultCodeSize;

		internal readonly OpSize defaultOperandSize;

		private readonly OpSize defaultAddressSize;

		internal readonly OpSize defaultInvertedOperandSize;

		internal readonly OpSize defaultInvertedAddressSize;

		internal readonly bool is64bMode;

		private static readonly Decoder.RegInfo2[] s_memRegs16 = new Decoder.RegInfo2[]
		{
			new Decoder.RegInfo2(Register.BX, Register.SI),
			new Decoder.RegInfo2(Register.BX, Register.DI),
			new Decoder.RegInfo2(Register.BP, Register.SI),
			new Decoder.RegInfo2(Register.BP, Register.DI),
			new Decoder.RegInfo2(Register.SI, Register.None),
			new Decoder.RegInfo2(Register.DI, Register.None),
			new Decoder.RegInfo2(Register.BP, Register.None),
			new Decoder.RegInfo2(Register.BX, Register.None)
		};

		internal struct ZState
		{
			public uint instructionLength;

			public uint extraRegisterBase;

			public uint extraIndexRegisterBase;

			public uint extraBaseRegisterBase;

			public uint extraIndexRegisterBaseVSIB;

			public StateFlags flags;

			public MandatoryPrefixByte mandatoryPrefix;

			public byte segmentPrio;
		}

		internal struct State
		{
			public readonly EncodingKind Encoding
			{
				get
				{
					return (EncodingKind)((this.zs.flags >> 29) & StateFlags.MvexSssMask);
				}
			}

			public uint modrm;

			public uint mod;

			public uint reg;

			public uint rm;

			public Decoder.ZState zs;

			public uint vvvv;

			public uint vvvv_invalidCheck;

			public uint aaa;

			public uint extraRegisterBaseEVEX;

			public uint extraBaseRegisterBaseEVEX;

			public uint vectorLength;

			public OpSize operandSize;

			public OpSize addressSize;
		}

		private readonly struct RegInfo2
		{
			public RegInfo2(Register baseReg, Register indexReg)
			{
				this.baseReg = baseReg;
				this.indexReg = indexReg;
			}

			public void Deconstruct(out Register baseReg, out Register indexReg)
			{
				baseReg = this.baseReg;
				indexReg = this.indexReg;
			}

			public readonly Register baseReg;

			public readonly Register indexReg;
		}

		public struct Enumerator : IEnumerator<Instruction>, IDisposable, IEnumerator
		{
			[NullableContext(1)]
			internal Enumerator(Decoder decoder)
			{
				this.decoder = decoder;
				this.instruction = default(Instruction);
			}

			public Instruction Current
			{
				get
				{
					return this.instruction;
				}
			}

			Instruction IEnumerator<Instruction>.Current
			{
				get
				{
					return this.Current;
				}
			}

			[Nullable(1)]
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				this.decoder.Decode(out this.instruction);
				return this.instruction.Length != 0;
			}

			void IEnumerator.Reset()
			{
				throw new InvalidOperationException();
			}

			public void Dispose()
			{
			}

			private readonly Decoder decoder;

			private Instruction instruction;
		}
	}
}
