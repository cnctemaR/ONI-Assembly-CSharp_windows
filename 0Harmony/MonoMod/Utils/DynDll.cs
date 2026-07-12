using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MonoMod.Utils
{
	internal static class DynDll
	{
		[DllImport("kernel32", SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32", SetLastError = true)]
		private static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool FreeLibrary(IntPtr hLibModule);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("dl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlopen")]
		private static extern IntPtr dl_dlopen(string filename, int flags);

		[DllImport("dl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlclose")]
		private static extern bool dl_dlclose(IntPtr handle);

		[DllImport("dl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlsym")]
		private static extern IntPtr dl_dlsym(IntPtr handle, string symbol);

		[DllImport("dl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlerror")]
		private static extern IntPtr dl_dlerror();

		[DllImport("libdl.so.2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlopen")]
		private static extern IntPtr dl2_dlopen(string filename, int flags);

		[DllImport("libdl.so.2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlclose")]
		private static extern bool dl2_dlclose(IntPtr handle);

		[DllImport("libdl.so.2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlsym")]
		private static extern IntPtr dl2_dlsym(IntPtr handle, string symbol);

		[DllImport("libdl.so.2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlerror")]
		private static extern IntPtr dl2_dlerror();

		private static IntPtr dlopen(string filename, int flags)
		{
			IntPtr intPtr;
			for (;;)
			{
				try
				{
					int num = DynDll.dlVersion;
					if (num != 0 && num == 1)
					{
						intPtr = DynDll.dl2_dlopen(filename, flags);
					}
					else
					{
						intPtr = DynDll.dl_dlopen(filename, flags);
					}
				}
				catch (DllNotFoundException obj) when (DynDll.dlVersion > 0)
				{
					DynDll.dlVersion--;
					continue;
				}
				break;
			}
			return intPtr;
		}

		private static bool dlclose(IntPtr handle)
		{
			bool flag;
			for (;;)
			{
				try
				{
					int num = DynDll.dlVersion;
					if (num != 0 && num == 1)
					{
						flag = DynDll.dl2_dlclose(handle);
					}
					else
					{
						flag = DynDll.dl_dlclose(handle);
					}
				}
				catch (DllNotFoundException obj) when (DynDll.dlVersion > 0)
				{
					DynDll.dlVersion--;
					continue;
				}
				break;
			}
			return flag;
		}

		private static IntPtr dlsym(IntPtr handle, string symbol)
		{
			IntPtr intPtr;
			for (;;)
			{
				try
				{
					int num = DynDll.dlVersion;
					if (num != 0 && num == 1)
					{
						intPtr = DynDll.dl2_dlsym(handle, symbol);
					}
					else
					{
						intPtr = DynDll.dl_dlsym(handle, symbol);
					}
				}
				catch (DllNotFoundException obj) when (DynDll.dlVersion > 0)
				{
					DynDll.dlVersion--;
					continue;
				}
				break;
			}
			return intPtr;
		}

		private static IntPtr dlerror()
		{
			IntPtr intPtr;
			for (;;)
			{
				try
				{
					int num = DynDll.dlVersion;
					if (num != 0 && num == 1)
					{
						intPtr = DynDll.dl2_dlerror();
					}
					else
					{
						intPtr = DynDll.dl_dlerror();
					}
				}
				catch (DllNotFoundException obj) when (DynDll.dlVersion > 0)
				{
					DynDll.dlVersion--;
					continue;
				}
				break;
			}
			return intPtr;
		}

		static DynDll()
		{
			if (!PlatformHelper.Is(Platform.Windows))
			{
				DynDll.dlerror();
			}
		}

		private static bool CheckError(out Exception exception)
		{
			if (PlatformHelper.Is(Platform.Windows))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 0)
				{
					exception = new Win32Exception(lastWin32Error);
					return false;
				}
			}
			else
			{
				IntPtr intPtr = DynDll.dlerror();
				if (intPtr != IntPtr.Zero)
				{
					exception = new Win32Exception(Marshal.PtrToStringAnsi(intPtr));
					return false;
				}
			}
			exception = null;
			return true;
		}

		public static IntPtr OpenLibrary(string name, bool skipMapping = false, int? flags = null)
		{
			IntPtr intPtr;
			if (!DynDll.InternalTryOpenLibrary(name, out intPtr, skipMapping, flags))
			{
				throw new DllNotFoundException("Unable to load library '" + name + "'");
			}
			Exception ex;
			if (!DynDll.CheckError(out ex))
			{
				throw ex;
			}
			return intPtr;
		}

		public static bool TryOpenLibrary(string name, out IntPtr libraryPtr, bool skipMapping = false, int? flags = null)
		{
			Exception ex;
			return DynDll.InternalTryOpenLibrary(name, out libraryPtr, skipMapping, flags) || DynDll.CheckError(out ex);
		}

		private static bool InternalTryOpenLibrary(string name, out IntPtr libraryPtr, bool skipMapping, int? flags)
		{
			List<DynDllMapping> list;
			if (name != null && !skipMapping && DynDll.Mappings.TryGetValue(name, out list))
			{
				foreach (DynDllMapping dynDllMapping in list)
				{
					if (DynDll.InternalTryOpenLibrary(dynDllMapping.LibraryName, out libraryPtr, true, dynDllMapping.Flags))
					{
						return true;
					}
				}
				libraryPtr = IntPtr.Zero;
				return true;
			}
			if (PlatformHelper.Is(Platform.Windows))
			{
				libraryPtr = ((name == null) ? DynDll.GetModuleHandle(name) : DynDll.LoadLibrary(name));
			}
			else
			{
				int num = flags ?? 258;
				libraryPtr = DynDll.dlopen(name, num);
				if (libraryPtr == IntPtr.Zero && File.Exists(name))
				{
					libraryPtr = DynDll.dlopen(Path.GetFullPath(name), num);
				}
			}
			return libraryPtr != IntPtr.Zero;
		}

		public static bool CloseLibrary(IntPtr lib)
		{
			if (PlatformHelper.Is(Platform.Windows))
			{
				DynDll.CloseLibrary(lib);
			}
			else
			{
				DynDll.dlclose(lib);
			}
			Exception ex;
			return DynDll.CheckError(out ex);
		}

		public static IntPtr GetFunction(this IntPtr libraryPtr, string name)
		{
			IntPtr intPtr;
			if (!DynDll.InternalTryGetFunction(libraryPtr, name, out intPtr))
			{
				throw new MissingMethodException("Unable to load function '" + name + "'");
			}
			Exception ex;
			if (!DynDll.CheckError(out ex))
			{
				throw ex;
			}
			return intPtr;
		}

		public static bool TryGetFunction(this IntPtr libraryPtr, string name, out IntPtr functionPtr)
		{
			Exception ex;
			return DynDll.InternalTryGetFunction(libraryPtr, name, out functionPtr) || DynDll.CheckError(out ex);
		}

		private static bool InternalTryGetFunction(IntPtr libraryPtr, string name, out IntPtr functionPtr)
		{
			if (libraryPtr == IntPtr.Zero)
			{
				throw new ArgumentNullException("libraryPtr");
			}
			functionPtr = (PlatformHelper.Is(Platform.Windows) ? DynDll.GetProcAddress(libraryPtr, name) : DynDll.dlsym(libraryPtr, name));
			return functionPtr != IntPtr.Zero;
		}

		public static T AsDelegate<T>(this IntPtr s) where T : class
		{
			return Marshal.GetDelegateForFunctionPointer(s, typeof(T)) as T;
		}

		public static void ResolveDynDllImports(this Type type, Dictionary<string, List<DynDllMapping>> mappings = null)
		{
			DynDll.InternalResolveDynDllImports(type, null, mappings);
		}

		public static void ResolveDynDllImports(object instance, Dictionary<string, List<DynDllMapping>> mappings = null)
		{
			DynDll.InternalResolveDynDllImports(instance.GetType(), instance, mappings);
		}

		private static void InternalResolveDynDllImports(Type type, object instance, Dictionary<string, List<DynDllMapping>> mappings)
		{
			BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
			if (instance == null)
			{
				bindingFlags |= BindingFlags.Static;
			}
			else
			{
				bindingFlags |= BindingFlags.Instance;
			}
			foreach (FieldInfo fieldInfo in type.GetFields(bindingFlags))
			{
				bool flag = true;
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(DynDllImportAttribute), true);
				int j = 0;
				while (j < customAttributes.Length)
				{
					DynDllImportAttribute dynDllImportAttribute = (DynDllImportAttribute)customAttributes[j];
					flag = false;
					IntPtr zero = IntPtr.Zero;
					List<DynDllMapping> list;
					if (mappings != null && mappings.TryGetValue(dynDllImportAttribute.LibraryName, out list))
					{
						bool flag2 = false;
						foreach (DynDllMapping dynDllMapping in list)
						{
							if (DynDll.TryOpenLibrary(dynDllMapping.LibraryName, out zero, true, dynDllMapping.Flags))
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							goto IL_00DC;
						}
					}
					else if (DynDll.TryOpenLibrary(dynDllImportAttribute.LibraryName, out zero, false, null))
					{
						goto IL_00DC;
					}
					IL_0158:
					j++;
					continue;
					IL_00DC:
					foreach (string text in dynDllImportAttribute.EntryPoints.Concat<string>(new string[]
					{
						fieldInfo.Name,
						fieldInfo.FieldType.Name
					}))
					{
						IntPtr intPtr;
						if (zero.TryGetFunction(text, out intPtr))
						{
							fieldInfo.SetValue(instance, Marshal.GetDelegateForFunctionPointer(intPtr, fieldInfo.FieldType));
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						goto IL_0158;
					}
					break;
				}
				if (!flag)
				{
					throw new EntryPointNotFoundException("No matching entry point found for " + fieldInfo.Name + " in " + fieldInfo.DeclaringType.FullName);
				}
			}
		}

		public static Dictionary<string, List<DynDllMapping>> Mappings = new Dictionary<string, List<DynDllMapping>>();

		private static int dlVersion = 1;

		public static class DlopenFlags
		{
			public const int RTLD_LAZY = 1;

			public const int RTLD_NOW = 2;

			public const int RTLD_LOCAL = 0;

			public const int RTLD_GLOBAL = 256;
		}
	}
}
