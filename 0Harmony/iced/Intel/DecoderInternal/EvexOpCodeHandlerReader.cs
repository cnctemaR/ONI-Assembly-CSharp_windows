using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class EvexOpCodeHandlerReader : OpCodeHandlerReader
	{
		public override int ReadHandlers(ref TableDeserializer deserializer, [Nullable(new byte[] { 1, 2 })] OpCodeHandler[] result, int resultIndex)
		{
			ref OpCodeHandler ptr = ref result[resultIndex];
			switch (deserializer.ReadEvexOpCodeHandlerKind())
			{
			case EvexOpCodeHandlerKind.Invalid:
				ptr = OpCodeHandler_Invalid.Instance;
				return 1;
			case EvexOpCodeHandlerKind.Invalid2:
				result[resultIndex] = OpCodeHandler_Invalid.Instance;
				result[resultIndex + 1] = OpCodeHandler_Invalid.Instance;
				return 2;
			case EvexOpCodeHandlerKind.Dup:
			{
				int num = deserializer.ReadInt32();
				OpCodeHandler opCodeHandler = deserializer.ReadHandler();
				for (int i = 0; i < num; i++)
				{
					result[resultIndex + i] = opCodeHandler;
				}
				return num;
			}
			case EvexOpCodeHandlerKind.HandlerReference:
				ptr = deserializer.ReadHandlerReference();
				return 1;
			case EvexOpCodeHandlerKind.ArrayReference:
				throw new InvalidOperationException();
			case EvexOpCodeHandlerKind.RM:
				ptr = new OpCodeHandler_RM(deserializer.ReadHandler(), deserializer.ReadHandler());
				return 1;
			case EvexOpCodeHandlerKind.Group:
				ptr = new OpCodeHandler_Group(deserializer.ReadArrayReference(4U));
				return 1;
			case EvexOpCodeHandlerKind.W:
				ptr = new OpCodeHandler_W(deserializer.ReadHandler(), deserializer.ReadHandler());
				return 1;
			case EvexOpCodeHandlerKind.MandatoryPrefix2:
				ptr = new OpCodeHandler_MandatoryPrefix2(deserializer.ReadHandler(), deserializer.ReadHandler(), deserializer.ReadHandler(), deserializer.ReadHandler());
				return 1;
			case EvexOpCodeHandlerKind.VectorLength:
				ptr = new OpCodeHandler_VectorLength_EVEX(deserializer.ReadHandler(), deserializer.ReadHandler(), deserializer.ReadHandler());
				return 1;
			case EvexOpCodeHandlerKind.VectorLength_er:
				ptr = new OpCodeHandler_VectorLength_EVEX_er(deserializer.ReadHandler(), deserializer.ReadHandler(), deserializer.ReadHandler());
				return 1;
			case EvexOpCodeHandlerKind.Ed_V_Ib:
			{
				Code code;
				ptr = new OpCodeHandler_EVEX_Ed_V_Ib(deserializer.ReadRegister(), code = deserializer.ReadCode(), code + 1, deserializer.ReadTupleType(), deserializer.ReadTupleType());
				return 1;
			}
			case EvexOpCodeHandlerKind.Ev_VX:
			{
				Code code = deserializer.ReadCode();
				ptr = new OpCodeHandler_EVEX_Ev_VX(code, code + 1, deserializer.ReadTupleType(), deserializer.ReadTupleType());
				return 1;
			}
			case EvexOpCodeHandlerKind.Ev_VX_Ib:
			{
				Code code;
				ptr = new OpCodeHandler_EVEX_Ev_VX_Ib(deserializer.ReadRegister(), code = deserializer.ReadCode(), code + 1);
				return 1;
			}
			case EvexOpCodeHandlerKind.Gv_W_er:
			{
				Code code;
				ptr = new OpCodeHandler_EVEX_Gv_W_er(deserializer.ReadRegister(), code = deserializer.ReadCode(), code + 1, deserializer.ReadTupleType(), deserializer.ReadBoolean());
				return 1;
			}
			case EvexOpCodeHandlerKind.GvM_VX_Ib:
			{
				Code code;
				ptr = new OpCodeHandler_EVEX_GvM_VX_Ib(deserializer.ReadRegister(), code = deserializer.ReadCode(), code + 1, deserializer.ReadTupleType(), deserializer.ReadTupleType());
				return 1;
			}
			case EvexOpCodeHandlerKind.HkWIb_3:
				ptr = new OpCodeHandler_EVEX_HkWIb(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.HkWIb_3b:
				ptr = new OpCodeHandler_EVEX_HkWIb(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), true);
				return 1;
			case EvexOpCodeHandlerKind.HWIb:
				ptr = new OpCodeHandler_EVEX_HWIb(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.KkHW_3:
				ptr = new OpCodeHandler_EVEX_KkHW(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.KkHW_3b:
				ptr = new OpCodeHandler_EVEX_KkHW(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), true);
				return 1;
			case EvexOpCodeHandlerKind.KkHWIb_sae_3:
				ptr = new OpCodeHandler_EVEX_KkHWIb_sae(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.KkHWIb_sae_3b:
				ptr = new OpCodeHandler_EVEX_KkHWIb_sae(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), true);
				return 1;
			case EvexOpCodeHandlerKind.KkHWIb_3:
				ptr = new OpCodeHandler_EVEX_KkHWIb(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.KkHWIb_3b:
				ptr = new OpCodeHandler_EVEX_KkHWIb(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), true);
				return 1;
			case EvexOpCodeHandlerKind.KkWIb_3:
				ptr = new OpCodeHandler_EVEX_KkWIb(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.KkWIb_3b:
				ptr = new OpCodeHandler_EVEX_KkWIb(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), true);
				return 1;
			case EvexOpCodeHandlerKind.KP1HW:
				ptr = new OpCodeHandler_EVEX_KP1HW(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.KR:
				ptr = new OpCodeHandler_EVEX_KR(deserializer.ReadRegister(), deserializer.ReadCode());
				return 1;
			case EvexOpCodeHandlerKind.MV:
				ptr = new OpCodeHandler_EVEX_MV(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.V_H_Ev_er:
			{
				Code code;
				ptr = new OpCodeHandler_EVEX_V_H_Ev_er(deserializer.ReadRegister(), code = deserializer.ReadCode(), code + 1, deserializer.ReadTupleType(), deserializer.ReadTupleType());
				return 1;
			}
			case EvexOpCodeHandlerKind.V_H_Ev_Ib:
			{
				Code code;
				ptr = new OpCodeHandler_EVEX_V_H_Ev_Ib(deserializer.ReadRegister(), code = deserializer.ReadCode(), code + 1, deserializer.ReadTupleType(), deserializer.ReadTupleType());
				return 1;
			}
			case EvexOpCodeHandlerKind.VHM:
				ptr = new OpCodeHandler_EVEX_VHM(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VHW_3:
				ptr = new OpCodeHandler_EVEX_VHW(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VHW_4:
				ptr = new OpCodeHandler_EVEX_VHW(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VHWIb:
				ptr = new OpCodeHandler_EVEX_VHWIb(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VK:
				ptr = new OpCodeHandler_EVEX_VK(deserializer.ReadRegister(), deserializer.ReadCode());
				return 1;
			case EvexOpCodeHandlerKind.Vk_VSIB:
				ptr = new OpCodeHandler_EVEX_Vk_VSIB(deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VkEv_REXW_2:
				ptr = new OpCodeHandler_EVEX_VkEv_REXW(deserializer.ReadRegister(), deserializer.ReadCode());
				return 1;
			case EvexOpCodeHandlerKind.VkEv_REXW_3:
				ptr = new OpCodeHandler_EVEX_VkEv_REXW(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadCode());
				return 1;
			case EvexOpCodeHandlerKind.VkHM:
				ptr = new OpCodeHandler_EVEX_VkHM(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VkHW_3:
				ptr = new OpCodeHandler_EVEX_VkHW(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.VkHW_3b:
				ptr = new OpCodeHandler_EVEX_VkHW(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), true);
				return 1;
			case EvexOpCodeHandlerKind.VkHW_5:
				ptr = new OpCodeHandler_EVEX_VkHW(deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.VkHW_er_4:
				ptr = new OpCodeHandler_EVEX_VkHW_er(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), deserializer.ReadBoolean(), false);
				return 1;
			case EvexOpCodeHandlerKind.VkHW_er_4b:
				ptr = new OpCodeHandler_EVEX_VkHW_er(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), deserializer.ReadBoolean(), true);
				return 1;
			case EvexOpCodeHandlerKind.VkHWIb_3:
				ptr = new OpCodeHandler_EVEX_VkHWIb(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.VkHWIb_3b:
				ptr = new OpCodeHandler_EVEX_VkHWIb(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), true);
				return 1;
			case EvexOpCodeHandlerKind.VkHWIb_5:
				ptr = new OpCodeHandler_EVEX_VkHWIb(deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.VkHWIb_er_4:
				ptr = new OpCodeHandler_EVEX_VkHWIb_er(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.VkHWIb_er_4b:
				ptr = new OpCodeHandler_EVEX_VkHWIb_er(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), true);
				return 1;
			case EvexOpCodeHandlerKind.VkM:
				ptr = new OpCodeHandler_EVEX_VkM(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VkW_3:
				ptr = new OpCodeHandler_EVEX_VkW(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.VkW_3b:
				ptr = new OpCodeHandler_EVEX_VkW(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), true);
				return 1;
			case EvexOpCodeHandlerKind.VkW_4:
				ptr = new OpCodeHandler_EVEX_VkW(deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.VkW_4b:
				ptr = new OpCodeHandler_EVEX_VkW(deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), true);
				return 1;
			case EvexOpCodeHandlerKind.VkW_er_4:
				ptr = new OpCodeHandler_EVEX_VkW_er(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), deserializer.ReadBoolean());
				return 1;
			case EvexOpCodeHandlerKind.VkW_er_5:
				ptr = new OpCodeHandler_EVEX_VkW_er(deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), deserializer.ReadBoolean());
				return 1;
			case EvexOpCodeHandlerKind.VkW_er_6:
				ptr = new OpCodeHandler_EVEX_VkW_er(deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), deserializer.ReadBoolean(), deserializer.ReadBoolean());
				return 1;
			case EvexOpCodeHandlerKind.VkWIb_3:
				ptr = new OpCodeHandler_EVEX_VkWIb(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.VkWIb_3b:
				ptr = new OpCodeHandler_EVEX_VkWIb(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), true);
				return 1;
			case EvexOpCodeHandlerKind.VkWIb_er:
				ptr = new OpCodeHandler_EVEX_VkWIb_er(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VM:
				ptr = new OpCodeHandler_EVEX_VM(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VSIB_k1:
				ptr = new OpCodeHandler_EVEX_VSIB_k1(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VSIB_k1_VX:
				ptr = new OpCodeHandler_EVEX_VSIB_k1_VX(deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VW:
				ptr = new OpCodeHandler_EVEX_VW(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VW_er:
				ptr = new OpCodeHandler_EVEX_VW_er(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VX_Ev:
			{
				Code code = deserializer.ReadCode();
				ptr = new OpCodeHandler_EVEX_VX_Ev(code, code + 1, deserializer.ReadTupleType(), deserializer.ReadTupleType());
				return 1;
			}
			case EvexOpCodeHandlerKind.WkHV:
				ptr = new OpCodeHandler_EVEX_WkHV(deserializer.ReadRegister(), deserializer.ReadCode());
				return 1;
			case EvexOpCodeHandlerKind.WkV_3:
				ptr = new OpCodeHandler_EVEX_WkV(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.WkV_4a:
				ptr = new OpCodeHandler_EVEX_WkV(deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.WkV_4b:
				ptr = new OpCodeHandler_EVEX_WkV(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), deserializer.ReadBoolean());
				return 1;
			case EvexOpCodeHandlerKind.WkVIb:
				ptr = new OpCodeHandler_EVEX_WkVIb(deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.WkVIb_er:
				ptr = new OpCodeHandler_EVEX_WkVIb_er(deserializer.ReadRegister(), deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.WV:
				ptr = new OpCodeHandler_EVEX_WV(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType());
				return 1;
			case EvexOpCodeHandlerKind.VkHW_er_ur_3:
				ptr = new OpCodeHandler_EVEX_VkHW_er_ur(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), false);
				return 1;
			case EvexOpCodeHandlerKind.VkHW_er_ur_3b:
				ptr = new OpCodeHandler_EVEX_VkHW_er_ur(deserializer.ReadRegister(), deserializer.ReadCode(), deserializer.ReadTupleType(), true);
				return 1;
			default:
				throw new InvalidOperationException();
			}
		}
	}
}
