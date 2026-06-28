using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Sql;
using System.Text;
using System.Xml;
using Mono.Data.Tds;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	[Designer("Microsoft.VSDesigner.Data.VS.SqlCommandDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[ToolboxItem("System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultEvent("RecordsAffected")]
	public sealed class SqlCommand : DbCommand, IDisposable, IDbCommand, ICloneable
	{
		public SqlCommand()
			: this(string.Empty, null, null)
		{
		}

		public SqlCommand(string cmdText)
			: this(cmdText, null, null)
		{
		}

		public SqlCommand(string cmdText, SqlConnection connection)
			: this(cmdText, connection, null)
		{
		}

		public SqlCommand(string cmdText, SqlConnection connection, SqlTransaction transaction)
		{
			this.commandText = cmdText;
			this.connection = connection;
			this.transaction = transaction;
			this.commandType = CommandType.Text;
			this.updatedRowSource = UpdateRowSource.Both;
			this.commandTimeout = 30;
			this.notificationAutoEnlist = true;
			this.designTimeVisible = true;
			this.parameters = new SqlParameterCollection(this);
		}

		private SqlCommand(string commandText, SqlConnection connection, SqlTransaction transaction, CommandType commandType, UpdateRowSource updatedRowSource, bool designTimeVisible, int commandTimeout, SqlParameterCollection parameters)
		{
			this.commandText = commandText;
			this.connection = connection;
			this.transaction = transaction;
			this.commandType = commandType;
			this.updatedRowSource = updatedRowSource;
			this.designTimeVisible = designTimeVisible;
			this.commandTimeout = commandTimeout;
			this.parameters = new SqlParameterCollection(this);
			for (int i = 0; i < parameters.Count; i++)
			{
				this.parameters.Add(((ICloneable)parameters[i]).Clone());
			}
		}

		public event StatementCompletedEventHandler StatementCompleted;

		object ICloneable.Clone()
		{
			return new SqlCommand(this.commandText, this.connection, this.transaction, this.commandType, this.updatedRowSource, this.designTimeVisible, this.commandTimeout, this.parameters);
		}

		internal CommandBehavior CommandBehavior
		{
			get
			{
				return this.behavior;
			}
		}

		[DefaultValue("")]
		[Editor("Microsoft.VSDesigner.Data.SQL.Design.SqlCommandTextEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[RefreshProperties(RefreshProperties.All)]
		public override string CommandText
		{
			get
			{
				if (this.commandText == null)
				{
					return string.Empty;
				}
				return this.commandText;
			}
			set
			{
				if (value != this.commandText && this.preparedStatement != null)
				{
					this.Unprepare();
				}
				this.commandText = value;
			}
		}

		public override int CommandTimeout
		{
			get
			{
				return this.commandTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The property value assigned is less than 0.", "CommandTimeout");
				}
				this.commandTimeout = value;
			}
		}

		[DefaultValue(CommandType.Text)]
		[RefreshProperties(RefreshProperties.All)]
		public override CommandType CommandType
		{
			get
			{
				return this.commandType;
			}
			set
			{
				if (value == CommandType.TableDirect)
				{
					throw new ArgumentOutOfRangeException("CommandType.TableDirect is not supported by the Mono SqlClient Data Provider.");
				}
				ExceptionHelper.CheckEnumValue(typeof(CommandType), value);
				this.commandType = value;
			}
		}

		[DefaultValue(null)]
		[Editor("Microsoft.VSDesigner.Data.Design.DbConnectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new SqlConnection Connection
		{
			get
			{
				return this.connection;
			}
			set
			{
				this.connection = value;
			}
		}

		[DefaultValue(true)]
		[Browsable(false)]
		[DesignOnly(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool DesignTimeVisible
		{
			get
			{
				return this.designTimeVisible;
			}
			set
			{
				this.designTimeVisible = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SqlParameterCollection Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		internal Tds Tds
		{
			get
			{
				return this.Connection.Tds;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public new SqlTransaction Transaction
		{
			get
			{
				if (this.transaction != null && !this.transaction.IsOpen)
				{
					this.transaction = null;
				}
				return this.transaction;
			}
			set
			{
				this.transaction = value;
			}
		}

		[DefaultValue(UpdateRowSource.Both)]
		public override UpdateRowSource UpdatedRowSource
		{
			get
			{
				return this.updatedRowSource;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(UpdateRowSource), value);
				this.updatedRowSource = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SqlNotificationRequest Notification
		{
			get
			{
				return this.notification;
			}
			set
			{
				this.notification = value;
			}
		}

		[DefaultValue(true)]
		public bool NotificationAutoEnlist
		{
			get
			{
				return this.notificationAutoEnlist;
			}
			set
			{
				this.notificationAutoEnlist = value;
			}
		}

		public override void Cancel()
		{
			if (this.Connection == null || this.Connection.Tds == null)
			{
				return;
			}
			this.Connection.Tds.Cancel();
		}

		public SqlCommand Clone()
		{
			return new SqlCommand(this.commandText, this.connection, this.transaction, this.commandType, this.updatedRowSource, this.designTimeVisible, this.commandTimeout, this.parameters);
		}

		internal void CloseDataReader()
		{
			if (this.Connection != null)
			{
				this.Connection.DataReader = null;
				if ((this.behavior & CommandBehavior.CloseConnection) != CommandBehavior.Default)
				{
					this.Connection.Close();
				}
				if (this.Tds != null)
				{
					this.Tds.SequentialAccess = false;
				}
			}
			this.behavior = CommandBehavior.Default;
		}

		public new SqlParameter CreateParameter()
		{
			return new SqlParameter();
		}

		private string EscapeProcName(string name, bool schema)
		{
			string text = name.Trim();
			int length = text.Length;
			char[] array = new char[] { '[', ']' };
			int num = 0;
			int num2 = length;
			string text2;
			if (length > 1)
			{
				int num3;
				bool flag = (num3 = text.IndexOf('[')) <= 0;
				if (flag && num3 > -1)
				{
					int num4 = text.IndexOf(']');
					if (num3 > num4 && num4 != -1)
					{
						flag = false;
					}
					else if (num4 == length - 1)
					{
						if (text.IndexOfAny(array, 1, length - 2) != -1)
						{
							flag = false;
						}
						else
						{
							num = 1;
							num2 = length - 2;
						}
					}
					else
					{
						flag = num4 == -1 && schema;
					}
				}
				if (!flag)
				{
					throw new ArgumentException(string.Format("SqlCommand.CommandText property value is an invalid multipart name {0}, incorrect usage of quotes", this.CommandText));
				}
				text2 = text.Substring(num, num2);
			}
			else
			{
				text2 = text;
			}
			return text2;
		}

		internal void DeriveParameters()
		{
			if (this.commandType != CommandType.StoredProcedure)
			{
				throw new InvalidOperationException(string.Format("SqlCommand DeriveParameters only supports CommandType.StoredProcedure, not CommandType.{0}", this.commandType));
			}
			this.ValidateCommand("DeriveParameters", false);
			string text = this.CommandText;
			string text2 = string.Empty;
			int num = text.IndexOf('.');
			if (num >= 0)
			{
				text2 = text.Substring(0, num);
				text = text.Substring(num + 1);
			}
			text = this.EscapeProcName(text, false);
			text2 = this.EscapeProcName(text2, true);
			SqlParameterCollection sqlParameterCollection = new SqlParameterCollection(this);
			sqlParameterCollection.Add("@procedure_name", SqlDbType.NVarChar, text.Length).Value = text;
			if (text2.Length > 0)
			{
				sqlParameterCollection.Add("@procedure_schema", SqlDbType.NVarChar, text2.Length).Value = text2;
			}
			string text3 = "sp_procedure_params_rowset";
			try
			{
				this.Connection.Tds.ExecProc(text3, sqlParameterCollection.MetaParameters, 0, true);
			}
			catch (TdsTimeoutException ex)
			{
				this.Connection.Tds.Reset();
				throw SqlException.FromTdsInternalException(ex);
			}
			catch (TdsInternalException ex2)
			{
				this.Connection.Close();
				throw SqlException.FromTdsInternalException(ex2);
			}
			SqlDataReader sqlDataReader = new SqlDataReader(this);
			this.parameters.Clear();
			object[] array = new object[sqlDataReader.FieldCount];
			while (sqlDataReader.Read())
			{
				sqlDataReader.GetValues(array);
				this.parameters.Add(new SqlParameter(array));
			}
			sqlDataReader.Close();
			if (this.parameters.Count == 0)
			{
				throw new InvalidOperationException("Stored procedure '" + text + "' does not exist.");
			}
		}

		private void Execute(bool wantResults)
		{
			int num = 0;
			this.Connection.Tds.RecordsAffected = -1;
			TdsMetaParameterCollection metaParameters = this.Parameters.MetaParameters;
			foreach (object obj in ((IEnumerable)metaParameters))
			{
				TdsMetaParameter tdsMetaParameter = (TdsMetaParameter)obj;
				tdsMetaParameter.Validate(num++);
			}
			if (this.preparedStatement == null)
			{
				bool flag = (this.behavior & CommandBehavior.SchemaOnly) > CommandBehavior.Default;
				bool flag2 = (this.behavior & CommandBehavior.KeyInfo) > CommandBehavior.Default;
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				if (flag || flag2)
				{
					stringBuilder.Append("SET FMTONLY OFF;");
				}
				if (flag2)
				{
					stringBuilder.Append("SET NO_BROWSETABLE ON;");
					stringBuilder2.Append("SET NO_BROWSETABLE OFF;");
				}
				if (flag)
				{
					stringBuilder.Append("SET FMTONLY ON;");
					stringBuilder2.Append("SET FMTONLY OFF;");
				}
				switch (this.CommandType)
				{
				case CommandType.Text:
				{
					string text;
					if (stringBuilder2.Length > 0)
					{
						text = string.Format("{0}{1};{2}", stringBuilder.ToString(), this.CommandText, stringBuilder2.ToString());
					}
					else
					{
						text = string.Format("{0}{1}", stringBuilder.ToString(), this.CommandText);
					}
					try
					{
						this.Connection.Tds.Execute(text, metaParameters, this.CommandTimeout, wantResults);
					}
					catch (TdsTimeoutException ex)
					{
						this.Connection.Tds.Reset();
						throw SqlException.FromTdsInternalException(ex);
					}
					catch (TdsInternalException ex2)
					{
						this.Connection.Close();
						throw SqlException.FromTdsInternalException(ex2);
					}
					break;
				}
				case CommandType.StoredProcedure:
					try
					{
						if (flag2 || flag)
						{
							this.Connection.Tds.Execute(stringBuilder.ToString());
						}
						this.Connection.Tds.ExecProc(this.CommandText, metaParameters, this.CommandTimeout, wantResults);
						if (flag2 || flag)
						{
							this.Connection.Tds.Execute(stringBuilder2.ToString());
						}
					}
					catch (TdsTimeoutException ex3)
					{
						this.Connection.Tds.Reset();
						throw SqlException.FromTdsInternalException(ex3);
					}
					catch (TdsInternalException ex4)
					{
						this.Connection.Close();
						throw SqlException.FromTdsInternalException(ex4);
					}
					break;
				}
			}
			else
			{
				try
				{
					this.Connection.Tds.ExecPrepared(this.preparedStatement, metaParameters, this.CommandTimeout, wantResults);
				}
				catch (TdsTimeoutException ex5)
				{
					this.Connection.Tds.Reset();
					throw SqlException.FromTdsInternalException(ex5);
				}
				catch (TdsInternalException ex6)
				{
					this.Connection.Close();
					throw SqlException.FromTdsInternalException(ex6);
				}
			}
		}

		public override int ExecuteNonQuery()
		{
			this.ValidateCommand("ExecuteNonQuery", false);
			int num = 0;
			this.behavior = CommandBehavior.Default;
			try
			{
				this.Execute(false);
				num = this.Connection.Tds.RecordsAffected;
			}
			catch (TdsTimeoutException ex)
			{
				this.Connection.Tds.Reset();
				throw SqlException.FromTdsInternalException(ex);
			}
			this.GetOutputParameters();
			return num;
		}

		public new SqlDataReader ExecuteReader()
		{
			return this.ExecuteReader(CommandBehavior.Default);
		}

		public new SqlDataReader ExecuteReader(CommandBehavior behavior)
		{
			this.ValidateCommand("ExecuteReader", false);
			if ((behavior & CommandBehavior.SingleRow) != CommandBehavior.Default)
			{
				behavior |= CommandBehavior.SingleResult;
			}
			this.behavior = behavior;
			if ((behavior & CommandBehavior.SequentialAccess) != CommandBehavior.Default)
			{
				this.Tds.SequentialAccess = true;
			}
			SqlDataReader dataReader;
			try
			{
				this.Execute(true);
				this.Connection.DataReader = new SqlDataReader(this);
				dataReader = this.Connection.DataReader;
			}
			catch
			{
				if ((behavior & CommandBehavior.CloseConnection) != CommandBehavior.Default)
				{
					this.Connection.Close();
				}
				throw;
			}
			return dataReader;
		}

		public override object ExecuteScalar()
		{
			object obj2;
			try
			{
				object obj = null;
				this.ValidateCommand("ExecuteScalar", false);
				this.behavior = CommandBehavior.Default;
				this.Execute(true);
				try
				{
					if (this.Connection.Tds.NextResult() && this.Connection.Tds.NextRow())
					{
						obj = this.Connection.Tds.ColumnValues[0];
					}
					if (this.commandType == CommandType.StoredProcedure)
					{
						this.Connection.Tds.SkipToEnd();
						this.GetOutputParameters();
					}
				}
				catch (TdsTimeoutException ex)
				{
					this.Connection.Tds.Reset();
					throw SqlException.FromTdsInternalException(ex);
				}
				catch (TdsInternalException ex2)
				{
					this.Connection.Close();
					throw SqlException.FromTdsInternalException(ex2);
				}
				obj2 = obj;
			}
			finally
			{
				this.CloseDataReader();
			}
			return obj2;
		}

		public XmlReader ExecuteXmlReader()
		{
			this.ValidateCommand("ExecuteXmlReader", false);
			this.behavior = CommandBehavior.Default;
			try
			{
				this.Execute(true);
			}
			catch (TdsTimeoutException ex)
			{
				this.Connection.Tds.Reset();
				throw SqlException.FromTdsInternalException(ex);
			}
			SqlDataReader sqlDataReader = new SqlDataReader(this);
			SqlXmlTextReader sqlXmlTextReader = new SqlXmlTextReader(sqlDataReader);
			return new XmlTextReader(sqlXmlTextReader);
		}

		internal void GetOutputParameters()
		{
			IList outputParameters = this.Connection.Tds.OutputParameters;
			if (outputParameters != null && outputParameters.Count > 0)
			{
				int num = 0;
				foreach (object obj in this.parameters)
				{
					SqlParameter sqlParameter = (SqlParameter)obj;
					if (sqlParameter.Direction != ParameterDirection.Input && sqlParameter.Direction != ParameterDirection.ReturnValue)
					{
						sqlParameter.Value = outputParameters[num];
						num++;
					}
					if (num >= outputParameters.Count)
					{
						break;
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing)
			{
				this.parameters.Clear();
			}
			base.Dispose(disposing);
			this.disposed = true;
		}

		public override void Prepare()
		{
			if (this.Connection == null)
			{
				throw new NullReferenceException();
			}
			if (this.CommandType == CommandType.StoredProcedure || (this.CommandType == CommandType.Text && this.Parameters.Count == 0))
			{
				return;
			}
			this.ValidateCommand("Prepare", false);
			try
			{
				foreach (object obj in this.Parameters)
				{
					SqlParameter sqlParameter = (SqlParameter)obj;
					sqlParameter.CheckIfInitialized();
				}
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("SqlCommand.Prepare requires " + ex.Message);
			}
			this.preparedStatement = this.Connection.Tds.Prepare(this.CommandText, this.Parameters.MetaParameters);
		}

		public void ResetCommandTimeout()
		{
			this.commandTimeout = 30;
		}

		private void Unprepare()
		{
			this.Connection.Tds.Unprepare(this.preparedStatement);
			this.preparedStatement = null;
		}

		private void ValidateCommand(string method, bool async)
		{
			if (this.Connection == null)
			{
				throw new InvalidOperationException(string.Format("{0}: A Connection object is required to continue.", method));
			}
			if (this.Transaction == null && this.Connection.Transaction != null)
			{
				throw new InvalidOperationException(string.Format("{0} requires a transaction if the command's connection is in a pending transaction.", method));
			}
			if (this.Transaction != null && this.Transaction.Connection != this.Connection)
			{
				throw new InvalidOperationException("The connection does not have the same transaction as the command.");
			}
			if (this.Connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException(string.Format("{0} requires an open connection to continue. This connection is closed.", method));
			}
			if (this.CommandText.Length == 0)
			{
				throw new InvalidOperationException(string.Format("{0}: CommandText has not been set for this Command.", method));
			}
			if (this.Connection.DataReader != null)
			{
				throw new InvalidOperationException("There is already an open DataReader associated with this Connection which must be closed first.");
			}
			if (this.Connection.XmlReader != null)
			{
				throw new InvalidOperationException("There is already an open XmlReader associated with this Connection which must be closed first.");
			}
			if (async && !this.Connection.AsyncProcessing)
			{
				throw new InvalidOperationException("This Connection object is not in Asynchronous mode. Use 'Asynchronous Processing = true' to set it.");
			}
		}

		protected override DbParameter CreateDbParameter()
		{
			return this.CreateParameter();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return this.ExecuteReader(behavior);
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return this.Connection;
			}
			set
			{
				this.Connection = (SqlConnection)value;
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				return this.Parameters;
			}
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				return this.Transaction;
			}
			set
			{
				this.Transaction = (SqlTransaction)value;
			}
		}

		internal IAsyncResult BeginExecuteInternal(CommandBehavior behavior, bool wantResults, AsyncCallback callback, object state)
		{
			IAsyncResult asyncResult = null;
			this.Connection.Tds.RecordsAffected = -1;
			TdsMetaParameterCollection metaParameters = this.Parameters.MetaParameters;
			if (this.preparedStatement == null)
			{
				bool flag = (behavior & CommandBehavior.SchemaOnly) > CommandBehavior.Default;
				bool flag2 = (behavior & CommandBehavior.KeyInfo) > CommandBehavior.Default;
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				if (flag || flag2)
				{
					stringBuilder.Append("SET FMTONLY OFF;");
				}
				if (flag2)
				{
					stringBuilder.Append("SET NO_BROWSETABLE ON;");
					stringBuilder2.Append("SET NO_BROWSETABLE OFF;");
				}
				if (flag)
				{
					stringBuilder.Append("SET FMTONLY ON;");
					stringBuilder2.Append("SET FMTONLY OFF;");
				}
				switch (this.CommandType)
				{
				case CommandType.Text:
				{
					string text = string.Format("{0}{1};{2}", stringBuilder.ToString(), this.CommandText, stringBuilder2.ToString());
					try
					{
						if (wantResults)
						{
							asyncResult = this.Connection.Tds.BeginExecuteQuery(text, metaParameters, callback, state);
						}
						else
						{
							asyncResult = this.Connection.Tds.BeginExecuteNonQuery(text, metaParameters, callback, state);
						}
					}
					catch (TdsTimeoutException ex)
					{
						this.Connection.Tds.Reset();
						throw SqlException.FromTdsInternalException(ex);
					}
					catch (TdsInternalException ex2)
					{
						this.Connection.Close();
						throw SqlException.FromTdsInternalException(ex2);
					}
					break;
				}
				case CommandType.StoredProcedure:
				{
					string text2 = string.Empty;
					string text3 = string.Empty;
					if (flag2 || flag)
					{
						text2 = stringBuilder.ToString();
					}
					if (flag2 || flag)
					{
						text3 = stringBuilder2.ToString();
					}
					try
					{
						this.Connection.Tds.BeginExecuteProcedure(text2, text3, this.CommandText, !wantResults, metaParameters, callback, state);
					}
					catch (TdsTimeoutException ex3)
					{
						this.Connection.Tds.Reset();
						throw SqlException.FromTdsInternalException(ex3);
					}
					catch (TdsInternalException ex4)
					{
						this.Connection.Close();
						throw SqlException.FromTdsInternalException(ex4);
					}
					break;
				}
				}
			}
			else
			{
				try
				{
					this.Connection.Tds.ExecPrepared(this.preparedStatement, metaParameters, this.CommandTimeout, wantResults);
				}
				catch (TdsTimeoutException ex5)
				{
					this.Connection.Tds.Reset();
					throw SqlException.FromTdsInternalException(ex5);
				}
				catch (TdsInternalException ex6)
				{
					this.Connection.Close();
					throw SqlException.FromTdsInternalException(ex6);
				}
			}
			return asyncResult;
		}

		internal void EndExecuteInternal(IAsyncResult ar)
		{
			SqlAsyncResult sqlAsyncResult = (SqlAsyncResult)ar;
			this.Connection.Tds.WaitFor(sqlAsyncResult.InternalResult);
			this.Connection.Tds.CheckAndThrowException(sqlAsyncResult.InternalResult);
		}

		public IAsyncResult BeginExecuteNonQuery()
		{
			return this.BeginExecuteNonQuery(null, null);
		}

		public IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, object stateObject)
		{
			this.ValidateCommand("BeginExecuteNonQuery", true);
			SqlAsyncResult sqlAsyncResult = new SqlAsyncResult(callback, stateObject);
			sqlAsyncResult.EndMethod = "EndExecuteNonQuery";
			sqlAsyncResult.InternalResult = this.BeginExecuteInternal(CommandBehavior.Default, false, sqlAsyncResult.BubbleCallback, sqlAsyncResult);
			return sqlAsyncResult;
		}

		public int EndExecuteNonQuery(IAsyncResult asyncResult)
		{
			this.ValidateAsyncResult(asyncResult, "EndExecuteNonQuery");
			this.EndExecuteInternal(asyncResult);
			int recordsAffected = this.Connection.Tds.RecordsAffected;
			this.GetOutputParameters();
			((SqlAsyncResult)asyncResult).Ended = true;
			return recordsAffected;
		}

		public IAsyncResult BeginExecuteReader()
		{
			return this.BeginExecuteReader(null, null, CommandBehavior.Default);
		}

		public IAsyncResult BeginExecuteReader(CommandBehavior behavior)
		{
			return this.BeginExecuteReader(null, null, behavior);
		}

		public IAsyncResult BeginExecuteReader(AsyncCallback callback, object stateObject)
		{
			return this.BeginExecuteReader(callback, stateObject, CommandBehavior.Default);
		}

		public IAsyncResult BeginExecuteReader(AsyncCallback callback, object stateObject, CommandBehavior behavior)
		{
			this.ValidateCommand("BeginExecuteReader", true);
			this.behavior = behavior;
			SqlAsyncResult sqlAsyncResult = new SqlAsyncResult(callback, stateObject);
			sqlAsyncResult.EndMethod = "EndExecuteReader";
			IAsyncResult asyncResult = this.BeginExecuteInternal(behavior, true, sqlAsyncResult.BubbleCallback, stateObject);
			sqlAsyncResult.InternalResult = asyncResult;
			return sqlAsyncResult;
		}

		public SqlDataReader EndExecuteReader(IAsyncResult asyncResult)
		{
			this.ValidateAsyncResult(asyncResult, "EndExecuteReader");
			this.EndExecuteInternal(asyncResult);
			SqlDataReader sqlDataReader = null;
			try
			{
				sqlDataReader = new SqlDataReader(this);
			}
			catch (TdsTimeoutException ex)
			{
				throw SqlException.FromTdsInternalException(ex);
			}
			catch (TdsInternalException ex2)
			{
				if ((this.behavior & CommandBehavior.CloseConnection) != CommandBehavior.Default)
				{
					this.Connection.Close();
				}
				throw SqlException.FromTdsInternalException(ex2);
			}
			((SqlAsyncResult)asyncResult).Ended = true;
			return sqlDataReader;
		}

		public IAsyncResult BeginExecuteXmlReader(AsyncCallback callback, object stateObject)
		{
			this.ValidateCommand("BeginExecuteXmlReader", true);
			SqlAsyncResult sqlAsyncResult = new SqlAsyncResult(callback, stateObject);
			sqlAsyncResult.EndMethod = "EndExecuteXmlReader";
			sqlAsyncResult.InternalResult = this.BeginExecuteInternal(this.behavior, true, sqlAsyncResult.BubbleCallback, stateObject);
			return sqlAsyncResult;
		}

		public IAsyncResult BeginExecuteXmlReader()
		{
			return this.BeginExecuteXmlReader(null, null);
		}

		public XmlReader EndExecuteXmlReader(IAsyncResult asyncResult)
		{
			this.ValidateAsyncResult(asyncResult, "EndExecuteXmlReader");
			this.EndExecuteInternal(asyncResult);
			SqlDataReader sqlDataReader = new SqlDataReader(this);
			SqlXmlTextReader sqlXmlTextReader = new SqlXmlTextReader(sqlDataReader);
			XmlReader xmlReader = new XmlTextReader(sqlXmlTextReader);
			((SqlAsyncResult)asyncResult).Ended = true;
			return xmlReader;
		}

		internal void ValidateAsyncResult(IAsyncResult ar, string endMethod)
		{
			if (ar == null)
			{
				throw new ArgumentException("result passed is null!");
			}
			if (!(ar is SqlAsyncResult))
			{
				throw new ArgumentException(string.Format("cannot test validity of types {0}", ar.GetType()));
			}
			SqlAsyncResult sqlAsyncResult = (SqlAsyncResult)ar;
			if (sqlAsyncResult.EndMethod != endMethod)
			{
				throw new InvalidOperationException(string.Format("Mismatched {0} called for AsyncResult. Expected call to {1} but {0} is called instead.", endMethod, sqlAsyncResult.EndMethod));
			}
			if (sqlAsyncResult.Ended)
			{
				throw new InvalidOperationException(string.Format("The method {0} cannot be called more than once for the same AsyncResult.", endMethod));
			}
		}

		private const int DEFAULT_COMMAND_TIMEOUT = 30;

		private int commandTimeout;

		private bool designTimeVisible;

		private string commandText;

		private CommandType commandType;

		private SqlConnection connection;

		private SqlTransaction transaction;

		private UpdateRowSource updatedRowSource;

		private CommandBehavior behavior;

		private SqlParameterCollection parameters;

		private string preparedStatement;

		private bool disposed;

		private SqlNotificationRequest notification;

		private bool notificationAutoEnlist;
	}
}
