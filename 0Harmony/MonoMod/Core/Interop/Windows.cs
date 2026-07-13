using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoMod.Utils;

namespace MonoMod.Core.Interop
{
	internal static class Windows
	{
		[DllImport("kernel32", ExactSpelling = true)]
		public unsafe static extern void* VirtualAlloc(void* lpAddress, [NativeInteger] UIntPtr dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32", ExactSpelling = true)]
		public unsafe static extern Windows.BOOL VirtualProtect(void* lpAddress, [NativeInteger] UIntPtr dwSize, uint flNewProtect, uint* lpflOldProtect);

		[DllImport("kernel32", ExactSpelling = true)]
		public unsafe static extern Windows.BOOL VirtualFree(void* lpAddress, [NativeInteger] UIntPtr dwSize, uint dwFreeType);

		[DllImport("kernel32", ExactSpelling = true)]
		[return: NativeInteger]
		public unsafe static extern UIntPtr VirtualQuery(void* lpAddress, Windows.MEMORY_BASIC_INFORMATION* lpBuffer, [NativeInteger] UIntPtr dwLength);

		[DllImport("kernel32", ExactSpelling = true)]
		public unsafe static extern void GetSystemInfo(Windows.SYSTEM_INFO* lpSystemInfo);

		[DllImport("kernel32", ExactSpelling = true)]
		public static extern Windows.HANDLE GetCurrentProcess();

		[DllImport("kernel32", ExactSpelling = true)]
		public unsafe static extern Windows.BOOL FlushInstructionCache(Windows.HANDLE hProcess, void* lpBaseAddress, [NativeInteger] UIntPtr dwSize);

		[DllImport("kernel32", ExactSpelling = true)]
		public static extern uint GetLastError();

		[DllImport("kernelbase", ExactSpelling = true)]
		private unsafe static extern Windows.BOOL SetProcessValidCallTargets(Windows.HANDLE hProcess, void* VirtualAddress, [NativeInteger] UIntPtr RegionSize, uint NumberOfOffsets, Windows.CFG_CALL_TARGET_INFO* OffsetInformation);

		public static bool HasSetProcessValidCallTargets
		{
			get
			{
				bool flag = Windows.hasSetProcessValidCallTargets.GetValueOrDefault();
				if (Windows.hasSetProcessValidCallTargets == null)
				{
					flag = Windows.<get_HasSetProcessValidCallTargets>g__DoGet|101_0();
					Windows.hasSetProcessValidCallTargets = new bool?(flag);
					return flag;
				}
				return flag;
			}
		}

		public unsafe static Windows.BOOL TrySetProcessValidCallTargets(void* VirtualAddress, [NativeInteger] UIntPtr RegionSize, uint NumberOfOffsets, Windows.CFG_CALL_TARGET_INFO* OffsetInformation)
		{
			if (Windows.HasSetProcessValidCallTargets)
			{
				return Windows.SetProcessValidCallTargets(Windows.GetCurrentProcess(), VirtualAddress, RegionSize, NumberOfOffsets, OffsetInformation);
			}
			return false;
		}

		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool <get_HasSetProcessValidCallTargets>g__DoGet|101_0()
		{
			IntPtr intPtr = DynDll.OpenLibrary("kernelbase");
			bool flag;
			try
			{
				IntPtr intPtr2;
				flag = intPtr.TryGetExport("SetProcessValidCallTargets", out intPtr2);
			}
			finally
			{
				DynDll.CloseLibrary(intPtr);
			}
			return flag;
		}

		public const int MEM_COMMIT = 4096;

		public const int MEM_RESERVE = 8192;

		public const int MEM_REPLACE_PLACEHOLDER = 16384;

		public const int MEM_RESERVE_PLACEHOLDER = 262144;

		public const int MEM_RESET = 524288;

		public const int MEM_TOP_DOWN = 1048576;

		public const int MEM_WRITE_WATCH = 2097152;

		public const int MEM_PHYSICAL = 4194304;

		public const int MEM_ROTATE = 8388608;

		public const int MEM_DIFFERENT_IMAGE_BASE_OK = 8388608;

		public const int MEM_RESET_UNDO = 16777216;

		public const int MEM_LARGE_PAGES = 536870912;

		public const uint MEM_4MB_PAGES = 2147483648U;

		public const int MEM_64K_PAGES = 541065216;

		public const int MEM_UNMAP_WITH_TRANSIENT_BOOST = 1;

		public const int MEM_COALESCE_PLACEHOLDERS = 1;

		public const int MEM_PRESERVE_PLACEHOLDER = 2;

		public const int MEM_DECOMMIT = 16384;

		public const int MEM_RELEASE = 32768;

		public const int MEM_FREE = 65536;

		public const int MEM_EXTENDED_PARAMETER_GRAPHICS = 1;

		public const int MEM_EXTENDED_PARAMETER_NONPAGED = 2;

		public const int MEM_EXTENDED_PARAMETER_ZERO_PAGES_OPTIONAL = 4;

		public const int MEM_EXTENDED_PARAMETER_NONPAGED_LARGE = 8;

		public const int MEM_EXTENDED_PARAMETER_NONPAGED_HUGE = 16;

		public const int MEM_EXTENDED_PARAMETER_SOFT_FAULT_PAGES = 32;

		public const int MEM_EXTENDED_PARAMETER_EC_CODE = 64;

		public const int MEM_EXTENDED_PARAMETER_IMAGE_NO_HPAT = 128;

		public const long MEM_EXTENDED_PARAMETER_NUMA_NODE_MANDATORY = -9223372036854775808L;

		public const int MEM_EXTENDED_PARAMETER_TYPE_BITS = 8;

		public const ulong MEM_DEDICATED_ATTRIBUTE_NOT_SPECIFIED = 18446744073709551615UL;

		public const int MEM_PRIVATE = 131072;

		public const int MEM_MAPPED = 262144;

		public const int MEM_IMAGE = 16777216;

		public const int PAGE_NOACCESS = 1;

		public const int PAGE_READONLY = 2;

		public const int PAGE_READWRITE = 4;

		public const int PAGE_WRITECOPY = 8;

		public const int PAGE_EXECUTE = 16;

		public const int PAGE_EXECUTE_READ = 32;

		public const int PAGE_EXECUTE_READWRITE = 64;

		public const int PAGE_EXECUTE_WRITECOPY = 128;

		public const int PAGE_GUARD = 256;

		public const int PAGE_NOCACHE = 512;

		public const int PAGE_WRITECOMBINE = 1024;

		public const int PAGE_GRAPHICS_NOACCESS = 2048;

		public const int PAGE_GRAPHICS_READONLY = 4096;

		public const int PAGE_GRAPHICS_READWRITE = 8192;

		public const int PAGE_GRAPHICS_EXECUTE = 16384;

		public const int PAGE_GRAPHICS_EXECUTE_READ = 32768;

		public const int PAGE_GRAPHICS_EXECUTE_READWRITE = 65536;

		public const int PAGE_GRAPHICS_COHERENT = 131072;

		public const int PAGE_GRAPHICS_NOCACHE = 262144;

		public const uint PAGE_ENCLAVE_THREAD_CONTROL = 2147483648U;

		public const uint PAGE_REVERT_TO_FILE_MAP = 2147483648U;

		public const int PAGE_TARGETS_NO_UPDATE = 1073741824;

		public const int PAGE_TARGETS_INVALID = 1073741824;

		public const int PAGE_ENCLAVE_UNVALIDATED = 536870912;

		public const int PAGE_ENCLAVE_MASK = 268435456;

		public const int PAGE_ENCLAVE_DECOMMIT = 268435456;

		public const int PAGE_ENCLAVE_SS_FIRST = 268435457;

		public const int PAGE_ENCLAVE_SS_REST = 268435458;

		public const int PROCESSOR_ARCHITECTURE_INTEL = 0;

		public const int PROCESSOR_ARCHITECTURE_MIPS = 1;

		public const int PROCESSOR_ARCHITECTURE_ALPHA = 2;

		public const int PROCESSOR_ARCHITECTURE_PPC = 3;

		public const int PROCESSOR_ARCHITECTURE_SHX = 4;

		public const int PROCESSOR_ARCHITECTURE_ARM = 5;

		public const int PROCESSOR_ARCHITECTURE_IA64 = 6;

		public const int PROCESSOR_ARCHITECTURE_ALPHA64 = 7;

		public const int PROCESSOR_ARCHITECTURE_MSIL = 8;

		public const int PROCESSOR_ARCHITECTURE_AMD64 = 9;

		public const int PROCESSOR_ARCHITECTURE_IA32_ON_WIN64 = 10;

		public const int PROCESSOR_ARCHITECTURE_NEUTRAL = 11;

		public const int PROCESSOR_ARCHITECTURE_ARM64 = 12;

		public const int PROCESSOR_ARCHITECTURE_ARM32_ON_WIN64 = 13;

		public const int PROCESSOR_ARCHITECTURE_IA32_ON_ARM64 = 14;

		public const int PROCESSOR_ARCHITECTURE_UNKNOWN = 65535;

		public const int CFG_CALL_TARGET_VALID = 1;

		public const int CFG_CALL_TARGET_PROCESSED = 2;

		public const int CFG_CALL_TARGET_CONVERT_EXPORT_SUPPRESSED_TO_VALID = 4;

		public const int CFG_CALL_TARGET_VALID_XFG = 8;

		public const int CFG_CALL_TARGET_CONVERT_XFG_TO_CFG = 16;

		private static bool? hasSetProcessValidCallTargets;

		[Conditional("NEVER")]
		[AttributeUsage(AttributeTargets.All)]
		private sealed class SetsLastSystemErrorAttribute : Attribute
		{
		}

		[Conditional("NEVER")]
		[AttributeUsage(AttributeTargets.All)]
		private sealed class NativeTypeNameAttribute : Attribute
		{
			[NullableContext(1)]
			public NativeTypeNameAttribute(string x)
			{
			}
		}

		public struct MEMORY_BASIC_INFORMATION
		{
			public unsafe void* BaseAddress;

			public unsafe void* AllocationBase;

			public uint AllocationProtect;

			[NativeInteger]
			public UIntPtr RegionSize;

			public uint State;

			public uint Protect;

			public uint Type;
		}

		public struct SYSTEM_INFO
		{
			[UnscopedRef]
			public ref uint dwOemId
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return ref this.Anonymous.dwOemId;
				}
			}

			[UnscopedRef]
			public ref ushort wProcessorArchitecture
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return ref this.Anonymous.Anonymous.wProcessorArchitecture;
				}
			}

			[UnscopedRef]
			public ref ushort wReserved
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return ref this.Anonymous.Anonymous.wReserved;
				}
			}

			public Windows.SYSTEM_INFO._Anonymous_e__Union Anonymous;

			public uint dwPageSize;

			public unsafe void* lpMinimumApplicationAddress;

			public unsafe void* lpMaximumApplicationAddress;

			[NativeInteger]
			public UIntPtr dwActiveProcessorMask;

			public uint dwNumberOfProcessors;

			public uint dwProcessorType;

			public uint dwAllocationGranularity;

			public ushort wProcessorLevel;

			public ushort wProcessorRevision;

			[StructLayout(LayoutKind.Explicit)]
			public struct _Anonymous_e__Union
			{
				[FieldOffset(0)]
				public uint dwOemId;

				[FieldOffset(0)]
				public Windows.SYSTEM_INFO._Anonymous_e__Union._Anonymous_e__Struct Anonymous;

				public struct _Anonymous_e__Struct
				{
					public ushort wProcessorArchitecture;

					public ushort wReserved;
				}
			}
		}

		public readonly struct BOOL : IComparable, IComparable<Windows.BOOL>, IEquatable<Windows.BOOL>, IFormattable
		{
			public BOOL(int value)
			{
				this.Value = value;
			}

			public static Windows.BOOL FALSE
			{
				get
				{
					return new Windows.BOOL(0);
				}
			}

			public static Windows.BOOL TRUE
			{
				get
				{
					return new Windows.BOOL(1);
				}
			}

			public static bool operator ==(Windows.BOOL left, Windows.BOOL right)
			{
				return left.Value == right.Value;
			}

			public static bool operator !=(Windows.BOOL left, Windows.BOOL right)
			{
				return left.Value != right.Value;
			}

			public static bool operator <(Windows.BOOL left, Windows.BOOL right)
			{
				return left.Value < right.Value;
			}

			public static bool operator <=(Windows.BOOL left, Windows.BOOL right)
			{
				return left.Value <= right.Value;
			}

			public static bool operator >(Windows.BOOL left, Windows.BOOL right)
			{
				return left.Value > right.Value;
			}

			public static bool operator >=(Windows.BOOL left, Windows.BOOL right)
			{
				return left.Value >= right.Value;
			}

			public static implicit operator bool(Windows.BOOL value)
			{
				return value.Value != 0;
			}

			public static implicit operator Windows.BOOL(bool value)
			{
				return new Windows.BOOL((value > false) ? 1 : 0);
			}

			public static bool operator false(Windows.BOOL value)
			{
				return value.Value == 0;
			}

			public static bool operator true(Windows.BOOL value)
			{
				return value.Value != 0;
			}

			public static implicit operator Windows.BOOL(byte value)
			{
				return new Windows.BOOL((int)value);
			}

			public static explicit operator byte(Windows.BOOL value)
			{
				return (byte)value.Value;
			}

			public static implicit operator Windows.BOOL(short value)
			{
				return new Windows.BOOL((int)value);
			}

			public static explicit operator short(Windows.BOOL value)
			{
				return (short)value.Value;
			}

			public static implicit operator Windows.BOOL(int value)
			{
				return new Windows.BOOL(value);
			}

			public static implicit operator int(Windows.BOOL value)
			{
				return value.Value;
			}

			public static explicit operator Windows.BOOL(long value)
			{
				return new Windows.BOOL((int)value);
			}

			public static implicit operator long(Windows.BOOL value)
			{
				return (long)value.Value;
			}

			public static explicit operator Windows.BOOL([NativeInteger] IntPtr value)
			{
				return new Windows.BOOL((int)value);
			}

			[return: NativeInteger]
			public static implicit operator IntPtr(Windows.BOOL value)
			{
				return (IntPtr)value.Value;
			}

			public static implicit operator Windows.BOOL(sbyte value)
			{
				return new Windows.BOOL((int)value);
			}

			public static explicit operator sbyte(Windows.BOOL value)
			{
				return (sbyte)value.Value;
			}

			public static implicit operator Windows.BOOL(ushort value)
			{
				return new Windows.BOOL((int)value);
			}

			public static explicit operator ushort(Windows.BOOL value)
			{
				return (ushort)value.Value;
			}

			public static explicit operator Windows.BOOL(uint value)
			{
				return new Windows.BOOL((int)value);
			}

			public static explicit operator uint(Windows.BOOL value)
			{
				return (uint)value.Value;
			}

			public static explicit operator Windows.BOOL(ulong value)
			{
				return new Windows.BOOL((int)value);
			}

			public static explicit operator ulong(Windows.BOOL value)
			{
				return (ulong)((long)value.Value);
			}

			public static explicit operator Windows.BOOL([NativeInteger] UIntPtr value)
			{
				return new Windows.BOOL((int)value);
			}

			[return: NativeInteger]
			public static explicit operator UIntPtr(Windows.BOOL value)
			{
				return (UIntPtr)((IntPtr)value.Value);
			}

			[NullableContext(2)]
			public int CompareTo(object obj)
			{
				if (obj is Windows.BOOL)
				{
					Windows.BOOL @bool = (Windows.BOOL)obj;
					return this.CompareTo(@bool);
				}
				if (obj != null)
				{
					throw new ArgumentException("obj is not an instance of BOOL.");
				}
				return 1;
			}

			public int CompareTo(Windows.BOOL other)
			{
				return this.Value.CompareTo(other.Value);
			}

			[NullableContext(2)]
			public override bool Equals(object obj)
			{
				if (obj is Windows.BOOL)
				{
					Windows.BOOL @bool = (Windows.BOOL)obj;
					return this.Equals(@bool);
				}
				return false;
			}

			public bool Equals(Windows.BOOL other)
			{
				return this.Value.Equals(other.Value);
			}

			public override int GetHashCode()
			{
				return this.Value.GetHashCode();
			}

			[NullableContext(1)]
			public override string ToString()
			{
				return this.Value.ToString(null);
			}

			[NullableContext(2)]
			[return: Nullable(1)]
			public string ToString(string format, IFormatProvider formatProvider)
			{
				return this.Value.ToString(format, formatProvider);
			}

			public readonly int Value;
		}

		public readonly struct HANDLE : IComparable, IComparable<Windows.HANDLE>, IEquatable<Windows.HANDLE>, IFormattable
		{
			public unsafe HANDLE(void* value)
			{
				this.Value = value;
			}

			public static Windows.HANDLE INVALID_VALUE
			{
				get
				{
					return new Windows.HANDLE(-1);
				}
			}

			public static Windows.HANDLE NULL
			{
				get
				{
					return new Windows.HANDLE(null);
				}
			}

			public static bool operator ==(Windows.HANDLE left, Windows.HANDLE right)
			{
				return left.Value == right.Value;
			}

			public static bool operator !=(Windows.HANDLE left, Windows.HANDLE right)
			{
				return left.Value != right.Value;
			}

			public static bool operator <(Windows.HANDLE left, Windows.HANDLE right)
			{
				return left.Value < right.Value;
			}

			public static bool operator <=(Windows.HANDLE left, Windows.HANDLE right)
			{
				return left.Value == right.Value;
			}

			public static bool operator >(Windows.HANDLE left, Windows.HANDLE right)
			{
				return left.Value != right.Value;
			}

			public static bool operator >=(Windows.HANDLE left, Windows.HANDLE right)
			{
				return left.Value >= right.Value;
			}

			public unsafe static explicit operator Windows.HANDLE(void* value)
			{
				return new Windows.HANDLE(value);
			}

			public unsafe static implicit operator void*(Windows.HANDLE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HANDLE(byte value)
			{
				return new Windows.HANDLE(value);
			}

			public static explicit operator byte(Windows.HANDLE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HANDLE(short value)
			{
				return new Windows.HANDLE(value);
			}

			public static explicit operator short(Windows.HANDLE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HANDLE(int value)
			{
				return new Windows.HANDLE(value);
			}

			public static explicit operator int(Windows.HANDLE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HANDLE(long value)
			{
				return new Windows.HANDLE(value);
			}

			public static explicit operator long(Windows.HANDLE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HANDLE([NativeInteger] IntPtr value)
			{
				return new Windows.HANDLE(value);
			}

			[return: NativeInteger]
			public static implicit operator IntPtr(Windows.HANDLE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HANDLE(sbyte value)
			{
				return new Windows.HANDLE(value);
			}

			public static explicit operator sbyte(Windows.HANDLE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HANDLE(ushort value)
			{
				return new Windows.HANDLE(value);
			}

			public static explicit operator ushort(Windows.HANDLE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HANDLE(uint value)
			{
				return new Windows.HANDLE(value);
			}

			public static explicit operator uint(Windows.HANDLE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HANDLE(ulong value)
			{
				return new Windows.HANDLE(value);
			}

			public static explicit operator ulong(Windows.HANDLE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HANDLE([NativeInteger] UIntPtr value)
			{
				return new Windows.HANDLE(value);
			}

			[return: NativeInteger]
			public static implicit operator UIntPtr(Windows.HANDLE value)
			{
				return value.Value;
			}

			[NullableContext(2)]
			public int CompareTo(object obj)
			{
				if (obj is Windows.HANDLE)
				{
					Windows.HANDLE handle = (Windows.HANDLE)obj;
					return this.CompareTo(handle);
				}
				if (obj != null)
				{
					throw new ArgumentException("obj is not an instance of HANDLE.");
				}
				return 1;
			}

			public int CompareTo(Windows.HANDLE other)
			{
				if (sizeof(IntPtr) != 4)
				{
					return this.Value.CompareTo(other.Value);
				}
				return this.Value.CompareTo(other.Value);
			}

			[NullableContext(2)]
			public override bool Equals(object obj)
			{
				if (obj is Windows.HANDLE)
				{
					Windows.HANDLE handle = (Windows.HANDLE)obj;
					return this.Equals(handle);
				}
				return false;
			}

			public bool Equals(Windows.HANDLE other)
			{
				return this.Value.Equals(other.Value);
			}

			public override int GetHashCode()
			{
				return this.Value.GetHashCode();
			}

			[NullableContext(1)]
			public override string ToString()
			{
				if (sizeof(UIntPtr) != 4)
				{
					return this.Value.ToString("X16", null);
				}
				return this.Value.ToString("X8", null);
			}

			[NullableContext(2)]
			[return: Nullable(1)]
			public string ToString(string format, IFormatProvider formatProvider)
			{
				if (sizeof(IntPtr) != 4)
				{
					return this.Value.ToString(format, formatProvider);
				}
				return this.Value.ToString(format, formatProvider);
			}

			public unsafe readonly void* Value;
		}

		public struct CFG_CALL_TARGET_INFO
		{
			[NativeInteger]
			public UIntPtr Offset;

			[NativeInteger]
			public UIntPtr Flags;
		}
	}
}
