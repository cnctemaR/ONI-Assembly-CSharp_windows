using System;
using System.Collections.Generic;

namespace System.Data
{
	internal sealed class LookupNode : ExpressionNode
	{
		internal LookupNode(DataTable table, string columnName, string relationName)
			: base(table)
		{
			this._relationName = relationName;
			this._columnName = columnName;
		}

		internal override void Bind(DataTable table, List<DataColumn> list)
		{
			base.BindTable(table);
			this._column = null;
			this._relation = null;
			if (table == null)
			{
				throw ExprException.ExpressionUnbound(this.ToString());
			}
			DataRelationCollection parentRelations = table.ParentRelations;
			if (this._relationName == null)
			{
				if (parentRelations.Count > 1)
				{
					throw ExprException.UnresolvedRelation(table.TableName, this.ToString());
				}
				this._relation = parentRelations[0];
			}
			else
			{
				this._relation = parentRelations[this._relationName];
			}
			if (this._relation == null)
			{
				throw ExprException.BindFailure(this._relationName);
			}
			DataTable parentTable = this._relation.ParentTable;
			this._column = parentTable.Columns[this._columnName];
			if (this._column == null)
			{
				throw ExprException.UnboundName(this._columnName);
			}
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
			AggregateNode.Bind(this._relation, list);
		}

		internal override object Eval()
		{
			throw ExprException.EvalNoContext();
		}

		internal override object Eval(DataRow row, DataRowVersion version)
		{
			if (this._column == null || this._relation == null)
			{
				throw ExprException.ExpressionUnbound(this.ToString());
			}
			DataRow parentRow = row.GetParentRow(this._relation, version);
			if (parentRow == null)
			{
				return DBNull.Value;
			}
			return parentRow[this._column, parentRow.HasVersion(version) ? version : DataRowVersion.Current];
		}

		internal override object Eval(int[] recordNos)
		{
			throw ExprException.ComputeNotAggregate(this.ToString());
		}

		internal override bool IsConstant()
		{
			return false;
		}

		internal override bool IsTableConstant()
		{
			return false;
		}

		internal override bool HasLocalAggregate()
		{
			return false;
		}

		internal override bool HasRemoteAggregate()
		{
			return false;
		}

		internal override bool DependsOn(DataColumn column)
		{
			return this._column == column;
		}

		internal override ExpressionNode Optimize()
		{
			return this;
		}

		private readonly string _relationName;

		private readonly string _columnName;

		private DataColumn _column;

		private DataRelation _relation;
	}
}
