using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MonoMod.Core.Platforms
{
	[NullableContext(1)]
	[Nullable(0)]
	internal readonly struct NativeDetourInfo : IEquatable<NativeDetourInfo>
	{
		public NativeDetourInfo(IntPtr From, IntPtr To, INativeDetourKind InternalKind, [Nullable(2)] IDisposable InternalData)
		{
			this.From = From;
			this.To = To;
			this.InternalKind = InternalKind;
			this.InternalData = InternalData;
		}

		public IntPtr From { get; set; }

		public IntPtr To { get; set; }

		public INativeDetourKind InternalKind { get; set; }

		[Nullable(2)]
		public IDisposable InternalData
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public int Size
		{
			get
			{
				return this.InternalKind.Size;
			}
		}

		[NullableContext(0)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("NativeDetourInfo");
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
			builder.Append("From = ");
			builder.Append(this.From.ToString());
			builder.Append(", To = ");
			builder.Append(this.To.ToString());
			builder.Append(", InternalKind = ");
			builder.Append(this.InternalKind);
			builder.Append(", InternalData = ");
			builder.Append(this.InternalData);
			builder.Append(", Size = ");
			builder.Append(this.Size.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(NativeDetourInfo left, NativeDetourInfo right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(NativeDetourInfo left, NativeDetourInfo right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<IntPtr>.Default.GetHashCode(this.<From>k__BackingField) * -1521134295 + EqualityComparer<IntPtr>.Default.GetHashCode(this.<To>k__BackingField)) * -1521134295 + EqualityComparer<INativeDetourKind>.Default.GetHashCode(this.<InternalKind>k__BackingField)) * -1521134295 + EqualityComparer<IDisposable>.Default.GetHashCode(this.<InternalData>k__BackingField);
		}

		[NullableContext(0)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is NativeDetourInfo && this.Equals((NativeDetourInfo)obj);
		}

		[CompilerGenerated]
		public bool Equals(NativeDetourInfo other)
		{
			return EqualityComparer<IntPtr>.Default.Equals(this.<From>k__BackingField, other.<From>k__BackingField) && EqualityComparer<IntPtr>.Default.Equals(this.<To>k__BackingField, other.<To>k__BackingField) && EqualityComparer<INativeDetourKind>.Default.Equals(this.<InternalKind>k__BackingField, other.<InternalKind>k__BackingField) && EqualityComparer<IDisposable>.Default.Equals(this.<InternalData>k__BackingField, other.<InternalData>k__BackingField);
		}

		[CompilerGenerated]
		public void Deconstruct(out IntPtr From, out IntPtr To, out INativeDetourKind InternalKind, [Nullable(2)] out IDisposable InternalData)
		{
			From = this.From;
			To = this.To;
			InternalKind = this.InternalKind;
			InternalData = this.InternalData;
		}
	}
}
