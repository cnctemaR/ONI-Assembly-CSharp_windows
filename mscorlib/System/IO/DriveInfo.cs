using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public sealed class DriveInfo : ISerializable
	{
		private DriveInfo(string path, string fstype)
		{
			this.drive_format = fstype;
			this.path = path;
		}

		public DriveInfo(string driveName)
		{
			if (!Environment.IsUnix)
			{
				if (driveName == null || driveName.Length == 0)
				{
					throw new ArgumentException("The drive name is null or empty", "driveName");
				}
				if (driveName.Length >= 2 && driveName[1] != ':')
				{
					throw new ArgumentException("Invalid drive name", "driveName");
				}
				driveName = char.ToUpperInvariant(driveName[0]).ToString() + ":\\";
			}
			foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
			{
				if (driveInfo.path == driveName)
				{
					this.path = driveInfo.path;
					this.drive_format = driveInfo.drive_format;
					this.path = driveInfo.path;
					return;
				}
			}
			throw new ArgumentException("The drive name does not exist", "driveName");
		}

		private static void GetDiskFreeSpace(string path, out ulong availableFreeSpace, out ulong totalSize, out ulong totalFreeSpace)
		{
			MonoIOError monoIOError;
			if (!DriveInfo.GetDiskFreeSpaceInternal(path, out availableFreeSpace, out totalSize, out totalFreeSpace, out monoIOError))
			{
				throw MonoIO.GetException(path, monoIOError);
			}
		}

		public long AvailableFreeSpace
		{
			get
			{
				ulong num;
				ulong num2;
				ulong num3;
				DriveInfo.GetDiskFreeSpace(this.path, out num, out num2, out num3);
				if (num <= 9223372036854775807UL)
				{
					return (long)num;
				}
				return long.MaxValue;
			}
		}

		public long TotalFreeSpace
		{
			get
			{
				ulong num;
				ulong num2;
				ulong num3;
				DriveInfo.GetDiskFreeSpace(this.path, out num, out num2, out num3);
				if (num3 <= 9223372036854775807UL)
				{
					return (long)num3;
				}
				return long.MaxValue;
			}
		}

		public long TotalSize
		{
			get
			{
				ulong num;
				ulong num2;
				ulong num3;
				DriveInfo.GetDiskFreeSpace(this.path, out num, out num2, out num3);
				if (num2 <= 9223372036854775807UL)
				{
					return (long)num2;
				}
				return long.MaxValue;
			}
		}

		[MonoTODO("Currently get only works on Mono/Unix; set not implemented")]
		public string VolumeLabel
		{
			get
			{
				return this.path;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string DriveFormat
		{
			get
			{
				return this.drive_format;
			}
		}

		public DriveType DriveType
		{
			get
			{
				return (DriveType)DriveInfo.GetDriveTypeInternal(this.path);
			}
		}

		public string Name
		{
			get
			{
				return this.path;
			}
		}

		public DirectoryInfo RootDirectory
		{
			get
			{
				return new DirectoryInfo(this.path);
			}
		}

		public bool IsReady
		{
			get
			{
				return Directory.Exists(this.Name);
			}
		}

		[MonoTODO("In windows, alldrives are 'Fixed'")]
		public static DriveInfo[] GetDrives()
		{
			string[] logicalDrives = Environment.GetLogicalDrives();
			DriveInfo[] array = new DriveInfo[logicalDrives.Length];
			int num = 0;
			foreach (string text in logicalDrives)
			{
				array[num++] = new DriveInfo(text, DriveInfo.GetDriveFormat(text));
			}
			return array;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return this.Name;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetDiskFreeSpaceInternal(string pathName, out ulong freeBytesAvail, out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes, out MonoIOError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetDriveTypeInternal(string rootPathName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetDriveFormat(string rootPathName);

		private string drive_format;

		private string path;
	}
}
