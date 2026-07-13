using System;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Systems
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class PosixExceptionHelper : INativeExceptionHelper
	{
		protected PosixExceptionHelper(IArchitecture arch, IntPtr getExPtr, IntPtr m2n, IntPtr n2m)
		{
			this.arch = arch;
			this.eh_get_exception_ptr = getExPtr;
			this.eh_managed_to_native = m2n;
			this.eh_native_to_managed = n2m;
		}

		public static PosixExceptionHelper CreateHelper(IArchitecture arch, string filename)
		{
			IntPtr intPtr = DynDll.OpenLibrary(filename);
			IntPtr export;
			IntPtr export2;
			IntPtr export3;
			try
			{
				export = intPtr.GetExport("eh_get_exception_ptr");
				export2 = intPtr.GetExport("eh_managed_to_native");
				export3 = intPtr.GetExport("eh_native_to_managed");
				Helpers.Assert(export != IntPtr.Zero, null, "eh_get_exception_ptr != IntPtr.Zero");
				Helpers.Assert(export2 != IntPtr.Zero, null, "eh_managed_to_native != IntPtr.Zero");
				Helpers.Assert(export3 != IntPtr.Zero, null, "eh_native_to_managed != IntPtr.Zero");
			}
			catch
			{
				DynDll.CloseLibrary(intPtr);
				throw;
			}
			return new PosixExceptionHelper(arch, export, export2, export3);
		}

		public unsafe IntPtr NativeException
		{
			get
			{
				return *calli(System.IntPtr*(), (void*)this.eh_get_exception_ptr);
			}
			set
			{
				*calli(System.IntPtr*(), (void*)this.eh_get_exception_ptr) = value;
			}
		}

		public unsafe GetExceptionSlot GetExceptionSlot
		{
			get
			{
				return () => calli(System.IntPtr*(), (void*)this.eh_get_exception_ptr);
			}
		}

		[NullableContext(2)]
		public IntPtr CreateManagedToNativeHelper(IntPtr target, out IDisposable handle)
		{
			IAllocatedMemory allocatedMemory = this.arch.CreateSpecialEntryStub(this.eh_managed_to_native, target);
			handle = allocatedMemory;
			return allocatedMemory.BaseAddress;
		}

		[NullableContext(2)]
		public IntPtr CreateNativeToManagedHelper(IntPtr target, out IDisposable handle)
		{
			IAllocatedMemory allocatedMemory = this.arch.CreateSpecialEntryStub(this.eh_native_to_managed, target);
			handle = allocatedMemory;
			return allocatedMemory.BaseAddress;
		}

		private readonly IArchitecture arch;

		private readonly IntPtr eh_get_exception_ptr;

		private readonly IntPtr eh_managed_to_native;

		private readonly IntPtr eh_native_to_managed;
	}
}
