using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.SourceGen.Attributes;
using MonoMod.Utils;

namespace MonoMod.Cil
{
	[NullableContext(1)]
	[Nullable(0)]
	[EmitILOverloads("ILOpcodes.txt", "ILMatcher")]
	internal static class ILPatternMatchingExt
	{
		private static bool IsEquivalent(int l, int r)
		{
			return l == r;
		}

		private static bool IsEquivalent(int l, uint r)
		{
			return l == (int)r;
		}

		private static bool IsEquivalent(long l, long r)
		{
			return l == r;
		}

		private static bool IsEquivalent(long l, ulong r)
		{
			return l == (long)r;
		}

		private static bool IsEquivalent(float l, float r)
		{
			return l == r;
		}

		private static bool IsEquivalent(double l, double r)
		{
			return l == r;
		}

		private static bool IsEquivalent(string l, string r)
		{
			return l == r;
		}

		private static bool IsEquivalent(ILLabel l, ILLabel r)
		{
			return l == r;
		}

		private static bool IsEquivalent(ILLabel l, Instruction r)
		{
			return ILPatternMatchingExt.IsEquivalent(l.Target, r);
		}

		[NullableContext(2)]
		private static bool IsEquivalent(Instruction l, Instruction r)
		{
			return l == r;
		}

		private static bool IsEquivalent(TypeReference l, TypeReference r)
		{
			return l == r;
		}

		private static bool IsEquivalent(TypeReference l, Type r)
		{
			return l.Is(r);
		}

		private static bool IsEquivalent(MethodReference l, MethodReference r)
		{
			return l == r;
		}

		private static bool IsEquivalent(MethodReference l, MethodBase r)
		{
			return l.Is(r);
		}

		private static bool IsEquivalent(MethodReference l, Type type, string name)
		{
			return l.DeclaringType.Is(type) && l.Name == name;
		}

		private static bool IsEquivalent(FieldReference l, FieldReference r)
		{
			return l == r;
		}

		private static bool IsEquivalent(FieldReference l, FieldInfo r)
		{
			return l.Is(r);
		}

		private static bool IsEquivalent(FieldReference l, Type type, string name)
		{
			return l.DeclaringType.Is(type) && l.Name == name;
		}

		private static bool IsEquivalent(ILLabel[] l, ILLabel[] r)
		{
			return l == r || l.SequenceEqual<ILLabel>(r);
		}

		private static bool IsEquivalent(ILLabel[] l, Instruction[] r)
		{
			if (l.Length != r.Length)
			{
				return false;
			}
			for (int i = 0; i < l.Length; i++)
			{
				if (!ILPatternMatchingExt.IsEquivalent(l[i].Target, r[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsEquivalent(IMethodSignature l, IMethodSignature r)
		{
			return l == r || (l.CallingConvention == r.CallingConvention && l.HasThis == r.HasThis && l.ExplicitThis == r.ExplicitThis && ILPatternMatchingExt.IsEquivalent(l.ReturnType, r.ReturnType) && ILPatternMatchingExt.CastParamsToRef(l).SequenceEqual<ParameterReference>(ILPatternMatchingExt.CastParamsToRef(r), ILPatternMatchingExt.ParameterRefEqualityComparer.Instance));
		}

		private static IEnumerable<ParameterReference> CastParamsToRef(IMethodSignature sig)
		{
			return sig.Parameters;
		}

		private static bool IsEquivalent(IMetadataTokenProvider l, IMetadataTokenProvider r)
		{
			return l == r || l.MetadataToken == r.MetadataToken;
		}

		private static bool IsEquivalent(IMetadataTokenProvider l, Type r)
		{
			TypeReference typeReference = l as TypeReference;
			return typeReference != null && ILPatternMatchingExt.IsEquivalent(typeReference, r);
		}

		private static bool IsEquivalent(IMetadataTokenProvider l, FieldInfo r)
		{
			FieldReference fieldReference = l as FieldReference;
			return fieldReference != null && ILPatternMatchingExt.IsEquivalent(fieldReference, r);
		}

		private static bool IsEquivalent(IMetadataTokenProvider l, MethodBase r)
		{
			MethodReference methodReference = l as MethodReference;
			return methodReference != null && ILPatternMatchingExt.IsEquivalent(methodReference, r);
		}

		public static bool Match(this Instruction instr, OpCode opcode)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == opcode;
		}

		public static bool Match<[Nullable(2)] T>(this Instruction instr, OpCode opcode, T value)
		{
			T t;
			if (instr.Match<T>(opcode, out t))
			{
				ref T ptr = ref t;
				T t2 = default(T);
				if (t2 == null)
				{
					t2 = t;
					ptr = ref t2;
					if (t2 == null)
					{
						return value == null;
					}
				}
				return ptr.Equals(value);
			}
			return false;
		}

		public static bool Match<[Nullable(2)] T>(this Instruction instr, OpCode opcode, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out T value)
		{
			Helpers.ThrowIfArgumentNull<Instruction>(instr, "instr");
			if (instr.OpCode == opcode)
			{
				object operand = instr.Operand;
				if (operand is T)
				{
					T t = (T)((object)operand);
					value = t;
					return true;
				}
			}
			value = default(T);
			return false;
		}

		[Obsolete("Leftover from legacy MonoMod, use MatchLeave instead")]
		public static bool MatchLeaveS(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchLeaveS(out illabel) && illabel == value;
		}

		[Obsolete("Leftover from legacy MonoMod, use MatchLeave instead")]
		public static bool MatchLeaveS(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			Helpers.ThrowIfArgumentNull<Instruction>(instr, "instr");
			if (instr.OpCode == OpCodes.Leave_S)
			{
				value = (ILLabel)instr.Operand;
				return true;
			}
			value = null;
			return false;
		}

		public static bool MatchLdarg(this Instruction instr, out int value)
		{
			Helpers.ThrowIfArgumentNull<Instruction>(instr, "instr");
			if (instr.OpCode == OpCodes.Ldarg || instr.OpCode == OpCodes.Ldarg_S)
			{
				value = ((ParameterReference)instr.Operand).Index;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldarg_0)
			{
				value = 0;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldarg_1)
			{
				value = 1;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldarg_2)
			{
				value = 2;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldarg_3)
			{
				value = 3;
				return true;
			}
			value = 0;
			return false;
		}

		[NullableContext(2)]
		private static int ParameterToIndex(object obj)
		{
			int num2;
			if (obj != null)
			{
				if (obj is int)
				{
					int num = (int)obj;
					num2 = num;
				}
				else if (obj is short)
				{
					short num3 = (short)obj;
					num2 = (int)num3;
				}
				else if (obj is uint)
				{
					uint num4 = (uint)obj;
					num2 = (int)num4;
				}
				else if (obj is ushort)
				{
					ushort num5 = (ushort)obj;
					num2 = (int)num5;
				}
				else if (obj is byte)
				{
					byte b = (byte)obj;
					num2 = (int)b;
				}
				else if (obj is sbyte)
				{
					sbyte b2 = (sbyte)obj;
					num2 = (int)b2;
				}
				else
				{
					ParameterReference parameterReference = obj as ParameterReference;
					if (parameterReference == null)
					{
						throw new InvalidCastException();
					}
					num2 = parameterReference.Index;
				}
			}
			else
			{
				num2 = 0;
			}
			return num2;
		}

		public static bool MatchStarg(this Instruction instr, out int value)
		{
			Helpers.ThrowIfArgumentNull<Instruction>(instr, "instr");
			if (instr.OpCode == OpCodes.Starg || instr.OpCode == OpCodes.Starg_S)
			{
				value = ILPatternMatchingExt.ParameterToIndex(instr.Operand);
				return true;
			}
			value = 0;
			return false;
		}

		public static bool MatchLdarga(this Instruction instr, out int value)
		{
			Helpers.ThrowIfArgumentNull<Instruction>(instr, "instr");
			if (instr.OpCode == OpCodes.Ldarga || instr.OpCode == OpCodes.Ldarga_S)
			{
				value = ILPatternMatchingExt.ParameterToIndex(instr.Operand);
				return true;
			}
			value = 0;
			return false;
		}

		[NullableContext(2)]
		private static int VarToIndex(object obj)
		{
			int num2;
			if (obj != null)
			{
				if (obj is int)
				{
					int num = (int)obj;
					num2 = num;
				}
				else if (obj is short)
				{
					short num3 = (short)obj;
					num2 = (int)num3;
				}
				else if (obj is uint)
				{
					uint num4 = (uint)obj;
					num2 = (int)num4;
				}
				else if (obj is ushort)
				{
					ushort num5 = (ushort)obj;
					num2 = (int)num5;
				}
				else if (obj is byte)
				{
					byte b = (byte)obj;
					num2 = (int)b;
				}
				else if (obj is sbyte)
				{
					sbyte b2 = (sbyte)obj;
					num2 = (int)b2;
				}
				else
				{
					VariableReference variableReference = obj as VariableReference;
					if (variableReference == null)
					{
						throw new InvalidCastException();
					}
					num2 = variableReference.Index;
				}
			}
			else
			{
				num2 = 0;
			}
			return num2;
		}

		public static bool MatchLdloc(this Instruction instr, out int value)
		{
			Helpers.ThrowIfArgumentNull<Instruction>(instr, "instr");
			if (instr.OpCode == OpCodes.Ldloc || instr.OpCode == OpCodes.Ldloc_S)
			{
				value = ILPatternMatchingExt.VarToIndex(instr.Operand);
				return true;
			}
			if (instr.OpCode == OpCodes.Ldloc_0)
			{
				value = 0;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldloc_1)
			{
				value = 1;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldloc_2)
			{
				value = 2;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldloc_3)
			{
				value = 3;
				return true;
			}
			value = 0;
			return false;
		}

		public static bool MatchStloc(this Instruction instr, out int value)
		{
			Helpers.ThrowIfArgumentNull<Instruction>(instr, "instr");
			if (instr.OpCode == OpCodes.Stloc || instr.OpCode == OpCodes.Stloc_S)
			{
				value = ILPatternMatchingExt.VarToIndex(instr.Operand);
				return true;
			}
			if (instr.OpCode == OpCodes.Stloc_0)
			{
				value = 0;
				return true;
			}
			if (instr.OpCode == OpCodes.Stloc_1)
			{
				value = 1;
				return true;
			}
			if (instr.OpCode == OpCodes.Stloc_2)
			{
				value = 2;
				return true;
			}
			if (instr.OpCode == OpCodes.Stloc_3)
			{
				value = 3;
				return true;
			}
			value = 0;
			return false;
		}

		public static bool MatchLdloca(this Instruction instr, out int value)
		{
			Helpers.ThrowIfArgumentNull<Instruction>(instr, "instr");
			if (instr.OpCode == OpCodes.Ldloca || instr.OpCode == OpCodes.Ldloca_S)
			{
				value = ILPatternMatchingExt.VarToIndex(instr.Operand);
				return true;
			}
			value = 0;
			return false;
		}

		public static bool MatchLdcI4(this Instruction instr, out int value)
		{
			Helpers.ThrowIfArgumentNull<Instruction>(instr, "instr");
			if (instr.OpCode == OpCodes.Ldc_I4)
			{
				value = (int)instr.Operand;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldc_I4_S)
			{
				value = (int)((sbyte)instr.Operand);
				return true;
			}
			if (instr.OpCode == OpCodes.Ldc_I4_0)
			{
				value = 0;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldc_I4_1)
			{
				value = 1;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldc_I4_2)
			{
				value = 2;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldc_I4_3)
			{
				value = 3;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldc_I4_4)
			{
				value = 4;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldc_I4_5)
			{
				value = 5;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldc_I4_6)
			{
				value = 6;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldc_I4_7)
			{
				value = 7;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldc_I4_8)
			{
				value = 8;
				return true;
			}
			if (instr.OpCode == OpCodes.Ldc_I4_M1)
			{
				value = -1;
				return true;
			}
			value = 0;
			return false;
		}

		public static bool MatchCallOrCallvirt(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out MethodReference value)
		{
			Helpers.ThrowIfArgumentNull<Instruction>(instr, "instr");
			if (instr.OpCode == OpCodes.Call || instr.OpCode == OpCodes.Callvirt)
			{
				MethodReference methodReference = instr.Operand as MethodReference;
				if (methodReference != null)
				{
					value = methodReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchNewobj(this Instruction instr, Type type)
		{
			MethodReference methodReference;
			return instr.MatchNewobj(out methodReference) && methodReference.DeclaringType.Is(type);
		}

		public static bool MatchNewobj<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchNewobj(typeof(T));
		}

		public static bool MatchNewobj(this Instruction instr, string typeFullName)
		{
			MethodReference methodReference;
			return instr.MatchNewobj(out methodReference) && methodReference.DeclaringType.Is(typeFullName);
		}

		public static bool MatchAdd(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Add;
		}

		public static bool MatchAddOvf(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Add_Ovf;
		}

		public static bool MatchAddOvfUn(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Add_Ovf_Un;
		}

		public static bool MatchAnd(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.And;
		}

		public static bool MatchArglist(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Arglist;
		}

		public static bool MatchBeq(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Beq || instr.OpCode == OpCodes.Beq_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBeq(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBeq(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBeq(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBeq(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBge(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Bge || instr.OpCode == OpCodes.Bge_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBge(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBge(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBge(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBge(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBgeUn(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Bge_Un || instr.OpCode == OpCodes.Bge_Un_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBgeUn(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBgeUn(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBgeUn(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBgeUn(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBgt(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Bgt || instr.OpCode == OpCodes.Bgt_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBgt(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBgt(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBgt(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBgt(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBgtUn(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Bgt_Un || instr.OpCode == OpCodes.Bgt_Un_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBgtUn(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBgtUn(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBgtUn(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBgtUn(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBle(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ble || instr.OpCode == OpCodes.Ble_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBle(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBle(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBle(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBle(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBleUn(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ble_Un || instr.OpCode == OpCodes.Ble_Un_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBleUn(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBleUn(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBleUn(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBleUn(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBlt(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Blt || instr.OpCode == OpCodes.Blt_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBlt(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBlt(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBlt(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBlt(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBltUn(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Blt_Un || instr.OpCode == OpCodes.Blt_Un_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBltUn(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBltUn(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBltUn(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBltUn(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBneUn(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Bne_Un || instr.OpCode == OpCodes.Bne_Un_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBneUn(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBneUn(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBneUn(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBneUn(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBox(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Box)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBox(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchBox(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchBox(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchBox(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchBox<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchBox(typeof(T));
		}

		public static bool MatchBox(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchBox(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchBr(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Br || instr.OpCode == OpCodes.Br_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBr(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBr(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBr(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBr(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBreak(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Break;
		}

		public static bool MatchBrfalse(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Brfalse || instr.OpCode == OpCodes.Brfalse_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBrfalse(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBrfalse(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBrfalse(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBrfalse(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBrtrue(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Brtrue || instr.OpCode == OpCodes.Brtrue_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchBrtrue(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchBrtrue(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchBrtrue(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchBrtrue(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchCall(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out MethodReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Call)
			{
				MethodReference methodReference = instr.Operand as MethodReference;
				if (methodReference != null)
				{
					value = methodReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchCall(this Instruction instr, MethodReference value)
		{
			MethodReference methodReference;
			return instr.MatchCall(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchCall(this Instruction instr, MethodBase value)
		{
			MethodReference methodReference;
			return instr.MatchCall(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchCall(this Instruction instr, Type type, string name)
		{
			MethodReference methodReference;
			return instr.MatchCall(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, type, name);
		}

		public static bool MatchCall<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchCall(typeof(T), name);
		}

		public static bool MatchCall(this Instruction instr, string typeFullName, string name)
		{
			MethodReference methodReference;
			return instr.MatchCall(out methodReference) && methodReference.Is(typeFullName, name);
		}

		public static bool MatchCalli(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out IMethodSignature value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Calli)
			{
				IMethodSignature methodSignature = instr.Operand as IMethodSignature;
				if (methodSignature != null)
				{
					value = methodSignature;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchCalli(this Instruction instr, IMethodSignature value)
		{
			IMethodSignature methodSignature;
			return instr.MatchCalli(out methodSignature) && ILPatternMatchingExt.IsEquivalent(methodSignature, value);
		}

		public static bool MatchCallvirt(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out MethodReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Callvirt)
			{
				MethodReference methodReference = instr.Operand as MethodReference;
				if (methodReference != null)
				{
					value = methodReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchCallvirt(this Instruction instr, MethodReference value)
		{
			MethodReference methodReference;
			return instr.MatchCallvirt(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchCallvirt(this Instruction instr, MethodBase value)
		{
			MethodReference methodReference;
			return instr.MatchCallvirt(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchCallvirt(this Instruction instr, Type type, string name)
		{
			MethodReference methodReference;
			return instr.MatchCallvirt(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, type, name);
		}

		public static bool MatchCallvirt<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchCallvirt(typeof(T), name);
		}

		public static bool MatchCallvirt(this Instruction instr, string typeFullName, string name)
		{
			MethodReference methodReference;
			return instr.MatchCallvirt(out methodReference) && methodReference.Is(typeFullName, name);
		}

		public static bool MatchCallOrCallvirt(this Instruction instr, MethodReference value)
		{
			MethodReference methodReference;
			return instr.MatchCallOrCallvirt(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchCallOrCallvirt(this Instruction instr, MethodBase value)
		{
			MethodReference methodReference;
			return instr.MatchCallOrCallvirt(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchCallOrCallvirt(this Instruction instr, Type type, string name)
		{
			MethodReference methodReference;
			return instr.MatchCallOrCallvirt(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, type, name);
		}

		public static bool MatchCallOrCallvirt<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchCallOrCallvirt(typeof(T), name);
		}

		public static bool MatchCallOrCallvirt(this Instruction instr, string typeFullName, string name)
		{
			MethodReference methodReference;
			return instr.MatchCallOrCallvirt(out methodReference) && methodReference.Is(typeFullName, name);
		}

		public static bool MatchCastclass(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Castclass)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchCastclass(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchCastclass(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchCastclass(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchCastclass(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchCastclass<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchCastclass(typeof(T));
		}

		public static bool MatchCastclass(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchCastclass(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchCeq(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ceq;
		}

		public static bool MatchCgt(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Cgt;
		}

		public static bool MatchCgtUn(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Cgt_Un;
		}

		public static bool MatchCkfinite(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ckfinite;
		}

		public static bool MatchClt(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Clt;
		}

		public static bool MatchCltUn(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Clt_Un;
		}

		public static bool MatchConstrained(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Constrained)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchConstrained(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchConstrained(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchConstrained(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchConstrained(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchConstrained<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchConstrained(typeof(T));
		}

		public static bool MatchConstrained(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchConstrained(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchConvI(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_I;
		}

		public static bool MatchConvI1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_I1;
		}

		public static bool MatchConvI2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_I2;
		}

		public static bool MatchConvI4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_I4;
		}

		public static bool MatchConvI8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_I8;
		}

		public static bool MatchConvOvfI(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_I;
		}

		public static bool MatchConvOvfIUn(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_I_Un;
		}

		public static bool MatchConvOvfI1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_I1;
		}

		public static bool MatchConvOvfI1Un(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_I1_Un;
		}

		public static bool MatchConvOvfI2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_I2;
		}

		public static bool MatchConvOvfI2Un(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_I2_Un;
		}

		public static bool MatchConvOvfI4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_I4;
		}

		public static bool MatchConvOvfI4Un(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_I4_Un;
		}

		public static bool MatchConvOvfI8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_I8;
		}

		public static bool MatchConvOvfI8Un(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_I8_Un;
		}

		public static bool MatchConvOvfU(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_U;
		}

		public static bool MatchConvOvfUUn(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_U_Un;
		}

		public static bool MatchConvOvfU1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_U1;
		}

		public static bool MatchConvOvfU1Un(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_U1_Un;
		}

		public static bool MatchConvOvfU2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_U2;
		}

		public static bool MatchConvOvfU2Un(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_U2_Un;
		}

		public static bool MatchConvOvfU4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_U4;
		}

		public static bool MatchConvOvfU4Un(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_U4_Un;
		}

		public static bool MatchConvOvfU8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_U8;
		}

		public static bool MatchConvOvfU8Un(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_Ovf_U8_Un;
		}

		public static bool MatchConvRUn(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_R_Un;
		}

		public static bool MatchConvR4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_R4;
		}

		public static bool MatchConvR8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_R8;
		}

		public static bool MatchConvU(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_U;
		}

		public static bool MatchConvU1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_U1;
		}

		public static bool MatchConvU2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_U2;
		}

		public static bool MatchConvU4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_U4;
		}

		public static bool MatchConvU8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Conv_U8;
		}

		public static bool MatchCpblk(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Cpblk;
		}

		public static bool MatchCpobj(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Cpobj)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchCpobj(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchCpobj(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchCpobj(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchCpobj(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchCpobj<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchCpobj(typeof(T));
		}

		public static bool MatchCpobj(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchCpobj(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchDiv(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Div;
		}

		public static bool MatchDivUn(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Div_Un;
		}

		public static bool MatchDup(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Dup;
		}

		public static bool MatchEndfilter(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Endfilter;
		}

		public static bool MatchEndfinally(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Endfinally;
		}

		public static bool MatchInitblk(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Initblk;
		}

		public static bool MatchInitobj(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Initobj)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchInitobj(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchInitobj(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchInitobj(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchInitobj(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchInitobj<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchInitobj(typeof(T));
		}

		public static bool MatchInitobj(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchInitobj(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchIsinst(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Isinst)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchIsinst(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchIsinst(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchIsinst(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchIsinst(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchIsinst<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchIsinst(typeof(T));
		}

		public static bool MatchIsinst(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchIsinst(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchJmp(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out MethodReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Jmp)
			{
				MethodReference methodReference = instr.Operand as MethodReference;
				if (methodReference != null)
				{
					value = methodReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchJmp(this Instruction instr, MethodReference value)
		{
			MethodReference methodReference;
			return instr.MatchJmp(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchJmp(this Instruction instr, MethodBase value)
		{
			MethodReference methodReference;
			return instr.MatchJmp(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchJmp(this Instruction instr, Type type, string name)
		{
			MethodReference methodReference;
			return instr.MatchJmp(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, type, name);
		}

		public static bool MatchJmp<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchJmp(typeof(T), name);
		}

		public static bool MatchJmp(this Instruction instr, string typeFullName, string name)
		{
			MethodReference methodReference;
			return instr.MatchJmp(out methodReference) && methodReference.Is(typeFullName, name);
		}

		public static bool MatchLdarg0(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldarg_0;
		}

		public static bool MatchLdarg1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldarg_1;
		}

		public static bool MatchLdarg2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldarg_2;
		}

		public static bool MatchLdarg3(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldarg_3;
		}

		public static bool MatchLdarg(this Instruction instr, int value)
		{
			int num;
			return instr.MatchLdarg(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdarg(this Instruction instr, uint value)
		{
			int num;
			return instr.MatchLdarg(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdarga(this Instruction instr, int value)
		{
			int num;
			return instr.MatchLdarga(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdarga(this Instruction instr, uint value)
		{
			int num;
			return instr.MatchLdarga(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdcI4(this Instruction instr, int value)
		{
			int num;
			return instr.MatchLdcI4(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdcI4(this Instruction instr, uint value)
		{
			int num;
			return instr.MatchLdcI4(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdcI8(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out long value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldc_I8)
			{
				object operand = instr.Operand;
				if (operand is long)
				{
					long num = (long)operand;
					value = num;
					return true;
				}
			}
			value = 0L;
			return false;
		}

		public static bool MatchLdcI8(this Instruction instr, long value)
		{
			long num;
			return instr.MatchLdcI8(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdcI8(this Instruction instr, ulong value)
		{
			long num;
			return instr.MatchLdcI8(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdcR4(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out float value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldc_R4)
			{
				object operand = instr.Operand;
				if (operand is float)
				{
					float num = (float)operand;
					value = num;
					return true;
				}
			}
			value = 0f;
			return false;
		}

		public static bool MatchLdcR4(this Instruction instr, float value)
		{
			float num;
			return instr.MatchLdcR4(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdcR8(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out double value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldc_R8)
			{
				object operand = instr.Operand;
				if (operand is double)
				{
					double num = (double)operand;
					value = num;
					return true;
				}
			}
			value = 0.0;
			return false;
		}

		public static bool MatchLdcR8(this Instruction instr, double value)
		{
			double num;
			return instr.MatchLdcR8(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdelemAny(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelem_Any)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchLdelemAny(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchLdelemAny(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchLdelemAny(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchLdelemAny(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchLdelemAny<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchLdelemAny(typeof(T));
		}

		public static bool MatchLdelemAny(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchLdelemAny(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchLdelemI(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelem_I;
		}

		public static bool MatchLdelemI1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelem_I1;
		}

		public static bool MatchLdelemI2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelem_I2;
		}

		public static bool MatchLdelemI4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelem_I4;
		}

		public static bool MatchLdelemI8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelem_I8;
		}

		public static bool MatchLdelemR4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelem_R4;
		}

		public static bool MatchLdelemR8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelem_R8;
		}

		public static bool MatchLdelemRef(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelem_Ref;
		}

		public static bool MatchLdelemU1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelem_U1;
		}

		public static bool MatchLdelemU2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelem_U2;
		}

		public static bool MatchLdelemU4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelem_U4;
		}

		public static bool MatchLdelema(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldelema)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchLdelema(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchLdelema(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchLdelema(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchLdelema(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchLdelema<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchLdelema(typeof(T));
		}

		public static bool MatchLdelema(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchLdelema(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchLdfld(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out FieldReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldfld)
			{
				FieldReference fieldReference = instr.Operand as FieldReference;
				if (fieldReference != null)
				{
					value = fieldReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchLdfld(this Instruction instr, FieldReference value)
		{
			FieldReference fieldReference;
			return instr.MatchLdfld(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, value);
		}

		public static bool MatchLdfld(this Instruction instr, FieldInfo value)
		{
			FieldReference fieldReference;
			return instr.MatchLdfld(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, value);
		}

		public static bool MatchLdfld(this Instruction instr, Type type, string name)
		{
			FieldReference fieldReference;
			return instr.MatchLdfld(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, type, name);
		}

		public static bool MatchLdfld<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchLdfld(typeof(T), name);
		}

		public static bool MatchLdfld(this Instruction instr, string typeFullName, string name)
		{
			FieldReference fieldReference;
			return instr.MatchLdfld(out fieldReference) && fieldReference.Is(typeFullName, name);
		}

		public static bool MatchLdflda(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out FieldReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldflda)
			{
				FieldReference fieldReference = instr.Operand as FieldReference;
				if (fieldReference != null)
				{
					value = fieldReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchLdflda(this Instruction instr, FieldReference value)
		{
			FieldReference fieldReference;
			return instr.MatchLdflda(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, value);
		}

		public static bool MatchLdflda(this Instruction instr, FieldInfo value)
		{
			FieldReference fieldReference;
			return instr.MatchLdflda(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, value);
		}

		public static bool MatchLdflda(this Instruction instr, Type type, string name)
		{
			FieldReference fieldReference;
			return instr.MatchLdflda(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, type, name);
		}

		public static bool MatchLdflda<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchLdflda(typeof(T), name);
		}

		public static bool MatchLdflda(this Instruction instr, string typeFullName, string name)
		{
			FieldReference fieldReference;
			return instr.MatchLdflda(out fieldReference) && fieldReference.Is(typeFullName, name);
		}

		public static bool MatchLdftn(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out MethodReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldftn)
			{
				MethodReference methodReference = instr.Operand as MethodReference;
				if (methodReference != null)
				{
					value = methodReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchLdftn(this Instruction instr, MethodReference value)
		{
			MethodReference methodReference;
			return instr.MatchLdftn(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchLdftn(this Instruction instr, MethodBase value)
		{
			MethodReference methodReference;
			return instr.MatchLdftn(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchLdftn(this Instruction instr, Type type, string name)
		{
			MethodReference methodReference;
			return instr.MatchLdftn(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, type, name);
		}

		public static bool MatchLdftn<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchLdftn(typeof(T), name);
		}

		public static bool MatchLdftn(this Instruction instr, string typeFullName, string name)
		{
			MethodReference methodReference;
			return instr.MatchLdftn(out methodReference) && methodReference.Is(typeFullName, name);
		}

		public static bool MatchLdindI(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldind_I;
		}

		public static bool MatchLdindI1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldind_I1;
		}

		public static bool MatchLdindI2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldind_I2;
		}

		public static bool MatchLdindI4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldind_I4;
		}

		public static bool MatchLdindI8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldind_I8;
		}

		public static bool MatchLdindR4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldind_R4;
		}

		public static bool MatchLdindR8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldind_R8;
		}

		public static bool MatchLdindRef(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldind_Ref;
		}

		public static bool MatchLdindU1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldind_U1;
		}

		public static bool MatchLdindU2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldind_U2;
		}

		public static bool MatchLdindU4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldind_U4;
		}

		public static bool MatchLdlen(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldlen;
		}

		public static bool MatchLdloc0(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldloc_0;
		}

		public static bool MatchLdloc1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldloc_1;
		}

		public static bool MatchLdloc2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldloc_2;
		}

		public static bool MatchLdloc3(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldloc_3;
		}

		public static bool MatchLdloc(this Instruction instr, int value)
		{
			int num;
			return instr.MatchLdloc(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdloc(this Instruction instr, uint value)
		{
			int num;
			return instr.MatchLdloc(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdloca(this Instruction instr, int value)
		{
			int num;
			return instr.MatchLdloca(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdloca(this Instruction instr, uint value)
		{
			int num;
			return instr.MatchLdloca(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchLdnull(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldnull;
		}

		public static bool MatchLdobj(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldobj)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchLdobj(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchLdobj(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchLdobj(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchLdobj(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchLdobj<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchLdobj(typeof(T));
		}

		public static bool MatchLdobj(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchLdobj(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchLdsfld(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out FieldReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldsfld)
			{
				FieldReference fieldReference = instr.Operand as FieldReference;
				if (fieldReference != null)
				{
					value = fieldReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchLdsfld(this Instruction instr, FieldReference value)
		{
			FieldReference fieldReference;
			return instr.MatchLdsfld(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, value);
		}

		public static bool MatchLdsfld(this Instruction instr, FieldInfo value)
		{
			FieldReference fieldReference;
			return instr.MatchLdsfld(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, value);
		}

		public static bool MatchLdsfld(this Instruction instr, Type type, string name)
		{
			FieldReference fieldReference;
			return instr.MatchLdsfld(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, type, name);
		}

		public static bool MatchLdsfld<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchLdsfld(typeof(T), name);
		}

		public static bool MatchLdsfld(this Instruction instr, string typeFullName, string name)
		{
			FieldReference fieldReference;
			return instr.MatchLdsfld(out fieldReference) && fieldReference.Is(typeFullName, name);
		}

		public static bool MatchLdsflda(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out FieldReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldsflda)
			{
				FieldReference fieldReference = instr.Operand as FieldReference;
				if (fieldReference != null)
				{
					value = fieldReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchLdsflda(this Instruction instr, FieldReference value)
		{
			FieldReference fieldReference;
			return instr.MatchLdsflda(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, value);
		}

		public static bool MatchLdsflda(this Instruction instr, FieldInfo value)
		{
			FieldReference fieldReference;
			return instr.MatchLdsflda(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, value);
		}

		public static bool MatchLdsflda(this Instruction instr, Type type, string name)
		{
			FieldReference fieldReference;
			return instr.MatchLdsflda(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, type, name);
		}

		public static bool MatchLdsflda<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchLdsflda(typeof(T), name);
		}

		public static bool MatchLdsflda(this Instruction instr, string typeFullName, string name)
		{
			FieldReference fieldReference;
			return instr.MatchLdsflda(out fieldReference) && fieldReference.Is(typeFullName, name);
		}

		public static bool MatchLdstr(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out string value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldstr)
			{
				string text = instr.Operand as string;
				if (text != null)
				{
					value = text;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchLdstr(this Instruction instr, string value)
		{
			string text;
			return instr.MatchLdstr(out text) && ILPatternMatchingExt.IsEquivalent(text, value);
		}

		public static bool MatchLdtoken(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out IMetadataTokenProvider value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldtoken)
			{
				IMetadataTokenProvider metadataTokenProvider = instr.Operand as IMetadataTokenProvider;
				if (metadataTokenProvider != null)
				{
					value = metadataTokenProvider;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchLdtoken(this Instruction instr, IMetadataTokenProvider value)
		{
			IMetadataTokenProvider metadataTokenProvider;
			return instr.MatchLdtoken(out metadataTokenProvider) && ILPatternMatchingExt.IsEquivalent(metadataTokenProvider, value);
		}

		public static bool MatchLdtoken(this Instruction instr, Type value)
		{
			IMetadataTokenProvider metadataTokenProvider;
			return instr.MatchLdtoken(out metadataTokenProvider) && ILPatternMatchingExt.IsEquivalent(metadataTokenProvider, value);
		}

		public static bool MatchLdtoken<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchLdtoken(typeof(T));
		}

		public static bool MatchLdtoken(this Instruction instr, FieldInfo value)
		{
			IMetadataTokenProvider metadataTokenProvider;
			return instr.MatchLdtoken(out metadataTokenProvider) && ILPatternMatchingExt.IsEquivalent(metadataTokenProvider, value);
		}

		public static bool MatchLdtoken(this Instruction instr, MethodBase value)
		{
			IMetadataTokenProvider metadataTokenProvider;
			return instr.MatchLdtoken(out metadataTokenProvider) && ILPatternMatchingExt.IsEquivalent(metadataTokenProvider, value);
		}

		public static bool MatchLdvirtftn(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out MethodReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ldvirtftn)
			{
				MethodReference methodReference = instr.Operand as MethodReference;
				if (methodReference != null)
				{
					value = methodReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchLdvirtftn(this Instruction instr, MethodReference value)
		{
			MethodReference methodReference;
			return instr.MatchLdvirtftn(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchLdvirtftn(this Instruction instr, MethodBase value)
		{
			MethodReference methodReference;
			return instr.MatchLdvirtftn(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchLdvirtftn(this Instruction instr, Type type, string name)
		{
			MethodReference methodReference;
			return instr.MatchLdvirtftn(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, type, name);
		}

		public static bool MatchLdvirtftn<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchLdvirtftn(typeof(T), name);
		}

		public static bool MatchLdvirtftn(this Instruction instr, string typeFullName, string name)
		{
			MethodReference methodReference;
			return instr.MatchLdvirtftn(out methodReference) && methodReference.Is(typeFullName, name);
		}

		public static bool MatchLeave(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Leave || instr.OpCode == OpCodes.Leave_S)
			{
				ILLabel illabel = instr.Operand as ILLabel;
				if (illabel != null)
				{
					value = illabel;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchLeave(this Instruction instr, ILLabel value)
		{
			ILLabel illabel;
			return instr.MatchLeave(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchLeave(this Instruction instr, Instruction value)
		{
			ILLabel illabel;
			return instr.MatchLeave(out illabel) && ILPatternMatchingExt.IsEquivalent(illabel, value);
		}

		public static bool MatchLocalloc(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Localloc;
		}

		public static bool MatchMkrefany(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Mkrefany)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchMkrefany(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchMkrefany(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchMkrefany(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchMkrefany(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchMkrefany<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchMkrefany(typeof(T));
		}

		public static bool MatchMkrefany(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchMkrefany(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchMul(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Mul;
		}

		public static bool MatchMulOvf(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Mul_Ovf;
		}

		public static bool MatchMulOvfUn(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Mul_Ovf_Un;
		}

		public static bool MatchNeg(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Neg;
		}

		public static bool MatchNewarr(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Newarr)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchNewarr(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchNewarr(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchNewarr(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchNewarr(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchNewarr<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchNewarr(typeof(T));
		}

		public static bool MatchNewarr(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchNewarr(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchNewobj(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out MethodReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Newobj)
			{
				MethodReference methodReference = instr.Operand as MethodReference;
				if (methodReference != null)
				{
					value = methodReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchNewobj(this Instruction instr, MethodReference value)
		{
			MethodReference methodReference;
			return instr.MatchNewobj(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchNewobj(this Instruction instr, MethodBase value)
		{
			MethodReference methodReference;
			return instr.MatchNewobj(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, value);
		}

		public static bool MatchNewobj(this Instruction instr, Type type, string name)
		{
			MethodReference methodReference;
			return instr.MatchNewobj(out methodReference) && ILPatternMatchingExt.IsEquivalent(methodReference, type, name);
		}

		public static bool MatchNewobj<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchNewobj(typeof(T), name);
		}

		public static bool MatchNewobj(this Instruction instr, string typeFullName, string name)
		{
			MethodReference methodReference;
			return instr.MatchNewobj(out methodReference) && methodReference.Is(typeFullName, name);
		}

		public static bool MatchNop(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Nop;
		}

		public static bool MatchNot(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Not;
		}

		public static bool MatchOr(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Or;
		}

		public static bool MatchPop(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Pop;
		}

		public static bool MatchReadonly(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Readonly;
		}

		public static bool MatchRefanytype(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Refanytype;
		}

		public static bool MatchRefanyval(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Refanyval)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchRefanyval(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchRefanyval(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchRefanyval(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchRefanyval(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchRefanyval<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchRefanyval(typeof(T));
		}

		public static bool MatchRefanyval(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchRefanyval(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchRem(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Rem;
		}

		public static bool MatchRemUn(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Rem_Un;
		}

		public static bool MatchRet(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Ret;
		}

		public static bool MatchRethrow(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Rethrow;
		}

		public static bool MatchShl(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Shl;
		}

		public static bool MatchShr(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Shr;
		}

		public static bool MatchShrUn(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Shr_Un;
		}

		public static bool MatchSizeof(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Sizeof)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchSizeof(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchSizeof(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchSizeof(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchSizeof(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchSizeof<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchSizeof(typeof(T));
		}

		public static bool MatchSizeof(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchSizeof(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchStarg(this Instruction instr, int value)
		{
			int num;
			return instr.MatchStarg(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchStarg(this Instruction instr, uint value)
		{
			int num;
			return instr.MatchStarg(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchStelemAny(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stelem_Any)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchStelemAny(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchStelemAny(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchStelemAny(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchStelemAny(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchStelemAny<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchStelemAny(typeof(T));
		}

		public static bool MatchStelemAny(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchStelemAny(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchStelemI(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stelem_I;
		}

		public static bool MatchStelemI1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stelem_I1;
		}

		public static bool MatchStelemI2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stelem_I2;
		}

		public static bool MatchStelemI4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stelem_I4;
		}

		public static bool MatchStelemI8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stelem_I8;
		}

		public static bool MatchStelemR4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stelem_R4;
		}

		public static bool MatchStelemR8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stelem_R8;
		}

		public static bool MatchStelemRef(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stelem_Ref;
		}

		public static bool MatchStfld(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out FieldReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stfld)
			{
				FieldReference fieldReference = instr.Operand as FieldReference;
				if (fieldReference != null)
				{
					value = fieldReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchStfld(this Instruction instr, FieldReference value)
		{
			FieldReference fieldReference;
			return instr.MatchStfld(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, value);
		}

		public static bool MatchStfld(this Instruction instr, FieldInfo value)
		{
			FieldReference fieldReference;
			return instr.MatchStfld(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, value);
		}

		public static bool MatchStfld(this Instruction instr, Type type, string name)
		{
			FieldReference fieldReference;
			return instr.MatchStfld(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, type, name);
		}

		public static bool MatchStfld<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchStfld(typeof(T), name);
		}

		public static bool MatchStfld(this Instruction instr, string typeFullName, string name)
		{
			FieldReference fieldReference;
			return instr.MatchStfld(out fieldReference) && fieldReference.Is(typeFullName, name);
		}

		public static bool MatchStindI(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stind_I;
		}

		public static bool MatchStindI1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stind_I1;
		}

		public static bool MatchStindI2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stind_I2;
		}

		public static bool MatchStindI4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stind_I4;
		}

		public static bool MatchStindI8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stind_I8;
		}

		public static bool MatchStindR4(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stind_R4;
		}

		public static bool MatchStindR8(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stind_R8;
		}

		public static bool MatchStindRef(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stind_Ref;
		}

		public static bool MatchStloc0(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stloc_0;
		}

		public static bool MatchStloc1(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stloc_1;
		}

		public static bool MatchStloc2(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stloc_2;
		}

		public static bool MatchStloc3(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stloc_3;
		}

		public static bool MatchStloc(this Instruction instr, int value)
		{
			int num;
			return instr.MatchStloc(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchStloc(this Instruction instr, uint value)
		{
			int num;
			return instr.MatchStloc(out num) && ILPatternMatchingExt.IsEquivalent(num, value);
		}

		public static bool MatchStobj(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stobj)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchStobj(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchStobj(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchStobj(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchStobj(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchStobj<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchStobj(typeof(T));
		}

		public static bool MatchStobj(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchStobj(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchStsfld(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out FieldReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Stsfld)
			{
				FieldReference fieldReference = instr.Operand as FieldReference;
				if (fieldReference != null)
				{
					value = fieldReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchStsfld(this Instruction instr, FieldReference value)
		{
			FieldReference fieldReference;
			return instr.MatchStsfld(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, value);
		}

		public static bool MatchStsfld(this Instruction instr, FieldInfo value)
		{
			FieldReference fieldReference;
			return instr.MatchStsfld(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, value);
		}

		public static bool MatchStsfld(this Instruction instr, Type type, string name)
		{
			FieldReference fieldReference;
			return instr.MatchStsfld(out fieldReference) && ILPatternMatchingExt.IsEquivalent(fieldReference, type, name);
		}

		public static bool MatchStsfld<[Nullable(2)] T>(this Instruction instr, string name)
		{
			return instr.MatchStsfld(typeof(T), name);
		}

		public static bool MatchStsfld(this Instruction instr, string typeFullName, string name)
		{
			FieldReference fieldReference;
			return instr.MatchStsfld(out fieldReference) && fieldReference.Is(typeFullName, name);
		}

		public static bool MatchSub(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Sub;
		}

		public static bool MatchSubOvf(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Sub_Ovf;
		}

		public static bool MatchSubOvfUn(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Sub_Ovf_Un;
		}

		public static bool MatchSwitch(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ILLabel[] value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Switch)
			{
				ILLabel[] array = instr.Operand as ILLabel[];
				if (array != null)
				{
					value = array;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchSwitch(this Instruction instr, ILLabel[] value)
		{
			ILLabel[] array;
			return instr.MatchSwitch(out array) && ILPatternMatchingExt.IsEquivalent(array, value);
		}

		public static bool MatchSwitch(this Instruction instr, Instruction[] value)
		{
			ILLabel[] array;
			return instr.MatchSwitch(out array) && ILPatternMatchingExt.IsEquivalent(array, value);
		}

		public static bool MatchTail(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Tail;
		}

		public static bool MatchThrow(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Throw;
		}

		public static bool MatchUnaligned(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out byte value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Unaligned)
			{
				object operand = instr.Operand;
				if (operand is byte)
				{
					byte b = (byte)operand;
					value = b;
					return true;
				}
			}
			value = 0;
			return false;
		}

		public static bool MatchUnaligned(this Instruction instr, byte value)
		{
			byte b;
			return instr.MatchUnaligned(out b) && ILPatternMatchingExt.IsEquivalent((int)b, (int)value);
		}

		public static bool MatchUnbox(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Unbox)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchUnbox(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchUnbox(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchUnbox(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchUnbox(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchUnbox<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchUnbox(typeof(T));
		}

		public static bool MatchUnbox(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchUnbox(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchUnboxAny(this Instruction instr, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference value)
		{
			if (Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Unbox_Any)
			{
				TypeReference typeReference = instr.Operand as TypeReference;
				if (typeReference != null)
				{
					value = typeReference;
					return true;
				}
			}
			value = null;
			return false;
		}

		public static bool MatchUnboxAny(this Instruction instr, TypeReference value)
		{
			TypeReference typeReference;
			return instr.MatchUnboxAny(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchUnboxAny(this Instruction instr, Type value)
		{
			TypeReference typeReference;
			return instr.MatchUnboxAny(out typeReference) && ILPatternMatchingExt.IsEquivalent(typeReference, value);
		}

		public static bool MatchUnboxAny<[Nullable(2)] T>(this Instruction instr)
		{
			return instr.MatchUnboxAny(typeof(T));
		}

		public static bool MatchUnboxAny(this Instruction instr, string typeFullName)
		{
			TypeReference typeReference;
			return instr.MatchUnboxAny(out typeReference) && typeReference.Is(typeFullName);
		}

		public static bool MatchVolatile(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Volatile;
		}

		public static bool MatchXor(this Instruction instr)
		{
			return Helpers.ThrowIfNull<Instruction>(instr, "instr").OpCode == OpCodes.Xor;
		}

		[Nullable(0)]
		private sealed class ParameterRefEqualityComparer : IEqualityComparer<ParameterReference>
		{
			[NullableContext(2)]
			public bool Equals(ParameterReference x, ParameterReference y)
			{
				if (x == null)
				{
					return y == null;
				}
				return y != null && ILPatternMatchingExt.IsEquivalent(x.ParameterType, y.ParameterType);
			}

			public int GetHashCode([global::System.Diagnostics.CodeAnalysis.DisallowNull] ParameterReference obj)
			{
				return obj.ParameterType.GetHashCode();
			}

			public static readonly ILPatternMatchingExt.ParameterRefEqualityComparer Instance = new ILPatternMatchingExt.ParameterRefEqualityComparer();
		}
	}
}
