using System;
using System.Runtime.CompilerServices;

namespace VYaml.Parser
{
	[NullableContext(1)]
	[Nullable(0)]
	public class Anchor : IEquatable<Anchor>
	{
		public string Name { get; }

		public int Id { get; }

		public Anchor(string name, int id)
		{
			this.Name = name;
			this.Id = id;
		}

		[NullableContext(2)]
		public bool Equals(Anchor other)
		{
			return other != null && this.Id == other.Id;
		}

		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			Anchor anchor = obj as Anchor;
			return anchor != null && this.Equals(anchor);
		}

		public override int GetHashCode()
		{
			return this.Id;
		}

		public override string ToString()
		{
			return string.Format("{0} Id={1}", this.Name, this.Id);
		}
	}
}
