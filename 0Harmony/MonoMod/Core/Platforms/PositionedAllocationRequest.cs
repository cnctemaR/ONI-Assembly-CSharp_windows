using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MonoMod.Core.Platforms
{
	internal readonly struct PositionedAllocationRequest : IEquatable<PositionedAllocationRequest>
	{
		public PositionedAllocationRequest(IntPtr Target, IntPtr LowBound, IntPtr HighBound, AllocationRequest Base)
		{
			this.Target = Target;
			this.LowBound = LowBound;
			this.HighBound = HighBound;
			this.Base = Base;
		}

		public IntPtr Target { get; set; }

		public IntPtr LowBound { get; set; }

		public IntPtr HighBound { get; set; }

		public AllocationRequest Base { get; set; }

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PositionedAllocationRequest");
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
			builder.Append("Target = ");
			builder.Append(this.Target.ToString());
			builder.Append(", LowBound = ");
			builder.Append(this.LowBound.ToString());
			builder.Append(", HighBound = ");
			builder.Append(this.HighBound.ToString());
			builder.Append(", Base = ");
			builder.Append(this.Base.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(PositionedAllocationRequest left, PositionedAllocationRequest right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(PositionedAllocationRequest left, PositionedAllocationRequest right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<IntPtr>.Default.GetHashCode(this.<Target>k__BackingField) * -1521134295 + EqualityComparer<IntPtr>.Default.GetHashCode(this.<LowBound>k__BackingField)) * -1521134295 + EqualityComparer<IntPtr>.Default.GetHashCode(this.<HighBound>k__BackingField)) * -1521134295 + EqualityComparer<AllocationRequest>.Default.GetHashCode(this.<Base>k__BackingField);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is PositionedAllocationRequest && this.Equals((PositionedAllocationRequest)obj);
		}

		[CompilerGenerated]
		public bool Equals(PositionedAllocationRequest other)
		{
			return EqualityComparer<IntPtr>.Default.Equals(this.<Target>k__BackingField, other.<Target>k__BackingField) && EqualityComparer<IntPtr>.Default.Equals(this.<LowBound>k__BackingField, other.<LowBound>k__BackingField) && EqualityComparer<IntPtr>.Default.Equals(this.<HighBound>k__BackingField, other.<HighBound>k__BackingField) && EqualityComparer<AllocationRequest>.Default.Equals(this.<Base>k__BackingField, other.<Base>k__BackingField);
		}

		[CompilerGenerated]
		public void Deconstruct(out IntPtr Target, out IntPtr LowBound, out IntPtr HighBound, out AllocationRequest Base)
		{
			Target = this.Target;
			LowBound = this.LowBound;
			HighBound = this.HighBound;
			Base = this.Base;
		}
	}
}
