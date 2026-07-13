using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Collections
{
	public static class AllocatorManager
	{
		internal static AllocatorManager.Block AllocateBlock<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this T t, int sizeOf, int alignOf, int items) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			AllocatorManager.Block block = default(AllocatorManager.Block);
			block.Range.Pointer = IntPtr.Zero;
			block.Range.Items = items;
			block.Range.Allocator = t.Handle;
			block.BytesPerItem = sizeOf;
			block.Alignment = math.max(64, alignOf);
			t.Try(ref block);
			return block;
		}

		internal static AllocatorManager.Block AllocateBlock<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(this T t, U u, int items) where T : struct, ValueType, AllocatorManager.IAllocator where U : struct, ValueType
		{
			return (ref t).AllocateBlock<T>(UnsafeUtility.SizeOf<U>(), UnsafeUtility.AlignOf<U>(), items);
		}

		public unsafe static void* Allocate<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this T t, int sizeOf, int alignOf, int items = 1) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			return (void*)(ref t).AllocateBlock<T>(sizeOf, alignOf, items).Range.Pointer;
		}

		internal unsafe static U* Allocate<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(this T t, U u, int items) where T : struct, ValueType, AllocatorManager.IAllocator where U : struct, ValueType
		{
			return (U*)(ref t).Allocate<T>(UnsafeUtility.SizeOf<U>(), UnsafeUtility.AlignOf<U>(), items);
		}

		internal unsafe static void* AllocateStruct<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(this T t, U u, int items) where T : struct, ValueType, AllocatorManager.IAllocator where U : struct, ValueType
		{
			return (ref t).Allocate<T>(UnsafeUtility.SizeOf<U>(), UnsafeUtility.AlignOf<U>(), items);
		}

		internal static void FreeBlock<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this T t, ref AllocatorManager.Block block) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			block.Range.Items = 0;
			t.Try(ref block);
		}

		internal unsafe static void Free<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this T t, void* pointer, int sizeOf, int alignOf, int items) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			if (pointer == null)
			{
				return;
			}
			AllocatorManager.Block block = default(AllocatorManager.Block);
			block.AllocatedItems = items;
			block.Range.Pointer = (IntPtr)pointer;
			block.BytesPerItem = sizeOf;
			block.Alignment = alignOf;
			(ref t).FreeBlock<T>(ref block);
		}

		internal unsafe static void Free<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(this T t, U* pointer, int items) where T : struct, ValueType, AllocatorManager.IAllocator where U : struct, ValueType
		{
			(ref t).Free<T>((void*)pointer, UnsafeUtility.SizeOf<U>(), UnsafeUtility.AlignOf<U>(), items);
		}

		public unsafe static void* Allocate(AllocatorManager.AllocatorHandle handle, int itemSizeInBytes, int alignmentInBytes, int items = 1)
		{
			return (ref handle).Allocate<AllocatorManager.AllocatorHandle>(itemSizeInBytes, alignmentInBytes, items);
		}

		public unsafe static T* Allocate<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(AllocatorManager.AllocatorHandle handle, int items = 1) where T : struct, ValueType
		{
			return (ref handle).Allocate(default(T), items);
		}

		public unsafe static void Free(AllocatorManager.AllocatorHandle handle, void* pointer, int itemSizeInBytes, int alignmentInBytes, int items = 1)
		{
			(ref handle).Free<AllocatorManager.AllocatorHandle>(pointer, itemSizeInBytes, alignmentInBytes, items);
		}

		public unsafe static void Free(AllocatorManager.AllocatorHandle handle, void* pointer)
		{
			(ref handle).Free<AllocatorManager.AllocatorHandle, byte>((byte*)pointer, 1);
		}

		public unsafe static void Free<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(AllocatorManager.AllocatorHandle handle, T* pointer, int items = 1) where T : struct, ValueType
		{
			(ref handle).Free<AllocatorManager.AllocatorHandle, T>(pointer, items);
		}

		public static AllocatorManager.AllocatorHandle ConvertToAllocatorHandle(Allocator a)
		{
			ushort num = (ushort)(a & (Allocator)65535);
			ushort num2 = (ushort)(a >> 16);
			return new AllocatorManager.AllocatorHandle
			{
				Index = num,
				Version = num2
			};
		}

		[BurstDiscard]
		private static void CheckDelegate(ref bool useDelegate)
		{
			useDelegate = true;
		}

		private static bool UseDelegate()
		{
			bool flag = false;
			AllocatorManager.CheckDelegate(ref flag);
			return flag;
		}

		private unsafe static int allocate_block(ref AllocatorManager.Block block)
		{
			AllocatorManager.TableEntry tableEntry = default(AllocatorManager.TableEntry);
			tableEntry = *block.Range.Allocator.TableEntry;
			FunctionPointer<AllocatorManager.TryFunction> functionPointer = new FunctionPointer<AllocatorManager.TryFunction>(tableEntry.function);
			return functionPointer.Invoke(tableEntry.state, ref block);
		}

		[BurstDiscard]
		private unsafe static void forward_mono_allocate_block(ref AllocatorManager.Block block, ref int error)
		{
			AllocatorManager.TableEntry tableEntry = default(AllocatorManager.TableEntry);
			tableEntry = *block.Range.Allocator.TableEntry;
			if (block.Range.Allocator.Handle.Index >= 32768)
			{
				throw new ArgumentException("Allocator index into TryFunction delegate table exceeds maximum.");
			}
			ref AllocatorManager.TryFunction ptr = ref AllocatorManager.Managed.TryFunctionDelegates[(int)block.Range.Allocator.Handle.Index];
			error = ptr(tableEntry.state, ref block);
		}

		internal static Allocator LegacyOf(AllocatorManager.AllocatorHandle handle)
		{
			if (handle.Value >= 64)
			{
				return Allocator.Persistent;
			}
			return (Allocator)handle.Value;
		}

		private unsafe static int TryLegacy(ref AllocatorManager.Block block)
		{
			if (block.Range.Pointer == IntPtr.Zero)
			{
				block.Range.Pointer = (IntPtr)Memory.Unmanaged.Allocate(block.Bytes, block.Alignment, AllocatorManager.LegacyOf(block.Range.Allocator));
				block.AllocatedItems = block.Range.Items;
				if (!(block.Range.Pointer == IntPtr.Zero))
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (block.Bytes == 0L)
				{
					if (AllocatorManager.LegacyOf(block.Range.Allocator) != Allocator.None)
					{
						Memory.Unmanaged.Free((void*)block.Range.Pointer, AllocatorManager.LegacyOf(block.Range.Allocator));
					}
					block.Range.Pointer = IntPtr.Zero;
					block.AllocatedItems = 0;
					return 0;
				}
				return -1;
			}
		}

		public unsafe static int Try(ref AllocatorManager.Block block)
		{
			if (block.Range.Allocator.Value < 64)
			{
				return AllocatorManager.TryLegacy(ref block);
			}
			AllocatorManager.TableEntry tableEntry = default(AllocatorManager.TableEntry);
			tableEntry = *block.Range.Allocator.TableEntry;
			new FunctionPointer<AllocatorManager.TryFunction>(tableEntry.function);
			if (AllocatorManager.UseDelegate())
			{
				int num = 0;
				AllocatorManager.forward_mono_allocate_block(ref block, ref num);
				return num;
			}
			return AllocatorManager.allocate_block(ref block);
		}

		public static void Initialize()
		{
		}

		internal static void Install(AllocatorManager.AllocatorHandle handle, IntPtr allocatorState, FunctionPointer<AllocatorManager.TryFunction> functionPointer, AllocatorManager.TryFunction function, bool IsAutoDispose = false)
		{
			if (functionPointer.Value == IntPtr.Zero)
			{
				(ref handle).Unregister<AllocatorManager.AllocatorHandle>();
				return;
			}
			if (ConcurrentMask.Succeeded(ConcurrentMask.TryAllocate<Long1024>(AllocatorManager.SharedStatics.IsInstalled.Ref.Data, handle.Value, 1)))
			{
				handle.Install(new AllocatorManager.TableEntry
				{
					state = allocatorState,
					function = functionPointer.Value
				});
				AllocatorManager.Managed.RegisterDelegate((int)handle.Index, function);
				if (IsAutoDispose)
				{
					ConcurrentMask.TryAllocate<Long1024>(AllocatorManager.SharedStatics.IsAutoDispose.Ref.Data, handle.Value, 1);
				}
			}
		}

		internal static void Install(AllocatorManager.AllocatorHandle handle, IntPtr allocatorState, AllocatorManager.TryFunction function)
		{
			FunctionPointer<AllocatorManager.TryFunction> functionPointer = ((function == null) ? new FunctionPointer<AllocatorManager.TryFunction>(IntPtr.Zero) : BurstCompiler.CompileFunctionPointer<AllocatorManager.TryFunction>(function));
			AllocatorManager.Install(handle, allocatorState, functionPointer, function, false);
		}

		internal static AllocatorManager.AllocatorHandle Register(IntPtr allocatorState, FunctionPointer<AllocatorManager.TryFunction> functionPointer, bool IsAutoDispose = false, bool isGlobal = false, int globalIndex = 0)
		{
			int num;
			int num2;
			if (isGlobal)
			{
				if ((long)globalIndex < (long)((ulong)AllocatorManager.GlobalAllocatorBaseIndex))
				{
					throw new ArgumentException(string.Format("Error: {0} is less than GlobalAllocatorBaseIndex", globalIndex));
				}
				num = ConcurrentMask.TryAllocate<Long1024>(AllocatorManager.SharedStatics.IsInstalled.Ref.Data, globalIndex, 1);
				num2 = globalIndex;
			}
			else
			{
				num = ConcurrentMask.TryAllocate<Long1024>(AllocatorManager.SharedStatics.IsInstalled.Ref.Data, out num2, 1, (int)(AllocatorManager.GlobalAllocatorBaseIndex - 1U), 1);
			}
			AllocatorManager.TableEntry tableEntry = new AllocatorManager.TableEntry
			{
				state = allocatorState,
				function = functionPointer.Value
			};
			AllocatorManager.AllocatorHandle allocatorHandle = default(AllocatorManager.AllocatorHandle);
			if (ConcurrentMask.Succeeded(num))
			{
				allocatorHandle.Index = (ushort)num2;
				allocatorHandle.Install(tableEntry);
				if (IsAutoDispose)
				{
					ConcurrentMask.TryAllocate<Long1024>(AllocatorManager.SharedStatics.IsAutoDispose.Ref.Data, num2, 1);
				}
			}
			return allocatorHandle;
		}

		[ExcludeFromBurstCompatTesting("Uses managed delegate")]
		public static void Register<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this T t, bool IsAutoDispose = false, bool isGlobal = false, int globalIndex = 0) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			AllocatorManager.TryFunction function = t.Function;
			FunctionPointer<AllocatorManager.TryFunction> tryFunction;
			if (function == null)
			{
				tryFunction = new FunctionPointer<AllocatorManager.TryFunction>(IntPtr.Zero);
			}
			else
			{
				if (function != AllocatorManager.AllocatorCache<T>.CachedFunction)
				{
					AllocatorManager.AllocatorCache<T>.TryFunction = BurstCompiler.CompileFunctionPointer<AllocatorManager.TryFunction>(function);
					AllocatorManager.AllocatorCache<T>.CachedFunction = function;
				}
				tryFunction = AllocatorManager.AllocatorCache<T>.TryFunction;
			}
			t.Handle = AllocatorManager.Register((IntPtr)UnsafeUtility.AddressOf<T>(ref t), tryFunction, IsAutoDispose, isGlobal, globalIndex);
			AllocatorManager.Managed.RegisterDelegate((int)t.Handle.Index, t.Function);
		}

		public static void UnmanagedUnregister<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this T t) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			if (t.Handle.IsInstalled)
			{
				t.Handle.Install(default(AllocatorManager.TableEntry));
				ConcurrentMask.TryFree<Long1024>(AllocatorManager.SharedStatics.IsInstalled.Ref.Data, t.Handle.Value, 1);
				ConcurrentMask.TryFree<Long1024>(AllocatorManager.SharedStatics.IsAutoDispose.Ref.Data, t.Handle.Value, 1);
			}
		}

		[ExcludeFromBurstCompatTesting("Uses managed delegate")]
		public static void Unregister<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this T t) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			if (t.Handle.IsInstalled)
			{
				t.Handle.Dispose();
				ConcurrentMask.TryFree<Long1024>(AllocatorManager.SharedStatics.IsInstalled.Ref.Data, t.Handle.Value, 1);
				ConcurrentMask.TryFree<Long1024>(AllocatorManager.SharedStatics.IsAutoDispose.Ref.Data, t.Handle.Value, 1);
				AllocatorManager.Managed.UnregisterDelegate((int)t.Handle.Index);
			}
		}

		[ExcludeFromBurstCompatTesting("Register uses managed delegate")]
		internal unsafe static ref T CreateAllocator<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(AllocatorManager.AllocatorHandle backingAllocator, bool isGlobal = false, int globalIndex = 0) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			T* ptr = (T*)Memory.Unmanaged.Allocate((long)UnsafeUtility.SizeOf<T>(), 16, backingAllocator);
			*ptr = default(T);
			ref T ptr2 = ref UnsafeUtility.AsRef<T>((void*)ptr);
			(ref ptr2).Register<T>(ptr->IsAutoDispose, isGlobal, globalIndex);
			return ref ptr2;
		}

		[ExcludeFromBurstCompatTesting("Registration uses managed delegates")]
		internal static void DestroyAllocator<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this T t, AllocatorManager.AllocatorHandle backingAllocator) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			(ref t).Unregister<T>();
			Memory.Unmanaged.Free(UnsafeUtility.AddressOf<T>(ref t), backingAllocator);
		}

		public static void Shutdown()
		{
		}

		internal static bool IsCustomAllocator(AllocatorManager.AllocatorHandle allocator)
		{
			return allocator.Index >= 64;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal static void CheckFailedToAllocate(int error)
		{
			if (error != 0)
			{
				throw new ArgumentException("failed to allocate");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal static void CheckFailedToFree(int error)
		{
			if (error != 0)
			{
				throw new ArgumentException("failed to free");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal static void CheckValid(AllocatorManager.AllocatorHandle handle)
		{
		}

		public static readonly AllocatorManager.AllocatorHandle Invalid = new AllocatorManager.AllocatorHandle
		{
			Index = 0
		};

		public static readonly AllocatorManager.AllocatorHandle None = new AllocatorManager.AllocatorHandle
		{
			Index = 1
		};

		public static readonly AllocatorManager.AllocatorHandle Temp = new AllocatorManager.AllocatorHandle
		{
			Index = 2
		};

		public static readonly AllocatorManager.AllocatorHandle TempJob = new AllocatorManager.AllocatorHandle
		{
			Index = 3
		};

		public static readonly AllocatorManager.AllocatorHandle Persistent = new AllocatorManager.AllocatorHandle
		{
			Index = 4
		};

		public static readonly AllocatorManager.AllocatorHandle AudioKernel = new AllocatorManager.AllocatorHandle
		{
			Index = 5
		};

		public const int kErrorNone = 0;

		public const int kErrorBufferOverflow = -1;

		public const ushort FirstUserIndex = 64;

		public const ushort MaxNumCustomAllocators = 32768;

		internal static readonly ushort NumGlobalScratchAllocators = (ushort)JobsUtility.ThreadIndexCount;

		internal static readonly ushort MaxNumGlobalAllocators = (ushort)JobsUtility.ThreadIndexCount;

		internal static readonly uint GlobalAllocatorBaseIndex = (uint)(32768 - AllocatorManager.MaxNumGlobalAllocators);

		internal static readonly uint FirstGlobalScratchpadAllocatorIndex = AllocatorManager.GlobalAllocatorBaseIndex;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int TryFunction(IntPtr allocatorState, ref AllocatorManager.Block block);

		public struct AllocatorHandle : AllocatorManager.IAllocator, IDisposable, IEquatable<AllocatorManager.AllocatorHandle>, IComparable<AllocatorManager.AllocatorHandle>
		{
			internal ref AllocatorManager.TableEntry TableEntry
			{
				get
				{
					return AllocatorManager.SharedStatics.TableEntry.Ref.Data.ElementAt((int)this.Index);
				}
			}

			internal unsafe bool IsInstalled
			{
				get
				{
					return ((*AllocatorManager.SharedStatics.IsInstalled.Ref.Data.ElementAt(this.Index >> 6) >> (int)this.Index) & 1L) != 0L;
				}
			}

			internal void IncrementVersion()
			{
			}

			internal void Rewind()
			{
			}

			internal unsafe void Install(AllocatorManager.TableEntry tableEntry)
			{
				this.Rewind();
				*this.TableEntry = tableEntry;
			}

			public static implicit operator AllocatorManager.AllocatorHandle(Allocator a)
			{
				return new AllocatorManager.AllocatorHandle
				{
					Index = (ushort)(a & (Allocator)65535),
					Version = 0
				};
			}

			public int Value
			{
				get
				{
					return (int)this.Index;
				}
			}

			public int TryAllocateBlock<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(out AllocatorManager.Block block, int items) where T : struct, ValueType
			{
				block = new AllocatorManager.Block
				{
					Range = new AllocatorManager.Range
					{
						Items = items,
						Allocator = this
					},
					BytesPerItem = UnsafeUtility.SizeOf<T>(),
					Alignment = 1 << math.min(3, math.tzcnt(UnsafeUtility.SizeOf<T>()))
				};
				return this.Try(ref block);
			}

			public AllocatorManager.Block AllocateBlock<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(int items) where T : struct, ValueType
			{
				AllocatorManager.Block block;
				this.TryAllocateBlock<T>(out block, items);
				return block;
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private static void CheckAllocatedSuccessfully(int error)
			{
				if (error != 0)
				{
					throw new ArgumentException(string.Format("Error {0}: Failed to Allocate", error));
				}
			}

			public AllocatorManager.TryFunction Function
			{
				get
				{
					return null;
				}
			}

			public int Try(ref AllocatorManager.Block block)
			{
				block.Range.Allocator = this;
				return AllocatorManager.Try(ref block);
			}

			public AllocatorManager.AllocatorHandle Handle
			{
				get
				{
					return this;
				}
				set
				{
					this = value;
				}
			}

			public Allocator ToAllocator
			{
				get
				{
					uint index = (uint)this.Index;
					return (Allocator)(((int)this.Version << 16) | (int)index);
				}
			}

			public bool IsCustomAllocator
			{
				get
				{
					return this.Index >= 64;
				}
			}

			public unsafe bool IsAutoDispose
			{
				get
				{
					return ((*AllocatorManager.SharedStatics.IsAutoDispose.Ref.Data.ElementAt(this.Index >> 6) >> (int)this.Index) & 1L) != 0L;
				}
			}

			public unsafe void Dispose()
			{
				this.Rewind();
				*this.TableEntry = default(AllocatorManager.TableEntry);
			}

			public override bool Equals(object obj)
			{
				if (obj is AllocatorManager.AllocatorHandle)
				{
					return this.Value == ((AllocatorManager.AllocatorHandle)obj).Value;
				}
				return obj is Allocator && this.ToAllocator == (Allocator)obj;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool Equals(AllocatorManager.AllocatorHandle other)
			{
				return this.Value == other.Value;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool Equals(Allocator other)
			{
				return this.ToAllocator == other;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public override int GetHashCode()
			{
				return this.Value;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool operator ==(AllocatorManager.AllocatorHandle lhs, AllocatorManager.AllocatorHandle rhs)
			{
				return lhs.Value == rhs.Value;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool operator !=(AllocatorManager.AllocatorHandle lhs, AllocatorManager.AllocatorHandle rhs)
			{
				return lhs.Value != rhs.Value;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool operator <(AllocatorManager.AllocatorHandle lhs, AllocatorManager.AllocatorHandle rhs)
			{
				return lhs.Value < rhs.Value;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool operator >(AllocatorManager.AllocatorHandle lhs, AllocatorManager.AllocatorHandle rhs)
			{
				return lhs.Value > rhs.Value;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool operator <=(AllocatorManager.AllocatorHandle lhs, AllocatorManager.AllocatorHandle rhs)
			{
				return lhs.Value <= rhs.Value;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool operator >=(AllocatorManager.AllocatorHandle lhs, AllocatorManager.AllocatorHandle rhs)
			{
				return lhs.Value >= rhs.Value;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int CompareTo(AllocatorManager.AllocatorHandle other)
			{
				return this.Value - other.Value;
			}

			public ushort Index;

			public ushort Version;
		}

		public struct BlockHandle
		{
			public ushort Value;
		}

		public struct Range : IDisposable
		{
			public void Dispose()
			{
				AllocatorManager.Block block = new AllocatorManager.Block
				{
					Range = this
				};
				block.Dispose();
				this = block.Range;
			}

			public IntPtr Pointer;

			public int Items;

			public AllocatorManager.AllocatorHandle Allocator;
		}

		public struct Block : IDisposable
		{
			public long Bytes
			{
				get
				{
					return (long)this.BytesPerItem * (long)this.Range.Items;
				}
			}

			public long AllocatedBytes
			{
				get
				{
					return (long)this.BytesPerItem * (long)this.AllocatedItems;
				}
			}

			public int Alignment
			{
				get
				{
					return 1 << (int)this.Log2Alignment;
				}
				set
				{
					this.Log2Alignment = (byte)(32 - math.lzcnt(math.max(1, value) - 1));
				}
			}

			public void Dispose()
			{
				this.TryFree();
			}

			public int TryAllocate()
			{
				this.Range.Pointer = IntPtr.Zero;
				return AllocatorManager.Try(ref this);
			}

			public int TryFree()
			{
				this.Range.Items = 0;
				return AllocatorManager.Try(ref this);
			}

			public void Allocate()
			{
				this.TryAllocate();
			}

			public void Free()
			{
				this.TryFree();
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private void CheckFailedToAllocate(int error)
			{
				if (error != 0)
				{
					throw new ArgumentException(string.Format("Error {0}: Failed to Allocate {1}", error, this));
				}
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private void CheckFailedToFree(int error)
			{
				if (error != 0)
				{
					throw new ArgumentException(string.Format("Error {0}: Failed to Free {1}", error, this));
				}
			}

			public AllocatorManager.Range Range;

			public int BytesPerItem;

			public int AllocatedItems;

			public byte Log2Alignment;

			public byte Padding0;

			public ushort Padding1;

			public uint Padding2;
		}

		public interface IAllocator : IDisposable
		{
			AllocatorManager.TryFunction Function { get; }

			int Try(ref AllocatorManager.Block block);

			AllocatorManager.AllocatorHandle Handle { get; set; }

			Allocator ToAllocator { get; }

			bool IsCustomAllocator { get; }

			bool IsAutoDispose
			{
				get
				{
					return false;
				}
			}
		}

		[BurstCompile]
		internal struct StackAllocator : AllocatorManager.IAllocator, IDisposable
		{
			public AllocatorManager.AllocatorHandle Handle
			{
				get
				{
					return this.m_handle;
				}
				set
				{
					this.m_handle = value;
				}
			}

			public Allocator ToAllocator
			{
				get
				{
					return this.m_handle.ToAllocator;
				}
			}

			public bool IsCustomAllocator
			{
				get
				{
					return this.m_handle.IsCustomAllocator;
				}
			}

			public void Initialize(AllocatorManager.Block storage)
			{
				this.m_storage = storage;
				this.m_top = 0L;
			}

			public unsafe int Try(ref AllocatorManager.Block block)
			{
				if (block.Range.Pointer == IntPtr.Zero)
				{
					if (this.m_top + block.Bytes > this.m_storage.Bytes)
					{
						return -1;
					}
					block.Range.Pointer = (IntPtr)((void*)((byte*)(void*)this.m_storage.Range.Pointer + this.m_top));
					block.AllocatedItems = block.Range.Items;
					this.m_top += block.Bytes;
					return 0;
				}
				else
				{
					if (block.Bytes != 0L)
					{
						return -1;
					}
					if ((long)((byte*)(void*)block.Range.Pointer - (byte*)(void*)this.m_storage.Range.Pointer) == this.m_top - block.AllocatedBytes)
					{
						this.m_top -= block.AllocatedBytes;
						block.Range.Pointer = IntPtr.Zero;
						block.AllocatedItems = 0;
						return 0;
					}
					return -1;
				}
			}

			[BurstCompile]
			[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
			public static int Try(IntPtr allocatorState, ref AllocatorManager.Block block)
			{
				return AllocatorManager.StackAllocator.Try_000000AB$BurstDirectCall.Invoke(allocatorState, ref block);
			}

			public AllocatorManager.TryFunction Function
			{
				get
				{
					return new AllocatorManager.TryFunction(AllocatorManager.StackAllocator.Try);
				}
			}

			public void Dispose()
			{
				this.m_handle.Rewind();
			}

			[BurstCompile]
			[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal unsafe static int Try$BurstManaged(IntPtr allocatorState, ref AllocatorManager.Block block)
			{
				return ((AllocatorManager.StackAllocator*)(void*)allocatorState)->Try(ref block);
			}

			internal AllocatorManager.AllocatorHandle m_handle;

			internal AllocatorManager.Block m_storage;

			internal long m_top;

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int Try_000000AB$PostfixBurstDelegate(IntPtr allocatorState, ref AllocatorManager.Block block);

			internal static class Try_000000AB$BurstDirectCall
			{
				[BurstDiscard]
				private static void GetFunctionPointerDiscard(ref IntPtr A_0)
				{
					if (AllocatorManager.StackAllocator.Try_000000AB$BurstDirectCall.Pointer == 0)
					{
						AllocatorManager.StackAllocator.Try_000000AB$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<AllocatorManager.StackAllocator.Try_000000AB$PostfixBurstDelegate>(new AllocatorManager.StackAllocator.Try_000000AB$PostfixBurstDelegate(AllocatorManager.StackAllocator.Try)).Value;
					}
					A_0 = AllocatorManager.StackAllocator.Try_000000AB$BurstDirectCall.Pointer;
				}

				private static IntPtr GetFunctionPointer()
				{
					IntPtr intPtr = (IntPtr)0;
					AllocatorManager.StackAllocator.Try_000000AB$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
					return intPtr;
				}

				public static int Invoke(IntPtr allocatorState, ref AllocatorManager.Block block)
				{
					if (BurstCompiler.IsEnabled)
					{
						IntPtr functionPointer = AllocatorManager.StackAllocator.Try_000000AB$BurstDirectCall.GetFunctionPointer();
						if (functionPointer != 0)
						{
							return calli(System.Int32(System.IntPtr,Unity.Collections.AllocatorManager/Block&), allocatorState, ref block, functionPointer);
						}
					}
					return AllocatorManager.StackAllocator.Try$BurstManaged(allocatorState, ref block);
				}

				private static IntPtr Pointer;
			}
		}

		[BurstCompile]
		internal struct SlabAllocator : AllocatorManager.IAllocator, IDisposable
		{
			public AllocatorManager.AllocatorHandle Handle
			{
				get
				{
					return this.m_handle;
				}
				set
				{
					this.m_handle = value;
				}
			}

			public Allocator ToAllocator
			{
				get
				{
					return this.m_handle.ToAllocator;
				}
			}

			public bool IsCustomAllocator
			{
				get
				{
					return this.m_handle.IsCustomAllocator;
				}
			}

			public long BudgetInBytes
			{
				get
				{
					return this.budgetInBytes;
				}
			}

			public long AllocatedBytes
			{
				get
				{
					return this.allocatedBytes;
				}
			}

			internal int SlabSizeInBytes
			{
				get
				{
					return 1 << this.Log2SlabSizeInBytes;
				}
				set
				{
					this.Log2SlabSizeInBytes = (int)((byte)(32 - math.lzcnt(math.max(1, value) - 1)));
				}
			}

			internal int Slabs
			{
				get
				{
					return (int)(this.Storage.Bytes >> this.Log2SlabSizeInBytes);
				}
			}

			internal void Initialize(AllocatorManager.Block storage, int slabSizeInBytes, long budget)
			{
				this.Storage = storage;
				this.Log2SlabSizeInBytes = 0;
				this.Occupied = default(FixedList4096Bytes<int>);
				this.budgetInBytes = budget;
				this.allocatedBytes = 0L;
				this.SlabSizeInBytes = slabSizeInBytes;
				this.Occupied.Length = (this.Slabs + 31) / 32;
			}

			public int Try(ref AllocatorManager.Block block)
			{
				if (block.Range.Pointer == IntPtr.Zero)
				{
					if (block.Bytes + this.allocatedBytes > this.budgetInBytes)
					{
						return -2;
					}
					if (block.Bytes > (long)this.SlabSizeInBytes)
					{
						return -1;
					}
					for (int i = 0; i < this.Occupied.Length; i++)
					{
						int num = this.Occupied[i];
						if (num != -1)
						{
							for (int j = 0; j < 32; j++)
							{
								if ((num & (1 << j)) == 0)
								{
									ref FixedList4096Bytes<int> ptr = ref this.Occupied;
									int num2 = i;
									ptr[num2] |= 1 << j;
									block.Range.Pointer = this.Storage.Range.Pointer + (int)((long)this.SlabSizeInBytes * ((long)i * 32L + (long)j));
									block.AllocatedItems = this.SlabSizeInBytes / block.BytesPerItem;
									this.allocatedBytes += block.Bytes;
									return 0;
								}
							}
						}
					}
					return -1;
				}
				else
				{
					if (block.Bytes == 0L)
					{
						ulong num3 = (ulong)((long)block.Range.Pointer - (long)this.Storage.Range.Pointer) >> this.Log2SlabSizeInBytes;
						int num4 = (int)(num3 >> 5);
						int num5 = (int)(num3 & 31UL);
						ref FixedList4096Bytes<int> ptr = ref this.Occupied;
						int num2 = num4;
						ptr[num2] &= ~(1 << num5);
						block.Range.Pointer = IntPtr.Zero;
						int num6 = block.AllocatedItems * block.BytesPerItem;
						this.allocatedBytes -= (long)num6;
						block.AllocatedItems = 0;
						return 0;
					}
					return -1;
				}
			}

			[BurstCompile]
			[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
			public static int Try(IntPtr allocatorState, ref AllocatorManager.Block block)
			{
				return AllocatorManager.SlabAllocator.Try_000000B9$BurstDirectCall.Invoke(allocatorState, ref block);
			}

			public AllocatorManager.TryFunction Function
			{
				get
				{
					return new AllocatorManager.TryFunction(AllocatorManager.SlabAllocator.Try);
				}
			}

			public void Dispose()
			{
				this.m_handle.Rewind();
			}

			[BurstCompile]
			[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal unsafe static int Try$BurstManaged(IntPtr allocatorState, ref AllocatorManager.Block block)
			{
				return ((AllocatorManager.SlabAllocator*)(void*)allocatorState)->Try(ref block);
			}

			internal AllocatorManager.AllocatorHandle m_handle;

			internal AllocatorManager.Block Storage;

			internal int Log2SlabSizeInBytes;

			internal FixedList4096Bytes<int> Occupied;

			internal long budgetInBytes;

			internal long allocatedBytes;

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate int Try_000000B9$PostfixBurstDelegate(IntPtr allocatorState, ref AllocatorManager.Block block);

			internal static class Try_000000B9$BurstDirectCall
			{
				[BurstDiscard]
				private static void GetFunctionPointerDiscard(ref IntPtr A_0)
				{
					if (AllocatorManager.SlabAllocator.Try_000000B9$BurstDirectCall.Pointer == 0)
					{
						AllocatorManager.SlabAllocator.Try_000000B9$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<AllocatorManager.SlabAllocator.Try_000000B9$PostfixBurstDelegate>(new AllocatorManager.SlabAllocator.Try_000000B9$PostfixBurstDelegate(AllocatorManager.SlabAllocator.Try)).Value;
					}
					A_0 = AllocatorManager.SlabAllocator.Try_000000B9$BurstDirectCall.Pointer;
				}

				private static IntPtr GetFunctionPointer()
				{
					IntPtr intPtr = (IntPtr)0;
					AllocatorManager.SlabAllocator.Try_000000B9$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
					return intPtr;
				}

				public static int Invoke(IntPtr allocatorState, ref AllocatorManager.Block block)
				{
					if (BurstCompiler.IsEnabled)
					{
						IntPtr functionPointer = AllocatorManager.SlabAllocator.Try_000000B9$BurstDirectCall.GetFunctionPointer();
						if (functionPointer != 0)
						{
							return calli(System.Int32(System.IntPtr,Unity.Collections.AllocatorManager/Block&), allocatorState, ref block, functionPointer);
						}
					}
					return AllocatorManager.SlabAllocator.Try$BurstManaged(allocatorState, ref block);
				}

				private static IntPtr Pointer;
			}
		}

		internal struct TableEntry
		{
			internal IntPtr function;

			internal IntPtr state;
		}

		internal struct Array16<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
		{
			internal T f0;

			internal T f1;

			internal T f2;

			internal T f3;

			internal T f4;

			internal T f5;

			internal T f6;

			internal T f7;

			internal T f8;

			internal T f9;

			internal T f10;

			internal T f11;

			internal T f12;

			internal T f13;

			internal T f14;

			internal T f15;
		}

		internal struct Array256<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
		{
			internal AllocatorManager.Array16<T> f0;

			internal AllocatorManager.Array16<T> f1;

			internal AllocatorManager.Array16<T> f2;

			internal AllocatorManager.Array16<T> f3;

			internal AllocatorManager.Array16<T> f4;

			internal AllocatorManager.Array16<T> f5;

			internal AllocatorManager.Array16<T> f6;

			internal AllocatorManager.Array16<T> f7;

			internal AllocatorManager.Array16<T> f8;

			internal AllocatorManager.Array16<T> f9;

			internal AllocatorManager.Array16<T> f10;

			internal AllocatorManager.Array16<T> f11;

			internal AllocatorManager.Array16<T> f12;

			internal AllocatorManager.Array16<T> f13;

			internal AllocatorManager.Array16<T> f14;

			internal AllocatorManager.Array16<T> f15;
		}

		internal struct Array4096<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
		{
			internal AllocatorManager.Array256<T> f0;

			internal AllocatorManager.Array256<T> f1;

			internal AllocatorManager.Array256<T> f2;

			internal AllocatorManager.Array256<T> f3;

			internal AllocatorManager.Array256<T> f4;

			internal AllocatorManager.Array256<T> f5;

			internal AllocatorManager.Array256<T> f6;

			internal AllocatorManager.Array256<T> f7;

			internal AllocatorManager.Array256<T> f8;

			internal AllocatorManager.Array256<T> f9;

			internal AllocatorManager.Array256<T> f10;

			internal AllocatorManager.Array256<T> f11;

			internal AllocatorManager.Array256<T> f12;

			internal AllocatorManager.Array256<T> f13;

			internal AllocatorManager.Array256<T> f14;

			internal AllocatorManager.Array256<T> f15;
		}

		internal struct Array32768<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : IIndexable<T> where T : struct, ValueType
		{
			public int Length
			{
				get
				{
					return 32768;
				}
				set
				{
				}
			}

			public unsafe ref T ElementAt(int index)
			{
				fixed (AllocatorManager.Array4096<T>* ptr = &this.f0)
				{
					return UnsafeUtility.AsRef<T>((void*)((byte*)ptr + (IntPtr)index * (IntPtr)sizeof(T)));
				}
			}

			internal AllocatorManager.Array4096<T> f0;

			internal AllocatorManager.Array4096<T> f1;

			internal AllocatorManager.Array4096<T> f2;

			internal AllocatorManager.Array4096<T> f3;

			internal AllocatorManager.Array4096<T> f4;

			internal AllocatorManager.Array4096<T> f5;

			internal AllocatorManager.Array4096<T> f6;

			internal AllocatorManager.Array4096<T> f7;
		}

		internal sealed class SharedStatics
		{
			internal sealed class IsInstalled
			{
				internal static readonly SharedStatic<Long1024> Ref = SharedStatic<Long1024>.GetOrCreateUnsafe(0U, -4832911380680317357L, 0L);
			}

			internal sealed class TableEntry
			{
				internal static readonly SharedStatic<AllocatorManager.Array32768<AllocatorManager.TableEntry>> Ref = SharedStatic<AllocatorManager.Array32768<AllocatorManager.TableEntry>>.GetOrCreateUnsafe(0U, -1297938794087215229L, 0L);
			}

			internal sealed class IsAutoDispose
			{
				internal static readonly SharedStatic<Long1024> Ref = SharedStatic<Long1024>.GetOrCreateUnsafe(0U, -5725630068035020733L, 0L);
			}
		}

		internal static class Managed
		{
			[ExcludeFromBurstCompatTesting("Uses managed delegate")]
			public static void RegisterDelegate(int index, AllocatorManager.TryFunction function)
			{
				if (index >= 32768)
				{
					throw new ArgumentException("index to be registered in TryFunction delegate table exceeds maximum.");
				}
				AllocatorManager.Managed.TryFunctionDelegates[index] = function;
			}

			[ExcludeFromBurstCompatTesting("Uses managed delegate")]
			public static void UnregisterDelegate(int index)
			{
				if (index >= 32768)
				{
					throw new ArgumentException("index to be unregistered in TryFunction delegate table exceeds maximum.");
				}
				AllocatorManager.Managed.TryFunctionDelegates[index] = null;
			}

			internal static AllocatorManager.TryFunction[] TryFunctionDelegates = new AllocatorManager.TryFunction[32768];
		}

		private static class AllocatorCache<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType, AllocatorManager.IAllocator
		{
			public static FunctionPointer<AllocatorManager.TryFunction> TryFunction;

			public static AllocatorManager.TryFunction CachedFunction;
		}
	}
}
