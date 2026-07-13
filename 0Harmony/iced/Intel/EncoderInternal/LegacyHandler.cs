using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class LegacyHandler : OpCodeHandler
	{
		private static Op[] CreateOps(EncFlags1 encFlags1)
		{
			int num = (int)(encFlags1 & EncFlags1.Legacy_OpMask);
			int num2 = (int)((encFlags1 >> 7) & EncFlags1.Legacy_OpMask);
			int num3 = (int)((encFlags1 >> 14) & EncFlags1.Legacy_OpMask);
			int num4 = (int)((encFlags1 >> 21) & EncFlags1.Legacy_OpMask);
			if (num4 != 0)
			{
				return new Op[]
				{
					OpHandlerData.LegacyOps[num - 1],
					OpHandlerData.LegacyOps[num2 - 1],
					OpHandlerData.LegacyOps[num3 - 1],
					OpHandlerData.LegacyOps[num4 - 1]
				};
			}
			if (num3 != 0)
			{
				return new Op[]
				{
					OpHandlerData.LegacyOps[num - 1],
					OpHandlerData.LegacyOps[num2 - 1],
					OpHandlerData.LegacyOps[num3 - 1]
				};
			}
			if (num2 != 0)
			{
				return new Op[]
				{
					OpHandlerData.LegacyOps[num - 1],
					OpHandlerData.LegacyOps[num2 - 1]
				};
			}
			if (num != 0)
			{
				return new Op[] { OpHandlerData.LegacyOps[num - 1] };
			}
			return Array2.Empty<Op>();
		}

		public LegacyHandler(EncFlags1 encFlags1, EncFlags2 encFlags2, EncFlags3 encFlags3)
			: base(encFlags2, encFlags3, false, null, LegacyHandler.CreateOps(encFlags1))
		{
			switch ((encFlags2 >> 17) & EncFlags2.TableMask)
			{
			case EncFlags2.None:
				this.tableByte1 = 0U;
				this.tableByte2 = 0U;
				break;
			case (EncFlags2)1U:
				this.tableByte1 = 15U;
				this.tableByte2 = 0U;
				break;
			case (EncFlags2)2U:
				this.tableByte1 = 15U;
				this.tableByte2 = 56U;
				break;
			case EncFlags2.MandatoryPrefixMask:
				this.tableByte1 = 15U;
				this.tableByte2 = 58U;
				break;
			default:
				throw new InvalidOperationException();
			}
			uint num;
			switch ((encFlags2 >> 20) & EncFlags2.MandatoryPrefixMask)
			{
			case EncFlags2.None:
				num = 0U;
				break;
			case (EncFlags2)1U:
				num = 102U;
				break;
			case (EncFlags2)2U:
				num = 243U;
				break;
			case EncFlags2.MandatoryPrefixMask:
				num = 242U;
				break;
			default:
				throw new InvalidOperationException();
			}
			this.mandatoryPrefix = num;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction)
		{
			uint num = this.mandatoryPrefix;
			encoder.WritePrefixes(in instruction, num != 243U);
			if (num != 0U)
			{
				encoder.WriteByteInternal(num);
			}
			num = (uint)encoder.EncoderFlags;
			num &= 79U;
			if (num != 0U)
			{
				if ((encoder.EncoderFlags & EncoderFlags.HighLegacy8BitRegs) != EncoderFlags.None)
				{
					encoder.ErrorMessage = "Registers AH, CH, DH, BH can't be used if there's a REX prefix. Use AL, CL, DL, BL, SPL, BPL, SIL, DIL, R8L-R15L instead.";
				}
				num |= 64U;
				encoder.WriteByteInternal(num);
			}
			if ((num = this.tableByte1) != 0U)
			{
				encoder.WriteByteInternal(num);
				if ((num = this.tableByte2) != 0U)
				{
					encoder.WriteByteInternal(num);
				}
			}
		}

		private readonly uint tableByte1;

		private readonly uint tableByte2;

		private readonly uint mandatoryPrefix;
	}
}
