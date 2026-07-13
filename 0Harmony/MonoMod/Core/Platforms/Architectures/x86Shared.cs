using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Platforms.Architectures
{
	internal static class x86Shared
	{
		public static void FixSizeHint(ref int sizeHint)
		{
			if (sizeHint < 0)
			{
				sizeHint = int.MaxValue;
			}
		}

		public unsafe static bool TryRel32Detour([NativeInteger] IntPtr from, [NativeInteger] IntPtr to, int sizeHint, out NativeDetourInfo info)
		{
			IntPtr intPtr = to - (from + (IntPtr)5);
			if (sizeHint >= x86Shared.Rel32Kind.Instance.Size && (x86Shared.Is32Bit((long)intPtr) || x86Shared.Is32Bit((long)(-(long)intPtr))) && *(from + 5) != 95)
			{
				info = new NativeDetourInfo(from, to, x86Shared.Rel32Kind.Instance, null);
				return true;
			}
			info = default(NativeDetourInfo);
			return false;
		}

		public static bool Is32Bit(long to)
		{
			return (to & 2147483647L) == to;
		}

		[NullableContext(2)]
		[Nullable(0)]
		public sealed class Rel32Kind : DetourKindBase
		{
			public override int Size
			{
				get
				{
					return 5;
				}
			}

			public unsafe override int GetBytes(IntPtr from, IntPtr to, [Nullable(0)] global::System.Span<byte> buffer, object data, out IDisposable allocHandle)
			{
				*buffer[0] = 233;
				Unsafe.WriteUnaligned<int>(buffer[1], (int)(to - (from + (IntPtr)5)));
				allocHandle = null;
				return this.Size;
			}

			public override bool TryGetRetargetInfo(NativeDetourInfo orig, IntPtr to, int maxSize, out NativeDetourInfo retargetInfo)
			{
				IntPtr intPtr = to - (orig.From + (IntPtr)5);
				if (x86Shared.Is32Bit((long)intPtr) || x86Shared.Is32Bit((long)(-(long)intPtr)))
				{
					retargetInfo = new NativeDetourInfo(orig.From, to, x86Shared.Rel32Kind.Instance, null);
					return true;
				}
				retargetInfo = default(NativeDetourInfo);
				return false;
			}

			public override int DoRetarget(NativeDetourInfo origInfo, IntPtr to, [Nullable(0)] global::System.Span<byte> buffer, object data, out IDisposable allocationHandle, out bool needsRepatch, out bool disposeOldAlloc)
			{
				needsRepatch = true;
				disposeOldAlloc = true;
				return this.GetBytes(origInfo.From, to, buffer, data, out allocationHandle);
			}

			[Nullable(1)]
			public static readonly x86Shared.Rel32Kind Instance = new x86Shared.Rel32Kind();
		}
	}
}
