using System;
using System.Runtime.CompilerServices;
using MonoMod.Core.Platforms.Architectures.AltEntryFactories;
using MonoMod.Core.Utils;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Architectures
{
	internal sealed class x86_64Arch : IArchitecture
	{
		public ArchitectureKind Target
		{
			get
			{
				return ArchitectureKind.x86_64;
			}
		}

		public ArchitectureFeature Features
		{
			get
			{
				return ArchitectureFeature.Immediate64 | ArchitectureFeature.CreateAltEntryPoint;
			}
		}

		[Nullable(1)]
		public BytePatternCollection KnownMethodThunks
		{
			[NullableContext(1)]
			get
			{
				return Helpers.GetOrInit<BytePatternCollection>(ref this.lazyKnownMethodThunks, x86_64Arch.createKnownMethodThunksFunc);
			}
		}

		[Nullable(1)]
		public IAltEntryFactory AltEntryFactory
		{
			[NullableContext(1)]
			get;
		}

		[NullableContext(1)]
		private static BytePatternCollection CreateKnownMethodThunks()
		{
			RuntimeKind runtime = PlatformDetection.Runtime;
			bool flag = runtime - RuntimeKind.Framework <= 1;
			if (flag)
			{
				BytePattern[] array = new BytePattern[14];
				array[0] = new BytePattern(new AddressMeaning(AddressKind.Abs64), true, new ushort[]
				{
					72, 133, 201, 116, 65280, 72, 139, 1, 73, 65280,
					65280, 65280, 65280, 65280, 65280, 65280, 65280, 65280, 73, 59,
					194, 116, 65280, 72, 184, 65282, 65282, 65282, 65282, 65282,
					65282, 65282, 65282
				});
				array[1] = new BytePattern(new AddressMeaning(AddressKind.Rel32, 5), true, new ushort[] { 233, 65282, 65282, 65282, 65282, 95 });
				array[2] = new BytePattern(new AddressMeaning(AddressKind.Abs64), false, new ushort[]
				{
					72, 184, 65282, 65282, 65282, 65282, 65282, 65282, 65282, 65282,
					255, 224
				});
				array[3] = new BytePattern(new AddressMeaning(AddressKind.Rel32, 19), false, new byte[]
				{
					240, byte.MaxValue, 0, 0, 0, 0, 0, 0, 0, 0,
					byte.MaxValue, byte.MaxValue, 240, byte.MaxValue, byte.MaxValue, 0, 0, 0, 0
				}, new byte[]
				{
					64, 184, 0, 0, 0, 0, 0, 0, 0, 0,
					102, byte.MaxValue, 0, 15, 133, 2, 2, 2, 2
				});
				array[4] = new BytePattern(new AddressMeaning(AddressKind.Abs64), false, new byte[]
				{
					240, byte.MaxValue, 0, 0, 0, 0, 0, 0, 0, 0,
					byte.MaxValue, byte.MaxValue, 240, byte.MaxValue, 0, byte.MaxValue, byte.MaxValue, 0, 0, 0,
					0, 0, 0, 0, 0, byte.MaxValue, byte.MaxValue
				}, new byte[]
				{
					64, 184, 0, 0, 0, 0, 0, 0, 0, 0,
					102, byte.MaxValue, 0, 116, 0, 72, 184, 2, 2, 2,
					2, 2, 2, 2, 2, byte.MaxValue, 224
				});
				array[5] = new BytePattern(new AddressMeaning(AddressKind.PrecodeFixupThunkRel32, 5), true, new ushort[] { 232, 65282, 65282, 65282, 65282, 94 });
				array[6] = new BytePattern(new AddressMeaning(AddressKind.PrecodeFixupThunkRel32, 5), true, new ushort[] { 232, 65282, 65282, 65282, 65282, 204 });
				array[7] = new BytePattern(new AddressMeaning(AddressKind.Indirect, 6), true, new byte[]
				{
					byte.MaxValue, byte.MaxValue, 0, 0, 0, 0, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue
				}, new byte[]
				{
					byte.MaxValue, 37, 2, 2, 2, 2, 76, 139, 21, 251,
					15, 0, 0, byte.MaxValue, 37, 253, 15, 0, 0
				});
				array[8] = new BytePattern(new AddressMeaning(AddressKind.PrecodeFixupThunkRel32 | AddressKind.Indirect, 13), true, new byte[]
				{
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 0,
					0, 0, 0
				}, new byte[]
				{
					76, 139, 21, 251, 15, 0, 0, byte.MaxValue, 37, 2,
					2, 2, 2
				});
				array[9] = new BytePattern(new AddressMeaning(AddressKind.Indirect, 18), true, new byte[]
				{
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 0, 0, 0, 0, byte.MaxValue, byte.MaxValue,
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue
				}, new byte[]
				{
					72, 139, 5, 249, 15, 0, 0, 102, byte.MaxValue, 8,
					116, 6, byte.MaxValue, 37, 2, 2, 2, 2, byte.MaxValue, 37,
					248, 15, 0, 0
				});
				array[10] = new BytePattern(new AddressMeaning(AddressKind.Indirect, 6), true, new byte[]
				{
					byte.MaxValue, byte.MaxValue, 0, 0, 0, 0, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue
				}, new byte[]
				{
					byte.MaxValue, 37, 2, 2, 2, 2, 76, 139, 21, 251,
					63, 0, 0, byte.MaxValue, 37, 253, 63, 0, 0
				});
				array[11] = new BytePattern(new AddressMeaning(AddressKind.PrecodeFixupThunkRel32 | AddressKind.Indirect, 13), true, new byte[]
				{
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 0,
					0, 0, 0
				}, new byte[]
				{
					76, 139, 21, 251, 63, 0, 0, byte.MaxValue, 37, 2,
					2, 2, 2
				});
				array[12] = new BytePattern(new AddressMeaning(AddressKind.Indirect, 18), true, new byte[]
				{
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 0, 0, 0, 0, byte.MaxValue, byte.MaxValue,
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue
				}, new byte[]
				{
					72, 139, 5, 249, 63, 0, 0, 102, byte.MaxValue, 8,
					116, 6, byte.MaxValue, 37, 2, 2, 2, 2, byte.MaxValue, 37,
					248, 63, 0, 0
				});
				return new BytePatternCollection(array);
			}
			return new BytePatternCollection(new BytePattern[0]);
		}

		[NullableContext(1)]
		public x86_64Arch(ISystem system)
		{
			this.system = system;
			this.AltEntryFactory = new IcedAltEntryFactory(system, 64);
		}

		public NativeDetourInfo ComputeDetourInfo([NativeInteger] IntPtr from, [NativeInteger] IntPtr to, int sizeHint)
		{
			x86Shared.FixSizeHint(ref sizeHint);
			NativeDetourInfo nativeDetourInfo;
			if (x86Shared.TryRel32Detour(from, to, sizeHint, out nativeDetourInfo))
			{
				return nativeDetourInfo;
			}
			IntPtr intPtr = from + (IntPtr)6;
			IntPtr intPtr2 = intPtr + (IntPtr)int.MinValue;
			if (intPtr2 > intPtr)
			{
				intPtr2 = (IntPtr)0;
			}
			IntPtr intPtr3 = intPtr + (IntPtr)int.MaxValue;
			if (intPtr3 < intPtr)
			{
				intPtr3 = (IntPtr)(-1);
			}
			PositionedAllocationRequest positionedAllocationRequest = new PositionedAllocationRequest(intPtr, intPtr2, intPtr3, new AllocationRequest(IntPtr.Size));
			IAllocatedMemory allocatedMemory;
			if (sizeHint >= x86_64Arch.Rel32Ind64Kind.Instance.Size && this.system.MemoryAllocator.TryAllocateInRange(positionedAllocationRequest, out allocatedMemory))
			{
				return new NativeDetourInfo(from, to, x86_64Arch.Rel32Ind64Kind.Instance, allocatedMemory);
			}
			if (sizeHint < x86_64Arch.Abs64Kind.Instance.Size)
			{
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(79, 1, out flag);
				if (flag)
				{
					debugLogWarningStringHandler.AppendLiteral("Size too small for all known detour kinds; defaulting to Abs64. provided size: ");
					debugLogWarningStringHandler.AppendFormatted<int>(sizeHint);
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
			}
			return new NativeDetourInfo(from, to, x86_64Arch.Abs64Kind.Instance, null);
		}

		public int GetDetourBytes(NativeDetourInfo info, global::System.Span<byte> buffer, [Nullable(2)] out IDisposable allocHandle)
		{
			return DetourKindBase.GetDetourBytes(info, buffer, out allocHandle);
		}

		public NativeDetourInfo ComputeRetargetInfo(NativeDetourInfo detour, IntPtr to, int maxSizeHint = -1)
		{
			x86Shared.FixSizeHint(ref maxSizeHint);
			NativeDetourInfo nativeDetourInfo;
			if (DetourKindBase.TryFindRetargetInfo(detour, to, maxSizeHint, out nativeDetourInfo))
			{
				return nativeDetourInfo;
			}
			return this.ComputeDetourInfo(detour.From, to, maxSizeHint);
		}

		public int GetRetargetBytes(NativeDetourInfo original, NativeDetourInfo retarget, global::System.Span<byte> buffer, [Nullable(2)] out IDisposable allocationHandle, out bool needsRepatch, out bool disposeOldAlloc)
		{
			return DetourKindBase.DoRetarget(original, retarget, buffer, out allocationHandle, out needsRepatch, out disposeOldAlloc);
		}

		private unsafe static global::System.ReadOnlySpan<byte> VtblProxyStubWin
		{
			get
			{
				return new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.74A985B69363DA038C215EBDC95C2DEC71CCAA7EEE4FE7A349F688FAF77E0BAE), 16);
			}
		}

		private unsafe static global::System.ReadOnlySpan<byte> VtblProxyStubSysV
		{
			get
			{
				return new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.D9C90CF616457576F11A613E9AD2971E209CEC74951E276569E1097ADFEB26D5), 16);
			}
		}

		[return: Nullable(new byte[] { 0, 1 })]
		public global::System.ReadOnlyMemory<IAllocatedMemory> CreateNativeVtableProxyStubs(IntPtr vtableBase, int vtableSize)
		{
			global::System.ReadOnlySpan<byte> readOnlySpan;
			int num;
			bool flag;
			if (PlatformDetection.OS.Is(OSKind.Windows))
			{
				readOnlySpan = x86_64Arch.VtblProxyStubWin;
				num = 9;
				flag = true;
			}
			else
			{
				readOnlySpan = x86_64Arch.VtblProxyStubSysV;
				num = 9;
				flag = true;
			}
			return Shared.CreateVtableStubs(this.system, vtableBase, vtableSize, readOnlySpan, num, flag);
		}

		private unsafe static global::System.ReadOnlySpan<byte> SpecEntryStub
		{
			get
			{
				return new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.EF9D8CA5CBE169340066F08BC0A169D0779A59AAABC495DBE3FF991D03534944), 23);
			}
		}

		[NullableContext(1)]
		public unsafe IAllocatedMemory CreateSpecialEntryStub(IntPtr target, IntPtr argument)
		{
			int length = x86_64Arch.SpecEntryStub.Length;
			global::System.Span<byte> span = new global::System.Span<byte>(stackalloc byte[(UIntPtr)length], length);
			x86_64Arch.SpecEntryStub.CopyTo(span);
			Unsafe.WriteUnaligned<IntPtr>(span[12], target);
			Unsafe.WriteUnaligned<IntPtr>(span[2], argument);
			return Shared.CreateSingleExecutableStub(this.system, span);
		}

		[Nullable(2)]
		private BytePatternCollection lazyKnownMethodThunks;

		[Nullable(1)]
		private static readonly Func<BytePatternCollection> createKnownMethodThunksFunc = new Func<BytePatternCollection>(x86_64Arch.CreateKnownMethodThunks);

		[Nullable(1)]
		private readonly ISystem system;

		private const int VtblProxyStubIdxOffs = 9;

		private const bool VtblProxyStubIdxPremul = true;

		private const int SpecEntryStubArgOffs = 2;

		private const int SpecEntryStubTargetOffs = 12;

		[NullableContext(2)]
		[Nullable(0)]
		private sealed class Abs64Kind : DetourKindBase
		{
			public override int Size
			{
				get
				{
					return 14;
				}
			}

			public unsafe override int GetBytes(IntPtr from, IntPtr to, [Nullable(0)] global::System.Span<byte> buffer, object data, out IDisposable allocHandle)
			{
				*buffer[0] = byte.MaxValue;
				*buffer[1] = 37;
				Unsafe.WriteUnaligned<int>(buffer[2], 0);
				Unsafe.WriteUnaligned<long>(buffer[6], (long)to);
				allocHandle = null;
				return this.Size;
			}

			public override bool TryGetRetargetInfo(NativeDetourInfo orig, IntPtr to, int maxSize, out NativeDetourInfo retargetInfo)
			{
				NativeDetourInfo nativeDetourInfo = orig;
				nativeDetourInfo.To = to;
				retargetInfo = nativeDetourInfo;
				return true;
			}

			public override int DoRetarget(NativeDetourInfo origInfo, IntPtr to, [Nullable(0)] global::System.Span<byte> buffer, object data, out IDisposable allocationHandle, out bool needsRepatch, out bool disposeOldAlloc)
			{
				needsRepatch = true;
				disposeOldAlloc = true;
				return this.GetBytes(origInfo.From, to, buffer, data, out allocationHandle);
			}

			[Nullable(1)]
			public static readonly x86_64Arch.Abs64Kind Instance = new x86_64Arch.Abs64Kind();
		}

		[NullableContext(2)]
		[Nullable(0)]
		private sealed class Rel32Ind64Kind : DetourKindBase
		{
			public override int Size
			{
				get
				{
					return 6;
				}
			}

			public unsafe override int GetBytes(IntPtr from, IntPtr to, [Nullable(0)] global::System.Span<byte> buffer, object data, out IDisposable allocHandle)
			{
				Helpers.ThrowIfArgumentNull<object>(data, "data");
				IAllocatedMemory allocatedMemory = (IAllocatedMemory)data;
				*buffer[0] = byte.MaxValue;
				*buffer[1] = 37;
				Unsafe.WriteUnaligned<int>(buffer[2], (int)(allocatedMemory.BaseAddress - (from + (IntPtr)6)));
				Unsafe.WriteUnaligned<IntPtr>(allocatedMemory.Memory[0], to);
				allocHandle = allocatedMemory;
				return this.Size;
			}

			public override bool TryGetRetargetInfo(NativeDetourInfo orig, IntPtr to, int maxSize, out NativeDetourInfo retargetInfo)
			{
				NativeDetourInfo nativeDetourInfo = orig;
				nativeDetourInfo.To = to;
				retargetInfo = nativeDetourInfo;
				return true;
			}

			public override int DoRetarget(NativeDetourInfo origInfo, IntPtr to, [Nullable(0)] global::System.Span<byte> buffer, object data, out IDisposable allocationHandle, out bool needsRepatch, out bool disposeOldAlloc)
			{
				if (origInfo.InternalKind == this)
				{
					needsRepatch = false;
					disposeOldAlloc = false;
					Helpers.ThrowIfArgumentNull<object>(data, "data");
					IAllocatedMemory allocatedMemory = (IAllocatedMemory)data;
					Unsafe.WriteUnaligned<IntPtr>(allocatedMemory.Memory[0], to);
					allocationHandle = allocatedMemory;
					return 0;
				}
				needsRepatch = true;
				disposeOldAlloc = true;
				return this.GetBytes(origInfo.From, to, buffer, data, out allocationHandle);
			}

			[Nullable(1)]
			public static readonly x86_64Arch.Rel32Ind64Kind Instance = new x86_64Arch.Rel32Ind64Kind();
		}
	}
}
