using System;

namespace Satsuma
{
	public struct Node : IEquatable<Node>
	{
		public long Id { get; private set; }

		public Node(long id)
		{
			this = default(Node);
			this.Id = id;
		}

		public static Node Invalid
		{
			get
			{
				return new Node(0L);
			}
		}

		public bool Equals(Node other)
		{
			return this.Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			return obj is Node && this.Equals((Node)obj);
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		public override string ToString()
		{
			return "#" + this.Id;
		}

		public static bool operator ==(Node a, Node b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Node a, Node b)
		{
			return !(a == b);
		}
	}
}
