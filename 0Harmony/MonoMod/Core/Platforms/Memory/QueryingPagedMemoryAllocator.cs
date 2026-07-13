using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Memory
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class QueryingPagedMemoryAllocator : PagedMemoryAllocator
	{
		public QueryingPagedMemoryAllocator(QueryingMemoryPageAllocatorBase alloc)
			: base((IntPtr)((UIntPtr)Helpers.ThrowIfNull<QueryingMemoryPageAllocatorBase>(alloc, "alloc").PageSize))
		{
			this.pageAlloc = alloc;
		}

		protected override bool TryAllocateNewPage(AllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
		{
			IntPtr intPtr;
			if (!this.pageAlloc.TryAllocatePage(base.PageSize, request.Executable, out intPtr))
			{
				allocated = null;
				return false;
			}
			PagedMemoryAllocator.Page page = new PagedMemoryAllocator.Page(this, intPtr, (uint)base.PageSize, request.Executable);
			base.InsertAllocatedPage(page);
			PagedMemoryAllocator.PageAllocation pageAllocation;
			if (!page.TryAllocate((uint)request.Size, (uint)request.Alignment, out pageAllocation))
			{
				base.RegisterForCleanup(page);
				allocated = null;
				return false;
			}
			allocated = pageAllocation;
			return true;
		}

		protected override bool TryAllocateNewPage(PositionedAllocationRequest request, [NativeInteger] IntPtr targetPage, [NativeInteger] IntPtr lowPageBound, [NativeInteger] IntPtr highPageBound, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
		{
			IntPtr target = request.Target;
			IntPtr intPtr = targetPage;
			IntPtr intPtr2 = targetPage + base.PageSize;
			while (intPtr >= lowPageBound || intPtr2 < highPageBound)
			{
				while (intPtr2 < highPageBound)
				{
					if (intPtr >= lowPageBound && target - intPtr <= intPtr2 - target)
					{
						break;
					}
					if (this.TryAllocNewPage(request, ref intPtr2, true, out allocated))
					{
						return true;
					}
				}
				while (intPtr >= lowPageBound && (intPtr2 >= highPageBound || target - intPtr < intPtr2 - target))
				{
					if (this.TryAllocNewPage(request, ref intPtr, false, out allocated))
					{
						return true;
					}
				}
			}
			allocated = null;
			return false;
		}

		private bool TryAllocNewPage(PositionedAllocationRequest request, [NativeInteger] ref IntPtr page, bool goingUp, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
		{
			bool flag;
			IntPtr intPtr;
			IntPtr intPtr2;
			if (this.pageAlloc.TryQueryPage(page, out flag, out intPtr, out intPtr2))
			{
				IntPtr intPtr3;
				if (flag && this.pageAlloc.TryAllocatePage(page, base.PageSize, request.Base.Executable, out intPtr3))
				{
					PagedMemoryAllocator.Page page2 = new PagedMemoryAllocator.Page(this, intPtr3, (uint)base.PageSize, request.Base.Executable);
					base.InsertAllocatedPage(page2);
					PagedMemoryAllocator.PageAllocation pageAllocation;
					if (page2.TryAllocate((uint)request.Base.Size, (uint)request.Base.Alignment, out pageAllocation))
					{
						allocated = pageAllocation;
						return true;
					}
					base.RegisterForCleanup(page2);
				}
				if (goingUp)
				{
					page = intPtr + intPtr2;
				}
				else
				{
					page = intPtr - base.PageSize;
				}
				allocated = null;
				return false;
			}
			if (goingUp)
			{
				page += base.PageSize;
			}
			else
			{
				page -= base.PageSize;
			}
			allocated = null;
			return false;
		}

		protected override bool TryFreePage(PagedMemoryAllocator.Page page, [Nullable(2)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>NotNullWhen(false)] out string errorMsg)
		{
			return this.pageAlloc.TryFreePage(page.BaseAddr, out errorMsg);
		}

		private readonly QueryingMemoryPageAllocatorBase pageAlloc;
	}
}
