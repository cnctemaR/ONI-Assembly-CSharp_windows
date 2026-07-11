using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Harmony.ILCopying
{
	public static class Memory
	{
		public static bool IsWindows
		{
			get
			{
				return Memory.WindowsPlatformIDSet.Contains(Environment.OSVersion.Platform);
			}
		}

		[DllImport("kernel32.dll")]
		public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, Protection flNewProtect, out Protection lpflOldProtect);

		public static void UnprotectMemoryPage(long memory)
		{
			bool isWindows = Memory.IsWindows;
			if (isWindows)
			{
				Protection protection;
				bool flag = Memory.VirtualProtect(new IntPtr(memory), new UIntPtr(1U), Protection.PAGE_EXECUTE_READWRITE, out protection);
				bool flag2 = !flag;
				if (flag2)
				{
					throw new Win32Exception();
				}
			}
		}

		public static string DetourMethod(MethodBase original, MethodBase replacement)
		{
			Exception ex;
			long methodStart = Memory.GetMethodStart(original, out ex);
			bool flag = methodStart == 0L;
			string text;
			if (flag)
			{
				text = ex.Message;
			}
			else
			{
				long methodStart2 = Memory.GetMethodStart(replacement, out ex);
				bool flag2 = methodStart2 == 0L;
				if (flag2)
				{
					text = ex.Message;
				}
				else
				{
					text = Memory.WriteJump(methodStart, methodStart2);
				}
			}
			return text;
		}

		public static string WriteJump(long memory, long destination)
		{
			Memory.UnprotectMemoryPage(memory);
			bool flag = IntPtr.Size == 8;
			if (flag)
			{
				bool flag2 = Memory.CompareBytes(memory, new byte[] { 233 });
				if (flag2)
				{
					int num = Memory.ReadInt(memory + 1L);
					memory += (long)(5 + num);
				}
				memory = Memory.WriteBytes(memory, new byte[] { 72, 184 });
				memory = Memory.WriteLong(memory, destination);
				memory = Memory.WriteBytes(memory, new byte[] { byte.MaxValue, 224 });
			}
			else
			{
				memory = Memory.WriteByte(memory, 104);
				memory = Memory.WriteInt(memory, (int)destination);
				memory = Memory.WriteByte(memory, 195);
			}
			return null;
		}

		private static RuntimeMethodHandle GetRuntimeMethodHandle(MethodBase method)
		{
			bool flag = method is DynamicMethod;
			RuntimeMethodHandle runtimeMethodHandle;
			if (flag)
			{
				BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
				MethodInfo method2 = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", bindingFlags);
				bool flag2 = method2 != null;
				if (flag2)
				{
					runtimeMethodHandle = (RuntimeMethodHandle)method2.Invoke(method, new object[0]);
				}
				else
				{
					FieldInfo field = typeof(DynamicMethod).GetField("m_method", bindingFlags);
					bool flag3 = field != null;
					if (flag3)
					{
						runtimeMethodHandle = (RuntimeMethodHandle)field.GetValue(method);
					}
					else
					{
						FieldInfo field2 = typeof(DynamicMethod).GetField("mhandle", bindingFlags);
						runtimeMethodHandle = (RuntimeMethodHandle)field2.GetValue(method);
					}
				}
			}
			else
			{
				runtimeMethodHandle = method.MethodHandle;
			}
			return runtimeMethodHandle;
		}

		public static long GetMethodStart(MethodBase method, out Exception exception)
		{
			RuntimeMethodHandle runtimeMethodHandle = Memory.GetRuntimeMethodHandle(method);
			try
			{
				RuntimeHelpers.PrepareMethod(runtimeMethodHandle);
			}
			catch (Exception)
			{
			}
			long num;
			try
			{
				exception = null;
				num = runtimeMethodHandle.GetFunctionPointer().ToInt64();
			}
			catch (Exception ex)
			{
				exception = ex;
				num = 0L;
			}
			return num;
		}

		public unsafe static bool CompareBytes(long memory, byte[] values)
		{
			byte* ptr = memory;
			foreach (byte b in values)
			{
				bool flag = b != *ptr;
				if (flag)
				{
					return false;
				}
				ptr++;
			}
			return true;
		}

		public unsafe static byte ReadByte(long memory)
		{
			byte* ptr = memory;
			return *ptr;
		}

		public unsafe static int ReadInt(long memory)
		{
			int* ptr = memory;
			return *ptr;
		}

		public unsafe static long ReadLong(long memory)
		{
			long* ptr = memory;
			return *ptr;
		}

		public unsafe static long WriteByte(long memory, byte value)
		{
			byte* ptr = memory;
			*ptr = value;
			return memory + 1L;
		}

		public static long WriteBytes(long memory, byte[] values)
		{
			foreach (byte b in values)
			{
				memory = Memory.WriteByte(memory, b);
			}
			return memory;
		}

		public unsafe static long WriteInt(long memory, int value)
		{
			int* ptr = memory;
			*ptr = value;
			return memory + 4L;
		}

		public unsafe static long WriteLong(long memory, long value)
		{
			long* ptr = memory;
			*ptr = value;
			return memory + 8L;
		}

		private static readonly HashSet<PlatformID> WindowsPlatformIDSet = new HashSet<PlatformID>
		{
			PlatformID.Win32NT,
			PlatformID.Win32S,
			PlatformID.Win32Windows,
			PlatformID.WinCE
		};
	}
}
