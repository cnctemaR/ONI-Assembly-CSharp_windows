using System;
using System.Runtime.CompilerServices;

namespace VYaml.Parser
{
	[NullableContext(1)]
	[Nullable(0)]
	public class Tag : ITokenContent
	{
		public string Handle { get; }

		public string Suffix { get; }

		public Tag(string handle, string suffix)
		{
			this.Handle = handle;
			this.Suffix = suffix;
		}

		public override string ToString()
		{
			return this.Handle + this.Suffix;
		}

		public bool Equals(string tagString)
		{
			if (tagString.Length != this.Handle.Length + this.Suffix.Length)
			{
				return false;
			}
			int num = tagString.IndexOf(this.Handle, StringComparison.Ordinal);
			return num >= 0 && tagString.IndexOf(this.Suffix, num, StringComparison.Ordinal) > 0;
		}
	}
}
