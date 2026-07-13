using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class VexHandler : OpCodeHandler
	{
		private static Op[] CreateOps(EncFlags1 encFlags1)
		{
			int num = (int)(encFlags1 & EncFlags1.VEX_OpMask);
			int num2 = (int)((encFlags1 >> 6) & EncFlags1.VEX_OpMask);
			int num3 = (int)((encFlags1 >> 12) & EncFlags1.VEX_OpMask);
			int num4 = (int)((encFlags1 >> 18) & EncFlags1.VEX_OpMask);
			int num5 = (int)((encFlags1 >> 24) & EncFlags1.VEX_OpMask);
			if (num5 != 0)
			{
				return new Op[]
				{
					OpHandlerData.VexOps[num - 1],
					OpHandlerData.VexOps[num2 - 1],
					OpHandlerData.VexOps[num3 - 1],
					OpHandlerData.VexOps[num4 - 1],
					OpHandlerData.VexOps[num5 - 1]
				};
			}
			if (num4 != 0)
			{
				return new Op[]
				{
					OpHandlerData.VexOps[num - 1],
					OpHandlerData.VexOps[num2 - 1],
					OpHandlerData.VexOps[num3 - 1],
					OpHandlerData.VexOps[num4 - 1]
				};
			}
			if (num3 != 0)
			{
				return new Op[]
				{
					OpHandlerData.VexOps[num - 1],
					OpHandlerData.VexOps[num2 - 1],
					OpHandlerData.VexOps[num3 - 1]
				};
			}
			if (num2 != 0)
			{
				return new Op[]
				{
					OpHandlerData.VexOps[num - 1],
					OpHandlerData.VexOps[num2 - 1]
				};
			}
			if (num != 0)
			{
				return new Op[] { OpHandlerData.VexOps[num - 1] };
			}
			return Array2.Empty<Op>();
		}

		public VexHandler(EncFlags1 encFlags1, EncFlags2 encFlags2, EncFlags3 encFlags3)
			: base(encFlags2, encFlags3, false, null, VexHandler.CreateOps(encFlags1))
		{
			this.table = (uint)((encFlags2 >> 17) & EncFlags2.TableMask);
			WBit wbit = (WBit)((encFlags2 >> 22) & EncFlags2.MandatoryPrefixMask);
			this.W1 = ((wbit == WBit.W1) ? uint.MaxValue : 0U);
			LBit lbit = (LBit)((encFlags2 >> 24) & EncFlags2.TableMask);
			if (lbit == LBit.L1 || lbit == LBit.L256)
			{
				this.lastByte = 4U;
			}
			if (this.W1 != 0U)
			{
				this.lastByte |= 128U;
			}
			this.lastByte |= (uint)((encFlags2 >> 20) & EncFlags2.MandatoryPrefixMask);
			if (wbit == WBit.WIG)
			{
				this.mask_W_L |= 128U;
			}
			if (lbit == LBit.LIG)
			{
				this.mask_W_L |= 4U;
				this.mask_L |= 4U;
			}
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction)
		{
			encoder.WritePrefixes(in instruction, true);
			uint encoderFlags = (uint)encoder.EncoderFlags;
			uint num = this.lastByte;
			num |= (~encoderFlags >> 24) & 120U;
			if ((encoder.Internal_PreventVEX2 | this.W1 | (this.table - 1U) | (encoderFlags & 11U)) != 0U)
			{
				encoder.WriteByteInternal(196U);
				uint num2 = this.table;
				num2 |= (~encoderFlags & 7U) << 5;
				encoder.WriteByteInternal(num2);
				num |= this.mask_W_L & encoder.Internal_VEX_WIG_LIG;
				encoder.WriteByteInternal(num);
				return;
			}
			encoder.WriteByteInternal(197U);
			num |= (~encoderFlags & 4U) << 5;
			num |= this.mask_L & encoder.Internal_VEX_LIG;
			encoder.WriteByteInternal(num);
		}

		private readonly uint table;

		private readonly uint lastByte;

		private readonly uint mask_W_L;

		private readonly uint mask_L;

		private readonly uint W1;
	}
}
