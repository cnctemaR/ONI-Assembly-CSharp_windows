using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace Unity.Collections
{
	[StructLayout(LayoutKind.Sequential, Size = 16)]
	public readonly struct MemoryLabel
	{
		public MemoryLabel(string areaName, string objectName, Allocator allocator = Allocator.Persistent)
		{
			bool flag = MemoryLabel.IsNullOrEmpty(areaName);
			if (flag)
			{
				throw new ArgumentNullException("areaName");
			}
			bool flag2 = MemoryLabel.IsNullOrEmpty(objectName);
			if (flag2)
			{
				throw new ArgumentNullException("objectName");
			}
			bool flag3 = !MemoryLabel.SupportsAllocator(allocator);
			if (flag3)
			{
				throw new ArgumentException("Only Allocator.Persistent and Allocator.Domain support allocating with a label");
			}
			this.allocator = allocator;
			this.pointer = ProfilerUnsafeUtility.GetOrCreateMemLabel(areaName, objectName);
		}

		public static bool SupportsAllocator(Allocator allocator)
		{
			return allocator == Allocator.Persistent || allocator == Allocator.Domain;
		}

		private static bool IsNullOrEmpty(string str)
		{
			return string.IsNullOrEmpty(str);
		}

		[RequiredMember]
		private unsafe static bool IsNullOrEmpty__Unmanaged(byte* name, int nameLen)
		{
			return name == null || nameLen <= 0;
		}

		internal long RelatedMemorySize
		{
			get
			{
				return ProfilerUnsafeUtility.GetMemLabelRelatedMemorySize(this.pointer);
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.allocator > Allocator.Invalid;
			}
		}

		internal void CheckArgument()
		{
			bool flag = !this.IsCreated;
			if (flag)
			{
				throw new ArgumentException("MemoryLabel has not been created. Use the constructor to create it.");
			}
		}

		[NativeDisableUnsafePtrRestriction]
		internal readonly IntPtr pointer;

		internal readonly Allocator allocator;
	}
}
