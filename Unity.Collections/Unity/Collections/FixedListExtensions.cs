using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	public static class FixedListExtensions
	{
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this FixedList32Bytes<T> list) where T : struct, ValueType, IComparable<T>
		{
			NativeSortExtension.Sort<T>((T*)(list.buffer + FixedList.PaddingBytes<T>()), list.Length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this FixedList32Bytes<T> list, U comp) where T : struct, ValueType, IComparable<T> where U : IComparer<T>
		{
			NativeSortExtension.Sort<T, U>((T*)(list.buffer + FixedList.PaddingBytes<T>()), list.Length, comp);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this FixedList64Bytes<T> list) where T : struct, ValueType, IComparable<T>
		{
			NativeSortExtension.Sort<T>((T*)(list.buffer + FixedList.PaddingBytes<T>()), list.Length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this FixedList64Bytes<T> list, U comp) where T : struct, ValueType, IComparable<T> where U : IComparer<T>
		{
			NativeSortExtension.Sort<T, U>((T*)(list.buffer + FixedList.PaddingBytes<T>()), list.Length, comp);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this FixedList128Bytes<T> list) where T : struct, ValueType, IComparable<T>
		{
			NativeSortExtension.Sort<T>((T*)(list.buffer + FixedList.PaddingBytes<T>()), list.Length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this FixedList128Bytes<T> list, U comp) where T : struct, ValueType, IComparable<T> where U : IComparer<T>
		{
			NativeSortExtension.Sort<T, U>((T*)(list.buffer + FixedList.PaddingBytes<T>()), list.Length, comp);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this FixedList512Bytes<T> list) where T : struct, ValueType, IComparable<T>
		{
			NativeSortExtension.Sort<T>((T*)(list.buffer + FixedList.PaddingBytes<T>()), list.Length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this FixedList512Bytes<T> list, U comp) where T : struct, ValueType, IComparable<T> where U : IComparer<T>
		{
			NativeSortExtension.Sort<T, U>((T*)(list.buffer + FixedList.PaddingBytes<T>()), list.Length, comp);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this FixedList4096Bytes<T> list) where T : struct, ValueType, IComparable<T>
		{
			NativeSortExtension.Sort<T>((T*)(list.buffer + FixedList.PaddingBytes<T>()), list.Length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this FixedList4096Bytes<T> list, U comp) where T : struct, ValueType, IComparable<T> where U : IComparer<T>
		{
			NativeSortExtension.Sort<T, U>((T*)(list.buffer + FixedList.PaddingBytes<T>()), list.Length, comp);
		}
	}
}
