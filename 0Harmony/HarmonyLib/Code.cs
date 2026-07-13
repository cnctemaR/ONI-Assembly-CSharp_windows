using System;
using System.Reflection.Emit;

namespace HarmonyLib
{
	public static class Code
	{
		public static Code.Operand_ Operand
		{
			get
			{
				return new Code.Operand_();
			}
		}

		public static Code.Nop_ Nop
		{
			get
			{
				return new Code.Nop_(OpCodes.Nop);
			}
		}

		public static Code.Break_ Break
		{
			get
			{
				return new Code.Break_(OpCodes.Break);
			}
		}

		public static Code.Ldarg_0_ Ldarg_0
		{
			get
			{
				return new Code.Ldarg_0_(OpCodes.Ldarg_0);
			}
		}

		public static Code.Ldarg_1_ Ldarg_1
		{
			get
			{
				return new Code.Ldarg_1_(OpCodes.Ldarg_1);
			}
		}

		public static Code.Ldarg_2_ Ldarg_2
		{
			get
			{
				return new Code.Ldarg_2_(OpCodes.Ldarg_2);
			}
		}

		public static Code.Ldarg_3_ Ldarg_3
		{
			get
			{
				return new Code.Ldarg_3_(OpCodes.Ldarg_3);
			}
		}

		public static Code.Ldloc_0_ Ldloc_0
		{
			get
			{
				return new Code.Ldloc_0_(OpCodes.Ldloc_0);
			}
		}

		public static Code.Ldloc_1_ Ldloc_1
		{
			get
			{
				return new Code.Ldloc_1_(OpCodes.Ldloc_1);
			}
		}

		public static Code.Ldloc_2_ Ldloc_2
		{
			get
			{
				return new Code.Ldloc_2_(OpCodes.Ldloc_2);
			}
		}

		public static Code.Ldloc_3_ Ldloc_3
		{
			get
			{
				return new Code.Ldloc_3_(OpCodes.Ldloc_3);
			}
		}

		public static Code.Stloc_0_ Stloc_0
		{
			get
			{
				return new Code.Stloc_0_(OpCodes.Stloc_0);
			}
		}

		public static Code.Stloc_1_ Stloc_1
		{
			get
			{
				return new Code.Stloc_1_(OpCodes.Stloc_1);
			}
		}

		public static Code.Stloc_2_ Stloc_2
		{
			get
			{
				return new Code.Stloc_2_(OpCodes.Stloc_2);
			}
		}

		public static Code.Stloc_3_ Stloc_3
		{
			get
			{
				return new Code.Stloc_3_(OpCodes.Stloc_3);
			}
		}

		public static Code.Ldarg_S_ Ldarg_S
		{
			get
			{
				return new Code.Ldarg_S_(OpCodes.Ldarg_S);
			}
		}

		public static Code.Ldarga_S_ Ldarga_S
		{
			get
			{
				return new Code.Ldarga_S_(OpCodes.Ldarga_S);
			}
		}

		public static Code.Starg_S_ Starg_S
		{
			get
			{
				return new Code.Starg_S_(OpCodes.Starg_S);
			}
		}

		public static Code.Ldloc_S_ Ldloc_S
		{
			get
			{
				return new Code.Ldloc_S_(OpCodes.Ldloc_S);
			}
		}

		public static Code.Ldloca_S_ Ldloca_S
		{
			get
			{
				return new Code.Ldloca_S_(OpCodes.Ldloca_S);
			}
		}

		public static Code.Stloc_S_ Stloc_S
		{
			get
			{
				return new Code.Stloc_S_(OpCodes.Stloc_S);
			}
		}

		public static Code.Ldnull_ Ldnull
		{
			get
			{
				return new Code.Ldnull_(OpCodes.Ldnull);
			}
		}

		public static Code.Ldc_I4_M1_ Ldc_I4_M1
		{
			get
			{
				return new Code.Ldc_I4_M1_(OpCodes.Ldc_I4_M1);
			}
		}

		public static Code.Ldc_I4_0_ Ldc_I4_0
		{
			get
			{
				return new Code.Ldc_I4_0_(OpCodes.Ldc_I4_0);
			}
		}

		public static Code.Ldc_I4_1_ Ldc_I4_1
		{
			get
			{
				return new Code.Ldc_I4_1_(OpCodes.Ldc_I4_1);
			}
		}

		public static Code.Ldc_I4_2_ Ldc_I4_2
		{
			get
			{
				return new Code.Ldc_I4_2_(OpCodes.Ldc_I4_2);
			}
		}

		public static Code.Ldc_I4_3_ Ldc_I4_3
		{
			get
			{
				return new Code.Ldc_I4_3_(OpCodes.Ldc_I4_3);
			}
		}

		public static Code.Ldc_I4_4_ Ldc_I4_4
		{
			get
			{
				return new Code.Ldc_I4_4_(OpCodes.Ldc_I4_4);
			}
		}

		public static Code.Ldc_I4_5_ Ldc_I4_5
		{
			get
			{
				return new Code.Ldc_I4_5_(OpCodes.Ldc_I4_5);
			}
		}

		public static Code.Ldc_I4_6_ Ldc_I4_6
		{
			get
			{
				return new Code.Ldc_I4_6_(OpCodes.Ldc_I4_6);
			}
		}

		public static Code.Ldc_I4_7_ Ldc_I4_7
		{
			get
			{
				return new Code.Ldc_I4_7_(OpCodes.Ldc_I4_7);
			}
		}

		public static Code.Ldc_I4_8_ Ldc_I4_8
		{
			get
			{
				return new Code.Ldc_I4_8_(OpCodes.Ldc_I4_8);
			}
		}

		public static Code.Ldc_I4_S_ Ldc_I4_S
		{
			get
			{
				return new Code.Ldc_I4_S_(OpCodes.Ldc_I4_S);
			}
		}

		public static Code.Ldc_I4_ Ldc_I4
		{
			get
			{
				return new Code.Ldc_I4_(OpCodes.Ldc_I4);
			}
		}

		public static Code.Ldc_I8_ Ldc_I8
		{
			get
			{
				return new Code.Ldc_I8_(OpCodes.Ldc_I8);
			}
		}

		public static Code.Ldc_R4_ Ldc_R4
		{
			get
			{
				return new Code.Ldc_R4_(OpCodes.Ldc_R4);
			}
		}

		public static Code.Ldc_R8_ Ldc_R8
		{
			get
			{
				return new Code.Ldc_R8_(OpCodes.Ldc_R8);
			}
		}

		public static Code.Dup_ Dup
		{
			get
			{
				return new Code.Dup_(OpCodes.Dup);
			}
		}

		public static Code.Pop_ Pop
		{
			get
			{
				return new Code.Pop_(OpCodes.Pop);
			}
		}

		public static Code.Jmp_ Jmp
		{
			get
			{
				return new Code.Jmp_(OpCodes.Jmp);
			}
		}

		public static Code.Call_ Call
		{
			get
			{
				return new Code.Call_(OpCodes.Call);
			}
		}

		public static Code.Calli_ Calli
		{
			get
			{
				return new Code.Calli_(OpCodes.Calli);
			}
		}

		public static Code.Ret_ Ret
		{
			get
			{
				return new Code.Ret_(OpCodes.Ret);
			}
		}

		public static Code.Br_S_ Br_S
		{
			get
			{
				return new Code.Br_S_(OpCodes.Br_S);
			}
		}

		public static Code.Brfalse_S_ Brfalse_S
		{
			get
			{
				return new Code.Brfalse_S_(OpCodes.Brfalse_S);
			}
		}

		public static Code.Brtrue_S_ Brtrue_S
		{
			get
			{
				return new Code.Brtrue_S_(OpCodes.Brtrue_S);
			}
		}

		public static Code.Beq_S_ Beq_S
		{
			get
			{
				return new Code.Beq_S_(OpCodes.Beq_S);
			}
		}

		public static Code.Bge_S_ Bge_S
		{
			get
			{
				return new Code.Bge_S_(OpCodes.Bge_S);
			}
		}

		public static Code.Bgt_S_ Bgt_S
		{
			get
			{
				return new Code.Bgt_S_(OpCodes.Bgt_S);
			}
		}

		public static Code.Ble_S_ Ble_S
		{
			get
			{
				return new Code.Ble_S_(OpCodes.Ble_S);
			}
		}

		public static Code.Blt_S_ Blt_S
		{
			get
			{
				return new Code.Blt_S_(OpCodes.Blt_S);
			}
		}

		public static Code.Bne_Un_S_ Bne_Un_S
		{
			get
			{
				return new Code.Bne_Un_S_(OpCodes.Bne_Un_S);
			}
		}

		public static Code.Bge_Un_S_ Bge_Un_S
		{
			get
			{
				return new Code.Bge_Un_S_(OpCodes.Bge_Un_S);
			}
		}

		public static Code.Bgt_Un_S_ Bgt_Un_S
		{
			get
			{
				return new Code.Bgt_Un_S_(OpCodes.Bgt_Un_S);
			}
		}

		public static Code.Ble_Un_S_ Ble_Un_S
		{
			get
			{
				return new Code.Ble_Un_S_(OpCodes.Ble_Un_S);
			}
		}

		public static Code.Blt_Un_S_ Blt_Un_S
		{
			get
			{
				return new Code.Blt_Un_S_(OpCodes.Blt_Un_S);
			}
		}

		public static Code.Br_ Br
		{
			get
			{
				return new Code.Br_(OpCodes.Br);
			}
		}

		public static Code.Brfalse_ Brfalse
		{
			get
			{
				return new Code.Brfalse_(OpCodes.Brfalse);
			}
		}

		public static Code.Brtrue_ Brtrue
		{
			get
			{
				return new Code.Brtrue_(OpCodes.Brtrue);
			}
		}

		public static Code.Beq_ Beq
		{
			get
			{
				return new Code.Beq_(OpCodes.Beq);
			}
		}

		public static Code.Bge_ Bge
		{
			get
			{
				return new Code.Bge_(OpCodes.Bge);
			}
		}

		public static Code.Bgt_ Bgt
		{
			get
			{
				return new Code.Bgt_(OpCodes.Bgt);
			}
		}

		public static Code.Ble_ Ble
		{
			get
			{
				return new Code.Ble_(OpCodes.Ble);
			}
		}

		public static Code.Blt_ Blt
		{
			get
			{
				return new Code.Blt_(OpCodes.Blt);
			}
		}

		public static Code.Bne_Un_ Bne_Un
		{
			get
			{
				return new Code.Bne_Un_(OpCodes.Bne_Un);
			}
		}

		public static Code.Bge_Un_ Bge_Un
		{
			get
			{
				return new Code.Bge_Un_(OpCodes.Bge_Un);
			}
		}

		public static Code.Bgt_Un_ Bgt_Un
		{
			get
			{
				return new Code.Bgt_Un_(OpCodes.Bgt_Un);
			}
		}

		public static Code.Ble_Un_ Ble_Un
		{
			get
			{
				return new Code.Ble_Un_(OpCodes.Ble_Un);
			}
		}

		public static Code.Blt_Un_ Blt_Un
		{
			get
			{
				return new Code.Blt_Un_(OpCodes.Blt_Un);
			}
		}

		public static Code.Switch_ Switch
		{
			get
			{
				return new Code.Switch_(OpCodes.Switch);
			}
		}

		public static Code.Ldind_I1_ Ldind_I1
		{
			get
			{
				return new Code.Ldind_I1_(OpCodes.Ldind_I1);
			}
		}

		public static Code.Ldind_U1_ Ldind_U1
		{
			get
			{
				return new Code.Ldind_U1_(OpCodes.Ldind_U1);
			}
		}

		public static Code.Ldind_I2_ Ldind_I2
		{
			get
			{
				return new Code.Ldind_I2_(OpCodes.Ldind_I2);
			}
		}

		public static Code.Ldind_U2_ Ldind_U2
		{
			get
			{
				return new Code.Ldind_U2_(OpCodes.Ldind_U2);
			}
		}

		public static Code.Ldind_I4_ Ldind_I4
		{
			get
			{
				return new Code.Ldind_I4_(OpCodes.Ldind_I4);
			}
		}

		public static Code.Ldind_U4_ Ldind_U4
		{
			get
			{
				return new Code.Ldind_U4_(OpCodes.Ldind_U4);
			}
		}

		public static Code.Ldind_I8_ Ldind_I8
		{
			get
			{
				return new Code.Ldind_I8_(OpCodes.Ldind_I8);
			}
		}

		public static Code.Ldind_I_ Ldind_I
		{
			get
			{
				return new Code.Ldind_I_(OpCodes.Ldind_I);
			}
		}

		public static Code.Ldind_R4_ Ldind_R4
		{
			get
			{
				return new Code.Ldind_R4_(OpCodes.Ldind_R4);
			}
		}

		public static Code.Ldind_R8_ Ldind_R8
		{
			get
			{
				return new Code.Ldind_R8_(OpCodes.Ldind_R8);
			}
		}

		public static Code.Ldind_Ref_ Ldind_Ref
		{
			get
			{
				return new Code.Ldind_Ref_(OpCodes.Ldind_Ref);
			}
		}

		public static Code.Stind_Ref_ Stind_Ref
		{
			get
			{
				return new Code.Stind_Ref_(OpCodes.Stind_Ref);
			}
		}

		public static Code.Stind_I1_ Stind_I1
		{
			get
			{
				return new Code.Stind_I1_(OpCodes.Stind_I1);
			}
		}

		public static Code.Stind_I2_ Stind_I2
		{
			get
			{
				return new Code.Stind_I2_(OpCodes.Stind_I2);
			}
		}

		public static Code.Stind_I4_ Stind_I4
		{
			get
			{
				return new Code.Stind_I4_(OpCodes.Stind_I4);
			}
		}

		public static Code.Stind_I8_ Stind_I8
		{
			get
			{
				return new Code.Stind_I8_(OpCodes.Stind_I8);
			}
		}

		public static Code.Stind_R4_ Stind_R4
		{
			get
			{
				return new Code.Stind_R4_(OpCodes.Stind_R4);
			}
		}

		public static Code.Stind_R8_ Stind_R8
		{
			get
			{
				return new Code.Stind_R8_(OpCodes.Stind_R8);
			}
		}

		public static Code.Add_ Add
		{
			get
			{
				return new Code.Add_(OpCodes.Add);
			}
		}

		public static Code.Sub_ Sub
		{
			get
			{
				return new Code.Sub_(OpCodes.Sub);
			}
		}

		public static Code.Mul_ Mul
		{
			get
			{
				return new Code.Mul_(OpCodes.Mul);
			}
		}

		public static Code.Div_ Div
		{
			get
			{
				return new Code.Div_(OpCodes.Div);
			}
		}

		public static Code.Div_Un_ Div_Un
		{
			get
			{
				return new Code.Div_Un_(OpCodes.Div_Un);
			}
		}

		public static Code.Rem_ Rem
		{
			get
			{
				return new Code.Rem_(OpCodes.Rem);
			}
		}

		public static Code.Rem_Un_ Rem_Un
		{
			get
			{
				return new Code.Rem_Un_(OpCodes.Rem_Un);
			}
		}

		public static Code.And_ And
		{
			get
			{
				return new Code.And_(OpCodes.And);
			}
		}

		public static Code.Or_ Or
		{
			get
			{
				return new Code.Or_(OpCodes.Or);
			}
		}

		public static Code.Xor_ Xor
		{
			get
			{
				return new Code.Xor_(OpCodes.Xor);
			}
		}

		public static Code.Shl_ Shl
		{
			get
			{
				return new Code.Shl_(OpCodes.Shl);
			}
		}

		public static Code.Shr_ Shr
		{
			get
			{
				return new Code.Shr_(OpCodes.Shr);
			}
		}

		public static Code.Shr_Un_ Shr_Un
		{
			get
			{
				return new Code.Shr_Un_(OpCodes.Shr_Un);
			}
		}

		public static Code.Neg_ Neg
		{
			get
			{
				return new Code.Neg_(OpCodes.Neg);
			}
		}

		public static Code.Not_ Not
		{
			get
			{
				return new Code.Not_(OpCodes.Not);
			}
		}

		public static Code.Conv_I1_ Conv_I1
		{
			get
			{
				return new Code.Conv_I1_(OpCodes.Conv_I1);
			}
		}

		public static Code.Conv_I2_ Conv_I2
		{
			get
			{
				return new Code.Conv_I2_(OpCodes.Conv_I2);
			}
		}

		public static Code.Conv_I4_ Conv_I4
		{
			get
			{
				return new Code.Conv_I4_(OpCodes.Conv_I4);
			}
		}

		public static Code.Conv_I8_ Conv_I8
		{
			get
			{
				return new Code.Conv_I8_(OpCodes.Conv_I8);
			}
		}

		public static Code.Conv_R4_ Conv_R4
		{
			get
			{
				return new Code.Conv_R4_(OpCodes.Conv_R4);
			}
		}

		public static Code.Conv_R8_ Conv_R8
		{
			get
			{
				return new Code.Conv_R8_(OpCodes.Conv_R8);
			}
		}

		public static Code.Conv_U4_ Conv_U4
		{
			get
			{
				return new Code.Conv_U4_(OpCodes.Conv_U4);
			}
		}

		public static Code.Conv_U8_ Conv_U8
		{
			get
			{
				return new Code.Conv_U8_(OpCodes.Conv_U8);
			}
		}

		public static Code.Callvirt_ Callvirt
		{
			get
			{
				return new Code.Callvirt_(OpCodes.Callvirt);
			}
		}

		public static Code.Cpobj_ Cpobj
		{
			get
			{
				return new Code.Cpobj_(OpCodes.Cpobj);
			}
		}

		public static Code.Ldobj_ Ldobj
		{
			get
			{
				return new Code.Ldobj_(OpCodes.Ldobj);
			}
		}

		public static Code.Ldstr_ Ldstr
		{
			get
			{
				return new Code.Ldstr_(OpCodes.Ldstr);
			}
		}

		public static Code.Newobj_ Newobj
		{
			get
			{
				return new Code.Newobj_(OpCodes.Newobj);
			}
		}

		public static Code.Castclass_ Castclass
		{
			get
			{
				return new Code.Castclass_(OpCodes.Castclass);
			}
		}

		public static Code.Isinst_ Isinst
		{
			get
			{
				return new Code.Isinst_(OpCodes.Isinst);
			}
		}

		public static Code.Conv_R_Un_ Conv_R_Un
		{
			get
			{
				return new Code.Conv_R_Un_(OpCodes.Conv_R_Un);
			}
		}

		public static Code.Unbox_ Unbox
		{
			get
			{
				return new Code.Unbox_(OpCodes.Unbox);
			}
		}

		public static Code.Throw_ Throw
		{
			get
			{
				return new Code.Throw_(OpCodes.Throw);
			}
		}

		public static Code.Ldfld_ Ldfld
		{
			get
			{
				return new Code.Ldfld_(OpCodes.Ldfld);
			}
		}

		public static Code.Ldflda_ Ldflda
		{
			get
			{
				return new Code.Ldflda_(OpCodes.Ldflda);
			}
		}

		public static Code.Stfld_ Stfld
		{
			get
			{
				return new Code.Stfld_(OpCodes.Stfld);
			}
		}

		public static Code.Ldsfld_ Ldsfld
		{
			get
			{
				return new Code.Ldsfld_(OpCodes.Ldsfld);
			}
		}

		public static Code.Ldsflda_ Ldsflda
		{
			get
			{
				return new Code.Ldsflda_(OpCodes.Ldsflda);
			}
		}

		public static Code.Stsfld_ Stsfld
		{
			get
			{
				return new Code.Stsfld_(OpCodes.Stsfld);
			}
		}

		public static Code.Stobj_ Stobj
		{
			get
			{
				return new Code.Stobj_(OpCodes.Stobj);
			}
		}

		public static Code.Conv_Ovf_I1_Un_ Conv_Ovf_I1_Un
		{
			get
			{
				return new Code.Conv_Ovf_I1_Un_(OpCodes.Conv_Ovf_I1_Un);
			}
		}

		public static Code.Conv_Ovf_I2_Un_ Conv_Ovf_I2_Un
		{
			get
			{
				return new Code.Conv_Ovf_I2_Un_(OpCodes.Conv_Ovf_I2_Un);
			}
		}

		public static Code.Conv_Ovf_I4_Un_ Conv_Ovf_I4_Un
		{
			get
			{
				return new Code.Conv_Ovf_I4_Un_(OpCodes.Conv_Ovf_I4_Un);
			}
		}

		public static Code.Conv_Ovf_I8_Un_ Conv_Ovf_I8_Un
		{
			get
			{
				return new Code.Conv_Ovf_I8_Un_(OpCodes.Conv_Ovf_I8_Un);
			}
		}

		public static Code.Conv_Ovf_U1_Un_ Conv_Ovf_U1_Un
		{
			get
			{
				return new Code.Conv_Ovf_U1_Un_(OpCodes.Conv_Ovf_U1_Un);
			}
		}

		public static Code.Conv_Ovf_U2_Un_ Conv_Ovf_U2_Un
		{
			get
			{
				return new Code.Conv_Ovf_U2_Un_(OpCodes.Conv_Ovf_U2_Un);
			}
		}

		public static Code.Conv_Ovf_U4_Un_ Conv_Ovf_U4_Un
		{
			get
			{
				return new Code.Conv_Ovf_U4_Un_(OpCodes.Conv_Ovf_U4_Un);
			}
		}

		public static Code.Conv_Ovf_U8_Un_ Conv_Ovf_U8_Un
		{
			get
			{
				return new Code.Conv_Ovf_U8_Un_(OpCodes.Conv_Ovf_U8_Un);
			}
		}

		public static Code.Conv_Ovf_I_Un_ Conv_Ovf_I_Un
		{
			get
			{
				return new Code.Conv_Ovf_I_Un_(OpCodes.Conv_Ovf_I_Un);
			}
		}

		public static Code.Conv_Ovf_U_Un_ Conv_Ovf_U_Un
		{
			get
			{
				return new Code.Conv_Ovf_U_Un_(OpCodes.Conv_Ovf_U_Un);
			}
		}

		public static Code.Box_ Box
		{
			get
			{
				return new Code.Box_(OpCodes.Box);
			}
		}

		public static Code.Newarr_ Newarr
		{
			get
			{
				return new Code.Newarr_(OpCodes.Newarr);
			}
		}

		public static Code.Ldlen_ Ldlen
		{
			get
			{
				return new Code.Ldlen_(OpCodes.Ldlen);
			}
		}

		public static Code.Ldelema_ Ldelema
		{
			get
			{
				return new Code.Ldelema_(OpCodes.Ldelema);
			}
		}

		public static Code.Ldelem_I1_ Ldelem_I1
		{
			get
			{
				return new Code.Ldelem_I1_(OpCodes.Ldelem_I1);
			}
		}

		public static Code.Ldelem_U1_ Ldelem_U1
		{
			get
			{
				return new Code.Ldelem_U1_(OpCodes.Ldelem_U1);
			}
		}

		public static Code.Ldelem_I2_ Ldelem_I2
		{
			get
			{
				return new Code.Ldelem_I2_(OpCodes.Ldelem_I2);
			}
		}

		public static Code.Ldelem_U2_ Ldelem_U2
		{
			get
			{
				return new Code.Ldelem_U2_(OpCodes.Ldelem_U2);
			}
		}

		public static Code.Ldelem_I4_ Ldelem_I4
		{
			get
			{
				return new Code.Ldelem_I4_(OpCodes.Ldelem_I4);
			}
		}

		public static Code.Ldelem_U4_ Ldelem_U4
		{
			get
			{
				return new Code.Ldelem_U4_(OpCodes.Ldelem_U4);
			}
		}

		public static Code.Ldelem_I8_ Ldelem_I8
		{
			get
			{
				return new Code.Ldelem_I8_(OpCodes.Ldelem_I8);
			}
		}

		public static Code.Ldelem_I_ Ldelem_I
		{
			get
			{
				return new Code.Ldelem_I_(OpCodes.Ldelem_I);
			}
		}

		public static Code.Ldelem_R4_ Ldelem_R4
		{
			get
			{
				return new Code.Ldelem_R4_(OpCodes.Ldelem_R4);
			}
		}

		public static Code.Ldelem_R8_ Ldelem_R8
		{
			get
			{
				return new Code.Ldelem_R8_(OpCodes.Ldelem_R8);
			}
		}

		public static Code.Ldelem_Ref_ Ldelem_Ref
		{
			get
			{
				return new Code.Ldelem_Ref_(OpCodes.Ldelem_Ref);
			}
		}

		public static Code.Stelem_I_ Stelem_I
		{
			get
			{
				return new Code.Stelem_I_(OpCodes.Stelem_I);
			}
		}

		public static Code.Stelem_I1_ Stelem_I1
		{
			get
			{
				return new Code.Stelem_I1_(OpCodes.Stelem_I1);
			}
		}

		public static Code.Stelem_I2_ Stelem_I2
		{
			get
			{
				return new Code.Stelem_I2_(OpCodes.Stelem_I2);
			}
		}

		public static Code.Stelem_I4_ Stelem_I4
		{
			get
			{
				return new Code.Stelem_I4_(OpCodes.Stelem_I4);
			}
		}

		public static Code.Stelem_I8_ Stelem_I8
		{
			get
			{
				return new Code.Stelem_I8_(OpCodes.Stelem_I8);
			}
		}

		public static Code.Stelem_R4_ Stelem_R4
		{
			get
			{
				return new Code.Stelem_R4_(OpCodes.Stelem_R4);
			}
		}

		public static Code.Stelem_R8_ Stelem_R8
		{
			get
			{
				return new Code.Stelem_R8_(OpCodes.Stelem_R8);
			}
		}

		public static Code.Stelem_Ref_ Stelem_Ref
		{
			get
			{
				return new Code.Stelem_Ref_(OpCodes.Stelem_Ref);
			}
		}

		public static Code.Ldelem_ Ldelem
		{
			get
			{
				return new Code.Ldelem_(OpCodes.Ldelem);
			}
		}

		public static Code.Stelem_ Stelem
		{
			get
			{
				return new Code.Stelem_(OpCodes.Stelem);
			}
		}

		public static Code.Unbox_Any_ Unbox_Any
		{
			get
			{
				return new Code.Unbox_Any_(OpCodes.Unbox_Any);
			}
		}

		public static Code.Conv_Ovf_I1_ Conv_Ovf_I1
		{
			get
			{
				return new Code.Conv_Ovf_I1_(OpCodes.Conv_Ovf_I1);
			}
		}

		public static Code.Conv_Ovf_U1_ Conv_Ovf_U1
		{
			get
			{
				return new Code.Conv_Ovf_U1_(OpCodes.Conv_Ovf_U1);
			}
		}

		public static Code.Conv_Ovf_I2_ Conv_Ovf_I2
		{
			get
			{
				return new Code.Conv_Ovf_I2_(OpCodes.Conv_Ovf_I2);
			}
		}

		public static Code.Conv_Ovf_U2_ Conv_Ovf_U2
		{
			get
			{
				return new Code.Conv_Ovf_U2_(OpCodes.Conv_Ovf_U2);
			}
		}

		public static Code.Conv_Ovf_I4_ Conv_Ovf_I4
		{
			get
			{
				return new Code.Conv_Ovf_I4_(OpCodes.Conv_Ovf_I4);
			}
		}

		public static Code.Conv_Ovf_U4_ Conv_Ovf_U4
		{
			get
			{
				return new Code.Conv_Ovf_U4_(OpCodes.Conv_Ovf_U4);
			}
		}

		public static Code.Conv_Ovf_I8_ Conv_Ovf_I8
		{
			get
			{
				return new Code.Conv_Ovf_I8_(OpCodes.Conv_Ovf_I8);
			}
		}

		public static Code.Conv_Ovf_U8_ Conv_Ovf_U8
		{
			get
			{
				return new Code.Conv_Ovf_U8_(OpCodes.Conv_Ovf_U8);
			}
		}

		public static Code.Refanyval_ Refanyval
		{
			get
			{
				return new Code.Refanyval_(OpCodes.Refanyval);
			}
		}

		public static Code.Ckfinite_ Ckfinite
		{
			get
			{
				return new Code.Ckfinite_(OpCodes.Ckfinite);
			}
		}

		public static Code.Mkrefany_ Mkrefany
		{
			get
			{
				return new Code.Mkrefany_(OpCodes.Mkrefany);
			}
		}

		public static Code.Ldtoken_ Ldtoken
		{
			get
			{
				return new Code.Ldtoken_(OpCodes.Ldtoken);
			}
		}

		public static Code.Conv_U2_ Conv_U2
		{
			get
			{
				return new Code.Conv_U2_(OpCodes.Conv_U2);
			}
		}

		public static Code.Conv_U1_ Conv_U1
		{
			get
			{
				return new Code.Conv_U1_(OpCodes.Conv_U1);
			}
		}

		public static Code.Conv_I_ Conv_I
		{
			get
			{
				return new Code.Conv_I_(OpCodes.Conv_I);
			}
		}

		public static Code.Conv_Ovf_I_ Conv_Ovf_I
		{
			get
			{
				return new Code.Conv_Ovf_I_(OpCodes.Conv_Ovf_I);
			}
		}

		public static Code.Conv_Ovf_U_ Conv_Ovf_U
		{
			get
			{
				return new Code.Conv_Ovf_U_(OpCodes.Conv_Ovf_U);
			}
		}

		public static Code.Add_Ovf_ Add_Ovf
		{
			get
			{
				return new Code.Add_Ovf_(OpCodes.Add_Ovf);
			}
		}

		public static Code.Add_Ovf_Un_ Add_Ovf_Un
		{
			get
			{
				return new Code.Add_Ovf_Un_(OpCodes.Add_Ovf_Un);
			}
		}

		public static Code.Mul_Ovf_ Mul_Ovf
		{
			get
			{
				return new Code.Mul_Ovf_(OpCodes.Mul_Ovf);
			}
		}

		public static Code.Mul_Ovf_Un_ Mul_Ovf_Un
		{
			get
			{
				return new Code.Mul_Ovf_Un_(OpCodes.Mul_Ovf_Un);
			}
		}

		public static Code.Sub_Ovf_ Sub_Ovf
		{
			get
			{
				return new Code.Sub_Ovf_(OpCodes.Sub_Ovf);
			}
		}

		public static Code.Sub_Ovf_Un_ Sub_Ovf_Un
		{
			get
			{
				return new Code.Sub_Ovf_Un_(OpCodes.Sub_Ovf_Un);
			}
		}

		public static Code.Endfinally_ Endfinally
		{
			get
			{
				return new Code.Endfinally_(OpCodes.Endfinally);
			}
		}

		public static Code.Leave_ Leave
		{
			get
			{
				return new Code.Leave_(OpCodes.Leave);
			}
		}

		public static Code.Leave_S_ Leave_S
		{
			get
			{
				return new Code.Leave_S_(OpCodes.Leave_S);
			}
		}

		public static Code.Stind_I_ Stind_I
		{
			get
			{
				return new Code.Stind_I_(OpCodes.Stind_I);
			}
		}

		public static Code.Conv_U_ Conv_U
		{
			get
			{
				return new Code.Conv_U_(OpCodes.Conv_U);
			}
		}

		public static Code.Prefix7_ Prefix7
		{
			get
			{
				return new Code.Prefix7_(OpCodes.Prefix7);
			}
		}

		public static Code.Prefix6_ Prefix6
		{
			get
			{
				return new Code.Prefix6_(OpCodes.Prefix6);
			}
		}

		public static Code.Prefix5_ Prefix5
		{
			get
			{
				return new Code.Prefix5_(OpCodes.Prefix5);
			}
		}

		public static Code.Prefix4_ Prefix4
		{
			get
			{
				return new Code.Prefix4_(OpCodes.Prefix4);
			}
		}

		public static Code.Prefix3_ Prefix3
		{
			get
			{
				return new Code.Prefix3_(OpCodes.Prefix3);
			}
		}

		public static Code.Prefix2_ Prefix2
		{
			get
			{
				return new Code.Prefix2_(OpCodes.Prefix2);
			}
		}

		public static Code.Prefix1_ Prefix1
		{
			get
			{
				return new Code.Prefix1_(OpCodes.Prefix1);
			}
		}

		public static Code.Prefixref_ Prefixref
		{
			get
			{
				return new Code.Prefixref_(OpCodes.Prefixref);
			}
		}

		public static Code.Arglist_ Arglist
		{
			get
			{
				return new Code.Arglist_(OpCodes.Arglist);
			}
		}

		public static Code.Ceq_ Ceq
		{
			get
			{
				return new Code.Ceq_(OpCodes.Ceq);
			}
		}

		public static Code.Cgt_ Cgt
		{
			get
			{
				return new Code.Cgt_(OpCodes.Cgt);
			}
		}

		public static Code.Cgt_Un_ Cgt_Un
		{
			get
			{
				return new Code.Cgt_Un_(OpCodes.Cgt_Un);
			}
		}

		public static Code.Clt_ Clt
		{
			get
			{
				return new Code.Clt_(OpCodes.Clt);
			}
		}

		public static Code.Clt_Un_ Clt_Un
		{
			get
			{
				return new Code.Clt_Un_(OpCodes.Clt_Un);
			}
		}

		public static Code.Ldftn_ Ldftn
		{
			get
			{
				return new Code.Ldftn_(OpCodes.Ldftn);
			}
		}

		public static Code.Ldvirtftn_ Ldvirtftn
		{
			get
			{
				return new Code.Ldvirtftn_(OpCodes.Ldvirtftn);
			}
		}

		public static Code.Ldarg_ Ldarg
		{
			get
			{
				return new Code.Ldarg_(OpCodes.Ldarg);
			}
		}

		public static Code.Ldarga_ Ldarga
		{
			get
			{
				return new Code.Ldarga_(OpCodes.Ldarga);
			}
		}

		public static Code.Starg_ Starg
		{
			get
			{
				return new Code.Starg_(OpCodes.Starg);
			}
		}

		public static Code.Ldloc_ Ldloc
		{
			get
			{
				return new Code.Ldloc_(OpCodes.Ldloc);
			}
		}

		public static Code.Ldloca_ Ldloca
		{
			get
			{
				return new Code.Ldloca_(OpCodes.Ldloca);
			}
		}

		public static Code.Stloc_ Stloc
		{
			get
			{
				return new Code.Stloc_(OpCodes.Stloc);
			}
		}

		public static Code.Localloc_ Localloc
		{
			get
			{
				return new Code.Localloc_(OpCodes.Localloc);
			}
		}

		public static Code.Endfilter_ Endfilter
		{
			get
			{
				return new Code.Endfilter_(OpCodes.Endfilter);
			}
		}

		public static Code.Unaligned_ Unaligned
		{
			get
			{
				return new Code.Unaligned_(OpCodes.Unaligned);
			}
		}

		public static Code.Volatile_ Volatile
		{
			get
			{
				return new Code.Volatile_(OpCodes.Volatile);
			}
		}

		public static Code.Tailcall_ Tailcall
		{
			get
			{
				return new Code.Tailcall_(OpCodes.Tailcall);
			}
		}

		public static Code.Initobj_ Initobj
		{
			get
			{
				return new Code.Initobj_(OpCodes.Initobj);
			}
		}

		public static Code.Constrained_ Constrained
		{
			get
			{
				return new Code.Constrained_(OpCodes.Constrained);
			}
		}

		public static Code.Cpblk_ Cpblk
		{
			get
			{
				return new Code.Cpblk_(OpCodes.Cpblk);
			}
		}

		public static Code.Initblk_ Initblk
		{
			get
			{
				return new Code.Initblk_(OpCodes.Initblk);
			}
		}

		public static Code.Rethrow_ Rethrow
		{
			get
			{
				return new Code.Rethrow_(OpCodes.Rethrow);
			}
		}

		public static Code.Sizeof_ Sizeof
		{
			get
			{
				return new Code.Sizeof_(OpCodes.Sizeof);
			}
		}

		public static Code.Refanytype_ Refanytype
		{
			get
			{
				return new Code.Refanytype_(OpCodes.Refanytype);
			}
		}

		public static Code.Readonly_ Readonly
		{
			get
			{
				return new Code.Readonly_(OpCodes.Readonly);
			}
		}

		public class Operand_ : CodeMatch
		{
			public Code.Operand_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Operand_)base.Set(operand, name);
				}
			}

			public Operand_()
				: base(null, null, null)
			{
			}
		}

		public class Nop_ : CodeMatch
		{
			public Nop_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Nop_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Nop_)base.Set(OpCodes.Nop, operand, name);
				}
			}
		}

		public class Break_ : CodeMatch
		{
			public Break_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Break_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Break_)base.Set(OpCodes.Break, operand, name);
				}
			}
		}

		public class Ldarg_0_ : CodeMatch
		{
			public Ldarg_0_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldarg_0_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarg_0_)base.Set(OpCodes.Ldarg_0, operand, name);
				}
			}
		}

		public class Ldarg_1_ : CodeMatch
		{
			public Ldarg_1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldarg_1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarg_1_)base.Set(OpCodes.Ldarg_1, operand, name);
				}
			}
		}

		public class Ldarg_2_ : CodeMatch
		{
			public Ldarg_2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldarg_2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarg_2_)base.Set(OpCodes.Ldarg_2, operand, name);
				}
			}
		}

		public class Ldarg_3_ : CodeMatch
		{
			public Ldarg_3_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldarg_3_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarg_3_)base.Set(OpCodes.Ldarg_3, operand, name);
				}
			}
		}

		public class Ldloc_0_ : CodeMatch
		{
			public Ldloc_0_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldloc_0_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloc_0_)base.Set(OpCodes.Ldloc_0, operand, name);
				}
			}
		}

		public class Ldloc_1_ : CodeMatch
		{
			public Ldloc_1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldloc_1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloc_1_)base.Set(OpCodes.Ldloc_1, operand, name);
				}
			}
		}

		public class Ldloc_2_ : CodeMatch
		{
			public Ldloc_2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldloc_2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloc_2_)base.Set(OpCodes.Ldloc_2, operand, name);
				}
			}
		}

		public class Ldloc_3_ : CodeMatch
		{
			public Ldloc_3_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldloc_3_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloc_3_)base.Set(OpCodes.Ldloc_3, operand, name);
				}
			}
		}

		public class Stloc_0_ : CodeMatch
		{
			public Stloc_0_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stloc_0_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stloc_0_)base.Set(OpCodes.Stloc_0, operand, name);
				}
			}
		}

		public class Stloc_1_ : CodeMatch
		{
			public Stloc_1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stloc_1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stloc_1_)base.Set(OpCodes.Stloc_1, operand, name);
				}
			}
		}

		public class Stloc_2_ : CodeMatch
		{
			public Stloc_2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stloc_2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stloc_2_)base.Set(OpCodes.Stloc_2, operand, name);
				}
			}
		}

		public class Stloc_3_ : CodeMatch
		{
			public Stloc_3_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stloc_3_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stloc_3_)base.Set(OpCodes.Stloc_3, operand, name);
				}
			}
		}

		public class Ldarg_S_ : CodeMatch
		{
			public Ldarg_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldarg_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarg_S_)base.Set(OpCodes.Ldarg_S, operand, name);
				}
			}
		}

		public class Ldarga_S_ : CodeMatch
		{
			public Ldarga_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldarga_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarga_S_)base.Set(OpCodes.Ldarga_S, operand, name);
				}
			}
		}

		public class Starg_S_ : CodeMatch
		{
			public Starg_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Starg_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Starg_S_)base.Set(OpCodes.Starg_S, operand, name);
				}
			}
		}

		public class Ldloc_S_ : CodeMatch
		{
			public Ldloc_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldloc_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloc_S_)base.Set(OpCodes.Ldloc_S, operand, name);
				}
			}
		}

		public class Ldloca_S_ : CodeMatch
		{
			public Ldloca_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldloca_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloca_S_)base.Set(OpCodes.Ldloca_S, operand, name);
				}
			}
		}

		public class Stloc_S_ : CodeMatch
		{
			public Stloc_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stloc_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stloc_S_)base.Set(OpCodes.Stloc_S, operand, name);
				}
			}
		}

		public class Ldnull_ : CodeMatch
		{
			public Ldnull_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldnull_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldnull_)base.Set(OpCodes.Ldnull, operand, name);
				}
			}
		}

		public class Ldc_I4_M1_ : CodeMatch
		{
			public Ldc_I4_M1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I4_M1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_M1_)base.Set(OpCodes.Ldc_I4_M1, operand, name);
				}
			}
		}

		public class Ldc_I4_0_ : CodeMatch
		{
			public Ldc_I4_0_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I4_0_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_0_)base.Set(OpCodes.Ldc_I4_0, operand, name);
				}
			}
		}

		public class Ldc_I4_1_ : CodeMatch
		{
			public Ldc_I4_1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I4_1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_1_)base.Set(OpCodes.Ldc_I4_1, operand, name);
				}
			}
		}

		public class Ldc_I4_2_ : CodeMatch
		{
			public Ldc_I4_2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I4_2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_2_)base.Set(OpCodes.Ldc_I4_2, operand, name);
				}
			}
		}

		public class Ldc_I4_3_ : CodeMatch
		{
			public Ldc_I4_3_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I4_3_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_3_)base.Set(OpCodes.Ldc_I4_3, operand, name);
				}
			}
		}

		public class Ldc_I4_4_ : CodeMatch
		{
			public Ldc_I4_4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I4_4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_4_)base.Set(OpCodes.Ldc_I4_4, operand, name);
				}
			}
		}

		public class Ldc_I4_5_ : CodeMatch
		{
			public Ldc_I4_5_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I4_5_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_5_)base.Set(OpCodes.Ldc_I4_5, operand, name);
				}
			}
		}

		public class Ldc_I4_6_ : CodeMatch
		{
			public Ldc_I4_6_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I4_6_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_6_)base.Set(OpCodes.Ldc_I4_6, operand, name);
				}
			}
		}

		public class Ldc_I4_7_ : CodeMatch
		{
			public Ldc_I4_7_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I4_7_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_7_)base.Set(OpCodes.Ldc_I4_7, operand, name);
				}
			}
		}

		public class Ldc_I4_8_ : CodeMatch
		{
			public Ldc_I4_8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I4_8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_8_)base.Set(OpCodes.Ldc_I4_8, operand, name);
				}
			}
		}

		public class Ldc_I4_S_ : CodeMatch
		{
			public Ldc_I4_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I4_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_S_)base.Set(OpCodes.Ldc_I4_S, operand, name);
				}
			}
		}

		public class Ldc_I4_ : CodeMatch
		{
			public Ldc_I4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_)base.Set(OpCodes.Ldc_I4, operand, name);
				}
			}
		}

		public class Ldc_I8_ : CodeMatch
		{
			public Ldc_I8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I8_)base.Set(OpCodes.Ldc_I8, operand, name);
				}
			}
		}

		public class Ldc_R4_ : CodeMatch
		{
			public Ldc_R4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_R4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_R4_)base.Set(OpCodes.Ldc_R4, operand, name);
				}
			}
		}

		public class Ldc_R8_ : CodeMatch
		{
			public Ldc_R8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldc_R8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_R8_)base.Set(OpCodes.Ldc_R8, operand, name);
				}
			}
		}

		public class Dup_ : CodeMatch
		{
			public Dup_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Dup_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Dup_)base.Set(OpCodes.Dup, operand, name);
				}
			}
		}

		public class Pop_ : CodeMatch
		{
			public Pop_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Pop_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Pop_)base.Set(OpCodes.Pop, operand, name);
				}
			}
		}

		public class Jmp_ : CodeMatch
		{
			public Jmp_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Jmp_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Jmp_)base.Set(OpCodes.Jmp, operand, name);
				}
			}
		}

		public class Call_ : CodeMatch
		{
			public Call_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Call_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Call_)base.Set(OpCodes.Call, operand, name);
				}
			}
		}

		public class Calli_ : CodeMatch
		{
			public Calli_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Calli_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Calli_)base.Set(OpCodes.Calli, operand, name);
				}
			}
		}

		public class Ret_ : CodeMatch
		{
			public Ret_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ret_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ret_)base.Set(OpCodes.Ret, operand, name);
				}
			}
		}

		public class Br_S_ : CodeMatch
		{
			public Br_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Br_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Br_S_)base.Set(OpCodes.Br_S, operand, name);
				}
			}
		}

		public class Brfalse_S_ : CodeMatch
		{
			public Brfalse_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Brfalse_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Brfalse_S_)base.Set(OpCodes.Brfalse_S, operand, name);
				}
			}
		}

		public class Brtrue_S_ : CodeMatch
		{
			public Brtrue_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Brtrue_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Brtrue_S_)base.Set(OpCodes.Brtrue_S, operand, name);
				}
			}
		}

		public class Beq_S_ : CodeMatch
		{
			public Beq_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Beq_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Beq_S_)base.Set(OpCodes.Beq_S, operand, name);
				}
			}
		}

		public class Bge_S_ : CodeMatch
		{
			public Bge_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Bge_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bge_S_)base.Set(OpCodes.Bge_S, operand, name);
				}
			}
		}

		public class Bgt_S_ : CodeMatch
		{
			public Bgt_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Bgt_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bgt_S_)base.Set(OpCodes.Bgt_S, operand, name);
				}
			}
		}

		public class Ble_S_ : CodeMatch
		{
			public Ble_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ble_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ble_S_)base.Set(OpCodes.Ble_S, operand, name);
				}
			}
		}

		public class Blt_S_ : CodeMatch
		{
			public Blt_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Blt_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Blt_S_)base.Set(OpCodes.Blt_S, operand, name);
				}
			}
		}

		public class Bne_Un_S_ : CodeMatch
		{
			public Bne_Un_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Bne_Un_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bne_Un_S_)base.Set(OpCodes.Bne_Un_S, operand, name);
				}
			}
		}

		public class Bge_Un_S_ : CodeMatch
		{
			public Bge_Un_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Bge_Un_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bge_Un_S_)base.Set(OpCodes.Bge_Un_S, operand, name);
				}
			}
		}

		public class Bgt_Un_S_ : CodeMatch
		{
			public Bgt_Un_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Bgt_Un_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bgt_Un_S_)base.Set(OpCodes.Bgt_Un_S, operand, name);
				}
			}
		}

		public class Ble_Un_S_ : CodeMatch
		{
			public Ble_Un_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ble_Un_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ble_Un_S_)base.Set(OpCodes.Ble_Un_S, operand, name);
				}
			}
		}

		public class Blt_Un_S_ : CodeMatch
		{
			public Blt_Un_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Blt_Un_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Blt_Un_S_)base.Set(OpCodes.Blt_Un_S, operand, name);
				}
			}
		}

		public class Br_ : CodeMatch
		{
			public Br_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Br_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Br_)base.Set(OpCodes.Br, operand, name);
				}
			}
		}

		public class Brfalse_ : CodeMatch
		{
			public Brfalse_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Brfalse_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Brfalse_)base.Set(OpCodes.Brfalse, operand, name);
				}
			}
		}

		public class Brtrue_ : CodeMatch
		{
			public Brtrue_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Brtrue_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Brtrue_)base.Set(OpCodes.Brtrue, operand, name);
				}
			}
		}

		public class Beq_ : CodeMatch
		{
			public Beq_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Beq_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Beq_)base.Set(OpCodes.Beq, operand, name);
				}
			}
		}

		public class Bge_ : CodeMatch
		{
			public Bge_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Bge_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bge_)base.Set(OpCodes.Bge, operand, name);
				}
			}
		}

		public class Bgt_ : CodeMatch
		{
			public Bgt_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Bgt_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bgt_)base.Set(OpCodes.Bgt, operand, name);
				}
			}
		}

		public class Ble_ : CodeMatch
		{
			public Ble_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ble_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ble_)base.Set(OpCodes.Ble, operand, name);
				}
			}
		}

		public class Blt_ : CodeMatch
		{
			public Blt_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Blt_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Blt_)base.Set(OpCodes.Blt, operand, name);
				}
			}
		}

		public class Bne_Un_ : CodeMatch
		{
			public Bne_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Bne_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bne_Un_)base.Set(OpCodes.Bne_Un, operand, name);
				}
			}
		}

		public class Bge_Un_ : CodeMatch
		{
			public Bge_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Bge_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bge_Un_)base.Set(OpCodes.Bge_Un, operand, name);
				}
			}
		}

		public class Bgt_Un_ : CodeMatch
		{
			public Bgt_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Bgt_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bgt_Un_)base.Set(OpCodes.Bgt_Un, operand, name);
				}
			}
		}

		public class Ble_Un_ : CodeMatch
		{
			public Ble_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ble_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ble_Un_)base.Set(OpCodes.Ble_Un, operand, name);
				}
			}
		}

		public class Blt_Un_ : CodeMatch
		{
			public Blt_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Blt_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Blt_Un_)base.Set(OpCodes.Blt_Un, operand, name);
				}
			}
		}

		public class Switch_ : CodeMatch
		{
			public Switch_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Switch_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Switch_)base.Set(OpCodes.Switch, operand, name);
				}
			}
		}

		public class Ldind_I1_ : CodeMatch
		{
			public Ldind_I1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldind_I1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_I1_)base.Set(OpCodes.Ldind_I1, operand, name);
				}
			}
		}

		public class Ldind_U1_ : CodeMatch
		{
			public Ldind_U1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldind_U1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_U1_)base.Set(OpCodes.Ldind_U1, operand, name);
				}
			}
		}

		public class Ldind_I2_ : CodeMatch
		{
			public Ldind_I2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldind_I2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_I2_)base.Set(OpCodes.Ldind_I2, operand, name);
				}
			}
		}

		public class Ldind_U2_ : CodeMatch
		{
			public Ldind_U2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldind_U2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_U2_)base.Set(OpCodes.Ldind_U2, operand, name);
				}
			}
		}

		public class Ldind_I4_ : CodeMatch
		{
			public Ldind_I4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldind_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_I4_)base.Set(OpCodes.Ldind_I4, operand, name);
				}
			}
		}

		public class Ldind_U4_ : CodeMatch
		{
			public Ldind_U4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldind_U4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_U4_)base.Set(OpCodes.Ldind_U4, operand, name);
				}
			}
		}

		public class Ldind_I8_ : CodeMatch
		{
			public Ldind_I8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldind_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_I8_)base.Set(OpCodes.Ldind_I8, operand, name);
				}
			}
		}

		public class Ldind_I_ : CodeMatch
		{
			public Ldind_I_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldind_I_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_I_)base.Set(OpCodes.Ldind_I, operand, name);
				}
			}
		}

		public class Ldind_R4_ : CodeMatch
		{
			public Ldind_R4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldind_R4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_R4_)base.Set(OpCodes.Ldind_R4, operand, name);
				}
			}
		}

		public class Ldind_R8_ : CodeMatch
		{
			public Ldind_R8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldind_R8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_R8_)base.Set(OpCodes.Ldind_R8, operand, name);
				}
			}
		}

		public class Ldind_Ref_ : CodeMatch
		{
			public Ldind_Ref_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldind_Ref_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_Ref_)base.Set(OpCodes.Ldind_Ref, operand, name);
				}
			}
		}

		public class Stind_Ref_ : CodeMatch
		{
			public Stind_Ref_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stind_Ref_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_Ref_)base.Set(OpCodes.Stind_Ref, operand, name);
				}
			}
		}

		public class Stind_I1_ : CodeMatch
		{
			public Stind_I1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stind_I1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_I1_)base.Set(OpCodes.Stind_I1, operand, name);
				}
			}
		}

		public class Stind_I2_ : CodeMatch
		{
			public Stind_I2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stind_I2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_I2_)base.Set(OpCodes.Stind_I2, operand, name);
				}
			}
		}

		public class Stind_I4_ : CodeMatch
		{
			public Stind_I4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stind_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_I4_)base.Set(OpCodes.Stind_I4, operand, name);
				}
			}
		}

		public class Stind_I8_ : CodeMatch
		{
			public Stind_I8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stind_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_I8_)base.Set(OpCodes.Stind_I8, operand, name);
				}
			}
		}

		public class Stind_R4_ : CodeMatch
		{
			public Stind_R4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stind_R4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_R4_)base.Set(OpCodes.Stind_R4, operand, name);
				}
			}
		}

		public class Stind_R8_ : CodeMatch
		{
			public Stind_R8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stind_R8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_R8_)base.Set(OpCodes.Stind_R8, operand, name);
				}
			}
		}

		public class Add_ : CodeMatch
		{
			public Add_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Add_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Add_)base.Set(OpCodes.Add, operand, name);
				}
			}
		}

		public class Sub_ : CodeMatch
		{
			public Sub_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Sub_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Sub_)base.Set(OpCodes.Sub, operand, name);
				}
			}
		}

		public class Mul_ : CodeMatch
		{
			public Mul_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Mul_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Mul_)base.Set(OpCodes.Mul, operand, name);
				}
			}
		}

		public class Div_ : CodeMatch
		{
			public Div_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Div_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Div_)base.Set(OpCodes.Div, operand, name);
				}
			}
		}

		public class Div_Un_ : CodeMatch
		{
			public Div_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Div_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Div_Un_)base.Set(OpCodes.Div_Un, operand, name);
				}
			}
		}

		public class Rem_ : CodeMatch
		{
			public Rem_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Rem_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Rem_)base.Set(OpCodes.Rem, operand, name);
				}
			}
		}

		public class Rem_Un_ : CodeMatch
		{
			public Rem_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Rem_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Rem_Un_)base.Set(OpCodes.Rem_Un, operand, name);
				}
			}
		}

		public class And_ : CodeMatch
		{
			public And_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.And_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.And_)base.Set(OpCodes.And, operand, name);
				}
			}
		}

		public class Or_ : CodeMatch
		{
			public Or_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Or_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Or_)base.Set(OpCodes.Or, operand, name);
				}
			}
		}

		public class Xor_ : CodeMatch
		{
			public Xor_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Xor_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Xor_)base.Set(OpCodes.Xor, operand, name);
				}
			}
		}

		public class Shl_ : CodeMatch
		{
			public Shl_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Shl_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Shl_)base.Set(OpCodes.Shl, operand, name);
				}
			}
		}

		public class Shr_ : CodeMatch
		{
			public Shr_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Shr_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Shr_)base.Set(OpCodes.Shr, operand, name);
				}
			}
		}

		public class Shr_Un_ : CodeMatch
		{
			public Shr_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Shr_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Shr_Un_)base.Set(OpCodes.Shr_Un, operand, name);
				}
			}
		}

		public class Neg_ : CodeMatch
		{
			public Neg_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Neg_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Neg_)base.Set(OpCodes.Neg, operand, name);
				}
			}
		}

		public class Not_ : CodeMatch
		{
			public Not_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Not_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Not_)base.Set(OpCodes.Not, operand, name);
				}
			}
		}

		public class Conv_I1_ : CodeMatch
		{
			public Conv_I1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_I1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_I1_)base.Set(OpCodes.Conv_I1, operand, name);
				}
			}
		}

		public class Conv_I2_ : CodeMatch
		{
			public Conv_I2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_I2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_I2_)base.Set(OpCodes.Conv_I2, operand, name);
				}
			}
		}

		public class Conv_I4_ : CodeMatch
		{
			public Conv_I4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_I4_)base.Set(OpCodes.Conv_I4, operand, name);
				}
			}
		}

		public class Conv_I8_ : CodeMatch
		{
			public Conv_I8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_I8_)base.Set(OpCodes.Conv_I8, operand, name);
				}
			}
		}

		public class Conv_R4_ : CodeMatch
		{
			public Conv_R4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_R4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_R4_)base.Set(OpCodes.Conv_R4, operand, name);
				}
			}
		}

		public class Conv_R8_ : CodeMatch
		{
			public Conv_R8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_R8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_R8_)base.Set(OpCodes.Conv_R8, operand, name);
				}
			}
		}

		public class Conv_U4_ : CodeMatch
		{
			public Conv_U4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_U4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_U4_)base.Set(OpCodes.Conv_U4, operand, name);
				}
			}
		}

		public class Conv_U8_ : CodeMatch
		{
			public Conv_U8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_U8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_U8_)base.Set(OpCodes.Conv_U8, operand, name);
				}
			}
		}

		public class Callvirt_ : CodeMatch
		{
			public Callvirt_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Callvirt_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Callvirt_)base.Set(OpCodes.Callvirt, operand, name);
				}
			}
		}

		public class Cpobj_ : CodeMatch
		{
			public Cpobj_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Cpobj_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Cpobj_)base.Set(OpCodes.Cpobj, operand, name);
				}
			}
		}

		public class Ldobj_ : CodeMatch
		{
			public Ldobj_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldobj_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldobj_)base.Set(OpCodes.Ldobj, operand, name);
				}
			}
		}

		public class Ldstr_ : CodeMatch
		{
			public Ldstr_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldstr_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldstr_)base.Set(OpCodes.Ldstr, operand, name);
				}
			}
		}

		public class Newobj_ : CodeMatch
		{
			public Newobj_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Newobj_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Newobj_)base.Set(OpCodes.Newobj, operand, name);
				}
			}
		}

		public class Castclass_ : CodeMatch
		{
			public Castclass_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Castclass_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Castclass_)base.Set(OpCodes.Castclass, operand, name);
				}
			}
		}

		public class Isinst_ : CodeMatch
		{
			public Isinst_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Isinst_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Isinst_)base.Set(OpCodes.Isinst, operand, name);
				}
			}
		}

		public class Conv_R_Un_ : CodeMatch
		{
			public Conv_R_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_R_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_R_Un_)base.Set(OpCodes.Conv_R_Un, operand, name);
				}
			}
		}

		public class Unbox_ : CodeMatch
		{
			public Unbox_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Unbox_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Unbox_)base.Set(OpCodes.Unbox, operand, name);
				}
			}
		}

		public class Throw_ : CodeMatch
		{
			public Throw_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Throw_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Throw_)base.Set(OpCodes.Throw, operand, name);
				}
			}
		}

		public class Ldfld_ : CodeMatch
		{
			public Ldfld_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldfld_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldfld_)base.Set(OpCodes.Ldfld, operand, name);
				}
			}
		}

		public class Ldflda_ : CodeMatch
		{
			public Ldflda_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldflda_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldflda_)base.Set(OpCodes.Ldflda, operand, name);
				}
			}
		}

		public class Stfld_ : CodeMatch
		{
			public Stfld_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stfld_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stfld_)base.Set(OpCodes.Stfld, operand, name);
				}
			}
		}

		public class Ldsfld_ : CodeMatch
		{
			public Ldsfld_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldsfld_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldsfld_)base.Set(OpCodes.Ldsfld, operand, name);
				}
			}
		}

		public class Ldsflda_ : CodeMatch
		{
			public Ldsflda_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldsflda_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldsflda_)base.Set(OpCodes.Ldsflda, operand, name);
				}
			}
		}

		public class Stsfld_ : CodeMatch
		{
			public Stsfld_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stsfld_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stsfld_)base.Set(OpCodes.Stsfld, operand, name);
				}
			}
		}

		public class Stobj_ : CodeMatch
		{
			public Stobj_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stobj_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stobj_)base.Set(OpCodes.Stobj, operand, name);
				}
			}
		}

		public class Conv_Ovf_I1_Un_ : CodeMatch
		{
			public Conv_Ovf_I1_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_I1_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I1_Un_)base.Set(OpCodes.Conv_Ovf_I1_Un, operand, name);
				}
			}
		}

		public class Conv_Ovf_I2_Un_ : CodeMatch
		{
			public Conv_Ovf_I2_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_I2_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I2_Un_)base.Set(OpCodes.Conv_Ovf_I2_Un, operand, name);
				}
			}
		}

		public class Conv_Ovf_I4_Un_ : CodeMatch
		{
			public Conv_Ovf_I4_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_I4_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I4_Un_)base.Set(OpCodes.Conv_Ovf_I4_Un, operand, name);
				}
			}
		}

		public class Conv_Ovf_I8_Un_ : CodeMatch
		{
			public Conv_Ovf_I8_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_I8_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I8_Un_)base.Set(OpCodes.Conv_Ovf_I8_Un, operand, name);
				}
			}
		}

		public class Conv_Ovf_U1_Un_ : CodeMatch
		{
			public Conv_Ovf_U1_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_U1_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U1_Un_)base.Set(OpCodes.Conv_Ovf_U1_Un, operand, name);
				}
			}
		}

		public class Conv_Ovf_U2_Un_ : CodeMatch
		{
			public Conv_Ovf_U2_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_U2_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U2_Un_)base.Set(OpCodes.Conv_Ovf_U2_Un, operand, name);
				}
			}
		}

		public class Conv_Ovf_U4_Un_ : CodeMatch
		{
			public Conv_Ovf_U4_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_U4_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U4_Un_)base.Set(OpCodes.Conv_Ovf_U4_Un, operand, name);
				}
			}
		}

		public class Conv_Ovf_U8_Un_ : CodeMatch
		{
			public Conv_Ovf_U8_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_U8_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U8_Un_)base.Set(OpCodes.Conv_Ovf_U8_Un, operand, name);
				}
			}
		}

		public class Conv_Ovf_I_Un_ : CodeMatch
		{
			public Conv_Ovf_I_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_I_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I_Un_)base.Set(OpCodes.Conv_Ovf_I_Un, operand, name);
				}
			}
		}

		public class Conv_Ovf_U_Un_ : CodeMatch
		{
			public Conv_Ovf_U_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_U_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U_Un_)base.Set(OpCodes.Conv_Ovf_U_Un, operand, name);
				}
			}
		}

		public class Box_ : CodeMatch
		{
			public Box_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Box_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Box_)base.Set(OpCodes.Box, operand, name);
				}
			}
		}

		public class Newarr_ : CodeMatch
		{
			public Newarr_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Newarr_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Newarr_)base.Set(OpCodes.Newarr, operand, name);
				}
			}
		}

		public class Ldlen_ : CodeMatch
		{
			public Ldlen_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldlen_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldlen_)base.Set(OpCodes.Ldlen, operand, name);
				}
			}
		}

		public class Ldelema_ : CodeMatch
		{
			public Ldelema_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelema_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelema_)base.Set(OpCodes.Ldelema, operand, name);
				}
			}
		}

		public class Ldelem_I1_ : CodeMatch
		{
			public Ldelem_I1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelem_I1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_I1_)base.Set(OpCodes.Ldelem_I1, operand, name);
				}
			}
		}

		public class Ldelem_U1_ : CodeMatch
		{
			public Ldelem_U1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelem_U1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_U1_)base.Set(OpCodes.Ldelem_U1, operand, name);
				}
			}
		}

		public class Ldelem_I2_ : CodeMatch
		{
			public Ldelem_I2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelem_I2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_I2_)base.Set(OpCodes.Ldelem_I2, operand, name);
				}
			}
		}

		public class Ldelem_U2_ : CodeMatch
		{
			public Ldelem_U2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelem_U2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_U2_)base.Set(OpCodes.Ldelem_U2, operand, name);
				}
			}
		}

		public class Ldelem_I4_ : CodeMatch
		{
			public Ldelem_I4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelem_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_I4_)base.Set(OpCodes.Ldelem_I4, operand, name);
				}
			}
		}

		public class Ldelem_U4_ : CodeMatch
		{
			public Ldelem_U4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelem_U4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_U4_)base.Set(OpCodes.Ldelem_U4, operand, name);
				}
			}
		}

		public class Ldelem_I8_ : CodeMatch
		{
			public Ldelem_I8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelem_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_I8_)base.Set(OpCodes.Ldelem_I8, operand, name);
				}
			}
		}

		public class Ldelem_I_ : CodeMatch
		{
			public Ldelem_I_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelem_I_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_I_)base.Set(OpCodes.Ldelem_I, operand, name);
				}
			}
		}

		public class Ldelem_R4_ : CodeMatch
		{
			public Ldelem_R4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelem_R4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_R4_)base.Set(OpCodes.Ldelem_R4, operand, name);
				}
			}
		}

		public class Ldelem_R8_ : CodeMatch
		{
			public Ldelem_R8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelem_R8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_R8_)base.Set(OpCodes.Ldelem_R8, operand, name);
				}
			}
		}

		public class Ldelem_Ref_ : CodeMatch
		{
			public Ldelem_Ref_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelem_Ref_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_Ref_)base.Set(OpCodes.Ldelem_Ref, operand, name);
				}
			}
		}

		public class Stelem_I_ : CodeMatch
		{
			public Stelem_I_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stelem_I_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_I_)base.Set(OpCodes.Stelem_I, operand, name);
				}
			}
		}

		public class Stelem_I1_ : CodeMatch
		{
			public Stelem_I1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stelem_I1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_I1_)base.Set(OpCodes.Stelem_I1, operand, name);
				}
			}
		}

		public class Stelem_I2_ : CodeMatch
		{
			public Stelem_I2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stelem_I2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_I2_)base.Set(OpCodes.Stelem_I2, operand, name);
				}
			}
		}

		public class Stelem_I4_ : CodeMatch
		{
			public Stelem_I4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stelem_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_I4_)base.Set(OpCodes.Stelem_I4, operand, name);
				}
			}
		}

		public class Stelem_I8_ : CodeMatch
		{
			public Stelem_I8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stelem_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_I8_)base.Set(OpCodes.Stelem_I8, operand, name);
				}
			}
		}

		public class Stelem_R4_ : CodeMatch
		{
			public Stelem_R4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stelem_R4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_R4_)base.Set(OpCodes.Stelem_R4, operand, name);
				}
			}
		}

		public class Stelem_R8_ : CodeMatch
		{
			public Stelem_R8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stelem_R8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_R8_)base.Set(OpCodes.Stelem_R8, operand, name);
				}
			}
		}

		public class Stelem_Ref_ : CodeMatch
		{
			public Stelem_Ref_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stelem_Ref_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_Ref_)base.Set(OpCodes.Stelem_Ref, operand, name);
				}
			}
		}

		public class Ldelem_ : CodeMatch
		{
			public Ldelem_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldelem_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_)base.Set(OpCodes.Ldelem, operand, name);
				}
			}
		}

		public class Stelem_ : CodeMatch
		{
			public Stelem_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stelem_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_)base.Set(OpCodes.Stelem, operand, name);
				}
			}
		}

		public class Unbox_Any_ : CodeMatch
		{
			public Unbox_Any_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Unbox_Any_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Unbox_Any_)base.Set(OpCodes.Unbox_Any, operand, name);
				}
			}
		}

		public class Conv_Ovf_I1_ : CodeMatch
		{
			public Conv_Ovf_I1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_I1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I1_)base.Set(OpCodes.Conv_Ovf_I1, operand, name);
				}
			}
		}

		public class Conv_Ovf_U1_ : CodeMatch
		{
			public Conv_Ovf_U1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_U1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U1_)base.Set(OpCodes.Conv_Ovf_U1, operand, name);
				}
			}
		}

		public class Conv_Ovf_I2_ : CodeMatch
		{
			public Conv_Ovf_I2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_I2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I2_)base.Set(OpCodes.Conv_Ovf_I2, operand, name);
				}
			}
		}

		public class Conv_Ovf_U2_ : CodeMatch
		{
			public Conv_Ovf_U2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_U2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U2_)base.Set(OpCodes.Conv_Ovf_U2, operand, name);
				}
			}
		}

		public class Conv_Ovf_I4_ : CodeMatch
		{
			public Conv_Ovf_I4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I4_)base.Set(OpCodes.Conv_Ovf_I4, operand, name);
				}
			}
		}

		public class Conv_Ovf_U4_ : CodeMatch
		{
			public Conv_Ovf_U4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_U4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U4_)base.Set(OpCodes.Conv_Ovf_U4, operand, name);
				}
			}
		}

		public class Conv_Ovf_I8_ : CodeMatch
		{
			public Conv_Ovf_I8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I8_)base.Set(OpCodes.Conv_Ovf_I8, operand, name);
				}
			}
		}

		public class Conv_Ovf_U8_ : CodeMatch
		{
			public Conv_Ovf_U8_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_U8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U8_)base.Set(OpCodes.Conv_Ovf_U8, operand, name);
				}
			}
		}

		public class Refanyval_ : CodeMatch
		{
			public Refanyval_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Refanyval_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Refanyval_)base.Set(OpCodes.Refanyval, operand, name);
				}
			}
		}

		public class Ckfinite_ : CodeMatch
		{
			public Ckfinite_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ckfinite_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ckfinite_)base.Set(OpCodes.Ckfinite, operand, name);
				}
			}
		}

		public class Mkrefany_ : CodeMatch
		{
			public Mkrefany_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Mkrefany_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Mkrefany_)base.Set(OpCodes.Mkrefany, operand, name);
				}
			}
		}

		public class Ldtoken_ : CodeMatch
		{
			public Ldtoken_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldtoken_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldtoken_)base.Set(OpCodes.Ldtoken, operand, name);
				}
			}
		}

		public class Conv_U2_ : CodeMatch
		{
			public Conv_U2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_U2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_U2_)base.Set(OpCodes.Conv_U2, operand, name);
				}
			}
		}

		public class Conv_U1_ : CodeMatch
		{
			public Conv_U1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_U1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_U1_)base.Set(OpCodes.Conv_U1, operand, name);
				}
			}
		}

		public class Conv_I_ : CodeMatch
		{
			public Conv_I_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_I_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_I_)base.Set(OpCodes.Conv_I, operand, name);
				}
			}
		}

		public class Conv_Ovf_I_ : CodeMatch
		{
			public Conv_Ovf_I_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_I_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I_)base.Set(OpCodes.Conv_Ovf_I, operand, name);
				}
			}
		}

		public class Conv_Ovf_U_ : CodeMatch
		{
			public Conv_Ovf_U_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_Ovf_U_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U_)base.Set(OpCodes.Conv_Ovf_U, operand, name);
				}
			}
		}

		public class Add_Ovf_ : CodeMatch
		{
			public Add_Ovf_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Add_Ovf_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Add_Ovf_)base.Set(OpCodes.Add_Ovf, operand, name);
				}
			}
		}

		public class Add_Ovf_Un_ : CodeMatch
		{
			public Add_Ovf_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Add_Ovf_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Add_Ovf_Un_)base.Set(OpCodes.Add_Ovf_Un, operand, name);
				}
			}
		}

		public class Mul_Ovf_ : CodeMatch
		{
			public Mul_Ovf_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Mul_Ovf_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Mul_Ovf_)base.Set(OpCodes.Mul_Ovf, operand, name);
				}
			}
		}

		public class Mul_Ovf_Un_ : CodeMatch
		{
			public Mul_Ovf_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Mul_Ovf_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Mul_Ovf_Un_)base.Set(OpCodes.Mul_Ovf_Un, operand, name);
				}
			}
		}

		public class Sub_Ovf_ : CodeMatch
		{
			public Sub_Ovf_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Sub_Ovf_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Sub_Ovf_)base.Set(OpCodes.Sub_Ovf, operand, name);
				}
			}
		}

		public class Sub_Ovf_Un_ : CodeMatch
		{
			public Sub_Ovf_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Sub_Ovf_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Sub_Ovf_Un_)base.Set(OpCodes.Sub_Ovf_Un, operand, name);
				}
			}
		}

		public class Endfinally_ : CodeMatch
		{
			public Endfinally_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Endfinally_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Endfinally_)base.Set(OpCodes.Endfinally, operand, name);
				}
			}
		}

		public class Leave_ : CodeMatch
		{
			public Leave_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Leave_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Leave_)base.Set(OpCodes.Leave, operand, name);
				}
			}
		}

		public class Leave_S_ : CodeMatch
		{
			public Leave_S_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Leave_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Leave_S_)base.Set(OpCodes.Leave_S, operand, name);
				}
			}
		}

		public class Stind_I_ : CodeMatch
		{
			public Stind_I_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stind_I_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_I_)base.Set(OpCodes.Stind_I, operand, name);
				}
			}
		}

		public class Conv_U_ : CodeMatch
		{
			public Conv_U_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Conv_U_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_U_)base.Set(OpCodes.Conv_U, operand, name);
				}
			}
		}

		public class Prefix7_ : CodeMatch
		{
			public Prefix7_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Prefix7_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix7_)base.Set(OpCodes.Prefix7, operand, name);
				}
			}
		}

		public class Prefix6_ : CodeMatch
		{
			public Prefix6_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Prefix6_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix6_)base.Set(OpCodes.Prefix6, operand, name);
				}
			}
		}

		public class Prefix5_ : CodeMatch
		{
			public Prefix5_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Prefix5_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix5_)base.Set(OpCodes.Prefix5, operand, name);
				}
			}
		}

		public class Prefix4_ : CodeMatch
		{
			public Prefix4_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Prefix4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix4_)base.Set(OpCodes.Prefix4, operand, name);
				}
			}
		}

		public class Prefix3_ : CodeMatch
		{
			public Prefix3_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Prefix3_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix3_)base.Set(OpCodes.Prefix3, operand, name);
				}
			}
		}

		public class Prefix2_ : CodeMatch
		{
			public Prefix2_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Prefix2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix2_)base.Set(OpCodes.Prefix2, operand, name);
				}
			}
		}

		public class Prefix1_ : CodeMatch
		{
			public Prefix1_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Prefix1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix1_)base.Set(OpCodes.Prefix1, operand, name);
				}
			}
		}

		public class Prefixref_ : CodeMatch
		{
			public Prefixref_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Prefixref_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefixref_)base.Set(OpCodes.Prefixref, operand, name);
				}
			}
		}

		public class Arglist_ : CodeMatch
		{
			public Arglist_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Arglist_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Arglist_)base.Set(OpCodes.Arglist, operand, name);
				}
			}
		}

		public class Ceq_ : CodeMatch
		{
			public Ceq_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ceq_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ceq_)base.Set(OpCodes.Ceq, operand, name);
				}
			}
		}

		public class Cgt_ : CodeMatch
		{
			public Cgt_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Cgt_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Cgt_)base.Set(OpCodes.Cgt, operand, name);
				}
			}
		}

		public class Cgt_Un_ : CodeMatch
		{
			public Cgt_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Cgt_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Cgt_Un_)base.Set(OpCodes.Cgt_Un, operand, name);
				}
			}
		}

		public class Clt_ : CodeMatch
		{
			public Clt_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Clt_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Clt_)base.Set(OpCodes.Clt, operand, name);
				}
			}
		}

		public class Clt_Un_ : CodeMatch
		{
			public Clt_Un_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Clt_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Clt_Un_)base.Set(OpCodes.Clt_Un, operand, name);
				}
			}
		}

		public class Ldftn_ : CodeMatch
		{
			public Ldftn_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldftn_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldftn_)base.Set(OpCodes.Ldftn, operand, name);
				}
			}
		}

		public class Ldvirtftn_ : CodeMatch
		{
			public Ldvirtftn_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldvirtftn_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldvirtftn_)base.Set(OpCodes.Ldvirtftn, operand, name);
				}
			}
		}

		public class Ldarg_ : CodeMatch
		{
			public Ldarg_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldarg_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarg_)base.Set(OpCodes.Ldarg, operand, name);
				}
			}
		}

		public class Ldarga_ : CodeMatch
		{
			public Ldarga_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldarga_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarga_)base.Set(OpCodes.Ldarga, operand, name);
				}
			}
		}

		public class Starg_ : CodeMatch
		{
			public Starg_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Starg_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Starg_)base.Set(OpCodes.Starg, operand, name);
				}
			}
		}

		public class Ldloc_ : CodeMatch
		{
			public Ldloc_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldloc_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloc_)base.Set(OpCodes.Ldloc, operand, name);
				}
			}
		}

		public class Ldloca_ : CodeMatch
		{
			public Ldloca_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Ldloca_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloca_)base.Set(OpCodes.Ldloca, operand, name);
				}
			}
		}

		public class Stloc_ : CodeMatch
		{
			public Stloc_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Stloc_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stloc_)base.Set(OpCodes.Stloc, operand, name);
				}
			}
		}

		public class Localloc_ : CodeMatch
		{
			public Localloc_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Localloc_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Localloc_)base.Set(OpCodes.Localloc, operand, name);
				}
			}
		}

		public class Endfilter_ : CodeMatch
		{
			public Endfilter_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Endfilter_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Endfilter_)base.Set(OpCodes.Endfilter, operand, name);
				}
			}
		}

		public class Unaligned_ : CodeMatch
		{
			public Unaligned_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Unaligned_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Unaligned_)base.Set(OpCodes.Unaligned, operand, name);
				}
			}
		}

		public class Volatile_ : CodeMatch
		{
			public Volatile_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Volatile_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Volatile_)base.Set(OpCodes.Volatile, operand, name);
				}
			}
		}

		public class Tailcall_ : CodeMatch
		{
			public Tailcall_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Tailcall_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Tailcall_)base.Set(OpCodes.Tailcall, operand, name);
				}
			}
		}

		public class Initobj_ : CodeMatch
		{
			public Initobj_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Initobj_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Initobj_)base.Set(OpCodes.Initobj, operand, name);
				}
			}
		}

		public class Constrained_ : CodeMatch
		{
			public Constrained_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Constrained_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Constrained_)base.Set(OpCodes.Constrained, operand, name);
				}
			}
		}

		public class Cpblk_ : CodeMatch
		{
			public Cpblk_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Cpblk_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Cpblk_)base.Set(OpCodes.Cpblk, operand, name);
				}
			}
		}

		public class Initblk_ : CodeMatch
		{
			public Initblk_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Initblk_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Initblk_)base.Set(OpCodes.Initblk, operand, name);
				}
			}
		}

		public class Rethrow_ : CodeMatch
		{
			public Rethrow_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Rethrow_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Rethrow_)base.Set(OpCodes.Rethrow, operand, name);
				}
			}
		}

		public class Sizeof_ : CodeMatch
		{
			public Sizeof_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Sizeof_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Sizeof_)base.Set(OpCodes.Sizeof, operand, name);
				}
			}
		}

		public class Refanytype_ : CodeMatch
		{
			public Refanytype_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Refanytype_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Refanytype_)base.Set(OpCodes.Refanytype, operand, name);
				}
			}
		}

		public class Readonly_ : CodeMatch
		{
			public Readonly_(OpCode opcode)
				: base(new OpCode?(opcode), null, null)
			{
			}

			public Code.Readonly_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Readonly_)base.Set(OpCodes.Readonly, operand, name);
				}
			}
		}
	}
}
