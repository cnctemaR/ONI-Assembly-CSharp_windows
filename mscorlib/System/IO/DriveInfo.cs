using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public sealed class DriveInfo : ISerializable
	{
		private DriveInfo(DriveInfo._DriveType _drive_type, string path, string fstype)
		{
			this._drive_type = _drive_type;
			this.drive_format = fstype;
			this.path = path;
		}

		public DriveInfo(string driveName)
		{
			DriveInfo[] drives = DriveInfo.GetDrives();
			foreach (DriveInfo driveInfo in drives)
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

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
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
				return (long)((num <= 9223372036854775807UL) ? num : 9223372036854775807UL);
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
				return (long)((num3 <= 9223372036854775807UL) ? num3 : 9223372036854775807UL);
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
				return (long)((num2 <= 9223372036854775807UL) ? num2 : 9223372036854775807UL);
			}
		}

		[MonoTODO("Currently get only works on Mono/Unix; set not implemented")]
		public string VolumeLabel
		{
			get
			{
				if (this._drive_type != DriveInfo._DriveType.Windows)
				{
					return this.path;
				}
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

		[MonoTODO("It always returns true")]
		public bool IsReady
		{
			get
			{
				return this._drive_type == DriveInfo._DriveType.Windows || true;
			}
		}

		private static StreamReader TryOpen(string name)
		{
			if (File.Exists(name))
			{
				return new StreamReader(name, Encoding.ASCII);
			}
			return null;
		}

		private static DriveInfo[] LinuxGetDrives()
		{
			DriveInfo[] array;
			using (StreamReader streamReader = DriveInfo.TryOpen("/proc/mounts"))
			{
				ArrayList arrayList = new ArrayList();
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					if (!text.StartsWith("rootfs"))
					{
						int num = text.IndexOf(' ');
						if (num != -1)
						{
							string text2 = text.Substring(num + 1);
							num = text2.IndexOf(' ');
							if (num != -1)
							{
								string text3 = text2.Substring(0, num);
								text2 = text2.Substring(num + 1);
								num = text2.IndexOf(' ');
								if (num != -1)
								{
									string text4 = text2.Substring(0, num);
									arrayList.Add(new DriveInfo(DriveInfo._DriveType.Linux, text3, text4));
								}
							}
						}
					}
				}
				array = (DriveInfo[])arrayList.ToArray(typeof(DriveInfo));
			}
			return array;
		}

		private static DriveInfo[] UnixGetDrives()
		{
			DriveInfo[] array = null;
			try
			{
				using (StreamReader streamReader = DriveInfo.TryOpen("/proc/sys/kernel/ostype"))
				{
					if (streamReader != null)
					{
						string text = streamReader.ReadLine();
						if (text == "Linux")
						{
							array = DriveInfo.LinuxGetDrives();
						}
					}
				}
				if (array != null)
				{
					return array;
				}
			}
			catch (Exception)
			{
			}
			return new DriveInfo[]
			{
				new DriveInfo(DriveInfo._DriveType.GenericUnix, "/", "unixfs")
			};
		}

		private static DriveInfo[] WindowsGetDrives()
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Currently only implemented on Mono/Linux")]
		public static DriveInfo[] GetDrives()
		{
			int platform = (int)Environment.Platform;
			if (platform == 4 || platform == 128 || platform == 6)
			{
				return DriveInfo.UnixGetDrives();
			}
			return DriveInfo.WindowsGetDrives();
		}

		public override string ToString()
		{
			return this.Name;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetDiskFreeSpaceInternal(string pathName, out ulong freeBytesAvail, out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes, out MonoIOError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetDriveTypeInternal(string rootPathName);

		private DriveInfo._DriveType _drive_type;

		private string drive_format;

		private string path;

		private enum _DriveType
		{
			GenericUnix,
			Linux,
			Windows
		}
	}
}
