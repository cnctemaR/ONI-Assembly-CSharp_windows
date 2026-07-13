using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Collections
{
	[BurstCompatible]
	internal struct Memory
	{
		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		internal static void CheckByteCountIsReasonable(long size)
		{
			if (size < 0L)
			{
				throw new InvalidOperationException("Attempted to operate on {size} bytes of memory: nonsensical");
			}
			if (size > 1099511627776L)
			{
				throw new InvalidOperationException("Attempted to operate on {size} bytes of memory: too big");
			}
		}

		internal const long k_MaximumRamSizeInBytes = 1099511627776L;

		[BurstCompatible]
		internal struct Unmanaged
		{
			internal unsafe static void* Allocate(long size, int align, AllocatorManager.AllocatorHandle allocator)
			{
				return Memory.Unmanaged.Array.Resize(null, 0L, 1L, allocator, size, align);
			}

			internal unsafe static void Free(void* pointer, AllocatorManager.AllocatorHandle allocator)
			{
				if (pointer == null)
				{
					return;
				}
				Memory.Unmanaged.Array.Resize(pointer, 1L, 0L, allocator, 1L, 1);
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			internal unsafe static T* Allocate<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
			{
				return Memory.Unmanaged.Array.Resize<T>(null, 0L, 1L, allocator);
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			internal unsafe static void Free<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* pointer, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
			{
				if (pointer == null)
				{
					return;
				}
				Memory.Unmanaged.Array.Resize<T>(pointer, 1L, 0L, allocator);
			}

			[BurstCompatible]
			internal struct Array
			{
				private static bool IsCustom(AllocatorManager.AllocatorHandle allocator)
				{
					return allocator.Index >= 64;
				}

				private unsafe static void* CustomResize(void* oldPointer, long oldCount, long newCount, AllocatorManager.AllocatorHandle allocator, long size, int align)
				{
					AllocatorManager.Block block = default(AllocatorManager.Block);
					block.Range.Allocator = allocator;
					block.Range.Items = (int)newCount;
					block.Range.Pointer = (IntPtr)oldPointer;
					block.BytesPerItem = (int)size;
					block.Alignment = align;
					block.AllocatedItems = (int)oldCount;
					AllocatorManager.Try(ref block);
					return (void*)block.Range.Pointer;
				}

				internal unsafe static void* Resize(void* oldPointer, long oldCount, long newCount, AllocatorManager.AllocatorHandle allocator, long size, int align)
				{
					int num = math.max(64, align);
					if (Memory.Unmanaged.Array.IsCustom(allocator))
					{
						return Memory.Unmanaged.Array.CustomResize(oldPointer, oldCount, newCount, allocator, size, num);
					}
					void* ptr = default(void*);
					if (newCount > 0L)
					{
						ptr = UnsafeUtility.Malloc(newCount * size, num, allocator.ToAllocator);
						if (oldCount > 0L)
						{
							long num2 = math.min(oldCount, newCount) * size;
							UnsafeUtility.MemCpy(ptr, oldPointer, num2);
						}
					}
					if (oldCount > 0L)
					{
						UnsafeUtility.Free(oldPointer, allocator.ToAllocator);
					}
					return ptr;
				}

				[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
				internal unsafe static T* Resize<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* oldPointer, long oldCount, long newCount, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
				{
					return (T*)Memory.Unmanaged.Array.Resize((void*)oldPointer, oldCount, newCount, allocator, (long)UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>());
				}

				[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
				internal unsafe static T* Allocate<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(long count, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
				{
					return Memory.Unmanaged.Array.Resize<T>(null, 0L, count, allocator);
				}

				[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
				internal unsafe static void Free<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* pointer, long count, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
				{
					if (pointer == null)
					{
						return;
					}
					Memory.Unmanaged.Array.Resize<T>(pointer, count, 0L, allocator);
				}
			}
		}

		[BurstCompatible]
		internal struct Array
		{
			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			internal unsafe static void Set<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* pointer, long count, T t = default(T)) where T : struct, ValueType
			{
				UnsafeUtility.SizeOf<T>();
				int num = 0;
				while ((long)num < count)
				{
					pointer[(IntPtr)num * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = t;
					num++;
				}
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			internal unsafe static void Clear<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* pointer, long count) where T : struct, ValueType
			{
				long num = count * (long)UnsafeUtility.SizeOf<T>();
				UnsafeUtility.MemClear((void*)pointer, num);
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			internal unsafe static void Copy<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* dest, T* src, long count) where T : struct, ValueType
			{
				long num = count * (long)UnsafeUtility.SizeOf<T>();
				UnsafeUtility.MemCpy((void*)dest, (void*)src, num);
			}
		}
	}
}
