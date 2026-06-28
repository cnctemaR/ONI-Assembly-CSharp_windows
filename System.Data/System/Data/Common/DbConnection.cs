using System;
using System.ComponentModel;
using System.Transactions;

namespace System.Data.Common
{
	public abstract class DbConnection : Component, IDisposable, IDbConnection
	{
		public virtual event StateChangeEventHandler StateChange;

		IDbTransaction IDbConnection.BeginTransaction()
		{
			return this.BeginTransaction();
		}

		IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
		{
			return this.BeginTransaction(il);
		}

		IDbCommand IDbConnection.CreateCommand()
		{
			return this.CreateCommand();
		}

		[RecommendedAsConfigurable(true)]
		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
		public abstract string ConnectionString { get; set; }

		public abstract string Database { get; }

		public abstract string DataSource { get; }

		[Browsable(false)]
		public abstract string ServerVersion { get; }

		[Browsable(false)]
		public abstract ConnectionState State { get; }

		public virtual int ConnectionTimeout
		{
			get
			{
				return 15;
			}
		}

		protected abstract DbTransaction BeginDbTransaction(IsolationLevel isolationLevel);

		public DbTransaction BeginTransaction()
		{
			return this.BeginDbTransaction(IsolationLevel.Unspecified);
		}

		public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			return this.BeginDbTransaction(isolationLevel);
		}

		public abstract void ChangeDatabase(string databaseName);

		public abstract void Close();

		public DbCommand CreateCommand()
		{
			return this.CreateDbCommand();
		}

		protected abstract DbCommand CreateDbCommand();

		public virtual void EnlistTransaction(Transaction transaction)
		{
			throw new NotSupportedException();
		}

		public virtual DataTable GetSchema()
		{
			return DbConnection.MetaDataCollections.Instance;
		}

		public virtual DataTable GetSchema(string collectionName)
		{
			return this.GetSchema(collectionName, null);
		}

		private void AddParameter(DbCommand command, string parameterName, DbType parameterType, int parameterSize)
		{
			DbParameter dbParameter = command.CreateParameter();
			dbParameter.ParameterName = parameterName;
			dbParameter.DbType = parameterType;
			dbParameter.Size = parameterSize;
			command.Parameters.Add(dbParameter);
		}

		public virtual DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			if (collectionName == null)
			{
				throw new ArgumentException();
			}
			string text = null;
			DataTable instance = DbConnection.MetaDataCollections.Instance;
			int num = ((restrictionValues != null) ? restrictionValues.Length : 0);
			foreach (object obj in instance.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (string.Compare((string)dataRow["CollectionName"], collectionName, true) == 0)
				{
					if (num > (int)dataRow["NumberOfRestrictions"])
					{
						throw new ArgumentException("More restrictions were provided than the requested schema ('" + dataRow["CollectionName"].ToString() + "') supports");
					}
					text = dataRow["CollectionName"].ToString();
				}
			}
			if (text == null)
			{
				throw new ArgumentException("The requested collection ('" + collectionName + "') is not defined.");
			}
			DbCommand dbCommand = null;
			DataTable dataTable = new DataTable();
			string text2 = text;
			switch (text2)
			{
			case "Databases":
				dbCommand = this.CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select name as database_name, dbid, crdate as create_date from master.sys.sysdatabases where (name = @Name or (@Name is null))";
				this.AddParameter(dbCommand, "@Name", DbType.StringFixedLength, 4000);
				break;
			case "ForeignKeys":
				dbCommand = this.CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select CONSTRAINT_CATALOG, CONSTRAINT_SCHEMA, CONSTRAINT_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, CONSTRAINT_TYPE, IS_DEFERRABLE, INITIALLY_DEFERRED from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where (CONSTRAINT_CATALOG = @Catalog or (@Catalog is null)) and (CONSTRAINT_SCHEMA = @Owner or (@Owner is null)) and (TABLE_NAME = @Table or (@Table is null)) and (CONSTRAINT_NAME = @Name or (@Name is null)) and CONSTRAINT_TYPE = 'FOREIGN KEY' order by CONSTRAINT_CATALOG, CONSTRAINT_SCHEMA, CONSTRAINT_NAME";
				this.AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Table", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Name", DbType.StringFixedLength, 4000);
				break;
			case "Indexes":
				dbCommand = this.CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select distinct db_name() as constraint_catalog, constraint_schema = user_name (o.uid), constraint_name = x.name, table_catalog = db_name (), table_schema = user_name (o.uid), table_name = o.name, index_name  = x.name from sysobjects o, sysindexes x, sysindexkeys xk where o.type in ('U') and x.id = o.id and o.id = xk.id and x.indid = xk.indid and xk.keyno = x.keycnt and (db_name() = @Catalog or (@Catalog is null)) and (user_name() = @Owner or (@Owner is null)) and (o.name = @Table or (@Table is null)) and (x.name = @Name or (@Name is null))order by table_name, index_name";
				this.AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Table", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Name", DbType.StringFixedLength, 4000);
				break;
			case "IndexColumns":
				dbCommand = this.CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select distinct db_name() as constraint_catalog, constraint_schema = user_name (o.uid), constraint_name = x.name, table_catalog = db_name (), table_schema = user_name (o.uid), table_name = o.name, column_name = c.name, ordinal_position = convert (int, xk.keyno), keyType = c.xtype, index_name = x.name from sysobjects o, sysindexes x, syscolumns c, sysindexkeys xk where o.type in ('U') and x.id = o.id and o.id = c.id and o.id = xk.id and x.indid = xk.indid and c.colid = xk.colid and xk.keyno <= x.keycnt and permissions (o.id, c.name) <> 0 and (db_name() = @Catalog or (@Catalog is null)) and (user_name() = @Owner or (@Owner is null)) and (o.name = @Table or (@Table is null)) and (x.name = @ConstraintName or (@ConstraintName is null)) and (c.name = @Column or (@Column is null)) order by table_name, index_name";
				this.AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 8);
				this.AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Table", DbType.StringFixedLength, 13);
				this.AddParameter(dbCommand, "@ConstraintName", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Column", DbType.StringFixedLength, 4000);
				break;
			case "Procedures":
				dbCommand = this.CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ROUTINE_CATALOG, ROUTINE_SCHEMA, ROUTINE_NAME, ROUTINE_TYPE, CREATED, LAST_ALTERED from INFORMATION_SCHEMA.ROUTINES where (SPECIFIC_CATALOG = @Catalog or (@Catalog is null)) and (SPECIFIC_SCHEMA = @Owner or (@Owner is null)) and (SPECIFIC_NAME = @Name or (@Name is null)) and (ROUTINE_TYPE = @Type or (@Type is null)) order by SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME";
				this.AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Name", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Type", DbType.StringFixedLength, 4000);
				break;
			case "ProcedureParameters":
				dbCommand = this.CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ORDINAL_POSITION, PARAMETER_MODE, IS_RESULT, AS_LOCATOR, PARAMETER_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, COLLATION_CATALOG, COLLATION_SCHEMA, COLLATION_NAME, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME, NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE, DATETIME_PRECISION, INTERVAL_TYPE, INTERVAL_PRECISION from INFORMATION_SCHEMA.PARAMETERS where (SPECIFIC_CATALOG = @Catalog or (@Catalog is null)) and (SPECIFIC_SCHEMA = @Owner or (@Owner is null)) and (SPECIFIC_NAME = @Name or (@Name is null)) and (PARAMETER_NAME = @Parameter or (@Parameter is null)) order by SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, PARAMETER_NAME";
				this.AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Name", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Parameter", DbType.StringFixedLength, 4000);
				break;
			case "Tables":
				dbCommand = this.CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE from INFORMATION_SCHEMA.TABLES where (TABLE_CATALOG = @catalog or (@catalog is null)) and (TABLE_SCHEMA = @owner or (@owner is null))and (TABLE_NAME = @name or (@name is null)) and (TABLE_TYPE = @table_type or (@table_type is null))";
				this.AddParameter(dbCommand, "@catalog", DbType.StringFixedLength, 8);
				this.AddParameter(dbCommand, "@owner", DbType.StringFixedLength, 3);
				this.AddParameter(dbCommand, "@name", DbType.StringFixedLength, 11);
				this.AddParameter(dbCommand, "@table_type", DbType.StringFixedLength, 10);
				break;
			case "Columns":
				dbCommand = this.CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, ORDINAL_POSITION, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE, DATETIME_PRECISION, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME, COLLATION_CATALOG from INFORMATION_SCHEMA.COLUMNS where (TABLE_CATALOG = @Catalog or (@Catalog is null)) and (TABLE_SCHEMA = @Owner or (@Owner is null)) and (TABLE_NAME = @table or (@Table is null)) and (COLUMN_NAME = @column or (@Column is null)) order by TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME";
				this.AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Table", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Column", DbType.StringFixedLength, 4000);
				break;
			case "Users":
				dbCommand = this.CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select uid, name as user_name, createdate, updatedate from sysusers where (name = @Name or (@Name is null))";
				this.AddParameter(dbCommand, "@Name", DbType.StringFixedLength, 4000);
				break;
			case "Views":
				dbCommand = this.CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, CHECK_OPTION, IS_UPDATABLE from INFORMATION_SCHEMA.VIEWS where (TABLE_CATALOG = @Catalog or (@Catalog is null)) TABLE_SCHEMA = @Owner or (@Owner is null)) and (TABLE_NAME = @table or (@Table is null)) order by TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME";
				this.AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Table", DbType.StringFixedLength, 4000);
				break;
			case "ViewColumns":
				dbCommand = this.CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select VIEW_CATALOG, VIEW_SCHEMA, VIEW_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME from INFORMATION_SCHEMA.VIEW_COLUMN_USAGE where (VIEW_CATALOG = @Catalog (@Catalog is null)) and (VIEW_SCHEMA = @Owner (@Owner is null)) and (VIEW_NAME = @Table or (@Table is null)) and (COLUMN_NAME = @Column or (@Column is null)) order by VIEW_CATALOG, VIEW_SCHEMA, VIEW_NAME";
				this.AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Table", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@Column", DbType.StringFixedLength, 4000);
				break;
			case "UserDefinedTypes":
				dbCommand = this.CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select assemblies.name as assembly_name, types.assembly_class as udt_name, ASSEMBLYPROPERTY(assemblies.name, 'VersionMajor') as version_major, ASSEMBLYPROPERTY(assemblies.name, 'VersionMinor') as version_minor, ASSEMBLYPROPERTY(assemblies.name, 'VersionBuild') as version_build, ASSEMBLYPROPERTY(assemblies.name, 'VersionRevision') as version_revision, ASSEMBLYPROPERTY(assemblies.name, 'CultureInfo') as culture_info, ASSEMBLYPROPERTY(assemblies.name, 'PublicKey') as public_key, is_fixed_length, max_length, Create_Date, Permission_set_desc from sys.assemblies as assemblies join sys.assembly_types as types on assemblies.assembly_id = types.assembly_id where (assemblies.name = @AssemblyName or (@AssemblyName is null)) and (types.assembly_class = @UDTName or (@UDTName is null))";
				this.AddParameter(dbCommand, "@AssemblyName", DbType.StringFixedLength, 4000);
				this.AddParameter(dbCommand, "@UDTName", DbType.StringFixedLength, 4000);
				break;
			case "MetaDataCollections":
				return DbConnection.MetaDataCollections.Instance;
			case "DataSourceInformation":
				throw new NotImplementedException();
			case "DataTypes":
				return DbConnection.DataTypes.Instance;
			case "ReservedWords":
				return DbConnection.ReservedWords.Instance;
			case "Restrictions":
				return DbConnection.Restrictions.Instance;
			}
			for (int i = 0; i < num; i++)
			{
				dbCommand.Parameters[i].Value = restrictionValues[i];
			}
			DbDataAdapter dbDataAdapter = this.DbProviderFactory.CreateDataAdapter();
			dbDataAdapter.SelectCommand = dbCommand;
			dbDataAdapter.Fill(dataTable);
			return dataTable;
		}

		protected virtual DbProviderFactory DbProviderFactory
		{
			get
			{
				return DbProviderFactories.GetFactory(base.GetType().ToString());
			}
		}

		public abstract void Open();

		protected virtual void OnStateChange(StateChangeEventArgs stateChanged)
		{
			if (this.StateChange != null)
			{
				this.StateChange(this, stateChanged);
			}
		}

		private static class DataTypes
		{
			// Note: this type is marked as 'beforefieldinit'.
			static DataTypes()
			{
				object[][] array = new object[26][];
				int num = 0;
				object[] array2 = new object[22];
				array2[0] = "smallint";
				array2[1] = 16;
				array2[2] = 5;
				array2[3] = "smallint";
				array2[5] = "System.Int16";
				array2[6] = true;
				array2[7] = true;
				array2[8] = false;
				array2[9] = true;
				array2[10] = true;
				array2[11] = false;
				array2[12] = true;
				array2[13] = true;
				array2[14] = false;
				array2[15] = false;
				array2[18] = false;
				array[num] = array2;
				int num2 = 1;
				object[] array3 = new object[22];
				array3[0] = "int";
				array3[1] = 8;
				array3[2] = 10;
				array3[3] = "int";
				array3[5] = "System.Int32";
				array3[6] = true;
				array3[7] = true;
				array3[8] = false;
				array3[9] = true;
				array3[10] = true;
				array3[11] = false;
				array3[12] = true;
				array3[13] = true;
				array3[14] = false;
				array3[15] = false;
				array3[18] = false;
				array[num2] = array3;
				int num3 = 2;
				object[] array4 = new object[22];
				array4[0] = "real";
				array4[1] = 13;
				array4[2] = 7;
				array4[3] = "real";
				array4[5] = "System.Single";
				array4[6] = false;
				array4[7] = true;
				array4[8] = false;
				array4[9] = true;
				array4[10] = false;
				array4[11] = false;
				array4[12] = true;
				array4[13] = true;
				array4[14] = false;
				array4[15] = false;
				array4[18] = false;
				array[num3] = array4;
				int num4 = 3;
				object[] array5 = new object[22];
				array5[0] = "float";
				array5[1] = 6;
				array5[2] = 53;
				array5[3] = "float({0})";
				array5[4] = "number of bits used to store the mantissa";
				array5[5] = "System.Double";
				array5[6] = false;
				array5[7] = true;
				array5[8] = false;
				array5[9] = true;
				array5[10] = false;
				array5[11] = false;
				array5[12] = true;
				array5[13] = true;
				array5[14] = false;
				array5[15] = false;
				array5[18] = false;
				array[num4] = array5;
				int num5 = 4;
				object[] array6 = new object[22];
				array6[0] = "money";
				array6[1] = 9;
				array6[2] = 19;
				array6[3] = "money";
				array6[5] = "System.Decimal";
				array6[6] = false;
				array6[7] = false;
				array6[8] = false;
				array6[9] = true;
				array6[10] = true;
				array6[11] = false;
				array6[12] = true;
				array6[13] = true;
				array6[14] = false;
				array6[15] = false;
				array6[18] = false;
				array[num5] = array6;
				int num6 = 5;
				object[] array7 = new object[22];
				array7[0] = "smallmoney";
				array7[1] = 17;
				array7[2] = 10;
				array7[3] = "smallmoney";
				array7[5] = "System.Decimal";
				array7[6] = false;
				array7[7] = false;
				array7[8] = false;
				array7[9] = true;
				array7[10] = true;
				array7[11] = false;
				array7[12] = true;
				array7[13] = true;
				array7[14] = false;
				array7[15] = false;
				array7[18] = false;
				array[num6] = array7;
				int num7 = 6;
				object[] array8 = new object[22];
				array8[0] = "bit";
				array8[1] = 2;
				array8[2] = 1;
				array8[3] = "bit";
				array8[5] = "System.Boolean";
				array8[6] = false;
				array8[7] = false;
				array8[8] = false;
				array8[9] = true;
				array8[10] = false;
				array8[11] = false;
				array8[12] = true;
				array8[13] = true;
				array8[14] = false;
				array8[18] = false;
				array[num7] = array8;
				int num8 = 7;
				object[] array9 = new object[22];
				array9[0] = "tinyint";
				array9[1] = 20;
				array9[2] = 3;
				array9[3] = "tinyint";
				array9[5] = "System.SByte";
				array9[6] = true;
				array9[7] = true;
				array9[8] = false;
				array9[9] = true;
				array9[10] = true;
				array9[11] = false;
				array9[12] = true;
				array9[13] = true;
				array9[14] = false;
				array9[15] = true;
				array9[18] = false;
				array[num8] = array9;
				int num9 = 8;
				object[] array10 = new object[22];
				array10[0] = "bigint";
				array10[1] = 0;
				array10[2] = 19;
				array10[3] = "bigint";
				array10[5] = "System.Int64";
				array10[6] = true;
				array10[7] = true;
				array10[8] = false;
				array10[9] = true;
				array10[10] = true;
				array10[11] = false;
				array10[12] = true;
				array10[13] = true;
				array10[14] = false;
				array10[15] = false;
				array10[18] = false;
				array[num9] = array10;
				int num10 = 9;
				object[] array11 = new object[22];
				array11[0] = "timestamp";
				array11[1] = 19;
				array11[2] = 8;
				array11[3] = "timestamp";
				array11[5] = "System.Byte[]";
				array11[6] = false;
				array11[7] = false;
				array11[8] = false;
				array11[9] = true;
				array11[10] = false;
				array11[11] = false;
				array11[12] = false;
				array11[13] = true;
				array11[14] = false;
				array11[18] = true;
				array11[20] = "0x";
				array[num10] = array11;
				int num11 = 10;
				object[] array12 = new object[22];
				array12[0] = "binary";
				array12[1] = 1;
				array12[2] = 8000;
				array12[3] = "binary({0})";
				array12[4] = "length";
				array12[5] = "System.Byte[]";
				array12[6] = false;
				array12[7] = true;
				array12[8] = false;
				array12[9] = true;
				array12[10] = false;
				array12[11] = false;
				array12[12] = true;
				array12[13] = true;
				array12[14] = false;
				array12[18] = false;
				array12[20] = "0x";
				array[num11] = array12;
				int num12 = 11;
				object[] array13 = new object[22];
				array13[0] = "image";
				array13[1] = 7;
				array13[2] = int.MaxValue;
				array13[3] = "image";
				array13[5] = "System.Byte[]";
				array13[6] = false;
				array13[7] = true;
				array13[8] = false;
				array13[9] = false;
				array13[10] = false;
				array13[11] = true;
				array13[12] = true;
				array13[13] = false;
				array13[14] = false;
				array13[18] = false;
				array13[20] = "0x";
				array[num12] = array13;
				array[12] = new object[]
				{
					"text", 18, int.MaxValue, "text", null, "System.String", false, true, false, false,
					false, true, true, false, true, null, null, null, false, null,
					"'", "'"
				};
				array[13] = new object[]
				{
					"ntext", 11, 1073741823, "ntext", null, "System.String", false, true, false, false,
					false, true, true, false, true, null, null, null, false, null,
					"N'", "'"
				};
				int num13 = 14;
				object[] array14 = new object[22];
				array14[0] = "decimal";
				array14[1] = 5;
				array14[2] = 38;
				array14[3] = "decimal({0}, {1})";
				array14[4] = "precision,scale";
				array14[5] = "System.Decimal";
				array14[6] = true;
				array14[7] = true;
				array14[8] = false;
				array14[9] = true;
				array14[10] = false;
				array14[11] = false;
				array14[12] = true;
				array14[13] = true;
				array14[14] = false;
				array14[15] = false;
				array14[16] = 38;
				array14[17] = 0;
				array14[18] = false;
				array[num13] = array14;
				int num14 = 15;
				object[] array15 = new object[22];
				array15[0] = "numeric";
				array15[1] = 5;
				array15[2] = 38;
				array15[3] = "numeric({0}, {1})";
				array15[4] = "precision,scale";
				array15[5] = "System.Decimal";
				array15[6] = true;
				array15[7] = true;
				array15[8] = false;
				array15[9] = true;
				array15[10] = false;
				array15[11] = false;
				array15[12] = true;
				array15[13] = true;
				array15[14] = false;
				array15[15] = false;
				array15[16] = 38;
				array15[17] = 0;
				array15[18] = false;
				array[num14] = array15;
				array[16] = new object[]
				{
					"datetime", 4, 23, "datetime", null, "System.DateTime", false, true, false, true,
					false, false, true, true, true, null, null, null, false, null,
					"{ts '", "'}"
				};
				array[17] = new object[]
				{
					"smalldatetime", 15, 16, "smalldatetime", null, "System.DateTime", false, true, false, true,
					false, false, true, true, true, null, null, null, false, null,
					"{ts '", "'}"
				};
				int num15 = 18;
				object[] array16 = new object[22];
				array16[0] = "sql_variant";
				array16[1] = 23;
				array16[3] = "sql_variant";
				array16[5] = "System.Object";
				array16[6] = false;
				array16[7] = true;
				array16[8] = false;
				array16[9] = false;
				array16[10] = false;
				array16[11] = false;
				array16[12] = true;
				array16[13] = true;
				array16[14] = false;
				array16[18] = false;
				array16[19] = false;
				array[num15] = array16;
				int num16 = 19;
				object[] array17 = new object[22];
				array17[0] = "xml";
				array17[1] = 25;
				array17[2] = int.MaxValue;
				array17[3] = "xml";
				array17[5] = "System.String";
				array17[6] = false;
				array17[7] = false;
				array17[8] = false;
				array17[9] = false;
				array17[10] = false;
				array17[11] = true;
				array17[12] = true;
				array17[13] = false;
				array17[14] = false;
				array17[18] = false;
				array17[19] = false;
				array[num16] = array17;
				array[20] = new object[]
				{
					"varchar", 22, int.MaxValue, "varchar({0})", "max length", "System.String", false, true, false, false,
					false, false, true, true, true, null, null, null, false, null,
					"'", "'"
				};
				array[21] = new object[]
				{
					"char", 3, int.MaxValue, "char({0})", "length", "System.String", false, true, false, true,
					false, false, true, true, true, null, null, null, false, null,
					"'", "'"
				};
				array[22] = new object[]
				{
					"nchar", 10, 1073741823, "nchar({0})", "length", "System.String", false, true, false, true,
					false, false, true, true, true, null, null, null, false, null,
					"N'", "'"
				};
				array[23] = new object[]
				{
					"nvarchar", 12, 1073741823, "nvarchar({0})", "max length", "System.String", false, true, false, false,
					false, false, true, true, true, null, null, null, false, null,
					"N'", "'"
				};
				int num17 = 24;
				object[] array18 = new object[22];
				array18[0] = "varbinary";
				array18[1] = 21;
				array18[2] = 1073741823;
				array18[3] = "varbinary({0})";
				array18[4] = "max length";
				array18[5] = "System.Byte[]";
				array18[6] = false;
				array18[7] = true;
				array18[8] = false;
				array18[9] = false;
				array18[10] = false;
				array18[11] = false;
				array18[12] = true;
				array18[13] = true;
				array18[14] = false;
				array18[18] = false;
				array18[20] = "0x";
				array[num17] = array18;
				array[25] = new object[]
				{
					"uniqueidentifier", 14, 16, "uniqueidentifier", null, "System.Guid", false, true, false, true,
					false, false, true, true, false, null, null, null, false, null,
					"'", "'"
				};
				DbConnection.DataTypes.rows = array;
			}

			public static DataTable Instance
			{
				get
				{
					if (DbConnection.DataTypes.instance == null)
					{
						DbConnection.DataTypes.instance = new DataTable("DataTypes");
						foreach (DbConnection.ColumnInfo columnInfo in DbConnection.DataTypes.columns)
						{
							DbConnection.DataTypes.instance.Columns.Add(columnInfo.name, columnInfo.type);
						}
						foreach (object[] array3 in DbConnection.DataTypes.rows)
						{
							DbConnection.DataTypes.instance.LoadDataRow(array3, true);
						}
					}
					return DbConnection.DataTypes.instance;
				}
			}

			private static readonly DbConnection.ColumnInfo[] columns = new DbConnection.ColumnInfo[]
			{
				new DbConnection.ColumnInfo("TypeName", typeof(string)),
				new DbConnection.ColumnInfo("ProviderDbType", typeof(int)),
				new DbConnection.ColumnInfo("ColumnSize", typeof(long)),
				new DbConnection.ColumnInfo("CreateFormat", typeof(string)),
				new DbConnection.ColumnInfo("CreateParameters", typeof(string)),
				new DbConnection.ColumnInfo("DataType", typeof(string)),
				new DbConnection.ColumnInfo("IsAutoIncrementable", typeof(bool)),
				new DbConnection.ColumnInfo("IsBestMatch", typeof(bool)),
				new DbConnection.ColumnInfo("IsCaseSensitive", typeof(bool)),
				new DbConnection.ColumnInfo("IsFixedLength", typeof(bool)),
				new DbConnection.ColumnInfo("IsFixedPrecisionScale", typeof(bool)),
				new DbConnection.ColumnInfo("IsLong", typeof(bool)),
				new DbConnection.ColumnInfo("IsNullable", typeof(bool)),
				new DbConnection.ColumnInfo("IsSearchable", typeof(bool)),
				new DbConnection.ColumnInfo("IsSearchableWithLike", typeof(bool)),
				new DbConnection.ColumnInfo("IsUnsigned", typeof(bool)),
				new DbConnection.ColumnInfo("MaximumScale", typeof(short)),
				new DbConnection.ColumnInfo("MinimumScale", typeof(short)),
				new DbConnection.ColumnInfo("IsConcurrencyType", typeof(bool)),
				new DbConnection.ColumnInfo("IsLiteralSupported", typeof(bool)),
				new DbConnection.ColumnInfo("LiteralPrefix", typeof(string)),
				new DbConnection.ColumnInfo("LiteralSuffix", typeof(string))
			};

			private static readonly object[][] rows;

			private static DataTable instance;
		}

		private struct ColumnInfo
		{
			public ColumnInfo(string name, Type type)
			{
				this.name = name;
				this.type = type;
			}

			public string name;

			public Type type;
		}

		internal static class MetaDataCollections
		{
			public static DataTable Instance
			{
				get
				{
					if (DbConnection.MetaDataCollections.instance == null)
					{
						DbConnection.MetaDataCollections.instance = new DataTable("GetSchema");
						foreach (DbConnection.ColumnInfo columnInfo in DbConnection.MetaDataCollections.columns)
						{
							DbConnection.MetaDataCollections.instance.Columns.Add(columnInfo.name, columnInfo.type);
						}
						foreach (object[] array3 in DbConnection.MetaDataCollections.rows)
						{
							DbConnection.MetaDataCollections.instance.LoadDataRow(array3, true);
						}
					}
					return DbConnection.MetaDataCollections.instance;
				}
			}

			private static readonly DbConnection.ColumnInfo[] columns = new DbConnection.ColumnInfo[]
			{
				new DbConnection.ColumnInfo("CollectionName", typeof(string)),
				new DbConnection.ColumnInfo("NumberOfRestrictions", typeof(int)),
				new DbConnection.ColumnInfo("NumberOfIdentifierParts", typeof(int))
			};

			private static readonly object[][] rows = new object[][]
			{
				new object[] { "MetaDataCollections", 0, 0 },
				new object[] { "DataSourceInformation", 0, 0 },
				new object[] { "DataTypes", 0, 0 },
				new object[] { "Restrictions", 0, 0 },
				new object[] { "ReservedWords", 0, 0 },
				new object[] { "Users", 1, 1 },
				new object[] { "Databases", 1, 1 },
				new object[] { "Tables", 4, 3 },
				new object[] { "Columns", 4, 4 },
				new object[] { "Views", 3, 3 },
				new object[] { "ViewColumns", 4, 4 },
				new object[] { "ProcedureParameters", 4, 1 },
				new object[] { "Procedures", 4, 3 },
				new object[] { "ForeignKeys", 4, 3 },
				new object[] { "IndexColumns", 5, 4 },
				new object[] { "Indexes", 4, 3 },
				new object[] { "UserDefinedTypes", 2, 1 }
			};

			private static DataTable instance;
		}

		private static class Restrictions
		{
			public static DataTable Instance
			{
				get
				{
					if (DbConnection.Restrictions.instance == null)
					{
						DbConnection.Restrictions.instance = new DataTable("Restrictions");
						foreach (DbConnection.ColumnInfo columnInfo in DbConnection.Restrictions.columns)
						{
							DbConnection.Restrictions.instance.Columns.Add(columnInfo.name, columnInfo.type);
						}
						foreach (object[] array3 in DbConnection.Restrictions.rows)
						{
							DbConnection.Restrictions.instance.LoadDataRow(array3, true);
						}
					}
					return DbConnection.Restrictions.instance;
				}
			}

			private static readonly DbConnection.ColumnInfo[] columns = new DbConnection.ColumnInfo[]
			{
				new DbConnection.ColumnInfo("CollectionName", typeof(string)),
				new DbConnection.ColumnInfo("RestrictionName", typeof(string)),
				new DbConnection.ColumnInfo("ParameterName", typeof(string)),
				new DbConnection.ColumnInfo("RestrictionDefault", typeof(string)),
				new DbConnection.ColumnInfo("RestrictionNumber", typeof(int))
			};

			private static readonly object[][] rows = new object[][]
			{
				new object[] { "Users", "User_Name", "@Name", "name", 1 },
				new object[] { "Databases", "Name", "@Name", "Name", 1 },
				new object[] { "Tables", "Catalog", "@Catalog", "TABLE_CATALOG", 1 },
				new object[] { "Tables", "Owner", "@Owner", "TABLE_SCHEMA", 2 },
				new object[] { "Tables", "Table", "@Name", "TABLE_NAME", 3 },
				new object[] { "Tables", "TableType", "@TableType", "TABLE_TYPE", 4 },
				new object[] { "Columns", "Catalog", "@Catalog", "TABLE_CATALOG", 1 },
				new object[] { "Columns", "Owner", "@Owner", "TABLE_SCHEMA", 2 },
				new object[] { "Columns", "Table", "@Table", "TABLE_NAME", 3 },
				new object[] { "Columns", "Column", "@Column", "COLUMN_NAME", 4 },
				new object[] { "Views", "Catalog", "@Catalog", "TABLE_CATALOG", 1 },
				new object[] { "Views", "Owner", "@Owner", "TABLE_SCHEMA", 2 },
				new object[] { "Views", "Table", "@Table", "TABLE_NAME", 3 },
				new object[] { "ViewColumns", "Catalog", "@Catalog", "VIEW_CATALOG", 1 },
				new object[] { "ViewColumns", "Owner", "@Owner", "VIEW_SCHEMA", 2 },
				new object[] { "ViewColumns", "Table", "@Table", "VIEW_NAME", 3 },
				new object[] { "ViewColumns", "Column", "@Column", "COLUMN_NAME", 4 },
				new object[] { "ProcedureParameters", "Catalog", "@Catalog", "SPECIFIC_CATALOG", 1 },
				new object[] { "ProcedureParameters", "Owner", "@Owner", "SPECIFIC_SCHEMA", 2 },
				new object[] { "ProcedureParameters", "Name", "@Name", "SPECIFIC_NAME", 3 },
				new object[] { "ProcedureParameters", "Parameter", "@Parameter", "PARAMETER_NAME", 4 },
				new object[] { "Procedures", "Catalog", "@Catalog", "SPECIFIC_CATALOG", 1 },
				new object[] { "Procedures", "Owner", "@Owner", "SPECIFIC_SCHEMA", 2 },
				new object[] { "Procedures", "Name", "@Name", "SPECIFIC_NAME", 3 },
				new object[] { "Procedures", "Type", "@Type", "ROUTINE_TYPE", 4 },
				new object[] { "IndexColumns", "Catalog", "@Catalog", "db_name(}", 1 },
				new object[] { "IndexColumns", "Owner", "@Owner", "user_name(}", 2 },
				new object[] { "IndexColumns", "Table", "@Table", "o.name", 3 },
				new object[] { "IndexColumns", "ConstraintName", "@ConstraintName", "x.name", 4 },
				new object[] { "IndexColumns", "Column", "@Column", "c.name", 5 },
				new object[] { "Indexes", "Catalog", "@Catalog", "db_name(}", 1 },
				new object[] { "Indexes", "Owner", "@Owner", "user_name(}", 2 },
				new object[] { "Indexes", "Table", "@Table", "o.name", 3 },
				new object[] { "Indexes", "Name", "@Name", "x.name", 4 },
				new object[] { "UserDefinedTypes", "assembly_name", "@AssemblyName", "assemblies.name", 1 },
				new object[] { "UserDefinedTypes", "udt_name", "@UDTName", "types.assembly_class", 2 },
				new object[] { "ForeignKeys", "Catalog", "@Catalog", "CONSTRAINT_CATALOG", 1 },
				new object[] { "ForeignKeys", "Owner", "@Owner", "CONSTRAINT_SCHEMA", 2 },
				new object[] { "ForeignKeys", "Table", "@Table", "TABLE_NAME", 3 },
				new object[] { "ForeignKeys", "Name", "@Name", "CONSTRAINT_NAME", 4 }
			};

			private static DataTable instance;
		}

		private static class ReservedWords
		{
			public static DataTable Instance
			{
				get
				{
					if (DbConnection.ReservedWords.instance == null)
					{
						DbConnection.ReservedWords.instance = new DataTable("ReservedWords");
						DbConnection.ReservedWords.instance.Columns.Add("ReservedWord", typeof(string));
						foreach (string text in DbConnection.ReservedWords.reservedWords)
						{
							DataRow dataRow = DbConnection.ReservedWords.instance.NewRow();
							dataRow["ReservedWord"] = text;
							DbConnection.ReservedWords.instance.Rows.Add(dataRow);
						}
					}
					return DbConnection.ReservedWords.instance;
				}
			}

			private static readonly string[] reservedWords = new string[]
			{
				"ADD", "EXCEPT", "PERCENT", "ALL", "EXEC", "PLAN", "ALTER", "EXECUTE", "PRECISION", "AND",
				"EXISTS", "PRIMARY", "ANY", "EXIT", "PRINT", "AS", "FETCH", "PROC", "ASC", "FILE",
				"PROCEDURE", "AUTHORIZATION", "FILLFACTOR", "PUBLIC", "BACKUP", "FOR", "RAISERROR", "BEGIN", "FOREIGN", "READ",
				"BETWEEN", "FREETEXT", "READTEXT", "BREAK", "FREETEXTTABLE", "RECONFIGURE", "BROWSE", "FROM", "REFERENCES", "BULK",
				"FULL", "REPLICATION", "BY", "FUNCTION", "RESTORE", "CASCADE", "GOTO", "RESTRICT", "CASE", "GRANT",
				"RETURN", "CHECK", "GROUP", "REVOKE", "CHECKPOINT", "HAVING", "RIGHT", "CLOSE", "HOLDLOCK", "ROLLBACK",
				"CLUSTERED", "IDENTITY", "ROWCOUNT", "COALESCE", "IDENTITY_INSERT", "ROWGUIDCOL", "COLLATE", "IDENTITYCOL", "RULE", "COLUMN",
				"IF", "SAVE", "COMMIT", "IN", "SCHEMA", "COMPUTE", "INDEX", "SELECT", "CONSTRAINT", "INNER",
				"SESSION_USER", "CONTAINS", "INSERT", "SET", "CONTAINSTABLE", "INTERSECT", "SETUSER", "CONTINUE", "INTO", "SHUTDOWN",
				"CONVERT", "IS", "SOME", "CREATE", "JOIN", "STATISTICS", "CROSS", "KEY", "SYSTEM_USER", "CURRENT",
				"KILL", "TABLE", "CURRENT_DATE", "LEFT", "TEXTSIZE", "CURRENT_TIME", "LIKE", "THEN", "CURRENT_TIMESTAMP", "LINENO",
				"TO", "CURRENT_USER", "LOAD", "TOP", "CURSOR", "NATIONAL", "TRAN", "DATABASE", "NOCHECK", "TRANSACTION",
				"DBCC", "NONCLUSTERED", "TRIGGER", "DEALLOCATE", "NOT", "TRUNCATE", "DECLARE", "NULL", "TSEQUAL", "DEFAULT",
				"NULLIF", "UNION", "DELETE", "OF", "UNIQUE", "DENY", "OFF", "UPDATE", "DESC", "OFFSETS",
				"UPDATETEXT", "DISK", "ON", "USE", "DISTINCT", "OPEN", "USER", "DISTRIBUTED", "OPENDATASOURCE", "VALUES",
				"DOUBLE", "OPENQUERY", "VARYING", "DROP", "OPENROWSET", "VIEW", "DUMMY", "OPENXML", "WAITFOR", "DUMP",
				"OPTION", "WHEN", "ELSE", "OR", "WHERE", "END", "ORDER", "WHILE", "ERRLVL", "OUTER",
				"WITH", "ESCAPE", "OVER", "WRITETEXT", "ABSOLUTE", "FOUND", "PRESERVE", "ACTION", "FREE", "PRIOR",
				"ADMIN", "GENERAL", "PRIVILEGES", "AFTER", "GET", "READS", "AGGREGATE", "GLOBAL", "REAL", "ALIAS",
				"GO", "RECURSIVE", "ALLOCATE", "GROUPING", "REF", "ARE", "HOST", "REFERENCING", "ARRAY", "HOUR",
				"RELATIVE", "ASSERTION", "IGNORE", "RESULT", "AT", "IMMEDIATE", "RETURNS", "BEFORE", "INDICATOR", "ROLE",
				"BINARY", "INITIALIZE", "ROLLUP", "BIT", "INITIALLY", "ROUTINE", "BLOB", "INOUT", "ROW", "BOOLEAN",
				"INPUT", "ROWS", "BOTH", "INT", "SAVEPOINT", "BREADTH", "INTEGER", "SCROLL", "CALL", "INTERVAL",
				"SCOPE", "CASCADED", "ISOLATION", "SEARCH", "CAST", "ITERATE", "SECOND", "CATALOG", "LANGUAGE", "SECTION",
				"CHAR", "LARGE", "SEQUENCE", "CHARACTER", "LAST", "SESSION", "CLASS", "LATERAL", "SETS", "CLOB",
				"LEADING", "SIZE", "COLLATION", "LESS", "SMALLINT", "COMPLETION", "LEVEL", "SPACE", "CONNECT", "LIMIT",
				"SPECIFIC", "CONNECTION", "LOCAL", "SPECIFICTYPE", "CONSTRAINTS", "LOCALTIME", "SQL", "CONSTRUCTOR", "LOCALTIMESTAMP", "SQLEXCEPTION",
				"CORRESPONDING", "LOCATOR", "SQLSTATE", "CUBE", "MAP", "SQLWARNING", "CURRENT_PATH", "MATCH", "START", "CURRENT_ROLE",
				"MINUTE", "STATE", "CYCLE", "MODIFIES", "STATEMENT", "DATA", "MODIFY", "STATIC", "DATE", "MODULE",
				"STRUCTURE", "DAY", "MONTH", "TEMPORARY", "DEC", "NAMES", "TERMINATE", "DECIMAL", "NATURAL", "THAN",
				"DEFERRABLE", "NCHAR", "TIME", "DEFERRED", "NCLOB", "TIMESTAMP", "DEPTH", "NEW", "TIMEZONE_HOUR", "DEREF",
				"NEXT", "TIMEZONE_MINUTE", "DESCRIBE", "NO", "TRAILING", "DESCRIPTOR", "NONE", "TRANSLATION", "DESTROY", "NUMERIC",
				"TREAT", "DESTRUCTOR", "OBJECT", "TRUE", "DETERMINISTIC", "OLD", "UNDER", "DICTIONARY", "ONLY", "UNKNOWN",
				"DIAGNOSTICS", "OPERATION", "UNNEST", "DISCONNECT", "ORDINALITY", "USAGE", "DOMAIN", "OUT", "USING", "DYNAMIC",
				"OUTPUT", "VALUE", "EACH", "PAD", "VARCHAR", "END-EXEC", "PARAMETER", "VARIABLE", "EQUALS", "PARAMETERS",
				"WHENEVER", "EVERY", "PARTIAL", "WITHOUT", "EXCEPTION", "PATH", "WORK", "EXTERNAL", "POSTFIX", "WRITE",
				"FALSE", "PREFIX", "YEAR", "FIRST", "PREORDER", "ZONE", "FLOAT", "PREPARE", "ADA", "AVG",
				"BIT_LENGTH", "CHAR_LENGTH", "CHARACTER_LENGTH", "COUNT", "EXTRACT", "FORTRAN", "INCLUDE", "INSENSITIVE", "LOWER", "MAX",
				"MIN", "OCTET_LENGTH", "OVERLAPS", "PASCAL", "POSITION", "SQLCA", "SQLCODE", "SQLERROR", "SUBSTRING", "SUM",
				"TRANSLATE", "TRIM", "UPPER"
			};

			private static DataTable instance;
		}
	}
}
