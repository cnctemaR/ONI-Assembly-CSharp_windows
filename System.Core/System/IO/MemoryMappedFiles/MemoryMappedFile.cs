using System;
using Microsoft.Win32.SafeHandles;

namespace System.IO.MemoryMappedFiles
{
	public class MemoryMappedFile : IDisposable
	{
		public static MemoryMappedFile CreateFromFile(string path)
		{
			return MemoryMappedFile.CreateFromFile(path, FileMode.Open, null, 0L, MemoryMappedFileAccess.ReadWrite);
		}

		public static MemoryMappedFile CreateFromFile(string path, FileMode mode)
		{
			long num = 0L;
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("path");
			}
			if (mode == FileMode.Append)
			{
				throw new ArgumentException("mode");
			}
			IntPtr intPtr = MemoryMapImpl.OpenFile(path, mode, null, out num, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None);
			return new MemoryMappedFile
			{
				handle = new SafeMemoryMappedFileHandle(intPtr, true)
			};
		}

		public static MemoryMappedFile CreateFromFile(string path, FileMode mode, string mapName)
		{
			return MemoryMappedFile.CreateFromFile(path, mode, mapName, 0L, MemoryMappedFileAccess.ReadWrite);
		}

		public static MemoryMappedFile CreateFromFile(string path, FileMode mode, string mapName, long capacity)
		{
			return MemoryMappedFile.CreateFromFile(path, mode, mapName, capacity, MemoryMappedFileAccess.ReadWrite);
		}

		public static MemoryMappedFile CreateFromFile(string path, FileMode mode, string mapName, long capacity, MemoryMappedFileAccess access)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("path");
			}
			if (mapName != null && mapName.Length == 0)
			{
				throw new ArgumentException("mapName");
			}
			if (mode == FileMode.Append)
			{
				throw new ArgumentException("mode");
			}
			if (capacity < 0L)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			IntPtr intPtr = MemoryMapImpl.OpenFile(path, mode, mapName, out capacity, access, MemoryMappedFileOptions.None);
			return new MemoryMappedFile
			{
				handle = new SafeMemoryMappedFileHandle(intPtr, true)
			};
		}

		public static MemoryMappedFile CreateFromFile(FileStream fileStream, string mapName, long capacity, MemoryMappedFileAccess access, HandleInheritability inheritability, bool leaveOpen)
		{
			if (fileStream == null)
			{
				throw new ArgumentNullException("fileStream");
			}
			if (mapName != null && mapName.Length == 0)
			{
				throw new ArgumentException("mapName");
			}
			if ((!MonoUtil.IsUnix && capacity == 0L && fileStream.Length == 0L) || capacity > fileStream.Length)
			{
				throw new ArgumentException("capacity");
			}
			IntPtr intPtr = MemoryMapImpl.OpenHandle(fileStream.SafeFileHandle.DangerousGetHandle(), mapName, out capacity, access, MemoryMappedFileOptions.None);
			MemoryMapImpl.ConfigureHandleInheritability(intPtr, inheritability);
			return new MemoryMappedFile
			{
				handle = new SafeMemoryMappedFileHandle(intPtr, true),
				stream = fileStream,
				keepOpen = leaveOpen
			};
		}

		[global::System.MonoLimitation("memoryMappedFileSecurity is currently ignored")]
		public static MemoryMappedFile CreateFromFile(FileStream fileStream, string mapName, long capacity, MemoryMappedFileAccess access, MemoryMappedFileSecurity memoryMappedFileSecurity, HandleInheritability inheritability, bool leaveOpen)
		{
			if (fileStream == null)
			{
				throw new ArgumentNullException("fileStream");
			}
			if (mapName != null && mapName.Length == 0)
			{
				throw new ArgumentException("mapName");
			}
			if ((!MonoUtil.IsUnix && capacity == 0L && fileStream.Length == 0L) || capacity > fileStream.Length)
			{
				throw new ArgumentException("capacity");
			}
			IntPtr intPtr = MemoryMapImpl.OpenHandle(fileStream.SafeFileHandle.DangerousGetHandle(), mapName, out capacity, access, MemoryMappedFileOptions.None);
			MemoryMapImpl.ConfigureHandleInheritability(intPtr, inheritability);
			return new MemoryMappedFile
			{
				handle = new SafeMemoryMappedFileHandle(intPtr, true),
				stream = fileStream,
				keepOpen = leaveOpen
			};
		}

		private static MemoryMappedFile CoreShmCreate(string mapName, long capacity, MemoryMappedFileAccess access, MemoryMappedFileOptions options, MemoryMappedFileSecurity memoryMappedFileSecurity, HandleInheritability inheritability, FileMode mode)
		{
			if (mapName != null && mapName.Length == 0)
			{
				throw new ArgumentException("mapName");
			}
			if (capacity < 0L)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			IntPtr intPtr = MemoryMapImpl.OpenFile(null, mode, mapName, out capacity, access, options);
			return new MemoryMappedFile
			{
				handle = new SafeMemoryMappedFileHandle(intPtr, true)
			};
		}

		[global::System.MonoLimitation("Named mappings scope is process local")]
		public static MemoryMappedFile CreateNew(string mapName, long capacity)
		{
			return MemoryMappedFile.CreateNew(mapName, capacity, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, null, HandleInheritability.None);
		}

		[global::System.MonoLimitation("Named mappings scope is process local")]
		public static MemoryMappedFile CreateNew(string mapName, long capacity, MemoryMappedFileAccess access)
		{
			return MemoryMappedFile.CreateNew(mapName, capacity, access, MemoryMappedFileOptions.None, null, HandleInheritability.None);
		}

		[global::System.MonoLimitation("Named mappings scope is process local; options is ignored")]
		public static MemoryMappedFile CreateNew(string mapName, long capacity, MemoryMappedFileAccess access, MemoryMappedFileOptions options, HandleInheritability inheritability)
		{
			return MemoryMappedFile.CreateNew(mapName, capacity, access, options, null, inheritability);
		}

		[global::System.MonoLimitation("Named mappings scope is process local; options and memoryMappedFileSecurity are ignored")]
		public static MemoryMappedFile CreateNew(string mapName, long capacity, MemoryMappedFileAccess access, MemoryMappedFileOptions options, MemoryMappedFileSecurity memoryMappedFileSecurity, HandleInheritability inheritability)
		{
			return MemoryMappedFile.CoreShmCreate(mapName, capacity, access, options, memoryMappedFileSecurity, inheritability, FileMode.CreateNew);
		}

		[global::System.MonoLimitation("Named mappings scope is process local")]
		public static MemoryMappedFile CreateOrOpen(string mapName, long capacity)
		{
			return MemoryMappedFile.CreateOrOpen(mapName, capacity, MemoryMappedFileAccess.ReadWrite);
		}

		[global::System.MonoLimitation("Named mappings scope is process local")]
		public static MemoryMappedFile CreateOrOpen(string mapName, long capacity, MemoryMappedFileAccess access)
		{
			return MemoryMappedFile.CreateOrOpen(mapName, capacity, access, MemoryMappedFileOptions.None, null, HandleInheritability.None);
		}

		[global::System.MonoLimitation("Named mappings scope is process local")]
		public static MemoryMappedFile CreateOrOpen(string mapName, long capacity, MemoryMappedFileAccess access, MemoryMappedFileOptions options, HandleInheritability inheritability)
		{
			return MemoryMappedFile.CreateOrOpen(mapName, capacity, access, options, null, inheritability);
		}

		[global::System.MonoLimitation("Named mappings scope is process local")]
		public static MemoryMappedFile CreateOrOpen(string mapName, long capacity, MemoryMappedFileAccess access, MemoryMappedFileOptions options, MemoryMappedFileSecurity memoryMappedFileSecurity, HandleInheritability inheritability)
		{
			return MemoryMappedFile.CoreShmCreate(mapName, capacity, access, options, memoryMappedFileSecurity, inheritability, FileMode.OpenOrCreate);
		}

		[global::System.MonoLimitation("Named mappings scope is process local")]
		public static MemoryMappedFile OpenExisting(string mapName)
		{
			return MemoryMappedFile.OpenExisting(mapName, MemoryMappedFileRights.ReadWrite);
		}

		[global::System.MonoLimitation("Named mappings scope is process local")]
		public static MemoryMappedFile OpenExisting(string mapName, MemoryMappedFileRights desiredAccessRights)
		{
			return MemoryMappedFile.OpenExisting(mapName, desiredAccessRights, HandleInheritability.None);
		}

		[global::System.MonoLimitation("Named mappings scope is process local")]
		public static MemoryMappedFile OpenExisting(string mapName, MemoryMappedFileRights desiredAccessRights, HandleInheritability inheritability)
		{
			return MemoryMappedFile.CoreShmCreate(mapName, 0L, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, null, inheritability, FileMode.Open);
		}

		public MemoryMappedViewStream CreateViewStream()
		{
			return this.CreateViewStream(0L, 0L);
		}

		public MemoryMappedViewStream CreateViewStream(long offset, long size)
		{
			return this.CreateViewStream(offset, size, MemoryMappedFileAccess.ReadWrite);
		}

		public MemoryMappedViewStream CreateViewStream(long offset, long size, MemoryMappedFileAccess access)
		{
			return new MemoryMappedViewStream(MemoryMappedView.Create(this.handle.DangerousGetHandle(), offset, size, access));
		}

		public MemoryMappedViewAccessor CreateViewAccessor()
		{
			return this.CreateViewAccessor(0L, 0L);
		}

		public MemoryMappedViewAccessor CreateViewAccessor(long offset, long size)
		{
			return this.CreateViewAccessor(offset, size, MemoryMappedFileAccess.ReadWrite);
		}

		public MemoryMappedViewAccessor CreateViewAccessor(long offset, long size, MemoryMappedFileAccess access)
		{
			return new MemoryMappedViewAccessor(MemoryMappedView.Create(this.handle.DangerousGetHandle(), offset, size, access));
		}

		private MemoryMappedFile()
		{
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.stream != null)
			{
				if (!this.keepOpen)
				{
					this.stream.Close();
				}
				this.stream = null;
			}
			if (this.handle != null)
			{
				this.handle.Dispose();
				this.handle = null;
			}
		}

		[global::System.MonoTODO]
		public MemoryMappedFileSecurity GetAccessControl()
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public void SetAccessControl(MemoryMappedFileSecurity memoryMappedFileSecurity)
		{
			throw new NotImplementedException();
		}

		public SafeMemoryMappedFileHandle SafeMemoryMappedFileHandle
		{
			get
			{
				return this.handle;
			}
		}

		internal static FileAccess GetFileAccess(MemoryMappedFileAccess access)
		{
			if (access == MemoryMappedFileAccess.Read)
			{
				return FileAccess.Read;
			}
			if (access == MemoryMappedFileAccess.Write)
			{
				return FileAccess.Write;
			}
			if (access == MemoryMappedFileAccess.ReadWrite)
			{
				return FileAccess.ReadWrite;
			}
			if (access == MemoryMappedFileAccess.CopyOnWrite)
			{
				return FileAccess.ReadWrite;
			}
			if (access == MemoryMappedFileAccess.ReadExecute)
			{
				return FileAccess.Read;
			}
			if (access == MemoryMappedFileAccess.ReadWriteExecute)
			{
				return FileAccess.ReadWrite;
			}
			throw new ArgumentOutOfRangeException("access");
		}

		private FileStream stream;

		private bool keepOpen;

		private SafeMemoryMappedFileHandle handle;
	}
}
