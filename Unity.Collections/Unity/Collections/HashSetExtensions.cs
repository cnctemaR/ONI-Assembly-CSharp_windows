using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	public static class HashSetExtensions
	{
		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList128Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList128Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList128Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList32Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList32Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList32Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList4096Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList4096Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList4096Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList512Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList512Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList512Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList64Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList64Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, FixedList64Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList128Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList128Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList128Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList32Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList32Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList32Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList4096Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList4096Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList4096Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList512Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList512Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList512Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList64Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList64Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, FixedList64Bytes<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeParallelHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeParallelHashSet<T>.ReadOnly other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}

		public static void ExceptWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Remove(t);
			}
		}

		public static void IntersectWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeList<T> unsafeList = new UnsafeList<T>(container.Count(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			foreach (T t in other)
			{
				if (container.Contains(t))
				{
					unsafeList.Add(in t);
				}
			}
			container.Clear();
			(ref container).UnionWith<T>(unsafeList);
			unsafeList.Dispose();
		}

		public static void UnionWith<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			foreach (T t in other)
			{
				container.Add(t);
			}
		}
	}
}
