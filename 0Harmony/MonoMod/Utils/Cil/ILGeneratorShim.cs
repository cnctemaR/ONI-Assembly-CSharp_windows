using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MonoMod.Utils.Cil
{
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class ILGeneratorShim
	{
		public abstract int ILOffset { get; }

		public abstract void BeginCatchBlock(Type exceptionType);

		public abstract void BeginExceptFilterBlock();

		public abstract Label BeginExceptionBlock();

		public abstract void BeginFaultBlock();

		public abstract void BeginFinallyBlock();

		public abstract void BeginScope();

		public abstract LocalBuilder DeclareLocal(Type localType);

		public abstract LocalBuilder DeclareLocal(Type localType, bool pinned);

		public abstract Label DefineLabel();

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, byte arg);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, double arg);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, short arg);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, int arg);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, long arg);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, ConstructorInfo con);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, Label label);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, Label[] labels);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, LocalBuilder local);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, SignatureHelper signature);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, FieldInfo field);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, MethodInfo meth);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, sbyte arg);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, float arg);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, string str);

		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, Type cls);

		public abstract void EmitCall(global::System.Reflection.Emit.OpCode opcode, MethodInfo methodInfo, [Nullable(new byte[] { 2, 1 })] Type[] optionalParameterTypes);

		[NullableContext(2)]
		public abstract void EmitCalli(global::System.Reflection.Emit.OpCode opcode, CallingConventions callingConvention, Type returnType, [Nullable(new byte[] { 2, 1 })] Type[] parameterTypes, [Nullable(new byte[] { 2, 1 })] Type[] optionalParameterTypes);

		[NullableContext(2)]
		public abstract void EmitCalli(global::System.Reflection.Emit.OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, [Nullable(new byte[] { 2, 1 })] Type[] parameterTypes);

		public abstract void EmitWriteLine(LocalBuilder localBuilder);

		public abstract void EmitWriteLine(FieldInfo fld);

		public abstract void EmitWriteLine(string value);

		public abstract void EndExceptionBlock();

		public abstract void EndScope();

		public abstract void MarkLabel(Label loc);

		public abstract void ThrowException(Type excType);

		public abstract void UsingNamespace(string usingNamespace);

		public ILGenerator GetProxy()
		{
			return (ILGenerator)ILGeneratorShim.ILGeneratorBuilder.GenerateProxy().MakeGenericType(new Type[] { base.GetType() }).GetConstructors()[0].Invoke(new object[] { this });
		}

		public static Type GetProxyType<[Nullable(0)] TShim>() where TShim : ILGeneratorShim
		{
			return ILGeneratorShim.GetProxyType(typeof(TShim));
		}

		public static Type GetProxyType(Type tShim)
		{
			return ILGeneratorShim.GenericProxyType.MakeGenericType(new Type[] { tShim });
		}

		public static Type GenericProxyType
		{
			get
			{
				return ILGeneratorShim.ILGeneratorBuilder.GenerateProxy();
			}
		}

		[Nullable(0)]
		internal static class ILGeneratorBuilder
		{
			public static Type GenerateProxy()
			{
				if (ILGeneratorShim.ILGeneratorBuilder.ProxyType != null)
				{
					return ILGeneratorShim.ILGeneratorBuilder.ProxyType;
				}
				Type typeFromHandle = typeof(ILGenerator);
				Type typeFromHandle2 = typeof(ILGeneratorShim);
				Assembly assembly;
				using (ModuleDefinition moduleDefinition = ModuleDefinition.CreateModule("MonoMod.Utils.Cil.ILGeneratorProxy", new ModuleParameters
				{
					Kind = ModuleKind.Dll,
					ReflectionImporterProvider = MMReflectionImporter.Provider
				}))
				{
					CustomAttribute customAttribute = new CustomAttribute(moduleDefinition.ImportReference(DynamicMethodDefinition.c_IgnoresAccessChecksToAttribute));
					customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(moduleDefinition.TypeSystem.String, typeof(ILGeneratorShim).Assembly.GetName().Name));
					moduleDefinition.Assembly.CustomAttributes.Add(customAttribute);
					TypeDefinition typeDefinition = new TypeDefinition("MonoMod.Utils.Cil", "ILGeneratorProxy", Mono.Cecil.TypeAttributes.Public)
					{
						BaseType = moduleDefinition.ImportReference(typeFromHandle)
					};
					moduleDefinition.Types.Add(typeDefinition);
					TypeReference typeReference = moduleDefinition.ImportReference(typeFromHandle2);
					GenericParameter genericParameter = new GenericParameter("TTarget", typeDefinition);
					genericParameter.Constraints.Add(new GenericParameterConstraint(typeReference));
					typeDefinition.GenericParameters.Add(genericParameter);
					FieldDefinition fieldDefinition = new FieldDefinition("Target", Mono.Cecil.FieldAttributes.Public, genericParameter);
					typeDefinition.Fields.Add(fieldDefinition);
					FieldReference fieldReference = new FieldReference("Target", genericParameter, new GenericInstanceType(typeDefinition)
					{
						GenericArguments = { genericParameter }
					});
					MethodDefinition methodDefinition = new MethodDefinition(".ctor", Mono.Cecil.MethodAttributes.FamANDAssem | Mono.Cecil.MethodAttributes.Family | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName, moduleDefinition.TypeSystem.Void);
					methodDefinition.Parameters.Add(new ParameterDefinition(genericParameter));
					typeDefinition.Methods.Add(methodDefinition);
					ILProcessor ilprocessor = methodDefinition.Body.GetILProcessor();
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_1);
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Stfld, fieldReference);
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
					foreach (MethodInfo methodInfo in typeFromHandle.GetMethods(BindingFlags.Instance | BindingFlags.Public))
					{
						MethodInfo method = typeFromHandle2.GetMethod(methodInfo.Name, (from p in methodInfo.GetParameters()
							select p.ParameterType).ToArray<Type>());
						if (!(method == null))
						{
							MethodDefinition methodDefinition2 = new MethodDefinition(methodInfo.Name, Mono.Cecil.MethodAttributes.FamANDAssem | Mono.Cecil.MethodAttributes.Family | Mono.Cecil.MethodAttributes.Virtual | Mono.Cecil.MethodAttributes.HideBySig, moduleDefinition.ImportReference(methodInfo.ReturnType))
							{
								HasThis = true
							};
							foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
							{
								methodDefinition2.Parameters.Add(new ParameterDefinition(moduleDefinition.ImportReference(parameterInfo.ParameterType)));
							}
							typeDefinition.Methods.Add(methodDefinition2);
							ilprocessor = methodDefinition2.Body.GetILProcessor();
							ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
							ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldfld, fieldReference);
							foreach (ParameterDefinition parameterDefinition in methodDefinition2.Parameters)
							{
								ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg, parameterDefinition);
							}
							ilprocessor.Emit(method.IsVirtual ? Mono.Cecil.Cil.OpCodes.Callvirt : Mono.Cecil.Cil.OpCodes.Call, ilprocessor.Body.Method.Module.ImportReference(method));
							ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
						}
					}
					assembly = ReflectionHelper.Load(moduleDefinition);
					assembly.SetMonoCorlibInternal(true);
				}
				ResolveEventHandler resolveEventHandler = delegate(object asmSender, ResolveEventArgs asmArgs)
				{
					if (new AssemblyName(asmArgs.Name).Name == typeof(ILGeneratorShim.ILGeneratorBuilder).Assembly.GetName().Name)
					{
						return typeof(ILGeneratorShim.ILGeneratorBuilder).Assembly;
					}
					return null;
				};
				AppDomain.CurrentDomain.AssemblyResolve += resolveEventHandler;
				try
				{
					ILGeneratorShim.ILGeneratorBuilder.ProxyType = assembly.GetType("MonoMod.Utils.Cil.ILGeneratorProxy");
				}
				finally
				{
					AppDomain.CurrentDomain.AssemblyResolve -= resolveEventHandler;
				}
				if (ILGeneratorShim.ILGeneratorBuilder.ProxyType == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("Couldn't find ILGeneratorShim proxy \"").Append("MonoMod.Utils.Cil.ILGeneratorProxy").Append("\" in autogenerated \"")
						.Append(assembly.FullName)
						.AppendLine("\"");
					Type[] array;
					Exception[] array2;
					try
					{
						array = assembly.GetTypes();
						array2 = null;
					}
					catch (ReflectionTypeLoadException ex)
					{
						array = ex.Types;
						array2 = new Exception[ex.LoaderExceptions.Length + 1];
						array2[0] = ex;
						for (int k = 0; k < ex.LoaderExceptions.Length; k++)
						{
							array2[k + 1] = ex.LoaderExceptions[k];
						}
					}
					stringBuilder.AppendLine("Listing all types in autogenerated assembly:");
					foreach (Type type in array)
					{
						stringBuilder.AppendLine(((type != null) ? type.FullName : null) ?? "<NULL>");
					}
					if (array2 != null && array2.Length != 0)
					{
						stringBuilder.AppendLine("Listing all exceptions:");
						for (int l = 0; l < array2.Length; l++)
						{
							StringBuilder stringBuilder2 = stringBuilder.Append('#').Append(l).Append(": ");
							Exception ex2 = array2[l];
							stringBuilder2.AppendLine(((ex2 != null) ? ex2.ToString() : null) ?? "NULL");
						}
					}
					throw new InvalidOperationException(stringBuilder.ToString());
				}
				return ILGeneratorShim.ILGeneratorBuilder.ProxyType;
			}

			public const string Namespace = "MonoMod.Utils.Cil";

			public const string Name = "ILGeneratorProxy";

			public const string FullName = "MonoMod.Utils.Cil.ILGeneratorProxy";

			public const string TargetName = "Target";

			[Nullable(2)]
			private static Type ProxyType;
		}
	}
}
