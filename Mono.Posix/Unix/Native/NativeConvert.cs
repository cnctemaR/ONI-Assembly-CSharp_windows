using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	public sealed class NativeConvert
	{
		private NativeConvert()
		{
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromRealTimeSignum")]
		private static extern int FromRealTimeSignum(int offset, out int rval);

		public static int FromRealTimeSignum(RealTimeSignum sig)
		{
			int num;
			if (NativeConvert.FromRealTimeSignum(sig.Offset, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(sig.Offset);
			}
			return num;
		}

		public static RealTimeSignum ToRealTimeSignum(int offset)
		{
			return new RealTimeSignum(offset);
		}

		public static FilePermissions FromOctalPermissionString(string value)
		{
			uint num = Convert.ToUInt32(value, 8);
			return NativeConvert.ToFilePermissions(num);
		}

		public static string ToOctalPermissionString(FilePermissions value)
		{
			string text = Convert.ToString((int)(value & ~FilePermissions.S_IFMT), 8);
			return new string('0', 4 - text.Length) + text;
		}

		public static FilePermissions FromUnixPermissionString(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length != 9 && value.Length != 10)
			{
				throw new ArgumentException("value", "must contain 9 or 10 characters");
			}
			int num = 0;
			FilePermissions filePermissions = (FilePermissions)0U;
			if (value.Length == 10)
			{
				filePermissions |= NativeConvert.GetUnixPermissionDevice(value[num]);
				num++;
			}
			filePermissions |= NativeConvert.GetUnixPermissionGroup(value[num++], FilePermissions.S_IRUSR, value[num++], FilePermissions.S_IWUSR, value[num++], FilePermissions.S_IXUSR, 's', 'S', FilePermissions.S_ISUID);
			filePermissions |= NativeConvert.GetUnixPermissionGroup(value[num++], FilePermissions.S_IRGRP, value[num++], FilePermissions.S_IWGRP, value[num++], FilePermissions.S_IXGRP, 's', 'S', FilePermissions.S_ISGID);
			return filePermissions | NativeConvert.GetUnixPermissionGroup(value[num++], FilePermissions.S_IROTH, value[num++], FilePermissions.S_IWOTH, value[num++], FilePermissions.S_IXOTH, 't', 'T', FilePermissions.S_ISVTX);
		}

		private static FilePermissions GetUnixPermissionDevice(char value)
		{
			switch (value)
			{
			case 'p':
				return FilePermissions.S_IFIFO;
			default:
				switch (value)
				{
				case 'b':
					return FilePermissions.S_IFBLK;
				case 'c':
					return FilePermissions.S_IFCHR;
				case 'd':
					return FilePermissions.S_IFDIR;
				default:
					if (value == '-')
					{
						return FilePermissions.S_IFREG;
					}
					if (value != 'l')
					{
						throw new ArgumentException("value", "invalid device specification: " + value);
					}
					return FilePermissions.S_IFLNK;
				}
				break;
			case 's':
				return FilePermissions.S_IFSOCK;
			}
		}

		private static FilePermissions GetUnixPermissionGroup(char read, FilePermissions readb, char write, FilePermissions writeb, char exec, FilePermissions execb, char xboth, char xbitonly, FilePermissions xbit)
		{
			FilePermissions filePermissions = (FilePermissions)0U;
			if (read == 'r')
			{
				filePermissions |= readb;
			}
			if (write == 'w')
			{
				filePermissions |= writeb;
			}
			if (exec == 'x')
			{
				filePermissions |= execb;
			}
			else if (exec == xbitonly)
			{
				filePermissions |= xbit;
			}
			else if (exec == xboth)
			{
				filePermissions |= execb | xbit;
			}
			return filePermissions;
		}

		public static string ToUnixPermissionString(FilePermissions value)
		{
			char[] array = new char[] { '-', '-', '-', '-', '-', '-', '-', '-', '-', '-' };
			bool flag = true;
			FilePermissions filePermissions = value & FilePermissions.S_IFMT;
			if (filePermissions != FilePermissions.S_IFIFO)
			{
				if (filePermissions != FilePermissions.S_IFCHR)
				{
					if (filePermissions != FilePermissions.S_IFDIR)
					{
						if (filePermissions != FilePermissions.S_IFBLK)
						{
							if (filePermissions != FilePermissions.S_IFREG)
							{
								if (filePermissions != FilePermissions.S_IFLNK)
								{
									if (filePermissions != FilePermissions.S_IFSOCK)
									{
										flag = false;
									}
									else
									{
										array[0] = 's';
									}
								}
								else
								{
									array[0] = 'l';
								}
							}
							else
							{
								array[0] = '-';
							}
						}
						else
						{
							array[0] = 'b';
						}
					}
					else
					{
						array[0] = 'd';
					}
				}
				else
				{
					array[0] = 'c';
				}
			}
			else
			{
				array[0] = 'p';
			}
			NativeConvert.SetUnixPermissionGroup(value, array, 1, FilePermissions.S_IRUSR, FilePermissions.S_IWUSR, FilePermissions.S_IXUSR, 's', 'S', FilePermissions.S_ISUID);
			NativeConvert.SetUnixPermissionGroup(value, array, 4, FilePermissions.S_IRGRP, FilePermissions.S_IWGRP, FilePermissions.S_IXGRP, 's', 'S', FilePermissions.S_ISGID);
			NativeConvert.SetUnixPermissionGroup(value, array, 7, FilePermissions.S_IROTH, FilePermissions.S_IWOTH, FilePermissions.S_IXOTH, 't', 'T', FilePermissions.S_ISVTX);
			return (!flag) ? new string(array, 1, 9) : new string(array);
		}

		private static void SetUnixPermissionGroup(FilePermissions value, char[] access, int index, FilePermissions read, FilePermissions write, FilePermissions exec, char both, char setonly, FilePermissions setxbit)
		{
			if (UnixFileSystemInfo.IsSet(value, read))
			{
				access[index] = 'r';
			}
			if (UnixFileSystemInfo.IsSet(value, write))
			{
				access[index + 1] = 'w';
			}
			access[index + 2] = NativeConvert.GetSymbolicMode(value, exec, both, setonly, setxbit);
		}

		private static char GetSymbolicMode(FilePermissions value, FilePermissions xbit, char both, char setonly, FilePermissions setxbit)
		{
			bool flag = UnixFileSystemInfo.IsSet(value, xbit);
			bool flag2 = UnixFileSystemInfo.IsSet(value, setxbit);
			if (flag && flag2)
			{
				return both;
			}
			if (flag2)
			{
				return setonly;
			}
			if (flag)
			{
				return 'x';
			}
			return '-';
		}

		public static DateTime ToDateTime(long time)
		{
			return NativeConvert.FromTimeT(time);
		}

		public static long FromDateTime(DateTime time)
		{
			return NativeConvert.ToTimeT(time);
		}

		public static DateTime FromTimeT(long time)
		{
			return NativeConvert.LocalUnixEpoch.AddSeconds((double)time + NativeConvert.LocalUtcOffset.TotalSeconds);
		}

		public static long ToTimeT(DateTime time)
		{
			return (long)(time.Subtract(NativeConvert.LocalUnixEpoch) - NativeConvert.LocalUtcOffset).TotalSeconds;
		}

		public static OpenFlags ToOpenFlags(FileMode mode, FileAccess access)
		{
			OpenFlags openFlags = OpenFlags.O_RDONLY;
			switch (mode)
			{
			case FileMode.CreateNew:
				openFlags = OpenFlags.O_CREAT | OpenFlags.O_EXCL;
				break;
			case FileMode.Create:
				openFlags = OpenFlags.O_CREAT | OpenFlags.O_TRUNC;
				break;
			case FileMode.Open:
				break;
			case FileMode.OpenOrCreate:
				openFlags = OpenFlags.O_CREAT;
				break;
			case FileMode.Truncate:
				openFlags = OpenFlags.O_TRUNC;
				break;
			case FileMode.Append:
				openFlags = OpenFlags.O_APPEND;
				break;
			default:
				throw new ArgumentException(Locale.GetText("Unsupported mode value"), "mode");
			}
			int num;
			if (NativeConvert.TryFromOpenFlags(OpenFlags.O_LARGEFILE, out num))
			{
				openFlags |= OpenFlags.O_LARGEFILE;
			}
			switch (access)
			{
			case FileAccess.Read:
				openFlags |= OpenFlags.O_RDONLY;
				break;
			case FileAccess.Write:
				openFlags |= OpenFlags.O_WRONLY;
				break;
			case FileAccess.ReadWrite:
				openFlags |= OpenFlags.O_RDWR;
				break;
			default:
				throw new ArgumentOutOfRangeException(Locale.GetText("Unsupported access value"), "access");
			}
			return openFlags;
		}

		public static string ToFopenMode(FileAccess access)
		{
			switch (access)
			{
			case FileAccess.Read:
				return "rb";
			case FileAccess.Write:
				return "wb";
			case FileAccess.ReadWrite:
				return "r+b";
			default:
				throw new ArgumentOutOfRangeException("access");
			}
		}

		public static string ToFopenMode(FileMode mode)
		{
			switch (mode)
			{
			case FileMode.CreateNew:
			case FileMode.Create:
				return "w+b";
			case FileMode.Open:
			case FileMode.OpenOrCreate:
				return "r+b";
			case FileMode.Truncate:
				return "w+b";
			case FileMode.Append:
				return "a+b";
			default:
				throw new ArgumentOutOfRangeException("mode");
			}
		}

		public static string ToFopenMode(FileMode mode, FileAccess access)
		{
			int num = -1;
			int num2 = -1;
			switch (mode)
			{
			case FileMode.CreateNew:
				num = 0;
				break;
			case FileMode.Create:
				num = 1;
				break;
			case FileMode.Open:
				num = 2;
				break;
			case FileMode.OpenOrCreate:
				num = 3;
				break;
			case FileMode.Truncate:
				num = 4;
				break;
			case FileMode.Append:
				num = 5;
				break;
			}
			switch (access)
			{
			case FileAccess.Read:
				num2 = 0;
				break;
			case FileAccess.Write:
				num2 = 1;
				break;
			case FileAccess.ReadWrite:
				num2 = 2;
				break;
			}
			if (num == -1)
			{
				throw new ArgumentOutOfRangeException("mode");
			}
			if (num2 == -1)
			{
				throw new ArgumentOutOfRangeException("access");
			}
			string text = NativeConvert.fopen_modes[num][num2];
			if (text[0] != 'r' && text[0] != 'w' && text[0] != 'a')
			{
				throw new ArgumentException(text);
			}
			return text;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromStatvfs")]
		private static extern int FromStatvfs(ref Statvfs source, IntPtr destination);

		public static bool TryCopy(ref Statvfs source, IntPtr destination)
		{
			return NativeConvert.FromStatvfs(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToStatvfs")]
		private static extern int ToStatvfs(IntPtr source, out Statvfs destination);

		public static bool TryCopy(IntPtr source, out Statvfs destination)
		{
			return NativeConvert.ToStatvfs(source, out destination) == 0;
		}

		private static void ThrowArgumentException(object value)
		{
			throw new ArgumentOutOfRangeException("value", value, Locale.GetText("Current platform doesn't support this value."));
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromAccessModes")]
		private static extern int FromAccessModes(AccessModes value, out int rval);

		public static bool TryFromAccessModes(AccessModes value, out int rval)
		{
			return NativeConvert.FromAccessModes(value, out rval) == 0;
		}

		public static int FromAccessModes(AccessModes value)
		{
			int num;
			if (NativeConvert.FromAccessModes(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToAccessModes")]
		private static extern int ToAccessModes(int value, out AccessModes rval);

		public static bool TryToAccessModes(int value, out AccessModes rval)
		{
			return NativeConvert.ToAccessModes(value, out rval) == 0;
		}

		public static AccessModes ToAccessModes(int value)
		{
			AccessModes accessModes;
			if (NativeConvert.ToAccessModes(value, out accessModes) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return accessModes;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromConfstrName")]
		private static extern int FromConfstrName(ConfstrName value, out int rval);

		public static bool TryFromConfstrName(ConfstrName value, out int rval)
		{
			return NativeConvert.FromConfstrName(value, out rval) == 0;
		}

		public static int FromConfstrName(ConfstrName value)
		{
			int num;
			if (NativeConvert.FromConfstrName(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToConfstrName")]
		private static extern int ToConfstrName(int value, out ConfstrName rval);

		public static bool TryToConfstrName(int value, out ConfstrName rval)
		{
			return NativeConvert.ToConfstrName(value, out rval) == 0;
		}

		public static ConfstrName ToConfstrName(int value)
		{
			ConfstrName confstrName;
			if (NativeConvert.ToConfstrName(value, out confstrName) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return confstrName;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromDirectoryNotifyFlags")]
		private static extern int FromDirectoryNotifyFlags(DirectoryNotifyFlags value, out int rval);

		public static bool TryFromDirectoryNotifyFlags(DirectoryNotifyFlags value, out int rval)
		{
			return NativeConvert.FromDirectoryNotifyFlags(value, out rval) == 0;
		}

		public static int FromDirectoryNotifyFlags(DirectoryNotifyFlags value)
		{
			int num;
			if (NativeConvert.FromDirectoryNotifyFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToDirectoryNotifyFlags")]
		private static extern int ToDirectoryNotifyFlags(int value, out DirectoryNotifyFlags rval);

		public static bool TryToDirectoryNotifyFlags(int value, out DirectoryNotifyFlags rval)
		{
			return NativeConvert.ToDirectoryNotifyFlags(value, out rval) == 0;
		}

		public static DirectoryNotifyFlags ToDirectoryNotifyFlags(int value)
		{
			DirectoryNotifyFlags directoryNotifyFlags;
			if (NativeConvert.ToDirectoryNotifyFlags(value, out directoryNotifyFlags) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return directoryNotifyFlags;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromErrno")]
		private static extern int FromErrno(Errno value, out int rval);

		public static bool TryFromErrno(Errno value, out int rval)
		{
			return NativeConvert.FromErrno(value, out rval) == 0;
		}

		public static int FromErrno(Errno value)
		{
			int num;
			if (NativeConvert.FromErrno(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToErrno")]
		private static extern int ToErrno(int value, out Errno rval);

		public static bool TryToErrno(int value, out Errno rval)
		{
			return NativeConvert.ToErrno(value, out rval) == 0;
		}

		public static Errno ToErrno(int value)
		{
			Errno errno;
			if (NativeConvert.ToErrno(value, out errno) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return errno;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromFcntlCommand")]
		private static extern int FromFcntlCommand(FcntlCommand value, out int rval);

		public static bool TryFromFcntlCommand(FcntlCommand value, out int rval)
		{
			return NativeConvert.FromFcntlCommand(value, out rval) == 0;
		}

		public static int FromFcntlCommand(FcntlCommand value)
		{
			int num;
			if (NativeConvert.FromFcntlCommand(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToFcntlCommand")]
		private static extern int ToFcntlCommand(int value, out FcntlCommand rval);

		public static bool TryToFcntlCommand(int value, out FcntlCommand rval)
		{
			return NativeConvert.ToFcntlCommand(value, out rval) == 0;
		}

		public static FcntlCommand ToFcntlCommand(int value)
		{
			FcntlCommand fcntlCommand;
			if (NativeConvert.ToFcntlCommand(value, out fcntlCommand) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return fcntlCommand;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromFilePermissions")]
		private static extern int FromFilePermissions(FilePermissions value, out uint rval);

		public static bool TryFromFilePermissions(FilePermissions value, out uint rval)
		{
			return NativeConvert.FromFilePermissions(value, out rval) == 0;
		}

		public static uint FromFilePermissions(FilePermissions value)
		{
			uint num;
			if (NativeConvert.FromFilePermissions(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToFilePermissions")]
		private static extern int ToFilePermissions(uint value, out FilePermissions rval);

		public static bool TryToFilePermissions(uint value, out FilePermissions rval)
		{
			return NativeConvert.ToFilePermissions(value, out rval) == 0;
		}

		public static FilePermissions ToFilePermissions(uint value)
		{
			FilePermissions filePermissions;
			if (NativeConvert.ToFilePermissions(value, out filePermissions) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return filePermissions;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromFlock")]
		private static extern int FromFlock(ref Flock source, IntPtr destination);

		public static bool TryCopy(ref Flock source, IntPtr destination)
		{
			return NativeConvert.FromFlock(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToFlock")]
		private static extern int ToFlock(IntPtr source, out Flock destination);

		public static bool TryCopy(IntPtr source, out Flock destination)
		{
			return NativeConvert.ToFlock(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromLockType")]
		private static extern int FromLockType(LockType value, out short rval);

		public static bool TryFromLockType(LockType value, out short rval)
		{
			return NativeConvert.FromLockType(value, out rval) == 0;
		}

		public static short FromLockType(LockType value)
		{
			short num;
			if (NativeConvert.FromLockType(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToLockType")]
		private static extern int ToLockType(short value, out LockType rval);

		public static bool TryToLockType(short value, out LockType rval)
		{
			return NativeConvert.ToLockType(value, out rval) == 0;
		}

		public static LockType ToLockType(short value)
		{
			LockType lockType;
			if (NativeConvert.ToLockType(value, out lockType) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return lockType;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromLockfCommand")]
		private static extern int FromLockfCommand(LockfCommand value, out int rval);

		public static bool TryFromLockfCommand(LockfCommand value, out int rval)
		{
			return NativeConvert.FromLockfCommand(value, out rval) == 0;
		}

		public static int FromLockfCommand(LockfCommand value)
		{
			int num;
			if (NativeConvert.FromLockfCommand(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToLockfCommand")]
		private static extern int ToLockfCommand(int value, out LockfCommand rval);

		public static bool TryToLockfCommand(int value, out LockfCommand rval)
		{
			return NativeConvert.ToLockfCommand(value, out rval) == 0;
		}

		public static LockfCommand ToLockfCommand(int value)
		{
			LockfCommand lockfCommand;
			if (NativeConvert.ToLockfCommand(value, out lockfCommand) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return lockfCommand;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMlockallFlags")]
		private static extern int FromMlockallFlags(MlockallFlags value, out int rval);

		public static bool TryFromMlockallFlags(MlockallFlags value, out int rval)
		{
			return NativeConvert.FromMlockallFlags(value, out rval) == 0;
		}

		public static int FromMlockallFlags(MlockallFlags value)
		{
			int num;
			if (NativeConvert.FromMlockallFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMlockallFlags")]
		private static extern int ToMlockallFlags(int value, out MlockallFlags rval);

		public static bool TryToMlockallFlags(int value, out MlockallFlags rval)
		{
			return NativeConvert.ToMlockallFlags(value, out rval) == 0;
		}

		public static MlockallFlags ToMlockallFlags(int value)
		{
			MlockallFlags mlockallFlags;
			if (NativeConvert.ToMlockallFlags(value, out mlockallFlags) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return mlockallFlags;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMmapFlags")]
		private static extern int FromMmapFlags(MmapFlags value, out int rval);

		public static bool TryFromMmapFlags(MmapFlags value, out int rval)
		{
			return NativeConvert.FromMmapFlags(value, out rval) == 0;
		}

		public static int FromMmapFlags(MmapFlags value)
		{
			int num;
			if (NativeConvert.FromMmapFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMmapFlags")]
		private static extern int ToMmapFlags(int value, out MmapFlags rval);

		public static bool TryToMmapFlags(int value, out MmapFlags rval)
		{
			return NativeConvert.ToMmapFlags(value, out rval) == 0;
		}

		public static MmapFlags ToMmapFlags(int value)
		{
			MmapFlags mmapFlags;
			if (NativeConvert.ToMmapFlags(value, out mmapFlags) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return mmapFlags;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMmapProts")]
		private static extern int FromMmapProts(MmapProts value, out int rval);

		public static bool TryFromMmapProts(MmapProts value, out int rval)
		{
			return NativeConvert.FromMmapProts(value, out rval) == 0;
		}

		public static int FromMmapProts(MmapProts value)
		{
			int num;
			if (NativeConvert.FromMmapProts(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMmapProts")]
		private static extern int ToMmapProts(int value, out MmapProts rval);

		public static bool TryToMmapProts(int value, out MmapProts rval)
		{
			return NativeConvert.ToMmapProts(value, out rval) == 0;
		}

		public static MmapProts ToMmapProts(int value)
		{
			MmapProts mmapProts;
			if (NativeConvert.ToMmapProts(value, out mmapProts) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return mmapProts;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMountFlags")]
		private static extern int FromMountFlags(MountFlags value, out ulong rval);

		public static bool TryFromMountFlags(MountFlags value, out ulong rval)
		{
			return NativeConvert.FromMountFlags(value, out rval) == 0;
		}

		public static ulong FromMountFlags(MountFlags value)
		{
			ulong num;
			if (NativeConvert.FromMountFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMountFlags")]
		private static extern int ToMountFlags(ulong value, out MountFlags rval);

		public static bool TryToMountFlags(ulong value, out MountFlags rval)
		{
			return NativeConvert.ToMountFlags(value, out rval) == 0;
		}

		public static MountFlags ToMountFlags(ulong value)
		{
			MountFlags mountFlags;
			if (NativeConvert.ToMountFlags(value, out mountFlags) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return mountFlags;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMremapFlags")]
		private static extern int FromMremapFlags(MremapFlags value, out ulong rval);

		public static bool TryFromMremapFlags(MremapFlags value, out ulong rval)
		{
			return NativeConvert.FromMremapFlags(value, out rval) == 0;
		}

		public static ulong FromMremapFlags(MremapFlags value)
		{
			ulong num;
			if (NativeConvert.FromMremapFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMremapFlags")]
		private static extern int ToMremapFlags(ulong value, out MremapFlags rval);

		public static bool TryToMremapFlags(ulong value, out MremapFlags rval)
		{
			return NativeConvert.ToMremapFlags(value, out rval) == 0;
		}

		public static MremapFlags ToMremapFlags(ulong value)
		{
			MremapFlags mremapFlags;
			if (NativeConvert.ToMremapFlags(value, out mremapFlags) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return mremapFlags;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMsyncFlags")]
		private static extern int FromMsyncFlags(MsyncFlags value, out int rval);

		public static bool TryFromMsyncFlags(MsyncFlags value, out int rval)
		{
			return NativeConvert.FromMsyncFlags(value, out rval) == 0;
		}

		public static int FromMsyncFlags(MsyncFlags value)
		{
			int num;
			if (NativeConvert.FromMsyncFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMsyncFlags")]
		private static extern int ToMsyncFlags(int value, out MsyncFlags rval);

		public static bool TryToMsyncFlags(int value, out MsyncFlags rval)
		{
			return NativeConvert.ToMsyncFlags(value, out rval) == 0;
		}

		public static MsyncFlags ToMsyncFlags(int value)
		{
			MsyncFlags msyncFlags;
			if (NativeConvert.ToMsyncFlags(value, out msyncFlags) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return msyncFlags;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromOpenFlags")]
		private static extern int FromOpenFlags(OpenFlags value, out int rval);

		public static bool TryFromOpenFlags(OpenFlags value, out int rval)
		{
			return NativeConvert.FromOpenFlags(value, out rval) == 0;
		}

		public static int FromOpenFlags(OpenFlags value)
		{
			int num;
			if (NativeConvert.FromOpenFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToOpenFlags")]
		private static extern int ToOpenFlags(int value, out OpenFlags rval);

		public static bool TryToOpenFlags(int value, out OpenFlags rval)
		{
			return NativeConvert.ToOpenFlags(value, out rval) == 0;
		}

		public static OpenFlags ToOpenFlags(int value)
		{
			OpenFlags openFlags;
			if (NativeConvert.ToOpenFlags(value, out openFlags) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return openFlags;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromPathconfName")]
		private static extern int FromPathconfName(PathconfName value, out int rval);

		public static bool TryFromPathconfName(PathconfName value, out int rval)
		{
			return NativeConvert.FromPathconfName(value, out rval) == 0;
		}

		public static int FromPathconfName(PathconfName value)
		{
			int num;
			if (NativeConvert.FromPathconfName(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToPathconfName")]
		private static extern int ToPathconfName(int value, out PathconfName rval);

		public static bool TryToPathconfName(int value, out PathconfName rval)
		{
			return NativeConvert.ToPathconfName(value, out rval) == 0;
		}

		public static PathconfName ToPathconfName(int value)
		{
			PathconfName pathconfName;
			if (NativeConvert.ToPathconfName(value, out pathconfName) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return pathconfName;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromPollEvents")]
		private static extern int FromPollEvents(PollEvents value, out short rval);

		public static bool TryFromPollEvents(PollEvents value, out short rval)
		{
			return NativeConvert.FromPollEvents(value, out rval) == 0;
		}

		public static short FromPollEvents(PollEvents value)
		{
			short num;
			if (NativeConvert.FromPollEvents(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToPollEvents")]
		private static extern int ToPollEvents(short value, out PollEvents rval);

		public static bool TryToPollEvents(short value, out PollEvents rval)
		{
			return NativeConvert.ToPollEvents(value, out rval) == 0;
		}

		public static PollEvents ToPollEvents(short value)
		{
			PollEvents pollEvents;
			if (NativeConvert.ToPollEvents(value, out pollEvents) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return pollEvents;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromPollfd")]
		private static extern int FromPollfd(ref Pollfd source, IntPtr destination);

		public static bool TryCopy(ref Pollfd source, IntPtr destination)
		{
			return NativeConvert.FromPollfd(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToPollfd")]
		private static extern int ToPollfd(IntPtr source, out Pollfd destination);

		public static bool TryCopy(IntPtr source, out Pollfd destination)
		{
			return NativeConvert.ToPollfd(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromPosixFadviseAdvice")]
		private static extern int FromPosixFadviseAdvice(PosixFadviseAdvice value, out int rval);

		public static bool TryFromPosixFadviseAdvice(PosixFadviseAdvice value, out int rval)
		{
			return NativeConvert.FromPosixFadviseAdvice(value, out rval) == 0;
		}

		public static int FromPosixFadviseAdvice(PosixFadviseAdvice value)
		{
			int num;
			if (NativeConvert.FromPosixFadviseAdvice(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToPosixFadviseAdvice")]
		private static extern int ToPosixFadviseAdvice(int value, out PosixFadviseAdvice rval);

		public static bool TryToPosixFadviseAdvice(int value, out PosixFadviseAdvice rval)
		{
			return NativeConvert.ToPosixFadviseAdvice(value, out rval) == 0;
		}

		public static PosixFadviseAdvice ToPosixFadviseAdvice(int value)
		{
			PosixFadviseAdvice posixFadviseAdvice;
			if (NativeConvert.ToPosixFadviseAdvice(value, out posixFadviseAdvice) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return posixFadviseAdvice;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromPosixMadviseAdvice")]
		private static extern int FromPosixMadviseAdvice(PosixMadviseAdvice value, out int rval);

		public static bool TryFromPosixMadviseAdvice(PosixMadviseAdvice value, out int rval)
		{
			return NativeConvert.FromPosixMadviseAdvice(value, out rval) == 0;
		}

		public static int FromPosixMadviseAdvice(PosixMadviseAdvice value)
		{
			int num;
			if (NativeConvert.FromPosixMadviseAdvice(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToPosixMadviseAdvice")]
		private static extern int ToPosixMadviseAdvice(int value, out PosixMadviseAdvice rval);

		public static bool TryToPosixMadviseAdvice(int value, out PosixMadviseAdvice rval)
		{
			return NativeConvert.ToPosixMadviseAdvice(value, out rval) == 0;
		}

		public static PosixMadviseAdvice ToPosixMadviseAdvice(int value)
		{
			PosixMadviseAdvice posixMadviseAdvice;
			if (NativeConvert.ToPosixMadviseAdvice(value, out posixMadviseAdvice) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return posixMadviseAdvice;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSeekFlags")]
		private static extern int FromSeekFlags(SeekFlags value, out short rval);

		public static bool TryFromSeekFlags(SeekFlags value, out short rval)
		{
			return NativeConvert.FromSeekFlags(value, out rval) == 0;
		}

		public static short FromSeekFlags(SeekFlags value)
		{
			short num;
			if (NativeConvert.FromSeekFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSeekFlags")]
		private static extern int ToSeekFlags(short value, out SeekFlags rval);

		public static bool TryToSeekFlags(short value, out SeekFlags rval)
		{
			return NativeConvert.ToSeekFlags(value, out rval) == 0;
		}

		public static SeekFlags ToSeekFlags(short value)
		{
			SeekFlags seekFlags;
			if (NativeConvert.ToSeekFlags(value, out seekFlags) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return seekFlags;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSignum")]
		private static extern int FromSignum(Signum value, out int rval);

		public static bool TryFromSignum(Signum value, out int rval)
		{
			return NativeConvert.FromSignum(value, out rval) == 0;
		}

		public static int FromSignum(Signum value)
		{
			int num;
			if (NativeConvert.FromSignum(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSignum")]
		private static extern int ToSignum(int value, out Signum rval);

		public static bool TryToSignum(int value, out Signum rval)
		{
			return NativeConvert.ToSignum(value, out rval) == 0;
		}

		public static Signum ToSignum(int value)
		{
			Signum signum;
			if (NativeConvert.ToSignum(value, out signum) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return signum;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromStat")]
		private static extern int FromStat(ref Stat source, IntPtr destination);

		public static bool TryCopy(ref Stat source, IntPtr destination)
		{
			return NativeConvert.FromStat(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToStat")]
		private static extern int ToStat(IntPtr source, out Stat destination);

		public static bool TryCopy(IntPtr source, out Stat destination)
		{
			return NativeConvert.ToStat(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSysconfName")]
		private static extern int FromSysconfName(SysconfName value, out int rval);

		public static bool TryFromSysconfName(SysconfName value, out int rval)
		{
			return NativeConvert.FromSysconfName(value, out rval) == 0;
		}

		public static int FromSysconfName(SysconfName value)
		{
			int num;
			if (NativeConvert.FromSysconfName(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSysconfName")]
		private static extern int ToSysconfName(int value, out SysconfName rval);

		public static bool TryToSysconfName(int value, out SysconfName rval)
		{
			return NativeConvert.ToSysconfName(value, out rval) == 0;
		}

		public static SysconfName ToSysconfName(int value)
		{
			SysconfName sysconfName;
			if (NativeConvert.ToSysconfName(value, out sysconfName) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return sysconfName;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSyslogFacility")]
		private static extern int FromSyslogFacility(SyslogFacility value, out int rval);

		public static bool TryFromSyslogFacility(SyslogFacility value, out int rval)
		{
			return NativeConvert.FromSyslogFacility(value, out rval) == 0;
		}

		public static int FromSyslogFacility(SyslogFacility value)
		{
			int num;
			if (NativeConvert.FromSyslogFacility(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSyslogFacility")]
		private static extern int ToSyslogFacility(int value, out SyslogFacility rval);

		public static bool TryToSyslogFacility(int value, out SyslogFacility rval)
		{
			return NativeConvert.ToSyslogFacility(value, out rval) == 0;
		}

		public static SyslogFacility ToSyslogFacility(int value)
		{
			SyslogFacility syslogFacility;
			if (NativeConvert.ToSyslogFacility(value, out syslogFacility) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return syslogFacility;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSyslogLevel")]
		private static extern int FromSyslogLevel(SyslogLevel value, out int rval);

		public static bool TryFromSyslogLevel(SyslogLevel value, out int rval)
		{
			return NativeConvert.FromSyslogLevel(value, out rval) == 0;
		}

		public static int FromSyslogLevel(SyslogLevel value)
		{
			int num;
			if (NativeConvert.FromSyslogLevel(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSyslogLevel")]
		private static extern int ToSyslogLevel(int value, out SyslogLevel rval);

		public static bool TryToSyslogLevel(int value, out SyslogLevel rval)
		{
			return NativeConvert.ToSyslogLevel(value, out rval) == 0;
		}

		public static SyslogLevel ToSyslogLevel(int value)
		{
			SyslogLevel syslogLevel;
			if (NativeConvert.ToSyslogLevel(value, out syslogLevel) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return syslogLevel;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSyslogOptions")]
		private static extern int FromSyslogOptions(SyslogOptions value, out int rval);

		public static bool TryFromSyslogOptions(SyslogOptions value, out int rval)
		{
			return NativeConvert.FromSyslogOptions(value, out rval) == 0;
		}

		public static int FromSyslogOptions(SyslogOptions value)
		{
			int num;
			if (NativeConvert.FromSyslogOptions(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSyslogOptions")]
		private static extern int ToSyslogOptions(int value, out SyslogOptions rval);

		public static bool TryToSyslogOptions(int value, out SyslogOptions rval)
		{
			return NativeConvert.ToSyslogOptions(value, out rval) == 0;
		}

		public static SyslogOptions ToSyslogOptions(int value)
		{
			SyslogOptions syslogOptions;
			if (NativeConvert.ToSyslogOptions(value, out syslogOptions) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return syslogOptions;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromTimespec")]
		private static extern int FromTimespec(ref Timespec source, IntPtr destination);

		public static bool TryCopy(ref Timespec source, IntPtr destination)
		{
			return NativeConvert.FromTimespec(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToTimespec")]
		private static extern int ToTimespec(IntPtr source, out Timespec destination);

		public static bool TryCopy(IntPtr source, out Timespec destination)
		{
			return NativeConvert.ToTimespec(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromTimeval")]
		private static extern int FromTimeval(ref Timeval source, IntPtr destination);

		public static bool TryCopy(ref Timeval source, IntPtr destination)
		{
			return NativeConvert.FromTimeval(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToTimeval")]
		private static extern int ToTimeval(IntPtr source, out Timeval destination);

		public static bool TryCopy(IntPtr source, out Timeval destination)
		{
			return NativeConvert.ToTimeval(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromTimezone")]
		private static extern int FromTimezone(ref Timezone source, IntPtr destination);

		public static bool TryCopy(ref Timezone source, IntPtr destination)
		{
			return NativeConvert.FromTimezone(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToTimezone")]
		private static extern int ToTimezone(IntPtr source, out Timezone destination);

		public static bool TryCopy(IntPtr source, out Timezone destination)
		{
			return NativeConvert.ToTimezone(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromUtimbuf")]
		private static extern int FromUtimbuf(ref Utimbuf source, IntPtr destination);

		public static bool TryCopy(ref Utimbuf source, IntPtr destination)
		{
			return NativeConvert.FromUtimbuf(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToUtimbuf")]
		private static extern int ToUtimbuf(IntPtr source, out Utimbuf destination);

		public static bool TryCopy(IntPtr source, out Utimbuf destination)
		{
			return NativeConvert.ToUtimbuf(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromWaitOptions")]
		private static extern int FromWaitOptions(WaitOptions value, out int rval);

		public static bool TryFromWaitOptions(WaitOptions value, out int rval)
		{
			return NativeConvert.FromWaitOptions(value, out rval) == 0;
		}

		public static int FromWaitOptions(WaitOptions value)
		{
			int num;
			if (NativeConvert.FromWaitOptions(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToWaitOptions")]
		private static extern int ToWaitOptions(int value, out WaitOptions rval);

		public static bool TryToWaitOptions(int value, out WaitOptions rval)
		{
			return NativeConvert.ToWaitOptions(value, out rval) == 0;
		}

		public static WaitOptions ToWaitOptions(int value)
		{
			WaitOptions waitOptions;
			if (NativeConvert.ToWaitOptions(value, out waitOptions) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return waitOptions;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromXattrFlags")]
		private static extern int FromXattrFlags(XattrFlags value, out int rval);

		public static bool TryFromXattrFlags(XattrFlags value, out int rval)
		{
			return NativeConvert.FromXattrFlags(value, out rval) == 0;
		}

		public static int FromXattrFlags(XattrFlags value)
		{
			int num;
			if (NativeConvert.FromXattrFlags(value, out num) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToXattrFlags")]
		private static extern int ToXattrFlags(int value, out XattrFlags rval);

		public static bool TryToXattrFlags(int value, out XattrFlags rval)
		{
			return NativeConvert.ToXattrFlags(value, out rval) == 0;
		}

		public static XattrFlags ToXattrFlags(int value)
		{
			XattrFlags xattrFlags;
			if (NativeConvert.ToXattrFlags(value, out xattrFlags) == -1)
			{
				NativeConvert.ThrowArgumentException(value);
			}
			return xattrFlags;
		}

		private const string LIB = "MonoPosixHelper";

		public static readonly DateTime LocalUnixEpoch = new DateTime(1970, 1, 1);

		public static readonly TimeSpan LocalUtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.UtcNow);

		private static readonly string[][] fopen_modes = new string[][]
		{
			new string[] { "Can't Read+Create", "wb", "w+b" },
			new string[] { "Can't Read+Create", "wb", "w+b" },
			new string[] { "rb", "wb", "r+b" },
			new string[] { "rb", "wb", "r+b" },
			new string[] { "Cannot Truncate and Read", "wb", "w+b" },
			new string[] { "Cannot Append and Read", "ab", "a+b" }
		};
	}
}
