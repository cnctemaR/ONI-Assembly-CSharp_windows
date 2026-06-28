using System;

namespace Mono.Unix.Native
{
	[Map("struct pollfd")]
	public struct Pollfd : IEquatable<Pollfd>
	{
		public override int GetHashCode()
		{
			return this.events.GetHashCode() ^ this.revents.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != base.GetType())
			{
				return false;
			}
			Pollfd pollfd = (Pollfd)obj;
			return pollfd.events == this.events && pollfd.revents == this.revents;
		}

		public bool Equals(Pollfd value)
		{
			return value.events == this.events && value.revents == this.revents;
		}

		public static bool operator ==(Pollfd lhs, Pollfd rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Pollfd lhs, Pollfd rhs)
		{
			return !lhs.Equals(rhs);
		}

		public int fd;

		[CLSCompliant(false)]
		public PollEvents events;

		[CLSCompliant(false)]
		public PollEvents revents;
	}
}
