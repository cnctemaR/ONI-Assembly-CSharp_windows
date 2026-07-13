using System;
using System.Runtime.CompilerServices;

namespace Unity.IntegerTime
{
	public static class DiscreteTimeTimeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime Abs(this DiscreteTime lhs)
		{
			return DiscreteTime.FromTicks(Math.Abs(lhs.Value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime Min(this DiscreteTime lhs, DiscreteTime rhs)
		{
			return DiscreteTime.FromTicks(Math.Min(lhs.Value, rhs.Value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime Max(this DiscreteTime lhs, DiscreteTime rhs)
		{
			return DiscreteTime.FromTicks(Math.Max(lhs.Value, rhs.Value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime Clamp(this DiscreteTime x, DiscreteTime a, DiscreteTime b)
		{
			return a.Max(b.Min(x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime Floor(this DiscreteTime x)
		{
			return (DiscreteTime)Math.Floor((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DiscreteTime Select(this DiscreteTime a, DiscreteTime b, bool c)
		{
			return DiscreteTime.FromTicks(c ? b.Value : a.Value);
		}
	}
}
