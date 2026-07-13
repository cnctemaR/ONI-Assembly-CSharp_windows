using System;
using System.Runtime.CompilerServices;
using MonoMod.Logs;

namespace MonoMod.Core.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class AddressKindExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRelative(this AddressKind value)
		{
			return (value & AddressKind.Abs32) == AddressKind.Rel32;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAbsolute(this AddressKind value)
		{
			return !value.IsRelative();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is32Bit(this AddressKind value)
		{
			return (value & AddressKind.Rel64) == AddressKind.Rel32;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is64Bit(this AddressKind value)
		{
			return !value.Is32Bit();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPrecodeFixup(this AddressKind value)
		{
			return (value & AddressKind.PrecodeFixupThunkRel32) > AddressKind.Rel32;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsIndirect(this AddressKind value)
		{
			return (value & AddressKind.Indirect) > AddressKind.Rel32;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsConstant(this AddressKind value)
		{
			return (value & AddressKind.ConstantAddr) > AddressKind.Rel32;
		}

		public static void Validate(this AddressKind value, [CallerArgumentExpression("value")] string argName = "")
		{
			if ((value & ~(AddressKind.Rel64 | AddressKind.Abs32 | AddressKind.PrecodeFixupThunkRel32 | AddressKind.Indirect | AddressKind.ConstantAddr)) != AddressKind.Rel32)
			{
				throw new ArgumentOutOfRangeException(argName);
			}
		}

		public static string FastToString(this AddressKind value)
		{
			FormatInterpolatedStringHandler formatInterpolatedStringHandler = new FormatInterpolatedStringHandler(0, 4);
			formatInterpolatedStringHandler.AppendFormatted(value.IsPrecodeFixup() ? "PrecodeFixupThunk" : "");
			formatInterpolatedStringHandler.AppendFormatted(value.IsRelative() ? "Rel" : "Abs");
			formatInterpolatedStringHandler.AppendFormatted(value.Is32Bit() ? "32" : "64");
			formatInterpolatedStringHandler.AppendFormatted(value.IsIndirect() ? "Indirect" : "");
			return DebugFormatter.Format(ref formatInterpolatedStringHandler);
		}

		public const AddressKind IsAbsoluteField = AddressKind.Abs32;

		public const AddressKind Is64BitField = AddressKind.Rel64;

		public const AddressKind IsPrecodeFixupField = AddressKind.PrecodeFixupThunkRel32;

		public const AddressKind IsIndirectField = AddressKind.Indirect;

		public const AddressKind IsConstantField = AddressKind.ConstantAddr;
	}
}
