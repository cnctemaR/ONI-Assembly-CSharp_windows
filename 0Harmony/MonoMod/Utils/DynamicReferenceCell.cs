using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MonoMod.Utils
{
	internal struct DynamicReferenceCell : IEquatable<DynamicReferenceCell>
	{
		public int Index { readonly get; internal set; }

		public int Hash { readonly get; internal set; }

		public DynamicReferenceCell(int idx, int hash)
		{
			this.Index = idx;
			this.Hash = hash;
		}

		[CompilerGenerated]
		public override readonly string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("DynamicReferenceCell");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Index = ");
			builder.Append(this.Index.ToString());
			builder.Append(", Hash = ");
			builder.Append(this.Hash.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(DynamicReferenceCell left, DynamicReferenceCell right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(DynamicReferenceCell left, DynamicReferenceCell right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override readonly int GetHashCode()
		{
			return EqualityComparer<int>.Default.GetHashCode(this.<Index>k__BackingField) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<Hash>k__BackingField);
		}

		[CompilerGenerated]
		public override readonly bool Equals(object obj)
		{
			return obj is DynamicReferenceCell && this.Equals((DynamicReferenceCell)obj);
		}

		[CompilerGenerated]
		public readonly bool Equals(DynamicReferenceCell other)
		{
			return EqualityComparer<int>.Default.Equals(this.<Index>k__BackingField, other.<Index>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<Hash>k__BackingField, other.<Hash>k__BackingField);
		}
	}
}
