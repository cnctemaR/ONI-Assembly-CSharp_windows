using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mono.Cecil;
using MonoMod.Core.Interop;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class Core21Runtime : CoreBaseRuntime
	{
		public override RuntimeFeature Features
		{
			get
			{
				return base.Features | RuntimeFeature.CompileMethodHook;
			}
		}

		public Core21Runtime(ISystem system)
			: base(system)
		{
		}

		private static Core21Runtime.JitHookHelpersHolder CreateJitHookHelpers(Core21Runtime self)
		{
			return new Core21Runtime.JitHookHelpersHolder(self);
		}

		protected Core21Runtime.JitHookHelpersHolder JitHookHelpers
		{
			get
			{
				return Helpers.GetOrInitWithLock<Core21Runtime, Core21Runtime.JitHookHelpersHolder>(ref this.lazyJitHookHelpers, this.sync, Core21Runtime.createJitHookHelpersFunc, this);
			}
		}

		protected virtual Guid ExpectedJitVersion
		{
			get
			{
				return Core21Runtime.JitVersionGuid;
			}
		}

		protected virtual int VtableIndexICorJitCompilerGetVersionGuid
		{
			get
			{
				return 4;
			}
		}

		protected virtual int VtableIndexICorJitCompilerCompileMethod
		{
			get
			{
				return 0;
			}
		}

		protected virtual CoreCLR.InvokeCompileMethodPtr InvokeCompileMethodPtr
		{
			get
			{
				return CoreCLR.V21.InvokeCompileMethodPtr;
			}
		}

		protected virtual Delegate CastCompileHookToRealType(Delegate del)
		{
			return del.CastDelegate<CoreCLR.V21.CompileMethodDelegate>();
		}

		[NullableContext(0)]
		protected unsafe static IntPtr* GetVTableEntry(IntPtr @object, int index)
		{
			return *(IntPtr*)(void*)@object / (IntPtr)sizeof(IntPtr) + index * sizeof(IntPtr);
		}

		protected unsafe static IntPtr ReadObjectVTable(IntPtr @object, int index)
		{
			return *Core21Runtime.GetVTableEntry(@object, index);
		}

		protected unsafe void CheckVersionGuid(IntPtr jit)
		{
			delegate* unmanaged[Thiscall]<IntPtr, Guid*, void> system.Void_u0020(System.IntPtr,System.Guid*) = (void*)Core21Runtime.ReadObjectVTable(jit, this.VtableIndexICorJitCompilerGetVersionGuid);
			delegate* unmanaged[Thiscall]<IntPtr, Guid*, void> system.Void_u0020(System.IntPtr,System.Guid*)2 = system.Void_u0020(System.IntPtr,System.Guid*);
			Guid guid;
			calli(System.Void(System.IntPtr,System.Guid*), jit, &guid, system.Void_u0020(System.IntPtr,System.Guid*)2);
			bool flag = guid == this.ExpectedJitVersion;
			bool flag2 = flag;
			bool flag3;
			AssertionInterpolatedStringHandler assertionInterpolatedStringHandler = new AssertionInterpolatedStringHandler(66, 2, flag, out flag3);
			if (flag3)
			{
				assertionInterpolatedStringHandler.AppendLiteral("JIT version does not match expected JIT version! ");
				assertionInterpolatedStringHandler.AppendLiteral("expected: ");
				assertionInterpolatedStringHandler.AppendFormatted<Guid>(this.ExpectedJitVersion);
				assertionInterpolatedStringHandler.AppendLiteral(", got: ");
				assertionInterpolatedStringHandler.AppendFormatted<Guid>(guid);
			}
			Helpers.Assert(flag2, ref assertionInterpolatedStringHandler, "guid == ExpectedJitVersion");
		}

		protected unsafe override void InstallManagedJitHook(IntPtr jit)
		{
			this.CheckVersionGuid(jit);
			IntPtr* vtableEntry = Core21Runtime.GetVTableEntry(jit, this.VtableIndexICorJitCompilerCompileMethod);
			IntPtr intPtr = base.EHManagedToNative(*vtableEntry, out this.m2nHookHelper);
			Delegate @delegate = this.CastCompileHookToRealType(this.CreateCompileMethodDelegate(intPtr));
			this.ourCompileMethod = @delegate;
			IntPtr intPtr2 = base.EHNativeToManaged(Marshal.GetFunctionPointerForDelegate(@delegate), out this.n2mHookHelper);
			this.InvokeCompileMethodToPrepare(intPtr2);
			int num = sizeof(IntPtr);
			global::System.Span<byte> span = new global::System.Span<byte>(stackalloc byte[(UIntPtr)num], num);
			global::System.Runtime.InteropServices.MemoryMarshal.Write<IntPtr>(span, ref intPtr2);
			base.System.PatchData(PatchTargetKind.ReadOnly, (IntPtr)((void*)vtableEntry), span, default(global::System.Span<byte>));
		}

		protected unsafe virtual void InvokeCompileMethodToPrepare(IntPtr method)
		{
			delegate*<IntPtr, IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> invokeCompileMethod = this.InvokeCompileMethodPtr.InvokeCompileMethod;
			CoreCLR.V21.CORINFO_METHOD_INFO corinfo_METHOD_INFO;
			byte* ptr;
			uint num;
			CoreCLR.CorJitResult corJitResult = calli(MonoMod.Core.Interop.CoreCLR/CorJitResult(System.IntPtr,System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*), method, IntPtr.Zero, IntPtr.Zero, &corinfo_METHOD_INFO, 0U, &ptr, &num, invokeCompileMethod);
		}

		protected virtual Delegate CreateCompileMethodDelegate(IntPtr compileMethod)
		{
			return new <>f__AnonymousDelegate0(new Core21Runtime.JitHookDelegateHolder(this, this.InvokeCompileMethodPtr, compileMethod).CompileMethodHook);
		}

		protected virtual MethodInfo MakeCreateRuntimeMethodInfoStub(Type methodHandleInternal)
		{
			Type[] array = new Type[]
			{
				typeof(IntPtr),
				typeof(object)
			};
			Type type = typeof(RuntimeMethodHandle).Assembly.GetType("System.RuntimeMethodInfoStub");
			ConstructorInfo constructor = type.GetConstructor(array);
			MethodInfo methodInfo;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("new RuntimeMethodInfoStub", type, array))
			{
				ILGenerator ilgenerator = dynamicMethodDefinition.GetILGenerator();
				ilgenerator.Emit(OpCodes.Ldarg_0);
				ilgenerator.Emit(OpCodes.Ldarg_1);
				ilgenerator.Emit(OpCodes.Newobj, constructor);
				ilgenerator.Emit(OpCodes.Ret);
				methodInfo = dynamicMethodDefinition.Generate();
			}
			return methodInfo;
		}

		protected virtual MethodInfo GetOrCreateGetTypeFromHandleUnsafe()
		{
			MethodInfo method = typeof(Type).GetMethod("GetTypeFromHandleUnsafe", (BindingFlags)(-1));
			if (method != null)
			{
				return method;
			}
			Assembly assembly;
			using (ModuleDefinition moduleDefinition = ModuleDefinition.CreateModule("MonoMod.Core.Platforms.Runtimes.Core21Runtime+Helpers", new ModuleParameters
			{
				Kind = ModuleKind.Dll
			}))
			{
				TypeDefinition typeDefinition = new TypeDefinition("System", "Type", Mono.Cecil.TypeAttributes.Abstract)
				{
					BaseType = moduleDefinition.TypeSystem.Object
				};
				moduleDefinition.Types.Add(typeDefinition);
				MethodDefinition methodDefinition = new MethodDefinition("GetTypeFromHandleUnsafe", Mono.Cecil.MethodAttributes.FamANDAssem | Mono.Cecil.MethodAttributes.Family | Mono.Cecil.MethodAttributes.Static, moduleDefinition.ImportReference(typeof(Type)))
				{
					IsInternalCall = true
				};
				methodDefinition.Parameters.Add(new ParameterDefinition(moduleDefinition.ImportReference(typeof(IntPtr))));
				typeDefinition.Methods.Add(methodDefinition);
				assembly = ReflectionHelper.Load(moduleDefinition);
			}
			this.MakeAssemblySystemAssembly(assembly);
			return assembly.GetType("System.Type").GetMethod("GetTypeFromHandleUnsafe", (BindingFlags)(-1));
		}

		protected unsafe virtual void MakeAssemblySystemAssembly(Assembly assembly)
		{
			IntPtr intPtr = (IntPtr)Core21Runtime.RuntimeAssemblyPtrField.GetValue(assembly);
			int num = IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + 4 + IntPtr.Size + IntPtr.Size + 4 + 4 + IntPtr.Size + IntPtr.Size + 4 + 4 + IntPtr.Size;
			if (IntPtr.Size == 8)
			{
				num += 4;
			}
			IntPtr intPtr2 = *(IntPtr*)((byte*)(void*)intPtr + num);
			int num2 = IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size;
			IntPtr intPtr3 = *(IntPtr*)((byte*)(void*)intPtr2 + num2);
			int num3 = IntPtr.Size + (FxCoreBaseRuntime.IsDebugClr ? (IntPtr.Size + 4 + 4 + 4 + IntPtr.Size + 4) : 0) + IntPtr.Size + IntPtr.Size + 4 + 4 + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + 4;
			if (FxCoreBaseRuntime.IsDebugClr && IntPtr.Size == 8)
			{
				num3 += 8;
			}
			int* ptr = (int*)((byte*)(void*)intPtr3 + num3);
			*ptr |= 1;
		}

		private static readonly Func<Core21Runtime, Core21Runtime.JitHookHelpersHolder> createJitHookHelpersFunc = new Func<Core21Runtime, Core21Runtime.JitHookHelpersHolder>(Core21Runtime.CreateJitHookHelpers);

		private readonly object sync = new object();

		[Nullable(2)]
		private Core21Runtime.JitHookHelpersHolder lazyJitHookHelpers;

		private static readonly Guid JitVersionGuid = new Guid(195102408U, 33184, 16511, 153, 161, 146, 132, 72, 193, 235, 98);

		[Nullable(2)]
		private Delegate ourCompileMethod;

		[Nullable(2)]
		private IDisposable n2mHookHelper;

		[Nullable(2)]
		private IDisposable m2nHookHelper;

		private protected static readonly FieldInfo RuntimeAssemblyPtrField = Type.GetType("System.Reflection.RuntimeAssembly").GetField("m_assembly", BindingFlags.Instance | BindingFlags.NonPublic);

		[Nullable(0)]
		private sealed class JitHookDelegateHolder
		{
			public unsafe JitHookDelegateHolder(Core21Runtime runtime, CoreCLR.InvokeCompileMethodPtr icmp, IntPtr compileMethod)
			{
				this.Runtime = runtime;
				this.NativeExceptionHelper = runtime.NativeExceptionHelper;
				this.JitHookHelpers = runtime.JitHookHelpers;
				this.InvokeCompileMethodPtr = icmp;
				this.CompileMethodPtr = compileMethod;
				delegate*<IntPtr, IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> invokeCompileMethod = icmp.InvokeCompileMethod;
				CoreCLR.V21.CORINFO_METHOD_INFO corinfo_METHOD_INFO;
				byte* ptr;
				uint num;
				CoreCLR.CorJitResult corJitResult = calli(MonoMod.Core.Interop.CoreCLR/CorJitResult(System.IntPtr,System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, &corinfo_METHOD_INFO, 0U, &ptr, &num, invokeCompileMethod);
				MarshalEx.SetLastPInvokeError(MarshalEx.GetLastPInvokeError());
				INativeExceptionHelper nativeExceptionHelper = this.NativeExceptionHelper;
				if (nativeExceptionHelper != null)
				{
					this.GetNativeExceptionSlot = nativeExceptionHelper.GetExceptionSlot;
					this.GetNativeExceptionSlot();
				}
				int num2 = Core21Runtime.JitHookDelegateHolder.hookEntrancy;
				Core21Runtime.JitHookDelegateHolder.hookEntrancy = 0;
			}

			[NullableContext(0)]
			public unsafe CoreCLR.CorJitResult CompileMethodHook(IntPtr jit, IntPtr corJitInfo, CoreCLR.V21.CORINFO_METHOD_INFO* methodInfo, uint flags, byte** pNativeEntry, uint* pNativeSizeOfCode)
			{
				if (jit == IntPtr.Zero)
				{
					return CoreCLR.CorJitResult.CORJIT_OK;
				}
				*(IntPtr*)pNativeEntry = (IntPtr)((UIntPtr)0);
				*pNativeSizeOfCode = 0U;
				int lastPInvokeError = MarshalEx.GetLastPInvokeError();
				IntPtr intPtr = (IntPtr)0;
				GetExceptionSlot getNativeExceptionSlot = this.GetNativeExceptionSlot;
				IntPtr* ptr = ((getNativeExceptionSlot != null) ? getNativeExceptionSlot() : null);
				Core21Runtime.JitHookDelegateHolder.hookEntrancy++;
				CoreCLR.CorJitResult corJitResult2;
				try
				{
					delegate*<IntPtr, IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> invokeCompileMethod = this.InvokeCompileMethodPtr.InvokeCompileMethod;
					CoreCLR.CorJitResult corJitResult = calli(MonoMod.Core.Interop.CoreCLR/CorJitResult(System.IntPtr,System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*), this.CompileMethodPtr, jit, corJitInfo, methodInfo, flags, pNativeEntry, pNativeSizeOfCode, invokeCompileMethod);
					if (ptr != null && (intPtr = *ptr) != 0)
					{
						bool flag;
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(59, 1, out flag);
						if (flag)
						{
							debugLogWarningStringHandler.AppendLiteral("Native exception caught in JIT by exception helper (ex: 0x");
							debugLogWarningStringHandler.AppendFormatted<IntPtr>(intPtr, "x16");
							debugLogWarningStringHandler.AppendLiteral(")");
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
						corJitResult2 = corJitResult;
					}
					else
					{
						if (Core21Runtime.JitHookDelegateHolder.hookEntrancy == 1)
						{
							try
							{
								RuntimeTypeHandle[] array = null;
								RuntimeTypeHandle[] array2 = null;
								if (methodInfo->args.sigInst.classInst != null)
								{
									array = new RuntimeTypeHandle[methodInfo->args.sigInst.classInstCount];
									for (int i = 0; i < array.Length; i++)
									{
										array[i] = this.JitHookHelpers.GetTypeFromNativeHandle(methodInfo->args.sigInst.classInst[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]).TypeHandle;
									}
								}
								if (methodInfo->args.sigInst.methInst != null)
								{
									array2 = new RuntimeTypeHandle[methodInfo->args.sigInst.methInstCount];
									for (int j = 0; j < array2.Length; j++)
									{
										array2[j] = this.JitHookHelpers.GetTypeFromNativeHandle(methodInfo->args.sigInst.methInst[(IntPtr)j * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]).TypeHandle;
									}
								}
								RuntimeTypeHandle typeHandle = this.JitHookHelpers.GetDeclaringTypeOfMethodHandle(methodInfo->ftn).TypeHandle;
								RuntimeMethodHandle runtimeMethodHandle = this.JitHookHelpers.CreateHandleForHandlePointer(methodInfo->ftn);
								this.Runtime.OnMethodCompiledCore(typeHandle, runtimeMethodHandle, new global::System.ReadOnlyMemory<RuntimeTypeHandle>?(array), new global::System.ReadOnlyMemory<RuntimeTypeHandle>?(array2), (IntPtr)(*(IntPtr*)pNativeEntry), (IntPtr)(*(IntPtr*)pNativeEntry), (ulong)(*pNativeSizeOfCode));
							}
							catch
							{
							}
						}
						corJitResult2 = corJitResult;
					}
				}
				finally
				{
					Core21Runtime.JitHookDelegateHolder.hookEntrancy--;
					if (ptr != null)
					{
						*ptr = intPtr;
					}
					MarshalEx.SetLastPInvokeError(lastPInvokeError);
				}
				return corJitResult2;
			}

			public readonly Core21Runtime Runtime;

			[Nullable(2)]
			public readonly INativeExceptionHelper NativeExceptionHelper;

			[Nullable(2)]
			public readonly GetExceptionSlot GetNativeExceptionSlot;

			public readonly Core21Runtime.JitHookHelpersHolder JitHookHelpers;

			public readonly CoreCLR.InvokeCompileMethodPtr InvokeCompileMethodPtr;

			public readonly IntPtr CompileMethodPtr;

			[ThreadStatic]
			private static int hookEntrancy;
		}

		[NullableContext(0)]
		protected sealed class JitHookHelpersHolder
		{
			public RuntimeMethodHandle CreateHandleForHandlePointer(IntPtr handle)
			{
				return this.CreateRuntimeMethodHandle(this.CreateRuntimeMethodInfoStub(handle, this.MethodHandle_GetLoaderAllocator(handle)));
			}

			[NullableContext(1)]
			public JitHookHelpersHolder(Core21Runtime runtime)
			{
				MethodInfo method = typeof(RuntimeMethodHandle).GetMethod("GetLoaderAllocator", BindingFlags.Static | BindingFlags.NonPublic);
				MethodInfo methodInfo;
				using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("MethodHandle_GetLoaderAllocator", typeof(object), new Type[] { typeof(IntPtr) }))
				{
					ILGenerator ilgenerator = dynamicMethodDefinition.GetILGenerator();
					Type parameterType = method.GetParameters().First<ParameterInfo>().ParameterType;
					ilgenerator.Emit(OpCodes.Ldarga_S, 0);
					ilgenerator.Emit(OpCodes.Ldobj, parameterType);
					ilgenerator.Emit(OpCodes.Call, method);
					ilgenerator.Emit(OpCodes.Ret);
					methodInfo = dynamicMethodDefinition.Generate();
				}
				this.MethodHandle_GetLoaderAllocator = methodInfo.CreateDelegate<Core21Runtime.JitHookHelpersHolder.MethodHandle_GetLoaderAllocatorD>();
				MethodInfo orCreateGetTypeFromHandleUnsafe = runtime.GetOrCreateGetTypeFromHandleUnsafe();
				this.GetTypeFromNativeHandle = orCreateGetTypeFromHandleUnsafe.CreateDelegate<Core21Runtime.JitHookHelpersHolder.GetTypeFromNativeHandleD>();
				Type type = typeof(RuntimeMethodHandle).Assembly.GetType("System.RuntimeMethodHandleInternal");
				MethodInfo method2 = typeof(RuntimeMethodHandle).GetMethod("GetDeclaringType", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { type }, null);
				MethodInfo methodInfo2;
				using (DynamicMethodDefinition dynamicMethodDefinition2 = new DynamicMethodDefinition("GetDeclaringTypeOfMethodHandle", typeof(Type), new Type[] { typeof(IntPtr) }))
				{
					ILGenerator ilgenerator2 = dynamicMethodDefinition2.GetILGenerator();
					ilgenerator2.Emit(OpCodes.Ldarga_S, 0);
					ilgenerator2.Emit(OpCodes.Ldobj, type);
					ilgenerator2.Emit(OpCodes.Call, method2);
					ilgenerator2.Emit(OpCodes.Ret);
					methodInfo2 = dynamicMethodDefinition2.Generate();
				}
				this.GetDeclaringTypeOfMethodHandle = methodInfo2.CreateDelegate<Core21Runtime.JitHookHelpersHolder.GetDeclaringTypeOfMethodHandleD>();
				this.CreateRuntimeMethodInfoStub = runtime.MakeCreateRuntimeMethodInfoStub(type).CreateDelegate<Core21Runtime.JitHookHelpersHolder.CreateRuntimeMethodInfoStubD>();
				ConstructorInfo constructorInfo = typeof(RuntimeMethodHandle).GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).First<ConstructorInfo>();
				MethodInfo methodInfo3;
				using (DynamicMethodDefinition dynamicMethodDefinition3 = new DynamicMethodDefinition("new RuntimeMethodHandle", typeof(RuntimeMethodHandle), new Type[] { typeof(object) }))
				{
					ILGenerator ilgenerator3 = dynamicMethodDefinition3.GetILGenerator();
					ilgenerator3.Emit(OpCodes.Ldarg_0);
					ilgenerator3.Emit(OpCodes.Newobj, constructorInfo);
					ilgenerator3.Emit(OpCodes.Ret);
					methodInfo3 = dynamicMethodDefinition3.Generate();
				}
				this.CreateRuntimeMethodHandle = methodInfo3.CreateDelegate<Core21Runtime.JitHookHelpersHolder.CreateRuntimeMethodHandleD>();
			}

			[Nullable(1)]
			public readonly Core21Runtime.JitHookHelpersHolder.MethodHandle_GetLoaderAllocatorD MethodHandle_GetLoaderAllocator;

			[Nullable(1)]
			public readonly Core21Runtime.JitHookHelpersHolder.CreateRuntimeMethodInfoStubD CreateRuntimeMethodInfoStub;

			[Nullable(1)]
			public readonly Core21Runtime.JitHookHelpersHolder.CreateRuntimeMethodHandleD CreateRuntimeMethodHandle;

			[Nullable(1)]
			public readonly Core21Runtime.JitHookHelpersHolder.GetDeclaringTypeOfMethodHandleD GetDeclaringTypeOfMethodHandle;

			[Nullable(1)]
			public readonly Core21Runtime.JitHookHelpersHolder.GetTypeFromNativeHandleD GetTypeFromNativeHandle;

			public delegate object MethodHandle_GetLoaderAllocatorD(IntPtr methodHandle);

			public delegate object CreateRuntimeMethodInfoStubD(IntPtr methodHandle, object loaderAllocator);

			public delegate RuntimeMethodHandle CreateRuntimeMethodHandleD(object runtimeMethodInfo);

			public delegate Type GetDeclaringTypeOfMethodHandleD(IntPtr methodHandle);

			public delegate Type GetTypeFromNativeHandleD(IntPtr handle);
		}
	}
}
