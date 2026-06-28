using System;

namespace Mono.Unix.Native
{
	public sealed class Fstab : IEquatable<Fstab>
	{
		public override int GetHashCode()
		{
			return this.fs_spec.GetHashCode() ^ this.fs_file.GetHashCode() ^ this.fs_vfstype.GetHashCode() ^ this.fs_mntops.GetHashCode() ^ this.fs_type.GetHashCode() ^ this.fs_freq ^ this.fs_passno;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || base.GetType() != obj.GetType())
			{
				return false;
			}
			Fstab fstab = (Fstab)obj;
			return this.Equals(fstab);
		}

		public bool Equals(Fstab value)
		{
			return !(value == null) && (value.fs_spec == this.fs_spec && value.fs_file == this.fs_file && value.fs_vfstype == this.fs_vfstype && value.fs_mntops == this.fs_mntops && value.fs_type == this.fs_type && value.fs_freq == this.fs_freq) && value.fs_passno == this.fs_passno;
		}

		public override string ToString()
		{
			return this.fs_spec;
		}

		public static bool operator ==(Fstab lhs, Fstab rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(Fstab lhs, Fstab rhs)
		{
			return !object.Equals(lhs, rhs);
		}

		public string fs_spec;

		public string fs_file;

		public string fs_vfstype;

		public string fs_mntops;

		public string fs_type;

		public int fs_freq;

		public int fs_passno;
	}
}
