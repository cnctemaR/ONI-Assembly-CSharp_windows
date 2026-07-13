using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using MonoMod.Utils;

namespace HarmonyLib
{
	internal class MethodPatcherTools
	{
		internal static DynamicMethodDefinition CreateDynamicMethod(MethodBase original, string suffix, bool debug)
		{
			if (original == null)
			{
				throw new ArgumentNullException("original");
			}
			Type declaringType = original.DeclaringType;
			string text = (((declaringType != null) ? declaringType.FullName : null) ?? "GLOBALTYPE") + "." + original.Name + suffix;
			text = text.Replace("<>", "");
			ParameterInfo[] parameters = original.GetParameters();
			List<Type> list = new List<Type>();
			list.AddRange(parameters.Types());
			if (!original.IsStatic)
			{
				if (AccessTools.IsStruct(original.DeclaringType))
				{
					list.Insert(0, original.DeclaringType.MakeByRefType());
				}
				else
				{
					list.Insert(0, original.DeclaringType);
				}
			}
			Type returnedType = AccessTools.GetReturnedType(original);
			DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(text, returnedType, list.ToArray());
			int num = ((!original.IsStatic) ? 1 : 0);
			if (!original.IsStatic)
			{
				dynamicMethodDefinition.Definition.Parameters[0].Name = "this";
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterDefinition parameterDefinition = dynamicMethodDefinition.Definition.Parameters[i + num];
				parameterDefinition.Attributes = (Mono.Cecil.ParameterAttributes)parameters[i].Attributes;
				parameterDefinition.Name = parameters[i].Name;
			}
			if (debug)
			{
				List<string> list2 = list.Select<Type, string>((Type p) => p.FullDescription()).ToList<string>();
				if (list.Count == dynamicMethodDefinition.Definition.Parameters.Count)
				{
					for (int j = 0; j < list.Count; j++)
					{
						List<string> list3 = list2;
						int num2 = j;
						list3[num2] = list3[num2] + " " + dynamicMethodDefinition.Definition.Parameters[j].Name;
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 4);
				defaultInterpolatedStringHandler.AppendLiteral("### Replacement: static ");
				defaultInterpolatedStringHandler.AppendFormatted(returnedType.FullDescription());
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				Type declaringType2 = original.DeclaringType;
				defaultInterpolatedStringHandler.AppendFormatted(((declaringType2 != null) ? declaringType2.FullName : null) ?? "GLOBALTYPE");
				defaultInterpolatedStringHandler.AppendLiteral("::");
				defaultInterpolatedStringHandler.AppendFormatted(text);
				defaultInterpolatedStringHandler.AppendLiteral("(");
				defaultInterpolatedStringHandler.AppendFormatted(list2.Join<string>(null, ", "));
				defaultInterpolatedStringHandler.AppendLiteral(")");
				FileLog.Log(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return dynamicMethodDefinition;
		}

		[return: TupleElementNames(new string[] { "info", "realName" })]
		internal static IEnumerable<ValueTuple<ParameterInfo, string>> OriginalParameters(MethodInfo method)
		{
			IEnumerable<HarmonyArgument> baseArgs = method.GetArgumentAttributes();
			if (method.DeclaringType != null)
			{
				baseArgs = baseArgs.Union<HarmonyArgument>(method.DeclaringType.GetArgumentAttributes()).OfType<HarmonyArgument>();
			}
			return method.GetParameters().Select<ParameterInfo, ValueTuple<ParameterInfo, string>>(delegate(ParameterInfo p)
			{
				HarmonyArgument argumentAttribute = p.GetArgumentAttribute();
				if (argumentAttribute != null)
				{
					return new ValueTuple<ParameterInfo, string>(p, argumentAttribute.OriginalName ?? p.Name);
				}
				return new ValueTuple<ParameterInfo, string>(p, baseArgs.GetRealName(p.Name, null) ?? p.Name);
			});
		}

		internal static Dictionary<string, string> RealNames(MethodInfo method)
		{
			return MethodPatcherTools.OriginalParameters(method).ToDictionary<ValueTuple<ParameterInfo, string>, string, string>(([TupleElementNames(new string[] { "info", "realName" })] ValueTuple<ParameterInfo, string> pair) => pair.Item1.Name, ([TupleElementNames(new string[] { "info", "realName" })] ValueTuple<ParameterInfo, string> pair) => pair.Item2);
		}

		internal static LocalBuilder[] DeclareOriginalLocalVariables(ILGenerator il, MethodBase member)
		{
			MethodBody methodBody = member.GetMethodBody();
			IList<LocalVariableInfo> list = ((methodBody != null) ? methodBody.LocalVariables : null);
			if (list == null)
			{
				return Array.Empty<LocalBuilder>();
			}
			return list.Select<LocalVariableInfo, LocalBuilder>((LocalVariableInfo lvi) => il.DeclareLocal(lvi.LocalType, lvi.IsPinned)).ToArray<LocalBuilder>();
		}

		internal static bool PrefixAffectsOriginal(MethodInfo fix)
		{
			if (fix.ReturnType == typeof(bool))
			{
				return true;
			}
			return MethodPatcherTools.OriginalParameters(fix).Any<ValueTuple<ParameterInfo, string>>(delegate([TupleElementNames(new string[] { "info", "realName" })] ValueTuple<ParameterInfo, string> pair)
			{
				ParameterInfo item = pair.Item1;
				string item2 = pair.Item2;
				Type parameterType = item.ParameterType;
				return !(item2 == "__instance") && !(item2 == "__originalMethod") && !(item2 == "__state") && (item.IsOut || item.IsRetval || parameterType.IsByRef || (!AccessTools.IsValue(parameterType) && !AccessTools.IsStruct(parameterType)));
			});
		}

		internal static bool EmitOriginalBaseMethod(MethodBase original, Emitter emitter)
		{
			MethodInfo methodInfo = original as MethodInfo;
			if (methodInfo != null)
			{
				emitter.Emit(OpCodes.Ldtoken, methodInfo);
			}
			else
			{
				ConstructorInfo constructorInfo = original as ConstructorInfo;
				if (constructorInfo == null)
				{
					return false;
				}
				emitter.Emit(OpCodes.Ldtoken, constructorInfo);
			}
			Type reflectedType = original.ReflectedType;
			if (reflectedType.IsGenericType)
			{
				emitter.Emit(OpCodes.Ldtoken, reflectedType);
			}
			emitter.Emit(OpCodes.Call, reflectedType.IsGenericType ? MethodPatcherTools.m_GetMethodFromHandle2 : MethodPatcherTools.m_GetMethodFromHandle1);
			return true;
		}

		internal static OpCode LoadIndOpCodeFor(Type type)
		{
			if (MethodPatcherTools.PrimitivesWithObjectTypeCode.Contains(type))
			{
				return OpCodes.Ldind_I;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Empty:
			case TypeCode.Object:
			case TypeCode.DBNull:
			case TypeCode.String:
				return OpCodes.Ldind_Ref;
			case TypeCode.Boolean:
			case TypeCode.SByte:
			case TypeCode.Byte:
				return OpCodes.Ldind_I1;
			case TypeCode.Char:
			case TypeCode.Int16:
			case TypeCode.UInt16:
				return OpCodes.Ldind_I2;
			case TypeCode.Int32:
			case TypeCode.UInt32:
				return OpCodes.Ldind_I4;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return OpCodes.Ldind_I8;
			case TypeCode.Single:
				return OpCodes.Ldind_R4;
			case TypeCode.Double:
				return OpCodes.Ldind_R8;
			case TypeCode.Decimal:
			case TypeCode.DateTime:
				throw new NotSupportedException();
			}
			return OpCodes.Ldind_Ref;
		}

		internal static OpCode StoreIndOpCodeFor(Type type)
		{
			if (MethodPatcherTools.PrimitivesWithObjectTypeCode.Contains(type))
			{
				return OpCodes.Stind_I;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Empty:
			case TypeCode.Object:
			case TypeCode.DBNull:
			case TypeCode.String:
				return OpCodes.Stind_Ref;
			case TypeCode.Boolean:
			case TypeCode.SByte:
			case TypeCode.Byte:
				return OpCodes.Stind_I1;
			case TypeCode.Char:
			case TypeCode.Int16:
			case TypeCode.UInt16:
				return OpCodes.Stind_I2;
			case TypeCode.Int32:
			case TypeCode.UInt32:
				return OpCodes.Stind_I4;
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return OpCodes.Stind_I8;
			case TypeCode.Single:
				return OpCodes.Stind_R4;
			case TypeCode.Double:
				return OpCodes.Stind_R8;
			case TypeCode.Decimal:
			case TypeCode.DateTime:
				throw new NotSupportedException();
			}
			return OpCodes.Stind_Ref;
		}

		internal const string INSTANCE_PARAM = "__instance";

		internal const string ORIGINAL_METHOD_PARAM = "__originalMethod";

		internal const string ARGS_ARRAY_VAR = "__args";

		internal const string RESULT_VAR = "__result";

		internal const string RESULT_REF_VAR = "__resultRef";

		internal const string STATE_VAR = "__state";

		internal const string EXCEPTION_VAR = "__exception";

		internal const string RUN_ORIGINAL_VAR = "__runOriginal";

		internal const string PARAM_INDEX_PREFIX = "__";

		internal const string INSTANCE_FIELD_PREFIX = "___";

		private static readonly MethodInfo m_GetMethodFromHandle1 = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) });

		private static readonly MethodInfo m_GetMethodFromHandle2 = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[]
		{
			typeof(RuntimeMethodHandle),
			typeof(RuntimeTypeHandle)
		});

		private static readonly HashSet<Type> PrimitivesWithObjectTypeCode = new HashSet<Type>
		{
			typeof(IntPtr),
			typeof(UIntPtr),
			typeof(IntPtr),
			typeof(UIntPtr)
		};
	}
}
