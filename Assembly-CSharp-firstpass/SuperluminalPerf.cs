using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

public static class SuperluminalPerf
{
	public static bool Enabled { get; set; } = true;

	public unsafe static void Initialize(string pathToPerformanceAPIDLL = null)
	{
		if (SuperluminalPerf._initialized)
		{
			return;
		}
		SuperluminalPerf._initialized = true;
		string text = ((IntPtr.Size == 8) ? "x64" : "x86");
		string text2 = pathToPerformanceAPIDLL ?? Path.Combine(new string[]
		{
			Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
			"Superluminal",
			"Performance",
			"API",
			"dll",
			text,
			"PerformanceAPI.dll"
		});
		IntPtr intPtr;
		if (!File.Exists(text2) || !SuperluminalPerf.NativeLibrary.TryLoad(text2, out intPtr))
		{
			return;
		}
		IntPtr intPtr2;
		if (SuperluminalPerf.NativeLibrary.TryGetExport(intPtr, "PerformanceAPI_GetAPI", out intPtr2))
		{
			delegate* unmanaged[Cdecl]<uint, SuperluminalPerf.PerformanceAPI_Functions*, uint> system.UInt32_u0020(System.UInt32,SuperluminalPerf/PerformanceAPI_Functions*) = (void*)intPtr2;
			delegate* unmanaged[Cdecl]<uint, SuperluminalPerf.PerformanceAPI_Functions*, uint> system.UInt32_u0020(System.UInt32,SuperluminalPerf/PerformanceAPI_Functions*)2 = system.UInt32_u0020(System.UInt32,SuperluminalPerf/PerformanceAPI_Functions*);
			SuperluminalPerf.PerformanceAPI_Functions performanceAPI_Functions;
			if (calli(System.UInt32(System.UInt32,SuperluminalPerf/PerformanceAPI_Functions*), 196608U, &performanceAPI_Functions, system.UInt32_u0020(System.UInt32,SuperluminalPerf/PerformanceAPI_Functions*)2) == 1U)
			{
				SuperluminalPerf._nativeSetCurrentThreadName = performanceAPI_Functions.SetCurrentThreadNameN;
				SuperluminalPerf._nativeBeginEvent = performanceAPI_Functions.BeginEventN;
				SuperluminalPerf._nativeBeginEventWide = performanceAPI_Functions.BeginEventWideN;
				SuperluminalPerf._nativeEndEvent = performanceAPI_Functions.EndEvent;
			}
		}
	}

	public unsafe static void SetCurrentThreadName(string name)
	{
		if (!SuperluminalPerf.Enabled || SuperluminalPerf._nativeSetCurrentThreadName == (UIntPtr)0)
		{
			return;
		}
		byte[] array;
		byte* ptr;
		if ((array = Encoding.UTF8.GetBytes(name)) == null || array.Length == 0)
		{
			ptr = null;
		}
		else
		{
			ptr = &array[0];
		}
		delegate* unmanaged[Cdecl]<byte*, ushort, void> nativeSetCurrentThreadName = SuperluminalPerf._nativeSetCurrentThreadName;
		calli(System.Void(System.Byte*,System.UInt16), ptr, (ushort)name.Length, nativeSetCurrentThreadName);
		array = null;
	}

	public static SuperluminalPerf.EventMarker BeginEvent(string eventId, string data = null)
	{
		return SuperluminalPerf.BeginEvent(eventId, data, SuperluminalPerf.ProfilerColor.Default);
	}

	public unsafe static SuperluminalPerf.EventMarker BeginEvent(string eventId, string data, SuperluminalPerf.ProfilerColor color)
	{
		if (SuperluminalPerf.Enabled && SuperluminalPerf._nativeBeginEventWide != (UIntPtr)0)
		{
			fixed (string text = eventId)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				fixed (string text2 = data)
				{
					char* ptr2 = text2;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					delegate* unmanaged[Cdecl]<char*, ushort, char*, ushort, uint, void> nativeBeginEventWide = SuperluminalPerf._nativeBeginEventWide;
					calli(System.Void(System.Char*,System.UInt16,System.Char*,System.UInt16,System.UInt32), ptr, (ushort)eventId.Length, ptr2, (data == null) ? 0 : ((ushort)data.Length), color.Value, nativeBeginEventWide);
				}
			}
		}
		return default(SuperluminalPerf.EventMarker);
	}

	public static void EndEvent()
	{
		if (SuperluminalPerf.Enabled && SuperluminalPerf._nativeEndEvent != (UIntPtr)0)
		{
			SuperluminalPerf.PerformanceAPI_SuppressTailCallOptimization performanceAPI_SuppressTailCallOptimization = calli(SuperluminalPerf/PerformanceAPI_SuppressTailCallOptimization(), SuperluminalPerf._nativeEndEvent);
		}
	}

	private unsafe static delegate* unmanaged[Cdecl]<byte*, ushort, void> _nativeSetCurrentThreadName;

	private unsafe static delegate* unmanaged[Cdecl]<byte*, ushort, byte*, ushort, uint, void> _nativeBeginEvent;

	private unsafe static delegate* unmanaged[Cdecl]<char*, ushort, char*, ushort, uint, void> _nativeBeginEventWide;

	private unsafe static delegate* unmanaged[Cdecl]<SuperluminalPerf.PerformanceAPI_SuppressTailCallOptimization> _nativeEndEvent;

	private static bool _initialized;

	public const uint Version = 196608U;

	public readonly struct EventMarker : IDisposable
	{
		public void Dispose()
		{
			SuperluminalPerf.EndEvent();
		}
	}

	public readonly struct ProfilerColor : IEquatable<SuperluminalPerf.ProfilerColor>
	{
		public ProfilerColor(byte r, byte g, byte b)
		{
			this.Value = (uint)(((int)r << 24) | ((int)g << 16) | ((int)b << 8) | 255);
		}

		public ProfilerColor(uint value)
		{
			this.Value = value;
		}

		public bool Equals(SuperluminalPerf.ProfilerColor other)
		{
			return this.Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			if (obj is SuperluminalPerf.ProfilerColor)
			{
				SuperluminalPerf.ProfilerColor profilerColor = (SuperluminalPerf.ProfilerColor)obj;
				return this.Equals(profilerColor);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)this.Value;
		}

		public static bool operator ==(SuperluminalPerf.ProfilerColor left, SuperluminalPerf.ProfilerColor right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(SuperluminalPerf.ProfilerColor left, SuperluminalPerf.ProfilerColor right)
		{
			return !left.Equals(right);
		}

		public override string ToString()
		{
			return string.Format("#{0:X8}", this.Value);
		}

		public static readonly SuperluminalPerf.ProfilerColor Default = new SuperluminalPerf.ProfilerColor(uint.MaxValue);

		public readonly uint Value;
	}

	private struct PerformanceAPI_SuppressTailCallOptimization
	{
		public long Value1;

		public long Value2;

		public long Value3;
	}

	private struct PerformanceAPI_Functions
	{
		public unsafe void* SetCurrentThreadName;

		public unsafe void* SetCurrentThreadNameN;

		public unsafe void* BeginEvent;

		public unsafe void* BeginEventN;

		public unsafe void* BeginEventWide;

		public unsafe void* BeginEventWideN;

		public unsafe void* EndEvent;

		public unsafe void* RegisterFiber;

		public unsafe void* UnregisterFiber;

		public unsafe void* BeginFiberSwitch;

		public unsafe void* EndFiberSwitch;
	}

	private static class NativeLibrary
	{
		public static bool TryLoad(string path, out IntPtr handle)
		{
			handle = SuperluminalPerf.NativeLibrary.LoadLibrary(path);
			return handle != IntPtr.Zero;
		}

		public static bool TryGetExport(IntPtr handle, string name, out IntPtr entryPtr)
		{
			entryPtr = SuperluminalPerf.NativeLibrary.GetProcAddress(handle, name);
			return entryPtr != IntPtr.Zero;
		}

		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern IntPtr LoadLibrary(string libraryName);

		[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
	}
}
