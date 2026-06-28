using System;
using System.Collections;
using System.Text;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixEnvironment
	{
		private UnixEnvironment()
		{
		}

		public static string CurrentDirectory
		{
			get
			{
				return UnixDirectoryInfo.GetCurrentDirectory();
			}
			set
			{
				UnixDirectoryInfo.SetCurrentDirectory(value);
			}
		}

		public static string MachineName
		{
			get
			{
				Utsname utsname;
				if (Syscall.uname(out utsname) != 0)
				{
					throw UnixMarshal.CreateExceptionForLastError();
				}
				return utsname.nodename;
			}
			set
			{
				int num = Syscall.sethostname(value);
				UnixMarshal.ThrowExceptionForLastErrorIf(num);
			}
		}

		public static string UserName
		{
			get
			{
				return UnixUserInfo.GetRealUser().UserName;
			}
		}

		public static UnixGroupInfo RealGroup
		{
			get
			{
				return new UnixGroupInfo(UnixEnvironment.RealGroupId);
			}
		}

		public static long RealGroupId
		{
			get
			{
				return (long)((ulong)Syscall.getgid());
			}
		}

		public static UnixUserInfo RealUser
		{
			get
			{
				return new UnixUserInfo(UnixEnvironment.RealUserId);
			}
		}

		public static long RealUserId
		{
			get
			{
				return (long)((ulong)Syscall.getuid());
			}
		}

		public static UnixGroupInfo EffectiveGroup
		{
			get
			{
				return new UnixGroupInfo(UnixEnvironment.EffectiveGroupId);
			}
			set
			{
				UnixEnvironment.EffectiveGroupId = value.GroupId;
			}
		}

		public static long EffectiveGroupId
		{
			get
			{
				return (long)((ulong)Syscall.getegid());
			}
			set
			{
				Syscall.setegid(Convert.ToUInt32(value));
			}
		}

		public static UnixUserInfo EffectiveUser
		{
			get
			{
				return new UnixUserInfo(UnixEnvironment.EffectiveUserId);
			}
			set
			{
				UnixEnvironment.EffectiveUserId = value.UserId;
			}
		}

		public static long EffectiveUserId
		{
			get
			{
				return (long)((ulong)Syscall.geteuid());
			}
			set
			{
				Syscall.seteuid(Convert.ToUInt32(value));
			}
		}

		public static string Login
		{
			get
			{
				return UnixUserInfo.GetRealUser().UserName;
			}
		}

		[CLSCompliant(false)]
		public static long GetConfigurationValue(SysconfName name)
		{
			long num = Syscall.sysconf(name);
			if (num == -1L && Stdlib.GetLastError() != (Errno)0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return num;
		}

		[CLSCompliant(false)]
		public static string GetConfigurationString(ConfstrName name)
		{
			ulong num = Syscall.confstr(name, null, 0UL);
			if (num == 18446744073709551615UL)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			if (num == 0UL)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder((int)num + 1);
			num = Syscall.confstr(name, stringBuilder, num);
			if (num == 18446744073709551615UL)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return stringBuilder.ToString();
		}

		public static void SetNiceValue(int inc)
		{
			int num = Syscall.nice(inc);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public static int CreateSession()
		{
			int num = Syscall.setsid();
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			return num;
		}

		public static void SetProcessGroup()
		{
			int num = Syscall.setpgrp();
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public static int GetProcessGroup()
		{
			return Syscall.getpgrp();
		}

		public static UnixGroupInfo[] GetSupplementaryGroups()
		{
			uint[] array = UnixEnvironment._GetSupplementaryGroupIds();
			UnixGroupInfo[] array2 = new UnixGroupInfo[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = new UnixGroupInfo((long)((ulong)array[i]));
			}
			return array2;
		}

		private static uint[] _GetSupplementaryGroupIds()
		{
			int num = Syscall.getgroups(0, new uint[0]);
			if (num == -1)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			uint[] array = new uint[num];
			int num2 = Syscall.getgroups(array);
			UnixMarshal.ThrowExceptionForLastErrorIf(num2);
			return array;
		}

		public static void SetSupplementaryGroups(UnixGroupInfo[] groups)
		{
			uint[] array = new uint[groups.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Convert.ToUInt32(groups[i].GroupId);
			}
			int num = Syscall.setgroups(array);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public static long[] GetSupplementaryGroupIds()
		{
			uint[] array = UnixEnvironment._GetSupplementaryGroupIds();
			long[] array2 = new long[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = (long)((ulong)array[i]);
			}
			return array2;
		}

		public static void SetSupplementaryGroupIds(long[] list)
		{
			uint[] array = new uint[list.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Convert.ToUInt32(list[i]);
			}
			int num = Syscall.setgroups(array);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public static int GetParentProcessId()
		{
			return Syscall.getppid();
		}

		public static UnixProcess GetParentProcess()
		{
			return new UnixProcess(UnixEnvironment.GetParentProcessId());
		}

		public static string[] GetUserShells()
		{
			ArrayList arrayList = new ArrayList();
			object usershell_lock = Syscall.usershell_lock;
			lock (usershell_lock)
			{
				try
				{
					if (Syscall.setusershell() != 0)
					{
						UnixMarshal.ThrowExceptionForLastError();
					}
					string text;
					while ((text = Syscall.getusershell()) != null)
					{
						arrayList.Add(text);
					}
				}
				finally
				{
					Syscall.endusershell();
				}
			}
			return (string[])arrayList.ToArray(typeof(string));
		}
	}
}
