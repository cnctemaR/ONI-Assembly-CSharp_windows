using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MonoMod.Logs
{
	internal readonly struct MessageHole : IEquatable<MessageHole>
	{
		public int Start { get; }

		public int End { get; }

		[Nullable(2)]
		public object Value
		{
			[NullableContext(2)]
			get;
		}

		public bool IsValueUnrepresentable { get; }

		public MessageHole(int start, int end)
		{
			this.Value = null;
			this.IsValueUnrepresentable = true;
			this.Start = start;
			this.End = end;
		}

		[NullableContext(2)]
		public MessageHole(int start, int end, object value)
		{
			this.Value = value;
			this.IsValueUnrepresentable = false;
			this.Start = start;
			this.End = end;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MessageHole");
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
			builder.Append("Start = ");
			builder.Append(this.Start.ToString());
			builder.Append(", End = ");
			builder.Append(this.End.ToString());
			builder.Append(", Value = ");
			builder.Append(this.Value);
			builder.Append(", IsValueUnrepresentable = ");
			builder.Append(this.IsValueUnrepresentable.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(MessageHole left, MessageHole right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(MessageHole left, MessageHole right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<int>.Default.GetHashCode(this.<Start>k__BackingField) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<End>k__BackingField)) * -1521134295 + EqualityComparer<object>.Default.GetHashCode(this.<Value>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<IsValueUnrepresentable>k__BackingField);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is MessageHole && this.Equals((MessageHole)obj);
		}

		[CompilerGenerated]
		public bool Equals(MessageHole other)
		{
			return EqualityComparer<int>.Default.Equals(this.<Start>k__BackingField, other.<Start>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<End>k__BackingField, other.<End>k__BackingField) && EqualityComparer<object>.Default.Equals(this.<Value>k__BackingField, other.<Value>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<IsValueUnrepresentable>k__BackingField, other.<IsValueUnrepresentable>k__BackingField);
		}
	}
}
