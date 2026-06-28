using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Mono.Data.Tds.Protocol
{
	[MonoTODO("FIXME: Can packetsize be anything other than 512?")]
	public sealed class Tds50 : Tds
	{
		public Tds50(string server, int port)
			: this(server, port, 512, 15)
		{
		}

		public Tds50(string server, int port, int packetSize, int timeout)
			: base(server, port, packetSize, timeout, Tds50.Version)
		{
			this.packetSize = packetSize;
		}

		public string BuildExec(string sql)
		{
			if (base.Parameters == null || base.Parameters.Count == 0)
			{
				return sql;
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			int num = 0;
			foreach (object obj in ((IEnumerable)base.Parameters))
			{
				TdsMetaParameter tdsMetaParameter = (TdsMetaParameter)obj;
				stringBuilder3.Append(string.Format("declare {0}\n", tdsMetaParameter.Prepare()));
				stringBuilder2.Append(string.Format("select {0}=", tdsMetaParameter.ParameterName));
				if (tdsMetaParameter.Direction == TdsParameterDirection.Input)
				{
					stringBuilder2.Append(this.FormatParameter(tdsMetaParameter));
				}
				else
				{
					stringBuilder2.Append("NULL");
					stringBuilder.Append(tdsMetaParameter.ParameterName);
					if (num == 0)
					{
						stringBuilder.Append("select ");
					}
					else
					{
						stringBuilder.Append(", ");
					}
					num++;
				}
				stringBuilder2.Append("\n");
			}
			return string.Format("{0}{1}{2}\n{3}", new object[]
			{
				stringBuilder3.ToString(),
				stringBuilder2.ToString(),
				sql,
				stringBuilder.ToString()
			});
		}

		public override bool Connect(TdsConnectionParameters connectionParameters)
		{
			if (base.IsConnected)
			{
				throw new InvalidOperationException("The connection is already open.");
			}
			byte[] array = new byte[] { 3, 239, 101, 65, byte.MaxValue, byte.MaxValue, byte.MaxValue, 214 };
			byte[] array2 = new byte[] { 0, 0, 0, 6, 72, 0, 0, 8 };
			base.SetCharset(connectionParameters.Charset);
			base.SetLanguage(connectionParameters.Language);
			byte b = 0;
			byte[] array3 = new byte[0];
			base.Comm.StartPacket(TdsPacketType.Logon);
			byte[] array4 = base.Comm.Append(connectionParameters.Hostname, 30, b);
			base.Comm.Append((byte)((array4.Length >= 30) ? 30 : array4.Length));
			array4 = base.Comm.Append(connectionParameters.User, 30, b);
			base.Comm.Append((byte)((array4.Length >= 30) ? 30 : array4.Length));
			array4 = base.Comm.Append(connectionParameters.Password, 30, b);
			base.Comm.Append((byte)((array4.Length >= 30) ? 30 : array4.Length));
			array4 = base.Comm.Append("37876", 30, b);
			base.Comm.Append((byte)((array4.Length >= 30) ? 30 : array4.Length));
			base.Comm.Append(3);
			base.Comm.Append(1);
			base.Comm.Append(6);
			base.Comm.Append(10);
			base.Comm.Append(9);
			base.Comm.Append(1);
			base.Comm.Append(1);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(array3, 7, b);
			array4 = base.Comm.Append(connectionParameters.ApplicationName, 30, b);
			base.Comm.Append((byte)((array4.Length >= 30) ? 30 : array4.Length));
			array4 = base.Comm.Append(base.DataSource, 30, b);
			base.Comm.Append((byte)((array4.Length >= 30) ? 30 : array4.Length));
			base.Comm.Append(array3, 2, b);
			array4 = base.Comm.Append(connectionParameters.Password, 253, b);
			base.Comm.Append((byte)((array4.Length >= 253) ? 255 : (array4.Length + 2)));
			base.Comm.Append(5);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(0);
			array4 = base.Comm.Append(connectionParameters.ProgName, 10, b);
			base.Comm.Append((byte)((array4.Length >= 10) ? 10 : array4.Length));
			base.Comm.Append(6);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(13);
			base.Comm.Append(17);
			array4 = base.Comm.Append(base.Language, 30, b);
			base.Comm.Append((byte)((array4.Length >= 30) ? 30 : array4.Length));
			base.Comm.Append(1);
			base.Comm.Append(0);
			base.Comm.Append(array3, 8, b);
			base.Comm.Append(0);
			base.Comm.Append(0);
			array4 = base.Comm.Append(base.Charset, 30, b);
			base.Comm.Append((byte)((array4.Length >= 30) ? 30 : array4.Length));
			base.Comm.Append(1);
			array4 = base.Comm.Append(this.packetSize.ToString(), 6, b);
			base.Comm.Append((byte)((array4.Length >= 6) ? 6 : array4.Length));
			base.Comm.Append(array3, 8, b);
			base.Comm.Append(226);
			base.Comm.Append(20);
			base.Comm.Append(1);
			base.Comm.Append(array);
			base.Comm.Append(2);
			base.Comm.Append(array2);
			base.Comm.SendPacket();
			base.MoreResults = true;
			base.SkipToEnd();
			return base.IsConnected;
		}

		public override void ExecPrepared(string id, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.Parameters = parameters;
			bool flag = base.Parameters != null && base.Parameters.Count > 0;
			base.Comm.StartPacket(TdsPacketType.Normal);
			base.Comm.Append(231);
			base.Comm.Append((short)(id.Length + 5));
			base.Comm.Append(2);
			base.Comm.Append((!flag) ? 0 : 1);
			base.Comm.Append((byte)id.Length);
			base.Comm.Append(id);
			base.Comm.Append(0);
			if (flag)
			{
				this.SendParamFormat();
				this.SendParams();
			}
			base.MoreResults = true;
			base.Comm.SendPacket();
			base.CheckForData(timeout);
			if (!wantResults)
			{
				base.SkipToEnd();
			}
		}

		public override void Execute(string sql, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.Parameters = parameters;
			string text = this.BuildExec(sql);
			base.ExecuteQuery(text, timeout, wantResults);
		}

		public override void ExecProc(string commandText, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.Parameters = parameters;
			base.ExecuteQuery(this.BuildProcedureCall(commandText), timeout, wantResults);
		}

		private string BuildProcedureCall(string procedure)
		{
			string text = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			int num = 0;
			if (base.Parameters != null)
			{
				foreach (object obj in ((IEnumerable)base.Parameters))
				{
					TdsMetaParameter tdsMetaParameter = (TdsMetaParameter)obj;
					if (tdsMetaParameter.Direction != TdsParameterDirection.Input)
					{
						if (num == 0)
						{
							stringBuilder2.Append("select ");
						}
						else
						{
							stringBuilder2.Append(", ");
						}
						stringBuilder2.Append(tdsMetaParameter.ParameterName);
						stringBuilder.Append(string.Format("declare {0}\n", tdsMetaParameter.Prepare()));
						if (tdsMetaParameter.Direction != TdsParameterDirection.ReturnValue)
						{
							if (tdsMetaParameter.Direction == TdsParameterDirection.InputOutput)
							{
								stringBuilder3.Append(string.Format("set {0}\n", this.FormatParameter(tdsMetaParameter)));
							}
							else
							{
								stringBuilder3.Append(string.Format("set {0}=NULL\n", tdsMetaParameter.ParameterName));
							}
						}
						num++;
					}
					if (tdsMetaParameter.Direction == TdsParameterDirection.ReturnValue)
					{
						text = tdsMetaParameter.ParameterName + "=";
					}
				}
			}
			text = "exec " + text;
			return string.Format("{0}{1}{2}{3} {4}\n{5}", new object[]
			{
				stringBuilder.ToString(),
				stringBuilder3.ToString(),
				text,
				procedure,
				this.BuildParameters(),
				stringBuilder2.ToString()
			});
		}

		private string BuildParameters()
		{
			if (base.Parameters == null || base.Parameters.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in ((IEnumerable)base.Parameters))
			{
				TdsMetaParameter tdsMetaParameter = (TdsMetaParameter)obj;
				if (tdsMetaParameter.Direction != TdsParameterDirection.ReturnValue)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					if (tdsMetaParameter.Direction == TdsParameterDirection.InputOutput)
					{
						stringBuilder.Append(string.Format("{0}={0} output", tdsMetaParameter.ParameterName));
					}
					else
					{
						stringBuilder.Append(this.FormatParameter(tdsMetaParameter));
					}
				}
			}
			return stringBuilder.ToString();
		}

		private string FormatParameter(TdsMetaParameter parameter)
		{
			if (parameter.Direction == TdsParameterDirection.Output)
			{
				return string.Format("{0} output", parameter.ParameterName);
			}
			if (parameter.Value == null || parameter.Value == DBNull.Value)
			{
				return "NULL";
			}
			string typeName = parameter.TypeName;
			switch (typeName)
			{
			case "smalldatetime":
			case "datetime":
			{
				DateTime dateTime = (DateTime)parameter.Value;
				return string.Format(CultureInfo.InvariantCulture, "'{0:MMM dd yyyy hh:mm:ss tt}'", new object[] { dateTime });
			}
			case "bigint":
			case "decimal":
			case "float":
			case "int":
			case "money":
			case "real":
			case "smallint":
			case "smallmoney":
			case "tinyint":
				return parameter.Value.ToString();
			case "nvarchar":
			case "nchar":
				return string.Format("N'{0}'", parameter.Value.ToString().Replace("'", "''"));
			case "uniqueidentifier":
				return string.Format("0x{0}", ((Guid)parameter.Value).ToString("N"));
			case "bit":
				if (parameter.Value.GetType() == typeof(bool))
				{
					return (!(bool)parameter.Value) ? "0x0" : "0x1";
				}
				return parameter.Value.ToString();
			case "image":
			case "binary":
			case "varbinary":
				return string.Format("0x{0}", BitConverter.ToString((byte[])parameter.Value).Replace("-", string.Empty).ToLower());
			}
			return string.Format("'{0}'", parameter.Value.ToString().Replace("'", "''"));
		}

		public override string Prepare(string sql, TdsMetaParameterCollection parameters)
		{
			base.Parameters = parameters;
			Random random = new Random();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 25; i++)
			{
				stringBuilder.Append((char)(random.Next(26) + 65));
			}
			string text = stringBuilder.ToString();
			sql = string.Format("create proc {0} as\n{1}", text, sql);
			short num = (short)(text.Length + sql.Length + 5);
			base.Comm.StartPacket(TdsPacketType.Normal);
			base.Comm.Append(231);
			base.Comm.Append(num);
			base.Comm.Append(1);
			base.Comm.Append(0);
			base.Comm.Append((byte)text.Length);
			base.Comm.Append(text);
			base.Comm.Append((short)sql.Length);
			base.Comm.Append(sql);
			base.Comm.SendPacket();
			base.MoreResults = true;
			base.SkipToEnd();
			return text;
		}

		protected override void ProcessColumnInfo()
		{
			this.isSelectQuery = true;
			base.Comm.GetTdsShort();
			int tdsShort = (int)base.Comm.GetTdsShort();
			for (int i = 0; i < tdsShort; i++)
			{
				string @string = base.Comm.GetString((int)base.Comm.GetByte());
				int @byte = (int)base.Comm.GetByte();
				bool flag = (@byte & 1) > 0;
				bool flag2 = (@byte & 2) > 0;
				bool flag3 = (@byte & 4) > 0;
				bool flag4 = (@byte & 16) > 0;
				bool flag5 = (@byte & 32) > 0;
				bool flag6 = (@byte & 64) > 0;
				base.Comm.Skip(4L);
				byte byte2 = base.Comm.GetByte();
				bool flag7 = byte2 == 36;
				TdsColumnType tdsColumnType = (TdsColumnType)byte2;
				byte b = 0;
				byte b2 = 0;
				int num;
				if (tdsColumnType == TdsColumnType.Text || tdsColumnType == TdsColumnType.Image)
				{
					num = base.Comm.GetTdsInt();
					base.Comm.Skip((long)base.Comm.GetTdsShort());
				}
				else if (Tds.IsFixedSizeColumn(tdsColumnType))
				{
					num = Tds.LookupBufferSize(tdsColumnType);
				}
				else
				{
					num = (int)base.Comm.GetByte();
				}
				if (tdsColumnType == TdsColumnType.Decimal || tdsColumnType == TdsColumnType.Numeric)
				{
					b = base.Comm.GetByte();
					b2 = base.Comm.GetByte();
				}
				base.Comm.Skip((long)base.Comm.GetByte());
				if (flag7)
				{
					base.Comm.Skip((long)base.Comm.GetTdsShort());
				}
				TdsDataColumn tdsDataColumn = new TdsDataColumn();
				base.Columns.Add(tdsDataColumn);
				tdsDataColumn.ColumnType = new TdsColumnType?(tdsColumnType);
				tdsDataColumn.ColumnName = @string;
				tdsDataColumn.IsIdentity = new bool?(flag6);
				tdsDataColumn.IsRowVersion = new bool?(flag3);
				tdsDataColumn.ColumnType = new TdsColumnType?(tdsColumnType);
				tdsDataColumn.ColumnSize = new int?(num);
				tdsDataColumn.NumericPrecision = new short?((short)b);
				tdsDataColumn.NumericScale = new short?((short)b2);
				tdsDataColumn.IsReadOnly = new bool?(!flag4);
				tdsDataColumn.IsKey = new bool?(flag2);
				tdsDataColumn.AllowDBNull = new bool?(flag5);
				tdsDataColumn.IsHidden = new bool?(flag);
			}
		}

		private void SendParamFormat()
		{
			base.Comm.Append(236);
			int num = 2 + 8 * base.Parameters.Count;
			foreach (object obj in ((IEnumerable)base.Parameters))
			{
				TdsMetaParameter tdsMetaParameter = (TdsMetaParameter)obj;
				TdsColumnType tdsColumnType = tdsMetaParameter.GetMetaType();
				if (!Tds.IsFixedSizeColumn(tdsColumnType))
				{
					num++;
				}
				if (tdsColumnType == TdsColumnType.Numeric || tdsColumnType == TdsColumnType.Decimal)
				{
					num += 2;
				}
			}
			base.Comm.Append((short)num);
			base.Comm.Append((short)base.Parameters.Count);
			foreach (object obj2 in ((IEnumerable)base.Parameters))
			{
				TdsMetaParameter tdsMetaParameter2 = (TdsMetaParameter)obj2;
				string empty = string.Empty;
				string empty2 = string.Empty;
				int num2 = 0;
				byte b = 0;
				if (tdsMetaParameter2.IsNullable)
				{
					b |= 32;
				}
				if (tdsMetaParameter2.Direction == TdsParameterDirection.Output)
				{
					b |= 1;
				}
				TdsColumnType tdsColumnType = tdsMetaParameter2.GetMetaType();
				base.Comm.Append((byte)empty2.Length);
				base.Comm.Append(empty2);
				base.Comm.Append(b);
				base.Comm.Append(num2);
				base.Comm.Append((byte)tdsColumnType);
				if (!Tds.IsFixedSizeColumn(tdsColumnType))
				{
					base.Comm.Append((byte)tdsMetaParameter2.Size);
				}
				if (tdsColumnType == TdsColumnType.Numeric || tdsColumnType == TdsColumnType.Decimal)
				{
					base.Comm.Append(tdsMetaParameter2.Precision);
					base.Comm.Append(tdsMetaParameter2.Scale);
				}
				base.Comm.Append((byte)empty.Length);
				base.Comm.Append(empty);
			}
		}

		private void SendParams()
		{
			base.Comm.Append(215);
			foreach (object obj in ((IEnumerable)base.Parameters))
			{
				TdsMetaParameter tdsMetaParameter = (TdsMetaParameter)obj;
				TdsColumnType metaType = tdsMetaParameter.GetMetaType();
				bool flag = tdsMetaParameter.Value == DBNull.Value || tdsMetaParameter.Value == null;
				if (!Tds.IsFixedSizeColumn(metaType))
				{
					base.Comm.Append((byte)tdsMetaParameter.GetActualSize());
				}
				if (!flag)
				{
					base.Comm.Append(tdsMetaParameter.Value);
				}
			}
		}

		public override void Unprepare(string statementId)
		{
			base.Comm.StartPacket(TdsPacketType.Normal);
			base.Comm.Append(231);
			base.Comm.Append((short)(3 + statementId.Length));
			base.Comm.Append(4);
			base.Comm.Append(0);
			base.Comm.Append((byte)statementId.Length);
			base.Comm.Append(statementId);
			base.MoreResults = true;
			base.Comm.SendPacket();
			base.SkipToEnd();
		}

		protected override bool IsValidRowCount(byte status, byte op)
		{
			if (this.isSelectQuery)
			{
				return this.isSelectQuery = false;
			}
			return (status & 64) == 0 && (status & 16) != 0;
		}

		public static readonly TdsVersion Version = TdsVersion.tds50;

		private int packetSize;

		private bool isSelectQuery;
	}
}
