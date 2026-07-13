using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Utils.Cil;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class DynamicMethodDefinition : IDisposable
	{
		private static void _InitCopier()
		{
			DynamicMethodDefinition._CecilOpCodes1X = new Mono.Cecil.Cil.OpCode[225];
			DynamicMethodDefinition._CecilOpCodes2X = new Mono.Cecil.Cil.OpCode[31];
			FieldInfo[] fields = typeof(Mono.Cecil.Cil.OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public);
			for (int i = 0; i < fields.Length; i++)
			{
				Mono.Cecil.Cil.OpCode opCode = (Mono.Cecil.Cil.OpCode)fields[i].GetValue(null);
				if (opCode.OpCodeType != Mono.Cecil.Cil.OpCodeType.Nternal)
				{
					if (opCode.Size == 1)
					{
						DynamicMethodDefinition._CecilOpCodes1X[(int)opCode.Value] = opCode;
					}
					else
					{
						DynamicMethodDefinition._CecilOpCodes2X[(int)(opCode.Value & 255)] = opCode;
					}
				}
			}
		}

		private static void _CopyMethodToDefinition(MethodBase from, MethodDefinition into)
		{
			DynamicMethodDefinition.<>c__DisplayClass3_0 CS$<>8__locals1 = new DynamicMethodDefinition.<>c__DisplayClass3_0();
			CS$<>8__locals1.into = into;
			CS$<>8__locals1.moduleFrom = from.Module;
			global::System.Reflection.MethodBody methodBody = from.GetMethodBody();
			if (methodBody == null)
			{
				throw new NotSupportedException("Body-less method");
			}
			global::System.Reflection.MethodBody methodBody2 = methodBody;
			byte[] ilasByteArray = methodBody2.GetILAsByteArray();
			if (ilasByteArray == null)
			{
				throw new InvalidOperationException();
			}
			byte[] array = ilasByteArray;
			CS$<>8__locals1.moduleTo = CS$<>8__locals1.into.Module;
			CS$<>8__locals1.bodyTo = CS$<>8__locals1.into.Body;
			CS$<>8__locals1.bodyTo.GetILProcessor();
			CS$<>8__locals1.typeArguments = null;
			Type declaringType = from.DeclaringType;
			if (declaringType != null && declaringType.IsGenericType)
			{
				CS$<>8__locals1.typeArguments = from.DeclaringType.GetGenericArguments();
			}
			CS$<>8__locals1.methodArguments = null;
			if (from.IsGenericMethod)
			{
				CS$<>8__locals1.methodArguments = from.GetGenericArguments();
			}
			foreach (LocalVariableInfo localVariableInfo in methodBody2.LocalVariables)
			{
				TypeReference typeReference = CS$<>8__locals1.moduleTo.ImportReference(localVariableInfo.LocalType);
				if (localVariableInfo.IsPinned)
				{
					typeReference = new PinnedType(typeReference);
				}
				CS$<>8__locals1.bodyTo.Variables.Add(new VariableDefinition(typeReference));
			}
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(array)))
			{
				Instruction instruction = null;
				while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
				{
					int num = (int)binaryReader.BaseStream.Position;
					Instruction instruction2 = Instruction.Create(Mono.Cecil.Cil.OpCodes.Nop);
					byte b = binaryReader.ReadByte();
					instruction2.OpCode = ((b != 254) ? DynamicMethodDefinition._CecilOpCodes1X[(int)b] : DynamicMethodDefinition._CecilOpCodes2X[(int)binaryReader.ReadByte()]);
					instruction2.Offset = num;
					if (instruction != null)
					{
						instruction.Next = instruction2;
					}
					instruction2.Previous = instruction;
					CS$<>8__locals1.<_CopyMethodToDefinition>g__ReadOperand|0(binaryReader, instruction2);
					CS$<>8__locals1.bodyTo.Instructions.Add(instruction2);
					instruction = instruction2;
				}
			}
			foreach (Instruction instruction3 in CS$<>8__locals1.bodyTo.Instructions)
			{
				Mono.Cecil.Cil.OperandType operandType = instruction3.OpCode.OperandType;
				if (operandType != Mono.Cecil.Cil.OperandType.InlineBrTarget)
				{
					if (operandType == Mono.Cecil.Cil.OperandType.InlineSwitch)
					{
						int[] array2 = (int[])instruction3.Operand;
						Instruction[] array3 = new Instruction[array2.Length];
						for (int i = 0; i < array2.Length; i++)
						{
							array3[i] = CS$<>8__locals1.<_CopyMethodToDefinition>g__GetInstruction|2(array2[i]);
						}
						instruction3.Operand = array3;
						continue;
					}
					if (operandType != Mono.Cecil.Cil.OperandType.ShortInlineBrTarget)
					{
						continue;
					}
				}
				instruction3.Operand = CS$<>8__locals1.<_CopyMethodToDefinition>g__GetInstruction|2((int)instruction3.Operand);
			}
			foreach (ExceptionHandlingClause exceptionHandlingClause in methodBody2.ExceptionHandlingClauses)
			{
				Mono.Cecil.Cil.ExceptionHandler exceptionHandler = new Mono.Cecil.Cil.ExceptionHandler((ExceptionHandlerType)exceptionHandlingClause.Flags);
				CS$<>8__locals1.bodyTo.ExceptionHandlers.Add(exceptionHandler);
				exceptionHandler.TryStart = CS$<>8__locals1.<_CopyMethodToDefinition>g__GetInstruction|2(exceptionHandlingClause.TryOffset);
				exceptionHandler.TryEnd = CS$<>8__locals1.<_CopyMethodToDefinition>g__GetInstruction|2(exceptionHandlingClause.TryOffset + exceptionHandlingClause.TryLength);
				exceptionHandler.FilterStart = ((exceptionHandler.HandlerType != ExceptionHandlerType.Filter) ? null : CS$<>8__locals1.<_CopyMethodToDefinition>g__GetInstruction|2(exceptionHandlingClause.FilterOffset));
				exceptionHandler.HandlerStart = CS$<>8__locals1.<_CopyMethodToDefinition>g__GetInstruction|2(exceptionHandlingClause.HandlerOffset);
				exceptionHandler.HandlerEnd = CS$<>8__locals1.<_CopyMethodToDefinition>g__GetInstruction|2(exceptionHandlingClause.HandlerOffset + exceptionHandlingClause.HandlerLength);
				exceptionHandler.CatchType = ((exceptionHandler.HandlerType != ExceptionHandlerType.Catch) ? null : ((exceptionHandlingClause.CatchType == null) ? null : CS$<>8__locals1.moduleTo.ImportReference(exceptionHandlingClause.CatchType)));
			}
		}

		static DynamicMethodDefinition()
		{
			bool flag;
			if (PlatformDetection.Runtime != RuntimeKind.Mono || DynamicMethodDefinition._IsNewMonoSRE || DynamicMethodDefinition._IsOldMonoSRE)
			{
				if (PlatformDetection.Runtime != RuntimeKind.Mono)
				{
					Type type = typeof(ILGenerator).Assembly.GetType("System.Reflection.Emit.DynamicILGenerator");
					flag = ((type != null) ? type.GetField("m_scope", BindingFlags.Instance | BindingFlags.NonPublic) : null) == null;
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = true;
			}
			DynamicMethodDefinition._PreferCecil = flag;
			DynamicMethodDefinition.c_DebuggableAttribute = typeof(DebuggableAttribute).GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
			DynamicMethodDefinition.c_UnverifiableCodeAttribute = typeof(UnverifiableCodeAttribute).GetConstructor(ArrayEx.Empty<Type>());
			DynamicMethodDefinition.c_IgnoresAccessChecksToAttribute = typeof(IgnoresAccessChecksToAttribute).GetConstructor(new Type[] { typeof(string) });
			DynamicMethodDefinition.t__IDMDGenerator = typeof(IDMDGenerator);
			DynamicMethodDefinition._DMDGeneratorCache = new ConcurrentDictionary<string, IDMDGenerator>();
			DynamicMethodDefinition._InitCopier();
		}

		public static bool IsDynamicILAvailable
		{
			get
			{
				return !DynamicMethodDefinition._PreferCecil;
			}
		}

		[Nullable(2)]
		public MethodBase OriginalMethod
		{
			[NullableContext(2)]
			get;
		}

		public MethodDefinition Definition { get; }

		public ModuleDefinition Module { get; }

		[Nullable(2)]
		public string Name
		{
			[NullableContext(2)]
			get;
		}

		public bool Debug { get; set; }

		private static bool GetDefaultDebugValue()
		{
			bool flag;
			return Switches.TryGetSwitchEnabled("DMDDebug", out flag) && flag;
		}

		public DynamicMethodDefinition(MethodBase method)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(method, "method");
			this.OriginalMethod = method;
			this.Debug = DynamicMethodDefinition.GetDefaultDebugValue();
			ModuleDefinition moduleDefinition;
			MethodDefinition methodDefinition;
			this.LoadFromMethod(method, out moduleDefinition, out methodDefinition);
			this.Module = moduleDefinition;
			this.Definition = methodDefinition;
		}

		public DynamicMethodDefinition(DynamicMethodDefinition method)
		{
			Helpers.ThrowIfArgumentNull<DynamicMethodDefinition>(method, "method");
			this.OriginalMethod = null;
			this.Debug = DynamicMethodDefinition.GetDefaultDebugValue();
			this.Name = method.Name;
			ModuleDefinition moduleDefinition;
			MethodDefinition methodDefinition;
			this.CreateFromDmd(method, out moduleDefinition, out methodDefinition);
			this.Module = moduleDefinition;
			this.Definition = methodDefinition;
		}

		public DynamicMethodDefinition(string name, [Nullable(2)] Type returnType, Type[] parameterTypes)
		{
			Helpers.ThrowIfArgumentNull<string>(name, "name");
			Helpers.ThrowIfArgumentNull<Type[]>(parameterTypes, "parameterTypes");
			this.Name = name;
			this.OriginalMethod = null;
			this.Debug = DynamicMethodDefinition.GetDefaultDebugValue();
			ModuleDefinition moduleDefinition;
			MethodDefinition methodDefinition;
			this._CreateDynModule(name, returnType, parameterTypes, out moduleDefinition, out methodDefinition);
			this.Module = moduleDefinition;
			this.Definition = methodDefinition;
		}

		[MemberNotNull("Definition")]
		public ILProcessor GetILProcessor()
		{
			if (this.Definition == null)
			{
				throw new InvalidOperationException();
			}
			return this.Definition.Body.GetILProcessor();
		}

		[MemberNotNull("Definition")]
		public ILGenerator GetILGenerator()
		{
			if (this.Definition == null)
			{
				throw new InvalidOperationException();
			}
			return new CecilILGenerator(this.Definition.Body.GetILProcessor()).GetProxy();
		}

		private void _CreateDynModule(string name, [Nullable(2)] Type returnType, Type[] parameterTypes, out ModuleDefinition Module, out MethodDefinition Definition)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
			defaultInterpolatedStringHandler.AppendLiteral("DMD:DynModule<");
			defaultInterpolatedStringHandler.AppendFormatted(name);
			defaultInterpolatedStringHandler.AppendLiteral(">?");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.GetHashCode());
			ModuleDefinition moduleDefinition;
			Module = (moduleDefinition = ModuleDefinition.CreateModule(defaultInterpolatedStringHandler.ToStringAndClear(), new ModuleParameters
			{
				Kind = ModuleKind.Dll,
				ReflectionImporterProvider = MMReflectionImporter.ProviderNoDefault
			}));
			ModuleDefinition moduleDefinition2 = moduleDefinition;
			string text = "";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(6, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("DMD<");
			defaultInterpolatedStringHandler2.AppendFormatted(name);
			defaultInterpolatedStringHandler2.AppendLiteral(">?");
			defaultInterpolatedStringHandler2.AppendFormatted<int>(this.GetHashCode());
			TypeDefinition typeDefinition = new TypeDefinition(text, defaultInterpolatedStringHandler2.ToStringAndClear(), Mono.Cecil.TypeAttributes.Public);
			moduleDefinition2.Types.Add(typeDefinition);
			MethodDefinition methodDefinition;
			Definition = (methodDefinition = new MethodDefinition(name, Mono.Cecil.MethodAttributes.FamANDAssem | Mono.Cecil.MethodAttributes.Family | Mono.Cecil.MethodAttributes.Static | Mono.Cecil.MethodAttributes.HideBySig, (returnType != null) ? moduleDefinition2.ImportReference(returnType) : moduleDefinition2.TypeSystem.Void));
			MethodDefinition methodDefinition2 = methodDefinition;
			foreach (Type type in parameterTypes)
			{
				methodDefinition2.Parameters.Add(new ParameterDefinition(moduleDefinition2.ImportReference(type)));
			}
			typeDefinition.Methods.Add(methodDefinition2);
		}

		private void CreateFromDmd(DynamicMethodDefinition src, out ModuleDefinition Module, out MethodDefinition Definition)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
			defaultInterpolatedStringHandler.AppendLiteral("DMD:DynModule<");
			defaultInterpolatedStringHandler.AppendFormatted(src.Name);
			defaultInterpolatedStringHandler.AppendLiteral(">?");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.GetHashCode());
			ModuleDefinition moduleDefinition;
			Module = (moduleDefinition = ModuleDefinition.CreateModule(defaultInterpolatedStringHandler.ToStringAndClear(), new ModuleParameters
			{
				Kind = ModuleKind.Dll,
				ReflectionImporterProvider = MMReflectionImporter.ProviderNoDefault
			}));
			ModuleDefinition moduleDefinition2 = moduleDefinition;
			string text = "";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(6, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("DMD<");
			defaultInterpolatedStringHandler2.AppendFormatted(src.Name);
			defaultInterpolatedStringHandler2.AppendLiteral(">?");
			defaultInterpolatedStringHandler2.AppendFormatted<int>(this.GetHashCode());
			TypeDefinition typeDefinition = new TypeDefinition(text, defaultInterpolatedStringHandler2.ToStringAndClear(), Mono.Cecil.TypeAttributes.Public);
			moduleDefinition2.Types.Add(typeDefinition);
			MethodDefinition methodDefinition = new MethodDefinition(src.Name, Mono.Cecil.MethodAttributes.FamANDAssem | Mono.Cecil.MethodAttributes.Family | Mono.Cecil.MethodAttributes.Static | Mono.Cecil.MethodAttributes.HideBySig, moduleDefinition2.ImportReference(src.Definition.ReturnType));
			typeDefinition.Methods.Add(methodDefinition);
			MethodDefinition methodDefinition2;
			Definition = (methodDefinition2 = src.Definition.Clone(methodDefinition));
			methodDefinition = methodDefinition2;
			methodDefinition.DeclaringType = typeDefinition;
		}

		private void LoadFromMethod(MethodBase orig, out ModuleDefinition Module, out MethodDefinition def)
		{
			ParameterInfo[] parameters = orig.GetParameters();
			int num = 0;
			Type[] array;
			if (!orig.IsStatic)
			{
				num++;
				array = new Type[parameters.Length + 1];
				array[0] = orig.GetThisParamType();
			}
			else
			{
				array = new Type[parameters.Length];
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i + num] = parameters[i].ParameterType;
			}
			string id = orig.GetID(null, null, true, false, true);
			MethodInfo methodInfo = orig as MethodInfo;
			this._CreateDynModule(id, (methodInfo != null) ? methodInfo.ReturnType : null, array, out Module, out def);
			DynamicMethodDefinition._CopyMethodToDefinition(orig, def);
			if (!orig.IsStatic)
			{
				def.Parameters[0].Name = "this";
			}
			for (int j = 0; j < parameters.Length; j++)
			{
				def.Parameters[j + num].Name = parameters[j].Name;
			}
		}

		public MethodInfo Generate()
		{
			return this.Generate(null);
		}

		public MethodInfo Generate([Nullable(2)] object context)
		{
			object obj;
			string text = (Switches.TryGetSwitchValue("DMDType", out obj) ? (obj as string) : null);
			if (text != null)
			{
				if (text.Equals("dynamicmethod", StringComparison.OrdinalIgnoreCase) || text.Equals("dm", StringComparison.OrdinalIgnoreCase))
				{
					return DMDGenerator<DMDEmitDynamicMethodGenerator>.Generate(this, context);
				}
				if (text.Equals("cecil", StringComparison.OrdinalIgnoreCase) || text.Equals("md", StringComparison.OrdinalIgnoreCase))
				{
					return DMDGenerator<DMDCecilGenerator>.Generate(this, context);
				}
				if (text.Equals("methodbuilder", StringComparison.OrdinalIgnoreCase) || text.Equals("mb", StringComparison.OrdinalIgnoreCase))
				{
					return DMDGenerator<DMDEmitMethodBuilderGenerator>.Generate(this, context);
				}
			}
			if (text != null)
			{
				Type type = ReflectionHelper.GetType(text);
				if (type != null)
				{
					if (!DynamicMethodDefinition.t__IDMDGenerator.IsCompatible(type))
					{
						throw new ArgumentException("Invalid DMDGenerator type: " + text);
					}
					return DynamicMethodDefinition._DMDGeneratorCache.GetOrAdd(text, (string _) => (IDMDGenerator)Activator.CreateInstance(type)).Generate(this, context);
				}
			}
			if (DynamicMethodDefinition._PreferCecil)
			{
				return DMDGenerator<DMDCecilGenerator>.Generate(this, context);
			}
			if (this.Debug)
			{
				return DMDGenerator<DMDEmitMethodBuilderGenerator>.Generate(this, context);
			}
			if (this.Definition.Body.ExceptionHandlers.Any<Mono.Cecil.Cil.ExceptionHandler>(delegate(Mono.Cecil.Cil.ExceptionHandler eh)
			{
				ExceptionHandlerType handlerType = eh.HandlerType;
				return handlerType == ExceptionHandlerType.Filter || handlerType == ExceptionHandlerType.Fault;
			}))
			{
				return DMDGenerator<DMDEmitMethodBuilderGenerator>.Generate(this, context);
			}
			return DMDGenerator<DMDEmitDynamicMethodGenerator>.Generate(this, context);
		}

		public void Dispose()
		{
			if (this.isDisposed)
			{
				return;
			}
			this.isDisposed = true;
			ModuleDefinition module = this.Module;
			if (module == null)
			{
				return;
			}
			module.Dispose();
		}

		public string GetDumpName(string type)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 2);
			defaultInterpolatedStringHandler.AppendLiteral("DMDASM.");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.GUID.GetHashCode(), "X8");
			defaultInterpolatedStringHandler.AppendFormatted(string.IsNullOrEmpty(type) ? "" : ("." + type));
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		private static Mono.Cecil.Cil.OpCode[] _CecilOpCodes1X = null;

		private static Mono.Cecil.Cil.OpCode[] _CecilOpCodes2X = null;

		internal static readonly bool _IsNewMonoSRE = PlatformDetection.Runtime == RuntimeKind.Mono && typeof(DynamicMethod).GetField("il_info", BindingFlags.Instance | BindingFlags.NonPublic) != null;

		internal static readonly bool _IsOldMonoSRE = PlatformDetection.Runtime == RuntimeKind.Mono && !DynamicMethodDefinition._IsNewMonoSRE && typeof(DynamicMethod).GetField("ilgen", BindingFlags.Instance | BindingFlags.NonPublic) != null;

		private static bool _PreferCecil;

		internal static readonly ConstructorInfo c_DebuggableAttribute;

		internal static readonly ConstructorInfo c_UnverifiableCodeAttribute;

		internal static readonly ConstructorInfo c_IgnoresAccessChecksToAttribute;

		internal static readonly Type t__IDMDGenerator;

		internal static readonly ConcurrentDictionary<string, IDMDGenerator> _DMDGeneratorCache;

		private Guid GUID = Guid.NewGuid();

		private bool isDisposed;

		[NullableContext(0)]
		private enum TokenResolutionMode
		{
			Any,
			Type,
			Method,
			Field
		}
	}
}
