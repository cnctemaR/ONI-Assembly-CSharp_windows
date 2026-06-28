using System;
using System.Diagnostics;

public class KDebug
{
	[Conditional("CHECK_ASSERTS")]
	public static void CheckValidFloat(float f)
	{
		if (float.IsNaN(f) || float.IsPositiveInfinity(f) || float.IsNegativeInfinity(f))
		{
			KDebug.DebugBreak();
		}
	}

	[Conditional("CHECK_ASSERTS")]
	public static void AssertLess(float f, float max)
	{
		if (f >= max)
		{
			KDebug.DebugBreak();
		}
	}

	[Conditional("CHECK_ASSERTS")]
	public static void AssertGreater(float f, float min)
	{
		if (f <= min)
		{
			KDebug.DebugBreak();
		}
	}

	[Conditional("CHECK_ASSERTS")]
	public static void AssertLessEqual(float f, float max)
	{
		if (f > max)
		{
			KDebug.DebugBreak();
		}
	}

	[Conditional("CHECK_ASSERTS")]
	public static void AssertEqual(float f, float expected)
	{
		if (f != expected)
		{
			KDebug.DebugBreak();
		}
	}

	[Conditional("CHECK_ASSERTS")]
	public static void Assert(bool condition)
	{
		if (!condition)
		{
			KDebug.DebugBreak();
		}
	}

	[Conditional("CHECK_ASSERTS")]
	public static void DebugBreak()
	{
	}
}
