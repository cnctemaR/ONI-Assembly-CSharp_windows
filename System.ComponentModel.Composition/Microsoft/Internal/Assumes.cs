using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.Internal
{
	internal static class Assumes
	{
		[DebuggerStepThrough]
		internal static void NotNull<T>(T value) where T : class
		{
			Assumes.IsTrue(value != null);
		}

		[DebuggerStepThrough]
		internal static void NotNull<T1, T2>(T1 value1, T2 value2) where T1 : class where T2 : class
		{
			Assumes.NotNull<T1>(value1);
			Assumes.NotNull<T2>(value2);
		}

		[DebuggerStepThrough]
		internal static void NotNull<T1, T2, T3>(T1 value1, T2 value2, T3 value3) where T1 : class where T2 : class where T3 : class
		{
			Assumes.NotNull<T1>(value1);
			Assumes.NotNull<T2>(value2);
			Assumes.NotNull<T3>(value3);
		}

		[DebuggerStepThrough]
		internal static void NotNullOrEmpty(string value)
		{
			Assumes.NotNull<string>(value);
			Assumes.IsTrue(value.Length > 0);
		}

		[DebuggerStepThrough]
		internal static void IsTrue(bool condition)
		{
			if (!condition)
			{
				throw Assumes.UncatchableException(null);
			}
		}

		[DebuggerStepThrough]
		internal static void IsTrue(bool condition, string message)
		{
			if (!condition)
			{
				throw Assumes.UncatchableException(message);
			}
		}

		[DebuggerStepThrough]
		internal static T NotReachable<T>()
		{
			throw Assumes.UncatchableException("Code path should never be reached!");
		}

		[DebuggerStepThrough]
		private static Exception UncatchableException(string message)
		{
			return new Assumes.InternalErrorException(message);
		}

		[Serializable]
		private class InternalErrorException : Exception
		{
			public InternalErrorException(string message)
				: base(string.Format(CultureInfo.CurrentCulture, Strings.InternalExceptionMessage, message))
			{
			}

			[SecuritySafeCritical]
			protected InternalErrorException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}
	}
}
