using System;
using System.Runtime.CompilerServices;
using MonoMod.Logs;

namespace MonoMod.Core.Utils
{
	internal readonly struct AddressMeaning : IEquatable<AddressMeaning>
	{
		public AddressKind Kind { get; }

		public int RelativeToOffset { get; }

		public ulong ConstantValue { get; }

		public AddressMeaning(AddressKind kind)
		{
			this.ConstantValue = 0UL;
			kind.Validate("kind");
			if (!kind.IsAbsolute())
			{
				throw new ArgumentOutOfRangeException("kind");
			}
			this.Kind = kind;
			this.RelativeToOffset = 0;
		}

		public AddressMeaning(AddressKind kind, int relativeOffset)
		{
			this.ConstantValue = 0UL;
			kind.Validate("kind");
			if (!kind.IsRelative())
			{
				throw new ArgumentOutOfRangeException("kind");
			}
			if (relativeOffset < 0)
			{
				throw new ArgumentOutOfRangeException("relativeOffset");
			}
			this.Kind = kind;
			this.RelativeToOffset = relativeOffset;
		}

		public AddressMeaning(AddressKind kind, int relativeOffset, ulong constantValue)
		{
			kind.Validate("kind");
			if (!kind.IsRelative())
			{
				throw new ArgumentOutOfRangeException("kind");
			}
			if (relativeOffset < 0)
			{
				throw new ArgumentOutOfRangeException("relativeOffset");
			}
			this.Kind = kind;
			this.RelativeToOffset = relativeOffset;
			this.ConstantValue = constantValue;
		}

		[return: NativeInteger]
		private unsafe static IntPtr DoProcessAddress(AddressKind kind, [NativeInteger] IntPtr basePtr, int offset, ulong constantValue, ulong address)
		{
			if (kind.IsConstant())
			{
				address = constantValue;
			}
			IntPtr intPtr;
			if (kind.IsAbsolute())
			{
				intPtr = (IntPtr)address;
			}
			else
			{
				long num = (kind.Is32Bit() ? ((long)(*Unsafe.As<ulong, int>(ref address))) : (*Unsafe.As<ulong, long>(ref address)));
				intPtr = (IntPtr)((long)(basePtr + (IntPtr)offset) + num);
			}
			if (kind.IsIndirect())
			{
				intPtr = *intPtr;
			}
			return intPtr;
		}

		[return: NativeInteger]
		public IntPtr ProcessAddress([NativeInteger] IntPtr basePtr, int offset, ulong address)
		{
			return AddressMeaning.DoProcessAddress(this.Kind, basePtr, offset + this.RelativeToOffset, this.ConstantValue, address);
		}

		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			if (obj is AddressMeaning)
			{
				AddressMeaning addressMeaning = (AddressMeaning)obj;
				return this.Equals(addressMeaning);
			}
			return false;
		}

		public bool Equals(AddressMeaning other)
		{
			return this.Kind == other.Kind && this.RelativeToOffset == other.RelativeToOffset && this.ConstantValue == other.ConstantValue;
		}

		[NullableContext(1)]
		public override string ToString()
		{
			FormatInterpolatedStringHandler formatInterpolatedStringHandler = new FormatInterpolatedStringHandler(38, 3);
			formatInterpolatedStringHandler.AppendLiteral("AddressMeaning(");
			formatInterpolatedStringHandler.AppendFormatted(this.Kind.FastToString());
			formatInterpolatedStringHandler.AppendLiteral(", offset: ");
			formatInterpolatedStringHandler.AppendFormatted<int>(this.RelativeToOffset);
			formatInterpolatedStringHandler.AppendLiteral(", constant: ");
			formatInterpolatedStringHandler.AppendFormatted<ulong>(this.ConstantValue);
			formatInterpolatedStringHandler.AppendLiteral(")");
			return DebugFormatter.Format(ref formatInterpolatedStringHandler);
		}

		public override int GetHashCode()
		{
			return global::System.HashCode.Combine<AddressKind, int, ulong>(this.Kind, this.RelativeToOffset, this.ConstantValue);
		}

		public static bool operator ==(AddressMeaning left, AddressMeaning right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(AddressMeaning left, AddressMeaning right)
		{
			return !(left == right);
		}
	}
}
