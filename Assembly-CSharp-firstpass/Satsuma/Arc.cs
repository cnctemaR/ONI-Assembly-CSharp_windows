using System;

namespace Satsuma
{
	public struct Arc : IEquatable<Arc>
	{
		public long Id { readonly get; private set; }

		public Arc(long id)
		{
			this = default(Arc);
			this.Id = id;
		}

		public static Arc Invalid
		{
			get
			{
				return new Arc(0L);
			}
		}

		public bool Equals(Arc other)
		{
			return this.Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			return obj is Arc && this.Equals((Arc)obj);
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		public override string ToString()
		{
			return "|" + this.Id.ToString();
		}

		public static bool operator ==(Arc a, Arc b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Arc a, Arc b)
		{
			return !(a == b);
		}
	}
}
