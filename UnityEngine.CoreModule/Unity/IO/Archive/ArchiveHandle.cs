using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.IO.Archive
{
	[NativeHeader("Runtime/VirtualFileSystem/ArchiveFileSystem/ArchiveFileHandle.h")]
	[RequiredByNativeCode]
	public struct ArchiveHandle
	{
		public ArchiveStatus Status
		{
			get
			{
				this.ThrowIfInvalid();
				return ArchiveFileInterface.Archive_GetStatus(this);
			}
		}

		public JobHandle JobHandle
		{
			get
			{
				this.ThrowIfInvalid();
				return ArchiveFileInterface.Archive_GetJobHandle(this);
			}
		}

		public JobHandle Unmount()
		{
			this.ThrowIfInvalid();
			return ArchiveFileInterface.Archive_UnmountAsync(this);
		}

		private void ThrowIfInvalid()
		{
			bool flag = !ArchiveFileInterface.Archive_IsValid(this);
			if (flag)
			{
				throw new InvalidOperationException("The archive has already been unmounted.");
			}
		}

		public string GetMountPath()
		{
			this.ThrowIfInvalid();
			return ArchiveFileInterface.Archive_GetMountPath(this);
		}

		public CompressionType Compression
		{
			get
			{
				this.ThrowIfInvalid();
				return ArchiveFileInterface.Archive_GetCompression(this);
			}
		}

		public bool IsStreamed
		{
			get
			{
				this.ThrowIfInvalid();
				return ArchiveFileInterface.Archive_IsStreamed(this);
			}
		}

		public ArchiveFileInfo[] GetFileInfo()
		{
			this.ThrowIfInvalid();
			return ArchiveFileInterface.Archive_GetFileInfo(this);
		}

		internal ulong Handle;
	}
}
