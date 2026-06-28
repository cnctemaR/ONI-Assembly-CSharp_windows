using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Mono.Security.Protocol.Ntlm;

namespace Mono.Data.Tds.Protocol
{
	public abstract class Tds
	{
		public Tds(string dataSource, int port, int packetSize, int timeout, TdsVersion tdsVersion)
		{
			this.tdsVersion = tdsVersion;
			this.packetSize = packetSize;
			this.dataSource = dataSource;
			this.columns = new TdsDataColumnCollection();
			this.comm = new TdsComm(dataSource, port, packetSize, timeout, tdsVersion);
		}

		public event TdsInternalErrorMessageEventHandler TdsErrorMessage;

		public event TdsInternalInfoMessageEventHandler TdsInfoMessage;

		protected string Charset
		{
			get
			{
				return this.charset;
			}
		}

		protected CultureInfo Locale
		{
			get
			{
				return this.locale;
			}
		}

		public bool DoneProc
		{
			get
			{
				return this.doneProc;
			}
		}

		protected string Language
		{
			get
			{
				return this.language;
			}
		}

		protected ArrayList ColumnNames
		{
			get
			{
				return this.columnNames;
			}
		}

		public TdsDataRow ColumnValues
		{
			get
			{
				return this.currentRow;
			}
		}

		internal TdsComm Comm
		{
			get
			{
				return this.comm;
			}
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		public string DataSource
		{
			get
			{
				return this.dataSource;
			}
		}

		public bool IsConnected
		{
			get
			{
				return this.connected && this.comm != null && this.comm.IsConnected();
			}
			set
			{
				this.connected = value;
			}
		}

		public bool Pooling
		{
			get
			{
				return this.pooling;
			}
			set
			{
				this.pooling = value;
			}
		}

		public bool MoreResults
		{
			get
			{
				return this.moreResults;
			}
			set
			{
				this.moreResults = value;
			}
		}

		public int PacketSize
		{
			get
			{
				return this.packetSize;
			}
		}

		public int RecordsAffected
		{
			get
			{
				return this.recordsAffected;
			}
			set
			{
				this.recordsAffected = value;
			}
		}

		public string ServerVersion
		{
			get
			{
				return this.databaseProductVersion;
			}
		}

		public TdsDataColumnCollection Columns
		{
			get
			{
				return this.columns;
			}
		}

		public TdsVersion TdsVersion
		{
			get
			{
				return this.tdsVersion;
			}
		}

		public ArrayList OutputParameters
		{
			get
			{
				return this.outputParameters;
			}
			set
			{
				this.outputParameters = value;
			}
		}

		protected TdsMetaParameterCollection Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = value;
			}
		}

		public bool SequentialAccess
		{
			get
			{
				return this.sequentialAccess;
			}
			set
			{
				this.sequentialAccess = value;
			}
		}

		public byte[] Collation
		{
			get
			{
				return this.collation;
			}
		}

		public TdsVersion ServerTdsVersion
		{
			get
			{
				switch (this.databaseMajorVersion)
				{
				case 4:
					return TdsVersion.tds42;
				case 5:
					return TdsVersion.tds50;
				case 7:
					return TdsVersion.tds70;
				case 8:
					return TdsVersion.tds80;
				case 9:
					return TdsVersion.tds90;
				case 10:
					return TdsVersion.tds100;
				}
				return this.tdsVersion;
			}
		}

		private void SkipRow()
		{
			this.SkipToColumnIndex(this.Columns.Count);
			this.StreamLength = 0L;
			this.StreamColumnIndex = 0;
			this.StreamIndex = 0L;
			this.LoadInProgress = false;
		}

		private void SkipToColumnIndex(int colIndex)
		{
			if (this.LoadInProgress)
			{
				this.EndLoad();
			}
			if (colIndex < this.StreamColumnIndex)
			{
				throw new Exception("Cannot Skip to a colindex less than the curr index");
			}
			while (colIndex != this.StreamColumnIndex)
			{
				TdsColumnType? columnType = this.Columns[this.StreamColumnIndex].ColumnType;
				if (columnType == null)
				{
					throw new Exception("Column type unset.");
				}
				if (!(columnType == TdsColumnType.Image) && !(columnType == TdsColumnType.Text) && !(columnType == TdsColumnType.NText))
				{
					this.GetColumnValue(columnType, false, this.StreamColumnIndex);
					this.StreamColumnIndex++;
				}
				else
				{
					this.BeginLoad(columnType);
					this.Comm.Skip(this.StreamLength);
					this.StreamLength = 0L;
					this.EndLoad();
				}
			}
		}

		public object GetSequentialColumnValue(int colIndex)
		{
			if (colIndex < this.StreamColumnIndex)
			{
				throw new InvalidOperationException("Invalid attempt tp read from column ordinal" + colIndex);
			}
			if (this.LoadInProgress)
			{
				this.EndLoad();
			}
			if (colIndex != this.StreamColumnIndex)
			{
				this.SkipToColumnIndex(colIndex);
			}
			object columnValue = this.GetColumnValue(this.Columns[colIndex].ColumnType, false, colIndex);
			this.StreamColumnIndex++;
			return columnValue;
		}

		public long GetSequentialColumnValue(int colIndex, long fieldIndex, byte[] buffer, int bufferIndex, int size)
		{
			if (colIndex < this.StreamColumnIndex)
			{
				throw new InvalidOperationException("Invalid attempt to read from column ordinal" + colIndex);
			}
			long num;
			try
			{
				if (colIndex != this.StreamColumnIndex)
				{
					this.SkipToColumnIndex(colIndex);
				}
				if (!this.LoadInProgress)
				{
					this.BeginLoad(this.Columns[colIndex].ColumnType);
				}
				if (buffer == null)
				{
					num = this.StreamLength;
				}
				else
				{
					num = this.LoadData(fieldIndex, buffer, bufferIndex, size);
				}
			}
			catch (IOException ex)
			{
				this.connected = false;
				throw new TdsInternalException("Server closed the connection.", ex);
			}
			return num;
		}

		private void BeginLoad(TdsColumnType? colType)
		{
			if (this.LoadInProgress)
			{
				this.EndLoad();
			}
			this.StreamLength = 0L;
			if (colType == null)
			{
				throw new ArgumentNullException("colType");
			}
			if (colType != null)
			{
				TdsColumnType value = colType.Value;
				switch (value)
				{
				case TdsColumnType.Image:
				case TdsColumnType.Text:
					break;
				default:
					switch (value)
					{
					case TdsColumnType.Binary:
					case TdsColumnType.Char:
						goto IL_0133;
					default:
						switch (value)
						{
						case TdsColumnType.BigVarBinary:
						case TdsColumnType.BigVarChar:
							break;
						default:
							switch (value)
							{
							case TdsColumnType.BigBinary:
							case TdsColumnType.BigChar:
								break;
							default:
								if (value == TdsColumnType.NText)
								{
									goto IL_00CD;
								}
								if (value != TdsColumnType.NVarChar && value != TdsColumnType.NChar)
								{
									goto IL_014A;
								}
								goto IL_0133;
							}
							break;
						}
						this.Comm.GetTdsShort();
						this.StreamLength = (long)this.Comm.GetTdsShort();
						goto IL_0157;
					}
					break;
				case TdsColumnType.VarBinary:
				case TdsColumnType.VarChar:
					goto IL_0133;
				}
				IL_00CD:
				if (this.Comm.GetByte() != 0)
				{
					this.Comm.Skip(24L);
					this.StreamLength = (long)this.Comm.GetTdsInt();
				}
				else
				{
					this.StreamLength = -2L;
				}
				goto IL_0157;
				IL_0133:
				this.StreamLength = (long)this.Comm.GetTdsShort();
				goto IL_0157;
			}
			IL_014A:
			this.StreamLength = -1L;
			IL_0157:
			this.StreamIndex = 0L;
			this.LoadInProgress = true;
		}

		private void EndLoad()
		{
			if (this.StreamLength > 0L)
			{
				this.Comm.Skip(this.StreamLength);
			}
			this.StreamLength = 0L;
			this.StreamIndex = 0L;
			this.StreamColumnIndex++;
			this.LoadInProgress = false;
		}

		private long LoadData(long fieldIndex, byte[] buffer, int bufferIndex, int size)
		{
			if (this.StreamLength <= 0L)
			{
				return this.StreamLength;
			}
			if (fieldIndex < this.StreamIndex)
			{
				throw new InvalidOperationException(string.Format("Attempting to read at dataIndex '{0}' is not allowed as this is less than the current position. You must read from dataIndex '{1}' or greater.", fieldIndex, this.StreamIndex));
			}
			if (fieldIndex >= this.StreamLength + this.StreamIndex)
			{
				return 0L;
			}
			int num = (int)(fieldIndex - this.StreamIndex);
			this.Comm.Skip((long)num);
			this.StreamIndex += fieldIndex - this.StreamIndex;
			this.StreamLength -= (long)num;
			int num2 = (int)(((long)size <= this.StreamLength) ? ((long)size) : this.StreamLength);
			byte[] bytes = this.Comm.GetBytes(num2, true);
			this.StreamIndex += (long)num2 + (fieldIndex - this.StreamIndex);
			this.StreamLength -= (long)num2;
			bytes.CopyTo(buffer, bufferIndex);
			return (long)bytes.Length;
		}

		protected internal void InitExec()
		{
			this.moreResults = true;
			this.doneProc = false;
			this.isResultRead = false;
			this.isRowRead = false;
			this.StreamLength = 0L;
			this.StreamIndex = 0L;
			this.StreamColumnIndex = 0;
			this.LoadInProgress = false;
			this.queryInProgress = false;
			this.cancelsRequested = 0;
			this.cancelsProcessed = 0;
			this.recordsAffected = -1;
			this.messages.Clear();
			this.outputParameters.Clear();
		}

		public void Cancel()
		{
			if (this.queryInProgress && this.cancelsRequested == this.cancelsProcessed)
			{
				this.comm.StartPacket(TdsPacketType.Cancel);
				try
				{
					this.Comm.SendPacket();
				}
				catch (IOException ex)
				{
					this.connected = false;
					throw new TdsInternalException("Server closed the connection.", ex);
				}
				this.cancelsRequested++;
			}
		}

		public abstract bool Connect(TdsConnectionParameters connectionParameters);

		public static TdsTimeoutException CreateTimeoutException(string dataSource, string method)
		{
			string text = "Timeout expired. The timeout period elapsed prior to completion of the operation or the server is not responding.";
			return new TdsTimeoutException(0, 0, text, -2, method, dataSource, "Mono TdsClient Data Provider", 0);
		}

		public void Disconnect()
		{
			try
			{
				this.comm.StartPacket(TdsPacketType.Logoff);
				this.comm.Append(0);
				this.comm.SendPacket();
			}
			catch
			{
			}
			this.connected = false;
			this.comm.Close();
		}

		public virtual bool Reset()
		{
			this.database = this.originalDatabase;
			return true;
		}

		protected virtual bool IsValidRowCount(byte status, byte op)
		{
			return (status & 16) != 0;
		}

		public void Execute(string sql)
		{
			this.Execute(sql, null, 0, false);
		}

		public void ExecProc(string sql)
		{
			this.ExecProc(sql, null, 0, false);
		}

		public virtual void Execute(string sql, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			this.ExecuteQuery(sql, timeout, wantResults);
		}

		public virtual void ExecProc(string sql, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			this.ExecuteQuery(string.Format("exec {0}", sql), timeout, wantResults);
		}

		public virtual void ExecPrepared(string sql, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			throw new NotSupportedException();
		}

		internal void ExecBulkCopyMetaData(int timeout, bool wantResults)
		{
			this.moreResults = true;
			try
			{
				this.Comm.SendPacket();
				this.CheckForData(timeout);
				if (!wantResults)
				{
					this.SkipToEnd();
				}
			}
			catch (IOException ex)
			{
				this.connected = false;
				throw new TdsInternalException("Server closed the connection.", ex);
			}
		}

		internal void ExecBulkCopy(int timeout, bool wantResults)
		{
			this.moreResults = true;
			try
			{
				this.Comm.SendPacket();
				this.CheckForData(timeout);
				if (!wantResults)
				{
					this.SkipToEnd();
				}
			}
			catch (IOException ex)
			{
				this.connected = false;
				throw new TdsInternalException("Server closed the connection.", ex);
			}
		}

		protected void ExecuteQuery(string sql, int timeout, bool wantResults)
		{
			this.InitExec();
			this.Comm.StartPacket(TdsPacketType.Query);
			this.Comm.Append(sql);
			try
			{
				this.Comm.SendPacket();
				this.CheckForData(timeout);
				if (!wantResults)
				{
					this.SkipToEnd();
				}
			}
			catch (IOException ex)
			{
				this.connected = false;
				throw new TdsInternalException("Server closed the connection.", ex);
			}
		}

		protected virtual void ExecRPC(string rpcName, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			this.Comm.StartPacket(TdsPacketType.DBRPC);
			byte[] bytes = this.Comm.Encoder.GetBytes(rpcName);
			byte b = (byte)bytes.Length;
			ushort num = 0;
			ushort num2 = (ushort)(1 + b + 2);
			this.Comm.Append(num2);
			this.Comm.Append(b);
			this.Comm.Append(bytes);
			this.Comm.Append(num);
			try
			{
				this.Comm.SendPacket();
				this.CheckForData(timeout);
				if (!wantResults)
				{
					this.SkipToEnd();
				}
			}
			catch (IOException ex)
			{
				this.connected = false;
				throw new TdsInternalException("Server closed the connection.", ex);
			}
		}

		public bool NextResult()
		{
			if (this.SequentialAccess && this.isResultRead)
			{
				while (this.NextRow())
				{
				}
				this.isRowRead = false;
				this.isResultRead = false;
			}
			if (!this.moreResults)
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			while (!flag)
			{
				TdsPacketSubType tdsPacketSubType = this.ProcessSubPacket();
				if (flag2)
				{
					this.moreResults = false;
					break;
				}
				TdsPacketSubType tdsPacketSubType2 = tdsPacketSubType;
				byte b;
				switch (tdsPacketSubType2)
				{
				case TdsPacketSubType.ColumnInfo:
					break;
				default:
					if (tdsPacketSubType2 != TdsPacketSubType.ColumnMetadata && tdsPacketSubType2 != TdsPacketSubType.RowFormat)
					{
						flag = !this.moreResults;
						continue;
					}
					break;
				case TdsPacketSubType.TableName:
					b = this.Comm.Peek();
					flag = b != 165;
					continue;
				case TdsPacketSubType.ColumnDetail:
					flag = true;
					continue;
				}
				b = this.Comm.Peek();
				flag = b != 164;
				if (flag && this.doneProc && b == 209)
				{
					flag2 = true;
					flag = false;
				}
			}
			return this.moreResults;
		}

		public bool NextRow()
		{
			if (this.SequentialAccess && this.isRowRead)
			{
				this.SkipRow();
				this.isRowRead = false;
			}
			bool flag = false;
			bool flag2 = false;
			do
			{
				TdsPacketSubType tdsPacketSubType = this.ProcessSubPacket();
				TdsPacketSubType tdsPacketSubType2 = tdsPacketSubType;
				switch (tdsPacketSubType2)
				{
				case TdsPacketSubType.Done:
				case TdsPacketSubType.DoneProc:
				case TdsPacketSubType.DoneInProc:
					flag2 = false;
					flag = true;
					break;
				default:
					if (tdsPacketSubType2 == TdsPacketSubType.Row)
					{
						flag2 = true;
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return flag2;
		}

		public virtual string Prepare(string sql, TdsMetaParameterCollection parameters)
		{
			throw new NotSupportedException();
		}

		public void SkipToEnd()
		{
			try
			{
				while (this.NextResult())
				{
				}
			}
			catch (IOException ex)
			{
				this.connected = false;
				throw new TdsInternalException("Server closed the connection.", ex);
			}
		}

		public virtual void Unprepare(string statementId)
		{
			throw new NotSupportedException();
		}

		[MonoTODO("Is cancel enough, or do we need to drop the connection?")]
		protected void CheckForData(int timeout)
		{
			if (timeout > 0 && !this.comm.Poll(timeout, SelectMode.SelectRead))
			{
				this.Cancel();
				throw Tds.CreateTimeoutException(this.dataSource, "CheckForData()");
			}
		}

		protected TdsInternalInfoMessageEventArgs CreateTdsInfoMessageEvent(TdsInternalErrorCollection errors)
		{
			return new TdsInternalInfoMessageEventArgs(errors);
		}

		protected TdsInternalErrorMessageEventArgs CreateTdsErrorMessageEvent(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
		{
			return new TdsInternalErrorMessageEventArgs(new TdsInternalError(theClass, lineNumber, message, number, procedure, server, source, state));
		}

		private Encoding GetEncodingFromColumnCollation(int lcid, int sortId)
		{
			if (sortId != 0)
			{
				return TdsCharset.GetEncodingFromSortOrder(sortId);
			}
			return TdsCharset.GetEncodingFromLCID(lcid);
		}

		protected object GetColumnValue(TdsColumnType? colType, bool outParam)
		{
			return this.GetColumnValue(colType, outParam, -1);
		}

		private object GetColumnValue(TdsColumnType? colType, bool outParam, int ordinal)
		{
			object obj = null;
			int num = 0;
			int num2 = 0;
			if (colType == null)
			{
				throw new ArgumentNullException("colType");
			}
			if (ordinal > -1 && this.tdsVersion > TdsVersion.tds70)
			{
				num = this.columns[ordinal].LCID.Value;
				num2 = this.columns[ordinal].SortOrder.Value;
			}
			if (colType != null)
			{
				TdsColumnType value = colType.Value;
				switch (value)
				{
				case TdsColumnType.Image:
					if (outParam)
					{
						this.comm.Skip(1L);
					}
					return this.GetImageValue();
				case TdsColumnType.Text:
				{
					Encoding encoding = this.GetEncodingFromColumnCollation(num, num2);
					if (outParam)
					{
						this.comm.Skip(1L);
					}
					return this.GetTextValue(false, encoding);
				}
				case TdsColumnType.UniqueIdentifier:
				{
					if (this.comm.Peek() != 16)
					{
						this.comm.GetByte();
						return DBNull.Value;
					}
					if (outParam)
					{
						this.comm.Skip(1L);
					}
					int num3 = (int)(this.comm.GetByte() & byte.MaxValue);
					if (num3 > 0)
					{
						byte[] bytes = this.comm.GetBytes(num3, true);
						if (!BitConverter.IsLittleEndian)
						{
							byte[] array = new byte[num3];
							for (int i = 0; i < 4; i++)
							{
								array[i] = bytes[4 - i - 1];
							}
							for (int j = 4; j < 6; j++)
							{
								array[j] = bytes[6 - (j - 4) - 1];
							}
							for (int k = 6; k < 8; k++)
							{
								array[k] = bytes[8 - (k - 6) - 1];
							}
							for (int l = 8; l < 16; l++)
							{
								array[l] = bytes[l];
							}
							Array.Copy(array, 0, bytes, 0, num3);
						}
						obj = new Guid(bytes);
					}
					return obj;
				}
				case TdsColumnType.VarBinary:
				case TdsColumnType.Binary:
					if (outParam)
					{
						this.comm.Skip(1L);
					}
					return this.GetBinaryValue();
				case TdsColumnType.IntN:
					if (outParam)
					{
						this.comm.Skip(1L);
					}
					return this.GetIntValue(colType);
				case TdsColumnType.VarChar:
				case TdsColumnType.Char:
				{
					Encoding encoding = this.GetEncodingFromColumnCollation(num, num2);
					if (outParam)
					{
						this.comm.Skip(1L);
					}
					return this.GetStringValue(colType, false, outParam, encoding);
				}
				default:
					switch (value)
					{
					case TdsColumnType.NText:
					{
						Encoding encoding = this.GetEncodingFromColumnCollation(num, num2);
						if (outParam)
						{
							this.comm.Skip(1L);
						}
						return this.GetTextValue(true, encoding);
					}
					default:
					{
						Encoding encoding;
						switch (value)
						{
						case TdsColumnType.BigVarBinary:
						{
							if (outParam)
							{
								this.comm.Skip(1L);
							}
							int num3 = (int)this.comm.GetTdsShort();
							return this.comm.GetBytes(num3, true);
						}
						default:
							switch (value)
							{
							case TdsColumnType.BigBinary:
								if (outParam)
								{
									this.comm.Skip(2L);
								}
								return this.GetBinaryValue();
							default:
								if (value == TdsColumnType.SmallMoney)
								{
									goto IL_0374;
								}
								if (value == TdsColumnType.BigInt)
								{
									goto IL_01C0;
								}
								if (value != TdsColumnType.BigNVarChar && value != TdsColumnType.NChar)
								{
									goto IL_062C;
								}
								encoding = this.GetEncodingFromColumnCollation(num, num2);
								if (outParam)
								{
									this.comm.Skip(2L);
								}
								return this.GetStringValue(colType, true, outParam, encoding);
							case TdsColumnType.BigChar:
								break;
							}
							break;
						case TdsColumnType.BigVarChar:
							break;
						}
						encoding = this.GetEncodingFromColumnCollation(num, num2);
						if (outParam)
						{
							this.comm.Skip(2L);
						}
						return this.GetStringValue(colType, false, outParam, encoding);
					}
					case TdsColumnType.NVarChar:
					{
						Encoding encoding = this.GetEncodingFromColumnCollation(num, num2);
						if (outParam)
						{
							this.comm.Skip(1L);
						}
						return this.GetStringValue(colType, true, outParam, encoding);
					}
					case TdsColumnType.BitN:
						if (outParam)
						{
							this.comm.Skip(1L);
						}
						if (this.comm.GetByte() == 0)
						{
							obj = DBNull.Value;
						}
						else
						{
							obj = this.comm.GetByte() != 0;
						}
						return obj;
					case TdsColumnType.Decimal:
					case TdsColumnType.Numeric:
					{
						byte b;
						byte b2;
						if (outParam)
						{
							this.comm.Skip(1L);
							b = this.comm.GetByte();
							b2 = this.comm.GetByte();
						}
						else
						{
							b = (byte)this.columns[ordinal].NumericPrecision.Value;
							b2 = (byte)this.columns[ordinal].NumericScale.Value;
						}
						obj = this.GetDecimalValue(b, b2);
						if (b2 == 0 && b <= 19 && this.tdsVersion == TdsVersion.tds70 && !(obj is DBNull))
						{
							obj = Convert.ToInt64(obj);
						}
						return obj;
					}
					case TdsColumnType.FloatN:
						if (outParam)
						{
							this.comm.Skip(1L);
						}
						return this.GetFloatValue(colType);
					case TdsColumnType.MoneyN:
						if (outParam)
						{
							this.comm.Skip(1L);
						}
						return this.GetMoneyValue(colType);
					case TdsColumnType.DateTimeN:
						if (outParam)
						{
							this.comm.Skip(1L);
						}
						return this.GetDateTimeValue(colType);
					}
					break;
				case TdsColumnType.Int1:
				case TdsColumnType.Int2:
				case TdsColumnType.Int4:
					break;
				case TdsColumnType.Bit:
				{
					int @byte = (int)this.comm.GetByte();
					return @byte != 0;
				}
				case TdsColumnType.DateTime4:
				case TdsColumnType.DateTime:
					return this.GetDateTimeValue(colType);
				case TdsColumnType.Real:
				case TdsColumnType.Float8:
					return this.GetFloatValue(colType);
				case TdsColumnType.Money:
					goto IL_0374;
				}
				IL_01C0:
				return this.GetIntValue(colType);
				IL_0374:
				obj = this.GetMoneyValue(colType);
				return obj;
			}
			IL_062C:
			return DBNull.Value;
		}

		private object GetBinaryValue()
		{
			object obj = DBNull.Value;
			if (this.tdsVersion >= TdsVersion.tds70)
			{
				int num = (int)this.comm.GetTdsShort();
				if (num != 65535 && num >= 0)
				{
					obj = this.comm.GetBytes(num, true);
				}
			}
			else
			{
				int num = (int)(this.comm.GetByte() & byte.MaxValue);
				if (num != 0)
				{
					obj = this.comm.GetBytes(num, true);
				}
			}
			return obj;
		}

		private object GetDateTimeValue(TdsColumnType? type)
		{
			int num = 0;
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type != null)
			{
				TdsColumnType value = type.Value;
				switch (value)
				{
				case TdsColumnType.DateTime4:
					num = 4;
					break;
				default:
					if (value == TdsColumnType.DateTimeN)
					{
						byte b = this.comm.Peek();
						if (b == 0 || b == 4 || b == 8)
						{
							num = (int)this.comm.GetByte();
						}
					}
					break;
				case TdsColumnType.DateTime:
					num = 8;
					break;
				}
			}
			DateTime dateTime = new DateTime(1900, 1, 1);
			int num2 = num;
			object obj;
			if (num2 != 4)
			{
				if (num2 != 8)
				{
					obj = DBNull.Value;
				}
				else
				{
					obj = dateTime.AddDays((double)this.comm.GetTdsInt());
					int tdsInt = this.comm.GetTdsInt();
					long num3 = (long)Math.Round((double)((float)((long)tdsInt % 300L * 1000L) / 300f));
					if (tdsInt != 0 || num3 != 0L)
					{
						obj = ((DateTime)obj).AddSeconds((double)(tdsInt / 300));
						obj = ((DateTime)obj).AddMilliseconds((double)num3);
					}
				}
			}
			else
			{
				obj = dateTime.AddDays((double)((ushort)this.comm.GetTdsShort()));
				short tdsShort = this.comm.GetTdsShort();
				if (tdsShort != 0)
				{
					obj = ((DateTime)obj).AddMinutes((double)tdsShort);
				}
			}
			return obj;
		}

		private object GetDecimalValue(byte precision, byte scale)
		{
			if (this.tdsVersion < TdsVersion.tds70)
			{
				return this.GetDecimalValueTds50(precision, scale);
			}
			return this.GetDecimalValueTds70(precision, scale);
		}

		private object GetDecimalValueTds70(byte precision, byte scale)
		{
			int[] array = new int[4];
			int num = (int)((this.comm.GetByte() & byte.MaxValue) - 1);
			if (num < 0)
			{
				return DBNull.Value;
			}
			bool flag = this.comm.GetByte() == 1;
			if (num > 16)
			{
				throw new OverflowException();
			}
			int num2 = 0;
			int num3 = 0;
			while (num2 < num && num2 < 16)
			{
				array[num3] = this.comm.GetTdsInt();
				num2 += 4;
				num3++;
			}
			if (array[3] != 0)
			{
				return new TdsBigDecimal(precision, scale, !flag, array);
			}
			return new decimal(array[0], array[1], array[2], !flag, scale);
		}

		private object GetDecimalValueTds50(byte precision, byte scale)
		{
			int[] array = new int[4];
			int num = (int)(this.comm.GetByte() & byte.MaxValue);
			if (num == 0)
			{
				return DBNull.Value;
			}
			byte[] bytes = this.comm.GetBytes(num, false);
			byte[] array2 = new byte[4];
			bool flag = bytes[0] == 1;
			if (num > 17)
			{
				throw new OverflowException();
			}
			int num2 = 1;
			int num3 = 0;
			while (num2 < num && num2 < 16)
			{
				for (int i = 0; i < 4; i++)
				{
					if (num2 + i < num)
					{
						array2[i] = bytes[num - (num2 + i)];
					}
					else
					{
						array2[i] = 0;
					}
				}
				if (!BitConverter.IsLittleEndian)
				{
					array2 = this.comm.Swap(array2);
				}
				array[num3] = BitConverter.ToInt32(array2, 0);
				num2 += 4;
				num3++;
			}
			if (array[3] != 0)
			{
				return new TdsBigDecimal(precision, scale, flag, array);
			}
			return new decimal(array[0], array[1], array[2], flag, scale);
		}

		private object GetFloatValue(TdsColumnType? columnType)
		{
			if (columnType == null)
			{
				throw new ArgumentNullException("columnType");
			}
			int num = 0;
			if (columnType != null)
			{
				TdsColumnType value = columnType.Value;
				switch (value)
				{
				case TdsColumnType.Real:
					num = 4;
					break;
				default:
					if (value == TdsColumnType.FloatN)
					{
						num = (int)this.comm.GetByte();
					}
					break;
				case TdsColumnType.Float8:
					num = 8;
					break;
				}
			}
			int num2 = num;
			if (num2 == 4)
			{
				return BitConverter.ToSingle(BitConverter.GetBytes(this.comm.GetTdsInt()), 0);
			}
			if (num2 != 8)
			{
				return DBNull.Value;
			}
			return BitConverter.Int64BitsToDouble(this.comm.GetTdsInt64());
		}

		private object GetImageValue()
		{
			if (this.comm.GetByte() == 0)
			{
				return DBNull.Value;
			}
			this.comm.Skip(24L);
			int tdsInt = this.comm.GetTdsInt();
			if (tdsInt < 0)
			{
				return DBNull.Value;
			}
			return this.comm.GetBytes(tdsInt, true);
		}

		private object GetIntValue(TdsColumnType? type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type != null)
			{
				TdsColumnType value = type.Value;
				int num;
				if (value != TdsColumnType.IntN)
				{
					if (value != TdsColumnType.Int1)
					{
						if (value != TdsColumnType.Int2)
						{
							if (value != TdsColumnType.Int4)
							{
								if (value != TdsColumnType.BigInt)
								{
									goto IL_0088;
								}
								num = 8;
							}
							else
							{
								num = 4;
							}
						}
						else
						{
							num = 2;
						}
					}
					else
					{
						num = 1;
					}
				}
				else
				{
					num = (int)this.comm.GetByte();
				}
				switch (num)
				{
				case 1:
					return this.comm.GetByte();
				case 2:
					return this.comm.GetTdsShort();
				case 4:
					return this.comm.GetTdsInt();
				case 8:
					return this.comm.GetTdsInt64();
				}
				return DBNull.Value;
			}
			IL_0088:
			return DBNull.Value;
		}

		private object GetMoneyValue(TdsColumnType? type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type != null)
			{
				TdsColumnType value = type.Value;
				int num;
				switch (value)
				{
				case TdsColumnType.MoneyN:
					num = (int)this.comm.GetByte();
					goto IL_0081;
				default:
					if (value == TdsColumnType.Money)
					{
						num = 8;
						goto IL_0081;
					}
					if (value != TdsColumnType.SmallMoney)
					{
						goto IL_007B;
					}
					break;
				case TdsColumnType.Money4:
					break;
				}
				num = 4;
				IL_0081:
				int num2 = num;
				if (num2 == 4)
				{
					int num3 = this.Comm.GetTdsInt();
					bool flag = num3 < 0;
					if (flag)
					{
						num3 = ~(num3 - 1);
					}
					return new decimal(num3, 0, 0, flag, 4);
				}
				if (num2 != 8)
				{
					return DBNull.Value;
				}
				int num4 = this.Comm.GetTdsInt();
				int num5 = this.Comm.GetTdsInt();
				bool flag2 = num4 < 0;
				if (flag2)
				{
					num4 = ~num4;
					num5 = ~(num5 - 1);
				}
				return new decimal(num5, num4, 0, flag2, 4);
			}
			IL_007B:
			return DBNull.Value;
		}

		protected object GetStringValue(TdsColumnType? colType, bool wideChars, bool outputParam, Encoding encoder)
		{
			Encoding encoding = encoder;
			bool flag;
			if (this.tdsVersion > TdsVersion.tds70 && outputParam && (colType == TdsColumnType.BigChar || colType == TdsColumnType.BigNVarChar || colType == TdsColumnType.BigVarChar || colType == TdsColumnType.NChar || colType == TdsColumnType.NVarChar))
			{
				byte[] bytes = this.Comm.GetBytes(5, true);
				encoding = TdsCharset.GetEncoding(bytes);
				flag = true;
			}
			else
			{
				flag = this.tdsVersion >= TdsVersion.tds70 && (wideChars || !outputParam);
			}
			int num = (int)((!flag) ? ((short)(this.comm.GetByte() & byte.MaxValue)) : this.comm.GetTdsShort());
			return this.GetStringValue(wideChars, num, encoding);
		}

		protected object GetStringValue(bool wideChars, int len, Encoding enc)
		{
			if (this.tdsVersion < TdsVersion.tds70 && len == 0)
			{
				return DBNull.Value;
			}
			if (len >= 0)
			{
				object obj;
				if (wideChars)
				{
					obj = this.comm.GetString(len / 2, enc);
				}
				else
				{
					obj = this.comm.GetString(len, false, enc);
				}
				if (this.tdsVersion < TdsVersion.tds70 && ((string)obj).Equals(" "))
				{
					obj = string.Empty;
				}
				return obj;
			}
			return DBNull.Value;
		}

		protected int GetSubPacketLength()
		{
			return (int)this.comm.GetTdsShort();
		}

		private object GetTextValue(bool wideChars, Encoding encoder)
		{
			byte @byte = this.comm.GetByte();
			if (@byte != 16)
			{
				return DBNull.Value;
			}
			this.comm.Skip(24L);
			int num = this.comm.GetTdsInt();
			if (num == 0)
			{
				return string.Empty;
			}
			string text;
			if (wideChars)
			{
				text = this.comm.GetString(num / 2, encoder);
			}
			else
			{
				text = this.comm.GetString(num, false, encoder);
			}
			num /= 2;
			if ((byte)this.tdsVersion < 70 && text == " ")
			{
				text = string.Empty;
			}
			return text;
		}

		internal bool IsBlobType(TdsColumnType columnType)
		{
			return columnType == TdsColumnType.Text || columnType == TdsColumnType.Image || columnType == TdsColumnType.NText;
		}

		internal bool IsLargeType(TdsColumnType columnType)
		{
			return (byte)columnType > 128;
		}

		protected bool IsWideType(TdsColumnType columnType)
		{
			return columnType == TdsColumnType.NText || columnType == TdsColumnType.NVarChar || columnType == TdsColumnType.NChar;
		}

		internal static bool IsFixedSizeColumn(TdsColumnType columnType)
		{
			switch (columnType)
			{
			case TdsColumnType.Int1:
			case TdsColumnType.Bit:
			case TdsColumnType.Int2:
			case TdsColumnType.Int4:
			case TdsColumnType.DateTime4:
			case TdsColumnType.Real:
			case TdsColumnType.Money:
			case TdsColumnType.DateTime:
			case TdsColumnType.Float8:
				break;
			default:
				if (columnType != TdsColumnType.Money4 && columnType != TdsColumnType.SmallMoney && columnType != TdsColumnType.BigInt)
				{
					return false;
				}
				break;
			}
			return true;
		}

		protected void LoadRow()
		{
			if (this.SequentialAccess)
			{
				if (this.isRowRead)
				{
					this.SkipRow();
				}
				this.isRowRead = true;
				this.isResultRead = true;
				return;
			}
			this.currentRow = new TdsDataRow();
			int num = 0;
			foreach (object obj in this.columns)
			{
				TdsDataColumn tdsDataColumn = (TdsDataColumn)obj;
				object columnValue = this.GetColumnValue(tdsDataColumn.ColumnType, false, num);
				this.currentRow.Add(columnValue);
				if (this.doneProc)
				{
					this.outputParameters.Add(columnValue);
				}
				if (columnValue is TdsBigDecimal && this.currentRow.BigDecimalIndex < 0)
				{
					this.currentRow.BigDecimalIndex = num;
				}
				num++;
			}
		}

		internal static int LookupBufferSize(TdsColumnType columnType)
		{
			switch (columnType)
			{
			case TdsColumnType.Int1:
			case TdsColumnType.Bit:
				return 1;
			default:
				if (columnType != TdsColumnType.Money4 && columnType != TdsColumnType.SmallMoney)
				{
					if (columnType != TdsColumnType.BigInt)
					{
						return 0;
					}
					return 8;
				}
				break;
			case TdsColumnType.Int2:
				return 2;
			case TdsColumnType.Int4:
			case TdsColumnType.DateTime4:
			case TdsColumnType.Real:
				break;
			case TdsColumnType.Money:
			case TdsColumnType.DateTime:
			case TdsColumnType.Float8:
				return 8;
			}
			return 4;
		}

		protected internal int ProcessAuthentication()
		{
			int tdsShort = (int)this.Comm.GetTdsShort();
			byte[] bytes = this.Comm.GetBytes(tdsShort, true);
			Type2Message type2Message = new Type2Message(bytes);
			Type3Message type3Message = new Type3Message();
			type3Message.Challenge = type2Message.Nonce;
			type3Message.Domain = this.connectionParms.DefaultDomain;
			type3Message.Host = this.connectionParms.Hostname;
			type3Message.Username = this.connectionParms.User;
			type3Message.Password = this.connectionParms.Password;
			this.Comm.StartPacket(TdsPacketType.SspAuth);
			this.Comm.Append(type3Message.GetBytes());
			try
			{
				this.Comm.SendPacket();
			}
			catch (IOException ex)
			{
				this.connected = false;
				throw new TdsInternalException("Server closed the connection.", ex);
			}
			return 1;
		}

		protected void ProcessColumnDetail()
		{
			int subPacketLength = this.GetSubPacketLength();
			byte[] array = new byte[3];
			string text = string.Empty;
			int i = 0;
			while (i < subPacketLength)
			{
				for (int j = 0; j < 3; j++)
				{
					array[j] = this.comm.GetByte();
				}
				i += 3;
				bool flag = (array[2] & 32) != 0;
				if (flag)
				{
					int num;
					if (this.tdsVersion >= TdsVersion.tds70)
					{
						num = (int)this.comm.GetByte();
						i += 2 * num + 1;
					}
					else
					{
						num = (int)this.comm.GetByte();
						i += num + 1;
					}
					text = this.comm.GetString(num);
				}
				byte b = array[0] - 1;
				byte b2 = array[1] - 1;
				bool flag2 = (array[2] & 4) != 0;
				TdsDataColumn tdsDataColumn = this.columns[(int)b];
				tdsDataColumn.IsHidden = new bool?((array[2] & 16) != 0);
				tdsDataColumn.IsExpression = new bool?(flag2);
				tdsDataColumn.IsKey = new bool?((array[2] & 8) != 0);
				tdsDataColumn.IsAliased = new bool?(flag);
				tdsDataColumn.BaseColumnName = ((!flag) ? null : text);
				tdsDataColumn.BaseTableName = (flag2 ? null : ((string)this.tableNames[(int)b2]));
			}
		}

		protected abstract void ProcessColumnInfo();

		protected void ProcessColumnNames()
		{
			this.columnNames = new ArrayList();
			int tdsShort = (int)this.comm.GetTdsShort();
			int i = 0;
			int num = 0;
			while (i < tdsShort)
			{
				int @byte = (int)this.comm.GetByte();
				string @string = this.comm.GetString(@byte);
				i = i + 1 + @byte;
				this.columnNames.Add(@string);
				num++;
			}
		}

		[MonoTODO("Make sure counting works right, especially with multiple resultsets.")]
		protected void ProcessEndToken(TdsPacketSubType type)
		{
			byte @byte = this.Comm.GetByte();
			this.Comm.Skip(1L);
			byte byte2 = this.comm.GetByte();
			this.Comm.Skip(1L);
			int tdsInt = this.comm.GetTdsInt();
			bool flag = this.IsValidRowCount(@byte, byte2);
			this.moreResults = (@byte & 1) != 0;
			bool flag2 = (@byte & 32) != 0;
			switch (type)
			{
			case TdsPacketSubType.Done:
			case TdsPacketSubType.DoneInProc:
				break;
			case TdsPacketSubType.DoneProc:
				this.doneProc = true;
				break;
			default:
				goto IL_00C0;
			}
			if (flag)
			{
				if (this.recordsAffected == -1)
				{
					this.recordsAffected = tdsInt;
				}
				else
				{
					this.recordsAffected += tdsInt;
				}
			}
			IL_00C0:
			if (this.moreResults)
			{
				this.queryInProgress = false;
			}
			if (flag2)
			{
				this.cancelsProcessed++;
			}
			if (this.messages.Count > 0 && !this.moreResults)
			{
				this.OnTdsInfoMessage(this.CreateTdsInfoMessageEvent(this.messages));
			}
		}

		protected void ProcessEnvironmentChange()
		{
			int subPacketLength = this.GetSubPacketLength();
			switch (this.comm.GetByte())
			{
			case 1:
			{
				int num = (int)this.comm.GetByte();
				string @string = this.comm.GetString(num);
				num = (int)(this.comm.GetByte() & byte.MaxValue);
				this.comm.GetString(num);
				if (this.originalDatabase == string.Empty)
				{
					this.originalDatabase = @string;
				}
				this.database = @string;
				return;
			}
			case 3:
			{
				int num = (int)this.comm.GetByte();
				if (this.tdsVersion == TdsVersion.tds70)
				{
					this.SetCharset(this.comm.GetString(num));
					this.comm.Skip((long)(subPacketLength - 2 - num * 2));
				}
				else
				{
					this.SetCharset(this.comm.GetString(num));
					this.comm.Skip((long)(subPacketLength - 2 - num));
				}
				return;
			}
			case 4:
			{
				int num = (int)this.comm.GetByte();
				string string2 = this.comm.GetString(num);
				if (this.tdsVersion >= TdsVersion.tds70)
				{
					this.comm.Skip((long)(subPacketLength - 2 - num * 2));
				}
				else
				{
					this.comm.Skip((long)(subPacketLength - 2 - num));
				}
				this.packetSize = int.Parse(string2);
				this.comm.ResizeOutBuf(this.packetSize);
				return;
			}
			case 5:
			{
				int num = (int)this.comm.GetByte();
				int num2;
				if (this.tdsVersion >= TdsVersion.tds70)
				{
					num2 = (int)Convert.ChangeType(this.comm.GetString(num), typeof(int));
					this.comm.Skip((long)(subPacketLength - 2 - num * 2));
				}
				else
				{
					num2 = (int)Convert.ChangeType(this.comm.GetString(num), typeof(int));
					this.comm.Skip((long)(subPacketLength - 2 - num));
				}
				this.locale = new CultureInfo(num2);
				return;
			}
			case 7:
			{
				int num = (int)this.comm.GetByte();
				this.collation = this.comm.GetBytes(num, true);
				int num2 = TdsCollation.LCID(this.collation);
				this.locale = new CultureInfo(num2);
				this.SetCharset(TdsCharset.GetEncoding(this.collation));
				return;
			}
			}
			this.comm.Skip((long)(subPacketLength - 1));
		}

		protected void ProcessLoginAck()
		{
			this.GetSubPacketLength();
			if (this.tdsVersion >= TdsVersion.tds70)
			{
				this.comm.Skip(1L);
				uint tdsInt = (uint)this.comm.GetTdsInt();
				uint num = tdsInt;
				if (num != 117440512U)
				{
					if (num != 117506048U)
					{
						if (num != 1895825409U)
						{
							if (num == 1913192450U)
							{
								this.tdsVersion = TdsVersion.tds90;
							}
						}
						else
						{
							this.tdsVersion = TdsVersion.tds81;
						}
					}
					else
					{
						this.tdsVersion = TdsVersion.tds80;
					}
				}
				else
				{
					this.tdsVersion = TdsVersion.tds70;
				}
			}
			if (this.tdsVersion >= TdsVersion.tds70)
			{
				int @byte = (int)this.comm.GetByte();
				this.databaseProductName = this.comm.GetString(@byte);
				this.databaseMajorVersion = (int)this.comm.GetByte();
				this.databaseProductVersion = string.Format("{0}.{1}.{2}", this.databaseMajorVersion.ToString("00"), this.comm.GetByte().ToString("00"), (256 * (int)this.comm.GetByte() + (int)this.comm.GetByte()).ToString("0000"));
			}
			else
			{
				this.comm.Skip(5L);
				short byte2 = (short)this.comm.GetByte();
				this.databaseProductName = this.comm.GetString((int)byte2);
				this.comm.Skip(1L);
				this.databaseMajorVersion = (int)this.comm.GetByte();
				this.databaseProductVersion = string.Format("{0}.{1}", this.databaseMajorVersion, this.comm.GetByte());
				this.comm.Skip(1L);
			}
			if (this.databaseProductName.Length > 1 && this.databaseProductName.IndexOf('\0') != -1)
			{
				int num2 = this.databaseProductName.IndexOf('\0');
				this.databaseProductName = this.databaseProductName.Substring(0, num2);
			}
			this.connected = true;
		}

		protected void OnTdsErrorMessage(TdsInternalErrorMessageEventArgs e)
		{
			if (this.TdsErrorMessage != null)
			{
				this.TdsErrorMessage(this, e);
			}
		}

		protected void OnTdsInfoMessage(TdsInternalInfoMessageEventArgs e)
		{
			if (this.TdsInfoMessage != null)
			{
				this.TdsInfoMessage(this, e);
			}
			this.messages.Clear();
		}

		protected void ProcessMessage(TdsPacketSubType subType)
		{
			this.GetSubPacketLength();
			int tdsInt = this.comm.GetTdsInt();
			byte @byte = this.comm.GetByte();
			byte byte2 = this.comm.GetByte();
			bool flag;
			if (subType == TdsPacketSubType.EED)
			{
				flag = byte2 > 10;
				this.comm.Skip((long)this.comm.GetByte());
				this.comm.Skip(1L);
				this.comm.Skip(2L);
			}
			else
			{
				flag = subType == TdsPacketSubType.Error;
			}
			string @string = this.comm.GetString((int)this.comm.GetTdsShort());
			string string2 = this.comm.GetString((int)this.comm.GetByte());
			string string3 = this.comm.GetString((int)this.comm.GetByte());
			byte byte3 = this.comm.GetByte();
			this.comm.Skip(1L);
			string empty = string.Empty;
			if (flag)
			{
				this.OnTdsErrorMessage(this.CreateTdsErrorMessageEvent(byte2, (int)byte3, @string, tdsInt, string3, string2, empty, @byte));
			}
			else
			{
				this.messages.Add(new TdsInternalError(byte2, (int)byte3, @string, tdsInt, string3, string2, empty, @byte));
			}
		}

		protected virtual void ProcessOutputParam()
		{
			this.GetSubPacketLength();
			this.comm.GetString((int)(this.comm.GetByte() & byte.MaxValue));
			this.comm.Skip(5L);
			TdsColumnType @byte = (TdsColumnType)this.comm.GetByte();
			object columnValue = this.GetColumnValue(new TdsColumnType?(@byte), true);
			this.outputParameters.Add(columnValue);
		}

		protected void ProcessDynamic()
		{
			this.Comm.Skip(2L);
			this.Comm.GetByte();
			this.Comm.GetByte();
			this.Comm.GetString((int)this.Comm.GetByte());
		}

		protected virtual TdsPacketSubType ProcessSubPacket()
		{
			TdsPacketSubType @byte = (TdsPacketSubType)this.comm.GetByte();
			TdsPacketSubType tdsPacketSubType = @byte;
			switch (tdsPacketSubType)
			{
			case TdsPacketSubType.ColumnName:
				this.Comm.Skip(8L);
				this.ProcessColumnNames();
				return @byte;
			case TdsPacketSubType.ColumnInfo:
				goto IL_019F;
			default:
				switch (tdsPacketSubType)
				{
				case TdsPacketSubType.Capability:
				case TdsPacketSubType.ParamFormat:
					break;
				case TdsPacketSubType.EnvironmentChange:
					this.ProcessEnvironmentChange();
					return @byte;
				default:
					switch (tdsPacketSubType)
					{
					case TdsPacketSubType.ReturnStatus:
						this.ProcessReturnStatus();
						return @byte;
					default:
						switch (tdsPacketSubType)
						{
						case TdsPacketSubType.Done:
						case TdsPacketSubType.DoneProc:
						case TdsPacketSubType.DoneInProc:
							this.ProcessEndToken(@byte);
							return @byte;
						default:
							if (tdsPacketSubType == TdsPacketSubType.ColumnMetadata)
							{
								goto IL_019F;
							}
							if (tdsPacketSubType != TdsPacketSubType.Row)
							{
								return @byte;
							}
							this.LoadRow();
							return @byte;
						}
						break;
					case TdsPacketSubType.ProcId:
						this.Comm.Skip(8L);
						return @byte;
					}
					break;
				case TdsPacketSubType.EED:
					goto IL_0130;
				case TdsPacketSubType.Dynamic:
					this.ProcessDynamic();
					return @byte;
				case TdsPacketSubType.Authentication:
					this.ProcessAuthentication();
					return @byte;
				case TdsPacketSubType.RowFormat:
					goto IL_019F;
				}
				break;
			case TdsPacketSubType.Dynamic2:
				this.comm.Skip((long)this.comm.GetTdsInt());
				return @byte;
			case TdsPacketSubType.TableName:
				this.ProcessTableName();
				return @byte;
			case TdsPacketSubType.ColumnDetail:
				this.ProcessColumnDetail();
				return @byte;
			case TdsPacketSubType.AltName:
			case TdsPacketSubType.AltFormat:
				break;
			case TdsPacketSubType.ColumnOrder:
				this.comm.Skip((long)this.comm.GetTdsShort());
				return @byte;
			case TdsPacketSubType.Error:
			case TdsPacketSubType.Info:
				goto IL_0130;
			case TdsPacketSubType.Param:
				this.ProcessOutputParam();
				return @byte;
			case TdsPacketSubType.LoginAck:
				this.ProcessLoginAck();
				return @byte;
			case TdsPacketSubType.Control:
				this.comm.Skip((long)this.comm.GetTdsShort());
				return @byte;
			}
			this.comm.Skip((long)this.comm.GetTdsShort());
			return @byte;
			IL_0130:
			this.ProcessMessage(@byte);
			return @byte;
			IL_019F:
			this.Columns.Clear();
			this.ProcessColumnInfo();
			return @byte;
		}

		protected void ProcessTableName()
		{
			this.tableNames = new ArrayList();
			int tdsShort = (int)this.comm.GetTdsShort();
			int i = 0;
			while (i < tdsShort)
			{
				int num;
				if (this.tdsVersion >= TdsVersion.tds70)
				{
					num = (int)this.comm.GetTdsShort();
					i += 2 * (num + 1);
				}
				else
				{
					num = (int)this.comm.GetByte();
					i += num + 1;
				}
				this.tableNames.Add(this.comm.GetString(num));
			}
		}

		protected void SetCharset(Encoding encoder)
		{
			this.comm.Encoder = encoder;
		}

		protected void SetCharset(string charset)
		{
			if (charset == null || charset.Length > 30)
			{
				charset = "iso_1";
			}
			if (this.charset != null && this.charset == charset)
			{
				return;
			}
			if (charset.StartsWith("cp"))
			{
				this.encoder = Encoding.GetEncoding(int.Parse(charset.Substring(2)));
				this.charset = charset;
			}
			else
			{
				this.encoder = Encoding.GetEncoding("iso-8859-1");
				this.charset = "iso_1";
			}
			this.SetCharset(this.encoder);
		}

		protected void SetLanguage(string language)
		{
			if (language == null || language.Length > 30)
			{
				language = "us_english";
			}
			this.language = language;
		}

		protected virtual void ProcessReturnStatus()
		{
			this.comm.Skip(4L);
		}

		protected IAsyncResult BeginExecuteQueryInternal(string sql, bool wantResults, AsyncCallback callback, object state)
		{
			this.InitExec();
			TdsAsyncResult tdsAsyncResult = new TdsAsyncResult(callback, state);
			tdsAsyncResult.TdsAsyncState.WantResults = wantResults;
			this.Comm.StartPacket(TdsPacketType.Query);
			this.Comm.Append(sql);
			try
			{
				this.Comm.SendPacket();
				this.Comm.BeginReadPacket(new AsyncCallback(this.OnBeginExecuteQueryCallback), tdsAsyncResult);
			}
			catch (IOException ex)
			{
				this.connected = false;
				throw new TdsInternalException("Server closed the connection.", ex);
			}
			return tdsAsyncResult;
		}

		protected void EndExecuteQueryInternal(IAsyncResult ar)
		{
			if (!ar.IsCompleted)
			{
				ar.AsyncWaitHandle.WaitOne();
			}
			TdsAsyncResult tdsAsyncResult = (TdsAsyncResult)ar;
			if (tdsAsyncResult.IsCompletedWithException)
			{
				throw tdsAsyncResult.Exception;
			}
		}

		protected void OnBeginExecuteQueryCallback(IAsyncResult ar)
		{
			TdsAsyncResult tdsAsyncResult = (TdsAsyncResult)ar.AsyncState;
			TdsAsyncState tdsAsyncState = tdsAsyncResult.TdsAsyncState;
			try
			{
				this.Comm.EndReadPacket(ar);
				if (!tdsAsyncState.WantResults)
				{
					this.SkipToEnd();
				}
			}
			catch (Exception ex)
			{
				tdsAsyncResult.MarkComplete(ex);
				return;
			}
			tdsAsyncResult.MarkComplete();
		}

		public virtual IAsyncResult BeginExecuteNonQuery(string sql, TdsMetaParameterCollection parameters, AsyncCallback callback, object state)
		{
			throw new NotImplementedException("should not be called!");
		}

		public virtual void EndExecuteNonQuery(IAsyncResult ar)
		{
			throw new NotImplementedException("should not be called!");
		}

		public virtual IAsyncResult BeginExecuteQuery(string sql, TdsMetaParameterCollection parameters, AsyncCallback callback, object state)
		{
			throw new NotImplementedException("should not be called!");
		}

		public virtual void EndExecuteQuery(IAsyncResult ar)
		{
			throw new NotImplementedException("should not be called!");
		}

		public virtual IAsyncResult BeginExecuteProcedure(string prolog, string epilog, string cmdText, bool IsNonQuery, TdsMetaParameterCollection parameters, AsyncCallback callback, object state)
		{
			throw new NotImplementedException("should not be called!");
		}

		public virtual void EndExecuteProcedure(IAsyncResult ar)
		{
			throw new NotImplementedException("should not be called!");
		}

		public void WaitFor(IAsyncResult ar)
		{
			if (!ar.IsCompleted)
			{
				ar.AsyncWaitHandle.WaitOne();
			}
		}

		public void CheckAndThrowException(IAsyncResult ar)
		{
			TdsAsyncResult tdsAsyncResult = (TdsAsyncResult)ar;
			if (tdsAsyncResult.IsCompleted && tdsAsyncResult.IsCompletedWithException)
			{
				throw tdsAsyncResult.Exception;
			}
		}

		private TdsComm comm;

		private TdsVersion tdsVersion;

		protected internal TdsConnectionParameters connectionParms;

		protected readonly byte[] NTLMSSP_ID = new byte[] { 78, 84, 76, 77, 83, 83, 80, 0 };

		private int packetSize;

		private string dataSource;

		private string database;

		private string originalDatabase = string.Empty;

		private string databaseProductName;

		private string databaseProductVersion;

		private int databaseMajorVersion;

		private CultureInfo locale = CultureInfo.InvariantCulture;

		private string charset;

		private string language;

		private bool connected;

		private bool moreResults;

		private Encoding encoder;

		private bool doneProc;

		private bool pooling = true;

		private TdsDataRow currentRow;

		private TdsDataColumnCollection columns;

		private ArrayList tableNames;

		private ArrayList columnNames;

		private TdsMetaParameterCollection parameters = new TdsMetaParameterCollection();

		private bool queryInProgress;

		private int cancelsRequested;

		private int cancelsProcessed;

		private ArrayList outputParameters = new ArrayList();

		protected TdsInternalErrorCollection messages = new TdsInternalErrorCollection();

		private int recordsAffected = -1;

		private long StreamLength;

		private long StreamIndex;

		private int StreamColumnIndex;

		private bool sequentialAccess;

		private bool isRowRead;

		private bool isResultRead;

		private bool LoadInProgress;

		private byte[] collation;

		internal int poolStatus;
	}
}
