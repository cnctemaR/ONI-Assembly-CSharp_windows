using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Internal
{
	internal static class Requires
	{
		[DebuggerStepThrough]
		public static void NotNull<T>(T value, string parameterName) where T : class
		{
			if (value == null)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		[DebuggerStepThrough]
		public static void NotNullOrEmpty(string value, string parameterName)
		{
			Requires.NotNull<string>(value, parameterName);
			if (value.Length == 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ArgumentException_EmptyString, parameterName), parameterName);
			}
		}

		[DebuggerStepThrough]
		public static void NotNullOrNullElements<T>(IEnumerable<T> values, string parameterName) where T : class
		{
			Requires.NotNull<IEnumerable<T>>(values, parameterName);
			Requires.NotNullElements<T>(values, parameterName);
		}

		[DebuggerStepThrough]
		public static void NullOrNotNullElements<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> values, string parameterName) where TKey : class where TValue : class
		{
			Requires.NotNullElements<TKey, TValue>(values, parameterName);
		}

		[DebuggerStepThrough]
		public static void NullOrNotNullElements<T>(IEnumerable<T> values, string parameterName) where T : class
		{
			Requires.NotNullElements<T>(values, parameterName);
		}

		private static void NotNullElements<T>(IEnumerable<T> values, string parameterName) where T : class
		{
			if (values != null)
			{
				if (!Contract.ForAll<T>(values, (T value) => value != null))
				{
					throw ExceptionBuilder.CreateContainsNullElement(parameterName);
				}
			}
		}

		private static void NotNullElements<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> values, string parameterName) where TKey : class where TValue : class
		{
			if (values != null)
			{
				if (!Contract.ForAll<KeyValuePair<TKey, TValue>>(values, (KeyValuePair<TKey, TValue> keyValue) => keyValue.Key != null && keyValue.Value != null))
				{
					throw ExceptionBuilder.CreateContainsNullElement(parameterName);
				}
			}
		}

		[DebuggerStepThrough]
		public static void IsInMembertypeSet(MemberTypes value, string parameterName, MemberTypes enumFlagSet)
		{
			if ((value & enumFlagSet) != value || (value & (value - 1)) != (MemberTypes)0)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ArgumentOutOfRange_InvalidEnumInSet, parameterName, value, enumFlagSet.ToString()), parameterName);
			}
		}
	}
}
