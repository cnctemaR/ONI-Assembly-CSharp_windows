using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using Microsoft.SqlServer.Server;

namespace System.Data.Common
{
	internal static class ADP
	{
		internal static Timer UnsafeCreateTimer(TimerCallback callback, object state, int dueTime, int period)
		{
			bool flag = false;
			Timer timer;
			try
			{
				if (!ExecutionContext.IsFlowSuppressed())
				{
					ExecutionContext.SuppressFlow();
					flag = true;
				}
				timer = new Timer(callback, state, dueTime, period);
			}
			finally
			{
				if (flag)
				{
					ExecutionContext.RestoreFlow();
				}
			}
			return timer;
		}

		internal static Task<bool> TrueTask
		{
			get
			{
				Task<bool> task;
				if ((task = ADP._trueTask) == null)
				{
					task = (ADP._trueTask = Task.FromResult<bool>(true));
				}
				return task;
			}
		}

		internal static Task<bool> FalseTask
		{
			get
			{
				Task<bool> task;
				if ((task = ADP._falseTask) == null)
				{
					task = (ADP._falseTask = Task.FromResult<bool>(false));
				}
				return task;
			}
		}

		private static void TraceException(string trace, Exception e)
		{
			if (e != null)
			{
				DataCommonEventSource.Log.Trace<Exception>(trace, e);
			}
		}

		internal static void TraceExceptionAsReturnValue(Exception e)
		{
			ADP.TraceException("<comm.ADP.TraceException|ERR|THROW> '{0}'", e);
		}

		internal static void TraceExceptionWithoutRethrow(Exception e)
		{
			ADP.TraceException("<comm.ADP.TraceException|ERR|CATCH> '%ls'\n", e);
		}

		internal static ArgumentException Argument(string error)
		{
			ArgumentException ex = new ArgumentException(error);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentException Argument(string error, Exception inner)
		{
			ArgumentException ex = new ArgumentException(error, inner);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentException Argument(string error, string parameter)
		{
			ArgumentException ex = new ArgumentException(error, parameter);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentNullException ArgumentNull(string parameter)
		{
			ArgumentNullException ex = new ArgumentNullException(parameter);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentNullException ArgumentNull(string parameter, string error)
		{
			ArgumentNullException ex = new ArgumentNullException(parameter, error);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentOutOfRangeException ArgumentOutOfRange(string parameterName)
		{
			ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException(parameterName);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string parameterName)
		{
			ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException(parameterName, message);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static IndexOutOfRangeException IndexOutOfRange(string error)
		{
			IndexOutOfRangeException ex = new IndexOutOfRangeException(error);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static InvalidCastException InvalidCast(string error)
		{
			return ADP.InvalidCast(error, null);
		}

		internal static InvalidCastException InvalidCast(string error, Exception inner)
		{
			InvalidCastException ex = new InvalidCastException(error, inner);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static InvalidOperationException InvalidOperation(string error)
		{
			InvalidOperationException ex = new InvalidOperationException(error);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static NotSupportedException NotSupported()
		{
			NotSupportedException ex = new NotSupportedException();
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static NotSupportedException NotSupported(string error)
		{
			NotSupportedException ex = new NotSupportedException(error);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static bool RemoveStringQuotes(string quotePrefix, string quoteSuffix, string quotedString, out string unquotedString)
		{
			int num = ((quotePrefix != null) ? quotePrefix.Length : 0);
			int num2 = ((quoteSuffix != null) ? quoteSuffix.Length : 0);
			if (num2 + num == 0)
			{
				unquotedString = quotedString;
				return true;
			}
			if (quotedString == null)
			{
				unquotedString = quotedString;
				return false;
			}
			int length = quotedString.Length;
			if (length < num + num2)
			{
				unquotedString = quotedString;
				return false;
			}
			if (num > 0 && !quotedString.StartsWith(quotePrefix, StringComparison.Ordinal))
			{
				unquotedString = quotedString;
				return false;
			}
			if (num2 > 0)
			{
				if (!quotedString.EndsWith(quoteSuffix, StringComparison.Ordinal))
				{
					unquotedString = quotedString;
					return false;
				}
				unquotedString = quotedString.Substring(num, length - (num + num2)).Replace(quoteSuffix + quoteSuffix, quoteSuffix);
			}
			else
			{
				unquotedString = quotedString.Substring(num, length - num);
			}
			return true;
		}

		internal static ArgumentOutOfRangeException NotSupportedEnumerationValue(Type type, string value, string method)
		{
			return ADP.ArgumentOutOfRange(SR.Format("The {0} enumeration value, {1}, is not supported by the {2} method.", type.Name, value, method), type.Name);
		}

		internal static InvalidOperationException DataAdapter(string error)
		{
			return ADP.InvalidOperation(error);
		}

		private static InvalidOperationException Provider(string error)
		{
			return ADP.InvalidOperation(error);
		}

		internal static ArgumentException InvalidMultipartName(string property, string value)
		{
			ArgumentException ex = new ArgumentException(SR.Format("{0} \"{1}\".", property, value));
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentException InvalidMultipartNameIncorrectUsageOfQuotes(string property, string value)
		{
			ArgumentException ex = new ArgumentException(SR.Format("{0} \"{1}\", incorrect usage of quotes.", property, value));
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentException InvalidMultipartNameToManyParts(string property, string value, int limit)
		{
			ArgumentException ex = new ArgumentException(SR.Format("{0} \"{1}\", the current limit of \"{2}\" is insufficient.", property, value, limit));
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static void CheckArgumentNull(object value, string parameterName)
		{
			if (value == null)
			{
				throw ADP.ArgumentNull(parameterName);
			}
		}

		internal static bool IsCatchableExceptionType(Exception e)
		{
			Type type = e.GetType();
			return type != ADP.s_stackOverflowType && type != ADP.s_outOfMemoryType && type != ADP.s_threadAbortType && type != ADP.s_nullReferenceType && type != ADP.s_accessViolationType && !ADP.s_securityType.IsAssignableFrom(type);
		}

		internal static bool IsCatchableOrSecurityExceptionType(Exception e)
		{
			Type type = e.GetType();
			return type != ADP.s_stackOverflowType && type != ADP.s_outOfMemoryType && type != ADP.s_threadAbortType && type != ADP.s_nullReferenceType && type != ADP.s_accessViolationType;
		}

		internal static ArgumentOutOfRangeException InvalidEnumerationValue(Type type, int value)
		{
			return ADP.ArgumentOutOfRange(SR.Format("The {0} enumeration value, {1}, is invalid.", type.Name, value.ToString(CultureInfo.InvariantCulture)), type.Name);
		}

		internal static ArgumentException ConnectionStringSyntax(int index)
		{
			return ADP.Argument(SR.Format("Format of the initialization string does not conform to specification starting at index {0}.", index));
		}

		internal static ArgumentException KeywordNotSupported(string keyword)
		{
			return ADP.Argument(SR.Format("Keyword not supported: '{0}'.", keyword));
		}

		internal static ArgumentException ConvertFailed(Type fromType, Type toType, Exception innerException)
		{
			return ADP.Argument(SR.Format(" Cannot convert object of type '{0}' to object of type '{1}'.", fromType.FullName, toType.FullName), innerException);
		}

		internal static Exception InvalidConnectionOptionValue(string key)
		{
			return ADP.InvalidConnectionOptionValue(key, null);
		}

		internal static Exception InvalidConnectionOptionValue(string key, Exception inner)
		{
			return ADP.Argument(SR.Format("Invalid value for key '{0}'.", key), inner);
		}

		internal static ArgumentException CollectionRemoveInvalidObject(Type itemType, ICollection collection)
		{
			return ADP.Argument(SR.Format("Attempted to remove an {0} that is not contained by this {1}.", itemType.Name, collection.GetType().Name));
		}

		internal static ArgumentNullException CollectionNullValue(string parameter, Type collection, Type itemType)
		{
			return ADP.ArgumentNull(parameter, SR.Format("The {0} only accepts non-null {1} type objects.", collection.Name, itemType.Name));
		}

		internal static IndexOutOfRangeException CollectionIndexInt32(int index, Type collection, int count)
		{
			return ADP.IndexOutOfRange(SR.Format("Invalid index {0} for this {1} with Count={2}.", index.ToString(CultureInfo.InvariantCulture), collection.Name, count.ToString(CultureInfo.InvariantCulture)));
		}

		internal static IndexOutOfRangeException CollectionIndexString(Type itemType, string propertyName, string propertyValue, Type collection)
		{
			return ADP.IndexOutOfRange(SR.Format("An {0} with {1} '{2}' is not contained by this {3}.", new object[] { itemType.Name, propertyName, propertyValue, collection.Name }));
		}

		internal static InvalidCastException CollectionInvalidType(Type collection, Type itemType, object invalidValue)
		{
			return ADP.InvalidCast(SR.Format("The {0} only accepts non-null {1} type objects, not {2} objects.", collection.Name, itemType.Name, invalidValue.GetType().Name));
		}

		private static string ConnectionStateMsg(ConnectionState state)
		{
			switch (state)
			{
			case ConnectionState.Closed:
				break;
			case ConnectionState.Open:
				return "The connection's current state is open.";
			case ConnectionState.Connecting:
				return "The connection's current state is connecting.";
			case ConnectionState.Open | ConnectionState.Connecting:
			case ConnectionState.Executing:
				goto IL_0046;
			case ConnectionState.Open | ConnectionState.Executing:
				return "The connection's current state is executing.";
			default:
				if (state == (ConnectionState.Open | ConnectionState.Fetching))
				{
					return "The connection's current state is fetching.";
				}
				if (state != (ConnectionState.Connecting | ConnectionState.Broken))
				{
					goto IL_0046;
				}
				break;
			}
			return "The connection's current state is closed.";
			IL_0046:
			return SR.Format("The connection's current state: {0}.", state.ToString());
		}

		internal static Exception StreamClosed([CallerMemberName] string method = "")
		{
			return ADP.InvalidOperation(SR.Format("Invalid attempt to {0} when stream is closed.", method));
		}

		internal static string BuildQuotedString(string quotePrefix, string quoteSuffix, string unQuotedString)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(quotePrefix))
			{
				stringBuilder.Append(quotePrefix);
			}
			if (!string.IsNullOrEmpty(quoteSuffix))
			{
				stringBuilder.Append(unQuotedString.Replace(quoteSuffix, quoteSuffix + quoteSuffix));
				stringBuilder.Append(quoteSuffix);
			}
			else
			{
				stringBuilder.Append(unQuotedString);
			}
			return stringBuilder.ToString();
		}

		internal static ArgumentException ParametersIsNotParent(Type parameterType, ICollection collection)
		{
			return ADP.Argument(SR.Format("The {0} is already contained by another {1}.", parameterType.Name, collection.GetType().Name));
		}

		internal static ArgumentException ParametersIsParent(Type parameterType, ICollection collection)
		{
			return ADP.Argument(SR.Format("The {0} is already contained by another {1}.", parameterType.Name, collection.GetType().Name));
		}

		internal static Exception InternalError(ADP.InternalErrorCode internalError)
		{
			return ADP.InvalidOperation(SR.Format("Internal .Net Framework Data Provider error {0}.", (int)internalError));
		}

		internal static Exception DataReaderClosed([CallerMemberName] string method = "")
		{
			return ADP.InvalidOperation(SR.Format("Invalid attempt to call {0} when reader is closed.", method));
		}

		internal static ArgumentOutOfRangeException InvalidSourceBufferIndex(int maxLen, long srcOffset, string parameterName)
		{
			return ADP.ArgumentOutOfRange(SR.Format("Invalid source buffer (size of {0}) offset: {1}", maxLen.ToString(CultureInfo.InvariantCulture), srcOffset.ToString(CultureInfo.InvariantCulture)), parameterName);
		}

		internal static ArgumentOutOfRangeException InvalidDestinationBufferIndex(int maxLen, int dstOffset, string parameterName)
		{
			return ADP.ArgumentOutOfRange(SR.Format("Invalid destination buffer (size of {0}) offset: {1}", maxLen.ToString(CultureInfo.InvariantCulture), dstOffset.ToString(CultureInfo.InvariantCulture)), parameterName);
		}

		internal static IndexOutOfRangeException InvalidBufferSizeOrIndex(int numBytes, int bufferIndex)
		{
			return ADP.IndexOutOfRange(SR.Format("Buffer offset '{1}' plus the bytes available '{0}' is greater than the length of the passed in buffer.", numBytes.ToString(CultureInfo.InvariantCulture), bufferIndex.ToString(CultureInfo.InvariantCulture)));
		}

		internal static Exception InvalidDataLength(long length)
		{
			return ADP.IndexOutOfRange(SR.Format("Data length '{0}' is less than 0.", length.ToString(CultureInfo.InvariantCulture)));
		}

		internal static bool CompareInsensitiveInvariant(string strvalue, string strconst)
		{
			return CultureInfo.InvariantCulture.CompareInfo.Compare(strvalue, strconst, CompareOptions.IgnoreCase) == 0;
		}

		internal static int DstCompare(string strA, string strB)
		{
			return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
		}

		internal static bool IsEmptyArray(string[] array)
		{
			return array == null || array.Length == 0;
		}

		internal static bool IsNull(object value)
		{
			if (value == null || DBNull.Value == value)
			{
				return true;
			}
			INullable nullable = value as INullable;
			return nullable != null && nullable.IsNull;
		}

		internal static Exception InvalidSeekOrigin(string parameterName)
		{
			return ADP.ArgumentOutOfRange("Specified SeekOrigin value is invalid.", parameterName);
		}

		internal static void SetCurrentTransaction(Transaction transaction)
		{
			Transaction.Current = transaction;
		}

		internal static Task<T> CreatedTaskWithCancellation<T>()
		{
			return Task.FromCanceled<T>(new CancellationToken(true));
		}

		internal static void TraceExceptionForCapture(Exception e)
		{
			ADP.TraceException("<comm.ADP.TraceException|ERR|CATCH> '{0}'", e);
		}

		internal static DataException Data(string message)
		{
			DataException ex = new DataException(message);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static void CheckArgumentLength(string value, string parameterName)
		{
			ADP.CheckArgumentNull(value, parameterName);
			if (value.Length == 0)
			{
				throw ADP.Argument(SR.Format("Expecting non-empty string for '{0}' parameter.", parameterName));
			}
		}

		internal static void CheckArgumentLength(Array value, string parameterName)
		{
			ADP.CheckArgumentNull(value, parameterName);
			if (value.Length == 0)
			{
				throw ADP.Argument(SR.Format("Expecting non-empty array for '{0}' parameter.", parameterName));
			}
		}

		internal static ArgumentOutOfRangeException InvalidAcceptRejectRule(AcceptRejectRule value)
		{
			return ADP.InvalidEnumerationValue(typeof(AcceptRejectRule), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidCatalogLocation(CatalogLocation value)
		{
			return ADP.InvalidEnumerationValue(typeof(CatalogLocation), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidConflictOptions(ConflictOption value)
		{
			return ADP.InvalidEnumerationValue(typeof(ConflictOption), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidDataRowState(DataRowState value)
		{
			return ADP.InvalidEnumerationValue(typeof(DataRowState), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidKeyRestrictionBehavior(KeyRestrictionBehavior value)
		{
			return ADP.InvalidEnumerationValue(typeof(KeyRestrictionBehavior), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidLoadOption(LoadOption value)
		{
			return ADP.InvalidEnumerationValue(typeof(LoadOption), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidMissingMappingAction(MissingMappingAction value)
		{
			return ADP.InvalidEnumerationValue(typeof(MissingMappingAction), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidMissingSchemaAction(MissingSchemaAction value)
		{
			return ADP.InvalidEnumerationValue(typeof(MissingSchemaAction), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidRule(Rule value)
		{
			return ADP.InvalidEnumerationValue(typeof(Rule), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidSchemaType(SchemaType value)
		{
			return ADP.InvalidEnumerationValue(typeof(SchemaType), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidStatementType(StatementType value)
		{
			return ADP.InvalidEnumerationValue(typeof(StatementType), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidUpdateStatus(UpdateStatus value)
		{
			return ADP.InvalidEnumerationValue(typeof(UpdateStatus), (int)value);
		}

		internal static ArgumentOutOfRangeException NotSupportedStatementType(StatementType value, string method)
		{
			return ADP.NotSupportedEnumerationValue(typeof(StatementType), value.ToString(), method);
		}

		internal static ArgumentException InvalidKeyname(string parameterName)
		{
			return ADP.Argument("Invalid keyword, contain one or more of 'no characters', 'control characters', 'leading or trailing whitespace' or 'leading semicolons'.", parameterName);
		}

		internal static ArgumentException InvalidValue(string parameterName)
		{
			return ADP.Argument("The value contains embedded nulls (\\\\u0000).", parameterName);
		}

		internal static Exception WrongType(Type got, Type expected)
		{
			return ADP.Argument(SR.Format("Expecting argument of type {1}, but received type {0}.", got.ToString(), expected.ToString()));
		}

		internal static Exception CollectionUniqueValue(Type itemType, string propertyName, string propertyValue)
		{
			return ADP.Argument(SR.Format("The {0}.{1} is required to be unique, '{2}' already exists in the collection.", itemType.Name, propertyName, propertyValue));
		}

		internal static InvalidOperationException MissingSelectCommand(string method)
		{
			return ADP.Provider(SR.Format("The SelectCommand property has not been initialized before calling '{0}'.", method));
		}

		private static InvalidOperationException DataMapping(string error)
		{
			return ADP.InvalidOperation(error);
		}

		internal static InvalidOperationException ColumnSchemaExpression(string srcColumn, string cacheColumn)
		{
			return ADP.DataMapping(SR.Format("The column mapping from SourceColumn '{0}' failed because the DataColumn '{1}' is a computed column.", srcColumn, cacheColumn));
		}

		internal static InvalidOperationException ColumnSchemaMismatch(string srcColumn, Type srcType, DataColumn column)
		{
			return ADP.DataMapping(SR.Format("Inconvertible type mismatch between SourceColumn '{0}' of {1} and the DataColumn '{2}' of {3}.", new object[]
			{
				srcColumn,
				srcType.Name,
				column.ColumnName,
				column.DataType.Name
			}));
		}

		internal static InvalidOperationException ColumnSchemaMissing(string cacheColumn, string tableName, string srcColumn)
		{
			if (string.IsNullOrEmpty(tableName))
			{
				return ADP.InvalidOperation(SR.Format("Missing the DataColumn '{0}' for the SourceColumn '{2}'.", cacheColumn, tableName, srcColumn));
			}
			return ADP.DataMapping(SR.Format("Missing the DataColumn '{0}' in the DataTable '{1}' for the SourceColumn '{2}'.", cacheColumn, tableName, srcColumn));
		}

		internal static InvalidOperationException MissingColumnMapping(string srcColumn)
		{
			return ADP.DataMapping(SR.Format("Missing SourceColumn mapping for '{0}'.", srcColumn));
		}

		internal static InvalidOperationException MissingTableSchema(string cacheTable, string srcTable)
		{
			return ADP.DataMapping(SR.Format("Missing the '{0}' DataTable for the '{1}' SourceTable.", cacheTable, srcTable));
		}

		internal static InvalidOperationException MissingTableMapping(string srcTable)
		{
			return ADP.DataMapping(SR.Format("Missing SourceTable mapping: '{0}'", srcTable));
		}

		internal static InvalidOperationException MissingTableMappingDestination(string dstTable)
		{
			return ADP.DataMapping(SR.Format("Missing TableMapping when TableMapping.DataSetTable='{0}'.", dstTable));
		}

		internal static Exception InvalidSourceColumn(string parameter)
		{
			return ADP.Argument("SourceColumn is required to be a non-empty string.", parameter);
		}

		internal static Exception ColumnsAddNullAttempt(string parameter)
		{
			return ADP.CollectionNullValue(parameter, typeof(DataColumnMappingCollection), typeof(DataColumnMapping));
		}

		internal static Exception ColumnsDataSetColumn(string cacheColumn)
		{
			return ADP.CollectionIndexString(typeof(DataColumnMapping), "DataSetColumn", cacheColumn, typeof(DataColumnMappingCollection));
		}

		internal static Exception ColumnsIndexInt32(int index, IColumnMappingCollection collection)
		{
			return ADP.CollectionIndexInt32(index, collection.GetType(), collection.Count);
		}

		internal static Exception ColumnsIndexSource(string srcColumn)
		{
			return ADP.CollectionIndexString(typeof(DataColumnMapping), "SourceColumn", srcColumn, typeof(DataColumnMappingCollection));
		}

		internal static Exception ColumnsIsNotParent(ICollection collection)
		{
			return ADP.ParametersIsNotParent(typeof(DataColumnMapping), collection);
		}

		internal static Exception ColumnsIsParent(ICollection collection)
		{
			return ADP.ParametersIsParent(typeof(DataColumnMapping), collection);
		}

		internal static Exception ColumnsUniqueSourceColumn(string srcColumn)
		{
			return ADP.CollectionUniqueValue(typeof(DataColumnMapping), "SourceColumn", srcColumn);
		}

		internal static Exception NotADataColumnMapping(object value)
		{
			return ADP.CollectionInvalidType(typeof(DataColumnMappingCollection), typeof(DataColumnMapping), value);
		}

		internal static Exception InvalidSourceTable(string parameter)
		{
			return ADP.Argument("SourceTable is required to be a non-empty string", parameter);
		}

		internal static Exception TablesAddNullAttempt(string parameter)
		{
			return ADP.CollectionNullValue(parameter, typeof(DataTableMappingCollection), typeof(DataTableMapping));
		}

		internal static Exception TablesDataSetTable(string cacheTable)
		{
			return ADP.CollectionIndexString(typeof(DataTableMapping), "DataSetTable", cacheTable, typeof(DataTableMappingCollection));
		}

		internal static Exception TablesIndexInt32(int index, ITableMappingCollection collection)
		{
			return ADP.CollectionIndexInt32(index, collection.GetType(), collection.Count);
		}

		internal static Exception TablesIsNotParent(ICollection collection)
		{
			return ADP.ParametersIsNotParent(typeof(DataTableMapping), collection);
		}

		internal static Exception TablesIsParent(ICollection collection)
		{
			return ADP.ParametersIsParent(typeof(DataTableMapping), collection);
		}

		internal static Exception TablesSourceIndex(string srcTable)
		{
			return ADP.CollectionIndexString(typeof(DataTableMapping), "SourceTable", srcTable, typeof(DataTableMappingCollection));
		}

		internal static Exception TablesUniqueSourceTable(string srcTable)
		{
			return ADP.CollectionUniqueValue(typeof(DataTableMapping), "SourceTable", srcTable);
		}

		internal static Exception NotADataTableMapping(object value)
		{
			return ADP.CollectionInvalidType(typeof(DataTableMappingCollection), typeof(DataTableMapping), value);
		}

		internal static InvalidOperationException UpdateConnectionRequired(StatementType statementType, bool isRowUpdatingCommand)
		{
			string text;
			if (!isRowUpdatingCommand)
			{
				switch (statementType)
				{
				case StatementType.Insert:
					text = "Update requires the InsertCommand to have a connection object. The Connection property of the InsertCommand has not been initialized.";
					goto IL_004A;
				case StatementType.Update:
					text = "Update requires the UpdateCommand to have a connection object. The Connection property of the UpdateCommand has not been initialized.";
					goto IL_004A;
				case StatementType.Delete:
					text = "Update requires the DeleteCommand to have a connection object. The Connection property of the DeleteCommand has not been initialized.";
					goto IL_004A;
				}
				throw ADP.InvalidStatementType(statementType);
			}
			text = "Update requires the command clone to have a connection object. The Connection property of the command clone has not been initialized.";
			IL_004A:
			return ADP.InvalidOperation(text);
		}

		internal static InvalidOperationException ConnectionRequired_Res(string method)
		{
			return ADP.InvalidOperation("ADP_ConnectionRequired_" + method);
		}

		internal static InvalidOperationException UpdateOpenConnectionRequired(StatementType statementType, bool isRowUpdatingCommand, ConnectionState state)
		{
			string text;
			if (isRowUpdatingCommand)
			{
				text = "Update requires the updating command to have an open connection object. {1}";
			}
			else
			{
				switch (statementType)
				{
				case StatementType.Insert:
					text = "Update requires the {0}Command to have an open connection object. {1}";
					break;
				case StatementType.Update:
					text = "Update requires the {0}Command to have an open connection object. {1}";
					break;
				case StatementType.Delete:
					text = "Update requires the {0}Command to have an open connection object. {1}";
					break;
				default:
					throw ADP.InvalidStatementType(statementType);
				}
			}
			return ADP.InvalidOperation(SR.Format(text, ADP.ConnectionStateMsg(state)));
		}

		internal static ArgumentException UnwantedStatementType(StatementType statementType)
		{
			return ADP.Argument(SR.Format("The StatementType {0} is not expected here.", statementType.ToString()));
		}

		internal static Exception FillSchemaRequiresSourceTableName(string parameter)
		{
			return ADP.Argument("FillSchema: expected a non-empty string for the SourceTable name.", parameter);
		}

		internal static Exception InvalidMaxRecords(string parameter, int max)
		{
			return ADP.Argument(SR.Format("The MaxRecords value of {0} is invalid; the value must be >= 0.", max.ToString(CultureInfo.InvariantCulture)), parameter);
		}

		internal static Exception InvalidStartRecord(string parameter, int start)
		{
			return ADP.Argument(SR.Format("The StartRecord value of {0} is invalid; the value must be >= 0.", start.ToString(CultureInfo.InvariantCulture)), parameter);
		}

		internal static Exception FillRequires(string parameter)
		{
			return ADP.ArgumentNull(parameter);
		}

		internal static Exception FillRequiresSourceTableName(string parameter)
		{
			return ADP.Argument("Fill: expected a non-empty string for the SourceTable name.", parameter);
		}

		internal static Exception FillChapterAutoIncrement()
		{
			return ADP.InvalidOperation("Hierarchical chapter columns must map to an AutoIncrement DataColumn.");
		}

		internal static InvalidOperationException MissingDataReaderFieldType(int index)
		{
			return ADP.DataAdapter(SR.Format("DataReader.GetFieldType({0}) returned null.", index));
		}

		internal static InvalidOperationException OnlyOneTableForStartRecordOrMaxRecords()
		{
			return ADP.DataAdapter("Only specify one item in the dataTables array when using non-zero values for startRecords or maxRecords.");
		}

		internal static ArgumentNullException UpdateRequiresNonNullDataSet(string parameter)
		{
			return ADP.ArgumentNull(parameter);
		}

		internal static InvalidOperationException UpdateRequiresSourceTable(string defaultSrcTableName)
		{
			return ADP.InvalidOperation(SR.Format("Update unable to find TableMapping['{0}'] or DataTable '{0}'.", defaultSrcTableName));
		}

		internal static InvalidOperationException UpdateRequiresSourceTableName(string srcTable)
		{
			return ADP.InvalidOperation(SR.Format("Update: expected a non-empty SourceTable name.", srcTable));
		}

		internal static ArgumentNullException UpdateRequiresDataTable(string parameter)
		{
			return ADP.ArgumentNull(parameter);
		}

		internal static Exception UpdateConcurrencyViolation(StatementType statementType, int affected, int expected, DataRow[] dataRows)
		{
			string text;
			switch (statementType)
			{
			case StatementType.Update:
				text = "Concurrency violation: the UpdateCommand affected {0} of the expected {1} records.";
				break;
			case StatementType.Delete:
				text = "Concurrency violation: the DeleteCommand affected {0} of the expected {1} records.";
				break;
			case StatementType.Batch:
				text = "Concurrency violation: the batched command affected {0} of the expected {1} records.";
				break;
			default:
				throw ADP.InvalidStatementType(statementType);
			}
			DBConcurrencyException ex = new DBConcurrencyException(SR.Format(text, affected.ToString(CultureInfo.InvariantCulture), expected.ToString(CultureInfo.InvariantCulture)), null, dataRows);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static InvalidOperationException UpdateRequiresCommand(StatementType statementType, bool isRowUpdatingCommand)
		{
			string text;
			if (isRowUpdatingCommand)
			{
				text = "Update requires the command clone to be valid.";
			}
			else
			{
				switch (statementType)
				{
				case StatementType.Select:
					text = "Auto SQL generation during Update requires a valid SelectCommand.";
					break;
				case StatementType.Insert:
					text = "Update requires a valid InsertCommand when passed DataRow collection with new rows.";
					break;
				case StatementType.Update:
					text = "Update requires a valid UpdateCommand when passed DataRow collection with modified rows.";
					break;
				case StatementType.Delete:
					text = "Update requires a valid DeleteCommand when passed DataRow collection with deleted rows.";
					break;
				default:
					throw ADP.InvalidStatementType(statementType);
				}
			}
			return ADP.InvalidOperation(text);
		}

		internal static ArgumentException UpdateMismatchRowTable(int i)
		{
			return ADP.Argument(SR.Format("DataRow[{0}] is from a different DataTable than DataRow[0].", i.ToString(CultureInfo.InvariantCulture)));
		}

		internal static DataException RowUpdatedErrors()
		{
			return ADP.Data("RowUpdatedEvent: Errors occurred; no additional is information available.");
		}

		internal static DataException RowUpdatingErrors()
		{
			return ADP.Data("RowUpdatingEvent: Errors occurred; no additional is information available.");
		}

		internal static InvalidOperationException ResultsNotAllowedDuringBatch()
		{
			return ADP.DataAdapter("When batching, the command's UpdatedRowSource property value of UpdateRowSource.FirstReturnedRecord or UpdateRowSource.Both is invalid.");
		}

		internal static InvalidOperationException DynamicSQLJoinUnsupported()
		{
			return ADP.InvalidOperation("Dynamic SQL generation is not supported against multiple base tables.");
		}

		internal static InvalidOperationException DynamicSQLNoTableInfo()
		{
			return ADP.InvalidOperation("Dynamic SQL generation is not supported against a SelectCommand that does not return any base table information.");
		}

		internal static InvalidOperationException DynamicSQLNoKeyInfoDelete()
		{
			return ADP.InvalidOperation("Dynamic SQL generation for the DeleteCommand is not supported against a SelectCommand that does not return any key column information.");
		}

		internal static InvalidOperationException DynamicSQLNoKeyInfoUpdate()
		{
			return ADP.InvalidOperation("Dynamic SQL generation for the UpdateCommand is not supported against a SelectCommand that does not return any key column information.");
		}

		internal static InvalidOperationException DynamicSQLNoKeyInfoRowVersionDelete()
		{
			return ADP.InvalidOperation("Dynamic SQL generation for the DeleteCommand is not supported against a SelectCommand that does not contain a row version column.");
		}

		internal static InvalidOperationException DynamicSQLNoKeyInfoRowVersionUpdate()
		{
			return ADP.InvalidOperation("Dynamic SQL generation for the UpdateCommand is not supported against a SelectCommand that does not contain a row version column.");
		}

		internal static InvalidOperationException DynamicSQLNestedQuote(string name, string quote)
		{
			return ADP.InvalidOperation(SR.Format("Dynamic SQL generation not supported against table names '{0}' that contain the QuotePrefix or QuoteSuffix character '{1}'.", name, quote));
		}

		internal static InvalidOperationException NoQuoteChange()
		{
			return ADP.InvalidOperation("The QuotePrefix and QuoteSuffix properties cannot be changed once an Insert, Update, or Delete command has been generated.");
		}

		internal static InvalidOperationException MissingSourceCommand()
		{
			return ADP.InvalidOperation("The DataAdapter.SelectCommand property needs to be initialized.");
		}

		internal static InvalidOperationException MissingSourceCommandConnection()
		{
			return ADP.InvalidOperation("The DataAdapter.SelectCommand.Connection property needs to be initialized;");
		}

		internal static DataRow[] SelectAdapterRows(DataTable dataTable, bool sorted)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			DataRowCollection rows = dataTable.Rows;
			foreach (object obj in rows)
			{
				DataRowState dataRowState = ((DataRow)obj).RowState;
				if (dataRowState != DataRowState.Added)
				{
					if (dataRowState != DataRowState.Deleted)
					{
						if (dataRowState == DataRowState.Modified)
						{
							num3++;
						}
					}
					else
					{
						num2++;
					}
				}
				else
				{
					num++;
				}
			}
			DataRow[] array = new DataRow[num + num2 + num3];
			if (sorted)
			{
				num3 = num + num2;
				num2 = num;
				num = 0;
				using (IEnumerator enumerator = rows.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj2 = enumerator.Current;
						DataRow dataRow = (DataRow)obj2;
						DataRowState dataRowState = dataRow.RowState;
						if (dataRowState != DataRowState.Added)
						{
							if (dataRowState != DataRowState.Deleted)
							{
								if (dataRowState == DataRowState.Modified)
								{
									array[num3++] = dataRow;
								}
							}
							else
							{
								array[num2++] = dataRow;
							}
						}
						else
						{
							array[num++] = dataRow;
						}
					}
					return array;
				}
			}
			int num4 = 0;
			foreach (object obj3 in rows)
			{
				DataRow dataRow2 = (DataRow)obj3;
				if ((dataRow2.RowState & (DataRowState.Added | DataRowState.Deleted | DataRowState.Modified)) != (DataRowState)0)
				{
					array[num4++] = dataRow2;
					if (num4 == array.Length)
					{
						break;
					}
				}
			}
			return array;
		}

		internal static void BuildSchemaTableInfoTableNames(string[] columnNameArray)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>(columnNameArray.Length);
			int num = columnNameArray.Length;
			int num2 = columnNameArray.Length - 1;
			while (0 <= num2)
			{
				string text = columnNameArray[num2];
				if (text != null && 0 < text.Length)
				{
					text = text.ToLower(CultureInfo.InvariantCulture);
					int num3;
					if (dictionary.TryGetValue(text, out num3))
					{
						num = Math.Min(num, num3);
					}
					dictionary[text] = num2;
				}
				else
				{
					columnNameArray[num2] = string.Empty;
					num = num2;
				}
				num2--;
			}
			int num4 = 1;
			for (int i = num; i < columnNameArray.Length; i++)
			{
				string text2 = columnNameArray[i];
				if (text2.Length == 0)
				{
					columnNameArray[i] = "Column";
					num4 = ADP.GenerateUniqueName(dictionary, ref columnNameArray[i], i, num4);
				}
				else
				{
					text2 = text2.ToLower(CultureInfo.InvariantCulture);
					if (i != dictionary[text2])
					{
						ADP.GenerateUniqueName(dictionary, ref columnNameArray[i], i, 1);
					}
				}
			}
		}

		private static int GenerateUniqueName(Dictionary<string, int> hash, ref string columnName, int index, int uniqueIndex)
		{
			string text;
			for (;;)
			{
				text = columnName + uniqueIndex.ToString(CultureInfo.InvariantCulture);
				string text2 = text.ToLower(CultureInfo.InvariantCulture);
				if (hash.TryAdd(text2, index))
				{
					break;
				}
				uniqueIndex++;
			}
			columnName = text;
			return uniqueIndex;
		}

		internal static int SrcCompare(string strA, string strB)
		{
			if (!(strA == strB))
			{
				return 1;
			}
			return 0;
		}

		internal static Exception ExceptionWithStackTrace(Exception e)
		{
			try
			{
				throw e;
			}
			catch (Exception ex)
			{
			}
			Exception ex;
			return ex;
		}

		internal static IndexOutOfRangeException IndexOutOfRange(int value)
		{
			return new IndexOutOfRangeException(value.ToString(CultureInfo.InvariantCulture));
		}

		internal static IndexOutOfRangeException IndexOutOfRange()
		{
			return new IndexOutOfRangeException();
		}

		internal static TimeoutException TimeoutException(string error)
		{
			return new TimeoutException(error);
		}

		internal static InvalidOperationException InvalidOperation(string error, Exception inner)
		{
			return new InvalidOperationException(error, inner);
		}

		internal static OverflowException Overflow(string error)
		{
			return ADP.Overflow(error, null);
		}

		internal static OverflowException Overflow(string error, Exception inner)
		{
			return new OverflowException(error, inner);
		}

		internal static TypeLoadException TypeLoad(string error)
		{
			TypeLoadException ex = new TypeLoadException(error);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static PlatformNotSupportedException DbTypeNotSupported(string dbType)
		{
			return new PlatformNotSupportedException(SR.GetString("Type {0} is not supported on this platform.", new object[] { dbType }));
		}

		internal static InvalidCastException InvalidCast()
		{
			return new InvalidCastException();
		}

		internal static IOException IO(string error)
		{
			return new IOException(error);
		}

		internal static IOException IO(string error, Exception inner)
		{
			return new IOException(error, inner);
		}

		internal static ObjectDisposedException ObjectDisposed(object instance)
		{
			return new ObjectDisposedException(instance.GetType().Name);
		}

		internal static Exception DataTableDoesNotExist(string collectionName)
		{
			return ADP.Argument(SR.GetString("The collection '{0}' is missing from the metadata XML.", new object[] { collectionName }));
		}

		internal static InvalidOperationException MethodCalledTwice(string method)
		{
			return new InvalidOperationException(SR.GetString("The method '{0}' cannot be called more than once for the same execution.", new object[] { method }));
		}

		internal static ArgumentOutOfRangeException InvalidCommandType(CommandType value)
		{
			return ADP.InvalidEnumerationValue(typeof(CommandType), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidIsolationLevel(IsolationLevel value)
		{
			return ADP.InvalidEnumerationValue(typeof(IsolationLevel), (int)value);
		}

		internal static ArgumentOutOfRangeException InvalidParameterDirection(ParameterDirection value)
		{
			return ADP.InvalidEnumerationValue(typeof(ParameterDirection), (int)value);
		}

		internal static Exception TooManyRestrictions(string collectionName)
		{
			return ADP.Argument(SR.GetString("More restrictions were provided than the requested schema ('{0}') supports.", new object[] { collectionName }));
		}

		internal static ArgumentOutOfRangeException InvalidUpdateRowSource(UpdateRowSource value)
		{
			return ADP.InvalidEnumerationValue(typeof(UpdateRowSource), (int)value);
		}

		internal static ArgumentException InvalidMinMaxPoolSizeValues()
		{
			return ADP.Argument(SR.GetString("Invalid min or max pool size values, min pool size cannot be greater than the max pool size."));
		}

		internal static InvalidOperationException NoConnectionString()
		{
			return ADP.InvalidOperation(SR.GetString("The ConnectionString property has not been initialized."));
		}

		internal static Exception MethodNotImplemented([CallerMemberName] string methodName = "")
		{
			return global::System.NotImplemented.ByDesignWithMessage(methodName);
		}

		internal static Exception QueryFailed(string collectionName, Exception e)
		{
			return ADP.InvalidOperation(SR.GetString("Unable to build the '{0}' collection because execution of the SQL query failed. See the inner exception for details.", new object[] { collectionName }), e);
		}

		internal static Exception InvalidConnectionOptionValueLength(string key, int limit)
		{
			return ADP.Argument(SR.GetString("The value's length for key '{0}' exceeds it's limit of '{1}'.", new object[] { key, limit }));
		}

		internal static Exception MissingConnectionOptionValue(string key, string requiredAdditionalKey)
		{
			return ADP.Argument(SR.GetString("Use of key '{0}' requires the key '{1}' to be present.", new object[] { key, requiredAdditionalKey }));
		}

		internal static Exception PooledOpenTimeout()
		{
			return ADP.InvalidOperation(SR.GetString("Timeout expired.  The timeout period elapsed prior to obtaining a connection from the pool.  This may have occurred because all pooled connections were in use and max pool size was reached."));
		}

		internal static Exception NonPooledOpenTimeout()
		{
			return ADP.TimeoutException(SR.GetString("Timeout attempting to open the connection.  The time period elapsed prior to attempting to open the connection has been exceeded.  This may have occurred because of too many simultaneous non-pooled connection attempts."));
		}

		internal static InvalidOperationException TransactionConnectionMismatch()
		{
			return ADP.Provider(SR.GetString("The transaction is either not associated with the current connection or has been completed."));
		}

		internal static InvalidOperationException TransactionRequired(string method)
		{
			return ADP.Provider(SR.GetString("{0} requires the command to have a transaction when the connection assigned to the command is in a pending local transaction.  The Transaction property of the command has not been initialized.", new object[] { method }));
		}

		internal static Exception CommandTextRequired(string method)
		{
			return ADP.InvalidOperation(SR.GetString("{0}: CommandText property has not been initialized", new object[] { method }));
		}

		internal static Exception NoColumns()
		{
			return ADP.Argument(SR.GetString("The schema table contains no columns."));
		}

		internal static InvalidOperationException ConnectionRequired(string method)
		{
			return ADP.InvalidOperation(SR.GetString("{0}: Connection property has not been initialized.", new object[] { method }));
		}

		internal static InvalidOperationException OpenConnectionRequired(string method, ConnectionState state)
		{
			return ADP.InvalidOperation(SR.GetString("{0} requires an open and available Connection. {1}", new object[]
			{
				method,
				ADP.ConnectionStateMsg(state)
			}));
		}

		internal static Exception OpenReaderExists()
		{
			return ADP.OpenReaderExists(null);
		}

		internal static Exception OpenReaderExists(Exception e)
		{
			return ADP.InvalidOperation(SR.GetString("There is already an open DataReader associated with this Command which must be closed first."), e);
		}

		internal static Exception NonSeqByteAccess(long badIndex, long currIndex, string method)
		{
			return ADP.InvalidOperation(SR.GetString("Invalid {2} attempt at dataIndex '{0}'.  With CommandBehavior.SequentialAccess, you may only read from dataIndex '{1}' or greater.", new object[]
			{
				badIndex.ToString(CultureInfo.InvariantCulture),
				currIndex.ToString(CultureInfo.InvariantCulture),
				method
			}));
		}

		internal static Exception InvalidXml()
		{
			return ADP.Argument(SR.GetString("The metadata XML is invalid."));
		}

		internal static Exception NegativeParameter(string parameterName)
		{
			return ADP.InvalidOperation(SR.GetString("Invalid value for argument '{0}'. The value must be greater than or equal to 0.", new object[] { parameterName }));
		}

		internal static Exception InvalidXmlMissingColumn(string collectionName, string columnName)
		{
			return ADP.Argument(SR.GetString("The metadata XML is invalid. The {0} collection must contain a {1} column and it must be a string column.", new object[] { collectionName, columnName }));
		}

		internal static Exception InvalidMetaDataValue()
		{
			return ADP.Argument(SR.GetString("Invalid value for this metadata."));
		}

		internal static InvalidOperationException NonSequentialColumnAccess(int badCol, int currCol)
		{
			return ADP.InvalidOperation(SR.GetString("Invalid attempt to read from column ordinal '{0}'.  With CommandBehavior.SequentialAccess, you may only read from column ordinal '{1}' or greater.", new object[]
			{
				badCol.ToString(CultureInfo.InvariantCulture),
				currCol.ToString(CultureInfo.InvariantCulture)
			}));
		}

		internal static Exception InvalidXmlInvalidValue(string collectionName, string columnName)
		{
			return ADP.Argument(SR.GetString("The metadata XML is invalid. The {1} column of the {0} collection must contain a non-empty string.", new object[] { collectionName, columnName }));
		}

		internal static Exception CollectionNameIsNotUnique(string collectionName)
		{
			return ADP.Argument(SR.GetString("There are multiple collections named '{0}'.", new object[] { collectionName }));
		}

		internal static Exception InvalidCommandTimeout(int value, [CallerMemberName] string property = "")
		{
			return ADP.Argument(SR.GetString("Invalid CommandTimeout value {0}; the value must be >= 0.", new object[] { value.ToString(CultureInfo.InvariantCulture) }), property);
		}

		internal static Exception UninitializedParameterSize(int index, Type dataType)
		{
			return ADP.InvalidOperation(SR.GetString("{1}[{0}]: the Size property has an invalid size of 0.", new object[]
			{
				index.ToString(CultureInfo.InvariantCulture),
				dataType.Name
			}));
		}

		internal static Exception UnableToBuildCollection(string collectionName)
		{
			return ADP.Argument(SR.GetString("Unable to build schema collection '{0}';", new object[] { collectionName }));
		}

		internal static Exception PrepareParameterType(DbCommand cmd)
		{
			return ADP.InvalidOperation(SR.GetString("{0}.Prepare method requires all parameters to have an explicitly set type.", new object[] { cmd.GetType().Name }));
		}

		internal static Exception UndefinedCollection(string collectionName)
		{
			return ADP.Argument(SR.GetString("The requested collection ({0}) is not defined.", new object[] { collectionName }));
		}

		internal static Exception UnsupportedVersion(string collectionName)
		{
			return ADP.Argument(SR.GetString(" requested collection ({0}) is not supported by this version of the provider.", new object[] { collectionName }));
		}

		internal static Exception AmbigousCollectionName(string collectionName)
		{
			return ADP.Argument(SR.GetString("The collection name '{0}' matches at least two collections with the same name but with different case, but does not match any of them exactly.", new object[] { collectionName }));
		}

		internal static Exception PrepareParameterSize(DbCommand cmd)
		{
			return ADP.InvalidOperation(SR.GetString("{0}.Prepare method requires all variable length parameters to have an explicitly set non-zero Size.", new object[] { cmd.GetType().Name }));
		}

		internal static Exception PrepareParameterScale(DbCommand cmd, string type)
		{
			return ADP.InvalidOperation(SR.GetString("{0}.Prepare method requires parameters of type '{1}' have an explicitly set Precision and Scale.", new object[]
			{
				cmd.GetType().Name,
				type
			}));
		}

		internal static Exception MissingDataSourceInformationColumn()
		{
			return ADP.Argument(SR.GetString("One of the required DataSourceInformation tables columns is missing."));
		}

		internal static Exception IncorrectNumberOfDataSourceInformationRows()
		{
			return ADP.Argument(SR.GetString("The DataSourceInformation table must contain exactly one row."));
		}

		internal static Exception MismatchedAsyncResult(string expectedMethod, string gotMethod)
		{
			return ADP.InvalidOperation(SR.GetString("Mismatched end method call for asyncResult.  Expected call to {0} but {1} was called instead.", new object[] { expectedMethod, gotMethod }));
		}

		internal static Exception ClosedConnectionError()
		{
			return ADP.InvalidOperation(SR.GetString("Invalid operation. The connection is closed."));
		}

		internal static Exception ConnectionAlreadyOpen(ConnectionState state)
		{
			return ADP.InvalidOperation(SR.GetString("The connection was not closed. {0}", new object[] { ADP.ConnectionStateMsg(state) }));
		}

		internal static Exception TransactionPresent()
		{
			return ADP.InvalidOperation(SR.GetString("Connection currently has transaction enlisted.  Finish current transaction and retry."));
		}

		internal static Exception LocalTransactionPresent()
		{
			return ADP.InvalidOperation(SR.GetString("Cannot enlist in the transaction because a local transaction is in progress on the connection.  Finish local transaction and retry."));
		}

		internal static Exception OpenConnectionPropertySet(string property, ConnectionState state)
		{
			return ADP.InvalidOperation(SR.GetString("Not allowed to change the '{0}' property. {1}", new object[]
			{
				property,
				ADP.ConnectionStateMsg(state)
			}));
		}

		internal static Exception EmptyDatabaseName()
		{
			return ADP.Argument(SR.GetString("Database cannot be null, the empty string, or string of only whitespace."));
		}

		internal static Exception MissingRestrictionColumn()
		{
			return ADP.Argument(SR.GetString("One or more of the required columns of the restrictions collection is missing."));
		}

		internal static Exception InternalConnectionError(ADP.ConnectionError internalError)
		{
			return ADP.InvalidOperation(SR.GetString("Internal DbConnection Error: {0}", new object[] { (int)internalError }));
		}

		internal static Exception InvalidConnectRetryCountValue()
		{
			return ADP.Argument(SR.GetString("Invalid ConnectRetryCount value (should be 0-255)."));
		}

		internal static Exception MissingRestrictionRow()
		{
			return ADP.Argument(SR.GetString("A restriction exists for which there is no matching row in the restrictions collection."));
		}

		internal static Exception InvalidConnectRetryIntervalValue()
		{
			return ADP.Argument(SR.GetString("Invalid ConnectRetryInterval value (should be 1-60)."));
		}

		internal static InvalidOperationException AsyncOperationPending()
		{
			return ADP.InvalidOperation(SR.GetString("Can not start another operation while there is an asynchronous operation pending."));
		}

		internal static IOException ErrorReadingFromStream(Exception internalException)
		{
			return ADP.IO(SR.GetString("An error occurred while reading."), internalException);
		}

		internal static ArgumentException InvalidDataType(TypeCode typecode)
		{
			return ADP.Argument(SR.GetString("The parameter data type of {0} is invalid.", new object[] { typecode.ToString() }));
		}

		internal static ArgumentException UnknownDataType(Type dataType)
		{
			return ADP.Argument(SR.GetString("No mapping exists from object type {0} to a known managed provider native type.", new object[] { dataType.FullName }));
		}

		internal static ArgumentException DbTypeNotSupported(DbType type, Type enumtype)
		{
			return ADP.Argument(SR.GetString("No mapping exists from DbType {0} to a known {1}.", new object[]
			{
				type.ToString(),
				enumtype.Name
			}));
		}

		internal static ArgumentException UnknownDataTypeCode(Type dataType, TypeCode typeCode)
		{
			string text = "Unable to handle an unknown TypeCode {0} returned by Type {1}.";
			object[] array = new object[2];
			int num = 0;
			int num2 = (int)typeCode;
			array[num] = num2.ToString(CultureInfo.InvariantCulture);
			array[1] = dataType.FullName;
			return ADP.Argument(SR.GetString(text, array));
		}

		internal static ArgumentException InvalidOffsetValue(int value)
		{
			return ADP.Argument(SR.GetString("Invalid parameter Offset value '{0}'. The value must be greater than or equal to 0.", new object[] { value.ToString(CultureInfo.InvariantCulture) }));
		}

		internal static ArgumentException InvalidSizeValue(int value)
		{
			return ADP.Argument(SR.GetString("Invalid parameter Size value '{0}'. The value must be greater than or equal to 0.", new object[] { value.ToString(CultureInfo.InvariantCulture) }));
		}

		internal static ArgumentException ParameterValueOutOfRange(decimal value)
		{
			return ADP.Argument(SR.GetString("Parameter value '{0}' is out of range.", new object[] { value.ToString(null) }));
		}

		internal static ArgumentException ParameterValueOutOfRange(SqlDecimal value)
		{
			return ADP.Argument(SR.GetString("Parameter value '{0}' is out of range.", new object[] { value.ToString() }));
		}

		internal static ArgumentException VersionDoesNotSupportDataType(string typeName)
		{
			return ADP.Argument(SR.GetString("The version of SQL Server in use does not support datatype '{0}'.", new object[] { typeName }));
		}

		internal static Exception ParameterConversionFailed(object value, Type destType, Exception inner)
		{
			string @string = SR.GetString("Failed to convert parameter value from a {0} to a {1}.", new object[]
			{
				value.GetType().Name,
				destType.Name
			});
			Exception ex;
			if (inner is ArgumentException)
			{
				ex = new ArgumentException(@string, inner);
			}
			else if (inner is FormatException)
			{
				ex = new FormatException(@string, inner);
			}
			else if (inner is InvalidCastException)
			{
				ex = new InvalidCastException(@string, inner);
			}
			else if (inner is OverflowException)
			{
				ex = new OverflowException(@string, inner);
			}
			else
			{
				ex = inner;
			}
			return ex;
		}

		internal static Exception ParametersMappingIndex(int index, DbParameterCollection collection)
		{
			return ADP.CollectionIndexInt32(index, collection.GetType(), collection.Count);
		}

		internal static Exception ParametersSourceIndex(string parameterName, DbParameterCollection collection, Type parameterType)
		{
			return ADP.CollectionIndexString(parameterType, "ParameterName", parameterName, collection.GetType());
		}

		internal static Exception ParameterNull(string parameter, DbParameterCollection collection, Type parameterType)
		{
			return ADP.CollectionNullValue(parameter, collection.GetType(), parameterType);
		}

		internal static Exception UndefinedPopulationMechanism(string populationMechanism)
		{
			throw new NotImplementedException();
		}

		internal static Exception InvalidParameterType(DbParameterCollection collection, Type parameterType, object invalidValue)
		{
			return ADP.CollectionInvalidType(collection.GetType(), parameterType, invalidValue);
		}

		internal static Exception ParallelTransactionsNotSupported(DbConnection obj)
		{
			return ADP.InvalidOperation(SR.GetString("{0} does not support parallel transactions.", new object[] { obj.GetType().Name }));
		}

		internal static Exception TransactionZombied(DbTransaction obj)
		{
			return ADP.InvalidOperation(SR.GetString("This {0} has completed; it is no longer usable.", new object[] { obj.GetType().Name }));
		}

		internal static Delegate FindBuilder(MulticastDelegate mcd)
		{
			if (mcd != null)
			{
				foreach (Delegate @delegate in mcd.GetInvocationList())
				{
					if (@delegate.Target is DbCommandBuilder)
					{
						return @delegate;
					}
				}
			}
			return null;
		}

		internal static void TimerCurrent(out long ticks)
		{
			ticks = DateTime.UtcNow.ToFileTimeUtc();
		}

		internal static long TimerCurrent()
		{
			return DateTime.UtcNow.ToFileTimeUtc();
		}

		internal static long TimerFromSeconds(int seconds)
		{
			checked
			{
				return unchecked((long)seconds) * 10000000L;
			}
		}

		internal static long TimerFromMilliseconds(long milliseconds)
		{
			return checked(milliseconds * 10000L);
		}

		internal static bool TimerHasExpired(long timerExpire)
		{
			return ADP.TimerCurrent() > timerExpire;
		}

		internal static long TimerRemaining(long timerExpire)
		{
			long num = ADP.TimerCurrent();
			return checked(timerExpire - num);
		}

		internal static long TimerRemainingMilliseconds(long timerExpire)
		{
			return ADP.TimerToMilliseconds(ADP.TimerRemaining(timerExpire));
		}

		internal static long TimerRemainingSeconds(long timerExpire)
		{
			return ADP.TimerToSeconds(ADP.TimerRemaining(timerExpire));
		}

		internal static long TimerToMilliseconds(long timerValue)
		{
			return timerValue / 10000L;
		}

		private static long TimerToSeconds(long timerValue)
		{
			return timerValue / 10000000L;
		}

		internal static string MachineName()
		{
			return Environment.MachineName;
		}

		internal static Transaction GetCurrentTransaction()
		{
			return Transaction.Current;
		}

		internal static bool IsDirection(DbParameter value, ParameterDirection condition)
		{
			return condition == (condition & value.Direction);
		}

		internal static void IsNullOrSqlType(object value, out bool isNull, out bool isSqlType)
		{
			if (value == null || value == DBNull.Value)
			{
				isNull = true;
				isSqlType = false;
				return;
			}
			INullable nullable = value as INullable;
			if (nullable != null)
			{
				isNull = nullable.IsNull;
				isSqlType = value is SqlBinary || value is SqlBoolean || value is SqlByte || value is SqlBytes || value is SqlChars || value is SqlDateTime || value is SqlDecimal || value is SqlDouble || value is SqlGuid || value is SqlInt16 || value is SqlInt32 || value is SqlInt64 || value is SqlMoney || value is SqlSingle || value is SqlString;
				return;
			}
			isNull = false;
			isSqlType = false;
		}

		internal static Version GetAssemblyVersion()
		{
			if (ADP.s_systemDataVersion == null)
			{
				ADP.s_systemDataVersion = new Version("4.6.57.0");
			}
			return ADP.s_systemDataVersion;
		}

		internal static bool IsAzureSqlServerEndpoint(string dataSource)
		{
			int i = dataSource.LastIndexOf(',');
			if (i >= 0)
			{
				dataSource = dataSource.Substring(0, i);
			}
			i = dataSource.LastIndexOf('\\');
			if (i >= 0)
			{
				dataSource = dataSource.Substring(0, i);
			}
			dataSource = dataSource.Trim();
			for (i = 0; i < ADP.AzureSqlServerEndpoints.Length; i++)
			{
				if (dataSource.EndsWith(ADP.AzureSqlServerEndpoints[i], StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		internal static ArgumentOutOfRangeException InvalidDataRowVersion(DataRowVersion value)
		{
			return ADP.InvalidEnumerationValue(typeof(DataRowVersion), (int)value);
		}

		internal static ArgumentException SingleValuedProperty(string propertyName, string value)
		{
			ArgumentException ex = new ArgumentException(SR.GetString("The only acceptable value for the property '{0}' is '{1}'.", new object[] { propertyName, value }));
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentException DoubleValuedProperty(string propertyName, string value1, string value2)
		{
			ArgumentException ex = new ArgumentException(SR.GetString("The acceptable values for the property '{0}' are '{1}' or '{2}'.", new object[] { propertyName, value1, value2 }));
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentException InvalidPrefixSuffix()
		{
			ArgumentException ex = new ArgumentException(SR.GetString("Specified QuotePrefix and QuoteSuffix values do not match."));
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentOutOfRangeException InvalidCommandBehavior(CommandBehavior value)
		{
			return ADP.InvalidEnumerationValue(typeof(CommandBehavior), (int)value);
		}

		internal static void ValidateCommandBehavior(CommandBehavior value)
		{
			if (value < CommandBehavior.Default || (CommandBehavior.SingleResult | CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo | CommandBehavior.SingleRow | CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection) < value)
			{
				throw ADP.InvalidCommandBehavior(value);
			}
		}

		internal static ArgumentOutOfRangeException NotSupportedCommandBehavior(CommandBehavior value, string method)
		{
			return ADP.NotSupportedEnumerationValue(typeof(CommandBehavior), value.ToString(), method);
		}

		internal static ArgumentException BadParameterName(string parameterName)
		{
			ArgumentException ex = new ArgumentException(SR.GetString("Specified parameter name '{0}' is not valid.", new object[] { parameterName }));
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static Exception DeriveParametersNotSupported(IDbCommand value)
		{
			return ADP.DataAdapter(SR.GetString("{0} DeriveParameters only supports CommandType.StoredProcedure, not CommandType. {1}.", new object[]
			{
				value.GetType().Name,
				value.CommandType.ToString()
			}));
		}

		internal static Exception NoStoredProcedureExists(string sproc)
		{
			return ADP.InvalidOperation(SR.GetString("The stored procedure '{0}' doesn't exist.", new object[] { sproc }));
		}

		internal static InvalidOperationException TransactionCompletedButNotDisposed()
		{
			return ADP.Provider(SR.GetString("The transaction associated with the current connection has completed but has not been disposed.  The transaction must be disposed before the connection can be used to execute SQL statements."));
		}

		internal static ArgumentOutOfRangeException InvalidUserDefinedTypeSerializationFormat(Format value)
		{
			return ADP.InvalidEnumerationValue(typeof(Format), (int)value);
		}

		internal static ArgumentOutOfRangeException NotSupportedUserDefinedTypeSerializationFormat(Format value, string method)
		{
			return ADP.NotSupportedEnumerationValue(typeof(Format), value.ToString(), method);
		}

		internal static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string parameterName, object value)
		{
			ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException(parameterName, value, message);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentException InvalidArgumentLength(string argumentName, int limit)
		{
			return ADP.Argument(SR.GetString("The length of argument '{0}' exceeds its limit of '{1}'.", new object[] { argumentName, limit }));
		}

		internal static ArgumentException MustBeReadOnly(string argumentName)
		{
			return ADP.Argument(SR.GetString("{0} must be marked as read only.", new object[] { argumentName }));
		}

		internal static InvalidOperationException InvalidMixedUsageOfSecureAndClearCredential()
		{
			return ADP.InvalidOperation(SR.GetString("Cannot use Credential with UserID, UID, Password, or PWD connection string keywords."));
		}

		internal static ArgumentException InvalidMixedArgumentOfSecureAndClearCredential()
		{
			return ADP.Argument(SR.GetString("Cannot use Credential with UserID, UID, Password, or PWD connection string keywords."));
		}

		internal static InvalidOperationException InvalidMixedUsageOfSecureCredentialAndIntegratedSecurity()
		{
			return ADP.InvalidOperation(SR.GetString("Cannot use Credential with Integrated Security connection string keyword."));
		}

		internal static ArgumentException InvalidMixedArgumentOfSecureCredentialAndIntegratedSecurity()
		{
			return ADP.Argument(SR.GetString("Cannot use Credential with Integrated Security connection string keyword."));
		}

		internal static InvalidOperationException InvalidMixedUsageOfAccessTokenAndIntegratedSecurity()
		{
			return ADP.InvalidOperation(SR.GetString("Cannot set the AccessToken property if the 'Integrated Security' connection string keyword has been set to 'true' or 'SSPI'."));
		}

		internal static InvalidOperationException InvalidMixedUsageOfAccessTokenAndUserIDPassword()
		{
			return ADP.InvalidOperation(SR.GetString("Cannot set the AccessToken property if 'UserID', 'UID', 'Password', or 'PWD' has been specified in connection string."));
		}

		internal static Exception InvalidMixedUsageOfCredentialAndAccessToken()
		{
			return ADP.InvalidOperation(SR.GetString("Cannot set the Credential property if the AccessToken property is already set."));
		}

		internal static bool NeedManualEnlistment()
		{
			return false;
		}

		internal static bool IsEmpty(string str)
		{
			return string.IsNullOrEmpty(str);
		}

		internal static Exception DatabaseNameTooLong()
		{
			return ADP.Argument(SR.GetString("The argument is too long."));
		}

		internal static int StringLength(string inputString)
		{
			if (inputString == null)
			{
				return 0;
			}
			return inputString.Length;
		}

		internal static Exception NumericToDecimalOverflow()
		{
			return ADP.InvalidCast(SR.GetString("The numerical value is too large to fit into a 96 bit decimal."));
		}

		internal static Exception OdbcNoTypesFromProvider()
		{
			return ADP.InvalidOperation(SR.GetString("The ODBC provider did not return results from SQLGETTYPEINFO."));
		}

		internal static ArgumentException InvalidRestrictionValue(string collectionName, string restrictionName, string restrictionValue)
		{
			return ADP.Argument(SR.GetString("'{2}' is not a valid value for the '{1}' restriction of the '{0}' schema collection.", new object[] { collectionName, restrictionName, restrictionValue }));
		}

		internal static Exception DataReaderNoData()
		{
			return ADP.InvalidOperation(SR.GetString("No data exists for the row/column."));
		}

		internal static Exception ConnectionIsDisabled(Exception InnerException)
		{
			return ADP.InvalidOperation(SR.GetString("The connection has been disabled."), InnerException);
		}

		internal static Exception OffsetOutOfRangeException()
		{
			return ADP.InvalidOperation(SR.GetString("Offset must refer to a location within the value."));
		}

		internal static InvalidOperationException QuotePrefixNotSet(string method)
		{
			return ADP.InvalidOperation(Res.GetString("{0} requires open connection when the quote prefix has not been set.", new object[] { method }));
		}

		internal static string GetFullPath(string filename)
		{
			return Path.GetFullPath(filename);
		}

		internal static InvalidOperationException InvalidDataDirectory()
		{
			return ADP.InvalidOperation(SR.GetString("The DataDirectory substitute is not a string."));
		}

		internal static void EscapeSpecialCharacters(string unescapedString, StringBuilder escapedString)
		{
			foreach (char c in unescapedString)
			{
				if (".$^{[(|)*+?\\]".IndexOf(c) >= 0)
				{
					escapedString.Append("\\");
				}
				escapedString.Append(c);
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal static IntPtr IntPtrOffset(IntPtr pbase, int offset)
		{
			checked
			{
				if (4 == ADP.PtrSize)
				{
					return (IntPtr)(pbase.ToInt32() + offset);
				}
				return (IntPtr)(pbase.ToInt64() + unchecked((long)offset));
			}
		}

		internal static Exception InvalidXMLBadVersion()
		{
			return ADP.Argument(Res.GetString("Invalid Xml; can only parse elements of version one."));
		}

		internal static Exception NotAPermissionElement()
		{
			return ADP.Argument(Res.GetString("Given security element is not a permission element."));
		}

		internal static Exception PermissionTypeMismatch()
		{
			return ADP.Argument(Res.GetString("Type mismatch."));
		}

		internal static ArgumentOutOfRangeException InvalidPermissionState(PermissionState value)
		{
			return ADP.InvalidEnumerationValue(typeof(PermissionState), (int)value);
		}

		internal static ConfigurationException Configuration(string message)
		{
			ConfigurationErrorsException ex = new ConfigurationErrorsException(message);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ConfigurationException Configuration(string message, XmlNode node)
		{
			ConfigurationErrorsException ex = new ConfigurationErrorsException(message, node);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static ArgumentException ConfigProviderNotFound()
		{
			return ADP.Argument(Res.GetString("Unable to find the requested .Net Framework Data Provider.  It may not be installed."));
		}

		internal static InvalidOperationException ConfigProviderInvalid()
		{
			return ADP.InvalidOperation(Res.GetString("The requested .Net Framework Data Provider's implementation does not have an Instance field of a System.Data.Common.DbProviderFactory derived type."));
		}

		internal static ConfigurationException ConfigProviderNotInstalled()
		{
			return ADP.Configuration(Res.GetString("Failed to find or load the registered .Net Framework Data Provider."));
		}

		internal static ConfigurationException ConfigProviderMissing()
		{
			return ADP.Configuration(Res.GetString("The missing .Net Framework Data Provider's assembly qualified name is required."));
		}

		internal static ConfigurationException ConfigBaseNoChildNodes(XmlNode node)
		{
			return ADP.Configuration(Res.GetString("Child nodes not allowed."), node);
		}

		internal static ConfigurationException ConfigBaseElementsOnly(XmlNode node)
		{
			return ADP.Configuration(Res.GetString("Only elements allowed."), node);
		}

		internal static ConfigurationException ConfigUnrecognizedAttributes(XmlNode node)
		{
			return ADP.Configuration(Res.GetString("Unrecognized attribute '{0}'.", new object[] { node.Attributes[0].Name }), node);
		}

		internal static ConfigurationException ConfigUnrecognizedElement(XmlNode node)
		{
			return ADP.Configuration(Res.GetString("Unrecognized element."), node);
		}

		internal static ConfigurationException ConfigSectionsUnique(string sectionName)
		{
			return ADP.Configuration(Res.GetString("The '{0}' section can only appear once per config file.", new object[] { sectionName }));
		}

		internal static ConfigurationException ConfigRequiredAttributeMissing(string name, XmlNode node)
		{
			return ADP.Configuration(Res.GetString("Required attribute '{0}' not found.", new object[] { name }), node);
		}

		internal static ConfigurationException ConfigRequiredAttributeEmpty(string name, XmlNode node)
		{
			return ADP.Configuration(Res.GetString("Required attribute '{0}' cannot be empty.", new object[] { name }), node);
		}

		internal static Exception OleDb()
		{
			return new NotImplementedException("OleDb is not implemented.");
		}

		private static Task<bool> _trueTask;

		private static Task<bool> _falseTask;

		internal const CompareOptions DefaultCompareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;

		internal const int DefaultConnectionTimeout = 15;

		private static readonly Type s_stackOverflowType = typeof(StackOverflowException);

		private static readonly Type s_outOfMemoryType = typeof(OutOfMemoryException);

		private static readonly Type s_threadAbortType = typeof(ThreadAbortException);

		private static readonly Type s_nullReferenceType = typeof(NullReferenceException);

		private static readonly Type s_accessViolationType = typeof(AccessViolationException);

		private static readonly Type s_securityType = typeof(SecurityException);

		internal const string ConnectionString = "ConnectionString";

		internal const string DataSetColumn = "DataSetColumn";

		internal const string DataSetTable = "DataSetTable";

		internal const string Fill = "Fill";

		internal const string FillSchema = "FillSchema";

		internal const string SourceColumn = "SourceColumn";

		internal const string SourceTable = "SourceTable";

		internal const string Parameter = "Parameter";

		internal const string ParameterName = "ParameterName";

		internal const string ParameterSetPosition = "set_Position";

		internal const int DefaultCommandTimeout = 30;

		internal const float FailoverTimeoutStep = 0.08f;

		internal static readonly string StrEmpty = "";

		internal const int CharSize = 2;

		private static Version s_systemDataVersion;

		internal static readonly string[] AzureSqlServerEndpoints = new string[]
		{
			SR.GetString(".database.windows.net"),
			SR.GetString(".database.cloudapi.de"),
			SR.GetString(".database.usgovcloudapi.net"),
			SR.GetString(".database.chinacloudapi.cn")
		};

		internal const int DecimalMaxPrecision = 29;

		internal const int DecimalMaxPrecision28 = 28;

		internal static readonly IntPtr PtrZero = new IntPtr(0);

		internal static readonly int PtrSize = IntPtr.Size;

		internal const string BeginTransaction = "BeginTransaction";

		internal const string ChangeDatabase = "ChangeDatabase";

		internal const string CommitTransaction = "CommitTransaction";

		internal const string CommandTimeout = "CommandTimeout";

		internal const string DeriveParameters = "DeriveParameters";

		internal const string ExecuteReader = "ExecuteReader";

		internal const string ExecuteNonQuery = "ExecuteNonQuery";

		internal const string ExecuteScalar = "ExecuteScalar";

		internal const string GetSchema = "GetSchema";

		internal const string GetSchemaTable = "GetSchemaTable";

		internal const string Prepare = "Prepare";

		internal const string RollbackTransaction = "RollbackTransaction";

		internal const string QuoteIdentifier = "QuoteIdentifier";

		internal const string UnquoteIdentifier = "UnquoteIdentifier";

		internal enum InternalErrorCode
		{
			UnpooledObjectHasOwner,
			UnpooledObjectHasWrongOwner,
			PushingObjectSecondTime,
			PooledObjectHasOwner,
			PooledObjectInPoolMoreThanOnce,
			CreateObjectReturnedNull,
			NewObjectCannotBePooled,
			NonPooledObjectUsedMoreThanOnce,
			AttemptingToPoolOnRestrictedToken,
			ConvertSidToStringSidWReturnedNull = 10,
			AttemptingToConstructReferenceCollectionOnStaticObject = 12,
			AttemptingToEnlistTwice,
			CreateReferenceCollectionReturnedNull,
			PooledObjectWithoutPool,
			UnexpectedWaitAnyResult,
			SynchronousConnectReturnedPending,
			CompletedConnectReturnedPending,
			NameValuePairNext = 20,
			InvalidParserState1,
			InvalidParserState2,
			InvalidParserState3,
			InvalidBuffer = 30,
			UnimplementedSMIMethod = 40,
			InvalidSmiCall,
			SqlDependencyObtainProcessDispatcherFailureObjectHandle = 50,
			SqlDependencyProcessDispatcherFailureCreateInstance,
			SqlDependencyProcessDispatcherFailureAppDomain,
			SqlDependencyCommandHashIsNotAssociatedWithNotification,
			UnknownTransactionFailure = 60
		}

		internal enum ConnectionError
		{
			BeginGetConnectionReturnsNull,
			GetConnectionReturnsNull,
			ConnectionOptionsMissing,
			CouldNotSwitchToClosedPreviouslyOpenedState
		}
	}
}
