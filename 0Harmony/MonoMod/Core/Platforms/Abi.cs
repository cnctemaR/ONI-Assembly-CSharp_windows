using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms
{
	internal readonly struct Abi : IEquatable<Abi>
	{
		public Abi(global::System.ReadOnlyMemory<SpecialArgumentKind> ArgumentOrder, [Nullable(1)] Classifier Classifier, bool ReturnsReturnBuffer)
		{
			this.ArgumentOrder = ArgumentOrder;
			this.Classifier = Classifier;
			this.ReturnsReturnBuffer = ReturnsReturnBuffer;
		}

		public global::System.ReadOnlyMemory<SpecialArgumentKind> ArgumentOrder { get; set; }

		[Nullable(1)]
		public Classifier Classifier
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public bool ReturnsReturnBuffer { get; set; }

		[NullableContext(1)]
		public TypeClassification Classify(Type type, bool isReturn)
		{
			Helpers.ThrowIfArgumentNull<Type>(type, "type");
			if (type == typeof(void))
			{
				return TypeClassification.InRegister;
			}
			if (!type.IsValueType)
			{
				return TypeClassification.InRegister;
			}
			if (type.IsPointer)
			{
				return TypeClassification.InRegister;
			}
			if (type.IsByRef)
			{
				return TypeClassification.InRegister;
			}
			return this.Classifier(type, isReturn);
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Abi");
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
			builder.Append("ArgumentOrder = ");
			builder.Append(this.ArgumentOrder.ToString());
			builder.Append(", Classifier = ");
			builder.Append(this.Classifier);
			builder.Append(", ReturnsReturnBuffer = ");
			builder.Append(this.ReturnsReturnBuffer.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(Abi left, Abi right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(Abi left, Abi right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (EqualityComparer<global::System.ReadOnlyMemory<SpecialArgumentKind>>.Default.GetHashCode(this.<ArgumentOrder>k__BackingField) * -1521134295 + EqualityComparer<Classifier>.Default.GetHashCode(this.<Classifier>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<ReturnsReturnBuffer>k__BackingField);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is Abi && this.Equals((Abi)obj);
		}

		[CompilerGenerated]
		public bool Equals(Abi other)
		{
			return EqualityComparer<global::System.ReadOnlyMemory<SpecialArgumentKind>>.Default.Equals(this.<ArgumentOrder>k__BackingField, other.<ArgumentOrder>k__BackingField) && EqualityComparer<Classifier>.Default.Equals(this.<Classifier>k__BackingField, other.<Classifier>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<ReturnsReturnBuffer>k__BackingField, other.<ReturnsReturnBuffer>k__BackingField);
		}

		[CompilerGenerated]
		public void Deconstruct(out global::System.ReadOnlyMemory<SpecialArgumentKind> ArgumentOrder, [Nullable(1)] out Classifier Classifier, out bool ReturnsReturnBuffer)
		{
			ArgumentOrder = this.ArgumentOrder;
			Classifier = this.Classifier;
			ReturnsReturnBuffer = this.ReturnsReturnBuffer;
		}
	}
}
