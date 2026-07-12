using System;
using System.Runtime.CompilerServices;

namespace System.IO.MemoryMappedFiles
{
	internal static class MemoryMapImpl
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern IntPtr OpenFileInternal(char* path, int path_length, FileMode mode, char* mapName, int mapName_length, out long capacity, MemoryMappedFileAccess access, MemoryMappedFileOptions options, out int error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern IntPtr OpenHandleInternal(IntPtr handle, char* mapName, int mapName_length, out long capacity, MemoryMappedFileAccess access, MemoryMappedFileOptions options, out int error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CloseMapping(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Flush(IntPtr file_handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ConfigureHandleInheritability(IntPtr handle, HandleInheritability inheritability);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Unmap(IntPtr mmap_handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int MapInternal(IntPtr handle, long offset, ref long size, MemoryMappedFileAccess access, out IntPtr mmap_handle, out IntPtr base_address);

		internal static void Map(IntPtr handle, long offset, ref long size, MemoryMappedFileAccess access, out IntPtr mmap_handle, out IntPtr base_address)
		{
			int num = MemoryMapImpl.MapInternal(handle, offset, ref size, access, out mmap_handle, out base_address);
			if (num != 0)
			{
				throw MemoryMapImpl.CreateException(num, "<none>");
			}
		}

		private static Exception CreateException(int error, string path)
		{
			switch (error)
			{
			case 1:
				return new ArgumentException("A positive capacity must be specified for a Memory Mapped File backed by an empty file.");
			case 2:
				return new ArgumentOutOfRangeException("capacity", "The capacity may not be smaller than the file size.");
			case 3:
				return new FileNotFoundException(path);
			case 4:
				return new IOException("The file already exists");
			case 5:
				return new PathTooLongException();
			case 6:
				return new IOException("Could not open file");
			case 7:
				return new ArgumentException("Capacity must be bigger than zero for non-file mappings");
			case 8:
				return new ArgumentException("Invalid FileMode value.");
			case 9:
				return new IOException("Could not map file");
			case 10:
				return new UnauthorizedAccessException("Access to the path is denied.");
			case 11:
				return new ArgumentOutOfRangeException("capacity", "The capacity cannot be greater than the size of the system's logical address space.");
			default:
				return new IOException("Failed with unknown error code " + error.ToString());
			}
		}

		private static int StringLength(string a)
		{
			if (a == null)
			{
				return 0;
			}
			return a.Length;
		}

		private static void CheckString(string name, string value)
		{
			if (value != null && value.IndexOf('\0') >= 0)
			{
				throw new ArgumentException("String must not contain embedded NULs.", name);
			}
		}

		internal unsafe static IntPtr OpenFile(string path, FileMode mode, string mapName, out long capacity, MemoryMappedFileAccess access, MemoryMappedFileOptions options)
		{
			MemoryMapImpl.CheckString("path", path);
			MemoryMapImpl.CheckString("mapName", mapName);
			char* ptr = path;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = mapName;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			int num = 0;
			IntPtr intPtr = MemoryMapImpl.OpenFileInternal(ptr, MemoryMapImpl.StringLength(path), mode, ptr2, MemoryMapImpl.StringLength(mapName), out capacity, access, options, out num);
			if (num != 0)
			{
				throw MemoryMapImpl.CreateException(num, path);
			}
			return intPtr;
		}

		internal unsafe static IntPtr OpenHandle(IntPtr handle, string mapName, out long capacity, MemoryMappedFileAccess access, MemoryMappedFileOptions options)
		{
			MemoryMapImpl.CheckString("mapName", mapName);
			char* ptr = mapName;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			int num = 0;
			IntPtr intPtr = MemoryMapImpl.OpenHandleInternal(handle, ptr, MemoryMapImpl.StringLength(mapName), out capacity, access, options, out num);
			if (num != 0)
			{
				throw MemoryMapImpl.CreateException(num, "<none>");
			}
			return intPtr;
		}
	}
}
