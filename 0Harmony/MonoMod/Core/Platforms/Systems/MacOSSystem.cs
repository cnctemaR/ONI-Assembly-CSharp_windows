using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Core.Interop;
using MonoMod.Core.Platforms.Memory;
using MonoMod.Core.Utils;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Systems
{
	internal sealed class MacOSSystem : ISystem, IInitialize<IArchitecture>
	{
		public OSKind Target
		{
			get
			{
				return OSKind.OSX;
			}
		}

		public SystemFeature Features { get; }

		public Abi? DefaultAbi { get; }

		public MacOSSystem()
		{
			ArchitectureKind architecture = PlatformDetection.Architecture;
			if (architecture == ArchitectureKind.x86_64)
			{
				this.Features = SystemFeature.RWXPages | SystemFeature.RXPages;
				global::System.ReadOnlyMemory<SpecialArgumentKind> readOnlyMemory = new SpecialArgumentKind[]
				{
					SpecialArgumentKind.ReturnBuffer,
					SpecialArgumentKind.ThisPointer,
					SpecialArgumentKind.UserArguments
				};
				Classifier classifier;
				if ((classifier = MacOSSystem.<>O.<0>__ClassifyAMD64) == null)
				{
					classifier = (MacOSSystem.<>O.<0>__ClassifyAMD64 = new Classifier(SystemVABI.ClassifyAMD64));
				}
				this.DefaultAbi = new Abi?(new Abi(readOnlyMemory, classifier, true));
				return;
			}
			if (architecture != ArchitectureKind.Arm64)
			{
				throw new NotImplementedException();
			}
			this.Features = SystemFeature.RXPages | SystemFeature.MayUseNativeJitHooks;
			global::System.ReadOnlyMemory<SpecialArgumentKind> readOnlyMemory2 = new SpecialArgumentKind[]
			{
				SpecialArgumentKind.ThisPointer,
				SpecialArgumentKind.UserArguments
			};
			Classifier classifier2;
			if ((classifier2 = MacOSSystem.<>O.<1>__ClassifyARM64) == null)
			{
				classifier2 = (MacOSSystem.<>O.<1>__ClassifyARM64 = new Classifier(SystemVABI.ClassifyARM64));
			}
			this.DefaultAbi = new Abi?(new Abi(readOnlyMemory2, classifier2, false));
		}

		[return: Nullable(new byte[] { 1, 2 })]
		public unsafe IEnumerable<string> EnumerateLoadedModuleFiles()
		{
			int count = OSX.task_dyld_info.Count;
			OSX.task_dyld_info task_dyld_info = default(OSX.task_dyld_info);
			if (!OSX.task_info(OSX.mach_task_self(), OSX.task_flavor_t.DyldInfo, &task_dyld_info, &count))
			{
				return ArrayEx.Empty<string>();
			}
			global::System.ReadOnlySpan<OSX.dyld_image_info> infoArray = task_dyld_info.all_image_infos->InfoArray;
			string[] array = new string[infoArray.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = infoArray[i].imageFilePath.ToString();
			}
			return array;
		}

		[return: NativeInteger]
		public IntPtr GetSizeOfReadableMemory(IntPtr start, [NativeInteger] IntPtr guess)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2;
			IntPtr intPtr3;
			OSX.vm_prot_t vm_prot_t;
			OSX.vm_prot_t vm_prot_t2;
			while (MacOSSystem.GetLocalRegionInfo(start, out intPtr2, out intPtr3, out vm_prot_t, out vm_prot_t2))
			{
				if (intPtr2 > start)
				{
					return intPtr;
				}
				if ((vm_prot_t & OSX.vm_prot_t.Read) <= OSX.vm_prot_t.None)
				{
					return intPtr;
				}
				intPtr += intPtr2 + intPtr3 - start;
				start = intPtr2 + intPtr3;
				if (intPtr >= guess)
				{
					return intPtr;
				}
			}
			return intPtr;
		}

		public unsafe void PatchData(PatchTargetKind targetKind, IntPtr patchTarget, global::System.ReadOnlySpan<byte> data, global::System.Span<byte> backup)
		{
			int length = data.Length;
			OSX.vm_prot_t vm_prot_t;
			OSX.vm_prot_t vm_prot_t2;
			bool flag;
			bool flag2;
			bool flag4;
			bool flag5;
			if (MacOSSystem.TryGetProtForMem(patchTarget, length, out vm_prot_t, out vm_prot_t2, out flag, out flag2))
			{
				if (flag)
				{
					bool flag3;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(101, 2, out flag3);
					if (flag3)
					{
						debugLogWarningStringHandler.AppendLiteral("Patch requested for memory which spans multiple memory allocations. Failures may result. (0x");
						debugLogWarningStringHandler.AppendFormatted<IntPtr>(patchTarget, "x16");
						debugLogWarningStringHandler.AppendLiteral(" length ");
						debugLogWarningStringHandler.AppendFormatted<int>(length);
						debugLogWarningStringHandler.AppendLiteral(")");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
				}
				flag4 = vm_prot_t2.Has(OSX.vm_prot_t.Write);
				flag5 = vm_prot_t2.Has(OSX.vm_prot_t.Execute);
			}
			else
			{
				if (flag2)
				{
					bool flag3;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(68, 2, out flag3);
					if (flag3)
					{
						debugLogErrorStringHandler.AppendLiteral("Requested patch of region which was not fully allocated (0x");
						debugLogErrorStringHandler.AppendFormatted<IntPtr>(patchTarget, "x16");
						debugLogErrorStringHandler.AppendLiteral(" length ");
						debugLogErrorStringHandler.AppendFormatted<int>(length);
						debugLogErrorStringHandler.AppendLiteral(")");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
					throw new InvalidOperationException("Cannot patch unallocated region");
				}
				flag4 = false;
				flag5 = targetKind == PatchTargetKind.Executable;
			}
			if (!flag4)
			{
				Helpers.Assert(!flag, null, "!crossesBoundary");
				MacOSSystem.MakePageWritable(patchTarget);
			}
			global::System.Span<byte> span = new global::System.Span<byte>((void*)patchTarget, data.Length);
			span.TryCopyTo(backup);
			MacOSSystem.JitMemcpyHelper jitMemcpyHelper = this.NativeExceptionHelper as MacOSSystem.JitMemcpyHelper;
			if (jitMemcpyHelper != null && vm_prot_t2 == OSX.vm_prot_t.All)
			{
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace("RWX memory detected, doing memcpy for MAP_JIT");
				fixed (byte* pinnableReference = data.GetPinnableReference())
				{
					byte* ptr = pinnableReference;
					jitMemcpyHelper.JitMemCpy(patchTarget, (IntPtr)((void*)ptr), (ulong)((long)data.Length));
					bool flag3;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(20, 2, out flag3);
					if (flag3)
					{
						debugLogTraceStringHandler.AppendFormatted<int>(data.Length);
						debugLogTraceStringHandler.AppendLiteral(" bytes written to 0x");
						debugLogTraceStringHandler.AppendFormatted<IntPtr>(patchTarget, "X16");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
				}
			}
			else
			{
				data.CopyTo(span);
			}
			if (flag5)
			{
				OSX.sys_icache_invalidate((void*)patchTarget, (UIntPtr)((IntPtr)data.Length));
			}
		}

		private unsafe static void MakePageWritable([NativeInteger] IntPtr addrInPage)
		{
			IntPtr intPtr;
			IntPtr intPtr2;
			OSX.vm_prot_t vm_prot_t;
			OSX.vm_prot_t vm_prot_t2;
			Helpers.Assert(MacOSSystem.GetLocalRegionInfo(addrInPage, out intPtr, out intPtr2, out vm_prot_t, out vm_prot_t2), null, "GetLocalRegionInfo(addrInPage, out var allocStart, out var allocSize, out var allocProt, out var allocMaxProt)");
			Helpers.Assert(intPtr <= addrInPage, null, "allocStart <= addrInPage");
			if (vm_prot_t.Has(OSX.vm_prot_t.Write))
			{
				return;
			}
			int num = OSX.mach_task_self();
			OSX.kern_return_t kern_return_t;
			bool flag;
			if (vm_prot_t2.Has(OSX.vm_prot_t.Write))
			{
				kern_return_t = OSX.mach_vm_protect(num, (ulong)((long)intPtr), (ulong)((long)intPtr2), false, vm_prot_t | OSX.vm_prot_t.Write);
				if (kern_return_t)
				{
					return;
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(60, 6, out flag);
				if (flag)
				{
					debugLogErrorStringHandler.AppendLiteral("Could not vm_protect page 0x");
					debugLogErrorStringHandler.AppendFormatted<IntPtr>(intPtr, "x16");
					debugLogErrorStringHandler.AppendLiteral("+0x");
					debugLogErrorStringHandler.AppendFormatted<IntPtr>(intPtr2, "x");
					debugLogErrorStringHandler.AppendLiteral(" ");
					debugLogErrorStringHandler.AppendLiteral("from ");
					debugLogErrorStringHandler.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t));
					debugLogErrorStringHandler.AppendLiteral(" to ");
					debugLogErrorStringHandler.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t | OSX.vm_prot_t.Write));
					debugLogErrorStringHandler.AppendLiteral(" (max prot ");
					debugLogErrorStringHandler.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t2));
					debugLogErrorStringHandler.AppendLiteral("): kr = ");
					debugLogErrorStringHandler.AppendFormatted<int>(kern_return_t.Value);
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error("Trying copy/remap instead...");
			}
			if (!vm_prot_t.Has(OSX.vm_prot_t.Read))
			{
				if (!vm_prot_t2.Has(OSX.vm_prot_t.Read))
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler2 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(66, 3, out flag);
					if (flag)
					{
						debugLogErrorStringHandler2.AppendLiteral("Requested 0x");
						debugLogErrorStringHandler2.AppendFormatted<IntPtr>(intPtr, "x16");
						debugLogErrorStringHandler2.AppendLiteral("+0x");
						debugLogErrorStringHandler2.AppendFormatted<IntPtr>(intPtr2, "x");
						debugLogErrorStringHandler2.AppendLiteral(" (max: ");
						debugLogErrorStringHandler2.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t2));
						debugLogErrorStringHandler2.AppendLiteral(") to be made writable, but its not readable!");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler2);
					throw new NotSupportedException("Cannot make page writable because its not readable");
				}
				kern_return_t = OSX.mach_vm_protect(num, (ulong)((long)intPtr), (ulong)((long)intPtr2), false, vm_prot_t | OSX.vm_prot_t.Read);
				if (!kern_return_t)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler3 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(60, 4, out flag);
					if (flag)
					{
						debugLogErrorStringHandler3.AppendLiteral("vm_protect of 0x");
						debugLogErrorStringHandler3.AppendFormatted<IntPtr>(intPtr, "x16");
						debugLogErrorStringHandler3.AppendLiteral("+0x");
						debugLogErrorStringHandler3.AppendFormatted<IntPtr>(intPtr2, "x");
						debugLogErrorStringHandler3.AppendLiteral(" (max: ");
						debugLogErrorStringHandler3.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t2));
						debugLogErrorStringHandler3.AppendLiteral(") to become readable failed: kr = ");
						debugLogErrorStringHandler3.AppendFormatted<int>(kern_return_t.Value);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler3);
					throw new NotSupportedException("Could not make page readable for remap");
				}
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(41, 5, out flag);
			if (flag)
			{
				debugLogTraceStringHandler.AppendLiteral("Performing page remap on 0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(intPtr, "x16");
				debugLogTraceStringHandler.AppendLiteral("+0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(intPtr2, "x");
				debugLogTraceStringHandler.AppendLiteral(" from ");
				debugLogTraceStringHandler.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t));
				debugLogTraceStringHandler.AppendLiteral("/");
				debugLogTraceStringHandler.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t2));
				debugLogTraceStringHandler.AppendLiteral(" to ");
				debugLogTraceStringHandler.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t | OSX.vm_prot_t.Write));
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
			OSX.vm_prot_t vm_prot_t3 = vm_prot_t | OSX.vm_prot_t.Write;
			OSX.vm_prot_t vm_prot_t4 = vm_prot_t2 | OSX.vm_prot_t.Write;
			ulong num2;
			kern_return_t = OSX.mach_vm_map(num, &num2, (ulong)((long)intPtr2), 0UL, OSX.vm_flags.Anywhere, 0, 0UL, true, vm_prot_t3, vm_prot_t4, OSX.vm_inherit_t.Copy);
			if (!kern_return_t)
			{
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler4 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(36, 1, out flag);
				if (flag)
				{
					debugLogErrorStringHandler4.AppendLiteral("Could not allocate new memory! kr = ");
					debugLogErrorStringHandler4.AppendFormatted<int>(kern_return_t.Value);
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler4);
				throw new OutOfMemoryException();
			}
			try
			{
				new global::System.Span<byte>(intPtr, (int)intPtr2).CopyTo(new global::System.Span<byte>(num2, (int)intPtr2));
				ulong num3 = (ulong)((long)intPtr2);
				int num4;
				kern_return_t = OSX.mach_make_memory_entry_64(num, &num3, num2, vm_prot_t4, &num4, 0);
				if (!kern_return_t)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler5 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(79, 4, out flag);
					if (flag)
					{
						debugLogErrorStringHandler5.AppendLiteral("make_memory_entry(task_self(), size: 0x");
						debugLogErrorStringHandler5.AppendFormatted<ulong>(num3, "x");
						debugLogErrorStringHandler5.AppendLiteral(", addr: ");
						debugLogErrorStringHandler5.AppendFormatted<ulong>(num2, "x16");
						debugLogErrorStringHandler5.AppendLiteral(", prot: ");
						debugLogErrorStringHandler5.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t4));
						debugLogErrorStringHandler5.AppendLiteral(", &obj, 0) failed: kr = ");
						debugLogErrorStringHandler5.AppendFormatted<int>(kern_return_t.Value);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler5);
					throw new NotSupportedException("make_memory_entry() failed");
				}
				ulong num5 = (ulong)((long)intPtr);
				kern_return_t = OSX.mach_vm_map(num, &num5, (ulong)((long)intPtr2), 0UL, OSX.vm_flags.Overwrite, num4, 0UL, true, vm_prot_t3, vm_prot_t4, OSX.vm_inherit_t.Copy);
				if (!kern_return_t)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler6 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(78, 10, out flag);
					if (flag)
					{
						debugLogErrorStringHandler6.AppendLiteral("vm_map() failed to map over target range: 0x");
						debugLogErrorStringHandler6.AppendFormatted<ulong>(num5, "x16");
						debugLogErrorStringHandler6.AppendLiteral("+0x");
						debugLogErrorStringHandler6.AppendFormatted<IntPtr>(intPtr2, "x");
						debugLogErrorStringHandler6.AppendLiteral(" (");
						debugLogErrorStringHandler6.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t));
						debugLogErrorStringHandler6.AppendLiteral("/");
						debugLogErrorStringHandler6.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t2));
						debugLogErrorStringHandler6.AppendLiteral(")");
						debugLogErrorStringHandler6.AppendLiteral(" <- (obj ");
						debugLogErrorStringHandler6.AppendFormatted<int>(num4);
						debugLogErrorStringHandler6.AppendLiteral(") 0x");
						debugLogErrorStringHandler6.AppendFormatted<ulong>(num2, "x16");
						debugLogErrorStringHandler6.AppendLiteral("+0x");
						debugLogErrorStringHandler6.AppendFormatted<IntPtr>(intPtr2, "x");
						debugLogErrorStringHandler6.AppendLiteral(" (");
						debugLogErrorStringHandler6.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t3));
						debugLogErrorStringHandler6.AppendLiteral("/");
						debugLogErrorStringHandler6.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t4));
						debugLogErrorStringHandler6.AppendLiteral("), kr = ");
						debugLogErrorStringHandler6.AppendFormatted<int>(kern_return_t.Value);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler6);
					throw new NotSupportedException("vm_map() failed");
				}
			}
			finally
			{
				kern_return_t = OSX.mach_vm_deallocate(num, num2, (ulong)((long)intPtr2));
				if (!kern_return_t)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler7 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(53, 3, out flag);
					if (flag)
					{
						debugLogErrorStringHandler7.AppendLiteral("Could not deallocate created memory page 0x");
						debugLogErrorStringHandler7.AppendFormatted<ulong>(num2, "x16");
						debugLogErrorStringHandler7.AppendLiteral("+0x");
						debugLogErrorStringHandler7.AppendFormatted<IntPtr>(intPtr2, "x");
						debugLogErrorStringHandler7.AppendLiteral("! kr = ");
						debugLogErrorStringHandler7.AppendFormatted<int>(kern_return_t.Value);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler7);
				}
			}
		}

		private static bool TryGetProtForMem([NativeInteger] IntPtr addr, int length, out OSX.vm_prot_t maxProt, out OSX.vm_prot_t prot, out bool crossesAllocBoundary, out bool notAllocated)
		{
			maxProt = (OSX.vm_prot_t)(-1);
			prot = (OSX.vm_prot_t)(-1);
			crossesAllocBoundary = false;
			notAllocated = false;
			IntPtr intPtr = addr;
			while (addr < intPtr + (IntPtr)length)
			{
				IntPtr intPtr2;
				IntPtr intPtr3;
				OSX.vm_prot_t vm_prot_t;
				OSX.vm_prot_t vm_prot_t2;
				OSX.kern_return_t localRegionInfo = MacOSSystem.GetLocalRegionInfo(addr, out intPtr2, out intPtr3, out vm_prot_t, out vm_prot_t2);
				if (localRegionInfo)
				{
					if (intPtr2 > addr)
					{
						notAllocated = true;
						return false;
					}
					prot &= vm_prot_t;
					maxProt &= vm_prot_t2;
					addr = intPtr2 + intPtr3;
					if (addr >= intPtr + (IntPtr)length)
					{
						break;
					}
					crossesAllocBoundary = true;
				}
				else
				{
					if (localRegionInfo == OSX.kern_return_t.NoSpace)
					{
						notAllocated = true;
						return false;
					}
					return false;
				}
			}
			return true;
		}

		private unsafe static OSX.kern_return_t GetLocalRegionInfo([NativeInteger] IntPtr origAddr, [NativeInteger] out IntPtr startAddr, [NativeInteger] out IntPtr outSize, out OSX.vm_prot_t prot, out OSX.vm_prot_t maxProt)
		{
			int maxValue = int.MaxValue;
			int count = OSX.vm_region_submap_short_info_64.Count;
			ulong num = (ulong)((long)origAddr);
			ulong num2;
			OSX.vm_region_submap_short_info_64 vm_region_submap_short_info_;
			OSX.kern_return_t kern_return_t = OSX.mach_vm_region_recurse(OSX.mach_task_self(), &num, &num2, &maxValue, &vm_region_submap_short_info_, &count);
			if (!kern_return_t)
			{
				startAddr = (IntPtr)0;
				outSize = (IntPtr)0;
				prot = OSX.vm_prot_t.None;
				maxProt = OSX.vm_prot_t.None;
				return kern_return_t;
			}
			Helpers.Assert(!vm_region_submap_short_info_.is_submap, null, "!info.is_submap");
			startAddr = (IntPtr)num;
			outSize = (IntPtr)num2;
			prot = vm_region_submap_short_info_.protection;
			maxProt = vm_region_submap_short_info_.max_protection;
			return kern_return_t;
		}

		[Nullable(1)]
		public IMemoryAllocator MemoryAllocator
		{
			[NullableContext(1)]
			get;
		} = new QueryingPagedMemoryAllocator(new MacOSSystem.MacOsQueryingAllocator());

		[NullableContext(1)]
		void IInitialize<IArchitecture>.Initialize(IArchitecture value)
		{
			this.arch = value;
		}

		[Nullable(2)]
		public INativeExceptionHelper NativeExceptionHelper
		{
			[NullableContext(2)]
			get
			{
				PosixExceptionHelper posixExceptionHelper;
				if ((posixExceptionHelper = this.lazyNativeExceptionHelper) == null)
				{
					posixExceptionHelper = (this.lazyNativeExceptionHelper = this.CreateNativeExceptionHelper());
				}
				return posixExceptionHelper;
			}
		}

		public IntPtr GetNativeJitHookConfig(int runtimeMajMin)
		{
			MacOSSystem.JitMemcpyHelper jitMemcpyHelper = this.NativeExceptionHelper as MacOSSystem.JitMemcpyHelper;
			if (jitMemcpyHelper != null)
			{
				return jitMemcpyHelper.GetJitHookConfig(runtimeMajMin);
			}
			return IntPtr.Zero;
		}

		private unsafe static global::System.ReadOnlySpan<byte> NEHTempl
		{
			get
			{
				return new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.D2092BB8F0B39D69931977DCBF7F6A48B0209B37F74004EAB47FA529ECBA7672), 29);
			}
		}

		[NullableContext(1)]
		private PosixExceptionHelper CreateNativeExceptionHelper()
		{
			Helpers.Assert(this.arch != null, null, "arch is not null");
			ArchitectureKind target = this.arch.Target;
			string text;
			if (target != ArchitectureKind.x86_64)
			{
				if (target != ArchitectureKind.Arm64)
				{
					throw new NotImplementedException("No exception helper for current arch");
				}
				text = "exhelper_macos_arm64.dylib";
			}
			else
			{
				text = "exhelper_macos_x86_64.dylib";
			}
			string text2 = text;
			string text3;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(text2))
			{
				Helpers.Assert(manifestResourceStream != null, null, "embedded is not null");
				text3 = MacOSSystem.MacOSNativeLibDrop.Instance.DropLibrary(manifestResourceStream, MacOSSystem.NEHTempl);
			}
			if (this.arch.Target != ArchitectureKind.Arm64)
			{
				return PosixExceptionHelper.CreateHelper(this.arch, text3);
			}
			return MacOSSystem.JitMemcpyHelper.CreateHelper(this.arch, text3);
		}

		[Nullable(2)]
		private IArchitecture arch;

		[Nullable(2)]
		private PosixExceptionHelper lazyNativeExceptionHelper;

		private sealed class MacOsQueryingAllocator : QueryingMemoryPageAllocatorBase
		{
			public override uint PageSize { get; }

			public MacOsQueryingAllocator()
			{
				this.PageSize = (uint)OSX.GetPageSize();
			}

			public unsafe override bool TryAllocatePage([NativeInteger] IntPtr size, bool executable, out IntPtr allocated)
			{
				Helpers.Assert((long)size == (long)((ulong)this.PageSize), null, "size == PageSize");
				OSX.vm_prot_t vm_prot_t = (executable ? OSX.vm_prot_t.Execute : OSX.vm_prot_t.None);
				vm_prot_t |= OSX.vm_prot_t.Default;
				if (PlatformDetection.Architecture == ArchitectureKind.Arm64 && vm_prot_t == OSX.vm_prot_t.All)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace("RWX memory detected, doing mmap with MAP_JIT");
					allocated = OSX.mmap(IntPtr.Zero, (ulong)((long)size), OSX.map_prot.Read | OSX.map_prot.Write | OSX.map_prot.Execute, OSX.map_flags.Private | OSX.map_flags.JIT | OSX.map_flags.Anonymous, -1, 0L);
					bool flag;
					if (allocated == (IntPtr)(-1))
					{
						int errno = OSX.Errno;
						Win32Exception ex = new Win32Exception(errno);
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(37, 2, out flag);
						if (flag)
						{
							debugLogErrorStringHandler.AppendLiteral("Error creating allocation anywhere! ");
							debugLogErrorStringHandler.AppendFormatted<int>(errno);
							debugLogErrorStringHandler.AppendLiteral(" ");
							debugLogErrorStringHandler.AppendFormatted<Win32Exception>(ex);
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
						allocated = 0;
						return false;
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(37, 2, out flag);
					if (flag)
					{
						debugLogTraceStringHandler.AppendLiteral("RWX memory allocated to 0x");
						debugLogTraceStringHandler.AppendFormatted<IntPtr>(allocated, "X16");
						debugLogTraceStringHandler.AppendLiteral(" with size ");
						debugLogTraceStringHandler.AppendFormatted<IntPtr>(size);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
					return true;
				}
				else
				{
					ulong num = 0UL;
					OSX.kern_return_t kern_return_t = OSX.mach_vm_map(OSX.mach_task_self(), &num, (ulong)((long)size), 0UL, OSX.vm_flags.Anywhere, 0, 0UL, true, vm_prot_t, vm_prot_t, OSX.vm_inherit_t.Copy);
					if (!kern_return_t)
					{
						bool flag;
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler2 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(41, 1, out flag);
						if (flag)
						{
							debugLogErrorStringHandler2.AppendLiteral("Error creating allocation anywhere! kr = ");
							debugLogErrorStringHandler2.AppendFormatted<int>(kern_return_t.Value);
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler2);
						allocated = 0;
						return false;
					}
					allocated = (IntPtr)((long)num);
					return true;
				}
			}

			public unsafe override bool TryAllocatePage(IntPtr pageAddr, [NativeInteger] IntPtr size, bool executable, out IntPtr allocated)
			{
				Helpers.Assert((long)size == (long)((ulong)this.PageSize), null, "size == PageSize");
				OSX.vm_prot_t vm_prot_t = (executable ? OSX.vm_prot_t.Execute : OSX.vm_prot_t.None);
				vm_prot_t |= OSX.vm_prot_t.Default;
				if (PlatformDetection.Architecture == ArchitectureKind.Arm64 && vm_prot_t == OSX.vm_prot_t.All)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace("RWX memory detected, doing mmap with MAP_JIT");
					allocated = OSX.mmap(pageAddr, (ulong)((long)size), OSX.map_prot.Read | OSX.map_prot.Write | OSX.map_prot.Execute, OSX.map_flags.Private | OSX.map_flags.Fixed | OSX.map_flags.JIT | OSX.map_flags.Anonymous, -1, 0L);
					bool flag;
					if (allocated == (IntPtr)(-1))
					{
						int errno = OSX.Errno;
						Win32Exception ex = new Win32Exception(errno);
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(37, 2, out flag);
						if (flag)
						{
							debugLogErrorStringHandler.AppendLiteral("Error creating allocation anywhere! ");
							debugLogErrorStringHandler.AppendFormatted<int>(errno);
							debugLogErrorStringHandler.AppendLiteral(" ");
							debugLogErrorStringHandler.AppendFormatted<Win32Exception>(ex);
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
						allocated = 0;
						return false;
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(45, 2, out flag);
					if (flag)
					{
						debugLogTraceStringHandler.AppendLiteral("RWX memory allocated to page at 0x");
						debugLogTraceStringHandler.AppendFormatted<IntPtr>(pageAddr, "X16");
						debugLogTraceStringHandler.AppendLiteral(" with size ");
						debugLogTraceStringHandler.AppendFormatted<IntPtr>(size);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
					return true;
				}
				else
				{
					ulong num = (ulong)(long)pageAddr;
					OSX.kern_return_t kern_return_t = OSX.mach_vm_map(OSX.mach_task_self(), &num, (ulong)((long)size), 0UL, OSX.vm_flags.Fixed, 0, 0UL, true, vm_prot_t, vm_prot_t, OSX.vm_inherit_t.Copy);
					if (!kern_return_t)
					{
						bool flag;
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogSpamStringHandler debugLogSpamStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogSpamStringHandler(38, 2, out flag);
						if (flag)
						{
							debugLogSpamStringHandler.AppendLiteral("Error creating allocation at 0x");
							debugLogSpamStringHandler.AppendFormatted<ulong>(num, "x16");
							debugLogSpamStringHandler.AppendLiteral(": kr = ");
							debugLogSpamStringHandler.AppendFormatted<int>(kern_return_t.Value);
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Spam(ref debugLogSpamStringHandler);
						allocated = 0;
						return false;
					}
					allocated = (IntPtr)((long)num);
					return true;
				}
			}

			[NullableContext(2)]
			public override bool TryFreePage(IntPtr pageAddr, [<24b3ba8a-00b7-40fc-a603-2711fa115297>NotNullWhen(false)] out string errorMsg)
			{
				OSX.kern_return_t kern_return_t = OSX.mach_vm_deallocate(OSX.mach_task_self(), (ulong)(long)pageAddr, (ulong)this.PageSize);
				if (!kern_return_t)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Could not deallocate page: kr = ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(kern_return_t.Value);
					errorMsg = defaultInterpolatedStringHandler.ToStringAndClear();
					return false;
				}
				errorMsg = null;
				return true;
			}

			public override bool TryQueryPage(IntPtr pageAddr, out bool isFree, out IntPtr allocBase, [NativeInteger] out IntPtr allocSize)
			{
				OSX.vm_prot_t vm_prot_t;
				OSX.vm_prot_t vm_prot_t2;
				OSX.kern_return_t localRegionInfo = MacOSSystem.GetLocalRegionInfo(pageAddr, out allocBase, out allocSize, out vm_prot_t, out vm_prot_t2);
				if (localRegionInfo)
				{
					if (allocBase > pageAddr)
					{
						allocSize = allocBase - pageAddr;
						allocBase = pageAddr;
						isFree = true;
						return true;
					}
					isFree = false;
					return true;
				}
				else
				{
					if (localRegionInfo == OSX.kern_return_t.InvalidAddress)
					{
						isFree = true;
						return true;
					}
					isFree = false;
					return false;
				}
			}
		}

		private sealed class MacOSNativeLibDrop : PosixNativeLibraryDrop
		{
			protected override void CloseFileDescriptor([NativeInteger] IntPtr fd)
			{
				OSX.Close((int)fd);
			}

			[return: NativeInteger]
			protected unsafe override IntPtr Mkstemp(global::System.Span<byte> template)
			{
				int num;
				fixed (byte* pinnableReference = template.GetPinnableReference())
				{
					num = OSX.MkSTemp(pinnableReference);
				}
				if (num == -1)
				{
					int errno = OSX.Errno;
					Win32Exception ex = new Win32Exception(errno);
					bool flag;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(29, 2, out flag);
					if (flag)
					{
						debugLogErrorStringHandler.AppendLiteral("Could not create temp file: ");
						debugLogErrorStringHandler.AppendFormatted<int>(errno);
						debugLogErrorStringHandler.AppendLiteral(" ");
						debugLogErrorStringHandler.AppendFormatted<Win32Exception>(ex);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
					throw ex;
				}
				return (IntPtr)num;
			}

			[Nullable(1)]
			public static readonly MacOSSystem.MacOSNativeLibDrop Instance = new MacOSSystem.MacOSNativeLibDrop();
		}

		[NullableContext(1)]
		[Nullable(0)]
		private sealed class JitMemcpyHelper : PosixExceptionHelper
		{
			private JitMemcpyHelper(IArchitecture arch, IntPtr getExPtr, IntPtr m2n, IntPtr n2m, IntPtr memcpy, IntPtr jitCfg)
				: base(arch, getExPtr, m2n, n2m)
			{
				this.mmch_jit_memcpy = memcpy;
				this.mmch_jit_hook_config = jitCfg;
			}

			public new static MacOSSystem.JitMemcpyHelper CreateHelper(IArchitecture arch, string filename)
			{
				IntPtr intPtr = DynDll.OpenLibrary(filename);
				IntPtr export;
				IntPtr export2;
				IntPtr export3;
				IntPtr export4;
				IntPtr export5;
				try
				{
					export = intPtr.GetExport("eh_get_exception_ptr");
					export2 = intPtr.GetExport("eh_managed_to_native");
					export3 = intPtr.GetExport("eh_native_to_managed");
					export4 = intPtr.GetExport("mmch_jit_memcpy");
					export5 = intPtr.GetExport("mmch_jit_hook_config");
					Helpers.Assert(export != IntPtr.Zero, null, "eh_get_exception_ptr != IntPtr.Zero");
					Helpers.Assert(export2 != IntPtr.Zero, null, "eh_managed_to_native != IntPtr.Zero");
					Helpers.Assert(export3 != IntPtr.Zero, null, "eh_native_to_managed != IntPtr.Zero");
					Helpers.Assert(export3 != IntPtr.Zero, null, "eh_native_to_managed != IntPtr.Zero");
					Helpers.Assert(export4 != IntPtr.Zero, null, "mmch_jit_memcpy != IntPtr.Zero");
					Helpers.Assert(export5 != IntPtr.Zero, null, "mmch_jit_hook_config != IntPtr.Zero");
				}
				catch
				{
					DynDll.CloseLibrary(intPtr);
					throw;
				}
				return new MacOSSystem.JitMemcpyHelper(arch, export, export2, export3, export4, export5);
			}

			public unsafe void JitMemCpy(IntPtr dst, IntPtr src, ulong size)
			{
				delegate* unmanaged[Cdecl]<IntPtr, IntPtr, ulong, void> system.Void_u0020(System.IntPtr,System.IntPtr,System.UInt64) = (void*)this.mmch_jit_memcpy;
				delegate* unmanaged[Cdecl]<IntPtr, IntPtr, ulong, void> system.Void_u0020(System.IntPtr,System.IntPtr,System.UInt64)2 = system.Void_u0020(System.IntPtr,System.IntPtr,System.UInt64);
				calli(System.Void(System.IntPtr,System.IntPtr,System.UInt64), dst, src, size, system.Void_u0020(System.IntPtr,System.IntPtr,System.UInt64)2);
			}

			internal unsafe IntPtr GetJitHookConfig(int runtimeMajMin)
			{
				delegate* unmanaged[Cdecl]<int, IntPtr> system.IntPtr_u0020(System.Int32) = (void*)this.mmch_jit_hook_config;
				delegate* unmanaged[Cdecl]<int, IntPtr> system.IntPtr_u0020(System.Int32)2 = system.IntPtr_u0020(System.Int32);
				return calli(System.IntPtr(System.Int32), runtimeMajMin, system.IntPtr_u0020(System.Int32)2);
			}

			private readonly IntPtr mmch_jit_memcpy;

			private readonly IntPtr mmch_jit_hook_config;
		}

		[CompilerGenerated]
		private static class <>O
		{
			public static Classifier <0>__ClassifyAMD64;

			public static Classifier <1>__ClassifyARM64;
		}
	}
}
