using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
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

		internal unsafe static void* Allocate<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this T t, int sizeOf, int alignOf, int items) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			return (void*)(ref t).AllocateBlock<T>(sizeOf, alignOf, items).Range.Pointer;
		}

		internal unsafe static U* Allocate<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(this T t, U u, int items) where T : struct, ValueType, AllocatorManager.IAllocator where U : struct, ValueType
		{
			return (U*)(ref t).Allocate<T>(UnsafeUtility.SizeOf<U>(), UnsafeUtility.AlignOf<U>(), items);
		}

		internal unsafe static void* AllocateStruct<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this T t, U u, int items) where T : struct, ValueType, AllocatorManager.IAllocator where U : struct
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

		internal static void Install(AllocatorManager.AllocatorHandle handle, IntPtr allocatorState, FunctionPointer<AllocatorManager.TryFunction> functionPointer, AllocatorManager.TryFunction function)
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
			}
		}

		internal static void Install(AllocatorManager.AllocatorHandle handle, IntPtr allocatorState, AllocatorManager.TryFunction function)
		{
			FunctionPointer<AllocatorManager.TryFunction> functionPointer = ((function == null) ? new FunctionPointer<AllocatorManager.TryFunction>(IntPtr.Zero) : BurstCompiler.CompileFunctionPointer<AllocatorManager.TryFunction>(function));
			AllocatorManager.Install(handle, allocatorState, functionPointer, function);
		}

		internal static AllocatorManager.AllocatorHandle Register(IntPtr allocatorState, FunctionPointer<AllocatorManager.TryFunction> functionPointer)
		{
			AllocatorManager.TableEntry tableEntry = new AllocatorManager.TableEntry
			{
				state = allocatorState,
				function = functionPointer.Value
			};
			int num2;
			int num = ConcurrentMask.TryAllocate<Long1024>(AllocatorManager.SharedStatics.IsInstalled.Ref.Data, out num2, 1, AllocatorManager.SharedStatics.IsInstalled.Ref.Data.Length, 1);
			AllocatorManager.AllocatorHandle allocatorHandle = default(AllocatorManager.AllocatorHandle);
			if (ConcurrentMask.Succeeded(num))
			{
				allocatorHandle.Index = (ushort)num2;
				allocatorHandle.Install(tableEntry);
			}
			return allocatorHandle;
		}

		[NotBurstCompatible]
		public static void Register<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this T t) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			FunctionPointer<AllocatorManager.TryFunction> functionPointer = ((t.Function == null) ? new FunctionPointer<AllocatorManager.TryFunction>(IntPtr.Zero) : BurstCompiler.CompileFunctionPointer<AllocatorManager.TryFunction>(t.Function));
			t.Handle = AllocatorManager.Register((IntPtr)UnsafeUtility.AddressOf<T>(ref t), functionPointer);
			AllocatorManager.Managed.RegisterDelegate((int)t.Handle.Index, t.Function);
		}

		public static void UnmanagedUnregister<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this T t) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			if (t.Handle.IsInstalled)
			{
				t.Handle.Install(default(AllocatorManager.TableEntry));
				ConcurrentMask.TryFree<Long1024>(AllocatorManager.SharedStatics.IsInstalled.Ref.Data, t.Handle.Value, 1);
			}
		}

		[NotBurstCompatible]
		public static void Unregister<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this T t) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			if (t.Handle.IsInstalled)
			{
				t.Handle.Install(default(AllocatorManager.TableEntry));
				ConcurrentMask.TryFree<Long1024>(AllocatorManager.SharedStatics.IsInstalled.Ref.Data, t.Handle.Value, 1);
				AllocatorManager.Managed.UnregisterDelegate((int)t.Handle.Index);
			}
		}

		[NotBurstCompatible]
		internal unsafe static ref T CreateAllocator<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(AllocatorManager.AllocatorHandle backingAllocator) where T : struct, ValueType, AllocatorManager.IAllocator
		{
			T* ptr = (T*)Memory.Unmanaged.Allocate((long)UnsafeUtility.SizeOf<T>(), 16, backingAllocator);
			*ptr = default(T);
			ref T ptr2 = ref UnsafeUtility.AsRef<T>((void*)ptr);
			(ref ptr2).Register<T>();
			return ref ptr2;
		}

		[NotBurstCompatible]
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

		[Conditional("ENABLE_UNITY_ALLOCATION_CHECKS")]
		internal static void CheckFailedToAllocate(int error)
		{
			if (error != 0)
			{
				throw new ArgumentException("failed to allocate");
			}
		}

		[Conditional("ENABLE_UNITY_ALLOCATION_CHECKS")]
		internal static void CheckFailedToFree(int error)
		{
			if (error != 0)
			{
				throw new ArgumentException("failed to free");
			}
		}

		[Conditional("ENABLE_UNITY_ALLOCATION_CHECKS")]
		internal static void CheckValid(AllocatorManager.AllocatorHandle handle)
		{
		}

		public static void Initialize$StackAllocator_Try_00000980$BurstDirectCall()
		{
			AllocatorManager.StackAllocator.Try_00000980$BurstDirectCall.Initialize();
		}

		public static void Initialize$SlabAllocator_Try_0000098E$BurstDirectCall()
		{
			AllocatorManager.SlabAllocator.Try_0000098E$BurstDirectCall.Initialize();
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

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int TryFunction(IntPtr allocatorState, ref AllocatorManager.Block block);

		public struct AllocatorHandle : AllocatorManager.IAllocator, IDisposable
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
					Version = (ushort)(a >> 16)
				};
			}

			public int Value
			{
				get
				{
					return (int)this.Index;
				}
			}

			public int TryAllocateBlock<T>(out AllocatorManager.Block block, int items) where T : struct
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

			public AllocatorManager.Block AllocateBlock<T>(int items) where T : struct
			{
				AllocatorManager.Block block;
				this.TryAllocateBlock<T>(out block, items);
				return block;
			}

			[Conditional("ENABLE_UNITY_ALLOCATION_CHECKS")]
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

			public void Dispose()
			{
				this.Rewind();
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
					return (long)(this.BytesPerItem * this.Range.Items);
				}
			}

			public long AllocatedBytes
			{
				get
				{
					return (long)(this.BytesPerItem * this.AllocatedItems);
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

			[Conditional("ENABLE_UNITY_ALLOCATION_CHECKS")]
			private void CheckFailedToAllocate(int error)
			{
				if (error != 0)
				{
					throw new ArgumentException(string.Format("Error {0}: Failed to Allocate {1}", error, this));
				}
			}

			[Conditional("ENABLE_UNITY_ALLOCATION_CHECKS")]
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
		}

		[BurstCompile(CompileSynchronously = true)]
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

			[BurstCompile(CompileSynchronously = true)]
			[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
			public static int Try(IntPtr allocatorState, ref AllocatorManager.Block block)
			{
				return AllocatorManager.StackAllocator.Try_00000980$BurstDirectCall.Invoke(allocatorState, ref block);
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

			[BurstCompile(CompileSynchronously = true)]
			[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe static int Try$BurstManaged(IntPtr allocatorState, ref AllocatorManager.Block block)
			{
				return ((AllocatorManager.StackAllocator*)(void*)allocatorState)->Try(ref block);
			}

			internal AllocatorManager.AllocatorHandle m_handle;

			internal AllocatorManager.Block m_storage;

			internal long m_top;

			public delegate int Try_00000980$PostfixBurstDelegate(IntPtr allocatorState, ref AllocatorManager.Block block);

			internal static class Try_00000980$BurstDirectCall
			{
				[BurstDiscard]
				private static void GetFunctionPointerDiscard(ref IntPtr A_0)
				{
					if (AllocatorManager.StackAllocator.Try_00000980$BurstDirectCall.Pointer == 0)
					{
						AllocatorManager.StackAllocator.Try_00000980$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(AllocatorManager.StackAllocator.Try_00000980$BurstDirectCall.DeferredCompilation, methodof(AllocatorManager.StackAllocator.Try$BurstManaged(IntPtr, ref AllocatorManager.Block)).MethodHandle, typeof(AllocatorManager.StackAllocator.Try_00000980$PostfixBurstDelegate).TypeHandle);
					}
					A_0 = AllocatorManager.StackAllocator.Try_00000980$BurstDirectCall.Pointer;
				}

				private static IntPtr GetFunctionPointer()
				{
					IntPtr intPtr = (IntPtr)0;
					AllocatorManager.StackAllocator.Try_00000980$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
					return intPtr;
				}

				public static void Constructor()
				{
					AllocatorManager.StackAllocator.Try_00000980$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(AllocatorManager.StackAllocator.Try(IntPtr, ref AllocatorManager.Block)).MethodHandle);
				}

				public static void Initialize()
				{
				}

				// Note: this type is marked as 'beforefieldinit'.
				static Try_00000980$BurstDirectCall()
				{
					AllocatorManager.StackAllocator.Try_00000980$BurstDirectCall.Constructor();
				}

				public static int Invoke(IntPtr allocatorState, ref AllocatorManager.Block block)
				{
					if (BurstCompiler.IsEnabled)
					{
						IntPtr functionPointer = AllocatorManager.StackAllocator.Try_00000980$BurstDirectCall.GetFunctionPointer();
						if (functionPointer != 0)
						{
							return calli(System.Int32(System.IntPtr,Unity.Collections.AllocatorManager/Block&), allocatorState, ref block, functionPointer);
						}
					}
					return AllocatorManager.StackAllocator.Try$BurstManaged(allocatorState, ref block);
				}

				private static IntPtr Pointer;

				private static IntPtr DeferredCompilation;
			}
		}

		[BurstCompile(CompileSynchronously = true)]
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

			[BurstCompile(CompileSynchronously = true)]
			[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
			public static int Try(IntPtr allocatorState, ref AllocatorManager.Block block)
			{
				return AllocatorManager.SlabAllocator.Try_0000098E$BurstDirectCall.Invoke(allocatorState, ref block);
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

			[BurstCompile(CompileSynchronously = true)]
			[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe static int Try$BurstManaged(IntPtr allocatorState, ref AllocatorManager.Block block)
			{
				return ((AllocatorManager.SlabAllocator*)(void*)allocatorState)->Try(ref block);
			}

			internal AllocatorManager.AllocatorHandle m_handle;

			internal AllocatorManager.Block Storage;

			internal int Log2SlabSizeInBytes;

			internal FixedList4096Bytes<int> Occupied;

			internal long budgetInBytes;

			internal long allocatedBytes;

			public delegate int Try_0000098E$PostfixBurstDelegate(IntPtr allocatorState, ref AllocatorManager.Block block);

			internal static class Try_0000098E$BurstDirectCall
			{
				[BurstDiscard]
				private static void GetFunctionPointerDiscard(ref IntPtr A_0)
				{
					if (AllocatorManager.SlabAllocator.Try_0000098E$BurstDirectCall.Pointer == 0)
					{
						AllocatorManager.SlabAllocator.Try_0000098E$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(AllocatorManager.SlabAllocator.Try_0000098E$BurstDirectCall.DeferredCompilation, methodof(AllocatorManager.SlabAllocator.Try$BurstManaged(IntPtr, ref AllocatorManager.Block)).MethodHandle, typeof(AllocatorManager.SlabAllocator.Try_0000098E$PostfixBurstDelegate).TypeHandle);
					}
					A_0 = AllocatorManager.SlabAllocator.Try_0000098E$BurstDirectCall.Pointer;
				}

				private static IntPtr GetFunctionPointer()
				{
					IntPtr intPtr = (IntPtr)0;
					AllocatorManager.SlabAllocator.Try_0000098E$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
					return intPtr;
				}

				public static void Constructor()
				{
					AllocatorManager.SlabAllocator.Try_0000098E$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(AllocatorManager.SlabAllocator.Try(IntPtr, ref AllocatorManager.Block)).MethodHandle);
				}

				public static void Initialize()
				{
				}

				// Note: this type is marked as 'beforefieldinit'.
				static Try_0000098E$BurstDirectCall()
				{
					AllocatorManager.SlabAllocator.Try_0000098E$BurstDirectCall.Constructor();
				}

				public static int Invoke(IntPtr allocatorState, ref AllocatorManager.Block block)
				{
					if (BurstCompiler.IsEnabled)
					{
						IntPtr functionPointer = AllocatorManager.SlabAllocator.Try_0000098E$BurstDirectCall.GetFunctionPointer();
						if (functionPointer != 0)
						{
							return calli(System.Int32(System.IntPtr,Unity.Collections.AllocatorManager/Block&), allocatorState, ref block, functionPointer);
						}
					}
					return AllocatorManager.SlabAllocator.Try$BurstManaged(allocatorState, ref block);
				}

				private static IntPtr Pointer;

				private static IntPtr DeferredCompilation;
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
		}

		internal static class Managed
		{
			[NotBurstCompatible]
			public static void RegisterDelegate(int index, AllocatorManager.TryFunction function)
			{
				if (index >= 32768)
				{
					throw new ArgumentException("index to be registered in TryFunction delegate table exceeds maximum.");
				}
				AllocatorManager.Managed.TryFunctionDelegates[index] = function;
			}

			[NotBurstCompatible]
			public static void UnregisterDelegate(int index)
			{
				if (index >= 32768)
				{
					throw new ArgumentException("index to be unregistered in TryFunction delegate table exceeds maximum.");
				}
				AllocatorManager.Managed.TryFunctionDelegates[index] = null;
			}

			internal const int kMaxNumCustomAllocator = 32768;

			internal static AllocatorManager.TryFunction[] TryFunctionDelegates = new AllocatorManager.TryFunction[32768];
		}
	}
}
