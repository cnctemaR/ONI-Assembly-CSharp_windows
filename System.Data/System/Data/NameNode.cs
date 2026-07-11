using System;
using System.Collections.Generic;
using System.Data.Common;

namespace System.Data
{
	internal sealed class NameNode : ExpressionNode
	{
		internal NameNode(DataTable table, char[] text, int start, int pos)
			: base(table)
		{
			this._name = NameNode.ParseName(text, start, pos);
		}

		internal NameNode(DataTable table, string name)
			: base(table)
		{
			this._name = name;
		}

		internal override bool IsSqlColumn
		{
			get
			{
				return this._column.IsSqlType;
			}
		}

		internal override void Bind(DataTable table, List<DataColumn> list)
		{
			base.BindTable(table);
			if (table == null)
			{
				throw ExprException.UnboundName(this._name);
			}
			try
			{
				this._column = table.Columns[this._name];
			}
			catch (Exception ex)
			{
				this._found = false;
				if (!ADP.IsCatchableExceptionType(ex))
				{
					throw;
				}
				throw ExprException.UnboundName(this._name);
			}
			if (this._column == null)
			{
				throw ExprException.UnboundName(this._name);
			}
			this._name = this._column.ColumnName;
			this._found = true;
			int i;
			for (i = 0; i < list.Count; i++)
			{
				DataColumn dataColumn = list[i];
				if (this._column == dataColumn)
				{
					break;
				}
			}
			if (i >= list.Count)
			{
				list.Add(this._column);
			}
		}

		internal override object Eval()
		{
			throw ExprException.EvalNoContext();
		}

		internal override object Eval(DataRow row, DataRowVersion version)
		{
			if (!this._found)
			{
				throw ExprException.UnboundName(this._name);
			}
			if (row != null)
			{
				return this._column[row.GetRecordFromVersion(version)];
			}
			if (this.IsTableConstant())
			{
				return this._column.DataExpression.Evaluate();
			}
			throw ExprException.UnboundName(this._name);
		}

		internal override object Eval(int[] records)
		{
			throw ExprException.ComputeNotAggregate(this.ToString());
		}

		internal override bool IsConstant()
		{
			return false;
		}

		internal override bool IsTableConstant()
		{
			return this._column != null && this._column.Computed && this._column.DataExpression.IsTableAggregate();
		}

		internal override bool HasLocalAggregate()
		{
			return this._column != null && this._column.Computed && this._column.DataExpression.HasLocalAggregate();
		}

		internal override bool HasRemoteAggregate()
		{
			return this._column != null && this._column.Computed && this._column.DataExpression.HasRemoteAggregate();
		}

		internal override bool DependsOn(DataColumn column)
		{
			return this._column == column || (this._column.Computed && this._column.DataExpression.DependsOn(column));
		}

		internal override ExpressionNode Optimize()
		{
			return this;
		}

		internal static string ParseName(char[] text, int start, int pos)
		{
			char c = '\0';
			string text2 = string.Empty;
			int num = start;
			int num2 = pos;
			checked
			{
				if (text[start] == '`')
				{
					start++;
					pos--;
					c = '\\';
					text2 = "`";
				}
				else if (text[start] == '[')
				{
					start++;
					pos--;
					c = '\\';
					text2 = "]\\";
				}
			}
			if (c != '\0')
			{
				int num3 = start;
				for (int i = start; i < pos; i++)
				{
					if (text[i] == c && i + 1 < pos && text2.IndexOf(text[i + 1]) >= 0)
					{
						i++;
					}
					text[num3] = text[i];
					num3++;
				}
				pos = num3;
			}
			if (pos == start)
			{
				throw ExprException.InvalidName(new string(text, num, num2 - num));
			}
			return new string(text, start, pos - start);
		}

		internal char _open;

		internal char _close;

		internal string _name;

		internal bool _found;

		internal bool _type;

		internal DataColumn _column;
	}
}
