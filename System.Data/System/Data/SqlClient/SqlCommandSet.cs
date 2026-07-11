using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.SqlClient
{
	internal sealed class SqlCommandSet
	{
		internal SqlCommandSet()
		{
			this._batchCommand = new SqlCommand();
		}

		private SqlCommand BatchCommand
		{
			get
			{
				SqlCommand batchCommand = this._batchCommand;
				if (batchCommand == null)
				{
					throw ADP.ObjectDisposed(this);
				}
				return batchCommand;
			}
		}

		internal int CommandCount
		{
			get
			{
				return this.CommandList.Count;
			}
		}

		private List<SqlCommandSet.LocalCommand> CommandList
		{
			get
			{
				List<SqlCommandSet.LocalCommand> commandList = this._commandList;
				if (commandList == null)
				{
					throw ADP.ObjectDisposed(this);
				}
				return commandList;
			}
		}

		internal int CommandTimeout
		{
			set
			{
				this.BatchCommand.CommandTimeout = value;
			}
		}

		internal SqlConnection Connection
		{
			get
			{
				return this.BatchCommand.Connection;
			}
			set
			{
				this.BatchCommand.Connection = value;
			}
		}

		internal SqlTransaction Transaction
		{
			set
			{
				this.BatchCommand.Transaction = value;
			}
		}

		internal void Append(SqlCommand command)
		{
			ADP.CheckArgumentNull(command, "command");
			string commandText = command.CommandText;
			if (string.IsNullOrEmpty(commandText))
			{
				throw ADP.CommandTextRequired("Append");
			}
			CommandType commandType = command.CommandType;
			if (commandType == CommandType.Text || commandType == CommandType.StoredProcedure)
			{
				SqlParameterCollection sqlParameterCollection = null;
				SqlParameterCollection parameters = command.Parameters;
				if (0 < parameters.Count)
				{
					sqlParameterCollection = new SqlParameterCollection();
					for (int i = 0; i < parameters.Count; i++)
					{
						SqlParameter sqlParameter = new SqlParameter();
						parameters[i].CopyTo(sqlParameter);
						sqlParameterCollection.Add(sqlParameter);
						if (!SqlCommandSet.s_sqlIdentifierParser.IsMatch(sqlParameter.ParameterName))
						{
							throw ADP.BadParameterName(sqlParameter.ParameterName);
						}
					}
					foreach (object obj in sqlParameterCollection)
					{
						SqlParameter sqlParameter2 = (SqlParameter)obj;
						object value = sqlParameter2.Value;
						byte[] array = value as byte[];
						if (array != null)
						{
							int offset = sqlParameter2.Offset;
							int size = sqlParameter2.Size;
							int num = array.Length - offset;
							if (size != 0 && size < num)
							{
								num = size;
							}
							byte[] array2 = new byte[Math.Max(num, 0)];
							Buffer.BlockCopy(array, offset, array2, 0, array2.Length);
							sqlParameter2.Offset = 0;
							sqlParameter2.Value = array2;
						}
						else
						{
							char[] array3 = value as char[];
							if (array3 != null)
							{
								int offset2 = sqlParameter2.Offset;
								int size2 = sqlParameter2.Size;
								int num2 = array3.Length - offset2;
								if (size2 != 0 && size2 < num2)
								{
									num2 = size2;
								}
								char[] array4 = new char[Math.Max(num2, 0)];
								Buffer.BlockCopy(array3, offset2, array4, 0, array4.Length * 2);
								sqlParameter2.Offset = 0;
								sqlParameter2.Value = array4;
							}
							else
							{
								ICloneable cloneable = value as ICloneable;
								if (cloneable != null)
								{
									sqlParameter2.Value = cloneable.Clone();
								}
							}
						}
					}
				}
				int num3 = -1;
				if (sqlParameterCollection != null)
				{
					for (int j = 0; j < sqlParameterCollection.Count; j++)
					{
						if (ParameterDirection.ReturnValue == sqlParameterCollection[j].Direction)
						{
							num3 = j;
							break;
						}
					}
				}
				SqlCommandSet.LocalCommand localCommand = new SqlCommandSet.LocalCommand(commandText, sqlParameterCollection, num3, command.CommandType);
				this.CommandList.Add(localCommand);
				return;
			}
			if (commandType == CommandType.TableDirect)
			{
				throw SQL.NotSupportedCommandType(commandType);
			}
			throw ADP.InvalidCommandType(commandType);
		}

		internal static void BuildStoredProcedureName(StringBuilder builder, string part)
		{
			if (part != null && 0 < part.Length)
			{
				if ('[' == part[0])
				{
					int num = 0;
					foreach (char c in part)
					{
						if (']' == c)
						{
							num++;
						}
					}
					if (1 == num % 2)
					{
						builder.Append(part);
						return;
					}
				}
				SqlServerEscapeHelper.EscapeIdentifier(builder, part);
			}
		}

		internal void Clear()
		{
			DbCommand batchCommand = this.BatchCommand;
			if (batchCommand != null)
			{
				batchCommand.Parameters.Clear();
				batchCommand.CommandText = null;
			}
			List<SqlCommandSet.LocalCommand> commandList = this._commandList;
			if (commandList != null)
			{
				commandList.Clear();
			}
		}

		internal void Dispose()
		{
			SqlCommand batchCommand = this._batchCommand;
			this._commandList = null;
			this._batchCommand = null;
			if (batchCommand != null)
			{
				batchCommand.Dispose();
			}
		}

		internal int ExecuteNonQuery()
		{
			this.ValidateCommandBehavior("ExecuteNonQuery", CommandBehavior.Default);
			this.BatchCommand.BatchRPCMode = true;
			this.BatchCommand.ClearBatchCommand();
			this.BatchCommand.Parameters.Clear();
			for (int i = 0; i < this._commandList.Count; i++)
			{
				SqlCommandSet.LocalCommand localCommand = this._commandList[i];
				this.BatchCommand.AddBatchCommand(localCommand.CommandText, localCommand.Parameters, localCommand.CmdType);
			}
			return this.BatchCommand.ExecuteBatchRPCCommand();
		}

		internal SqlParameter GetParameter(int commandIndex, int parameterIndex)
		{
			return this.CommandList[commandIndex].Parameters[parameterIndex];
		}

		internal bool GetBatchedAffected(int commandIdentifier, out int recordsAffected, out Exception error)
		{
			error = this.BatchCommand.GetErrors(commandIdentifier);
			int? recordsAffected2 = this.BatchCommand.GetRecordsAffected(commandIdentifier);
			recordsAffected = recordsAffected2.GetValueOrDefault();
			return recordsAffected2 != null;
		}

		internal int GetParameterCount(int commandIndex)
		{
			return this.CommandList[commandIndex].Parameters.Count;
		}

		private void ValidateCommandBehavior(string method, CommandBehavior behavior)
		{
			if ((behavior & ~(CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection)) != CommandBehavior.Default)
			{
				ADP.ValidateCommandBehavior(behavior);
				throw ADP.NotSupportedCommandBehavior(behavior & ~(CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection), method);
			}
		}

		private const string SqlIdentifierPattern = "^@[\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}\\p{Nd}\uff3f_@#\\$]*$";

		private static readonly Regex s_sqlIdentifierParser = new Regex("^@[\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}\\p{Nd}\uff3f_@#\\$]*$", RegexOptions.ExplicitCapture | RegexOptions.Singleline);

		private List<SqlCommandSet.LocalCommand> _commandList = new List<SqlCommandSet.LocalCommand>();

		private SqlCommand _batchCommand;

		private sealed class LocalCommand
		{
			internal LocalCommand(string commandText, SqlParameterCollection parameters, int returnParameterIndex, CommandType cmdType)
			{
				this.CommandText = commandText;
				this.Parameters = parameters;
				this.ReturnParameterIndex = returnParameterIndex;
				this.CmdType = cmdType;
			}

			internal readonly string CommandText;

			internal readonly SqlParameterCollection Parameters;

			internal readonly int ReturnParameterIndex;

			internal readonly CommandType CmdType;
		}
	}
}
