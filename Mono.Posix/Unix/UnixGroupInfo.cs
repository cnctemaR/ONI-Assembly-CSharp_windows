using System;
using System.Collections;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixGroupInfo
	{
		public UnixGroupInfo(string group)
		{
			this.group = new Group();
			Group group2;
			if (Syscall.getgrnam_r(group, this.group, out group2) != 0 || group2 == null)
			{
				throw new ArgumentException(Locale.GetText("invalid group name"), "group");
			}
		}

		public UnixGroupInfo(long group)
		{
			this.group = new Group();
			Group group2;
			if (Syscall.getgrgid_r(Convert.ToUInt32(group), this.group, out group2) != 0 || group2 == null)
			{
				throw new ArgumentException(Locale.GetText("invalid group id"), "group");
			}
		}

		public UnixGroupInfo(Group group)
		{
			this.group = UnixGroupInfo.CopyGroup(group);
		}

		private static Group CopyGroup(Group group)
		{
			return new Group
			{
				gr_gid = group.gr_gid,
				gr_mem = group.gr_mem,
				gr_name = group.gr_name,
				gr_passwd = group.gr_passwd
			};
		}

		public string GroupName
		{
			get
			{
				return this.group.gr_name;
			}
		}

		public string Password
		{
			get
			{
				return this.group.gr_passwd;
			}
		}

		public long GroupId
		{
			get
			{
				return (long)((ulong)this.group.gr_gid);
			}
		}

		public UnixUserInfo[] GetMembers()
		{
			ArrayList arrayList = new ArrayList(this.group.gr_mem.Length);
			for (int i = 0; i < this.group.gr_mem.Length; i++)
			{
				try
				{
					arrayList.Add(new UnixUserInfo(this.group.gr_mem[i]));
				}
				catch (ArgumentException)
				{
				}
			}
			return (UnixUserInfo[])arrayList.ToArray(typeof(UnixUserInfo));
		}

		public string[] GetMemberNames()
		{
			return (string[])this.group.gr_mem.Clone();
		}

		public override int GetHashCode()
		{
			return this.group.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && base.GetType() == obj.GetType() && this.group.Equals(((UnixGroupInfo)obj).group);
		}

		public override string ToString()
		{
			return this.group.ToString();
		}

		public Group ToGroup()
		{
			return UnixGroupInfo.CopyGroup(this.group);
		}

		public static UnixGroupInfo[] GetLocalGroups()
		{
			ArrayList arrayList = new ArrayList();
			object grp_lock = Syscall.grp_lock;
			lock (grp_lock)
			{
				if (Syscall.setgrent() != 0)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				try
				{
					Group group;
					while ((group = Syscall.getgrent()) != null)
					{
						arrayList.Add(new UnixGroupInfo(group));
					}
					if (Stdlib.GetLastError() != (Errno)0)
					{
						UnixMarshal.ThrowExceptionForLastError();
					}
				}
				finally
				{
					Syscall.endgrent();
				}
			}
			return (UnixGroupInfo[])arrayList.ToArray(typeof(UnixGroupInfo));
		}

		private Group group;
	}
}
