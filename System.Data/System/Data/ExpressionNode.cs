using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;

namespace System.Data
{
	internal abstract class ExpressionNode
	{
		protected ExpressionNode(DataTable table)
		{
			this._table = table;
		}

		internal IFormatProvider FormatProvider
		{
			get
			{
				if (this._table == null)
				{
					return CultureInfo.CurrentCulture;
				}
				return this._table.FormatProvider;
			}
		}

		internal virtual bool IsSqlColumn
		{
			get
			{
				return false;
			}
		}

		protected DataTable table
		{
			get
			{
				return this._table;
			}
		}

		protected void BindTable(DataTable table)
		{
			this._table = table;
		}

		internal abstract void Bind(DataTable table, List<DataColumn> list);

		internal abstract object Eval();

		internal abstract object Eval(DataRow row, DataRowVersion version);

		internal abstract object Eval(int[] recordNos);

		internal abstract bool IsConstant();

		internal abstract bool IsTableConstant();

		internal abstract bool HasLocalAggregate();

		internal abstract bool HasRemoteAggregate();

		internal abstract ExpressionNode Optimize();

		internal virtual bool DependsOn(DataColumn column)
		{
			return false;
		}

		internal static bool IsInteger(StorageType type)
		{
			return type == StorageType.Int16 || type == StorageType.Int32 || type == StorageType.Int64 || type == StorageType.UInt16 || type == StorageType.UInt32 || type == StorageType.UInt64 || type == StorageType.SByte || type == StorageType.Byte;
		}

		internal static bool IsIntegerSql(StorageType type)
		{
			return type == StorageType.Int16 || type == StorageType.Int32 || type == StorageType.Int64 || type == StorageType.UInt16 || type == StorageType.UInt32 || type == StorageType.UInt64 || type == StorageType.SByte || type == StorageType.Byte || type == StorageType.SqlInt64 || type == StorageType.SqlInt32 || type == StorageType.SqlInt16 || type == StorageType.SqlByte;
		}

		internal static bool IsSigned(StorageType type)
		{
			return type == StorageType.Int16 || type == StorageType.Int32 || type == StorageType.Int64 || type == StorageType.SByte || ExpressionNode.IsFloat(type);
		}

		internal static bool IsSignedSql(StorageType type)
		{
			return type == StorageType.Int16 || type == StorageType.Int32 || type == StorageType.Int64 || type == StorageType.SByte || type == StorageType.SqlInt64 || type == StorageType.SqlInt32 || type == StorageType.SqlInt16 || ExpressionNode.IsFloatSql(type);
		}

		internal static bool IsUnsigned(StorageType type)
		{
			return type == StorageType.UInt16 || type == StorageType.UInt32 || type == StorageType.UInt64 || type == StorageType.Byte;
		}

		internal static bool IsUnsignedSql(StorageType type)
		{
			return type == StorageType.UInt16 || type == StorageType.UInt32 || type == StorageType.UInt64 || type == StorageType.SqlByte || type == StorageType.Byte;
		}

		internal static bool IsNumeric(StorageType type)
		{
			return ExpressionNode.IsFloat(type) || ExpressionNode.IsInteger(type);
		}

		internal static bool IsNumericSql(StorageType type)
		{
			return ExpressionNode.IsFloatSql(type) || ExpressionNode.IsIntegerSql(type);
		}

		internal static bool IsFloat(StorageType type)
		{
			return type == StorageType.Single || type == StorageType.Double || type == StorageType.Decimal;
		}

		internal static bool IsFloatSql(StorageType type)
		{
			return type == StorageType.Single || type == StorageType.Double || type == StorageType.Decimal || type == StorageType.SqlDouble || type == StorageType.SqlDecimal || type == StorageType.SqlMoney || type == StorageType.SqlSingle;
		}

		private DataTable _table;
	}
}
