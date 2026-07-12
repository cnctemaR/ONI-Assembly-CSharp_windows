using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	public class DetourRuntimeNETCore30Platform : DetourRuntimeNETCorePlatform
	{
		protected unsafe override void DisableInlining(MethodBase method, RuntimeMethodHandle handle)
		{
			ushort* ptr = (ushort*)((byte*)(void*)handle.Value + 6);
			ushort* ptr2 = ptr;
			*ptr2 |= 8192;
		}

		private IntPtr GetCompileMethod(IntPtr jit)
		{
			return DetourRuntimeNETCorePlatform.ReadObjectVTable(jit, this.VTableIndex_ICorJitCompiler_compileMethod);
		}

		public override bool OnMethodCompiledWillBeCalled
		{
			get
			{
				return true;
			}
		}

		protected unsafe virtual DetourRuntimeNETCore30Platform.CorJitResult InvokeRealCompileMethod(IntPtr thisPtr, IntPtr corJitInfo, in DetourRuntimeNETCore30Platform.CORINFO_METHOD_INFO methodInfo, uint flags, out byte* nativeEntry, out uint nativeSizeOfCode)
		{
			nativeEntry = (IntPtr)((UIntPtr)0);
			nativeSizeOfCode = 0U;
			if (this.real_compileMethod == null)
			{
				return DetourRuntimeNETCore30Platform.CorJitResult.CORJIT_OK;
			}
			return this.real_compileMethod(thisPtr, corJitInfo, in methodInfo, flags, out nativeEntry, out nativeSizeOfCode);
		}

		protected unsafe virtual IntPtr GetCompileMethodHook(IntPtr real)
		{
			this.real_compileMethod = real.AsDelegate<DetourRuntimeNETCore30Platform.d_compileMethod>();
			this.our_compileMethod = new DetourRuntimeNETCore30Platform.d_compileMethod(this.CompileMethodHook);
			IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate<DetourRuntimeNETCore30Platform.d_compileMethod>(this.our_compileMethod);
			NativeDetourData nativeDetourData = DetourRuntimeNETCore30Platform.CreateNativeTrampolineTo(functionPointerForDelegate);
			DetourRuntimeNETCore30Platform.d_compileMethod d_compileMethod = nativeDetourData.Method.AsDelegate<DetourRuntimeNETCore30Platform.d_compileMethod>();
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			DetourRuntimeNETCore30Platform.CORINFO_METHOD_INFO corinfo_METHOD_INFO = default(DetourRuntimeNETCore30Platform.CORINFO_METHOD_INFO);
			byte* ptr;
			uint num;
			d_compileMethod(zero, zero2, in corinfo_METHOD_INFO, 0U, out ptr, out num);
			DetourRuntimeNETCore30Platform.FreeNativeTrampoline(nativeDetourData);
			return functionPointerForDelegate;
		}

		protected unsafe override void InstallJitHooks(IntPtr jit)
		{
			this.SetupJitHookHelpers();
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			DetourRuntimeNETCore30Platform.CORINFO_METHOD_INFO corinfo_METHOD_INFO = default(DetourRuntimeNETCore30Platform.CORINFO_METHOD_INFO);
			byte* ptr;
			uint num;
			this.InvokeRealCompileMethod(zero, zero2, in corinfo_METHOD_INFO, 0U, out ptr, out num);
			IntPtr compileMethodHook = this.GetCompileMethodHook(this.GetCompileMethod(jit));
			int num2 = DetourRuntimeNETCore30Platform.hookEntrancy;
			IntPtr* vtableEntry = DetourRuntimeNETCorePlatform.GetVTableEntry(jit, this.VTableIndex_ICorJitCompiler_compileMethod);
			DetourHelper.Native.MakeWritable((IntPtr)((void*)vtableEntry), (uint)IntPtr.Size);
			this.real_compileMethodPtr = *vtableEntry;
			*vtableEntry = compileMethodHook;
		}

		protected static NativeDetourData CreateNativeTrampolineTo(IntPtr target)
		{
			IntPtr intPtr = DetourHelper.Native.MemAlloc(64U);
			NativeDetourData nativeDetourData = DetourHelper.Native.Create(intPtr, target, null);
			DetourHelper.Native.MakeWritable(nativeDetourData);
			DetourHelper.Native.Apply(nativeDetourData);
			DetourHelper.Native.MakeExecutable(nativeDetourData);
			DetourHelper.Native.FlushICache(nativeDetourData);
			return nativeDetourData;
		}

		protected static void FreeNativeTrampoline(NativeDetourData data)
		{
			DetourHelper.Native.MakeWritable(data);
			DetourHelper.Native.MemFree(data.Method);
			DetourHelper.Native.Free(data);
		}

		protected unsafe DetourRuntimeNETCore30Platform.CorJitResult CompileMethodHook(IntPtr jit, IntPtr corJitInfo, in DetourRuntimeNETCore30Platform.CORINFO_METHOD_INFO methodInfo, uint flags, out byte* nativeEntry, out uint nativeSizeOfCode)
		{
			nativeEntry = (IntPtr)((UIntPtr)0);
			nativeSizeOfCode = 0U;
			if (jit == IntPtr.Zero)
			{
				return DetourRuntimeNETCore30Platform.CorJitResult.CORJIT_OK;
			}
			DetourRuntimeNETCore30Platform.hookEntrancy++;
			DetourRuntimeNETCore30Platform.CorJitResult corJitResult2;
			try
			{
				DetourRuntimeNETCore30Platform.CorJitResult corJitResult = this.InvokeRealCompileMethod(jit, corJitInfo, in methodInfo, flags, out nativeEntry, out nativeSizeOfCode);
				if (DetourRuntimeNETCore30Platform.hookEntrancy == 1)
				{
					try
					{
						RuntimeTypeHandle[] array = null;
						RuntimeTypeHandle[] array2 = null;
						if (methodInfo.args.sigInst.classInst != null)
						{
							array = new RuntimeTypeHandle[methodInfo.args.sigInst.classInstCount];
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = this.GetTypeFromNativeHandle(methodInfo.args.sigInst.classInst[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]).TypeHandle;
							}
						}
						if (methodInfo.args.sigInst.methInst != null)
						{
							array2 = new RuntimeTypeHandle[methodInfo.args.sigInst.methInstCount];
							for (int j = 0; j < array2.Length; j++)
							{
								array2[j] = this.GetTypeFromNativeHandle(methodInfo.args.sigInst.methInst[(IntPtr)j * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]).TypeHandle;
							}
						}
						RuntimeTypeHandle typeHandle = this.GetDeclaringTypeOfMethodHandle(methodInfo.ftn).TypeHandle;
						RuntimeMethodHandle runtimeMethodHandle = this.CreateHandleForHandlePointer(methodInfo.ftn);
						this.JitHookCore(typeHandle, runtimeMethodHandle, (IntPtr)nativeEntry, (ulong)nativeSizeOfCode, array, array2);
					}
					catch
					{
					}
				}
				corJitResult2 = corJitResult;
			}
			finally
			{
				DetourRuntimeNETCore30Platform.hookEntrancy--;
			}
			return corJitResult2;
		}

		protected RuntimeMethodHandle CreateHandleForHandlePointer(IntPtr handle)
		{
			return this.CreateRuntimeMethodHandle(this.CreateRuntimeMethodInfoStub(handle, this.MethodHandle_GetLoaderAllocator(handle)));
		}

		protected virtual void SetupJitHookHelpers()
		{
			MethodInfo methodInfo = typeof(object).Assembly.GetType("Internal.Runtime.CompilerServices.Unsafe").GetMethods().First<MethodInfo>((MethodInfo m) => m.Name == "As" && m.ReturnType.IsByRef);
			MethodInfo method = typeof(RuntimeMethodHandle).GetMethod("GetLoaderAllocator", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo methodInfo2;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("MethodHandle_GetLoaderAllocator", typeof(object), new Type[] { typeof(IntPtr) }))
			{
				ILProcessor ilprocessor = dynamicMethodDefinition.GetILProcessor();
				ModuleDefinition module = ilprocessor.Body.Method.Module;
				Type parameterType = method.GetParameters().First<ParameterInfo>().ParameterType;
				ilprocessor.Emit(OpCodes.Ldarga_S, ilprocessor.Body.Method.Parameters[0]);
				ilprocessor.Emit(OpCodes.Call, module.ImportReference(methodInfo.MakeGenericMethod(new Type[]
				{
					typeof(IntPtr),
					parameterType
				})));
				ilprocessor.Emit(OpCodes.Ldobj, module.ImportReference(parameterType));
				ilprocessor.Emit(OpCodes.Call, module.ImportReference(method));
				ilprocessor.Emit(OpCodes.Ret);
				methodInfo2 = dynamicMethodDefinition.Generate();
			}
			this.MethodHandle_GetLoaderAllocator = methodInfo2.CreateDelegate<DetourRuntimeNETCore30Platform.d_MethodHandle_GetLoaderAllocator>();
			MethodInfo orCreateGetTypeFromHandleUnsafe = this.GetOrCreateGetTypeFromHandleUnsafe();
			this.GetTypeFromNativeHandle = orCreateGetTypeFromHandleUnsafe.CreateDelegate<DetourRuntimeNETCore30Platform.d_GetTypeFromNativeHandle>();
			Type type = typeof(RuntimeMethodHandle).Assembly.GetType("System.RuntimeMethodHandleInternal");
			MethodInfo method2 = typeof(RuntimeMethodHandle).GetMethod("GetDeclaringType", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { type }, null);
			MethodInfo methodInfo3;
			using (DynamicMethodDefinition dynamicMethodDefinition2 = new DynamicMethodDefinition("GetDeclaringTypeOfMethodHandle", typeof(Type), new Type[] { typeof(IntPtr) }))
			{
				ILProcessor ilprocessor2 = dynamicMethodDefinition2.GetILProcessor();
				ModuleDefinition module2 = ilprocessor2.Body.Method.Module;
				ilprocessor2.Emit(OpCodes.Ldarga_S, ilprocessor2.Body.Method.Parameters[0]);
				ilprocessor2.Emit(OpCodes.Call, module2.ImportReference(methodInfo.MakeGenericMethod(new Type[]
				{
					typeof(IntPtr),
					type
				})));
				ilprocessor2.Emit(OpCodes.Ldobj, module2.ImportReference(type));
				ilprocessor2.Emit(OpCodes.Call, module2.ImportReference(method2));
				ilprocessor2.Emit(OpCodes.Ret);
				methodInfo3 = dynamicMethodDefinition2.Generate();
			}
			this.GetDeclaringTypeOfMethodHandle = methodInfo3.CreateDelegate<DetourRuntimeNETCore30Platform.d_GetDeclaringTypeOfMethodHandle>();
			Type[] array = new Type[]
			{
				typeof(IntPtr),
				typeof(object)
			};
			Type type2 = typeof(RuntimeMethodHandle).Assembly.GetType("System.RuntimeMethodInfoStub");
			ConstructorInfo constructor = type2.GetConstructor(array);
			MethodInfo methodInfo4;
			using (DynamicMethodDefinition dynamicMethodDefinition3 = new DynamicMethodDefinition("new RuntimeMethodInfoStub", type2, array))
			{
				ILProcessor ilprocessor3 = dynamicMethodDefinition3.GetILProcessor();
				ModuleDefinition module3 = ilprocessor3.Body.Method.Module;
				ilprocessor3.Emit(OpCodes.Ldarg_0);
				ilprocessor3.Emit(OpCodes.Ldarg_1);
				ilprocessor3.Emit(OpCodes.Newobj, module3.ImportReference(constructor));
				ilprocessor3.Emit(OpCodes.Ret);
				methodInfo4 = dynamicMethodDefinition3.Generate();
			}
			this.CreateRuntimeMethodInfoStub = methodInfo4.CreateDelegate<DetourRuntimeNETCore30Platform.d_CreateRuntimeMethodInfoStub>();
			ConstructorInfo constructorInfo = typeof(RuntimeMethodHandle).GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).First<ConstructorInfo>();
			MethodInfo methodInfo5;
			using (DynamicMethodDefinition dynamicMethodDefinition4 = new DynamicMethodDefinition("new RuntimeMethodHandle", typeof(RuntimeMethodHandle), new Type[] { typeof(object) }))
			{
				ILProcessor ilprocessor4 = dynamicMethodDefinition4.GetILProcessor();
				ModuleDefinition module4 = ilprocessor4.Body.Method.Module;
				ilprocessor4.Emit(OpCodes.Ldarg_0);
				ilprocessor4.Emit(OpCodes.Newobj, module4.ImportReference(constructorInfo));
				ilprocessor4.Emit(OpCodes.Ret);
				methodInfo5 = dynamicMethodDefinition4.Generate();
			}
			this.CreateRuntimeMethodHandle = methodInfo5.CreateDelegate<DetourRuntimeNETCore30Platform.d_CreateRuntimeMethodHandle>();
		}

		private MethodInfo GetOrCreateGetTypeFromHandleUnsafe()
		{
			if (this._getTypeFromHandleUnsafeMethod != null)
			{
				return this._getTypeFromHandleUnsafeMethod;
			}
			Assembly assembly;
			using (ModuleDefinition moduleDefinition = ModuleDefinition.CreateModule("MonoMod.RuntimeDetour.Runtime.NETCore3+Helpers", new ModuleParameters
			{
				Kind = ModuleKind.Dll
			}))
			{
				TypeDefinition typeDefinition = new TypeDefinition("System", "Type", Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Abstract)
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
			return this._getTypeFromHandleUnsafeMethod = assembly.GetType("System.Type").GetMethod("GetTypeFromHandleUnsafe");
		}

		protected unsafe virtual void MakeAssemblySystemAssembly(Assembly assembly)
		{
			IntPtr intPtr = (IntPtr)DetourRuntimeNETCore30Platform._runtimeAssemblyPtrField.GetValue(assembly);
			int num = IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + 4 + IntPtr.Size + IntPtr.Size + 4 + 4 + IntPtr.Size + IntPtr.Size + 4 + 4 + IntPtr.Size;
			if (IntPtr.Size == 8)
			{
				num += 4;
			}
			IntPtr intPtr2 = *(IntPtr*)((byte*)(void*)intPtr + num);
			int num2 = IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size;
			IntPtr intPtr3 = *(IntPtr*)((byte*)(void*)intPtr2 + num2);
			int num3 = IntPtr.Size + IntPtr.Size + IntPtr.Size + 4 + 4 + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + 4;
			int* ptr = (int*)((byte*)(void*)intPtr3 + num3);
			*ptr |= 1;
		}

		protected void HookPermanent(MethodBase from, MethodBase to)
		{
			this.Pin(from);
			this.Pin(to);
			this.HookPermanent(this.GetNativeStart(from), this.GetNativeStart(to));
		}

		protected void HookPermanent(IntPtr from, IntPtr to)
		{
			NativeDetourData nativeDetourData = DetourHelper.Native.Create(from, to, null);
			DetourHelper.Native.MakeWritable(nativeDetourData);
			DetourHelper.Native.Apply(nativeDetourData);
			DetourHelper.Native.MakeExecutable(nativeDetourData);
			DetourHelper.Native.FlushICache(nativeDetourData);
			DetourHelper.Native.Free(nativeDetourData);
		}

		public static readonly Guid JitVersionGuid = new Guid("d609bed1-7831-49fc-bd49-b6f054dd4d46");

		private DetourRuntimeNETCore30Platform.d_compileMethod our_compileMethod;

		private IntPtr real_compileMethodPtr;

		private DetourRuntimeNETCore30Platform.d_compileMethod real_compileMethod;

		[ThreadStatic]
		private static int hookEntrancy = 0;

		protected DetourRuntimeNETCore30Platform.d_MethodHandle_GetLoaderAllocator MethodHandle_GetLoaderAllocator;

		protected DetourRuntimeNETCore30Platform.d_CreateRuntimeMethodInfoStub CreateRuntimeMethodInfoStub;

		protected DetourRuntimeNETCore30Platform.d_CreateRuntimeMethodHandle CreateRuntimeMethodHandle;

		protected DetourRuntimeNETCore30Platform.d_GetDeclaringTypeOfMethodHandle GetDeclaringTypeOfMethodHandle;

		protected DetourRuntimeNETCore30Platform.d_GetTypeFromNativeHandle GetTypeFromNativeHandle;

		private MethodInfo _getTypeFromHandleUnsafeMethod;

		private static FieldInfo _runtimeAssemblyPtrField = Type.GetType("System.Reflection.RuntimeAssembly").GetField("m_assembly", BindingFlags.Instance | BindingFlags.NonPublic);

		protected enum CorJitResult
		{
			CORJIT_OK
		}

		protected struct CORINFO_SIG_INST
		{
			public uint classInstCount;

			public unsafe IntPtr* classInst;

			public uint methInstCount;

			public unsafe IntPtr* methInst;
		}

		protected struct CORINFO_SIG_INFO
		{
			public int callConv;

			public IntPtr retTypeClass;

			public IntPtr retTypeSigClass;

			public byte retType;

			public byte flags;

			public ushort numArgs;

			public DetourRuntimeNETCore30Platform.CORINFO_SIG_INST sigInst;

			public IntPtr args;

			public IntPtr pSig;

			public uint sbSig;

			public IntPtr scope;

			public uint token;
		}

		protected struct CORINFO_METHOD_INFO
		{
			public IntPtr ftn;

			public IntPtr scope;

			public unsafe byte* ILCode;

			public uint ILCodeSize;

			public uint maxStack;

			public uint EHcount;

			public int options;

			public int regionKind;

			public DetourRuntimeNETCore30Platform.CORINFO_SIG_INFO args;

			public DetourRuntimeNETCore30Platform.CORINFO_SIG_INFO locals;
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private unsafe delegate DetourRuntimeNETCore30Platform.CorJitResult d_compileMethod(IntPtr thisPtr, IntPtr corJitInfo, in DetourRuntimeNETCore30Platform.CORINFO_METHOD_INFO methodInfo, uint flags, out byte* nativeEntry, out uint nativeSizeOfCode);

		protected delegate object d_MethodHandle_GetLoaderAllocator(IntPtr methodHandle);

		protected delegate object d_CreateRuntimeMethodInfoStub(IntPtr methodHandle, object loaderAllocator);

		protected delegate RuntimeMethodHandle d_CreateRuntimeMethodHandle(object runtimeMethodInfo);

		protected delegate Type d_GetDeclaringTypeOfMethodHandle(IntPtr methodHandle);

		protected delegate Type d_GetTypeFromNativeHandle(IntPtr handle);
	}
}
