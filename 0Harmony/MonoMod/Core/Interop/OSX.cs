using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoMod.Logs;
using MonoMod.Utils;

namespace MonoMod.Core.Interop
{
	internal static class OSX
	{
		[DllImport("libSystem", EntryPoint = "getpagesize")]
		public static extern int GetPageSize();

		[DllImport("libSystem")]
		public unsafe static extern void sys_icache_invalidate(void* start, [NativeInteger] UIntPtr size);

		[DllImport("libSystem", CallingConvention = CallingConvention.Cdecl, EntryPoint = "mkstemp")]
		public unsafe static extern int MkSTemp(byte* template);

		[DllImport("libSystem", CallingConvention = CallingConvention.Cdecl, EntryPoint = "close")]
		public static extern int Close(int fd);

		[DllImport("libSystem", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int* __error();

		public unsafe static int Errno
		{
			get
			{
				return *OSX.__error();
			}
		}

		static OSX()
		{
			int errno = OSX.Errno;
		}

		[DllImport("libSystem")]
		public unsafe static extern OSX.kern_return_t mach_vm_region_recurse(int targetTask, [In] [Out] ulong* address, [Out] ulong* size, [In] [Out] int* nestingDepth, [Out] OSX.vm_region_submap_short_info_64* info, [In] [Out] int* infoSize);

		[DllImport("libSystem")]
		public static extern OSX.kern_return_t mach_vm_protect(int targetTask, ulong address, ulong size, OSX.boolean_t setMax, OSX.vm_prot_t protection);

		[DllImport("libSystem")]
		public unsafe static extern OSX.kern_return_t mach_vm_allocate(int targetTask, [In] [Out] ulong* address, ulong size, OSX.vm_flags flags);

		[DllImport("libSystem")]
		public unsafe static extern OSX.kern_return_t mach_vm_map(int targetTask, [In] [Out] ulong* address, ulong size, ulong mask, OSX.vm_flags flags, int @object, ulong offset, OSX.boolean_t copy, OSX.vm_prot_t curProt, OSX.vm_prot_t maxProt, OSX.vm_inherit_t inheritance);

		[DllImport("libSystem")]
		public unsafe static extern OSX.kern_return_t mach_vm_remap(int targetTask, [In] [Out] ulong* targetAddress, ulong size, ulong offset, OSX.vm_flags flags, int srcTask, ulong srcAddress, OSX.boolean_t copy, [Out] OSX.vm_prot_t* curProt, [Out] OSX.vm_prot_t* maxProt, OSX.vm_inherit_t inherit);

		[DllImport("libSystem")]
		public unsafe static extern OSX.kern_return_t mach_make_memory_entry_64(int targetTask, [In] [Out] ulong* size, ulong offset, OSX.vm_prot_t permission, int* objectHandle, int parentHandle);

		[DllImport("libSystem")]
		public static extern OSX.kern_return_t mach_vm_deallocate(int targetTask, ulong address, ulong size);

		public unsafe static int mach_task_self()
		{
			int* ptr = OSX.mach_task_self_;
			if (ptr == null)
			{
				IntPtr intPtr = DynDll.OpenLibrary("libSystem");
				try
				{
					ptr = (OSX.mach_task_self_ = (int*)(void*)intPtr.GetExport("mach_task_self_"));
				}
				finally
				{
					DynDll.CloseLibrary(intPtr);
				}
			}
			return *ptr;
		}

		[DllImport("libSystem")]
		public unsafe static extern OSX.kern_return_t task_info(int targetTask, OSX.task_flavor_t flavor, [Out] OSX.task_dyld_info* taskInfoOut, int* taskInfoCnt);

		public static OSX.VmProtFmtProxy P(OSX.vm_prot_t prot)
		{
			return new OSX.VmProtFmtProxy(prot);
		}

		[DllImport("libSystem")]
		public static extern IntPtr mmap(IntPtr address, ulong length, OSX.map_prot prot, OSX.map_flags flags, int fd, long offset);

		[Nullable(1)]
		public const string LibSystem = "libSystem";

		private unsafe static int* mach_task_self_;

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		public struct vm_region_submap_short_info_64
		{
			public unsafe static int Count
			{
				get
				{
					return sizeof(OSX.vm_region_submap_short_info_64) / 4;
				}
			}

			public OSX.vm_prot_t protection;

			public OSX.vm_prot_t max_protection;

			public OSX.vm_inherit_t inheritance;

			public ulong offset;

			public uint user_tag;

			public uint ref_count;

			public ushort shadow_depth;

			public byte external_pager;

			public OSX.ShareMode share_mode;

			public OSX.boolean_t is_submap;

			public OSX.vm_behavior_t behavior;

			public uint object_id;

			public ushort user_wired_count;
		}

		public enum ShareMode : byte
		{
			COW = 1,
			Private,
			Empty,
			Shared,
			TrueShared,
			PrivateAliased,
			SharedAliased,
			LargePage
		}

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		public struct task_dyld_info
		{
			public unsafe OSX.dyld_all_image_infos* all_image_infos
			{
				get
				{
					return this.all_image_info_addr;
				}
			}

			public unsafe static int Count
			{
				get
				{
					return sizeof(OSX.task_dyld_info) / 4;
				}
			}

			public ulong all_image_info_addr;

			public ulong all_image_info_size;

			public OSX.task_dyld_all_image_info_format all_image_info_format;
		}

		public struct dyld_all_image_infos
		{
			public unsafe global::System.ReadOnlySpan<OSX.dyld_image_info> InfoArray
			{
				get
				{
					return new global::System.ReadOnlySpan<OSX.dyld_image_info>((void*)this.infoArray, (int)this.infoArrayCount);
				}
			}

			public uint version;

			public uint infoArrayCount;

			public unsafe OSX.dyld_image_info* infoArray;
		}

		public struct dyld_image_info
		{
			public unsafe void* imageLoadAddress;

			public PCSTR imageFilePath;

			[NativeInteger]
			public UIntPtr imageFileModDate;
		}

		public enum task_dyld_all_image_info_format
		{
			Bits32,
			Bits64
		}

		public enum task_flavor_t : uint
		{
			DyldInfo = 17U
		}

		public enum vm_region_flavor_t
		{
			BasicInfo64 = 9
		}

		[Flags]
		public enum vm_prot_t
		{
			None = 0,
			Read = 1,
			Write = 2,
			Execute = 4,
			Default = 3,
			All = 7,
			[Obsolete("Only used for memory_object_lock_request. Invalid otherwise.")]
			NoChange = 8,
			Copy = 16,
			WantsCopy = 16,
			[Obsolete("Invalid value. Indicates that other bits are to be applied as mask to actual bits.")]
			IsMask = 64,
			[Obsolete("Invalid value. Tells mprotect to not set Read. Used for execute-only.")]
			StripRead = 128,
			[Obsolete("Invalid value. Use only for mprotect.")]
			ExecuteOnly = 132
		}

		public struct VmProtFmtProxy : IDebugFormattable
		{
			public VmProtFmtProxy(OSX.vm_prot_t value)
			{
				this.value = value;
			}

			public unsafe bool TryFormatInto(global::System.Span<char> span, out int wrote)
			{
				int num = 0;
				if (this.value.Has(OSX.vm_prot_t.NoChange))
				{
					if (span.Slice(num).Length < 1)
					{
						wrote = num;
						return false;
					}
					*span[num++] = '~';
				}
				if (span.Slice(num).Length < 3)
				{
					wrote = 0;
					return false;
				}
				*span[num++] = (this.value.Has(OSX.vm_prot_t.Read) ? 'r' : '-');
				*span[num++] = (this.value.Has(OSX.vm_prot_t.Write) ? 'w' : '-');
				*span[num++] = (this.value.Has(OSX.vm_prot_t.Execute) ? 'x' : '-');
				if (this.value.Has(OSX.vm_prot_t.StripRead))
				{
					if (span.Slice(num).Length < 1)
					{
						wrote = num;
						return false;
					}
					*span[num++] = '!';
				}
				if (this.value.Has(OSX.vm_prot_t.Copy))
				{
					if (span.Slice(num).Length < 1)
					{
						wrote = num;
						return false;
					}
					*span[num++] = 'c';
				}
				if (this.value.Has(OSX.vm_prot_t.IsMask))
				{
					if (span.Slice(num).Length < " (mask)".Length)
					{
						wrote = num;
						return false;
					}
					" (mask)".AsSpan().CopyTo(span.Slice(num));
					num += " (mask)".Length;
				}
				wrote = num;
				return true;
			}

			private readonly OSX.vm_prot_t value;
		}

		[Flags]
		public enum vm_flags
		{
			Fixed = 0,
			Anywhere = 1,
			Purgable = 2,
			Chunk4GB = 4,
			RandomAddr = 8,
			NoCache = 16,
			Overwrite = 16384,
			SuperpageMask = 458752,
			SuperpageSizeAny = 65536,
			SuperpageWSize2MB = 131072,
			AliasMask = -16777216
		}

		public enum vm_inherit_t : uint
		{
			Share,
			Copy,
			None,
			DonateCopy,
			Default = 1U,
			LastValid
		}

		public enum vm_behavior_t
		{
			Default,
			Random,
			Sequential,
			ReverseSequential,
			WillNeed,
			DontNeed,
			Free,
			ZeroWiredPages,
			Reusable,
			Reuse,
			CanReuse,
			PageOut
		}

		[DebuggerDisplay("{ToString(),nq}")]
		public struct boolean_t
		{
			public boolean_t(bool value)
			{
				this.value = ((value > false) ? 1 : 0);
			}

			public static implicit operator bool(OSX.boolean_t v)
			{
				return v.value != 0;
			}

			public static implicit operator OSX.boolean_t(bool v)
			{
				return new OSX.boolean_t(v);
			}

			public static bool operator true(OSX.boolean_t v)
			{
				return v;
			}

			public static bool operator false(OSX.boolean_t v)
			{
				return !v;
			}

			[NullableContext(1)]
			public override string ToString()
			{
				if (!this)
				{
					return "false";
				}
				return "true";
			}

			private int value;
		}

		[DebuggerDisplay("{Value}")]
		public struct kern_return_t : IEquatable<OSX.kern_return_t>
		{
			public int Value
			{
				get
				{
					return this.value;
				}
			}

			public kern_return_t(int value)
			{
				this.value = value;
			}

			public static implicit operator bool(OSX.kern_return_t v)
			{
				return v.value == 0;
			}

			public static bool operator ==(OSX.kern_return_t x, OSX.kern_return_t y)
			{
				return x.value == y.value;
			}

			public static bool operator !=(OSX.kern_return_t x, OSX.kern_return_t y)
			{
				return x.value != y.value;
			}

			[NullableContext(2)]
			public override bool Equals(object obj)
			{
				if (obj is OSX.kern_return_t)
				{
					OSX.kern_return_t kern_return_t = (OSX.kern_return_t)obj;
					return this.Equals(kern_return_t);
				}
				return false;
			}

			public bool Equals(OSX.kern_return_t other)
			{
				return this.value == other.value;
			}

			public override int GetHashCode()
			{
				return global::System.HashCode.Combine<int>(this.value);
			}

			private int value;

			public static OSX.kern_return_t Success = new OSX.kern_return_t(0);

			public static OSX.kern_return_t InvalidAddress = new OSX.kern_return_t(1);

			public static OSX.kern_return_t ProtectionFailure = new OSX.kern_return_t(2);

			public static OSX.kern_return_t NoSpace = new OSX.kern_return_t(3);

			public static OSX.kern_return_t InvalidArgument = new OSX.kern_return_t(4);

			public static OSX.kern_return_t Failure = new OSX.kern_return_t(5);
		}

		[Flags]
		public enum map_prot
		{
			None = 0,
			Read = 1,
			Write = 2,
			Execute = 4
		}

		[Flags]
		public enum map_flags
		{
			Private = 2,
			Fixed = 16,
			JIT = 2048,
			Anonymous = 4096,
			Failed = 4097
		}
	}
}
