using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoMod.Core.Utils;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Architectures
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class Arm64Arch : IArchitecture
	{
		public ArchitectureKind Target
		{
			get
			{
				return ArchitectureKind.Arm64;
			}
		}

		public ArchitectureFeature Features
		{
			get
			{
				return ArchitectureFeature.Immediate64;
			}
		}

		public BytePatternCollection KnownMethodThunks
		{
			get
			{
				Func<BytePatternCollection> func;
				if ((func = Arm64Arch.<>O.<0>__CreateKnownMethodThunks) == null)
				{
					func = (Arm64Arch.<>O.<0>__CreateKnownMethodThunks = new Func<BytePatternCollection>(Arm64Arch.CreateKnownMethodThunks));
				}
				return Helpers.GetOrInit<BytePatternCollection>(ref this.lazyKnownMethodThunks, func);
			}
		}

		public IAltEntryFactory AltEntryFactory
		{
			get
			{
				return null;
			}
		}

		public Arm64Arch(ISystem system)
		{
			this.System = system;
		}

		public NativeDetourInfo ComputeDetourInfo(IntPtr from, IntPtr target, int maxSizeHint)
		{
			x86Shared.FixSizeHint(ref maxSizeHint);
			if (maxSizeHint < Arm64Arch.BranchRegisterKind.Instance.Size)
			{
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(88, 1, out flag);
				if (flag)
				{
					debugLogWarningStringHandler.AppendLiteral("Size too small for all known detour kinds! Defaulting to BranchRegister. provided size: ");
					debugLogWarningStringHandler.AppendFormatted<int>(maxSizeHint);
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
			}
			return new NativeDetourInfo(from, target, Arm64Arch.BranchRegisterKind.Instance, null);
		}

		[NullableContext(0)]
		public int GetDetourBytes(NativeDetourInfo info, global::System.Span<byte> buffer, [Nullable(2)] out IDisposable allocationHandle)
		{
			return DetourKindBase.GetDetourBytes(info, buffer, out allocationHandle);
		}

		public NativeDetourInfo ComputeRetargetInfo(NativeDetourInfo detour, IntPtr target, int maxSizeHint = -1)
		{
			x86Shared.FixSizeHint(ref maxSizeHint);
			NativeDetourInfo nativeDetourInfo;
			if (DetourKindBase.TryFindRetargetInfo(detour, target, maxSizeHint, out nativeDetourInfo))
			{
				return nativeDetourInfo;
			}
			return this.ComputeDetourInfo(detour.From, target, maxSizeHint);
		}

		[NullableContext(0)]
		public int GetRetargetBytes(NativeDetourInfo original, NativeDetourInfo retarget, global::System.Span<byte> buffer, [Nullable(2)] out IDisposable allocationHandle, out bool needsRepatch, out bool disposeOldAlloc)
		{
			return DetourKindBase.DoRetarget(original, retarget, buffer, out allocationHandle, out needsRepatch, out disposeOldAlloc);
		}

		[return: Nullable(new byte[] { 0, 1 })]
		public unsafe global::System.ReadOnlyMemory<IAllocatedMemory> CreateNativeVtableProxyStubs(IntPtr vtableBase, int vtableSize)
		{
			global::System.ReadOnlySpan<byte> readOnlySpan = new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.F3802BB374CB7730856DFA9B2994568F5F20D78A6D085EE83B6B63FE286F83C7), 28);
			return Shared.CreateVtableStubs(this.System, vtableBase, vtableSize, readOnlySpan, 24, true);
		}

		public unsafe IAllocatedMemory CreateSpecialEntryStub(IntPtr target, IntPtr argument)
		{
			global::System.ReadOnlySpan<byte> readOnlySpan = new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.6C0B441759C39B924045B27B9BC8490EBEAD0D2E3FB37A7E54D7585CE448D12E), 32);
			int length = readOnlySpan.Length;
			global::System.Span<byte> span = new global::System.Span<byte>(stackalloc byte[(UIntPtr)length], length);
			readOnlySpan.CopyTo(span);
			Unsafe.WriteUnaligned<IntPtr>(span[16], argument);
			Unsafe.WriteUnaligned<IntPtr>(span[24], target);
			return Shared.CreateSingleExecutableStub(this.System, span);
		}

		private unsafe static BytePatternCollection CreateKnownMethodThunks()
		{
			RuntimeKind runtime = PlatformDetection.Runtime;
			bool flag = runtime - RuntimeKind.Framework <= 1;
			if (flag)
			{
				List<BytePattern> list = new List<BytePattern>
				{
					new BytePattern(new AddressMeaning(AddressKind.Abs64), true, new byte[]
					{
						byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
						byte.MaxValue, byte.MaxValue, 0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0
					}, new byte[]
					{
						137, 0, 0, 16, 42, 49, 64, 169, 64, 1,
						31, 214, 0, 0, 0, 0, 2, 2, 2, 2,
						2, 2, 2, 2
					}),
					new BytePattern(new AddressMeaning(AddressKind.Abs64), true, new byte[]
					{
						byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
						byte.MaxValue, byte.MaxValue, 0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0
					}, new byte[]
					{
						139, 0, 0, 16, 106, 49, 64, 169, 64, 1,
						31, 214, 0, 0, 0, 0, 2, 2, 2, 2,
						2, 2, 2, 2
					}),
					new BytePattern(new AddressMeaning(AddressKind.Abs64), true, new byte[]
					{
						byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
						byte.MaxValue, byte.MaxValue, 0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0
					}, new byte[]
					{
						12, 0, 0, 16, 107, 0, 0, 88, 96, 1,
						31, 214, 0, 0, 0, 0, 2, 2, 2, 2,
						2, 2, 2, 2
					}),
					new BytePattern(new AddressMeaning(AddressKind.Abs64), true, new byte[]
					{
						byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
						byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
						0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
						0, 0
					}, new byte[]
					{
						16, 0, 0, 145, 32, 0, 0, 145, 1, 2,
						0, 145, 112, 0, 0, 88, 0, 2, 31, 214,
						0, 0, 0, 0, 2, 2, 2, 2, 2, 2,
						2, 2
					})
				};
				int[] array;
				if ((array = <24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.85AD7009527B6DE3AC5F5E12927AD6128ABCC8515CCE30BCB0255EBFDEEECE0B_A6) == null)
				{
					array = (<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.85AD7009527B6DE3AC5F5E12927AD6128ABCC8515CCE30BCB0255EBFDEEECE0B_A6 = new int[] { 4096, 8192, 16384, 32768, 65536 });
				}
				global::System.ReadOnlySpan<int> readOnlySpan = new global::System.ReadOnlySpan<int>(array);
				global::System.ReadOnlySpan<byte> readOnlySpan2 = new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.6429D8F0DD24CBCF7B3A2285E61F8070D49FF70D9E25C10C0FC23F7B01DA5111), 20);
				global::System.ReadOnlySpan<byte> readOnlySpan3 = new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.FC7B60795B80EF58D2D4162DEE34AF892D9A11536D4150FA5C0B6BA59698AEDD), 24);
				global::System.ReadOnlySpan<byte> readOnlySpan4 = new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.3D23FF3F668395EBA947A1A486FB6291E544D53C9F48661A3B9C2B51F481CEF7), 36);
				global::System.ReadOnlyMemory<byte> readOnlyMemory = new byte[]
				{
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue
				};
				global::System.ReadOnlySpan<int> readOnlySpan5 = readOnlySpan;
				for (int i = 0; i < readOnlySpan5.Length; i++)
				{
					int num = *readOnlySpan5[i];
					byte[] array2 = readOnlySpan2.ToArray();
					Arm64Arch.EncodeLdr64LiteralTo(array2.AsSpan<byte>(0), num, 11);
					Arm64Arch.EncodeLdr64LiteralTo(array2.AsSpan<byte>(8), -8 + num + 8, 12);
					Arm64Arch.EncodeLdr64LiteralTo(array2.AsSpan<byte>(12), -12 + num + 16, 11);
					list.Add(new BytePattern(new AddressMeaning(AddressKind.Rel64 | AddressKind.Indirect | AddressKind.ConstantAddr, 0, (ulong)num), true, readOnlyMemory.Slice(0, array2.Length), array2));
					list.Add(new BytePattern(new AddressMeaning(AddressKind.Rel64 | AddressKind.PrecodeFixupThunkRel32 | AddressKind.Indirect | AddressKind.ConstantAddr, 0, (ulong)(num + 16 - 8)), true, readOnlyMemory.Slice(0, array2.Length - 8), array2.AsMemory<byte>(8)));
					byte[] array3 = readOnlySpan3.ToArray();
					Arm64Arch.EncodeLdr64LiteralTo(array3.AsSpan<byte>(0), num, 11);
					Arm64Arch.EncodeLdr64LiteralTo(array3.AsSpan<byte>(12), -12 + num + 8, 12);
					Arm64Arch.EncodeLdr64LiteralTo(array3.AsSpan<byte>(16), -16 + num + 16, 11);
					list.Add(new BytePattern(new AddressMeaning(AddressKind.Rel64 | AddressKind.Indirect | AddressKind.ConstantAddr, 0, (ulong)num), true, readOnlyMemory.Slice(0, array3.Length), array3));
					list.Add(new BytePattern(new AddressMeaning(AddressKind.Rel64 | AddressKind.PrecodeFixupThunkRel32 | AddressKind.Indirect | AddressKind.ConstantAddr, 0, (ulong)(num + 16 - 8)), true, readOnlyMemory.Slice(0, array3.Length - 8), array3.AsMemory<byte>(8)));
					byte[] array4 = readOnlySpan4.ToArray();
					Arm64Arch.EncodeLdr64LiteralTo(array4.AsSpan<byte>(0), num, 9);
					Arm64Arch.EncodeLdr64LiteralTo(array4.AsSpan<byte>(20), -20 + num + 8, 9);
					Arm64Arch.EncodeLdr64LiteralTo(array4.AsSpan<byte>(28), -28 + num + 16, 9);
					list.Add(new BytePattern(new AddressMeaning(AddressKind.Rel64 | AddressKind.Indirect | AddressKind.ConstantAddr, 0, (ulong)(num + 8)), true, readOnlyMemory.Slice(0, array4.Length), array4));
				}
				return new BytePatternCollection(list.ToArray());
			}
			return new BytePatternCollection(new BytePattern[0]);
		}

		[NullableContext(0)]
		private static void EncodeLdr64LiteralTo(global::System.Span<byte> dest, int offset, byte reg)
		{
			uint num = (uint)offset >> 2;
			num &= 524287U;
			uint num2 = 1476395008U;
			num2 |= num << 5;
			num2 |= (uint)reg;
			global::System.Runtime.InteropServices.MemoryMarshal.Write<uint>(dest, ref num2);
		}

		[Nullable(2)]
		private BytePatternCollection lazyKnownMethodThunks;

		private readonly ISystem System;

		[NullableContext(2)]
		[Nullable(0)]
		private sealed class BranchRegisterKind : DetourKindBase
		{
			public override int Size
			{
				get
				{
					return 16;
				}
			}

			public unsafe override int GetBytes(IntPtr from, IntPtr to, [Nullable(0)] global::System.Span<byte> buffer, object data, out IDisposable allocHandle)
			{
				global::System.ReadOnlySpan<byte> readOnlySpan = new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.464FCBB1223EB3625C4B5CE8ED1C096B7322AFC2C52C73CE667DAB0F609B3B17), 16);
				readOnlySpan.CopyTo(buffer);
				Unsafe.WriteUnaligned<ulong>(buffer[8], (ulong)(long)to);
				allocHandle = null;
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(29, 2, out flag);
				if (flag)
				{
					debugLogTraceStringHandler.AppendLiteral("Detouring arm64 from 0x");
					debugLogTraceStringHandler.AppendFormatted<IntPtr>(from, "X16");
					debugLogTraceStringHandler.AppendLiteral(" to 0x");
					debugLogTraceStringHandler.AppendFormatted<IntPtr>(to, "X16");
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
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
			public static readonly Arm64Arch.BranchRegisterKind Instance = new Arm64Arch.BranchRegisterKind();
		}

		[CompilerGenerated]
		private static class <>O
		{
			[Nullable(new byte[] { 0, 1 })]
			public static Func<BytePatternCollection> <0>__CreateKnownMethodThunks;
		}
	}
}
