using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MonoMod.Core
{
	[CLSCompliant(true)]
	internal readonly struct CreateNativeDetourRequest : IEquatable<CreateNativeDetourRequest>
	{
		public CreateNativeDetourRequest(IntPtr Source, IntPtr Target)
		{
			this.Source = Source;
			this.Target = Target;
			this.ApplyByDefault = true;
		}

		public IntPtr Source { get; set; }

		public IntPtr Target { get; set; }

		public bool ApplyByDefault { get; set; }

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("CreateNativeDetourRequest");
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
			builder.Append("Source = ");
			builder.Append(this.Source.ToString());
			builder.Append(", Target = ");
			builder.Append(this.Target.ToString());
			builder.Append(", ApplyByDefault = ");
			builder.Append(this.ApplyByDefault.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(CreateNativeDetourRequest left, CreateNativeDetourRequest right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(CreateNativeDetourRequest left, CreateNativeDetourRequest right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (EqualityComparer<IntPtr>.Default.GetHashCode(this.<Source>k__BackingField) * -1521134295 + EqualityComparer<IntPtr>.Default.GetHashCode(this.<Target>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<ApplyByDefault>k__BackingField);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is CreateNativeDetourRequest && this.Equals((CreateNativeDetourRequest)obj);
		}

		[CompilerGenerated]
		public bool Equals(CreateNativeDetourRequest other)
		{
			return EqualityComparer<IntPtr>.Default.Equals(this.<Source>k__BackingField, other.<Source>k__BackingField) && EqualityComparer<IntPtr>.Default.Equals(this.<Target>k__BackingField, other.<Target>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<ApplyByDefault>k__BackingField, other.<ApplyByDefault>k__BackingField);
		}

		[CompilerGenerated]
		public void Deconstruct(out IntPtr Source, out IntPtr Target)
		{
			Source = this.Source;
			Target = this.Target;
		}
	}
}
