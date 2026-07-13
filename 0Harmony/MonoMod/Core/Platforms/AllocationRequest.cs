using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MonoMod.Core.Platforms
{
	internal readonly struct AllocationRequest : IEquatable<AllocationRequest>
	{
		public AllocationRequest(int Size)
		{
			this.Executable = false;
			this.Size = Size;
			this.Alignment = 8;
		}

		public int Size { get; set; }

		public int Alignment { get; set; }

		public bool Executable { get; set; }

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("AllocationRequest");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Size = ");
			builder.Append(this.Size.ToString());
			builder.Append(", Alignment = ");
			builder.Append(this.Alignment.ToString());
			builder.Append(", Executable = ");
			builder.Append(this.Executable.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(AllocationRequest left, AllocationRequest right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(AllocationRequest left, AllocationRequest right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (EqualityComparer<int>.Default.GetHashCode(this.<Size>k__BackingField) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<Alignment>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Executable>k__BackingField);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is AllocationRequest && this.Equals((AllocationRequest)obj);
		}

		[CompilerGenerated]
		public bool Equals(AllocationRequest other)
		{
			return EqualityComparer<int>.Default.Equals(this.<Size>k__BackingField, other.<Size>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<Alignment>k__BackingField, other.<Alignment>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Executable>k__BackingField, other.<Executable>k__BackingField);
		}

		[CompilerGenerated]
		public void Deconstruct(out int Size)
		{
			Size = this.Size;
		}
	}
}
