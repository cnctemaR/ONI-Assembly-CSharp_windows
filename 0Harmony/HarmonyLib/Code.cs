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
				return new Code.Nop_
				{
					opcode = OpCodes.Nop
				};
			}
		}

		public static Code.Break_ Break
		{
			get
			{
				return new Code.Break_
				{
					opcode = OpCodes.Break
				};
			}
		}

		public static Code.Ldarg_0_ Ldarg_0
		{
			get
			{
				return new Code.Ldarg_0_
				{
					opcode = OpCodes.Ldarg_0
				};
			}
		}

		public static Code.Ldarg_1_ Ldarg_1
		{
			get
			{
				return new Code.Ldarg_1_
				{
					opcode = OpCodes.Ldarg_1
				};
			}
		}

		public static Code.Ldarg_2_ Ldarg_2
		{
			get
			{
				return new Code.Ldarg_2_
				{
					opcode = OpCodes.Ldarg_2
				};
			}
		}

		public static Code.Ldarg_3_ Ldarg_3
		{
			get
			{
				return new Code.Ldarg_3_
				{
					opcode = OpCodes.Ldarg_3
				};
			}
		}

		public static Code.Ldloc_0_ Ldloc_0
		{
			get
			{
				return new Code.Ldloc_0_
				{
					opcode = OpCodes.Ldloc_0
				};
			}
		}

		public static Code.Ldloc_1_ Ldloc_1
		{
			get
			{
				return new Code.Ldloc_1_
				{
					opcode = OpCodes.Ldloc_1
				};
			}
		}

		public static Code.Ldloc_2_ Ldloc_2
		{
			get
			{
				return new Code.Ldloc_2_
				{
					opcode = OpCodes.Ldloc_2
				};
			}
		}

		public static Code.Ldloc_3_ Ldloc_3
		{
			get
			{
				return new Code.Ldloc_3_
				{
					opcode = OpCodes.Ldloc_3
				};
			}
		}

		public static Code.Stloc_0_ Stloc_0
		{
			get
			{
				return new Code.Stloc_0_
				{
					opcode = OpCodes.Stloc_0
				};
			}
		}

		public static Code.Stloc_1_ Stloc_1
		{
			get
			{
				return new Code.Stloc_1_
				{
					opcode = OpCodes.Stloc_1
				};
			}
		}

		public static Code.Stloc_2_ Stloc_2
		{
			get
			{
				return new Code.Stloc_2_
				{
					opcode = OpCodes.Stloc_2
				};
			}
		}

		public static Code.Stloc_3_ Stloc_3
		{
			get
			{
				return new Code.Stloc_3_
				{
					opcode = OpCodes.Stloc_3
				};
			}
		}

		public static Code.Ldarg_S_ Ldarg_S
		{
			get
			{
				return new Code.Ldarg_S_
				{
					opcode = OpCodes.Ldarg_S
				};
			}
		}

		public static Code.Ldarga_S_ Ldarga_S
		{
			get
			{
				return new Code.Ldarga_S_
				{
					opcode = OpCodes.Ldarga_S
				};
			}
		}

		public static Code.Starg_S_ Starg_S
		{
			get
			{
				return new Code.Starg_S_
				{
					opcode = OpCodes.Starg_S
				};
			}
		}

		public static Code.Ldloc_S_ Ldloc_S
		{
			get
			{
				return new Code.Ldloc_S_
				{
					opcode = OpCodes.Ldloc_S
				};
			}
		}

		public static Code.Ldloca_S_ Ldloca_S
		{
			get
			{
				return new Code.Ldloca_S_
				{
					opcode = OpCodes.Ldloca_S
				};
			}
		}

		public static Code.Stloc_S_ Stloc_S
		{
			get
			{
				return new Code.Stloc_S_
				{
					opcode = OpCodes.Stloc_S
				};
			}
		}

		public static Code.Ldnull_ Ldnull
		{
			get
			{
				return new Code.Ldnull_
				{
					opcode = OpCodes.Ldnull
				};
			}
		}

		public static Code.Ldc_I4_M1_ Ldc_I4_M1
		{
			get
			{
				return new Code.Ldc_I4_M1_
				{
					opcode = OpCodes.Ldc_I4_M1
				};
			}
		}

		public static Code.Ldc_I4_0_ Ldc_I4_0
		{
			get
			{
				return new Code.Ldc_I4_0_
				{
					opcode = OpCodes.Ldc_I4_0
				};
			}
		}

		public static Code.Ldc_I4_1_ Ldc_I4_1
		{
			get
			{
				return new Code.Ldc_I4_1_
				{
					opcode = OpCodes.Ldc_I4_1
				};
			}
		}

		public static Code.Ldc_I4_2_ Ldc_I4_2
		{
			get
			{
				return new Code.Ldc_I4_2_
				{
					opcode = OpCodes.Ldc_I4_2
				};
			}
		}

		public static Code.Ldc_I4_3_ Ldc_I4_3
		{
			get
			{
				return new Code.Ldc_I4_3_
				{
					opcode = OpCodes.Ldc_I4_3
				};
			}
		}

		public static Code.Ldc_I4_4_ Ldc_I4_4
		{
			get
			{
				return new Code.Ldc_I4_4_
				{
					opcode = OpCodes.Ldc_I4_4
				};
			}
		}

		public static Code.Ldc_I4_5_ Ldc_I4_5
		{
			get
			{
				return new Code.Ldc_I4_5_
				{
					opcode = OpCodes.Ldc_I4_5
				};
			}
		}

		public static Code.Ldc_I4_6_ Ldc_I4_6
		{
			get
			{
				return new Code.Ldc_I4_6_
				{
					opcode = OpCodes.Ldc_I4_6
				};
			}
		}

		public static Code.Ldc_I4_7_ Ldc_I4_7
		{
			get
			{
				return new Code.Ldc_I4_7_
				{
					opcode = OpCodes.Ldc_I4_7
				};
			}
		}

		public static Code.Ldc_I4_8_ Ldc_I4_8
		{
			get
			{
				return new Code.Ldc_I4_8_
				{
					opcode = OpCodes.Ldc_I4_8
				};
			}
		}

		public static Code.Ldc_I4_S_ Ldc_I4_S
		{
			get
			{
				return new Code.Ldc_I4_S_
				{
					opcode = OpCodes.Ldc_I4_S
				};
			}
		}

		public static Code.Ldc_I4_ Ldc_I4
		{
			get
			{
				return new Code.Ldc_I4_
				{
					opcode = OpCodes.Ldc_I4
				};
			}
		}

		public static Code.Ldc_I8_ Ldc_I8
		{
			get
			{
				return new Code.Ldc_I8_
				{
					opcode = OpCodes.Ldc_I8
				};
			}
		}

		public static Code.Ldc_R4_ Ldc_R4
		{
			get
			{
				return new Code.Ldc_R4_
				{
					opcode = OpCodes.Ldc_R4
				};
			}
		}

		public static Code.Ldc_R8_ Ldc_R8
		{
			get
			{
				return new Code.Ldc_R8_
				{
					opcode = OpCodes.Ldc_R8
				};
			}
		}

		public static Code.Dup_ Dup
		{
			get
			{
				return new Code.Dup_
				{
					opcode = OpCodes.Dup
				};
			}
		}

		public static Code.Pop_ Pop
		{
			get
			{
				return new Code.Pop_
				{
					opcode = OpCodes.Pop
				};
			}
		}

		public static Code.Jmp_ Jmp
		{
			get
			{
				return new Code.Jmp_
				{
					opcode = OpCodes.Jmp
				};
			}
		}

		public static Code.Call_ Call
		{
			get
			{
				return new Code.Call_
				{
					opcode = OpCodes.Call
				};
			}
		}

		public static Code.Calli_ Calli
		{
			get
			{
				return new Code.Calli_
				{
					opcode = OpCodes.Calli
				};
			}
		}

		public static Code.Ret_ Ret
		{
			get
			{
				return new Code.Ret_
				{
					opcode = OpCodes.Ret
				};
			}
		}

		public static Code.Br_S_ Br_S
		{
			get
			{
				return new Code.Br_S_
				{
					opcode = OpCodes.Br_S
				};
			}
		}

		public static Code.Brfalse_S_ Brfalse_S
		{
			get
			{
				return new Code.Brfalse_S_
				{
					opcode = OpCodes.Brfalse_S
				};
			}
		}

		public static Code.Brtrue_S_ Brtrue_S
		{
			get
			{
				return new Code.Brtrue_S_
				{
					opcode = OpCodes.Brtrue_S
				};
			}
		}

		public static Code.Beq_S_ Beq_S
		{
			get
			{
				return new Code.Beq_S_
				{
					opcode = OpCodes.Beq_S
				};
			}
		}

		public static Code.Bge_S_ Bge_S
		{
			get
			{
				return new Code.Bge_S_
				{
					opcode = OpCodes.Bge_S
				};
			}
		}

		public static Code.Bgt_S_ Bgt_S
		{
			get
			{
				return new Code.Bgt_S_
				{
					opcode = OpCodes.Bgt_S
				};
			}
		}

		public static Code.Ble_S_ Ble_S
		{
			get
			{
				return new Code.Ble_S_
				{
					opcode = OpCodes.Ble_S
				};
			}
		}

		public static Code.Blt_S_ Blt_S
		{
			get
			{
				return new Code.Blt_S_
				{
					opcode = OpCodes.Blt_S
				};
			}
		}

		public static Code.Bne_Un_S_ Bne_Un_S
		{
			get
			{
				return new Code.Bne_Un_S_
				{
					opcode = OpCodes.Bne_Un_S
				};
			}
		}

		public static Code.Bge_Un_S_ Bge_Un_S
		{
			get
			{
				return new Code.Bge_Un_S_
				{
					opcode = OpCodes.Bge_Un_S
				};
			}
		}

		public static Code.Bgt_Un_S_ Bgt_Un_S
		{
			get
			{
				return new Code.Bgt_Un_S_
				{
					opcode = OpCodes.Bgt_Un_S
				};
			}
		}

		public static Code.Ble_Un_S_ Ble_Un_S
		{
			get
			{
				return new Code.Ble_Un_S_
				{
					opcode = OpCodes.Ble_Un_S
				};
			}
		}

		public static Code.Blt_Un_S_ Blt_Un_S
		{
			get
			{
				return new Code.Blt_Un_S_
				{
					opcode = OpCodes.Blt_Un_S
				};
			}
		}

		public static Code.Br_ Br
		{
			get
			{
				return new Code.Br_
				{
					opcode = OpCodes.Br
				};
			}
		}

		public static Code.Brfalse_ Brfalse
		{
			get
			{
				return new Code.Brfalse_
				{
					opcode = OpCodes.Brfalse
				};
			}
		}

		public static Code.Brtrue_ Brtrue
		{
			get
			{
				return new Code.Brtrue_
				{
					opcode = OpCodes.Brtrue
				};
			}
		}

		public static Code.Beq_ Beq
		{
			get
			{
				return new Code.Beq_
				{
					opcode = OpCodes.Beq
				};
			}
		}

		public static Code.Bge_ Bge
		{
			get
			{
				return new Code.Bge_
				{
					opcode = OpCodes.Bge
				};
			}
		}

		public static Code.Bgt_ Bgt
		{
			get
			{
				return new Code.Bgt_
				{
					opcode = OpCodes.Bgt
				};
			}
		}

		public static Code.Ble_ Ble
		{
			get
			{
				return new Code.Ble_
				{
					opcode = OpCodes.Ble
				};
			}
		}

		public static Code.Blt_ Blt
		{
			get
			{
				return new Code.Blt_
				{
					opcode = OpCodes.Blt
				};
			}
		}

		public static Code.Bne_Un_ Bne_Un
		{
			get
			{
				return new Code.Bne_Un_
				{
					opcode = OpCodes.Bne_Un
				};
			}
		}

		public static Code.Bge_Un_ Bge_Un
		{
			get
			{
				return new Code.Bge_Un_
				{
					opcode = OpCodes.Bge_Un
				};
			}
		}

		public static Code.Bgt_Un_ Bgt_Un
		{
			get
			{
				return new Code.Bgt_Un_
				{
					opcode = OpCodes.Bgt_Un
				};
			}
		}

		public static Code.Ble_Un_ Ble_Un
		{
			get
			{
				return new Code.Ble_Un_
				{
					opcode = OpCodes.Ble_Un
				};
			}
		}

		public static Code.Blt_Un_ Blt_Un
		{
			get
			{
				return new Code.Blt_Un_
				{
					opcode = OpCodes.Blt_Un
				};
			}
		}

		public static Code.Switch_ Switch
		{
			get
			{
				return new Code.Switch_
				{
					opcode = OpCodes.Switch
				};
			}
		}

		public static Code.Ldind_I1_ Ldind_I1
		{
			get
			{
				return new Code.Ldind_I1_
				{
					opcode = OpCodes.Ldind_I1
				};
			}
		}

		public static Code.Ldind_U1_ Ldind_U1
		{
			get
			{
				return new Code.Ldind_U1_
				{
					opcode = OpCodes.Ldind_U1
				};
			}
		}

		public static Code.Ldind_I2_ Ldind_I2
		{
			get
			{
				return new Code.Ldind_I2_
				{
					opcode = OpCodes.Ldind_I2
				};
			}
		}

		public static Code.Ldind_U2_ Ldind_U2
		{
			get
			{
				return new Code.Ldind_U2_
				{
					opcode = OpCodes.Ldind_U2
				};
			}
		}

		public static Code.Ldind_I4_ Ldind_I4
		{
			get
			{
				return new Code.Ldind_I4_
				{
					opcode = OpCodes.Ldind_I4
				};
			}
		}

		public static Code.Ldind_U4_ Ldind_U4
		{
			get
			{
				return new Code.Ldind_U4_
				{
					opcode = OpCodes.Ldind_U4
				};
			}
		}

		public static Code.Ldind_I8_ Ldind_I8
		{
			get
			{
				return new Code.Ldind_I8_
				{
					opcode = OpCodes.Ldind_I8
				};
			}
		}

		public static Code.Ldind_I_ Ldind_I
		{
			get
			{
				return new Code.Ldind_I_
				{
					opcode = OpCodes.Ldind_I
				};
			}
		}

		public static Code.Ldind_R4_ Ldind_R4
		{
			get
			{
				return new Code.Ldind_R4_
				{
					opcode = OpCodes.Ldind_R4
				};
			}
		}

		public static Code.Ldind_R8_ Ldind_R8
		{
			get
			{
				return new Code.Ldind_R8_
				{
					opcode = OpCodes.Ldind_R8
				};
			}
		}

		public static Code.Ldind_Ref_ Ldind_Ref
		{
			get
			{
				return new Code.Ldind_Ref_
				{
					opcode = OpCodes.Ldind_Ref
				};
			}
		}

		public static Code.Stind_Ref_ Stind_Ref
		{
			get
			{
				return new Code.Stind_Ref_
				{
					opcode = OpCodes.Stind_Ref
				};
			}
		}

		public static Code.Stind_I1_ Stind_I1
		{
			get
			{
				return new Code.Stind_I1_
				{
					opcode = OpCodes.Stind_I1
				};
			}
		}

		public static Code.Stind_I2_ Stind_I2
		{
			get
			{
				return new Code.Stind_I2_
				{
					opcode = OpCodes.Stind_I2
				};
			}
		}

		public static Code.Stind_I4_ Stind_I4
		{
			get
			{
				return new Code.Stind_I4_
				{
					opcode = OpCodes.Stind_I4
				};
			}
		}

		public static Code.Stind_I8_ Stind_I8
		{
			get
			{
				return new Code.Stind_I8_
				{
					opcode = OpCodes.Stind_I8
				};
			}
		}

		public static Code.Stind_R4_ Stind_R4
		{
			get
			{
				return new Code.Stind_R4_
				{
					opcode = OpCodes.Stind_R4
				};
			}
		}

		public static Code.Stind_R8_ Stind_R8
		{
			get
			{
				return new Code.Stind_R8_
				{
					opcode = OpCodes.Stind_R8
				};
			}
		}

		public static Code.Add_ Add
		{
			get
			{
				return new Code.Add_
				{
					opcode = OpCodes.Add
				};
			}
		}

		public static Code.Sub_ Sub
		{
			get
			{
				return new Code.Sub_
				{
					opcode = OpCodes.Sub
				};
			}
		}

		public static Code.Mul_ Mul
		{
			get
			{
				return new Code.Mul_
				{
					opcode = OpCodes.Mul
				};
			}
		}

		public static Code.Div_ Div
		{
			get
			{
				return new Code.Div_
				{
					opcode = OpCodes.Div
				};
			}
		}

		public static Code.Div_Un_ Div_Un
		{
			get
			{
				return new Code.Div_Un_
				{
					opcode = OpCodes.Div_Un
				};
			}
		}

		public static Code.Rem_ Rem
		{
			get
			{
				return new Code.Rem_
				{
					opcode = OpCodes.Rem
				};
			}
		}

		public static Code.Rem_Un_ Rem_Un
		{
			get
			{
				return new Code.Rem_Un_
				{
					opcode = OpCodes.Rem_Un
				};
			}
		}

		public static Code.And_ And
		{
			get
			{
				return new Code.And_
				{
					opcode = OpCodes.And
				};
			}
		}

		public static Code.Or_ Or
		{
			get
			{
				return new Code.Or_
				{
					opcode = OpCodes.Or
				};
			}
		}

		public static Code.Xor_ Xor
		{
			get
			{
				return new Code.Xor_
				{
					opcode = OpCodes.Xor
				};
			}
		}

		public static Code.Shl_ Shl
		{
			get
			{
				return new Code.Shl_
				{
					opcode = OpCodes.Shl
				};
			}
		}

		public static Code.Shr_ Shr
		{
			get
			{
				return new Code.Shr_
				{
					opcode = OpCodes.Shr
				};
			}
		}

		public static Code.Shr_Un_ Shr_Un
		{
			get
			{
				return new Code.Shr_Un_
				{
					opcode = OpCodes.Shr_Un
				};
			}
		}

		public static Code.Neg_ Neg
		{
			get
			{
				return new Code.Neg_
				{
					opcode = OpCodes.Neg
				};
			}
		}

		public static Code.Not_ Not
		{
			get
			{
				return new Code.Not_
				{
					opcode = OpCodes.Not
				};
			}
		}

		public static Code.Conv_I1_ Conv_I1
		{
			get
			{
				return new Code.Conv_I1_
				{
					opcode = OpCodes.Conv_I1
				};
			}
		}

		public static Code.Conv_I2_ Conv_I2
		{
			get
			{
				return new Code.Conv_I2_
				{
					opcode = OpCodes.Conv_I2
				};
			}
		}

		public static Code.Conv_I4_ Conv_I4
		{
			get
			{
				return new Code.Conv_I4_
				{
					opcode = OpCodes.Conv_I4
				};
			}
		}

		public static Code.Conv_I8_ Conv_I8
		{
			get
			{
				return new Code.Conv_I8_
				{
					opcode = OpCodes.Conv_I8
				};
			}
		}

		public static Code.Conv_R4_ Conv_R4
		{
			get
			{
				return new Code.Conv_R4_
				{
					opcode = OpCodes.Conv_R4
				};
			}
		}

		public static Code.Conv_R8_ Conv_R8
		{
			get
			{
				return new Code.Conv_R8_
				{
					opcode = OpCodes.Conv_R8
				};
			}
		}

		public static Code.Conv_U4_ Conv_U4
		{
			get
			{
				return new Code.Conv_U4_
				{
					opcode = OpCodes.Conv_U4
				};
			}
		}

		public static Code.Conv_U8_ Conv_U8
		{
			get
			{
				return new Code.Conv_U8_
				{
					opcode = OpCodes.Conv_U8
				};
			}
		}

		public static Code.Callvirt_ Callvirt
		{
			get
			{
				return new Code.Callvirt_
				{
					opcode = OpCodes.Callvirt
				};
			}
		}

		public static Code.Cpobj_ Cpobj
		{
			get
			{
				return new Code.Cpobj_
				{
					opcode = OpCodes.Cpobj
				};
			}
		}

		public static Code.Ldobj_ Ldobj
		{
			get
			{
				return new Code.Ldobj_
				{
					opcode = OpCodes.Ldobj
				};
			}
		}

		public static Code.Ldstr_ Ldstr
		{
			get
			{
				return new Code.Ldstr_
				{
					opcode = OpCodes.Ldstr
				};
			}
		}

		public static Code.Newobj_ Newobj
		{
			get
			{
				return new Code.Newobj_
				{
					opcode = OpCodes.Newobj
				};
			}
		}

		public static Code.Castclass_ Castclass
		{
			get
			{
				return new Code.Castclass_
				{
					opcode = OpCodes.Castclass
				};
			}
		}

		public static Code.Isinst_ Isinst
		{
			get
			{
				return new Code.Isinst_
				{
					opcode = OpCodes.Isinst
				};
			}
		}

		public static Code.Conv_R_Un_ Conv_R_Un
		{
			get
			{
				return new Code.Conv_R_Un_
				{
					opcode = OpCodes.Conv_R_Un
				};
			}
		}

		public static Code.Unbox_ Unbox
		{
			get
			{
				return new Code.Unbox_
				{
					opcode = OpCodes.Unbox
				};
			}
		}

		public static Code.Throw_ Throw
		{
			get
			{
				return new Code.Throw_
				{
					opcode = OpCodes.Throw
				};
			}
		}

		public static Code.Ldfld_ Ldfld
		{
			get
			{
				return new Code.Ldfld_
				{
					opcode = OpCodes.Ldfld
				};
			}
		}

		public static Code.Ldflda_ Ldflda
		{
			get
			{
				return new Code.Ldflda_
				{
					opcode = OpCodes.Ldflda
				};
			}
		}

		public static Code.Stfld_ Stfld
		{
			get
			{
				return new Code.Stfld_
				{
					opcode = OpCodes.Stfld
				};
			}
		}

		public static Code.Ldsfld_ Ldsfld
		{
			get
			{
				return new Code.Ldsfld_
				{
					opcode = OpCodes.Ldsfld
				};
			}
		}

		public static Code.Ldsflda_ Ldsflda
		{
			get
			{
				return new Code.Ldsflda_
				{
					opcode = OpCodes.Ldsflda
				};
			}
		}

		public static Code.Stsfld_ Stsfld
		{
			get
			{
				return new Code.Stsfld_
				{
					opcode = OpCodes.Stsfld
				};
			}
		}

		public static Code.Stobj_ Stobj
		{
			get
			{
				return new Code.Stobj_
				{
					opcode = OpCodes.Stobj
				};
			}
		}

		public static Code.Conv_Ovf_I1_Un_ Conv_Ovf_I1_Un
		{
			get
			{
				return new Code.Conv_Ovf_I1_Un_
				{
					opcode = OpCodes.Conv_Ovf_I1_Un
				};
			}
		}

		public static Code.Conv_Ovf_I2_Un_ Conv_Ovf_I2_Un
		{
			get
			{
				return new Code.Conv_Ovf_I2_Un_
				{
					opcode = OpCodes.Conv_Ovf_I2_Un
				};
			}
		}

		public static Code.Conv_Ovf_I4_Un_ Conv_Ovf_I4_Un
		{
			get
			{
				return new Code.Conv_Ovf_I4_Un_
				{
					opcode = OpCodes.Conv_Ovf_I4_Un
				};
			}
		}

		public static Code.Conv_Ovf_I8_Un_ Conv_Ovf_I8_Un
		{
			get
			{
				return new Code.Conv_Ovf_I8_Un_
				{
					opcode = OpCodes.Conv_Ovf_I8_Un
				};
			}
		}

		public static Code.Conv_Ovf_U1_Un_ Conv_Ovf_U1_Un
		{
			get
			{
				return new Code.Conv_Ovf_U1_Un_
				{
					opcode = OpCodes.Conv_Ovf_U1_Un
				};
			}
		}

		public static Code.Conv_Ovf_U2_Un_ Conv_Ovf_U2_Un
		{
			get
			{
				return new Code.Conv_Ovf_U2_Un_
				{
					opcode = OpCodes.Conv_Ovf_U2_Un
				};
			}
		}

		public static Code.Conv_Ovf_U4_Un_ Conv_Ovf_U4_Un
		{
			get
			{
				return new Code.Conv_Ovf_U4_Un_
				{
					opcode = OpCodes.Conv_Ovf_U4_Un
				};
			}
		}

		public static Code.Conv_Ovf_U8_Un_ Conv_Ovf_U8_Un
		{
			get
			{
				return new Code.Conv_Ovf_U8_Un_
				{
					opcode = OpCodes.Conv_Ovf_U8_Un
				};
			}
		}

		public static Code.Conv_Ovf_I_Un_ Conv_Ovf_I_Un
		{
			get
			{
				return new Code.Conv_Ovf_I_Un_
				{
					opcode = OpCodes.Conv_Ovf_I_Un
				};
			}
		}

		public static Code.Conv_Ovf_U_Un_ Conv_Ovf_U_Un
		{
			get
			{
				return new Code.Conv_Ovf_U_Un_
				{
					opcode = OpCodes.Conv_Ovf_U_Un
				};
			}
		}

		public static Code.Box_ Box
		{
			get
			{
				return new Code.Box_
				{
					opcode = OpCodes.Box
				};
			}
		}

		public static Code.Newarr_ Newarr
		{
			get
			{
				return new Code.Newarr_
				{
					opcode = OpCodes.Newarr
				};
			}
		}

		public static Code.Ldlen_ Ldlen
		{
			get
			{
				return new Code.Ldlen_
				{
					opcode = OpCodes.Ldlen
				};
			}
		}

		public static Code.Ldelema_ Ldelema
		{
			get
			{
				return new Code.Ldelema_
				{
					opcode = OpCodes.Ldelema
				};
			}
		}

		public static Code.Ldelem_I1_ Ldelem_I1
		{
			get
			{
				return new Code.Ldelem_I1_
				{
					opcode = OpCodes.Ldelem_I1
				};
			}
		}

		public static Code.Ldelem_U1_ Ldelem_U1
		{
			get
			{
				return new Code.Ldelem_U1_
				{
					opcode = OpCodes.Ldelem_U1
				};
			}
		}

		public static Code.Ldelem_I2_ Ldelem_I2
		{
			get
			{
				return new Code.Ldelem_I2_
				{
					opcode = OpCodes.Ldelem_I2
				};
			}
		}

		public static Code.Ldelem_U2_ Ldelem_U2
		{
			get
			{
				return new Code.Ldelem_U2_
				{
					opcode = OpCodes.Ldelem_U2
				};
			}
		}

		public static Code.Ldelem_I4_ Ldelem_I4
		{
			get
			{
				return new Code.Ldelem_I4_
				{
					opcode = OpCodes.Ldelem_I4
				};
			}
		}

		public static Code.Ldelem_U4_ Ldelem_U4
		{
			get
			{
				return new Code.Ldelem_U4_
				{
					opcode = OpCodes.Ldelem_U4
				};
			}
		}

		public static Code.Ldelem_I8_ Ldelem_I8
		{
			get
			{
				return new Code.Ldelem_I8_
				{
					opcode = OpCodes.Ldelem_I8
				};
			}
		}

		public static Code.Ldelem_I_ Ldelem_I
		{
			get
			{
				return new Code.Ldelem_I_
				{
					opcode = OpCodes.Ldelem_I
				};
			}
		}

		public static Code.Ldelem_R4_ Ldelem_R4
		{
			get
			{
				return new Code.Ldelem_R4_
				{
					opcode = OpCodes.Ldelem_R4
				};
			}
		}

		public static Code.Ldelem_R8_ Ldelem_R8
		{
			get
			{
				return new Code.Ldelem_R8_
				{
					opcode = OpCodes.Ldelem_R8
				};
			}
		}

		public static Code.Ldelem_Ref_ Ldelem_Ref
		{
			get
			{
				return new Code.Ldelem_Ref_
				{
					opcode = OpCodes.Ldelem_Ref
				};
			}
		}

		public static Code.Stelem_I_ Stelem_I
		{
			get
			{
				return new Code.Stelem_I_
				{
					opcode = OpCodes.Stelem_I
				};
			}
		}

		public static Code.Stelem_I1_ Stelem_I1
		{
			get
			{
				return new Code.Stelem_I1_
				{
					opcode = OpCodes.Stelem_I1
				};
			}
		}

		public static Code.Stelem_I2_ Stelem_I2
		{
			get
			{
				return new Code.Stelem_I2_
				{
					opcode = OpCodes.Stelem_I2
				};
			}
		}

		public static Code.Stelem_I4_ Stelem_I4
		{
			get
			{
				return new Code.Stelem_I4_
				{
					opcode = OpCodes.Stelem_I4
				};
			}
		}

		public static Code.Stelem_I8_ Stelem_I8
		{
			get
			{
				return new Code.Stelem_I8_
				{
					opcode = OpCodes.Stelem_I8
				};
			}
		}

		public static Code.Stelem_R4_ Stelem_R4
		{
			get
			{
				return new Code.Stelem_R4_
				{
					opcode = OpCodes.Stelem_R4
				};
			}
		}

		public static Code.Stelem_R8_ Stelem_R8
		{
			get
			{
				return new Code.Stelem_R8_
				{
					opcode = OpCodes.Stelem_R8
				};
			}
		}

		public static Code.Stelem_Ref_ Stelem_Ref
		{
			get
			{
				return new Code.Stelem_Ref_
				{
					opcode = OpCodes.Stelem_Ref
				};
			}
		}

		public static Code.Ldelem_ Ldelem
		{
			get
			{
				return new Code.Ldelem_
				{
					opcode = OpCodes.Ldelem
				};
			}
		}

		public static Code.Stelem_ Stelem
		{
			get
			{
				return new Code.Stelem_
				{
					opcode = OpCodes.Stelem
				};
			}
		}

		public static Code.Unbox_Any_ Unbox_Any
		{
			get
			{
				return new Code.Unbox_Any_
				{
					opcode = OpCodes.Unbox_Any
				};
			}
		}

		public static Code.Conv_Ovf_I1_ Conv_Ovf_I1
		{
			get
			{
				return new Code.Conv_Ovf_I1_
				{
					opcode = OpCodes.Conv_Ovf_I1
				};
			}
		}

		public static Code.Conv_Ovf_U1_ Conv_Ovf_U1
		{
			get
			{
				return new Code.Conv_Ovf_U1_
				{
					opcode = OpCodes.Conv_Ovf_U1
				};
			}
		}

		public static Code.Conv_Ovf_I2_ Conv_Ovf_I2
		{
			get
			{
				return new Code.Conv_Ovf_I2_
				{
					opcode = OpCodes.Conv_Ovf_I2
				};
			}
		}

		public static Code.Conv_Ovf_U2_ Conv_Ovf_U2
		{
			get
			{
				return new Code.Conv_Ovf_U2_
				{
					opcode = OpCodes.Conv_Ovf_U2
				};
			}
		}

		public static Code.Conv_Ovf_I4_ Conv_Ovf_I4
		{
			get
			{
				return new Code.Conv_Ovf_I4_
				{
					opcode = OpCodes.Conv_Ovf_I4
				};
			}
		}

		public static Code.Conv_Ovf_U4_ Conv_Ovf_U4
		{
			get
			{
				return new Code.Conv_Ovf_U4_
				{
					opcode = OpCodes.Conv_Ovf_U4
				};
			}
		}

		public static Code.Conv_Ovf_I8_ Conv_Ovf_I8
		{
			get
			{
				return new Code.Conv_Ovf_I8_
				{
					opcode = OpCodes.Conv_Ovf_I8
				};
			}
		}

		public static Code.Conv_Ovf_U8_ Conv_Ovf_U8
		{
			get
			{
				return new Code.Conv_Ovf_U8_
				{
					opcode = OpCodes.Conv_Ovf_U8
				};
			}
		}

		public static Code.Refanyval_ Refanyval
		{
			get
			{
				return new Code.Refanyval_
				{
					opcode = OpCodes.Refanyval
				};
			}
		}

		public static Code.Ckfinite_ Ckfinite
		{
			get
			{
				return new Code.Ckfinite_
				{
					opcode = OpCodes.Ckfinite
				};
			}
		}

		public static Code.Mkrefany_ Mkrefany
		{
			get
			{
				return new Code.Mkrefany_
				{
					opcode = OpCodes.Mkrefany
				};
			}
		}

		public static Code.Ldtoken_ Ldtoken
		{
			get
			{
				return new Code.Ldtoken_
				{
					opcode = OpCodes.Ldtoken
				};
			}
		}

		public static Code.Conv_U2_ Conv_U2
		{
			get
			{
				return new Code.Conv_U2_
				{
					opcode = OpCodes.Conv_U2
				};
			}
		}

		public static Code.Conv_U1_ Conv_U1
		{
			get
			{
				return new Code.Conv_U1_
				{
					opcode = OpCodes.Conv_U1
				};
			}
		}

		public static Code.Conv_I_ Conv_I
		{
			get
			{
				return new Code.Conv_I_
				{
					opcode = OpCodes.Conv_I
				};
			}
		}

		public static Code.Conv_Ovf_I_ Conv_Ovf_I
		{
			get
			{
				return new Code.Conv_Ovf_I_
				{
					opcode = OpCodes.Conv_Ovf_I
				};
			}
		}

		public static Code.Conv_Ovf_U_ Conv_Ovf_U
		{
			get
			{
				return new Code.Conv_Ovf_U_
				{
					opcode = OpCodes.Conv_Ovf_U
				};
			}
		}

		public static Code.Add_Ovf_ Add_Ovf
		{
			get
			{
				return new Code.Add_Ovf_
				{
					opcode = OpCodes.Add_Ovf
				};
			}
		}

		public static Code.Add_Ovf_Un_ Add_Ovf_Un
		{
			get
			{
				return new Code.Add_Ovf_Un_
				{
					opcode = OpCodes.Add_Ovf_Un
				};
			}
		}

		public static Code.Mul_Ovf_ Mul_Ovf
		{
			get
			{
				return new Code.Mul_Ovf_
				{
					opcode = OpCodes.Mul_Ovf
				};
			}
		}

		public static Code.Mul_Ovf_Un_ Mul_Ovf_Un
		{
			get
			{
				return new Code.Mul_Ovf_Un_
				{
					opcode = OpCodes.Mul_Ovf_Un
				};
			}
		}

		public static Code.Sub_Ovf_ Sub_Ovf
		{
			get
			{
				return new Code.Sub_Ovf_
				{
					opcode = OpCodes.Sub_Ovf
				};
			}
		}

		public static Code.Sub_Ovf_Un_ Sub_Ovf_Un
		{
			get
			{
				return new Code.Sub_Ovf_Un_
				{
					opcode = OpCodes.Sub_Ovf_Un
				};
			}
		}

		public static Code.Endfinally_ Endfinally
		{
			get
			{
				return new Code.Endfinally_
				{
					opcode = OpCodes.Endfinally
				};
			}
		}

		public static Code.Leave_ Leave
		{
			get
			{
				return new Code.Leave_
				{
					opcode = OpCodes.Leave
				};
			}
		}

		public static Code.Leave_S_ Leave_S
		{
			get
			{
				return new Code.Leave_S_
				{
					opcode = OpCodes.Leave_S
				};
			}
		}

		public static Code.Stind_I_ Stind_I
		{
			get
			{
				return new Code.Stind_I_
				{
					opcode = OpCodes.Stind_I
				};
			}
		}

		public static Code.Conv_U_ Conv_U
		{
			get
			{
				return new Code.Conv_U_
				{
					opcode = OpCodes.Conv_U
				};
			}
		}

		public static Code.Prefix7_ Prefix7
		{
			get
			{
				return new Code.Prefix7_
				{
					opcode = OpCodes.Prefix7
				};
			}
		}

		public static Code.Prefix6_ Prefix6
		{
			get
			{
				return new Code.Prefix6_
				{
					opcode = OpCodes.Prefix6
				};
			}
		}

		public static Code.Prefix5_ Prefix5
		{
			get
			{
				return new Code.Prefix5_
				{
					opcode = OpCodes.Prefix5
				};
			}
		}

		public static Code.Prefix4_ Prefix4
		{
			get
			{
				return new Code.Prefix4_
				{
					opcode = OpCodes.Prefix4
				};
			}
		}

		public static Code.Prefix3_ Prefix3
		{
			get
			{
				return new Code.Prefix3_
				{
					opcode = OpCodes.Prefix3
				};
			}
		}

		public static Code.Prefix2_ Prefix2
		{
			get
			{
				return new Code.Prefix2_
				{
					opcode = OpCodes.Prefix2
				};
			}
		}

		public static Code.Prefix1_ Prefix1
		{
			get
			{
				return new Code.Prefix1_
				{
					opcode = OpCodes.Prefix1
				};
			}
		}

		public static Code.Prefixref_ Prefixref
		{
			get
			{
				return new Code.Prefixref_
				{
					opcode = OpCodes.Prefixref
				};
			}
		}

		public static Code.Arglist_ Arglist
		{
			get
			{
				return new Code.Arglist_
				{
					opcode = OpCodes.Arglist
				};
			}
		}

		public static Code.Ceq_ Ceq
		{
			get
			{
				return new Code.Ceq_
				{
					opcode = OpCodes.Ceq
				};
			}
		}

		public static Code.Cgt_ Cgt
		{
			get
			{
				return new Code.Cgt_
				{
					opcode = OpCodes.Cgt
				};
			}
		}

		public static Code.Cgt_Un_ Cgt_Un
		{
			get
			{
				return new Code.Cgt_Un_
				{
					opcode = OpCodes.Cgt_Un
				};
			}
		}

		public static Code.Clt_ Clt
		{
			get
			{
				return new Code.Clt_
				{
					opcode = OpCodes.Clt
				};
			}
		}

		public static Code.Clt_Un_ Clt_Un
		{
			get
			{
				return new Code.Clt_Un_
				{
					opcode = OpCodes.Clt_Un
				};
			}
		}

		public static Code.Ldftn_ Ldftn
		{
			get
			{
				return new Code.Ldftn_
				{
					opcode = OpCodes.Ldftn
				};
			}
		}

		public static Code.Ldvirtftn_ Ldvirtftn
		{
			get
			{
				return new Code.Ldvirtftn_
				{
					opcode = OpCodes.Ldvirtftn
				};
			}
		}

		public static Code.Ldarg_ Ldarg
		{
			get
			{
				return new Code.Ldarg_
				{
					opcode = OpCodes.Ldarg
				};
			}
		}

		public static Code.Ldarga_ Ldarga
		{
			get
			{
				return new Code.Ldarga_
				{
					opcode = OpCodes.Ldarga
				};
			}
		}

		public static Code.Starg_ Starg
		{
			get
			{
				return new Code.Starg_
				{
					opcode = OpCodes.Starg
				};
			}
		}

		public static Code.Ldloc_ Ldloc
		{
			get
			{
				return new Code.Ldloc_
				{
					opcode = OpCodes.Ldloc
				};
			}
		}

		public static Code.Ldloca_ Ldloca
		{
			get
			{
				return new Code.Ldloca_
				{
					opcode = OpCodes.Ldloca
				};
			}
		}

		public static Code.Stloc_ Stloc
		{
			get
			{
				return new Code.Stloc_
				{
					opcode = OpCodes.Stloc
				};
			}
		}

		public static Code.Localloc_ Localloc
		{
			get
			{
				return new Code.Localloc_
				{
					opcode = OpCodes.Localloc
				};
			}
		}

		public static Code.Endfilter_ Endfilter
		{
			get
			{
				return new Code.Endfilter_
				{
					opcode = OpCodes.Endfilter
				};
			}
		}

		public static Code.Unaligned_ Unaligned
		{
			get
			{
				return new Code.Unaligned_
				{
					opcode = OpCodes.Unaligned
				};
			}
		}

		public static Code.Volatile_ Volatile
		{
			get
			{
				return new Code.Volatile_
				{
					opcode = OpCodes.Volatile
				};
			}
		}

		public static Code.Tailcall_ Tailcall
		{
			get
			{
				return new Code.Tailcall_
				{
					opcode = OpCodes.Tailcall
				};
			}
		}

		public static Code.Initobj_ Initobj
		{
			get
			{
				return new Code.Initobj_
				{
					opcode = OpCodes.Initobj
				};
			}
		}

		public static Code.Constrained_ Constrained
		{
			get
			{
				return new Code.Constrained_
				{
					opcode = OpCodes.Constrained
				};
			}
		}

		public static Code.Cpblk_ Cpblk
		{
			get
			{
				return new Code.Cpblk_
				{
					opcode = OpCodes.Cpblk
				};
			}
		}

		public static Code.Initblk_ Initblk
		{
			get
			{
				return new Code.Initblk_
				{
					opcode = OpCodes.Initblk
				};
			}
		}

		public static Code.Rethrow_ Rethrow
		{
			get
			{
				return new Code.Rethrow_
				{
					opcode = OpCodes.Rethrow
				};
			}
		}

		public static Code.Sizeof_ Sizeof
		{
			get
			{
				return new Code.Sizeof_
				{
					opcode = OpCodes.Sizeof
				};
			}
		}

		public static Code.Refanytype_ Refanytype
		{
			get
			{
				return new Code.Refanytype_
				{
					opcode = OpCodes.Refanytype
				};
			}
		}

		public static Code.Readonly_ Readonly
		{
			get
			{
				return new Code.Readonly_
				{
					opcode = OpCodes.Readonly
				};
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
			public Code.Nop_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Nop_)base.Set(operand, name);
				}
			}

			public Nop_()
				: base(null, null, null)
			{
			}
		}

		public class Break_ : CodeMatch
		{
			public Code.Break_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Break_)base.Set(operand, name);
				}
			}

			public Break_()
				: base(null, null, null)
			{
			}
		}

		public class Ldarg_0_ : CodeMatch
		{
			public Code.Ldarg_0_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarg_0_)base.Set(operand, name);
				}
			}

			public Ldarg_0_()
				: base(null, null, null)
			{
			}
		}

		public class Ldarg_1_ : CodeMatch
		{
			public Code.Ldarg_1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarg_1_)base.Set(operand, name);
				}
			}

			public Ldarg_1_()
				: base(null, null, null)
			{
			}
		}

		public class Ldarg_2_ : CodeMatch
		{
			public Code.Ldarg_2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarg_2_)base.Set(operand, name);
				}
			}

			public Ldarg_2_()
				: base(null, null, null)
			{
			}
		}

		public class Ldarg_3_ : CodeMatch
		{
			public Code.Ldarg_3_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarg_3_)base.Set(operand, name);
				}
			}

			public Ldarg_3_()
				: base(null, null, null)
			{
			}
		}

		public class Ldloc_0_ : CodeMatch
		{
			public Code.Ldloc_0_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloc_0_)base.Set(operand, name);
				}
			}

			public Ldloc_0_()
				: base(null, null, null)
			{
			}
		}

		public class Ldloc_1_ : CodeMatch
		{
			public Code.Ldloc_1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloc_1_)base.Set(operand, name);
				}
			}

			public Ldloc_1_()
				: base(null, null, null)
			{
			}
		}

		public class Ldloc_2_ : CodeMatch
		{
			public Code.Ldloc_2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloc_2_)base.Set(operand, name);
				}
			}

			public Ldloc_2_()
				: base(null, null, null)
			{
			}
		}

		public class Ldloc_3_ : CodeMatch
		{
			public Code.Ldloc_3_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloc_3_)base.Set(operand, name);
				}
			}

			public Ldloc_3_()
				: base(null, null, null)
			{
			}
		}

		public class Stloc_0_ : CodeMatch
		{
			public Code.Stloc_0_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stloc_0_)base.Set(operand, name);
				}
			}

			public Stloc_0_()
				: base(null, null, null)
			{
			}
		}

		public class Stloc_1_ : CodeMatch
		{
			public Code.Stloc_1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stloc_1_)base.Set(operand, name);
				}
			}

			public Stloc_1_()
				: base(null, null, null)
			{
			}
		}

		public class Stloc_2_ : CodeMatch
		{
			public Code.Stloc_2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stloc_2_)base.Set(operand, name);
				}
			}

			public Stloc_2_()
				: base(null, null, null)
			{
			}
		}

		public class Stloc_3_ : CodeMatch
		{
			public Code.Stloc_3_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stloc_3_)base.Set(operand, name);
				}
			}

			public Stloc_3_()
				: base(null, null, null)
			{
			}
		}

		public class Ldarg_S_ : CodeMatch
		{
			public Code.Ldarg_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarg_S_)base.Set(operand, name);
				}
			}

			public Ldarg_S_()
				: base(null, null, null)
			{
			}
		}

		public class Ldarga_S_ : CodeMatch
		{
			public Code.Ldarga_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarga_S_)base.Set(operand, name);
				}
			}

			public Ldarga_S_()
				: base(null, null, null)
			{
			}
		}

		public class Starg_S_ : CodeMatch
		{
			public Code.Starg_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Starg_S_)base.Set(operand, name);
				}
			}

			public Starg_S_()
				: base(null, null, null)
			{
			}
		}

		public class Ldloc_S_ : CodeMatch
		{
			public Code.Ldloc_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloc_S_)base.Set(operand, name);
				}
			}

			public Ldloc_S_()
				: base(null, null, null)
			{
			}
		}

		public class Ldloca_S_ : CodeMatch
		{
			public Code.Ldloca_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloca_S_)base.Set(operand, name);
				}
			}

			public Ldloca_S_()
				: base(null, null, null)
			{
			}
		}

		public class Stloc_S_ : CodeMatch
		{
			public Code.Stloc_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stloc_S_)base.Set(operand, name);
				}
			}

			public Stloc_S_()
				: base(null, null, null)
			{
			}
		}

		public class Ldnull_ : CodeMatch
		{
			public Code.Ldnull_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldnull_)base.Set(operand, name);
				}
			}

			public Ldnull_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I4_M1_ : CodeMatch
		{
			public Code.Ldc_I4_M1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_M1_)base.Set(operand, name);
				}
			}

			public Ldc_I4_M1_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I4_0_ : CodeMatch
		{
			public Code.Ldc_I4_0_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_0_)base.Set(operand, name);
				}
			}

			public Ldc_I4_0_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I4_1_ : CodeMatch
		{
			public Code.Ldc_I4_1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_1_)base.Set(operand, name);
				}
			}

			public Ldc_I4_1_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I4_2_ : CodeMatch
		{
			public Code.Ldc_I4_2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_2_)base.Set(operand, name);
				}
			}

			public Ldc_I4_2_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I4_3_ : CodeMatch
		{
			public Code.Ldc_I4_3_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_3_)base.Set(operand, name);
				}
			}

			public Ldc_I4_3_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I4_4_ : CodeMatch
		{
			public Code.Ldc_I4_4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_4_)base.Set(operand, name);
				}
			}

			public Ldc_I4_4_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I4_5_ : CodeMatch
		{
			public Code.Ldc_I4_5_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_5_)base.Set(operand, name);
				}
			}

			public Ldc_I4_5_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I4_6_ : CodeMatch
		{
			public Code.Ldc_I4_6_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_6_)base.Set(operand, name);
				}
			}

			public Ldc_I4_6_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I4_7_ : CodeMatch
		{
			public Code.Ldc_I4_7_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_7_)base.Set(operand, name);
				}
			}

			public Ldc_I4_7_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I4_8_ : CodeMatch
		{
			public Code.Ldc_I4_8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_8_)base.Set(operand, name);
				}
			}

			public Ldc_I4_8_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I4_S_ : CodeMatch
		{
			public Code.Ldc_I4_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_S_)base.Set(operand, name);
				}
			}

			public Ldc_I4_S_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I4_ : CodeMatch
		{
			public Code.Ldc_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I4_)base.Set(operand, name);
				}
			}

			public Ldc_I4_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_I8_ : CodeMatch
		{
			public Code.Ldc_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_I8_)base.Set(operand, name);
				}
			}

			public Ldc_I8_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_R4_ : CodeMatch
		{
			public Code.Ldc_R4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_R4_)base.Set(operand, name);
				}
			}

			public Ldc_R4_()
				: base(null, null, null)
			{
			}
		}

		public class Ldc_R8_ : CodeMatch
		{
			public Code.Ldc_R8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldc_R8_)base.Set(operand, name);
				}
			}

			public Ldc_R8_()
				: base(null, null, null)
			{
			}
		}

		public class Dup_ : CodeMatch
		{
			public Code.Dup_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Dup_)base.Set(operand, name);
				}
			}

			public Dup_()
				: base(null, null, null)
			{
			}
		}

		public class Pop_ : CodeMatch
		{
			public Code.Pop_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Pop_)base.Set(operand, name);
				}
			}

			public Pop_()
				: base(null, null, null)
			{
			}
		}

		public class Jmp_ : CodeMatch
		{
			public Code.Jmp_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Jmp_)base.Set(operand, name);
				}
			}

			public Jmp_()
				: base(null, null, null)
			{
			}
		}

		public class Call_ : CodeMatch
		{
			public Code.Call_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Call_)base.Set(operand, name);
				}
			}

			public Call_()
				: base(null, null, null)
			{
			}
		}

		public class Calli_ : CodeMatch
		{
			public Code.Calli_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Calli_)base.Set(operand, name);
				}
			}

			public Calli_()
				: base(null, null, null)
			{
			}
		}

		public class Ret_ : CodeMatch
		{
			public Code.Ret_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ret_)base.Set(operand, name);
				}
			}

			public Ret_()
				: base(null, null, null)
			{
			}
		}

		public class Br_S_ : CodeMatch
		{
			public Code.Br_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Br_S_)base.Set(operand, name);
				}
			}

			public Br_S_()
				: base(null, null, null)
			{
			}
		}

		public class Brfalse_S_ : CodeMatch
		{
			public Code.Brfalse_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Brfalse_S_)base.Set(operand, name);
				}
			}

			public Brfalse_S_()
				: base(null, null, null)
			{
			}
		}

		public class Brtrue_S_ : CodeMatch
		{
			public Code.Brtrue_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Brtrue_S_)base.Set(operand, name);
				}
			}

			public Brtrue_S_()
				: base(null, null, null)
			{
			}
		}

		public class Beq_S_ : CodeMatch
		{
			public Code.Beq_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Beq_S_)base.Set(operand, name);
				}
			}

			public Beq_S_()
				: base(null, null, null)
			{
			}
		}

		public class Bge_S_ : CodeMatch
		{
			public Code.Bge_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bge_S_)base.Set(operand, name);
				}
			}

			public Bge_S_()
				: base(null, null, null)
			{
			}
		}

		public class Bgt_S_ : CodeMatch
		{
			public Code.Bgt_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bgt_S_)base.Set(operand, name);
				}
			}

			public Bgt_S_()
				: base(null, null, null)
			{
			}
		}

		public class Ble_S_ : CodeMatch
		{
			public Code.Ble_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ble_S_)base.Set(operand, name);
				}
			}

			public Ble_S_()
				: base(null, null, null)
			{
			}
		}

		public class Blt_S_ : CodeMatch
		{
			public Code.Blt_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Blt_S_)base.Set(operand, name);
				}
			}

			public Blt_S_()
				: base(null, null, null)
			{
			}
		}

		public class Bne_Un_S_ : CodeMatch
		{
			public Code.Bne_Un_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bne_Un_S_)base.Set(operand, name);
				}
			}

			public Bne_Un_S_()
				: base(null, null, null)
			{
			}
		}

		public class Bge_Un_S_ : CodeMatch
		{
			public Code.Bge_Un_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bge_Un_S_)base.Set(operand, name);
				}
			}

			public Bge_Un_S_()
				: base(null, null, null)
			{
			}
		}

		public class Bgt_Un_S_ : CodeMatch
		{
			public Code.Bgt_Un_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bgt_Un_S_)base.Set(operand, name);
				}
			}

			public Bgt_Un_S_()
				: base(null, null, null)
			{
			}
		}

		public class Ble_Un_S_ : CodeMatch
		{
			public Code.Ble_Un_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ble_Un_S_)base.Set(operand, name);
				}
			}

			public Ble_Un_S_()
				: base(null, null, null)
			{
			}
		}

		public class Blt_Un_S_ : CodeMatch
		{
			public Code.Blt_Un_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Blt_Un_S_)base.Set(operand, name);
				}
			}

			public Blt_Un_S_()
				: base(null, null, null)
			{
			}
		}

		public class Br_ : CodeMatch
		{
			public Code.Br_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Br_)base.Set(operand, name);
				}
			}

			public Br_()
				: base(null, null, null)
			{
			}
		}

		public class Brfalse_ : CodeMatch
		{
			public Code.Brfalse_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Brfalse_)base.Set(operand, name);
				}
			}

			public Brfalse_()
				: base(null, null, null)
			{
			}
		}

		public class Brtrue_ : CodeMatch
		{
			public Code.Brtrue_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Brtrue_)base.Set(operand, name);
				}
			}

			public Brtrue_()
				: base(null, null, null)
			{
			}
		}

		public class Beq_ : CodeMatch
		{
			public Code.Beq_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Beq_)base.Set(operand, name);
				}
			}

			public Beq_()
				: base(null, null, null)
			{
			}
		}

		public class Bge_ : CodeMatch
		{
			public Code.Bge_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bge_)base.Set(operand, name);
				}
			}

			public Bge_()
				: base(null, null, null)
			{
			}
		}

		public class Bgt_ : CodeMatch
		{
			public Code.Bgt_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bgt_)base.Set(operand, name);
				}
			}

			public Bgt_()
				: base(null, null, null)
			{
			}
		}

		public class Ble_ : CodeMatch
		{
			public Code.Ble_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ble_)base.Set(operand, name);
				}
			}

			public Ble_()
				: base(null, null, null)
			{
			}
		}

		public class Blt_ : CodeMatch
		{
			public Code.Blt_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Blt_)base.Set(operand, name);
				}
			}

			public Blt_()
				: base(null, null, null)
			{
			}
		}

		public class Bne_Un_ : CodeMatch
		{
			public Code.Bne_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bne_Un_)base.Set(operand, name);
				}
			}

			public Bne_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Bge_Un_ : CodeMatch
		{
			public Code.Bge_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bge_Un_)base.Set(operand, name);
				}
			}

			public Bge_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Bgt_Un_ : CodeMatch
		{
			public Code.Bgt_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Bgt_Un_)base.Set(operand, name);
				}
			}

			public Bgt_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Ble_Un_ : CodeMatch
		{
			public Code.Ble_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ble_Un_)base.Set(operand, name);
				}
			}

			public Ble_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Blt_Un_ : CodeMatch
		{
			public Code.Blt_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Blt_Un_)base.Set(operand, name);
				}
			}

			public Blt_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Switch_ : CodeMatch
		{
			public Code.Switch_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Switch_)base.Set(operand, name);
				}
			}

			public Switch_()
				: base(null, null, null)
			{
			}
		}

		public class Ldind_I1_ : CodeMatch
		{
			public Code.Ldind_I1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_I1_)base.Set(operand, name);
				}
			}

			public Ldind_I1_()
				: base(null, null, null)
			{
			}
		}

		public class Ldind_U1_ : CodeMatch
		{
			public Code.Ldind_U1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_U1_)base.Set(operand, name);
				}
			}

			public Ldind_U1_()
				: base(null, null, null)
			{
			}
		}

		public class Ldind_I2_ : CodeMatch
		{
			public Code.Ldind_I2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_I2_)base.Set(operand, name);
				}
			}

			public Ldind_I2_()
				: base(null, null, null)
			{
			}
		}

		public class Ldind_U2_ : CodeMatch
		{
			public Code.Ldind_U2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_U2_)base.Set(operand, name);
				}
			}

			public Ldind_U2_()
				: base(null, null, null)
			{
			}
		}

		public class Ldind_I4_ : CodeMatch
		{
			public Code.Ldind_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_I4_)base.Set(operand, name);
				}
			}

			public Ldind_I4_()
				: base(null, null, null)
			{
			}
		}

		public class Ldind_U4_ : CodeMatch
		{
			public Code.Ldind_U4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_U4_)base.Set(operand, name);
				}
			}

			public Ldind_U4_()
				: base(null, null, null)
			{
			}
		}

		public class Ldind_I8_ : CodeMatch
		{
			public Code.Ldind_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_I8_)base.Set(operand, name);
				}
			}

			public Ldind_I8_()
				: base(null, null, null)
			{
			}
		}

		public class Ldind_I_ : CodeMatch
		{
			public Code.Ldind_I_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_I_)base.Set(operand, name);
				}
			}

			public Ldind_I_()
				: base(null, null, null)
			{
			}
		}

		public class Ldind_R4_ : CodeMatch
		{
			public Code.Ldind_R4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_R4_)base.Set(operand, name);
				}
			}

			public Ldind_R4_()
				: base(null, null, null)
			{
			}
		}

		public class Ldind_R8_ : CodeMatch
		{
			public Code.Ldind_R8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_R8_)base.Set(operand, name);
				}
			}

			public Ldind_R8_()
				: base(null, null, null)
			{
			}
		}

		public class Ldind_Ref_ : CodeMatch
		{
			public Code.Ldind_Ref_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldind_Ref_)base.Set(operand, name);
				}
			}

			public Ldind_Ref_()
				: base(null, null, null)
			{
			}
		}

		public class Stind_Ref_ : CodeMatch
		{
			public Code.Stind_Ref_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_Ref_)base.Set(operand, name);
				}
			}

			public Stind_Ref_()
				: base(null, null, null)
			{
			}
		}

		public class Stind_I1_ : CodeMatch
		{
			public Code.Stind_I1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_I1_)base.Set(operand, name);
				}
			}

			public Stind_I1_()
				: base(null, null, null)
			{
			}
		}

		public class Stind_I2_ : CodeMatch
		{
			public Code.Stind_I2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_I2_)base.Set(operand, name);
				}
			}

			public Stind_I2_()
				: base(null, null, null)
			{
			}
		}

		public class Stind_I4_ : CodeMatch
		{
			public Code.Stind_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_I4_)base.Set(operand, name);
				}
			}

			public Stind_I4_()
				: base(null, null, null)
			{
			}
		}

		public class Stind_I8_ : CodeMatch
		{
			public Code.Stind_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_I8_)base.Set(operand, name);
				}
			}

			public Stind_I8_()
				: base(null, null, null)
			{
			}
		}

		public class Stind_R4_ : CodeMatch
		{
			public Code.Stind_R4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_R4_)base.Set(operand, name);
				}
			}

			public Stind_R4_()
				: base(null, null, null)
			{
			}
		}

		public class Stind_R8_ : CodeMatch
		{
			public Code.Stind_R8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_R8_)base.Set(operand, name);
				}
			}

			public Stind_R8_()
				: base(null, null, null)
			{
			}
		}

		public class Add_ : CodeMatch
		{
			public Code.Add_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Add_)base.Set(operand, name);
				}
			}

			public Add_()
				: base(null, null, null)
			{
			}
		}

		public class Sub_ : CodeMatch
		{
			public Code.Sub_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Sub_)base.Set(operand, name);
				}
			}

			public Sub_()
				: base(null, null, null)
			{
			}
		}

		public class Mul_ : CodeMatch
		{
			public Code.Mul_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Mul_)base.Set(operand, name);
				}
			}

			public Mul_()
				: base(null, null, null)
			{
			}
		}

		public class Div_ : CodeMatch
		{
			public Code.Div_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Div_)base.Set(operand, name);
				}
			}

			public Div_()
				: base(null, null, null)
			{
			}
		}

		public class Div_Un_ : CodeMatch
		{
			public Code.Div_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Div_Un_)base.Set(operand, name);
				}
			}

			public Div_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Rem_ : CodeMatch
		{
			public Code.Rem_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Rem_)base.Set(operand, name);
				}
			}

			public Rem_()
				: base(null, null, null)
			{
			}
		}

		public class Rem_Un_ : CodeMatch
		{
			public Code.Rem_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Rem_Un_)base.Set(operand, name);
				}
			}

			public Rem_Un_()
				: base(null, null, null)
			{
			}
		}

		public class And_ : CodeMatch
		{
			public Code.And_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.And_)base.Set(operand, name);
				}
			}

			public And_()
				: base(null, null, null)
			{
			}
		}

		public class Or_ : CodeMatch
		{
			public Code.Or_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Or_)base.Set(operand, name);
				}
			}

			public Or_()
				: base(null, null, null)
			{
			}
		}

		public class Xor_ : CodeMatch
		{
			public Code.Xor_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Xor_)base.Set(operand, name);
				}
			}

			public Xor_()
				: base(null, null, null)
			{
			}
		}

		public class Shl_ : CodeMatch
		{
			public Code.Shl_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Shl_)base.Set(operand, name);
				}
			}

			public Shl_()
				: base(null, null, null)
			{
			}
		}

		public class Shr_ : CodeMatch
		{
			public Code.Shr_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Shr_)base.Set(operand, name);
				}
			}

			public Shr_()
				: base(null, null, null)
			{
			}
		}

		public class Shr_Un_ : CodeMatch
		{
			public Code.Shr_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Shr_Un_)base.Set(operand, name);
				}
			}

			public Shr_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Neg_ : CodeMatch
		{
			public Code.Neg_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Neg_)base.Set(operand, name);
				}
			}

			public Neg_()
				: base(null, null, null)
			{
			}
		}

		public class Not_ : CodeMatch
		{
			public Code.Not_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Not_)base.Set(operand, name);
				}
			}

			public Not_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_I1_ : CodeMatch
		{
			public Code.Conv_I1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_I1_)base.Set(operand, name);
				}
			}

			public Conv_I1_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_I2_ : CodeMatch
		{
			public Code.Conv_I2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_I2_)base.Set(operand, name);
				}
			}

			public Conv_I2_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_I4_ : CodeMatch
		{
			public Code.Conv_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_I4_)base.Set(operand, name);
				}
			}

			public Conv_I4_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_I8_ : CodeMatch
		{
			public Code.Conv_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_I8_)base.Set(operand, name);
				}
			}

			public Conv_I8_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_R4_ : CodeMatch
		{
			public Code.Conv_R4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_R4_)base.Set(operand, name);
				}
			}

			public Conv_R4_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_R8_ : CodeMatch
		{
			public Code.Conv_R8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_R8_)base.Set(operand, name);
				}
			}

			public Conv_R8_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_U4_ : CodeMatch
		{
			public Code.Conv_U4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_U4_)base.Set(operand, name);
				}
			}

			public Conv_U4_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_U8_ : CodeMatch
		{
			public Code.Conv_U8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_U8_)base.Set(operand, name);
				}
			}

			public Conv_U8_()
				: base(null, null, null)
			{
			}
		}

		public class Callvirt_ : CodeMatch
		{
			public Code.Callvirt_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Callvirt_)base.Set(operand, name);
				}
			}

			public Callvirt_()
				: base(null, null, null)
			{
			}
		}

		public class Cpobj_ : CodeMatch
		{
			public Code.Cpobj_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Cpobj_)base.Set(operand, name);
				}
			}

			public Cpobj_()
				: base(null, null, null)
			{
			}
		}

		public class Ldobj_ : CodeMatch
		{
			public Code.Ldobj_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldobj_)base.Set(operand, name);
				}
			}

			public Ldobj_()
				: base(null, null, null)
			{
			}
		}

		public class Ldstr_ : CodeMatch
		{
			public Code.Ldstr_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldstr_)base.Set(operand, name);
				}
			}

			public Ldstr_()
				: base(null, null, null)
			{
			}
		}

		public class Newobj_ : CodeMatch
		{
			public Code.Newobj_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Newobj_)base.Set(operand, name);
				}
			}

			public Newobj_()
				: base(null, null, null)
			{
			}
		}

		public class Castclass_ : CodeMatch
		{
			public Code.Castclass_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Castclass_)base.Set(operand, name);
				}
			}

			public Castclass_()
				: base(null, null, null)
			{
			}
		}

		public class Isinst_ : CodeMatch
		{
			public Code.Isinst_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Isinst_)base.Set(operand, name);
				}
			}

			public Isinst_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_R_Un_ : CodeMatch
		{
			public Code.Conv_R_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_R_Un_)base.Set(operand, name);
				}
			}

			public Conv_R_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Unbox_ : CodeMatch
		{
			public Code.Unbox_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Unbox_)base.Set(operand, name);
				}
			}

			public Unbox_()
				: base(null, null, null)
			{
			}
		}

		public class Throw_ : CodeMatch
		{
			public Code.Throw_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Throw_)base.Set(operand, name);
				}
			}

			public Throw_()
				: base(null, null, null)
			{
			}
		}

		public class Ldfld_ : CodeMatch
		{
			public Code.Ldfld_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldfld_)base.Set(operand, name);
				}
			}

			public Ldfld_()
				: base(null, null, null)
			{
			}
		}

		public class Ldflda_ : CodeMatch
		{
			public Code.Ldflda_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldflda_)base.Set(operand, name);
				}
			}

			public Ldflda_()
				: base(null, null, null)
			{
			}
		}

		public class Stfld_ : CodeMatch
		{
			public Code.Stfld_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stfld_)base.Set(operand, name);
				}
			}

			public Stfld_()
				: base(null, null, null)
			{
			}
		}

		public class Ldsfld_ : CodeMatch
		{
			public Code.Ldsfld_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldsfld_)base.Set(operand, name);
				}
			}

			public Ldsfld_()
				: base(null, null, null)
			{
			}
		}

		public class Ldsflda_ : CodeMatch
		{
			public Code.Ldsflda_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldsflda_)base.Set(operand, name);
				}
			}

			public Ldsflda_()
				: base(null, null, null)
			{
			}
		}

		public class Stsfld_ : CodeMatch
		{
			public Code.Stsfld_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stsfld_)base.Set(operand, name);
				}
			}

			public Stsfld_()
				: base(null, null, null)
			{
			}
		}

		public class Stobj_ : CodeMatch
		{
			public Code.Stobj_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stobj_)base.Set(operand, name);
				}
			}

			public Stobj_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_I1_Un_ : CodeMatch
		{
			public Code.Conv_Ovf_I1_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I1_Un_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_I1_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_I2_Un_ : CodeMatch
		{
			public Code.Conv_Ovf_I2_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I2_Un_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_I2_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_I4_Un_ : CodeMatch
		{
			public Code.Conv_Ovf_I4_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I4_Un_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_I4_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_I8_Un_ : CodeMatch
		{
			public Code.Conv_Ovf_I8_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I8_Un_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_I8_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_U1_Un_ : CodeMatch
		{
			public Code.Conv_Ovf_U1_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U1_Un_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_U1_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_U2_Un_ : CodeMatch
		{
			public Code.Conv_Ovf_U2_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U2_Un_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_U2_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_U4_Un_ : CodeMatch
		{
			public Code.Conv_Ovf_U4_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U4_Un_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_U4_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_U8_Un_ : CodeMatch
		{
			public Code.Conv_Ovf_U8_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U8_Un_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_U8_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_I_Un_ : CodeMatch
		{
			public Code.Conv_Ovf_I_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I_Un_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_I_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_U_Un_ : CodeMatch
		{
			public Code.Conv_Ovf_U_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U_Un_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_U_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Box_ : CodeMatch
		{
			public Code.Box_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Box_)base.Set(operand, name);
				}
			}

			public Box_()
				: base(null, null, null)
			{
			}
		}

		public class Newarr_ : CodeMatch
		{
			public Code.Newarr_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Newarr_)base.Set(operand, name);
				}
			}

			public Newarr_()
				: base(null, null, null)
			{
			}
		}

		public class Ldlen_ : CodeMatch
		{
			public Code.Ldlen_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldlen_)base.Set(operand, name);
				}
			}

			public Ldlen_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelema_ : CodeMatch
		{
			public Code.Ldelema_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelema_)base.Set(operand, name);
				}
			}

			public Ldelema_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelem_I1_ : CodeMatch
		{
			public Code.Ldelem_I1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_I1_)base.Set(operand, name);
				}
			}

			public Ldelem_I1_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelem_U1_ : CodeMatch
		{
			public Code.Ldelem_U1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_U1_)base.Set(operand, name);
				}
			}

			public Ldelem_U1_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelem_I2_ : CodeMatch
		{
			public Code.Ldelem_I2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_I2_)base.Set(operand, name);
				}
			}

			public Ldelem_I2_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelem_U2_ : CodeMatch
		{
			public Code.Ldelem_U2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_U2_)base.Set(operand, name);
				}
			}

			public Ldelem_U2_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelem_I4_ : CodeMatch
		{
			public Code.Ldelem_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_I4_)base.Set(operand, name);
				}
			}

			public Ldelem_I4_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelem_U4_ : CodeMatch
		{
			public Code.Ldelem_U4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_U4_)base.Set(operand, name);
				}
			}

			public Ldelem_U4_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelem_I8_ : CodeMatch
		{
			public Code.Ldelem_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_I8_)base.Set(operand, name);
				}
			}

			public Ldelem_I8_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelem_I_ : CodeMatch
		{
			public Code.Ldelem_I_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_I_)base.Set(operand, name);
				}
			}

			public Ldelem_I_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelem_R4_ : CodeMatch
		{
			public Code.Ldelem_R4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_R4_)base.Set(operand, name);
				}
			}

			public Ldelem_R4_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelem_R8_ : CodeMatch
		{
			public Code.Ldelem_R8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_R8_)base.Set(operand, name);
				}
			}

			public Ldelem_R8_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelem_Ref_ : CodeMatch
		{
			public Code.Ldelem_Ref_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_Ref_)base.Set(operand, name);
				}
			}

			public Ldelem_Ref_()
				: base(null, null, null)
			{
			}
		}

		public class Stelem_I_ : CodeMatch
		{
			public Code.Stelem_I_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_I_)base.Set(operand, name);
				}
			}

			public Stelem_I_()
				: base(null, null, null)
			{
			}
		}

		public class Stelem_I1_ : CodeMatch
		{
			public Code.Stelem_I1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_I1_)base.Set(operand, name);
				}
			}

			public Stelem_I1_()
				: base(null, null, null)
			{
			}
		}

		public class Stelem_I2_ : CodeMatch
		{
			public Code.Stelem_I2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_I2_)base.Set(operand, name);
				}
			}

			public Stelem_I2_()
				: base(null, null, null)
			{
			}
		}

		public class Stelem_I4_ : CodeMatch
		{
			public Code.Stelem_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_I4_)base.Set(operand, name);
				}
			}

			public Stelem_I4_()
				: base(null, null, null)
			{
			}
		}

		public class Stelem_I8_ : CodeMatch
		{
			public Code.Stelem_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_I8_)base.Set(operand, name);
				}
			}

			public Stelem_I8_()
				: base(null, null, null)
			{
			}
		}

		public class Stelem_R4_ : CodeMatch
		{
			public Code.Stelem_R4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_R4_)base.Set(operand, name);
				}
			}

			public Stelem_R4_()
				: base(null, null, null)
			{
			}
		}

		public class Stelem_R8_ : CodeMatch
		{
			public Code.Stelem_R8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_R8_)base.Set(operand, name);
				}
			}

			public Stelem_R8_()
				: base(null, null, null)
			{
			}
		}

		public class Stelem_Ref_ : CodeMatch
		{
			public Code.Stelem_Ref_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_Ref_)base.Set(operand, name);
				}
			}

			public Stelem_Ref_()
				: base(null, null, null)
			{
			}
		}

		public class Ldelem_ : CodeMatch
		{
			public Code.Ldelem_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldelem_)base.Set(operand, name);
				}
			}

			public Ldelem_()
				: base(null, null, null)
			{
			}
		}

		public class Stelem_ : CodeMatch
		{
			public Code.Stelem_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stelem_)base.Set(operand, name);
				}
			}

			public Stelem_()
				: base(null, null, null)
			{
			}
		}

		public class Unbox_Any_ : CodeMatch
		{
			public Code.Unbox_Any_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Unbox_Any_)base.Set(operand, name);
				}
			}

			public Unbox_Any_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_I1_ : CodeMatch
		{
			public Code.Conv_Ovf_I1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I1_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_I1_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_U1_ : CodeMatch
		{
			public Code.Conv_Ovf_U1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U1_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_U1_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_I2_ : CodeMatch
		{
			public Code.Conv_Ovf_I2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I2_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_I2_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_U2_ : CodeMatch
		{
			public Code.Conv_Ovf_U2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U2_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_U2_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_I4_ : CodeMatch
		{
			public Code.Conv_Ovf_I4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I4_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_I4_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_U4_ : CodeMatch
		{
			public Code.Conv_Ovf_U4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U4_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_U4_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_I8_ : CodeMatch
		{
			public Code.Conv_Ovf_I8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I8_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_I8_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_U8_ : CodeMatch
		{
			public Code.Conv_Ovf_U8_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U8_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_U8_()
				: base(null, null, null)
			{
			}
		}

		public class Refanyval_ : CodeMatch
		{
			public Code.Refanyval_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Refanyval_)base.Set(operand, name);
				}
			}

			public Refanyval_()
				: base(null, null, null)
			{
			}
		}

		public class Ckfinite_ : CodeMatch
		{
			public Code.Ckfinite_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ckfinite_)base.Set(operand, name);
				}
			}

			public Ckfinite_()
				: base(null, null, null)
			{
			}
		}

		public class Mkrefany_ : CodeMatch
		{
			public Code.Mkrefany_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Mkrefany_)base.Set(operand, name);
				}
			}

			public Mkrefany_()
				: base(null, null, null)
			{
			}
		}

		public class Ldtoken_ : CodeMatch
		{
			public Code.Ldtoken_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldtoken_)base.Set(operand, name);
				}
			}

			public Ldtoken_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_U2_ : CodeMatch
		{
			public Code.Conv_U2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_U2_)base.Set(operand, name);
				}
			}

			public Conv_U2_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_U1_ : CodeMatch
		{
			public Code.Conv_U1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_U1_)base.Set(operand, name);
				}
			}

			public Conv_U1_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_I_ : CodeMatch
		{
			public Code.Conv_I_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_I_)base.Set(operand, name);
				}
			}

			public Conv_I_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_I_ : CodeMatch
		{
			public Code.Conv_Ovf_I_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_I_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_I_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_Ovf_U_ : CodeMatch
		{
			public Code.Conv_Ovf_U_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_Ovf_U_)base.Set(operand, name);
				}
			}

			public Conv_Ovf_U_()
				: base(null, null, null)
			{
			}
		}

		public class Add_Ovf_ : CodeMatch
		{
			public Code.Add_Ovf_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Add_Ovf_)base.Set(operand, name);
				}
			}

			public Add_Ovf_()
				: base(null, null, null)
			{
			}
		}

		public class Add_Ovf_Un_ : CodeMatch
		{
			public Code.Add_Ovf_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Add_Ovf_Un_)base.Set(operand, name);
				}
			}

			public Add_Ovf_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Mul_Ovf_ : CodeMatch
		{
			public Code.Mul_Ovf_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Mul_Ovf_)base.Set(operand, name);
				}
			}

			public Mul_Ovf_()
				: base(null, null, null)
			{
			}
		}

		public class Mul_Ovf_Un_ : CodeMatch
		{
			public Code.Mul_Ovf_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Mul_Ovf_Un_)base.Set(operand, name);
				}
			}

			public Mul_Ovf_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Sub_Ovf_ : CodeMatch
		{
			public Code.Sub_Ovf_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Sub_Ovf_)base.Set(operand, name);
				}
			}

			public Sub_Ovf_()
				: base(null, null, null)
			{
			}
		}

		public class Sub_Ovf_Un_ : CodeMatch
		{
			public Code.Sub_Ovf_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Sub_Ovf_Un_)base.Set(operand, name);
				}
			}

			public Sub_Ovf_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Endfinally_ : CodeMatch
		{
			public Code.Endfinally_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Endfinally_)base.Set(operand, name);
				}
			}

			public Endfinally_()
				: base(null, null, null)
			{
			}
		}

		public class Leave_ : CodeMatch
		{
			public Code.Leave_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Leave_)base.Set(operand, name);
				}
			}

			public Leave_()
				: base(null, null, null)
			{
			}
		}

		public class Leave_S_ : CodeMatch
		{
			public Code.Leave_S_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Leave_S_)base.Set(operand, name);
				}
			}

			public Leave_S_()
				: base(null, null, null)
			{
			}
		}

		public class Stind_I_ : CodeMatch
		{
			public Code.Stind_I_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stind_I_)base.Set(operand, name);
				}
			}

			public Stind_I_()
				: base(null, null, null)
			{
			}
		}

		public class Conv_U_ : CodeMatch
		{
			public Code.Conv_U_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Conv_U_)base.Set(operand, name);
				}
			}

			public Conv_U_()
				: base(null, null, null)
			{
			}
		}

		public class Prefix7_ : CodeMatch
		{
			public Code.Prefix7_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix7_)base.Set(operand, name);
				}
			}

			public Prefix7_()
				: base(null, null, null)
			{
			}
		}

		public class Prefix6_ : CodeMatch
		{
			public Code.Prefix6_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix6_)base.Set(operand, name);
				}
			}

			public Prefix6_()
				: base(null, null, null)
			{
			}
		}

		public class Prefix5_ : CodeMatch
		{
			public Code.Prefix5_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix5_)base.Set(operand, name);
				}
			}

			public Prefix5_()
				: base(null, null, null)
			{
			}
		}

		public class Prefix4_ : CodeMatch
		{
			public Code.Prefix4_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix4_)base.Set(operand, name);
				}
			}

			public Prefix4_()
				: base(null, null, null)
			{
			}
		}

		public class Prefix3_ : CodeMatch
		{
			public Code.Prefix3_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix3_)base.Set(operand, name);
				}
			}

			public Prefix3_()
				: base(null, null, null)
			{
			}
		}

		public class Prefix2_ : CodeMatch
		{
			public Code.Prefix2_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix2_)base.Set(operand, name);
				}
			}

			public Prefix2_()
				: base(null, null, null)
			{
			}
		}

		public class Prefix1_ : CodeMatch
		{
			public Code.Prefix1_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefix1_)base.Set(operand, name);
				}
			}

			public Prefix1_()
				: base(null, null, null)
			{
			}
		}

		public class Prefixref_ : CodeMatch
		{
			public Code.Prefixref_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Prefixref_)base.Set(operand, name);
				}
			}

			public Prefixref_()
				: base(null, null, null)
			{
			}
		}

		public class Arglist_ : CodeMatch
		{
			public Code.Arglist_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Arglist_)base.Set(operand, name);
				}
			}

			public Arglist_()
				: base(null, null, null)
			{
			}
		}

		public class Ceq_ : CodeMatch
		{
			public Code.Ceq_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ceq_)base.Set(operand, name);
				}
			}

			public Ceq_()
				: base(null, null, null)
			{
			}
		}

		public class Cgt_ : CodeMatch
		{
			public Code.Cgt_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Cgt_)base.Set(operand, name);
				}
			}

			public Cgt_()
				: base(null, null, null)
			{
			}
		}

		public class Cgt_Un_ : CodeMatch
		{
			public Code.Cgt_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Cgt_Un_)base.Set(operand, name);
				}
			}

			public Cgt_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Clt_ : CodeMatch
		{
			public Code.Clt_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Clt_)base.Set(operand, name);
				}
			}

			public Clt_()
				: base(null, null, null)
			{
			}
		}

		public class Clt_Un_ : CodeMatch
		{
			public Code.Clt_Un_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Clt_Un_)base.Set(operand, name);
				}
			}

			public Clt_Un_()
				: base(null, null, null)
			{
			}
		}

		public class Ldftn_ : CodeMatch
		{
			public Code.Ldftn_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldftn_)base.Set(operand, name);
				}
			}

			public Ldftn_()
				: base(null, null, null)
			{
			}
		}

		public class Ldvirtftn_ : CodeMatch
		{
			public Code.Ldvirtftn_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldvirtftn_)base.Set(operand, name);
				}
			}

			public Ldvirtftn_()
				: base(null, null, null)
			{
			}
		}

		public class Ldarg_ : CodeMatch
		{
			public Code.Ldarg_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarg_)base.Set(operand, name);
				}
			}

			public Ldarg_()
				: base(null, null, null)
			{
			}
		}

		public class Ldarga_ : CodeMatch
		{
			public Code.Ldarga_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldarga_)base.Set(operand, name);
				}
			}

			public Ldarga_()
				: base(null, null, null)
			{
			}
		}

		public class Starg_ : CodeMatch
		{
			public Code.Starg_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Starg_)base.Set(operand, name);
				}
			}

			public Starg_()
				: base(null, null, null)
			{
			}
		}

		public class Ldloc_ : CodeMatch
		{
			public Code.Ldloc_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloc_)base.Set(operand, name);
				}
			}

			public Ldloc_()
				: base(null, null, null)
			{
			}
		}

		public class Ldloca_ : CodeMatch
		{
			public Code.Ldloca_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Ldloca_)base.Set(operand, name);
				}
			}

			public Ldloca_()
				: base(null, null, null)
			{
			}
		}

		public class Stloc_ : CodeMatch
		{
			public Code.Stloc_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Stloc_)base.Set(operand, name);
				}
			}

			public Stloc_()
				: base(null, null, null)
			{
			}
		}

		public class Localloc_ : CodeMatch
		{
			public Code.Localloc_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Localloc_)base.Set(operand, name);
				}
			}

			public Localloc_()
				: base(null, null, null)
			{
			}
		}

		public class Endfilter_ : CodeMatch
		{
			public Code.Endfilter_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Endfilter_)base.Set(operand, name);
				}
			}

			public Endfilter_()
				: base(null, null, null)
			{
			}
		}

		public class Unaligned_ : CodeMatch
		{
			public Code.Unaligned_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Unaligned_)base.Set(operand, name);
				}
			}

			public Unaligned_()
				: base(null, null, null)
			{
			}
		}

		public class Volatile_ : CodeMatch
		{
			public Code.Volatile_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Volatile_)base.Set(operand, name);
				}
			}

			public Volatile_()
				: base(null, null, null)
			{
			}
		}

		public class Tailcall_ : CodeMatch
		{
			public Code.Tailcall_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Tailcall_)base.Set(operand, name);
				}
			}

			public Tailcall_()
				: base(null, null, null)
			{
			}
		}

		public class Initobj_ : CodeMatch
		{
			public Code.Initobj_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Initobj_)base.Set(operand, name);
				}
			}

			public Initobj_()
				: base(null, null, null)
			{
			}
		}

		public class Constrained_ : CodeMatch
		{
			public Code.Constrained_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Constrained_)base.Set(operand, name);
				}
			}

			public Constrained_()
				: base(null, null, null)
			{
			}
		}

		public class Cpblk_ : CodeMatch
		{
			public Code.Cpblk_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Cpblk_)base.Set(operand, name);
				}
			}

			public Cpblk_()
				: base(null, null, null)
			{
			}
		}

		public class Initblk_ : CodeMatch
		{
			public Code.Initblk_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Initblk_)base.Set(operand, name);
				}
			}

			public Initblk_()
				: base(null, null, null)
			{
			}
		}

		public class Rethrow_ : CodeMatch
		{
			public Code.Rethrow_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Rethrow_)base.Set(operand, name);
				}
			}

			public Rethrow_()
				: base(null, null, null)
			{
			}
		}

		public class Sizeof_ : CodeMatch
		{
			public Code.Sizeof_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Sizeof_)base.Set(operand, name);
				}
			}

			public Sizeof_()
				: base(null, null, null)
			{
			}
		}

		public class Refanytype_ : CodeMatch
		{
			public Code.Refanytype_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Refanytype_)base.Set(operand, name);
				}
			}

			public Refanytype_()
				: base(null, null, null)
			{
			}
		}

		public class Readonly_ : CodeMatch
		{
			public Code.Readonly_ this[object operand = null, string name = null]
			{
				get
				{
					return (Code.Readonly_)base.Set(operand, name);
				}
			}

			public Readonly_()
				: base(null, null, null)
			{
			}
		}
	}
}
