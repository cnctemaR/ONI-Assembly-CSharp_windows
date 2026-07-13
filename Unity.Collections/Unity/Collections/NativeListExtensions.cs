using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	public static class NativeListExtensions
	{
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static bool Contains<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeList<T> list, U value) where T : struct, ValueType, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>((void*)list.GetUnsafeReadOnlyPtr<T>(), list.Length, value) != -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static int IndexOf<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeList<T> list, U value) where T : struct, ValueType, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>((void*)list.GetUnsafeReadOnlyPtr<T>(), list.Length, value);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static bool ArraysEqual<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeArray<T> container, in NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			NativeList<T> nativeList = other;
			return container.ArraysEqual<T>(nativeList.AsArray());
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static bool ArraysEqual<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> container, in NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			return other.ArraysEqual<T>(in container);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static bool ArraysEqual<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> container, in NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			NativeArray<T> nativeArray = container.AsArray();
			NativeList<T> nativeList = other;
			return nativeArray.ArraysEqual<T>(nativeList.AsArray());
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static bool ArraysEqual<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> container, in UnsafeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			return (*container.m_ListData).ArraysEqual<T>(in other);
		}
	}
}
