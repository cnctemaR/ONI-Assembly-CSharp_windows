using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoMod.Utils.Interop;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class DynDll
	{
		private static DynDll.BackendImpl CreateCrossplatBackend()
		{
			OSKind os = PlatformDetection.OS;
			if (os.Is(OSKind.Windows))
			{
				return new DynDll.WindowsBackend();
			}
			if (os.Is(OSKind.Linux) || os.Is(OSKind.OSX))
			{
				return new DynDll.LinuxOSXBackend(os.Is(OSKind.Linux));
			}
			bool flag;
			MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new MMDbgLog.DebugLogWarningStringHandler(55, 1, out flag);
			if (flag)
			{
				debugLogWarningStringHandler.AppendLiteral("Unknown OS ");
				debugLogWarningStringHandler.AppendFormatted<OSKind>(os);
				debugLogWarningStringHandler.AppendLiteral(" when setting up DynDll; assuming posix-like");
			}
			MMDbgLog.Warning(ref debugLogWarningStringHandler);
			return new DynDll.UnknownPosixBackend();
		}

		[NullableContext(2)]
		public static IntPtr OpenLibrary(string name)
		{
			return DynDll.Backend.OpenLibrary(name, Assembly.GetCallingAssembly());
		}

		[NullableContext(2)]
		public static bool TryOpenLibrary(string name, out IntPtr libraryPtr)
		{
			return DynDll.Backend.TryOpenLibrary(name, Assembly.GetCallingAssembly(), out libraryPtr);
		}

		public static void CloseLibrary(IntPtr lib)
		{
			DynDll.Backend.CloseLibrary(lib);
		}

		public static bool TryCloseLibrary(IntPtr lib)
		{
			return DynDll.Backend.TryCloseLibrary(lib);
		}

		public static IntPtr GetExport(this IntPtr libraryPtr, string name)
		{
			return DynDll.Backend.GetExport(libraryPtr, name);
		}

		public static bool TryGetExport(this IntPtr libraryPtr, string name, out IntPtr functionPtr)
		{
			return DynDll.Backend.TryGetExport(libraryPtr, name, out functionPtr);
		}

		private static readonly DynDll.BackendImpl Backend = DynDll.CreateCrossplatBackend();

		[Nullable(0)]
		private abstract class BackendImpl
		{
			protected abstract bool TryOpenLibraryCore([Nullable(2)] string name, Assembly assembly, out IntPtr handle);

			public abstract bool TryCloseLibrary(IntPtr handle);

			public abstract bool TryGetExport(IntPtr handle, string name, out IntPtr ptr);

			protected abstract void CheckAndThrowError();

			public virtual bool TryOpenLibrary([Nullable(2)] string name, Assembly assembly, out IntPtr handle)
			{
				if (name != null)
				{
					foreach (string text in this.GetLibrarySearchOrder(name))
					{
						if (this.TryOpenLibraryCore(text, assembly, out handle))
						{
							return true;
						}
					}
					handle = IntPtr.Zero;
					return false;
				}
				return this.TryOpenLibraryCore(null, assembly, out handle);
			}

			protected virtual IEnumerable<string> GetLibrarySearchOrder(string name)
			{
				DynDll.BackendImpl.<GetLibrarySearchOrder>d__6 <GetLibrarySearchOrder>d__ = new DynDll.BackendImpl.<GetLibrarySearchOrder>d__6(-2);
				<GetLibrarySearchOrder>d__.<>3__name = name;
				return <GetLibrarySearchOrder>d__;
			}

			public virtual IntPtr OpenLibrary([Nullable(2)] string name, Assembly assembly)
			{
				IntPtr intPtr;
				if (!this.TryOpenLibrary(name, assembly, out intPtr))
				{
					this.CheckAndThrowError();
				}
				return intPtr;
			}

			public virtual void CloseLibrary(IntPtr handle)
			{
				if (!this.TryCloseLibrary(handle))
				{
					this.CheckAndThrowError();
				}
			}

			public virtual IntPtr GetExport(IntPtr handle, string name)
			{
				IntPtr intPtr;
				if (!this.TryGetExport(handle, name, out intPtr))
				{
					this.CheckAndThrowError();
				}
				return intPtr;
			}
		}

		[Nullable(0)]
		private sealed class WindowsBackend : DynDll.BackendImpl
		{
			protected override void CheckAndThrowError()
			{
				uint lastError = Windows.GetLastError();
				if (lastError != 0U)
				{
					throw new Win32Exception((int)lastError);
				}
			}

			protected unsafe override bool TryOpenLibraryCore([Nullable(2)] string name, Assembly assembly, out IntPtr handle)
			{
				IntPtr intPtr;
				if (name == null)
				{
					intPtr = (handle = Windows.GetModuleHandleW(null));
				}
				else
				{
					fixed (char* pinnableReference = name.AsSpan().GetPinnableReference())
					{
						char* ptr = pinnableReference;
						intPtr = (handle = Windows.LoadLibraryW((ushort*)ptr));
					}
				}
				return intPtr != IntPtr.Zero;
			}

			public unsafe override bool TryCloseLibrary(IntPtr handle)
			{
				return Windows.FreeLibrary(new Windows.HMODULE((void*)handle));
			}

			public unsafe override bool TryGetExport(IntPtr handle, string name, out IntPtr ptr)
			{
				byte[] array2;
				byte[] array = (array2 = Unix.MarshalToUtf8(name));
				byte* ptr2;
				if (array == null || array2.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array2[0];
				}
				IntPtr intPtr = (ptr = Windows.GetProcAddress(new Windows.HMODULE((void*)handle), (sbyte*)ptr2));
				array2 = null;
				Unix.FreeMarshalledArray(array);
				return intPtr != IntPtr.Zero;
			}

			protected override IEnumerable<string> GetLibrarySearchOrder(string name)
			{
				DynDll.WindowsBackend.<GetLibrarySearchOrder>d__4 <GetLibrarySearchOrder>d__ = new DynDll.WindowsBackend.<GetLibrarySearchOrder>d__4(-2);
				<GetLibrarySearchOrder>d__.<>3__name = name;
				return <GetLibrarySearchOrder>d__;
			}
		}

		[Nullable(0)]
		private abstract class LibdlBackend : DynDll.BackendImpl
		{
			protected LibdlBackend()
			{
				Unix.DlError();
			}

			[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
			private static void ThrowError(IntPtr dlerr)
			{
				throw new Win32Exception(Marshal.PtrToStringAnsi(dlerr));
			}

			protected override void CheckAndThrowError()
			{
				IntPtr intPtr = DynDll.LibdlBackend.lastDlErrorReturn;
				IntPtr intPtr2;
				if (intPtr == IntPtr.Zero)
				{
					intPtr2 = Unix.DlError();
				}
				else
				{
					intPtr2 = intPtr;
					DynDll.LibdlBackend.lastDlErrorReturn = IntPtr.Zero;
				}
				if (intPtr2 != IntPtr.Zero)
				{
					DynDll.LibdlBackend.ThrowError(intPtr2);
				}
			}

			protected override bool TryOpenLibraryCore([Nullable(2)] string name, Assembly assembly, out IntPtr handle)
			{
				Unix.DlopenFlags dlopenFlags = (Unix.DlopenFlags)258;
				return (handle = Unix.DlOpen(name, dlopenFlags)) != IntPtr.Zero;
			}

			public override bool TryCloseLibrary(IntPtr handle)
			{
				return Unix.DlClose(handle);
			}

			public override bool TryGetExport(IntPtr handle, string name, out IntPtr ptr)
			{
				Unix.DlError();
				ptr = Unix.DlSym(handle, name);
				return (DynDll.LibdlBackend.lastDlErrorReturn = Unix.DlError()) == IntPtr.Zero;
			}

			public override IntPtr GetExport(IntPtr handle, string name)
			{
				Unix.DlError();
				IntPtr intPtr = Unix.DlSym(handle, name);
				IntPtr intPtr2 = Unix.DlError();
				if (intPtr2 != IntPtr.Zero)
				{
					DynDll.LibdlBackend.ThrowError(intPtr2);
				}
				return intPtr;
			}

			[ThreadStatic]
			private static IntPtr lastDlErrorReturn;
		}

		[NullableContext(0)]
		private sealed class LinuxOSXBackend : DynDll.LibdlBackend
		{
			public LinuxOSXBackend(bool isLinux)
			{
				this.isLinux = isLinux;
			}

			[NullableContext(1)]
			protected override IEnumerable<string> GetLibrarySearchOrder(string name)
			{
				DynDll.LinuxOSXBackend.<GetLibrarySearchOrder>d__2 <GetLibrarySearchOrder>d__ = new DynDll.LinuxOSXBackend.<GetLibrarySearchOrder>d__2(-2);
				<GetLibrarySearchOrder>d__.<>4__this = this;
				<GetLibrarySearchOrder>d__.<>3__name = name;
				return <GetLibrarySearchOrder>d__;
			}

			private readonly bool isLinux;
		}

		[NullableContext(0)]
		private sealed class UnknownPosixBackend : DynDll.LibdlBackend
		{
		}
	}
}
