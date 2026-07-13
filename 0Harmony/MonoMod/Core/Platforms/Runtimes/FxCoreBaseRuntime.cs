using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class FxCoreBaseRuntime : IRuntime
	{
		public abstract RuntimeKind Target { get; }

		public virtual RuntimeFeature Features
		{
			get
			{
				return RuntimeFeature.PreciseGC | RuntimeFeature.GenericSharing | RuntimeFeature.DisableInlining | RuntimeFeature.RequiresMethodIdentification | RuntimeFeature.RequiresBodyThunkWalking | RuntimeFeature.HasKnownABI | RuntimeFeature.RequiresCustomMethodCompile;
			}
		}

		public Abi Abi
		{
			get
			{
				Abi? abiCore = this.AbiCore;
				if (abiCore == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 1);
					defaultInterpolatedStringHandler.AppendLiteral("The runtime's Abi field is not set, and is unusable (");
					defaultInterpolatedStringHandler.AppendFormatted<Type>(base.GetType());
					defaultInterpolatedStringHandler.AppendLiteral(")");
					throw new PlatformNotSupportedException(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				return abiCore.GetValueOrDefault();
			}
		}

		private static TypeClassification ClassifyRyuJitX86(Type type, bool isReturn)
		{
			while (!type.IsPrimitive || type.IsEnum)
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (fields == null || fields.Length != 1)
				{
					break;
				}
				type = fields[0].FieldType;
			}
			TypeCode typeCode = Type.GetTypeCode(type);
			bool flag = typeCode == TypeCode.Boolean || typeCode - TypeCode.SByte <= 5;
			if (flag || type == typeof(IntPtr) || type == typeof(UIntPtr))
			{
				return TypeClassification.InRegister;
			}
			flag = isReturn;
			if (flag)
			{
				bool flag2 = typeCode - TypeCode.Int64 <= 1;
				flag = flag2;
			}
			if (flag)
			{
				return TypeClassification.InRegister;
			}
			flag = isReturn;
			if (flag)
			{
				bool flag2 = typeCode - TypeCode.Single <= 1;
				flag = flag2;
			}
			if (flag)
			{
				return TypeClassification.InRegister;
			}
			if (!isReturn)
			{
				return TypeClassification.OnStack;
			}
			int managedSize = type.GetManagedSize();
			flag = managedSize - 1 <= 1 || managedSize == 4;
			if (flag)
			{
				return TypeClassification.InRegister;
			}
			return TypeClassification.ByReference;
		}

		protected FxCoreBaseRuntime()
		{
			if (PlatformDetection.Architecture == ArchitectureKind.x86)
			{
				global::System.ReadOnlyMemory<SpecialArgumentKind> readOnlyMemory = new SpecialArgumentKind[]
				{
					SpecialArgumentKind.ThisPointer,
					SpecialArgumentKind.ReturnBuffer,
					SpecialArgumentKind.UserArguments,
					SpecialArgumentKind.GenericContext
				};
				Classifier classifier;
				if ((classifier = FxCoreBaseRuntime.<>O.<0>__ClassifyRyuJitX86) == null)
				{
					classifier = (FxCoreBaseRuntime.<>O.<0>__ClassifyRyuJitX86 = new Classifier(FxCoreBaseRuntime.ClassifyRyuJitX86));
				}
				this.AbiCore = new Abi?(new Abi(readOnlyMemory, classifier, true));
			}
		}

		protected static Abi AbiForCoreFx45X64(Abi baseAbi)
		{
			Abi abi = baseAbi;
			abi.ArgumentOrder = new SpecialArgumentKind[]
			{
				SpecialArgumentKind.ThisPointer,
				SpecialArgumentKind.ReturnBuffer,
				SpecialArgumentKind.GenericContext,
				SpecialArgumentKind.UserArguments
			};
			return abi;
		}

		protected static Abi AbiForCoreFx45ARM64(Abi baseAbi)
		{
			Abi abi = baseAbi;
			abi.ArgumentOrder = new SpecialArgumentKind[]
			{
				SpecialArgumentKind.ThisPointer,
				SpecialArgumentKind.GenericContext,
				SpecialArgumentKind.UserArguments
			};
			return abi;
		}

		public virtual MethodBase GetIdentifiable(MethodBase method)
		{
			if (FxCoreBaseRuntime.RTDynamicMethod_m_owner != null && method.GetType() == FxCoreBaseRuntime.RTDynamicMethod)
			{
				return (MethodBase)FxCoreBaseRuntime.RTDynamicMethod_m_owner.GetValue(method);
			}
			return method;
		}

		public virtual RuntimeMethodHandle GetMethodHandle(MethodBase method)
		{
			DynamicMethod dynamicMethod = method as DynamicMethod;
			if (dynamicMethod != null)
			{
				RuntimeMethodHandle runtimeMethodHandle;
				if (this.TryGetDMHandle(dynamicMethod, out runtimeMethodHandle) && this.TryInvokeBclCompileMethod(runtimeMethodHandle))
				{
					return runtimeMethodHandle;
				}
				try
				{
					dynamicMethod.CreateDelegate(typeof(MulticastDelegate));
				}
				catch
				{
				}
				if (this.TryGetDMHandle(dynamicMethod, out runtimeMethodHandle))
				{
					return runtimeMethodHandle;
				}
				if (FxCoreBaseRuntime._DynamicMethod_m_method != null)
				{
					return (RuntimeMethodHandle)FxCoreBaseRuntime._DynamicMethod_m_method.GetValue(method);
				}
			}
			return method.MethodHandle;
		}

		public bool RequiresGenericContext(MethodBase method)
		{
			Type type = method.DeclaringType ?? typeof(object);
			if (!method.IsGenericMethod && !type.IsGenericType)
			{
				return false;
			}
			IEnumerable<Type> enumerable = (method.IsGenericMethod ? method.GetGenericArguments() : Type.EmptyTypes);
			Func<Type, bool> func;
			if ((func = FxCoreBaseRuntime.<>O.<1>__IsGenericSharedType) == null)
			{
				func = (FxCoreBaseRuntime.<>O.<1>__IsGenericSharedType = new Func<Type, bool>(FxCoreBaseRuntime.IsGenericSharedType));
			}
			if (enumerable.Any<Type>(func))
			{
				return true;
			}
			if (!method.IsGenericMethod && !method.IsStatic && !type.IsValueType && (!type.IsInterface || method.IsAbstract))
			{
				return false;
			}
			IEnumerable<Type> enumerable2 = (type.IsGenericType ? type.GetGenericArguments() : Type.EmptyTypes);
			Func<Type, bool> func2;
			if ((func2 = FxCoreBaseRuntime.<>O.<1>__IsGenericSharedType) == null)
			{
				func2 = (FxCoreBaseRuntime.<>O.<1>__IsGenericSharedType = new Func<Type, bool>(FxCoreBaseRuntime.IsGenericSharedType));
			}
			return enumerable2.Any<Type>(func2);
		}

		private static bool IsGenericSharedType(Type type)
		{
			if (type.IsPrimitive)
			{
				return false;
			}
			if (!type.IsValueType)
			{
				return true;
			}
			if (type.IsGenericType)
			{
				IEnumerable<Type> genericArguments = type.GetGenericArguments();
				Func<Type, bool> func;
				if ((func = FxCoreBaseRuntime.<>O.<1>__IsGenericSharedType) == null)
				{
					func = (FxCoreBaseRuntime.<>O.<1>__IsGenericSharedType = new Func<Type, bool>(FxCoreBaseRuntime.IsGenericSharedType));
				}
				return genericArguments.Any<Type>(func);
			}
			return false;
		}

		private Func<DynamicMethod, RuntimeMethodHandle> GetDMHandleHelper
		{
			get
			{
				Func<DynamicMethod, RuntimeMethodHandle> func;
				if ((func = this.lazyGetDmHandleHelper) == null)
				{
					func = (this.lazyGetDmHandleHelper = FxCoreBaseRuntime.CreateGetDMHandleHelper());
				}
				return func;
			}
		}

		private static bool CanCreateGetDMHandleHelper
		{
			get
			{
				return FxCoreBaseRuntime._DynamicMethod_GetMethodDescriptor != null;
			}
		}

		private static Func<DynamicMethod, RuntimeMethodHandle> CreateGetDMHandleHelper()
		{
			Helpers.Assert(FxCoreBaseRuntime.CanCreateGetDMHandleHelper, null, "CanCreateGetDMHandleHelper");
			Func<DynamicMethod, RuntimeMethodHandle> func;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("get DynamicMethod RuntimeMethodHandle", typeof(RuntimeMethodHandle), new Type[] { typeof(DynamicMethod) }))
			{
				ModuleDefinition module = dynamicMethodDefinition.Module;
				ILProcessor ilprocessor = dynamicMethodDefinition.GetILProcessor();
				Helpers.Assert(FxCoreBaseRuntime._DynamicMethod_GetMethodDescriptor != null, null, "_DynamicMethod_GetMethodDescriptor is not null");
				ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
				ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Call, FxCoreBaseRuntime._DynamicMethod_GetMethodDescriptor);
				ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
				func = dynamicMethodDefinition.Generate().CreateDelegate<Func<DynamicMethod, RuntimeMethodHandle>>();
			}
			return func;
		}

		private Action<RuntimeMethodHandle> BclCompileMethodHelper
		{
			get
			{
				Action<RuntimeMethodHandle> action;
				if ((action = this.lazyBclCompileMethod) == null)
				{
					action = (this.lazyBclCompileMethod = FxCoreBaseRuntime.CreateBclCompileMethodHelper());
				}
				return action;
			}
		}

		private static bool CanCreateBclCompileMethodHelper
		{
			get
			{
				return FxCoreBaseRuntime._RuntimeHelpers__CompileMethod != null && (FxCoreBaseRuntime._RuntimeHelpers__CompileMethod_TakesIntPtr || (FxCoreBaseRuntime._RuntimeMethodHandle_m_value != null && (FxCoreBaseRuntime._RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo || (FxCoreBaseRuntime._IRuntimeMethodInfo_get_Value != null && FxCoreBaseRuntime._RuntimeHelpers__CompileMethod_TakesRuntimeMethodHandleInternal))));
			}
		}

		private static Action<RuntimeMethodHandle> CreateBclCompileMethodHelper()
		{
			Helpers.Assert(FxCoreBaseRuntime.CanCreateBclCompileMethodHelper, null, "CanCreateBclCompileMethodHelper");
			Action<RuntimeMethodHandle> action;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("invoke RuntimeHelpers.CompileMethod", null, new Type[] { typeof(RuntimeMethodHandle) }))
			{
				ModuleDefinition module = dynamicMethodDefinition.Module;
				ILProcessor ilprocessor = dynamicMethodDefinition.GetILProcessor();
				ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldarga_S, 0);
				if (FxCoreBaseRuntime._RuntimeHelpers__CompileMethod_TakesIntPtr)
				{
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Call, module.ImportReference(FxCoreBaseRuntime._RuntimeMethodHandle_get_Value));
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Call, module.ImportReference(FxCoreBaseRuntime._RuntimeHelpers__CompileMethod));
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
					action = dynamicMethodDefinition.Generate().CreateDelegate<Action<RuntimeMethodHandle>>();
				}
				else
				{
					Helpers.Assert(FxCoreBaseRuntime._RuntimeMethodHandle_m_value != null, null, "_RuntimeMethodHandle_m_value is not null");
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldfld, module.ImportReference(FxCoreBaseRuntime._RuntimeMethodHandle_m_value));
					if (FxCoreBaseRuntime._RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo)
					{
						ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Call, module.ImportReference(FxCoreBaseRuntime._RuntimeHelpers__CompileMethod));
						ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
						action = dynamicMethodDefinition.Generate().CreateDelegate<Action<RuntimeMethodHandle>>();
					}
					else
					{
						Helpers.Assert(FxCoreBaseRuntime._IRuntimeMethodInfo_get_Value != null, null, "_IRuntimeMethodInfo_get_Value is not null");
						ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Callvirt, module.ImportReference(FxCoreBaseRuntime._IRuntimeMethodInfo_get_Value));
						if (!FxCoreBaseRuntime._RuntimeHelpers__CompileMethod_TakesRuntimeMethodHandleInternal)
						{
							Helpers.Assert(false, "Tried to generate BCL CompileMethod helper when it's not possible? (This should never happen if CanCreateBclCompileMethodHelper is correct)", "false");
							throw new InvalidOperationException("UNREACHABLE");
						}
						ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Call, module.ImportReference(FxCoreBaseRuntime._RuntimeHelpers__CompileMethod));
						ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
						action = dynamicMethodDefinition.Generate().CreateDelegate<Action<RuntimeMethodHandle>>();
					}
				}
			}
			return action;
		}

		private bool TryGetDMHandle(DynamicMethod dm, out RuntimeMethodHandle handle)
		{
			if (FxCoreBaseRuntime.CanCreateGetDMHandleHelper)
			{
				handle = this.GetDMHandleHelper(dm);
				return true;
			}
			return FxCoreBaseRuntime.TryGetDMHandleRefl(dm, out handle);
		}

		protected bool TryInvokeBclCompileMethod(RuntimeMethodHandle handle)
		{
			if (FxCoreBaseRuntime.CanCreateBclCompileMethodHelper)
			{
				this.BclCompileMethodHelper(handle);
				return true;
			}
			return FxCoreBaseRuntime.TryInvokeBclCompileMethodRefl(handle);
		}

		private static bool TryGetDMHandleRefl(DynamicMethod dm, out RuntimeMethodHandle handle)
		{
			handle = default(RuntimeMethodHandle);
			if (FxCoreBaseRuntime._DynamicMethod_GetMethodDescriptor == null)
			{
				return false;
			}
			handle = (RuntimeMethodHandle)FxCoreBaseRuntime._DynamicMethod_GetMethodDescriptor.Invoke(dm, null);
			return true;
		}

		private static bool TryInvokeBclCompileMethodRefl(RuntimeMethodHandle handle)
		{
			if (FxCoreBaseRuntime._RuntimeHelpers__CompileMethod == null)
			{
				return false;
			}
			if (FxCoreBaseRuntime._RuntimeHelpers__CompileMethod_TakesIntPtr)
			{
				FxCoreBaseRuntime._RuntimeHelpers__CompileMethod.Invoke(null, new object[] { handle.Value });
				return true;
			}
			if (FxCoreBaseRuntime._RuntimeMethodHandle_m_value == null)
			{
				return false;
			}
			object value = FxCoreBaseRuntime._RuntimeMethodHandle_m_value.GetValue(handle);
			if (FxCoreBaseRuntime._RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo)
			{
				FxCoreBaseRuntime._RuntimeHelpers__CompileMethod.Invoke(null, new object[] { value });
				return true;
			}
			if (FxCoreBaseRuntime._IRuntimeMethodInfo_get_Value == null)
			{
				return false;
			}
			object obj = FxCoreBaseRuntime._IRuntimeMethodInfo_get_Value.Invoke(value, null);
			if (FxCoreBaseRuntime._RuntimeHelpers__CompileMethod_TakesRuntimeMethodHandleInternal)
			{
				FxCoreBaseRuntime._RuntimeHelpers__CompileMethod.Invoke(null, new object[] { obj });
				return true;
			}
			bool flag;
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(81, 1, out flag);
			if (flag)
			{
				debugLogErrorStringHandler.AppendLiteral("Could not compile DynamicMethod using BCL reflection (_CompileMethod first arg: ");
				debugLogErrorStringHandler.AppendFormatted<Type>(FxCoreBaseRuntime.RtH_CM_FirstArg);
				debugLogErrorStringHandler.AppendLiteral(")");
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
			return false;
		}

		public virtual void Compile(MethodBase method)
		{
			RuntimeMethodHandle methodHandle = this.GetMethodHandle(method);
			RuntimeHelpers.PrepareMethod(methodHandle);
			Helpers.Assert(this.TryInvokeBclCompileMethod(methodHandle), null, "TryInvokeBclCompileMethod(handle)");
			if (method.IsVirtual)
			{
				Type declaringType = method.DeclaringType;
				if (declaringType != null && declaringType.IsValueType)
				{
					if (this.TryGetCanonicalMethodHandle(ref methodHandle))
					{
						Helpers.Assert(this.TryInvokeBclCompileMethod(methodHandle), null, "TryInvokeBclCompileMethod(handle)");
						return;
					}
					try
					{
						method.CreateDelegate<Action>();
					}
					catch (Exception ex)
					{
						bool flag;
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogSpamStringHandler debugLogSpamStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogSpamStringHandler(91, 1, out flag);
						if (flag)
						{
							debugLogSpamStringHandler.AppendLiteral("Caught exception while attempting to compile real entry point of virtual method on struct: ");
							debugLogSpamStringHandler.AppendFormatted<Exception>(ex);
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Spam(ref debugLogSpamStringHandler);
					}
				}
			}
		}

		protected virtual bool TryGetCanonicalMethodHandle(ref RuntimeMethodHandle handle)
		{
			return false;
		}

		[return: Nullable(2)]
		public virtual IDisposable PinMethodIfNeeded(MethodBase method)
		{
			return null;
		}

		public unsafe virtual void DisableInlining(MethodBase method)
		{
			RuntimeMethodHandle methodHandle = this.GetMethodHandle(method);
			int num = (FxCoreBaseRuntime.IsDebugClr ? (IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size) : 0) + 2 + 1 + 1 + 2;
			ushort* ptr = (ushort*)((byte*)(void*)methodHandle.Value + num);
			ushort* ptr2 = ptr;
			*ptr2 |= 8192;
		}

		public virtual IntPtr GetMethodEntryPoint(MethodBase method)
		{
			method = this.GetIdentifiable(method);
			if (method.IsVirtual)
			{
				Type declaringType = method.DeclaringType;
				if (declaringType != null && declaringType.IsValueType)
				{
					return method.GetLdftnPointer();
				}
			}
			return this.GetMethodHandle(method).GetFunctionPointer();
		}

		[Nullable(2)]
		[method: NullableContext(2)]
		[field: Nullable(2)]
		public event OnMethodCompiledCallback OnMethodCompiled;

		[NullableContext(0)]
		protected unsafe virtual void OnMethodCompiledCore(RuntimeTypeHandle declaringType, RuntimeMethodHandle methodHandle, global::System.ReadOnlyMemory<RuntimeTypeHandle>? genericTypeArguments, global::System.ReadOnlyMemory<RuntimeTypeHandle>? genericMethodArguments, IntPtr methodBodyStart, IntPtr methodBodyRw, ulong methodBodySize)
		{
			try
			{
				Type type = Type.GetTypeFromHandle(declaringType);
				if (genericTypeArguments != null)
				{
					global::System.ReadOnlyMemory<RuntimeTypeHandle> valueOrDefault = genericTypeArguments.GetValueOrDefault();
					if (type.IsGenericTypeDefinition)
					{
						Type[] array = new Type[valueOrDefault.Length];
						for (int i = 0; i < valueOrDefault.Length; i++)
						{
							array[i] = Type.GetTypeFromHandle(*valueOrDefault.Span[i]);
						}
						type = type.MakeGenericType(array);
					}
				}
				MethodBase methodBase = MethodBase.GetMethodFromHandle(methodHandle, type.TypeHandle);
				if (methodBase == null)
				{
					foreach (MethodInfo methodInfo in type.GetMethods((BindingFlags)(-1)))
					{
						if (methodInfo.MethodHandle.Value == methodHandle.Value)
						{
							methodBase = methodInfo;
							break;
						}
					}
				}
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogSpamStringHandler debugLogSpamStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogSpamStringHandler(28, 3, out flag);
				if (flag)
				{
					debugLogSpamStringHandler.AppendLiteral("JIT compiled ");
					debugLogSpamStringHandler.AppendFormatted<MethodBase>(methodBase);
					debugLogSpamStringHandler.AppendLiteral(" to 0x");
					debugLogSpamStringHandler.AppendFormatted<IntPtr>(methodBodyStart, "x16");
					debugLogSpamStringHandler.AppendLiteral(" (rw: 0x");
					debugLogSpamStringHandler.AppendFormatted<IntPtr>(methodBodyRw, "x16");
					debugLogSpamStringHandler.AppendLiteral(")");
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Spam(ref debugLogSpamStringHandler);
				try
				{
					OnMethodCompiledCallback onMethodCompiled = this.OnMethodCompiled;
					if (onMethodCompiled != null)
					{
						onMethodCompiled(methodHandle, methodBase, methodBodyStart, methodBodyRw, methodBodySize);
					}
				}
				catch (Exception ex)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(40, 1, out flag);
					if (flag)
					{
						debugLogErrorStringHandler.AppendLiteral("Error executing OnMethodCompiled event: ");
						debugLogErrorStringHandler.AppendFormatted<Exception>(ex);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
				}
			}
			catch (Exception ex2)
			{
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler2 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(31, 1, out flag);
				if (flag)
				{
					debugLogErrorStringHandler2.AppendLiteral("Error in OnMethodCompiledCore: ");
					debugLogErrorStringHandler2.AppendFormatted<Exception>(ex2);
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler2);
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static FxCoreBaseRuntime()
		{
			Type rtdynamicMethod = FxCoreBaseRuntime.RTDynamicMethod;
			FxCoreBaseRuntime.RTDynamicMethod_m_owner = ((rtdynamicMethod != null) ? rtdynamicMethod.GetField("m_owner", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : null);
			FxCoreBaseRuntime._DynamicMethod_m_method = typeof(DynamicMethod).GetField("m_method", BindingFlags.Instance | BindingFlags.NonPublic);
			FxCoreBaseRuntime._DynamicMethod_GetMethodDescriptor = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", BindingFlags.Instance | BindingFlags.NonPublic);
			FxCoreBaseRuntime._RuntimeMethodHandle_get_Value = typeof(RuntimeMethodHandle).GetMethod("get_Value", BindingFlags.Instance | BindingFlags.Public);
			FxCoreBaseRuntime._RuntimeMethodHandle_m_value = typeof(RuntimeMethodHandle).GetField("m_value", BindingFlags.Instance | BindingFlags.NonPublic);
			Type type = typeof(RuntimeMethodHandle).Assembly.GetType("System.IRuntimeMethodInfo");
			FxCoreBaseRuntime._IRuntimeMethodInfo_get_Value = ((type != null) ? type.GetMethod("get_Value") : null);
			FxCoreBaseRuntime._RuntimeHelpers__CompileMethod = typeof(RuntimeHelpers).GetMethod("_CompileMethod", BindingFlags.Static | BindingFlags.NonPublic) ?? typeof(RuntimeHelpers).GetMethod("CompileMethod", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo runtimeHelpers__CompileMethod = FxCoreBaseRuntime._RuntimeHelpers__CompileMethod;
			FxCoreBaseRuntime.RtH_CM_FirstArg = ((runtimeHelpers__CompileMethod != null) ? runtimeHelpers__CompileMethod.GetParameters()[0].ParameterType : null);
			Type rtH_CM_FirstArg = FxCoreBaseRuntime.RtH_CM_FirstArg;
			FxCoreBaseRuntime._RuntimeHelpers__CompileMethod_TakesIntPtr = ((rtH_CM_FirstArg != null) ? rtH_CM_FirstArg.FullName : null) == "System.IntPtr";
			Type rtH_CM_FirstArg2 = FxCoreBaseRuntime.RtH_CM_FirstArg;
			FxCoreBaseRuntime._RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo = ((rtH_CM_FirstArg2 != null) ? rtH_CM_FirstArg2.FullName : null) == "System.IRuntimeMethodInfo";
			Type rtH_CM_FirstArg3 = FxCoreBaseRuntime.RtH_CM_FirstArg;
			FxCoreBaseRuntime._RuntimeHelpers__CompileMethod_TakesRuntimeMethodHandleInternal = ((rtH_CM_FirstArg3 != null) ? rtH_CM_FirstArg3.FullName : null) == "System.RuntimeMethodHandleInternal";
			bool flag;
			FxCoreBaseRuntime.IsDebugClr = Switches.TryGetSwitchEnabled("DebugClr", out flag) && flag;
		}

		protected Abi? AbiCore;

		[Nullable(2)]
		private static readonly Type RTDynamicMethod = typeof(DynamicMethod).GetNestedType("RTDynamicMethod", BindingFlags.NonPublic);

		[Nullable(2)]
		private static readonly FieldInfo RTDynamicMethod_m_owner;

		[Nullable(2)]
		private static readonly FieldInfo _DynamicMethod_m_method;

		[Nullable(2)]
		private static readonly MethodInfo _DynamicMethod_GetMethodDescriptor;

		[Nullable(2)]
		private static readonly MethodInfo _RuntimeMethodHandle_get_Value;

		[Nullable(2)]
		private static readonly FieldInfo _RuntimeMethodHandle_m_value;

		[Nullable(2)]
		private static readonly MethodInfo _IRuntimeMethodInfo_get_Value;

		[Nullable(2)]
		private static readonly MethodInfo _RuntimeHelpers__CompileMethod;

		[Nullable(2)]
		private static readonly Type RtH_CM_FirstArg;

		private static readonly bool _RuntimeHelpers__CompileMethod_TakesIntPtr;

		private static readonly bool _RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo;

		private static readonly bool _RuntimeHelpers__CompileMethod_TakesRuntimeMethodHandleInternal;

		[Nullable(new byte[] { 2, 1 })]
		private Func<DynamicMethod, RuntimeMethodHandle> lazyGetDmHandleHelper;

		[Nullable(2)]
		private Action<RuntimeMethodHandle> lazyBclCompileMethod;

		protected static readonly bool IsDebugClr;

		[CompilerGenerated]
		private static class <>O
		{
			[Nullable(0)]
			public static Classifier <0>__ClassifyRyuJitX86;

			[Nullable(0)]
			public static Func<Type, bool> <1>__IsGenericSharedType;
		}
	}
}
