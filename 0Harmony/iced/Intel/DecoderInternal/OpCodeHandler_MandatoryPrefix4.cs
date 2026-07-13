using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_MandatoryPrefix4 : OpCodeHandler
	{
		public OpCodeHandler_MandatoryPrefix4(OpCodeHandler handlerNP, OpCodeHandler handler66, OpCodeHandler handlerF3, OpCodeHandler handlerF2, uint flags)
		{
			if (handlerNP == null)
			{
				throw new ArgumentNullException("handlerNP");
			}
			this.handlerNP = handlerNP;
			if (handler66 == null)
			{
				throw new ArgumentNullException("handler66");
			}
			this.handler66 = handler66;
			if (handlerF3 == null)
			{
				throw new ArgumentNullException("handlerF3");
			}
			this.handlerF3 = handlerF3;
			if (handlerF2 == null)
			{
				throw new ArgumentNullException("handlerF2");
			}
			this.handlerF2 = handlerF2;
			this.flags = flags;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			OpCodeHandler opCodeHandler;
			switch (decoder.state.zs.mandatoryPrefix)
			{
			case MandatoryPrefixByte.None:
				opCodeHandler = this.handlerNP;
				break;
			case MandatoryPrefixByte.P66:
				opCodeHandler = this.handler66;
				break;
			case MandatoryPrefixByte.PF3:
				if ((this.flags & 4U) != 0U)
				{
					decoder.ClearMandatoryPrefixF3(ref instruction);
				}
				opCodeHandler = this.handlerF3;
				break;
			case MandatoryPrefixByte.PF2:
				if ((this.flags & 8U) != 0U)
				{
					decoder.ClearMandatoryPrefixF2(ref instruction);
				}
				opCodeHandler = this.handlerF2;
				break;
			default:
				throw new InvalidOperationException();
			}
			if (opCodeHandler.HasModRM && (this.flags & 16U) != 0U)
			{
				decoder.ReadModRM();
			}
			opCodeHandler.Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler handlerNP;

		private readonly OpCodeHandler handler66;

		private readonly OpCodeHandler handlerF3;

		private readonly OpCodeHandler handlerF2;

		private readonly uint flags;
	}
}
