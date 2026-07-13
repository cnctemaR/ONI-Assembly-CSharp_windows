using System;
using System.Runtime.CompilerServices;
using MonoMod.Core.Interop.Attributes;
using MonoMod.Utils;

namespace MonoMod.Core.Interop
{
	internal static class Fx
	{
		public static class V48
		{
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

			public enum MethodDescClassification : ushort
			{
				ClassificationMask = 7,
				HasNonVtableSlot,
				MethodImpl = 16,
				Static = 32,
				Intercepted = 64,
				RequiresLinktimeCheck = 128,
				RequiresInheritanceCheck = 256,
				ParentRequiresInheritanceCheck = 512,
				Duplicate = 1024,
				VerifiedState = 2048,
				Verifiable = 4096,
				NotInline = 8192,
				Synchronized = 16384,
				RequiresFullSlotNumber = 32768
			}

			public struct MethodImpl
			{
				public unsafe uint* pdwSlots;

				public unsafe Fx.V48.MethodDesc* pImplementedMD;
			}

			public struct MethodDesc
			{
				public ushort SlotNumber
				{
					get
					{
						if (!this.m_wFlags.Has(Fx.V48.MethodDescClassification.RequiresFullSlotNumber))
						{
							return this.m_wSlotNumber & 1023;
						}
						return this.m_wSlotNumber;
					}
				}

				public Fx.V48.MethodClassification Classification
				{
					get
					{
						return (Fx.V48.MethodClassification)(this.m_wFlags & Fx.V48.MethodDescClassification.ClassificationMask);
					}
				}

				public unsafe Fx.V48.MethodDescChunk* MethodDescChunk
				{
					get
					{
						return (Fx.V48.MethodDescChunk*)Unsafe.AsPointer<Fx.V48.MethodDesc>(Unsafe.SubtractByteOffset<Fx.V48.MethodDesc>(ref this, (UIntPtr)((IntPtr)sizeof(Fx.V48.MethodDescChunk) + (IntPtr)((UIntPtr)this.m_chunkIndex * Fx.V48.MethodDesc.Alignment))));
					}
				}

				public unsafe Fx.V48.MethodTable* MethodTable
				{
					get
					{
						return this.MethodDescChunk->MethodTable;
					}
				}

				public unsafe void* GetMethodEntryPoint()
				{
					if (this.HasNonVtableSlot)
					{
						UIntPtr baseSize = this.GetBaseSize();
						return *(IntPtr*)((byte*)Unsafe.AsPointer<Fx.V48.MethodDesc>(ref this) + (ulong)baseSize);
					}
					return this.MethodTable->GetSlot((uint)this.SlotNumber);
				}

				public unsafe bool TryAsInstantiated(out Fx.V48.InstantiatedMethodDesc* md)
				{
					if (this.Classification == Fx.V48.MethodClassification.Instantiated)
					{
						md = Unsafe.AsPointer<Fx.V48.MethodDesc>(ref this);
						return true;
					}
					md = default(Fx.V48.InstantiatedMethodDesc*);
					return false;
				}

				[return: NativeInteger]
				public unsafe UIntPtr SizeOf(bool includeNonVtable = true, bool includeMethodImpl = true, bool includeComPlus = true, bool includeNativeCode = true)
				{
					UIntPtr uintPtr = this.GetBaseSize() + (UIntPtr)((includeNonVtable && this.m_wFlags.Has(Fx.V48.MethodDescClassification.HasNonVtableSlot)) ? ((IntPtr)sizeof(void*)) : ((IntPtr)0)) + (UIntPtr)((includeMethodImpl && this.m_wFlags.Has(Fx.V48.MethodDescClassification.MethodImpl)) ? ((IntPtr)sizeof(Fx.V48.MethodImpl)) : ((IntPtr)0));
					if (includeNativeCode && this.HasNativeCodeSlot)
					{
						uintPtr += (UIntPtr)(((*Unsafe.As<Fx.V48.MethodDesc, UIntPtr>(Unsafe.AddByteOffset<Fx.V48.MethodDesc>(ref this, uintPtr)) & (UIntPtr)((IntPtr)1)) != 0) ? (sizeof(void*) + sizeof(void*)) : sizeof(void*));
					}
					if (includeComPlus && this.IsGenericComPlusCall)
					{
						uintPtr += (UIntPtr)Fx.V48.ComPlusCallInfoPtr.CurrentSize;
					}
					return uintPtr;
				}

				public unsafe void* GetNativeCode()
				{
					if (this.HasNativeCodeSlot)
					{
						UIntPtr uintPtr = ((Fx.V48.RelativePointer*)this.GetAddrOfNativeCodeSlot())->Value & ~1;
						if (uintPtr != 0)
						{
							return uintPtr;
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
						return this.m_wFlags.Has(Fx.V48.MethodDescClassification.HasNonVtableSlot);
					}
				}

				public bool HasStableEntryPoint
				{
					get
					{
						return this.m_bFlags2.Has(Fx.V48.MethodDesc.Flags2.HasStableEntryPoint);
					}
				}

				public bool HasPrecode
				{
					get
					{
						return this.m_bFlags2.Has(Fx.V48.MethodDesc.Flags2.HasPrecode);
					}
				}

				public bool HasNativeCodeSlot
				{
					get
					{
						return this.m_bFlags2.Has(Fx.V48.MethodDesc.Flags2.HasNativeCodeSlot);
					}
				}

				public bool IsUnboxingStub
				{
					get
					{
						return this.m_bFlags2.Has(Fx.V48.MethodDesc.Flags2.IsUnboxingStub);
					}
				}

				public unsafe bool HasMethodInstantiation
				{
					get
					{
						Fx.V48.InstantiatedMethodDesc* ptr;
						return this.TryAsInstantiated(out ptr) && ptr->IMD_HasMethodInstantiation;
					}
				}

				public unsafe bool IsGenericMethodDefinition
				{
					get
					{
						Fx.V48.InstantiatedMethodDesc* ptr;
						return this.TryAsInstantiated(out ptr) && ptr->IMD_IsGenericMethodDefinition;
					}
				}

				public unsafe bool IsInstantiatingStub
				{
					get
					{
						Fx.V48.InstantiatedMethodDesc* ptr;
						return !this.IsUnboxingStub && this.TryAsInstantiated(out ptr) && ptr->IMD_IsWrapperStubWithInstantiations;
					}
				}

				public unsafe bool IsGenericComPlusCall
				{
					get
					{
						Fx.V48.InstantiatedMethodDesc* ptr;
						return this.TryAsInstantiated(out ptr) && ptr->IMD_HasComPlusCallInfo;
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

				public unsafe static Fx.V48.MethodDesc* FindTightlyBoundWrappedMethodDesc(Fx.V48.MethodDesc* pMD)
				{
					Fx.V48.InstantiatedMethodDesc* ptr;
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
						pMD = Fx.V48.MethodDesc.GetNextIntroducedMethod(pMD);
					}
					return pMD;
				}

				public unsafe static Fx.V48.MethodDesc* GetNextIntroducedMethod(Fx.V48.MethodDesc* pMD)
				{
					Fx.V48.MethodDescChunk* ptr = pMD->MethodDescChunk;
					UIntPtr uintPtr = pMD + pMD->SizeOf(true, true, true, true) / (UIntPtr)sizeof(Fx.V48.MethodDesc);
					UIntPtr uintPtr2 = ptr + ptr->SizeOf / (UIntPtr)sizeof(Fx.V48.MethodDescChunk);
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

				public unsafe Fx.V48.MethodTable* GetCanonicalMethodTable()
				{
					return this.MethodTable->GetCanonicalMethodTable();
				}

				public unsafe void* GetAddrOfNativeCodeSlot()
				{
					UIntPtr uintPtr = this.SizeOf(true, true, false, false);
					return Unsafe.AsPointer<Fx.V48.MethodDesc>(Unsafe.AddByteOffset<Fx.V48.MethodDesc>(ref this, uintPtr));
				}

				[return: NativeInteger]
				public UIntPtr GetBaseSize()
				{
					return Fx.V48.MethodDesc.GetBaseSize(this.Classification);
				}

				[return: NativeInteger]
				public static UIntPtr GetBaseSize(Fx.V48.MethodClassification classification)
				{
					return Fx.V48.MethodDesc.s_ClassificationSizeTable[(int)classification];
				}

				[NativeInteger]
				public static readonly UIntPtr Alignment = (UIntPtr)((IntPtr.Size == 8) ? ((IntPtr)8) : ((IntPtr)4));

				public Fx.V48.MethodDesc.Flags3 m_wFlags3AndTokenRemainder;

				public byte m_chunkIndex;

				public Fx.V48.MethodDesc.Flags2 m_bFlags2;

				public const ushort PackedSlot_SlotMask = 1023;

				public const ushort PackedSlot_NameHashMask = 64512;

				public ushort m_wSlotNumber;

				public Fx.V48.MethodDescClassification m_wFlags;

				[NativeInteger]
				[Nullable(1)]
				private static readonly UIntPtr[] s_ClassificationSizeTable = new UIntPtr[]
				{
					(UIntPtr)((IntPtr)sizeof(Fx.V48.MethodDesc)),
					(UIntPtr)((IntPtr)Fx.V48.FCallMethodDescPtr.CurrentSize),
					(UIntPtr)((IntPtr)Fx.V48.NDirectMethodDescPtr.CurrentSize),
					(UIntPtr)((IntPtr)Fx.V48.EEImplMethodDescPtr.CurrentSize),
					(UIntPtr)((IntPtr)Fx.V48.ArrayMethodDescPtr.CurrentSize),
					(UIntPtr)((IntPtr)sizeof(Fx.V48.InstantiatedMethodDesc)),
					(UIntPtr)((IntPtr)sizeof(Fx.V48.ComPlusCallMethodDesc)),
					(UIntPtr)((IntPtr)Fx.V48.DynamicMethodDescPtr.CurrentSize)
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
					HasNativeCodeSlot = 8,
					TransparencyMask = 48,
					TransparencyUnknown = 0,
					TransparencyTransparent = 16,
					TransparencyCritical = 32,
					TransparencyTreatAsSafe = 48,
					CASDemandsOnly = 64,
					HostProtectionLinkChecksOnly = 128
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
				} = ((IntPtr.Size == 8) ? Fx.V48.StoredSigMethodDesc_64.FatVtable_ : Fx.V48.StoredSigMethodDesc_32.FatVtable_);

				public static int CurrentSize { get; } = ((IntPtr.Size == 8) ? sizeof(Fx.V48.StoredSigMethodDesc_64) : sizeof(Fx.V48.StoredSigMethodDesc_32));

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

			[FatInterfaceImpl(typeof(Fx.V48.StoredSigMethodDescPtr))]
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
						if ((array = Fx.V48.StoredSigMethodDesc_64.fatVtable_) == null)
						{
							array = (Fx.V48.StoredSigMethodDesc_64.fatVtable_ = new IntPtr[]
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
					return ((Fx.V48.StoredSigMethodDesc_64*)ptr__)->GetPSig();
				}

				[CompilerGenerated]
				internal unsafe static uint <get_FatVtable_>g__S_GetCSig_1|8_1(void* ptr__)
				{
					return ((Fx.V48.StoredSigMethodDesc_64*)ptr__)->GetCSig();
				}

				public Fx.V48.MethodDesc @base;

				public unsafe void* m_pSig;

				public uint m_cSig;

				public uint m_dwExtendedFlags;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[FatInterfaceImpl(typeof(Fx.V48.StoredSigMethodDescPtr))]
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
						if ((array = Fx.V48.StoredSigMethodDesc_32.fatVtable_) == null)
						{
							array = (Fx.V48.StoredSigMethodDesc_32.fatVtable_ = new IntPtr[]
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
					return ((Fx.V48.StoredSigMethodDesc_32*)ptr__)->GetPSig();
				}

				[CompilerGenerated]
				internal unsafe static uint <get_FatVtable_>g__S_GetCSig_1|7_1(void* ptr__)
				{
					return ((Fx.V48.StoredSigMethodDesc_32*)ptr__)->GetCSig();
				}

				public Fx.V48.MethodDesc @base;

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
				public static IntPtr[] CurrentVtable { get; } = ((IntPtr.Size == 8) ? Fx.V48.FCallMethodDesc_64.FatVtable_ : Fx.V48.FCallMethodDesc_32.FatVtable_);

				public static int CurrentSize { get; } = ((IntPtr.Size == 8) ? sizeof(Fx.V48.FCallMethodDesc_64) : sizeof(Fx.V48.FCallMethodDesc_32));

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
			[FatInterfaceImpl(typeof(Fx.V48.FCallMethodDescPtr))]
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
						if ((array = Fx.V48.FCallMethodDesc_64.fatVtable_) == null)
						{
							array = (Fx.V48.FCallMethodDesc_64.fatVtable_ = new IntPtr[] { (IntPtr)ldftn(<get_FatVtable_>g__S_GetECallID_0|6_0) });
						}
						return array;
					}
				}

				[NullableContext(0)]
				[CompilerGenerated]
				internal unsafe static uint <get_FatVtable_>g__S_GetECallID_0|6_0(void* ptr__)
				{
					return ((Fx.V48.FCallMethodDesc_64*)ptr__)->GetECallID();
				}

				public Fx.V48.MethodDesc @base;

				public uint m_dwECallID;

				public uint m_padding;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterfaceImpl(typeof(Fx.V48.FCallMethodDescPtr))]
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
						if ((array = Fx.V48.FCallMethodDesc_32.fatVtable_) == null)
						{
							array = (Fx.V48.FCallMethodDesc_32.fatVtable_ = new IntPtr[] { (IntPtr)ldftn(<get_FatVtable_>g__S_GetECallID_0|5_0) });
						}
						return array;
					}
				}

				[NullableContext(0)]
				[CompilerGenerated]
				internal unsafe static uint <get_FatVtable_>g__S_GetECallID_0|5_0(void* ptr__)
				{
					return ((Fx.V48.FCallMethodDesc_32*)ptr__)->GetECallID();
				}

				public Fx.V48.MethodDesc @base;

				public uint m_dwECallID;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			public struct DynamicResolver
			{
			}

			[Flags]
			public enum DynamicMethodDesc_ExtendedFlags : uint
			{
				Attrs = 65535U,
				ILStubAttrs = 23U,
				MemberAccessMask = 7U,
				ReverseStub = 8U,
				Static = 16U,
				CALLIStub = 32U,
				DelegateStub = 64U,
				CopyCtorArgs = 128U,
				Unbreakable = 256U,
				DelegateCOMStub = 512U,
				SignatureNeedsResture = 1024U,
				StubNeedsCOMStarted = 2048U,
				MulticastStub = 4096U,
				UnboxingILStub = 8192U,
				ILStub = 65536U,
				LCGMethod = 131072U,
				StackArgSize = 4294705152U
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterface]
			public struct DynamicMethodDescPtr
			{
				public static IntPtr[] CurrentVtable { get; } = ((IntPtr.Size == 8) ? Fx.V48.DynamicMethodDesc_64.FatVtable_ : Fx.V48.DynamicMethodDesc_32.FatVtable_);

				public static int CurrentSize { get; } = ((IntPtr.Size == 8) ? sizeof(Fx.V48.DynamicMethodDesc_64) : sizeof(Fx.V48.DynamicMethodDesc_32));

				private unsafe Fx.V48.DynamicMethodDesc_ExtendedFlags GetFlags()
				{
					delegate*<void*, Fx.V48.DynamicMethodDesc_ExtendedFlags> monoMod.Core.Interop.Fx/V48/DynamicMethodDesc_ExtendedFlags_u0020(System.Void*) = (void*)this.vtbl_[0];
					return calli(MonoMod.Core.Interop.Fx/V48/DynamicMethodDesc_ExtendedFlags(System.Void*), this.ptr_, monoMod.Core.Interop.Fx/V48/DynamicMethodDesc_ExtendedFlags_u0020(System.Void*));
				}

				public Fx.V48.DynamicMethodDesc_ExtendedFlags Flags
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

			[FatInterfaceImpl(typeof(Fx.V48.DynamicMethodDescPtr))]
			public struct DynamicMethodDesc_64
			{
				private Fx.V48.DynamicMethodDesc_ExtendedFlags GetFlags()
				{
					return (Fx.V48.DynamicMethodDesc_ExtendedFlags)this.@base.m_dwExtendedFlags;
				}

				public Fx.V48.DynamicMethodDesc_ExtendedFlags Flags
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
						if ((array = Fx.V48.DynamicMethodDesc_64.fatVtable_) == null)
						{
							array = (Fx.V48.DynamicMethodDesc_64.fatVtable_ = new IntPtr[] { (IntPtr)ldftn(<get_FatVtable_>g__S_GetFlags_0|8_0) });
						}
						return array;
					}
				}

				[CompilerGenerated]
				internal unsafe static Fx.V48.DynamicMethodDesc_ExtendedFlags <get_FatVtable_>g__S_GetFlags_0|8_0(void* ptr__)
				{
					return ((Fx.V48.DynamicMethodDesc_64*)ptr__)->GetFlags();
				}

				public Fx.V48.StoredSigMethodDesc_64 @base;

				public unsafe byte* m_pszMethodName;

				public unsafe Fx.V48.DynamicResolver* m_pResolver;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[FatInterfaceImpl(typeof(Fx.V48.DynamicMethodDescPtr))]
			public struct DynamicMethodDesc_32
			{
				private Fx.V48.DynamicMethodDesc_ExtendedFlags GetFlags()
				{
					return (Fx.V48.DynamicMethodDesc_ExtendedFlags)this.m_dwExtendedFlags;
				}

				public Fx.V48.DynamicMethodDesc_ExtendedFlags Flags
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
						if ((array = Fx.V48.DynamicMethodDesc_32.fatVtable_) == null)
						{
							array = (Fx.V48.DynamicMethodDesc_32.fatVtable_ = new IntPtr[] { (IntPtr)ldftn(<get_FatVtable_>g__S_GetFlags_0|9_0) });
						}
						return array;
					}
				}

				[CompilerGenerated]
				internal unsafe static Fx.V48.DynamicMethodDesc_ExtendedFlags <get_FatVtable_>g__S_GetFlags_0|9_0(void* ptr__)
				{
					return ((Fx.V48.DynamicMethodDesc_32*)ptr__)->GetFlags();
				}

				public Fx.V48.StoredSigMethodDesc_32 @base;

				public unsafe byte* m_pszMethodName;

				public unsafe Fx.V48.DynamicResolver* m_pResolver;

				public uint m_dwExtendedFlags;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterface]
			public struct ArrayMethodDescPtr
			{
				public static IntPtr[] CurrentVtable { get; } = ((IntPtr.Size == 8) ? Fx.V48.ArrayMethodDesc_64.FatVtable_ : Fx.V48.ArrayMethodDesc_32.FatVtable_);

				public static int CurrentSize { get; } = ((IntPtr.Size == 8) ? sizeof(Fx.V48.ArrayMethodDesc_64) : sizeof(Fx.V48.ArrayMethodDesc_32));

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
			[FatInterfaceImpl(typeof(Fx.V48.ArrayMethodDescPtr))]
			public struct ArrayMethodDesc_64
			{
				public static IntPtr[] FatVtable_
				{
					get
					{
						IntPtr[] array;
						if ((array = Fx.V48.ArrayMethodDesc_64.fatVtable_) == null)
						{
							array = (Fx.V48.ArrayMethodDesc_64.fatVtable_ = new IntPtr[0]);
						}
						return array;
					}
				}

				public Fx.V48.StoredSigMethodDesc_64 @base;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterfaceImpl(typeof(Fx.V48.ArrayMethodDescPtr))]
			public struct ArrayMethodDesc_32
			{
				public static IntPtr[] FatVtable_
				{
					get
					{
						IntPtr[] array;
						if ((array = Fx.V48.ArrayMethodDesc_32.fatVtable_) == null)
						{
							array = (Fx.V48.ArrayMethodDesc_32.fatVtable_ = new IntPtr[0]);
						}
						return array;
					}
				}

				public Fx.V48.StoredSigMethodDesc_32 @base;

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
				public static IntPtr[] CurrentVtable { get; } = ((PlatformDetection.Architecture == ArchitectureKind.x86) ? Fx.V48.NDirectMethodDesc_x86.FatVtable_ : Fx.V48.NDirectMethodDesc_other.FatVtable_);

				public static int CurrentSize { get; } = ((PlatformDetection.Architecture == ArchitectureKind.x86) ? sizeof(Fx.V48.NDirectMethodDesc_x86) : sizeof(Fx.V48.NDirectMethodDesc_other));

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

			[FatInterfaceImpl(typeof(Fx.V48.NDirectMethodDescPtr))]
			public struct NDirectMethodDesc_other
			{
				[Nullable(1)]
				public static IntPtr[] FatVtable_
				{
					[NullableContext(1)]
					get
					{
						IntPtr[] array;
						if ((array = Fx.V48.NDirectMethodDesc_other.fatVtable_) == null)
						{
							array = (Fx.V48.NDirectMethodDesc_other.fatVtable_ = new IntPtr[0]);
						}
						return array;
					}
				}

				public Fx.V48.MethodDesc @base;

				private Fx.V48.NDirectMethodDesc_other.NDirect ndirect;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;

				public struct NDirect
				{
					public unsafe void* m_pNativeNDirectTarget;

					public unsafe byte* m_pszEntrypointName;

					[NativeInteger]
					public UIntPtr union_pszLibName_dwECallID;

					public unsafe Fx.V48.NDirectWriteableData* m_pWriteableData;

					public unsafe void* m_pImportThunkGlue;

					public uint m_DefaultDllImportSearchPathsAttributeValue;

					public Fx.V48.NDirectMethodDesc_Flags m_wFlags;

					public unsafe Fx.V48.MethodDesc* m_pStubMD;
				}
			}

			[FatInterfaceImpl(typeof(Fx.V48.NDirectMethodDescPtr))]
			public struct NDirectMethodDesc_x86
			{
				[Nullable(1)]
				public static IntPtr[] FatVtable_
				{
					[NullableContext(1)]
					get
					{
						IntPtr[] array;
						if ((array = Fx.V48.NDirectMethodDesc_x86.fatVtable_) == null)
						{
							array = (Fx.V48.NDirectMethodDesc_x86.fatVtable_ = new IntPtr[0]);
						}
						return array;
					}
				}

				public Fx.V48.MethodDesc @base;

				private Fx.V48.NDirectMethodDesc_x86.NDirect ndirect;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;

				public struct NDirect
				{
					public unsafe void* m_pNativeNDirectTarget;

					public unsafe byte* m_pszEntrypointName;

					[NativeInteger]
					public UIntPtr union_pszLibName_dwECallID;

					public unsafe Fx.V48.NDirectWriteableData* m_pWriteableData;

					public unsafe void* m_pImportThunkGlue;

					public uint m_DefaultDllImportSearchPathsAttributeValue;

					public Fx.V48.NDirectMethodDesc_Flags m_wFlags;

					public ushort m_cbStackArgumentSize;

					public unsafe Fx.V48.MethodDesc* m_pStubMD;
				}
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterface]
			public struct EEImplMethodDescPtr
			{
				public static IntPtr[] CurrentVtable { get; } = ((IntPtr.Size == 8) ? Fx.V48.EEImplMethodDesc_64.FatVtable_ : Fx.V48.EEImplMethodDesc_32.FatVtable_);

				public static int CurrentSize { get; } = ((IntPtr.Size == 8) ? sizeof(Fx.V48.EEImplMethodDesc_64) : sizeof(Fx.V48.EEImplMethodDesc_32));

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
			[FatInterfaceImpl(typeof(Fx.V48.EEImplMethodDescPtr))]
			public struct EEImplMethodDesc_64
			{
				public static IntPtr[] FatVtable_
				{
					get
					{
						IntPtr[] array;
						if ((array = Fx.V48.EEImplMethodDesc_64.fatVtable_) == null)
						{
							array = (Fx.V48.EEImplMethodDesc_64.fatVtable_ = new IntPtr[0]);
						}
						return array;
					}
				}

				public Fx.V48.StoredSigMethodDesc_64 @base;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[NullableContext(1)]
			[Nullable(0)]
			[FatInterfaceImpl(typeof(Fx.V48.EEImplMethodDescPtr))]
			public struct EEImplMethodDesc_32
			{
				public static IntPtr[] FatVtable_
				{
					get
					{
						IntPtr[] array;
						if ((array = Fx.V48.EEImplMethodDesc_32.fatVtable_) == null)
						{
							array = (Fx.V48.EEImplMethodDesc_32.fatVtable_ = new IntPtr[0]);
						}
						return array;
					}
				}

				public Fx.V48.StoredSigMethodDesc_32 @base;

				[Nullable(2)]
				private static IntPtr[] fatVtable_;
			}

			[FatInterface]
			public struct ComPlusCallInfoPtr
			{
				public static int CurrentSize { get; } = ((PlatformDetection.Architecture == ArchitectureKind.x86) ? sizeof(Fx.V48.ComPlusCallInfo_x86) : sizeof(Fx.V48.ComPlusCallInfo_other));

				public unsafe ComPlusCallInfoPtr(void* ptr, [Nullable(1)] IntPtr[] vtbl)
				{
					this.ptr_ = ptr;
					this.vtbl_ = vtbl;
				}

				private unsafe readonly void* ptr_;

				[Nullable(1)]
				private readonly IntPtr[] vtbl_;
			}

			public struct ComPlusCallInfo_x86
			{
				public unsafe void* union_m_pILStub_pEventProviderMD;

				public unsafe Fx.V48.MethodTable* m_pInterfaceMT;

				public byte m_flags;

				public ushort m_cachedComSlot;

				public ushort m_cbStackArgumentSize;

				public unsafe void* union_m_pRetThunk_pInterceptStub;

				private Fx.V48.RelativePointer m_pStubMD;
			}

			public struct ComPlusCallInfo_other
			{
				public unsafe void* union_m_pILStub_pEventProviderMD;

				public unsafe Fx.V48.MethodTable* m_pInterfaceMT;

				public byte m_flags;

				public ushort m_cachedComSlot;

				private Fx.V48.RelativePointer m_pStubMD;
			}

			public struct ComPlusCallMethodDesc
			{
				public Fx.V48.MethodDesc @base;

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
						return (this.m_wFlags2 & Fx.V48.InstantiatedMethodDesc.Flags.KindMask) == Fx.V48.InstantiatedMethodDesc.Flags.GenericMethodDefinition;
					}
				}

				public bool IMD_IsWrapperStubWithInstantiations
				{
					get
					{
						return (this.m_wFlags2 & Fx.V48.InstantiatedMethodDesc.Flags.KindMask) == Fx.V48.InstantiatedMethodDesc.Flags.WrapperStubWithInstantiations;
					}
				}

				public bool IMD_HasComPlusCallInfo
				{
					get
					{
						return this.m_wFlags2.Has(Fx.V48.InstantiatedMethodDesc.Flags.HasComPlusCallInfo);
					}
				}

				public unsafe Fx.V48.MethodDesc* IMD_GetWrappedMethodDesc()
				{
					Helpers.Assert(this.IMD_IsWrapperStubWithInstantiations, null, "IMD_IsWrapperStubWithInstantiations");
					return (Fx.V48.MethodDesc*)this.union_pDictLayout_pWrappedMethodDesc;
				}

				public Fx.V48.MethodDesc @base;

				public unsafe void* union_pDictLayout_pWrappedMethodDesc;

				public unsafe Fx.V48.Dictionary* m_pPerInstInfo;

				public Fx.V48.InstantiatedMethodDesc.Flags m_wFlags2;

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

			public struct RelativeFixupPointer
			{
				public unsafe void* Value
				{
					get
					{
						IntPtr intPtr = this.value;
						if (intPtr == 0)
						{
							return null;
						}
						IntPtr intPtr2 = (byte*)Unsafe.AsPointer<Fx.V48.RelativeFixupPointer>(ref this) + intPtr;
						if ((intPtr2 & (IntPtr)1) != 0)
						{
							intPtr2 = *(intPtr2 - (IntPtr)1);
						}
						return intPtr2;
					}
				}

				[NativeInteger]
				private readonly IntPtr value;
			}

			public struct MethodDescChunk
			{
				public unsafe Fx.V48.MethodTable* MethodTable
				{
					get
					{
						return (Fx.V48.MethodTable*)this.m_methodTable.Value;
					}
				}

				public unsafe Fx.V48.MethodDesc* FirstMethodDesc
				{
					get
					{
						return (Fx.V48.MethodDesc*)((byte*)Unsafe.AsPointer<Fx.V48.MethodDescChunk>(ref this) + sizeof(Fx.V48.MethodDescChunk));
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
						return (UIntPtr)((IntPtr)sizeof(Fx.V48.MethodDescChunk) + (IntPtr)((UIntPtr)this.Size * Fx.V48.MethodDesc.Alignment));
					}
				}

				public Fx.V48.RelativeFixupPointer m_methodTable;

				public unsafe Fx.V48.MethodDescChunk* m_next;

				public byte m_size;

				public byte m_count;

				public Fx.V48.MethodDescChunk.Flags m_flagsAndTokenRange;

				[Flags]
				public enum Flags : ushort
				{
					TokenRangeMask = 1023,
					HasCompactEntrypoints = 16384,
					IsZapped = 32768
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
							return Unsafe.AsPointer<Fx.V48.RelativePointer>(Unsafe.AddByteOffset<Fx.V48.RelativePointer>(ref this, delta));
						}
						return null;
					}
				}

				[NativeInteger]
				private IntPtr m_delta;
			}

			public struct MethodTable
			{
				public Fx.V48.MethodTable.WFlagsHigh FlagsHigh
				{
					get
					{
						return (Fx.V48.MethodTable.WFlagsHigh)(this.m_dwFlags & 4294901760U);
					}
				}

				public Fx.V48.MethodTable.WFlagsLow FlagsLow
				{
					get
					{
						if (!this.FlagsHigh.Has((Fx.V48.MethodTable.WFlagsHigh)2147483648U))
						{
							return (Fx.V48.MethodTable.WFlagsLow)(this.m_dwFlags & 65535U);
						}
						return Fx.V48.MethodTable.WFlagsLow.StaticsMask_NonDynamic;
					}
				}

				public int ComponentSize
				{
					get
					{
						if (!this.FlagsHigh.Has((Fx.V48.MethodTable.WFlagsHigh)2147483648U))
						{
							return 0;
						}
						return (int)(this.m_dwFlags & 65535U);
					}
				}

				public unsafe Fx.V48.MethodTable* GetCanonicalMethodTable()
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

				public unsafe Fx.V48.MethodDesc* GetParallelMethodDesc(Fx.V48.MethodDesc* pDefMD)
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

				public unsafe Fx.V48.MethodDesc* GetMethodDescForSlot(uint slotNumber)
				{
					if (this.IsInterface)
					{
						this.GetNumVirtuals();
					}
					throw new NotImplementedException();
				}

				public unsafe void* GetRestoredSlot(uint slotNumber)
				{
					Fx.V48.MethodTable* ptr = (Fx.V48.MethodTable*)Unsafe.AsPointer<Fx.V48.MethodTable>(ref this);
					void* slot;
					for (;;)
					{
						ptr = ptr->GetCanonicalMethodTable();
						slot = ptr->GetSlot(slotNumber);
						if (slot != null)
						{
							break;
						}
						ptr = ptr->ParentMethodTable;
					}
					return slot;
				}

				public bool HasIndirectParent
				{
					get
					{
						return this.FlagsHigh.Has(Fx.V48.MethodTable.WFlagsHigh.HasIndirectParent);
					}
				}

				public unsafe Fx.V48.MethodTable* ParentMethodTable
				{
					get
					{
						void* pParentMethodTable = this.m_pParentMethodTable;
						if (this.HasIndirectParent)
						{
							return (Fx.V48.MethodTable*)((Fx.V48.MethodTable*)pParentMethodTable)->m_pParentMethodTable;
						}
						return (Fx.V48.MethodTable*)pParentMethodTable;
					}
				}

				public unsafe void* GetSlot(uint slotNumber)
				{
					return *this.GetSlotPtrRaw(slotNumber);
				}

				[return: NativeInteger]
				public unsafe IntPtr GetSlotPtrRaw(uint slotNum)
				{
					if (slotNum < (uint)this.GetNumVirtuals())
					{
						uint indexOfVtableIndirection = Fx.V48.MethodTable.GetIndexOfVtableIndirection(slotNum);
						void** ptr = *(IntPtr*)(this.GetVtableIndirections() + (ulong)indexOfVtableIndirection * (ulong)((long)sizeof(void*)) / (ulong)sizeof(void*));
						return ptr + (ulong)Fx.V48.MethodTable.GetIndexAfterVtableIndirection(slotNum) * (ulong)((long)sizeof(void*)) / (ulong)sizeof(void*);
					}
					if (this.HasSingleNonVirtualSlot)
					{
						return this.GetNonVirtualSlotsPtr();
					}
					return this.GetNonVirtualSlotsArray() + (ulong)(slotNum - (uint)this.GetNumVirtuals()) * (ulong)((long)sizeof(void*)) / (ulong)sizeof(void*);
				}

				public ushort GetNumVirtuals()
				{
					return this.m_wNumVirtuals;
				}

				public static uint GetIndexOfVtableIndirection(uint slotNum)
				{
					return slotNum >> 3;
				}

				public unsafe void** GetVtableIndirections()
				{
					return (void**)((byte*)Unsafe.AsPointer<Fx.V48.MethodTable>(ref this) + sizeof(Fx.V48.MethodTable));
				}

				public static uint GetIndexAfterVtableIndirection(uint slotNum)
				{
					return slotNum & 7U;
				}

				public bool HasSingleNonVirtualSlot
				{
					get
					{
						return this.m_wFlags2.Has(Fx.V48.MethodTable.Flags2.HasSingleNonVirtualSlot);
					}
				}

				[NullableContext(1)]
				[MultipurposeSlotOffsetTable(3, typeof(Fx.V48.MethodTable.MultipurposeSlotHelpers))]
				private static byte[] GetNonVirtualSlotsOffsets()
				{
					return new byte[]
					{
						Fx.V48.MethodTable.MultipurposeSlotHelpers.OffsetOfMp1(),
						Fx.V48.MethodTable.MultipurposeSlotHelpers.OffsetOfMp2(),
						Fx.V48.MethodTable.MultipurposeSlotHelpers.OffsetOfMp1(),
						Fx.V48.MethodTable.MultipurposeSlotHelpers.RegularOffset(2),
						Fx.V48.MethodTable.MultipurposeSlotHelpers.OffsetOfMp2(),
						Fx.V48.MethodTable.MultipurposeSlotHelpers.RegularOffset(2),
						Fx.V48.MethodTable.MultipurposeSlotHelpers.RegularOffset(2),
						Fx.V48.MethodTable.MultipurposeSlotHelpers.RegularOffset(3)
					};
				}

				[return: NativeInteger]
				public IntPtr GetNonVirtualSlotsPtr()
				{
					return this.GetMultipurposeSlotPtr(Fx.V48.MethodTable.Flags2.HasNonVirtualSlots, Fx.V48.MethodTable.c_NonVirtualSlotsOffsets);
				}

				[NullableContext(1)]
				[return: NativeInteger]
				public unsafe IntPtr GetMultipurposeSlotPtr(Fx.V48.MethodTable.Flags2 flag, byte[] offsets)
				{
					IntPtr intPtr = (IntPtr)((UIntPtr)offsets[(int)(this.m_wFlags2 & (flag - 1))]);
					if (intPtr >= (IntPtr)sizeof(Fx.V48.MethodTable))
					{
						intPtr += (IntPtr)((UIntPtr)this.GetNumVTableIndirections() * (UIntPtr)((IntPtr)sizeof(void**)));
					}
					return (byte*)Unsafe.AsPointer<Fx.V48.MethodTable>(ref this) + intPtr;
				}

				public unsafe void** GetNonVirtualSlotsArray()
				{
					return (void**)this.GetNonVirtualSlotsPtr().Value;
				}

				public uint GetNumVTableIndirections()
				{
					return Fx.V48.MethodTable.GetNumVtableIndirections((uint)this.GetNumVirtuals());
				}

				public static uint GetNumVtableIndirections(uint numVirtuals)
				{
					return numVirtuals + 7U >> 3;
				}

				private uint m_dwFlags;

				public uint m_BaseSize;

				public Fx.V48.MethodTable.Flags2 m_wFlags2;

				public ushort m_wToken;

				public ushort m_wNumVirtuals;

				public ushort m_wNumInterfaces;

				private unsafe void* m_pParentMethodTable;

				public unsafe Fx.V48.Module* m_pLoaderModule;

				public unsafe Fx.V48.MethodTableWriteableData* m_pWriteableData;

				public unsafe void* union_pEEClass_pCanonMT;

				public unsafe void* union_pPerInstInfo_ElementTypeHnd_pMultipurposeSlot1;

				public unsafe void* union_p_InterfaceMap_pMultipurposeSlot2;

				public const int VTABLE_SLOTS_PER_CHUNK = 8;

				public const int VTABLE_SLOTS_PER_CHUNK_LOG2 = 3;

				[Nullable(1)]
				private static readonly byte[] c_NonVirtualSlotsOffsets = Fx.V48.MethodTable.GetNonVirtualSlotsOffsets();

				[Flags]
				public enum WFlagsLow : uint
				{
					UNUSED_ComponentSize_1 = 1U,
					StaticsMask = 6U,
					StaticsMask_NonDynamic = 0U,
					StaticsMask_Dynamic = 2U,
					StaticsMask_Generics = 4U,
					StaticsMask_CrossModuleGenerics = 6U,
					StaticsMask_IfGenericsThenCrossModule = 2U,
					NotInPZM = 8U,
					GenericsMask = 48U,
					GenericsMask_NonGeneric = 0U,
					GenericsMask_GenericInst = 16U,
					GenericsMask_SharedInst = 32U,
					GenericsMask_TypicalInst = 48U,
					ContextStatic = 64U,
					HasRemotingVtsInfo = 128U,
					HasVariance = 256U,
					HasDefaultCtor = 512U,
					HasPreciseInitCctors = 1024U,
					IsHFA_IsRegStructPassed = 2048U,
					IsByRefLike = 4096U,
					UNUSED_ComponentSize_5 = 8192U,
					UNUSED_ComponentSize_6 = 16384U,
					UNUSED_ComponentSize_7 = 32768U,
					StringArrayValues = 0U
				}

				[Flags]
				public enum WFlagsHigh : uint
				{
					Category_Mask = 983040U,
					Category_Class = 0U,
					Category_Unused_1 = 65536U,
					Category_MarshalByRef_Mask = 917504U,
					Category_MarshalByRef = 131072U,
					Category_Contextful = 196608U,
					Category_ValueType = 262144U,
					Category_ValueType_Mask = 786432U,
					Category_Nullable = 327680U,
					Category_PrimitiveValueType = 393216U,
					Category_TruePrimitive = 458752U,
					Category_Array = 524288U,
					Category_Array_Mask = 786432U,
					Category_IfArrayThenSzArray = 131072U,
					Category_Interface = 786432U,
					Category_Unused_2 = 851968U,
					Category_TransparentProxy = 917504U,
					Category_AsyncPin = 983040U,
					Category_ElementTypeMask = 917504U,
					HasFinalizer = 1048576U,
					IfNotInterfaceThenMarshalable = 2097152U,
					IfInterfaceThenHasGuidInfo = 2097152U,
					ICastable = 4194304U,
					HasIndirectParent = 8388608U,
					ContainsPointers = 16777216U,
					HasTypeEquivalence = 33554432U,
					HasRCWPerTypeData = 67108864U,
					HasCriticalFinalizer = 134217728U,
					Collectible = 268435456U,
					ContainsGenericVariables = 536870912U,
					ComObject = 1073741824U,
					HasComponentSize = 2147483648U,
					NonTrivialInterfaceCast = 1078460416U
				}

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
					NoSecurityProperties = 256,
					RequiresDispatchTokenFat = 512,
					HasCctor = 1024,
					HasCCWTemplate = 2048,
					RequiresAlign8 = 4096,
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

				private static class MultipurposeSlotHelpers
				{
					public unsafe static byte OffsetOfMp1()
					{
						Fx.V48.MethodTable methodTable = default(Fx.V48.MethodTable);
						return (byte)((long)((byte*)(&methodTable.union_pPerInstInfo_ElementTypeHnd_pMultipurposeSlot1) - (byte*)(&methodTable)));
					}

					public unsafe static byte OffsetOfMp2()
					{
						Fx.V48.MethodTable methodTable = default(Fx.V48.MethodTable);
						return (byte)((long)((byte*)(&methodTable.union_p_InterfaceMap_pMultipurposeSlot2) - (byte*)(&methodTable)));
					}

					public unsafe static byte RegularOffset(int index)
					{
						return (byte)(sizeof(Fx.V48.MethodTable) + index * IntPtr.Size - 2 * IntPtr.Size);
					}
				}
			}
		}
	}
}
