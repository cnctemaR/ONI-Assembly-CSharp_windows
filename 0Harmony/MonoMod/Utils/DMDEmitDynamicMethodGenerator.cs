using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using MonoMod.Logs;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1 })]
	internal sealed class DMDEmitDynamicMethodGenerator : DMDGenerator<DMDEmitDynamicMethodGenerator>
	{
		protected override MethodInfo GenerateCore(DynamicMethodDefinition dmd, [Nullable(2)] object context)
		{
			MethodBase originalMethod = dmd.OriginalMethod;
			MethodDefinition definition = dmd.Definition;
			if (definition == null)
			{
				throw new InvalidOperationException();
			}
			MethodDefinition methodDefinition = definition;
			Type[] array;
			if (originalMethod != null)
			{
				ParameterInfo[] parameters = originalMethod.GetParameters();
				int num = 0;
				if (!originalMethod.IsStatic)
				{
					num++;
					array = new Type[parameters.Length + 1];
					array[0] = originalMethod.GetThisParamType();
				}
				else
				{
					array = new Type[parameters.Length];
				}
				for (int i = 0; i < parameters.Length; i++)
				{
					array[i + num] = parameters[i].ParameterType;
				}
			}
			else
			{
				int num2 = 0;
				if (methodDefinition.HasThis)
				{
					num2++;
					array = new Type[methodDefinition.Parameters.Count + 1];
					Type type3 = methodDefinition.DeclaringType.ResolveReflection();
					if (type3.IsValueType)
					{
						type3 = type3.MakeByRefType();
					}
					array[0] = type3;
				}
				else
				{
					array = new Type[methodDefinition.Parameters.Count];
				}
				for (int j = 0; j < methodDefinition.Parameters.Count; j++)
				{
					array[j + num2] = methodDefinition.Parameters[j].ParameterType.ResolveReflection();
				}
			}
			string text;
			if ((text = dmd.Name) == null)
			{
				FormatInterpolatedStringHandler formatInterpolatedStringHandler = new FormatInterpolatedStringHandler(5, 1);
				formatInterpolatedStringHandler.AppendLiteral("DMD<");
				formatInterpolatedStringHandler.AppendFormatted<object>(originalMethod ?? methodDefinition.GetID(null, null, true, true));
				formatInterpolatedStringHandler.AppendLiteral(">");
				text = DebugFormatter.Format(ref formatInterpolatedStringHandler);
			}
			string text2 = text;
			MethodInfo methodInfo = originalMethod as MethodInfo;
			Type type2 = ((methodInfo != null) ? methodInfo.ReturnType : null) ?? methodDefinition.ReturnType.ResolveReflection();
			bool flag;
			MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new MMDbgLog.DebugLogTraceStringHandler(22, 3, out flag);
			if (flag)
			{
				debugLogTraceStringHandler.AppendLiteral("new DynamicMethod: ");
				debugLogTraceStringHandler.AppendFormatted<Type>(type2);
				debugLogTraceStringHandler.AppendLiteral(" ");
				debugLogTraceStringHandler.AppendFormatted(text2);
				debugLogTraceStringHandler.AppendLiteral("(");
				debugLogTraceStringHandler.AppendFormatted(string.Join(",", array.Select<Type, string>(delegate(Type type)
				{
					if (type == null)
					{
						return null;
					}
					return type.ToString();
				}).ToArray<string>()));
				debugLogTraceStringHandler.AppendLiteral(")");
			}
			MMDbgLog.Trace(ref debugLogTraceStringHandler);
			if (originalMethod != null)
			{
				MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler2 = new MMDbgLog.DebugLogTraceStringHandler(6, 1, out flag);
				if (flag)
				{
					debugLogTraceStringHandler2.AppendLiteral("orig: ");
					debugLogTraceStringHandler2.AppendFormatted<MethodBase>(originalMethod);
				}
				MMDbgLog.Trace(ref debugLogTraceStringHandler2);
			}
			MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler3 = new MMDbgLog.DebugLogTraceStringHandler(9, 3, out flag);
			if (flag)
			{
				debugLogTraceStringHandler3.AppendLiteral("mdef: ");
				TypeReference returnType = methodDefinition.ReturnType;
				debugLogTraceStringHandler3.AppendFormatted(((returnType != null) ? returnType.ToString() : null) ?? "NULL");
				debugLogTraceStringHandler3.AppendLiteral(" ");
				debugLogTraceStringHandler3.AppendFormatted(text2);
				debugLogTraceStringHandler3.AppendLiteral("(");
				debugLogTraceStringHandler3.AppendFormatted(string.Join(",", methodDefinition.Parameters.Select<ParameterDefinition, string>(delegate(ParameterDefinition arg)
				{
					string text3;
					if (arg == null)
					{
						text3 = null;
					}
					else
					{
						TypeReference parameterType = arg.ParameterType;
						text3 = ((parameterType != null) ? parameterType.ToString() : null);
					}
					return text3 ?? "NULL";
				}).ToArray<string>()));
				debugLogTraceStringHandler3.AppendLiteral(")");
			}
			MMDbgLog.Trace(ref debugLogTraceStringHandler3);
			DynamicMethod dynamicMethod = new DynamicMethod(text2, typeof(void), array, ((originalMethod != null) ? originalMethod.DeclaringType : null) ?? typeof(DynamicMethodDefinition), true);
			DMDEmitDynamicMethodGenerator._DynamicMethod_returnType.SetValue(dynamicMethod, type2);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			_DMDEmit.Generate(dmd, dynamicMethod, ilgenerator);
			return dynamicMethod;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static DMDEmitDynamicMethodGenerator()
		{
			FieldInfo fieldInfo;
			if ((fieldInfo = typeof(DynamicMethod).GetField("returnType", BindingFlags.Instance | BindingFlags.NonPublic)) == null && (fieldInfo = typeof(DynamicMethod).GetField("_returnType", BindingFlags.Instance | BindingFlags.NonPublic)) == null && (fieldInfo = typeof(DynamicMethod).GetField("m_returnType", BindingFlags.Instance | BindingFlags.NonPublic)) == null)
			{
				throw new InvalidOperationException("Cannot find returnType field on DynamicMethod");
			}
			DMDEmitDynamicMethodGenerator._DynamicMethod_returnType = fieldInfo;
		}

		private static readonly FieldInfo _DynamicMethod_returnType;
	}
}
