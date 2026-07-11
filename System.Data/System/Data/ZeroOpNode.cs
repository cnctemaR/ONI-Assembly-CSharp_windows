using System;
using System.Collections.Generic;

namespace System.Data
{
	internal sealed class ZeroOpNode : ExpressionNode
	{
		internal ZeroOpNode(int op)
			: base(null)
		{
			this._op = op;
		}

		internal override void Bind(DataTable table, List<DataColumn> list)
		{
		}

		internal override object Eval()
		{
			switch (this._op)
			{
			case 32:
				return DBNull.Value;
			case 33:
				return true;
			case 34:
				return false;
			default:
				return DBNull.Value;
			}
		}

		internal override object Eval(DataRow row, DataRowVersion version)
		{
			return this.Eval();
		}

		internal override object Eval(int[] recordNos)
		{
			return this.Eval();
		}

		internal override bool IsConstant()
		{
			return true;
		}

		internal override bool IsTableConstant()
		{
			return true;
		}

		internal override bool HasLocalAggregate()
		{
			return false;
		}

		internal override bool HasRemoteAggregate()
		{
			return false;
		}

		internal override ExpressionNode Optimize()
		{
			return this;
		}

		internal readonly int _op;

		internal const int zop_True = 1;

		internal const int zop_False = 0;

		internal const int zop_Null = -1;
	}
}
