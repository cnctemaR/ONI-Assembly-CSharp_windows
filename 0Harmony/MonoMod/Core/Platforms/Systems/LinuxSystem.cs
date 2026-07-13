using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Core.Interop;
using MonoMod.Core.Platforms.Memory;
using MonoMod.Core.Utils;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Systems
{
	internal sealed class LinuxSystem : ISystem, IInitialize<IArchitecture>
	{
		public OSKind Target
		{
			get
			{
				return OSKind.Linux;
			}
		}

		public SystemFeature Features
		{
			get
			{
				return SystemFeature.RWXPages | SystemFeature.RXPages;
			}
		}

		public Abi? DefaultAbi
		{
			get
			{
				return new Abi?(this.defaultAbi);
			}
		}

		[return: Nullable(new byte[] { 1, 2 })]
		public IEnumerable<string> EnumerateLoadedModuleFiles()
		{
			return from ProcessModule m in Process.GetCurrentProcess().Modules
				select m.FileName;
		}

		[Nullable(1)]
		public IMemoryAllocator MemoryAllocator
		{
			[NullableContext(1)]
			get
			{
				return this.allocator;
			}
		}

		public LinuxSystem()
		{
			this.PageSize = (IntPtr)Unix.Sysconf(Unix.SysconfName.PageSize);
			this.allocator = new LinuxSystem.MmapPagedMemoryAllocator(this.PageSize);
			ArchitectureKind architecture = PlatformDetection.Architecture;
			if (architecture == ArchitectureKind.x86_64)
			{
				global::System.ReadOnlyMemory<SpecialArgumentKind> readOnlyMemory = new SpecialArgumentKind[]
				{
					SpecialArgumentKind.ReturnBuffer,
					SpecialArgumentKind.ThisPointer,
					SpecialArgumentKind.UserArguments
				};
				Classifier classifier;
				if ((classifier = LinuxSystem.<>O.<0>__ClassifyAMD64) == null)
				{
					classifier = (LinuxSystem.<>O.<0>__ClassifyAMD64 = new Classifier(SystemVABI.ClassifyAMD64));
				}
				this.defaultAbi = new Abi(readOnlyMemory, classifier, true);
				return;
			}
			if (architecture != ArchitectureKind.Arm64)
			{
				throw new NotImplementedException();
			}
			global::System.ReadOnlyMemory<SpecialArgumentKind> readOnlyMemory2 = new SpecialArgumentKind[]
			{
				SpecialArgumentKind.ThisPointer,
				SpecialArgumentKind.UserArguments
			};
			Classifier classifier2;
			if ((classifier2 = LinuxSystem.<>O.<1>__ClassifyARM64) == null)
			{
				classifier2 = (LinuxSystem.<>O.<1>__ClassifyARM64 = new Classifier(SystemVABI.ClassifyARM64));
			}
			this.defaultAbi = new Abi(readOnlyMemory2, classifier2, false);
		}

		[return: NativeInteger]
		public IntPtr GetSizeOfReadableMemory(IntPtr start, [NativeInteger] IntPtr guess)
		{
			IntPtr intPtr = this.allocator.RoundDownToPageBoundary(start);
			if (!LinuxSystem.MmapPagedMemoryAllocator.PageReadable(intPtr))
			{
				return (IntPtr)0;
			}
			intPtr += this.PageSize;
			IntPtr intPtr2 = intPtr - start;
			while (intPtr2 < guess)
			{
				if (!LinuxSystem.MmapPagedMemoryAllocator.PageReadable(intPtr))
				{
					return intPtr2;
				}
				intPtr2 += this.PageSize;
				intPtr += this.PageSize;
			}
			return intPtr2;
		}

		public unsafe void PatchData(PatchTargetKind patchKind, IntPtr patchTarget, global::System.ReadOnlySpan<byte> data, global::System.Span<byte> backup)
		{
			if (patchKind == PatchTargetKind.Executable)
			{
				this.ProtectRWX(patchTarget, (IntPtr)data.Length);
			}
			else
			{
				this.ProtectRW(patchTarget, (IntPtr)data.Length);
			}
			global::System.Span<byte> span = new global::System.Span<byte>((void*)patchTarget, data.Length);
			span.TryCopyTo(backup);
			data.CopyTo(span);
		}

		private void RoundToPageBoundary([NativeInteger] ref IntPtr addr, [NativeInteger] ref IntPtr size)
		{
			IntPtr intPtr = this.allocator.RoundDownToPageBoundary(addr);
			size += addr - intPtr;
			addr = intPtr;
		}

		private void ProtectRW(IntPtr addr, [NativeInteger] IntPtr size)
		{
			this.RoundToPageBoundary(ref addr, ref size);
			if (Unix.Mprotect(addr, (UIntPtr)size, Unix.Protection.Read | Unix.Protection.Write) != 0)
			{
				throw new Win32Exception(Unix.Errno);
			}
		}

		private void ProtectRWX(IntPtr addr, [NativeInteger] IntPtr size)
		{
			this.RoundToPageBoundary(ref addr, ref size);
			if (Unix.Mprotect(addr, (UIntPtr)size, Unix.Protection.Read | Unix.Protection.Write | Unix.Protection.Execute) != 0)
			{
				throw new Win32Exception(Unix.Errno);
			}
		}

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

		private unsafe static global::System.ReadOnlySpan<byte> NEHTempl
		{
			get
			{
				return new global::System.ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.93CA2598B856E56332AD9FCA06FE4AE26CF02ED7D37C8C4D790EDFC53FB9DA81), 26);
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
				text = "exhelper_linux_arm64.so";
			}
			else
			{
				text = "exhelper_linux_x86_64.so";
			}
			string text2 = text;
			string text3;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(text2))
			{
				Helpers.Assert(manifestResourceStream != null, null, "embedded is not null");
				text3 = LinuxSystem.LinuxNativeLibDrop.Instance.DropLibrary(manifestResourceStream, LinuxSystem.NEHTempl);
			}
			return PosixExceptionHelper.CreateHelper(this.arch, text3);
		}

		public IntPtr GetNativeJitHookConfig(int runtimeMajMin)
		{
			throw new NotImplementedException();
		}

		private readonly Abi defaultAbi;

		[NativeInteger]
		private readonly IntPtr PageSize;

		[Nullable(1)]
		private readonly LinuxSystem.MmapPagedMemoryAllocator allocator;

		[Nullable(2)]
		private IArchitecture arch;

		[Nullable(2)]
		private PosixExceptionHelper lazyNativeExceptionHelper;

		[NullableContext(1)]
		[Nullable(0)]
		private sealed class MmapPagedMemoryAllocator : PagedMemoryAllocator
		{
			public MmapPagedMemoryAllocator([NativeInteger] IntPtr pageSize)
				: base(pageSize)
			{
			}

			unsafe static MmapPagedMemoryAllocator()
			{
				IntPtr intPtr = stackalloc byte[(UIntPtr)8];
				if (Unix.Pipe2(intPtr, Unix.PipeFlags.CloseOnExec) == -1)
				{
					throw new Win32Exception(Unix.Errno, "Failed to create pipe for page probes");
				}
				LinuxSystem.MmapPagedMemoryAllocator.PageProbePipeReadFD = *intPtr;
				LinuxSystem.MmapPagedMemoryAllocator.PageProbePipeWriteFD = *(intPtr + 4);
			}

			public unsafe static bool PageAllocated([NativeInteger] IntPtr page)
			{
				byte b;
				if (Unix.Mincore(page, (UIntPtr)((IntPtr)1), &b) != -1)
				{
					return true;
				}
				int errno = Unix.Errno;
				if (errno == 12)
				{
					return false;
				}
				if (errno == 38)
				{
					throw new LinuxSystem.MmapPagedMemoryAllocator.SyscallNotImplementedException();
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Got unimplemented errno for mincore(2); errno = ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(errno);
				throw new NotImplementedException(defaultInterpolatedStringHandler.ToStringAndClear());
			}

			public unsafe static bool PageReadable([NativeInteger] IntPtr page)
			{
				if (Unix.Write(LinuxSystem.MmapPagedMemoryAllocator.PageProbePipeWriteFD, page, (IntPtr)1) == (IntPtr)(-1))
				{
					int errno = Unix.Errno;
					if (errno == 14)
					{
						return false;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Got unimplemented errno for write(2); errno = ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(errno);
					throw new NotImplementedException(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					byte b;
					if (Unix.Read(LinuxSystem.MmapPagedMemoryAllocator.PageProbePipeReadFD, new IntPtr((void*)(&b)), (IntPtr)1) == (IntPtr)(-1))
					{
						throw new Win32Exception("Failed to clean up page probe pipe after successful page probe");
					}
					return true;
				}
			}

			protected override bool TryAllocateNewPage(AllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
			{
				Unix.Protection protection = (request.Executable ? Unix.Protection.Execute : Unix.Protection.None);
				protection |= Unix.Protection.Read | Unix.Protection.Write;
				IntPtr intPtr = Unix.Mmap(IntPtr.Zero, (UIntPtr)base.PageSize, protection, Unix.MmapFlags.Private | Unix.MmapFlags.Anonymous, -1, 0);
				long num = (long)intPtr;
				bool flag = num - -1L <= 1L;
				if (flag)
				{
					int errno = Unix.Errno;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(28, 2, out flag);
					if (flag)
					{
						debugLogErrorStringHandler.AppendLiteral("Error creating allocation: ");
						debugLogErrorStringHandler.AppendFormatted<int>(errno);
						debugLogErrorStringHandler.AppendLiteral(" ");
						debugLogErrorStringHandler.AppendFormatted(new Win32Exception(errno).Message);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
					allocated = null;
					return false;
				}
				PagedMemoryAllocator.Page page = new PagedMemoryAllocator.Page(this, intPtr, (uint)base.PageSize, request.Executable);
				base.InsertAllocatedPage(page);
				PagedMemoryAllocator.PageAllocation pageAllocation;
				if (!page.TryAllocate((uint)request.Size, (uint)request.Alignment, out pageAllocation))
				{
					base.RegisterForCleanup(page);
					allocated = null;
					return false;
				}
				allocated = pageAllocation;
				return true;
			}

			protected override bool TryAllocateNewPage(PositionedAllocationRequest request, [NativeInteger] IntPtr targetPage, [NativeInteger] IntPtr lowPageBound, [NativeInteger] IntPtr highPageBound, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
			{
				if (!this.canTestPageAllocation)
				{
					allocated = null;
					return false;
				}
				Unix.Protection protection = (request.Base.Executable ? Unix.Protection.Execute : Unix.Protection.None);
				protection |= Unix.Protection.Read | Unix.Protection.Write;
				IntPtr intPtr = (IntPtr)request.Base.Size / base.PageSize + (IntPtr)1;
				IntPtr intPtr2 = targetPage - base.PageSize;
				IntPtr intPtr3 = targetPage;
				IntPtr intPtr4 = (IntPtr)(-1);
				try
				{
					IL_00C5:
					while (intPtr2 >= lowPageBound || intPtr3 <= highPageBound)
					{
						if (intPtr3 <= highPageBound)
						{
							for (IntPtr intPtr5 = (IntPtr)0; intPtr5 < intPtr; intPtr5++)
							{
								if (LinuxSystem.MmapPagedMemoryAllocator.PageAllocated(intPtr3 + base.PageSize * intPtr5))
								{
									intPtr3 += base.PageSize;
									goto IL_008E;
								}
							}
							intPtr4 = intPtr3;
							break;
						}
						IL_008E:
						if (intPtr2 >= lowPageBound)
						{
							for (IntPtr intPtr6 = (IntPtr)0; intPtr6 < intPtr; intPtr6++)
							{
								if (LinuxSystem.MmapPagedMemoryAllocator.PageAllocated(intPtr2 + base.PageSize * intPtr6))
								{
									intPtr2 -= base.PageSize;
									goto IL_00C5;
								}
							}
							intPtr4 = intPtr2;
							break;
						}
					}
				}
				catch (LinuxSystem.MmapPagedMemoryAllocator.SyscallNotImplementedException)
				{
					this.canTestPageAllocation = false;
					allocated = null;
					return false;
				}
				if (intPtr4 == (IntPtr)(-1))
				{
					allocated = null;
					return false;
				}
				IntPtr intPtr7 = Unix.Mmap(intPtr4, (UIntPtr)base.PageSize, protection, Unix.MmapFlags.Private | Unix.MmapFlags.Anonymous | Unix.MmapFlags.FixedNoReplace, -1, 0);
				long num = (long)intPtr7;
				bool flag = num - -1L <= 1L;
				if (flag)
				{
					allocated = null;
					return false;
				}
				PagedMemoryAllocator.Page page = new PagedMemoryAllocator.Page(this, intPtr7, (uint)base.PageSize, request.Base.Executable);
				base.InsertAllocatedPage(page);
				PagedMemoryAllocator.PageAllocation pageAllocation;
				if (!page.TryAllocate((uint)request.Base.Size, (uint)request.Base.Alignment, out pageAllocation))
				{
					base.RegisterForCleanup(page);
					allocated = null;
					return false;
				}
				if (pageAllocation.BaseAddress < request.LowBound || pageAllocation.BaseAddress + (IntPtr)pageAllocation.Size >= request.HighBound)
				{
					pageAllocation.Dispose();
					allocated = null;
					return false;
				}
				allocated = pageAllocation;
				return true;
			}

			protected override bool TryFreePage(PagedMemoryAllocator.Page page, [Nullable(2)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>NotNullWhen(false)] out string errorMsg)
			{
				if (Unix.Munmap(page.BaseAddr, (UIntPtr)page.Size) != 0)
				{
					errorMsg = new Win32Exception(Unix.Errno).Message;
					return false;
				}
				errorMsg = null;
				return true;
			}

			private static int PageProbePipeReadFD;

			private static int PageProbePipeWriteFD;

			private bool canTestPageAllocation = true;

			[NullableContext(0)]
			private sealed class SyscallNotImplementedException : Exception
			{
			}
		}

		private sealed class LinuxNativeLibDrop : PosixNativeLibraryDrop
		{
			protected override void CloseFileDescriptor([NativeInteger] IntPtr fd)
			{
				Unix.Close((int)fd);
			}

			[return: NativeInteger]
			protected unsafe override IntPtr Mkstemp(global::System.Span<byte> template)
			{
				int num;
				fixed (byte* pinnableReference = template.GetPinnableReference())
				{
					num = Unix.MkSTemp(pinnableReference);
				}
				if (num == -1)
				{
					int errno = Unix.Errno;
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
			public static readonly LinuxSystem.LinuxNativeLibDrop Instance = new LinuxSystem.LinuxNativeLibDrop();
		}

		[CompilerGenerated]
		private static class <>O
		{
			public static Classifier <0>__ClassifyAMD64;

			public static Classifier <1>__ClassifyARM64;
		}
	}
}
