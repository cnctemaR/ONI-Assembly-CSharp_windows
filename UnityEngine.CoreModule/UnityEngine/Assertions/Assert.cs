using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine.Assertions.Comparers;

namespace UnityEngine.Assertions
{
	/// <summary>
	///   <para>The Assert class contains assertion methods for setting invariants in the code.</para>
	/// </summary>
	[DebuggerStepThrough]
	public static class Assert
	{
		private static void Fail(string message, string userMessage)
		{
			if (Debugger.IsAttached)
			{
				throw new AssertionException(message, userMessage);
			}
			if (Assert.raiseExceptions)
			{
				throw new AssertionException(message, userMessage);
			}
			if (message == null)
			{
				message = "Assertion has failed\n";
			}
			if (userMessage != null)
			{
				message = userMessage + '\n' + message;
			}
			Debug.LogAssertion(message);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Assert.Equals should not be used for Assertions", true)]
		public new static bool Equals(object obj1, object obj2)
		{
			throw new InvalidOperationException("Assert.Equals should not be used for Assertions");
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Assert.ReferenceEquals should not be used for Assertions", true)]
		public new static bool ReferenceEquals(object obj1, object obj2)
		{
			throw new InvalidOperationException("Assert.ReferenceEquals should not be used for Assertions");
		}

		/// <summary>
		///   <para>Asserts that the condition is true.</para>
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void IsTrue(bool condition)
		{
			if (!condition)
			{
				Assert.IsTrue(condition, null);
			}
		}

		/// <summary>
		///   <para>Asserts that the condition is true.</para>
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void IsTrue(bool condition, string message)
		{
			if (!condition)
			{
				Assert.Fail(AssertionMessageUtil.BooleanFailureMessage(true), message);
			}
		}

		/// <summary>
		///   <para>Asserts that the condition is false.</para>
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void IsFalse(bool condition)
		{
			if (condition)
			{
				Assert.IsFalse(condition, null);
			}
		}

		/// <summary>
		///   <para>Asserts that the condition is false.</para>
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void IsFalse(bool condition, string message)
		{
			if (condition)
			{
				Assert.Fail(AssertionMessageUtil.BooleanFailureMessage(false), message);
			}
		}

		/// <summary>
		///   <para>Asserts that the values are approximately equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.
		///
		/// Note: Every time you call the method with tolerance specified, a new instance of Assertions.Comparers.FloatComparer is created. For performance reasons you might want to instance your own comparer and pass it to the AreEqual method. If the tolerance is not specifies, a default comparer is used and the issue does not occur.</para>
		/// </summary>
		/// <param name="tolerance">Tolerance of approximation.</param>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="message"></param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void AreApproximatelyEqual(float expected, float actual)
		{
			Assert.AreEqual<float>(expected, actual, null, FloatComparer.s_ComparerWithDefaultTolerance);
		}

		/// <summary>
		///   <para>Asserts that the values are approximately equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.
		///
		/// Note: Every time you call the method with tolerance specified, a new instance of Assertions.Comparers.FloatComparer is created. For performance reasons you might want to instance your own comparer and pass it to the AreEqual method. If the tolerance is not specifies, a default comparer is used and the issue does not occur.</para>
		/// </summary>
		/// <param name="tolerance">Tolerance of approximation.</param>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="message"></param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void AreApproximatelyEqual(float expected, float actual, string message)
		{
			Assert.AreEqual<float>(expected, actual, message, FloatComparer.s_ComparerWithDefaultTolerance);
		}

		/// <summary>
		///   <para>Asserts that the values are approximately equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.
		///
		/// Note: Every time you call the method with tolerance specified, a new instance of Assertions.Comparers.FloatComparer is created. For performance reasons you might want to instance your own comparer and pass it to the AreEqual method. If the tolerance is not specifies, a default comparer is used and the issue does not occur.</para>
		/// </summary>
		/// <param name="tolerance">Tolerance of approximation.</param>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="message"></param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void AreApproximatelyEqual(float expected, float actual, float tolerance)
		{
			Assert.AreApproximatelyEqual(expected, actual, tolerance, null);
		}

		/// <summary>
		///   <para>Asserts that the values are approximately equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.
		///
		/// Note: Every time you call the method with tolerance specified, a new instance of Assertions.Comparers.FloatComparer is created. For performance reasons you might want to instance your own comparer and pass it to the AreEqual method. If the tolerance is not specifies, a default comparer is used and the issue does not occur.</para>
		/// </summary>
		/// <param name="tolerance">Tolerance of approximation.</param>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="message"></param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void AreApproximatelyEqual(float expected, float actual, float tolerance, string message)
		{
			Assert.AreEqual<float>(expected, actual, message, new FloatComparer(tolerance));
		}

		/// <summary>
		///   <para>Asserts that the values are approximately not equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.</para>
		/// </summary>
		/// <param name="tolerance">Tolerance of approximation.</param>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="message"></param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotApproximatelyEqual(float expected, float actual)
		{
			Assert.AreNotEqual<float>(expected, actual, null, FloatComparer.s_ComparerWithDefaultTolerance);
		}

		/// <summary>
		///   <para>Asserts that the values are approximately not equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.</para>
		/// </summary>
		/// <param name="tolerance">Tolerance of approximation.</param>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="message"></param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotApproximatelyEqual(float expected, float actual, string message)
		{
			Assert.AreNotEqual<float>(expected, actual, message, FloatComparer.s_ComparerWithDefaultTolerance);
		}

		/// <summary>
		///   <para>Asserts that the values are approximately not equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.</para>
		/// </summary>
		/// <param name="tolerance">Tolerance of approximation.</param>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="message"></param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotApproximatelyEqual(float expected, float actual, float tolerance)
		{
			Assert.AreNotApproximatelyEqual(expected, actual, tolerance, null);
		}

		/// <summary>
		///   <para>Asserts that the values are approximately not equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.</para>
		/// </summary>
		/// <param name="tolerance">Tolerance of approximation.</param>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="message"></param>
		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotApproximatelyEqual(float expected, float actual, float tolerance, string message)
		{
			Assert.AreNotEqual<float>(expected, actual, message, new FloatComparer(tolerance));
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual<T>(T expected, T actual)
		{
			Assert.AreEqual<T>(expected, actual, null);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual<T>(T expected, T actual, string message)
		{
			Assert.AreEqual<T>(expected, actual, message, EqualityComparer<T>.Default);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual<T>(T expected, T actual, string message, IEqualityComparer<T> comparer)
		{
			if (typeof(Object).IsAssignableFrom(typeof(T)))
			{
				Assert.AreEqual(expected as Object, actual as Object, message);
			}
			else if (!comparer.Equals(actual, expected))
			{
				Assert.Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, true), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(Object expected, Object actual, string message)
		{
			if (actual != expected)
			{
				Assert.Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, true), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual<T>(T expected, T actual)
		{
			Assert.AreNotEqual<T>(expected, actual, null);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual<T>(T expected, T actual, string message)
		{
			Assert.AreNotEqual<T>(expected, actual, message, EqualityComparer<T>.Default);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual<T>(T expected, T actual, string message, IEqualityComparer<T> comparer)
		{
			if (typeof(Object).IsAssignableFrom(typeof(T)))
			{
				Assert.AreNotEqual(expected as Object, actual as Object, message);
			}
			else if (comparer.Equals(actual, expected))
			{
				Assert.Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, false), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(Object expected, Object actual, string message)
		{
			if (actual == expected)
			{
				Assert.Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, false), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsNull<T>(T value) where T : class
		{
			Assert.IsNull<T>(value, null);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsNull<T>(T value, string message) where T : class
		{
			if (typeof(Object).IsAssignableFrom(typeof(T)))
			{
				Assert.IsNull(value as Object, message);
			}
			else if (value != null)
			{
				Assert.Fail(AssertionMessageUtil.NullFailureMessage(value, true), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsNull(Object value, string message)
		{
			if (value != null)
			{
				Assert.Fail(AssertionMessageUtil.NullFailureMessage(value, true), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsNotNull<T>(T value) where T : class
		{
			Assert.IsNotNull<T>(value, null);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsNotNull<T>(T value, string message) where T : class
		{
			if (typeof(Object).IsAssignableFrom(typeof(T)))
			{
				Assert.IsNotNull(value as Object, message);
			}
			else if (value == null)
			{
				Assert.Fail(AssertionMessageUtil.NullFailureMessage(value, false), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void IsNotNull(Object value, string message)
		{
			if (value == null)
			{
				Assert.Fail(AssertionMessageUtil.NullFailureMessage(value, false), message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(sbyte expected, sbyte actual)
		{
			if ((int)expected != (int)actual)
			{
				Assert.AreEqual<sbyte>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(sbyte expected, sbyte actual, string message)
		{
			if ((int)expected != (int)actual)
			{
				Assert.AreEqual<sbyte>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(sbyte expected, sbyte actual)
		{
			if ((int)expected == (int)actual)
			{
				Assert.AreNotEqual<sbyte>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(sbyte expected, sbyte actual, string message)
		{
			if ((int)expected == (int)actual)
			{
				Assert.AreNotEqual<sbyte>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(byte expected, byte actual)
		{
			if (expected != actual)
			{
				Assert.AreEqual<byte>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(byte expected, byte actual, string message)
		{
			if (expected != actual)
			{
				Assert.AreEqual<byte>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(byte expected, byte actual)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<byte>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(byte expected, byte actual, string message)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<byte>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(char expected, char actual)
		{
			if (expected != actual)
			{
				Assert.AreEqual<char>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(char expected, char actual, string message)
		{
			if (expected != actual)
			{
				Assert.AreEqual<char>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(char expected, char actual)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<char>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(char expected, char actual, string message)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<char>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(short expected, short actual)
		{
			if (expected != actual)
			{
				Assert.AreEqual<short>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(short expected, short actual, string message)
		{
			if (expected != actual)
			{
				Assert.AreEqual<short>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(short expected, short actual)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<short>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(short expected, short actual, string message)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<short>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(ushort expected, ushort actual)
		{
			if (expected != actual)
			{
				Assert.AreEqual<ushort>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(ushort expected, ushort actual, string message)
		{
			if (expected != actual)
			{
				Assert.AreEqual<ushort>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(ushort expected, ushort actual)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<ushort>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(ushort expected, ushort actual, string message)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<ushort>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(int expected, int actual)
		{
			if (expected != actual)
			{
				Assert.AreEqual<int>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(int expected, int actual, string message)
		{
			if (expected != actual)
			{
				Assert.AreEqual<int>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(int expected, int actual)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<int>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(int expected, int actual, string message)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<int>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(uint expected, uint actual)
		{
			if (expected != actual)
			{
				Assert.AreEqual<uint>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(uint expected, uint actual, string message)
		{
			if (expected != actual)
			{
				Assert.AreEqual<uint>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(uint expected, uint actual)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<uint>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(uint expected, uint actual, string message)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<uint>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(long expected, long actual)
		{
			if (expected != actual)
			{
				Assert.AreEqual<long>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(long expected, long actual, string message)
		{
			if (expected != actual)
			{
				Assert.AreEqual<long>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(long expected, long actual)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<long>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(long expected, long actual, string message)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<long>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(ulong expected, ulong actual)
		{
			if (expected != actual)
			{
				Assert.AreEqual<ulong>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreEqual(ulong expected, ulong actual, string message)
		{
			if (expected != actual)
			{
				Assert.AreEqual<ulong>(expected, actual, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(ulong expected, ulong actual)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<ulong>(expected, actual, null);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AreNotEqual(ulong expected, ulong actual, string message)
		{
			if (expected == actual)
			{
				Assert.AreNotEqual<ulong>(expected, actual, message);
			}
		}

		internal const string UNITY_ASSERTIONS = "UNITY_ASSERTIONS";

		/// <summary>
		///   <para>Whether Unity should throw an exception on a failure.</para>
		/// </summary>
		public static bool raiseExceptions = false;
	}
}
