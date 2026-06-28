using System;
using System.Collections;
using System.Globalization;
using System.Text;
using Mono.Security.Protocol.Ntlm;

namespace Mono.Data.Tds.Protocol
{
	public class Tds70 : Tds
	{
		public Tds70(string server, int port)
			: this(server, port, 512, 15)
		{
		}

		public Tds70(string server, int port, int packetSize, int timeout)
			: base(server, port, packetSize, timeout, TdsVersion.tds70)
		{
		}

		public Tds70(string server, int port, int packetSize, int timeout, TdsVersion version)
			: base(server, port, packetSize, timeout, version)
		{
		}

		protected virtual byte[] ClientVersion
		{
			get
			{
				return new byte[] { 0, 0, 0, 112 };
			}
		}

		private string BuildExec(string sql)
		{
			string text = sql.Replace("'", "''");
			if (base.Parameters != null && base.Parameters.Count > 0)
			{
				return this.BuildProcedureCall(string.Format("sp_executesql N'{0}', N'{1}', ", text, this.BuildPreparedParameters()));
			}
			return this.BuildProcedureCall(string.Format("sp_executesql N'{0}'", text));
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
				string text = tdsMetaParameter.ParameterName;
				if (text[0] == '@')
				{
					text = text.Substring(1);
				}
				if (tdsMetaParameter.Direction != TdsParameterDirection.ReturnValue)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					if (tdsMetaParameter.Direction == TdsParameterDirection.InputOutput)
					{
						stringBuilder.AppendFormat("@{0}={0} output", text);
					}
					else
					{
						stringBuilder.Append(this.FormatParameter(tdsMetaParameter));
					}
				}
			}
			return stringBuilder.ToString();
		}

		private string BuildPreparedParameters()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in ((IEnumerable)base.Parameters))
			{
				TdsMetaParameter tdsMetaParameter = (TdsMetaParameter)obj;
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(tdsMetaParameter.Prepare());
				if (tdsMetaParameter.Direction == TdsParameterDirection.Output)
				{
					stringBuilder.Append(" output");
				}
			}
			return stringBuilder.ToString();
		}

		private string BuildPreparedQuery(string id)
		{
			return this.BuildProcedureCall(string.Format("sp_execute {0},", id));
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
					string text2 = tdsMetaParameter.ParameterName;
					if (text2[0] == '@')
					{
						text2 = text2.Substring(1);
					}
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
						stringBuilder2.Append("@" + text2);
						stringBuilder.Append(string.Format("declare {0}\n", tdsMetaParameter.Prepare()));
						if (tdsMetaParameter.Direction != TdsParameterDirection.ReturnValue)
						{
							if (tdsMetaParameter.Direction == TdsParameterDirection.InputOutput)
							{
								stringBuilder3.Append(string.Format("set {0}\n", this.FormatParameter(tdsMetaParameter)));
							}
							else
							{
								stringBuilder3.Append(string.Format("set @{0}=NULL\n", text2));
							}
						}
						num++;
					}
					if (tdsMetaParameter.Direction == TdsParameterDirection.ReturnValue)
					{
						text = "@" + text2 + "=";
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

		public override bool Connect(TdsConnectionParameters connectionParameters)
		{
			if (base.IsConnected)
			{
				throw new InvalidOperationException("The connection is already open.");
			}
			this.connectionParms = connectionParameters;
			base.SetLanguage(connectionParameters.Language);
			base.SetCharset("utf-8");
			byte[] array = new byte[0];
			short num = 0;
			byte b = 0;
			byte[] array2 = new byte[]
			{
				6, 125, 15, 253, byte.MaxValue, 0, 0, 0, 0, 224,
				131, 0, 0, 104, 1, 0, 0, 9, 4, 0,
				0
			};
			byte[] array3 = new byte[21];
			array3[0] = 6;
			array3[9] = 224;
			array3[10] = 3;
			byte[] array4 = array3;
			byte[] array5;
			if (connectionParameters.DomainLogin)
			{
				array5 = array2;
			}
			else
			{
				array5 = array4;
			}
			string text = connectionParameters.User;
			int num2 = text.IndexOf("\\");
			string text2;
			if (num2 != -1)
			{
				text2 = text.Substring(0, num2);
				text = text.Substring(num2 + 1);
				connectionParameters.DefaultDomain = text2;
				connectionParameters.User = text;
			}
			else
			{
				text2 = Environment.UserDomainName;
				connectionParameters.DefaultDomain = text2;
			}
			short num3 = (short)(86 + (connectionParameters.Hostname.Length + connectionParameters.ApplicationName.Length + base.DataSource.Length + connectionParameters.LibraryName.Length + base.Language.Length + connectionParameters.Database.Length + connectionParameters.AttachDBFileName.Length) * 2);
			if (connectionParameters.DomainLogin)
			{
				num = (short)(32 + (connectionParameters.Hostname.Length + text2.Length));
				num3 += num;
			}
			else
			{
				num3 += (short)((text.Length + connectionParameters.Password.Length) * 2);
			}
			int num4 = (int)num3;
			base.Comm.StartPacket(TdsPacketType.Logon70);
			base.Comm.Append(num4);
			base.Comm.Append(this.ClientVersion);
			base.Comm.Append(base.PacketSize);
			base.Comm.Append(array, 3, b);
			base.Comm.Append(array5);
			short num5 = 86;
			base.Comm.Append(num5);
			base.Comm.Append((short)connectionParameters.Hostname.Length);
			num5 += (short)(connectionParameters.Hostname.Length * 2);
			if (connectionParameters.DomainLogin)
			{
				base.Comm.Append(0);
				base.Comm.Append(0);
				base.Comm.Append(0);
				base.Comm.Append(0);
			}
			else
			{
				base.Comm.Append(num5);
				base.Comm.Append((short)text.Length);
				num5 += (short)(text.Length * 2);
				base.Comm.Append(num5);
				base.Comm.Append((short)connectionParameters.Password.Length);
				num5 += (short)(connectionParameters.Password.Length * 2);
			}
			base.Comm.Append(num5);
			base.Comm.Append((short)connectionParameters.ApplicationName.Length);
			num5 += (short)(connectionParameters.ApplicationName.Length * 2);
			base.Comm.Append(num5);
			base.Comm.Append((short)base.DataSource.Length);
			num5 += (short)(base.DataSource.Length * 2);
			base.Comm.Append(num5);
			base.Comm.Append(0);
			base.Comm.Append(num5);
			base.Comm.Append((short)connectionParameters.LibraryName.Length);
			num5 += (short)(connectionParameters.LibraryName.Length * 2);
			base.Comm.Append(num5);
			base.Comm.Append((short)base.Language.Length);
			num5 += (short)(base.Language.Length * 2);
			base.Comm.Append(num5);
			base.Comm.Append((short)connectionParameters.Database.Length);
			num5 += (short)(connectionParameters.Database.Length * 2);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(num5);
			if (connectionParameters.DomainLogin)
			{
				base.Comm.Append(num);
				num5 += num;
			}
			else
			{
				base.Comm.Append(0);
			}
			base.Comm.Append(num5);
			base.Comm.Append((short)connectionParameters.AttachDBFileName.Length);
			num5 += (short)(connectionParameters.AttachDBFileName.Length * 2);
			base.Comm.Append(connectionParameters.Hostname);
			if (!connectionParameters.DomainLogin)
			{
				base.Comm.Append(connectionParameters.User);
				string text3 = Tds70.EncryptPassword(connectionParameters.Password);
				base.Comm.Append(text3);
			}
			base.Comm.Append(connectionParameters.ApplicationName);
			base.Comm.Append(base.DataSource);
			base.Comm.Append(connectionParameters.LibraryName);
			base.Comm.Append(base.Language);
			base.Comm.Append(connectionParameters.Database);
			if (connectionParameters.DomainLogin)
			{
				Type1Message type1Message = new Type1Message();
				type1Message.Domain = text2;
				type1Message.Host = connectionParameters.Hostname;
				type1Message.Flags = NtlmFlags.NegotiateUnicode | NtlmFlags.NegotiateNtlm | NtlmFlags.NegotiateDomainSupplied | NtlmFlags.NegotiateWorkstationSupplied | NtlmFlags.NegotiateAlwaysSign;
				base.Comm.Append(type1Message.GetBytes());
			}
			base.Comm.Append(connectionParameters.AttachDBFileName);
			base.Comm.SendPacket();
			base.MoreResults = true;
			base.SkipToEnd();
			return base.IsConnected;
		}

		private static string EncryptPassword(string pass)
		{
			int num = 23130;
			int length = pass.Length;
			char[] array = new char[length];
			for (int i = 0; i < length; i++)
			{
				int num2 = (int)pass[i] ^ num;
				int num3 = (num2 >> 4) & 3855;
				int num4 = (num2 << 4) & 61680;
				array[i] = (char)(num3 | num4);
			}
			return new string(array);
		}

		public override bool Reset()
		{
			if (!base.Comm.IsConnected())
			{
				return false;
			}
			base.Comm.ResetConnection = true;
			base.Reset();
			return true;
		}

		public override void ExecPrepared(string commandText, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.Parameters = parameters;
			if (base.Parameters != null && base.Parameters.Count > 0)
			{
				this.ExecRPC(TdsRpcProcId.ExecuteSql, commandText, parameters, timeout, wantResults);
			}
			else
			{
				base.ExecuteQuery(this.BuildPreparedQuery(commandText), timeout, wantResults);
			}
		}

		public override void ExecProc(string commandText, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.Parameters = parameters;
			this.ExecRPC(commandText, parameters, timeout, wantResults);
		}

		private void WriteRpcParameterInfo(TdsMetaParameterCollection parameters)
		{
			if (parameters != null)
			{
				foreach (object obj in ((IEnumerable)parameters))
				{
					TdsMetaParameter tdsMetaParameter = (TdsMetaParameter)obj;
					if (tdsMetaParameter.Direction != TdsParameterDirection.ReturnValue)
					{
						string parameterName = tdsMetaParameter.ParameterName;
						if (parameterName != null && parameterName.Length > 0 && parameterName[0] == '@')
						{
							base.Comm.Append((byte)parameterName.Length);
							base.Comm.Append(parameterName);
						}
						else
						{
							base.Comm.Append((byte)(parameterName.Length + 1));
							base.Comm.Append("@" + parameterName);
						}
						short num = 0;
						if (tdsMetaParameter.Direction != TdsParameterDirection.Input)
						{
							num |= 1;
						}
						base.Comm.Append((byte)num);
						this.WriteParameterInfo(tdsMetaParameter);
					}
				}
			}
		}

		private void WritePreparedParameterInfo(TdsMetaParameterCollection parameters)
		{
			if (parameters == null)
			{
				return;
			}
			string text = this.BuildPreparedParameters();
			base.Comm.Append(0);
			base.Comm.Append(0);
			this.WriteParameterInfo(new TdsMetaParameter("prep_params", (text.Length <= 4000) ? "nvarchar" : "ntext", text));
		}

		private void ExecRPC(TdsRpcProcId rpcId, string sql, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.InitExec();
			base.Comm.StartPacket(TdsPacketType.Proc);
			base.Comm.Append(ushort.MaxValue);
			base.Comm.Append((ushort)rpcId);
			base.Comm.Append(2);
			base.Comm.Append(0);
			base.Comm.Append(0);
			TdsMetaParameter tdsMetaParameter = new TdsMetaParameter("sql", (sql.Length <= 4000) ? "nvarchar" : "ntext", sql);
			this.WriteParameterInfo(tdsMetaParameter);
			this.WritePreparedParameterInfo(parameters);
			this.WriteRpcParameterInfo(parameters);
			base.Comm.SendPacket();
			base.CheckForData(timeout);
			if (!wantResults)
			{
				base.SkipToEnd();
			}
		}

		protected override void ExecRPC(string rpcName, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.InitExec();
			base.Comm.StartPacket(TdsPacketType.Proc);
			base.Comm.Append((short)rpcName.Length);
			base.Comm.Append(rpcName);
			base.Comm.Append(0);
			this.WriteRpcParameterInfo(parameters);
			base.Comm.SendPacket();
			base.CheckForData(timeout);
			if (!wantResults)
			{
				base.SkipToEnd();
			}
		}

		private void WriteParameterInfo(TdsMetaParameter param)
		{
			param.IsNullable = true;
			TdsColumnType tdsColumnType = param.GetMetaType();
			param.IsNullable = false;
			bool flag = false;
			int num = param.Size;
			if (num < 1)
			{
				if (num < 0)
				{
					flag = true;
				}
				num = param.GetActualSize();
			}
			TdsColumnType tdsColumnType2 = tdsColumnType;
			if (tdsColumnType == TdsColumnType.BigNVarChar)
			{
				if (num == param.Size)
				{
					num <<= 1;
				}
				if (num >> 1 > 4000)
				{
					tdsColumnType = TdsColumnType.NText;
				}
			}
			else if (tdsColumnType == TdsColumnType.BigVarChar)
			{
				if (num > 8000)
				{
					tdsColumnType = TdsColumnType.Text;
				}
			}
			else if (tdsColumnType == TdsColumnType.BigVarBinary && num > 8000)
			{
				tdsColumnType = TdsColumnType.Image;
			}
			if (base.TdsVersion > TdsVersion.tds81 && flag)
			{
				base.Comm.Append((byte)tdsColumnType2);
				base.Comm.Append(-1);
			}
			else if (base.ServerTdsVersion > TdsVersion.tds70 && tdsColumnType2 == TdsColumnType.Decimal)
			{
				base.Comm.Append(108);
			}
			else
			{
				base.Comm.Append((byte)tdsColumnType);
			}
			if (base.IsLargeType(tdsColumnType))
			{
				base.Comm.Append((short)num);
			}
			else if (base.IsBlobType(tdsColumnType))
			{
				base.Comm.Append(num);
			}
			else
			{
				base.Comm.Append((byte)num);
			}
			if (param.TypeName == "decimal" || param.TypeName == "numeric")
			{
				base.Comm.Append((param.Precision == 0) ? 29 : param.Precision);
				base.Comm.Append(param.Scale);
				if (param.Value != null && param.Value != DBNull.Value && (decimal)param.Value != 79228162514264337593543950335m && (decimal)param.Value != -79228162514264337593543950335m)
				{
					decimal num2 = new decimal(Math.Pow(10.0, (double)param.Scale));
					int num3 = (int)((decimal)param.Value * num2);
					param.Value = num3;
				}
			}
			if (base.Collation != null && (tdsColumnType == TdsColumnType.BigChar || tdsColumnType == TdsColumnType.BigNVarChar || tdsColumnType == TdsColumnType.BigVarChar || tdsColumnType == TdsColumnType.NChar || tdsColumnType == TdsColumnType.NVarChar || tdsColumnType == TdsColumnType.Text || tdsColumnType == TdsColumnType.NText))
			{
				base.Comm.Append(base.Collation);
			}
			if ((tdsColumnType == TdsColumnType.BigVarChar || tdsColumnType == TdsColumnType.BigNVarChar || tdsColumnType == TdsColumnType.BigVarBinary) && (param.Value == null || param.Value == DBNull.Value))
			{
				num = -1;
			}
			else
			{
				num = param.GetActualSize();
			}
			if (base.IsLargeType(tdsColumnType))
			{
				base.Comm.Append((short)num);
			}
			else if (base.IsBlobType(tdsColumnType))
			{
				base.Comm.Append(num);
			}
			else
			{
				base.Comm.Append((byte)num);
			}
			if (num > 0)
			{
				string typeName = param.TypeName;
				switch (typeName)
				{
				case "money":
				{
					decimal num5 = (decimal)param.Value;
					int[] bits = decimal.GetBits(num5);
					if (num5 >= 0m)
					{
						base.Comm.Append(bits[1]);
						base.Comm.Append(bits[0]);
					}
					else
					{
						base.Comm.Append(~bits[1]);
						base.Comm.Append(~bits[0] + 1);
					}
					return;
				}
				case "smallmoney":
				{
					decimal num6 = (decimal)param.Value;
					if (num6 < Tds70.SMALLMONEY_MIN || num6 > Tds70.SMALLMONEY_MAX)
					{
						throw new OverflowException(string.Format(CultureInfo.InvariantCulture, "Value '{0}' is not valid for SmallMoney.  Must be between {1:N4} and {2:N4}.", new object[]
						{
							num6,
							Tds70.SMALLMONEY_MIN,
							Tds70.SMALLMONEY_MAX
						}));
					}
					int[] bits2 = decimal.GetBits(num6);
					int num7 = ((!(num6 > 0m)) ? (-1) : 1);
					base.Comm.Append(num7 * bits2[0]);
					return;
				}
				case "datetime":
					base.Comm.Append((DateTime)param.Value, 8);
					return;
				case "smalldatetime":
					base.Comm.Append((DateTime)param.Value, 4);
					return;
				case "varchar":
				case "nvarchar":
				case "char":
				case "nchar":
				case "text":
				case "ntext":
				{
					byte[] bytes = param.GetBytes();
					base.Comm.Append(bytes);
					return;
				}
				case "uniqueidentifier":
					base.Comm.Append(((Guid)param.Value).ToByteArray());
					return;
				}
				base.Comm.Append(param.Value);
			}
		}

		public override void Execute(string commandText, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.Parameters = parameters;
			string text = commandText;
			if (base.Parameters != null && base.Parameters.Count > 0)
			{
				this.ExecRPC(TdsRpcProcId.ExecuteSql, commandText, parameters, timeout, wantResults);
			}
			else
			{
				if (wantResults)
				{
					text = this.BuildExec(commandText);
				}
				base.ExecuteQuery(text, timeout, wantResults);
			}
		}

		private string FormatParameter(TdsMetaParameter parameter)
		{
			string text = parameter.ParameterName;
			if (text[0] == '@')
			{
				text = text.Substring(1);
			}
			if (parameter.Direction == TdsParameterDirection.Output)
			{
				return string.Format("@{0}=@{0} output", text);
			}
			if (parameter.Value == null || parameter.Value == DBNull.Value)
			{
				return string.Format("@{0}=NULL", text);
			}
			string typeName = parameter.TypeName;
			string text2;
			switch (typeName)
			{
			case "smalldatetime":
			case "datetime":
			{
				DateTime dateTime = Convert.ToDateTime(parameter.Value);
				text2 = string.Format(base.Locale, "'{0:MMM dd yyyy hh:mm:ss.fff tt}'", new object[] { dateTime });
				goto IL_0327;
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
			{
				object obj = parameter.Value;
				Type type = obj.GetType();
				if (type.IsEnum)
				{
					obj = Convert.ChangeType(obj, Type.GetTypeCode(type));
				}
				text2 = obj.ToString();
				goto IL_0327;
			}
			case "nvarchar":
			case "nchar":
				text2 = string.Format("N'{0}'", parameter.Value.ToString().Replace("'", "''"));
				goto IL_0327;
			case "uniqueidentifier":
				text2 = string.Format("'{0}'", ((Guid)parameter.Value).ToString(string.Empty));
				goto IL_0327;
			case "bit":
				if (parameter.Value.GetType() == typeof(bool))
				{
					text2 = ((!(bool)parameter.Value) ? "0x0" : "0x1");
				}
				else
				{
					text2 = parameter.Value.ToString();
				}
				goto IL_0327;
			case "image":
			case "binary":
			case "varbinary":
			{
				byte[] array = (byte[])parameter.Value;
				if (array.Length == 0)
				{
					text2 = "0x";
				}
				else
				{
					text2 = string.Format("0x{0}", BitConverter.ToString(array).Replace("-", string.Empty).ToLower());
				}
				goto IL_0327;
			}
			}
			text2 = string.Format("'{0}'", parameter.Value.ToString().Replace("'", "''"));
			IL_0327:
			return "@" + text + "=" + text2;
		}

		public override string Prepare(string commandText, TdsMetaParameterCollection parameters)
		{
			base.Parameters = parameters;
			this.ExecProc("sp_prepare", new TdsMetaParameterCollection
			{
				new TdsMetaParameter("@Handle", "int", null)
				{
					Direction = TdsParameterDirection.Output
				},
				new TdsMetaParameter("@VarDecl", "nvarchar", this.BuildPreparedParameters()),
				new TdsMetaParameter("@Query", "nvarchar", commandText)
			}, 0, true);
			base.SkipToEnd();
			return base.OutputParameters[0].ToString();
		}

		protected override void ProcessColumnInfo()
		{
			int tdsShort = (int)base.Comm.GetTdsShort();
			for (int i = 0; i < tdsShort; i++)
			{
				byte[] array = new byte[4];
				for (int j = 0; j < 4; j++)
				{
					array[j] = base.Comm.GetByte();
				}
				bool flag = (array[2] & 1) > 0;
				bool flag2 = (array[2] & 12) > 0;
				bool flag3 = (array[2] & 16) > 0;
				bool flag4 = (array[2] & 16) > 0;
				TdsColumnType tdsColumnType = (TdsColumnType)(base.Comm.GetByte() & byte.MaxValue);
				byte b = 0;
				if (base.IsLargeType(tdsColumnType))
				{
					b = (byte)tdsColumnType;
					if (tdsColumnType != TdsColumnType.NChar)
					{
						tdsColumnType -= 128;
					}
				}
				string text = null;
				int num;
				if (base.IsBlobType(tdsColumnType))
				{
					num = base.Comm.GetTdsInt();
					text = base.Comm.GetString((int)base.Comm.GetTdsShort());
				}
				else if (Tds.IsFixedSizeColumn(tdsColumnType))
				{
					num = Tds.LookupBufferSize(tdsColumnType);
				}
				else if (base.IsLargeType((TdsColumnType)b))
				{
					num = (int)base.Comm.GetTdsShort();
				}
				else
				{
					num = (int)(base.Comm.GetByte() & byte.MaxValue);
				}
				if (base.IsWideType(tdsColumnType))
				{
					num /= 2;
				}
				byte b2;
				byte b3;
				if (tdsColumnType == TdsColumnType.Decimal || tdsColumnType == TdsColumnType.Numeric)
				{
					b2 = base.Comm.GetByte();
					b3 = base.Comm.GetByte();
				}
				else
				{
					b2 = this.GetPrecision(tdsColumnType, num);
					b3 = this.GetScale(tdsColumnType, num);
				}
				string @string = base.Comm.GetString((int)base.Comm.GetByte());
				TdsDataColumn tdsDataColumn = new TdsDataColumn();
				base.Columns.Add(tdsDataColumn);
				tdsDataColumn.ColumnType = new TdsColumnType?(tdsColumnType);
				tdsDataColumn.ColumnName = @string;
				tdsDataColumn.IsAutoIncrement = new bool?(flag3);
				tdsDataColumn.IsIdentity = new bool?(flag4);
				tdsDataColumn.ColumnSize = new int?(num);
				tdsDataColumn.NumericPrecision = new short?((short)b2);
				tdsDataColumn.NumericScale = new short?((short)b3);
				tdsDataColumn.IsReadOnly = new bool?(!flag2);
				tdsDataColumn.AllowDBNull = new bool?(flag);
				tdsDataColumn.BaseTableName = text;
				tdsDataColumn.DataTypeName = Enum.GetName(typeof(TdsColumnType), b);
			}
		}

		public override void Unprepare(string statementId)
		{
			this.ExecProc("sp_unprepare", new TdsMetaParameterCollection
			{
				new TdsMetaParameter("@P1", "int", int.Parse(statementId))
			}, 0, false);
		}

		protected override bool IsValidRowCount(byte status, byte op)
		{
			return (status & 16) != 0 && op != 193;
		}

		protected override void ProcessReturnStatus()
		{
			int tdsInt = base.Comm.GetTdsInt();
			if (base.Parameters != null)
			{
				foreach (object obj in ((IEnumerable)base.Parameters))
				{
					TdsMetaParameter tdsMetaParameter = (TdsMetaParameter)obj;
					if (tdsMetaParameter.Direction == TdsParameterDirection.ReturnValue)
					{
						tdsMetaParameter.Value = tdsInt;
						break;
					}
				}
			}
		}

		private byte GetScale(TdsColumnType type, int columnSize)
		{
			switch (type)
			{
			case TdsColumnType.DateTime4:
				return 0;
			default:
				if (type != TdsColumnType.DateTimeN)
				{
					return byte.MaxValue;
				}
				if (columnSize == 4)
				{
					return 0;
				}
				if (columnSize != 8)
				{
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Fixed scale not defined for column type '{0}' with size {1}.", new object[] { type, columnSize }));
				}
				return 3;
			case TdsColumnType.DateTime:
				return 3;
			}
		}

		private byte GetPrecision(TdsColumnType type, int columnSize)
		{
			switch (type)
			{
			case TdsColumnType.Void:
				return 1;
			default:
				switch (type)
				{
				case TdsColumnType.Variant:
					return byte.MaxValue;
				case TdsColumnType.NText:
					return byte.MaxValue;
				default:
					switch (type)
					{
					case TdsColumnType.BigVarBinary:
						return byte.MaxValue;
					default:
						switch (type)
						{
						case TdsColumnType.BigBinary:
							return byte.MaxValue;
						default:
							if (type == TdsColumnType.SmallMoney)
							{
								return 10;
							}
							if (type == TdsColumnType.BigInt)
							{
								return byte.MaxValue;
							}
							if (type == TdsColumnType.BigNVarChar)
							{
								return byte.MaxValue;
							}
							if (type == TdsColumnType.NChar)
							{
								return byte.MaxValue;
							}
							break;
						case TdsColumnType.BigChar:
							return byte.MaxValue;
						}
						break;
					case TdsColumnType.BigVarChar:
						return byte.MaxValue;
					}
					break;
				case TdsColumnType.NVarChar:
					return byte.MaxValue;
				case TdsColumnType.BitN:
					return byte.MaxValue;
				case TdsColumnType.FloatN:
					if (columnSize == 4)
					{
						return 7;
					}
					if (columnSize == 8)
					{
						return 15;
					}
					break;
				case TdsColumnType.MoneyN:
					if (columnSize == 4)
					{
						return 10;
					}
					if (columnSize == 8)
					{
						return 19;
					}
					break;
				case TdsColumnType.DateTimeN:
					if (columnSize == 4)
					{
						return 16;
					}
					if (columnSize == 8)
					{
						return 23;
					}
					break;
				case TdsColumnType.Money4:
					return 10;
				}
				break;
			case TdsColumnType.Image:
				return byte.MaxValue;
			case TdsColumnType.Text:
				return byte.MaxValue;
			case TdsColumnType.UniqueIdentifier:
				return byte.MaxValue;
			case TdsColumnType.VarBinary:
				return byte.MaxValue;
			case TdsColumnType.IntN:
				switch (columnSize)
				{
				case 1:
					return 3;
				case 2:
					return 5;
				case 4:
					return 10;
				}
				break;
			case TdsColumnType.VarChar:
				return byte.MaxValue;
			case TdsColumnType.Binary:
				return byte.MaxValue;
			case TdsColumnType.Char:
				return byte.MaxValue;
			case TdsColumnType.Int1:
				return 3;
			case TdsColumnType.Bit:
				return byte.MaxValue;
			case TdsColumnType.Int2:
				return 5;
			case TdsColumnType.Int4:
				return 10;
			case TdsColumnType.DateTime4:
				return 16;
			case TdsColumnType.Real:
				return 7;
			case TdsColumnType.Money:
				return 19;
			case TdsColumnType.DateTime:
				return 23;
			case TdsColumnType.Float8:
				return 15;
			}
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Fixed precision not defined for column type '{0}' with size {1}.", new object[] { type, columnSize }));
		}

		public override IAsyncResult BeginExecuteNonQuery(string cmdText, TdsMetaParameterCollection parameters, AsyncCallback callback, object state)
		{
			base.Parameters = parameters;
			string text = cmdText;
			if (base.Parameters != null && base.Parameters.Count > 0)
			{
				text = this.BuildExec(cmdText);
			}
			return base.BeginExecuteQueryInternal(text, false, callback, state);
		}

		public override void EndExecuteNonQuery(IAsyncResult ar)
		{
			base.EndExecuteQueryInternal(ar);
		}

		public override IAsyncResult BeginExecuteQuery(string cmdText, TdsMetaParameterCollection parameters, AsyncCallback callback, object state)
		{
			base.Parameters = parameters;
			string text = cmdText;
			if (base.Parameters != null && base.Parameters.Count > 0)
			{
				text = this.BuildExec(cmdText);
			}
			return base.BeginExecuteQueryInternal(text, true, callback, state);
		}

		public override void EndExecuteQuery(IAsyncResult ar)
		{
			base.EndExecuteQueryInternal(ar);
		}

		public override IAsyncResult BeginExecuteProcedure(string prolog, string epilog, string cmdText, bool IsNonQuery, TdsMetaParameterCollection parameters, AsyncCallback callback, object state)
		{
			base.Parameters = parameters;
			string text = this.BuildProcedureCall(cmdText);
			string text2 = string.Format("{0};{1};{2};", prolog, text, epilog);
			return base.BeginExecuteQueryInternal(text2, !IsNonQuery, callback, state);
		}

		public override void EndExecuteProcedure(IAsyncResult ar)
		{
			base.EndExecuteQueryInternal(ar);
		}

		private static readonly decimal SMALLMONEY_MIN = -214748.3648m;

		private static readonly decimal SMALLMONEY_MAX = 214748.3647m;
	}
}
