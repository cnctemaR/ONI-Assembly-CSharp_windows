using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace MonoMod.Core
{
	[NullableContext(1)]
	[Nullable(0)]
	[CLSCompliant(true)]
	internal readonly struct CreateDetourRequest : IEquatable<CreateDetourRequest>
	{
		public CreateDetourRequest(MethodBase Source, MethodBase Target)
		{
			this.CreateSourceCloneIfNotILClone = false;
			this.Source = Source;
			this.Target = Target;
			this.ApplyByDefault = true;
		}

		public MethodBase Source { get; set; }

		public MethodBase Target { get; set; }

		public bool ApplyByDefault { get; set; }

		public bool CreateSourceCloneIfNotILClone { get; set; }

		[NullableContext(0)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("CreateDetourRequest");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[NullableContext(0)]
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Source = ");
			builder.Append(this.Source);
			builder.Append(", Target = ");
			builder.Append(this.Target);
			builder.Append(", ApplyByDefault = ");
			builder.Append(this.ApplyByDefault.ToString());
			builder.Append(", CreateSourceCloneIfNotILClone = ");
			builder.Append(this.CreateSourceCloneIfNotILClone.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(CreateDetourRequest left, CreateDetourRequest right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(CreateDetourRequest left, CreateDetourRequest right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<MethodBase>.Default.GetHashCode(this.<Source>k__BackingField) * -1521134295 + EqualityComparer<MethodBase>.Default.GetHashCode(this.<Target>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<ApplyByDefault>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<CreateSourceCloneIfNotILClone>k__BackingField);
		}

		[NullableContext(0)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is CreateDetourRequest && this.Equals((CreateDetourRequest)obj);
		}

		[CompilerGenerated]
		public bool Equals(CreateDetourRequest other)
		{
			return EqualityComparer<MethodBase>.Default.Equals(this.<Source>k__BackingField, other.<Source>k__BackingField) && EqualityComparer<MethodBase>.Default.Equals(this.<Target>k__BackingField, other.<Target>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<ApplyByDefault>k__BackingField, other.<ApplyByDefault>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<CreateSourceCloneIfNotILClone>k__BackingField, other.<CreateSourceCloneIfNotILClone>k__BackingField);
		}

		[CompilerGenerated]
		public void Deconstruct(out MethodBase Source, out MethodBase Target)
		{
			Source = this.Source;
			Target = this.Target;
		}
	}
}
