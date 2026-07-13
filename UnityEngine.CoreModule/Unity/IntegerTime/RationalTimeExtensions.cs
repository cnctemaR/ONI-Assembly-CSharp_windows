using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace Unity.IntegerTime
{
	public static class RationalTimeExtensions
	{
		[FreeFunction("IntegerTime::RationalTime::ToDouble", IsFreeFunction = true, ThrowsException = true)]
		public static double ToDouble(this RationalTime value)
		{
			return RationalTimeExtensions.ToDouble_Injected(ref value);
		}

		[FreeFunction("IntegerTime::RationalTime::IsValid", IsFreeFunction = true, ThrowsException = false)]
		public static bool IsValid(this RationalTime value)
		{
			return RationalTimeExtensions.IsValid_Injected(ref value);
		}

		[FreeFunction("IntegerTime::RationalTime::ConvertRate", IsFreeFunction = true, ThrowsException = true)]
		public static RationalTime Convert(this RationalTime time, RationalTime.TicksPerSecond rate)
		{
			RationalTime rationalTime;
			RationalTimeExtensions.Convert_Injected(ref time, ref rate, out rationalTime);
			return rationalTime;
		}

		[FreeFunction("IntegerTime::RationalTime::Add", IsFreeFunction = true, ThrowsException = true)]
		public static RationalTime Add(this RationalTime lhs, RationalTime rhs)
		{
			RationalTime rationalTime;
			RationalTimeExtensions.Add_Injected(ref lhs, ref rhs, out rationalTime);
			return rationalTime;
		}

		[FreeFunction("IntegerTime::RationalTime::Subtract", IsFreeFunction = true, ThrowsException = true)]
		public static RationalTime Subtract(this RationalTime lhs, RationalTime rhs)
		{
			RationalTime rationalTime;
			RationalTimeExtensions.Subtract_Injected(ref lhs, ref rhs, out rationalTime);
			return rationalTime;
		}

		[FreeFunction("IntegerTime::RationalTime::Multiply", IsFreeFunction = true, ThrowsException = true)]
		public static RationalTime Multiply(this RationalTime lhs, RationalTime rhs)
		{
			RationalTime rationalTime;
			RationalTimeExtensions.Multiply_Injected(ref lhs, ref rhs, out rationalTime);
			return rationalTime;
		}

		[FreeFunction("IntegerTime::RationalTime::Divide", IsFreeFunction = true, ThrowsException = true)]
		public static RationalTime Divide(this RationalTime lhs, RationalTime rhs)
		{
			RationalTime rationalTime;
			RationalTimeExtensions.Divide_Injected(ref lhs, ref rhs, out rationalTime);
			return rationalTime;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double ToDouble_Injected([In] ref RationalTime value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValid_Injected([In] ref RationalTime value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Convert_Injected([In] ref RationalTime time, [In] ref RationalTime.TicksPerSecond rate, out RationalTime ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Add_Injected([In] ref RationalTime lhs, [In] ref RationalTime rhs, out RationalTime ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Subtract_Injected([In] ref RationalTime lhs, [In] ref RationalTime rhs, out RationalTime ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Multiply_Injected([In] ref RationalTime lhs, [In] ref RationalTime rhs, out RationalTime ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Divide_Injected([In] ref RationalTime lhs, [In] ref RationalTime rhs, out RationalTime ret);
	}
}
