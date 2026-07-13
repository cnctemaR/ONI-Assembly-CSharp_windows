using System;
using System.Runtime.CompilerServices;
using MonoMod.Core.Utils;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms
{
	[NullableContext(1)]
	internal interface IArchitecture
	{
		ArchitectureKind Target { get; }

		ArchitectureFeature Features { get; }

		BytePatternCollection KnownMethodThunks { get; }

		IAltEntryFactory AltEntryFactory { get; }

		NativeDetourInfo ComputeDetourInfo(IntPtr from, IntPtr target, int maxSizeHint = -1);

		[NullableContext(0)]
		int GetDetourBytes(NativeDetourInfo info, global::System.Span<byte> buffer, [Nullable(2)] out IDisposable allocationHandle);

		NativeDetourInfo ComputeRetargetInfo(NativeDetourInfo detour, IntPtr target, int maxSizeHint = -1);

		[NullableContext(0)]
		int GetRetargetBytes(NativeDetourInfo original, NativeDetourInfo retarget, global::System.Span<byte> buffer, [Nullable(2)] out IDisposable allocationHandle, out bool needsRepatch, out bool disposeOldAlloc);

		[return: Nullable(new byte[] { 0, 1 })]
		global::System.ReadOnlyMemory<IAllocatedMemory> CreateNativeVtableProxyStubs(IntPtr vtableBase, int vtableSize);

		IAllocatedMemory CreateSpecialEntryStub(IntPtr target, IntPtr argument);
	}
}
