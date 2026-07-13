using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using MonoMod.Core.Interop;
using MonoMod.Core.Platforms.Memory;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Systems
{
	internal sealed class WindowsSystem : ISystem, IControlFlowGuard
	{
		public OSKind Target
		{
			get
			{
				return OSKind.Windows;
			}
		}

		public SystemFeature Features
		{
			get
			{
				return SystemFeature.RWXPages;
			}
		}

		[Nullable(2)]
		public INativeExceptionHelper NativeExceptionHelper
		{
			[NullableContext(2)]
			get
			{
				return null;
			}
		}

		public Abi? DefaultAbi { get; }

		[NullableContext(1)]
		private static TypeClassification ClassifyX64(Type type, bool isReturn)
		{
			int managedSize = type.GetManagedSize();
			bool flag = managedSize - 1 <= 1 || managedSize == 4 || managedSize == 8;
			if (flag)
			{
				return TypeClassification.InRegister;
			}
			return TypeClassification.ByReference;
		}

		[NullableContext(1)]
		private static TypeClassification ClassifyX86(Type type, bool isReturn)
		{
			if (!isReturn)
			{
				return TypeClassification.OnStack;
			}
			int managedSize = type.GetManagedSize();
			bool flag = managedSize - 1 <= 1 || managedSize == 4;
			if (flag)
			{
				return TypeClassification.InRegister;
			}
			return TypeClassification.ByReference;
		}

		public WindowsSystem()
		{
			if (PlatformDetection.Architecture == ArchitectureKind.x86_64)
			{
				global::System.ReadOnlyMemory<SpecialArgumentKind> readOnlyMemory = new SpecialArgumentKind[]
				{
					SpecialArgumentKind.ReturnBuffer,
					SpecialArgumentKind.ThisPointer,
					SpecialArgumentKind.UserArguments
				};
				Classifier classifier;
				if ((classifier = WindowsSystem.<>O.<0>__ClassifyX64) == null)
				{
					classifier = (WindowsSystem.<>O.<0>__ClassifyX64 = new Classifier(WindowsSystem.ClassifyX64));
				}
				this.DefaultAbi = new Abi?(new Abi(readOnlyMemory, classifier, true));
				return;
			}
			if (PlatformDetection.Architecture == ArchitectureKind.x86)
			{
				global::System.ReadOnlyMemory<SpecialArgumentKind> readOnlyMemory2 = new SpecialArgumentKind[]
				{
					SpecialArgumentKind.ThisPointer,
					SpecialArgumentKind.ReturnBuffer,
					SpecialArgumentKind.UserArguments
				};
				Classifier classifier2;
				if ((classifier2 = WindowsSystem.<>O.<1>__ClassifyX86) == null)
				{
					classifier2 = (WindowsSystem.<>O.<1>__ClassifyX86 = new Classifier(WindowsSystem.ClassifyX86));
				}
				this.DefaultAbi = new Abi?(new Abi(readOnlyMemory2, classifier2, true));
			}
		}

		public unsafe void PatchData(PatchTargetKind patchKind, IntPtr patchTarget, global::System.ReadOnlySpan<byte> data, global::System.Span<byte> backup)
		{
			if (patchKind == PatchTargetKind.Executable)
			{
				WindowsSystem.ProtectRWX(patchTarget, (UIntPtr)((IntPtr)data.Length));
			}
			else
			{
				WindowsSystem.ProtectRW(patchTarget, (UIntPtr)((IntPtr)data.Length));
			}
			global::System.Span<byte> span = new global::System.Span<byte>((void*)patchTarget, data.Length);
			span.TryCopyTo(backup);
			data.CopyTo(span);
			if (patchKind == PatchTargetKind.Executable)
			{
				WindowsSystem.FlushInstructionCache(patchTarget, (UIntPtr)((IntPtr)data.Length));
			}
		}

		private unsafe static void ProtectRW(IntPtr addr, [NativeInteger] UIntPtr size)
		{
			uint num;
			if (!Windows.VirtualProtect((void*)addr, size, 4U, &num))
			{
				throw WindowsSystem.LogAllSections(Windows.GetLastError(), addr, size, "ProtectRW");
			}
		}

		private unsafe static void ProtectRWX(IntPtr addr, [NativeInteger] UIntPtr size)
		{
			uint num;
			if (!Windows.VirtualProtect((void*)addr, size, 64U, &num))
			{
				throw WindowsSystem.LogAllSections(Windows.GetLastError(), addr, size, "ProtectRWX");
			}
		}

		private unsafe static void FlushInstructionCache(IntPtr addr, [NativeInteger] UIntPtr size)
		{
			if (!Windows.FlushInstructionCache(Windows.GetCurrentProcess(), (void*)addr, size))
			{
				throw WindowsSystem.LogAllSections(Windows.GetLastError(), addr, size, "FlushInstructionCache");
			}
		}

		[return: Nullable(new byte[] { 1, 2 })]
		public IEnumerable<string> EnumerateLoadedModuleFiles()
		{
			return from ProcessModule m in Process.GetCurrentProcess().Modules
				select m.FileName;
		}

		[return: NativeInteger]
		public unsafe IntPtr GetSizeOfReadableMemory([NativeInteger] IntPtr start, [NativeInteger] IntPtr guess)
		{
			IntPtr intPtr = (IntPtr)0;
			Windows.MEMORY_BASIC_INFORMATION memory_BASIC_INFORMATION;
			bool flag;
			while (Windows.VirtualQuery(start, &memory_BASIC_INFORMATION, (UIntPtr)((IntPtr)sizeof(Windows.MEMORY_BASIC_INFORMATION))) != 0)
			{
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogSpamStringHandler debugLogSpamStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogSpamStringHandler(56, 4, out flag);
				if (flag)
				{
					debugLogSpamStringHandler.AppendLiteral("VirtualQuery(0x");
					debugLogSpamStringHandler.AppendFormatted<IntPtr>(start, "x16");
					debugLogSpamStringHandler.AppendLiteral(") == { Protect = ");
					debugLogSpamStringHandler.AppendFormatted<uint>(memory_BASIC_INFORMATION.Protect, "x");
					debugLogSpamStringHandler.AppendLiteral(", BaseAddr = ");
					debugLogSpamStringHandler.AppendFormatted<UIntPtr>(memory_BASIC_INFORMATION.BaseAddress, "x16");
					debugLogSpamStringHandler.AppendLiteral(", Size = ");
					debugLogSpamStringHandler.AppendFormatted<UIntPtr>(memory_BASIC_INFORMATION.RegionSize, "x4");
					debugLogSpamStringHandler.AppendLiteral(" }");
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Spam(ref debugLogSpamStringHandler);
				if ((memory_BASIC_INFORMATION.Protect & 102U) <= 0U)
				{
					return intPtr;
				}
				IntPtr intPtr2 = (byte*)memory_BASIC_INFORMATION.BaseAddress + memory_BASIC_INFORMATION.RegionSize;
				intPtr += intPtr2 - start;
				start = intPtr2;
				if (intPtr >= guess)
				{
					return intPtr;
				}
			}
			uint lastError = Windows.GetLastError();
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(22, 2, out flag);
			if (flag)
			{
				debugLogWarningStringHandler.AppendLiteral("VirtualQuery failed: ");
				debugLogWarningStringHandler.AppendFormatted<uint>(lastError);
				debugLogWarningStringHandler.AppendLiteral(" ");
				debugLogWarningStringHandler.AppendFormatted(new Win32Exception((int)lastError).Message);
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
			return (IntPtr)0;
		}

		[NullableContext(1)]
		private unsafe static Exception LogAllSections(uint error, IntPtr src, [NativeInteger] UIntPtr size, [CallerMemberName] string from = "")
		{
			Exception ex = new Win32Exception((int)error);
			if (!<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.IsWritingLog)
			{
				return ex;
			}
			bool flag;
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(47, 3, out flag);
			if (flag)
			{
				debugLogErrorStringHandler.AppendFormatted(from);
				debugLogErrorStringHandler.AppendLiteral(" failed for 0x");
				debugLogErrorStringHandler.AppendFormatted<IntPtr>(src, "X16");
				debugLogErrorStringHandler.AppendLiteral(" + ");
				debugLogErrorStringHandler.AppendFormatted<UIntPtr>(size);
				debugLogErrorStringHandler.AppendLiteral(" - logging all memory sections");
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler2 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(8, 1, out flag);
			if (flag)
			{
				debugLogErrorStringHandler2.AppendLiteral("reason: ");
				debugLogErrorStringHandler2.AppendFormatted(ex.Message);
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler2);
			try
			{
				IntPtr intPtr = (IntPtr)65536;
				int num = 0;
				Windows.MEMORY_BASIC_INFORMATION memory_BASIC_INFORMATION;
				while (Windows.VirtualQuery((void*)intPtr, &memory_BASIC_INFORMATION, (UIntPtr)((IntPtr)sizeof(Windows.MEMORY_BASIC_INFORMATION))) != 0)
				{
					UIntPtr uintPtr = (UIntPtr)(src + (IntPtr)size);
					void* baseAddress = memory_BASIC_INFORMATION.BaseAddress;
					UIntPtr uintPtr2 = (byte*)baseAddress + memory_BASIC_INFORMATION.RegionSize;
					bool flag2 = baseAddress == uintPtr && src <= (IntPtr)uintPtr2;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(2, 2, out flag);
					if (flag)
					{
						debugLogTraceStringHandler.AppendFormatted(flag2 ? "*" : "-");
						debugLogTraceStringHandler.AppendLiteral(" #");
						debugLogTraceStringHandler.AppendFormatted<int>(num++);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler2 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(8, 1, out flag);
					if (flag)
					{
						debugLogTraceStringHandler2.AppendLiteral("addr: 0x");
						debugLogTraceStringHandler2.AppendFormatted<UIntPtr>(memory_BASIC_INFORMATION.BaseAddress, "X16");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler2);
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler3 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(8, 1, out flag);
					if (flag)
					{
						debugLogTraceStringHandler3.AppendLiteral("size: 0x");
						debugLogTraceStringHandler3.AppendFormatted<UIntPtr>(memory_BASIC_INFORMATION.RegionSize, "X16");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler3);
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler4 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(9, 1, out flag);
					if (flag)
					{
						debugLogTraceStringHandler4.AppendLiteral("aaddr: 0x");
						debugLogTraceStringHandler4.AppendFormatted<UIntPtr>(memory_BASIC_INFORMATION.AllocationBase, "X16");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler4);
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler5 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(7, 1, out flag);
					if (flag)
					{
						debugLogTraceStringHandler5.AppendLiteral("state: ");
						debugLogTraceStringHandler5.AppendFormatted<uint>(memory_BASIC_INFORMATION.State);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler5);
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler6 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(6, 1, out flag);
					if (flag)
					{
						debugLogTraceStringHandler6.AppendLiteral("type: ");
						debugLogTraceStringHandler6.AppendFormatted<uint>(memory_BASIC_INFORMATION.Type);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler6);
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler7 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(9, 1, out flag);
					if (flag)
					{
						debugLogTraceStringHandler7.AppendLiteral("protect: ");
						debugLogTraceStringHandler7.AppendFormatted<uint>(memory_BASIC_INFORMATION.Protect);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler7);
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler8 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(10, 1, out flag);
					if (flag)
					{
						debugLogTraceStringHandler8.AppendLiteral("aprotect: ");
						debugLogTraceStringHandler8.AppendFormatted<uint>(memory_BASIC_INFORMATION.AllocationProtect);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler8);
					try
					{
						IntPtr intPtr2 = intPtr;
						intPtr = (IntPtr)((byte*)memory_BASIC_INFORMATION.BaseAddress + (ulong)memory_BASIC_INFORMATION.RegionSize);
						if ((long)intPtr > (long)intPtr2)
						{
							continue;
						}
					}
					catch (OverflowException ex2)
					{
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler3 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(9, 1, out flag);
						if (flag)
						{
							debugLogErrorStringHandler3.AppendLiteral("overflow ");
							debugLogErrorStringHandler3.AppendFormatted<OverflowException>(ex2);
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler3);
					}
					break;
				}
			}
			catch
			{
				throw ex;
			}
			return ex;
		}

		[Nullable(1)]
		public IMemoryAllocator MemoryAllocator
		{
			[NullableContext(1)]
			get;
		} = new QueryingPagedMemoryAllocator(new WindowsSystem.PageAllocator());

		bool IControlFlowGuard.IsSupported
		{
			get
			{
				return Windows.HasSetProcessValidCallTargets;
			}
		}

		int IControlFlowGuard.TargetAlignmentRequirement
		{
			get
			{
				return 16;
			}
		}

		unsafe void IControlFlowGuard.RegisterValidIndirectCallTargets(void* memoryRegionStart, [NativeInteger] IntPtr memoryRegionLength, [NativeInteger] global::System.ReadOnlySpan<IntPtr> validTargetsInMemoryRegion)
		{
			Windows.CFG_CALL_TARGET_INFO[] array = global::System.Buffers.ArrayPool<Windows.CFG_CALL_TARGET_INFO>.Shared.Rent(validTargetsInMemoryRegion.Length);
			for (int i = 0; i < validTargetsInMemoryRegion.Length; i++)
			{
				IntPtr intPtr = *validTargetsInMemoryRegion[i];
				array[i] = new Windows.CFG_CALL_TARGET_INFO
				{
					Offset = (UIntPtr)intPtr,
					Flags = (UIntPtr)((IntPtr)9)
				};
			}
			Windows.CFG_CALL_TARGET_INFO[] array2;
			Windows.CFG_CALL_TARGET_INFO* ptr;
			if ((array2 = array) == null || array2.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array2[0];
			}
			Windows.TrySetProcessValidCallTargets(memoryRegionStart, (UIntPtr)memoryRegionLength, (uint)validTargetsInMemoryRegion.Length, ptr);
			array2 = null;
			global::System.Buffers.ArrayPool<Windows.CFG_CALL_TARGET_INFO>.Shared.Return(array, false);
		}

		public IntPtr GetNativeJitHookConfig(int runtimeMajMin)
		{
			throw new NotImplementedException();
		}

		private sealed class PageAllocator : QueryingMemoryPageAllocatorBase
		{
			public override uint PageSize { get; }

			public unsafe PageAllocator()
			{
				Windows.SYSTEM_INFO system_INFO;
				Windows.GetSystemInfo(&system_INFO);
				this.PageSize = system_INFO.dwAllocationGranularity;
			}

			public override bool TryAllocatePage([NativeInteger] IntPtr size, bool executable, out IntPtr allocated)
			{
				int num = (executable ? 64 : 4);
				allocated = (IntPtr)Windows.VirtualAlloc(null, (UIntPtr)size, 12288U, (uint)num);
				return allocated != IntPtr.Zero;
			}

			public unsafe override bool TryAllocatePage(IntPtr pageAddr, [NativeInteger] IntPtr size, bool executable, out IntPtr allocated)
			{
				int num = (executable ? 64 : 4);
				allocated = (IntPtr)Windows.VirtualAlloc((void*)pageAddr, (UIntPtr)size, 12288U, (uint)num);
				return allocated != IntPtr.Zero;
			}

			[NullableContext(2)]
			public unsafe override bool TryFreePage(IntPtr pageAddr, [<24b3ba8a-00b7-40fc-a603-2711fa115297>NotNullWhen(false)] out string errorMsg)
			{
				if (!Windows.VirtualFree((void*)pageAddr, (UIntPtr)((IntPtr)0), 32768U))
				{
					errorMsg = new Win32Exception((int)Windows.GetLastError()).Message;
					return false;
				}
				errorMsg = null;
				return true;
			}

			public unsafe override bool TryQueryPage(IntPtr pageAddr, out bool isFree, out IntPtr allocBase, [NativeInteger] out IntPtr allocSize)
			{
				Windows.MEMORY_BASIC_INFORMATION memory_BASIC_INFORMATION;
				if (Windows.VirtualQuery((void*)pageAddr, &memory_BASIC_INFORMATION, (UIntPtr)((IntPtr)sizeof(Windows.MEMORY_BASIC_INFORMATION))) != 0)
				{
					isFree = memory_BASIC_INFORMATION.State == 65536U;
					allocBase = (isFree ? memory_BASIC_INFORMATION.BaseAddress : memory_BASIC_INFORMATION.AllocationBase);
					allocSize = pageAddr + (IntPtr)memory_BASIC_INFORMATION.RegionSize - allocBase;
					return true;
				}
				isFree = false;
				allocBase = IntPtr.Zero;
				allocSize = (IntPtr)0;
				return false;
			}
		}

		[CompilerGenerated]
		private static class <>O
		{
			public static Classifier <0>__ClassifyX64;

			public static Classifier <1>__ClassifyX86;
		}
	}
}
