using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MonoMod.Core.Platforms.Memory
{
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class PagedMemoryAllocator : IMemoryAllocator
	{
		[NativeInteger]
		protected IntPtr PageSize
		{
			[return: NativeInteger]
			get
			{
				return this.pageSize;
			}
		}

		protected PagedMemoryAllocator([NativeInteger] IntPtr pageSize)
		{
			this.pageSize = pageSize;
			this.pageSizeIsPow2 = BitOperationsEx.IsPow2(pageSize);
			this.pageBaseMask = ~(IntPtr)0 << (BitOperationsEx.TrailingZeroCount(pageSize) & (sizeof(IntPtr) * 8 - 1));
		}

		[return: NativeInteger]
		public IntPtr RoundDownToPageBoundary([NativeInteger] IntPtr addr)
		{
			if (this.pageSizeIsPow2)
			{
				return addr & this.pageBaseMask;
			}
			return addr - addr % this.pageSize;
		}

		protected unsafe void InsertAllocatedPage(PagedMemoryAllocator.Page page)
		{
			if (this.pageCount == this.allocationList.Length)
			{
				int num = (int)BitOperationsEx.RoundUpToPowerOf2((uint)(this.allocationList.Length + 1));
				Array.Resize<PagedMemoryAllocator.Page>(ref this.allocationList, num);
			}
			global::System.Span<PagedMemoryAllocator.Page> span = this.allocationList.AsSpan<PagedMemoryAllocator.Page>();
			int num2 = span.Slice(0, this.pageCount).BinarySearch(page, default(PagedMemoryAllocator.PageComparer));
			if (num2 >= 0)
			{
				return;
			}
			num2 = ~num2;
			if (num2 + 1 < span.Length)
			{
				span.Slice(num2, this.pageCount - num2).CopyTo(span.Slice(num2 + 1));
			}
			*span[num2] = page;
			this.pageCount++;
		}

		private void RemoveAllocatedPage(PagedMemoryAllocator.Page page)
		{
			global::System.Span<PagedMemoryAllocator.Page> span = this.allocationList.AsSpan<PagedMemoryAllocator.Page>();
			int num = span.Slice(0, this.pageCount).BinarySearch(page, default(PagedMemoryAllocator.PageComparer));
			if (num < 0)
			{
				return;
			}
			span.Slice(num + 1).CopyTo(span.Slice(num));
			this.pageCount--;
		}

		[Nullable(new byte[] { 0, 1 })]
		private global::System.ReadOnlySpan<PagedMemoryAllocator.Page> AllocList
		{
			[return: Nullable(new byte[] { 0, 1 })]
			get
			{
				return this.allocationList.AsSpan<PagedMemoryAllocator.Page>().Slice(0, this.pageCount);
			}
		}

		private int GetBoundIndex(IntPtr ptr)
		{
			int num = this.AllocList.BinarySearch(new PagedMemoryAllocator.PageAddrComparable(ptr));
			if (num < 0)
			{
				return ~num;
			}
			return num;
		}

		protected void RegisterForCleanup(PagedMemoryAllocator.Page page)
		{
			if (Environment.HasShutdownStarted || AppDomain.CurrentDomain.IsFinalizingForUnload())
			{
				return;
			}
			this.pagesToClean.Add(page);
			if (Interlocked.CompareExchange(ref this.registeredForCleanup, 1, 0) == 0)
			{
				Gen2GcCallback.Register(new Func<bool>(this.DoCleanup));
			}
		}

		private bool DoCleanup()
		{
			if (Environment.HasShutdownStarted || AppDomain.CurrentDomain.IsFinalizingForUnload())
			{
				return false;
			}
			Volatile.Write(ref this.registeredForCleanup, 0);
			PagedMemoryAllocator.Page page;
			while (this.pagesToClean.TryTake(out page))
			{
				object obj = this.sync;
				lock (obj)
				{
					if (!page.IsEmpty)
					{
						continue;
					}
					this.RemoveAllocatedPage(page);
				}
				string text;
				if (!this.TryFreePage(page, out text))
				{
					bool flag;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(27, 1, out flag);
					if (flag)
					{
						debugLogErrorStringHandler.AppendLiteral("Could not deallocate page! ");
						debugLogErrorStringHandler.AppendFormatted(text);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
				}
			}
			return false;
		}

		protected abstract bool TryFreePage(PagedMemoryAllocator.Page page, [Nullable(2)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>NotNullWhen(false)] out string errorMsg);

		public int MaxSize
		{
			get
			{
				return (int)this.pageSize;
			}
		}

		public unsafe bool TryAllocateInRange(PositionedAllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
		{
			if (request.Target < request.LowBound || request.Target > request.HighBound)
			{
				throw new ArgumentException("Target not between low and high", "request");
			}
			if (request.Base.Size < 0)
			{
				throw new ArgumentException("Size is negative", "request");
			}
			if (request.Base.Alignment <= 0)
			{
				throw new ArgumentException("Alignment is zero or negative", "request");
			}
			if ((IntPtr)request.Base.Size > this.pageSize)
			{
				throw new NotSupportedException("Single allocations cannot be larger than a page");
			}
			IntPtr intPtr = this.RoundDownToPageBoundary(request.LowBound + this.pageSize - (IntPtr)1);
			IntPtr intPtr2 = this.RoundDownToPageBoundary(request.HighBound);
			IntPtr intPtr3 = this.RoundDownToPageBoundary(request.Target);
			IntPtr target = request.Target;
			object obj = this.sync;
			bool flag2;
			lock (obj)
			{
				int boundIndex = this.GetBoundIndex(intPtr);
				int boundIndex2 = this.GetBoundIndex(intPtr2);
				if (boundIndex != boundIndex2)
				{
					int boundIndex3 = this.GetBoundIndex(intPtr3);
					int num = boundIndex3 - 1;
					int num2 = boundIndex3;
					while ((ulong)num2 <= (ulong)((long)this.AllocList.Length) && (ulong)num < (ulong)((long)this.AllocList.Length) && (num >= boundIndex || num2 < boundIndex2))
					{
						while ((ulong)num2 < (ulong)((long)this.AllocList.Length) && num2 < boundIndex2)
						{
							if (num >= boundIndex && target - this.AllocList[num]->BaseAddr <= this.AllocList[num2]->BaseAddr - target)
							{
								break;
							}
							if (PagedMemoryAllocator.TryAllocWithPage(*this.AllocList[num2], request, out allocated))
							{
								return true;
							}
							num2++;
						}
						while ((ulong)num < (ulong)((long)this.AllocList.Length) && num >= boundIndex && (num2 >= boundIndex2 || target - this.AllocList[num]->BaseAddr < this.AllocList[num2]->BaseAddr - target))
						{
							if (PagedMemoryAllocator.TryAllocWithPage(*this.AllocList[num], request, out allocated))
							{
								return true;
							}
							num++;
						}
					}
				}
				flag2 = this.TryAllocateNewPage(request, intPtr3, intPtr, intPtr2, out allocated);
			}
			return flag2;
		}

		private static bool TryAllocWithPage(PagedMemoryAllocator.Page page, PositionedAllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
		{
			PagedMemoryAllocator.PageAllocation pageAllocation;
			if (page.IsExecutable == request.Base.Executable && page.BaseAddr >= request.LowBound && page.BaseAddr < request.HighBound && page.TryAllocate((uint)request.Base.Size, (uint)request.Base.Alignment, out pageAllocation))
			{
				if (pageAllocation.BaseAddress >= request.LowBound && pageAllocation.BaseAddress < request.HighBound)
				{
					allocated = pageAllocation;
					return true;
				}
				pageAllocation.Dispose();
			}
			allocated = null;
			return false;
		}

		public unsafe bool TryAllocate(AllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
		{
			if (request.Size < 0)
			{
				throw new ArgumentException("Size is negative", "request");
			}
			if (request.Alignment <= 0)
			{
				throw new ArgumentException("Alignment is zero or negative", "request");
			}
			if ((IntPtr)request.Size > this.pageSize)
			{
				throw new NotSupportedException("Single allocations cannot be larger than a page");
			}
			object obj = this.sync;
			bool flag2;
			lock (obj)
			{
				global::System.ReadOnlySpan<PagedMemoryAllocator.Page> allocList = this.AllocList;
				for (int i = 0; i < allocList.Length; i++)
				{
					PagedMemoryAllocator.Page page = *allocList[i];
					PagedMemoryAllocator.PageAllocation pageAllocation;
					if (page.IsExecutable == request.Executable && page.TryAllocate((uint)request.Size, (uint)request.Alignment, out pageAllocation))
					{
						allocated = pageAllocation;
						return true;
					}
				}
				flag2 = this.TryAllocateNewPage(request, out allocated);
			}
			return flag2;
		}

		protected abstract bool TryAllocateNewPage(AllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated);

		protected abstract bool TryAllocateNewPage(PositionedAllocationRequest request, [NativeInteger] IntPtr targetPage, [NativeInteger] IntPtr lowPageBound, [NativeInteger] IntPtr highPageBound, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated);

		[NativeInteger]
		private readonly IntPtr pageBaseMask;

		[NativeInteger]
		private readonly IntPtr pageSize;

		private readonly bool pageSizeIsPow2;

		[Nullable(new byte[] { 1, 2 })]
		private PagedMemoryAllocator.Page[] allocationList = new PagedMemoryAllocator.Page[16];

		private int pageCount;

		private readonly ConcurrentBag<PagedMemoryAllocator.Page> pagesToClean = new ConcurrentBag<PagedMemoryAllocator.Page>();

		private int registeredForCleanup;

		private readonly object sync = new object();

		[NullableContext(0)]
		private sealed class FreeMem
		{
			public uint BaseOffset;

			public uint Size;

			[Nullable(2)]
			public PagedMemoryAllocator.FreeMem NextFree;
		}

		[NullableContext(0)]
		protected sealed class PageAllocation : IAllocatedMemory, IDisposable
		{
			public bool IsExecutable
			{
				get
				{
					return this.owner.IsExecutable;
				}
			}

			[NullableContext(1)]
			public PageAllocation(PagedMemoryAllocator.Page page, uint offset, int size)
			{
				this.owner = page;
				this.offset = offset;
				this.Size = size;
			}

			public IntPtr BaseAddress
			{
				get
				{
					return this.owner.BaseAddr + (IntPtr)((UIntPtr)this.offset);
				}
			}

			public int Size { get; }

			public unsafe global::System.Span<byte> Memory
			{
				get
				{
					return new global::System.Span<byte>((void*)this.BaseAddress, this.Size);
				}
			}

			private void Dispose(bool disposing)
			{
				if (!this.disposedValue)
				{
					this.owner.FreeMem(this.offset, (uint)this.Size);
					this.disposedValue = true;
				}
			}

			~PageAllocation()
			{
				this.Dispose(false);
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			[Nullable(1)]
			private readonly PagedMemoryAllocator.Page owner;

			private readonly uint offset;

			private bool disposedValue;
		}

		[Nullable(0)]
		protected sealed class Page
		{
			public bool IsEmpty
			{
				get
				{
					PagedMemoryAllocator.FreeMem freeMem = this.freeList;
					return freeMem != null && freeMem.BaseOffset == 0U && freeMem.Size == this.Size;
				}
			}

			public IntPtr BaseAddr { get; }

			public uint Size { get; }

			public bool IsExecutable { get; }

			public Page(PagedMemoryAllocator owner, IntPtr baseAddr, uint size, bool isExecutable)
			{
				this.owner = owner;
				this.BaseAddr = baseAddr;
				this.Size = size;
				this.IsExecutable = isExecutable;
				this.freeList = new PagedMemoryAllocator.FreeMem
				{
					BaseOffset = 0U,
					Size = size,
					NextFree = null
				};
			}

			public bool TryAllocate(uint size, uint align, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out PagedMemoryAllocator.PageAllocation alloc)
			{
				object obj = this.sync;
				bool flag2;
				lock (obj)
				{
					ref PagedMemoryAllocator.FreeMem ptr = ref this.freeList;
					uint num = 0U;
					while (ptr != null)
					{
						uint num2 = ptr.BaseOffset % align;
						num2 = ((num2 != 0U) ? (align - num2) : num2);
						if (ptr.Size >= num2 + size)
						{
							num = num2;
							break;
						}
						ptr = ref ptr.NextFree;
					}
					if (ptr == null)
					{
						alloc = null;
						flag2 = false;
					}
					else
					{
						uint num3 = ptr.BaseOffset + num;
						if (num == 0U)
						{
							ptr.BaseOffset += size;
							ptr.Size -= size;
						}
						else
						{
							PagedMemoryAllocator.FreeMem freeMem = new PagedMemoryAllocator.FreeMem
							{
								BaseOffset = ptr.BaseOffset,
								Size = num,
								NextFree = ptr
							};
							ptr.BaseOffset += num + size;
							ptr.Size -= num + size;
							ptr = freeMem;
						}
						this.NormalizeFreeList();
						alloc = new PagedMemoryAllocator.PageAllocation(this, num3, (int)size);
						flag2 = true;
					}
				}
				return flag2;
			}

			private void NormalizeFreeList()
			{
				ref PagedMemoryAllocator.FreeMem ptr = ref this.freeList;
				while (ptr != null)
				{
					if (ptr.Size <= 0U)
					{
						ptr = ptr.NextFree;
					}
					else
					{
						PagedMemoryAllocator.FreeMem nextFree = ptr.NextFree;
						if (nextFree != null && nextFree.BaseOffset == ptr.BaseOffset + ptr.Size)
						{
							ptr.Size += nextFree.Size;
							ptr.NextFree = nextFree.NextFree;
						}
						else
						{
							ptr = ref ptr.NextFree;
						}
					}
				}
			}

			internal void FreeMem(uint offset, uint size)
			{
				object obj = this.sync;
				lock (obj)
				{
					ref PagedMemoryAllocator.FreeMem ptr = ref this.freeList;
					while (ptr != null && ptr.BaseOffset <= offset)
					{
						ptr = ref ptr.NextFree;
					}
					ptr = new PagedMemoryAllocator.FreeMem
					{
						BaseOffset = offset,
						Size = size,
						NextFree = ptr
					};
					this.NormalizeFreeList();
					if (this.IsEmpty)
					{
						this.owner.RegisterForCleanup(this);
					}
				}
			}

			private readonly PagedMemoryAllocator owner;

			private readonly object sync = new object();

			[Nullable(2)]
			private PagedMemoryAllocator.FreeMem freeList;
		}

		[NullableContext(0)]
		private readonly struct PageComparer : IComparer<PagedMemoryAllocator.Page>
		{
			[NullableContext(2)]
			public int Compare(PagedMemoryAllocator.Page x, PagedMemoryAllocator.Page y)
			{
				if (x == y)
				{
					return 0;
				}
				if (x == null)
				{
					return 1;
				}
				if (y == null)
				{
					return -1;
				}
				return ((long)x.BaseAddr).CompareTo((long)y.BaseAddr);
			}
		}

		[NullableContext(0)]
		private readonly struct PageAddrComparable : IComparable<PagedMemoryAllocator.Page>
		{
			public PageAddrComparable(IntPtr addr)
			{
				this.addr = addr;
			}

			[NullableContext(2)]
			public int CompareTo(PagedMemoryAllocator.Page other)
			{
				if (other == null)
				{
					return 1;
				}
				return ((long)this.addr).CompareTo((long)other.BaseAddr);
			}

			private readonly IntPtr addr;
		}
	}
}
