using System;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public struct UnixPipes : IEquatable<UnixPipes>
	{
		public UnixPipes(UnixStream reading, UnixStream writing)
		{
			this.Reading = reading;
			this.Writing = writing;
		}

		public static UnixPipes CreatePipes()
		{
			int num;
			int num2;
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.pipe(out num, out num2));
			return new UnixPipes(new UnixStream(num), new UnixStream(num2));
		}

		public override bool Equals(object value)
		{
			if (value == null || value.GetType() != base.GetType())
			{
				return false;
			}
			UnixPipes unixPipes = (UnixPipes)value;
			return this.Reading.Handle == unixPipes.Reading.Handle && this.Writing.Handle == unixPipes.Writing.Handle;
		}

		public bool Equals(UnixPipes value)
		{
			return this.Reading.Handle == value.Reading.Handle && this.Writing.Handle == value.Writing.Handle;
		}

		public override int GetHashCode()
		{
			return this.Reading.Handle.GetHashCode() ^ this.Writing.Handle.GetHashCode();
		}

		public static bool operator ==(UnixPipes lhs, UnixPipes rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(UnixPipes lhs, UnixPipes rhs)
		{
			return !lhs.Equals(rhs);
		}

		public UnixStream Reading;

		public UnixStream Writing;
	}
}
