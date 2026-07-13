using System;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Architectures
{
	internal abstract class DetourKindBase : INativeDetourKind
	{
		public abstract int Size { get; }

		[NullableContext(2)]
		public abstract int GetBytes(IntPtr from, IntPtr to, [Nullable(0)] global::System.Span<byte> buffer, object data, out IDisposable allocHandle);

		public static int GetDetourBytes(NativeDetourInfo info, global::System.Span<byte> buffer, [Nullable(2)] out IDisposable allocHandle)
		{
			Helpers.ThrowIfArgumentNull<INativeDetourKind>(info.InternalKind, "info.InternalKind");
			if (buffer.Length < info.Size)
			{
				throw new ArgumentException("Buffer too short", "buffer");
			}
			return ((DetourKindBase)info.InternalKind).GetBytes(info.From, info.To, buffer, info.InternalData, out allocHandle);
		}

		public abstract bool TryGetRetargetInfo(NativeDetourInfo orig, IntPtr to, int maxSize, out NativeDetourInfo retargetInfo);

		public static bool TryFindRetargetInfo(NativeDetourInfo info, IntPtr to, int maxSize, out NativeDetourInfo retargetInfo)
		{
			Helpers.ThrowIfArgumentNull<INativeDetourKind>(info.InternalKind, "info.InternalKind");
			return ((DetourKindBase)info.InternalKind).TryGetRetargetInfo(info, to, maxSize, out retargetInfo);
		}

		[NullableContext(2)]
		public abstract int DoRetarget(NativeDetourInfo origInfo, IntPtr to, [Nullable(0)] global::System.Span<byte> buffer, object data, out IDisposable allocationHandle, out bool needsRepatch, out bool disposeOldAlloc);

		public static int DoRetarget(NativeDetourInfo orig, NativeDetourInfo info, global::System.Span<byte> buffer, [Nullable(2)] out IDisposable allocationHandle, out bool needsRepatch, out bool disposeOldAlloc)
		{
			Helpers.ThrowIfArgumentNull<INativeDetourKind>(info.InternalKind, "info.InternalKind");
			if (buffer.Length < info.Size)
			{
				throw new ArgumentException("Buffer too short", "buffer");
			}
			return ((DetourKindBase)info.InternalKind).DoRetarget(orig, info.To, buffer, info.InternalData, out allocationHandle, out needsRepatch, out disposeOldAlloc);
		}
	}
}
