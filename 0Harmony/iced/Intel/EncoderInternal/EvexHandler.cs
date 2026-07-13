using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class EvexHandler : OpCodeHandler
	{
		private static Op[] CreateOps(EncFlags1 encFlags1)
		{
			int num = (int)(encFlags1 & EncFlags1.XOP_OpMask);
			int num2 = (int)((encFlags1 >> 5) & EncFlags1.XOP_OpMask);
			int num3 = (int)((encFlags1 >> 10) & EncFlags1.XOP_OpMask);
			int num4 = (int)((encFlags1 >> 15) & EncFlags1.XOP_OpMask);
			if (num4 != 0)
			{
				return new Op[]
				{
					OpHandlerData.EvexOps[num - 1],
					OpHandlerData.EvexOps[num2 - 1],
					OpHandlerData.EvexOps[num3 - 1],
					OpHandlerData.EvexOps[num4 - 1]
				};
			}
			if (num3 != 0)
			{
				return new Op[]
				{
					OpHandlerData.EvexOps[num - 1],
					OpHandlerData.EvexOps[num2 - 1],
					OpHandlerData.EvexOps[num3 - 1]
				};
			}
			if (num2 != 0)
			{
				return new Op[]
				{
					OpHandlerData.EvexOps[num - 1],
					OpHandlerData.EvexOps[num2 - 1]
				};
			}
			if (num != 0)
			{
				return new Op[] { OpHandlerData.EvexOps[num - 1] };
			}
			return Array2.Empty<Op>();
		}

		public EvexHandler(EncFlags1 encFlags1, EncFlags2 encFlags2, EncFlags3 encFlags3)
			: base(encFlags2, encFlags3, false, EvexHandler.tryConvertToDisp8N, EvexHandler.CreateOps(encFlags1))
		{
			this.tupleType = (TupleType)((encFlags3 >> 7) & EncFlags3.TupleTypeMask);
			this.table = (uint)((encFlags2 >> 17) & EncFlags2.TableMask);
			this.p1Bits = (uint)((EncFlags2)4U | ((encFlags2 >> 20) & EncFlags2.MandatoryPrefixMask));
			this.wbit = (WBit)((encFlags2 >> 22) & EncFlags2.MandatoryPrefixMask);
			if (this.wbit == WBit.W1)
			{
				this.p1Bits |= 128U;
			}
			switch ((encFlags2 >> 24) & EncFlags2.TableMask)
			{
			case EncFlags2.None:
			case EncFlags2.MandatoryPrefixMask:
			case (EncFlags2)4U:
				this.llBits = 0U;
				break;
			case (EncFlags2)1U:
			case (EncFlags2)5U:
				this.llBits = 32U;
				break;
			case (EncFlags2)2U:
				this.llBits = 0U;
				this.mask_LL = 96U;
				break;
			case (EncFlags2)6U:
				this.llBits = 64U;
				break;
			default:
				throw new InvalidOperationException();
			}
			if (this.wbit == WBit.WIG)
			{
				this.mask_W |= 128U;
			}
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction)
		{
			encoder.WritePrefixes(in instruction, true);
			uint encoderFlags = (uint)encoder.EncoderFlags;
			encoder.WriteByteInternal(98U);
			uint num = this.table;
			num |= (encoderFlags & 7U) << 5;
			num |= (encoderFlags >> 5) & 16U;
			num ^= 4294967280U;
			encoder.WriteByteInternal(num);
			num = this.p1Bits;
			num |= (~encoderFlags >> 24) & 120U;
			num |= this.mask_W & encoder.Internal_EVEX_WIG;
			encoder.WriteByteInternal(num);
			num = instruction.InternalOpMask;
			if (num != 0U)
			{
				if ((this.EncFlags3 & EncFlags3.OpMaskRegister) == EncFlags3.None)
				{
					encoder.ErrorMessage = "The instruction doesn't support opmask registers";
				}
			}
			else if ((this.EncFlags3 & (EncFlags3)2147483648U) != EncFlags3.None)
			{
				encoder.ErrorMessage = "The instruction must use an opmask register";
			}
			num |= (encoderFlags >> 28) & 8U;
			if (instruction.SuppressAllExceptions)
			{
				if ((this.EncFlags3 & EncFlags3.SuppressAllExceptions) == EncFlags3.None)
				{
					encoder.ErrorMessage = "The instruction doesn't support suppress-all-exceptions";
				}
				num |= 16U;
			}
			RoundingControl roundingControl = instruction.RoundingControl;
			if (roundingControl != RoundingControl.None)
			{
				if ((this.EncFlags3 & EncFlags3.RoundingControl) == EncFlags3.None)
				{
					encoder.ErrorMessage = "The instruction doesn't support rounding control";
				}
				num |= 16U;
				num |= (uint)((uint)(roundingControl - RoundingControl.RoundToNearest) << 5);
			}
			else if ((this.EncFlags3 & EncFlags3.SuppressAllExceptions) == EncFlags3.None || !instruction.SuppressAllExceptions)
			{
				num |= this.llBits;
			}
			if ((encoderFlags & 1024U) != 0U)
			{
				num |= 16U;
			}
			else if (instruction.IsBroadcast)
			{
				encoder.ErrorMessage = "The instruction doesn't support broadcasting";
			}
			if (instruction.ZeroingMasking)
			{
				if ((this.EncFlags3 & EncFlags3.ZeroingMasking) == EncFlags3.None)
				{
					encoder.ErrorMessage = "The instruction doesn't support zeroing masking";
				}
				num |= 128U;
			}
			num ^= 8U;
			num |= this.mask_LL & encoder.Internal_EVEX_LIG;
			encoder.WriteByteInternal(num);
		}

		private readonly WBit wbit;

		private readonly TupleType tupleType;

		private readonly uint table;

		private readonly uint p1Bits;

		private readonly uint llBits;

		private readonly uint mask_W;

		private readonly uint mask_LL;

		private static readonly TryConvertToDisp8N tryConvertToDisp8N = new TryConvertToDisp8N(EvexHandler.TryConvertToDisp8NImpl.TryConvertToDisp8N);

		private sealed class TryConvertToDisp8NImpl
		{
			public static bool TryConvertToDisp8N(Encoder encoder, OpCodeHandler handler, in Instruction instruction, int displ, out sbyte compressedValue)
			{
				int disp8N = (int)TupleTypeTable.GetDisp8N(((EvexHandler)handler).tupleType, (encoder.EncoderFlags & EncoderFlags.Broadcast) > EncoderFlags.None);
				int num = displ / disp8N;
				if (num * disp8N == displ && -128 <= num && num <= 127)
				{
					compressedValue = (sbyte)num;
					return true;
				}
				compressedValue = 0;
				return false;
			}
		}
	}
}
