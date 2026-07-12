using System;
using System.Data;
using System.Globalization;
using System.Security;
using System.Threading;

internal static class DataSetUtil
{
	internal static void CheckArgumentNull<T>(T argumentValue, string argumentName) where T : class
	{
		if (argumentValue == null)
		{
			throw DataSetUtil.ArgumentNull(argumentName);
		}
	}

	private static T TraceException<T>(string trace, T e)
	{
		return e;
	}

	private static T TraceExceptionAsReturnValue<T>(T e)
	{
		return DataSetUtil.TraceException<T>("<comm.ADP.TraceException|ERR|THROW> '%ls'\n", e);
	}

	internal static ArgumentException Argument(string message)
	{
		return DataSetUtil.TraceExceptionAsReturnValue<ArgumentException>(new ArgumentException(message));
	}

	internal static ArgumentNullException ArgumentNull(string message)
	{
		return DataSetUtil.TraceExceptionAsReturnValue<ArgumentNullException>(new ArgumentNullException(message));
	}

	internal static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string parameterName)
	{
		return DataSetUtil.TraceExceptionAsReturnValue<ArgumentOutOfRangeException>(new ArgumentOutOfRangeException(parameterName, message));
	}

	internal static InvalidCastException InvalidCast(string message)
	{
		return DataSetUtil.TraceExceptionAsReturnValue<InvalidCastException>(new InvalidCastException(message));
	}

	internal static InvalidOperationException InvalidOperation(string message)
	{
		return DataSetUtil.TraceExceptionAsReturnValue<InvalidOperationException>(new InvalidOperationException(message));
	}

	internal static NotSupportedException NotSupported(string message)
	{
		return DataSetUtil.TraceExceptionAsReturnValue<NotSupportedException>(new NotSupportedException(message));
	}

	internal static ArgumentOutOfRangeException InvalidEnumerationValue(Type type, int value)
	{
		return DataSetUtil.ArgumentOutOfRange(string.Format("The {0} enumeration value, {1}, is not valid.", type.Name, value.ToString(CultureInfo.InvariantCulture)), type.Name);
	}

	internal static ArgumentOutOfRangeException InvalidDataRowState(DataRowState value)
	{
		return DataSetUtil.InvalidEnumerationValue(typeof(DataRowState), (int)value);
	}

	internal static ArgumentOutOfRangeException InvalidLoadOption(LoadOption value)
	{
		return DataSetUtil.InvalidEnumerationValue(typeof(LoadOption), (int)value);
	}

	internal static bool IsCatchableExceptionType(Exception e)
	{
		Type type = e.GetType();
		return type != DataSetUtil.s_stackOverflowType && type != DataSetUtil.s_outOfMemoryType && type != DataSetUtil.s_threadAbortType && type != DataSetUtil.s_nullReferenceType && type != DataSetUtil.s_accessViolationType && !DataSetUtil.s_securityType.IsAssignableFrom(type);
	}

	private static readonly Type s_stackOverflowType = typeof(StackOverflowException);

	private static readonly Type s_outOfMemoryType = typeof(OutOfMemoryException);

	private static readonly Type s_threadAbortType = typeof(ThreadAbortException);

	private static readonly Type s_nullReferenceType = typeof(NullReferenceException);

	private static readonly Type s_accessViolationType = typeof(AccessViolationException);

	private static readonly Type s_securityType = typeof(SecurityException);
}
