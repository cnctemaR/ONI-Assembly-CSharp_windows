using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Hashing/Hash128.bindings.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Utilities/Hash128.h")]
	[Serializable]
	public struct Hash128 : IComparable, IComparable<Hash128>, IEquatable<Hash128>
	{
		public Hash128(uint u32_0, uint u32_1, uint u32_2, uint u32_3)
		{
			this.u64_0 = ((ulong)u32_1 << 32) | (ulong)u32_0;
			this.u64_1 = ((ulong)u32_3 << 32) | (ulong)u32_2;
		}

		public Hash128(ulong u64_0, ulong u64_1)
		{
			this.u64_0 = u64_0;
			this.u64_1 = u64_1;
		}

		public bool isValid
		{
			get
			{
				return this.u64_0 != 0UL || this.u64_1 > 0UL;
			}
		}

		public int CompareTo(Hash128 rhs)
		{
			bool flag = this < rhs;
			int num;
			if (flag)
			{
				num = -1;
			}
			else
			{
				bool flag2 = this > rhs;
				if (flag2)
				{
					num = 1;
				}
				else
				{
					num = 0;
				}
			}
			return num;
		}

		public override string ToString()
		{
			return Hash128.Hash128ToStringImpl(this);
		}

		[FreeFunction("StringToHash128", IsThreadSafe = true)]
		public unsafe static Hash128 Parse(string hashString)
		{
			Hash128 hash2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(hashString, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = hashString.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Hash128 hash;
				Hash128.Parse_Injected(ref managedSpanWrapper, out hash);
			}
			finally
			{
				char* ptr = null;
				Hash128 hash;
				hash2 = hash;
			}
			return hash2;
		}

		[FreeFunction("Hash128ToString", IsThreadSafe = true)]
		private static string Hash128ToStringImpl(Hash128 hash)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				Hash128.Hash128ToStringImpl_Injected(ref hash, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[FreeFunction("ComputeHash128FromScriptString", IsThreadSafe = true)]
		private unsafe static void ComputeFromString(string data, ref Hash128 hash)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(data, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = data.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Hash128.ComputeFromString_Injected(ref managedSpanWrapper, ref hash);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("ComputeHash128FromScriptPointer", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ComputeFromPtr(IntPtr data, int start, int count, int elemSize, ref Hash128 hash);

		[FreeFunction("ComputeHash128FromScriptArray", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ComputeFromArray(Array data, int start, int count, int elemSize, ref Hash128 hash);

		public static Hash128 Compute(string data)
		{
			Hash128 hash = default(Hash128);
			Hash128.ComputeFromString(data, ref hash);
			return hash;
		}

		public static Hash128 Compute<T>(NativeArray<T> data) where T : struct
		{
			Hash128 hash = default(Hash128);
			Hash128.ComputeFromPtr((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), 0, data.Length, UnsafeUtility.SizeOf<T>(), ref hash);
			return hash;
		}

		public static Hash128 Compute<T>(NativeArray<T> data, int start, int count) where T : struct
		{
			bool flag = start < 0 || count < 0 || start + count > data.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (start:{0} count:{1})", start, count));
			}
			Hash128 hash = default(Hash128);
			Hash128.ComputeFromPtr((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), start, count, UnsafeUtility.SizeOf<T>(), ref hash);
			return hash;
		}

		public static Hash128 Compute<T>(T[] data) where T : struct
		{
			bool flag = !UnsafeUtility.IsArrayBlittable(data);
			if (flag)
			{
				throw new ArgumentException("Array passed to Compute must be blittable.\n" + UnsafeUtility.GetReasonForArrayNonBlittable(data));
			}
			Hash128 hash = default(Hash128);
			Hash128.ComputeFromArray(data, 0, data.Length, UnsafeUtility.SizeOf<T>(), ref hash);
			return hash;
		}

		public static Hash128 Compute<T>(T[] data, int start, int count) where T : struct
		{
			bool flag = !UnsafeUtility.IsArrayBlittable(data);
			if (flag)
			{
				throw new ArgumentException("Array passed to Compute must be blittable.\n" + UnsafeUtility.GetReasonForArrayNonBlittable(data));
			}
			bool flag2 = start < 0 || count < 0 || start + count > data.Length;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (start:{0} count:{1})", start, count));
			}
			Hash128 hash = default(Hash128);
			Hash128.ComputeFromArray(data, start, count, UnsafeUtility.SizeOf<T>(), ref hash);
			return hash;
		}

		public static Hash128 Compute<T>(List<T> data) where T : struct
		{
			bool flag = !UnsafeUtility.IsGenericListBlittable<T>();
			if (flag)
			{
				throw new ArgumentException(string.Format("List<{0}> passed to {1} must be blittable.\n{2}", typeof(T), "Compute", UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			Hash128 hash = default(Hash128);
			Hash128.ComputeFromArray(NoAllocHelpers.ExtractArrayFromList<T>(data), 0, data.Count, UnsafeUtility.SizeOf<T>(), ref hash);
			return hash;
		}

		public static Hash128 Compute<T>(List<T> data, int start, int count) where T : struct
		{
			bool flag = !UnsafeUtility.IsGenericListBlittable<T>();
			if (flag)
			{
				throw new ArgumentException(string.Format("List<{0}> passed to {1} must be blittable.\n{2}", typeof(T), "Compute", UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			bool flag2 = start < 0 || count < 0 || start + count > data.Count;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (start:{0} count:{1})", start, count));
			}
			Hash128 hash = default(Hash128);
			Hash128.ComputeFromArray(NoAllocHelpers.ExtractArrayFromList<T>(data), start, count, UnsafeUtility.SizeOf<T>(), ref hash);
			return hash;
		}

		public unsafe static Hash128 Compute<[IsUnmanaged] T>(ref T val) where T : struct, ValueType
		{
			fixed (T* ptr = &val)
			{
				void* ptr2 = (void*)ptr;
				Hash128 hash = default(Hash128);
				Hash128.ComputeFromPtr((IntPtr)ptr2, 0, 1, UnsafeUtility.SizeOf<T>(), ref hash);
				return hash;
			}
		}

		public static Hash128 Compute(int val)
		{
			Hash128 hash = default(Hash128);
			hash.Append(val);
			return hash;
		}

		public static Hash128 Compute(float val)
		{
			Hash128 hash = default(Hash128);
			hash.Append(val);
			return hash;
		}

		public unsafe static Hash128 Compute(void* data, ulong size)
		{
			Hash128 hash = default(Hash128);
			Hash128.ComputeFromPtr(new IntPtr(data), 0, (int)size, 1, ref hash);
			return hash;
		}

		public void Append(string data)
		{
			Hash128.ComputeFromString(data, ref this);
		}

		public void Append<T>(NativeArray<T> data) where T : struct
		{
			Hash128.ComputeFromPtr((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), 0, data.Length, UnsafeUtility.SizeOf<T>(), ref this);
		}

		public void Append<T>(NativeArray<T> data, int start, int count) where T : struct
		{
			bool flag = start < 0 || count < 0 || start + count > data.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (start:{0} count:{1})", start, count));
			}
			Hash128.ComputeFromPtr((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), start, count, UnsafeUtility.SizeOf<T>(), ref this);
		}

		public void Append<T>(T[] data) where T : struct
		{
			bool flag = !UnsafeUtility.IsArrayBlittable(data);
			if (flag)
			{
				throw new ArgumentException("Array passed to Append must be blittable.\n" + UnsafeUtility.GetReasonForArrayNonBlittable(data));
			}
			Hash128.ComputeFromArray(data, 0, data.Length, UnsafeUtility.SizeOf<T>(), ref this);
		}

		public void Append<T>(T[] data, int start, int count) where T : struct
		{
			bool flag = !UnsafeUtility.IsArrayBlittable(data);
			if (flag)
			{
				throw new ArgumentException("Array passed to Append must be blittable.\n" + UnsafeUtility.GetReasonForArrayNonBlittable(data));
			}
			bool flag2 = start < 0 || count < 0 || start + count > data.Length;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (start:{0} count:{1})", start, count));
			}
			Hash128.ComputeFromArray(data, start, count, UnsafeUtility.SizeOf<T>(), ref this);
		}

		public void Append<T>(List<T> data) where T : struct
		{
			bool flag = !UnsafeUtility.IsGenericListBlittable<T>();
			if (flag)
			{
				throw new ArgumentException(string.Format("List<{0}> passed to {1} must be blittable.\n{2}", typeof(T), "Append", UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			Hash128.ComputeFromArray(NoAllocHelpers.ExtractArrayFromList<T>(data), 0, data.Count, UnsafeUtility.SizeOf<T>(), ref this);
		}

		public void Append<T>(List<T> data, int start, int count) where T : struct
		{
			bool flag = !UnsafeUtility.IsGenericListBlittable<T>();
			if (flag)
			{
				throw new ArgumentException(string.Format("List<{0}> passed to {1} must be blittable.\n{2}", typeof(T), "Append", UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			bool flag2 = start < 0 || count < 0 || start + count > data.Count;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (start:{0} count:{1})", start, count));
			}
			Hash128.ComputeFromArray(NoAllocHelpers.ExtractArrayFromList<T>(data), start, count, UnsafeUtility.SizeOf<T>(), ref this);
		}

		public unsafe void Append<[IsUnmanaged] T>(ref T val) where T : struct, ValueType
		{
			fixed (T* ptr = &val)
			{
				void* ptr2 = (void*)ptr;
				Hash128.ComputeFromPtr((IntPtr)ptr2, 0, 1, UnsafeUtility.SizeOf<T>(), ref this);
			}
		}

		public void Append(int val)
		{
			this.ShortHash4((uint)val);
		}

		public unsafe void Append(float val)
		{
			this.ShortHash4(*(uint*)(&val));
		}

		public unsafe void Append(void* data, ulong size)
		{
			Hash128.ComputeFromPtr(new IntPtr(data), 0, (int)size, 1, ref this);
		}

		public override bool Equals(object obj)
		{
			return obj is Hash128 && this == (Hash128)obj;
		}

		public bool Equals(Hash128 obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return this.u64_0.GetHashCode() ^ this.u64_1.GetHashCode();
		}

		public int CompareTo(object obj)
		{
			bool flag = obj == null || !(obj is Hash128);
			int num;
			if (flag)
			{
				num = 1;
			}
			else
			{
				Hash128 hash = (Hash128)obj;
				num = this.CompareTo(hash);
			}
			return num;
		}

		public static bool operator ==(Hash128 hash1, Hash128 hash2)
		{
			return hash1.u64_0 == hash2.u64_0 && hash1.u64_1 == hash2.u64_1;
		}

		public static bool operator !=(Hash128 hash1, Hash128 hash2)
		{
			return !(hash1 == hash2);
		}

		public static bool operator <(Hash128 x, Hash128 y)
		{
			bool flag = x.u64_0 != y.u64_0;
			bool flag2;
			if (flag)
			{
				flag2 = x.u64_0 < y.u64_0;
			}
			else
			{
				flag2 = x.u64_1 < y.u64_1;
			}
			return flag2;
		}

		public static bool operator >(Hash128 x, Hash128 y)
		{
			bool flag = x < y;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = x == y;
				flag2 = !flag3;
			}
			return flag2;
		}

		private void ShortHash4(uint data)
		{
			ulong num = this.u64_0;
			ulong num2 = this.u64_1;
			ulong num3 = 16045690984833335023UL;
			ulong num4 = 16045690984833335023UL;
			num4 += 288230376151711744UL;
			num3 += (ulong)data;
			Hash128.ShortEnd(ref num, ref num2, ref num3, ref num4);
			this.u64_0 = num;
			this.u64_1 = num2;
		}

		private static void ShortEnd(ref ulong h0, ref ulong h1, ref ulong h2, ref ulong h3)
		{
			h3 ^= h2;
			Hash128.Rot64(ref h2, 15);
			h3 += h2;
			h0 ^= h3;
			Hash128.Rot64(ref h3, 52);
			h0 += h3;
			h1 ^= h0;
			Hash128.Rot64(ref h0, 26);
			h1 += h0;
			h2 ^= h1;
			Hash128.Rot64(ref h1, 51);
			h2 += h1;
			h3 ^= h2;
			Hash128.Rot64(ref h2, 28);
			h3 += h2;
			h0 ^= h3;
			Hash128.Rot64(ref h3, 9);
			h0 += h3;
			h1 ^= h0;
			Hash128.Rot64(ref h0, 47);
			h1 += h0;
			h2 ^= h1;
			Hash128.Rot64(ref h1, 54);
			h2 += h1;
			h3 ^= h2;
			Hash128.Rot64(ref h2, 32);
			h3 += h2;
			h0 ^= h3;
			Hash128.Rot64(ref h3, 25);
			h0 += h3;
			h1 ^= h0;
			Hash128.Rot64(ref h0, 63);
			h1 += h0;
		}

		private static void Rot64(ref ulong x, int k)
		{
			x = (x << k) | (x >> 64 - k);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Parse_Injected(ref ManagedSpanWrapper hashString, out Hash128 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Hash128ToStringImpl_Injected([In] ref Hash128 hash, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ComputeFromString_Injected(ref ManagedSpanWrapper data, ref Hash128 hash);

		internal ulong u64_0;

		internal ulong u64_1;

		private const ulong kConst = 16045690984833335023UL;
	}
}
