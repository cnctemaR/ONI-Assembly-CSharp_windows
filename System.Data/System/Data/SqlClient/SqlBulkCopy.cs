using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace System.Data.SqlClient
{
	public sealed class SqlBulkCopy : IDisposable
	{
		public SqlBulkCopy(SqlConnection connection)
		{
			if (connection == null)
			{
				throw ADP.ArgumentNull("connection");
			}
			this._connection = connection;
			this._columnMappings = new SqlBulkCopyColumnMappingCollection();
		}

		public SqlBulkCopy(SqlConnection connection, SqlBulkCopyOptions copyOptions, SqlTransaction externalTransaction)
			: this(connection)
		{
			this._copyOptions = copyOptions;
			if (externalTransaction != null && this.IsCopyOption(SqlBulkCopyOptions.UseInternalTransaction))
			{
				throw SQL.BulkLoadConflictingTransactionOption();
			}
			if (!this.IsCopyOption(SqlBulkCopyOptions.UseInternalTransaction))
			{
				this._externalTransaction = externalTransaction;
			}
		}

		public SqlBulkCopy(string connectionString)
			: this(new SqlConnection(connectionString))
		{
			if (connectionString == null)
			{
				throw ADP.ArgumentNull("connectionString");
			}
			this._connection = new SqlConnection(connectionString);
			this._columnMappings = new SqlBulkCopyColumnMappingCollection();
			this._ownConnection = true;
		}

		public SqlBulkCopy(string connectionString, SqlBulkCopyOptions copyOptions)
			: this(connectionString)
		{
			this._copyOptions = copyOptions;
		}

		public int BatchSize
		{
			get
			{
				return this._batchSize;
			}
			set
			{
				if (value >= 0)
				{
					this._batchSize = value;
					return;
				}
				throw ADP.ArgumentOutOfRange("BatchSize");
			}
		}

		public int BulkCopyTimeout
		{
			get
			{
				return this._timeout;
			}
			set
			{
				if (value < 0)
				{
					throw SQL.BulkLoadInvalidTimeout(value);
				}
				this._timeout = value;
			}
		}

		public bool EnableStreaming
		{
			get
			{
				return this._enableStreaming;
			}
			set
			{
				this._enableStreaming = value;
			}
		}

		public SqlBulkCopyColumnMappingCollection ColumnMappings
		{
			get
			{
				return this._columnMappings;
			}
		}

		public string DestinationTableName
		{
			get
			{
				return this._destinationTableName;
			}
			set
			{
				if (value == null)
				{
					throw ADP.ArgumentNull("DestinationTableName");
				}
				if (value.Length == 0)
				{
					throw ADP.ArgumentOutOfRange("DestinationTableName");
				}
				this._destinationTableName = value;
			}
		}

		public int NotifyAfter
		{
			get
			{
				return this._notifyAfter;
			}
			set
			{
				if (value >= 0)
				{
					this._notifyAfter = value;
					return;
				}
				throw ADP.ArgumentOutOfRange("NotifyAfter");
			}
		}

		public event SqlRowsCopiedEventHandler SqlRowsCopied
		{
			add
			{
				this._rowsCopiedEventHandler = (SqlRowsCopiedEventHandler)Delegate.Combine(this._rowsCopiedEventHandler, value);
			}
			remove
			{
				this._rowsCopiedEventHandler = (SqlRowsCopiedEventHandler)Delegate.Remove(this._rowsCopiedEventHandler, value);
			}
		}

		internal SqlStatistics Statistics
		{
			get
			{
				if (this._connection != null && this._connection.StatisticsEnabled)
				{
					return this._connection.Statistics;
				}
				return null;
			}
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool IsCopyOption(SqlBulkCopyOptions copyOption)
		{
			return (this._copyOptions & copyOption) == copyOption;
		}

		private string CreateInitialQuery()
		{
			string[] array;
			try
			{
				array = MultipartIdentifier.ParseMultipartIdentifier(this.DestinationTableName, "[\"", "]\"", "SqlBulkCopy.WriteToServer failed because the SqlBulkCopy.DestinationTableName is an invalid multipart name", true);
			}
			catch (Exception ex)
			{
				throw SQL.BulkLoadInvalidDestinationTable(this.DestinationTableName, ex);
			}
			if (string.IsNullOrEmpty(array[3]))
			{
				throw SQL.BulkLoadInvalidDestinationTable(this.DestinationTableName, null);
			}
			string text = "select @@trancount; SET FMTONLY ON select * from " + this.DestinationTableName + " SET FMTONLY OFF ";
			string text2;
			if (this._connection.IsKatmaiOrNewer)
			{
				text2 = "sp_tablecollations_100";
			}
			else
			{
				text2 = "sp_tablecollations_90";
			}
			string text3 = array[3];
			bool flag = text3.Length > 0 && '#' == text3[0];
			if (!string.IsNullOrEmpty(text3))
			{
				text3 = SqlServerEscapeHelper.EscapeStringAsLiteral(text3);
				text3 = SqlServerEscapeHelper.EscapeIdentifier(text3);
			}
			string text4 = array[2];
			if (!string.IsNullOrEmpty(text4))
			{
				text4 = SqlServerEscapeHelper.EscapeStringAsLiteral(text4);
				text4 = SqlServerEscapeHelper.EscapeIdentifier(text4);
			}
			string text5 = array[1];
			if (flag && string.IsNullOrEmpty(text5))
			{
				text += string.Format(null, "exec tempdb..{0} N'{1}.{2}'", text2, text4, text3);
			}
			else
			{
				if (!string.IsNullOrEmpty(text5))
				{
					text5 = SqlServerEscapeHelper.EscapeIdentifier(text5);
				}
				text += string.Format(null, "exec {0}..{1} N'{2}.{3}'", new object[] { text5, text2, text4, text3 });
			}
			return text;
		}

		private Task<BulkCopySimpleResultSet> CreateAndExecuteInitialQueryAsync(out BulkCopySimpleResultSet result)
		{
			string text = this.CreateInitialQuery();
			Task task = this._parser.TdsExecuteSQLBatch(text, this.BulkCopyTimeout, null, this._stateObj, !this._isAsyncBulkCopy, true);
			if (task == null)
			{
				result = new BulkCopySimpleResultSet();
				this.RunParser(result);
				return null;
			}
			result = null;
			return task.ContinueWith<BulkCopySimpleResultSet>(delegate(Task t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.InnerException;
				}
				BulkCopySimpleResultSet bulkCopySimpleResultSet = new BulkCopySimpleResultSet();
				this.RunParserReliably(bulkCopySimpleResultSet);
				return bulkCopySimpleResultSet;
			}, TaskScheduler.Default);
		}

		private string AnalyzeTargetAndCreateUpdateBulkCommand(BulkCopySimpleResultSet internalResults)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (internalResults[2].Count == 0)
			{
				throw SQL.BulkLoadNoCollation();
			}
			stringBuilder.AppendFormat("insert bulk {0} (", this.DestinationTableName);
			int num = 0;
			int num2 = 0;
			if (this._connection.HasLocalTransaction && this._externalTransaction == null && this._internalTransaction == null && this._connection.Parser != null && this._connection.Parser.CurrentTransaction != null && this._connection.Parser.CurrentTransaction.IsLocal)
			{
				throw SQL.BulkLoadExistingTransaction();
			}
			_SqlMetaDataSet metaData = internalResults[1].MetaData;
			this._sortedColumnMappings = new List<_ColumnMapping>(metaData.Length);
			for (int i = 0; i < metaData.Length; i++)
			{
				_SqlMetaData sqlMetaData = metaData[i];
				bool flag = false;
				if (sqlMetaData.type == SqlDbType.Timestamp || (sqlMetaData.isIdentity && !this.IsCopyOption(SqlBulkCopyOptions.KeepIdentity)))
				{
					metaData[i] = null;
					flag = true;
				}
				int j = 0;
				while (j < this._localColumnMappings.Count)
				{
					if (this._localColumnMappings[j]._destinationColumnOrdinal == sqlMetaData.ordinal || this.UnquotedName(this._localColumnMappings[j]._destinationColumnName) == sqlMetaData.column)
					{
						if (flag)
						{
							num2++;
							break;
						}
						this._sortedColumnMappings.Add(new _ColumnMapping(this._localColumnMappings[j]._internalSourceColumnOrdinal, sqlMetaData));
						num++;
						if (num > 1)
						{
							stringBuilder.Append(", ");
						}
						if (sqlMetaData.type == SqlDbType.Variant)
						{
							this.AppendColumnNameAndTypeName(stringBuilder, sqlMetaData.column, "sql_variant");
						}
						else
						{
							if (sqlMetaData.type == SqlDbType.Udt)
							{
								throw ADP.DbTypeNotSupported(SqlDbType.Udt.ToString());
							}
							this.AppendColumnNameAndTypeName(stringBuilder, sqlMetaData.column, sqlMetaData.type.ToString());
						}
						byte b = sqlMetaData.metaType.NullableType;
						if (b <= 106)
						{
							if (b - 41 > 2)
							{
								if (b != 106)
								{
									goto IL_029B;
								}
								goto IL_0217;
							}
							else
							{
								stringBuilder.AppendFormat(null, "({0})", sqlMetaData.scale);
							}
						}
						else
						{
							if (b == 108)
							{
								goto IL_0217;
							}
							if (b != 240)
							{
								goto IL_029B;
							}
							if (sqlMetaData.IsLargeUdt)
							{
								stringBuilder.Append("(max)");
							}
							else
							{
								int length = sqlMetaData.length;
								stringBuilder.AppendFormat(null, "({0})", length);
							}
						}
						IL_032C:
						object obj = internalResults[2][i][3];
						SqlDbType type = sqlMetaData.type;
						if (type <= SqlDbType.NVarChar)
						{
							if (type != SqlDbType.Char && type - SqlDbType.NChar > 2)
							{
								goto IL_0371;
							}
							goto IL_036C;
						}
						else
						{
							if (type == SqlDbType.Text || type == SqlDbType.VarChar)
							{
								goto IL_036C;
							}
							goto IL_0371;
						}
						IL_0374:
						bool flag2;
						if (obj == null || !flag2)
						{
							break;
						}
						SqlString sqlString = (SqlString)obj;
						if (sqlString.IsNull)
						{
							break;
						}
						stringBuilder.Append(" COLLATE " + sqlString.Value);
						if (this._SqlDataReaderRowSource == null || sqlMetaData.collation == null)
						{
							break;
						}
						int internalSourceColumnOrdinal = this._localColumnMappings[j]._internalSourceColumnOrdinal;
						int lcid = sqlMetaData.collation.LCID;
						int localeId = this._SqlDataReaderRowSource.GetLocaleId(internalSourceColumnOrdinal);
						if (localeId != lcid)
						{
							throw SQL.BulkLoadLcidMismatch(localeId, this._SqlDataReaderRowSource.GetName(internalSourceColumnOrdinal), lcid, sqlMetaData.column);
						}
						break;
						IL_0371:
						flag2 = false;
						goto IL_0374;
						IL_036C:
						flag2 = true;
						goto IL_0374;
						IL_0217:
						stringBuilder.AppendFormat(null, "({0},{1})", sqlMetaData.precision, sqlMetaData.scale);
						goto IL_032C;
						IL_029B:
						if (!sqlMetaData.metaType.IsFixed && !sqlMetaData.metaType.IsLong)
						{
							int num3 = sqlMetaData.length;
							b = sqlMetaData.metaType.NullableType;
							if (b == 99 || b == 231 || b == 239)
							{
								num3 /= 2;
							}
							stringBuilder.AppendFormat(null, "({0})", num3);
							goto IL_032C;
						}
						if (sqlMetaData.metaType.IsPlp && sqlMetaData.metaType.SqlDbType != SqlDbType.Xml)
						{
							stringBuilder.Append("(max)");
							goto IL_032C;
						}
						goto IL_032C;
					}
					else
					{
						j++;
					}
				}
				if (j == this._localColumnMappings.Count)
				{
					metaData[i] = null;
				}
			}
			if (num + num2 != this._localColumnMappings.Count)
			{
				throw SQL.BulkLoadNonMatchingColumnMapping();
			}
			stringBuilder.Append(")");
			if ((this._copyOptions & (SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.FireTriggers)) != SqlBulkCopyOptions.Default)
			{
				bool flag3 = false;
				stringBuilder.Append(" with (");
				if (this.IsCopyOption(SqlBulkCopyOptions.KeepNulls))
				{
					stringBuilder.Append("KEEP_NULLS");
					flag3 = true;
				}
				if (this.IsCopyOption(SqlBulkCopyOptions.TableLock))
				{
					stringBuilder.Append((flag3 ? ", " : "") + "TABLOCK");
					flag3 = true;
				}
				if (this.IsCopyOption(SqlBulkCopyOptions.CheckConstraints))
				{
					stringBuilder.Append((flag3 ? ", " : "") + "CHECK_CONSTRAINTS");
					flag3 = true;
				}
				if (this.IsCopyOption(SqlBulkCopyOptions.FireTriggers))
				{
					stringBuilder.Append((flag3 ? ", " : "") + "FIRE_TRIGGERS");
				}
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}

		private Task SubmitUpdateBulkCommand(string TDSCommand)
		{
			Task task = this._parser.TdsExecuteSQLBatch(TDSCommand, this.BulkCopyTimeout, null, this._stateObj, !this._isAsyncBulkCopy, true);
			if (task == null)
			{
				this.RunParser(null);
				return null;
			}
			return task.ContinueWith(delegate(Task t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.InnerException;
				}
				this.RunParserReliably(null);
			}, TaskScheduler.Default);
		}

		private void WriteMetaData(BulkCopySimpleResultSet internalResults)
		{
			this._stateObj.SetTimeoutSeconds(this.BulkCopyTimeout);
			_SqlMetaDataSet metaData = internalResults[1].MetaData;
			this._stateObj._outputMessageType = 7;
			this._parser.WriteBulkCopyMetaData(metaData, this._sortedColumnMappings.Count, this._stateObj);
		}

		public void Close()
		{
			if (this._insideRowsCopiedEvent)
			{
				throw SQL.InvalidOperationInsideEvent();
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._columnMappings = null;
				this._parser = null;
				try
				{
					if (this._internalTransaction != null)
					{
						this._internalTransaction.Rollback();
						this._internalTransaction.Dispose();
						this._internalTransaction = null;
					}
				}
				catch (Exception ex)
				{
					if (!ADP.IsCatchableExceptionType(ex))
					{
						throw;
					}
				}
				finally
				{
					if (this._connection != null)
					{
						if (this._ownConnection)
						{
							this._connection.Dispose();
						}
						this._connection = null;
					}
				}
			}
		}

		private object GetValueFromSourceRow(int destRowIndex, out bool isSqlType, out bool isDataFeed, out bool isNull)
		{
			_SqlMetaData metadata = this._sortedColumnMappings[destRowIndex]._metadata;
			int sourceColumnOrdinal = this._sortedColumnMappings[destRowIndex]._sourceColumnOrdinal;
			switch (this._rowSourceType)
			{
			case SqlBulkCopy.ValueSourceType.IDataReader:
			case SqlBulkCopy.ValueSourceType.DbDataReader:
				if (this._currentRowMetadata[destRowIndex].IsDataFeed)
				{
					if (this._DbDataReaderRowSource.IsDBNull(sourceColumnOrdinal))
					{
						isSqlType = false;
						isDataFeed = false;
						isNull = true;
						return DBNull.Value;
					}
					isSqlType = false;
					isDataFeed = true;
					isNull = false;
					switch (this._currentRowMetadata[destRowIndex].Method)
					{
					case SqlBulkCopy.ValueMethod.DataFeedStream:
						return new StreamDataFeed(this._DbDataReaderRowSource.GetStream(sourceColumnOrdinal));
					case SqlBulkCopy.ValueMethod.DataFeedText:
						return new TextDataFeed(this._DbDataReaderRowSource.GetTextReader(sourceColumnOrdinal));
					case SqlBulkCopy.ValueMethod.DataFeedXml:
						return new XmlDataFeed(this._SqlDataReaderRowSource.GetXmlReader(sourceColumnOrdinal));
					default:
					{
						isDataFeed = false;
						object value = this._DbDataReaderRowSource.GetValue(sourceColumnOrdinal);
						ADP.IsNullOrSqlType(value, out isNull, out isSqlType);
						return value;
					}
					}
				}
				else if (this._SqlDataReaderRowSource != null)
				{
					if (this._currentRowMetadata[destRowIndex].IsSqlType)
					{
						isSqlType = true;
						isDataFeed = false;
						INullable nullable;
						switch (this._currentRowMetadata[destRowIndex].Method)
						{
						case SqlBulkCopy.ValueMethod.SqlTypeSqlDecimal:
							nullable = this._SqlDataReaderRowSource.GetSqlDecimal(sourceColumnOrdinal);
							break;
						case SqlBulkCopy.ValueMethod.SqlTypeSqlDouble:
							nullable = new SqlDecimal(this._SqlDataReaderRowSource.GetSqlDouble(sourceColumnOrdinal).Value);
							break;
						case SqlBulkCopy.ValueMethod.SqlTypeSqlSingle:
							nullable = new SqlDecimal((double)this._SqlDataReaderRowSource.GetSqlSingle(sourceColumnOrdinal).Value);
							break;
						default:
							nullable = (INullable)this._SqlDataReaderRowSource.GetSqlValue(sourceColumnOrdinal);
							break;
						}
						isNull = nullable.IsNull;
						return nullable;
					}
					isSqlType = false;
					isDataFeed = false;
					object value2 = this._SqlDataReaderRowSource.GetValue(sourceColumnOrdinal);
					isNull = value2 == null || value2 == DBNull.Value;
					if (!isNull && metadata.type == SqlDbType.Udt)
					{
						INullable nullable2 = value2 as INullable;
						isNull = nullable2 != null && nullable2.IsNull;
					}
					return value2;
				}
				else
				{
					isDataFeed = false;
					IDataReader dataReader = (IDataReader)this._rowSource;
					if (this._enableStreaming && this._SqlDataReaderRowSource == null && dataReader.IsDBNull(sourceColumnOrdinal))
					{
						isSqlType = false;
						isNull = true;
						return DBNull.Value;
					}
					object value3 = dataReader.GetValue(sourceColumnOrdinal);
					ADP.IsNullOrSqlType(value3, out isNull, out isSqlType);
					return value3;
				}
				break;
			case SqlBulkCopy.ValueSourceType.DataTable:
			case SqlBulkCopy.ValueSourceType.RowArray:
			{
				isDataFeed = false;
				object obj = this._currentRow[sourceColumnOrdinal];
				ADP.IsNullOrSqlType(obj, out isNull, out isSqlType);
				if (!isNull && this._currentRowMetadata[destRowIndex].IsSqlType)
				{
					switch (this._currentRowMetadata[destRowIndex].Method)
					{
					case SqlBulkCopy.ValueMethod.SqlTypeSqlDecimal:
						if (isSqlType)
						{
							return (SqlDecimal)obj;
						}
						isSqlType = true;
						return new SqlDecimal((decimal)obj);
					case SqlBulkCopy.ValueMethod.SqlTypeSqlDouble:
					{
						if (isSqlType)
						{
							return new SqlDecimal(((SqlDouble)obj).Value);
						}
						double num = (double)obj;
						if (!double.IsNaN(num))
						{
							isSqlType = true;
							return new SqlDecimal(num);
						}
						break;
					}
					case SqlBulkCopy.ValueMethod.SqlTypeSqlSingle:
					{
						if (isSqlType)
						{
							return new SqlDecimal((double)((SqlSingle)obj).Value);
						}
						float num2 = (float)obj;
						if (!float.IsNaN(num2))
						{
							isSqlType = true;
							return new SqlDecimal((double)num2);
						}
						break;
					}
					}
				}
				return obj;
			}
			default:
				throw ADP.NotSupported();
			}
		}

		private Task ReadFromRowSourceAsync(CancellationToken cts)
		{
			if (this._isAsyncBulkCopy && this._DbDataReaderRowSource != null)
			{
				return this._DbDataReaderRowSource.ReadAsync(cts).ContinueWith<Task<bool>>(delegate(Task<bool> t)
				{
					if (t.Status == TaskStatus.RanToCompletion)
					{
						this._hasMoreRowToCopy = t.Result;
					}
					return t;
				}, TaskScheduler.Default).Unwrap<bool>();
			}
			this._hasMoreRowToCopy = false;
			try
			{
				this._hasMoreRowToCopy = this.ReadFromRowSource();
			}
			catch (Exception ex)
			{
				if (this._isAsyncBulkCopy)
				{
					return Task.FromException<bool>(ex);
				}
				throw;
			}
			return null;
		}

		private bool ReadFromRowSource()
		{
			switch (this._rowSourceType)
			{
			case SqlBulkCopy.ValueSourceType.IDataReader:
			case SqlBulkCopy.ValueSourceType.DbDataReader:
				return ((IDataReader)this._rowSource).Read();
			case SqlBulkCopy.ValueSourceType.DataTable:
			case SqlBulkCopy.ValueSourceType.RowArray:
				while (this._rowEnumerator.MoveNext())
				{
					this._currentRow = (DataRow)this._rowEnumerator.Current;
					if ((this._currentRow.RowState & this._rowStateToSkip) == (DataRowState)0)
					{
						this._currentRowLength = this._currentRow.ItemArray.Length;
						return true;
					}
				}
				return false;
			default:
				throw ADP.NotSupported();
			}
		}

		private SqlBulkCopy.SourceColumnMetadata GetColumnMetadata(int ordinal)
		{
			int sourceColumnOrdinal = this._sortedColumnMappings[ordinal]._sourceColumnOrdinal;
			_SqlMetaData metadata = this._sortedColumnMappings[ordinal]._metadata;
			bool flag;
			bool flag2;
			SqlBulkCopy.ValueMethod valueMethod;
			if ((this._SqlDataReaderRowSource != null || this._dataTableSource != null) && (metadata.metaType.NullableType == 106 || metadata.metaType.NullableType == 108))
			{
				flag = false;
				Type type;
				switch (this._rowSourceType)
				{
				case SqlBulkCopy.ValueSourceType.IDataReader:
				case SqlBulkCopy.ValueSourceType.DbDataReader:
					type = this._SqlDataReaderRowSource.GetFieldType(sourceColumnOrdinal);
					break;
				case SqlBulkCopy.ValueSourceType.DataTable:
				case SqlBulkCopy.ValueSourceType.RowArray:
					type = this._dataTableSource.Columns[sourceColumnOrdinal].DataType;
					break;
				default:
					type = null;
					break;
				}
				if (typeof(SqlDecimal) == type || typeof(decimal) == type)
				{
					flag2 = true;
					valueMethod = SqlBulkCopy.ValueMethod.SqlTypeSqlDecimal;
				}
				else if (typeof(SqlDouble) == type || typeof(double) == type)
				{
					flag2 = true;
					valueMethod = SqlBulkCopy.ValueMethod.SqlTypeSqlDouble;
				}
				else if (typeof(SqlSingle) == type || typeof(float) == type)
				{
					flag2 = true;
					valueMethod = SqlBulkCopy.ValueMethod.SqlTypeSqlSingle;
				}
				else
				{
					flag2 = false;
					valueMethod = SqlBulkCopy.ValueMethod.GetValue;
				}
			}
			else if (this._enableStreaming && metadata.length == 2147483647)
			{
				flag2 = false;
				if (this._SqlDataReaderRowSource != null)
				{
					MetaType metaType = this._SqlDataReaderRowSource.MetaData[sourceColumnOrdinal].metaType;
					if (metadata.type == SqlDbType.VarBinary && metaType.IsBinType && metaType.SqlDbType != SqlDbType.Timestamp && this._SqlDataReaderRowSource.IsCommandBehavior(CommandBehavior.SequentialAccess))
					{
						flag = true;
						valueMethod = SqlBulkCopy.ValueMethod.DataFeedStream;
					}
					else if ((metadata.type == SqlDbType.VarChar || metadata.type == SqlDbType.NVarChar) && metaType.IsCharType && metaType.SqlDbType != SqlDbType.Xml)
					{
						flag = true;
						valueMethod = SqlBulkCopy.ValueMethod.DataFeedText;
					}
					else if (metadata.type == SqlDbType.Xml && metaType.SqlDbType == SqlDbType.Xml)
					{
						flag = true;
						valueMethod = SqlBulkCopy.ValueMethod.DataFeedXml;
					}
					else
					{
						flag = false;
						valueMethod = SqlBulkCopy.ValueMethod.GetValue;
					}
				}
				else if (this._DbDataReaderRowSource != null)
				{
					if (metadata.type == SqlDbType.VarBinary)
					{
						flag = true;
						valueMethod = SqlBulkCopy.ValueMethod.DataFeedStream;
					}
					else if (metadata.type == SqlDbType.VarChar || metadata.type == SqlDbType.NVarChar)
					{
						flag = true;
						valueMethod = SqlBulkCopy.ValueMethod.DataFeedText;
					}
					else
					{
						flag = false;
						valueMethod = SqlBulkCopy.ValueMethod.GetValue;
					}
				}
				else
				{
					flag = false;
					valueMethod = SqlBulkCopy.ValueMethod.GetValue;
				}
			}
			else
			{
				flag2 = false;
				flag = false;
				valueMethod = SqlBulkCopy.ValueMethod.GetValue;
			}
			return new SqlBulkCopy.SourceColumnMetadata(valueMethod, flag2, flag);
		}

		private void CreateOrValidateConnection(string method)
		{
			if (this._connection == null)
			{
				throw ADP.ConnectionRequired(method);
			}
			if (this._ownConnection && this._connection.State != ConnectionState.Open)
			{
				this._connection.Open();
			}
			this._connection.ValidateConnectionForExecute(method, null);
			if (this._externalTransaction != null && this._connection != this._externalTransaction.Connection)
			{
				throw ADP.TransactionConnectionMismatch();
			}
		}

		private void RunParser(BulkCopySimpleResultSet bulkCopyHandler = null)
		{
			SqlInternalConnectionTds openTdsConnection = this._connection.GetOpenTdsConnection();
			openTdsConnection.ThreadHasParserLockForClose = true;
			try
			{
				this._parser.Run(RunBehavior.UntilDone, null, null, bulkCopyHandler, this._stateObj);
			}
			finally
			{
				openTdsConnection.ThreadHasParserLockForClose = false;
			}
		}

		private void RunParserReliably(BulkCopySimpleResultSet bulkCopyHandler = null)
		{
			SqlInternalConnectionTds openTdsConnection = this._connection.GetOpenTdsConnection();
			openTdsConnection.ThreadHasParserLockForClose = true;
			try
			{
				this._parser.Run(RunBehavior.UntilDone, null, null, bulkCopyHandler, this._stateObj);
			}
			finally
			{
				openTdsConnection.ThreadHasParserLockForClose = false;
			}
		}

		private void CommitTransaction()
		{
			if (this._internalTransaction != null)
			{
				SqlInternalConnectionTds openTdsConnection = this._connection.GetOpenTdsConnection();
				openTdsConnection.ThreadHasParserLockForClose = true;
				try
				{
					this._internalTransaction.Commit();
					this._internalTransaction.Dispose();
					this._internalTransaction = null;
				}
				finally
				{
					openTdsConnection.ThreadHasParserLockForClose = false;
				}
			}
		}

		private void AbortTransaction()
		{
			if (this._internalTransaction != null)
			{
				if (!this._internalTransaction.IsZombied)
				{
					SqlInternalConnectionTds openTdsConnection = this._connection.GetOpenTdsConnection();
					openTdsConnection.ThreadHasParserLockForClose = true;
					try
					{
						this._internalTransaction.Rollback();
					}
					finally
					{
						openTdsConnection.ThreadHasParserLockForClose = false;
					}
				}
				this._internalTransaction.Dispose();
				this._internalTransaction = null;
			}
		}

		private void AppendColumnNameAndTypeName(StringBuilder query, string columnName, string typeName)
		{
			SqlServerEscapeHelper.EscapeIdentifier(query, columnName);
			query.Append(" ");
			query.Append(typeName);
		}

		private string UnquotedName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			if (name[0] == '[')
			{
				int length = name.Length;
				name = name.Substring(1, length - 2);
			}
			return name;
		}

		private object ValidateBulkCopyVariant(object value)
		{
			byte tdstype = MetaType.GetMetaTypeFromValue(value, true, true).TDSType;
			if (tdstype <= 108)
			{
				if (tdstype <= 43)
				{
					if (tdstype != 36 && tdstype - 40 > 3)
					{
						goto IL_00AD;
					}
				}
				else
				{
					switch (tdstype)
					{
					case 48:
					case 50:
					case 52:
					case 56:
					case 59:
					case 60:
					case 61:
					case 62:
						break;
					case 49:
					case 51:
					case 53:
					case 54:
					case 55:
					case 57:
					case 58:
						goto IL_00AD;
					default:
						if (tdstype != 108)
						{
							goto IL_00AD;
						}
						break;
					}
				}
			}
			else if (tdstype <= 165)
			{
				if (tdstype != 127 && tdstype != 165)
				{
					goto IL_00AD;
				}
			}
			else if (tdstype != 167 && tdstype != 231)
			{
				goto IL_00AD;
			}
			if (value is INullable)
			{
				return MetaType.GetComValueFromSqlVariant(value);
			}
			return value;
			IL_00AD:
			throw SQL.BulkLoadInvalidVariantValue();
		}

		private object ConvertValue(object value, _SqlMetaData metadata, bool isNull, ref bool isSqlType, out bool coercedToDataFeed)
		{
			coercedToDataFeed = false;
			if (!isNull)
			{
				MetaType metaType = metadata.metaType;
				bool flag = false;
				object obj;
				try
				{
					byte nullableType = metaType.NullableType;
					MetaType metaType2;
					if (nullableType <= 165)
					{
						if (nullableType <= 59)
						{
							switch (nullableType)
							{
							case 34:
							case 35:
							case 36:
							case 38:
							case 40:
							case 41:
							case 42:
							case 43:
							case 50:
								break;
							case 37:
							case 39:
							case 44:
							case 45:
							case 46:
							case 47:
							case 48:
							case 49:
								goto IL_02A3;
							default:
								if (nullableType - 58 > 1)
								{
									goto IL_02A3;
								}
								break;
							}
						}
						else if (nullableType - 61 > 1)
						{
							switch (nullableType)
							{
							case 98:
								value = this.ValidateBulkCopyVariant(value);
								flag = true;
								goto IL_02B6;
							case 99:
								goto IL_0212;
							case 100:
							case 101:
							case 102:
							case 103:
							case 105:
							case 107:
								goto IL_02A3;
							case 104:
							case 109:
							case 110:
							case 111:
								break;
							case 106:
							case 108:
							{
								metaType2 = MetaType.GetMetaTypeFromSqlDbType(metaType.SqlDbType, false);
								value = SqlParameter.CoerceValue(value, metaType2, out coercedToDataFeed, out flag, false);
								SqlDecimal sqlDecimal;
								if (isSqlType && !flag)
								{
									sqlDecimal = (SqlDecimal)value;
								}
								else
								{
									sqlDecimal = new SqlDecimal((decimal)value);
								}
								if (sqlDecimal.Scale != metadata.scale)
								{
									sqlDecimal = TdsParser.AdjustSqlDecimalScale(sqlDecimal, (int)metadata.scale);
								}
								if (sqlDecimal.Precision > metadata.precision)
								{
									try
									{
										sqlDecimal = SqlDecimal.ConvertToPrecScale(sqlDecimal, (int)metadata.precision, (int)sqlDecimal.Scale);
									}
									catch (SqlTruncateException)
									{
										throw SQL.BulkLoadCannotConvertValue(value.GetType(), metaType2, ADP.ParameterValueOutOfRange(sqlDecimal));
									}
								}
								value = sqlDecimal;
								isSqlType = true;
								flag = false;
								goto IL_02B6;
							}
							default:
								if (nullableType != 165)
								{
									goto IL_02A3;
								}
								break;
							}
						}
					}
					else if (nullableType <= 173)
					{
						if (nullableType != 167 && nullableType != 173)
						{
							goto IL_02A3;
						}
					}
					else if (nullableType != 175)
					{
						if (nullableType == 231)
						{
							goto IL_0212;
						}
						switch (nullableType)
						{
						case 239:
							goto IL_0212;
						case 240:
							throw ADP.DbTypeNotSupported("UDT");
						case 241:
							if (value is XmlReader)
							{
								value = new XmlDataFeed((XmlReader)value);
								flag = true;
								coercedToDataFeed = true;
								goto IL_02B6;
							}
							goto IL_02B6;
						default:
							goto IL_02A3;
						}
					}
					metaType2 = MetaType.GetMetaTypeFromSqlDbType(metaType.SqlDbType, false);
					value = SqlParameter.CoerceValue(value, metaType2, out coercedToDataFeed, out flag, false);
					goto IL_02B6;
					IL_0212:
					metaType2 = MetaType.GetMetaTypeFromSqlDbType(metaType.SqlDbType, false);
					value = SqlParameter.CoerceValue(value, metaType2, out coercedToDataFeed, out flag, false);
					if (!coercedToDataFeed && ((isSqlType && !flag) ? ((SqlString)value).Value.Length : ((string)value).Length) > metadata.length / 2)
					{
						throw SQL.BulkLoadStringTooLong();
					}
					goto IL_02B6;
					IL_02A3:
					throw SQL.BulkLoadCannotConvertValue(value.GetType(), metadata.metaType, null);
					IL_02B6:
					if (flag)
					{
						isSqlType = false;
					}
					obj = value;
				}
				catch (Exception ex)
				{
					if (!ADP.IsCatchableExceptionType(ex))
					{
						throw;
					}
					throw SQL.BulkLoadCannotConvertValue(value.GetType(), metadata.metaType, ex);
				}
				return obj;
			}
			if (!metadata.isNullable)
			{
				throw SQL.BulkLoadBulkLoadNotAllowDBNull(metadata.column);
			}
			return value;
		}

		public void WriteToServer(DbDataReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (this._isBulkCopyingInProgress)
			{
				throw SQL.BulkLoadPendingOperation();
			}
			SqlStatistics sqlStatistics = this.Statistics;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				this._rowSource = reader;
				this._DbDataReaderRowSource = reader;
				this._SqlDataReaderRowSource = reader as SqlDataReader;
				this._dataTableSource = null;
				this._rowSourceType = SqlBulkCopy.ValueSourceType.DbDataReader;
				this._isAsyncBulkCopy = false;
				this.WriteRowSourceToServerAsync(reader.FieldCount, CancellationToken.None);
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
			}
		}

		public void WriteToServer(IDataReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (this._isBulkCopyingInProgress)
			{
				throw SQL.BulkLoadPendingOperation();
			}
			SqlStatistics sqlStatistics = this.Statistics;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				this._rowSource = reader;
				this._SqlDataReaderRowSource = this._rowSource as SqlDataReader;
				this._DbDataReaderRowSource = this._rowSource as DbDataReader;
				this._dataTableSource = null;
				this._rowSourceType = SqlBulkCopy.ValueSourceType.IDataReader;
				this._isAsyncBulkCopy = false;
				this.WriteRowSourceToServerAsync(reader.FieldCount, CancellationToken.None);
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
			}
		}

		public void WriteToServer(DataTable table)
		{
			this.WriteToServer(table, (DataRowState)0);
		}

		public void WriteToServer(DataTable table, DataRowState rowState)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			if (this._isBulkCopyingInProgress)
			{
				throw SQL.BulkLoadPendingOperation();
			}
			SqlStatistics sqlStatistics = this.Statistics;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				this._rowStateToSkip = ((rowState == (DataRowState)0 || rowState == DataRowState.Deleted) ? DataRowState.Deleted : (~rowState | DataRowState.Deleted));
				this._rowSource = table;
				this._dataTableSource = table;
				this._SqlDataReaderRowSource = null;
				this._rowSourceType = SqlBulkCopy.ValueSourceType.DataTable;
				this._rowEnumerator = table.Rows.GetEnumerator();
				this._isAsyncBulkCopy = false;
				this.WriteRowSourceToServerAsync(table.Columns.Count, CancellationToken.None);
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
			}
		}

		public void WriteToServer(DataRow[] rows)
		{
			SqlStatistics sqlStatistics = this.Statistics;
			if (rows == null)
			{
				throw new ArgumentNullException("rows");
			}
			if (this._isBulkCopyingInProgress)
			{
				throw SQL.BulkLoadPendingOperation();
			}
			if (rows.Length == 0)
			{
				return;
			}
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				DataTable table = rows[0].Table;
				this._rowStateToSkip = DataRowState.Deleted;
				this._rowSource = rows;
				this._dataTableSource = table;
				this._SqlDataReaderRowSource = null;
				this._rowSourceType = SqlBulkCopy.ValueSourceType.RowArray;
				this._rowEnumerator = rows.GetEnumerator();
				this._isAsyncBulkCopy = false;
				this.WriteRowSourceToServerAsync(table.Columns.Count, CancellationToken.None);
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
			}
		}

		public Task WriteToServerAsync(DataRow[] rows)
		{
			return this.WriteToServerAsync(rows, CancellationToken.None);
		}

		public Task WriteToServerAsync(DataRow[] rows, CancellationToken cancellationToken)
		{
			Task task = null;
			if (rows == null)
			{
				throw new ArgumentNullException("rows");
			}
			if (this._isBulkCopyingInProgress)
			{
				throw SQL.BulkLoadPendingOperation();
			}
			SqlStatistics sqlStatistics = this.Statistics;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				if (rows.Length == 0)
				{
					return cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;
				}
				DataTable table = rows[0].Table;
				this._rowStateToSkip = DataRowState.Deleted;
				this._rowSource = rows;
				this._dataTableSource = table;
				this._SqlDataReaderRowSource = null;
				this._rowSourceType = SqlBulkCopy.ValueSourceType.RowArray;
				this._rowEnumerator = rows.GetEnumerator();
				this._isAsyncBulkCopy = true;
				task = this.WriteRowSourceToServerAsync(table.Columns.Count, cancellationToken);
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
			}
			return task;
		}

		public Task WriteToServerAsync(DbDataReader reader)
		{
			return this.WriteToServerAsync(reader, CancellationToken.None);
		}

		public Task WriteToServerAsync(DbDataReader reader, CancellationToken cancellationToken)
		{
			Task task = null;
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (this._isBulkCopyingInProgress)
			{
				throw SQL.BulkLoadPendingOperation();
			}
			SqlStatistics sqlStatistics = this.Statistics;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				this._rowSource = reader;
				this._SqlDataReaderRowSource = reader as SqlDataReader;
				this._DbDataReaderRowSource = reader;
				this._dataTableSource = null;
				this._rowSourceType = SqlBulkCopy.ValueSourceType.DbDataReader;
				this._isAsyncBulkCopy = true;
				task = this.WriteRowSourceToServerAsync(reader.FieldCount, cancellationToken);
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
			}
			return task;
		}

		public Task WriteToServerAsync(IDataReader reader)
		{
			return this.WriteToServerAsync(reader, CancellationToken.None);
		}

		public Task WriteToServerAsync(IDataReader reader, CancellationToken cancellationToken)
		{
			Task task = null;
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (this._isBulkCopyingInProgress)
			{
				throw SQL.BulkLoadPendingOperation();
			}
			SqlStatistics sqlStatistics = this.Statistics;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				this._rowSource = reader;
				this._SqlDataReaderRowSource = this._rowSource as SqlDataReader;
				this._DbDataReaderRowSource = this._rowSource as DbDataReader;
				this._dataTableSource = null;
				this._rowSourceType = SqlBulkCopy.ValueSourceType.IDataReader;
				this._isAsyncBulkCopy = true;
				task = this.WriteRowSourceToServerAsync(reader.FieldCount, cancellationToken);
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
			}
			return task;
		}

		public Task WriteToServerAsync(DataTable table)
		{
			return this.WriteToServerAsync(table, (DataRowState)0, CancellationToken.None);
		}

		public Task WriteToServerAsync(DataTable table, CancellationToken cancellationToken)
		{
			return this.WriteToServerAsync(table, (DataRowState)0, cancellationToken);
		}

		public Task WriteToServerAsync(DataTable table, DataRowState rowState)
		{
			return this.WriteToServerAsync(table, rowState, CancellationToken.None);
		}

		public Task WriteToServerAsync(DataTable table, DataRowState rowState, CancellationToken cancellationToken)
		{
			Task task = null;
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			if (this._isBulkCopyingInProgress)
			{
				throw SQL.BulkLoadPendingOperation();
			}
			SqlStatistics sqlStatistics = this.Statistics;
			try
			{
				sqlStatistics = SqlStatistics.StartTimer(this.Statistics);
				this._rowStateToSkip = ((rowState == (DataRowState)0 || rowState == DataRowState.Deleted) ? DataRowState.Deleted : (~rowState | DataRowState.Deleted));
				this._rowSource = table;
				this._SqlDataReaderRowSource = null;
				this._dataTableSource = table;
				this._rowSourceType = SqlBulkCopy.ValueSourceType.DataTable;
				this._rowEnumerator = table.Rows.GetEnumerator();
				this._isAsyncBulkCopy = true;
				task = this.WriteRowSourceToServerAsync(table.Columns.Count, cancellationToken);
			}
			finally
			{
				SqlStatistics.StopTimer(sqlStatistics);
			}
			return task;
		}

		private Task WriteRowSourceToServerAsync(int columnCount, CancellationToken ctoken)
		{
			Task currentReconnectionTask = this._connection._currentReconnectionTask;
			if (currentReconnectionTask != null && !currentReconnectionTask.IsCompleted)
			{
				if (this._isAsyncBulkCopy)
				{
					TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
					Action <>9__2;
					currentReconnectionTask.ContinueWith(delegate(Task t)
					{
						Task task3 = this.WriteRowSourceToServerAsync(columnCount, ctoken);
						if (task3 == null)
						{
							tcs.SetResult(null);
							return;
						}
						Task task4 = task3;
						TaskCompletionSource<object> tcs2 = tcs;
						Action action;
						if ((action = <>9__2) == null)
						{
							action = (<>9__2 = delegate
							{
								tcs.SetResult(null);
							});
						}
						AsyncHelper.ContinueTask(task4, tcs2, action, null, null, null, null, null);
					}, ctoken);
					return tcs.Task;
				}
				AsyncHelper.WaitForCompletion(currentReconnectionTask, this.BulkCopyTimeout, delegate
				{
					throw SQL.CR_ReconnectTimeout();
				}, false);
			}
			bool flag = true;
			this._isBulkCopyingInProgress = true;
			this.CreateOrValidateConnection("WriteToServer");
			SqlInternalConnectionTds openTdsConnection = this._connection.GetOpenTdsConnection();
			this._parserLock = openTdsConnection._parserLock;
			this._parserLock.Wait(this._isAsyncBulkCopy);
			Task task2;
			try
			{
				this.WriteRowSourceToServerCommon(columnCount);
				Task task = this.WriteToServerInternalAsync(ctoken);
				if (task != null)
				{
					flag = false;
					task2 = task.ContinueWith<Task>(delegate(Task t)
					{
						this.AbortTransaction();
						this._isBulkCopyingInProgress = false;
						if (this._parser != null)
						{
							this._parser._asyncWrite = false;
						}
						if (this._parserLock != null)
						{
							this._parserLock.Release();
							this._parserLock = null;
						}
						return t;
					}, TaskScheduler.Default).Unwrap();
				}
				else
				{
					task2 = null;
				}
			}
			catch (OutOfMemoryException ex)
			{
				this._connection.Abort(ex);
				throw;
			}
			catch (StackOverflowException ex2)
			{
				this._connection.Abort(ex2);
				throw;
			}
			catch (ThreadAbortException ex3)
			{
				this._connection.Abort(ex3);
				throw;
			}
			finally
			{
				this._columnMappings.ReadOnly = false;
				if (flag)
				{
					this.AbortTransaction();
					this._isBulkCopyingInProgress = false;
					if (this._parser != null)
					{
						this._parser._asyncWrite = false;
					}
					if (this._parserLock != null)
					{
						this._parserLock.Release();
						this._parserLock = null;
					}
				}
			}
			return task2;
		}

		private void WriteRowSourceToServerCommon(int columnCount)
		{
			bool flag = false;
			this._columnMappings.ReadOnly = true;
			this._localColumnMappings = this._columnMappings;
			if (this._localColumnMappings.Count > 0)
			{
				this._localColumnMappings.ValidateCollection();
				using (IEnumerator enumerator = this._localColumnMappings.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (((SqlBulkCopyColumnMapping)enumerator.Current)._internalSourceColumnOrdinal == -1)
						{
							flag = true;
							break;
						}
					}
					goto IL_008A;
				}
			}
			this._localColumnMappings = new SqlBulkCopyColumnMappingCollection();
			this._localColumnMappings.CreateDefaultMapping(columnCount);
			IL_008A:
			if (flag)
			{
				int num = -1;
				flag = false;
				if (this._localColumnMappings.Count > 0)
				{
					foreach (object obj in this._localColumnMappings)
					{
						SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping = (SqlBulkCopyColumnMapping)obj;
						if (sqlBulkCopyColumnMapping._internalSourceColumnOrdinal == -1)
						{
							string text = this.UnquotedName(sqlBulkCopyColumnMapping.SourceColumn);
							switch (this._rowSourceType)
							{
							case SqlBulkCopy.ValueSourceType.IDataReader:
							case SqlBulkCopy.ValueSourceType.DbDataReader:
								try
								{
									num = this._DbDataReaderRowSource.GetOrdinal(text);
								}
								catch (IndexOutOfRangeException ex)
								{
									throw SQL.BulkLoadNonMatchingColumnName(text, ex);
								}
								break;
							case SqlBulkCopy.ValueSourceType.DataTable:
								num = ((DataTable)this._rowSource).Columns.IndexOf(text);
								break;
							case SqlBulkCopy.ValueSourceType.RowArray:
								num = ((DataRow[])this._rowSource)[0].Table.Columns.IndexOf(text);
								break;
							}
							if (num == -1)
							{
								throw SQL.BulkLoadNonMatchingColumnName(text);
							}
							sqlBulkCopyColumnMapping._internalSourceColumnOrdinal = num;
						}
					}
				}
			}
		}

		internal void OnConnectionClosed()
		{
			TdsParserStateObject stateObj = this._stateObj;
			if (stateObj != null)
			{
				stateObj.OnConnectionClosed();
			}
		}

		private void OnRowsCopied(SqlRowsCopiedEventArgs value)
		{
			SqlRowsCopiedEventHandler rowsCopiedEventHandler = this._rowsCopiedEventHandler;
			if (rowsCopiedEventHandler != null)
			{
				rowsCopiedEventHandler(this, value);
			}
		}

		private bool FireRowsCopiedEvent(long rowsCopied)
		{
			SqlInternalConnectionTds openTdsConnection = this._connection.GetOpenTdsConnection();
			bool canBeReleasedFromAnyThread = openTdsConnection._parserLock.CanBeReleasedFromAnyThread;
			openTdsConnection._parserLock.Release();
			SqlRowsCopiedEventArgs e = new SqlRowsCopiedEventArgs(rowsCopied);
			try
			{
				this._insideRowsCopiedEvent = true;
				this.OnRowsCopied(e);
			}
			finally
			{
				this._insideRowsCopiedEvent = false;
				openTdsConnection._parserLock.Wait(canBeReleasedFromAnyThread);
			}
			return e.Abort;
		}

		private Task ReadWriteColumnValueAsync(int col)
		{
			bool flag;
			bool flag2;
			bool flag3;
			object obj = this.GetValueFromSourceRow(col, out flag, out flag2, out flag3);
			_SqlMetaData metadata = this._sortedColumnMappings[col]._metadata;
			if (!flag2)
			{
				obj = this.ConvertValue(obj, metadata, flag3, ref flag, out flag2);
			}
			Task task = null;
			if (metadata.type != SqlDbType.Variant)
			{
				task = this._parser.WriteBulkCopyValue(obj, metadata, this._stateObj, flag, flag2, flag3);
			}
			else
			{
				SqlBuffer.StorageType storageType = SqlBuffer.StorageType.Empty;
				if (this._SqlDataReaderRowSource != null && this._connection.IsKatmaiOrNewer)
				{
					storageType = this._SqlDataReaderRowSource.GetVariantInternalStorageType(this._sortedColumnMappings[col]._sourceColumnOrdinal);
				}
				if (storageType == SqlBuffer.StorageType.DateTime2)
				{
					this._parser.WriteSqlVariantDateTime2((DateTime)obj, this._stateObj);
				}
				else if (storageType == SqlBuffer.StorageType.Date)
				{
					this._parser.WriteSqlVariantDate((DateTime)obj, this._stateObj);
				}
				else
				{
					task = this._parser.WriteSqlVariantDataRowValue(obj, this._stateObj, true);
				}
			}
			return task;
		}

		private void RegisterForConnectionCloseNotification<T>(ref Task<T> outerTask)
		{
			SqlConnection connection = this._connection;
			if (connection == null)
			{
				throw ADP.ClosedConnectionError();
			}
			connection.RegisterForConnectionCloseNotification<T>(ref outerTask, this, 3);
		}

		private Task CopyColumnsAsync(int col, TaskCompletionSource<object> source = null)
		{
			Task task = null;
			Task task2 = null;
			try
			{
				int i;
				for (i = col; i < this._sortedColumnMappings.Count; i++)
				{
					task2 = this.ReadWriteColumnValueAsync(i);
					if (task2 != null)
					{
						break;
					}
				}
				if (task2 != null)
				{
					if (source == null)
					{
						source = new TaskCompletionSource<object>();
						task = source.Task;
					}
					this.CopyColumnsAsyncSetupContinuation(source, task2, i);
					return task;
				}
				if (source != null)
				{
					source.SetResult(null);
				}
			}
			catch (Exception ex)
			{
				if (source == null)
				{
					throw;
				}
				source.TrySetException(ex);
			}
			return task;
		}

		private void CopyColumnsAsyncSetupContinuation(TaskCompletionSource<object> source, Task task, int i)
		{
			AsyncHelper.ContinueTask(task, source, delegate
			{
				if (i + 1 < this._sortedColumnMappings.Count)
				{
					this.CopyColumnsAsync(i + 1, source);
					return;
				}
				source.SetResult(null);
			}, this._connection.GetOpenTdsConnection(), null, null, null, null);
		}

		private void CheckAndRaiseNotification()
		{
			bool flag = false;
			Exception ex = null;
			this._rowsCopied++;
			if (this._notifyAfter > 0 && this._rowsUntilNotification > 0)
			{
				int num = this._rowsUntilNotification - 1;
				this._rowsUntilNotification = num;
				if (num == 0)
				{
					try
					{
						this._stateObj.BcpLock = true;
						flag = this.FireRowsCopiedEvent((long)this._rowsCopied);
						if (ConnectionState.Open != this._connection.State)
						{
							ex = ADP.OpenConnectionRequired("CheckAndRaiseNotification", this._connection.State);
						}
					}
					catch (Exception ex2)
					{
						if (!ADP.IsCatchableExceptionType(ex2))
						{
							ex = ex2;
						}
						else
						{
							ex = OperationAbortedException.Aborted(ex2);
						}
					}
					finally
					{
						this._stateObj.BcpLock = false;
					}
					if (!flag)
					{
						this._rowsUntilNotification = this._notifyAfter;
					}
				}
			}
			if (!flag && this._rowsUntilNotification > this._notifyAfter)
			{
				this._rowsUntilNotification = this._notifyAfter;
			}
			if (ex == null && flag)
			{
				ex = OperationAbortedException.Aborted(null);
			}
			if (this._connection.State != ConnectionState.Open)
			{
				throw ADP.OpenConnectionRequired("WriteToServer", this._connection.State);
			}
			if (ex != null)
			{
				this._parser._asyncWrite = false;
				this._parser.WriteBulkCopyDone(this._stateObj);
				this.RunParser(null);
				this.AbortTransaction();
				throw ex;
			}
		}

		private Task CheckForCancellation(CancellationToken cts, TaskCompletionSource<object> tcs)
		{
			if (cts.IsCancellationRequested)
			{
				if (tcs == null)
				{
					tcs = new TaskCompletionSource<object>();
				}
				tcs.SetCanceled();
				return tcs.Task;
			}
			return null;
		}

		private TaskCompletionSource<object> ContinueTaskPend(Task task, TaskCompletionSource<object> source, Func<TaskCompletionSource<object>> action)
		{
			if (task == null)
			{
				return action();
			}
			AsyncHelper.ContinueTask(task, source, delegate
			{
				action();
			}, null, null, null, null, null);
			return null;
		}

		private Task CopyRowsAsync(int rowsSoFar, int totalRows, CancellationToken cts, TaskCompletionSource<object> source = null)
		{
			Task task = null;
			try
			{
				int i = rowsSoFar;
				Action <>9__1;
				Action <>9__2;
				while ((totalRows <= 0 || i < totalRows) && this._hasMoreRowToCopy)
				{
					if (this._isAsyncBulkCopy)
					{
						task = this.CheckForCancellation(cts, source);
						if (task != null)
						{
							return task;
						}
					}
					this._stateObj.WriteByte(209);
					Task task2 = this.CopyColumnsAsync(0, null);
					if (task2 != null)
					{
						source = source ?? new TaskCompletionSource<object>();
						task = source.Task;
						AsyncHelper.ContinueTask(task2, source, delegate
						{
							this.CheckAndRaiseNotification();
							Task task5 = this.ReadFromRowSourceAsync(cts);
							if (task5 == null)
							{
								this.CopyRowsAsync(i + 1, totalRows, cts, source);
								return;
							}
							Task task6 = task5;
							TaskCompletionSource<object> source3 = source;
							Action action2;
							if ((action2 = <>9__2) == null)
							{
								action2 = (<>9__2 = delegate
								{
									this.CopyRowsAsync(i + 1, totalRows, cts, source);
								});
							}
							AsyncHelper.ContinueTask(task6, source3, action2, this._connection.GetOpenTdsConnection(), null, null, null, null);
						}, this._connection.GetOpenTdsConnection(), null, null, null, null);
						return task;
					}
					this.CheckAndRaiseNotification();
					Task task3 = this.ReadFromRowSourceAsync(cts);
					if (task3 != null)
					{
						if (source == null)
						{
							source = new TaskCompletionSource<object>();
						}
						task = source.Task;
						Task task4 = task3;
						TaskCompletionSource<object> source2 = source;
						Action action;
						if ((action = <>9__1) == null)
						{
							action = (<>9__1 = delegate
							{
								this.CopyRowsAsync(i + 1, totalRows, cts, source);
							});
						}
						AsyncHelper.ContinueTask(task4, source2, action, this._connection.GetOpenTdsConnection(), null, null, null, null);
						return task;
					}
					int j = i;
					i = j + 1;
				}
				if (source != null)
				{
					source.TrySetResult(null);
				}
			}
			catch (Exception ex)
			{
				if (source == null)
				{
					throw;
				}
				source.TrySetException(ex);
			}
			return task;
		}

		private Task CopyBatchesAsync(BulkCopySimpleResultSet internalResults, string updateBulkCommandText, CancellationToken cts, TaskCompletionSource<object> source = null)
		{
			try
			{
				Action <>9__0;
				while (this._hasMoreRowToCopy)
				{
					SqlInternalConnectionTds openTdsConnection = this._connection.GetOpenTdsConnection();
					if (this.IsCopyOption(SqlBulkCopyOptions.UseInternalTransaction))
					{
						openTdsConnection.ThreadHasParserLockForClose = true;
						try
						{
							this._internalTransaction = this._connection.BeginTransaction();
						}
						finally
						{
							openTdsConnection.ThreadHasParserLockForClose = false;
						}
					}
					Task task = this.SubmitUpdateBulkCommand(updateBulkCommandText);
					if (task != null)
					{
						if (source == null)
						{
							source = new TaskCompletionSource<object>();
						}
						Task task2 = task;
						TaskCompletionSource<object> source2 = source;
						Action action;
						if ((action = <>9__0) == null)
						{
							action = (<>9__0 = delegate
							{
								if (this.CopyBatchesAsyncContinued(internalResults, updateBulkCommandText, cts, source) == null)
								{
									this.CopyBatchesAsync(internalResults, updateBulkCommandText, cts, source);
								}
							});
						}
						AsyncHelper.ContinueTask(task2, source2, action, this._connection.GetOpenTdsConnection(), null, null, null, null);
						return source.Task;
					}
					Task task3 = this.CopyBatchesAsyncContinued(internalResults, updateBulkCommandText, cts, source);
					if (task3 != null)
					{
						return task3;
					}
				}
			}
			catch (Exception ex)
			{
				if (source != null)
				{
					source.TrySetException(ex);
					return source.Task;
				}
				throw;
			}
			if (source != null)
			{
				source.SetResult(null);
				return source.Task;
			}
			return null;
		}

		private Task CopyBatchesAsyncContinued(BulkCopySimpleResultSet internalResults, string updateBulkCommandText, CancellationToken cts, TaskCompletionSource<object> source)
		{
			Task task2;
			try
			{
				this.WriteMetaData(internalResults);
				Task task = this.CopyRowsAsync(0, this._savedBatchSize, cts, null);
				if (task != null)
				{
					if (source == null)
					{
						source = new TaskCompletionSource<object>();
					}
					AsyncHelper.ContinueTask(task, source, delegate
					{
						if (this.CopyBatchesAsyncContinuedOnSuccess(internalResults, updateBulkCommandText, cts, source) == null)
						{
							this.CopyBatchesAsync(internalResults, updateBulkCommandText, cts, source);
						}
					}, this._connection.GetOpenTdsConnection(), delegate(Exception _)
					{
						this.CopyBatchesAsyncContinuedOnError(false);
					}, delegate
					{
						this.CopyBatchesAsyncContinuedOnError(true);
					}, null, null);
					task2 = source.Task;
				}
				else
				{
					task2 = this.CopyBatchesAsyncContinuedOnSuccess(internalResults, updateBulkCommandText, cts, source);
				}
			}
			catch (Exception ex)
			{
				if (source == null)
				{
					throw;
				}
				source.TrySetException(ex);
				task2 = source.Task;
			}
			return task2;
		}

		private Task CopyBatchesAsyncContinuedOnSuccess(BulkCopySimpleResultSet internalResults, string updateBulkCommandText, CancellationToken cts, TaskCompletionSource<object> source)
		{
			Task task2;
			try
			{
				Task task = this._parser.WriteBulkCopyDone(this._stateObj);
				if (task == null)
				{
					this.RunParser(null);
					this.CommitTransaction();
					task2 = null;
				}
				else
				{
					if (source == null)
					{
						source = new TaskCompletionSource<object>();
					}
					AsyncHelper.ContinueTask(task, source, delegate
					{
						try
						{
							this.RunParser(null);
							this.CommitTransaction();
						}
						catch (Exception)
						{
							this.CopyBatchesAsyncContinuedOnError(false);
							throw;
						}
						this.CopyBatchesAsync(internalResults, updateBulkCommandText, cts, source);
					}, this._connection.GetOpenTdsConnection(), delegate(Exception _)
					{
						this.CopyBatchesAsyncContinuedOnError(false);
					}, null, null, null);
					task2 = source.Task;
				}
			}
			catch (Exception ex)
			{
				if (source == null)
				{
					throw;
				}
				source.TrySetException(ex);
				task2 = source.Task;
			}
			return task2;
		}

		private void CopyBatchesAsyncContinuedOnError(bool cleanupParser)
		{
			SqlInternalConnectionTds openTdsConnection = this._connection.GetOpenTdsConnection();
			try
			{
				if (cleanupParser && this._parser != null && this._stateObj != null)
				{
					this._parser._asyncWrite = false;
					this._parser.WriteBulkCopyDone(this._stateObj);
					this.RunParser(null);
				}
				if (this._stateObj != null)
				{
					this.CleanUpStateObjectOnError();
				}
			}
			catch (OutOfMemoryException)
			{
				openTdsConnection.DoomThisConnection();
				throw;
			}
			catch (StackOverflowException)
			{
				openTdsConnection.DoomThisConnection();
				throw;
			}
			catch (ThreadAbortException)
			{
				openTdsConnection.DoomThisConnection();
				throw;
			}
			this.AbortTransaction();
		}

		private void CleanUpStateObjectOnError()
		{
			if (this._stateObj != null)
			{
				this._parser.Connection.ThreadHasParserLockForClose = true;
				try
				{
					this._stateObj.ResetBuffer();
					this._stateObj._outputPacketNumber = 1;
					if (this._parser.State == TdsParserState.OpenNotLoggedIn || this._parser.State == TdsParserState.OpenLoggedIn)
					{
						this._stateObj.CancelRequest();
					}
					this._stateObj._internalTimeout = false;
					this._stateObj.CloseSession();
					this._stateObj._bulkCopyOpperationInProgress = false;
					this._stateObj._bulkCopyWriteTimeout = false;
					this._stateObj = null;
				}
				finally
				{
					this._parser.Connection.ThreadHasParserLockForClose = false;
				}
			}
		}

		private void WriteToServerInternalRestContinuedAsync(BulkCopySimpleResultSet internalResults, CancellationToken cts, TaskCompletionSource<object> source)
		{
			Task task = null;
			try
			{
				string text = this.AnalyzeTargetAndCreateUpdateBulkCommand(internalResults);
				if (this._sortedColumnMappings.Count != 0)
				{
					this._stateObj.SniContext = SniContext.Snix_SendRows;
					this._savedBatchSize = this._batchSize;
					this._rowsUntilNotification = this._notifyAfter;
					this._rowsCopied = 0;
					this._currentRowMetadata = new SqlBulkCopy.SourceColumnMetadata[this._sortedColumnMappings.Count];
					for (int i = 0; i < this._currentRowMetadata.Length; i++)
					{
						this._currentRowMetadata[i] = this.GetColumnMetadata(i);
					}
					task = this.CopyBatchesAsync(internalResults, text, cts, null);
				}
				if (task != null)
				{
					if (source == null)
					{
						source = new TaskCompletionSource<object>();
					}
					AsyncHelper.ContinueTask(task, source, delegate
					{
						if (task.IsCanceled)
						{
							this._localColumnMappings = null;
							try
							{
								this.CleanUpStateObjectOnError();
								return;
							}
							finally
							{
								source.SetCanceled();
							}
						}
						if (task.Exception != null)
						{
							source.SetException(task.Exception.InnerException);
							return;
						}
						this._localColumnMappings = null;
						try
						{
							this.CleanUpStateObjectOnError();
						}
						finally
						{
							if (source != null)
							{
								if (cts.IsCancellationRequested)
								{
									source.SetCanceled();
								}
								else
								{
									source.SetResult(null);
								}
							}
						}
					}, this._connection.GetOpenTdsConnection(), null, null, null, null);
				}
				else
				{
					this._localColumnMappings = null;
					try
					{
						this.CleanUpStateObjectOnError();
					}
					catch (Exception)
					{
					}
					if (source != null)
					{
						source.SetResult(null);
					}
				}
			}
			catch (Exception ex)
			{
				this._localColumnMappings = null;
				try
				{
					this.CleanUpStateObjectOnError();
				}
				catch (Exception)
				{
				}
				if (source == null)
				{
					throw;
				}
				source.TrySetException(ex);
			}
		}

		private void WriteToServerInternalRestAsync(CancellationToken cts, TaskCompletionSource<object> source)
		{
			this._hasMoreRowToCopy = true;
			Task<BulkCopySimpleResultSet> internalResultsTask = null;
			BulkCopySimpleResultSet bulkCopySimpleResultSet = new BulkCopySimpleResultSet();
			SqlInternalConnectionTds openTdsConnection = this._connection.GetOpenTdsConnection();
			try
			{
				this._parser = this._connection.Parser;
				this._parser._asyncWrite = this._isAsyncBulkCopy;
				Task task;
				try
				{
					task = this._connection.ValidateAndReconnect(delegate
					{
						if (this._parserLock != null)
						{
							this._parserLock.Release();
							this._parserLock = null;
						}
					}, this.BulkCopyTimeout);
				}
				catch (SqlException ex)
				{
					SqlException ex5;
					throw SQL.BulkLoadInvalidDestinationTable(this._destinationTableName, ex5);
				}
				if (task != null)
				{
					if (this._isAsyncBulkCopy)
					{
						CancellationTokenRegistration regReconnectCancel = default(CancellationTokenRegistration);
						TaskCompletionSource<object> cancellableReconnectTS = new TaskCompletionSource<object>();
						if (cts.CanBeCanceled)
						{
							regReconnectCancel = cts.Register(delegate(object s)
							{
								((TaskCompletionSource<object>)s).TrySetCanceled();
							}, cancellableReconnectTS);
						}
						AsyncHelper.ContinueTask(task, cancellableReconnectTS, delegate
						{
							cancellableReconnectTS.SetResult(null);
						}, null, null, null, null, null);
						AsyncHelper.SetTimeoutException(cancellableReconnectTS, this.BulkCopyTimeout, () => SQL.BulkLoadInvalidDestinationTable(this._destinationTableName, SQL.CR_ReconnectTimeout()), CancellationToken.None);
						AsyncHelper.ContinueTask(cancellableReconnectTS.Task, source, delegate
						{
							regReconnectCancel.Dispose();
							if (this._parserLock != null)
							{
								this._parserLock.Release();
								this._parserLock = null;
							}
							this._parserLock = this._connection.GetOpenTdsConnection()._parserLock;
							this._parserLock.Wait(true);
							this.WriteToServerInternalRestAsync(cts, source);
						}, null, delegate(Exception e)
						{
							regReconnectCancel.Dispose();
						}, delegate
						{
							regReconnectCancel.Dispose();
						}, (Exception ex) => SQL.BulkLoadInvalidDestinationTable(this._destinationTableName, ex), this._connection);
					}
					else
					{
						try
						{
							AsyncHelper.WaitForCompletion(task, this.BulkCopyTimeout, delegate
							{
								throw SQL.CR_ReconnectTimeout();
							}, true);
						}
						catch (SqlException ex2)
						{
							throw SQL.BulkLoadInvalidDestinationTable(this._destinationTableName, ex2);
						}
						this._parserLock = this._connection.GetOpenTdsConnection()._parserLock;
						this._parserLock.Wait(false);
						this.WriteToServerInternalRestAsync(cts, source);
					}
				}
				else
				{
					if (this._isAsyncBulkCopy)
					{
						this._connection.AddWeakReference(this, 3);
					}
					openTdsConnection.ThreadHasParserLockForClose = true;
					try
					{
						this._stateObj = this._parser.GetSession(this);
						this._stateObj._bulkCopyOpperationInProgress = true;
						this._stateObj.StartSession(this);
					}
					finally
					{
						openTdsConnection.ThreadHasParserLockForClose = false;
					}
					try
					{
						internalResultsTask = this.CreateAndExecuteInitialQueryAsync(out bulkCopySimpleResultSet);
					}
					catch (SqlException ex3)
					{
						throw SQL.BulkLoadInvalidDestinationTable(this._destinationTableName, ex3);
					}
					if (internalResultsTask != null)
					{
						AsyncHelper.ContinueTask(internalResultsTask, source, delegate
						{
							this.WriteToServerInternalRestContinuedAsync(internalResultsTask.Result, cts, source);
						}, this._connection.GetOpenTdsConnection(), null, null, null, null);
					}
					else
					{
						this.WriteToServerInternalRestContinuedAsync(bulkCopySimpleResultSet, cts, source);
					}
				}
			}
			catch (Exception ex4)
			{
				if (source == null)
				{
					throw;
				}
				source.TrySetException(ex4);
			}
		}

		private Task WriteToServerInternalAsync(CancellationToken ctoken)
		{
			TaskCompletionSource<object> source = null;
			Task<object> task = null;
			if (this._isAsyncBulkCopy)
			{
				source = new TaskCompletionSource<object>();
				task = source.Task;
				this.RegisterForConnectionCloseNotification<object>(ref task);
			}
			if (this._destinationTableName != null)
			{
				try
				{
					Task task2 = this.ReadFromRowSourceAsync(ctoken);
					if (task2 != null)
					{
						AsyncHelper.ContinueTask(task2, source, delegate
						{
							if (!this._hasMoreRowToCopy)
							{
								source.SetResult(null);
								return;
							}
							this.WriteToServerInternalRestAsync(ctoken, source);
						}, this._connection.GetOpenTdsConnection(), null, null, null, null);
						return task;
					}
					if (!this._hasMoreRowToCopy)
					{
						if (source != null)
						{
							source.SetResult(null);
						}
						return task;
					}
					this.WriteToServerInternalRestAsync(ctoken, source);
					return task;
				}
				catch (Exception ex)
				{
					if (source == null)
					{
						throw;
					}
					source.TrySetException(ex);
				}
				return task;
			}
			if (source != null)
			{
				source.SetException(SQL.BulkLoadMissingDestinationTable());
				return task;
			}
			throw SQL.BulkLoadMissingDestinationTable();
		}

		private const int MetaDataResultId = 1;

		private const int CollationResultId = 2;

		private const int CollationId = 3;

		private const int MAX_LENGTH = 2147483647;

		private const int DefaultCommandTimeout = 30;

		private bool _enableStreaming;

		private int _batchSize;

		private bool _ownConnection;

		private SqlBulkCopyOptions _copyOptions;

		private int _timeout = 30;

		private string _destinationTableName;

		private int _rowsCopied;

		private int _notifyAfter;

		private int _rowsUntilNotification;

		private bool _insideRowsCopiedEvent;

		private object _rowSource;

		private SqlDataReader _SqlDataReaderRowSource;

		private DbDataReader _DbDataReaderRowSource;

		private DataTable _dataTableSource;

		private SqlBulkCopyColumnMappingCollection _columnMappings;

		private SqlBulkCopyColumnMappingCollection _localColumnMappings;

		private SqlConnection _connection;

		private SqlTransaction _internalTransaction;

		private SqlTransaction _externalTransaction;

		private SqlBulkCopy.ValueSourceType _rowSourceType;

		private DataRow _currentRow;

		private int _currentRowLength;

		private DataRowState _rowStateToSkip;

		private IEnumerator _rowEnumerator;

		private TdsParser _parser;

		private TdsParserStateObject _stateObj;

		private List<_ColumnMapping> _sortedColumnMappings;

		private SqlRowsCopiedEventHandler _rowsCopiedEventHandler;

		private int _savedBatchSize;

		private bool _hasMoreRowToCopy;

		private bool _isAsyncBulkCopy;

		private bool _isBulkCopyingInProgress;

		private SqlInternalConnectionTds.SyncAsyncLock _parserLock;

		private SqlBulkCopy.SourceColumnMetadata[] _currentRowMetadata;

		private enum ValueSourceType
		{
			Unspecified,
			IDataReader,
			DataTable,
			RowArray,
			DbDataReader
		}

		private enum ValueMethod : byte
		{
			GetValue,
			SqlTypeSqlDecimal,
			SqlTypeSqlDouble,
			SqlTypeSqlSingle,
			DataFeedStream,
			DataFeedText,
			DataFeedXml
		}

		private struct SourceColumnMetadata
		{
			public SourceColumnMetadata(SqlBulkCopy.ValueMethod method, bool isSqlType, bool isDataFeed)
			{
				this.Method = method;
				this.IsSqlType = isSqlType;
				this.IsDataFeed = isDataFeed;
			}

			public readonly SqlBulkCopy.ValueMethod Method;

			public readonly bool IsSqlType;

			public readonly bool IsDataFeed;
		}
	}
}
