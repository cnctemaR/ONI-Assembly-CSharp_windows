using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using MonoMod.Core.Interop;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class Core60Runtime : Core50Runtime
	{
		public Core60Runtime(ISystem system, IArchitecture arch)
			: base(system)
		{
			this.arch = arch;
		}

		protected override Guid ExpectedJitVersion
		{
			get
			{
				return Core60Runtime.JitVersionGuid;
			}
		}

		protected override void InstallJitHook(IntPtr jit)
		{
			if ((base.System.Features & SystemFeature.MayUseNativeJitHooks) != SystemFeature.None && this.InstallNativeJitHook(jit))
			{
				return;
			}
			this.InstallManagedJitHook(jit);
		}

		protected unsafe virtual bool InstallNativeJitHook(IntPtr jit)
		{
			Core60Runtime.NativeJitHookConfig* nativeJitHookConfig = this.GetNativeJitHookConfig();
			if (nativeJitHookConfig == null)
			{
				return false;
			}
			base.CheckVersionGuid(jit);
			IntPtr* vtableEntry = Core21Runtime.GetVTableEntry(jit, this.VtableIndexICorJitCompilerCompileMethod);
			Delegate @delegate = this.CastCompileMethodHookPostToRealType(this.CreateCompileMethodHookPostDelegate());
			this.ourCompileMethodHookPost = @delegate;
			IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate(@delegate);
			delegate*<IntPtr, IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult, CoreCLR.V60.AllocMemArgs*, CoreCLR.CorJitResult> invokeCompileMethodHookPost = CoreCLR.V60.InvokeCompileMethodHookPostPtr.InvokeCompileMethodHookPost;
			CoreCLR.V21.CORINFO_METHOD_INFO corinfo_METHOD_INFO;
			byte* ptr;
			uint num;
			CoreCLR.V60.AllocMemArgs allocMemArgs;
			CoreCLR.CorJitResult corJitResult = calli(MonoMod.Core.Interop.CoreCLR/CorJitResult(System.IntPtr,System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*,MonoMod.Core.Interop.CoreCLR/CorJitResult,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*), functionPointerForDelegate, IntPtr.Zero, IntPtr.Zero, &corinfo_METHOD_INFO, 0U, &ptr, &num, CoreCLR.CorJitResult.CORJIT_OK, &allocMemArgs, invokeCompileMethodHookPost);
			nativeJitHookConfig->compileMethod = *vtableEntry;
			nativeJitHookConfig->compileMethodHookPost = functionPointerForDelegate;
			IntPtr compileMethodHook = nativeJitHookConfig->compileMethodHook;
			int num2 = sizeof(IntPtr);
			global::System.Span<byte> span = new global::System.Span<byte>(stackalloc byte[(UIntPtr)num2], num2);
			global::System.Runtime.InteropServices.MemoryMarshal.Write<IntPtr>(span, ref compileMethodHook);
			base.System.PatchData(PatchTargetKind.ReadOnly, (IntPtr)((void*)vtableEntry), span, default(global::System.Span<byte>));
			Core60Runtime.CompileMethodPatchPrimer();
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static void CompileMethodPatchPrimer()
		{
		}

		protected override Delegate CreateCompileMethodDelegate(IntPtr compileMethod)
		{
			return new <>f__AnonymousDelegate0(new Core60Runtime.JitHookDelegateHolder(this, this.InvokeCompileMethodPtr, compileMethod).CompileMethodHook);
		}

		[NullableContext(0)]
		private unsafe void CompileMethodHookPostCommon(CoreCLR.V21.CORINFO_METHOD_INFO* methodInfo, byte** nativeEntry, uint* nativeSizeOfCode, IntPtr rwEntry)
		{
			RuntimeTypeHandle[] array = null;
			RuntimeTypeHandle[] array2 = null;
			if (methodInfo->args.sigInst.classInst != null)
			{
				array = new RuntimeTypeHandle[methodInfo->args.sigInst.classInstCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = base.JitHookHelpers.GetTypeFromNativeHandle(methodInfo->args.sigInst.classInst[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]).TypeHandle;
				}
			}
			if (methodInfo->args.sigInst.methInst != null)
			{
				array2 = new RuntimeTypeHandle[methodInfo->args.sigInst.methInstCount];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = base.JitHookHelpers.GetTypeFromNativeHandle(methodInfo->args.sigInst.methInst[(IntPtr)j * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]).TypeHandle;
				}
			}
			RuntimeTypeHandle typeHandle = base.JitHookHelpers.GetDeclaringTypeOfMethodHandle(methodInfo->ftn).TypeHandle;
			RuntimeMethodHandle runtimeMethodHandle = base.JitHookHelpers.CreateHandleForHandlePointer(methodInfo->ftn);
			this.OnMethodCompiledCore(typeHandle, runtimeMethodHandle, new global::System.ReadOnlyMemory<RuntimeTypeHandle>?(array), new global::System.ReadOnlyMemory<RuntimeTypeHandle>?(array2), (IntPtr)(*(IntPtr*)nativeEntry), rwEntry, (ulong)(*nativeSizeOfCode));
		}

		[NullableContext(0)]
		protected unsafe virtual Core60Runtime.NativeJitHookConfig* GetNativeJitHookConfig()
		{
			return (Core60Runtime.NativeJitHookConfig*)(void*)base.System.GetNativeJitHookConfig(60);
		}

		protected virtual Delegate CreateCompileMethodHookPostDelegate()
		{
			return new <>f__AnonymousDelegate1(new Core60Runtime.JitHookPostDelegateHolder(this).CompileMethodHookPost);
		}

		protected virtual Delegate CastCompileMethodHookPostToRealType(Delegate del)
		{
			return del.CastDelegate<CoreCLR.V60.CompileMethodHookPostDelegate>();
		}

		[NullableContext(0)]
		protected unsafe virtual void PatchWrapperVtable(IntPtr* vtbl)
		{
			this.allocMemDelegate = this.CastAllocMemToRealType(this.CreateAllocMemDelegate());
			IntPtr intPtr = base.EHNativeToManaged(Marshal.GetFunctionPointerForDelegate(this.allocMemDelegate), out this.n2mAllocMemHelper);
			delegate*<IntPtr, IntPtr, CoreCLR.V60.AllocMemArgs*, void> invokeAllocMem = this.InvokeAllocMemPtr.InvokeAllocMem;
			calli(System.Void(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*), intPtr, IntPtr.Zero, null, invokeAllocMem);
			vtbl[(IntPtr)this.VtableIndexICorJitInfoAllocMem * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = intPtr;
		}

		protected virtual int VtableIndexICorJitInfoAllocMem
		{
			get
			{
				return 156;
			}
		}

		protected virtual int ICorJitInfoFullVtableCount
		{
			get
			{
				return 173;
			}
		}

		protected virtual CoreCLR.InvokeAllocMemPtr InvokeAllocMemPtr
		{
			get
			{
				return CoreCLR.V60.InvokeAllocMemPtr;
			}
		}

		protected override CoreCLR.InvokeCompileMethodPtr InvokeCompileMethodPtr
		{
			get
			{
				return CoreCLR.V60.InvokeCompileMethodPtr;
			}
		}

		protected override Delegate CastCompileHookToRealType(Delegate del)
		{
			return del.CastDelegate<CoreCLR.V60.CompileMethodDelegate>();
		}

		protected virtual Delegate CastAllocMemToRealType(Delegate del)
		{
			return del.CastDelegate<CoreCLR.V60.AllocMemDelegate>();
		}

		protected virtual Delegate CreateAllocMemDelegate()
		{
			return new <>f__AnonymousDelegate2(new Core60Runtime.AllocMemDelegateHolder(this, this.InvokeAllocMemPtr).AllocMemHook);
		}

		private readonly IArchitecture arch;

		private static readonly Guid JitVersionGuid = new Guid(1590910040U, 34171, 18653, 168, 24, 124, 1, 54, 220, 159, 115);

		[Nullable(2)]
		private Delegate ourCompileMethodHookPost;

		[Nullable(2)]
		private Delegate allocMemDelegate;

		[Nullable(2)]
		private IDisposable n2mAllocMemHelper;

		[NullableContext(0)]
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		protected struct ICorJitInfoWrapper
		{
			public ref IntPtr this[int index]
			{
				get
				{
					return Unsafe.Add<IntPtr>(Unsafe.As<ulong, IntPtr>(ref this.data.FixedElementField), index);
				}
			}

			public IntPtr Vtbl;

			public unsafe IntPtr** Wrapped;

			public const int HotCodeRW = 0;

			public const int ColdCodeRW = 1;

			private const int DataQWords = 4;

			[FixedBuffer(typeof(ulong), 4)]
			private Core60Runtime.ICorJitInfoWrapper.<data>e__FixedBuffer data;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 32)]
			public struct <data>e__FixedBuffer
			{
				public ulong FixedElementField;
			}
		}

		[Nullable(0)]
		private sealed class JitHookDelegateHolder
		{
			public unsafe JitHookDelegateHolder(Core60Runtime runtime, CoreCLR.InvokeCompileMethodPtr icmp, IntPtr compileMethod)
			{
				this.Runtime = runtime;
				this.NativeExceptionHelper = runtime.NativeExceptionHelper;
				this.JitHookHelpers = runtime.JitHookHelpers;
				this.InvokeCompileMethodPtr = icmp;
				this.CompileMethodPtr = compileMethod;
				this.iCorJitInfoWrapperVtbl = Marshal.AllocHGlobal(IntPtr.Size * runtime.ICorJitInfoFullVtableCount);
				this.iCorJitInfoWrapperAllocs = this.Runtime.arch.CreateNativeVtableProxyStubs(this.iCorJitInfoWrapperVtbl, runtime.ICorJitInfoFullVtableCount);
				this.Runtime.PatchWrapperVtable((IntPtr*)(void*)this.iCorJitInfoWrapperVtbl);
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(42, 1, out flag);
				if (flag)
				{
					debugLogTraceStringHandler.AppendLiteral("Allocated ICorJitInfo wrapper vtable at 0x");
					debugLogTraceStringHandler.AppendFormatted<IntPtr>(this.iCorJitInfoWrapperVtbl, "x16");
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
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
				int num2 = Core60Runtime.JitHookDelegateHolder.hookEntrancy;
				Core60Runtime.JitHookDelegateHolder.hookEntrancy = 0;
			}

			[NullableContext(0)]
			public unsafe CoreCLR.CorJitResult CompileMethodHook(IntPtr jit, IntPtr corJitInfo, CoreCLR.V21.CORINFO_METHOD_INFO* methodInfo, uint flags, byte** nativeEntry, uint* nativeSizeOfCode)
			{
				if (jit == IntPtr.Zero)
				{
					return CoreCLR.CorJitResult.CORJIT_OK;
				}
				*(IntPtr*)nativeEntry = (IntPtr)((UIntPtr)0);
				*nativeSizeOfCode = 0U;
				int lastPInvokeError = MarshalEx.GetLastPInvokeError();
				IntPtr intPtr = (IntPtr)0;
				GetExceptionSlot getNativeExceptionSlot = this.GetNativeExceptionSlot;
				IntPtr* ptr = ((getNativeExceptionSlot != null) ? getNativeExceptionSlot() : null);
				Core60Runtime.JitHookDelegateHolder.hookEntrancy++;
				CoreCLR.CorJitResult corJitResult2;
				try
				{
					if (Core60Runtime.JitHookDelegateHolder.hookEntrancy == 1)
					{
						try
						{
							IAllocatedMemory allocatedMemory = this.iCorJitInfoWrapper.Value;
							if (allocatedMemory == null)
							{
								AllocationRequest allocationRequest = new AllocationRequest(sizeof(Core60Runtime.ICorJitInfoWrapper))
								{
									Alignment = IntPtr.Size,
									Executable = false
								};
								IAllocatedMemory allocatedMemory2;
								if (this.Runtime.System.MemoryAllocator.TryAllocate(allocationRequest, out allocatedMemory2))
								{
									allocatedMemory = (this.iCorJitInfoWrapper.Value = allocatedMemory2);
								}
							}
							if (allocatedMemory != null)
							{
								Core60Runtime.ICorJitInfoWrapper* ptr2 = (Core60Runtime.ICorJitInfoWrapper*)(void*)allocatedMemory.BaseAddress;
								ptr2->Vtbl = this.iCorJitInfoWrapperVtbl;
								ptr2->Wrapped = (IntPtr**)(void*)corJitInfo;
								*(*ptr2)[0] = IntPtr.Zero;
								*(*ptr2)[1] = IntPtr.Zero;
								corJitInfo = (IntPtr)((void*)ptr2);
							}
						}
						catch (Exception ex)
						{
							try
							{
								bool flag;
								<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(48, 1, out flag);
								if (flag)
								{
									debugLogErrorStringHandler.AppendLiteral("Error while setting up the ICorJitInfo wrapper: ");
									debugLogErrorStringHandler.AppendFormatted<Exception>(ex);
								}
								<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
							}
							catch
							{
							}
						}
					}
					delegate*<IntPtr, IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> invokeCompileMethod = this.InvokeCompileMethodPtr.InvokeCompileMethod;
					CoreCLR.CorJitResult corJitResult = calli(MonoMod.Core.Interop.CoreCLR/CorJitResult(System.IntPtr,System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*), this.CompileMethodPtr, jit, corJitInfo, methodInfo, flags, nativeEntry, nativeSizeOfCode, invokeCompileMethod);
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
						if (Core60Runtime.JitHookDelegateHolder.hookEntrancy == 1)
						{
							try
							{
								IAllocatedMemory value = this.iCorJitInfoWrapper.Value;
								if (value == null)
								{
									return corJitResult;
								}
								ref Core60Runtime.ICorJitInfoWrapper ptr3 = ref *(Core60Runtime.ICorJitInfoWrapper*)(void*)value.BaseAddress;
								IntPtr intPtr2 = *ptr3[0];
								this.Runtime.CompileMethodHookPostCommon(methodInfo, nativeEntry, nativeSizeOfCode, intPtr2);
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
					Core60Runtime.JitHookDelegateHolder.hookEntrancy--;
					if (ptr != null)
					{
						*ptr = intPtr;
					}
					MarshalEx.SetLastPInvokeError(lastPInvokeError);
				}
				return corJitResult2;
			}

			public readonly Core60Runtime Runtime;

			[Nullable(2)]
			public readonly INativeExceptionHelper NativeExceptionHelper;

			[Nullable(2)]
			public readonly GetExceptionSlot GetNativeExceptionSlot;

			public readonly Core21Runtime.JitHookHelpersHolder JitHookHelpers;

			public readonly CoreCLR.InvokeCompileMethodPtr InvokeCompileMethodPtr;

			public readonly IntPtr CompileMethodPtr;

			public readonly ThreadLocal<IAllocatedMemory> iCorJitInfoWrapper = new ThreadLocal<IAllocatedMemory>();

			[Nullable(new byte[] { 0, 1 })]
			public readonly global::System.ReadOnlyMemory<IAllocatedMemory> iCorJitInfoWrapperAllocs;

			public readonly IntPtr iCorJitInfoWrapperVtbl;

			[ThreadStatic]
			private static int hookEntrancy;
		}

		[NullableContext(0)]
		protected struct NativeJitHookConfig
		{
			public IntPtr compileMethod;

			public IntPtr compileMethodHook;

			public IntPtr compileMethodHookPost;

			public IntPtr allocMem;

			public IntPtr allocMemHook;
		}

		[Nullable(0)]
		private sealed class JitHookPostDelegateHolder
		{
			public JitHookPostDelegateHolder(Core60Runtime runtime)
			{
				this.Runtime = runtime;
				this.JitHookHelpers = runtime.JitHookHelpers;
			}

			[NullableContext(0)]
			public unsafe CoreCLR.CorJitResult CompileMethodHookPost(IntPtr jit, IntPtr corJitInfo, CoreCLR.V21.CORINFO_METHOD_INFO* methodInfo, uint flags, byte** nativeEntry, uint* nativeSizeOfCode, CoreCLR.CorJitResult res, CoreCLR.V60.AllocMemArgs* pArgs)
			{
				if (jit == IntPtr.Zero)
				{
					return res;
				}
				try
				{
					if (!Core60Runtime.JitHookPostDelegateHolder.patchedICorJitInfo)
					{
						object obj = Core60Runtime.JitHookPostDelegateHolder.patchedICorJitInfoSyncRoot;
						lock (obj)
						{
							if (!Core60Runtime.JitHookPostDelegateHolder.patchedICorJitInfo)
							{
								IntPtr* vtableEntry = Core21Runtime.GetVTableEntry(corJitInfo, this.Runtime.VtableIndexICorJitInfoAllocMem);
								Core60Runtime.NativeJitHookConfig* nativeJitHookConfig = this.Runtime.GetNativeJitHookConfig();
								nativeJitHookConfig->allocMem = *vtableEntry;
								IntPtr allocMemHook = nativeJitHookConfig->allocMemHook;
								int num = sizeof(IntPtr);
								global::System.Span<byte> span = new global::System.Span<byte>(stackalloc byte[(UIntPtr)num], num);
								global::System.Runtime.InteropServices.MemoryMarshal.Write<IntPtr>(span, ref allocMemHook);
								this.Runtime.System.PatchData(PatchTargetKind.ReadOnly, (IntPtr)((void*)vtableEntry), span, default(global::System.Span<byte>));
								Core60Runtime.JitHookPostDelegateHolder.patchedICorJitInfo = true;
							}
						}
					}
					this.Runtime.CompileMethodHookPostCommon(methodInfo, nativeEntry, nativeSizeOfCode, pArgs->hotCodeBlockRW);
				}
				catch
				{
				}
				return res;
			}

			public readonly Core60Runtime Runtime;

			public readonly Core21Runtime.JitHookHelpersHolder JitHookHelpers;

			public static volatile bool patchedICorJitInfo;

			public static readonly object patchedICorJitInfoSyncRoot = new object();
		}

		[NullableContext(0)]
		private sealed class AllocMemDelegateHolder
		{
			[NullableContext(1)]
			public unsafe AllocMemDelegateHolder(Core60Runtime runtime, CoreCLR.InvokeAllocMemPtr iamp)
			{
				this.Runtime = runtime;
				this.NativeExceptionHelper = runtime.NativeExceptionHelper;
				INativeExceptionHelper nativeExceptionHelper = this.NativeExceptionHelper;
				this.GetNativeExceptionSlot = ((nativeExceptionHelper != null) ? nativeExceptionHelper.GetExceptionSlot : null);
				this.InvokeAllocMemPtr = iamp;
				this.ICorJitInfoAllocMemIdx = this.Runtime.VtableIndexICorJitInfoAllocMem;
				delegate*<IntPtr, IntPtr, CoreCLR.V60.AllocMemArgs*, void> invokeAllocMem = iamp.InvokeAllocMem;
				calli(System.Void(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*), IntPtr.Zero, IntPtr.Zero, null, invokeAllocMem);
			}

			private IntPtr GetRealInvokePtr(IntPtr ptr)
			{
				if (this.NativeExceptionHelper == null)
				{
					return ptr;
				}
				return this.AllocMemExceptionHelperCache.GetOrAdd(ptr, delegate(IntPtr p)
				{
					IDisposable disposable;
					return new ValueTuple<IntPtr, IDisposable>(this.Runtime.EHManagedToNative(p, out disposable), disposable);
				}).Item1;
			}

			public unsafe void AllocMemHook(IntPtr thisPtr, CoreCLR.V60.AllocMemArgs* args)
			{
				if (thisPtr == IntPtr.Zero)
				{
					return;
				}
				Core60Runtime.ICorJitInfoWrapper* ptr = (Core60Runtime.ICorJitInfoWrapper*)(void*)thisPtr;
				IntPtr** wrapped = ptr->Wrapped;
				delegate*<IntPtr, IntPtr, CoreCLR.V60.AllocMemArgs*, void> invokeAllocMem = this.InvokeAllocMemPtr.InvokeAllocMem;
				calli(System.Void(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*), this.GetRealInvokePtr(*(*(IntPtr*)wrapped + (IntPtr)this.ICorJitInfoAllocMemIdx * (IntPtr)sizeof(IntPtr))), (IntPtr)((void*)wrapped), args, invokeAllocMem);
				GetExceptionSlot getNativeExceptionSlot = this.GetNativeExceptionSlot;
				if (getNativeExceptionSlot != null && *getNativeExceptionSlot() != 0)
				{
					return;
				}
				*(*ptr)[0] = args->hotCodeBlockRW;
				*(*ptr)[1] = args->coldCodeBlockRW;
			}

			[Nullable(1)]
			public readonly Core60Runtime Runtime;

			[Nullable(2)]
			public readonly INativeExceptionHelper NativeExceptionHelper;

			[Nullable(2)]
			public readonly GetExceptionSlot GetNativeExceptionSlot;

			public readonly CoreCLR.InvokeAllocMemPtr InvokeAllocMemPtr;

			public readonly int ICorJitInfoAllocMemIdx;

			[TupleElementNames(new string[] { "M2N", null })]
			[Nullable(new byte[] { 1, 0, 2 })]
			public readonly ConcurrentDictionary<IntPtr, ValueTuple<IntPtr, IDisposable>> AllocMemExceptionHelperCache = new ConcurrentDictionary<IntPtr, ValueTuple<IntPtr, IDisposable>>();
		}
	}
}
