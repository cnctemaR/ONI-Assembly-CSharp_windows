using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Architectures
{
	internal static class Shared
	{
		[NullableContext(1)]
		public unsafe static IAllocatedMemory CreateSingleExecutableStub(ISystem system, [Nullable(0)] global::System.ReadOnlySpan<byte> stubBytes)
		{
			IControlFlowGuard controlFlowGuard = system as IControlFlowGuard;
			if (controlFlowGuard != null && !controlFlowGuard.IsSupported)
			{
				controlFlowGuard = null;
			}
			IAllocatedMemory allocatedMemory;
			Helpers.Assert(system.MemoryAllocator.TryAllocate(new AllocationRequest(stubBytes.Length)
			{
				Executable = true,
				Alignment = ((controlFlowGuard != null) ? controlFlowGuard.TargetAlignmentRequirement : 1)
			}, out allocatedMemory), null, "system.MemoryAllocator.TryAllocate(new(stubBytes.Length)\n            {\n                Executable = true,\n                Alignment = cfg is not null ? cfg.TargetAlignmentRequirement : 1, // if CFG is supported, use that alignment\n            }, out var alloc)");
			system.PatchData(PatchTargetKind.Executable, allocatedMemory.BaseAddress, stubBytes, default(global::System.Span<byte>));
			if (controlFlowGuard != null)
			{
				IControlFlowGuard controlFlowGuard2 = controlFlowGuard;
				void* ptr = (void*)allocatedMemory.BaseAddress;
				IntPtr intPtr = (IntPtr)allocatedMemory.Size;
				IntPtr[] array;
				if ((array = <24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.DF3F619804A92FDB4057192DC43DD748EA778ADC52BC498CE80524C014B81119_B8) == null)
				{
					array = (<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.DF3F619804A92FDB4057192DC43DD748EA778ADC52BC498CE80524C014B81119_B8 = new IntPtr[1]);
				}
				controlFlowGuard2.RegisterValidIndirectCallTargets(ptr, intPtr, new global::System.ReadOnlySpan<IntPtr>(array));
			}
			return allocatedMemory;
		}

		[return: Nullable(new byte[] { 0, 1 })]
		public unsafe static global::System.ReadOnlyMemory<IAllocatedMemory> CreateVtableStubs([Nullable(1)] ISystem system, IntPtr vtableBase, int vtableSize, global::System.ReadOnlySpan<byte> stubData, int indexOffs, bool premulOffset)
		{
			IControlFlowGuard controlFlowGuard = system as IControlFlowGuard;
			if (controlFlowGuard != null && !controlFlowGuard.IsSupported)
			{
				controlFlowGuard = null;
			}
			int num = stubData.Length;
			if (controlFlowGuard != null)
			{
				int targetAlignmentRequirement = controlFlowGuard.TargetAlignmentRequirement;
				num = ((num - 1) / targetAlignmentRequirement + 1) * targetAlignmentRequirement;
			}
			int maxSize = system.MemoryAllocator.MaxSize;
			int num2 = num * vtableSize;
			int num3 = num2 / maxSize;
			int num4 = maxSize / num;
			int num5 = num4 * num;
			int num6 = num2 % num5;
			IAllocatedMemory[] array = new IAllocatedMemory[num3 + ((num6 != 0) ? 1 : 0)];
			byte[] array2 = global::System.Buffers.ArrayPool<byte>.Shared.Rent(num5);
			IntPtr[] array3 = global::System.Buffers.ArrayPool<IntPtr>.Shared.Rent(num4);
			global::System.Span<byte> span = array2.AsSpan<byte>().Slice(0, num5);
			for (int i = 0; i < num4; i++)
			{
				stubData.CopyTo(span.Slice(i * num));
			}
			ref IntPtr ptr = ref Unsafe.AsRef<IntPtr>((void*)vtableBase);
			AllocationRequest allocationRequest = new AllocationRequest(num5)
			{
				Alignment = ((controlFlowGuard != null) ? controlFlowGuard.TargetAlignmentRequirement : IntPtr.Size),
				Executable = true
			};
			AllocationRequest allocationRequest2 = allocationRequest;
			for (int j = 0; j < num3; j++)
			{
				IAllocatedMemory allocatedMemory;
				Helpers.Assert(system.MemoryAllocator.TryAllocate(allocationRequest2, out allocatedMemory), null, "system.MemoryAllocator.TryAllocate(allocReq, out var alloc)");
				array[j] = allocatedMemory;
				Shared.<CreateVtableStubs>g__FillBufferIndicies|1_0(num, indexOffs, num4, j, span, premulOffset);
				Shared.<CreateVtableStubs>g__FillVtbl|1_1(num, num4 * j, ref ptr, num4, allocatedMemory.BaseAddress, array3);
				system.PatchData(PatchTargetKind.Executable, allocatedMemory.BaseAddress, span, default(global::System.Span<byte>));
				if (controlFlowGuard != null)
				{
					controlFlowGuard.RegisterValidIndirectCallTargets((void*)allocatedMemory.BaseAddress, (IntPtr)allocatedMemory.Size, array3.AsSpan<IntPtr>(0, num4));
				}
			}
			if (num6 > 0)
			{
				allocationRequest = allocationRequest2;
				allocationRequest.Size = num6;
				allocationRequest2 = allocationRequest;
				IAllocatedMemory allocatedMemory2;
				Helpers.Assert(system.MemoryAllocator.TryAllocate(allocationRequest2, out allocatedMemory2), null, "system.MemoryAllocator.TryAllocate(allocReq, out var alloc)");
				array[array.Length - 1] = allocatedMemory2;
				int num7 = num6 / num;
				Shared.<CreateVtableStubs>g__FillBufferIndicies|1_0(num, indexOffs, num4, num3, span, premulOffset);
				Shared.<CreateVtableStubs>g__FillVtbl|1_1(num, num4 * num3, ref ptr, num7, allocatedMemory2.BaseAddress, array3);
				system.PatchData(PatchTargetKind.Executable, allocatedMemory2.BaseAddress, span.Slice(0, num6), default(global::System.Span<byte>));
				if (controlFlowGuard != null)
				{
					controlFlowGuard.RegisterValidIndirectCallTargets((void*)allocatedMemory2.BaseAddress, (IntPtr)allocatedMemory2.Size, array3.AsSpan<IntPtr>(0, num7));
				}
			}
			global::System.Buffers.ArrayPool<IntPtr>.Shared.Return(array3, false);
			global::System.Buffers.ArrayPool<byte>.Shared.Return(array2, false);
			return array;
		}

		[CompilerGenerated]
		internal static void <CreateVtableStubs>g__FillBufferIndicies|1_0(int stubSize, int indexOffs, int numPerAlloc, int i, global::System.Span<byte> mainAllocBuf, bool premul)
		{
			for (int j = 0; j < numPerAlloc; j++)
			{
				ref byte ptr = ref mainAllocBuf[j * stubSize + indexOffs];
				uint num = (uint)(numPerAlloc * i + j);
				if (premul)
				{
					num *= (uint)IntPtr.Size;
				}
				Unsafe.WriteUnaligned<uint>(ref ptr, num);
			}
		}

		[NullableContext(1)]
		[CompilerGenerated]
		internal unsafe static void <CreateVtableStubs>g__FillVtbl|1_1(int stubSize, int baseIndex, ref IntPtr vtblBase, int numEntries, [NativeInteger] IntPtr baseAddr, [NativeInteger] IntPtr[] offsets)
		{
			for (int i = 0; i < numEntries; i++)
			{
				IntPtr intPtr = (offsets[i] = (IntPtr)(stubSize * i));
				*Unsafe.Add<IntPtr>(ref vtblBase, baseIndex + i) = baseAddr + intPtr;
			}
		}
	}
}
