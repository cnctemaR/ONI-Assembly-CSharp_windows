using System;
using System.Globalization;

namespace System.Data.Common
{
	internal sealed class ExceptionHelper
	{
		internal static ArgumentException InvalidSizeValue(int value)
		{
			string[] array = new string[] { value.ToString() };
			return new ArgumentException(ExceptionHelper.GetExceptionMessage("Invalid parameter Size value '{0}'. The value must be greater than or equal to 0.", array));
		}

		internal static void CheckEnumValue(Type enumType, object value)
		{
			if (!Enum.IsDefined(enumType, value))
			{
				throw ExceptionHelper.InvalidEnumValueException(enumType.Name, value);
			}
		}

		internal static ArgumentException InvalidEnumValueException(string enumeration, object value)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "The {0} enumeration value, {1}, is invalid.", new object[] { enumeration, value });
			return new ArgumentOutOfRangeException(enumeration, text);
		}

		internal static ArgumentOutOfRangeException InvalidDataRowVersion(DataRowVersion value)
		{
			object[] array = new object[]
			{
				"DataRowVersion",
				value.ToString()
			};
			return new ArgumentOutOfRangeException(ExceptionHelper.GetExceptionMessage("{0}: Invalid DataRow Version enumeration value: {1}", array));
		}

		internal static ArgumentOutOfRangeException InvalidParameterDirection(ParameterDirection value)
		{
			object[] array = new object[]
			{
				"ParameterDirection",
				value.ToString()
			};
			return new ArgumentOutOfRangeException(ExceptionHelper.GetExceptionMessage("Invalid direction '{0}' for '{1}' parameter.", array));
		}

		internal static InvalidOperationException NoStoredProcedureExists(string procedureName)
		{
			object[] array = new object[] { procedureName };
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("The stored procedure '{0}' doesn't exist.", array));
		}

		internal static ArgumentNullException ArgumentNull(string parameter)
		{
			return new ArgumentNullException(parameter);
		}

		internal static InvalidOperationException TransactionRequired()
		{
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("Execute requires the command to have a transaction object when the connection assigned to the command is in a pending local transaction.  The Transaction property of the command has not been initialized."));
		}

		internal static ArgumentOutOfRangeException InvalidOleDbType(int value)
		{
			string[] array = new string[] { value.ToString() };
			return new ArgumentOutOfRangeException(ExceptionHelper.GetExceptionMessage("Invalid OleDbType enumeration value: {0}", array));
		}

		internal static ArgumentException InvalidDbType(int value)
		{
			string[] array = new string[] { value.ToString() };
			return new ArgumentException(ExceptionHelper.GetExceptionMessage("No mapping exists from DbType {0} to a known {1}.", array));
		}

		internal static InvalidOperationException DeriveParametersNotSupported(Type type, CommandType commandType)
		{
			string[] array = new string[]
			{
				type.ToString(),
				commandType.ToString()
			};
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("{0} DeriveParameters only supports CommandType.StoredProcedure, not CommandType.{1}.", array));
		}

		internal static InvalidOperationException ReaderClosed(string mehodName)
		{
			string[] array = new string[] { mehodName };
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("Invalid attempt to {0} when reader is closed.", array));
		}

		internal static ArgumentOutOfRangeException InvalidSqlDbType(int value)
		{
			string[] array = new string[] { value.ToString() };
			return new ArgumentOutOfRangeException(ExceptionHelper.GetExceptionMessage("{0}: Invalid SqlDbType enumeration value: {1}.", array));
		}

		internal static ArgumentException UnknownDataType(string type1, string type2)
		{
			string[] array = new string[] { type1, type2 };
			return new ArgumentException(ExceptionHelper.GetExceptionMessage("No mapping exists from DbType {0} to a known {1}.", array));
		}

		internal static InvalidOperationException TransactionNotInitialized()
		{
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("Execute requires the command to have a transaction object when the connection assigned to the command is in a pending local transaction.  The Transaction property of the command has not been initialized."));
		}

		internal static InvalidOperationException TransactionNotUsable(Type type)
		{
			return new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "This {0} has completed; it is no longer usable.", new object[] { type.Name }));
		}

		internal static InvalidOperationException ParametersNotInitialized(int parameterPosition, string parameterName, string parameterType)
		{
			object[] array = new object[] { parameterPosition, parameterName, parameterType };
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("Parameter {0}: '{1}', the property DbType is uninitialized: OleDbType.{2}.", array));
		}

		internal static InvalidOperationException WrongParameterSize(string provider)
		{
			string[] array = new string[] { provider };
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("{0}.Prepare method requires all variable length parameters to have an explicitly set non-zero Size.", array));
		}

		internal static InvalidOperationException ConnectionNotOpened(string operationName, string connectionState)
		{
			object[] array = new object[] { operationName, connectionState };
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("{0} requires an open and available Connection. The connection's current state is {1}.", array));
		}

		internal static InvalidOperationException ConnectionNotInitialized(string methodName)
		{
			object[] array = new object[] { methodName };
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("{0}: Connection property has not been initialized.", array));
		}

		internal static InvalidOperationException OpenConnectionRequired(string methodName, object connectionState)
		{
			object[] array = new object[] { methodName, connectionState };
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("{0} requires an open and available Connection. The connection's current state is {1}.", array));
		}

		internal static InvalidOperationException OpenedReaderExists()
		{
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("There is already an open DataReader associated with this Connection which must be closed first."));
		}

		internal static InvalidOperationException ConnectionAlreadyOpen(object connectionState)
		{
			object[] array = new object[] { connectionState };
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("The connection is already Open (state={0}).", array));
		}

		internal static InvalidOperationException ConnectionClosed()
		{
			return new InvalidOperationException("Invalid operation. The Connection is closed.");
		}

		internal static InvalidOperationException ConnectionStringNotInitialized()
		{
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("The ConnectionString property has not been initialized."));
		}

		internal static InvalidOperationException ConnectionIsBusy(object commandType, object connectionState)
		{
			object[] array = new object[]
			{
				commandType.ToString(),
				connectionState.ToString()
			};
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("The {0} is currently busy {1}.", array));
		}

		internal static InvalidOperationException NotAllowedWhileConnectionOpen(string propertyName, object connectionState)
		{
			object[] array = new object[] { propertyName, connectionState };
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("Not allowed to change the '{0}' property while the connection (state={1}).", array));
		}

		internal static ArgumentException OleDbNoProviderSpecified()
		{
			return new ArgumentException(ExceptionHelper.GetExceptionMessage("An OLE DB Provider was not specified in the ConnectionString.  An example would be, 'Provider=SQLOLEDB;'."));
		}

		internal static ArgumentException InvalidValueForKey(string key)
		{
			string[] array = new string[] { key };
			return new ArgumentException(string.Format("Invalid value for key {0}", array));
		}

		internal static InvalidOperationException ParameterSizeNotInitialized(int parameterIndex, string parameterName, string parameterType, int parameterSize)
		{
			object[] array = new object[]
			{
				parameterIndex.ToString(),
				parameterName,
				parameterType,
				parameterSize.ToString()
			};
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("Parameter {0}: '{1}' of type: {2}, the property Size has an invalid size: {3}", array));
		}

		internal static ArgumentException InvalidUpdateStatus(UpdateStatus status)
		{
			object[] array = new object[] { status };
			return new ArgumentException(ExceptionHelper.GetExceptionMessage("Invalid UpdateStatus: {0}", array));
		}

		internal static InvalidOperationException UpdateRequiresCommand(string command)
		{
			object[] array = new object[] { command };
			return new InvalidOperationException(ExceptionHelper.GetExceptionMessage("Auto SQL generation during {0} requires a valid SelectCommand.", array));
		}

		internal static DataException RowUpdatedError()
		{
			return new DataException(ExceptionHelper.GetExceptionMessage("RowUpdatedEvent: Errors occurred; no additional is information available."));
		}

		internal static ArgumentNullException CollectionNoNullsAllowed(object collection, object objectsType)
		{
			object[] array = new object[]
			{
				collection.GetType().ToString(),
				objectsType.ToString()
			};
			return new ArgumentNullException(ExceptionHelper.GetExceptionMessage("The {0} only accepts non-null {1} type objects.", array));
		}

		internal static ArgumentException CollectionAlreadyContains(object objectType, string propertyName, object propertyValue, object collection)
		{
			object[] array = new object[]
			{
				objectType.ToString(),
				propertyName,
				propertyValue,
				collection.GetType().ToString()
			};
			return new ArgumentException(ExceptionHelper.GetExceptionMessage("The {0} with {1} '{2}' is already contained by this {3}.", array));
		}

		internal static string GetExceptionMessage(string exceptionMessage, object[] args)
		{
			if (args == null || args.Length == 0)
			{
				return exceptionMessage;
			}
			return string.Format(exceptionMessage, args);
		}

		internal static string GetExceptionMessage(string exceptionMessage)
		{
			return ExceptionHelper.GetExceptionMessage(exceptionMessage, null);
		}
	}
}
