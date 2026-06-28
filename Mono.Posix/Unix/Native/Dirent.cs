using System;

namespace Mono.Unix.Native
{
	public sealed class Dirent : IEquatable<Dirent>
	{
		public override int GetHashCode()
		{
			return this.d_ino.GetHashCode() ^ this.d_off.GetHashCode() ^ this.d_reclen.GetHashCode() ^ this.d_type.GetHashCode() ^ this.d_name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || base.GetType() != obj.GetType())
			{
				return false;
			}
			Dirent dirent = (Dirent)obj;
			return this.Equals(dirent);
		}

		public bool Equals(Dirent value)
		{
			return !(value == null) && (value.d_ino == this.d_ino && value.d_off == this.d_off && value.d_reclen == this.d_reclen && value.d_type == this.d_type) && value.d_name == this.d_name;
		}

		public override string ToString()
		{
			return this.d_name;
		}

		public static bool operator ==(Dirent lhs, Dirent rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(Dirent lhs, Dirent rhs)
		{
			return !object.Equals(lhs, rhs);
		}

		[CLSCompliant(false)]
		public ulong d_ino;

		public long d_off;

		[CLSCompliant(false)]
		public ushort d_reclen;

		public byte d_type;

		public string d_name;
	}
}
