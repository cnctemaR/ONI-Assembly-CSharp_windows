using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoMod.Utils.Interop
{
	internal static class Windows
	{
		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("kernel32", ExactSpelling = true)]
		public unsafe static extern void GetSystemInfo(Windows.SYSTEM_INFO* lpSystemInfo);

		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("kernel32", ExactSpelling = true)]
		public unsafe static extern Windows.HMODULE GetModuleHandleW(ushort* lpModuleName);

		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("kernel32", ExactSpelling = true)]
		public unsafe static extern IntPtr GetProcAddress(Windows.HMODULE hModule, sbyte* lpProcName);

		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("kernel32", ExactSpelling = true)]
		public unsafe static extern Windows.HMODULE LoadLibraryW(ushort* lpLibFileName);

		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("kernel32", ExactSpelling = true)]
		public static extern Windows.BOOL FreeLibrary(Windows.HMODULE hLibModule);

		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		[DllImport("kernel32", ExactSpelling = true)]
		public static extern uint GetLastError();

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

		public readonly struct HMODULE : IComparable, IComparable<Windows.HMODULE>, IEquatable<Windows.HMODULE>, IFormattable
		{
			public unsafe HMODULE(void* value)
			{
				this.Value = value;
			}

			public static Windows.HMODULE INVALID_VALUE
			{
				get
				{
					return new Windows.HMODULE(-1);
				}
			}

			public static Windows.HMODULE NULL
			{
				get
				{
					return new Windows.HMODULE(null);
				}
			}

			public static bool operator ==(Windows.HMODULE left, Windows.HMODULE right)
			{
				return left.Value == right.Value;
			}

			public static bool operator !=(Windows.HMODULE left, Windows.HMODULE right)
			{
				return left.Value != right.Value;
			}

			public static bool operator <(Windows.HMODULE left, Windows.HMODULE right)
			{
				return left.Value < right.Value;
			}

			public static bool operator <=(Windows.HMODULE left, Windows.HMODULE right)
			{
				return left.Value == right.Value;
			}

			public static bool operator >(Windows.HMODULE left, Windows.HMODULE right)
			{
				return left.Value != right.Value;
			}

			public static bool operator >=(Windows.HMODULE left, Windows.HMODULE right)
			{
				return left.Value >= right.Value;
			}

			public unsafe static explicit operator Windows.HMODULE(void* value)
			{
				return new Windows.HMODULE(value);
			}

			public unsafe static implicit operator void*(Windows.HMODULE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HMODULE(Windows.HANDLE value)
			{
				return new Windows.HMODULE(value);
			}

			public static implicit operator Windows.HANDLE(Windows.HMODULE value)
			{
				return new Windows.HANDLE(value.Value);
			}

			public static explicit operator Windows.HMODULE(byte value)
			{
				return new Windows.HMODULE(value);
			}

			public static explicit operator byte(Windows.HMODULE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HMODULE(short value)
			{
				return new Windows.HMODULE(value);
			}

			public static explicit operator short(Windows.HMODULE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HMODULE(int value)
			{
				return new Windows.HMODULE(value);
			}

			public static explicit operator int(Windows.HMODULE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HMODULE(long value)
			{
				return new Windows.HMODULE(value);
			}

			public static explicit operator long(Windows.HMODULE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HMODULE([NativeInteger] IntPtr value)
			{
				return new Windows.HMODULE(value);
			}

			[return: NativeInteger]
			public static implicit operator IntPtr(Windows.HMODULE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HMODULE(sbyte value)
			{
				return new Windows.HMODULE(value);
			}

			public static explicit operator sbyte(Windows.HMODULE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HMODULE(ushort value)
			{
				return new Windows.HMODULE(value);
			}

			public static explicit operator ushort(Windows.HMODULE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HMODULE(uint value)
			{
				return new Windows.HMODULE(value);
			}

			public static explicit operator uint(Windows.HMODULE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HMODULE(ulong value)
			{
				return new Windows.HMODULE(value);
			}

			public static explicit operator ulong(Windows.HMODULE value)
			{
				return value.Value;
			}

			public static explicit operator Windows.HMODULE([NativeInteger] UIntPtr value)
			{
				return new Windows.HMODULE(value);
			}

			[return: NativeInteger]
			public static implicit operator UIntPtr(Windows.HMODULE value)
			{
				return value.Value;
			}

			[NullableContext(2)]
			public int CompareTo(object obj)
			{
				if (obj is Windows.HMODULE)
				{
					Windows.HMODULE hmodule = (Windows.HMODULE)obj;
					return this.CompareTo(hmodule);
				}
				if (obj != null)
				{
					throw new ArgumentException("obj is not an instance of HMODULE.");
				}
				return 1;
			}

			public int CompareTo(Windows.HMODULE other)
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
				if (obj is Windows.HMODULE)
				{
					Windows.HMODULE hmodule = (Windows.HMODULE)obj;
					return this.Equals(hmodule);
				}
				return false;
			}

			public bool Equals(Windows.HMODULE other)
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
	}
}
