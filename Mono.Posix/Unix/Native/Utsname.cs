using System;

namespace Mono.Unix.Native
{
	public sealed class Utsname : IEquatable<Utsname>
	{
		public override int GetHashCode()
		{
			return this.sysname.GetHashCode() ^ this.nodename.GetHashCode() ^ this.release.GetHashCode() ^ this.version.GetHashCode() ^ this.machine.GetHashCode() ^ this.domainname.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || base.GetType() != obj.GetType())
			{
				return false;
			}
			Utsname utsname = (Utsname)obj;
			return this.Equals(utsname);
		}

		public bool Equals(Utsname value)
		{
			return value.sysname == this.sysname && value.nodename == this.nodename && value.release == this.release && value.version == this.version && value.machine == this.machine && value.domainname == this.domainname;
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3} {4}", new object[] { this.sysname, this.nodename, this.release, this.version, this.machine });
		}

		public static bool operator ==(Utsname lhs, Utsname rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(Utsname lhs, Utsname rhs)
		{
			return !object.Equals(lhs, rhs);
		}

		public string sysname;

		public string nodename;

		public string release;

		public string version;

		public string machine;

		public string domainname;
	}
}
