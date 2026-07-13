using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_MandatoryPrefix3 : OpCodeHandlerModRM
	{
		public OpCodeHandler_MandatoryPrefix3(OpCodeHandler handler_reg, OpCodeHandler handler_mem, OpCodeHandler handler66_reg, OpCodeHandler handler66_mem, OpCodeHandler handlerF3_reg, OpCodeHandler handlerF3_mem, OpCodeHandler handlerF2_reg, OpCodeHandler handlerF2_mem, LegacyHandlerFlags flags)
		{
			OpCodeHandler_MandatoryPrefix3.Info[] array = new OpCodeHandler_MandatoryPrefix3.Info[4];
			int num = 0;
			if (handler_reg == null)
			{
				throw new ArgumentNullException("handler_reg");
			}
			array[num] = new OpCodeHandler_MandatoryPrefix3.Info(handler_reg, (flags & LegacyHandlerFlags.HandlerReg) == (LegacyHandlerFlags)0U);
			int num2 = 1;
			if (handler66_reg == null)
			{
				throw new ArgumentNullException("handler66_reg");
			}
			array[num2] = new OpCodeHandler_MandatoryPrefix3.Info(handler66_reg, (flags & LegacyHandlerFlags.Handler66Reg) == (LegacyHandlerFlags)0U);
			int num3 = 2;
			if (handlerF3_reg == null)
			{
				throw new ArgumentNullException("handlerF3_reg");
			}
			array[num3] = new OpCodeHandler_MandatoryPrefix3.Info(handlerF3_reg, (flags & LegacyHandlerFlags.HandlerF3Reg) == (LegacyHandlerFlags)0U);
			int num4 = 3;
			if (handlerF2_reg == null)
			{
				throw new ArgumentNullException("handlerF2_reg");
			}
			array[num4] = new OpCodeHandler_MandatoryPrefix3.Info(handlerF2_reg, (flags & LegacyHandlerFlags.HandlerF2Reg) == (LegacyHandlerFlags)0U);
			this.handlers_reg = array;
			OpCodeHandler_MandatoryPrefix3.Info[] array2 = new OpCodeHandler_MandatoryPrefix3.Info[4];
			int num5 = 0;
			if (handler_mem == null)
			{
				throw new ArgumentNullException("handler_mem");
			}
			array2[num5] = new OpCodeHandler_MandatoryPrefix3.Info(handler_mem, (flags & LegacyHandlerFlags.HandlerMem) == (LegacyHandlerFlags)0U);
			int num6 = 1;
			if (handler66_mem == null)
			{
				throw new ArgumentNullException("handler66_mem");
			}
			array2[num6] = new OpCodeHandler_MandatoryPrefix3.Info(handler66_mem, (flags & LegacyHandlerFlags.Handler66Mem) == (LegacyHandlerFlags)0U);
			int num7 = 2;
			if (handlerF3_mem == null)
			{
				throw new ArgumentNullException("handlerF3_mem");
			}
			array2[num7] = new OpCodeHandler_MandatoryPrefix3.Info(handlerF3_mem, (flags & LegacyHandlerFlags.HandlerF3Mem) == (LegacyHandlerFlags)0U);
			int num8 = 3;
			if (handlerF2_mem == null)
			{
				throw new ArgumentNullException("handlerF2_mem");
			}
			array2[num8] = new OpCodeHandler_MandatoryPrefix3.Info(handlerF2_mem, (flags & LegacyHandlerFlags.HandlerF2Mem) == (LegacyHandlerFlags)0U);
			this.handlers_mem = array2;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			OpCodeHandler_MandatoryPrefix3.Info info = ((decoder.state.mod == 3U) ? this.handlers_reg : this.handlers_mem)[(int)decoder.state.zs.mandatoryPrefix];
			if (info.mandatoryPrefix)
			{
				decoder.ClearMandatoryPrefix(ref instruction);
			}
			info.handler.Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler_MandatoryPrefix3.Info[] handlers_reg;

		private readonly OpCodeHandler_MandatoryPrefix3.Info[] handlers_mem;

		private readonly struct Info
		{
			public Info(OpCodeHandler handler, bool mandatoryPrefix)
			{
				this.handler = handler;
				this.mandatoryPrefix = mandatoryPrefix;
			}

			public readonly OpCodeHandler handler;

			public readonly bool mandatoryPrefix;
		}
	}
}
