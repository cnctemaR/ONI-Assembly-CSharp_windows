using System;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace System.Data.SqlClient
{
	internal static class SQL
	{
		internal static Exception CannotGetDTCAddress()
		{
			return ADP.InvalidOperation(SR.GetString("Unable to get the address of the distributed transaction coordinator for the server, from the server.  Is DTC enabled on the server?"));
		}

		internal static Exception InvalidInternalPacketSize(string str)
		{
			return ADP.ArgumentOutOfRange(str);
		}

		internal static Exception InvalidPacketSize()
		{
			return ADP.ArgumentOutOfRange(SR.GetString("Invalid Packet Size."));
		}

		internal static Exception InvalidPacketSizeValue()
		{
			return ADP.Argument(SR.GetString("Invalid 'Packet Size'.  The value must be an integer >= 512 and <= 32768."));
		}

		internal static Exception InvalidSSPIPacketSize()
		{
			return ADP.Argument(SR.GetString("Invalid SSPI packet size."));
		}

		internal static Exception NullEmptyTransactionName()
		{
			return ADP.Argument(SR.GetString("Invalid transaction or invalid name for a point at which to save within the transaction."));
		}

		internal static Exception UserInstanceFailoverNotCompatible()
		{
			return ADP.Argument(SR.GetString("User Instance and Failover are not compatible options.  Please choose only one of the two in the connection string."));
		}

		internal static Exception InvalidSQLServerVersionUnknown()
		{
			return ADP.DataAdapter(SR.GetString("Unsupported SQL Server version.  The .Net Framework SqlClient Data Provider can only be used with SQL Server versions 7.0 and later."));
		}

		internal static Exception SynchronousCallMayNotPend()
		{
			return new Exception(SR.GetString("Internal Error"));
		}

		internal static Exception ConnectionLockedForBcpEvent()
		{
			return ADP.InvalidOperation(SR.GetString("The connection cannot be used because there is an ongoing operation that must be finished."));
		}

		internal static Exception InstanceFailure()
		{
			return ADP.InvalidOperation(SR.GetString("Instance failure."));
		}

		internal static Exception GlobalTransactionsNotEnabled()
		{
			return ADP.InvalidOperation(SR.GetString("Global Transactions are not enabled for this Azure SQL Database. Please contact Azure SQL Database support for assistance."));
		}

		internal static Exception UnknownSysTxIsolationLevel(IsolationLevel isolationLevel)
		{
			return ADP.InvalidOperation(SR.GetString("Unrecognized System.Transactions.IsolationLevel enumeration value: {0}.", new object[] { isolationLevel.ToString() }));
		}

		internal static Exception InvalidPartnerConfiguration(string server, string database)
		{
			return ADP.InvalidOperation(SR.GetString("Server {0}, database {1} is not configured for database mirroring.", new object[] { server, database }));
		}

		internal static Exception MARSUnspportedOnConnection()
		{
			return ADP.InvalidOperation(SR.GetString("The connection does not support MultipleActiveResultSets."));
		}

		internal static Exception CannotModifyPropertyAsyncOperationInProgress([CallerMemberName] string property = "")
		{
			return ADP.InvalidOperation(SR.GetString("{0} cannot be changed while async operation is in progress.", new object[] { property }));
		}

		internal static Exception NonLocalSSEInstance()
		{
			return ADP.NotSupported(SR.GetString("SSE Instance re-direction is not supported for non-local user instances."));
		}

		internal static ArgumentOutOfRangeException NotSupportedEnumerationValue(Type type, int value)
		{
			return ADP.ArgumentOutOfRange(SR.GetString("The {0} enumeration value, {1}, is not supported by the .Net Framework SqlClient Data Provider.", new object[]
			{
				type.Name,
				value.ToString(CultureInfo.InvariantCulture)
			}), type.Name);
		}

		internal static ArgumentOutOfRangeException NotSupportedCommandType(CommandType value)
		{
			return SQL.NotSupportedEnumerationValue(typeof(CommandType), (int)value);
		}

		internal static ArgumentOutOfRangeException NotSupportedIsolationLevel(IsolationLevel value)
		{
			return SQL.NotSupportedEnumerationValue(typeof(IsolationLevel), (int)value);
		}

		internal static Exception OperationCancelled()
		{
			return ADP.InvalidOperation(SR.GetString("Operation cancelled by user."));
		}

		internal static Exception PendingBeginXXXExists()
		{
			return ADP.InvalidOperation(SR.GetString("The command execution cannot proceed due to a pending asynchronous operation already in progress."));
		}

		internal static ArgumentOutOfRangeException InvalidSqlDependencyTimeout(string param)
		{
			return ADP.ArgumentOutOfRange(SR.GetString("Timeout specified is invalid. Timeout cannot be < 0."), param);
		}

		internal static Exception NonXmlResult()
		{
			return ADP.InvalidOperation(SR.GetString("Invalid command sent to ExecuteXmlReader.  The command must return an Xml result."));
		}

		internal static Exception InvalidParameterTypeNameFormat()
		{
			return ADP.Argument(SR.GetString("Invalid 3 part name format for TypeName."));
		}

		internal static Exception InvalidParameterNameLength(string value)
		{
			return ADP.Argument(SR.GetString("The length of the parameter '{0}' exceeds the limit of 128 characters.", new object[] { value }));
		}

		internal static Exception PrecisionValueOutOfRange(byte precision)
		{
			return ADP.Argument(SR.GetString("Precision value '{0}' is either less than 0 or greater than the maximum allowed precision of 38.", new object[] { precision.ToString(CultureInfo.InvariantCulture) }));
		}

		internal static Exception ScaleValueOutOfRange(byte scale)
		{
			return ADP.Argument(SR.GetString("Scale value '{0}' is either less than 0 or greater than the maximum allowed scale of 38.", new object[] { scale.ToString(CultureInfo.InvariantCulture) }));
		}

		internal static Exception TimeScaleValueOutOfRange(byte scale)
		{
			return ADP.Argument(SR.GetString("Scale value '{0}' is either less than 0 or greater than the maximum allowed scale of 7.", new object[] { scale.ToString(CultureInfo.InvariantCulture) }));
		}

		internal static Exception InvalidSqlDbType(SqlDbType value)
		{
			return ADP.InvalidEnumerationValue(typeof(SqlDbType), (int)value);
		}

		internal static Exception UnsupportedTVPOutputParameter(ParameterDirection direction, string paramName)
		{
			return ADP.NotSupported(SR.GetString("ParameterDirection '{0}' specified for parameter '{1}' is not supported. Table-valued parameters only support ParameterDirection.Input.", new object[]
			{
				direction.ToString(),
				paramName
			}));
		}

		internal static Exception DBNullNotSupportedForTVPValues(string paramName)
		{
			return ADP.NotSupported(SR.GetString("DBNull value for parameter '{0}' is not supported. Table-valued parameters cannot be DBNull.", new object[] { paramName }));
		}

		internal static Exception UnexpectedTypeNameForNonStructParams(string paramName)
		{
			return ADP.NotSupported(SR.GetString("TypeName specified for parameter '{0}'.  TypeName must only be set for Structured parameters.", new object[] { paramName }));
		}

		internal static Exception ParameterInvalidVariant(string paramName)
		{
			return ADP.InvalidOperation(SR.GetString("Parameter '{0}' exceeds the size limit for the sql_variant datatype.", new object[] { paramName }));
		}

		internal static Exception MustSetTypeNameForParam(string paramType, string paramName)
		{
			return ADP.Argument(SR.GetString("The {0} type parameter '{1}' must have a valid type name.", new object[] { paramType, paramName }));
		}

		internal static Exception NullSchemaTableDataTypeNotSupported(string columnName)
		{
			return ADP.Argument(SR.GetString("DateType column for field '{0}' in schema table is null.  DataType must be non-null.", new object[] { columnName }));
		}

		internal static Exception InvalidSchemaTableOrdinals()
		{
			return ADP.Argument(SR.GetString("Invalid column ordinals in schema table.  ColumnOrdinals, if present, must not have duplicates or gaps."));
		}

		internal static Exception EnumeratedRecordMetaDataChanged(string fieldName, int recordNumber)
		{
			return ADP.Argument(SR.GetString("Metadata for field '{0}' of record '{1}' did not match the original record's metadata.", new object[] { fieldName, recordNumber }));
		}

		internal static Exception EnumeratedRecordFieldCountChanged(int recordNumber)
		{
			return ADP.Argument(SR.GetString("Number of fields in record '{0}' does not match the number in the original record.", new object[] { recordNumber }));
		}

		internal static Exception InvalidTDSVersion()
		{
			return ADP.InvalidOperation(SR.GetString("The SQL Server instance returned an invalid or unsupported protocol version during login negotiation."));
		}

		internal static Exception ParsingError()
		{
			return ADP.InvalidOperation(SR.GetString("Internal connection fatal error."));
		}

		internal static Exception MoneyOverflow(string moneyValue)
		{
			return ADP.Overflow(SR.GetString("SqlDbType.SmallMoney overflow.  Value '{0}' is out of range.  Must be between -214,748.3648 and 214,748.3647.", new object[] { moneyValue }));
		}

		internal static Exception SmallDateTimeOverflow(string datetime)
		{
			return ADP.Overflow(SR.GetString("SqlDbType.SmallDateTime overflow.  Value '{0}' is out of range.  Must be between 1/1/1900 12:00:00 AM and 6/6/2079 11:59:59 PM.", new object[] { datetime }));
		}

		internal static Exception SNIPacketAllocationFailure()
		{
			return ADP.InvalidOperation(SR.GetString("Memory allocation for internal connection failed."));
		}

		internal static Exception TimeOverflow(string time)
		{
			return ADP.Overflow(SR.GetString("SqlDbType.Time overflow.  Value '{0}' is out of range.  Must be between 00:00:00.0000000 and 23:59:59.9999999.", new object[] { time }));
		}

		internal static Exception InvalidRead()
		{
			return ADP.InvalidOperation(SR.GetString("Invalid attempt to read when no data is present."));
		}

		internal static Exception NonBlobColumn(string columnName)
		{
			return ADP.InvalidCast(SR.GetString("Invalid attempt to GetBytes on column '{0}'.  The GetBytes function can only be used on columns of type Text, NText, or Image.", new object[] { columnName }));
		}

		internal static Exception NonCharColumn(string columnName)
		{
			return ADP.InvalidCast(SR.GetString("Invalid attempt to GetChars on column '{0}'.  The GetChars function can only be used on columns of type Text, NText, Xml, VarChar or NVarChar.", new object[] { columnName }));
		}

		internal static Exception StreamNotSupportOnColumnType(string columnName)
		{
			return ADP.InvalidCast(SR.GetString("Invalid attempt to GetStream on column '{0}'. The GetStream function can only be used on columns of type Binary, Image, Udt or VarBinary.", new object[] { columnName }));
		}

		internal static Exception TextReaderNotSupportOnColumnType(string columnName)
		{
			return ADP.InvalidCast(SR.GetString("Invalid attempt to GetTextReader on column '{0}'. The GetTextReader function can only be used on columns of type Char, NChar, NText, NVarChar, Text or VarChar.", new object[] { columnName }));
		}

		internal static Exception XmlReaderNotSupportOnColumnType(string columnName)
		{
			return ADP.InvalidCast(SR.GetString("Invalid attempt to GetXmlReader on column '{0}'. The GetXmlReader function can only be used on columns of type Xml.", new object[] { columnName }));
		}

		internal static Exception SqlCommandHasExistingSqlNotificationRequest()
		{
			return ADP.InvalidOperation(SR.GetString("This SqlCommand object is already associated with another SqlDependency object."));
		}

		internal static Exception SqlDepDefaultOptionsButNoStart()
		{
			return ADP.InvalidOperation(SR.GetString("When using SqlDependency without providing an options value, SqlDependency.Start() must be called prior to execution of a command added to the SqlDependency instance."));
		}

		internal static Exception SqlDependencyDatabaseBrokerDisabled()
		{
			return ADP.InvalidOperation(SR.GetString("The SQL Server Service Broker for the current database is not enabled, and as a result query notifications are not supported.  Please enable the Service Broker for this database if you wish to use notifications."));
		}

		internal static Exception SqlDependencyEventNoDuplicate()
		{
			return ADP.InvalidOperation(SR.GetString("SqlDependency.OnChange does not support multiple event registrations for the same delegate."));
		}

		internal static Exception SqlDependencyDuplicateStart()
		{
			return ADP.InvalidOperation(SR.GetString("SqlDependency does not support calling Start() with different connection strings having the same server, user, and database in the same app domain."));
		}

		internal static Exception SqlDependencyIdMismatch()
		{
			return ADP.InvalidOperation(SR.GetString("No SqlDependency exists for the key."));
		}

		internal static Exception SqlDependencyNoMatchingServerStart()
		{
			return ADP.InvalidOperation(SR.GetString("When using SqlDependency without providing an options value, SqlDependency.Start() must be called for each server that is being executed against."));
		}

		internal static Exception SqlDependencyNoMatchingServerDatabaseStart()
		{
			return ADP.InvalidOperation(SR.GetString("SqlDependency.Start has been called for the server the command is executing against more than once, but there is no matching server/user/database Start() call for current command."));
		}

		internal static TransactionPromotionException PromotionFailed(Exception inner)
		{
			TransactionPromotionException ex = new TransactionPromotionException(SR.GetString("Failure while attempting to promote transaction."), inner);
			ADP.TraceExceptionAsReturnValue(ex);
			return ex;
		}

		internal static Exception InvalidSqlDbTypeForConstructor(SqlDbType type)
		{
			return ADP.Argument(SR.GetString("The dbType {0} is invalid for this constructor.", new object[] { type.ToString() }));
		}

		internal static Exception NameTooLong(string parameterName)
		{
			return ADP.Argument(SR.GetString("The name is too long."), parameterName);
		}

		internal static Exception InvalidSortOrder(SortOrder order)
		{
			return ADP.InvalidEnumerationValue(typeof(SortOrder), (int)order);
		}

		internal static Exception MustSpecifyBothSortOrderAndOrdinal(SortOrder order, int ordinal)
		{
			return ADP.InvalidOperation(SR.GetString("The sort order and ordinal must either both be specified, or neither should be specified (SortOrder.Unspecified and -1).  The values given were: order = {0}, ordinal = {1}.", new object[]
			{
				order.ToString(),
				ordinal
			}));
		}

		internal static Exception UnsupportedColumnTypeForSqlProvider(string columnName, string typeName)
		{
			return ADP.Argument(SR.GetString("The type of column '{0}' is not supported.  The type is '{1}'", new object[] { columnName, typeName }));
		}

		internal static Exception InvalidColumnMaxLength(string columnName, long maxLength)
		{
			return ADP.Argument(SR.GetString("The size of column '{0}' is not supported. The size is {1}.", new object[] { columnName, maxLength }));
		}

		internal static Exception InvalidColumnPrecScale()
		{
			return ADP.Argument(SR.GetString("Invalid numeric precision/scale."));
		}

		internal static Exception NotEnoughColumnsInStructuredType()
		{
			return ADP.Argument(SR.GetString("There are not enough fields in the Structured type.  Structured types must have at least one field."));
		}

		internal static Exception DuplicateSortOrdinal(int sortOrdinal)
		{
			return ADP.InvalidOperation(SR.GetString("The sort ordinal {0} was specified twice.", new object[] { sortOrdinal }));
		}

		internal static Exception MissingSortOrdinal(int sortOrdinal)
		{
			return ADP.InvalidOperation(SR.GetString("The sort ordinal {0} was not specified.", new object[] { sortOrdinal }));
		}

		internal static Exception SortOrdinalGreaterThanFieldCount(int columnOrdinal, int sortOrdinal)
		{
			return ADP.InvalidOperation(SR.GetString("The sort ordinal {0} on field {1} exceeds the total number of fields.", new object[] { sortOrdinal, columnOrdinal }));
		}

		internal static Exception IEnumerableOfSqlDataRecordHasNoRows()
		{
			return ADP.Argument(SR.GetString("There are no records in the SqlDataRecord enumeration. To send a table-valued parameter with no rows, use a null reference for the value instead."));
		}

		internal static Exception BulkLoadMappingInaccessible()
		{
			return ADP.InvalidOperation(SR.GetString("The mapped collection is in use and cannot be accessed at this time;"));
		}

		internal static Exception BulkLoadMappingsNamesOrOrdinalsOnly()
		{
			return ADP.InvalidOperation(SR.GetString("Mappings must be either all name or all ordinal based."));
		}

		internal static Exception BulkLoadCannotConvertValue(Type sourcetype, MetaType metatype, Exception e)
		{
			return ADP.InvalidOperation(SR.GetString("The given value of type {0} from the data source cannot be converted to type {1} of the specified target column.", new object[] { sourcetype.Name, metatype.TypeName }), e);
		}

		internal static Exception BulkLoadNonMatchingColumnMapping()
		{
			return ADP.InvalidOperation(SR.GetString("The given ColumnMapping does not match up with any column in the source or destination."));
		}

		internal static Exception BulkLoadNonMatchingColumnName(string columnName)
		{
			return SQL.BulkLoadNonMatchingColumnName(columnName, null);
		}

		internal static Exception BulkLoadNonMatchingColumnName(string columnName, Exception e)
		{
			return ADP.InvalidOperation(SR.GetString("The given ColumnName '{0}' does not match up with any column in data source.", new object[] { columnName }), e);
		}

		internal static Exception BulkLoadStringTooLong()
		{
			return ADP.InvalidOperation(SR.GetString("String or binary data would be truncated."));
		}

		internal static Exception BulkLoadInvalidVariantValue()
		{
			return ADP.InvalidOperation(SR.GetString("Value cannot be converted to SqlVariant."));
		}

		internal static Exception BulkLoadInvalidTimeout(int timeout)
		{
			return ADP.Argument(SR.GetString("Timeout Value '{0}' is less than 0.", new object[] { timeout.ToString(CultureInfo.InvariantCulture) }));
		}

		internal static Exception BulkLoadExistingTransaction()
		{
			return ADP.InvalidOperation(SR.GetString("Unexpected existing transaction."));
		}

		internal static Exception BulkLoadNoCollation()
		{
			return ADP.InvalidOperation(SR.GetString("Failed to obtain column collation information for the destination table. If the table is not in the current database the name must be qualified using the database name (e.g. [mydb]..[mytable](e.g. [mydb]..[mytable]); this also applies to temporary-tables (e.g. #mytable would be specified as tempdb..#mytable)."));
		}

		internal static Exception BulkLoadConflictingTransactionOption()
		{
			return ADP.Argument(SR.GetString("Must not specify SqlBulkCopyOption.UseInternalTransaction and pass an external Transaction at the same time."));
		}

		internal static Exception BulkLoadLcidMismatch(int sourceLcid, string sourceColumnName, int destinationLcid, string destinationColumnName)
		{
			return ADP.InvalidOperation(SR.GetString("The locale id '{0}' of the source column '{1}' and the locale id '{2}' of the destination column '{3}' do not match.", new object[] { sourceLcid, sourceColumnName, destinationLcid, destinationColumnName }));
		}

		internal static Exception InvalidOperationInsideEvent()
		{
			return ADP.InvalidOperation(SR.GetString("Function must not be called during event."));
		}

		internal static Exception BulkLoadMissingDestinationTable()
		{
			return ADP.InvalidOperation(SR.GetString("The DestinationTableName property must be set before calling this method."));
		}

		internal static Exception BulkLoadInvalidDestinationTable(string tableName, Exception inner)
		{
			return ADP.InvalidOperation(SR.GetString("Cannot access destination table '{0}'.", new object[] { tableName }), inner);
		}

		internal static Exception BulkLoadBulkLoadNotAllowDBNull(string columnName)
		{
			return ADP.InvalidOperation(SR.GetString("Column '{0}' does not allow DBNull.Value.", new object[] { columnName }));
		}

		internal static Exception BulkLoadPendingOperation()
		{
			return ADP.InvalidOperation(SR.GetString("Attempt to invoke bulk copy on an object that has a pending operation."));
		}

		internal static Exception InvalidTableDerivedPrecisionForTvp(string columnName, byte precision)
		{
			return ADP.InvalidOperation(SR.GetString("Precision '{0}' required to send all values in column '{1}' exceeds the maximum supported precision '{2}'. The values must all fit in a single precision.", new object[]
			{
				precision,
				columnName,
				SqlDecimal.MaxPrecision
			}));
		}

		internal static Exception ConnectionDoomed()
		{
			return ADP.InvalidOperation(SR.GetString("The requested operation cannot be completed because the connection has been broken."));
		}

		internal static Exception OpenResultCountExceeded()
		{
			return ADP.InvalidOperation(SR.GetString("Open result count exceeded."));
		}

		internal static Exception UnsupportedSysTxForGlobalTransactions()
		{
			return ADP.InvalidOperation(SR.GetString("The currently loaded System.Transactions.dll does not support Global Transactions."));
		}

		internal static Exception MultiSubnetFailoverWithFailoverPartner(bool serverProvidedFailoverPartner, SqlInternalConnectionTds internalConnection)
		{
			string @string = SR.GetString("Connecting to a mirrored SQL Server instance using the MultiSubnetFailover connection option is not supported.");
			if (serverProvidedFailoverPartner)
			{
				SqlException ex = SqlException.CreateException(new SqlErrorCollection
				{
					new SqlError(0, 0, 20, null, @string, "", 0, null)
				}, null, internalConnection, null);
				ex._doNotReconnect = true;
				return ex;
			}
			return ADP.Argument(@string);
		}

		internal static Exception MultiSubnetFailoverWithMoreThan64IPs()
		{
			return ADP.InvalidOperation(SQL.GetSNIErrorMessage(47));
		}

		internal static Exception MultiSubnetFailoverWithInstanceSpecified()
		{
			return ADP.Argument(SQL.GetSNIErrorMessage(48));
		}

		internal static Exception MultiSubnetFailoverWithNonTcpProtocol()
		{
			return ADP.Argument(SQL.GetSNIErrorMessage(49));
		}

		internal static Exception ROR_FailoverNotSupportedConnString()
		{
			return ADP.Argument(SR.GetString("Connecting to a mirrored SQL Server instance using the ApplicationIntent ReadOnly connection option is not supported."));
		}

		internal static Exception ROR_FailoverNotSupportedServer(SqlInternalConnectionTds internalConnection)
		{
			SqlException ex = SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 20, null, SR.GetString("Connecting to a mirrored SQL Server instance using the ApplicationIntent ReadOnly connection option is not supported."), "", 0, null)
			}, null, internalConnection, null);
			ex._doNotReconnect = true;
			return ex;
		}

		internal static Exception ROR_RecursiveRoutingNotSupported(SqlInternalConnectionTds internalConnection)
		{
			SqlException ex = SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 20, null, SR.GetString("Two or more redirections have occurred. Only one redirection per login is allowed."), "", 0, null)
			}, null, internalConnection, null);
			ex._doNotReconnect = true;
			return ex;
		}

		internal static Exception ROR_UnexpectedRoutingInfo(SqlInternalConnectionTds internalConnection)
		{
			SqlException ex = SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 20, null, SR.GetString("Unexpected routing information received."), "", 0, null)
			}, null, internalConnection, null);
			ex._doNotReconnect = true;
			return ex;
		}

		internal static Exception ROR_InvalidRoutingInfo(SqlInternalConnectionTds internalConnection)
		{
			SqlException ex = SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 20, null, SR.GetString("Invalid routing information received."), "", 0, null)
			}, null, internalConnection, null);
			ex._doNotReconnect = true;
			return ex;
		}

		internal static Exception ROR_TimeoutAfterRoutingInfo(SqlInternalConnectionTds internalConnection)
		{
			SqlException ex = SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 20, null, SR.GetString("Server provided routing information, but timeout already expired."), "", 0, null)
			}, null, internalConnection, null);
			ex._doNotReconnect = true;
			return ex;
		}

		internal static SqlException CR_ReconnectTimeout()
		{
			return SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(-2, 0, 11, null, SQLMessage.Timeout(), "", 0, 258U, null)
			}, "");
		}

		internal static SqlException CR_ReconnectionCancelled()
		{
			return SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 11, null, SQLMessage.OperationCancelled(), "", 0, null)
			}, "");
		}

		internal static Exception CR_NextAttemptWillExceedQueryTimeout(SqlException innerException, Guid connectionId)
		{
			return SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 11, null, SR.GetString("Next reconnection attempt will exceed query timeout. Reconnection was terminated."), "", 0, null)
			}, "", connectionId, innerException);
		}

		internal static Exception CR_EncryptionChanged(SqlInternalConnectionTds internalConnection)
		{
			return SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 20, null, SR.GetString("The server did not preserve SSL encryption during a recovery attempt, connection recovery is not possible."), "", 0, null)
			}, "", internalConnection, null);
		}

		internal static SqlException CR_AllAttemptsFailed(SqlException innerException, Guid connectionId)
		{
			return SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 11, null, SR.GetString("The connection is broken and recovery is not possible.  The client driver attempted to recover the connection one or more times and all attempts failed.  Increase the value of ConnectRetryCount to increase the number of recovery attempts."), "", 0, null)
			}, "", connectionId, innerException);
		}

		internal static SqlException CR_NoCRAckAtReconnection(SqlInternalConnectionTds internalConnection)
		{
			return SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 20, null, SR.GetString("The server did not acknowledge a recovery attempt, connection recovery is not possible."), "", 0, null)
			}, "", internalConnection, null);
		}

		internal static SqlException CR_TDSVersionNotPreserved(SqlInternalConnectionTds internalConnection)
		{
			return SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 20, null, SR.GetString("The server did not preserve the exact client TDS version requested during a recovery attempt, connection recovery is not possible."), "", 0, null)
			}, "", internalConnection, null);
		}

		internal static SqlException CR_UnrecoverableServer(Guid connectionId)
		{
			return SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 20, null, SR.GetString("The connection is broken and recovery is not possible.  The connection is marked by the server as unrecoverable.  No attempt was made to restore the connection."), "", 0, null)
			}, "", connectionId, null);
		}

		internal static SqlException CR_UnrecoverableClient(Guid connectionId)
		{
			return SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 20, null, SR.GetString("The connection is broken and recovery is not possible.  The connection is marked by the client driver as unrecoverable.  No attempt was made to restore the connection."), "", 0, null)
			}, "", connectionId, null);
		}

		internal static Exception StreamWriteNotSupported()
		{
			return ADP.NotSupported(SR.GetString("The Stream does not support writing."));
		}

		internal static Exception StreamReadNotSupported()
		{
			return ADP.NotSupported(SR.GetString("The Stream does not support reading."));
		}

		internal static Exception StreamSeekNotSupported()
		{
			return ADP.NotSupported(SR.GetString("The Stream does not support seeking."));
		}

		internal static SqlNullValueException SqlNullValue()
		{
			return new SqlNullValueException();
		}

		internal static Exception SubclassMustOverride()
		{
			return ADP.InvalidOperation(SR.GetString("Subclass did not override a required method."));
		}

		internal static Exception UnsupportedKeyword(string keyword)
		{
			return ADP.NotSupported(SR.GetString("The keyword '{0}' is not supported on this platform.", new object[] { keyword }));
		}

		internal static Exception NetworkLibraryKeywordNotSupported()
		{
			return ADP.NotSupported(SR.GetString("The keyword 'Network Library' is not supported on this platform, prefix the 'Data Source' with the protocol desired instead ('tcp:' for a TCP connection, or 'np:' for a Named Pipe connection)."));
		}

		internal static Exception UnsupportedFeatureAndToken(SqlInternalConnectionTds internalConnection, string token)
		{
			NotSupportedException ex = ADP.NotSupported(SR.GetString("Received an unsupported token '{0}' while reading data from the server.", new object[] { token }));
			return SqlException.CreateException(new SqlErrorCollection
			{
				new SqlError(0, 0, 20, null, SR.GetString("The server is attempting to use a feature that is not supported on this platform."), "", 0, null)
			}, "", internalConnection, ex);
		}

		internal static Exception BatchedUpdatesNotAvailableOnContextConnection()
		{
			return ADP.InvalidOperation(SR.GetString("Batching updates is not supported on the context connection."));
		}

		internal static string GetSNIErrorMessage(int sniError)
		{
			string text = string.Format(null, "SNI_ERROR_{0}", sniError);
			return SR.GetResourceString(text, text);
		}

		internal static readonly byte[] AttentionHeader = new byte[] { 6, 1, 0, 8, 0, 0, 0, 0 };

		internal const int SqlDependencyTimeoutDefault = 0;

		internal const int SqlDependencyServerTimeout = 432000;

		internal const string SqlNotificationServiceDefault = "SqlQueryNotificationService";

		internal const string SqlNotificationStoredProcedureDefault = "SqlQueryNotificationStoredProcedure";
	}
}
