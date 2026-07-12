using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MonoMod.Utils.Cil
{
	internal static class ILGeneratorShimExt
	{
		static ILGeneratorShimExt()
		{
			foreach (MethodInfo methodInfo in typeof(ILGenerator).GetMethods())
			{
				if (!(methodInfo.Name != "Emit"))
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length == 2 && !(parameters[0].ParameterType != typeof(OpCode)))
					{
						ILGeneratorShimExt._Emitters[parameters[1].ParameterType] = methodInfo;
					}
				}
			}
			foreach (MethodInfo methodInfo2 in typeof(ILGeneratorShim).GetMethods())
			{
				if (!(methodInfo2.Name != "Emit"))
				{
					ParameterInfo[] parameters2 = methodInfo2.GetParameters();
					if (parameters2.Length == 2 && !(parameters2[0].ParameterType != typeof(OpCode)))
					{
						ILGeneratorShimExt._EmittersShim[parameters2[1].ParameterType] = methodInfo2;
					}
				}
			}
		}

		public static ILGeneratorShim GetProxiedShim(this ILGenerator il)
		{
			FieldInfo field = il.GetType().GetField("Target", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return ((field != null) ? field.GetValue(il) : null) as ILGeneratorShim;
		}

		public static T GetProxiedShim<T>(this ILGenerator il) where T : ILGeneratorShim
		{
			return il.GetProxiedShim() as T;
		}

		public static object DynEmit(this ILGenerator il, OpCode opcode, object operand)
		{
			return il.DynEmit(new object[] { opcode, operand });
		}

		public static object DynEmit(this ILGenerator il, object[] emitArgs)
		{
			Type operandType = emitArgs[1].GetType();
			object obj = il.GetProxiedShim() ?? il;
			Dictionary<Type, MethodInfo> dictionary = ((obj is ILGeneratorShim) ? ILGeneratorShimExt._EmittersShim : ILGeneratorShimExt._Emitters);
			MethodInfo value;
			if (!dictionary.TryGetValue(operandType, out value))
			{
				value = dictionary.FirstOrDefault<KeyValuePair<Type, MethodInfo>>((KeyValuePair<Type, MethodInfo> kvp) => kvp.Key.IsAssignableFrom(operandType)).Value;
			}
			if (value == null)
			{
				throw new InvalidOperationException("Unexpected unemittable operand type " + operandType.FullName);
			}
			return value.Invoke(obj, emitArgs);
		}

		private static readonly Dictionary<Type, MethodInfo> _Emitters = new Dictionary<Type, MethodInfo>();

		private static readonly Dictionary<Type, MethodInfo> _EmittersShim = new Dictionary<Type, MethodInfo>();
	}
}
