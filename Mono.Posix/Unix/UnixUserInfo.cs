using System;
using System.Collections;
using System.Text;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixUserInfo
	{
		public UnixUserInfo(string user)
		{
			this.passwd = new Passwd();
			Passwd passwd;
			if (Syscall.getpwnam_r(user, this.passwd, out passwd) != 0 || passwd == null)
			{
				throw new ArgumentException(Locale.GetText("invalid username"), "user");
			}
		}

		[CLSCompliant(false)]
		public UnixUserInfo(uint user)
		{
			this.passwd = new Passwd();
			Passwd passwd;
			if (Syscall.getpwuid_r(user, this.passwd, out passwd) != 0 || passwd == null)
			{
				throw new ArgumentException(Locale.GetText("invalid user id"), "user");
			}
		}

		public UnixUserInfo(long user)
		{
			this.passwd = new Passwd();
			Passwd passwd;
			if (Syscall.getpwuid_r(Convert.ToUInt32(user), this.passwd, out passwd) != 0 || passwd == null)
			{
				throw new ArgumentException(Locale.GetText("invalid user id"), "user");
			}
		}

		public UnixUserInfo(Passwd passwd)
		{
			this.passwd = UnixUserInfo.CopyPasswd(passwd);
		}

		private static Passwd CopyPasswd(Passwd pw)
		{
			return new Passwd
			{
				pw_name = pw.pw_name,
				pw_passwd = pw.pw_passwd,
				pw_uid = pw.pw_uid,
				pw_gid = pw.pw_gid,
				pw_gecos = pw.pw_gecos,
				pw_dir = pw.pw_dir,
				pw_shell = pw.pw_shell
			};
		}

		public string UserName
		{
			get
			{
				return this.passwd.pw_name;
			}
		}

		public string Password
		{
			get
			{
				return this.passwd.pw_passwd;
			}
		}

		public long UserId
		{
			get
			{
				return (long)((ulong)this.passwd.pw_uid);
			}
		}

		public UnixGroupInfo Group
		{
			get
			{
				return new UnixGroupInfo((long)((ulong)this.passwd.pw_gid));
			}
		}

		public long GroupId
		{
			get
			{
				return (long)((ulong)this.passwd.pw_gid);
			}
		}

		public string GroupName
		{
			get
			{
				return this.Group.GroupName;
			}
		}

		public string RealName
		{
			get
			{
				return this.passwd.pw_gecos;
			}
		}

		public string HomeDirectory
		{
			get
			{
				return this.passwd.pw_dir;
			}
		}

		public string ShellProgram
		{
			get
			{
				return this.passwd.pw_shell;
			}
		}

		public override int GetHashCode()
		{
			return this.passwd.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.passwd.Equals(((UnixUserInfo)obj).passwd);
		}

		public override string ToString()
		{
			return this.passwd.ToString();
		}

		public static UnixUserInfo GetRealUser()
		{
			return new UnixUserInfo(UnixUserInfo.GetRealUserId());
		}

		public static long GetRealUserId()
		{
			return (long)((ulong)Syscall.getuid());
		}

		public static string GetLoginName()
		{
			StringBuilder stringBuilder = new StringBuilder(4);
			int num;
			do
			{
				stringBuilder.Capacity *= 2;
				num = Syscall.getlogin_r(stringBuilder, (ulong)((long)stringBuilder.Capacity));
			}
			while (num == -1 && Stdlib.GetLastError() == Errno.ERANGE);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			return stringBuilder.ToString();
		}

		public Passwd ToPasswd()
		{
			return UnixUserInfo.CopyPasswd(this.passwd);
		}

		public static UnixUserInfo[] GetLocalUsers()
		{
			ArrayList arrayList = new ArrayList();
			object pwd_lock = Syscall.pwd_lock;
			lock (pwd_lock)
			{
				if (Syscall.setpwent() != 0)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				try
				{
					Passwd passwd;
					while ((passwd = Syscall.getpwent()) != null)
					{
						arrayList.Add(new UnixUserInfo(passwd));
					}
					if (Stdlib.GetLastError() != (Errno)0)
					{
						UnixMarshal.ThrowExceptionForLastError();
					}
				}
				finally
				{
					Syscall.endpwent();
				}
			}
			return (UnixUserInfo[])arrayList.ToArray(typeof(UnixUserInfo));
		}

		private Passwd passwd;
	}
}
