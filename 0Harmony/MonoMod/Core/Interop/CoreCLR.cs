using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoMod.Core.Interop.Attributes;
using MonoMod.Utils;

namespace MonoMod.Core.Interop
{
	internal static class CoreCLR
	{
		public enum CorJitResult
		{
			CORJIT_OK
		}

		public readonly struct InvokeCompileMethodPtr
		{
			public unsafe InvokeCompileMethodPtr(delegate*<IntPtr, IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> ptr)
			{
				this.methodPtr = (IntPtr)ptr;
			}

			public unsafe delegate*<IntPtr, IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> InvokeCompileMethod
			{
				get
				{
					return (void*)this.methodPtr;
				}
			}

			private readonly IntPtr methodPtr;
		}

		public class V21
		{
			public static CoreCLR.InvokeCompileMethodPtr InvokeCompileMethodPtr
			{
				get
				{
					return new CoreCLR.InvokeCompileMethodPtr(ldftn(InvokeCompileMethod));
				}
			}

			public unsafe static CoreCLR.CorJitResult InvokeCompileMethod(IntPtr functionPtr, IntPtr thisPtr, IntPtr corJitInfo, CoreCLR.V21.CORINFO_METHOD_INFO* methodInfo, uint flags, byte** pNativeEntry, uint* pNativeSizeOfCode)
			{
				if (functionPtr == IntPtr.Zero)
				{
					*(IntPtr*)pNativeEntry = (IntPtr)((UIntPtr)0);
					*pNativeSizeOfCode = 0U;
					return CoreCLR.CorJitResult.CORJIT_OK;
				}
				delegate* unmanaged[Stdcall]<IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> monoMod.Core.Interop.CoreCLR/CorJitResult_u0020(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*) = (void*)functionPtr;
				delegate* unmanaged[Stdcall]<IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> monoMod.Core.Interop.CoreCLR/CorJitResult_u0020(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*)2 = monoMod.Core.Interop.CoreCLR/CorJitResult_u0020(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*);
				return calli(MonoMod.Core.Interop.CoreCLR/CorJitResult(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*), thisPtr, corJitInfo, methodInfo, flags, pNativeEntry, pNativeSizeOfCode, monoMod.Core.Interop.CoreCLR/CorJitResult_u0020(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*)2);
			}

			public struct CORINFO_SIG_INST
			{
				public uint classInstCount;

				public unsafe IntPtr* classInst;

				public uint methInstCount;

				public unsafe IntPtr* methInst;
			}

			public struct CORINFO_SIG_INFO
			{
				public int callConv;

				public IntPtr retTypeClass;

				public IntPtr retTypeSigClass;

				public byte retType;

				public byte flags;

				public ushort numArgs;

				public CoreCLR.V21.CORINFO_SIG_INST sigInst;

				public IntPtr args;

				public IntPtr pSig;

				public uint sbSig;

				public IntPtr scope;

				public uint token;
			}

			public struct CORINFO_METHOD_INFO
			{
				public IntPtr ftn;

				public IntPtr scope;

				public unsafe byte* ILCode;

				public uint ILCodeSize;

				public uint maxStack;

				public uint EHcount;

				public int options;

				public int regionKind;

				public CoreCLR.V21.CORINFO_SIG_INFO args;

				public CoreCLR.V21.CORINFO_SIG_INFO locals;
			}

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			public unsafe delegate CoreCLR.CorJitResult CompileMethodDelegate(IntPtr thisPtr, IntPtr corJitInfo, CoreCLR.V21.CORINFO_METHOD_INFO* methodInfo, uint flags, byte** nativeEntry, uint* nativeSizeOfCode);
		}

		public class V30 : CoreCLR.V21
		{
		}

		public class V31 : CoreCLR.V30
		{
		}

		public class V50 : CoreCLR.V31
		{
		}

		public class V100 : CoreCLR.V90
		{
			public new static class ICorJitInfoVtable
			{
				public const int AllocMemIndex = 160;

				public const int TotalVtableCount = 176;
			}
		}

		public readonly struct InvokeCompileMethodHookPostPtr
		{
			public unsafe InvokeCompileMethodHookPostPtr(delegate*<IntPtr, IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult, CoreCLR.V60.AllocMemArgs*, CoreCLR.CorJitResult> ptr)
			{
				this.methodPtr = (IntPtr)ptr;
			}

			public unsafe delegate*<IntPtr, IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult, CoreCLR.V60.AllocMemArgs*, CoreCLR.CorJitResult> InvokeCompileMethodHookPost
			{
				get
				{
					return (void*)this.methodPtr;
				}
			}

			private readonly IntPtr methodPtr;
		}

		public readonly struct InvokeAllocMemPtr
		{
			public unsafe InvokeAllocMemPtr(delegate*<IntPtr, IntPtr, CoreCLR.V60.AllocMemArgs*, void> ptr)
			{
				this.methodPtr = (IntPtr)ptr;
			}

			public unsafe delegate*<IntPtr, IntPtr, CoreCLR.V60.AllocMemArgs*, void> InvokeAllocMem
			{
				get
				{
					return (void*)this.methodPtr;
				}
			}

			private readonly IntPtr methodPtr;
		}

		public class V60 : CoreCLR.V50
		{
			public static CoreCLR.InvokeAllocMemPtr InvokeAllocMemPtr
			{
				get
				{
					return new CoreCLR.InvokeAllocMemPtr(ldftn(InvokeAllocMem));
				}
			}

			public unsafe static void InvokeAllocMem(IntPtr functionPtr, IntPtr thisPtr, CoreCLR.V60.AllocMemArgs* args)
			{
				if (functionPtr == IntPtr.Zero)
				{
					return;
				}
				delegate* unmanaged[Thiscall]<IntPtr, CoreCLR.V60.AllocMemArgs*, void> system.Void_u0020(System.IntPtr,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*) = (void*)functionPtr;
				delegate* unmanaged[Thiscall]<IntPtr, CoreCLR.V60.AllocMemArgs*, void> system.Void_u0020(System.IntPtr,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*)2 = system.Void_u0020(System.IntPtr,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*);
				calli(System.Void(System.IntPtr,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*), thisPtr, args, system.Void_u0020(System.IntPtr,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*)2);
			}

			public new static CoreCLR.InvokeCompileMethodPtr InvokeCompileMethodPtr
			{
				get
				{
					return new CoreCLR.InvokeCompileMethodPtr(ldftn(InvokeCompileMethod));
				}
			}

			public new unsafe static CoreCLR.CorJitResult InvokeCompileMethod(IntPtr functionPtr, IntPtr thisPtr, IntPtr corJitInfo, CoreCLR.V21.CORINFO_METHOD_INFO* methodInfo, uint flags, byte** nativeEntry, uint* nativeSizeOfCode)
			{
				if (functionPtr == IntPtr.Zero)
				{
					*(IntPtr*)nativeEntry = (IntPtr)((UIntPtr)0);
					*nativeSizeOfCode = 0U;
					return CoreCLR.CorJitResult.CORJIT_OK;
				}
				delegate* unmanaged[Thiscall]<IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> monoMod.Core.Interop.CoreCLR/CorJitResult_u0020(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*) = (void*)functionPtr;
				delegate* unmanaged[Thiscall]<IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> monoMod.Core.Interop.CoreCLR/CorJitResult_u0020(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*)2 = monoMod.Core.Interop.CoreCLR/CorJitResult_u0020(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*);
				return calli(MonoMod.Core.Interop.CoreCLR/CorJitResult(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*), thisPtr, corJitInfo, methodInfo, flags, nativeEntry, nativeSizeOfCode, monoMod.Core.Interop.CoreCLR/CorJitResult_u0020(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*)2);
			}

			public static CoreCLR.InvokeCompileMethodHookPostPtr InvokeCompileMethodHookPostPtr
			{
				get
				{
					return new CoreCLR.InvokeCompileMethodHookPostPtr(ldftn(InvokeCompileMethodHookPost));
				}
			}

			public unsafe static CoreCLR.CorJitResult InvokeCompileMethodHookPost(IntPtr functionPtr, IntPtr thisPtr, IntPtr corJitInfo, CoreCLR.V21.CORINFO_METHOD_INFO* methodInfo, uint flags, byte** nativeEntry, uint* nativeSizeOfCode, CoreCLR.CorJitResult res, CoreCLR.V60.AllocMemArgs* pArgs)
			{
				if (functionPtr == IntPtr.Zero)
				{
					return res;
				}
				delegate* unmanaged[Thiscall]<IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult, CoreCLR.V60.AllocMemArgs*, CoreCLR.CorJitResult> monoMod.Core.Interop.CoreCLR/CorJitResult_u0020(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*,MonoMod.Core.Interop.CoreCLR/CorJitResult,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*) = (void*)functionPtr;
				delegate* unmanaged[Thiscall]<IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult, CoreCLR.V60.AllocMemArgs*, CoreCLR.CorJitResult> monoMod.Core.Interop.CoreCLR/CorJitResult_u0020(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*,MonoMod.Core.Interop.CoreCLR/CorJitResult,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*)2 = monoMod.Core.Interop.CoreCLR/CorJitResult_u0020(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*,MonoMod.Core.Interop.CoreCLR/CorJitResult,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*);
				return calli(MonoMod.Core.Interop.CoreCLR/CorJitResult(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*,MonoMod.Core.Interop.CoreCLR/CorJitResult,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*), thisPtr, corJitInfo, methodInfo, flags, nativeEntry, nativeSizeOfCode, res, pArgs, monoMod.Core.Interop.CoreCLR/CorJitResult_u0020(System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*,MonoMod.Core.Interop.CoreCLR/CorJitResult,MonoMod.Core.Interop.CoreCLR/V60/AllocMemArgs*)2);
			}

			public static class ICorJitInfoVtable
			{
				public const int AllocMemIndex = 156;

				public const int TotalVtableCount = 173;
			}

			public struct AllocMemArgs
			{
				public uint hotCodeSize;

				public uint coldCodeSize;

				public uint roDataSize;

				public uint xcptnsCount;

				public int flag;

				public IntPtr hotCodeBlock;

				public IntPtr hotCodeBlockRW;

				public IntPtr coldCodeBlock;

				public IntPtr coldCodeBlockRW;

				public IntPtr roDataBlock;

				public IntPtr roDataBlockRW;
			}

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public unsafe delegate void AllocMemDelegate(IntPtr thisPtr, CoreCLR.V60.AllocMemArgs* args);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public new unsafe delegate CoreCLR.CorJitResult CompileMethodDelegate(IntPtr thisPtr, IntPtr corJitInfo, CoreCLR.V21.CORINFO_METHOD_INFO* methodInfo, uint flags, byte** nativeEntry, uint* nativeSizeOfCode);

			[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
			public unsafe delegate CoreCLR.CorJitResult CompileMethodHookPostDelegate(IntPtr thisPtr, IntPtr corJitInfo, CoreCLR.V21.CORINFO_METHOD_INFO* methodInfo, uint flags, byte** nativeEntry, uint* nativeSizeOfCode, CoreCLR.CorJitResult res, CoreCLR.V60.AllocMemArgs* allocMemArgs);

			public enum MethodClassification
			{
				IL,
				FCall,
				NDirect,
				EEImpl,
				Array,
				Instantiated,
				ComInterop,
				Dynamic
			}

			[Flags]
			public enum MethodDescClassification : ushort
			{
				ClassificationMask = 7,
				HasNonVtableSlot = 8,
				MethodImpl = 16,
				HasNativeCodeSlot = 32,
				HasComPlusCallInfo = 64,
				Static = 128,
				Duplicate = 1024,
				VerifiedState = 2048,
				Verifiable = 4096,
				NotInline = 8192,
				Synchronized = 16384,
				RequiresFullSlotNumber = 32768
			}

			public struct RelativePointer
			{
				public RelativePointer([NativeInteger] IntPtr delta)
				{
					this.m_delta = delta;
				}

				public unsafe void* Value
				{
					get
					{
						IntPtr delta = this.m_delta;
						if (delta != 0)
						{
							return Unsafe.AsPointer<CoreCLR.V60.RelativePointer>(Unsafe.AddByteOffset<CoreCLR.V60.RelativePointer>(ref this, delta));
						}
						return null;
					}
				}

				[NativeInteger]
				private IntPtr m_delta;
			}

			public struct RelativeFixupPointer
			{
				public unsafe void* Value
				{
					get
					{
						IntPtr delta = this.m_delta;
						if (delta == 0)
						{
							return null;
						}
						IntPtr intPtr = Unsafe.AsPointer<CoreCLR.V60.RelativeFixupPointer>(Unsafe.AddByteOffset<CoreCLR.V60.RelativeFixupPointer>(ref this, delta));
						if ((intPtr & (IntPtr)1) != 0)
						{
							intPtr = *(intPtr - (IntPtr)1);
						}
						return intPtr;
					}
				}

				[NativeInteger]
				private IntPtr m_delta;

				[NativeInteger]
				public const IntPtr FIXUP_POINTER_INDIRECTION = 1;
			}

			public struct MethodDesc
			{
				public ushort SlotNumber
				{
					get
					{
						if (!this.m_wFlags.Has(CoreCLR.V60.MethodDescClassification.RequiresFullSlotNumber))
						{
							return this.m_wSlotNumber & 1023;
						}
						return this.m_wSlotNumber;
					}
				}

				public CoreCLR.V60.MethodClassification Classification
				{
					get
					{
						return (CoreCLR.V60.MethodClassification)(this.m_wFlags & CoreCLR.V60.MethodDescClassification.ClassificationMask);
					}
				}

				public unsafe CoreCLR.V60.MethodDescChunk* MethodDescChunk
				{
					get
					{
						return (CoreCLR.V60.MethodDescChunk*)((byte*)Unsafe.AsPointer<CoreCLR.V60.MethodDesc>(ref this) - (ulong)((IntPtr)sizeof(CoreCLR.V60.MethodDescChunk) + (IntPtr)((UIntPtr)this.m_chunkIndex * CoreCLR.V60.MethodDesc.Alignment)));
					}
				}

				public unsafe CoreCLR.V60.MethodTable* MethodTable
				{
					get
					{
						return this.MethodDescChunk->m_methodTable;
					}
				}

				public unsafe void* GetMethodEntryPoint()
				{
					if (!this.HasNonVtableSlot)
					{
						return this.MethodTable->GetSlot((uint)this.SlotNumber);
					}
					UIntPtr baseSize = this.GetBaseSize();
					byte* ptr = (byte*)Unsafe.AsPointer<CoreCLR.V60.MethodDesc>(ref this) + (ulong)baseSize;
					if (!this.MethodDescChunk->m_flagsAndTokenRange.Has(CoreCLR.V60.MethodDescChunk.Flags.IsZapped))
					{
						return *(IntPtr*)ptr;
					}
					return new CoreCLR.V60.RelativePointer(ptr).Value;
				}

				public bool TryAsFCall(out CoreCLR.V60.FCallMethodDescPtr md)
				{
					if (this.Classification == CoreCLR.V60.MethodClassification.FCall)
					{
						md = new CoreCLR.V60.FCallMethodDescPtr(Unsafe.AsPointer<CoreCLR.V60.MethodDesc>(ref this), CoreCLR.V60.FCallMethodDescPtr.CurrentVtable);
						return true;
					}
					md = default(CoreCLR.V60.FCallMethodDescPtr);
					return false;
				}

				public bool TryAsNDirect(out CoreCLR.V60.NDirectMethodDescPtr md)
				{
					if (this.Classification == CoreCLR.V60.MethodClassification.NDirect)
					{
						md = new CoreCLR.V60.NDirectMethodDescPtr(Unsafe.AsPointer<CoreCLR.V60.MethodDesc>(ref this), CoreCLR.V60.NDirectMethodDescPtr.CurrentVtable);
						return true;
					}
					md = default(CoreCLR.V60.NDirectMethodDescPtr);
					return false;
				}

				public bool TryAsEEImpl(out CoreCLR.V60.EEImplMethodDescPtr md)
				{
					if (this.Classification == CoreCLR.V60.MethodClassification.EEImpl)
					{
						md = new CoreCLR.V60.EEImplMethodDescPtr(Unsafe.AsPointer<CoreCLR.V60.MethodDesc>(ref this), CoreCLR.V60.EEImplMethodDescPtr.CurrentVtable);
						return true;
					}
					md = default(CoreCLR.V60.EEImplMethodDescPtr);
					return false;
				}

				public bool TryAsArray(out CoreCLR.V60.ArrayMethodDescPtr md)
				{
					if (this.Classification == CoreCLR.V60.MethodClassification.Array)
					{
						md = new CoreCLR.V60.ArrayMethodDescPtr(Unsafe.AsPointer<CoreCLR.V60.MethodDesc>(ref this), CoreCLR.V60.ArrayMethodDescPtr.CurrentVtable);
						return true;
					}
					md = default(CoreCLR.V60.ArrayMethodDescPtr);
					return false;
				}

				public unsafe bool TryAsInstantiated(out CoreCLR.V60.InstantiatedMethodDesc* md)
				{
					if (this.Classification == CoreCLR.V60.MethodClassification.Instantiated)
					{
						md = Unsafe.AsPointer<CoreCLR.V60.MethodDesc>(ref this);
						return true;
					}
					md = default(CoreCLR.V60.InstantiatedMethodDesc*);
					return false;
				}

				public unsafe bool TryAsComPlusCall(out CoreCLR.V60.ComPlusCallMethodDesc* md)
				{
					if (this.Classification == CoreCLR.V60.MethodClassification.ComInterop)
					{
						md = Unsafe.AsPointer<CoreCLR.V60.MethodDesc>(ref this);
						return true;
					}
					md = default(CoreCLR.V60.ComPlusCallMethodDesc*);
					return false;
				}

				public bool TryAsDynamic(out CoreCLR.V60.DynamicMethodDescPtr md)
				{
					if (this.Classification == CoreCLR.V60.MethodClassification.Dynamic)
					{
						md = new CoreCLR.V60.DynamicMethodDescPtr(Unsafe.AsPointer<CoreCLR.V60.MethodDesc>(ref this), CoreCLR.V60.DynamicMethodDescPtr.CurrentVtable);
						return true;
					}
					md = default(CoreCLR.V60.DynamicMethodDescPtr);
					return false;
				}

				[return: NativeInteger]
				public unsafe UIntPtr SizeOf(bool includeNonVtable = true, bool includeMethodImpl = true, bool includeComPlus = true, bool includeNativeCode = true)
				{
					UIntPtr uintPtr = this.GetBaseSize() + (UIntPtr)((includeNonVtable && this.m_wFlags.Has(CoreCLR.V60.MethodDescClassification.HasNonVtableSlot)) ? ((IntPtr)sizeof(void*)) : ((IntPtr)0)) + (UIntPtr)((includeMethodImpl && this.m_wFlags.Has(CoreCLR.V60.MethodDescClassification.MethodImpl)) ? ((IntPtr)sizeof(void*) * (IntPtr)2) : ((IntPtr)0)) + (UIntPtr)((includeComPlus && this.m_wFlags.Has(CoreCLR.V60.MethodDescClassification.HasComPlusCallInfo)) ? ((IntPtr)sizeof(void*)) : ((IntPtr)0)) + (UIntPtr)((includeNativeCode && this.m_wFlags.Has(CoreCLR.V60.MethodDescClassification.HasNativeCodeSlot)) ? ((IntPtr)sizeof(void*)) : ((IntPtr)0));
					if (includeNativeCode && this.HasNativeCodeSlot)
					{
						uintPtr += (UIntPtr)(((this.GetAddrOfNativeCodeSlot() & 1) != null) ? ((IntPtr)sizeof(void*)) : ((IntPtr)0));
					}
					return uintPtr;
				}

				public unsafe void* GetNativeCode()
				{
					if (this.HasNativeCodeSlot)
					{
						void* ptr = *(IntPtr*)(this.GetAddrOfNativeCodeSlot() & ~1);
						if (ptr != null)
						{
							return ptr;
						}
					}
					if (!this.HasStableEntryPoint || this.HasPrecode)
					{
						return null;
					}
					return this.GetStableEntryPoint();
				}

				public unsafe void* GetStableEntryPoint()
				{
					return this.GetMethodEntryPoint();
				}

				public bool HasNonVtableSlot
				{
					get
					{
						return this.m_wFlags.Has(CoreCLR.V60.MethodDescClassification.HasNonVtableSlot);
					}
				}

				public bool HasStableEntryPoint
				{
					get
					{
						return this.m_bFlags2.Has(CoreCLR.V60.MethodDesc.Flags2.HasStableEntryPoint);
					}
				}

				public bool HasPrecode
				{
					get
					{
						return this.m_bFlags2.Has(CoreCLR.V60.MethodDesc.Flags2.HasPrecode);
					}
				}

				public bool HasNativeCodeSlot
				{
					get
					{
						return this.m_wFlags.Has(CoreCLR.V60.MethodDescClassification.HasNativeCodeSlot);
					}
				}

				public bool IsUnboxingStub
				{
					get
					{
						return this.m_bFlags2.Has(CoreCLR.V60.MethodDesc.Flags2.IsUnboxingStub);
					}
				}

				public unsafe bool HasMethodInstantiation
				{
					get
					{
						CoreCLR.V60.InstantiatedMethodDesc* ptr;
						return this.TryAsInstantiated(out ptr) && ptr->IMD_HasMethodInstantiation;
					}
				}

				public unsafe bool IsGenericMethodDefinition
				{
					get
					{
						CoreCLR.V60.InstantiatedMethodDesc* ptr;
						return this.TryAsInstantiated(out ptr) && ptr->IMD_IsGenericMethodDefinition;
					}
				}

				public unsafe bool IsInstantiatingStub
				{
					get
					{
						CoreCLR.V60.InstantiatedMethodDesc* ptr;
						return !this.IsUnboxingStub && this.TryAsInstantiated(out ptr) && ptr->IMD_IsWrapperStubWithInstantiations;
					}
				}

				public bool IsWrapperStub
				{
					get
					{
						return this.IsUnboxingStub || this.IsInstantiatingStub;
					}
				}

				public bool IsTightlyBoundToMethodTable
				{
					get
					{
						if (!this.HasNonVtableSlot)
						{
							return true;
						}
						if (this.HasMethodInstantiation)
						{
							return this.IsGenericMethodDefinition;
						}
						return !this.IsWrapperStub;
					}
				}

				public unsafe static CoreCLR.V60.MethodDesc* FindTightlyBoundWrappedMethodDesc(CoreCLR.V60.MethodDesc* pMD)
				{
					CoreCLR.V60.InstantiatedMethodDesc* ptr;
					if (pMD->IsUnboxingStub && pMD->TryAsInstantiated(out ptr))
					{
						pMD = ptr->IMD_GetWrappedMethodDesc();
					}
					if (!pMD->IsTightlyBoundToMethodTable)
					{
						pMD = pMD->GetCanonicalMethodTable()->GetParallelMethodDesc(pMD);
					}
					if (pMD->IsUnboxingStub)
					{
						pMD = CoreCLR.V60.MethodDesc.GetNextIntroducedMethod(pMD);
					}
					return pMD;
				}

				public unsafe static CoreCLR.V60.MethodDesc* GetNextIntroducedMethod(CoreCLR.V60.MethodDesc* pMD)
				{
					CoreCLR.V60.MethodDescChunk* ptr = pMD->MethodDescChunk;
					UIntPtr uintPtr = pMD + pMD->SizeOf(true, true, true, true) / (UIntPtr)sizeof(CoreCLR.V60.MethodDesc);
					UIntPtr uintPtr2 = ptr + ptr->SizeOf / (UIntPtr)sizeof(CoreCLR.V60.MethodDescChunk);
					if (uintPtr < uintPtr2)
					{
						return uintPtr;
					}
					ptr = ptr->m_next;
					if (ptr != null)
					{
						return ptr->FirstMethodDesc;
					}
					return null;
				}

				public unsafe CoreCLR.V60.MethodTable* GetCanonicalMethodTable()
				{
					return this.MethodTable->GetCanonicalMethodTable();
				}

				public unsafe void* GetAddrOfNativeCodeSlot()
				{
					UIntPtr uintPtr = this.SizeOf(true, true, false, false);
					return Unsafe.AsPointer<CoreCLR.V60.MethodDesc>(Unsafe.AddByteOffset<CoreCLR.V60.MethodDesc>(ref this, uintPtr));
				}

				[return: NativeInteger]
				public UIntPtr GetBaseSize()
				{
					return CoreCLR.V60.MethodDesc.GetBaseSize(this.Classification);
				}

				[return: NativeInteger]
				public static UIntPtr GetBaseSize(CoreCLR.V60.MethodClassification classification)
				{
					return CoreCLR.V60.MethodDesc.s_ClassificationSizeTable[(int)classification];
				}

				[NativeInteger]
				public static readonly UIntPtr Alignment = (UIntPtr)((IntPtr.Size == 8) ? ((IntPtr)8) : ((IntPtr)4));

				public CoreCLR.V60.MethodDesc.Flags3 m_wFlags3AndTokenRemainder;

				public byte m_chunkIndex;

				public CoreCLR.V60.MethodDesc.Flags2 m_bFlags2;

				public const ushort PackedSlot_SlotMask = 1023;

				public const ushort PackedSlot_NameHashMask = 64512;

				public ushort m_wSlotNumber;

				public CoreCLR.V60.MethodDescClassification m_wFlags;

				[NativeInteger]
				[Nullable(1)]
				private static readonly UIntPtr[] s_ClassificationSizeTable = new UIntPtr[]
				{
					(UIntPtr)((IntPtr)sizeof(CoreCLR.V60.MethodDesc)),
					(UIntPtr)((IntPtr)CoreCLR.V60.FCallMethodDescPtr.CurrentSize),
					(UIntPtr)((IntPtr)CoreCLR.V60.NDirectMethodDescPtr.CurrentSize),
					(UIntPtr)((IntPtr)CoreCLR.V60.EEImplMethodDescPtr.CurrentSize),
					(UIntPtr)((IntPtr)CoreCLR.V60.ArrayMethodDescPtr.CurrentSize),
					(UIntPtr)((IntPtr)sizeof(CoreCLR.V60.InstantiatedMethodDesc)),
					(UIntPtr)((IntPtr)sizeof(CoreCLR.V60.ComPlusCallMethodDesc)),
					(UIntPtr)((IntPtr)CoreCLR.V60.DynamicMethodDescPtr.CurrentSize)
				};

				[Flags]
				public enum Flags3 : ushort
				{
					TokenRemainderMask = 16383,
					HasForwardedValuetypeParameter = 16384,
					ValueTypeParametersWalked = 16384,
					DoesNotHaveEquivalentValuetypeParameters = 32768
				}

				[Flags]
				public enum Flags2 : byte
				{
					HasStableEntryPoint = 1,
					HasPrecode = 2,
					IsUnboxingStub = 4,
					IsJitIntrinsic = 16,
					IsEligibleForTieredCompilation = 32,
					RequiresCovariantReturnTypeChecking = 64
				}
			}

			public struct MethodDescChunk
			{
				public unsafe CoreCLR.V60.MethodDesc* FirstMethodDesc
				{
					get
					{
						return (CoreCLR.V60.MethodDesc*)((byte*)Unsafe.AsPointer<CoreCLR.V60.MethodDescChunk>(ref this) + sizeof(CoreCLR.V60.MethodDescChunk));
					}
				}

				public uint Size
				{
					get
					{
						return (uint)(this.m_size + 1);
					}
				}

				public uint Count
				{
					get
					{
						return (uint)(this.m_count + 1);
					}
				}

				[NativeInteger]
				public unsafe UIntPtr SizeOf
				{
					[return: NativeInteger]
					get
					{
						return (UIntPtr)((IntPtr)sizeof(CoreCLR.V60.MethodDescChunk) + (IntPtr)((UIntPtr)this.Size * CoreCLR.V60.MethodDesc.Alignment));
					}
				}

				public unsafe CoreCLR.V60.MethodTable* m_methodTable;

				public unsafe CoreCLR.V60.MethodDescChunk* m_next;

				public byte m_size;

				public byte m_count;

				public CoreCLR.V60.MethodDescChunk.Flags m_flagsAndTokenRange;

				[Flags]
				public enum Flags : ushort
				{
					TokenRangeMask = 1023,
					HasCompactEntrypoints = 16384,
					IsZapped = 32768
				}
			}

			[FatInterface]
			public struct StoredSigMethodDescPtr
			{
				[Nullable(1)]
				public static IntPtr[] CurrentVtable
				{
					[NullableContext(1)]
					get;
				} = ((IntPtr.Size == 8) ? CoreCLR.V60.StoredSigMethodDesc_64.FatVtable_ : CoreCLR.V60.StoredSigMethodDesc_32.FatVtable_);

				public static int CurrentSize { get; } = ((IntPtr.Size == 8) ? sizeof(CoreCLR.V60.StoredSigMethodDesc_64) : sizeof(CoreCLR.V60.StoredSigMethodDesc_32));

				private unsafe void* GetPSig()
				{
					delegate*<void*, void*> system.Void*_u0020(System.Void*) = (void*)this.vtbl_[0];
					return calli(System.Void*(System.Void*), this.ptr_, system.Void*_u0020(System.Void*));
				}

				public unsafe void* m_pSig
				{
					[FatInterfaceIgnore]
					get
					{
						return this.GetPSig();
					}
				}

				private unsafe uint GetCSig()
				{
					delegate*<void*, uint> system.UInt32_u0020(System.Void*) = (void*)this.vtbl_[0];
					return calli(System.UInt32(System.Void*), this.ptr_, system.UInt32_u0020(System.Void*));
				}

				public uint m_cSig
				{
					[FatInterfaceIgnore]
					get
					{
						return this.GetCSig();
					}
				}

				public unsafe StoredSigMethodDescPtr(void* ptr, [Nullable(1)] IntPtr[] vtbl)
				{
					this.ptr_ = ptr;
					this.vtbl_ = vtbl;
				}

				private unsafe readonly void* ptr_;

				[Nullable(1)]
				private readonly IntPtr[] vtbl_;
			}

			[FatInterfaceImpl(typeof(CoreCLR.V60.StoredSigMethodDescPtr))]
			public struct StoredSigMethodDesc_64
			{
				private unsafe void* GetPSig()
				{
					return this.m_pSig;
				}

				private uint GetCSig()
				{
					return this.m_cSig;
				}

				[Nullable(1)]
				public static IntPtr[] FatVtable_
				{
					[NullableContext(1)]
					get
					{
						IntPtr[] array;
						if ((array = CoreCLR.V60.StoredSigMethodDesc_64.fatVtable_) == null)
						{
							array = (CoreCLR.V60.StoredSigMethodDesc_64.fatVtable_ = new IntPtr[]
							{
								(IntPtr)ldftn(<get_FatVtable_>g__S_GetPSig_0|8_0),
								(IntPtr)ldftn(<get_FatVtable_>g__S_GetCSig_1|8_1)
							});
						}
						return array;
					}
				}

				[CompilerGenerated]
				internal unsafe static void* <get_FatVtable_>g__S_GetPSig_0|8_0(void* ptr__)
				{
					return ((CoreCLR.V60.StoredSigMethodDesc_64*)ptr__)->GetPSig();
				}

				[CompilerGenerated]
				internal unsafe static uint <get_FatVtable_>g__S_GetCSig_1|8_1(void* ptr__)
				{
					return ((CoreCLR.V60.StoredSigMethodDesc_64*)ptr__)->GetCSig();
				}

				public CoreCLR.V60.MethodDesc @base;

				public unsafe void* m_pSig;

				public uint m_cSig;

				public uint m_dwExtendedFlags;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[FatInterfaceImpl(typeof(CoreCLR.V60.StoredSigMethodDescPtr))]
			public struct StoredSigMethodDesc_32
			{
				private unsafe void* GetPSig()
				{
					return this.m_pSig;
				}

				private uint GetCSig()
				{
					return this.m_cSig;
				}

				[Nullable(1)]
				public static IntPtr[] FatVtable_
				{
					[NullableContext(1)]
					get
					{
						IntPtr[] array;
						if ((array = CoreCLR.V60.StoredSigMethodDesc_32.fatVtable_) == null)
						{
							array = (CoreCLR.V60.StoredSigMethodDesc_32.fatVtable_ = new IntPtr[]
							{
								(IntPtr)ldftn(<get_FatVtable_>g__S_GetPSig_0|7_0),
								(IntPtr)ldftn(<get_FatVtable_>g__S_GetCSig_1|7_1)
							});
						}
						return array;
					}
				}

				[CompilerGenerated]
				internal unsafe static void* <get_FatVtable_>g__S_GetPSig_0|7_0(void* ptr__)
				{
					return ((CoreCLR.V60.StoredSigMethodDesc_32*)ptr__)->GetPSig();
				}

				[CompilerGenerated]
				internal unsafe static uint <get_FatVtable_>g__S_GetCSig_1|7_1(void* ptr__)
				{
					return ((CoreCLR.V60.StoredSigMethodDesc_32*)ptr__)->GetCSig();
				}

				public CoreCLR.V60.MethodDesc @base;

				public unsafe void* m_pSig;

				public uint m_cSig;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterface]
			public struct FCallMethodDescPtr
			{
				public static IntPtr[] CurrentVtable { get; } = ((IntPtr.Size == 8) ? CoreCLR.V60.FCallMethodDesc_64.FatVtable_ : CoreCLR.V60.FCallMethodDesc_32.FatVtable_);

				public static int CurrentSize { get; } = ((IntPtr.Size == 8) ? sizeof(CoreCLR.V60.FCallMethodDesc_64) : sizeof(CoreCLR.V60.FCallMethodDesc_32));

				private unsafe uint GetECallID()
				{
					delegate*<void*, uint> system.UInt32_u0020(System.Void*) = (void*)this.vtbl_[0];
					return calli(System.UInt32(System.Void*), this.ptr_, system.UInt32_u0020(System.Void*));
				}

				public uint m_dwECallID
				{
					[FatInterfaceIgnore]
					get
					{
						return this.GetECallID();
					}
				}

				[NullableContext(0)]
				public unsafe FCallMethodDescPtr(void* ptr, [Nullable(1)] IntPtr[] vtbl)
				{
					this.ptr_ = ptr;
					this.vtbl_ = vtbl;
				}

				[Nullable(0)]
				private unsafe readonly void* ptr_;

				private readonly IntPtr[] vtbl_;
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterfaceImpl(typeof(CoreCLR.V60.FCallMethodDescPtr))]
			public struct FCallMethodDesc_64
			{
				private uint GetECallID()
				{
					return this.m_dwECallID;
				}

				public static IntPtr[] FatVtable_
				{
					get
					{
						IntPtr[] array;
						if ((array = CoreCLR.V60.FCallMethodDesc_64.fatVtable_) == null)
						{
							array = (CoreCLR.V60.FCallMethodDesc_64.fatVtable_ = new IntPtr[] { (IntPtr)ldftn(<get_FatVtable_>g__S_GetECallID_0|6_0) });
						}
						return array;
					}
				}

				[NullableContext(0)]
				[CompilerGenerated]
				internal unsafe static uint <get_FatVtable_>g__S_GetECallID_0|6_0(void* ptr__)
				{
					return ((CoreCLR.V60.FCallMethodDesc_64*)ptr__)->GetECallID();
				}

				public CoreCLR.V60.MethodDesc @base;

				public uint m_dwECallID;

				public uint m_padding;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterfaceImpl(typeof(CoreCLR.V60.FCallMethodDescPtr))]
			public struct FCallMethodDesc_32
			{
				private uint GetECallID()
				{
					return this.m_dwECallID;
				}

				public static IntPtr[] FatVtable_
				{
					get
					{
						IntPtr[] array;
						if ((array = CoreCLR.V60.FCallMethodDesc_32.fatVtable_) == null)
						{
							array = (CoreCLR.V60.FCallMethodDesc_32.fatVtable_ = new IntPtr[] { (IntPtr)ldftn(<get_FatVtable_>g__S_GetECallID_0|5_0) });
						}
						return array;
					}
				}

				[NullableContext(0)]
				[CompilerGenerated]
				internal unsafe static uint <get_FatVtable_>g__S_GetECallID_0|5_0(void* ptr__)
				{
					return ((CoreCLR.V60.FCallMethodDesc_32*)ptr__)->GetECallID();
				}

				public CoreCLR.V60.MethodDesc @base;

				public uint m_dwECallID;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			public struct DynamicResolver
			{
			}

			[Flags]
			public enum DynamicMethodDesc_ExtendedFlags
			{
				Attrs = 65535,
				ILStubAttrs = 23,
				MemberAccessMask = 7,
				ReverseStub = 8,
				Static = 16,
				CALLIStub = 32,
				DelegateStub = 64,
				StructMarshalStub = 128,
				Unbreakable = 256,
				SignatureNeedsResture = 1024,
				StubNeedsCOMStarted = 2048,
				MulticastStub = 4096,
				UnboxingILStub = 8192,
				WrapperDelegateStub = 16384,
				UnmanagedCallersOnlyStub = 32768,
				ILStub = 65536,
				LCGMethod = 131072,
				StackArgSize = 268173312
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterface]
			public struct DynamicMethodDescPtr
			{
				public static IntPtr[] CurrentVtable { get; } = ((IntPtr.Size == 8) ? CoreCLR.V60.DynamicMethodDesc_64.FatVtable_ : CoreCLR.V60.DynamicMethodDesc_32.FatVtable_);

				public static int CurrentSize { get; } = ((IntPtr.Size == 8) ? sizeof(CoreCLR.V60.DynamicMethodDesc_64) : sizeof(CoreCLR.V60.DynamicMethodDesc_32));

				private unsafe CoreCLR.V60.DynamicMethodDesc_ExtendedFlags GetFlags()
				{
					delegate*<void*, CoreCLR.V60.DynamicMethodDesc_ExtendedFlags> monoMod.Core.Interop.CoreCLR/V60/DynamicMethodDesc_ExtendedFlags_u0020(System.Void*) = (void*)this.vtbl_[0];
					return calli(MonoMod.Core.Interop.CoreCLR/V60/DynamicMethodDesc_ExtendedFlags(System.Void*), this.ptr_, monoMod.Core.Interop.CoreCLR/V60/DynamicMethodDesc_ExtendedFlags_u0020(System.Void*));
				}

				public CoreCLR.V60.DynamicMethodDesc_ExtendedFlags Flags
				{
					get
					{
						return this.GetFlags();
					}
				}

				[NullableContext(0)]
				public unsafe DynamicMethodDescPtr(void* ptr, [Nullable(1)] IntPtr[] vtbl)
				{
					this.ptr_ = ptr;
					this.vtbl_ = vtbl;
				}

				[Nullable(0)]
				private unsafe readonly void* ptr_;

				private readonly IntPtr[] vtbl_;
			}

			[FatInterfaceImpl(typeof(CoreCLR.V60.DynamicMethodDescPtr))]
			public struct DynamicMethodDesc_64
			{
				private CoreCLR.V60.DynamicMethodDesc_ExtendedFlags GetFlags()
				{
					return (CoreCLR.V60.DynamicMethodDesc_ExtendedFlags)this.@base.m_dwExtendedFlags;
				}

				public CoreCLR.V60.DynamicMethodDesc_ExtendedFlags Flags
				{
					get
					{
						return this.GetFlags();
					}
				}

				[Nullable(1)]
				public static IntPtr[] FatVtable_
				{
					[NullableContext(1)]
					get
					{
						IntPtr[] array;
						if ((array = CoreCLR.V60.DynamicMethodDesc_64.fatVtable_) == null)
						{
							array = (CoreCLR.V60.DynamicMethodDesc_64.fatVtable_ = new IntPtr[] { (IntPtr)ldftn(<get_FatVtable_>g__S_GetFlags_0|8_0) });
						}
						return array;
					}
				}

				[CompilerGenerated]
				internal unsafe static CoreCLR.V60.DynamicMethodDesc_ExtendedFlags <get_FatVtable_>g__S_GetFlags_0|8_0(void* ptr__)
				{
					return ((CoreCLR.V60.DynamicMethodDesc_64*)ptr__)->GetFlags();
				}

				public CoreCLR.V60.StoredSigMethodDesc_64 @base;

				public unsafe byte* m_pszMethodName;

				public unsafe CoreCLR.V60.DynamicResolver* m_pResolver;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[FatInterfaceImpl(typeof(CoreCLR.V60.DynamicMethodDescPtr))]
			public struct DynamicMethodDesc_32
			{
				private CoreCLR.V60.DynamicMethodDesc_ExtendedFlags GetFlags()
				{
					return (CoreCLR.V60.DynamicMethodDesc_ExtendedFlags)this.m_dwExtendedFlags;
				}

				public CoreCLR.V60.DynamicMethodDesc_ExtendedFlags Flags
				{
					get
					{
						return this.GetFlags();
					}
				}

				[Nullable(1)]
				public static IntPtr[] FatVtable_
				{
					[NullableContext(1)]
					get
					{
						IntPtr[] array;
						if ((array = CoreCLR.V60.DynamicMethodDesc_32.fatVtable_) == null)
						{
							array = (CoreCLR.V60.DynamicMethodDesc_32.fatVtable_ = new IntPtr[] { (IntPtr)ldftn(<get_FatVtable_>g__S_GetFlags_0|9_0) });
						}
						return array;
					}
				}

				[CompilerGenerated]
				internal unsafe static CoreCLR.V60.DynamicMethodDesc_ExtendedFlags <get_FatVtable_>g__S_GetFlags_0|9_0(void* ptr__)
				{
					return ((CoreCLR.V60.DynamicMethodDesc_32*)ptr__)->GetFlags();
				}

				public CoreCLR.V60.StoredSigMethodDesc_32 @base;

				public unsafe byte* m_pszMethodName;

				public unsafe CoreCLR.V60.DynamicResolver* m_pResolver;

				public uint m_dwExtendedFlags;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterface]
			public struct ArrayMethodDescPtr
			{
				public static IntPtr[] CurrentVtable { get; } = ((IntPtr.Size == 8) ? CoreCLR.V60.ArrayMethodDesc_64.FatVtable_ : CoreCLR.V60.ArrayMethodDesc_32.FatVtable_);

				public static int CurrentSize { get; } = ((IntPtr.Size == 8) ? sizeof(CoreCLR.V60.ArrayMethodDesc_64) : sizeof(CoreCLR.V60.ArrayMethodDesc_32));

				[NullableContext(0)]
				public unsafe ArrayMethodDescPtr(void* ptr, [Nullable(1)] IntPtr[] vtbl)
				{
					this.ptr_ = ptr;
					this.vtbl_ = vtbl;
				}

				[Nullable(0)]
				private unsafe readonly void* ptr_;

				private readonly IntPtr[] vtbl_;
			}

			public enum ArrayFunc
			{
				Get,
				Set,
				Address,
				Ctor
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterfaceImpl(typeof(CoreCLR.V60.ArrayMethodDescPtr))]
			public struct ArrayMethodDesc_64
			{
				public static IntPtr[] FatVtable_
				{
					get
					{
						IntPtr[] array;
						if ((array = CoreCLR.V60.ArrayMethodDesc_64.fatVtable_) == null)
						{
							array = (CoreCLR.V60.ArrayMethodDesc_64.fatVtable_ = new IntPtr[0]);
						}
						return array;
					}
				}

				public CoreCLR.V60.StoredSigMethodDesc_64 @base;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterfaceImpl(typeof(CoreCLR.V60.ArrayMethodDescPtr))]
			public struct ArrayMethodDesc_32
			{
				public static IntPtr[] FatVtable_
				{
					get
					{
						IntPtr[] array;
						if ((array = CoreCLR.V60.ArrayMethodDesc_32.fatVtable_) == null)
						{
							array = (CoreCLR.V60.ArrayMethodDesc_32.fatVtable_ = new IntPtr[0]);
						}
						return array;
					}
				}

				public CoreCLR.V60.StoredSigMethodDesc_32 @base;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			public struct NDirectWriteableData
			{
			}

			[Flags]
			public enum NDirectMethodDesc_Flags : ushort
			{
				EarlyBound = 1,
				HasSuppressUnmanagedCodeAccess = 2,
				DefaultDllImportSearchPathIsCached = 4,
				IsMarshalingRequiredCached = 16,
				CachedMarshalingRequired = 32,
				NativeAnsi = 64,
				LastError = 128,
				NativeNoMangle = 256,
				VarArgs = 512,
				StdCall = 1024,
				ThisCall = 2048,
				IsQCall = 4096,
				DefaultDllImportSearchPathsStatus = 8192,
				NDirectPopulated = 32768
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterface]
			public struct NDirectMethodDescPtr
			{
				public static IntPtr[] CurrentVtable { get; } = ((PlatformDetection.Architecture == ArchitectureKind.x86) ? CoreCLR.V60.NDirectMethodDesc_x86.FatVtable_ : CoreCLR.V60.NDirectMethodDesc_other.FatVtable_);

				public static int CurrentSize { get; } = ((PlatformDetection.Architecture == ArchitectureKind.x86) ? sizeof(CoreCLR.V60.NDirectMethodDesc_x86) : sizeof(CoreCLR.V60.NDirectMethodDesc_other));

				[NullableContext(0)]
				public unsafe NDirectMethodDescPtr(void* ptr, [Nullable(1)] IntPtr[] vtbl)
				{
					this.ptr_ = ptr;
					this.vtbl_ = vtbl;
				}

				[Nullable(0)]
				private unsafe readonly void* ptr_;

				private readonly IntPtr[] vtbl_;
			}

			[FatInterfaceImpl(typeof(CoreCLR.V60.NDirectMethodDescPtr))]
			public struct NDirectMethodDesc_other
			{
				[Nullable(1)]
				public static IntPtr[] FatVtable_
				{
					[NullableContext(1)]
					get
					{
						IntPtr[] array;
						if ((array = CoreCLR.V60.NDirectMethodDesc_other.fatVtable_) == null)
						{
							array = (CoreCLR.V60.NDirectMethodDesc_other.fatVtable_ = new IntPtr[0]);
						}
						return array;
					}
				}

				public CoreCLR.V60.MethodDesc @base;

				private CoreCLR.V60.NDirectMethodDesc_other.NDirect ndirect;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;

				public struct NDirect
				{
					public unsafe void* m_pNativeNDirectTarget;

					public unsafe byte* m_pszEntrypointName;

					[NativeInteger]
					public UIntPtr union_pszLibName_dwECallID;

					public unsafe CoreCLR.V60.NDirectWriteableData* m_pWriteableData;

					public unsafe void* m_pImportThunkGlue;

					public uint m_DefaultDllImportSearchPathsAttributeValue;

					public CoreCLR.V60.NDirectMethodDesc_Flags m_wFlags;

					public unsafe CoreCLR.V60.MethodDesc* m_pStubMD;
				}
			}

			[FatInterfaceImpl(typeof(CoreCLR.V60.NDirectMethodDescPtr))]
			public struct NDirectMethodDesc_x86
			{
				[Nullable(1)]
				public static IntPtr[] FatVtable_
				{
					[NullableContext(1)]
					get
					{
						IntPtr[] array;
						if ((array = CoreCLR.V60.NDirectMethodDesc_x86.fatVtable_) == null)
						{
							array = (CoreCLR.V60.NDirectMethodDesc_x86.fatVtable_ = new IntPtr[0]);
						}
						return array;
					}
				}

				public CoreCLR.V60.MethodDesc @base;

				private CoreCLR.V60.NDirectMethodDesc_x86.NDirect ndirect;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;

				public struct NDirect
				{
					public unsafe void* m_pNativeNDirectTarget;

					public unsafe byte* m_pszEntrypointName;

					[NativeInteger]
					public UIntPtr union_pszLibName_dwECallID;

					public unsafe CoreCLR.V60.NDirectWriteableData* m_pWriteableData;

					public unsafe void* m_pImportThunkGlue;

					public uint m_DefaultDllImportSearchPathsAttributeValue;

					public CoreCLR.V60.NDirectMethodDesc_Flags m_wFlags;

					public ushort m_cbStackArgumentSize;

					public unsafe CoreCLR.V60.MethodDesc* m_pStubMD;
				}
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterface]
			public struct EEImplMethodDescPtr
			{
				public static IntPtr[] CurrentVtable { get; } = ((IntPtr.Size == 8) ? CoreCLR.V60.EEImplMethodDesc_64.FatVtable_ : CoreCLR.V60.EEImplMethodDesc_32.FatVtable_);

				public static int CurrentSize { get; } = ((IntPtr.Size == 8) ? sizeof(CoreCLR.V60.EEImplMethodDesc_64) : sizeof(CoreCLR.V60.EEImplMethodDesc_32));

				[NullableContext(0)]
				public unsafe EEImplMethodDescPtr(void* ptr, [Nullable(1)] IntPtr[] vtbl)
				{
					this.ptr_ = ptr;
					this.vtbl_ = vtbl;
				}

				[Nullable(0)]
				private unsafe readonly void* ptr_;

				private readonly IntPtr[] vtbl_;
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterfaceImpl(typeof(CoreCLR.V60.EEImplMethodDescPtr))]
			public struct EEImplMethodDesc_64
			{
				public static IntPtr[] FatVtable_
				{
					get
					{
						IntPtr[] array;
						if ((array = CoreCLR.V60.EEImplMethodDesc_64.fatVtable_) == null)
						{
							array = (CoreCLR.V60.EEImplMethodDesc_64.fatVtable_ = new IntPtr[0]);
						}
						return array;
					}
				}

				public CoreCLR.V60.StoredSigMethodDesc_64 @base;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterfaceImpl(typeof(CoreCLR.V60.EEImplMethodDescPtr))]
			public struct EEImplMethodDesc_32
			{
				public static IntPtr[] FatVtable_
				{
					get
					{
						IntPtr[] array;
						if ((array = CoreCLR.V60.EEImplMethodDesc_32.fatVtable_) == null)
						{
							array = (CoreCLR.V60.EEImplMethodDesc_32.fatVtable_ = new IntPtr[0]);
						}
						return array;
					}
				}

				public CoreCLR.V60.StoredSigMethodDesc_32 @base;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			public struct ComPlusCallMethodDesc
			{
				public CoreCLR.V60.MethodDesc @base;

				public unsafe void* m_pComPlusCallInfo;
			}

			public struct InstantiatedMethodDesc
			{
				public bool IMD_HasMethodInstantiation
				{
					get
					{
						return this.IMD_IsGenericMethodDefinition || this.m_pPerInstInfo != null;
					}
				}

				public bool IMD_IsGenericMethodDefinition
				{
					get
					{
						return (this.m_wFlags2 & CoreCLR.V60.InstantiatedMethodDesc.Flags.KindMask) == CoreCLR.V60.InstantiatedMethodDesc.Flags.GenericMethodDefinition;
					}
				}

				public bool IMD_IsWrapperStubWithInstantiations
				{
					get
					{
						return (this.m_wFlags2 & CoreCLR.V60.InstantiatedMethodDesc.Flags.KindMask) == CoreCLR.V60.InstantiatedMethodDesc.Flags.WrapperStubWithInstantiations;
					}
				}

				public unsafe CoreCLR.V60.MethodDesc* IMD_GetWrappedMethodDesc()
				{
					Helpers.Assert(this.IMD_IsWrapperStubWithInstantiations, null, "IMD_IsWrapperStubWithInstantiations");
					return (CoreCLR.V60.MethodDesc*)this.union_pDictLayout_pWrappedMethodDesc;
				}

				public CoreCLR.V60.MethodDesc @base;

				public unsafe void* union_pDictLayout_pWrappedMethodDesc;

				public unsafe CoreCLR.V60.Dictionary* m_pPerInstInfo;

				public CoreCLR.V60.InstantiatedMethodDesc.Flags m_wFlags2;

				public ushort m_wNumGenericArgs;

				[Flags]
				public enum Flags : ushort
				{
					KindMask = 7,
					GenericMethodDefinition = 0,
					UnsharedMethodInstantiation = 1,
					SharedMethodInstantiation = 2,
					WrapperStubWithInstantiations = 3,
					EnCAddedMethod = 7,
					Unrestored = 8,
					HasComPlusCallInfo = 16
				}
			}

			public struct Dictionary
			{
			}

			public struct Module
			{
			}

			public struct MethodTableWriteableData
			{
			}

			public struct VTableIndir2_t
			{
				public unsafe void* Value
				{
					get
					{
						return this.pCode;
					}
				}

				public unsafe void* pCode;
			}

			public struct VTableIndir_t
			{
				public unsafe CoreCLR.V60.VTableIndir2_t* Value;
			}

			private static class MultipurposeSlotHelpers
			{
				public unsafe static byte OffsetOfMp1()
				{
					CoreCLR.V60.MethodTable methodTable = default(CoreCLR.V60.MethodTable);
					return (byte)((long)((byte*)(&methodTable.union_pPerInstInfo_ElementTypeHnd_pMultipurposeSlot1) - (byte*)(&methodTable)));
				}

				public unsafe static byte OffsetOfMp2()
				{
					CoreCLR.V60.MethodTable methodTable = default(CoreCLR.V60.MethodTable);
					return (byte)((long)((byte*)(&methodTable.union_p_InterfaceMap_pMultipurposeSlot2) - (byte*)(&methodTable)));
				}

				public unsafe static byte RegularOffset(int index)
				{
					return (byte)(sizeof(CoreCLR.V60.MethodTable) + index * IntPtr.Size - 2 * IntPtr.Size);
				}
			}

			public struct MethodTable
			{
				public unsafe CoreCLR.V60.MethodTable* GetCanonicalMethodTable()
				{
					UIntPtr uintPtr = this.union_pEEClass_pCanonMT;
					if ((uintPtr & (UIntPtr)((IntPtr)2)) == 0)
					{
						return uintPtr;
					}
					if ((uintPtr & (UIntPtr)((IntPtr)1)) != 0)
					{
						return *(uintPtr - (UIntPtr)((IntPtr)3));
					}
					return uintPtr - 2;
				}

				public unsafe CoreCLR.V60.MethodDesc* GetParallelMethodDesc(CoreCLR.V60.MethodDesc* pDefMD)
				{
					return this.GetMethodDescForSlot((uint)pDefMD->SlotNumber);
				}

				public bool IsInterface
				{
					get
					{
						return (this.m_dwFlags & 983040U) == 786432U;
					}
				}

				public unsafe CoreCLR.V60.MethodDesc* GetMethodDescForSlot(uint slotNumber)
				{
					if (this.IsInterface)
					{
						this.GetNumVirtuals();
					}
					throw new NotImplementedException();
				}

				public unsafe void* GetRestoredSlot(uint slotNumber)
				{
					CoreCLR.V60.MethodTable* ptr = (CoreCLR.V60.MethodTable*)Unsafe.AsPointer<CoreCLR.V60.MethodTable>(ref this);
					void* slot;
					for (;;)
					{
						ptr = ptr->GetCanonicalMethodTable();
						slot = ptr->GetSlot(slotNumber);
						if (slot != null)
						{
							break;
						}
						ptr = ptr->GetParentMethodTable();
					}
					return slot;
				}

				public bool HasIndirectParent
				{
					get
					{
						return (this.m_dwFlags & 8388608U) > 0U;
					}
				}

				public unsafe CoreCLR.V60.MethodTable* GetParentMethodTable()
				{
					void* pParentMethodTable = this.m_pParentMethodTable;
					if (this.HasIndirectParent)
					{
						return *(IntPtr*)pParentMethodTable;
					}
					return (CoreCLR.V60.MethodTable*)pParentMethodTable;
				}

				public unsafe void* GetSlot(uint slotNumber)
				{
					IntPtr slotPtrRaw = this.GetSlotPtrRaw(slotNumber);
					if (slotNumber < (uint)this.GetNumVirtuals())
					{
						return slotPtrRaw.Value;
					}
					if ((this.m_wFlags2 & CoreCLR.V60.MethodTable.Flags2.IsZapped) != ~(CoreCLR.V60.MethodTable.Flags2.MultipurposeSlotsMask | CoreCLR.V60.MethodTable.Flags2.IsZapped | CoreCLR.V60.MethodTable.Flags2.IsPreRestored | CoreCLR.V60.MethodTable.Flags2.HasModuleDependencies | CoreCLR.V60.MethodTable.Flags2.IsIntrinsicType | CoreCLR.V60.MethodTable.Flags2.RequiresDispatchTokenFat | CoreCLR.V60.MethodTable.Flags2.HasCctor | CoreCLR.V60.MethodTable.Flags2.HasVirtualStaticMethods | CoreCLR.V60.MethodTable.Flags2.REquiresAlign8 | CoreCLR.V60.MethodTable.Flags2.HasBoxedRegularStatics | CoreCLR.V60.MethodTable.Flags2.HasSingleNonVirtualSlot | CoreCLR.V60.MethodTable.Flags2.DependsOnEquivalentOrForwardedStructs) && slotNumber >= (uint)this.GetNumVirtuals())
					{
						return slotPtrRaw.Value;
					}
					return *slotPtrRaw;
				}

				[return: NativeInteger]
				public unsafe IntPtr GetSlotPtrRaw(uint slotNum)
				{
					if (slotNum < (uint)this.GetNumVirtuals())
					{
						uint indexOfVtableIndirection = CoreCLR.V60.MethodTable.GetIndexOfVtableIndirection(slotNum);
						return CoreCLR.V60.MethodTable.VTableIndir_t__GetValueMaybeNullAtPtr(this.GetVtableIndirections() + (ulong)indexOfVtableIndirection * (ulong)((long)sizeof(CoreCLR.V60.VTableIndir_t)) / (ulong)sizeof(CoreCLR.V60.VTableIndir_t)) + (ulong)CoreCLR.V60.MethodTable.GetIndexAfterVtableIndirection(slotNum) * (ulong)((long)sizeof(CoreCLR.V60.VTableIndir2_t)) / (ulong)sizeof(CoreCLR.V60.VTableIndir2_t);
					}
					if (this.HasSingleNonVirtualSlot)
					{
						return this.GetNonVirtualSlotsPtr();
					}
					return this.GetNonVirtualSlotsArray() + (ulong)(slotNum - (uint)this.GetNumVirtuals()) * (ulong)((long)sizeof(void**)) / (ulong)sizeof(void**);
				}

				public ushort GetNumVirtuals()
				{
					return this.m_wNumVirtuals;
				}

				public static uint GetIndexOfVtableIndirection(uint slotNum)
				{
					return slotNum >> 3;
				}

				public unsafe CoreCLR.V60.VTableIndir_t* GetVtableIndirections()
				{
					return (CoreCLR.V60.VTableIndir_t*)((byte*)Unsafe.AsPointer<CoreCLR.V60.MethodTable>(ref this) + sizeof(CoreCLR.V60.MethodTable));
				}

				public unsafe static CoreCLR.V60.VTableIndir2_t* VTableIndir_t__GetValueMaybeNullAtPtr([NativeInteger] IntPtr @base)
				{
					return @base;
				}

				public static uint GetIndexAfterVtableIndirection(uint slotNum)
				{
					return slotNum & 7U;
				}

				public bool HasSingleNonVirtualSlot
				{
					get
					{
						return this.m_wFlags2.Has(CoreCLR.V60.MethodTable.Flags2.HasSingleNonVirtualSlot);
					}
				}

				[NullableContext(1)]
				[MultipurposeSlotOffsetTable(3, typeof(CoreCLR.V60.MultipurposeSlotHelpers))]
				private static byte[] GetNonVirtualSlotsOffsets()
				{
					return new byte[]
					{
						CoreCLR.V60.MultipurposeSlotHelpers.OffsetOfMp1(),
						CoreCLR.V60.MultipurposeSlotHelpers.OffsetOfMp2(),
						CoreCLR.V60.MultipurposeSlotHelpers.OffsetOfMp1(),
						CoreCLR.V60.MultipurposeSlotHelpers.RegularOffset(2),
						CoreCLR.V60.MultipurposeSlotHelpers.OffsetOfMp2(),
						CoreCLR.V60.MultipurposeSlotHelpers.RegularOffset(2),
						CoreCLR.V60.MultipurposeSlotHelpers.RegularOffset(2),
						CoreCLR.V60.MultipurposeSlotHelpers.RegularOffset(3)
					};
				}

				[return: NativeInteger]
				public IntPtr GetNonVirtualSlotsPtr()
				{
					return this.GetMultipurposeSlotPtr(CoreCLR.V60.MethodTable.Flags2.HasNonVirtualSlots, CoreCLR.V60.MethodTable.c_NonVirtualSlotsOffsets);
				}

				[NullableContext(1)]
				[return: NativeInteger]
				public unsafe IntPtr GetMultipurposeSlotPtr(CoreCLR.V60.MethodTable.Flags2 flag, byte[] offsets)
				{
					IntPtr intPtr = (IntPtr)((UIntPtr)offsets[(int)(this.m_wFlags2 & (flag - 1))]);
					if (intPtr >= (IntPtr)sizeof(CoreCLR.V60.MethodTable))
					{
						intPtr += (IntPtr)((UIntPtr)this.GetNumVTableIndirections() * (UIntPtr)((IntPtr)sizeof(CoreCLR.V60.VTableIndir_t)));
					}
					return (byte*)Unsafe.AsPointer<CoreCLR.V60.MethodTable>(ref this) + intPtr;
				}

				public unsafe void*** GetNonVirtualSlotsArray()
				{
					return this.GetNonVirtualSlotsPtr();
				}

				public uint GetNumVTableIndirections()
				{
					return CoreCLR.V60.MethodTable.GetNumVtableIndirections((uint)this.GetNumVirtuals());
				}

				public static uint GetNumVtableIndirections(uint numVirtuals)
				{
					return numVirtuals + 7U >> 3;
				}

				public uint m_dwFlags;

				public uint m_BaseSize;

				public CoreCLR.V60.MethodTable.Flags2 m_wFlags2;

				public ushort m_wToken;

				public ushort m_wNumVirtuals;

				public ushort m_wNumInterfaces;

				private unsafe void* m_pParentMethodTable;

				public unsafe CoreCLR.V60.Module* m_pLoaderModule;

				public unsafe CoreCLR.V60.MethodTableWriteableData* m_pWriteableData;

				public unsafe void* union_pEEClass_pCanonMT;

				public unsafe void* union_pPerInstInfo_ElementTypeHnd_pMultipurposeSlot1;

				public unsafe void* union_p_InterfaceMap_pMultipurposeSlot2;

				public const int VTABLE_SLOTS_PER_CHUNK = 8;

				public const int VTABLE_SLOTS_PER_CHUNK_LOG2 = 3;

				[Nullable(1)]
				private static readonly byte[] c_NonVirtualSlotsOffsets = CoreCLR.V60.MethodTable.GetNonVirtualSlotsOffsets();

				[Flags]
				public enum Flags2 : ushort
				{
					MultipurposeSlotsMask = 31,
					HasPerInstInfo = 1,
					HasInterfaceMap = 2,
					HasDispatchMapSlot = 4,
					HasNonVirtualSlots = 8,
					HasModuleOverride = 16,
					IsZapped = 32,
					IsPreRestored = 64,
					HasModuleDependencies = 128,
					IsIntrinsicType = 256,
					RequiresDispatchTokenFat = 512,
					HasCctor = 1024,
					HasVirtualStaticMethods = 2048,
					REquiresAlign8 = 4096,
					HasBoxedRegularStatics = 8192,
					HasSingleNonVirtualSlot = 16384,
					DependsOnEquivalentOrForwardedStructs = 32768
				}

				public enum UnionLowBits
				{
					EEClass,
					Invalid,
					MethodTable,
					Indirection
				}
			}
		}

		public class V70 : CoreCLR.V60
		{
			public new static class ICorJitInfoVtable
			{
				public const int AllocMemIndex = 159;

				public const int TotalVtableCount = 175;
			}
		}

		public class V80 : CoreCLR.V70
		{
			public new static class ICorJitInfoVtable
			{
				public const int AllocMemIndex = 154;

				public const int TotalVtableCount = 170;
			}
		}

		public class V90 : CoreCLR.V80
		{
			public new static class ICorJitInfoVtable
			{
				public const int AllocMemIndex = 158;

				public const int TotalVtableCount = 174;
			}
		}
	}
}
