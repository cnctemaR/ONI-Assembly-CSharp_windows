using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;

namespace System.Data
{
	internal sealed class UnaryNode : ExpressionNode
	{
		internal UnaryNode(DataTable table, int op, ExpressionNode right)
			: base(table)
		{
			this._op = op;
			this._right = right;
		}

		internal override void Bind(DataTable table, List<DataColumn> list)
		{
			base.BindTable(table);
			this._right.Bind(table, list);
		}

		internal override object Eval()
		{
			return this.Eval(null, DataRowVersion.Default);
		}

		internal override object Eval(DataRow row, DataRowVersion version)
		{
			return this.EvalUnaryOp(this._op, this._right.Eval(row, version));
		}

		internal override object Eval(int[] recordNos)
		{
			return this._right.Eval(recordNos);
		}

		private object EvalUnaryOp(int op, object vl)
		{
			object value = DBNull.Value;
			if (DataExpression.IsUnknown(vl))
			{
				return DBNull.Value;
			}
			switch (op)
			{
			case 0:
				return vl;
			case 1:
			{
				StorageType storageType = DataStorage.GetStorageType(vl.GetType());
				if (ExpressionNode.IsNumericSql(storageType))
				{
					switch (storageType)
					{
					case StorageType.Byte:
						return (int)(-(int)((byte)vl));
					case StorageType.Int16:
						return (int)(-(int)((short)vl));
					case StorageType.UInt16:
					case StorageType.UInt32:
					case StorageType.UInt64:
						break;
					case StorageType.Int32:
						return -(int)vl;
					case StorageType.Int64:
						return -(long)vl;
					case StorageType.Single:
						return -(float)vl;
					case StorageType.Double:
						return -(double)vl;
					case StorageType.Decimal:
						return -(decimal)vl;
					default:
						switch (storageType)
						{
						case StorageType.SqlDecimal:
							return -(SqlDecimal)vl;
						case StorageType.SqlDouble:
							return -(SqlDouble)vl;
						case StorageType.SqlInt16:
							return -(SqlInt16)vl;
						case StorageType.SqlInt32:
							return -(SqlInt32)vl;
						case StorageType.SqlInt64:
							return -(SqlInt64)vl;
						case StorageType.SqlMoney:
							return -(SqlMoney)vl;
						case StorageType.SqlSingle:
							return -(SqlSingle)vl;
						}
						break;
					}
					return DBNull.Value;
				}
				throw ExprException.TypeMismatch(this.ToString());
			}
			case 2:
			{
				StorageType storageType = DataStorage.GetStorageType(vl.GetType());
				if (ExpressionNode.IsNumericSql(storageType))
				{
					return vl;
				}
				throw ExprException.TypeMismatch(this.ToString());
			}
			case 3:
				if (vl is SqlBoolean)
				{
					if (((SqlBoolean)vl).IsFalse)
					{
						return SqlBoolean.True;
					}
					if (((SqlBoolean)vl).IsTrue)
					{
						return SqlBoolean.False;
					}
					throw ExprException.UnsupportedOperator(op);
				}
				else
				{
					if (DataExpression.ToBoolean(vl))
					{
						return false;
					}
					return true;
				}
				break;
			default:
				throw ExprException.UnsupportedOperator(op);
			}
		}

		internal override bool IsConstant()
		{
			return this._right.IsConstant();
		}

		internal override bool IsTableConstant()
		{
			return this._right.IsTableConstant();
		}

		internal override bool HasLocalAggregate()
		{
			return this._right.HasLocalAggregate();
		}

		internal override bool HasRemoteAggregate()
		{
			return this._right.HasRemoteAggregate();
		}

		internal override bool DependsOn(DataColumn column)
		{
			return this._right.DependsOn(column);
		}

		internal override ExpressionNode Optimize()
		{
			this._right = this._right.Optimize();
			if (this.IsConstant())
			{
				object obj = this.Eval();
				return new ConstNode(base.table, ValueType.Object, obj, false);
			}
			return this;
		}

		internal readonly int _op;

		internal ExpressionNode _right;
	}
}
