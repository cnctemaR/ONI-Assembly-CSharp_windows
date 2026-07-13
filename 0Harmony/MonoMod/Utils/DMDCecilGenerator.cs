using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1 })]
	internal sealed class DMDCecilGenerator : DMDGenerator<DMDCecilGenerator>
	{
		protected override MethodInfo GenerateCore(DynamicMethodDefinition dmd, [Nullable(2)] object context)
		{
			DMDCecilGenerator.<>c__DisplayClass0_0 CS$<>8__locals1 = new DMDCecilGenerator.<>c__DisplayClass0_0();
			DMDCecilGenerator.<>c__DisplayClass0_0 CS$<>8__locals2 = CS$<>8__locals1;
			MethodDefinition definition = dmd.Definition;
			if (definition == null)
			{
				throw new InvalidOperationException();
			}
			CS$<>8__locals2.def = definition;
			TypeDefinition typeDefinition = context as TypeDefinition;
			bool flag = false;
			CS$<>8__locals1.module = ((typeDefinition != null) ? typeDefinition.Module : null);
			HashSet<string> hashSet = null;
			MethodInfo methodInfo;
			try
			{
				if (typeDefinition == null || CS$<>8__locals1.module == null)
				{
					flag = true;
					string dumpName = dmd.GetDumpName("Cecil");
					CS$<>8__locals1.module = ModuleDefinition.CreateModule(dumpName, new ModuleParameters
					{
						Kind = ModuleKind.Dll,
						ReflectionImporterProvider = MMReflectionImporter.ProviderNoDefault
					});
					hashSet = new HashSet<string>();
					CS$<>8__locals1.module.Assembly.CustomAttributes.Add(new CustomAttribute(CS$<>8__locals1.module.ImportReference(DynamicMethodDefinition.c_UnverifiableCodeAttribute)));
					if (dmd.Debug)
					{
						CustomAttribute customAttribute = new CustomAttribute(CS$<>8__locals1.module.ImportReference(DynamicMethodDefinition.c_DebuggableAttribute));
						customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(CS$<>8__locals1.module.ImportReference(typeof(DebuggableAttribute.DebuggingModes)), DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations));
						CS$<>8__locals1.module.Assembly.CustomAttributes.Add(customAttribute);
					}
					string text = "";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
					defaultInterpolatedStringHandler.AppendLiteral("DMD<");
					MethodBase originalMethod = dmd.OriginalMethod;
					string text2;
					if (originalMethod == null)
					{
						text2 = null;
					}
					else
					{
						string name = originalMethod.Name;
						text2 = ((name != null) ? name.Replace('.', '_') : null);
					}
					defaultInterpolatedStringHandler.AppendFormatted(text2);
					defaultInterpolatedStringHandler.AppendLiteral(">?");
					defaultInterpolatedStringHandler.AppendFormatted<int>(this.GetHashCode());
					typeDefinition = new TypeDefinition(text, defaultInterpolatedStringHandler.ToStringAndClear(), Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Abstract | Mono.Cecil.TypeAttributes.Sealed)
					{
						BaseType = CS$<>8__locals1.module.TypeSystem.Object
					};
					CS$<>8__locals1.module.Types.Add(typeDefinition);
				}
				CS$<>8__locals1.clone = null;
				new TypeReference("System.Runtime.CompilerServices", "IsVolatile", CS$<>8__locals1.module, CS$<>8__locals1.module.TypeSystem.CoreLibrary);
				Relinker relinker = delegate(IMetadataTokenProvider mtp, [Nullable(2)] IGenericParameterProvider ctx)
				{
					if (mtp == CS$<>8__locals1.def)
					{
						return CS$<>8__locals1.clone;
					}
					MethodReference methodReference = mtp as MethodReference;
					if (methodReference != null && methodReference.FullName == CS$<>8__locals1.def.FullName && methodReference.DeclaringType.FullName == CS$<>8__locals1.def.DeclaringType.FullName && methodReference.DeclaringType.Scope.Name == CS$<>8__locals1.def.DeclaringType.Scope.Name)
					{
						return CS$<>8__locals1.clone;
					}
					return CS$<>8__locals1.module.ImportReference(mtp);
				};
				CS$<>8__locals1.clone = new MethodDefinition(dmd.Name ?? ("_" + CS$<>8__locals1.def.Name.Replace('.', '_')), CS$<>8__locals1.def.Attributes, CS$<>8__locals1.module.TypeSystem.Void)
				{
					MethodReturnType = CS$<>8__locals1.def.MethodReturnType,
					Attributes = (Mono.Cecil.MethodAttributes.FamANDAssem | Mono.Cecil.MethodAttributes.Family | Mono.Cecil.MethodAttributes.Static | Mono.Cecil.MethodAttributes.HideBySig),
					ImplAttributes = Mono.Cecil.MethodImplAttributes.IL,
					DeclaringType = typeDefinition,
					NoInlining = true
				};
				foreach (ParameterDefinition parameterDefinition in CS$<>8__locals1.def.Parameters)
				{
					CS$<>8__locals1.clone.Parameters.Add(parameterDefinition.Clone().Relink(relinker, CS$<>8__locals1.clone));
				}
				CS$<>8__locals1.clone.ReturnType = CS$<>8__locals1.def.ReturnType.Relink(relinker, CS$<>8__locals1.clone);
				typeDefinition.Methods.Add(CS$<>8__locals1.clone);
				CS$<>8__locals1.clone.HasThis = CS$<>8__locals1.def.HasThis;
				Mono.Cecil.Cil.MethodBody methodBody = (CS$<>8__locals1.clone.Body = CS$<>8__locals1.def.Body.Clone(CS$<>8__locals1.clone));
				foreach (VariableDefinition variableDefinition in CS$<>8__locals1.clone.Body.Variables)
				{
					variableDefinition.VariableType = variableDefinition.VariableType.Relink(relinker, CS$<>8__locals1.clone);
				}
				foreach (ExceptionHandler exceptionHandler in CS$<>8__locals1.clone.Body.ExceptionHandlers)
				{
					if (exceptionHandler.CatchType != null)
					{
						exceptionHandler.CatchType = exceptionHandler.CatchType.Relink(relinker, CS$<>8__locals1.clone);
					}
				}
				for (int i = 0; i < methodBody.Instructions.Count; i++)
				{
					Instruction instruction = methodBody.Instructions[i];
					object obj = instruction.Operand;
					ParameterDefinition parameterDefinition2 = obj as ParameterDefinition;
					if (parameterDefinition2 != null)
					{
						obj = CS$<>8__locals1.clone.Parameters[parameterDefinition2.Index];
					}
					else
					{
						IMetadataTokenProvider metadataTokenProvider = obj as IMetadataTokenProvider;
						if (metadataTokenProvider != null)
						{
							obj = metadataTokenProvider.Relink(relinker, CS$<>8__locals1.clone);
						}
					}
					DynamicMethodReference dynamicMethodReference = obj as DynamicMethodReference;
					if (hashSet != null)
					{
						MemberReference memberReference = obj as MemberReference;
						if (memberReference != null)
						{
							TypeReference typeReference = memberReference as TypeReference;
							IMetadataScope metadataScope = ((typeReference != null) ? typeReference.Scope : null) ?? memberReference.DeclaringType.Scope;
							if (!hashSet.Contains(metadataScope.Name))
							{
								CustomAttribute customAttribute2 = new CustomAttribute(CS$<>8__locals1.module.ImportReference(DynamicMethodDefinition.c_IgnoresAccessChecksToAttribute));
								customAttribute2.ConstructorArguments.Add(new CustomAttributeArgument(CS$<>8__locals1.module.ImportReference(typeof(DebuggableAttribute.DebuggingModes)), metadataScope.Name));
								CS$<>8__locals1.module.Assembly.CustomAttributes.Add(customAttribute2);
								hashSet.Add(metadataScope.Name);
							}
						}
					}
					instruction.Operand = obj;
				}
				CS$<>8__locals1.clone.HasThis = false;
				if (CS$<>8__locals1.def.HasThis)
				{
					TypeReference typeReference2 = CS$<>8__locals1.def.DeclaringType;
					if (typeReference2.IsValueType)
					{
						typeReference2 = new ByReferenceType(typeReference2);
					}
					CS$<>8__locals1.clone.Parameters.Insert(0, new ParameterDefinition("<>_this", Mono.Cecil.ParameterAttributes.None, typeReference2.Relink(relinker, CS$<>8__locals1.clone)));
				}
				object obj2;
				string text3 = (Switches.TryGetSwitchValue("DMDDumpTo", out obj2) ? (obj2 as string) : null);
				if (!string.IsNullOrEmpty(text3))
				{
					string text4 = Path.GetFullPath(text3);
					string text5 = CS$<>8__locals1.module.Name + ".dll";
					string text6 = Path.Combine(text4, text5);
					text4 = Path.GetDirectoryName(text6);
					if (!string.IsNullOrEmpty(text4) && !Directory.Exists(text4))
					{
						Directory.CreateDirectory(text4);
					}
					if (File.Exists(text6))
					{
						File.Delete(text6);
					}
					using (Stream stream = File.OpenWrite(text6))
					{
						CS$<>8__locals1.module.Write(stream);
					}
				}
				MethodInfo method = ReflectionHelper.Load(CS$<>8__locals1.module).GetType(typeDefinition.FullName.Replace("+", "\\+", StringComparison.Ordinal), false, false).GetMethod(CS$<>8__locals1.clone.Name, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method == null)
				{
					throw new InvalidOperationException("Could not find generated method");
				}
				methodInfo = method;
			}
			finally
			{
				if (flag)
				{
					CS$<>8__locals1.module.Dispose();
				}
				CS$<>8__locals1.module = null;
			}
			return methodInfo;
		}
	}
}
