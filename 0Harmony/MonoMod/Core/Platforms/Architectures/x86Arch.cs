using System;
using System.Runtime.CompilerServices;
using MonoMod.Core.Platforms.Architectures.AltEntryFactories;
using MonoMod.Core.Utils;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Architectures
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class x86Arch : IArchitecture
	{
		public ArchitectureKind Target
		{
			get
			{
				return ArchitectureKind.x86;
			}
		}

		public ArchitectureFeature Features
		{
			get
			{
				return ArchitectureFeature.CreateAltEntryPoint;
			}
		}

		public BytePatternCollection KnownMethodThunks
		{
			get
			{
				return Helpers.GetOrInit<BytePatternCollection>(ref this.lazyKnownMethodThunks, x86Arch.createKnownMethodThunksFunc);
			}
		}

		public IAltEntryFactory AltEntryFactory { get; }

		private static BytePatternCollection CreateKnownMethodThunks()
		{
			RuntimeKind runtime = PlatformDetection.Runtime;
			bool flag = runtime - RuntimeKind.Framework <= 1;
			if (flag)
			{
				BytePattern[] array = new BytePattern[7];
				array[0] = new BytePattern(new AddressMeaning(AddressKind.Rel32, 16), new ushort[]
				{
					184, 65280, 65280, 65280, 65280, 144, 232, 65280, 65280, 65280,
					65280, 233, 65282, 65282, 65282, 65282
				});
				array[1] = new BytePattern(new AddressMeaning(AddressKind.Rel32, 5), true, new ushort[] { 233, 65282, 65282, 65282, 65282, 95 });
				array[2] = new BytePattern(new AddressMeaning(AddressKind.PrecodeFixupThunkRel32, 5), true, new ushort[] { 232, 65282, 65282, 65282, 65282, 94 });
				array[3] = new BytePattern(new AddressMeaning(AddressKind.PrecodeFixupThunkRel32, 5), true, new ushort[] { 232, 65282, 65282, 65282, 65282, 204 });
				array[4] = new BytePattern(new AddressMeaning(AddressKind.Abs32 | AddressKind.Indirect), true, new ushort[]
				{
					255, 37, 65282, 65282, 65282, 65282, 161, 65280, 65280, 65280,
					65280, 255, 37, 65280, 65280, 65280, 65280
				});
				array[5] = new BytePattern(new AddressMeaning(AddressKind.Abs32 | AddressKind.PrecodeFixupThunkRel32 | AddressKind.Indirect), true, new ushort[]
				{
					161, 65280, 65280, 65280, 65280, 255, 37, 65282, 65282, 65282,
					65282
				});
				return new BytePatternCollection(array);
			}
			return new BytePatternCollection(new BytePattern[0]);
		}

		public NativeDetourInfo ComputeDetourInfo(IntPtr from, IntPtr to, int maxSizeHint = -1)
		{
			x86Shared.FixSizeHint(ref maxSizeHint);
			NativeDetourInfo nativeDetourInfo;
			if (x86Shared.TryRel32Detour(from, to, maxSizeHint, out nativeDetourInfo))
			{
				return nativeDetourInfo;
			}
			if (maxSizeHint < x86Arch.Abs32Kind.Instance.Size)
			{
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(79, 1, out flag);
				if (flag)
				{
					debugLogWarningStringHandler.AppendLiteral("Size too small for all known detour kinds; defaulting to Abs32. provided size: ");
					debugLogWarningStringHandler.AppendFormatted<int>(maxSizeHint);
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
			}
			return new NativeDetourInfo(from, to, x86Arch.Abs32Kind.Instance, null);
		}

		[NullableContext(0)]
		public int GetDetourBytes(NativeDetourInfo info, global::System.Span<byte> buffer, [Nullable(2)] out IDisposable allocationHandle)
		{
			return DetourKindBase.GetDetourBytes(info, buffer, out allocationHandle);
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

		[NullableContext(0)]
		public int GetRetargetBytes(NativeDetourInfo original, NativeDetourInfo retarget, global::System.Span<byte> buffer, [Nullable(2)] out IDisposable allocationHandle, out bool needsRepatch, out bool disposeOldAlloc)
		{
			return DetourKindBase.DoRetarget(original, retarget, buffer, out allocationHandle, out needsRepatch, out disposeOldAlloc);
		}

		public x86Arch(ISystem system)
		{
			this.system = system;
			this.AltEntryFactory = new IcedAltEntryFactory(system, 32);
		}

		[Nullable(0)]
		private unsafe static global::System.ReadOnlySpan<byte> WinThisVtableProxyThunk
		{
			[NullableContext(0)]
			get
			{
				return new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.CC04E60244F11264BA0C35EBD477099E8E811C6267800C53D2A16574265D530A), 12);
			}
		}

		[return: Nullable(new byte[] { 0, 1 })]
		public global::System.ReadOnlyMemory<IAllocatedMemory> CreateNativeVtableProxyStubs(IntPtr vtableBase, int vtableSize)
		{
			OSKind kernel = PlatformDetection.OS.GetKernel();
			bool flag = true;
			if (kernel.Is(OSKind.Windows))
			{
				global::System.ReadOnlySpan<byte> winThisVtableProxyThunk = x86Arch.WinThisVtableProxyThunk;
				int num = 7;
				return Shared.CreateVtableStubs(this.system, vtableBase, vtableSize, winThisVtableProxyThunk, num, flag);
			}
			throw new PlatformNotSupportedException();
		}

		[Nullable(0)]
		private unsafe static global::System.ReadOnlySpan<byte> SpecEntryStub
		{
			[NullableContext(0)]
			get
			{
				return new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.73C5DDF3D1B50EF2973C84F1EFCA106392E86FA50DDE4274E0BB239D13E1B2C4), 12);
			}
		}

		public unsafe IAllocatedMemory CreateSpecialEntryStub(IntPtr target, IntPtr argument)
		{
			int length = x86Arch.SpecEntryStub.Length;
			global::System.Span<byte> span = new global::System.Span<byte>(stackalloc byte[(UIntPtr)length], length);
			x86Arch.SpecEntryStub.CopyTo(span);
			Unsafe.WriteUnaligned<IntPtr>(span[6], target);
			Unsafe.WriteUnaligned<IntPtr>(span[1], argument);
			return Shared.CreateSingleExecutableStub(this.system, span);
		}

		[Nullable(2)]
		private BytePatternCollection lazyKnownMethodThunks;

		private static readonly Func<BytePatternCollection> createKnownMethodThunksFunc = new Func<BytePatternCollection>(x86Arch.CreateKnownMethodThunks);

		private readonly ISystem system;

		private const int WinThisVtableThunkIndexOffs = 7;

		private const int SpecEntryStubArgOffs = 1;

		private const int SpecEntryStubTargetOffs = 6;

		[NullableContext(2)]
		[Nullable(0)]
		private sealed class Abs32Kind : DetourKindBase
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
				*buffer[0] = 104;
				Unsafe.WriteUnaligned<int>(buffer[1], *Unsafe.As<IntPtr, int>(ref to));
				*buffer[5] = 195;
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
			public static readonly x86Arch.Abs32Kind Instance = new x86Arch.Abs32Kind();
		}
	}
}
